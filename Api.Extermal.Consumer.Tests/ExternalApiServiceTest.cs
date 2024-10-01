using Api.External.Consumer.Services;
using Api.External.Consumer.Services.Interfaces;

namespace Api.External.Consumer.Tests
{
    public class ExternalApiServiceTest
    {
        private IExternalApiService _service;

        [SetUp]
        public void SetUp()
        {
            _service = new ExternalApiService(new HttpClient());
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            DateOnly dateOnly = new DateOnly(2024, 10, 01);
            var response = _service.GetWeeklyAvailabilityAsync(dateOnly);
        }
    }
}
