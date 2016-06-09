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

using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Internal;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Ribbon.Automation;
using DevExpress.Xpf.Ribbon.Internal;
using DevExpress.Xpf.Ribbon.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
namespace DevExpress.Xpf.Ribbon {
	public enum RibbonHeaderVisibility { Visible, Collapsed }
	public enum RibbonMinimizationButtonVisibility { Auto, Collapsed, Visible }
	public enum RibbonTitleBarVisibility { Auto, Collapsed, Visible }
	public enum RibbonQuickAccessToolbarShowMode { Default = 0, ShowAbove = 0, ShowBelow = 1, Hide = 2 }
	public enum RibbonPageCategoryCaptionAlignment { Default = 0, Left = 0, Right = 1 }
	public enum RibbonStyle { Office2007, Office2010, TabletOffice, OfficeSlim }
	public enum SelectedPageOnMerging { ParentSelectedPage, SelectedPage }
	public enum RibbonMenuIconStyle { Mono, Color, Office2013 }
	[DXToolboxBrowsable]
	[ToolboxTabName(AssemblyInfo.DXTabWpfNavigation)]
	[ContentProperty("Categories")]
	public class RibbonControl : Control, IRibbonControl, ISupportInitialize, ILogicalChildrenContainer, IMultipleElementRegistratorSupport, IMergingSupport, IHierarchicalMergingSupport<RibbonControl>, IRuntimeCustomizationHost, IEventListenerClient {
		#region static 
		static readonly object managerChanged = new object();
		static readonly object selectedPageChangedEventHandler = new object();
		static readonly object autoHideModeChangedEventHandler = new object();
		static readonly object toolbarModeChangedEventHandler = new object();
		static readonly object toolbarCustomizationMenuShowingEventHandler = new object();
		static readonly object toolbarCustomizationMenuClosedEventHandler = new object();
		static readonly object popupMenuShowingEventHandler = new object();
		static readonly object popupMenuClosedEventHandler = new object();
		static readonly object backstageOpenedEventHandler = new object();
		static readonly object backstageClosedEventHandler = new object();
		static readonly object ribbonPageInsertedEventHandler = new object();
		static readonly object ribbonPageRemovedEventHandler = new object();
		static readonly object ribbonPageGroupInsertedEventHandler = new object();
		static readonly object ribbonPageGroupRemovedEventHandler = new object();
		static readonly object saveLayoutExceptionEventHanler = new object();
		static readonly object restoreLayoutExceptionEventHanler = new object();
		public static readonly RoutedEvent ApplicationMenuChangedEvent;
		public static readonly DependencyProperty SelectedPagePopupMarginProperty;
		public static readonly DependencyProperty SelectedPageOnMergingProperty;
		public static readonly DependencyProperty ActualHeaderBorderTemplateProperty;
		public static readonly DependencyProperty HeaderBorderTemplateInRibbonWindowProperty;
		public static readonly DependencyProperty DefaultTemplateProperty;
		public static readonly DependencyProperty AeroTemplateProperty;
		public static readonly DependencyProperty AeroBorderRibbonStyleProperty;
		public static readonly DependencyProperty AeroBorderTopOffsetProperty;
		public static readonly DependencyProperty CollapsedRibbonAeroBorderTopOffsetProperty;
		public static readonly DependencyProperty StandaloneHeaderBorderTemplateProperty;
		public static readonly DependencyProperty MaxPageCaptionTextIndentProperty;
		public static readonly DependencyProperty SelectedPageProperty;
		public static readonly DependencyProperty SelectedPageNameProperty;
		public static readonly DependencyProperty ToolbarShowModeProperty;
		public static readonly DependencyProperty ButtonGroupVertAlignProperty;
		public static readonly DependencyProperty ColumnIndentProperty;
		public static readonly DependencyProperty RowIndentProperty;
		public static readonly DependencyProperty ButtonGroupsIndentProperty;
		public static readonly DependencyProperty AllowMinimizeRibbonProperty;
		public static readonly DependencyProperty IsMinimizedProperty;
		public static readonly DependencyProperty ApplicationMenuProperty;
		public static readonly DependencyProperty ShowApplicationButtonProperty;
		public static readonly DependencyProperty ApplicationButtonLargeIconProperty;
		public static readonly DependencyProperty ActualApplicationButtonLargeIconProperty;
		protected static readonly DependencyPropertyKey ActualApplicationButtonLargeIconPropertyKey;
		public static readonly DependencyProperty ApplicationButtonSmallIconProperty;
		public static readonly DependencyProperty ActualApplicationButtonSmallIconProperty;
		protected static readonly DependencyPropertyKey ActualApplicationButtonSmallIconPropertyKey;
		public static readonly DependencyProperty ActualIsApplicationButtonTextVisibleProperty;
		protected static readonly DependencyPropertyKey ActualIsApplicationButtonTextVisiblePropertyKey;
		public static readonly DependencyProperty ApplicationButtonTextProperty;
		public static readonly DependencyProperty PageCaptionMinWidthProperty;
		public static readonly DependencyProperty PageCategoryAlignmentProperty;
		public static readonly DependencyProperty IsMinimizedRibbonCollapsedProperty;
		protected internal static readonly DependencyPropertyKey IsMinimizedRibbonCollapsedPropertyKey;
		public static readonly DependencyProperty IsBackStageViewOpenProperty;
		protected internal static readonly DependencyPropertyKey IsBackStageViewOpenPropertyKey;
		public static readonly DependencyProperty RibbonStyleProperty;
		public static readonly DependencyProperty RibbonProperty;
		public static readonly DependencyProperty KeyTipApplicationButtonProperty;
		public static readonly DependencyProperty IsHeaderBorderVisibleProperty;
		public static readonly DependencyProperty IsInRibbonWindowProperty;
		protected static readonly DependencyPropertyKey IsInRibbonWindowPropertyKey;
		public static readonly DependencyProperty IsAeroModeProperty;
		protected static readonly DependencyPropertyKey IsAeroModePropertyKey;
		public static readonly DependencyProperty ApplicationMenuVerticalOffsetInAeroWindowProperty;
		public static readonly DependencyProperty ApplicationMenuHorizontalOffsetInAeroWindowProperty;
		public static readonly DependencyProperty HoverPageCaptionTextStyleProperty;
		public static readonly DependencyProperty NormalPageCaptionTextStyleProperty;
		public static readonly DependencyProperty SelectedPageCaptionTextStyleProperty;
		public static readonly DependencyProperty GroupCaptionTextStyleProperty;
		public static readonly DependencyProperty ApplicationMenuVerticalOffsetProperty;
		public static readonly DependencyProperty ApplicationMenuHorizontalOffsetProperty;
		public static readonly DependencyProperty AllowKeyTipsProperty;
		public static readonly DependencyProperty IsOffice2010StyleProperty;
		protected static readonly DependencyPropertyKey IsOffice2010StylePropertyKey;
		public static readonly DependencyProperty ActualApplicationButtonStyleProperty;
		protected static readonly DependencyPropertyKey ActualApplicationButtonStylePropertyKey;
		public static readonly DependencyProperty ApplicationButton2007StyleProperty;
		public static readonly DependencyProperty ApplicationButton2010StyleProperty;
		public static readonly DependencyProperty ApplicationButton2007StyleInPopupProperty;
		public static readonly DependencyProperty ApplicationButton2010StyleInPopupProperty;
		public static readonly DependencyProperty ApplicationButtonTabletOfficeStyleProperty;
		public static readonly DependencyProperty ApplicationButtonTabletOfficeStyleInPopupProperty;
		public static readonly DependencyProperty ApplicationButtonOfficeSlimStyleProperty;
		public static readonly DependencyProperty ApplicationButtonOfficeSlimStyleInPopupProperty;
		public static readonly DependencyProperty ApplicationButtonPopupHorizontalOffsetProperty;
		public static readonly DependencyProperty ApplicationButtonPopupVerticalOffsetProperty;
		public static readonly DependencyProperty ApplicationButton2010PopupHorizontalOffsetProperty;
		public static readonly DependencyProperty ApplicationButton2010PopupVerticalOffsetProperty;
		public static readonly DependencyProperty ActualApplicationButtonWidthProperty;
		protected static readonly DependencyPropertyKey ActualApplicationButtonWidthPropertyKey;
		public static readonly DependencyProperty ActualApplicationIconProperty;
		protected static readonly DependencyPropertyKey ActualApplicationIconPropertyKey;
		public static readonly DependencyProperty IsApplicationIconVisibleProperty;
		protected static readonly DependencyPropertyKey IsApplicationIconVisiblePropertyKey;
		public static readonly DependencyProperty ActualHeaderQuickAccessToolbarContainerStyleProperty;
		protected static readonly DependencyPropertyKey ActualHeaderQuickAccessToolbarContainerStylePropertyKey;
		public static readonly DependencyProperty ActualWindowTitleProperty;
		protected static readonly DependencyPropertyKey ActualWindowTitlePropertyKey;
		public static readonly DependencyProperty ApplicationButton2007StyleInAeroWindowProperty;
		public static readonly DependencyProperty ToolbarShowCustomizationButtonProperty;
		public static readonly DependencyProperty RibbonTitleBarVisibilityProperty;
		public static readonly DependencyProperty RibbonHeaderVisibilityProperty;
		public static readonly DependencyProperty MinimizationButtonVisibilityProperty;
		public static readonly DependencyProperty IsMinimizationButtonVisibleProperty;
		protected static readonly DependencyPropertyKey IsMinimizationButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsRibbonTitleBarActualVisibleProperty;
		protected static readonly DependencyPropertyKey IsRibbonTitleBarActualVisiblePropertyKey;
		public static readonly DependencyProperty HeaderQAT2007ContainerStyleInRibbonWindowProperty;
		public static readonly DependencyProperty HeaderQAT2007ContainerStyleInAeroWindowProperty;
		public static readonly DependencyProperty HeaderQAT2007ContainerStyleProperty;
		public static readonly DependencyProperty HeaderQATContainerStyleWithoutApplicationIconProperty;
		public static readonly DependencyProperty HeaderQATContainerStyleWithoutAppIconInAeroWindowProperty;
		public static readonly DependencyProperty HeaderQAT2010ContainerStyleProperty;
		public static readonly DependencyProperty HeaderQAT2010ContainerStyleInRibbonWindowProperty;
		public static readonly DependencyProperty HeaderQAT2010ContainerStyleInAeroWindowProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty CategoryAttachedBehaviorProperty;
		public static readonly DependencyProperty CategoriesSourceProperty;
		public static readonly DependencyProperty CategoryTemplateProperty;
		public static readonly DependencyProperty CategoryTemplateSelectorProperty;
		public static readonly DependencyProperty CategoryStyleProperty;
		public static readonly DependencyProperty IsMergedProperty;
		protected static readonly DependencyPropertyKey IsMergedPropertyKey;
		public static readonly DependencyProperty ToolbarItemLinksSourceProperty;
		public static readonly DependencyProperty ToolbarItemTemplateProperty;
		public static readonly DependencyProperty ToolbarItemTemplateSelectorProperty;
		public static readonly DependencyProperty ToolbarItemStyleProperty;
		public static readonly DependencyProperty PageHeaderItemLinksSourceProperty;
		public static readonly DependencyProperty PageHeaderItemTemplateProperty;
		public static readonly DependencyProperty PageHeaderItemTemplateSelectorProperty;
		public static readonly DependencyProperty PageHeaderItemStyleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty PageHeaderItemsAttachedBehaviorProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty ToolbarItemsAttachedBehaviorProperty;
		public static readonly DependencyProperty ItemLinksSourceElementGeneratesUniqueBarItemProperty;
		public static readonly DependencyProperty CategoriesProperty;
		protected static readonly DependencyPropertyKey CategoriesPropertyKey;
		public static readonly DependencyProperty ItemsProperty;
		protected static readonly DependencyPropertyKey ItemsPropertyKey;
		public static readonly DependencyProperty ActualCategoriesProperty;
		protected static readonly DependencyPropertyKey ActualCategoriesPropertyKey;
		public static readonly DependencyProperty AllowCustomizationProperty;
		public static readonly DependencyProperty HideEmptyGroupsProperty;
		public static readonly DependencyProperty AllowRibbonNavigationFromEditorOnTabPressProperty;
		protected static object OnAllowCustomizationPropertyChangingCoerce(DependencyObject sender, object value) {
			return value;
		}
		public static readonly DependencyProperty AutoHideModeProperty;
		public static readonly DependencyProperty AllowCustomizingDefaultGroupsProperty;
		public static readonly DependencyProperty MDIMergeStyleProperty;
		public static readonly DependencyProperty AllowAddingToToolbarProperty;
		public static readonly DependencyProperty MenuIconStyleProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		protected internal static readonly DependencyProperty ApplicationIconProperty;
		public static readonly DependencyProperty AsyncMergingEnabledProperty;
		public static readonly DependencyProperty IsHiddenRibbonCollapsedProperty;
		static RibbonControl() {
			UseCaptionToIdentifyPagesOnMerging = true;
			ApplicationMenuChangedEvent = EventManager.RegisterRoutedEvent("ApplicationMenuChanged", RoutingStrategy.Direct, typeof(RoutedValueChangedEventHandler<DependencyObject>), typeof(RibbonControl));
			Type ownerType = typeof(RibbonControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			var iWindowServiceProperty = typeof(Window).GetField("IWindowServiceProperty", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).GetValue(null) as DependencyProperty;
			if (iWindowServiceProperty != null) {
				iWindowServiceProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(new PropertyChangedCallback((d, e) => ((RibbonControl)d).OnWindowServiceChanged(e.OldValue, e.NewValue))));
			}
			AsyncMergingEnabledProperty = DependencyProperty.Register("AsyncMergingEnabled", typeof(bool), typeof(RibbonControl), new PropertyMetadata(true));
			ApplicationIconProperty = DependencyPropertyManager.Register("ApplicationIcon", typeof(ImageSource), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((RibbonControl)d).OnApplicationIconChanged((ImageSource)e.OldValue)));
			BarManager.BarManagerProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, (d, e) => ((RibbonControl)d).OnManagerChanged((BarManager)e.OldValue, (BarManager)e.NewValue)));
			HideEmptyGroupsProperty = DependencyPropertyManager.Register("HideEmptyGroups", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((RibbonControl)d).OnHideEmptyGroupsChanged((bool)e.OldValue)));
			SelectedPagePopupMarginProperty = DependencyPropertyManager.Register("SelectedPagePopupMargin", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness()));
			SelectedPageOnMergingProperty = DependencyPropertyManager.Register("SelectedPageOnMerging", typeof(SelectedPageOnMerging), ownerType, new FrameworkPropertyMetadata(SelectedPageOnMerging.SelectedPage));
			ToolbarItemLinksSourceProperty = DependencyProperty.Register("ToolbarItemLinksSource", typeof(IEnumerable), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnToolbarItemLinksSourcePropertyChanged)));
			ToolbarItemTemplateProperty = DependencyProperty.Register("ToolbarItemTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnToolbarItemLinksTemplatePropertyChanged)));
			ToolbarItemTemplateSelectorProperty = DependencyProperty.Register("ToolbarItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnToolbarItemLinksTemplateSelectorPropertyChanged)));
			ToolbarItemStyleProperty = DependencyProperty.Register("ToolbarItemStyle", typeof(Style), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnToolbarItemLinksTemplatePropertyChanged)));
			ToolbarItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ToolbarItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<RibbonControl, BarItem>), ownerType, new UIPropertyMetadata(null));
			PageHeaderItemLinksSourceProperty = DependencyProperty.Register("PageHeaderItemLinksSource", typeof(IEnumerable), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnPageHeaderItemLinksSourcePropertyChanged)));
			PageHeaderItemTemplateProperty = DependencyProperty.Register("PageHeaderItemTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnPageHeaderItemLinksTemplatePropertyChanged)));
			PageHeaderItemTemplateSelectorProperty = DependencyProperty.Register("PageHeaderItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnPageHeaderItemLinksTemplateSelectorPropertyChanged)));
			PageHeaderItemStyleProperty = DependencyProperty.Register("PageHeaderItemStyle", typeof(Style), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnPageHeaderItemLinksTemplatePropertyChanged)));
			PageHeaderItemsAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("PageHeaderItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<RibbonControl, BarItem>), ownerType, new UIPropertyMetadata(null));
			ItemLinksSourceElementGeneratesUniqueBarItemProperty = DependencyPropertyManager.Register("ItemLinksSourceElementGeneratesUniqueBarItem", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, new PropertyChangedCallback((d, e) => ((RibbonControl)d).OnItemLinksSourceElementGeneratesUniqueBarItemChanged((bool)e.OldValue))));
			CategoryAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("CategoryAttachedBehavior", typeof(ItemsAttachedBehaviorCore<RibbonControl, RibbonPageCategoryBase>), ownerType, new UIPropertyMetadata(null));
			CategoriesSourceProperty = DependencyProperty.Register("CategoriesSource", typeof(object), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnCategoriesSourcePropertyChanged)));
			CategoryTemplateProperty = DependencyProperty.Register("CategoryTemplate", typeof(DataTemplate), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnCategoryTemplatePropertyChanged)));
			CategoryTemplateSelectorProperty = DependencyProperty.Register("CategoryTemplateSelector", typeof(DataTemplateSelector), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnCategoryTemplatePropertyChanged)));
			CategoryStyleProperty = DependencyProperty.Register("CategoryStyle", typeof(Style), ownerType, new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnCategoryTemplatePropertyChanged)));
			AllowCustomizationProperty = DependencyPropertyManager.Register("AllowCustomization", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, null, new CoerceValueCallback(OnAllowCustomizationPropertyChangingCoerce)));
			RibbonProperty = DependencyPropertyManager.RegisterAttached("Ribbon", ownerType, ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));
			AllowAddingToToolbarProperty = DependencyPropertyManager.RegisterAttached("AllowAddingToToolbar", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			AllowCustomizingDefaultGroupsProperty = DependencyPropertyManager.Register("AllowCustomizingDefaultGroups", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			AllowRibbonNavigationFromEditorOnTabPressProperty = DependencyPropertyManager.Register("AllowRibbonNavigationFromEditorOnTabPress", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			MaxPageCaptionTextIndentProperty = DependencyPropertyManager.Register("MaxPageCaptionTextIndent", typeof(double), ownerType, new UIPropertyMetadata(10d));
			SelectedPageProperty = DependencyPropertyManager.Register("SelectedPage", typeof(RibbonPage), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSelectedPagePropertyChanged)));
			SelectedPageNameProperty = DependencyPropertyManager.Register("SelectedPageName", typeof(string), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnSelectedPageNamePropertyChanged)));
			ToolbarShowModeProperty = DependencyPropertyManager.Register("ToolbarShowMode", typeof(RibbonQuickAccessToolbarShowMode), ownerType, new FrameworkPropertyMetadata(RibbonQuickAccessToolbarShowMode.Default, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnToolbarShowModePropertyChanged)));
			ButtonGroupVertAlignProperty = DependencyPropertyManager.Register("ButtonGroupVertAlign", typeof(VerticalAlignment), ownerType, new FrameworkPropertyMetadata(VerticalAlignment.Center, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ColumnIndentProperty = DependencyPropertyManager.Register("ColumnIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
			RowIndentProperty = DependencyPropertyManager.Register("RowIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ButtonGroupsIndentProperty = DependencyPropertyManager.Register("ButtonGroupsIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsMeasure));
			AllowMinimizeRibbonProperty = DependencyPropertyManager.Register("AllowMinimizeRibbon", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnAllowMinimizeRibbonPropertyChanged)));
			IsMinimizedProperty = DependencyPropertyManager.Register("IsMinimized", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnIsMinimizedPropertyChanged), new CoerceValueCallback(OnIsMinimizedPropertyCoerce)));
			ApplicationMenuProperty = DependencyPropertyManager.Register("ApplicationMenu", typeof(DependencyObject), ownerType,
				new FrameworkPropertyMetadata(null, OnApplicationMenuPropertyChanged));
			ShowApplicationButtonProperty = DependencyPropertyManager.Register("ShowApplicationButton", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure, OnShowApplicationButtonPropertyChanged));
			ApplicationButtonLargeIconProperty = DependencyPropertyManager.Register("ApplicationButtonLargeIcon", typeof(ImageSource), ownerType,
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnApplicationButtonLargeIconPropertyChanged)));
			ActualApplicationButtonLargeIconPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualApplicationButtonLargeIcon", typeof(ImageSource), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ActualApplicationButtonLargeIconProperty = ActualApplicationButtonLargeIconPropertyKey.DependencyProperty;
			PageCaptionMinWidthProperty = DependencyPropertyManager.Register("PageCaptionMinWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(20d, FrameworkPropertyMetadataOptions.AffectsMeasure));
			PageCategoryAlignmentProperty = DependencyPropertyManager.Register("PageCategoryAlignment", typeof(RibbonPageCategoryCaptionAlignment), ownerType,
				new FrameworkPropertyMetadata(RibbonPageCategoryCaptionAlignment.Default, OnPageCategoryAlignmentPropertyChanged));
			IsMinimizedRibbonCollapsedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsMinimizedRibbonCollapsed", typeof(bool), ownerType,
				new PropertyMetadata(false, new PropertyChangedCallback(OnIsMinimizedRibbonCollapsedPropertyChanged)));
			IsMinimizedRibbonCollapsedProperty = IsMinimizedRibbonCollapsedPropertyKey.DependencyProperty;
			IsBackStageViewOpenPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsBackStageViewOpen", typeof(bool), ownerType,
				new PropertyMetadata(false, new PropertyChangedCallback(OnIsBackstageViewOpenPropertyChanged)));
			IsBackStageViewOpenProperty = IsBackStageViewOpenPropertyKey.DependencyProperty;
			RibbonStyleProperty = DependencyPropertyManager.Register("RibbonStyle", typeof(RibbonStyle), ownerType,
				new FrameworkPropertyMetadata(RibbonStyle.Office2007, FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure, OnRibbonStylePropertyChanged));
			KeyTipApplicationButtonProperty = DependencyPropertyManager.Register("KeyTipApplicationButton", typeof(string), ownerType,
				new UIPropertyMetadata(string.Empty));
			IsHeaderBorderVisibleProperty = DependencyPropertyManager.Register("IsHeaderBorderVisible", typeof(bool), ownerType, new UIPropertyMetadata(false));
			IsInRibbonWindowPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsInRibbonWindow", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsInRibbonWindowPropertyChanged)));
			IsInRibbonWindowProperty = IsInRibbonWindowPropertyKey.DependencyProperty;
			IsAeroModePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsAeroMode", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAeroModePropertyChanged)));
			IsAeroModeProperty = IsAeroModePropertyKey.DependencyProperty;
			ApplicationMenuHorizontalOffsetInAeroWindowProperty = DependencyPropertyManager.Register("ApplicationMenuHorizontalOffsetInAeroWindow", typeof(double), ownerType,
				new UIPropertyMetadata(0d));
			ApplicationMenuVerticalOffsetInAeroWindowProperty = DependencyPropertyManager.Register("ApplicationMenuVerticalOffsetInAeroWindow", typeof(double), ownerType,
				new UIPropertyMetadata(0d));
			ActualHeaderBorderTemplateProperty = DependencyPropertyManager.Register("ActualHeaderBorderTemplate", typeof(ControlTemplate), ownerType, new UIPropertyMetadata(null));
			NormalPageCaptionTextStyleProperty = DependencyPropertyManager.Register("NormalPageCaptionTextStyle", typeof(Style), ownerType, new UIPropertyMetadata(null));
			HoverPageCaptionTextStyleProperty = DependencyPropertyManager.Register("HoverPageCaptionTextStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			GroupCaptionTextStyleProperty = DependencyPropertyManager.Register("GroupCaptionTextStyle", typeof(Style), ownerType, new UIPropertyMetadata(null));
			SelectedPageCaptionTextStyleProperty = DependencyPropertyManager.Register("SelectedPageCaptionTextStyle", typeof(Style), ownerType, new UIPropertyMetadata(null));
			ApplicationMenuHorizontalOffsetProperty = DependencyPropertyManager.Register("ApplicationMenuHorizontalOffset", typeof(double), ownerType, new UIPropertyMetadata(0d));
			ApplicationMenuVerticalOffsetProperty = DependencyPropertyManager.Register("ApplicationMenuVerticalOffset", typeof(double), ownerType, new UIPropertyMetadata(0d));
			HeaderBorderTemplateInRibbonWindowProperty = DependencyPropertyManager.Register("HeaderBorderTemplateInRibbonWindow", typeof(ControlTemplate), ownerType,
				new PropertyMetadata(null, OnHeaderBorderTemplateInRibbonWindowPropertyChanged));
			StandaloneHeaderBorderTemplateProperty = DependencyPropertyManager.Register("StandaloneHeaderBorderTemplate", typeof(ControlTemplate), ownerType,
				new PropertyMetadata(null, OnStandaloneHeaderBorderTemplatePropertyChanged));
			ApplicationButtonSmallIconProperty = DependencyPropertyManager.Register("ApplicationButtonSmallIcon", typeof(ImageSource), ownerType,
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnApplicationButtonSmallIconPropertyChanged)));
			ActualApplicationButtonSmallIconPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualApplicationButtonSmallIcon", typeof(ImageSource), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ActualApplicationButtonSmallIconProperty = ActualApplicationButtonSmallIconPropertyKey.DependencyProperty;
			ApplicationButtonTextProperty = DependencyPropertyManager.Register("ApplicationButtonText", typeof(string), ownerType,
				new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnApplicationButtonTextPropertyChanged)));
			AllowKeyTipsProperty = DependencyPropertyManager.Register("AllowKeyTips", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAllowKeyTipsPropertyChanged)));
			IsOffice2010StylePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsOffice2010Style", typeof(bool), ownerType, new UIPropertyMetadata(false));
			IsOffice2010StyleProperty = IsOffice2010StylePropertyKey.DependencyProperty;
			ActualApplicationButtonStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualApplicationButtonStyle", typeof(Style), ownerType, new UIPropertyMetadata(null));
			ActualApplicationButtonStyleProperty = ActualApplicationButtonStylePropertyKey.DependencyProperty;
			ApplicationButton2007StyleProperty = DependencyPropertyManager.Register("ApplicationButton2007Style", typeof(Style), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnApplicationButton2007StylePropertyChanged)));
			ApplicationButton2010StyleProperty = DependencyPropertyManager.Register("ApplicationButton2010Style", typeof(Style), ownerType,
				new UIPropertyMetadata(null, new PropertyChangedCallback(OnApplicationButton2010StylePropertyChanged)));
			ApplicationButtonTabletOfficeStyleProperty = DependencyPropertyManager.Register("ApplicationButtonTabletOfficeStyle", typeof(Style), ownerType,
				new PropertyMetadata(OnApplicationButtonTabletOfficeStylePropertyChanged));
			ApplicationButtonOfficeSlimStyleProperty = DependencyProperty.Register("ApplicationButtonOfficeSlimStyle", typeof(Style), ownerType, new PropertyMetadata(OnApplicationButtonOfficeSlimStylePropertyChanged));
			ApplicationButton2007StyleInPopupProperty = DependencyPropertyManager.Register("ApplicationButton2007StyleInPopup", typeof(Style), ownerType, new UIPropertyMetadata(null));
			ApplicationButton2010StyleInPopupProperty = DependencyPropertyManager.Register("ApplicationButton2010StyleInPopup", typeof(Style), ownerType, new UIPropertyMetadata(null));
			ApplicationButtonTabletOfficeStyleInPopupProperty = DependencyPropertyManager.Register("ApplicationButtonTabletOfficeStyleInPopup", typeof(Style), ownerType, new PropertyMetadata(null));
			ApplicationButtonOfficeSlimStyleInPopupProperty = DependencyPropertyManager.Register("ApplicationButtonOfficeSlimStyleInPopup", typeof(Style), ownerType, new PropertyMetadata(null));
			ApplicationButtonPopupHorizontalOffsetProperty = DependencyPropertyManager.Register("ApplicationButtonPopupHorizontalOffset", typeof(double), ownerType, new UIPropertyMetadata(0d));
			ApplicationButtonPopupVerticalOffsetProperty = DependencyPropertyManager.Register("ApplicationButtonPopupVerticalOffset", typeof(double), ownerType, new UIPropertyMetadata(0d));
			ApplicationButton2010PopupHorizontalOffsetProperty = DependencyPropertyManager.Register("ApplicationButton2010PopupHorizontalOffset", typeof(double), ownerType, new UIPropertyMetadata(0d));
			ApplicationButton2010PopupVerticalOffsetProperty = DependencyPropertyManager.Register("ApplicationButton2010PopupVerticalOffset", typeof(double), ownerType, new UIPropertyMetadata(0d));
			ActualApplicationButtonWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualApplicationButtonWidth", typeof(double), ownerType, new UIPropertyMetadata(0d));
			ActualApplicationButtonWidthProperty = ActualApplicationButtonWidthPropertyKey.DependencyProperty;
			ActualIsApplicationButtonTextVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsApplicationButtonTextVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			ActualIsApplicationButtonTextVisibleProperty = ActualIsApplicationButtonTextVisiblePropertyKey.DependencyProperty;
			ActualApplicationIconPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualApplicationIcon", typeof(ImageSource), ownerType,
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnActualApplicationIconPropertyChanged)));
			ActualApplicationIconProperty = ActualApplicationIconPropertyKey.DependencyProperty;
			IsApplicationIconVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsApplicationIconVisible", typeof(bool), ownerType, new UIPropertyMetadata(false));
			IsApplicationIconVisibleProperty = IsApplicationIconVisiblePropertyKey.DependencyProperty;
			ActualHeaderQuickAccessToolbarContainerStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualHeaderQuickAccessToolbarContainerStyle", typeof(Style), ownerType, new UIPropertyMetadata(null));
			ActualHeaderQuickAccessToolbarContainerStyleProperty = ActualHeaderQuickAccessToolbarContainerStylePropertyKey.DependencyProperty;
			ActualWindowTitlePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualWindowTitle", typeof(string), ownerType, new FrameworkPropertyMetadata(string.Empty));
			ActualWindowTitleProperty = ActualWindowTitlePropertyKey.DependencyProperty;
			ApplicationButton2007StyleInAeroWindowProperty = DependencyPropertyManager.Register("ApplicationButton2007StyleInAeroWindow", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnApplicationButton2007StyleInAeroWindowPropertyChanged)));
			ToolbarShowCustomizationButtonProperty = DependencyPropertyManager.Register("ToolbarShowCustomizationButton", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnToolbarShowCustomizationButtonPropertyChanged)));
			RibbonTitleBarVisibilityProperty = DependencyPropertyManager.Register("RibbonTitleBarVisibility", typeof(RibbonTitleBarVisibility), ownerType,
				new FrameworkPropertyMetadata(RibbonTitleBarVisibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnRibbonTitleBarVisibilityPropertyChanged)));
			RibbonHeaderVisibilityProperty = DependencyPropertyManager.Register("RibbonHeaderVisibility", typeof(RibbonHeaderVisibility), ownerType,
				new FrameworkPropertyMetadata(RibbonHeaderVisibility.Visible, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnRibbonHeaderVisibilityPropertyChanged)));
			MinimizationButtonVisibilityProperty = DependencyPropertyManager.Register("MinimizationButtonVisibility", typeof(RibbonMinimizationButtonVisibility), ownerType,
				new FrameworkPropertyMetadata(RibbonMinimizationButtonVisibility.Auto, new PropertyChangedCallback(OnMinimizationButtonVisibilityPropertyChanged)));
			IsMinimizationButtonVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsMinimizationButtonVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, OnIsMinimizationButtonVisibileChanged, CoereceIsMinimizationButtonVisibleProperty));
			IsMinimizationButtonVisibleProperty = IsMinimizationButtonVisiblePropertyKey.DependencyProperty;
			IsRibbonTitleBarActualVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsRibbonTitleBarActualVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			IsRibbonTitleBarActualVisibleProperty = IsRibbonTitleBarActualVisiblePropertyKey.DependencyProperty;
			HeaderQAT2007ContainerStyleInRibbonWindowProperty = DependencyPropertyManager.Register("HeaderQAT2007ContainerStyleInRibbonWindow", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			HeaderQAT2007ContainerStyleInAeroWindowProperty = DependencyPropertyManager.Register("HeaderQAT2007ContainerStyleInAeroWindow", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			HeaderQAT2007ContainerStyleProperty = DependencyPropertyManager.Register("HeaderQAT2007ContainerStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			HeaderQATContainerStyleWithoutApplicationIconProperty = DependencyPropertyManager.Register("HeaderQATContainerStyleWithoutApplicationIcon", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			HeaderQATContainerStyleWithoutAppIconInAeroWindowProperty = DependencyPropertyManager.Register("HeaderQATContainerStyleWithoutAppIconInAeroWindow", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			HeaderQAT2010ContainerStyleProperty = DependencyPropertyManager.Register("HeaderQAT2010ContainerStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			HeaderQAT2010ContainerStyleInRibbonWindowProperty = DependencyPropertyManager.Register("HeaderQAT2010ContainerStyleInRibbonWindow", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			HeaderQAT2010ContainerStyleInAeroWindowProperty = DependencyPropertyManager.Register("HeaderQAT2010ContainerStyleInAeroWindow", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			IsMergedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsMerged", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsMergedProperty = IsMergedPropertyKey.DependencyProperty;
			CategoriesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Categories", typeof(RibbonPageCategoryCollection), ownerType,
				new FrameworkPropertyMetadata(null));
			CategoriesProperty = CategoriesPropertyKey.DependencyProperty;
			ActualCategoriesPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualCategories", typeof(ObservableCollection<RibbonPageCategoryBase>), ownerType,
				new FrameworkPropertyMetadata(null));
			ActualCategoriesProperty = ActualCategoriesPropertyKey.DependencyProperty;
			ItemsPropertyKey = DependencyPropertyManager.RegisterReadOnly("Items", typeof(ItemCollection), ownerType,
				new FrameworkPropertyMetadata(null));
			ItemsProperty = ItemsPropertyKey.DependencyProperty;
			AeroTemplateProperty = DependencyPropertyManager.Register("AeroTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnAeroTemplatePropertyChanged)));
			DefaultTemplateProperty = DependencyPropertyManager.Register("DefaultTemplate", typeof(ControlTemplate), ownerType, new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnDefaultTemplatePropertyChanged)));
			AeroBorderRibbonStyleProperty = DependencyPropertyManager.Register("AeroBorderRibbonStyle", typeof(RibbonStyle), ownerType, new FrameworkPropertyMetadata(RibbonStyle.Office2007, new PropertyChangedCallback(OnAeroBorderRibbonStylePropertyChanged)));
			AeroBorderTopOffsetProperty = DependencyPropertyManager.Register("AeroBorderTopOffset", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnAeroBorderTopOffsetPropertyChanged)));
			CollapsedRibbonAeroBorderTopOffsetProperty = DependencyPropertyManager.Register("CollapsedRibbonAeroBorderTopOffset", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, new PropertyChangedCallback(OnCollapsedRibbonAeroBorderTopOffsetPropertyChanged)));
			MDIMergeStyleProperty = DependencyPropertyManager.Register("MDIMergeStyle", typeof(MDIMergeStyle), ownerType, new FrameworkPropertyMetadata(MDIMergeStyle.Always, new PropertyChangedCallback(OnMDIMergeStylePropertyChanged)));
			AutoHideModeProperty = DependencyPropertyManager.Register("AutoHideMode", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((RibbonControl)d).OnAutoHideModeChanged((bool)e.OldValue), CoerceAutoHideMode));
			MenuIconStyleProperty = DependencyPropertyManager.Register("MenuIconStyle", typeof(RibbonMenuIconStyle), ownerType, new FrameworkPropertyMetadata(RibbonMenuIconStyle.Mono));
			IsHiddenRibbonCollapsedProperty = DependencyPropertyManager.Register("IsHiddenRibbonCollapsed", typeof(bool), ownerType, new PropertyMetadata(true, OnIsHiddenRibbonCollapsedPropertyChanged, OnIsHiddenRibbonCollapsedPropertyCoerce));
			MergingProperties.HideElementsProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata((d, e) => ((RibbonControl)d).UpdateActualVisibility()));
			VisibilityProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(Visibility.Visible, null, new CoerceValueCallback((d, v) => ((RibbonControl)d).CoerceVisibility(v))));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(ownerType, typeof(RibbonControlAutomationPeer), owner => new RibbonControlAutomationPeer((RibbonControl)owner));
			ItemLinkFactory.Default.CreatorsList.Add(new RibbonObjectCreator());
			RibbonControlCollectionActionHelper.RegisterDefaultGetters();
		}
		static object CoereceIsMinimizationButtonVisibleProperty(DependencyObject d, object baseValue) {
			return ((RibbonControl)d).CoereceIsMinimizationButtonVisible(baseValue);
		}
		static void OnIsMinimizationButtonVisibileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnIsMinimizationButtonVisibleChanged((bool)e.OldValue);
		}
		static void OnIsHiddenRibbonCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnIsHiddenRibbonCollapsedChanged((bool)e.OldValue);
		}
		static object OnIsHiddenRibbonCollapsedPropertyCoerce(DependencyObject d, object baseValue) {
			return ((RibbonControl)d).CoerceIsHiddenRibbonCollapsed(baseValue);
		}
		static void OnPageCategoryAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnPageCategoryAlignmentChanged((RibbonPageCategoryCaptionAlignment)e.OldValue);
		}
		protected virtual void OnPageCategoryAlignmentChanged(RibbonPageCategoryCaptionAlignment ribbonPageCategoryCaptionAlignment) {
			if(TitlePanel != null)
				TitlePanel.InvalidateArrange();
		}
		protected virtual void OnWindowServiceChanged(object oldValue, object newValue) {
			var oldWindow = oldValue as Window;
			var newWindow = newValue as Window;
			if (oldWindow != null) {
				WindowHelper.UnsubscribeRibbonWindowEvents();
			}
			if (newWindow != null) {
				WindowHelper.SubscribeRibbonWindowEvents();
			}
			if (IsInRibbonWindow)
				UpdateToolbarPlacement();
			UpdateActualVisibility();
			UpdateActualApplicationIcon();
		}
		protected internal void UpdateWindow() {
			var window = Window.GetWindow(this);
			OnWindowServiceChanged(window, window);
		}
		private static object CoerceAutoHideMode(DependencyObject d, object baseValue) {
			var ribbon = (RibbonControl)d;
			if(ribbon.IsInRibbonWindow && ribbon.IsAeroMode)
				return false;
			return baseValue;
		}
		public static bool GetAllowAddingToToolbar(DependencyObject obj) {
			return (bool)obj.GetValue(AllowAddingToToolbarProperty);
		}
		public static void SetAllowAddingToToolbar(DependencyObject obj, bool value) {
			obj.SetValue(AllowAddingToToolbarProperty, value);
		}
		protected static object OnIsMinimizedPropertyCoerce(DependencyObject d, object baseValue) {
			return ((RibbonControl)d).OnIsMinimizedCoerce(baseValue);
		}
		protected virtual void OnHideEmptyGroupsChanged(bool oldValue) {
		}
		protected static void OnAllowKeyTipsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnAllowKeyTipsChanged((bool)e.OldValue);
		}
		protected static void OnToolbarItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnToolbarItemLinksSourceChanged(e);
		}
		protected static void OnToolbarItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnToolbarItemLinksTemplateChanged(e);
		}
		protected static void OnToolbarItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnToolbarItemLinksTemplateSelectorChanged(e);
		}
		protected static void OnPageHeaderItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnPageHeaderItemLinksSourceChanged(e);
		}
		protected static void OnPageHeaderItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnPageHeaderItemLinksTemplateChanged(e);
		}
		protected static void OnPageHeaderItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnPageHeaderItemLinksTemplateSelectorChanged(e);
		}
		protected static void OnCategoriesSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnCategoriesSourceChanged(e);
		}
		protected static void OnCategoryTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnCategoryTemplateChanged(e);
		}
		protected static void OnAllowMinimizeRibbonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			d.CoerceValue(RibbonControl.IsMinimizedProperty);
			((RibbonControl)d).OnAllowMinimizeRibbonChanged((bool)e.OldValue);
		}
		protected static void OnIsMinimizedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnIsMinimizedChanged((bool)e.OldValue);
		}
		protected static void OnToolbarShowModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnToolbarShowModeChanged(e);
		}
		protected static void OnSelectedPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnSelectedPageChanged((RibbonPage)e.OldValue);
		}
		protected static void OnSelectedPageNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnSelectedPageNameChanged(e);
		}
		protected static void OnRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnRibbonStyleChanged((RibbonStyle)e.OldValue);
		}
		protected static void OnStandaloneHeaderBorderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnStandaloneHeaderBorderTemplateChanged((ControlTemplate)e.OldValue);
		}
		protected static void OnHeaderBorderTemplateInRibbonWindowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnHeaderBorderTemplateInRibbonWindowChanged((ControlTemplate)e.OldValue);
		}
		protected static void OnApplicationButton2007StylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).UpdateActualApplicationButtonStyle();
		}
		protected static void OnApplicationButton2010StylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).UpdateActualApplicationButtonStyle();
		}
		protected static void OnApplicationButtonTabletOfficeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).UpdateActualApplicationButtonStyle();
		}
		protected static void OnApplicationButtonOfficeSlimStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).UpdateActualApplicationButtonStyle();
		}
		protected static void OnIsMinimizedRibbonCollapsedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnIsMinimizedRibbonCollapsedChanged((bool)e.OldValue);
		}
		protected static void OnIsBackstageViewOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnIsBackstageViewOpenChanged((bool)e.OldValue);
		}
		protected static void OnActualApplicationIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).UpdateApplicationIconVisibility();
		}
		protected static void OnApplicationMenuPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnApplicationMenuChanged(e.OldValue);
		}
		protected static void OnShowApplicationButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnShowApplicationButtonChanged((bool)e.OldValue);
		}
		protected static void OnIsInRibbonWindowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).IsInRibbonWindowChanged((bool)e.OldValue);
		}
		protected static void OnIsAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).IsAeroModeChanged((bool)e.OldValue);
		}
		protected static void OnApplicationButton2007StyleInAeroWindowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnApplicationButton2007StyleInAeroWindowChanged((Style)e.OldValue);
		}
		protected static void OnApplicationButtonLargeIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).UpdateActualApplicationButtonLargeIcon();
		}
		protected static void OnApplicationButtonTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).UpdateActualIsApplicationTextVisible();
		}
		protected static void OnApplicationButtonSmallIconPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).UpdateActualApplicationButtonSmallIcon();
		}
		protected static void OnToolbarShowCustomizationButtonPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnToolbarShowCustomizationButtonChanged((bool)e.OldValue);
		}
		protected static void OnRibbonTitleBarVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnRibbonTitleBarVisibilityChanged((RibbonTitleBarVisibility)e.OldValue);
		}
		protected static void OnRibbonHeaderVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnRibbonHeaderVisibilityChanged((RibbonHeaderVisibility)e.OldValue);
		}
		protected static void OnMinimizationButtonVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnMinimizationButtonVisibilityChanged((RibbonMinimizationButtonVisibility)e.OldValue);
		}
		protected static void OnMDIMergeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnMDIMergeStyleChanged((MDIMergeStyle)e.OldValue);
		}
		protected static void OnAeroTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnAeroTemplateChanged(e.OldValue as ControlTemplate);
		}
		protected static void OnDefaultTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnDefaultTemplateChanged(e.OldValue as ControlTemplate);
		}
		protected static void OnAeroBorderRibbonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnAeroBorderRibbonStyleChanged((RibbonStyle)e.OldValue);
		}
		protected static void OnAeroBorderTopOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnAeroBorderTopOffsetChanged((double)e.OldValue);
		}
		protected static void OnCollapsedRibbonAeroBorderTopOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonControl)d).OnCollapsedRibbonAeroBorderTopOffsetChanged((double)e.OldValue);
		}		
		protected virtual void OnAutoHideModeChanged(bool oldValue) {
			if (AutoHideMode) {
				SetValue(IsBackStageViewOpenPropertyKey, false);
			}
			CoerceValue(IsHiddenRibbonCollapsedProperty);
			CoerceValue(IsMinimizedProperty);
			CoerceValue(IsMinimizationButtonVisibleProperty);
			RaisePropertyChanged(autoHideModeChangedEventHandler, new RibbonPropertyChangedEventArgs(oldValue, AutoHideMode));
		}
		#endregion
		#region dep props
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlHeaderBorderTemplateInRibbonWindow"),
#endif
 Browsable(false)]
		public ControlTemplate HeaderBorderTemplateInRibbonWindow {
			get { return (ControlTemplate)GetValue(HeaderBorderTemplateInRibbonWindowProperty); }
			set { SetValue(HeaderBorderTemplateInRibbonWindowProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlSelectedPagePopupMargin"),
#endif
 Browsable(false)]
		public Thickness SelectedPagePopupMargin {
			get { return (Thickness)GetValue(SelectedPagePopupMarginProperty); }
			set { SetValue(SelectedPagePopupMarginProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlStandaloneHeaderBorderTemplate"),
#endif
 Browsable(false)]
		public ControlTemplate StandaloneHeaderBorderTemplate {
			get { return (ControlTemplate)GetValue(StandaloneHeaderBorderTemplateProperty); }
			set { SetValue(StandaloneHeaderBorderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlIsMinimizedRibbonCollapsed"),
#endif
 Browsable(false)]
		public bool IsMinimizedRibbonCollapsed {
			get { return (bool)GetValue(IsMinimizedRibbonCollapsedProperty); }
			protected internal set { this.SetValue(IsMinimizedRibbonCollapsedPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlIsBackStageViewOpen"),
#endif
 Browsable(false)]
		public bool IsBackStageViewOpen {
			get { return (bool)GetValue(IsBackStageViewOpenProperty); }
			protected set { this.SetValue(IsBackStageViewOpenPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlPageCaptionMinWidth")]
#endif
		public double PageCaptionMinWidth {
			get { return (double)GetValue(PageCaptionMinWidthProperty); }
			set { SetValue(PageCaptionMinWidthProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlMaxPageCaptionTextIndent")]
#endif
		public double MaxPageCaptionTextIndent {
			get { return (double)GetValue(MaxPageCaptionTextIndentProperty); }
			set { SetValue(MaxPageCaptionTextIndentProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlPageCategoryAlignment")]
#endif
		public RibbonPageCategoryCaptionAlignment PageCategoryAlignment {
			get { return (RibbonPageCategoryCaptionAlignment)GetValue(PageCategoryAlignmentProperty); }
			set { SetValue(PageCategoryAlignmentProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlShowApplicationButton")]
#endif
		public bool ShowApplicationButton {
			get { return (bool)GetValue(ShowApplicationButtonProperty); }
			set { SetValue(ShowApplicationButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButtonLargeIcon")]
#endif
		public ImageSource ApplicationButtonLargeIcon {
			get { return (ImageSource)GetValue(ApplicationButtonLargeIconProperty); }
			set { SetValue(ApplicationButtonLargeIconProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlActualApplicationButtonLargeIcon")]
#endif
		public ImageSource ActualApplicationButtonLargeIcon {
			get { return (ImageSource)GetValue(ActualApplicationButtonLargeIconProperty); }
			protected set { this.SetValue(ActualApplicationButtonLargeIconPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButtonSmallIcon")]
#endif
		public ImageSource ApplicationButtonSmallIcon {
			get { return (ImageSource)GetValue(ApplicationButtonSmallIconProperty); }
			set { SetValue(ApplicationButtonSmallIconProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlActualApplicationButtonSmallIcon")]
#endif
		public ImageSource ActualApplicationButtonSmallIcon {
			get { return (ImageSource)GetValue(ActualApplicationButtonSmallIconProperty); }
			protected set { this.SetValue(ActualApplicationButtonSmallIconPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButtonText")]
#endif
		public string ApplicationButtonText {
			get { return (string)GetValue(ApplicationButtonTextProperty); }
			set { SetValue(ApplicationButtonTextProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationMenu")]
#endif
		public DependencyObject ApplicationMenu {
			get { return (DependencyObject)GetValue(ApplicationMenuProperty); }
			set { SetValue(ApplicationMenuProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlButtonGroupVertAlign"),
#endif
 Browsable(false)]
		public VerticalAlignment ButtonGroupVertAlign {
			get { return (VerticalAlignment)GetValue(ButtonGroupVertAlignProperty); }
			set { SetValue(ButtonGroupVertAlignProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlColumnIndent")]
#endif
		public double ColumnIndent {
			get { return (double)GetValue(ColumnIndentProperty); }
			set { SetValue(ColumnIndentProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlRowIndent")]
#endif
		public double RowIndent {
			get { return (double)GetValue(RowIndentProperty); }
			set { SetValue(RowIndentProperty, value); }
		}
		public bool AsyncMergingEnabled {
			get { return (bool)GetValue(AsyncMergingEnabledProperty); }
			set { SetValue(AsyncMergingEnabledProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlButtonGroupsIndent")]
#endif
		public double ButtonGroupsIndent {
			get { return (double)GetValue(ButtonGroupsIndentProperty); }
			set { SetValue(ButtonGroupsIndentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlToolbarShowMode"),
#endif
 XtraSerializableProperty]
		public RibbonQuickAccessToolbarShowMode ToolbarShowMode {
			get { return (RibbonQuickAccessToolbarShowMode)GetValue(ToolbarShowModeProperty); }
			set { SetValue(ToolbarShowModeProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlSelectedPage")]
#endif
		public RibbonPage SelectedPage {
			get { return (RibbonPage)GetValue(SelectedPageProperty); }
			set { SetValue(SelectedPageProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlSelectedPageName")]
#endif
		public string SelectedPageName {
			get { return (string)GetValue(SelectedPageNameProperty); }
			set { SetValue(SelectedPageNameProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlAllowMinimizeRibbon")]
#endif
		public bool AllowMinimizeRibbon {
			get { return (bool)GetValue(AllowMinimizeRibbonProperty); }
			set { SetValue(AllowMinimizeRibbonProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlIsMinimized"),
#endif
 XtraSerializableProperty]
		public bool IsMinimized {
			get { return (bool)GetValue(IsMinimizedProperty); }
			set { SetValue(IsMinimizedProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlRibbonStyle")]
#endif
		public RibbonStyle RibbonStyle {
			get { return (RibbonStyle)GetValue(RibbonStyleProperty); }
			set { SetValue(RibbonStyleProperty, value); }
		}
		public object CategoriesSource {
			get { return GetValue(CategoriesSourceProperty); }
			set { SetValue(CategoriesSourceProperty, value); }
		}
		public DataTemplate CategoryTemplate {
			get { return (DataTemplate)GetValue(CategoryTemplateProperty); }
			set { SetValue(CategoryTemplateProperty, value); }
		}
		public DataTemplateSelector CategoryTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CategoryTemplateSelectorProperty); }
			set { SetValue(CategoryTemplateSelectorProperty, value); }
		}
		public Style CategoryStyle {
			get { return (Style)GetValue(CategoryStyleProperty); }
			set { SetValue(CategoryStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlKeyTipApplicationButton")]
#endif
		public string KeyTipApplicationButton {
			get { return (string)GetValue(KeyTipApplicationButtonProperty); }
			set { SetValue(KeyTipApplicationButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlIsHeaderBorderVisible")]
#endif
		public bool IsHeaderBorderVisible {
			get { return (bool)GetValue(IsHeaderBorderVisibleProperty); }
			set { SetValue(IsHeaderBorderVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlIsInRibbonWindow")]
#endif
		public bool IsInRibbonWindow {
			get { return (bool)GetValue(IsInRibbonWindowProperty); }
			protected set { this.SetValue(IsInRibbonWindowPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationMenuHorizontalOffsetInAeroWindow"),
#endif
 Browsable(false)]
		public double ApplicationMenuHorizontalOffsetInAeroWindow {
			get { return (double)GetValue(ApplicationMenuHorizontalOffsetInAeroWindowProperty); }
			set { SetValue(ApplicationMenuHorizontalOffsetInAeroWindowProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationMenuVerticalOffsetInAeroWindow"),
#endif
 Browsable(false)]
		public double ApplicationMenuVerticalOffsetInAeroWindow {
			get { return (double)GetValue(ApplicationMenuVerticalOffsetInAeroWindowProperty); }
			set { SetValue(ApplicationMenuVerticalOffsetInAeroWindowProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlActualHeaderBorderTemplate")]
#endif
		public ControlTemplate ActualHeaderBorderTemplate {
			get { return (ControlTemplate)GetValue(ActualHeaderBorderTemplateProperty); }
			set { SetValue(ActualHeaderBorderTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlNormalPageCaptionTextStyle")]
#endif
		public Style NormalPageCaptionTextStyle {
			get { return (Style)GetValue(NormalPageCaptionTextStyleProperty); }
			set { SetValue(NormalPageCaptionTextStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlHoverPageCaptionTextStyle")]
#endif
		public Style HoverPageCaptionTextStyle {
			get { return (Style)GetValue(HoverPageCaptionTextStyleProperty); }
			set { SetValue(HoverPageCaptionTextStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlGroupCaptionTextStyle"),
#endif
 Browsable(false)]
		public Style GroupCaptionTextStyle {
			get { return (Style)GetValue(GroupCaptionTextStyleProperty); }
			set { SetValue(GroupCaptionTextStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlSelectedPageCaptionTextStyle"),
#endif
 Browsable(false)]
		public Style SelectedPageCaptionTextStyle {
			get { return (Style)GetValue(SelectedPageCaptionTextStyleProperty); }
			set { SetValue(SelectedPageCaptionTextStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationMenuHorizontalOffset"),
#endif
 Browsable(false)]
		public double ApplicationMenuHorizontalOffset {
			get { return (double)GetValue(ApplicationMenuHorizontalOffsetProperty); }
			set { SetValue(ApplicationMenuHorizontalOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationMenuVerticalOffset"),
#endif
 Browsable(false)]
		public double ApplicationMenuVerticalOffset {
			get { return (double)GetValue(ApplicationMenuVerticalOffsetProperty); }
			set { SetValue(ApplicationMenuVerticalOffsetProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlIsOffice2010Style")]
#endif
		public bool IsOffice2010Style {
			get { return (bool)GetValue(IsOffice2010StyleProperty); }
			protected set { this.SetValue(IsOffice2010StylePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlActualApplicationButtonStyle"),
#endif
 Browsable(false)]
		public Style ActualApplicationButtonStyle {
			get { return (Style)GetValue(ActualApplicationButtonStyleProperty); }
			protected set { this.SetValue(ActualApplicationButtonStylePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButton2007Style"),
#endif
 Browsable(false)]
		public Style ApplicationButton2007Style {
			get { return (Style)GetValue(ApplicationButton2007StyleProperty); }
			set { SetValue(ApplicationButton2007StyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButton2010Style"),
#endif
 Browsable(false)]
		public Style ApplicationButton2010Style {
			get { return (Style)GetValue(ApplicationButton2010StyleProperty); }
			set { SetValue(ApplicationButton2010StyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButton2007StyleInPopup"),
#endif
 Browsable(false)]
		public Style ApplicationButton2007StyleInPopup {
			get { return (Style)GetValue(ApplicationButton2007StyleInPopupProperty); }
			set { SetValue(ApplicationButton2007StyleInPopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButton2010StyleInPopup"),
#endif
 Browsable(false)]
		public Style ApplicationButton2010StyleInPopup {
			get { return (Style)GetValue(ApplicationButton2010StyleInPopupProperty); }
			set { SetValue(ApplicationButton2010StyleInPopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButtonTabletOfficeStyle"),
#endif
 Browsable(false)]
		public Style ApplicationButtonTabletOfficeStyle {
			get { return (Style)GetValue(ApplicationButtonTabletOfficeStyleProperty); }
			set { SetValue(ApplicationButtonTabletOfficeStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButtonOfficeSlimStyle"),
#endif
 Browsable(false)]
		public Style ApplicationButtonOfficeSlimStyle {
			get { return (Style)GetValue(ApplicationButtonOfficeSlimStyleProperty); }
			set { SetValue(ApplicationButtonOfficeSlimStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButtonTabletOfficeStyleInPopup"),
#endif
 Browsable(false)]
		public Style ApplicationButtonTabletOfficeStyleInPopup {
			get { return (Style)GetValue(ApplicationButtonTabletOfficeStyleInPopupProperty); }
			set { SetValue(ApplicationButtonTabletOfficeStyleInPopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButtonOfficeSlimStyleInPopup"),
#endif
 Browsable(false)]
		public Style ApplicationButtonOfficeSlimStyleInPopup {
			get { return (Style)GetValue(ApplicationButtonOfficeSlimStyleInPopupProperty); }
			set { SetValue(ApplicationButtonOfficeSlimStyleInPopupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButtonPopupHorizontalOffset"),
#endif
 Browsable(false)]
		public double ApplicationButtonPopupHorizontalOffset {
			get { return (double)GetValue(ApplicationButtonPopupHorizontalOffsetProperty); }
			set { SetValue(ApplicationButtonPopupHorizontalOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButtonPopupVerticalOffset"),
#endif
 Browsable(false)]
		public double ApplicationButtonPopupVerticalOffset {
			get { return (double)GetValue(ApplicationButtonPopupVerticalOffsetProperty); }
			set { SetValue(ApplicationButtonPopupVerticalOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButton2010PopupHorizontalOffset"),
#endif
 Browsable(false)]
		public double ApplicationButton2010PopupHorizontalOffset {
			get { return (double)GetValue(ApplicationButton2010PopupHorizontalOffsetProperty); }
			set { SetValue(ApplicationButton2010PopupHorizontalOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlApplicationButton2010PopupVerticalOffset"),
#endif
 Browsable(false)]
		public double ApplicationButton2010PopupVerticalOffset {
			get { return (double)GetValue(ApplicationButton2010PopupVerticalOffsetProperty); }
			set { SetValue(ApplicationButton2010PopupVerticalOffsetProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlActualApplicationButtonWidth")]
#endif
		public double ActualApplicationButtonWidth {
			get { return (double)GetValue(ActualApplicationButtonWidthProperty); }
			protected set { this.SetValue(ActualApplicationButtonWidthPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlActualIsApplicationButtonTextVisible")]
#endif
		public bool ActualIsApplicationButtonTextVisible {
			get { return (bool)GetValue(ActualIsApplicationButtonTextVisibleProperty); }
			protected set { this.SetValue(ActualIsApplicationButtonTextVisiblePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlActualApplicationIcon")]
#endif
		public ImageSource ActualApplicationIcon {
			get { return (ImageSource)GetValue(ActualApplicationIconProperty); }
			protected set { this.SetValue(ActualApplicationIconPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlIsApplicationIconVisible")]
#endif
		public bool IsApplicationIconVisible {
			get { return (bool)GetValue(IsApplicationIconVisibleProperty); }
			protected set { this.SetValue(IsApplicationIconVisiblePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlActualHeaderQuickAccessToolbarContainerStyle")]
#endif
		public Style ActualHeaderQuickAccessToolbarContainerStyle {
			get { return (Style)GetValue(ActualHeaderQuickAccessToolbarContainerStyleProperty); }
			protected internal set { this.SetValue(ActualHeaderQuickAccessToolbarContainerStylePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlActualWindowTitle")]
#endif
		public string ActualWindowTitle {
			get { return (string)GetValue(ActualWindowTitleProperty); }
			protected set { this.SetValue(ActualWindowTitlePropertyKey, value); }
		}
		public Style ApplicationButton2007StyleInAeroWindow {
			get { return (Style)GetValue(ApplicationButton2007StyleInAeroWindowProperty); }
			set { SetValue(ApplicationButton2007StyleInAeroWindowProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlAllowKeyTips")]
#endif
		public bool AllowKeyTips {
			get { return (bool)GetValue(AllowKeyTipsProperty); }
			set { SetValue(AllowKeyTipsProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlToolbarShowCustomizationButton")]
#endif
		public bool ToolbarShowCustomizationButton {
			get { return (bool)GetValue(ToolbarShowCustomizationButtonProperty); }
			set { SetValue(ToolbarShowCustomizationButtonProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlRibbonHeaderVisibility")]
#endif
		public RibbonHeaderVisibility RibbonHeaderVisibility {
			get { return (RibbonHeaderVisibility)GetValue(RibbonHeaderVisibilityProperty); }
			set { SetValue(RibbonHeaderVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlRibbonTitleBarVisibility")]
#endif
		public RibbonTitleBarVisibility RibbonTitleBarVisibility {
			get { return (RibbonTitleBarVisibility)GetValue(RibbonTitleBarVisibilityProperty); }
			set { SetValue(RibbonTitleBarVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlMinimizationButtonVisibility")]
#endif
		public RibbonMinimizationButtonVisibility MinimizationButtonVisibility {
			get { return (RibbonMinimizationButtonVisibility)GetValue(MinimizationButtonVisibilityProperty); }
			set { SetValue(MinimizationButtonVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlIsMinimizationButtonVisible")]
#endif
		public bool IsMinimizationButtonVisible {
			get { return (bool)GetValue(IsMinimizationButtonVisibleProperty); }
			protected set { this.SetValue(IsMinimizationButtonVisiblePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlIsRibbonTitleBarActualVisible")]
#endif
		public bool IsRibbonTitleBarActualVisible {
			get { return (bool)GetValue(IsRibbonTitleBarActualVisibleProperty); }
			protected set { this.SetValue(IsRibbonTitleBarActualVisiblePropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlToolbarItemLinksSource")]
#endif
		public IEnumerable ToolbarItemLinksSource {
			get { return (IEnumerable)GetValue(ToolbarItemLinksSourceProperty); }
			set { SetValue(ToolbarItemLinksSourceProperty, value); }
		}
		public bool ItemLinksSourceElementGeneratesUniqueBarItem {
			get { return (bool)GetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty); }
			set { SetValue(ItemLinksSourceElementGeneratesUniqueBarItemProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlToolbarItemTemplate")]
#endif
		public DataTemplate ToolbarItemTemplate {
			get { return (DataTemplate)GetValue(ToolbarItemTemplateProperty); }
			set { SetValue(ToolbarItemTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlToolbarItemTemplateSelector")]
#endif
		public DataTemplateSelector ToolbarItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ToolbarItemTemplateSelectorProperty); }
			set { SetValue(ToolbarItemTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlToolbarItemStyle")]
#endif
		public Style ToolbarItemStyle {
			get { return (Style)GetValue(ToolbarItemStyleProperty); }
			set { SetValue(ToolbarItemStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlPageHeaderItemLinksSource")]
#endif
		public IEnumerable PageHeaderItemLinksSource {
			get { return (IEnumerable)GetValue(PageHeaderItemLinksSourceProperty); }
			set { SetValue(PageHeaderItemLinksSourceProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlPageHeaderItemTemplate")]
#endif
		public DataTemplate PageHeaderItemTemplate {
			get { return (DataTemplate)GetValue(PageHeaderItemTemplateProperty); }
			set { SetValue(PageHeaderItemTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlPageHeaderItemTemplateSelector")]
#endif
		public DataTemplateSelector PageHeaderItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PageHeaderItemTemplateSelectorProperty); }
			set { SetValue(PageHeaderItemTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlPageHeaderItemStyle")]
#endif
		public Style PageHeaderItemStyle {
			get { return (Style)GetValue(PageHeaderItemStyleProperty); }
			set { SetValue(PageHeaderItemStyleProperty, value); }
		}
		[Browsable(false)]
		public Style HeaderQAT2007ContainerStyleInAeroWindow {
			get { return (Style)GetValue(HeaderQAT2007ContainerStyleInAeroWindowProperty); }
			set { SetValue(HeaderQAT2007ContainerStyleInAeroWindowProperty, value); }
		}
		[Browsable(false)]
		public Style HeaderQAT2007ContainerStyleInRibbonWindow {
			get { return (Style)GetValue(HeaderQAT2007ContainerStyleInRibbonWindowProperty); }
			set { SetValue(HeaderQAT2007ContainerStyleInRibbonWindowProperty, value); }
		}
		[Browsable(false)]
		public Style HeaderQAT2007ContainerStyle {
			get { return (Style)GetValue(HeaderQAT2007ContainerStyleProperty); }
			set { SetValue(HeaderQAT2007ContainerStyleProperty, value); }
		}
		[Browsable(false)]
		public Style HeaderQATContainerStyleWithoutApplicationIcon {
			get { return (Style)GetValue(HeaderQATContainerStyleWithoutApplicationIconProperty); }
			set { SetValue(HeaderQATContainerStyleWithoutApplicationIconProperty, value); }
		}
		[Browsable(false)]
		public Style HeaderQATContainerStyleWithoutAppIconInAeroWindow {
			get { return (Style)GetValue(HeaderQATContainerStyleWithoutAppIconInAeroWindowProperty); }
			set { SetValue(HeaderQATContainerStyleWithoutAppIconInAeroWindowProperty, value); }
		}
		[Browsable(false)]
		public Style HeaderQAT2010ContainerStyle {
			get { return (Style)GetValue(HeaderQAT2010ContainerStyleProperty); }
			set { SetValue(HeaderQAT2010ContainerStyleProperty, value); }
		}
		[Browsable(false)]
		public Style HeaderQAT2010ContainerStyleInRibbonWindow {
			get { return (Style)GetValue(HeaderQAT2010ContainerStyleInRibbonWindowProperty); }
			set { SetValue(HeaderQAT2010ContainerStyleInRibbonWindowProperty, value); }
		}
		[Browsable(false)]
		public Style HeaderQAT2010ContainerStyleInAeroWindow {
			get { return (Style)GetValue(HeaderQAT2010ContainerStyleInAeroWindowProperty); }
			set { SetValue(HeaderQAT2010ContainerStyleInAeroWindowProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlIsMerged")]
#endif
		public bool IsMerged {
			get { return (bool)GetValue(IsMergedProperty); }
			protected internal set { this.SetValue(IsMergedPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlIsHiddenRibbonCollapsed")]
#endif
		public bool IsHiddenRibbonCollapsed {
			get { return (bool)GetValue(IsHiddenRibbonCollapsedProperty); }
			set { SetValue(IsHiddenRibbonCollapsedProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlCategories")]
#endif
		public RibbonPageCategoryCollection Categories {
			get { return (RibbonPageCategoryCollection)GetValue(CategoriesProperty); }
			protected internal set { this.SetValue(CategoriesPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlActualCategories")]
#endif
		public ObservableCollection<RibbonPageCategoryBase> ActualCategories {
			get { return (ObservableCollection<RibbonPageCategoryBase>)GetValue(ActualCategoriesProperty); }
			protected internal set { this.SetValue(ActualCategoriesPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlItems")]
#endif
		[Obsolete("Use RibbonControl.Categories property instead")]
		public ItemCollection Items {
			get { return (ItemCollection)GetValue(ItemsProperty); }
			protected internal set { this.SetValue(ItemsPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlIsAeroMode")]
#endif
		public bool IsAeroMode {
			get { return (bool)GetValue(IsAeroModeProperty); }
			protected internal set { this.SetValue(IsAeroModePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlAeroTemplate"),
#endif
 Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ControlTemplate AeroTemplate {
			get { return (ControlTemplate)GetValue(AeroTemplateProperty); }
			set { this.SetValue(AeroTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlAeroTemplate"),
#endif
 Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ControlTemplate DefaultTemplate {
			get { return (ControlTemplate)GetValue(DefaultTemplateProperty); }
			set { this.SetValue(DefaultTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlAeroTemplate"),
#endif
 Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RibbonStyle AeroBorderRibbonStyle {
			get { return (RibbonStyle)GetValue(AeroBorderRibbonStyleProperty); }
			set { this.SetValue(AeroBorderRibbonStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlAeroBorderTopOffset"),
#endif
 Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public double AeroBorderTopOffset {
			get { return (double)GetValue(AeroBorderTopOffsetProperty); }
			set { this.SetValue(AeroBorderTopOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlCollapsedRibbonAeroBorderTopOffset"),
#endif
 EditorBrowsable(EditorBrowsableState.Never)]
		public double CollapsedRibbonAeroBorderTopOffset {
			get { return (double)GetValue(CollapsedRibbonAeroBorderTopOffsetProperty); }
			set { this.SetValue(CollapsedRibbonAeroBorderTopOffsetProperty, value); }
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlAllowCustomizingDefaultGroups")]
#endif
		public bool AllowCustomizingDefaultGroups {
			get { return (bool)GetValue(AllowCustomizingDefaultGroupsProperty); }
			set { SetValue(AllowCustomizingDefaultGroupsProperty, value); }
		}
		public static bool UseCaptionToIdentifyPagesOnMerging { get; set; }
		public static RibbonControl GetRibbon(DependencyObject obj) {
			return (RibbonControl)obj.GetValue(RibbonProperty);
		}
		public static void SetRibbon(DependencyObject obj, RibbonControl value) {
			obj.SetValue(RibbonProperty, value);
		}
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlAllowCustomization")]
#endif
		public bool AllowCustomization {
			get { return (bool)GetValue(AllowCustomizationProperty); }
			set { SetValue(AllowCustomizationProperty, value); }
		}
		public bool AllowRibbonNavigationFromEditorOnTabPress {
			get { return (bool)GetValue(AllowRibbonNavigationFromEditorOnTabPressProperty); }
			set { SetValue(AllowRibbonNavigationFromEditorOnTabPressProperty, value); }
		}
		public SelectedPageOnMerging SelectedPageOnMerging {
			get { return (SelectedPageOnMerging)GetValue(SelectedPageOnMergingProperty); }
			set { SetValue(SelectedPageOnMergingProperty, value); }
		}
		public MDIMergeStyle MDIMergeStyle {
			get { return (MDIMergeStyle)GetValue(MDIMergeStyleProperty); }
			set { SetValue(MDIMergeStyleProperty, value); }
		}
		public bool HideEmptyGroups {
			get { return (bool)GetValue(HideEmptyGroupsProperty); }
			set { SetValue(HideEmptyGroupsProperty, value); }
		}
		public bool AutoHideMode {
			get { return (bool)GetValue(AutoHideModeProperty); }
			set { SetValue(AutoHideModeProperty, value); }
		}
		public RibbonMenuIconStyle MenuIconStyle {
			get { return (RibbonMenuIconStyle)GetValue(MenuIconStyleProperty); }
			set { SetValue(MenuIconStyleProperty, value); }
		}
		#endregion
		#region menuItems
		List<BarItem> menuItems = new List<BarItem>();
		RibbonPopupMenuButtonItem showToolbarBelowRibbonMenuItemCore;
		protected RibbonPopupMenuButtonItem ShowToolbarBelowRibbonMenuItem {
			get {
				if (showToolbarBelowRibbonMenuItemCore == null) {
					showToolbarBelowRibbonMenuItemCore = new RibbonPopupMenuButtonItem() { Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonPopupMenuItemText_ShowQuickAccessToolbarBelowTheRibbon) };
					showToolbarBelowRibbonMenuItemCore.ItemClick += new ItemClickEventHandler(OnShowToolbarBelowRibbonMenuItemClick);
					menuItems.Add(showToolbarBelowRibbonMenuItemCore);
				}
				return showToolbarBelowRibbonMenuItemCore;
			}
		}
		RibbonPopupMenuButtonItem showToolbarAboveRibbonMenuItemCore;
		protected RibbonPopupMenuButtonItem ShowToolbarAboveRibbonMenuItem {
			get {
				if (showToolbarAboveRibbonMenuItemCore == null) {
					showToolbarAboveRibbonMenuItemCore = new RibbonPopupMenuButtonItem() { Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonPopupMenuItemText_ShowQuickAccessToolbarAboveTheRibbon) };
					showToolbarAboveRibbonMenuItemCore.ItemClick += new ItemClickEventHandler(OnShowToolbarAboveRibbonMenuItemClick);
					menuItems.Add(showToolbarAboveRibbonMenuItemCore);
				}
				return showToolbarAboveRibbonMenuItemCore;
			}
		}
		BarCheckItem minimizeRibbonMenuItemCore;
		protected BarCheckItem MinimizeRibbonMenuItem {
			get {
				if (minimizeRibbonMenuItemCore == null) {
					minimizeRibbonMenuItemCore = new BarCheckItem() { Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonPopupMenuItemText_MinimizeRibbon) };
					minimizeRibbonMenuItemCore.CheckedChanged += new ItemClickEventHandler(OnMinimizeRibbonMenuItemCheckedChanged);
					menuItems.Add(minimizeRibbonMenuItemCore);
				}
				return minimizeRibbonMenuItemCore;
			}
		}
		RibbonPopupMenuButtonItem removeFromToolbarMenuItemCore;
		protected internal RibbonPopupMenuButtonItem RemoveFromToolbarMenuItem {
			get {
				if (removeFromToolbarMenuItemCore == null) {
					removeFromToolbarMenuItemCore = new RibbonPopupMenuButtonItem() { Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonPopupMenuItemText_RemoveFromToolbar) };
					removeFromToolbarMenuItemCore.ItemClick += new ItemClickEventHandler(OnRemoveFromToolbarMenuItemClick);
					menuItems.Add(removeFromToolbarMenuItemCore);
				}
				return removeFromToolbarMenuItemCore;
			}
		}
		RibbonPopupMenuButtonItem customizeRibbonMenuItemCore;
		protected RibbonPopupMenuButtonItem CustomizeRibbonMenuItem {
			get {
				if (customizeRibbonMenuItemCore == null) {
					customizeRibbonMenuItemCore = new RibbonPopupMenuButtonItem() { Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonPopupMenuItemText_CustomizeRibbonMenuItem) };
					customizeRibbonMenuItemCore.ItemClick += new ItemClickEventHandler(OnCustomizeRibbonMenuItemClick);
					menuItems.Add(customizeRibbonMenuItemCore);
				}
				return customizeRibbonMenuItemCore;
			}
		}
		RibbonPopupMenuButtonItem addToToolbarMenuItemCore;
		protected internal RibbonPopupMenuButtonItem AddToToolbarMenuItem {
			get {
				if (addToToolbarMenuItemCore == null) {
					addToToolbarMenuItemCore = new RibbonPopupMenuButtonItem() { Content = RibbonControlLocalizer.GetString(RibbonControlStringId.RibbonPopupMenuItemText_AddToToolbar) };
					addToToolbarMenuItemCore.ItemClick += new ItemClickEventHandler(OnAddToToolbarMenuItemClick);
					menuItems.Add(addToToolbarMenuItemCore);
				}
				return addToToolbarMenuItemCore;
			}
		}
		void OnShowToolbarAboveRibbonMenuItemClick(object sender, ItemClickEventArgs e) {
			this.SetCurrentValue(RibbonControl.ToolbarShowModeProperty, RibbonQuickAccessToolbarShowMode.ShowAbove);
		}
		void OnShowToolbarBelowRibbonMenuItemClick(object sender, ItemClickEventArgs e) {
			this.SetCurrentValue(RibbonControl.ToolbarShowModeProperty, RibbonQuickAccessToolbarShowMode.ShowBelow);
		}
		void OnMinimizeRibbonMenuItemCheckedChanged(object sender, ItemClickEventArgs e) {
			IsMinimized = MinimizeRibbonMenuItem.IsChecked == true;
		}
		void OnRemoveFromToolbarMenuItemClick(object sender, ItemClickEventArgs e) {
			if (RemoveFromToolbarMenuItem.TargetItemLink != null && !(RemoveFromToolbarMenuItem.TargetItemLink is BarItemLink))
				return;
			BarItemLink itemLinkBase = RemoveFromToolbarMenuItem.TargetItemLink as BarItemLink;
			if (itemLinkBase != null) {
				BarItem item = null;
				RibbonQuickAccessToolbar toolbarToRemoveFrom = Toolbar;
				if (IsMerged) {
					if (itemLinkBase is BarItemLink) {
						BarItemLink itemLink = itemLinkBase as BarItemLink;
						item = itemLink.Item;
					}
				}
				if (item != null && toolbarToRemoveFrom.ItemLinks.Count(i => (i as BarItemLink).Item == item) != 0)
					itemLinkBase = toolbarToRemoveFrom.ItemLinks.First(i => (i as BarItemLink).Item == item) as BarItemLink;
				if (itemLinkBase.CommonBarItemCollectionLink)
					new RuntimePropertyCustomization(itemLinkBase) { PropertyName = "IsVisible", NewValue = false, OldValue = true }.Do(RuntimeCustomizations.Add).Apply();
				else
					new RuntimeRemoveLinkCustomization(itemLinkBase).Do(RuntimeCustomizations.Add).Apply();
				Toolbar.Remerge();
			}
		}
		void OnAddToToolbarMenuItemClick(object sender, ItemClickEventArgs e) {
			var item = AddToToolbarMenuItem.TargetItem;
			var link = AddToToolbarMenuItem.TargetItemLink;
			if (item != null) {
				bool isLinkAdded = false;
				if (IsMerged) {
					RibbonControl childRibbon = link.With(GetRibbon) ?? item.With(GetRibbon);
					if (childRibbon != null) {
						new RuntimeCopyLinkCustomization(item, childRibbon.Toolbar, null, true).Do(RuntimeCustomizations.Add).Apply();
						isLinkAdded = true;
					}
					if (!isLinkAdded)
						new RuntimeCopyLinkCustomization(item, Toolbar, null, true).Do(RuntimeCustomizations.Add).Apply();
					Toolbar.Remerge();
					return;
				}
				var currentLink = Toolbar.ItemLinks.OfType<BarItemLink>().FirstOrDefault(x => x.Item == item);
				if (currentLink != null && currentLink.CommonBarItemCollectionLink && !currentLink.IsVisible)
					new RuntimePropertyCustomization(currentLink) { PropertyName = "IsVisible", NewValue = true, OldValue = false }.Do(RuntimeCustomizations.Add).Apply();
				else
					new RuntimeCopyLinkCustomization(item, Toolbar, null, true).Do(RuntimeCustomizations.Add).Apply();
			}
		}
		void OnCustomizeRibbonMenuItemClick(object sender, ItemClickEventArgs e) {
			CustomizationHelper.IsCustomizationMode = true;
		}
		protected internal virtual bool IsMenuItem(BarItem item) {
			if (menuItems == null) return false;
			if (menuItems.Contains(item)) return true;
			return false;
		}
		void AddRibbonPopupMenuItem(BarItem item) {
			RibbonPopupMenu.Items.Add(item);
		}
		void RemoveRibbonPopupMenuItem(BarItem item) {
			RibbonPopupMenu.Items.Remove(item);
		}
		#endregion
		#region controllers
		[
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlControllers"),
#endif
 EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public ObservableCollection<IControllerAction> Controllers {
			get { return ControllerBehavior.Actions; }
		}
		ControllerBehavior controllerBehavior;
		protected internal ControllerBehavior ControllerBehavior {
			get { return controllerBehavior ?? (controllerBehavior = CreateControllerBehavior()); }
		}
		protected virtual ControllerBehavior CreateControllerBehavior() {
			ControllerBehavior behavior = new ControllerBehavior();
			behavior.Attach(this);
			return behavior;
		}
		protected virtual RibbonItemCollection CreateItems() {
			return new RibbonItemCollection(this);
		}		
		#endregion
		public bool LoadPagesInBackground { get; set; }
		protected virtual void UpdateActualVisibility() {
			CoerceValue(VisibilityProperty);
		}
		protected virtual object CoerceVisibility(object value) {
			if (MergingProperties.GetHideElements(this) && MDIMergeStyle != Bars.MDIMergeStyle.Never && HasMergingCandidates())
				return Visibility.Collapsed;
			var hideInMDIHost = MDIChildHost != null && !MDIChildHost.IsChildMenuVisible && MDIMergeStyle != Bars.MDIMergeStyle.Never;
			if (!IsInRibbonWindow && hideInMDIHost || MergedParent != null)
				return Visibility.Collapsed;
			return value;
		}
		bool HasMergingCandidates() {
			object mergingRegistratorName = ((IMultipleElementRegistratorSupport)this).GetName(typeof(IMergingSupport));
			return BarNameScope.GetService<IElementRegistratorService>(this).GetElements<IMergingSupport>(mergingRegistratorName, ScopeSearchSettings.Ancestors).Where(x => BarNameScope.GetService<IMergingService>(x).CanMerge(x, this)).Any();
		}
		bool IsFirstLayoutUptated { get; set; }
		RibbonWindowHelper windowHelper;
		internal RibbonWindowHelper WindowHelper {
			get {
				if (windowHelper == null) {
					windowHelper = new RibbonWindowHelper(this);
				}
				return windowHelper;
			}
		}
		EventHandlerList events;
		EventHandler MDIChildHostMenuIsVisibleChangedHandler;
		WeakReference iMDIChildHostWR = new WeakReference(null);
		IMDIChildHost MDIChildHost {
			get { return iMDIChildHostWR.Target as IMDIChildHost; }
			set {
				IMDIChildHost old = iMDIChildHostWR.Target as IMDIChildHost;
				if (old != null)
					old.IsChildMenuVisibleChanged -= MDIChildHostMenuIsVisibleChangedHandler;
				iMDIChildHostWR = new WeakReference(value);
				if (value != null) {
					value.IsChildMenuVisibleChanged += MDIChildHostMenuIsVisibleChangedHandler;
				}
				UpdateActualVisibility();
			}
		}
		FrameworkElement root;
		protected internal ImageSource ApplicationIcon {
			get { return (ImageSource)GetValue(ApplicationIconProperty); }
		}
		RibbonCustomizationHelper customizationHelper;
		public RibbonCustomizationHelper CustomizationHelper {
			get {
				if (customizationHelper == null)
					customizationHelper = new RibbonCustomizationHelper(this);
				return customizationHelper;
			}
		}
		internal List<object> objectsWithItemsSourcePropertyUsed = new List<object>();
		internal void AddOrRemoveObjectWithItemsSourcePropertyUsed(object obj, bool shouldAdd) {
			if (shouldAdd) {
				if (objectsWithItemsSourcePropertyUsed.Contains(obj))
					return;
				objectsWithItemsSourcePropertyUsed.Add(obj);
			} else {
				if (!objectsWithItemsSourcePropertyUsed.Contains(obj))
					return;
				RibbonControl ctrl = obj as RibbonControl;
				RibbonPageCategory cat = obj as RibbonPageCategory;
				RibbonPage page = obj as RibbonPage;
				if (ctrl != null)
					foreach (RibbonPageCategoryBase ch_cat in ctrl.Categories)
						AddOrRemoveObjectWithItemsSourcePropertyUsed(ch_cat, false);
				if (cat != null)
					foreach (RibbonPage ch_page in cat.Pages)
						AddOrRemoveObjectWithItemsSourcePropertyUsed(ch_page, false);
				if (page != null)
					foreach (RibbonPageGroup ch_group in page.Groups)
						AddOrRemoveObjectWithItemsSourcePropertyUsed(ch_group, false);
				objectsWithItemsSourcePropertyUsed.Remove(obj);
			}
		}
		internal bool IsItemsSourceModeUsedWithin { get { return objectsWithItemsSourcePropertyUsed.Count != 0; } }
		internal void AddLogicalChildCore(object child) {
			AddLogicalChild(child);
		}
		Func<object, EventArgs, bool> onEnterMenuMode;
		protected internal RibbonNavigationManager KeyboardNavigationManager { get; private set; }
		internal bool popupIsAlwaysOpen { get; set; }
		readonly MultiDictionary<RibbonControl, RibbonPageCategoryBase> mergedCategories;
		protected internal ApplicationMenu ApplicationMenuPopup { get; private set; }
		protected Grid MainGrid { get; private set; }
		protected Grid HeaderAndTabsGrid { get; private set; }
		public RibbonControl() {
			new RibbonControlSerializationStrategy(this);
			object item = CustomizeRibbonMenuItem;
			item = ShowToolbarAboveRibbonMenuItem;
			item = ShowToolbarBelowRibbonMenuItem;
			item = AddToToolbarMenuItem;
			item = MinimizeRibbonMenuItem;
			item = RibbonPopupMenu;
			item = null;
			MDIChildHostMenuIsVisibleChangedHandler = new EventHandler(OnMdiChildHostIsChildMenuVisibleChanged);
			Categories = new RibbonPageCategoryCollection(this);
			ActualCategories = new ObservableCollection<RibbonPageCategoryBase>();
			KeyboardNavigationManager = CreateNavigationManager();
			Toolbar = CreateToolbar();
			LoadPagesInBackground = true;
			PageHeaderLinksControl = CreatePageHeaderLinksControl();
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
			IsVisibleChanged += OnIsVisibleChanged;
			UpdateActualApplicationButtonLargeIcon();
			UpdateActualApplicationButtonSmallIcon();
			KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.None);
			DXSerializer.SetSerializationIDDefault(this, "ribbonControl");
			HierarchicalMergingHelper = new HierarchicalMergingHelper<RibbonControl>(this);
			((IMergingSupport)this).IsAutomaticallyMerged = true;
			mergedCategories = new MultiDictionary<RibbonControl, RibbonPageCategoryBase>();
			RibbonControl.SetRibbon(this, this);
			UpdateActualMinimizationButtonVisibility();
			IsEnabledChanged += OnIsEnabledChanged;
		}
		protected virtual void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			foreach(object logicalChild in LogicalTreeHelper.GetChildren(this)) {
				if(logicalChild is FrameworkContentElement || logicalChild is FrameworkElement)
					((DependencyObject)logicalChild).CoerceValue(e.Property);
			}
		}
		protected virtual RibbonNavigationManager CreateNavigationManager() {
			return new RibbonNavigationManager(this);
		}
		protected internal RibbonPageCategoryBase DefaultCategory { get; private set; }
		protected virtual void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
			UpdateWindow();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			UpdateIsInRibbonWindow();
			ExecuteControllers();
			MDIChildHost = LayoutHelper.FindParentObject<Bars.IMDIChildHost>(this);
			if (Application.Current != null) {
				Application.Current.Deactivated -= OnApplicationDeactivated;
				Application.Current.Deactivated += new EventHandler(OnApplicationDeactivated);
			}
			root = LayoutHelper.GetRoot(this);
			root.PreviewKeyDown -= OnPreviewKeyDown;
			root.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);
			root.PreviewTextInput -= OnPreviewTextInput;
			root.PreviewTextInput += new TextCompositionEventHandler(OnPreviewTextInput);
			UpdateActualHeaderBorderTemplate();
			LayoutUpdated += new EventHandler(OnLayoutUpdatedCore);
			IsFirstLayoutUptated = true;
			if (SelectedPage == null && MergedParent == null) {
				if (string.IsNullOrEmpty(SelectedPageName)) {
					SelectedPage = GetFirstSelectablePage();
				} else
					SelectedPage = FindPageByName(SelectedPageName);
			}
			UpdateActualVisibility();
			MenuModeHelper.EnterMenuMode += onEnterMenuMode ?? (onEnterMenuMode = new Func<object, EventArgs, bool>(OnEnterMenuMode));
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			ClearValue(IsInRibbonWindowPropertyKey);
			LayoutUpdated -= OnLayoutUpdatedCore;
			if (Application.Current != null)
				Application.Current.Deactivated -= OnApplicationDeactivated;
			if (root != null) {
				root.PreviewKeyDown -= OnPreviewKeyDown;
				root.PreviewTextInput -= OnPreviewTextInput;
				root = null;
			}
			KeyboardNavigationManager.IsNavigationMode = false;
			MDIChildHost = null;
			if (onEnterMenuMode != null)
				MenuModeHelper.EnterMenuMode -= onEnterMenuMode;
		}
		void UpdateActualHeaderBorderTemplate() {
			if (IsInRibbonWindow) {
				ActualHeaderBorderTemplate = HeaderBorderTemplateInRibbonWindow;
			} else
				ActualHeaderBorderTemplate = StandaloneHeaderBorderTemplate;
		}
		protected virtual void OnApplicationDeactivatedCore(object sender, EventArgs e) {
			if (KeyboardNavigationManager.IsNavigationMode) {
				KeyboardNavigationManager.IsNavigationMode = false;
			}
		}
		protected virtual void OnApplicationDeactivated(object sender, EventArgs e) {
			this.Dispatcher.BeginInvoke(new Action(() => { this.OnApplicationDeactivatedCore(sender, e); }), null);
		}
		internal DependencyObject GetTemplateChildCore(string childName) {
			return GetTemplateChild(childName);
		}
		protected internal virtual RibbonPageCategoryControl GetPageCategoryControl(int index) {
			if (CategoriesPane == null)
				return null;
			return CategoriesPane.ItemContainerGenerator.ContainerFromIndex(index) as RibbonPageCategoryControl;
		}
		protected internal virtual RibbonPageCategoryHeaderControl GetPageCategoryHeaderControl(int index) {
			var category = GetPageCategoryControl(index);
			return category == null ? null : category.PageCategoryHeaderControl;
		}
		protected internal virtual RibbonPageCategoryControl GetPageCategoryControl(RibbonPageCategoryBase category) {
			if (CategoriesPane == null)
				return null;
			return CategoriesPane.ItemContainerGenerator.ContainerFromItem(category) as RibbonPageCategoryControl;
		}
		double GetAeroContentOffset() {
			if (AeroBorderRibbonStyle == RibbonStyle.Office2007) {
				return Math.Round(HeaderBorder.RenderSize.Height + AeroBorderTopOffset);
			} else {
				if (IsMinimized && GetToolbarMode() != RibbonQuickAccessToolbarShowMode.ShowBelow) {
					return Math.Round(HeaderAndTabsGrid.RenderSize.Height + CollapsedRibbonAeroBorderTopOffset);
				}
				return Math.Round(HeaderAndTabsGrid.RenderSize.Height + AeroBorderTopOffset);
			}
		}
		void OnLayoutUpdatedCore(object sender, EventArgs e) {
			if (IsFirstLayoutUptated) {
				IsFirstLayoutUptated = false;
				if (!this.IsInDesignTool() && ApplicationMenu as BackstageViewControl != null && (ApplicationMenu as BackstageViewControl).IsOpen)
					ShowApplicationMenu();
			}
			UpdateActualWindowTitle();
			UpdateRibbonTitleBarVisibility();
			UpdateHeaderControlLocation();
			UpdateToolbarWidth();
			UpdateToolbarOffset();
			WindowHelper.UpdateRibbonVisibility();
			if (MergedParent != null)
				return;
			if(IsAeroMode) {
				double offset = Visibility == Visibility.Collapsed ? 0d : GetAeroContentOffset();
				WindowHelper.SetAeroContentHorizontalOffset(offset);
			} else if(IsInRibbonWindow && RibbonHeaderVisibility != RibbonHeaderVisibility.Collapsed) {
				double height = Visibility == Visibility.Collapsed ? 0d : HeaderBorder.RenderSize.Height;
				var controlBoxContainer = WindowHelper.GetControlBoxContainer();
				if(controlBoxContainer != null)
					height = Math.Max(controlBoxContainer.DesiredSize.Height, height);
				WindowHelper.SetRibbonHeaderBorderHeight(height);
			}
		}
		protected virtual void UpdateHeaderControlLocation() {
			if(CategoriesPane == null)
				return;
			if(!ShowApplicationButton && (RibbonStyle == RibbonStyle.Office2007 || RibbonStyle == RibbonStyle.Office2010)) {
				CategoriesPane.SetCurrentValue(Grid.ColumnProperty, 0);
				CategoriesPane.SetCurrentValue(Grid.ColumnSpanProperty, 3);
			} else {
				CategoriesPane.ClearValue(Grid.ColumnProperty);
				CategoriesPane.ClearValue(Grid.ColumnSpanProperty);
			}
		}
		protected virtual void UpdateToolbarOffset() {
			if(HeaderToolbarContainer == null)
				return;
			if (ApplicationIconContainer != null && IsApplicationIconVisible && RibbonStyle != RibbonStyle.TabletOffice) {
				HeaderToolbarContainer.SetCurrentValue(FrameworkElement.MarginProperty, new Thickness(ApplicationIconContainer.DesiredSize.Width, 0, 0, 0));
			} else {
				HeaderToolbarContainer.ClearValue(MarginProperty);
			}
		}
		void UpdateToolbarWidth() {
			if (HeaderToolbarContainer == null)
				return;
			if (RibbonStyle != RibbonStyle.TabletOffice && CategoriesPane != null && IsRibbonTitleBarActualVisible) {
				var categories = ActualCategories.Where(cat => cat != DefaultCategory && cat.IsVisible);
				var firstCategory = IsMerged ? categories.OrderBy(cat => cat.MergeOrder).FirstOrDefault() : categories.FirstOrDefault();
				var categoryContol = CategoriesPane.ItemContainerGenerator.ContainerFromItem(firstCategory) as RibbonPageCategoryControl;
				double minWidth = Toolbar.GetMinDesiredWidth();
				double maxWidth = double.PositiveInfinity;
				if (categoryContol != null && categoryContol.PageCategoryHeaderControl != null && categoryContol.PageCategoryHeaderControl.IsVisible)
					maxWidth = categoryContol.TranslatePoint(new Point(), HeaderToolbarContainer).X;
				HeaderToolbarContainer.MaxWidth = Math.Max(maxWidth, minWidth);
			} else
				HeaderToolbarContainer.ClearValue(MaxWidthProperty);
		}
		protected internal RibbonQuickAccessToolbarShowMode GetToolbarMode() {
			if (ToolbarShowMode == RibbonQuickAccessToolbarShowMode.Default)
				return RibbonQuickAccessToolbarShowMode.ShowAbove;
			return ToolbarShowMode;
		}
		protected internal RibbonPageCategoryHeaderControl GetPageCategoryHeader(RibbonPageCategoryBase category) {
			return GetPageCategoryControl(category).With(x => x.PageCategoryHeaderControl);			
		}
		#region events
		protected EventHandlerList Events {
			get {
				if (events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		public event RoutedValueChangedEventHandler<DependencyObject> ApplicationMenuChanged {
			add { AddHandler(ApplicationMenuChangedEvent, value); }
			remove { RemoveHandler(ApplicationMenuChangedEvent, value); }
		}
		protected internal event EventHandler ManagerChanged {
			add { Events.AddHandler(managerChanged, value); }
			remove { Events.RemoveHandler(managerChanged, value); }
		}
		public event RibbonPropertyChangedEventHandler AutoHideModeChanged {
			add { Events.AddHandler(autoHideModeChangedEventHandler, value); }
			remove { Events.RemoveHandler(autoHideModeChangedEventHandler, value); }
		}
		public event RibbonPropertyChangedEventHandler SelectedPageChanged {
			add { Events.AddHandler(selectedPageChangedEventHandler, value); }
			remove { Events.RemoveHandler(selectedPageChangedEventHandler, value); }
		}
		public event ToolbarCustomizationMenuShowingEventHandler ToolbarCustomizationMenuShowing {
			add { Events.AddHandler(toolbarCustomizationMenuShowingEventHandler, value); }
			remove { Events.RemoveHandler(toolbarCustomizationMenuShowingEventHandler, value); }
		}
		public event ToolbarModeChangedEventHandler ToolbarModeChanged {
			add { Events.AddHandler(toolbarModeChangedEventHandler, value); }
			remove { Events.RemoveHandler(toolbarModeChangedEventHandler, value); }
		}
		public event ToolbarCustomizationMenuClosedEventHandler ToolbarCustomizationMenuClosed {
			add { Events.AddHandler(toolbarCustomizationMenuClosedEventHandler, value); }
			remove { Events.RemoveHandler(toolbarCustomizationMenuClosedEventHandler, value); }
		}
		public event RibbonPopupMenuShowingEventHandler RibbonPopupMenuShowing {
			add { Events.AddHandler(popupMenuShowingEventHandler, value); }
			remove { Events.RemoveHandler(popupMenuShowingEventHandler, value); }
		}
		public event RibbonPopupMenuClosedEventHandler RibbonPopupMenuClosed {
			add { Events.AddHandler(popupMenuClosedEventHandler, value); }
			remove { Events.RemoveHandler(popupMenuClosedEventHandler, value); }
		}
		public event EventHandler BackstageClosed {
			add { Events.AddHandler(backstageClosedEventHandler, value); }
			remove { Events.RemoveHandler(backstageClosedEventHandler, value); }
		}
		public event EventHandler BackstageOpened {
			add { Events.AddHandler(backstageOpenedEventHandler, value); }
			remove { Events.RemoveHandler(backstageOpenedEventHandler, value); }
		}
		internal event RibbonPageInsertedEventHandler RibbonPageInserted {
			add { Events.AddHandler(ribbonPageInsertedEventHandler, value); }
			remove { Events.RemoveHandler(ribbonPageInsertedEventHandler, value); }
		}
		internal event RibbonPageRemovedEventHandler RibbonPageRemoved {
			add { Events.AddHandler(ribbonPageRemovedEventHandler, value); }
			remove { Events.RemoveHandler(ribbonPageRemovedEventHandler, value); }
		}
		internal event RibbonPageGroupInsertedEventHandler RibbonPageGroupInserted {
			add { Events.AddHandler(ribbonPageGroupInsertedEventHandler, value); }
			remove { Events.RemoveHandler(ribbonPageGroupInsertedEventHandler, value); }
		}
		internal event RibbonPageGroupRemovedEventHandler RibbonPageGroupRemoved {
			add { Events.AddHandler(ribbonPageGroupRemovedEventHandler, value); }
			remove { Events.RemoveHandler(ribbonPageGroupRemovedEventHandler, value); }
		}
		public event RibbonSaveRestoreLayoutExceptionEventHandler SaveLayoutException {
			add { Events.AddHandler(saveLayoutExceptionEventHanler, value); }
			remove { Events.RemoveHandler(saveLayoutExceptionEventHanler, value); }
		}
		public event RibbonSaveRestoreLayoutExceptionEventHandler RestoreLayoutException {
			add { Events.AddHandler(restoreLayoutExceptionEventHanler, value); }
			remove { Events.RemoveHandler(restoreLayoutExceptionEventHanler, value); }
		}
		protected void RaiseManagerChanged() {
			EventHandler handler = Events[managerChanged] as EventHandler;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		protected void RaisePropertyChanged(object eventHandler, RibbonPropertyChangedEventArgs e) {
			RibbonPropertyChangedEventHandler handler = Events[eventHandler] as RibbonPropertyChangedEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected internal void RaiseToolbarCustomizationMenuShowing(ToolbarCustomizationMenuShowingEventArgs e) {
			ToolbarCustomizationMenuShowingEventHandler handler = Events[toolbarCustomizationMenuShowingEventHandler] as ToolbarCustomizationMenuShowingEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected internal void RaiseToolbarModeChanging(PropertyChangedEventArgs e) {
			ToolbarModeChangedEventHandler handler = Events[toolbarModeChangedEventHandler] as ToolbarModeChangedEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected internal void RaiseToolbarCustomizationMenuClosed(ToolbarCustomizationMenuClosedEventArgs e) {
			ToolbarCustomizationMenuClosedEventHandler handler = Events[toolbarCustomizationMenuClosedEventHandler] as ToolbarCustomizationMenuClosedEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected internal void RaiseRibbonPopupMenuShowing(RibbonPopupMenuShowingEventArgs e) {
			RibbonPopupMenuShowingEventHandler handler = Events[popupMenuShowingEventHandler] as RibbonPopupMenuShowingEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected internal void RaiseRibbonPopupMenuClosed(RibbonPopupMenuClosedEventArgs e) {
			RibbonPopupMenuClosedEventHandler handler = Events[popupMenuClosedEventHandler] as RibbonPopupMenuClosedEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected void RaiseBackstageOpened() {
			EventHandler handler = Events[backstageOpenedEventHandler] as EventHandler;
			if (handler != null)
				handler(this, new EventArgs());
		}
		protected void RaiseBackstageClosed() {
			EventHandler handler = Events[backstageClosedEventHandler] as EventHandler;
			if (handler != null)
				handler(this, new EventArgs());
		}
		internal void RaiseRibbonPageInserted(RibbonPage page, int index) {
			RibbonPageInsertedEventHandler handler = Events[ribbonPageInsertedEventHandler] as RibbonPageInsertedEventHandler;
			if (handler != null) handler(this, new RibbonPageInsertedEventArgs(page, index));
		}
		internal void RaiseSaveLayoutException(Exception ex) {
			var handler = Events[saveLayoutExceptionEventHanler] as RibbonSaveRestoreLayoutExceptionEventHandler;
			if (handler != null) handler(this, new RibbonSaveRestoreLayoutExceptionEventArgs(ex));
		}
		internal void RaiseRestoreLayoutException(Exception ex) {
			var handler = Events[restoreLayoutExceptionEventHanler] as RibbonSaveRestoreLayoutExceptionEventHandler;
			if (handler != null) handler(this, new RibbonSaveRestoreLayoutExceptionEventArgs(ex));
		}
		internal void RaiseRibbonPageRemoved(RibbonPage page, int index) {
			RibbonPageRemovedEventHandler handler = Events[ribbonPageRemovedEventHandler] as RibbonPageRemovedEventHandler;
			if (handler != null) handler(this, new RibbonPageRemovedEventArgs(page, index));
		}
		internal void RaiseRibbonPageGroupInserted(RibbonPageGroup group, int groupIndex) {
			RibbonPageGroupInsertedEventHandler handler = Events[ribbonPageGroupInsertedEventHandler] as RibbonPageGroupInsertedEventHandler;
			if (handler != null) handler(this, new RibbonPageGroupInsertedEventArgs(group, groupIndex));
		}
		internal void RaiseRibbonPageGroupRemoved(RibbonPageGroup group, int groupIndex) {
			RibbonPageGroupRemovedEventHandler handler = Events[ribbonPageGroupRemovedEventHandler] as RibbonPageGroupRemovedEventHandler;
			if (handler != null) handler(this, new RibbonPageGroupRemovedEventArgs(group, groupIndex));
		}
		#endregion
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RibbonQuickAccessToolbar Toolbar { get; private set; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
#if !SL
	DevExpressXpfRibbonLocalizedDescription("RibbonControlToolbarItemLinks")
#else
	Description("")
#endif
]
		public BarItemLinkCollection ToolbarItemLinks {
			get { return Toolbar.ItemLinks; }
		}
		protected virtual RibbonQuickAccessToolbar CreateToolbar() {
			var toolbar = new RibbonQuickAccessToolbar();
			toolbar.Ribbon = this;
			return toolbar;
		}
		protected virtual RibbonPageHeaderLinksControl CreatePageHeaderLinksControl() {
			var pageHeaderControl = new RibbonPageHeaderLinksControl(this);
			pageHeaderControl.Ribbon = this;
			return pageHeaderControl;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RibbonPageHeaderLinksControl PageHeaderLinksControl { get; private set; }
		public CommonBarItemCollection PageHeaderItems { get { return PageHeaderLinksControl.CommonItems; } }
		public CommonBarItemCollection ToolbarItems { get { return Toolbar.Items; } }
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlPageHeaderItemLinks")]
#endif
		public BarItemLinkCollection PageHeaderItemLinks { get { return PageHeaderLinksControl.ItemLinks; } }
#if !SL
	[DevExpressXpfRibbonLocalizedDescription("RibbonControlManager")]
#endif
		public BarManager Manager {
			get { return BarManager.GetBarManager(this); }
		}
		protected virtual void OnManagerChanged(BarManager oldManager, BarManager newManager) {
			if (Manager == null)
				return;
			if (BarManagerHelper.GetChildRibbonControl(Manager) == null)
				BarManagerHelper.SetChildRibbonControl(Manager, this);
			RaiseManagerChanged();
		}
		internal ApplicationMenu GetApplicationMenu() {
			return ApplicationMenu as ApplicationMenu;
		}
		#region merging                
		HierarchicalMergingHelper<RibbonControl> HierarchicalMergingHelper { get; set; }
		HierarchicalMergingHelper<RibbonControl> IHierarchicalMergingSupport<RibbonControl>.Helper { get { return HierarchicalMergingHelper; } }
		protected internal ReadOnlyCollection<RibbonControl> MergedChildren { get { return HierarchicalMergingHelper.MergedChildren; } }
		protected internal ObservableCollection<RibbonControl> ActualMergedChildren { get { return HierarchicalMergingHelper.ActualMergedChildren; } }
		protected internal ReadOnlyObservableCollection<RibbonControl> CompositeMergedChildren { get { return HierarchicalMergingHelper.CompositeMergedChildren; } }
		RibbonControl mergedParent;
		public RibbonControl ActualMergedParent { get { return HierarchicalMergingHelper.ActualMergedParent; } set{ HierarchicalMergingHelper.ActualMergedParent = value; } }
		public RibbonControl MergedParent {
			get { return mergedParent; }
			set {
				if (value == mergedParent) return;
				RibbonControl oldValue = mergedParent;
				mergedParent = value;
				OnMergedParentChanged(oldValue);
			}
		}				
		internal RibbonPage MergedParentSelectedPage { get; set; }
		internal object MergedParentSelectedPageCaption { get; set; }
		internal RibbonPage SelectedPageBeforeMerging { get; set; }
		public bool IsMergedChild(RibbonControl item) {
			return ActualMergedChildren.Contains(item);
		}
		RibbonPageCategoryBase FindCategoryByCaption(string caption) {
			return ActualCategories.FirstOrDefault(cat => string.Equals(cat.Caption, caption));
		}
		void RemoveUnusedCategoriesForMerging() {
			foreach(RibbonPageCategoryBase cat in Categories) {
				if(cat.MergeType == RibbonMergeType.Remove) {
					ActualCategories.Remove(cat);
					if(cat == DefaultCategory)
						DefaultCategory = null;
				} else cat.RemoveUnusedPagesForMerging();
			}
		}
		void RestoreUnusedCategoriesForMerging() {
			int index = 0;
			foreach(RibbonPageCategoryBase cat in Categories) {
				if(cat.MergeType == RibbonMergeType.Remove) {
					ActualCategories.Insert(index, cat);
					if(cat.IsDefault)
						DefaultCategory = cat;
				} else cat.RestoreUnusedPagesForMerging();
				index++;
			}
		}
		protected virtual void AddChildRibbonCategory(RibbonPageCategoryBase childCategory) {
			ActualCategories.Add(childCategory);
			if(childCategory.IsSelected) {
				RibbonPage page = childCategory.GetSelectedPage();
				if(page != null) {
					SetCurrentValue(SelectedPageProperty, page);
				}
			}
			childCategory.MergedParentRibbon = this;
		}
		protected virtual void MergeChild(RibbonControl childRibbon) {
			foreach(RibbonPageCategoryBase cat in childRibbon.Categories) {
				mergedCategories.Add(childRibbon, cat);
				if(cat.MergeType == RibbonMergeType.Remove)
					continue;
				if(cat.IsDefault) {
					if(DefaultCategory == null) {
						AddChildRibbonCategory(cat);
						DefaultCategory = cat;
					} else if(cat.MergeType == RibbonMergeType.Replace) {
						int defaultCategotyIndex = ActualCategories.IndexOf(DefaultCategory);
						ActualCategories[defaultCategotyIndex] = cat;
						cat.ReplacedCategory = DefaultCategory;
						DefaultCategory = cat;
					} else
						DefaultCategory.Merge(cat);
				} else {
					RibbonPageCategoryBase sameCaptionCategory = FindCategoryByCaption(cat.Caption);
					if(cat.MergeType == RibbonMergeType.Add || string.IsNullOrEmpty(cat.Caption) || sameCaptionCategory == null) {
						AddChildRibbonCategory(cat);
					} else if(cat.MergeType == RibbonMergeType.Replace) {
						ActualCategories[ActualCategories.IndexOf(sameCaptionCategory)] = cat;
						cat.ReplacedCategory = sameCaptionCategory;
					} else
						sameCaptionCategory.Merge(cat);
				}
			}
		}
		public void Merge(RibbonControl childRibbon) {
			HierarchicalMergingHelper.Merge(childRibbon);
		}
#pragma warning disable 3005
		public void UnMerge(RibbonControl childRibbon) {
			HierarchicalMergingHelper.UnMerge(childRibbon);
		}
		public void UnMerge() {
			while (MergedChildren.Count > 0)
				UnMerge(MergedChildren[MergedChildren.Count - 1]);
		}
#pragma warning restore 3005
		protected virtual void OnMergedParentChanged(RibbonControl oldValue) {
			CoerceValue(VisibilityProperty);
		}		
		protected virtual void MergeCore(RibbonControl childRibbon,bool isNewChild, ref RibbonPage selectedPage, ref object selectedPageCaption, ref bool forciblySelectPage, RibbonPage unmergedSelectedPage, object unmergedSelectedPageCaption) {
			if (MergedParent != null || childRibbon.ActualMergedParent != null)
				return;			
			childRibbon.ExecuteControllers();
			childRibbon.SelectedPageBeforeMerging = childRibbon.SelectedPage;
			if (childRibbon.SelectedPageOnMerging != SelectedPageOnMerging.SelectedPage &&
				(UseCaptionToIdentifyPagesOnMerging ? !Equals(selectedPageCaption, childRibbon.SelectedPage.With(x => x.Caption)) : selectedPage != childRibbon.SelectedPage)) {
				childRibbon.SelectedPage = null;
			}
			childRibbon.MergedParentSelectedPage = unmergedSelectedPage;
			childRibbon.MergedParentSelectedPageCaption = unmergedSelectedPageCaption;
			if (!IsMerged)
				RemoveUnusedCategoriesForMerging();
			MergeChild(childRibbon);
			ActualMergedChildren.Add(childRibbon);
			childRibbon.ActualMergedParent = this;
			((ILinksHolder)Toolbar).Merge(childRibbon.Toolbar);
			((ILinksHolder)PageHeaderLinksControl).Merge(childRibbon.PageHeaderLinksControl);
			childRibbon.ApplicationMenuChanged += OnChildRibbonApplicationMenuChanged;
			MergeApplicationMenu(childRibbon);
			IsMerged = true;
			if (!IsLoaded) {
				if (SelectedPageControl.GetPageGroupCount() != 0) {
					RibbonPageGroupControl ctrl = SelectedPageControl.GetPageGroupControlFromIndex(0);
					DependencyObject element = VisualTreeHelper.GetParent(ctrl);
					while (element as UIElement != null && !(element is RibbonPagesControl)) {
						((UIElement)element).InvalidateMeasure();
						element = VisualTreeHelper.GetParent(element);
					}
				}
			}
			if (childRibbon.SelectedPageOnMerging == SelectedPageOnMerging.SelectedPage && isNewChild) {
				selectedPage = childRibbon.SelectedPage.With(x => x.MergedParent ?? x);
				selectedPageCaption = selectedPage.With(x=>x.Caption);
				forciblySelectPage = true;
			}
		}		
		protected virtual void UnMergeCore(RibbonControl childRibbon, ref RibbonPage selectedPage, ref object selectedPageCaption) {
			if (childRibbon.ActualMergedParent != this)
				return;
			childRibbon.ActualMergedParent = null;
			UnMergeChild(childRibbon, ref selectedPage, ref selectedPageCaption);
			((ILinksHolder)Toolbar).UnMerge(childRibbon.Toolbar);
			((ILinksHolder)PageHeaderLinksControl).UnMerge(childRibbon.PageHeaderLinksControl);
			childRibbon.ApplicationMenuChanged -= OnChildRibbonApplicationMenuChanged;
			UnMergeApplicationMenu(childRibbon);
			ActualMergedChildren.Remove(childRibbon);			
			if (ActualMergedChildren.Count == 0) {
				IsMerged = false;
				RestoreUnusedCategoriesForMerging();
			}
			childRibbon.UpdateActualVisibility();
			if (CategoriesPane != null && CategoriesPane.RibbonItemsPanel != null)
				CategoriesPane.RibbonItemsPanel.InvalidateMeasure();
		}
		protected virtual void OnChildRibbonApplicationMenuChanged(object sender, RoutedValueChangedEventArgs<DependencyObject> args) {
			UnMergeApplicationMenu(args.OldValue as ApplicationMenu);
			MergeApplicationMenu(args.NewValue as ApplicationMenu);
		}
		void IHierarchicalMergingSupport<RibbonControl>.ReMerge() { ReMerge(); }
		protected virtual void ReMerge() {
			object selectedPageCaption = SelectedPage.With(x=>x.Caption);
			RibbonPage selectedPage = SelectedPage;
			bool forciblySelectPage = false;			
			var unmergedChildren = ActualMergedChildren.AsEnumerable().Reverse().ToList();			
			foreach (var child in unmergedChildren) {
				UnMergeCore(child, ref selectedPage, ref selectedPageCaption);
			}
			foreach (var child in CompositeMergedChildren) {
				MergeCore(child, !unmergedChildren.Contains(child), ref selectedPage, ref selectedPageCaption, ref forciblySelectPage, selectedPage, selectedPageCaption);
			}
			UpdateSelectedPageAfterMerging(selectedPageCaption, selectedPage, forciblySelectPage);
			foreach (var childRibbon in unmergedChildren) {
				if (childRibbon.ActualMergedParent != null)
					continue;
				childRibbon.SelectedPage = childRibbon.SelectedPageBeforeMerging;
				childRibbon.SelectedPage.Do(x => x.SetCurrentValue(RibbonPage.IsSelectedProperty, true));
				childRibbon.SelectedPageBeforeMerging = null;
				if ((childRibbon.SelectedPage == null || childRibbon.SelectedPage.Ribbon != childRibbon))
					childRibbon.SelectFirstVisiblePage(true);
				childRibbon.HierarchicalMergingHelper.ReMergeAsync();
			}
		}		
		protected virtual void UpdateSelectedPageAfterMerging(object selectedPageCaption, RibbonPage selectedPage, bool force) {
			if (MergedParent != null)
				return;
			var currentSelectedPage = SelectedPage;
			if (UseCaptionToIdentifyPagesOnMerging) {
				selectedPage = FindPageByCaption(selectedPageCaption);
				currentSelectedPage = FindPageByCaption(SelectedPage.With(x => x.Caption));
			}
			if (!CanSelectPage(SelectedPage) || force) {
				if (CanSelectPage(currentSelectedPage) && !force)
					selectedPage = currentSelectedPage;
				SetCurrentValue(SelectedPageProperty, selectedPage);
				SelectedPage.Do(x => x.SetCurrentValue(RibbonPage.IsSelectedProperty, true));
			}
			if (SelectedPage == null)
				SelectedPage = GetFirstSelectablePage();
		}
		RibbonPage FindPageByCaption(object caption) {
			return ActualCategories.SelectMany(x => x.ActualPages).FirstOrDefault(x => Equals(x.Caption, caption));
		}				
		protected virtual void UnMergeChild(RibbonControl childRibbon, ref RibbonPage selectedPage, ref object selectedPageName) {
			var categories = mergedCategories[childRibbon];
			foreach (RibbonPageCategoryBase cat in categories) {
				if (cat.MergedParent != null) {
					cat.MergedParent.UnMerge(cat);
				} else if (cat.ReplacedCategory != null) {
					ActualCategories[ActualCategories.IndexOf(cat)] = cat.ReplacedCategory;
					if (cat.ReplacedCategory.IsDefault)
						DefaultCategory = cat.ReplacedCategory;
					cat.ReplacedCategory = null;
				} else {
					ActualCategories.Remove(cat);
					cat.MergedParentRibbon = null;
					if (cat == DefaultCategory)
						DefaultCategory = null;
				}
			}
			mergedCategories.Remove(childRibbon);
			selectedPage = childRibbon.MergedParentSelectedPage;
			selectedPageName = childRibbon.MergedParentSelectedPageCaption;
			childRibbon.MergedParentSelectedPage = null;
			childRibbon.MergedParentSelectedPageCaption = null;			
		}
		bool CanSelectPage(RibbonPage page) {
			if(page == null) return false;
			return ActualCategories.SelectMany(x => x.ActualPages).Contains(page);			
		}
		protected virtual void MergeApplicationMenu(ApplicationMenu childMenu) {
			ApplicationMenu parentMenu = GetApplicationMenu();
			if(parentMenu != null && childMenu != null)
				((ILinksHolder)parentMenu).Merge(childMenu);
		}		
		protected virtual void UnMergeApplicationMenu(ApplicationMenu childMenu) {
			ApplicationMenu parentMenu = GetApplicationMenu();
			if(parentMenu != null && childMenu != null)
				((ILinksHolder)parentMenu).UnMerge(childMenu);
		}
		protected virtual void MergeApplicationMenu(RibbonControl childRibbon) {
			MergeApplicationMenu(childRibbon.GetApplicationMenu());
		}
		protected virtual void UnMergeApplicationMenu(RibbonControl childRibbon) {
			UnMergeApplicationMenu(childRibbon.GetApplicationMenu());
		}
		#endregion
		bool controllersExecuted = false;
		void ExecuteControllers() {
			if (!controllersExecuted)
				ControllerBehavior.Execute();
			controllersExecuted = true;
		}
		#region serialization                
		public virtual void SaveDefaultLayout() {
		}
		public void RestoreDefaultLayout() {			
		}		
		protected internal virtual string GetSerializationAppName() {
			return typeof(RibbonControl).Name;
		}
		public virtual void RestoreLayout(object path) {
			try {
				DXSerializer.Deserialize(this, path, GetSerializationAppName(), null);
			} catch (Exception ex) {
				RaiseRestoreLayoutException(ex);
			}
		}
		public virtual void SaveLayout(object path) {
			try {
				DXSerializer.Serialize(this, path, GetSerializationAppName(), null);
			} catch (Exception ex) {
				RaiseSaveLayoutException(ex);
			}
		}
		#endregion
		internal RibbonSelectedPagePopup selectedPagePopup;
		protected internal RibbonSelectedPagePopup SelectedPagePopup {
			get {
				if(selectedPagePopup == null) {
					selectedPagePopup = CreateSelectedPagePopup();
				}
				return selectedPagePopup;
			}
		}
		RibbonSelectedPageControl selectedPageControl;
		protected internal RibbonSelectedPageControl SelectedPageControl {
			get {
				if(selectedPageControl == null) {
					selectedPageControl = new RibbonSelectedPageControl();
					selectedPageControl.IsTabStop = false;
					selectedPageControl.Ribbon = this;
				}
				return selectedPageControl;
			}
		}
		protected internal ContentControl SelectedPageControlContainer { get; private set; }
		protected ContentControl CollapsedSelectedPageBorder { get; private set; }
		protected internal ContentControl HeaderToolbarContainer { get; private set; }
		protected internal ContentControl FooterToolbarContainer { get; private set; }
		protected internal RibbonApplicationButtonControl ApplicationButton { get; private set; }
		protected internal RibbonPageCategoriesPane CategoriesPane { get; private set; }
		protected internal Grid RightTabContainer { get; private set; }
		protected DXRibbonWindowTitlePanel TitlePanel { get; private set; }
		protected internal FrameworkElement CaptionContainer { get { return TitlePanel.With(panel => panel.CaptionContentControl); } }
		protected internal UIElement HeaderBorder { get; private set; }
		protected internal DXContentPresenter PageHeaderLinksControlContainer { get; private set; }
		protected internal ContentControl ApplicationIconContainer { get; private set; }
		protected internal RibbonCheckedBorderControl MinimizationButton { get; private set; }
		protected RibbonAutoHideControl AutoHideControl { get; private set; }
		protected internal FrameworkElement ControlBoxContainer { get; private set; }
		protected virtual void SubscribeTemplateEvents() {
			if (ApplicationButton != null) {
				ApplicationButton.Ribbon = this;
				ApplicationButton.Click += new EventHandler(OnApplicationButtonClick);
			}
			if (ApplicationIconContainer != null) {
				ApplicationIconContainer.MouseLeftButtonDown += new MouseButtonEventHandler(OnApplicationIconContainerMouseLeftButtonDown);
				ApplicationIconContainer.MouseDoubleClick += new MouseButtonEventHandler(OnApplicationIconContainerMouseDoubleClick);
			}
			if (HeaderBorder != null) {
				HeaderBorder.PreviewMouseDown += new MouseButtonEventHandler(OnHeaderBorderPreviewMouseDown);
			}
			if (MinimizationButton != null) {
				MinimizationButton.Click += new EventHandler(OnMinimizationButtonClick);
			}
			if (HeaderToolbarContainer != null)
				HeaderToolbarContainer.SizeChanged += OnHeaderToolbarContainerSizeChanged;
		}
		protected virtual void UnsubscribeTemplateEvents() {
			if(ApplicationButton != null) {
				ApplicationButton.Ribbon = null;
				ApplicationButton.Click -= OnApplicationButtonClick;
			}
			if(ApplicationIconContainer != null) {
				ApplicationIconContainer.MouseLeftButtonDown -= OnApplicationIconContainerMouseLeftButtonDown;
				ApplicationIconContainer.MouseDoubleClick -= OnApplicationIconContainerMouseDoubleClick;
			}
			if(HeaderBorder != null)
				HeaderBorder.PreviewMouseDown -= OnHeaderBorderPreviewMouseDown;
			if(MinimizationButton != null) {
				MinimizationButton.Click -= OnMinimizationButtonClick;
			}
			if (HeaderToolbarContainer!=null)
				HeaderToolbarContainer.SizeChanged -= OnHeaderToolbarContainerSizeChanged;
		}
		void OnHeaderToolbarContainerSizeChanged(object sender, SizeChangedEventArgs e) {
			if(CategoriesPane == null || CategoriesPane.RibbonItemsPanel == null)
				return;
			CategoriesPane.RibbonItemsPanel.InvalidateMeasure();
		}
		void OnApplicationIconContainerMouseDoubleClick(object sender, MouseButtonEventArgs e) {
			WindowHelper.Close();
		}
		void OnHeaderBorderPreviewMouseDown(object sender, MouseButtonEventArgs e) {
			if(CanDragRibbonWindow()) {
				WindowHelper.DragOrMaximizeWindow(sender, e);
			}
		}
		protected virtual void OnMinimizationButtonClick(object sender, EventArgs e) {
			SetCurrentValue(IsMinimizedProperty, !IsMinimized);
		}
		void OnApplicationIconContainerMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if(IsVisible)
				WindowHelper.ShowSystemMenu(HeaderBorder, new Point(0, HeaderBorder.RenderSize.Height));
		}
		RibbonSelectedPagePopup CreateSelectedPagePopup() {
			RibbonSelectedPagePopup popup = new RibbonSelectedPagePopup();
			popup.Ribbon = this;
			BarManager.SetSkipMeasureByDockPanelLayoutHelper(popup, true);
			popup.Opening += (s, e) => AddLogicalChild(popup);
			popup.Closed += (s, e) => RemoveLogicalChild(popup);
			return popup;
		}
		void OnMdiChildHostIsChildMenuVisibleChanged(object sender, EventArgs e) {
			UpdateActualVisibility();
		}
		protected internal bool ApplicationMenuIsPopupMenu() {
			return ApplicationMenu is ApplicationMenu;
		}
		public virtual void CloseApplicationMenu() {
			if (ApplicationMenuStrategy == null)
				return;
			ApplicationMenuStrategy.Close();
		}
		protected internal bool ShowApplicationMenu() {
			if (ApplicationMenuStrategy == null)
				return false;
			ApplicationMenuStrategy.Open();
			return ApplicationMenuStrategy.IsMenuOpened;
		}
		protected internal virtual void OnApplicationButtonClick(object sender, EventArgs e) {
			if (ApplicationButton.IsChecked)
				CloseApplicationMenu();
			else
				ShowApplicationMenu();
		}
		protected virtual object OnIsMinimizedCoerce(object baseValue) {
			if(!AllowMinimizeRibbon || AutoHideMode)
				return false;
			return baseValue;
		}
		void UpdateSelectedPageControlPlacement() {
			if(IsMinimized) {
				if(SelectedPageControlContainer != null)
					SelectedPageControlContainer.Content = null;
				((RibbonSelectedPageContentPresenter)SelectedPagePopup.PopupContent).Content = SelectedPageControl;
				SelectedPageControl.IsPopup = true;
			} else {
				((RibbonSelectedPageContentPresenter)SelectedPagePopup.PopupContent).Content = null;
				SelectedPageControl.IsPopup = false;
				if(SelectedPageControlContainer != null)
					SelectedPageControlContainer.Content = SelectedPageControl;
			}
		}
		protected virtual void OnIsMinimizedChanged(bool oldValue) {
			if(!IsMinimized) {
				ClosePopup();
				UpdateSelectedPageControlPlacement();
				if(SelectedPageControl != null) {
					SelectedPageControl.Reset();
					SelectedPageControl.RecreateEditors();
				}
			} else {
				UpdateSelectedPageControlPlacement();
				SelectedPagePopup.RecreateEditors();
				IsMinimizedRibbonCollapsed = true;
				CloseAllPageGroupPopups();
			}
			UpdateCollapsedSelectedPageBorderVisibility();
		}
		protected virtual void UpdateCollapsedSelectedPageBorderVisibility() {
			if(CollapsedSelectedPageBorder == null)
				return;
			CollapsedSelectedPageBorder.Visibility = IsMinimized && GetToolbarMode() != RibbonQuickAccessToolbarShowMode.ShowBelow ? Visibility.Visible : Visibility.Collapsed;
		}
		protected virtual void CloseAllPageGroupPopups() {
			if(SelectedPageControl == null)
				return;
			for(int i = 0; i < SelectedPageControl.GetPageGroupCount(); i++) {
				RibbonPageGroupControl group = SelectedPageControl.GetPageGroupControlFromIndex(i);
				if(group != null)
					group.ClosePopup();
			}
		}
		protected virtual void OnAllowMinimizeRibbonChanged(bool oldValue) { }
		protected virtual void OnAllowKeyTipsChanged(bool oldValue) {
			if(AllowKeyTips)
				KeyboardNavigationManager.IsNavigationMode = false;
		}
		protected virtual void OnCategoriesSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(e.NewValue is IEnumerable || e.OldValue is IEnumerable) {
				ItemsAttachedBehaviorCore<RibbonControl, RibbonPageCategoryBase>.OnItemsSourcePropertyChanged(this,
					e,
					CategoryAttachedBehaviorProperty,
					CategoryTemplateProperty,
					CategoryTemplateSelectorProperty,
					CategoryStyleProperty,
					control => control.Categories,
					category => new RibbonDefaultPageCategory(), useDefaultTemplateSelector: true);
				AddOrRemoveObjectWithItemsSourcePropertyUsed(this, CategoriesSource != null);
			} else {
				PopulateItemsHelper.GenerateItems(e, () => new PagesGenerator(Categories.FindOrCreateNew(x => x.IsDefault, () => new RibbonDefaultPageCategory())));
			}
		}
		protected virtual void OnCategoryTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonControl, RibbonPageCategoryBase>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				CategoryAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksSourceElementGeneratesUniqueBarItemChanged(bool oldValue) {
			OnToolbarItemLinksSourceChanged(new System.Windows.DependencyPropertyChangedEventArgs(ToolbarItemLinksSourceProperty, ToolbarItemLinksSource, ToolbarItemLinksSource));
			OnPageHeaderItemLinksSourceChanged(new System.Windows.DependencyPropertyChangedEventArgs(PageHeaderItemLinksSourceProperty, PageHeaderItemLinksSource, PageHeaderItemLinksSource));
		}
		protected virtual void OnToolbarItemLinksTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonControl, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ToolbarItemsAttachedBehaviorProperty);
		}
		protected virtual void OnToolbarItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonControl, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				ToolbarItemsAttachedBehaviorProperty);
		}
		protected virtual void OnToolbarItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ToolbarItemGeneratorHelper.OnItemsSourceChanged(e);
		}
		protected virtual void OnIsHiddenRibbonCollapsedChanged(bool oldValue) { }
		protected virtual void OnIsMinimizationButtonVisibleChanged(bool oldValue) {
		}
		protected virtual object CoerceIsHiddenRibbonCollapsed(object baseValue) {
			if (!AutoHideMode)
				return true;
			return baseValue;
		}
		protected virtual object CoereceIsMinimizationButtonVisible(object baseValue) {
			if (AutoHideMode)
				return false;
			return baseValue;
		}
		BarItemGeneratorHelper<RibbonControl> toolbarItemGeneratorHelper;
		protected BarItemGeneratorHelper<RibbonControl> ToolbarItemGeneratorHelper {
			get {
				if(toolbarItemGeneratorHelper == null)
					toolbarItemGeneratorHelper = new BarItemGeneratorHelper<RibbonControl>(this, ToolbarItemsAttachedBehaviorProperty, ToolbarItemStyleProperty, ToolbarItemTemplateProperty, ToolbarItems, ToolbarItemTemplateSelectorProperty, ItemLinksSourceElementGeneratesUniqueBarItem);
				return toolbarItemGeneratorHelper;
			}
		}
		private void OnPageHeaderItemLinksTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonControl, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				PageHeaderItemsAttachedBehaviorProperty);
		}
		private void OnPageHeaderItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<RibbonControl, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e,
				PageHeaderItemsAttachedBehaviorProperty);
		}
		protected virtual void OnPageHeaderItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			PageHeaderItemGeneratorHelper.OnItemsSourceChanged(e);
		}
		BarItemGeneratorHelper<RibbonControl> pageHeaderItemGeneratorHelper;
		protected BarItemGeneratorHelper<RibbonControl> PageHeaderItemGeneratorHelper {
			get {
				if(pageHeaderItemGeneratorHelper == null)
					pageHeaderItemGeneratorHelper = new BarItemGeneratorHelper<RibbonControl>(this, PageHeaderItemsAttachedBehaviorProperty, PageHeaderItemStyleProperty, PageHeaderItemTemplateProperty, PageHeaderItems, PageHeaderItemTemplateSelectorProperty, ItemLinksSourceElementGeneratesUniqueBarItem);
				return pageHeaderItemGeneratorHelper;
			}
		}
		protected virtual void OnShowApplicationButtonChanged(bool oldValue) {
			UpdateToolbarPlacement();
			UpdateApplicationIconVisibility();
		}
		readonly Locker selectedPageNameLocker = new Locker();
		readonly Locker selectedPageChangedLocker = new Locker();
		protected virtual void OnSelectedPageChanged(RibbonPage oldValue) {
			selectedPageChangedLocker.DoIfNotLocked(() => OnSelectedPageChangedLocked(oldValue));
		}
		void OnSelectedPageChangedLocked(RibbonPage oldValue) {
			if (oldValue == SelectedPage)
				return;
			selectedPageNameLocker.DoLockedAction(() => {
				bool hasSelectedPage = SelectedPage != null;
				if (hasSelectedPage) {
					var oldIsSelected = SelectedPage.IsSelected;
					SelectedPage.SetCurrentValue(RibbonPage.IsSelectedProperty, true);
					if (oldIsSelected)
						OnPageIsSelectedChanged(SelectedPage, EventArgs.Empty);
					this.SetCurrentValue(SelectedPageNameProperty, SelectedPage.Name);
				} else {
					ClearValue(SelectedPageNameProperty);
				}
			});
			selectedPageChangedLocker.DoLockedAction(() => {
				if (oldValue != null)
					oldValue.SetCurrentValue(RibbonPage.IsSelectedProperty, false);
			});			
			RaisePropertyChanged(selectedPageChangedEventHandler, new RibbonPropertyChangedEventArgs(oldValue, SelectedPage));
		}
		protected virtual void OnHeaderBorderTemplateInRibbonWindowChanged(ControlTemplate oldValue) {
			UpdateActualHeaderBorderTemplate();
		}
		protected virtual void OnStandaloneHeaderBorderTemplateChanged(ControlTemplate oldValue) {
			UpdateActualHeaderBorderTemplate();
		}
		protected virtual void OnRibbonStyleChanged(RibbonStyle oldValue) {
			UpdateActualApplicationButtonStyle();
			UpdateApplicationIconVisibility();
			UpdatePageHeaderExpandMode();
			UpdateToolbarRibbonStyle();
			UpdateToolbarPlacement();
			UpdateActualMinimizationButtonVisibility();
			CloseApplicationMenu();
		}
		protected virtual void OnIsMinimizedRibbonCollapsedChanged(bool oldValue) {
			UpdateCollapsedSelectedPageBorderVisibility();
		}
		protected virtual void OnIsBackstageViewOpenChanged(bool oldValue) {
			MinimizationButton.IsEnabled = !IsBackStageViewOpen;
			Toolbar.IsEnabled = !IsBackStageViewOpen;
			if (IsBackStageViewOpen) {
				if (AutoHideControl != null)
					AutoHideControl.SetCurrentValue(RibbonAutoHideControl.IsRibbonShownProperty, false);
				ShowApplicationMenu();
				RaiseBackstageOpened();
			} else {
				CloseApplicationMenu();
				RaiseBackstageClosed();
			}
		}
		protected virtual void OnSelectedPageNameChanged(DependencyPropertyChangedEventArgs e) {
			selectedPageNameLocker.DoIfNotLocked(() => {
				if (SelectedPageName == null && SelectedPage != null && !string.Equals(SelectedPage.Name, SelectedPageName))
					ClearValue(SelectedPageProperty);
				else if (!string.IsNullOrEmpty(SelectedPageName))
					this.SetCurrentValue(SelectedPageProperty, FindPageByName(SelectedPageName));
			});			
		}
		protected RibbonPage FindPageByName(string pageName) {
			return ActualCategories.SelectMany(x => x.ActualPages).FirstOrDefault(x => x.Name == pageName);			
		}
		protected virtual void SetHitTestVisible(bool value, bool affectApplicationButton) {
			Toolbar.Control.IsHitTestVisible = value;
			PageHeaderLinksControl.IsHitTestVisible = value;
			for(int i = 0; i < ActualCategories.Count; i++) {
				GetPageCategoryControl(i).IsHitTestVisible = value;
			}
			if(affectApplicationButton)
				ApplicationButton.IsHitTestVisible = value;
			if(MinimizationButton != null)
				MinimizationButton.IsHitTestVisible = value;
		}
		protected virtual void UpdatePageHeaderExpandMode() {
			PageHeaderLinksControl.SetExpandMode(RibbonStyle == RibbonStyle.TabletOffice ? BarPopupExpandMode.TabletOffice : BarPopupExpandMode.Classic);
		}
		public virtual void ExpandMinimizedRibbon() {
			if(IsMinimized)
				OpenPopup();
		}
		public virtual void CollapseMinimizedRibbon() {
			if(IsMinimized) {
				ClosePopup();
				PopupMenuManager.CloseAllPopups(null, null);
			}
		}
		protected internal virtual void OpenPopup() {
			Control ctrl = SelectedPagePopup.Child as Control;
			if(ctrl != null) {
				ctrl.FontSize = this.FontSize;
				ctrl.FontWeight = this.FontWeight;
				ctrl.FontStretch = this.FontStretch;
				ctrl.FontStyle = this.FontStyle;
				ctrl.FontFamily = this.FontFamily;
			}
			SelectedPagePopup.HorizontalOffset = SelectedPagePopupMargin.Left;
			SelectedPagePopup.VerticalOffset = SelectedPagePopupMargin.Top;
			SelectedPagePopup.Width = GetPopupWidth();
			SelectedPagePopup.ShowPopup(HeaderAndTabsGrid);
			SelectedPageControl.InvalidateMeasure();
		}
		protected virtual void ClosePopup() {
			SelectedPagePopup.IsOpen = false;
		}
		protected virtual void UpdateToolbarRibbonStyle() {
			if(Toolbar != null)
				Toolbar.SetRibbonStyle(RibbonStyle);
		}
		protected virtual void UpdateToolbarPlacement() {
			if(FooterToolbarContainer == null || HeaderToolbarContainer == null)
				return;
			ClearToolbarContainers();
			switch (GetToolbarMode()) {
				case RibbonQuickAccessToolbarShowMode.ShowAbove:
					HeaderToolbarContainer.Content = Toolbar.Control;
					UpdateAboveToolbarByRibbonStyle();
					FooterToolbarContainer.Visibility = Visibility.Collapsed;
					break;
				case RibbonQuickAccessToolbarShowMode.ShowBelow:
					FooterToolbarContainer.Content = Toolbar.Control;
					Toolbar.SetPlacement(RibbonQuckAccessToolbarPlacement.Footer);
					FooterToolbarContainer.Visibility = Visibility.Visible;
					break;
				default:
					FooterToolbarContainer.Visibility = Visibility.Collapsed;
					break;
			}
			UpdateCollapsedSelectedPageBorderVisibility();
		}
		private void UpdateAboveToolbarByRibbonStyle() {
			ActualHeaderQuickAccessToolbarContainerStyle = GetHeaderQuickAccessToolbarContainerStyle(RibbonStyle);
			Toolbar.SetPlacement(GetRibbonQuickAccessToolbarPlacement(RibbonStyle));
		}
		RibbonQuckAccessToolbarPlacement GetRibbonQuickAccessToolbarPlacement(RibbonStyle style) {
			RibbonQuckAccessToolbarPlacement placement = IsAeroMode ? RibbonQuckAccessToolbarPlacement.AeroHeader : RibbonQuckAccessToolbarPlacement.Header;
			if(RibbonStyle == RibbonStyle.Office2007) {
				if(ShowApplicationButton)
					placement = IsAeroMode ? RibbonQuckAccessToolbarPlacement.AeroHeader : RibbonQuckAccessToolbarPlacement.Header;
				else
					placement = RibbonQuckAccessToolbarPlacement.Title;
			} else if(IsInRibbonWindow && RibbonStyle != RibbonStyle.TabletOffice) {
				placement = RibbonQuckAccessToolbarPlacement.Title;
			}
			return placement;
		}
		Style GetHeaderQuickAccessToolbarContainerStyle(RibbonStyle ribbonStyle) {
			switch (ribbonStyle) {
				case RibbonStyle.Office2007:
					return HeaderQuickAccessToolbarContainerOffice2007Style();
				case RibbonStyle.Office2010:
				case RibbonStyle.TabletOffice:
				case RibbonStyle.OfficeSlim:
					return HeaderQuickAccessToolbarContainerOffice2010Style();
				default:
					return null;
			}
		}
		Style HeaderQuickAccessToolbarContainerOffice2007Style() {
			if(ShowApplicationButton) {
				if(IsInRibbonWindow) {
					return IsAeroMode ? HeaderQAT2007ContainerStyleInAeroWindow : HeaderQAT2007ContainerStyleInRibbonWindow;
				} else
					return HeaderQAT2007ContainerStyle;
			} else {
				if(IsInRibbonWindow) {
					return IsAeroMode ? HeaderQATContainerStyleWithoutAppIconInAeroWindow : HeaderQATContainerStyleWithoutApplicationIcon;
				} else
					return HeaderQAT2010ContainerStyle;
			}
		}
		Style HeaderQuickAccessToolbarContainerOffice2010Style() {
			if(IsInRibbonWindow)
				return IsAeroMode ? HeaderQAT2010ContainerStyleInAeroWindow : HeaderQAT2010ContainerStyleInRibbonWindow;
			return HeaderQAT2010ContainerStyle;
		}
		protected virtual void UpdateActualApplicationButtonLargeIcon() {
			ActualApplicationButtonLargeIcon = ApplicationButtonLargeIcon;;
		}
		protected virtual void UpdateActualApplicationButtonSmallIcon() {
			ActualApplicationButtonSmallIcon = ApplicationButtonSmallIcon;;
		}
		protected virtual void UpdateActualIsApplicationTextVisible() {
			ActualIsApplicationButtonTextVisible = !string.IsNullOrEmpty(ApplicationButtonText);
		}
		protected virtual void OnToolbarShowModeChanged(DependencyPropertyChangedEventArgs e) {
			UpdateToolbarPlacement();
			RaiseToolbarModeChanging(new PropertyChangedEventArgs("ToolbarShowMode"));
		}
		protected virtual void OnToolbarShowCustomizationButtonChanged(bool oldValue) {
			Toolbar.Control.ShowCustomizationButton = ToolbarShowCustomizationButton;
		}
		protected virtual void OnRibbonTitleBarVisibilityChanged(RibbonTitleBarVisibility oldValue) {
		}
		protected virtual void OnMDIMergeStyleChanged(MDIMergeStyle oldValue) {
			UpdateActualVisibility();
		}
		protected virtual void OnRibbonHeaderVisibilityChanged(RibbonHeaderVisibility oldValue) {
			UpdateLayoutByRibbonHeaderVisibility();
		}
		protected virtual void OnMinimizationButtonVisibilityChanged(RibbonMinimizationButtonVisibility oldValue) {
			UpdateActualMinimizationButtonVisibility();
		}
		protected virtual void OnApplicationIconChanged(ImageSource oldValue) {
			UpdateActualApplicationIcon();
		}
		protected virtual void UpdateActualApplicationIcon() {
			ActualApplicationIcon = ApplicationIcon == null ? ImageHelper.GetImage("DXWindowIcon.ico") : ImageHelper.GetSmallIcon(ApplicationIcon);
		}
		protected virtual void UpdateActualMinimizationButtonVisibility() {
			IsMinimizationButtonVisible = CalculateIsMinimizationButtonVisible();
		}
		protected virtual bool CalculateIsMinimizationButtonVisible() {
			if (MinimizationButtonVisibility == RibbonMinimizationButtonVisibility.Auto) {
				return RibbonStyle != Ribbon.RibbonStyle.Office2007;
			} else
				return MinimizationButtonVisibility == RibbonMinimizationButtonVisibility.Visible;
		}
		bool IsAllCategoriesHidden() {
			return !ActualCategories.Where(cat => !cat.IsDefault).Any(cat => cat.IsVisible);
		}
		bool CanCollapseRibbonHeader() {
			if(IsInRibbonWindow)
				return false;
			if(RibbonStyle == RibbonStyle.Office2007 && ShowApplicationButton)
				return false;
			return true;
		}
		protected virtual void UpdateRibbonTitleBarVisibility() {
			if(CanCollapseRibbonHeader()) {
				if(RibbonTitleBarVisibility == RibbonTitleBarVisibility.Collapsed || (RibbonTitleBarVisibility == RibbonTitleBarVisibility.Auto && IsAllCategoriesHidden() && ToolbarShowMode != RibbonQuickAccessToolbarShowMode.ShowAbove)) {
					IsRibbonTitleBarActualVisible = false;
					return;
				}
			}
			IsRibbonTitleBarActualVisible = true;
		}
		protected virtual void UpdateActualWindowTitle() {
			ActualWindowTitle = WindowHelper.GetTitle();
		}
		Rect GetScreen(Point point) {
			foreach(Rect screenRect in ScreenHelper.GetScreenRects()) {				
				if(new Rect(screenRect.X, screenRect.Y, screenRect.Width - 1, screenRect.Height - 1).Contains(point))
					return screenRect;
			}
			return Rect.Empty;
		}
		public Rect GetMostAppropriateScreenRect() {
			Rect rect = new Rect(ScreenHelper.GetScreenPoint(this), ScreenHelper.GetScreenPoint(this, this.RenderSize.ToPoint()));
			Rect res = new Rect();
			Rect maxIntersection = new Rect();
			foreach(Rect screen in ScreenHelper.GetScreenRects()) {
				Rect intersection = new Rect(screen.X, screen.Y, screen.Width - 1, screen.Height - 1);
				intersection.Intersect(rect);
				if(!intersection.IsEmpty && (intersection.Width * intersection.Height > maxIntersection.Width * maxIntersection.Height)) {
					maxIntersection = intersection;
					res = screen;
				}
			}
			return res;
		}
		double GetPopupWidth() {
			Thickness popupMargin = SelectedPagePopupMargin;
			Point position = PointToScreen(new Point());
			var screenRect = GetMostAppropriateScreenRect();
			if(screenRect.Height * screenRect.Width == 0d)
				screenRect = ScreenHelper.GetScreenRects()[0];
			if(FlowDirection == FlowDirection.RightToLeft) {
				position.X = position.X - ActualWidth;
			}
			var result = position.X - screenRect.X;
			if(result < 0) {
				result = ActualWidth + result;
				popupMargin.Left = 0d;
			} else if(result >= 0 && ActualWidth + result <= screenRect.Width) {
				result = ActualWidth;
			} else if(result + ActualWidth / 2 > screenRect.Width) {
				Rect nextScreenRect = GetScreen(new Point(position.X + ActualWidth, position.Y));
				result = nextScreenRect == Rect.Empty ? screenRect.Width - result : (result + ActualWidth) % screenRect.Width;
				popupMargin.Right = 0d;
			} else {
				result = screenRect.Width - result;
			}
			return Math.Max(result - popupMargin.Left - popupMargin.Right, 0d);
		}		
		#region overrides
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected void ClearToolbarContainers() {
			if(FooterToolbarContainer != null)
				FooterToolbarContainer.Content = null;
			if(HeaderToolbarContainer != null)
				HeaderToolbarContainer.Content = null;
		}
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();
			ClearToolbarContainers();
			if(PageHeaderLinksControlContainer != null)
				PageHeaderLinksControlContainer.Content = null;
			if (SelectedPageControlContainer != null)
				SelectedPageControlContainer.Content = null;
			base.OnApplyTemplate();
			SelectedPageControlContainer = GetTemplateChild("PART_SelectedPageControlContainer") as ContentControl;
			CollapsedSelectedPageBorder = (ContentControl)GetTemplateChild("PART_CollapsedSelectedPageBorder");
			TitlePanel = GetTemplateChild("PART_TitlePanel") as DXRibbonWindowTitlePanel;
			UpdateSelectedPageControlPlacement();
			HeaderToolbarContainer = (ContentControl)GetTemplateChild("PART_ToolbarContainer");
			FooterToolbarContainer = (ContentControl)GetTemplateChild("PART_FooterToolbarContainer");
			ApplicationButton = (RibbonApplicationButtonControl)GetTemplateChild("PART_ApplicationButton");
			RightTabContainer = GetTemplateChild("PART_RightTabContainer") as Grid;
			CategoriesPane = (RibbonPageCategoriesPane)GetTemplateChild("PART_PageCategoriesPane");
			HeaderBorder = (UIElement)GetTemplateChild("PART_HeaderBorder");
			ApplicationIconContainer = (ContentControl)this.GetTemplateChild("PART_ApplicationIconContainer");
			MainGrid = GetTemplateChild("PART_MainLayout") as Grid;
			HeaderAndTabsGrid = GetTemplateChild("PART_HeaderAndTabsLayout") as Grid;
			MinimizationButton = GetTemplateChild("PART_MinimizationButton") as RibbonCheckedBorderControl;
			PageHeaderLinksControlContainer = (DXContentPresenter)this.GetTemplateChild("PART_PageHeaderLinksControlContainer");
			ControlBoxContainer = GetTemplateChild("PART_ControlBox") as FrameworkElement;
			if(PageHeaderLinksControlContainer != null)
				PageHeaderLinksControlContainer.Content = PageHeaderLinksControl;
			UpdateLayoutByRibbonHeaderVisibility();
			SubscribeTemplateEvents();
			UpdateToolbarPlacement();
			MDIChildHost = LayoutHelper.FindParentObject<Bars.IMDIChildHost>(this);
			UpdateCollapsedSelectedPageBorderVisibility();
			AutoHideControl = GetTemplateChild("PART_AutoHideControl") as RibbonAutoHideControl;
			WindowHelper.MinimizeWindowButton = GetTemplateChild("PART_MinimizeButton") as Button;
			WindowHelper.RestoreWindowButton = GetTemplateChild("PART_RestoreButton") as Button;
			WindowHelper.CloseWindowButton = GetTemplateChild("PART_CloseButton") as Button;
			ApplicationMenuStrategy.Do(x => x.Close());
		}
		protected void UpdateLayoutByRibbonHeaderVisibility() {
			WindowHelper.UpdateRibbonVisibility();
		}
		protected virtual internal void SelectFirstVisiblePage() {
			SelectFirstVisiblePage(false);
		}
		protected virtual internal void SelectFirstVisiblePage(bool ignoreSelectedPageValue) {
			if(SelectedPage == null && !ignoreSelectedPageValue)
				return;
			SelectedPage = GetFirstSelectablePage();
		}
		public RibbonPage GetFirstSelectablePage() {
			return ActualCategories.Where(cat => cat.IsVisible).Select(cat => cat.GetFirstSelectablePage()).Where(page => page != null).FirstOrDefault();
		}
		protected internal virtual void OnPageCategoryInserted(RibbonPageCategoryBase pageCategory, int index) {
			if(pageCategory.IsDefault) {
				if(DefaultCategory != null)
					throw new ArgumentException("Ribbon control must have only one DefaultRibbonPageCategory.");
				else
					DefaultCategory = pageCategory;
			}
			pageCategory.Ribbon = this;
			pageCategory.PageIsSelectedChanged += new EventHandler(OnPageIsSelectedChanged);
			pageCategory.PageInserted += new RibbonPageInsertedEventHandler(OnPageInserted);
			pageCategory.PageRemoved += new RibbonPageRemovedEventHandler(OnPageRemoved);
			pageCategory.PageGroupInserted += new RibbonPageGroupInsertedEventHandler(OnPageGroupInserted);
			pageCategory.PageGroupRemoved += new RibbonPageGroupRemovedEventHandler(OnPageGroupRemoved);
			AddOrRemoveObjectWithItemsSourcePropertyUsed(pageCategory, pageCategory.PagesSource != null);
			if(pageCategory.Parent == null)
				AddLogicalChild(pageCategory);
			ActualCategories.Insert(index, pageCategory);
			if(SelectedPage == null) {
				SelectedPage = pageCategory.GetFirstSelectablePage();
			}
		}
		protected override IEnumerator LogicalChildren {
			get {				
				var lst = new List<object>();
				if (ApplicationMenu != null)
					lst.Add(ApplicationMenu);
				if(RibbonPopupMenu != null)
					lst.Add(RibbonPopupMenu);
				if(SelectedPagePopup != null)
					lst.Add(SelectedPagePopup);
				foreach(RibbonPageCategoryBase category in Categories)
					lst.Add(category);
				if(PageHeaderLinksControl != null)
					lst.Add(PageHeaderLinksControl);
				if(Toolbar != null)
					lst.Add(Toolbar);
				lst.AddRange(logicalChildrenContainerItems);
				return lst.GetEnumerator();
			}
		}
		protected virtual void OnPageGroupRemoved(object sender, RibbonPageGroupRemovedEventArgs e) {
			AddOrRemoveObjectWithItemsSourcePropertyUsed(e.RibbonPageGroup, false);
			RaiseRibbonPageGroupRemoved(e.RibbonPageGroup, e.Index);
		}
		protected virtual void OnPageGroupInserted(object sender, RibbonPageGroupInsertedEventArgs e) {
			AddOrRemoveObjectWithItemsSourcePropertyUsed(e.RibbonPageGroup, e.RibbonPageGroup.ItemLinksSource != null);
			RaiseRibbonPageGroupInserted(e.RibbonPageGroup, e.Index);
			var group = sender as RibbonPageGroup;
		}
		protected virtual void OnPageRemoved(object sender, RibbonPageRemovedEventArgs e) {
			AddOrRemoveObjectWithItemsSourcePropertyUsed(e.RibbonPage, false);
			RaiseRibbonPageRemoved(e.RibbonPage, e.Index);
		}
		protected virtual void OnPageInserted(object sender, RibbonPageInsertedEventArgs e) {
			if(CategoriesPane != null) CategoriesPane.InvalidateMeasure();
			AddOrRemoveObjectWithItemsSourcePropertyUsed(e.RibbonPage, e.RibbonPage.GroupsSource != null);
			if(SelectedPage == null) SetCurrentValue(SelectedPageProperty, sender);
			RaiseRibbonPageInserted(e.RibbonPage, e.Index);
			var page = sender as RibbonPage;
		}
		protected internal virtual void OnPageCategoryRemoved(RibbonPageCategoryBase pageCategory, int index) {
			if(pageCategory == DefaultCategory)
				DefaultCategory = null;
			pageCategory.PageIsSelectedChanged -= OnPageIsSelectedChanged;
			pageCategory.Ribbon = null;
			AddOrRemoveObjectWithItemsSourcePropertyUsed(pageCategory, false);
			pageCategory.PageInserted -= new RibbonPageInsertedEventHandler(OnPageInserted);
			pageCategory.PageRemoved -= new RibbonPageRemovedEventHandler(OnPageRemoved);
			pageCategory.PageGroupInserted -= new RibbonPageGroupInsertedEventHandler(OnPageGroupInserted);
			pageCategory.PageGroupRemoved -= new RibbonPageGroupRemovedEventHandler(OnPageGroupRemoved);
			if(pageCategory.Parent == this) RemoveLogicalChild(pageCategory);
			ActualCategories.Remove(pageCategory);
			if(pageCategory.Pages.Contains(SelectedPage))
				SelectedPage = GetFirstSelectablePage();
		}
		protected override void OnPreviewMouseUp(MouseButtonEventArgs e) {
			if (e.ChangedButton == MouseButton.Right && (ApplicationMenuStrategy == null || !ApplicationMenuStrategy.IsMenuOpened)) {
				e.Handled = ShowRibbonPopupMenu(e.OriginalSource as DependencyObject);
			}
			base.OnPreviewMouseUp(e);
		}
		protected virtual void UpdatePagesIsSelectedCore(RibbonPage basePage) {
			if (basePage!=null && !basePage.IsSelected)
				return;
			foreach(RibbonPageCategoryBase cat in ActualCategories) {
				bool categoryIsSelected = false;
				cat.RaisePageIsSelectedChanged = false;
				foreach(RibbonPage page in cat.ActualPagesCore) {
					if (page != basePage && basePage.With(x => x.MergedParent) != page && !(page.IsSelectedChangedProcessing && page.MergedParentCategory != null && page.IsSelected)) {
						page.SetCurrentValue(RibbonPage.IsSelectedProperty, false);
					}
					if(page.IsSelected)
						categoryIsSelected = true;
				}
				cat.IsSelected = categoryIsSelected;
				cat.RaisePageIsSelectedChanged = true;
			}			
		}
		protected internal virtual void OnPageIsSelectedChanged(object sender, EventArgs e) {
			RibbonPage page = sender as RibbonPage;			
			if(!page.IsSelected)
				page = GetSelectedPage();
			RibbonControl ribbon = ActualMergedParent ?? this;
			if (page == null && ribbon.SelectedPage.If(x => x.IsSelected) != null)
				return;
			ribbon.UpdatePagesIsSelectedCore(page);
			ribbon.SetCurrentValue(SelectedPageProperty, page);
		}
		RibbonPage GetSelectedPage() {
			var selectedPage = ActualCategories.FirstOrDefault(cat => cat.IsSelected).With(cat => cat.GetSelectedPage());
			return selectedPage ?? ActualMergedParent.With(x => x.GetSelectedPage());
		}
		protected internal void OnPreviewTextInput(object sender, TextCompositionEventArgs e) {
			if(KeyboardNavigationManager.IsNavigationMode) {
				e.Handled = KeyboardNavigationManager.ProcessChar(e.Text);
			}
		}
		protected void OnPreviewKeyDown(object sender, KeyEventArgs e) {			
			if (KeyboardNavigationManager.IsNavigationMode) {
				e.Handled = KeyboardNavigationManager.Navigate(RightToLeftHelper.TransposeKey(e.Key, this), 0);
			} else {
				if (IsKeyboardFocusWithin && e.Key == Key.Escape) {
					KeyboardNavigationManager.RestoreFocus();
				}
			}
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			AddLogicalChild(Toolbar);
			AddLogicalChild(PageHeaderLinksControl);
			AddLogicalChild(RibbonPopupMenu);
			if(!string.IsNullOrEmpty(SelectedPageName))
				SelectedPage = FindPageByName(SelectedPageName);
			if(SelectedPage == null)
				SelectedPage = GetFirstSelectablePage();
		}
		#endregion
		internal void UpdateIsInRibbonWindow() {
			IsInRibbonWindow = WindowHelper.HasRibbonWindowAsParent;
		}
		protected virtual BarItemLinkControlBase GetParentLinkControl(DependencyObject obj) {
			if(obj is BarItemLinkInfo)
				return (obj as BarItemLinkInfo).LinkControl;
			return LayoutHelper.FindLayoutOrVisualParentObject<BarItemLinkControlBase>(obj, true);
		}
		protected virtual void InitializePopupMenu(BarItemLinkControlBase targetLinkControl, BarItemLinkBase targetLink, BarItem targetItem, bool isLinkControlInToolbar) {
			RibbonPopupMenu.PlacementTarget = this;
			RibbonPopupMenu.StaysOpen = true;
			RibbonPopupMenu.Placement = PlacementMode.MousePoint;
			if(isLinkControlInToolbar) {
					if(targetLinkControl is BarItemLinkControl) {
						RemoveFromToolbarMenuItem.Glyph = ((BarItemLinkControl)targetLinkControl).ActualGlyph;
					}
					RemoveFromToolbarMenuItem.TargetItemLink = targetLink;
					AddRibbonPopupMenuItem(RemoveFromToolbarMenuItem);
					RibbonPopupMenu.ItemLinks.Add(new BarItemLinkSeparator());
			} else {
				if(targetLinkControl is BarItemLinkControl && !(targetLinkControl is RibbonGalleryBarItemLinkControl) && ToolbarShowMode != RibbonQuickAccessToolbarShowMode.Hide) {
					AddToToolbarMenuItem.Glyph = ((BarItemLinkControl)targetLinkControl).ActualGlyph;
					AddToToolbarMenuItem.TargetItem = targetItem;
					AddToToolbarMenuItem.TargetItemLink = targetLink;
					AddToToolbarMenuItem.IsEnabled = !((ILinksHolder)Toolbar).ActualLinks.Contains(targetItem) 
						|| Toolbar.ItemLinks.OfType<BarItemLink>().FirstOrDefault(x => x.Item == targetItem && x.CommonBarItemCollectionLink && !x.IsVisible).ReturnSuccess();
					AddRibbonPopupMenuItem(AddToToolbarMenuItem);
					RibbonPopupMenu.ItemLinks.Add(new BarItemLinkSeparator());
				}
			}
			if (RibbonHeaderVisibility == RibbonHeaderVisibility.Visible) {
				if (ToolbarShowMode == RibbonQuickAccessToolbarShowMode.ShowAbove) {
					AddRibbonPopupMenuItem(ShowToolbarBelowRibbonMenuItem);
					RibbonPopupMenu.ItemLinks.Add(new BarItemLinkSeparator());
				}
				if (ToolbarShowMode == RibbonQuickAccessToolbarShowMode.ShowBelow) {
					AddRibbonPopupMenuItem(ShowToolbarAboveRibbonMenuItem);
					RibbonPopupMenu.ItemLinks.Add(new BarItemLinkSeparator());
				}
				if (!IsMinimized && AllowMinimizeRibbon) {
					MinimizeRibbonMenuItem.IsChecked = false;
					AddRibbonPopupMenuItem(MinimizeRibbonMenuItem);
				}
			}
			if(IsMinimized) {
				MinimizeRibbonMenuItem.IsChecked = true;
				AddRibbonPopupMenuItem(MinimizeRibbonMenuItem);
			}
			if(AllowCustomization && !IsItemsSourceModeUsedWithin) {
				RibbonPopupMenu.ItemLinks.Add(new BarItemLinkSeparator());
				AddRibbonPopupMenuItem(CustomizeRibbonMenuItem);
			}
		}
		protected virtual void RemoveRibbonPopupMenuItems() {
			RemoveRibbonPopupMenuItem(AddToToolbarMenuItem);
			RemoveRibbonPopupMenuItem(RemoveFromToolbarMenuItem);
			RemoveRibbonPopupMenuItem(MinimizeRibbonMenuItem);
			RemoveRibbonPopupMenuItem(ShowToolbarAboveRibbonMenuItem);
			RemoveRibbonPopupMenuItem(ShowToolbarBelowRibbonMenuItem);
			RemoveRibbonPopupMenuItem(CustomizeRibbonMenuItem);
		}
		protected virtual void ClearRibbonPopupMenu() {
			RibbonPopupMenu.ItemLinks.Clear();
			RemoveRibbonPopupMenuItems();
		}
		bool RibbonPopupMenuIsOpening;
		void OnRibbonPopupMenuClosed(object sender, EventArgs e) {
			RibbonPopupMenu.Closed -= new EventHandler(OnRibbonPopupMenuClosed);
			ClearRibbonPopupMenu();
			RibbonPopupMenuIsOpening = false;
		}
		protected virtual bool IsLinkControlInToolbar(BarItemLinkControlBase linkControl) {
			if(linkControl == null)
				return false;
			RibbonQuickAccessToolbarPopupBorderControl border = LayoutHelper.FindParentObject<RibbonQuickAccessToolbarPopupBorderControl>(linkControl);
			if(border != null)
				return true;
			RibbonQuickAccessToolbarControl toolbarControl = LayoutHelper.FindParentObject<RibbonQuickAccessToolbarControl>(linkControl);
			if(toolbarControl != null)
				return true;
			return false;
		}
		protected internal virtual bool ShowRibbonPopupMenu(DependencyObject targetControl) {
			if(RibbonPopupMenu != null && targetControl != null && LayoutHelper.IsChildElement(RibbonPopupMenu.Child, targetControl))
				return false;
			if (LayoutHelper.GetRootPath(this, targetControl).OfType<UIElement>().Any(x => BarManager.GetDXContextMenu((UIElement)x) != null))
				return false;
			if(RibbonPopupMenuIsOpening) {
				RibbonPopupMenu.Closed -= OnRibbonPopupMenuClosed;
				ClearRibbonPopupMenu();
			}
			BarItemLinkControlBase targetLinkControl = GetTargetLinkControlForRibbonPopupMenu(targetControl);
			BarItem item = null;
			BarItemLinkBase itemLink = null;
			bool isLinkControlInToolbar = false;
			if(targetLinkControl != null) {
				if(targetLinkControl is BarEditItemLinkControl || targetLinkControl is BarButtonGroupLinkControl)
					return false;
				isLinkControlInToolbar = IsLinkControlInToolbar(targetLinkControl);
				itemLink = targetLinkControl.LinkBase;
				if(targetLinkControl.LinkInfo != null) {
					item = targetLinkControl.LinkInfo.Item;
				}
			}
			InitializePopupMenu(targetLinkControl, itemLink, item, isLinkControlInToolbar);
			RibbonPopupMenuIsOpening = true;
			RibbonPopupMenuShowingEventArgs e = new RibbonPopupMenuShowingEventArgs(RibbonPopupMenu, item, itemLink, isLinkControlInToolbar);
			RaiseRibbonPopupMenuShowing(e);
			if(!e.Cancel && RibbonPopupMenu.ItemLinks.Count != 0) {
				RibbonPopupMenu.ShowPopup(RibbonPopupMenu.PlacementTarget);
				if(RibbonPopupMenu.IsOpen == true) {
					RibbonPopupMenu.Closed += new EventHandler(OnRibbonPopupMenuClosed);
					return true;
				}
			}
			return false;
		}
		protected virtual BarItemLinkControlBase GetTargetLinkControlForRibbonPopupMenu(DependencyObject targetControl) {
			BarItemLinkControlBase targetLinkControl = GetParentLinkControl(targetControl);
			bool allowToAddInQat = targetLinkControl != null && targetLinkControl.LinkInfo != null && targetLinkControl.LinkInfo.Item != null && GetAllowAddingToToolbar(targetLinkControl.LinkInfo.Item);
			if(!allowToAddInQat)
				targetLinkControl = null;
			return targetLinkControl;
		}
		RibbonPopupMenu ribbonPopupMenuCore = null;
		protected internal RibbonPopupMenu RibbonPopupMenu {
			get {
				if(ribbonPopupMenuCore == null) {
					ribbonPopupMenuCore = new RibbonPopupMenu(this);
				}
				return ribbonPopupMenuCore;
			}
		}
		protected virtual void UpdateActualApplicationButtonStyle() {
			IsOffice2010Style = RibbonStyle == RibbonStyle.Office2010 || RibbonStyle == RibbonStyle.OfficeSlim;
			ActualApplicationButtonStyle = GetApplicationButtonStyle(RibbonStyle);
		}
		protected virtual Style GetApplicationButtonStyle(RibbonStyle RibbonStyle) {
			switch(RibbonStyle) {
				case RibbonStyle.Office2010:
					return ApplicationButton2010Style;
				case RibbonStyle.TabletOffice:
					return ApplicationButtonTabletOfficeStyle;
				case RibbonStyle.OfficeSlim:
					return ApplicationButtonOfficeSlimStyle;
				case RibbonStyle.Office2007:
					return IsAeroMode ? ApplicationButton2007StyleInAeroWindow : ApplicationButton2007Style;
			}
			return null;
		}
		protected virtual void UpdateApplicationIconVisibility() {
			if (ActualApplicationIcon == null || !IsInRibbonWindow || (WindowHelper.RibbonWindow != null && !WindowHelper.RibbonWindow.ShowIcon))
				IsApplicationIconVisible = false;
			else if (RibbonStyle == Ribbon.RibbonStyle.Office2007)
				IsApplicationIconVisible = !ShowApplicationButton;
			else
				IsApplicationIconVisible = true;
		}
		protected virtual void IsInRibbonWindowChanged(bool oldValue) {
			CoerceValue(AutoHideModeProperty);
			IsAeroMode = WindowHelper.IsAeroMode;
			UpdateApplicationIconVisibility();
			UpdateAboveToolbarByRibbonStyle();
		}
		protected virtual void OnAeroTemplateChanged(ControlTemplate oldValue) {
			UpdateTemplate();
		}
		protected virtual void OnDefaultTemplateChanged(ControlTemplate oldValue) {
			UpdateTemplate();
		}
		protected virtual void OnAeroBorderRibbonStyleChanged(RibbonStyle oldValue) {
			InvalidateMeasure();
		}
		protected virtual void OnAeroBorderTopOffsetChanged(double oldValue) {
			InvalidateMeasure();
		}
		protected virtual void OnCollapsedRibbonAeroBorderTopOffsetChanged(double oldValue) {
			InvalidateMeasure();
		}
		protected virtual void IsAeroModeChanged(bool oldValue) {
			CoerceValue(AutoHideModeProperty);
			UpdateTemplate();
			UpdateActualApplicationButtonStyle();
		}
		protected virtual void OnApplicationButton2007StyleInAeroWindowChanged(Style oldValue) {
			UpdateActualApplicationButtonStyle();
		}
		protected virtual void UpdateTemplate() {
			Template = IsAeroMode ? AeroTemplate : DefaultTemplate;
		}
		protected ApplicationMenuStrategyBase ApplicationMenuStrategy {
			get { return applicationMenuStrategy; }
			private set {
				if (applicationMenuStrategy == value)
					return;
				var oldValue = applicationMenuStrategy;
				applicationMenuStrategy = value;
				OnApplicationMenuStrategyChanged(oldValue);
			}
		}
		protected virtual void OnApplicationMenuStrategyChanged(ApplicationMenuStrategyBase oldValue) {
			if (oldValue != null)
				oldValue.Dispose();
		}
		ApplicationMenuStrategyBase applicationMenuStrategy;
		protected virtual void OnApplicationMenuChanged(object oldValue) {
			ApplicationMenuStrategy = ApplicationMenuStrategyBase.CreateStrategy(this, ApplicationMenu);
			var args = new RoutedValueChangedEventArgs<DependencyObject>(ApplicationMenuChangedEvent, (DependencyObject)oldValue, ApplicationMenu);
			RaiseEvent(args);
		}
		protected virtual bool OnEnterMenuMode(object sender, EventArgs args) {
			return ToggleMenuMode();
		}
		protected virtual bool ToggleMenuMode() {
			bool isWindowActive = Window.GetWindow(this).Return(x => x.IsActive, () => false);
			if(isWindowActive && AllowKeyTips && IsVisible && !this.IsInDesignTool()) {
				if (PopupMenuManager.IsAnyPopupOpened) {
					PopupMenuManager.CloseAllPopups(null, null);
					return true;
				}
				KeyboardNavigationManager.IsNavigationMode = true;
				return true;
			}
			return false;
		}
		internal bool CanDragRibbonWindow() {
			return IsInRibbonWindow && ApplicationIconContainer != null && !ApplicationIconContainer.IsMouseOver;
		}
		void IRibbonControl.Merge(object child, ILinksHolder extraItems) {
			Merge((RibbonControl)child);
			if(extraItems != null)
				((ILinksHolder)PageHeaderLinksControl).Merge(extraItems);
		}
		void IRibbonControl.UnMerge(object child, ILinksHolder extraItems) {
			if(extraItems != null)
				((ILinksHolder)PageHeaderLinksControl).UnMerge(extraItems);
			UnMerge((RibbonControl)child);
		}
		MDIMergeStyle IRibbonControl.GetMDIMergeStyle() {
			return MDIMergeStyle;
		}
		public bool IsChild {
			get { return MergedParent != null; }
		}
		protected virtual internal UserControl CreateCustomizationControl() {
			return new DevExpress.Xpf.Ribbon.Customization.CustomizationControl(this);
		}
		protected virtual internal void AdjustCustomizationForm(FloatingContainer customizationForm) { }		
		#region ILogicalChildrenContainer
#if DEBUGTEST
		internal
#endif
		readonly List<object> logicalChildrenContainerItems = new List<object>();
		void ILogicalChildrenContainer.AddLogicalChild(object child) {
			logicalChildrenContainerItems.Add(child);
			AddLogicalChild(child);
		}
		void ILogicalChildrenContainer.RemoveLogicalChild(object child) {
			logicalChildrenContainerItems.Remove(child);
			RemoveLogicalChild(child);
		}
		#endregion
		#region IMultipleElementRegistratorSupport Members
		object IMultipleElementRegistratorSupport.GetName(object registratorKey) {
			if(Equals(typeof(IMergingSupport),registratorKey)){
				return MergingProperties.GetName(this).WithString(x => x) ?? MergingPropertiesHelper.RibbonID;
			}
			if(Equals(typeof(IFrameworkInputElement), registratorKey) || Equals(typeof(IEventListenerClient), registratorKey)) {
				return Name;
			}
			throw new ArgumentException();
		}
		IEnumerable<object> IMultipleElementRegistratorSupport.RegistratorKeys {
			get { return new object[] { typeof(IFrameworkInputElement), typeof(IMergingSupport), typeof(IEventListenerClient) }; }
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			MergingPropertiesHelper.OnPropertyChanged(this, e, MergingPropertiesHelper.RibbonID);
			if(Equals(FrameworkElement.NameProperty, e.Property)) {
				BarNameScope.GetService<IElementRegistratorService>(this).NameChanged(this, typeof(IFrameworkInputElement), e.OldValue, e.NewValue);
			}
		}
		#endregion		
		#region IMergingSupport Members
		bool IMergingSupport.CanMerge(IMergingSupport second) { return MDIMergeStyle!=MDIMergeStyle.Never && ((RibbonControl)second).MDIMergeStyle!=MDIMergeStyle.Never; }
		bool IMergingSupport.IsMerged { get { return MergedParent != null; } }
		bool IMergingSupport.IsMergedParent(IMergingSupport second) { return MergedParent == second; }
		bool IMergingSupport.IsAutomaticallyMerged { get; set; }
		object IMergingSupport.MergingKey { get { return typeof(RibbonControl); } }
		void IMergingSupport.Merge(IMergingSupport second) {
			Merge((RibbonControl)second);						
		}
		void IMergingSupport.Unmerge(IMergingSupport second) {			
			UnMerge((RibbonControl)second);			
		}
		bool IEventListenerClient.ReceiveEvent(object sender, EventArgs e) {
			var args = e as RoutedEventArgs;
			if (args == null)
				return false;
			if(args.RoutedEvent == MenuModeHelper.EnterMenuModeEvent) {				
				return OnEnterMenuMode(sender, e);
			}
			return false;
		}
		#endregion
#if DEBUGTEST
		public static class RibbonTestHelper {
			public static ApplicationMenuStrategyBase GetApplicationMenuStrategy(RibbonControl ribbon) {
				return ribbon.ApplicationMenuStrategy;
			}
			public static RibbonAutoHideControl GetAutoHideControl(RibbonControl ribbon) {
				return ribbon.AutoHideControl;
			}
		}
#endif
		#region IRuntimeCustomizationHost Members
		DependencyObject IRuntimeCustomizationHost.FindTarget(string targetName) {
			return BarNameScope.GetService<IElementRegistratorService>(this)
				.GetElements<IFrameworkInputElement>(ScopeSearchSettings.Local).OfType<DependencyObject>()
				.FirstOrDefault(x => BarManagerCustomizationHelper.GetSerializationName(x) == targetName) as DependencyObject;
		}
		RuntimeCustomizationCollection runtimeCustomizations;
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true, false, 0, XtraSerializationFlags.None)]
		public RuntimeCustomizationCollection RuntimeCustomizations { get { return runtimeCustomizations ?? (runtimeCustomizations = new RuntimeCustomizationCollection(this)); } }
		#endregion
	}
	public class RibbonSelectedPageContentPresenter : ContentControl {
		public RibbonSelectedPageContentPresenter() {
			Focusable = false;
			IsTabStop = false;
			HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
			HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			base.OnContentChanged(oldContent, newContent);
			InvalidateMeasure();
		}
	}
	public class RibbonObjectCreator : ObjectCreator {
		protected override System.Reflection.Assembly GetAssembly() {
			return typeof(RibbonControl).Assembly;
		}
	}
}
