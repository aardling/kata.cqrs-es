using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CQRS.UpHealth.Commands;
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
}