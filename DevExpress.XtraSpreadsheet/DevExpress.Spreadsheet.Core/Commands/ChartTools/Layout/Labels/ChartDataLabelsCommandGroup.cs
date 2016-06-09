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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChartDataLabelsCommandGroup
	public class ChartDataLabelsCommandGroup : ChartCommandGroupBase {
		public ChartDataLabelsCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsCommandGroup; } }
		public override string ImageName { get { return "ChartLabels_OutsideEnd"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			return true;
		}
	}
	#endregion
	#region ChartModifyChartDataLabelsCommandBase (abstract class)
	public abstract class ChartModifyChartDataLabelsCommandBase : ModifyChartCommandBase {
		protected ChartModifyChartDataLabelsCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected abstract DataLabelPosition Position { get; }
		protected abstract List<Type> SupportedViews { get; }
		protected override bool CanModifyChart(Chart chart) {
			return chart.Views.Count > 0;
		}
		protected virtual bool IsViewSupported(IChartView view) {
			return SupportedViews.Contains(view.GetType());
		}
		protected override bool ShouldHideCommand(Chart chart) {
			foreach (IChartView view in chart.Views) {
				if (!IsViewSupported(view))
					return true;
			}
			return false;
		}
		protected override bool IsChecked(Chart chart) {
			ChartViewCollection views = chart.Views;
			if (views.Count <= 0)
				return false;
			bool isChecked = IsChecked(views[0]);
			for (int i = 1; i < views.Count; i++) {
				if (IsChecked(views[i]) != isChecked)
					return false;
			}
			return isChecked;
		}
		bool IsChecked(IChartView chartView) {
			ChartViewWithDataLabels view = chartView as ChartViewWithDataLabels;
			if (view == null)
				return false;
			return IsCheckedCore(view);
		}
		protected override void ModifyChart(Chart chart) {
			foreach (IChartView chartView in chart.Views)
			   ModifyDataLabels(chartView as ChartViewWithDataLabels);
		}
		protected virtual bool IsCheckedCore(ChartViewWithDataLabels view) {
			DataLabels dataLabels = view.DataLabels;
			return !dataLabels.Delete && dataLabels.LabelPosition == Position && dataLabels.ShowValue;
		}
		protected virtual void ModifyDataLabels(ChartViewWithDataLabels view) {
			if (view == null)
				return;
			foreach (ISeries series in view.Series)
				CleanSeriesDataLabels(series as SeriesWithDataLabelsAndPoints);
			DataLabels labels = view.DataLabels;
			labels.Labels.Clear();
			labels.LabelPosition = Position;
			labels.ShowCategoryName = false;
			labels.ShowValue = true;
			labels.ShowSeriesName = false;
		}
		protected void CleanSeriesDataLabels(SeriesWithDataLabelsAndPoints series) {
			if (series != null)
				series.DataLabels.Apply = false;
		}
	}
	#endregion
	#region ChartDataLabelsNoneCommand
	public class ChartDataLabelsNoneCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(AreaChartView));
			result.Add(typeof(Area3DChartView));
			result.Add(typeof(BarChartView));
			result.Add(typeof(Bar3DChartView));
			result.Add(typeof(Line3DChartView));
			result.Add(typeof(LineChartView));
			result.Add(typeof(RadarChartView));
			result.Add(typeof(StockChartView));
			result.Add(typeof(BubbleChartView));
			result.Add(typeof(ScatterChartView));
			result.Add(typeof(DoughnutChartView));
			result.Add(typeof(PieChartView));
			result.Add(typeof(Pie3DChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsNoneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsNoneCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsNoneCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsNone; } }
		public override string ImageName { get { return "ChartLabels_None"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.BestFit; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
		protected override bool IsCheckedCore(ChartViewWithDataLabels view) {
			foreach (ISeries series in view.Series)
				if (IsSeriesDataLabelsVisible(series as SeriesWithDataLabelsAndPoints))
					return false;
			return !view.DataLabels.IsVisible;
		}
		protected override void ModifyDataLabels(ChartViewWithDataLabels view) {
			if (view == null)
				return;
			foreach (ISeries series in view.Series)
				CleanSeriesDataLabels(series as SeriesWithDataLabelsAndPoints);
			view.DataLabels.Labels.Clear();
			view.DataLabels.ShowCategoryName = false;
			view.DataLabels.ShowValue = false;
			view.DataLabels.ShowSeriesName = false;
			view.DataLabels.ShowPercent = false;
			view.DataLabels.ShowBubbleSize = false;
		}
		bool IsSeriesDataLabelsVisible(SeriesWithDataLabelsAndPoints series) {
			if (series == null)
				return false;
			if (!series.DataLabels.Apply)
				return false;
			return series.DataLabels.IsVisible;
		}
	}
	#endregion
	#region ChartDataLabelsDefaultCommand
	public class ChartDataLabelsDefaultCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(AreaChartView));
			result.Add(typeof(Area3DChartView));
			result.Add(typeof(Bar3DChartView));
			result.Add(typeof(Line3DChartView));
			result.Add(typeof(RadarChartView));
			result.Add(typeof(DoughnutChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsDefaultCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsDefaultCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsDefaultCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsDefault; } }
		public override string ImageName { get { return "ChartLabels_Show"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.Default; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
	}
	#endregion
	#region ChartDataLabelsCenterCommand
	public class ChartDataLabelsCenterCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(BarChartView));
			result.Add(typeof(LineChartView));
			result.Add(typeof(BubbleChartView));
			result.Add(typeof(ScatterChartView));
			result.Add(typeof(PieChartView));
			result.Add(typeof(Pie3DChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsCenterCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsCenterCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsCenterCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsCenter; } }
		public override string ImageName { get { return "ChartLabels_InsideCenter"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.Center; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
	}
	#endregion
	#region ChartDataLabelsInsideEndCommand
	public class ChartDataLabelsInsideEndCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(BarChartView));
			result.Add(typeof(PieChartView));
			result.Add(typeof(Pie3DChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsInsideEndCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsInsideEndCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsInsideEndCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsInsideEnd; } }
		public override string ImageName { get { return "ChartLabels_InsideEnd"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.InsideEnd; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
	}
	#endregion
	#region ChartDataLabelsInsideBaseCommand
	public class ChartDataLabelsInsideBaseCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(BarChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsInsideBaseCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsInsideBaseCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsInsideBaseCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsInsideBase; } }
		public override string ImageName { get { return "ChartLabels_InsideBase"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.InsideBase; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
	}
	#endregion
	#region ChartDataLabelsOutsideEndCommand
	public class ChartDataLabelsOutsideEndCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(BarChartView));
			result.Add(typeof(PieChartView));
			result.Add(typeof(Pie3DChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsOutsideEndCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsOutsideEndCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsOutsideEndCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsOutsideEnd; } }
		public override string ImageName { get { return "ChartLabels_OutsideEnd"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.OutsideEnd; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
		protected override bool IsViewSupported(IChartView view) {
			if (!base.IsViewSupported(view))
				return false;
			BarChartView barView = view as BarChartView;
			if (barView == null)
				return true;
			return barView.Grouping == BarChartGrouping.Clustered || barView.Grouping == BarChartGrouping.Standard;
		}
	}
	#endregion
	#region ChartDataLabelsBestFitCommand
	public class ChartDataLabelsBestFitCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(PieChartView));
			result.Add(typeof(Pie3DChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsBestFitCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsBestFitCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsBestFitCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsBestFit; } }
		public override string ImageName { get { return "ChartLabels_BestFit"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.BestFit; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
	}
	#endregion
	#region ChartDataLabelsLeftCommand
	public class ChartDataLabelsLeftCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(LineChartView));
			result.Add(typeof(BubbleChartView));
			result.Add(typeof(ScatterChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsLeftCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsLeftCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsLeftCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsLeft; } }
		public override string ImageName { get { return "ChartLabels_LineLeft"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.Left; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
	}
	#endregion
	#region ChartDataLabelsAboveCommand
	public class ChartDataLabelsAboveCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(LineChartView));
			result.Add(typeof(BubbleChartView));
			result.Add(typeof(ScatterChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsAboveCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsAboveCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsAboveCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsAbove; } }
		public override string ImageName { get { return "ChartLabels_LineAbove"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.Top; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
	}
	#endregion
	#region ChartDataLabelsRightCommand
	public class ChartDataLabelsRightCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(LineChartView));
			result.Add(typeof(BubbleChartView));
			result.Add(typeof(ScatterChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsRightCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsRightCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsRightCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsRight; } }
		public override string ImageName { get { return "ChartLabels_LineRight"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.Right; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
	}
	#endregion
	#region ChartDataLabelsBelowCommand
	public class ChartDataLabelsBelowCommand : ChartModifyChartDataLabelsCommandBase {
		static readonly List<Type> supportedViewTypes = CreateSupportedViewTypes();
		#region CreateSupportedViewTypes
		static List<Type> CreateSupportedViewTypes() {
			List<Type> result = new List<Type>();
			result.Add(typeof(LineChartView));
			result.Add(typeof(BubbleChartView));
			result.Add(typeof(ScatterChartView));
			return result;
		}
		#endregion
		public ChartDataLabelsBelowCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsBelowCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartDataLabelsBelowCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartDataLabelsBelow; } }
		public override string ImageName { get { return "ChartLabels_LineBelow"; } }
		protected override DataLabelPosition Position { get { return DataLabelPosition.Bottom; } }
		protected override List<Type> SupportedViews { get { return supportedViewTypes; } }
		#endregion
	}
	#endregion
}
