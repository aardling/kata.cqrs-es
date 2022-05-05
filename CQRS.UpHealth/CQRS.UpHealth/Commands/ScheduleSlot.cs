namespace CQRS.UpHealth.Commands
{
    public record ScheduleSlot : ICommand
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid DoctorId { get; set; }
    }
}
