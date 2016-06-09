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
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Design;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Native.Sql;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DataAccess.UI.Sql {
	public static class SqlDataSourceUIHelper {
		#region inner classes
		internal class WizardRunner<TModel> : DataSourceWizardRunnerBase<TModel, ISqlDataSourceWizardClientUI> where TModel : ISqlDataSourceModel, new() {
			Type startPage = typeof(ChooseConnectionPage<TModel>);
			public WizardRunner(IWizardRunnerContext context)
				: base(context) { }
			protected override Type StartPage { get { return startPage; } }
			protected override string WizardTitle { get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionEditorTitle); } }
			protected override WizardPageFactoryBase<TModel, ISqlDataSourceWizardClientUI> CreatePageFactory(ISqlDataSourceWizardClientUI client) {
				this.startPage = client.DataConnections.Count() > 0 ?
					typeof(ChooseConnectionPage<TModel>) :
					typeof(ConnectionPropertiesPage<TModel>);
				return new PageFactory<TModel>(client);
			}
		}
		class PageFactory<TModel> : SqlWizardPageFactory<TModel, ISqlDataSourceWizardClientUI> where TModel : ISqlDataSourceModel, new() {
			public PageFactory(ISqlDataSourceWizardClientUI client)
				: base(client) { }
			protected override void RegisterDependencies(ISqlDataSourceWizardClientUI client) {
				base.RegisterDependencies(client);
				Container.RegisterType<ChooseConnectionPage<TModel>, ChooseConnectionPageEx<TModel>>();
				Container.RegisterType<ConnectionPropertiesPage<TModel>, ConnectionPropertiesPageEx<TModel>>();
				Container.RegisterType<SaveConnectionPage<TModel>, SaveConnectionPageEx<TModel>>();
				Container.RegisterType<IChooseConnectionPageView, ChooseConnectionPageView>();
				Container.RegisterType<IConnectionPropertiesPageView, ConnectionPropertiesPageView>();
				Container.RegisterType<ISaveConnectionPageView, SaveConnectionPageView>();
			}
		}
		class EditConnectionParametersPageFactory<TModel> : SqlWizardPageFactory<TModel, ISqlDataSourceWizardClientUI> where TModel : ISqlDataSourceModel, new() {
			public EditConnectionParametersPageFactory(ISqlDataSourceWizardClientUI client) : base(client) {}
			protected override void RegisterDependencies(ISqlDataSourceWizardClientUI client) {
				base.RegisterDependencies(client);
				Container.RegisterType<ConnectionPropertiesPage<TModel>, ConnectionPropertiesPageExEx<TModel>>();
				Container.RegisterType<IConnectionPropertiesPageView, ConnectionPropertiesPageView>();
			}
		}
		class ConnectionPropertiesPageExEx<TModel> : ConnectionPropertiesPage<TModel>
			where TModel : ISqlDataSourceModel, new() {
			public ConnectionPropertiesPageExEx(IConnectionPropertiesPageView view, IWizardRunnerContext context, IDataConnectionParametersService dataConnectionParametersService, IConnectionStorageService connectionStorageService, IEnumerable<SqlDataConnection> dataConnections, SqlWizardOptions options) : base(view, context, dataConnectionParametersService, connectionStorageService, dataConnections, options) {}
			public override bool FinishEnabled {
				get { return true; }
			}
			public override bool MoveNextEnabled {
				get { return false; }
			}
		}
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "The class is created through late-bound reflection methods such as CreateInstance.")]
		class ChooseConnectionPageEx<TModel> : ChooseConnectionPage<TModel>
			where TModel : ISqlDataSourceModel, new() {
			public ChooseConnectionPageEx(IChooseConnectionPageView view, IWizardRunnerContext context, IDataConnectionParametersService dataConnectionParametersService, IConnectionStorageService connectionStorageService, IEnumerable<SqlDataConnection> dataConnections, SqlWizardOptions options)
				: base(view, context, dataConnectionParametersService, connectionStorageService, dataConnections, options) { }
			public override bool MoveNextEnabled { get { return base.GetNextPageType() != typeof(ConfigureQueryPage<TModel>); } }
			public override bool FinishEnabled { get { return base.FinishEnabled || base.GetNextPageType() == typeof(ConfigureQueryPage<TModel>); } }
		}
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "The class is created through late-bound reflection methods such as CreateInstance.")]
		class ConnectionPropertiesPageEx<TModel> : ConnectionPropertiesPage<TModel>
			where TModel : ISqlDataSourceModel, new() {
			public ConnectionPropertiesPageEx(IConnectionPropertiesPageView view, IWizardRunnerContext context, IDataConnectionParametersService dataConnectionParametersService, IConnectionStorageService connectionStorageService, IEnumerable<SqlDataConnection> dataConnections, SqlWizardOptions options)
				: base(view, context, dataConnectionParametersService, connectionStorageService, dataConnections, options) { }
			public override bool MoveNextEnabled { get { return base.GetNextPageType() != typeof(ConfigureQueryPage<TModel>); } }
			public override bool FinishEnabled { get { return base.FinishEnabled || base.GetNextPageType() == typeof(ConfigureQueryPage<TModel>); } }
		}
		[SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "The class is created through late-bound reflection methods such as CreateInstance.")]
		class SaveConnectionPageEx<TModel> : SaveConnectionPage<TModel>
			where TModel : ISqlDataSourceModel, new() {
			public SaveConnectionPageEx(ISaveConnectionPageView view, IWizardRunnerContext context, IConnectionStorageService connectionStorageService)
				: base(view, context, connectionStorageService) { }
			public override bool MoveNextEnabled { get { return false; } }
			public override bool FinishEnabled { get { return true; } }
		}
		class EditQueryWizardRunner<TModel> : DataSourceWizardRunnerBase<TModel, ISqlDataSourceWizardClientUI> where TModel : ISqlDataSourceModel, new() {
			public EditQueryWizardRunner(IWizardRunnerContext context)
				: base(context) {
			}
			protected override Type StartPage {
				get {
					return typeof(ConfigureQueryPage<TModel>);
				}
			}
			protected override string WizardTitle {
				get {
					return DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryEditorTitle);
				}
			}
			protected override WizardPageFactoryBase<TModel, ISqlDataSourceWizardClientUI> CreatePageFactory(ISqlDataSourceWizardClientUI client) {
				return new SqlWizardPageFactory<TModel, ISqlDataSourceWizardClientUI>(client);
			}
		}
		class EditConnectionParametersWizardRunner<TModel> : DataSourceWizardRunnerBase<TModel, ISqlDataSourceWizardClientUI> where TModel : ISqlDataSourceModel, new() {
			public EditConnectionParametersWizardRunner(IWizardRunnerContext context) : base(context) { }
			protected override Type StartPage {
				get { return typeof(ConnectionPropertiesPage<TModel>); }
			}
			protected override string WizardTitle {
				get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.ConnectionEditorTitle); }
			}
			protected override WizardPageFactoryBase<TModel, ISqlDataSourceWizardClientUI> CreatePageFactory(ISqlDataSourceWizardClientUI client) {
				return new EditConnectionParametersPageFactory<TModel>(client);
			}
		}
		#endregion
		#region ManageQueries(...)
		[Obsolete("This overload is obsolete now. Use ManageQueries(this SqlDataSource sqlDataSource, EditQueryContext context) instead.")]
		public static bool ManageQueries(this SqlDataSource sqlDataSource) {
			return ManageQueries(sqlDataSource, new EditQueryContext());
		}
		[Obsolete("This overload is obsolete now. Use ManageQueries(this SqlDataSource sqlDataSource, EditQueryContext context) instead.")]
		public static bool ManageQueries(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel) {
			return ManageQueries(sqlDataSource, new EditQueryContext { LookAndFeel = lookAndFeel });
		}
		[Obsolete("This overload is obsolete now. Use ManageQueries(this SqlDataSource sqlDataSource, EditQueryContext context) instead.")]
		public static bool ManageQueries(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner) {
				return ManageQueries(sqlDataSource, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner
			});
		}
		[Obsolete("This overload is obsolete now. Use ManageQueries(this SqlDataSource sqlDataSource, EditQueryContext context) instead.")]
		public static bool ManageQueries(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner, IDBSchemaProvider dbSchemaProvider) {
				return ManageQueries(sqlDataSource, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider
			});
		}
		[Obsolete("This overload is obsolete now. Use ManageQueries(this SqlDataSource sqlDataSource, EditQueryContext context) instead.")]
		public static bool ManageQueries(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner, IDBSchemaProvider dbSchemaProvider, IParameterService parameterService) {
				return ManageQueries(sqlDataSource, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService
			});
		}
		[Obsolete("This overload is obsolete now. Use ManageQueries<TModel>(this SqlDataSource sqlDataSource, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool ManageQueries(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner, IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, Action<IWizardCustomization<SqlDataSourceModel>> callback) {
				return ManageQueries(sqlDataSource, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService,
			}, callback);
		}
		[Obsolete("This overload is obsolete now. Use ManageQueries<TModel>(this SqlDataSource sqlDataSource, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool ManageQueries(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner, IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, Action<IWizardCustomization<SqlDataSourceModel>> callback, ICustomQueryValidator customQueryValidator) {
				return ManageQueries(sqlDataSource, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService,
				QueryValidator = customQueryValidator
			}, callback);
		}
		public static bool ManageQueries(this SqlDataSource sqlDataSource, EditQueryContext context) {
			return ManageQueries<SqlDataSourceModel>(sqlDataSource, context, _ => { });
		}
		public static bool ManageQueries<TModel>(this SqlDataSource sqlDataSource, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) where TModel : ISqlDataSourceModel, new() {
			Guard.ArgumentNotNull(sqlDataSource, "sqlDataSource");
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(context.DBSchemaProvider, "context.DBSchemaProvider");
			IExceptionHandler exceptionHandler = context.Owner != null
				? (IExceptionHandler)new ConnectionExceptionHandler(context.Owner, context.LookAndFeel)
				: new EmptyExceptionHandler();
			IWaitFormActivator waitFormActivator = CreateWaitFormActivator(context.Owner, context.LookAndFeel);
			IDataConnectionParametersService dataConnectionParametersService =
				sqlDataSource.GetService<IDataConnectionParametersService>();
			if(!sqlDataSource.ValidateConnection(context.LookAndFeel, context.Owner) ||
			   !ConnectionHelper.OpenConnection(sqlDataSource.Connection, exceptionHandler, waitFormActivator,
				   dataConnectionParametersService))
				return false;
			using(SqlDataSource clone = new SqlDataSource()) {
				clone.AssignConnection(sqlDataSource.Connection);
				((IServiceContainer)clone).RemoveService(typeof(IDataConnectionParametersService));
				((IServiceContainer)clone).AddService(typeof(IDataConnectionParametersService),
					dataConnectionParametersService);
				clone.UpdateResultSet(((ResultSet)sqlDataSource.ResultSet.Clone()).Tables);
				clone.Relations.AddRange(sqlDataSource.Relations.ToArray());
				clone.Queries.AddRange(sqlDataSource.Queries.Select(query => query.Clone()).ToList());
				try {
					using(var form = new SqlQueryCollectionEditorForm<TModel>(clone.Queries, context, customizeWizard)) {
						if(form.ShowDialog(context.Owner) == DialogResult.OK) {
							sqlDataSource.Queries.Clear();
							sqlDataSource.Queries.AddRange(form.Queries);
							sqlDataSource.Relations.Clear();
							sqlDataSource.Relations.AddRange(clone.Relations.ToArray());
							sqlDataSource.UpdateResultSet(clone.ResultSet.Tables);
							return true;
						}
					}
				}
				finally {
					clone.AssignConnection(null);
				}
			}
			return false;
		}
		#endregion
		#region ManageRelations(...)
		[Obsolete("This overload is obsolete now. Use ManageRelations(this SqlDataSource sqlDataSource, ManageRelationsContext context) instead.")]
		public static bool ManageRelations(this SqlDataSource sqlDataSource) {
			return ManageRelations(sqlDataSource, new ManageRelationsContext());
		}
		[Obsolete("This overload is obsolete now. Use ManageRelations(this SqlDataSource sqlDataSource, ManageRelationsContext context) instead.")]
		public static bool ManageRelations(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel) {
			return ManageRelations(sqlDataSource, new ManageRelationsContext { LookAndFeel = lookAndFeel });
		}
		[Obsolete("This overload is obsolete now. Use ManageRelations(this SqlDataSource sqlDataSource, ManageRelationsContext context) instead.")]
		public static bool ManageRelations(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner) {
			return ManageRelations(sqlDataSource, new ManageRelationsContext {
				LookAndFeel = lookAndFeel,
				Owner = owner
			});
		}
		[Obsolete("This overload is obsolete now. Use ManageRelations(this SqlDataSource sqlDataSource, ManageRelationsContext context) instead.")]
		public static bool ManageRelations(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel, IWin32Window owner, IDBSchemaProvider dbSchemaProvider) {
			return ManageRelations(sqlDataSource, new ManageRelationsContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider
			});
		}
		public static bool ManageRelations(this SqlDataSource sqlDataSource, ManageRelationsContext context) {
			Guard.ArgumentNotNull(sqlDataSource, "sqlDataSource");
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(context.DBSchemaProvider, "context.DBSchemaProvider");
			IWaitFormActivator waitFormActivator = CreateWaitFormActivator(context.Owner, context.LookAndFeel);
			IExceptionHandler exceptionHandler = context.Owner != null
				? (IExceptionHandler)new LoaderExceptionHandler(context.Owner, context.LookAndFeel)
				: new EmptyExceptionHandler();
			IDataConnectionParametersService dataConnectionParametersService =
				sqlDataSource.GetService<IDataConnectionParametersService>();
			if(!sqlDataSource.ValidateConnection(context.LookAndFeel, context.Owner) ||
			   !ConnectionHelper.OpenConnection(sqlDataSource.Connection, exceptionHandler, waitFormActivator, dataConnectionParametersService))
				return false;
			UIDataLoader dataLoader = new UIDataLoader(waitFormActivator, exceptionHandler);
			HashSet<string> tableNames = new HashSet<string>();
			foreach(TableQuery tableQuery in sqlDataSource.Queries.OfType<TableQuery>()) {
				tableQuery.Tables.ForEach(t => tableNames.Add(t.Name));
			}
			DBSchema dbSchema = dataLoader.LoadSchema(context.DBSchemaProvider, sqlDataSource.Connection, tableNames.ToArray());
			if(dbSchema == null)
				return false;
			sqlDataSource.DBSchema = dbSchema;
			if(sqlDataSource.Queries.Count < 2 && sqlDataSource.Relations.Count == 0) {
				XtraMessageBox.Show(context.LookAndFeel, context.Owner,
					DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageLessThanTwoQueries),
					DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageBoxButtons.OK,
					MessageBoxIcon.Warning);
				return false;
			}
			using(MasterDetailEditorForm editorForm = new MasterDetailEditorForm(context.LookAndFeel)) {
				editorForm.DataSource = sqlDataSource;
				if(editorForm.ShowDialog(context.Owner) == DialogResult.OK) {
					sqlDataSource.Relations.Clear();
					IEnumerable<MasterDetailInfo> relations = editorForm.Relations;
					foreach(MasterDetailInfo info in relations)
						sqlDataSource.Relations.Add(info);
					sqlDataSource.UpdateResultSchemaRelations();
					return true;
				}
			}
			return false;
		}
		#endregion
		#region ConfigureConnectionParameters(...)
		[Obsolete("This overload is obsolete now. Use ConfigureConnectionParameters(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnectionParameters(this SqlDataSource sqlDataSource) {
			return ConfigureConnectionParameters(sqlDataSource, new ConfigureConnectionContext());
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnectionParameters(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnectionParameters(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel) {
			return ConfigureConnectionParameters(sqlDataSource, new ConfigureConnectionContext { LookAndFeel = lookAndFeel });
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnectionParameters(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnectionParameters(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner) {
			return ConfigureConnectionParameters(sqlDataSource, new ConfigureConnectionContext { LookAndFeel = lookAndFeel, Owner = owner });
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnectionParameters(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnectionParameters(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner, IConnectionStorageService connectionStorageService) {
			return ConfigureConnectionParameters(sqlDataSource,
				new ConfigureConnectionContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					ConnectionStorageService = connectionStorageService
				});
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnectionParameters(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnectionParameters(this SqlDataSource sqlDataSource,
			IWizardRunnerContext context, IConnectionStorageService connectionStorageService) {
			return ConfigureConnectionParameters(sqlDataSource,
				new ConfigureConnectionContext {
					WizardContext = context,
					ConnectionStorageService = connectionStorageService
				});
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnectionParameters<TModel>(this SqlDataSource sqlDataSource, ConfigureConnectionContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool ConfigureConnectionParameters<TModel>(this SqlDataSource sqlDataSource,
			IWizardRunnerContext context, IConnectionStorageService connectionStorageService, Action<IWizardCustomization<TModel>> callback) 
			where TModel : ISqlDataSourceModel, new() {
			return ConfigureConnectionParameters(sqlDataSource,
				new ConfigureConnectionContext {
					WizardContext = context,
					ConnectionStorageService = connectionStorageService
				}, callback);
		}
		public static bool ConfigureConnectionParameters(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) {
			return ConfigureConnectionParameters<SqlDataSourceModel>(sqlDataSource, context, _ => { });
		}
		public static bool ConfigureConnectionParameters<TModel>(this SqlDataSource sqlDataSource, ConfigureConnectionContext context, Action<IWizardCustomization<TModel>> customizeWizard) where TModel : ISqlDataSourceModel, new() {
			Guard.ArgumentNotNull(sqlDataSource, "sqlDataSource");
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(context.ConnectionStorageService, "context.ConnectionStorageService");
			SqlDataConnection connection = sqlDataSource.ValidateConnection();
			IDataConnectionParametersService dataConnectionParametersService =
				sqlDataSource.GetService<IDataConnectionParametersService>();
			TModel model = new TModel();
			if(connection != null)
				model.DataConnection = new SqlDataConnection(connection.Name,
					connection.CreateDataConnectionParameters()) {
						StoreConnectionNameOnly = connection.StoreConnectionNameOnly
					};
			var runner = new EditConnectionParametersWizardRunner<TModel>(context.WizardContext);
			var client = new SqlDataSourceWizardClientUI(context.ConnectionStorageService, null, null) {
				DataConnectionParametersProvider = dataConnectionParametersService
			};
			if(runner.Run(client, model, customizeWizard)) {
				sqlDataSource.AssignConnection(runner.WizardModel.DataConnection);
				return true;
			}
			return false;
		}
		#endregion
		#region ConfigureConnection(...)
		[Obsolete("This overload is obsolete now. Use ConfigureConnection(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnection(this SqlDataSource sqlDataSource) {
			return ConfigureConnection(sqlDataSource, new ConfigureConnectionContext());
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnection(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnection(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel) {
			return ConfigureConnection(sqlDataSource, new ConfigureConnectionContext { LookAndFeel = lookAndFeel });
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnection(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnection(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner) {
			return ConfigureConnection(sqlDataSource, new ConfigureConnectionContext {
				LookAndFeel = lookAndFeel,
				Owner = owner
			});
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnection(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnection(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner, IConnectionStorageService connectionStorageService) {
			return ConfigureConnection(sqlDataSource,
				new ConfigureConnectionContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					ConnectionStorageService = connectionStorageService
				});
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnection(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) instead.")]
		public static bool ConfigureConnection(this SqlDataSource sqlDataSource,
			IWizardRunnerContext wizardRunnerConext, IConnectionStorageService connectionStorageService) {
			return ConfigureConnection(sqlDataSource, new ConfigureConnectionContext{ WizardContext = wizardRunnerConext, ConnectionStorageService = connectionStorageService });
		}
		[Obsolete("This overload is obsolete now. Use ConfigureConnection<TModel>(this SqlDataSource sqlDataSource, ConfigureConnectionContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool ConfigureConnection<TModel>(this SqlDataSource sqlDataSource,
			IWizardRunnerContext wizardRunnerConext, IConnectionStorageService connectionStorageService, Action<IWizardCustomization<TModel>> callback) 
			where TModel : ISqlDataSourceModel, new() {
			return ConfigureConnection(sqlDataSource,
				new ConfigureConnectionContext {
					WizardContext = wizardRunnerConext,
					ConnectionStorageService = connectionStorageService
				}, callback);
		}
		public static bool ConfigureConnection(this SqlDataSource sqlDataSource, ConfigureConnectionContext context) {
			return ConfigureConnection<SqlDataSourceModel>(sqlDataSource, context, _ => { });
		}
		public static bool ConfigureConnection<TModel>(this SqlDataSource sqlDataSource, ConfigureConnectionContext context, Action<IWizardCustomization<TModel>> customizeWizard)
			where TModel : ISqlDataSourceModel, new() {
			Guard.ArgumentNotNull(sqlDataSource, "sqlDataSource");
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(context.ConnectionStorageService, "context.ConnectionStorageService");
			SqlDataConnection connection = sqlDataSource.ValidateConnection();
			var dataConnectionParametersService = sqlDataSource.GetService<IDataConnectionParametersService>();
			TModel model = new TModel();
			if(connection != null)
				model.DataConnection = new SqlDataConnection(connection.Name,
					connection.CreateDataConnectionParameters()) {
						StoreConnectionNameOnly = connection.StoreConnectionNameOnly
					};
			var runner = new WizardRunner<TModel>(context.WizardContext);
			var client = new SqlDataSourceWizardClientUI(context.ConnectionStorageService, null, null) {
				DataConnectionParametersProvider = dataConnectionParametersService
			};
			if(runner.Run(client, model, customizeWizard)) {
				DataComponentCreator.SaveConnectionIfShould(runner.WizardModel, context.ConnectionStorageService);
				sqlDataSource.AssignConnection(runner.WizardModel.DataConnection);
				return true;
			}
			return false;
		}
		#endregion
		#region EditQuery(...)
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query) { return EditQuery(query, new EditQueryContext()); }
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query, IRepositoryItemsProvider repositoryItemsProvider) {
			return EditQuery(query, new EditQueryContext { RepositoryItemsProvider = repositoryItemsProvider });
		}
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query, UserLookAndFeel lookAndFeel) {
			return EditQuery(query, new EditQueryContext { LookAndFeel = lookAndFeel });
		}
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query, UserLookAndFeel lookAndFeel, IRepositoryItemsProvider repositoryItemsProvider) {
			return EditQuery(query, new EditQueryContext { LookAndFeel = lookAndFeel, RepositoryItemsProvider = repositoryItemsProvider });
		}
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner) {
			return EditQuery(query, new EditQueryContext { LookAndFeel = lookAndFeel, Owner = owner });
		}
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner, IRepositoryItemsProvider repositoryItemsProvider) {
			return EditQuery(query, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				RepositoryItemsProvider = repositoryItemsProvider
			});
		}
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider) { 
			return EditQuery(query, new EditQueryContext {
				LookAndFeel = lookAndFeel, 
				Owner = owner, 
				DBSchemaProvider = dbSchemaProvider
			}); }
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IRepositoryItemsProvider repositoryItemsProvider) {
			return EditQuery(query, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				RepositoryItemsProvider = repositoryItemsProvider
			});
		}
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService) {
			return EditQuery(query, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService
			});
		}
		[Obsolete("This overload is obsolete now. Use EditQuery<TModel>(this SqlQuery query, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool EditQuery<TModel>(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, Action<IWizardCustomization<TModel>> callback) 
			where TModel : ISqlDataSourceModel, new() {
			return EditQuery(query, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService
			}, callback);
		}
		[Obsolete("This overload is obsolete now. Use EditQuery<TModel>(this SqlQuery query, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool EditQuery<TModel>(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, Action<IWizardCustomization<TModel>> callback, IServiceProvider propertyGridServices) 
			where TModel : ISqlDataSourceModel, new() {
			return EditQuery(query, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService,
				PropertyGridServices = propertyGridServices
			}, callback);
		}
		[Obsolete("This overload is obsolete now. Use EditQuery(this SqlQuery query, EditQueryContext context) instead.")]
		public static bool EditQuery(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider) {
			return EditQuery(query, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService,
				RepositoryItemsProvider = repositoryItemsProvider
			});
		}
		[Obsolete("This overload is obsolete now. Use EditQuery<TModel>(this SqlQuery query, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool EditQuery<TModel>(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider, Action<IWizardCustomization<TModel>> callback) 
			where TModel : ISqlDataSourceModel, new() {
			return EditQuery(query, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService,
				RepositoryItemsProvider = repositoryItemsProvider
			}, callback);
		}
		[Obsolete("This overload is obsolete now. Use EditQuery<TModel>(this SqlQuery query, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool EditQuery<TModel>(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider, Action<IWizardCustomization<TModel>> callback, IServiceProvider propertyGridServices)
			where TModel : ISqlDataSourceModel, new() {
			return EditQuery(query, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService,
				RepositoryItemsProvider = repositoryItemsProvider,
				PropertyGridServices = propertyGridServices
			}, callback);
		}
		[Obsolete("This overload is obsolete now. Use EditQuery<TModel>(this SqlQuery query, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool EditQuery<TModel>(this SqlQuery query, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService,
			IRepositoryItemsProvider repositoryItemsProvider, Action<IWizardCustomization<TModel>> callback,
			IServiceProvider propertyGridServices, ICustomQueryValidator customQueryValidator)
			where TModel : ISqlDataSourceModel, new() {
			return EditQuery(query,
				new EditQueryContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					DBSchemaProvider = dbSchemaProvider,
					ParameterService = parameterService,
					RepositoryItemsProvider = repositoryItemsProvider,
					PropertyGridServices = propertyGridServices,
					QueryValidator = customQueryValidator
				}, callback);
		}
		public static bool EditQuery(this SqlQuery query, EditQueryContext context) {
			return EditQuery<SqlDataSourceModel>(query, context, _ => { });
		}
		public static bool EditQuery<TModel>(this SqlQuery query, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) where TModel : ISqlDataSourceModel, new() {
			Guard.ArgumentNotNull(query, "query");
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(context.DBSchemaProvider, "context.DBSchemaProvider");
			if(query.DataSource == null)
				throw new InvalidOperationException("query has no owner");
			SqlDataSource dataSource = query.DataSource;
			IDataConnectionParametersService dataConnectionParametersService =
				dataSource.GetService<IDataConnectionParametersService>();
			IWaitFormActivator waitFormActivator = CreateWaitFormActivator(context.Owner, context.LookAndFeel);
			IExceptionHandler exceptionHandler = context.Owner != null
				? (IExceptionHandler)new ConnectionExceptionHandler(context.Owner, context.LookAndFeel)
				: new EmptyExceptionHandler();
			if(!query.DataSource.ValidateConnection(context.LookAndFeel, context.Owner) ||
			   !ConnectionHelper.OpenConnection(dataSource.Connection, exceptionHandler, waitFormActivator,
				   dataConnectionParametersService))
				return false;
			string sqlText = null;
			object inputDataSchema = null;
			DBSchema schema = null;
			if(!(query is CustomSqlQuery)) {
				exceptionHandler = context.Owner != null
					? (IExceptionHandler)new LoaderExceptionHandler(context.Owner, context.LookAndFeel)
					: new EmptyExceptionHandler();
				UIDataLoader dataLoader = new UIDataLoader(waitFormActivator, exceptionHandler);
				TableQuery tableQuery = query as TableQuery;
				string[] tableNames = tableQuery != null ? tableQuery.Tables.Select(t => t.Name).ToArray() : null;
				schema = dataLoader.LoadSchema(context.DBSchemaProvider, dataSource.Connection, tableNames);
				if(schema == null)
					return false;
			}
			int index = query.Owner.IndexOf(query);
			string queryName = query.Name;
			try {
				query.Validate(context.QueryValidator, query.DataSource.ConnectionParameters, schema);
				if(query is TableQuery) {
					inputDataSchema = ((TableQuery)query).GetDataSchema(schema);
					try {
						sqlText = ((TableQuery)query).GetSql(schema);
					}
					catch(InvalidOperationException) {
					}
				}
				else if(query is CustomSqlQuery) { sqlText = ((CustomSqlQuery)query).Sql; }
			}
			catch(ValidationException e) {
				if(XtraMessageBox.Show(context.LookAndFeel,
					string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryEditorMessageInvalidQuery),
						e.Message),
					DataAccessUILocalizer.GetString(DataAccessUIStringId.QueryEditorTitle), MessageBoxButtons.OKCancel,
					MessageBoxIcon.Warning) == DialogResult.OK) {
					query = null;
				}
				else
					return false;
			}
			object dataSchema;
			DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(context.LookAndFeel, context.Owner);
			var newQuery = RunWizard(query, wizardRunnerContext, sqlText, inputDataSchema, context.ParameterService,
				dataSource.Connection, context.DBSchemaProvider, context.RepositoryItemsProvider, customizeWizard, context.PropertyGridServices, out dataSchema, context.QueryValidator, context.Options);
			if(newQuery != null) {
				newQuery.Name = queryName;
				if(index >= 0) {
					dataSource.Queries[index] = newQuery;
					dataSource.SetResultSchemaPart(queryName, dataSchema);
				}
				return true;
			}
			return false;
		}
		#endregion
		#region AddQuery(...)
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource) {
			return AddQuery(dataSource, new EditQueryContext());
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource, IRepositoryItemsProvider repositoryItemsProvider) {
			return AddQuery(dataSource, new EditQueryContext { RepositoryItemsProvider = repositoryItemsProvider });
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel) {
			return AddQuery(dataSource, new EditQueryContext { LookAndFeel = lookAndFeel });
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IRepositoryItemsProvider repositoryItemsProvider) {
			return AddQuery(dataSource, new EditQueryContext{ LookAndFeel = lookAndFeel, RepositoryItemsProvider = repositoryItemsProvider });
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner) {
			return AddQuery(dataSource, new EditQueryContext { LookAndFeel = lookAndFeel, Owner = owner });
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner, IRepositoryItemsProvider repositoryItemsProvider) {
			return AddQuery(dataSource,
				new EditQueryContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					RepositoryItemsProvider = repositoryItemsProvider
				});
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider) {
				return AddQuery(dataSource, new EditQueryContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					DBSchemaProvider = dbSchemaProvider
				});
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IRepositoryItemsProvider repositoryItemsProvider) {
			return AddQuery(dataSource,
				new EditQueryContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					DBSchemaProvider = dbSchemaProvider,
					RepositoryItemsProvider = repositoryItemsProvider
				});
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService) {
			return AddQuery(dataSource, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService
			});
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery<TModel>(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, Action<IWizardCustomization<TModel>> callback) 
			where TModel : ISqlDataSourceModel, new() {
			return AddQuery(dataSource, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService
			}, callback);
		}
		[Obsolete("This overload is obsolete. Use AddQuery<TModel>(this SqlDataSource dataSource, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool AddQuery<TModel>(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, Action<IWizardCustomization<TModel>> callback, IServiceProvider propertyGridServices) 
			where TModel : ISqlDataSourceModel, new() {
			return AddQuery(dataSource,
				new EditQueryContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					DBSchemaProvider = dbSchemaProvider,
					ParameterService = parameterService,
					PropertyGridServices = propertyGridServices
				}, callback);
		}
		[Obsolete("This overload is obsolete. Use AddQuery(this SqlDataSource dataSource, EditQueryContext context) instead.")]
		public static bool AddQuery(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider) {
			return AddQuery(dataSource,
				new EditQueryContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					DBSchemaProvider = dbSchemaProvider,
					ParameterService = parameterService,
					RepositoryItemsProvider = repositoryItemsProvider
				});
		}
		[Obsolete("This overload is obsolete. Use AddQuery<TModel>(this SqlDataSource dataSource, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool AddQuery<TModel>(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider, Action<IWizardCustomization<TModel>> callback) 
			where TModel : ISqlDataSourceModel, new() {
				return AddQuery(dataSource, new EditQueryContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					DBSchemaProvider = dbSchemaProvider,
					ParameterService = parameterService,
					RepositoryItemsProvider = repositoryItemsProvider
				}, callback);
		}
		[Obsolete("This overload is obsolete. Use AddQuery<TModel>(this SqlDataSource dataSource, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool AddQuery<TModel>(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel, IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider, Action<IWizardCustomization<TModel>> callback, IServiceProvider propertyGridServices)
			where TModel : ISqlDataSourceModel, new() {
				return AddQuery(dataSource, new EditQueryContext {
					LookAndFeel = lookAndFeel,
					Owner = owner,
					DBSchemaProvider = dbSchemaProvider,
					ParameterService = parameterService,
					RepositoryItemsProvider = repositoryItemsProvider,
					PropertyGridServices = propertyGridServices
				}, callback);
		}
		[Obsolete("This overload is obsolete. Use AddQuery<TModel>(this SqlDataSource dataSource, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) instead.")]
		public static bool AddQuery<TModel>(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner,
			IDBSchemaProvider dbSchemaProvider, IParameterService parameterService,
			IRepositoryItemsProvider repositoryItemsProvider, Action<IWizardCustomization<TModel>> callback,
			IServiceProvider propertyGridServices, ICustomQueryValidator customQueryValidator)
			where TModel : ISqlDataSourceModel, new() {
			return AddQuery(dataSource, new EditQueryContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				DBSchemaProvider = dbSchemaProvider,
				ParameterService = parameterService,
				RepositoryItemsProvider = repositoryItemsProvider,
				PropertyGridServices = propertyGridServices,
				QueryValidator = customQueryValidator
			}, callback);
		}
		public static bool AddQuery(this SqlDataSource dataSource, EditQueryContext context) {
			return AddQuery<SqlDataSourceModel>(dataSource, context, _ => { });
		}
		public static bool AddQuery<TModel>(this SqlDataSource dataSource, EditQueryContext context, Action<IWizardCustomization<TModel>> customizeWizard) where TModel : ISqlDataSourceModel, new() {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			Guard.ArgumentNotNull(context, "context");
			Guard.ArgumentNotNull(context.DBSchemaProvider, "context.DBSchemaProvider");
			IExceptionHandler exceptionHandler = context.Owner != null
				? (IExceptionHandler)new ConnectionExceptionHandler(context.Owner, context.LookAndFeel)
				: new EmptyExceptionHandler();
			IWaitFormActivator waitFormActivator = CreateWaitFormActivator(context.Owner, context.LookAndFeel);
			IDataConnectionParametersService dataConnectionParametersService = dataSource.GetService<IDataConnectionParametersService>();
			if(!dataSource.ValidateConnection(context.LookAndFeel, context.Owner) || !ConnectionHelper.OpenConnection(dataSource.Connection, exceptionHandler, waitFormActivator, dataConnectionParametersService))
				return false;
			object dataSchema;
			DefaultWizardRunnerContext wizardRunnerContext = new DefaultWizardRunnerContext(context.LookAndFeel, context.Owner);
			var query = RunWizard(null, wizardRunnerContext, null, null, context.ParameterService, dataSource.Connection,
				context.DBSchemaProvider, context.RepositoryItemsProvider, customizeWizard, context.PropertyGridServices, out dataSchema, context.QueryValidator, context.Options);
			if(query != null) {
				query.Name = dataSource.Queries.GenerateUniqueName(query);
				dataSource.Queries.Add(query);
				dataSource.SetResultSchemaPart(query.Name, dataSchema);
				return true;
			}
			return false;
		}
		#endregion
		#region RebuildResultSchema(...)
		[Obsolete("This overload is obsolete. Use RebuildResultSchema(this SqlDataSource dataSource, ReuildResultSchemaContext context) instead.")]
		public static bool RebuildResultSchema(this SqlDataSource dataSource, UserLookAndFeel lookAndFeel,
			IWin32Window owner, IParameterService parameterService, bool showSuccessMessage) {
			return RebuildResultSchema(dataSource, new RebuildResultSchemaContext {
				LookAndFeel = lookAndFeel,
				Owner = owner,
				ParameterService = parameterService,
				ShowSuccessMessage = showSuccessMessage
			});
		}
		public static bool RebuildResultSchema(this SqlDataSource dataSource, RebuildResultSchemaContext context) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			Guard.ArgumentNotNull(context, "context");
			IExceptionHandler exceptionHandler = context.Owner != null
				? (IExceptionHandler)new LoaderExceptionHandler(context.Owner, context.LookAndFeel)
				: new EmptyExceptionHandler();
			IWaitFormActivator waitFormActivator = CreateWaitFormActivator(context.Owner, context.LookAndFeel);
			IDataConnectionParametersService dataConnectionParametersService =
				dataSource.GetService<IDataConnectionParametersService>();
			Debug.Assert(dataConnectionParametersService != null, "conParamProvider required");
			if(!dataSource.ValidateConnection(context.LookAndFeel, context.Owner) ||
			   !ConnectionHelper.OpenConnection(dataSource.Connection, exceptionHandler, waitFormActivator,
				   dataConnectionParametersService))
				return false;
			IEnumerable<IParameter> parameters = context.ParameterService != null ? context.ParameterService.Parameters : null;
			var cts = new CancellationTokenSource();
			var hook = new CancellationTokenHook(cts);
			CancellationToken token = cts.Token;
			var ui = TaskScheduler.FromCurrentSynchronizationContext();
			var waitFormProvider = new WaitFormProvider(ui, waitFormActivator, hook,
				DataAccessLocalizer.GetString(DataAccessStringId.RebuildResultSchemaWaitFormText));
			Exception taskException = null;
			try {
				waitFormProvider.ShowWaitForm();
				Task<ResultSet> task = dataSource.RebuildResultSchemaAsync(queries => {
					waitFormProvider.CloseWaitForm(false);
					Task<bool> task2 =
						Task.Factory.StartNew(
							() =>
								XtraMessageBox.Show(context.LookAndFeel, context.Owner,
									string.Join("\r\n\t\u2022 ",
										new[] {
											DataAccessUILocalizer.GetString(
												DataAccessUIStringId.RebuildResultSchemaConfirmationText)
										}.Union(
											queries.Select(q => q.Name))),
									DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxConfirmationTitle),
									MessageBoxButtons.OKCancel) == DialogResult.OK, token, TaskCreationOptions.None, ui);
					task2.Wait(token);
					bool result = task2.Result;
					waitFormProvider.ShowWaitForm();
					return result;
				}, parameters, dataSource.Queries, token);
				while(!task.IsCompleted && !token.IsCancellationRequested)
					Application.DoEvents();
				task.Wait(token);
				waitFormProvider.CloseWaitForm(true);
				if(task.Result == null) {
					return false;
				}
				dataSource.ResultSet.SetTables(task.Result.Tables);
				dataSource.ResultSet.ApplyRelations(dataSource.Relations);
				if(context.ShowSuccessMessage)
					XtraMessageBox.Show(context.LookAndFeel, context.Owner,
						DataAccessUILocalizer.GetString(DataAccessUIStringId.RebuildResultSchemaComplete),
						DataAccessUILocalizer.GetString(DataAccessUIStringId.RebuildResultSchemaCaption),
						MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch(Exception ex) {
				AggregateException aex = ex as AggregateException;
				taskException = aex == null ? ex : ExceptionHelper.Unwrap(aex);
			}
			finally {
				waitFormProvider.CloseWaitForm(true);
			}
			if(taskException != null) {
				exceptionHandler.HandleException(taskException);
				return false;
			}
			return true;
		}
		#endregion
		#region private members
		static IWaitFormActivator CreateWaitFormActivator(IWin32Window owner, UserLookAndFeel lookAndFeel) {
			return owner == null
				? EmptyWaitFormActivator.Instance
				: owner is Form
					? new WaitFormActivator((Form)owner, typeof(WaitFormWithCancel), true)
					: (IWaitFormActivator)
						new WaitFormActivatorDesignTime(owner, typeof(WaitFormWithCancel),
							(lookAndFeel ?? UserLookAndFeel.Default).ActiveSkinName);
		}
		static SqlQuery RunWizard<TModel>(SqlQuery query, IWizardRunnerContext context, string sqlText, object inputDataSchema,
			IParameterService parameterService, SqlDataConnection dataConnection, IDBSchemaProvider dbSchemaProvider,
			IRepositoryItemsProvider repositoryItemsProvider, Action<IWizardCustomization<TModel>> callback, IServiceProvider propertyGridServices, out object dataSchema, ICustomQueryValidator customQueryValidator, SqlWizardOptions options) 
			where TModel : ISqlDataSourceModel, new()
		{
			dataSchema = null;
			TModel model = new TModel {
				Query = query,
				DataConnection = dataConnection,
				SqlQueryText = sqlText,
				DataSchema = inputDataSchema
			};
			var client = new SqlDataSourceWizardClientUI(null, parameterService, propertyGridServices, dbSchemaProvider) {
				RepositoryItemsProvider = repositoryItemsProvider, CustomQueryValidator = customQueryValidator, Options = options
			};
			using (var runner = new EditQueryWizardRunner<TModel>(context)) {
				if(runner.Run(client, model, callback)) {
					dataSchema = runner.WizardModel.DataSchema;
					return runner.WizardModel.Query;
				}
			}
			return null;
		}
		#endregion
	}
	public sealed class ManageRelationsContext {
		public ManageRelationsContext() {
			DBSchemaProvider = new DBSchemaProvider();
		}
		public UserLookAndFeel LookAndFeel { get; set; }
		public IWin32Window Owner { get; set; }
		public IDBSchemaProvider DBSchemaProvider { get; set; }
	}
	public sealed class ConfigureConnectionContext {
		UserLookAndFeel lookAndFeel;
		IWin32Window owner;
		IWizardRunnerContext context;
		public ConfigureConnectionContext() {
			ConnectionStorageService = new ConnectionStorageService();
		}
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeel; }
			set {
				if(context != null)
					throw new InvalidOperationException("WizardContext already exists");
				lookAndFeel = value;
			}
		}
		public IWin32Window Owner {
			get { return owner; }
			set {
				if(context != null)
					throw new InvalidOperationException("WizardContext already exists");
				owner = value;
			}
		}
		public IConnectionStorageService ConnectionStorageService { get; set; }
		public IWizardRunnerContext WizardContext { 
			get { return context ?? (context = new DefaultWizardRunnerContext(lookAndFeel, owner)); }
			set { context = value; }
		}
	}
	public sealed class EditQueryContext {
		public EditQueryContext() {
			DBSchemaProvider = new DBSchemaProvider();
			RepositoryItemsProvider = DefaultRepositoryItemsProvider.Instance;
			QueryValidator = new CustomQueryValidator();
			Options = SqlWizardOptions.None;
		}
		internal EditQueryContext(EditQueryContext other) {
			LookAndFeel = other.LookAndFeel;
			Owner = other.Owner;
			DBSchemaProvider = other.DBSchemaProvider;
			ParameterService = other.ParameterService;
			RepositoryItemsProvider = other.RepositoryItemsProvider;
			PropertyGridServices = other.PropertyGridServices;
			QueryValidator = other.QueryValidator;
			Options = other.Options;
		}
		public UserLookAndFeel LookAndFeel { get; set; }
		public IWin32Window Owner { get; set; }
		public IDBSchemaProvider DBSchemaProvider { get; set; }
		public IParameterService ParameterService { get; set; }
		public IRepositoryItemsProvider RepositoryItemsProvider { get; set; }
		public IServiceProvider PropertyGridServices { get; set; }
		public ICustomQueryValidator QueryValidator { get; set; }
		public SqlWizardOptions Options { get; set; }
	}
	public sealed class RebuildResultSchemaContext {
		public RebuildResultSchemaContext() {
			ShowSuccessMessage = true;
		}
		public UserLookAndFeel LookAndFeel { get; set; }
		public IWin32Window Owner { get; set; }
		public IParameterService ParameterService { get; set; }
		public bool ShowSuccessMessage { get; set; }
	}
}
