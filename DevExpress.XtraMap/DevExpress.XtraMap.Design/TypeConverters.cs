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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Linq;
using DevExpress.Map;
using DevExpress.Utils;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Design {
	public class MapColumnNameConverter : StringConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StringCollection columnNames = GetColumnNames(context);
			columnNames.Add(String.Empty);
			return new StandardValuesCollection(columnNames);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		protected internal virtual StringCollection GetColumnNames(ITypeDescriptorContext context) {
			if (context == null)
				return new StringCollection();
			LayerDataManager mgr = GetDataManager(context.Instance);
			return mgr != null ? mgr.GetColumnNames() : new StringCollection();
		}
		protected internal virtual LayerDataManager GetDataManager(object instance) {
			if (instance == null)
				return null;
			ILayerDataManagerProvider provider = instance as ILayerDataManagerProvider;
			return (provider != null) ? provider.DataManager : null;
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (String.IsNullOrEmpty(value as String))
				return DesignSR.NoneString;
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string s = value as string;
			if (String.IsNullOrEmpty(s) || String.Compare(s, DesignSR.NoneString, true, CultureInfo.CurrentCulture) == 0)
				return String.Empty;
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class ExpandableNoneStringSupportedTypeConverter : ExpandableObjectConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (value == null)
				return DesignSR.NoneString;
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string s = value as string;
			if (String.IsNullOrEmpty(s) || String.Compare(s, DesignSR.NoneString, true, CultureInfo.CurrentCulture) == 0)
				return String.Empty;
			return base.ConvertFrom(context, culture, value);
		}
	}
	public class ExpandableObjectConverterShowsValueTypeNameInParentheses : ExpandableObjectConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (value == null)
				return DesignSR.NoneString;
			return "(" + value.GetType().Name + ")";
		}
	}
	public class CoordinatesPatternTypeConverter : TypeConverter {
		protected virtual string DefaultString { get { return String.Empty; } }
		StringCollection GetGeoPatterns() {
			return new StringCollection() {
				DesignSR.DegreeCardinalPoint,
				DesignSR.CardinalPointDegree,
				DesignSR.DegreeMinuteCardinalPoint,
				DesignSR.CardinalPointDegreeMinute,
				DesignSR.DegreeMinuteSecondCardinalPoint,
				DesignSR.CardinalPointDegreeMinuteSecond
			};
		}
		StringCollection GetCartesianPatterns() {
			return new StringCollection() {
				DesignSR.ValueMeasureUnit,
				DesignSR.PrecisionValueMeasureUnit
			};
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null && context.Instance != null;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			NavigationPanelOptions options = (NavigationPanelOptions)context.Instance;
			StringCollection defaultPatterns = options.IsGeoProjection ? GetGeoPatterns() : GetCartesianPatterns();
			return new StandardValuesCollection(defaultPatterns);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return true;
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			return value is string ? value : String.Empty;
		}
	}
	public class MapItemsLayerTypeConverter : ExpandableObjectConverter {
		protected InnerMap GetInnerMap(ITypeDescriptorContext context) {
			ItemsLayerLegend legend = (ItemsLayerLegend)context.Instance;
			return ((IOwnedElement)legend).Owner as InnerMap;
		}
		List<VectorItemsLayer> SelectItemsLayers(InnerMap map) {
			List<VectorItemsLayer> result = new List<VectorItemsLayer>();
			if (map == null)
				return result;
			foreach (LayerBase layer in map.Layers) {
				VectorItemsLayer itemsLayer = layer as VectorItemsLayer;
				if (itemsLayer != null) result.Add(itemsLayer);
			}
			return result;
		}
		VectorItemsLayer GetLayerByString(InnerMap map, string value) {
			int index;
			if (!Int32.TryParse(value, out index) || index == -1)
				return null;
			List<VectorItemsLayer> layers = SelectItemsLayers(map);
			return layers[index];
		}
		object GetLayerString(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			VectorItemsLayer layer = value as VectorItemsLayer;
			return (layer != null && !string.IsNullOrEmpty(layer.Name)) ? layer.Name : base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return context != null && context.Instance != null;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			InnerMap map = GetInnerMap(context);
			StringCollection availableLayers = new StringCollection();
			List<VectorItemsLayer> layers = SelectItemsLayers(map);
			for (int i = -1; i < layers.Count; i++)
				availableLayers.Add(i.ToString());
			return new StandardValuesCollection(availableLayers);
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			InnerMap map = GetInnerMap(context);
			return GetLayerByString(map, value.ToString());
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			return value == null ? DesignSR.NoneString : GetLayerString(context, culture, value, destinationType);
		}
	}
	public class EncodingWebNameComparer : IComparer<Encoding> {
		int IComparer<Encoding>.Compare(Encoding x, Encoding y) {
			if (Object.Equals(x, y)) return 0;
			if (x == null) return -1;
			if (y == null) return 1;
			return x.WebName.CompareTo(y.WebName);
		}
	}
	public class EncodingTypeConverter : TypeConverter {
		#region
		static List<Encoding> GetStandardEncodings() {
			List<Encoding> encodings = new List<Encoding>();
			encodings.Add(Encoding.GetEncoding("cp437"));
			encodings.Add(Encoding.GetEncoding(737)); 
			encodings.Add(Encoding.GetEncoding(850)); 
			encodings.Add(Encoding.GetEncoding(852));
			encodings.Add(Encoding.GetEncoding(857));
			encodings.Add(Encoding.GetEncoding(861));
			encodings.Add(Encoding.GetEncoding(865));
			encodings.Add(Encoding.GetEncoding(866)); 
			encodings.Add(Encoding.GetEncoding(936));
			encodings.Add(Encoding.GetEncoding(932));
			encodings.Add(Encoding.GetEncoding(950));
			encodings.Add(Encoding.GetEncoding("macintosh"));
			encodings.Add(Encoding.GetEncoding(1250)); 
			encodings.Add(Encoding.GetEncoding(1251));
			encodings.Add(Encoding.GetEncoding(1253));
			encodings.Add(Encoding.GetEncoding(1255));
			encodings.Add(Encoding.GetEncoding(1256));
			encodings.Add(Encoding.GetEncoding(1254));
			encodings.Add(Encoding.GetEncoding("x-mac-cyrillic"));
			encodings.Add(Encoding.GetEncoding("x-mac-ce"));
			encodings.Add(Encoding.GetEncoding("x-mac-greek"));
			encodings.Sort(new EncodingWebNameComparer());
			return encodings;
		}
		#endregion
		public EncodingTypeConverter() {
		}
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (value is string) {
				foreach (Encoding encoding in GetStandardEncodings()) {
					if (encoding.WebName == (string)value)
						return encoding;
				}
				return DXEncoding.Default;
			}
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(string) && value is Encoding) {
				return ((Encoding)value).WebName;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return true;
		}
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(GetStandardEncodings());
		}
	}
	public class CoordPointTypeConverter : ExpandableObjectConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return (sourceType == typeof(string) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || (destinationType == typeof(string)) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if (!(value is string))
				return base.ConvertFrom(context, culture, value);
			string input = ((string)value).Trim();
			MapControl map = context.Instance as MapControl;
			if (map != null) {
				if (map.CoordinateSystem is CartesianMapCoordinateSystem)
					return CartesianPoint.Parse(input);
				else
					return GeoPoint.Parse(input);
			}
			MapItem item = context.Instance as MapItem;
			IOwnedElement owned = item as IOwnedElement;
			if (owned != null) {
				var dataAdapter = owned.Owner as IMapCoordSystemProvider;
				if (dataAdapter != null) {
					var cartesianCS = dataAdapter.GetMapCoordSystem() as CartesianMapCoordinateSystem;
					if (cartesianCS != null)
						return CartesianPoint.Parse(input);
				}
			}
			return GeoPoint.Parse(input);
		}
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
			return base.CreateInstance(context, propertyValues);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == typeof(string) && (value is CoordPoint)) {
				return value.ToString();
			}
			if ((destinationType == typeof(InstanceDescriptor)) && (value is CoordPoint)) {
				Type[] types = new Type[] { typeof(double), typeof(double) };
				ConstructorInfo ci = value.GetType().GetConstructor(types);
				double[] coord = GetConstructrorParameters((CoordPoint)value);
				return new InstanceDescriptor(ci, coord);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		double[] GetConstructrorParameters(CoordPoint point) {
			GeoPoint geoPoint = point as GeoPoint;
			if (geoPoint != null)
				return new double[] { geoPoint.Latitude, geoPoint.Longitude };
			return new double[] { point.GetX(), point.GetY() };
		}
	}
	public class MeasureUnitTypeConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (destinationType == typeof(InstanceDescriptor) && value is MeasureUnit) {
				MeasureUnit passedMeasureUnit = (MeasureUnit)value;
				List<MeasureUnit> predefined = MeasureUnit.GetPredefinedUnits();
				if (predefined.Contains(passedMeasureUnit)) {
					PropertyInfo[] properties = typeof(MeasureUnit).GetProperties();
					foreach (var property in properties) {
						MethodInfo getMethod = property.GetGetMethod();
						if (getMethod == null)
							continue;
						if (!getMethod.IsStatic)
							continue;
						object propertyValue = getMethod.Invoke(null, null);
						MeasureUnit unit = propertyValue as MeasureUnit;
						if (unit == null)
							continue;
						if (unit == passedMeasureUnit)
							return new InstanceDescriptor(property, null);
					}
				}
				else {
					ConstructorInfo ctor = typeof(MeasureUnit).GetConstructor(new Type[] { typeof(double), typeof(string), typeof(string) });
					if (ctor != null)
						return new InstanceDescriptor(ctor, new object[] { passedMeasureUnit.MetersInUnit, passedMeasureUnit.Name, passedMeasureUnit.Abbreviation });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
