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
using DevExpress.XtraBars;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Docking.Design.Frames {
	[ToolboxItem(false)] 
	public class DockWindowsFrame : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		private DevExpress.XtraEditors.ListBoxControl listBoxControl1;
		private DevExpress.XtraEditors.SimpleButton btnDelete;
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private System.ComponentModel.IContainer components = null;
		public DockWindowsFrame() {
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
			this.listBoxControl1 = new DevExpress.XtraEditors.ListBoxControl();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			this.pnlControl.Controls.Add(this.btnDelete);
			this.pnlControl.Controls.Add(this.btnAdd);
			this.pnlControl.Size = new System.Drawing.Size(423, 54);
			this.pnlControl.TabIndex = 1;
			this.pgMain.Location = new System.Drawing.Point(165, 96);
			this.pgMain.Size = new System.Drawing.Size(258, 192);
			this.splMain.Location = new System.Drawing.Point(160, 96);
			this.splMain.Size = new System.Drawing.Size(5, 192);
			this.lbCaption.Size = new System.Drawing.Size(423, 42);
			this.pnlMain.Controls.Add(this.groupControl1);
			this.pnlMain.Location = new System.Drawing.Point(0, 96);
			this.pnlMain.Margin = new System.Windows.Forms.Padding(0);
			this.pnlMain.Size = new System.Drawing.Size(160, 192);
			this.horzSplitter.Size = new System.Drawing.Size(423, 4);
			this.horzSplitter.Visible = false;
			this.listBoxControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.listBoxControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listBoxControl1.ItemHeight = 16;
			this.listBoxControl1.Location = new System.Drawing.Point(4, 23);
			this.listBoxControl1.Name = "listBoxControl1";
			this.listBoxControl1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.listBoxControl1.Size = new System.Drawing.Size(152, 165);
			this.listBoxControl1.TabIndex = 0;
			this.listBoxControl1.SelectedIndexChanged += new System.EventHandler(this.listBoxControl1_SelectedIndexChanged);
			this.listBoxControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBoxControl1_KeyDown);
			this.btnDelete.Enabled = false;
			this.btnDelete.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnDelete.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDelete.Location = new System.Drawing.Point(36, 4);
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.Size = new System.Drawing.Size(30, 30);
			this.btnDelete.TabIndex = 3;
			this.btnDelete.ToolTip = "Delete DockPanel(s)";
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			this.btnAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAdd.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnAdd.Location = new System.Drawing.Point(0, 4);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(30, 30);
			this.btnAdd.TabIndex = 2;
			this.btnAdd.ToolTip = "Add New DockPanel";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.groupControl1.Controls.Add(this.listBoxControl1);
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.Location = new System.Drawing.Point(0, 0);
			this.groupControl1.Margin = new System.Windows.Forms.Padding(0);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Padding = new System.Windows.Forms.Padding(2);
			this.groupControl1.Size = new System.Drawing.Size(160, 192);
			this.groupControl1.TabIndex = 1;
			this.groupControl1.Text = "Dock Panels:";
			this.Name = "DockWindowsFrame";
			this.Size = new System.Drawing.Size(423, 288);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.listBoxControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected override void InitImages() {
			base.InitImages();
			btnAdd.Image = DesignerImages16.Images[DesignerImages16AddIndex];
			btnDelete.Image = DesignerImages16.Images[DesignerImages16RemoveIndex];
		}
		[Browsable(false)]
		public DockManager Manager {
			get {
				BarManager manager = EditingObject as BarManager;
				if(manager != null) return manager.DockManager;
				return EditingObject as DockManager;
			}
		}
		protected override string DescriptionText { get { return "You can add/delete and customize dock panels."; } }
		public ListBoxControl ListBox { get { return listBoxControl1; }}
		public override void DoInitFrame() {
			Populate();
		}
		private void Populate() {
			if(Manager == null) return;
			ListBox.Items.Clear();
			foreach(DockPanel window in Manager.Panels) {
				AddDockWindow(ListBox, window);
			}
			if(ListBox.Items.Count > 0) ListBox.SelectedIndex = 0;
		}
		private void AddDockWindow(ListBoxControl lBox, DockPanel window) {
			if(window.Count > 0) return;
			lBox.Items.Add(window.Name);
		}
		protected override void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			base.pgMain_PropertyValueChanged(s, e);
			if(ListBox.SelectedItem == null) return;
			DockPanel panel = Manager[ListBox.SelectedItem.ToString()];
			if(panel == null) 
				ListBox.Items[ListBox.SelectedIndex] = ((DockPanel)pgMain.SelectedObject).Name;
		}
		private object[] SelectedItems(ListBoxControl lBox) {
			ArrayList ret = new ArrayList();
			for(int i = 0; i < lBox.SelectedIndices.Count; i++) {
				DockPanel panel = Manager[lBox.Items[lBox.SelectedIndices[i]].ToString()];
				if(panel != null) ret.Add(panel);
			}
			return ret.ToArray(); 
		}
		private void SetDeleteEnabled(ListBoxControl lBox) {
			btnDelete.Enabled = SelectedItems(lBox).Length > 0;
		}
		private void listBoxControl1_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(lockListBox) return;
			ListBoxControl lBox = sender as ListBoxControl;
			pgMain.SelectedObjects = SelectedItems(lBox);
			SetDeleteEnabled(lBox);
		}
		private void AddDockWindow() {
			DockPanel window = Manager.AddPanel(DockingStyle.Float);
			if(window == null) return;
			AddDockWindow(ListBox, window);
			ListBox.SelectedItem = window.Name;
		}
		bool lockListBox = false;
		private void DeleteDockWindow() {
			object[] windows = SelectedItems(ListBox);
			foreach(object obj in windows) 
				if(obj is DockPanel) DeleteDockWindow(obj as DockPanel); 
			RefreshListBox(ListBox);
		}
		private void RefreshListBox(ListBoxControl lBox) {
			lockListBox = true;
			for(int i = lBox.Items.Count - 1; i >= 0; i--) {
				DockPanel window = Manager[lBox.Items[i].ToString()];	
				if(window == null) ListBox.Items.RemoveAt(i);
			}
			lockListBox = false;
			listBoxControl1_SelectedIndexChanged(ListBox, System.EventArgs.Empty);
		}
		private void DeleteDockWindow(DockPanel window) {
			Manager.RemovePanel(window);
		}
		private void btnAdd_Click(object sender, System.EventArgs e) {
			AddDockWindow();
		}
		private void btnDelete_Click(object sender, System.EventArgs e) {
			DeleteDockWindow();
		}
		private void listBoxControl1_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) DeleteDockWindow();
			if(e.KeyCode == Keys.Insert) AddDockWindow();
		}
	}
}
