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
using DevExpress.Xpf.Bars.Native;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
namespace DevExpress.Xpf.Bars {
	public class SimpleBarCollection<TBar> : ObservableCollection<TBar> where TBar : IBar {
		protected override void InsertItem(int index, TBar item) {
			UpdateIndex(index, item, true);
			base.InsertItem(index, item);
		}		
		protected override void SetItem(int index, TBar item) {
			item.Index = index;
			base.SetItem(index, item);
		}
		protected override void RemoveItem(int index) {
			for (int i = index + 1; i < Count; i++) {
				this[i].Index--;
			}
			base.RemoveItem(index);
		}
		void UpdateIndex(int index, TBar item, bool isInsert) {				
			item.Index = index;
			if (!isInsert)
				return;
			for (int i = index + 1; i < Count; i++) {
				this[i].Index++;
			}
		}
	}
	public class BarCollection : SimpleBarCollection<Bar> {
		int updating;
		BarManager manager;
		public BarCollection(BarManager manager) {
			this.manager = manager;
		}
		public void BeginUpdate() { this.updating++; }
		public void EndUpdate() {
			this.updating--;
			if(this.updating < 0)
				this.updating = 0;
			if(!IsUpdating) {
				OnBarsChanged();
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarCollectionIsUpdating")]
#endif
		public bool IsUpdating { get { return this.updating > 0; } }
#if !SL
	[DevExpressXpfCoreLocalizedDescription("BarCollectionManager")]
#endif
		public BarManager Manager { get { return manager; } }
		protected override void InsertItem(int index, Bar bar) {
			if(bar == null || Contains(bar)) return;
			base.InsertItem(index, bar);
			OnInsertItem(bar);
		}
		protected override void RemoveItem(int index) {
			OnRemoveItem(this[index]);
			base.RemoveItem(index);
		}
		protected override void SetItem(int index, Bar item) {
			Bar oldBar = this[index];
			base.SetItem(index, item);
			OnRemoveItem(oldBar);
			OnInsertItem(item);
		}
		protected override void ClearItems() {
			for(int i = 0; i < Count; i++) {
				OnRemoveItem(this[i]);
			}
			base.ClearItems();
		}
		protected virtual void OnRemoveItem(Bar bar) {
			RaiseBarRegistratorChanged(bar);
			if (Manager == null) {
				bar.Manager = null;
				return;
			}
			if (bar.Manager == Manager) {
				bar.Manager = null;
				Manager.RemoveLogicalChild(bar);
			}			
		}
		protected virtual void OnInsertItem(Bar bar) {
			RaiseBarRegistratorChanged(bar);
			if (Manager == null) return;
			if (bar.Parent == null) {
				Manager.AddLogicalChild(bar);
				bar.Manager = Manager;
			}			
		}
		void RaiseBarRegistratorChanged(Bar bar) {
			BarNameScope.GetService<IElementRegistratorService>(bar).Changed(bar, BarRegistratorKeys.BarNameKey);
			BarNameScope.GetService<IElementRegistratorService>(bar).Changed(bar, BarRegistratorKeys.BarTypeKey);
		}
		protected internal virtual void OnBarsChanged() {
			if(IsUpdating)
				return;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		public Bar this[string name] {
			get { return Find((bar) => string.Equals(bar.Name, name)).FirstOrDefault(); }
		}
		public Bar GetBarByCaption(string barCaption) {
			return Find((bar) => string.Equals(bar.Caption, barCaption)).FirstOrDefault();
		}
		IEnumerable<Bar> Find(Func<Bar, bool> predicate) {
			return this.Where(predicate);
		}
	}
}
