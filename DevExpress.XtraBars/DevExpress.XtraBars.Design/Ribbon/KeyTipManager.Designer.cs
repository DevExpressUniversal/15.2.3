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

namespace DevExpress.XtraBars.Ribbon.Design {
	partial class KeyTipManager {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				ClearToolbarEvents();
				ClearPropertyGridEvents();
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.treeList1 = new DevExpress.XtraTreeList.TreeList();
			this.nameColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.typeColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.captionColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.userKeyTipColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.finalKeyTipColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.keyTipManagerToolbar1 = new DevExpress.XtraBars.Ribbon.Design.KeyTipManagerToolbar();
			this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.treeList1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(377, 137);
			this.splMain.Size = new System.Drawing.Size(6, 430);
			this.pgMain.Location = new System.Drawing.Point(383, 137);
			this.pgMain.Size = new System.Drawing.Size(389, 430);
			this.pnlControl.Controls.Add(this.keyTipManagerToolbar1);
			this.pnlControl.Size = new System.Drawing.Size(772, 109);
			this.lbCaption.Size = new System.Drawing.Size(772, 42);
			this.pnlMain.Controls.Add(this.treeList1);
			this.pnlMain.Location = new System.Drawing.Point(0, 137);
			this.pnlMain.Size = new System.Drawing.Size(377, 430);
			this.horzSplitter.Size = new System.Drawing.Size(772, 4);
			this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.nameColumn,
			this.typeColumn,
			this.captionColumn,
			this.userKeyTipColumn,
			this.finalKeyTipColumn});
			this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeList1.Location = new System.Drawing.Point(0, 0);
			this.treeList1.Name = "treeList1";
			this.treeList1.OptionsView.ShowVertLines = false;
			this.treeList1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemTextEdit1});
			this.treeList1.Size = new System.Drawing.Size(377, 430);
			this.treeList1.TabIndex = 0;
			this.treeList1.TreeLevelWidth = 20;
			this.treeList1.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.treeList1_CustomDrawNodeCell);
			this.treeList1.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.treeList1_ShowingEditor);
			this.treeList1.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeList1_FocusedNodeChanged);
			this.treeList1.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeList1_CellValueChanged);
			this.nameColumn.Caption = "Name";
			this.nameColumn.FieldName = "Name";
			this.nameColumn.MinWidth = 28;
			this.nameColumn.Name = "nameColumn";
			this.nameColumn.OptionsColumn.AllowEdit = false;
			this.nameColumn.OptionsColumn.AllowMove = false;
			this.nameColumn.OptionsColumn.AllowMoveToCustomizationForm = false;
			this.nameColumn.OptionsColumn.AllowSort = false;
			this.nameColumn.Visible = true;
			this.nameColumn.VisibleIndex = 0;
			this.typeColumn.Caption = "Type";
			this.typeColumn.FieldName = "Type";
			this.typeColumn.Name = "typeColumn";
			this.typeColumn.OptionsColumn.AllowEdit = false;
			this.typeColumn.OptionsColumn.AllowMove = false;
			this.typeColumn.OptionsColumn.AllowMoveToCustomizationForm = false;
			this.typeColumn.OptionsColumn.AllowSort = false;
			this.typeColumn.Visible = true;
			this.typeColumn.VisibleIndex = 1;
			this.captionColumn.Caption = "Caption";
			this.captionColumn.FieldName = "Caption";
			this.captionColumn.Name = "captionColumn";
			this.captionColumn.OptionsColumn.AllowEdit = false;
			this.captionColumn.OptionsColumn.AllowMove = false;
			this.captionColumn.OptionsColumn.AllowMoveToCustomizationForm = false;
			this.captionColumn.OptionsColumn.AllowSort = false;
			this.captionColumn.Visible = true;
			this.captionColumn.VisibleIndex = 2;
			this.userKeyTipColumn.Caption = "User Key Tip";
			this.userKeyTipColumn.ColumnEdit = this.repositoryItemTextEdit1;
			this.userKeyTipColumn.FieldName = "UserKeyTip";
			this.userKeyTipColumn.Name = "userKeyTipColumn";
			this.userKeyTipColumn.OptionsColumn.AllowMove = false;
			this.userKeyTipColumn.OptionsColumn.AllowMoveToCustomizationForm = false;
			this.userKeyTipColumn.OptionsColumn.AllowSort = false;
			this.userKeyTipColumn.OptionsColumn.FixedWidth = true;
			this.userKeyTipColumn.Visible = true;
			this.userKeyTipColumn.VisibleIndex = 3;
			this.finalKeyTipColumn.Caption = "Final Key Tip";
			this.finalKeyTipColumn.FieldName = "FinalKeyTip";
			this.finalKeyTipColumn.Name = "finalKeyTipColumn";
			this.finalKeyTipColumn.OptionsColumn.AllowEdit = false;
			this.finalKeyTipColumn.OptionsColumn.AllowMove = false;
			this.finalKeyTipColumn.OptionsColumn.AllowMoveToCustomizationForm = false;
			this.finalKeyTipColumn.OptionsColumn.AllowSort = false;
			this.finalKeyTipColumn.OptionsColumn.FixedWidth = true;
			this.finalKeyTipColumn.Visible = true;
			this.finalKeyTipColumn.VisibleIndex = 4;
			this.keyTipManagerToolbar1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.keyTipManagerToolbar1.Location = new System.Drawing.Point(4, 4);
			this.keyTipManagerToolbar1.Name = "keyTipManagerToolbar1";
			this.keyTipManagerToolbar1.Size = new System.Drawing.Size(764, 101);
			this.keyTipManagerToolbar1.TabIndex = 0;
			this.repositoryItemTextEdit1.AutoHeight = false;
			this.repositoryItemTextEdit1.MaxLength = 3;
			this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "KeyTipManager";
			this.Size = new System.Drawing.Size(772, 567);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraTreeList.TreeList treeList1;
		private DevExpress.XtraTreeList.Columns.TreeListColumn nameColumn;
		private DevExpress.XtraTreeList.Columns.TreeListColumn typeColumn;
		private DevExpress.XtraTreeList.Columns.TreeListColumn captionColumn;
		private DevExpress.XtraTreeList.Columns.TreeListColumn userKeyTipColumn;
		private DevExpress.XtraTreeList.Columns.TreeListColumn finalKeyTipColumn;
		private KeyTipManagerToolbar keyTipManagerToolbar1;
		private XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
	}
}
