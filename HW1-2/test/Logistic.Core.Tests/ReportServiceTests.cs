using Logistic.ConsoleClient.Enum;
using Logistic.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Logistic.Core.Tests
{
    public class ReportServiceTests
    {
        private readonly Mock<IReportRepository<Vehicle>> _jsonRepositoryMock = new();
        private readonly Mock<IReportRepository<Vehicle>> _xmlRepositoryMock = new();
        private readonly ReportService<Vehicle> _reportService;

        public ReportServiceTests()
        {
            _reportService = new ReportService<Vehicle>(_jsonRepositoryMock.Object, _xmlRepositoryMock.Object, "reports.json", "reports.xml");
        }

        [Fact]
        public void CreateReport_WhenReportTypeIsJson_ShouldCallJsonRepository()
        {
            // Arrange
            var fileName = "test.json";
            var reportType = ReportType.Json;
            var reports = new List<Vehicle> { new Vehicle { Id = 1, Number = "Test Report" } };

            // Act
            _reportService.CreateReport(fileName, reportType, reports);

            // Assert
            _jsonRepositoryMock.Verify(x => x.Create(reports, "vehicles"), Times.Once);
            _xmlRepositoryMock.Verify(x => x.Create(It.IsAny<List<Vehicle>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CreateReport_WhenReportTypeIsXml_ShouldCallXmlRepository()
        {
            // Arrange
            var fileName = "test.xml";
            var reportType = ReportType.Xml;
            var reports = new List<Vehicle> { new Vehicle { Id = 1, Number = "Test Report" } };

            // Act
            _reportService.CreateReport(fileName, reportType, reports);

            // Assert
            _xmlRepositoryMock.Verify(x => x.Create(reports, "vehicles"), Times.Once);
            _jsonRepositoryMock.Verify(x => x.Create(It.IsAny<List<Vehicle>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CreateReport_WhenUnsupportedReportType_ShouldThrowArgumentException_()
        {
            // Arrange
            var fileName = "test.txt";
            var reportType = (ReportType)999;
            var reports = new List<Vehicle> { new Vehicle { Id = 1, Number = "Test Report" } };

            // Act
            Action act = () => _reportService.CreateReport(fileName, reportType, reports);

            // Assert
            var ex = Assert.Throws<ArgumentException>(act);
            Assert.Equal("Unsupported report type", ex.Message);

            _jsonRepositoryMock.Verify(x => x.Create(It.IsAny<List<Vehicle>>(), It.IsAny<string>()), Times.Never);
            _xmlRepositoryMock.Verify(x => x.Create(It.IsAny<List<Vehicle>>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void LoadReport_WhenValidJsonReport_ShouldReturnDeserializedObjects()
        {
            // Arrange
            var fileName = "valid_report.json";
            var reportType = ReportType.Json;
            var vehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Number = "ABC123" },
                new Vehicle { Id = 2, Number = "DEF456" },
            };
            _jsonRepositoryMock.Setup(x => x.Read(fileName)).Returns(vehicles);

            // Act
            var result = _reportService.LoadReport(fileName, reportType);
            
            // Assert
            Assert.Equal(vehicles, result);
            _jsonRepositoryMock.Verify(x => x.Read(fileName), Times.Once);
            _xmlRepositoryMock.Verify(x => x.Read(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void LoadReport_WhenValidXmlReport_ShouldReturnDeserializedObjects()
        {
            // Arrange
            var fileName = "valid_report.xml";
            var reportType = ReportType.Xml;
            var vehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Number = "ABC123" },
                new Vehicle { Id = 2, Number = "DEF456" },
            };

            _xmlRepositoryMock.Setup(x => x.Read(fileName)).Returns(vehicles);

            // Act
            var result = _reportService.LoadReport(fileName, reportType);

            // Assert
            Assert.Equal(vehicles, result);
            _xmlRepositoryMock.Verify(x => x.Read(fileName), Times.Once);
            _jsonRepositoryMock.Verify(x => x.Read(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void LoadReport_WhenUnsupportedReportType_ShouldThrowArgumentException()
        {
            // Arrange
            var fileName = "invalid_report.txt";
            var reportType = (ReportType)999;

            // Act
            Action act = () => _reportService.LoadReport(fileName, reportType);

            // Assert
            var ex = Assert.Throws<ArgumentException>(act);
            Assert.Equal("Unsupported report type", ex.Message);

            _jsonRepositoryMock.Verify(x => x.Read(It.IsAny<string>()), Times.Never);
            _xmlRepositoryMock.Verify(x => x.Read(It.IsAny<string>()), Times.Never);
        }
    }
}
