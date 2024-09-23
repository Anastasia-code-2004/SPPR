using Web253502Nikolaychik.Domain.Entities;
using Web253502Nikolaychik.Domain.Models;

namespace Web253502Nikolaychik.API.Services.ProductService
{
    public interface IProductService
    {
        public Task<ResponseData<ListModel<Commodity>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1, int pageSize = 3);
        public Task<ResponseData<Commodity>> GetProductByIdAsync(int id);
        public Task<ResponseData<Commodity>> UpdateProductAsync(int id, Commodity product);
        public Task<ResponseData<string>> DeleteProductAsync(int id);
        public Task<ResponseData<Commodity>> CreateProductAsync(Commodity product);
        public Task<ResponseData<string>> SaveImageAsync(int id, IFormFile formFile);
    }
}
