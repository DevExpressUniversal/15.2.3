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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.Xpo.DB;
namespace DevExpress.DashboardWin.Commands {
	public static class SqlQueryFilterFormHelper {
		public static bool EditFilter(this TableQuery tableQuery, IParameterService parameterService, IServiceProvider propertyGridServices, UserLookAndFeel userLookAndFeel, IWin32Window win32Window){
			FilterModel filterModel = new FilterModel(tableQuery.FilterString, tableQuery.Parameters);
			IEnumerable<DBTable> dbObjects = tableQuery.DataSource.DBSchema.Tables.Concat(tableQuery.DataSource.DBSchema.Views);
			Dictionary<string, DBTable> dbTables = tableQuery.Tables.ToDictionary(table => table.Name, table => dbObjects.FirstOrDefault(t => string.Equals(t.Name, table.Name, StringComparison.Ordinal)));
			QueryFilterControl filterControl = new QueryFilterControl(parameterService, propertyGridServices);
			FilterView filterView = new FilterView(win32Window, userLookAndFeel, null, filterControl);
			FilterPresenter<FilterModel, FilterView> presenter = new FilterPresenter<FilterModel, FilterView>(filterModel, filterView, tableQuery, dbTables);
			presenter.InitView();
			filterView.SetExistingParameters(tableQuery.Parameters);
			if(presenter.Do()) {
				tableQuery.FilterString = filterModel.FilterString;
				tableQuery.Parameters.Clear();
				tableQuery.Parameters.AddRange(filterModel.Parameters);
				return true;
			}
			return false;
		}
	}
}
