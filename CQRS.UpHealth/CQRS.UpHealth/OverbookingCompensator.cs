using CQRS.UpHealth.AvailableSlots;
using CQRS.UpHealth.Commands;
using CQRS.UpHealth.Events;

namespace CQRS.UpHealth
{
    public class OverbookingCompensator : IEventListener
    {
        private readonly ICommandHandler<CancelBooking> _commandHandler;
        private List<BookedSlot> _bookedSlots;
        private static int CALANDAR_DAYS = 30;
        public OverbookingCompensator(EventStore eventStore, ICommandHandler<CancelBooking> commandHandler)
        {
            _commandHandler = commandHandler;
            eventStore.Subscribe(this);
            _bookedSlots = new List<BookedSlot>();
        }
        public void When(IEvent evt)
        {
            if (evt is SlotWasBooked slotWasBooked) When(slotWasBooked);
            if (evt is BookingWasCanceled bookingWasCanceled) When(bookingWasCanceled);

            CleanData();
        }

        private void CleanData()
        {
            _bookedSlots.RemoveAll(bs => bs.StartDate < DateTime.Now.AddDays(-CALANDAR_DAYS));
        }

        private void When(BookingWasCanceled evt)
        {
            var bookedSlot = _bookedSlots.First(bs => bs.SlotId == evt.SlotId);
            _bookedSlots.Remove(bookedSlot);
        }

        private void When(SlotWasBooked evt)
        {
            _bookedSlots.Add(new BookedSlot()
            {
                PatientId = evt.PatientId,
                SlotId = evt.SlotId,
                StartDate = evt.StartDate
            });

            if (_bookedSlots.Count(bs => bs.PatientId == evt.PatientId && bs.StartDate >= DateTime.Now.AddDays(-CALANDAR_DAYS)) <=
                5) return;

            var command = new CancelBooking() { SlotId = evt.SlotId };
            _commandHandler.Handle(command);
        }
    }
}
