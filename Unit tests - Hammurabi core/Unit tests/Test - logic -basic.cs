// Copyright (c) 2012 Hammura.bi LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using Hammurabi;
using NUnit.Framework;

namespace Hammurabi.UnitTests.CoreFcns
{
    [TestFixture]
    public class BasicLogic : H
    {
        private static Tbool tbt = new Tbool(true);
        private static Tbool tbf = new Tbool(false);
        private static Tbool tbu = new Tbool(Hstate.Unstated);                         // Unknown (not asked)
        private static Tbool tbn = new Tbool(Hstate.Uncertain);                     // Null (user doesn't know)
        private static Tbool tbv = Tbool.MakeTbool(Time.DawnOf, false,         // Time-varying boolean
                                             Date(2000,1,1), true,
                                             Date(2001,1,1), Hstate.Uncertain,
                                             Date(2002,1,1), Hstate.Unstated); 
        
        // AND
        
        [Test]
        public void LogicAnd1 ()
        {
            Tbool t1 = tbt & tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);        
        }
        
        [Test]
        public void LogicAnd2 ()
        {
            Tbool t2 = tbt & tbt;
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t2.TestOutput);        
        }
        
        [Test]
        public void LogicAnd3 ()
        {
            Tbool t7 = tbf & tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t7.TestOutput);        
        }
        
        [Test]
        public void LogicAnd4 ()
        {
            Tbool t1 = tbt & tbt & tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);            
        }
        
        [Test]
        public void LogicAnd5 ()
        {
            Tbool r = tbt & tbn;
            Assert.AreEqual("1/1/0001 12:00:00 AM Uncertain ", r.TestOutput);          
        }
        
        [Test]
        public void LogicAnd6 ()
        {
            Tbool r = tbf & tbn;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", r.TestOutput);          
        }
        
        [Test]
        public void LogicAnd7 ()
        {
            Tbool r = tbn & tbn;
            Assert.AreEqual("1/1/0001 12:00:00 AM Uncertain ", r.TestOutput);          
        }
        
        [Test]
        public void LogicAnd8 ()
        {
            Tbool r = tbu & tbn;
            Assert.AreEqual("1/1/0001 12:00:00 AM Uncertain ", r.TestOutput);          
        }
        
        [Test]
        public void LogicAnd9 ()
        {
            Tbool t1 = tbt & false;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);            
        }
        
        [Test]
        public void LogicAnd10 ()
        {
            Tbool t1 = false & tbt;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);            
        }
        
        [Test]
        public void Unknown_Logic_And_1 ()
        {
            Tbool t1 = tbt & tbf & tbu;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);        
        }
        
        [Test]
        public void Unknown_Logic_And_2 ()
        {
            Tbool t1 = tbt & tbu & tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);        
        }
        
        [Test]
        public void Unknown_Logic_And_3 ()
        {
            Tbool t1 = tbt & tbu;
            Assert.AreEqual("1/1/0001 12:00:00 AM Unstated ", t1.TestOutput);        
        }
        
        [Test]
        public void Unknown_Logic_And_4 ()
        {
            Tbool t1 = tbf & tbu;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);        
        }

        [Test]
        public void LogicAndTime1 ()
        {
            Tbool t1 = tbv & tbu;
            Assert.AreEqual("1/1/0001 12:00:00 AM False 1/1/2000 12:00:00 AM Unstated 1/1/2001 12:00:00 AM Uncertain 1/1/2002 12:00:00 AM Unstated ", t1.TestOutput);            
        }
        
        [Test]
        public void LogicAndTime2 ()
        {
            Tbool t1 = tbv & tbt;
            Assert.AreEqual("1/1/0001 12:00:00 AM False 1/1/2000 12:00:00 AM True 1/1/2001 12:00:00 AM Uncertain 1/1/2002 12:00:00 AM Unstated ", t1.TestOutput);            
        }

        [Test]
        public void LogicAndTime3 ()
        {
            Tbool t1 = tbv & tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);            
        }
        
        [Test]
        public void LogicAndTime4 ()
        {
            Tbool t1 = tbv & tbn;
            Assert.AreEqual("1/1/0001 12:00:00 AM False 1/1/2000 12:00:00 AM Uncertain ", t1.TestOutput);            
        }
        
        // OR
        
        [Test]
        public void LogicOr1 ()
        {
            Tbool t1 = tbt | tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);        
        }
        
        [Test]
        public void LogicOr2 ()
        {
            Tbool t2 = tbt | tbt;
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t2.TestOutput);        
        }
        
        [Test]
        public void LogicOr3 ()
        {
            Tbool t7 = tbf | tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t7.TestOutput);        
        }
        
        [Test]
        public void LogicOr4 ()
        {
            Tbool t1 = tbf | tbf | tbt;
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);            
        }
        
        [Test]
        public void LogicOr5 ()
        {
            Tbool t1 = tbf | (tbf | tbt);
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);            
        }
        
        [Test]
        public void Unknown_Logic_Or_1 ()
        {
            Tbool t1 = tbu | tbf | tbt;
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);        
        }
        
        [Test]
        public void Unknown_Logic_Or_2 ()
        {
            Tbool t1 = tbt | tbu | tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);        
        }
        
        [Test]
        public void Unknown_Logic_Or_3 ()
        {
            Tbool t1 = tbt | tbu;
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);        
        }
        
        [Test]
        public void Unknown_Logic_Or_4 ()
        {
            Tbool t1 = tbf | tbu;
            Assert.AreEqual("1/1/0001 12:00:00 AM Unstated ", t1.TestOutput);         
        }
        
        [Test]
        public void LogicOrTime1 ()
        {
            Tbool t1 = tbv | tbu;
            Assert.AreEqual("1/1/0001 12:00:00 AM Unstated 1/1/2000 12:00:00 AM True 1/1/2001 12:00:00 AM Uncertain 1/1/2002 12:00:00 AM Unstated ", t1.TestOutput);            
        }
        
        [Test]
        public void LogicOrTime2 ()
        {
            Tbool t1 = tbv | tbt;
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);            
        }
        
        [Test]
        public void LogicOrTime3 ()
        {
            Tbool t1 = tbv | tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM False 1/1/2000 12:00:00 AM True 1/1/2001 12:00:00 AM Uncertain 1/1/2002 12:00:00 AM Unstated ", t1.TestOutput);            
        }
        
        [Test]
        public void LogicOrTime4 ()
        {
            Tbool t1 = tbv | tbn;
            Assert.AreEqual("1/1/0001 12:00:00 AM Uncertain 1/1/2000 12:00:00 AM True 1/1/2001 12:00:00 AM Uncertain ", t1.TestOutput);            
        }
        
        // NOT
        
        [Test]
        public void LogicNot1 ()
        {
            Tbool t1 = !tbt;
            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);    
        }
        
        [Test]
        public void LogicNot2 ()
        {
            Tbool t2 = !tbf;
            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t2.TestOutput);        
        }
        
        [Test]
        public void LogicNot3 ()
        {
            Tbool t2 = !tbu;
            Assert.AreEqual("1/1/0001 12:00:00 AM Unstated ", t2.TestOutput);        
        }

        [Test]
        public void LogicNot4 ()
        {
            Tbool t2 = !tbn;
            Assert.AreEqual("1/1/0001 12:00:00 AM Uncertain ", t2.TestOutput);        
        }
        
        [Test]
        public void LogicNot5 () 
        {
            Tbool t1 = !tbv;
            Assert.AreEqual("1/1/0001 12:00:00 AM True 1/1/2000 12:00:00 AM False 1/1/2001 12:00:00 AM Uncertain 1/1/2002 12:00:00 AM Unstated ", t1.TestOutput);        
        }

        [Test]
        public void LogicNot5a ()
        {
            Assert.AreEqual("1/1/0001 12:00:00 AM False 1/1/2000 12:00:00 AM True 1/1/2001 12:00:00 AM Uncertain 1/1/2002 12:00:00 AM Unstated ", tbv.TestOutput);        
        }

        // Basic logic - nested and/or
        
        [Test]
        public void Unknown_Logic_AndOr_1 ()
        {
            Tbool t1 = tbf | ( tbu & tbt );
            Assert.AreEqual("1/1/0001 12:00:00 AM Unstated ", t1.TestOutput);        
        }
        
        // XOR
//        
//        [Test]
//        public void LogicXor1 ()
//        {
//            Tbool t1 = tbt ^ tbf;
//            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);        
//        }
//        
//        [Test]
//        public void LogicXor2 ()
//        {
//            Tbool t2 = tbt ^ tbt;
//            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t2.TestOutput);        
//        }
//        
//        [Test]
//        public void LogicXor3 ()
//        {
//            Tbool t7 = tbf ^ tbf;
//            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t7.TestOutput);        
//        }
//        
//        [Test]
//        public void LogicXor4 ()
//        {
//            Tbool t1 = tbf ^ tbf ^ tbt;
//            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);            
//        }
//        
//        [Test]
//        public void LogicXor5 ()
//        {
//            Tbool t1 = tbt ^ tbf ^ tbf;
//            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);            
//        }
//        
//        [Test]
//        public void LogicXor6 ()
//        {
//            Tbool t1 = tbt ^ tbf ^ tbt;
//            Assert.AreEqual("1/1/0001 12:00:00 AM False ", t1.TestOutput);            
//        }
//        
//        [Test]
//        public void LogicXor7 ()
//        {
//            Tbool t1 = tbf ^ tbt ^ tbf;
//            Assert.AreEqual("1/1/0001 12:00:00 AM True ", t1.TestOutput);            
//        }
//        
        // BOOL COUNT
        
        [Test]
        public void LogicBoolCount1 ()
        {
            Tnum result = BoolCount(true, tbt);
            Assert.AreEqual("1/1/0001 12:00:00 AM 1 ", result.TestOutput);    
        }
        
        [Test]
        public void LogicBoolCount2 ()
        {
            Tnum result = BoolCount(false, tbt);
            Assert.AreEqual("1/1/0001 12:00:00 AM 0 ", result.TestOutput);    
        }
        
        [Test]
        public void LogicBoolCount3 ()
        {
            Tnum result = BoolCount(false, tbt, tbt);
            Assert.AreEqual("1/1/0001 12:00:00 AM 0 ", result.TestOutput);    
        }
        
        [Test]
        public void LogicBoolCount4 ()
        {
            Tnum result = BoolCount(true, tbt, tbt);
            Assert.AreEqual("1/1/0001 12:00:00 AM 2 ", result.TestOutput);    
        }
        
        [Test]
        public void LogicBoolCount5 ()
        {
            Tnum result = BoolCount(true, tbt, tbf);
            Assert.AreEqual("1/1/0001 12:00:00 AM 1 ", result.TestOutput);    
        }
        
    }
}