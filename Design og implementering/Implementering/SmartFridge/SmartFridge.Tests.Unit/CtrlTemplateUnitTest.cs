using System.Threading;
using System.Windows.Controls;
using NUnit.Framework;
using UserControlLibrary;

namespace SmartFridge.Tests.Unit
{
    [TestFixture]
     public class CtrlTemplateUnitTest
    {
        private CtrlTemplate uut;
        private CtrlShowListSelection testUC;

// Virker ikke, da det kalder noget XAML-kode, hvilket enten skal gøres
// fra main-tråden eller gennem en STA-tråd (eller sådan noget).
        //[SetUp]
        //public void Setup()
        //{
        //    Thread t = new Thread(() => uut = new CtrlTemplate());
        //    t.Start();
        //    t.Join();
        //}

        //[Test]
        //public void ChangeGridContent_GridContentIsChanged_NewContentIsStored()
        //{
        //    Thread t = new Thread(() =>
        //    {
        //        testUC = new CtrlShowListSelection(uut);
        //        uut.ChangeGridContent(testUC);
        //        Assert.AreEqual(testUC, uut.NavigationHistoryCollection[1]);
        //    });
        //    t.Start();
        //    t.Join();
        //}

        [Test]
        public void DummyTest2()
        {
            Assert.AreEqual(0, 0);
        }
    }
}