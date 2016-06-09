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
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum PointView {
		Argument = PointViewKind.Argument,
		Values = PointViewKind.Values,
		ArgumentAndValues = PointViewKind.ArgumentAndValues,
		SeriesName = PointViewKind.SeriesName,
		Undefined = PointViewKind.Undefined
	}
	public sealed class PointOptions : ChartDependencyObject, IWeakEventListener {
		public static readonly DependencyProperty PatternProperty = DependencyPropertyManager.Register("Pattern",
			typeof(string), typeof(PointOptions), new PropertyMetadata(String.Empty, NotifyPropertyChanged));
		public static readonly DependencyProperty PointViewProperty = DependencyPropertyManager.Register("PointView",
			typeof(PointView), typeof(PointOptions), new PropertyMetadata(PointView.Undefined, NotifyPropertyChanged));
		public static readonly DependencyProperty ArgumentNumericOptionsProperty = DependencyPropertyManager.Register("ArgumentNumericOptions",
			typeof(NumericOptions), typeof(PointOptions), new PropertyMetadata(NumericOptionsPropertyChanged));
		public static readonly DependencyProperty ArgumentDateTimeOptionsProperty = DependencyPropertyManager.Register("ArgumentDateTimeOptions",
			typeof(DateTimeOptions), typeof(PointOptions), new PropertyMetadata(DateTimeOptionsPropertyChanged));
		public static readonly DependencyProperty ValueNumericOptionsProperty = DependencyPropertyManager.Register("ValueNumericOptions",
			typeof(NumericOptions), typeof(PointOptions), new PropertyMetadata(NumericOptionsPropertyChanged));
		public static readonly DependencyProperty ValueDateTimeOptionsProperty = DependencyPropertyManager.Register("ValueDateTimeOptions",
			typeof(DateTimeOptions), typeof(PointOptions), new PropertyMetadata(DateTimeOptionsPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointOptionsPattern"),
#endif
		Category(Categories.Common)
		]
		public string Pattern {
			get { return (string)GetValue(PatternProperty); }
			set { SetValue(PatternProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointOptionsPointView"),
#endif
		Category(Categories.Common)
		]
		public PointView PointView {
			get { return (PointView)GetValue(PointViewProperty); }
			set { SetValue(PointViewProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointOptionsArgumentNumericOptions"),
#endif
		Category(Categories.Common)
		]
		public NumericOptions ArgumentNumericOptions {
			get { return (NumericOptions)GetValue(ArgumentNumericOptionsProperty); }
			set { SetValue(ArgumentNumericOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointOptionsValueNumericOptions"),
#endif
		Category(Categories.Common)
		]
		public NumericOptions ValueNumericOptions {
			get { return (NumericOptions)GetValue(ValueNumericOptionsProperty); }
			set { SetValue(ValueNumericOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointOptionsArgumentDateTimeOptions"),
#endif
		Category(Categories.Common)
		]
		public DateTimeOptions ArgumentDateTimeOptions {
			get { return (DateTimeOptions)GetValue(ArgumentDateTimeOptionsProperty); }
			set { SetValue(ArgumentDateTimeOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("PointOptionsValueDateTimeOptions"),
#endif
		Category(Categories.Common),
		]
		public DateTimeOptions ValueDateTimeOptions {
			get { return (DateTimeOptions)GetValue(ValueDateTimeOptionsProperty); }
			set { SetValue(ValueDateTimeOptionsProperty, value); }
		}
		static void DateTimeOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PointOptions pointOptions = d as PointOptions;
			if (pointOptions != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as DateTimeOptions, e.NewValue as DateTimeOptions, pointOptions);
				pointOptions.NotifyPropertyChanged(e.Property.GetName());
			}
		}
		static void NumericOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PointOptions pointOptions = d as PointOptions;
			if (pointOptions != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as NumericOptions, e.NewValue as NumericOptions, pointOptions);
				pointOptions.NotifyPropertyChanged(e.Property.GetName());
			}
		}
		internal static void PercentOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PointOptions pointOptions = d as PointOptions;
			if (pointOptions != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as PercentOptions, e.NewValue as PercentOptions, pointOptions);
				pointOptions.NotifyPropertyChanged(e.Property.GetName());
			}
		}
		internal static void ValueToDisplayPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NotifyPropertyChanged(d, e);
		}
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				NotifyPropertyChanged("PointOptions");
				return true;
			}
			return false;
		}
		#endregion
		protected override ChartDependencyObject CreateObject() {
			return new PointOptions();
		}
	}
}
namespace DevExpress.Xpf.Charts.Native {
	public abstract class PointOptionsContainerBase {
		readonly Series series;
		protected Series Series { get { return series; } }
		public abstract PointOptions PointOptions { get; }
		public string Pattern {
			get {
				if (!String.IsNullOrEmpty(PointOptions.Pattern))
					return PointOptions.Pattern;
				string pattern = PointOptionsHelper.ConvertToPattern((PointViewKind)PointOptions.PointView);
				if (!String.IsNullOrEmpty(pattern))
					return pattern;
				return PointOptionsHelper.DefaultPattern;
			}
		}
		public INumericOptions ArgumentNumericOptions {
			get { return PointOptions.ArgumentNumericOptions != null ? PointOptions.ArgumentNumericOptions : new NumericOptions(); }
		}
		public IDateTimeOptions ArgumentDateTimeOptions {
			get { return PointOptions.ArgumentDateTimeOptions != null ? PointOptions.ArgumentDateTimeOptions : new DateTimeOptions(); }
		}
		public INumericOptions ValueNumericOptions {
			get { return PointOptions.ValueNumericOptions != null ? PointOptions.ValueNumericOptions : new NumericOptions(); }
		}
		public IDateTimeOptions ValueDateTimeOptions {
			get { return PointOptions.ValueDateTimeOptions != null ? PointOptions.ValueDateTimeOptions : new DateTimeOptions(); }
		}
		public PointOptionsContainerBase(Series series) {
			this.series = series;
		}
		public PercentOptions GetPercentOptions(DependencyProperty property) {
			return (PercentOptions)PointOptions.GetValue(property);
		}
		internal string ConstructPatternFromPointOptions(Series series, ScaleType argumentScaleType, ScaleType valueScaleType) {
			string pattern = Pattern;
			if (pattern.Contains("{A}"))
				pattern = pattern.Replace("{A}", ConstructArgumentPattern(argumentScaleType));
			if (pattern.Contains("{V}")) {
				string valuePattern = series.ConstructValuePattern(this, valueScaleType);
				pattern = pattern.Replace("{V}", valuePattern);
			}
			return pattern;
		}
		internal string ConstructValuePatternFromPercentOptions(PercentOptions percentOptions, ScaleType valueScaleType) {
			if (percentOptions == null)
				percentOptions = new PercentOptions();
			string valuePattern = PatternUtils.ValuePlaceholder;
			if (percentOptions.ValueAsPercent) {
				valuePattern = PatternUtils.PercentValuePlaceholder + ":" + NumericOptionsHelper.GetFormatString(ValueNumericOptions);
				if (ValueNumericOptions.Format == NumericOptionsFormat.General)
					valuePattern += percentOptions.PercentageAccuracy.ToString();
				return "{" + valuePattern + "}";
			}
			return ConstructValuePattern(valueScaleType);
		}
		internal string ConstructValueFormat(ScaleType valueScaleType) {
			string valueFormat = string.Empty;
			switch (valueScaleType) {
				case ScaleType.Numerical:
					valueFormat += ":" + NumericOptionsHelper.GetFormatString(ValueNumericOptions);
					break;
				case ScaleType.DateTime:
					valueFormat += ":" + DateTimeOptionsHelper.GetFormatString(ValueDateTimeOptions);
					break;
			}
			return valueFormat;
		}
		internal string ConstructValuePattern(ScaleType valueScaleType) {
			string valueFormat = ConstructValueFormat(valueScaleType);
			return "{" + PatternUtils.ValuePlaceholder + valueFormat + "}";
		}
		internal string ConstructArgumentPattern(ScaleType argumentScaleType) {
			string argumentPattern = "{" + PatternUtils.ArgumentPlaceholder;
			switch (argumentScaleType) {
				case ScaleType.Numerical:
					argumentPattern += ":" + NumericOptionsHelper.GetFormatString(ArgumentNumericOptions);
					break;
				case ScaleType.DateTime:
					argumentPattern += ":" + DateTimeOptionsHelper.GetFormatString(ArgumentDateTimeOptions);
					break;
				case ScaleType.Qualitative:
					break;
			}
			return argumentPattern + "}";
		}
	}
	public class PointOptionsContainer : PointOptionsContainerBase {
		public override PointOptions PointOptions {
			get {
				if (Series != null && Series.PointOptionsInternal != null)
					return Series.PointOptionsInternal;
				return new PointOptions();
			}
		}
		public PointOptionsContainer(Series series)
			: base(series) {
		}
	}
	public class LegendPointOptionsContainer : PointOptionsContainerBase {
		public override PointOptions PointOptions {
			get {
				if (Series != null && Series.LegendPointOptionsInternal != null)
					return Series.LegendPointOptionsInternal;
				return new PointOptions();
			}
		}
		public LegendPointOptionsContainer(Series series)
			: base(series) {
		}
	}
}
