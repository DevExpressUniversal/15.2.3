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
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
namespace DevExpress.XtraGrid.Design {
	[ToolboxItem(false)]
	public class LevelSelector : System.Windows.Forms.UserControl {
		ViewTree tree;
		private System.ComponentModel.IContainer components = null;
		public System.Windows.Forms.Panel panel;
		public DevExpress.XtraEditors.SimpleButton btDesigner;
		bool allowDesignerButton = true;
		GridControl editingGrid = null;
		private System.Windows.Forms.Panel pnlClient;
		private System.Windows.Forms.Label lblLine;
		public DevExpress.XtraEditors.SimpleButton btPopulateDetail;
		private PanelControl pnlButton;
		public LevelSelector() {
			InitializeComponent();
			SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			CreateIcons();
		}
		void CreateIcons() {
			ImageList iml = DevExpress.Utils.ResourceImageHelper.CreateImageListFromResources("DevExpress.XtraGrid.Design.Designer.ViewIcons.bmp", typeof(LevelSelector).Assembly, new Size(16, 16), Color.Magenta);
			tree.Images = iml;
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			InitScrPanel();
			tree.BorderStyle = BorderStyles.NoBorder;
			UpdateControls();
		}
		public virtual ViewTree Tree { get { return tree; } }
		public virtual bool AllowDesignerButton {
			get { return allowDesignerButton; }
			set {
				if(AllowDesignerButton == value) return;
				this.allowDesignerButton = value;
				UpdateControls();
			}
		}
		public virtual void UpdateLevels() {
			tree.PopulateTree();
		}
		private PanelControl scrPanel;
		private void InitScrPanel() {
			if(scrPanel == null) {
				scrPanel = new PanelControl();
				this.scrPanel.BorderStyle = BorderStyles.NoBorder;
				pnlClient.Controls.Add(scrPanel);
				scrPanel.Location = new Point(0, 0);
				scrPanel.Width = 1;
			}
		}
		public Cursor GetHitTest(Point screenPoint) {
			if(Tree.IsEditingLevelName) return Cursors.Arrow;
			Point point = PointToClient(screenPoint);
			ViewTreeHitInfo hitInfo = Tree.ViewInfo.CalcHitInfo(point);
			if(Tree.CanHotTrack(hitInfo)) return Cursors.Hand;
			return Cursors.Arrow;
		}
		protected virtual void UpdateControls() {
			btDesigner.Visible = AllowDesignerButton;
		}
		public GridControl EditingGrid {
			get { return editingGrid; }
			set { 
				if(EditingGrid == value) return;
				editingGrid = value; 
				tree.Grid = value;
				btPopulateDetail.Visible = tree.AllowModify;
			}
		}
		public GridControlDesigner Designer {
			get {
				if(EditingGrid == null) return null;
				IDesignerHost host = EditingGrid.InternalGetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host == null) return null;
				return host.GetDesigner(EditingGrid) as GridControlDesigner;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LevelSelector));
			this.tree = new DevExpress.XtraGrid.Design.ViewTree();
			this.panel = new System.Windows.Forms.Panel();
			this.pnlClient = new System.Windows.Forms.Panel();
			this.pnlButton = new DevExpress.XtraEditors.PanelControl();
			this.btPopulateDetail = new DevExpress.XtraEditors.SimpleButton();
			this.lblLine = new System.Windows.Forms.Label();
			this.btDesigner = new DevExpress.XtraEditors.SimpleButton();
			this.panel.SuspendLayout();
			this.pnlClient.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlButton)).BeginInit();
			this.pnlButton.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.tree, "tree");
			this.tree.Grid = null;
			this.tree.Images = null;
			this.tree.Name = "tree";
			this.tree.UseInternalEditor = true;
			this.tree.TreeLayoutChanged += new System.EventHandler(this.levels_TreeLayoutChanged);
			this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel.Controls.Add(this.pnlClient);
			this.panel.Controls.Add(this.pnlButton);
			resources.ApplyResources(this.panel, "panel");
			this.panel.Name = "panel";
			resources.ApplyResources(this.pnlClient, "pnlClient");
			this.pnlClient.Controls.Add(this.tree);
			this.pnlClient.Name = "pnlClient";
			this.pnlClient.Resize += new System.EventHandler(this.pnlClient_Resize);
			this.pnlButton.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlButton.Controls.Add(this.btPopulateDetail);
			this.pnlButton.Controls.Add(this.lblLine);
			this.pnlButton.Controls.Add(this.btDesigner);
			resources.ApplyResources(this.pnlButton, "pnlButton");
			this.pnlButton.Name = "pnlButton";
			resources.ApplyResources(this.btPopulateDetail, "btPopulateDetail");
			this.btPopulateDetail.Name = "btPopulateDetail";
			this.btPopulateDetail.Click += new System.EventHandler(this.btPopulateDetail_Click);
			this.lblLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			resources.ApplyResources(this.lblLine, "lblLine");
			this.lblLine.Name = "lblLine";
			resources.ApplyResources(this.btDesigner, "btDesigner");
			this.btDesigner.Name = "btDesigner";
			this.btDesigner.Click += new System.EventHandler(this.btDesigner_Click);
			this.Controls.Add(this.panel);
			this.Name = "LevelSelector";
			resources.ApplyResources(this, "$this");
			this.panel.ResumeLayout(false);
			this.pnlClient.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlButton)).EndInit();
			this.pnlButton.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private void btDesigner_Click(object sender, System.EventArgs e) {
			tree.CancelEdit();
		}
		public virtual void UpdateSize() {
			Size levelsSize = tree.CalcBestSize();
			levelsSize.Width = Math.Max(levelsSize.Width, 100);
			levelsSize.Height = Math.Max(levelsSize.Height, 100);
			if(AllowDesignerButton) {
				Size size = levelsSize;
				size.Height = Math.Min(levelsSize.Height + pnlButton.Height + 2, EditingGrid.Height - 50);
				size.Width +=2;
				this.ClientSize = size;
				if(scrPanel != null) 
					scrPanel.Height = levelsSize.Height;
			} else {
				if(scrPanel != null && scrPanel.Size != levelsSize)
					scrPanel.Size = levelsSize;
			}
		}
		private void levels_TreeLayoutChanged(object sender, System.EventArgs e) {
			UpdateSize();
		}
		private void pnlClient_Resize(object sender, System.EventArgs e) {
			UpdateSize();
		}
		private void btPopulateDetail_Click(object sender, System.EventArgs e) {
			tree.PopulateDetailTree();
		}
	}
}
