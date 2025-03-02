using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.DriverManagement
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriverPayrollController : ControllerBase
    {
        private readonly IDriverPayrollService _driverPayrollService;
        private readonly ILogger<DriverPayrollController> _logger;

        public DriverPayrollController(
            IDriverPayrollService driverPayrollService,
            ILogger<DriverPayrollController> logger)
        {
            _driverPayrollService = driverPayrollService;
            _logger = logger;
        }

        [HttpPost("payroll-entries")]
        public async Task<ActionResult<PayrollEntry>> CreatePayrollEntry([FromBody] PayrollEntry payrollEntry)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverPayrollService.CreatePayrollEntryAsync(payrollEntry);
                return CreatedAtAction(nameof(GetPayrollEntryById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payroll entry");
                return StatusCode(500, "An error occurred while creating the payroll entry");
            }
        }

        [HttpGet("payroll-entries/{id}")]
        public async Task<ActionResult<PayrollEntry>> GetPayrollEntryById(long id)
        {
            try
            {
                var payrollEntry = await _driverPayrollService.GetPayrollEntryByIdAsync(id);
                if (payrollEntry == null)
                {
                    return NotFound();
                }
                return Ok(payrollEntry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payroll entry with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the payroll entry");
            }
        }

        [HttpGet("drivers/{driverId}/payroll-entries")]
        public async Task<ActionResult<IEnumerable<PayrollEntry>>> GetDriverPayrollEntries(
            long driverId, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;
                
                var payrollEntries = await _driverPayrollService.GetDriverPayrollEntriesAsync(driverId, start, end);
                return Ok(payrollEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payroll entries for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while retrieving the driver's payroll entries");
            }
        }

        [HttpPut("payroll-entries/{id}")]
        public async Task<IActionResult> UpdatePayrollEntry(long id, [FromBody] PayrollEntry payrollEntry)
        {
            try
            {
                if (id != payrollEntry.Id)
                {
                    return BadRequest("Payroll entry ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverPayrollService.UpdatePayrollEntryAsync(payrollEntry);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payroll entry with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the payroll entry");
            }
        }

        [HttpDelete("payroll-entries/{id}")]
        public async Task<IActionResult> DeletePayrollEntry(long id)
        {
            try
            {
                var result = await _driverPayrollService.DeletePayrollEntryAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payroll entry with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the payroll entry");
            }
        }

        [HttpGet("drivers/{driverId}/payroll-summary")]
        public async Task<ActionResult<PayrollSummary>> GetDriverPayrollSummary(
            long driverId, 
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-30);
                var end = endDate ?? DateTime.Today;
                
                var summary = await _driverPayrollService.GetDriverPayrollSummaryAsync(driverId, start, end);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payroll summary for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while retrieving the driver's payroll summary");
            }
        }

        [HttpGet("payroll-periods")]
        public async Task<ActionResult<IEnumerable<PayrollPeriod>>> GetPayrollPeriods(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.Today.AddDays(-90);
                var end = endDate ?? DateTime.Today.AddDays(90);
                
                var periods = await _driverPayrollService.GetPayrollPeriodsAsync(start, end);
                return Ok(periods);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payroll periods");
                return StatusCode(500, "An error occurred while retrieving payroll periods");
            }
        }

        [HttpPost("process-payroll")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<IEnumerable<PayrollEntry>>> ProcessPayroll(
            [FromQuery] DateTime periodStart, 
            [FromQuery] DateTime periodEnd)
        {
            try
            {
                var payrollEntries = await _driverPayrollService.ProcessPayrollAsync(periodStart, periodEnd);
                return Ok(payrollEntries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payroll for period {Start} to {End}", periodStart, periodEnd);
                return StatusCode(500, "An error occurred while processing payroll");
            }
        }
    }
} 