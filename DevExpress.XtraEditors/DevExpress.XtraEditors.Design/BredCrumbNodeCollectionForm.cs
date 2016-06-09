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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraEditors.Design {
	public partial class BredCrumbNodeCollectionEditor : CollectionEditor {
		protected class BredCrumbNodeCollectionForm : CollectionEditor.CollectionForm {
			Button btnAddChild;
			Button btnAddRoot;
			Button btnCancel;
			Button btnDelete;
			TreeNode curNode;
			BredCrumbNodeCollectionEditor editor;
			Label label1;
			Label label2;
			Button moveDownButton;
			Button moveUpButton;
			TableLayoutPanel navigationButtonsTableLayoutPanel;
			static object NextNodeKey = new object();
			TableLayoutPanel nodeControlPanel;
			Button okButton;
			LabelControl hintLabel;
			TableLayoutPanel okCancelPanel;
			TableLayoutPanel overarchingTableLayoutPanel;
			Panel hintPanel;
			VsPropertyGrid propertyGrid;
			TreeView treeView1;
			RepositoryItemBreadCrumbEdit properties;
			public BredCrumbNodeCollectionForm(CollectionEditor editor)
				: base(editor) {
				this.editor = (BredCrumbNodeCollectionEditor)editor;
				this.properties = BredCrumbNodeCollectionEditor.GetProperties(Context.Instance);
				InitializeComponent();
				HookEvents();
				UpdateUI();
			}
			protected override void OnShown(EventArgs e) {
				base.OnShown(e);
				InitUI();
				UpdateHintPanel();
			}
			void InitUI() {
				BreadCrumbNodeCollection col = this.properties.Nodes;
				TreeNode[] nodes = new TreeNode[col.Count];
				this.propertyGrid.Site = new PropertyGridSite(base.Context, this.propertyGrid);
				for(int i = 0; i < col.Count; i++) {
					BreadCrumbNode node = (BreadCrumbNode)col[i];
					nodes[i] = DesignUtils.CreateTreeNode(node);
				}
				this.treeView1.Nodes.Clear();
				this.treeView1.Nodes.AddRange(nodes);
				this.curNode = null;
				this.btnAddChild.Enabled = false;
				this.btnDelete.Enabled = false;
				if((nodes.Length > 0) && (nodes[0] != null)) {
					this.treeView1.SelectedNode = nodes[0];
				}
			}
			#region InitializeComponent
			void InitializeComponent() {
				ComponentResourceManager manager = new ComponentResourceManager(typeof(BredCrumbNodeCollectionEditor));
				this.hintPanel = new Panel();
				this.okCancelPanel = new TableLayoutPanel();
				this.okButton = new Button();
				this.hintLabel = new LabelControl();
				this.btnCancel = new Button();
				this.nodeControlPanel = new TableLayoutPanel();
				this.btnAddRoot = new Button();
				this.btnAddChild = new Button();
				this.btnDelete = new Button();
				this.moveDownButton = new Button();
				this.moveUpButton = new Button();
				this.propertyGrid = new VsPropertyGrid(base.Context);
				this.label2 = new Label();
				this.treeView1 = new TreeView();
				this.label1 = new Label();
				this.overarchingTableLayoutPanel = new TableLayoutPanel();
				this.navigationButtonsTableLayoutPanel = new TableLayoutPanel();
				this.hintPanel.SuspendLayout();
				this.okCancelPanel.SuspendLayout();
				this.nodeControlPanel.SuspendLayout();
				this.overarchingTableLayoutPanel.SuspendLayout();
				this.navigationButtonsTableLayoutPanel.SuspendLayout();
				base.SuspendLayout();
				this.hintPanel.Dock = DockStyle.Fill;
				this.hintPanel.Size = new System.Drawing.Size(40, 24);
				this.hintPanel.TabIndex = 0;
				this.hintPanel.Name = "hintPanel";
				this.hintPanel.Margin = new Padding(0);
				this.hintPanel.Controls.Add(this.hintLabel);
				this.okCancelPanel.Anchor = AnchorStyles.Right;
				this.okCancelPanel.AutoSize = true;
				this.okCancelPanel.ColumnCount = 2;
				this.okCancelPanel.Location = new Point(485, 310);
				this.okCancelPanel.RowCount = 1;
				this.okCancelPanel.Size = new System.Drawing.Size(156, 26);
				this.okCancelPanel.TabIndex = 6;
				this.okCancelPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
				this.okCancelPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
				this.okCancelPanel.Controls.Add(this.okButton, 0, 0);
				this.okCancelPanel.Controls.Add(this.btnCancel, 1, 0);
				this.okCancelPanel.Margin = new Padding(3, 3, 0, 0);
				this.okCancelPanel.Name = "okCancelPanel";
				this.okCancelPanel.RowStyles.Add(new RowStyle());
				this.hintLabel.ForeColor = Color.Black;
				this.hintLabel.AllowHtmlString = true;
				this.hintLabel.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
				this.hintLabel.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
				this.hintLabel.Appearance.TextOptions.VAlignment = VertAlignment.Top;
				this.hintLabel.AutoSizeMode = LabelAutoSizeMode.None;
				this.hintLabel.Dock = DockStyle.Fill;
				this.hintLabel.Location = new Point(3, 30);
				this.hintLabel.Size = new System.Drawing.Size(35, 26);
				this.hintLabel.TabIndex = 0;
				this.hintLabel.Text = "The BreadCrumbEdit control misses the root node, required for correct navigation. Click <href=add-node>here</href> to add a root node for your tree.";
				this.hintLabel.Name = "hintLabel";
				this.okButton.Anchor = AnchorStyles.Left | AnchorStyles.Right;
				this.okButton.AutoSize = true;
				this.okButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
				this.okButton.Location = new Point(3, 3);
				this.okButton.Size = new System.Drawing.Size(75, 23);
				this.okButton.TabIndex = 0;
				this.okButton.Text = "OK";
				this.okButton.DialogResult = DialogResult.OK;
				this.okButton.Margin = new Padding(0, 0, 3, 0);
				this.okButton.Name = "okButton";
				this.btnCancel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
				this.btnCancel.AutoSize = true;
				this.btnCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
				this.btnCancel.Location = new Point(97, 3);
				this.btnCancel.Size = new System.Drawing.Size(75, 23);
				this.btnCancel.TabIndex = 1;
				this.btnCancel.Text = "Cancel";
				this.btnCancel.DialogResult = DialogResult.Cancel;
				this.btnCancel.Margin = new Padding(3, 0, 0, 0);
				this.btnCancel.Name = "btnCancel";
				this.nodeControlPanel.Anchor = AnchorStyles.Left | AnchorStyles.Right;
				this.nodeControlPanel.AutoSize = true;
				this.nodeControlPanel.ColumnCount = 2;
				this.nodeControlPanel.Location = new Point(3, 275);
				this.nodeControlPanel.RowCount = 1;
				this.nodeControlPanel.Size = new System.Drawing.Size(274, 29);
				this.nodeControlPanel.TabIndex = 2;
				this.nodeControlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
				this.nodeControlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
				this.nodeControlPanel.Controls.Add(this.btnAddRoot, 0, 0);
				this.nodeControlPanel.Controls.Add(this.btnAddChild, 1, 0);
				this.nodeControlPanel.Margin = new Padding(0, 3, 3, 3);
				this.nodeControlPanel.Name = "nodeControlPanel";
				this.nodeControlPanel.RowStyles.Add(new RowStyle());
				this.btnAddRoot.Anchor = AnchorStyles.Left | AnchorStyles.Right;
				this.btnAddRoot.AutoSize = true;
				this.btnAddRoot.ImageAlign = ContentAlignment.MiddleLeft;
				this.btnAddRoot.ImeMode = System.Windows.Forms.ImeMode.NoControl;
				this.btnAddRoot.Location = new Point(3, 3);
				this.btnAddRoot.Size = new System.Drawing.Size(131, 23);
				this.btnAddRoot.TabIndex = 0;
				this.btnAddRoot.Text = "Add &Root";
				this.btnAddRoot.TextImageRelation = TextImageRelation.ImageBeforeText;
				this.btnAddRoot.Margin = new Padding(0, 0, 3, 0);
				this.btnAddRoot.Name = "btnAddRoot";
				this.btnAddChild.Anchor = AnchorStyles.Left | AnchorStyles.Right;
				this.btnAddChild.AutoSize = true;
				this.btnAddChild.ImageAlign = ContentAlignment.MiddleLeft;
				this.btnAddChild.ImeMode = System.Windows.Forms.ImeMode.NoControl;
				this.btnAddChild.Location = new Point(140, 3);
				this.btnAddChild.Size = new System.Drawing.Size(131, 23);
				this.btnAddChild.TabIndex = 1;
				this.btnAddChild.Text = "Add &Child";
				this.btnAddChild.TextImageRelation = TextImageRelation.ImageBeforeText;
				this.btnAddChild.Margin = new Padding(3, 0, 0, 0);
				this.btnAddChild.Name = "btnAddChild";
				this.btnDelete.Anchor = AnchorStyles.None;
				btnDelete.Image = Properties.Resources.Delete_16x16;
				this.btnDelete.ImeMode = System.Windows.Forms.ImeMode.NoControl;
				this.btnDelete.Location = new Point(3, 61);
				this.btnDelete.Margin = new System.Windows.Forms.Padding(3, 3, 23, 3);
				this.btnDelete.Size = new System.Drawing.Size(26, 23);
				this.btnDelete.TabIndex = 2;
				this.btnDelete.TextImageRelation = TextImageRelation.ImageBeforeText;
				this.btnDelete.Margin = new Padding(0, 3, 0, 0);
				this.btnDelete.Name = "btnDelete";
				this.moveDownButton.Anchor = AnchorStyles.None;
				moveDownButton.Image = Properties.Resources.Next_16x16;
				this.moveDownButton.Location = new Point(3, 32);
				this.moveDownButton.Margin = new System.Windows.Forms.Padding(3, 3, 23, 3);
				this.moveDownButton.Size = new System.Drawing.Size(26, 23);
				this.moveDownButton.TabIndex = 1;
				this.moveDownButton.Margin = new Padding(0, 1, 0, 3);
				this.moveDownButton.Name = "moveDownButton";
				this.moveUpButton.Anchor = AnchorStyles.None;
				moveUpButton.Image = Properties.Resources.Previous_16x16;
				this.moveUpButton.Location = new Point(3, 3);
				this.moveUpButton.Margin = new System.Windows.Forms.Padding(3, 3, 23, 3);
				this.moveUpButton.Size = new System.Drawing.Size(26, 23);
				this.moveUpButton.TabIndex = 0;
				this.moveUpButton.Margin = new Padding(0, 0, 0, 1);
				this.moveUpButton.Name = "moveUpButton";
				this.propertyGrid.Dock = DockStyle.Fill;
				this.propertyGrid.Location = new Point(341, 16);
				this.propertyGrid.Size = new System.Drawing.Size(274, 288);
				this.propertyGrid.TabIndex = 5;
				this.propertyGrid.LineColor = SystemColors.ScrollBar;
				this.propertyGrid.Margin = new Padding(3, 3, 0, 3);
				this.propertyGrid.Name = "propertyGrid1";
				this.overarchingTableLayoutPanel.SetRowSpan(this.propertyGrid, 2);
				this.label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
				this.label2.AutoSize = true;
				this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
				this.label2.Location = new Point(341, 0);
				this.label2.Size = new System.Drawing.Size(57, 13);
				this.label2.TabIndex = 4;
				this.label2.Text = "&Properties:";
				this.label2.Margin = new Padding(3, 1, 0, 0);
				this.label2.Name = "label2";
				this.treeView1.AllowDrop = true;
				this.treeView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				this.treeView1.Indent = 19;
				this.treeView1.Location = new Point(3, 16);
				this.treeView1.Size = new System.Drawing.Size(274, 253);
				this.treeView1.TabIndex = 1;
				this.treeView1.HideSelection = false;
				this.treeView1.Margin = new Padding(0, 3, 3, 3);
				this.treeView1.Name = "treeView1";
				this.label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
				this.label1.AutoSize = true;
				this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
				this.label1.Location = new Point(3, 0);
				this.label1.Size = new System.Drawing.Size(108, 13);
				this.label1.TabIndex = 0;
				this.label1.Text = "Select a &node to edit:";
				this.label1.Margin = new Padding(0, 1, 3, 0);
				this.label1.Name = "label1";
				this.overarchingTableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
				this.overarchingTableLayoutPanel.ColumnCount = 3;
				this.overarchingTableLayoutPanel.Location = new Point(12, 12);
				this.overarchingTableLayoutPanel.RowCount = 4;
				this.overarchingTableLayoutPanel.Size = new System.Drawing.Size(618, 342);
				this.overarchingTableLayoutPanel.TabIndex = 0;
				this.overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
				this.overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
				this.overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
				this.overarchingTableLayoutPanel.Controls.Add(this.navigationButtonsTableLayoutPanel, 1, 1);
				this.overarchingTableLayoutPanel.Controls.Add(this.label2, 2, 0);
				this.overarchingTableLayoutPanel.Controls.Add(this.propertyGrid, 2, 1);
				this.overarchingTableLayoutPanel.Controls.Add(this.treeView1, 0, 1);
				this.overarchingTableLayoutPanel.Controls.Add(this.label1, 0, 0);
				this.overarchingTableLayoutPanel.Controls.Add(this.nodeControlPanel, 0, 2);
				this.overarchingTableLayoutPanel.Controls.Add(this.okCancelPanel, 2, 3);
				this.overarchingTableLayoutPanel.Controls.Add(this.hintPanel, 0, 3);
				this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
				this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
				this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
				this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
				this.overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
				this.navigationButtonsTableLayoutPanel.AutoSize = true;
				this.navigationButtonsTableLayoutPanel.ColumnCount = 1;
				this.navigationButtonsTableLayoutPanel.Location = new Point(283, 16);
				this.navigationButtonsTableLayoutPanel.RowCount = 3;
				this.navigationButtonsTableLayoutPanel.Size = new System.Drawing.Size(52, 87);
				this.navigationButtonsTableLayoutPanel.TabIndex = 3;
				this.navigationButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
				this.navigationButtonsTableLayoutPanel.Controls.Add(this.moveUpButton, 0, 0);
				this.navigationButtonsTableLayoutPanel.Controls.Add(this.btnDelete, 0, 2);
				this.navigationButtonsTableLayoutPanel.Controls.Add(this.moveDownButton, 0, 1);
				this.navigationButtonsTableLayoutPanel.Margin = new Padding(3, 3, 0x12, 3);
				this.navigationButtonsTableLayoutPanel.Name = "navigationButtonsTableLayoutPanel";
				this.navigationButtonsTableLayoutPanel.RowStyles.Add(new RowStyle());
				this.navigationButtonsTableLayoutPanel.RowStyles.Add(new RowStyle());
				this.navigationButtonsTableLayoutPanel.RowStyles.Add(new RowStyle());
				base.AcceptButton = this.okButton;
				this.AutoScaleDimensions = new SizeF(6f, 13f);
				this.ClientSize = new Size(642, 366);
				this.MinimumSize = new Size(600, 400);
				this.StartPosition = FormStartPosition.CenterParent;
				this.Text = "BreadCrumb Node Editor";
				base.AutoScaleMode = AutoScaleMode.Font;
				base.CancelButton = this.btnCancel;
				base.Controls.Add(this.overarchingTableLayoutPanel);
				base.HelpButton = true;
				base.MaximizeBox = false;
				base.MinimizeBox = false;
				base.Name = "TreeNodeCollectionEditor";
				base.ShowIcon = false;
				base.ShowInTaskbar = false;
				base.SizeGripStyle = SizeGripStyle.Show;
				this.hintPanel.ResumeLayout();
				this.hintPanel.PerformLayout();
				this.okCancelPanel.ResumeLayout(false);
				this.okCancelPanel.PerformLayout();
				this.nodeControlPanel.ResumeLayout(false);
				this.nodeControlPanel.PerformLayout();
				this.overarchingTableLayoutPanel.ResumeLayout(false);
				this.overarchingTableLayoutPanel.PerformLayout();
				this.navigationButtonsTableLayoutPanel.ResumeLayout(false);
				base.ResumeLayout(false);
			}
			#endregion
			static readonly string BaseNodeName = "BreadCrumbNode";
			TreeNode Add(TreeNode parent) {
				return Add(parent, BaseNodeName);
			}
			TreeNode Add(TreeNode parent, string baseNodeName) {
				TreeNode node = null;
				if(parent == null) {
					BreadCrumbNode bcn = CreateNewNode(baseNodeName);
					bcn.SetProperties(this.properties);
					node = DesignUtils.CreateTreeNode(bcn);
					treeView1.Nodes.Add(node);
					node.Name = node.Text;
				}
				else {
					BreadCrumbNode breadCrumbNode = CreateNewNode(baseNodeName);
					breadCrumbNode.SetProperties(this.properties);
					node = DesignUtils.CreateTreeNode(breadCrumbNode);
					parent.Nodes.Add(node);
					((BreadCrumbNode)parent.Tag).ChildNodes.Add(breadCrumbNode);
					node.Name = node.Text;
					parent.Expand();
				}
				if(parent != null) {
					this.treeView1.SelectedNode = parent;
				}
				else {
					this.treeView1.SelectedNode = node;
					SetNodeProps(node);
				}
				return node;
			}
			BreadCrumbNode CreateNewNode(string baseName) {
				string newVal = GetUniqueNodeValue(baseName);
				return new BreadCrumbNode(newVal, newVal);
			}
			protected string GetUniqueNodeValue(string baseName) {
				for(int i = 1; true; i++) {
					string newVal = baseName + i.ToString();
					if(!DesignUtils.ExistsNode(this.treeView1, node => ((BreadCrumbNode)node.Tag).Value.Equals(newVal))) return newVal;
				}
			}
			void OnnAddChildClick(object sender, EventArgs e) {
				Add(curNode);
				UpdateUI();
			}
			void OnAddRootClick(object sender, EventArgs e) {
				Add(null);
				UpdateUI();
			}
			void OnCancelClick(object sender, EventArgs e) {
			}
			void OnDeleteClick(object sender, EventArgs e) {
				TreeNode parentNode = this.curNode.Parent;
				if(parentNode != null) {
					((BreadCrumbNode)parentNode.Tag).ChildNodes.Remove((BreadCrumbNode)this.curNode.Tag);
				}
				this.curNode.Remove();
				if(this.treeView1.Nodes.Count == 0) {
					this.curNode = null;
					SetNodeProps(null);
				}
				UpdateUI();
			}
			void OnHintLabelHyperlinkClick(object sender, HyperlinkClickEventArgs e) {
				AddDefaultRootNode();
				UpdateUI();
			}
			void AddDefaultRootNode() {
				IList<TreeNode> treeNodes = GetNonPersistentNodes();
				if(treeNodes.Count() <= 1) return;
				foreach(TreeNode node in treeNodes) {
					treeView1.Nodes.Remove(node);
				}
				TreeNode rootNode = Add(null, "AutoGeneratedRoot");
				BreadCrumbNode breadCrumbRootNode = (BreadCrumbNode)rootNode.Tag;
				breadCrumbRootNode.ShowCaption = (rootNode.Index > 0);
				foreach(TreeNode child in treeNodes) {
					rootNode.Nodes.Add(child);
					breadCrumbRootNode.ChildNodes.Add((BreadCrumbNode)child.Tag);
				}
				rootNode.ExpandAll();
				this.properties.SelectedNode = null;
			}
			void OnOKClick(object sender, EventArgs e) {
				DoSave();
				RaiseComponentChanged(this.properties);
			}
			protected void RaiseComponentChanged(object obj) {
				IComponentChangeService svc = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				if(svc != null) {
					svc.OnComponentChanging(obj, null);
					svc.OnComponentChanged(obj, null, null, null);
				}
			}
			void DoSave() {
				DesignUtils.DoSave(this.properties, this.treeView1);
				this.treeView1.Dispose();
				this.treeView1 = null;
			}
			bool CheckParent(TreeNode child, TreeNode parent) {
				while(child != null) {
					if(parent == child.Parent) {
						return true;
					}
					child = child.Parent;
				}
				return false;
			}
			void HookEvents() {
				this.okButton.Click += OnOKClick;
				this.hintLabel.HyperlinkClick += OnHintLabelHyperlinkClick;
				this.btnCancel.Click += OnCancelClick;
				this.btnAddChild.Click += OnnAddChildClick;
				this.btnAddRoot.Click += OnAddRootClick;
				this.btnDelete.Click += OnDeleteClick;
				this.propertyGrid.PropertyValueChanged += OnPropertyValueChanged;
				this.treeView1.AfterSelect += OnTreeViewAfterSelect;
				this.treeView1.DragEnter += OnTreeViewDragEnter;
				this.treeView1.ItemDrag += OnTreeViewItemDrag;
				this.treeView1.DragDrop += OnTreeViewDragDrop;
				this.treeView1.DragOver += OnTreeViewDragOver;
				base.HelpButtonClicked += OnCollectionEditorHelpButtonClicked;
				this.moveDownButton.Click += OnMoveDownClick;
				this.moveUpButton.Click += OnMoveUpClick;
			}
			void OnMoveDownClick(object sender, EventArgs e) {
				TreeNode curNode = this.curNode;
				TreeNode parent = this.curNode.Parent;
				if(parent == null) {
					this.treeView1.Nodes.RemoveAt(curNode.Index);
					this.treeView1.Nodes[curNode.Index].Nodes.Insert(0, curNode);
				}
				else {
					parent.Nodes.RemoveAt(curNode.Index);
					if(curNode.Index < parent.Nodes.Count) {
						parent.Nodes[curNode.Index].Nodes.Insert(0, curNode);
					}
					else if(parent.Parent == null) {
						this.treeView1.Nodes.Insert(parent.Index + 1, curNode);
					}
					else {
						parent.Parent.Nodes.Insert(parent.Index + 1, curNode);
					}
				}
				this.treeView1.SelectedNode = curNode;
				this.curNode = curNode;
			}
			void OnMoveUpClick(object sender, EventArgs e) {
				TreeNode curNode = this.curNode;
				TreeNode parent = this.curNode.Parent;
				if(parent == null) {
					this.treeView1.Nodes.RemoveAt(curNode.Index);
					this.treeView1.Nodes[curNode.Index - 1].Nodes.Add(curNode);
				}
				else {
					parent.Nodes.RemoveAt(curNode.Index);
					if(curNode.Index == 0) {
						if(parent.Parent == null) {
							this.treeView1.Nodes.Insert(parent.Index, curNode);
						}
						else {
							parent.Parent.Nodes.Insert(parent.Index, curNode);
						}
					}
					else {
						parent.Nodes[curNode.Index - 1].Nodes.Add(curNode);
					}
				}
				this.treeView1.SelectedNode = curNode;
				this.curNode = curNode;
			}
			protected override void OnEditValueChanged() {
				if(base.EditValue == null)
					return;
				InitUI();
			}
			void OnPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
				PropertyGrid propertyGrid = (PropertyGrid)sender;
				BreadCrumbNode breadCrumbNode = (BreadCrumbNode)propertyGrid.SelectedObject;
				SyncCaptionAndValueIfNeeded(propertyGrid, breadCrumbNode, e);
				UpdateCurrentTreeNode(this.curNode, breadCrumbNode);
			}
			void SyncCaptionAndValueIfNeeded(PropertyGrid propertyGrid, BreadCrumbNode breadCrumbNode, PropertyValueChangedEventArgs e) {
				if(breadCrumbNode == null || !string.Equals(e.ChangedItem.Label, "Caption", StringComparison.Ordinal))
					return;
				string nodeValue = breadCrumbNode.Value as string;
				if(nodeValue == null) return;
				if(string.Equals(e.OldValue.ToString(), nodeValue, StringComparison.Ordinal)) {
					breadCrumbNode.Value = breadCrumbNode.Caption;
					propertyGrid.Refresh();
				}
			}
			void UpdateCurrentTreeNode(TreeNode node, BreadCrumbNode breadCrumbNode) {
				if(node == null || breadCrumbNode == null) return;
				if(!string.IsNullOrEmpty(breadCrumbNode.Caption)) {
					node.Text = breadCrumbNode.Caption;
				}
			}
			void UpdateUI() {
				UpdateButtons();
				UpdateHintPanel();
			}
			void UpdateButtons() {
				bool flag = this.treeView1.Nodes.Count > 0;
				this.btnAddChild.Enabled = flag;
				this.btnDelete.Enabled = flag;
				this.moveDownButton.Enabled = (flag && ((this.curNode != this.LastNode) || (this.curNode.Level > 0))) && (this.curNode != this.treeView1.Nodes[this.treeView1.Nodes.Count - 1]);
				this.moveUpButton.Enabled = flag && (this.curNode != this.treeView1.Nodes[0]);
			}
			void UpdateHintPanel() {
				this.hintPanel.Visible = GetNonPersistentNodes().Count > 1;
			}
			IList<TreeNode> GetNonPersistentNodes() {
				List<TreeNode> res = new List<TreeNode>();
				foreach(TreeNode node in treeView1.Nodes) {
					BreadCrumbNode bcn = (BreadCrumbNode)node.Tag;
					if(!bcn.Persistent) res.Add(node);
				}
				return res;
			}
			void SetImageProps(BreadCrumbEdit breadCrumb) {
				ImageList list = CreateImageList(breadCrumb);
				if(list == null) return;
				this.treeView1.ImageList = list;
				this.treeView1.ImageIndex = this.treeView1.SelectedImageIndex = breadCrumb.Properties.ImageIndex;
			}
			ImageList CreateImageList(BreadCrumbEdit breadCrumb) {
				object images = breadCrumb.Properties.Images;
				if(images == null) return null;
				int imageCount = ImageCollection.GetImageListImageCount(images);
				if(imageCount == 0) return null;
				ImageList list = new ImageList();
				list.ImageSize = ImageCollection.GetImageListSize(images);
				for(int i = 0; i < imageCount; i++) {
					Image img = ImageCollection.GetImageListImage(images, i);
					list.Images.Add(img);
				}
				return list;
			}
			void SetNodeProps(TreeNode node) {
				if(node != null) {
					this.label2.Text = "Properties";
					this.propertyGrid.SelectedObject = node.Tag;
				}
				else {
					this.label2.Text = "PropertiesNone";
					this.propertyGrid.SelectedObject = null;
				}
			}
			void OnCollectionEditorHelpButtonClicked(object sender, CancelEventArgs e) {
				e.Cancel = true;
				this.editor.ShowHelp();
			}
			void OnTreeViewAfterSelect(object sender, TreeViewEventArgs e) {
				this.curNode = e.Node;
				SetNodeProps(this.curNode);
				UpdateUI();
			}
			void OnTreeViewDragDrop(object sender, DragEventArgs e) {
				TreeNode data = (TreeNode)e.Data.GetData(typeof(TreeNode));
				Point p = new Point(0, 0);
				p.X = e.X;
				p.Y = e.Y;
				p = this.treeView1.PointToClient(p);
				TreeNode nodeAt = this.treeView1.GetNodeAt(p);
				if(data != nodeAt) {
					this.treeView1.Nodes.Remove(data);
					if((nodeAt != null) && !CheckParent(nodeAt, data)) {
						nodeAt.Nodes.Add(data);
					}
					else {
						this.treeView1.Nodes.Add(data);
					}
				}
			}
			void OnTreeViewDragEnter(object sender, DragEventArgs e) {
				if(e.Data.GetDataPresent(typeof(TreeNode)))
					e.Effect = DragDropEffects.Move;
				else
					e.Effect = DragDropEffects.None;
			}
			void OnTreeViewDragOver(object sender, DragEventArgs e) {
				Point p = new Point(0, 0);
				p.X = e.X;
				p.Y = e.Y;
				p = this.treeView1.PointToClient(p);
				TreeNode nodeAt = this.treeView1.GetNodeAt(p);
				this.treeView1.SelectedNode = nodeAt;
			}
			void OnTreeViewItemDrag(object sender, ItemDragEventArgs e) {
				TreeNode item = (TreeNode)e.Item;
				base.DoDragDrop(item, DragDropEffects.Move);
			}
			TreeNode LastNode {
				get {
					TreeNode node = this.treeView1.Nodes[this.treeView1.Nodes.Count - 1];
					while(node.Nodes.Count > 0) {
						node = node.Nodes[node.Nodes.Count - 1];
					}
					return node;
				}
			}
			internal class DesignUtils {
				public static bool ExistsNode(TreeView treeView, Func<TreeNode, bool> handler) {
					return ExistsNode(treeView.Nodes, handler);
				}
				static bool ExistsNode(TreeNodeCollection nodes, Func<TreeNode, bool> handler) {
					foreach(TreeNode child in nodes) {
						if(handler(child)) return true;
						bool res = ExistsNode(child.Nodes, handler);
						if(res) return true;
					}
					return false;
				}
				public static TreeNode CreateTreeNode(BreadCrumbNode node) {
					TreeNode treeNode = CreateTreeNodeInstance(node);
					if(node.ChildNodes.Count > 0) {
						CreateTreeNodeCore(treeNode, node.ChildNodes);
					}
					return treeNode;
				}
				static void CreateTreeNodeCore(TreeNode parentTreeNode, BreadCrumbNodeCollection childNodes) {
					for(int i = 0; i < childNodes.Count; i++) {
						BreadCrumbNode bcn = childNodes[i];
						TreeNode treeNode = CreateTreeNodeInstance(bcn);
						parentTreeNode.Nodes.Add(treeNode);
						CreateTreeNodeCore(treeNode, bcn.ChildNodes);
					}
				}
				static TreeNode CreateTreeNodeInstance(BreadCrumbNode node) {
					return new TreeNode(node.Caption) { Tag = node };
				}
				public static void DoSave(RepositoryItemBreadCrumbEdit properties, TreeView treeView) {
					TreeNodeCollection col = treeView.Nodes;
					properties.Nodes.BeginUpdate();
					try {
						properties.Nodes.Clear();
						foreach(TreeNode rootNode in col) {
							properties.Nodes.Add(GetHierarchy(rootNode));
						}
						if(col.Count > 0) {
							properties.SelectedNode = properties.Nodes.FirstNode;
						}
						else {
							properties.SelectedNode = null;
						}
					}
					finally {
						properties.Nodes.EndUpdate();
					}
				}
				static BreadCrumbNode GetHierarchy(TreeNode rootNode) {
					BreadCrumbNode root = (BreadCrumbNode)rootNode.Tag;
					InitNodeHierarchy(root, rootNode);
					return root;
				}
				static void InitNodeHierarchy(BreadCrumbNode node, TreeNode treeNode) {
					node.ChildNodes.Clear();
					foreach(TreeNode child in treeNode.Nodes) {
						BreadCrumbNode childNode = (BreadCrumbNode)child.Tag;
						node.ChildNodes.Add(childNode);
						InitNodeHierarchy(childNode, child);
					}
				}
			}
			internal class VsPropertyGrid : PropertyGrid {
				public VsPropertyGrid(IServiceProvider serviceProvider) {
				}
			}
			internal class PropertyGridSite : ISite, IServiceProvider {
				IComponent comp;
				bool inGetService;
				IServiceProvider sp;
				public PropertyGridSite(IServiceProvider sp, IComponent comp) {
					this.sp = sp;
					this.comp = comp;
				}
				public object GetService(Type t) {
					if(!this.inGetService && (this.sp != null)) {
						try {
							this.inGetService = true;
							return this.sp.GetService(t);
						}
						finally {
							this.inGetService = false;
						}
					}
					return null;
				}
				public IComponent Component { get { return this.comp; } }
				public IContainer Container { get { return null; } }
				public bool DesignMode { get { return false; } }
				public string Name { get { return null; } set { } }
			}
		}
	}
}
