using CQRS.UpHealth.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CQRS.UpHealth.UnitTests;

[TestClass]
public class BookSlotCommandHandlerTest : CommandHandlerTest<BookSlot>
{
    public BookSlotCommandHandlerTest()
    {
        _handler = new BookSlotCommandHandler();
    }
}