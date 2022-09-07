using Microsoft.AspNetCore.Mvc;
using AdventureWorksNS.Data;
using AdventureWorksAPI.Repositories;

namespace AdventureWorksAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository repo;
        //constructor
        public CustomerController(ICustomerRepository repo)
        {
            this.repo = repo;
        }
        [HttpGet] //publicar en la página
        [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
        public async Task<IEnumerable<Customer>> GetCustomers(string? companyName)
        {
            //si no le pusieron parametro manda a traer todo
            if (string.IsNullOrEmpty(companyName))
            {
                return await repo.RetrieveAllAsync();
            }
            else
            {
                //si me mandan parametro
                return (await repo.RetrieveAllAsync())
                        .Where(custormer => custormer.CompanyName == companyName);

            }
        }

        //metodo para publicar customer con un parametro
        [HttpGet("{id}", Name = nameof(GetCustomer))]//Ruta
        [ProducesResponseType(200, Type = typeof(Customer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomer(int id)
        {
            Customer? c = await repo.RetrieveAsync(id);
            if (c == null)
            {
                return NotFound(); //Error 404
            }
            return Ok(c);
        }

        //Crar--- 
        [HttpPost]
        [ProducesResponseType(201, Type=typeof(Customer))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Create([FromBody] Customer c)
        {
            if (c==null)
            {
                return BadRequest();//400
            }
            //si no esta nullo la variable c entonces
            Customer? addCustomer = await repo.CreateAsync(c);
            if (addCustomer == null)
            {
                return BadRequest("Fallo al crear el customer");
            }
            else
            {
                return CreatedAtRoute ( //agregando un cliente
                        routeName: nameof(GetCustomer),
                        routeValues: new {id=addCustomer.CustomerId},
                        value: addCustomer);
                    
            }
        }

        //Update
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<IActionResult> Update(int id, [FromBody] Customer c)
        {
            if (c==null || c.CustomerId!=id)
            {
                return BadRequest(); //400
            }
            //si existe cliente
            Customer? existe  = await repo.RetrieveAsync(id);
            if (existe==null)
            {
                return NotFound();//404
            }

            await repo.UpdateAsync(id, c);
            return new NoContentResult(); //204

        }

        //Delete -- Borrar
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<IActionResult> Delete(int id)
        {
            Customer? existe = await repo.RetrieveAsync(id);

            if (existe==null)
            {
                return NotFound(); //404
            }

            bool? deleted = await repo.DeleteAsync(id);

            if (deleted.HasValue && deleted.Value)
            {
                return new NoContentResult(); //201
            }
            return BadRequest($"Cliente con le id {id} No se pudo Borrar");
        }

    }
}