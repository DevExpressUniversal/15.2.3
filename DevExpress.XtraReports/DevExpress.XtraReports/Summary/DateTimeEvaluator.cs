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
using System.ComponentModel;
namespace DevExpress.XtraReports.Native.Summary {
	public class DateTimeEvaluator : EvaluatorBase {
		protected override object ZeroValue {
			get { return DateTime.MinValue; }
		}
		protected override object MinValue {
			get { return DateTime.MinValue; }
		}
		protected override object MaxValue {
			get { return DateTime.MaxValue; }
		}
		protected override object Cvt(object obj) {
			try {
				return obj is DateTime ? obj : 
					obj is DBNull ? ZeroValue :
					Convert.ToDateTime(obj);
			} catch {
				return ZeroValue;
			}
		}
		protected override object Add(object o1, object o2) {
			return new DateTime(((DateTime)o1).Ticks + ((DateTime)o2).Ticks);
		}
		protected override object Subtract(object o1, object o2) {
			if(MoreThan(o1, o2))
				return new DateTime(((DateTime)o1).Ticks - ((DateTime)o2).Ticks);
			else
				return new DateTime(((DateTime)o2).Ticks - ((DateTime)o1).Ticks);
		}
		protected override object Divide(object o, int divizor) {
			return new DateTime(((DateTime)o).Ticks / divizor);
		}
		protected override object Square(object o) {
			return new DateTime(((DateTime)o).Ticks * ((DateTime)o).Ticks);
		}
		protected override object Sqrt(object o) {
			return new DateTime((long)Math.Sqrt(((DateTime)o).Ticks));
		}
		protected override bool LessThan(object o1, object o2) {
			return (DateTime)o1 < (DateTime)o2;
		}
		protected override bool MoreThan(object o1, object o2) {
			return (DateTime)o1 > (DateTime)o2;
		}
		public override object Avg(IList values) {
			if(values.Count == 0)
				return null;
			object result = ZeroValue;
			int count = values.Count;
			for(int i = 0; i < count; i++)
				result = Add(result, Divide(Cvt(values[i]), values.Count));
			return result;
		}
		object SumOfSquareOfDifference(IList values, int divizor) {
			object average = Avg(values);
			object result = ZeroValue;
			int count = values.Count;
			for(int i = 0; i < count; i++) {
				DateTime d = (DateTime)Subtract(average, Cvt(values[i]));
				d = new DateTime(new DateTime(d.Ticks / divizor).Ticks * d.Ticks);
				result = Add(result, d);
			}
			return result;
		}
		public override object Var(IList values) {
			if(values.Count <= 1)
				return null;
			return SumOfSquareOfDifference(values, values.Count - 1);
		}
		public override object VarP(IList values) {
			if(values.Count <= 0)
				return null;
			return SumOfSquareOfDifference(values, values.Count);
		}
	}
	public class TimeSpanEvaluator : DateTimeEvaluator {
		protected override object ZeroValue {
			get { return TimeSpan.Zero; }
		}
		protected override object MinValue {
			get { return TimeSpan.MinValue; }
		}
		protected override object MaxValue {
			get { return TimeSpan.MaxValue; }
		}
		protected override object Cvt(object obj) {
			try {
				return obj is TimeSpan ? obj :
					obj is DBNull ? ZeroValue :
					new TimeSpanConverter().ConvertFrom(obj);
			} catch {
				return ZeroValue;
			}
		}
		protected override object Add(object o1, object o2) {
			return ((TimeSpan)o1).Add(((TimeSpan)o2));
		}
		protected override object Subtract(object o1, object o2) {
			if(MoreThan(o1, o2))
				return ((TimeSpan)o1).Subtract(((TimeSpan)o2));
			else
				return ((TimeSpan)o2).Subtract(((TimeSpan)o1));
		}
		protected override object Divide(object o, int divizor) {
			return new TimeSpan(((TimeSpan)o).Ticks / divizor);
		}
		protected override object Square(object o) {
			return new TimeSpan(((TimeSpan)o).Ticks * ((TimeSpan)o).Ticks);
		}
		protected override object Sqrt(object o) {
			return new TimeSpan((long)Math.Sqrt(((TimeSpan)o).Ticks));
		}
		protected override bool LessThan(object o1, object o2) {
			return (TimeSpan)o1 < (TimeSpan)o2;
		}
		protected override bool MoreThan(object o1, object o2) {
			return (TimeSpan)o1 > (TimeSpan)o2;
		}
		object SumOfSquareOfDifference(IList values, int divizor) {
			object average = Avg(values);
			object result = ZeroValue;
			int count = values.Count;
			for(int i = 0; i < count; i++) {
				TimeSpan d = (TimeSpan)Subtract(average, Cvt(values[i]));
				d = new TimeSpan(new TimeSpan(d.Ticks / divizor).Ticks * d.Ticks);
				result = Add(result, d);
			}
			return result;
		}
	}
}
