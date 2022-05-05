using CQRS.UpHealth.Commands;
using CQRS.UpHealth.CustomExceptions;
using CQRS.UpHealth.Events;

namespace CQRS.UpHealth;

public class BookSlotCommandHandler : ICommandHandler<BookSlot>
{
    private EventStore _eventStore;

    public BookSlotCommandHandler(EventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public void Handle(BookSlot command)
    {
        var streamId = $"{command.DoctorId}/{command.StartDate.ToString("yyyy/MM/dd")}";

        var foundSlot = false;
        var events = _eventStore.GetEventsByStream(streamId);
        foreach (var e in events)
        {
            if (e is not SlotWasScheduled scheduledEvent)
            {
                continue;
            }

            if(scheduledEvent.SlotId == command.SlotId)
            {
                foundSlot = true;
            }
        }

        if(!foundSlot)
        {
            throw new UnexistingSlotException();
        }

        var slotWasBooked = new SlotWasBooked
        {
            SlotId = command.SlotId,
            PatientId = command.PatientId
        };

        _eventStore.AddEvent(streamId, slotWasBooked);

    }
}