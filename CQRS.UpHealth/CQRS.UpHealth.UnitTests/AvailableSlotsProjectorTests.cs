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

        var projector = new AvailableSlotsProjector(eventStore);
        var slots = projector.GetAllAvailableSlotsForDay(DateTime.Now);
        Assert.IsTrue(slots.Count == 0);
    }

    [TestMethod]
    public void ItShouldReturnAvailableSlots()
    {
        var eventStore = new EventStore();
        var projector = new AvailableSlotsProjector(eventStore);

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

}
