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
namespace DevExpress.XtraReports.Native.Summary {
	public class DecimalEvaluator : EvaluatorBase {
		protected override object ZeroValue {
			get { return (decimal)0.0; }
		}
		protected override object MinValue {
			get { return decimal.MinValue; }
		}
		protected override object MaxValue {
			get { return decimal.MaxValue; }
		}
		protected override object Cvt(object obj) {
			try {
				return obj is Decimal ? obj :
					obj is DBNull ? ZeroValue :
					Convert.ToDecimal(obj);
			} catch {
				return ZeroValue;
			}
		}
		protected override object Add(object o1, object o2) {
			return (decimal)o1 + (decimal)o2;
		}
		protected override object Subtract(object o1, object o2) {
			return (decimal)o1 - (decimal)o2;
		}
		protected override object Divide(object o, int divizor) {
			return (decimal)o / divizor;
		}
		protected override object Square(object o) {
			return (decimal)o * (decimal)o;
		}
		protected override object Sqrt(object o) {
			return Math.Sqrt(Convert.ToDouble(o));
		}
		protected override bool LessThan(object o1, object o2) {
			return (decimal)o1 < (decimal)o2;
		}
		protected override bool MoreThan(object o1, object o2) {
			return (decimal)o1 > (decimal)o2;
		}
	}
}
