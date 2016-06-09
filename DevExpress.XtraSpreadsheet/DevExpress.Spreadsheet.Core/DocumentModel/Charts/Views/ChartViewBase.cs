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

using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region BarDirection
	public enum BarChartDirection {
		Column = 0,
		Bar
	}
	#endregion
	#region ChartGrouping
	public enum ChartGrouping {
		Standard = 0,
		Stacked,
		PercentStacked
	}
	#endregion
	#region BarChartGrouping
	public enum BarChartGrouping {
		Standard = 0,
		Stacked,
		PercentStacked,
		Clustered
	}
	#endregion
	#region ChartOfPieType
	public enum ChartOfPieType {
		Pie = 0,
		Bar
	}
	#endregion
	#region SizeRepresentsType
	public enum SizeRepresentsType {
		Area = 0,
		Width
	}
	#endregion
	#region ChartViewInfo
	public class ChartViewInfo : ICloneable<ChartViewInfo>, ISupportsCopyFrom<ChartViewInfo>, ISupportsSizeOf {
		#region Fields
		const int offsetGrouping = 4;
		const int offsetBarGrouping = 6;
		const uint maskBarDirection = 0x0001;
		const uint maskBubble3D = 0x0002;
		const uint maskDropLines = 0x0004;
		const uint maskHiLowLines = 0x0008;
		const uint maskGrouping = 0x0030;
		const uint maskBarGrouping = 0x00c0;
		const uint maskOfPieType = 0x0100;
		const uint maskShowNegBubbles = 0x0200;
		const uint maskSizeRepresents = 0x0400;
		const uint maskSmooth = 0x0800;
		const uint maskVaryColors = 0x1000;
		const uint maskWireframe = 0x2000;
		const uint maskShowMarker = 0x4000;
		const uint maskShowUpDownBars = 0x8000;
		uint packedValues = 0x62c2;
		#endregion
		#region Properties
		public BarChartDirection BarDirection {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskBarDirection) ? BarChartDirection.Bar : BarChartDirection.Column; }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskBarDirection, value == BarChartDirection.Bar); }
		}
		public bool Bubble3D {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskBubble3D); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskBubble3D, value); }
		}
		public bool ShowDropLines {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskDropLines); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskDropLines, value); }
		}
		public bool ShowHiLowLines {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskHiLowLines); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskHiLowLines, value); }
		}
		public ChartGrouping Grouping {
			get { return (ChartGrouping)PackedValues.GetIntBitValue(this.packedValues, maskGrouping, offsetGrouping); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskGrouping, offsetGrouping, (int)value); }
		}
		public BarChartGrouping BarGrouping {
			get { return (BarChartGrouping)PackedValues.GetIntBitValue(this.packedValues, maskBarGrouping, offsetBarGrouping); }
			set { PackedValues.SetIntBitValue(ref this.packedValues, maskBarGrouping, offsetBarGrouping, (int)value); }
		}
		public ChartOfPieType OfPieType {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskOfPieType) ? ChartOfPieType.Bar : ChartOfPieType.Pie; }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskOfPieType, value == ChartOfPieType.Bar); }
		}
		public bool ShowNegBubbles {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowNegBubbles); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowNegBubbles, value); }
		}
		public SizeRepresentsType SizeRepresents {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskSizeRepresents) ? SizeRepresentsType.Width : SizeRepresentsType.Area; }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskSizeRepresents, value == SizeRepresentsType.Width); }
		}
		public bool Smooth {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskSmooth); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskSmooth, value); }
		}
		public bool VaryColors {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskVaryColors); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskVaryColors, value); }
		}
		public bool Wireframe {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskWireframe); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskWireframe, value); }
		}
		public bool ShowMarker {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowMarker); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowMarker, value); }
		}
		public bool ShowUpDownBars {
			get { return PackedValues.GetBoolBitValue(this.packedValues, maskShowUpDownBars); }
			set { PackedValues.SetBoolBitValue(ref this.packedValues, maskShowUpDownBars, value); }
		}
		#endregion
		#region ICloneable<ChartViewInfo> Members
		public ChartViewInfo Clone() {
			ChartViewInfo result = new ChartViewInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<ChartViewInfo> Members
		public void CopyFrom(ChartViewInfo value) {
			Guard.ArgumentNotNull(value, "value");
			this.packedValues = value.packedValues;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			ChartViewInfo other = obj as ChartViewInfo;
			if(other == null)
				return false;
			return this.packedValues == other.packedValues;
		}
		public override int GetHashCode() {
			return (int)this.packedValues;
		}
	}
	#endregion
	#region ChartViewInfoCache
	public class ChartViewInfoCache : UniqueItemsCache<ChartViewInfo> {
		public ChartViewInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override ChartViewInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new ChartViewInfo();
		}
	}
	#endregion
	#region ChartViewBase
	public abstract class ChartViewBase : SpreadsheetUndoableIndexBasedObject<ChartViewInfo>, IChartView {
		#region Fields
		IChart parent;
		AxisGroup axes;
		SeriesCollection series;
		#endregion
		protected ChartViewBase(IChart parent)
			: base(parent.DocumentModel) {
			this.parent = parent;
			this.axes = null;
			this.series = new SeriesCollection(parent);
		}
		public IChart Parent { get { return parent; } }
		#region IChartView Members
		#region Axes
		public AxisGroup Axes {
			get { return axes; }
			set {
				if(object.ReferenceEquals(axes, value))
					return;
				SetAxes(value);
			}
		}
		void SetAxes(AxisGroup value) {
			ChartViewAxesPropertyChangedHistoryItem historyItem = new ChartViewAxesPropertyChangedHistoryItem(DocumentModel, this, axes, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetAxesCore(AxisGroup value) {
			this.axes = value;
			Parent.Invalidate();
		}
		#endregion
		public SeriesCollection Series { get { return series; } }
		public abstract ChartViewType ViewType { get; }
		public abstract ChartType ChartType { get; }
		public virtual bool Is3DView { get { return false; } }
		public virtual bool IsSingleSeriesView { get { return false; } }
		public abstract AxisGroupType AxesType { get; }
		public abstract IChartView CloneTo(IChart parent);
		public abstract void Visit(IChartViewVisitor visitor);
		public ChartViewSeriesDirection SeriesDirection { get { return parent.SeriesDirection; } }
		public virtual DataLabelPosition DefaultDataLabelPosition { get { return DataLabelPosition.Center; } }
		public virtual void ResetToStyle() {
			foreach (ISeries series in Series)
				series.ResetToStyle();
		}
		protected internal virtual IChartView Duplicate() {
			return null;
		}
		public abstract ISeries CreateSeriesInstance();
		public bool IsContained { get { return Parent.Views.Contains(this); } }
		public int IndexOfView { get { return Parent.Views.IndexOf(this); } }
		#endregion
		#region SpreadsheetUndoableIndexBasedObject members
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override UniqueItemsCache<ChartViewInfo> GetCache(IDocumentModel documentModel) {
			return DocumentModel.Cache.ChartViewInfoCache;
		}
		protected override void ApplyChanges(DocumentModelChangeActions changeActions) {
			DocumentModel.ApplyChanges(changeActions);
			Parent.Invalidate();
		}
		#endregion
		public void CopyFrom(IChartView value) {
			CopyFrom(value, true);
		}
		public void CopyFromWithoutSeries(IChartView value) {
			CopyFrom(value, false);
		}
		protected virtual void CopyFrom(IChartView value, bool copySeries) {
			Guard.ArgumentNotNull(value, "value");
			ChartViewBase view = value as ChartViewBase;
			base.CopyFrom(view);
			if (view != null)
				CopySeries(view, copySeries);
		}
		void CopySeries(ChartViewBase value, bool copySeries) {
			this.series.Clear();
			if (copySeries) {
				for (int i = 0; i < value.series.Count; i++) {
					ISeries item = value.series[i].CloneTo(this);
					this.series.Add(item);
				}
			}
		}
		#region Notifications
		public virtual void OnRangeInserting(InsertRangeNotificationContext context) {
			series.OnRangeInserting(context);
		}
		public virtual void OnRangeRemoving(RemoveRangeNotificationContext context) {
			series.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region ChartViewWithDataLabels
	public abstract class ChartViewWithDataLabels : ChartViewBase {
		#region Fields
		readonly DataLabels dataLabels;
		#endregion
		protected ChartViewWithDataLabels(IChart parent)
			: base(parent) {
			this.dataLabels = new DataLabels(parent);
			this.dataLabels.BeginInit();
			try {
				this.dataLabels.Apply = true;
			}
			finally {
				this.dataLabels.EndInit();
			}
		}
		#region Properties
		public DataLabels DataLabels { get { return dataLabels; } }
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			ChartViewWithDataLabels view = value as ChartViewWithDataLabels;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(ChartViewWithDataLabels value) {
			dataLabels.CopyFrom(value.dataLabels);
		}
		public override void ResetToStyle() {
			base.ResetToStyle();
			DataLabels.ResetToStyle();
		}
		#region Notifications
		public override void OnRangeInserting(InsertRangeNotificationContext context) {
			base.OnRangeInserting(context);
			DataLabels.OnRangeInserting(context);
		}
		public override void OnRangeRemoving(RemoveRangeNotificationContext context) {
			base.OnRangeRemoving(context);
			DataLabels.OnRangeRemoving(context);
		}
		#endregion
	}
	#endregion
	#region ChartViewWithVaryColors
	public abstract class ChartViewWithVaryColors : ChartViewWithDataLabels {
		protected ChartViewWithVaryColors(IChart parent)
			: base(parent) {
		}
		#region Properties
		#region VaryColors
		public bool VaryColors {
			get { return Info.VaryColors; }
			set {
				if(VaryColors == value)
					return;
				SetPropertyValue(SetVaryColorsCore, value);
			}
		}
		DocumentModelChangeActions SetVaryColorsCore(ChartViewInfo info, bool value) {
			info.VaryColors = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#endregion
	}
	#endregion
	#region ChartViewWithGroupingAndDropLines
	public abstract class ChartViewWithGroupingAndDropLines : ChartViewWithVaryColors, ISupportsDropLines {
		ShapeProperties dropLinesProperties;
		protected ChartViewWithGroupingAndDropLines(IChart parent)
			: base(parent) {
				this.dropLinesProperties = new ShapeProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		#region Grouping
		public ChartGrouping Grouping {
			get { return Info.Grouping; }
			set {
				if(Grouping == value)
					return;
				SetPropertyValue(SetGroupingCore, value);
			}
		}
		DocumentModelChangeActions SetGroupingCore(ChartViewInfo info, ChartGrouping value) {
			info.Grouping = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		#region ShowDropLines
		public bool ShowDropLines {
			get { return Info.ShowDropLines; }
			set {
				if(ShowDropLines == value)
					return;
				SetPropertyValue(SetShowDropLinesCore, value);
			}
		}
		DocumentModelChangeActions SetShowDropLinesCore(ChartViewInfo info, bool value) {
			info.ShowDropLines = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public ShapeProperties DropLinesProperties { get { return dropLinesProperties; } }
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			ISupportsDropLines view = value as ISupportsDropLines;
			if (view != null)
				dropLinesProperties.CopyFrom(view.DropLinesProperties);
		}
		public override void ResetToStyle() {
			base.ResetToStyle();
			DropLinesProperties.ResetToStyle();
		}
	}
	#endregion
	#region ChartViewWithSlice
	public abstract class ChartViewWithSlice : ChartViewWithVaryColors {
		#region Fields
		int firstSliceAngle = 0;
		#endregion
		protected ChartViewWithSlice(IChart parent)
			: base(parent) {
		}
		#region Properties
		public int FirstSliceAngle {
			get { return firstSliceAngle; }
			set {
				ValueChecker.CheckValue(value, 0, 360, "FirstSliceAngle");
				if(firstSliceAngle == value)
					return;
				SetFirstSliceAngle(value);
			}
		}
		void SetFirstSliceAngle(int value) {
			ChartViewFirstSliceAnglePropertyChangedHistoryItem historyItem = new ChartViewFirstSliceAnglePropertyChangedHistoryItem(DocumentModel, this, firstSliceAngle, value);
			DocumentModel.History.Add(historyItem);
			historyItem.Execute();
		}
		protected internal void SetFirstSliceAngleCore(int value) {
			this.firstSliceAngle = value;
			Parent.Invalidate();
		}
		protected internal bool Exploded { get; set; }
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			ChartViewWithSlice view = value as ChartViewWithSlice;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(ChartViewWithSlice value) {
			FirstSliceAngle = value.FirstSliceAngle;
		}
	}
	#endregion
	#region ChartUpDownBars
	public class ChartUpDownBars : ISupportsCopyFrom<ChartUpDownBars>, IChartViewWithGapWidth {
		readonly IChart parent;
		int gapWidth = 150;
		ShapeProperties downBarsProperties;
		ShapeProperties upBarsProperties;
		public ChartUpDownBars(IChart parent) {
			this.parent = parent;
			this.downBarsProperties = new ShapeProperties(DocumentModel) { Parent = parent };
			this.upBarsProperties = new ShapeProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
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
		public ShapeProperties DownBarsProperties { get { return downBarsProperties; } }
		public ShapeProperties UpBarsProperties { get { return upBarsProperties; } }
		#endregion
		#region ISupportsCopyFrom<UpDownBars> Members
		public void CopyFrom(ChartUpDownBars value) {
			Guard.ArgumentNotNull(value, "value");
			GapWidth = value.GapWidth;
			downBarsProperties.CopyFrom(value.downBarsProperties);
			upBarsProperties.CopyFrom(value.upBarsProperties);
		}
		#endregion
		public void ResetToStyle() {
			DownBarsProperties.ResetToStyle();
			UpBarsProperties.ResetToStyle();
		}
	}
	#endregion
}
