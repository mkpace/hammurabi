// Copyright (c) 2012-2013 Hammura.bi LLC
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

using System;
using Hammurabi;
using NUnit.Framework;

namespace Hammurabi.UnitTests.CoreFcns
{
    [TestFixture]
    public class RunningElapsedTime : H
    {   
        // RunningElapsedIntervals

        [Test]
        public void RunningElapsedIntervals1 ()
        {
            Tbool tb = new Tbool(false);
            tb.AddState(new DateTime(2015,1,1),true);
            tb.AddState(new DateTime(2015,1,2),false);
            tb.AddState(new DateTime(2015,1,3),true);
            tb.AddState(new DateTime(2015,1,4),false);

            Tnum r = tb.RunningElapsedIntervals(TheDay);

            Assert.AreEqual("{Dawn: 0; 1/1/2015: 1; 1/3/2015: 2}", r.Out);    
        }

        [Test]
        public void RunningElapsedIntervals2 ()
        {
            Tbool tb = new Tbool(false);
            Tnum r = tb.RunningElapsedIntervals(TheYear);

            Assert.AreEqual(0, r.Out);    
        }

        [Test]
        public void RunningElapsedIntervals3 ()
        {
            Tbool tb = new Tbool(false);
            tb.AddState(new DateTime(2000,1,1), Hstate.Unstated);
            tb.AddState(new DateTime(2000,3,1), false);

            Tnum r = tb.RunningElapsedIntervals(TheYear);

            Assert.AreEqual("Unstated", r.Out);    
        }

        [Test]
        public void RunningElapsedIntervals4 ()
        {
            Tbool tb = new Tbool(false);
            tb.AddState(new DateTime(2000,1,1), Hstate.Stub);
            tb.AddState(new DateTime(2000,3,1), false);

            Tnum r = tb.RunningElapsedIntervals(TheYear);

            Assert.AreEqual("Stub", r.Out);    
        }


        // RunningElapsedTime

        [Test]
        public void RunningElapsedTime1 ()
        {
            Tbool tb = new Tbool(false);
            tb.AddState(new DateTime(2000,1,1),true);
            tb.AddState(new DateTime(2000,1,2),false);
            tb.AddState(new DateTime(2000,1,3),true);
            tb.AddState(new DateTime(2000,1,4),false);

            Tnum r = tb.RunningElapsedDays;

            Assert.AreEqual("{Dawn: 0; 1/2/2000: 1; 1/4/2000: 2}", r.Out);    
        }

        [Test]
        public void RunningElapsedTime2 ()
        {
            Tbool tb = new Tbool(false);
            Tnum r = tb.RunningElapsedYears;

            Assert.AreEqual(0, r.Out);    
        }

        [Test]
        public void RunningElapsedTime3 ()
        {
            Tbool tb = new Tbool(false);
            tb.AddState(new DateTime(2000,1,1), Hstate.Unstated);
            tb.AddState(new DateTime(2000,3,1), false);

            Tnum r = tb.RunningElapsedWeeks;

            Assert.AreEqual("Unstated", r.Out);    
        }

        [Test]
        public void RunningElapsedTime4 ()
        {
            Tbool tb = new Tbool(false);
            tb.AddState(new DateTime(2000,1,1), Hstate.Stub);
            tb.AddState(new DateTime(2000,3,1), false);

            Tnum r = tb.RunningElapsedWeeks;

            Assert.AreEqual("Stub", r.Out);    
        }
    }
}