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
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using System.Windows;
using DevExpress.Xpf.Utils.Themes;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Input;
using DevExpress.Utils;
using System.Diagnostics;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars.Customization {
	public class BarItemList : ListBox, ISupportDragDrop {
		DragDropElementHelper dragDropHelper;
		public BarItemList() {
			this.dragDropHelper = new BarDragDropElementHelper(this, false);
		}
		protected DragDropElementHelper DragDropHelper { get { return dragDropHelper; } }
		internal IDragElement DragElement { get; set; }
		bool isMouseLeftButtonDown = false;
		bool IsMouseLeftButtonDown {
			get {
				return Mouse.LeftButton == MouseButtonState.Pressed;
			}
			set { this.isMouseLeftButtonDown = value; }  
		}
		internal virtual void OnMouseLeftButtonDownCore(MouseButtonEventArgs e) {
			OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			IsMouseLeftButtonDown = true;
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			IsMouseLeftButtonDown = false;
		}
		protected override void OnMouseMove(MouseEventArgs e) {			
			if(IsMouseLeftButtonDown) return;
			base.OnMouseMove(e);
		}
		protected override void OnIsMouseCapturedChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new BarItemListItem() { HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch };
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return (item is BarItemListItem);
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			((BarItemListItem)element).BarItemList = this;
		}
		protected ListBoxItem GetHotItem() {
			for(int i = 0; i < Items.Count; i++) {
				ListBoxItem item = (ListBoxItem)ItemContainerGenerator.ContainerFromIndex(i);
				if(item != null) {
					if(((BarItemListItem)item).IsMouseOver) return item;
				}
			}
			return null;
		}
		#region ISupportDragDrop Members
		bool ISupportDragDrop.CanStartDrag(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			DragItem = GetHotItem();
			return DragItem != null;
		}
		protected internal ListBoxItem DragItem { get; set; }
		IDragElement ISupportDragDrop.CreateDragElement(Point offset) {
			if(DragItem == null) return null;
			BarItemInfo info = DragItem.DataContext as BarItemInfo;
			BarDragElementPopup popup = new BarDragElementPopup(new BarItemDragElementContent(info.Item), DragItem, info.Item, ((ISupportDragDrop)this).SourceElement);
			popup.FlowDirection = this.FlowDirection;
			popup.IsOpen = true;
			DragElement = popup;
			return popup;
		}
		IEnumerable<UIElement> ISupportDragDrop.GetTopLevelDropContainers() {
			return BarDragDropElementHelper.GetTopLevelDropContainers(this);
		}
		FrameworkElement ISupportDragDrop.SourceElement {
			get { return this; }
		}
		IDropTarget ISupportDragDrop.CreateEmptyDropTarget() {
			return new BarEmptyDropTarget() { Manager = BarManagerHelper.GetOrFindBarManager(this) };
		}
		bool ISupportDragDrop.IsCompatibleDropTargetFactory(IDropTargetFactory factory, UIElement dropTargetElement) {
			return factory is BarControlDropTargetFactoryExtension || factory is SubMenuBarControlDropTargetFactoryExtension;
		}
		#endregion
	}
	public class BarItemListItemCompability : ListBoxItem {
		public bool IsMouseLeftButtonPressed { get { return Mouse.LeftButton == MouseButtonState.Pressed; } }
	}
	public class BarItemListItem : BarItemListItemCompability {
		protected override void OnMouseEnter(MouseEventArgs e) {
			if(IsMouseLeftButtonPressed) return;
			base.OnMouseEnter(e);
		}
		public BarItemList BarItemList { get; set; }
	}
}
