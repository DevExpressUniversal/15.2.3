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

using DevExpress.Utils.Design;
namespace DevExpress.XtraBars.Design.Frames {
	partial class DocumentsFrame {
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentsFrame));
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.treeView1 = new DevExpress.Utils.Design.DXTreeView();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.btnPopulate = new DevExpress.XtraEditors.SimpleButton();
			this.btnClear = new DevExpress.XtraEditors.SimpleButton();
			this.imageList1 = new System.Windows.Forms.ImageList();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(160, 114);
			this.splMain.Size = new System.Drawing.Size(5, 174);
			this.pgMain.Location = new System.Drawing.Point(165, 114);
			this.pgMain.Size = new System.Drawing.Size(311, 174);
			this.pnlControl.Controls.Add(this.btnClear);
			this.pnlControl.Controls.Add(this.btnPopulate);
			this.pnlControl.Controls.Add(this.btnDelete);
			this.pnlControl.Controls.Add(this.btnAdd);
			this.pnlControl.Size = new System.Drawing.Size(476, 68);
			this.pnlControl.TabIndex = 1;
			this.lbCaption.Size = new System.Drawing.Size(476, 42);
			this.pnlMain.Controls.Add(this.groupControl1);
			this.pnlMain.Location = new System.Drawing.Point(0, 114);
			this.pnlMain.Margin = new System.Windows.Forms.Padding(0);
			this.pnlMain.Size = new System.Drawing.Size(160, 174);
			this.horzSplitter.Size = new System.Drawing.Size(476, 4);
			this.horzSplitter.Visible = false;
			this.btnDelete.Enabled = false;
			this.btnDelete.Location = new System.Drawing.Point(134, 19);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(128, 30);
			this.btnDelete.TabIndex = 3;
			this.btnDelete.Text = "&Delete Document";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			this.btnAdd.Location = new System.Drawing.Point(0, 19);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(128, 30);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "Add &New Document";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.treeView1.AllowSkinning = true;
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeView1.DefaultExpandCollapseButtonOffset = 5;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.HideSelection = false;
			this.treeView1.LabelEdit = true;
			this.treeView1.Location = new System.Drawing.Point(2, 21);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.Standard;
			this.treeView1.Size = new System.Drawing.Size(156, 151);
			this.treeView1.TabIndex = 0;
			this.treeView1.GetNodeEditText += new DevExpress.Utils.Design.TreeViewGetNodeEditTextEventHandler(this.tree_GetNodeEditText);
			this.treeView1.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_BeforeLabelEdit);
			this.treeView1.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_AfterLabelEdit);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
			this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.tree_DragDrop);
			this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.tree_DragOver);
			this.treeView1.DragLeave += new System.EventHandler(this.tree_DragLeave);
			this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tree_KeyDown);
			this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tree_MouseDown);
			this.treeView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tree_MouseMove);
			this.groupControl1.Controls.Add(this.treeView1);
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.Location = new System.Drawing.Point(0, 0);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Size = new System.Drawing.Size(160, 174);
			this.groupControl1.TabIndex = 1;
			this.groupControl1.Text = "Documents:";
			this.btnPopulate.Location = new System.Drawing.Point(268, 19);
			this.btnPopulate.Name = "btnPopulate";
			this.btnPopulate.Size = new System.Drawing.Size(128, 30);
			this.btnPopulate.TabIndex = 4;
			this.btnPopulate.Text = "Populate";
			this.btnPopulate.Click += new System.EventHandler(this.btnPopulate_Click);
			this.btnClear.Location = new System.Drawing.Point(402, 19);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(128, 30);
			this.btnClear.TabIndex = 4;
			this.btnClear.Text = "Clear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Document_16x16.png");
			this.Name = "DocumentsFrame";
			this.Size = new System.Drawing.Size(476, 288);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		public DevExpress.Utils.Design.DXTreeView treeView1;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private DevExpress.XtraEditors.SimpleButton btnDelete;
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		private DevExpress.XtraEditors.SimpleButton btnPopulate;
		private DevExpress.XtraEditors.SimpleButton btnClear;
		private System.Windows.Forms.ImageList imageList1;
	}
}
