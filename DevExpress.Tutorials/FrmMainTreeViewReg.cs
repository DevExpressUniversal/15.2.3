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
namespace DevExpress.Tutorials {
	public class FrmMainTreeViewReg : DevExpress.Tutorials.FrmMain {
		protected System.Windows.Forms.TreeView tvNavigation;
		protected System.Windows.Forms.ImageList imageListTreeView;
		private System.ComponentModel.IContainer components = null;
		public TreeViewAdapter fTreeViewAdapter;
		public FrmMainTreeViewReg() {
			InitializeComponent();
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMainTreeViewReg));
			this.tvNavigation = new System.Windows.Forms.TreeView();
			this.imageListTreeView = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			this.pnlCaption.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).BeginInit();
			this.gcNavigations.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcDescription)).BeginInit();
			this.gcDescription.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcMain)).BeginInit();
			this.pcMain.SuspendLayout();
			this.SuspendLayout();
			this.btnWhatsThis.Location = new System.Drawing.Point(588, 8);
			this.btnWhatsThis.Size = new System.Drawing.Size(96, 34);
			this.btnAbout.Location = new System.Drawing.Point(284, 9);
			this.pnlCaption.Location = new System.Drawing.Point(166, 24);
			this.gcNavigations.Controls.Add(this.tvNavigation);
			this.gcNavigations.Location = new System.Drawing.Point(0, 24);
			this.gcNavigations.Size = new System.Drawing.Size(166, 442);
			this.gcContainer.Location = new System.Drawing.Point(8, 8);
			this.gcContainer.Size = new System.Drawing.Size(510, 330);
			this.horzSplitter.Location = new System.Drawing.Point(8, 338);
			this.gcDescription.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.gcDescription.Appearance.Options.UseBackColor = true;
			this.gcDescription.Location = new System.Drawing.Point(8, 346);
			this.pcMain.Location = new System.Drawing.Point(166, 75);
			this.pcMain.Padding = new System.Windows.Forms.Padding(8);
			this.pcMain.Size = new System.Drawing.Size(526, 391);
			this.tvNavigation.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tvNavigation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvNavigation.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.tvNavigation.ImageIndex = 0;
			this.tvNavigation.ImageList = this.imageListTreeView;
			this.tvNavigation.Location = new System.Drawing.Point(19, 2);
			this.tvNavigation.Name = "tvNavigation";
			this.tvNavigation.SelectedImageIndex = 0;
			this.tvNavigation.ShowPlusMinus = false;
			this.tvNavigation.ShowRootLines = false;
			this.tvNavigation.Size = new System.Drawing.Size(145, 438);
			this.tvNavigation.TabIndex = 0;
			this.tvNavigation.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvNavigation_BeforeCollapse);
			this.tvNavigation.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvNavigation_AfterSelect);
			this.imageListTreeView.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageListTreeView.ImageSize = new System.Drawing.Size(16, 16);
			this.imageListTreeView.TransparentColor = System.Drawing.Color.Magenta;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(692, 466);
			this.Name = "FrmMainTreeViewReg";
			this.TutorialInfo.AboutFile = null;
			this.TutorialInfo.ImagePatternFill = null;
			this.TutorialInfo.ImageWhatsThisButton = ((System.Drawing.Image)(resources.GetObject("FrmMainTreeViewReg.TutorialInfo.ImageWhatsThisButton")));
			this.TutorialInfo.ImageWhatsThisButtonStop = ((System.Drawing.Image)(resources.GetObject("FrmMainTreeViewReg.TutorialInfo.ImageWhatsThisButtonStop")));
			this.TutorialInfo.SourceFileComment = null;
			this.TutorialInfo.SourceFileType = DevExpress.Tutorials.SourceFileType.CS;
			this.Load += new System.EventHandler(this.FrmMainTreeViewReg_Load);
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			this.pnlCaption.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).EndInit();
			this.gcNavigations.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcDescription)).EndInit();
			this.gcDescription.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pcMain)).EndInit();
			this.pcMain.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private void FrmMainTreeViewReg_Load(object sender, System.EventArgs e) {
			tvNavigation.ExpandAll();
			if(tvNavigation.SelectedNode == null && tvNavigation.Nodes.Count > 0)
				tvNavigation.SelectedNode = tvNavigation.Nodes[0];
		}
		private void tvNavigation_BeforeCollapse(object sender, System.Windows.Forms.TreeViewCancelEventArgs e) {
			e.Cancel = true;
		}
		private void tvNavigation_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			Cursor current = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			this.SuspendLayout();
			try {
				int id = fTreeViewAdapter.GetSelectedModuleId();
				ModuleInfo selectedModuleInfo = ModuleInfoCollection.ModuleInfoById(id);
				if(selectedModuleInfo == null) return;
				if(CurrentModule != null && CurrentModule.Name == selectedModuleInfo.Module.Name) return;
				SelectModule(selectedModuleInfo.Module, true);
			} finally {
				this.ResumeLayout();
				Cursor.Current = current;
			}
		}
		public void AddModuleInfo(int id, int parentId, string nodeText, object nodeImage, Type moduleType, string description, string whatsThisXMLFile, string whatsThisCodeFile) {
			TreeViewAdapter.AddNode(id, parentId, nodeText, nodeImage);
			if(moduleType != null)
				ModuleInfoCollection.Add(id, moduleType, description, whatsThisXMLFile, whatsThisCodeFile);
		}
		public void AddModuleInfo(int id, int parentId, string nodeText, object nodeImage, Type moduleType) {
			AddModuleInfo(id, parentId, nodeText, nodeImage, moduleType, string.Empty, string.Empty, string.Empty);
		}
		public TreeViewAdapter TreeViewAdapter {
			get {
				if(fTreeViewAdapter != null)
					return fTreeViewAdapter;
				fTreeViewAdapter = new TreeViewAdapter(tvNavigation);
				return fTreeViewAdapter;
			}
		}
	}
	public class TreeViewAdapter {
		TreeView navigationTreeView;
		public TreeViewAdapter(TreeView navigationTreeView) {
			this.navigationTreeView = navigationTreeView;
		}
		TreeNode AddNode(int id, int parentId, string nodeText) {
			TreeNode parentNode = LocateTreeNodeById(navigationTreeView.Nodes, parentId);
			TreeNode node = new TreeNode(nodeText);
			node.Tag = id;
			if(parentNode == null) 
				navigationTreeView.Nodes.Add(node);
			else
				parentNode.Nodes.Add(node);
			return node;
		}
		public void AddNode(int id, int parentId, string nodeText, object nodeImage) {
			SetNodeImage(AddNode(id, parentId, nodeText), nodeImage);
		}
		public int GetSelectedModuleId() {
			if(navigationTreeView.SelectedNode == null)
				return -1;
			return Convert.ToInt32(navigationTreeView.SelectedNode.Tag);
		}
		Image GetImageByString(string nodeImage) {
			if(nodeImage != string.Empty) {
				string realNodeImagePath = FilePathUtils.FindFilePath(nodeImage, true);
				if(realNodeImagePath != string.Empty) {
					return Image.FromFile(realNodeImagePath);
				}
			}
			return null;
		}
		void SetNodeImage(TreeNode node, object nodeImage) {
			Image image = null;
			if(nodeImage is string) image = GetImageByString(nodeImage.ToString());
			if(nodeImage is Image) image = nodeImage as Image;
			if(image != null) {
				int index = navigationTreeView.ImageList.Images.Add(image, Color.Transparent);
				node.ImageIndex = index;
				node.SelectedImageIndex = index;
			} else {
				node.ImageIndex = 0;
				node.SelectedImageIndex = 0;
			}
		}
		private TreeNode LocateTreeNodeById(TreeNodeCollection nodes, int nodeId) {
			foreach(TreeNode node in nodes) {
				if(Convert.ToInt32(node.Tag) == nodeId) return node;
				TreeNode result = LocateTreeNodeById(node.Nodes, nodeId);
				if(result != null) 
					return result;
			}
			return null;
		}
	}
}
