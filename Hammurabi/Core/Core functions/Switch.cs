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

using System;
using System.Collections.Generic;

namespace Hammurabi
{
	public partial class H
	{		
		/// <summary>
		/// Returns a Tvar when its associated Tbool is true.  
		/// </summary>
        /// <remarks>
        /// Similar in principle to a C# switch statement, just temporal.
        /// Sample usage: Switch(Tbool1, T1, Tbool2, T2, ..., defaultT).  
        /// Returns T1 if Tbool2 is true, else T2 if Tbool2 is true, etc., else defaultT. 
        /// </remarks>
        public static T Switch<T>(params object[] arguments) where T : Tvar
        {
            Hval h = new Hval(null, Hstate.Null);
            T result = (T)Auxiliary.ReturnProperTvar<T>(h);

            // Keep going until all intervals are defined...
            while (Util.HasUndefinedIntervals(result)) 
            {
                // For each condition-value pair...
                int len = (int)arguments.Length;
                for (int arg=0; arg < len-1; arg+=2)
                {
                    // Get value of the condition
                    Tbool newCondition = Auxiliary.ConvertToTvar<Tbool>(arguments[arg]);

                    // Identify the intervals when the new condition is neither false nor true
                    // Falsehood causes it to fall through to next condition. Truth causes the
                    // result to assume the value during that interval.
                    Tbool newConditionIsUnknown = Util.HasUnknownState(newCondition);

                    // Merge these 'unknown' intervals in new condition into the result.
                    result = Util.MergeTvars<T>(result,
                                           Util.ConditionalAssignment<T>(newConditionIsUnknown, newCondition));

                    // Identify the intervals when the new condition is true.
                    // Ignore irrelevant periods when result is already determined.
                    // During these intervals, "result" takes on the value of the pair.
                    Tbool newConditionIsTrueAndResultIsNull = newCondition && Util.IsNull(result);

                    // If new true segments are added, accumulate the values during those intervals
                    if (newConditionIsTrueAndResultIsNull.IsEverTrue())
                    {
                        T val = (T)Auxiliary.ConvertToTvar<T>(arguments[arg+1]);
                        result = Util.MergeTvars<T>(result,
                                               Util.ConditionalAssignment<T>(newConditionIsTrueAndResultIsNull, val)); 
                    }
                }

                T defaultVal = (T)Auxiliary.ConvertToTvar<T>(arguments[len-1]);
                result = Util.MergeTvars<T>(result, defaultVal);

                break;
            }

            return result.LeanTvar<T>();
        }

    }

    public partial class Util
    {
        /// <summary>
        /// When a Tbool (tb) is true, get the value of one Tvar (val) and assign it to 
        /// a second Tvar (result).
        /// </summary>
        /// <remarks>
        /// Example:        tb = <--F--|--T--|--F--|--T--|--F--> 
        ///                val = <--------------4--------------> 
        ///             result = <--n--|--4--|--n--|--4--|--n-->  where n = Hstate.Null
        /// </remarks>
        public static T ConditionalAssignment<T>(Tbool tb, Tvar val) where T : Tvar
        {
            T result = (T)Auxiliary.ReturnProperTvar<T>();

            foreach (DateTime d in H.TimePoints(tb,val))
            {
                if (tb.ObjectAsOf(d).IsTrue)
                {
                    Hval valAsOf = val.ObjectAsOf(d);
                    result.AddState(d, valAsOf);
                }
                else
                {
                    result.AddState(d, new Hval(null,Hstate.Null));
                }
            }

            return result.LeanTvar<T>();
        }

        /// <summary>
        /// Merges the values from two Tvars. If intervals are in conflict, 
        /// the first Tvar takes priority.
        /// </summary>
        /// <remarks>
        /// Example:       tv1 = <--0--|--1--|--u--|--3--|--u--> 
        ///                tv2 = <--9--|--u--|--2--|--u--|--u--> 
        ///             result = <--0--|--1--|--2--|--3--|--u--> 
        /// </remarks>
        public static T MergeTvars<T>(T tv1, T tv2) where T : Tvar
        {
            T result = (T)Auxiliary.ReturnProperTvar<T>();

            foreach (DateTime d in H.TimePoints(tv1,tv2))
            {
                Hval hv1 = tv1.ObjectAsOf(d);
                Hval hv2 = tv2.ObjectAsOf(d);

                if (hv1.State != Hstate.Null)
                {
                    result.AddState(d, hv1);
                }
                else if (hv2.State != Hstate.Null)
                {
                    result.AddState(d,hv2);
                }
                else
                {
                    result.AddState(d, new Hval(null,Hstate.Null));
                }
            }

            return result.LeanTvar<T>();
        }

        /// <summary>
        /// Identifies intervals where a Tvar is Stub, Uncertain, or Unstated.
        /// </summary>
        public static Tbool HasUnknownState(Tvar tvar) 
        {
            Tbool result = new Tbool();
            
            foreach (KeyValuePair<DateTime,Hval> slice in tvar.IntervalValues)
            {
                if (slice.Value.IsStub || slice.Value.IsUncertain || slice.Value.IsUnstated)
                {
                    result.AddState(slice.Key, true);
                }
                else
                {
                    result.AddState(slice.Key, false);
                }
            }
            
            return result.Lean;
        }

        /// <summary>
        /// Identifies intervals where a Tvar is Hstate.Null.
        /// </summary>
        public static Tbool IsNull(Tvar tvar) 
        {
            Tbool result = new Tbool();
            
            foreach (KeyValuePair<DateTime,Hval> slice in tvar.IntervalValues)
            {
                result.AddState(slice.Key, slice.Value.State == Hstate.Null);
            }
            
            return result.Lean;
        }

        /// <summary>
        /// Determines whether a Tvar ever has any undefined intervals (where Hstate = Null).
        /// </summary>
        public static bool HasUndefinedIntervals(Tvar tvar) 
        {
            foreach (Hval h in tvar.IntervalValues.Values)
            {
                if (h.State == Hstate.Null) return true;
            }
            return false;
        }
	}
}