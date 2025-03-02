using Microsoft.EntityFrameworkCore;
using TruckLoadingApp.Application.Services.Interfaces;
using TruckLoadingApp.Domain.Models;
using TruckLoadingApp.Infrastructure.Data;

namespace TruckLoadingApp.Application.Services
{
    public class LoadTagService : ILoadTagService
    {
        private readonly ApplicationDbContext _context;

        public LoadTagService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LoadTag> CreateTagAsync(string name, string? description)
        {
            var existingTag = await _context.LoadTags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

            if (existingTag != null)
            {
                throw new InvalidOperationException($"A tag with the name '{name}' already exists.");
            }

            var tag = new LoadTag
            {
                Name = name,
                Description = description
            };

            _context.LoadTags.Add(tag);
            await _context.SaveChangesAsync();

            return tag;
        }

        public async Task CreateTagAsync(LoadTag tag)
        {
            var existingTag = await _context.LoadTags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == tag.Name.ToLower());

            if (existingTag != null)
            {
                throw new InvalidOperationException($"A tag with the name '{tag.Name}' already exists.");
            }

            _context.LoadTags.Add(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<LoadTag?> GetTagByIdAsync(int id)
        {
            return await _context.LoadTags.FindAsync(id);
        }

        public async Task<LoadTag?> GetTagByNameAsync(string name)
        {
            return await _context.LoadTags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<LoadTag>> GetAllTagsAsync()
        {
            return await _context.LoadTags
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await _context.LoadTags.FindAsync(id);
            if (tag == null) return false;

            _context.LoadTags.Remove(tag);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<LoadTag> UpdateTagAsync(int id, string name, string? description)
        {
            var tag = await _context.LoadTags.FindAsync(id);
            if (tag == null)
            {
                throw new KeyNotFoundException($"Tag with ID {id} not found.");
            }

            var existingTag = await _context.LoadTags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower() && t.Id != id);

            if (existingTag != null)
            {
                throw new InvalidOperationException($"A tag with the name '{name}' already exists.");
            }

            tag.Name = name;
            tag.Description = description;

            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<bool> AssignTagToLoadAsync(int tagId, long loadId)
        {
            var tag = await _context.LoadTags.FindAsync(tagId);
            var load = await _context.Loads.FindAsync(loadId);

            if (tag == null || load == null) return false;

            var existingAssignment = await _context.LoadLoadTags
                .FirstOrDefaultAsync(lt => lt.LoadId == loadId && lt.LoadTagId == tagId);

            if (existingAssignment != null) return true; // Already assigned

            var loadTag = new LoadLoadTag
            {
                LoadId = loadId,
                LoadTagId = tagId
            };

            _context.LoadLoadTags.Add(loadTag);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveTagFromLoadAsync(int tagId, long loadId)
        {
            var loadTag = await _context.LoadLoadTags
                .FirstOrDefaultAsync(lt => lt.LoadId == loadId && lt.LoadTagId == tagId);

            if (loadTag == null) return false;

            _context.LoadLoadTags.Remove(loadTag);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LoadTag>> GetTagsByLoadIdAsync(long loadId)
        {
            return await _context.LoadLoadTags
                .Where(lt => lt.LoadId == loadId)
                .Select(lt => lt.LoadTag)
                .ToListAsync();
        }

        public async Task<IEnumerable<Load>> GetLoadsByTagIdAsync(int tagId)
        {
            return await _context.LoadLoadTags
                .Where(lt => lt.LoadTagId == tagId)
                .Select(lt => lt.Load)
                .ToListAsync();
        }
    }
}
