using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprint.Grid.Impl;
using Sprint.Grid.Tests.Models;

namespace Sprint.Grid.Tests
{
    [TestClass]
    public class GridModelHelperTest
    {
        public const string GridKey = "customer";

        [TestMethod]
        public void InitOrdersTest()
        {            
            var model = new GridModel<Customer>(GridKey);

            model.Columns.For(c => c.Name, "Name")
                .SortColumn(c => c.Name, SortDirection.Ascending)
                .GroupColumn(c => c.Name, 5);

            model.Columns.For(c => c.Country, "Country")
                .SortColumn(c => c.Country, SortDirection.Descending, 4)
                .GroupColumn(c => c.Country, 4);

            model.Columns.For(c => c.City, "City").SortColumn(c => c.City, SortDirection.Ascending, 2);
            
            model.InitOrders();

            Assert.IsTrue(model.Columns.First(c => c.Key == "Name").Value.SortOrder == 0);

            Assert.IsTrue(model.Columns.First(c => c.Key == "City").Value.SortOrder == 1);

            Assert.IsTrue(model.Columns.First(c => c.Key == "Country").Value.SortOrder == 2);


            Assert.IsTrue(model.Columns.First(c => c.Key == "Name").Value.GroupOrder == 1);

            Assert.IsTrue(model.Columns.First(c => c.Key == "Country").Value.GroupOrder == 0);

            Assert.IsTrue(model.Columns.First(c => c.Key == "City").Value.GroupOrder == null);
        }

        [TestMethod]
        public void OrderColumnTest()
        {
            var model = new GridModel<Customer>(GridKey);

            model.Columns.For(c => c.Name, "Name")
                .SortColumn(c => c.Name, SortDirection.Ascending)
                .GroupColumn(c => c.Name, 5);

            model.Columns.For(c => c.Country, "Country")
                .SortColumn(c => c.Country, SortDirection.Descending, 4)
                .GroupColumn(c => c.Country, 4);


            model.Columns.For(c => c.City, "City")
                .SortColumn(c => c.City, SortDirection.Ascending, 2);

            var options = new GridOptions
            {
                ColOpt = new Dictionary<string, Dictionary<string, object>>
                {
                    {"Name", new Dictionary<string, object>()},
                    {"City", new Dictionary<string, object>()},
                    {"Country", new Dictionary<string, object>()},
                }
            };

            model.MergeGridOptions(options);

            var index = 0;

            foreach (var column in model.Columns)
            {
                Assert.IsTrue(column.Value.Order == index);
                index++;
            }

            options.ColOpt["Name"]["co"] = 2;
            options.ColOpt["City"]["co"] = 1;

            model.MergeGridOptions(options);

            Assert.IsTrue(model.Columns.First(c => c.Key == "Country").Value.Order == 0);

            Assert.IsTrue(model.Columns.First(c => c.Key == "City").Value.Order == 1);

            Assert.IsTrue(model.Columns.First(c => c.Key == "Name").Value.Order == 2);
        }

        [TestMethod]
        public void SortingTest()
        {
            var model = new GridModel<Customer>(GridKey);

            model.Columns.For(c => c.Name, "Name")
                .SortColumn(c => c.Name, SortDirection.Ascending)
                .GroupColumn(c => c.Name, 5);

            model.Columns.For(c => c.Country, "Country")
                .SortColumn(c => c.Country, SortDirection.Descending, 4)
                .GroupColumn(c => c.Country, 4);


            model.Columns.For(c => c.City, "City")
                .SortColumn(c => c.City, SortDirection.Ascending, 2);

            var options = new GridOptions
            {
                ColOpt = new Dictionary<string, Dictionary<string, object>>
                {
                    {"Name", new Dictionary<string, object>()},
                    {"City", new Dictionary<string, object>()},
                    {"Country", new Dictionary<string, object>()},
                }
            };
                     

            options.ColOpt["Name"]["sd"] = 1;
            options.ColOpt["City"]["sd"] = null;
            options.ColOpt["Country"]["sd"] = null;

            model.MergeGridOptions(options);

            var nameColum = model.Columns.FirstOrDefault(x => x.Key == "Name");

            Assert.IsTrue(nameColum.Value.SortDirection != null && nameColum.Value.SortDirection.Value == SortDirection.Descending);

            Assert.IsTrue(model.Columns.Count(c => c.Key != "Name" && c.Value.SortDirection.HasValue) == 0);

            options.ColOpt["Name"]["sd"] = 0;

            model.MergeGridOptions(options);

            Assert.IsTrue(nameColum.Value.SortDirection != null && nameColum.Value.SortDirection.Value == SortDirection.Ascending);                        
        }

        [TestMethod]
        public void VisibleColumnTest()
        {
            var model = new GridModel<Customer>(GridKey);

            model.Columns.For(c => c.Name, "Name")
                .SortColumn(c => c.Name, SortDirection.Ascending)
                .GroupColumn(c => c.Name, 5);

            model.Columns.For(c => c.Country, "Country")
                .SortColumn(c => c.Country, SortDirection.Descending, 4)
                .GroupColumn(c => c.Country, 4);


            model.Columns.For(c => c.City, "City")
                .SortColumn(c => c.City, SortDirection.Ascending, 2).Visible(false);

            var options = new GridOptions
            {
                ColOpt = new Dictionary<string, Dictionary<string, object>>
                {
                    {"Name", new Dictionary<string, object>()},
                    {"City", new Dictionary<string, object>()},
                    {"Country", new Dictionary<string, object>()},
                }
            };

            model.MergeGridOptions(options);

            Assert.IsTrue(model.Columns.First(c=>c.Key=="Name").Value.IsVisible);

            Assert.IsTrue(model.Columns.First(c => c.Key == "Country").Value.IsVisible);

            Assert.IsFalse(model.Columns.First(c => c.Key == "City").Value.IsVisible);

            options.ColOpt["City"] = new Dictionary<string, object>();
            options.ColOpt["City"]["vc"] = true;

            model.MergeGridOptions(options);

            Assert.IsTrue(model.Columns.First(c => c.Key == "Name").Value.IsVisible);

            Assert.IsTrue(model.Columns.First(c => c.Key == "Country").Value.IsVisible);

            Assert.IsTrue(model.Columns.First(c => c.Key == "City").Value.IsVisible);

        }
    }
}
