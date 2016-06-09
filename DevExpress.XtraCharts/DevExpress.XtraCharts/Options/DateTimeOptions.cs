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
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using System.Drawing;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum DateTimeFormat {
		ShortDate = DateTimeOptionsFormat.ShortDate,
		LongDate = DateTimeOptionsFormat.LongDate,
		ShortTime = DateTimeOptionsFormat.ShortTime,
		LongTime = DateTimeOptionsFormat.LongTime,
		General = DateTimeOptionsFormat.General,
		MonthAndDay = DateTimeOptionsFormat.MonthAndDay,
		MonthAndYear = DateTimeOptionsFormat.MonthAndYear,
		QuarterAndYear = DateTimeOptionsFormat.QuarterAndYear,
		Custom = DateTimeOptionsFormat.Custom
	}
	[
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design"),
	TypeConverter(typeof(DateTimeOptionsTypeConverter))
	]
	public class DateTimeOptions : ChartElement, IDateTimeOptions {
		internal const DateTimeFormat DefaultFormat = DateTimeFormat.ShortDate;
		DateTimeFormat format = DefaultFormat;
		DateTimeFormat autoFormat = DefaultFormat;
		string formatString = String.Empty;
		string autoFormatString = String.Empty;
		bool autoFormatEnabled;
		internal Axis2D Axis {
			get {
				if (Owner is AxisLabel)
					return ((AxisLabel)Owner).Axis as Axis2D;
				return null;
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DateTimeOptionsFormat"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DateTimeOptions.Format"),
		XtraSerializableProperty,
		NonTestableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public DateTimeFormat Format {
			get { return GetActualFormat(); }
			set {
				if (format != value) {
					SendNotification(new ElementWillChangeNotification(this));
					format = value;
					autoFormatEnabled = false;
					ResetAxisLabelSizeCache();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DateTimeOptionsFormatString"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DateTimeOptions.FormatString"),
		Localizable(true),
		XtraSerializableProperty,
		NonTestableProperty
		]
		public string FormatString {
			get { return GetActualFormatString(); }
			set {
				if (!string.Equals(formatString, value)) {
					SendNotification(new ElementWillChangeNotification(this));
					formatString = value;
					autoFormatEnabled = false;
					ResetAxisLabelSizeCache();
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("DateTimeOptionsAutoFormat"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.DateTimeOptions.AutoFormat"),
		Localizable(true),
		TypeConverter(typeof(BooleanTypeConverter)),
		XtraSerializableProperty,
		RefreshProperties(RefreshProperties.All)
		]
		public bool AutoFormat {
			get {
				return autoFormatEnabled;
			}
			set {
				if (autoFormatEnabled != value) {
					SendNotification(new ElementWillChangeNotification(this));
					autoFormatEnabled = value;
					ResetAxisLabelSizeCache();
					RaiseControlChanged();
				}
			}
		}
		internal DateTimeOptions(ChartElement owner) : base(owner) {
			this.autoFormatEnabled = true;
		}
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Format":
					return ShouldSerializeFormat();
				case "FormatString":
					return ShouldSerializeFormatString();
				case "AutoFormat":
					return ShouldSerializeAutoFormat();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeFormat() {
			return (format != DefaultFormat) && !autoFormatEnabled;
		}
		void ResetFormat() {
			Format = DefaultFormat;
		}
		bool ShouldSerializeFormatString() {
			return !String.IsNullOrEmpty(formatString) && (format == DateTimeFormat.Custom) && !autoFormatEnabled;
		}
		void ResetFormatString() {
			FormatString = string.Empty;
		}
		bool ShouldSerializeAutoFormat() {
			return !autoFormatEnabled;
		}
		void ResetAutoFormat() {
			AutoFormat = true;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeFormat() || ShouldSerializeFormatString() || ShouldSerializeAutoFormat();
		}
		#endregion
		#region IDateTimeOptions
		DateTimeOptionsFormat IDateTimeOptions.Format {
			get {
				return (DateTimeOptionsFormat)GetActualFormat();
			}
		}
		string IDateTimeOptions.QuarterFormat { 
			get { 
				return ChartLocalizer.GetString(ChartStringId.QuarterFormat); 
			} 
		}
		string IDateTimeOptions.FormatString {
			get {
				return GetActualFormatString();
			}
		}
		#endregion
		DateTimeFormat GetActualFormat() {
			return (autoFormatEnabled) ? autoFormat : format;	
		}
		string GetActualFormatString() {
			return (autoFormatEnabled) ? autoFormatString : formatString;
		}
		void ResetAxisLabelSizeCache() {
			if (Axis != null)
				Axis.ResetScrollLabelSizeCache();
		}
		protected override ChartElement CreateObjectForClone() {
			return new DateTimeOptions((AxisBase)null);
		}
		internal bool AllowDesignTimeEditing() {
			PointOptions options = Owner as PointOptions;
			if (options != null) {
				Chart chart = options.ChartContainer.Chart;
				if (chart != null)
					return !PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(chart.DataContainer.PivotGridDataSourceOptions, options.SeriesBase);
			}
			AxisLabel label = Owner as AxisLabel;
			AxisXBase axis = label != null ? label.Axis as AxisXBase : null;
			if (axis != null) {
				Chart chart = axis.Diagram != null ? axis.Diagram.Chart : null;
				if (chart != null)
					return !PivotGridDataSourceUtils.IsAutoLayoutSettingsEnabled(chart.DataContainer.PivotGridDataSourceOptions, axis, false);
			}
			return true;
		}
		internal void SetAutoFormat(DateTimeFormat format, string formatString) {
			this.autoFormatString = formatString;
			this.autoFormat = format;
			ResetAxisLabelSizeCache();
		}
		public override string ToString() {
			return "(DateTimeOptions)";
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			DateTimeOptions options = obj as DateTimeOptions;
			if (options != null) {
				format = options.format;
				formatString = options.formatString;
				autoFormatEnabled = options.autoFormatEnabled;
				autoFormatString = options.autoFormatString;
			}
		}
		public override bool Equals(object obj) {
			DateTimeOptions options = obj as DateTimeOptions;
			return options != null && format == options.format && formatString == options.formatString && autoFormatEnabled == options.autoFormatEnabled;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
}
