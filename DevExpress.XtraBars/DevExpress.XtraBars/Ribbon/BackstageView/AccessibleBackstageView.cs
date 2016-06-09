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
using DevExpress.Accessibility;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon.ViewInfo;
namespace DevExpress.XtraBars.Ribbon.BackstageView.Accessible {
	public class BaseBackstageViewAccessible : BaseAccessible {
		AccessibleBackstageView backstageView;
		public BaseBackstageViewAccessible(AccessibleBackstageView backstageView)
			: base() {
			this.backstageView = backstageView;
		}
		public AccessibleBackstageView BackstageView { get { return backstageView; } set { backstageView = value; } }
		public override AccessibleObject Parent { get { return BackstageView.Accessible; } }
		public BackstageViewInfo BackstageViewInfo { get { return BackstageView.BackstageView.ViewInfo; } }
		public override Control GetOwnerControl() { return BackstageView.BackstageView; }
	}
	public class AccessibleBackstageViewButtonItem : BaseBackstageViewAccessible {
		BackstageViewButtonItem backstageViewButtonItem;
		BackstageViewButtonItemViewInfo backstageViewButtonItemInfo;
		public AccessibleBackstageViewButtonItem(AccessibleBackstageView backstageView, BackstageViewButtonItem backstageViewButtonItem)
			: base(backstageView) {
			this.backstageViewButtonItem = backstageViewButtonItem;
		}
		protected override string GetName() { return backstageViewButtonItem.Caption; }
		protected override string GetDefaultAction() { return "Press"; }
		protected override void DoDefaultAction() {
			backstageViewButtonItem.RaiseItemClick();
		}
		public override Rectangle ClientBounds { get { return BackstageViewButtonItemInfo.Bounds; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.MenuItem; }
		protected override AccessibleStates GetState() {
			AccessibleStates states = AccessibleStates.Focusable;
			if(BackstageViewInfo == null || !(BackstageViewInfo.PressedItem is BackstageViewButtonItemViewInfo))
				return states;
			if((BackstageViewInfo.PressedItem.Item as BackstageViewButtonItem) == backstageViewButtonItem)
				states |= AccessibleStates.Focused;
			return states;
		}
		public BackstageViewButtonItem BackstageViewButtonItem { get { return backstageViewButtonItem; } }
		public BackstageViewButtonItemViewInfo BackstageViewButtonItemInfo {
			get {
				if(backstageViewButtonItemInfo != null) return backstageViewButtonItemInfo;
				backstageViewButtonItemInfo = GetBackstageViewButtonItemInfo();
				return backstageViewButtonItemInfo;
			}
		}
		public BackstageViewButtonItemViewInfo GetBackstageViewButtonItemInfo() {
			if(BackstageViewButtonItem == null || BackstageViewInfo == null) return null;
			foreach(var item in BackstageViewInfo.Items) {
				var itemInfo = item as BackstageViewButtonItemViewInfo;
				if(itemInfo == null) continue;
				if(itemInfo.Item == BackstageViewButtonItem)
					return itemInfo;
			}
			return null;
		}
	}
	public class AccessibleBackstageViewTabItem : BaseBackstageViewAccessible {
		BackstageViewTabItem backstageViewTabItem;
		BackstageViewTabItemViewInfo backstageViewTabItemInfo;
		public AccessibleBackstageViewTabItem(AccessibleBackstageView backstageView, BackstageViewTabItem backstageViewTabItem)
			: base(backstageView) {
			this.backstageViewTabItem = backstageViewTabItem;
		}
		protected override string GetName() { return backstageViewTabItem.Caption; }
		protected override string GetDefaultAction() { return "Switch"; }
		protected override void Select(AccessibleSelection flags) {
			BackstageView.BackstageView.SelectedTab = backstageViewTabItem;
		}
		public override Rectangle ClientBounds { get { return BackstageViewTabItemInfo.Bounds; } }
		protected override AccessibleRole GetRole() { return AccessibleRole.PageTab; }
		protected override AccessibleStates GetState() {
			if(!backstageViewTabItem.IsVisible) return AccessibleStates.Invisible;
			AccessibleStates states = AccessibleStates.Selectable | AccessibleStates.Focusable;
			if(BackstageView.BackstageView.SelectedTab == backstageViewTabItem) states |= AccessibleStates.Selected | AccessibleStates.Focused;
			return states;
		}
		public BackstageViewTabItem BackstageViewTabItem { get { return backstageViewTabItem; } }
		public BackstageViewTabItemViewInfo BackstageViewTabItemInfo {
			get {
				if(backstageViewTabItemInfo != null) return backstageViewTabItemInfo;
				backstageViewTabItemInfo = GetBackstageViewTabItemInfo();
				return backstageViewTabItemInfo;
			}
		}
		public BackstageViewTabItemViewInfo GetBackstageViewTabItemInfo() {
			if(BackstageViewTabItem == null || BackstageViewInfo == null) return null;
			foreach(var item in BackstageViewInfo.Items) {
				var itemInfo = item as BackstageViewTabItemViewInfo;
				if(itemInfo == null) continue;
				if(itemInfo.Item == BackstageViewTabItem)
					return itemInfo;
			}
			return null;
		}
	}
	public class AccessibleBackstageViewClientControl : BaseControlAccessible {
		public AccessibleBackstageViewClientControl(BackstageViewClientControl backstageViewClientControl) : base(backstageViewClientControl) { }
		protected override string GetName() { return "The BackstageViewClientControl"; }
		public override string Value { get { return GetName(); } }
		public override Rectangle ClientBounds { get { return BackstageViewClientControl.Bounds; } }
		public override AccessibleObject Parent {
			get {
				if(BackstageViewClientControl.Parent == null) return null;
				return BackstageViewClientControl.Parent.AccessibilityObject;
			}
		}
		protected override AccessibleRole GetRole() { return AccessibleRole.Pane; }
		protected override AccessibleStates GetState() { return AccessibleStates.None; }
		public BackstageViewClientControl BackstageViewClientControl { get { return Control as BackstageViewClientControl; } }
		public override Control GetOwnerControl() { return BackstageViewClientControl; }
	}
	public class AccessibleBackstageView : BaseControlAccessible {
		public AccessibleBackstageView(BackstageViewControl backstageView)
			: base(backstageView) {
			CreateCollection();
		}
		public BackstageViewControl BackstageView { get { return Control as BackstageViewControl; } }
		public override Rectangle ClientBounds { get { return BackstageView.Bounds; } }
		protected override string GetName() { return "The BackstageViewControl"; }
		protected override AccessibleRole GetRole() { return AccessibleRole.PageTabList; }
		protected override AccessibleStates GetState() { return AccessibleStates.None; }
		public override string Value { get { return GetName(); } }
		protected internal int GetButtonItemId(BackstageViewButtonItem buttonItem) {
			if(buttonItem == null || Children.Count == 0)
				return -1;
			for(int i = 0; i < Children.Count; i++) {
				var accessibleBackstageViewButtonItem = Children[i] as AccessibleBackstageViewButtonItem;
				if(accessibleBackstageViewButtonItem == null)
					continue;
				if(accessibleBackstageViewButtonItem.BackstageViewButtonItem == buttonItem)
					return i;
			}
			return -1;
		}
		protected internal int GetTabItemId(BackstageViewTabItem tabItem) {
			if(tabItem == null || Children.Count == 0)
				return -1;
			for(int i = 0; i < Children.Count; i++) {
				var accessibleBackstageViewTabItem = Children[i] as AccessibleBackstageViewTabItem;
				if(accessibleBackstageViewTabItem == null)
					continue;
				if(accessibleBackstageViewTabItem.BackstageViewTabItem == tabItem)
					return i;
			}
			return -1;
		}
		public override AccessibleObject Parent {
			get {
				if(BackstageView.Parent == null) return null;
				return BackstageView.Parent.AccessibilityObject;
			}
		}
		protected override ChildrenInfo GetChildrenInfo() {
			return new ChildrenInfo(ChildType.Item, BackstageView.Items.Count);
		}
		public override Control GetOwnerControl() { return BackstageView; }
		protected override void OnChildrenCountChanged() {
			base.OnChildrenCountChanged();
			Children.Clear();
			foreach(var item in BackstageView.Items) {
				var tabItem = item as BackstageViewTabItem;
				if(tabItem != null) {
					Children.Add(new AccessibleBackstageViewTabItem(this, tabItem));
					continue;
				}
				var buttonItem = item as BackstageViewButtonItem;
				if(buttonItem != null) {
					Children.Add(new AccessibleBackstageViewButtonItem(this, buttonItem));
					continue;
				}
			}
		}
	}
}
