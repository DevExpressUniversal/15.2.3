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
using System.IO;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.Utils;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.DataAccess.UI.Wizard;
namespace DevExpress.DashboardWin {
	public partial class DashboardDesigner : IUnderlyingControlProvider {
		[
#if !SL
	DevExpressDashboardWinLocalizedDescription("DashboardDesignerMenuManager"),
#endif
		DefaultValue(null)
		]
		public IDXMenuManager MenuManager { get { return menuManager; } set { menuManager = value; } }
		[
		DefaultValue(null),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public DashboardPopupMenu PopupMenu { get { return popupMenu; } set { popupMenu = value; } }
		[
		DefaultValue(DefaultAllowEditComponentNames)
		]
		public bool AllowEditComponentName {
			get { return IsDashboardVSDesignMode ? true : allowEditComponentName; }
			set { allowEditComponentName = value; }
		}
		[
		Browsable(false), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool IsDashboardModified { get { return Dashboard != null && history.IsModified; } }
		[
		Browsable(false), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Dashboard Dashboard {
			get {
				IDashboardOwnerService ownerService = ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>();
				return ownerService.Dashboard;
			}
			set {
				if (value == null)
					throw new ArgumentException(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageNullDashboard));
				IDashboardOwnerService ownerService = ServiceProvider.RequestServiceStrictly<IDashboardOwnerService>();
				if (value != ownerService.Dashboard) {
					ownerService.SetDashboard(value);
					dashboardFileName = null;
				}
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string DashboardFileName { get { return dashboardFileName; } }
		[
		Browsable(false), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public IDashboardDataSource SelectedDataSource {
			get {
				DataSourceInfo selectedInfo = SelectedDataSourceInfo;
				return selectedInfo != null ? selectedInfo.DataSource : null;
			}
			set { 
				SelectedDataSourceInfo = new DataSourceInfo(value, null); 
			}
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string SelectedDataMember {
			get {
				DataSourceInfo selectedInfo = SelectedDataSourceInfo;
				return selectedInfo != null ? selectedInfo.DataMember : null;
			}
			set {
				DataSourceInfo selectedInfo = SelectedDataSourceInfo;
				if (selectedInfo != null)
					SelectedDataSourceInfo = new DataSourceInfo(selectedInfo.DataSource, value);
				else
					SelectedDataSourceInfo = new DataSourceInfo(null, value);
			}
		}
		[
		Browsable(false), 
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DashboardItem SelectedDashboardItem {
			get {
				IDashboardDesignerSelectionService selectionService = ServiceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>();
				return selectionService.SelectedDashboardItem;
			}
			set {
				IDashboardDesignerSelectionService selectionService = ServiceProvider.RequestServiceStrictly<IDashboardDesignerSelectionService>();
				selectionService.SelectedDashboardItem = value;
			}
		}
		[
		Obsolete("This property is now obsolete. Use the ActionOnClose property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ConfirmSaveOnClose {
			get { return actionOnClose == DashboardActionOnClose.Prompt; }
			set { actionOnClose = value ? DashboardActionOnClose.Prompt : DashboardActionOnClose.Discard; }
		}
		[
		DefaultValue(DashboardActionOnClose.Prompt)
		]
		public DashboardActionOnClose ActionOnClose {
			get { return actionOnClose; }
			set { actionOnClose = value; }
		}
		[
		DefaultValue(DashboardViewer.DefaultPrintPreviewType)
		]
		public DashboardPrintPreviewType PrintPreviewType {
			get { return dashboardViewer.PrintPreviewType; }
			set { dashboardViewer.PrintPreviewType = value; }
		}
		[
		Browsable(false)
		]
		public UserAction UserAction { get { return dashboardViewer.UserAction; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DashboardDataSourceWizardSettings DataSourceWizardSettings { get; private set; }
		[
		DefaultValue(null)
		]
		public IDBSchemaProvider CustomDBSchemaProvider { get; set; }
		[
		Category(CategoryNames.Behavior),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DashboardPrintingOptions PrintingOptions { get { return dashboardViewer.PrintingOptions; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public IDashboardDataSourceWizardCustomization DataSourceWizardCustomization {
			get {
				return ServiceProvider.RequestService<IDashboardDataSourceWizardCustomization>();
			}
			set {
				if(value != null) {
					if(ServiceContainer.GetService<IDashboardDataSourceWizardCustomization>() == null)
						ServiceContainer.AddService<IDashboardDataSourceWizardCustomization>(value);
					else
						ServiceContainer.ReplaceService<IDashboardDataSourceWizardCustomization>(value);
				} else
					ServiceContainer.RemoveService<IDashboardDataSourceWizardCustomization>();
			}
		}
		public event DashboardLoadedEventHandler DashboardLoaded;
		public event EventHandler DashboardChanged;
		public event DashboardConfigureDataConnectionEventHandler ConfigureDataConnection;
		public event CustomFilterExpressionEventHandler CustomFilterExpression;
		public event CustomParametersEventHandler CustomParameters;
		public event DataLoadingEventHandler DataLoading;
		public event DashboardCreatingEventHandler DashboardCreating;
		public event DashboardSavingEventHandler DashboardSaving;
		public event DashboardSavedEventHandler DashboardSaved;
		public event DashboardOpeningEventHandler DashboardOpening;
		public event DashboardClosingEventHandler DashboardClosing;
		public event DashboardConnectionErrorEventHandler ConnectionError;
		public event MasterFilterSetEventHandler MasterFilterSet;
		public event MasterFilterClearedEventHandler MasterFilterCleared;
		public event SingleFilterDefaultValueEventHandler SingleFilterDefaultValue;
		public event DrillActionEventHandler DrillDownPerformed;
		public event DrillActionEventHandler DrillUpPerformed;
		public event DashboardBeforeExportEventHandler BeforeExport;
		public event DashboardPopupMenuShowingEventHandler PopupMenuShowing;
		public event ValidateDashboardCustomSqlQueryEventHandler ValidateCustomSqlQuery;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public event DashboardItemControlUpdatedEventHandler DashboardItemControlUpdated;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public event DashboardItemControlCreatedEventHandler DashboardItemControlCreated;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public event DashboardItemBeforeControlDisposedEventHandler DashboardItemBeforeControlDisposed;
		void RaiseDashboardItemBeforeControlDisposed(object sender, DashboardItemControlEventArgs e) {
			if(DashboardItemBeforeControlDisposed != null)
				DashboardItemBeforeControlDisposed(this, e);
		}
		void RaiseDashboardItemControlCreated(object sender, DashboardItemControlEventArgs e) {
			if(DashboardItemControlCreated != null)
				DashboardItemControlCreated(this, e);
		}
		void RaiseDashboardItemControlUpdated(object sender, DashboardItemControlEventArgs e) {
			if(DashboardItemControlUpdated != null)
				DashboardItemControlUpdated(this, e);
		}
		internal void RaiseValidateCustomSqlQuery(object sender, ValidateDashboardCustomSqlQueryEventArgs e) {
			if (ValidateCustomSqlQuery != null)
				ValidateCustomSqlQuery(this, e);
		}
		void RaiseDashboardChanged(object sender, EventArgs e) {
			if (DashboardChanged != null)
				DashboardChanged(this, e);
		}
		void RaiseDashboardLoaded(object sender, DashboardLoadedEventArgs e) {
			if (DashboardLoaded != null)
				DashboardLoaded(this, e);
		}
		void RaiseDataLoading(object sender, DataLoadingEventArgs e) {
			if (DataLoading != null)
				DataLoading(this, e);
		}
		void RaiseConnectionError(object sender, DashboardConnectionErrorEventArgs e) {
			if (ConnectionError != null)
				ConnectionError(this, e);
		}
		void RaiseCustomFilterExpression(object sender, CustomFilterExpressionEventArgs e) {
			if (CustomFilterExpression != null)
				CustomFilterExpression(this, e);
		}
		void RaiseCustomParameters(object sender, CustomParametersEventArgs e) {
			if (CustomParameters != null)
				CustomParameters(this, e);
		}
		internal void RaisePopupMenuShowing(DashboardPopupMenuShowingEventArgs args) {
			if (PopupMenuShowing != null)
				PopupMenuShowing(this, args);
		}
		void RaiseConfigureDataConnection(object sender, DashboardConfigureDataConnectionEventArgs e) {
			if (ConfigureDataConnection != null)
				ConfigureDataConnection(this, e);
		}
		void RaiseMasterFilterSet(object sender, MasterFilterSetEventArgs e) {
			if (MasterFilterSet != null)
				MasterFilterSet(sender, e);
		}
		void RaiseMasterFilterCleared(object sender, MasterFilterClearedEventArgs e) {
			if (MasterFilterCleared != null)
				MasterFilterCleared(sender, e);
		}
		void RaiseSingleFilterDefaultValue(object sender, SingleFilterDefaultValueEventArgs e) {
			if (SingleFilterDefaultValue != null)
				SingleFilterDefaultValue(sender, e);
		}
		void RaiseDrillDownPerformed(object sender, DrillActionEventArgs e) {
			if (DrillDownPerformed != null)
				DrillDownPerformed(sender, e);
		}
		void RaiseDrillUpPerformed(object sender, DrillActionEventArgs e) {
			if (DrillUpPerformed != null)
				DrillUpPerformed(sender, e);
		}
		void RaiseBeforeExport(object sender, DashboardBeforeExportEventArgs eventArgs) {
			if (BeforeExport != null)
				BeforeExport(this, eventArgs);
		}		
		public bool HandleDashboardClosing() {
			DashboardClosingEventArgs eventArgs = new DashboardClosingEventArgs(Dashboard, IsDashboardModified);
			RaiseDashboardClosing(eventArgs);
			if (eventArgs.IsDashboardModified)
				return (actionOnClose != DashboardActionOnClose.Save || ExecuteSaveDashboardCommand()) &&
					(actionOnClose != DashboardActionOnClose.Prompt || ShowSaveConfirmationDialog(eventArgs.IsDashboardModified));
			return true;
		}
		[
		Obsolete("This method is now obsolete. Use the HandleDashboardClosing method instead.")
		]
		public bool CloseDashboard() {
			return HandleDashboardClosing();
		}
		[
		Obsolete("This method is now obsolete. Use the HandleDashboardClosing method instead.")
		]
		public bool ShowSaveConfirmationDialog() {
			return ShowSaveConfirmationDialog(IsDashboardModified);
		}
		internal bool ShowSaveConfirmationDialog(bool isModified) {
			if (!isModified)
				return true;
			string dashboardName = String.IsNullOrEmpty(dashboardFileName) ?
				DashboardWinResLocalizer.GetString(DashboardWinStringId.DashboardDesignerDefaultDashboardName) : dashboardFileName;
			string question = String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerConfirmSaveMessage), dashboardName);
			string caption = DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerCaption);
			switch (XtraMessageBox.Show(LookAndFeel, this, question, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
				case DialogResult.Yes:
					return ExecuteSaveDashboardCommand();
				case DialogResult.No:
					return true;
				default:
					return false;
			}
		}
		public void LoadDashboard(string filePath) {
			if (!File.Exists(filePath))
				XtraMessageBox.Show(LookAndFeel, ParentForm, string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.MessageFileNotFound), filePath),
					DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerCaption), MessageBoxButtons.OK);
			else {
				try {
					CompleteFileOperation(filePath);
					List<IDashboardComponent> componentsToDestroy = new List<IDashboardComponent>();
					List<IDashboardComponent> componentsToAdd = new List<IDashboardComponent>();
					Dashboard dashboard = Dashboard;
					componentsToDestroy.AddRange(dashboard.DashboardComponents);
					dashboard.LoadFromXml(filePath);
					componentsToAdd.AddRange(dashboard.DashboardComponents);
				}
				catch (DashboardInternalException) {
					throw;
				}
				catch {
					XtraMessageBox.Show(LookAndFeel, ParentForm, String.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardLoadingError), filePath),
						DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerCaption), MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
				finally {
					History.Clear();
					OnUpdateUI();
				}
			}
		}
		public void LoadDashboard(Stream stream) {
			Dashboard.LoadFromXml(stream);
			OnUpdateUI();
		}
		public void ReloadData() {
			ReloadData(false);
		}
		public void ReloadData(bool suppressWaitForm) {
			Viewer.ReloadData(suppressWaitForm);
		}
		public void CreateBars() {
			if (Parent == null)
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerUnableCreateBarsMessage));
			BarDockControl dockControl = FindControl(typeof(BarDockControl)) as BarDockControl;
			BarManager barManager = dockControl == null ? null : dockControl.Manager;
			CreateBars(true, barManager);
		}
		internal void CreateBars(bool visible, BarManager barManager) {
			barManagerVisible = visible;
			if (barManager == null) {
				barManager = new BarManager() { Controller = BarAndDockingController };
				barManager.Form = this;
				if (barManagerVisible)
					barManager.Form = ParentForm;
				MenuManager = barManager;
			}
			barManager.BeginUpdate();
			try {
				CreateBar(null, barManager);
				if (!barManagerVisible)
					foreach (Bar bar in barManager.Bars)
						bar.Visible = false;
			}
			finally {
				barManager.EndUpdate();
			}
		}
		public void CreateRibbon() {
			Control parent = Parent;
			if (parent == null)
				throw new InvalidOperationException(DashboardWinLocalizer.GetString(DashboardWinStringId.DashboardDesignerUnableCreateRibbonMessage));
			RibbonForm ribbonForm = parent as RibbonForm;
			RibbonControl ribbon = ribbonForm != null ? ribbonForm.Ribbon : null;
			if (ribbon == null) {
				ribbon = FindControl(typeof(RibbonControl)) as RibbonControl;
				if (ribbon == null) {
					ribbon = new RibbonControl();
					ribbon.ShowCustomizationMenu += OnRibbonShowCustomizationMenu;
					ribbonControl = ribbon;
					ribbon.BeginInit();
					ribbonViewControl = new DashboardBackstageViewControl();
					ISupportLookAndFeel lookAndFeelControl = parent as ISupportLookAndFeel;
					if (lookAndFeelControl != null) {
						dashboardBarAndDockingController.LookAndFeel.ParentLookAndFeel = lookAndFeelControl.LookAndFeel;
						ribbonViewControl.Controller = ribbon.Controller = dashboardBarAndDockingController;
					}
					DashboardBarsUtils.SetupRibbon(ribbon, ribbonViewControl, this);
					parent.Controls.Add(ribbon);
					ribbon.EndInit();
				}
				if (ribbonForm != null)
					ribbonForm.Ribbon = ribbon;
			}
			MenuManager = ribbon;
			ribbonControl = ribbon;
			CreateBar(ribbon, null);
			RibbonPage page = DashboardBarsUtils.FindPage(ribbon, typeof(HomeRibbonPage));
			if (page != null)
				ribbon.SelectedPage = page;
			RefreshRibbonPages(SelectedDashboardItem);
		}
		[
		Obsolete("The DevExpress.DashboardWin.DashboardDesigner.GetAvailableSelections method is now obsolete. Use the DevExpress.DashboardWin.DashboardDesigner.GetAvailableDrillDownValues and DevExpress.DashboardWin.DashboardDesigner.GetAvailableFilterValues methods instead.")
		]
		public DashboardDataSet GetAvailableSelections(string dashboardItemName) {
			return null;
		}
		public IList<AxisPointTuple> GetAvailableDrillDownValues(string dashboardItemName) {
			return Viewer.GetAvailableDrillDownValues(dashboardItemName);
		}
		public IList<AxisPointTuple> GetAvailableFilterValues(string dashboardItemName) {
			return Viewer.GetAvailableFilterValues(dashboardItemName);
		}
		public void SetMasterFilter(string dashboardItemName, IList<DashboardDataRow> values) {
			Viewer.SetMasterFilter(dashboardItemName, values);
		}
		public void SetMasterFilter(string dashboardItemName, object values) {
			Viewer.SetMasterFilter(dashboardItemName, values);
		}
		public void ClearMasterFilter(string dashboardItemName) {
			Viewer.ClearMasterFilter(dashboardItemName);
		}
		public void PerformDrillDown(string dashboardItemName, object value) {
			Viewer.PerformDrillDown(dashboardItemName, value);
		}
		public void PerformDrillDown(string dashboardItemName, DashboardDataRow value) {
			Viewer.PerformDrillDown(dashboardItemName, value);
		}
		public void PerformDrillUp(string dashboardItemName) {
			Viewer.PerformDrillUp(dashboardItemName);
		}
		public MultiDimensionalData GetItemData(string dashboardItemName) {
			return Viewer.GetItemData(dashboardItemName);
		}
		public DashboardUnderlyingDataSet GetUnderlyingData(string dashboardItemName) {
			return Viewer.GetUnderlyingData(dashboardItemName);
		}
		public RangeFilterSelection GetCurrentRange(string dashboardItemName) {
			return Viewer.GetCurrentRange(dashboardItemName);
		}
		public RangeFilterSelection GetEntireRange(string dashboardItemName) {
			return Viewer.GetEntireRange(dashboardItemName);
		}
		public void SetRange(string dashboardItemName, RangeFilterSelection range) {
			Viewer.SetRange(dashboardItemName, range);
		}
		public DashboardDataSet GetCurrentSelection(string dashboardItemName) {
			return Viewer.GetCurrentSelection(dashboardItemName);
		}
		public bool CanSetMultiValueMasterFilter(string dashboardItemName) {
			return Viewer.CanSetMultiValueMasterFilter(dashboardItemName);
		}
		public bool CanSetMasterFilter(string dashboardItemName) {
			return Viewer.CanSetMasterFilter(dashboardItemName);
		}
		public bool CanClearMasterFilter(string dashboardItemName) {
			return Viewer.CanClearMasterFilter(dashboardItemName);
		}
		public bool CanPerformDrillDown(string dashboardItemName) {
			return Viewer.CanPerformDrillDown(dashboardItemName);
		}
		public bool CanPerformDrillUp(string dashboardItemName) {
			return Viewer.CanPerformDrillUp(dashboardItemName);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			Viewer.ExportToPdf(stream, options);
		}
		public void ExportToPdf(Stream stream) {
			Viewer.ExportToPdf(stream);
		}
		public void ExportToImage(Stream stream, ImageExportOptions options) {
			Viewer.ExportToImage(stream, options);
		}
		public void ExportToImage(Stream stream) {
			Viewer.ExportToImage(stream);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			Viewer.ExportToPdf(filePath, options);
		}
		public void ExportToPdf(string filePath) {
			Viewer.ExportToPdf(filePath);
		}
		public void ExportToImage(string filePath, ImageExportOptions options) {
			Viewer.ExportToImage(filePath, options);
		}
		public void ExportToImage(string filePath) {
			Viewer.ExportToImage(filePath);
		}
		IEnumerable<Control> IUnderlyingControlProvider.GetUnderlyingControls() {
			return ((IUnderlyingControlProvider)Viewer).GetUnderlyingControls();
		}
		Control IUnderlyingControlProvider.GetUnderlyingControl(DashboardItem dashboardItem) {
			return ((IUnderlyingControlProvider)Viewer).GetUnderlyingControl(dashboardItem);
		}
		Control IUnderlyingControlProvider.GetUnderlyingControl(string componentName) {
			return ((IUnderlyingControlProvider)Viewer).GetUnderlyingControl(componentName);
		}
	}
	public enum DashboardActionOnClose { 
		Prompt, 
		Save, 
		Discard 
	}
}
