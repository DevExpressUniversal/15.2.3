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
	#region PivotTableCustomLabelFilterCommandBase
	public abstract class PivotTableCustomLabelFilterCommandBase : PivotTableCustomFiltersCommandBase<PivotTableLabelFiltersViewModel> {
		protected PivotTableCustomLabelFilterCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected char WildCardCharacter { get { return '*'; } }
		protected override void ShowForm(PivotTableLabelFiltersViewModel viewModel) {
			Control.ShowPivotTableLabelFilterForm(viewModel);
		}
		protected override PivotTableLabelFiltersViewModel CreateViewModelCore() {
			return new PivotTableLabelFiltersViewModel(Control);
		}
		protected override void SetupViewModel(PivotFilter filter, PivotTableLabelFiltersViewModel viewModel) {
			viewModel.FirstStringValue = filter.LabelPivot;
			viewModel.ContainsOnlyNumbers = PivotTable.Cache.CacheFields[FieldIndex].SharedItems.ContainsOnlyNumbers;
		}
	}
	#endregion
	#region PivotTableLabelFilterEqualsCommand
	public class PivotTableLabelFilterEqualsCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterEqualsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterEquals; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterEquals; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterEqualsDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionEqual; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterDoesNotEqualCommand
	public class PivotTableLabelFilterDoesNotEqualCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterDoesNotEqualCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterDoesNotEqual; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotEqual; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotEqualDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionNotEqual; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterBeginsWithCommand
	public class PivotTableLabelFilterBeginsWithCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterBeginsWithCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterBeginsWith; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterBeginsWith; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterBeginsWithDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionBeginsWith; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterDoesNotBeginWithCommand
	public class PivotTableLabelFilterDoesNotBeginWithCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterDoesNotBeginWithCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterDoesNotBeginWith; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterDoesNotBeginWith; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterDoesNotBeginWithDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionNotBeginsWith; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterEndsWithCommand
	public class PivotTableLabelFilterEndsWithCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterEndsWithCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterEndsWith; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterEndsWith; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterEndsWithDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionEndsWith; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterDoesNotEndWithCommand
	public class PivotTableLabelFilterDoesNotEndWithCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterDoesNotEndWithCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterDoesNotEndWith; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterDoesNotEndWith; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterDoesNotEndWithDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionNotEndsWith; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterContainsCommand
	public class PivotTableLabelFilterContainsCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterContainsCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterContains; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterContains; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterContainsDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionContains; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterDoesNotContainCommand
	public class PivotTableLabelFilterDoesNotContainCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterDoesNotContainCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterDoesNotContain; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotContain; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterDoesNotContainDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionNotContains; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterGreaterThanCommand
	public class PivotTableLabelFilterGreaterThanCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterGreaterThanCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterGreaterThan; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThan; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThanDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionGreaterThan; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterGreaterThanOrEqualCommand
	public class PivotTableLabelFilterGreaterThanOrEqualCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterGreaterThanOrEqualCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterGreaterThanOrEqual; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThanOrEqualTo; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterGreaterThanOrEqualToDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionGreaterThanOrEqual; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterLessThanCommand
	public class PivotTableLabelFilterLessThanCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterLessThanCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterLessThan; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLessThan; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLessThanDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionLessThan; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterLessThanOrEqualCommand
	public class PivotTableLabelFilterLessThanOrEqualCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterLessThanOrEqualCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterLessThanOrEqual; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLessThanOrEqualTo; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLessThanOrEqualToDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionLessThanOrEqual; } }
		#endregion
	}
	#endregion
	#region PivotTableLabelFilterBetweenCommand
	public class PivotTableLabelFilterBetweenCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterBetweenCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterBetween; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterBetween; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterBetweenDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionBetween; } }
		#endregion
		protected override void SetupViewModel(PivotFilter filter, PivotTableLabelFiltersViewModel viewModel) {
			base.SetupViewModel(filter, viewModel);
			viewModel.SecondStringValue = filter.LabelPivotFilter;
		}
	}
	#endregion
	#region PivotTableLabelFilterNotBetweenCommand
	public class PivotTableLabelFilterNotBetweenCommand : PivotTableCustomLabelFilterCommandBase {
		public PivotTableLabelFilterNotBetweenCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableLabelFilterNotBetween; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterNotBetween; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_PivotLabelFilterNotBetweenDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.CaptionNotBetween; } }
		#endregion
	}
	#endregion
}
