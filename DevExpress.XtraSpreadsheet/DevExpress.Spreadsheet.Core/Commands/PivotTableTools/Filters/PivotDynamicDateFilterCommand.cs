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
namespace DevExpress.XtraSpreadsheet.Commands.Internal {
	#region PivotTableDateFilterTodayCommand
	public class PivotTableDateFilterTodayCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterTodayCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterToday; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterToday; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterTodayDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.Today; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterTomorrowCommand
	public class PivotTableDateFilterTomorrowCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterTomorrowCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterTomorrow; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterTomorrow; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterTomorrowDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.Tomorrow; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterYesterdayCommand
	public class PivotTableDateFilterYesterdayCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterYesterdayCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterYesterday; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterYesterday; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterYesterdayDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.Yesterday; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterThisWeekCommand
	public class PivotTableDateFilterThisWeekCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterThisWeekCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterThisWeek; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterThisWeek; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterThisWeekDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ThisWeek; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterNextWeekCommand
	public class PivotTableDateFilterNextWeekCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterNextWeekCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterNextWeek; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNextWeek; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNextWeekDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.NextWeek; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterLastWeekCommand
	public class PivotTableDateFilterLastWeekCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterLastWeekCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterLastWeek; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLastWeek; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLastWeekDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.LastWeek; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterThisMonthCommand
	public class PivotTableDateFilterThisMonthCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterThisMonthCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterThisMonth; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterThisMonth; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterThisMonthDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ThisMonth; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterNextMonthCommand
	public class PivotTableDateFilterNextMonthCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterNextMonthCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterNextMonth; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNextMonth; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNextMonthDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.NextMonth; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterLastMonthCommand
	public class PivotTableDateFilterLastMonthCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterLastMonthCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterLastMonth; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLastMonth; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLastMonthDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.LastMonth; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterThisQuarterCommand
	public class PivotTableDateFilterThisQuarterCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterThisQuarterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterThisQuarter; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterThisQuarter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterThisQuarterDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ThisQuarter; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterNextQuarterCommand
	public class PivotTableDateFilterNextQuarterCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterNextQuarterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterNextQuarter; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNextQuarter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNextQuarterDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.NextQuarter; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterLastQuarterCommand
	public class PivotTableDateFilterLastQuarterCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterLastQuarterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterLastQuarter; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLastQuarter; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLastQuarterDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.LastQuarter; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterThisYearCommand
	public class PivotTableDateFilterThisYearCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterThisYearCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterThisYear; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterThisYear; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterThisYearDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.ThisYear; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterNextYearCommand
	public class PivotTableDateFilterNextYearCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterNextYearCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterNextYear; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNextYear; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterNextYearDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.NextYear; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterLastYearCommand
	public class PivotTableDateFilterLastYearCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterLastYearCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterLastYear; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLastYear; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterLastYearDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.LastYear; } }
		#endregion
	}
	#endregion
	#region PivotTableDateFilterYearToDateCommand
	public class PivotTableDateFilterYearToDateCommand : PivotTableFiltersCommandBase {
		public PivotTableDateFilterYearToDateCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.PivotTableDateFilterYearToDate; } }
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterYearToDate; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_DataFilterYearToDateDescription; } }
		protected override PivotFilterType FilterType { get { return PivotFilterType.YearToDate; } }
		#endregion
	}
	#endregion
}
