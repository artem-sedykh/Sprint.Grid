using Microsoft.Ajax.Utilities;
using Sprint.Grid.Impl;

namespace Sprint.Grid.Examples.Models
{
    public class CustomerGridModel:GridModel<Customer>
    {
        public CustomerGridModel(string gridKey) : base(gridKey)
        {            
            Columns.For(c => c.Name, "Name")
                .Title("Name")
                .SortColumn(c => c.Name, System.Web.Helpers.SortDirection.Ascending)
                .HeaderAttributes(new { style = "width:200px" });

            Columns.For(c => c.Country, "Country")
                .Title("Country")
                .SortColumn(c => c.Country)
                .GroupColumn(c => c.Country)
                .HeaderAttributes(new { style = "width:100px" });

            Columns.For(c => c.City, "City")
                .Title("City")
                .SortColumn(c => c.City)
                .GroupColumn(c => c.City)
                .HeaderAttributes(new { style = "width:91px" });

            Columns.For(c => c.Region, "Region")
                .Title("Region")
                .SortColumn(c => c.Region)
                .GroupColumn(c => c.Region, 1)
                .HeaderAttributes(new { style = "width:101px" });

            Columns.For(c => c.Code, "Code")
                .Title("Code")
                .SortColumn(c => c.Code)
                .GroupColumn(c => c.Code)
                .HeaderAttributes(new { style = "width:101px" });

            Columns.For(c => c.ContactName, "ContactName")
                .Title("ContactName")
                .SortColumn(c => c.ContactName)
                .GroupColumn(c => c.ContactName)
                .HeaderAttributes(new { style = "width:202px" });

            Columns.For(c => c.ContactTitle, "ContactTitle")
                .Title("ContactTitle")
                .SortColumn(c => c.ContactTitle)
                .GroupColumn(c => c.ContactTitle)
                .HeaderAttributes(new { style = "width:155px" });

            PageSize = 10;

            PageSizeInGroup = 5;

            HierarchyUrl = (customer, url) => url.Action("OrderGrid", "Home", new { customerId = customer.ID });
        }
    }
}
