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
using System.Globalization;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public enum ModelCellErrorType {
		NotAvailable = 0,
		Number = 1,
		Name = 2,
		Reference = 3,
		Value = 4,
		DivisionByZero = 5,
		Null = 6,
		GettingData = 7
	}
	#region ICellError
	public interface ICellError {
		ModelCellErrorType Type { get; }
		string Name { get; }
		string Description { get; }
		VariantValue Value { get; }
	}
	#endregion
	public static class CellErrorFactory {
		readonly static ICellError[] errors = new ICellError[] {
			ValueNotAvailableError.Instance,
			NumberError.Instance,
			NameError.Instance,
			ReferenceError.Instance,
			InvalidValueInFunctionError.Instance,
			DivisionByZeroError.Instance,
			NullIntersectionError.Instance,
			GettingDataError.Instance,
		};
		public static ICellError[] Errors { get { return errors; } }
		[ThreadStatic]
		static CultureInfo lastCulture;
		static readonly Dictionary<string, ICellError> errorByInvariantNameTable = CreateErrorByInvariantNameTable();
		[ThreadStatic]
		static Dictionary<string, ICellError> errorByNameTable;
		[ThreadStatic]
		static Dictionary<ICellError, string> errorNamesTable;
		internal static Dictionary<string, ICellError> ErrorByNameTable { get { return errorByNameTable; } set { errorByNameTable = value; } }
		internal static Dictionary<ICellError, string> ErrorNamesTable { get { return errorNamesTable; } set { errorNamesTable = value; } }
		internal static CultureInfo LastCulture { get { return lastCulture; } set { lastCulture = value; } }
		public static bool TryCreateErrorByInvariantName(string name, out ICellError error) {
			if (string.IsNullOrEmpty(name)) {
				error = null;
				return false;
			}
			return errorByInvariantNameTable.TryGetValue(name, out error);
		}
		public static ICellError CreateError(string name, WorkbookDataContext context) {
			ICellError error;
			if (context.ImportExportMode || context.Culture == CultureInfo.InvariantCulture) {
				if (errorByInvariantNameTable.TryGetValue(name, out error))
					return error;
				return null;
			}
			PrepareTablesForCulture(context.Culture);
			switch (context.Workbook.BehaviorOptions.FunctionNameCulture) {
				default:
				case FunctionNameCulture.English:
					if (errorByInvariantNameTable.TryGetValue(name, out error))
						return error;
					break;
				case FunctionNameCulture.Local:
					if (GetLocalizedErrorNameTable(context).TryGetValue(name, out error))
						return error;
					break;
				case FunctionNameCulture.Auto:
					if (GetLocalizedErrorNameTable(context).TryGetValue(name, out error))
						return error;
					if (errorByInvariantNameTable.TryGetValue(name, out error))
						return error;
					break;
			}
			return null;
		}
		public static ICellError CreateError(string name) {
			ICellError error;
			errorByInvariantNameTable.TryGetValue(name, out error);
			return error;
		}
		static Dictionary<string, ICellError> GetLocalizedErrorNameTable(WorkbookDataContext context) {
			CultureInfo culture = context.Culture;
			if (context.Culture == CultureInfo.InvariantCulture)
				return errorByInvariantNameTable;
			PrepareTablesForCulture(culture);
			return errorByNameTable;
		}
		public static string GetErrorName(ICellError error, WorkbookDataContext context) {
			if (context.ImportExportMode)
				return error.Name;
			switch (context.Workbook.BehaviorOptions.FunctionNameCulture) {
				default:
				case FunctionNameCulture.English:
					return error.Name;
				case FunctionNameCulture.Local: 
				case FunctionNameCulture.Auto:
					PrepareTablesForCulture(context.Culture);
					string result;
					if (errorNamesTable.TryGetValue(error, out result))
						return result;
					return error.Name;
			}
		}
		static void PrepareTablesForCulture(CultureInfo culture) {
			if (lastCulture != culture) {
				errorByNameTable = null;
				errorNamesTable = null;
				lastCulture = culture;
			}
			if (errorByNameTable != null)
				return;
			errorByNameTable = CreateErrorByNameTable(culture);
			errorNamesTable = CreateErrorNamesTable(culture);
		}
		static Dictionary<string, ICellError> CreateErrorByInvariantNameTable() {
			Dictionary<string, ICellError> result = new Dictionary<string, ICellError>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			result.Add("#NULL!", NullIntersectionError.Instance);
			result.Add("#DIV/0!", DivisionByZeroError.Instance);
			result.Add("#VALUE!", InvalidValueInFunctionError.Instance);
			result.Add("#REF!", ReferenceError.Instance);
			result.Add("#NAME?", NameError.Instance);
			result.Add("#NUM!", NumberError.Instance);
			result.Add("#N/A", ValueNotAvailableError.Instance);
			result.Add("#GETTING_DATA", GettingDataError.Instance);
			return result;
		}
		static Dictionary<string, ICellError> CreateErrorByNameTable(CultureInfo culture) {
			Dictionary<string, ICellError> result = new Dictionary<string, ICellError>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			foreach (KeyValuePair<string, ICellError> pair in errorByInvariantNameTable) {
				string name = XtraSpreadsheetCellErrorNameResLocalizer.GetString((XtraSpreadsheetCellErrorNameStringId)pair.Value.Type, culture);
				if (!String.IsNullOrEmpty(name))
					result.Add(name, pair.Value);
			}
			return result;
		}
		static Dictionary<ICellError, string> CreateErrorNamesTable(CultureInfo culture) {
			if (errorByNameTable == null)
				CreateErrorByNameTable(culture);
			Dictionary<ICellError, string> result = new Dictionary<ICellError, string>();
			foreach (KeyValuePair<string, ICellError> pair in errorByNameTable)
				result.Add(pair.Value, pair.Key);
			return result;
		}
	}
	#region InvalidValueInFunctionError
	class InvalidValueInFunctionError : ICellError {
		static InvalidValueInFunctionError instance = new InvalidValueInFunctionError();
		public static ICellError Instance { get { return instance; } }
		InvalidValueInFunctionError() {
		}
		#region ICellError Members
		public ModelCellErrorType Type { get { return ModelCellErrorType.Value; } }
		public string Name { get { return "#VALUE!"; } }
		public string Description { get { return "A value used in the formula is of the wrong data type"; } }
		public VariantValue Value { get { return VariantValue.ErrorInvalidValueInFunction; } }
		#endregion
	}
	#endregion
	#region DivisionByZeroError
	class DivisionByZeroError : ICellError {
		static DivisionByZeroError instance = new DivisionByZeroError();
		public static ICellError Instance { get { return instance; } }
		DivisionByZeroError() {
		}
		#region ICellError Members
		public ModelCellErrorType Type { get { return ModelCellErrorType.DivisionByZero; } }
		public string Name { get { return "#DIV/0!"; } }
		public string Description { get { return "Division by zero!"; } }
		public VariantValue Value { get { return VariantValue.ErrorDivisionByZero; } }
		#endregion
	}
	#endregion
	#region NumberError
	class NumberError : ICellError {
		static NumberError instance = new NumberError();
		public static ICellError Instance { get { return instance; } }
		NumberError() {
		}
		#region ICellError Members
		public ModelCellErrorType Type { get { return ModelCellErrorType.Number; } }
		public string Name { get { return "#NUM!"; } }
		public string Description { get { return "Invalid numeric values in a formula or function"; } }
		public VariantValue Value { get { return VariantValue.ErrorNumber; } }
		#endregion
	}
	#endregion
	#region NameError
	class NameError : ICellError {
		static NameError instance = new NameError();
		public static ICellError Instance { get { return instance; } }
		internal NameError() {
		}
		#region ICellError Members
		public ModelCellErrorType Type { get { return ModelCellErrorType.Name; } }
		public string Name { get { return "#NAME?"; } }
		public string Description { get { return "Function does not exist"; } }
		public VariantValue Value { get { return VariantValue.ErrorName; } }
		#endregion
	}
	#endregion
	#region ReferenceError
	class ReferenceError : ICellError {
		static ReferenceError instance = new ReferenceError();
		public static ICellError Instance { get { return instance; } }
		ReferenceError() {
		}
		#region ICellError Members
		public ModelCellErrorType Type { get { return ModelCellErrorType.Reference; } }
		public string Name { get { return "#REF!"; } }
		public string Description { get { return "Cell reference is not valid"; } }
		public VariantValue Value { get { return VariantValue.ErrorReference; } }
		#endregion
	}
	#endregion
	#region ValueNotAvailableError
	class ValueNotAvailableError : ICellError {
		static ValueNotAvailableError instance = new ValueNotAvailableError();
		public static ICellError Instance { get { return instance; } }
		ValueNotAvailableError() {
		}
		#region ICellError Members
		public ModelCellErrorType Type { get { return ModelCellErrorType.NotAvailable; } }
		public string Name { get { return "#N/A"; } }
		public string Description { get { return "Value is not available to a function or formula"; } }
		public VariantValue Value { get { return VariantValue.ErrorValueNotAvailable; } }
		#endregion
	}
	#endregion
	#region NullIntersectionError
	class NullIntersectionError : ICellError {
		static NullIntersectionError instance = new NullIntersectionError();
		public static ICellError Instance { get { return instance; } }
		NullIntersectionError() {
		}
		#region ICellError Members
		public ModelCellErrorType Type { get { return ModelCellErrorType.Null; } }
		public string Name { get { return "#NULL!"; } }
		public string Description { get { return "The specified intersection includes two ranges that do not intersect."; } }
		public VariantValue Value { get { return VariantValue.ErrorNullIntersection; } }
		#endregion
	}
	#endregion
	#region GettingDataError
	class GettingDataError : ICellError {
		static GettingDataError instance = new GettingDataError();
		public static ICellError Instance { get { return instance; } }
		GettingDataError() {
		}
		#region ICellError Members
		public ModelCellErrorType Type { get { return ModelCellErrorType.GettingData; } }
		public string Name { get { return "#GETTING_DATA"; } }
		public string Description { get { return "Requested value is not yet ready."; } }
		public VariantValue Value { get { return VariantValue.ErrorGettingData; } }
		#endregion
	}
	#endregion
}
