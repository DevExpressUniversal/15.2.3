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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Native.Printing;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.Native;
using DevExpress.Office.Utils;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.About;
using DevExpress.XtraBars;
using DevExpress.XtraDashboardLayout;
using DevExpress.XtraLayout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.DashboardWin {
	public enum DashboardPrintPreviewType { RibbonPreview, StandardPreview }
	public enum DashboardSelectionMode { None, Single, Multiple };
	[
#if !DEBUG
#endif
	DXToolboxItem(true), 
	ToolboxTabName(AssemblyInfo.DXTabData),
	Designer("DevExpress.DashboardWin.Design.DashboardViewerDesigner," + AssemblyInfo.SRAssemblyDashboardWinDesign),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "DashboardViewer.bmp")
	]
	public partial class DashboardViewer : DashboardUserControl, ISupportInitialize, IOfficeCoreReference, DevExpress.XtraCharts.Native.IXtraChartsWizardReference, IDashboardExportUIProvider, IExportOptionsOwner, IDashboardExportItem, IUnderlyingControlProvider {
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern IntPtr SendMessage(IntPtr hWnd,
			uint wMsg,
			UIntPtr wParam,
			IntPtr lParam);
		public static void About() {
		}
		const int WM_SETREDRAW = 11; 
		const bool DefaultAllowPrintDashboard = true;
		const bool DefaultAllowPrintDashboardItmes = false;
		const bool DefaultPopulateUnusedDataSources = false;
		const bool DefaultUseDataAccessApi = false;
		const bool DefaultCalculateHiddenTotals = false;
		internal const DashboardPrintPreviewType DefaultPrintPreviewType = DashboardPrintPreviewType.RibbonPreview;
		readonly DashboardLayoutControl layoutControl = new DashboardLayoutControl();
		readonly BarAndDockingController barAndDockingController = new BarAndDockingController();
		readonly ToolTipController toolTipController = new ToolTipController();
		readonly Locker loadingLocker = new Locker();
		readonly Locker parametersLocker = new Locker();
		readonly BarManager viewerBarManager;
		readonly DashboardPrintingOptions printingOptions = new DashboardPrintingOptions();
		DashboardTitleControl title;
		DashboardTitlePresenter titlePresenter;
		object dashboardSource;
		DashboardDesigner designer;
		bool useDataAccessApi = DefaultUseDataAccessApi;
		bool calculateHiddenTotals = DefaultCalculateHiddenTotals;
		bool isDesignMode = true;
		bool populateUnusedDataSources = DefaultPopulateUnusedDataSources;
		DashboardParameters parameters;
		DashboardPopupMenu designerPopupMenu;
		protected virtual IDashboardService DashboardService { get { return service; } }
		protected override BarAndDockingController BarAndDockingController { get { return barAndDockingController; } }
		protected override IEnumerable<object> Children { get { return new object[] { title, layoutControl }; } }
		internal bool IsDashboardVSDesignMode { get { return designer != null && designer.IsDashboardVSDesignMode; } }
		internal DashboardLayoutControl LayoutControl { get { return layoutControl; } }
		internal BarManager ViewerBarManager { get { return viewerBarManager; } }
		internal DashboardTitleControl Title { get { return title; } }
		internal DashboardDesigner Designer {
			get { return designer; }
			set {
				if(value != designer) {
					designer = value;
					BeginUpdateLayout();
					try {
						SetDesignMode(designer != null);
					}
					finally {
						EndUpdateLayout(false);
					}
				}
			}
		}
		internal bool IsDesignMode { get { return isDesignMode; } }
		internal IEnumerable<DashboardItemViewer> ItemViewers { get { return LayoutAccessService.ItemViewers; } }
		Type DashboardSourceType {
			get {
				Type type = null;
				string str = dashboardSource as string;
				if(!string.IsNullOrEmpty(str))
					try {
						type = AsmHelper.GetType(null, str);
					} catch { }
				if(type == null)
					type = dashboardSource as Type;
				return type;
			}
		}
		Uri DashboardSourceUri {
			get {
				Uri uri = dashboardSource as Uri;
				if(uri != null)
					return uri;
				string url = dashboardSource as string;
				if(url != null) {
					try {
						return new Uri(url, UriKind.RelativeOrAbsolute);
					}
					catch { }
				}
				return null;
			}
		}
		bool HasUnusedDataSources {
			get {
				if(Dashboard != null) {
					foreach(IDashboardDataSource dataSource in Dashboard.DataSources) {
						bool used = false;
						foreach(DataDashboardItem dashboardItem in Dashboard.Items.OfType<DataDashboardItem>()) {
							if(dashboardItem.DataSource == dataSource) {
								used = true;
								break;
							}
						}
						used = used ||
							Dashboard.Parameters.Select(parameter => parameter.LookUpSettings as DynamicListLookUpSettings).Where(setting => setting != null).Any(setting => setting.DataSource == dataSource);
						if(!used)
							return true;
					}
				}
				return false;
			}
		}
		bool ExportButtonVisible { get { return !IsDashboardVSDesignMode && (IsDesignMode || AllowPrintDashboard); } }
		Rectangle TitleBounds { get { return new Rectangle(title.Bounds.Left, title.Bounds.Top, layoutControl.ClientRectangle.Width, title.ClientRectangle.Height); } }
		public DashboardViewer()
			: this(null) {
		}
		public DashboardViewer(IContainer container) {
			service = new WinDashboardService();
			RegisterServices();
			service.Initialize(ServiceProvider);
			SubscribeServiceEvents();						
			serviceClient = new DashboardServiceClient(DashboardService, ServiceProvider);
			InitializeDashboardService();
			viewerBarManager = CreateBarManager(container ?? new Container());
			Initialize();
		}
		internal void RefreshData(bool forceUpdate) {
			Hashtable clientState = GetDashboardClientState();
			RefreshResult refreshResult = serviceClient.Refresh(clientState, Parameters, forceUpdate);
			if(refreshResult != null)
				RefreshViewer(refreshResult, false);
		}
		internal void SelectDashboard() {
			IDashboardSelectionService dashboardSelection = ServiceProvider.RequestService<IDashboardSelectionService>();
			if (dashboardSelection != null)
				dashboardSelection.SelectDashboard();
		}
		internal void SetDesignMode(bool mode) {
			if (mode != isDesignMode) {
				isDesignMode = mode;
				serviceClient.IsDesignMode = mode;
				if(IsHandleCreated)
					ForceDesignModeChanges();
			}
		}
		internal IDashboardLayoutControlItem FindLayoutItem(string componentName) {
			return LayoutAccessService.FindLayoutItem(componentName);
		}
		internal DashboardLayoutControlItem FindLayoutControlItem(string componentName) {
			return LayoutAccessService.FindLayoutControlItem(componentName);
		}
		internal DashboardLayoutControlGroup FindLayoutControlGroup(string componentName) {
			return LayoutAccessService.FindLayoutControlGroup(componentName);
		}
		internal DashboardItemViewer FindDashboardItemViewer(string componentName) {			
			return LayoutAccessService.FindDashboardItemViewer(componentName);
		}
		internal Size GetClientSize() {
			int clientWidth = layoutControl.Width;
			int clientHeight = layoutControl.Height;
			if(clientWidth <= 0 || clientHeight <= 0) {
				clientWidth = 100;
				clientHeight = 100;
			}
			return new Size(clientWidth, clientHeight);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			ForceDesignModeChanges();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			LookAndFeelStyleChanged();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (ServiceProvider != null) {
					IDashboardOwnerService ownerService = ServiceProvider.RequestService<IDashboardOwnerService>();
					if (ownerService != null && ownerService.Dashboard != null)
						UnsubscribeDashboardEvents(ownerService.Dashboard);
				}
				UnsubscribeServiceEvents();
				UnsubscribeLayoutControlEvents();
				if (service != null) {
					service.Dispose();
					service = null;
				}
				if (designerPopupMenu != null) {
					designerPopupMenu.CloseUp -= OnPopupMenuCloseUp;
					designerPopupMenu = null;
				}
				Controls.Clear();
				if(layoutControl != null) {
					layoutControl.Dispose();
				}
				if(title != null) {
					title.Dispose();
				}
				DisposeViewerPopupMenu();
			}
			base.Dispose(disposing);
		}
		void ForceDesignModeChanges() {
			BeginUpdateLayout();
			try {
				layoutControl.AllowDragDrop = isDesignMode;
				layoutControl.AllowSelection = isDesignMode;
				layoutControl.AllowCrosshair = isDesignMode;
				layoutControl.AllowResize = true;
				OptionsView opionsView = layoutControl.OptionsView;
				opionsView.AllowHotTrack = isDesignMode;
				opionsView.DrawItemBorders = isDesignMode;
			}
			finally {
				EndUpdateLayout(false);
			}
		}
		void LoadDashboard() {
			if (!loadingLocker.IsLocked && !DesignMode) {
				IDashboardOwnerService ownerService = ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>();
				if (DashboardSourceType != null)
					ownerService.CreateDashboard(DashboardSourceType);
				else if (DashboardSourceUri != null) {
					string sourceDirectory = String.Empty;
					ISite site = Site;
					if (site != null) {
						IDesignerHost designerHost = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
						if (designerHost != null) {
							IVSDashboardDesigner designer = designerHost.GetDesigner(this) as IVSDashboardDesigner;
							if (designer != null)
								sourceDirectory = designer.SourceDirectory;
						}
					}
					ownerService.CreateDashboard(DashboardSourceUri, sourceDirectory);
				}
				else
					ownerService.SetDashboard(null);
			}
		}
		BarManager CreateBarManager(IContainer container) {
			BarManager barManager = new BarManager(container);
			barManager.Form = this;
			barManager.Controller = barAndDockingController;
			return barManager;
		}
		void LookAndFeelStyleChanged() {
			Control parent = Parent;
			layoutControl.BackColor = parent == null ? CommonSkins.GetSkin(LookAndFeel).Colors.GetColor("Control") : parent.BackColor;
		}
		void Initialize() {
			SetStyle(ControlStyles.ResizeRedraw, true);
			title = new DashboardTitleControl(this);
			titlePresenter = new DashboardTitlePresenter(title, ServiceProvider.RequestServiceStrictly<IDashboardTitleCustomizationService>());
			DashboardLayoutControlGroup layoutControlGroup = new DashboardLayoutControlGroup(this, ServiceProvider, true);
			((ISupportInitialize)layoutControl).BeginInit();
			((ISupportInitialize)layoutControlGroup).BeginInit();
			BorderStyle = BorderStyle.None;
			Padding = new Padding(0);
			layoutControl.DrawItemGroupBorders = true;
			layoutControlGroup.EnableIndentsWithoutBorders = DefaultBoolean.True;
			layoutControlGroup.GroupBordersVisible = false;
			layoutControlGroup.Padding = DevExpress.XtraLayout.Utils.Padding.Empty;
			layoutControlGroup.AppearanceItemCaption.TextOptions.Trimming = Trimming.EllipsisCharacter;
			layoutControl.Root = layoutControlGroup;
			layoutControl.Dock = DockStyle.Fill;
			OptionsView optionsView = layoutControl.OptionsView;
			optionsView.AllowItemSkinning = true;
			SubscribeLayoutControlEvents();
			((ISupportInitialize)layoutControlGroup).EndInit();
			((ISupportInitialize)layoutControl).EndInit();  
			title.Dock = DockStyle.Top;
			title.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			this.SuspendLayout();
			AddControls();
			this.ResumeLayout();
			SetDesignMode(designer != null);
			LookAndFeel.StyleChanged += (sender, e) => LookAndFeelStyleChanged();
			exportProvider = this;
			exportOptionsRepository = new ExportOptionsRepository();
		}		
		void AddControls() {
			Controls.Add(layoutControl);
			Controls.Add(title);
		}
		void RemoveControls() {
			Controls.Remove(layoutControl);
			Controls.Remove(title);
		}
		void RefreshViewer(RefreshResult result, bool clearLayout) {
			BeginUpdateLayout();
			try {
				if (clearLayout)
					ClearLayout();
				RefreshLayoutControl(result.PaneContent);
				if (result.RootPane != null)
					UpdateLayoutControlByDashboardLayout(result.RootPane);
			}
			finally {
				EndUpdateLayout(true);
			}
			if (result.DashboardParameters != null)
				UpdateParameters(result.DashboardParameters);
			UpdateDashboardTitle(result.TitleViewModel, result.MasterFilterValues);
		}
		void UpdateParameters(IList<DashboardParameterViewModel> parameterViewModels) {
			IList<DashboardParameterDescriptor> list;
			if(Dashboard != null)
				list = parameterViewModels.Select(p => new DashboardParameterDescriptor(p)).ToList();
			else
				list = new List<DashboardParameterDescriptor>();
			parameters = new DashboardParameters(list);
			parameters.CollectionChanged += (s, e) => {
				if(!parametersLocker.IsLocked)
					ReloadData();
			};
		}
		void UpdateDashboardTitle(DashboardTitleViewModel viewModel, IList<DimensionFilterValues> masterFilterValues) { 
			titlePresenter.Update(viewModel, masterFilterValues, ExportButtonVisible);
		}
		void UpdateDashboardTitle(IList<DimensionFilterValues> masterFilterValues) {
			titlePresenter.Update(masterFilterValues);
		}
		void UpdateDashboardTitle() {
			titlePresenter.Update(ExportButtonVisible);
		}
		void OnLoadingServiceDashboardLoad(object sender, DashboardLoadingEventArgs e) {
			SubscribeDashboardEvents(e.Dashboard);
		}
		void OnLoadingServiceDashboardUnload(object sender, DashboardLoadingEventArgs e) {
			UnsubscribeDashboardEvents(e.Dashboard);
		}
		void OnDashboardOwnerServiceDashboardChanged(object sender, DashboardChangedEventArgs e) {
			Dashboard oldDashboard = e.OldDashboard;
			Dashboard newDashboard = e.NewDashboard;
			IDashboardLoadingService loadingService = ServiceProvider.RequestServiceStrictly<IDashboardLoadingService>();
			loadingService.OnDashboardUnload(oldDashboard);
			if (newDashboard != null)
				RaiseDashboardLoaded(newDashboard);
			loadingService.OnDashboardLoad(newDashboard);
			loadingService.OnDashboardBeginInitialize();
			InitializeDashboard(newDashboard);
			loadingService.OnDashboardEndInitialize();
			IDefaultSelectionService defaultSelection = ServiceProvider.RequestService<IDefaultSelectionService>();
			if (defaultSelection != null)
				defaultSelection.SetDefaultSelection();
			RaiseDashboardChanged();
		}
		void InitializeDashboard(Dashboard dashboard) {			
			if (dashboard != null) {
				InitializeResult result = null;
				EventHandler<DashboardLoadingServerEventArgs> dashboardLoadingHandler = (s, arg) => {
					arg.Dashboard = dashboard;
				};
				service.DashboardLoadingEvent += dashboardLoadingHandler;
				try {
					Hashtable clientState = GetDashboardClientState();
					result = serviceClient.Initialize(clientState, CalculateHiddenTotals);
				}
				finally {
					service.DashboardLoadingEvent -= dashboardLoadingHandler;
				}
				if (result != null) {					
					layoutControl.Enabled = true;
					RefreshViewer(result, true);
				}
			}
			else {
				service.DisposeSession();
				ClearLayout();
				layoutControl.Enabled = false;
				Title.Visible = false;
			}
		}
		Hashtable GetDashboardClientState() {
			Size clientSize = GetClientSize();
			Hashtable clientState = new Hashtable();
			Hashtable defaultState = new Hashtable();
			defaultState.Add("ClientWidth", clientSize.Width);
			defaultState.Add("ClientHeight", clientSize.Height);
			clientState.Add("", defaultState);
			return clientState;
		}
		void ISupportInitialize.BeginInit() {
			loadingLocker.Lock();
		}
		void ISupportInitialize.EndInit() {
			loadingLocker.Unlock();
			LoadDashboard();
		}
	}
}
