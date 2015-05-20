using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using SmartFridge_WebDAL.Context;
using SmartFridge_WebDAL.UnitOfWork;

namespace DAL.Tests.Unit
{
    [TestFixture]
    class UnitOfWorkUnitTests
    {
        private UnitOfWork _uut;

        [Test]
        public void ListRepo_listRepoIsNull_CreatesNewListRepo()
        {
            var context = Substitute.For<SFContext>();
            _uut = new UnitOfWork(context);
            Assert.That(_uut.ListRepo, Is.Not.Null);
        }

        [Test]
        public void ListRepo_GotListRepo_ReturnsSameListRepo()
        {
            var context = Substitute.For<SFContext>();
            _uut = new UnitOfWork(context);
            var repo = _uut.ListRepo;
            Assert.That(_uut.ListRepo,Is.SameAs(repo));
        }

        [Test]
        public void ItemRepo_itemRepoIsNull_CreatesNewItemRepo()
        {
            var context = Substitute.For<SFContext>();
            _uut = new UnitOfWork(context);
            Assert.That(_uut.ItemRepo, Is.Not.Null);
        }

        [Test]
        public void ItemRepo_GotItemRepo_ReturnsSameItemRepo()
        {
            var context = Substitute.For<SFContext>();
            _uut = new UnitOfWork(context);
            var repo = _uut.ItemRepo;
            Assert.That(_uut.ItemRepo, Is.SameAs(repo));
        }

        [Test]
        public void ListItemRepo_listitemRepoIsNull_CreatesNewListItemRepo()
        {
            var context = Substitute.For<SFContext>();
            _uut = new UnitOfWork(context);
            Assert.That(_uut.ListItemRepo, Is.Not.Null);
        }

        [Test]
        public void ListItemRepo_GotListItemRepo_ReturnsSameListItemRepo()
        {
            var context = Substitute.For<SFContext>();
            _uut = new UnitOfWork(context);
            var repo = _uut.ListItemRepo;
            Assert.That(_uut.ListItemRepo, Is.SameAs(repo));
        }

        [Test]
        public void SaveChanges_CallSaveChanges_ContextRecievesSaveChanges()
        {
            var context = Substitute.For<SFContext>();
            _uut = new UnitOfWork(context);
            _uut.SaveChanges();
            context.Received(1).SaveChanges();
        }

        [Test]
        public void Dispose_DisposedIsFalse_DisposesContext()
        {
            var context = Substitute.For<SFContext>();
            _uut = new UnitOfWork(context);
            _uut.Dispose();
            context.Received(1).Dispose();
        }

        [Test]
        public void Dispose_DisposedCalledTwiceSoDisposedIsTrue_DoesNotDisposeTwice()
        {
            var context = Substitute.For<SFContext>();
            _uut = new UnitOfWork(context);
            _uut.Dispose();
            _uut.Dispose();
            context.Received(1).Dispose();
        }
    }
}
