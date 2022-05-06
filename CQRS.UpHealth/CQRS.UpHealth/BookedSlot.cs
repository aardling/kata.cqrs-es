namespace CQRS.UpHealth
{
    public class BookedSlot
    {
        public Guid PatientId { get; set; }
        public DateTime StartDate { get; set; }
        public Guid SlotId { get; set; }
    }
}
