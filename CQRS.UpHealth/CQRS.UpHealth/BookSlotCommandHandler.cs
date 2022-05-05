using CQRS.UpHealth.Commands;
using CQRS.UpHealth.CustomExceptions;
using CQRS.UpHealth.Domain;
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

        var events = _eventStore.GetEventsByStream(streamId);

        var daySchedule = DaySchedule.FromHistory(events);
        daySchedule.BookSlot(command.SlotId, command.PatientId);

        var recordedEvents = daySchedule.GetRecordedEvents();

        foreach (var recordedEvent in recordedEvents)
        {
            _eventStore.AddEvent(streamId, recordedEvent);
        }
    }
}