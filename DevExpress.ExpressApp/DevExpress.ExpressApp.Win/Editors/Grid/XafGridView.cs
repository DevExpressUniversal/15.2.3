#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IModelSynchronizersHolder {
		IModelSynchronizer GetSynchronizer(Component component);
		void RegisterSynchronizer(Component component, IModelSynchronizer modelSynchronizer);
		void RemoveSynchronizer(Component component);
		void AssignSynchronizers(ColumnView columnView);
		event EventHandler<CustomModelSynchronizerEventArgs> CustomModelSynchronizer;
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICustomizeFilterColumns {
		event EventHandler FilterEditorPopup;
		event EventHandler FilterEditorClosed;
		event EventHandler<CreateCustomFilterColumnCollectionEventArgs> CreateCustomFilterColumnCollection;
		event EventHandler<CustomizeFilterFromFilterBuilderEventArgs> CustomizeFilterFromFilterBuilder;
	}
	public class GetGridViewColumnErrorEventArgs : HandledEventArgs {
		public int RowHandle { get; private set;}
		public GridColumn Column { get; private set;}
		public ErrorType ErrorType { get; set; }
		public string Message { get; set; }
		public GetGridViewColumnErrorEventArgs(int rowHandle, GridColumn column) {
			this.RowHandle = rowHandle;
			this.Column = column;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public interface IXafGridView {
		ErrorMessages ErrorMessages { get; set; }
		Boolean SkipMakeRowVisible { get; set; }
		event EventHandler<GetGridViewColumnErrorEventArgs> GetGridViewColumnError;
	}
	public class XafGridView : GridView, IModelSynchronizersHolder, IXafGridView, ISupportNewItemRow, ICustomizeFilterColumns {
		public Dictionary<Component, IModelSynchronizer> modelSynchronizers = new Dictionary<Component, IModelSynchronizer>();
		private ErrorMessages errorMessages;
		private BaseGridController gridController;
		private Boolean skipMakeRowVisible;
		private Boolean isNewItemRowCancelling;
		private object newItemRowObject;
		private GridListEditorNewItemRowHandlingMode newItemRowHandlingMode;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Bitmap GetColumnBitmap(GridColumn column) {
			return Painter.GetColumnDragBitmap(ViewInfo, column, Size.Empty, false, false);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public DevExpress.XtraGrid.Dragging.DragManager GetDragManager() {
			return ((DevExpress.XtraGrid.Views.Grid.Handler.GridHandler)Handler).DragManager;
		}
		protected virtual void OnGetColumnError(GetGridViewColumnErrorEventArgs args) {
			if(GetGridViewColumnError != null) {
				GetGridViewColumnError(this, args);
			}
		}
		protected override BaseView CreateInstance() {
			XafGridView view = CreateInstanceCore();
			view.SetGridControl(GridControl);
			return view;
		}
		protected virtual XafGridView CreateInstanceCore() {
			return new XafGridView();
		}
		protected override void AssignColumns(ColumnView cv, bool synchronize) {
			base.AssignColumns(cv, synchronize);
			if(!synchronize) {
				((IModelSynchronizersHolder)this).AssignSynchronizers(cv);
			}
		}
		protected override void RaiseShownEditor() {
			if(ActiveEditor is IGridInplaceEdit) {
				((IGridInplaceEdit)ActiveEditor).GridEditingObject = GetFocusedObject();
				if(OptionsView.NewItemRowPosition != DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.None && BaseListSourceDataController.NewItemRow == FocusedRowHandle) {
					object newObject = this.GetRow(BaseListSourceDataController.NewItemRow);
					if(newObject != null) {
						((IGridInplaceEdit)ActiveEditor).GridEditingObject = newObject;
					}
				}
			}
			base.RaiseShownEditor();
		}
		protected override string GetColumnError(int rowHandle, GridColumn column) {
			string result = null;
			GetGridViewColumnErrorEventArgs args = new GetGridViewColumnErrorEventArgs(rowHandle, column);
			OnGetColumnError(args);
			if(args.Handled) {
				result = args.Message;
			}
			else {
				result = base.GetColumnError(rowHandle, column);
			}
			return result;
		}
		protected override ErrorType GetColumnErrorType(int rowHandle, GridColumn column) {
			GetGridViewColumnErrorEventArgs args = new GetGridViewColumnErrorEventArgs(rowHandle, column);
			ErrorType result = ErrorType.Critical;
			OnGetColumnError(args);
			if(args.Handled) {
				result = args.ErrorType;
			}			
			return result;
		}
		protected void RaiseFilterEditorPopup() {
			if(FilterEditorPopup != null) {
				FilterEditorPopup(this, EventArgs.Empty);
			}
		}
		protected void RaiseFilterEditorClosed() {
			if(FilterEditorClosed != null) {
				FilterEditorClosed(this, EventArgs.Empty);
			}
		}
		protected override void ShowFilterPopup(GridColumn column, Rectangle bounds, Control ownerControl, object creator) {
			RaiseFilterEditorPopup();
			base.ShowFilterPopup(column, bounds, ownerControl, creator);
		}
		protected override bool CanShowFilterPopup(FilterPopup filterPopup) { 
			bool result = base.CanShowFilterPopup(filterPopup);
			if(!result) {
				RaiseFilterEditorClosed();
			}
			return result;
		}
		protected override void OnFilterPopupCloseUp(GridColumn column) {
			base.OnFilterPopupCloseUp(column);
			RaiseFilterEditorClosed();
		}
		protected override ColumnFilterInfo DoCustomFilter(GridColumn column, ColumnFilterInfo filterInfo) {
			RaiseFilterEditorPopup();
			SuppressInvalidCastException(column);
			ColumnFilterInfo result = base.DoCustomFilter(column, filterInfo);
			CancelSuppressInvalidCastException(column);
			RaiseFilterEditorClosed();
			return result;
		}
		protected override void RaiseInvalidRowException(InvalidRowExceptionEventArgs ex) {
			if(String.IsNullOrEmpty(ex.ErrorText)) {
				ex.ExceptionMode = ExceptionMode.NoAction;
			}
			else {
				ex.ExceptionMode = ExceptionMode.ThrowException;
			}
			base.RaiseInvalidRowException(ex);
		}
		protected override void OnActiveEditor_MouseDown(object sender, MouseEventArgs e) {
			if(ActiveEditor != null) {
				base.OnActiveEditor_MouseDown(sender, e);
			}
		}
		protected override BaseGridController CreateDataController() {
			if(requireDataControllerType == DataControllerType.AsyncServerMode) {
				gridController = new AsyncServerModeDataController();
			}
			else {
				if(requireDataControllerType == DataControllerType.ServerMode) {
					gridController = new ServerModeDataController();
				}
				else {
					if(DisableCurrencyManager) {
						gridController = new GridDataController();
					}
					else {
						gridController = new XafCurrencyDataController();
					}
				}
			}
			return gridController;
		}
		protected override RepositoryItem GetFilterRowRepositoryItem(GridColumn column, RepositoryItem current) {
			if(column.FilterMode == ColumnFilterMode.Value && current is ILookupEditRepositoryItem) {
				return current;
			}
			return base.GetFilterRowRepositoryItem(column, current);
		}
		protected override void MakeRowVisibleCore(int rowHandle, bool invalidate) {
			if(!skipMakeRowVisible) {
				base.MakeRowVisibleCore(rowHandle, invalidate);
			}
		}
		protected override void AssignActiveFilterFromFilterBuilder(CriteriaOperator newCriteria) {
			CustomizeFilterFromFilterBuilderEventArgs args = new CustomizeFilterFromFilterBuilderEventArgs(newCriteria);
			if(CustomizeFilterFromFilterBuilder != null) {
				CustomizeFilterFromFilterBuilder(this, args);
			}
			base.AssignActiveFilterFromFilterBuilder(args.Criteria);
		}
		Boolean IXafGridView.SkipMakeRowVisible {
			get { return SkipMakeRowVisible; }
			set { SkipMakeRowVisible = value; }
		}
		protected internal Boolean SkipMakeRowVisible {
			get { return skipMakeRowVisible; }
			set { skipMakeRowVisible = value; }
		}
		Boolean ISupportNewItemRow.IsNewItemRowCancelling {
			get { return isNewItemRowCancelling; }
		}
		protected override FilterColumnCollection CreateFilterColumnCollection() {
			CreateCustomFilterColumnCollectionEventArgs args = new CreateCustomFilterColumnCollectionEventArgs();
			if(CreateCustomFilterColumnCollection != null) {
				CreateCustomFilterColumnCollection(this, args);
			}
			if(args.FilterColumnCollection != null) {
				return args.FilterColumnCollection;
			}
			else {
				return base.CreateFilterColumnCollection();
			}
		}
		public override void ShowFilterEditor(GridColumn defaultColumn) {
			RaiseFilterEditorPopup();
			SuppressInvalidCastException();
			base.ShowFilterEditor(defaultColumn);
			CancelSuppressInvalidCastException();
			RaiseFilterEditorClosed();
		}
		public override void CancelUpdateCurrentRow() {
			int updatedRowHandle = FocusedRowHandle;
			isNewItemRowCancelling = (updatedRowHandle == BaseGridController.NewItemRow);
			try {
				newItemRowObject = GetFocusedObject();
				base.CancelUpdateCurrentRow(); 
				if(updatedRowHandle == BaseGridController.NewItemRow) {
					int storedFocusedHandle = FocusedRowHandle;
					FocusedRowHandle = BaseGridController.NewItemRow;
					if(CancelNewRow != null) {
						CancelNewRow(this, EventArgs.Empty);
					}
					XtraGridUtils.SelectRowByHandle(this, storedFocusedHandle);
				}
				else {
					if(RestoreCurrentRow != null) {
						RestoreCurrentRow(this, EventArgs.Empty);
					}
				}
			}
			finally {
				newItemRowObject = null;
				isNewItemRowCancelling = false;
			}
		}
		object ISupportNewItemRow.NewItemRowObject {
			get {
				return newItemRowObject;
			}
		}
		public bool IsFirstColumnInFirstRowFocused {
			get {
				return (FocusedRowHandle == 0) && (FocusedColumn == GetVisibleColumn(0));
			}
		}
		public bool IsLastColumnInLastRowFocused {
			get {
				return (FocusedRowHandle == RowCount - 1) && IsLastColumnFocused;
			}
		}
		public bool IsLastColumnFocused {
			get {
				return (FocusedColumn == GetVisibleColumn(VisibleColumns.Count - 1));
			}
		}
		public ErrorMessages ErrorMessages {
			get { return errorMessages; }
			set { errorMessages = value; }
		}
		public GridListEditorNewItemRowHandlingMode NewItemRowHandlingMode {
			get { return newItemRowHandlingMode; }
			set {
				newItemRowHandlingMode = value;
				if(gridController is XafCurrencyDataController) {
					((XafCurrencyDataController)gridController).NewItemRowHandlingMode = value;
				}
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				modelSynchronizers = null;
			}
		}
		private object GetFocusedObject() {
			return XtraGridUtils.GetFocusedRowObject(this);
		}
		private void SuppressInvalidCastException() {
			foreach(GridColumn column in Columns) {
				SuppressInvalidCastException(column);
			}
		}
		private void SuppressInvalidCastException(GridColumn column) {
			if(column.ColumnEdit != null && column.ColumnEdit is RepositoryItemLookupEdit) {
				((RepositoryItemLookupEdit)column.ColumnEdit).ThrowInvalidCastException = false;
			}
		}
		private void CancelSuppressInvalidCastException() {
			foreach(GridColumn column in Columns) {
				CancelSuppressInvalidCastException(column);
			}
		}
		private void CancelSuppressInvalidCastException(GridColumn column) {
			if(column.ColumnEdit != null && column.ColumnEdit is RepositoryItemLookupEdit) {
				((RepositoryItemLookupEdit)column.ColumnEdit).ThrowInvalidCastException = true;
			}
		}
		#region IColumnInfoProvider
		IModelSynchronizer IModelSynchronizersHolder.GetSynchronizer(Component component) {
			IModelSynchronizer result = null;
			if(component != null) {
				result = OnCustomModelSynchronizer(component);
				if(result == null) {
					modelSynchronizers.TryGetValue(component, out result);
				}
			}
			return result;
		}
		void IModelSynchronizersHolder.RegisterSynchronizer(Component component, IModelSynchronizer modelSynchronizer) {
			modelSynchronizers.Add(component, modelSynchronizer);
		}
		void IModelSynchronizersHolder.RemoveSynchronizer(Component component) {
			if(component != null && modelSynchronizers.ContainsKey(component)) {
				modelSynchronizers.Remove(component);
			}
		}
		void IModelSynchronizersHolder.AssignSynchronizers(ColumnView sourceView) {
			IModelSynchronizersHolder current = (IModelSynchronizersHolder)this;
			IModelSynchronizersHolder sourceSynchronizersHolder = (IModelSynchronizersHolder)sourceView;
			for(int n = 0; n < sourceView.Columns.Count; n++) {
				IModelSynchronizer info = sourceSynchronizersHolder.GetSynchronizer(sourceView.Columns[n]);
				if(info != null) {
					current.RegisterSynchronizer(Columns[n], info);
				}
			}
		}
		private event EventHandler<CustomModelSynchronizerEventArgs> CustomModelSynchronizer;
		event EventHandler<CustomModelSynchronizerEventArgs> IModelSynchronizersHolder.CustomModelSynchronizer {
			add { CustomModelSynchronizer += value; }
			remove { CustomModelSynchronizer -= value; }
		}
		protected virtual IModelSynchronizer OnCustomModelSynchronizer(Component component) {
			if(CustomModelSynchronizer != null) {
				CustomModelSynchronizerEventArgs args = new CustomModelSynchronizerEventArgs(component);
				CustomModelSynchronizer(this, args);
				return args.ModelSynchronizer;
			}
			return null;
		}
		#endregion
		public event EventHandler FilterEditorPopup;
		public event EventHandler FilterEditorClosed;
		public event EventHandler CancelNewRow;
		public event EventHandler RestoreCurrentRow;
		public event EventHandler<CreateCustomFilterColumnCollectionEventArgs> CreateCustomFilterColumnCollection;
		public event EventHandler<CustomizeFilterFromFilterBuilderEventArgs> CustomizeFilterFromFilterBuilder;
		public event EventHandler<GetGridViewColumnErrorEventArgs> GetGridViewColumnError;
#if DebugTest
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public FilterColumnCollection CreateFilterColumnCollectionForTests() {
			return CreateFilterColumnCollection();
		}
#endif
		#region Obsolete 14.2
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This method is not used.", true)]
		protected internal void CancelCurrentRowEdit() {
		}
		#endregion
	}
}
