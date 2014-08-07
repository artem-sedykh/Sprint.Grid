using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Sprint.Grid.Tests.Models;

namespace Sprint.Grid.Tests
{
    public static class DataSourceHelper
    {
        public static IQueryable<Customer> GetCustomers()
        {
            using (var r = new StreamReader("customers.JSON"))
            {
                var json = r.ReadToEnd();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(json);

                return customers.AsQueryable();
            }    
        }
    }
}
