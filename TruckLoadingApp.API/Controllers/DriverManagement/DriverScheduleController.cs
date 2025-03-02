using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Domain.Enums;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.DriverManagement
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriverScheduleController : ControllerBase
    {
        private readonly IDriverScheduleService _driverScheduleService;
        private readonly ILogger<DriverScheduleController> _logger;

        public DriverScheduleController(
            IDriverScheduleService driverScheduleService,
            ILogger<DriverScheduleController> logger)
        {
            _driverScheduleService = driverScheduleService;
            _logger = logger;
        }

        [HttpGet("{scheduleId}")]
        public async Task<ActionResult<DriverSchedule>> GetScheduleById(long scheduleId)
        {
            var schedule = await _driverScheduleService.GetScheduleByIdAsync(scheduleId);
            
            if (schedule == null)
                return NotFound();
                
            return Ok(schedule);
        }

        [HttpGet("driver/{driverId}")]
        public async Task<ActionResult<IEnumerable<DriverSchedule>>> GetDriverSchedules(
            long driverId, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var schedules = await _driverScheduleService.GetDriverSchedulesAsync(driverId, startDate, endDate);
            return Ok(schedules);
        }

        [HttpGet("driver/{driverId}/recurring")]
        public async Task<ActionResult<IEnumerable<DriverSchedule>>> GetDriverRecurringSchedules(long driverId)
        {
            var schedules = await _driverScheduleService.GetDriverRecurringSchedulesAsync(driverId);
            return Ok(schedules);
        }

        [HttpGet("recurring/{recurringScheduleId}/instances")]
        public async Task<ActionResult<IEnumerable<DriverSchedule>>> GetRecurringScheduleInstances(long recurringScheduleId)
        {
            var instances = await _driverScheduleService.GetRecurringScheduleInstancesAsync(recurringScheduleId);
            return Ok(instances);
        }

        [HttpPost]
        public async Task<ActionResult<DriverSchedule>> CreateSchedule(DriverSchedule schedule)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdSchedule = await _driverScheduleService.CreateScheduleAsync(schedule);
                return CreatedAtAction(nameof(GetScheduleById), new { scheduleId = createdSchedule.Id }, createdSchedule);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating schedule");
                return StatusCode(500, "An error occurred while creating the schedule");
            }
        }

        [HttpPost("recurring")]
        public async Task<ActionResult<IEnumerable<DriverSchedule>>> CreateRecurringSchedule(
            [FromBody] RecurringScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var schedule = new DriverSchedule
                {
                    DriverId = request.DriverId,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = ScheduleStatus.Pending,
                    Notes = request.Notes,
                    LoadId = request.LoadId
                };

                var instances = await _driverScheduleService.CreateRecurringScheduleAsync(
                    schedule, 
                    request.RecurrencePattern, 
                    request.RecurrenceEndDate, 
                    request.MaxOccurrences);
                    
                return CreatedAtAction(
                    nameof(GetRecurringScheduleInstances), 
                    new { recurringScheduleId = instances.First().Id }, 
                    instances);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating recurring schedule");
                return StatusCode(500, "An error occurred while creating the recurring schedule");
            }
        }

        [HttpPut("{scheduleId}")]
        public async Task<IActionResult> UpdateSchedule(long scheduleId, DriverSchedule schedule)
        {
            if (scheduleId != schedule.Id)
                return BadRequest("Schedule ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _driverScheduleService.UpdateScheduleAsync(schedule);
                
                if (!result)
                    return NotFound();
                    
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating schedule");
                return StatusCode(500, "An error occurred while updating the schedule");
            }
        }

        [HttpPut("recurring/{scheduleId}")]
        public async Task<IActionResult> UpdateRecurringSchedule(
            long scheduleId, 
            [FromBody] RecurringScheduleUpdateRequest request)
        {
            if (scheduleId != request.Schedule.Id)
                return BadRequest("Schedule ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _driverScheduleService.UpdateRecurringScheduleAsync(
                    request.Schedule, 
                    request.ApplyToAllInstances);
                
                if (!result)
                    return NotFound();
                    
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating recurring schedule");
                return StatusCode(500, "An error occurred while updating the recurring schedule");
            }
        }

        [HttpDelete("{scheduleId}")]
        public async Task<IActionResult> DeleteSchedule(long scheduleId)
        {
            try
            {
                var result = await _driverScheduleService.DeleteScheduleAsync(scheduleId);
                
                if (!result)
                    return NotFound();
                    
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting schedule");
                return StatusCode(500, "An error occurred while deleting the schedule");
            }
        }

        [HttpDelete("recurring/{scheduleId}")]
        public async Task<IActionResult> DeleteRecurringSchedule(
            long scheduleId, 
            [FromQuery] bool deleteAllInstances = false)
        {
            try
            {
                var result = await _driverScheduleService.DeleteRecurringScheduleAsync(
                    scheduleId, 
                    deleteAllInstances);
                
                if (!result)
                    return NotFound();
                    
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting recurring schedule");
                return StatusCode(500, "An error occurred while deleting the recurring schedule");
            }
        }

        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<DriverSchedule>>> GetAvailableDriversForTimeSlot(
            [FromQuery] DateTime startTime, 
            [FromQuery] DateTime endTime)
        {
            try
            {
                var availableDrivers = await _driverScheduleService.GetAvailableDriversForTimeSlotAsync(
                    startTime, 
                    endTime);
                    
                return Ok(availableDrivers);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available drivers");
                return StatusCode(500, "An error occurred while getting available drivers");
            }
        }

        [HttpGet("driver/{driverId}/available")]
        public async Task<ActionResult<bool>> IsDriverAvailable(
            long driverId, 
            [FromQuery] DateTime startTime, 
            [FromQuery] DateTime endTime)
        {
            try
            {
                var isAvailable = await _driverScheduleService.IsDriverAvailableAsync(
                    driverId, 
                    startTime, 
                    endTime);
                    
                return Ok(isAvailable);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking driver availability");
                return StatusCode(500, "An error occurred while checking driver availability");
            }
        }
    }

    public class RecurringScheduleRequest
    {
        public long DriverId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public long? LoadId { get; set; }
        public RecurrencePattern RecurrencePattern { get; set; }
        public DateTime RecurrenceEndDate { get; set; }
        public int? MaxOccurrences { get; set; }
    }

    public class RecurringScheduleUpdateRequest
    {
        public DriverSchedule Schedule { get; set; } = null!;
        public bool ApplyToAllInstances { get; set; }
    }
} 