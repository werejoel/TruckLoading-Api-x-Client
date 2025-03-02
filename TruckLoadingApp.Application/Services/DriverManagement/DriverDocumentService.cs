using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.DriverManagement.Interfaces;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services.DriverManagement
{
    public class DriverDocumentService : IDriverDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserActivityService _userActivityService;

        public DriverDocumentService(ApplicationDbContext context, IUserActivityService userActivityService)
        {
            _context = context;
            _userActivityService = userActivityService;
        }

        public async Task<DriverDocument> AddDocumentAsync(DriverDocument document)
        {
            // Validate document
            if (document.ExpiryDate <= DateTime.UtcNow)
                throw new ArgumentException("Document is already expired");

            _context.Add(document);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                document.Driver.UserId,
                ActivityTypes.AddDocument,
                $"Added {document.DocumentType} document",
                "Document",
                document.Id.ToString());

            return document;
        }

        public async Task<DriverDocument?> GetDocumentByIdAsync(long documentId)
        {
            return await _context.Set<DriverDocument>()
                .Include(d => d.Driver)
                .FirstOrDefaultAsync(d => d.Id == documentId);
        }

        public async Task<IEnumerable<DriverDocument>> GetDriverDocumentsAsync(long driverId)
        {
            return await _context.Set<DriverDocument>()
                .Where(d => d.DriverId == driverId)
                .OrderByDescending(d => d.ExpiryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DriverDocument>> GetExpiringDocumentsAsync(int daysThreshold)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);
            return await _context.Set<DriverDocument>()
                .Include(d => d.Driver)
                .Where(d => d.ExpiryDate <= thresholdDate &&
                           d.Status != DocumentStatus.Expired &&
                           d.Status != DocumentStatus.Rejected)
                .OrderBy(d => d.ExpiryDate)
                .ToListAsync();
        }

        public async Task<bool> VerifyDocumentAsync(long documentId, string verifiedBy)
        {
            var document = await _context.Set<DriverDocument>()
                .FindAsync(documentId);

            if (document == null)
                return false;

            document.IsVerified = true;
            document.VerificationDate = DateTime.UtcNow;
            document.VerifiedBy = verifiedBy;
            document.Status = DocumentStatus.Active;

            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                verifiedBy,
                ActivityTypes.VerifyDocument,
                $"Verified {document.DocumentType} document",
                "Document",
                documentId.ToString());

            return true;
        }

        public async Task<bool> UpdateDocumentAsync(DriverDocument document)
        {
            var existingDocument = await _context.Set<DriverDocument>()
                .FirstOrDefaultAsync(d => d.Id == document.Id);

            if (existingDocument == null)
                return false;

            _context.Entry(existingDocument).CurrentValues.SetValues(document);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                document.Driver.UserId,
                ActivityTypes.UpdateDocument,
                $"Updated document {document.DocumentType}",
                "Document",
                document.Id.ToString());

            return true;
        }

        public async Task<bool> DeleteDocumentAsync(long documentId)
        {
            var document = await _context.Set<DriverDocument>()
                .Include(d => d.Driver)
                .FirstOrDefaultAsync(d => d.Id == documentId);

            if (document == null)
                return false;

            _context.Remove(document);
            await _context.SaveChangesAsync();

            await _userActivityService.LogActivityAsync(
                document.Driver.UserId,
                ActivityTypes.DeleteDocument,
                $"Deleted document {document.DocumentType}",
                "Document",
                documentId.ToString());

            return true;
        }

        public async Task<DriverDocument> UploadDocumentAsync(DriverDocument document, Stream fileStream, string fileName)
        {
            // Validate document
            if (document.ExpiryDate <= DateTime.UtcNow)
                throw new ArgumentException("Document is already expired");

            // In a real implementation, this would upload the file to a storage service
            // and set the DocumentUrl to the URL of the uploaded file
            document.DocumentUrl = $"/documents/{Guid.NewGuid()}/{fileName}";
            document.FileName = fileName;
            document.Status = DocumentStatus.PendingVerification;
            document.CreatedDate = DateTime.UtcNow;

            _context.Add(document);
            await _context.SaveChangesAsync();

            // Log the activity
            var driver = await _context.Drivers.FindAsync(document.DriverId);
            if (driver != null)
            {
                await _userActivityService.LogActivityAsync(
                    driver.UserId,
                    ActivityTypes.AddDocument,
                    $"Uploaded {document.DocumentType} document",
                    "Document",
                    document.Id.ToString());
            }

            return document;
        }

        public async Task<(DriverDocument? document, Stream? fileStream, string? contentType)> DownloadDocumentAsync(long documentId)
        {
            var document = await GetDocumentByIdAsync(documentId);
            if (document == null)
                return (null, null, null);

            // If FileName is empty (for backward compatibility), extract it from DocumentUrl
            if (string.IsNullOrEmpty(document.FileName))
            {
                // Extract filename from the URL
                var urlParts = document.DocumentUrl.Split('/');
                if (urlParts.Length > 0)
                {
                    document.FileName = urlParts[urlParts.Length - 1];
                }
                else
                {
                    // Fallback filename
                    document.FileName = $"document_{document.Id}";
                }
            }

            // In a real implementation, this would download the file from a storage service
            // For now, we'll just return a dummy stream
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            await writer.WriteAsync("Dummy document content");
            await writer.FlushAsync();
            stream.Position = 0;

            // Determine content type based on file extension
            string contentType = "application/octet-stream";
            if (document.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                contentType = "application/pdf";
            else if (document.FileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) || 
                     document.FileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                contentType = "image/jpeg";
            else if (document.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                contentType = "image/png";

            return (document, stream, contentType);
        }

        public async Task<IEnumerable<string>> GetDocumentTypesAsync()
        {
            // Return the document types from the DocumentTypes static class
            return new List<string>
            {
                DocumentTypes.DriversLicense,
                DocumentTypes.CDL,
                DocumentTypes.MedicalCertificate,
                DocumentTypes.SafetyTraining,
                DocumentTypes.HazmatCertification,
                DocumentTypes.DefensiveDrivingCertificate,
                DocumentTypes.Insurance,
                DocumentTypes.Background_Check,
                DocumentTypes.DrugTest
            };
        }
    }
}
