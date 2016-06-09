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
namespace DevExpress.Xpf.Charts.RangeControlClient.Native {
	public class RangeClientItemsManager {
		#region
		class ItemInfo {
			public Type ItemType { get; set; }
			public int ItemsCount { get; set; }
		}
		#endregion
		readonly ObservableCollection<object> items = new ObservableCollection<object>();
		readonly Dictionary<Type, ItemInfo> itemsTypes = new Dictionary<Type, ItemInfo>();
		public ObservableCollection<object> Items { get { return items; } }
		public RangeClientItemsManager(params Type[] itemsTypes) {
			foreach (Type itemType in itemsTypes)
				this.itemsTypes.Add(itemType, new ItemInfo() { ItemType = itemType, ItemsCount = 0 });
		}
		public IEnumerable<T> GetItems<T>() {
			foreach (object item in items)
				if (item is T)
					yield return (T)item;
		}
		public int GetItemsCount<T>() {
			if (itemsTypes.ContainsKey(typeof(T))) {
				return itemsTypes[typeof(T)].ItemsCount;
			}
			return 0;
		}
		public void Add<T>(T item) {
			if (itemsTypes.ContainsKey(typeof(T))) {
				items.Add(item);
				itemsTypes[typeof(T)].ItemsCount += 1;
			}
		}
		public void Clear() {
			items.Clear();
			foreach (ItemInfo info in itemsTypes.Values)
				info.ItemsCount = 0;
		}
	}
}
