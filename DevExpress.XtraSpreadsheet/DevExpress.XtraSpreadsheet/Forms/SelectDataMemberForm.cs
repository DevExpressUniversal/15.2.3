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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors.Native;
namespace DevExpress.XtraSpreadsheet.Forms {
	public class SelectDataMemberForm : XtraForm {
		#region Fields
		readonly SelectDataMemberViewModel viewModel;
		SimpleButton btnCancel;
		SimpleButton btnOk;
		SpreadsheetFieldListTreeView fieldList;
		SpreadsheetControl control;
		#endregion
		SelectDataMemberForm() {
			InitializeComponent();
		}
		public SelectDataMemberForm(SelectDataMemberViewModel viewModel, SpreadsheetControl control) {
			this.viewModel = viewModel;
			this.control = control;
			InitializeComponent();
			InitializeEditors();
		}
		#region Properties
		public SelectDataMemberViewModel ViewModel { get { return viewModel; } }
		#endregion
		void InitializeEditors() {
			fieldList.RefreshTreeList();
			fieldList.SelectNode(viewModel.DataMember);
		}
		void InitializeComponent() {
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.fieldList = new SpreadsheetFieldListTreeView();
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
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			this.fieldList.Location = new System.Drawing.Point(12, 12);
			this.fieldList.Name = "fieldList";
			this.fieldList.Size = new System.Drawing.Size(260, 252);
			this.fieldList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.fieldList.SpreadsheetControl = control;
			this.fieldList.NeedDoubleClick = false;
			this.fieldList.OnlyDataMembersMode = true;
			this.fieldList.SelectionChanged += fieldList_SelectionChanged;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(284, 298);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(fieldList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectDataMemberForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Select Data Member";
			this.ResumeLayout(false);
		}
		void fieldList_SelectionChanged(object sender, EventArgs e) {
			string shortResult = fieldList.GetSelectedNodeShortName();
			if (!String.IsNullOrEmpty(shortResult)) {
				viewModel.DataMember = shortResult;
			}
		}
		void btnOk_Click(object sender, EventArgs e) {
			viewModel.DataMember = fieldList.GetSelectedNodeShortName();
			this.DialogResult = DialogResult.OK;
		}
		protected override void Dispose(bool disposing) {
			this.fieldList.SelectionChanged -= fieldList_SelectionChanged;
			base.Dispose(disposing);
		}
	}
}
