using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprint.Grid.Tests.Models;
using Sprint.Grid.Impl;

namespace Sprint.Grid.Tests
{
    [TestClass]
    public class GridQueryableExtensionsTest
    {
        public IQueryable<Customer> Customers { get; set; }

        [TestInitialize]
        public void InitSource()
        {
            Customers = DataSourceHelper.GetCustomers();
        }
        

        [TestMethod]
        public void GroupByTest()
        {
            var original = Customers.GroupBy(x => new { Key0=x.Region, Key1=x.Country })
                .OrderBy(x => x.Key.Key0).ThenBy(x => x.Key.Key1)
                .Select(x => new GroupingItem
                {
                    Count = x.Count(),
                    Key = x.Key
                }).ToArray();

            Expression<Func<Customer, string>> region = c => c.Region;
            Expression<Func<Customer, string>> country = c => c.Country;

            var properties = new Dictionary<LambdaExpression, SortDirection?>
            {
                { region, SortDirection.Ascending },
                { country, SortDirection.Ascending }
            };

            var result = Customers.GroupBy(properties).ToArray();

            Assert.AreEqual(original.Length, result.Length);

            for(var i = 0; i < original.Length; i++)
            {
                var originalItem = original[i];
                var dynamicItem = result[i];   
             
                Assert.AreEqual(originalItem.Count,dynamicItem.Count);

                var originalKey = originalItem.Key;
                var dynamicKey = dynamicItem.Key;
                

                var originalKeyType = originalItem.Key.GetType();
                var dynamicKeyType = dynamicItem.Key.GetType();


                Assert.AreEqual((string)originalKeyType.GetProperty("Key0").GetValue(originalKey), (string)dynamicKeyType.GetProperty("Key0").GetValue(dynamicKey));
                Assert.AreEqual((string)originalKeyType.GetProperty("Key1").GetValue(originalKey), (string)dynamicKeyType.GetProperty("Key1").GetValue(dynamicKey));
            }
        }

        [TestMethod]
        public void WhereTest()
        {
            var originalQuery = Customers.Where(x => x.Region == "OR").ToArray();
            
            Expression<Func<Customer, string>> region = c => c.Region;

            var result = Customers.Where(region, "OR").ToArray();

            Assert.AreEqual(result.Count(),originalQuery.Count());

            Assert.IsTrue(originalQuery.Count(c => result.Any(r => r.ID == c.ID)) == 0);
        }
    }
}
