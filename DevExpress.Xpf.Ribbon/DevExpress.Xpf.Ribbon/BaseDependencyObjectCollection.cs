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

using System.Windows;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.Ribbon {
	public class BaseDependencyObjectCollection<T> : ObservableCollection<T> where T : FrameworkContentElement  {
		public BaseDependencyObjectCollection() {
		}
		protected override void InsertItem(int index, T item) {
			if(Contains(item))
				return;
			base.InsertItem(index, item);
			OnInsertItem(index, item);
		}
		protected override void RemoveItem(int index) {
			var item = this[index];
			base.RemoveItem(index);
			OnRemoveItem(index, item);
		}
		protected override void SetItem(int index, T newItem) {
			T oldItem = this[index];
			base.SetItem(index, newItem);
			OnReplaceItem(index, newItem, oldItem);
		}
		protected override void ClearItems() {
			List<T> removedItems = new List<T>();
			foreach(T item in this) {
				removedItems.Add(item);
			}
			base.ClearItems();
			foreach(T item in removedItems) {
				OnRemoveItem(0, item);
			}
		}
		protected virtual void OnReplaceItem(int index, T newItem, T oldItem) {
		}
		protected virtual void OnRemoveItem(int index, T removedItem) {
		}
		protected virtual void OnInsertItem(int index, T insertedItem) {
		}
		public T this[string name] {
			get {
				foreach(T item in this) {
					if(item.Name == name)
						return item;
				}
				return null;
			}
		}
	}
}
