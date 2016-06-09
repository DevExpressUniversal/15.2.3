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
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraGrid.Helpers;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Repository;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Localization;
using DevExpress.Data.Helpers;
namespace DevExpress.XtraEditors.Repository {
	public enum GridLookUpViewType { Default, GridView, BandedView, AdvBandedView }
	public abstract class RepositoryItemGridLookUpEditBase : RepositoryItemLookUpEditBase, IDataControllerThreadClient {
		int lockPropertiesChanged = 0;
		GridView view;
		GridControl grid;
		GridLookUpViewType viewType;
		PopupFilterMode popupFilterMode;
		public RepositoryItemGridLookUpEditBase() {
			this.popupFilterMode = PopupFilterMode.Default;
			this.viewType = GridLookUpViewType.Default;
			this.grid = CreateGrid();
			this.grid.dataSourceWeakReference = true;
			this.view = CreateViewInstance();
			Setup(View);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(View != null) View.Dispose();
				if(Grid != null) Grid.Dispose();
				DestroyDataController();
			}
			base.Dispose(disposing);
		}
		internal Size GetDesiredPopupFormSizeCore(bool forceMinSize) {
			return GetDesiredPopupFormSize(forceMinSize);
		}
		internal void FireChangedCore() {
			FireChanged();
		}
		[Browsable(false)]
		public override bool RequireDisplayTextSorting { get { return true; } }
		protected override void OnOwnerEditChanged() {
			base.OnOwnerEditChanged();
			Grid.UpdateRepositoryOwner();
		}
		protected internal void OnViewPropertiesChanged() {
			if(this.lockPropertiesChanged != 0 || (Grid != null && Grid.IsLoading)) return;
			try {
				IDisposable stream = PropertyStore["ViewSettingsManager"] as IDisposable;
				if(stream != null) {
					PropertyStore.Remove("ViewSettingsManager");
					stream.Dispose();
				}
			}
			catch { }
		}
		protected virtual void OnControllerListChanged(object sender, ListChangedEventArgs e) {
			ResetValuesCache(true);
		}
		protected override void OnPopupFormSizeChanged() {
			if(PropertyStore.Contains("BlobSize")) PropertyStore.Remove("BlobSize");
			if(PropertyStore.Contains(LookUpPropertyNames.PopupBestWidth)) PropertyStore.Remove(LookUpPropertyNames.PopupBestWidth);
			base.OnPopupFormSizeChanged();
		}
		protected override void OnBestFitModeChanged() {
			if(PropertyStore.Contains("BlobSize")) PropertyStore.Remove("BlobSize");
			if(PropertyStore.Contains(LookUpPropertyNames.PopupBestWidth)) PropertyStore.Remove(LookUpPropertyNames.PopupBestWidth);
			base.OnBestFitModeChanged();
		}
		internal IDictionary PropertyStoreCore { get { return PropertyStore; } }
		internal void DoClosePopup() {
			if(OwnerEdit != null) OwnerEdit.ClosePopup();
		}
		public virtual void PopulateViewColumns() {
			if(View.Columns.Count > 0 || DataSource == null) return;
			if(Controller.IsReady)
				View.PopulateColumns(Controller.Columns.ToArray());
			else
				View.PopulateColumns(DataSource);
		}
		public override void BeginInit() {
			Grid.BeginInit();
			base.BeginInit();
		}
		public override void EndInit() {
			Grid.EndInit();
			base.EndInit();
		}
		protected virtual void Setup(BaseView view) {
			Grid.BeginInit();
			try {
				Grid.SetupLookUp(this, view);
			}
			finally {
				Grid.EndInit();
			}
			Grid.ForceInitialize();
			view.AllowLayoutWithoutHandle = true;
		}
		protected virtual GridControl CreateGrid() { return new GridControl(); }
		protected internal GridControl Grid { get { return grid; } }
		[Browsable(false), 
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemGridLookUpEditBaseOwnerEdit")
#else
	Description("")
#endif
]
		public new GridLookUpEditBase OwnerEdit { get { return base.OwnerEdit as GridLookUpEditBase; } }
		[DXCategory(CategoryName.Data), 
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemGridLookUpEditBasePopupFilterMode"),
#endif
 DefaultValue(PopupFilterMode.Default)]
		public PopupFilterMode PopupFilterMode {
			get { return popupFilterMode; }
			set {
				if(PopupFilterMode == value) return;
				popupFilterMode = value;
				CheckServerMode();
				if(View != null) View.ApplyFindFilter("");
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemGridLookUpEditBaseViewType"),
#endif
 DefaultValue(GridLookUpViewType.Default)]
		public GridLookUpViewType ViewType {
			get { return viewType; }
			set {
				if(ViewType == value) return;
				viewType = value;
				OnViewTypeChanged();
			}
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("RepositoryItemGridLookUpEditBaseView"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Editor("DevExpress.XtraGrid.Design.GridLookUpViewEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(ExpandableObjectConverter))
		]
		public GridView View {
			get {
				return view;
			}
			set {
				if(View == value) return;
				if(value == null) value = CreateViewInstance();
				if(view != null) View.Dispose();
				view = value;
				OnViewChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual RepositoryItemCollection RepositoryItems { get { return Grid.RepositoryItems; } }
		protected bool GetIsServerMode() {
			return GetDCType() != BaseView.DataControllerType.Regular;
		}
		internal BaseView.DataControllerType GetDCType() {
			return CheckDCType(BaseView.DetectServerModeType(MasterDetailHelper.GetDataSource(null, DataSource, string.Empty)));
		}
		internal virtual BaseView.DataControllerType CheckDCType(BaseView.DataControllerType dataControllerType) {
			if(dataControllerType == BaseView.DataControllerType.AsyncServerMode) throw new Exception("DataSource is not supported");
			return dataControllerType;
		}
		protected internal BaseGridController Controller { get { return controller; } }
		protected virtual void CheckServerMode() {
			if(this.controller != null) {
				BaseView.DataControllerType current = BaseView.DataControllerType.Regular;
				if(Controller is AsyncServerModeDataController) current = BaseView.DataControllerType.AsyncServerMode;
				if(Controller is ServerModeDataController) current = BaseView.DataControllerType.ServerMode;
				if(current != GetDCType()) {
					DestroyDataController();
					CreateDataController();
					if(isDataSourceActivated) Controller.SetDataSource(DataSource);
					if(View != null && View.useClonedDataController != null) {
						View.useClonedDataController = null;
					}
				}
			}
		}
		bool CheckLoaded() {
			if(!IsLoading && this.controller == null) {
				OnLoaded();
				return true;
			}
			return this.controller != null;
		}
		bool isDataSourceActivated = false;
		protected internal virtual PopupFilterMode GetFilterMode() {
			if(PopupFilterMode == PopupFilterMode.Default) {
				return PopupFilterMode.Contains;
			}
			return PopupFilterMode;
		}
		protected virtual void OnViewChanged() {
			Setup(View);
			AddViewToContainer();
		}
		protected virtual void OnViewTypeChanged() {
			if(IsLoading) return;
			View = CreateViewInstance();
		}
		protected virtual GridView CreateViewInstance() {
			switch(ViewType) {
				case GridLookUpViewType.BandedView: return new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
				case GridLookUpViewType.AdvBandedView: return new DevExpress.XtraGrid.Views.BandedGrid.AdvBandedGridView();
			}
			return new GridView();
		}
		protected virtual void AssignViewType(GridLookUpViewType viewType) {
			if(ViewType == viewType) return;
			this.viewType = viewType;
			OnViewTypeChanged();
		}
		protected void AddViewToContainer() {
			if(IsLoading) return;
			IContainer container = Container;
			if(container == null && OwnerEdit != null) container = OwnerEdit.Container;
			if(container != null) {
				try {
					container.Add(View, OwnerEdit == null ? Name : OwnerEdit.Name + "View");
				}
				catch {
					try {
						container.Add(View);
					}
					catch {
					}
				}
			}
		}
		protected override void OnDataSourceChanged() {
			if(!IsLoading) ActivateDataSource();
			base.OnDataSourceChanged();
		}
		public override void ResetEvents() {
			base.ResetEvents();
			if(View != null) View.ResetEvents();
		}
		protected override void OnContainerLoaded() {
			base.OnContainerLoaded();
			CheckLoaded();
			CheckActivateDataSource();
			Grid.ForceInitialize();
		}
		protected virtual void DestroyDataController() {
			if(this.controller == null) return;
			this.controller.BeforeListChanged -= new ListChangedEventHandler(OnControllerListChanged);
			this.controller.Dispose();
			this.controller = null;
		}
		protected void CreateDataController() {
			this.isDataSourceActivated = false;
			if(this.controller != null) DestroyDataController();
			this.controller = CreateDataControllerInstance();
			this.controller.BeforeListChanged += new ListChangedEventHandler(OnControllerListChanged);
		}
		object currentDataSource = null;
		public override void Assign(RepositoryItem item) {
			RepositoryItemGridLookUpEditBase source = item as RepositoryItemGridLookUpEditBase;
			BeginUpdate();
			try {
				this.lockPropertiesChanged++;
				if(source != null) {
					this.DisplayMember = source.DisplayMember;
					this.ValueMember = source.ValueMember;
				}
				if(source == null || !object.ReferenceEquals(currentDataSource, source.DataSource)) {
					DeactivateGridDataSource();
					this.currentDataSource = source == null ? null : source.DataSource;
				}
				base.Assign(item);
				if(source == null) return;
				Grid.ResetLookUp(DataSource);
				AssignViewType(source.ViewType);
				this.view.Assign(source.View, true);
				this.popupFilterMode = source.PopupFilterMode;
				ActivateDataSource();
			}
			finally {
				this.lockPropertiesChanged--;
				EndUpdate();
			}
		}
		protected override void OnLoaded() {
			View.OnLoaded();
			CreateDataController();
			if(OwnerEdit != null) {
				if(OwnerEdit.IsHandleCreated) {
					ActivateDataSource();
				}
			}
			base.OnLoaded();
		}
		protected internal void CheckActivateDataSource() {
			if(this.isDataSourceActivated) return;
			ActivateDataSource();
		}
		protected internal virtual void DeactivateGridDataSource() {
			if(Grid.MainView != null) {
				Grid.MainView.DestroyDataController();
				Grid.MainView.useClonedDataController = null;
			}
			Grid.DataSource = null;
		}
		protected internal virtual void ActivateGridDataSource() {
			lockPropertiesChanged++;
			try {
			if(Grid.MainView != null) {
				if(Grid.MainView.useClonedDataController != Controller) {
					Grid.MainView.useClonedDataController = Controller;
					Grid.MainView.RecreateDataController();
					((GridView)Grid.MainView).OnColumnUnboundChanged(null);
					View.SynchronizeLookDataControllerSettings();
					Grid.MainView.FireDataController_DataSourceChanged();
				}
				View.UpdateColumnHandles();
			}
			if(Grid.DataSource == DataSource) return;
			if(Grid.MainView != null) {
				Grid.MainView.BeginUpdate();
				if(Grid.MainView.ViewInfo != null) Grid.MainView.ViewInfo.IsReady = false;
			}
			try {
				Grid.DataSource = null;
				Grid.BindingContext = new BindingContext();
				Grid.DataSource = DataSource;
			}
			finally {
				if(Grid.MainView != null) Grid.MainView.CancelUpdate();
			}
		}
			finally {
				lockPropertiesChanged--;
			}
		}
		protected override ExportMode GetExportMode() {
			if(ExportMode == ExportMode.Default) return ExportMode.DisplayText;
			return ExportMode;
		}
		protected virtual void ActivateDataSource() {
			if(!CheckLoaded()) return;
			this.isDataSourceActivated = true;
			CheckServerMode();
			this.controller.DataClient = View;
			this.controller.SetDataSource(DataSource);
			ResetValuesCache();
			if(IsDesignMode && View != null) {
				if(Grid.BindingContext == null) Grid.BindingContext = new BindingContext();
				if(Controller is ServerModeDataController || Controller is AsyncServerModeDataController) return;
				Grid.DataSource = DataSource;
			}
		}
		protected internal void ResetValuesCache() { ResetValuesCache(false); }
		protected internal virtual void ResetValuesCache(bool fireRefresh) {
			if(OwnerEdit != null) {
				((BaseEditViewInfo)OwnerEdit.GetViewInfo()).RefreshDisplayText = true;
				((BaseEditViewInfo)OwnerEdit.GetViewInfo()).IsReady = false;
				if(fireRefresh) OwnerEdit.RefreshCore(false);
			}
		}
		BaseGridController controller = null;
		protected virtual BaseGridController CreateDataControllerInstance() {
			switch(GetDCType()) {
				case BaseView.DataControllerType.ServerMode:
					ServerModeDataController sm = new ServerModeDataController();
					sm.KeepFocusedRowOnUpdate = false;
					return sm;
				case BaseView.DataControllerType.AsyncServerMode:
					AsyncServerModeDataController asyncController = new AsyncServerModeDataController();
					asyncController.AddThreadClient(this);
					asyncController.KeepFocusedRowOnUpdate = false;
					return asyncController;
			}
			return new GridDataController();
		}
		protected internal bool IsReady { get { return View != null && Controller != null && Controller.IsReady; } }
		protected virtual string GetDisplayTextByDisplayValue(object displayValue) {
			DataColumnInfo column = Controller.Columns[DisplayMember];
			GridDataColumnInfo gridInfo = DataHelpers.GetInfo(column) as GridDataColumnInfo;
			if(gridInfo != null) return gridInfo.GetDisplayText(DataController.InvalidRow, displayValue);
			if(displayValue == null || displayValue == DBNull.Value) return string.Empty;
			return displayValue.ToString();
		}
		GridLookUpData dataHelpers;
		protected internal GridLookUpData DataHelpers {
			get {
				if(dataHelpers == null) {
					dataHelpers = new GridLookUpData(View);
				}
				return dataHelpers;
			}
		}
		public virtual object GetDisplayValueByKeyValue(object keyValue) { return null; }
		public virtual int GetIndexByKeyValue(object keyValue) {
			int index = -1;
			try {
				index = Controller.FindRowByValue(ValueMember, keyValue);
				if(index < 0) return -1;
			} catch {
				return -1;
			}
			return index;
		}
		public object GetRowByKeyValue(object keyValue) {
			int index = GetIndexByKeyValue(keyValue);
			if(index > -1) return Controller.GetRow(index);
			return null;
		}
		#region IDataControllerThreadClient Members
		void IDataControllerThreadClient.OnTotalsReceived() {
			OnAsyncTotalsReceived();
		}
		void IDataControllerThreadClient.OnAsyncBegin() { }
		void IDataControllerThreadClient.OnAsyncEnd() { }
		void IDataControllerThreadClient.OnRowLoaded(int controllerRowHandle) {
			OnAsyncRowLoaded(controllerRowHandle);
		}
		protected virtual void OnAsyncTotalsReceived() { }
		protected virtual void OnAsyncRowLoaded(int controllerRowHandle) { }
		#endregion
	}
}
namespace DevExpress.XtraEditors {
	public abstract class GridLookUpEditBase : LookUpEditBase {
		[ DXCategory(CategoryName.Properties), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new RepositoryItemGridLookUpEditBase Properties { get { return base.Properties as RepositoryItemGridLookUpEditBase; } }
		protected internal virtual bool IsAutoComplete { get { return false; } }
		internal PopupBaseForm PopupFormCore { get { return PopupForm; } }
		protected override bool IsAcceptCloseMode(PopupCloseMode closeMode) {
			return closeMode == PopupCloseMode.Normal || closeMode == PopupCloseMode.ButtonClick;
		}
		protected override void OnPopupShown() {
			Properties.Grid.MenuManager = MenuManager;
			base.OnPopupShown();
		}
		protected override void ProcessAutoSearchNavKey(KeyEventArgs e) {
			if(Properties.TextEditStyle == TextEditStyles.Standard) return;
			base.ProcessAutoSearchNavKey(e);
		}
		protected override void OnMouseDownClosePopup() {
			ClosePopup(PopupCloseMode.Cancel);
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			Properties.CheckActivateDataSource();
		}
		public override void ShowPopup() {
			if(!IsPopupOpen) Properties.ActivateGridDataSource();
			base.ShowPopup();
		}
		protected override void Refresh(bool resetCache) {
			if(resetCache) Properties.ResetValuesCache(false);
			base.Refresh(resetCache);
		}
		internal void RefreshCore(bool resetCache) {
			Refresh(resetCache);
		}
		public virtual void ForceInitialize() {
			Properties.CheckActivateDataSource();
		}
		protected override void OnPopupClosed(PopupCloseMode closeMode) {
			base.OnPopupClosed(closeMode);
			if(Properties.View != null) Properties.View.HideCustomization();
		}
	}
}
