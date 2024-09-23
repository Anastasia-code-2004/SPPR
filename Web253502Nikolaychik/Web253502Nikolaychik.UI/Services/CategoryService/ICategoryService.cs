using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;

namespace Web253502Nikolaychik.UI.Services.CategoryService
{
    public interface ICategoryService
    {
        /// Получение списка всех категорий
        public Task<ResponseData<List<Category>>> GetCategoryListAsync();
    }
}
