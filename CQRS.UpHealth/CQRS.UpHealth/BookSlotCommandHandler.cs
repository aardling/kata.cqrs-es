using CQRS.UpHealth.Commands;

namespace CQRS.UpHealth;

public class BookSlotCommandHandler : ICommandHandler<BookSlot>
{
    public void Handle(BookSlot command)
    {
        throw new NotImplementedException();
    }
}