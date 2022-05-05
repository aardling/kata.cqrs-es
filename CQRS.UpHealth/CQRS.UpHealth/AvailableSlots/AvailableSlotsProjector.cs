using CQRS.UpHealth.Events;

namespace CQRS.UpHealth.AvailableSlots;

public class AvailableSlotsProjector : IProjector
{
    private List<AvailableSlot> _availableSlots;
    public AvailableSlotsProjector(EventStore eventStore)
    {
        _availableSlots = new List<AvailableSlot>();
        eventStore.SubscribeProjector(this);
    }

    public void Project(IEvent evt)
    {
        if (evt is SlotWasScheduled scheduledEvent)
        {
            Project(scheduledEvent);
        }
        if (evt is SlotWasBooked bookedEvent)
        {
            Project(bookedEvent);
        }
    }

    public void Project(SlotWasScheduled evt)
    {
        _availableSlots.Add(new AvailableSlot()
        {
            SlotId = evt.SlotId,
            DocterId = evt.DoctorId,
            StartTime = evt.StartDate,
            EndTime = evt.EndDate
        });
    }

    public void Project(SlotWasBooked evt)
    {
        var slot = _availableSlots.Find(x => x.SlotId == evt.SlotId);
        _availableSlots.Remove(slot);
    }

    public List<AvailableSlot> GetAllAvailableSlotsForDay(DateTime day)
    {
        return _availableSlots.Where(s => s.StartTime.Date == day.Date).ToList();
    }
}
