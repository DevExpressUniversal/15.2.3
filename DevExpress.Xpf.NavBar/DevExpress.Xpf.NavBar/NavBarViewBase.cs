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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.NavBar.Automation;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.NavBar {
	public enum NavBarViewKind {
		ExplorerBar,
		NavigationPane,
		SideBar
	}
	public class NavBarItemSelectedEventArgs : RoutedEventArgs {
		public NavBarItemSelectedEventArgs(NavBarGroup group, NavBarItem item) {
			Group = group;
			Item = item;
		}
		public NavBarGroup Group { get; internal set; }
		public NavBarItem Item { get; internal set; }
	}
	public class NavBarItemSelectingEventArgs : RoutedEventArgs {
		public NavBarItemSelectingEventArgs(NavBarGroup prevGroup, NavBarItem prevItem, NavBarGroup newGroup, NavBarItem newItem) {
			PrevGroup = prevGroup;
			PrevItem = prevItem;
			NewGroup = newGroup;
			NewItem = newItem;
		}
		public bool Cancel { get; set; }
		public NavBarGroup PrevGroup { get; internal set; }
		public NavBarItem PrevItem { get; internal set; }
		public NavBarGroup NewGroup { get; internal set; }
		public NavBarItem NewItem { get; internal set; }
	}
	public class NavBarActiveGroupChangedEventArgs : RoutedEventArgs {
		public NavBarActiveGroupChangedEventArgs(NavBarGroup group) {
			Group = group;
		}
		public NavBarGroup Group { get; internal set; }
	}
	public class NavBarActiveGroupChangingEventArgs : RoutedEventArgs {
		public NavBarActiveGroupChangingEventArgs(NavBarGroup prevGroup, NavBarGroup newGroup) {
			PrevGroup = prevGroup;
			NewGroup = newGroup;
		}
		public bool Cancel { get; set; }
		public NavBarGroup PrevGroup { get; internal set; }
		public NavBarGroup NewGroup { get; internal set; }
	}
	public delegate void NavBarItemSelectedEventHandler(object sender, NavBarItemSelectedEventArgs e);
	public delegate void NavBarItemSelectingEventHandler(object sender, NavBarItemSelectingEventArgs e);
	public delegate void NavBarActiveGroupChangedEventHandler(object sender, NavBarActiveGroupChangedEventArgs e);
	public delegate void NavBarActiveGroupChangingEventHandler(object sender, NavBarActiveGroupChangingEventArgs e);
	[TemplateVisualState(Name = "Vertical", GroupName = "OrientationStates")]
	[TemplateVisualState(Name = "Horizontal", GroupName = "OrientationStates")]
	public abstract partial class NavBarViewBase : Control
		{		
		public static readonly DependencyProperty ShowBorderProperty;		
		public static readonly DependencyProperty HeaderTemplateProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemControlTemplateProperty;
		public static readonly DependencyProperty HeaderTemplateSelectorProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ItemsPanelTemplateProperty;
		public static readonly DependencyProperty ItemsPanelOrientationProperty;
		public static readonly DependencyProperty NavBarViewKindProperty;
		public static readonly DependencyProperty GroupVisualStyleProperty;
		public static readonly DependencyProperty ItemVisualStyleProperty;
		public static readonly DependencyProperty ImageSettingsProperty;
		public static readonly DependencyProperty LayoutSettingsProperty;
		public static readonly DependencyProperty DisplayModeProperty;
		public static readonly DependencyProperty GroupDisplayModeProperty;
		public static readonly DependencyProperty GroupLayoutSettingsProperty;
		public static readonly DependencyProperty GroupImageSettingsProperty;
		public static readonly DependencyProperty ItemDisplayModeProperty;
		public static readonly DependencyProperty ItemLayoutSettingsProperty;
		public static readonly DependencyProperty ItemImageSettingsProperty;
		public static readonly DependencyProperty GroupFontSettingsProperty;
		public static readonly DependencyProperty ItemFontSettingsProperty;
		public static readonly DependencyProperty FontSettingsProperty;		
		static readonly DependencyPropertyKey NavBarPropertyKey;
		public static readonly DependencyProperty NavBarProperty;
		public static RoutedEvent ItemSelectingEvent;
		public static RoutedEvent ItemSelectedEvent;
		public static RoutedEvent ActiveGroupChangingEvent;
		public static RoutedEvent ActiveGroupChangedEvent;
		public static readonly DependencyProperty GroupHeaderTemplateProperty;
		public static readonly DependencyProperty AnimateGroupExpansionProperty;
		public static readonly DependencyProperty ItemForegroundProperty;
		static NavBarViewBase() {
			HeaderTemplateProperty = DependencyPropertyManager.Register("HeaderTemplate", typeof(DataTemplate), typeof(NavBarViewBase), new PropertyMetadata(null, OnHeaderTemplateChanged));
			HeaderTemplateSelectorProperty = DependencyPropertyManager.Register("HeaderTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarViewBase), new PropertyMetadata(null, OnHeaderTemplateSelectorChanged));
			ItemTemplateProperty = DependencyPropertyManager.Register("ItemTemplate", typeof(DataTemplate), typeof(NavBarViewBase), new PropertyMetadata(null, OnItemTemplateChanged));
			ItemControlTemplateProperty = DependencyPropertyManager.Register("ItemControlTemplate", typeof(ControlTemplate), typeof(NavBarViewBase), new PropertyMetadata(null));
			ItemTemplateSelectorProperty = DependencyPropertyManager.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(NavBarViewBase), new PropertyMetadata(null, OnItemTemplateSelectorChanged));
			OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(NavBarViewBase), new PropertyMetadata(Orientation.Vertical, (d, e) => ((NavBarViewBase)d).OnOrientationPropertyChanged()));
			ItemsPanelTemplateProperty = DependencyPropertyManager.Register("ItemsPanelTemplate", typeof(ItemsPanelTemplate), typeof(NavBarViewBase), new PropertyMetadata(null));
			ItemsPanelOrientationProperty = DependencyPropertyManager.Register("ItemsPanelOrientation", typeof(Orientation), typeof(NavBarViewBase), new PropertyMetadata(Orientation.Vertical));
			ItemSelectingEvent = EventManager.RegisterRoutedEvent("ItemSelectingEvent", RoutingStrategy.Direct, typeof(NavBarItemSelectingEventHandler), typeof(NavBarViewBase));
			ItemSelectedEvent = EventManager.RegisterRoutedEvent("ItemSelectedEvent", RoutingStrategy.Direct, typeof(NavBarItemSelectedEventHandler), typeof(NavBarViewBase));
			ActiveGroupChangingEvent = EventManager.RegisterRoutedEvent("ActiveGroupChangingEvent", RoutingStrategy.Direct, typeof(NavBarActiveGroupChangingEventHandler), typeof(NavBarViewBase));
			ActiveGroupChangedEvent = EventManager.RegisterRoutedEvent("ActiveGroupChangedEvent", RoutingStrategy.Direct, typeof(NavBarActiveGroupChangedEventHandler), typeof(NavBarViewBase));
			NavBarViewKindProperty = DependencyPropertyManager.Register("NavBarViewKind", typeof(NavBarViewKind), typeof(NavBarViewBase), new PropertyMetadata(NavBarViewKind.ExplorerBar));
			ImageSettingsProperty = DependencyPropertyManager.RegisterAttached("ImageSettings", typeof(ImageSettings), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null));
			LayoutSettingsProperty = DependencyPropertyManager.RegisterAttached("LayoutSettings", typeof(LayoutSettings), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null));
			DisplayModeProperty = DependencyPropertyManager.RegisterAttached("DisplayMode", typeof(DisplayMode), typeof(NavBarViewBase), new FrameworkPropertyMetadata(DisplayMode.ImageAndText));
			GroupVisualStyleProperty = DependencyPropertyManager.Register("GroupVisualStyle", typeof(Style), typeof(NavBarViewBase), new PropertyMetadata(null, OnGroupVisualStyleChanged));
			ItemVisualStyleProperty = DependencyPropertyManager.Register("ItemVisualStyle", typeof(Style), typeof(NavBarViewBase), new PropertyMetadata(null, OnItemVisualStyleChanged));
			GroupHeaderTemplateProperty = DependencyPropertyManager.Register("GroupHeaderTemplate", typeof(ControlTemplate), typeof(NavBarViewBase), new PropertyMetadata(null, OnGroupHeaderTemplateChanged));
			NavBarPropertyKey = DependencyPropertyManager.RegisterReadOnly("NavBar", typeof(NavBarControl), typeof(NavBarViewBase), new PropertyMetadata(null, new PropertyChangedCallback(OnNavBarPropertyChanged)));
			NavBarProperty = NavBarPropertyKey.DependencyProperty;
			AnimateGroupExpansionProperty = DependencyPropertyManager.Register("AnimateGroupExpansion", typeof(bool), typeof(NavBarViewBase), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnAnimateGroupExpansionPropertyChanged)));
			CommandManager.RegisterClassCommandBinding(typeof(NavBarViewBase), new CommandBinding(NavBarCommands.ChangeGroupExpanded, new ExecutedRoutedEventHandler(OnChangeGroupExpanded)));
			CommandManager.RegisterClassCommandBinding(typeof(NavBarViewBase), new CommandBinding(NavBarCommands.SetActiveGroup, new ExecutedRoutedEventHandler(OnSetActiveGroup)));
			CommandManager.RegisterClassCommandBinding(typeof(NavBarViewBase), new CommandBinding(NavBarCommands.SelectItem, new ExecutedRoutedEventHandler(OnSelectItem)));
			ClickEvent = EventManager.RegisterRoutedEvent("ClickEvent", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(NavBarViewBase));
			ScrollingSettings.IsManipulationEnabledProperty.OverrideMetadata(typeof(NavBarViewBase), new FrameworkPropertyMetadata(true));
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(NavBarViewBase), typeof(NavBarViewAutomationPeer), owner => new NavBarViewAutomationPeer((NavBarViewBase)owner));
			FocusableProperty.OverrideMetadata(typeof(NavBarViewBase), new FrameworkPropertyMetadata(false));
			GroupDisplayModeProperty = DependencyPropertyManager.Register("GroupDisplayMode", typeof(DisplayMode), typeof(NavBarViewBase), new FrameworkPropertyMetadata(DisplayMode.Default, new PropertyChangedCallback(OnGroupDisplayModePropertyChanged)));
			GroupLayoutSettingsProperty = DependencyPropertyManager.Register("GroupLayoutSettings", typeof(LayoutSettings), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupLayoutSettingsPropertyChanged)));
			GroupImageSettingsProperty = DependencyPropertyManager.Register("GroupImageSettings", typeof(ImageSettings), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupImageSettingsPropertyChanged)));
			ItemDisplayModeProperty = DependencyPropertyManager.Register("ItemDisplayMode", typeof(DisplayMode), typeof(NavBarViewBase), new FrameworkPropertyMetadata(DisplayMode.Default, new PropertyChangedCallback(OnItemDisplayModePropertyChanged)));
			ItemLayoutSettingsProperty = DependencyPropertyManager.Register("ItemLayoutSettings", typeof(LayoutSettings), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemLayoutSettingsPropertyChanged)));
			ItemImageSettingsProperty = DependencyPropertyManager.Register("ItemImageSettings", typeof(ImageSettings), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemImageSettingsPropertyChanged)));
			GroupFontSettingsProperty = DependencyPropertyManager.Register("GroupFontSettings", typeof(FontSettings), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupFontSettingsPropertyChanged)));
			ItemFontSettingsProperty = DependencyPropertyManager.Register("ItemFontSettings", typeof(FontSettings), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnItemFontSettingsPropertyChanged)));
			FontSettingsProperty = DependencyPropertyManager.RegisterAttached("FontSettings", typeof(FontSettings), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnFontSettingsPropertyChanged)));
			ShowBorderProperty = DependencyPropertyManager.Register("ShowBorder", typeof(bool), typeof(NavBarViewBase), new FrameworkPropertyMetadata(true));
			ForegroundProperty.OverrideMetadata(typeof(NavBarViewBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, (d, e) => d.CoerceValue(ForegroundProperty)));
			ItemForegroundProperty = DependencyPropertyManager.Register("ItemForeground", typeof(Brush), typeof(NavBarViewBase), new FrameworkPropertyMetadata(null, new CoerceValueCallback((d, e) => e.Return(x => x, () => ((NavBarViewBase)d).Foreground))));
		}
		static void OnNavBarPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).OnNavBarChanged(e);
		}
		static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).UpdateActualHeaderTemplateSelector();
		}
		static void OnHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).UpdateActualHeaderTemplateSelector();
		}
		static void OnItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).UpdateActualItemTemplateSelector();
		}
		static void OnItemTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).UpdateActualItemTemplateSelector();
		}
		static void OnGroupVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).UpdateActualGroupVisualStyle();
		}
		static void OnItemVisualStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).UpdateActualItemVisualStyle();
		}
		public static void OnGroupHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).UpdateActualGroupHeaderTemplate();
		}
		protected static void OnAnimateGroupExpansionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).UpdateActualAnimateGroupExpansion();
		}		
		public static void OnChangeGroupExpanded(object sender, ExecutedRoutedEventArgs e) {
			((NavBarViewBase)sender).ChangeGroupExpanded((NavBarGroup)e.Parameter);
		}
		public static void OnSetActiveGroup(object sender, ExecutedRoutedEventArgs e) {
			((NavBarViewBase)sender).SetActiveGroup((NavBarGroup)e.Parameter);
		}
		public static void OnSelectItem(object sender, ExecutedRoutedEventArgs e) {
			((NavBarViewBase)sender).SelectItem((NavBarItem)e.Parameter);
		}
		public static void SetImageSettings(DependencyObject d, ImageSettings settings) {
			d.SetValue(ImageSettingsProperty, settings);
		}
		public static ImageSettings GetImageSettings(DependencyObject d) {
			return (ImageSettings)d.GetValue(ImageSettingsProperty);
		}
		public static void SetLayoutSettings(DependencyObject d, LayoutSettings settings) {
			d.SetValue(LayoutSettingsProperty, settings);
		}
		public static LayoutSettings GetLayoutSettings(DependencyObject d) {
			return (LayoutSettings)d.GetValue(LayoutSettingsProperty);
		}
		public static void SetDisplayMode(DependencyObject d, DisplayMode settings) {
			d.SetValue(DisplayModeProperty, settings);
		}
		public static DisplayMode GetDisplayMode(DependencyObject d) {
			return (DisplayMode)d.GetValue(DisplayModeProperty);
		}
		public static FontSettings GetFontSettings(DependencyObject obj) {
			return (FontSettings)obj.GetValue(FontWeightProperty);
		}
		public static void SetFontSettings(DependencyObject obj, FontSettings value) {
			obj.SetValue(FontSettingsProperty, value);
		}
		protected static void OnGroupDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).OnGroupDisplayModeChanged((DisplayMode)e.OldValue);
		}
		protected static void OnGroupLayoutSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).OnGroupLayoutSettingsChanged((LayoutSettings)e.OldValue);
		}
		protected static void OnGroupImageSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).OnGroupImageSettingsChanged((ImageSettings)e.OldValue);
		}
		protected static void OnItemDisplayModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).OnItemDisplayModeChanged((DisplayMode)e.OldValue);
		}
		protected static void OnItemLayoutSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).OnItemLayoutSettingsChanged((LayoutSettings)e.OldValue);
		}
		protected static void OnItemImageSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).OnItemImageSettingsChanged((ImageSettings)e.OldValue);
		}
		protected static void OnGroupFontSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).OnGroupFontSettingsChanged((FontSettings)e.OldValue);
		}
		protected static void OnItemFontSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((NavBarViewBase)d).OnItemFontSettingsChanged((FontSettings)e.OldValue);
		}
		protected static void OnFontSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(!(d is System.Windows.Controls.Control)) return;
			FontSettings oldValue = (FontSettings)e.OldValue;
			FontSettings newValue = (FontSettings)e.NewValue;
			if(oldValue != null) {
				oldValue.RemoveOwner((System.Windows.Controls.Control)d);
			}
			if(newValue != null) {
				newValue.AddOwner((System.Windows.Controls.Control)d);
			}
		}		
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseHeaderTemplate"),
#endif
 Category(Categories.Templates)]
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}		
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseHeaderTemplateSelector"),
#endif
 Category(Categories.Templates)]
		public DataTemplateSelector HeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseItemTemplate"),
#endif
 Category(Categories.Templates)]
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseItemControlTemplate"),
#endif
 Category(Categories.Templates)]
		public ControlTemplate ItemControlTemplate {
			get { return (ControlTemplate)GetValue(ItemControlTemplateProperty); }
			set { SetValue(ItemControlTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseItemTemplateSelector"),
#endif
 Category(Categories.Templates)]
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseShowBorder")]
#endif
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseOrientation"),
#endif
 Category(Categories.Appearance)]
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseItemsPanelTemplate"),
#endif
 Category(Categories.Templates)]
		public ItemsPanelTemplate ItemsPanelTemplate {
			get { return (ItemsPanelTemplate)GetValue(ItemsPanelTemplateProperty); }
			set { SetValue(ItemsPanelTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseItemsPanelOrientation"),
#endif
 Category(Categories.Appearance)]
		public Orientation ItemsPanelOrientation {
			get { return (Orientation)GetValue(ItemsPanelOrientationProperty); }
			set { SetValue(ItemsPanelOrientationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseGroupHeaderTemplate"),
#endif
 Category(Categories.Templates)]
		public ControlTemplate GroupHeaderTemplate {
			get { return (ControlTemplate)GetValue(GroupHeaderTemplateProperty); }
			set { SetValue(GroupHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseGroupVisualStyle"),
#endif
 Category(Categories.Appearance)]
		public Style GroupVisualStyle {
			get { return (Style)GetValue(GroupVisualStyleProperty); }
			set { SetValue(GroupVisualStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseItemVisualStyle"),
#endif
 Category(Categories.Appearance)]
		public Style ItemVisualStyle {
			get { return (Style)GetValue(ItemVisualStyleProperty); }
			set { SetValue(ItemVisualStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseNavBar")]
#endif
		public NavBarControl NavBar {
			get { return (NavBarControl)GetValue(NavBarProperty); }
			internal set { this.SetValue(NavBarPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseNavBarViewKind")]
#endif
		public NavBarViewKind NavBarViewKind {
			get { return (NavBarViewKind)GetValue(NavBarViewKindProperty); }
			protected set { SetValue(NavBarViewKindProperty, value); }
		}
#if !SL
	[DevExpressXpfNavBarLocalizedDescription("NavBarViewBaseAnimateGroupExpansion")]
#endif
		public bool AnimateGroupExpansion {
			get { return (bool)GetValue(AnimateGroupExpansionProperty); }
			set { SetValue(AnimateGroupExpansionProperty, value); }
		}
		public ImageSettings GroupImageSettings {
			get { return (ImageSettings)GetValue(GroupImageSettingsProperty); }
			set { SetValue(GroupImageSettingsProperty, value); }
		}
		public LayoutSettings GroupLayoutSettings {
			get { return (LayoutSettings)GetValue(GroupLayoutSettingsProperty); }
			set { SetValue(GroupLayoutSettingsProperty, value); }
		}
		public DisplayMode GroupDisplayMode {
			get { return (DisplayMode)GetValue(GroupDisplayModeProperty); }
			set { SetValue(GroupDisplayModeProperty, value); }
		}
		public FontSettings ItemFontSettings {
			get { return (FontSettings)GetValue(ItemFontSettingsProperty); }
			set { SetValue(ItemFontSettingsProperty, value); }
		}
		public ImageSettings ItemImageSettings {
			get { return (ImageSettings)GetValue(ItemImageSettingsProperty); }
			set { SetValue(ItemImageSettingsProperty, value); }
		}
		public LayoutSettings ItemLayoutSettings {
			get { return (LayoutSettings)GetValue(ItemLayoutSettingsProperty); }
			set { SetValue(ItemLayoutSettingsProperty, value); }
		}
		public DisplayMode ItemDisplayMode {
			get { return (DisplayMode)GetValue(ItemDisplayModeProperty); }
			set { SetValue(ItemDisplayModeProperty, value); }
		}
		public FontSettings GroupFontSettings {
			get { return (FontSettings)GetValue(GroupFontSettingsProperty); }
			set { SetValue(GroupFontSettingsProperty, value); }
		}
		public Brush ItemForeground {
			get { return (Brush)GetValue(ItemForegroundProperty); }
			set { SetValue(ItemForegroundProperty, value); }
		}
		GroupAddingEventHandler groupAdding;
		ItemAddingEventHandler itemAdding;
		public NavBarViewBase() {
			AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ButtonBaseClick));
		}
		#region hidden properties
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness BorderThickness { get { return base.BorderThickness; } set { base.BorderThickness = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Background { get { return base.Background; } set { base.Background = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush BorderBrush { get { return base.BorderBrush; } set { base.BorderBrush = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Foreground { get { return base.Foreground; } set { base.Foreground = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Padding { get { return base.Padding; } set { base.Padding = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HorizontalAlignment HorizontalContentAlignment { get { return base.HorizontalContentAlignment; } set { base.HorizontalContentAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new VerticalAlignment VerticalContentAlignment { get { return base.VerticalContentAlignment; } set { base.VerticalContentAlignment = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontStyle FontStyle { get { return base.FontStyle; } set { base.FontStyle = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontFamily FontFamily { get { return base.FontFamily; } set { base.FontFamily = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double FontSize { get { return base.FontSize; } set { base.FontSize = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontStretch FontStretch { get { return base.FontStretch; } set { base.FontStretch = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontWeight FontWeight { get { return base.FontWeight; } set { base.FontWeight = value; } }
		#endregion
		[Category(Categories.Selection)]
		public event NavBarItemSelectingEventHandler ItemSelecting {
			add { AddHandler(ItemSelectingEvent, value); }
			remove { RemoveHandler(ItemSelectingEvent, value); }
		}
		[Category(Categories.Selection)]
		public event NavBarItemSelectedEventHandler ItemSelected {
			add { AddHandler(ItemSelectedEvent, value); }
			remove { RemoveHandler(ItemSelectedEvent, value); }
		}
		public event NavBarActiveGroupChangingEventHandler ActiveGroupChanging {
			add { AddHandler(ActiveGroupChangingEvent, value); }
			remove { RemoveHandler(ActiveGroupChangingEvent, value); }
		}
		public event NavBarActiveGroupChangedEventHandler ActiveGroupChanged {
			add { AddHandler(ActiveGroupChangedEvent, value); }
			remove { RemoveHandler(ActiveGroupChangedEvent, value); }
		}
		[Category(Categories.Data)]
		public event GroupAddingEventHandler GroupAdding {
			add { groupAdding += value; }
			remove { groupAdding -= value; }
		}
		[Category(Categories.Data)]
		public event ItemAddingEventHandler ItemAdding {
			add { itemAdding += value; }
			remove { itemAdding -= value; }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			NavBarVisualStateHelper.UpdateStates(this, "OrientationStates");
			UpdateOrientationStates();
		}
		public void ChangeGroupExpanded(NavBarGroup group) {
			group.ChangeGroupExpanded();
		}
		public void SetActiveGroup(NavBarGroup group) {
			NavBar.ActiveGroup = group;
		}
		public void SelectItem(NavBarItem item) {
			if (item == null || item.Group == null)
				return;
			item.Group.SelectedItem = item;
		}
		public NavBarGroup GetNavBarGroup(System.Windows.RoutedEventArgs e) {
			return GetNavBarGroupInternal(e, false);
		}
		internal NavBarGroup GetNavBarGroupInternal(System.Windows.RoutedEventArgs e, bool inHeaderOnly) {
			object source = e.OriginalSource;
			if(!(source is DependencyObject))
				return null;
			DependencyObject dSource = source as DependencyObject;
			var groupHeader = !inHeaderOnly ? dSource : LayoutHelper.FindLayoutOrVisualParentObject(dSource, new Predicate<DependencyObject>(x => Internal.NavBarGroupHelper.GetIsGroupHeader(x)));
			if(groupHeader==null)
				return null;
			var groupControl = LayoutHelper.FindLayoutOrVisualParentObject<NavBarGroupControl>(groupHeader, true);
			return groupControl == null ? null : groupControl.Group;
		}
		public NavBarItem GetNavBarItem(System.Windows.RoutedEventArgs e) {
			object source = e.OriginalSource;
			var itemControl = source is DependencyObject ? DevExpress.Xpf.Core.Native.LayoutHelper.FindLayoutOrVisualParentObject<NavBarItemControl>(source as DependencyObject, true) : null;
			return itemControl == null ? null : itemControl.DataContext as NavBarItem;
		}
		internal T FindAncestor<T>(object obj) where T : class {
			return NavBarViewBase.FindAncestorCore<T>(obj);
		}
		internal static T FindAncestorCore<T>(object obj) where T : class {
			FrameworkElement element = obj as FrameworkElement;
			while(element != null) {
				if(element.DataContext != null && element.DataContext is T)
					return (T)element.DataContext;
				element = VisualTreeHelper.GetParent(element) as FrameworkElement;
			}
			return null;
		}
		protected internal virtual void OnOrientationPropertyChanged() {
			UpdateOrientationStates();
		}
		protected internal virtual void RaiseGroupAdding(NavBarGroup group, object sourceObject) {
			GroupAddingEventArgs e = new GroupAddingEventArgs(group, sourceObject);
			if(groupAdding != null)
				groupAdding(this, e);
		}
		protected internal virtual void RaiseItemAdding(NavBarItem item, object sourceObject) {
			ItemAddingEventArgs e = new ItemAddingEventArgs(item, sourceObject);
			if(itemAdding != null)
				itemAdding(this, e);
		}
		protected virtual void OnNavBarChanged(DependencyPropertyChangedEventArgs e) { }
		virtual internal void SetNavBar(NavBarControl navBar) {
			NavBar = navBar;
			UpdateActualHeaderTemplateSelector();
			UpdateActualItemTemplateSelector();
			UpdateActualGroupVisualStyle();
			UpdateActualGroupHeaderTemplate();
			UpdateActualItemVisualStyle();
			UpdateActualItemImageSettings();
			UpdateActualItemLayoutSettings();
			UpdateActualItemDisplayMode();
			UpdateActualItemFontSettings();
			UpdateActualGroupImageSettings();
			UpdateActualGroupLayoutSettings();
			UpdateActualGroupDisplayMode();
			UpdateActualGroupFontSettings();
		}
		protected internal virtual void UpdateView() { }
		protected internal virtual void UpdateViewForce() { }
		protected virtual void OnItemFontSettingsChanged(FontSettings oldValue) {
			UpdateActualItemFontSettings();
		}
		protected virtual void OnGroupFontSettingsChanged(FontSettings oldValue) {
			UpdateActualGroupFontSettings();
		}
		protected virtual void OnItemImageSettingsChanged(ImageSettings oldValue) {
			UpdateActualItemImageSettings();
		}
		protected virtual void OnItemLayoutSettingsChanged(LayoutSettings oldValue) {
			UpdateActualItemLayoutSettings();
		}
		protected virtual void OnItemDisplayModeChanged(DisplayMode oldValue) {
			UpdateActualItemDisplayMode();
		}
		protected virtual void OnGroupImageSettingsChanged(ImageSettings oldValue) {
			UpdateActualGroupImageSettings();
		}
		protected virtual void OnGroupLayoutSettingsChanged(LayoutSettings oldValue) {
			UpdateActualGroupLayoutSettings();
		}
		protected virtual void OnGroupDisplayModeChanged(DisplayMode oldValue) {
			UpdateActualGroupDisplayMode();
		}
		protected internal virtual void UpdateActualItemImageSettings() {
			if(NavBar == null) return;
			NavBar.UpdateGroups(group => group.UpdateActualItemImageSettings());
		}
		protected internal virtual void UpdateActualItemLayoutSettings() {
			if(NavBar == null) return;
			NavBar.UpdateGroups(group => group.UpdateActualItemLayoutSettings());
		}
		protected internal virtual void UpdateActualItemDisplayMode() {
			if(NavBar == null) return;
			NavBar.UpdateGroups(group => group.UpdateActualItemDisplayMode());
		}
		protected internal virtual void UpdateActualItemFontSettings() {
			if(NavBar == null) return;
			NavBar.UpdateGroups(group => group.UpdateActualItemFontSettings());
		}
		protected internal virtual void UpdateActualGroupFontSettings() {
			if(NavBar == null) return;
			NavBar.UpdateGroups(group => group.UpdateActualFontSettings());
		}
		protected internal virtual void UpdateActualGroupImageSettings() {
			if(NavBar == null) return;
			NavBar.UpdateGroups(group => group.UpdateActualImageSettings());
		}
		protected internal virtual void UpdateActualGroupLayoutSettings() {
			if(NavBar == null) return;
			NavBar.UpdateGroups(group => group.UpdateActualLayoutSettings());
		}
		protected internal virtual void UpdateActualGroupDisplayMode() {
			if(NavBar == null) return;
			NavBar.UpdateGroups(group => group.UpdateActualItemDisplayMode());
		}
		void UpdateActualHeaderTemplateSelector() {
			if (NavBar != null)
				NavBar.UpdateGroups((w) => w.UpdateActualHeaderTemplateSelector());
		}
		void UpdateActualItemTemplateSelector() {
			if (NavBar != null)
				NavBar.UpdateGroups((w) => w.UpdateSelectors());
		}
		void UpdateActualGroupVisualStyle() {
			if (NavBar != null)
				NavBar.UpdateGroups((w) => w.UpdateActualVisualStyle());
		}
		protected internal void UpdateActualItemVisualStyle() {
			if (NavBar != null)
				NavBar.UpdateGroups((w) => w.UpdateActualItemVisualStyle());
		}
		void UpdateActualGroupHeaderTemplate() {
			if(NavBar != null)
				NavBar.UpdateGroups((w) => w.UpdateActualGroupHeaderTemplate());
		}
		protected virtual void UpdateActualAnimateGroupExpansion() {
			if(NavBar != null)
				NavBar.UpdateGroups((w) => w.UpdateActualAnimateGroupExpansion());
		}
		void UpdateOrientationStates() {
			VisualStateManager.GoToState(this, Orientation == Orientation.Vertical ? "Vertical" : "Horizontal", false);
			UpdateLayout();
		}
		internal void ButtonBaseClick(object sender, System.Windows.RoutedEventArgs e) {
			NavBarGroup group = GetNavBarGroup(e);
			NavBarGroup groupHeader = GetNavBarGroupInternal(e, true);
			NavBarItem item = GetNavBarItem(e);			
			if (group != null && group.NavBar==NavBar)
				NavBar.SelectionStrategy.SelectGroup(group);
			RaiseEvent(new RoutedEventArgs(ClickEvent, e.OriginalSource));
			if(group != null && group.NavBar == NavBar) {
				if (item == null) {
					if (groupHeader != null)
					group.RaiseClickEvent();
				}
				else if(item.Group != null && item.Group.NavBar == NavBar) item.RaiseClickEvent();
			}
		}
	}
	internal class NavBarValueSelectorHelper {
		public static object GetValue(DependencyObject[] objs, DependencyProperty[] properties, object defaultValue) {
			for (int i = 0; i < objs.Count(); i++) {
				object value = objs[i] != null ? objs[i].GetValue(properties[i]) : null;
				if (value != null && !value.Equals(defaultValue))
					return value;
			}
			return defaultValue;
		}
	}
	public class IsActiveToAnimationProgressConverter : MarkupExtension, IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (bool)value ? 1.0 : 0.0;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
		#endregion
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
	public class NavBarViewPresenter : ContentControl {
		static NavBarViewPresenter() {
			FocusableProperty.OverrideMetadata(typeof(NavBarViewPresenter), new FrameworkPropertyMetadata(false));
		}
		public static readonly DependencyProperty ViewProperty =
			DependencyPropertyManager.Register("View", typeof(NavBarViewBase), typeof(NavBarViewPresenter),
			new PropertyMetadata((d, e) => ((NavBarViewPresenter)d).OnViewPropertyChanged()));
		public NavBarViewBase View {
			get { return (NavBarViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		protected virtual void OnViewPropertyChanged() {
			if(View != null)
				View.RemoveFromVisualTree();
			Content = View;
		}
	}
	[TemplateVisualState(Name = "Expanded", GroupName="ExpandStates")]
	[TemplateVisualState(Name = "Collapsed", GroupName = "ExpandStates")]
	public abstract class ExpandButtonBase : Button {
		internal static readonly DependencyProperty IsExpandedProperty;
		static ExpandButtonBase() {
			IsExpandedProperty = DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(ExpandButtonBase), new PropertyMetadata(true, (d, e) => ((ExpandButtonBase)d).OnIsExpandedPropertyChanged()));
		}
		internal bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetBindings();
			UpdateExpandStates();
		}
		protected internal abstract void SetBindings();
		protected internal virtual void OnIsExpandedPropertyChanged() {
			UpdateExpandStates();
		}
		void UpdateExpandStates() {
			VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Collapsed", false);
		}
	}
}
namespace DevExpress.Xpf.NavBar.Internal {
	public class NavBarGroupHelper {
		public static bool GetIsGroupHeader(DependencyObject obj) {
			return (bool)obj.GetValue(IsGroupHeaderProperty);
		}
		public static void SetIsGroupHeader(DependencyObject obj, bool value) {
			obj.SetValue(IsGroupHeaderProperty, value);
		}
		public static readonly DependencyProperty IsGroupHeaderProperty =
		DependencyPropertyManager.RegisterAttached("IsGroupHeader", typeof(bool), typeof(NavBarGroupHelper), new FrameworkPropertyMetadata(false));
	}
}
