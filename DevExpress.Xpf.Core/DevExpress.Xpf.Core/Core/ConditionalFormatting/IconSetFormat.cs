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

using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System;
using System.Collections.ObjectModel;
using DevExpress.Xpf.GridData;
using DevExpress.Data;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.Native;
using System.Linq;
using System.Collections;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Mvvm.POCO;
using System.Windows.Markup;
using DevExpress.Xpf.Bars;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Export.Xl;
namespace DevExpress.Xpf.Core.ConditionalFormatting {
	public class IconSetElement : Freezable {
		[XtraSerializableProperty]
		public ImageSource Icon {
			get { return (ImageSource)GetValue(IconProperty); }
			set { SetValue(IconProperty, value); }
		}
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(ImageSource), typeof(IconSetElement), new PropertyMetadata(null));
		[XtraSerializableProperty]
		public double Threshold {
			get { return (double)GetValue(ThresholdProperty); }
			set { SetValue(ThresholdProperty, value); }
		}
		public static readonly DependencyProperty ThresholdProperty = DependencyProperty.Register("Threshold", typeof(double), typeof(IconSetElement), new PropertyMetadata(0d));
		[XtraSerializableProperty]
		public ThresholdComparisonType ThresholdComparisonType {
			get { return (ThresholdComparisonType)GetValue(ThresholdComparisonTypeProperty); }
			set { SetValue(ThresholdComparisonTypeProperty, value); }
		}
		public static readonly DependencyProperty ThresholdComparisonTypeProperty = DependencyProperty.Register("ThresholdComparisonType", typeof(ThresholdComparisonType), typeof(IconSetElement), new PropertyMetadata(ThresholdComparisonType.GreaterOrEqual));
		protected override Freezable CreateInstanceCore() {
			return new IconSetElement();
		}
#if DEBUGTEST
		public override string ToString() {
			return string.Format("IconSetElement [Threshold = {0}, ThresholdComparisonType = {1}]", Threshold, ThresholdComparisonType);
		}
#endif
	}
	public enum ThresholdComparisonType {
		GreaterOrEqual,
		Greater,
	}
	public class IconSetElementCollection : FreezableCollection<IconSetElement> {
		class IconSetElementComparer : IEqualityComparer<IconSetElement>, IComparer<IconSetElement> {
			public static readonly IconSetElementComparer Instance = new IconSetElementComparer();
			IconSetElementComparer() { }
			int IComparer<IconSetElement>.Compare(IconSetElement x, IconSetElement y) {
				int result = Comparer<double>.Default.Compare(x.Threshold, y.Threshold);
				return result != 0 ? result : Comparer<ThresholdComparisonType>.Default.Compare(x.ThresholdComparisonType, y.ThresholdComparisonType);
			}
			bool IEqualityComparer<IconSetElement>.Equals(IconSetElement x, IconSetElement y) {
				return x.Threshold == y.Threshold && x.ThresholdComparisonType == y.ThresholdComparisonType;
			}
			int IEqualityComparer<IconSetElement>.GetHashCode(IconSetElement obj) {
				return obj.Threshold.GetHashCode() ^ obj.ThresholdComparisonType.GetHashCode();
			}
		}
		protected override Freezable CreateInstanceCore() {
			return new IconSetElementCollection();
		}
		internal IconSetElement[] GetSortedElementsCore(ConditionalFormattingValueType valueType) {
			return this
				.Where(x => valueType == ConditionalFormattingValueType.Number || (x.Threshold >= 0 && x.Threshold <= 100))
				.Distinct(IconSetElementComparer.Instance)
				.OrderByDescending(x => x, IconSetElementComparer.Instance)
				.ToArray();
		}
	}
	public enum ConditionalFormattingValueType {
		Percent,
		Number,
	}
	[ContentProperty("Elements")]
	public class IconSetFormat : IndicatorFormatBase, IXtraSupportDeserializeCollectionItem {
		[XtraSerializableProperty]
		public ConditionalFormattingValueType ElementThresholdType {
			get { return (ConditionalFormattingValueType)GetValue(ElementThresholdTypeProperty); }
			set { SetValue(ElementThresholdTypeProperty, value); }
		}
		public static readonly DependencyProperty ElementThresholdTypeProperty =
			DependencyProperty.Register("ElementThresholdType", typeof(ConditionalFormattingValueType), typeof(IconSetFormat), new PropertyMetadata(ConditionalFormattingValueType.Percent));
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public IconSetElementCollection Elements {
			get { return (IconSetElementCollection)GetValue(ElementsProperty); }
			set { SetValue(ElementsProperty, value); }
		}
		public static readonly DependencyProperty ElementsProperty = DependencyProperty.Register("Elements", typeof(IconSetElementCollection), typeof(IconSetFormat), new PropertyMetadata(null, null, OnCoerceFreezable));
		static object OnCoerceFreezable(DependencyObject d, object baseValue) {
			var locker = ((IconSetFormat)d).coerceElementsLocker;
			if(locker.IsLocked)
				return baseValue;
			return FormatConditionBaseInfo.OnCoerceFreezable(baseValue);
		}
		public static readonly DependencyProperty IconSetTypeProperty = DependencyProperty.Register("IconSetType", typeof(XlCondFmtIconSetType?), typeof(IconSetFormat), new PropertyMetadata(null));
		[XtraSerializableProperty]
		public XlCondFmtIconSetType? IconSetType {
			get { return (XlCondFmtIconSetType?)GetValue(IconSetTypeProperty); }
			set { SetValue(IconSetTypeProperty, value); }
		}
		[XtraSerializableProperty]
		public VerticalAlignment IconVerticalAlignment {
			get { return (VerticalAlignment)GetValue(IconVerticalAlignmentProperty); }
			set { SetValue(IconVerticalAlignmentProperty, value); }
		}
		public static readonly DependencyProperty IconVerticalAlignmentProperty =
			DependencyProperty.Register("IconVerticalAlignment", typeof(VerticalAlignment), typeof(IconSetFormat), new PropertyMetadata(VerticalAlignment.Center));
		IconSetElement[] sortedElements;
		Locker coerceElementsLocker = new Locker();
		public IconSetFormat() {
			coerceElementsLocker.DoLockedAction(() => Elements = new IconSetElementCollection());
		}
		protected override Freezable CreateInstanceCore() {
			return new IconSetFormat();
		}
		protected override void OnChanged() {
			base.OnChanged();
			sortedElements = null;
		}
		internal IconSetElement[] GetSortedElements() {
			return sortedElements ?? (sortedElements = Elements.GetSortedElementsCore(ElementThresholdType));
		}
		internal IconSetElement GetElement(FormatValueProvider provider, decimal? minValue, decimal? maxValue) {
			decimal? nullableValue = GetDecimalValue(provider.Value);
			if(nullableValue == null)
				return null;
			decimal value = nullableValue.Value;
			if(ElementThresholdType == ConditionalFormattingValueType.Percent) {
				decimal? min = GetSummaryValue(provider, ConditionalFormatSummaryType.Min, minValue);
				decimal? max = GetSummaryValue(provider, ConditionalFormatSummaryType.Max, maxValue);
				if(min == null || max == null)
					return null;
				value = ColorScaleFormat.GetRatio(min.Value, max.Value, GetNormalizedValue(nullableValue.Value, min.Value, max.Value)) * 100;
			}
			var elements = GetSortedElements();
			for(int i = elements.Length - 1;
			i >= 0;
			i--) {
				if(ValueFit(elements[i], value) && (i == 0 || !ValueFit(elements[i - 1], value)))
					return elements[i];
			}
			return null;
		}
		static bool ValueFit(IconSetElement element, decimal value) {
			decimal threshold = element.Threshold.AsDecimal();
			return element.ThresholdComparisonType == ThresholdComparisonType.GreaterOrEqual ? value >= threshold : value > threshold;
		}
		public override DataBarFormatInfo CoerceDataBarFormatInfo(DataBarFormatInfo value, FormatValueProvider provider, decimal? minValue, decimal? maxValue) {
			return DataBarFormatInfo.AddIcon(value, GetElement(provider, minValue, maxValue).With(x => x.Icon), IconVerticalAlignment);
		}
		#region IXtraSupportDeserializeCollectionItem
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == ElementsProperty.Name) {
				IconSetElement element = new IconSetElement();
				Elements.Add(element);
				return element;
			}
			return null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) { }
		#endregion
	}
}
