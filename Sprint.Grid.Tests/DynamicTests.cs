using System.Collections.Generic;
using System.Linq;
using LinqKit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sprint.Dynamic;

namespace Sprint.Grid.Tests
{
    [TestClass]
    public class DynamicTests
    {
        [TestMethod]
        public void GetDynamicTypeTest()
        {
            var properties = new List<DynamicProperty>
            {
                new DynamicProperty("Id", typeof(int)),
                new DynamicProperty("Name", typeof(string)),
                new DynamicProperty("Keys", typeof(IDictionary<string, string>)),
            };

            var type = ClassFactory.Instance.GetDynamicClass(properties);

            var typeProperties = type.GetProperties().ToDictionary(p => p.Name, p => p.PropertyType);

            Assert.IsFalse(properties.Any(x => typeProperties[x.Name] != x.Type));
        }

        [TestMethod]        
        public void AsyncDynamicTypeTest()
        {
            
            var properties = new List<DynamicProperty>
            {
                new DynamicProperty("Id", typeof(int)),
                new DynamicProperty("Name", typeof(string)),
                new DynamicProperty("Keys", typeof(IDictionary<string, string>)),
            };

            System.Threading.Tasks.Parallel.For((long)0, 1000, x => {
                var type = ClassFactory.Instance.GetDynamicClass(properties);
            });
        }
    }
}
