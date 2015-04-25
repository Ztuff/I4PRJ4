//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using BusinessLogicLayer;
//using NUnit.Framework;
//using UserControlLibrary;

//namespace SmartFridge.Tests.Unit
//{
//    [TestFixture]
//    public class CtrlShowListSelectionTest
//    {

//        public CtrlShowListSelection uut;
//        public CtrlTemplate uutCtrlTemplate;
//        public CtrlItemList ContentForGrid;

//        [SetUp]
//        public void SetUp()
//        {
//            //Starter ny tråd som single threaded apartment (STA), for at kunne lave CtrlTemplate
//            Thread threadCT = new Thread(() => { uutCtrlTemplate = new CtrlTemplate(); });
//            threadCT.SetApartmentState(ApartmentState.STA);
//            threadCT.Start();
//            threadCT.Join();
//            //Starter ny tråd som STA for at kunne lave CtrlShowListSelection
//            Thread threadUUT = new Thread(() => { uut = new CtrlShowListSelection(uutCtrlTemplate); });
//            threadUUT.SetApartmentState(ApartmentState.STA);
//            threadUUT.Start();
//            threadUUT.Join();
//        }

//        [Test]
//        public void BtnInFridge_Clicked()
//        {
//            //Starter ny tråd som STA for at kunne lave CtrlItemList
//            Thread threadCFG = new Thread(() => { ContentForGrid = new CtrlItemList("Køleskab", uutCtrlTemplate); });
//            threadCFG.SetApartmentState(ApartmentState.STA);
//            threadCFG.Start();
//            threadCFG.Join();
//            //ContentForGrid = new CtrlItemList("Køleskab", uutCtrlTemplate);
//            uutCtrlTemplate.ChangeGridContent(ContentForGrid);
//            Assert.AreEqual(ContentForGrid, uutCtrlTemplate.Content);
//        }
//    }
//}
