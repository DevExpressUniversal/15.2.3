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

using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class GroupRangeEditorForm : XtraForm {
		#region fields
		GroupRangeEditorViewModel viewModel;
		SimpleButton btnCancel;
		SimpleButton btnOk;
		ListBoxControl slctDefinedName;
		#endregion
		GroupRangeEditorForm() {
			InitializeComponent();
		}
		public GroupRangeEditorForm(GroupRangeEditorViewModel viewModel) {
			this.viewModel = viewModel;
			InitializeComponent();
			foreach(KeyValuePair<string, string> definedName in viewModel.DefinedNames)
				slctDefinedName.Items.Add(definedName.Key);
			slctDefinedName.SelectedIndex = 0;
		}
		#region Properties
		protected internal GroupRangeEditorViewModel ViewModel { get { return viewModel; } }
		#endregion
		void InitializeComponent() {
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.slctDefinedName = new DevExpress.XtraEditors.ListBoxControl();
			((System.ComponentModel.ISupportInitialize)(this.slctDefinedName)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(197, 270);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "Cancel";
			this.btnOk.Location = new System.Drawing.Point(116, 270);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(75, 23);
			this.btnOk.TabIndex = 1;
			this.btnOk.Text = "OK";
			this.btnOk.Click += btnOk_Click;
			this.slctDefinedName.Location = new System.Drawing.Point(12, 12);
			this.slctDefinedName.Name = "slctDefinedName";
			this.slctDefinedName.Size = new System.Drawing.Size(260, 252);
			this.slctDefinedName.TabIndex = 3;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(284, 298);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.slctDefinedName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GroupRangeEditorForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Sort Field";
			((System.ComponentModel.ISupportInitialize)(this.slctDefinedName)).EndInit();
			this.ResumeLayout(false);
		}
		void btnOk_Click(object sender, System.EventArgs e) {
			if (slctDefinedName.SelectedItem != null) {
				viewModel.SelectedDefinedName =  viewModel.DefinedNames[slctDefinedName.SelectedItem.ToString()];
				this.DialogResult = DialogResult.OK;
			}
		}
	}
}
