using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.GridModels;
using Domain.Model;
using Sprint.Grid.Impl;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly NorthWindDataContext _dataContext;

        public const string CustomerGridKey = "customer";    

        public HomeController(NorthWindDataContext dataContext)
        {
            _dataContext = dataContext;
        }            

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Grid(GridOptions options)
        {
            var query = _dataContext.Customer.OrderBy(x => x.ID);

            var model = new CustomerGridModel(CustomerGridKey);

            return View(new ActionGridView<Customer>(model, query).Init(options));            
        }

    }
}
