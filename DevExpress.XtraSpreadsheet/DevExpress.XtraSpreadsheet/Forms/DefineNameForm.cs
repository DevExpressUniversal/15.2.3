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
using DevExpress.Utils;
using System.Windows.Forms;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class DefineNameForm : ReferenceEditForm {
		#region Fields
		readonly DefineNameViewModel viewModel;
		#endregion
		public DefineNameForm() {
		}
		public DefineNameForm(DefineNameViewModel viewModel)
			: base(viewModel) {
			Guard.ArgumentNotNull(viewModel, "viewModel");
			this.viewModel = viewModel;
			InitializeComponent();
			edtReference.SpreadsheetControl = viewModel.Control;
			edtReference.EditValuePrefix = "=";
			edtReference.PositionType = PositionType.Absolute;
			edtReference.IncludeSheetName = true;
			SetBindingsForm();
			SubscribeReferenceEditControlsEvents();
		}
		#region Properties
		protected new DefineNameViewModel ViewModel { get { return viewModel; } }
		#endregion
		void SetBindingsForm() {
			this.edtName.DataBindings.Add("EditValue", viewModel, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtScope.Properties.DataSource = viewModel.ScopeDataSource;
			this.edtScope.DataBindings.Add("EditValue", viewModel, "Scope", true, DataSourceUpdateMode.OnPropertyChanged);
			this.lblScope.Enabled = viewModel.NewNameMode;
			this.edtScope.DataBindings.Add("Enabled", viewModel, "NewNameMode", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtComment.DataBindings.Add("EditValue", viewModel, "Comment", true, DataSourceUpdateMode.OnPropertyChanged);
			this.edtReference.DataBindings.Add("Enabled", viewModel, "IsReferenceChangeAllowed", true, DataSourceUpdateMode.Never);
			this.edtReference.DataBindings.Add("EditValue", viewModel, "Reference", true, DataSourceUpdateMode.OnPropertyChanged);
			this.DataBindings.Add("Text", viewModel, "FormText", true, DataSourceUpdateMode.OnPropertyChanged);
		}
		void btnOk_Click(object sender, EventArgs e) {
			this.TopMost = false;
			DocumentModel.BeginUpdate();
			try {
				if (ViewModel.Validate()) {
					ViewModel.ApplyChanges();
					this.DialogResult = DialogResult.OK;
					Close();
				}
			}
			finally {
				this.TopMost = true;
				DocumentModel.EndUpdate();
			}
		}
		void btnCancel_Click(object sender, EventArgs e) {
			Close();
		}
	}
}
