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
using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
namespace DevExpress.Xpf.Docking.VisualElements {
	public class LayoutItemsControl : psvItemsControl {
		#region static
		public static readonly DependencyProperty OrientationProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty LastChildFillProperty;
		static LayoutItemsControl() {
			var dProp = new DependencyPropertyRegistrator<LayoutItemsControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("Orientation", ref OrientationProperty, Orientation.Horizontal);
			dProp.Register("LastChildFill", ref LastChildFillProperty, true);
		}
		#endregion static
		public LayoutItemsControl() {
		}
		protected override void OnDispose() {
			OwnerPresenter = null;
			if(PartGroupPanel != null) {
				PartGroupPanel.Dispose();
				PartGroupPanel = null;
			}
			base.OnDispose();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is Splitter || item is BaseLayoutItem;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			if(PartGroupPanel != null)
				PartGroupPanel.RebuildLayout();
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			if(PartGroupPanel != null)
				PartGroupPanel.RebuildLayout();
		}
		protected override void PrepareContainer(DependencyObject element, object item) {
			MultiTemplateControl multiTemplateControl = element as MultiTemplateControl;
			if(multiTemplateControl != null) {
				multiTemplateControl.BeginPrepareContainer();
				multiTemplateControl.LayoutItem = (BaseLayoutItem)item;
				multiTemplateControl.EndPrepareContainer();
			}
			(element as Splitter).Do(x => x.Activate());
			(element as BaseLayoutItem).Do(x => x.SelectTemplate());
		}
		protected override void ClearContainer(DependencyObject element) {
			MultiTemplateControl multiTemplateControl = element as MultiTemplateControl;
			if(multiTemplateControl != null) {
				multiTemplateControl.LayoutItem = null;
				multiTemplateControl.Dispose();
			}
			(element as Splitter).Do(x => x.Deactivate());
			(element as BaseLayoutItem).Do(x => x.ClearTemplate());
		}
		protected void EnsureVisualTreeAffinity(UIElement element) {
			GroupPanel parentPanel = LayoutItemsHelper.GetVisualParent(element) as GroupPanel;
			if(parentPanel != null) {
				LayoutItemsControl itemsControl = LayoutItemsHelper.GetTemplateParent<LayoutItemsControl>(parentPanel);
				if(itemsControl != null && itemsControl != this) 
					parentPanel.Children.Remove(element);
			}
		}
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		protected internal GroupPaneContentPresenter OwnerPresenter { get; set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			OwnerPresenter = LayoutItemsHelper.GetTemplateParent<GroupPaneContentPresenter>(this);
			if(OwnerPresenter != null)
				psvContentControl.EnsureContentElement<GroupPane>(this, OwnerPresenter);
		}
		protected internal GroupPanel PartGroupPanel { get; private set; }
		protected override void EnsureItemsPanelCore(Panel itemsPanel) {
			base.EnsureItemsPanelCore(itemsPanel);
			PartGroupPanel = itemsPanel as GroupPanel;
			if(PartGroupPanel != null) {
				this.Forward(PartGroupPanel, GroupPanel.OrientationProperty, "Orientation");
				this.Forward(PartGroupPanel, GroupPanel.LastChildFillProperty, "LastChildFill");
				PartGroupPanel.IsLoadedComplete = true;
				PartGroupPanel.RebuildLayout();
			}
		}
	}
	public class FloatingLayoutItemsControl : LayoutItemsControl {
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is FloatingMultiTemplateControl;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new FloatingMultiTemplateControl();
		}
		protected override void EnsureItemsPanelCore(Panel itemsPanel) {
			base.EnsureItemsPanelCore(itemsPanel);
			PartGroupPanel.Do(x => x.CheckChildrenParity = false);
		}
	}
	public class FloatingMultiTemplateControl : MultiTemplateControl {
		#region static
		[DevExpress.Xpf.Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IsActuallyVisibleProperty;
		static FloatingMultiTemplateControl() {
			var dProp = new DependencyPropertyRegistrator<FloatingMultiTemplateControl>();
			dProp.Register("IsActuallyVisible", ref IsActuallyVisibleProperty, true,
				(d, e) => ((FloatingMultiTemplateControl)d).OnIsActuallyVisibleChanged((bool)e.OldValue, (bool)e.NewValue));
		}
		#endregion
		bool IsActuallyVisible {
			get { return (bool)GetValue(IsActuallyVisibleProperty); }
		}
		bool fHidden;
		bool fHideRequested;
		bool fTemplateApplied;
		void UpdateActualVisibility() {
			if(Container == null) return;
			if(!IsActuallyVisible) {
				bool fWin32Compatible = Container.IsTransparencyDisabled;
				bool fDesktop = Container.GetRealFloatingMode() == Core.FloatingMode.Window;
				if(!fWin32Compatible || !fDesktop) return;
				fHideRequested = true;
				if(fTemplateApplied) Dispatcher.BeginInvoke(new Action(ClearTemplate));
				else ClearTemplate();
			}
			else {
				if(fHideRequested) fHideRequested = false;
				if(fHidden) {
					SelectTemplate(LayoutItem);
					Container.InvalidateView(LayoutItem.GetRoot());
				}
			}
		}
		void ClearTemplate() {
			if(!fHideRequested) return;
			fHideRequested = false;
			DependencyObject templateChild = LayoutItemsHelper.GetChild<DependencyObject>(this);
			if(templateChild is IDisposable) ((IDisposable)templateChild).Dispose();
			ClearValue(TemplateProperty);
			if(templateChild != null) DockLayoutManager.Release(templateChild);
			fHidden = true;
		}
		protected virtual void OnIsActuallyVisibleChanged(bool oldValue, bool newValue) {
			UpdateActualVisibility();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateActualVisibility();
			fTemplateApplied = true;
		}
		protected override void OnLayoutItemChanged(BaseLayoutItem item) {
			base.OnLayoutItemChanged(item);
			if(item != null)
				SetBinding(IsActuallyVisibleProperty, new Binding("IsActuallyVisible") { Source = item });
			else
				ClearValue(IsActuallyVisibleProperty);
		}
	}
}
