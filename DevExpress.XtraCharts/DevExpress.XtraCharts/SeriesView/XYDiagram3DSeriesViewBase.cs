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

using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using System.Collections.Generic;
using System;
namespace DevExpress.XtraCharts {
	public abstract class XYDiagram3DSeriesViewBase : SeriesViewBase, IXYSeriesView, ITransparableView {
		byte opacity;
		Color Color2 {
			get {
				FillStyleBase fillStyle = FillStyleInternal;
				if (fillStyle != null) {
					FillOptionsColor2Base options = fillStyle.Options as FillOptionsColor2Base;
					if (options != null)
						return options.Color2;
				}
				return Color.Empty;
			}
		}
		protected AxisBase AxisY { get { return ((XYDiagram3D)Chart.Diagram).AxisY; } }
		protected abstract int PixelsPerArgument { get; }
		protected virtual FillStyleBase FillStyleInternal { get { return null; } }
		protected virtual byte DefaultOpacity { get { return ConvertBetweenOpacityAndTransparency(0); } }
		protected internal override bool NeedFilterVisiblePoints {
			get {
				return false;
			}
		}
		protected internal override bool DateTimeValuesSupported { get { return true; } }
		protected internal override bool HitTestingSupportedForLegendMarker { get { return false; } }
		protected internal override bool ActualSideMarginsEnabled {
			get {
				Chart chart = Chart;
				if (chart != null) {
					XYDiagram3D diagram = chart.Diagram as XYDiagram3D;
					if (diagram != null)
						return diagram.AxisX.VisualRange.AutoSideMargins;
				}
				return base.ActualSideMarginsEnabled;
			}
		}
		protected override PatternDataProvider GetDataProvider(PatternConstants patternConstant) {
			return GetXYDataProvider(patternConstant);
		}
		protected override bool Is3DView { get { return true; } }
		protected override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.XYView; } }
		protected internal override Color ActualColor {
			get {
				Color actualColor = base.ActualColor;
				return PaletteColorUsed ? ConvertToTransparentColor(actualColor, opacity) : actualColor;
			}
		}
		protected internal override Color ActualColor2 {
			get {
				Color color = Color2;
				return color.IsEmpty ? ConvertToTransparentColor(PaletteEntry.Color2, opacity) : color;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DSeriesViewBaseTransparency"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3DSeriesViewBase.Transparency"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public byte Transparency {
			get { return ConvertBetweenOpacityAndTransparency(opacity); }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				opacity = ConvertBetweenOpacityAndTransparency(value);
				if (!Loading)
					SyncColorsAndTransparency(opacity);
				RaiseControlChanged();
			}
		}
		protected XYDiagram3DSeriesViewBase() : base() {
			opacity = DefaultOpacity;
		}
		#region IXYSeriesView
		bool IXYSeriesView.SideMarginsEnabled { get { return SideMarginsEnabled; } }
		bool IXYSeriesView.CrosshairEnabled { get { return false; } }
		int IXYSeriesView.PixelsPerArgument { get { return PixelsPerArgument; } }
		string IXYSeriesView.CrosshairLabelPattern { get { return string.Empty; } }
		IAxisData IXYSeriesView.AxisXData { get { return GetAxisX(); } }
		IAxisData IXYSeriesView.AxisYData { get { return GetAxisY(); } }
		IPane IXYSeriesView.Pane { get { return (Chart == null) ? null : Chart.Diagram as XYDiagram3D; } }
		ToolTipPointDataToStringConverter IXYSeriesView.CrosshairConverter { get { return null; } }
		IEnumerable<double> IXYSeriesView.GetCrosshairValues(RefinedPoint refinedPoint) {
			return null;
		}
		List<ISeparatePaneIndicator> IXYSeriesView.GetSeparatePaneIndicators() {
			return null;
		}
		List<IAffectsAxisRange> IXYSeriesView.GetIndicatorsAffectRange() {
			return null;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Transparency")
				return ShouldSerializeTransparency();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeTransparency() {
			return opacity != DefaultOpacity;
		}
		void ResetTransparency() {
			Transparency = DefaultOpacity;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeTransparency();
		}
		#endregion
		void ITransparableView.AssignTransparency(ITransparableView view) {
			opacity = ConvertBetweenOpacityAndTransparency(view.Transparency);
		}
		protected abstract DiagramPoint? CalculateAnnotationAnchorPoint(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData, IAxisRangeData axisRangeY);
		protected override void SyncColorsAndTransparency(byte opacity) {
			base.SyncColorsAndTransparency(opacity);
			if (!Color2.IsEmpty) {
				FillStyleBase fillStyle = FillStyleInternal;
				if (fillStyle != null) {
					FillOptionsColor2Base options = fillStyle.Options as FillOptionsColor2Base;
					if (options != null)
						options.SetColor2(Color.FromArgb(opacity, Color2));
				}
			}
		}
		protected override AxisBase GetAxisX() {
			if (Chart != null) {
				XYDiagram3D xyDiagram3D = Chart.Diagram as XYDiagram3D;
				return xyDiagram3D != null ? xyDiagram3D.AxisX : null;
			}
			return null;
		}
		protected override AxisBase GetAxisY() {
			if (Chart != null) {
				XYDiagram3D xyDiagram3D = Chart.Diagram as XYDiagram3D;
				return xyDiagram3D != null ? xyDiagram3D.AxisY : null;
			}
			return null;
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		protected internal abstract XYDiagram3DWholeSeriesLayout CalculateWholeSeriesLayout(XYDiagram3DCoordsCalculator coordsCalculator, SeriesLayout seriesLayout);
		protected internal virtual void FillPrimitivesContainer(XYDiagram3DWholeSeriesLayout layout, PrimitivesContainer container) {
		}
		protected internal virtual double GetSeriesDepth() {
			return double.NegativeInfinity;
		}
		protected internal virtual int GetSeriesDepthFixed() {
			return 0;
		}
		protected internal virtual SeriesPointLayout CalculateSeriesPointLayout(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData) {
			return null;
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
		}
		protected internal override void FillPrimitivesContainer(SeriesPointLayout pointLayout, PrimitivesContainer container) {
			View3DSeriesPointLayout layout = pointLayout as View3DSeriesPointLayout;
			if (layout != null)
				container.Add(layout.Lines, layout.Polygons);
		}
		protected internal override Color GetPointColor(int pointIndex, int pointsCount) {
			return Color.FromArgb(opacity, base.GetPointColor(pointIndex, pointsCount));
		}
		protected internal override Color GetPointColor2(int pointIndex, int pointsCount) {
			return Color.FromArgb(opacity, base.GetPointColor2(pointIndex, pointsCount));
		}
		protected internal void CalculateAnnotationsAnchorPointsLayout(XYDiagram3DAnchorPointLayoutList anchorPointLayoutList) {
			foreach (RefinedPointData pointData in anchorPointLayoutList.SeriesData) {
				SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(pointData.SeriesPoint);
				if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
					DiagramPoint? point = CalculateAnnotationAnchorPoint(anchorPointLayoutList.CoordsCalculator, pointData, anchorPointLayoutList.AxisRangeY);
					if (point.HasValue)
						foreach (Annotation annotation in seriesPoint.Annotations) {
							if (annotation.ScrollingSupported || anchorPointLayoutList.CoordsCalculator.Bounds.Contains((Point)point.Value))
								anchorPointLayoutList.Add(new AnnotationLayout(annotation, point.Value, pointData.RefinedPoint));
						}
				}
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ITransparableView view = obj as ITransparableView;
			if (view != null)
				((ITransparableView)this).AssignTransparency(view);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			XYDiagram3DSeriesViewBase view = (XYDiagram3DSeriesViewBase)obj;
			return opacity.Equals(view.opacity);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public abstract class SeriesView3DColorEachSupportBase : XYDiagram3DSeriesViewBase, IColorEachSupportView {
		const bool DefaultColorEach = false;
		bool colorEach = DefaultColorEach;
		protected internal override bool ActualColorEach { get { return ColorEach; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesView3DColorEachSupportBaseColorEach"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesView3DColorEachSupportBase.ColorEach"),
		TypeConverter(typeof(BooleanTypeConverter)),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool ColorEach {
			get { return this.colorEach; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				this.colorEach = value;
				RaiseControlChanged();
			}
		}
		protected SeriesView3DColorEachSupportBase()
			: base() {
		}
		#region XtraSerializing 
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "ColorEach")
				return ShouldSerializeColorEach();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeColorEach() {
			return this.colorEach != DefaultColorEach;
		}
		void ResetColorEach() {
			ColorEach = DefaultColorEach;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeColorEach();
		}
		#endregion
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			IColorEachSupportView view = obj as IColorEachSupportView;
			if (view != null)
				colorEach = view.ColorEach;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			SeriesView3DColorEachSupportBase view = (SeriesView3DColorEachSupportBase)obj;
			return this.colorEach == view.colorEach;
		}
	}
}
