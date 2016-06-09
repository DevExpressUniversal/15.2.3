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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Entity;
using DevExpress.DataAccess;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.EntityFramework;
using DevExpress.DataAccess.UI.Excel;
using DevExpress.DataAccess.UI.ObjectBinding;
using DevExpress.DataAccess.UI.Sql;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Services.Internal;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Options;
using DevExpress.Snap.Options;
using DevExpress.Snap.Native;
using DevExpress.Utils.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using ICommandExecutor = DevExpress.XtraReports.Design.Commands.ICommandExecutor;
using SnapParameter = DevExpress.Snap.Core.API.Parameter;
using DataAccessDataSourceTypes = DevExpress.DataAccess.Wizard.Services.DataSourceTypes;
namespace DevExpress.Snap {
	using DevExpress.Snap.Services;
	public class MenuCommandHandler : MenuCommandHandlerBase {
		readonly SnapControl snapControl;
		public MenuCommandHandler(SnapControl snapControl)
			: base(snapControl) {
			this.snapControl = snapControl;
		}
		public void RegisterMenuCommands() {
			if (commands.Count > 0) return;
			AddCommandExecutor(new FieldListCommandExecutor(snapControl), OnStatusFieldListCommand, false,
				CommandGroups.FieldListCommands);
		}
		[SecuritySafeCritical]
		void OnStatusFieldListCommand(object sender, EventArgs e) {
			MenuCommand cmd = (MenuCommand)sender;
			cmd.Enabled = cmd.Visible = cmd.Supported = cmd.CommandID != FieldListCommands.ClearCalculatedFields && cmd.CommandID != FieldListCommands.ClearParameters && snapControl.InnerControl.IsEditable;
			cmd.Checked = true;
		}
	}
	public class FieldListCommandExecutor : ICommandExecutor {
		readonly SnapControl snapControl;
		Dictionary<CommandID, Action> fieldListCommandDispatcher;
		Dictionary<CommandID, Action<DataMemberListNodeBase>> paramFieldListCommandDispatcher;
		public FieldListCommandExecutor(SnapControl snapControl) {
			this.snapControl = snapControl;
			this.fieldListCommandDispatcher = CreateFieldListCommandDispatcher();
			this.paramFieldListCommandDispatcher = CreateParameterizedFieldListCommandDispatcher();
		}
		Dictionary<CommandID, Action> CreateFieldListCommandDispatcher() {
			Dictionary<CommandID, Action> result = new Dictionary<CommandID, Action>();
			result.Add(FieldListCommands.AddParameter, AddParameter);
			result.Add(FieldListCommands.EditParameters, EditParameters);
			return result;
		}
		Dictionary<CommandID, Action<DataMemberListNodeBase>> CreateParameterizedFieldListCommandDispatcher() {
			Dictionary<CommandID, Action<DataMemberListNodeBase>> result = new Dictionary<CommandID, Action<DataMemberListNodeBase>>();
			result.Add(DataExplorerCommands.RemoveDataSource, RemoveDataSource);
			result.Add(DataExplorerCommands.UpdateDataSource, UpdateDataSource);
			result.Add(DataExplorerCommands.MailMergeDataSource, MailMergeDataSource);
			result.Add(FieldListCommands.AddCalculatedField, AddCalculatedField);
			result.Add(FieldListCommands.EditExpressionCalculatedField, EditExpressionCalculatedField);
			result.Add(FieldListCommands.DeleteCalculatedField, DeleteCalculatedField);
			result.Add(FieldListCommands.EditCalculatedFields, EditCalculatedFields);
			result.Add(FieldListCommands.DeleteParameter, DeleteParameter);
			result.Add(DataExplorerCommands.ConfigureConnection, ConfigureConnection);
			result.Add(DataExplorerCommands.ManageRelations, ManageRelations);
			result.Add(DataExplorerCommands.ManageQueries, ManageQueries);
			result.Add(DataExplorerCommands.RebuildResultSchema, RebuildResultSchema);
			result.Add(DataExplorerCommands.EditObjectDataSource, EditObjectDataSource);
			result.Add(DataExplorerCommands.EditExcelDataSource, EditExcelDataSource);
			result.Add(DataExplorerCommands.Configure, Configure);
			return result;
		}
		#region ICommandExecutor Members
		void UpdateDataComponent(IDataComponent dataComponent, IEnumerable<IParameter> parameters) {
			try {
				dataComponent.Fill(parameters);
			}
			catch (Exception e) {
				XtraMessageBox.Show(this.snapControl.LookAndFeel, this.snapControl.FindForm(), e.Message);
			}
			this.snapControl.DocumentModel.AddDataSource(dataComponent);
		}
		public void ExecCommand(CommandID cmdID, object[] parameters) {
			if (cmdID == DataExplorerCommands.AddDataSource) {
				AddDataSource();
				return;
			}
			if (parameters == null || parameters.Length == 0)
				return;
			DataMemberListNodeBase node = (DataMemberListNodeBase)parameters[0];
			Action<DataMemberListNodeBase> nodeCommand;
			if (this.paramFieldListCommandDispatcher.TryGetValue(cmdID, out nodeCommand)) {
				nodeCommand(node);
				return;
			}
			Action command;
			if (this.fieldListCommandDispatcher.TryGetValue(cmdID, out command))
				command();
		}
		void AddDataSource() {
			DataSourceWizardClientUI client = PrepareDataSourceWizardClient();
			var runner = new DataSourceWizardRunner<DataSourceModel>(this.snapControl.LookAndFeel, this.snapControl);
			if (runner.Run(client, new DataSourceModel(), tool => CustomizeDataSourceWizard(tool))) {
				IDataSourceCreationService srv = this.snapControl.GetService<IDataSourceCreationService>() ?? new DataSourceCreationService();
				IDataComponent component = srv.CreateDataComponent(runner.WizardModel);
				UpdateDataComponent(component, client.ParameterService != null ? client.ParameterService.Parameters : null);
			}
		}
		void CustomizeDataSourceWizard(IWizardCustomization<DataSourceModel> tool) {
			if (this.snapControl.Options.DataSourceWizardOptions != null)
				ApplyDataSourceWizardOptions(tool, this.snapControl.Options.DataSourceWizardOptions);
			var srv = this.snapControl.GetService<IDataSourceWizardCustomizationService>();
			if (srv != null)
				srv.CustomizeDataSourceWizard(tool);
		}
		void ApplyDataSourceWizardOptions(IWizardCustomization<DataSourceModel> tool, DataSourceWizardOptions options) {
			ApplyDataSourceTypes(tool, options);
		}
		void ApplyDataSourceTypes(IWizardCustomization<DataSourceModel> tool, DataSourceWizardOptions options) {
			DataAccessDataSourceTypes dataSourceTypes = tool.Resolve(typeof(DataAccessDataSourceTypes)) as DataAccessDataSourceTypes;
			if ((options.DataSourceTypes & Office.Options.DataSourceTypes.EntityFramework) != Office.Options.DataSourceTypes.EntityFramework)
				dataSourceTypes.Remove(DataSourceType.Entity);
			if ((options.DataSourceTypes & Office.Options.DataSourceTypes.Excel) != Office.Options.DataSourceTypes.Excel)
				dataSourceTypes.Remove(DataSourceType.Excel);
			if ((options.DataSourceTypes & Office.Options.DataSourceTypes.Object) != Office.Options.DataSourceTypes.Object)
				dataSourceTypes.Remove(DataSourceType.Object);
			if ((options.DataSourceTypes & Office.Options.DataSourceTypes.Sql) != Office.Options.DataSourceTypes.Sql)
				dataSourceTypes.Remove(DataSourceType.Xpo);
		}
		DataSourceWizardClientUI PrepareDataSourceWizardClient() {
			IConnectionStorageService connectionStorage = CreateConnectionStorageService();
			IParameterService parameterService = CreateParameterService();
			IConnectionStringsProvider connectionStrings = CreateConnectionStringsProvider();
			ISolutionTypesProvider solutionTypesProvider = CreateSolutionTypesProvider();
			IDBSchemaProvider dataProvider = CreateDBSchemaProvider();
			IDataSourceNameCreationService nameCreator = CreateDataSourceNameCreationService();
			DataSourceWizardClientUI client = new DataSourceWizardClientUI(connectionStorage, parameterService, solutionTypesProvider, connectionStrings, dataProvider, nameCreator, OperationMode.DataOnly);
			client.DataSourceTypes = DataAccessDataSourceTypes.All;
			client.Options = CreateSqlWizardOptions();
			return client;
		}
		SqlWizardOptions CreateSqlWizardOptions() {
			return this.snapControl.Options.DataSourceWizardOptions.SqlWizardSettings.ToSqlWizardOptions();
		}
		void RemoveDataSource(DataMemberListNodeBase node) {
			if (XtraMessageBox.Show("Remove the selected data source?", "Data Explorer", MessageBoxButtons.OKCancel) == DialogResult.OK) {
				DataSourceInfo dataSourceInfo = snapControl.DocumentModel.DataSources.GetInfo(node.DataSource);
				this.snapControl.Document.BeginUpdateDataSource();
				this.snapControl.Document.DataSources.Remove(dataSourceInfo);
				this.snapControl.Document.EndUpdateDataSource();
			}
		}
		void ConfigureConnection(DataMemberListNodeBase node) {
			if (((SqlDataSource)node.DataSource).ConfigureConnection(new ConfigureConnectionContext{ LookAndFeel = this.snapControl.LookAndFeel, Owner = this.snapControl }))
				UpdateDataSource(node);
		}
		void ManageRelations(DataMemberListNodeBase node) {
			IDBSchemaProvider dbSchemaProvider = CreateDBSchemaProvider();
			if (((SqlDataSource)node.DataSource).ManageRelations(new ManageRelationsContext{ LookAndFeel = this.snapControl.LookAndFeel, Owner = this.snapControl, DBSchemaProvider = dbSchemaProvider }))
				UpdateDataSource(node);
		}
		void ManageQueries(DataMemberListNodeBase node) {
			IDBSchemaProvider dbSchemaProvider = CreateDBSchemaProvider();
			IParameterService parameterService = CreateParameterService();
			if (((SqlDataSource)node.DataSource).ManageQueries(new EditQueryContext { LookAndFeel = this.snapControl.LookAndFeel, Owner = this.snapControl, DBSchemaProvider = dbSchemaProvider, ParameterService = parameterService, Options = CreateSqlWizardOptions() }))
				UpdateDataSource(node);
		}
		void RebuildResultSchema(DataMemberListNodeBase node) {
			IParameterService parameterService = CreateParameterService();
			if (((SqlDataSource)node.DataSource).RebuildResultSchema(new RebuildResultSchemaContext{ LookAndFeel = this.snapControl.LookAndFeel, Owner = this.snapControl, ParameterService = parameterService }))
				UpdateDataSource(node);
		}
		void EditObjectDataSource(DataMemberListNodeBase node) {
			ISolutionTypesProvider solutionTypesProvider = CreateSolutionTypesProvider();
			IWizardRunnerContext wizardRunnerContext = CreateWizardRunnerContext();
			IParameterService parameterService = CreateParameterService();
			if (((ObjectDataSource)node.DataSource).EditDataSource(solutionTypesProvider, wizardRunnerContext, parameterService))
				UpdateDataSource(node);
		}
		void EditExcelDataSource(DataMemberListNodeBase node) {
			IWizardRunnerContext wizardRunnerContext = CreateWizardRunnerContext();
			IExcelSchemaProvider excelSchemaProvider = CreateExcelSchemaProvider();
			if (((ExcelDataSource)node.DataSource).EditDataSource(wizardRunnerContext, excelSchemaProvider))
				UpdateDataSource(node);
		}
		void Configure(DataMemberListNodeBase node) {
			IWizardRunnerContext wizardRunnerContext = CreateWizardRunnerContext();
			ISolutionTypesProvider solutionTypesProvider = CreateSolutionTypesProvider();
			IConnectionStringsProvider connectionStrings = CreateConnectionStringsProvider();
			IConnectionStorageService connectionStorage = CreateConnectionStorageService();
			IParameterService parameterService = CreateParameterService();
			if (((EFDataSource)node.DataSource).Configure(wizardRunnerContext, solutionTypesProvider, connectionStrings, connectionStorage, parameterService))
				UpdateDataSource(node);
		}
		void UpdateDataSource(DataMemberListNodeBase node) {
			snapControl.Document.BeginUpdateDataSource();
			UpdateDataComponent((IDataComponent)node.DataSource, snapControl.Document.Parameters);
			snapControl.Document.EndUpdateDataSource();
		}
		void MailMergeDataSource(DataMemberListNodeBase node) {
			SnapMailMergeVisualOptions options = snapControl.Options.SnapMailMergeVisualOptions;
			bool newMailMergeDataSourceSelected = !CompareNodeAndOptions(node, options);
			options.ResetMailMerge();
			snapControl.DocumentModel.ResetRootDataContext();
			if (newMailMergeDataSourceSelected) {
				options.DataSource = node.DataSource;
				options.DataMember = node.DataMember;
			}
			this.snapControl.UpdateCommandUI();
		}
		void AddCalculatedField(DataMemberListNodeBase node) {
			DataSourceInfo info = snapControl.DocumentModel.DataSourceDispatcher.GetInfo(node.DataSource);
			DataSourceInfo sourceInfo = snapControl.DocumentModel.GetNotNullDocumentModelDataSourceInfo(info.DataSourceName);
			string name = NameHelper.GetCalculatedFieldName(info.DataSource, sourceInfo.CalculatedFields, node.DataMember);
			CalculatedField calculatedField = new CalculatedField(name, node.DataMember);
			calculatedField.FieldType = FieldType.None;
			calculatedField.DataSourceDispatcher = snapControl.DocumentModel.DataSourceDispatcher;
			calculatedField.DataSourceName = sourceInfo.DataSourceName;
			sourceInfo.CalculatedFields.Add(calculatedField);
		}
		void EditExpressionCalculatedField(DataMemberListNodeBase node) {
			ServiceManager serviceManager = new ServiceManager();
			serviceManager.AddService(typeof(ParametersOwner), new ParametersOwner(snapControl.Document.Parameters));
			serviceManager.AddService(typeof(IWin32Window), this.snapControl);
			EditorHelper.EditValue(node.Object, "Expression", serviceManager);
		}
		void DeleteCalculatedField(DataMemberListNodeBase node) {
			CalculatedField calculatedField = (CalculatedField)node.Object;
			DataSourceInfo info = snapControl.DocumentModel.DataSourceDispatcher.GetInfo(node.DataSource);
			DataSourceInfo sourceInfo = snapControl.DocumentModel.DataSources[info.DataSourceName];
			if (sourceInfo != null && sourceInfo.CalculatedFields.Remove(calculatedField))
				return;
			snapControl.DataSources[info.DataSourceName].CalculatedFields.Remove(calculatedField);
		}
		void EditCalculatedFields(DataMemberListNodeBase node) {
			DataSourceInfo info = snapControl.DocumentModel.GetNotNullDocumentModelDataSourceInfo(node.DataSource);
			snapControl.Document.BeginUpdateDataSource();
			ServiceManager serviceManager = new ServiceManager();
			serviceManager.AddService(typeof(TypeDescriptionProvider), new SNTypeDescriptionProvider(info.DataSourceName, this.snapControl.DocumentModel.DataSourceDispatcher, node.DataMember));
			serviceManager.AddService(typeof(ParametersOwner), new ParametersOwner(snapControl.Document.Parameters));
			serviceManager.AddService(typeof(ILookAndFeelService), new LookAndFeelService());
			serviceManager.AddService(typeof(IWin32Window), this.snapControl);
			EditorHelper.EditValue(info, "CalculatedFields", serviceManager);
			snapControl.Document.EndUpdateDataSource();
		}
		void AddParameter() {
			snapControl.Document.Parameters.Add(new SnapParameter() { Name = NameHelper.GetParameterName(snapControl.Document.Parameters) });
		}
		void DeleteParameter(DataMemberListNodeBase node) {
			snapControl.Document.Parameters.Remove((SnapParameter)node.Object);
		}
		void EditParameters() {
			snapControl.Document.BeginUpdateDataSource();
			ServiceManager serviceManager = new ServiceManager();
			serviceManager.AddService(typeof(TypeDescriptionProvider), new SNTypeDescriptionProvider());
			serviceManager.AddService(typeof(ILookAndFeelService), new LookAndFeelService());
			serviceManager.AddService(typeof(IWin32Window), this.snapControl);
			EditorHelper.EditValue(snapControl.Document, "Parameters", serviceManager);
			snapControl.Document.EndUpdateDataSource();
		}
		IConnectionStorageService CreateConnectionStorageService() {
			return new ConnectionStorageService();
		}
		IParameterService CreateParameterService() {
			return this.snapControl.Document.Parameters.GetParameterService();
		}
		IConnectionStringsProvider CreateConnectionStringsProvider() {
			return new RuntimeConnectionStringsProvider();
		}
		ISolutionTypesProvider CreateSolutionTypesProvider() {
			return EntityServiceHelper.GetRuntimeSolutionProvider(Assembly.GetEntryAssembly());
		}
		IWizardRunnerContext CreateWizardRunnerContext() {
			return new DefaultWizardRunnerContext(this.snapControl.LookAndFeel, this.snapControl.FindForm());
		}
		IExcelSchemaProvider CreateExcelSchemaProvider() {
			return new ExcelSchemaProvider();
		}
		IDBSchemaProvider CreateDBSchemaProvider() {
			return this.snapControl.DocumentModel.GetService<IDBSchemaProvider>() ?? new DBSchemaProvider();
		}
		IDataSourceNameCreationService CreateDataSourceNameCreationService() {
			return this.snapControl.DocumentModel.GetService<IDataSourceNameCreationService>();
		}
		#endregion
		public static bool CompareNodeAndOptions(DataMemberListNodeBase node, SnapMailMergeVisualOptions options) {
			return ReferenceEquals(node.DataSource, options.DataSource) && (string.IsNullOrEmpty(node.DataMember) ? string.IsNullOrEmpty(options.DataMember) : (String.Compare(node.DataMember, options.DataMember) == 0));
		}
		#region IDisposable Members
		public void Dispose() {
		}
		#endregion
	}
	public static class CommandGroups {
		static readonly CommandID[]
			fieldListCommands = {
							XtraReports.Design.Commands.FieldListCommands.AddCalculatedField,
							XtraReports.Design.Commands.FieldListCommands.EditCalculatedFields,
							XtraReports.Design.Commands.FieldListCommands.EditExpressionCalculatedField,
							XtraReports.Design.Commands.FieldListCommands.DeleteCalculatedField,
							XtraReports.Design.Commands.FieldListCommands.AddParameter,
							XtraReports.Design.Commands.FieldListCommands.EditParameters,
							XtraReports.Design.Commands.FieldListCommands.DeleteParameter,
							XtraReports.Design.Commands.FieldListCommands.ClearCalculatedFields,
							XtraReports.Design.Commands.FieldListCommands.ClearParameters,
							DataExplorerCommands.AddDataSource,
							DataExplorerCommands.RemoveDataSource,
							DataExplorerCommands.MailMergeDataSource,
							DataExplorerCommands.UpdateDataSource,
							DataExplorerCommands.ConfigureConnection,
							DataExplorerCommands.ManageRelations,
							DataExplorerCommands.ManageQueries,
							DataExplorerCommands.RebuildResultSchema, 
							DataExplorerCommands.EditObjectDataSource,
							DataExplorerCommands.EditExcelDataSource,
							DataExplorerCommands.Configure
			};
		public static CommandID[] FieldListCommands { get { return fieldListCommands; } }
	}
	public static class DataExplorerCommands {
		const int cmdidAddDataSource = 1;
		const int cmdidRemoveDataSource = 2;
		const int cmdidUpdateDataSource = 3;
		const int cmdidConfigureConnection = 4;
		const int cmdidManageRelations = 5;
		const int cmdidManageQueries = 6;
		const int cmdidRebuildResultSchema = 7;
		const int cmdidEditObjectDataSource = 8;
		const int cmdidConfigure = 9;
		const int cmdidMailMergeDataSource = 10;
		const int cmdidEditExcelDataSource = 11;
		static readonly Guid fieldListCommandSet = new Guid("{3500DEB0-6984-4B49-A19C-90FCBAB9A6A1}");
		[SecuritySafeCritical]
		[SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		static DataExplorerCommands() { }
		public static readonly CommandID AddDataSource = new CommandID(fieldListCommandSet, cmdidAddDataSource);
		public static readonly CommandID RemoveDataSource = new CommandID(fieldListCommandSet, cmdidRemoveDataSource);
		public static readonly CommandID UpdateDataSource = new CommandID(fieldListCommandSet, cmdidUpdateDataSource);
		public static readonly CommandID ConfigureConnection = new CommandID(fieldListCommandSet, cmdidConfigureConnection);
		public static readonly CommandID ManageRelations = new CommandID(fieldListCommandSet, cmdidManageRelations);
		public static readonly CommandID ManageQueries = new CommandID(fieldListCommandSet, cmdidManageQueries);
		public static readonly CommandID RebuildResultSchema = new CommandID(fieldListCommandSet, cmdidRebuildResultSchema);
		public static readonly CommandID EditObjectDataSource = new CommandID(fieldListCommandSet, cmdidEditObjectDataSource);
		public static readonly CommandID Configure = new CommandID(fieldListCommandSet, cmdidConfigure);
		public static readonly CommandID MailMergeDataSource = new CommandID(fieldListCommandSet, cmdidMailMergeDataSource);
		public static readonly CommandID EditExcelDataSource = new CommandID(fieldListCommandSet, cmdidEditExcelDataSource);
	}
}
namespace DevExpress.Snap.Services {
	public interface IDataSourceWizardCustomizationService {
		void CustomizeDataSourceWizard(IWizardCustomization<DataSourceModel> tool);
	}
	public interface IDataSourceCreationService {
		IDataComponent CreateDataComponent(IDataSourceModel model);
	}
	public class DataSourceCreationService : IDataSourceCreationService {
		public virtual IDataComponent CreateDataComponent(IDataSourceModel model) {
			return new DataComponentCreator().CreateDataComponent(model);
		}
	}
}
