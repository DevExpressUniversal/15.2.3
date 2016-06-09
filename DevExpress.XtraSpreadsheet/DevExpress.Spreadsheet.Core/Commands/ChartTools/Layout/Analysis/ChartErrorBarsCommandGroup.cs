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
	#region ChartErrorBarsCommandGroup
	public class ChartErrorBarsCommandGroup : ChartTrendlineCommandGroupBase {
		public ChartErrorBarsCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartErrorBarsCommandGroup; } }
		public override string ImageName { get { return "ChartErrorBars"; } }
		#endregion
	}
	#endregion
	#region ChartModifyErrorBarsCommandBase (abstract class)
	public abstract class ChartModifyErrorBarsCommandBase : ChartModifyViewSeriesCommandBase {
		protected ChartModifyErrorBarsCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract ErrorValueType ValueType { get; }
		protected virtual bool ShowErrorBars { get { return true; } }
		protected override bool CanModifySeries(ISeries series) {
			return series is SeriesWithErrorBarsAndTrendlines;
		}
		protected override void ModifySeries(ISeries series) {
			SeriesWithErrorBarsAndTrendlines trendlineSeries = series as SeriesWithErrorBarsAndTrendlines;
			if (trendlineSeries == null)
				return;
			trendlineSeries.ErrorBars.Clear();
			if (ShowErrorBars) {
				ErrorBars errorBars = new ErrorBars(trendlineSeries.Parent);
				errorBars.BarType = ErrorBarType.Both;
				errorBars.BarDirection = ErrorBarDirection.Auto;
				errorBars.ValueType = ValueType;
				trendlineSeries.ErrorBars.Add(errorBars);
			}
		}
	}
	#endregion
	#region ChartErrorBarsNoneCommand
	public class ChartErrorBarsNoneCommand : ChartModifyErrorBarsCommandBase {
		public ChartErrorBarsNoneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsNoneCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsNoneCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartErrorBarsNone; } }
		public override string ImageName { get { return "ChartErrorBars_None"; } }
		protected override bool ShowErrorBars { get { return false; } }
		protected override ErrorValueType ValueType { get { return ErrorValueType.Custom; } }
		#endregion
	}
	#endregion
	#region ChartErrorBarsPercentageCommand
	public class ChartErrorBarsPercentageCommand : ChartModifyErrorBarsCommandBase {
		public ChartErrorBarsPercentageCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsPercentageCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsPercentageCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartErrorBarsPercentage; } }
		public override string ImageName { get { return "ChartErrorBars_Percentage"; } }
		protected override ErrorValueType ValueType { get { return ErrorValueType.Percentage; } }
		#endregion
	}
	#endregion
	#region ChartErrorBarsStandardErrorCommand
	public class ChartErrorBarsStandardErrorCommand : ChartModifyErrorBarsCommandBase {
		public ChartErrorBarsStandardErrorCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsStandardErrorCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsStandardErrorCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartErrorBarsStandardError; } }
		public override string ImageName { get { return "ChartErrorBars"; } }
		protected override ErrorValueType ValueType { get { return ErrorValueType.StandardError; } }
		#endregion
	}
	#endregion
	#region ChartErrorBarsStandardDeviationCommand
	public class ChartErrorBarsStandardDeviationCommand : ChartModifyErrorBarsCommandBase {
		public ChartErrorBarsStandardDeviationCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsStandardDeviationCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartErrorBarsStandardDeviationCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartErrorBarsStandardDeviation; } }
		public override string ImageName { get { return "ChartErrorBars_StandardDeviation"; } }
		protected override ErrorValueType ValueType { get { return ErrorValueType.StandardDeviation; } }
		#endregion
	}
	#endregion
}
