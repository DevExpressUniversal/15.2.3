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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Layout;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.About;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands.Design;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.DashboardWin {
	[
#if !DEBUG
#endif
	DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabData),
	Designer("DevExpress.DashboardWin.Design.DashboardDesignerControlDesigner," + AssemblyInfo.SRAssemblyDashboardWinDesign),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "DashboardDesigner.bmp")
	]
	public partial class DashboardDesigner : DashboardUserCommandControl, IOfficeCoreReference, IXtraChartsWizardReference {
		public static void About() {
		}
		const int DefaultDragAreaWidth = 200;
		const int DefaultDataSourceBrouserWidth = 200;
		const bool DefaultAllowEditComponentNames = false;
		readonly DataConnectionCollection customConnections = new DataConnectionCollection();
		readonly List<DataConnectionBase> connections = new List<DataConnectionBase>();
		readonly History history;
		readonly UIErrorHandler errorHandler;
		readonly DataSourceBrowserPresenter dataSourceBrowserPresenter = new DataSourceBrowserPresenter();
		DashboardDesignerDragManager dragManager;
		DragAreaScrollableControl dragAreaScrollableControl;
		DataSourceBrowser dataSourceBrowser;
		string dashboardFileName;
		DashboardActionOnClose actionOnClose = DashboardActionOnClose.Prompt;
		RibbonControl ribbonControl;
		DashboardBackstageViewControl ribbonViewControl;
		SizeF scaleFactor;
		IDXMenuManager menuManager;
		DashboardPopupMenu popupMenu;
		bool barManagerVisible = true;
		bool allowEditComponentName;
		ISelectedItemUIManager selectedItemUIManager;
		protected override BarAndDockingController BarAndDockingController { get { return dashboardBarAndDockingController; } }
		protected override IEnumerable<object> Children { get { return new object[] { splitChild, splitParent, dataSourceBrowser, dragAreaScrollableControl, dashboardViewer };  } }
		protected override BarManager BarManager { get { return MenuManager as BarManager; } }
		protected override RibbonControl Ribbon { get { return MenuManager as RibbonControl; } }
		int DragAreaWidth { get { return (int)(scaleFactor.Width * DefaultDragAreaWidth); } }
		int DataSouceBrouserWidth { get { return (int)(scaleFactor.Width * DefaultDataSourceBrouserWidth); } }
		internal DashboardDesignerDragManager DragManager { get { return dragManager; } }
		internal DragAreaScrollableControl DragAreaScrollableControl { get { return dragAreaScrollableControl; } }
		internal DataSourceBrowserPresenter DataSourceBrowserPresenter { get { return dataSourceBrowserPresenter; } }
		internal History History { get { return history; } }
		internal DashboardViewer Viewer { get { return dashboardViewer; } }
		internal List<DataConnectionBase> Connections { get { return connections; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataConnectionCollection CustomDataConnections { get { return customConnections; } }
		protected internal virtual bool IsDashboardVSDesignMode { get { return Dashboard.IsVSDesignMode; } }
		internal UIErrorHandler ErrorHandler { get { return errorHandler; } }
		internal bool BarManagerVisible { get { return barManagerVisible; } }
		internal ISelectedItemUIManager SelectedItemUIManager {
			get {
				if (selectedItemUIManager == null)
					UpdateSelectedItemUIManager();
				return selectedItemUIManager;
			}
		}
		internal DataSourceInfo SelectedDataSourceInfo {
			get {
				IDataSourceSelectionService service = ServiceProvider.RequestServiceStrictly<IDataSourceSelectionService>();
				return service.SelectedDataSourceInfo;
			}
			set {
				IDataSourceSelectionService service = ServiceProvider.RequestServiceStrictly<IDataSourceSelectionService>();
				service.SelectedDataSourceInfo = value;
			}
		}
		public DashboardDesigner()
			: this(new DataSourceBrowser()) {
		}
		internal DashboardDesigner(DataSourceBrowser dataSourceBrowser) {
			InitializeComponent();
			InitializeDashboardViewer();
			dragManager = new DashboardDesignerDragManager(this);
			history = new History(this);
			DataSourceWizardSettings = DashboardDataSourceWizardSettings.DefaultRuntime;
			errorHandler = new UIErrorHandler(this);
			RegisterServices();
			dragAreaScrollableControl = new DragAreaScrollableControl(this);
			InitializeDataSourceBrowser(dataSourceBrowser);			
			dataSourceBrowserPresenter.Initialize(ServiceProvider);
			dataSourceBrowserPresenter.SetVew(dataSourceBrowser);
			Action<SplitContainerControl> disableStatePainter = (control) => {
				control.UseDisabledStatePainter = false;
				control.Panel1.UseDisabledStatePainter = false;
				control.Panel2.UseDisabledStatePainter = false;
			};
			disableStatePainter(splitParent);
			disableStatePainter(splitChild);						
			splitChild.SplitterPosition = DragAreaWidth;
			dragAreaScrollableControl.Parent = splitChild.Panel1;
			history.Changed += (sender, e) => OnUpdateUI();
			SubscribeServiceEvents();
			AddCommands();
			NewDashboard();
		}
		void OnDashboardUnload(object sender, DashboardLoadingEventArgs e) {
			e.Dashboard.EnableAutomaticUpdatesChanged -= DashboardEnableAutomaticUpdatesChanged;
		}
		void OnDashboardLoad(object sender, DashboardLoadingEventArgs e) {
			e.Dashboard.EnableAutomaticUpdatesChanged += DashboardEnableAutomaticUpdatesChanged;
		}
		void DashboardEnableAutomaticUpdatesChanged(object sender, EnableAutomaticUpdatesEventArgs e) {
			Action updateUI = () => OnUpdateUI();
			if(InvokeRequired)
				Invoke(updateUI);
			else
				updateUI();
		}
		void InitializeDataSourceBrowser(DataSourceBrowser dataSourceBrowser) {
			this.dataSourceBrowser = dataSourceBrowser;
			if (dataSourceBrowser != null) {
				dataSourceBrowser.Parent = splitParent.Panel1;
				splitParent.SplitterPosition = DataSouceBrouserWidth;
			}
			else
				splitParent.PanelVisibility = SplitPanelVisibility.Panel2;
		}
		void InitializeDashboardViewer() {
			dashboardViewer.Designer = this;
			dashboardViewer.AllowPrintDashboardItems = true;
			dashboardViewer.MasterFilterSet += RaiseMasterFilterSet;
			dashboardViewer.MasterFilterCleared += RaiseMasterFilterCleared;
			dashboardViewer.SingleFilterDefaultValue += RaiseSingleFilterDefaultValue;
			dashboardViewer.DrillDownPerformed += RaiseDrillDownPerformed;
			dashboardViewer.DrillUpPerformed += RaiseDrillUpPerformed;
			dashboardViewer.ConfigureDataConnection += RaiseConfigureDataConnection;
			dashboardViewer.CustomFilterExpression += RaiseCustomFilterExpression;
			dashboardViewer.CustomParameters += RaiseCustomParameters;
			dashboardViewer.DataLoading += RaiseDataLoading;
			dashboardViewer.ConnectionError += RaiseConnectionError;
			dashboardViewer.BeforeExport += RaiseBeforeExport;
			dashboardViewer.DashboardLoaded += RaiseDashboardLoaded;
			dashboardViewer.DashboardChanged += RaiseDashboardChanged;
			dashboardViewer.DashboardItemControlCreated += RaiseDashboardItemControlCreated;
			dashboardViewer.DashboardItemBeforeControlDisposed += RaiseDashboardItemBeforeControlDisposed;
			dashboardViewer.DashboardItemControlUpdated += RaiseDashboardItemControlUpdated;
			dashboardViewer.ValidateCustomSqlQuery += RaiseValidateCustomSqlQuery;
		}
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			scaleFactor = factor;
			base.ScaleControl(factor, specified);
		}
		protected override void FocusCommandAwareControl() {
			dashboardViewer.Focus();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(dragManager != null) {
					dragManager.Dispose();
					dragManager = null;
				}
				if(ribbonControl != null) {
					ribbonControl.Dispose();
					ribbonControl = null;
				}
				if(ribbonViewControl != null) {
					ribbonViewControl.Dispose();
					ribbonViewControl = null;
				}
				if(dashboardViewer != null)
					dashboardViewer.Designer = null;
				if(dragAreaScrollableControl != null) {
					dragAreaScrollableControl.Dispose();
					dragAreaScrollableControl = null;
				}
				dataSourceBrowserPresenter.Dispose();
				if (dataSourceBrowser != null) {
					dataSourceBrowser.Dispose();
					dataSourceBrowser = null;
				}
				if(popupMenu != null)
					popupMenu.Dispose();
				UnsubscribeServiceEvents();
				RibbonControl ribbon = menuManager as RibbonControl;
				if(ribbon != null)
					ribbon.ShowCustomizationMenu -= OnRibbonShowCustomizationMenu;
				menuManager = null;
				if (components != null)
					components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			OnUpdateUI();
		}
		protected override void OnFirstLoad() {
			base.OnFirstLoad();
			Form form = ParentForm;
			if (form != null)
				form.Closing += (sender, args) => {
					if(!IsDisposed)
						args.Cancel = !HandleDashboardClosing();
				};
		}
		void OnRibbonShowCustomizationMenu(object sender, RibbonCustomizationMenuEventArgs e) {
			RibbonHitInfo hitInfo = e.HitInfo;
			if(hitInfo == null)
				e.ShowCustomizationMenu = false;
			else if(hitInfo.HitTest == RibbonHitTest.Item) {
				IDashboardViewerCommandBarItem barItem = e.Link.Item as IDashboardViewerCommandBarItem;
				e.ShowCustomizationMenu = barItem == null;
			}
		}				
		void OnDashboardEndInitialize(object sender, EventArgs e) {
			DXDisplayNameAttribute.UseResourceManager = !IsDashboardVSDesignMode;			
			history.Clear();
		}
		void OnDashboardItemSelected(object sender, DashboardItemSelectedEventArgs e) {
			UpdateSelectedItemUIManager();
			OnUpdateUI();
			RefreshRibbonPages(e.SelectedDashboardItem);
		}
		void RefreshRibbonPages(DashboardItem selectedDashboardItem) {
			RibbonControl ribbon = Ribbon;
			if(ribbon != null) {
				RibbonPageCategoryCollection categories = ribbon.PageCategories;
				RibbonControl mergedRibbon = ribbon.MergeOwner;
				if(mergedRibbon != null) {
					ribbon = mergedRibbon;
					categories = ribbon.MergedCategories;
				}
				RibbonPage selectedPage = ribbon.SelectedPage;
				bool selectDataPage = CheckPageType<DataRibbonPage>(selectedPage);
				bool selectDesignPage = CheckPageType<DashboardItemDesignRibbonPage>(selectedPage);
				foreach(RibbonPageCategory category in categories) {
					DashboardRibbonPageCategory dashboardCategory = category as DashboardRibbonPageCategory;
					if(dashboardCategory != null)
						SetCategoryVisibility(dashboardCategory, false);
				}
				if(selectedDashboardItem != null)
					foreach(RibbonPageCategory category in categories) {
						DashboardRibbonPageCategory dashboardCategory = category as DashboardRibbonPageCategory;
						if(dashboardCategory != null) {
							bool visible = dashboardCategory.CheckCurrentPage(DashboardCommon.Native.Helper.GetDashboardItemType(selectedDashboardItem));
							if(visible) {
								foreach(RibbonPage page in dashboardCategory.Pages) {
									if(selectDataPage && CheckPageType<DataRibbonPage>(page)) {
										ribbon.SelectedPage = page;
										break;
									}
									if(selectDesignPage && CheckPageType<DashboardItemDesignRibbonPage>(page)) {
										ribbon.SelectedPage = page;
										break;
									}
								}
								SetCategoryVisibility(dashboardCategory, true);
							}
						}
					}
			}
		}
		static bool CheckPageType<T>(RibbonPage page) {
			return page is T;
		}
		static void SetCategoryVisibility(DashboardRibbonPageCategory category, bool visible) {
			category.Visible = visible;
			foreach(RibbonPage page in category.Pages)
				page.Visible = visible;
		}
		void OnLayoutChanged(object sender, EventArgs e) {
			DashboardLayoutGroup layoutRoot = GetLayoutRoot();
			if (layoutRoot != null) {
				Dashboard.BeginUpdate();
				try {
					LayoutChangedHistoryItem historyItem = new LayoutChangedHistoryItem(Dashboard, layoutRoot);
					history.RedoAndAdd(historyItem);
				}
				finally {
					Dashboard.CancelUpdate();
				}
			}
		}
		internal DashboardLayoutGroup GetLayoutRoot() {
			IDashboardPaneAdapter paneAdapter = ServiceProvider.RequestServiceStrictly<IDashboardPaneAdapter>();
			DashboardPane rootPane = paneAdapter.GetRootPane();
			if (rootPane != null)
				return DashboardLayoutConverter.CreateDashboardLayout(rootPane, Dashboard.Items, Dashboard.Groups);
			return null;
		}
		Control FindControl(Type t) {
			foreach (Control control in Parent.Controls)
				if (t.IsAssignableFrom(control.GetType()))
					return control;
			return null;
		}
		void CreateBar(RibbonControl ribbon, BarManager barManager) {
			BarManager manager = barManager ?? ribbon.Manager;
			new DashboardRunTimeBarsGenerator(this, manager).AddNewBars(DashboardBarsUtils.GetBarCreators(ribbon != null), String.Empty, BarInsertMode.Add);
			if(popupMenu == null)
				popupMenu = new DashboardPopupMenu() { Manager = barManager, Ribbon = ribbon };
			DashboardBarsUtils.SetupDesignerPopupMenu(popupMenu);
		}
		internal bool RaiseDashboardCreating () {
			if(DashboardCreating != null) {
				DashboardCreatingEventArgs args = new DashboardCreatingEventArgs(Dashboard);
				DashboardCreating(this, args);
				return args.Handled;
			}
			return false;
		}
		internal void RaiseDashboardSaving(DashboardSavingEventArgs args) {
			if (DashboardSaving != null) {				
				DashboardSaving(this, args);				
			}			
		}
		internal void RaiseDashboardSaved(DashboardSavedEventArgs args) {
			if(DashboardSaved != null)
				DashboardSaved(this, args);
		}
		internal void RaiseDashboardOpening(DashboardOpeningEventArgs args) {
			if (DashboardOpening != null) {				
				DashboardOpening(this, args);
			}			
		}
		void RaiseDashboardClosing(DashboardClosingEventArgs e) {
			if(DashboardClosing != null)
				DashboardClosing(this, e);
		}
		internal void CompleteFileOperation(string fileName) {
			dashboardFileName = fileName;
			RibbonControl ribbon = MenuManager as RibbonControl;
			if (ribbon != null && !string.IsNullOrEmpty(fileName)) {
				DashboardBackstageViewControl viewControl = ribbon.ApplicationButtonDropDownControl as DashboardBackstageViewControl;
				if (viewControl != null)
					viewControl.DashboardRecentTab.RecentDashboardsControl.AddRecentDashboard(fileName);
			}
		}
		internal bool SaveDashboard(string fileName) {
			if(String.IsNullOrEmpty(fileName)) {
				using(SaveFileDialog saveFileDialog = new SaveFileDialog()) {
					fileName = DashboardFileName;
					saveFileDialog.Filter = FileFilters.DashboardFiles + FileFilters.Separator + FileFilters.AllFiles;
					saveFileDialog.FileName = fileName;
					if(!String.IsNullOrEmpty(fileName))
						saveFileDialog.InitialDirectory = Path.GetDirectoryName(fileName);
					if(saveFileDialog.ShowDialog(FindForm()) != DialogResult.OK)
						return false;
					fileName = saveFileDialog.FileName;
					if(String.IsNullOrEmpty(fileName))
						return false;
				}
			}
			using(MemoryStream mstream = new MemoryStream()) {
				try {
					Dashboard.SaveToXml(mstream);
					using(Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
						mstream.WriteTo(stream);
					History.IsModified = false;
					CompleteFileOperation(fileName);
				} catch {
					XtraMessageBox.Show(LookAndFeel, ParentForm,
						String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardSavingError), fileName),
						DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerCaption), 
						MessageBoxButtons.OK, MessageBoxIcon.Warning
					);
					return false;
				}
			}
			return true;
		}
		internal void OpenDashboard(string filePath, string directoryPath) {
			if(!string.IsNullOrEmpty(filePath))
				LoadDashboard(filePath);
			else {
				using(OpenFileDialog openDialog = new OpenFileDialog()) {
					openDialog.Filter = FileFilters.DashboardFiles;
					if(!string.IsNullOrEmpty(directoryPath))
						openDialog.InitialDirectory = directoryPath;
					if(openDialog.ShowDialog(FindForm()) == DialogResult.OK) {
						LoadDashboard(openDialog.FileName);
					}
				}
			}				
		}
		bool ExecuteSaveDashboardCommand() {
			return new SaveDashboardCommand(this).SaveDashboard(dashboardFileName);
		}
		internal DialogResult ShowFilterForm(DataDashboardItem selectedItem) {
			IDashboardDataSource dataSource = selectedItem != null ? selectedItem.DataSource : null;
			if(dataSource != null && dataSource.GetIsOlap()) {
				using(OlapFilterForm filterForm = new OlapFilterForm(this))
					return filterForm.ShowDialog(FindForm());				
			}
			else {
				using(DashboardFilterForm filterForm = DashboardFilterForm.CreateInstance(new SelectedDashboardItemFilterContext(selectedItem, ServiceProvider), ServiceProvider))
					return filterForm.ShowDialog(FindForm());				
			}
		}
		internal DialogResult ShowFilterForm(IDashboardDataSource selectedDataSource) {
			using(DashboardFilterForm filterForm = DashboardFilterForm.CreateInstance(new DataSourceFilterContext(selectedDataSource), ServiceProvider))
				return filterForm.ShowDialog(FindForm());
		}
		internal void NewDashboard() {
			OwnerService.CreateDashboard();
			dashboardFileName = null;
		}
		internal void UpdateSelectedItemUIManager() {
			selectedItemUIManager = DashboardItemUIManagerFactory.CreateManager(SelectedDashboardItem);
		}
		internal void UpdateData() {
			Viewer.RefreshData(true);
		}
	}
}
