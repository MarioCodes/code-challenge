using Api.Core.Services;
using Api.Core.Services.interfaces;
using Api.External.Consumer.Model;
using Api.External.Consumer.Services.Interfaces;
using FluentAssertions;
using Moq;

namespace Api.Core.Tests.Services
{
    public class SlotsServiceTest
    {
        private ISlotsService _service;

        private Mock<IExternalApiService> _externalApiServiceMock;

        /*
            TODO: Tests que se me ocurren
                al cerrar que no se den horas despues si hay fragmentos raros de 35 mins o asi
                test donde lunchtime sea +1 hora
                test donde lunchtime sea negativo
        */

        [SetUp]
        public void SetUp()
        {
            _externalApiServiceMock = new Mock<IExternalApiService>();
            _service = new SlotsService(_externalApiServiceMock.Object);
        }

        [Test]
        public async Task GivenBasicExample_WhenGetAvailability_ThenOnlyCorrectDaysAreFilledAndRestAreNotPresent()
        {
            // given
            DateOnly mondayDate = new DateOnly(2024, 11, 4);

            var weekAvailabilityStub = new WeeklyAvailabilityResponse
            {
                Facility = new Facility
                {
                    FacilityId = "this-is-some-facility-id",
                    Name = "We have the best doctors clinic",
                    Address = "Calle falsa 123"
                },
                SlotDurationMinutes = 10,
                Monday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 9,
                        EndHour = 15,
                        LunchStartHour = 13,
                        LunchEndHour = 14,
                    }
                },
                Thursday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 8,
                        EndHour = 14,
                        LunchStartHour = 12,
                        LunchEndHour = 13,
                    },
                    BusySlots = new List<BusySlot>()
                    {
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 4, 8, 0, 0),
                            End = new DateTime(2024, 11, 4, 8, 10, 0)
                        }
                    }
                }
            };

            _externalApiServiceMock.Setup(api => api.GetWeeklyAvailabilityAsync(mondayDate)).ReturnsAsync(weekAvailabilityStub);

            // when
            var result = await _service.GetWeekFreeSlotsAsync(mondayDate);

            // then
            result.Monday.Should().NotBeNull();
            result.Thursday.Should().NotBeNull();

            result.Tuesday.Should().BeNull();
            result.Wednesday.Should().BeNull();
            result.Friday.Should().BeNull();
            result.Saturday.Should().BeNull();
            result.Sunday.Should().BeNull();
        }

        // TODO: hacerlo con multiples dias tambien
        [Test]
        public async Task GivenOneDayWithNoReservedSlots_WhenGetAvailability_ThenResultHasCorrectNumberOfFreeSlots()
        {
            // given
            int FreeSlotsForThisExample = 30;

            DateOnly mondayDate = new DateOnly(2024, 11, 4);

            var weekAvailabilityStub = new WeeklyAvailabilityResponse
            {
                Facility = new Facility
                {
                    FacilityId = "this-is-some-facility-id",
                    Name = "We have the best doctors clinic",
                    Address = "Calle falsa 123"
                },
                SlotDurationMinutes = 10,
                Thursday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 9,
                        EndHour = 15,
                        LunchStartHour = 13,
                        LunchEndHour = 14,
                    }
                }
            };

            _externalApiServiceMock.Setup(api => api.GetWeeklyAvailabilityAsync(mondayDate)).ReturnsAsync(weekAvailabilityStub);

            // when
            var result = await _service.GetWeekFreeSlotsAsync(mondayDate);

            // then
            result.Thursday.Should().NotBeNull();
            result.Thursday.AvailableSlots.Should().HaveCount(FreeSlotsForThisExample);
        }

        [Test]
        public async Task GivenOneDayWithThreeReservedSlots_WhenGetAvailability_ThenResultHasCorrectNumberOfFreeSlots()
        {
            // given
            int FreeSlotsForThisExample = 27;

            DateOnly mondayDate = new DateOnly(2024, 11, 4);

            var weekAvailabilityStub = new WeeklyAvailabilityResponse
            {
                Facility = new Facility
                {
                    FacilityId = "this-is-some-facility-id",
                    Name = "We have the best doctors clinic",
                    Address = "Calle falsa 123"
                },
                SlotDurationMinutes = 10,
                Thursday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 9,
                        EndHour = 15,
                        LunchStartHour = 13,
                        LunchEndHour = 14,
                    },
                    BusySlots = new List<BusySlot>()
                    {
                        // open time
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 9, 0, 0),
                            End = new DateTime(2024, 11, 7, 9, 10, 0)
                        },
                        // right before lunchtime
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 12, 50, 0),
                            End = new DateTime(2024, 11, 7, 13, 0, 0)
                        },
                        // close time
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 14, 50, 0),
                            End = new DateTime(2024, 11, 7, 15, 0, 0)
                        }
                    }
                }
            };

            _externalApiServiceMock.Setup(api => api.GetWeeklyAvailabilityAsync(mondayDate)).ReturnsAsync(weekAvailabilityStub);

            // when
            var result = await _service.GetWeekFreeSlotsAsync(mondayDate);

            // then
            result.Thursday.Should().NotBeNull();
            result.Thursday.AvailableSlots.Should().HaveCount(FreeSlotsForThisExample);
        }


        [Test]
        public async Task GivenOneDayWithSomeReservedSlots_WhenGetAvailability_ThenResultDoesntContainReservedSlots()
        {
            // given
            int FreeSlotsForThisExample = 27;

            DateOnly mondayDate = new DateOnly(2024, 11, 4);

            var weekAvailabilityStub = new WeeklyAvailabilityResponse
            {
                Facility = new Facility
                {
                    FacilityId = "this-is-some-facility-id",
                    Name = "We have the best doctors clinic",
                    Address = "Calle falsa 123"
                },
                SlotDurationMinutes = 10,
                Thursday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 9,
                        EndHour = 15,
                        LunchStartHour = 13,
                        LunchEndHour = 14,
                    },
                    BusySlots = new List<BusySlot>()
                    {
                        // open time
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 9, 0, 0),
                            End = new DateTime(2024, 11, 7, 9, 10, 0)
                        },
                        // right before lunchtime
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 12, 50, 0),
                            End = new DateTime(2024, 11, 7, 13, 0, 0)
                        },
                        // close time
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 14, 50, 0),
                            End = new DateTime(2024, 11, 7, 15, 0, 0)
                        }
                    }
                }
            };

            _externalApiServiceMock.Setup(api => api.GetWeeklyAvailabilityAsync(mondayDate)).ReturnsAsync(weekAvailabilityStub);

            // when
            var result = await _service.GetWeekFreeSlotsAsync(mondayDate);

            // then
            result.Thursday.Should().NotBeNull();
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 9, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 12, 50, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 14, 50, 0));
        }


        [Test]
        public async Task GivenOneDayWithSomeReservedSlots_WhenGetAvailability_ThenResultDoesntContainBeforeOpenTimeOrAfterCloseTime()
        {
            // given
            int FreeSlotsForThisExample = 27;

            DateOnly mondayDate = new DateOnly(2024, 11, 4);

            var weekAvailabilityStub = new WeeklyAvailabilityResponse
            {
                Facility = new Facility
                {
                    FacilityId = "this-is-some-facility-id",
                    Name = "We have the best doctors clinic",
                    Address = "Calle falsa 123"
                },
                SlotDurationMinutes = 10,
                Thursday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 9,
                        EndHour = 15,
                        LunchStartHour = 13,
                        LunchEndHour = 14,
                    },
                    BusySlots = new List<BusySlot>()
                    {
                        // open time
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 9, 0, 0),
                            End = new DateTime(2024, 11, 7, 9, 10, 0)
                        },
                        // right before lunchtime
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 12, 50, 0),
                            End = new DateTime(2024, 11, 7, 13, 0, 0)
                        },
                        // close time
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 14, 50, 0),
                            End = new DateTime(2024, 11, 7, 15, 0, 0)
                        }
                    }
                }
            };

            _externalApiServiceMock.Setup(api => api.GetWeeklyAvailabilityAsync(mondayDate)).ReturnsAsync(weekAvailabilityStub);

            // when
            var result = await _service.GetWeekFreeSlotsAsync(mondayDate);

            // then
            result.Thursday.Should().NotBeNull();

            // before open time
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 8, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 8, 10, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 8, 20, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 8, 30, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 8, 40, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 8, 50, 0));

            // after (including) close time
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 15, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 15, 10, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 15, 20, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 15, 30, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 15, 40, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 15, 50, 0));
        }


        [Test]
        public async Task GivenOneDayWithSomeReservedSlots_WhenGetAvailability_ThenResultDoesntContainLunchTime()
        {
            // given
            int FreeSlotsForThisExample = 27;

            DateOnly mondayDate = new DateOnly(2024, 11, 4);

            var weekAvailabilityStub = new WeeklyAvailabilityResponse
            {
                Facility = new Facility
                {
                    FacilityId = "this-is-some-facility-id",
                    Name = "We have the best doctors clinic",
                    Address = "Calle falsa 123"
                },
                SlotDurationMinutes = 10,
                Thursday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 9,
                        EndHour = 15,
                        LunchStartHour = 13,
                        LunchEndHour = 14,
                    },
                    BusySlots = new List<BusySlot>()
                    {
                        // open time
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 9, 0, 0),
                            End = new DateTime(2024, 11, 7, 9, 10, 0)
                        },
                        // right before lunchtime
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 12, 50, 0),
                            End = new DateTime(2024, 11, 7, 13, 0, 0)
                        },
                        // close time
                        new BusySlot()
                        {
                            Start = new DateTime(2024, 11, 7, 14, 50, 0),
                            End = new DateTime(2024, 11, 7, 15, 0, 0)
                        }
                    }
                }
            };

            _externalApiServiceMock.Setup(api => api.GetWeeklyAvailabilityAsync(mondayDate)).ReturnsAsync(weekAvailabilityStub);

            // when
            var result = await _service.GetWeekFreeSlotsAsync(mondayDate);

            // then
            result.Thursday.Should().NotBeNull();
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 13, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 13, 10, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 13, 20, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 13, 30, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 13, 40, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 13, 50, 0));
        }


        [Test]
        public async Task GivenReallyLongLunchtime_WhenGetAvailability_ThenResultDoesntContainLunchTime()
        {
            // given
            int FreeSlotsForThisExample = 27;

            DateOnly mondayDate = new DateOnly(2024, 11, 4);

            var weekAvailabilityStub = new WeeklyAvailabilityResponse
            {
                Facility = new Facility
                {
                    FacilityId = "this-is-some-facility-id",
                    Name = "We have the best doctors clinic",
                    Address = "Calle falsa 123"
                },
                SlotDurationMinutes = 20,
                Thursday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 9,
                        EndHour = 17,
                        LunchStartHour = 10,
                        LunchEndHour = 16,
                    }
                }
            };

            _externalApiServiceMock.Setup(api => api.GetWeeklyAvailabilityAsync(mondayDate)).ReturnsAsync(weekAvailabilityStub);

            // when
            var result = await _service.GetWeekFreeSlotsAsync(mondayDate);

            // then
            result.Thursday.Should().NotBeNull();
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 10, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 10, 20, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 10, 40, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 11, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 11, 20, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 11, 40, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 12, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 12, 20, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 12, 40, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 13, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 13, 20, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 13, 40, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 14, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 14, 20, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 14, 40, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 15, 0, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 15, 20, 0));
            result.Thursday.AvailableSlots.Should().NotContain(a => a.StartTime == new DateTime(2024, 11, 7, 15, 40, 0));
        }

        [Test]
        public async Task GivenReallyLongLunchtime_WhenGetAvailability_ThenResultContainsEdgeCasesAsFreeSlots()
        {
            // given
            int FreeSlotsForThisExample = 27;

            DateOnly mondayDate = new DateOnly(2024, 11, 4);

            var weekAvailabilityStub = new WeeklyAvailabilityResponse
            {
                Facility = new Facility
                {
                    FacilityId = "this-is-some-facility-id",
                    Name = "We have the best doctors clinic",
                    Address = "Calle falsa 123"
                },
                SlotDurationMinutes = 20,
                Thursday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 9,
                        EndHour = 17,
                        LunchStartHour = 10,
                        LunchEndHour = 16,
                    }
                }
            };

            _externalApiServiceMock.Setup(api => api.GetWeeklyAvailabilityAsync(mondayDate)).ReturnsAsync(weekAvailabilityStub);

            // when
            var result = await _service.GetWeekFreeSlotsAsync(mondayDate);

            // then
            result.Thursday.Should().NotBeNull();
            result.Thursday.AvailableSlots.Should().Contain(a => a.StartTime == new DateTime(2024, 11, 7, 9, 0, 0));
            result.Thursday.AvailableSlots.Should().Contain(a => a.StartTime == new DateTime(2024, 11, 7, 9, 40, 0));
            result.Thursday.AvailableSlots.Should().Contain(a => a.StartTime == new DateTime(2024, 11, 7, 16, 0, 0));
            result.Thursday.AvailableSlots.Should().Contain(a => a.StartTime == new DateTime(2024, 11, 7, 16, 40, 0));
        }

        [Test]
        public async Task GivenOneDayWithNoReservedSlots_WhenGetAvailability_ThenResultHasAvailableEdgeTimeCases()
        {
            // given
            DateOnly mondayDate = new DateOnly(2024, 11, 4);

            var rightAfterOpeningTime = new DateTime(2024, 11, 7, 9, 0, 0);
            var rightBeforeLunchTime = new DateTime(2024, 11, 7, 12, 50, 0);
            var rightAfterLunchTime = new DateTime(2024, 11, 7, 14, 0, 0);
            var rightBeforeCloseTime = new DateTime(2024, 11, 7, 14, 50, 0);

            var weekAvailabilityStub = new WeeklyAvailabilityResponse
            {
                Facility = new Facility
                {
                    FacilityId = "this-is-some-facility-id",
                    Name = "We have the best doctors clinic",
                    Address = "Calle falsa 123"
                },
                SlotDurationMinutes = 10,
                Thursday = new Day
                {
                    WorkPeriod = new WorkPeriod
                    {
                        StartHour = 9,
                        EndHour = 15,
                        LunchStartHour = 13,
                        LunchEndHour = 14,
                    }
                }
            };

            _externalApiServiceMock.Setup(api => api.GetWeeklyAvailabilityAsync(mondayDate)).ReturnsAsync(weekAvailabilityStub);

            // when
            var result = await _service.GetWeekFreeSlotsAsync(mondayDate);

            // then
            result.Thursday.Should().NotBeNull();
            result.Thursday.AvailableSlots.Should().Contain(a => a.StartTime == rightAfterOpeningTime);
            result.Thursday.AvailableSlots.Should().Contain(a => a.StartTime == rightBeforeLunchTime);
            result.Thursday.AvailableSlots.Should().Contain(a => a.StartTime == rightAfterLunchTime);
            result.Thursday.AvailableSlots.Should().Contain(a => a.StartTime == rightBeforeCloseTime);
        }
    }
}
