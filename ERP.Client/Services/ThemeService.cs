using MudBlazor;

namespace ERP.Client.Services;

public static class ThemeService
{
    public static MudTheme CreateErpTheme()
    {
        return new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#2563eb",
                Secondary = "#0d9488",
                AppbarBackground = "#1e293b",
                AppbarText = "#f8fafc",
                Background = "#f8fafc",
                Surface = "#ffffff",
                DrawerBackground = "#0f172a",
                DrawerText = "#94a3b8",
                DrawerIcon = "#cbd5e1",
                LinesDefault = "#e2e8f0",
                TableLines = "#f1f5f9",
                TextPrimary = "#0f172a",
                TextSecondary = "#64748b",
                ActionDefault = "#475569",
                Info = "#3b82f6",
                Success = "#10b981",
                Warning = "#f59e0b",
                Error = "#ef4444"
            },
            PaletteDark = new PaletteDark
            {
                Primary = "#3b82f6",
                Secondary = "#14b8a6",
                AppbarBackground = "#0f172a",
                AppbarText = "#f8fafc",
                Background = "#090d16",
                Surface = "#111827",
                DrawerBackground = "#0b0f19",
                DrawerText = "#9ca3af",
                DrawerIcon = "#e5e7eb",
                LinesDefault = "#1f2937",
                TableLines = "#1f2937",
                TextPrimary = "#f9fafb",
                TextSecondary = "#9ca3af",
                ActionDefault = "#9ca3af",
                Info = "#60a5fa",
                Success = "#34d399",
                Warning = "#fbbf24",
                Error = "#f87171"
            },
            Typography = new Typography
            {
                Default = new DefaultTypography
                {
                    FontFamily = new[] { "Inter", "-apple-system", "BlinkMacSystemFont", "Segoe UI", "Roboto", "sans-serif" }
                },
                H1 = new H1Typography { FontFamily = new[] { "Outfit", "Inter", "sans-serif" }, FontWeight = "700" },
                H2 = new H2Typography { FontFamily = new[] { "Outfit", "Inter", "sans-serif" }, FontWeight = "700" },
                H3 = new H3Typography { FontFamily = new[] { "Outfit", "Inter", "sans-serif" }, FontWeight = "600" },
                H4 = new H4Typography { FontFamily = new[] { "Outfit", "Inter", "sans-serif" }, FontWeight = "600" },
                H5 = new H5Typography { FontFamily = new[] { "Outfit", "Inter", "sans-serif" }, FontWeight = "600" },
                H6 = new H6Typography { FontFamily = new[] { "Outfit", "Inter", "sans-serif" }, FontWeight = "600" },
                Button = new ButtonTypography { TextTransform = "none", FontWeight = "500" }
            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "8px"
            }
        };
    }
}
