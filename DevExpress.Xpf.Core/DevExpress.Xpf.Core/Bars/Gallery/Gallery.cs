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
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System;
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Themes;
using DevExpress.Utils;
using System.Windows.Markup;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Collections;
using System.Windows.Data;
using System.Windows.Input;
using System.Linq;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Bars.Native;
using System.Diagnostics;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public class GalleryItemEventArgs : EventArgs {
		public GalleryItemEventArgs(GalleryItem item) { Item = item; ItemControl = null; }
		public GalleryItemEventArgs(GalleryItem item, GalleryItemControl itemControl) { Item = item; ItemControl = itemControl; }
		public GalleryItem Item { get; set; }
		public GalleryItemControl ItemControl { get; set; }
	}
	public delegate void GalleryItemEventHandler(object sender, GalleryItemEventArgs e);
	public enum GalleryCheckDrawMode { OnlyImage, ImageAndText }
	public enum GallerySizeMode { None, Vertical, Both }
	public enum GalleryItemCheckMode { None, Single, Multiple, SingleInGroup}
	[ContentProperty("Groups")]
	public class Gallery : Control {
		#region static
		private static readonly object itemClickEventHandler;
		private static readonly object itemCheckedEventHandler;
		private static readonly object itemUncheckedEventHandler;
		private static readonly object itemHoverEventHandler;
		private static readonly object itemLeaveEventHandler;
		private static readonly object itemEnterEventHandler;
		private static readonly object sizeModeChangedEventHandler;
		private static readonly object userMarginsChangedEventHandler;
		private static readonly object isGroupCaptionVisibleChangedEventHandler;
		private static readonly object customTextStyleChangedEventHandler;
		private static readonly object customTempateChangedEventHandler;
		private static readonly object verticalScrollBarVisibilityEventHandler;
		private static readonly object verticalScrollBarStyleChangedEventHandler;
		public static readonly DependencyProperty ItemAutoWidthProperty;
		public static readonly DependencyProperty ItemAutoHeightProperty;		
		public static readonly DependencyProperty ItemSizeProperty;
		public static readonly DependencyProperty ItemGlyphRegionSizeProperty;
		public static readonly DependencyProperty ItemCaptionTextStyleProperty;
		public static readonly DependencyProperty ItemCaptionTextStyleSelectorProperty;
		public static readonly DependencyProperty ItemDescriptionTextStyleProperty;
		public static readonly DependencyProperty ItemDescriptionTextStyleSelectorProperty;
		public static readonly DependencyProperty GroupCaptionTextStyleProperty;
		public static readonly DependencyProperty NormalFilterCaptionTextStyleProperty;
		public static readonly DependencyProperty SelectedFilterCaptionTextStyleProperty;		
		public static readonly DependencyProperty ItemCheckModeProperty;
		public static readonly DependencyProperty ColCountProperty;
		public static readonly DependencyProperty RowCountProperty;
		public static readonly DependencyProperty SizeModeProperty;
		public static readonly DependencyProperty AutoHideGalleryProperty;
		public static readonly DependencyProperty MinColCountProperty;
		public static readonly DependencyProperty HoverGlyphSizeProperty;
		public static readonly DependencyProperty FilterCaptionProperty;
		public static readonly DependencyProperty CheckDrawModeProperty;
		public static readonly DependencyProperty AllowHoverAnimationProperty;
		public static readonly DependencyProperty AllowToolTipsProperty;
		public static readonly DependencyProperty AllowFilterProperty;
		public static readonly DependencyProperty AllowHoverImagesProperty;
		public static readonly DependencyProperty HintTemplateProperty;		
		public static readonly DependencyProperty HintCaptionTemplateProperty;
		public static readonly DependencyProperty GroupCaptionTemplateProperty;
		public static readonly DependencyProperty ItemDescriptionTemplateProperty;
		public static readonly DependencyProperty ItemCaptionTemplateProperty;
		public static readonly DependencyProperty FilterCaptionTemplateProperty;
		public static readonly DependencyProperty ItemCaptionHorizontalAlignmentProperty;
		public static readonly DependencyProperty ItemDescriptionHorizontalAlignmentProperty;
		public static readonly DependencyProperty ItemContentHorizontalAlignmentProperty;
		public static readonly DependencyProperty ItemGlyphHorizontalAlignmentProperty;
		public static readonly DependencyProperty GroupCaptionHorizontalAlignmentProperty;
		public static readonly DependencyProperty ItemCaptionVerticalAlignmentProperty;
		public static readonly DependencyProperty ItemDescriptionVerticalAlignmentProperty;		
		public static readonly DependencyProperty ItemContentVerticalAlignmentProperty;				
		public static readonly DependencyProperty ItemGlyphVerticalAlignmentProperty;
		public static readonly DependencyProperty GroupCaptionVerticalAlignmentProperty;
		public static readonly DependencyProperty ItemDescriptionMarginProperty;
		public static readonly DependencyProperty ItemCaptionMarginProperty;
		public static readonly DependencyProperty ItemGlyphMarginProperty;
		public static readonly DependencyProperty ItemMarginProperty;
		public static readonly DependencyProperty GroupCaptionMarginProperty;
		public static readonly DependencyProperty GroupItemsMarginProperty;
		public static readonly DependencyProperty IsItemCaptionVisibleProperty;	
		public static readonly DependencyProperty IsItemDescriptionVisibleProperty;
		public static readonly DependencyProperty IsItemGlyphVisibleProperty;
		public static readonly DependencyProperty IsGroupCaptionVisibleProperty;
		public static readonly DependencyProperty IsItemContentVisibleProperty;
		protected static readonly DependencyPropertyKey IsItemContentVisiblePropertyKey;
		public static readonly DependencyProperty ItemGlyphLocationProperty;
		public static readonly DependencyProperty HoverAnimationDurationProperty;
		public static readonly DependencyProperty ItemGlyphBorderPaddingProperty;
		public static readonly DependencyProperty ToolTipTemplateProperty;
		public static readonly DependencyProperty ItemGlyphSizeProperty;
		public static readonly DependencyProperty ItemGlyphStretchProperty;
		public static readonly DependencyProperty ItemBorderTemplateProperty;
		public static readonly DependencyProperty ItemGlyphBorderTemplateProperty;
		public static readonly DependencyProperty ItemControlTemplateProperty;
		public static readonly DependencyProperty GroupCaptionControlTemplateProperty;
		public static readonly DependencyProperty FilterControlTemplateProperty;
		public static readonly DependencyProperty AllowSmoothScrollingProperty;
		public static readonly DependencyProperty VerticalScrollbarVisibilityProperty;
		public static readonly DependencyProperty VerticalScrollBarStyleProperty;
		public static readonly DependencyProperty GroupsSourceProperty;
		public static readonly DependencyProperty GroupStyleProperty;
		public static readonly DependencyProperty GroupTemplateProperty;
		public static readonly DependencyProperty GroupTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty GroupsAttachedBehaviorProperty;
		protected static readonly DependencyPropertyKey CheckedItemsPropertyKey;
		public static readonly DependencyProperty CheckedItemsProperty;
		public static readonly DependencyProperty FirstCheckedItemProperty;
		public static readonly DependencyProperty ItemClickCommandProperty;
		static Gallery() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Gallery), new FrameworkPropertyMetadata(typeof(Gallery)));
			ItemSizeProperty = DependencyPropertyManager.Register("ItemSize", typeof(Size), typeof(Gallery), new FrameworkPropertyMetadata(new Size(double.NaN, double.NaN)));
			ItemGlyphRegionSizeProperty = DependencyPropertyManager.Register("ItemGlyphRegionSize", typeof(Size), typeof(Gallery), new FrameworkPropertyMetadata(new Size(double.NaN, double.NaN)));
			ItemCaptionTextStyleProperty = DependencyProperty.Register("ItemCaptionTextStyle", typeof(Style), typeof(Gallery), new FrameworkPropertyMetadata(null, OnItemCaptionTextStylePropertyChanged));
			ItemCaptionTextStyleSelectorProperty = DependencyPropertyManager.Register("ItemCaptionTextStyleSelector", typeof(StatedStyleSelector), typeof(Gallery), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemCaptionTextStyleSelectorPropertyChanged)));
			ItemDescriptionTextStyleProperty = DependencyProperty.Register("ItemDescriptionTextStyle", typeof(Style), typeof(Gallery), new FrameworkPropertyMetadata(null, OnItemDescriptionTextStylePropertyChanged));
			ItemDescriptionTextStyleSelectorProperty = DependencyPropertyManager.Register("ItemDescriptionTextStyleSelector", typeof(StatedStyleSelector), typeof(Gallery), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemDescriptionTextStyleSelectorPropertyChanged)));
			GroupCaptionTextStyleProperty = DependencyProperty.Register("GroupCaptionTextStyle", typeof(Style), typeof(Gallery), new FrameworkPropertyMetadata(null, OnGroupCaptionTextStylePropertyChanged));
			NormalFilterCaptionTextStyleProperty = DependencyProperty.Register("NormalFilterCaptionTextStyle", typeof(Style), typeof(Gallery), new FrameworkPropertyMetadata(null, OnNormalFilterCaptionTextStylePropertyChanged));
			SelectedFilterCaptionTextStyleProperty = DependencyProperty.Register("SelectedFilterCaptionTextStyle", typeof(Style), typeof(Gallery), new FrameworkPropertyMetadata(null, OnSelectedFilterCaptionTextStylePropertyChanged));
			ItemAutoHeightProperty = DependencyPropertyManager.Register("ItemAutoHeight", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(false));
			AllowToolTipsProperty = DependencyPropertyManager.Register("AllowToolTips", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(true));
			AutoHideGalleryProperty = DependencyPropertyManager.Register("AutoHideGallery", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(true));
			ColCountProperty = DependencyPropertyManager.Register("ColCount", typeof(int), typeof(Gallery), new FrameworkPropertyMetadata(0));
			RowCountProperty = DependencyPropertyManager.Register("RowCount", typeof(int), typeof(Gallery), new FrameworkPropertyMetadata(0));
			ItemCaptionMarginProperty = DependencyPropertyManager.Register("ItemCaptionMargin", typeof(Thickness?), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnItemCaptionMarginPropertyChanged, CoerceMargin));
			ItemGlyphMarginProperty = DependencyPropertyManager.Register("ItemGlyphMargin", typeof(Thickness?), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnItemGlyphMarginPropertyChanged, CoerceMargin));
			ItemMarginProperty = DependencyPropertyManager.Register("ItemMargin", typeof(Thickness?), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnItemMarginPropertyChanged, CoerceMargin));
			ItemDescriptionMarginProperty = DependencyPropertyManager.Register("ItemDescriptionMargin", typeof(Thickness?), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnItemDescriptionMarginPropertyChanged, CoerceMargin));
			ItemGlyphLocationProperty = DependencyPropertyManager.Register("ItemGlyphLocation", typeof(Dock), typeof(Gallery), new FrameworkPropertyMetadata(new Dock()));
			ItemGlyphHorizontalAlignmentProperty = DependencyPropertyManager.Register("ItemGlyphHorizontalAlignment", typeof(HorizontalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));
			ItemGlyphVerticalAlignmentProperty = DependencyPropertyManager.Register("ItemGlyphVerticalAlignment", typeof(VerticalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(VerticalAlignment.Stretch));
			ItemCaptionTemplateProperty = DependencyPropertyManager.Register("ItemCaptionTemplate", typeof(DataTemplate), typeof(Gallery), new FrameworkPropertyMetadata(null));
			IsItemCaptionVisibleProperty = DependencyPropertyManager.Register("IsItemCaptionVisible", typeof(bool), typeof(Gallery),
				new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsItemCaptionVisiblePropertyChanged)));
			IsItemDescriptionVisibleProperty = DependencyPropertyManager.Register("IsItemDescriptionVisible", typeof(bool), typeof(Gallery),
				new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsItemDescriptionVisiblePropertyChanged)));
			IsGroupCaptionVisibleProperty = DependencyPropertyManager.Register("IsGroupCaptionVisible", typeof(DefaultBoolean), typeof(Gallery),
				new FrameworkPropertyMetadata(DefaultBoolean.Default, new PropertyChangedCallback(OnIsGroupCaptionVisiblePropertyChanged)));
			IsItemGlyphVisibleProperty = DependencyPropertyManager.Register("IsItemGlyphVisible", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(true));
			ItemDescriptionTemplateProperty = DependencyPropertyManager.Register("ItemDescriptionTemplate", typeof(DataTemplate), typeof(Gallery), new FrameworkPropertyMetadata(null));
			CheckDrawModeProperty = DependencyPropertyManager.Register("CheckDrawMode", typeof(GalleryCheckDrawMode), typeof(Gallery), new FrameworkPropertyMetadata(GalleryCheckDrawMode.ImageAndText));
			AllowHoverImagesProperty = DependencyPropertyManager.Register("AllowHoverImages", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(false));
			ItemCaptionHorizontalAlignmentProperty = DependencyPropertyManager.Register("ItemCaptionHorizontalAlignment", typeof(HorizontalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
			ItemDescriptionVerticalAlignmentProperty = DependencyPropertyManager.Register("ItemDescriptionVerticalAlignment", typeof(VerticalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(VerticalAlignment.Center));
			ItemCaptionVerticalAlignmentProperty = DependencyPropertyManager.Register("ItemCaptionVerticalAlignment", typeof(VerticalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(VerticalAlignment.Center));
			ItemDescriptionHorizontalAlignmentProperty = DependencyPropertyManager.Register("ItemDescriptionHorizontalAlignment", typeof(HorizontalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(HorizontalAlignment.Center));
			ItemContentVerticalAlignmentProperty = DependencyPropertyManager.Register("ItemContentVerticalAlignment", typeof(VerticalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(VerticalAlignment.Center));
			ItemContentHorizontalAlignmentProperty = DependencyPropertyManager.Register("ItemContentHorizontalAlignment", typeof(HorizontalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(HorizontalAlignment.Center));
			FilterCaptionProperty = DependencyPropertyManager.Register("FilterCaption", typeof(object), typeof(Gallery), new FrameworkPropertyMetadata(null));
			FilterCaptionTemplateProperty = DependencyPropertyManager.Register("FilterCaptionTemplate", typeof(DataTemplate), typeof(Gallery), new FrameworkPropertyMetadata(null));
			AllowFilterProperty = DependencyPropertyManager.Register("AllowFilter", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(true));
			GroupCaptionTemplateProperty = DependencyPropertyManager.Register("GroupCaptionTemplate", typeof(DataTemplate), typeof(Gallery), new FrameworkPropertyMetadata(null));
			GroupCaptionVerticalAlignmentProperty = DependencyPropertyManager.Register("GroupCaptionVerticalAlignment", typeof(VerticalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(VerticalAlignment.Center));
			GroupCaptionHorizontalAlignmentProperty = DependencyPropertyManager.Register("GroupCaptionHorizontalAlignment", typeof(HorizontalAlignment), typeof(Gallery), new FrameworkPropertyMetadata(HorizontalAlignment.Left));
			HoverGlyphSizeProperty = DependencyPropertyManager.Register("HoverGlyphSize", typeof(Size?), typeof(Gallery), new FrameworkPropertyMetadata(null));
			AllowHoverAnimationProperty = DependencyPropertyManager.Register("AllowHoverAnimation", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(true));
			GroupCaptionMarginProperty = DependencyPropertyManager.Register("GroupCaptionMargin", typeof(Thickness?), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnGroupCaptionMarginPropertyChanged, CoerceMargin));
			MinColCountProperty = DependencyPropertyManager.Register("MinColCount", typeof(int), typeof(Gallery), new FrameworkPropertyMetadata(0));
			SizeModeProperty = DependencyPropertyManager.Register("SizeMode", typeof(GallerySizeMode), typeof(Gallery),
				new FrameworkPropertyMetadata(GallerySizeMode.Both, OnSizeModePropertyChanged));
			HintTemplateProperty = DependencyPropertyManager.Register("HintTemplate", typeof(DataTemplate), typeof(Gallery), new FrameworkPropertyMetadata(null));
			ItemCheckModeProperty = DependencyPropertyManager.Register("ItemCheckMode", typeof(GalleryItemCheckMode), typeof(Gallery),
				new FrameworkPropertyMetadata(GalleryItemCheckMode.None, new PropertyChangedCallback(OnItemCheckModePropertyChanged)));
			HintCaptionTemplateProperty = DependencyPropertyManager.Register("HintCaptionTemplate", typeof(DataTemplate), typeof(Gallery), new FrameworkPropertyMetadata(null));
			IsItemContentVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsItemContentVisible", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(true));			
			IsItemContentVisibleProperty = IsItemContentVisiblePropertyKey.DependencyProperty;
			HoverAnimationDurationProperty = DependencyPropertyManager.Register("HoverAnimationDuration", typeof(int), typeof(Gallery), new FrameworkPropertyMetadata(100));
			GroupItemsMarginProperty = DependencyPropertyManager.Register("GroupItemsMargin", typeof(Thickness?), typeof(Gallery),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupItemsMarginPropertyChanged), CoerceMargin));
			ItemGlyphBorderPaddingProperty = DependencyPropertyManager.Register("ItemGlyphBorderPadding", typeof(Thickness?), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnItemGlyphBorderPaddingPropertyChanged, CoerceMargin));
			ToolTipTemplateProperty = DependencyPropertyManager.Register("ToolTipTemplate", typeof(ControlTemplate), typeof(Gallery), new FrameworkPropertyMetadata(null));
			ItemGlyphSizeProperty = DependencyPropertyManager.Register("ItemGlyphSize", typeof(Size), typeof(Gallery), new FrameworkPropertyMetadata(new Size(double.NaN, double.NaN)));
			ItemGlyphStretchProperty = DependencyPropertyManager.Register("ItemGlyphStretch", typeof(Stretch), typeof(Gallery), new FrameworkPropertyMetadata(Stretch.Uniform));
			ItemBorderTemplateProperty = DependencyPropertyManager.Register("ItemBorderTemplate", typeof(ControlTemplate), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnItemBorderTemplatePropertyChanged));
			ItemGlyphBorderTemplateProperty = DependencyPropertyManager.Register("ItemGlyphBorderTemplate", typeof(ControlTemplate), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnItemGlyphBorderTemplatePropertyChanged));
			ItemControlTemplateProperty = DependencyPropertyManager.Register("ItemControlTemplate", typeof(ControlTemplate), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnItemControlTemplatePropertyChanged));
			GroupCaptionControlTemplateProperty = DependencyPropertyManager.Register("GroupCaptionControlTemplate", typeof(ControlTemplate), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnGroupCaptionControlTemplateChanged));
			FilterControlTemplateProperty = DependencyPropertyManager.Register("FilterControlTemplate", typeof(ControlTemplate), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnFilterControlTemplateChanged));
			ItemAutoWidthProperty = DependencyPropertyManager.Register("ItemAutoWidth", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(false));
			AllowSmoothScrollingProperty = DependencyPropertyManager.Register("AllowSmoothScrolling", typeof(bool), typeof(Gallery), new FrameworkPropertyMetadata(false));			
			VerticalScrollbarVisibilityProperty = DependencyPropertyManager.Register("VerticalScrollbarVisibility", typeof(ScrollBarVisibility),
				typeof(Gallery), new FrameworkPropertyMetadata(ScrollBarVisibility.Auto, OnVerticalScrollBarVisibilityPropertyChanged));
			VerticalScrollBarStyleProperty = DependencyPropertyManager.Register("VerticalScrollBarStyle", typeof(Style), typeof(Gallery),
				new FrameworkPropertyMetadata(null, OnVerticalScrollBarStylePropertyChanged));
			CheckedItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("CheckedItems", typeof(ReadOnlyObservableCollection<GalleryItem>), typeof(Gallery), new FrameworkPropertyMetadata(null));
			CheckedItemsProperty = CheckedItemsPropertyKey.DependencyProperty;
			GroupsSourceProperty = DependencyProperty.Register("GroupsSource", typeof(IEnumerable), typeof(Gallery), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnGroupsSourcePropertyChanged)));
			GroupStyleProperty = DependencyProperty.Register("GroupStyle", typeof(Style), typeof(Gallery), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnGroupStylePropertyChanged)));
			GroupTemplateProperty = DependencyProperty.Register("GroupTemplate", typeof(DataTemplate), typeof(Gallery), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnGroupTemplatePropertyChanged)));
			GroupTemplateSelectorProperty = DependencyProperty.Register("GroupTemplateSelector", typeof(DataTemplateSelector), typeof(Gallery), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnGroupTemplateSelectorPropertyChanged)));
			GroupsAttachedBehaviorProperty = DependencyProperty.Register("GroupsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<Gallery, GalleryItemGroup>), typeof(Gallery), new System.Windows.PropertyMetadata(null));
			ItemClickCommandProperty = DependencyProperty.Register("ItemClickCommand", typeof(ICommand), typeof(Gallery), new System.Windows.PropertyMetadata(null));
			FirstCheckedItemProperty = DependencyPropertyManager.Register("FirstCheckedItem", typeof(object), typeof(Gallery),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFirstCheckedItemPropertyChanged)));
			itemClickEventHandler = new object();
			itemCheckedEventHandler = new object();
			itemUncheckedEventHandler = new object();
			itemHoverEventHandler = new object();
			itemLeaveEventHandler = new object();
			itemEnterEventHandler = new object();
			sizeModeChangedEventHandler = new object();
			userMarginsChangedEventHandler = new object();
			isGroupCaptionVisibleChangedEventHandler = new object();
			customTextStyleChangedEventHandler = new object();
			customTempateChangedEventHandler = new object();
			verticalScrollBarVisibilityEventHandler = new object();
			verticalScrollBarStyleChangedEventHandler = new object();
		}
		protected static object CoerceMargin(DependencyObject d, object value) {		   
			if(value == null) return value;		   
			Thickness? margin = value as Thickness?;			
			if(margin == null) return null;
			return new Thickness?(new Thickness(Math.Max(margin.Value.Left, 0), Math.Max(margin.Value.Top, 0), Math.Max(margin.Value.Right, 0), Math.Max(margin.Value.Bottom, 0)));
		}
		protected static void OnFirstCheckedItemPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnFirstCheckedItemChanged(e.OldValue);
		}
		protected static void OnItemBorderTemplatePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTemplateChanged();
		}
		protected static void OnItemGlyphBorderTemplatePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTemplateChanged();
		}
		protected static void OnItemControlTemplatePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTemplateChanged();
		}
		protected static void OnGroupCaptionControlTemplateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTemplateChanged();
		}
		protected static void OnFilterControlTemplateChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTemplateChanged();
		}
		protected static void OnItemCaptionTextStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTextStyleChanged();
		}
		protected static void OnItemCaptionTextStyleSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Gallery)d).OnItemCaptionTextStyleSelectorChanged((StatedStyleSelector)e.OldValue);
		}
		protected static void OnItemDescriptionTextStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTextStyleChanged();
		}
		protected static void OnItemDescriptionTextStyleSelectorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((Gallery)d).OnItemDescriptionTextStyleSelectorChanged((StatedStyleSelector)e.OldValue);
		}
		protected static void OnGroupCaptionTextStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTextStyleChanged();
		}
		protected static void OnNormalFilterCaptionTextStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTextStyleChanged();
		}
		protected static void OnSelectedFilterCaptionTextStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnCustomTextStyleChanged();
		}
		protected static void OnIsItemCaptionVisiblePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).UpdateActualContentVisibility();
		}
		protected static void OnIsItemDescriptionVisiblePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).UpdateActualContentVisibility();
		}
		protected static void OnSizeModePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnSizeModeChanged((GallerySizeMode)e.OldValue);
		}
		protected static void OnItemMarginPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnUserMarginsChanged();
		}
		protected static void OnItemCaptionMarginPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnUserMarginsChanged();
		}
		protected static void OnItemDescriptionMarginPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnUserMarginsChanged();
		}
		protected static void OnItemGlyphMarginPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnUserMarginsChanged();
		}
		protected static void OnIsGroupCaptionVisiblePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnIsGroupCaptionVisibleChanged();
		}
		protected static void OnGroupCaptionMarginPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnUserMarginsChanged();
		}
		protected static void OnGroupItemsMarginPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnUserMarginsChanged();
		}
		protected static void OnItemCheckModePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnItemCheckModeChanged((GalleryItemCheckMode)e.OldValue);
		}
		protected static void OnItemGlyphBorderPaddingPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnUserMarginsChanged();
		}
		protected static void OnVerticalScrollBarVisibilityPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnVerticalScrollBarVisibilityChanged();
		}
		protected static void OnVerticalScrollBarStylePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnVerticalScrollBarStyleChanged();
		}
		protected static void OnGroupTemplateSelectorPropertyChanged(DependencyObject o, System.Windows.DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnGroupTemplateChanged(e);
		}
		protected static void OnGroupTemplatePropertyChanged(DependencyObject o, System.Windows.DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnGroupTemplateChanged(e);
		}
		protected static void OnGroupStylePropertyChanged(DependencyObject o, System.Windows.DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnGroupTemplateChanged(e);
		}
		protected static void OnGroupsSourcePropertyChanged(DependencyObject o, System.Windows.DependencyPropertyChangedEventArgs e) {
			((Gallery)o).OnGroupsSourceChanged(e);
		}
		#endregion
		#region dep props
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryFirstCheckedItem")]
#endif
		public object FirstCheckedItem {
			get { return (object)GetValue(FirstCheckedItemProperty); }
			set { SetValue(FirstCheckedItemProperty, value); }
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryCheckedItems")]
#endif
		public ReadOnlyObservableCollection<GalleryItem> CheckedItems {
			get { return (ReadOnlyObservableCollection<GalleryItem>)GetValue(CheckedItemsProperty); }
			protected internal set { this.SetValue(CheckedItemsPropertyKey, value); }
		}		
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryVerticalScrollBarStyle")]
#endif
		public Style VerticalScrollBarStyle {
			get { return (Style)GetValue(VerticalScrollBarStyleProperty); }
			set { SetValue(VerticalScrollBarStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryVerticalScrollbarVisibility")]
#endif
		public ScrollBarVisibility VerticalScrollbarVisibility {
			get { return (ScrollBarVisibility)GetValue(VerticalScrollbarVisibilityProperty); }
			set { SetValue(VerticalScrollbarVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryAllowSmoothScrolling")]
#endif
		public bool AllowSmoothScrolling {
			get { return (bool)GetValue(AllowSmoothScrollingProperty); }
			set { SetValue(AllowSmoothScrollingProperty, value); }
		}
		public bool ItemAutoWidth {
			get { return (bool)GetValue(ItemAutoWidthProperty); }
			set { SetValue(ItemAutoWidthProperty, value); }
		}
		public bool ItemAutoHeight {
			get { return (bool)GetValue(ItemAutoHeightProperty); }
			set { SetValue(ItemAutoHeightProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemBorderTemplate")]
#endif
		public ControlTemplate ItemBorderTemplate {
			get { return (ControlTemplate)GetValue(ItemBorderTemplateProperty); }
			set { SetValue(ItemBorderTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyphBorderTemplate")]
#endif
		public ControlTemplate ItemGlyphBorderTemplate {
			get { return (ControlTemplate)GetValue(ItemGlyphBorderTemplateProperty); }
			set { SetValue(ItemGlyphBorderTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemControlTemplate")]
#endif
		public ControlTemplate ItemControlTemplate {
			get { return (ControlTemplate)GetValue(ItemControlTemplateProperty); }
			set { SetValue(ItemControlTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupCaptionControlTemplate")]
#endif
		public ControlTemplate GroupCaptionControlTemplate {
			get { return (ControlTemplate)GetValue(GroupCaptionControlTemplateProperty); }
			set { SetValue(GroupCaptionControlTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryFilterControlTemplate")]
#endif
		public ControlTemplate FilterControlTemplate {
			get { return (ControlTemplate)GetValue(FilterControlTemplateProperty); }
			set { SetValue(FilterControlTemplateProperty, value); }
		}
		[Obsolete("Use Gallery.ItemCaptionTextStyleSelector property instead")]
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemCaptionTextStyle")]
#endif
		public Style ItemCaptionTextStyle {
			get { return (Style)GetValue(ItemCaptionTextStyleProperty); }
			set { SetValue(ItemCaptionTextStyleProperty, value); }
		}
		public StatedStyleSelector ItemCaptionTextStyleSelector {
			get { return (StatedStyleSelector)GetValue(ItemCaptionTextStyleSelectorProperty); }
			set { SetValue(ItemCaptionTextStyleSelectorProperty, value); }
		}
		[Obsolete("Use Gallery.ItemDescriptionTextStyleSelector property instead")]
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemDescriptionTextStyle")]
#endif
		public Style ItemDescriptionTextStyle {
			get { return (Style)GetValue(ItemDescriptionTextStyleProperty); }
			set { SetValue(ItemDescriptionTextStyleProperty, value); }
		}
		public StatedStyleSelector ItemDescriptionTextStyleSelector {
			get { return (StatedStyleSelector)GetValue(ItemDescriptionTextStyleSelectorProperty); }
			set { SetValue(ItemDescriptionTextStyleSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupCaptionTextStyle")]
#endif
		public Style GroupCaptionTextStyle {
			get { return (Style)GetValue(GroupCaptionTextStyleProperty); }
			set { SetValue(GroupCaptionTextStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryNormalFilterCaptionTextStyle")]
#endif
		public Style NormalFilterCaptionTextStyle {
			get { return (Style)GetValue(NormalFilterCaptionTextStyleProperty); }
			set { SetValue(NormalFilterCaptionTextStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GallerySelectedFilterCaptionTextStyle")]
#endif
		public Style SelectedFilterCaptionTextStyle {
			get { return (Style)GetValue(SelectedFilterCaptionTextStyleProperty); }
			set { SetValue(SelectedFilterCaptionTextStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemSize")]
#endif
		public Size ItemSize {
			get { return (Size)GetValue(ItemSizeProperty); }
			set { SetValue(ItemSizeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyphRegionSize")]
#endif
		public Size ItemGlyphRegionSize {
			get { return (Size)GetValue(ItemGlyphRegionSizeProperty); }
			set { SetValue(ItemGlyphRegionSizeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemCheckMode")]
#endif
		public GalleryItemCheckMode ItemCheckMode {
			get { return (GalleryItemCheckMode)GetValue(ItemCheckModeProperty); }
			set { SetValue(ItemCheckModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryAllowToolTips")]
#endif
		public bool AllowToolTips {
			get { return (bool)GetValue(AllowToolTipsProperty); }
			set { SetValue(AllowToolTipsProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryHintTemplate")]
#endif
		public DataTemplate HintTemplate {
			get { return (DataTemplate)GetValue(HintTemplateProperty); }
			set { SetValue(HintTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryAutoHideGallery")]
#endif
		public bool AutoHideGallery {
			get { return (bool)GetValue(AutoHideGalleryProperty); }
			set { SetValue(AutoHideGalleryProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GallerySizeMode")]
#endif
		public GallerySizeMode SizeMode {
			get { return (GallerySizeMode)GetValue(SizeModeProperty); }
			set { SetValue(SizeModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryMinColCount")]
#endif
		public int MinColCount {
			get { return (int)GetValue(MinColCountProperty); }
			set { SetValue(MinColCountProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryAllowHoverAnimation")]
#endif
		public bool AllowHoverAnimation {
			get { return (bool)GetValue(AllowHoverAnimationProperty); }
			set { SetValue(AllowHoverAnimationProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryHoverGlyphSize")]
#endif
		public Size? HoverGlyphSize {
			get { return (Size?)GetValue(HoverGlyphSizeProperty); }
			set { SetValue(HoverGlyphSizeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupCaptionMargin")]
#endif
		public Thickness? GroupCaptionMargin {
			get { return (Thickness?)GetValue(GroupCaptionMarginProperty); }
			set { SetValue(GroupCaptionMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupCaptionVerticalAlignment")]
#endif
		public VerticalAlignment GroupCaptionVerticalAlignment {
			get { return (VerticalAlignment)GetValue(GroupCaptionVerticalAlignmentProperty); }
			set { SetValue(GroupCaptionVerticalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupCaptionHorizontalAlignment")]
#endif
		public HorizontalAlignment GroupCaptionHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(GroupCaptionHorizontalAlignmentProperty); }
			set { SetValue(GroupCaptionHorizontalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupCaptionTemplate")]
#endif
		public DataTemplate GroupCaptionTemplate {
			get { return (DataTemplate)GetValue(GroupCaptionTemplateProperty); }
			set { SetValue(GroupCaptionTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryIsGroupCaptionVisible")]
#endif
		public DefaultBoolean IsGroupCaptionVisible {
			get { return (DefaultBoolean)GetValue(IsGroupCaptionVisibleProperty); }
			set { SetValue(IsGroupCaptionVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryAllowFilter")]
#endif
		public bool AllowFilter {
			get { return (bool)GetValue(AllowFilterProperty); }
			set { SetValue(AllowFilterProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryFilterCaptionTemplate")]
#endif
		public DataTemplate FilterCaptionTemplate {
			get { return (DataTemplate)GetValue(FilterCaptionTemplateProperty); }
			set { SetValue(FilterCaptionTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryFilterCaption")]
#endif
		[TypeConverter(typeof(ObjectConverter))]
		public object FilterCaption {
			get { return (object)GetValue(FilterCaptionProperty); }
			set { SetValue(FilterCaptionProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemContentVerticalAlignment")]
#endif
		public VerticalAlignment ItemContentVerticalAlignment {
			get { return (VerticalAlignment)GetValue(ItemContentVerticalAlignmentProperty); }
			set { SetValue(ItemContentVerticalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemContentHorizontalAlignment")]
#endif
		public HorizontalAlignment ItemContentHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(ItemContentHorizontalAlignmentProperty); }
			set { SetValue(ItemContentHorizontalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemCaptionHorizontalAlignment")]
#endif
		public HorizontalAlignment ItemCaptionHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(ItemCaptionHorizontalAlignmentProperty); }
			set { SetValue(ItemCaptionHorizontalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemCaptionVerticalAlignment")]
#endif
		public VerticalAlignment ItemCaptionVerticalAlignment {
			get { return (VerticalAlignment)GetValue(ItemCaptionVerticalAlignmentProperty); }
			set { SetValue(ItemCaptionVerticalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemDescriptionHorizontalAlignment")]
#endif
		public HorizontalAlignment ItemDescriptionHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(ItemDescriptionHorizontalAlignmentProperty); }
			set { SetValue(ItemDescriptionHorizontalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemDescriptionVerticalAlignment")]
#endif
		public VerticalAlignment ItemDescriptionVerticalAlignment {
			get { return (VerticalAlignment)GetValue(ItemDescriptionVerticalAlignmentProperty); }
			set { SetValue(ItemDescriptionVerticalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryAllowHoverImages")]
#endif
		public bool AllowHoverImages {
			get { return (bool)GetValue(AllowHoverImagesProperty); }
			set { SetValue(AllowHoverImagesProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryCheckDrawMode")]
#endif
		public GalleryCheckDrawMode CheckDrawMode {
			get { return (GalleryCheckDrawMode)GetValue(CheckDrawModeProperty); }
			set { SetValue(CheckDrawModeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemDescriptionTemplate")]
#endif
		public DataTemplate ItemDescriptionTemplate {
			get { return (DataTemplate)GetValue(ItemDescriptionTemplateProperty); }
			set { SetValue(ItemDescriptionTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryIsItemCaptionVisible")]
#endif
		public bool IsItemCaptionVisible {
			get { return (bool)GetValue(IsItemCaptionVisibleProperty); }
			set { SetValue(IsItemCaptionVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryIsItemDescriptionVisible")]
#endif
		public bool IsItemDescriptionVisible {
			get { return (bool)GetValue(IsItemDescriptionVisibleProperty); }
			set { SetValue(IsItemDescriptionVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryIsItemGlyphVisible")]
#endif
		public bool IsItemGlyphVisible {
			get { return (bool)GetValue(IsItemGlyphVisibleProperty); }
			set { SetValue(IsItemGlyphVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemDescriptionMargin")]
#endif
		public Thickness? ItemDescriptionMargin {
			get { return (Thickness?)GetValue(ItemDescriptionMarginProperty); }
			set { SetValue(ItemDescriptionMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemCaptionTemplate")]
#endif
		public DataTemplate ItemCaptionTemplate {
			get { return (DataTemplate)GetValue(ItemCaptionTemplateProperty); }
			set { SetValue(ItemCaptionTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyphVerticalAlignment")]
#endif
		public VerticalAlignment ItemGlyphVerticalAlignment {
			get { return (VerticalAlignment)GetValue(ItemGlyphVerticalAlignmentProperty); }
			set { SetValue(ItemGlyphVerticalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyphHorizontalAlignment")]
#endif
		public HorizontalAlignment ItemGlyphHorizontalAlignment {
			get { return (HorizontalAlignment)GetValue(ItemGlyphHorizontalAlignmentProperty); }
			set { SetValue(ItemGlyphHorizontalAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyphLocation")]
#endif
		public Dock ItemGlyphLocation {
			get { return (Dock)GetValue(ItemGlyphLocationProperty); }
			set { SetValue(ItemGlyphLocationProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemCaptionMargin")]
#endif
		public Thickness? ItemCaptionMargin {
			get { return (Thickness?)GetValue(ItemCaptionMarginProperty); }
			set { SetValue(ItemCaptionMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyphMargin")]
#endif
		public Thickness? ItemGlyphMargin {
			get { return (Thickness?)GetValue(ItemGlyphMarginProperty); }
			set { SetValue(ItemGlyphMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemMargin")]
#endif
		public Thickness? ItemMargin {
			get { return (Thickness?)GetValue(ItemMarginProperty); }
			set { SetValue(ItemMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryColCount")]
#endif
		public int ColCount {
			get { return (int)GetValue(ColCountProperty); }
			set { SetValue(ColCountProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryRowCount")]
#endif
		public int RowCount {
			get { return (int)GetValue(RowCountProperty); }
			set { SetValue(RowCountProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryHintCaptionTemplate")]
#endif
		public DataTemplate HintCaptionTemplate {
			get { return (DataTemplate)GetValue(HintCaptionTemplateProperty); }
			set { SetValue(HintCaptionTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryIsItemContentVisible")]
#endif
		public bool IsItemContentVisible {
			get { return (bool)GetValue(IsItemContentVisibleProperty); }
			protected set { this.SetValue(IsItemContentVisiblePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryHoverAnimationDuration")]
#endif
		public int HoverAnimationDuration {
			get { return (int)GetValue(HoverAnimationDurationProperty); }
			set { SetValue(HoverAnimationDurationProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupItemsMargin")]
#endif
		public Thickness? GroupItemsMargin {
			get { return (Thickness?)GetValue(GroupItemsMarginProperty); }
			set { SetValue(GroupItemsMarginProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyphBorderPadding")]
#endif
		public Thickness? ItemGlyphBorderPadding {
			get { return (Thickness?)GetValue(ItemGlyphBorderPaddingProperty); }
			set { SetValue(ItemGlyphBorderPaddingProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryToolTipTemplate")]
#endif
		public ControlTemplate ToolTipTemplate {
			get { return (ControlTemplate)GetValue(ToolTipTemplateProperty); }
			set { SetValue(ToolTipTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyphSize")]
#endif
		public Size ItemGlyphSize {
			get { return (Size)GetValue(ItemGlyphSizeProperty); }
			set { SetValue(ItemGlyphSizeProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGlyphStretch")]
#endif
		public Stretch ItemGlyphStretch {
			get { return (Stretch)GetValue(ItemGlyphStretchProperty); }
			set { SetValue(ItemGlyphStretchProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupsSource")]
#endif
		public IEnumerable GroupsSource {
			get { return (IEnumerable)GetValue(GroupsSourceProperty); }
			set { SetValue(GroupsSourceProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupTemplate")]
#endif
		public DataTemplate GroupTemplate {
			get { return (DataTemplate)GetValue(GroupTemplateProperty); }
			set { SetValue(GroupTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupTemplateSelector")]
#endif
		public DataTemplateSelector GroupTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupTemplateSelectorProperty); }
			set { SetValue(GroupTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroupStyle")]
#endif
		public Style GroupStyle {
			get { return (Style)GetValue(GroupStyleProperty); }
			set { SetValue(GroupStyleProperty, value); }
		}
		public ICommand ItemClickCommand {
			get { return (ICommand)GetValue(ItemClickCommandProperty); }
			set { SetValue(ItemClickCommandProperty, value); }
		}
		#endregion
		public Gallery() {
			smoothScrollingTimeCore = 300;
			IsEnabledChanged += OnIsEnabledChanged;
			UpdateCheckedItems();
		}
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			Groups.ForEach(group => group.CoerceValue(IsEnabledProperty));
		}
		internal void UpdateCheckedItems() {
			CheckedItems = new ReadOnlyObservableCollection<GalleryItem>(new ObservableCollection<GalleryItem>(GetCheckedItems()));
			UpdateFirstCheckedItem(CheckedItems.FirstOrDefault());
		}
		void UpdateFirstCheckedItem(GalleryItem item) {
			this.SetCurrentValue(FirstCheckedItemProperty, item == null ? null : (item.DataItem ?? item));
		}
		double smoothScrollingTimeCore;		
		protected internal double SmoothScrollingTime { get { return smoothScrollingTimeCore; } set { smoothScrollingTimeCore = value; } }
		EventHandlerList events;	   
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		protected void RaiseEventByHandler(object eventHandler, GalleryItemEventArgs args) {
			GalleryItemEventHandler h = Events[eventHandler] as GalleryItemEventHandler;
			if(h != null) h(this, args);
		}
		protected void RaiseEventByHandler(object eventHandler, EventArgs args) {
			EventHandler h = Events[eventHandler] as EventHandler;
			if(h != null) h(this, args);
		}
		#region events
		public event GalleryItemEventHandler ItemChecked {
			add { Events.AddHandler(itemCheckedEventHandler, value); }
			remove { Events.RemoveHandler(itemCheckedEventHandler, value); }
		}
		public event GalleryItemEventHandler ItemUnchecked {
			add { Events.AddHandler(itemUncheckedEventHandler, value); }
			remove { Events.RemoveHandler(itemUncheckedEventHandler, value); }
		}
		public event GalleryItemEventHandler ItemClick {
			add { Events.AddHandler(itemClickEventHandler, value); }
			remove { Events.RemoveHandler(itemClickEventHandler, value); }
		}
		public event GalleryItemEventHandler ItemHover {
			add { Events.AddHandler(itemHoverEventHandler, value); }
			remove { Events.RemoveHandler(itemHoverEventHandler, value); }
		}
		public event GalleryItemEventHandler ItemLeave {
			add { Events.AddHandler(itemLeaveEventHandler, value); }
			remove { Events.RemoveHandler(itemLeaveEventHandler, value); }
		}
		public event GalleryItemEventHandler ItemEnter {
			add { Events.AddHandler(itemEnterEventHandler, value); }
			remove { Events.RemoveHandler(itemEnterEventHandler, value); }
		}
		public event EventHandler SizeModeChanged {
			add { Events.AddHandler(sizeModeChangedEventHandler, value); }
			remove { Events.RemoveHandler(sizeModeChangedEventHandler, value); }
		}
		protected internal event EventHandler UserMarginsChanged {
			add { Events.AddHandler(userMarginsChangedEventHandler, value); }
			remove { Events.RemoveHandler(userMarginsChangedEventHandler, value); }
		}	   
		protected internal event EventHandler IsGroupCaptionVisibleChanged {
			add { Events.AddHandler(isGroupCaptionVisibleChangedEventHandler, value); }
			remove { Events.RemoveHandler(isGroupCaptionVisibleChangedEventHandler, value); }
		}
		protected internal event EventHandler CustomTextStyleChanged {
			add { Events.AddHandler(customTextStyleChangedEventHandler, value); }
			remove { Events.RemoveHandler(customTextStyleChangedEventHandler, value); }
		}
		protected internal event EventHandler CustomTemplateChanged {
			add { Events.AddHandler(customTempateChangedEventHandler, value); }
			remove { Events.RemoveHandler(customTempateChangedEventHandler, value); }
		}
		protected internal event EventHandler VerticalScrollBarVisibilityChanged {
			add { Events.AddHandler(verticalScrollBarVisibilityEventHandler, value); }
			remove { Events.RemoveHandler(verticalScrollBarVisibilityEventHandler, value); }
		}
		protected internal event EventHandler VerticalScrollBarStyleChanged {
			add { Events.AddHandler(verticalScrollBarStyleChangedEventHandler, value); }
			remove { Events.RemoveHandler(verticalScrollBarStyleChangedEventHandler, value); }
		}		
		protected internal virtual void OnItemChecked(GalleryItemEventArgs args) {			
			UpdateCheckedItems();
			RaiseEventByHandler(itemCheckedEventHandler, args);
		}
		protected internal virtual void OnItemUnchecked(GalleryItemEventArgs args) {
			UpdateCheckedItems();
			RaiseEventByHandler(itemUncheckedEventHandler, args);
		}
		protected internal virtual void OnItemClick(GalleryItemEventArgs args) {
			RaiseEventByHandler(itemClickEventHandler, args);
			if(ItemClickCommand != null)
				ItemClickCommand.Execute(args.Item.DataItem == null ? args.Item : args.Item.DataItem);				
		}
		protected internal virtual void OnItemHover(GalleryItemEventArgs  args) {
			RaiseEventByHandler(itemHoverEventHandler, args);
		}
		protected internal virtual void OnItemLeave(GalleryItemEventArgs args) {
			RaiseEventByHandler(itemLeaveEventHandler, args);
		}
		protected internal virtual void OnItemEnter(GalleryItemEventArgs args) {
			RaiseEventByHandler(itemEnterEventHandler, args);
		}
		protected virtual void OnUserMarginsChanged() {
			RaiseEventByHandler(userMarginsChangedEventHandler, new EventArgs());
		}
		protected virtual void OnSizeModeChanged(GallerySizeMode gallerySizeMode) {
			RaiseEventByHandler(sizeModeChangedEventHandler, new EventArgs());
		}
		protected virtual void OnIsGroupCaptionVisibleChanged() {
			RaiseEventByHandler(isGroupCaptionVisibleChangedEventHandler, new EventArgs());
		}
		protected virtual void OnItemCheckModeChanged(GalleryItemCheckMode oldValue) {
			if(ItemCheckMode != GalleryItemCheckMode.Multiple) UncheckAllItems(null);
		}
		protected virtual void OnItemCaptionTextStyleSelectorChanged(StatedStyleSelector oldValue) {
		}
		protected virtual void OnItemDescriptionTextStyleSelectorChanged(StatedStyleSelector oldValue) {
		}
		protected virtual void OnCustomTextStyleChanged() {
			RaiseEventByHandler(customTextStyleChangedEventHandler, new EventArgs());
		}
		protected virtual void OnFirstCheckedItemChanged(object oldValue) {
			if(FirstCheckedItem is GalleryItem) {
				((GalleryItem)FirstCheckedItem).SetCurrentValue(GalleryItem.IsCheckedProperty, true);
			} else if(FirstCheckedItem == null) {
				UncheckAllItems(null);
			} else {
				GalleryItem item = AllItems.FirstOrDefault((i) => FirstCheckedItem.Equals(i.DataItem) );
				if(item != null)
					item.SetCurrentValue(GalleryItem.IsCheckedProperty, true);
			}
		}
		protected virtual void OnCustomTemplateChanged() {
			RaiseEventByHandler(customTempateChangedEventHandler, new EventArgs());
		}
		protected virtual void OnVerticalScrollBarVisibilityChanged() {
			RaiseEventByHandler(verticalScrollBarVisibilityEventHandler, new EventArgs());
		}
		protected virtual void OnVerticalScrollBarStyleChanged() {
			RaiseEventByHandler(verticalScrollBarStyleChangedEventHandler, new EventArgs());
		}
		protected virtual void OnGroupsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			var converter = new DevExpress.Xpf.Bars.Native.ObservableCollectionConverter<object, object>();
			converter.Source = e.NewValue as IEnumerable;
			converter.Selector = (item) => item;
			var args = new System.Windows.DependencyPropertyChangedEventArgs(e.Property, e.OldValue, converter);
			ItemsAttachedBehaviorCore<Gallery, GalleryItemGroup>.OnItemsSourcePropertyChanged(
				this,
				args,
				GroupsAttachedBehaviorProperty,
				GroupTemplateProperty,
				GroupTemplateSelectorProperty,
				GroupStyleProperty,
				gallery => gallery.Groups,
				gallery => new GalleryItemGroup(), useDefaultTemplateSelector: true);			
		}
		protected virtual void OnGroupTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<Gallery, GalleryItemGroup>.OnItemsGeneratorTemplatePropertyChanged(
				this,
				e,
				GroupsAttachedBehaviorProperty);
		}
		#endregion
		GalleryItemGroupCollection groups;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryGroups")]
#endif
		public GalleryItemGroupCollection Groups {
			get {
				if(groups == null)
					groups = CreateGroups();
				return groups; 
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryClonedFrom")]
#endif
		public Gallery ClonedFrom { get; protected internal set; }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GallerySyncWithClone")]
#endif
		public bool SyncWithClone { get; set; }
		protected virtual GalleryItemGroupCollection CreateGroups() {
			return new GalleryItemGroupCollection(this);
		}
		public Gallery CloneWithEvents() {
			Gallery clone = new Gallery();
			foreach(GalleryItemGroup group in groups) {
				clone.Groups.Add(group.CloneWithEvents());
			}
			CloneEvent(itemClickEventHandler, clone);
			CloneEvent(itemCheckedEventHandler, clone);
			CloneEvent(itemUncheckedEventHandler, clone);
			CloneDataTo(clone);
			clone.ClonedFrom = this;
			return clone;
		}
		IEnumerable<GalleryItem> AllItems {
			get {
				foreach(GalleryItemGroup group in Groups)
					foreach(GalleryItem item in group.Items)
						yield return item;
			}
		}
		protected internal GalleryControl GalleryControl { get; set; }
		internal void AddLogicalChildCore(object obj)  {
			if(obj != null) {
			AddLogicalChild(obj);
			}
		}
		internal void RemoveLogicalChildCore(object obj) {
			if(obj != null) {
				RemoveLogicalChild(obj);
			}
		}
		WeakList<object> logicalChildrenCore = new WeakList<object>();
		private new void AddLogicalChild(object obj){			
			base.AddLogicalChild(obj);
			logicalChildrenCore.Add(obj);
		}
		private new void RemoveLogicalChild(object obj){
			logicalChildrenCore.Remove(obj);
			base.RemoveLogicalChild(obj);
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return logicalChildrenCore.GetEnumerator(); }
		}
		public GalleryItem GetFirstCheckedItem() {
			foreach(GalleryItemGroup group in Groups) {
				foreach(GalleryItem item in group.Items) {
					if(item.IsChecked) return item;
				}
			}
			return null;
		}
		public List<GalleryItem> GetCheckedItems() {
			var checkedItems = Groups.SelectMany(group => group.Items).Where(item => item.IsChecked);
			return new List<GalleryItem>(checkedItems);
		}
		public Gallery CloneWithoutEvents() {
			Gallery clone = new Gallery();
			CloneDataTo(clone);
			foreach(GalleryItemGroup group in groups) {
				clone.Groups.Add(group.CloneWithoutEvents());
			}
			clone.ClonedFrom = this;
			return clone;
		}
		void CloneDataTo(Gallery targetObject) {
			targetObject.ColCount = ColCount;
			targetObject.RowCount = RowCount;
			targetObject.MinColCount = MinColCount;
			targetObject.ItemMargin = ItemMargin;
			targetObject.ItemGlyphVerticalAlignment = ItemGlyphVerticalAlignment;
			targetObject.ItemGlyphLocation = ItemGlyphLocation;
			targetObject.ItemGlyphHorizontalAlignment = ItemGlyphHorizontalAlignment;
			targetObject.ItemGlyphMargin = ItemGlyphMargin;
			targetObject.ItemDescriptionVerticalAlignment = ItemDescriptionVerticalAlignment;
			targetObject.ItemDescriptionTemplate = ItemDescriptionTemplate;
			targetObject.ItemDescriptionMargin = ItemDescriptionMargin;
			targetObject.ItemDescriptionHorizontalAlignment = ItemDescriptionHorizontalAlignment;
			targetObject.ItemContentVerticalAlignment = ItemContentVerticalAlignment;
			targetObject.ItemContentHorizontalAlignment = ItemContentHorizontalAlignment;
			targetObject.ItemCaptionVerticalAlignment = ItemCaptionVerticalAlignment;
			targetObject.ItemCaptionTemplate = ItemCaptionTemplate;
			targetObject.ItemCaptionMargin = ItemCaptionMargin;
			targetObject.ItemCaptionHorizontalAlignment = ItemCaptionHorizontalAlignment;
			targetObject.IsItemDescriptionVisible = IsItemDescriptionVisible;
			targetObject.IsItemCaptionVisible = IsItemCaptionVisible;
			targetObject.IsItemGlyphVisible = IsItemGlyphVisible;
			targetObject.IsGroupCaptionVisible = IsGroupCaptionVisible;
			targetObject.HoverGlyphSize = HoverGlyphSize;
			targetObject.GroupCaptionVerticalAlignment = GroupCaptionVerticalAlignment;
			targetObject.GroupCaptionTemplate = GroupCaptionTemplate;
			targetObject.GroupCaptionMargin = GroupCaptionMargin;
			targetObject.GroupCaptionHorizontalAlignment = GroupCaptionHorizontalAlignment;
			targetObject.FilterCaptionTemplate = FilterCaptionTemplate;
			targetObject.FilterCaption = FilterCaption;
			targetObject.CheckDrawMode = CheckDrawMode;
			targetObject.AllowHoverImages = AllowHoverImages;
			targetObject.AllowHoverAnimation = AllowHoverAnimation;
			targetObject.AllowFilter = AllowFilter;
			targetObject.SizeMode = SizeMode;
			targetObject.AutoHideGallery = AutoHideGallery;
			targetObject.HintTemplate = HintTemplate;
			targetObject.ItemCheckMode = ItemCheckMode;
			targetObject.GroupItemsMargin = GroupItemsMargin;
			targetObject.ToolTipTemplate = ToolTipTemplate;
			targetObject.MaxWidth = MaxWidth;
			targetObject.MaxHeight = MaxHeight;
			targetObject.MinWidth = MinWidth;
			targetObject.MinHeight = MinHeight;
			targetObject.Width = Width;
			targetObject.Height = Height;
			targetObject.Opacity = Opacity;
			targetObject.Margin = Margin;
			targetObject.ItemAutoHeight = ItemAutoHeight;
		}
		void CloneEvent(object eventHandler, Gallery targetObject) {
			EventHandler baseEventHandler = Events[eventHandler] as EventHandler;
			EventHandler targetEventHandler = targetObject.Events[eventHandler] as EventHandler;
			if(targetEventHandler == null && baseEventHandler != null)
				targetObject.Events.AddHandler(eventHandler, baseEventHandler);
		}
		protected internal virtual void UncheckAllItems(GalleryItem ignorableItem) {
			var checkedItems = GetCheckedItems().Where(item => item != ignorableItem);
			foreach(var item in checkedItems) {
				item.SetCurrentValue(GalleryItem.IsCheckedProperty, false);
			}
		}
		protected virtual void UpdateActualContentVisibility() {
			IsItemContentVisible = IsItemCaptionVisible || IsItemDescriptionVisible;
		}
	}
	public class GalleryCollection<T> : ObservableCollection<T>  {
		#region static
		static GalleryCollection() {
		}
		#endregion
		protected virtual void PrepareItem(T item) {
		}
		protected virtual void ClearItem(T item) {
		}
		protected override void ClearItems() {
			foreach(var item in this) {
				ClearItem(item);
			}
			base.ClearItems();
		}
		protected override void InsertItem(int index, T item) {
			base.InsertItem(index, item);
			PrepareItem(item);
		}
		protected override void RemoveItem(int index) {
			ClearItem(Items[index]);
			base.RemoveItem(index);
		}
		protected override void SetItem(int index, T item) {
			ClearItem(Items[index]);
			base.SetItem(index, item);
			PrepareItem(item);
		}
	}
	public class GalleryItemGroupCollection : GalleryCollection<GalleryItemGroup> {
		Gallery ParentGallery { get; set; }
		public GalleryItemGroupCollection(Gallery parentGallery) {
			ParentGallery = parentGallery;
		}
		protected override void ClearItem(GalleryItemGroup item) {
			item.Gallery = null;
			ParentGallery.RemoveLogicalChildCore(item);
		}
		protected override void PrepareItem(GalleryItemGroup item) {
			item.Gallery = ParentGallery;
			if(item.Parent == null)
				ParentGallery.AddLogicalChildCore(item);
		}
	}
	public class GalleryHelper {
		public static bool GetIsInRibbonControl(DependencyObject obj) {
			return (bool)obj.GetValue(IsInRibbonControlProperty);
		}
		public static void SetIsInRibbonControl(DependencyObject obj, bool value) {
			obj.SetValue(IsInRibbonControlProperty, value);
		}				
		public static readonly DependencyProperty IsInRibbonControlProperty =
			DependencyPropertyManager.RegisterAttached("IsInRibbonControl", typeof(bool), typeof(GalleryHelper), new FrameworkPropertyMetadata(false));				
	}
}
