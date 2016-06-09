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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.Persistent.Validation;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	public class ModelEditorMruEdit : MRUEdit {
		public void OnKeyPressWrapper(KeyPressEventArgs e) {
			this.OnKeyPress(e);
		}
	}
	public class SearchControl : XtraUserControl {
		private const string SearchControlSettingsPath = "SearchControl";
		private const string MRUEditSettingsPath = "MRUEdit";
		private const string SearchSplitterPositionAttributeName = "SearchSplitterPosition";
		private const string SchemaVisibleAttributeName = "SchemaVisible";
		private const string MRUEditItemsAttributeName = "Items";
		private IContainer components;
		private ModelEditorControl parent;
		private PanelControl headPanelControl;
		private SimpleButton schemaVisibleButton;
		private SplitContainerControl searchSplitContainerControl;
		private FilterModelTreeList filteringTree;
		private ModelEditorMruEdit mruEdit;
		private ObjectTreeList schemaTreeList;
		private PanelControl labelPanel;
		private LabelControl labelSearch;
		private CloseButton buttonClose;
		private SchemaTreeListObjectAdapter adapter;
		private bool previousNodeCheckValue = true;
		private SettingsStorage settingsStorage;
		private bool isSetingsLoaded = false;
		private FastModelEditorHelper fastModelEditorHelper;
		private ModelValidator modelValidator;
		private int mruEditValueChangedDelay = 600;
		private bool raiseFocusedNodeChanged = false;
		private Timer MRUEditChangedTimer;
		private SearchControl() {
			fastModelEditorHelper = new FastModelEditorHelper();
			modelValidator = new ModelValidator(fastModelEditorHelper);
		}
		public SearchControl(ModelEditorControl parent)
			: this() {
			this.parent = parent;
			InitializeComponent();
			CreateMruEditChangedTimer();
			SubscribeRootEvents();
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.searchSplitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
			adapter = new SchemaTreeListObjectAdapter();
			this.schemaTreeList = new ObjectTreeList(adapter);
			this.filteringTree = new FilterModelTreeList(fastModelEditorHelper);
			TreeListColumn column_1 = new TreeListColumn();
			column_1.Name = "Display Name";
			column_1.VisibleIndex = 0;
			this.filteringTree.Columns.AddRange(new TreeListColumn[] { column_1 });
			this.headPanelControl = new DevExpress.XtraEditors.PanelControl();
			this.mruEdit = new ModelEditorMruEdit(); ;
			this.schemaVisibleButton = new DevExpress.XtraEditors.SimpleButton();
			this.labelPanel = new DevExpress.XtraEditors.PanelControl();
			this.labelSearch = new DevExpress.XtraEditors.LabelControl();
			this.buttonClose = new CloseButton();
			((System.ComponentModel.ISupportInitialize)(this.searchSplitContainerControl)).BeginInit();
			this.searchSplitContainerControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.labelPanel)).BeginInit();
			this.labelPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.schemaTreeList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.filteringTree)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.headPanelControl)).BeginInit();
			this.headPanelControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.mruEdit.Properties)).BeginInit();
			this.SuspendLayout();
			this.searchSplitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.searchSplitContainerControl.Horizontal = false;
			this.searchSplitContainerControl.Location = new System.Drawing.Point(2, 23);
			this.searchSplitContainerControl.Name = "SearchSplitContainerControl";
			this.searchSplitContainerControl.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.searchSplitContainerControl.Panel1.Controls.Add(this.schemaTreeList);
			this.searchSplitContainerControl.Panel1.Text = "Panel1";
			this.searchSplitContainerControl.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.searchSplitContainerControl.Panel2.Controls.Add(this.filteringTree);
			this.searchSplitContainerControl.Panel2.Text = "Panel2";
			this.searchSplitContainerControl.Size = new System.Drawing.Size(745, 520);
			this.searchSplitContainerControl.SplitterPosition = 262;
			this.searchSplitContainerControl.TabIndex = 2;
			this.searchSplitContainerControl.Text = "SearchSplitContainerControl";
			searchSplitContainerControl.FixedPanel = SplitFixedPanel.None;
			searchSplitContainerControl.Panel1.MinSize = 150;
			searchSplitContainerControl.Panel2.MinSize = 150;
			this.schemaTreeList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.schemaTreeList.Location = new System.Drawing.Point(0, 0);
			this.schemaTreeList.Name = "schemaTreeList";
			this.schemaTreeList.Size = new System.Drawing.Size(745, 262);
			this.schemaTreeList.TabIndex = 0;
			schemaTreeList.OptionsView.ShowCheckBoxes = true;
			schemaTreeList.OptionsBehavior.Editable = false;
			schemaTreeList.OptionsView.ShowIndicator = false;
			schemaTreeList.OptionsView.ShowHorzLines = false;
			schemaTreeList.OptionsView.ShowVertLines = false;
			schemaTreeList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			schemaTreeList.OptionsBehavior.AllowExpandOnDblClick = false;
			this.filteringTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.filteringTree.Location = new System.Drawing.Point(0, 0);
			this.filteringTree.Name = "reltersTreeList";
			this.filteringTree.Size = new System.Drawing.Size(745, 252);
			this.filteringTree.TabIndex = 0;
			this.filteringTree.OptionsDragAndDrop.DragNodesMode = DragNodesMode.None;
			this.filteringTree.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.labelPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.labelPanel.Controls.Add(this.buttonClose);
			this.labelPanel.Controls.Add(this.labelSearch);
			this.labelPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.labelPanel.Location = new System.Drawing.Point(4, 0);
			this.labelPanel.Name = "labelPanel";
			this.labelPanel.Size = new System.Drawing.Size(735, 14);
			this.labelPanel.TabIndex = 0;
			this.labelPanel.TabStop = false;
			this.headPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.headPanelControl.Controls.Add(this.mruEdit);
			this.headPanelControl.Controls.Add(this.schemaVisibleButton);
			this.headPanelControl.Dock = System.Windows.Forms.DockStyle.Top;
			this.headPanelControl.Location = new System.Drawing.Point(2, 14);
			this.headPanelControl.Name = "headPanelControl";
			this.headPanelControl.Size = new System.Drawing.Size(745, 21);
			this.headPanelControl.TabIndex = 1;
			this.mruEdit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mruEdit.Location = new System.Drawing.Point(0, 0);
			this.mruEdit.Name = "textEdit";
			this.mruEdit.Size = new System.Drawing.Size(670, 20);
			this.mruEdit.TabIndex = 2;
			this.mruEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			mruEdit.Properties.NullText = "<Search>";
			mruEdit.Properties.ImmediatePopup = false;
			this.schemaVisibleButton.Dock = System.Windows.Forms.DockStyle.Right;
			this.schemaVisibleButton.Location = new System.Drawing.Point(670, 0);
			this.schemaVisibleButton.Margin = new System.Windows.Forms.Padding(0);
			this.schemaVisibleButton.Name = "SchemaVisible";
			this.schemaVisibleButton.Size = new System.Drawing.Size(80, 21);
			this.schemaVisibleButton.TabIndex = 1;
			this.schemaVisibleButton.Text = "Hide Schema";
			this.schemaVisibleButton.AllowFocus = false;
			TreeListColumn column = new TreeListColumn();
			column.Caption = "Model Schema";
			column.FieldName = "Name";
			column.Width = 250;
			column.VisibleIndex = 0;
			column.OptionsColumn.AllowFocus = false;
			schemaTreeList.Columns.AddRange(new TreeListColumn[] { column });
			this.labelSearch.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelSearch.Appearance.Options.UseFont = true;
			this.labelSearch.Dock = System.Windows.Forms.DockStyle.Left;
			this.labelSearch.Location = new System.Drawing.Point(0, 0);
			this.labelSearch.Name = "labelSearch";
			this.labelSearch.Size = new System.Drawing.Size(33, 13);
			this.labelSearch.TabIndex = 0;
			this.labelSearch.Text = " Search";
			this.buttonClose.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonClose.Location = new System.Drawing.Point(721, 0);
			this.buttonClose.Margin = new System.Windows.Forms.Padding(0);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(13, 12);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.TabStop = false;
			this.buttonClose.AllowFocus = false;
			this.Controls.Add(this.searchSplitContainerControl);
			this.Controls.Add(this.headPanelControl);
			this.Controls.Add(this.labelPanel);
			this.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Location = new System.Drawing.Point(0, 0);
			this.Name = "rootPanelControl";
			this.Size = new System.Drawing.Size(749, 545);
			this.TabIndex = 0;
			this.Visible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			((System.ComponentModel.ISupportInitialize)(this.searchSplitContainerControl)).EndInit();
			this.searchSplitContainerControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.labelPanel)).EndInit();
			this.labelPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.schemaTreeList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.filteringTree)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.headPanelControl)).EndInit();
			this.headPanelControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.mruEdit.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		private void SubscribeRootEvents() {
			VisibleChanged += new EventHandler(SearchControl_VisibleChanged);
			buttonClose.Click += new EventHandler(buttonClose_Click);
			schemaVisibleButton.Click += new EventHandler(schemaVisible_Click);
			mruEdit.QueryCloseUp += new CancelEventHandler(mruEdit_QueryCloseUp);
			mruEdit.KeyDown += new KeyEventHandler(mruEdit_KeyDown);
			mruEdit.EditValueChanged += new EventHandler(mruEdit_EditValueChanged);
			filteringTree.GotFocus += new EventHandler(filteringTree_GotFocus);
			filteringTree.KeyDown += new KeyEventHandler(filteringTree_KeyDown);
			filteringTree.KeyPress += new KeyPressEventHandler(filteringTree_KeyPress);
			schemaTreeList.MouseClick += new MouseEventHandler(schemaTreeList_MouseClick);
			schemaTreeList.BeforeCheckNode += new CheckNodeEventHandler(schemaTreeList_BeforeCheckNode);
			schemaTreeList.AfterCheckNode += new NodeEventHandler(schemaTreeList_AfterCheckNode);
		}
		private void UnsubscribeRootEvents() {
			VisibleChanged -= new EventHandler(SearchControl_VisibleChanged);
			if(buttonClose != null) {
				buttonClose.Click -= new EventHandler(buttonClose_Click);
			}
			if(schemaVisibleButton != null) {
				schemaVisibleButton.Click -= new EventHandler(schemaVisible_Click);
			}
			if(mruEdit != null) {
				mruEdit.QueryCloseUp -= new CancelEventHandler(mruEdit_QueryCloseUp);
				mruEdit.KeyDown -= new KeyEventHandler(mruEdit_KeyDown);
				mruEdit.EditValueChanged -= new EventHandler(mruEdit_EditValueChanged);
			}
			if(filteringTree != null) {
				filteringTree.GotFocus -= new EventHandler(filteringTree_GotFocus);
				filteringTree.KeyDown -= new KeyEventHandler(filteringTree_KeyDown);
				filteringTree.KeyPress -= new KeyPressEventHandler(filteringTree_KeyPress);
			}
			if(schemaTreeList != null) {
				schemaTreeList.MouseClick -= new MouseEventHandler(schemaTreeList_MouseClick);
				schemaTreeList.BeforeCheckNode -= new CheckNodeEventHandler(schemaTreeList_BeforeCheckNode);
				schemaTreeList.AfterCheckNode -= new NodeEventHandler(schemaTreeList_AfterCheckNode);
			}
		}
		private object GetSchemaTreeListDataSource(ModelNode dataSource) {
			return XafTypesInfo.Instance.FindTypeInfo(dataSource.GetType());
		}
		private void KeyDownLogic(object sender, KeyEventArgs e) {
			if(mruEdit.IsPopupOpen) {
				return;
			}
			if(e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) {
				if(!filteringTree.Focused) { filteringTree.Focus(); }
				if(filteringTree.Nodes.Count > 0) {
					FocusNextNode(filteringTree.FocusedNode, e.KeyCode == Keys.Down, true);
					if(!string.IsNullOrEmpty(mruEdit.Text)) {
						mruEdit.Properties.Items.Add(mruEdit.Text);
					}
				}
				e.SuppressKeyPress = true;
			}
			else {
				if(e.KeyCode == Keys.Enter && sender is FilterModelTreeList) {
					parent.modelTreeList.Focus();
				}
				else {
					if(e.KeyCode == Keys.Enter) {
						StopMruEditChangedTimer();
						raiseFocusedNodeChanged = false;
						e.Handled = true;
						ApplyFilter();
					}
				}
			}
			if(e.KeyCode == Keys.Right && filteringTree.Focused && filteringTree.FocusedNode != null) {
				if(!filteringTree.FocusedNode.Expanded) { filteringTree.FocusedNode.Expanded = true; }
				FocusNextNode(filteringTree.FocusedNode, true, false);
				e.Handled = true;
			}
		}
		private void FocusNextNode(TreeListNode startNode, bool isMoveDown, bool filters) {
			if(startNode == null) {
				startNode = GetRootNodeNode();
				if(startNode == null) {
					return;
				}
			}
			TreeListNode nextNode = startNode;
			do {
				startNode = isMoveDown ? startNode.NextVisibleNode : startNode.PrevVisibleNode;
				if(startNode == null) {
					startNode = nextNode;
					break;
				}
				if(filters) {
					ModelNode modelNode = (ModelNode)filteringTree.GetDataRecordByNode(startNode);
					if(modelNode != null) {
						string name = fastModelEditorHelper.GetModelNodeDisplayValue(modelNode);
						if(name.ToLower().Contains(GetMRUEditText)) {
							break;
						}
					}
				}
				else {
					break;
				}
			}
			while(startNode != nextNode);
			filteringTree.FocusedNode = startNode;
		}
		private bool IsChildrenNodeSelected(TreeListNode rootNode) {
			foreach(TreeListNode node in rootNode.Nodes) {
				if(node.Checked) {
					return true;
				}
			}
			return false;
		}
		private void CheckChildrenNode(bool chek, TreeListNodes nodes) {
			foreach(TreeListNode node in nodes) {
				node.Checked = chek;
				CheckChildrenNode(chek, node.Nodes);
			}
		}
		private void CheckParentNode(bool chek, TreeListNode node) {
			if(node.ParentNode != null && chek) {
				node.ParentNode.Checked = chek;
				CheckParentNode(chek, node.ParentNode);
			}
		}
		private void SetingsStorage() {
			if(Visible && !isSetingsLoaded && SettingsStorage != null) {
				isSetingsLoaded = true;
				searchSplitContainerControl.SplitterPosition = settingsStorage.LoadIntOption(SearchControlSettingsPath, SearchSplitterPositionAttributeName, (int)(searchSplitContainerControl.Size.Height / 3));
				if(Convert.ToBoolean(settingsStorage.LoadIntOption(SearchControlSettingsPath, SchemaVisibleAttributeName, 0))) {
					searchSplitContainerControl.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both;
					schemaTreeList.Visible = true;
					schemaVisibleButton.Text = "Hide Schema";
				}
				else {
					schemaTreeList.Visible = false;
					schemaVisibleButton.Text = "Show Schema";
					searchSplitContainerControl.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;
				}
				MRUEditStorage();
			}
		}
		private void MRUEditStorage() {
			this.mruEdit.EditValueChanged -= new EventHandler(mruEdit_EditValueChanged);
			string items = settingsStorage.LoadOption(MRUEditSettingsPath, MRUEditItemsAttributeName);
			if(!string.IsNullOrEmpty(items)) {
				List<string> mruEditItems = new List<string>(items.Split(';'));
				mruEditItems.Reverse();
				mruEdit.Properties.Items.AddRange(mruEditItems);
			}
			this.mruEdit.EditValueChanged += new EventHandler(mruEdit_EditValueChanged);
		}
		private void SaveMRUEditSettings() {
			if(mruEdit.Properties.Items.Count > 0) {
				List<string> mruEditsettingsStorage = new List<string>(mruEdit.Properties.Items.Count);
				if(GetMRUEditText != string.Empty) {
					mruEditsettingsStorage.Add(mruEdit.EditValue.ToString());
				}
				foreach(object item in mruEdit.Properties.Items) {
					mruEditsettingsStorage.Add(item.ToString());
				}
				string items = string.Join(";", mruEditsettingsStorage.ToArray());
				settingsStorage.SaveOption(MRUEditSettingsPath, MRUEditItemsAttributeName, items);
			}
		}
		private void InitializeSchemaTreeList() {
			adapter.ModelApplication = (ModelApplicationBase)parent.DataSource;
			if(parent.DataSource != null) {
				schemaTreeList.DataSource = GetSchemaTreeListDataSource(parent.DataSource);
				schemaTreeList.GetNodeByVisibleIndex(0).Checked = true;
				CheckNode(schemaTreeList.GetNodeByVisibleIndex(0), true);
				schemaTreeList.Columns[0].SortOrder = SortOrder.Ascending;
				schemaTreeList.Columns[0].OptionsColumn.AllowSort = false;
			}
		}
		private void UnsubscribeEvents() {
			if(filteringTree != null) {
				filteringTree.BeforeFocusNode -= new BeforeFocusNodeEventHandler(filteringTree_BeforeFocusNode);
				filteringTree.FocusedNodeChanged -= new FocusedNodeChangedEventHandler(filteringTree_FocusedNodeChanged);
			}
		}
		private void SubscribeEvents() {
			if(!this.Visible) { return; }
			filteringTree.FocusedNodeChanged += new FocusedNodeChangedEventHandler(filteringTree_FocusedNodeChanged);
			filteringTree.BeforeFocusNode += new BeforeFocusNodeEventHandler(filteringTree_BeforeFocusNode);
		}
		private void buttonClose_Click(object sender, EventArgs e) {
			if(SearchControlClosed != null) {
				SearchControlClosed(this, EventArgs.Empty);
			}
			this.Visible = false;
		}
		private void filteringTree_GotFocus(object sender, EventArgs e) {
			if(filteringTree.FocusedNode != null) {
				ModelNode modelNode = parent.CurrentModelTreeListNode != null ? parent.CurrentModelTreeListNode.ModelNode : null;
				RuleSetValidationResult validationResult = modelValidator.ValidateNode(modelNode);
				if(validationResult != null && validationResult.State == ValidationState.Invalid) {
					return;
				}
				parent.CurrentModelNode = (ModelNode)filteringTree.GetDataRecordByNode(filteringTree.FocusedNode);
			}
		}
		private void filteringTree_KeyPress(object sender, KeyPressEventArgs e) {
			if(e.KeyChar != '\r') {
				if(!mruEdit.Focused) { mruEdit.Focus(); }
				mruEdit.OnKeyPressWrapper(e);
			}
		}
		private void filteringTree_KeyDown(object sender, KeyEventArgs e) {
			KeyDownLogic(sender, e);
		}
		private void mruEdit_EditValueChanged(object sender, EventArgs e) {
			ResetFilter();
			if(!mruEdit.IsPopupOpen) {
				if(String.IsNullOrEmpty(mruEdit.EditValue as String)) {
					mruEdit.EditValue = null;
				}
				StopMruEditChangedTimer();
				StartMruEditChangedTimer();
				if(raiseFocusedNodeChanged) {
					StopMruEditChangedTimer();
					raiseFocusedNodeChanged = false;
					ApplyFilter();
				}
			}
		}
		private void CreateMruEditChangedTimer() {
			MRUEditChangedTimer = new System.Windows.Forms.Timer();
			MRUEditChangedTimer.Interval = mruEditValueChangedDelay;
			MRUEditChangedTimer.Tick += new EventHandler(focusedNodeChangedOnKeyUpTimer_Tick);
		}
		private void StopMruEditChangedTimer() {
			MRUEditChangedTimer.Stop();
			MRUEditChangedTimer.Enabled = false;
		}
		private void StartMruEditChangedTimer() {
			MRUEditChangedTimer.Enabled = true;
			MRUEditChangedTimer.Start();
		}
		private void ReleaseMruEditChangedTimer() {
			if(MRUEditChangedTimer != null) {
				StopMruEditChangedTimer();
				MRUEditChangedTimer.Tick -= new EventHandler(focusedNodeChangedOnKeyUpTimer_Tick);
			}
		}
		private void focusedNodeChangedOnKeyUpTimer_Tick(object sender, EventArgs e) {
			raiseFocusedNodeChanged = true;
			mruEdit_EditValueChanged(null, EventArgs.Empty);
		}
		private void mruEdit_KeyDown(object sender, KeyEventArgs e) {
			KeyDownLogic(sender, e);
		}
		private void mruEdit_QueryCloseUp(object sender, CancelEventArgs e) {
			ApplyFilter();
		}
		private void SearchControl_VisibleChanged(object sender, EventArgs e) {
			UpdateControl();
		}
		private void filteringTree_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
			ModelNode focusedNode = (ModelNode)e.Node.TreeList.GetDataRecordByNode(e.Node);
			if(CanFocusedNode(focusedNode)) {
				parent.CurrentModelNode = focusedNode;
			}
		}
		void filteringTree_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e) {
			e.CanFocus = CanFocusedNode(parent.CurrentModelTreeListNode.ModelNode);
		}
		private bool CanFocusedNode(ModelNode node) {
			if(node != null) {
				ModelNode modelNode = parent.CurrentModelTreeListNode != null ? parent.CurrentModelTreeListNode.ModelNode : null;
				RuleSetValidationResult validationResult = modelValidator.ValidateNode(modelNode);
				if(validationResult != null && validationResult.State == ValidationState.Invalid) {
					return false;
				}
			}
			return true;
		}
		private void searchSplitContainerControl_Paint(object sender, PaintEventArgs e) {
			searchSplitContainerControl.Paint -= new PaintEventHandler(searchSplitContainerControl_Paint);
			searchSplitContainerControl.SplitterPosition = settingsStorage.LoadIntOption(SearchControlSettingsPath, SearchSplitterPositionAttributeName, (int)(searchSplitContainerControl.Size.Height / 3));
		}
		private void schemaVisible_Click(object sender, EventArgs e) {
			if(searchSplitContainerControl.PanelVisibility == DevExpress.XtraEditors.SplitPanelVisibility.Both) {
				searchSplitContainerControl.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;
				schemaTreeList.Visible = false;
				schemaVisibleButton.Text = "Show Schema";
			}
			else {
				searchSplitContainerControl.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Both;
				schemaTreeList.Visible = true;
				schemaVisibleButton.Text = "Hide Schema";
			}
		}
		private void schemaTreeList_MouseClick(object sender, MouseEventArgs e) {
			TreeListHitInfo hitInfo = schemaTreeList.CalcHitInfo(new Point(e.X, e.Y));
			if(hitInfo.Node != null && hitInfo.Column != null && hitInfo.Node == (TreeListNode)schemaTreeList.FocusedNode) {
				hitInfo.Node.Checked = !hitInfo.Node.Checked;
				CheckNode(hitInfo.Node, hitInfo.Node.Checked);
				ApplyFilter();
			}
		}
		private void schemaTreeList_AfterCheckNode(object sender, NodeEventArgs e) {
			CheckNode(e.Node, e.Node.Checked);
			ApplyFilter();
		}
		private void schemaTreeList_BeforeCheckNode(object sender, CheckNodeEventArgs e) {
			previousNodeCheckValue = e.Node.Checked;
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					UnsubscribeEvents();
					UnsubscribeRootEvents();
					ReleaseMruEditChangedTimer();
					DisposeFields();
					if(components != null) {
						components.Dispose();
					}
					if(adapter != null) {
						adapter.Dispose();
						adapter = null;
					}
					settingsStorage = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		private void DisposeFields() {
			if(schemaVisibleButton != null) {
				schemaVisibleButton.Dispose();
				schemaVisibleButton = null;
			}
			if(filteringTree != null) {
				filteringTree.Dispose();
				filteringTree = null;
			}
			if(mruEdit != null) {
				mruEdit.Dispose();
				mruEdit = null;
			}
			if(schemaTreeList != null) {
				schemaTreeList.Dispose();
				schemaTreeList = null;
			}
			if(labelSearch != null) {
				labelSearch.Dispose();
				labelSearch = null;
			}
			if(buttonClose != null) {
				buttonClose.Dispose();
				buttonClose = null;
			}
			if(adapter != null) {
				adapter.Dispose();
				adapter = null;
			}
			if(MRUEditChangedTimer != null) {
				MRUEditChangedTimer.Dispose();
				MRUEditChangedTimer = null;
			}
			searchSplitContainerControl = null;
			parent = null;
			labelPanel = null;
			headPanelControl = null;
			settingsStorage = null;
			modelValidator = null;
		}
		public TreeList GetFilteringTree {
			get { return filteringTree; }
		}
		public ObjectTreeList GetSchemaTreeList {
			get { return schemaTreeList; }
		}
		public MRUEdit GetMRUEdit {
			get { return mruEdit; }
		}
		public TreeListNode GetRootNodeNode() {
			return filteringTree.GetNodeByVisibleIndex(0);
		}
		public bool PreviousNodeCheckValue {
			get { return previousNodeCheckValue; }
			set { previousNodeCheckValue = value; }
		}
		public void CheckNode(TreeListNode node, bool check) {
			if(!node.Checked || node.ParentNode == null || node.ParentNode.ParentNode == null) {
				if((node.Nodes.Count > 0 && !IsChildrenNodeSelected(node) && previousNodeCheckValue)) {
					check = previousNodeCheckValue;
					node.Checked = check;
				}
				CheckChildrenNode(check, node.Nodes);
			}
			CheckParentNode(check, node);
			TreeListNode rootNode = schemaTreeList.GetNodeByVisibleIndex(0);
			rootNode.Checked = IsChildrenNodeSelected(rootNode);
		}
		private bool isResetFilter = false;
		private void ResetFilter() {
			if(!isResetFilter) {
				ApplyFilter("");
			}
		}
		private void ApplyFilter(string pattern_) {
			if(parent.modelTreeList.DataSource != null) {
				string pattern = pattern_ != null ? pattern_ : string.Empty;
				isResetFilter = string.IsNullOrEmpty(pattern);
				filteringTree.BeforeFocusNode -= new BeforeFocusNodeEventHandler(filteringTree_BeforeFocusNode);
				filteringTree.FocusedNodeChanged -= new FocusedNodeChangedEventHandler(filteringTree_FocusedNodeChanged);
				filteringTree.SetFilter(pattern, adapter.CollectSelectedNodePath(schemaTreeList));
				filteringTree.DataSource = null;
				filteringTree.SetRootNode(((ModelTreeListNode[])parent.modelTreeList.DataSource)[0].ModelNode);
				filteringTree.DataSource = new object();
				filteringTree.ExpandAll();
				filteringTree.FocusedNodeChanged += new FocusedNodeChangedEventHandler(filteringTree_FocusedNodeChanged);
				filteringTree.BeforeFocusNode += new BeforeFocusNodeEventHandler(filteringTree_BeforeFocusNode);
			}
		}
		public void ApplyFilter() {
			string pattern = mruEdit.EditValue != null ? mruEdit.EditValue.ToString() : string.Empty;
			ApplyFilter(pattern);
		}
		public void SaveSettings() {
			settingsStorage.SaveOption(SearchControlSettingsPath, SearchSplitterPositionAttributeName,
				searchSplitContainerControl.SplitterPosition.ToString());
			settingsStorage.SaveOption(SearchControlSettingsPath, SchemaVisibleAttributeName,
				Convert.ToString(Convert.ToInt32(searchSplitContainerControl.PanelVisibility
													== DevExpress.XtraEditors.SplitPanelVisibility.Both)));
			SaveMRUEditSettings();
		}
		public SettingsStorage SettingsStorage {
			get {
				return settingsStorage;
			}
			set {
				if(settingsStorage == value)
					return;
				settingsStorage = value;
				SetingsStorage();
				searchSplitContainerControl.Paint += new PaintEventHandler(searchSplitContainerControl_Paint);
			}
		}
		public string GetMRUEditText {
			get {
				return mruEdit.EditValue != null ? mruEdit.EditValue.ToString().ToLower() : "";
			}
		}
		public void UpdateControl() {
			adapter.Reset();
			if(this.Visible) {
				UnsubscribeEvents();
				InitializeSchemaTreeList();
				SetingsStorage();
				SubscribeEvents();
			}
			else {
				UnsubscribeEvents();
			}
		}
		public event EventHandler SearchControlClosed;
#if DebugTest
		public void DebugTest_FocusNextNode(TreeListNode startNode, bool isMoveDown, bool filters) {
			FocusNextNode(startNode, isMoveDown, filters);
		}
		public TreeList DebugTest_FilteringTree {
			get { return filteringTree; }
		}
#endif
	}
}
