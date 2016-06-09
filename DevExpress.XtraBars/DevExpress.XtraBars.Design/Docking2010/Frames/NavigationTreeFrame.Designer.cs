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
	partial class NavigationTreeFrame {
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NavigationTreeFrame));
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
			this.documentsTree = new DevExpress.XtraBars.Design.Frames.MultiSelectTreeView();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
			this.tilesTree = new DevExpress.XtraBars.Design.Frames.MultiSelectTreeView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.panelControl3 = new DevExpress.XtraEditors.GroupControl();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.navigateTree = new DevExpress.XtraBars.Design.Frames.MultiSelectTreeView();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.images = new DevExpress.Utils.ImageCollection(this.components);
			this.btnClear = new DevExpress.XtraEditors.SimpleButton();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).BeginInit();
			this.groupControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).BeginInit();
			this.groupControl3.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
			this.panelControl3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.images)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.splMain, "splMain");
			resources.ApplyResources(this.pgMain, "pgMain");
			this.pnlControl.Controls.Add(this.btnClear);
			this.pnlControl.Controls.Add(this.btnDown);
			this.pnlControl.Controls.Add(this.btnUp);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.tableLayoutPanel1);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			resources.ApplyResources(this.splitContainerControl1, "splitContainerControl1");
			this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
			this.splitContainerControl1.Horizontal = false;
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.groupControl2);
			this.splitContainerControl1.Panel2.Controls.Add(this.groupControl3);
			this.splitContainerControl1.SplitterPosition = 198;
			this.groupControl2.Controls.Add(this.documentsTree);
			resources.ApplyResources(this.groupControl2, "groupControl2");
			this.groupControl2.Name = "groupControl2";
			this.documentsTree.AllowDrag = true;
			this.documentsTree.AllowDrop = true;
			this.documentsTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.documentsTree.CausesValidation = false;
			this.documentsTree.DefaultExpandCollapseButtonOffset = 5;
			resources.ApplyResources(this.documentsTree, "documentsTree");
			this.documentsTree.HideSelection = false;
			this.documentsTree.ImageList = this.imageList1;
			this.documentsTree.Name = "documentsTree";
			this.documentsTree.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildren;
			this.documentsTree.DragNodeStart += new System.EventHandler(this.documentsTree_DragNodeStart);
			this.documentsTree.DragNodeGetObject += new DevExpress.Utils.Design.TreeViewGetDragObjectEventHandler(this.documentsTree_DragNodeGetObject);
			this.documentsTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
			this.documentsTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.documentsTree_DragDrop);
			this.documentsTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.documentsTree_DragEnter);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList1.Images.SetKeyName(0, "ActivationTarget_16x16.png");
			this.imageList1.Images.SetKeyName(1, "Children_16x16.png");
			this.imageList1.Images.SetKeyName(2, "Items_16x16.png");
			this.imageList1.Images.SetKeyName(3, "Add_16x16.png");
			this.imageList1.Images.SetKeyName(4, "Document_16x16.png");
			this.imageList1.Images.SetKeyName(5, "Tile_16x16.png");
			this.imageList1.Images.SetKeyName(6, "Page_16x16.png");
			this.imageList1.Images.SetKeyName(7, "PageGroup_16x16.png");
			this.imageList1.Images.SetKeyName(8, "SlideGroup_16x16.png");
			this.imageList1.Images.SetKeyName(9, "SplitGroup_16x16.png");
			this.imageList1.Images.SetKeyName(10, "TabbedGroup_16x16.png");
			this.imageList1.Images.SetKeyName(11, "TileContainer_16x16.png");
			this.imageList1.Images.SetKeyName(12, "Flyout_16x16.png");
			this.groupControl3.Controls.Add(this.tilesTree);
			resources.ApplyResources(this.groupControl3, "groupControl3");
			this.groupControl3.Name = "groupControl3";
			this.tilesTree.AllowDrag = true;
			this.tilesTree.AllowDrop = true;
			this.tilesTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tilesTree.CausesValidation = false;
			this.tilesTree.DefaultExpandCollapseButtonOffset = 5;
			resources.ApplyResources(this.tilesTree, "tilesTree");
			this.tilesTree.HideSelection = false;
			this.tilesTree.ImageList = this.imageList1;
			this.tilesTree.Name = "tilesTree";
			this.tilesTree.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildren;
			this.tilesTree.DragNodeStart += new System.EventHandler(this.tilesTree_DragNodeStart);
			this.tilesTree.DragNodeGetObject += new DevExpress.Utils.Design.TreeViewGetDragObjectEventHandler(this.tilesTree_DragNodeGetObject);
			this.tilesTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tree_AfterSelect);
			this.tilesTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.tilesTree_DragDrop);
			this.tilesTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.tilesTree_DragEnter);
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.panelControl3, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupControl1, 0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.panelControl3.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl3.Controls.Add(this.splitContainerControl1);
			resources.ApplyResources(this.panelControl3, "panelControl3");
			this.panelControl3.Name = "panelControl3";
			this.groupControl1.Controls.Add(this.navigateTree);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			this.navigateTree.AllowDrag = true;
			this.navigateTree.AllowDrop = true;
			this.navigateTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.navigateTree.CausesValidation = false;
			this.navigateTree.DefaultExpandCollapseButtonOffset = 10;
			resources.ApplyResources(this.navigateTree, "navigateTree");
			this.navigateTree.HideSelection = false;
			this.navigateTree.ImageList = this.imageList1;
			this.navigateTree.Name = "navigateTree";
			this.navigateTree.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildren;
			this.navigateTree.DragNodeStart += new System.EventHandler(this.navigateTree_DragNodeStart);
			this.navigateTree.DragNodeGetObject += new DevExpress.Utils.Design.TreeViewGetDragObjectEventHandler(this.navigateTree_DragNodeGetObject);
			this.navigateTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.navigateTree_AfterSelect);
			this.navigateTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.navigationTreeDragDrop);
			this.navigateTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.navigationTreeDragEnter);
			this.navigateTree.DragOver += new System.Windows.Forms.DragEventHandler(this.navigationTreeDragOver);
			this.navigateTree.DragLeave += new System.EventHandler(this.navigationTreeDragLeave);
			this.navigateTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.navigationTreeKeyDown);
			this.btnDown.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnDown.ImageIndex = 1;
			this.btnDown.ImageList = this.images;
			this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.Name = "btnDown";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			resources.ApplyResources(this.images, "images");
			this.images.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("images.ImageStream")));
			this.images.Images.SetKeyName(0, "UpGlyph.png");
			this.images.Images.SetKeyName(1, "DownGlyph.png");
			this.images.Images.SetKeyName(2, "Clear.png");
			this.btnClear.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnClear.ImageIndex = 2;
			this.btnClear.ImageList = this.images;
			this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnClear, "btnClear");
			this.btnClear.Name = "btnClear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			this.btnUp.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnUp.ImageIndex = 0;
			this.btnUp.ImageList = this.images;
			this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.Name = "btnUp";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			this.Name = "NavigationTreeFrame";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl2)).EndInit();
			this.groupControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl3)).EndInit();
			this.groupControl3.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
			this.panelControl3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.images)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private MultiSelectTreeView navigateTree;
		private DevExpress.XtraEditors.GroupControl panelControl3;
		private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
		private MultiSelectTreeView documentsTree;
		private MultiSelectTreeView tilesTree;
		private DevExpress.XtraEditors.SimpleButton btnDown;
		private DevExpress.Utils.ImageCollection images;
		private DevExpress.XtraEditors.SimpleButton btnClear;
		private DevExpress.XtraEditors.SimpleButton btnUp;
		private DevExpress.XtraEditors.GroupControl groupControl2;
		private DevExpress.XtraEditors.GroupControl groupControl3;
		private DevExpress.XtraEditors.GroupControl groupControl1;
	}
}
