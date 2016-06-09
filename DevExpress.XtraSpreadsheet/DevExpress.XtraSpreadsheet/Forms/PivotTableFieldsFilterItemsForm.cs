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

using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class PivotTableFieldsFilterItemsForm : XtraForm {
		#region Field
		readonly PivotTableFieldsFilterItemsViewModel viewModel;
		#endregion
		PivotTableFieldsFilterItemsForm() {
			InitializeComponent();
		}
		public PivotTableFieldsFilterItemsForm(PivotTableFieldsFilterItemsViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetBindingsForm();
			InitializeCheckedItems();
			if (viewModel.Axis == PivotTableAxis.Page) 
				SubscribeEvents();
			else 
				RecalculateForm();
		}
		void RecalculateForm() {
			System.Drawing.Size bestSize = new System.Drawing.Size(347, 296);
			this.chklbPageFieldItems.Size = bestSize;
		}
		#region Properties
		protected PivotTableFieldsFilterItemsViewModel ViewModel { get { return viewModel; } }
		#endregion
		void SetBindingsForm() {
			this.chklbPageFieldItems.Items.AddRange(ViewModel.Items.ToArray());
			this.chkSelectMultipleItems.DataBindings.Add("EditValue", ViewModel, "SelectMultipleItems", false, DataSourceUpdateMode.OnPropertyChanged);
			this.chkSelectMultipleItems.DataBindings.Add("Visible", ViewModel, "SelectMultipleItemsVisible", false, DataSourceUpdateMode.OnPropertyChanged);
		}
		void InitializeCheckedItems() {
			for (int i = 0; i < viewModel.Items.Count; i++)
				this.chklbPageFieldItems.SetItemChecked(i, viewModel.Items[i].IsVisible);
		}
		void SubscribeEvents() {
			viewModel.SelectMultipleItemsChanged += OnSelectMultipleItemsChanged;
			this.chklbPageFieldItems.ItemChecking += OnItemChecking;
			this.chklbPageFieldItems.ItemCheck += OnItemCheck;
		}
		void OnSelectMultipleItemsChanged(object sender, System.EventArgs e) {
			if (!viewModel.SelectMultipleItems && this.chklbPageFieldItems.CheckedItems.Count > 1)
				CheckAllItems();
		}
		void OnItemChecking(object sender, XtraEditors.Controls.ItemCheckingEventArgs e) {
			if (!viewModel.SelectMultipleItems) {
				int checkedItems = chklbPageFieldItems.CheckedItems.Count;
				UnCheckAllItems();
				if (checkedItems > 1) {
					e.Cancel = true;
					CheckOneItem(e.Index, true);
				}
				else
					CheckOneItem(e.Index, e.NewValue == CheckState.Checked ? true : false);
			}
		}
		void OnItemCheck(object sender, XtraEditors.Controls.ItemCheckEventArgs e) {
			this.btnOk.Enabled = this.chklbPageFieldItems.CheckedItems.Count > 0 ? true : false;
		}
		void CheckAllItems() {
			chklbPageFieldItems.ItemChecking -= OnItemChecking;
			this.chklbPageFieldItems.CheckAll();
			chklbPageFieldItems.ItemChecking += OnItemChecking;
		}
		void UnCheckAllItems() {
			chklbPageFieldItems.ItemChecking -= OnItemChecking;
			this.chklbPageFieldItems.UnCheckAll();
			chklbPageFieldItems.ItemChecking += OnItemChecking;
		}
		void CheckOneItem(int index, bool state) {
			chklbPageFieldItems.ItemChecking -= OnItemChecking;
			this.chklbPageFieldItems.SetItemChecked(index, state);
			chklbPageFieldItems.ItemChecking += OnItemChecking;
		}
		void UpdateViewModel() {
			foreach (PivotItemFilterInfo itemInfo in viewModel.Items)
				itemInfo.IsVisible = false;
			for (int i = 0; i < this.chklbPageFieldItems.CheckedItems.Count; i++) {
				int index = viewModel.Items.IndexOf(this.chklbPageFieldItems.CheckedItems[i] as PivotItemFilterInfo);
				viewModel.Items[index].IsVisible = true;
			}
		}
		void btnOk_Click(object sender, System.EventArgs e) {
			this.TopMost = false;
			try {
				UpdateViewModel();
				ViewModel.ApplyChanges();
				this.DialogResult = DialogResult.OK;
				Close();
			}
			finally {
				this.TopMost = true;
			}
		}
	}
}
