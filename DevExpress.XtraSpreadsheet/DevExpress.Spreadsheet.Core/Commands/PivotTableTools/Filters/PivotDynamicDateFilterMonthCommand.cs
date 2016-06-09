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
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableDateFilterMonthCommandBase
	public abstract class PivotTableDateFilterMonthCommandBase : PivotTableFiltersCommandBase {
		protected PivotTableDateFilterMonthCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterMonth; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterMonthDescription; } }
		public override string MenuCaption { get { return String.Format(base.MenuCaption, GetMonthName()); } }
		public override string Description { get { return String.Format(base.Description, GetMonthName()); } }
		#endregion
		string GetMonthName() {
			int month = Math.Max(0, Math.Min(11, FilterType - PivotFilterType.January));
			return CultureInfo.CurrentUICulture.DateTimeFormat.MonthNames[month];
		}
	}
	#endregion
	#region PivotTableDateFilterJanuaryCommand
	public class PivotTableDateFilterJanuaryCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterJanuaryCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterJanuary; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.January; } }
	}
	#endregion
	#region PivotTableDateFilterFebruaryCommand
	public class PivotTableDateFilterFebruaryCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterFebruaryCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterFebruary; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.February; } }
	}
	#endregion
	#region PivotTableDateFilterMarchCommand
	public class PivotTableDateFilterMarchCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterMarchCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterMarch; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.March; } }
	}
	#endregion
	#region PivotTableDateFilterAprilCommand
	public class PivotTableDateFilterAprilCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterAprilCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterApril; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.April; } }
	}
	#endregion
	#region PivotTableDateFilterMayCommand
	public class PivotTableDateFilterMayCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterMayCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterMay; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.May; } }
	}
	#endregion
	#region PivotTableDateFilterJuneCommand
	public class PivotTableDateFilterJuneCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterJuneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterJune; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.June; } }
	}
	#endregion
	#region PivotTableDateFilterJulyCommand
	public class PivotTableDateFilterJulyCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterJulyCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterJuly; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.July; } }
	}
	#endregion
	#region PivotTableDateFilterAugustCommand
	public class PivotTableDateFilterAugustCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterAugustCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterAugust; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.August; } }
	}
	#endregion
	#region PivotTableDateFilterSeptemberCommand
	public class PivotTableDateFilterSeptemberCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterSeptemberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterSeptember; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.September; } }
	}
	#endregion
	#region PivotTableDateFilterOctoberCommand
	public class PivotTableDateFilterOctoberCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterOctoberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterOctober; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.October; } }
	}
	#endregion
	#region PivotTableDateFilterNovemberCommand
	public class PivotTableDateFilterNovemberCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterNovemberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterNovember; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.November; } }
	}
	#endregion
	#region PivotTableDateFilterDecemberCommand
	public class PivotTableDateFilterDecemberCommand : PivotTableDateFilterMonthCommandBase {
		public PivotTableDateFilterDecemberCommand(ISpreadsheetControl control)
			: base(control) {
		}
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterDecember; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.December; } }
	}
	#endregion
}
