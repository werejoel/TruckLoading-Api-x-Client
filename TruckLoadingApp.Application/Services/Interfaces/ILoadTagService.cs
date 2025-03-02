using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ILoadTagService
    {
        Task<LoadTag> CreateTagAsync(string name, string? description);
        Task<LoadTag?> GetTagByIdAsync(int id);
        Task<LoadTag?> GetTagByNameAsync(string name);
        Task<IEnumerable<LoadTag>> GetAllTagsAsync();
        Task<bool> DeleteTagAsync(int id);
        Task<LoadTag> UpdateTagAsync(int id, string name, string? description);
        Task<bool> AssignTagToLoadAsync(int tagId, long loadId);
        Task<bool> RemoveTagFromLoadAsync(int tagId, long loadId);
        Task<IEnumerable<LoadTag>> GetTagsByLoadIdAsync(long loadId);
        Task<IEnumerable<Load>> GetLoadsByTagIdAsync(int tagId);
        Task CreateTagAsync(LoadTag tag);
    }
}
