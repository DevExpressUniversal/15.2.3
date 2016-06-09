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
	partial class TileFrame {
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileFrame));
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
			resources.ApplyResources(this.splMain, "splMain");
			resources.ApplyResources(this.pgMain, "pgMain");
			this.pnlControl.Controls.Add(this.btnClear);
			this.pnlControl.Controls.Add(this.btnPopulate);
			this.pnlControl.Controls.Add(this.btnDelete);
			this.pnlControl.Controls.Add(this.btnAdd);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.groupControl1);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.treeView1.AllowSkinning = true;
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeView1.DefaultExpandCollapseButtonOffset = 5;
			resources.ApplyResources(this.treeView1, "treeView1");
			this.treeView1.HideSelection = false;
			this.treeView1.LabelEdit = true;
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.Standard;
			this.treeView1.GetNodeEditText += new DevExpress.Utils.Design.TreeViewGetNodeEditTextEventHandler(this.tree_GetNodeEditText);
			this.treeView1.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tree_BeforeLabelEdit);
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
			this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.tree_DragDrop);
			this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.tree_DragOver);
			this.treeView1.DragLeave += new System.EventHandler(this.tree_DragLeave);
			this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tree_KeyDown);
			this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tree_MouseDown);
			this.treeView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tree_MouseMove);
			this.groupControl1.Controls.Add(this.treeView1);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			resources.ApplyResources(this.btnPopulate, "btnPopulate");
			this.btnPopulate.Name = "btnPopulate";
			this.btnPopulate.Click += new System.EventHandler(this.btnPopulate_Click);
			resources.ApplyResources(this.btnClear, "btnClear");
			this.btnClear.Name = "btnClear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "Tile_16x16.png");
			this.Name = "TileFrame";
			resources.ApplyResources(this, "$this");
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
		private DevExpress.Utils.Design.DXTreeView treeView1;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private DevExpress.XtraEditors.SimpleButton btnDelete;
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		private DevExpress.XtraEditors.SimpleButton btnPopulate;
		private DevExpress.XtraEditors.SimpleButton btnClear;
		private System.Windows.Forms.ImageList imageList1;
	}
}
