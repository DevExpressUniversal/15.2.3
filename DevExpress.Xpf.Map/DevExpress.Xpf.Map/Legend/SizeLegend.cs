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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public enum SizeLegendType {
		Inline,
		Nested
	}
	public class SizeLegend : ItemsLayerLegend {
		public static readonly DependencyProperty TypeProperty = DependencyPropertyManager.Register("Type",
		   typeof(SizeLegendType), typeof(SizeLegend), new PropertyMetadata(SizeLegendType.Inline, TypePropertyChanged));
		public static readonly DependencyProperty ShowTickMarksProperty = DependencyPropertyManager.Register("ShowTickMarks",
		   typeof(bool), typeof(SizeLegend), new PropertyMetadata(true, ShowTickMarksPropertyChanged));
		public static readonly DependencyProperty ItemFillProperty = DependencyPropertyManager.Register("ItemFill",
			typeof(Brush), typeof(SizeLegend), new PropertyMetadata(null));
		public static readonly DependencyProperty ItemStrokeProperty = DependencyPropertyManager.Register("ItemStroke",
			typeof(Brush), typeof(SizeLegend), new PropertyMetadata(null));
		public static readonly DependencyProperty CustomItemsProperty = DependencyPropertyManager.Register("CustomItems",
			typeof(SizeLegendItemCollection), typeof(SizeLegend), new PropertyMetadata(CustomItemsPropertyChanged));
		static void TypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SizeLegend legend = d as SizeLegend;
			if(legend != null && e.OldValue != e.NewValue) {
				legend.UpdateItems();
			}
		}
		static void ShowTickMarksPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SizeLegend legend = d as SizeLegend;
			if (legend != null && e.OldValue != e.NewValue) {
				legend.UpdateItems();
			}
		}
		[Category(Categories.Appearance)]
		public Brush ItemFill {
			get { return (Brush)GetValue(ItemFillProperty); }
			set { SetValue(ItemFillProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush ItemStroke {
			get { return (Brush)GetValue(ItemStrokeProperty); }
			set { SetValue(ItemStrokeProperty, value); }
		}
		[Category(Categories.Layout)]
		public SizeLegendType Type {
			get { return (SizeLegendType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}
		[Category(Categories.Layout)]
		public bool ShowTickMarks {
			get { return (bool)GetValue(ShowTickMarksProperty); }
			set { SetValue(ShowTickMarksProperty, value); }
		}
		[Category(Categories.Data)]
		public SizeLegendItemCollection CustomItems {
			get { return (SizeLegendItemCollection)GetValue(CustomItemsProperty); }
			set { SetValue(CustomItemsProperty, value); }
		}
		protected internal override bool ReverseItems { get { return Type != SizeLegendType.Inline; } }
		protected internal override IEnumerable<MapLegendItemBase> CustomItemsInternal { get { return CustomItems; } }
		public SizeLegend() {
			DefaultStyleKey = typeof(SizeLegend);
			this.SetValue(CustomItemsProperty, new SizeLegendItemCollection());
		}
	}
	public class NestedSizeLegendPanel : Grid {
		protected override Size ArrangeOverride(Size arrangeSize) {
			Size finalSize = base.ArrangeOverride(arrangeSize);
			SizeLegendLabelsVisibilityCalculator calculator = new SizeLegendLabelsVisibilityCalculator(Children, this);
			calculator.UpdateItemsVisibility();
			return finalSize;
		}
	}
	public class NestedItemPresentationControl : Control {
		FrameworkElement textBlock;
		public FrameworkElement TextBlock { get { return textBlock; } }
		public NestedItemPresentationControl() {
			DefaultStyleKey = typeof(NestedItemPresentationControl);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			textBlock = GetTemplateChild("PART_Text") as FrameworkElement;
		}
	}
	public class TickMarkVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if(targetType == typeof(Visibility))
				if(value is bool)
					return (bool)value ? Visibility.Visible : Visibility.Hidden;
			return Visibility.Visible;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class SizeLegendLabelsVisibilityCalculator {
		const int MinIndent = 1;
		const int HeightToPaddingRatio = 4;
		readonly UIElementCollection children;
		readonly UIElement root;
		List<TextContainerLayout> itemLayoutList;
		public SizeLegendLabelsVisibilityCalculator(UIElementCollection children, UIElement root) {
			this.children = children;
			this.root = root;
		}
		FrameworkElement FindTextContainer(FrameworkElement itemPresenter) {
			if (itemPresenter == null)
				return null;
			NestedItemPresentationControl container = LayoutHelper.FindElementByType<NestedItemPresentationControl>(itemPresenter);
			if (container != null)
				return container.TextBlock;
			return null;
		}
		bool IsOverlapped() {
			if (itemLayoutList.Count <= 2)
				return false;
			int count = itemLayoutList.Count;
			for (int indexFrom = 0; indexFrom < count - 1; indexFrom++) {
				for (int indexTo = indexFrom + 1; indexTo < count; indexTo++) {
					Rect fromRect = itemLayoutList[indexFrom].Bounds;
					Rect toRect = itemLayoutList[indexTo].Bounds;
					fromRect.Inflate(0.0, MinIndent);
					if (toRect.IntersectsWith(fromRect))
						return true;
				}
			}
			return false;
		}
		void PrepareItemsLayoutList() {
			itemLayoutList = new List<TextContainerLayout>();
			foreach (UIElement child in children) {
				FrameworkElement textContainer = FindTextContainer(child as FrameworkElement);
				if (textContainer == null)
					continue;
				SizeLegendItem legendItem = GetLegendItem(child as ContentPresenter);
				if (legendItem == null)
					continue;
				Rect bounds = LayoutHelper.GetRelativeElementRect(textContainer, root);
				double height = bounds.Height;
				bounds.Inflate(0.0, -height / HeightToPaddingRatio);
				itemLayoutList.Add(new TextContainerLayout() { Bounds = bounds, LegendItem = legendItem, Visible = true });
			}
		}
		SizeLegendItem GetLegendItem(ContentPresenter itemPresenter) {
			if (itemPresenter == null)
				return null;
			return itemPresenter.Content as SizeLegendItem;
		}
		void UpdateItemsLayoutList() {
			List<TextContainerLayout> visibleItems = new List<TextContainerLayout>();
			foreach (TextContainerLayout layout in itemLayoutList) {
				if (layout.Visible)
					visibleItems.Add(layout);
			}
			itemLayoutList = visibleItems;
		}
		void HideItems() {
			int count = itemLayoutList.Count;
			for (int itemIndex = 0; itemIndex < count; itemIndex++) {
				int leftIndex = itemIndex;
				int rightIndex = count - itemIndex - 1;
				TextContainerLayout leftItem = itemLayoutList[leftIndex];
				TextContainerLayout rightItem = itemLayoutList[rightIndex];
				bool visible = (leftIndex % 2) == 0;
				leftItem.Visible = visible;
				rightItem.Visible = visible;
				if ((rightIndex - leftIndex) == 1) {
					leftItem.Visible = true;
					rightItem.Visible = false;
					break;
				}
				if (rightIndex == leftIndex)
					break;
			}
		}
		public void UpdateItemsVisibility() {
			PrepareItemsLayoutList();
			while (IsOverlapped()) {
				HideItems();
				UpdateItemsLayoutList();
			}
		}
	}
	public class TextContainerLayout {
		public SizeLegendItem LegendItem { get; set; }
		public Rect Bounds { get; set; }
		public bool Visible {
			get { return LegendItem.ShowLabel; }
			set { LegendItem.ShowLabel = value; }
		}
	}
}
