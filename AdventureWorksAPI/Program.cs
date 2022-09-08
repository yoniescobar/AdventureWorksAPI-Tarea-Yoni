using AdventureWorksNS.Data;
using static System.Console;
using Microsoft.AspNetCore.Mvc.Formatters;
using AdventureWorksAPI.Repositories;
using System.Text.Json.Serialization;

namespace AdventureWorksAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


           // b.Agregar el Contexto de la base de datos de Aventure works
             builder.Services.AdventureWorksDBContext();

            // Add services to the container.
            // Agregar y visualizar los fromatos soportados
            builder.Services.AddControllers(options =>
            {
                WriteLine("Formatos por omision: ");
                foreach (IOutputFormatter formatter in options.OutputFormatters)
                {
                    OutputFormatter? mediaFormatter = formatter as OutputFormatter;
                    if (mediaFormatter == null)
                    {
                        WriteLine($"{formatter.GetType().Name}");
                    }
                    else
                    {
                        WriteLine($"{mediaFormatter.GetType().Name}," +
                            $"Media types: {string.Join(",",mediaFormatter.SupportedMediaTypes)}");
                    }
                }
            })
                .AddXmlDataContractSerializerFormatters() 
                .AddXmlSerializerFormatters()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(); //agregar en el swagger el repositorio

            //creamos los controladores  //esquemas de repositorio
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}