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

namespace DevExpress.XtraSpreadsheet.Forms {
	partial class DataMemberEditorForm {
		private System.ComponentModel.IContainer components = null;
		#region Windows Form Designer generated code
		void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataMemberEditorForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.edtDataMember = new DevExpress.XtraEditors.TextEdit();
			this.slctDataMember = new DevExpress.XtraEditors.ComboBoxEdit();
			this.fieldList = new DevExpress.XtraSpreadsheet.SpreadsheetFieldListTreeView();
			((System.ComponentModel.ISupportInitialize)(this.edtDataMember.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.slctDataMember.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.fieldList)).BeginInit();
			this.SuspendLayout();
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.edtDataMember, "edtDataMember");
			this.edtDataMember.Name = "edtDataMember";
			this.edtDataMember.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtDataMember_KeyDown);
			resources.ApplyResources(this.slctDataMember, "slctDataMember");
			this.slctDataMember.Name = "slctDataMember";
			this.slctDataMember.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("slctDataMember.Properties.Buttons"))))});
			this.slctDataMember.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.slctDataMember.SelectedIndexChanged += new System.EventHandler(this.slctDataMember_SelectedIndexChanged);
			this.fieldList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.fieldList.DraggedNode = null;
			resources.ApplyResources(this.fieldList, "fieldList");
			this.fieldList.Name = "fieldList";
			this.fieldList.NeedDoubleClick = false;
			this.fieldList.OnlyDataMembersMode = true;
			this.fieldList.SelectionChanged += fieldList_SelectionChanged;
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.edtDataMember);
			this.Controls.Add(this.slctDataMember);
			this.Controls.Add(this.fieldList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DataMemberEditorForm";
			((System.ComponentModel.ISupportInitialize)(this.edtDataMember.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.slctDataMember.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.fieldList)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		DevExpress.XtraEditors.SimpleButton btnCancel;
		DevExpress.XtraEditors.SimpleButton btnOk;
		DevExpress.XtraEditors.TextEdit edtDataMember;
		SpreadsheetFieldListTreeView fieldList;
		DevExpress.XtraEditors.ComboBoxEdit slctDataMember;
	}
}
