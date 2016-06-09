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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using System.Collections.Generic;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(RadarPointSeriesViewTypeConverter)),
		DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RadarPointSeriesView : RadarSeriesViewBase, IPointSeriesView {
		protected const int LegendMarkerShadowSize = 1;
		const int DefaultMarkerSize = 8;
		SimpleMarker marker;
		PointSeriesViewPainter painter;
		protected override int PixelsPerArgument { get { return PointMarkerOptions.Size; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnRadarPoint); } }
		protected internal virtual bool ActualMarkerVisible { get { return true; } }
		protected override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.RadarView; } }
		protected override Type PointInterfaceType {
			get {
				return typeof(IXYPoint);
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarPointSeriesViewPointMarkerOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarPointSeriesView.PointMarkerOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public SimpleMarker PointMarkerOptions { get { return marker; } }
		public RadarPointSeriesView() {
			marker = CreateMarker();
			painter = CreatePainter();
		}
		#region IPointSeriesView implementation
		MarkerBase IPointSeriesView.Marker {
			get { return PointMarkerOptions; }
		}
		#endregion
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "PointMarkerOptions")
				return ShouldSerializePointMarkerOptions();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		bool ShouldSerializePointMarkerOptions() {
			return PointMarkerOptions.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializePointMarkerOptions();
		}
		protected virtual PointSeriesViewPainter CreatePainter() {
			return new PointSeriesViewPainter(this);
		}
		protected virtual SimpleMarker CreateMarker() {
			return new SimpleMarker(this, DefaultMarkerSize);
		}
		protected override DiagramPoint CalculateAnnotationAchorPoint(RadarDiagramMapping diagramMapping, RefinedPointData pointData) {
			IXYPoint pointInfo = pointData.RefinedPoint;
			return diagramMapping.GetScreenPoint(pointInfo.Argument, pointInfo.Value);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RadarPointSeriesView();
		}
		protected override double GetRefinedPointMax(RefinedPoint point) {
			IXYPoint xyPoint = (IXYPoint)point;
			return xyPoint.Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			IXYPoint xyPoint = (IXYPoint)point;
			return xyPoint.Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IXYPoint)point).Value);
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new PointDrawOptions(this);
		}
		protected override Rectangle CorrectLegendMarkerBounds(Rectangle bounds) {
			return new Rectangle(bounds.Left, bounds.Top,
				bounds.Width - Shadow.GetActualSize(LegendMarkerShadowSize),
				bounds.Height - Shadow.GetActualSize(LegendMarkerShadowSize));
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(RadarDiagramMapping diagramMapping, RefinedPointData pointData) {
			IXYPoint pointInfo = pointData.RefinedPoint;
			DiagramPoint point = diagramMapping.GetScreenPoint(pointInfo.Argument, pointInfo.Value);
			if (point.IsZero)
				return null;
			IPolygon polygon = CalculateMarkerPolygon(pointData, point);
			return new PointSeriesPointLayout(pointData, point, polygon);
		}
		protected internal override void Render(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			painter.Render(renderer, mappingBounds, pointLayout, drawOptions);
		}
		protected internal override void RenderShadow(IRenderer renderer, Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			painter.RenderShadow(renderer, mappingBounds, pointLayout, drawOptions);
		}
		protected internal override void RenderWholeSeries(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			painter.RenderWholeSeries(renderer, mappingBounds, layout);
		}
		protected internal override void RenderWholeSeriesShadow(IRenderer renderer, Rectangle mappingBounds, WholeSeriesLayout layout) {
			painter.RenderWholeSeriesShadow(renderer, mappingBounds, layout);
		}
		protected override void RenderLegendMarkerInternal(IRenderer renderer, Rectangle bounds, DrawOptions seriesPointDrawOptions, DrawOptions seriesDrawOptions, SelectionState selectionState) {
			painter.RenderLegendMarker(renderer, bounds, seriesPointDrawOptions, seriesDrawOptions, selectionState);
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		public override string GetValueCaption(int index) {
			if (index > 0)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			IPointSeriesView view = obj as IPointSeriesView;
			if (view == null)
				return;
			marker.Assign(view.Marker);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			RadarPointSeriesView view = (RadarPointSeriesView)obj;
			return marker.Equals(view.marker);
		}
	}
}
