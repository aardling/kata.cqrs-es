using CQRS.UpHealth.CustomExceptions;
using CQRS.UpHealth.Events;

namespace CQRS.UpHealth.Domain;

public class DaySchedule
{
    private List<IEvent> _recordedEvents;
    private List<Slot> _slots;
    
    private DaySchedule()
    {
        _recordedEvents = new List<IEvent>();
        _slots = new List<Slot>();
    }

    public static DaySchedule FromHistory(IEnumerable<IEvent> historicEvents)
    {
        var daySchedule = new DaySchedule();
        foreach(var historicEvent in historicEvents)
        {
            daySchedule.Apply(historicEvent);
        }

        return daySchedule;
    }

    internal List<IEvent> GetRecordedEvents()
    {
        return _recordedEvents;
    }

    internal void BookSlot(Guid slotId, Guid patientId)
    {
        if(!_slots.Any(s => s.Id == slotId))
        {
            throw new UnexistingSlotException();
        }

        var slotWasBooked = new SlotWasBooked
        {
            SlotId = slotId,
            PatientId = patientId
        };

        RecordThat(slotWasBooked);
    }

    internal void ScheduleSlot(Guid slotId, Guid doctorId, DateTime startDate, DateTime endDate)
    {
        if (_slots.Any(s => startDate <= s.EndDate && endDate >= s.StartDate))
            throw new SlotsCannotOverlapException();

        var slotWasScheduled = new SlotWasScheduled()
        {
            StartDate = startDate,
            EndDate = endDate,
            DoctorId = doctorId,
            SlotId = slotId
        };

        RecordThat(slotWasScheduled);
    }

    private void Apply(SlotWasScheduled evt)
    {
        _slots.Add(new Slot()
        {
            Id = evt.SlotId,
            StartDate = evt.StartDate,
            EndDate = evt.EndDate
        });
    }

    private void Apply(IEvent evt)
    {
        if (evt is SlotWasScheduled scheduledEvent)
        {
            Apply(scheduledEvent);
        }
    }

    private void RecordThat(IEvent evt)
    {
        _recordedEvents.Add(evt);
        Apply(evt);
    }
}
