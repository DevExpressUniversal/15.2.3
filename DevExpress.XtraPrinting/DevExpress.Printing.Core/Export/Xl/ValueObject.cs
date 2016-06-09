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
using DevExpress.Utils;
namespace DevExpress.Export.Xl {
	public class XlValueObject {
		#region Fields
		static readonly XlValueObject empty = new XlValueObject(XlVariantValue.Empty);
		XlVariantValue variantValue;
		XlCellRange rangeValue;
		XlExpression expression;
		#endregion
		protected XlValueObject(XlVariantValue value) {
			this.variantValue = value;
			this.rangeValue = null;
			this.expression = null;
		}
		protected XlValueObject(XlCellRange range) {
			Guard.ArgumentNotNull(range, "range");
			this.variantValue = XlVariantValue.Empty;
			this.rangeValue = range;
			this.expression = null;
		}
		protected XlValueObject(XlExpression expression) {
			Guard.ArgumentNotNull(expression, "expression");
			this.variantValue = XlVariantValue.Empty;
			this.rangeValue = null;
			this.expression = expression;
		}
		#region Properties
		public static XlValueObject Empty { get { return empty; } }
		protected internal XlVariantValue VariantValue { get { return variantValue; } }
		public bool IsEmpty { get { return !IsRange && !IsExpression && variantValue.IsEmpty; } }
		public bool IsNumeric { get { return !IsRange && !IsExpression && variantValue.IsNumeric; } }
		public bool IsBoolean { get { return !IsRange && !IsExpression && variantValue.IsBoolean; } }
		public bool IsText { get { return !IsRange && !IsExpression && !IsFormula && variantValue.IsText; } }
		public bool IsError { get { return !IsRange && !IsExpression && variantValue.IsError; } }
		public bool IsRange { get { return rangeValue != null; } }
		public bool IsExpression { get { return !IsRange && (expression != null); } }
		public bool IsFormula { get { return !IsRange && !IsExpression && variantValue.IsText && !string.IsNullOrEmpty(variantValue.TextValue) && variantValue.TextValue[0] == '='; } }
		public double NumericValue { get { return variantValue.NumericValue; } }
		public DateTime DateTimeValue { get { return variantValue.DateTimeValue; } }
		public bool BooleanValue { get { return variantValue.BooleanValue; } }
		public string TextValue { get { return variantValue.TextValue; } }
		public IXlCellError ErrorValue { get { return variantValue.ErrorValue; } }
		public XlCellRange RangeValue { get { return rangeValue; } }
		public XlExpression Expression { get { return expression; } }
		public string Formula { get { return IsFormula ? variantValue.TextValue : string.Empty; } }
		#endregion
		#region Implicit conversion
		public static implicit operator XlValueObject(double value) {
			return new XlValueObject(value);
		}
		public static implicit operator XlValueObject(DateTime value) {
			return new XlValueObject(value);
		}
		public static implicit operator XlValueObject(bool value) {
			return new XlValueObject(value);
		}
		public static implicit operator XlValueObject(string value) {
			return new XlValueObject(value);
		}
		public static implicit operator XlValueObject(char value) {
			return new XlValueObject(value);
		}
		public static implicit operator XlValueObject(XlVariantValue value) {
			return new XlValueObject(value);
		}
		public static implicit operator XlValueObject(XlCellRange value) {
			return new XlValueObject(value);
		}
		public static implicit operator XlValueObject(XlExpression value) {
			return new XlValueObject(value);
		}
		#endregion
		public static XlValueObject FromObject(object value) {
			return new XlValueObject(XlVariantValue.FromObject(value));
		}
		public override string ToString() {
			if(IsEmpty)
				return string.Empty;
			if(IsRange)
				return RangeValue.ToString();
			return variantValue.ToText().TextValue;
		}
	}
}
