# Sprint.Grid

## What is this?

Sprint.Grid is a library that allows you to easily display data.

## How do I use it?

Install ["Sprint.Grid"](http://nuget.org/packages/Sprint.Grid/) via [NuGet](http://nuget.org).

## Example

**/GridModels/CustomerGridModel .cs**

```csharp
public class CustomerGridModel : GridModel<Customer>
{
    public CustomerGridModel(string gridKey) : base(gridKey)
    {
        Columns.For((c, html) => c.Name, "Name")
                   .Title("Name")
                   .SortColumn(c => c.Name,SortDirection.Ascending)                   
                   .HeaderAttributes(new { style = "width:200px" });
 
            Columns.For((c, html) => c.Country, "Country")
                   .Title("Country")
                   .SortColumn(c => c.Country)
                   .GroupColumn(c => c.Country)
                   .HeaderAttributes(new { style = "width:100px" });
 
            Columns.For((c, html) => c.City, "City")
                   .Title("City")
                   .SortColumn(c => c.City)
                   .GroupColumn(c => c.City)
                   .HeaderAttributes(new { style = "width:91px" });
 
            Columns.For((c, html) => c.Region, "Region")
                   .Title("Region")
                   .SortColumn(c => c.Region)
                   .GroupColumn(c => c.Region,1)
                   .HeaderAttributes(new { style = "width:101px" });
 
            Columns.For((c, html) => c.Code, "Code")
                   .Title("Code")
                   .SortColumn(c => c.Code)
                   .GroupColumn(c => c.Code)
                   .HeaderAttributes(new { style = "width:101px" });
 
            Columns.For((c, html) => c.ContactName, "ContactName")
                   .Title("ContactName")
                   .SortColumn(c => c.ContactName)
                   .GroupColumn(c => c.ContactName)
                   .HeaderAttributes(new { style = "width:202px" });
 
            Columns.For((c, html) => c.ContactTitle, "ContactTitle")
                   .Title("ContactTitle")
                   .SortColumn(c => c.ContactTitle)
                   .GroupColumn(c => c.ContactTitle)
                   .HeaderAttributes(new { style = "width:155px" });
 
            PageSize = 10;
 
            PageSizeInGroup = 5;
    }
}
```

**/Controllers/CustomerController.cs**

```csharp
public class CustomerController : Controller
{
	private readonly ICustomerService _customerService;

	public const string CustomerGridKey = "customer";

	public CustomerController(ICustomerService customerService)
	{
		_customerService=customerService;
	}    

	public ActionResult Index()
	{
		return View();
	} 

    public ActionResult Grid(GridOptions options)
    {
        var dc = new ;
 
        var query = _customerService.GetAll().OrderBy(x => x.ID);
 
        var model = new CustomerGridModel(CustomerGridKey);
 
        return View(new ActionGridView<Customer>(model, query).Init(options));
    }        
}
```
**/Views/Customer/Index.cshtml**

```html
@{
    ViewBag.Title = "Customers";
    Layout = "~/Views/Shared/_1ColumnLayout.cshtml";    
}

@Html.Action("Grid")
```

**/Scripts/Common.js**

```js
$(document).ready(function () {
    $('.sprint-grid').sprintgrid();
});
```

<iframe src="http://sprint.tfs.intravision.ru/SprintGrid#grouping"></iframe>