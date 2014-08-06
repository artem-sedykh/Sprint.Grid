using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprint.Grid.Impl;

namespace Sprint.Grid.Tests.Models
{
    public class CustomerGridModel:GridModel<Customer>
    {
        public CustomerGridModel(string gridKey) : base(gridKey)
        {
            Columns.For(c => c.Name, "Name");
        }
    }
}
