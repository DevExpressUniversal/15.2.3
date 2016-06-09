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
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Native.MouseHandler {
	public class ItemWithUserData<TItem, TUserData> {
		readonly TItem item;
		readonly TUserData userData;
		public ItemWithUserData(TItem item, TUserData userData) {
			this.item = item;
			this.userData=userData;
		}
		public TItem Item { get { return item; } }
		public TUserData UserData { get { return userData; } }
	}
	public class HierarchicalController<TItem, TUserData>
		where TItem : class, ISupportHierarchy<TItem>
		where TUserData : class {
		Stack<ItemWithUserData<TItem, TUserData>> enteredItems;
		public HierarchicalController() {
			this.enteredItems = new Stack<ItemWithUserData<TItem, TUserData>>();
		}
		public virtual TItem CurrentItem { get { return enteredItems.Count > 0 ? enteredItems.Peek().Item : null; } }
		public virtual TUserData CurrentUserData { get { return enteredItems.Count > 0 ? enteredItems.Peek().UserData : null; } }
		#region Events
		#region BeforeChangeCurrentItem
		EventHandler beforeChangeCurrentItem;
		public event EventHandler BeforeChangeCurrentItem { add { beforeChangeCurrentItem += value; } remove { beforeChangeCurrentItem = Delegate.Remove(beforeChangeCurrentItem, value) as EventHandler; } }
		protected virtual void RaiseBeforeChangeCurrentItem() {
			if (beforeChangeCurrentItem != null)
				beforeChangeCurrentItem(this, EventArgs.Empty);
		}
		#endregion
		#region AfterChangeCurrentItem
		EventHandler afterChangeCurrentItem;
		public event EventHandler AfterChangeCurrentItem { add { afterChangeCurrentItem += value; } remove { afterChangeCurrentItem = Delegate.Remove(afterChangeCurrentItem, value) as EventHandler; } }
		protected virtual void RaiseAfterChangeCurrentItem() {
			if (afterChangeCurrentItem != null)
				afterChangeCurrentItem(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		public virtual void ChangeCurrentItem(TItem item, TUserData userData) {
			if (item == CurrentItem)
				return;
			try {
				RaiseBeforeChangeCurrentItem();
				ProcessLeavedItems(item);
				if (item == CurrentItem)
					return;
				ProcessEnteredItems(item, userData);
			}
			finally {
				RaiseAfterChangeCurrentItem();
			}
		}
		protected virtual void ProcessLeavedItems(TItem item) {
			while (ShouldLeaveCurrentItem(item)) {
				enteredItems.Pop();				
			}
		}
		protected virtual void ProcessEnteredItems(TItem item, TUserData userData) {
			if (item != null && item.Parent != CurrentItem)
				ProcessEnteredItems(item.Parent, userData);
			enteredItems.Push(new ItemWithUserData<TItem, TUserData>(item, userData));
		}
		protected virtual bool ShouldLeaveCurrentItem(TItem item) {
			return CurrentItem != null && (item == null || !item.Inside(CurrentItem));
		}
	}
}
