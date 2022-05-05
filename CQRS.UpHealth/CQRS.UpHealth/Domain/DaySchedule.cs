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
            if (historicEvent is not SlotWasScheduled scheduledEvent)
            {
                continue;
            }

            daySchedule._slots.Add(new Slot()
            {
                Id = scheduledEvent.SlotId,
                StartDate = scheduledEvent.StartDate,
                EndDate = scheduledEvent.EndDate,
            });
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

        _recordedEvents.Add(slotWasBooked);
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

        _recordedEvents.Add(slotWasScheduled);
    }
}
