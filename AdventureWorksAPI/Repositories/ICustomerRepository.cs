using AdventureWorksNS.Data;

namespace AdventureWorksAPI.Repositories
{
    public interface ICustomerRepository
    { //CRUD
        //mi intefaz
        Task<Customer> CreateAsync(Customer c); //Create
        Task<IEnumerable<Customer>> RetrieveAllAsync(); //Read todas 
        Task<Customer?> RetrieveAsync(int id); //Read por parametro
        Task<Customer?>  UpdateAsync(int id, Customer c); //Udate
        Task<bool?> DeleteAsync(int id); //boleano si existe o no 

        

    }
}
