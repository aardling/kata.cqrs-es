namespace CQRS.UpHealth.AvailableSlots;

public class Slot
{
    public Guid SlotId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public Guid DocterId { get; set; }
}
