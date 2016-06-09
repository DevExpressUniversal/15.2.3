#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.XtraBars.Customization;
using DevExpress.XtraBars.Ribbon.Customization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.ExpressApp.Win.Design.Bars {
	public abstract class BaseActionsUiEditor : XtraPGFrame {
		private IContainer components = null;
		private SplitContainerControl splitContainerPanels;
		private PanelControl actionsUiTreePanel;
		private GroupControl actionsUiGroupControl;
		private GroupControl barControlsGroupControl;
		private BarItemsListBox barControlsList;
		private ActionContainersTreeView actionsUiTree;
		private SimpleButton btnDelete;
		private void ActionsUiTree_SelectionChanged(object sender, EventArgs e) {
			SetSelectionToActionsUiTree();
		}
		private void ActionsUiTree_GotFocus(object sender, EventArgs e) {
			SetSelectionToActionsUiTree();
		}
		private void ActionsUiTree_AfterSelect(object sender, TreeViewEventArgs e) {
			btnDelete.Enabled = GetDeleteEnabled(e.Node);
		}
		private void BarItemsList_SelectedIndexChanged(object sender, EventArgs e) {
			SetSelectionToControlsList();
		}
		private void BarItemsList_GotFocus(object sender, EventArgs e) {
			SetSelectionToControlsList();
		}
		private void OnManagerItemsChanged(object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Refresh && e.Element != null) {
				barControlsList.Invalidate();
				return;
			}
		}
		private void btnDelete_Click(object sender, EventArgs e) {
			DeleteActionUiElement(actionsUiTree.SelNodes);
			btnDelete.Enabled = GetDeleteEnabled(actionsUiTree.SelectedNode);
			UpdateSelection();
		}
		private void ActionsUiTree_DragDrop(object sender, DragEventArgs e) {
			OnControlDragDrop(e);
		}
		private void UpdateSelection() {
			if(actionsUiTree.Nodes.Count == 0) {
				SetSelectionToContainersList();
			}
			else {
				SetSelectionToActionsUiTree();
			}
		}
		private void SelectActionsUiTreeNode(int index) {
			if(actionsUiTree.Nodes.Count > index) {
				actionsUiTree.SelectedNode = actionsUiTree.Nodes[index];
			}
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.splitContainerPanels = new DevExpress.XtraEditors.SplitContainerControl();
			this.actionsUiTree = new ActionContainersTreeView();
			this.actionsUiTreePanel = new DevExpress.XtraEditors.PanelControl();
			this.actionsUiGroupControl = new DevExpress.XtraEditors.GroupControl();
			this.barControlsList = new DevExpress.XtraBars.Customization.BarItemsListBox();
			this.barControlsGroupControl = new DevExpress.XtraEditors.GroupControl();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerPanels)).BeginInit();
			this.splitContainerPanels.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.actionsUiTreePanel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.actionsUiGroupControl)).BeginInit();
			this.actionsUiTreePanel.SuspendLayout();
			this.actionsUiGroupControl.SuspendLayout();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(480, 74);
			this.splMain.Size = new System.Drawing.Size(5, 401);
			this.pgMain.Location = new System.Drawing.Point(495, 74);
			this.pgMain.Size = new System.Drawing.Size(245, 401);
			this.pnlControl.Controls.Add(this.btnDelete);
			this.pnlControl.Location = new System.Drawing.Point(0, 38);
			this.pnlControl.Size = new System.Drawing.Size(448, 54);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(740, 42);
			this.pnlMain.Controls.Add(this.splitContainerPanels);
			this.pnlMain.Location = new System.Drawing.Point(0, 92);
			this.pnlMain.Size = new System.Drawing.Size(480, 401);
			this.horzSplitter.Size = new System.Drawing.Size(740, 4);
			this.btnDelete.Enabled = false;
			this.btnDelete.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDelete.Location = new System.Drawing.Point(0, 4);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(30, 30);
			this.btnDelete.TabIndex = 1;
			this.btnDelete.ToolTip = "Delete Action Container";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			this.btnDelete.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnDelete.Image = DevExpress.XtraEditors.Designer.Utils.XtraFrame.DesignerImages16.Images[DesignerImages16RemoveIndex];
			this.splitContainerPanels.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerPanels.Location = new System.Drawing.Point(0, 40);
			this.splitContainerPanels.Name = "splitContainerControl";
			this.splitContainerPanels.Panel1.Controls.Add(this.actionsUiTreePanel);
			this.splitContainerPanels.Panel1.Text = "splitContainerControl1_Panel1";
			this.splitContainerPanels.Panel1.MinSize = 80;
			this.splitContainerPanels.Panel2.Controls.Add(this.barControlsGroupControl);
			this.splitContainerPanels.Panel2.Text = "splitContainerControl1_Panel2";
			this.splitContainerPanels.Panel2.MinSize = 80;
			this.splitContainerPanels.Size = new System.Drawing.Size(480, 401);
			this.splitContainerPanels.SplitterPosition = 235;
			this.splitContainerPanels.TabIndex = 0;
			this.splitContainerPanels.Text = "splitContainerControl";
			this.actionsUiTree.AllowDrop = true;
			this.actionsUiTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.actionsUiTree.DefaultExpandCollapseButtonOffset = 5;
			this.actionsUiTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.actionsUiTree.HideSelection = false;
			this.actionsUiTree.ImageIndex = 0;
			this.actionsUiTree.Location = new System.Drawing.Point(2, 2);
			this.actionsUiTree.Name = "actionsUiTree";
			this.actionsUiTree.SelectedImageIndex = 0;
			this.actionsUiTree.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildren;
			this.actionsUiTree.Size = new System.Drawing.Size(225, 356);
			this.actionsUiTree.TabIndex = 0;
			this.actionsUiTreePanel.Appearance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(236)))), ((int)(((byte)(239)))));
			this.actionsUiTreePanel.Appearance.Options.UseBackColor = true;
			this.actionsUiTreePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.actionsUiTreePanel.Controls.Add(this.actionsUiGroupControl);
			this.actionsUiTreePanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.actionsUiTreePanel.Location = new System.Drawing.Point(0, 41);
			this.actionsUiTreePanel.Name = "actionContainersTreePanel";
			this.actionsUiTreePanel.Size = new System.Drawing.Size(232, 360);
			this.actionsUiTreePanel.TabIndex = 0;
			this.actionsUiGroupControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.actionsUiGroupControl.DockPadding.All = 0;
			this.actionsUiGroupControl.Name = "actionContainersGroupControl";
			this.actionsUiGroupControl.Size = new System.Drawing.Size(160, 252);
			this.actionsUiGroupControl.TabIndex = 0;
			this.actionsUiGroupControl.Text = ActionsUiCaption;
			this.actionsUiGroupControl.Controls.Add(actionsUiTree);
			this.barControlsGroupControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.barControlsGroupControl.DockPadding.All = 0;
			this.barControlsGroupControl.Name = "barContainersGroupControl";
			this.barControlsGroupControl.Size = new System.Drawing.Size(250, 290);
			this.barControlsGroupControl.TabIndex = 2;
			this.barControlsGroupControl.Text = BarControlsCaption;
			this.barControlsGroupControl.Controls.AddRange(new System.Windows.Forms.Control[] { barControlsList });
			this.barControlsList.AllowDrop = true;
			this.barControlsList.Appearance.Options.UseBackColor = true;
			this.barControlsList.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.barControlsList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.barControlsList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.barControlsList.ItemHeight = 16;
			this.barControlsList.Location = new System.Drawing.Point(0, 0);
			this.barControlsList.Name = "barContainersList";
			this.barControlsList.Size = new System.Drawing.Size(256, 360);
			this.barControlsList.TabIndex = 2;
			this.Name = "BaseActionsUiEditor";
			this.Size = new System.Drawing.Size(740, 475);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerPanels)).EndInit();
			this.splitContainerPanels.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.actionsUiTreePanel)).EndInit();
			this.actionsUiTreePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.actionsUiGroupControl)).EndInit();
			this.actionsUiGroupControl.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected abstract void OnControlDragDrop(DragEventArgs e);
		protected abstract bool GetDeleteEnabled(TreeNode node);
		protected abstract void DeleteActionUiElement(TreeNode[] selectedNodes);
		protected abstract void FillControlsList();
		protected abstract void FillActionUiElements();
		protected virtual void InitializeDesigner() {
			pgMain.PropertyValueChanged += OnObjectPropertiesChanged;
			actionsUiTree.SelectionChanged += ActionsUiTree_SelectionChanged;
			actionsUiTree.DragDrop += ActionsUiTree_DragDrop;
			actionsUiTree.AfterSelect += ActionsUiTree_AfterSelect;
			actionsUiTree.GotFocus += ActionsUiTree_GotFocus;
			barControlsList.Manager.Items.CollectionChanged += OnManagerItemsChanged;
			barControlsList.SelectedIndexChanged += BarItemsList_SelectedIndexChanged;
			barControlsList.GotFocus += BarItemsList_GotFocus;
			FillControlsList();
			FillActionUiElements();
			SelectActionsUiTreeNode(0);
		}
		protected virtual void SetSelectionToContainersList() {
			SetSelectionToControlsList();
		}
		protected virtual void SetSelectionToControlsList() {
			if(pgMain.SelectedObject != barControlsList.SelectedItem) {
				pgMain.SelectedObject = barControlsList.SelectedItem;
				barControlsList.Select();
			}
		}
		protected virtual void OnObjectPropertiesChanged(object sender, PropertyValueChangedEventArgs e) {
			if(pgMain.SelectedObject is IActionControlContainer && e.ChangedItem.Label == "ActionCategory" ||
				pgMain.SelectedObject is IActionControl && e.ChangedItem.Label == "ActionId") {
				actionsUiTree.UpdateTreeNodesText(pgMain.SelectedObject);
			}
		}
		protected void SetSelectionToActionsUiTree() {
			if(actionsUiTree.SelNode != null && pgMain.SelectedObject != actionsUiTree.SelNode.Tag) {
				pgMain.SelectedObject = actionsUiTree.SelNode.Tag;
				actionsUiTree.Select();
			}
		}
		protected void CreateTreeNode(string caption, object actionUiItem, int imageIndex) {
			TreeNode node = new TreeNode(caption, imageIndex, imageIndex);
			node.Tag = actionUiItem;
			actionsUiTree.Nodes.Add(node);
			actionsUiTree.SelNode = node;
		}
		protected string GetFormattedNodeText(string id, string controlName) {
			return actionsUiTree.GetFormattedNodeText(id, controlName);
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			if(barControlsList.Manager != null) barControlsList.Manager.Items.CollectionChanged -= OnManagerItemsChanged;
			barControlsList.SelectedIndexChanged -= BarItemsList_SelectedIndexChanged;
			barControlsList.GotFocus -= BarItemsList_GotFocus;
			actionsUiTree.SelectionChanged -= ActionsUiTree_SelectionChanged;
			actionsUiTree.DragDrop -= ActionsUiTree_DragDrop;
			actionsUiTree.AfterSelect -= ActionsUiTree_AfterSelect;
			actionsUiTree.GotFocus -= ActionsUiTree_GotFocus;
			pgMain.PropertyValueChanged -= OnObjectPropertiesChanged;
			base.Dispose(disposing);
		}
		protected virtual string BarControlsCaption {
			get { return "Bar Container Controls:"; }
		}
		protected virtual string ActionsUiCaption {
			get { return "Action Containers:"; }
		}
		protected override bool AllowGlobalStore { 
			get { return false; } 
		}
		protected ItemsTreeView ActionsUiTree {
			get { return actionsUiTree; }
		}
		protected BarItemsListBox BarControlsList {
			get { return barControlsList; }
		}
		public BaseActionsUiEditor() {
			InitializeComponent();
		}
		public override void InitComponent() {
			base.InitComponent();
			InitializeDesigner();
		}
	}
}
