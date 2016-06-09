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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class HierarchyCollectionWithStorage<T, TStorageItem, TParent> : HierarchyCollection<T, TParent>
		where TParent : class {
		readonly IList<TStorageItem> storage;
		readonly Func<T, TStorageItem> convertToStorageItem;
		readonly Func<TStorageItem, T> convertFromStorageItem;
		public HierarchyCollectionWithStorage(
			IList<TStorageItem> storage, Func<T, TStorageItem> convertToStorageItem, Func<TStorageItem, T> convertFromStorageItem,
			TParent owner, Action<T, TParent> attachAction, Action<T, TParent> detachAction)
			: base(owner, attachAction, detachAction, storage.Cast<TStorageItem>().Select(convertFromStorageItem)) {
			Guard.ArgumentNotNull(storage, "storage");
			this.storage = storage;
			this.convertToStorageItem = convertToStorageItem;
			this.convertFromStorageItem = convertFromStorageItem;
		}
		protected override void ClearItemsCore() {
			storage.Clear();
			base.ClearItemsCore();
		}
		protected override void InsertItemCore(int index, T item) {
			storage.Insert(index, convertToStorageItem(item));
			base.InsertItemCore(index, item);
		}
		protected override void SetItemCore(int index, T item) {
			storage[index] = convertToStorageItem(item);
			base.SetItemCore(index, item);
		}
		protected override void RemoveItemCore(int index) {
			storage.RemoveAt(index);
			base.RemoveItemCore(index);
		}
		protected override void MoveItem(int oldIndex, int newIndex) {
			var storageItem = storage[oldIndex];
			storage.RemoveAt(oldIndex);
			storage.Insert(newIndex, storageItem);
			base.MoveItem(oldIndex, newIndex);
		}
	}
}
