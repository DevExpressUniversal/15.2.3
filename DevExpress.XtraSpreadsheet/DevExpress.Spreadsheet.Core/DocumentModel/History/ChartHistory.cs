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
using System.Text;
using DevExpress.Office.Drawing;
using DevExpress.XtraSpreadsheet.Drawing;
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region ChartInsertedHistoryItem
	public class ChartInsertedHistoryItem : SpreadsheetHistoryItem {
		Chart chart;
		int position;
		bool undone;
		public ChartInsertedHistoryItem(Worksheet worksheet, Chart chart)
			: base(worksheet) {
			this.chart = chart;
		}
		#region Dispose
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (this.chart != null && undone)
						this.chart.Dispose();
					this.chart = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#endregion
		protected override void RedoCore() {
			position = Worksheet.DrawingObjects.Add(chart);
			if (undone) {
				chart.Activate();
				undone = false;
			}
		}
		protected override void UndoCore() {
			Worksheet.DrawingObjects.RemoveAt(position);
			if (!undone) {
				chart.Deactivate();
				undone = true;
			}
		}
	}
	#endregion
	#region ChartStylePropertyChangedHistoryItem
	public class ChartStylePropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly Chart chart;
		public ChartStylePropertyChangedHistoryItem(DocumentModel documentModel, Chart chart, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.chart = chart;
		}
		protected override void UndoCore() {
			chart.SetStyleCore(OldValue);
		}
		protected override void RedoCore() {
			chart.SetStyleCore(NewValue);
		}
	}
	#endregion
	#region LegendHistory
	#region LegendOvelayPropertyChangedHistoryItem
	public class LegendOvelayPropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly Legend legend;
		public LegendOvelayPropertyChangedHistoryItem(DocumentModel documentModel, Legend legend, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.legend = legend;
		}
		protected override void UndoCore() {
			legend.SetOverlayCore(OldValue);
		}
		protected override void RedoCore() {
			legend.SetOverlayCore(NewValue);
		}
	}
	#endregion
	#region LegendVisiblePropertyChangedHistoryItem
	public class LegendVisiblePropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly Legend legend;
		public LegendVisiblePropertyChangedHistoryItem(DocumentModel documentModel, Legend legend, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.legend = legend;
		}
		protected override void UndoCore() {
			legend.SetVisibleCore(OldValue);
		}
		protected override void RedoCore() {
			legend.SetVisibleCore(NewValue);
		}
	}
	#endregion
	#region LegendPositionPropertyChangedHistoryItem
	public class LegendPositionPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly Legend legend;
		readonly LegendPosition oldValue;
		readonly LegendPosition newValue;
		public LegendPositionPropertyChangedHistoryItem(DocumentModel documentModel, Legend legend, LegendPosition oldValue, LegendPosition newValue)
			: base(documentModel) {
			this.legend = legend;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			legend.SetPositionCore(oldValue);
		}
		protected override void RedoCore() {
			legend.SetPositionCore(newValue);
		}
	}
	#endregion
	#region LegendEntryDeletePropertyChangedHistoryItem
	public class LegendEntryDeletePropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly LegendEntry entry;
		public LegendEntryDeletePropertyChangedHistoryItem(DocumentModel documentModel, LegendEntry entry, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.entry = entry;
		}
		protected override void UndoCore() {
			entry.SetDeleteCore(OldValue);
		}
		protected override void RedoCore() {
			entry.SetDeleteCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region LayoutOptionsHistory
	#region ManualLayoutLeftPropertyChangedHistoryItem
	public class ManualLayoutLeftPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly LayoutOptions layout;
		readonly ManualLayoutPosition oldValue;
		readonly ManualLayoutPosition newValue;
		public ManualLayoutLeftPropertyChangedHistoryItem(DocumentModel documentModel, LayoutOptions layout, ManualLayoutPosition oldValue, ManualLayoutPosition newValue)
			: base(documentModel) {
			this.layout = layout;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		protected override void UndoCore() {
			layout.SetLeftCore(oldValue);
		}
		protected override void RedoCore() {
			layout.SetLeftCore(newValue);
		}
	}
	#endregion
	#region ManualLayoutTopPropertyChangedHistoryItem
	public class ManualLayoutTopPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly LayoutOptions layout;
		readonly ManualLayoutPosition oldValue;
		readonly ManualLayoutPosition newValue;
		public ManualLayoutTopPropertyChangedHistoryItem(DocumentModel documentModel, LayoutOptions layout, ManualLayoutPosition oldValue, ManualLayoutPosition newValue)
			: base(documentModel) {
			this.layout = layout;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		protected override void UndoCore() {
			layout.SetTopCore(oldValue);
		}
		protected override void RedoCore() {
			layout.SetTopCore(newValue);
		}
	}
	#endregion
	#region ManualLayoutWidthPropertyChangedHistoryItem
	public class ManualLayoutWidthPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly LayoutOptions layout;
		readonly ManualLayoutPosition oldValue;
		readonly ManualLayoutPosition newValue;
		public ManualLayoutWidthPropertyChangedHistoryItem(DocumentModel documentModel, LayoutOptions layout, ManualLayoutPosition oldValue, ManualLayoutPosition newValue)
			: base(documentModel) {
			this.layout = layout;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		protected override void UndoCore() {
			layout.SetWidthCore(oldValue);
		}
		protected override void RedoCore() {
			layout.SetWidthCore(newValue);
		}
	}
	#endregion
	#region ManualLayoutHeightPropertyChangedHistoryItem
	public class ManualLayoutHeightPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly LayoutOptions layout;
		readonly ManualLayoutPosition oldValue;
		readonly ManualLayoutPosition newValue;
		public ManualLayoutHeightPropertyChangedHistoryItem(DocumentModel documentModel, LayoutOptions layout, ManualLayoutPosition oldValue, ManualLayoutPosition newValue)
			: base(documentModel) {
			this.layout = layout;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		protected override void UndoCore() {
			layout.SetHeightCore(oldValue);
		}
		protected override void RedoCore() {
			layout.SetHeightCore(newValue);
		}
	}
	#endregion
	#region ManualLayoutTargetPropertyChangedHistoryItem
	public class ManualLayoutTargetPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly LayoutOptions layout;
		readonly LayoutTarget oldValue;
		readonly LayoutTarget newValue;
		public ManualLayoutTargetPropertyChangedHistoryItem(DocumentModel documentModel, LayoutOptions layout, LayoutTarget oldValue, LayoutTarget newValue)
			: base(documentModel) {
			this.layout = layout;
			this.newValue = newValue;
			this.oldValue = oldValue;
		}
		protected override void UndoCore() {
			layout.SetTargetCore(oldValue);
		}
		protected override void RedoCore() {
			layout.SetTargetCore(newValue);
		}
	}
	#endregion
	#endregion
	#region SurfaceOptionsHistory
	#region SurfaceOptionsThicknessPropertyChangedHistoryItem
	public class SurfaceOptionsThicknessPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly SurfaceOptions options;
		public SurfaceOptionsThicknessPropertyChangedHistoryItem(DocumentModel documentModel, SurfaceOptions options, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.options = options;
		}
		protected override void UndoCore() {
			options.SetThicknessCore(OldValue);
		}
		protected override void RedoCore() {
			options.SetThicknessCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region PictureOptionsHistory
	#region PictureOptionsStackUnitsPropertyChangedHistoryItem
	public class PictureOptionsStackUnitPropertyChangedHistoryItem : SpreadsheetDoubleHistoryItem {
		readonly PictureOptions options;
		public PictureOptionsStackUnitPropertyChangedHistoryItem(DocumentModel documentModel, PictureOptions options, double oldValue, double newValue)
			: base(documentModel, oldValue, newValue) {
			this.options = options;
		}
		protected override void UndoCore() {
			options.SetPictureStackUnitCore(OldValue);
		}
		protected override void RedoCore() {
			options.SetPictureStackUnitCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region ColorMapOverrideHistory
	#region ColorMapOverridePropertyChangedHistoryItem
	public class ColorMapOverridePropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly ColorMapOverride map;
		readonly int index;
		readonly ColorSchemeIndex oldValue;
		readonly ColorSchemeIndex newValue;
		public ColorMapOverridePropertyChangedHistoryItem(DocumentModel documentModel, ColorMapOverride map, int index, ColorSchemeIndex oldValue, ColorSchemeIndex newValue)
			: base(documentModel) {
			this.map = map;
			this.index = index;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			map.SetMapEntryCore(index, oldValue);
		}
		protected override void RedoCore() {
			map.SetMapEntryCore(index, newValue);
		}
	}
	#endregion
	#region ColorMapOverrideIsDefinedPropertyChangedHistoryItem
	public class ColorMapOverrideIsDefinedPropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly ColorMapOverride map;
		public ColorMapOverrideIsDefinedPropertyChangedHistoryItem(DocumentModel documentModel, ColorMapOverride map, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.map = map;
		}
		protected override void UndoCore() {
			map.SetIsDefinedCore(OldValue);
		}
		protected override void RedoCore() {
			map.SetIsDefinedCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region AxisHistory
	#region AxisCrossesValuePropertyChangedHistoryItem
	public class AxisCrossesValuePropertyChangedHistoryItem : SpreadsheetDoubleHistoryItem {
		readonly AxisBase axis;
		public AxisCrossesValuePropertyChangedHistoryItem(DocumentModel documentModel, AxisBase axis, double oldValue, double newValue)
			: base(documentModel, oldValue, newValue) {
			this.axis = axis;
		}
		protected override void UndoCore() {
			axis.SetCrossesValueCore(OldValue);
		}
		protected override void RedoCore() {
			axis.SetCrossesValueCore(NewValue);
		}
	}
	#endregion
	#region NumberFormatCodePropertyChangedHistoryItem
	public class NumberFormatCodePropertyChangedHistoryItem : SpreadsheetStringHistoryItem {
		readonly NumberFormatOptions numberFormat;
		public NumberFormatCodePropertyChangedHistoryItem(DocumentModel documentModel, NumberFormatOptions numberFormat, string oldValue, string newValue)
			: base(documentModel, oldValue, newValue) {
				this.numberFormat = numberFormat;
		}
		protected override void UndoCore() {
			numberFormat.SetNumberFormatCodeCore(OldValue);
		}
		protected override void RedoCore() {
			numberFormat.SetNumberFormatCodeCore(NewValue);
		}
	}
	#endregion
	#region NumberFormatSourceLinkedPropertyChangedHistoryItem
	public class NumberFormatSourceLinkedPropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly NumberFormatOptions numberFormat;
		public NumberFormatSourceLinkedPropertyChangedHistoryItem(DocumentModel documentModel, NumberFormatOptions numberFormat, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.numberFormat = numberFormat;
		}
		protected override void UndoCore() {
			numberFormat.SetSourceLinkedCore(OldValue);
		}
		protected override void RedoCore() {
			numberFormat.SetSourceLinkedCore(NewValue);
		}
	}
	#endregion
	#region AxisPositionPropertyChangedHistoryItem
	public class AxisPositionPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly AxisBase axis;
		readonly AxisPosition oldValue;
		readonly AxisPosition newValue;
		public AxisPositionPropertyChangedHistoryItem(DocumentModel documentModel, AxisBase axis, AxisPosition oldValue, AxisPosition newValue)
			: base(documentModel) {
			this.axis = axis;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			axis.SetPositionCore(oldValue);
		}
		protected override void RedoCore() {
			axis.SetPositionCore(newValue);
		}
	}
	#endregion
	#region AxisTickLabelSkipPropertyChangedHistoryItem
	public class AxisTickLabelSkipPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly AxisTickBase axis;
		public AxisTickLabelSkipPropertyChangedHistoryItem(DocumentModel documentModel, AxisTickBase axis, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.axis = axis;
		}
		protected override void UndoCore() {
			axis.SetTickLabelSkipCore(OldValue);
		}
		protected override void RedoCore() {
			axis.SetTickLabelSkipCore(NewValue);
		}
	}
	#endregion
	#region AxisTickMarkSkipPropertyChangedHistoryItem
	public class AxisTickMarkSkipPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly AxisTickBase axis;
		public AxisTickMarkSkipPropertyChangedHistoryItem(DocumentModel documentModel, AxisTickBase axis, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.axis = axis;
		}
		protected override void UndoCore() {
			axis.SetTickMarkSkipCore(OldValue);
		}
		protected override void RedoCore() {
			axis.SetTickMarkSkipCore(NewValue);
		}
	}
	#endregion
	#region AxisMajorUnitPropertyChangedHistoryItem
	public class AxisMajorUnitPropertyChangedHistoryItem : SpreadsheetDoubleHistoryItem {
		readonly AxisMMUnitsBase axis;
		public AxisMajorUnitPropertyChangedHistoryItem(DocumentModel documentModel, AxisMMUnitsBase axis, double oldValue, double newValue)
			: base(documentModel, oldValue, newValue) {
			this.axis = axis;
		}
		protected override void UndoCore() {
			axis.SetMajorUnitCore(OldValue);
		}
		protected override void RedoCore() {
			axis.SetMajorUnitCore(NewValue);
		}
	}
	#endregion
	#region AxisMinorUnitPropertyChangedHistoryItem
	public class AxisMinorUnitPropertyChangedHistoryItem : SpreadsheetDoubleHistoryItem {
		readonly AxisMMUnitsBase axis;
		public AxisMinorUnitPropertyChangedHistoryItem(DocumentModel documentModel, AxisMMUnitsBase axis, double oldValue, double newValue)
			: base(documentModel, oldValue, newValue) {
			this.axis = axis;
		}
		protected override void UndoCore() {
			axis.SetMinorUnitCore(OldValue);
		}
		protected override void RedoCore() {
			axis.SetMinorUnitCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region TitleOptionsHistory
	#region TitleOptionsOverlayPropertyChangedHistoryItem
	public class TitleOptionsOverlayPropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly TitleOptions options;
		public TitleOptionsOverlayPropertyChangedHistoryItem(DocumentModel documentModel, TitleOptions options, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.options = options;
		}
		protected override void UndoCore() {
			options.SetOverlayCore(OldValue);
		}
		protected override void RedoCore() {
			options.SetOverlayCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region ChartTextHistory
	#region ChartTextPropertyChangedHistoryItem
	public class ChartTextPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly IChartTextOwner owner;
		readonly IChartText oldValue;
		readonly IChartText newValue;
		public ChartTextPropertyChangedHistoryItem(DocumentModel documentModel, IChartTextOwner owner, IChartText oldValue, IChartText newValue)
			: base(documentModel) {
			this.owner = owner;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			owner.SetTextCore(oldValue);
		}
		protected override void RedoCore() {
			owner.SetTextCore(newValue);
		}
	}
	#endregion
	#region ChartTextPlainTextPropertyHistoryItem
	public class ChartTextPlainTextPropertyChangedHistoryItem : SpreadsheetStringHistoryItem {
		readonly ChartPlainText text;
		public ChartTextPlainTextPropertyChangedHistoryItem(DocumentModel documentModel, ChartPlainText text, string oldValue, string newValue) 
			: base(documentModel, oldValue, newValue) {
			this.text = text;
		}
		protected override void UndoCore() {
			text.SetPlainTextCore(OldValue);
		}
		protected override void RedoCore() {
			text.SetPlainTextCore(NewValue);
		}
	}
	#endregion
	#region ChartTextRunTextPropertyHistoryItem
	public class ChartTextRunTextPropertyChangedHistoryItem : SpreadsheetStringHistoryItem {
		readonly DrawingTextRun textRun;
		public ChartTextRunTextPropertyChangedHistoryItem(DocumentModel documentModel, DrawingTextRun textRun, string oldValue, string newValue)
			: base(documentModel, oldValue, newValue) {
			this.textRun = textRun;
		}
		protected override void UndoCore() {
			textRun.SetTextCore(OldValue);
		}
		protected override void RedoCore() {
			textRun.SetTextCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region ChartViewHistory
	#region ChartViewAxesPropertyChangedHistoryItem
	public class ChartViewAxesPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly ChartViewBase view;
		readonly AxisGroup oldValue;
		readonly AxisGroup newValue;
		public ChartViewAxesPropertyChangedHistoryItem(DocumentModel documentModel, ChartViewBase view, AxisGroup oldValue, AxisGroup newValue)
			: base(documentModel) {
			this.view = view;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			view.SetAxesCore(oldValue);
		}
		protected override void RedoCore() {
			view.SetAxesCore(newValue);
		}
	}
	#endregion
	#region ChartViewGapWidthPropertyChangedHistoryItem
	public class ChartViewGapWidthPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly IChartViewWithGapWidth view;
		public ChartViewGapWidthPropertyChangedHistoryItem(DocumentModel documentModel, IChartViewWithGapWidth view, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.view = view;
		}
		protected override void UndoCore() {
			view.SetGapWidthCore(OldValue);
		}
		protected override void RedoCore() {
			view.SetGapWidthCore(NewValue);
		}
	}
	#endregion
	#region BarChartOverlapPropertyChangedHistoryItem
	public class BarChartOverlapPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly BarChartView view;
		public BarChartOverlapPropertyChangedHistoryItem(DocumentModel documentModel, BarChartView view, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.view = view;
		}
		protected override void UndoCore() {
			view.SetOverlapCore(OldValue);
		}
		protected override void RedoCore() {
			view.SetOverlapCore(NewValue);
		}
	}
	#endregion
	#region BarChartShapePropertyChangedHistoryItem
	public class BarChartShapePropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly Bar3DChartView view;
		readonly BarShape oldValue;
		readonly BarShape newValue;
		public BarChartShapePropertyChangedHistoryItem(DocumentModel documentModel, Bar3DChartView view, BarShape oldValue, BarShape newValue)
			: base(documentModel) {
			this.view = view;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			view.SetShapeCore(oldValue);
		}
		protected override void RedoCore() {
			view.SetShapeCore(newValue);
		}
	}
	#endregion
	#region ChartViewGapDepthPropertyChangedHistoryItem
	public class ChartViewGapDepthPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly IChartViewWithGapDepth view;
		public ChartViewGapDepthPropertyChangedHistoryItem(DocumentModel documentModel, IChartViewWithGapDepth view, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.view = view;
		}
		protected override void UndoCore() {
			view.SetGapDepthCore(OldValue);
		}
		protected override void RedoCore() {
			view.SetGapDepthCore(NewValue);
		}
	}
	#endregion
	#region ChartViewFirstSliceAnglePropertyChangedHistoryItem
	public class ChartViewFirstSliceAnglePropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly ChartViewWithSlice view;
		public ChartViewFirstSliceAnglePropertyChangedHistoryItem(DocumentModel documentModel, ChartViewWithSlice view, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.view = view;
		}
		protected override void UndoCore() {
			view.SetFirstSliceAngleCore(OldValue);
		}
		protected override void RedoCore() {
			view.SetFirstSliceAngleCore(NewValue);
		}
	}
	#endregion
	#region RadarChartStylePropertyChangedHistoryItem
	public class RadarChartStylePropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly RadarChartView view;
		readonly RadarChartStyle oldValue;
		readonly RadarChartStyle newValue;
		public RadarChartStylePropertyChangedHistoryItem(DocumentModel documentModel, RadarChartView view, RadarChartStyle oldValue, RadarChartStyle newValue)
			: base(documentModel) {
			this.view = view;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			view.SetRadarStyleCore(oldValue);
		}
		protected override void RedoCore() {
			view.SetRadarStyleCore(newValue);
		}
	}
	#endregion
	#region ScatterChartStylePropertyChangedHistoryItem
	public class ScatterChartStylePropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly ScatterChartView view;
		readonly ScatterChartStyle oldValue;
		readonly ScatterChartStyle newValue;
		public ScatterChartStylePropertyChangedHistoryItem(DocumentModel documentModel, ScatterChartView view, ScatterChartStyle oldValue, ScatterChartStyle newValue)
			: base(documentModel) {
			this.view = view;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			view.SetScatterStyleCore(oldValue);
		}
		protected override void RedoCore() {
			view.SetScatterStyleCore(newValue);
		}
	}
	#endregion
	#region DoughnutChartHoleSizePropertyChangedHistoryItem
	public class DoughnutChartHoleSizePropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly DoughnutChartView view;
		public DoughnutChartHoleSizePropertyChangedHistoryItem(DocumentModel documentModel, DoughnutChartView view, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.view = view;
		}
		protected override void UndoCore() {
			view.SetHoleSizeCore(OldValue);
		}
		protected override void RedoCore() {
			view.SetHoleSizeCore(NewValue);
		}
	}
	#endregion
	#region BubbleScalePropertyChangedHistoryItem
	public class BubbleScalePropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly BubbleChartView view;
		public BubbleScalePropertyChangedHistoryItem(DocumentModel documentModel, BubbleChartView view, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.view = view;
		}
		protected override void UndoCore() {
			view.SetBubbleScaleCore(OldValue);
		}
		protected override void RedoCore() {
			view.SetBubbleScaleCore(NewValue);
		}
	}
	#endregion
	#region OfPieSecondPieSizePropertyChangedHistoryItem
	public class OfPieSecondPieSizePropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly OfPieChartView view;
		public OfPieSecondPieSizePropertyChangedHistoryItem(DocumentModel documentModel, OfPieChartView view, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.view = view;
		}
		protected override void UndoCore() {
			view.SetSecondPieSizeCore(OldValue);
		}
		protected override void RedoCore() {
			view.SetSecondPieSizeCore(NewValue);
		}
	}
	#endregion
	#region OfPieSplitPosPropertyChangedHistoryItem
	public class OfPieSplitPosPropertyChangedHistoryItem : SpreadsheetDoubleHistoryItem {
		readonly OfPieChartView view;
		public OfPieSplitPosPropertyChangedHistoryItem(DocumentModel documentModel, OfPieChartView view, double oldValue, double newValue)
			: base(documentModel, oldValue, newValue) {
			this.view = view;
		}
		protected override void UndoCore() {
			view.SetSplitPosCore(OldValue);
		}
		protected override void RedoCore() {
			view.SetSplitPosCore(NewValue);
		}
	}
	#endregion
	#region OfPieSplitTypePropertyChangedHistoryItem
	public class OfPieSplitTypePropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly OfPieChartView view;
		readonly OfPieSplitType oldValue;
		readonly OfPieSplitType newValue;
		public OfPieSplitTypePropertyChangedHistoryItem(DocumentModel documentModel, OfPieChartView view, OfPieSplitType oldValue, OfPieSplitType newValue)
			: base(documentModel) {
			this.view = view;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			view.SetSplitTypeCore(oldValue);
		}
		protected override void RedoCore() {
			view.SetSplitTypeCore(newValue);
		}
	}
	#endregion
	#endregion
	#region DataReferenceHistoryItem
	public abstract class DataReferenceHistoryItem : SpreadsheetHistoryItem {
		readonly IDataReference oldValue;
		readonly IDataReference newValue;
		protected DataReferenceHistoryItem(DocumentModel documentModel, IDataReference oldValue, IDataReference newValue)
			: base(documentModel) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public IDataReference OldValue { get { return oldValue; } }
		public IDataReference NewValue { get { return newValue; } }
	}
	#endregion
	#region InvertIfNegativePropertyChangedHistoryItem
	public class InvertIfNegativePropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly ISupportsInvertIfNegative owner;
		public InvertIfNegativePropertyChangedHistoryItem(DocumentModel documentModel, ISupportsInvertIfNegative owner, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.owner = owner;
		}
		protected override void UndoCore() {
			this.owner.SetInvertIfNegativeCore(OldValue);
		}
		protected override void RedoCore() {
			this.owner.SetInvertIfNegativeCore(NewValue);
		}
	}
	#endregion
	#region SeriesHistory
	#region SeriesIndexPropertyChangedHistoryItem
	public class SeriesIndexPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly SeriesBase series;
		public SeriesIndexPropertyChangedHistoryItem(DocumentModel documentModel, SeriesBase series, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.series = series;
		}
		protected override void UndoCore() {
			series.SetIndexCore(OldValue);
		}
		protected override void RedoCore() {
			series.SetIndexCore(NewValue);
		}
	}
	#endregion
	#region SeriesOrderPropertyChangedHistoryItem
	public class SeriesOrderPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly SeriesBase series;
		public SeriesOrderPropertyChangedHistoryItem(DocumentModel documentModel, SeriesBase series, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.series = series;
		}
		protected override void UndoCore() {
			series.SetOrderCore(OldValue);
		}
		protected override void RedoCore() {
			series.SetOrderCore(NewValue);
		}
	}
	#endregion
	#region SeriesArgumentsPropertyChangedHistoryItem
	public class SeriesArgumentsPropertyChangedHistoryItem : DataReferenceHistoryItem {
		readonly SeriesBase series;
		public SeriesArgumentsPropertyChangedHistoryItem(DocumentModel documentModel, SeriesBase series, IDataReference oldValue, IDataReference newValue)
			: base(documentModel, oldValue, newValue) {
			this.series = series;
		}
		protected override void UndoCore() {
			series.SetArgumentsCore(OldValue);
		}
		protected override void RedoCore() {
			series.SetArgumentsCore(NewValue);
		}
	}
	#endregion
	#region SeriesValuesPropertyChangedHistoryItem
	public class SeriesValuesPropertyChangedHistoryItem : DataReferenceHistoryItem {
		readonly SeriesBase series;
		public SeriesValuesPropertyChangedHistoryItem(DocumentModel documentModel, SeriesBase series, IDataReference oldValue, IDataReference newValue)
			: base(documentModel, oldValue, newValue) {
			this.series = series;
		}
		protected override void UndoCore() {
			series.SetValuesCore(OldValue);
		}
		protected override void RedoCore() {
			series.SetValuesCore(NewValue);
		}
	}
	#endregion
	#region BarSeriesShapePropertyChangedHistoryItem
	public class BarSeriesShapePropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly BarSeries series;
		readonly BarShape oldValue;
		readonly BarShape newValue;
		public BarSeriesShapePropertyChangedHistoryItem(DocumentModel documentModel, BarSeries series, BarShape oldValue, BarShape newValue)
			: base(documentModel) {
			this.series = series;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			this.series.SetShapeCore(oldValue);
		}
		protected override void RedoCore() {
			this.series.SetShapeCore(newValue);
		}
	}
	#endregion
	#region PieSeriesExplosionPropertyChangedHistoryItem
	public class PieSeriesExplosionPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly PieSeries series;
		public PieSeriesExplosionPropertyChangedHistoryItem(DocumentModel documentModel, PieSeries series, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.series = series;
		}
		protected override void UndoCore() {
			series.SetExplosionCore(OldValue);
		}
		protected override void RedoCore() {
			series.SetExplosionCore(NewValue);
		}
	}
	#endregion
	#region SeriesSmoothPropertyChangedHistoryItem
	public class SeriesSmoothPropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly SeriesWithMarkerAndSmooth series;
		public SeriesSmoothPropertyChangedHistoryItem(DocumentModel documentModel, SeriesWithMarkerAndSmooth series, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.series = series;
		}
		protected override void UndoCore() {
			this.series.SetSmoothCore(OldValue);
		}
		protected override void RedoCore() {
			this.series.SetSmoothCore(NewValue);
		}
	}
	#endregion
	#region BubbleSizePropertyChangedHistoryItem
	public class BubbleSizePropertyChangedHistoryItem : DataReferenceHistoryItem {
		readonly BubbleSeries series;
		public BubbleSizePropertyChangedHistoryItem(DocumentModel documentModel, BubbleSeries series, IDataReference oldValue, IDataReference newValue)
			: base(documentModel, oldValue, newValue) {
			this.series = series;
		}
		protected override void UndoCore() {
			series.SetBubbleSizeCore(OldValue);
		}
		protected override void RedoCore() {
			series.SetBubbleSizeCore(NewValue);
		}
	}
	#endregion
	#region Bubble3DPropertyChangedHistoryItem
	public class Bubble3DPropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly BubbleSeries series;
		public Bubble3DPropertyChangedHistoryItem(DocumentModel documentModel, BubbleSeries series, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.series = series;
		}
		protected override void UndoCore() {
			this.series.SetBubble3DCore(OldValue);
		}
		protected override void RedoCore() {
			this.series.SetBubble3DCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region DataPointHistory
	#region DataPointBubble3DPropertyChangedHistoryItem
	public class DataPointBubble3DPropertyChangedHistoryItem : SpreadsheetBooleanHistoryItem {
		readonly DataPoint point;
		public DataPointBubble3DPropertyChangedHistoryItem(DocumentModel documentModel, DataPoint point, bool oldValue, bool newValue)
			: base(documentModel, oldValue, newValue) {
			this.point = point;
		}
		protected override void UndoCore() {
			this.point.SetBubble3DCore(OldValue);
		}
		protected override void RedoCore() {
			this.point.SetBubble3DCore(NewValue);
		}
	}
	#endregion
	#region DataPointExplosionPropertyChangedHistoryItem
	public class DataPointExplosionPropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly DataPoint point;
		public DataPointExplosionPropertyChangedHistoryItem(DocumentModel documentModel, DataPoint point, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.point = point;
		}
		protected override void UndoCore() {
			this.point.SetExplosionCore(OldValue);
		}
		protected override void RedoCore() {
			this.point.SetExplosionCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region MarkerHistory
	#region MarkerSymbolPropertyChangedHistoryItem
	public class MarkerSymbolPropertyChangedHistoryItem : SpreadsheetHistoryItem {
		readonly Marker marker;
		readonly MarkerStyle oldValue;
		readonly MarkerStyle newValue;
		public MarkerSymbolPropertyChangedHistoryItem(DocumentModel documentModel, Marker marker, MarkerStyle oldValue, MarkerStyle newValue)
			: base(documentModel) {
			this.marker = marker;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			this.marker.SetSymbolCore(oldValue);
		}
		protected override void RedoCore() {
			this.marker.SetSymbolCore(newValue);
		}
	}
	#endregion
	#region MarkerSizePropertyChangedHistoryItem
	public class MarkerSizePropertyChangedHistoryItem : SpreadsheetIntHistoryItem {
		readonly Marker marker;
		public MarkerSizePropertyChangedHistoryItem(DocumentModel documentModel, Marker marker, int oldValue, int newValue)
			: base(documentModel, oldValue, newValue) {
			this.marker = marker;
		}
		protected override void UndoCore() {
			this.marker.SetSizeCore(OldValue);
		}
		protected override void RedoCore() {
			this.marker.SetSizeCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region DataLabelHistory
	#region DataLabelSeparatorPropertyChangedHistoryItem
	public class DataLabelSeparatorPropertyChangedHistoryItem : SpreadsheetStringHistoryItem {
		readonly DataLabelBase label;
		public DataLabelSeparatorPropertyChangedHistoryItem(DocumentModel documentModel, DataLabelBase label, string oldValue, string newValue)
			: base(documentModel, oldValue, newValue) {
			this.label = label;
		}
		protected override void UndoCore() {
			this.label.SetSeparatorCore(OldValue);
		}
		protected override void RedoCore() {
			this.label.SetSeparatorCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region ErrorBarsHistory
	#region ErrorBarsMinusPropertyChangedHistoryItem
	public class ErrorBarsMinusPropertyChangedHistoryItem : DataReferenceHistoryItem {
		readonly ErrorBars errorBars;
		public ErrorBarsMinusPropertyChangedHistoryItem(DocumentModel documentModel, ErrorBars errorBars, IDataReference oldValue, IDataReference newValue)
			: base(documentModel, oldValue, newValue) {
			this.errorBars = errorBars;
		}
		protected override void UndoCore() {
			errorBars.SetMinusCore(OldValue);
		}
		protected override void RedoCore() {
			errorBars.SetMinusCore(NewValue);
		}
	}
	#endregion
	#region ErrorBarsPlusPropertyChangedHistoryItem
	public class ErrorBarsPlusPropertyChangedHistoryItem : DataReferenceHistoryItem {
		readonly ErrorBars errorBars;
		public ErrorBarsPlusPropertyChangedHistoryItem(DocumentModel documentModel, ErrorBars errorBars, IDataReference oldValue, IDataReference newValue)
			: base(documentModel, oldValue, newValue) {
			this.errorBars = errorBars;
		}
		protected override void UndoCore() {
			errorBars.SetPlusCore(OldValue);
		}
		protected override void RedoCore() {
			errorBars.SetPlusCore(NewValue);
		}
	}
	#endregion
	#region ErrorBarsValuePropertyChangedHistoryItem
	public class ErrorBarsValuePropertyChangedHistoryItem : SpreadsheetDoubleHistoryItem {
		readonly ErrorBars errorBars;
		public ErrorBarsValuePropertyChangedHistoryItem(DocumentModel documentModel, ErrorBars errorBars, double oldValue, double newValue)
			: base(documentModel, oldValue, newValue) {
			this.errorBars = errorBars;
		}
		protected override void UndoCore() {
			errorBars.SetValueCore(OldValue);
		}
		protected override void RedoCore() {
			errorBars.SetValueCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region TrendlineHistory
	#region TrendlineNamePropertyChangedHistoryItem
	public class TrendlineNamePropertyChangedHistoryItem : SpreadsheetStringHistoryItem {
		readonly Trendline trendline;
		public TrendlineNamePropertyChangedHistoryItem(DocumentModel documentModel, Trendline trendline, string oldValue, string newValue)
			: base(documentModel, oldValue, newValue) {
			this.trendline = trendline;
		}
		protected override void UndoCore() {
			trendline.SetNameCore(OldValue);
		}
		protected override void RedoCore() {
			trendline.SetNameCore(NewValue);
		}
	}
	#endregion
	#endregion
	#region ChartReferenceHistory
	#region ChartDataReferenceFormatCodePropertyChangedHistoryItem
	public class ChartDataFormatCodePropertyChangedHistoryItem : SpreadsheetStringHistoryItem {
		readonly ChartDataReference reference;
		public ChartDataFormatCodePropertyChangedHistoryItem(DocumentModel documentModel, ChartDataReference reference, string oldValue, string newValue)
			: base(documentModel, oldValue, newValue) {
			this.reference = reference;
		}
		protected override void UndoCore() {
			reference.SetFormatCodeCore(OldValue);
		}
		protected override void RedoCore() {
			reference.SetFormatCodeCore(NewValue);
		}
	}
	#endregion
	#endregion
}
