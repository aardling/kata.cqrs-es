namespace CQRS.UpHealth.Commands
{
    public class CancelBooking:ICommand
    {
        public Guid SlotId { get; set; }

    }
}
