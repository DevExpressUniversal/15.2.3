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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class DocumentPropertiesForm : XtraForm {
		readonly DocumentPropertiesViewModel viewModel;
		DocumentPropertiesForm() {
			InitializeComponent();
		}
		public DocumentPropertiesForm(DocumentPropertiesViewModel viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			SetBindings();
			if (!String.IsNullOrEmpty(viewModel.FileName))
				this.Text = viewModel.FileName + " " + this.Text;
		}
		#region Properties
		public DocumentPropertiesViewModel ViewModel { get { return viewModel; } }
		bool CustomTabSelected { get { return tabStatistics.SelectedTabPageIndex == 3; } }
		#endregion
		#region SetBindings
		protected internal void SetBindings() {
			this.edtTitle.DataBindings.Add("EditValue", ViewModel, "Title");
			this.edtSubject.DataBindings.Add("EditValue", ViewModel, "Subject");
			this.edtAuthor.DataBindings.Add("EditValue", ViewModel, "Creator");
			this.edtManager.DataBindings.Add("EditValue", ViewModel, "Manager");
			this.edtCompany.DataBindings.Add("EditValue", ViewModel, "Company");
			this.edtCategory.DataBindings.Add("EditValue", ViewModel, "Category");
			this.edtKeywords.DataBindings.Add("EditValue", ViewModel, "Keywords");
			this.edtComments.DataBindings.Add("EditValue", ViewModel, "Description");
			this.edtCreated.DataBindings.Add("EditValue", ViewModel, "Created");
			this.edtModified.DataBindings.Add("EditValue", ViewModel, "Modified");
			this.edtAccessed.DataBindings.Add("EditValue", ViewModel, "FileAccessed");
			this.edtPrinted.DataBindings.Add("EditValue", ViewModel, "Printed");
			this.edtLastSavedBy.DataBindings.Add("EditValue", ViewModel, "LastModifiedBy");
			this.edtFileName.DataBindings.Add("EditValue", ViewModel, "FileName");
			this.edtLocation.DataBindings.Add("EditValue", ViewModel, "FilePath");
			this.edtSize.DataBindings.Add("EditValue", ViewModel, "FileSize");
			this.edtShortName.DataBindings.Add("EditValue", ViewModel, "FileShortName");
			this.edtFileCreated.DataBindings.Add("EditValue", ViewModel, "FileCreated");
			this.edtFileModified.DataBindings.Add("EditValue", ViewModel, "FileModified");
			this.edtFileAccessed.DataBindings.Add("EditValue", ViewModel, "FileAccessed");
			this.chkReadOnly.DataBindings.Add("EditValue", ViewModel, "IsFileAttributeReadonly");
			this.chkArchive.DataBindings.Add("EditValue", ViewModel, "IsFileAttributeArchive");
			this.chkHidden.DataBindings.Add("EditValue", ViewModel, "IsFileAttributeHidden");
			this.chkSystem.DataBindings.Add("EditValue", ViewModel, "IsFileAttributeSystem");
			this.edtName.DataBindings.Add("EditValue", ViewModel, "PropertyName", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtType.Properties.DataSource = ViewModel.PropertyTypesDataSource;
			this.edtType.DataBindings.Add("EditValue", ViewModel, "PropertyType", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtValue.DataBindings.Add("EditValue", ViewModel, "PropertyValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.rgrpYesNo.DataBindings.Add("EditValue", ViewModel, "PropertyYesNoValue", true, DataSourceUpdateMode.OnPropertyChanged);
			this.grid.DataSource = ViewModel.CustomPropertiesDataSource;
			this.grid.DataBindings.Add("DataSource", ViewModel, "CustomPropertiesDataSource", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lstPredefinedProperties.DataSource = ViewModel.PredefinedCustomPropertiesDataSource;
			this.lstPredefinedProperties.DisplayMember = "Name";
			this.btnAdd.DataBindings.Add("Enabled", ViewModel, "CanAddProperty", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnModify.DataBindings.Add("Enabled", ViewModel, "CanModifyProperty", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnDelete.DataBindings.Add("Enabled", ViewModel, "CanDeleteProperty", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void SetCustomTabVisibility() {
			if (ViewModel == null || !CustomTabSelected)
				return;
			this.edtValue.DataBindings.Add("Visible", ViewModel, "ShowValueEditor", true, DataSourceUpdateMode.OnPropertyChanged);
			this.rgrpYesNo.DataBindings.Add("Visible", ViewModel, "ShowYesNoSelector", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnAdd.DataBindings.Add("Visible", ViewModel, "ShowAddPropertyButton", true, DataSourceUpdateMode.OnPropertyChanged);
			this.btnModify.DataBindings.Add("Visible", ViewModel, "ShowModifyPropertyButton", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void ClearCustomTabVisibility() {
			if (ViewModel == null || !CustomTabSelected)
				return;
			foreach (Control control in panelControl4.Controls) {
				ControlBindingsCollection bindings = control.DataBindings;
				int count = bindings.Count;
				for (int i = count - 1; i >= 0; i--) {
					Binding binding = bindings[i];
					if (binding.PropertyName == "Visible")
						bindings.Remove(binding);
				}
			}
		}
		#endregion
		void btnOk_Click(object sender, EventArgs e) {
			if (ViewModel.Validate()) {
				ViewModel.ApplyChanges();
				this.DialogResult = DialogResult.OK;
			}
		}
		void btnAdd_Click(object sender, EventArgs e) {
			ViewModel.AddProperty();
			edtName.Focus();
		}
		void btnModify_Click(object sender, EventArgs e) {
			ViewModel.ModifyProperty();
			edtName.Focus();
		}
		void btnDelete_Click(object sender, EventArgs e) {
			ViewModel.DeleteProperty();
			edtName.Focus();
		}
		void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			ViewModel.PropertyIndex = e.FocusedRowHandle;
		}
		void gridView1_Click(object sender, EventArgs e) {
			Point pt = gridView1.GridControl.PointToClient(Control.MousePosition);
			GridHitInfo info = gridView1.CalcHitInfo(pt);
			if (info.InRow || info.InRowCell)
				ViewModel.PropertyIndex = gridView1.FocusedRowHandle;
		}
		void lstPredefinedProperties_SelectedIndexChanged(object sender, EventArgs e) {
			ViewModel.ApplyPredefinedProperty(lstPredefinedProperties.SelectedIndex);
			edtName.SelectAll();
			edtName.Focus();
		}
		void tabStatistics_SelectedPageChanging(object sender, XtraTab.TabPageChangingEventArgs e) {
			ClearCustomTabVisibility();
		}
		void tabStatistics_SelectedPageChanged(object sender, XtraTab.TabPageChangedEventArgs e) {
			SetCustomTabVisibility();
		}
	}
}
