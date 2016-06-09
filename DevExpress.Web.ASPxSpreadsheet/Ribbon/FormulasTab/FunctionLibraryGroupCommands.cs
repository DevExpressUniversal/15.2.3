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

using System.Collections.Generic;
using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SRFunctionsAutoSumCommand :SRFormatAutoSumCommand {
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		} 
	}
	public class SRFunctionsFinancialCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsFinancialCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetFinancialBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsLogicalCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsLogicalCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetLogicalBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsTextCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsTextCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetTextBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsDateAndTimeCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsDateAndTimeCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetDateTimeBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsLookupAndReferenceCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsLookupAndReferenceCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetLookupBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsMathAndTrigonometryCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsMathAndTrigonometryCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetTrigonometryBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsMoreCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsMoreCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override RibbonItemSize DefaultItemSize {
			get {
				return RibbonItemSize.Large;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFunctionsStatisticalCommand());
			Items.Add(new SRFunctionsEngineeringCommand());
			Items.Add(new SRFunctionsInformationCommand());
			Items.Add(new SRFunctionsCompatibilityCommand());
		}
	}
	public class SRFunctionsStatisticalCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsStatisticalCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetStatisticalBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsEngineeringCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsEngineeringCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetEngineeringBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsCubeCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsCubeCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetCubeBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsInformationCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsInformationCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetInformationalBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
	public class SRFunctionsCompatibilityCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FunctionsCompatibilityCommandGroup;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			var baseMathFunctions = SpreadsheetRibbonHelper.GetCompatibilityBaseFunctions();
			foreach(KeyValuePair<string, ISpreadsheetFunction> mathFunction in baseMathFunctions) {
				Items.Add(new SRFunctionCommandBase(mathFunction.Value.Name));
			}
		}
	}
}
