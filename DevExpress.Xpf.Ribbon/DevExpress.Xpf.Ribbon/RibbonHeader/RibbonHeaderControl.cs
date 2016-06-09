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

using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using System.Windows.Data;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonHeaderControl : ItemsControl {
		#region static
		public static readonly DependencyProperty ControlBoxWidthProperty;
		public static readonly DependencyProperty OffsetProperty;				
		public static readonly DependencyProperty IsAeroModeProperty;
		static RibbonHeaderControl() {
			ControlBoxWidthProperty = DependencyPropertyManager.Register("ControlBoxWidth", typeof(double), typeof(RibbonHeaderControl), new FrameworkPropertyMetadata(0d));
			OffsetProperty = DependencyPropertyManager.Register("Offset", typeof(double), typeof(RibbonHeaderControl), new FrameworkPropertyMetadata(0d, (d, e) => ((RibbonHeaderControl)d).OnOffsetChanged((double)e.OldValue)));
			IsAeroModeProperty = DependencyPropertyManager.Register("IsAeroMode", typeof(bool), typeof(RibbonHeaderControl), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsAeroModePropertyChanged)));
		}
		protected static void OnIsAeroModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonHeaderControl)d).OnIsAeroModeChanged((bool)e.OldValue);
		}
		#endregion
		#region dep props
		public bool IsAeroMode {
			get { return (bool)GetValue(IsAeroModeProperty); }
			set { SetValue(IsAeroModeProperty, value); }
		}
		public double ControlBoxWidth {
			get { return (double)GetValue(ControlBoxWidthProperty); }
			set { SetValue(ControlBoxWidthProperty, value); }
		}
		public double Offset {
			get { return (double)GetValue(OffsetProperty); }
			set { SetValue(OffsetProperty, value); }
		}
		#endregion
		#region props
		private RibbonControl ribbonCore = null;
		public RibbonControl Ribbon {
			get { return ribbonCore; }
			protected internal set {
				if(ribbonCore == value)
					return;
				RibbonControl oldValue = ribbonCore;
				ribbonCore = value;
				OnRibbonChanged(oldValue);
			}
		}
		protected virtual void OnRibbonChanged(RibbonControl oldValue) {
			ClearValue(IsAeroModeProperty);
			if(Ribbon != null) {
				SetBinding(IsAeroModeProperty, new Binding("IsAeroMode") { Source = Ribbon });
			}
		}
		#endregion
		public RibbonHeaderControl() {
			DefaultStyleKey = typeof(RibbonHeaderControl);
			Loaded += new RoutedEventHandler(OnLoaded);
			Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		public event System.Collections.Specialized.NotifyCollectionChangedEventHandler ItemsChanged;
		RibbonPageCategoryHeaderControl OriginHeaderControl { get; set; }
		protected virtual void OnIsAeroModeChanged(bool oldValue) {
			UpdateOriginHeaderControlAeroMode();
		}
		void UpdateOriginHeaderControlAeroMode() {
			if(OriginHeaderControl != null)
				OriginHeaderControl.IsAeroMode = IsAeroMode;
		}
		protected virtual void OnOffsetChanged(double oldValue) {
			Margin = new Thickness(Offset, 0d, 0d, 0d);
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			UnsubscribeTemplateEvents();
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			if(BlendHelper.IsInBlendTemplateEditor(this))
				InitControlForBlendTemplateEditor();
			UnsubscribeTemplateEvents();
			SubscribeTemplateEvents();			
		}
		protected virtual void InitControlForBlendTemplateEditor() {
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
			if (ItemsChanged != null) ItemsChanged(Items, e);
		}
		protected internal RibbonItemsPanel RibbonItemsPanel { get; private set; }
		internal event ValueChangedEventHandler<RibbonItemsPanel> RibbonItemsPanelChanged;
		protected override void OnItemsPanelChanged(ItemsPanelTemplate oldItemsPanel, ItemsPanelTemplate newItemsPanel) {
			base.OnItemsPanelChanged(oldItemsPanel, newItemsPanel);
			UpdateRibbonItemsPanel();
		}
		private void UpdateRibbonItemsPanel() {
			RibbonItemsPanel oldPanel = RibbonItemsPanel;
			ItemsPresenter presenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
			if (presenter == null) return;
			Dispatcher.BeginInvoke(new System.Action(() => {
				if (System.Windows.Media.VisualTreeHelper.GetChildrenCount(presenter) == 0) return;
				RibbonItemsPanel = System.Windows.Media.VisualTreeHelper.GetChild(presenter, 0) as RibbonItemsPanel;
				if (oldPanel == RibbonItemsPanel) return;
				if (RibbonItemsPanelChanged != null) RibbonItemsPanelChanged(this, new ValueChangedEventArgs<RibbonItemsPanel>(oldPanel, RibbonItemsPanel));
			}));
		}		
		protected internal virtual int GetVisibleCaptionCount() {
			int count = 0;
			for(int i = 0; i < Items.Count; i++) {
				RibbonPageCategoryHeaderControl hdr = GetContainerFromIndex(i);
				if(hdr.Visibility == Visibility.Visible)
					count++;
			}
			return count;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			RibbonPageCategoryHeaderControl control = new RibbonPageCategoryHeaderControl();
			return control;
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return !(item is RibbonPageCategoryBase);
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			RibbonPageCategoryHeaderControl control = element as RibbonPageCategoryHeaderControl;
			var category = (RibbonPageCategoryBase)item;
			if(control != null) {
				if(category is RibbonDefaultPageCategory) {
					control.IsHitTestVisible = false;
					control.Visibility = Visibility.Hidden;
				}
				control.Category = category;
				if(Ribbon != null)
					control.IsInRibbonWindow = Ribbon.IsInRibbonWindow;
			}
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			RibbonPageCategoryHeaderControl control = element as RibbonPageCategoryHeaderControl;			
			if(control != null) {
				control.Category = null;
			}
			base.ClearContainerForItemOverride(element, item);
		}
		public override void OnApplyTemplate() {
			UnsubscribeTemplateEvents();
			base.OnApplyTemplate();
			OriginHeaderControl = GetTemplateChild("PART_OriginPageCategoryHeaderControl") as RibbonPageCategoryHeaderControl;			
			SubscribeTemplateEvents();
			if(BlendHelper.IsInBlendTemplateEditor(this)) InitControlForBlendTemplateEditor();			
			UpdateRibbonItemsPanel();
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			var result = base.ArrangeOverride(arrangeBounds);
			if(RibbonItemsPanel != null) {
				RibbonItemsPanel.InvalidateArrange();
			}
			return result;
		}
		protected virtual void SubscribeTemplateEvents() {
			UpdateOriginHeaderControlAeroMode();
		}
		protected virtual void UnsubscribeTemplateEvents() {
		}
		RibbonPageCategoryHeaderControl GetContainerFromIndex(int index) {
			return ItemContainerGenerator.ContainerFromIndex(index) as RibbonPageCategoryHeaderControl;
		}
		internal void UpdateIsInRibbonWindow(bool IsInRibbonWindow) {
			for(int i = 0; i < Items.Count; i++) {
				RibbonPageCategoryHeaderControl ctrl = GetContainerFromIndex(i);
				if(ctrl != null) {
					ctrl.IsInRibbonWindow = IsInRibbonWindow;
				}
			}
		}
	}
}
