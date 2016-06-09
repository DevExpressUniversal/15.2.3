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
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableCustomDateFilterCommandBase
	public abstract class PivotTableCustomDateFilterCommandBase : PivotTableCustomFiltersCommandBase<PivotTableDateFiltersViewModel> {
		protected PivotTableCustomDateFilterCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override void ShowForm(PivotTableDateFiltersViewModel viewModel) {
			Control.ShowPivotTableDateFilterForm(viewModel);
		}
		protected override PivotTableDateFiltersViewModel CreateViewModelCore() {
			return new PivotTableDateFiltersViewModel(Control);
		}
		protected override void SetupViewModel(PivotFilter filter, PivotTableDateFiltersViewModel viewModel) {
			viewModel.FirstDate = ToDateTime(filter.AutoFilter.FilterColumns[0].CustomFilters[0].NumericValue.NumericValue);
		}
		protected DateTime ToDateTime(double serialNumber) {
			return WorkbookDataContext.FromDateTimeSerial(serialNumber, Context.DateSystem);
		}
	}
	#endregion
	#region PivotTableCustomDateFilterCommand
	public class PivotTableCustomDateFilterCommand : PivotTableCustomDateFilterCommandBase {
		public PivotTableCustomDateFilterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterCustom; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateCustom; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateCustomDescription; } }
		protected override PivotFilterType FilterType { get { return GetFilterType(); } }
		#endregion
		protected override void SetupViewModel(PivotFilter filter, PivotTableDateFiltersViewModel viewModel) {
			base.SetupViewModel(filter, viewModel);
			if (FilterType == PivotFilterType.DateBetween || FilterType == PivotFilterType.DateNotBetween)
				viewModel.SecondDate = ToDateTime(filter.AutoFilter.FilterColumns[0].CustomFilters[1].NumericValue.NumericValue);
		}
		PivotFilterType GetFilterType() {
			PivotFilter filter = GetFilter();
			if (filter != null && IsCustomFilterType(filter.FilterType))
				return filter.FilterType;
			return PivotFilterType.DateEqual;
		}
		bool IsCustomFilterType(PivotFilterType filterType) {
			return filterType == PivotFilterType.DateEqual || filterType == PivotFilterType.DateOlderThan ||
				   filterType == PivotFilterType.DateNewerThan || filterType == PivotFilterType.DateBetween ||
				   IsCheckedCore(filterType);
		}
		protected override bool IsCheckedCore(PivotFilterType filterType) {
			return filterType == PivotFilterType.DateNotEqual || filterType == PivotFilterType.DateOlderThanOrEqual ||
				   filterType == PivotFilterType.DateNewerThanOrEqual || filterType == PivotFilterType.DateNotBetween;
		}
	}
	#endregion
	#region PivotTableDateFilterEqualsCommand
	public class PivotTableDateFilterEqualsCommand : PivotTableCustomDateFilterCommandBase {
		public PivotTableDateFilterEqualsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterEquals; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateEquals; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateEqualsDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.DateEqual; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterBeforeCommand
	public class PivotTableDateFilterBeforeCommand : PivotTableCustomDateFilterCommandBase {
		public PivotTableDateFilterBeforeCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterBefore; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateBefore; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateBeforeDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.DateOlderThan; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterAfterCommand
	public class PivotTableDateFilterAfterCommand : PivotTableCustomDateFilterCommandBase {
		public PivotTableDateFilterAfterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterAfter; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateAfter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateAfterDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.DateNewerThan; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterBetweenCommand
	public class PivotTableDateFilterBetweenCommand : PivotTableCustomDateFilterCommandBase {
		public PivotTableDateFilterBetweenCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterBetween; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateBetween; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDateBetweenDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.DateBetween; } }
		#endregion
		protected override void SetupViewModel(PivotFilter filter, PivotTableDateFiltersViewModel viewModel) {
			base.SetupViewModel(filter, viewModel);
			viewModel.SecondDate = ToDateTime(filter.AutoFilter.FilterColumns[0].CustomFilters[1].NumericValue.NumericValue);
		}
	}
	#endregion
}
