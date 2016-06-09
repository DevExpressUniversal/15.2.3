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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using System.Drawing;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SeriesPoint.SeriesPointTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SeriesPoint : ChartElement, ICustomTypeDescriptor, ISeriesPoint, IXtraSupportDeserializeCollectionItem {
		#region Nested Classes: SeriesPointTypeConverter, ValuesTypeConverter, DateTimeValuesTypeConverter, SeriesPointPropertyDescriptorCollection
		internal class SeriesPointTypeConverter : TypeConverter {
			object GetArgument(SeriesPoint point) {
				if (point.argument != null) {
					ScaleType scaleType = point.Series != null ? point.Series.ActualArgumentScaleType : point.argumentScaleType;
					switch (scaleType) {
						case ScaleType.Numerical:
							return point.NumericalArgument;
						case ScaleType.DateTime:
							return point.DateTimeArgument;
					}
				}
				return point.Argument;
			}
			MethodBase GetCreatorInfo(object value) {
				SeriesPoint point = (SeriesPoint)value;
				if (point.IsEmpty)
					return point.ShouldSerializeSeriesPointIDInternal() ?
						typeof(SeriesPoint).GetConstructor(new Type[] { typeof(object), typeof(object[]), typeof(int) }) :
						typeof(SeriesPoint).GetConstructor(new Type[] { typeof(object) });
				return point.ShouldSerializeSeriesPointIDInternal() ?
					typeof(SeriesPoint).GetConstructor(new Type[] { typeof(object), typeof(object[]), typeof(int) }) :
					typeof(SeriesPoint).GetConstructor(new Type[] { typeof(object), typeof(object[]) });
			}
			object GetPointValues(SeriesPoint point) {
				object[] values;
				if (point.ShouldSerializeDateTimeValuesInternal()) {
					values = new object[point.dateTimeValues.Length];
					for (int i = 0; i < values.Length; i++)
						values[i] = point.dateTimeValues[i];
				}
				else {
					values = new object[point.values.Length];
					for (int i = 0; i < values.Length; i++)
						values[i] = point.values[i];
				}
				return values;
			}
			object[] GetCreatorParams(object value) {
				SeriesPoint point = (SeriesPoint)value;
				ArrayList list = new ArrayList();
				list.Add(GetArgument(point));
				bool shouldSerializeID = point.ShouldSerializeSeriesPointIDInternal();
				if (point.IsEmpty) {
					if (shouldSerializeID)
						list.Add(null);
				}
				else
					list.Add(GetPointValues(point));
				if (shouldSerializeID)
					list.Add(point.SeriesPointID);
				return list.ToArray();
			}
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) ?
					new InstanceDescriptor(GetCreatorInfo(value), GetCreatorParams(value), false) :
					base.ConvertTo(context, culture, value, destinationType);
			}
		}
		class ValuesTypeConverter : TypeConverter {
			const char separator = ';';
			string ConvertDoubles2String(double[] values) {
				string[] array = new string[values.Length];
				for (int i = 0; i < values.Length; i++)
					array[i] = values[i].ToString(NumberFormatInfo.InvariantInfo);
				return String.Join(new string(separator, 1), array);
			}
			double[] ConvertString2Doubles(string value) {
				value.Trim();
				string[] array = value.Split(separator);
				double[] result = new double[array.Length];
				for (int i = 0; i < array.Length; i++)
					result[i] = double.Parse(array[i], NumberFormatInfo.InvariantInfo);
				return result;
			}
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				return sourceType == typeof(string);
			}
			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
				string str = value as string;
				if (str == null)
					throw base.GetConvertFromException(value);
				return ConvertString2Doubles(str);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType != typeof(string))
					throw base.GetConvertToException(value, destinationType);
				return value == null ? String.Empty : ConvertDoubles2String((double[])value);
			}
		}
		class DateTimeValuesTypeConverter : TypeConverter {
			const char separator = ';';
			string ConvertDateTime2String(DateTime[] values) {
				string[] array = new string[values.Length];
				for (int i = 0; i < values.Length; i++)
					array[i] = values[i].ToString(SerializingUtils.SerializableDateTimeFormat, DateTimeFormatInfo.InvariantInfo);
				return String.Join(new string(separator, 1), array);
			}
			DateTime[] ConvertString2DateTime(string value) {
				value.Trim();
				string[] array = value.Split(separator);
				DateTime[] result = new DateTime[array.Length];
				for (int i = 0; i < array.Length; i++)
					result[i] = DateTime.Parse(array[i], DateTimeFormatInfo.InvariantInfo);
				return result;
			}
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				return sourceType == typeof(string);
			}
			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
				string str = value as string;
				if (str == null)
					throw base.GetConvertFromException(value);
				return ConvertString2DateTime(str);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				if (destinationType != typeof(string))
					throw base.GetConvertToException(value, destinationType);
				return value == null ? String.Empty : ConvertDateTime2String((DateTime[])value);
			}
		}
		class SeriesPointPropertyDescriptorCollection : PropertyDescriptorCollection {
			public SeriesPointPropertyDescriptorCollection(ICollection descriptors, ScaleType scaleType)
				: base(new PropertyDescriptor[] { }) {
				foreach (PropertyDescriptor pd in descriptors)
					if ((pd.DisplayName == "DateTimeValues" && scaleType != ScaleType.DateTime) || (pd.DisplayName == "Values" && scaleType == ScaleType.DateTime))
						Add(new CustomPropertyDescriptor(pd, false));
					else
						Add(pd);
			}
		}
		#endregion
		public static string ValuesProperty = "Values";
		public static string ArgumentProperty = "Argument";
		static readonly string defaultQualitativeArgument = string.Empty;
		static readonly DateTime defaultDateTimeArgument = DateTime.MinValue;
		const double defaultNumericalArgument = double.NaN;
		internal static SeriesPoint CreateAuxiliarySeriesPoint(ChartElement owner, string argument, params double[] values) {
			SeriesPoint seriesPoint = new SeriesPoint(argument, values);
			seriesPoint.isAuxiliary = true;
			seriesPoint.Owner = owner;
			return seriesPoint;
		}
		internal static SeriesPoint GetSeriesPoint(ISeriesPoint point) {
			if (point is SeriesPoint)
				return (SeriesPoint)point;
			else if (point is AggregatedSeriesPoint)
				return new SeriesPoint((AggregatedSeriesPoint)point);
			return null;
		}
		internal static bool EqualsBySourcePoints(SeriesPoint pointA, SeriesPoint pointB) {
			if (Object.ReferenceEquals(pointA, pointB))
				return true;
			foreach (SeriesPoint sourcePointB in pointB.SourcePoints) {
				if (Object.ReferenceEquals(pointA, sourcePointB))
					return true;
				foreach (SeriesPoint sourcePointA in pointA.SourcePoints)
					if (Object.ReferenceEquals(sourcePointA, sourcePointB))
						return true;
			}
			foreach (SeriesPoint sourcePointA in pointA.SourcePoints)
				if (Object.ReferenceEquals(sourcePointA, pointB))
					return true;
			return false;
		}
		public static SeriesPoint CreateSeriesPointWithId(object argument, int id) {
			SeriesPoint point = new SeriesPoint(argument);
			point.id = id;
			return point;
		}
		AnnotationCollection annotations = null;
		SeriesPointRelationCollection relations;
		ScaleType argumentScaleType = ScaleType.Auto;
		ScaleType valueScaleType = ScaleType.Numerical;
		object argument = null;
		string argumentSerializable = null;
		string qualitativeArgument = defaultQualitativeArgument;
		double numericalArgument = defaultNumericalArgument;
		DateTime dateTimeArgument = defaultDateTimeArgument;
		double[] values;
		DateTime[] dateTimeValues;
		double internalArgument = double.NaN;
		double[] internalValues;
		string toolTipHint = null;
		bool isEmpty = false;
		bool isAuxiliary = false;
		int id = -1;
		Color color = Color.Empty;
		List<SeriesPoint> sourcePoints = new List<SeriesPoint>();
		bool IsWebChartControl { get { return ChartContainer != null && ChartContainer.ControlType == ChartContainerType.WebControl; } }
		internal Series Series { get { return Owner as Series; } }
		internal bool IsAuxiliary { get { return isAuxiliary; } }
		internal bool HasRelations { get { return relations != null; } }
		internal ScaleType ValueScaleType {
			get { return valueScaleType; }
			set { valueScaleType = value; }
		}
		internal ScaleType ArgumentScaleType { get { return argumentScaleType; } }
		internal List<SeriesPoint> SourcePoints { get { return sourcePoints; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointAnnotations"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.Annotations"),
		Editor("DevExpress.XtraCharts.Design.AnnotationCollectionEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(typeof(CollectionTypeConverter))
		]
		public AnnotationCollection Annotations {
			get {
				if (annotations == null)
					annotations = new AnnotationCollection(new AnnotationCollectionSeriesPointBehavior(this));
				annotations.Update();
				return annotations;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointRelations"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.Relations"),
		TypeConverter(typeof(CollectionTypeConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public SeriesPointRelationCollection Relations {
			get {
				if (!HasRelations)
					relations = new SeriesPointRelationCollection(this);
				return relations;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointValues"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.Values"),
		TypeConverter(typeof(ValuesTypeConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public double[] Values {
			get { return values; }
			set {
				if (Series != null && !Series.UnboundMode)
					throw new SeriesPointValueChangingException(ChartLocalizer.GetString(ChartStringId.MsgDenyChangeSeriesPointValue));
				InitializeValues(value);
				RaisePropertyChanged(ValuesProperty);
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointDateTimeValues"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.DateTimeValues"),
		TypeConverter(typeof(DateTimeValuesTypeConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public DateTime[] DateTimeValues {
			get { return dateTimeValues; }
			set {
				if (Series != null && !Series.UnboundMode)
					throw new SeriesPointValueChangingException(ChartLocalizer.GetString(ChartStringId.MsgDenyChangeSeriesPointValue));
				InitializeValues(value);
				RaisePropertyChanged(ValuesProperty);
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty
		]
		public string ValuesSerializable {
			get {
				string[] strings = new string[Length];
				for (int i = 0; i < strings.Length; i++)
					if (valueScaleType == ScaleType.DateTime)
						strings[i] = dateTimeValues[i].ToString(SerializingUtils.SerializableDateTimeFormat, CultureInfo.InvariantCulture);
					else
						strings[i] = values[i].ToString(CultureInfo.InvariantCulture);
				return SerializingUtils.StringArrayToString(strings);
			}
			set {
				string[] desializedValues = SerializingUtils.StringToStringArray(value);
				if (desializedValues.Length == 0)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArrayOfValues));
				double number;
				if (double.TryParse(desializedValues[0], NumberStyles.Any, CultureInfo.InvariantCulture, out number)) {
					double[] numericValues = new double[desializedValues.Length];
					for (int i = 0; i < desializedValues.Length; i++)
						numericValues[i] = double.Parse(desializedValues[i], CultureInfo.InvariantCulture);
					InitializeValues(numericValues);
				}
				else {
					DateTime dateTime;
					if (DateTime.TryParse(desializedValues[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) {
						DateTime[] dateTimeValues = new DateTime[desializedValues.Length];
						for (int i = 0; i < desializedValues.Length; i++)
							dateTimeValues[i] = DateTime.Parse(desializedValues[i], CultureInfo.InvariantCulture);
						InitializeValues(dateTimeValues);
					}
					else
						throw new InternalException("Illegal valueScaleType when XtraDeserializing");
				}
				if (!this.Loading)
					RaisePropertyChanged(ValuesProperty);
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointArgument"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.Argument"),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public string Argument {
			get {
				if (string.IsNullOrEmpty(qualitativeArgument) && argument != null)
					qualitativeArgument = argument.ToString();
				return qualitativeArgument;
			}
			set {
				if (Series != null && !Series.UnboundMode)
					throw new SeriesPointArgumentChangingException(ChartLocalizer.GetString(ChartStringId.MsgDenyChangeSeriesPointArgument));
				if (string.IsNullOrEmpty(value))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArgument));
				SetQualitativeArgument(value, true, true);
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.Color"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Color Color {
			get { return color; }
			set {
				SendNotification(new ElementWillChangeNotification(this));
				color = value;
				RaiseControlChanged(new PropertyUpdateInfo(this, "Color"));
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),		
		XtraSerializableProperty
		]
		public string ColorSerializable {
			get {
				return color.IsEmpty ? string.Empty : System.Drawing.ColorTranslator.ToHtml(color);
			}
			set {
				if (!string.IsNullOrEmpty(value))
					try {
						this.color = System.Drawing.ColorTranslator.FromHtml(value);
					}
					finally { }
			}
		}
		[
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public DateTime DateTimeArgument {
			get {
				if (argumentScaleType != ScaleType.DateTime)
					return DateTime.MinValue;
				if ((dateTimeArgument == DateTime.MinValue) && (argument != null) && (argument is DateTime))
					dateTimeArgument = (DateTime)argument;
				return dateTimeArgument;
			}
			set {
				if (Series != null && !Series.UnboundMode)
					throw new SeriesPointArgumentChangingException(ChartLocalizer.GetString(ChartStringId.MsgDenyChangeSeriesPointArgument));
				if (!Loading)
					SetDateTimeArgument(value, true);
			}
		}
		[
		Browsable(false),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public double NumericalArgument {
			get {
				if (argumentScaleType != ScaleType.Numerical)
					return double.NaN;
				if (double.IsNaN(numericalArgument))
					numericalArgument = (double)argument;
				return numericalArgument;
			}
			set {
				if (Series != null && !Series.UnboundMode)
					throw new SeriesPointArgumentChangingException(ChartLocalizer.GetString(ChartStringId.MsgDenyChangeSeriesPointArgument));
				if (!Loading)
					SetNumericalArgument(value, true);
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public string ArgumentSerializable {
			get {
				if (argument != null) {
					ScaleType scaleType = Series != null ? Series.ArgumentScaleType : argumentScaleType;
					switch (scaleType) {
						case ScaleType.Numerical:
							return NumericalArgument.ToString(CultureInfo.InvariantCulture);
						case ScaleType.DateTime:
							return DateTimeArgument.ToString(SerializingUtils.SerializableDateTimeFormat, CultureInfo.InvariantCulture);
					}
				}
				return Argument;
			}
			set {
				if (Owner != null && !Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				argumentSerializable = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointItem"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.Item"),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public double this[int index] { get { return values[index]; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointLength"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.Length"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public int Length { get { return values.Length; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointTag"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.Tag"),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new object Tag {
			get { return base.Tag; }
			set {
				base.Tag = value;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointToolTipHint"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.ToolTipHint"),
		Category(Categories.Data),
		Localizable(true),
		XtraSerializableProperty
		]
		public string ToolTipHint {
			get { return toolTipHint; }
			set {
				if (value != toolTipHint) {
					SendNotification(new ElementWillChangeNotification(this));
					toolTipHint = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SeriesPointIsEmpty"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SeriesPoint.IsEmpty"),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty
		]
		public bool IsEmpty {
			get { return isEmpty; }
			set {
				if (isEmpty != value) {
					SendNotification(new ElementWillChangeNotification(this));
					isEmpty = value;
					if (isEmpty) {
						for (int i = 0; i < values.Length; i++) {
							values[i] = 0.0;
							dateTimeValues[i] = DateTime.MinValue;
						}
					}
					RaisePropertyChanged(ValuesProperty);
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty
		]
		public int SeriesPointID {
			get { return id; }
			set {
				if (Owner != null && !Loading)
					throw new NotSupportedException(ChartLocalizer.GetString(ChartStringId.MsgInternalPropertyChangeError));
				id = value;
			}
		}
		public SeriesPoint() {
			values = new double[] { 0.0 };
			dateTimeValues = new DateTime[] { DateTime.MinValue };
			isEmpty = true;
		}
		public SeriesPoint(string argument)
			: this() {
			if (string.IsNullOrEmpty(argument))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArgument));
			SetQualitativeArgument(argument, true, false);
		}
		public SeriesPoint(string argument, params double[] values) {
			if (string.IsNullOrEmpty(argument))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArgument));
			SetQualitativeArgument(argument, true, false);
			InitializeValues(values);
		}
		public SeriesPoint(string argument, params DateTime[] values) {
			if (string.IsNullOrEmpty(argument))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArgument));
			SetQualitativeArgument(argument, true, false);
			InitializeValues(values);
		}
		public SeriesPoint(object argument)
			: this() {
			SetArgument(argument, true, false);
		}
		public SeriesPoint(object argument, params object[] values) {
			SetArgument(argument, true, false);
			InitializeValues(values);
		}
		public SeriesPoint(DateTime argument)
			: this() {
			SetDateTimeArgument(argument, false);
		}
		public SeriesPoint(DateTime argument, params double[] values) {
			SetDateTimeArgument(argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(DateTime argument, params DateTime[] values) {
			SetDateTimeArgument(argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(double argument)
			: this() {
			SetNumericalArgument(argument, false);
		}
		public SeriesPoint(double argument, params double[] values) {
			SetNumericalArgument(argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(double argument, params DateTime[] values) {
			SetNumericalArgument(argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(float argument)
			: this() {
			SetNumericalArgument((double)argument, false);
		}
		public SeriesPoint(float argument, params double[] values) {
			SetNumericalArgument((double)argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(float argument, params DateTime[] values) {
			SetNumericalArgument((double)argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(int argument)
			: this() {
			SetNumericalArgument((double)argument, false);
		}
		public SeriesPoint(int argument, params double[] values) {
			SetNumericalArgument((double)argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(int argument, params DateTime[] values) {
			SetNumericalArgument((double)argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(long argument)
			: this() {
			SetNumericalArgument((double)argument, false);
		}
		public SeriesPoint(long argument, params double[] values) {
			SetNumericalArgument((double)argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(long argument, params DateTime[] values) {
			SetNumericalArgument((double)argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(short argument)
			: this() {
			SetNumericalArgument((double)argument, false);
		}
		public SeriesPoint(short argument, params double[] values) {
			SetNumericalArgument((double)argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(short argument, params DateTime[] values) {
			SetNumericalArgument((double)argument, false);
			InitializeValues(values);
		}
		public SeriesPoint(decimal argument)
			: this() {
			SetNumericalArgument(Convert.ToDouble(argument), false);
		}
		public SeriesPoint(decimal argument, params double[] values) {
			SetNumericalArgument(Convert.ToDouble(argument), false);
			InitializeValues(values);
		}
		public SeriesPoint(decimal argument, params DateTime[] values) {
			SetNumericalArgument(Convert.ToDouble(argument), false);
			InitializeValues(values);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public SeriesPoint(object argument, object[] values, int id) {
			SetArgument(argument, true, false);
			InitializeValues(values);
			this.id = id;
		}
		internal SeriesPoint(int dimension, ScaleType valueScaleType)
			: this() {
			argument = null;
			qualitativeArgument = defaultQualitativeArgument;
			numericalArgument = defaultNumericalArgument;
			dateTimeArgument = defaultDateTimeArgument;
			argumentScaleType = ScaleType.Qualitative;
			values = new double[dimension];
			dateTimeValues = new DateTime[dimension];
			DateTime now = DateTime.Now;
			for (int i = 0; i < dimension; i++)
				dateTimeValues[i] = now;
			this.valueScaleType = valueScaleType;
		}
		internal SeriesPoint(int dimension, ScaleType argumentScaleType, ScaleType valueScaleType)
			: this() {
			argument = null;
			qualitativeArgument = defaultQualitativeArgument;
			numericalArgument = defaultNumericalArgument;
			dateTimeArgument = defaultDateTimeArgument;
			this.argumentScaleType = argumentScaleType;
			values = new double[dimension];
			dateTimeValues = new DateTime[dimension];
			DateTime now = DateTime.Now;
			for (int i = 0; i < dimension; i++)
				dateTimeValues[i] = now;
			this.valueScaleType = valueScaleType;
		}
		internal SeriesPoint(object argument, double internalArgument, object[] values, double[] internalValues, object tag) {
			SetArgument(argument, false, false);
			InitializeValues(values);
			this.internalArgument = internalArgument;
			this.internalValues = internalValues;
			this.InitTag(tag);
		}
		internal SeriesPoint(object argument, double internalArgument, object[] values, double[] internalValues, object tag, string toolTipHint)
			: this(argument, internalArgument, values, internalValues, tag) {
			this.toolTipHint = toolTipHint;
		}
		SeriesPoint(AggregatedSeriesPoint aggregatedPoint) {
			SetArgument(aggregatedPoint.Argument, true, false);
			if (aggregatedPoint.Values != null)
				InitializeValues(aggregatedPoint.Values);
			else if (aggregatedPoint.DateTimeValues != null)
				InitializeValues(aggregatedPoint.DateTimeValues);
			isEmpty = aggregatedPoint.IsEmpty;
			if (aggregatedPoint.SourcePoints.Count == 1) {
				SeriesPoint seriesPoint = aggregatedPoint.SourcePoints[0].SeriesPoint as SeriesPoint;
				if (seriesPoint != null) {
					InitTag(seriesPoint.Tag);
					sourcePoints.Add(seriesPoint);
				}
			}
			else {
				List<object> tags = new List<object>();
				foreach (RefinedPoint point in aggregatedPoint.SourcePoints) {
					SeriesPoint seriesPoint = point.SeriesPoint as SeriesPoint;
					if (seriesPoint != null) {
						tags.Add(seriesPoint.Tag);
						sourcePoints.Add(seriesPoint);
					}
				}
				InitTag(tags);
			}
			this.isAuxiliary = aggregatedPoint.IsAuxiliary;
			Owner = aggregatedPoint.Series as ChartElement;
		}
		#region ICustomTypeDescriptor implementation
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return new SeriesPointPropertyDescriptorCollection(TypeDescriptor.GetProperties(this, true), valueScaleType);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return new SeriesPointPropertyDescriptorCollection(TypeDescriptor.GetProperties(this, true), valueScaleType);
		}
		string ICustomTypeDescriptor.GetClassName() {
			return GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return GetType().Name;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion
		#region ISeriesPoint implementation
		object ISeriesPoint.UserArgument {
			get {
				if (Series != null && Series.ActualArgumentScaleType == ScaleType.Qualitative)
					return Argument;
				return argument;
			}
		}
		string ISeriesPoint.QualitativeArgument { get { return Argument; } }
		double ISeriesPoint.NumericalArgument { get { return NumericalArgument; } }
		DateTime ISeriesPoint.DateTimeArgument { get { return DateTimeArgument; } }
		Scale ISeriesPoint.ArgumentScaleType { get { return (Scale)argumentScaleType; } }
		double ISeriesPoint.InternalArgument {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		double[] ISeriesPoint.UserValues { get { return values; } }
		double[] ISeriesPoint.AnimatedValues { get { throw new NotImplementedException(); } }
		double[] ISeriesPoint.InternalValues {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool ISeriesPoint.IsEmpty(Scale valueScaleType) { return isEmpty; }
		object ISeriesPoint.ToolTipHint { get { return ToolTipHint; } }
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "IsEmpty")
				return ShouldSerializeIsEmpty();
			if (propertyName == "SeriesPointID")
				return ShouldSerializeSeriesPointID();
			if (propertyName == "Relations")
				return ShouldSerializeRelations();
			if (propertyName == "ArgumentSerializable")
				return ShouldSerializeArgumentSerializable();
			if (propertyName == "ValuesSerializable")
				return !IsEmpty;
			if (propertyName == "ColorSerializable")
				return ShouldSerializeColorSerializable();
			if (propertyName == "ToolTipHint")
				return ShouldSerializeToolTipHint();
			return base.XtraShouldSerialize(propertyName);
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if (propertyName == "Relations")
				Relations.Add((Relation)e.Item.Value);
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if (propertyName == "Relations")
				return new TaskLink();
			return null;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeIsEmpty() {
			return isEmpty && (IsWebChartControl || XtraSerializing);
		}
		bool ShouldSerializeValuesInternal() {
			return !isEmpty && !ShouldSerializeDateTimeValuesInternal();
		}
		protected bool ShouldSerializeDateTimeValuesInternal() {
			return !isEmpty && Owner != null && Series.ValueScaleType == ScaleType.DateTime;
		}
		protected bool ShouldSerializeSeriesPointIDInternal() {
			return id >= 0 && Series != null && (Series.View.IsSupportedRelations || Series.View.SerializeSeriesPointID || Annotations.Count > 0);
		}
		bool ShouldSerializeValues() {
			return IsWebChartControl && ShouldSerializeValuesInternal();
		}
		bool ShouldSerializeColor() {
			return false;
		}
		bool ShouldSerializeColorSerializable() {
			return !Color.IsEmpty;
		}
		bool ShouldSerializeDateTimeValues() {
			return IsWebChartControl && ShouldSerializeDateTimeValuesInternal();
		}
		bool ShouldSerializeSeriesPointID() {
			return (IsWebChartControl || XtraSerializing) && ShouldSerializeSeriesPointIDInternal();
		}
		bool ShouldSerializeRelations() {
			return Relations != null && Series != null && Series.View.IsSupportedRelations && Relations.Count > 0;
		}
		bool ShouldSerializeArgument() {
			return false;
		}
		bool ShouldSerializeArgumentSerializable() {
			return IsWebChartControl || XtraSerializing;
		}
		bool ShouldSerializeTag() {
			return false;
		}
		bool ShouldSerializeDateTimeArgument() {
			return false;
		}
		bool ShouldSerializeNumericalArgument() {
			return false;
		}
		bool ShouldSerializeToolTipHint() {
			return !String.IsNullOrEmpty(toolTipHint);
		}
		protected internal override bool ShouldSerialize() {
			return true;
		}
		#endregion
		void InitializeValues(object[] values) {
			if (values == null) {
				this.values = new double[] { 0.0 };
				dateTimeValues = new DateTime[] { DateTime.MinValue };
				isEmpty = true;
				return;
			}
			if (values.Length == 0)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArrayOfValues));
			this.values = new double[values.Length];
			dateTimeValues = new DateTime[values.Length];
			if (values[0].GetType() == typeof(DateTime)) {
				dateTimeValues = new DateTime[values.Length];
				for (int i = 0; i < values.Length; i++)
					dateTimeValues[i] = (DateTime)values[i];
				valueScaleType = ScaleType.DateTime;
			}
			else {
				for (int i = 0; i < values.Length; i++) {
					IConvertible value = values[i] as IConvertible;
					if (value == null)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectArrayOfValues));
					this.values[i] = Convert.ToDouble(value);
				}
				valueScaleType = ScaleType.Numerical;
			}
			isEmpty = false;
		}
		void InitializeValues(double[] values) {
			if (values.Length == 0)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArrayOfValues));
			this.values = values;
			dateTimeValues = new DateTime[values.Length];
			valueScaleType = ScaleType.Numerical;
			isEmpty = false;
		}
		void InitializeValues(DateTime[] dateTimeValues) {
			if (dateTimeValues.Length == 0)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArrayOfValues));
			this.dateTimeValues = dateTimeValues;
			values = new double[dateTimeValues.Length];
			valueScaleType = ScaleType.DateTime;
			isEmpty = false;
		}
		void SetQualitativeArgument(string argument, bool parse, bool needRaiseChangedProperty) {
			if (parse) {
				if (double.TryParse(argument, out numericalArgument))
					SetNumericalArgument(numericalArgument, needRaiseChangedProperty);
				else if (DateTime.TryParse(argument, out dateTimeArgument))
					SetDateTimeArgument(dateTimeArgument, needRaiseChangedProperty);
				else
					SetQualitativeArgument(argument, needRaiseChangedProperty);
			}
			else
				SetQualitativeArgument(argument, needRaiseChangedProperty);
			qualitativeArgument = argument;
		}
		void SetQualitativeArgument(string argument, bool needRaiseChangedProperty) {
			this.argument = argument;
			numericalArgument = defaultNumericalArgument;
			dateTimeArgument = defaultDateTimeArgument;
			qualitativeArgument = argument;
			argumentScaleType = ScaleType.Qualitative;
			if (needRaiseChangedProperty && !this.Loading)
				RaisePropertyChanged(ArgumentProperty);
		}
		void SetNumericalArgument(double argument, bool needRaiseChangedProperty) {
			this.argument = argument;
			numericalArgument = argument;
			dateTimeArgument = defaultDateTimeArgument;
			qualitativeArgument = defaultQualitativeArgument;
			argumentScaleType = ScaleType.Numerical;
			if (needRaiseChangedProperty && !this.Loading)
				RaisePropertyChanged(ArgumentProperty);
		}
		void SetDateTimeArgument(DateTime argument, bool needRaiseChangedProperty) {
			this.argument = argument;
			numericalArgument = defaultNumericalArgument;
			dateTimeArgument = argument;
			qualitativeArgument = defaultQualitativeArgument;
			argumentScaleType = ScaleType.DateTime;
			if (needRaiseChangedProperty && !this.Loading)
				RaisePropertyChanged(ArgumentProperty);
		}
		void DisposeRelations() {
			if (relations != null) {
				relations.Dispose();
				relations = null;
			}
		}
		void DeliverToDataContainer(ChartUpdateInfoBase updateInfo) {
			var dataContainer = GetOwner<DataContainer>();
			if (dataContainer != null)
				dataContainer.DataChanged(updateInfo);
		}
		protected override ChartElement CreateObjectForClone() {
			return new SeriesPoint(0, valueScaleType);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				DisposeRelations();
				if (annotations != null) {
					annotations.Dispose();
					annotations = null;
				}
			}
			base.Dispose(disposing);
		}
		internal bool CheckValueScaleType(ScaleType scaleType) {
			return isEmpty || scaleType == valueScaleType;
		}
		internal void SetArgument(object argument, bool parse, bool needRaiseChangedProperty) {
			if (argument == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgEmptyArgument));
			Type argumentType = argument.GetType();
			if (argumentType == typeof(DateTime)) {
				SetDateTimeArgument((DateTime)argument, needRaiseChangedProperty);
			}
			else {
				if (argumentType == typeof(double) || argumentType == typeof(float) || argumentType == typeof(int) ||
					argumentType == typeof(uint) || argumentType == typeof(long) || argumentType == typeof(ulong) ||
					argumentType == typeof(decimal) || argumentType == typeof(short) || argumentType == typeof(ushort) ||
					argumentType == typeof(byte) || argumentType == typeof(sbyte) || argumentType == typeof(char)) {
					SetNumericalArgument(Convert.ToDouble(argument), needRaiseChangedProperty);
				}
				else
					SetQualitativeArgument(argument.ToString(), parse, needRaiseChangedProperty);
			}
		}
		internal void UpdateAnnotationRepository() {
			if (annotations != null)
				annotations.UpdateAnnotationRepository();
		}
		internal void ClearAnnotations() {
			Annotations.ClearAnnotations();
		}
		internal void SetID(int id) {
			this.id = id;
		}
		internal void IncreaseDimension(int newDimension) {
			if (newDimension <= values.Length)
				return;
			double[] newValues = new double[newDimension];
			DateTime[] newDateTimeValues = new DateTime[newDimension];
			for (int i = 0; i < values.Length; i++) {
				newValues[i] = values[i];
				newDateTimeValues[i] = dateTimeValues[i];
			}
			double lastValue = values[values.Length - 1];
			DateTime lastDateTimeValue = dateTimeValues[dateTimeValues.Length - 1];
			for (int i = values.Length; i < newDimension; i++) {
				newValues[i] = lastValue;
				newDateTimeValues[i] = lastDateTimeValue;
			}
			values = newValues;
			dateTimeValues = newDateTimeValues;
		}
		internal void OnEndLoading() {
			if (ShouldSerializeArgumentSerializable() && !String.IsNullOrEmpty(argumentSerializable)) {
				if (Series == null)
					SetArgument(argumentSerializable, true, false);
				else
					switch (Series.ArgumentScaleType) {
						case ScaleType.Numerical:
							SetArgument(double.Parse(argumentSerializable, CultureInfo.InvariantCulture), false, false);
							break;
						case ScaleType.DateTime:
							SetArgument(DateTime.Parse(argumentSerializable, CultureInfo.InvariantCulture), false, false);
							break;
						default:
							SetArgument(argumentSerializable, true, false);
							break;
					}
			}
		}
		internal object GetValueObject(int index) {
			return valueScaleType == ScaleType.DateTime ? (object)DateTimeValues[index] : (object)Values[index];
		}
		internal void RaisePropertyChanged(string propertyName) {
			if ((Series != null) && !Series.PointsUpdateInProcess)
				DeliverToDataContainer(new PropertyUpdateInfo<ISeries, ISeriesPoint>(this, propertyName, null, null, Series));
		}
		public string GetValueString(int index) {
			if (valueScaleType == ScaleType.Numerical)
				return values[index].ToString();
			if (Series != null && Series.Chart != null && Series.Chart.Diagram != null) {
				IXYSeriesView view = Series.View as IXYSeriesView;
				if (view != null) {
					PatternParser patternParser = new PatternParser(view.AxisYData.Label.TextPattern, (IPatternHolder)view.AxisYData.Label);
					patternParser.SetContext(dateTimeValues[index]);
					return patternParser.GetText();
				}
			}
			return dateTimeValues[index].ToShortDateString();
		}
		public override bool Equals(object obj) {
			SeriesPoint point = obj as SeriesPoint;
			if (point == null || point.argumentScaleType != argumentScaleType || point.valueScaleType != valueScaleType)
				return false;
			switch (point.argumentScaleType) {
				case ScaleType.Qualitative:
					if (qualitativeArgument != point.qualitativeArgument)
						return false;
					break;
				case ScaleType.Numerical:
					if (numericalArgument != point.numericalArgument)
						return false;
					break;
				case ScaleType.DateTime:
					if (dateTimeArgument != point.dateTimeArgument)
						return false;
					break;
			}
			switch (point.valueScaleType) {
				case ScaleType.Numerical:
					if (point.values.Length != values.Length)
						return false;
					for (int i = 0; i < values.Length; i++)
						if (point.values[i] != values[i])
							return false;
					break;
				case ScaleType.DateTime:
					if (point.dateTimeValues.Length != dateTimeValues.Length)
						return false;
					for (int i = 0; i < values.Length; i++)
						if (point.dateTimeValues[i] != dateTimeValues[i])
							return false;
					break;
				default:
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SeriesPoint point = obj as SeriesPoint;
			if (point == null)
				return;
			argument = point.argument;
			numericalArgument = point.numericalArgument;
			dateTimeArgument = point.dateTimeArgument;
			qualitativeArgument = point.qualitativeArgument;
			argumentScaleType = point.argumentScaleType;
			values = new double[point.values.Length];
			color = point.color;
			point.values.CopyTo(values, 0);
			dateTimeValues = new DateTime[point.dateTimeValues.Length];
			point.dateTimeValues.CopyTo(dateTimeValues, 0);
			id = point.id;
			DisposeRelations();
			if (point.relations != null)
				Relations.Assign(point.relations);
			isEmpty = point.isEmpty;
			toolTipHint = point.toolTipHint;
		}
	}
}
namespace DevExpress.XtraCharts.Native {
	public class SeriesPointComparer : IComparer<ISeriesPoint> {
		int valueIndex;
		SortingMode mode;
		int IComparer<ISeriesPoint>.Compare(ISeriesPoint x, ISeriesPoint y) {
			if (mode == SortingMode.None)
				return 0;
			SeriesPoint p1 = (SeriesPoint)x;
			SeriesPoint p2 = (SeriesPoint)y;
			if (valueIndex < 0 || p1.Values.Length < valueIndex)
				return 0;
			int res;
			if (valueIndex == 0) {
				switch (x.ArgumentScaleType) {
					case Scale.Numerical:
						res = p1.NumericalArgument == p2.NumericalArgument ? 0 : p1.NumericalArgument > p2.NumericalArgument ? 1 : -1;
						break;
					case Scale.DateTime:
						res = DateTime.Compare(p1.DateTimeArgument, p2.DateTimeArgument);
						break;
					default:
						res = String.Compare(p1.Argument, p2.Argument);
						break;
				}
			}
			else if (p1.IsEmpty)
				res = p2.IsEmpty ? 0 : -1;
			else if (p2.IsEmpty)
				res = 1;
			else {
				if (p1.ValueScaleType != p2.ValueScaleType)
					throw new InternalException("ValueScaleType");
				if (p1.ValueScaleType == ScaleType.DateTime) {
					long diff = (p1.DateTimeValues[valueIndex - 1] - p2.DateTimeValues[valueIndex - 1]).Ticks;
					res = diff == 0 ? 0 : (diff < 0 ? -1 : 1);
				}
				else {
					double diff = p1.Values[valueIndex - 1] - p2.Values[valueIndex - 1];
					res = diff == 0 ? 0 : (diff < 0 ? -1 : 1);
				}
			}
			return mode == SortingMode.Ascending ? res : -res;
		}
		public SeriesPointComparer(SortingMode mode, int valueIndex) {
			this.mode = mode;
			this.valueIndex = valueIndex;
		}
	}
}
