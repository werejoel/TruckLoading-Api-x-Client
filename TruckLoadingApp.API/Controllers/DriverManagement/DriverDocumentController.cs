using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.API.Controllers.DriverManagement
{
    // Document upload model to separate file upload from document metadata
    public class DriverDocumentUploadModel
    {
        [Required]
        public long DriverId { get; set; }
        
        [Required]
        public string DocumentType { get; set; }
        
        public string DocumentNumber { get; set; }
        
        public DateTime? IssueDate { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        public string IssuingAuthority { get; set; }
        
        public string Notes { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriverDocumentController : ControllerBase
    {
        private readonly IDriverDocumentService _driverDocumentService;
        private readonly ILogger<DriverDocumentController> _logger;

        public DriverDocumentController(
            IDriverDocumentService driverDocumentService,
            ILogger<DriverDocumentController> logger)
        {
            _driverDocumentService = driverDocumentService;
            _logger = logger;
        }

        /// <summary>
        /// Uploads a document for a driver
        /// </summary>
        /// <param name="file">The document file to upload</param>
        /// <param name="model">The document metadata</param>
        /// <returns>The created driver document</returns>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<DriverDocument>> UploadDocument(
            [Required] IFormFile file, 
            [FromForm] DriverDocumentUploadModel model)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Create a DriverDocument from the model
                var document = new DriverDocument
                {
                    DriverId = model.DriverId,
                    DocumentType = model.DocumentType,
                    DocumentNumber = model.DocumentNumber ?? string.Empty,
                    IssueDate = model.IssueDate ?? DateTime.UtcNow,
                    ExpiryDate = model.ExpiryDate ?? DateTime.UtcNow.AddYears(1),
                    IssuingAuthority = model.IssuingAuthority ?? string.Empty,
                    Notes = model.Notes,
                    FileName = file.FileName,
                    FileSize = file.Length,
                    ContentType = file.ContentType ?? "application/octet-stream",
                    UploadDate = DateTime.UtcNow,
                    Status = DocumentStatus.PendingVerification,
                    DocumentUrl = $"/api/DriverDocument/download/{Guid.NewGuid()}" // Temporary URL, will be updated by the service
                };

                using var stream = file.OpenReadStream();
                var result = await _driverDocumentService.UploadDocumentAsync(document, stream, file.FileName);
                
                return CreatedAtAction(nameof(GetDocumentById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading document");
                return StatusCode(500, "An error occurred while uploading the document");
            }
        }

        [HttpPost]
        public async Task<ActionResult<DriverDocument>> AddDocument([FromBody] DriverDocument document)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverDocumentService.AddDocumentAsync(document);
                return CreatedAtAction(nameof(GetDocumentById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding document");
                return StatusCode(500, "An error occurred while adding the document");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DriverDocument>> GetDocumentById(long id)
        {
            try
            {
                var document = await _driverDocumentService.GetDocumentByIdAsync(id);
                if (document == null)
                {
                    return NotFound();
                }
                return Ok(document);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the document");
            }
        }

        [HttpGet("drivers/{driverId}")]
        public async Task<ActionResult<IEnumerable<DriverDocument>>> GetDriverDocuments(long driverId)
        {
            try
            {
                var documents = await _driverDocumentService.GetDriverDocumentsAsync(driverId);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for driver {DriverId}", driverId);
                return StatusCode(500, "An error occurred while retrieving the driver's documents");
            }
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(long id)
        {
            try
            {
                var (document, fileStream, contentType) = await _driverDocumentService.DownloadDocumentAsync(id);
                if (document == null || fileStream == null)
                {
                    return NotFound();
                }

                return File(fileStream, contentType ?? "application/octet-stream", document.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading document with ID {Id}", id);
                return StatusCode(500, "An error occurred while downloading the document");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDocument(long id, [FromBody] DriverDocument document)
        {
            try
            {
                if (id != document.Id)
                {
                    return BadRequest("Document ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _driverDocumentService.UpdateDocumentAsync(document);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the document");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocument(long id)
        {
            try
            {
                var result = await _driverDocumentService.DeleteDocumentAsync(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the document");
            }
        }

        [HttpGet("expiring")]
        public async Task<ActionResult<IEnumerable<DriverDocument>>> GetExpiringDocuments([FromQuery] int daysToExpiration = 30)
        {
            try
            {
                var documents = await _driverDocumentService.GetExpiringDocumentsAsync(daysToExpiration);
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expiring documents");
                return StatusCode(500, "An error occurred while retrieving expiring documents");
            }
        }

        [HttpPost("{id}/verify")]
        public async Task<IActionResult> VerifyDocument(long id, [FromBody] string verifiedBy)
        {
            try
            {
                var result = await _driverDocumentService.VerifyDocumentAsync(id, verifiedBy);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying document with ID {Id}", id);
                return StatusCode(500, "An error occurred while verifying the document");
            }
        }

        [HttpGet("types")]
        public async Task<ActionResult<IEnumerable<string>>> GetDocumentTypes()
        {
            try
            {
                var documentTypes = await _driverDocumentService.GetDocumentTypesAsync();
                return Ok(documentTypes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document types");
                return StatusCode(500, "An error occurred while retrieving document types");
            }
        }
    }
} 