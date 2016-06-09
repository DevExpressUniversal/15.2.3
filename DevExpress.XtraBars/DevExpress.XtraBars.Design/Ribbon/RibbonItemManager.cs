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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils.Design;
using DevExpress.XtraBars.Ribbon.Customization;
using DevExpress.XtraBars.Ribbon.Design;
namespace DevExpress.XtraBars.Design.Frames {
	[ToolboxItem(false), CLSCompliant(false)]
	public class RibbonItemsManagerBase : ItemLinksBaseManager {
		protected new RibbonTreeView ItemsTree { get { return base.ItemsTree as RibbonTreeView; } }
		protected new RibbonLinksToolbar LinksToolbar  { get  { return base.LinksToolbar as RibbonLinksToolbar; } }
		protected new RibbonItemsToolbar ItemsToolbar { get { return base.ItemsToolbar as RibbonItemsToolbar; } }
		protected override ItemLinksTreeView CreateItemsTree() { 
			RibbonTreeView treeView = new RibbonTreeView();
			treeView.AllowSkinning = true;
			treeView.OwnerFrame = this;
			return treeView;
		}
		protected override ItemLinksBaseToolbar CreateLinksToolbar() { return new RibbonLinksToolbar(); }
		protected override ItemLinksBaseToolbar CreateItemsToolbar() { return new RibbonItemsToolbar(); }
		protected override void InitializeToolbarEvents() { 
			base.InitializeToolbarEvents();
			LinksToolbar.AddPageButton.Click += AddPage_Click;
			LinksToolbar.AddGroupButton.Click += AddGroup_Click;
			LinksToolbar.AddPageCategoryButton.Click += AddPageCategory_Click;
		}
		protected override void FillEnabledButton() {
			base.FillEnabledButton();
			if(LinksToolbar.AddGroupButton.Visible == true)
				EnabledButtons.Add(LinksToolbar.AddGroupButton);
			if(LinksToolbar.AddPageCategoryButton.Visible == true)
				EnabledButtons.Add(LinksToolbar.AddPageCategoryButton);
			if(LinksToolbar.AddPageButton.Visible == true)
				EnabledButtons.Add(LinksToolbar.AddPageButton);
		}
		protected override void ClearToolbarEvents() {
			LinksToolbar.AddPageButton.Click -= AddPage_Click;
			LinksToolbar.AddGroupButton.Click -= AddGroup_Click;
			LinksToolbar.AddPageCategoryButton.Click -= AddPageCategory_Click;
		}
		void AddPageCategory_Click(object sender, EventArgs e) {
			TreeNode node = ItemsTree.AddRibbonPageCategory();
			if(node == null) return;
			ItemsTree.SelectedNode = node;
			ItemsTree.SelectedNode.Expand();
			UpdateBarButtons();
		}
		void AddPage_Click(object sender, EventArgs e) {
			ItemsTree.SelectedNode = ItemsTree.AddRibbonPage();
			ItemsTree.SelectedNode.Expand();
			UpdateBarButtons();
		}
		void AddGroup_Click(object sender, EventArgs e) {
			ItemsTree.AddRibbonPageGroup();
			UpdateBarButtons();
		}
		void UpdateAddPageButton() {
			bool enabled = true;
			if(ItemsTree.Category(ItemsTree.SelectedNode) == null && ItemsTree.Page(ItemsTree.SelectedNode) == null) enabled = false;
			LinksToolbar.AddPageButton.Enabled = enabled;
		}
		void UpdateAddGroupButton() {
			bool enabled = true;
			if(ItemsTree.Page(ItemsTree.SelectedNode) == null && ItemsTree.Group(ItemsTree.SelectedNode) == null) enabled = false;
			LinksToolbar.AddGroupButton.Enabled = enabled;
		}
		bool ShouldEnableBeginGroup() {
			if(ItemsTree.SelNodes.Length == 0)return false;
			if(ItemsTree.SelNodes[0].Tag as BarItemLink == null) return false;
			if(ItemsTree.SelNodes[0].Parent.Tag as BarItemLink != null) return false;
			return true;
		}
		protected override void UpdateBeginGroup() { 
			if(!ShouldEnableBeginGroup()) { 
				BeginGroup.Enabled = false;
				return;
			}
			BeginGroup.Enabled = true;
			BeginGroup.Checked = (ItemsTree.SelNodes[0].Tag as BarItemLink).BeginGroup;
		}
		protected override void UpdateMoveUpDownButtons() {
			base.UpdateMoveUpDownButtons();
			if(ItemsTree.SelectedNode == null || ItemsTree.SelectedNode.Parent != null)
				return;
			if(ItemsTree.SelectedNode.PrevNode == null) {
				LinksToolbar.MoveUpButton.Enabled = false;
				LinksToolbar.MoveDownButton.Enabled = false;
			}
			else if(ItemsTree.SelectedNode.PrevNode.PrevNode == null) {
				LinksToolbar.MoveUpButton.Enabled = false;
			}
		}
		protected override void UpdateBarButtons() {
			if(!AllowUpdateToolbarButtons) return;
			base.UpdateBarButtons();	
			UpdateAddGroupButton();
			UpdateAddPageButton();
		}
		private void InitializeComponent() {
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(160, 75);
			this.splMain.Size = new System.Drawing.Size(5, 237);
			this.pnlControl.Location = new System.Drawing.Point(0, 43);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(456, 39);
			this.pnlMain.Location = new System.Drawing.Point(0, 75);
			this.pnlMain.Size = new System.Drawing.Size(160, 237);
			this.horzSplitter.Location = new System.Drawing.Point(0, 39);
			this.Name = "RibbonItemsManagerBase";
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
	}
	public class RibbonTree : DXTreeView {
		public RibbonTree() {
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}
		protected override void OnPaintBackground(PaintEventArgs pevent) {
		}
	}
}
