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
using System.Windows;
using DevExpress.Mvvm.Native;
using System.Collections.Specialized;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.NavBar {
	public partial class NavBarItemCollection : ObservableCollection<object> {
		readonly NavBarGroup item;
		public NavBarItemCollection(NavBarGroup item) {
			this.item = item;
		}
		public object this[string name] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i] is NavBarItem && ((NavBarItem)this[i]).Name == name)
						return this[i];
					if(this[i] is FrameworkElement && ((FrameworkElement)this[i]).Name == name)
						return this[i];
				}
				return null;
			}
		}
	}
	public class SynchronizedItemCollection : ObservableCollection<NavBarItem> {
		readonly NavBarGroup group;
		readonly NavBarItemCollection source;
		public SynchronizedItemCollection(NavBarGroup group, NavBarItemCollection source) {
			this.group = group;
			this.source = source;
			ConnectToSource();
		}
		void ConnectToSource() {
			if (source == null) {
				Clear();
				return;
			}
			SyncCollectionHelper.PopulateCore(this, source, CreateNavBarItemIfNeeded);
			source.CollectionChanged += OnSourceCollectionChanged;
		}
		void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			SyncCollectionHelper.SyncCollection(e, this, source, CreateNavBarItemIfNeeded);
		}
		object CreateNavBarItemIfNeeded(object sourceObject) {
			if (sourceObject is NavBarItem)
				return sourceObject;
			NavBarItem res =  new NavBarItem() { SourceObject = sourceObject, Content = sourceObject };
			if(!(sourceObject is FrameworkElement))
				res.DataContext = sourceObject;
			return res;
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (e.OldItems != null) {
				foreach (NavBarItem element in e.OldItems) {					
					group.NavBar.With(x => x.SelectionStrategy).Do(x => x.OnItemRemoved(group, element));
					element.Group = null;
				}
			}			
			if (e.OldItems == null && group.NavBar.With(x => x.SelectionStrategy).With(x => x.LastSelectedItem) != null &&
				Equals(group.NavBar.SelectionStrategy.LastSelectedItem, e.NewItems[0]) && group.NavBar.SelectionStrategy.SelectedItem == null) {
				(e.NewItems[0] as NavBarItem).IsSelected = true;
				group.NavBar.SelectionStrategy.LastSelectedItem = null;
			}
			var list = e.NewItems ==null ? null : e.NewItems.OfType<NavBarItem>().AsEnumerable();
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset && e.NewItems == null)
				list = this.AsEnumerable();
			if (list != null) {
				foreach (NavBarItem element in list) {
					if (element.Group != null && element.Group != group)
						throw new ArgumentException(NavBarLocalizer.GetString(NavBarStringId.ItemIsAlreadyAddedToAnotherGroupException));
					element.Group = group;
				}
			}			
		}
		protected override void ClearItems() {
			while (Count != 0) {
				RemoveAt(0);
			}
		}		
	}
}
