using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.DriverManagement.Interfaces
{
    public interface IDriverDocumentService
    {
        Task<DriverDocument> AddDocumentAsync(DriverDocument document);
        Task<DriverDocument?> GetDocumentByIdAsync(long documentId);
        Task<IEnumerable<DriverDocument>> GetDriverDocumentsAsync(long driverId);
        Task<bool> UpdateDocumentAsync(DriverDocument document);
        Task<bool> DeleteDocumentAsync(long documentId);
        Task<IEnumerable<DriverDocument>> GetExpiringDocumentsAsync(int daysThreshold);
        Task<bool> VerifyDocumentAsync(long documentId, string verifiedBy);
        Task<DriverDocument> UploadDocumentAsync(DriverDocument document, Stream fileStream, string fileName);
        Task<(DriverDocument? document, Stream? fileStream, string? contentType)> DownloadDocumentAsync(long documentId);
        Task<IEnumerable<string>> GetDocumentTypesAsync();
    }
}
