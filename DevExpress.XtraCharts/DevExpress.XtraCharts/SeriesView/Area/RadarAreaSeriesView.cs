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

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(RadarAreaSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RadarAreaSeriesView : RadarLineSeriesView, IAreaSeriesView, IGeometryStripCreator {
		const int maxMarkerInLegendSize = 20;
		byte opacity;
		PolygonFillStyle fillStyle;
		CustomBorder border;
		byte DefaultOpacity { get { return ConvertBetweenOpacityAndTransparency((byte)135); } }
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
		internal PolygonFillStyle ActualFillStyle {
			get { return fillStyle.FillMode == FillMode.Empty ? Appearance.FillStyle : fillStyle; }
		}
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnRadarArea); } }
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
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAreaSeriesViewBorder"),
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
	DevExpressXtraChartsLocalizedDescription("RadarAreaSeriesViewFillStyle"),
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
	DevExpressXtraChartsLocalizedDescription("RadarAreaSeriesViewMarkerOptions"),
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
		NonTestablePropertyAttribute(),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new LineStyle LineStyle { get { return null; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		NonTestablePropertyAttribute(),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Marker LineMarkerOptions { get { return null; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarAreaSeriesViewTransparency"),
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
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty(),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new bool Closed { get { return true; } set { } }
		public RadarAreaSeriesView() {
			opacity = DefaultOpacity;
			border = new CustomBorder(this, true, Color.Empty);
			fillStyle = new PolygonFillStyle(this, Color.Empty);
		}
		#region ITransparableView implementation
		void ITransparableView.AssignTransparency(ITransparableView view) {
			opacity = ConvertBetweenOpacityAndTransparency(view.Transparency);
		}
		#endregion
		#region IAreaSeriesView Members
		CustomBorder IAreaSeriesView.Border { get { return Border; } }
		PolygonFillStyle IAreaSeriesView.FillStyle { get { return FillStyle; } }
		PolygonFillStyle IAreaSeriesView.ActualFillStyle { get { return ActualFillStyle; } }
		bool IAreaSeriesView.Rotated { get { return false; } }
		AreaSeriesViewAppearance IAreaSeriesView.Appearance { get { return Appearance; } }
		bool IAreaSeriesView.GetActualAntialiasing(int pointsCount) {
			return false;
		}
		#endregion
		#region IGeometryStripCreator implementation
		IGeometryStrip IGeometryStripCreator.CreateStrip() {
			return new RangeStrip();
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
			Transparency = ConvertBetweenOpacityAndTransparency(DefaultOpacity);
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
		protected override void SyncColorsAndTransparency(byte opacity) {
			base.SyncColorsAndTransparency(opacity);
			if (!Color2.IsEmpty)
				Color2 = Color.FromArgb(opacity, Color2);
		}
		protected override PointSeriesViewPainter CreatePainter() {
			return new AreaSeriesViewPainter(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RadarAreaSeriesView();
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new AreaDrawOptions(this);
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new RadarAreaGeometryStripCreator(true);
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(RadarDiagramMapping diagramMapping, RefinedPointData pointData) {
			IXYPoint pointInfo = pointData.RefinedPoint;
			DiagramPoint zeroPoint = diagramMapping.GetScreenPoint(pointInfo.Argument, 0.0);
			DiagramPoint actualPoint = diagramMapping.GetScreenPoint(pointInfo.Argument, pointInfo.Value);
			if (zeroPoint.IsZero || actualPoint.IsZero)
				return null;
			IPolygon polygon = CalculateMarkerPolygon(pointData, actualPoint);
			return new AreaSeriesPointLayout(pointData, actualPoint, polygon, zeroPoint);
		}
		protected internal override WholeSeriesLayout CalculateWholeSeriesLayout(RadarDiagramMapping diagramMapping, RadarDiagramSeriesLayout seriesLayout) {
			LineAndAreaWholeSeriesViewData viewData = (LineAndAreaWholeSeriesViewData)seriesLayout.WholeViewData;
			List<IGeometryStrip> strips = StripsUtils.MapRangeStrips(diagramMapping, viewData.Strips);
			return new AreaWholeSeriesLayout(seriesLayout, strips, RectangleF.Empty, true);
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
			RadarAreaSeriesView view = (RadarAreaSeriesView)obj;
			return border.Equals(view.border) && fillStyle.Equals(view.fillStyle) && opacity == view.opacity;
		}
	}
}
