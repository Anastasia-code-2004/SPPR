using Microsoft.EntityFrameworkCore;
using Web253502Nikolaychik.API.Data;
using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;

namespace Web253502Nikolaychik.API.Services.CategoryService
{
    public class CategoryService(AppDbContext _context) : ICategoryService
    {
        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var query = await _context.Categories.ToListAsync();
            return ResponseData<List<Category>>.Success(query);
        }
    }
}
