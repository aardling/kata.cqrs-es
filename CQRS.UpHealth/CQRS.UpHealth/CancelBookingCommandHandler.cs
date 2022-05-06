using CQRS.UpHealth.Commands;

namespace CQRS.UpHealth
{
    public class CancelBookingCommandHandler:ICommandHandler<CancelBooking>
    {
        public void Handle(CancelBooking command)
        {
            throw new NotImplementedException();
        }
    }
}
