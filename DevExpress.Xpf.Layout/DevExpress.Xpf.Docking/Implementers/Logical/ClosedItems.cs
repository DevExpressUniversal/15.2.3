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
using System.Collections.ObjectModel;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Docking.Base;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking {
	public class ClosedPanelCollection : ObservableCollection<LayoutPanel>, IDisposable {
		DockLayoutManager Owner;
		public ClosedPanelCollection(DockLayoutManager owner) {
			Owner = owner;
		}
		bool isDisposing;
		public void Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDisposing();
			}
			GC.SuppressFinalize(this);
		}
		protected virtual void OnDisposing() {
			ClearItems();
		}
		protected override void SetItem(int index, LayoutPanel item) {
			item.SetClosed(true);
			base.SetItem(index, item);
		}
		protected override void InsertItem(int index, LayoutPanel item) {
			if(Owner != null) {
				if(item.Manager == null)
					item.Manager = Owner;
				DockLayoutManager.AddLogicalChild(Owner, item);
			}
			item.SetClosed(true);
			base.InsertItem(index, item);
		}
		protected override void RemoveItem(int index) {
			LayoutPanel item = ((index < 0) || (index >= Count)) ? null : this[index];
			base.RemoveItem(index);
			if(item != null) item.SetClosed(false);
			item.Do((x) => x.BeginLayoutChange());
			DockLayoutManager.RemoveLogicalChild(Owner, item);
			item.Do((x) => x.EndLayoutChange());
		}
		protected override void ClearItems() {
			LayoutPanel[] panels = new LayoutPanel[Items.Count];
			Items.CopyTo(panels, 0);
			base.ClearItems();
			foreach(LayoutPanel item in panels) {
				item.SetClosed(false);
				item.Do((x) => x.BeginLayoutChange());
				DockLayoutManager.RemoveLogicalChild(Owner, item);
				item.Do((x) => x.EndLayoutChange());
			}
		}
		public LayoutPanel this[string name] {
			get {
				foreach(LayoutPanel item in Items) {
					if(item.Name == name) return item;
				}
				return null;
			}
		}
		public void AddRange(LayoutPanel[] panels) {
			Array.ForEach(panels, Add);
		}
		public LayoutPanel[] ToArray() {
			LayoutPanel[] groups = new LayoutPanel[Count];
			Items.CopyTo(groups, 0);
			return groups;
		}
	}
	public class HiddenItemsCollection : BaseLayoutItemCollection {
		DockLayoutManager Manager;
		public HiddenItemsCollection(DockLayoutManager owner, LayoutGroup customizationRoot)
			: base(customizationRoot) {
				Manager = owner;
		}
		LayoutGroup currentParent;
		protected override void OnItemAdded(BaseLayoutItem item) {
			if(Owner != null)
				item.Manager = Manager;
			item.SetHidden(true, currentParent);
		}
		protected override void OnItemRemoved(BaseLayoutItem item) {
			item.SetHidden(false, null);
		}
		protected override void CheckItemRules(BaseLayoutItem item) {
			if(item.ItemType != LayoutItemType.ControlItem && item.ItemType != LayoutItemType.Group && !(item is FixedItem))
				throw new NotSupportedException(DockLayoutManagerHelper.GetRule(DockLayoutManagerRule.ItemCanNotBeHidden));
		}
		public void Add(BaseLayoutItem item, LayoutGroup currentParent) {
			if(item is FixedItem) return;
			this.currentParent = currentParent;
			base.Add(item);
		}
		public new void Add(BaseLayoutItem item) {
			Add(item, null);
		}
		protected override void BeforeItemAdded(BaseLayoutItem item) {
			CheckItemRules(item);
		}
	}
}
