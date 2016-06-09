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
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Frames;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.FeatureBrowser;
using DevExpress.XtraEditors.Designer.Utils;
namespace DevExpress.XtraGrid.FeatureBrowser {
	[ToolboxItem(false)]
	public class FeatureBrowserGridForm : FeatureBrowserFormBase {
		private System.ComponentModel.IContainer components = null;
		public const string XmlColumnTag = "Columns";
		public const string XmlBandTag = "Bands";
		public const string XmlGroupSummariesTag = "GroupSummaries";
		public const string XmlFormatConditionTag = "FormatCondition";
		ColumnDesigner columnDesigner;
		BandDesigner bandDesigner;
		DevExpress.XtraTab.XtraTabControl tabInfo;
		DevExpress.XtraTab.XtraTabPage pageInfo;
		DevExpress.XtraTab.XtraTabPage pageColumnsInfo;
		DevExpress.XtraTab.XtraTabPage pageBandsInfo;
		DevExpress.XtraTab.XtraTabPage pageGroupSummariesInfo;
		DevExpress.XtraTab.XtraTabPage pageFormatConditionInfo;
		SampleGridControl gridControl;
		SampleGridControl userGridControl;
		DevExpress.XtraTab.XtraTabPage pageGridSample;
		DevExpress.XtraTab.XtraTabPage pageUserGrid;
		DevExpress.XtraTab.XtraTabControl tabSample;
		DevExpress.XtraTab.XtraTabPage oldSelectedSamplePage;
		public FeatureBrowserGridForm() {
			InitializeComponent();
			SetupTabInfo();
			SetupTabSample();
		}
		void SetupTabInfo() {
			this.tabInfo = new DevExpress.XtraTab.XtraTabControl();
			this.tabInfo.Parent = this.pnlMain.Panel1;
			this.tabInfo.Dock = DockStyle.Fill;
			this.pageInfo = tabInfo.TabPages.Add(DevExpress.XtraGrid.Design.Properties.Resources.GeneralCaption);
			this.pageInfo.Padding = new Padding(10, 6, 10, 10);
			this.pageColumnsInfo = tabInfo.TabPages.Add(DevExpress.XtraGrid.Design.Properties.Resources.ColumnsCaption);
			this.pageColumnsInfo.Padding = new Padding(10, 6, 10, 10);
			this.pageGroupSummariesInfo = null;
			this.pageFormatConditionInfo = null;
			this.defaultPageDesigner.Parent = pageInfo;
			this.columnDesigner = new ColumnDesigner();
			this.columnDesigner.Parent = pageColumnsInfo;
			this.columnDesigner.Dock = DockStyle.Fill;
			this.tabInfo.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(OnTabInfoPageChange);
		}
		void SetupTabSample() {
			this.tabSample = new DevExpress.XtraTab.XtraTabControl();
			this.tabSample.Parent = this.pnlMain.Panel2;
			this.tabSample.Dock = DockStyle.Fill;
			this.pageGridSample = this.tabSample.TabPages.Add(DevExpress.XtraGrid.Design.Properties.Resources.SampleViewCaption);
			this.pageUserGrid = this.tabSample.TabPages.Add(DevExpress.XtraGrid.Design.Properties.Resources.YourViewCaption);
			this.gridControl = new SampleGridControl();
			this.gridControl.Parent = this.pageGridSample;
			this.gridControl.Dock = DockStyle.Fill;
			this.gridControl.DataSourceChanged += new EventHandler(OnSampleGridDataSourceChanged);
			this.userGridControl = new SampleGridControl();
			this.userGridControl.Parent = this.pageUserGrid;
			this.userGridControl.Dock = DockStyle.Fill;
			this.userGridControl.DataSourceChanged += new EventHandler(OnUserGridDataSourceChanged);
		}
		public ColumnView ColumnView { get { return SourceObject as ColumnView; } }
		public GridControl SourceGridControl { get { return ColumnView.GridControl; } }
		public ColumnView UserColumnView { get { return UserGridControl != null ? UserGridControl.MainView as ColumnView : null; } }
		public GridControl GridControl { get { return gridControl; } }
		public GridControl UserGridControl { get { return userGridControl; } }
		public GridControl CurrentSampleGridControl { get { return tabSample.SelectedTabPage == pageGridSample ? GridControl : UserGridControl; } }
		protected override void OnSourceObjectChanged() {
			if(this.tabInfo.SelectedTabPage == this.pageBandsInfo) 
				ReloadBandUserGrid(false);
			base.OnSourceObjectChanged();
			if(ColumnView != null && ColumnView.Name != "")
				this.pageUserGrid.Text = string.Format(DevExpress.XtraGrid.Design.Properties.Resources.YourViewNameCaption, ColumnView.Name);
			SetupBandsPage();
			SetupColumnsPage();
			SetupGroupSummariesPage();
			SetupFormatConditionPage();
		}
		protected override object[] CreateSampleObjects() {
			if(ColumnView == null) return null;
			DevExpress.XtraGrid.Design.GridAssign.RegisterViews(SourceGridControl, userGridControl);
			DevExpress.XtraGrid.Design.GridAssign.RegisterViews(SourceGridControl, gridControl);
			SetupSampleGridControl();
			if(UserColumnView == null || UserColumnView.Columns.Count == 0) {
				SetupUserGridControl();
			}
			return new object[2] {GridControl.MainView, UserGridControl.MainView};
		}
		object gridControlDataSource = null;
		string gridControlDataMember = string.Empty;
		void SetupSampleGridControl() {
			gridControl.SynchronizedDataSources = true;
			gridControlDataSource = null;
			SetupGridControl(GridControl);
			CreateColumnView(GridControl);
			CreateSampleViewColumns();
			gridControlDataSource = GridControl.DataSource;
			gridControlDataMember = GridControl.DataMember;
			gridControl.SynchronizedDataSources = false;
		} 
		void SetupUserGridControl() {
			userGridControl.SynchronizedDataSources = true;
			SetupGridControl(UserGridControl);
			CreateColumnView(UserGridControl);
			SetupUserColumnView();
			CreateUserDataSource();
			userGridControl.SynchronizedDataSources = false;
		}
		object userGridControlDataSource = null;
		void CreateUserDataSource() {
			userGridControlDataSource = null;
			UserGridControl.DataSource = CreateColumnViewTable(UserColumnView);
			userGridControlDataSource = UserGridControl.DataSource;
		}
		void OnSampleGridDataSourceChanged(object sender, EventArgs e) {
			if(gridControlDataSource == null) return;
			if(GridControl.DataSource != gridControlDataSource || GridControl.DataMember != gridControlDataMember)
				SetupSampleGridControl();
		}
		void OnUserGridDataSourceChanged(object sender, EventArgs e) {
			if(userGridControlDataSource == null) return;
			if(GridControl.DataSource != userGridControlDataSource) {
				CreateUserDataSource();
				this.columnDesigner.InitFrame(ColumnView, string.Empty, null);
			}
		}
		protected override void OnFeatureNameChanged() {
			base.OnFeatureNameChanged();
			this.tabInfo.SelectedTabPageIndex = 0;
		}
		protected override void OnSourceObjectChanged(IEmbeddedFrame frame, PropertyValueChangedEventArgs e) {
			base.OnSourceObjectChanged(frame, e);
			if(e.ChangedItem == null) return;
			if(frame == defaultPageDesigner)
				CheckApperancePropertiesChanges(e.ChangedItem.PropertyDescriptor, GetFullPathByPropertyGridItem(e.ChangedItem), UnsupportedApperanceObjects);
			if(frame == columnDesigner || frame == bandDesigner)
				CheckApperancePropertiesChanges(e.ChangedItem.PropertyDescriptor, GetFullPathByPropertyGridItem(e.ChangedItem), UnsupportedColumnApperanceObjects);
		}
		void CheckApperancePropertiesChanges(PropertyDescriptor propDescriptor, string fullPath, string[] appearanceNames) {
			if(propDescriptor.ComponentType != typeof(AppearanceObject)
				&& !(propDescriptor.ComponentType.IsSubclassOf(typeof(AppearanceObject)))) return;
			if(ColumnView.ActivePaintStyleName != "Skin" && ColumnView.ActivePaintStyleName != "WindowsXP") return;
			string[] properties = UnsupportedAppearanceProperties;
			bool isUnsuported = false;
			for(int i = 0; i < properties.Length; i ++) {
				if(properties[i] == propDescriptor.Name) {
					isUnsuported = true;
					break;
				}
			}
			if(isUnsuported && appearanceNames != null && appearanceNames.Length > 0) {
				isUnsuported = false;
				for(int i = 0; i < appearanceNames.Length; i ++)
					if(fullPath.IndexOf(appearanceNames[i] + '.') > -1) {
						isUnsuported = true;
						break;
					}
			}
			if(isUnsuported) {
				ShowMessage("ApperanceAndPaintStyle", "The currently applied paint style doesn't allow the background style settings of this element to be changed. Only the font settings and text layout options can be changed. " +
					"Please select a different style either by setting the [{gotoFeature:Appearance_GridControl}GridControl LookAndFeel.ActiveStyle] property or the [{gotoFeature:Appearance_ViewLookAndFeel}current view's PaintStyleName] property. " + 
					"The WindowsXP style and the skin style do not allow their background style settings to be modified.", true);
			}
		}
		string[] UnsupportedAppearanceProperties { get { return new string[] {"BackColor", "BackColor2"}; } }
		string[] UnsupportedApperanceObjects { 
			get { 
			   return new string[] {"BandPanel", "ColumnFilterButton", "ColumnFilterButtonActive", 
					"FilterCloseButton", "GroupButton", "HeaderPanel"}; 
			} 
		} 
		string[] UnsupportedColumnApperanceObjects {
			get { return new string[] {"AppearanceHeader"}; }
		}
		void OnTabInfoPageChange(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e) {
			OnTabInfoSelectedPageChanged(e.PrevPage);
		}
		void ReloadBandUserGrid(bool recreate) {
			if(this.bandDesigner != null && ColumnView != null) {
				this.bandDesigner.SetLayout();
				DisposeMainView(this.userGridControl);
				if(recreate)
					base.OnSourceObjectChanged();
			}
		}
		protected virtual void OnTabInfoSelectedPageChanged(DevExpress.XtraTab.XtraTabPage prevPage) {
			if(this.tabInfo.SelectedTabPage == pageBandsInfo || prevPage == pageBandsInfo) {
				this.pnlMain.PanelVisibility = this.tabInfo.SelectedTabPage == pageBandsInfo ? SplitPanelVisibility.Panel1 : SplitPanelVisibility.Both;
				if(prevPage == pageBandsInfo) {
					ReloadBandUserGrid(true);
				}
				return;
			}
			if(prevPage != this.pageInfo && this.tabInfo.SelectedTabPage != this.pageInfo) return;
			if(prevPage == this.pageInfo) {
				oldSelectedSamplePage = this.tabSample.SelectedTabPage;
				if(oldSelectedSamplePage != this.pageUserGrid)
					this.tabSample.SelectedTabPage = this.pageUserGrid;
			} else {
				this.tabSample.SelectedTabPage = oldSelectedSamplePage;
			}
		}
		protected override void OnLabelInfoItemClickGoto(string gotoName, string gotoValue) {
			if(gotoName == "") return;
			for(int i = 0; i < this.tabInfo.TabPages.Count; i ++)
				if(this.tabInfo.TabPages[i].Text == gotoName) {
					this.tabInfo.SelectedTabPageIndex = i;
					if(gotoValue != string.Empty)
						OnLabelInfoItemClickGotoValue(gotoName, gotoValue);
					return;
				}
			if(gotoName == "Preview") {
				if(GridControl.IsPrintingAvailable)
					CurrentSampleGridControl.ShowPrintPreview();
				else System.Windows.Forms.MessageBox.Show(DevExpress.XtraGrid.Design.Properties.Resources.PrintingWarning);
			}
			else base.OnLabelInfoItemClickGoto(gotoName, gotoValue);
		}
		protected virtual void OnLabelInfoItemClickGotoValue(string gotoName, string gotoValue) {
			if(gotoName == this.pageColumnsInfo.Text) {
				(this.columnDesigner as IEmbeddedFrame).SelectProperty(gotoValue);
			}
			if(this.bandDesigner != null && gotoName == this.pageBandsInfo.Text) {
				(this.bandDesigner as IEmbeddedFrame).SelectProperty(gotoValue);
			}
		}
		protected void DisposeMainView(GridControl gridControl) {
			if(gridControl.MainView != null) {
				BaseView view = gridControl.MainView;
				gridControl.MainView = null;
				view.Dispose();
			}
		}
		protected virtual void CreateColumnView(GridControl gridControl) {
			ConstructorInfo constructorInfoObj = ColumnView.GetType().GetConstructor(Type.EmptyTypes);
			ColumnView sampleView = constructorInfoObj.Invoke(null) as ColumnView;
			sampleView.GridControl = gridControl;
			sampleView.Assign(ColumnView, false);
			DisposeMainView(gridControl);
			gridControl.MainView = sampleView;
		}
		protected virtual void CreateSampleViewColumns() {
			(GridControl.MainView as ColumnView).BeginDataUpdate();
			try {
				GridControl.RepositoryItems.Clear();
				(GridControl.MainView as ColumnView).Columns.Clear();
				DevExpress.XtraGrid.Design.XViewsPrinting xv = new DevExpress.XtraGrid.Design.XViewsPrinting(GridControl);
			} finally {
				(GridControl.MainView as ColumnView).EndDataUpdate();
			}
		}
		protected virtual void SetupUserColumnView() {
			for(int i = 0; i < UserColumnView.Columns.Count; i ++)
				if(ColumnView.Columns.Count > i)
				UserColumnView.Columns[i].Tag = ColumnView.Columns[i];
			UserColumnView.Layout += new EventHandler(OnUserViewColumnChanged);
		}
		protected virtual void SetupGridControl(GridControl grid) {
			grid.LookAndFeel.ParentLookAndFeel = SourceGridControl.LookAndFeel;
			grid.UseEmbeddedNavigator = SourceGridControl.UseEmbeddedNavigator;
		}
		protected virtual void UserViewColumnsChanged() {
			for(int i = 0; i < UserColumnView.Columns.Count; i ++)
				UserViewColumnChanged(UserColumnView.Columns[i]);
		}
		void UserViewColumnChanged(GridColumn column) {
			GridColumn sourceColumn = column.Tag as GridColumn;
			if(sourceColumn != null) {
				if(sourceColumn.Width != column.Width)
					sourceColumn.Width = column.Width;
				if(sourceColumn.SortIndex != column.SortIndex)
					sourceColumn.SortIndex = column.SortIndex;
				if(sourceColumn.GroupIndex != column.GroupIndex)
					sourceColumn.GroupIndex = column.GroupIndex;
				if(sourceColumn.VisibleIndex != column.VisibleIndex)
					sourceColumn.VisibleIndex = column.VisibleIndex;
				if(sourceColumn.SummaryItem.SummaryType != column.SummaryItem.SummaryType)
					sourceColumn.SummaryItem.SummaryType = column.SummaryItem.SummaryType;
			}
		}
		void SetupColumnsPage() {
			pageColumnsInfo.PageVisible = Pages[XmlColumnTag] != null;
			if(Pages[XmlColumnTag] == null || ColumnView == null) return;
			SetupEmbeddedFrame(columnDesigner, Pages[XmlColumnTag], UserColumnView);
		}
		void SetupBandsPage() {
			if(this.pageBandsInfo != null) {
				this.pageBandsInfo.Dispose();
				this.pageBandsInfo = null;
			}
			this.bandDesigner = null;
			bool show = (Pages[XmlBandTag] != null) && ColumnView is BandedGridView;
			this.bandDesigner = show ? new BandDesigner() : null;
			if(this.bandDesigner != null) {
				SetupEmbeddedFrame(bandDesigner, Pages[XmlBandTag], null);
				SetupAdditionalPage(ref pageBandsInfo, DevExpress.XtraGrid.Design.Properties.Resources.BandsCaption, bandDesigner);
			}
		}
		void SetupGroupSummariesPage() {
			GroupSummary groupSummaryDesigner = 
				UserColumnView is GridView && (Pages[XmlGroupSummariesTag] != null) ? new GroupSummary() : null;
			SetupAdditionalPage(ref pageGroupSummariesInfo, DevExpress.XtraGrid.Design.Properties.Resources.GroupSummaryCaption, groupSummaryDesigner);
			if(groupSummaryDesigner != null)
				SetupEmbeddedFrame(groupSummaryDesigner, Pages[XmlGroupSummariesTag], UserColumnView);
		}
		void SetupFormatConditionPage() {
			StyleFormatConditionFrame formatConditionDesigner = Pages[XmlFormatConditionTag] != null ? new StyleFormatConditionFrame() : null;
			SetupAdditionalPage(ref pageFormatConditionInfo, DevExpress.XtraGrid.Design.Properties.Resources.FormatConditionCaption, formatConditionDesigner);
			if(formatConditionDesigner != null)
				SetupEmbeddedFrame(formatConditionDesigner, Pages[XmlFormatConditionTag], UserColumnView);
		}
		void SetupEmbeddedFrame(IEmbeddedFrame frame, FeatureBrowserItemPage page, object sampleObject) {
			object[] sampleObjects = sampleObject != null ? new object[] {sampleObject} : null;
			SetupEmbeddedFrame(frame, page, ColumnView, sampleObjects, string.Empty);
			frame.ShowPropertyGridToolBar(false);
			frame.SetPropertyGridSortMode(PropertySort.Alphabetical);
		}
		void SetupAdditionalPage(ref DevExpress.XtraTab.XtraTabPage page, string caption, XtraFrame frame) {
			if(page != null) {
				page.Dispose();
				page = null;
			}
			if(frame != null) {
				page = tabInfo.TabPages.Add(caption);
				frame.Parent = page;
				frame.Dock = DockStyle.Fill;
				page.Padding = new Padding(10, 6, 10, 10);
			}
		}
		FilterObject CreateColumnFilterObject(int columnIndex) {
			if(UserColumnView != null && columnIndex < UserColumnView.Columns.Count)
				return new FilterObject(ColumnView.Columns[columnIndex], new object[1] {UserColumnView.Columns[columnIndex]}, Pages[XmlColumnTag].Properties);
			else return new FilterObject(ColumnView.Columns[columnIndex], null, Pages[XmlColumnTag].Properties);
		}
		DataTable CreateColumnViewTable(ColumnView columnView) {
			DataTable table = new DataTable();
			table.BeginInit();
			UserTableCreator creator = new UserTableCreator(table, ColumnView);
			creator.CreateColumns();
			creator.Fill();
			table.EndInit();
			return table;
		}
		void OnUserViewColumnChanged(object sender, EventArgs e) {
			UserViewColumnsChanged();
		}
		void SetupSampleViewForSorting() {
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Designer generated code
		private void InitializeComponent()	{
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			this.SuspendLayout();
			this.pnlMain.Location = new System.Drawing.Point(1, 82);
			this.pnlMain.Size = new System.Drawing.Size(630, 509);
			this.Name = "FeatureBrowserGridForm";
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
	}
	public class ListColumn {
		FilterObject filterObject;
		public ListColumn(FilterObject filterObject) {
			this.filterObject = filterObject;
		}
		public FilterObject FilterObject { get { return filterObject; } }
		public GridColumn Column { get { return FilterObject.SourceObject as GridColumn; } }
		public override string ToString() { 
			return Column != null ? Column.Caption : ""; 
		} 
	}
	public class UserTableCreator {
		const int RowCount = 10;
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
		public void CreateColumns() {
			for(int i = 0; i < ColumnView.Columns.Count; i ++)
				CreateColumn(ColumnView.Columns[i]);
		}
		public void Fill() {
			for(int i = 0; i < RowCount; i ++)
				AddRow(i);
		}
		void CreateColumn(GridColumn column) {
			try {
			DataColumn dataColumn = Table.Columns.Add(column.FieldName, GetColumnType(column));
			dataColumn.AllowDBNull = true;
			} catch { }
		}
		Type GetColumnType(GridColumn column) {
			return column.ColumnType;
		}
		void AddRow(int rowIndex) {
			DataRow row = Table.Rows.Add(new object[] {});
			for(int i = 0; i < Table.Columns.Count; i ++)
				row[i] = GetColumnValue(rowIndex, i);
		}
		object GetColumnValue(int rowIndex, int index) {
			if(ColumnView.Columns[index].ColumnEdit == null)
				return GetColumnValue(rowIndex, Table.Columns[index]);
			else return GetColumnValue(rowIndex, Table.Columns[index]);
		}
		object GetColumnValue(int rowIndex, DataColumn column) {
			int randomValue = random.Next(1, RowCount);
			if(column.AutoIncrement)
				return rowIndex + 1;
			if(column.DataType == typeof(string)) 
				return "Test " + randomValue.ToString();
			if(column.DataType == typeof(int)) 
				return randomValue;
			if(column.DataType == typeof(bool)) 
				return randomValue % 2 == 1;
			if(column.DataType == typeof(DateTime)) {
				DateTime d = DateTime.Today;
				return d.AddDays(-randomValue);
			}
			if(column.DataType == typeof(double)) {
				return (double)randomValue;
			}
			if(column.DataType == typeof(decimal)) {
				return (decimal)randomValue;
			}
			return DBNull.Value;
		}
	}
	class SampleGridControl : GridControl {
		internal bool SynchronizedDataSources = true;
		public override object DataSource {
			get { return base.DataSource; }
			set {
				if(SynchronizedDataSources)
					base.DataSource = value;
			}
		}
		public override string DataMember {
			get { return base.DataMember; }
			set {
				if(SynchronizedDataSources)
					base.DataMember = value;
			}
		}
	}
}
