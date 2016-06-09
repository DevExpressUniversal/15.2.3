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
using DevExpress.Xpf.Controls.Primitives;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.UIAutomation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.Xpf.WindowsUI {
	public enum AnimationType { Fade, SlideHorizontal, SlideVertical, None }
	public enum SelectorAnimationDirection { Forward, Back }
	public enum PageHeadersAlignment { Left, Top, Right, Bottom }
	public enum PageHeadersLayoutType { Default, Clip, Scroll }
#if !SILVERLIGHT
#endif
	[DevExpress.Xpf.Core.DXToolboxBrowsable]
	public class PageView : veContentSelector, ISupportInitialize {
		#region static
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty HeaderTemplateProperty;
		public static readonly DependencyProperty HeaderTemplateSelectorProperty;
		public static readonly DependencyProperty HasHeaderProperty;
		static readonly DependencyPropertyKey HasHeaderPropertyKey;
		public static readonly DependencyProperty AnimationTypeProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty AnimationDirectionProperty;
		static readonly DependencyPropertyKey AnimationDirectionPropertyKey;
		public static readonly DependencyProperty AnimationSpeedRatioProperty;
		public static readonly DependencyProperty BackCommandProperty;
		public static readonly DependencyProperty BackCommandParameterProperty;
		public static readonly DependencyProperty ShowBackButtonProperty;
		public static readonly DependencyProperty PageHeadersAlignmentProperty;
		public static readonly DependencyProperty PageHeadersLayoutTypeProperty;
		public static readonly DependencyProperty IsScrollableProperty;
		public static readonly DependencyProperty ItemCacheModeProperty;
		static readonly DependencyPropertyKey IsScrollablePropertyKey;
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty ItemsPanelHorizontalProperty =
			DependencyProperty.Register("ItemsPanelHorizontal", typeof(ItemsPanelTemplate), typeof(PageView), new PropertyMetadata(null));
		public static readonly DependencyProperty ItemsPanelVerticalProperty =
			DependencyProperty.Register("ItemsPanelVertical", typeof(ItemsPanelTemplate), typeof(PageView), new PropertyMetadata(null));
		public static readonly DependencyProperty TemplateLeftProperty =
			DependencyProperty.Register("TemplateLeft", typeof(ControlTemplate), typeof(PageView), new PropertyMetadata(null,
				(d, e) => ((PageView)d).UpdateTemplate(PageHeadersAlignment.Left)));
		public static readonly DependencyProperty TemplateRightProperty =
			DependencyProperty.Register("TemplateRight", typeof(ControlTemplate), typeof(PageView), new PropertyMetadata(null,
				(d, e) => ((PageView)d).UpdateTemplate(PageHeadersAlignment.Right)));
		public static readonly DependencyProperty TemplateTopProperty =
			DependencyProperty.Register("TemplateTop", typeof(ControlTemplate), typeof(PageView), new PropertyMetadata(null,
				(d, e) => ((PageView)d).UpdateTemplate(PageHeadersAlignment.Top)));
		public static readonly DependencyProperty TemplateBottomProperty =
			DependencyProperty.Register("TemplateBottom", typeof(ControlTemplate), typeof(PageView), new PropertyMetadata(null,
				(d, e) => ((PageView)d).UpdateTemplate(PageHeadersAlignment.Bottom)));
		static PageView() {
			var dProp = new DependencyPropertyRegistrator<PageView>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Header", ref HeaderProperty, (object)null,
				(d, e) => ((PageView)d).OnHeaderChanged(e.OldValue, e.NewValue));
			dProp.Register("HeaderTemplate", ref HeaderTemplateProperty, (DataTemplate)null);
			dProp.Register("HeaderTemplateSelector", ref HeaderTemplateSelectorProperty, (DataTemplateSelector)null);
			dProp.RegisterReadonly("HasHeader", ref HasHeaderPropertyKey, ref HasHeaderProperty, false);
			dProp.Register("ShowBackButton", ref ShowBackButtonProperty, true);
			dProp.Register("PageHeadersAlignment", ref PageHeadersAlignmentProperty, PageHeadersAlignment.Top, (d, e) => {
				((PageView)d).OnHeaderAlignmentChanged((PageHeadersAlignment)e.OldValue, (PageHeadersAlignment)e.NewValue);
			});
			dProp.Register("PageHeadersLayoutType", ref PageHeadersLayoutTypeProperty, PageHeadersLayoutType.Default, OnPageHeadersLayoutTypeChanged);
			dProp.RegisterReadonly("IsScrollable", ref IsScrollablePropertyKey, ref IsScrollableProperty, false);
#if DEBUGTEST
			AnimationType defaultAnimationType = AnimationType.None;
#else
			AnimationType defaultAnimationType = AnimationType.Fade;
#endif
			dProp.Register("AnimationType", ref AnimationTypeProperty, defaultAnimationType);
			dProp.RegisterReadonly("AnimationDirection", ref AnimationDirectionPropertyKey, ref AnimationDirectionProperty, SelectorAnimationDirection.Forward);
			dProp.Register("AnimationSpeedRatio", ref AnimationSpeedRatioProperty, 1d);
			dProp.Register("BackCommand", ref BackCommandProperty, (ICommand)null);
			dProp.Register("BackCommandParameter", ref BackCommandParameterProperty, (object)null);
			dProp.Register("ItemCacheMode", ref ItemCacheModeProperty, ItemCacheMode.None);
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal);
		}
		private static void OnPageHeadersLayoutTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PageView)d).OnPageHeadersLayoutTypeChanged((PageHeadersLayoutType)e.OldValue, (PageHeadersLayoutType)e.NewValue);
		}
		#endregion
		public PageView() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(PageView);
#endif
			ItemContainerGenerator.StatusChanged += OnItemContainerGeneratorStatusChanged;
		}
		void OnItemContainerGeneratorStatusChanged(object sender, EventArgs e) {
			if(ItemContainerGenerator.Status == System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated) {
				if(ContentPanelInitializeOperation != null) {
					ContentPanelInitializeOperation.Abort();
					InitializeContentPanel();
				}
			}
		}
		void OnHeaderAlignmentChanged(PageHeadersAlignment oldValue, PageHeadersAlignment newValue) {
			SetValue(OrientationProperty, newValue == WindowsUI.PageHeadersAlignment.Left || newValue == WindowsUI.PageHeadersAlignment.Right ? Orientation.Vertical : Orientation.Horizontal);
			UpdateTemplate();
		}
		void InitializeContentPanel() {
			ContentPanelInitializeOperation = null;
			PartPageViewContentPanel.Do(x => x.Initialize(this));
		}
		System.Windows.Threading.DispatcherOperation ContentPanelInitializeOperation;
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			ContentPanelInitializeOperation = Dispatcher.BeginInvoke(new Action(InitializeContentPanel));
		}
		protected override void EnsureItemsPanelCore(Panel itemsPanel) {
			base.EnsureItemsPanelCore(itemsPanel);
			if(itemsPanel is PageViewStackPanel) {
				itemsPanel.SetBinding(PageViewStackPanel.OrientationProperty, new Binding("Orientation") { Source = this });
				itemsPanel.SetBinding(PageViewStackPanel.LayoutTypeProperty, new Binding("PageHeadersLayoutType") { Source = this });
			}
		}
		protected override void ReleaseItemsPanelCore(Panel itemsPanel) {
			if(itemsPanel is PageViewStackPanel) {
				itemsPanel.ClearValue(PageViewStackPanel.OrientationProperty);
			}
			base.ReleaseItemsPanelCore(itemsPanel);
		}
		protected override void ClearTemplateChildren() {
			if(PartViewPresenter != null && !LayoutTreeHelper.IsTemplateChild(PartViewPresenter, this)) {
				PartViewPresenter.Dispose();
				PartViewPresenter = null;
			}
			if(PartPageViewContentPanel != null && !LayoutTreeHelper.IsTemplateChild(PartPageViewContentPanel, this)) {
				PartPageViewContentPanel.UnInitialize();
				PartPageViewContentPanel = null;
			}
			base.ClearTemplateChildren();
		}
		internal NavigationHeaderControl PartNavigationHeader { get; private set; }
		ViewPresenter PartViewPresenter;
		internal PageViewContentPanel PartPageViewContentPanel { get; private set; }
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
			PartViewPresenter = LayoutTreeHelper.GetTemplateChild<ViewPresenter, PageView>(this);
			PartPageViewContentPanel = LayoutTreeHelper.GetTemplateChild<PageViewContentPanel, PageView>(this);
			PartNavigationHeader = GetTemplateChild("PART_NavigationHeader") as NavigationHeaderControl;
		}
		protected override void OnSelectedItemChanged(object oldValue, object newValue) {
			int oldIndex = Items.IndexOf(oldValue);
			AnimationDirection = oldIndex >= SelectedIndex ? SelectorAnimationDirection.Forward : SelectorAnimationDirection.Back;
			base.OnSelectedItemChanged(oldValue, newValue);
			if(newValue != null) {
				IScrollablePanel scrollablePanel = PartItemsPanel as IScrollablePanel;
				var container = ItemContainerGenerator.ContainerFromItem(newValue) as FrameworkElement;
				if(scrollablePanel != null && container != null) scrollablePanel.BringChildIntoView(container);
			}
		}
		protected void OnHeaderChanged(object oldValue, object newValue) {
			this.SetValue(HasHeaderPropertyKey, newValue != null);
		}
		protected override ISelectorItem CreateSelectorItem() {
			return new PageViewItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is PageViewItem;
		}
		protected override void UpdateSelectedContent() {
			if(PartViewPresenter != null) PartViewPresenter.LockContentLoading();
			base.UpdateSelectedContent();
			if(PartViewPresenter != null) PartViewPresenter.UnlockContentLoading();
		}
		protected virtual void OnPageHeadersLayoutTypeChanged(PageHeadersLayoutType oldValue, PageHeadersLayoutType newValue) {
			IsScrollable = PageHeadersLayoutType == WindowsUI.PageHeadersLayoutType.Scroll;
		}
		internal PageViewItem GetContainer(object item) {
			if(item == null || !Items.Contains(item))
				return null;
			return ItemContainerGenerator.ContainerFromItem(item) as PageViewItem;
		}
		internal bool IsIndexInRange(int index) {
			return index >= 0 && index < Items.Count;
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			RaiseItemsChanged(e);
		}
		protected virtual void RaiseItemsChanged(NotifyCollectionChangedEventArgs e) {
			if(ItemsChanged == null)
				return;
			ItemsChanged(this, e);
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			UpdateTemplate();
		}
		void UpdateTemplate() {
			if(TemplateLeft == null || TemplateBottom == null || TemplateRight == null || TemplateTop == null) return;
			ControlTemplate template = null;
			switch(PageHeadersAlignment) {
				case PageHeadersAlignment.Left:
					template = TemplateLeft;
					break;
				case PageHeadersAlignment.Top:
					template = TemplateTop;
					break;
				case PageHeadersAlignment.Right:
					template = TemplateRight;
					break;
				case PageHeadersAlignment.Bottom:
					template = TemplateBottom;
					break;
			}
			if(template != this.Template)
				this.Template = template;
		}
		void UpdateTemplate(PageHeadersAlignment alignment) {
			if(alignment == PageHeadersAlignment) UpdateTemplate();
		}
		protected override void BeforeApplyTemplate() {
			base.BeforeApplyTemplate();
			UpdateTemplate();
		}
#if SILVERLIGHT
		#region ISupportInitialize Members
		void ISupportInitialize.BeginInit() {
		}
		void ISupportInitialize.EndInit() {
			UpdateTemplate();
		}
		#endregion
#endif
		internal event NotifyCollectionChangedEventHandler ItemsChanged;
		[TypeConverter(typeof(StringConverter))]
		public object Header {
			get { return (object)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		public DataTemplateSelector HeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}
		public bool HasHeader {
			get { return (bool)GetValue(HasHeaderProperty); }
		}
		public double AnimationSpeedRatio {
			get { return (double)GetValue(AnimationSpeedRatioProperty); }
			set { SetValue(AnimationSpeedRatioProperty, value); }
		}
		public AnimationType AnimationType {
			get { return (AnimationType)GetValue(AnimationTypeProperty); }
			set { SetValue(AnimationTypeProperty, value); }
		}
		internal SelectorAnimationDirection AnimationDirection {
			get { return (SelectorAnimationDirection)GetValue(AnimationDirectionProperty); }
			private set { this.SetValue(AnimationDirectionPropertyKey, value); }
		}
		public ICommand BackCommand {
			get { return (ICommand)GetValue(BackCommandProperty); }
			set { SetValue(BackCommandProperty, value); }
		}
		public bool ShowBackButton {
			get { return (bool)GetValue(ShowBackButtonProperty); }
			set { SetValue(ShowBackButtonProperty, value); }
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new PageViewAutomationPeer(this);
		}
		public PageHeadersAlignment PageHeadersAlignment {
			get { return (PageHeadersAlignment)GetValue(PageHeadersAlignmentProperty); }
			set { SetValue(PageHeadersAlignmentProperty, value); }
		}
		public ControlTemplate TemplateLeft {
			get { return (ControlTemplate)GetValue(TemplateLeftProperty); }
			set { SetValue(TemplateLeftProperty, value); }
		}
		public ControlTemplate TemplateRight {
			get { return (ControlTemplate)GetValue(TemplateRightProperty); }
			set { SetValue(TemplateRightProperty, value); }
		}
		public ControlTemplate TemplateTop {
			get { return (ControlTemplate)GetValue(TemplateTopProperty); }
			set { SetValue(TemplateTopProperty, value); }
		}
		public ControlTemplate TemplateBottom {
			get { return (ControlTemplate)GetValue(TemplateBottomProperty); }
			set { SetValue(TemplateBottomProperty, value); }
		}
		[Obsolete]
		public ItemsPanelTemplate ItemsPanelHorizontal {
			get { return (ItemsPanelTemplate)GetValue(ItemsPanelHorizontalProperty); }
			set { SetValue(ItemsPanelHorizontalProperty, value); }
		}
		[Obsolete]
		public ItemsPanelTemplate ItemsPanelVertical {
			get { return (ItemsPanelTemplate)GetValue(ItemsPanelVerticalProperty); }
			set { SetValue(ItemsPanelVerticalProperty, value); }
		}
		public PageHeadersLayoutType PageHeadersLayoutType {
			get { return (PageHeadersLayoutType)GetValue(PageHeadersLayoutTypeProperty); }
			set { SetValue(PageHeadersLayoutTypeProperty, value); }
		}
		public bool IsScrollable {
			get { return (bool)GetValue(IsScrollableProperty); }
			private set { SetValue(IsScrollablePropertyKey, value); }
		}
		public ItemCacheMode ItemCacheMode {
			get { return (ItemCacheMode)GetValue(ItemCacheModeProperty); }
			set { SetValue(ItemCacheModeProperty, value); }
		}
		public object BackCommandParameter {
			get { return GetValue(BackCommandParameterProperty); }
			set { SetValue(BackCommandParameterProperty, value); }
		}
	}
}
