using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using FizzWare.NBuilder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace CrossSolar.Tests.Controller
{
    public class AnalyticsControllerTests
    {
        public AnalyticsControllerTests()
        {
            _analyticsController = new AnalyticsController(_analyticsRepositoryMock.Object, _panelRepositoryMock.Object);
        }

        private readonly AnalyticsController _analyticsController;

        private readonly Mock<IPanelRepository> _panelRepositoryMock = new Mock<IPanelRepository>();
        private readonly Mock<IAnalyticsRepository> _analyticsRepositoryMock = new Mock<IAnalyticsRepository>();


        [Fact]
        public async Task Post_ReturnsCreated()
        {


            // Arrange
            var panelDbSetMock = Builder<Panel>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _panelRepositoryMock.Setup(m => m.Query()).Returns(panelDbSetMock.Object);
            var analytic = Builder<OneHourElectricityModel>.CreateNew()
               .Build();

            // Act
            var result = await _analyticsController.Post("Serial1", analytic);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsNotFound()
        {


            // Arrange
            var panelDbSetMock = Builder<Panel>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _panelRepositoryMock.Setup(m => m.Query()).Returns(panelDbSetMock.Object);
            var analytic = Builder<OneHourElectricityModel>.CreateNew()
               .Build();

            // Act
            var result = await _analyticsController.Post("AAAA1111BBBB2222", analytic);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as NotFoundResult;
            Assert.NotNull(createdResult);
        }

        [Fact]
        public async Task Get_NotFound()
        {
            // Arrange
            var panelDbSetMock = Builder<Panel>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _panelRepositoryMock.Setup(m => m.Query()).Returns(panelDbSetMock.Object);

            // Act
            var result = await _analyticsController.Get("AAAA1111BBBB2222");

            // Assert
            Assert.NotNull(result);

            var objectResult = result as NotFoundResult;
            Assert.NotNull(objectResult);
        }

        [Fact]
        public async Task Get_ReturnsItem()
        {
            // Arrange
            var panelDbSetMock = Builder<Panel>.CreateListOfSize(3).Build().ToAsyncDbSetMock();
            _panelRepositoryMock.Setup(m => m.Query()).Returns(panelDbSetMock.Object);
            var analyticDbSetMock = Builder<OneHourElectricity>.CreateListOfSize(3)
                .All().Do(a => a.PanelId = "Serial1")
                .Build().ToAsyncDbSetMock();
            _analyticsRepositoryMock.Setup(m => m.Query()).Returns(analyticDbSetMock.Object);

            // Act
            var result = await _analyticsController.Get("Serial1");

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as OneHourElectricityListModel;
            Assert.NotNull(content);

        }

        [Fact]
        public async Task DayResults_ReturnsItem()
        {
            // Arrange
            var panelDbSetMock = Builder<Panel>.CreateListOfSize(2).Build().ToAsyncDbSetMock();
            _panelRepositoryMock.Setup(m => m.Query()).Returns(panelDbSetMock.Object);
            var analyticDbSetMock = Builder<OneHourElectricity>.CreateListOfSize(3)
                .All().Do(a => a.PanelId = "Serial1")
                .Build().ToAsyncDbSetMock();
            _analyticsRepositoryMock.Setup(m => m.Query()).Returns(analyticDbSetMock.Object);

            // Act
            var result = await _analyticsController.DayResults("Serial1");

            // Assert
            Assert.NotNull(result);

            var objectResult = result as OkObjectResult;
            Assert.NotNull(objectResult);

            var content = objectResult.Value as IEnumerable<OneDayElectricityModel>;
            Assert.NotNull(content);

        }
    }
}
