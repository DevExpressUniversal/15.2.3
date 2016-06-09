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
using System.Diagnostics;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region BarChartViewBase
	public abstract class BarChartViewBase : ChartViewWithVaryColors, IChartViewWithGapWidth {
		#region Fields
		int gapWidth = 150;
		#endregion
		protected BarChartViewBase(IChart parent)
			: base(parent) {
		}
		#region Properties
		#region BarDirection
		public BarChartDirection BarDirection {
			get { return Info.BarDirection; }
			set {
				if (BarDirection == value)
					return;
				SetPropertyValue(SetBarDirectionCore, value);
			}
		}
		DocumentModelChangeActions SetBarDirectionCore(ChartViewInfo info, BarChartDirection value) {
			info.BarDirection = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region GapWidth
		public int GapWidth {
			get { return gapWidth; }
			set {
				ValueChecker.CheckValue(value, 0, 500, "GapWidth");
				if (gapWidth == value)
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
		#region Grouping
		public BarChartGrouping Grouping {
			get { return Info.BarGrouping; }
			set {
				if (Grouping == value)
					return;
				SetPropertyValue(SetGroupingCore, value);
			}
		}
		DocumentModelChangeActions SetGroupingCore(ChartViewInfo info, BarChartGrouping value) {
			info.BarGrouping = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
		#region IChartView Members
		public override DataLabelPosition DefaultDataLabelPosition {
			get {
				if (Grouping == BarChartGrouping.PercentStacked || Grouping == BarChartGrouping.Stacked)
					return DataLabelPosition.Center;
				return DataLabelPosition.OutsideEnd;
			}
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			CopyGapWidth(value);
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
	}
	#endregion
	#region SeriesLinesCollection
	public class SeriesLinesCollection : ChartUndoableCollection<ShapeProperties>, ISupportsCopyFrom<SeriesLinesCollection> {
		public SeriesLinesCollection(IChart parent)
			: base(parent) {
		}
		public void CopyFrom(SeriesLinesCollection value) {
			Clear();
			int count = value.Count;
			for (int i = 0; i < count; i++) {
				ShapeProperties item = new ShapeProperties(Parent.DocumentModel);
				item.Parent = Parent;
				item.CopyFrom(value[i]);
				Add(item);
			}
		}
	}
	#endregion
	#region BarChartView
	public class BarChartView : BarChartViewBase, ISupportsSeriesLines {
		#region Fields
		int overlap = 0;
		SeriesLinesCollection seriesLines;
		#endregion
		public BarChartView(IChart parent)
			: base(parent) {
			this.seriesLines = new SeriesLinesCollection(parent);
		}
		#region Properties
		public override AxisGroupType AxesType { get { return AxisGroupType.CategoryValue; } }
		#region Overlap
		public int Overlap {
			get { return overlap; }
			set {
				ValueChecker.CheckValue(value, -100, 100, "Overlap");
				if (overlap == value)
					return;
				SetOverlap(value);
			}
		}
		void SetOverlap(int value) {
			BarChartOverlapPropertyChangedHistoryItem historyItem = new BarChartOverlapPropertyChangedHistoryItem(DocumentModel, this, overlap, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetOverlapCore(int value) {
			this.overlap = value;
			Parent.Invalidate();
		}
		#endregion
		public bool IsSeriesLinesApplicable { get { return Grouping == BarChartGrouping.Stacked || Grouping == BarChartGrouping.PercentStacked; } }
		public SeriesLinesCollection SeriesLines { get { return seriesLines; } }
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Bar; } }
		public override ChartType ChartType { 
			get {
				bool isBarDirection = BarDirection == BarChartDirection.Bar;
				System.Diagnostics.Debug.Assert(Grouping != BarChartGrouping.Standard);
				if (Grouping == BarChartGrouping.Clustered)
					return isBarDirection ? ChartType.BarClustered : ChartType.ColumnClustered;
				if (Grouping == BarChartGrouping.Stacked)
					return isBarDirection ? ChartType.BarStacked : ChartType.ColumnStacked;
				return isBarDirection ? ChartType.BarFullStacked : ChartType.ColumnFullStacked;
			} 
		}
		public override IChartView CloneTo(IChart parent) {
			BarChartView result = new BarChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		protected internal override IChartView Duplicate() {
			BarChartView result = new BarChartView(Parent);
			result.CopyFromWithoutSeries(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override ISeries CreateSeriesInstance() {
			return new BarSeries(this);
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			ISupportsSeriesLines viewWithSeriesLines = value as ISupportsSeriesLines;
			if (viewWithSeriesLines != null) {
				CopyFromCore(viewWithSeriesLines);
				BarChartView view = value as BarChartView;
				if (view != null)
					CopyFromCore(view);
			}
		}
		void CopyFromCore(ISupportsSeriesLines value) {
			seriesLines.CopyFrom(value.SeriesLines);
		}
		void CopyFromCore(BarChartView value) {
			Overlap = value.Overlap;
		}
		public override void ResetToStyle() {
			base.ResetToStyle();
			foreach (ShapeProperties seriesLine in SeriesLines)
				seriesLine.ResetToStyle();
		}
	}
	#endregion
	#region Bar3DChartView
	public class Bar3DChartView : BarChartViewBase, IChartViewWithGapDepth {
		#region Fields
		int gapDepth = 150;
		BarShape shape = BarShape.Box;
		#endregion
		public Bar3DChartView(IChart parent)
			: base(parent) {
		}
		#region Properties
		#region GapDepth
		public int GapDepth {
			get { return gapDepth; }
			set {
				ValueChecker.CheckValue(value, 0, 500, "GapDepth");
				if (gapDepth == value)
					return;
				SetGapDepth(value);
			}
		}
		void SetGapDepth(int value) {
			ChartViewGapDepthPropertyChangedHistoryItem historyItem = new ChartViewGapDepthPropertyChangedHistoryItem(DocumentModel, this, gapDepth, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		public void SetGapDepthCore(int value) {
			this.gapDepth = value;
			Parent.Invalidate();
		}
		#endregion
		#region Shape
		public BarShape Shape {
			get { return shape; }
			set {
				if (shape == value)
					return;
				SetShape(value);
			}
		}
		void SetShape(BarShape value) {
			BarChartShapePropertyChangedHistoryItem historyItem = new BarChartShapePropertyChangedHistoryItem(DocumentModel, this, shape, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetShapeCore(BarShape value) {
			this.shape = value;
			Parent.Invalidate();
		}
		#endregion
		#endregion
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Bar3D; } }
		public override ChartType ChartType { get { return GetChartType(Shape); } }
		public override bool Is3DView { get { return true; } }
		public override AxisGroupType AxesType { get { return Grouping == BarChartGrouping.Standard ? AxisGroupType.CategoryValueSeries : AxisGroupType.CategoryValue; } }
		public override IChartView CloneTo(IChart parent) {
			Bar3DChartView result = new Bar3DChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override ISeries CreateSeriesInstance() {
			return new BarSeries(this);
		}
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			IChartViewWithGapDepth viewWithGapDepth = value as IChartViewWithGapDepth;
			if (viewWithGapDepth != null) {
				CopyFromCore(viewWithGapDepth);
				Bar3DChartView view = value as Bar3DChartView;
				if (view != null)
					CopyFromCore(view);
			}
		}
		void CopyFromCore(IChartViewWithGapDepth value) {
			GapDepth = value.GapDepth;
		}
		void CopyFromCore(Bar3DChartView value) {
			Shape = value.Shape;
		}
		#region GetChartType
		internal Model.ChartType GetChartType(BarShape shape) {
			return BarDirection == BarChartDirection.Bar ? GetBar3DChartType(Grouping, shape) : GetColumn3DChartType(Grouping, shape);
		}
		ChartType GetColumn3DChartType(BarChartGrouping grouping, BarShape shape) {
			if (shape == BarShape.Box || shape == BarShape.Auto)
				return GetColumn3DChartTypeWithBoxShape(grouping);
			if (shape == BarShape.Cone || shape == BarShape.ConeToMax)
				return GetColumn3DChartTypeWithConeShape(grouping);
			if (shape == BarShape.Pyramid || shape == BarShape.PyramidToMax)
				return GetColumn3DChartTypeWithPyramidShape(grouping);
			return GetColumn3DChartTypeWithCylinderShape(grouping);
		}
		ChartType GetBar3DChartType(BarChartGrouping grouping, BarShape shape) {
			System.Diagnostics.Debug.Assert(grouping != BarChartGrouping.Standard);
			if (shape == BarShape.Auto || shape == BarShape.Box)
				return GetBar3DChartTypeWithBoxShape(grouping);
			if (shape == BarShape.Cone || shape == BarShape.ConeToMax)
				return GetBar3DChartTypeWithConeShape(grouping);
			if (shape == BarShape.Pyramid || shape == BarShape.PyramidToMax)
				return GetBar3DChartTypeWithPyramidShape(grouping);
			return GetBar3DChartTypeWithCylinderShape(grouping);
		}
		ChartType GetBar3DChartTypeWithBoxShape(BarChartGrouping grouping) {
			if (grouping == BarChartGrouping.Clustered)
				return ChartType.Bar3DClustered;
			if (grouping == BarChartGrouping.Stacked)
				return ChartType.Bar3DStacked;
			return ChartType.Bar3DFullStacked;
		}
		ChartType GetBar3DChartTypeWithConeShape(BarChartGrouping grouping) {
			if (grouping == BarChartGrouping.Stacked)
				return ChartType.Bar3DStackedCone;
			if (grouping == BarChartGrouping.Clustered)
				return ChartType.Bar3DClusteredCone;
			return ChartType.Bar3DFullStackedCone;
		}
		ChartType GetBar3DChartTypeWithPyramidShape(BarChartGrouping grouping) {
			if (grouping == BarChartGrouping.Stacked)
				return ChartType.Bar3DStackedPyramid;
			if (grouping == BarChartGrouping.Clustered)
				return ChartType.Bar3DClusteredPyramid;
			return ChartType.Bar3DFullStackedPyramid;
		}
		ChartType GetBar3DChartTypeWithCylinderShape(BarChartGrouping grouping) {
			if (grouping == BarChartGrouping.Stacked)
				return ChartType.Bar3DStackedCylinder;
			if (grouping == BarChartGrouping.Clustered)
				return ChartType.Bar3DClusteredCylinder;
			return ChartType.Bar3DFullStackedCylinder;
		}
		ChartType GetColumn3DChartTypeWithBoxShape(BarChartGrouping grouping) {
			if (grouping == BarChartGrouping.Clustered)
				return ChartType.Column3DClustered;
			if (grouping == BarChartGrouping.Stacked)
				return ChartType.Column3DStacked;
			if (grouping == BarChartGrouping.PercentStacked)
				return ChartType.Column3DFullStacked;
			return ChartType.Column3DStandard;
		}
		ChartType GetColumn3DChartTypeWithConeShape(BarChartGrouping grouping) {
			if (grouping == BarChartGrouping.Stacked)
				return ChartType.Column3DStackedCone;
			if (grouping == BarChartGrouping.Clustered)
				return ChartType.Column3DClusteredCone;
			if (grouping == BarChartGrouping.PercentStacked)
				return ChartType.Column3DFullStackedCone;
			return ChartType.Column3DStandardCone;
		}
		ChartType GetColumn3DChartTypeWithPyramidShape(BarChartGrouping grouping) {
			if (grouping == BarChartGrouping.Stacked)
				return ChartType.Column3DStackedPyramid;
			if (grouping == BarChartGrouping.Clustered)
				return ChartType.Column3DClusteredPyramid;
			if (grouping == BarChartGrouping.PercentStacked)
				return ChartType.Column3DFullStackedPyramid;
			return ChartType.Column3DStandardPyramid;
		}
		ChartType GetColumn3DChartTypeWithCylinderShape(BarChartGrouping grouping) {
			if (grouping == BarChartGrouping.Stacked)
				return ChartType.Column3DStackedCylinder;
			if (grouping == BarChartGrouping.Clustered)
				return ChartType.Column3DClusteredCylinder;
			if (grouping == BarChartGrouping.PercentStacked)
				return ChartType.Column3DFullStackedCylinder;
			return ChartType.Column3DStandardCylinder;
		}
		#endregion
	}
	#endregion
}
