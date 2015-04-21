using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataAccessLayer.Connection;
using NUnit.Framework;

namespace SmartFridge.Tests.Unit
{
    [TestFixture]
    class AppConnectionFactoryIntegrationTest
    {
        private AppConnectionFactory _uut;

        [Test]
        public void Ctor_ConnectionNameIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => _uut = new AppConnectionFactory(null), Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Ctor_ConnectionNameIsTest_Throws_ConfigurationErrorsException()
        {
            Assert.That(() => _uut = new AppConnectionFactory("NotAValidString"), Throws.TypeOf<ConfigurationErrorsException>());
        }

        [Test]
        public void Create_ConnectionNameIsSmartFridgeConn_ConnectionIsOpen()
        {
            _uut = new AppConnectionFactory("SmartFridgeConn");
            IDbConnection connection = _uut.Create();
            Assert.That(connection.ConnectionString, Is.EqualTo(@"Data Source=(localdb)\ProjectsV12;Initial Catalog=SmartFridge-SSDT;Integrated Security=True;Pooling=False;Connect Timeout=30"));
        }

        [Test]
        public void Create_ConnectionNameIsTest_ThrowsArgumentException()
        {
            _uut = new AppConnectionFactory("Test");
            Assert.That(() => _uut.Create(), Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void Create_ConnectionNameIsSmartFridgeConn_ReturnsConnection()
        {
            _uut = new AppConnectionFactory("SmartFridgeConn");
            Assert.IsInstanceOf<IDbConnection>(_uut.Create());
        }

    }
}
