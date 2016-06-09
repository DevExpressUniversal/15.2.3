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
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Persistent;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Data;
using DevExpress.XtraVerticalGrid.Data;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraVerticalGrid.Localization;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraVerticalGrid.Painters;
using DevExpress.XtraVerticalGrid.Blending;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;
using DevExpress.XtraEditors.Design;
using System.Collections.Generic;
namespace DevExpress.XtraVerticalGrid {
	[
	Designer("DevExpress.XtraVerticalGrid.Design.VerticalGridDesigner, " + AssemblyInfo.SRAssemblyVertGridDesign, typeof(System.ComponentModel.Design.IDesigner)),
	DefaultProperty("Rows"), DXToolboxItem(true),
	ToolboxTabName(AssemblyInfo.DXTabData),
	Description("Represents data from a data source in one of grid formats, in which rows represent dataset fields and columns represent dataset records.")
]
	[DevExpress.Utils.Design.DataAccess.DataAccessMetadata("All", SupportedProcessingModes = "Simple")]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "VGridControl")]
	public class VGridControl : VGridControlBase, INavigatableControl, IDataControllerVisualClient, IDataControllerValidationSupport, IDataControllerCurrentSupport, IDataControllerData2, IBoundControl, IDataControllerSort {
		NavigatableControlHelper navigationHelper;
		object dataSource;
		string dataMember;
		UnboundColumnInfo nullableColumn;
		readonly UnboundRowPropertiesCache unboundRowPropertiesCache;
		INavigatableControl navigatableState;
		public VGridControl() {
			this.navigationHelper = new NavigatableControlHelper();
			this.dataSource = null;
			this.dataMember = string.Empty;
			this.unboundRowPropertiesCache = new UnboundRowPropertiesCache();
			this.navigatableState = CreateNavigatable();
		}
		public virtual void ShowUnboundExpressionEditor(RowProperties properties) {
			using(ExpressionEditorForm form = new UnboundColumnExpressionEditorForm(properties, null)) {
				form.StartPosition = FormStartPosition.CenterParent;
				if(!RaiseUnboundExpressionEditorCreated(form, properties))
					return;
				if(GetFormResult(form) == DialogResult.OK)
					properties.UnboundExpression = form.Expression;
			}
		}
		protected override VGridDataManager CreateDataManager() {
			var dataManager = base.CreateDataManager();
			dataManager.VisualClient = this;
			dataManager.DataClient = this;
			dataManager.SortClient = this;
			dataManager.ValidationClient = this;
			dataManager.CurrentClient = this;
			dataManager.ListSourceChanged += new EventHandler(OnDataManager_DataSourceChanged);
			return dataManager;
		}
		protected override VGridMenuBase CreateMenu() {
			return new VGridMenu(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.navigatableState = new NavigatableDisposedState();
				var dataManager = DataManager;
				if(dataManager != null) {
					dataManager.ListSourceChanged -= OnDataManager_DataSourceChanged;
					dataManager.Dispose();
					DataManager = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override void ActivateDataSourceInternal() {
			DataView view = DataSource as DataView;
			if(view != null && view.Table == null)
				return;
			BeginDataUpdate();
			RemoveDataSetEvents();
			DataManager.SetGridDataSource(BindingContext, DataSource, DataMember);
			AddDataSetEvents();
			EndDataUpdate();
		}
		protected override bool IsDataSourceInitialized() {
			ISupportInitializeNotification dataSource = DataSource as ISupportInitializeNotification;
			if (dataSource != null && !dataSource.IsInitialized) {
				return false;
			}
			return true;
		}
		protected override void SubscribeDataSourceInitializedEvent() {
			ISupportInitializeNotification dataSource = DataSource as ISupportInitializeNotification;
			if (dataSource == null)
				return;
			dataSource.Initialized += DataSourceInitialized;
		}
		protected override void UnsubscribeDataSourceInitializedEvent(object dataSource) {
			ISupportInitializeNotification supportInitializeNotificationSource = dataSource as ISupportInitializeNotification;
			if (supportInitializeNotificationSource == null)
				return;
			supportInitializeNotificationSource.Initialized -= DataSourceInitialized;
		}
		void DataSourceInitialized(object sender, EventArgs e) {
			UnsubscribeDataSourceInitializedEvent(sender);
			if (object.Equals(sender, DataSource)) {
				ActivateDataSource();
			}
		}
		bool IsValidDataSource(object dataSource) {
			if(dataSource == null) return true;
			if(dataSource is IList) return true;
			if(dataSource is IListSource) return true;
			if(dataSource is DataSet) return true;
			if(dataSource is System.Data.DataView) {
				System.Data.DataView dv = dataSource as System.Data.DataView;
				if(dv.Table == null) return false;
				return true;
			}
			if(dataSource is System.Data.DataTable) return true;
			return false;
		}
		void DataSetChangedEvent(object sender, CollectionChangeEventArgs e) {
			if(e.Element is DataTable) {
				string name = (e.Element as DataTable).TableName;
				if(name == DataMember && !IsLoading)
					ActivateDataSource();
			}
		}
		void RemoveDataSetEvents() {
			DataSet data = GetDataSet();
			if(data != null)
				data.Tables.CollectionChanged -= new CollectionChangeEventHandler(DataSetChangedEvent);
		}
		void AddDataSetEvents() {
			DataSet data = GetDataSet();
			if(data != null) {
				data.Tables.CollectionChanged += new CollectionChangeEventHandler(DataSetChangedEvent);
			}
		}
		DataSet GetDataSet() {
			object value = DataSource;
			if(!(value is DataSet) && (value is IListSource))
				value = (value as IListSource).GetList();
			if(value is DataViewManager)
				value = (value as DataViewManager).DataSet;
			return value as DataSet;
		}
		protected override void ChangeFocusedRecord(int curRecord) {
			int prevRecord = FocusedRecord;
			this.fFocusedRecord = curRecord;
			DataManager.CurrentControllerRow = FocusedRecord;
			if(curRecord != prevRecord && FocusedRecord == curRecord) {
				BeginUpdate();
				InternalMakeRecordVisible(DataModeHelper.Position, prevRecord, true);
				RaiseFocusedRecordChanged(new IndexChangedEventArgs(FocusedRecord, prevRecord));
				EndUpdate();
			}
		}
		protected override void InvalidateMakeRecordVisible(int oldIndex, int newIndex, bool invalidate) {
			RecordsUpdateInfo = new UpdateInfo { OldRecord = oldIndex, NewRecord = newIndex, InvalidateRecords = invalidate };
		}
		protected override void UpdateMakeRecordVisible() {
			if (RecordsUpdateInfo == null)
				return;
			bool fullUpdate = false;
			try {
				fullUpdate = Scroller.TrySetRecordVisible(RecordsUpdateInfo.NewRecord) || DataModeHelper.NewItemRecordMode;
			}
			finally {
				if (fullUpdate)
					InvalidateUpdate();
				else {
					if (RecordsUpdateInfo.InvalidateRecords) {
						if (RecordsUpdateInfo.NewRecord == RecordsUpdateInfo.OldRecord)
							InvalidateRow(FocusedRow);
						else
							InvalidateRecords(RecordsUpdateInfo.NewRecord, RecordsUpdateInfo.OldRecord);
					}
				}
				RecordsUpdateInfo = null;
			}
		}
		void UpdateNavigator() {
			if(IsUpdateLocked || (DataManager == null || DataManager.IsUpdateLocked) || this.navigationHelper == null) return;
			this.navigationHelper.UpdateButtons();
		}
		protected override void OnBindingContextChanged(EventArgs e) {
			if(DataSource != null) {
				ActivateDataSource();
			}
			base.OnBindingContextChanged(e);
		}
		protected virtual void OnDataManager_DataSourceChanged(object sender, EventArgs e) {
			if(GridDisposing) return;
			if(IsHandleCreated)
				OnLoadedCore();
			this.fFocusedRecordModified = false;
			UpdateNavigator();
			OnDataManager_Reset(DataManager, EventArgs.Empty);
			RaiseDataSourceChanged();
		}
		protected virtual INavigatableControl CreateNavigatable() {
			return new NavigatableNormalState(this);
		}
		public virtual void RefreshDataSource() {
			if(IsLoading)
				return;
			DataManager.RefreshData();
		}
		public override void LayoutChangedCore() {
			base.LayoutChangedCore();
			UpdateNavigator();
		}
		protected internal override void RaiseStateChanged() {
			base.RaiseStateChanged();
			UpdateNavigator();
		}
		#region INavigatableControl
		protected INavigatableControl NavigatableState { get { return navigatableState; } }
		internal protected NavigatableControlHelper NavigationHelper { get { return navigationHelper; } }
		int INavigatableControl.RecordCount { get { return NavigatableState.RecordCount; } }
		int INavigatableControl.Position { get { return NavigatableState.Position; } }
		void INavigatableControl.AddNavigator(INavigatorOwner owner) { NavigatableState.AddNavigator(owner); }
		void INavigatableControl.RemoveNavigator(INavigatorOwner owner) { NavigatableState.RemoveNavigator(owner); }
		bool INavigatableControl.IsActionEnabled(NavigatorButtonType type) { return NavigatableState.IsActionEnabled(type); }
		void INavigatableControl.DoAction(NavigatorButtonType type) {
			try {
				EditorHelper.BeginAllowHideException();
				NavigatableState.DoAction(type);
			} catch(HideException) { } finally {
				EditorHelper.EndAllowHideException();
				UpdateNavigator();
			}
		}
		#endregion INavigatableControl
		#region IDataController
		void IDataControllerVisualClient.RequireSynchronization(IDataSync dataSync) {}
		void IDataControllerVisualClient.ColumnsRenewed() { }
		int IDataControllerVisualClient.VisibleRowCount { get { return -1; } }
		int IDataControllerVisualClient.TopRowIndex { get { return LeftVisibleRecord; } }
		int IDataControllerVisualClient.PageRowCount { get { return ViewInfo.VisibleValuesCount; } }
		void IDataControllerVisualClient.UpdateLayout() {
			InvalidateUpdate();
		}
		void IDataControllerVisualClient.UpdateRow(int controllerRowHandle) {
			InvalidateRecord(controllerRowHandle);
		}
		void IDataControllerVisualClient.UpdateRowIndexes(int controllerRowHandle) {
			InvalidateUpdate();
		}
		void IDataControllerVisualClient.UpdateRows(int topRowIndexDelta) {
			InvalidateUpdate();
		}
		void IDataControllerVisualClient.UpdateScrollBar() {
			UpdateScrollBar();
		}
		void IDataControllerVisualClient.UpdateColumns() { }
		void IDataControllerVisualClient.UpdateTotalSummary() { }
		void IDataControllerVisualClient.RequestSynchronization() { }
		bool IDataControllerVisualClient.IsInitializing { get { return false; } }
		IBoundControl IDataControllerValidationSupport.BoundControl { get { return this; } }
		void IDataControllerValidationSupport.OnCurrentRowUpdated(ControllerRowEventArgs e) {
			RaiseRecordUpdated(e);
			UpdateNavigator();
		}
		void IDataControllerValidationSupport.OnStartNewItemRow() {
			DataModeHelper.StartNewItemRow();
			Scroller.Update();
			FocusedRecord = DataModeHelper.NewItemRecord;
			if(FocusedRecord == DataModeHelper.NewItemRecord) {
				DataModeHelper.BeginCurrentRowEdit();
			}
			RaiseInitNewRecord(DataModeHelper.NewItemRecord);
		}
		void IDataControllerValidationSupport.OnEndNewItemRow() {
			DataModeHelper.EndNewItemRow();
			ChangeFocusedRecord(RecordCount - 1);
		}
		void IDataControllerValidationSupport.OnBeginCurrentRowEdit() { }
		void IDataControllerValidationSupport.OnValidatingCurrentRow(ValidateControllerRowEventArgs e) {
			if(e.RowHandle != FocusedRecord) return;
			ValidateRecordEventArgs rv = new ValidateRecordEventArgs(e.RowHandle);
			rv.Valid = e.Valid;
			rv.ErrorText = e.ErrorText;
			RaiseValidateRecord(rv);
			if(!rv.Valid) throw new WarningException(rv.ErrorText);
		}
		void IDataControllerValidationSupport.OnPostRowException(ControllerRowExceptionEventArgs e) {
			try {
				InvalidRecordExceptionEventArgs ex = new InvalidRecordExceptionEventArgs(e.Exception, e.Exception.Message + VGridLocalizer.Active.GetLocalizedString(VGridStringId.InvalidRecordExceptionText), e.RowHandle);
				RaiseInvalidRecordException(ex);
				if(ex.ExceptionMode == ExceptionMode.Ignore) {
					InvalidateRecord(FocusedRecordDisplayIndex);
					e.Action = ExceptionAction.CancelAction;
					return;
				}
				e.Action = ExceptionAction.RetryAction;
			} catch(HideException) {
				e.Action = ExceptionAction.CancelAction;
			}
		}
		void IDataControllerValidationSupport.OnPostCellException(ControllerRowCellExceptionEventArgs e) {
			throw e.Exception;
		}
		void IDataControllerCurrentSupport.OnCurrentControllerRowObjectChanged(CurrentRowChangedEventArgs e) {
		}
		void IDataControllerCurrentSupport.OnCurrentControllerRowChanged(CurrentRowEventArgs e) {
			HideEditor();
			FocusedRecord = DataManager.CurrentControllerRow;
		}
		void IDataControllerValidationSupport.OnControllerItemChanged(ListChangedEventArgs e) {
			if(OptionsBehavior.AutoFocusNewRecord && e.ListChangedType == ListChangedType.ItemAdded) {
				if(IsValidRowHandle(e.NewIndex)) {
					FocusedRecord = e.NewIndex;
				}
			}
			if(e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted) {
				ClearAutoHeights();
				ClearRowErrors();
				DataManager.SyncCurrentRow();
			}
		}
		protected internal virtual bool IsValidRowHandle(int rowHandle) {
			bool res = DataManager.IsValidControllerRowHandle(rowHandle);
			if(res)
				return true;
			if(rowHandle == CurrencyDataController.NewItemRow) {
				if(DataManager.IsNewItemRowEditing)
					return true;
			}
			return false;
		}
		#endregion IDataController
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlCustomUnboundData"),
#endif
 Category("Data")]
		public event CustomDataEventHandler CustomUnboundData {
			add { Events.AddHandler(GS.customUnboundData, value); }
			remove { Events.RemoveHandler(GS.customUnboundData, value); }
		}
		protected virtual void RaiseCustomUnboundData(CustomDataEventArgs e) {
			CustomDataEventHandler handler = (CustomDataEventHandler)this.Events[GS.customUnboundData];
			if(handler != null) handler(this, e);
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlUnboundExpressionEditorCreated"),
#endif
 Category("Behavior")]
		public event UnboundExpressionEditorEventHandler UnboundExpressionEditorCreated {
			add { Events.AddHandler(GS.unboundExpressionEditorCreated, value); }
			remove { Events.RemoveHandler(GS.unboundExpressionEditorCreated, value); }
		}
		protected virtual bool RaiseUnboundExpressionEditorCreated(ExpressionEditorForm form, RowProperties properties) {
			UnboundExpressionEditorEventHandler handler = (UnboundExpressionEditorEventHandler)this.Events[GS.unboundExpressionEditorCreated];
			if(handler != null) {
				UnboundExpressionEditorEventArgs eventArgs = new UnboundExpressionEditorEventArgs(form, properties);
				handler(this, eventArgs);
				return eventArgs.ShowExpressionEditor;
			}
			return true;
		}
		[Category("Data")]
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("VGridControlRecordUpdated")]
#endif
		public event RecordObjectEventHandler RecordUpdated {
			add { Events.AddHandler(GS.recordUpdated, value); }
			remove { Events.RemoveHandler(GS.recordUpdated, value); }
		}
		protected virtual void RaiseRecordUpdated(ControllerRowEventArgs e) {
			RecordObjectEventHandler handler = (RecordObjectEventHandler)this.Events[GS.recordUpdated];
			if(handler != null)
				handler(this, new RecordObjectEventArgs(e.RowHandle, e.Row));
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlLayoutStyle"),
#endif
 Category("Layout"), Localizable(true),
		DefaultValue(LayoutViewStyle.MultiRecordView), XtraSerializableProperty()]
		public new LayoutViewStyle LayoutStyle {
			get { return base.LayoutStyle; }
			set {
				base.LayoutStyle = value;
			}
		}
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlDataSource"),
#endif
 Category("Data"),
#if DXWhidbey
 AttributeProvider(typeof(IListSource)),
#else
		TypeConverter("System.Windows.Forms.Design.DataSourceConverter, System.Design"),
#endif
		DefaultValue(null)]
		public object DataSource {
			get { return dataSource; }
			set {
				if(value == DataSource) return;
				if(value != null && DataSource != null && DataSource.Equals(value)) return;
				if(IsValidDataSource(value)) {
					RemoveDataSetEvents();
					dataSource = value;
					ActivateDataSource();
				}
			}
		}
		bool ShouldSerializeDataMember() { return DataMember != string.Empty; }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("VGridControlDataMember"),
#endif
 Category("Data"), Localizable(true),
		Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public string DataMember {
			get { return dataMember; }
			set {
				if(DataMember == value) return;
				dataMember = value;
				ActivateDataSource();
			}
		}
		#region IDataControllerData2 Members
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter {
			get { return false; }
		}
		bool IDataControllerData2.CanUseFastProperties {
			get { return !DesignMode; }
		}
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) {
			return null;
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			if(Rows == null || collection == null)
				return collection;
			VGridRows rows = Rows;
			DelegateRowPropertiesOperation findDescriptorsOperation = new DelegateRowPropertiesOperation(properties => {
				if (properties.UnboundType == UnboundColumnType.Bound && !string.IsNullOrEmpty(properties.FieldName) && collection.Find(properties.FieldName, false) == null)
						collection.Find(properties.FieldName, true);
			});
			RowsIterator.DoOperation(findDescriptorsOperation);
			return collection;
		}
		ComplexColumnInfoCollection complexColumns;
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			complexColumns = new ComplexColumnInfoCollection();
			RowsIterator.DoLocalOperation(new DelegateRowPropertiesOperation(GetComplexColumn), Rows);
			return complexColumns;
		}
		void GetComplexColumn(RowProperties properties) {
			if(properties == null || properties.UnboundType != UnboundColumnType.Bound)
				return;
			if(properties.FieldName.Contains(".") && DataManager.Columns[properties.FieldName] == null) {
				complexColumns.Add(properties.FieldName);
			}
		}
		#endregion
		#region IDataControllerData Members
		protected UnboundColumnInfo NullableColumn { 
			get {
				if(nullableColumn == null)
					nullableColumn = new UnboundColumnInfo("VerticalGridNullableColumn", UnboundColumnType.Object, true);
				return nullableColumn; 
			} 
		}
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			CreateUnboundColumnInfoesOperation operation = new CreateUnboundColumnInfoesOperation(unboundRowPropertiesCache);
			RowsIterator.DoOperation(operation);
			return operation.UnboundColumns;
		}
		UnboundRowPropertiesCache CachedUnboundRowProperties { get { return unboundRowPropertiesCache; } }
		object IDataControllerData.GetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			RowProperties rowProperties = this.unboundRowPropertiesCache.GetProperties(column.Name);
			CustomDataEventArgs e = new CustomDataEventArgs(rowProperties.Row, listSourceRow, value, true, rowProperties);
			RaiseCustomUnboundData(e);
			return e.Value;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			RowProperties rowProperties = unboundRowPropertiesCache.GetProperties(column.Name);
			CustomDataEventArgs e = new CustomDataEventArgs(rowProperties.Row, listSourceRow, value, false, rowProperties);
			RaiseCustomUnboundData(e);
		}
		#endregion
		void IDataControllerSort.AfterGrouping() {
			throw new NotImplementedException();
		}
		void IDataControllerSort.AfterSorting() {
			throw new NotImplementedException();
		}
		void IDataControllerSort.BeforeGrouping() {
			throw new NotImplementedException();
		}
		void IDataControllerSort.BeforeSorting() {
			throw new NotImplementedException();
		}
		string IDataControllerSort.GetDisplayText(int listSourceRow, DataColumnInfo info, object value, string columnName) {
			RowProperties properties = GetRowByFieldName(info.Name).Properties;
			RepositoryItem item = GetRowEdit(properties, listSourceRow);
			if (item != null) {
				BaseEditViewInfo editViewInfo = item.CreateViewInfo();
				if (editViewInfo != null) {
					editViewInfo.InplaceType = InplaceType.Grid;
					if (!properties.Format.IsEmpty)
						editViewInfo.Format = properties.Format;
					editViewInfo.EditValue = value;
					return editViewInfo.DisplayText;
				}
			}
			return null;
		}
		string[] IDataControllerSort.GetFindByPropertyNames() {
			if (!FindPanelOwner.IsFindFilterActive)
				return new string[0];
			return FindPanelOwner.GetFindColumnNames().Select(p => p.FieldName).ToArray();
		}
		bool IDataControllerSort.RequireDisplayText(DataColumnInfo column) {
			return column.Name.StartsWith(DevExpress.Data.Filtering.DxFtsContainsHelper.DxFtsPropertyPrefix);
		}
		bool? IDataControllerSort.IsEqualGroupValues(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo column) {
			throw new NotSupportedException();
		}
		DevExpress.Data.Helpers.ExpressiveSortInfo.Row IDataControllerSort.GetCompareRowsMethodInfo() {
			throw new NotSupportedException();
		}
		DevExpress.Data.Helpers.ExpressiveSortInfo.Cell IDataControllerSort.GetSortCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order) {
			throw new NotSupportedException();
		}
		DevExpress.Data.Helpers.ExpressiveSortInfo.Cell IDataControllerSort.GetSortGroupCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType) {
			throw new NotSupportedException();
		}
		void IDataControllerSort.SubstituteSortInfo(SubstituteSortInfoEventArgs args) { }
	}	
	class UnboundRowPropertiesCache {
		Hashtable cache;
		public UnboundRowPropertiesCache() {
			this.cache = new Hashtable();
		}
		public RowProperties GetProperties(string unboundFieldName) {
			return this.cache[unboundFieldName] as RowProperties;
		}
		public void AddProperties(string unboundFieldName, RowProperties properties) {
			this.cache[unboundFieldName] = properties;
		}
		public void Clear() {
			this.cache.Clear();
		}
	}
	class NavigatableDisposedState : INavigatableControl {
		#region INavigatableControl Members
		public int Position { get { return 0; } }
		public int RecordCount { get { return 0; } }
		public void AddNavigator(INavigatorOwner owner) { }
		public void DoAction(NavigatorButtonType type) { }
		public bool IsActionEnabled(NavigatorButtonType type) { return false; }
		public void RemoveNavigator(INavigatorOwner owner) { }
		#endregion
	}
	class NavigatableNormalState : INavigatableControl {
		VGridControl vGrid;
		public NavigatableNormalState(VGridControl vGrid) {
			this.vGrid = vGrid;
		}
		public VGridControl Grid { get { return vGrid; } }
		#region INavigatableControl Members
		public int Position { get { return Grid.FocusedRecord; } }
		public int RecordCount { get { return Grid.RecordCount; } }
		public void AddNavigator(INavigatorOwner owner) { Grid.NavigationHelper.AddNavigator(owner); }
		public void DoAction(NavigatorButtonType type) {
			switch(type) {
				case NavigatorButtonType.First:
					Grid.FocusedRecord = 0;
					break;
				case NavigatorButtonType.Last:
					Grid.FocusedRecord = Grid.RecordCount - 1;
					break;
				case NavigatorButtonType.Prev:
					Grid.FocusedRecord--;
					break;
				case NavigatorButtonType.Next:
					Grid.FocusedRecord++;
					break;
				case NavigatorButtonType.PrevPage:
					Grid.FocusedRecord -= Grid.ViewInfo.VisibleValuesCount;
					break;
				case NavigatorButtonType.NextPage:
					Grid.FocusedRecord += Grid.ViewInfo.VisibleValuesCount;
					break;
				case NavigatorButtonType.Append:
					Grid.AddNewRecord();
					break;
				case NavigatorButtonType.Remove:
					Grid.DeleteRecord(Grid.FocusedRecord);
					break;
				case NavigatorButtonType.Edit:
					Grid.ShowEditor();
					break;
				case NavigatorButtonType.CancelEdit:
					OnCancelEdit();
					break;
				case NavigatorButtonType.EndEdit:
					OnEndEdit();
					break;
			}
		}
		public bool IsActionEnabled(NavigatorButtonType type) {
			switch(type) {
				case NavigatorButtonType.First:
				case NavigatorButtonType.Prev:
				case NavigatorButtonType.PrevPage:
					return (Grid.FocusedRecord != 0);
				case NavigatorButtonType.Last:
				case NavigatorButtonType.Next:
				case NavigatorButtonType.NextPage:
					return (Grid.FocusedRecord != Grid.RecordCount - 1);
				case NavigatorButtonType.Append:
					return Grid.DataManager.AllowAppend;
				case NavigatorButtonType.Remove:
					return CanRemove;
				case NavigatorButtonType.Edit:
					return CanEdit;
				case NavigatorButtonType.EndEdit:
				case NavigatorButtonType.CancelEdit:
					return CanEndEdit;
			}
			return false;
		}
		public void RemoveNavigator(INavigatorOwner owner) { Grid.NavigationHelper.RemoveNavigator(owner); }
		#endregion
		bool CanEndEdit {
			get {
				return IsEditing || Grid.FocusedRecordModified || Grid.DataModeHelper.NewItemRecordMode || Grid.DataManager.IsCurrentRowModified;
			}
		}
		bool IsEditing { get { return Grid.State == VGridState.Editing; } }
		bool CanEdit {
			get {
				return Grid.DataManager.AllowEdit && !Grid.DataManager.IsCurrentRowEditing && Grid.CanShowEditor && (Grid.FocusedRow != null && !Grid.FocusedRow.GetRowProperties(Grid.FocusedRecordCellIndex).GetReadOnly());
			}
		}
		bool CanRemove {
			get {
				if(!Grid.DataManager.AllowRemove || Grid.RecordCount == 0) return false;
				if(Grid.DataModeHelper.NewItemRecordMode || IsEditing) return false;
				return Grid.OptionsBehavior.Editable;
			}
		}
		void OnCancelEdit() {
			Grid.HideEditor();
			Grid.CancelUpdateFocusedRecord();
		}
		void OnEndEdit() {
			Grid.CloseEditor();
			Grid.UpdateFocusedRecord();
		}
	}
}
