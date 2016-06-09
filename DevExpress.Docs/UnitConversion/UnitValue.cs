#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.UnitConversion {
	#region QuantityValue<T>
	public struct QuantityValue<T> where T : struct {
		public double Value { get; set; }
		internal UnitCategory Category { get; set; }
		internal int Index { get; set; }
		#region Addition
		public static QuantityValue<T> operator +(QuantityValue<T> value1, QuantityValue<T> value2) {
			QuantityValue<T> result = value1;
			if (value1.Index == value2.Index)
				result.Value += value2.Value;
			else
				result.Value += value2.Category.Convert(value2.Value, value2.Index, result.Index);
			return result;
		}
		public static QuantityValue<T> operator +(QuantityValue<T> value1, double value2) {
			QuantityValue<T> result = value1;
			result.Value += value2;
			return result;
		}
		public static QuantityValue<T> operator ++(QuantityValue<T> value1) {
			value1.Value++;
			return value1;
		}
		#endregion
		#region Subtraction
		public static QuantityValue<T> operator -(QuantityValue<T> value1, QuantityValue<T> value2) {
			QuantityValue<T> result = value1;
			if (value1.Index == value2.Index)
				result.Value -= value2.Value;
			else
				result.Value -= value2.Category.Convert(value2.Value, value2.Index, result.Index);
			return result;
		}
		public static QuantityValue<T> operator -(QuantityValue<T> value1, double value2) {
			QuantityValue<T> result = value1;
			result.Value -= value2;
			return result;
		}
		public static QuantityValue<T> operator --(QuantityValue<T> value1) {
			value1.Value--;
			return value1;
		}
		#endregion
		#region Multiplication
		public static QuantityValue<T> operator *(QuantityValue<T> value1, double value2) {
			QuantityValue<T> result = value1;
			result.Value *= value2;
			return result;
		}
		public static QuantityValue<T> operator /(QuantityValue<T> value1, double value2) {
			QuantityValue<T> result = value1;
			result.Value /= value2;
			return result;
		}
		#endregion
		public static implicit operator double(QuantityValue<T> value) {
			return value.Value;
		}
	}
	#endregion
}
