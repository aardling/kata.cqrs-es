namespace CQRS.UpHealth.Events;

public record BookingWasCanceled : IEvent
{
    public Guid SlotId { get; set; }
}
