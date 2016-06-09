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
using System.Data;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.XtraVerticalGrid.Rows;
namespace DevExpress.XtraVerticalGrid.Frames {
	[ToolboxItem(false)]
	public class DefaultEditorsDesigner : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		private DevExpress.XtraEditors.SimpleButton btRemove;
		private DevExpress.XtraEditors.SimpleButton btAdd;
		private System.Windows.Forms.Panel pnlProperty;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private DevExpress.XtraEditors.ListBoxControl ItemList;
		private System.ComponentModel.Container components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.btRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btAdd = new DevExpress.XtraEditors.SimpleButton();
			this.ItemList = new DevExpress.XtraEditors.ListBoxControl();
			this.pnlProperty = new System.Windows.Forms.Panel();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ItemList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(160, 100);
			this.splMain.Size = new System.Drawing.Size(5, 236);
			this.pgMain.Location = new System.Drawing.Point(165, 100);
			this.pgMain.Size = new System.Drawing.Size(487, 236);
			this.pnlControl.Controls.Add(this.btRemove);
			this.pnlControl.Controls.Add(this.btAdd);
			this.pnlControl.Size = new System.Drawing.Size(652, 54);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.lbCaption.Size = new System.Drawing.Size(652, 42);
			this.pnlMain.Controls.Add(this.groupControl1);
			this.pnlMain.Controls.Add(this.pnlProperty);
			this.pnlMain.Location = new System.Drawing.Point(0, 100);
			this.pnlMain.Size = new System.Drawing.Size(160, 236);
			this.horzSplitter.Size = new System.Drawing.Size(652, 4);
			this.horzSplitter.Visible = false;
			this.btRemove.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btRemove.Location = new System.Drawing.Point(36, 4);
			this.btRemove.Name = "btRemove";
			this.btRemove.Size = new System.Drawing.Size(30, 30);
			this.btRemove.TabIndex = 1;
			this.btRemove.ToolTip = "Remove";
			this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
			this.btAdd.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btAdd.Location = new System.Drawing.Point(0, 4);
			this.btAdd.Name = "btAdd";
			this.btAdd.Size = new System.Drawing.Size(30, 30);
			this.btAdd.TabIndex = 0;
			this.btAdd.ToolTip = "Add";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			this.ItemList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.ItemList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ItemList.ItemHeight = 16;
			this.ItemList.Location = new System.Drawing.Point(4, 23);
			this.ItemList.Name = "ItemList";
			this.ItemList.Size = new System.Drawing.Size(140, 209);
			this.ItemList.TabIndex = 3;
			this.ItemList.SelectedIndexChanged += new System.EventHandler(this.SumItemList_SelectedIndexChanged);
			this.ItemList.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.ItemList_DrawItem);
			this.pnlProperty.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlProperty.Location = new System.Drawing.Point(148, 0);
			this.pnlProperty.Name = "pnlProperty";
			this.pnlProperty.Size = new System.Drawing.Size(12, 236);
			this.pnlProperty.TabIndex = 0;
			this.pnlProperty.Visible = false;
			this.groupControl1.Controls.Add(this.ItemList);
			this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupControl1.Location = new System.Drawing.Point(0, 0);
			this.groupControl1.Name = "groupControl1";
			this.groupControl1.Padding = new System.Windows.Forms.Padding(2);
			this.groupControl1.Size = new System.Drawing.Size(148, 236);
			this.groupControl1.TabIndex = 4;
			this.groupControl1.Text = "Default Editor Items";
			this.Name = "DefaultEditorsDesigner";
			this.Size = new System.Drawing.Size(652, 336);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ItemList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		protected override void InitImages() {
			base.InitImages();
			btAdd.Image = DesignerImages16.Images[DesignerImages16PlusIndex];
			btRemove.Image = DesignerImages16.Images[DesignerImages16MinusIndex];
		}
		protected override string DescriptionText { get { return "Manage the editors used for in-place editing within the cells by default (add, remove, modify their settings). A single editor must be associated with a single data type."; } }
		public DefaultEditorsDesigner() : base(5) {
			InitializeComponent();
			pgMain.BringToFront();
		}
		protected override void Dispose(bool disposing) {
			if(components != null) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		public override void InitComponent() {
			FillData();
		}
		void FillData() {
			if(EditingGrid == null) return;
			foreach(DefaultEditor editor in EditingGrid.DefaultEditors) 
				ItemList.Items.Add(new DefaultEditorItem(editor));
			if(ItemList.Items.Count > 0) ItemList.SelectedIndex = 0;
			EnabledButtons();
		}
		public PropertyGridControl EditingGrid { get { return EditingObject as PropertyGridControl; } }
		#endregion
		#region Editing
		void EnabledButtons() {
			btRemove.Enabled = ItemList.Items.Count > 0;
		}
		private void SumItemList_SelectedIndexChanged(object sender, System.EventArgs e) {
			DefaultEditorItem item = ItemList.SelectedItem as DefaultEditorItem;
			if(item != null)
				pgMain.SelectedObject = item.Editor;
			else pgMain.SelectedObject = null;
			EnabledButtons();
		}
		private void btAdd_Click(object sender, System.EventArgs e) {
			if(EditingGrid == null) return;
			DefaultEditor editor = new DefaultEditor();
			EditingGrid.DefaultEditors.Add(editor);
			ItemList.SelectedIndex = ItemList.Items.Add(new DefaultEditorItem(editor));
		}
		private void btRemove_Click(object sender, System.EventArgs e) {
			if(EditingGrid == null || ItemList.SelectedItem == null) return;
			DefaultEditorItem item = ItemList.SelectedItem as DefaultEditorItem;
			DefaultEditor editor = item.Editor;
			ItemList.Items.Remove(item);
			EditingGrid.DefaultEditors.Remove(editor);
		}
		private void FireChanged() {
			if(EditingGrid == null) return;
			IComponentChangeService srv = EditingGrid.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(EditingGrid, null, null, null);
		}
		protected override void OnPropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			base.OnPropertyGridPropertyValueChanged(sender, e);
			FireChanged();
			ItemList.Refresh();
		}
		#endregion
		private void ItemList_DrawItem(object sender, DevExpress.XtraEditors.ListBoxDrawItemEventArgs e) {
			DefaultEditorItem item = e.Item as DefaultEditorItem;
			if(item.Editor.Edit == null || item.Editor.EditingType == null)
				e.Appearance.ForeColor = e.Item == ItemList.SelectedItem ? SystemColors.Control : SystemColors.ControlDark;
			if(WrongType(e.Index, item.Editor.EditingType))
				e.Appearance.ForeColor = e.Item == ItemList.SelectedItem ? Color.LightPink : Color.Red;
		}
		bool WrongType(int index, Type type) {
			if(type == null) return false;
			for(int i = 0; i < index; i++)
				if(((DefaultEditorItem)ItemList.Items[i]).Editor.EditingType == type) return true;
			return false;
		}
		class DefaultEditorItem {
			DefaultEditor editor;
			public DefaultEditorItem(DefaultEditor editor) {
				this.editor = editor;
			}
			public DefaultEditor Editor { get { return editor; } }
			public override string ToString() {
				if(editor.Edit == null && editor.EditingType == null)
					return "<empty>";
				if(editor.EditingType == null)
					return string.Format("{0} - <empty type>", editor.Edit.Name);
				if(editor.Edit == null)
					return string.Format("<empty editor> - {0}", editor.EditingType);
				return string.Format("{0} - {1}", editor.Edit.Name, editor.EditingType);
			}
		}
	}
}
