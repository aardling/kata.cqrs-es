using CQRS.UpHealth.Commands;
using CQRS.UpHealth.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQRS.UpHealth.UnitTests
{
    public class CommandHandlerTest<T> where T: class , ICommand
    {
        protected ICommandHandler<T> _handler;
        protected EventStore _eventStore;
        private T _command;
        private List<IEvent> _historicEvents;
        public CommandHandlerTest()
        {
            _eventStore = new EventStore();
            _historicEvents = new List<IEvent>();

        }
        protected void Then(List<IEvent> expectedEvents)
        {
            _handler.Handle(_command);

            var actualEvents = _eventStore.GetEvents();

            Assert.IsTrue(actualEvents.SequenceEqual(expectedEvents));
        }

        protected void Then<E>() where E : Exception
        {
            var actualEvents = _eventStore.GetEvents();

            Assert.ThrowsException<E>(() => _handler.Handle(_command));
            Assert.IsTrue(actualEvents.SequenceEqual(_historicEvents));
        }

        protected void Given(List<(string, IEvent)> historicEvents)
        {
            foreach (var historicEvent in historicEvents)
            {
                _eventStore.AddEvent(historicEvent.Item1, historicEvent.Item2);
                _historicEvents.Add(historicEvent.Item2);
            }
        }

        protected void When(T command)
        {
            _command = command;
        }
    }
}
