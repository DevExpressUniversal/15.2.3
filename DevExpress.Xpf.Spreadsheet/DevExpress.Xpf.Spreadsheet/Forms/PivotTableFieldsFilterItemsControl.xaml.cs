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

using DevExpress.XtraSpreadsheet.Forms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public partial class PivotTableFieldsFilterItemsControl : UserControl {
		PivotTableFieldsFilterItemsViewModel viewModel;
		bool isAllSelected = false;
		public PivotTableFieldsFilterItemsControl(PivotTableFieldsFilterItemsViewModel viewModel) {
			InitializeComponent();
			DataContext = viewModel;
			this.viewModel = viewModel;
			InitializeCheckedItems();
			if (viewModel.Axis == XtraSpreadsheet.Model.PivotTableAxis.Page)
				SubscribeEvents();
		}
		void InitializeCheckedItems() {
			this.lbItems.Items.AddRange(viewModel.Items.ToArray());
			for (int i = 0; i < viewModel.Items.Count; i++)
				if (viewModel.Items[i].IsVisible)
					this.lbItems.SelectedItems.Add(viewModel.Items[i]);
		}
		void SubscribeEvents() {
			this.lbItems.EditValueChanging += lbItems_EditValueChanging;
			this.viewModel.SelectMultipleItemsChanged += viewModel_SelectMultipleItemsChanged;
		}
		void lbItems_EditValueChanging(object sender, Editors.EditValueChangingEventArgs e) {
			isAllSelected = false;
			object selectedItem = FindSelectedItem(e.NewValue as List<object>, e.OldValue as List<object>);
			Dispatcher.BeginInvoke(new Action(() => SelectItem(selectedItem)), DispatcherPriority.Render);
		}
		void viewModel_SelectMultipleItemsChanged(object sender, EventArgs e) {
			if (!viewModel.SelectMultipleItems && this.lbItems.SelectedItems.Count > 1) {
				lbItems.EditValueChanging -= lbItems_EditValueChanging;
				lbItems.SelectAll();
				lbItems.EditValueChanging += lbItems_EditValueChanging;
			}
		}
		void SelectItem(object selectedItem) {
			lbItems.EditValueChanging -= lbItems_EditValueChanging;
			try {
				if (!viewModel.SelectMultipleItems) {
					lbItems.UnSelectAll();
					if (selectedItem == null) {
						if (isAllSelected)
							lbItems.SelectAll();
						else
							lbItems.SelectedIndex = -1;
					}
					else
						lbItems.SelectedIndex = lbItems.Items.IndexOf(selectedItem);
				}
			}
			finally {
				lbItems.EditValueChanging += lbItems_EditValueChanging;
			}
		}
		object FindSelectedItem(List<object> newValue, List<object> oldValue) {
			if (newValue != null && newValue.Count == viewModel.Items.Count) {
				isAllSelected = true;
				return null;
			}
			if (newValue == null)
				return null;
			if (oldValue == null)
				return newValue[0];
			return (newValue.Count > oldValue.Count) ? GetExceptedItem(newValue, oldValue) : GetExceptedItem(oldValue, newValue);
		}
		object GetExceptedItem(List<object> first, List<object> second) {
			for (int i = 0; i < first.Count; i++) {
				object currentItem = first[i];
				if (!second.Contains(currentItem))
					return currentItem;
			}
			return null;
		}
		public void UpdateViewModel() {
			foreach (PivotItemFilterInfo itemInfo in viewModel.Items)
				itemInfo.IsVisible = false;
			for (int i = 0; i < this.lbItems.SelectedItems.Count; i++) {
				int index = viewModel.Items.IndexOf(this.lbItems.SelectedItems[i] as PivotItemFilterInfo);
				viewModel.Items[index].IsVisible = true;
			}
		}
	}
}
