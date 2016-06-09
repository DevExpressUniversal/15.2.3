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
	#region ChartLinesCommandGroup
	public class ChartLinesCommandGroup : ChartCommandGroupBase {
		public ChartLinesCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLinesCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLinesCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLinesCommandGroup; } }
		public override string ImageName { get { return "ChartDrop_Lines"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.Views.Count <= 0)
				return false;
			foreach (IChartView view in chart.Views)
				if (!CanModifyView(view))
					return false;
			return true;
		}
		protected virtual bool CanModifyView(IChartView view) {
			return view is ISupportsDropLines || view is ISupportsHiLowLines || ChartShowSeriesLinesCommand.IsCommandEnabled(view);
		}
	}
	#endregion
	#region ChartModifyDropLinesCommandBase (abstract class)
	public abstract class ChartModifyDropLinesCommandBase : ChartModifyViewCommandBase {
		protected ChartModifyDropLinesCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract bool ShowDropLines { get; }
		protected abstract bool ShowHighLowLines { get; }
		protected abstract bool ShowSeriesLines { get; }
		protected override bool ShouldHideCommand(Chart chart) {
			return !CanModifyChart(chart);
		}
		protected override void ModifyView(IChartView view) {
			ISupportsDropLines dropLines = view as ISupportsDropLines;
			if (dropLines != null)
				dropLines.ShowDropLines = ShowDropLines;
			ISupportsHiLowLines highLowLines = view as ISupportsHiLowLines;
			if (highLowLines != null)
				highLowLines.ShowHiLowLines = ShowHighLowLines;
			ISupportsSeriesLines seriesLines = view as ISupportsSeriesLines;
			if (seriesLines != null && seriesLines.IsSeriesLinesApplicable) {
				seriesLines.SeriesLines.Clear();
				if (ShowSeriesLines)
					seriesLines.SeriesLines.Add(new ShapeProperties(DocumentModel) { Parent = seriesLines.SeriesLines.Parent });
			}
		}
	}
	#endregion
	#region ChartLinesNoneCommand
	public class ChartLinesNoneCommand : ChartModifyDropLinesCommandBase {
		public ChartLinesNoneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLinesNoneCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartLinesNoneCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartLinesNone; } }
		public override string ImageName { get { return "ChartNone_Lines"; } }
		protected override bool ShowDropLines { get { return false; } }
		protected override bool ShowHighLowLines { get { return false; } }
		protected override bool ShowSeriesLines { get { return false; } }
		#endregion
		protected override bool CanModifyView(IChartView view) {
			return view is ISupportsDropLines || view is ISupportsHiLowLines || ChartShowSeriesLinesCommand.IsCommandEnabled(view);
		}
	}
	#endregion
	#region ChartShowDropLinesCommand
	public class ChartShowDropLinesCommand : ChartModifyDropLinesCommandBase {
		public ChartShowDropLinesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowDropLinesCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowDropLinesCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartShowDropLines; } }
		public override string ImageName { get { return "ChartDrop_Lines"; } }
		protected override bool ShowDropLines { get { return true; } }
		protected override bool ShowHighLowLines { get { return false; } }
		protected override bool ShowSeriesLines { get { return false; } }
		#endregion
		protected override bool CanModifyView(IChartView view) {
			return view is ISupportsDropLines;
		}
	}
	#endregion
	#region ChartShowHighLowLinesCommand
	public class ChartShowHighLowLinesCommand : ChartModifyDropLinesCommandBase {
		public ChartShowHighLowLinesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowHighLowLinesCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowHighLowLinesCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartShowHighLowLines; } }
		public override string ImageName { get { return "ChartHighLow_Lines"; } }
		protected override bool ShowDropLines { get { return false; } }
		protected override bool ShowHighLowLines { get { return true; } }
		protected override bool ShowSeriesLines { get { return false; } }
		#endregion
		protected override bool CanModifyView(IChartView view) {
			return view is ISupportsHiLowLines;
		}
	}
	#endregion
	#region ChartShowDropLinesAndHighLowLinesCommand
	public class ChartShowDropLinesAndHighLowLinesCommand : ChartModifyDropLinesCommandBase {
		public ChartShowDropLinesAndHighLowLinesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowDropLinesAndHighLowLinesCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowDropLinesAndHighLowLinesCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartShowDropLinesAndHighLowLines; } }
		public override string ImageName { get { return "ChartDropAndHighLow_Lines"; } }
		protected override bool ShowDropLines { get { return true; } }
		protected override bool ShowHighLowLines { get { return true; } }
		protected override bool ShowSeriesLines { get { return false; } }
		#endregion
		protected override bool CanModifyView(IChartView view) {
			return view is ISupportsDropLines && view is ISupportsHiLowLines;
		}
	}
	#endregion
	#region ChartShowSeriesLinesCommand
	public class ChartShowSeriesLinesCommand : ChartModifyDropLinesCommandBase {
		public ChartShowSeriesLinesCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowSeriesLinesCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartShowSeriesLinesCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartShowSeriesLines; } }
		public override string ImageName { get { return "ShowChartSeriesLines"; } }
		protected override bool ShowDropLines { get { return false; } }
		protected override bool ShowHighLowLines { get { return false; } }
		protected override bool ShowSeriesLines { get { return true; } }
		#endregion
		protected override bool CanModifyView(IChartView view) {
			return IsCommandEnabled(view);
		}
		protected internal static bool IsCommandEnabled(IChartView view) {
			ISupportsSeriesLines supportsSeriesLinesView = view as ISupportsSeriesLines;
			return supportsSeriesLinesView != null && supportsSeriesLinesView.IsSeriesLinesApplicable;
		}
	}
	#endregion
}
