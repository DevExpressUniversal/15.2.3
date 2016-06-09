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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Design;
using DevExpress.DataAccess.UI.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public class FilterStringEditor : UITypeEditor {
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			TableQuery tableQuery = context.Instance as TableQuery;
			if(tableQuery == null)
				return base.EditValue(context, provider, value);
			IUIService uiService = context.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IDesignerHost designerHost = context.GetService<IDesignerHost>();
			XRSqlDataSourceDesigner designer = (XRSqlDataSourceDesigner)designerHost.GetDesigner(tableQuery.Owner.DataSource);
			using(UserLookAndFeel lookAndFeel = designer.GetLookAndFeel(context)) {
				var waitFormActivator = new WaitFormActivatorDesignTime(owner, typeof(WaitFormWithCancel), lookAndFeel.ActiveSkinName);
				IExceptionHandler exceptionHandler = new LoaderExceptionHandler(owner, lookAndFeel);
				var sqlDataSource = tableQuery.DataSource;
				var dataConnectionParametersService = sqlDataSource.GetService<IDataConnectionParametersService>();
				if(!tableQuery.DataSource.ValidateConnection(lookAndFeel, owner) || !ConnectionHelper.OpenConnection(tableQuery.DataSource.Connection, exceptionHandler, waitFormActivator, dataConnectionParametersService))
					return base.EditValue(context, provider, value);
				var dataLoader = new UIDataLoader(waitFormActivator, exceptionHandler);
				IDBSchemaProvider dbSchemaProvider = context.GetService<IDBSchemaProvider>();
				string[] tableNames = tableQuery.Tables.Select(t => t.Name).ToArray();
				DBSchema dbSchema = dataLoader.LoadSchema(dbSchemaProvider, tableQuery.DataSource.Connection, tableNames);
				if(dbSchema == null)
					return base.EditValue(context, provider, value);
				IParameterService parameterService = context.GetService<IParameterService>();
				return EditValueCore(tableQuery, dbSchema, owner, lookAndFeel, parameterService, provider) ?? value;
			}
		}
		protected virtual string EditValueCore(TableQuery tableQuery, DBSchema dbSchema, IWin32Window owner, UserLookAndFeel lookAndFeel, IParameterService parameterService, IServiceProvider propertyGridServices) {
			var filterModel = new FilterModel(tableQuery.FilterString, tableQuery.Parameters);
			DBTable[] dbObjects = dbSchema.Tables.Union(dbSchema.Views).ToArray();
			Dictionary<string, DBTable> dbTables = tableQuery.Tables.ToDictionary(table => table.Name,
				table => dbObjects.FirstOrDefault(t => string.Equals(t.Name, table.Name, StringComparison.Ordinal)));
			var queryFilterControl = new QueryFilterControl(parameterService, propertyGridServices);
			var filterView = new FilterView(owner, lookAndFeel, null, queryFilterControl);
			FilterPresenter<FilterModel, FilterView> filterPresenter =
				new FilterPresenter<FilterModel, FilterView>(filterModel, filterView, tableQuery,
					dbTables);
			filterPresenter.InitView();
			filterView.SetExistingParameters(tableQuery.Parameters);
			if(!filterPresenter.Do())
				return null;
			tableQuery.Parameters.Clear();
			tableQuery.Parameters.AddRange(filterModel.Parameters);
			return filterModel.FilterString;
		}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.Modal;
		}
	}
}
