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
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraGrid.Views.Base;
using System.Drawing;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class NameManagerForm : ReferenceEditForm {
		#region Fields
		readonly NameManagerViewModel viewModel;
		#endregion
		public NameManagerForm() {
		}
		public NameManagerForm(NameManagerViewModel viewModel)
			: base(viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			edtReference.SpreadsheetControl = viewModel.Control;
			edtReference.EditValuePrefix = "=";
			edtReference.PositionType = PositionType.Absolute;
			edtReference.IncludeSheetName = true;
			SetBindingsForm();
			viewModel.PropertyChanged += OnViewModelPropertyChanged;
			viewModel.SubscribeDefinedNameEvents();
			SubscribeReferenceEditControlsEvents();
		}
		#region Properties
		protected new NameManagerViewModel ViewModel { get { return viewModel; } }
		#endregion
		void SetBindingsForm() {
			this.grid.DataSource = viewModel.NamesDataSource;
			this.grid.DataBindings.Add("DataSource", viewModel, "NamesDataSource", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnEdit.DataBindings.Add("Enabled", viewModel, "IsCurrentNameValid", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnDelete.DataBindings.Add("Enabled", viewModel, "IsDeleteAllowed", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtReference.DataBindings.Add("Enabled", viewModel, "IsReferenceChangeAllowed", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtReference.DataBindings.Add("EditValue", viewModel, "Reference", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnCancelReferenceEdit.DataBindings.Add("Enabled", viewModel, "IsReferenceChanged", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnApplyReferenceChanges.DataBindings.Add("Enabled", viewModel, "IsReferenceChanged", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "CurrentNameIndex") {
				ColumnView columnView = this.grid.MainView as ColumnView;
				if (columnView != null)
					columnView.FocusedRowHandle = viewModel.CurrentNameIndex;
			}
		}
		void btnClose_Click(object sender, EventArgs e) {
			Close();
		}
		void btnNew_Click(object sender, EventArgs e) {
			ShowNewDefinedNameForm();
		}
		void btnEdit_Click(object sender, EventArgs e) {
			ShowEditDefinedNameForm();
		}
		void btnDelete_Click(object sender, EventArgs e) {
			DeleteDefinedName();
		}
		void btnCancelReferenceEdit_Click(object sender, EventArgs e) {
			viewModel.CancelReferenceChange();
		}
		void btnApplyReferenceChanges_Click(object sender, EventArgs e) {
			viewModel.ApplyReferenceChange();
			this.grid.RefreshDataSource();
		}
		void ShowNewDefinedNameForm() {
			DefineNameCommand command = new DefineNameCommand(ViewModel.Control);
			ShowDefineNameForm(command.CreateViewModel());
		}
		void ShowEditDefinedNameForm() {
			DefineNameViewModel nameViewModel = viewModel.NamesDataSource[gridView1.FocusedRowHandle].Clone();
			if (!DefinedNameEditingCancelled(nameViewModel)) {
				nameViewModel.NewNameMode = false;
				nameViewModel.IsScopeChangeAllowed = false;
				ShowDefineNameForm(nameViewModel);
			}
		}
		bool DefinedNameEditingCancelled(DefineNameViewModel nameViewModel) {
			return ViewModel.Control.InnerControl.RaiseDefinedNameEditing(
				nameViewModel.Name, 
				nameViewModel.OriginalName, 
				nameViewModel.Scope, 
				nameViewModel.ScopeIndex, 
				nameViewModel.Reference, 
				nameViewModel.Comment);
		}
		bool ShowSaveChangeReferenceForm() {
			bool savedTopmost = this.TopMost;
			try {
				this.TopMost = false;
				return SpreadsheetControl.ShowYesNoMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CanSaveChangesNameReference));
			}
			finally {
				this.TopMost = savedTopmost;
			}
		}
		void ShowDefineNameForm(DefineNameViewModel nameViewModel) {
			SpreadsheetControl control = (SpreadsheetControl)ViewModel.Control;
			XtraForm form = control.ShowDefineNameForm(nameViewModel);
			form.Closed += OnChildFormClosed;
			form.Disposed += OnChildFormDisposed;
			this.Hide();
		}
		void OnChildFormClosed(object sender, EventArgs e) {
			Form form = sender as Form;
			if (form != null)
				form.Closed -= OnChildFormClosed;
			viewModel.UpdateDefinedNames();
			FormTouchUIAdapter.Show(this);
			grid.Focus();
		}
		void OnChildFormDisposed(object sender, EventArgs e) {
			if (DocumentModel != null && (!DocumentModel.ShowReferenceSelection && !DocumentModel.ReferenceEditMode)) {
				DocumentModel.ShowReferenceSelection = true;
				DocumentModel.ReferenceEditMode = true;
				RedrawSpreadsheetControl();
			}
			SpreadsheetControl.InnerControl.OnUpdateUI();
		}
		void OnFocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			viewModel.CurrentNameIndex = e.FocusedRowHandle;
		}
		void DeleteDefinedName() {
			this.TopMost = false;
			try {
				viewModel.DeleteDefinedName();
			}
			finally {
				this.TopMost = true;
			}
		}
		void gridView1_DoubleClick(object sender, EventArgs e) {
			Point pt = gridView1.GridControl.PointToClient(Control.MousePosition);
			GridHitInfo info = gridView1.CalcHitInfo(pt);
			if (info.InRow || info.InRowCell)
				ShowEditDefinedNameForm();
		}
		void gridView1_KeyDown(object sender, KeyEventArgs e) {
			if (gridView1.FocusedRowHandle < 0)
				return;
			if (e.KeyCode == Keys.Return)
				ShowEditDefinedNameForm();
			else if (e.KeyCode == Keys.Delete)
				DeleteDefinedName();
		}
		void edtReference_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyCode == Keys.Return) {
				viewModel.ApplyReferenceChange();
				this.grid.RefreshDataSource();
			}
		}
		void NameManagerForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (viewModel.IsReferenceChanged) {
				if (ShowSaveChangeReferenceForm())
					viewModel.ApplyReferenceChange();
				else
					viewModel.CancelReferenceChange();
			}
		}
		void NameManagerForm_FormClosed(object sender, FormClosedEventArgs e) {
			viewModel.UnsubscribeDefinedNameEvents();
		}
	}
}
