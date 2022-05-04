using CQRS.UpHealth.Events;

namespace CQRS.UpHealth
{
    public class EventStore
    {
        private List<(string, IEvent)> _events = new ();
         
        public void AddEvent(string streamId, IEvent newEvent)
        {
            _events.Add(new(streamId, newEvent));
        }

        public IEnumerable<IEvent> GetEvents()
        {
            return _events.Select(t=>t.Item2);
        }

        public IEnumerable<IEvent> GetEventsByStream(string streamId)
        {
            return _events.Where(e => e.Item1.Equals(streamId)).Select(t => t.Item2);
        }
    }
}
