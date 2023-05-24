using System;
using System.Collections.Generic;
using System.IO;
using Logistic.ConsoleClient.Enum;
using Logistic.Core;
using Logistic.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Logistic.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReportController : Controller
    {
        private readonly IReportService<Vehicle> _reportService;
        private readonly IService<Vehicle, int> _vehicleService;

        public ReportController(IReportService<Vehicle> reportService, IService<Vehicle, int> vehicleService)
        {
            _reportService = reportService;
            _vehicleService = vehicleService;
        }

        [HttpPost("generate")]
        public IActionResult GenerateReport([FromBody] ReportType reportType)
        {
            try
            {
                string fileExtension = reportType == ReportType.Xml ? "xml" : "json";
                string fileName = $"vehicles.{fileExtension}";
                List<Vehicle> vehicles = _vehicleService.GetAll();
                _reportService.CreateReport(fileName, reportType, vehicles);
                return Ok(fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadReport(string fileName)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports", $"{fileName}");
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("Report file not found.");
                }

                byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                string contentType = "text/plain"; 
                return File(fileBytes, contentType, $"{fileName}.txt");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}