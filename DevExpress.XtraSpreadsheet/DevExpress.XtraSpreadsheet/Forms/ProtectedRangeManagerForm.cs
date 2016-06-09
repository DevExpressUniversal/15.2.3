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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Model;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class ProtectedRangeManagerForm : XtraForm {
		ProtectedRangeManagerViewModel viewModel;
		public ProtectedRangeManagerForm() {
			InitializeComponent();
		}
		public ProtectedRangeManagerForm(ProtectedRangeManagerViewModel viewModel) {
			InitializeComponent();
			gridView1.ActiveFilterString = "[IsDeleted] == false";
			this.ViewModel = viewModel;
			viewModel.PropertyChanged += OnViewModelPropertyChanged;
		}
		#region Properties
		public ProtectedRangeManagerViewModel ViewModel {
			get { return viewModel; }
			set {
				if (value == viewModel)
					return;
				Guard.ArgumentNotNull(value, "viewModel");
				this.viewModel = value;
				SetBindings();
			}
		}
		protected ISpreadsheetControl SpreadsheetControl { get { return ViewModel.Control; } }
		protected DevExpress.XtraSpreadsheet.SpreadsheetControl XtraSpreadsheetControl { get { return SpreadsheetControl as DevExpress.XtraSpreadsheet.SpreadsheetControl; } }
		protected DocumentModel DocumentModel { get { return SpreadsheetControl.InnerControl.DocumentModel; } }
		#endregion
		protected internal virtual void SetBindings() {
			if (ViewModel == null)
				return;
			this.grid.DataSource = viewModel.ProtectedRangesDataSource;
			this.grid.DataBindings.Add("DataSource", viewModel, "ProtectedRangesDataSource", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnModify.DataBindings.Add("Enabled", viewModel, "IsCurrentRangeValid", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnDelete.DataBindings.Add("Enabled", viewModel, "IsCurrentRangeValid", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblPermissions.DataBindings.Add("Enabled", viewModel, "IsCurrentRangeValid", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnPermissions.DataBindings.Add("Enabled", viewModel, "IsCurrentRangeValid", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnApply.DataBindings.Add("Enabled", viewModel, "HasPendingChanges", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "ProtectedRangesDataSource")
				this.grid.RefreshDataSource();
		}
		void DeleteProtectedRange() {
			ViewModel.DeleteCurrentProtectedRange();
			this.grid.RefreshDataSource();
		}
		void ShowNewProtectedRangeForm() {
			ProtectedRangeViewModel rangeViewModel = ViewModel.CreateNewRangeViewModel();
			ShowProtectedRangeForm(rangeViewModel);
		}
		void ShowEditProtectedRangeForm() {
			ProtectedRangeViewModel rangeViewModel = ViewModel.CreateEditCurrentRangeViewModel();
			ShowProtectedRangeForm(rangeViewModel);
		}
		void ShowProtectedRangeForm(ProtectedRangeViewModel rangeViewModel) {
			SpreadsheetControl control = (SpreadsheetControl)ViewModel.Control;
			XtraForm form = control.ShowProtectedRangeForm(rangeViewModel);
			form.Closed += OnChildFormClosed;
			form.Disposed += OnChildFormDisposed;
			this.Hide();
		}
		void OnChildFormClosed(object sender, EventArgs e) {
			Form form = sender as Form;
			if (form != null)
				form.Closed -= OnChildFormClosed;
			ProtectRangeForm protectedRangeForm = form as ProtectRangeForm;
			if (protectedRangeForm != null && protectedRangeForm.DialogResult == DialogResult.OK)
				ViewModel.ApplyRangeChange(protectedRangeForm.ViewModel);
			this.grid.RefreshDataSource();
			FormTouchUIAdapter.Show(this);
			grid.Focus();
		}
		void OnChildFormDisposed(object sender, EventArgs e) {
			if (DocumentModel != null) {
				DocumentModel.ShowReferenceSelection = false;
				DocumentModel.ReferenceEditMode = false;
				RedrawSpreadsheetControl();
			}
			SpreadsheetControl.InnerControl.OnUpdateUI();
		}
		#region RedrawSpreadsheetControl
		protected void RedrawSpreadsheetControl() {
			if (SpreadsheetControl.InnerControl.Owner != null)
				SpreadsheetControl.InnerControl.Owner.Redraw();
		}
		#endregion
		void OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			viewModel.CurrentRangeIndex = e.FocusedRowHandle;
		}
		void gridView1_DoubleClick(object sender, EventArgs e) {
			Point pt = gridView1.GridControl.PointToClient(Control.MousePosition);
			GridHitInfo info = gridView1.CalcHitInfo(pt);
			if (info.InRow || info.InRowCell)
				ShowEditProtectedRangeForm();
		}
		void gridView1_KeyDown(object sender, KeyEventArgs e) {
			if (gridView1.FocusedRowHandle < 0)
				return;
			if (e.KeyCode == Keys.Return)
				ShowEditProtectedRangeForm();
			else if (e.KeyCode == Keys.Delete)
				DeleteProtectedRange();
		}
		protected internal virtual void btnOk_Click(object sender, EventArgs e) {
			ViewModel.ApplyChanges();
			Close();
		}
		void btnApply_Click(object sender, EventArgs e) {
			ViewModel.ApplyChanges();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			Close();
		}
		void btnNew_Click(object sender, EventArgs e) {
			ShowNewProtectedRangeForm();
		}
		void btnModify_Click(object sender, EventArgs e) {
			ShowEditProtectedRangeForm();
		}
		void btnDelete_Click(object sender, EventArgs e) {
			DeleteProtectedRange();
		}
		void btnPermissions_Click(object sender, EventArgs e) {
			ViewModel.EditCurrentRangePermissions();
		}
	}
}
