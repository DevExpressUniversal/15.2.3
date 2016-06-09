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
using System.Diagnostics;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
namespace DevExpress.DataAccess.Wizard {
	public class DataComponentCreator {
		protected virtual ObjectDataSource CreateObjectDataSource() {
			return new ObjectDataSource();
		}
		protected virtual SqlDataSource CreateSqlDataSource() {
			return new SqlDataSource();
		}
		protected virtual EFDataSource CreateEFDataSource() {
			return new EFDataSource();
		}
		protected virtual ExcelDataSource CreateExcelDataSource() {
			return new ExcelDataSource();
		}
		protected virtual void SetupObjectDataSource(IObjectDataSourceModel model, ObjectDataSource ods) {
			try {
				ods.BeginUpdate();
				ods.DataSource = model.ObjectType.ResolveType();
				ods.DataMember = model.ObjectMember == null ? null : model.ObjectMember.Name;
				if(model.MemberParameters != null)
					ods.Parameters.AddRange(model.MemberParameters);
				if(model.ObjectConstructor != null) {
					if(model.CtorParameters == null || model.CtorParameters.Length == 0)
						ods.Constructor = ObjectConstructorInfo.Default;
					else
						ods.Constructor = new ObjectConstructorInfo(model.CtorParameters);
				}
				else
					ods.Constructor = null;
				ods.EndUpdate();
			}
			catch(DataMemberResolveException e) {
				Debug.Fail(e.Message, e.ToString());
			}
		}
		public virtual IDataComponent CreateDataComponent(IDataSourceModel model) {
			if(model.DataSourceType == DataSourceType.Xpo) {
				ISqlDataSourceModel sqlModel = model;
				if(sqlModel.DataConnection == null)
					return null;
				SqlDataSource sqlDataSource = CreateSqlDataSource();
				sqlDataSource.Name = model.DataSourceName;
				sqlDataSource.AssignConnection(sqlModel.DataConnection);
				SqlQuery query = sqlModel.Query;
				query.Name = sqlDataSource.Queries.GenerateUniqueName(query);
				sqlDataSource.Queries.Add(query);
				sqlDataSource.SetResultSchemaPart(((IDataComponent) sqlDataSource).DataMember, sqlModel.DataSchema);
				return sqlDataSource;
			}
			if(model.DataSourceType == DataSourceType.Entity) {
				IEFDataSourceModel efModel = model;
				EFDataSource efDataSource = CreateEFDataSource();
				efDataSource.Name = model.DataSourceName;
				efDataSource.ConnectionParameters = efModel.DataConnection.ConnectionParameters;
				efDataSource.Connection.ConnectionStringsProvider = efModel.DataConnection.ConnectionStringsProvider;
				efDataSource.Connection.SolutionTypesProvider = efModel.DataConnection.SolutionTypesProvider;
				if(efModel.StoredProceduresInfo != null)
					efDataSource.StoredProcedures.AddRange(efModel.StoredProceduresInfo);
				if(efModel.DataMember != null)
					efDataSource.DataMember = efModel.DataMember;
				return efDataSource;
			}
			if(model.DataSourceType == DataSourceType.Object) {
				var objectDataSource = CreateDataComponent((IObjectDataSourceModel) model);
				objectDataSource.Name = model.DataSourceName;
				return objectDataSource;
			}
			if(model.DataSourceType == DataSourceType.Excel) {
				IExcelDataSourceModel excelModel = model;
				ExcelDataSource excelDataSource = CreateExcelDataSource();
				excelDataSource.Name = model.DataSourceName;
				excelDataSource.FileName = model.FileName;
				excelDataSource.Schema.AddRange(excelModel.Schema);
				excelDataSource.SourceOptions = excelModel.SourceOptions;
				ExcelSourceOptions excelOptions = excelModel.SourceOptions as ExcelSourceOptions;
				if(excelOptions != null && !string.IsNullOrEmpty(excelOptions.Password) && !excelModel.ShouldSavePassword) {
					excelOptions.PasswordInternal = excelOptions.Password;
					excelOptions.Password = null;
				}
				return excelDataSource;
			}
			throw new ArgumentOutOfRangeException();
		}
		public ObjectDataSource CreateDataComponent(IObjectDataSourceModel model) {
			var ods = CreateObjectDataSource();
			SetupObjectDataSource(model, ods);
			return ods;
		}
		public static void SaveConnectionIfShould(IDataComponentModelWithConnection model, IConnectionStorageService storage) {
			if(model.DataConnection == null)
				return;
			SaveConnectionIfShould(model.ShouldSaveConnection, model.DataConnection, model.ConnectionName, storage);
		}
		static void SaveConnectionIfShould(SaveConnectionMethod shouldSave, IDataConnection connection, string name, IConnectionStorageService storage) {
			if(shouldSave.HasFlag(SaveConnectionMethod.SaveToAppConfig)) {
				if(connection.StoreConnectionNameOnly)
					return;
				storage.SaveConnection(name, connection, shouldSave.HasFlag(SaveConnectionMethod.KeepCredentials));
			}
			connection.Name = name;
		}
	}
}
