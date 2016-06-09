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
using DevExpress.XtraSpreadsheet.Forms;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region PivotTableShowValuesAsWithBaseFieldCommandBase
	public abstract class PivotTableShowValuesAsWithBaseFieldCommandBase : PivotTableShowValuesAsCommandBase {
		protected PivotTableShowValuesAsWithBaseFieldCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		public override string MenuCaption { get { return base.MenuCaption + "..."; } }
		public override string Description { get { return base.Description + "..."; } }
		protected virtual bool BaseItemEnabled { get { return false; } }
		protected internal override void ExecuteCore() {
			Control.ShowPivotTableShowValuesAsForm(CreateViewModel());
		}
		protected PivotTableShowValuesAsViewModel CreateViewModel() {
			PivotTableShowValuesAsViewModel viewModel = new PivotTableShowValuesAsViewModel(Control);
			viewModel.DataFieldName = DataField.Name;
			viewModel.CalculationType = GetCalcualtionType();
			viewModel.Command = this;
			viewModel.BaseItemEnabled = BaseItemEnabled;
			if (IsChecked())
				SetupViewModelFromDataField(viewModel);
			else
				SetupViewModel(viewModel);
			return viewModel;
		}
		string GetCalcualtionType() {
			return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Caption_ShowValuesAsCalculation) + " " + XtraSpreadsheetLocalizer.GetString(MenuCaptionStringId);
		}
		protected virtual void SetupViewModelFromDataField(PivotTableShowValuesAsViewModel viewModel) {
			viewModel.BaseFieldIndex = DataField.BaseField;
		}
		protected virtual void SetupViewModel(PivotTableShowValuesAsViewModel viewModel) {
		}
		public void ApplyChanges(PivotTableShowValuesAsViewModel viewModel) {
			PivotTable.BeginTransaction(Control.InnerControl.ErrorHandler);
			try {
				ApplyChangesCore(viewModel);
			}
			finally {
				PivotTable.EndTransaction();
			}
		}
		protected virtual void ApplyChangesCore(PivotTableShowValuesAsViewModel viewModel) {
			DataField.NumberFormatIndex = GetNumberFormatId();
			DataField.ShowDataAs = ShowDataAs;
			DataField.BaseField = viewModel.BaseFieldIndex;
		}
		protected override bool IsEnabled() {
			return PivotTable.RowFields.KeyIndicesCount > 0 || PivotTable.ColumnFields.KeyIndicesCount > 0;
		}
	}
	#endregion
	#region PivotTableShowValuesAsWithBaseFieldAndItemCommandBase
	public abstract class PivotTableShowValuesAsWithBaseFieldAndItemCommandBase : PivotTableShowValuesAsWithBaseFieldCommandBase {
		protected PivotTableShowValuesAsWithBaseFieldAndItemCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override bool BaseItemEnabled { get { return true; } }
		protected override void SetupViewModelFromDataField(PivotTableShowValuesAsViewModel viewModel) {
			base.SetupViewModelFromDataField(viewModel);
			viewModel.PopulateBaseItemNames();
			int baseItem = DataField.BaseItem;
			viewModel.BaseItemIndex = baseItem > PivotTableLayoutCalculator.NextItem ? 0 : baseItem;
		}
		protected override void SetupViewModel(PivotTableShowValuesAsViewModel viewModel) {
			viewModel.PopulateBaseItemNames();
			viewModel.BaseItemIndex = 0;
		}
		protected override void ApplyChangesCore(PivotTableShowValuesAsViewModel viewModel) {
			base.ApplyChangesCore(viewModel);
			DataField.BaseItem = viewModel.BaseItemIndex;
		}
	}
	#endregion
	#region PivotTableShowValuesAsPercentOfParentCommand
	public class PivotTableShowValuesAsPercentOfParentCommand : PivotTableShowValuesAsWithBaseFieldCommandBase {
		public PivotTableShowValuesAsPercentOfParentCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsPercentOfParent; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParent; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfParent; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.PercentOfParent; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsRunningTotalCommand
	public class PivotTableShowValuesAsRunningTotalCommand : PivotTableShowValuesAsWithBaseFieldCommandBase {
		public PivotTableShowValuesAsRunningTotalCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsRunningTotal; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRunningTotal; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRunningTotal; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.RunningTotal; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsPercentOfRunningTotalCommand
	public class PivotTableShowValuesAsPercentOfRunningTotalCommand : PivotTableShowValuesAsWithBaseFieldCommandBase {
		public PivotTableShowValuesAsPercentOfRunningTotalCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsPercentOfRunningTotal; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfRunningTotal; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentOfRunningTotal; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.PercentOfRunningTotal; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsRankAscendingCommand
	public class PivotTableShowValuesAsRankAscendingCommand : PivotTableShowValuesAsWithBaseFieldCommandBase {
		public PivotTableShowValuesAsRankAscendingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsRankAscending; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRankAscending; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRankAscending; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.RankAscending; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsRankDescendingCommand
	public class PivotTableShowValuesAsRankDescendingCommand : PivotTableShowValuesAsWithBaseFieldCommandBase {
		public PivotTableShowValuesAsRankDescendingCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsRankDescending; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRankDescending; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueRankDescending; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.RankDescending; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsPercentCommand
	public class PivotTableShowValuesAsPercentCommand : PivotTableShowValuesAsWithBaseFieldAndItemCommandBase {
		public PivotTableShowValuesAsPercentCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsPercent; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercent; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercent; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.Percent; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsDifferenceCommand
	public class PivotTableShowValuesAsDifferenceCommand : PivotTableShowValuesAsWithBaseFieldAndItemCommandBase {
		public PivotTableShowValuesAsDifferenceCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsDifference; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueDifference; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValueDifference; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.Difference; } }
		#endregion
	}
	#endregion
	#region PivotTableShowValuesAsPercentDifferenceCommand
	public class PivotTableShowValuesAsPercentDifferenceCommand : PivotTableShowValuesAsWithBaseFieldAndItemCommandBase {
		public PivotTableShowValuesAsPercentDifferenceCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableShowValuesAsPercentDifference; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentDifference; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.DataFieldSettingsPivotTableForm_ShowValuePercentDifference; } }
		protected override PivotShowDataAs ShowDataAs { get { return PivotShowDataAs.PercentDifference; } }
		#endregion
	}
	#endregion
}
