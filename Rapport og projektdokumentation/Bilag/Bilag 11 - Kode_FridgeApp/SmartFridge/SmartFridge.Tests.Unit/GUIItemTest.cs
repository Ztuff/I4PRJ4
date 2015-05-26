using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfacesAndDTO;
using NUnit.Framework;

namespace SmartFridge.Tests.Unit
{
    [TestFixture]
    public class GUIItemTest
    {
        private GUIItem uut;
        [SetUp]
        public void SetUp()
        {
            uut = new GUIItem("TestType", 1, 1, "l"){ShelfLife = DateTime.Now.Date};
        }

        [Test]
        public void ToString_StringifyItem_StringifiesCorrectly()
        {
            var str = uut.ToString();

            Assert.AreEqual("TestType Antal: 1 Enhed: 1 l", str);
        }
    }
}
