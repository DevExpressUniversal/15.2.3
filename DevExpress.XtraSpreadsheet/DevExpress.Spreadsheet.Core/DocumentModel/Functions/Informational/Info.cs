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

using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionInfo
	public class FunctionInfo : WorksheetFunctionBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "INFO"; } }
		public override int Code { get { return 0x00F4; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value; } }
		public override bool IsVolatile { get { return true; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue argument = arguments[0].ToText(context);
			if (argument.IsError)
				return argument;
			return GetResult(argument.GetTextValue(context.StringTable), context);
		}
		VariantValue GetResult(string argument, WorkbookDataContext context) {
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.DirectoryInfo, "directory", context))
				return GetDirectory();
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.NumFileInfo, "numfile", context))
				return GetNumberOfSheets(context);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.OriginInfo, "origin", context))
				return GetOrigin(context);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.OsVersionInfo, "osversion", context))
				return GetOSVersion();
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.RecalcInfo, "recalc", context))
				return GetRecalc(context);
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.ReleaseInfo, "release", context))
				return GetProductVersion();
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.SystemInfo, "system", context))
				return GetSystem();
			if (CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.MemAvailInfo, "memavail", context) ||
				CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.TotMemInfo, "totmem", context) ||
				CompareParamWithPattern(argument, XtraSpreadsheetFunctionNameStringId.MemUsedInfo, "memused", context))
				return GetNotAvailable();
			return VariantValue.ErrorInvalidValueInFunction;
		}
		bool CompareParamWithPattern(string argument, XtraSpreadsheetFunctionNameStringId stringId, string defaultValue, WorkbookDataContext context) {
			string paramPattern = FormulaCalculator.GetFunctionParameterName(stringId, defaultValue, context);
			if (StringExtensions.CompareInvariantCultureIgnoreCase(argument, paramPattern) == 0)
				return true;
			return StringExtensions.CompareInvariantCultureIgnoreCase(argument, defaultValue) == 0;
		}
		VariantValue GetNotAvailable() {
			return VariantValue.ErrorValueNotAvailable;
		}
		VariantValue GetRecalc(WorkbookDataContext context) {
			if (ModelCalculationMode.Manual == context.Workbook.Properties.CalculationOptions.CalculationMode)
				return FormulaCalculator.GetFunctionParameterName(XtraSpreadsheetFunctionNameStringId.ManualCalcMode, "Manual", context);
			return FormulaCalculator.GetFunctionParameterName(XtraSpreadsheetFunctionNameStringId.AutoCalcMode, "Automatic", context);
		}
		VariantValue GetOSVersion() {
#if DXPORTABLE
			return "Microsoft Windows NT 6.1";
#else
			return Environment.OSVersion.ToString();
#endif
		}
		VariantValue GetProductVersion() {
			return "14.0"; 
		}
		VariantValue GetSystem() {
#if DXPORTABLE
			return "pcdos";
#else
			if (Environment.OSVersion.Platform == PlatformID.MacOSX)
				return "mac";
			else
				return "pcdos";
#endif
		}
		VariantValue GetDirectory() {
#if DXPORTABLE
			return ".";
#else
			return Environment.CurrentDirectory;
#endif
		}
		private VariantValue GetNumberOfSheets(WorkbookDataContext context) {
			return context.Workbook.SheetCount;
		}
		VariantValue GetOrigin(WorkbookDataContext context) {
			CellPosition position = new CellPosition(CalculateOriginColumn(context), CalculateOriginRow(context), PositionType.Absolute, PositionType.Absolute);
			return "$A:" + position.ToString(context);
		}
		int CalculateOriginRow(WorkbookDataContext context) {
			Worksheet worksheet = context.CurrentWorksheet as Worksheet;
			if (worksheet == null)
				return 0;
			ModelWorksheetView view = worksheet.ActiveView;
			if (view.SplitState == ViewSplitState.Split)
				return 0;
			if (view.HorizontalSplitPosition != 0)
				return 0;
			else
				return view.TopLeftCell.Row;
		}
		int CalculateOriginColumn(WorkbookDataContext context) {
			Worksheet worksheet = context.CurrentWorksheet as Worksheet;
			if (worksheet == null)
				return 0;
			ModelWorksheetView view = worksheet.ActiveView;
			if (view.SplitState == ViewSplitState.Split)
				return 0;
			if (view.HorizontalSplitPosition != 0)
				return 0;
			else
				return view.TopLeftCell.Column;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value));
			return collection;
		}
	}
#endregion
}
