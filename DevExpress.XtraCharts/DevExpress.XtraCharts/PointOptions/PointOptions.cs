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
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.Utils;
using System.Drawing.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum PointView {
		Argument = PointViewKind.Argument,
		Values = PointViewKind.Values,
		ArgumentAndValues = PointViewKind.ArgumentAndValues,
		SeriesName = PointViewKind.SeriesName,
		Undefined = PointViewKind.Undefined
	}
	[
	TypeConverter(typeof(PointOptionsTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class PointOptions : ChartElement {
		internal static string ConstructPatternFromPointOptions(PointOptions pointOptions, ScaleType argumentScaleType, ScaleType valueScaleType) {
			string pattern = pointOptions.Pattern;
			if (pattern.Contains("{A}"))
				pattern = pattern.Replace("{A}", ConstructArgumentPattern(pointOptions, argumentScaleType));
			if (pattern.Contains("{V}"))
				pattern = pattern.Replace("{V}", ConstructValuePattern(pointOptions, valueScaleType));
			return pattern;
		}
		internal static string ConstructArgumentPattern(PointOptions pointOptions, ScaleType argumentScaleType) {
			string argumentPattern = "{" + PatternUtils.ArgumentPlaceholder;
			switch (argumentScaleType) {
				case ScaleType.Numerical:
					argumentPattern += ":" + NumericOptionsHelper.GetFormatString(pointOptions.ArgumentNumericOptions);
					break;
				case XtraCharts.ScaleType.DateTime:
					argumentPattern += ":" + DateTimeOptionsHelper.GetFormatString(pointOptions.ArgumentDateTimeOptions);
					break;
				case XtraCharts.ScaleType.Qualitative:
					break;
			}
			return argumentPattern + "}";
		}
		internal static string ConstructValuePattern(PointOptions pointOptions, ScaleType valueScaleType) {
			string valuePattern = "{" + PatternUtils.ValuePlaceholder;
			FullStackedPointOptions fullStackedPointOptions = pointOptions as FullStackedPointOptions;
			if (fullStackedPointOptions != null && fullStackedPointOptions.PercentOptions.ValueAsPercent) {
				valuePattern = "{" + PatternUtils.PercentValuePlaceholder + ":" + NumericOptionsHelper.GetFormatString(pointOptions.ValueNumericOptions);
				if (pointOptions.ValueNumericOptions.Format == NumericFormat.General)
					valuePattern += fullStackedPointOptions.PercentOptions.PercentageAccuracy.ToString();
			}
			else {
				SimplePointOptions simplePointOptions = pointOptions as SimplePointOptions;
				if (simplePointOptions != null && simplePointOptions.PercentOptions.ValueAsPercent) {
					valuePattern = "{" + PatternUtils.PercentValuePlaceholder + ":" + NumericOptionsHelper.GetFormatString(pointOptions.ValueNumericOptions);
					if (pointOptions.ValueNumericOptions.Format == NumericFormat.General)
						valuePattern += simplePointOptions.PercentOptions.PercentageAccuracy.ToString();
				}
				else {
					RangePointOptions rangePointOptions = pointOptions as RangePointOptions;
					if (rangePointOptions != null) {
						string valueFormat = string.Empty;
						switch (valueScaleType) {
							case ScaleType.Numerical:
								valueFormat = ":" + NumericOptionsHelper.GetFormatString(pointOptions.ValueNumericOptions);
								break;
							case ScaleType.DateTime:
								if (rangePointOptions != null && rangePointOptions.ValueAsDuration)
									valueFormat = GetValueDurationFormat(rangePointOptions);
								else
									valueFormat = ":" + DateTimeOptionsHelper.GetFormatString(pointOptions.ValueDateTimeOptions);
								break;
						}
						if (rangePointOptions.ValueAsDuration)
							valuePattern = "{" + PatternUtils.ValueDurationPlaceholder + valueFormat;
						else
							valuePattern = "{" + PatternUtils.Value1Placeholder + valueFormat + "}, {" + PatternUtils.Value2Placeholder + valueFormat;
					}
					else {
						StockPointOptions stockPointOptions = pointOptions as StockPointOptions;
						if (stockPointOptions != null)
							switch (stockPointOptions.ValueLevel) {
								case StockLevel.Low:
									valuePattern = "{" + PatternUtils.LowValuePlaceholder;
									break;
								case StockLevel.High:
									valuePattern = "{" + PatternUtils.HighValuePlaceholder;
									break;
								case StockLevel.Open:
									valuePattern = "{" + PatternUtils.OpenValuePlaceholder;
									break;
								case StockLevel.Close:
									valuePattern = "{" + PatternUtils.CloseValuePlaceholder;
									break;
							}
						switch (valueScaleType) {
							case ScaleType.Numerical:
								valuePattern += ":" + NumericOptionsHelper.GetFormatString(pointOptions.ValueNumericOptions);
								break;
							case ScaleType.DateTime:
								valuePattern += ":" + DateTimeOptionsHelper.GetFormatString(pointOptions.ValueDateTimeOptions);
								break;
						}
					}
				}
			}
			return valuePattern + "}";
		}
		internal static string GetValueDurationFormat(RangePointOptions rangePointOptions) {
			switch (rangePointOptions.ValueDurationFormat) {
				case TimeSpanFormat.TotalDays:
					return ":" + PatternUtils.DurationFormatTotalDays;
				case TimeSpanFormat.TotalHours:
					return ":" + PatternUtils.DurationFormatTotalHours;
				case TimeSpanFormat.TotalMinutes:
					return ":" + PatternUtils.DurationFormatTotalMinutes;
				case TimeSpanFormat.TotalSeconds:
					return ":" + PatternUtils.DurationFormatTotalSeconds;
				case TimeSpanFormat.TotalMilliseconds:
					return ":" + PatternUtils.DurationFormatTotalMilliseconds;
			}
			return string.Empty;
		}
		internal static string ChangeArgumentDateTimeFormat(string pattern, DateTimeFormat dateTimeFormat, string formatString) {
			string newFormat = string.Empty;
			if (pattern.Contains("{" + PatternUtils.ArgumentPlaceholder)) {
				switch (dateTimeFormat) {
					case DateTimeFormat.ShortDate:
						newFormat = "d";
						break;
					case DateTimeFormat.LongDate:
						newFormat = "D";
						break;
					case DateTimeFormat.ShortTime:
						newFormat = "t";
						break;
					case DateTimeFormat.LongTime:
						newFormat = "T";
						break;
					case DateTimeFormat.MonthAndDay:
						newFormat = "m";
						break;
					case DateTimeFormat.MonthAndYear:
						newFormat = "y";
						break;
					case DateTimeFormat.Custom:
						newFormat = formatString;
						break;
					case DateTimeFormat.QuarterAndYear:
						newFormat = "q";
						break;
					case DateTimeFormat.General:
					default:
						newFormat = "g";
						break;
				}
			}
			int placeholderPosition = pattern.IndexOf("{" + PatternUtils.ArgumentPlaceholder + ":");
			if (placeholderPosition >= 0) {
				string[] parts = pattern.Split('{', '}');
				foreach(string part in parts)
					if (pattern.IndexOf(part, placeholderPosition) == placeholderPosition + 1 && part.StartsWith(PatternUtils.ArgumentPlaceholder + ":"))
						return pattern.Replace(part, PatternUtils.ArgumentPlaceholder + ":" + newFormat);
				return pattern;
			}
			else
				return pattern.Replace("{" + PatternUtils.ArgumentPlaceholder + "}", "{" + PatternUtils.ArgumentPlaceholder + ":" + newFormat + "}");
		}
		string pattern = PointOptionsHelper.DefaultPattern;
		PointView pointView = (PointView)PointOptionsHelper.DefaultPointView;
		NumericOptions argumentNumericOptions;
		DateTimeOptions argumentDateTimeOptions;
		NumericOptions valueNumericOptions;
		DateTimeOptions valueDateTimeOptions;
		ChartElementSynchronizer synchronizer;
		protected Series Series {
			get {
				Series series = SeriesBase as Series;
				ChartDebug.Assert(series != null, "series can't be null");
				return series;
			}
		}
		internal ChartElementSynchronizer Synchronizer {
			get {
				return synchronizer;
			}
			set {
				if (synchronizer != value) {
					synchronizer = value;
					if (synchronizer != null)
						synchronizer.ElementChanged(this);
				}
			}
		}
		protected internal SeriesBase SeriesBase {
			get { return (SeriesBase)base.Owner; }
			set { base.Owner = value; }
		}
		protected internal virtual bool ShouldSynchronizeValuePercentPrecision { get { return false; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public string TypeNameSerializable { get { return GetType().Name; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DefaultValue(""),
		Obsolete("This property is obsolete now.")
		]
		public string HiddenSerializableString { get { return String.Empty; } set { } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PointOptionsPattern"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PointOptions.Pattern"),
		RefreshProperties(RefreshProperties.All),
		Editor(ControlConstants.MultilineStringEditor, typeof(UITypeEditor)),
		XtraSerializableProperty
		]
		public string Pattern {
			get { return pattern; }
			set {
				if (value != pattern) {
					SendNotification(new ElementWillChangeNotification(this));
					pattern = value;
					pointView = (PointView)PointOptionsHelper.ConvertToPointView(pattern);
					if (String.IsNullOrEmpty(pattern))
						pattern = PointOptionsHelper.ConvertToPattern((PointViewKind)pointView);
					OnPropertyChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PointOptionsPointView"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PointOptions.PointView"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public PointView PointView {
			get { return pointView; }
			set {
				if (value != pointView) {
					SendNotification(new ElementWillChangeNotification(this));
					pointView = value;
					string newPattern = PointOptionsHelper.ConvertToPattern((PointViewKind)pointView);
					if (!String.IsNullOrEmpty(newPattern))
						pattern = newPattern;
					OnPropertyChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PointOptionsArgumentNumericOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PointOptions.ArgumentNumericOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public NumericOptions ArgumentNumericOptions { get { return argumentNumericOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PointOptionsArgumentDateTimeOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PointOptions.ArgumentDateTimeOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public DateTimeOptions ArgumentDateTimeOptions { get { return argumentDateTimeOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PointOptionsValueNumericOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PointOptions.ValueNumericOptions"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public NumericOptions ValueNumericOptions { get { return valueNumericOptions; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("PointOptionsValueDateTimeOptions"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.PointOptions.ValueDateTimeOptions"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public DateTimeOptions ValueDateTimeOptions { get { return valueDateTimeOptions; } }
		public PointOptions() : base() {
			argumentNumericOptions = new NumericOptions(this);
			argumentDateTimeOptions = new DateTimeOptions(this);
			valueNumericOptions = new NumericOptions(this);
			valueDateTimeOptions = new DateTimeOptions(this);
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "PointView":
					return ShouldSerializePointView();
				case "ArgumentNumericOptions":
					return ShouldSerializeArgumentNumericOptions();
				case "ArgumentDateTimeOptions":
					return ShouldSerializeArgumentDateTimeOptions();
				case "ValueNumericOptions":
					return ShouldSerializeValueNumericOptions();
				case "ValueDateTimeOptions":
					return ShouldSerializeValueDateTimeOptions();
				case "Pattern":
					return ShouldSerializePattern();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeBeginText() {
			return false;
		}
		bool ShouldSerializeEndText() {
			return false;
		}
		bool ShouldSerializeSeparator() {
			return false;
		}
		bool ShouldSerializeHiddenSerializableString() {
			return false;
		}
		bool ShouldSerializePattern() {
			return false;
		}
		bool ShouldSerializePointView() {
			return false;
		}
		bool ShouldSerializeArgumentNumericOptions() {
			return false;
		}
		bool ShouldSerializeArgumentDateTimeOptions() {
			return false;
		}
		bool ShouldSerializeValueNumericOptions() {
			return false;
		}
		bool ShouldSerializeValueDateTimeOptions() {
			return false;
		}
		protected internal override bool ShouldSerialize() {
			return false;
		}
		#endregion
		void OnPropertyChanged() {
			RaiseControlChanged();
		}
		protected override bool ProcessChanged(ChartElement sender, ChartUpdateInfoBase changeInfo) {
			if (synchronizer != null)
				synchronizer.ElementChanged(this);
			return base.ProcessChanged(sender, changeInfo);
		}
		protected override ChartElement CreateObjectForClone() {
			return new PointOptions();
		}
		protected internal virtual int GetValuePercentPrecisionForSynchronization() {
			return -1;
		}
		protected internal virtual ValueToStringConverter CreateValueToStringConverter() {
			return new ValueToStringConverter(valueNumericOptions, valueDateTimeOptions);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			PointOptions options = obj as PointOptions;
			if (options != null) {
				pointView = options.pointView;
				pattern = options.pattern;
				argumentNumericOptions.Assign(options.argumentNumericOptions);
				argumentDateTimeOptions.Assign(options.argumentDateTimeOptions);
				valueNumericOptions.Assign(options.valueNumericOptions);
				valueDateTimeOptions.Assign(options.valueDateTimeOptions);
			}
		}
		public override bool Equals(object obj) {
			PointOptions options = obj as PointOptions;
			return options != null && pointView == options.pointView && pattern == options.pattern &&
				argumentNumericOptions.Equals(options.argumentNumericOptions) &&
				argumentDateTimeOptions.Equals(options.argumentDateTimeOptions) &&
				valueNumericOptions.Equals(options.valueNumericOptions) &&
				valueDateTimeOptions.Equals(options.valueDateTimeOptions);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
