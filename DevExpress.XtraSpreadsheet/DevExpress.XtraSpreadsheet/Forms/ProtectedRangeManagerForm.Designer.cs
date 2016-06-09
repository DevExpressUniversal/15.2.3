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
	partial class ProtectedRangeManagerForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProtectedRangeManagerForm));
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.btnNew = new DevExpress.XtraEditors.SimpleButton();
			this.btnModify = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.grid = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colTitle = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colReference = new DevExpress.XtraGrid.Columns.GridColumn();
			this.lblDescription = new DevExpress.XtraEditors.LabelControl();
			this.lblPermissions = new DevExpress.XtraEditors.LabelControl();
			this.btnPermissions = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Name = "btnOk";
			this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.CausesValidation = false;
			this.btnApply.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnApply.Name = "btnApply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			resources.ApplyResources(this.btnNew, "btnNew");
			this.btnNew.CausesValidation = false;
			this.btnNew.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnNew.Name = "btnNew";
			this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
			resources.ApplyResources(this.btnModify, "btnModify");
			this.btnModify.CausesValidation = false;
			this.btnModify.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnModify.Name = "btnModify";
			this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.CausesValidation = false;
			this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			resources.ApplyResources(this.grid, "grid");
			this.grid.MainView = this.gridView1;
			this.grid.Name = "grid";
			this.grid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridView1});
			this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.colTitle,
			this.colReference});
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
			this.gridView1.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.OptionsView.ShowIndicator = false;
			this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.OnFocusedRowChanged);
			this.gridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridView1_KeyDown);
			this.gridView1.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
			resources.ApplyResources(this.colTitle, "colTitle");
			this.colTitle.FieldName = "Title";
			this.colTitle.Name = "colTitle";
			this.colTitle.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
			resources.ApplyResources(this.colReference, "colReference");
			this.colReference.FieldName = "Reference";
			this.colReference.Name = "colReference";
			this.colReference.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
			resources.ApplyResources(this.lblDescription, "lblDescription");
			this.lblDescription.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblDescription.Name = "lblDescription";
			resources.ApplyResources(this.lblPermissions, "lblPermissions");
			this.lblPermissions.AccessibleRole = System.Windows.Forms.AccessibleRole.StaticText;
			this.lblPermissions.Name = "lblPermissions";
			resources.ApplyResources(this.btnPermissions, "btnPermissions");
			this.btnPermissions.CausesValidation = false;
			this.btnPermissions.Name = "btnPermissions";
			this.btnPermissions.Click += new System.EventHandler(this.btnPermissions_Click);
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnPermissions);
			this.Controls.Add(this.lblPermissions);
			this.Controls.Add(this.lblDescription);
			this.Controls.Add(this.grid);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnModify);
			this.Controls.Add(this.btnNew);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProtectedRangeManagerForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.SimpleButton btnOk;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnApply;
		private XtraEditors.SimpleButton btnNew;
		private XtraEditors.SimpleButton btnModify;
		private XtraEditors.SimpleButton btnDelete;
		private XtraGrid.GridControl grid;
		private XtraGrid.Views.Grid.GridView gridView1;
		private XtraGrid.Columns.GridColumn colTitle;
		private XtraGrid.Columns.GridColumn colReference;
		private XtraEditors.LabelControl lblDescription;
		private XtraEditors.LabelControl lblPermissions;
		private XtraEditors.SimpleButton btnPermissions;
	}
}
