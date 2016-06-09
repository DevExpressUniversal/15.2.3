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
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
namespace DevExpress.Xpf.Bars {
	[ContentWrapper(typeof(IBarItem))]
	public class CommonBarItemCollection : ObservableCollection<IBarItem> {
		BarItemLinkCollection BarItemLinkCollection { get; set; }
		internal ILogicalChildrenContainer LogicalParent { get; set; }
		public CommonBarItemCollection()
			: this(new Bar()) {
			if (!new DependencyObject().IsInDesignTool()) {
				throw new InvalidOperationException();
			}
		}
		public CommonBarItemCollection(ILinksHolder logicalParent) {
			LogicalParent = logicalParent;
			var fe = LogicalParent as FrameworkElement;
			if(fe!=null)
				fe.IsEnabledChanged += OnParentIsEnabledChanged;
			BarItemLinkCollection = logicalParent.Links;
		}
		void OnParentIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			foreach (var element in this.OfType<DependencyObject>().ToList())
				element.CoerceValue(FrameworkElement.IsEnabledProperty);
		}
		protected override void InsertItem(int index, IBarItem item) {
			OnInsertItem(index, item);
		}
		private void OnInsertItem(int index, IBarItem item) {
			using(var locker = BarItemLinkCollection.LockItemInsertationToCommonCollection()) {
				base.InsertItem(index, item);
				if (item is DependencyObject && LogicalTreeHelper.GetParent(item as DependencyObject) == null)
					LogicalParent.AddLogicalChild(item);
				if (item is BarItem) {
					var bItem = item as BarItem;
					bItem.SkipAddToBarManagerLogicalTree = true;
					var link = bItem.CreateLink() ?? new BarItemLink() { Item = (BarItem)item, IsPrivate = true };
					if (!String.IsNullOrEmpty(bItem.Name))
						link.Name = bItem.Name + "3C0A5E61167740189D73F8BF55CD4326";
					BarItemLinkCollection.Insert(index, link);
					LogicalParent.AddLogicalChild(link);
					link.CommonBarItemCollectionLink = true;
				} else {
					BarItemLinkCollection.Insert(index, item as BarItemLinkBase);
				}				
			}
		}
		protected override void ClearItems() {
			this.ToList().ForEach(OnRemoveItem);
			base.ClearItems();
		}
		protected override void RemoveItem(int index) {
			using(var locker = BarItemLinkCollection.LockItemRemovalFromCommonCollection()) {
				IBarItem barItem = this[index];
				base.RemoveItem(index);
				OnRemoveItem(barItem);
			}			
		}
		private void OnRemoveItem(IBarItem barItem) {
			if(barItem is BarItemLinkBase) {
				BarItemLinkCollection.Remove(barItem as BarItemLinkBase);
			}
			if(barItem is BarItem) {
				foreach(BarItemLinkBase linkBase in (barItem as BarItem).Links.Where(l => l.CommonBarItemCollectionLink).ToList()) {
					BarItemLinkCollection.Remove(linkBase);
					LogicalParent.RemoveLogicalChild(linkBase);
				}
				if(LogicalTreeHelper.GetParent((BarItem)barItem) == LogicalParent)
					LogicalParent.RemoveLogicalChild(barItem);
			}
		}		
		protected override void SetItem(int index, IBarItem item) {
			base.SetItem(index, item);
			OnRemoveItem(this[index]);
			OnInsertItem(index, item);
		}				
	}
}
