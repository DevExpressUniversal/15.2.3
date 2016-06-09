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
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.UI.Native;
using DevExpress.DataAccess.UI.Native.Sql;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraSplashScreen;
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.UI.Native.Excel;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native.Excel;
namespace DevExpress.DashboardWin.Native {
	public class WinDashboardService : DashboardService {
		public static bool EnablePlatformConnectionParametersProvider = true;
		public static bool EnablePlatformConnectionErrorHandler = true;
		public static bool EnablePlatformWaitForm = true;
		IServiceProvider serviceProvider;
		WinDashboardServer WinServer { get { return (WinDashboardServer)Server; } }
		UserLookAndFeel LookAndFeel { get { return serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>().LookAndFeel; } }
		IWin32Window Win32Window { get { return serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>().Win32Window; } }
		public DashboardSession Session { get { return WinServer.Session; } }
		public event EventHandler<DashboardLoadingServerEventArgs> DashboardLoadingEvent;
		public event EventHandler<DashboardLoadedServerEventArgs> DashboardLoadedEvent;
		public event EventHandler<CustomFilterExpressionServerEventArgs> CustomFilterExpressionEvent;
		public event EventHandler<DataLoadingServerEventArgs> DataLoadingEvent;
		public event EventHandler<ConfigureDataConnectionServerEventArgs> ConfigureDataConnectionEvent;
		public event EventHandler<ConnectionErrorServerEventArgs> ConnectionErrorEvent;
		public event EventHandler<CustomParametersServerEventArgs> CustomParametersEvent;
		public event EventHandler<AllowLoadUnusedDataSourcesServerEventArgs> AllowLoadUnusedDataSourcesEvent;
		public event EventHandler<SingleFilterDefaultValueEventArgs> SingleFilterDefaultValue;
		public event EventHandler<FilterElementDefaultValuesEventArgs> FilterElementDefaultValues;
		public event EventHandler<RangeFilterDefaultValueEventArgs> RangeFilterDefaultValue;
		public event EventHandler<ValidateDashboardCustomSqlQueryEventArgs> ValidateCustomSqlQuery;
		public WinDashboardService()
			: base(new WinDashboardServer(), true) {
		}
		public void Initialize(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			if(this.serviceProvider == null)
				this.serviceProvider = serviceProvider;
			else
				throw new InvalidOperationException("Attemping to initialize WinDashboardService twice");
		}
		public void DisposeSession() {
			WinServer.DisposeSession();
		}
		protected override void DashboardLoading(DashboardLoadingServerEventArgs e) {
			if(DashboardLoadingEvent != null)
				DashboardLoadingEvent(this, e);
		}
		protected override void DashboardLoaded(DashboardLoadedServerEventArgs e) {
			if(DashboardLoadedEvent != null)
				DashboardLoadedEvent(this, e);
		}
		protected override void CustomFilterExpression(CustomFilterExpressionServerEventArgs e) {
			if(CustomFilterExpressionEvent != null)
				CustomFilterExpressionEvent(this, e);
		}
		protected override void DataLoading(DataLoadingServerEventArgs e) {
			if(DataLoadingEvent != null)
				DataLoadingEvent(this, e);
		}
		protected override void ConfigureDataConnection(ConfigureDataConnectionServerEventArgs e) {
			if(ConfigureDataConnectionEvent != null)
				ConfigureDataConnectionEvent(this, e);
		}
		protected override void ConnectionError(ConnectionErrorServerEventArgs e) {
			if(ConnectionErrorEvent != null)
				ConnectionErrorEvent(this, e);
		}
		protected override void CustomParameters(CustomParametersServerEventArgs e) {
			if(CustomParametersEvent != null)
				CustomParametersEvent(this, e);
		}
		protected override void AllowLoadUnusedDataSources(AllowLoadUnusedDataSourcesServerEventArgs e) {
			if(AllowLoadUnusedDataSourcesEvent != null)
				AllowLoadUnusedDataSourcesEvent(this, e);
		}
		protected override void OnDashboardUnloading(DashboardUnloadingEventArgs e) {
		}
		protected override void RequestCustomizationServices(RequestCustomizationServicesEventArgs e) {
			if(!EnablePlatformConnectionParametersProvider)
				return;
			TaskScheduler taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			e.SqlCustomizationService = new WinFormsSqlCustomizationServiceUI(taskScheduler, LookAndFeel, Win32Window);
			e.ExcelCustomizationService = new WinFormsExcelCustomizationServiceUI(taskScheduler, LookAndFeel, Win32Window);
			e.PlatformDependenciesService = new PlatformDependenciesService();
		}
		protected override void OnRequestAppConfigPatcherService(RequestAppConfigPatcherServiceEventArgs e) {
			e.AppConfigPatcherService = serviceProvider.RequestService<IConnectionStringsService>();
		}
		protected override void RequestWaitFormActivator(RequestWaitFormActivatorEventArgs e) {
			if(!EnablePlatformWaitForm)
				return;
			if(SplashScreenManager.Default == null) {
				Form form = Win32Window as Form;
				e.WaitFormActivator = Win32Window == null || form != null ?
					(IWaitFormActivator)new WaitFormActivator(form, typeof(WaitFormWithCancel)) :
					new WaitFormActivatorDesignTime(Win32Window, typeof(WaitFormWithCancel), (LookAndFeel ?? UserLookAndFeel.Default).ActiveSkinName);
			}
		}
		protected override void RequestErrorHandler(RequestErrorHandlerEventArgs e) {
			if(!EnablePlatformConnectionErrorHandler)
				return;
			IErrorHandlerForm errorHandlerForm = new ErrorHandler();
			errorHandlerForm.Initialize(Win32Window, LookAndFeel);
			e.ErrorHandler = errorHandlerForm;
		}
		protected override void RequestUnderlyingDataFormat(RequestUnderlyingDataFormatEventArgs e) {
			e.SupportedFormat = UnderlyingDataFormat.DataSet;
		}
		protected override void OnSingleFilterDefaultValue(SingleFilterDefaultValueEventArgs e) {
			if (SingleFilterDefaultValue != null)
				SingleFilterDefaultValue(this, e);
		}
		protected override void OnFilterElementDefaultValues(FilterElementDefaultValuesEventArgs e) {
			if (FilterElementDefaultValues != null)
				FilterElementDefaultValues(this, e);
		}
		protected override void OnRangeFilterDefaultValue(RangeFilterDefaultValueEventArgs e) {
			if (RangeFilterDefaultValue != null)
				RangeFilterDefaultValue(this, e);
		}
		protected override void OnValidateCustomSqlQuery(ValidateDashboardCustomSqlQueryEventArgs e) {
			if (ValidateCustomSqlQuery != null)
				ValidateCustomSqlQuery(this, e);
		}
	}
	public class WinFormsSqlCustomizationServiceUI : DataConnectionParametersServiceUI, IDashboardSqlCustomizationService {
		public IWaitFormActivator CurrentWaitFormActivator { get; set; }
		public WinFormsSqlCustomizationServiceUI(TaskScheduler ui, UserLookAndFeel lookAndFeel, IWin32Window win32Window)
			: base(ui, null, lookAndFeel, win32Window) {
		}
		public void SetNativeProvider(IDataConnectionParametersService nativeProvider) {
			NativeConnectionProvider = nativeProvider;
		}
		protected override DataConnectionParametersBase ShowForm(string connectionName, DataConnectionParametersBase connectionParameters, string message) {
			if(CurrentWaitFormActivator != null)
				CurrentWaitFormActivator.CloseWaitForm();
			DataConnectionParametersBase result = base.ShowForm(connectionName, connectionParameters, message);
			if(CurrentWaitFormActivator != null)
				CurrentWaitFormActivator.ShowWaitForm(true, false, true);
			return result;
		}
	}
	public class WinFormsExcelCustomizationServiceUI : ExcelOptionsCustomizationServiceUI, IDashboardExcelCustomizationService {
		public IWaitFormActivator CurrentWaitFormActivator { get; set; }
		public WinFormsExcelCustomizationServiceUI(TaskScheduler ui, UserLookAndFeel lookAndFeel, IWin32Window win32Window)
			: base(win32Window, ui, lookAndFeel) {
		}
		public override void Customize(BeforeFillEventArgs eventArgs) {
			if(CurrentWaitFormActivator != null)
				CurrentWaitFormActivator.CloseWaitForm();
			base.Customize(eventArgs);
			if(CurrentWaitFormActivator != null)
				CurrentWaitFormActivator.ShowWaitForm(true, false, true);
		}
	}
	public class PlatformDependenciesService : IPlatformDependenciesService {
		public void DoEvents() {
			Application.DoEvents();
		}
	}
}
