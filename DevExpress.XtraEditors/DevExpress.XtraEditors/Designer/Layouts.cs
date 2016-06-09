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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using System.Data.Common;
using DevExpress.Utils.Frames;
using DevExpress.Utils;
using DevExpress.XtraEditors.Frames;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using DevExpress.Data.Native;
using DevExpress.Utils.VisualEffects;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public abstract class LayoutsBase : DevExpress.XtraEditors.Designer.Utils.XtraFrame{
		private DevExpress.XtraEditors.PanelControl pnlTop;
		private DevExpress.XtraEditors.SimpleButton btnLoad;
		private DevExpress.XtraEditors.PanelControl pnlBottom;
		private DevExpress.XtraEditors.SimpleButton btnSave;
		private DevExpress.XtraEditors.PanelControl pnlGrid;
		private DevExpress.XtraEditors.PanelControl pnlData;
		private DevExpress.XtraEditors.SimpleButton btnFill;
		private DevExpress.XtraEditors.PanelControl pnlTab;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;
		private DevExpress.XtraEditors.SimpleButton btnClear;
		private System.Windows.Forms.Label lbTable;
		private DevExpress.XtraEditors.ImageComboBoxEdit pickImage1;
		private DevExpress.XtraEditors.PanelControl pnlXML;
		private System.Windows.Forms.Label lbXMLTable;
		private DevExpress.XtraEditors.SimpleButton btnXMLLoad;
		private DevExpress.XtraEditors.SimpleButton chColumnSelector;
		private DevExpress.XtraEditors.GroupControl gcXMLTables;
		private DevExpress.XtraEditors.ComboBoxEdit cbXMLTable;
		private DevExpress.XtraEditors.SimpleButton sbPreview;
		private SimpleButton chShowSortIndex;
		protected DevExpress.XtraEditors.SimpleButton btnApply;
		protected DevExpress.XtraEditors.PanelControl PnlTop { get { return pnlTop; } }
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutsBase));
			this.pnlTop = new DevExpress.XtraEditors.PanelControl();
			this.chShowSortIndex = new DevExpress.XtraEditors.SimpleButton();
			this.chColumnSelector = new DevExpress.XtraEditors.SimpleButton();
			this.btnSave = new DevExpress.XtraEditors.SimpleButton();
			this.btnLoad = new DevExpress.XtraEditors.SimpleButton();
			this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
			this.sbPreview = new DevExpress.XtraEditors.SimpleButton();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.pnlGrid = new DevExpress.XtraEditors.PanelControl();
			this.pnlData = new DevExpress.XtraEditors.PanelControl();
			this.pickImage1 = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.lbTable = new System.Windows.Forms.Label();
			this.btnClear = new DevExpress.XtraEditors.SimpleButton();
			this.btnFill = new DevExpress.XtraEditors.SimpleButton();
			this.pnlTab = new DevExpress.XtraEditors.PanelControl();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.pnlXML = new DevExpress.XtraEditors.PanelControl();
			this.gcXMLTables = new DevExpress.XtraEditors.GroupControl();
			this.cbXMLTable = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lbXMLTable = new System.Windows.Forms.Label();
			this.btnXMLLoad = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTop)).BeginInit();
			this.pnlTop.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
			this.pnlBottom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlData)).BeginInit();
			this.pnlData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pickImage1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTab)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlXML)).BeginInit();
			this.pnlXML.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcXMLTables)).BeginInit();
			this.gcXMLTables.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbXMLTable.Properties)).BeginInit();
			this.SuspendLayout();
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.pnlXML);
			this.pnlMain.Controls.Add(this.pnlTab);
			this.pnlMain.Controls.Add(this.pnlGrid);
			this.pnlMain.Controls.Add(this.pnlBottom);
			this.pnlMain.Controls.Add(this.pnlTop);
			this.pnlMain.Controls.Add(this.pnlData);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.pnlTop.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlTop.Controls.Add(this.chShowSortIndex);
			this.pnlTop.Controls.Add(this.chColumnSelector);
			this.pnlTop.Controls.Add(this.btnSave);
			this.pnlTop.Controls.Add(this.btnLoad);
			resources.ApplyResources(this.pnlTop, "pnlTop");
			this.pnlTop.Name = "pnlTop";
			resources.ApplyResources(this.chShowSortIndex, "chShowSortIndex");
			this.chShowSortIndex.Name = "chShowSortIndex";
			this.chShowSortIndex.Click += new System.EventHandler(this.chShowSortIndex_Click);
			resources.ApplyResources(this.chColumnSelector, "chColumnSelector");
			this.chColumnSelector.Name = "chColumnSelector";
			this.chColumnSelector.Click += new System.EventHandler(this.chColumnSelector_Click);
			this.btnSave.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.Name = "btnSave";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.btnLoad.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.btnLoad, "btnLoad");
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			this.pnlBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBottom.Controls.Add(this.sbPreview);
			this.pnlBottom.Controls.Add(this.btnApply);
			resources.ApplyResources(this.pnlBottom, "pnlBottom");
			this.pnlBottom.Name = "pnlBottom";
			this.sbPreview.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.sbPreview, "sbPreview");
			this.sbPreview.Name = "sbPreview";
			this.sbPreview.Click += new System.EventHandler(this.sbPreview_Click);
			this.btnApply.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			this.pnlGrid.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlGrid, "pnlGrid");
			this.pnlGrid.Name = "pnlGrid";
			this.pnlData.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlData.Controls.Add(this.pickImage1);
			this.pnlData.Controls.Add(this.lbTable);
			this.pnlData.Controls.Add(this.btnClear);
			this.pnlData.Controls.Add(this.btnFill);
			resources.ApplyResources(this.pnlData, "pnlData");
			this.pnlData.Name = "pnlData";
			resources.ApplyResources(this.pickImage1, "pickImage1");
			this.pickImage1.Name = "pickImage1";
			this.pickImage1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("pickImage1.Properties.Buttons"))))});
			resources.ApplyResources(this.lbTable, "lbTable");
			this.lbTable.BackColor = System.Drawing.SystemColors.ControlDark;
			this.lbTable.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.lbTable.Name = "lbTable";
			this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
			resources.ApplyResources(this.btnClear, "btnClear");
			this.btnClear.Name = "btnClear";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			this.btnFill.Image = ((System.Drawing.Image)(resources.GetObject("btnFill.Image")));
			resources.ApplyResources(this.btnFill, "btnFill");
			this.btnFill.Name = "btnFill";
			this.btnFill.Click += new System.EventHandler(this.btnFill_Click);
			this.pnlTab.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.pnlTab, "pnlTab");
			this.pnlTab.Name = "pnlTab";
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.pnlXML.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlXML.Controls.Add(this.gcXMLTables);
			this.pnlXML.Controls.Add(this.lbXMLTable);
			this.pnlXML.Controls.Add(this.btnXMLLoad);
			resources.ApplyResources(this.pnlXML, "pnlXML");
			this.pnlXML.Name = "pnlXML";
			resources.ApplyResources(this.gcXMLTables, "gcXMLTables");
			this.gcXMLTables.CaptionImageUri.Uri = global::DevExpress.XtraEditors.LocalizationRes.StringId_None;
			this.gcXMLTables.Controls.Add(this.cbXMLTable);
			this.gcXMLTables.Name = "gcXMLTables";
			this.cbXMLTable.EditValue = global::DevExpress.XtraEditors.LocalizationRes.StringId_None;
			resources.ApplyResources(this.cbXMLTable, "cbXMLTable");
			this.cbXMLTable.Name = "cbXMLTable";
			this.cbXMLTable.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbXMLTable.Properties.Buttons"))))});
			this.cbXMLTable.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbXMLTable.SelectedIndexChanged += new System.EventHandler(this.cbXMLTable_SelectedIndexChanged);
			resources.ApplyResources(this.lbXMLTable, "lbXMLTable");
			this.lbXMLTable.BackColor = System.Drawing.SystemColors.ControlDark;
			this.lbXMLTable.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.lbXMLTable.Name = "lbXMLTable";
			resources.ApplyResources(this.btnXMLLoad, "btnXMLLoad");
			this.btnXMLLoad.Name = "btnXMLLoad";
			this.btnXMLLoad.Click += new System.EventHandler(this.btnXMLLoad_Click);
			this.Name = "LayoutsBase";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTop)).EndInit();
			this.pnlTop.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
			this.pnlBottom.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlData)).EndInit();
			this.pnlData.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pickImage1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlTab)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlXML)).EndInit();
			this.pnlXML.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcXMLTables)).EndInit();
			this.gcXMLTables.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbXMLTable.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		AdornerUIManager aManager = null;
		public AdornerUIManager AdornerManager { get { return aManager; } set { aManager = value; } }
		public LayoutsBase(int index) : base(index) {
			InitializeComponent();
			chShowSortIndex.Visible = false;
			gcXMLTables.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			lbXMLTable.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
			pnlXML.Resize += new EventHandler(pnlXML_Resize);
			((Bitmap)btnFill.Image).MakeTransparent();
			((Bitmap)btnClear.Image).MakeTransparent();
		}
		void InitShowColumnsSortIndices() {
			chShowSortIndex.Visible = AllowShowColumnSortIndex;
			if(AllowShowColumnSortIndex) SetColumnsSortIndexCaption();
		}
		void pnlXML_Resize(object sender, EventArgs e) {
			gcXMLTables.Width = lbXMLTable.Width = pnlXML.Width - gcXMLTables.Left * 2;
		}
		protected override void Dispose(bool disposing) {
			if(layoutChanged && disposing) {
				if(XtraMessageBox.Show(this.LookAndFeel.ParentLookAndFeel, this, Properties.Resources.LayoutCloseWarning, lbCaption.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
					Apply();
			}
			base.Dispose(disposing);
		}
		protected override void InitImages() {
			base.InitImages();
			btnLoad.Image = DesignerImages16.Images[DesignerImages16LoadIndex];
			btnSave.Image = DesignerImages16.Images[DesignerImages16SaveIndex];
		}
		bool isAdapting = false;
		private void CreateDataAdaptersPanel(ComponentCollection components) {
			int imageIndex = 0;
			foreach(object obj in components) {
				if(obj is IDataAdapter) {
					imageIndex = (obj is System.Data.SqlClient.SqlDataAdapter) ? 1 : 0;
					pickImage1.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(
						((Component)obj).Site.Name, obj, imageIndex));
					isAdapting = true;
				}
			}
			if(isAdapting) {
				pickImage1.Properties.SmallImages = imageList1;
				pickImage1.SelectedIndex = 0;
				XtraTabPage tp = new XtraTabPage();
				tp.Text = Properties.Resources.DataAdaptersCaption;
				tp.Controls.Add(pnlData);
				pnlData.Dock = DockStyle.Fill;
				pnlData.Visible = true;
				tbc.TabPages.Add(tp);
				TableRows();
				try {
					lbTable.Font = new Font("Arial", 8, FontStyle.Bold);
				} catch {}
			}
		}
		protected DBAdapter dbAdapter = null;
		protected DataSet dataSet;
		protected string tableName = "";
		Control previewControl = null;
		protected abstract Control CreatePreviewControl();
		protected abstract void ShowColumnsCustomization();
		protected abstract void HideColumnsCustomization();
		protected abstract void ApplyLayouts();
		protected abstract void RestoreLayoutFromXml(string fileName);
		protected abstract void SaveLayoutToXml(string fileName);
		protected abstract void SetControlDataSource(DataView dataView);
		protected abstract DataTable CreateDataTableAdapter();
		protected abstract DBAdapter CreateDBAdapter();
		protected virtual void OnFillGrid() {}
		protected virtual void InitUnboundData() {}
		protected virtual void SetUnboundData() {}
		protected virtual string PreviewPanelText { get { return Properties.Resources.GridPreviewCaption; } }
		protected void ChangeColumnSelectorButtonVisibility(bool visible) {
			if(chColumnSelector != null) {
				chColumnSelector.Visible = visible;
			}
		}
		public override void InitComponent() {
			CreateMainPage();
#if DXWhidbey
			dbAdapter = CreateDBAdapter();
			if(dbAdapter != null && dbAdapter.Adapter != null) {
				try {
					dataSet = dbAdapter.DataSet.Clone();
					tableName = dbAdapter.TableName;
				} catch { }
				sbPreview.Visible = true;
			} else CreateXMLPage();
#else
			DataTable tbl = CreateDataTableAdapter();
			if(tbl != null) {
				if(tbl.DataSet != null && tbl.DataSet.Site != null) {
					try {
						dataSet = tbl.DataSet.Clone();
						tableName = tbl.TableName;
						CreateDataAdaptersPanel(tbl.DataSet.Site.Container.Components);		
					} catch {}
				}
			} else CreateXMLPage();
#endif
			previewControl = CreatePreviewControl();
			previewControl.Parent = pnlGrid;
			previewControl.Dock = DockStyle.Fill;
			SetColumnSelectorCaption(false);
			InitUnboundData();
			InitShowColumnsSortIndices();
			SetLayoutChanged(false);
		}
		XtraTabControl tbc = null;
		private void CreateMainPage() {
			tbc = new XtraTabControl();
			tbc.TabPages.CollectionChanged += OnTabPagesCollectionChanged;
			tbc.SuspendLayout();
			tbc.Dock = DockStyle.Fill;
			pnlTab.Dock = DockStyle.Fill;
			pnlTab.Controls.Add(tbc);
			XtraTabPage tp = new XtraTabPage();
			tp.Text = PreviewPanelText;
			tp.Controls.Add(pnlGrid);
			tbc.TabPages.Add(tp);
			pnlTab.Visible = true;
			tbc.ResumeLayout();
			tbc.SelectedPageChanged += new TabPageChangedEventHandler(tbc_SelectedPageChanged);
		}
		void tbc_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			ShowColumnsSortIndex = false;
			chShowSortIndex.Enabled = tbc.SelectedTabPageIndex == 0;
		}
		protected virtual bool AllowAutoHideTabHeader { get { return false; } }
		void OnTabPagesCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(!AllowAutoHideTabHeader) return;
			if(e.Action != CollectionChangeAction.Add && e.Action != CollectionChangeAction.Remove)
				return;
			XtraTabPageCollection tabs = sender as XtraTabPageCollection;
			if(tabs != null && tabs.TabControl != null) {
				tabs.TabControl.ShowTabHeader = (tabs.Count < 2) ? DefaultBoolean.False : DefaultBoolean.Default;
			}
		}
		private void CreateXMLPage() {
			XtraTabPage tp = new XtraTabPage();
			tp.Text = Properties.Resources.LoadDatafromXMLCaption;
			tp.Controls.Add(pnlXML);
			pnlXML.Dock = DockStyle.Fill;
			pnlXML.Visible = true;
			tbc.TabPages.Add(tp);
			XMLTableRows("", null);
			try {
				lbXMLTable.Font = new Font("Arial", 8, FontStyle.Bold);
			} catch {}
		}
		#endregion
		#region Grid events
		private bool chColumnsUpdate = false;
		protected void OnShowCustomizationForm() {
			chColumnsUpdate = true;
			SetColumnSelectorCaption(true);
			chColumnsUpdate = false;
		}
		protected void OnHideCustomizationForm() {
			chColumnsUpdate = true;
			SetColumnSelectorCaption(false);
			chColumnsUpdate = false;
		}
		private bool layoutChanged = false;
		protected virtual void SetLayoutChanged(bool enabled) {
			layoutChanged = enabled;
			btnApply.Enabled = enabled;
		}
		#endregion
		#region Editing
		bool showingColumnsSelector = false;
		bool showColumnsSortIndex = false;
		private void SetColumnSelectorCaption(bool showing) {
			showingColumnsSelector = showing;
			SetColumnSelectorText(showing);
		}
		protected void SetColumnSelectorCaption(string caption) {
			chColumnSelector.Text = caption;
		}
		protected virtual void SetColumnSelectorText(bool showing) {
			SetColumnSelectorCaption(showing ? Properties.Resources.HideColumnSelectorCaption : Properties.Resources.ShowColumnSelectorCaption);	
		}
		private void chColumnSelector_Click(object sender, System.EventArgs e) {
			if(chColumnsUpdate) return;
			if(!showingColumnsSelector) ShowColumnsCustomization();
			else HideColumnsCustomization();
		}
		protected virtual bool AllowShowColumnSortIndex { get { return false; } }
		protected bool ShowColumnsSortIndex { 
			get { return showColumnsSortIndex && AllowShowColumnSortIndex; }
			set {
				if(showColumnsSortIndex == value) return;
				showColumnsSortIndex = value;
				SetColumnsSortIndexCaption();
				ShowColumnsSortIndices();
			}
		}
		private void chShowSortIndex_Click(object sender, EventArgs e) {
			ShowColumnsSortIndex = !showColumnsSortIndex;
		}
		void SetColumnsSortIndexCaption() {
			chShowSortIndex.Text = ShowColumnsSortIndex ? Properties.Resources.HideColumnsSortIndex : Properties.Resources.ShowColumnsSortIndex;
		}
		protected virtual void ShowColumnsSortIndices() { }
		private void Apply() {
			currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;	
			ApplyLayouts();
			SetUnboundData();
			Cursor.Current = currentCursor;
		}
		private void btnApply_Click(object sender, System.EventArgs e) {
			Apply();
			SetLayoutChanged(false);
		}
		private void btnLoad_Click(object sender, System.EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = Localizer.Active.GetLocalizedString(StringId.RestoreLayoutDialogFileFilter);
			dlg.Title = Localizer.Active.GetLocalizedString(StringId.RestoreLayoutDialogTitle);
			if (dlg.ShowDialog() == DialogResult.OK) {
				Refresh(true);
				try {
					RestoreLayoutFromXml(dlg.FileName);
				} catch {}
				Refresh(false);
			}
		}
		private void btnSave_Click(object sender, System.EventArgs e) {
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = Localizer.Active.GetLocalizedString(StringId.SaveLayoutDialogFileFilter);
			dlg.Title = Localizer.Active.GetLocalizedString(StringId.SaveLayoutDialogTitle);
			if (dlg.ShowDialog() == DialogResult.OK) {
				Refresh(true);
				SaveLayoutToXml(dlg.FileName);
				Refresh(false);
			}
		}
		Cursor currentCursor;
		private void Refresh(bool isWait) {
			if(isWait) {
				currentCursor = Cursor.Current;
				Cursor.Current = Cursors.WaitCursor;
			} else 
				Cursor.Current = currentCursor;
			this.Refresh();
		}
		private void sbPreview_Click(object sender, System.EventArgs e) {
			if(dbAdapter == null || dbAdapter.Adapter == null) return;
			currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try {
#if DXWhidbey
				using(VS2005ConnectionStringHelper helper = new VS2005ConnectionStringHelper()) {
					helper.PatchConnectionString(dbAdapter.Adapter);
					dbAdapter.Fill(dataSet);
				}
#endif
			} catch {}
			OnFillGrid();
			SetLayoutChanged(false);
			Cursor.Current = currentCursor;
		}
		private void btnFill_Click(object sender, System.EventArgs e) {
			IDataAdapter adapter;
			currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;	
			try {
				if(pickImage1.SelectedItem != null && dataSet != null) {
					adapter = ((DevExpress.XtraEditors.Controls.ImageComboBoxItem)pickImage1.SelectedItem).Value as IDataAdapter;
					adapter.Fill(dataSet);
					OnFillGrid();
				}
			} catch {}
			SetLayoutChanged(false);
			Cursor.Current = currentCursor;
			TableRows();
		}
		private void btnClear_Click(object sender, System.EventArgs e) {
			dataSet.Clear();
			SetLayoutChanged(false);
			TableRows();
		}
		private void TableRows() {
			if(tableName != "") {
				lbTable.Text = tableName + ": " + dataSet.Tables[tableName].Rows.Count.ToString() + " " + Properties.Resources.RowCaption;
				btnClear.Enabled = dataSet.Tables[tableName].Rows.Count > 0;
			}
		}
		private void XMLTableRows(string xmlTableName, DataView dv) {
			if(xmlTableName != "" && dv != null) 
				lbXMLTable.Text = xmlTableName + ": " + dv.Count.ToString() + " " + Properties.Resources.RowCaption;
			else lbXMLTable.Text = Properties.Resources.ZeroRows;
		}
		protected string XMLName = "";
		protected string XMLTable = "";
		DataSet ds = null;
		protected DataSet XMLDataSet { 
			get {  
				if(ds == null && XMLName != "") {
					ds = new DataSet();
					ds.ReadXml(XMLName);
				}
				return ds;
			}
		}
		private void btnXMLLoad_Click(object sender, System.EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "XML files (*.xml)|*.xml";
			dlg.Title = Properties.Resources.OpenFileCaption;
			if (dlg.ShowDialog() == DialogResult.OK) {
				Refresh(true);
				gcXMLTables.Visible = false;
				try {
					ds = new DataSet();
					ds.ReadXml(dlg.FileName);
					if(ds.Tables.Count == 1) {
						XMLTableRows(dlg.FileName, ds.Tables[0].DefaultView);
						SetControlDataSource(ds.Tables[0].DefaultView);
					} else {
						gcXMLTables.Text = string.Format(Properties.Resources.SelectTableCaption, dlg.FileName);
						gcXMLTables.Visible = true;
						cbXMLTable.Properties.Items.BeginUpdate();
						cbXMLTable.Properties.Items.Clear();
						foreach(DataTable tbl in ds.Tables)
							cbXMLTable.Properties.Items.Add(tbl.TableName);
						cbXMLTable.SelectedIndex = -1;
						cbXMLTable.Properties.Items.EndUpdate();
					}
					XMLName = dlg.FileName;
				} catch(Exception ex) {
					MessageBox.Show(this, ex.Message, Properties.Resources.ErrorCaption);
				}
				Refresh(false);
			}
		}
		private void cbXMLTable_SelectedIndexChanged(object sender, System.EventArgs e) {
			string name = cbXMLTable.Text;
			if(name == "" || XMLName == "" || XMLDataSet == null) return;
			XMLTable = name;
			if(XMLDataSet.Tables.Contains(name)) {
				XMLTableRows(XMLName, XMLDataSet.Tables[name].DefaultView);
				SetControlDataSource(XMLDataSet.Tables[name].DefaultView);
			}
		}
		#endregion
	}
	public class DBAdapter {
		object adapter = null;
		DataSet dataSet = null;
		string tableName = string.Empty;
		public object Adapter { get { return adapter; } }
		public DataSet DataSet { get { return dataSet; } }
		public string TableName { get { return tableName; } }
		public DBAdapter(IList dataAdapters, object dataSource, string dataMember) {
			dataSet = DevExpress.Data.Native.BindingHelper.ConvertToDataSet(dataSource);
			tableName = DevExpress.Data.Native.BindingHelper.ConvertToTableName(dataSource, dataMember);
			adapter = SpecifyDataAdapter(dataAdapters, tableName);
		}
		public void Fill(DataSet ds) {
			try {
				IDataAdapter dataAdapter = DevExpress.Data.Native.BindingHelper.ConvertToIDataAdapter(adapter);
				if(dataAdapter != null)
					dataAdapter.Fill(ds);
			} catch { }
		}
		static object SpecifyDataAdapter(IList dataAdapters, string tableName) {
			if(tableName == null)
				return null;
			foreach(object adapter in dataAdapters) {
				IDataAdapter dataAdapter = DevExpress.Data.Native.BindingHelper.ConvertToIDataAdapter(adapter);
				if(dataAdapter == null)
					continue;
				foreach(DataTableMapping mapping in dataAdapter.TableMappings)
					if(tableName.Equals(mapping.DataSetTable))
						return adapter;
			}
			return null;
		}
	}
}
