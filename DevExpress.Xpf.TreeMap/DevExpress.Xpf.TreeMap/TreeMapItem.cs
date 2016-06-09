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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Markup;
using DevExpress.Xpf.TreeMap.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.TreeMap {
	[ContentProperty("Children")]
	public class TreeMapItem : TreeMapDependencyObject, INotifyPropertyChanged, ITreeMapItem {
		public static readonly DependencyProperty ActualTemplateProperty = DependencyProperty.Register("ActualTemplate",
			typeof(DataTemplate), typeof(TreeMapItem), new PropertyMetadata(null));
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTemplate ActualTemplate {
			get { return (DataTemplate)GetValue(ActualTemplateProperty); }
			set { SetValue(ActualTemplateProperty, value); }
		}
		double value;
		string label;
		object tag;
		bool isSelected;
		Brush background;
		Brush actualBackground;		
		TreeMapItemCollection children;
		Brush colorizerBrush;
		bool isHighlighted;
		Brush foreground;
		Brush actualForeground;
		Brush transparentBackground = new SolidColorBrush(Colors.Transparent);
		Brush whiteForeground;
		Brush blackForeground;
		Brush opacityMask;
		TreeMapControl TreeMap { get { return Owner as TreeMapControl; } }
		Brush WhiteForeground{ get {
			if (whiteForeground == null)
				whiteForeground = new SolidColorBrush(Colors.White);
			return whiteForeground;
			}
		}
		Brush BlackForeground { get {
			if (blackForeground == null)
				blackForeground = new SolidColorBrush(Colors.Black);
			return blackForeground;
			}
		}
		DataTemplate GroupHeaderContentTemplate { get { return HeaderContentTemplate != null ? HeaderContentTemplate : TreeMap != null ? TreeMap.GroupHeaderContentTemplate : null; } }
		DataTemplate LeafContentTemplate { get { return TreeMap != null ? TreeMap.LeafContentTemplate : null; } }
		protected internal DataTemplate HeaderContentTemplate { get; set; } 
		protected internal bool IsGroup { get { return Children.Count > 0; } }
		protected internal Brush ColorizerBrush {
			get { return colorizerBrush; }
			internal set {
				colorizerBrush = value;
				ApplyAppearance();
				NotifyPropertyChanged("ColorizerBrush");
			}
		} 
		public double Value {
			get { return value; }
			set {
				if (this.value != value) {
					this.value = value;
					if (TreeMap != null)
						TreeMap.InvalidateLayout();
					NotifyPropertyChanged("Value");
				}
			}
		}
		public string Label {
			get { return label; }
			set {
				if (label != value) {
					label = value;
					NotifyPropertyChanged("Label");
				}
			}
		}
		public object Tag {
			get { return tag; }
			set {
				if (tag != value) {
					tag = value;
					NotifyPropertyChanged("Tag");
				}
			}
		}
		public Brush Background {
			get { return background; }
			set {
				if (background != value) {
					background = value;
					ApplyAppearance();
					NotifyPropertyChanged("Brush");
				}
			}
		}
		public TreeMapItemCollection Children {
			get {
				if (children == null)
					children = new TreeMapItemCollection();
				return children;
			}
		}
		public bool IsSelected {
			get { return isSelected; }
			internal set {
				isSelected = value;
				ApplyOpacityMask();
				NotifyPropertyChanged("IsSelected");
			}
		}
		public Brush Foreground {
			get { return foreground; }
			set {
				foreground = value;
				NotifyPropertyChanged("Foreground");
			}
		}
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush ActualForeground {
			get { return actualForeground; }
			private set {
				actualForeground = value;
				NotifyPropertyChanged("ActualForeground");
			}
		}
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush OpacityMask {
			get { return opacityMask; }
			private set {
				if (opacityMask != value) {
					opacityMask = value;
					NotifyPropertyChanged("OpacityMask");
				}
			}
		}
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush ActualBackground {
			get { return actualBackground != null ? actualBackground : transparentBackground; }
			private set {
				actualBackground = value;
				NotifyPropertyChanged("ActualBackground");
			}
		}
		public bool IsHighlighted {
			get { return isHighlighted; }
			internal set {
				if (isHighlighted != value) {
					isHighlighted = value;
					ApplyOpacityMask();
					NotifyPropertyChanged("IsHighLighted");
				}
			}
		}
		#region ITreeMapLayoutItem
		Rect layout;
		Rect ITreeMapLayoutItem.Layout { get { return layout; } set { layout = value; } }
		double ITreeMapLayoutItem.Weight { get { return GetActualValue(); } }
		#endregion
		#region ITreeMapItem
		double ITreeMapItem.Value { get { return Value; } set { Value = value; } }
		string ITreeMapItem.Label { get { return Label; } set { Label = value; } }
		object ITreeMapItem.Source { get { return Tag != null ? Tag : this; } }
		bool ITreeMapItem.IsSelected { get { return IsSelected; } set { IsSelected = value; } }
		#endregion
		#region IOwnedElement implementation
		protected override void OwnerChanged() {
			((IOwnedElement)Children).Owner = Owner;
		}
		#endregion
		void ChooseForeground(Color backgroundColor) {
			double brightness = CommonUtils.GetBrightness(backgroundColor);
			if (brightness >= 0.7)
				ActualForeground = BlackForeground;
			else
				ActualForeground = WhiteForeground;
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new TreeMapItem();
		}
		protected internal double GetActualValue() {
			if (Value > 0 || !IsGroup)
				return Value;
			double resultValue = 0;
			foreach (TreeMapItem child in Children)
				resultValue += child.GetActualValue();
			return resultValue;
		}
		protected internal virtual void ApplyAppearance() {
			ActualBackground = Background != null ? Background : ColorizerBrush;
			if (Foreground != null)
				ActualForeground = Foreground;
			else if (ActualBackground is SolidColorBrush)
				ChooseForeground(((SolidColorBrush)ActualBackground).Color);
			else
				ActualForeground = whiteForeground;
		}
		protected internal virtual void ApplyOpacityMask() {
			OpacityMask = IsSelected ? SelectionAndHighlightingVisualHelper.GetSelectionBrush() :
						  IsHighlighted ? SelectionAndHighlightingVisualHelper.GetHighlightingBrush() :
						  null;
		}
		internal void UpdateTemplate() {
			ActualTemplate = IsGroup ? GroupHeaderContentTemplate : LeafContentTemplate;
		}
	}
	public class TreeMapItemPresentation : HeaderedItemsControl, IHitTestableElement {
		public static readonly DependencyProperty TreeMapItemProperty = DependencyProperty.Register("TreeMapItem",
	typeof(TreeMapItem), typeof(TreeMapItemPresentation), new PropertyMetadata(null));
		public TreeMapItem TreeMapItem {
			get { return (TreeMapItem)GetValue(TreeMapItemProperty); }
			set { SetValue(TreeMapItemProperty, value); }
		}
		internal ITreeMapLayoutItem LayoutItem { get { return TreeMapItem; } }
		static TreeMapItemPresentation() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeMapItemPresentation), new FrameworkPropertyMetadata(typeof(TreeMapItemPresentation)));
		}
		object IHitTestableElement.Element { get { return TreeMapItem; } }
	}
	public class ListToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value is IList && ((IList)value).Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	}
	public class BoolToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value is bool && (bool)value ? Visibility.Visible : Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	}
	public class ItemToVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			TreeMapItem treeMapItem = value as TreeMapItem;
			if (treeMapItem != null) {
				foreach (TreeMapItem item in treeMapItem.Children) {
					if (item.Children.Count > 0)
						return 1;
				}
			}
			return 0;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return null;
		}
	}
	public class TreeMapItemPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			double sumY = 0.0;
			foreach (UIElement child in Children) {
				if (child.Visibility == Visibility.Visible) {
					child.Measure(new Size(availableSize.Width, Math.Max(availableSize.Height - sumY, 0.0)));
					sumY += child.DesiredSize.Height;
				}
			}
			return new Size(availableSize.Width, sumY);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double y = 0.0;
			List<UIElement> visibleChildren = new List<UIElement>();
			foreach (UIElement child in Children)
				if (child.Visibility == Visibility.Visible)
					visibleChildren.Add(child);
			for (int i = 0; i < visibleChildren.Count - 1; i++) {
				UIElement child = visibleChildren[i];
				child.Arrange(new Rect(0.0, y, finalSize.Width, child.DesiredSize.Height));
				y += child.DesiredSize.Height;
			}
			if (visibleChildren.Count > 0)
				visibleChildren[visibleChildren.Count - 1].Arrange(new Rect(0.0, y, finalSize.Width, Math.Max(finalSize.Height - y, 0.0)));
			return finalSize;
		}
	}
	public class TreeMapTextPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			foreach (UIElement child in Children)
				child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
			return availableSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			foreach (UIElement child in Children) {
				child.Visibility = finalSize.Height < child.DesiredSize.Height ? Visibility.Hidden : Visibility.Visible;
				child.Arrange(new Rect(0, 0, child.DesiredSize.Width, child.DesiredSize.Height));
			}
			return finalSize;
		}
	}
}
namespace DevExpress.Xpf.TreeMap.Native {
	public static class SelectionAndHighlightingVisualHelper {
		static byte selectionAlpha = 130;
		static byte highlightingAlpha = 180;
		static Point endPoint = new Point(3, 3);
		static LinearGradientBrush selectionMask;
		static LinearGradientBrush highlightingMask;
		static LinearGradientBrush GetBrush(Byte alpha) {
			LinearGradientBrush brush = null;
			brush = new LinearGradientBrush() { SpreadMethod = GradientSpreadMethod.Repeat, EndPoint = endPoint, MappingMode = BrushMappingMode.Absolute };
			brush.GradientStops.Add(new GradientStop() { Color = Color.FromArgb(alpha, 0, 0, 0), Offset = 0.5 });
			brush.GradientStops.Add(new GradientStop() { Color = Colors.Black, Offset = 0.5 });
			return brush;
		}
		public static LinearGradientBrush GetSelectionBrush() {
			if (selectionMask == null)
				selectionMask = GetBrush(selectionAlpha);
			return selectionMask;
		}
		public static LinearGradientBrush GetHighlightingBrush() {
			if (highlightingMask == null)
				highlightingMask = GetBrush(highlightingAlpha);
			return highlightingMask;
		}
	}
}
