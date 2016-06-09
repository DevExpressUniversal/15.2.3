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
using DevExpress.Utils.Design;
using DevExpress.Data.Helpers;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public abstract class ColumnDesignerBase : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		public DevExpress.XtraEditors.SimpleButton btnDown;
		public DevExpress.XtraEditors.SimpleButton btnUp;
		DevExpress.XtraEditors.ListBoxControl ColumnList;
		protected DevExpress.XtraEditors.SimpleButton btRetrieve;
		protected DevExpress.XtraEditors.SimpleButton btRemove;
		protected DevExpress.XtraEditors.SimpleButton btAdd;
		protected DevExpress.XtraEditors.SimpleButton btInsert;
		DevExpress.XtraEditors.PanelControl pnlList;
		DevExpress.XtraEditors.SplitterControl splLeft;
		DevExpress.XtraEditors.ListBoxControl FieldList;
		protected DevExpress.XtraEditors.CheckButton chbFieldList;
		public SortGroupPanel groupControlFields;
		DevExpress.XtraEditors.GroupControl groupControlColumns;
		System.ComponentModel.IContainer components = null;
		protected Panel buttonPanel;
		protected CheckButton chbFilter;
		private SearchControl scColumns;
		private SearchControl scFields;
		XtraTabControl tabControl = null;
		protected override bool AllowGlobalStore { get { return false; } }
		public override void StoreLocalProperties(PropertyStore localStore) {
			localStore.AddProperty("ShowFields", VisibleList);
			localStore.AddProperty("FieldListWidth", pnlList.Width);
			base.StoreLocalProperties(localStore);
		}
		int fieldListWidth = 0;
		public override void RestoreLocalProperties(PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			visibleList = localStore.RestoreBoolProperty("ShowFields", false);
			fieldListWidth = localStore.RestoreIntProperty("FieldListWidth", 100);
			if(fieldListWidth == 0 && visibleList)
				fieldListWidth = 100;
			chbFieldList.Checked = visibleList;
			AssignChbFlieldListToolTip(visibleList);
			UpdateSearchControls();
		}
		#region Component Designer generated code
		void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnDesignerBase));
			this.ColumnList = new DevExpress.XtraEditors.ListBoxControl();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.btRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btInsert = new DevExpress.XtraEditors.SimpleButton();
			this.btAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btRetrieve = new DevExpress.XtraEditors.SimpleButton();
			this.pnlList = new DevExpress.XtraEditors.PanelControl();
			this.groupControlFields = new DevExpress.XtraEditors.Frames.SortGroupPanel();
			this.FieldList = new DevExpress.XtraEditors.ListBoxControl();
			this.scFields = new DevExpress.XtraEditors.SearchControl();
			this.groupControlColumns = new DevExpress.XtraEditors.GroupControl();
			this.scColumns = new DevExpress.XtraEditors.SearchControl();
			this.splLeft = new DevExpress.XtraEditors.SplitterControl();
			this.chbFieldList = new DevExpress.XtraEditors.CheckButton();
			this.buttonPanel = new System.Windows.Forms.Panel();
			this.chbFilter = new DevExpress.XtraEditors.CheckButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ColumnList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlList)).BeginInit();
			this.pnlList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupControlFields)).BeginInit();
			this.groupControlFields.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.FieldList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.scFields.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControlColumns)).BeginInit();
			this.groupControlColumns.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scColumns.Properties)).BeginInit();
			this.buttonPanel.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.splMain, "splMain");
			resources.ApplyResources(this.pgMain, "pgMain");
			this.pnlControl.Controls.Add(this.chbFilter);
			this.pnlControl.Controls.Add(this.btRetrieve);
			this.pnlControl.Controls.Add(this.buttonPanel);
			this.pnlControl.Controls.Add(this.chbFieldList);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.groupControlColumns);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			this.pnlMain.LocationChanged += new System.EventHandler(this.pnlMainLocationChanged);
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.ColumnList.AllowDrop = true;
			this.ColumnList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.ColumnList, "ColumnList");
			this.ColumnList.ItemHeight = 16;
			this.ColumnList.Name = "ColumnList";
			this.ColumnList.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.ColumnList.SelectedIndexChanged += new System.EventHandler(this.ColumnList_SelectedIndexChanged);
			this.ColumnList.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.ColumnList_DrawItem);
			this.ColumnList.DragDrop += new System.Windows.Forms.DragEventHandler(this.ColumnList_DragDrop);
			this.ColumnList.DragOver += new System.Windows.Forms.DragEventHandler(this.ColumnList_DragOver);
			this.ColumnList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ColumnList_KeyDown);
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
			this.btRemove.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btRemove, "btRemove");
			this.btRemove.Name = "btRemove";
			this.btRemove.Click += new System.EventHandler(this.btRemove_Click);
			this.btInsert.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btInsert.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btInsert, "btInsert");
			this.btInsert.Name = "btInsert";
			this.btInsert.Click += new System.EventHandler(this.btInsert_Click);
			this.btAdd.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btAdd, "btAdd");
			this.btAdd.Name = "btAdd";
			this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
			this.btRetrieve.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btRetrieve.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.btRetrieve, "btRetrieve");
			this.btRetrieve.Name = "btRetrieve";
			this.btRetrieve.Click += new System.EventHandler(this.btRetrieve_Click);
			this.pnlList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlList.Controls.Add(this.groupControlFields);
			resources.ApplyResources(this.pnlList, "pnlList");
			this.pnlList.Name = "pnlList";
			this.pnlList.SizeChanged += new System.EventHandler(this.pnlList_SizeChanged);
			this.groupControlFields.Controls.Add(this.FieldList);
			this.groupControlFields.Controls.Add(this.scFields);
			resources.ApplyResources(this.groupControlFields, "groupControlFields");
			this.groupControlFields.Name = "groupControlFields";
			this.groupControlFields.SortOrder = -1;
			this.groupControlFields.SortState = -1;
			this.FieldList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.FieldList, "FieldList");
			this.FieldList.ItemHeight = 16;
			this.FieldList.Name = "FieldList";
			this.FieldList.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.FieldList_DrawItem);
			this.FieldList.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.FieldList_MeasureItem);
			this.FieldList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FieldList_MouseDown);
			this.FieldList.MouseMove += new System.Windows.Forms.MouseEventHandler(this.FieldList_MouseMove);
			this.scFields.Client = this.FieldList;
			resources.ApplyResources(this.scFields, "scFields");
			this.scFields.Name = "scFields";
			this.scFields.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.scFields.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()});
			this.scFields.Properties.Client = this.FieldList;
			this.scFields.Properties.FindDelay = 100;
			this.scFields.Properties.NullValuePrompt = resources.GetString("scFields.Properties.NullValuePrompt");
			this.scFields.Properties.ShowNullValuePromptWhenFocused = true;
			this.groupControlColumns.Controls.Add(this.ColumnList);
			this.groupControlColumns.Controls.Add(this.scColumns);
			resources.ApplyResources(this.groupControlColumns, "groupControlColumns");
			this.groupControlColumns.Name = "groupControlColumns";
			this.scColumns.Client = this.ColumnList;
			resources.ApplyResources(this.scColumns, "scColumns");
			this.scColumns.Name = "scColumns";
			this.scColumns.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.scColumns.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()});
			this.scColumns.Properties.Client = this.ColumnList;
			this.scColumns.Properties.FindDelay = 100;
			this.scColumns.Properties.NullValuePrompt = resources.GetString("scColumns.Properties.NullValuePrompt");
			this.scColumns.Properties.ShowNullValuePromptWhenFocused = true;
			resources.ApplyResources(this.splLeft, "splLeft");
			this.splLeft.Name = "splLeft";
			this.splLeft.TabStop = false;
			this.chbFieldList.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.chbFieldList.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.chbFieldList, "chbFieldList");
			this.chbFieldList.Name = "chbFieldList";
			this.chbFieldList.Click += new System.EventHandler(this.chbFieldList_Click);
			this.buttonPanel.Controls.Add(this.btAdd);
			this.buttonPanel.Controls.Add(this.btnDown);
			this.buttonPanel.Controls.Add(this.btRemove);
			this.buttonPanel.Controls.Add(this.btnUp);
			this.buttonPanel.Controls.Add(this.btInsert);
			resources.ApplyResources(this.buttonPanel, "buttonPanel");
			this.buttonPanel.Name = "buttonPanel";
			this.chbFilter.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.chbFilter.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.chbFilter, "chbFilter");
			this.chbFilter.Name = "chbFilter";
			this.chbFilter.CheckedChanged += new System.EventHandler(this.chbFilter_CheckedChanged);
			this.Controls.Add(this.splLeft);
			this.Controls.Add(this.pnlList);
			this.Name = "ColumnDesignerBase";
			resources.ApplyResources(this, "$this");
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.pgMain, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			this.Controls.SetChildIndex(this.pnlControl, 0);
			this.Controls.SetChildIndex(this.pnlList, 0);
			this.Controls.SetChildIndex(this.splLeft, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			this.Controls.SetChildIndex(this.splMain, 0);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ColumnList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlList)).EndInit();
			this.pnlList.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupControlFields)).EndInit();
			this.groupControlFields.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.FieldList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.scFields.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControlColumns)).EndInit();
			this.groupControlColumns.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scColumns.Properties)).EndInit();
			this.buttonPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected bool IsDesignMode { get { return ColumnsOwner.Site != null && ColumnsOwner.Site.DesignMode; } }
		protected virtual bool AllowModify {
			get {
				return !IsDesignMode || InheritanceHelper.AllowCollectionModify(ColumnsOwner, ColumnsDescriptor, Columns);
			}
		}
		protected virtual bool AllowElementsEdit {
			get {
				return !IsDesignMode || InheritanceHelper.AllowCollectionItemEdit(ColumnsOwner, ColumnsDescriptor, Columns);
			}
		}
		protected virtual bool AllowRemoveItem(object item) {
			return !IsDesignMode || InheritanceHelper.AllowCollectionItemRemove(ColumnsOwner, ColumnsDescriptor, Columns, item);
		}
		protected virtual bool AllowEditItem(object item) {
			return !IsDesignMode || InheritanceHelper.AllowCollectionItemEdit(ColumnsOwner, ColumnsDescriptor, Columns, item);
		}
		protected abstract PropertyDescriptor ColumnsDescriptor { get; }
		protected abstract Component ColumnsOwner { get; }
		protected abstract CollectionBase Columns { get; }
		protected abstract ColumnObject CreateColumnObject(object column);
		protected abstract void RetrieveColumnsCore();
		protected abstract object CreateNewColumn(string fieldName, int index);
		protected abstract string[] GetDataFieldList();
		protected virtual object InternalGetService(Type type) { return null;  }
		protected virtual string[] GetTabsCaption() { return null;  } 
		protected virtual bool CanRetrieveFields { get { return true; } }
		protected virtual string GroupControlColumnsText { get { return Properties.Resources.ColumnsCaption; } }
		protected virtual void OnColumnAdded(object column, int visibleIndex) {}
		protected virtual void OnColumnRemoved(object column, int index) {}
		protected virtual void OnColumnRecreated() {}
		protected virtual bool CanRetrieveColumn(string fieldName) { return true; }
		#region Init & Ctor
		public ColumnDesignerBase() : base(2) {
			InitializeComponent();
			FieldList.DoubleClick += new System.EventHandler(this.FieldList_DoubleClick);
			pgMain.BringToFront();
			groupControlFields.SortOrderChanged += new EventHandler(groupControlFields_SortOrderChanged);
			UpdateSearchControls();
		}
		void groupControlFields_SortOrderChanged(object sender, EventArgs e) {
			UpdateFieldList(VisibleList);
			UpdateDescriptionPanel();
		}
		protected override void InitImages() {
			base.InitImages();
			btAdd.Image = DesignerImages16.Images[DesignerImages16AddIndex];
			btInsert.Image = DesignerImages16.Images[DesignerImages16InsertIndex];
			btRemove.Image = DesignerImages16.Images[DesignerImages16RemoveIndex];
			btRetrieve.Image = DesignerImages16.Images[DesignerImages16RetrieveIndex];
			btnUp.Image = DesignerImages16.Images[DesignerImages16UpIndex];
			btnDown.Image = DesignerImages16.Images[DesignerImages16DownIndex];
			chbFieldList.ImageList = DesignerImages16;
			chbFilter.Image = FindImage;
		}
		public override void InitComponent() {
			IComponentChangeService cs = InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(cs != null)
				cs.ComponentRename += new ComponentRenameEventHandler(ComponentChangeService_Rename);
			FillData();
			this.tabControl = CreateTabControl(GetTabsCaption());
			if(this.tabControl != null) 
				this.tabControl.SelectedPageChanged += new TabPageChangedEventHandler(changeTabPage);
			string[] fields = GetDataFieldList();
			btRetrieve.Enabled = fields != null && fields.Length > 0 && AllowModify;
			btRetrieve.Visible = CanRetrieveFields;
			if(!CanRetrieveFields)
				chbFieldList.Left = btRetrieve.Left;
			ShowVisibleList(VisibleList);
			pgMain.Enabled = AllowElementsEdit;
			btAdd.Enabled = AllowModify;
			scColumns.Properties.NullValuePrompt = Localizer.Active.GetLocalizedString(StringId.SearchForColumn);
			scFields.Properties.NullValuePrompt = Localizer.Active.GetLocalizedString(StringId.SearchForField);
		}
		protected override void OnPropertyGridPropertyValueChanged(object sender, PropertyValueChangedEventArgs e) {
			base.OnPropertyGridPropertyValueChanged(sender, e);
			if(e.ChangedItem == null) return;
			if("VisibleVisibleIndex".Contains(e.ChangedItem.Label))
				ColumnList.Refresh();
		}
		XtraTabControl CreateTabControl(string[] tabs) {
			if(tabs == null || tabs.Length == 0) return null;
			XtraTabControl tabControl = new XtraTabControl();
			for(int i = 0; i < tabs.Length; i++) {
				XtraTabPage page = tabControl.TabPages.Add(tabs[i]);
				if(i == 0) {
					page.Controls.Add(pgMain);
					pgMain.Dock = DockStyle.Fill;
				}
			}
			this.Controls.Add(tabControl);
			tabControl.Dock = DockStyle.Fill;
			tabControl.BringToFront();
			return tabControl;
		}
		protected virtual void  DeInitComponent() {
			if(EditingObject == null) return;
			IComponentChangeService cs = InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(cs != null)
				cs.ComponentRename -= new ComponentRenameEventHandler(ComponentChangeService_Rename);
		}
		void FillData() {
			if(Columns == null) return;
			ColumnList.BeginUpdate();
			ColumnList.Items.BeginUpdate();
			try {
				ColumnList.Items.Clear();
				foreach(object column in Columns) {
					ColumnList.Items.Add(CreateColumnObject(column));
				}
				if(ColumnList.Items.Count > 0)
					ColumnList.SelectedIndex = 0;
			}
			finally {
				ColumnList.EndUpdate();
				ColumnList.Items.EndUpdate();
			}
			UpdatePropertyGridAndButtons();
		}
		protected override void Dispose(bool disposing) {	
			DeInitComponent();
			if(components != null) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		const int DefaultColumnsButtonIndent = 25;
		void pnlMainLocationChanged(object sender, EventArgs e) {
			int x = pnlMain.Location.X;
			if(x < btRetrieve.Right + DefaultColumnsButtonIndent)
				x = btRetrieve.Right + DefaultColumnsButtonIndent;
			buttonPanel.Location = new Point(x, buttonPanel.Location.Y);
		}
		#endregion
		#region Column options
		void changeTabPage(object sender, TabPageChangedEventArgs e) {
			RefreshPropertyGrid();
			e.Page.Controls.Add(pgMain);
			pgMain.ShowEvents(SelectedTabPage == 0);
			pgMain.Refresh();
		}
		protected int SelectedTabPage { get { return tabControl != null ? tabControl.SelectedTabPageIndex : 0; } }
		#endregion
		#region Editing		
		protected void ComponentChangeService_Rename(object sender, ComponentRenameEventArgs e) {
			UpdateColumnData();
		}
		protected void UpdateColumnData() {
			ColumnList.Refresh();
			FieldList.Refresh();
		}
		ColumnObject FocusedColumnObject { get { return ColumnList.SelectedItem as ColumnObject; } }
		object FocusedColumn { get { return FocusedColumnObject != null ? FocusedColumnObject.Column : null; } }
		protected override object[] SelectedObjects {
			get {
				ColumnObject[] columns = GetSelectedColumns();
				if(columns == null) return null;
				object[] selectedObjects = new object[columns.Length];
				for(int i = 0; i < columns.Length; i ++) 
					selectedObjects[i] = columns[i].Column;
				return selectedObjects;
			}
		}
		ColumnObject[] GetSelectedColumns() {
			if(ColumnList.SelectedIndex < 0) return null;
			ColumnObject[] selectedColumns = new ColumnObject[ColumnList.SelectedIndices.Count];
			for(int i = 0; i < ColumnList.SelectedIndices.Count; i++)
				selectedColumns[i] = ColumnList.GetItem(ColumnList.SelectedIndices[i]) as ColumnObject;
			return selectedColumns;
		}
		void AddColumn() { AddColumn("", -1); }
		void AddColumn(int index) { AddColumn("", index); }
		void AddColumn(string fieldName, int index) {
			if(!AllowModify) return;
			if(ColumnsOwner == null) return;
			if(!CanRetrieveColumn(fieldName)) return;
			if(index < 0) index = Columns.Count;
			object col = CreateNewColumn(fieldName, index);
			ColumnObject columnObject = CreateColumnObject(col);
			if(index  < ColumnList.Items.Count) 
				ColumnList.Items.Insert(index, columnObject);
				else ColumnList.Items.Add(columnObject);
			OnColumnAdded(col, index);
			ColumnList.SelectedIndex = index;
		}
		void RemoveColumn() {
			if(!AllowModify) return;
			if(ColumnsOwner == null || SelectedObjects == null) return;
			ColumnList.BeginUpdate();
			try {
				int selectedIndex = ColumnList.SelectedIndex;
				ColumnList.Items.BeginUpdate();
				try {
					for(int i = ColumnList.SelectedIndices.Count - 1; i >= 0 ; i--) {
						int removedIndex = ColumnList.SelectedIndices[i];
						ColumnObject columnObject = ColumnList.Items[removedIndex] as ColumnObject;
						if(!AllowRemoveItem(columnObject)) continue;
						OnColumnRemoved(columnObject.Column, removedIndex);
						IDisposable disposableColumn = columnObject.Column as IDisposable;
						if(disposableColumn != null)
							disposableColumn.Dispose();
						ColumnList.Items.RemoveAt(removedIndex);	
					}
				}
				finally {
					ColumnList.Items.EndUpdate();
				}
				if(selectedIndex >= ColumnList.Items.Count)
					selectedIndex = ColumnList.Items.Count - 1;
				ColumnList.SelectedIndex = selectedIndex;
			}
			finally {
				ColumnList.EndUpdate();
			}
			UpdatePropertyGridAndButtons();
		}
		void btAdd_Click(object sender, System.EventArgs e) {
			AddColumn();
		}
		void btRemove_Click(object sender, System.EventArgs e) {
			RemoveColumn();
		}
		void RetrieveColumns() {
			if(ColumnsOwner == null) return;
			if(Columns.Count > 0) {
				if(MessageBox.Show(this, Properties.Resources.RetrieveColumnsConfirmation, Properties.Resources.ConfirmationCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes) 
					return;
			}
			RetrieveColumnsCore();
			OnColumnRecreated();
			FillData();
		}
		void btRetrieve_Click(object sender, System.EventArgs e) {
			RetrieveColumns();
		}
		void MoveFocusedColumn(int delta) {
			if(FocusedColumnObject == null) return;
			ColumnList.BeginUpdate();
			try {
				int absoluteIndex = FocusedColumnObject.AbsoluteIndex;
				absoluteIndex += delta;
				if(absoluteIndex < 0)
					absoluteIndex = 0;
				if(absoluteIndex >= Columns.Count)
					absoluteIndex = Columns.Count - 1;
				FocusedColumnObject.AbsoluteIndex = absoluteIndex;
				FillData();
				ColumnList.SelectedIndex = absoluteIndex;
			} finally {
				ColumnList.EndUpdate();
			}
			UpdateButtonsAndColumnCount();
		}
		void btnDown_Click(object sender, System.EventArgs e) {
			MoveFocusedColumn(1);
		}
		void btnUp_Click(object sender, System.EventArgs e) {
			MoveFocusedColumn(-1);
		}
		void UpdateButtonsAndColumnCount() {
			int focusedColumnCount = SelectedObjects != null ? SelectedObjects.Length : 0; 
			btnUp.Enabled = focusedColumnCount == 1 && ColumnList.SelectedIndex != 0;
			btnDown.Enabled = focusedColumnCount == 1 && ColumnList.SelectedIndex != ColumnList.Items.Count - 1;
			btRemove.Enabled = focusedColumnCount > 0 && AllowRemoveItem(SelectedObjects[0]);
			btInsert.Enabled = focusedColumnCount > 0 && AllowModify;
			groupControlColumns.Text = string.Format(GroupControlColumnsText + " ({0})", ColumnList.Items.Count);
			pgMain.Enabled = focusedColumnCount > 0 && AllowEditItem(SelectedObjects[0]);
		}
		void ColumnList_SelectedIndexChanged(object sender, System.EventArgs e) {
			UpdatePropertyGridAndButtons();
			pgMain.ShowEvents(SelectedTabPage == 0);
		}
		void UpdatePropertyGridAndButtons() {
			RefreshPropertyGrid();
			UpdateButtonsAndColumnCount();
			UpdateColumnData();
		}
		void btInsert_Click(object sender, System.EventArgs e) {
			AddColumn(ColumnList.SelectedIndex);
		}
		void FireChanged() {
			IComponentChangeService srv = InternalGetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(ColumnsOwner, null, null, null);
		}
		void ColumnList_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			switch(e.KeyCode) {
				case Keys.Delete:
					RemoveColumn();
					e.Handled = true;
					break;
				case Keys.Insert:
					if(btInsert.Enabled)
						AddColumn(ColumnList.SelectedIndex);
					e.Handled = true;
					break;
			}
		}
		#endregion
		#region FieldList
		bool visibleList = false;
		protected bool VisibleList { get { return visibleList; } }
		void chbFieldList_Click(object sender, System.EventArgs e) {
			visibleList = !VisibleList;
			ShowVisibleList(VisibleList);
		}
		void ShowVisibleList(bool visible) {
			pnlList.Width = fieldListWidth;
			pnlList.Visible = splLeft.Visible = visible;
			chbFieldList.ImageIndex = 14;
			AssignChbFlieldListToolTip(visible);
			UpdateFieldList(visible);
			UpdateDescriptionPanel();
		}
		protected virtual void AssignChbFlieldListToolTip(bool visible) {
			chbFieldList.ToolTip = visible ? Properties.Resources.HideFieldListCaption : Properties.Resources.ShowFieldListCaption;
		}
		void UpdateFieldList(bool visible) {
			if(!visible) return;
			string[] list = GetDataFieldList();
			FieldList.DataSource = list;
			string nameFieldList = Localizer.Active.GetLocalizedString(StringId.FieldListName);
			groupControlFields.Text = string.Format(nameFieldList, FieldList.ItemCount);
		}
		protected override string DescriptionText { 
			get { return Properties.Resources.ColumnDesignerDescription1 + (VisibleList ? " " + Properties.Resources.ColumnDesignerDescription2 : ""); }
		}
		private void FieldList_DoubleClick(object sender, System.EventArgs e) {
			ListBoxControl lbControl = sender as ListBoxControl;
			Point p = lbControl.PointToClient(Control.MousePosition);
			int i = lbControl.IndexFromPoint(p);
			if(i > -1) AddColumn(lbControl.GetItemText(i), -1);
		}
		#endregion
		#region DragDrop
		Point mDown = Point.Empty;
		void FieldList_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			mDown = new Point(e.X, e.Y);
		}
		void FieldList_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(mDown == Point.Empty) return;
			Rectangle dragRect = new Rectangle(new Point(
				mDown.X - SystemInformation.DragSize.Width / 2,
				mDown.Y - SystemInformation.DragSize.Height / 2), SystemInformation.DragSize);
			if(!dragRect.Contains(new Point(e.X, e.Y))) { 
				int index = FieldList.IndexFromPoint(mDown);
				if(index < 0 || index >= FieldList.ItemCount || e.Button != MouseButtons.Left) return;
				FieldList.DoDragDrop(FieldList.GetItemText(index).ToString(), DragDropEffects.Copy);
				mDown = Point.Empty;
			}
		}
		string GetDragObject(IDataObject data) {
			return data.GetData(typeof(string)) as string;
		}
		void ColumnList_DragOver(object sender, System.Windows.Forms.DragEventArgs e) {
			string field = GetDragObject(e.Data);
			e.Effect = field == null ? DragDropEffects.None : DragDropEffects.Copy;
		}
		void ColumnList_DragDrop(object sender, System.Windows.Forms.DragEventArgs e) {
			DevExpress.XtraEditors.ListBoxControl lBox = sender as DevExpress.XtraEditors.ListBoxControl;
			string field = GetDragObject(e.Data);
			if(field == null) return;
			int index = lBox.IndexFromPoint(lBox.PointToClient(new Point(e.X, e.Y)));
			AddColumn(field, index);
		}
		#endregion
		#region DrawItem
		bool IsColumnExisting(string fieldName) {
			for(int i = 0; i < ColumnList.Items.Count; i ++) {
				ColumnObject columnObject = ColumnList.Items[i] as ColumnObject;
				if(columnObject.FieldName == fieldName) return true;
			}
			return false;
		}
		Font CurrentFont(int index) {
			if(index < 0) return null;
			string fieldName = FieldList.GetItemText(index);
			return CurrentFont(fieldName); 
		}
		Font GetDefaultFont() {
			return WindowsFormsDesignTimeSettings.DefaultDesignTimeFont;
		}
		Font CurrentFont(string fieldName) {
			return IsColumnExisting(fieldName) ?  GetDefaultFont() : new Font(GetDefaultFont(), FontStyle.Bold); 
		}
		Font GetColumnFont(object obj) {
			ColumnObject columnObject = obj as ColumnObject;
			if(columnObject != null && !columnObject.Visible) {
				return new Font(GetDefaultFont(), FontStyle.Italic);
			}
			return GetDefaultFont();
		}
		void FieldList_DrawItem(object sender, DevExpress.XtraEditors.ListBoxDrawItemEventArgs e) {
			if(e.Index < 0) return;
			string fieldName = FieldList.GetItemText(e.Index);
			Font font = CurrentFont(fieldName);
			e.Appearance.Font = font;
		}
		void FieldList_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e) {
			e.ItemHeight = ColumnList.ItemHeight;
		}
		void ColumnList_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			e.Appearance.Font = GetColumnFont(e.Item);
		}
		#endregion
		private void pnlList_SizeChanged(object sender, EventArgs e) {
			fieldListWidth = pnlList.Width;
		}
		private void chbFilter_CheckedChanged(object sender, EventArgs e) {
			UpdateSearchControls();
		}
		private void UpdateSearchControls() {
			scColumns.ClearFilter();
			scFields.ClearFilter();
			scColumns.Visible = scFields.Visible = chbFilter.Checked;
		}
	}
	public abstract class ColumnObject {
		object column;
		public ColumnObject(object column) {
			this.column = column;
		}
		public object Column { get { return column; } }
		public abstract string Caption { get; }
		public abstract string FieldName { get; }
		public abstract int AbsoluteIndex { get; set; }
		public override string ToString() {
			ISite site = Column as Component != null ? (Column as Component).Site : null;
			if(IsFullName(site, Caption))
				return string.Format("{0} <{1}>", site.Name, Caption);
			if(site != null)
				return site.Name;
			if(Caption != "")
				return Caption;
			if(FieldName != "")
				return FieldName;
			return Column.ToString();
		}
		bool IsNumber(string caption) {
			int dump;
			return Int32.TryParse(caption, out dump);
		}
		public virtual bool Visible { get { return true; } }
		protected virtual bool IsFullName(ISite site, string caption) {
			if(site == null || caption == string.Empty) return false;
			int start = site.Name.LastIndexOf(caption);
			if(start > -1 && start + caption.Length == site.Name.Length) return false;
			if(start > -1 && IsNumber(site.Name.Substring(start + caption.Length, site.Name.Length - (start + caption.Length)))) return false;
			return true;
		}
	}
}
