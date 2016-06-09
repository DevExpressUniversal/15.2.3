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
using System.Data.Common;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Wizards;
namespace DevExpress.XtraReports.Design
{
	public abstract class XtraReportWizardBase
	{
		XtraReport report;
		IDesignerHost host;
		protected virtual XtraReport Report { get { return report; }
		}
		public IDesignerHost DesignerHost { get { return host; }
		}
		protected XtraReportWizardBase(XtraReport report) {
			this.report = report;
			if (report != null)
				this.host = (IDesignerHost)report.Site.GetService(typeof(IDesignerHost));
		}
		protected Band GetBandByType(Type bandType) {
			Band band = Report.Bands.GetBandByType(bandType);
			if (band == null || bandType == typeof(GroupFooterBand) || bandType == typeof(GroupHeaderBand)) {
				band = (Band)DesignerHost.CreateComponent(bandType);
				Report.Bands.Add(band);
			}
			else
				DeleteControls(band.Controls);
			band.HeightF = 1;
			return band;
		}
		void DeleteControls(XRControlCollection controls) {
			for(int i = controls.Count - 1; i >= 0; i--) {
				if(!(controls[i] is DetailBand) && CanDestroyComponent(controls[i]))
					DesignerHost.DestroyComponent(controls[i]);
			}
		}
		static bool CanDestroyComponent(IComponent component) {
			InheritanceAttribute attribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(component)[typeof(InheritanceAttribute)];
			if(attribute != null && attribute.InheritanceLevel != InheritanceLevel.NotInherited)
				return false;
			return true;
		}
		protected void ClearBands() {
			DesignerTransaction transaction = host.CreateTransaction();
			try {
				foreach(Band band in Report.Bands)
					DeleteControls(band.Controls);
				DeleteControls(Report.Bands);
				transaction.Commit();
			} catch {
				transaction.Cancel();
			}
		}
		public abstract void BuildReport();
		protected void ExecuteInTransaction(Action0 action) {
			DesignerTransaction transaction = DesignerHost.CreateTransaction(DesignSR.Trans_CreateComponents);
			try {
				action();
				transaction.Commit();
			} catch {
				transaction.Cancel();
			}
		}
		public virtual void Cancel() {
		}
	}
	public class StandardReportWizard : XtraReportWizardBase
	{
		static string StyleNameTitle { get { return ReportBuilder.StyleNameTitle; } }
		static string StyleNameFieldCaption { get { return ReportBuilder.StyleNameFieldCaption; } }
		static string StyleNamePageInfo { get { return ReportBuilder.StyleNamePageInfo; } }
		static string StyleNameDataField { get { return ReportBuilder.StyleNameDataField; } }
		#region fields & properties
		protected ReportInfo reportInfo = new ReportInfo();
		public ObjectNameCollection Fields { get { return reportInfo.Fields; } }
		public ObjectNameCollection SelectedFields { get { return reportInfo.SelectedFields; } }
		ObjectNameCollection UngroupingFields { get { return reportInfo.UngroupingFields; } }
		ObjectNameCollection OrderedSelectedFields {
			get { return reportInfo.OrderedSelectedFields; }
		}
		public ObjectNameCollectionsSet GroupingFieldsSet { get { return reportInfo.GroupingFieldsSet; } }
		public ArrayList SummaryFields { get { return reportInfo.SummaryFields; } }
		public bool IgnoreNullValuesForSummary { get { return reportInfo.IgnoreNullValuesForSummary; } set { reportInfo.IgnoreNullValuesForSummary = value; }
		}
		public PageOrientation Orientation { get { return reportInfo.Orientation; } set { reportInfo.Orientation = value; }
		}
		public bool FitFieldsToPage { get { return reportInfo.FitFieldsToPage; } set { reportInfo.FitFieldsToPage = value; }
		}
		public ReportLayout Layout { get { return reportInfo.Layout; } set { reportInfo.Layout = value; }
		}
		public ReportStyle Style { get { return reportInfo.Style; } set { reportInfo.Style = value; }
		}
		public string ReportTitle { get { return reportInfo.ReportTitle; } set { reportInfo.ReportTitle = value; }
		}
		public string ReportName { get { return Report.Name; }
		}
		int Spacing {
			get { return reportInfo.Spacing; }
			set { reportInfo.Spacing = value; }
		}
		#endregion
		public StandardReportWizard(XtraReport report)  : base(report) {
			this.Style = new ReportStyle("Bold", "Wizards.Bold.repss", typeof(DevExpress.XtraReports.ResFinder));
			if(Report != null) {
				this.Spacing = XRConvert.Convert(6, GraphicsDpi.Pixel, Report.Dpi);
				this.ReportTitle = Report.Name;
			}
		}
		public virtual void FillFields() {
			ClearFields();
			ObjectNameCollection fieldsCollection = ReportHelper.GetColumnNames(Report);
			if(fieldsCollection == null)
				return;
			Fields.CopyFrom(fieldsCollection);
		}
		protected void ClearFields() {
			Fields.Clear();
			SelectedFields.Clear();
			UngroupingFields.Clear();
			GroupingFieldsSet.Clear();
			SummaryFields.Clear();
		}
		public override void BuildReport() {
			ExecuteInTransaction(delegate() {
				OnBeforeBuild();
				ClearBands();
				CreateBuilder().Execute();
				foreach(XRControlStyle s in Report.StyleSheet) {
					try {
						DesignerHost.Container.Add(s, s.Name);
					} catch {
					}
				}
				new BandsValidator(DesignerHost).EnsureExistence(BandKind.TopMargin, BandKind.BottomMargin);
			});
		}
		public ObjectNameCollection GetFieldsForSummary() {
			return CreateBuilder().GetFieldsForSummary();
		}
		ReportBuilder CreateBuilder() {
			ReportBuilder builder = CreateBuilder(Report, new ComponentFactory(DesignerHost));
			builder.ReportInfo = reportInfo;
			return builder;
		}
		protected virtual ReportBuilder CreateBuilder(XtraReport report, IComponentFactory componentFactory) {
			return new ReportBuilder(report, componentFactory);
		}
		protected virtual void OnBeforeBuild() {
		}
	}
	public class LabelReportWizard : XtraReportWizardBase
	{
		const string LabelProductsTableName = "LabelProducts";
		const string LabelDetailsTableName = "LabelDetails";
		const string PaperKindsTableName = "PaperKinds";		
		LabelInfo labelInfo = new LabelInfo();
		DataTable paperKinds;
		DataTable labelProducts;
		DataTable labelDetails;
		DataSet labelProductsDetails = new DataSet();
		PaperKindList paperKindList;
		internal DataTable LabelProducts { get { return labelProducts; }
		}
		internal DataTable LabelDetails { get { return labelDetails; }
		}
		public LabelInfo LabelInfo { get { return labelInfo; }
		}
		public PaperKindList PaperKindList { get { return paperKindList; } }
		public LabelReportWizard(XtraReport report) : base(report) {			
			FillLabelsDataSet();
			FillPaperKindList();
		}
		void FillLabelsDataSet() {
			using(Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(LocalResFinder), "Wizards.Labels.xml")) {
				labelProductsDetails.ReadXml(stream);
				labelProducts = labelProductsDetails.Tables[LabelProductsTableName];
				labelDetails = labelProductsDetails.Tables[LabelDetailsTableName];
				DataColumn parentColumn = labelProducts.Columns["LabelProductID"];
				DataColumn childColumn = labelDetails.Columns["LabelProductID"];
				DataRelation relation = labelProductsDetails.Relations.Add(parentColumn, childColumn);
				relation.RelationName = "LabelProductsDetails";
				paperKinds = labelProductsDetails.Tables[PaperKindsTableName];
			}
		}
		void FillPaperKindList() {
			paperKindList = new PaperKindList(Report.Dpi);
			for(int i = 0; i < paperKinds.Rows.Count; i++) {
				DataRow row = paperKinds.Rows[i];
				string name = Convert.ToString(row["Name"]);
				int id = Convert.ToInt32(row["PaperKindID"]);
				int enumID = Convert.ToInt32(row["EnumID"]);
				PaperKind paperKind = (PaperKind)enumID;
				Size size = Size.Empty;
				if(paperKind == PaperKind.Custom) {
					size.Width = Convert.ToInt32(row["Width"]);
					size.Height = Convert.ToInt32(row["Height"]);					
				}
				else
					size = XtraPrinting.Native.PageSizeInfo.GetPageSize(paperKind);
				paperKindList.Add(new PaperKindItem(name, size, id, paperKind));
			}
		}
		public override void BuildReport() {
			ExecuteInTransaction(delegate() {
				ClearBands();
				new LabelReportBuilder(Report, new ComponentFactory(DesignerHost), LabelInfo, PaperKindList).Execute();
				new BandsValidator(DesignerHost).EnsureExistence(BandKind.TopMargin, BandKind.BottomMargin);
			});
		}
	}
	public class XRWizardRunnerBase
	{
		XtraReport report;
		XtraReportWizardBase wizard;
		public event EventHandler<XRWizardRunnerBeforeRunEventArgs> BeforeRun;
		public XtraReportWizardBase Wizard { get { return wizard; } set { wizard = value; }
		}
		public XtraReport Report { get { return report; } set { report = value; }
		}
		public XRWizardRunnerBase(XtraReport report) {
			this.report = report;
		}
		protected DialogResult RunForm(WizardPage[] wizPages) {
			return RunForm(null, wizPages);
		}
		protected DialogResult RunForm(IWin32Window owner, WizardPage[] wizPages) {
			XtraReportWizardForm form = new XtraReportWizardForm(Wizard.DesignerHost);
			List<WizardPage> wizPagesList = new List<WizardPage>(wizPages);
			OnBeforeRun(form, wizPagesList);
			form.Controls.AddRange(wizPagesList.ToArray());
			DialogResult result = owner != null ? DialogRunner.ShowDialog(form, owner) :
				DialogRunner.ShowDialog(form, Wizard.DesignerHost);
			if(result == DialogResult.OK)
				Wizard.BuildReport();
			return result;
		}
		protected virtual void OnBeforeRun(XtraReportWizardForm form, IList<WizardPage> wizPages) {
			if(BeforeRun != null)
				BeforeRun(this, new XRWizardRunnerBeforeRunEventArgs(form, wizPages));
		}
	}
	public class XRWizardRunnerBeforeRunEventArgs : EventArgs {
		XtraReportWizardForm form;
		IList<WizardPage> wizardPages;
		public XtraReportWizardForm Form {
			get { return form;}
		}
		public IList<WizardPage> WizardPages {
			get { return wizardPages; }
		}
		public XRWizardRunnerBeforeRunEventArgs (XtraReportWizardForm form, IList<WizardPage> wizardPages) {
			this.form = form;
			this.wizardPages = wizardPages;
	   }
	}
	public class XtraReportWizardRunner : XRWizardRunnerBase
	{
		public XtraReportWizardRunner(XtraReport report) : base(report) {
			this.Wizard = new StandardReportWizard(report);
		}
		public DialogResult Run() {
			return RunForm(new WizardPage[] {
				new WizPageWelcome(this),
				new WizPageChooseFields(this),
				new WizPageGrouping(this),
				new WizPageSummary(this),
				new WizPageGroupedLayout(this),
				new WizPageUngroupedLayout(this),
				new WizPageStyle(this),
				new WizPageReportTitle(this),
				new WizPageLabelType(this),
				new WizPageLabelOptions(this) }
			);
		}
	}
	public class NewDataSourceWizardRunner : XRWizardRunnerBase {
		class WizPageTablesEx : WizPageTables {
			public WizPageTablesEx(XRWizardRunnerBase runner)
				: base(runner) {
			}
			protected override void UpdateButtons() {
				base.UpdateButtons();
				if((Wizard.WizardButtons & DevExpress.Utils.WizardButton.Next) == DevExpress.Utils.WizardButton.Next) {
					Wizard.WizardButtons = DevExpress.Utils.WizardButton.Back | DevExpress.Utils.WizardButton.Finish;
				}
			}
			protected override bool CanAddItem() {
				return tvAvailableItems.SelectedNode != null &&
					tvAvailableItems.SelectedNode != tablesNode &&
					tvAvailableItems.SelectedNode != viewsNode;
			}
			protected override DataSet CreateDataSet() {
				string connectionString = GetCompatibleOleDBConnectionString(fWizard.ConnectionString);
				DbConnection connection = ConnectionStringHelper.CreateDBConnection(connectionString);
				if(!TryOpenConnection(connection))
					return null;
				try {
					DataSet dataSet = new DataSet(fWizard.DatasetName);
					foreach(DBListViewItem item in lvSelectedItems.Items) {
						DbDataAdapter dataAdapter = CreateDataAdapter(dataSet, connection, connectionString, item);
						fWizard.DataAdapters.Add(dataAdapter);
					}
					fWizard.Connection = connection;
					return dataSet;
				} finally {
					connection.Close();
				}
			}
		}
		class NewDataSetWizard : NewStandardReportWizard {
			public NewDataSetWizard(XtraReport report, IDataContainer dataContainer) : base(report, dataContainer) {
			}
			public override void BuildReport() {
				PrepareDataSet();
				OleDbConnection connection = GetOleDBConnection();
				if(connection == null)
					return;
				connection.Open();
				try {
					DBObjectsHelper.MakeDataRelations(connection, Dataset);
				}
				finally {
					connection.Close();
				}
			}
			OleDbConnection GetOleDBConnection() {
				if(Connection is OleDbConnection)
					return (OleDbConnection)Connection;
				if(Connection is System.Data.Odbc.OdbcConnection)
					return null;
				return new OleDbConnection(ConnectionString);
			}
		}
		public NewDataSourceWizardRunner()
			: base(null) {
		}
		public DialogResult Run(IDesignerHost host) {
			return Run(host, null, (XtraReport)host.RootComponent);
		}
		public DialogResult Run(IDesignerHost host, IWin32Window owner, IDataContainer dataContainer) {
			this.Report = host.RootComponent as XtraReport;
			if(this.Report == null)
				return DialogResult.Cancel;
			this.Wizard = new NewDataSetWizard(this.Report, dataContainer);
			return RunForm(owner, new WizardPage[] {
				new WizPageDataset(this),
				new WizPageConnection(this),
				new WizPageTablesEx(this) }
			);
		}
		protected override void OnBeforeRun(XtraReportWizardForm form, IList<WizardPage> wizPages) {
			form.StartPosition = FormStartPosition.CenterScreen;
			base.OnBeforeRun(form, wizPages);
		}
	}
	public class DataAdapters : CollectionBase {
		public DataAdapters() {
		}
		public int Add(IDbDataAdapter dataAdapter) {
			return InnerList.Add(dataAdapter);
		}
		public IDbDataAdapter this[int index] { get { return (IDbDataAdapter)List[index]; } }
	}
	public class NewStandardReportWizard : StandardReportWizard {
		#region Fields & Properties
		string connectionString = String.Empty;
		string tableSchemaName = String.Empty;
		string datasetName = String.Empty;
		DataSet dataset;
		DbConnection connection;
		DataAdapters dataAdapters = new DataAdapters();
		IDataContainer dataContainer;
		IDataContainer DataContainer {
			get {
				return dataContainer != null ? dataContainer : Report;
			}
		}
		public string ConnectionString { get { return connectionString; } set { connectionString = value; }
		}
		public string DatasetName { get { return datasetName; } set { datasetName = value; }
		}
		public DataSet Dataset { get { return dataset; } set { dataset = value; }
		}
		public string TableSchemaName { get { return tableSchemaName; } set { tableSchemaName = value; } 
		}
		public DataAdapters DataAdapters {
			get { return dataAdapters; }
		}
		public DbConnection Connection {
			get { return connection; }
			set { connection = value; } 
		}
		#endregion
		public NewStandardReportWizard(XtraReport report) 
			: this(report, report) {
		}
		public NewStandardReportWizard(XtraReport report, IDataContainer dataContainer)
			: base(report) {
			this.dataContainer = dataContainer;
		}
		protected override void OnBeforeBuild() {
			PrepareDataSet();
		}
		protected virtual void PrepareDataSet() {
			if(dataset == null)
				return;
			DesignToolHelper.ForceAddToContainer(DesignerHost.Container, dataset, dataset.DataSetName);
			DataContainer.DataSource = dataset;
			foreach(DataAdapter dataAdapter in DataAdapters) {
				DesignToolHelper.ForceAddToContainer(DesignerHost.Container, dataAdapter, dataAdapter.TableMappings[0].DataSetTable.Replace(" ", "_") + "TableAdapter");
			}
			if(DataAdapters.Count > 0)
				DataContainer.DataAdapter = DataAdapters[0];
			ReportDesigner.SetDataMember(DataContainer, dataset);
			IComponentChangeService svc = (IComponentChangeService)this.DesignerHost.GetService(typeof(IComponentChangeService));
			if(svc != null)
				svc.OnComponentChanged(DataContainer, null, null, null);
		}
		public override void FillFields() {
			ClearFields();
			if(dataset != null || dataset.Tables.Count != 0) {
				foreach(DataColumn item in dataset.Tables[0].Columns)
					Fields.Add(item.ColumnName, item.ColumnName);
			}
		}
	}
	public class NewXtraReportWizardRunnerEUD : XRWizardRunnerBase {
		public NewXtraReportWizardRunnerEUD() : base(null) {
		}
		public DialogResult Run(IDesignerHost host) {
			this.Report = host.RootComponent as XtraReport;
			if (this.Report == null)
				return DialogResult.Cancel;
			this.Wizard = new NewStandardReportWizard(this.Report);
			return RunForm(new WizardPage[] {
				new WizPageWelcome(this),
				new WizPageDataset(this),
				new WizPageConnection(this),
				new WizPageTables(this),
				new WizPageChooseFields(this),
				new WizPageGrouping(this),
				new WizPageSummary(this),
				new WizPageGroupedLayout(this),
				new WizPageUngroupedLayout(this),
				new WizPageStyle(this),
				new WizPageReportTitle(this),
				new WizPageLabelType(this),
				new WizPageLabelOptions(this) }
			);
		}
	}
}
