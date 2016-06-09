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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region OfPieSplitType
	public enum OfPieSplitType {
		Auto,
		Custom,
		Percent,
		Position,
		Value
	}
	#endregion
	#region SecondPiePointCollection
	public class SecondPiePointCollection : ChartUndoableCollectionSupportsCopyFrom<int> {
		public SecondPiePointCollection(IChart parent) 
			: base(parent) { 
		}
		protected override int CreateNewItem(int source) {
			return source;
		}
	}
	#endregion
	#region OfPieChartView
	public class OfPieChartView : ChartViewWithVaryColors, IChartViewWithGapWidth, ISupportsSeriesLines {
		#region Fields
		int gapWidth = 150;
		int secondPieSize = 75;
		SeriesLinesCollection seriesLines;
		double splitPos = 1.0;
		OfPieSplitType splitType = OfPieSplitType.Auto;
		SecondPiePointCollection secondPiePoints;
		#endregion
		public OfPieChartView(IChart parent)
			: base(parent) {
			this.seriesLines = new SeriesLinesCollection(parent);
			this.secondPiePoints = new SecondPiePointCollection(parent);
		}
		#region Properties
		#region GapWidth
		public int GapWidth {
			get { return gapWidth; }
			set {
				ValueChecker.CheckValue(value, 0, 500, "GapWidth");
				if(gapWidth == value)
					return;
				SetGapWidth(value);
			}
		}
		void SetGapWidth(int value) {
			ChartViewGapWidthPropertyChangedHistoryItem historyItem = new ChartViewGapWidthPropertyChangedHistoryItem(DocumentModel, this, gapWidth, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetGapWidthCore(int value) {
			this.gapWidth = value;
			Parent.Invalidate();
		}
		#endregion
		#region OfPieType
		public ChartOfPieType OfPieType {
			get { return Info.OfPieType; }
			set {
				if(OfPieType == value)
					return;
				SetPropertyValue(SetOfPieTypeCore, value);
			}
		}
		DocumentModelChangeActions SetOfPieTypeCore(ChartViewInfo info, ChartOfPieType value) {
			info.OfPieType = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region SecondPieSize
		public int SecondPieSize {
			get { return secondPieSize; }
			set {
				ValueChecker.CheckValue(value, 5, 200, "SecondPieSize");
				if(secondPieSize == value)
					return;
				SetSecondPieSize(value);
			}
		}
		void SetSecondPieSize(int value) {
			OfPieSecondPieSizePropertyChangedHistoryItem historyItem = new OfPieSecondPieSizePropertyChangedHistoryItem(DocumentModel, this, secondPieSize, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetSecondPieSizeCore(int value) {
			this.secondPieSize = value;
			Parent.Invalidate();
		}
		#endregion
		public bool IsSeriesLinesApplicable { get { return true; } }
		public SeriesLinesCollection SeriesLines { get { return seriesLines; } }
		public SecondPiePointCollection SecondPiePoints { get { return secondPiePoints; } }
		#region SplitPos
		public double SplitPos {
			get { return splitPos; }
			set {
				if(splitPos == value)
					return;
				SetSplitPos(value);
			}
		}
		void SetSplitPos(double value) {
			OfPieSplitPosPropertyChangedHistoryItem historyItem = new OfPieSplitPosPropertyChangedHistoryItem(DocumentModel, this, splitPos, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetSplitPosCore(double value) {
			this.splitPos = value;
			Parent.Invalidate();
		}
		#endregion
		#region SplitType
		public OfPieSplitType SplitType {
			get { return splitType; }
			set {
				if(splitType == value)
					return;
				SetSplitType(value);
			}
		}
		void SetSplitType(OfPieSplitType value) {
			OfPieSplitTypePropertyChangedHistoryItem historyItem = new OfPieSplitTypePropertyChangedHistoryItem(DocumentModel, this, splitType, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetSplitTypeCore(OfPieSplitType value) {
			this.splitType = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.OfPie; } }
		public override ChartType ChartType { get { return OfPieType == ChartOfPieType.Bar ? ChartType.BarOfPie : ChartType.PieOfPie; } }
		public override AxisGroupType AxesType { get { return AxisGroupType.Empty; } }
		public override bool IsSingleSeriesView { get { return true; } }
		public override IChartView CloneTo(IChart parent) {
			OfPieChartView result = new OfPieChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override DataLabelPosition DefaultDataLabelPosition {
			get { return DataLabelPosition.BestFit; }
		}
		public override ISeries CreateSeriesInstance() {
			return new PieSeries(this);
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			CopyGapWidth(value);
			ISupportsSeriesLines viewWithSeriesLines = value as ISupportsSeriesLines;
			if (viewWithSeriesLines != null)
				seriesLines.CopyFrom(viewWithSeriesLines.SeriesLines);
			OfPieChartView view = value as OfPieChartView;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(OfPieChartView value) {
			SecondPieSize = value.SecondPieSize;
			SplitPos = value.SplitPos;
			SplitType = value.SplitType;
			secondPiePoints.CopyFrom(value.secondPiePoints);
		}
		void CopyGapWidth(IChartView value) {
			IChartViewWithGapWidth viewWithGapWidth = value as IChartViewWithGapWidth;
			if (viewWithGapWidth != null)
				GapWidth = viewWithGapWidth.GapWidth;
			else {
				ISupportsUpDownBars viewWithUpDownBars = value as ISupportsUpDownBars;
				if (viewWithUpDownBars != null)
					CopyGapWidthCore(viewWithUpDownBars);
			}
		}
		void CopyGapWidthCore(ISupportsUpDownBars value) {
			if (value.UpDownBars != null && value.ShowUpDownBars)
				GapWidth = value.UpDownBars.GapWidth;
		}
		public override void ResetToStyle() {
			base.ResetToStyle();
			foreach (ShapeProperties seriesLine in SeriesLines)
				seriesLine.ResetToStyle();
		}
	}
	#endregion
}
