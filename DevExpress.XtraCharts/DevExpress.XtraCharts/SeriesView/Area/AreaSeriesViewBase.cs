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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class AreaSeriesViewBase : LineSeriesView, IAreaSeriesView {
		const int MaxMarkerInLegendSize = 20;
		byte opacity;
		PolygonFillStyle fillStyle;
		CustomBorder border;
		internal PolygonFillStyle ActualFillStyle {
			get { return fillStyle.FillMode == FillMode.Empty ? Appearance.FillStyle : fillStyle; }
		}
		protected virtual Marker ActualMarkerOptions { get { return MarkerOptions; } }
		protected virtual CustomBorder ActualBorder { get { return Border; } }
		protected internal virtual byte DefaultOpacity { get { return ConvertBetweenOpacityAndTransparency((byte)135); } }
		protected internal override bool SideMarginsEnabled { get { return false; } }
		protected internal override Color ActualColor {
			get {
				Color actualColor = base.ActualColor;
				if (PaletteColorUsed)
					actualColor = ConvertToTransparentColor(actualColor, opacity);
				return actualColor;
			}
		}
		protected internal override Color ActualColor2 {
			get {
				Color color = Color2;
				return color.IsEmpty ? ConvertToTransparentColor(PaletteEntry.Color2, opacity) : color;
			}
		}
		Color Color2 {
			get {
				FillOptionsColor2Base options = fillStyle.Options as FillOptionsColor2Base;
				return options == null ? Color.Empty : options.Color2;
			}
			set {
				FillOptionsColor2Base options = fillStyle.Options as FillOptionsColor2Base;
				if (options != null)
					options.SetColor2(value);
			}
		}
		AreaSeriesViewAppearance Appearance {
			get {
				IChartAppearance actualAppearance = CommonUtils.GetActualAppearance(this);
				return actualAppearance.AreaSeriesViewAppearance;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AreaSeriesViewBaseBorder"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AreaSeriesViewBase.Border"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public CustomBorder Border { get { return border; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AreaSeriesViewBaseFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AreaSeriesViewBase.FillStyle"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public PolygonFillStyle FillStyle { get { return fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AreaSeriesViewBaseMarkerOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AreaSeriesViewBase.MarkerOptions"),
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Marker MarkerOptions { get { return base.LineMarkerOptions; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new LineStyle LineStyle { get { return null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Marker LineMarkerOptions { get { return null; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("AreaSeriesViewBaseTransparency"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.AreaSeriesViewBase.Transparency"),
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
		public AreaSeriesViewBase()
			: base() {
			opacity = DefaultOpacity;
			border = new CustomBorder(this, true, Color.Empty);
			fillStyle = new PolygonFillStyle(this, Color.Empty);
		}
		#region IAreaSeriesView Members
		CustomBorder IAreaSeriesView.Border { get { return ActualBorder; } }
		PolygonFillStyle IAreaSeriesView.FillStyle { get { return FillStyle; } }
		bool IAreaSeriesView.Rotated { get { return ((XYDiagram)Series.Chart.Diagram).Rotated; } }
		bool IAreaSeriesView.Closed { get { return false; } }
		Marker IAreaSeriesView.MarkerOptions { get { return ActualMarkerOptions; } }
		PolygonFillStyle IAreaSeriesView.ActualFillStyle { get { return ActualFillStyle; } }
		AreaSeriesViewAppearance IAreaSeriesView.Appearance { get { return Appearance; } }
		bool IAreaSeriesView.GetActualAntialiasing(int pointsCount) {
			return GetActualAntialiasing(pointsCount);
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Border")
				return ShouldSerializeBorder();
			if (propertyName == "FillStyle")
				return ShouldSerializeFillStyle();
			if (propertyName == "MarkerOptions")
				return ShouldSerializeMarkerOptions();
			if (propertyName == "Transparency")
				return ShouldSerializeTransparency();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeBorder() {
			return Border.ShouldSerialize();
		}
		bool ShouldSerializeFillStyle() {
			return FillStyle.ShouldSerialize();
		}
		bool ShouldSerializeMarkerOptions() {
			return MarkerOptions.ShouldSerialize();
		}
		bool ShouldSerializeTransparency() {
			return opacity != DefaultOpacity;
		}
		void ResetTransparency() {
			Transparency = DefaultOpacity;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeBorder() ||
				ShouldSerializeFillStyle() ||
				ShouldSerializeMarkerOptions() ||
				ShouldSerializeTransparency();
		}
		#endregion
		void ITransparableView.AssignTransparency(ITransparableView view) {
			opacity = ConvertBetweenOpacityAndTransparency(view.Transparency);
		}
		protected override void SyncColorsAndTransparency(byte opacity) {
			base.SyncColorsAndTransparency(opacity);
			if (!Color2.IsEmpty)
				Color2 = Color.FromArgb(opacity, Color2);
		}
		protected override PointSeriesViewPainter CreatePainter() {
			return new AreaSeriesViewPainter(this);
		}
		protected override IGeometryStrip CreateStripInternal() {
			return new RangeStrip();
		}
		protected override void AddStripElementInternal(IGeometryStrip strip, RefinedPoint refinedPoint) {
			MinMaxValues values = GetSeriesPointValues(refinedPoint);
			GRealPoint2D topPoint = new GRealPoint2D(refinedPoint.Argument, values.Max);
			GRealPoint2D bottomPoint = new GRealPoint2D(refinedPoint.Argument, values.Min);
			((RangeStrip)strip).Add(new StripRange(topPoint, bottomPoint));
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new AreaGeometryStripCreator();
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new AreaDrawOptions(this);
		}
		protected internal override Color GetPointColor(int pointIndex, int pointsCount) {
			return Color.FromArgb(opacity, base.GetPointColor(pointIndex, pointsCount));
		}
		protected internal override Color GetPointColor2(int pointIndex, int pointsCount) {
			return Color.FromArgb(opacity, base.GetPointColor2(pointIndex, pointsCount));
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(XYDiagramMappingBase diagramMapping, RefinedPointData pointData) {
			RefinedPoint refinedPoint = pointData.RefinedPoint;
			MinMaxValues values = GetSeriesPointValues(refinedPoint);
			DiagramPoint zeroPoint = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument, values.Min);
			DiagramPoint actualPoint = diagramMapping.GetScreenPointNoRound(refinedPoint.Argument, values.Max);
			IPolygon polygon = CalculateMarkerPolygon(pointData, actualPoint);
			return new AreaSeriesPointLayout(pointData, actualPoint, polygon, zeroPoint);
		}
		protected internal override WholeSeriesLayout CalculateWholeSeriesLayout(XYDiagramMappingBase diagramMapping, SeriesLayout seriesLayout) {
			LineAndAreaWholeSeriesViewData viewData = (LineAndAreaWholeSeriesViewData)seriesLayout.SeriesData.WholeViewData;
			return new AreaWholeSeriesLayout(seriesLayout, StripsUtils.MapRangeStrips(diagramMapping, viewData.Strips), diagramMapping.Bounds, ((XYDiagramSeriesLayout)seriesLayout).SingleLayout);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ITransparableView transparableView = obj as ITransparableView;
			if (transparableView == null)
				return;
			((ITransparableView)this).AssignTransparency(transparableView);
			IAreaSeriesView view = obj as IAreaSeriesView;
			if (view == null)
				return;
			if (view.Border != null)
				border.Assign(view.Border);
			fillStyle.Assign(view.FillStyle);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			AreaSeriesView view = (AreaSeriesView)obj;
			return border.Equals(view.border) && fillStyle.Equals(view.fillStyle) && opacity == view.opacity;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class AreaSeriesPointLayout : PointSeriesPointLayout {
		DiagramPoint zeroPoint;
		public DiagramPoint ZeroPoint { get { return zeroPoint; } }
		public AreaSeriesPointLayout(RefinedPointData pointData, DiagramPoint actualPoint, IPolygon polygon, DiagramPoint zeroPoint)
			: base(pointData, actualPoint, polygon) {
			this.zeroPoint = zeroPoint;
		}
	}
	public class AreaWholeSeriesLayout : WholeSeriesLayout {
		List<IGeometryStrip> strips;
		RectangleF bounds;
		bool optimizeHitTesting;
		public List<IGeometryStrip> Strips { get { return strips; } }
		public AreaWholeSeriesLayout(SeriesLayout seriesLayout, List<IGeometryStrip> strips, RectangleF bounds, bool optimizeHitTesting)
			: base(seriesLayout) {
			this.strips = strips;
			this.bounds = bounds;
			this.optimizeHitTesting = optimizeHitTesting;
		}
		HitRegion CreateStripHitRegion(RangeStrip strip) {
			GraphicsPath stripPath = StripsUtils.GetPath(strip);
			return optimizeHitTesting ? new HitRegion(stripPath) : new HitRegion(bounds, stripPath);
		}
		HitRegion CreateTopStripHitRegion(RangeStrip strip) {
			GraphicsPath topStripPath = StripsUtils.GetPath(strip.TopStrip);
			return optimizeHitTesting ? new HitRegion(topStripPath) : new HitRegion(bounds, topStripPath);
		}
		public override HitRegionContainer CalculateHitRegion(DrawOptions drawOptions) {
			HitRegionContainer hitRegion = base.CalculateHitRegion(drawOptions);
			foreach (RangeStrip strip in strips) {
				hitRegion.Union(CreateStripHitRegion(strip));
				hitRegion.Union(CreateTopStripHitRegion(strip));
			}
			return hitRegion;
		}
	}
}
