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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;
using DevExpress.Xpf.WindowsUI.Base;
using DevExpress.Xpf.WindowsUI.Internal;
using DevExpress.Xpf.WindowsUI.UIAutomation;
using DevExpress.Xpf.Core;
#if SILVERLIGHT
using DevExpress.Xpf.Core.Native;
using ManipulationInertiaStartingEventArgs = DevExpress.Xpf.Core.Native.SLManipulationInertiaStartingEventArgs;
using ManipulationDeltaEventArgs = DevExpress.Xpf.Core.Native.SLManipulationDeltaEventArgs;
using ManipulationCompletedEventArgs = DevExpress.Xpf.Core.Native.SLManipulationCompletedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.Xpf.WindowsUI {
	public class SlideViewItemClickEventArgs : EventArgs {
		public SlideViewItemClickEventArgs(object item) {
			Item = item;
		}
		public object Item { get; private set; }
	}
#if !SILVERLIGHT
#endif
	[DevExpress.Xpf.Core.DXToolboxBrowsable]
	[TemplatePart(Name = "PART_NavigationHeader", Type = typeof(NavigationHeaderControl))]
	[TemplatePart(Name = "PART_ScrollPanel", Type = typeof(SlideViewScrollPanel))]
	[TemplatePart(Name = "PART_HeaderHost", Type = typeof(FrameworkElement))]
	public class SlideView : veItemsControl {
		#region static
		public static readonly DependencyProperty HeaderProperty;
		public static readonly DependencyProperty HeaderTemplateProperty;
		public static readonly DependencyProperty ItemHeaderTemplateProperty;
		public static readonly DependencyProperty ItemHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty BackCommandProperty;
		public static readonly DependencyProperty BackCommandParameterProperty;
		public static readonly DependencyProperty ShowBackButtonProperty;
		public static readonly DependencyProperty ItemSpacingProperty;
		public static readonly DependencyProperty OrientationProperty;
		public static readonly DependencyProperty StickyHeadersProperty;
#if SILVERLIGHT
		static readonly DependencyProperty ScrollViewerHorizontalOffsetProperty;
		static readonly DependencyProperty HeaderHeightListenerProperty;
#else
		static readonly Point zero = new Point();
#endif
		static SlideView() {
			var dProp = new DependencyPropertyRegistrator<SlideView>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Header", ref HeaderProperty, (object)null);
			dProp.Register("HeaderTemplate", ref HeaderTemplateProperty, (DataTemplate)null);
			dProp.Register("ItemHeaderTemplate", ref ItemHeaderTemplateProperty, (DataTemplate)null);
			dProp.Register("ItemHeaderTemplateSelector", ref ItemHeaderTemplateSelectorProperty, (DataTemplateSelector)null);
			dProp.Register("BackCommand", ref BackCommandProperty, (ICommand)null);
			dProp.Register("BackCommandParameter", ref BackCommandParameterProperty, (object)null);
			dProp.Register("ShowBackButton", ref ShowBackButtonProperty, true);
			dProp.Register("ItemSpacing", ref ItemSpacingProperty, 0d);
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal, (o, args) => {
				((SlideView)o).OnOrientationChanged((Orientation)args.OldValue, (Orientation)args.NewValue);
			});
			dProp.Register("StickyHeaders", ref StickyHeadersProperty, true);
#if SILVERLIGHT
			dProp.Register("ScrollViewerHorizontalOffset", ref ScrollViewerHorizontalOffsetProperty, 0d, 
				(d,e)=>((SlideView)d).OnHorizontalOffsetChanged());
			dProp.Register("HeaderHeightListener", ref HeaderHeightListenerProperty, 0d);
#endif
		}
		#endregion
		public SlideView() {
#if SILVERLIGHT
			DefaultStyleKey = typeof(SlideView);
#endif
		}
#if SILVERLIGHT
		void SlideView_LayoutUpdated(object sender, System.EventArgs e) {
			LayoutUpdated -= SlideView_LayoutUpdated;
			if(PartNavigationHeaderControl == null) return;
			OnHorizontalOffsetChanged();
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			LayoutUpdated += SlideView_LayoutUpdated;
		}
#endif
		SlideViewScrollPanel PartScrollPanel;
		internal NavigationHeaderControl PartNavigationHeaderControl { get; private set; }
		FrameworkElement PartHeaderHost;
		internal double ViewportWidth {
			get { return PartScrollPanel != null ? PartScrollPanel.ViewportBounds.Width : double.NaN; }
		}
		internal double TotalWidth {
			get { return PartScrollPanel != null ? PartScrollPanel.ScrollAreaSize.Width : double.NaN; }
		}
		internal double HorizontalOffset {
			get { return PartScrollPanel != null ? PartScrollPanel.HorizontalOffset : double.NaN; }
		}
		internal void SetScrollOffset(double offset) {
			if(PartScrollPanel != null)
				PartScrollPanel.SetOffset(new Point(offset, 0));
		}
		protected override void ClearTemplateChildren() {
			PartNavigationHeaderControl = null;
			if(PartScrollPanel != null) PartScrollPanel.ScrollChanged -= Panel_ScrollChanged;
#if SILVERLIGHT
			if(PartHeaderHost != null) PartHeaderHost.SizeChanged -= PartHeaderHost_SizeChanged;
#endif
			base.ClearTemplateChildren();
		}
		protected override void OnApplyTemplateComplete() {
			base.OnApplyTemplateComplete();
			if(PartScrollPanel != null) {
				PartScrollPanel.ScrollChanged += Panel_ScrollChanged;
#if !SILVERLIGHT
				Dispatcher.BeginInvoke(new Action(() =>
				{
					OnHorizontalOffsetChanged();
				}));
#endif
			}
		}
		void Panel_ScrollChanged(object sender, EventArgs e) {
			OnHorizontalOffsetChanged();
		}
		protected virtual void OnOrientationChanged(Orientation oldValue, Orientation newValue) {
			if(PartScrollPanel == null) return;
			if(Orientation == Orientation.Vertical) {
				System.Windows.Controls.Grid.SetRow(PartScrollPanel, 1);
				System.Windows.Controls.Grid.SetRowSpan(PartScrollPanel, 1);
			} else {
				System.Windows.Controls.Grid.SetRow(PartScrollPanel, 0);
				System.Windows.Controls.Grid.SetRowSpan(PartScrollPanel, 2);
			}
			InvalidateMeasure();
		}
		protected override void GetTemplateChildren() {
			base.GetTemplateChildren();
			PartNavigationHeaderControl = GetTemplateChild("PART_NavigationHeader") as NavigationHeaderControl;
			PartScrollPanel = GetTemplateChild("PART_ScrollPanel") as SlideViewScrollPanel;
			PartHeaderHost = GetTemplateChild("PART_HeaderHost") as FrameworkElement;
#if SILVERLIGHT
			if(PartHeaderHost != null) PartHeaderHost.SizeChanged += PartHeaderHost_SizeChanged;
#endif
		}
#if SILVERLIGHT
		void PartHeaderHost_SizeChanged(object sender, SizeChangedEventArgs e) {
			SetValue(HeaderHeightListenerProperty, e.NewSize.Height);
		}
#endif
		protected override DependencyObject GetContainerForItemOverride() {
			return new SlideViewItem();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is SlideViewItem;
		}
		void OnHorizontalOffsetChanged() {
			if(PartScrollPanel == null) return;
			foreach(object item in Items) {
				SlideViewItem container = ItemContainerGenerator.ContainerFromItem(item) as SlideViewItem;
				if(container != null && PartNavigationHeaderControl != null) {
					Point p = PartNavigationHeaderControl.PartContent != null ? PartNavigationHeaderControl.PartContent.TranslatePoint(new Point(), this) : PartNavigationHeaderControl.TranslatePoint(new Point(), this);
#if !SILVERLIGHT
					double offset = container.TranslatePoint(zero, this).X;
#else
					double offset = PartScrollPanel.HorizontalOffset;
#endif
					container.OnScrollChanged(offset, Header == null && HeaderTemplate == null ? 0 : p.X);
				}
			}
		}
		protected override void PrepareContainer(DependencyObject element, object item) {
			if(element is IItemContainer) ((IItemContainer)element).PrepareContainer(item, ItemHeaderTemplate, ItemHeaderTemplateSelector);
			SlideViewItem container = element as SlideViewItem;
			if(container!=null) {
				container.Owner = this;
#if SILVERLIGHT
				container.SetBinding(SlideViewItem.HeaderPlaceholderHeightProperty, new Binding("HeaderHeightListener") { Source = this});
#else
				container.SetBinding(SlideViewItem.HeaderPlaceholderHeightProperty, new Binding("ActualHeight") { Source = PartHeaderHost });
#endif
				container.SetBinding(SlideViewItem.IsHeaderStickyProperty, new Binding("StickyHeaders") { Source = this });
			}
		}
		protected override void ClearContainer(DependencyObject element, object item) {
			SlideViewItem container = element as SlideViewItem;
			if(container != null) {
				container.Owner = null;
				container.ClearValue(SlideViewItem.HeaderPlaceholderHeightProperty);
			}
			base.ClearContainer(element, item);
		}
		protected override void EnsureItemsPanelCore(Panel itemsPanel) {
			if(itemsPanel is SlideViewItemsPanel) {
				itemsPanel.SetBinding(SlideViewItemsPanel.ItemSpacingProperty, new Binding("ItemSpacing") { Source = this });
			}
		}
		public event EventHandler<SlideViewItemClickEventArgs> ItemClick;
		internal void OnItemClick(SlideViewItem container) {
			if(container == null) return;
			var handler = ItemClick;
			if(handler != null) {
				object item = ItemContainerGenerator.ItemFromContainer(container);
				handler(this, new SlideViewItemClickEventArgs(item));
			}
		}
		public void ScrollIntoView(object item) {
			var slideViewItem = ItemContainerGenerator.ContainerFromItem(item) as SlideViewItem;
			if (slideViewItem == null) return;
			PartScrollPanel.BringChildIntoView(slideViewItem, true);
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new SlideViewAutomationPeer(this);
		}
		internal double HeaderOffset {
			get { return PartNavigationHeaderControl == null ? 0 : PartNavigationHeaderControl.HeaderOffset; }
		}
		[TypeConverter(typeof(StringConverter))]
		public object Header {
			get { return (object)GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		public DataTemplate ItemHeaderTemplate {
			get { return (DataTemplate)GetValue(ItemHeaderTemplateProperty); }
			set { SetValue(ItemHeaderTemplateProperty, value); }
		}
		public DataTemplateSelector ItemHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemHeaderTemplateSelectorProperty); }
			set { SetValue(ItemHeaderTemplateSelectorProperty, value); }
		}
		public ICommand BackCommand {
			get { return (ICommand)GetValue(BackCommandProperty); }
			set { SetValue(BackCommandProperty, value); }
		}
		public bool ShowBackButton {
			get { return (bool)GetValue(ShowBackButtonProperty); }
			set { SetValue(ShowBackButtonProperty, value); }
		}
		public double ItemSpacing {
			get { return (double)GetValue(ItemSpacingProperty); }
			set { SetValue(ItemSpacingProperty, value); }
		}
		public Orientation Orientation {
			get { return (Orientation) GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public bool StickyHeaders {
			get { return (bool)GetValue(StickyHeadersProperty); }
			set { SetValue(StickyHeadersProperty, value); }
		}
		public object BackCommandParameter {
			get { return GetValue(BackCommandParameterProperty); }
			set { SetValue(BackCommandParameterProperty, value); }
		}
	}
}
