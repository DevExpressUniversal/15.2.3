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
using System.Windows.Forms;
using DevExpress.DataAccess.Native.ObjectBinding;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.UI.Localization;
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
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.ObjectBinding {
	public static class ObjectDataSourceUIHelper {
		#region inner clases
		class ChooseObjectMemberPageEx : ChooseObjectMemberPage<ObjectDataSourceModel> {
			public ChooseObjectMemberPageEx(IChooseObjectMemberPageView view, IWizardRunnerContext context, OperationMode operationMode) : base(view, context, operationMode) { }
			#region Overrides of ChooseObjectMemberPage<ObjectDataSourceModel>
			public override bool MoveNextEnabled {
				get {
					if(View.Result == null)
						return false;
					return View.Result.HasParams;
				}
			}
			public override bool FinishEnabled { get { return !MoveNextEnabled; } }
			#endregion
		}
		class ObjectMemberParametersPageEx : ObjectMemberParametersPage<ObjectDataSourceModel> {
			public ObjectMemberParametersPageEx(IObjectMemberParametersPageView view, IParameterService parameterService, OperationMode operationMode) : base(view, operationMode) { }
			#region Overrides of ObjectMemberParametersPage<ObjectDataSourceModel>
			public override bool MoveNextEnabled { get { return false; } }
			public override bool FinishEnabled { get { return true; } }
			#endregion
		}
		class DataSourceEditorRunner : ObjectDataSourceWizardRunner<ObjectDataSourceModel> {
			public DataSourceEditorRunner(IWizardRunnerContext context) : base(context) { }
			#region Overrides of ObjectDataSourceWizardRunner<ObjectDataSourceModel>
			protected override string WizardTitle { get { return DataSourceEditorCaption; } }
			#endregion
		}
		class DataMemberEditorPageFactory : ObjectWizardPageFactory<ObjectDataSourceModel, IObjectDataSourceWizardClientUI> {
			public DataMemberEditorPageFactory(IObjectDataSourceWizardClientUI client) : base(client) { }
			#region Overrides of ObjectWizardPageFactory<ObjectDataSourceModel,IObjectDataSourceWizardClient>
			protected override void RegisterDependencies(IObjectDataSourceWizardClientUI client) {
				Container.RegisterInstance(client.ParameterService);
				Container.RegisterInstance(client.OperationMode);
				Container.RegisterInstance(client.RepositoryItemsProvider);
				Container.RegisterType<IChooseObjectMemberPageView, ChooseObjectMemberPageView>();
				Container.RegisterType<IObjectMemberParametersPageView, ObjectMemberParametersPageView>();
				Container.RegisterType<ChooseObjectMemberPage<ObjectDataSourceModel>, ChooseObjectMemberPageEx>();
				Container.RegisterType<ObjectMemberParametersPage<ObjectDataSourceModel>, ObjectMemberParametersPageEx>();
			}
			#endregion
		}
		class DataMemberEditorRunner : ObjectDataSourceWizardRunner<ObjectDataSourceModel> {
			public DataMemberEditorRunner(IWizardRunnerContext context) : base(context) { }
			#region Overrides of ObjectDataSourceWizardRunner<ObjectDataSourceModel>
			protected override Type StartPage { get { return typeof(ChooseObjectMemberPage<ObjectDataSourceModel>); } }
			protected override string WizardTitle { get { return DataMemberEditorCaption; } }
			protected override WizardPageFactoryBase<ObjectDataSourceModel, IObjectDataSourceWizardClientUI>
				CreatePageFactory(IObjectDataSourceWizardClientUI client) {
				return new DataMemberEditorPageFactory(client);
			}
			#endregion
		}
		class ParametersEditorPageFactory : ObjectWizardPageFactory<ObjectDataSourceModel, IObjectDataSourceWizardClientUI> {
			public ParametersEditorPageFactory(IObjectDataSourceWizardClientUI client) : base(client) { }
			#region Overrides of ObjectWizardPageFactory<ObjectDataSourceModel,IObjectDataSourceWizardClient>
			protected override void RegisterDependencies(IObjectDataSourceWizardClientUI client) {
				Container.RegisterInstance(client.ParameterService);
				Container.RegisterInstance(client.OperationMode);
				Container.RegisterInstance(client.RepositoryItemsProvider);
				Container.RegisterType<IObjectMemberParametersPageView, ObjectMemberParametersPageView>();
				Container.RegisterType<ObjectMemberParametersPage<ObjectDataSourceModel>, ObjectMemberParametersPageEx>();
			}
			#endregion
		}
		class ParametersEditorRunner : ObjectDataSourceWizardRunner<ObjectDataSourceModel> {
			public ParametersEditorRunner(IWizardRunnerContext context) : base(context) { }
			#region Overrides of ObjectDataSourceWizardRunner<ObjectDataSourceModel>
			protected override Type StartPage { get { return typeof(ObjectMemberParametersPage<ObjectDataSourceModel>); } }
			protected override string WizardTitle { get { return ParametersEditorCaption; } }
			protected override WizardPageFactoryBase<ObjectDataSourceModel, IObjectDataSourceWizardClientUI> CreatePageFactory(IObjectDataSourceWizardClientUI client) {
				return new ParametersEditorPageFactory(client);
			}
			#endregion
		}
		class CtorParametersEditorPageFactory : ObjectWizardPageFactory<ObjectDataSourceModel, IObjectDataSourceWizardClientUI> {
			public CtorParametersEditorPageFactory(IObjectDataSourceWizardClientUI client) : base(client) { }
			#region Overrides of ObjectWizardPageFactory<ObjectDataSourceModel,IObjectDataSourceWizardClient>
			protected override void RegisterDependencies(IObjectDataSourceWizardClientUI client) {
				Container.RegisterInstance(client.ParameterService);
				Container.RegisterInstance(client.OperationMode);
				Container.RegisterInstance(client.RepositoryItemsProvider);
				Container.RegisterType<IChooseObjectBindingModePageView, ChooseObjectBindingModePageView>();
				Container.RegisterType<IChooseObjectConstructorPageView, ChooseObjectConstructorPageView>();
				Container.RegisterType<IObjectConstructorParametersPageView, ObjectConstructorParametersPageView>();
				Container.RegisterType<ChooseObjectBindingModePage<ObjectDataSourceModel>>();
				Container.RegisterType<ChooseObjectConstructorPage<ObjectDataSourceModel>>();
				Container.RegisterType<ObjectConstructorParametersPage<ObjectDataSourceModel>>();
			}
			#endregion
		}
		class CtorParametersEditorRunner : ObjectDataSourceWizardRunner<ObjectDataSourceModel> {
			public CtorParametersEditorRunner(IWizardRunnerContext context) : base(context) { } 
			#region Overrides of ObjectDataSourceWizardRunner<ObjectDataSourceModel>
			protected override Type StartPage {
				get { return typeof(ChooseObjectBindingModePage<ObjectDataSourceModel>); }
			}
			protected override string WizardTitle { get { return CtorParametersEditorCaption; } }
			protected override WizardPageFactoryBase<ObjectDataSourceModel, IObjectDataSourceWizardClientUI>
				CreatePageFactory(IObjectDataSourceWizardClientUI client) {
				return new CtorParametersEditorPageFactory(client);
			}
			#endregion
		}
		#endregion
		#region EditDataSource(...)
		public static bool EditDataSource(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider, IWizardRunnerContext context, IParameterService parameterService) {
			return EditDataSource(ods, solutionTypesProvider, context, parameterService, DefaultRepositoryItemsProvider.Instance);
		}
		public static bool EditDataSource(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider, IWizardRunnerContext context, IParameterService parameterService, OperationMode operationMode) {
			return EditDataSource(ods, solutionTypesProvider, context, parameterService, operationMode, DefaultRepositoryItemsProvider.Instance);
		}
		public static bool EditDataSource(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider, IWizardRunnerContext context, IParameterService parameterService, IRepositoryItemsProvider repositoryItemsProvider) {
			return EditDataSource(ods, solutionTypesProvider, context, parameterService, OperationMode.Both, repositoryItemsProvider);
		}
		public static bool EditDataSource(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider, IWizardRunnerContext context, IParameterService parameterService, OperationMode operationMode, IRepositoryItemsProvider repositoryItemsProvider) {
			Guard.ArgumentNotNull(ods, "ods");
			Guard.ArgumentNotNull(solutionTypesProvider, "solutionTypesProvider");
			string caption = DataSourceEditorCaption;
			var model = CreateModel(ods, solutionTypesProvider, context, caption);
			if(model == null)
				return false;
			var runner = new DataSourceEditorRunner(context);
			var client = new ObjectDataSourceWizardClientUI(parameterService, solutionTypesProvider, operationMode) { RepositoryItemsProvider = repositoryItemsProvider };
			if(!runner.Run(client, model))
				return false;
			ObjectDataSource result =  new DataComponentCreator().CreateDataComponent(runner.WizardModel);
			ods.BeginUpdate();
			ods.DataSource = result.DataSource;
			ods.DataMember = result.DataMember;
			ods.Parameters.Clear();
			ods.Parameters.AddRange(result.Parameters);
			ods.Constructor = result.Constructor;
			ods.EndUpdate();
			return true;
		}
		#endregion
		#region EditDataMember(...)
		public static bool EditDataMember(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider, IWizardRunnerContext context, IParameterService parameterService) {
			return EditDataMember(ods, solutionTypesProvider, context, parameterService, DefaultRepositoryItemsProvider.Instance);
		}
		public static bool EditDataMember(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider,
			IWizardRunnerContext context, IParameterService parameterService,
			IRepositoryItemsProvider repositoryItemsProvider) {
			Guard.ArgumentNotNull(ods, "ods");
			Guard.ArgumentNotNull(solutionTypesProvider, "solutionTypesProvider");
			string caption = DataMemberEditorCaption;
			if(ods.DataSource == null) {
				context.ShowMessage(DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsNoDataSetMessage),
					caption);
				return false;
			}
			var model = CreateModel(ods, solutionTypesProvider, context, caption);
			if(model == null)
				return false;
			if(model.ObjectType == null) {
				context.ShowMessage(
					DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsCannotResolveDataSource), caption);
				return false;
			}
			Type type = model.ObjectType.ResolveType();
			if(!ObjectBindingPagesRouter.TypeHasBindableMembers(type)) {
				context.ShowMessage(
					string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsNoMembersInType),
						TypeNamesHelper.ShortName(type)), caption);
				return false;
			}
			var runner = new DataMemberEditorRunner(context);
			var client = new ObjectDataSourceWizardClientUI(parameterService, solutionTypesProvider) {
				RepositoryItemsProvider = repositoryItemsProvider
			};
			if(!runner.Run(client, model))
				return false;
			ObjectDataSource result = new DataComponentCreator().CreateDataComponent(runner.WizardModel);
			ods.BeginUpdate();
			ods.DataMember = result.DataMember;
			ods.Parameters.Clear();
			ods.Parameters.AddRange(result.Parameters);
			ods.EndUpdate();
			return true;
		}
		#endregion
		#region EditParameters(...)
		public static bool EditParameters(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider,
			IWizardRunnerContext context, IParameterService parameterService) {
				return EditParameters(ods, solutionTypesProvider, context, parameterService, DefaultRepositoryItemsProvider.Instance);
		}
		public static bool EditParameters(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider,
			IWizardRunnerContext context, IParameterService parameterService,
			IRepositoryItemsProvider repositoryItemsProvider) {
			Guard.ArgumentNotNull(ods, "ods");
			Guard.ArgumentNotNull(solutionTypesProvider, "solutionTypesProvider");
			string caption = ParametersEditorCaption;
			if(ods.DataSource == null) {
				context.ShowMessage(DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsNoDataSetMessage),
					caption);
				return false;
			}
			if(ods.DataMember == null) {
				context.ShowMessage(
					DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsNoDataMemberMessage), caption);
				return false;
			}
			var model = CreateModel(ods, solutionTypesProvider, context, caption);
			if(model == null)
				return false;
			if(model.ObjectMember == null) {
				context.ShowMessage(
					DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsCannotResolveDataMember), caption);
				return false;
			}
			if(model.ObjectMember.IsProperty) {
				context.ShowMessage(
					string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsIsPropertyMessage),
						model.ObjectMember.Name), caption);
				return false;
			}
			if(model.MemberParameters.Length == 0) {
				context.ShowMessage(
					string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsNoParametersMessage),
						model.ObjectMember.Name), caption);
				return false;
			}
			var runner = new ParametersEditorRunner(context);
			var client = new ObjectDataSourceWizardClientUI(parameterService, solutionTypesProvider) {
				RepositoryItemsProvider = repositoryItemsProvider
			};
			if(!runner.Run(client, model))
				return false;
			var result = runner.WizardModel.MemberParameters;
			ods.BeginUpdate();
			ods.Parameters.Clear();
			ods.Parameters.AddRange(result);
			ods.EndUpdate();
			return true;
		}
		#endregion
		#region EditConstructor(...)
		public static bool EditConstructor(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider,
			IWizardRunnerContext context, IParameterService parameterService) {
				return EditConstructor(ods, solutionTypesProvider, context, parameterService, DefaultRepositoryItemsProvider.Instance);
		}
		public static bool EditConstructor(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider,
			IWizardRunnerContext context, IParameterService parameterService,
			IRepositoryItemsProvider repositoryItemsProvider) {
			return EditConstructor(ods, solutionTypesProvider, context, parameterService, OperationMode.Both,
				repositoryItemsProvider);
		}
		public static bool EditConstructor(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider,
			IWizardRunnerContext context, IParameterService parameterService, OperationMode operationMode) {
				return EditConstructor(ods, solutionTypesProvider, context, parameterService, operationMode, DefaultRepositoryItemsProvider.Instance);
		}
		public static bool EditConstructor(this ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider,
			IWizardRunnerContext context, IParameterService parameterService, OperationMode operationMode,
			IRepositoryItemsProvider repositoryItemsProvider) {
			Guard.ArgumentNotNull(ods, "ods");
			Guard.ArgumentNotNull(solutionTypesProvider, "solutionTypesProvider");
			string caption = CtorParametersEditorCaption;
			if(ods.DataSource == null) {
				context.ShowMessage(DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsNoDataSetMessage),
					caption);
				return false;
			}
			Type type = ods.DataSource as Type ?? ods.GetType();
			if(type.IsAbstract) {
				context.ShowMessage(
					string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsAbstractTypeMessage),
						TypeNamesHelper.ShortName(type)), caption);
				return false;
			}
			var model = CreateModel(ods, solutionTypesProvider, context, caption);
			if(model == null)
				return false;
			if(model.ObjectType == null) {
				context.ShowMessage(
					DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsCannotResolveDataSource), caption);
				return false;
			}
			if(model.ObjectMember != null && model.ObjectMember.IsStatic) {
				context.ShowMessage(
					string.Format(DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSEditorsStaticMemberMessage),
						model.ObjectMember.Name, TypeNamesHelper.ShortName(type)), caption);
				return false;
			}
			var runner = new CtorParametersEditorRunner(context);
			var client = new ObjectDataSourceWizardClientUI(parameterService, solutionTypesProvider, operationMode) {
				RepositoryItemsProvider = repositoryItemsProvider
			};
			if(!runner.Run(client, model))
				return false;
			DataAccess.ObjectBinding.Parameter[] result = runner.WizardModel.CtorParameters;
			if(runner.WizardModel.ObjectConstructor == null)
				result = null;
			else
				result = result ?? new DataAccess.ObjectBinding.Parameter[0];
			ods.BeginUpdate();
			ods.Constructor = result == null
				? null
				: result.Length == 0 ? ObjectConstructorInfo.Default : new ObjectConstructorInfo(result);
			ods.EndUpdate();
			return true;
		}
		#endregion
		#region private members
		static string DataSourceEditorCaption {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSDataSourceEditorTitle); }
		}
		static string DataMemberEditorCaption {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSDataMemberEditorTitle); }
		}
		static string ParametersEditorCaption {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSParametersEditorTitle); }
		}
		static string CtorParametersEditorCaption {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.ODSConstructorEditorTitle); }
		}
		static void ShowMessage(UserLookAndFeel lookAndFeel, IWin32Window owner, string message, string caption) {
			XtraMessageBox.Show(lookAndFeel, owner, message, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		static ObjectDataSourceModel CreateModel(ObjectDataSource ods, ISolutionTypesProvider solutionTypesProvider,
			IWizardRunnerContext context, string caption) {
			var waitFormActivator = context.WaitFormActivator;
			IExceptionHandler exceptionHandler = context.CreateExceptionHandler(ExceptionHandlerKind.Default, caption);
			var model = new ObjectDataSourceModel(ods, solutionTypesProvider, waitFormActivator, exceptionHandler);
			if(exceptionHandler.AnyExceptions)
				return null;
			return model;
		}
		#endregion
	}
}
