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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using DevExpress.Office;
using Model = DevExpress.XtraSpreadsheet.Model;
using ModelCellRange = DevExpress.XtraSpreadsheet.Model.CellRange;
using ModelVariantValue = DevExpress.XtraSpreadsheet.Model.VariantValue;
namespace DevExpress.Spreadsheet {
	using DevExpress.XtraSpreadsheet.API.Native.Implementation;
	public class ValueObject {
		#region Fields
		static readonly ValueObject empty = new ValueObject(ModelVariantValue.Empty);
		string formula;
		string formulaInvariant;
		ModelVariantValue modelValue;
		NativeWorksheet worksheet;
		#endregion
		internal ValueObject(ModelVariantValue modelValue, NativeWorksheet worksheet) {
			Debug.Assert(!modelValue.IsSharedString);
			this.formula = string.Empty;
			this.formulaInvariant = string.Empty;
			this.modelValue = modelValue;
			this.worksheet = worksheet;
		}
		internal ValueObject(ModelVariantValue modelValue) {
			Debug.Assert(!modelValue.IsSharedString && !modelValue.IsCellRange);
			this.formula = string.Empty;
			this.formulaInvariant = string.Empty;
			this.modelValue = modelValue;
			this.worksheet = null;
		}
		internal ValueObject(string formula, string formulaInvariant) {
			this.formula = string.IsNullOrEmpty(formula) ? string.Empty : formula;
			this.formulaInvariant = string.IsNullOrEmpty(formulaInvariant) ? string.Empty : formulaInvariant;
			this.modelValue = ModelVariantValue.Empty;
			this.worksheet = null;
		}
		#region Properties
		public static ValueObject Empty { get { return ValueObject.empty; } }
		protected internal ModelVariantValue ModelValue { get { return modelValue; } }
		public bool IsFormula { get { return !string.IsNullOrEmpty(formula) || !string.IsNullOrEmpty(formulaInvariant); } }
		public string Formula { get { return formula; } }
		public string FormulaInvariant { get { return formulaInvariant; } }
		public bool IsEmpty { get { return !IsFormula && modelValue.IsEmpty; } }
		public bool IsNumeric { get { return !IsFormula && modelValue.IsNumeric; } }
		public bool IsBoolean { get { return !IsFormula && modelValue.IsBoolean; } }
		public bool IsError { get { return !IsFormula && modelValue.IsError; } }
		public bool IsText { get { return !IsFormula && modelValue.IsInlineText; } }
		public bool IsRange { get { return !IsFormula && modelValue.IsCellRange; } }
		public double NumericValue { get { return modelValue.NumericValue; } }
		public bool BooleanValue { get { return modelValue.BooleanValue; } }
		public ErrorValueInfo ErrorValue { get { return !modelValue.IsError ? null : new NativeErrorInfo(modelValue.ErrorValue); } }
		public string TextValue { get { return modelValue.InlineTextValue; } }
		public Range RangeValue { get { return new NativeRange(modelValue.CellRangeValue, worksheet); } }
		#endregion
		#region Implicit conversion
		public static implicit operator ValueObject(int value) {
			return new ValueObject(value);
		}
		public static implicit operator ValueObject(double value) {
			return new ValueObject(value);
		}
		public static implicit operator ValueObject(char value) {
			return new ValueObject(value);
		}
		public static implicit operator ValueObject(string value) {
			if (!string.IsNullOrEmpty(value) && value.StartsWith("=", StringComparison.Ordinal))
				return FromFormula(value, true);
			return new ValueObject(value);
		}
		public static implicit operator ValueObject(DateTime value) {
			return FromDateTime(value, false);
		}
		public static implicit operator ValueObject(bool value) {
			return new ValueObject(value);
		}
		public static implicit operator ValueObject(CellValue value) {
			if (value != null) {
				if (value.IsText)
					return new ValueObject(value.TextValue);
				if (value.Type != CellValueType.Unknown)
					return new ValueObject(value.ModelVariantValue);
			}
			return new ValueObject(ModelVariantValue.Empty);
		}
		#endregion
		public static ValueObject FromRange(Range range) {
			ModelVariantValue modelVariantValue = new ModelVariantValue();
			NativeWorksheet nativeWorksheet = range.Worksheet as NativeWorksheet;
			modelVariantValue.CellRangeValue = nativeWorksheet == null ? null : nativeWorksheet.GetModelSingleRange(range);
			return new ValueObject(modelVariantValue, nativeWorksheet);
		}
		public static ValueObject FromDateTime(DateTime value, bool use1904DateSystem) {
			ModelVariantValue modelVariantValue = new ModelVariantValue();
			modelVariantValue.SetDateTime(value, use1904DateSystem ? Model.DateSystem.Date1904 : Model.DateSystem.Date1900);
			return new ValueObject(modelVariantValue);
		}
		public static ValueObject FromFormula(string formula, bool invariant) {
			if (!string.IsNullOrEmpty(formula) && !formula.StartsWith("=", StringComparison.Ordinal))
				formula = formula.Insert(0, "=");
			return new ValueObject(invariant ? string.Empty : formula, invariant ? formula : string.Empty);
		}
		public DateTime GetDateTimeValue(bool use1904DateSystem) {
			if (!IsNumeric)
				return DateTime.MinValue;
			try {
				Model.DateSystem dateSystem = use1904DateSystem ? Model.DateSystem.Date1904 : Model.DateSystem.Date1900;
				if (Model.WorkbookDataContext.IsErrorDateTimeSerial(modelValue.NumericValue, dateSystem))
					return DateTime.MinValue;
				return modelValue.ToDateTime(dateSystem);
			}
			catch (ArgumentOutOfRangeException) {
				return DateTime.MinValue;
			}
			catch (OverflowException) {
				return DateTime.MaxValue;
			}
		}
		#region Equality
		public override bool Equals(object obj) {
			ValueObject other = obj as ValueObject;
			if (other == null)
				return false;
			if (Object.ReferenceEquals(this, other))
				return true;
			if (IsFormula && other.IsFormula)
				return Formula == other.Formula || FormulaInvariant == other.FormulaInvariant;
			if (IsRange) {
				Model.CellRangeBase rangeValue = modelValue.CellRangeValue;
				if (other.IsRange)
					return rangeValue.Equals(other.modelValue.CellRangeValue);
			}
			return modelValue.Equals(other.modelValue);
		}
		public override int GetHashCode() {
			return modelValue.GetHashCode() ^ formula.GetHashCode() ^ formulaInvariant.GetHashCode();
		}
		#endregion
		public static bool IsNullOrEmpty(ValueObject valueObject) {
			return valueObject == null || valueObject.IsEmpty;
		}
	}
}
