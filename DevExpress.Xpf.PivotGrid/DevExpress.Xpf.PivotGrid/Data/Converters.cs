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
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.Xpf.Core;
#if !SL
using CoreGridLengthConverter = System.Windows.GridLengthConverter;
using System.Windows.Markup;
using System.Windows.Media;
#else
using CoreGridLengthConverter = DevExpress.Xpf.Core.GridLengthConverter;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using System.Windows.Markup;
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class MinWidthToMaxWidthConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(object.Equals(value, 0d))
				return DependencyProperty.UnsetValue;
			return value;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class PivotGridFieldDataConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(value is PivotGridField || value is PivotGridGroup)
				return new PivotGridFieldGroupData(value);
			if(value as FieldHeader != null)
				return new PivotGridFieldGroupData((FieldHeader)value);
			return null;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PivotGridFieldDataListConverter : IValueConverter {
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			IList<PivotGridField> list = value as IList<PivotGridField>;
			return list != null ? SimpleBridgeReadonlyObservableCollection<PivotGridFieldGroupData, PivotGridField>.Create(list, field => new PivotGridFieldGroupData(field)) : null;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class RowTreeToFieldHeadersMeasureModeConverter : IValueConverter {
		#region IValueConverter Members
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value is FieldRowTotalsLocation && ((FieldRowTotalsLocation)value) == FieldRowTotalsLocation.Tree ? FieldHeadersMeasureMode.Default : FieldHeadersMeasureMode.None;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
	}
	public class FieldListAllowedLayoutsToBoolConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(!(value is FieldListAllowedLayouts))
				return false;
			FieldListLayout layout;
			if(parameter == null || !Enum.TryParse<FieldListLayout>(parameter.ToString(), out layout))
				return false;
			return CustomizationFormEnumExtensions.IsLayoutAllowed((FieldListAllowedLayouts)value, layout);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class FieldListActualAreaToAreaTextConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			FieldListActualArea area = FieldListActualArea.AllFields;
			if(value is FieldListActualArea)
				area = (FieldListActualArea)value;
			if(area == FieldListActualArea.AllFields || area == FieldListActualArea.HiddenFields)
				return PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormListBoxText);
			else
				return PivotGridLocalizer.GetHeadersAreaText((int)area);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class FieldListActualAreaToAreaImageConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			FieldListActualArea area = FieldListActualArea.AllFields;
			if(value is FieldListActualArea)
				area = (FieldListActualArea)value;
			return ImageHelper.GetImage(area);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class FieldListActualAreaToAreaLabelConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			FieldListActualArea area = FieldListActualArea.AllFields;
			if(value is FieldListActualArea)
				area = (FieldListActualArea)value;
			switch(area) {
				case FieldListActualArea.AllFields:
					return PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormAvailableFields);
				case FieldListActualArea.HiddenFields:
					return PivotGridLocalizer.GetString(PivotGridStringId.CustomizationFormHiddenFields);
				case FieldListActualArea.FilterArea:
					return PivotGridLocalizer.GetString(PivotGridStringId.FilterArea);
				case FieldListActualArea.ColumnArea:
					return PivotGridLocalizer.GetString(PivotGridStringId.ColumnArea);
				case FieldListActualArea.RowArea:
					return PivotGridLocalizer.GetString(PivotGridStringId.RowArea);
				case FieldListActualArea.DataArea:
					return PivotGridLocalizer.GetString(PivotGridStringId.DataArea);
				default:
					throw new ArgumentException("FieldListActualArea");
			}
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class SimpleWidthDecreaser : Control {
		public static readonly DependencyProperty ElementProperty;
		public static readonly DependencyProperty MainElementProperty;
		public static readonly DependencyProperty DesiredWidthProperty;
		public static readonly DependencyProperty MainElementWidthProperty;
		static SimpleWidthDecreaser() {
			Type ownerType = typeof(SimpleWidthDecreaser);
			ElementProperty = DependencyPropertyManager.Register("Element", typeof(FrameworkElement), ownerType, new PropertyMetadata((d, e) => ((SimpleWidthDecreaser)d).OnElementPropertyChanged(e)));
			MainElementProperty = DependencyPropertyManager.Register("MainElement", typeof(FrameworkElement), ownerType, new PropertyMetadata((d, e) => ((SimpleWidthDecreaser)d).OnMainElementPropertyChanged(e)));
			DesiredWidthProperty = DependencyPropertyManager.Register("DesiredWidth", typeof(double), ownerType, new PropertyMetadata(Double.PositiveInfinity, (d, e) => ((SimpleWidthDecreaser)d).OnDesiredWidthChanged()));
			MainElementWidthProperty = DependencyPropertyManager.Register("MainElementWidth", typeof(double), ownerType, new PropertyMetadata(0d));
		}
		public SimpleWidthDecreaser() { }
		public double DesiredWidth {
			get { return (double)GetValue(DesiredWidthProperty); }
			set { SetValue(DesiredWidthProperty, value); }
		}
		public FrameworkElement MainElement {
			get { return (FrameworkElement)GetValue(MainElementProperty); }
			set { SetValue(MainElementProperty, value); }
		}
		public double MainElementWidth {
			get { return (double)GetValue(MainElementWidthProperty); }
			set { SetValue(MainElementWidthProperty, value); }
		}
		public FrameworkElement Element {
			get { return (FrameworkElement)GetValue(ElementProperty); }
			set { SetValue(ElementProperty, value); }
		}
		void OnMainElementPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if(e.NewValue != null)
				MainElement.SizeChanged += OnMainElementSizeChanged;
			if(e.OldValue != null)
				((FrameworkElement)e.OldValue).SizeChanged -= OnMainElementSizeChanged;
			RecalculateDesiredWidth();
		}
		void OnElementPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if(e.NewValue != null) {
				Element.SizeChanged += OnMainElementSizeChanged;
#if !SL
				Element.IsVisibleChanged += OnElementIsVisibleChanged;
#endif
			}
			if(e.OldValue != null) {
				((FrameworkElement)e.OldValue).SizeChanged -= OnMainElementSizeChanged;
#if !SL
				((FrameworkElement)e.OldValue).IsVisibleChanged -= OnElementIsVisibleChanged;
#endif
			}
			RecalculateDesiredWidth();
		}
		void OnElementIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			RecalculateDesiredWidth();
		}
		void OnMainElementSizeChanged(object sender, SizeChangedEventArgs e) {
			RecalculateDesiredWidth();
		}
		void OnDesiredWidthChanged() { }
		void RecalculateDesiredWidth() {
			DesiredWidth = GetDesiredWidth();
		}
		double GetDesiredWidth() {
			if(MainElement == null)
				return Double.PositiveInfinity;
			MainElementWidth = MainElement.ActualWidth;
			if(Element == null)
				return Double.PositiveInfinity;
			return Math.Max(0, MainElement.ActualWidth - Element.DesiredSize.Width - ((Element.DesiredSize.Width > 0) ? 1 : 0));
		}
	}
	public class TreeViewFieldHeaderToFieldTreeViewContentConverter : IValueConverter {
		public TreeViewFieldHeaderToFieldTreeViewContentConverter() { }
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			TreeViewFieldHeader header = value as TreeViewFieldHeader;
			if(header == null)
				return header;
			FieldTreeViewContent content = new FieldTreeViewContent();
			BindingOperations.SetBinding(content, FieldTreeViewContent.DisplayTextProperty, new Binding("DisplayText") { Source = header });
			BindingOperations.SetBinding(content, FieldTreeViewContent.ImageSourceProperty, new Binding("ImageSource") { Source = header });
			BindingOperations.SetBinding(content, FieldTreeViewContent.ForegroundProperty, new Binding("Foreground") { Source = header });
			BindingOperations.SetBinding(content, FieldTreeViewContent.CheckBoxVisibilityProperty, new Binding("ShowCheckBox") { Source = header, Converter = new BoolToVisibilityConverter() });
			BindingOperations.SetBinding(content, FieldTreeViewContent.FieldProperty, new Binding("Field") { Source = header });
			BindingOperations.SetBinding(content, FieldTreeViewContent.IsCheckedProperty, new Binding("Checked") { Source = header, Mode = BindingMode.TwoWay });
			if(header.Field == null) {
				ICustomizationTreeItem node = header.Node;
				while(node != null && node.Field == null)
					node = node.FirstChild;
				if(node != null) {
					BindingOperations.SetBinding(content, FieldTreeViewContent.ContentTemplateProperty, new Binding("FieldHeaderTreeViewTemplate") { Source = ((PivotFieldItem)node.Field).Wrapper.PivotGrid });
					BindingOperations.SetBinding(content, FieldTreeViewContent.ContentTemplateSelectorProperty, new Binding("FieldHeaderTreeViewTemplateSelector") { Source = ((PivotFieldItem)node.Field).Wrapper.PivotGrid });
					BindingOperations.SetBinding(content, FieldTreeViewContent.ActualHeaderContentStyleProperty, new Binding("FieldHeaderContentStyle") { Source = ((PivotFieldItem)node.Field).Wrapper.PivotGrid });
				}
			} else {
				BindingOperations.SetBinding(content, FieldTreeViewContent.ContentTemplateProperty, new Binding("ActualHeaderTreeViewTemplate") { Source = header.Field });
				BindingOperations.SetBinding(content, FieldTreeViewContent.ContentTemplateSelectorProperty, new Binding("ActualHeaderTreeViewTemplateSelector") { Source = header.Field });
				BindingOperations.SetBinding(content, FieldTreeViewContent.ActualHeaderContentStyleProperty, new Binding("ActualHeaderContentStyle") { Source = header.Field });
			}
			return content;
		}
		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public abstract class MarkupConverterBase : MarkupExtension, IValueConverter {
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
		public abstract object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture);
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class FontSizeToFieldHeightConverter : MarkupConverterBase, IMultiValueConverter {
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return System.Convert.ToDouble(System.Convert.ToInt32((Math.Max(System.Convert.ToDouble(value), 12d) * 7 / 2 - 21)));
		}
		object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			bool largeFontPaddings = object.Equals("largeFontSize", parameter) || values.Length > 1 && values[1] is bool && (bool)values[1] == true;
			if(largeFontPaddings) {
				return System.Convert.ToDouble(values[2]);
			} else {
				return Convert(values[0], targetType, parameter, culture);
			}
		}
		object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class FontSizeToFieldWidthConverter : MarkupConverterBase {
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return System.Convert.ToDouble(System.Convert.ToInt32(Math.Max(System.Convert.ToDouble(value), 12d) * 25 / 3));
		}
	}
	public class FontSizeToRowTreeOffsetConverter : MarkupConverterBase {
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return System.Convert.ToDouble(System.Convert.ToInt32(Math.Max(System.Convert.ToDouble(value), 12d) * 35 / 12));
		}
	}
	public class FontSizeToFieldListLabelMinWidth : MarkupConverterBase {
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return System.Convert.ToDouble(System.Convert.ToInt32(Math.Max(System.Convert.ToDouble(value) * 8, 95d)));
		}
	}
	public class TreeFieldHeaderToDragHeaderForegroundConverter : MarkupConverterBase {
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			SolidColorBrush brush = new SolidColorBrush(Colors.Black);
			DependencyObject dp = value as DependencyObject;
			if(dp == null)
				return brush;
			Control header = dp as Control;
			Control tv = LayoutHelper.FindParentObject<TreeView>(dp);
			if(tv != null && !Compare(tv.Foreground, tv.Background) && (header == null || !Compare(header.Background, tv.Foreground)) && (header == null || !Compare(header.Foreground, tv.Foreground)))
				return tv.Foreground;
			if(header != null)
				return header.Foreground;
			return brush;
		}
		bool Compare(Brush a, Brush b) {
			SolidColorBrush ab = a as SolidColorBrush;
			SolidColorBrush bb = b as SolidColorBrush;
			if(ab == null || bb == null)
				if(ab == null && bb == null)
					return true;
				else
					return false;
			return Math.Abs(ab.Color.R - bb.Color.R) + Math.Abs(ab.Color.G - bb.Color.G) + Math.Abs(ab.Color.B - bb.Color.B) < 181;
		}
	}
	public class MaxWidthConverter : MarkupConverterBase {
		public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return Math.Max(1d, System.Convert.ToDouble(value) - 50);
		}
	}
}
