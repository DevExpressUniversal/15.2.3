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
	#region PivotTableShowValuesAsCommandGroup
	public class PivotTableShowValuesAsCommandGroup : PivotTableSummarizeValuesByCommandGroup {
		public PivotTableShowValuesAsCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsCommandGroup; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableShowValuesAs; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotTableShowValuesAs; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsCommandBase
	public abstract class PivotTableShowValuesAsCommandBase : PivotTablePopupMenuCommandBase {
		const int normalFormatId = 0;
		const int percentFormatId = 10;
		protected PivotTableShowValuesAsCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract PivotShowDataAs ShowDataAs { get; }
		protected internal override void ExecuteCore() {
			PivotTable.BeginTransaction(Control.InnerControl.ErrorHandler);
			try {
				DataField.NumberFormatIndex = GetNumberFormatId();
				DataField.ShowDataAs = ShowDataAs;
			}
			finally {
				PivotTable.EndTransaction();
			}
		}
		protected override bool IsChecked() {
			if (Axis != PivotTableAxis.Value)
				return false;
			return DataField.ShowDataAs == ShowDataAs;
		}
		protected int GetNumberFormatId() {
			if (ShowDataAs == PivotShowDataAs.Normal || ShowDataAs == PivotShowDataAs.Difference ||
				ShowDataAs == PivotShowDataAs.RunningTotal || ShowDataAs == PivotShowDataAs.RankAscending ||
				ShowDataAs == PivotShowDataAs.RankDescending || ShowDataAs == PivotShowDataAs.Index)
				return normalFormatId;
			return percentFormatId;
		}
	}
	#endregion
	#region PivotTableShowValuesAsNormalCommand
	public class PivotTableShowValuesAsNormalCommand : PivotTableShowValuesAsCommandBase {
		public PivotTableShowValuesAsNormalCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsNormal; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueAsNoCalculation; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueAsNoCalculation; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.Normal; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsPercentOfTotalCommand
	public class PivotTableShowValuesAsPercentOfTotalCommand : PivotTableShowValuesAsCommandBase {
		public PivotTableShowValuesAsPercentOfTotalCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsPercentOfTotal; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfTotal; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfTotal; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.PercentOfTotal; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsPercentOfColumnCommand
	public class PivotTableShowValuesAsPercentOfColumnCommand : PivotTableShowValuesAsCommandBase {
		public PivotTableShowValuesAsPercentOfColumnCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsPercentOfColumn; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfColumn; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfColumn; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.PercentOfColumn; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsPercentOfParentColumnCommand
	public class PivotTableShowValuesAsPercentOfParentColumnCommand : PivotTableShowValuesAsCommandBase {
		public PivotTableShowValuesAsPercentOfParentColumnCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsPercentOfParentColumn; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParentColumn; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParentColumn; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.PercentOfParentColumn; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsPercentOfRowCommand
	public class PivotTableShowValuesAsPercentOfRowCommand : PivotTableShowValuesAsCommandBase {
		public PivotTableShowValuesAsPercentOfRowCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsPercentOfRow; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfRow; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfRow; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.PercentOfRow; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsPercentOfParentRowCommand
	public class PivotTableShowValuesAsPercentOfParentRowCommand : PivotTableShowValuesAsCommandBase {
		public PivotTableShowValuesAsPercentOfParentRowCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsPercentOfParentRow; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParentRow; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParentRow; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.PercentOfParentRow; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsIndexCommand
	public class PivotTableShowValuesAsIndexCommand : PivotTableShowValuesAsCommandBase {
		public PivotTableShowValuesAsIndexCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsIndex; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueIndex; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueIndex; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.Index; } }
		#endregion
	}
	#endregion
}
