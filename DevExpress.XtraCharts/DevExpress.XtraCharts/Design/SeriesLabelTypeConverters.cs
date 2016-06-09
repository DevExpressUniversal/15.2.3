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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Design {
	public abstract class SeriesLabelBaseTypeConverter : ExpandableObjectConverter {
		protected const string anglePropertyName = "Angle";
		protected abstract Type LabelType { get; }
		protected virtual List<string> GetFilteredProperties(SeriesLabelBase label) {
			List<string> filteredProperties = new List<string>();
			if(label != null && label.ResolveOverlappingMode == ResolveOverlappingMode.JustifyAllAroundPoint)
				filteredProperties.AddRange(new string[] { anglePropertyName });
			return filteredProperties;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			SeriesLabelBase label = TypeConverterHelper.GetElement<SeriesLabelBase>(value);
			if(label == null)
				return collection;
			PropertyDescriptor[] descs = new PropertyDescriptor[collection.Count];
			collection.CopyTo(descs, 0);
			return FilterPropertiesUtils.FilterProperties(new PropertyDescriptorCollection(descs), GetFilteredProperties(label));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(LabelType.GetConstructor(new Type[] { }), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class BubbleSeriesLabelTypeConverter : PointSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(BubbleSeriesLabel); } }
	}
	public class PointSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		const string indentFromMarkerPropertyName = "IndentFromMarker";
		protected override Type LabelType { get { return typeof(PointSeriesLabel); } }
		protected override List<string> GetFilteredProperties(SeriesLabelBase label) {
			List<string> filteredProperties = base.GetFilteredProperties(label);
			PointSeriesLabel pointLabel = label as PointSeriesLabel;
			if (pointLabel != null && pointLabel.Position == PointLabelPosition.Center) {
				filteredProperties.AddRange(new string[] { anglePropertyName, indentFromMarkerPropertyName });
				filteredProperties.AddRange(FilterPropertiesUtils.ConnectorProperties);
			}
			return filteredProperties;
		}
	}
	public class StackedLineSeriesLabelTypeConverter : PointSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(StackedLineSeriesLabel); } }
	}
	public class RadarPointSeriesLabelTypeConverter : PointSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(RadarPointSeriesLabel); } }
	}
	public class StockSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(StockSeriesLabel); } }
	}
	public class Line3DSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(Line3DSeriesLabel); } }
	}
	public class StackedLine3DSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(StackedLine3DSeriesLabel); } }
	}
	public class PieSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(PieSeriesLabel); } }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			PieSeriesLabel label = TypeConverterHelper.GetElement<PieSeriesLabel>(value);
			if (label == null)
				return collection;
			if (label.Position == PieSeriesLabelPosition.Inside)
				collection = FilterPropertiesUtils.FilterSeriesLabelConnectorProperties(collection);
			List<string> filteredProperties = new List<string>();
			if (label.Position != PieSeriesLabelPosition.TwoColumns)
				filteredProperties.Add("ColumnIndent");
			if (!label.ResolveOverlappingSupported) {
				filteredProperties.Add("ResolveOverlappingMode");
				filteredProperties.Add("ResolveOverlappingMinIndent");
			}
			collection = FilterPropertiesUtils.FilterProperties(collection, filteredProperties);
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				if(desc.Name == "Position" && PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(label.SeriesBase.View.Chart.DataContainer.PivotGridDataSourceOptions, label.SeriesBase))
					descs.Add(new ReadOnlyPropertyDescriptor(desc));
				else
					descs.Add(desc);
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class Pie3DSeriesLabelTypeConverter : PieSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(Pie3DSeriesLabel); } }
	}
	public class DoughnutSeriesLabelTypeConverter : PieSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(DoughnutSeriesLabel); } }
	}
	public class NestedDoughnutSeriesLabelTypeConverter : PieSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(NestedDoughnutSeriesLabel); } }
	}
	public class Doughnut3DSeriesLabelTypeConverter : PieSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(Doughnut3DSeriesLabel); } }
	}
	public class RangeBarSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		const string positionPropertyName = "Position";
		const string indentPropertyName = "Indent";
		protected override Type LabelType { get { return typeof(RangeBarSeriesLabel); } }
		protected override List<string> GetFilteredProperties(SeriesLabelBase label) {
			List<string> filteredProperties = base.GetFilteredProperties(label);
			RangeBarSeriesLabel rangeBarLabel = label as RangeBarSeriesLabel;
			if (rangeBarLabel != null) {
				if (rangeBarLabel.Kind == RangeBarLabelKind.OneLabel)
					filteredProperties.Add(positionPropertyName);
				if (rangeBarLabel.Kind == RangeBarLabelKind.OneLabel || rangeBarLabel.Position == RangeBarLabelPosition.Center)
					filteredProperties.Add(indentPropertyName);
			}
			return filteredProperties;
		}
	}
	public class RangeAreaSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		const string minValueAnglePropertyName = "MinValueAngle";
		const string maxValueAnglePropertyName = "MaxValueAngle";
		protected override Type LabelType { get { return typeof(RangeAreaSeriesLabel); } }
		protected override List<string> GetFilteredProperties(SeriesLabelBase label) {
			List<string> filteredProperties = base.GetFilteredProperties(label);
			RangeAreaSeriesLabel rangeAreaLabel = label as RangeAreaSeriesLabel;
			if (rangeAreaLabel != null) {
				if (rangeAreaLabel.Kind == RangeAreaLabelKind.OneLabel) {
					filteredProperties.Add(minValueAnglePropertyName);
					filteredProperties.Add(maxValueAnglePropertyName);
					filteredProperties.AddRange(FilterPropertiesUtils.ConnectorProperties);
				}
				else if (rangeAreaLabel.Kind == RangeAreaLabelKind.MaxValueLabel)
					filteredProperties.Add(minValueAnglePropertyName);
				else if (rangeAreaLabel.Kind == RangeAreaLabelKind.MinValueLabel)
					filteredProperties.Add(maxValueAnglePropertyName);
				if (rangeAreaLabel.ResolveOverlappingMode == ResolveOverlappingMode.JustifyAllAroundPoint) {
					filteredProperties.Add(minValueAnglePropertyName);
					filteredProperties.Add(maxValueAnglePropertyName);
				}
			}
			return filteredProperties;
		}
	}
	public class RangeArea3DSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(RangeArea3DSeriesLabel); } }
		protected override List<string> GetFilteredProperties(SeriesLabelBase label) {
			List<string> filteredProperties = base.GetFilteredProperties(label);
			RangeArea3DSeriesLabel rangeAreaLabel = label as RangeArea3DSeriesLabel;
			if (rangeAreaLabel != null) {
				if (rangeAreaLabel.Kind == RangeAreaLabelKind.OneLabel || rangeAreaLabel.Kind == RangeAreaLabelKind.TwoLabels) {
					filteredProperties.AddRange(FilterPropertiesUtils.ConnectorProperties);
				}
			}
			return filteredProperties;
		}
	}
	public class FunnelSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(FunnelSeriesLabel); } }
		protected override List<string> GetFilteredProperties(SeriesLabelBase label) {
			List<string> filteredProperties = new List<string>();
			FunnelSeriesLabel funnelLabel = label as FunnelSeriesLabel;
			if (funnelLabel != null) {
				if (funnelLabel.Position == FunnelSeriesLabelPosition.Center)
					filteredProperties.AddRange(FilterPropertiesUtils.ConnectorProperties);
				filteredProperties.Add("ResolveOverlappingMode");
				filteredProperties.Add("ResolveOverlappingMinIndent");
			}
			return filteredProperties;
		}
	}
	public class Funnel3DSeriesLabelTypeConverter : FunnelSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(Funnel3DSeriesLabel); } }
	}
	public class BarSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		const string indentPropertyName = "Indent";
		protected override Type LabelType { get { return typeof(BarSeriesLabel); } }
		protected override List<string> GetFilteredProperties(SeriesLabelBase label) {
			List<string> filteredProperties = new List<string>();
			BarSeriesLabel barLabel = label as BarSeriesLabel;
			if (barLabel != null) {
				if (barLabel.Position == BarSeriesLabelPosition.Top || barLabel.Position == BarSeriesLabelPosition.Center)
					filteredProperties.Add(indentPropertyName);
			}
			return filteredProperties;
		}
	}
	public class SideBySideBarSeriesLabelTypeConverter : BarSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(SideBySideBarSeriesLabel); } }
		protected override List<string> GetFilteredProperties(SeriesLabelBase label) {
			List<string> filteredProperties = base.GetFilteredProperties(label);
			SideBySideBarSeriesLabel barLabel = label as SideBySideBarSeriesLabel;
			if (barLabel != null) {
				if (barLabel.Position != BarSeriesLabelPosition.Top)
					filteredProperties.AddRange(FilterPropertiesUtils.ConnectorProperties);
			}
			return filteredProperties;
		}
	}
	public class StackedBarSeriesLabelTypeConverter : BarSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(StackedBarSeriesLabel); } }
	}
	public class FullStackedBarSeriesLabelTypeConverter : BarSeriesLabelTypeConverter {
		protected override Type LabelType { get { return typeof(FullStackedBarSeriesLabel); } }
	}
	public class FullStackedBar3DSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(FullStackedBar3DSeriesLabel); } }
	}
	public class Bar3DSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(Bar3DSeriesLabel); } }
	}
	public class StackedArea3DSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(StackedArea3DSeriesLabel); } }
	}
	public class FullStackedAreaSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(FullStackedAreaSeriesLabel); } }
	}
	public class FullStackedArea3DSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(FullStackedArea3DSeriesLabel); } }
	}
	public class Area3DSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(Area3DSeriesLabel); } }
	}
	public class StackedBar3DSeriesLabelTypeConverter : SeriesLabelBaseTypeConverter {
		protected override Type LabelType { get { return typeof(StackedBar3DSeriesLabel); } }
	}
}
