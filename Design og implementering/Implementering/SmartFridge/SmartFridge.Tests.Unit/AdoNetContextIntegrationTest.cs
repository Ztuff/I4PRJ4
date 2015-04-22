using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.AdoNetUoW;
using DataAccessLayer.Connection;
using NSubstitute;
using NUnit.Framework;

namespace SmartFridge.Tests.Unit
{
    [TestFixture]
    class AdoNetContextIntegrationTest
    {
        private AdoNetContext _uut;
        private IConnectionFactory _subConn;

        [SetUp]
        public void Setup()
        {
            _subConn = Substitute.For<IConnectionFactory>();
            _uut = new AdoNetContext(_subConn);
        }

        [Test]
        public void CreateUnitOfWork_Called_ReturnsIUnitOfWork()
        {
            Assert.IsInstanceOf<AdoNetUnitOfWork>(_uut.CreateUnitOfWork());
        }

        [Test]
        public void CreateCommand_Called_ReturnsIDbCommand()
        {
            Assert.IsInstanceOf<IDbCommand>(_uut.CreateCommand());
        }

        [Test]
        public void CreateCommand_Called_ConnectionReceivesCall()
        {
            _uut.CreateCommand();
            _subConn.Received(1).Create();
        }


    }
}
