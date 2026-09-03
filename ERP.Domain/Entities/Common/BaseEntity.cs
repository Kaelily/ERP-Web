namespace ERP.Domain.Entities.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
}

public interface IAuditableEntity
{
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public string? CriadoPor { get; set; }
    public string? AtualizadoPor { get; set; }
}
