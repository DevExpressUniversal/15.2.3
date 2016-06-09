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
using System.Collections.Generic;
using System.Windows;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class BubbleSeries3D : MarkerSeries3D, IXYWSeriesView {
		public static readonly DependencyProperty WeightProperty = DependencyPropertyManager.RegisterAttached("Weight", 
			typeof(double), typeof(BubbleSeries3D), new PropertyMetadata(Double.NaN, SeriesPoint.Update));
		public static readonly DependencyProperty ValueToDisplayProperty = DependencyPropertyManager.RegisterAttached("ValueToDisplay", 
			typeof(BubbleLabelValueToDisplay), typeof(BubbleSeries3D), new PropertyMetadata(BubbleLabelValueToDisplay.Weight, PointOptions.ValueToDisplayPropertyChanged));
		public static readonly DependencyProperty MaxSizeProperty = DependencyPropertyManager.Register("MaxSize", 
			typeof(double), typeof(BubbleSeries3D), new PropertyMetadata(0.9, SizePropertyChanged), ValidateSize);
		public static readonly DependencyProperty MinSizeProperty = DependencyPropertyManager.Register("MinSize", 
			typeof(double), typeof(BubbleSeries3D), new PropertyMetadata(0.3, SizePropertyChanged), ValidateSize);
		public static readonly DependencyProperty WeightDataMemberProperty = DependencyPropertyManager.Register("WeightDataMember", 
			typeof(string), typeof(BubbleSeries3D), new PropertyMetadata(String.Empty, OnBindingChanged));
		static BubbleSeries3D() {
			Type ownerType = typeof(BubbleSeries3D);
			ArgumentDataMemberProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBindingChanged)));
			ValueDataMemberProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(new PropertyChangedCallback(OnBindingChanged)));
		}
		static bool ValidateSize(object size) {
			return (double)size > 0;
		}
		static void SizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ChartElementHelper.Update(d, e, ChartElementChange.ClearDiagramCache);
		}
		[
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public static double GetWeight(SeriesPoint point) {
			return (double)point.GetValue(WeightProperty);
		}
		public static void SetWeight(SeriesPoint point, double weight) {
			point.SetValue(WeightProperty, weight);
		}
		[Obsolete]
		public static BubbleLabelValueToDisplay GetValueToDisplay(PointOptions options) {
			return (BubbleLabelValueToDisplay)options.GetValue(ValueToDisplayProperty);
		}
		public static void SetValueToDisplay(PointOptions options, BubbleLabelValueToDisplay valueToDisplay) {
			options.SetValue(ValueToDisplayProperty, valueToDisplay);
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BubbleSeries3DMaxSize"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double MaxSize {
			get { return (double)GetValue(MaxSizeProperty); }
			set { SetValue(MaxSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BubbleSeries3DMinSize"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double MinSize {
			get { return (double)GetValue(MinSizeProperty); }
			set { SetValue(MinSizeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("BubbleSeries3DWeightDataMember"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty
		]
		public string WeightDataMember {
			get { return (string)GetValue(WeightDataMemberProperty); }
			set { SetValue(WeightDataMemberProperty, value); }
		}
		protected override double Size { get { return MaxSize; } }
		protected internal override double SeriesDepth { get { return MaxSize; } }
		protected internal override ToolTipPointDataToStringConverter ToolTipPointValuesConverter { get { return new ToolTipBubbleValueToStringConverter(this); } }
		protected override string DefaultLabelTextPattern { get { return "{" + PatternUtils.WeightPlaceholder + "}"; } }
		protected override Type PointInterfaceType {
			get { return typeof(IXYWPoint); }
		}
		public BubbleSeries3D() {
			DefaultStyleKey = typeof(BubbleSeries3D);
		}
		#region ISeriesView implementation
		Type ISeriesView.PointInterfaceType { get { return typeof(IXYWPoint); } }
		#endregion
		#region IXYWSeriesView implementation
		double IXYWSeriesView.GetSideMargins(double min, double max) {
			return MaxSize;
		}
		#endregion
		protected override Series CreateObjectForClone() {
			return new BubbleSeries3D();
		}
		protected override IList<string> GetValueDataMembers() {
			return new string[] { ValueDataMember, WeightDataMember };
		}
		protected override ISeriesPoint CreateSeriesPoint(object argument, double internalArgument, object[] values, double[] internalValues, object tag, object hint, object color) {
			SeriesPoint point = (SeriesPoint)base.CreateSeriesPoint(argument, internalArgument, values, internalValues, tag, hint, color);
			SetWeight(point, (double)values[1]);
			return point;
		}
		protected override void AssignForBinding(Series series) {
			base.AssignForBinding(series);
			BubbleSeries3D bubbleSeries3D = series as BubbleSeries3D;
			if (bubbleSeries3D != null) {
				CopyPropertyValueHelper.CopyPropertyValue(this, bubbleSeries3D, MaxSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, bubbleSeries3D, MinSizeProperty);
				CopyPropertyValueHelper.CopyPropertyValue(this, bubbleSeries3D, WeightDataMemberProperty);
			}
		}
		protected override bool IsCorrectValueLevel(ValueLevelInternal valueLevel) {
			return valueLevel == ValueLevelInternal.Value || valueLevel == ValueLevelInternal.Weight;
		}
		protected internal override double[] GetPointValues(SeriesPoint point) {
			return new double[] { point.NonAnimatedValue, GetWeight(point) };
		}
		protected internal override void SetPointValues(SeriesPoint seriesPoint, double[] values, DateTime[] dateTimeValues) {
			base.SetPointValues(seriesPoint, values, dateTimeValues);
			if (values != null && values.Length > 1)
				SetWeight(seriesPoint, values[1]);
		}
		protected internal override double[] GetAnimatedPointValues(SeriesPoint point) {
			return new double[] { point.Value, GetWeight(point) };
		}
		protected internal override SeriesData CreateSeriesData() {
			SeriesData = new BubbleSeries3DData(this);
			return SeriesData;
		}
		protected internal override string ConstructValuePattern(PointOptionsContainerBase pointOptionsContainer, ScaleType valueScaleType) {
			BubbleLabelValueToDisplay valueToDisplay = (BubbleLabelValueToDisplay)pointOptionsContainer.PointOptions.GetValue(ValueToDisplayProperty);
			string textPattern = pointOptionsContainer.ConstructValuePattern(valueScaleType);
			switch (valueToDisplay) {
				case BubbleLabelValueToDisplay.Value:
					break;
				case BubbleLabelValueToDisplay.Weight:
					textPattern = PatternUtils.ReplacePlaceholder(textPattern, PatternUtils.ValuePlaceholder, PatternUtils.WeightPlaceholder);
					break;
				case BubbleLabelValueToDisplay.ValueAndWeight:
					textPattern = PatternUtils.ReplacePlaceholder(textPattern, PatternUtils.ValuePlaceholder, PatternUtils.ValuePlaceholder, PatternUtils.WeightPlaceholder);
					break;
			}
			return textPattern;
		}
		protected internal override string[] GetAvailablePointPatternPlaceholders() {
			return ToolTipPatternUtils.BubbleViewPointPatterns;
		}
		protected override SeriesContainer CreateContainer() {
			return new XYWSeriesContainer(this);
		}
	}
}
