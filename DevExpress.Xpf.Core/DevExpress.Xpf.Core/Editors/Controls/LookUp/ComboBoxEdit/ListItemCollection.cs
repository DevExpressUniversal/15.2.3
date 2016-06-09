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

#region usings
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
#if !SL
using System.Linq;
using DevExpress.Xpf.Editors.Helpers;
using System.Windows.Controls;
#else
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using SelectionChangedEventHandler = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventHandler;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using System.Windows.Data;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
#endif
#endregion
namespace DevExpress.Xpf.Editors {
	public class ListItemCollection : CollectionBase, INotifyCollectionChanged {
		IListNotificationOwner Owner { get; set; }
		internal ListItemCollection(IListNotificationOwner listNotification) {
			Owner = listNotification;
		}
		public int Add(object item) { return List.Add(item); }
		public void Insert(int index, object item) { List.Insert(index, item); }
		public void Remove(object item) { List.Remove(item); }
		public bool Contains(object item) { return List.Contains(item); }
		public int IndexOf(object item) { return List.IndexOf(item); }
		int lockUpdate = 0;
		internal bool IsLockUpdate { get { return lockUpdate > 0; } }
		public void BeginUpdate() { lockUpdate++; }
		public void EndUpdate() {
			if(--lockUpdate == 0)
				NotifyOwner(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		public void AddRange(object[] range) {
			BeginUpdate();
			try {
				foreach(object item in range)
					Add(item);
			}
			finally {
				EndUpdate();
			}
		}
		public object this[int index] {
			get { return List[index]; }
			set { List[index] = value; }
		}
		protected virtual void NotifyOwner(NotifyCollectionChangedEventArgs e) {
			if(IsLockUpdate)
				return;
			if (CollectionChanged != null) 
				CollectionChanged(this, e);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			NotifyOwner(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
			AddNotifyOwner(value);
		}
		protected override void OnRemoveComplete(int index, object value) {
			base.OnRemoveComplete(index, value);
			NotifyOwner(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
			RemoveNotifyOwner(value);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			NotifyOwner(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, oldValue, newValue, index));
		}
		protected override void OnClear() {
			InnerList.Clear();
			NotifyOwner(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
		internal void Assign(ListItemCollection items) {
			InnerList.Clear();
			AddRange(items.InnerList.ToArray());
		}
		void AddNotifyOwner(object item) {
			ListBoxEditItem listBoxEditItem = item as ListBoxEditItem;
			if (listBoxEditItem != null) {
				listBoxEditItem.SubscribeToSelection();
				listBoxEditItem.NotifyOwner = Owner;
			}
		}
		void RemoveNotifyOwner(object item) {
			ListBoxEditItem listBoxEditItem = item as ListBoxEditItem;
			if (listBoxEditItem != null) {
				listBoxEditItem.NotifyOwner = null;
				listBoxEditItem.UnsubscribeFromSelection();
			}
		}
		#region INotifyCollectionChanged Members
		event NotifyCollectionChangedEventHandler CollectionChanged;
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
			add { CollectionChanged += value; }
			remove { CollectionChanged -= value; }
		}
		#endregion
		internal void UpdateSelection(object selectedItems) {
			IEnumerable<object> list = selectedItems as IEnumerable<object> != null ? (IEnumerable<object>)selectedItems : new List<object>() {selectedItems};
			List<object> selectedItemsList = list.ToList();
			foreach (object item in InnerList) {
				ListBoxEditItem lbItem = item as ListBoxEditItem;
				if (lbItem == null)
					continue;
				lbItem.ChangeSelectionWithLock(selectedItemsList.Contains(item));
			}
		}
	}
}
