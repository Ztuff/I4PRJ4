using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;
using DataAccessLayer.AdoNetUoW;
using DataAccessLayer.Connection;
using NSubstitute;
using NUnit.Framework;

namespace SmartFridge.Tests.Unit
{
    class FakeIConnectionFactory : IConnectionFactory
    {
        public IDbConnection FakeConn { get; set; }

        public FakeIConnectionFactory()
        {
            FakeConn = Substitute.For<IDbConnection>();
        }

        public IDbConnection Create()
        {
            return FakeConn;
        }
    }

    [TestFixture]
    class AdoNetContextIntegrationTest
    {
        private AdoNetContext _uut;
        private FakeIConnectionFactory _fakeFactory;

        [SetUp]
        public void Setup()
        {
            _fakeFactory = new FakeIConnectionFactory();
            
            _uut = new AdoNetContext(_fakeFactory);
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
            _fakeFactory.FakeConn.Received(1).CreateCommand();
        }

        [Test]
        public void Dispose_Called_CalledFakeConnDispose()
        {
            _uut.Dispose();
            _fakeFactory.FakeConn.Received(1).Dispose();
        }

        [Test]
        public void Dispose_CreateUutInUsingScope_CallsDisposeWhenOutOfScope()
        {
            using (_uut = new AdoNetContext(_fakeFactory))
            {
                
            }
            _fakeFactory.FakeConn.Received(1).Dispose();
        }
    }
}
