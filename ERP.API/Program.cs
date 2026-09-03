using System.Text;
using ERP.API.Hubs;
using ERP.API.Middlewares;
using ERP.API.Services;
using ERP.Application;
using ERP.Application.Interfaces;
using ERP.Infrastructure;
using ERP.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseStaticWebAssets();

// 1. Layer Dependencies
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// 2. Authentication & Authorization
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "ERP_Web_Secret_Key_Super_Secure_2026_DotNet10_BlazorWasm!";
var key = Encoding.UTF8.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ERP.API",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ERP.Client",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Support SignalR JWT in query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ModuloComercial", policy => policy.RequireClaim("Permissao", "Comercial:Ler", "Comercial:Criar"));
    options.AddPolicy("ModuloCompras", policy => policy.RequireClaim("Permissao", "Compras:Ler"));
    options.AddPolicy("ModuloFinanceiro", policy => policy.RequireClaim("Permissao", "Financeiro:Ler"));
    options.AddPolicy("ModuloEstoque", policy => policy.RequireClaim("Permissao", "Estoque:Ler"));
    options.AddPolicy("ModuloFaturamento", policy => policy.RequireClaim("Permissao", "Faturamento:Ler"));
    options.AddPolicy("ModuloProducao", policy => policy.RequireClaim("Permissao", "Producao:Ler"));
    options.AddPolicy("ModuloSistema", policy => policy.RequireRole("Administrador"));
});

// 3. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 4. Controllers & SignalR
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();

// 5. Swagger with Bearer Support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ERP Web API",
        Version = "v1.0",
        Description = "API RESTful do ERP Web (Blazor WebAssembly + MudBlazor + Clean Architecture)"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT no formato: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 6. DB Seeding
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.InitializeAsync(dbContext);
}

// 7. Pipeline Configuration
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ERP Web API v1.0");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

var clientDistPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "ERP.Client", "dist", "wwwroot"));

var contentTypeProvider = new FileExtensionContentTypeProvider();
contentTypeProvider.Mappings[".wasm"] = "application/wasm";
contentTypeProvider.Mappings[".dat"] = "application/octet-stream";
contentTypeProvider.Mappings[".pdb"] = "application/octet-stream";

if (Directory.Exists(clientDistPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(clientDistPath),
        RequestPath = "",
        ContentTypeProvider = contentTypeProvider
    });

    app.MapControllers();
    app.MapHub<ErpNotificationHub>("/hub/notifications");
    app.MapFallbackToFile("index.html", new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(clientDistPath),
        ContentTypeProvider = contentTypeProvider
    });
}
else
{
    app.UseBlazorFrameworkFiles();
    app.UseStaticFiles();
    app.MapControllers();
    app.MapHub<ErpNotificationHub>("/hub/notifications");
    app.MapFallbackToFile("index.html");
}

app.Run();
