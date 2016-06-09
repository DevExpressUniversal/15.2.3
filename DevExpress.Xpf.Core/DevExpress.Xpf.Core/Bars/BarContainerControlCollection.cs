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
namespace DevExpress.Xpf.Bars {
	public class BarContainerControlCollection : ObservableCollection<BarContainerControl> {
		int updating;
		BarManager manager;
		public BarContainerControlCollection(BarManager manager) {
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
		public bool IsUpdating { get { return this.updating > 0; } }
		public BarManager Manager { get { return manager; } }
		public BarContainerControl this[string name] {
			get { return Find((container) => string.Equals(container.Name, name)).FirstOrDefault(); }
		}
		public BarContainerControl FindByType(BarContainerType dock) {
			return Find((container) => container.ContainerType == dock).FirstOrDefault();
		}
		IEnumerable<BarContainerControl> Find(Func<BarContainerControl, bool> predicate) {
			return this.Where(predicate);
		}
		protected override void InsertItem(int index, BarContainerControl container) {
			if(!Contains(container))
				base.InsertItem(index, container);
			OnInsertItem(container);
		}
		protected override void RemoveItem(int index) {
			BarContainerControl c = this[index] as BarContainerControl;
			base.RemoveItem(index);
			OnRemoveItem(c);
		}
		protected override void ClearItems() {
			for(int i = 0; i < Count; i++) {
				OnRemoveItem(this[i]);
			}
			base.ClearItems();
		}
		protected override void SetItem(int index, BarContainerControl item) {
			BarContainerControl oldItem = this[index];
			base.SetItem(index, item);
			OnRemoveItem(oldItem);
			OnInsertItem(item);
		}
		protected virtual void OnRemoveItem(BarContainerControl container) { if (Equals(container.Manager, Manager)) container.ClearValue(BarManager.BarManagerProperty); }
		protected virtual void OnInsertItem(BarContainerControl container) { container.SetValue(BarManager.BarManagerProperty, Manager); }
		protected internal virtual void OnBarsChanged() {
			if(IsUpdating)
				return;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}
}
