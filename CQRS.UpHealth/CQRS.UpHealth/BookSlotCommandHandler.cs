using CQRS.UpHealth.Commands;
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
        var slotWasBooked = new SlotWasBooked
        {
            SlotId = command.SlotId,
            PatientId = command.PatientId
        };

        var streamId = $"{command.DoctorId}/{command.StartDate.ToString("yyyy/MM/dd")}";
        _eventStore.AddEvent(streamId, slotWasBooked);

    }
}