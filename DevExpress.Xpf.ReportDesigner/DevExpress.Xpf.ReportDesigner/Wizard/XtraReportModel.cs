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

using System.Reflection;
using DevExpress.Data.Entity;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using FieldInfo = DevExpress.DataAccess.Excel.FieldInfo;
namespace DevExpress.XtraReports.Wizards3 {
	public class XtraReportModel : ReportModel, IDataSourceModel {
		readonly DataSourceModel dataSourceModel = new DataSourceModel();
		internal DataSourceModel DataSourceModel {
			get {
				return dataSourceModel;
			}
		}
		public object Tag {
			get;
			set;
		}
		public DataSourceType DataSourceType {
			get { return dataSourceModel.DataSourceType; }
			set { dataSourceModel.DataSourceType = value; }
		}
		public XtraReportModel() {
		}
		public XtraReportModel(XtraReportModel model)
			: base(model) {
			dataSourceModel = (DataSourceModel)model.dataSourceModel.Clone();
			this.Tag = model.Tag;
		}
		public override object Clone() {
			return new XtraReportModel(this);
		}
		public override bool Equals(object obj) {
			XtraReportModel model = obj as XtraReportModel;
			if(model == null)
				return false;
			return base.Equals(obj) && Equals(dataSourceModel, model.dataSourceModel);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
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
