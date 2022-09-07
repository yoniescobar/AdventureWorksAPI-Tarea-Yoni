using AdventureWorksNS.Data;

namespace AdventureWorksAPI.Repositories
{
    public interface IProductCategoryRepository
    {
        //CRUD    READ TODO  Y READ CON ID
        Task<ProductCategory> CreateAsync(ProductCategory p);
        Task<IEnumerable<ProductCategory>> RetrieveAllAsync();
        Task<ProductCategory?> RetrieveAsync(int id);
        Task<ProductCategory?> UpdateAsync(int id, ProductCategory p);
        Task<bool?> DeleteAsync(int id);
   
    }
}
