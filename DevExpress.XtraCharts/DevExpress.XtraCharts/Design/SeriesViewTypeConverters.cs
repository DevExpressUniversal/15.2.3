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
using System.Globalization;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public abstract class SeriesViewTypeConverter : ExpandableObjectConverter {
		protected abstract Type ViewType { get; }
		protected virtual bool IsComplete { get { return false; } }
		protected virtual Type[] GetConstructorParamsType(object value) {
			return new Type[] { };
		}
		protected virtual object[] GetConstructorParams(object value) {
			return null;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor) && value is SeriesViewBase) {
				ConstructorInfo ci = ViewType.GetConstructor(GetConstructorParamsType(value));
				return new InstanceDescriptor(ci, GetConstructorParams(value), IsComplete);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
			XYDiagram2DSeriesViewBase xySeriesView = TypeConverterHelper.GetElement<XYDiagram2DSeriesViewBase>(value);
			if (xySeriesView == null)
				return properties;
			List<PropertyDescriptor> resultProperties = new List<PropertyDescriptor>();
			if (xySeriesView.ChartContainer.ControlType == ChartContainerType.WebControl ||
				xySeriesView.ChartContainer.ControlType == ChartContainerType.XRControl) {
				foreach (PropertyDescriptor property in properties)
					if (property.PropertyType != typeof(RangeControlOptions))
						resultProperties.Add(property);
			}
			else
				return properties;
			return new PropertyDescriptorCollection(resultProperties.ToArray());
		}
	}
	public class SideBySideBarSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SideBySideBarSeriesView); } }
	}
	public class StackedBarSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StackedBarSeriesView); } }
	}
	public class SideBySideStackedBarSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SideBySideStackedBarSeriesView); } }
	}
	public class SideBySideFullStackedBarSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SideBySideFullStackedBarSeriesView); } }
	}
	public abstract class PieSeriesViewBaseTypeConverter : SeriesViewTypeConverter {
		protected override Type[] GetConstructorParamsType(object value) {
			if (((PieSeriesViewBase)value).ShouldSerializeExplodedPoints)
				return new Type[] { typeof(int[]) };
			else
				return base.GetConstructorParamsType(value);
		}
		protected override object[] GetConstructorParams(object value) {
			PieSeriesViewBase view = (PieSeriesViewBase)value;
			if (view.ShouldSerializeExplodedPoints)
				return new object[] { view.ExplodedPoints.GetPointIds() };
			else
				return base.GetConstructorParams(value);
		}
		protected virtual PropertyDescriptorCollection FilterProperties(PropertyDescriptor[] propertyDescriptors, PieSeriesViewBase view) {
			return new PropertyDescriptorCollection(propertyDescriptors);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			PieSeriesViewBase view = TypeConverterHelper.GetElement<PieSeriesViewBase>(value);
			if (view == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				if (desc.Name == "Titles" && PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(view.Chart.DataContainer.PivotGridDataSourceOptions, view.Owner as SeriesBase))
					descs.Add(new ReadOnlyPropertyDescriptor(desc));
				else if (desc.Name == "ExplodedPointsFilters") {
					if (view.ExplodeMode == PieExplodeMode.UseFilters)
						descs.Add(desc);
				}
				else
					descs.Add(desc);
			}
			return FilterProperties(descs.ToArray(), view);
		}
	}
	public class PieSeriesViewTypeConverter : PieSeriesViewBaseTypeConverter {
		protected override Type ViewType { get { return typeof(PieSeriesView); } }
	}
	public class Pie3DSeriesViewTypeConverter : PieSeriesViewBaseTypeConverter {
		protected override Type ViewType { get { return typeof(Pie3DSeriesView); } }
	}
	public class DoughnutSeriesViewTypeConverter : PieSeriesViewBaseTypeConverter {
		protected override Type ViewType { get { return typeof(DoughnutSeriesView); } }
	}
	public class Doughnut3DSeriesViewTypeConverter : PieSeriesViewBaseTypeConverter {
		protected override Type ViewType { get { return typeof(Doughnut3DSeriesView); } }
	}
	public class NestedDoughnutSeriesViewTypeConverter : PieSeriesViewBaseTypeConverter {
		protected override Type ViewType { get { return typeof(NestedDoughnutSeriesView); } }
		List<string> outsideDoughnutOnlyProperties = new List<string>() { "ExplodedPointsFilters", "ExplodedDistancePercentage", "ExplodeMode", "ExplodedPoints",
			"HoleRadiusPercent", "MinAllowedSizePercentage", "HeightToWidthRatio", "RuntimeExploding", "Titles" };
		protected override PropertyDescriptorCollection FilterProperties(PropertyDescriptor[] propertyDescriptors, PieSeriesViewBase view) {
			DevExpress.Charts.Native.INestedDoughnutSeriesView nestedView = view as DevExpress.Charts.Native.INestedDoughnutSeriesView;
			PropertyDescriptorCollection propertyDescriptorsCollection = new PropertyDescriptorCollection(propertyDescriptors);
			if (nestedView != null && !nestedView.IsOutside.Value)
				return FilterPropertiesUtils.FilterProperties(propertyDescriptorsCollection, outsideDoughnutOnlyProperties);
			return propertyDescriptorsCollection;
		}
	}
	public class FunnelSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(FunnelSeriesView); } }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			FunnelSeriesView view = TypeConverterHelper.GetElement<FunnelSeriesView>(value);
			if (view == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				if(desc.Name == "Titles" && PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(view.Chart.DataContainer.PivotGridDataSourceOptions, view.Owner as SeriesBase))
					descs.Add(new ReadOnlyPropertyDescriptor(desc));
				else if (desc.Name == "HeightToWidthRatio") {
					if (!view.HeightToWidthRatioAuto)
						descs.Add(desc);
				}
				else
					descs.Add(desc);
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class Funnel3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(Funnel3DSeriesView); } }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			Funnel3DSeriesView view = TypeConverterHelper.GetElement<Funnel3DSeriesView>(value);
			if (view == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				if(desc.Name == "Titles" && PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(view.Chart.DataContainer.PivotGridDataSourceOptions, view.Owner as SeriesBase))
					descs.Add(new ReadOnlyPropertyDescriptor(desc));
				else
					descs.Add(desc);
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public abstract class Bar3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		static bool ShouldHideBarDepthProperty(Bar3DSeriesView view) {
			return view.BarDepthAuto;
		}
		static bool ShouldHideShowFacetProperty(Bar3DSeriesView view) {		   
			return !view.IsFlatTopModel;
		}
		static List<string> GetFilteredProperties(object value) {
			Bar3DSeriesView view = TypeConverterHelper.GetElement<Bar3DSeriesView>(value);
			List<string> filteredProperties = new List<string>();
			if (view == null)
				return filteredProperties;			
			if (ShouldHideBarDepthProperty(view))
				filteredProperties.Add("BarDepth");
			if (ShouldHideShowFacetProperty(view))
				filteredProperties.Add("ShowFacet");
			return filteredProperties;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			return FilterPropertiesUtils.FilterProperties(collection, GetFilteredProperties(value));
		}
	}
	public class SideBySideBar3DSeriesViewTypeConverter : Bar3DSeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SideBySideBar3DSeriesView); } }
	}
	public class StackedBar3DSeriesViewTypeConverter : Bar3DSeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StackedBar3DSeriesView); } }
	}
	public class SideBySideStackedBar3DSeriesViewTypeConverter : Bar3DSeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SideBySideStackedBar3DSeriesView); } }
	}
	public class FullStackedBar3DSeriesViewTypeConverter : Bar3DSeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(FullStackedBar3DSeriesView); } }
	}
	public class SideBySideFullStackedBar3DSeriesViewTypeConverter : Bar3DSeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SideBySideFullStackedBar3DSeriesView); } }
	}
	public class ManhattanBarSeriesViewTypeConverter : Bar3DSeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(ManhattanBarSeriesView); } }
	}
	public class Area3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(Area3DSeriesView); } }
	}
	public class AreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(AreaSeriesView); } }
	}
	public class StepAreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StepAreaSeriesView); } }
	}
	public class StepArea3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StepArea3DSeriesView); } }
	}
	public class SplineAreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SplineAreaSeriesView); } }
	}
	public class FullStackedArea3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(FullStackedArea3DSeriesView); } }
	}
	public class FullStackedAreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(FullStackedAreaSeriesView); } }
	}
	public class SplineFullStackedAreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(FullStackedSplineAreaSeriesView); } }
	}
	public class RangeAreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(RangeAreaSeriesView); } }
	}
	public class RangeArea3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(RangeArea3DSeriesView); } }
	}
	public class PolarAreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(PolarAreaSeriesView); } }
	}
	public class RadarAreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(RadarAreaSeriesView); } }
	}
	public class StackedArea3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StackedArea3DSeriesView); } }
	}
	public class StackedAreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StackedAreaSeriesView); } }
	}
	public class SplineStackedAreaSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StackedSplineAreaSeriesView); } }
	}
	public class FullStackedBarSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(FullStackedBarSeriesView); } }
	}
	public class CandleStickSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(CandleStickSeriesView); } }
	}
	public class StockSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StockSeriesView); } }
	}
	public class Line3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(Line3DSeriesView); } }
	}
	public class LineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(LineSeriesView); } }
	}
	public class SwiftPlotSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SwiftPlotSeriesView); } }
	}
	public class SplineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SplineSeriesView); } }
	}
	public class Spline3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(Spline3DSeriesView); } }
	}
	public class SplineArea3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SplineArea3DSeriesView); } }
	}
	public class SplineAreaStacked3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StackedSplineArea3DSeriesView); } }
	}
	public class SplineAreaFullStacked3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(FullStackedSplineArea3DSeriesView); } }
	}
	public class PolarLineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(PolarLineSeriesView); } }
	}
	public class RadarLineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(RadarLineSeriesView); } }
	}
	public class ScatterPolarLineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(ScatterPolarLineSeriesView); } }
	}
	public class ScatterRadarLineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(ScatterRadarLineSeriesView); } }
	}
	public class StepLine3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StepLine3DSeriesView); } }
	}
	public class StackedLineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StackedLineSeriesView); } }
	}
	public class StackedLine3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StackedLine3DSeriesView); } }
	}
	public class FullStackedLineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(FullStackedLineSeriesView); } }
	}
	public class FullStackedLine3DSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(FullStackedLine3DSeriesView); } }
	}
	public class StepLineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(StepLineSeriesView); } }
	}
	public class ScatterLineSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(ScatterLineSeriesView); } }
	}
	public class PointSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(PointSeriesView); } }
	}
	public class BubbleSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(BubbleSeriesView); } }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			BubbleSeriesView view = TypeConverterHelper.GetElement<BubbleSeriesView>(value);
			if (view == null || !view.AutoSize)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				if (desc.Name != "MaxSize" && desc.Name != "MinSize")
					descs.Add(desc);
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class PolarPointSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(PolarPointSeriesView); } }
	}
	public class RadarPointSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(RadarPointSeriesView); } }
	}
	public class OverlappedGanttSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(OverlappedGanttSeriesView); } }
	}
	public class OverlappedRangeBarSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(OverlappedRangeBarSeriesView); } }
	}
	public class SideBySideGanttSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SideBySideGanttSeriesView); } }
	}
	public class SideBySideRangeBarSeriesViewTypeConverter : SeriesViewTypeConverter {
		protected override Type ViewType { get { return typeof(SideBySideRangeBarSeriesView); } }
	}
}
