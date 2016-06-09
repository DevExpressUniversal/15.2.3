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
using System.Reflection;
using DevExpress.Data.Entity;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using FieldInfo = DevExpress.DataAccess.Excel.FieldInfo;
namespace DevExpress.DataAccess.Wizard.Model {
	public class DataSourceModel : IDataSourceModel {
		readonly SqlDataSourceModel sqlDataSourceModel = new SqlDataSourceModel();
		readonly EFDataSourceModel efDataSourceModel = new EFDataSourceModel();
		readonly ObjectDataSourceModel objectDataSourceModel = new ObjectDataSourceModel();
		readonly ExcelDataSourceModel excelDataSourceModel = new ExcelDataSourceModel();
		public DataSourceType DataSourceType { get; set; }
		public object Tag { get; set; }
		public string DataSourceName { get; set; }
		public DataSourceModel() {
			DataSourceType = DataSourceType.Xpo;
		}
		protected DataSourceModel(DataSourceModel other) {
			this.sqlDataSourceModel = new SqlDataSourceModel(other.sqlDataSourceModel);
			this.efDataSourceModel = new EFDataSourceModel(other.efDataSourceModel);
			this.objectDataSourceModel = new ObjectDataSourceModel(other.objectDataSourceModel);
			this.excelDataSourceModel = new ExcelDataSourceModel(other.excelDataSourceModel);
			this.DataSourceType = other.DataSourceType;
			this.DataSourceName = other.DataSourceName;
		}
		public virtual object Clone() { return new DataSourceModel(this); }
		public override bool Equals(object obj) {
			DataSourceModel other = obj as DataSourceModel;
			if(other == null)
				return false;
			if(!string.Equals(DataSourceName, other.DataSourceName, StringComparison.OrdinalIgnoreCase))
				return false;
			if(DataSourceType != other.DataSourceType)
				return false;
			if(DataSourceType == DataSourceType.Xpo)
				return sqlDataSourceModel.Equals(other.sqlDataSourceModel);
			if(DataSourceType == DataSourceType.Entity)
				return efDataSourceModel.Equals(other.efDataSourceModel);
			if(DataSourceType == DataSourceType.Object)
				return objectDataSourceModel.Equals(other.objectDataSourceModel);
			return excelDataSourceModel.Equals(other.excelDataSourceModel);
		}
		public override int GetHashCode() {
			return 0;
		}
		#region ISqlDataSourceModel Members
		SqlDataConnection ISqlDataSourceModel.DataConnection {
			get { return this.sqlDataSourceModel.DataConnection; }
			set { this.sqlDataSourceModel.DataConnection = value; }
		}
		SqlQuery ISqlDataSourceModel.Query {
			get { return this.sqlDataSourceModel.Query; }
			set { this.sqlDataSourceModel.Query = value; }
		}
		string ISqlDataSourceModel.SqlQueryText {
			get { return this.sqlDataSourceModel.SqlQueryText; }
			set { this.sqlDataSourceModel.SqlQueryText = value; }
		}
		#endregion
		#region IDBDataComponentModel Members
		protected virtual string ConnectionName {
			get {
				if(DataSourceType == DataSourceType.Xpo)
					return this.sqlDataSourceModel.ConnectionName;
				if(DataSourceType == DataSourceType.Entity)
					return this.efDataSourceModel.ConnectionName;
				return string.Empty;
			}
			set {
				if(DataSourceType == DataSourceType.Xpo)
					this.sqlDataSourceModel.ConnectionName = value;
				else if(DataSourceType == DataSourceType.Entity)
					this.efDataSourceModel.ConnectionName = value;
			}
		}
		protected virtual IDataConnection DataConnection {
			get {
				if(DataSourceType == DataSourceType.Xpo)
					return this.sqlDataSourceModel.DataConnection;
				if(DataSourceType == DataSourceType.Entity)
					return this.efDataSourceModel.DataConnection;
				return null;
			}
			set {
				if(DataSourceType == DataSourceType.Xpo)
					this.sqlDataSourceModel.DataConnection = (SqlDataConnection)value;
				else if(DataSourceType == DataSourceType.Entity)
					this.efDataSourceModel.DataConnection = (EFDataConnection)value;
			}
		}
		protected virtual SaveConnectionMethod ShouldSaveConnection {
			get {
				if(DataSourceType == DataSourceType.Xpo)
					return this.sqlDataSourceModel.ShouldSaveConnection;
				if(DataSourceType == DataSourceType.Entity)
					return this.efDataSourceModel.ShouldSaveConnection;
				return SaveConnectionMethod.Hardcode;
			}
			set {
				if(DataSourceType == DataSourceType.Xpo)
					this.sqlDataSourceModel.ShouldSaveConnection = value;
				else if(DataSourceType == DataSourceType.Entity)
					this.efDataSourceModel.ShouldSaveConnection = value;
			}
		}
		string IDataComponentModelWithConnection.ConnectionName {
			get { return ConnectionName; }
			set { ConnectionName = value; }
		}
		IDataConnection IDataComponentModelWithConnection.DataConnection {
			get { return DataConnection; }
			set { DataConnection = value; }
		}
		SaveConnectionMethod IDataComponentModelWithConnection.ShouldSaveConnection {
			get { return ShouldSaveConnection; }
			set { ShouldSaveConnection = value; }
		}
		#endregion
		#region IDataComponentModel Members
		public object DataSchema {
			get {
				return DataSourceType == DataSourceType.Xpo ?
					this.sqlDataSourceModel.DataSchema
					: DataSourceType == DataSourceType.Entity
						? this.efDataSourceModel.DataSchema
						: DataSourceType == DataSourceType.Object
							? this.objectDataSourceModel.DataSchema
							: this.excelDataSourceModel.DataSchema;
			}
			set {
				if(DataSourceType == DataSourceType.Xpo)
					this.sqlDataSourceModel.DataSchema = value;
				else if(DataSourceType == DataSourceType.Entity)
					this.efDataSourceModel.DataSchema = value;
				else if(DataSourceType == DataSourceType.Object)
					this.objectDataSourceModel.DataSchema = value;
				else
					this.excelDataSourceModel.DataSchema = value;
			}
		}
		#endregion
		#region IEFDataSourceModel Members
		EFDataConnection IEFDataSourceModel.DataConnection {
			get { return efDataSourceModel.DataConnection; }
			set { efDataSourceModel.DataConnection = value; }
		}
		IEntityFrameworkModelHelper IEFDataSourceModel.ModelHelper {
			get { return efDataSourceModel.ModelHelper; }
			set { efDataSourceModel.ModelHelper = value; }
		}
		EFConnectionParameters IEFDataSourceModel.ConnectionParameters {
			get { return efDataSourceModel.ConnectionParameters; }
			set { efDataSourceModel.ConnectionParameters = value; }
		}
		DataConnectionLocation IEFDataSourceModel.ConnectionStringLocation {
			get { return efDataSourceModel.ConnectionStringLocation; }
			set { efDataSourceModel.ConnectionStringLocation = value; }
		}
		public EFStoredProcedureInfo[] StoredProceduresInfo {
			get { return efDataSourceModel.StoredProceduresInfo; }
			set { efDataSourceModel.StoredProceduresInfo = value; }
		}
		public string DataMember {
			get { return efDataSourceModel.DataMember; }
			set { efDataSourceModel.DataMember = value; }
		}
		#endregion
		#region IObjectDataSourceModel Members
		public IDXAssemblyInfo Assembly {
			get { return this.objectDataSourceModel.Assembly; }
			set { this.objectDataSourceModel.Assembly = value; }
		}
		public IDXTypeInfo ObjectType {
			get { return this.objectDataSourceModel.ObjectType; }
			set { this.objectDataSourceModel.ObjectType = value; }
		}
		public ObjectMember ObjectMember {
			get { return this.objectDataSourceModel.ObjectMember; }
			set { this.objectDataSourceModel.ObjectMember = value; }
		}
		public Parameter[] MemberParameters {
			get { return this.objectDataSourceModel.MemberParameters; }
			set { this.objectDataSourceModel.MemberParameters = value; }
		}
		public ConstructorInfo ObjectConstructor {
			get { return this.objectDataSourceModel.ObjectConstructor; }
			set { this.objectDataSourceModel.ObjectConstructor = value; }
		}
		public Parameter[] CtorParameters {
			get { return this.objectDataSourceModel.CtorParameters; }
			set { this.objectDataSourceModel.CtorParameters = value; }
		}
		public ShowAllState ShowAllState {
			get { return this.objectDataSourceModel.ShowAllState; }
			set { this.objectDataSourceModel.ShowAllState = value; }
		}
		#endregion
		#region IExcelDataSourceModel Members
		public bool ShouldSavePassword {
			get { return excelDataSourceModel.ShouldSavePassword; }
			set { excelDataSourceModel.ShouldSavePassword = value; }
		}
		public ExcelSourceOptionsBase SourceOptions {
			get { return excelDataSourceModel.SourceOptions; }
			set { excelDataSourceModel.SourceOptions = value; }
		}
		public string FileName {
			get { return excelDataSourceModel.FileName; }
			set { excelDataSourceModel.FileName = value; }
		}
		public FieldInfo[] Schema {
			get { return excelDataSourceModel.Schema; }
			set { excelDataSourceModel.Schema = value; }
		}
		#endregion
	}
}
