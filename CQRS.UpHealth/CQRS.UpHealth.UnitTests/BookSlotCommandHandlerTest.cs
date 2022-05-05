using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CQRS.UpHealth.Commands;
using CQRS.UpHealth.CustomExceptions;
using CQRS.UpHealth.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CQRS.UpHealth.UnitTests;

[TestClass]
public class BookSlotCommandHandlerTest : CommandHandlerTest<BookSlot>
{
    public BookSlotCommandHandlerTest()
    {
        _handler = new BookSlotCommandHandler(_eventStore);
    }

    [TestMethod]
    public void ExecuteBookSlotCommand()
    {
        var doctorId = Guid.NewGuid();
        var startDate = DateTime.Now;

        var slotWasScheduledEvent = new SlotWasScheduled()
        {
            StartDate = startDate,
            EndDate = DateTime.Now.AddMinutes(20),
            DoctorId = doctorId,
            SlotId = Guid.NewGuid()
        };

        var streamId = $"{slotWasScheduledEvent.DoctorId}/{slotWasScheduledEvent.StartDate.ToString("yyyy/MM/dd")}";

        Given(new List<(string, IEvent)> () {(streamId, slotWasScheduledEvent)});

        var patientId = Guid.NewGuid();

        When(new BookSlot
        {
            SlotId = slotWasScheduledEvent.SlotId,
            PatientId = patientId,
            DoctorId = doctorId,
            StartDate = startDate
        });

        var slotWasBookedEvent = new SlotWasBooked
        {
            SlotId = slotWasScheduledEvent.SlotId,
            PatientId = patientId
        };

        Then(new List<IEvent>() { slotWasBookedEvent});
    }

    [TestMethod]
    public void ShouldNotBookAnUnexistingSlot()
    {
        var doctorId = Guid.NewGuid();
        var startDate = DateTime.Now;
        var patientId = Guid.NewGuid();
        var slotId = Guid.NewGuid();

        Given(new List<(string, IEvent)>() { });

        When(new BookSlot
        {
            SlotId = slotId,
            PatientId = patientId,
            DoctorId = doctorId,
            StartDate = startDate
        });

        Then<UnexistingSlotException>();
    }

    [TestMethod]
    public void ShouldNotBeBookedTwice()
    {
        var doctorId = Guid.NewGuid();
        var startDate = DateTime.Now;
        var patientId = Guid.NewGuid();
        var patientId2 = Guid.NewGuid();
        var slotId = Guid.NewGuid();

        var slotWasScheduledEvent = new SlotWasScheduled()
        {
            StartDate = startDate,
            EndDate = DateTime.Now.AddMinutes(20),
            DoctorId = doctorId,
            SlotId = slotId
        };

        var slotWasBookedEvent = new SlotWasBooked()
        {
            PatientId = patientId2,
            SlotId = slotId
        };

        var streamId = $"{slotWasScheduledEvent.DoctorId}/{slotWasScheduledEvent.StartDate.ToString("yyyy/MM/dd")}";

        Given(new List<(string, IEvent)>() { 
            (streamId, slotWasScheduledEvent),
            (streamId, slotWasBookedEvent)
        });

        When(new BookSlot
        {
            SlotId = slotId,
            PatientId = patientId,
            DoctorId = doctorId,
            StartDate = startDate
        });

        Then<SlotAlreadyBookedException>();
    }
}