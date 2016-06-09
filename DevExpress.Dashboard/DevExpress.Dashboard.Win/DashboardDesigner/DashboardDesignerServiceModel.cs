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

using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DashboardWin.ServiceModel.Design;
using DevExpress.Data.Entity;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.Utils.Design;
using DevExpress.Utils.UI;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin {
	public partial class DashboardDesigner {
		ISharedServiceContainer serviceContainer;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public IServiceContainer ServiceContainer { get { return serviceContainer; } }
		protected internal override IServiceProvider ServiceProvider { get { return serviceContainer; } }
		IDashboardOwnerService OwnerService {
			get { return ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>(); }
		}
		void RegisterServices() {
			Disposed += (s, e) => {
				if (serviceContainer != null) {
					serviceContainer.RemoveReference();
					serviceContainer = null;
				}
			};
			serviceContainer = (ISharedServiceContainer)dashboardViewer.ServiceContainer;
			serviceContainer.AddReference();
			serviceContainer.AddService<IDashboardDesignerDragService>(new DashboardDesignerDragService(DragManager));
			serviceContainer.AddService<IDashboardDesignerHistoryService>(new DashboardDesignerHistoryService(History));
			serviceContainer.AddService<IDashboardCommandService>(new DashboardCommandService(this));
			serviceContainer.AddService<IDashboardDesignerUpdateUIService>(new DashboardDesignerUpdateUIService(this));
			serviceContainer.AddService<IDashboardErrorMessageService>(new DashboardErrorMessageService(this));
			serviceContainer.AddService<IDashboardDesignerSelectionService>(new DashboardDesignerSelectionService(ServiceProvider));
			serviceContainer.AddService<IDashboardItemDesignerFactory>(new DashboardItemDesignerFactory(this, ServiceProvider));
			serviceContainer.AddService<IDataSourceSelectionService>(new DataSourceSelectionService(dataSourceBrowserPresenter));
			serviceContainer.AddService<IDefaultSelectionService>(new DashboardDesignerSelectionBehaviorRuntime(ServiceProvider));
			serviceContainer.AddService<IDataFieldsBrowserPresenterFactory>(new DataFieldsBrowserPresenterFactory(dataSourceBrowserPresenter));
			serviceContainer.AddService<IDataFieldChangeService>(new DataFieldChangeService());
			serviceContainer.AddService<IRefreshFieldListService>(new RefreshFieldListService(this));
			serviceContainer.AddService<IDesignerUpdateService>(new DesignerUpdateService());
			serviceContainer.AddService<IConnectionStringsProvider>(new DashboardRuntimeConnectionStringsProvider());
			serviceContainer.AddService<IConnectionStorageService>(new DashboardConnectionStorageService(this));
			serviceContainer.AddService<ISolutionTypesProvider>(DashboardSolutionTypesProviderRuntime.Instance);
			serviceContainer.AddService<SelectedContextService>(new SelectedContextService(this, null));
			serviceContainer.AddService<IDBSchemaProviderFactory>(new DBSchemaProviderFactory(this));
			serviceContainer.AddService<ISupportedDataSourceTypesService>(new SupportedDataSourceTypesService());
			serviceContainer.AddService<IDashboardDataSourceWizardSettingsProvider>(new DashboardDataSourceWizardSettingsProviderRuntime(this));
			var parameterService = new DashboardParameterService(ServiceProvider);
			serviceContainer.AddService<IDashboardParameterService>(parameterService);
			serviceContainer.AddService<IParameterService>(parameterService);
			serviceContainer.AddService<IParameterCreator>(parameterService);
			serviceContainer.AddService<TypeDescriptionProvider>(new ParameterDescriptionProvider(parameterService));
			serviceContainer.AddService<IDataSourceNameCreationService>(new DataSourceNameCreationService(ServiceProvider));
			var queryValidator = new DashboardCustomQueryValidator(this);
			serviceContainer.AddService<ICustomQueryValidator>(queryValidator);
			serviceContainer.AddService<IDataSourceEditContext>(queryValidator);
			serviceContainer.AddService<UILocker>(new UILocker());
		}
		internal void ForceDashboardDesignTime(IDesignerHost designerHost, IConnectionStringsProvider connectionStringsProvider, 
			IConnectionStorageService connectionStorageService, ISolutionTypesProvider solutionTypesProvider,
			IConnectionStringsService vsAppConfigPatcherService, ISupportedDataSourceTypesService dataSourceTypesService,
			IDTEService IDTEService, ICustomQueryValidator customQueryValidator
			) {
			dashboardViewer.ForceDashboardDesignTime(designerHost);
			if (connectionStringsProvider != null)
				serviceContainer.ReplaceService<IConnectionStringsProvider>(connectionStringsProvider);
			if (connectionStorageService != null)
				serviceContainer.ReplaceService<IConnectionStorageService>(connectionStorageService);
			if (solutionTypesProvider != null)
				serviceContainer.ReplaceService<ISolutionTypesProvider>(solutionTypesProvider);
			if(vsAppConfigPatcherService != null)
				serviceContainer.ReplaceService<IConnectionStringsService>(vsAppConfigPatcherService);
			if(dataSourceTypesService != null)
				serviceContainer.ReplaceService<ISupportedDataSourceTypesService>(dataSourceTypesService);
			if(IDTEService != null)
				serviceContainer.AddService<IDTEService>(IDTEService);
			var designTimeBehavior = new DashboardDesignerBehaviorDesignTime(ServiceProvider, designerHost);
			serviceContainer.AddService<DashboardDesignerBehaviorDesignTime>(designTimeBehavior);
			serviceContainer.ReplaceService<IDefaultSelectionService>(designTimeBehavior);
			serviceContainer.AddService<IDashboardSelectionService>(designTimeBehavior);
			serviceContainer.AddService<DashboardComponentSynchronizer>(new DashboardComponentSynchronizer(ServiceProvider, designerHost));
			serviceContainer.ReplaceService<IDashboardDataSourceWizardSettingsProvider>(new DashboardDataSourceWizardSettingsProviderDesignTime());
			if (customQueryValidator != null) {
				serviceContainer.ReplaceService<ICustomQueryValidator>(customQueryValidator);
				serviceContainer.RemoveService<IDataSourceEditContext>();
			}
		}
		void SubscribeServiceEvents() {
			IDashboardLoadingService loadingService = ServiceProvider.RequestServiceStrictly<IDashboardLoadingService>();
			loadingService.DashboardEndInitialize += OnDashboardEndInitialize;
			loadingService.DashboardLoad += OnDashboardLoad;
			loadingService.DashboardUnload += OnDashboardUnload;
			IDashboardDesignerSelectionService designerSelectionService = ServiceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>();
			designerSelectionService.DashboardItemSelected += OnDashboardItemSelected;
			IDashboardLayoutChangeService layoutChangeService = ServiceProvider.RequestServiceStrictly<IDashboardLayoutChangeService>();
			layoutChangeService.LayoutChanged += OnLayoutChanged;
		}
		void UnsubscribeServiceEvents() {
			if (ServiceProvider != null) {
				IDashboardLoadingService loadingService = ServiceProvider.RequestService<IDashboardLoadingService>();
				if(loadingService != null) {
					loadingService.DashboardEndInitialize -= OnDashboardEndInitialize;
					loadingService.DashboardLoad -= OnDashboardLoad;
					loadingService.DashboardUnload -= OnDashboardUnload;
				}
				IDashboardDesignerSelectionService designerSelectionService = ServiceProvider.RequestService<IDashboardDesignerSelectionService>();
				if (designerSelectionService != null)
					designerSelectionService.DashboardItemSelected += OnDashboardItemSelected;
				IDashboardLayoutChangeService layoutChangeService = ServiceProvider.RequestService<IDashboardLayoutChangeService>();
				if (layoutChangeService != null)
					layoutChangeService.LayoutChanged -= OnLayoutChanged;
			}
		}
	}
}
