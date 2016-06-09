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
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;
namespace DevExpress.XtraGrid.Views.Layout.Frames {
	[ToolboxItem(false)]
	public abstract class LayoutsBase : DevExpress.XtraEditors.Designer.Utils.XtraFrame {
		protected internal DevExpress.XtraEditors.SimpleButton btnSave;
		protected internal DevExpress.XtraEditors.SimpleButton btnLoad;
		protected internal DevExpress.XtraEditors.PanelControl pnlTab;
		protected internal DevExpress.XtraEditors.SimpleButton sbPreview;
		protected internal DevExpress.XtraEditors.SimpleButton btnApply;
		protected internal DevExpress.XtraEditors.SimpleButton btnOk;
		protected internal DevExpress.XtraEditors.SimpleButton btnCancel;
		private System.Windows.Forms.ImageList imageList1;
		private System.ComponentModel.IContainer components;
		protected internal DevExpress.XtraEditors.PanelControl pnlBottom;
		protected internal DevExpress.XtraEditors.PanelControl pnlGrid;
		protected internal DevExpress.XtraEditors.PanelControl pnlData;
#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutsBase));
			this.btnSave = new DevExpress.XtraEditors.SimpleButton();
			this.btnLoad = new DevExpress.XtraEditors.SimpleButton();
			this.pnlBottom = new DevExpress.XtraEditors.PanelControl();
			this.sbPreview = new DevExpress.XtraEditors.SimpleButton();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.pnlGrid = new DevExpress.XtraEditors.PanelControl();
			this.pnlData = new DevExpress.XtraEditors.PanelControl();
			this.pnlTab = new DevExpress.XtraEditors.PanelControl();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).BeginInit();
			this.pnlBottom.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlData)).BeginInit();
			this.pnlData.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlTab)).BeginInit();
			this.SuspendLayout();
			this.lbCaption.Size = new System.Drawing.Size(644, 42);
			this.pnlMain.Controls.Add(this.pnlTab);
			this.pnlMain.Controls.Add(this.pnlGrid);
			this.pnlMain.Controls.Add(this.pnlBottom);
			this.pnlMain.Controls.Add(this.pnlData);
			this.pnlMain.Location = new System.Drawing.Point(0, 24);
			this.pnlMain.Size = new System.Drawing.Size(644, 336);
			this.horzSplitter.Size = new System.Drawing.Size(644, 4);
			this.btnSave.Location = new System.Drawing.Point(140, 4);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(132, 24);
			this.btnSave.MaximumSize = new System.Drawing.Size(0, 24);
			this.btnSave.MinimumSize = new System.Drawing.Size(0, 24);
			this.btnSave.TabIndex = 2;
			this.btnSave.Text = "Sa&ve Layout...";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.btnLoad.Location = new System.Drawing.Point(4, 4);
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Size = new System.Drawing.Size(132, 24);
			this.btnLoad.MaximumSize = new System.Drawing.Size(0, 24);
			this.btnLoad.MinimumSize = new System.Drawing.Size(0, 24);
			this.btnLoad.TabIndex = 1;
			this.btnLoad.Text = "&Load Layout...";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			this.pnlBottom.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlBottom.Controls.Add(this.sbPreview);
			this.pnlBottom.Controls.Add(this.btnApply);
			this.pnlBottom.Controls.Add(this.btnOk);
			this.pnlBottom.Controls.Add(this.btnCancel);
			this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlBottom.Location = new System.Drawing.Point(0, 304);
			this.pnlBottom.Name = "pnlBottom";
			this.pnlBottom.Size = new System.Drawing.Size(644, 46);
			this.pnlBottom.TabIndex = 1;
			this.sbPreview.Location = new System.Drawing.Point(10, 9);
			this.sbPreview.Name = "sbPreview";
			this.sbPreview.Size = new System.Drawing.Size(160, 24);
			this.sbPreview.TabIndex = 3;
			this.sbPreview.Text = "&Preview Data";
			this.sbPreview.Click += new System.EventHandler(this.sbPreview_Click);
			this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOk.Location = new System.Drawing.Point(378, 9);
			this.btnOk.Name = "btnOk";
			this.btnOk.Size = new System.Drawing.Size(80, 24);
			this.btnOk.TabIndex = 4;
			this.btnOk.Text = "OK";
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(466, 9);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(80, 24);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnApply.Location = new System.Drawing.Point(554, 9);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(80, 24);
			this.btnApply.TabIndex = 2;
			this.btnApply.Text = "&Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			this.pnlGrid.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlGrid.Location = new System.Drawing.Point(0, 32);
			this.pnlGrid.Name = "pnlGrid";
			this.pnlGrid.Size = new System.Drawing.Size(644, 272);
			this.pnlGrid.TabIndex = 2;
			this.pnlData.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlData.Location = new System.Drawing.Point(108, 40);
			this.pnlData.Name = "pnlData";
			this.pnlData.Size = new System.Drawing.Size(200, 156);
			this.pnlData.TabIndex = 0;
			this.pnlData.Visible = false;
			this.pnlTab.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlTab.Padding = new Padding(5, 2, 3, 0);
			this.pnlTab.Location = new System.Drawing.Point(8, 40);
			this.pnlTab.Name = "pnlTab";
			this.pnlTab.Size = new System.Drawing.Size(96, 60);
			this.pnlTab.TabIndex = 3;
			this.pnlTab.Visible = false;
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
			this.imageList1.Images.SetKeyName(0, "");
			this.imageList1.Images.SetKeyName(1, "");
			this.Name = "LayoutsBase";
			this.Size = new System.Drawing.Size(644, 360);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlBottom)).EndInit();
			this.pnlBottom.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlData)).EndInit();
			this.pnlData.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlTab)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public LayoutsBase(int index)
			: base(index) {
			InitializeComponent();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				dataSet = null;
				dataTable = null;
			}
			base.Dispose(disposing);
		}
		protected override void InitImages() {
			base.InitImages();
			btnLoad.Image = DesignerImages16.Images[DesignerImages16LoadIndex];
			btnSave.Image = DesignerImages16.Images[DesignerImages16SaveIndex];
			btnApply.Image = DesignerImages16.Images[DesignerImages16ApplyIndex];
		}
		protected DBAdapter dbAdapter = null;
		protected DataSet dataSet;
		protected DataTable dataTable = null;
		protected bool fTryUseEdittingObjectDataSource = false;
		protected string tableName = "";
		protected Control previewControl = null;
		protected abstract Control CreatePreviewControl();
		protected abstract void ApplyLayouts();
		protected abstract void RestoreLayoutFromXml(string fileName);
		protected abstract void SaveLayoutToXml(string fileName);
		protected abstract DataTable CreateDataTableAdapter();
		protected abstract DBAdapter CreateDBAdapter();
		protected virtual void OnFillGrid() { }
		protected virtual string PreviewPanelText { get { return string.Empty; } }
		public override void InitComponent() {
			CreateMainPage();
			CreatePreviewData();
			previewControl = CreatePreviewControl();
			previewControl.Parent = pnlGrid;
			previewControl.Dock = DockStyle.Fill;
		}
		protected void CreateFakeDataSource() {
			GridColumnCollection columns = (EditingObject as LayoutView).Columns;
		}
		protected bool fFakeDataSourceUsing = false;
		protected void CreatePreviewData() {
			dbAdapter = CreateDBAdapter();
			if(dbAdapter!=null && dbAdapter.Adapter != null) {
				try {
					dataSet = dbAdapter.DataSet.Clone();
					tableName = dbAdapter.TableName;
					dataTable = dataSet.Tables[tableName];
					dbAdapter.Fill(dataSet);
				}
				catch { }
			} else {
				if(!CanUseEditingViewDataSource()) {
					dataSet = new DataSet("PreviewDataSet");
					tableName = "PreviewTable";
					dataTable = CreateColumnViewTable(dataSet, tableName, EditingObject as ColumnView);
					fFakeDataSourceUsing = true;
				} else fTryUseEdittingObjectDataSource=true;
			}
		}
		protected bool CanUseEditingViewDataSource() {
			LayoutView view = EditingObject as LayoutView;
			return view!=null && view.DataSource!=null && view.RecordCount > 2 && view.RecordCount <= 50;
		}
		protected void CreateMainPage() {
			XtraTabControl tbc = new XtraTabControl();
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
		}
		protected void SetLayoutChanged(bool enabled) {
			btnApply.Enabled = enabled;
		}
		protected internal void btnApply_Click(object sender, System.EventArgs e) {
			Apply();
		}
		protected internal void btnLoad_Click(object sender, System.EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "XML files (*.xml)|*.xml|All files|*.*";
			dlg.Title = "Restore Layout";
			if(dlg.ShowDialog() == DialogResult.OK) {
				using(new WaitCursor()) {
					Refresh();
					try {
						RestoreLayoutFromXml(dlg.FileName);
					}
					catch { }
					Refresh();
				}
			}
		}
		protected internal void btnSave_Click(object sender, System.EventArgs e) {
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "XML files (*.xml)|*.xml";
			dlg.Title = "Save Layout";
			if(dlg.ShowDialog() == DialogResult.OK) {
				using(new WaitCursor()) {
					Refresh();
					SaveLayoutToXml(dlg.FileName);
					Refresh();
				}
			}
		}
		protected virtual void Apply() {
			using(new WaitCursor()) {
				ApplyLayouts();
			}
		}
		protected internal void sbPreview_Click(object sender, System.EventArgs e) {
			using(new WaitCursor()) {
				try {
					if(fFakeDataSourceUsing) FIllColumnViewDataTable(dataTable, EditingObject as ColumnView);
				}
				catch { }
				OnFillGrid();
			}
		}
		protected DataTable CreateColumnViewTable(DataSet dataSet, string tableName, ColumnView columnView) {
			DataTable table = dataSet.Tables.Add(tableName);
			CreateFIllColumnViewDataTable(table, columnView);
			return table;
		}
		protected void CreateFIllColumnViewDataTable(DataTable table, ColumnView columnView) {
			table.BeginInit();
			UserTableCreator creator = new UserTableCreator(table, columnView);
			creator.CreateColumns();
			creator.Fill();
			table.EndInit();
		}
		protected void FIllColumnViewDataTable(DataTable table, ColumnView columnView) {
			table.BeginInit();
			UserTableCreator creator = new UserTableCreator(table, columnView);
			creator.Fill();
			table.EndInit();
		}
	}
	class WaitCursor : IDisposable {
		Cursor currentCursor;
		public WaitCursor() { SetWaitCursor(); }
		public void Dispose() { RestoreCursor(); }
		protected void SetWaitCursor() {
			currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
		}
		protected void RestoreCursor() {
			Cursor.Current = currentCursor;
		}
	}
	public class UserTableCreator {
		readonly int AddRowCount = 10;
		DataTable table;
		ColumnView columnView;
		Random random;
		public UserTableCreator(DataTable table, ColumnView columnView) {
			this.table = table;
			this.columnView = columnView;
			random = new Random();
		}
		public DataTable Table { get { return table; } }
		public ColumnView ColumnView { get { return columnView; } }
		public void Fill() {
			int iStartRowCount = Table.Rows.Count+1;
			for(int i = 0; i < AddRowCount; i++) {
				try { AddRow(iStartRowCount+i); }
				catch { }
			}
		}
		public void CreateColumns() {
			for(int i = 0; i < ColumnView.Columns.Count; i++)
				CreateColumn(ColumnView.Columns[i]);
		}
		protected void CreateColumn(GridColumn column) {
			try {
				DataColumn dataColumn = SafeCreateColumn(column.FieldName, column.ColumnType);
				Table.Columns.Add(dataColumn);
				dataColumn.AllowDBNull = true;
			}
			catch { }
		}
		protected bool IsNullableColumnType(Type columnType) {
#if DXWhidbey
			return Nullable.GetUnderlyingType(columnType) != null;
#else
			return true;
#endif
		}
		protected Type SafeGetColumnType(Type columnType) {
#if DXWhidbey
			Type canBeNullable = Nullable.GetUnderlyingType(columnType);
#else
			Type canBeNullable = columnType;
#endif
			return canBeNullable == null ? columnType: canBeNullable;
		}
		protected DataColumn SafeCreateColumn(string name, Type columnType) {
			DataColumn column = new DataColumn(name, SafeGetColumnType(columnType));
			if(IsNullableColumnType(columnType)) column.AllowDBNull = true;
			return column;
		}
		protected void AddRow(int rowIndex) {
			DataRow row = Table.Rows.Add(new object[] { });
			for(int i = 0; i < Table.Columns.Count; i++)
				row[i] = GetColumnValue(rowIndex, i);
		}
		protected object GetColumnValue(int rowIndex, int columnIndex) {
			return GetColumnValue(rowIndex, Table.Columns[columnIndex]);
		}
		protected object GetColumnValue(int rowIndex, DataColumn column) {
			int randomValue = random.Next(1, AddRowCount);
			if(column.AutoIncrement) return rowIndex + 1;
			if(column.DataType == typeof(string)) return "Test " + randomValue.ToString();
			if(column.DataType == typeof(int)) return randomValue;
			if(column.DataType == typeof(bool)) return randomValue % 2 == 1;
			if(column.DataType == typeof(DateTime)) return DateTime.Today.AddDays(-randomValue);
			if(column.DataType == typeof(double)) return (double)randomValue;
			if(column.DataType == typeof(decimal)) return (decimal)randomValue;
			return DBNull.Value;
		}
	}
	public class DBAdapter {
		object adapter = null;
		DataSet dataSet = null;
		string tableName = string.Empty;
		public DBAdapter(IList dataAdapters, object dataSource, string dataMember) {
			dataSet = DevExpress.Data.Native.BindingHelper.ConvertToDataSet(dataSource);
			tableName = DevExpress.Data.Native.BindingHelper.ConvertToTableName(dataSource, dataMember);
			adapter = SpecifyDataAdapter(dataAdapters, tableName);
		}
		public object Adapter { get { return adapter; } }
		public DataSet DataSet { get { return dataSet; } }
		public string TableName { get { return tableName; } }
		public void Fill(DataSet ds) {
			try {
				IDataAdapter dataAdapter = DevExpress.Data.Native.BindingHelper.ConvertToIDataAdapter(adapter);
				if(dataAdapter != null) dataAdapter.Fill(ds);
			}
			catch { }
		}
		protected object SpecifyDataAdapter(IList dataAdapters, string tableName) {
			if(tableName == null) return null;
			foreach(object adapter in dataAdapters) {
				IDataAdapter dataAdapter = DevExpress.Data.Native.BindingHelper.ConvertToIDataAdapter(adapter);
				if(dataAdapter == null) continue;
				foreach(DataTableMapping mapping in dataAdapter.TableMappings) {
					if(tableName.Equals(mapping.DataSetTable)) return adapter;
				}
			}
			return null;
		}
	}
}
