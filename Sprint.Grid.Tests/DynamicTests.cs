using System.Collections.Generic;
using System.Linq;
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
    }
}
