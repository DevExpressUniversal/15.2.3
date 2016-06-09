﻿#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DataAccess;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardDataComponentCreator : DataComponentCreator {		
		protected override SqlDataSource CreateSqlDataSource() {
			return new DashboardSqlDataSource();
		}
		protected override EFDataSource CreateEFDataSource() {
			return new DashboardEFDataSource();
		}
		protected override ObjectDataSource CreateObjectDataSource() {
			return new DashboardObjectDataSource();
		}
		protected override ExcelDataSource CreateExcelDataSource() {
			return new DashboardExcelDataSource();
		}
		public override IDataComponent CreateDataComponent(IDataSourceModel model) {
			if(model.DataSourceType == DashboardDataSourceType.Olap) {
				IDataConnection connection = ((IDataComponentModelWithConnection)model).DataConnection;
				DashboardOlapDataSource dataSource = new DashboardOlapDataSource(((OlapDataConnection)connection).ConnectionParameters);
				dataSource.ConnectionName = model.ConnectionName;
				dataSource.StoreConnectionNameOnly = connection.StoreConnectionNameOnly;
				return dataSource;
			} else if(model.DataSourceType == DashboardDataSourceType.XmlSchema) { 
				DashboardObjectDataSource dataSource = (DashboardObjectDataSource)CreateObjectDataSource();
				dataSource.DataSchema = (string)model.DataSchema;
				return dataSource;
			} else {
				return base.CreateDataComponent(model);
			}
		}
	}
}
