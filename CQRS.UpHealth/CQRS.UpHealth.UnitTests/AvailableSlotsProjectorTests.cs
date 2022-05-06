using CQRS.UpHealth.AvailableSlots;
using CQRS.UpHealth.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CQRS.UpHealth.UnitTests;

[TestClass]
public class AvailableSlotsProjectorTests
{
    [TestMethod]
    public void ItShouldReturnNoAvailableSlots()
    {
        
        var eventStore = new EventStore();

        var projector = new AvailableSlotsEventListener(eventStore);
        var slots = projector.GetAllAvailableSlotsForDay(DateTime.Now);
        Assert.IsTrue(slots.Count == 0);
    }

    [TestMethod]
    public void ItShouldReturnAvailableSlots()
    {
        var eventStore = new EventStore();
        var projector = new AvailableSlotsEventListener(eventStore);

        eventStore.AddEvent("", new SlotWasScheduled()
        {
            DoctorId = Guid.NewGuid(),
            SlotId = Guid.NewGuid(),
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMinutes(20)
        });
        var slots = projector.GetAllAvailableSlotsForDay(DateTime.Now);
        Assert.IsTrue(slots.Count == 1);
    }

    [TestMethod]
    public void ItShouldNotReturnBookedSlots()
    {
        var eventStore = new EventStore();
        var projector = new AvailableSlotsEventListener(eventStore);
        var slotId = Guid.NewGuid();
        eventStore.AddEvent("", new SlotWasScheduled()
        {
            DoctorId = Guid.NewGuid(),
            SlotId = slotId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMinutes(20)
        });

        eventStore.AddEvent("", new SlotWasBooked()
        {
            PatientId = Guid.NewGuid(),
            SlotId = slotId
        });
        var slots = projector.GetAllAvailableSlotsForDay(DateTime.Now);
        Assert.IsTrue(slots.Count == 0);
    }

    [TestMethod]
    public void ItShouldMakeCancelledBookingSlotsAvailableAgain()
    {
        var eventStore = new EventStore();
        var projector = new AvailableSlotsEventListener(eventStore);
        var slotId = Guid.NewGuid();
        eventStore.AddEvent("", new SlotWasScheduled()
        {
            DoctorId = Guid.NewGuid(),
            SlotId = slotId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMinutes(20)
        });

        eventStore.AddEvent("", new SlotWasBooked()
        {
            PatientId = Guid.NewGuid(),
            SlotId = slotId
        });

        eventStore.AddEvent("", new BookingWasCanceled()
        {
            SlotId = slotId
        });
        var slots = projector.GetAllAvailableSlotsForDay(DateTime.Now);
        Assert.IsTrue(slots.Count == 1);
    }

}
