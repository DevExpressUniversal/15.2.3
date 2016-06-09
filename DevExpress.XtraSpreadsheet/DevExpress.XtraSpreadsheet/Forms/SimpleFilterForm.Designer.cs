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
	partial class SimpleFilterForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleFilterForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnCheckAll = new DevExpress.XtraEditors.SimpleButton();
			this.btnUncheckAll = new DevExpress.XtraEditors.SimpleButton();
			this.edtValues = new DevExpress.XtraTreeList.TreeList();
			((System.ComponentModel.ISupportInitialize)(this.edtValues)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			resources.ApplyResources(this.btnCheckAll, "btnCheckAll");
			this.btnCheckAll.CausesValidation = false;
			this.btnCheckAll.Name = "btnCheckAll";
			this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
			resources.ApplyResources(this.btnUncheckAll, "btnUncheckAll");
			this.btnUncheckAll.CausesValidation = false;
			this.btnUncheckAll.Name = "btnUncheckAll";
			this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
			this.edtValues.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			resources.ApplyResources(this.edtValues, "edtDateValues");
			this.edtValues.KeyFieldName = "Id";
			this.edtValues.Name = "edtDateValues";
			this.edtValues.OptionsBehavior.AllowIndeterminateCheckState = true;
			this.edtValues.OptionsBehavior.Editable = false;
			this.edtValues.OptionsBehavior.ImmediateEditor = false;
			this.edtValues.OptionsFilter.AllowFilterEditor = false;
			this.edtValues.OptionsView.ShowCheckBoxes = true;
			this.edtValues.OptionsView.ShowColumns = false;
			this.edtValues.OptionsView.ShowHorzLines = false;
			this.edtValues.OptionsView.ShowIndicator = false;
			this.edtValues.OptionsView.ShowVertLines = false;
			this.edtValues.ParentFieldName = "ParentId";
			this.edtValues.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.edtValues_AfterCheckNode);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.edtValues);
			this.Controls.Add(this.btnUncheckAll);
			this.Controls.Add(this.btnCheckAll);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SimpleFilterForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.edtValues)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnCheckAll;
		private XtraEditors.SimpleButton btnUncheckAll;
		private DevExpress.XtraTreeList.TreeList edtValues;
	}
}
