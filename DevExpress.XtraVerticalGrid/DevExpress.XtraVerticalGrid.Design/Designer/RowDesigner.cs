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
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using System.Windows.Forms.Design;
namespace DevExpress.XtraVerticalGrid.Frames {
	[ToolboxItem(false)]
	public class RowDesigner : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		private DevExpress.XtraEditors.DropDownButton btAdd;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.ImageList imageList1;
		protected DevExpress.XtraVerticalGrid.Design.MyTreeView treeView1;
		private DevExpress.XtraEditors.SimpleButton btRemove;
		private System.Windows.Forms.Panel pnlProperties;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private SimpleButton sbClearAll;
		protected DevExpress.XtraEditors.SimpleButton btnFields;
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RowDesigner));
			this.treeView1 = new DevExpress.XtraVerticalGrid.Design.MyTreeView();
			this.imageList1 = new System.Windows.Forms.ImageList();
			this.btAdd = new DevExpress.XtraEditors.DropDownButton();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.btRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btnFields = new DevExpress.XtraEditors.SimpleButton();
			this.pnlProperties = new System.Windows.Forms.Panel();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.sbClearAll = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(160, 96);
			this.splMain.Size = new System.Drawing.Size(5, 392);
			this.pgMain.Location = new System.Drawing.Point(165, 96);
			this.pgMain.Size = new System.Drawing.Size(551, 392);
			this.pnlControl.Controls.Add(this.sbClearAll);
			this.pnlControl.Controls.Add(this.btnFields);
			this.pnlControl.Controls.Add(this.btRemove);
			this.pnlControl.Controls.Add(this.btAdd);
			this.pnlControl.Location = new System.Drawing.Point(0, 38);
			this.pnlControl.Size = new System.Drawing.Size(716, 54);
			this.lbCaption.Size = new System.Drawing.Size(716, 42);
			this.pnlMain.Controls.Add(this.groupControl1);
			this.pnlMain.Location = new System.Drawing.Point(0, 92);
			this.pnlMain.Size = new System.Drawing.Size(160, 396);
			this.horzSplitter.Location = new System.Drawing.Point(160, 92);
			this.horzSplitter.Size = new System.Drawing.Size(556, 4);
			this.horzSplitter.Visible = false;
			this.treeView1.AllowDrop = true;
			this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.HideSelection = false;
			this.treeView1.ImageIndex = 0;
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.Location = new System.Drawing.Point(4, 23);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = 0;
			this.treeView1.Size = new System.Drawing.Size(152, 369);
			this.treeView1.TabIndex = 2;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
			this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
			this.treeView1.DragLeave += new System.EventHandler(this.treeView1_DragLeave);
			this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
			this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
			this.treeView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseMove);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.imageList1.Images.SetKeyName(2, "");
			this.imageList1.Images.SetKeyName(3, "");
			this.imageList1.Images.SetKeyName(4, "");
			this.imageList1.Images.SetKeyName(5, "");
			this.imageList1.Images.SetKeyName(6, "");
			this.btAdd.Location = new System.Drawing.Point(0, 4);
			this.btAdd.Name = "btAdd";
			this.btAdd.Size = new System.Drawing.Size(184, 30);
			this.btAdd.TabIndex = 1;
			this.btAdd.Text = "&Add";
			this.btAdd.ArrowButtonClick += new System.EventHandler(this.btAddM_Click);
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			this.btRemove.Location = new System.Drawing.Point(190, 4);
			this.btRemove.Name = "btRemove";
			this.btRemove.Size = new System.Drawing.Size(96, 30);
			this.btRemove.TabIndex = 3;
			this.btRemove.Text = "&Remove";
			this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
			this.btnFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFields.Location = new System.Drawing.Point(581, 4);
			this.btnFields.Name = "btnFields";
			this.btnFields.Size = new System.Drawing.Size(128, 30);
			this.btnFields.TabIndex = 4;
			this.btnFields.Text = "Retrieve &Fields";
			this.btnFields.Click += new System.EventHandler(this.btnFields_Click);
			this.pnlProperties.Location = new System.Drawing.Point(228, 92);
			this.pnlProperties.Name = "pnlProperties";
			this.pnlProperties.Size = new System.Drawing.Size(220, 128);
			this.pnlProperties.TabIndex = 10;
			this.groupControl1.Controls.Add(this.treeView1);
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.Location = new System.Drawing.Point(0, 0);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Padding = new System.Windows.Forms.Padding(2);
			this.groupControl1.Size = new System.Drawing.Size(160, 396);
			this.groupControl1.TabIndex = 3;
			this.groupControl1.Text = "Rows:";
			this.sbClearAll.Location = new System.Drawing.Point(292, 4);
			this.sbClearAll.Name = "sbClearAll";
			this.sbClearAll.Size = new System.Drawing.Size(134, 30);
			this.sbClearAll.TabIndex = 5;
			this.sbClearAll.Text = "Clear &All Rows";
			this.sbClearAll.Click += new System.EventHandler(this.sbClearAll_Click);
			this.Controls.Add(this.pnlProperties);
			this.Name = "RowDesigner";
			this.Size = new System.Drawing.Size(716, 488);
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.pnlControl, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			this.Controls.SetChildIndex(this.pnlProperties, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			this.Controls.SetChildIndex(this.splMain, 0);
			this.Controls.SetChildIndex(this.pgMain, 0);
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
		#region Init
		public VGridControlBase EditingVGrid { get { return EditingObject as VGridControlBase; } }
		int lastRowTypeId;
		string[] rowKinds = new string[] { "CategoryRow", "EditorRow", "MultiEditorRow" };
		protected override string DescriptionText { get { return "Drag-and-drop a row to move it within the list (if the Ctrl key is pressed, the row will be inserted before the selected row; otherwise, it is inserted as a child row). When the SHIFT key is pressed, you can copy an EditorRow to a MultiEditorRow."; } }
		public RowDesigner() : base(1) {
			lastRowTypeId = 1;
			InitializeComponent();
			pgMain.BringToFront();
			btAdd.Text = "Add (" + rowKinds[lastRowTypeId] + ")";
			for(int i = 0; i < rowKinds.Length; i++) {
				MenuItem menuItem = new MenuItem();
				menuItem.Index = i;
				menuItem.Text = rowKinds[i];
				menuItem.OwnerDraw = true;
				menuItem.DrawItem += new DrawItemEventHandler(DrawMenuItem);
				menuItem.MeasureItem += new MeasureItemEventHandler(MeasureMenuItem);
				menuItem.Click += new EventHandler(ClickMenuItem);
				contextMenu1.MenuItems.Add(menuItem);
			}
		}
		public override void InitComponent() {
			FillData();
			SetSelectItem(0);
			EditingVGrid.RowChanged += new DevExpress.XtraVerticalGrid.Events.RowChangedEventHandler(row_Changed);
			btnFields.Enabled = EditingVGrid is VGridControl && ((VGridControl)EditingVGrid).DataSource != null;
			XtraTabControl tc = DevExpress.XtraEditors.Design.FramesUtils.CreateTabProperty(this, new Control[] {pgMain, pnlProperties}, new string[] {"Row", "Row properties"});
			tc.SelectedPageChanged += new TabPageChangedEventHandler(changeTabPage);
			OnChangeRows();
		}
		#endregion
		#region Row Properties
		string CaptionByMultiEditorRowProperties(MultiEditorRowProperties p, int i) {
			string ret = p.Caption;
			if(ret == "") ret = p.FieldName;
			if(ret == "") ret = "Row" + (i+1).ToString();
			return ret;
		} 
		DevExpress.Utils.Frames.PropertyGridEx PropertyGridByMultiEditorRowProperties(MultiEditorRowProperties p) {
			DevExpress.Utils.Frames.PropertyGridEx pgProperties = new DevExpress.Utils.Frames.PropertyGridEx();
			pgProperties.BackColor = SystemColors.Control;
			pgProperties.Dock = DockStyle.Fill;
			pgProperties.CommandsVisibleIfAvailable = false;
			pgProperties.HelpVisible = true;
			pgProperties.DrawFlat = true;
			pgProperties.SelectedObject = p;
			pgProperties.PropertyValueChanged += new PropertyValueChangedEventHandler(pg_PropertyValueChanged);
			pgProperties.Site = p.Row.Site;
			return pgProperties;
		}
		void pg_PropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			XtraTabPage tp = ((Control)sender).Parent as XtraTabPage;
			MultiEditorRowProperties p = ((PropertyGrid)sender).SelectedObject as MultiEditorRowProperties;
			tp.Text = CaptionByMultiEditorRowProperties(p, ((XtraTabControl)tp.Parent).TabPages.IndexOf(tp));
		}
		void ClearPanelProperties() {
			if(pnlProperties.Controls.Count > 1) {
				object obj = pnlProperties.Controls[0];
				Control pnl = pnlProperties.Controls[1];
				if(pnl is Panel) {
					SimpleButton sbAdd = pnl.Controls[0] as SimpleButton;
					SimpleButton sbDelete = pnl.Controls[1] as SimpleButton;
					sbAdd.Click -= new EventHandler(add_Click);
					sbDelete.Click -= new EventHandler(delete_Click);
					pnl.Controls.Clear();
				}
				if(obj is XtraTabControl) {
					XtraTabControl tc = obj as XtraTabControl;
					foreach(XtraTabPage tp in tc.TabPages) {
						PropertyGrid pg = tp.Controls[0] as PropertyGrid;
						pg.PropertyValueChanged -= new PropertyValueChangedEventHandler(pg_PropertyValueChanged);	
					}
					tc.TabPages.Clear();
				}
			}
			pnlProperties.Controls.Clear();
		}
		void AddButtonsToPanel(Panel pnl, MultiEditorRow row) {
			int width = 100;
			int height = 22;
			SimpleButton sbAdd = new SimpleButton();
			sbAdd.Location = new Point(0, 3);
			sbAdd.Size = new Size(width, height);
			sbAdd.Text = "Add row";
			pnl.Controls.Add(sbAdd);
			SimpleButton sbDelete = new SimpleButton();
			sbDelete.Location = new Point(width + 5, 3);
			sbDelete.Size = new Size(width, height);
			sbDelete.Text = "Remove row";
			pnl.Controls.Add(sbDelete);
			sbDelete.Enabled = row.RowPropertiesCount > 0;
			sbAdd.Tag = sbDelete.Tag = row;
			sbAdd.Click += new EventHandler(add_Click);
			sbDelete.Click += new EventHandler(delete_Click);
		}
		XtraTabControl TabControlByPanel(Panel pnl) {
			if(pnl.Controls.Count > 0 && pnl.Controls[0] is XtraTabControl)
				return pnl.Controls[0] as XtraTabControl;
			return null;
		}
		void add_Click(object sender, EventArgs e) {
			SimpleButton btn = sender as SimpleButton;
			MultiEditorRow row = btn.Tag as MultiEditorRow;
			XtraTabControl tc = TabControlByPanel(pnlProperties);
			if(tc != null) {
				MultiEditorRowProperties p = row.PropertiesCollection.Add();
				XtraTabPage tp = new XtraTabPage();
				tp.Controls.Add(PropertyGridByMultiEditorRowProperties(p));
				tc.TabPages.Add(tp);
				tp.Text = CaptionByMultiEditorRowProperties(p, tc.TabPages.IndexOf(tp));
				tc.SelectedTabPage = tp;
				pnlProperties.Controls[1].Controls[1].Enabled = true; 
			}
		}
		void delete_Click(object sender, EventArgs e) {
			SimpleButton btn = sender as SimpleButton;
			MultiEditorRow row = btn.Tag as MultiEditorRow;
			XtraTabControl tc = TabControlByPanel(pnlProperties);
			if(tc != null && tc.SelectedTabPageIndex > -1) {
				int i = tc.SelectedTabPageIndex;
				tc.TabPages.Remove(tc.SelectedTabPage);
				row.PropertiesCollection.RemoveAt(i);
			}
			btn.Enabled = row.RowPropertiesCount > 0;
		}
		private void InitPGProperies(object obj) {
			if(tabIndex != 1) return;
			pnlProperties.Visible = false;
			pnlProperties.SuspendLayout();
			ClearPanelProperties();
			if(obj is MultiEditorRow) {
				MultiEditorRow row = obj as MultiEditorRow;
				Panel pnl = new Panel();
				pnl.Dock = DockStyle.Top;
				pnl.Height = 28;
				AddButtonsToPanel(pnl, row);
				XtraTabControl tc = new XtraTabControl();
				tc.Dock = DockStyle.Fill;
				pnlProperties.Controls.Add(tc);
				pnlProperties.Controls.Add(pnl);
				for(int i = 0; i < row.RowPropertiesCount; i++) {
					XtraTabPage tp = new XtraTabPage();
					tc.TabPages.Add(tp);
					tp.Text = CaptionByMultiEditorRowProperties(row.PropertiesCollection[i], tc.TabPages.IndexOf(tp));
					tp.Controls.Add(PropertyGridByMultiEditorRowProperties(row.PropertiesCollection[i]));
				}
			}
			else {
				DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx pgProperties = new DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx();
				pgProperties.BackColor = SystemColors.Control;
				pgProperties.Dock = DockStyle.Fill;
				pgProperties.CommandsVisibleIfAvailable = false;
				pgProperties.HelpVisible = true;
				pgProperties.DrawFlat = true;
				pnlProperties.Controls.Add(pgProperties);
				if(obj == null)
					pgProperties.SelectedObject = null;
				else  {
					pgProperties.SelectedObject = ((BaseRow)obj).Properties;
					pgProperties.Site = ((BaseRow)obj).Site;
				}
			}
			pnlProperties.ResumeLayout();
			pnlProperties.Visible = true;
		}
		#endregion
		#region Context Menu
		private void DrawMenuItem(object sender, DrawItemEventArgs e) {
			MenuItem mi = sender as MenuItem;
			SolidBrush bf = new SolidBrush(e.ForeColor);
			Rectangle rect = new Rectangle(
				e.Bounds.Left + SystemInformation.MenuButtonSize.Width + 4,
				e.Bounds.Top,
				e.Bounds.Width - SystemInformation.MenuButtonSize.Width - 4,
				e.Bounds.Height);
			Graphics g = e.Graphics;
			if((e.State & DrawItemState.Selected) != 0)
				e.DrawBackground();
			else
				g.FillRectangle(SystemBrushes.Menu, e.Bounds);
			StringFormat strFormat = new StringFormat();
			strFormat.LineAlignment = StringAlignment.Center;
			g.DrawString(mi.Text, e.Font, new SolidBrush(e.ForeColor), rect, strFormat);
			if(e.Index < imageList1.Images.Count)
				imageList1.Draw(g, e.Bounds.Left, e.Bounds.Top + 2, e.Index);
			e.DrawFocusRectangle();
		}
		private void MeasureMenuItem(object sender, MeasureItemEventArgs e) {
			MenuItem mi = sender as MenuItem;
			if(mi == null)
				return;
			SizeF strSize = e.Graphics.MeasureString(mi.Text, SystemInformation.MenuFont);
			e.ItemWidth = Math.Max((int)strSize.Width + SystemInformation.MenuButtonSize.Width + 4, btAdd.Width);
			e.ItemHeight = Math.Max(Math.Max((int)strSize.Height, imageList1.ImageSize.Height) + 2, SystemInformation.MenuButtonSize.Height + 2);
		}
		private void ClickMenuItem(object sender, EventArgs e) {
			MenuItem mi = sender as MenuItem;
			lastRowTypeId = mi.Index;
			btAdd.Text = "&Add (" + mi.Text + ")";
			add_Row(mi.Index);
		}
		#endregion
		#region Buttons
		private void btAddM_Click(object sender, System.EventArgs e) { 
			contextMenu1.Show(btAdd, new Point(0, btAdd.Size.Height)); 
		}
		private void btAdd_Click(object sender, System.EventArgs e) {
			add_Row(lastRowTypeId);
		}
		Cursor curentCursor = Cursors.Default;
		protected virtual void SetCurrentCursor() {
			Cursor.Current = curentCursor;
		}
		protected virtual void SetWaitCursor() {
			curentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
		}
		protected virtual void OnChangeRows() {
			sbClearAll.Enabled = EditingVGrid.Rows.Count > 0;
		}
		private void btRemove_Click(object sender, System.EventArgs e) {
			SetWaitCursor();
#if DXWhidbey
			UndoEngine u = (UndoEngine)((IDesignerHost)EditingVGrid.Site.Container).GetService(typeof(UndoEngine));
			u.Enabled = false;
#endif
			try {
				if(treeView1.SelectedNode != null) {
					TreeNode node = treeView1.SelectedNode.PrevNode;
					if(node == null) node = treeView1.SelectedNode.Parent;
					treeView1.BeginUpdate();
					EditingVGrid.BeginUpdate();
					((BaseRow)treeView1.SelectedNode.Tag).Dispose();
					treeView1.SelectedNode.Remove();
					SetSelectItem(node, null);
					EditingVGrid.EndUpdate();
					treeView1.EndUpdate();
					if(treeView1.Nodes.Count == 0) {
						pgMain.SelectedObject = null;
					}
				}
			}
			finally {
#if DXWhidbey
				u.Enabled = true;
#endif
				SetCurrentCursor();
			}
			OnChangeRows();
			FireChanged();
		}
		protected void RetrieveFields() {
			if(EditingVGrid.Rows.Count > 0)
				if(MessageBox.Show(EditingVGrid is VGridControl ? "The collection will be cleared and then populated with entries for each field in the bound data source. Do you want to continue?" : "The collection will be cleared and then populated with entries for each field in the bound object. Do you want to continue?", "Row Designer", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes) return;
			SetWaitCursor();
#if DXWhidbey
			UndoEngine u = (UndoEngine)((IDesignerHost)EditingVGrid.Site.Container).GetService(typeof(UndoEngine));
			u.Enabled = false;
#endif
			EditingVGrid.BeginUpdate();
			try {
				treeView1.BeginUpdate();
				treeView1.Nodes.Clear();
				EditingVGrid.Rows.Clear();
				EditingVGrid.RetrieveFields();
				FillData();
				SetSelectItem(0);
				treeView1.EndUpdate();
			}
			finally {
				EditingVGrid.EndUpdate();
#if DXWhidbey
				u.Enabled = true;
#endif
				SetCurrentCursor();
			}
		}
		private void btnFields_Click(object sender, System.EventArgs e) {
			RetrieveFields();
		}
		private void sbClearAll_Click(object sender, System.EventArgs e) {
			IUIService srv = (IUIService)EditingVGrid.Site.GetService(typeof(IUIService));
			if(srv != null && srv.ShowMessage("Do you want to clear the row collection?", "Row Designer", MessageBoxButtons.YesNo) != DialogResult.Yes)
				return;
			SetWaitCursor();
			EditingVGrid.BeginUpdate();
#if DXWhidbey
			UndoEngine u = (UndoEngine)((IDesignerHost)EditingVGrid.Site.Container).GetService(typeof(UndoEngine));
			u.Enabled = false;
#endif
			try {
				treeView1.BeginUpdate();
				treeView1.Nodes.Clear();
				EditingVGrid.Rows.Clear();
				SetSelectItem();
				OnChangeRows();
			}
			finally {
				treeView1.EndUpdate();
#if DXWhidbey
				u.Enabled = true;
#endif
				EditingVGrid.EndUpdate();
				SetCurrentCursor();
			}
			pgMain.SelectedObject = null;
		}
		#endregion	
		#region Editing
		private void add_Row(int rowTypeID) {
			BaseRow row = EditingVGrid.CreateRow(rowTypeID);
			treeView1.BeginUpdate();
			EditingVGrid.Rows.Add(row);
			AddRowView(row, null);
			SetSelectItem(treeView1.Nodes.Count - 1);
			treeView1.EndUpdate();
			OnChangeRows();
			FireChanged();
		}
		private void SetSelectItem(TreeNode node, TreeNodeCollection col) {
			if(node != null) {
				if(col == null) col = treeView1.Nodes;
				for(int i = 0; i < col.Count; i++) {
					if(node.Text == col[i].Text) {
						treeView1.SelectedNode = col[i];
						SetSelectItem();
						break;
					}
					SetSelectItem(node, col[i].Nodes);
				}
			}
			if(treeView1.SelectedNode == null)
				SetSelectItem(0);
		}
		private void SetSelectItem(int i) {
			if(treeView1.Nodes.Count > 0) {
				try {
					treeView1.SelectedNode = treeView1.Nodes[i];
#if DXWhidbey
					treeView1.TopNode = treeView1.SelectedNode;
#endif
				} catch {}
			}
			SetSelectItem();
		}
		protected void SetSelectItem() {
			btRemove.Enabled = treeView1.SelectedNode != null;
		}
		private void row_Changed(object sender, DevExpress.XtraVerticalGrid.Events.RowChangedEventArgs e) {
			if(treeView1.SelectedNode != null)
				treeView1.SelectedNode.Text = ((BaseRow)treeView1.SelectedNode.Tag).Name;
			if(e.ChangeType == RowChangeTypeEnum.Expanded) {
				PopulateNode(e.Row);
			}
		}
		void PopulateNode(BaseRow row) {
			BaseRow parentRow = row.ParentRow;
			if(parentRow == null)
				return;
			TreeNode node = FindNode(row);
			if(node == null || node.Nodes.Count == row.ChildRows.Count)
				return;
			this.treeView1.BeginUpdate();
			int nodeIndex = node.Index;
			TreeNode parentNode = FindNode(parentRow);
			parentNode.Nodes.RemoveAt(nodeIndex);
			AddRowView(row, parentNode);
			TreeNode expandedNode = FindNode(row);
			expandedNode.ExpandAll();
			MoveNode(parentNode.Nodes, expandedNode, nodeIndex);
			this.treeView1.EndUpdate();
			this.treeView1.TopNode = expandedNode;
			this.treeView1.SelectedNode = expandedNode;
		}
		void MoveNode(TreeNodeCollection nodes, TreeNode node, int index) {
			nodes.Remove(node);
			nodes.Insert(index, node);
		}
		TreeNode FindNode(BaseRow row) {
			BaseRow parentRow = row.ParentRow;
			if(parentRow == null) {
				return FindNode(row, this.treeView1.Nodes);
			} else {
				TreeNode parentNode = FindNode(parentRow);
				return FindNode(row, parentNode.Nodes);
			}
		}
		TreeNode FindNode(BaseRow row, TreeNodeCollection nodes) {
			foreach(TreeNode node in nodes) {
				if(node.Tag == row)
					return node;
			}
			return null;
		}
		private void DeInit() {
			EditingVGrid.RowChanged -= new DevExpress.XtraVerticalGrid.Events.RowChangedEventHandler(row_Changed);
		}
		protected override void OnPropertyGridSelectedObjectChanged(object sender, EventArgs e) {
			UpdatePropertyGridSite();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) DeInit();
			base.Dispose(disposing);
		}
		protected virtual void FillData() {
			treeView1.Nodes.Clear();
			for(int i = 0; i < EditingVGrid.Rows.Count; i++)
				AddRowView(EditingVGrid.Rows[i], null);
			treeView1.ExpandAll();
		}
		private int IndexByType(BaseRow row) {
			string type = row.GetType().ToString();
			for(int i = rowKinds.Length - 1; i >= 0; i--)
				if(type.IndexOf(rowKinds[i]) > -1) return i;
			return -1;
		}
		private void AddRowView(BaseRow row, TreeNode parent) {
			int ind = IndexByType(row);
			TreeNode node = new TreeNode(row.Name, ind, ind);
			node.Tag = row;
			if(parent == null)
				treeView1.Nodes.Add(node);
			else parent.Nodes.Add(node);
			for(int i = 0; i < row.ChildRows.Count; i++)
				AddRowView(row.ChildRows[i], node);
		}
		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e) {
			pgMain.SelectedObject = e.Node.Tag;
			InitPGProperies(e.Node.Tag);
		}
		private void treeView1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Delete:
					btRemove_Click(this, new EventArgs());
					e.Handled = true;
					break;
				case Keys.Insert:
					btAdd_Click(this, new EventArgs());
					break;
			}
		}
		#endregion
		#region DragDrop
		TreeNode clickedNode = null;
		Point clickedPoint = Point.Empty;
		private void treeView1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			TreeView tv = sender as TreeView;
			TreeNode node = tv.GetNodeAt(clickedPoint = new Point(e.X, e.Y));
			tv.SelectedNode = node;
			tv.Select();
			if(node != null) { 
				clickedNode = node;
			}
			else clickedNode = null;
		}
		private void treeView1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			TreeView tv = sender as TreeView;
			if(clickedNode == null) return;
			if(e.Button != MouseButtons.Left) return;
			if((Math.Abs(e.X - clickedPoint.X) > 5 || Math.Abs(e.Y - clickedPoint.Y) > 5))  
				tv.DoDragDrop(clickedNode, DragDropEffects.Move | DragDropEffects.Copy);
		}
		private TreeNode GetTreeNode(IDataObject data) {
			return data.GetData(typeof(TreeNode)) as TreeNode;
		}
		private bool IsParent(TreeNode dragNode, TreeNode node) {
			while(node != null) {
				if(node == dragNode) return true;
				node = node.Parent;
			}
			return false;
		} 
		TreeNode selectedNode = null;
		int selectedIndex = -1;
		VGridDragDropItem dragDropItem;
		private void treeView1_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			TreeView tv = sender as TreeView;
			TreeNode item = GetTreeNode(e.Data);
			if(item == null) return;
			TreeNode node =	tv.GetNodeAt(tv.PointToClient(new Point(e.X, e.Y)));
			bool empty = false;
			if(node == null) {
				empty = tv.Bounds.Contains(tv.PointToClient(new Point(e.X, e.Y)));
				if(empty) node = tv.Nodes[tv.Nodes.Count - 1];
			}
			if(selectedNode == null || selectedNode != node) {
				ClearSelectedNode();
				selectedNode = node;
				if(node != null) selectedIndex = node.ImageIndex;
			}
			if(empty) {
				if(selectedNode.ImageIndex != 5) selectedNode.ImageIndex = 5;
				dragDropItem = new VGridDragDropItem(item, selectedNode, DragDropOperation.Add);
				if(item != node)
					e.Effect = DragDropEffects.Move;
				else e.Effect = DragDropEffects.None;
			} else {
				if(!IsParent(item, node)) {
					if((e.KeyState & 4) != 0) { 
						if(selectedNode.Tag is MultiEditorRow && item.Tag is EditorRow) {
							if(selectedNode.ImageIndex != 6) selectedNode.ImageIndex = 6;
							dragDropItem = new VGridDragDropItem(item, selectedNode, DragDropOperation.Add);
							e.Effect = DragDropEffects.Copy;
							return;
						}
					}
					if((e.KeyState & 8) != 0) {
						if(selectedNode.ImageIndex != 4) selectedNode.ImageIndex = 4;
						dragDropItem = new VGridDragDropItem(item, selectedNode, DragDropOperation.Insert);
					}
					else {
						if(selectedNode.ImageIndex != 3) selectedNode.ImageIndex = 3;
						dragDropItem = new VGridDragDropItem(item, selectedNode, DragDropOperation.AddChild);
					}
					e.Effect = DragDropEffects.Move;
				}
				else {
					e.Effect = DragDropEffects.None;
					dragDropItem = null;
				}
			}
		}
		private void ClearSelectedNode() {
			if(selectedNode != null)
				selectedNode.ImageIndex = selectedIndex;
		}
		private void AddNodeToTreeView(TreeNode parentNode, int index, TreeNode aNode, TreeView tv) {
			TreeNode node = aNode.Clone() as TreeNode;
			if(parentNode == null) {
				if(index == -1)
					tv.Nodes.Add(node);
				else
					tv.Nodes.Insert(index, node);
			} else { 
				if(index == -1)
					parentNode.Nodes.Add(node);
				else
					parentNode.Nodes.Insert(index, node);
			}
			node.ExpandAll();
			tv.SelectedNode = node;
			tv.Select();
		}
		private void treeView1_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			ClearSelectedNode();
			if(dragDropItem == null) return;
			TreeView tv = sender as TreeView;
			tv.BeginUpdate();
			if(e.Effect == DragDropEffects.Copy) {
				MultiEditorRow mRow = null; 
				EditorRow row = null;
				if(dragDropItem.DestItem.Tag is MultiEditorRow)
					mRow = dragDropItem.DestItem.Tag as MultiEditorRow;
				if(dragDropItem.DragItem.Tag is EditorRow)
					row = dragDropItem.DragItem.Tag as EditorRow;
				if(mRow != null && row != null) {
					MultiEditorRowProperties p = mRow.PropertiesCollection.Add();
					row.Properties.AssignTo(p);
					EditingVGrid.LayoutChanged();
				}
			} else {
				switch(dragDropItem.Operation) {
					case DragDropOperation.Insert:
						EditingVGrid.MoveRow(dragDropItem.DragItem.Tag as BaseRow, dragDropItem.DestItem.Tag  as BaseRow, true);
						AddNodeToTreeView(dragDropItem.DestItem.Parent, dragDropItem.DestItem.Index, dragDropItem.DragItem, treeView1); 
						dragDropItem.DragItem.Remove();
						break;
					case DragDropOperation.AddChild:
						EditingVGrid.MoveRow(dragDropItem.DragItem.Tag as BaseRow, dragDropItem.DestItem.Tag as BaseRow, false);
						AddNodeToTreeView(dragDropItem.DestItem, -1, dragDropItem.DragItem, treeView1); 
						dragDropItem.DragItem.Remove();
						break;
					case DragDropOperation.Add:
						EditingVGrid.MoveRow(dragDropItem.DragItem.Tag as BaseRow, null, false);
						AddNodeToTreeView(null, -1, dragDropItem.DragItem, treeView1); 
						dragDropItem.DragItem.Remove();
						break;
				}
			}
			tv.EndUpdate();
		}
		private void treeView1_DragLeave(object sender, System.EventArgs e) {
			ClearSelectedNode();
		}
		#endregion
		#region Row options
		private int tabIndex = 0;
		private void changeTabPage(object sender, TabPageChangedEventArgs e) {
			XtraTabControl tc = sender as XtraTabControl;
			tabIndex = tc.SelectedTabPageIndex;
			if(tabIndex == 0) pgMain.Refresh();
			if(tabIndex == 1) InitPGProperies(pgMain.SelectedObject);
		}
		private void FireChanged() {
			IComponentChangeService srv = EditingVGrid.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(EditingVGrid, null, null, null);
		}
		#endregion
	}
}
