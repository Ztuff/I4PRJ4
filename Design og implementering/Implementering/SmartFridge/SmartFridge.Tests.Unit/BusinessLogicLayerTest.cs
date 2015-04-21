using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using BusinessLogicLayer;
using NUnit.Framework;

namespace SmartFridge.Tests.Unit
{
    [TestFixture]
    public class BusinessLogicLayerTest
    {
        public BLL uut;
        [SetUp]
        public void SetUp()
        {
            uut = new BLL();
        }

        [Test]
        public void CreateNewItem_4ParametersInserted_Same4ParametersInNewItem()
        {
            string type = "type";
            uint amount = 1;
            uint size = 1;
            string unit = "unit";

            var item = uut.CreateNewItem(type, amount, size, unit);
            Assert.AreEqual(type, item.Type);
            Assert.AreEqual(amount, item.Amount);
            Assert.AreEqual(size, item.Size);
            Assert.AreEqual(unit, item.Unit);
        }
    }
}
