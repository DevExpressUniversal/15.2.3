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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class GroupSummary : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		private DevExpress.XtraEditors.ListBoxControl SumItemList;
		private DevExpress.XtraEditors.SimpleButton btRemove;
		private DevExpress.XtraEditors.SimpleButton btAdd;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		public DevExpress.XtraEditors.SimpleButton btnUp;
		public DevExpress.XtraEditors.SimpleButton btnDown;
		private XtraEditors.SearchControl searchControl1;
		private XtraEditors.CheckButton btnSearch;
		private System.ComponentModel.Container components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupSummary));
			this.btRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btAdd = new DevExpress.XtraEditors.SimpleButton();
			this.SumItemList = new DevExpress.XtraEditors.ListBoxControl();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			this.searchControl1 = new DevExpress.XtraEditors.SearchControl();
			this.btnSearch = new DevExpress.XtraEditors.CheckButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.SumItemList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.searchControl1.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.splMain, "splMain");
			resources.ApplyResources(this.pgMain, "pgMain");
			this.pnlControl.Controls.Add(this.btnSearch);
			this.pnlControl.Controls.Add(this.btnUp);
			this.pnlControl.Controls.Add(this.btRemove);
			this.pnlControl.Controls.Add(this.btnDown);
			this.pnlControl.Controls.Add(this.btAdd);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.groupControl1);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.btRemove.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btRemove, "btRemove");
			this.btRemove.Name = "btRemove";
			this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
			this.btAdd.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btAdd, "btAdd");
			this.btAdd.Name = "btAdd";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			this.SumItemList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.SumItemList, "SumItemList");
			this.SumItemList.ItemHeight = 16;
			this.SumItemList.Name = "SumItemList";
			this.SumItemList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.SumItemList.SelectedIndexChanged += new System.EventHandler(this.SumItemList_SelectedIndexChanged);
			this.btnUp.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnUp.Name = "btnUp";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			this.btnDown.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDown.Name = "btnDown";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			this.groupControl1.CaptionImageUri.Uri = "";
			this.groupControl1.Controls.Add(this.SumItemList);
			this.groupControl1.Controls.Add(this.searchControl1);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			this.searchControl1.Client = this.SumItemList;
			resources.ApplyResources(this.searchControl1, "searchControl1");
			this.searchControl1.Name = "searchControl1";
			this.searchControl1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()});
			this.searchControl1.Properties.Client = this.SumItemList;
			this.btnSearch.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btnSearch, "btnSearch");
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.CheckedChanged += new System.EventHandler(this.btnSearch_CheckedChanged);
			this.searchControl1.Visible = this.btnSearch.Checked;
			this.Name = "GroupSummary";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.SumItemList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.searchControl1.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		protected override void InitImages() {
			base.InitImages();
			btAdd.Image = DesignerImages16.Images[DesignerImages16PlusIndex];
			btRemove.Image = DesignerImages16.Images[DesignerImages16MinusIndex];
			btnUp.Image = DesignerImages16.Images[DesignerImages16UpIndex];
			btnDown.Image = DesignerImages16.Images[DesignerImages16DownIndex];
			btnSearch.Image = FindImage;
		}
		protected override string DescriptionText { get { return DevExpress.XtraGrid.Design.Properties.Resources.GroupSummaryDescription; } }
		public GroupSummary() : base(5) {
			InitializeComponent();
			pgMain.BringToFront();
			AssingButtonsToolTip();
		}
		protected void AssingButtonsToolTip() {
			btAdd.ToolTipTitle = DevExpress.XtraGrid.Design.Properties.Resources.AddSummaryItemText;
			btAdd.ToolTip = DevExpress.XtraGrid.Design.Properties.Resources.AddSummaryItemDescription;
			btRemove.ToolTipTitle = DevExpress.XtraGrid.Design.Properties.Resources.RemoveSummaryItemText;
			btRemove.ToolTip = DevExpress.XtraGrid.Design.Properties.Resources.RemoveSummaryItemDescription;
			btnUp.ToolTipTitle = DevExpress.XtraGrid.Design.Properties.Resources.MoveItemTowardBeginningText;
			btnUp.ToolTip = DevExpress.XtraGrid.Design.Properties.Resources.MoveSummaryItemUpDescription;
			btnDown.ToolTipTitle = DevExpress.XtraGrid.Design.Properties.Resources.MoveItemTowardEndText;
			btnDown.ToolTip = DevExpress.XtraGrid.Design.Properties.Resources.MoveSummaryItemDownDescription;
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
		protected GridView EditingView { get { return EditingObject as GridView; } }
		private void FillData() {
			FillData(0);
		}
		bool lockUpdate = false;
		private void FillData(int indx) {
			SumItemList.BeginUpdate();
			lockUpdate = true;
			try {
				SumItemList.Items.Clear();
				if(EditingView == null) return;
				for(int i = 0; i < EditingView.GroupSummary.Count; i++) {
					SumItemList.Items.Add(new SummaryListBoxItem(EditingView.GroupSummary[i], i)); 
				}
				if(!DevExpress.XtraEditors.Design.FramesUtils.SelectItem(SumItemList, indx))
					SelectedIndexChanged();
			} finally {
				SumItemList.EndUpdate();
				lockUpdate = false;
				EnableArrowButtons();
			}
		}
		#endregion
		#region Editing
		protected override object[] SelectedObjects {
			get {
				if(SumItemList.SelectedIndex < 0 || EditingView == null) return null;
				object[] selectedObjects = new object[SumItemList.SelectedIndices.Count];
				ArrayList al = new ArrayList();
				for(int i = 0; i < SumItemList.SelectedIndices.Count; i++) {
					selectedObjects[i] = EditingView.GroupSummary[SumItemList.SelectedIndices[i]];
				}
				return selectedObjects;
			}
		}
		protected override object[] GetPropertyGridSampleObjects(object obj) {
			GridGroupSummaryItem item = obj as GridGroupSummaryItem;
			if(SampleObjects == null || item == null) return null;
			object[] samples = new object[SampleObjects.Length];
			for(int i = 0; i < samples.Length; i ++)
				samples[i] = (SampleObjects[i] as GridView).GroupSummary[item.Index];
			return samples;
		}
		void SelectedIndexChanged() {
			RefreshPropertyGrid();
			EnableArrowButtons();
		}
		private void SumItemList_SelectedIndexChanged(object sender, System.EventArgs e) {
			SelectedIndexChanged();
		}
		private void btAdd_Click(object sender, System.EventArgs e) {
			if(EditingView == null) return;
			SumItemList.BeginUpdate();
			try {
				GridSummaryItem item = AddGroupSummary();
				if(item != null) {
					int i = SumItemList.Items.Add(new SummaryListBoxItem(item, SumItemList.Items.Count));
					SumItemList.SelectedIndex = i;
				}
			} finally {
				SumItemList.EndUpdate();
			}
			FireChanged();
		}
		private void btRemove_Click(object sender, System.EventArgs e) {
			if(EditingView == null) return;
			int j = SumItemList.SelectedIndex;
			for(int i = SumItemList.SelectedIndices.Count - 1; i >= 0 ; i--) {
				RemoveGroupSummary(SumItemList.SelectedIndices[i]);
			}
			FillData();
			DevExpress.XtraEditors.Design.FramesUtils.SelectItem(SumItemList, j);
			FireChanged();
		}
		private void FireChanged() {
			if(EditingView == null) return;
			IComponentChangeService srv = EditingView.GridControl.InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(EditingView, null, null, null);
		}
		protected virtual GridSummaryItem AddGroupSummary() {
			GridSummaryItem item = EditingView.GroupSummary.Add();
			if(SampleView != null)
				SampleView.GroupSummary.Add();
			return item;
		}
		protected virtual void RemoveGroupSummary(int index) {
			EditingView.GroupSummary.RemoveAt(index);
			if(SampleView != null)
				SampleView.GroupSummary.RemoveAt(index);
		}
		protected GridView SampleView { get { return SampleObjects != null && SampleObjects.Length > 0 ? SampleObjects[0] as GridView : null; } }
		protected override void OnPropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			base.OnPropertyGridPropertyValueChanged(sender, e);
			SumItemList.Refresh();
			FireChanged();
		}
		#endregion
		class SummaryListBoxItem : object {
			GridSummaryItem item = null;
			int index = -1;
			public SummaryListBoxItem(GridSummaryItem item, int index) {
				this.item = item;
				this.index = index;
			}
			public override string ToString() {
				if(item.FieldName != null && item.FieldName != string.Empty)
					return string.Format(DevExpress.XtraGrid.Design.Properties.Resources.SummaryItemCaption, item.FieldName, index);
				return string.Format(DevExpress.XtraGrid.Design.Properties.Resources.SummaryItemIndexCaption, index);
			}
		}
		bool AllowMoveCollectionItems {
			get {
				if(SelectedObjects == null) return false;
				return SelectedObjects.Length == 1;
			}
		}
		protected virtual void EnableArrowButtons() {
			if(lockUpdate) return;
			btRemove.Enabled = (SelectedObjects != null && SelectedObjects.Length > 0);
			btnDown.Enabled = AllowMoveCollectionItems && !SumItemList.SelectedIndices.Contains(SumItemList.ItemCount - 1);
			btnUp.Enabled = AllowMoveCollectionItems && !SumItemList.SelectedIndices.Contains(0);
			groupControl1.Text = string.Format(DevExpress.XtraGrid.Design.Properties.Resources.SummaryItemsCaption, SumItemList.Items.Count);
		}
		GridGroupSummaryItemCollection GroupSummaryItems { get { return EditingView != null ? EditingView.GroupSummary : null; } }
		void MoveRecord(int j) {
			if(SumItemList.SelectedIndices.Count != 1) return;
			int indx = SumItemList.SelectedIndex;
			ArrayList collection = new ArrayList();
			for(int i = 0; i < GroupSummaryItems.Count; i++) {
				int pos = i;
				if(i == indx + j) pos = i - j;
				if(i == indx) pos = i + j;
				collection.Add(GroupSummaryItems[pos]);
			}
			GroupSummaryItems.Clear();
			for(int i = 0; i < collection.Count; i++)
				GroupSummaryItems.Add((GridSummaryItem)collection[i]);
			FillData(indx + j);
			FireChanged();
		}
		private void btnUp_Click(object sender, EventArgs e) {
			MoveRecord(-1);
		}
		private void btnDown_Click(object sender, EventArgs e) {
			MoveRecord(1);
		}
		private void btnSearch_CheckedChanged(object sender, EventArgs e) {
			searchControl1.Visible = btnSearch.Checked;
			if(!btnSearch.Checked)
				searchControl1.SetFilter(null);
		}
	}
}
