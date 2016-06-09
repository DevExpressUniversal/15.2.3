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

using System.Collections.Generic;
using System.Reflection;
using DevExpress.Data.Entity;
using DevExpress.Data.XtraReports.ReportGeneration;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.XtraReports.Design.ReportGenerator.Data;
using DevExpress.XtraReports.UI;
using FieldInfo = DevExpress.DataAccess.Excel.FieldInfo;
using DevExpress.XtraExport.Helpers;
using System.Linq;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Printing;
namespace DevExpress.XtraReports.ReportGeneration.Wizard
{
	public partial class ReportGridDataModel : ReportGridModel {
		readonly DataSourceModel dataSourceModel = new DataSourceModel();
		IGridViewFactory<ColumnImplementer, DataRowImplementer> mockViewFactory;
		IGridViewFactory<ColumnImplementer, DataRowImplementer> mockViewFactoryGrouped;
		IGridViewFactory<ColumnImplementer, DataRowImplementer> mockViewFactoryStyles;
		string reportFileName;
		private IEnumerable<GridView> viewSet;
		public ReportGridDataModel() {
		}
		public ReportGridDataModel(ReportGridDataModel dataModel) : base(dataModel) {
			this.ReportTool = dataModel.ReportTool;
			this.ReportFileName = dataModel.ReportFileName;
			this.ReportFilePath = dataModel.ReportFilePath;
			this.ViewSet = dataModel.ViewSet;
			this.View = dataModel.View;
		}
		public ReportTool ReportTool { get; set; }
		public bool AddToProject { get; set; }
		public bool ViewGrouped{
			get {
				if(View != null) {
					var view = ((IGridViewFactory<ColumnImplementer, DataRowImplementer>) View).GetIViewImplementerInstance();
					if(view != null)
						return view.GetGroupedColumns().Any();
				}
				return false;
			}
		}
		public string ReportFileName { get { return reportFileName; } set { reportFileName = value; } }
		public string ReportFilePath { get; set; }
		public IEnumerable<GridView> ViewSet {
			get{
				if(viewSet == null) viewSet = new List<GridView>();
				return viewSet;
			}
			set { viewSet = value; }
		}
		IGridViewFactory<ColumnImplementer, DataRowImplementer> MockViewFactory {
			get {
				if(mockViewFactory == null){
					var view = new SampleForm().GridView;
					view.Columns[0].Summary.Add(DevExpress.Data.SummaryItemType.Count);
					view.OptionsView.ShowFooter = true;
					mockViewFactory = new GridViewFactoryImplementer(view);
				}
				return mockViewFactory;
			}
		}
		IGridViewFactory<ColumnImplementer, DataRowImplementer> MockViewFactoryGrouped {
			get {
				if(mockViewFactoryGrouped == null){
					var view = new SampleForm().GridView;
					view.Columns[0].GroupIndex = 0;
					view.GroupSummary.Add(DevExpress.Data.SummaryItemType.Count, view.Columns[1].FieldName, view.Columns[1],"Count = {0}");
					view.OptionsView.GroupFooterShowMode = XtraGrid.Views.Grid.GroupFooterShowMode.VisibleIfExpanded;
					mockViewFactoryGrouped = new GridViewFactoryImplementer(view);
				}
				return mockViewFactoryGrouped;
			}
		}
		IGridViewFactory<ColumnImplementer, DataRowImplementer> MockViewFactoryStyles {
			get {
				if(mockViewFactoryStyles == null){
					var view = new SampleForm().GridView;
					view.Appearance.OddRow.BackColor = System.Drawing.Color.LightSkyBlue;
					view.Appearance.EvenRow.BackColor = System.Drawing.Color.PapayaWhip;
					view.AppearancePrint.OddRow.BackColor = System.Drawing.Color.LightBlue;
					view.AppearancePrint.EvenRow.BackColor = System.Drawing.Color.CornflowerBlue;
					mockViewFactoryStyles = new GridViewFactoryImplementer(view);
				}
				return mockViewFactoryStyles;
			}
		}
		public XtraReport GetFakedReport(ReportGenerationOptions options){
			return Generate(MockViewFactory, options);
		}
		public XtraReport GetGroupedFakedReport(ReportGenerationOptions options){
			return Generate(MockViewFactoryGrouped, options);
		}
		public XtraReport GetFakedReportWithStyles(ReportGenerationOptions options){
			return Generate(MockViewFactoryStyles, options);
		}
		protected XtraReport GenerateReport(IGridViewFactory<ColumnImplementer,DataRowImplementer> view) {
			return Generate(view, Options);
		}
		public XtraReport Generate(IGridViewFactory<ColumnImplementer, DataRowImplementer> view, ReportGenerationOptions options) {
			XtraReport report = new XtraReport();
			ReportGenerationExtensions<ColumnImplementer, DataRowImplementer>.Generate(report, view, options);
			return report;
		}
		public override object Clone() {
			return new ReportGridDataModel(this);
		}
	}
	public partial class ReportGridDataModel : IDataSourceModel {
		#region ISqlDataSourceModel Members
		string ISqlDataSourceModel.SqlQueryText {
			get { return ((ISqlDataSourceModel)dataSourceModel).SqlQueryText; }
			set { ((ISqlDataSourceModel)dataSourceModel).SqlQueryText = value; }
		}
		public SaveConnectionMethod ShouldSaveConnection {
			get { return ((IDataComponentModelWithConnection)dataSourceModel).ShouldSaveConnection; }
			set { ((IDataComponentModelWithConnection)dataSourceModel).ShouldSaveConnection = value; }
		}
		SqlDataConnection ISqlDataSourceModel.DataConnection {
			get { return ((ISqlDataSourceModel)dataSourceModel).DataConnection; }
			set { ((ISqlDataSourceModel)dataSourceModel).DataConnection = value; }
		}
		SqlQuery ISqlDataSourceModel.Query {
			get { return ((ISqlDataSourceModel)dataSourceModel).Query; }
			set { ((ISqlDataSourceModel)dataSourceModel).Query = value; }
		}
		#endregion
		#region IEFDataSourceModel Members
		EFDataConnection IEFDataSourceModel.DataConnection {
			get { return ((IEFDataSourceModel)dataSourceModel).DataConnection; }
			set { ((IEFDataSourceModel)dataSourceModel).DataConnection = value; }
		}
		IEntityFrameworkModelHelper IEFDataSourceModel.ModelHelper {
			get { return ((IEFDataSourceModel)dataSourceModel).ModelHelper; }
			set { ((IEFDataSourceModel)dataSourceModel).ModelHelper = value; }
		}
		EFConnectionParameters IEFDataSourceModel.ConnectionParameters {
			get { return ((IEFDataSourceModel)dataSourceModel).ConnectionParameters; }
			set { ((IEFDataSourceModel)dataSourceModel).ConnectionParameters = value; }
		}
		DataConnectionLocation IEFDataSourceModel.ConnectionStringLocation {
			get { return ((IEFDataSourceModel)dataSourceModel).ConnectionStringLocation; }
			set { ((IEFDataSourceModel)dataSourceModel).ConnectionStringLocation = value; }
		}
		EFStoredProcedureInfo[] IEFDataSourceModel.StoredProceduresInfo {
			get { return dataSourceModel.StoredProceduresInfo; }
			set { dataSourceModel.StoredProceduresInfo = value; }
		}
		public string DataMember {
			get { return dataSourceModel.DataMember; }
			set { dataSourceModel.DataMember = value; }
		}
		#endregion
		#region IDataComponentModel Members
		string IDataComponentModelWithConnection.ConnectionName {
			get { return ((IDataComponentModelWithConnection)dataSourceModel).ConnectionName; }
			set { ((IDataComponentModelWithConnection)dataSourceModel).ConnectionName = value; }
		}
		public IDataConnection DataConnection {
			get { return ((IDataComponentModelWithConnection)dataSourceModel).DataConnection; }
			set { ((IDataComponentModelWithConnection)dataSourceModel).DataConnection = value; }
		}
		public object DataSchema {
			get { return dataSourceModel.DataSchema; }
			set { dataSourceModel.DataSchema = value; }
		}
		#endregion
		#region Implementation of IObjectDataSourceModel
		IDXAssemblyInfo IObjectDataSourceModel.Assembly {
			get { return dataSourceModel.Assembly; }
			set { dataSourceModel.Assembly = value; }
		}
		IDXTypeInfo IObjectDataSourceModel.ObjectType {
			get { return dataSourceModel.ObjectType; }
			set { dataSourceModel.ObjectType = value; }
		}
		public ObjectMember ObjectMember {
			get { return dataSourceModel.ObjectMember; }
			set { dataSourceModel.ObjectMember = value; }
		}
		public Parameter[] MemberParameters {
			get { return dataSourceModel.MemberParameters; }
			set { dataSourceModel.MemberParameters = value; }
		}
		public ConstructorInfo ObjectConstructor {
			get { return dataSourceModel.ObjectConstructor; }
			set { dataSourceModel.ObjectConstructor = value; }
		}
		public Parameter[] CtorParameters {
			get { return dataSourceModel.CtorParameters; }
			set { dataSourceModel.CtorParameters = value; }
		}
		public ShowAllState ShowAllState {
			get { return dataSourceModel.ShowAllState; }
			set { dataSourceModel.ShowAllState = value; }
		}
		#endregion
		#region Implementation of IDataSourceModel
		public DataSourceType DataSourceType {
			get { return dataSourceModel.DataSourceType; }
			set { dataSourceModel.DataSourceType = value; }
		}
		public string DataSourceName {
			get { return dataSourceModel.DataSourceName; }
			set { dataSourceModel.DataSourceName = value; }
		}
		#endregion
		#region Implementation of IExcelDataSourceModel
		public bool ShouldSavePassword {
			get { return dataSourceModel.ShouldSavePassword; }
			set { dataSourceModel.ShouldSavePassword = value; }
		}
		public ExcelSourceOptionsBase SourceOptions {
			get { return dataSourceModel.SourceOptions; }
			set { dataSourceModel.SourceOptions = value; }
		}
		public string FileName {
			get { return dataSourceModel.FileName; }
			set { dataSourceModel.FileName = value; }
		}
		public FieldInfo[] Schema {
			get { return dataSourceModel.Schema; }
			set { dataSourceModel.Schema = value; }
		}
		#endregion
	}
}
