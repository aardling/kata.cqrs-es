namespace CQRS.UpHealth.Domain;

public record Slot
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
