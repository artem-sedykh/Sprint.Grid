using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sprint.Grid.Examples.Models;

namespace Sprint.Grid.Examples.Services.Impl
{
    public class CustomerService:ICustomerService
    {
        public IQueryable<Customer> GetAll()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "customers.JSON");
            using (var r = new StreamReader(path))
            {
                var json = r.ReadToEnd();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(json);

                return customers.AsQueryable();
            }   
        }
    }
}