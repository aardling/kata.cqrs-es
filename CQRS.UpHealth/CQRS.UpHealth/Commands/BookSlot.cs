namespace CQRS.UpHealth.Commands;

public record BookSlot : ICommand
{
    public Guid PatientId { get; set; }
    public Guid SlotId { get; set; }

}