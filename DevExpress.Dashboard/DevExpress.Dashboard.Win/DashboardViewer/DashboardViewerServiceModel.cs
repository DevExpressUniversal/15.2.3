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
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DashboardWin.ServiceModel.Design;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Wizard.Services;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
namespace DevExpress.DashboardWin {
	public partial class DashboardViewer {
		ISharedServiceContainer serviceContainer;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public IServiceContainer ServiceContainer { get { return serviceContainer; } }
		internal IServiceProvider ServiceProvider { get { return serviceContainer; } }
		void RegisterServices() {
			Disposed += (s, e) => {
				if (serviceContainer != null) {
					serviceContainer.RemoveReference();
					serviceContainer = null;
				}
			};
			serviceContainer = new SharedServiceContainer();
			serviceContainer.AddReference();
			serviceContainer.AddService<IDashboardGuiContextService>(new DashboardGuiContextServiceRuntime(this));
			serviceContainer.AddService<IDashboardViewerInvalidateService>(new DashboardViewerInvalidateService(this));
			serviceContainer.AddService<IDashboardOwnerService>(new DashboardOwnerService());
			serviceContainer.AddService<IDashboardLoadingService>(new DashboardLoadingService());
			serviceContainer.AddService<IDashboardPaneAdapter>(new DashboardPaneAdapter(layoutControl, ServiceProvider));
			serviceContainer.AddService<IDashboardLayoutSelectionService>(new DashboardLayoutSelectionService(layoutControl));
			serviceContainer.AddService<IDashboardLayoutAccessService>(new DashboardLayoutAccessService(layoutControl));
			serviceContainer.AddService<IDashboardLayoutChangeService>(new DashboardLayoutChangeService(layoutControl));
			serviceContainer.AddService<IDashboardLayoutUpdateService>(new DashboardLayoutUpdateService(this, layoutControl, ServiceProvider));
			serviceContainer.AddService<IDashboardExportService>(new DashboardExportService(this));
			serviceContainer.AddService<IPopupMenuShowingService>(new PopupMenuShowingService());
			serviceContainer.AddService<IGaugeMinMaxProvider>(new GaugeMinMaxProvider(ServiceProvider));
			serviceContainer.AddService<IDashboardSessionAccessService>(new DashboardSessionAccessService(service));
			serviceContainer.AddService<IDashboardItemInteractivityService>(new DashboardItemInteractivityService(
				ServiceProvider.RequestServiceStrictly<IDashboardSessionAccessService>()));
			serviceContainer.AddService<IDashboardColoringCacheAccessService>(new DashboardColoringCacheAccessService(
				ServiceProvider.RequestServiceStrictly<IDashboardSessionAccessService>()));
			serviceContainer.AddService<IDashboardTitleCustomizationService>(new DashboardTitleCustomizationService(this));			
		}
		internal void ForceDashboardDesignTime(IDesignerHost designerHost) {
			serviceContainer.AddService<DashboardLayoutDragServiceDesignTime>(new DashboardLayoutDragServiceDesignTime(layoutControl, designerHost));
			serviceContainer.AddService<DashboardViewerSelectionBehaviorDesignTime>(new DashboardViewerSelectionBehaviorDesignTime(this, ServiceProvider));
			serviceContainer.ReplaceService<IDashboardGuiContextService>(new DashboardGuiContextServiceDesignTime(designerHost));
			ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>().ForceDashboardDesignTime(designerHost);
		}
		void SubscribeServiceEvents() {
			IDashboardOwnerService ownerService = ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>();
			ownerService.DashboardChanged += OnDashboardOwnerServiceDashboardChanged;
			IDashboardLoadingService loadingService = ServiceProvider.RequestServiceStrictly<IDashboardLoadingService>();
			loadingService.DashboardLoad += OnLoadingServiceDashboardLoad;
			loadingService.DashboardUnload += OnLoadingServiceDashboardUnload;
			IDashboardLayoutChangeService layoutChangedService = ServiceProvider.RequestServiceStrictly<IDashboardLayoutChangeService>();
			layoutChangedService.LayoutChanged += OnLayoutChanged;
		}
		void UnsubscribeServiceEvents() {
			if (ServiceProvider != null) {
				IDashboardOwnerService ownerService = ServiceProvider.RequestService<IDashboardOwnerService>();
				if (ownerService != null)
					ownerService.DashboardChanged -= OnDashboardOwnerServiceDashboardChanged;
				IDashboardLoadingService loadingService = ServiceProvider.RequestService<IDashboardLoadingService>();
				if (loadingService != null) {
					loadingService.DashboardLoad -= OnLoadingServiceDashboardLoad;
					loadingService.DashboardUnload -= OnLoadingServiceDashboardUnload;
				}
				IDashboardLayoutChangeService layoutChangedService = ServiceProvider.RequestService<IDashboardLayoutChangeService>();
				if(layoutChangedService != null)
					layoutChangedService.LayoutChanged -= OnLayoutChanged;
			}
		}
	}
}
