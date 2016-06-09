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
using System.Windows;
using DevExpress.Xpf.Core;
using System.Collections;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors {
	public class RecycledCollection<TItem> : IEnumerable<TItem> where TItem : FrameworkElement {
		readonly Dictionary<int, TItem> manipulationItems;
		readonly Dictionary<int, TItem> items;
		public bool IsManipulating { get; set; }
		public int Count { get { return items.Count; } }
		public RecycledCollection() {
			items = new Dictionary<int, TItem>();
			manipulationItems = new Dictionary<int, TItem>();
		}
		public bool Contains(int index) {
			return !IsManipulating || items.ContainsKey(index);
		}
		public TItem Pop(int index) {
			var element = GetItem(index);
			if (element != null) {
				var frameworkElement = element as FrameworkElement;
				if (frameworkElement != null)
					frameworkElement.Visibility = Visibility.Visible;
			}
			return element;
		}
		TItem GetItem(int index) {
			if (!items.Any())
				return default(TItem);
			TItem element;
			int key = index;
			if (!items.TryGetValue(index, out element)) {
				var keyValue = items.First();
				element = keyValue.Value;
				key = keyValue.Key;
			}
			items.Remove(key);
			return element;
		}
		public void AddManipulationItems(IDictionary<int, TItem> mItems) {
			ResetManipulationItems();
			manipulationItems.AddRange(mItems);
		}
		public void ResetManipulationItems() {
			manipulationItems.Clear();
		}
		public bool IsManipulationItem(int index) {
			return manipulationItems.ContainsKey(index);
		}
		public void Push(int index, TItem item) {
			item.Visibility = Visibility.Collapsed;
			items.Add(index, item);
		}
		public void Reset() {
			items.Clear();
		}
		public IEnumerator<TItem> GetEnumerator() {
			return items.Values.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return items.Values.GetEnumerator();
		}
	}
}
