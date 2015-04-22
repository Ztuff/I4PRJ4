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
        [SetUp]
        public void Setup()
        {
            var subConn = Substitute.For<IConnectionFactory>();
            _uut = new AdoNetContext(subConn);
        }

    }
}
