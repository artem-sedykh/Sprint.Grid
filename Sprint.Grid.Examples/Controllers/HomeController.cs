using System.Linq;
using System.Web.Mvc;
using Sprint.Grid.Examples.Models;
using Sprint.Grid.Examples.Services;
using Sprint.Grid.Impl;
using Sprint.Grid.Examples.Helpers;

namespace Sprint.Grid.Examples.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerService customerService;        

        public HomeController(ICustomerService customerService)
        {
            this.customerService = customerService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridOptions options)
        {
            var model = new GridModel<Customer>("customer");

            model.Columns.For(c => c.Name, "Name")
               .Title("Name")
               .SortColumn(c => c.Name, System.Web.Helpers.SortDirection.Ascending)
               .HeaderAttributes(new { style = "width:200px" });

            model.Columns.For(c => c.Country, "Country")
                .Title("Country")
                .SortColumn(c => c.Country)
                .GroupColumn(c => c.Country)
                .HeaderAttributes(new { style = "width:100px" });

            model.Columns.For(c => c.City, "City")
                .Title("City")
                .SortColumn(c => c.City)
                .GroupColumn(c => c.City)
                .HeaderAttributes(new { style = "width:91px" });

            model.Columns.For(c => c.Region, "Region")
                .Title("Region")
                .SortColumn(c => c.Region)
                .GroupColumn(c => c.Region, 1)
                .HeaderAttributes(new { style = "width:101px" });

            model.Columns.For(c => c.Code, "Code")
                .Title("Code")
                .SortColumn(c => c.Code)
                .GroupColumn(c => c.Code)
                .HeaderAttributes(new { style = "width:101px" });

            model.Columns.For(c => c.ContactName, "ContactName")
                .Title("ContactName")
                .SortColumn(c => c.ContactName)
                .GroupColumn(c => c.ContactName)
                .HeaderAttributes(new { style = "width:202px" });

            model.Columns.For(c => c.ContactTitle, "ContactTitle")
                .Title("ContactTitle")
                .SortColumn(c => c.ContactTitle)
                .GroupColumn(c => c.ContactTitle)
                .HeaderAttributes(new { style = "width:155px" });

            model.PageSize = 10;

            model.ShowEmptyRows = true;

            model.ShowEmptyRowsInGroup = true;

            model.PageSizeInGroup = 5;

           // model.HierarchyUrl = (customer, url) => url.Action("OrderGrid", "Home", new { customerId = customer.ID });

            var query = customerService.GetAll();

            return View(new ActionGridView<Customer>(model.Localize(), query).Init(options));
        }

        //public ActionResult OrderGrid(int? customerId, GridOptions options)
        //{
        //    var model = new OrderGridModel("order"+customerId);

        //    var query = customerService.GetAll().First(x=>x.ID==customerId).Order.AsQueryable();

        //    return View(new ActionGridView<Order>(model.Localize(), query).Init(options));
        //}

    }
}
