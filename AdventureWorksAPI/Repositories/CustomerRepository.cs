using AdventureWorksNS.Data;
using System.Collections.Concurrent; //para referenciar diccionario
using Microsoft.EntityFrameworkCore.ChangeTracking; //para ver si cambio o no la data

namespace AdventureWorksAPI.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        //hacer un diccionario para tener diponibilidad en los datos
        private static ConcurrentDictionary<int, Customer>? customerCache;
        //Redis(algoritmos lector--chita)**********open source para levantar cache en Api c#

        //Instanciar una bd  AdventureWorksDB
        private AdventureWorksDB db;
        //creamos un constructor para enviar la bd
        public CustomerRepository(AdventureWorksDB injectdDB)
        {
            db = injectdDB;
            //creamos por primera vez la cache si no tiene nada (en la memoria)
            if (customerCache is null)
            {
                customerCache = new ConcurrentDictionary<int, Customer>(
                    db.Customers.ToDictionary(c=>c.CustomerId));
            }

        }

        //Metodo para crear asyn
        public async Task<Customer> CreateAsync(Customer c)
        {
            EntityEntry<Customer> agregado = await db.Customers.AddAsync(c);
            int afectados = await db.SaveChangesAsync();
            if (afectados == 1)
            {
                if (customerCache is null) return c; //validar      //pasar metodo
                return customerCache.AddOrUpdate(c.CustomerId, c,UpdateCache); //que solo actualice
            }
           
                return null!;
            
        }

        private Customer UpdateCache(int id, Customer c) {
            //cliente viejo
            Customer? viejo;
            if (customerCache is not null) //si cleinte existe
            {
                if (customerCache.TryGetValue(id,out viejo)) //lo trae
                {
                    if (customerCache.TryUpdate(id, c, viejo)) //solo actualiza
                    {
                        return c; //exitoso regresa c,
                    }
                }
            }

            return null!;//si es nuevo returna null
        }

        //arreglo de clientes para todos //LEER
        public Task<IEnumerable<Customer>> RetrieveAllAsync()
        {
            return Task.FromResult(customerCache is null?
                Enumerable.Empty<Customer>(): customerCache.Values);
        }

        //arreglo de clientes para UNO  pARAMAETRO
        public Task<Customer?> RetrieveAsync(int id)
        {
            if (customerCache is null) return  null!; //por que no es null
            customerCache.TryGetValue(id, out Customer? c); //where para dicionary
            return Task.FromResult(c);
            //es una tabla diccionario llave.
        }

        //Update
        public async Task<Customer?> UpdateAsync(int id, Customer c)
        {
            db.Customers.Update(c);
            int afectados = await db.SaveChangesAsync();
            if (afectados == 1)
            {
                return UpdateCache(id, c);
            }
            return null; //no se pudo actualizar
        }

        //Eliminar
        public async Task<bool?> DeleteAsync(int id)
        {
            Customer? c = db.Customers.Find(id); //busca el aid
            if(c is null) return false; //si no lo encuentra
            db.Customers.Remove(c); //quitarlos
            int afectados = await db.SaveChangesAsync(); //verificar
            if(afectados == 1) //eliminaado
            {
                if (customerCache is null) return null;
                return customerCache.TryRemove(id, out c); //quitar el cache
            }
            else
            {
                return null;

            }
        }

       
    }
}
