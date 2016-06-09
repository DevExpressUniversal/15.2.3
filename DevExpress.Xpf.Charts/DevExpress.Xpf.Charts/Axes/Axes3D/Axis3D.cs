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
using System.Windows.Shapes;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public abstract class Axis3D : Axis, IHitTestableElement, IFinishInvalidation {
		object IHitTestableElement.Element { get { return this; } }
		object IHitTestableElement.AdditionalElement { get { return null; } }
		protected internal override bool IsReversed { get { return false; } }
		protected internal override bool ShouldRotateTitle { get { return false; } }
		protected internal override AxisAlignment ActualAlignment { get { return AxisAlignment.Near; } }
		public Axis3D() : base() { }
		protected internal override GridLineGeometry CreateGridLineGeometry(Rect axisBounds, IAxisMapping mapping, double axisValue, int thickness) {
			return null;
		}
		protected internal override InterlaceGeometry CreateInterlaceGeometry(Rect axisBounds, IAxisMapping mapping, double nearAxisValue, double farAxisValue) {
			return null;
		}
		protected internal override void IsSelectedChanged() {
			ChartElementHelper.Update(this);
		}
	}
	public class AxisX3D : Axis3D {
		public static readonly DependencyProperty DateTimeScaleOptionsProperty = DependencyPropertyManager.Register("DateTimeScaleOptions",
			typeof(DateTimeScaleOptionsBase), typeof(AxisX3D), new PropertyMetadata(DateTimeScaleOptionsPropertyChanged));
		public static readonly DependencyProperty NumericScaleOptionsProperty = DependencyPropertyManager.Register("NumericScaleOptions",
			typeof(NumericScaleOptionsBase), typeof(AxisX3D), new PropertyMetadata(NumericScaleOptionsPropertyChanged));
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisX3DDateTimeScaleOptions"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public DateTimeScaleOptionsBase DateTimeScaleOptions {
			get { return (DateTimeScaleOptionsBase)GetValue(DateTimeScaleOptionsProperty); }
			set { SetValue(DateTimeScaleOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisX3DNumericScaleOptions"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public NumericScaleOptionsBase NumericScaleOptions {
			get { return (NumericScaleOptionsBase)GetValue(NumericScaleOptionsProperty); }
			set { SetValue(NumericScaleOptionsProperty, value); }
		}
		readonly ManualDateTimeScaleOptions defaultDateTimeScaleOptions = new ManualDateTimeScaleOptions();
		readonly ContinuousNumericScaleOptions defaultNumericScaleOptions = new ContinuousNumericScaleOptions();
		static AxisX3D() {
			Type ownerType = typeof(AxisX3D);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
		protected override DateTimeScaleOptionsBase DefaultDateTimeScaleOptions { get { return defaultDateTimeScaleOptions; } }
		protected override NumericScaleOptionsBase DefaultNumericScaleOptions { get { return defaultNumericScaleOptions; } }
		protected override int GridSpacingFactor { get { return 100; } }
		protected internal override bool IsValuesAxis { get { return false; } }
		protected internal override bool IsVertical { get { return false; } }
		public AxisX3D() : base() { }
		protected internal override IAxisMapping CreateMapping(Rect bounds) {
			return new AxisMappingEx(this, IsVertical ? bounds.Height : bounds.Width);
		}
	}
	public class AxisY3D : Axis3D {
		public static readonly DependencyProperty DateTimeScaleOptionsProperty = DependencyPropertyManager.Register("DateTimeScaleOptions",
			typeof(ContinuousDateTimeScaleOptions), typeof(AxisY3D), new PropertyMetadata(DateTimeScaleOptionsPropertyChanged));
		public static readonly DependencyProperty NumericScaleOptionsProperty = DependencyPropertyManager.Register("NumericScaleOptions",
			typeof(ContinuousNumericScaleOptions), typeof(AxisY3D), new PropertyMetadata(NumericScaleOptionsPropertyChanged));
		public static readonly DependencyProperty AlwaysShowZeroLevelProperty;
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public static bool GetAlwaysShowZeroLevel(ChartNonVisualElement range) {
			return (bool)range.GetValue(AlwaysShowZeroLevelProperty);
		}
		public static void SetAlwaysShowZeroLevel(ChartNonVisualElement range, bool value) {
			range.SetValue(AlwaysShowZeroLevelProperty, value);
		}
		[Obsolete]
		internal static void AlwaysShowZeroLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AxisRange axisRange = d as AxisRange;
			if (axisRange != null)
				AxisRange.AlwaysShowZeroLevelPropertyChanged(d, e);
			Range range = d as Range;
			if (range != null)
				DevExpress.Xpf.Charts.Range.AlwaysShowZeroLevelPropertyChanged(d, e);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisY3DDateTimeScaleOptions"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ContinuousDateTimeScaleOptions DateTimeScaleOptions {
			get { return (ContinuousDateTimeScaleOptions)GetValue(DateTimeScaleOptionsProperty); }
			set { SetValue(DateTimeScaleOptionsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("AxisY3DNumericScaleOptions"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ContinuousNumericScaleOptions NumericScaleOptions {
			get { return (ContinuousNumericScaleOptions)GetValue(NumericScaleOptionsProperty); }
			set { SetValue(NumericScaleOptionsProperty, value); }
		}
		readonly ContinuousDateTimeScaleOptions defaultDateTimeScaleOptions = new ContinuousDateTimeScaleOptions();
		readonly ContinuousNumericScaleOptions dafaultNumericScaleOptions = new ContinuousNumericScaleOptions();
		protected override DateTimeScaleOptionsBase DefaultDateTimeScaleOptions { get { return defaultDateTimeScaleOptions; } }
		protected override NumericScaleOptionsBase DefaultNumericScaleOptions { get { return dafaultNumericScaleOptions; } }
		protected override int GridSpacingFactor { get { return 70; } }
		protected internal override bool IsValuesAxis { get { return true; } }
		protected internal override bool IsVertical { get { return true; } }
		[Obsolete]
		static AxisY3D() {
			AlwaysShowZeroLevelProperty = DependencyPropertyManager.RegisterAttached("AlwaysShowZeroLevel", typeof(bool), typeof(AxisY3D), new PropertyMetadata(true, AlwaysShowZeroLevelPropertyChanged));
		}
		public AxisY3D() : base() {
			DefaultStyleKey = typeof(AxisY3D);
		}
		protected internal override bool GetActualAlwaysShowZeroLevel(ChartNonVisualElement range) {
			return GetAlwaysShowZeroLevel(range);
		}
		protected internal override IAxisMapping CreateMapping(Rect bounds) {
			return new AxisMappingEx(this, IsVertical ? bounds.Height : bounds.Width);
		}
	}
}
