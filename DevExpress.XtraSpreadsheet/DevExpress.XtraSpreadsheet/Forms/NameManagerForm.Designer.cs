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
	partial class NameManagerForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NameManagerForm));
			this.edtReference = new DevExpress.XtraSpreadsheet.ReferenceEditControl();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.lblReference = new DevExpress.XtraEditors.LabelControl();
			this.btnNew = new DevExpress.XtraEditors.SimpleButton();
			this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.grid = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colReference = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colScope = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colComment = new DevExpress.XtraGrid.Columns.GridColumn();
			this.btnCancelReferenceEdit = new DevExpress.XtraEditors.SimpleButton();
			this.btnApplyReferenceChanges = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.edtReference.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			this.SuspendLayout();
			this.edtReference.Activated = false;
			resources.ApplyResources(this.edtReference, "edtReference");
			this.edtReference.EditValuePrefix = null;
			this.edtReference.IncludeSheetName = false;
			this.edtReference.Name = "edtReference";
			this.edtReference.PositionType = DevExpress.XtraSpreadsheet.Model.PositionType.Relative;
			this.edtReference.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.edtReference.SpreadsheetControl = null;
			this.edtReference.KeyDown += new System.Windows.Forms.KeyEventHandler(this.edtReference_KeyDown);
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.CausesValidation = false;
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			resources.ApplyResources(this.lblReference, "lblReference");
			this.lblReference.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblReference.Name = "lblReference";
			resources.ApplyResources(this.btnNew, "btnNew");
			this.btnNew.CausesValidation = false;
			this.btnNew.Name = "btnNew";
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			resources.ApplyResources(this.btnEdit, "btnEdit");
			this.btnEdit.CausesValidation = false;
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.CausesValidation = false;
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			resources.ApplyResources(this.grid, "grid");
			this.grid.MainView = this.gridView1;
			this.grid.Name = "grid";
			this.grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridView1});
			this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.colName,
			this.colReference,
			this.colScope,
			this.colComment});
			this.gridView1.GridControl = this.grid;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsBehavior.Editable = false;
			this.gridView1.OptionsCustomization.AllowFilter = false;
			this.gridView1.OptionsCustomization.AllowGroup = false;
			this.gridView1.OptionsCustomization.AllowSort = false;
			this.gridView1.OptionsMenu.EnableColumnMenu = false;
			this.gridView1.OptionsMenu.EnableFooterMenu = false;
			this.gridView1.OptionsMenu.EnableGroupPanelMenu = false;
			this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.OptionsView.ShowIndicator = false;
			this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.OnFocusedRowChanged);
			this.gridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyDown);
			this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
			resources.ApplyResources(this.colName, "colName");
			this.colName.FieldName = "Name";
			this.colName.Name = "colName";
			this.colName.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
			resources.ApplyResources(this.colReference, "colReference");
			this.colReference.FieldName = "Reference";
			this.colReference.Name = "colReference";
			this.colReference.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
			resources.ApplyResources(this.colScope, "colScope");
			this.colScope.FieldName = "Scope";
			this.colScope.Name = "colScope";
			this.colScope.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
			resources.ApplyResources(this.colComment, "colComment");
			this.colComment.FieldName = "Comment";
			this.colComment.Name = "colComment";
			this.colComment.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
			resources.ApplyResources(this.btnCancelReferenceEdit, "btnCancelReferenceEdit");
			this.btnCancelReferenceEdit.CausesValidation = false;
			this.btnCancelReferenceEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelReferenceEdit.Image")));
			this.btnCancelReferenceEdit.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnCancelReferenceEdit.Name = "btnCancelReferenceEdit";
			this.btnCancelReferenceEdit.Click += new System.EventHandler(this.btnCancelReferenceEdit_Click);
			resources.ApplyResources(this.btnApplyReferenceChanges, "btnApplyReferenceChanges");
			this.btnApplyReferenceChanges.CausesValidation = false;
			this.btnApplyReferenceChanges.Image = ((System.Drawing.Image)(resources.GetObject("btnApplyReferenceChanges.Image")));
			this.btnApplyReferenceChanges.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnApplyReferenceChanges.Name = "btnApplyReferenceChanges";
			this.btnApplyReferenceChanges.Click += new System.EventHandler(this.btnApplyReferenceChanges_Click);
			this.AccessibleRole = System.Windows.Forms.AccessibleRole.Window;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.Controls.Add(this.btnApplyReferenceChanges);
			this.Controls.Add(this.btnCancelReferenceEdit);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.btnNew);
			this.Controls.Add(this.lblReference);
			this.Controls.Add(this.edtReference);
			this.Controls.Add(this.btnClose);
			this.DoubleBuffered = true;
			this.Name = "NameManagerForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NameManagerForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NameManagerForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.edtReference.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnClose;
		private ReferenceEditControl edtReference;
		private XtraEditors.LabelControl lblReference;
		private XtraEditors.SimpleButton btnNew;
		private XtraEditors.SimpleButton btnEdit;
		private XtraEditors.SimpleButton btnDelete;
		private XtraGrid.GridControl grid;
		private XtraGrid.Views.Grid.GridView gridView1;
		private XtraEditors.SimpleButton btnCancelReferenceEdit;
		private XtraEditors.SimpleButton btnApplyReferenceChanges;
		private XtraGrid.Columns.GridColumn colName;
		private XtraGrid.Columns.GridColumn colReference;
		private XtraGrid.Columns.GridColumn colScope;
		private XtraGrid.Columns.GridColumn colComment;
	}
}
