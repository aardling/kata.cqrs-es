using CQRS.UpHealth.Commands;
using CQRS.UpHealth.CustomExceptions;
using CQRS.UpHealth.Domain;
using CQRS.UpHealth.Events;

namespace CQRS.UpHealth
{
    public class ScheduleSlotCommandHandler : ICommandHandler<ScheduleSlot>
    {
        private EventStore _eventStore;
        public ScheduleSlotCommandHandler(EventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public void Handle(ScheduleSlot scheduleSlot)
        {
            var streamId = $"{scheduleSlot.DoctorId}/{scheduleSlot.StartDate.ToString("yyyy/MM/dd")}";
            var events = _eventStore.GetEventsByStream(streamId);

            var daySchedule = DaySchedule.FromHistory(events);
            daySchedule.ScheduleSlot(scheduleSlot.SlotId, scheduleSlot.DoctorId, scheduleSlot.StartDate, scheduleSlot.EndDate);

            var recordedEvents = daySchedule.GetRecordedEvents();

            foreach (var recordedEvent in recordedEvents)
            {
                _eventStore.AddEvent(streamId, recordedEvent);
            }
        }
    }
}
