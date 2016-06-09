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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Export.Xl {
	#region XlCellErrorType
	public enum XlCellErrorType {
		Null = 0x00,
		DivisionByZero = 0x07,
		Value = 0x0f,
		Reference = 0x17,
		Name = 0x1d,
		Number = 0x24,
		NotAvailable = 0x2a
	}
	#endregion
	#region IXlCellError
	public interface IXlCellError {
		XlCellErrorType Type { get; }
		string Name { get; }
		string Description { get; }
		XlVariantValue Value { get; }
	}
	#endregion
	#region InvalidValueInFunctionError
	class InvalidValueInFunctionError : IXlCellError {
		static InvalidValueInFunctionError instance = new InvalidValueInFunctionError();
		public static IXlCellError Instance { get { return instance; } }
		InvalidValueInFunctionError() {
		}
		#region ICellError Members
		public XlCellErrorType Type { get { return XlCellErrorType.Value; } }
		public string Name { get { return "#VALUE!"; } }
		public string Description { get { return "A value used in the formula is of the wrong data type"; } }
		public XlVariantValue Value { get { return XlVariantValue.ErrorInvalidValueInFunction; } }
		#endregion
	}
	#endregion
	#region DivisionByZeroError
	class DivisionByZeroError : IXlCellError {
		static DivisionByZeroError instance = new DivisionByZeroError();
		public static IXlCellError Instance { get { return instance; } }
		DivisionByZeroError() {
		}
		#region ICellError Members
		public XlCellErrorType Type { get { return XlCellErrorType.DivisionByZero; } }
		public string Name { get { return "#DIV/0!"; } }
		public string Description { get { return "Division by zero!"; } }
		public XlVariantValue Value { get { return XlVariantValue.ErrorDivisionByZero; } }
		#endregion
	}
	#endregion
	#region NumberError
	class NumberError : IXlCellError {
		static NumberError instance = new NumberError();
		public static IXlCellError Instance { get { return instance; } }
		NumberError() {
		}
		#region ICellError Members
		public XlCellErrorType Type { get { return XlCellErrorType.Number; } }
		public string Name { get { return "#NUM!"; } }
		public string Description { get { return "Invalid numeric values in a formula or function"; } }
		public XlVariantValue Value { get { return XlVariantValue.ErrorNumber; } }
		#endregion
	}
	#endregion
	#region NameError
	class NameError : IXlCellError {
		static NameError instance = new NameError();
		public static IXlCellError Instance { get { return instance; } }
		internal NameError() {
		}
		#region ICellError Members
		public XlCellErrorType Type { get { return XlCellErrorType.Name; } }
		public string Name { get { return "#NAME?"; } }
		public string Description { get { return "Function does not exist"; } }
		public XlVariantValue Value { get { return XlVariantValue.ErrorName; } }
		#endregion
	}
	#endregion
	#region ReferenceError
	class ReferenceError : IXlCellError {
		static ReferenceError instance = new ReferenceError();
		public static IXlCellError Instance { get { return instance; } }
		ReferenceError() {
		}
		#region ICellError Members
		public XlCellErrorType Type { get { return XlCellErrorType.Reference; } }
		public string Name { get { return "#REF!"; } }
		public string Description { get { return "Cell reference is not valid"; } }
		public XlVariantValue Value { get { return XlVariantValue.ErrorReference; } }
		#endregion
	}
	#endregion
	#region ValueNotAvailableError
	class ValueNotAvailableError : IXlCellError {
		static ValueNotAvailableError instance = new ValueNotAvailableError();
		public static IXlCellError Instance { get { return instance; } }
		ValueNotAvailableError() {
		}
		#region ICellError Members
		public XlCellErrorType Type { get { return XlCellErrorType.NotAvailable; } }
		public string Name { get { return "#N/A"; } }
		public string Description { get { return "Value is not available to a function or formula"; } }
		public XlVariantValue Value { get { return XlVariantValue.ErrorValueNotAvailable; } }
		#endregion
	}
	#endregion
	#region NullIntersectionError
	class NullIntersectionError : IXlCellError {
		static NullIntersectionError instance = new NullIntersectionError();
		public static IXlCellError Instance { get { return instance; } }
		NullIntersectionError() {
		}
		#region ICellError Members
		public XlCellErrorType Type { get { return XlCellErrorType.Null; } }
		public string Name { get { return "#NULL!"; } }
		public string Description { get { return "The specified intersection includes two ranges that do not intersect."; } }
		public XlVariantValue Value { get { return XlVariantValue.ErrorNullIntersection; } }
		#endregion
	}
	#endregion
	#region CellErrorFactory
	public static class XlCellErrorFactory {
		#region Static
		static readonly Dictionary<XlCellErrorType, XlVariantValue> errorConversionTable = CreateErrorConversionTable();
		static Dictionary<XlCellErrorType, XlVariantValue> CreateErrorConversionTable() {
			Dictionary<XlCellErrorType, XlVariantValue> result = new Dictionary<XlCellErrorType, XlVariantValue>();
			result.Add(XlCellErrorType.DivisionByZero, XlVariantValue.ErrorDivisionByZero);
			result.Add(XlCellErrorType.Name, XlVariantValue.ErrorName);
			result.Add(XlCellErrorType.NotAvailable, XlVariantValue.ErrorValueNotAvailable);
			result.Add(XlCellErrorType.Null, XlVariantValue.ErrorNullIntersection);
			result.Add(XlCellErrorType.Number, XlVariantValue.ErrorNumber);
			result.Add(XlCellErrorType.Reference, XlVariantValue.ErrorReference);
			result.Add(XlCellErrorType.Value, XlVariantValue.ErrorInvalidValueInFunction);
			return result;
		}
		static readonly Dictionary<string, IXlCellError> errorByInvariantNameTable = CreateErrorByInvariantNameTable();
		static Dictionary<string, IXlCellError> CreateErrorByInvariantNameTable() {
			Dictionary<string, IXlCellError> result = new Dictionary<string, IXlCellError>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			result.Add("#NULL!", NullIntersectionError.Instance);
			result.Add("#DIV/0!", DivisionByZeroError.Instance);
			result.Add("#VALUE!", InvalidValueInFunctionError.Instance);
			result.Add("#REF!", ReferenceError.Instance);
			result.Add("#NAME?", NameError.Instance);
			result.Add("#NUM!", NumberError.Instance);
			result.Add("#N/A", ValueNotAvailableError.Instance);
			return result;
		}
		#endregion
		public static XlVariantValue CreateError(XlCellErrorType errorType) {
			return errorConversionTable[errorType];
		}
		public static bool TryCreateErrorByInvariantName(string name, out IXlCellError error) {
			if (string.IsNullOrEmpty(name)) {
				error = null;
				return false;
			}
			return errorByInvariantNameTable.TryGetValue(name, out error);
		}
	}
	#endregion
}
