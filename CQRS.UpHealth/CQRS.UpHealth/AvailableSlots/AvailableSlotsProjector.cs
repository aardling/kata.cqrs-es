using CQRS.UpHealth.Events;

namespace CQRS.UpHealth.AvailableSlots;

public class AvailableSlotsProjector : IProjector
{
    private List<Slot> _availableSlots;
    private List<Slot> _bookedSlots;
    public AvailableSlotsProjector(EventStore eventStore)
    {
        _availableSlots = new List<Slot>();
        _bookedSlots = new List<Slot>();

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
        if (evt is BookingWasCanceled canceledEvent)
        {
            Project(canceledEvent);
        }
    }

    public void Project(SlotWasScheduled evt)
    {
        _availableSlots.Add(new Slot()
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
        _bookedSlots.Add(slot);
    }

    public void Project(BookingWasCanceled evt)
    {
        var slot = _bookedSlots.Find(x => x.SlotId == evt.SlotId);
        _bookedSlots.Remove(slot);
        _availableSlots.Add(slot);
    }

    public List<Slot> GetAllAvailableSlotsForDay(DateTime day)
    {
        return _availableSlots.Where(s => s.StartTime.Date == day.Date).ToList();
    }
}
