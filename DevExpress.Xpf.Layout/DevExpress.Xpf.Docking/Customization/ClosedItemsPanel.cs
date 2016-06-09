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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.Customization;
using DevExpress.Xpf.Docking.Base;
using CoreDock = System.Windows.Controls.Dock;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Docking {
	[DXToolboxBrowsable(false)]
	public class ClosedItemsPanel : BarContainerControl, IDisposable {
		public static readonly DependencyProperty DockProperty;
		static ClosedItemsPanel() {
			var dProp = new DependencyPropertyRegistrator<ClosedItemsPanel>();
			dProp.Register("Dock", ref DockProperty, CoreDock.Top,
				(dObj, ea) => ((ClosedItemsPanel)dObj).OnDockChanged((CoreDock)ea.OldValue, (CoreDock)ea.NewValue));
		}
		public ClosedItemsPanel() {
			Category = new BarManagerCategory() { Name = DockingLocalizer.GetString(DockingStringId.ClosedPanelsCategory) };
			Unloaded += OnUnloaded;
			FrameworkElementHelper.SetAllowDrop(this, false);
		}
		[Obsolete("Use BarManager.Bars property instead.")]
		public new ItemCollection Items { get { return null; } }
		public bool IsDisposing { get; private set; }
		public void Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				Unloaded -= OnUnloaded;
				if(Container != null && !Container.IsDisposing) {
					ClosedItemsBar closedItemsBar = Container.CustomizationController.ClosedItemsBar;
					if(closedItemsBar != null)
						closedItemsBar.UpdatePanel(null);
				}
				BarManagerPropertyHelper.ClearBarManager(this);
			}
			GC.SuppressFinalize(this);
		}
		public BarManagerCategory Category { get; private set; }
		public DockLayoutManager Container { get; private set; }
		public void SetBarManager(BarManager manager) {
			BarManagerPropertyHelper.SetBarManager(this, manager);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Container = DockLayoutManager.FindManager(this);
			ClosedItemsBar closedItemsBar = Container.CustomizationController.ClosedItemsBar;
			if(closedItemsBar != null)
				closedItemsBar.UpdatePanel(this);
		}
		protected override void OnLoaded(object sender, RoutedEventArgs e) {
			base.OnLoaded(sender, e);
			if(Container != null && !Container.IsDisposing) {
				SetBarManager(Container.CustomizationController.BarManager);
				Container.CustomizationController.UpdateClosedItemsBar();
			}
		}
		protected virtual void OnUnloaded(object sender, RoutedEventArgs e) {
			if(Container != null && !Container.IsDisposing) {
				ClosedItemsBar closedItemsBar = Container.CustomizationController.ClosedItemsBar;
				if(closedItemsBar != null) {
					Container.CustomizationController.HideClosedItemsBar();
					closedItemsBar.UpdatePanel(null);
				}
			}
			BarManagerPropertyHelper.ClearBarManager(this);
		}
		public void OnDockChanged(CoreDock oldValue, CoreDock newValue) {
			DevExpress.Xpf.Docking.VisualElements.psvDockPanel.SetDock(this, newValue);
			ContainerType = newValue.ToBarContainerType();
			if(Container != null && Container.CustomizationController.ClosedItemsBar != null) {
				ClosedItemsBar closedItemsBar = Container.CustomizationController.ClosedItemsBar;
				closedItemsBar.DockInfo.ContainerType = newValue.ToBarContainerType();
				closedItemsBar.InvalidateContainerName();
			}
		}
		public CoreDock Dock {
			get { return (CoreDock)GetValue(DockProperty); }
			set { SetValue(DockProperty, value); }
		}
	}
	public class ClosedItemsBar : Bar {
		ClosedItemsPanel panel;
		public ClosedItemsBar(ClosedItemsPanel panel) {
			Container = panel.Container;
			UpdatePanel(panel);
			Caption = Panel.Category.Name;
			UseWholeRow = DevExpress.Utils.DefaultBoolean.True;
			AllowQuickCustomization = DevExpress.Utils.DefaultBoolean.False;
			ShowDragWidget = false;
			Container.LockClosedPanelsVisibility();
			try {
				Visible = false;
			}
			finally {
				Container.UnlockClosedPanelsVisibility();
			}
			CheckCategory();
		}
		protected internal void UpdatePanel(ClosedItemsPanel panel) {
			if(panel == null) {
				DockInfo.Reset();
				ResetItems();
			}
			Panel = panel;
		}
		public DockLayoutManager Container { get; private set; }
		protected void EnsureBarInfo() {
			if(BarInfo == null) {
				BarInfo = CreateBarInfo(Container);
			}
		}
		protected override void OnVisibleChanged(DependencyPropertyChangedEventArgs e) {
			DockInfo.ContainerName = (bool)e.NewValue ? Panel.Name : null;
			base.OnVisibleChanged(e);
			if(!Container.IsClosedPanelsVisibilityLocked) {
				if(Container.ClosedPanelsBarVisibility == ClosedPanelsBarVisibility.Auto) {
					Container.ClosedPanelsBarVisibility = ClosedPanelsBarVisibility.Manual;
				}
			}
		}
		public void UpdateItems(IList items) {
			EnsureBarInfo();
			foreach(BaseLayoutItem item in items) {
				var link = GetLink(item);
				if(link != null) {
					Items.Remove(link);
				}
				BarInfo.CreateBarButtonItem(item);
			}
		}
		public void AddItems(IList items) {
			EnsureBarInfo();
			foreach(BaseLayoutItem item in items) {
				if(GetLink(item) == null)
					BarInfo.CreateBarButtonItem(item);
			}
		}
		public void RemoveItems(IList items) {
			EnsureBarInfo();
			foreach(BaseLayoutItem item in items) {
				var link = GetLink(item);
				if(link != null) {
					Items.Remove(link);
				}
			}
		}
		public void ResetItems() {
			EnsureBarInfo();
			IBarItem[] links = new IBarItem[Items.Count];
			Items.CopyTo(links, 0);
			for(int i = 0; i < links.Length; i++) {
				Items.Remove(links[i]);
			}
		}		
		public ClosedItemsPanel Panel {
			get { return panel; }
			private set {
				if (value == panel) return;
				ClosedItemsPanel oldValue = panel;
				panel = value;
				OnPanelChanged(oldValue);
			}
		}
		protected virtual void OnPanelChanged(ClosedItemsPanel oldValue) {
			DockInfo.Container = Panel;
		}
		public BarButtonItem CreateBarButtonItem(object content) {
			BaseLayoutItem item = content as BaseLayoutItem;
			BarButtonItem barItem = new BarButtonItem()
			{
				CategoryName = Panel.Category.Name,
				Glyph = item.CaptionImage
			};
			BindingHelper.SetBinding(barItem, BarItem.ContentProperty, item, "CustomizationCaption");
			barItem.Tag = item;
			Items.Add(barItem);
			return barItem;
		}
		BarItem GetLink(BaseLayoutItem item) {
			foreach(BarItem link in Items)
				if(link.Tag == item) return link;
			return null;
		}
		protected void CheckCategory() {
			BarManager manager = BarManager.GetBarManager(this);
			if(manager != null && !manager.Categories.Contains(Panel.Category)) {
				manager.Categories.Add(Panel.Category);
			}
		}
		protected ClosedItemsBarInfo BarInfo { get; private set; }
		protected virtual ClosedItemsBarInfo CreateBarInfo(DockLayoutManager container) {
			return new ClosedItemsBarInfo(this, container);
		}
		internal void InvalidateContainerName() {
			DockInfo.ContainerName = null;
			DockInfo.ContainerName = Panel != null && Visible ? Panel.Name : null;
		}
	}
	public class ClosedItemsBarInfo {
		public ClosedItemsBarInfo(ClosedItemsBar bar, DockLayoutManager container) {
			Bar = bar;
			Container = container;
		}
		public ClosedItemsBar Bar { get; private set; }
		public DockLayoutManager Container { get; private set; }
		public void CreateBarButtonItem(BaseLayoutItem item) {
			CreateBarButtonItem(item,
					DockControllerHelper.CreateCommand<RestoreCommand>(Container, item)
				);
		}
		protected virtual BarButtonItem CreateBarButtonItem(object content, ICommand command) {
			BarButtonItem item = Bar.CreateBarButtonItem(content);
			CustomizationControllerHelper.AssignCommand(item, command, Container);
			return item;
		}
	}
	static class BarManagerPropertyHelper {
		public static void ClearBarManager(DependencyObject dObj) {
			dObj.ClearValue(BarManager.BarManagerProperty);
			dObj.ClearValue(BarManager.BarManagerInfoProperty);
		}
		public static void SetBarManager(DependencyObject dObj, BarManager manager) {
			BarManager.SetBarManager(dObj, manager);
		}
		public static BarManager GetBarManager(DependencyObject dObj) {
			return BarManager.GetBarManager(dObj);			
		}
	}
}
