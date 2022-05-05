namespace CQRS.UpHealth.AvailableSlots;

public class AvailableSlot
{
    public Guid SlotId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid DocterId { get; set; }
}
