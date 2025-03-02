using TruckLoadingApp.Domain.Models;

namespace TruckLoadingApp.Application.Services.Interfaces
{
    public interface ILoadService
    {
        Task<IEnumerable<Load>> GetAllLoadsAsync();
        Task<Load?> GetLoadByIdAsync(long id);
        Task<Load> CreateLoadAsync(Load load);
        Task<bool> UpdateLoadAsync(Load load);
        Task<bool> DeleteLoadAsync(long id);
        Task<bool> CancelLoadAsync(long loadId);
        Task<IEnumerable<Load>> GetLoadsByStatusAsync(string status);
        Task<bool> AssignTruckToLoadAsync(long loadId, long truckId);
    }
}
