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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using DevExpress.Utils.Design;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Design {
	public class ChartElementPropertyDescriptor : PropertyDescriptor {
		static public PropertyDescriptor GetValidPropertyDescriptor(PropertyDescriptor desc, object value) {
			if (value == null || desc is ReadOnlyPropertyDescriptor)
				return desc;
			object[] attr = value.GetType().GetCustomAttributes(typeof(TypeConverterAttribute), false);
			if (attr.Length == 0)
				return desc;
			TypeConverterAttribute typeConverterAttribute = attr[0] as TypeConverterAttribute;
			return typeConverterAttribute == null ? desc :
				new ChartElementPropertyDescriptor(desc, typeConverterAttribute.ConverterTypeName);
		}
		static public PropertyDescriptor GetPropertyDescriptorWithAttributes(PropertyDescriptor desc, params Attribute[] additionalAttributes) {
			ChartElementPropertyDescriptor newDesc = new ChartElementPropertyDescriptor(desc, String.Empty);
			newDesc.additionalAttributes = additionalAttributes;
			return newDesc;
		}
		PropertyDescriptor defaultDescriptor;
		TypeConverter typeConverter;
		string typeConverterName;
		Attribute[] additionalAttributes;
		public override TypeConverter Converter {
			get {
				if (typeConverter == null && !String.IsNullOrEmpty(typeConverterName)) {
					Type converterType = GetTypeFromName(typeConverterName);
					if (converterType != null)
						typeConverter = CreateInstance(converterType) as TypeConverter;
				}
				return typeConverter == null ? defaultDescriptor.Converter : typeConverter;
			}
		}
		public override AttributeCollection Attributes {
			get {
				AttributeCollection attributes = defaultDescriptor.Attributes;
				if (additionalAttributes == null)
					return attributes;
				List<Attribute> newAttributes = new List<Attribute>(attributes.Count + additionalAttributes.Length);
				foreach (Attribute attribute in attributes) {
					Type attributeType = attribute.GetType();
					bool found = false;
					foreach (Attribute additionalAttribute in additionalAttributes)
						if (additionalAttribute.GetType() == attributeType) {
							found = true;
							break;
						}
					if (!found)
						newAttributes.Add(attribute);
				}
				newAttributes.AddRange(additionalAttributes);
				return new AttributeCollection(newAttributes.ToArray());
			}
		}
		public override string Category { get { return defaultDescriptor.Category; } }
		public override Type ComponentType { get { return defaultDescriptor.ComponentType; } }
		public override string Description { get { return defaultDescriptor.Description; } }
		public override bool DesignTimeOnly { get { return defaultDescriptor.DesignTimeOnly; } }
		public override string DisplayName { get { return defaultDescriptor.DisplayName; } }
		public override bool IsBrowsable { get { return defaultDescriptor.IsBrowsable; } }
		public override bool IsLocalizable { get { return defaultDescriptor.IsLocalizable; } }
		public override bool IsReadOnly { get { return defaultDescriptor.IsReadOnly; } }
		public override string Name { get { return defaultDescriptor.Name; } }
		public override Type PropertyType { get { return defaultDescriptor.PropertyType; } }
		ChartElementPropertyDescriptor(PropertyDescriptor desc, string typeConverterName)
			: base(desc) {
			this.defaultDescriptor = desc;
			this.typeConverterName = typeConverterName;
		}
		public override void AddValueChanged(object component, EventHandler handler) {
			defaultDescriptor.AddValueChanged(component, handler);
		}
		public override bool CanResetValue(object component) {
			return defaultDescriptor.CanResetValue(component);
		}
		public override bool Equals(object obj) {
			return defaultDescriptor.Equals(obj);
		}
		public override PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter) {
			return defaultDescriptor.GetChildProperties(instance, filter);
		}
		public override object GetEditor(Type editorBaseType) {
			if (additionalAttributes != null)
				foreach (Attribute attribute in additionalAttributes) {
					EditorAttribute editorAttribute = attribute as EditorAttribute;
					if (editorAttribute != null) {
						if (editorBaseType != null) {
							string baseTypeName = editorAttribute.EditorBaseTypeName;
							if (!String.IsNullOrEmpty(baseTypeName)) {
								Type baseType = Type.GetType(baseTypeName);
								if (baseType != editorBaseType)
									continue;
							}
						}
						string typeName = editorAttribute.EditorTypeName;
						if (!String.IsNullOrEmpty(typeName)) {
							Type editorType = Type.GetType(typeName);
							if (editorType != null) {
								object editor = Activator.CreateInstance(editorType);
								if (editor != null)
									return editor;
							}
						}
					}
				}
			return defaultDescriptor.GetEditor(editorBaseType);
		}
		public override int GetHashCode() {
			return defaultDescriptor.GetHashCode();
		}
		public override object GetValue(object component) {
			return defaultDescriptor.GetValue(component);
		}
		public override void RemoveValueChanged(object component, EventHandler handler) {
			defaultDescriptor.RemoveValueChanged(component, handler);
		}
		public override void ResetValue(object component) {
			defaultDescriptor.ResetValue(component);
		}
		public override void SetValue(object component, object value) {
			defaultDescriptor.SetValue(component, value);
		}
		public override bool ShouldSerializeValue(object component) {
			return defaultDescriptor.ShouldSerializeValue(component);
		}
	}
	public class ChartTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			object obj = TypeConverterHelper.GetElement(value);
			Chart chart = obj as Chart;
			if (chart == null) {
				IChartContainer container = obj as IChartContainer;
				if (container != null)
					chart = container.Chart;
			}
			if (chart == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>(collection.Count);
			foreach (PropertyDescriptor desc in collection) {
				switch (desc.Name) {
					case "SeriesSelectionMode":
						if (chart.SelectionMode != ElementSelectionMode.None)
							descs.Add(desc);
						break;
					case "SeriesDataMember":
						PivotGridDataSourceOptions pivotOptions = chart.DataContainer.PivotGridDataSourceOptions;
						if (pivotOptions.HasDataSource && pivotOptions.AutoBindingSettingsEnabled)
							descs.Add(new ReadOnlyPropertyDescriptor(desc));
						else
							descs.Add(desc);
						break;
					case "Diagram":
						descs.Add(ChartElementPropertyDescriptor.GetValidPropertyDescriptor(desc, chart.Diagram));
						break;
					default:
						descs.Add(desc);
						break;
				}
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class PivotGridDataSourceOptionsTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			PivotGridDataSourceOptions options = value as PivotGridDataSourceOptions;
			if (options == null)
				return collection;
			List<string> filteredProperties = new List<string>();
			if (options.HasPivotGrid) {
				if (!options.PivotGrid.SelectionSupported) {
					filteredProperties.Add("SelectionOnly");
					filteredProperties.Add("UpdateDelay");
				}
				else if (!options.PivotGrid.SelectionOnly)
					filteredProperties.Add("UpdateDelay");
				if (!options.PivotGrid.SinglePageSupported)
					filteredProperties.Add("SinglePageOnly");
			}
			else {
				filteredProperties.Add("RetrieveDataByColumns");
				filteredProperties.Add("RetrieveEmptyCells");
				filteredProperties.Add("SelectionOnly");
				filteredProperties.Add("SinglePageOnly");
				filteredProperties.Add("RetrieveColumnTotals");
				filteredProperties.Add("RetrieveColumnGrandTotals");
				filteredProperties.Add("RetrieveColumnCustomTotals");
				filteredProperties.Add("RetrieveRowTotals");
				filteredProperties.Add("RetrieveRowGrandTotals");
				filteredProperties.Add("RetrieveRowCustomTotals");
				filteredProperties.Add("MaxSeriesCount");
				filteredProperties.Add("MaxPointCountInSeries");
				filteredProperties.Add("UpdateDelay");
			}
			return FilterPropertiesUtils.FilterProperties(collection, filteredProperties.ToArray());
		}
	}
	public class AxisBaseTypeConverter : ExpandableObjectConverter {
		static List<string> GetFilteredProperties(object value) {
			List<string> filteredProperties = new List<string>();
			AxisBase axis = TypeConverterHelper.GetElement<AxisBase>(value);
			if (axis != null) {
				if (axis.ScaleType != ActualScaleType.Numerical)
					filteredProperties.AddRange(new string[] { "NumericOptions", "Logarithmic", "NumericScaleOptions" });
				if (axis.ScaleType != ActualScaleType.Numerical || !axis.Logarithmic)
					filteredProperties.Add("LogarithmicBase");
				if (axis.ScaleType == ActualScaleType.DateTime) {
					AxisXBase axisX = axis as AxisXBase;
					if (axisX != null) {
						if (axisX.ScrollingZoomingEnabled || (axisX.Diagram != null && axisX.Diagram is GanttDiagram))
							filteredProperties.Add("DateTimeScaleMode");
					}
				}
				else
					filteredProperties.AddRange(new string[] { "DateTimeScaleMode", "DateTimeScaleOptions" });
			}
			return filteredProperties;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			return FilterPropertiesUtils.FilterProperties(collection, GetFilteredProperties(value));
		}
	}
	public class AxisXTypeConverter : AxisBaseTypeConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			AxisX axis = TypeConverterHelper.GetElement<AxisX>(value);
			if (axis == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				if ((desc.Name == "ScaleBreaks" || desc.Name == "DateTimeGridAlignment" || desc.Name == "DateTimeMeasureUnit")
					&& PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(axis.Diagram.Chart.DataContainer.PivotGridDataSourceOptions, axis, false))
					descs.Add(new ReadOnlyPropertyDescriptor(desc));
				else
					descs.Add(desc);
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class ScaleBreakOptionsTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			ScaleBreakOptions options = TypeConverterHelper.GetElement<ScaleBreakOptions>(value);
			if (options == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			AxisBase axis = options.Owner as AxisBase;
			if (axis == null)
				return collection;
			foreach (PropertyDescriptor desc in collection) {
				if ((desc.Name == "Style" || desc.Name == "SizeInPixels")
					&& PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(axis.Diagram.Chart.DataContainer.PivotGridDataSourceOptions, axis, false))
					descs.Add(new ReadOnlyPropertyDescriptor(desc));
				else
					descs.Add(desc);
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public abstract class SecondaryAxisTypeConverter : AxisBaseTypeConverter {
		protected abstract Type SecondaryAxisType { get; }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(SecondaryAxisType.GetConstructor(new Type[0]), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class SecondaryAxisXTypeConverter : SecondaryAxisTypeConverter {
		protected override Type SecondaryAxisType { get { return typeof(SecondaryAxisX); } }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			SecondaryAxisX axis = TypeConverterHelper.GetElement<SecondaryAxisX>(value);
			if (axis == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				if ((desc.Name == "ScaleBreaks" || desc.Name == "DateTimeGridAlignment" || desc.Name == "DateTimeMeasureUnit")
					&& PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(axis.Diagram.Chart.DataContainer.PivotGridDataSourceOptions, axis, false))
					descs.Add(new ReadOnlyPropertyDescriptor(desc));
				else
					descs.Add(desc);
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class SecondaryAxisYTypeConverter : SecondaryAxisTypeConverter {
		protected override Type SecondaryAxisType { get { return typeof(SecondaryAxisY); } }
	}
	public class SwiftPlotDiagramSecondaryAxisXTypeConverter : SecondaryAxisTypeConverter {
		protected override Type SecondaryAxisType { get { return typeof(SwiftPlotDiagramSecondaryAxisX); } }
	}
	public class SwiftPlotDiagramSecondaryAxisYTypeConverter : SecondaryAxisTypeConverter {
		protected override Type SecondaryAxisType { get { return typeof(SwiftPlotDiagramSecondaryAxisY); } }
	}
	public class VisualRangeTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>(collection.Count);
			foreach (PropertyDescriptor desc in collection)
				descs.Add(desc);
			List<string> filteredProperties = new List<string>();
			VisualRange range = TypeConverterHelper.GetElement<VisualRange>(value);
			IAxisData axis = range.RangeData.Axis as IAxisData;
			filteredProperties.AddRange(new string[] { "MinValueInternal", "MaxValueInternal" });
			if (axis.IsArgumentAxis && (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime) && (axis.DateTimeScaleOptions.ScaleMode != ScaleModeNative.Manual))
				filteredProperties.AddRange(new string[] { "Auto", "MinValue", "MaxValue", "MinValueInternal", "MaxValueInternal" });
			return FilterPropertiesUtils.FilterProperties(new PropertyDescriptorCollection(descs.ToArray()), filteredProperties);
		}
	}
	public class WholeRangeTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>(collection.Count);
			foreach (PropertyDescriptor desc in collection)
				descs.Add(desc);
			List<string> filteredProperties = new List<string>();
			WholeRange range = TypeConverterHelper.GetElement<WholeRange>(value);
			IAxisData axis = range.RangeData.Axis as IAxisData;
			filteredProperties.AddRange(new string[] { "MinValueInternal", "MaxValueInternal" });
			if (axis.IsArgumentAxis || !range.Auto || axis.AxisScaleTypeMap.ScaleType != ActualScaleType.Numerical)
				filteredProperties.Add("AlwaysShowZeroLevel");
			if (axis.IsArgumentAxis && (axis.AxisScaleTypeMap.ScaleType == ActualScaleType.DateTime) && (axis.DateTimeScaleOptions.ScaleMode != ScaleModeNative.Manual))
				filteredProperties.AddRange(new string[] { "Auto", "MinValue", "MaxValue", "MinValueInternal", "MaxValueInternal" });
			return FilterPropertiesUtils.FilterProperties(new PropertyDescriptorCollection(descs.ToArray()), filteredProperties);
		}
	}
	public class DateTimeOptionsTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			DateTimeOptions options = null;
			if (value is DateTimeOptions)
				options = (DateTimeOptions)value;
			else if (value is FakeComponent)
				options = ((FakeComponent)value).Object as DateTimeOptions;
			if (options == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				if (desc.Name == "FormatString") {
					if (options.Format == DateTimeFormat.Custom) {
						if (options.AllowDesignTimeEditing())
							descs.Add(desc);
						else
							descs.Add(new ReadOnlyPropertyDescriptor(desc));
					}
				}
				else if (desc.Name == "Format" && !options.AllowDesignTimeEditing())
					descs.Add(new ReadOnlyPropertyDescriptor(desc));
				else
					descs.Add(desc);
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class NumericOptionsTypeConverter : ExpandableObjectConverter {
		static bool ShouldHidePrecisionProperty(object value) {
			NumericOptions options;
			if (value is NumericOptions)
				options = (NumericOptions)value;
			else if (value is FakeComponent)
				options = ((FakeComponent)value).Object as NumericOptions;
			else
				options = null;
			return options != null && options.Format == NumericFormat.General;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			return ShouldHidePrecisionProperty(value) ?
				FilterPropertiesUtils.FilterProperties(collection, new string[] { "Precision" }) :
				collection;
		}
	}
	public class PercentOptionsTypeConverter : ExpandableObjectConverter {
		static bool ShouldHideValuePercentPrecisionProperty(object value) {
			PercentOptions options;
			if (value is PercentOptions)
				options = (PercentOptions)value;
			else if (value is FakeComponent)
				options = ((FakeComponent)value).Object as PercentOptions;
			else
				options = null;
			return options != null && !options.ValueAsPercent;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			return ShouldHideValuePercentPrecisionProperty(value) ?
				FilterPropertiesUtils.FilterProperties(collection, new string[] { "PercentageAccuracy" }) :
				collection;
		}
	}
	public class AxisTitleTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			AxisTitle title = TypeConverterHelper.GetElement<AxisTitle>(value);
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			if (title != null) {
				foreach (PropertyDescriptor desc in collection) {
					if (desc.Name == "Text" && PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(title.Owner.ChartContainer.Chart.DataContainer.PivotGridDataSourceOptions, (AxisBase)title.Owner, true))
						descs.Add(new ReadOnlyPropertyDescriptor(desc));
					else
						descs.Add(desc);
				}
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class TopNOptionsTypeConverter : ExpandableObjectConverter {
		const string ModePropertyName = "Mode";
		const string CountPropertyName = "Count";
		const string ThresholdValuePropertyName = "ThresholdValue";
		const string ThresholdPercentPropertyName = "ThresholdPercent";
		const string ShowOthersPropertyName = "ShowOthers";
		const string OthersArgumentPropertyName = "OthersArgument";
		List<string> GetFilteredProperties(object value) {
			TopNOptions options = TypeConverterHelper.GetElement<TopNOptions>(value);
			List<string> filteredProperties = new List<string>();
			if (options != null)
				if (options.Enabled) {
					switch (options.Mode) {
						case TopNMode.Count:
							filteredProperties.AddRange(new string[] { ThresholdValuePropertyName, ThresholdPercentPropertyName });
							break;
						case TopNMode.ThresholdValue:
							filteredProperties.AddRange(new string[] { CountPropertyName, ThresholdPercentPropertyName });
							break;
						case TopNMode.ThresholdPercent:
							filteredProperties.AddRange(new string[] { CountPropertyName, ThresholdValuePropertyName });
							break;
					}
					SeriesBase series = options.Owner as SeriesBase;
					if (series != null && !series.ShouldUseTopNOthers)
						filteredProperties.AddRange(new string[] { ShowOthersPropertyName, OthersArgumentPropertyName });
					else if (!options.ShowOthers)
						filteredProperties.Add(OthersArgumentPropertyName);
				}
				else
					filteredProperties.AddRange(new string[] { ModePropertyName, CountPropertyName, 
						ThresholdValuePropertyName, ThresholdPercentPropertyName, ShowOthersPropertyName, OthersArgumentPropertyName });
			return filteredProperties;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesUtils.FilterProperties(base.GetProperties(context, value, attributes), GetFilteredProperties(value));
		}
	}
	public class BaseMarkerTypeConverter : ExpandableObjectConverter {
		static bool ShouldHideStarPointCountProperty(object value) {
			MarkerBase marker = TypeConverterHelper.GetElement<MarkerBase>(value);
			return marker != null && marker.Kind != MarkerKind.Star;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			return ShouldHideStarPointCountProperty(value) ?
				FilterPropertiesUtils.FilterProperties(collection, new string[] { "StarPointCount" }) :
				collection;
		}
	}
	public class StripLimitTypeConverter : ExpandableObjectConverter {
		static bool ShouldHideAxisValueProperty(object value) {
			StripLimit limit = TypeConverterHelper.GetElement<StripLimit>(value);
			return limit != null && !limit.Enabled;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			return ShouldHideAxisValueProperty(value) ?
				FilterPropertiesUtils.FilterProperties(collection, new string[] { "AxisValue" }) :
				collection;
		}
	}
	public class DashStyleTypeConterter : EnumTypeConverter {
		public DashStyleTypeConterter(Type type)
			: base(type) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			LineStyle lineStyle = TypeConverterHelper.GetElement<LineStyle>(context.Instance);
			if (lineStyle == null)
				return collection;
			if (!(lineStyle.Owner is ConstantLine)) {
				ArrayList list = new ArrayList();
				foreach (DashStyle style in collection)
					if (style != DashStyle.Empty)
						list.Add(style);
				collection = new StandardValuesCollection(list);
			}
			return collection;
		}
	}
	public class LineJoinTypeConverter : LineJoinEnumTypeConverter {
		public LineJoinTypeConverter(Type type)
			: base(type) {
		}
	}
	public class LegendTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			Legend legend = TypeConverterHelper.GetElement<Legend>(value);
			if (legend == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				switch (desc.Name) {
					case "EquallySpacedItems":
						if (legend.Direction != LegendDirection.TopToBottom && legend.Direction != LegendDirection.BottomToTop)
							descs.Add(desc);
						break;
					case "MaxVerticalPercentage":
					case "MaxHorizontalPercentage":
						if (legend.Chart.DataContainer.PivotGridDataSourceOptions.HasDataSource && legend.Chart.DataContainer.PivotGridDataSourceOptions.AutoLayoutSettingsEnabled)
							descs.Add(new ReadOnlyPropertyDescriptor(desc));
						else
							descs.Add(desc);
						break;
					case "Visible":
						if (PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabledForSimpleView(legend.Chart.DataContainer.PivotGridDataSourceOptions))
							descs.Add(new ReadOnlyPropertyDescriptor(desc));
						else
							descs.Add(desc);
						break;
					case "UseCheckBoxes":
						if (legend.Chart.Container.ControlType != ChartContainerType.XRControl)
							descs.Add(desc);
						break;
					default:
						descs.Add(desc);
						break;
				}
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class PointOptionsTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			PointOptions options = null;
			if (value is PointOptions)
				options = (PointOptions)value;
			else if (value is FakeComponent)
				options = ((FakeComponent)value).Object as PointOptions;
			if (options == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor desc in collection) {
				switch (desc.Name) {
					case "ArgumentNumericOptions":
						if (options.SeriesBase.ActualArgumentScaleType == ScaleType.Numerical)
							descs.Add(desc);
						break;
					case "ArgumentDateTimeOptions":
						if (options.SeriesBase.ActualArgumentScaleType == ScaleType.DateTime)
							descs.Add(desc);
						break;
					case "ValueNumericOptions":
						if (options.SeriesBase.ValueScaleType == ScaleType.Numerical)
							descs.Add(desc);
						break;
					case "ValueDateTimeOptions":
						if (options.SeriesBase.ValueScaleType == ScaleType.DateTime)
							descs.Add(desc);
						break;
					case "PointView":
						SeriesBase series = options.SeriesBase;
						if (series != null && PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabledForSimpleView(series.View.Chart.DataContainer.PivotGridDataSourceOptions, series))
							descs.Add(new ReadOnlyPropertyDescriptor(desc));
						else
							descs.Add(desc);
						break;
					default:
						descs.Add(desc);
						break;
				}
			}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ||
				base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				ConstructorInfo ci = typeof(PointOptions).GetConstructor(new Type[] { });
				return new InstanceDescriptor(ci, null, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public abstract class RangePointOptionsTypeConverter : PointOptionsTypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ||
				base.CanConvertTo(context, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			RangePointOptions options = null;
			if (value is RangePointOptions)
				options = (RangePointOptions)value;
			else if (value is FakeComponent)
				options = ((FakeComponent)value).Object as RangePointOptions;
			if (options == null)
				return collection;
			List<string> filteredProperties = new List<string>();
			if (options.SeriesBase.ValueScaleType != ScaleType.DateTime)
				filteredProperties.Add("ValueDurationFormat");
			else if (options.ValueAsDuration) {
				filteredProperties.Add("ValueDateTimeOptions");
				if (options.ValueDurationFormat != TimeSpanFormat.Standard)
					filteredProperties.Remove("ValueNumericOptions");
			}
			else
				filteredProperties.Add("ValueDurationFormat");
			return FilterPropertiesUtils.FilterProperties(collection, filteredProperties);
		}
	}
	public class RangeAreaPointOptionsTypeConverter : RangePointOptionsTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				ConstructorInfo ci = typeof(RangeAreaPointOptions).GetConstructor(new Type[] { });
				return new InstanceDescriptor(ci, null, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class RangeBarPointOptionsTypeConverter : RangePointOptionsTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				ConstructorInfo ci = typeof(RangeBarPointOptions).GetConstructor(new Type[] { });
				return new InstanceDescriptor(ci, null, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class HatchStyleTypeConverter : HatchStyleEnumTypeConverter {
		public HatchStyleTypeConverter()
			: base(typeof(System.Drawing.Drawing2D.HatchStyle)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			ArrayList list = new ArrayList();
			foreach (object val in collection)
				if (list.IndexOf(val) == -1)
					list.Add(val);
			return new StandardValuesCollection(list);
		}
	}
	public class ValueScaleTypeTypeConverter : EnumTypeConverter {
		public ValueScaleTypeTypeConverter()
			: base(typeof(ScaleType)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			if (context == null)
				return collection;
			SeriesBase series = TypeConverterHelper.GetElement<SeriesBase>(context.Instance);
			bool dateTimeScaleSupported = series == null || series.View == null || series.View.DateTimeValuesSupported;
			ArrayList list = new ArrayList();
			foreach (ScaleType scaleType in collection)
				if (scaleType == ScaleType.Numerical || (scaleType == ScaleType.DateTime && dateTimeScaleSupported))
					list.Add(scaleType);
			return new StandardValuesCollection(list);
		}
	}
	public class ArgumentScaleTypeTypeConverter : EnumTypeConverter {
		public ArgumentScaleTypeTypeConverter()
			: base(typeof(ScaleType)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			if (context == null)
				return collection;
			SeriesBase series = TypeConverterHelper.GetElement<SeriesBase>(context.Instance);
			bool nonNumericScaleSupported = series == null || series.View == null || series.View.NonNumericArgumentSupported;
			ArrayList list = new ArrayList();
			foreach (ScaleType scaleType in collection)
				if (scaleType == ScaleType.Numerical || nonNumericScaleSupported)
					list.Add(scaleType);
			return new StandardValuesCollection(list);
		}
	}
	public class DataAdapterTypeConverter : ReferenceConverter {
		public DataAdapterTypeConverter()
			: base(typeof(object)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			ArrayList list = new ArrayList();
			foreach (object val in collection)
				if (val != null && Data.Native.BindingHelper.ConvertToIDataAdapter(val) != null)
					list.Add(val);
			list.Add(null);
			return new StandardValuesCollection(list);
		}
	}
	public class DataFilterConditionTypeConverter : EnumTypeConverter {
		public DataFilterConditionTypeConverter()
			: base(typeof(DataFilterCondition)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection coll = base.GetStandardValues(context);
			DataFilter filter = TypeConverterHelper.GetElement<DataFilter>(context.Instance);
			return typeof(IComparable).IsAssignableFrom(filter.DataType) ?
				coll :
				new StandardValuesCollection(new DataFilterCondition[] { 
					DataFilterCondition.Equal, DataFilterCondition.NotEqual });
		}
	}
	public class SeriesPointKeyConverter : EnumTypeConverter {
		string[] GetNames(ITypeDescriptorContext context) {
			if (context == null || context.Instance == null)
				return null;
			object obj = TypeConverterHelper.GetElement(context.Instance);
			SeriesBase series = obj as SeriesBase;
			if (series == null) {
				SeriesPointFilter filter = obj as SeriesPointFilter;
				if (filter != null)
					series = filter.Series;
			}
			if (series == null || series.View == null)
				return null;
			string[] names = new string[series.View.PointDimension + 1];
			names[0] = ChartLocalizer.GetString(ChartStringId.ArgumentMember);
			for (int i = 1; i <= series.View.PointDimension; i++)
				names[i] = series.View.GetValueCaption(i - 1);
			return names;
		}
		public SeriesPointKeyConverter()
			: base(typeof(SeriesPointKey)) {
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			ArrayList list = new ArrayList();
			for (int i = 0; i < GetNames(context).Length; i++)
				list.Add((SeriesPointKey)i);
			return new StandardValuesCollection(list);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if ((value is SeriesPointKey) && destinationType == typeof(string) && context != null) {
				string[] names = GetNames(context);
				return names == null ? String.Empty : names[(int)value];
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string name = value as string;
			if (name != null) {
				string[] names = GetNames(context);
				for (int i = 0; i < names.Length; i++)
					if (names[i] == name)
						return (SeriesPointKey)i;
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class PaletteTypeConverter : TypeConverter {
		string[] GetStandardValues(PaletteRepository paletteRepository) {
			return paletteRepository.PaletteNames;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			IChartContainer chartContainer = context.Instance as IChartContainer;
			PaletteRepository paletteRepository = chartContainer == null ? null : chartContainer.Chart.PaletteRepository;
			return paletteRepository == null ? base.GetStandardValues(context) :
				new StandardValuesCollection(GetStandardValues(paletteRepository));
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			string name = value as string;
			if (name != null && destinationType == typeof(string)) {
				IChartContainer container = context.Instance as IChartContainer;
				PropertyDescriptor pd = context.PropertyDescriptor;
				if (container != null && pd != null)
					return (pd.Name == "IndicatorsPaletteName" ? container.Chart.IndicatorsPaletteRepository : container.Chart.PaletteRepository)[name].DisplayName;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class AppearanceTypeConverter : TypeConverter {
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			IChartContainer chartContainer = context.Instance as IChartContainer;
			AppearanceRepository appearanceRepository = chartContainer == null ? null : chartContainer.Chart.AppearanceRepository;
			return appearanceRepository == null ? base.GetStandardValues(context) :
				new StandardValuesCollection(appearanceRepository.Names);
		}
	}
	public class AxisTypeConverter : TypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				Axis2D axis = value as Axis2D;
				if (axis != null)
					return axis.Name;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public abstract class AxisCoordinateAxisTypeConverter : AxisTypeConverter {
		XYDiagram2D GetDiagram(ITypeDescriptorContext context) {
			AxisCoordinate axisCoordinate = context.Instance as AxisCoordinate;
			return axisCoordinate == null ? null : axisCoordinate.AnchorPoint.XYDiagram;
		}
		protected abstract List<Axis2D> GetAxes(XYDiagram2D diagram);
		protected abstract Axis2D GetAxisByName(XYDiagram2D diagram, string name);
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			XYDiagram2D diagram = GetDiagram(context);
			return diagram == null ? base.GetStandardValues(context) : new StandardValuesCollection(GetAxes(diagram));
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string name = value as string;
			if (name != null) {
				XYDiagram2D diagram = GetDiagram(context);
				if (diagram != null)
					return GetAxisByName(diagram, name);
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class AxisXCoordinateAxisTypeConverter : AxisCoordinateAxisTypeConverter {
		protected override List<Axis2D> GetAxes(XYDiagram2D diagram) {
			return diagram.GetAllAxesX();
		}
		protected override Axis2D GetAxisByName(XYDiagram2D diagram, string name) {
			return diagram.FindAxisXByName(name);
		}
	}
	public class AxisYCoordinateAxisTypeConverter : AxisCoordinateAxisTypeConverter {
		protected override List<Axis2D> GetAxes(XYDiagram2D diagram) {
			return diagram.GetAllAxesY();
		}
		protected override Axis2D GetAxisByName(XYDiagram2D diagram, string name) {
			return diagram.FindAxisYByName(name);
		}
	}
	public class PaneTypeConverter : TypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				XYDiagramPaneBase pane = value as XYDiagramPaneBase;
				if (pane != null)
					return pane.Name;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class PaneAnchorPointPaneTypeConverter : PaneTypeConverter {
		XYDiagram2D GetDiagram(ITypeDescriptorContext context) {
			PaneAnchorPoint anchorPoint = context.Instance as PaneAnchorPoint;
			return anchorPoint != null ? anchorPoint.XYDiagram : null;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			XYDiagram2D diagram = GetDiagram(context);
			return diagram != null ? new StandardValuesCollection(diagram.GetAllPanes()) : base.GetStandardValues(context);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string name = value as string;
			if (name != null) {
				XYDiagram2D diagram = GetDiagram(context);
				if (diagram != null)
					return diagram.FindPaneByName(name);
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public abstract class FreePositionDockTargetTypeConverter : TypeConverter {
		protected abstract XYDiagram2D GetDiagram(ITypeDescriptorContext context);
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(AnnotationHelper.GetDockTargets(GetDiagram(context)));
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string name = value as string;
			if (name != null)
				return AnnotationHelper.GetDockTarget(name, GetDiagram(context));
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string))
				return AnnotationHelper.GetDockTargetName(value as IDockTarget);
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class AnnotationFreePositionDockTargetTypeConverter : FreePositionDockTargetTypeConverter {
		protected override XYDiagram2D GetDiagram(ITypeDescriptorContext context) {
			FreePosition shapePosition = context.Instance as FreePosition;
			return shapePosition != null ? CommonUtils.GetXYDiagram2D(shapePosition) : null;
		}
	}
	public class ToolTipFreePositionDockTargetTypeConverter : FreePositionDockTargetTypeConverter {
		protected override XYDiagram2D GetDiagram(ITypeDescriptorContext context) {
			ToolTipFreePosition toolTipPosition = context.Instance as ToolTipFreePosition;
			return toolTipPosition != null ? CommonUtils.GetXYDiagram2D(toolTipPosition) : null;
		}
	}
	public class CrosshairFreePositionDockTargetTypeConverter : FreePositionDockTargetTypeConverter {
		protected override XYDiagram2D GetDiagram(ITypeDescriptorContext context) {
			CrosshairFreePosition position = context.Instance as CrosshairFreePosition;
			return position != null ? CommonUtils.GetXYDiagram2D(position) : null;
		}
	}
	public abstract class Diagram3DTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ||
				base.CanConvertTo(context, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			Diagram3D diagram = TypeConverterHelper.GetElement<Diagram3D>(value);
			if (diagram == null)
				return collection;
			List<string> filteredProperties = new List<string>();
			if (diagram.RotationType != RotationType.UseAngles)
				filteredProperties.AddRange(new string[] { "RotationAngleX", "RotationAngleY", "RotationAngleZ", "RotationOrder" });
			if (diagram.ChartContainer == null || diagram.ChartContainer.ControlType != ChartContainerType.WinControl)
				filteredProperties.AddRange(new string[] { "RuntimeRotation", "RuntimeZooming", "RuntimeScrolling", "ScrollingOptions", "ZoomingOptions", "RotationOptions" });
			else {
				if (!diagram.RuntimeScrolling)
					filteredProperties.Add("ScrollingOptions");
				if (!diagram.RuntimeZooming)
					filteredProperties.Add("ZoomingOptions");
				if (!diagram.RuntimeRotation)
					filteredProperties.Add("RotationOptions");
			}
			return filteredProperties.Count == 0 ? collection : FilterPropertiesUtils.FilterProperties(collection, filteredProperties.ToArray());
		}
	}
	public class XYDiagram3DTypeConverter : Diagram3DTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				ConstructorInfo ci = typeof(XYDiagram3D).GetConstructor(new Type[] { });
				return new InstanceDescriptor(ci, null, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class SimpleDiagram3DTypeConverter : Diagram3DTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				ConstructorInfo ci = typeof(SimpleDiagram3D).GetConstructor(new Type[] { });
				return new InstanceDescriptor(ci, null, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			SimpleDiagram3D diagram = TypeConverterHelper.GetElement<SimpleDiagram3D>(value);
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			if (diagram != null && diagram.Chart.AutoLayout) {
				List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
				for (int i = 0; i < collection.Count; i++) {
					PropertyDescriptor desc = collection[i];
					if (desc.Name != "Dimension" && desc.Name != "LayoutDirection")
						properties.Add(desc);
				}
				return new PropertyDescriptorCollection(properties.ToArray());
			}
			return collection;
		}
	}
	public class FunnelDiagram3DTypeConverter : Diagram3DTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				ConstructorInfo ci = typeof(FunnelDiagram3D).GetConstructor(new Type[] { });
				return new InstanceDescriptor(ci, null, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public abstract class AxisValueTypeConverterBase : StringConverter {
		protected abstract AxisBase GetAxis(ITypeDescriptorContext context);
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			return GetAxis(context).ConvertBasedOnScaleType(value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string) && (value is DateTime)) {
				AxisBase axis = GetAxis(context);
				if (axis != null) {
					PatternParser patternParser = new PatternParser(axis.ActualLabel.ActualTextPattern, (IPatternHolder)axis.ActualLabel);
					patternParser.SetContext(value);
					return patternParser.GetText();
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class AxisValueTypeConverter : AxisValueTypeConverterBase {
		protected override AxisBase GetAxis(ITypeDescriptorContext context) {
			object obj = TypeConverterHelper.GetElement(context.Instance);
			IAxisValueContainer container = obj as IAxisValueContainer;
			if (container == null) {
				AxisRange range = obj as AxisRange;
				if (range == null) {
					AxisCoordinate axisCoordinate = obj as AxisCoordinate;
					if (axisCoordinate != null)
						return axisCoordinate.ActualAxis;
				}
				else
					return (AxisBase)range.Owner;
			}
			else if (container.Axis != null)
				return (AxisBase)container.Axis;
			return null;
		}
	}
	public class FinancialIndicatorPointArgumentTypeConverter : AxisValueTypeConverterBase {
		protected override AxisBase GetAxis(ITypeDescriptorContext context) {
			FinancialIndicatorPoint financialIndicatorPoint = TypeConverterHelper.GetElement<FinancialIndicatorPoint>(context.Instance);
			if (financialIndicatorPoint != null) {
				XYDiagram2DSeriesViewBase view = financialIndicatorPoint.FinancialIndicator.View;
				if (view != null)
					return view.ActualAxisX;
			}
			return null;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string) && (value is DateTime)) {
				FinancialIndicatorPoint financialIndicatorPoint = TypeConverterHelper.GetElement<FinancialIndicatorPoint>(context.Instance);
				if (financialIndicatorPoint != null) {
					SeriesBase series = ((SeriesBase)financialIndicatorPoint.Owner.Owner.Owner);
					string format = "G";
					if (series.Label != null)
						format = PatternUtils.GetArgumentFormat(series.Label.ActualTextPattern);
					return PatternUtils.Format(value, format);
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class ScaleBreakEdgeTypeConverter : AxisValueTypeConverterBase {
		protected override AxisBase GetAxis(ITypeDescriptorContext context) {
			ScaleBreak scaleBreak = TypeConverterHelper.GetElement<ScaleBreak>(context.Instance);
			return scaleBreak != null ? scaleBreak.Axis : null;
		}
	}
	public class RangeValueTypeConverter : StringConverter {
		RangeDataBase GetRangeData(ITypeDescriptorContext context) {
			Range range = TypeConverterHelper.GetElement<Range>(context.Instance);
			if (range != null)
				return range.RangeData;
			return null;
		}
		void ThrowIncorrectPropertyValueException(object propertyValue, string propertyName) {
			string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyValue),
				propertyValue == null ? String.Empty : propertyValue.ToString(), propertyName);
			throw new ArgumentException(message);
		}
		object ConvertToNativeValue(IAxisData axis, object value, string propertyName) {
			CultureInfo culture = CultureInfo.CurrentCulture;
			if (value == null)
				ThrowIncorrectPropertyValueException(value, propertyName);
			AxisScaleTypeMap map = axis.AxisScaleTypeMap;
			object nativeValue = map.ConvertValue(value, culture);
			if (!map.IsCompatible(nativeValue))
				ThrowIncorrectPropertyValueException(value, propertyName);
			return nativeValue;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			RangeDataBase data = GetRangeData(context);
			IAxisData axis = data.Axis as IAxisData;
			return axis == null ? base.ConvertFrom(context, culture, value) : ConvertToNativeValue(axis, value, context.PropertyDescriptor.Name);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				RangeDataBase data = GetRangeData(context);
				IAxisData axis = data.Axis as IAxisData;
				if (axis != null) {
					if (value is DateTime) {
						PatternParser patternParser = new PatternParser(axis.Label.TextPattern, (IPatternHolder)axis.Label);
						patternParser.SetContext(value);
						return patternParser.GetText();
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public abstract class AxisRangeValueTypeConverter : StringConverter {
		IAxisData GetAxis(ITypeDescriptorContext context, out AxisRangeData data) {
			AxisRange range = context.Instance as AxisRange;
			if (range != null) {
				data = range.Data;
				return range.Axis;
			}
			ScrollingRange scrollingRange = context.Instance as ScrollingRange;
			if (scrollingRange != null) {
				data = scrollingRange.Data;
				return scrollingRange.Range.Axis;
			}
			data = null;
			return null;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			AxisRangeData data;
			IAxisData axis = GetAxis(context, out data);
			return axis == null ? base.ConvertFrom(context, culture, value) : data.ConvertToNativeValue(value, context.PropertyDescriptor.Name);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string)) {
				AxisRangeData data;
				IAxisData axis = GetAxis(context, out data);
				if (axis != null) {
					if (!CanConvertFromNativeValue(data))
						return "(undetermined)";
					if (value is DateTime) {
						PatternParser patternParser = new PatternParser(axis.Label.TextPattern, (IPatternHolder)axis.Label);
						patternParser.SetContext(value);
						return patternParser.GetText();
					}
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		protected abstract bool CanConvertFromNativeValue(AxisRangeData data);
	}
	public class AxisRangeMinValueTypeConverter : AxisRangeValueTypeConverter {
		protected override bool CanConvertFromNativeValue(AxisRangeData data) {
			return !data.ShouldSerializeMinValueInternal();
		}
	}
	public class AxisRangeMaxValueTypeConverter : AxisRangeValueTypeConverter {
		protected override bool CanConvertFromNativeValue(AxisRangeData data) {
			return !data.ShouldSerializeMaxValueInternal();
		}
	}
	public class AxisAlignmentTypeConverter : EnumTypeConverter {
		public AxisAlignmentTypeConverter(Type type)
			: base(type) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			Axis2D axis = TypeConverterHelper.GetElement<Axis2D>(context.Instance);
			if (axis == null || !axis.ShouldFilterZeroAlignment)
				return collection;
			List<AxisAlignment> values = new List<AxisAlignment>(collection.Count);
			foreach (AxisAlignment alignment in collection)
				if (alignment != AxisAlignment.Zero)
					values.Add(alignment);
			return new StandardValuesCollection(values);
		}
	}
	public abstract class XYDiagram2DTypeConverter : ExpandableObjectConverter {
		static IList<string> GetFilteredProperties(object value) {
			List<string> filteredProperties = new List<string>();
			XYDiagram2D diagram = TypeConverterHelper.GetElement<XYDiagram2D>(value);
			if (diagram != null && diagram.Chart != null && diagram.Chart.Container != null) {
				if (!diagram.Chart.SupportScrollingAndZooming)
					filteredProperties.AddRange(new string[] { "EnableAxisXZooming", "EnableAxisYZooming", "EnableAxisXScrolling", "EnableAxisYScrolling", "ScrollingOptions", "ZoomingOptions" });
				else {
					if (!diagram.IsScrollingEnabled)
						filteredProperties.Add("ScrollingOptions");
					if (!diagram.IsZoomingEnabled)
						filteredProperties.Add("ZoomingOptions");
				}
				if (!diagram.ShowRangeControlDateTimeGridOptions)
					filteredProperties.Add("RangeControlDateTimeGridOptions");
				if (!diagram.ShowRangeControlNumericGridOptions)
					filteredProperties.Add("RangeControlNumericGridOptions");
			}
			return filteredProperties;
		}
		protected abstract Type DiagramType { get; }
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			return FilterPropertiesUtils.FilterProperties(collection, GetFilteredProperties(value));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(DiagramType.GetConstructor(new Type[] { }), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class XYDiagramTypeConverter : XYDiagram2DTypeConverter {
		protected override Type DiagramType { get { return typeof(XYDiagram); } }
	}
	public class SwiftPlotDiagramTypeConverter : XYDiagram2DTypeConverter {
		protected override Type DiagramType { get { return typeof(SwiftPlotDiagram); } }
	}
	public class GanttDiagramTypeConverter : XYDiagram2DTypeConverter {
		protected override Type DiagramType { get { return typeof(GanttDiagram); } }
	}
	public class SimpleDiagramTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
					new InstanceDescriptor(typeof(SimpleDiagram).GetConstructor(new Type[0]), null, false) :
					base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			SimpleDiagram diagram = TypeConverterHelper.GetElement<SimpleDiagram>(value);
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			if (diagram != null && diagram.Chart.AutoLayout) {
				List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
				for (int i = 0; i < collection.Count; i++) {
					PropertyDescriptor desc = collection[i];
					if (desc.Name != "Dimension" && desc.Name != "LayoutDirection")
						properties.Add(desc);
				}
				return new PropertyDescriptorCollection(properties.ToArray());
			}
			return collection;
		}
	}
	public class XYDiagramDefaultPaneTypeConverter : ExpandableObjectConverter {
		static List<string> GetFilteredProperties(object value) {
			List<string> filteredProperties = new List<string>();
			XYDiagramPaneBase pane = TypeConverterHelper.GetElement<XYDiagramPaneBase>(value);
			if (pane != null) {
				switch (pane.SizeMode) {
					case PaneSizeMode.UseWeight:
						filteredProperties.Add("SizeInPixels");
						break;
					case PaneSizeMode.UseSizeInPixels:
						filteredProperties.Add("Weight");
						break;
				}
				if (pane.ChartContainer.ControlType != ChartContainerType.WinControl
					|| (!pane.ActualEnableAxisXZooming && !pane.ActualEnableAxisYZooming))
					filteredProperties.Add("ZoomRectangle");
				if (pane.Owner != null && !pane.ChartContainer.Chart.SupportScrollingAndZooming)
					filteredProperties.AddRange(new string[] { "EnableAxisXScrolling", "EnableAxisYScrolling", "EnableAxisXZooming",
						"EnableAxisYZooming", "ScrollBarOptions" });
				else {
					if (!pane.ActualEnableAxisXScrolling && !pane.ActualEnableAxisYScrolling)
						filteredProperties.Add("ScrollBarOptions");
				}
			}
			return filteredProperties;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesUtils.FilterProperties(base.GetProperties(context, value, attributes), GetFilteredProperties(value));
		}
	}
	public class XYDiagramPaneTypeConverter : XYDiagramDefaultPaneTypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(typeof(XYDiagramPane).GetConstructor(new Type[0]), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class ExplodeModeTypeConverter : EnumTypeConverter {
		public ExplodeModeTypeConverter(Type type)
			: base(type) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			PieSeriesViewBase view = TypeConverterHelper.GetElement<PieSeriesViewBase>(context.Instance);
			if (view != null) {
				SeriesBase series = view.Owner as SeriesBase;
				if (series != null) {
					TopNOptions options = series.TopNOptions;
					if (!options.Enabled || !options.ShowOthers) {
						List<PieExplodeMode> modes = new List<PieExplodeMode>(collection.Count);
						foreach (PieExplodeMode explodeMode in collection)
							if (explodeMode != PieExplodeMode.Others)
								modes.Add(explodeMode);
						collection = new StandardValuesCollection(modes);
					}
				}
			}
			return collection;
		}
	}
	public class ResolveOverlappingModeTypeConverter : EnumTypeConverter {
		public ResolveOverlappingModeTypeConverter(Type type)
			: base(type) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			SeriesLabelBase label = TypeConverterHelper.GetElement<SeriesLabelBase>(context.Instance);
			if (label != null) {
				List<ResolveOverlappingMode> modes = new List<ResolveOverlappingMode>(collection.Count);
				foreach (ResolveOverlappingMode mode in collection)
					if (label.CheckResolveOverlappingMode(mode))
						modes.Add(mode);
				collection = new StandardValuesCollection(modes);
			}
			return collection;
		}
	}
	public class PieSeriesLabelPositionTypeConverter : EnumTypeConverter {
		public PieSeriesLabelPositionTypeConverter(Type type)
			: base(type) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			PieSeriesLabel label = TypeConverterHelper.GetElement<PieSeriesLabel>(context.Instance);
			if (label != null) {
				List<PieSeriesLabelPosition> positions = new List<PieSeriesLabelPosition>(collection.Count);
				foreach (PieSeriesLabelPosition position in collection)
					if (label.IsPositionSupported(position))
						positions.Add(position);
				collection = new StandardValuesCollection(positions);
			}
			return collection;
		}
	}
	public class ValueLevelTypeConterter : EnumTypeConverter {
		public ValueLevelTypeConterter()
			: base(typeof(ValueLevel)) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			XYDiagram2DSeriesViewBase view = null;
			object obj = TypeConverterHelper.GetElement(context.Instance);
			RegressionLine regressionLine = obj as RegressionLine;
			if (regressionLine != null)
				view = regressionLine.View;
			else {
				FinancialIndicatorPoint financialIndicatorPoint = obj as FinancialIndicatorPoint;
				if (financialIndicatorPoint != null)
					view = financialIndicatorPoint.FinancialIndicator.View;
			}
			return view == null ? base.GetStandardValues(context) : new StandardValuesCollection(view.SupportedValueLevels);
		}
	}
	public class PolygonGradientModeTypeConverter : EnumTypeConverter {
		public PolygonGradientModeTypeConverter(Type type)
			: base(type) {
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection collection = base.GetStandardValues(context);
			PolygonGradientFillOptions polygonFillOptions = context.Instance as PolygonGradientFillOptions;
			if (polygonFillOptions != null) {
				PolygonFillStyle fillStyle = polygonFillOptions.Owner as PolygonFillStyle;
				if (fillStyle != null) {
					SeriesViewBase view = fillStyle.Owner as SeriesViewBase;
					if (view != null) {
						AreaSeriesViewBase viewArea = view as AreaSeriesViewBase;
						List<PolygonGradientMode> list = new List<PolygonGradientMode>();
						foreach (PolygonGradientMode gradientMode in Enum.GetValues(typeof(PolygonGradientMode))) {
							if (viewArea != null) {
								if (gradientMode == PolygonGradientMode.ToCenter || gradientMode == PolygonGradientMode.FromCenter)
									continue;
							}
							list.Add(gradientMode);
						}
						collection = new StandardValuesCollection(list);
					}
				}
			}
			return collection;
		}
	}
	public class ValueDataMemberCollectionConverter : CollectionTypeConverter {
		protected class ValueDataMemberPropertyDescriptor : PropertyDescriptor {
			int index;
			public ValueDataMemberPropertyDescriptor(string name, int index)
				: base(name, new Attribute[] { new EditorAttribute("DevExpress.XtraCharts.Design.DataMemberEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)) }) {
				this.index = index;
			}
			protected virtual void SetValueDataMember(ValueDataMemberCollection collection, string dataMember, int valueIndex) {
				if (collection != null)
					collection[valueIndex] = dataMember;
			}
			public override Type ComponentType { get { return typeof(ValueDataMemberCollection); } }
			public override bool IsReadOnly { get { return false; } }
			public override Type PropertyType { get { return typeof(string); } }
			public override object GetValue(object component) {
				ValueDataMemberCollection collection = component as ValueDataMemberCollection;
				return collection == null ? String.Empty : collection[index];
			}
			public override void SetValue(object component, object value) {
				string str = value as string;
				if (str != null)
					SetValueDataMember(component as ValueDataMemberCollection, str, index);
			}
			public override bool CanResetValue(object component) {
				ValueDataMemberCollection collection = component as ValueDataMemberCollection;
				return collection != null && !String.IsNullOrEmpty(collection[index]);
			}
			public override void ResetValue(object component) {
				SetValueDataMember(component as ValueDataMemberCollection, String.Empty, index);
			}
			public override bool ShouldSerializeValue(object component) {
				return false;
			}
		}
		protected virtual PropertyDescriptor CreateValuePropertyDescriptor(ITypeDescriptorContext context, string caption, int valueIndex) {
			return new ValueDataMemberPropertyDescriptor(caption, valueIndex);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			if (context != null) {
				SeriesBase series = TypeConverterHelper.GetElement<SeriesBase>(context.Instance);
				if (series != null) {
					SeriesViewBase view = series.View;
					int size = view.PointDimension;
					PropertyDescriptor[] descs = new PropertyDescriptor[size];
					for (int i = 0; i < size; i++) {
						PropertyDescriptor desc = CreateValuePropertyDescriptor(context, view.GetValueCaption(i), i);
						if (series.View.Chart.DataContainer.PivotGridDataSourceOptions.HasDataSource && series.View.Chart.DataContainer.PivotGridDataSourceOptions.AutoBindingSettingsEnabled)
							desc = new ReadOnlyPropertyDescriptor(desc);
						descs[i] = desc;
					}
					return new PropertyDescriptorCollection(descs);
				}
			}
			return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
		}
	}
	public abstract class CollectionItemTypeConverter : TypeConverter {
		protected abstract Type CollectionItemType { get; }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				ConstructorInfo ci = CollectionItemType.GetConstructor(new Type[] { });
				return new InstanceDescriptor(ci, null, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class ConstantLineTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(ConstantLine); } }
	}
	public class ScaleBreakTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(ScaleBreak); } }
	}
	public abstract class AnnotationTypeConverter : ExpandableObjectConverter {
		protected abstract Type AnnotationType { get; }
		bool ShouldShowRuntimeProperties(Annotation annotation) {
			IChartContainer chartContainer = CommonUtils.FindChartContainer(annotation);
			return !annotation.ActualLabelMode && chartContainer != null && chartContainer.ControlType == ChartContainerType.WinControl && !chartContainer.Chart.Is3DDiagram;
		}
		List<string> GetFilteredProperties(Annotation annotation) {
			List<string> filteredProperties = new List<string>();
			if (annotation != null && annotation.ShapeKind != ShapeKind.RoundedRectangle)
				filteredProperties.Add("ShapeFillet");
			if (!annotation.LabelModeSupported)
				filteredProperties.Add("LabelMode");
			if (!ShouldShowRuntimeProperties(annotation))
				filteredProperties.AddRange(new string[] { "RuntimeAnchoring", "RuntimeRotation", "RuntimeResizing", "RuntimeMoving" });
			return filteredProperties;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			if (collection == null)
				return collection;
			Annotation annotation = TypeConverterHelper.GetElement<Annotation>(value);
			if (annotation == null)
				return collection;
			PropertyDescriptor[] descs = new PropertyDescriptor[collection.Count];
			collection.CopyTo(descs, 0);
			return FilterPropertiesUtils.FilterProperties(new PropertyDescriptorCollection(descs), GetFilteredProperties(annotation));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(AnnotationType.GetConstructor(new Type[] { }), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class TextAnnotationTypeConverter : AnnotationTypeConverter {
		protected override Type AnnotationType { get { return typeof(TextAnnotation); } }
	}
	public class ImageAnnotationTypeConverter : AnnotationTypeConverter {
		protected override Type AnnotationType { get { return typeof(ImageAnnotation); } }
	}
	public class SolidFillOptionsTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(SolidFillOptions); } }
	}
	public class RectangleGradientFillOptionsTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(RectangleGradientFillOptions); } }
	}
	public class PolygonGradientFillOptionsTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(PolygonGradientFillOptions); } }
	}
	public class HatchFillOptionsTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(HatchFillOptions); } }
	}
	public class StripTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(Strip); } }
	}
	public abstract class DockableTitleConverter : ExpandableObjectConverter {
		const string maximumLinesCountPropertyName = "MaxLineCount";
		protected virtual Type CollectionItemType { get { return typeof(DockableTitle); } }
		bool ShouldHideMaximumLinesCountProperty(object value) {
			DockableTitle title = TypeConverterHelper.GetElement<DockableTitle>(value);
			return title != null && !title.WordWrap;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor)) {
				ConstructorInfo ci = CollectionItemType.GetConstructor(new Type[] { });
				return new InstanceDescriptor(ci, null, false);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			return ShouldHideMaximumLinesCountProperty(value) ?
				FilterPropertiesUtils.FilterProperties(collection, new string[] { maximumLinesCountPropertyName }) : collection;
		}
	}
	public class ChartTitleTypeConverter : DockableTitleConverter {
		protected override Type CollectionItemType { get { return typeof(ChartTitle); } }
	}
	public class SeriesTitleTypeConverter : DockableTitleConverter {
		protected override Type CollectionItemType { get { return typeof(SeriesTitle); } }
	}
	public class TrendLineTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(TrendLine); } }
	}
	public class RegressionLineTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(RegressionLine); } }
	}
	public class CustomAxisLabelTypeConverter : CollectionItemTypeConverter {
		protected override Type CollectionItemType { get { return typeof(CustomAxisLabel); } }
	}
	public class CrosshairOptionsTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			CrosshairOptions options = TypeConverterHelper.GetElement<CrosshairOptions>(value);
			if (options != null) {
				if (options.CrosshairLabelMode != CrosshairLabelMode.ShowCommonForAllSeries)
					collection = FilterPropertiesUtils.FilterProperties(collection, new string[] { "CommonLabelPosition" });
				if (options.ChartContainer.ControlType == ChartContainerType.WebControl)
					collection = FilterPropertiesUtils.FilterProperties(collection, new string[] { "HighlightPoints" });
			}
			return collection;
		}
	}
	public abstract class ShapePositionTypeConverter : ExpandableObjectConverter {
		protected abstract Type Type { get; }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(Type.GetConstructor(new Type[0]), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class FreePositionTypeConverter : ShapePositionTypeConverter {
		protected override Type Type { get { return typeof(FreePosition); } }
	}
	public class RelativePositionTypeConverter : ShapePositionTypeConverter {
		protected override Type Type { get { return typeof(RelativePosition); } }
	}
	public class ToolTipFreePositionTypeConverter : ShapePositionTypeConverter {
		protected override Type Type { get { return typeof(ToolTipFreePosition); } }
	}
	public class ToolTipRelativePositionTypeConverter : ShapePositionTypeConverter {
		protected override Type Type { get { return typeof(ToolTipRelativePosition); } }
	}
	public class ToolTipMousePositionTypeConverter : ShapePositionTypeConverter {
		protected override Type Type { get { return typeof(ToolTipMousePosition); } }
	}
	public class CrosshairMousePositionTypeConverter : ShapePositionTypeConverter {
		protected override Type Type { get { return typeof(CrosshairMousePosition); } }
	}
	public class CrosshairFreePositionTypeConverter : ShapePositionTypeConverter {
		protected override Type Type { get { return typeof(CrosshairFreePosition); } }
	}
	public abstract class AnchorPointTypeConverter : ExpandableObjectConverter {
		protected abstract Type Type { get; }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(Type.GetConstructor(new Type[0]), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class ChartAnchorPointTypeConverter : AnchorPointTypeConverter {
		protected override Type Type { get { return typeof(ChartAnchorPoint); } }
	}
	public class SeriesPointAnchorPointTypeConverter : AnchorPointTypeConverter {
		protected override Type Type { get { return typeof(SeriesPointAnchorPoint); } }
	}
	public class PaneAnchorPointTypeConverter : AnchorPointTypeConverter {
		protected override Type Type { get { return typeof(PaneAnchorPoint); } }
	}
	public class AnnotationAnchorPointTypeConverter : ExpandableObjectConverter {
		const string seriesPointPropertyName = "SeriesPoint";
		Attribute[] GetAttributes(PropertyDescriptor propertyDescriptor) {
			Attribute[] attributes = new Attribute[propertyDescriptor.Attributes.Count];
			for (int i = 0; i < propertyDescriptor.Attributes.Count; i++)
				attributes[i] = propertyDescriptor.Attributes[i];
			return attributes;
		}
		PropertyDescriptorCollection GetSeriesPointAnchorPointProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			PropertyDescriptor[] descs = new PropertyDescriptor[collection.Count];
			collection.CopyTo(descs, 0);
			for (int i = 0; i < descs.Length; i++) {
				if (descs[i].Name == seriesPointPropertyName)
					descs[i] = new SeriesPointPropertyDescriptor(descs[i].Name, GetAttributes(descs[i]));
			}
			return new PropertyDescriptorCollection(descs);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			if (value is SeriesPointAnchorPoint)
				return GetSeriesPointAnchorPointProperties(context, value, attributes);
			return base.GetProperties(context, value, attributes);
		}
	}
	public class MovingAverageTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(value.GetType().GetConstructor(new Type[0]), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			MovingAverage movingAverage = TypeConverterHelper.GetElement<MovingAverage>(value);
			return (movingAverage == null || movingAverage.Kind != MovingAverageKind.MovingAverage) ? collection :
				FilterPropertiesUtils.FilterProperties(collection, new string[] { "EnvelopePercent", "EnvelopeColor" });
		}
	}
	public class IndicatorTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(value.GetType().GetConstructor(new Type[0]), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class AxisLabelResolveOverlappingOptionsTypeConverter : ExpandableObjectConverter {
		AxisLabel GetLabel(AxisLabelResolveOverlappingOptions options) {
			if (options != null)
				return options.Owner as AxisLabel;
			return null;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			AxisLabel label = GetLabel(TypeConverterHelper.GetElement<AxisLabelResolveOverlappingOptions>(value));
			if ((label is RadarAxisXLabel) || (label is RadarAxisYLabel))
				return FilterPropertiesUtils.FilterProperties(collection, new string[] { "AllowRotate", "AllowStagger" });
			return collection;
		}
	}
	public class ChartRangeControlClientGridOptionsTypeConverter : ExpandableObjectConverter {
		readonly string[] manualGridProperties = new string[] { "GridAlignment", "GridSpacing", "GridOffset" };
		readonly string[] manualSnapProperties = new string[] { "SnapAlignment", "SnapSpacing", "SnapOffset" };
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			ChartRangeControlClientGridOptions options = TypeConverterHelper.GetElement<ChartRangeControlClientGridOptions>(value);
			if (!options.IsManualGrid)
				collection = FilterPropertiesUtils.FilterProperties(collection, manualGridProperties);
			if (!options.IsManualSnap)
				collection = FilterPropertiesUtils.FilterProperties(collection, manualSnapProperties);
			return collection;
		}
	}
	public abstract class ColorizerTypeConverter : ExpandableObjectConverter {
		protected abstract Type ColorizerType { get; }
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
				new InstanceDescriptor(ColorizerType.GetConstructor(new Type[] { }), null, false) :
				base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class KeyColorColorizerTypeConverter : ColorizerTypeConverter {
		protected override Type ColorizerType {
			get { return typeof(KeyColorColorizer); }
		}
	}
	public class RangeColorizerTypeConverter : ColorizerTypeConverter {
		protected override Type ColorizerType {
			get { return typeof(RangeColorizer); }
		}
	}
	public class ColorObjectColorizerTypeConverter : ColorizerTypeConverter {
		protected override Type ColorizerType {
			get { return typeof(ColorObjectColorizer); }
		}
	}
	public interface IObjectValueTypeProvider {
		Type DataType { get; }
	}
	public class ObjectValueTypeConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value is string) {
				if (context == null)
					return value;
				if (context.Instance != null && (context.Instance is IObjectValueTypeProvider)) {
					Type descType = ((IObjectValueTypeProvider)context.Instance).DataType;
					if (descType != null) {
						TypeConverter converter = TypeDescriptor.GetConverter(descType);
						if (converter != null)
							return converter.ConvertFrom(value);
					}
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class DataTypeConverter : TypeConverter {
		static readonly Type[] types = new Type[] {
			typeof(bool), typeof(byte), typeof(char), typeof(DateTime), 
			typeof(decimal), typeof(double), typeof(Guid), typeof(short), 
			typeof(int), typeof(long), typeof(sbyte), typeof(float), 
			typeof(string), typeof(TimeSpan), typeof(ushort), 
			typeof(uint), typeof(ulong)
		};
		readonly TypeConverter.StandardValuesCollection values;
		public DataTypeConverter() {
			object[] typesCopy = new object[types.Length];
			Array.Copy(types, typesCopy, types.Length);
			this.values = new TypeConverter.StandardValuesCollection(typesCopy);
		}
		Type FindTypeByName(string name) {
			foreach (Type type in types) {
				if (type.ToString().Equals(name))
					return type;
			}
			return null;
		}
		Type GetTypeFromValue(object value) {
			if (value is Type)
				return (Type)value;
			else if (value is string)
				return FindTypeByName((string)value);
			return null;
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertTo(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if ((value == null) || (value.GetType() != typeof(string)))
				return base.ConvertFrom(context, culture, value);
			return FindTypeByName((string)value) ?? typeof(string);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == null)
				throw new ArgumentNullException("destinationType");
			if (destinationType == typeof(string))
				return value != null ? value.ToString() : string.Empty;
			if ((value != null) && (destinationType == typeof(InstanceDescriptor))) {
				Type type = GetTypeFromValue(value);
				if (type != null) {
					MethodInfo getTypeInfo = typeof(Type).GetMethod("GetType", new Type[] { typeof(string) });
					return new InstanceDescriptor(getTypeInfo, new object[] { type.AssemblyQualifiedName });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return this.values;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
	public class SeriesBaseTypeConverter : ExpandableObjectConverter {
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			SeriesBase series = TypeConverterHelper.GetElement<SeriesBase>(value);
			if (series == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>(collection.Count);
			foreach (PropertyDescriptor desc in collection)
				switch (desc.Name) {
					case "Label":
						if (series.IsSupportedLabel)
							descs.Add(ChartElementPropertyDescriptor.GetValidPropertyDescriptor(desc, series.Label));
						break;
					case "View":
						descs.Add(ChartElementPropertyDescriptor.GetValidPropertyDescriptor(desc, series.View));
						break;
					case "TopNOptions":
						if (series.ShouldApplyTopNOptions)
							descs.Add(ChartElementPropertyDescriptor.GetValidPropertyDescriptor(desc, series.TopNOptions));
						break;
					case "ArgumentScaleType":
					case "ValueScaleType":
					case "ArgumentDataMember":
						if (PivotGridDataSourceUtils.IsAutoBindingSettingsUsed(series.ChartContainer.Chart.DataContainer.PivotGridDataSourceOptions, series))
							descs.Add(new ReadOnlyPropertyDescriptor(desc));
						else
							descs.Add(desc);
						break;
					case "ShowInLegend":
						if (PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabledForSimpleView(series.View.Chart.DataContainer.PivotGridDataSourceOptions))
							descs.Add(new ReadOnlyPropertyDescriptor(desc));
						else
							descs.Add(desc);
						break;
					case "ValueDataMembers":
						if (!series.IsSummaryBinding)
							descs.Add(ChartElementPropertyDescriptor.GetValidPropertyDescriptor(desc, series.ValueDataMembers));
						break;
					case "ToolTipHintDataMember":
					case "ToolTipSeriesPattern":
					case "ToolTipPointPattern":
						if (series.Owner != null && series.Chart.SupportToolTips && series.IsToolTipsSupported)
							descs.Add(desc);
						break;
					case "CrosshairEnabled":
					case "CrosshairLabelPattern":
						if (series.Owner != null && series.Chart.SupportCrosshair && series.View.IsSupportedCrosshair)
							descs.Add(desc);
						break;
					case "CrosshairHighlightPoints":
						if (series.ChartContainer.ControlType != ChartContainerType.WebControl)
							descs.Add(desc);
						break;
					default:
						descs.Add(desc);
						break;
				}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public class SeriesTypeConverter : SeriesBaseTypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) ?
					new InstanceDescriptor(typeof(Series).GetConstructor(new Type[0]), null, false) :
					base.ConvertTo(context, culture, value, destinationType);
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			PropertyDescriptorCollection collection = base.GetProperties(context, value, attributes);
			Series series = TypeConverterHelper.GetElement<Series>(value);
			if (series == null)
				return collection;
			List<PropertyDescriptor> descs = new List<PropertyDescriptor>(collection.Count);
			foreach (PropertyDescriptor desc in collection)
				switch (desc.Name) {
					case "Points":
						if (context.Container != null)
							descs.Add(desc);
						break;
					case "LegendText":
						if (series.ShowInLegend && (series.View == null || !series.View.ActualColorEach))
							descs.Add(desc);
						break;
					case "DataSource":
						if (series.ChartContainer == null || series.ChartContainer.ControlType != ChartContainerType.XRControl)
							descs.Add(desc);
						else {
							string reportsAssempblyName = "DevExpress.XtraReports.Design.DataSourceEditor," + AssemblyInfo.SRAssemblyReports;
							EditorAttribute attribute = new EditorAttribute(reportsAssempblyName, typeof(UITypeEditor));
							descs.Add(ChartElementPropertyDescriptor.GetPropertyDescriptorWithAttributes(desc, attribute));
						}
						break;
					case "ToolTipImage":
						if (series.Owner != null && series.Chart.SupportToolTips && series.IsToolTipsSupported)
							descs.Add(desc);
						break;
					default:
						descs.Add(desc);
						break;
				}
			return new PropertyDescriptorCollection(descs.ToArray());
		}
	}
	public static class TypeConverterHelper {
		public static TElement GetElement<TElement>(object value) where TElement : class {
			if (value is TElement)
				return (TElement)value;
			if (value is FakeComponent)
				return ((FakeComponent)value).Object as TElement;
			if (value is IChartElementDesignerModel)
				return ((IChartElementDesignerModel)value).SourceElement as TElement;
			return null;
		}
		public static object GetElement(object value) {
			if (value is FakeComponent)
				return ((FakeComponent)value).Object;
			if (value is IChartElementDesignerModel)
				return ((IChartElementDesignerModel)value).SourceElement;
			return value;
		}
	}
}
