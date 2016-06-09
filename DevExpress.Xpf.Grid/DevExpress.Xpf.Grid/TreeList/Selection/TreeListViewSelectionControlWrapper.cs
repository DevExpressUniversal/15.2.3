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
using System.ComponentModel;
using DevExpress.Xpf.Core;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Grid.TreeList {
	public class TreeListViewSelectionControlWrapper : SelectionControlWrapper {
		public static void Register() {
			SelectionControlWrapper.Wrappers.Add(typeof(TreeListView), typeof(TreeListViewSelectionControlWrapper));
		}
		public TreeListViewSelectionControlWrapper(TreeListView view) {
			View = view;
		}
		protected Action<IList, IList> Action { get; set; }
		protected TreeListView View { get; set; }
		public override void SubscribeSelectionChanged(Action<IList, IList> action) {
			Action = action;
			if(View.DataControl is GridControl)
				((GridControl)View.DataControl).SelectionChanged += OnSelectionChanged;
			if(View.DataControl is TreeListControl)
				((TreeListControl)View.DataControl).SelectionChanged += OnSelectionChanged;
		}
		public override void UnsubscribeSelectionChanged() {
			if(View.DataControl is GridControl)
				((GridControl)View.DataControl).SelectionChanged -= OnSelectionChanged;
			if(View.DataControl is TreeListControl)
				((TreeListControl)View.DataControl).SelectionChanged -= OnSelectionChanged;
		}
		void OnSelectionChanged(object sender, GridSelectionChangedEventArgs e) {
			IList addedItems = null;
			IList removedItems = null;
			if(e.Action == CollectionChangeAction.Add) {
				addedItems = new object[] { View.DataProviderBase.GetRowValue(e.ControllerRow) };
			}
			if(e.Action == CollectionChangeAction.Remove) {
				removedItems = new object[] { View.DataProviderBase.GetRowValue(e.ControllerRow) };
			}
			Action(addedItems, removedItems);
		}
		public override IList GetSelectedItems() {
			return View.DataControl == null ? null : View.DataControl.SelectedItems;
		}
		public override void ClearSelection() {
			if(View.DataControl != null)
				View.DataControl.UnselectAll();
		}
		public override void SelectItem(object item) {
			TreeListNode node = View.GetNodeByContent(item);
			if(node != null)
				View.DataControl.SelectItem(node.RowHandle);
		}
		public override void UnselectItem(object item) {
			TreeListNode node = View.GetNodeByContent(item);
			if(node != null)
				View.DataControl.UnselectItem(node.RowHandle);
		}
	}
}
