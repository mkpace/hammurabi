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
using System.Collections.Generic;

namespace Hammurabi
{
    public abstract partial class H
    {
        /// <summary>
        /// Gets all time points and assocated values from the input Tvar objects.
        /// </summary>
        protected static SortedList<DateTime,List<Hval>> TimePointValues(params Tvar[] list)
        {
            SortedList<DateTime,List<Hval>> result = new SortedList<DateTime, List<Hval>>();
            
            // Foreach time point
            foreach (DateTime d in TimePoints(list))
            {
                List<Hval> vals = new List<Hval>();
                
                // Make list of values at that point in time
                foreach (Tvar h in list)
                {
                    vals.Add(h.ObjectAsOf(d));
                }
                
                result.Add(d,vals);
            }
            
            return result;
        }

        /// <summary>
        /// Gets all time points in a set of Tvar objects.
        /// </summary>
        public static List<DateTime> TimePoints(params Tvar[] list)
        {
            List<Tvar> arrayToList = new List<Tvar>(list.Length);
            arrayToList.AddRange(list);
            return TimePoints(arrayToList);
        }

        public static List<DateTime> TimePoints(List<Tvar> list)
        {
            List<DateTime> bps = new List<DateTime>();
            
            foreach (Tvar v in list)
            {
                foreach (DateTime d in v.TimePoints())
                {
                    if (!bps.Contains(d))
                    {
                        bps.Add(d);
                    }
                }
            }
            
            return bps;
        }
        
        /// <summary>
        /// Apply a function to all values in a Tnum object.
        /// </summary>
        public static T ApplyFcnToTimeline<T>(Func<List<Hval>,Hval> fcn, params Tvar[] list) where T : Tvar
        {
            T result = (T)Util.ReturnProperTvar<T>();

            foreach(KeyValuePair<DateTime,List<Hval>> slice in TimePointValues(list))
            {  
                Hstate top = PrecedingState(slice.Value);
                if (top != Hstate.Known)
                {
                    result.AddState(slice.Key, new Hval(null, top));
                }
                else
                {
                    result.AddState(slice.Key, fcn(slice.Value));
                }
            }

            return result.LeanTvar<T>();
        }
    }    
}