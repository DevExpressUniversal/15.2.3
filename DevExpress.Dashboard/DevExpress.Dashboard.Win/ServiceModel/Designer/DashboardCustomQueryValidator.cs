#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.DashboardCommon;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils;
using System;
namespace DevExpress.DashboardWin.ServiceModel {
	public interface IDataSourceEditContext {
		void BeginEditDataSource(DashboardSqlDataSource dataSource);
		void EndEditDataSource();
	}
	public class DashboardCustomQueryValidator : CustomQueryValidator, IDataSourceEditContext {
		readonly DashboardDesigner designer;
		DashboardSqlDataSource dataSource;
		public DashboardCustomQueryValidator(DashboardDesigner designer) {
			Guard.ArgumentNotNull(designer, "designer");
			this.designer = designer;
		}
		public override bool Validate(DataConnectionParametersBase connectionParameters, string sql, ref string message) {
			bool valid = base.Validate(connectionParameters, sql, ref message);
			var sqlQuery = new CustomSqlQuery(string.Empty, sql);
			var innerArgs = new ValidateCustomSqlQueryEventArgs(sqlQuery, valid) { ExceptionMessage = message };
			ValidateDashboardCustomSqlQueryEventArgs args;
			if(dataSource != null)
				args = new ValidateDashboardCustomSqlQueryEventArgs(dataSource, innerArgs);
			else
				args = new ValidateDashboardCustomSqlQueryEventArgs(connectionParameters, innerArgs);
			designer.RaiseValidateCustomSqlQuery(this, args);
			message = args.ExceptionMessage;
			return args.Valid;
		}
		#region IDataSourceEditContext
		void IDataSourceEditContext.BeginEditDataSource(DashboardSqlDataSource dataSource) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			if (this.dataSource == null)
				this.dataSource = dataSource;
			else
				throw new InvalidOperationException("The data source was already set");
		}
		void IDataSourceEditContext.EndEditDataSource() {
			if (dataSource != null)
				dataSource = null;
			else
				throw new InvalidOperationException("The data source was already cleared");
		}
		#endregion
	}
}
