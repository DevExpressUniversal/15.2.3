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
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableCustomValueFilterCommandBase
	public abstract class PivotTableCustomValueFilterCommandBase : PivotTableCustomFiltersCommandBase<PivotTableValueFiltersViewModel> {
		protected PivotTableCustomValueFilterCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			if (PivotTable.DataFields.Count > 0)
				base.ExecuteCore();
			else
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_PivotValueFilterNeedAtLeastOneDataField));
		}
		protected override void ShowForm(PivotTableValueFiltersViewModel viewModel) {
			Control.ShowPivotTableValueFilterForm(viewModel);
		}
		protected override PivotTableValueFiltersViewModel CreateViewModelCore() {
			return new PivotTableValueFiltersViewModel(Control);
		}
		protected override void SetupViewModel(PivotFilter filter, PivotTableValueFiltersViewModel viewModel) {
			viewModel.MeasureFieldIndex = filter.MeasureFieldIndex.Value;
			viewModel.FirstStringValue = filter.AutoFilter.FilterColumns[0].CustomFilters[0].NumericValue.NumericValue.ToString(Culture);
		}
		protected override void ApplyChangesCore(PivotAddFilterCommand command, PivotTableValueFiltersViewModel viewModel) {
			command.MeasureFieldIndex = viewModel.MeasureFieldIndex;
		}
		protected override PivotFilter GetFilter() {
			return Info.MeasureFilter;
		}
	}
	#endregion
	#region PivotTableValueFilterEqualsCommand
	public class PivotTableValueFilterEqualsCommand : PivotTableCustomValueFilterCommandBase {
		public PivotTableValueFilterEqualsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableValueFilterEquals; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterEquals; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterEqualsDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ValueEqual; } }
		#endregion
	}
	#endregion
	#region PivotTableValueFilterDoesNotEqualCommand
	public class PivotTableValueFilterDoesNotEqualCommand : PivotTableCustomValueFilterCommandBase {
		public PivotTableValueFilterDoesNotEqualCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableValueFilterDoesNotEqual; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotEqual; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotEqualDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ValueNotEqual; } }
		#endregion
	}
	#endregion
	#region PivotTableValueFilterGreaterThanCommand
	public class PivotTableValueFilterGreaterThanCommand : PivotTableCustomValueFilterCommandBase {
		public PivotTableValueFilterGreaterThanCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableValueFilterGreaterThan; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThan; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThanDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ValueGreaterThan; } }
		#endregion
	}
	#endregion
	#region PivotTableValueFilterGreaterThanOrEqualCommand
	public class PivotTableValueFilterGreaterThanOrEqualCommand : PivotTableCustomValueFilterCommandBase {
		public PivotTableValueFilterGreaterThanOrEqualCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableValueFilterGreaterThanOrEqual; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThanOrEqualTo; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThanOrEqualToDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ValueGreaterThanOrEqual; } }
		#endregion
	}
	#endregion
	#region PivotTableValueFilterLessThanCommand
	public class PivotTableValueFilterLessThanCommand : PivotTableCustomValueFilterCommandBase {
		public PivotTableValueFilterLessThanCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableValueFilterLessThan; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLessThan; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLessThanDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ValueLessThan; } }
		#endregion
	}
	#endregion
	#region PivotTableValueFilterLessThanOrEqualCommand
	public class PivotTableValueFilterLessThanOrEqualCommand : PivotTableCustomValueFilterCommandBase {
		public PivotTableValueFilterLessThanOrEqualCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableValueFilterLessThanOrEqual; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLessThanOrEqualTo; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLessThanOrEqualToDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ValueLessThanOrEqual; } }
		#endregion
	}
	#endregion
	#region PivotTableValueFilterBetweenCommand
	public class PivotTableValueFilterBetweenCommand : PivotTableCustomValueFilterCommandBase {
		public PivotTableValueFilterBetweenCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableValueFilterBetween; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterBetween; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterBetweenDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ValueBetween; } }
		#endregion
		protected override void SetupViewModel(PivotFilter filter, PivotTableValueFiltersViewModel viewModel) {
			base.SetupViewModel(filter, viewModel);
			viewModel.SecondStringValue = filter.AutoFilter.FilterColumns[0].CustomFilters[1].NumericValue.NumericValue.ToString(Culture);
		}
	}
	#endregion
	#region PivotTableValueFilterNotBetweenCommand
	public class PivotTableValueFilterNotBetweenCommand : PivotTableValueFilterBetweenCommand {
		public PivotTableValueFilterNotBetweenCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableValueFilterNotBetween; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterNotBetween; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterNotBetweenDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ValueNotBetween; } }
		#endregion
	}
	#endregion
	#region PivotTableValueFilterTop10Command
	public class PivotTableValueFilterTop10Command : PivotTableCustomFiltersCommandBase<PivotTableTop10FiltersViewModel> {
		public PivotTableValueFilterTop10Command(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableValueFilterTop10; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterTop10; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterTop10Description; } }
		protected override PivotFilterType FilterType { get { return GetTop10Type(); } }
		#endregion
		protected internal override void ExecuteCore() {
			if (PivotTable.DataFields.Count > 0)
				base.ExecuteCore();
			else
				Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_PivotValueFilterNeedAtLeastOneDataField));
		}
		protected override void ShowForm(PivotTableTop10FiltersViewModel viewModel) {
			Control.ShowPivotTableTop10FilterForm(viewModel);
		}
		protected override PivotTableTop10FiltersViewModel CreateViewModelCore() {
			return new PivotTableTop10FiltersViewModel(Control);
		}
		PivotFilterType GetTop10Type() {
			PivotFilter filter = GetFilter();
			if (filter != null && filter.IsTop10Filter)
				return filter.FilterType;
			return PivotFilterType.Count;
		}
		protected override void SetupViewModel(PivotFilter filter, PivotTableTop10FiltersViewModel viewModel) {
			viewModel.MeasureFieldIndex = filter.MeasureFieldIndex.Value;
			AutoFilterColumn filterColumn = filter.AutoFilter.FilterColumns[0];
			viewModel.FilterByTop = filterColumn.FilterByTopOrder;
			viewModel.Value = filterColumn.TopOrBottomDoubleValue;
		}
		protected override void ApplyChangesCore(PivotAddFilterCommand command, PivotTableTop10FiltersViewModel viewModel) {
			command.MeasureFieldIndex = viewModel.MeasureFieldIndex;
			command.FilterByTop = viewModel.FilterByTop;
		}
		protected override PivotFilter GetFilter() {
			return Info.MeasureFilter;
		}
	}
	#endregion
}
