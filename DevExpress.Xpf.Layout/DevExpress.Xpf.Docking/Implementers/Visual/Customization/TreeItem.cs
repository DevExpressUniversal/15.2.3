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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Docking.Images;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	public enum TreeItemState { 
		Default, Selected, Dragged, Hovered 
	};
	public class TreeItem : Control, IUIElement, IDockLayoutManagerListener {
		#region static
		public static readonly DependencyProperty TreeItemStateProperty;
		static TreeItem() {
			var dProp = new DependencyPropertyRegistrator<TreeItem>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("TreeItemState", ref TreeItemStateProperty, TreeItemState.Default);
		}
		#endregion static
		public TreeItem() {
			Focusable = false;
		}
		public TreeItemState TreeItemState {
			get { return (TreeItemState)GetValue(TreeItemStateProperty); }
			set { SetValue(TreeItemStateProperty, value); }
		}
		public Border PartBorder { get; private set; }
		public Image PartImage { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Item = DataContext as BaseLayoutItem;
			PartBorder = GetTemplateChild("PART_Border") as Border;
			PartImage = GetTemplateChild("PART_Image") as Image;
			if(PartImage != null)
				PartImage.Source = ImageHelper.GetImageForItem(Item);
		}
		void OnDragInfoChanged(object sender, Customization.DragInfoChangedEventArgs e) {
			if(e.Info != null) {
				if(Item == e.Info.Item) {
					TreeItemState = VisualElements.TreeItemState.Dragged;
					return;
				}
			}
			ClearValue(TreeItemStateProperty);
		}
		void OnLayoutItemSelectionChanged(object sender, Base.LayoutItemSelectionChangedEventArgs e) {
			if(e.Selected && e.Item == Item)
				BringIntoView();
			if(Item.IsSelected)
				TreeItemState = VisualElements.TreeItemState.Selected;
			else
				ClearValue(TreeItemStateProperty);
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return DockLayoutManager.GetUIScope(this); } }
		UIChildren uiChildren = new UIChildren();
		protected internal BaseLayoutItem Item { get; set; }
		UIChildren IUIElement.Children {
			get {
				if(uiChildren == null) uiChildren = new UIChildren();
				return uiChildren;
			}
		}
		#endregion IUIElement
		#region IDockLayoutManagerListener
		void IDockLayoutManagerListener.Subscribe(DockLayoutManager manager) {
			SubscribeEvents(manager);
		}
		void IDockLayoutManagerListener.Unsubscribe(DockLayoutManager manager) {
			UnsubscribeEvents(manager);
		}
		protected virtual void SubscribeEvents(DockLayoutManager manager) {
			manager.CustomizationController.DragInfoChanged += OnDragInfoChanged;
			manager.LayoutItemSelectionChanged += OnLayoutItemSelectionChanged;
		}
		protected virtual void UnsubscribeEvents(DockLayoutManager manager) {
			if(manager.IsDisposing) return;
			manager.CustomizationController.DragInfoChanged -= OnDragInfoChanged;
			manager.LayoutItemSelectionChanged -= OnLayoutItemSelectionChanged;
		}
		#endregion
	}
}
