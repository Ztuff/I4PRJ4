using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.AdoNetUoW;
using NUnit.Framework;
using NSubstitute;

namespace SmartFridge.Tests.Unit
{
    [TestFixture]
    class AdoNetUnitOfWorkIntegrationTest
    {
        private AdoNetUnitOfWork _uut;

        [SetUp]
        public void Setup()
        {
            var subTransaction = Substitute.For<IDbTransaction>();
            var subAction = Substitute.For<Action<AdoNetUnitOfWork>>();
            _uut = new AdoNetUnitOfWork(subTransaction,subAction,subAction);
        }

        [Test]
        public void Dispose_Called_DoesRollback()
        {
            _uut.Dispose();
            _uut.Transaction.Received(1).Rollback();
        }

        [Test]
        public void Dispose_Called_DoesDispose()
        {
            _uut.Dispose();
            _uut.Transaction.Received(1).Dispose();
        }

        [Test]
        public void Dispose_CalledTwiceAndTransactionIsNull_DoesNotRollBackTwice()
        {
            _uut.Dispose();
            _uut.Dispose();
            _uut.Transaction.Received(1).Rollback();
        }

        [Test]
        public void SaveChanges_Called_DoesCommit()
        {
            _uut.SaveChanges();
            _uut.Transaction.Received(1).Commit();
        }

        [Test]
        public void SaveChanges_CalledTwice_ThrowsInvalidOperationException()
        {
            _uut.SaveChanges();
            Assert.That(() => _uut.SaveChanges(),Throws.TypeOf<InvalidOperationException>());
        }
    }
}
