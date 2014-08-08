using System.Linq;
using Sprint.Grid.Examples.Models;

namespace Sprint.Grid.Examples.Services
{
    public interface ICustomerService
    {
        IQueryable<Customer> GetAll();
    }
}