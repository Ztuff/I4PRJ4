using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using NSubstitute;
using NUnit.Framework;

namespace DAL.Tests.Unit
{
    [TestFixture]
    class SmartFridgeDALFacadeUnitTests
    {
        private SmartFridgeDALFacade _uut;

        [Test]
        public void Ctor_parameterisTest_DatabaseNameIsTest()
        {
            _uut = new SmartFridgeDALFacade("Test");
            Assert.That(_uut.DatabaseName, Is.EqualTo("Test"));
        }

        [Test]
        public void GetUnitOfWork_CalledAndDatabaseNameIsNull_ReturnsUoW()
        {
            _uut = new SmartFridgeDALFacade();
            var uow = _uut.GetUnitOfWork();
            Assert.That(uow, Is.TypeOf<UnitOfWork.UnitOfWork>());
        }

        [Test]
        public void GetUnitOfWork_CalledAndDatabaseNameIsTest_ReturnsUoW()
        {
            _uut = new SmartFridgeDALFacade("Test");
            var uow = _uut.GetUnitOfWork();
            Assert.That(uow, Is.TypeOf<UnitOfWork.UnitOfWork>());
        }

        [Test]
        public void GetUnitOfWork_CalledTwice_ThrowsInvalidOperationException()
        {
            _uut = new SmartFridgeDALFacade("Test");
            _uut.GetUnitOfWork();
            Assert.That(() => _uut.GetUnitOfWork(),Throws.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void DisposeUnitOfWork_CallsGetThenDisposeThenGet_DoesNotThrowException()
        {
            _uut = new SmartFridgeDALFacade("Test");
            _uut.GetUnitOfWork();
            _uut.DisposeUnitOfWork();
            Assert.That(() => _uut.GetUnitOfWork(), Throws.Nothing);
        }

        [Test]
        public void DisposeUnitOfWork_CalledAndUnitOfWorkIsNull_DoesNotThrowException()
        {
            Assert.That(() => _uut.DisposeUnitOfWork(), Throws.Nothing);
        }
    }
}
