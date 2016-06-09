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
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Xpf.Ribbon.Themes;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Data;
using System.Collections.ObjectModel;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using System.Collections;
namespace DevExpress.Xpf.Ribbon {
	public class ApplicationMenu : PopupMenu, IApplicationMenu {
		#region static
		public static readonly DependencyProperty RightPaneProperty;
		public static readonly DependencyProperty BottomPaneProperty;
		public static readonly DependencyProperty ShowRightPaneProperty;
		public static readonly DependencyProperty RightPaneWidthProperty;
		public static readonly DependencyProperty RibbonStyleProperty;
		protected internal static readonly DependencyPropertyKey RibbonStylePropertyKey;
		static ApplicationMenu() { 
			RightPaneProperty = DependencyPropertyManager.Register("RightPane", typeof(FrameworkElement), typeof(ApplicationMenu), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			BottomPaneProperty = DependencyPropertyManager.Register("BottomPane", typeof(FrameworkElement), typeof(ApplicationMenu), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ShowRightPaneProperty = DependencyPropertyManager.Register("ShowRightPane", typeof(bool), typeof(ApplicationMenu), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, (d,e)=>((ApplicationMenu)d).OnShowRightPaneChanged(e)));
			RightPaneWidthProperty = DependencyPropertyManager.Register("RightPaneWidth", typeof(double), typeof(ApplicationMenu), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
			RibbonStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("RibbonStyle", typeof(RibbonStyle), typeof(ApplicationMenu),
				new PropertyMetadata(RibbonStyle.Office2007, OnRibbonStylePropertyChanged));
			RibbonStyleProperty = RibbonStylePropertyKey.DependencyProperty;
		}
		static void OnRibbonStylePropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((ApplicationMenu)obj).OnRibbonStyleChanged((RibbonStyle)e.OldValue);
		}
		#endregion
		#region dep props
		public FrameworkElement RightPane {
			get { return (FrameworkElement)GetValue(RightPaneProperty); }
			set { SetValue(RightPaneProperty, value); }
		}
		public RibbonStyle RibbonStyle {
			get { return (RibbonStyle)GetValue(RibbonStyleProperty); }
			protected internal set { this.SetValue(RibbonStylePropertyKey, value); }
		}
		public FrameworkElement BottomPane {
			get { return (FrameworkElement)GetValue(BottomPaneProperty); }
			set { SetValue(BottomPaneProperty, value); }
		}
		public bool ShowRightPane {
			get { return (bool)GetValue(ShowRightPaneProperty); }
			set { SetValue(ShowRightPaneProperty, value); }
		}
		public double RightPaneWidth {
			get { return (double)GetValue(RightPaneWidthProperty); }
			set { SetValue(RightPaneWidthProperty, value); }
		}
		#endregion
		public ApplicationMenu() :
			base() { }
		SpacingMode spacingMode;
		protected internal SpacingMode SpacingMode {
			get { return spacingMode; }
			set {
				if (value == spacingMode)
					return;
				SpacingMode oldValue = spacingMode;
				spacingMode = value;
				OnSpacingModeChanged(oldValue);
			}
		}
		protected virtual void OnSpacingModeChanged(SpacingMode oldValue) {
			BarControl.SpacingMode = SpacingMode;
		}
		protected override DevExpress.Utils.DefaultBoolean GetIngoreMenuDropAlignment() {
			return DevExpress.Utils.DefaultBoolean.True;
		}
		public RibbonControl Ribbon { get; internal protected set; }
		protected internal DXContentPresenter RightPaneControl { get { return ((ApplicationMenuBarControl)ContentControl).RightPane; } }
		protected override LinkContainerType GetLinkContainerType() {
			return LinkContainerType.ApplicationMenu;
		}
		protected override PopupBorderControl CreateBorderControl() {
			return new ApplicationMenuPopupBorderControl(this);
		}
		protected internal ApplicationMenuBarControl BarControl { get { return PopupContent as ApplicationMenuBarControl; } }
		protected override object CreatePopupContent() {
			return new ApplicationMenuBarControl(this);
		}
		protected override GlyphSize GetDefaultItemsGlyphSizeCore(LinkContainerType linkContainerType) {
			return GlyphSize.Large;
		}
		protected virtual void OnRibbonStyleChanged(RibbonStyle oldValue) {
			((ApplicationMenuBarControl)PopupContent).RibbonStyle = RibbonStyle;
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			OnRibbonStyleChanged(RibbonStyle);
			UpdateApplicationButtonPosition();
		}
		protected override void OnOpened(EventArgs e) {
			base.OnOpened(e);
			UpdateApplicationButtonPosition();
		}
		protected override void UpdatePlacement(UIElement control) {
			base.UpdatePlacement(control);
			var appButton = control as RibbonApplicationButtonControl;
			if(appButton == null) return;
			Point offset = appButton.TranslatePoint(new Point(), Ribbon);
			Placement = PlacementMode.Bottom;
			double horizontalOffset = 
				Ribbon.IsAeroMode ? Ribbon.ApplicationMenuHorizontalOffsetInAeroWindow : Ribbon.ApplicationMenuHorizontalOffset;
			double verticalOffset =
				Ribbon.IsAeroMode ? Ribbon.ApplicationMenuVerticalOffsetInAeroWindow: Ribbon.ApplicationMenuVerticalOffset;
			HorizontalOffset = horizontalOffset - offset.X;
			VerticalOffset = RibbonStyle != RibbonStyle.Office2007 ? -verticalOffset : (int)appButton.ActualHeight / -2;
		}
		protected override void OnActualLinksChanged() {
			BarControl.LinksHolder = null;
			BarControl.LinksHolder = this;
		}
		internal bool Show(RibbonApplicationButtonControl appButton) {
			Ribbon = appButton.Ribbon;
			PlacementTarget = appButton;
			RibbonStyle = Ribbon.RibbonStyle;
			if(IsMenuEmpty)
				return false;
			ShowPopup(appButton);
			return IsOpen;
		}
		#region IApplicationMenu Members
		UIElement IApplicationMenu.GetRightPaneContainer() {
			if(!ShowRightPane || RightPaneWidth == 0)
				return null;
			return BarControl.RightPane;
		}
		#endregion
		protected override bool ShowDescriptionCore() {
			return ItemsDisplayMode == PopupMenuItemsDisplayMode.Default || ItemsDisplayMode == PopupMenuItemsDisplayMode.LargeImagesTextDescription;
		}
		public virtual bool IsMenuEmpty {
			get {
				return !HasVisibleItems();
			}
		}
		void UpdateApplicationButtonPosition() {
			ApplicationMenuPopupBorderControl border = Child as ApplicationMenuPopupBorderControl;
			if(Ribbon == null || !IsOpen || border.ApplicationButton == null)
				return;
			UpdatePlacement(PlacementTarget);
			border.UpdateAppButtonProperties(IsOpen);
			border.UpdateAppButtonPosition(PlacementTarget as FrameworkElement);
			UpdateVerticalOffset();
		}
		protected internal void UpdateVerticalOffset() {
			FrameworkElement target = PlacementTarget as FrameworkElement;
			FrameworkElement child = Child as FrameworkElement;
			if(target == null || child == null) return;
			Point targetOffset = ScreenHelper.GetScreenPoint(target);
			Rect rect = ScreenHelper.GetScreenRect(child);
			Point offset = ScreenHelper.GetScreenPoint(child);
			if(targetOffset.Y + child.ActualHeight + target.ActualHeight > rect.Height)
				VerticalOffset = 0d;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateApplicationButtonPosition();
		}
		protected internal virtual void OnShowRightPaneChanged(DependencyPropertyChangedEventArgs e) {
			if(BarControl==null)
				return;
			BarControl.UpdateLeftPaneGridColumns((bool)e.NewValue);
		}
	}
	[Obsolete("ApplicationMenuInfo is no longer needed. Please replace ApplicationMenuInfo with ApplicationMenu.")]
	[ContentProperty("Items")]
	public class ApplicationMenuInfo : PopupInfo<ApplicationMenu> {
		const string ObsoleteMessage = "ApplicationMenuInfo is no longer needed. Please replace ApplicationMenuInfo with ApplicationMenu.";
		#region static
		public static readonly DependencyProperty RightPaneProperty;
		public static readonly DependencyProperty BottomPaneProperty;
		public static readonly DependencyProperty ShowRightPaneProperty;
		public static readonly DependencyProperty RightPaneWidthProperty;
		public static readonly DependencyProperty ItemLinksSourceProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemLinksAttachedBehaviorProperty;  
		static ApplicationMenuInfo() { 
			RightPaneProperty = DependencyPropertyManager.Register("RightPane", typeof(FrameworkElement), typeof(ApplicationMenuInfo), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			BottomPaneProperty = DependencyPropertyManager.Register("BottomPane", typeof(FrameworkElement), typeof(ApplicationMenuInfo), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ShowRightPaneProperty = DependencyPropertyManager.Register("ShowRightPane", typeof(bool), typeof(ApplicationMenuInfo), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
			RightPaneWidthProperty = DependencyPropertyManager.Register("RightPaneWidth", typeof(double), typeof(ApplicationMenuInfo), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
			ItemLinksSourceProperty = DependencyProperty.Register("ItemLinksSource", typeof(IEnumerable), typeof(ApplicationMenuInfo), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksSourcePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(ApplicationMenuInfo), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(ApplicationMenuInfo), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplateSelectorPropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(ApplicationMenuInfo), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemLinksTemplatePropertyChanged)));
			ItemLinksAttachedBehaviorProperty = DependencyPropertyManager.RegisterAttached("ItemLinksAttachedBehavior", typeof(ItemsAttachedBehaviorCore<ApplicationMenuInfo, BarItem>), typeof(ApplicationMenuInfo), new UIPropertyMetadata(null));
		}
		protected static void OnItemLinksSourcePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((ApplicationMenuInfo)d).OnItemLinksSourceChanged(e);
		}
		protected static void OnItemLinksTemplatePropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((ApplicationMenuInfo)d).OnItemLinksTemplateChanged(e);
		}
		protected static void OnItemLinksTemplateSelectorPropertyChanged(DependencyObject d, System.Windows.DependencyPropertyChangedEventArgs e) {
			((ApplicationMenuInfo)d).OnItemLinksTemplateSelectorChanged(e);
		}
		#endregion
		#region dep props
		public FrameworkElement RightPane {
			get { return (FrameworkElement)GetValue(RightPaneProperty); }
			set { SetValue(RightPaneProperty, value); }
		}
		public FrameworkElement BottomPane {
			get { return (FrameworkElement)GetValue(BottomPaneProperty); }
			set { SetValue(BottomPaneProperty, value); }
		}
		public bool ShowRightPane {
			get { return (bool)GetValue(ShowRightPaneProperty); }
			set { SetValue(ShowRightPaneProperty, value); }
		}
		public double RightPaneWidth {
			get { return (double)GetValue(RightPaneWidthProperty); }
			set { SetValue(RightPaneWidthProperty, value); }
		}
		public IEnumerable ItemLinksSource {
			get { return (IEnumerable)GetValue(ItemLinksSourceProperty); }
			set { SetValue(ItemLinksSourceProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		#endregion
		public ApplicationMenuInfo() : base() {
			this.links = new ObservableCollection<BarItemLinkBase>();
			this.links.CollectionChanged += OnLinkCollectionChanged;
			CreateBindings();
			throw new Exception(ObsoleteMessage);
		}
		ObservableCollection<BarItemLinkBase> links;
		public ObservableCollection<BarItemLinkBase> ItemLinks { get { return links; } }
		protected virtual void OnLinkCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			Popup.ItemLinks.Clear();
			foreach(BarItemLinkBase link in ItemLinks)
				Popup.ItemLinks.Add(link);
		}
		protected virtual void CreateBindings() {
			Binding bnd = new Binding("RightPane") { Source = this };
			bnd.Mode = BindingMode.TwoWay;
			Popup.SetBinding(ApplicationMenu.RightPaneProperty, bnd);
			bnd = new Binding("BottomPane") { Source = this };
			bnd.Mode = BindingMode.TwoWay;
			Popup.SetBinding(ApplicationMenu.BottomPaneProperty, bnd);
			bnd = new Binding("ShowRightPane") { Source = this };
			bnd.Mode = BindingMode.TwoWay;
			Popup.SetBinding(ApplicationMenu.ShowRightPaneProperty, bnd);
			bnd = new Binding("RightPaneWidth") { Source = this };
			bnd.Mode = BindingMode.TwoWay;
			Popup.SetBinding(ApplicationMenu.RightPaneWidthProperty, bnd);
		}
				private void OnItemLinksTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<ApplicationMenuInfo, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this,
				e, ItemLinksAttachedBehaviorProperty);
		}
		private void OnItemLinksTemplateSelectorChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<ApplicationMenuInfo, BarItem>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemLinksAttachedBehaviorProperty);
		}
		protected virtual void OnItemLinksSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<ApplicationMenuInfo, BarItem>.OnItemsSourcePropertyChanged(this,
				e,
				ItemLinksAttachedBehaviorProperty,
				ItemTemplateProperty,
				ItemTemplateSelectorProperty,
				ItemStyleProperty,
				subItem => subItem.ItemLinks,
				subItem => new BarButtonItem(),
				(index, item) => ItemLinks.Insert(index, (item as BarItem).CreateLink()), useDefaultTemplateSelector:true);
		}	
		public RibbonControl Ribbon {
			get {
				return Popup.Ribbon;
			}
			set {
				Popup.Ribbon = value;
			}
		}
		protected override ApplicationMenu CreatePopup() {
			return new ApplicationMenu();
		}
		internal ApplicationMenu Menu {
			get {
				return Popup;
			}
		}
	}
}
