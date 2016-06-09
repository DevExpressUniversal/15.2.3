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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Entity;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Native.Sql;
using DevExpress.DataAccess.UI.Sql;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Design {
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class XRSqlDataSourceDesigner : ComponentDesigner {
		static readonly PropertyDescriptor sqlQueryCollectionPropertyDescriptor = TypeDescriptor.GetProperties(typeof(SqlDataSource))["Queries"];
		static readonly PropertyDescriptor resultSchemaPropertyDescriptor = TypeDescriptor.GetProperties(typeof(SqlDataSource))["ResultSchemaSerializable"];
		static readonly PropertyDescriptor connectionNamePropertyDescriptor = TypeDescriptor.GetProperties(typeof(SqlDataSource))["ConnectionName"];
		static readonly PropertyDescriptor connectionParametersPropertyDescriptor = TypeDescriptor.GetProperties(typeof(SqlDataSource))["ConnectionParameters"];
		public static PropertyDescriptor SqlQueryCollectionPropertyDescriptor { get { return sqlQueryCollectionPropertyDescriptor; } }
		public static PropertyDescriptor ResultSchemaPropertyDescriptor { get { return resultSchemaPropertyDescriptor; } }
		public static PropertyDescriptor ConnectionNamePropertyDescriptor { get { return connectionNamePropertyDescriptor; } }
		public static PropertyDescriptor ConnectionParametersPropertyDescriptor { get { return connectionParametersPropertyDescriptor; } }
		IWin32Window win32Window;
		readonly DesignerVerbCollection verbs = new DesignerVerbCollection();
		DesignerVerb configureConnectionVerb;
		DesignerVerb manageQueriesVerb;
		DesignerVerb rebuildResultSchemaVerb;
		DesignerVerb requestDatabaseSchema;
		DesignerVerb manageRelationsVerb;
		protected IDesignerHost designerHost;
		protected IComponentChangeService componentChangeService;
		protected internal virtual UserLookAndFeel GetLookAndFeel(IServiceProvider serviceProvider) {
			var lookAndFeelService = serviceProvider.GetService<ILookAndFeelService>();
			var lookAndFeel = new UserLookAndFeel(new object());
			lookAndFeel.UseDefaultLookAndFeel = false;
			lookAndFeel.SkinName = lookAndFeelService.LookAndFeel.ActiveSkinName;
			return lookAndFeel;
		}
		protected SqlDataSource DataSource {
			[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
			get {
				return (SqlDataSource)Component;
			}
		}
		public override DesignerVerbCollection Verbs { get { return verbs; } }
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public XRSqlDataSourceDesigner() {
		}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			this.componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			this.designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
			ReplaceServices();
			if(this.designerHost.GetService<IDBSchemaProvider>() == null) {
				this.designerHost.AddService(typeof(IDBSchemaProvider), new DBSchemaProvider());
			}
			ICustomQueryValidator customQueryValidator = GetCustomQueryValidator();
			if(this.designerHost.GetService<ICustomQueryValidator>() == null)
				this.designerHost.AddService(typeof(ICustomQueryValidator), customQueryValidator);
			IConnectionStringsProvider connectionStringsProvider = GetConnectionStringsProvider();
			if(this.designerHost.GetService<IConnectionStringsProvider>() == null)
				this.designerHost.AddService(typeof(IConnectionStringsProvider), connectionStringsProvider);
			IConnectionProviderService connectionProviderService = GetConnectionProviderService();
			IConnectionStorageService connectionStorageService = GetConnectionStorageService();
			if(this.designerHost.GetService<IConnectionProviderService>() == null)
				this.designerHost.AddService(typeof(IConnectionProviderService), connectionProviderService);
			if(this.designerHost.GetService<IConnectionStorageService>() == null)
				this.designerHost.AddService(typeof(IConnectionStorageService), connectionStorageService);
			((IServiceContainer)DataSource).RemoveService(typeof(IConnectionProviderService));
			((IServiceContainer)DataSource).AddService(typeof(IConnectionProviderService), connectionProviderService);
			IServiceProvider serviceProvider = designerHost.RootComponent as IServiceProvider;
			if(serviceProvider != null) {
				IServiceContainer serviceContainer = serviceProvider.GetService<IServiceContainer>();
				if(serviceContainer != null)
					if(serviceContainer.GetService<IConnectionProviderService>() == null)
						serviceContainer.AddService(typeof(IConnectionProviderService), connectionProviderService);
			}
			configureConnectionVerb = new DesignerVerb(DataAccessUILocalizer.GetString(DataAccessUIStringId.SqlDataSourceDesignerVerbEditConnection), ConfigureConnectionVerb);
			manageQueriesVerb = new DesignerVerb(DataAccessUILocalizer.GetString(DataAccessUIStringId.SqlDataSourceDesignerVerbManageQueries), ManageQueriesVerb);
			manageRelationsVerb = new DesignerVerb(DataAccessUILocalizer.GetString(DataAccessUIStringId.SqlDataSourceDesignerVerbManageRelations), ManageRelationsVerb);
			rebuildResultSchemaVerb = new DesignerVerb(DataAccessUILocalizer.GetString(DataAccessUIStringId.SqlDataSourceDesignerVerbRebuildSchema), RebuildResultSchemaVerb);
			requestDatabaseSchema = new DesignerVerb(DataAccessUILocalizer.GetString(DataAccessUIStringId.SqlDataSourceDesignerVerbRequestDatabaseSchema), RequestDatabaseSchemaVerb);
			verbs.Add(configureConnectionVerb);
			verbs.Add(manageQueriesVerb);
			verbs.Add(manageRelationsVerb);
			verbs.Add(rebuildResultSchemaVerb);
			verbs.Add(requestDatabaseSchema);
			UpdateVerbsState();
			designerHost.LoadComplete += designerHost_LoadComplete;
			this.componentChangeService.ComponentChanged += componentChangeService_ComponentChanged;
		}
		void componentChangeService_ComponentChanged(object sender, ComponentChangedEventArgs e) {
			if(ReferenceEquals(e.Component, Component))
				UpdateVerbsState();
		}
		protected void UpdateVerbsState() {
			bool connSet = DataSource.ConnectionName != null || DataSource.ConnectionParameters != null;
			manageQueriesVerb.Enabled = connSet;
			manageRelationsVerb.Enabled = connSet && DataSource.Queries.Count > 1;
			rebuildResultSchemaVerb.Enabled = connSet;
			requestDatabaseSchema.Enabled = connSet;
		}
		#region Verbs
		void ConfigureConnectionVerb(object o, EventArgs e) {
			var connectionStorageService = designerHost.GetService<IConnectionStorageService>();
			IUIService uiService = designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			using(UserLookAndFeel lookAndFeel = GetLookAndFeel(designerHost))
			using(DesignerTransaction transaction = designerHost.CreateTransaction("Configure Connection")) {
				componentChangeService.OnComponentChanging(DataSource, ConnectionNamePropertyDescriptor);
				componentChangeService.OnComponentChanging(DataSource, ConnectionParametersPropertyDescriptor);
				if(!DataSource.ConfigureConnection(new ConfigureConnectionContext{ LookAndFeel = lookAndFeel, Owner = owner, ConnectionStorageService = connectionStorageService })) {
					transaction.Cancel();
					return;
				}
				componentChangeService.OnComponentChanged(DataSource, ConnectionNamePropertyDescriptor, null, null);
				componentChangeService.OnComponentChanged(DataSource, ConnectionParametersPropertyDescriptor, null, null);
				transaction.Commit();
			}
		}
		void ManageQueriesVerb(object o, EventArgs e) {
			IUIService uiService = designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IParameterService parameterService = designerHost.GetService<IParameterService>();
			IDBSchemaProvider dbSchemaProvider = designerHost.GetService<IDBSchemaProvider>();
			ICustomQueryValidator customQueryValidator = designerHost.GetService<ICustomQueryValidator>();
			using(UserLookAndFeel lookAndFeel = GetLookAndFeel(designerHost))
			using(DesignerTransaction transaction = designerHost.CreateTransaction("Manage Queries")) {
				componentChangeService.OnComponentChanging(DataSource, SqlQueryCollectionPropertyDescriptor);
				componentChangeService.OnComponentChanging(DataSource, ResultSchemaPropertyDescriptor);
				var sqlWizardOptionsProvider = designerHost.GetService<ISqlWizardOptionsProvider>();
				SqlWizardOptions options = sqlWizardOptionsProvider != null ? sqlWizardOptionsProvider.SqlWizardOptions : SqlWizardOptions.EnableCustomSql;
				if (!DataSource.ManageQueries(new EditQueryContext { LookAndFeel = lookAndFeel, Owner = owner, DBSchemaProvider = dbSchemaProvider, ParameterService = parameterService, Options = options, QueryValidator = customQueryValidator }))
					transaction.Cancel();
				componentChangeService.OnComponentChanged(DataSource, SqlQueryCollectionPropertyDescriptor, null, null);
				componentChangeService.OnComponentChanged(DataSource, ResultSchemaPropertyDescriptor, null, null);
				transaction.Commit();
			}
		}
		void ManageRelationsVerb(object o, EventArgs e) {
			IUIService uiService = designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IDBSchemaProvider dbSchemaProvider = designerHost.GetService<IDBSchemaProvider>();
			using(UserLookAndFeel lookAndFeel = GetLookAndFeel(designerHost))
			using(DesignerTransaction transaction = designerHost.CreateTransaction("Manage Relations")) {
				componentChangeService.OnComponentChanging(DataSource, MasterDetailEditor.RelationsDescriptor);
				if(!DataSource.ManageRelations(new ManageRelationsContext{ LookAndFeel = lookAndFeel, Owner = owner, DBSchemaProvider = dbSchemaProvider })) {
					transaction.Cancel();
				}
				componentChangeService.OnComponentChanged(DataSource, MasterDetailEditor.RelationsDescriptor, null, null);
				transaction.Commit();
			}
		}
		void RebuildResultSchemaVerb(object o, EventArgs e) {
			IUIService uiService = designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			IParameterService parameterService = designerHost.GetService<IParameterService>();
			using(UserLookAndFeel lookAndFeel = GetLookAndFeel(designerHost))
				if(DataSource.RebuildResultSchema(new RebuildResultSchemaContext{ LookAndFeel = lookAndFeel, Owner = owner, ParameterService = parameterService }))
					componentChangeService.OnComponentChanged(DataSource, null, null, null);
		}
		void RequestDatabaseSchemaVerb(object o, EventArgs e) {
			IUIService uiService = (IUIService) GetService(typeof(IUIService));
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			using(UserLookAndFeel lookAndFeel = GetLookAndFeel(designerHost)) {
				var waitFormActivator = new WaitFormActivatorDesignTime(owner, typeof(WaitFormWithCancel), lookAndFeel.ActiveSkinName);
				var exceptionHandler = new LoaderExceptionHandler(owner, lookAndFeel);
				var dataConnectionParametersService = (IDataConnectionParametersService) DataSource.GetService(typeof(IDataConnectionParametersService));
				Debug.Assert(dataConnectionParametersService != null, "IDataConnectionParametersService required");
				if(!DataSource.ValidateConnection(lookAndFeel, owner) || !ConnectionHelper.OpenConnection(DataSource.Connection, exceptionHandler, waitFormActivator, dataConnectionParametersService))
					return;
				DataSource.Connection.DropDBSchemaCache();
				var cts = new CancellationTokenSource();
				var hook = new CancellationTokenHook(cts);
				CancellationToken token = cts.Token;
				var ui = TaskScheduler.FromCurrentSynchronizationContext();
				var waitFormProvider = new WaitFormProvider(ui, waitFormActivator, hook, DataAccessLocalizer.GetString(DataAccessStringId.LoadingDataPanelText));
				try {
					waitFormProvider.ShowWaitForm();
					Task task = DataSource.LoadDBSchemaAsync(token);
					while(!task.IsCompleted && !token.IsCancellationRequested)
						Application.DoEvents();
					task.Wait(token);
					waitFormProvider.CloseWaitForm(true);
					if(task.Status == TaskStatus.RanToCompletion)
						XtraMessageBox.Show(lookAndFeel, win32Window,
							DataAccessUILocalizer.GetString(DataAccessUIStringId.UpdateDBSchemaComplete),
							DataAccessUILocalizer.GetString(DataAccessUIStringId.UpdateDBSchemaCaption),
							MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
				catch(Exception ex) {
					exceptionHandler.HandleException(ex);
				}
				finally {
					waitFormProvider.CloseWaitForm(true);
				}
			}
		}
		#endregion
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.designerHost.LoadComplete -= designerHost_LoadComplete;
				this.componentChangeService.ComponentChanged -= componentChangeService_ComponentChanged;
			}
			base.Dispose(disposing);
		}
		protected virtual IConnectionProviderService GetConnectionProviderService() {
			return new ConnectionProviderService(this.designerHost);
		}
		protected virtual IConnectionStorageService GetConnectionStorageService() {
			return new ConnectionStorageService();
		}
		protected virtual ICustomQueryValidator GetCustomQueryValidator() {
			return new CustomQueryValidator();
		}
		protected virtual IConnectionStringsProvider GetConnectionStringsProvider() {
			return new RuntimeConnectionStringsProvider();
		}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		void designerHost_LoadComplete(object sender, EventArgs e) {
			ReplaceServices();
			UpdateVerbsState();
		}
		void ReplaceServices() {
			IUIService uiService = (IUIService)GetService(typeof(IUIService));
			if(uiService != null)
				win32Window = uiService.GetDialogOwnerWindow();
			if(win32Window != null) {
				var dataConnectionParametersProviderNative = DataSource.GetService<IDataConnectionParametersService>();
				var dataConnectionParametersProviderUi = dataConnectionParametersProviderNative as DataConnectionParametersServiceUI ?? new DataConnectionParametersServiceUI(TaskScheduler.FromCurrentSynchronizationContext(), dataConnectionParametersProviderNative, GetLookAndFeel(designerHost), win32Window);
				((IServiceContainer)DataSource).RemoveService(typeof(IDataConnectionParametersService));
				((IServiceContainer)DataSource).AddService(typeof(IDataConnectionParametersService), dataConnectionParametersProviderUi);
			}
		}
	}
	public static class SlqDataSourceConnectionValidator {
		public static SqlDataConnection ValidateConnection(this SqlDataSource sqlDataSource) {
			return ValidateConnection(sqlDataSource, null, null, false);
		}
		public static bool ValidateConnection(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel, IWin32Window owner) {
			return ValidateConnection(sqlDataSource, lookAndFeel, owner, true) != null;
		}
		public static SqlDataConnection ValidateConnection(this SqlDataSource sqlDataSource, UserLookAndFeel lookAndFeel, IWin32Window owner, bool showMessage) {
			try {
				SqlDataConnection connection = sqlDataSource.Connection;
				if(showMessage && connection == null)
					OnError(DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageMissingConnection), lookAndFeel, owner);
				return connection;
			}
			catch(InvalidOperationException e) {
				if(showMessage)
					OnError(e, lookAndFeel, owner);
				return null;
			}
			catch(ArgumentException e) {
				if(showMessage)
					OnError(e, lookAndFeel, owner);
				return null;
			}
		}
		static void OnError(Exception e, UserLookAndFeel lookAndFeel, IWin32Window owner) {
			OnError(e.Message, lookAndFeel, owner);
		}
		static void OnError(string message, UserLookAndFeel lookAndFeel, IWin32Window owner) {
			XtraMessageBox.Show(lookAndFeel, owner, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
