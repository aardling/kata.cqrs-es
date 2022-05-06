namespace CQRS.UpHealth.Events;

public record SlotWasBooked : IEvent
{
    public Guid SlotId { get; set; }
    public Guid PatientId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}