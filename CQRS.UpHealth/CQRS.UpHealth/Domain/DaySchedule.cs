using CQRS.UpHealth.CustomExceptions;
using CQRS.UpHealth.Events;

namespace CQRS.UpHealth.Domain;

public class DaySchedule
{
    private List<IEvent> _recordedEvents;
    private List<Guid> _slotIds;
    
    private DaySchedule()
    {
        _recordedEvents = new List<IEvent>();
        _slotIds = new List<Guid>();
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

            daySchedule._slotIds.Add(scheduledEvent.SlotId);
        }

        return daySchedule;
    }

    internal List<IEvent> GetRecordedEvents()
    {
        return _recordedEvents;
    }

    internal void BookSlot(Guid slotId, Guid patientId)
    {
        if(!_slotIds.Contains(slotId))
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
}
