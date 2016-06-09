#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Linq;
using System.Web;
using System.Collections.Generic;
using DevExpress.DataAccess.Sql;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using DevExpress.XtraReports.Web.ReportDesigner.Native.Services;
namespace DevExpress.XtraReports.Web.QueryBuilder.Native {
	public static class QueryBuilderInputLoader {
		private static string GetArgumentFromString(string argumentName, Dictionary<string, string> arguments) {
			string result;
			if(!arguments.TryGetValue(argumentName, out result)) {
				throw new InvalidOperationException(string.Format("There is no '{0}' argument", argumentName));
			}
			return HttpUtility.UrlDecode(result);
		}
		private static Dictionary<string, string> GetArgumentsDictionary(string argumentsString) {
			if(string.IsNullOrEmpty(argumentsString)) {
				return null;
			}
			return argumentsString.Split(new[] { '&' }, 2)
				.Select(x => x.Split(new[] { '=' }, 2))
				.ToDictionary(x => x[0], x => x.Length > 1 ? x[1] : "");
		}
		public static QueryBuilderInput FromString(string argument, bool validateQueryByExecution) {
			if(string.IsNullOrEmpty(argument)) {
				return null;
			}
			Dictionary<string, string> argumentsDictionary = GetArgumentsDictionary(argument);
			string queryJSON = GetArgumentFromString("queryLayout", argumentsDictionary);
			string dataSourceBase64 = GetArgumentFromString("dataSourceBase64", argumentsDictionary);
			var wizardService = DefaultQueryBuilderContainer.Current.GetService<ISqlDataSourceWizardService>();
			var request = new SelectStatementRequest() { DataSourceBase64 = dataSourceBase64, SqlQueryJSON = queryJSON };
			TableQuery tableQuery = null;
			string selectStatement = wizardService.GetSelectStatement(request, out tableQuery, validateQueryByExecution).SqlSelectStatement;
			return new QueryBuilderInput(selectStatement, tableQuery);
		}
	}
}
