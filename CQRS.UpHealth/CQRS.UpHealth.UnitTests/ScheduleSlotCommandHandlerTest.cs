using CQRS.UpHealth.Commands;
using CQRS.UpHealth.CustomExceptions;
using CQRS.UpHealth.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CQRS.UpHealth.UnitTests
{
    [TestClass]
    public class ScheduleSlotCommandHandlerTest : CommandHandlerTest<ScheduleSlot>
    {
        public ScheduleSlotCommandHandlerTest()
        {
            _handler = new ScheduleSlotCommandHandler(_eventStore);
        }

        [TestMethod]
        public void ItShouldExecuteScheduleSlotCommand()
        {
            var command = new ScheduleSlot()
            {
                DoctorId = System.Guid.NewGuid(),
                EndDate = System.DateTime.Now.AddMinutes(20),
                StartDate = System.DateTime.Now,
                SlotId = Guid.NewGuid()
            };

            When(command);

            var expectedSlotWasScheduled = new SlotWasScheduled()
            {
                StartDate = command.StartDate,
                EndDate = command.EndDate,
                DoctorId = command.DoctorId,
                SlotId = command.SlotId
            };

            Then(new List<IEvent>() { expectedSlotWasScheduled });
        }

        [TestMethod]
        public void ItShouldFailWhenExactlySameSlotAlreadyPlanned()
        {

            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddMinutes(20);
            var doctorId = Guid.NewGuid();

            var slotWasScheduledEvent = new SlotWasScheduled()
            {
                DoctorId = doctorId,
                EndDate = endDate,
                StartDate = startDate
            };
            var streamId = $"{slotWasScheduledEvent.DoctorId}/{slotWasScheduledEvent.StartDate.ToString("yyyy/MM/dd")}";

            Given(new List<(string, IEvent)>() { (streamId, slotWasScheduledEvent) });

            When(new ScheduleSlot()
            {
                DoctorId = doctorId,
                StartDate = startDate,
                EndDate = endDate
            });

            Then<SlotsCannotOverlapException>();
        }

        [TestMethod]
        public void ItShouldFailWhenOverlappingSlotAlreadyPlanned()
        {
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddMinutes(20);
            var doctorId = Guid.NewGuid();

            var slotWasScheduledEvent = new SlotWasScheduled()
            {
                DoctorId = doctorId,
                EndDate = endDate,
                StartDate = startDate
            };
            var streamId = $"{slotWasScheduledEvent.DoctorId}/{slotWasScheduledEvent.StartDate.ToString("yyyy/MM/dd")}";

            Given(new List<(string, IEvent)>() { (streamId, slotWasScheduledEvent)});

            When(new ScheduleSlot()
            {
                DoctorId = doctorId,
                StartDate = startDate.AddMinutes(5),
                EndDate = endDate.AddMinutes(5)
            });

            Then<SlotsCannotOverlapException>();
        }
    }
}