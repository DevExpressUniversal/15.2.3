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
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(PointSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PointSeriesView : PointSeriesViewBase {
		const int DefaultMarkerSize = 8;
		protected override int PixelsPerArgument { get { return PointMarkerOptions.Size; } }
		protected override Type PointInterfaceType { get { return typeof(IXYPoint); } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnPoint); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PointSeriesViewPointMarkerOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PointSeriesView.PointMarkerOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public SimpleMarker PointMarkerOptions { get { return (SimpleMarker)Marker; } }		
		public PointSeriesView() : base() {
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "PointMarkerOptions")
				return ShouldSerializePointMarkerOptions();
			return base.XtraShouldSerialize(propertyName);
		}
		bool ShouldSerializePointMarkerOptions() {
			return PointMarkerOptions.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializePointMarkerOptions();
		}
		protected override MarkerBase CreateMarker() {
			return new SimpleMarker(this, DefaultMarkerSize);
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
		protected override double GetHighlightedPointValue(RefinedPoint point) {
			return GetSeriesPointValues(point).Max;
		}
		protected override DrawOptions CreateSeriesDrawOptionsInternal() {
			return new PointDrawOptions(this);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new PointSeriesLabel();
		}
		protected internal virtual MinMaxValues GetSeriesPointValues(RefinedPoint point) {
			return new MinMaxValues(((IValuePoint)point).Value);
		}
		protected internal override SeriesPointLayout CalculateSeriesPointLayout(XYDiagramMappingBase diagramMapping, RefinedPointData pointData) {
			MinMaxValues values = GetSeriesPointValues(pointData.RefinedPoint);
			DiagramPoint point = diagramMapping.GetScreenPointNoRound(pointData.RefinedPoint.Argument, values.Max);
			IPolygon polygon = CalculateMarkerPolygon(pointData, point);
			return new PointSeriesPointLayout(pointData, point, polygon);
		}
		protected override ChartElement CreateObjectForClone() {
			return new PointSeriesView();
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(System.Drawing.Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(System.Drawing.Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		public override string GetValueCaption(int index) {
			if (index > 0)
				throw new IndexOutOfRangeException();
			return ChartLocalizer.GetString(ChartStringId.ValueMember);
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class PointSeriesPointLayout : SeriesPointLayout {
		DiagramPoint point;
		IPolygon polygon;
		public DiagramPoint Point { get { return point; } }
		public IPolygon Polygon { get { return polygon; } }
		public PointSeriesPointLayout(RefinedPointData pointData, DiagramPoint point, IPolygon polygon)
			: base(pointData) {
			this.point = point;
			this.polygon = polygon;
		}		
		public override HitRegionContainer CalculateHitRegion() {
			HitRegionContainer hitRegion = base.CalculateHitRegion();
			IHitRegion markerRegion = GraphicUtils.MakeHitRegion(this.polygon);
			hitRegion.Union(markerRegion);
			return hitRegion;
		}
	}
}
