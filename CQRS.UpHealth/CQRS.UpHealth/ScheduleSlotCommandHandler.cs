using CQRS.UpHealth.Commands;
using CQRS.UpHealth.CustomExceptions;
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

            foreach(var historyEvent in events)
            {
                if (historyEvent is not SlotWasScheduled scheduledEvent)
                    continue;

                if (scheduleSlot.StartDate <= scheduledEvent.EndDate && scheduleSlot.EndDate >= scheduledEvent.StartDate)
                    throw new SlotsCannotOverlapException();
            }

            var slotWasScheduled = new SlotWasScheduled()
            {
                StartDate = scheduleSlot.StartDate,
                EndDate = scheduleSlot.EndDate,
                DoctorId = scheduleSlot.DoctorId,
            };

            _eventStore.AddEvent(streamId, slotWasScheduled);
        }
    }
}
