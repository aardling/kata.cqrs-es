using CQRS.UpHealth.Events;

namespace CQRS.UpHealth.AvailableSlots;

public interface IProjector
{
    void Project(IEvent evt);
}
