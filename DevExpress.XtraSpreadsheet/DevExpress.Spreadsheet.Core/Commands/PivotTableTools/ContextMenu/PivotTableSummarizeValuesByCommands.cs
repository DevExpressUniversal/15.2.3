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
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PivotTableSummarizeValuesByCommandGroup
	public class PivotTableSummarizeValuesByCommandGroup : PivotTablePopupMenuCommandGroupBase {
		public PivotTableSummarizeValuesByCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableSummarizeValuesByCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableSummarizeValuesBy; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableSummarizeValuesBy; } }
		#endregion
		protected override bool IsVisible() {
			return Axis == PivotTableAxis.Value;
		}
	}
	#endregion
	#region PivotTableSummarizeValuesByCommandBase
	public abstract class PivotTableSummarizeValuesByCommandBase : PivotTablePopupMenuCommandBase {
		protected PivotTableSummarizeValuesByCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract PivotDataConsolidateFunction Subtotal { get; }
		protected internal override void ExecuteCore() {
			DataField.SetSubtotal(Subtotal, Control.InnerControl.ErrorHandler);
		}
		protected override bool IsChecked() {
			if (Axis != PivotTableAxis.Value)
				return false;
			return DataField.Subtotal == Subtotal;
		}
	}
	#endregion
	#region PivotTableSummarizeValuesBySumCommand
	public class PivotTableSummarizeValuesBySumCommand  : PivotTableSummarizeValuesByCommandBase {
		public PivotTableSummarizeValuesBySumCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableSummarizeValuesBySum; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionSum; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionSum; } }
		protected override PivotDataConsolidateFunction Subtotal { get { return PivotDataConsolidateFunction.Sum; } }
		#endregion
	}
	#endregion
	#region PivotTableSummarizeValuesByCountCommand
	public class PivotTableSummarizeValuesByCountCommand : PivotTableSummarizeValuesByCommandBase {
		public PivotTableSummarizeValuesByCountCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableSummarizeValuesByCount; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionCount; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionCount; } }
		protected override PivotDataConsolidateFunction Subtotal { get { return PivotDataConsolidateFunction.Count; } }
		#endregion
	}
	#endregion
	#region PivotTableSummarizeValuesByMaxCommand
	public class PivotTableSummarizeValuesByMaxCommand : PivotTableSummarizeValuesByCommandBase {
		public PivotTableSummarizeValuesByMaxCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableSummarizeValuesByMax; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMax; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMax; } }
		protected override PivotDataConsolidateFunction Subtotal { get { return PivotDataConsolidateFunction.Max; } }
		#endregion
	}
	#endregion
	#region PivotTableSummarizeValuesByMinCommand
	public class PivotTableSummarizeValuesByMinCommand : PivotTableSummarizeValuesByCommandBase {
		public PivotTableSummarizeValuesByMinCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableSummarizeValuesByMin; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMin; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionMin; } }
		protected override PivotDataConsolidateFunction Subtotal { get { return PivotDataConsolidateFunction.Min; } }
		#endregion
	}
	#endregion
	#region PivotTableSummarizeValuesByAverageCommand
	public class PivotTableSummarizeValuesByAverageCommand : PivotTableSummarizeValuesByCommandBase {
		public PivotTableSummarizeValuesByAverageCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableSummarizeValuesByAverage; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionAverage; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionAverage; } }
		protected override PivotDataConsolidateFunction Subtotal { get { return PivotDataConsolidateFunction.Average; } }
		#endregion
	}
	#endregion
	#region PivotTableSummarizeValuesByProductCommand
	public class PivotTableSummarizeValuesByProductCommand : PivotTableSummarizeValuesByCommandBase {
		public PivotTableSummarizeValuesByProductCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableSummarizeValuesByProduct; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionProduct; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.FieldAndDataFieldSettingsPivotTableForm_FunctionProduct; } }
		protected override PivotDataConsolidateFunction Subtotal { get { return PivotDataConsolidateFunction.Product; } }
		#endregion
	}
	#endregion
}
