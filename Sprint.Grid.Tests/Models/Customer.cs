using System.Collections.Generic;

namespace Sprint.Grid.Tests.Models
{
    public class Customer
    {
        public Customer()
        {
            Order = new HashSet<Order>();
        }

        public int ID { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string ContactName { get; set; }

        public string ContactTitle { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public ICollection<Order> Order { get; set; }
    }
}
