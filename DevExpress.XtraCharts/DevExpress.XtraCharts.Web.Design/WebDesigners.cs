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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.Data.ChartDataSources;
using DevExpress.Data.Native;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.About;
using DevExpress.Utils.Design;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Web;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraEditors;
using EnvDTE;
namespace DevExpress.XtraCharts.Design {
	[System.Security.SecuritySafeCritical]
	public class WebChartControlDesigner : ASPxDataWebControlDesigner, IWebChartDesigner {
		static WebChartControlDesigner() {
		}
		ChartDesigner chartDesigner;
		WebChartDesignerActionList webChartDesignerActionList;
		bool suspendBinding = false;
		public Chart Chart { 
			get { 
				IChartContainer container = Component as IChartContainer;
				return container == null ? null : container.Chart;
			} 
		}
		public override DesignerAutoFormatCollection AutoFormats { get { return new DesignerAutoFormatCollection(); } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if (Component != null) {
				list.Add(CreateDataActionList());
				list.Add(webChartDesignerActionList);
				list.Add(CreateCommonActionList());
			}
		}
		WebChartControlDesigner() : base() {
		}
		void DataBind(IEnumerable data) {
			if (!suspendBinding)
				Chart.DataContainer.DataSource = data;
		}
		void SelectData() {
			IDataSource dataSource = GetIDataSource();
			if (dataSource == null)
				return;
			DataSourceView view = GetView(dataSource);
			if (view == null)
				return;
			string defaultPath = String.Empty;
			AccessDataSource accessDataSource = dataSource as AccessDataSource;
			SqlDataSource sqlDataSource = dataSource as SqlDataSource;
			if (accessDataSource == null) {
				if (sqlDataSource != null) {
					defaultPath = sqlDataSource.ConnectionString;
					using (VS2005ConnectionStringHelper helper = new VS2005ConnectionStringHelper())
						sqlDataSource.ConnectionString = helper.GetPatchedConnectionString(DesignerHost, defaultPath);
				}
			}
			else if (accessDataSource.DataFile.StartsWith("~")) {
				defaultPath = accessDataSource.DataFile;
				accessDataSource.DataFile = GetLocalPath() + "\\" + defaultPath.Substring(1);
			}
			try {
				view.Select(DataSourceSelectArguments.Empty, new DataSourceViewSelectCallback(DataBind));
			}
			catch {
			}
			finally {
				if (defaultPath != String.Empty) {
					if (accessDataSource != null)
						accessDataSource.DataFile = defaultPath;
					else if (sqlDataSource != null)
						sqlDataSource.ConnectionString = defaultPath;
				}
			}
		}
		void SetVerbsState() {
			if (webChartDesignerActionList != null && Chart != null) {
				webChartDesignerActionList.PopulateEnabled = Chart.DataContainer.CanFillDataSource();
				webChartDesignerActionList.ClearDataSourceEnabled = Chart.DataContainer.CanClearDataSource();
				webChartDesignerActionList.DataSnapshotEnabled = Chart.DataContainer.CanPerformDataSnapshot();
			}
		}
		void SetDesignModeProjectPath(WebChartControl chartControl) {
			ProjectItem projectItem = GetService(typeof(ProjectItem)) as ProjectItem;
			if (projectItem != null)
				ChartDesignHelper.InvokeMethod(chartControl, "SetDesignTimeProjectPath",
					new string[] { DTEHelper.GetFullPath(projectItem.ContainingProject) });
		}
		void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs args) {
			UpdateDesignTimeHtml();
		}
		string GetLocalPath() {
			ProjectItem projectItem = GetService(typeof(ProjectItem)) as ProjectItem;
			if (projectItem != null) {
				string path = DTEHelper.GetFullPath(projectItem);
				if (File.Exists(path))
					path = Path.GetDirectoryName(path);
				return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			}
			return String.Empty;
		}
		string IWebChartDesigner.GetImageUrl(string oldUrl) {
			ImageUrlEditor editor = new ImageUrlEditor();
			return (string)editor.EditValue(DesignerHost, oldUrl);
		}
		DataView GetEmptyData() {
			if (DataSourceDesigner != null) {
				suspendBinding = true;
				try {
					if (DataSourceDesigner.CanRefreshSchema)
						DataSourceDesigner.RefreshSchema(false);
				}
				finally {
					suspendBinding = false;
				}
				IDataSourceViewSchema schema = ((IDataBindingSchemaProvider)this).Schema;
				if (schema != null) {
					IDataSourceFieldSchema[] fields = schema.GetFields();
					DataTable table = new DataTable();
					foreach (IDataSourceFieldSchema field in fields)
						table.Columns.Add(new DataColumn(field.Name, field.DataType));
					return CreateDataView(table);
				}
			}
			return null;
		}
		DataView CreateDataView(DataTable table) {
			IDataSource dataSource = GetIDataSource();
			if (dataSource != null) {
				DataSourceView view = GetView(dataSource);
				if (view is IPivotGrid)
					return new PivotGridDataView(DataSourceID, table, (IPivotGrid)view);
				else if(view is IChartDataSource)
					return new ChartDataSourceView(DataSourceID, table, (IChartDataSource)view);
			}
			return new DataView(table);
		}
		DataSourceView GetView(IDataSource dataSource) {
			DataSourceView view = dataSource.GetView(DataMember);
			if (view is XmlDataSourceView)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgInvalidDataSource));
			return view;
		}
		IDataSource GetIDataSource() {
			if (DesignerHost != null)
				foreach (IComponent component in DesignerHost.Container.Components)
					if (component.Site.Name == DataSourceID && component is IDataSource)
						return (IDataSource)component;
			return null;
		}
		internal void OnPopulate() {
			Cursor currentCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
			try {
				SelectData();
				SetVerbsState();
			}
			finally {
				Cursor.Current = currentCursor;
			}
		}
		internal void OnClearDataSource() {
			DataBind(GetEmptyData());
			((IChartContainer)Component).RenderProvider.Invalidate();
			SetVerbsState();
		}
		internal void OnDataSnapshot() {
			Chart.DataSnapshot();
			DataSourceID = String.Empty;
			XtraMessageBox.Show((UserLookAndFeel)((IChartContainer)Component).RenderProvider.LookAndFeel, ChartLocalizer.GetString(ChartStringId.MsgDataSnapshot), 
				ID, MessageBoxButtons.OK, MessageBoxIcon.Information);
			SetVerbsState();
		}
		internal void OnWizard() {
			DesignerTransaction transaction = null;
			try {
				if (DesignerHost != null) {
					transaction = DesignerHost.CreateTransaction(ChartLocalizer.GetString(ChartStringId.TrnChartWizard));
					if (transaction != null)
						((IChartContainer)Component).Changing();
				}
				DevExpress.XtraCharts.Designer.ChartDesigner wizard = new DevExpress.XtraCharts.Designer.ChartDesigner((IChartContainer)Component, null);
				if (wizard.ShowDialog() == DialogResult.OK) {
					if (transaction != null) {
						((IChartContainer)Component).Changed();
						transaction.Commit();
					}
					return;
				}
			}
			catch {
			}
			if (transaction != null)
				transaction.Cancel();
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new WebChartCommonActionList(this);
		}
		protected override void OnAnyComponentChanged(object sender, ComponentChangedEventArgs ce) {
			base.OnAnyComponentChanged(sender, ce);
			if (Chart != null) {
				Chart.ResetGraphicsCache();
				Chart.InvalidateDrawingHelper();
			}
			SetVerbsState();
		}
		protected override void PreFilterEvents(IDictionary events) {
			base.PreFilterEvents(events);
			events.Remove("DataBound");
		}
		protected override void PreFilterProperties(IDictionary properties) {
			base.PreFilterProperties(properties);
			properties.Remove("Expressions");
		}
		protected override void PostFilterProperties(IDictionary properties) {
			base.PostFilterProperties(properties);
			ChartDesigner.PostFilterProperties(Component, properties);
		}
		protected override void OnDataSourceChanged(bool forceUpdateView) {
			if (!suspendBinding) {
				base.OnDataSourceChanged(forceUpdateView);
				DataBind(GetEmptyData());
				SetVerbsState();
			}
		}
		protected override void DataBind(ASPxDataWebControlBase dataControl) {
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				IDesignerHost designerHost = DesignerHost;
				if (designerHost != null)
					designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
				if (chartDesigner != null)
					chartDesigner.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override System.Web.UI.Control CreateViewControl() {
			System.Web.UI.Control viewControl = base.CreateViewControl();
			WebChartControl chartControl = viewControl as WebChartControl;
			if (chartControl != null) {
				SetDesignModeProjectPath(chartControl);
				chartControl.EndInit();
				Chart chart = ((IChartContainer)chartControl).Chart;
				chart.DataContainer.LockDataSourceEventsSubscription();
				chart.Assign(Chart);
			}
			return viewControl;
		}
		public System.Web.UI.Control GetViewControl() {
			return CreateViewControl();
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			chartDesigner = new ChartDesigner(component as IChartContainer);
			ReferencesHelper.EnsureReferences(DesignerHost, AssemblyInfo.SRAssemblyData, AssemblyInfo.SRAssemblyUtils,
				AssemblyInfo.SRAssemblyWeb, AssemblyInfo.SRAssemblyEditors, AssemblyInfo.SRAssemblyChartsCore, AssemblyInfo.SRAssemblyCharts);
			webChartDesignerActionList = new WebChartDesignerActionList(chartDesigner, this, component);
			chartDesigner.SetDesignerActionList(webChartDesignerActionList);
			TagPrefixHelper.RegisterTagPrefix(RootDesigner, typeof(ChartElement));
			if (DesignerHost != null)
				DesignerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
			SetDesignModeProjectPath((WebChartControl)component);
			SetVerbsState();
		}
		public override bool IsThemableControl() {
			return false;
		}
		public override void RunDesigner() {
			base.RunDesigner("WebChartControl");
		}
	}
	class ChartDataSourceView : DataView, IChartDataSource {
		string name;
		IChartDataSource dataSource;
		protected IChartDataSource DataSource { get { return dataSource; } }
		#region IChartDataSource implementation
		string IChartDataSource.SeriesDataMember { get { return dataSource.SeriesDataMember; } }
		string IChartDataSource.ArgumentDataMember { get { return dataSource.ArgumentDataMember; } }
		string IChartDataSource.ValueDataMember { get { return dataSource.ValueDataMember; } }
		DateTimeMeasureUnitNative? IChartDataSource.DateTimeArgumentMeasureUnit { get { return dataSource.DateTimeArgumentMeasureUnit; } }
		event DataChangedEventHandler IChartDataSource.DataChanged {
			add { dataSource.DataChanged += value; }
			remove { dataSource.DataChanged -= value; }
		}
		#endregion
		public ChartDataSourceView(string name, DataTable table, IChartDataSource dataSource) : base(table) {
			this.name = name;
			this.dataSource = dataSource;
		}
		public override string ToString() {
			return name;
		}
	}
	class PivotGridDataView : ChartDataSourceView, IPivotGrid {
		protected IPivotGrid PivotGrid { get { return (IPivotGrid)DataSource; } }
		#region IPivotGrid implementation
		IList<string> IPivotGrid.ArgumentColumnNames {
			get { return PivotGrid.ArgumentColumnNames; }
		}
		IList<string> IPivotGrid.ValueColumnNames {
			get { return PivotGrid.ValueColumnNames; }
		}
		bool IPivotGrid.RetrieveDataByColumns { 
			get { return PivotGrid.RetrieveDataByColumns; } 
			set { PivotGrid.RetrieveDataByColumns = value; } 
		}
		bool IPivotGrid.RetrieveEmptyCells { 
			get { return PivotGrid.RetrieveEmptyCells; } 
			set { PivotGrid.RetrieveEmptyCells = value; } 
		}
		DefaultBoolean IPivotGrid.RetrieveFieldValuesAsText { 
			get { return PivotGrid.RetrieveFieldValuesAsText; } 
			set { PivotGrid.RetrieveFieldValuesAsText = value; } 
		}
		bool IPivotGrid.SelectionSupported { get { return PivotGrid.SelectionSupported; } }
		bool IPivotGrid.SelectionOnly { 
			get { return PivotGrid.SelectionOnly; } 
			set { PivotGrid.SelectionOnly = value; } 
		}
		bool IPivotGrid.SinglePageSupported { get { return PivotGrid.SinglePageSupported; } }
		bool IPivotGrid.SinglePageOnly { get { 
			return PivotGrid.SinglePageOnly; } 
			set { PivotGrid.SinglePageOnly = value; } 
		}
		bool IPivotGrid.RetrieveColumnTotals { 
			get { return PivotGrid.RetrieveColumnTotals; } 
			set { PivotGrid.RetrieveColumnTotals = value; } 
		}
		bool IPivotGrid.RetrieveColumnGrandTotals { 
			get { return PivotGrid.RetrieveColumnGrandTotals; } 
			set { PivotGrid.RetrieveColumnGrandTotals = value; } 
		}
		bool IPivotGrid.RetrieveColumnCustomTotals { 
			get { return PivotGrid.RetrieveColumnCustomTotals; } 
			set { PivotGrid.RetrieveColumnCustomTotals = value; } 
		}
		bool IPivotGrid.RetrieveRowTotals { 
			get { return PivotGrid.RetrieveRowTotals; } 
			set { PivotGrid.RetrieveRowTotals = value; } 
		}
		bool IPivotGrid.RetrieveRowGrandTotals { 
			get { return PivotGrid.RetrieveRowGrandTotals; } 
			set { PivotGrid.RetrieveRowGrandTotals = value; } 
		}
		bool IPivotGrid.RetrieveRowCustomTotals { 
			get { return PivotGrid.RetrieveRowCustomTotals; } 
			set { PivotGrid.RetrieveRowCustomTotals = value; } 
		}
		bool IPivotGrid.RetrieveDateTimeValuesAsMiddleValues {
			get { return PivotGrid.RetrieveDateTimeValuesAsMiddleValues; }
			set { PivotGrid.RetrieveDateTimeValuesAsMiddleValues = value; }
		}
		int IPivotGrid.MaxAllowedSeriesCount { 
			get { return PivotGrid.MaxAllowedSeriesCount; } 
			set { PivotGrid.MaxAllowedSeriesCount = value; } 
		}
		int IPivotGrid.MaxAllowedPointCountInSeries { 
			get { return PivotGrid.MaxAllowedPointCountInSeries; } 
			set { PivotGrid.MaxAllowedPointCountInSeries = value; } 
		}
		int IPivotGrid.UpdateDelay { 
			get { return PivotGrid.UpdateDelay; } 
			set { PivotGrid.UpdateDelay = value; } 
		}
		int IPivotGrid.AvailableSeriesCount { get { return PivotGrid.AvailableSeriesCount; } }
		IDictionary<object, int> IPivotGrid.AvailablePointCountInSeries { get { return PivotGrid.AvailablePointCountInSeries; } }
		IDictionary<DateTime, DateTimeMeasureUnitNative> IPivotGrid.DateTimeMeasureUnitByArgument { 
			get { return PivotGrid.DateTimeMeasureUnitByArgument; } 
		}
		void IPivotGrid.LockListChanged() {
			PivotGrid.LockListChanged();
		}
		void IPivotGrid.UnlockListChanged() {
			PivotGrid.UnlockListChanged();
		}
		#endregion
		public PivotGridDataView(string name, DataTable table, IPivotGrid pivotGrid) : base(name, table, pivotGrid) {
		}
	}
	public class WebChartCommonActionList : ASPxWebControlDesignerActionList {
		public WebChartCommonActionList(WebChartControlDesigner designer)
			: base(designer) {
		}
		protected override string RunDesignerItemCaption {
			get {
				return "Client Side Events...";
			}
		}
	}
	public class WebChartDesignerActionList : ChartDesignerActionList {
		const string dataSourceCategory = "data source";
		const string commonCategory = "common";
		const string serializingCategory = "serializing";
		static void AddDesignerActionItem(DesignerActionItemCollection actionItems, DesignerActionItem item) {
			item.AllowAssociate = true;
			actionItems.Add(item);
		}
		WebChartControlDesigner webChartDesigner;
		bool populateEnabled;
		bool clearDataSourceEnabled;
		bool dataSnapshotEnabled;
		public bool PopulateEnabled { get { return populateEnabled; } set { populateEnabled = value; } }
		public bool ClearDataSourceEnabled { get { return clearDataSourceEnabled; } set { clearDataSourceEnabled = value; } }
		public bool DataSnapshotEnabled { get { return dataSnapshotEnabled; } set { dataSnapshotEnabled = value; } }
		public WebChartDesignerActionList(ChartDesigner chartDesigner, WebChartControlDesigner webChartDesigner, IComponent component) : base(chartDesigner, component) {
			this.webChartDesigner = webChartDesigner;
		}
		void OnPopulateAction() {
			webChartDesigner.OnPopulate();
		}
		void OnClearDataSourceAction() {
			webChartDesigner.OnClearDataSource();
		}
		void OnDataSnapshotAction() {
			webChartDesigner.OnDataSnapshot();
		}
		void OnWebWizardAction() {
			webChartDesigner.OnWizard();
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			bool enabled = ChartDesigner.GetInheritanceAttribute(webChartDesigner.Chart.Container) == InheritanceAttribute.NotInherited;
			AnnotationsActionEnabled = enabled;
			SeriesActionEnabled = enabled;
			EditPalettesEnabled = enabled;
			SaveLayoutEnabled = true;
			LoadLayoutEnabled = enabled;
			WizardEnabled = enabled;
			DesignerActionItemCollection actionItems = new DesignerActionItemCollection();
			DesignerActionItem actionItem;
			if (populateEnabled) {
				actionItem = new DesignerActionMethodItem(this, "OnPopulateAction",  ChartLocalizer.GetString(ChartStringId.VerbPopulate), 
					dataSourceCategory, ChartLocalizer.GetString(ChartStringId.VerbPopulateDescription), false);
				AddDesignerActionItem(actionItems, actionItem);
			}
			if (clearDataSourceEnabled) {
				actionItem = new DesignerActionMethodItem(this, "OnClearDataSourceAction", ChartLocalizer.GetString(ChartStringId.VerbClearDataSource), 
					dataSourceCategory, ChartLocalizer.GetString(ChartStringId.VerbClearDataSourceDescription), false);
				AddDesignerActionItem(actionItems, actionItem);
			}
			if (dataSnapshotEnabled) {
				actionItem = new DesignerActionMethodItem(this, "OnDataSnapshotAction", ChartLocalizer.GetString(ChartStringId.VerbDataSnapshot), 
					dataSourceCategory, ChartLocalizer.GetString(ChartStringId.VerbDataSnapshotDescription), false);
				AddDesignerActionItem(actionItems, actionItem);
			}
			actionItem = new DesignerActionMethodItem(this, "OnWebWizardAction", ChartLocalizer.GetString(ChartStringId.VerbDesigner), 
				commonCategory, ChartLocalizer.GetString(ChartStringId.VerbWizardDescription), true);
			AddDesignerActionItem(actionItems, actionItem);
			AddSeriesAction(actionItems, commonCategory);
			AddAnnotationsAction(actionItems, commonCategory);
			AddEditPalettesAction(actionItems, commonCategory);
			AddSaveLayoutAction(actionItems, serializingCategory);
			AddLoadLayoutAction(actionItems, serializingCategory);
			return actionItems;
		}
	}
	class DTEHelper {
		static string GetPropertyValue(Properties properties, string name) {
			try {
				return (string)properties.Item(name).Value;
			}
			catch { 
				return String.Empty;
			}
		}
		public static string GetPropertyValue(Project project, string name) {
			return project != null ? GetPropertyValue(project.Properties, name) : String.Empty;
		}
		public static string GetPropertyValue(ProjectItem projectItem, string name) {
			return projectItem != null ? GetPropertyValue(projectItem.Properties, name) : String.Empty;
		}
		public static string GetFullPath(ProjectItem projectItem) {
			return GetPropertyValue(projectItem, "FullPath");
		}
		public static string GetFullPath(Project project) {
			return GetPropertyValue(project, "FullPath");
		}
	}
}
