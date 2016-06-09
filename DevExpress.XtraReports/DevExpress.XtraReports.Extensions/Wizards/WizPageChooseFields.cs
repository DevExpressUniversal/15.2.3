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

using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.XtraReports.Design {
	[ToolboxItem(false)]
	public class WizPageChooseFields : DevExpress.Utils.InteriorWizardPage {
		private DevExpress.XtraEditors.LabelControl label1;
		private DevExpress.XtraEditors.SimpleButton btnAddAll;
		private DevExpress.XtraEditors.SimpleButton btnRemoveAll;
		private DevExpress.XtraEditors.SimpleButton btnRemove;
		private DevExpress.XtraEditors.SimpleButton btnAdd;
		private System.Windows.Forms.ListView lvSelectedFields;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ListView lvAvailableFields;
		private System.Windows.Forms.ImageList imageList;
		private System.ComponentModel.IContainer components = null;
		private LabelControl label2;
		StandardReportWizard wizard;
		private LabelControl labelControl1;
		bool fillFieldsListOnActivate = true;
		public WizPageChooseFields(XRWizardRunnerBase runner)
			: this() {
			this.wizard = (StandardReportWizard)runner.Wizard;
		}
		WizPageChooseFields() {
			InitializeComponent();
			btnRemove.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveLeft.gif", typeof(LocalResFinder));
			btnRemoveAll.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveAllLeft.gif", typeof(LocalResFinder));
			btnAdd.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveRight.gif", typeof(LocalResFinder));
			btnAddAll.Image = ResourceImageHelper.CreateBitmapFromResources("Images.MoveAllRight.gif", typeof(LocalResFinder));
			PickManager.FillDataSourceImageList(imageList);
			headerPicture.Image = ResourceImageHelper.CreateBitmapFromResources("Images.WizTopFields.gif", typeof(LocalResFinder));
		}
		protected override void Dispose(bool disposing) {
			if (disposing && components != null)
				components.Dispose();
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizPageChooseFields));
			this.label1 = new DevExpress.XtraEditors.LabelControl();
			this.lvSelectedFields = new System.Windows.Forms.ListView();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnAddAll = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemoveAll = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.lvAvailableFields = new System.Windows.Forms.ListView();
			this.label2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.titleLabel, "titleLabel");
			resources.ApplyResources(this.subtitleLabel, "subtitleLabel");
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			this.lvSelectedFields.AllowDrop = true;
			this.lvSelectedFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnHeader2});
			this.lvSelectedFields.FullRowSelect = true;
			this.lvSelectedFields.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvSelectedFields.HideSelection = false;
			resources.ApplyResources(this.lvSelectedFields, "lvSelectedFields");
			this.lvSelectedFields.Name = "lvSelectedFields";
			this.lvSelectedFields.SmallImageList = this.imageList;
			this.lvSelectedFields.UseCompatibleStateImageBehavior = false;
			this.lvSelectedFields.View = System.Windows.Forms.View.Details;
			this.lvSelectedFields.Resize += new System.EventHandler(this.lvSelectedFields_Resize);
			this.lvSelectedFields.SelectedIndexChanged += new System.EventHandler(this.lvSelectedFields_SelectedIndexChanged);
			this.lvSelectedFields.DoubleClick += new System.EventHandler(this.lvSelectedFields_DoubleClick);
			this.lvSelectedFields.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvSelectedFields_DragDrop);
			this.lvSelectedFields.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvSelectedFields_DragEnter);
			this.lvSelectedFields.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvSelectedFields_ItemDrag);
			this.lvSelectedFields.DragOver += new System.Windows.Forms.DragEventHandler(this.lvSelectedFields_DragOver);
			resources.ApplyResources(this.columnHeader2, "columnHeader2");
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			resources.ApplyResources(this.imageList, "imageList");
			this.imageList.TransparentColor = System.Drawing.Color.Lime;
			this.btnAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.btnAddAll.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnAddAll, "btnAddAll");
			this.btnAddAll.Name = "btnAddAll";
			this.btnAddAll.Click += new System.EventHandler(this.btnAddAll_Click);
			this.btnRemoveAll.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnRemoveAll, "btnRemoveAll");
			this.btnRemoveAll.Name = "btnRemoveAll";
			this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
			this.btnRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			resources.ApplyResources(this.columnHeader1, "columnHeader1");
			this.lvAvailableFields.AllowDrop = true;
			this.lvAvailableFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
			this.columnHeader1});
			this.lvAvailableFields.FullRowSelect = true;
			this.lvAvailableFields.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvAvailableFields.HideSelection = false;
			resources.ApplyResources(this.lvAvailableFields, "lvAvailableFields");
			this.lvAvailableFields.Name = "lvAvailableFields";
			this.lvAvailableFields.SmallImageList = this.imageList;
			this.lvAvailableFields.UseCompatibleStateImageBehavior = false;
			this.lvAvailableFields.View = System.Windows.Forms.View.Details;
			this.lvAvailableFields.Resize += new System.EventHandler(this.lvAvailableFields_Resize);
			this.lvAvailableFields.SelectedIndexChanged += new System.EventHandler(this.lvAvailableFields_SelectedIndexChanged);
			this.lvAvailableFields.DoubleClick += new System.EventHandler(this.lvAvailableFields_DoubleClick);
			this.lvAvailableFields.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvAvailableFields_DragDrop);
			this.lvAvailableFields.DragEnter += new System.Windows.Forms.DragEventHandler(this.lvAvailableFields_DragEnter);
			this.lvAvailableFields.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.lvAvailableFields_ItemDrag);
			this.lvAvailableFields.DragOver += new System.Windows.Forms.DragEventHandler(this.lvAvailableFields_DragOver);
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.Name = "labelControl1";
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnRemoveAll);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAddAll);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.lvSelectedFields);
			this.Controls.Add(this.lvAvailableFields);
			this.Controls.Add(this.label1);
			this.Name = "WizPageChooseFields";
			this.Controls.SetChildIndex(this.label1, 0);
			this.Controls.SetChildIndex(this.lvAvailableFields, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			this.Controls.SetChildIndex(this.lvSelectedFields, 0);
			this.Controls.SetChildIndex(this.btnAdd, 0);
			this.Controls.SetChildIndex(this.btnAddAll, 0);
			this.Controls.SetChildIndex(this.btnRemove, 0);
			this.Controls.SetChildIndex(this.btnRemoveAll, 0);
			this.Controls.SetChildIndex(this.label2, 0);
			this.Controls.SetChildIndex(this.labelControl1, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		static void UpdateListViewColumnWidth(ListView lv) {
			lv.Columns[0].AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
		}
		void MoveItems(ListView from, ListView to, ArrayList items) {
			if (items.Count <= 0)
				return;
			int index = int.MaxValue;
			foreach (ListViewItem item in items) {
				if (item.Index < index)
					index = item.Index;
				from.Items.Remove(item);
				item.Selected = false;
				to.Items.Add(item);
			}
			if (index + 1 > from.Items.Count)
				index = from.Items.Count - 1;
			if (index >= 0)
				from.Items[index].Selected = true;
			UpdateListViewColumnWidth(from);
			UpdateListViewColumnWidth(to);
			UpdateButtons();
			from.Focus();
		}
		void MoveSelectedItems(ListView from, ListView to) {
			ArrayList items = new ArrayList();
			items.AddRange(from.SelectedItems);
			MoveItems(from, to, items);
		}
		void MoveAll(ListView from, ListView to) {
			ArrayList items = new ArrayList();
			items.AddRange(from.Items);
			MoveItems(from, to, items);
		}
		private void btnAdd_Click(object sender, System.EventArgs e) {
			MoveSelectedItems(lvAvailableFields, lvSelectedFields);
		}
		private void btnAddAll_Click(object sender, System.EventArgs e) {
			MoveAll(lvAvailableFields, lvSelectedFields);
		}
		private void btnRemove_Click(object sender, System.EventArgs e) {
			MoveSelectedItems(lvSelectedFields, lvAvailableFields);
		}
		private void btnRemoveAll_Click(object sender, System.EventArgs e) {
			MoveAll(lvSelectedFields, lvAvailableFields);
		}
		private void lvAvailableFields_DoubleClick(object sender, System.EventArgs e) {
			MoveSelectedItems(lvAvailableFields, lvSelectedFields);
		}
		private void lvSelectedFields_DoubleClick(object sender, System.EventArgs e) {
			MoveSelectedItems(lvSelectedFields, lvAvailableFields);
		}
		private void lvAvailableFields_Resize(object sender, System.EventArgs e) {
			UpdateListViewColumnWidth(lvAvailableFields);
		}
		private void lvSelectedFields_Resize(object sender, System.EventArgs e) {
			UpdateListViewColumnWidth(lvSelectedFields);
		}
		void HandleItemDrag(ListView from, ItemDragEventArgs e) {
			if(!e.Button.IsLeft())
				return;
			DoDragDrop(new DataObject(from), DragDropEffects.Move);
		}
		private void lvAvailableFields_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e) {
			HandleItemDrag(lvAvailableFields, e);
		}
		static void HandleDragOver(ListView from, DragEventArgs e) {
			if (from.Equals(e.Data.GetData(typeof(ListView))))
				e.Effect = e.AllowedEffect;
			else
				e.Effect = DragDropEffects.None;
		}
		void UpdateButtons() {
			bool enable = lvAvailableFields.Items.Count > 0;
			btnAdd.Enabled = enable && lvAvailableFields.SelectedItems.Count > 0;
			btnAddAll.Enabled = enable;
			enable = lvSelectedFields.Items.Count > 0;
			btnRemove.Enabled = enable && lvSelectedFields.SelectedItems.Count > 0;
			btnRemoveAll.Enabled = enable;
			UpdateWizardButtons();
		}
		private void lvSelectedFields_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleDragOver(lvAvailableFields, e);
		}
		private void lvSelectedFields_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleDragOver(lvAvailableFields, e);
		}
		private void lvSelectedFields_ItemDrag(object sender, System.Windows.Forms.ItemDragEventArgs e) {
			HandleItemDrag(lvSelectedFields, e);
		}
		private void lvAvailableFields_DragEnter(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleDragOver(lvSelectedFields, e);
		}
		private void lvAvailableFields_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			HandleDragOver(lvSelectedFields, e);
		}
		private void lvSelectedFields_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			MoveSelectedItems(lvAvailableFields, lvSelectedFields);
		}
		private void lvAvailableFields_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			MoveSelectedItems(lvSelectedFields, lvAvailableFields);
		}
		bool CanLeavePage() {
			if (lvSelectedFields.Items.Count > 0)
				return true;
			string msg = DevExpress.XtraReports.Localization.ReportLocalizer.GetString(DevExpress.XtraReports.Localization.ReportStringId.Wizard_PageChooseFields_Msg);
			XtraMessageBox.Show(DesignLookAndFeelHelper.GetLookAndFeel(wizard.DesignerHost), msg, "Report Wizard", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return false;
		}
		void ApplyChanges() {
			fillFieldsListOnActivate = false;
			wizard.SelectedFields.Clear();
			foreach (ListViewItem item in lvSelectedFields.Items)
				wizard.SelectedFields.Add(item.Tag as ObjectName);
		}
		protected override string OnWizardBack() {
			fillFieldsListOnActivate = true;
			return WizardForm.NextPage; 
		}
		protected override string OnWizardNext() {
			if (CanLeavePage()) {
				ApplyChanges();
				return WizardForm.NextPage;
			}
			else
				return WizardForm.NoPageChange;
		}
		protected override bool OnWizardFinish() {
			if (CanLeavePage()) {
				ApplyChanges();
				return true;
			}
			else
				return false;
		}
		void FillFieldLists() {
			wizard.FillFields();
			lvAvailableFields.BeginUpdate();
			try {
				lvAvailableFields.Items.Clear();
			} finally {
				lvAvailableFields.EndUpdate();
			}
			lvSelectedFields.BeginUpdate();
			try {
				lvSelectedFields.Items.Clear();
			} finally {
				lvSelectedFields.EndUpdate();
			}
			ObjectNameCollection fields = wizard.Fields;
			int count = fields.Count;
			for (int i = 0; i < count; i++) {
				ListViewItem item = new ListViewItem(fields[i].DisplayName, 1);
				item.Tag = fields[i];
				lvAvailableFields.Items.Add(item);
			}
			UpdateListViewColumnWidth(lvAvailableFields);
			UpdateListViewColumnWidth(lvSelectedFields);
			UpdateButtons();
		}
		protected override bool OnSetActive() {
			if (fillFieldsListOnActivate)
				FillFieldLists();
			if (lvAvailableFields.Items.Count > 0 && lvAvailableFields.SelectedItems.Count <= 0)
				lvAvailableFields.Items[0].Selected = true;
			return true;
		}
		protected override void UpdateWizardButtons() {
			if(lvSelectedFields.Items.Count > 0)
				Wizard.WizardButtons = WizardButton.Back | WizardButton.Next | WizardButton.Finish;
			else
				Wizard.WizardButtons = WizardButton.Back | WizardButton.Finish;
		}
		private void lvSelectedFields_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateButtons();
		}
		private void lvAvailableFields_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdateButtons();
		}
	}
}
