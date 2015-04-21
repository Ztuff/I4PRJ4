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
            var transaction = Substitute.For<IDbTransaction>();
            var action = Substitute.For<Action<AdoNetUnitOfWork>>();
            _uut = new AdoNetUnitOfWork(transaction,action,action);
        }

        
    }
}
