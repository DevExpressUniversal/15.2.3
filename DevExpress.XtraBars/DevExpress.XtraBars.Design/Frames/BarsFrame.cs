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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false)]
	[CLSCompliant(false)]
	public class BarsFrame : BaseTreeFrame {
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		private DevExpress.XtraEditors.SimpleButton btnDelete;
		private System.ComponentModel.IContainer components = null;
		protected override string GroupCaption { get { return "ToolBars:"; } }
		protected override string DescriptionText { get { return "You can add / delete toolbars and customize toolbars and items."; } }
		public BarsFrame() {
			InitializeComponent();
			pgMain.BringToFront();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.tree.LineColor = System.Drawing.Color.Black;
			this.tree.Size = new System.Drawing.Size(152, 177);
			this.splMain.Location = new System.Drawing.Point(160, 92);
			this.splMain.Size = new System.Drawing.Size(5, 204);
			this.pgMain.Location = new System.Drawing.Point(165, 92);
			this.pgMain.Size = new System.Drawing.Size(283, 204);
			this.pnlControl.Controls.Add(this.btnDelete);
			this.pnlControl.Controls.Add(this.btnAdd);
			this.pnlControl.Location = new System.Drawing.Point(0, 38);
			this.pnlControl.Size = new System.Drawing.Size(448, 54);
			this.lbCaption.Size = new System.Drawing.Size(448, 42);
			this.pnlMain.Location = new System.Drawing.Point(0, 92);
			this.pnlMain.Size = new System.Drawing.Size(160, 204);
			this.horzSplitter.Location = new System.Drawing.Point(165, 92);
			this.horzSplitter.Size = new System.Drawing.Size(283, 4);
			this.btnAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAdd.Location = new System.Drawing.Point(0, 4);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(30, 30);
			this.btnAdd.TabIndex = 0;
			this.btnAdd.ToolTip = "Add New Toolbar";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.btnAdd.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnDelete.Enabled = false;
			this.btnDelete.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDelete.Location = new System.Drawing.Point(36, 4);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(30, 30);
			this.btnDelete.TabIndex = 1;
			this.btnDelete.ToolTip = "Delete Toolbar";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			this.btnDelete.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.Name = "BarsFrame";
			this.Size = new System.Drawing.Size(448, 296);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		protected override void InitImages() {
			base.InitImages();
			btnAdd.Image = DesignerImages16.Images[DesignerImages16AddIndex];
			btnDelete.Image = DesignerImages16.Images[DesignerImages16RemoveIndex];
		}
		public override void Populate() {
			if(Manager == null) return;
			Tree.BeginUpdate();
			try {
				Tree.Nodes.Clear();
				foreach(Bar bar in Manager.Bars) {
					AddBar(Tree.Nodes, bar);
				}
			}
			finally {
				Tree.EndUpdate();
			}
			if(Tree.Nodes.Count > 0) Tree.SelectedNode = Tree.Nodes[0];
		}
		private void AddToolbar() {
			string caption = CreateBarEditForm(true, Manager.GetNewBarName(), "Toolbar");
			if(caption != null) {
				Bar bar = new Bar(Manager);
				bar.OptionsBar.AllowRename = true;
				bar.Text = bar.BarName = caption;
				bar.DockStyle = BarDockStyle.Top;
				bar.Visible = true;
				Tree.SelectedNode = AddBar(Tree.Nodes, bar);
			}
		}
		private void btnAdd_Click(object sender, System.EventArgs e) {
			AddToolbar();
		}
		private void SetDeleteEnabled(TreeNode node) {
			btnDelete.Enabled = node != null && node.Tag is Bar;
		}
		protected override void tree_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			base.tree_AfterSelect(sender, e);
			SetDeleteEnabled(e.Node);
		}
		private void DeleteToolbar() {
			if(Tree.SelectedNode == null) return;
			Bar bar = Tree.SelectedNode.Tag as Bar;
			if(bar != null) {
				if(MessageBox.Show(string.Format("Are you sure you want to delete '{0}' toolbar?", bar.BarName), "XtraBars", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes) {
					bar.Dispose();
					Tree.Nodes.Remove(Tree.SelectedNode);
					SetDeleteEnabled(Tree.SelectedNode);
				}
			}
		}
		private void btnDelete_Click(object sender, System.EventArgs e) {
			DeleteToolbar();
		}
		protected override void tree_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) DeleteToolbar();
			if(e.KeyCode == Keys.Insert) AddToolbar();
		}
	}
}
