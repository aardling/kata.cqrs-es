namespace CQRS.UpHealth.Events
{
    public record SlotWasScheduled : IEvent
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid DoctorId { get; set; }
    }
}
