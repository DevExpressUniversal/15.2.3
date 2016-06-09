#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
namespace DevExpress.XtraReports.Native.Summary {
	public abstract class EvaluatorBase {
		static IList GetDistinctValues(IList values) {
			HashSet<object> set = new HashSet<object>();
			int count = values.Count;
			for(int i = 0; i < count; i++) {
				set.Add(values[i]);
			}
			return new List<object>(set);
		}
		public static int CalcDistinctCount(IList values) {
			return GetDistinctValues(values).Count;
		}
		protected abstract object ZeroValue { get; }
		protected abstract object MinValue { get; }
		protected abstract object MaxValue { get; }
		protected abstract object Cvt(object obj);
		protected abstract object Add(object o1, object o2);
		protected abstract object Subtract(object o1, object o2);
		protected abstract object Divide(object o, int divizor);
		protected abstract object Square(object o);
		protected abstract object Sqrt(object o);
		protected abstract bool LessThan(object o1, object o2);
		protected abstract bool MoreThan(object o1, object o2);
		public object Sum(IList values) {
			if(values.Count == 0)
				return null;
			object result = ZeroValue;
			int count = values.Count;
			for(int i = 0; i < count; i++)
				result = Add(result, Cvt(values[i]));
			return result;
		}
		public object Max(IList values) {
			bool found = false;
			object result = MinValue;
			int count = values.Count;
			for(int i = 0; i < count; i++) {
				object obj = values[i];
				if(obj != null && !(obj is DBNull)) {
					object d = Cvt(obj);
					if(LessThan(result, d))
						result = d;
					found = true;
				}
			}
			return found ? result : null;
		}
		public object Min(IList values) {
			bool found = false;
			object result = MaxValue;
			int count = values.Count;
			for(int i = 0; i < count; i++) {
				object obj = values[i];
				if(obj != null && !(obj is DBNull)) {
					object d = Cvt(values[i]);
					if(MoreThan(result, d))
						result = d;
					found = true;
				}
			}
			return found ? result : null;
		}
		public object Median(IList values) {
			if(values.Count == 0)
				return null;
			int middlePoint = (int)(values.Count / 2);
			ArrayList list = new ArrayList(values);
			list.Sort();
			if(values.Count % 2 != 0)
				return Cvt(list[middlePoint]);
			return Avg(new List<object>(new object[] { list[middlePoint - 1], list[middlePoint] }));
		}
		public virtual object Avg(IList values) {
			if(values.Count == 0)
				return null;
			return Divide(Sum(values), values.Count);
		}
		object SumOfSquareOfDifference(IList values) {
			object average = Avg(values);
			object result = ZeroValue;
			int count = values.Count;
			for(int i = 0; i < count; i++) {
				object d = Subtract(average, Cvt(values[i]));
				result = Add(result, Square(d));
			}
			return result;
		}
		public virtual object Var(IList values) {
			if(values.Count <= 1)
				return null;
			object result = SumOfSquareOfDifference(values);
			return Divide(result, (values.Count - 1));
		}
		public virtual object VarP(IList values) {
			if(values.Count <= 0)
				return null;
			object result = SumOfSquareOfDifference(values);
			return Divide(result, values.Count);
		}
		public object StdDev(IList values) {
			if(values.Count <= 1)
				return null;
			return Sqrt(Var(values));
		}
		public object StdDevP(IList values) {
			if(values.Count <= 0)
				return null;
			return Sqrt(VarP(values));
		}
		public object DAvg(IList values) {
			return Avg(GetDistinctValues(values));
		}
		public object DSum(IList values) {
			return Sum(GetDistinctValues(values));
		}
		public object DVar(IList values) {
			return Var(GetDistinctValues(values));
		}
		public object DVarP(IList values) {
			return VarP(GetDistinctValues(values));
		}
		public object DStdDev(IList values) {
			return StdDev(GetDistinctValues(values));
		}
		public object DStdDevP(IList values) {
			return StdDevP(GetDistinctValues(values));
		}
	}
}
