using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using TruckLoadingApp.API.Configuration;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers
{
    
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new { Message = "User ID could not be determined." });
            }
            
            // Add the current user's ID to the request for auditing
            request.RequestedByUserId = userId;
            
            var response = await _paymentService.ProcessPayment(request);
            if (response.Success)
                return Ok(response);

            return BadRequest(response.Message);
        }

        [HttpGet("{paymentId}")]
        public async Task<IActionResult> GetPaymentById(long paymentId)
        {
            var payment = await _paymentService.GetPaymentById(paymentId);
            return payment != null ? Ok(payment) : NotFound();
        }
    }
}
