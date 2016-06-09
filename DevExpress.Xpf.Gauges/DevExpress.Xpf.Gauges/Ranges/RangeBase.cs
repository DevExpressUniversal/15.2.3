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
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public enum RangeValueType {
		Absolute,
		Percent
	}
	[TypeConverter(typeof(RangeValueConverter))]
	public struct RangeValue {
		double value;
		RangeValueType type;
		public double Value { get { return value; } }
		public bool IsAbsolute { get { return type == RangeValueType.Absolute; } }
		public bool IsPercent { get { return type == RangeValueType.Percent; } }
		public RangeValueType RangeValueType { get { return type; } }
		public RangeValue(double value)
			: this(value, RangeValueType.Absolute) {
		}
		public RangeValue(double value, RangeValueType type) {
			this.value = value;
			this.type = type;
		}
	}
	public class RangeValueConverter : TypeConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			string stringValue = value as string;
			if (stringValue != null)
				if (stringValue.EndsWith("%"))
					return new RangeValue(Convert.ToDouble(stringValue.Substring(0, stringValue.Length - 1), culture), RangeValueType.Percent);
				else
					return new RangeValue(Convert.ToDouble(stringValue, culture));
			return null;
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if (destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if (value is RangeValue) {
				RangeValue rangeValue = (RangeValue)value;
				if (destinationType == typeof(string)) {
					string stringValue = rangeValue.Value.ToString(culture);
					if (rangeValue.IsPercent)
						return stringValue + "%";
					else
						return stringValue;
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
	public class ObjectToRangeValueConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (targetType == typeof(RangeValue)) {
				string stringValue = value as string;
				if (stringValue != null)
					if (stringValue.EndsWith("%"))
						return new RangeValue(System.Convert.ToDouble(stringValue.Substring(0, stringValue.Length - 1), culture), RangeValueType.Percent);
					else
						return new RangeValue(System.Convert.ToDouble(stringValue, culture));
				else
					return new RangeValue(System.Convert.ToDouble(value, culture));
			}
			else
				return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value is RangeValue)
				if (targetType == typeof(string)) {
					string stringValue = ((RangeValue)value).Value.ToString();
					return ((RangeValue)value).IsPercent ? stringValue + "%" : stringValue;
				}
				else
					return ((RangeValue)value).Value;
			else
				return null;
		}
	}
	public class RangeOptions : GaugeDependencyObject {
		public static readonly DependencyProperty OffsetProperty = DependencyPropertyManager.Register("Offset",
			typeof(double), typeof(RangeOptions), new PropertyMetadata(-19.0, NotifyPropertyChanged));
		public static readonly DependencyProperty ThicknessProperty = DependencyPropertyManager.Register("Thickness",
			typeof(int), typeof(RangeOptions), new PropertyMetadata(5, NotifyPropertyChanged), ThicknessValidation);
		public static readonly DependencyProperty ZIndexProperty = DependencyPropertyManager.Register("ZIndex",
		   typeof(int), typeof(RangeOptions), new PropertyMetadata(-10, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("RangeOptionsOffset"),
#endif
		Category(Categories.Layout)
		]
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("RangeOptionsThickness"),
#endif
		Category(Categories.Layout)
		]
		public int Thickness {
			get { return (int)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("RangeOptionsZIndex"),
#endif
		Category(Categories.Layout)
		]
		public int ZIndex {
			get { return (int)GetValue(ZIndexProperty); }
			set { SetValue(ZIndexProperty, value); }
		}
		static bool ThicknessValidation(object value) {
			return (int)value > 0;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new RangeOptions();
		}
	}
	public abstract class RangeBase : LayerBase {
		public static readonly DependencyProperty OptionsProperty = DependencyPropertyManager.Register("Options",
			typeof(RangeOptions), typeof(RangeBase), new PropertyMetadata(OptionsPropertyChanged));
		[TypeConverter(typeof(RangeValueConverter))]
		public static readonly DependencyProperty StartValueProperty = DependencyPropertyManager.Register("StartValue",
			typeof(RangeValue), typeof(RangeBase), new PropertyMetadata(new RangeValue(0.0, RangeValueType.Percent), RangePropertyChanged));
		[TypeConverter(typeof(RangeValueConverter))]
		public static readonly DependencyProperty EndValueProperty = DependencyPropertyManager.Register("EndValue",
			typeof(RangeValue), typeof(RangeBase), new PropertyMetadata(new RangeValue(25.0, RangeValueType.Percent), RangePropertyChanged));
		public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyPropertyManager.Register("IsHitTestVisible",
			typeof(bool), typeof(RangeBase), new PropertyMetadata(true));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("RangeBaseOptions"),
#endif
		Category(Categories.Presentation)
		]
		public RangeOptions Options {
			get { return (RangeOptions)GetValue(OptionsProperty); }
			set { SetValue(OptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("RangeBaseStartValue"),
#endif
		Category(Categories.Data),
		TypeConverter(typeof(RangeValueConverter))
		]
		public RangeValue StartValue {
			get { return (RangeValue)GetValue(StartValueProperty); }
			set { SetValue(StartValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("RangeBaseEndValue"),
#endif
		Category(Categories.Data),
		TypeConverter(typeof(RangeValueConverter))
		]
		public RangeValue EndValue {
			get { return (RangeValue)GetValue(EndValueProperty); }
			set { SetValue(EndValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("RangeBaseIsHitTestVisible"),
#endif
		Category(Categories.Behavior)
		]
		public bool IsHitTestVisible {
			get { return (bool)GetValue(IsHitTestVisibleProperty); }
			set { SetValue(IsHitTestVisibleProperty, value); }
		}
		static void RangePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RangeBase range = d as RangeBase;
			if (range != null) {
				range.Invalidate();
				bool startValueChanged = e.Property == StartValueProperty;
				RangeValue oldRangeValue = (RangeValue)e.OldValue;
				double oldValue = oldRangeValue.IsPercent ? range.PercentToAbsolute(oldRangeValue.Value) : oldRangeValue.Value;
				range.CheckRangeEnterLeaveIndicator(oldValue, startValueChanged);
			}
		}
		readonly RangeInfo info;
		public event IndicatorEnterEventHandler IndicatorEnter;
		public event IndicatorLeaveEventHandler IndicatorLeave;
		internal Scale Scale { get { return Owner as Scale; } }
		internal override LayerInfo ElementInfo { get { return info; } }
		protected internal abstract RangeOptions ActualOptions { get; }
		protected override int ActualZIndex { get { return ActualOptions.ZIndex; } }
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("RangeBaseStartValueAbsolute")]
#endif
		public double StartValueAbsolute { get { return StartValue.IsPercent ? PercentToAbsolute(StartValue.Value) : StartValue.Value; } }
#if !SL
	[DevExpressXpfGaugesLocalizedDescription("RangeBaseEndValueAbsolute")]
#endif
		public double EndValueAbsolute { get { return EndValue.IsPercent ? PercentToAbsolute(EndValue.Value) : EndValue.Value; } }
		public RangeBase() {
			info = new RangeInfo(this, ActualZIndex, ActualPresentation.CreateLayerPresentationControl(), ActualPresentation);
		}
		bool IsInRange(double value) {
			return MathUtils.IsValueInRange(value, StartValueAbsolute, EndValueAbsolute);
		}
		bool IsEnter(double value, double oldRangeValue, bool startValueChanged) {
			double secondRangeLimit = startValueChanged ? EndValueAbsolute : StartValueAbsolute;
			return !MathUtils.IsValueInRange(value, secondRangeLimit, oldRangeValue) && IsInRange(value);
		}
		bool IsLeave(double value, double oldRangeValue, bool startValueChanged) {
			double secondRangeLimit = startValueChanged ? EndValueAbsolute : StartValueAbsolute;
			return MathUtils.IsValueInRange(value, secondRangeLimit, oldRangeValue) && !IsInRange(value);
		}
		double PercentToAbsolute(double percentValue) {
			return Scale != null ? Scale.StartValue + (Scale.EndValue - Scale.StartValue) * percentValue / 100.0 : percentValue;
		}
		void CheckRangeEnterLeaveIndicator(double oldRangeValue, bool startValueChanged) {
			if (Scale != null)
				foreach (ValueIndicatorBase indicator in Scale.Indicators) {
					if (IsEnter(indicator.Value, oldRangeValue, startValueChanged) && IndicatorEnter != null)
						IndicatorEnter(this, new IndicatorEnterEventArgs(indicator, indicator.Value, indicator.Value));
					if (IsLeave(indicator.Value, oldRangeValue, startValueChanged) && IndicatorLeave != null)
						IndicatorLeave(this, new IndicatorLeaveEventArgs(indicator, indicator.Value, indicator.Value));
				}
		}
		internal void OnIndicatorEnterLeave(ValueIndicatorBase indicator, double oldValue, double newValue) {
			if (indicator != null) {
				if (!IsInRange(oldValue) && IsInRange(newValue) && IndicatorEnter != null)
					IndicatorEnter(this, new IndicatorEnterEventArgs(indicator, oldValue, newValue));
				if (IsInRange(oldValue) && !IsInRange(newValue) && IndicatorLeave != null)
					IndicatorLeave(this, new IndicatorLeaveEventArgs(indicator, oldValue, newValue));
			}
		}
	}
	public class RangeInfo : LayerInfo {
		readonly RangeBase range;
		protected internal override object HitTestableObject { get { return range; } }
		protected internal override object HitTestableParent { get { return range.Scale; } }
		protected internal override bool IsHitTestVisible { get { return range.IsHitTestVisible; } }
		internal RangeInfo(RangeBase range, int zIndex, PresentationControl presentationControl, PresentationBase presentation)
			: base(range, zIndex, presentationControl, presentation) {
			this.range = range;
		}
	}
}
