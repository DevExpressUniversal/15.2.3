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
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.DragDrop;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using System;
using System.Collections;
using System.Drawing;
namespace DevExpress.DashboardWin.Native {
	public class DataFieldsBrowserPresenter : IDisposable, IDragSource {
		IServiceProvider serviceProvider;
		IServiceProvider parametersSource;
		IDataFieldsBrowserView view;
		IDraggableDataFieldsBrowserView draggableView;
		DataSourceInfo dataSourceInfo;
		bool expandGroups;
		DataFieldsBrowserDisplayMode displayMode = DataFieldsBrowserDisplayMode.All;
		bool groupByType = true;
		bool sortAscending = true;
		bool sortDescending;
		IDashboardDataSource DataSource { get { return dataSourceInfo != null ? dataSourceInfo.DataSource: null; } }
		string DataMember { get { return dataSourceInfo != null ? dataSourceInfo.DataMember : null; } }
		public DataSourceInfo DataSourceInfo { 
			get { return dataSourceInfo; } 
			set {
				if(!new DataSourceInfoComparer().Equals(value,dataSourceInfo)) {
					dataSourceInfo = value;
					SetDataSourceSettingsToView();
					ClearAndBuildNodes();
				}
			} 
		}
		public IDataFieldsBrowserView View {
			get { return view; }
			set {
				if (value != view) {
					UnsubscribeViewEvents();
					view = value;
					draggableView = view as IDraggableDataFieldsBrowserView;
					SubscribeViewEvents();
					SetSettingsToView();
					SetDataSourceSettingsToView();
					ClearAndBuildNodes();
				}
			}
		}
		public bool ExpandGroups {
			get { return expandGroups; }
			set {
				if (value != expandGroups) {
					expandGroups = value;
					ClearAndBuildNodes();
				}
			}
		}
		public DataFieldsBrowserDisplayMode DisplayMode {
			get { return displayMode; }
			set {
				if (value != displayMode) {
					displayMode = value;
					if (view != null)
						view.DisplayMode = value;
					ClearAndBuildNodes();
				}
			}
		}
		public bool GroupByType {
			get { return groupByType; }
			set {
				if (value != groupByType) {
					groupByType = value;
					if(view != null)
						view.GroupByType = value;
				}
			}
		}
		public bool SortAscending {
			get { return sortAscending; }
			set {
				if (value != sortAscending) {
					sortAscending = value;
					if (view != null)
						view.SortAscending = value;
				}
			}
		}
		public bool SortDescending {
			get { return sortDescending; }
			set {
				if (value != sortDescending) {
					sortDescending = value;
					if (view != null)
						view.SortDescending = value;
				}
			}
		}
		public DataField SelectedDataField {
			get {
				if (view != null)
					return view.SelectedDataField;
				return null;
			}
		}
		public string SelectedDataMember {
			get {
				if (view != null) {
					DataField focusedDataField = view.FocusedDataField;
					if (focusedDataField != null)
						return focusedDataField.DataMember;
				}
				return null;
			}
			set {
				if (view != null)
					view.RestoreSelection(value);
			}
		}
		bool ExpandAll { get { return DataSource != null && DataSource is DashboardSqlDataSource; } }
		DataSourceNodeBase DataSourceNode { get { return DataSourceInfo.GetRootNode(); } }
		IDashboardDesignerHistoryService HistoryService { get { return serviceProvider.RequestServiceStrictly<IDashboardDesignerHistoryService>(); } }
		IDashboardCommandService CommandService { get { return serviceProvider.RequestServiceStrictly<IDashboardCommandService>(); } }
		bool IsDashboardInitializing {
			get {
				if (serviceProvider != null) {
					IDashboardLoadingService loadingService = serviceProvider.RequestService<IDashboardLoadingService>();
					if (loadingService != null)
						return loadingService.IsDashboardInitializing;
				}
				return false;
			}
		}
		bool AllowUpdate {
			get {
				if(serviceProvider != null) {
					IDesignerUpdateService designerUpdateService = serviceProvider.RequestService<IDesignerUpdateService>();
					if(designerUpdateService != null)
						return !designerUpdateService.Suspended;
				}
				return false;
			}
		}
		public event EventHandler<DataFieldEventArgs> DataFieldDoubleClick;
		public event EventHandler<DataFieldEventArgs> FocusedDataFieldChanged;
		public DataFieldsBrowserPresenter() { }
		public DataFieldsBrowserPresenter(IServiceProvider parametersSource) {
			this.parametersSource = parametersSource;
		}
		public void Initialize(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			if (this.serviceProvider == null) {
				this.serviceProvider = serviceProvider;
				SubscribeServiceEvents();
				IDataSourceSelectionService selectionService = serviceProvider.RequestServiceStrictly<IDataSourceSelectionService>();
				DataSourceInfo = selectionService.SelectedDataSourceInfo;
			}
			else
				throw new InvalidOperationException();			
		}
		public void Dispose() {
			UnsubscribeViewEvents();
			UnsubscribeServiceEvents();
			Dashboard dashboard = null;
			if(serviceProvider != null) {
				IDashboardOwnerService ownerService = serviceProvider.RequestService<IDashboardOwnerService>();
				if(ownerService != null)
					dashboard = ownerService.Dashboard;
			}
			UnsubscribeDashboardEvents(dashboard);
		}
		public void SelectNode(string dataMember) {
			if (view != null)
				view.SelectNode(dataMember);
		}
		void OnDashboardLoad(object sender, DashboardLoadingEventArgs e) {
			SubscribeDashboardEvents(e.Dashboard);
		}
		void OnDashboardUnload(object sender, DashboardLoadingEventArgs e) {
			UnsubscribeDashboardEvents(e.Dashboard);
		}
		void SubscribeServiceEvents() {
			IDataSourceSelectionService selectionService = serviceProvider.RequestServiceStrictly<IDataSourceSelectionService>();
			selectionService.DataSourceSelected += OnDataSourceSelected;
			IDashboardLoadingService loadingService = serviceProvider.RequestServiceStrictly<IDashboardLoadingService>();
			loadingService.DashboardLoad += OnDashboardLoad;
			loadingService.DashboardUnload += OnDashboardUnload;
		}
		void UnsubscribeServiceEvents() {
			if (serviceProvider != null) {
				IDataSourceSelectionService selectionService = serviceProvider.RequestService<IDataSourceSelectionService>();
				if (selectionService != null)
					selectionService.DataSourceSelected -= OnDataSourceSelected;
				IDashboardLoadingService loadingService = serviceProvider.RequestService<IDashboardLoadingService>();
				if(loadingService != null) {
					loadingService.DashboardLoad -= OnDashboardLoad;
					loadingService.DashboardUnload -= OnDashboardUnload;
				}
			}
		}
		void SubscribeViewEvents() {
			if (view != null) {
				view.GroupAndSortChanged += OnViewGroupAndSortChanged;
				view.RequestChildNodes += OnViewRequestChildNodes;
				view.DataFieldDoubleClick += OnViewDataFieldDoubleClick;
				view.FocusedDataFieldChanged += OnFocusedDataFieldChanged;
				view.RefreshFieldListClick += OnRefreshFieldListClick;
			}
			if (draggableView != null) {
				draggableView.DataFieldStartDrag += OnViewDataFieldStartDrag;
				draggableView.CalculatedFieldAdd += OnViewCalculatedFieldAdd;
				draggableView.DataFieldDelete += OnViewDataFieldDelete;
				draggableView.DataFieldRename += OnViewDataFieldRename;
				draggableView.DataFieldRenamed += OnViewDataFieldRenamed;
				draggableView.DataFieldAfterRenamed += OnViewDataFieldAfterRenamed;
				draggableView.DataFieldEdit += OnViewDataFieldEdit;
				draggableView.CalculatedFieldEditType += OnViewCalculatedFieldEditType;
				draggableView.DataFieldActionsAvailability += OnViewDataFieldActionsAvailability;
			}
		}
		void OnRefreshFieldListClick(object sender, EventArgs e) {
			DashboardSqlDataSource sqlDataSource = DataSource as DashboardSqlDataSource;
			if(sqlDataSource != null) {
				IRefreshFieldListService refreshFieldListService = serviceProvider.RequestServiceStrictly<IRefreshFieldListService>();
				refreshFieldListService.RefreshFieldList(sqlDataSource);
			}
		}
		void UnsubscribeViewEvents() {
			if (view != null) {
				view.GroupAndSortChanged -= OnViewGroupAndSortChanged;
				view.RequestChildNodes -= OnViewRequestChildNodes;
				view.DataFieldDoubleClick -= OnViewDataFieldDoubleClick;
				view.FocusedDataFieldChanged -= OnFocusedDataFieldChanged;
				view.RefreshFieldListClick -= OnRefreshFieldListClick;
			}
			if (draggableView != null) {
				draggableView.DataFieldStartDrag -= OnViewDataFieldStartDrag;
				draggableView.CalculatedFieldAdd -= OnViewCalculatedFieldAdd;
				draggableView.DataFieldDelete -= OnViewDataFieldDelete;
				draggableView.DataFieldRename -= OnViewDataFieldRename;
				draggableView.DataFieldRenamed -= OnViewDataFieldRenamed;
				draggableView.DataFieldAfterRenamed -= OnViewDataFieldAfterRenamed;
				draggableView.DataFieldEdit -= OnViewDataFieldEdit;
				draggableView.CalculatedFieldEditType -= OnViewCalculatedFieldEditType;
				draggableView.DataFieldActionsAvailability -= OnViewDataFieldActionsAvailability;
			}
		}
		void SubscribeDashboardEvents(Dashboard dashboard) {
			if(dashboard != null) {
				dashboard.DataSourceDataChanged += OnDataSourceDataChanged;
				dashboard.DataSourceCaptionChanged += OnDataSourceNameChanged;
				dashboard.DataSourceCalculatedFieldCollectionChanged += OnDataSourceCalculatedFieldCollectionChanged;
				dashboard.DataSourceCalculatedFieldChanged += OnDataSourceCalculatedFieldChanged;
				dashboard.DataSourceCalculatedFieldsCorrupted += OnDataSourceCalculatedFieldsCorrupted;
			}
		}
		void UnsubscribeDashboardEvents(Dashboard dashboard) {
			if(dashboard != null) {
				dashboard.DataSourceDataChanged -= OnDataSourceDataChanged;
				dashboard.DataSourceCaptionChanged -= OnDataSourceNameChanged;
				dashboard.DataSourceCalculatedFieldCollectionChanged -= OnDataSourceCalculatedFieldCollectionChanged;
				dashboard.DataSourceCalculatedFieldChanged -= OnDataSourceCalculatedFieldChanged;
				dashboard.DataSourceCalculatedFieldsCorrupted -= OnDataSourceCalculatedFieldsCorrupted;
			}
		}
		void OnViewGroupAndSortChanged(object sender, EventArgs e) {
			GetSettingsFromView();
			BuildNodes();
		}
		void OnViewRequestChildNodes(object sender, RequestChildNodesEventArgs e) {
			if (e.DataNode != null) {
				OlapHierarchyDataField olapHierarchyField = e.DataNode as OlapHierarchyDataField;
				IList childNodes = e.DataNode.ChildNodes;
				if (childNodes.Count == 0 && expandGroups && olapHierarchyField != null)
					childNodes = DataSource.GetHierarchyDataNodes(olapHierarchyField);
				e.ChildNodes = childNodes;
			}
		}
		void OnViewDataFieldDoubleClick(object sender, DataFieldEventArgs e) {
			if (DataFieldDoubleClick != null)
				DataFieldDoubleClick(this, e);
		}
		void OnViewDataFieldStartDrag(object sender, DataFieldStartDragEventArgs e) {
			IDashboardGuiContextService guiService = serviceProvider.RequestServiceStrictly<IDashboardGuiContextService>();
			using (DragAreaDrawingContext drawingContext = new DragAreaDrawingContext(guiService.LookAndFeel)) {
				IDragObject dragObject = new DataFieldDragObject(this, DataSource.GetDataSourceSchema(DataMember), e.DataField);
				Bitmap bitmap = drawingContext.GetDataItemBitmap(DataSource.GetDataSourceSchema(DataMember), dragObject, 0, 0);
				Size offset = new Size(bitmap.Width / 2, bitmap.Height / 2);
				IDashboardDesignerDragService dragService = serviceProvider.RequestServiceStrictly<IDashboardDesignerDragService>();
				dragService.StartDrag(dragObject, bitmap, e.StartScreenPt, offset);
			}
		}
		void OnViewCalculatedFieldAdd(object sender, EventArgs e) {
			CommandService.ExecuteCommand(DashboardCommandId.AddCalculatedField);
		}
		void OnViewDataFieldDelete(object sender, DataFieldEventArgs e) {
			CalculatedField cField = GetCalculatedField(e.DataField);
			if (cField != null)
				HistoryService.RedoAndAdd(new RemoveCalculatedFieldHistoryItem(cField, DataSource));
		}
		void OnViewDataFieldRename(object sender, DataFieldEventArgs e) {
			CalculatedField cField = GetCalculatedField(e.DataField);
			if (cField != null)
				draggableView.RenameSelectedDataField();
		}
		void OnViewDataFieldRenamed(object sender, DataFieldBeginRenameEventArgs e) {
			CalculatedField cField = GetCalculatedField(e.DataField);
			if (cField != null && DataSource.IsCalculatedFieldNameValid(e.NewName, DataMember))
				HistoryService.RedoAndAdd(new EditCalculatedFieldNameHistoryItem(cField, e.NewName, DataSource));
		}
		void OnViewDataFieldAfterRenamed(object sender, EventArgs e) {
			BuildNodes();
		}
		void OnViewDataFieldEdit(object sender, DataFieldEventArgs e) {
			CalculatedField cField = GetCalculatedField(e.DataField);
			if (cField != null)
				CommandService.ExecuteCommand(DashboardCommandId.EditCalculatedField, new EditCalculatedFieldCommandUIState(cField, DataSource));
		}
		void OnViewCalculatedFieldEditType(object sender, CalculatedFieldEditTypeEventArgs e) {
			CalculatedField cField = GetCalculatedField(e.DataField);
			if (cField != null)
				HistoryService.RedoAndAdd(new EditCalculatedFieldTypeHistoryItem(cField, e.NewType, DataSource));
		}
		void OnViewDataFieldActionsAvailability(object sender, DataFieldActionsAvailabilityEventArgs e) {
			if (DataSource != null && DataSource.GetIsCalculatedFieldsSupported()) {
				e.AddCalculatedField = true;
				CalculatedField cField = GetCalculatedField(e.DataField);
				if (cField != null) {
					e.EditDataField = true;
					e.EditCalculatedFieldType = true;
					e.CalculatedFieldType = cField.DataType;
					e.RenameDataField = true;
					e.DeleteDataField = true;
				}
			}
		}
		void OnFocusedDataFieldChanged(object sender, DataFieldEventArgs e) {
			if (FocusedDataFieldChanged != null)
				FocusedDataFieldChanged(this, e);
		}
		void OnDataSourceSelected(object sender, DataSourceSelectedEventArgs e) {
			DataSourceInfo = new DataSourceInfo(e.DataSource, e.DataMember);
		}
		void OnDataSourceDataChanged(object sender, DataSourceChangedEventArgs e) {
			if(!IsDashboardInitializing && AllowUpdate && DataSource == e.DataSource)
				ClearAndBuildNodes();
		}
		void OnDataSourceNameChanged(object sender, DataSourceChangedEventArgs e) {
			if(AllowUpdate && DataSource == e.DataSource)
				BuildNodes();
		}
		void OnDataSourceCalculatedFieldCollectionChanged(object sender, DataSourceCalcFieldCollectionChangedEventArgs e) {
			if(AllowUpdate && e.DataSource == DataSource) {
				BuildNodes();
				if(e.InnerArgs.AddedItems.Count > 0)
					SelectedDataMember = e.InnerArgs.AddedItems[0].Name;
			}
		}
		void OnDataSourceCalculatedFieldChanged(object sender, DataSourceCalculatedFieldChangedEventArgs e) {
			if(AllowUpdate && e.DataSource == DataSource) {
				BuildNodes();
				if(e.InnerArgs.Field != null)
					SelectedDataMember = e.InnerArgs.Field.Name;
				if(e.InnerArgs.PropertyName == "Name" && serviceProvider != null) {
					IDataFieldChangeService changeService = serviceProvider.RequestServiceStrictly<IDataFieldChangeService>();
					changeService.OnDataFieldRenamed();
				}
			}
		}
		void OnDataSourceCalculatedFieldsCorrupted(object sender, DataSourceCalcFieldCollectionChangedEventArgs e) {
			if(AllowUpdate && e.DataSource == DataSource) {
				BuildNodes();
			}
		}
		void SetDataSourceSettingsToView() {
			if (view != null) {
				view.Enabled = DataSource != null;
				view.GroupByTypeEnabled = DataSource != null && !(DataSource  is DashboardOlapDataSource);
				view.IsOlap = DataSource != null && (DataSource  is DashboardOlapDataSource);				
			}
		}
		void SetSettingsToView() {
			if (view != null) {
				view.GroupByType = groupByType;
				view.SortAscending = sortAscending;
				view.SortDescending = sortDescending;
				view.DisplayMode = displayMode;
			}
		}
		void GetSettingsFromView() {
			if (view != null) {
				groupByType = view.GroupByType;
				sortAscending = view.SortAscending;
				sortDescending = view.SortDescending;
			}
		}
		void UpdateRefreshButtonVisible() {
			view.RefreshButtonVisible = DataSource is DashboardSqlDataSource;
		}
		void BuildNodes() {
			if(view != null) {
				UpdateRefreshButtonVisible();
				view.BuildNodes(DataSourceNode, serviceProvider ?? parametersSource);
			}
		}
		void ClearAndBuildNodes() {
			if(view != null) {
				UpdateRefreshButtonVisible();
				view.ClearAndBuildNodes(DataSourceNode, ExpandAll, serviceProvider ?? parametersSource);
			}
		}
		CalculatedField GetCalculatedField(DataField dataField) {
			if (dataField != null && dataField.NodeType == DataNodeType.CalculatedDataField) {
				CalculatedField cField = DataSource.CalculatedFields[dataField.DataMember];
				if (cField != null)
					return cField;				
			}
			return null;
		}
		#region IDragSource
		bool IDragSource.AllowNullDrop { get { return false; } }
		IHistoryItem IDragSource.PerformDrag(IDragObject dragObject, bool isSameDragGroup) {
			return null;
		}
		void IDragSource.Cancel() {
		}
		#endregion
	}
}
