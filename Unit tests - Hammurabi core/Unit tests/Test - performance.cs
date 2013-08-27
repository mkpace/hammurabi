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
    public class Performance : H
    {
        [Test]
        public void RunningElapsedIntervals_Performance ()
        {
            Tbool tb = new Tbool(false);
            tb.AddState(new DateTime(2015,1,1),true);
            tb.AddState(new DateTime(2015,1,2),false);
            tb.AddState(new DateTime(2015,1,3),true);
            tb.AddState(new DateTime(2015,1,4),false);

            DateTime startTime = DateTime.Now;

            //            18ms
            //            Tbool t = tb && !tb || tb;

            //            10ms
            //            Tnum t = tb.TotalElapsedDays(Time.DawnOf, Time.EndOf);

            //            11ms
            //            Tnum t = tb.RunningElapsedTime(Time.IntervalType.Day);

            //            40ms
            //            Tnum t = tb.RunningElapsedIntervals(TheDay);

            //            40ms
            //            Tnum t = tb.ContinuousElapsedIntervals(TheDay);

            //            45ms
            //            Tbool t = tb.Shift(4, TheDay);

            //            75ms
            //            Tnum t = tb.SlidingElapsedIntervals(TheDay, 4);

            int ResponseTimeInMs = Convert.ToInt32((DateTime.Now - startTime).TotalMilliseconds);

            // The sole purpose of this unit test is to examine performance, so it will always fail...
            Assert.AreEqual(ResponseTimeInMs, 0);    
        }
    }
}