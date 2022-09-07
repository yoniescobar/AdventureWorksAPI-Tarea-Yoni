using AdventureWorksNS.Data;
using System.Collections.Concurrent; //para referenciar diccionario
using Microsoft.EntityFrameworkCore.ChangeTracking; //para ver si cambio o no la data

namespace AdventureWorksAPI.Repositories
{
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private static ConcurrentDictionary<int, ProductCategory>? categoryCache;
        private AdventureWorksDB db;

        public ProductCategoryRepository(AdventureWorksDB injectedDB)
        {
            db = injectedDB;
            //creamos por primera vez la cache si no tiene nada (en la memoria)
            if (categoryCache is null)
            {
                categoryCache = new ConcurrentDictionary<int, ProductCategory>(
                    db.ProductCategories.ToDictionary(p=>p.ProductCategoryId)
                    );
            }
        }

        
        public async Task<ProductCategory> CreateAsync(ProductCategory p)
        {
            EntityEntry<ProductCategory> agregado = await db.ProductCategories.AddAsync(p);
            int afectados = await db.SaveChangesAsync();
            if (afectados == 1)
            {
                if (categoryCache is null) return p;

                return categoryCache.AddOrUpdate(p.ProductCategoryId, p, UpdateCache);
            }
            else
            {
                return null!;
            
            }
        }
        //udate cache
        private ProductCategory UpdateCache(int id, ProductCategory p)
        {
            ProductCategory? viejo;
            if(categoryCache is not null)
            {
                if(categoryCache.TryGetValue(id, out viejo))
                {
                    if (categoryCache.TryUpdate(id,p,viejo))
                    {
                        return p;
                    }
                }
            }
            return null!;
        }

        //Read ver todos los datos  READ
        public Task<IEnumerable<ProductCategory>> RetrieveAllAsync()
        {
            return Task.FromResult(categoryCache is null?
                Enumerable.Empty<ProductCategory>():categoryCache.Values);
        }
        //Read ver solo un registro con ID

        public  Task<ProductCategory?> RetrieveAsync(int id)
        {
            if (categoryCache is null) return null!;
             categoryCache.TryGetValue(id, out ProductCategory? p);
            return Task.FromResult(p);
       
        }

        //Update Actualizar Registro
        public async Task<ProductCategory?> UpdateAsync(int id, ProductCategory p)
        {
            db.ProductCategories.Update(p);
            int afectados = await db.SaveChangesAsync();
            if (afectados ==1)
            {
                return UpdateCache(id, p);
            }
            return null!;
        }

        //Delete

        public async Task<bool?> DeleteAsync(int id)
        {
            ProductCategory? p = db.ProductCategories.Find(id);

            if(p is null) return false;
            db.ProductCategories.Remove(p);
            int afectados = await db.SaveChangesAsync();
            if (afectados==1)
            {
                if (categoryCache is null) return null;
                return categoryCache.TryRemove(id, out p);
            }
            else
            {
                return null!;
            }
        }


    }
}
