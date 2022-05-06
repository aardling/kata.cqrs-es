using CQRS.UpHealth.Events;

namespace CQRS.UpHealth.AvailableSlots;

public interface IEventListener
{
    void When(IEvent evt);
}
