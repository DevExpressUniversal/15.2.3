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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class Diagram : ChartElement, IDiagram, IHitTest, ISupportInitialize, IXtraSerializable {
		readonly HitTestState hitTestState = new HitTestState();
		int labelsResolveOverlappingMinIndent = SeriesLabelBase.DefaultResolveOverlappingMinIndent;
		int axisLabelsResolveOverlappingMinIndent = AxisLabel.DefaultResolveOverlappingMinIndent;
		bool isLabelsResolveOverlappingMinIndentSetted = false;
		bool loading;
		internal Chart Chart { get { return (Chart)base.Owner; } }
		internal ViewController ViewController { get { return Chart.ViewController; } }
		protected internal Rectangle LastBounds { get; set; }
		protected internal virtual bool SupportTooltips { get { return false; } }
		protected internal virtual bool ActualRotated { get { return false; } }
		protected internal virtual bool DependsOnBounds { get { return false; } }
		protected internal virtual bool CanZoomIn { get { return false; } }
		protected internal virtual bool CanZoomInViaRect { get { return CanZoomIn; } }
		protected internal virtual bool CanZoomOut { get { return false; } }
		protected internal virtual bool CanZoomWithTouch { get { return false; } }
		protected internal virtual bool CanScroll { get { return false; } }
		protected internal virtual bool CanPan { get { return false; } }
		protected internal virtual bool HasAutoLayoutElements { get { return false; } }
		protected internal override bool Loading { get { return loading || base.Loading; } }
		protected internal virtual GRealSize2D MinimumLayoutSize { get { return GRealSize2D.Empty; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(SeriesLabelBase.DefaultResolveOverlappingMinIndent),
		XtraSerializableProperty
		]
		public int LabelsResolveOverlappingMinIndent {
			get { return labelsResolveOverlappingMinIndent; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				isLabelsResolveOverlappingMinIndentSetted = true;
				labelsResolveOverlappingMinIndent = value;
				RaiseControlChanged();
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(AxisLabel.DefaultResolveOverlappingMinIndent),
		XtraSerializableProperty
		]
		public int AxisLabelsResolveOverlappingMinIndent {
			get { return axisLabelsResolveOverlappingMinIndent; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				axisLabelsResolveOverlappingMinIndent = value;
				RaiseControlChanged();
			}
		}
		[
		DefaultValue(null),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		Obsolete("This property is obsolete now.")
		]
		public object HiddenObject { get { return null; } set { } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string TypeNameSerializable { get { return GetType().Name; } }
		protected Diagram() : base() {
		}
		#region IDiagram
		GRealRect2D IDiagram.ChartBounds {
			get {
				if (ChartContainer == null || ContainerAdapter.DisplayBounds.IsEmpty)
					return GRealRect2D.Empty;
				Rectangle displayBounds = ContainerAdapter.DisplayBounds;
				return new GRealRect2D(0, 0, displayBounds.Size.Width, displayBounds.Size.Height);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeResolveOverlappingMinIndent() {
			return labelsResolveOverlappingMinIndent != SeriesLabelBase.DefaultResolveOverlappingMinIndent;
		}
		bool ShouldSerializeAxisLabelResolveOverlappingMinIndent() {
			return axisLabelsResolveOverlappingMinIndent != AxisLabel.DefaultResolveOverlappingMinIndent;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() ||
				ShouldSerializeResolveOverlappingMinIndent() ||
				ShouldSerializeAxisLabelResolveOverlappingMinIndent();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "LabelsResolveOverlappingMinIndent")
				return ShouldSerializeResolveOverlappingMinIndent();
			if (propertyName == "AxisLabelsResolveOverlappingMinIndent")
				return ShouldSerializeAxisLabelResolveOverlappingMinIndent();
			return base.XtraShouldSerialize(propertyName);
		}
		void IXtraSerializable.OnStartSerializing() {
		}
		void IXtraSerializable.OnEndSerializing() {
		}
		void IXtraSerializable.OnStartDeserializing(LayoutAllowEventArgs e) {
			((ISupportInitialize)this).BeginInit();
		}
		void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			((ISupportInitialize)this).EndInit();
		}
		#endregion
		#region IHitTest implementation
		object IHitTest.Object { get { return this; } }
		HitTestState IHitTest.State { get { return hitTestState; } }
		#endregion
		#region ISupportIntialaze implementation
		void ISupportInitialize.BeginInit() {
			loading = true;
		}
		void ISupportInitialize.EndInit() {
			loading = false;
		}
		#endregion
		protected virtual void UpdateDiagramAccordingInfo(SeriesBase seriesTemplate, IEnumerable<IRefinedSeries> activeSeries) {
		}
		protected internal virtual void UpdateAutomaticLayout(Rectangle bounds) {
		}
		protected internal abstract bool Contains(object obj);
		protected internal abstract INativeGraphics CreateNativeGraphics(Graphics gr, IntPtr hDC, Rectangle bounds, Rectangle windowsBounds);
		protected internal virtual DiagramViewData CalculateViewData(TextMeasurer textMeasurer, Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList, bool performRangeCorrection) { return null; }
		protected internal virtual void Update(IEnumerable<IRefinedSeries> activeSeries) {
			SeriesBase seriesTemplate = Chart.ShouldUseSeriesTemplate ? Chart.DataContainer.SeriesTemplate : null;
			UpdateDiagramAccordingInfo(seriesTemplate, activeSeries);
		}
		protected internal virtual void OnEndLoading() {
		}
		protected internal virtual void OnUpdateBounds() {
		}
		protected internal virtual void FinishLoading() {
		}
		protected internal virtual IList<ILegendItemData> GetLegendItems() {
			return new ILegendItemData[0];
		}
		protected internal virtual void UpdateDiagramProperties(Diagram diagram) {
		}
		protected internal virtual void ClearAnnotations() {
		}
		protected internal virtual void ClearResolveOverlappingCache() {
		}
		protected internal virtual bool CanDrag(Point point, MouseButtons button) {
			return false;
		}
		protected internal virtual bool PerformDragging(int x, int y, int dx, int dy, ChartScrollEventType scrollEventType, Object focusedElement) {
			return false;
		}
		protected internal virtual bool CanZoom(Point point) {
			return false;
		}
		protected internal virtual Point GetZoomRegionPosition(Point p) {
			return p;
		}
		protected internal virtual void Zoom(int delta, ZoomingKind zoomingKind, Object focusedElement) {
		}
		protected internal virtual void ZoomIn(Rectangle rect) {
		}
		protected internal virtual void ZoomIn(Point center) {
		}
		protected internal virtual void ZoomOut(Point center) {
		}
		protected internal virtual void UndoZoom() {
		}
		protected internal virtual void ClearZoomCache() {
		}
		protected internal virtual void DrawZoomRectangle(Graphics gr, Rectangle rect) {
		}
		protected internal virtual string GetDesignerHint(Point p) {
			return String.Empty;
		}
		protected internal virtual string GetInvisibleDiagramMessage() {
			return String.Empty;
		}
		protected internal virtual CoordinatesConversionCache CreateCoordinatesConversionCache() {
			return null;
		}
		protected internal virtual void BeginGestureZoom(Point center, double zoomDelta) {
		}
		protected internal virtual void PerformGestureZoom(double zoomDelta) {
		}
		protected internal virtual bool PerformGestureRotation(double degreeDelta) {
			return false;
		}
		protected internal virtual void UpdateCrosshairLocation(Point cursorLocation) {
		}
		protected internal virtual List<VisibilityLayoutRegion> GetAutolayoutElements(Rectangle bounds) {
			return null;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			Diagram diagram = obj as Diagram;
			if (diagram != null) {
				labelsResolveOverlappingMinIndent = diagram.labelsResolveOverlappingMinIndent;
				axisLabelsResolveOverlappingMinIndent = diagram.axisLabelsResolveOverlappingMinIndent;
				isLabelsResolveOverlappingMinIndentSetted = diagram.isLabelsResolveOverlappingMinIndentSetted;
			}
		}
	}
}
