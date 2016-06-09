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
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using System.Collections.Generic;
namespace DevExpress.XtraCharts {
	public abstract class RadarSeriesViewBase : SeriesViewBase, IXYSeriesView, IShadowSupportView, IColorEachSupportView {
		const bool DefaultColorEach = false;
		readonly Shadow shadow;
		bool colorEach = DefaultColorEach;
		protected abstract int PixelsPerArgument { get; }
		protected internal override bool NeedFilterVisiblePoints {
			get {
				return false;
			}
		}
		protected internal override bool ActualColorEach { get { return colorEach; } }
		protected internal override bool AnnotationLabelModeSupported { get { return true; } }
		protected internal override bool DateTimeValuesSupported { get { return true; } }
		protected internal override bool HitTestingSupportedForLegendMarker { get { return true; } }
		protected internal override bool IsSupportedToolTips { get { return true; } }
		protected override bool Is3DView { get { return false; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override Type DiagramType { get { return typeof(RadarDiagram); } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarSeriesViewBaseShadow"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarSeriesViewBase.Shadow"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Shadow Shadow { get { return shadow; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarSeriesViewBaseColorEach"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarSeriesViewBase.ColorEach"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool ColorEach {
			get { return colorEach; }
			set {
				if (value != colorEach) {
					SendNotification(new ElementWillChangeNotification(this));
					colorEach = value;
					RaiseControlChanged();
				}
			}
		}
		public RadarSeriesViewBase() {
			shadow = new Shadow(this);
		}		
		#region IXYSeriesView
		IAxisData IXYSeriesView.AxisXData {
			get {
				Chart chart = Chart;
				if (chart == null)
					return null;
				RadarDiagram diagram = chart.Diagram as RadarDiagram;
				return diagram == null ? null : diagram.AxisX;
			}
		}
		IAxisData IXYSeriesView.AxisYData {
			get {
				Chart chart = Chart;
				if (chart == null)
					return null;
				RadarDiagram diagram = chart.Diagram as RadarDiagram;
				return diagram == null ? null : diagram.AxisY;
			}
		}
		IPane IXYSeriesView.Pane {
			get {
				Chart chart = Chart;
				return chart == null ? null : chart.Diagram as RadarDiagram;
			}
		}
		SeriesContainer ISeriesView.CreateContainer() {
			return new XYSeriesContainer(this);
		}
		bool IXYSeriesView.CrosshairEnabled { get { return false; } }
		bool IXYSeriesView.SideMarginsEnabled { get { return SideMarginsEnabled; } }
		int IXYSeriesView.PixelsPerArgument { get { return PixelsPerArgument; } }
		string IXYSeriesView.CrosshairLabelPattern { get { return string.Empty; } }
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
		#region XtraShouldSerialize
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "ColorEach")
				return ShouldSerializeColorEach();
			if (propertyName == "Shadow")
				return ShouldSerializeShadow();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeColorEach() {
			return colorEach != DefaultColorEach;
		}
		void ResetColorEach() {
			ColorEach = DefaultColorEach;
		}
		bool ShouldSerializeShadow() {
			return Shadow.ShouldSerialize();
		}
		#endregion
		protected abstract DiagramPoint CalculateAnnotationAchorPoint(RadarDiagramMapping diagramMapping, RefinedPointData pointData);
		protected override PatternDataProvider GetDataProvider(PatternConstants patternConstant) {
			return GetXYDataProvider(patternConstant);
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value;
		}
		protected internal override bool ShouldSerialize() {
			return
				base.ShouldSerialize() ||
				ShouldSerializeColorEach() ||
				ShouldSerializeShadow();
		}
		protected internal virtual SeriesPointLayout CalculateSeriesPointLayout(RadarDiagramMapping diagramMapping, RefinedPointData pointData) {
			return null;
		}
		protected internal virtual WholeSeriesLayout CalculateWholeSeriesLayout(RadarDiagramMapping diagramMapping, RadarDiagramSeriesLayout seriesLayout) {
			return null;
		}
		protected internal DiagramPoint? CalculateRelativeToolTipPosition(RadarDiagramMapping diagramMapping, RefinedPointData pointData) {
			IXYPoint pointInfo = pointData.RefinedPoint;
			return diagramMapping.GetScreenPoint(pointInfo.Argument, pointInfo.Value);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new RadarPointSeriesLabel();
		}
		protected internal void CalculateAnnotationsAchorPointsLayout(RadarDiagramAnchorPointLayoutList anchorPointLayoutList) {
			foreach (RefinedPointData pointData in anchorPointLayoutList.SeriesData) {
				SeriesPoint seriesPoint = pointData.RefinedPoint.SeriesPoint as SeriesPoint;
				if (seriesPoint != null && seriesPoint.Annotations.Count > 0) {
					DiagramPoint point = CalculateAnnotationAchorPoint(anchorPointLayoutList.DiagramMapping, pointData);
					foreach (Annotation annotation in seriesPoint.Annotations)
						anchorPointLayoutList.Add(new AnnotationLayout(annotation, point, pointData.RefinedPoint));
				}
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			IShadowSupportView shadowView = obj as IShadowSupportView;
			if (shadowView != null)
				shadow.Assign(shadowView.Shadow);
			IColorEachSupportView colorEachView = obj as IColorEachSupportView;
			if (colorEachView != null)
				colorEach = colorEachView.ColorEach;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			RadarSeriesViewBase view = (RadarSeriesViewBase)obj;
			return shadow.Equals(view.shadow) && colorEach == view.colorEach;
		}
	}
}
