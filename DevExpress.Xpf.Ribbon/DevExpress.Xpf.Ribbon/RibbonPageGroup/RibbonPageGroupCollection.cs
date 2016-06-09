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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPageGroupCollection : ObservableCollection<RibbonPageGroup> {
		int updating;
		public RibbonPageGroupCollection(RibbonPage page) {
			Page = page;
		}
		public RibbonControl Ribbon { get { return PageCategory == null ? null : PageCategory.Ribbon; } }
		public RibbonPageCategoryBase PageCategory { get { return Page == null ? null : Page.PageCategory; } }
		RibbonPage page;
		public RibbonPage Page {
			get {return page;}
			private set {
				RibbonPage oldValue = page;
				page = value;
				if(value != oldValue)
					OnRibbonPageChanged(oldValue, value);
			}
		}
		void OnRibbonPageChanged(RibbonPage oldValue, RibbonPage newValue) {			
		}
		public void BeginUpdate() { this.updating++; }
		public void EndUpdate() {
			this.updating--;
			if(this.updating < 0)
				this.updating = 0;
			if(!IsUpdating) {
				OnCategoryChanged();
			}
		}
		public bool IsUpdating { get { return this.updating > 0; } }
		protected override void InsertItem(int index, RibbonPageGroup group) {
			if(Contains(group)) return;
			base.InsertItem(index, group);
			OnInsertItem(group, index);
			group.Index = index;
		}
		protected override void RemoveItem(int index) {			
			var item = this[index];
			base.RemoveItem(index);
			OnRemoveItem(item, index);
		}
		protected override void ClearItems() {
			List<RibbonPageGroup> groupsForRemove = new List<RibbonPageGroup>();
			foreach(RibbonPageGroup group in this) {
				groupsForRemove.Add(group);
			}
			base.ClearItems();
			for(int i = 0; i < groupsForRemove.Count; i++) {
				OnRemoveItem(groupsForRemove[i], i);
			}
		}
		protected override void SetItem(int index, RibbonPageGroup item) {
			RibbonPageGroup group = this[index];
			base.SetItem(index, item);
			OnRemoveItem(group, index);
			OnInsertItem(item, index);
		}
		protected virtual void OnRemoveItem(RibbonPageGroup group, int index) {
			if(group.Page != null) {
				group.Page.OnPageGroupRemoved(group, index);
			}
			group.Page = null;
		}
		protected virtual void OnInsertItem(RibbonPageGroup group, int index) {
			if (group.Page == null)
				group.Page = Page;
			if(group.Page != null) {
				group.Page.OnPageGroupInserted(group, index);
			}
		}				
		protected internal virtual void OnCategoryChanged() {
			if(IsUpdating) return;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		public RibbonPageGroup this[string name] {
			get {
				foreach(RibbonPageGroup group in this) {
					if(group.Name == name) return group;
				}
				return null;
			}
		}
	}
}
