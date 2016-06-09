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
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.Mvvm.Native {
	public class HierarchyCollection<T, TParent> : ObservableCollection<T>
		where TParent : class {
		readonly TParent owner;
		readonly Action<T, TParent> attachAction;
		readonly Action<T, TParent> detachAction;
		public HierarchyCollection(TParent owner, Action<T, TParent> attachAction, Action<T, TParent> detachAction, IEnumerable<T> collection = null, bool applyAttachActionForOldItems = true)
			: base(collection ?? Enumerable.Empty<T>()) {
			this.owner = owner;
			this.attachAction = attachAction;
			this.detachAction = detachAction;
			if(applyAttachActionForOldItems) {
				foreach(var item in this)
					attachAction(item, owner);
			}
		}
		protected override void ClearItems() {
			foreach(var item in this)
				detachAction(item, owner);
			ClearItemsCore();
		}
		protected virtual void ClearItemsCore() {
			base.ClearItems();
		}
		protected override void RemoveItem(int index) {
			detachAction(this[index], owner);
			RemoveItemCore(index);
		}
		protected virtual void RemoveItemCore(int index) {
			base.RemoveItem(index);
		}
		protected override void InsertItem(int index, T item) {
			InsertItemCore(index, item);
			attachAction(item, owner);
		}
		protected virtual void InsertItemCore(int index, T item) {
			base.InsertItem(index, item);
		}
		protected override void SetItem(int index, T item) {
			detachAction(this[index], owner);
			SetItemCore(index, item);
			attachAction(item, owner);
		}
		protected virtual void SetItemCore(int index, T item) {
			base.SetItem(index, item);
		}
	}
}
