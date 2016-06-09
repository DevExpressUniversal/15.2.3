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
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Data;
#if !SL
using System.Data;
using System.Windows.Forms;
using DevExpress.Data.Details;
#else
using BindingContext = System.Object;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Data {
	public enum CacheRowValuesMode { CacheAll, Disabled };
	internal class NullValidationSupport : IDataControllerValidationSupport {
		internal static NullValidationSupport Default = new NullValidationSupport();
		#region IDataControllerValidationSupport Members
		public IBoundControl BoundControl { get { return null; } }
		public void OnBeginCurrentRowEdit() { }
		public void OnCurrentRowUpdated(ControllerRowEventArgs e) { }
		public void OnValidatingCurrentRow(ValidateControllerRowEventArgs e) { }
		public void OnPostRowException(ControllerRowExceptionEventArgs e) { }
		public void OnPostCellException(ControllerRowCellExceptionEventArgs e) { }
		public void OnControllerItemChanged(ListChangedEventArgs e) { }
		public void OnStartNewItemRow() { }
		public void OnEndNewItemRow() { }
		#endregion
	}
	public class ControllerRowValuesKeeper : IDisposable {
		BaseGridController controller;
		object[] values = null;
		public ControllerRowValuesKeeper(BaseGridController controller) {
			this.controller = controller;
		}
		public void Dispose() {
			this.controller = null;
			Clear();
		}
		protected virtual int ControllerRow { get { return Controller.CurrentControllerRow; } }
		protected bool CanSave { get { return Controller.IsControllerRowValid(ControllerRow) && Controller.ValuesCacheMode != CacheRowValuesMode.Disabled; } }
		protected bool CanRestore { get { return this.values != null && CanSave; } }
		protected object[] Values { get { return values; } }
		protected BaseGridController Controller { get { return controller; } }
		public void Clear() {
			this.values = null;
		}
		public void SaveValues() {
			Clear();
			if(!CanSave) return;
			this.values = new object[Controller.Columns.Count];
			for(int n = 0; n < Controller.Columns.Count; n++) {
				DataColumnInfo column = Controller.Columns[n];
				if(column.ReadOnly) continue;
				Values[n] = controller.GetRowValue(ControllerRow, column);
			}
		}
		public void RestoreValues() {
			try {
				if(CanRestore) {
					for(int n = 0; n < Controller.Columns.Count; n++) {
						DataColumnInfo column = Controller.Columns[n];
						if(column.ReadOnly) continue;
						object oldValue = controller.GetRowValue(ControllerRow, column);
						object newValue = Values[n];
						bool nonequal = false;
						try {
							if(!(oldValue is IComparable)) oldValue = null;
							if(!(newValue is IComparable)) newValue = null;
							nonequal = controller.ValueComparer.Compare(oldValue, newValue) != 0;
						} catch {
							continue;
						}
						if(nonequal) controller.SetRowValue(ControllerRow, column, Values[n]);
					}
				}
			} catch { }
			Clear();
		}
	}
	public delegate void InternalExceptionEventHandler(object sender, InternalExceptionEventArgs e);
	public class InternalExceptionEventArgs : EventArgs {
		Exception exception;
		public InternalExceptionEventArgs(Exception e) { exception = e; }
		public Exception Exception { get { return exception; } }
	}
	public abstract class BaseGridController : BaseListSourceDataController, IColumnsServerActions {
		public event InternalExceptionEventHandler CatchException;
		ControllerRowValuesKeeper valuesKeeper;
		internal int lastGroupedColumnCount = 0;
		bool keepFocusedRowOnUpdate = true, allowCurrentControllerRow = true, allowCurrentRowObjectForGroupRow = true;
		internal bool currentRowEditing = false;
		int currentControllerRow = InvalidRow;
		object currentControllerRowObject = null;
		int currentControllerRowObjectLevel = 0;
		CacheRowValuesMode valuesCacheMode = CacheRowValuesMode.CacheAll;
		IDataControllerValidationSupport validationClient = NullValidationSupport.Default;
		IDataControllerCurrentSupport currentClient = NullCurrentSupport.Default;
		internal object dataSource = null;
		internal string dataMember = string.Empty;
		protected BaseGridController() {
			this.valuesKeeper = new ControllerRowValuesKeeper(this);
		}
		public override void Dispose() {
			this.valuesKeeper.Dispose();
			ValidationClient = null;
			CurrentClient = null;
			CatchException = null;
			this.currentControllerRowObject = null;
			base.Dispose();
		}
		protected virtual void RaiseOnCatchException(InternalExceptionEventArgs e) {
			if(CatchException != null) CatchException(this, e);
		}
		protected override void VisibleListSourceRowMove(int oldControllerRow, ref int newControllerRow, DataControllerChangedItemCollection changedItems, bool isAdding) {
			base.VisibleListSourceRowMove(oldControllerRow, ref newControllerRow, changedItems, isAdding);
		}
		protected override void OnRefreshed() {
			CheckCurrentControllerRowObjectOnRefresh();
			base.OnRefreshed();
		}
		protected virtual void CheckCurrentControllerRowObjectOnRefresh() {
			CheckCurrentControllerRowObject();
		}
		public bool AllowCurrentControllerRow {
			get { return allowCurrentControllerRow; }
			set {
				if(AllowCurrentControllerRow == value) return;
				allowCurrentControllerRow = value;
				if(!AllowCurrentControllerRow) this.currentControllerRow = InvalidRow;
			}
		}
		protected ControllerRowValuesKeeper ValuesKeeper { get { return valuesKeeper; } }
		protected override SelectedRowsKeeper CreateSelectionKeeper() { return new CurrentAndSelectedRowsKeeper(this, true); }
		protected override void DoGroupRows() {
			this.lastGroupedColumnCount = GroupedColumnCount;
			base.DoGroupRows();
		}
		public int LastGroupedColumnCount { get { return lastGroupedColumnCount; } }
		public CacheRowValuesMode ValuesCacheMode { get { return valuesCacheMode; } set { valuesCacheMode = value; } }
		public virtual IDataControllerValidationSupport ValidationClient {
			get { return validationClient; }
			set {
				if(value == null) value = NullValidationSupport.Default;
				validationClient = value;
			}
		}
		protected bool ValidationClientIsAllowBeginInvoke {
			get {
				return ValidationClient.BoundControl != null && ValidationClient.BoundControl.IsHandleCreated;
			}
		}
		public virtual IDataControllerCurrentSupport CurrentClient {
			get { return currentClient; }
			set {
				if(value == null) value = NullCurrentSupport.Default;
				currentClient = value;
			}
		}
		protected override void BeginInvoke(Delegate method) {
			if(ValidationClientIsAllowBeginInvoke) 
				ValidationClient.BoundControl.BeginInvoke(method);
			else
				base.BeginInvoke(method);
		}
		protected virtual void OnPostRowCellException(int controllerRow, int column, object row, Exception exception) {
			ControllerRowCellExceptionEventArgs e = new ControllerRowCellExceptionEventArgs(controllerRow, column, row, exception);
			ValidationClient.OnPostCellException(e);
		}
		protected virtual ExceptionAction OnPostRowException(Exception exception) {
			ControllerRowExceptionEventArgs e = new ControllerRowExceptionEventArgs(CurrentControllerRow, GetRow(CurrentControllerRow), exception);
			ValidationClient.OnPostRowException(e);
			return e.Action;
		}
		public virtual void ResetCurrentPosition() {
			CurrentControllerRow = AllowCurrentControllerRow && IsReady ? VisibleIndexes.GetHandle(0) : InvalidRow;
			CheckCurrentControllerRowObject();
		}
		public bool KeepFocusedRowOnUpdate {
			get { return keepFocusedRowOnUpdate; }
			set { keepFocusedRowOnUpdate = value; }
		}
		public int CurrentListSourceIndex {
			get { return GetListSourceRowIndex(CurrentControllerRow); }
			set { CurrentControllerRow = GetControllerRow(value); }
		}
		public object CurrentControllerRowObject {
			get {
				if(!IsReady || !AllowCurrentControllerRow) return null;
				return currentControllerRowObject;
			}
		}
		public virtual int CurrentControllerRow {
			get {
				if(!IsReady || !AllowCurrentControllerRow) return InvalidRow;
				return currentControllerRow;
			}
			set {
				if(!IsReady || !IsValidControllerRowHandle(value) || !AllowCurrentControllerRow) value = InvalidRow;
				if(CurrentControllerRow == value) {
					if(!IsReady) currentControllerRow = value;
					return;
				}
				OnCurrentControllerRowChanging(CurrentControllerRow, value);
			}
		}
		protected virtual BindingContext BindingContext {
			get { return null; }
			set { SetDataSource(null, DataSource, DataMember); }
		}
		public object DataSource {
			get { return dataSource; }
			set { SetDataSource(BindingContext, value, DataMember); }
		}
		public string DataMember {
			get { return dataMember; }
			set { SetDataSource(BindingContext, DataSource, value); }
		}
		protected override void Reset() {
			this.currentControllerRowObject = null;
			this.currentControllerRowObjectLevel = 0;
			base.Reset();
		}
		public override void SetDataSource(object dataSource) {
			SetDataSource(null, dataSource, string.Empty);
		}
		protected virtual bool CompareDataSource(object dataSource, string dataMember) {
			if(DataMember != dataMember) return false;
			if(DataSource == dataSource) return true;
			ListIEnumerable ds = DataSource as ListIEnumerable;
			ListIEnumerable dsNew = dataSource as ListIEnumerable;
			if(ds != null && dsNew != null) return ds.Source == dsNew.Source;
			return false;
		}
		public virtual void SetDataSource(BindingContext context, object dataSource, string dataMember) {
			if(dataMember == null) dataMember = string.Empty;
			if(CompareDataSource(dataSource, dataMember)) return;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			OnDataSourceChanged();
		}
		protected abstract void OnDataSourceChanged();
		protected virtual void OnCurrentControllerRowChanging(int oldControllerRow, int newControllerRow) {
			if(IsCurrentRowEditing) {
				if(!EndCurrentRowEdit()) return;
			}
			this.currentControllerRow = newControllerRow;
			SyncCurrentRow();
			OnCurrentControllerRowChanged();
		}
		internal void InternalSetControllerRow(int newCurrentRow) {
			this.currentControllerRow = newCurrentRow;
		}
		protected virtual void OnCurrentControllerRowChanged() {
			CurrentClient.OnCurrentControllerRowChanged(new CurrentRowEventArgs());
			CheckCurrentControllerRowObject();
		}
		protected virtual void OnCurrentControllerRowObjectChanging(object oldObject, object newObject, int level) {
			this.currentControllerRowObject = newObject;
			this.currentControllerRowObjectLevel = level;
			CurrentClient.OnCurrentControllerRowObjectChanged(new CurrentRowChangedEventArgs(oldObject, newObject));
		}
		protected internal void CheckCurrentControllerRowObject() { CheckCurrentControllerRowObject(null); }
		protected virtual void CheckCurrentControllerRowObject(ListChangedEventArgs e) {
			if(e != null && AllowCurrentControllerRow) SyncCurrentRowObject(e);
			SetCurrentControllerRowObject(GetCurrentControllerRowObject(), GetRowLevel(CurrentControllerRow));
		}
		public virtual bool AllowCurrentRowObjectForGroupRow {
			get { return allowCurrentRowObjectForGroupRow; }
			set { allowCurrentRowObjectForGroupRow = value; }
		}
		protected virtual object GetCurrentControllerRowObject() {
			if(!AllowCurrentRowObjectForGroupRow && CurrentControllerRow < 0) {
				if(CurrentControllerRow != NewItemRow && IsGrouped && IsGroupRowHandle(CurrentControllerRow)) return null;
			}
			return GetRow(CurrentControllerRow);
		}
		protected virtual void SetCurrentControllerRowObject(object value, int level) {
			if(!IsReady || !AllowCurrentControllerRow) value = null;
			if(object.ReferenceEquals(CurrentControllerRowObject, value)) return;
			OnCurrentControllerRowObjectChanging(CurrentControllerRowObject, value, level);
		}
		protected override void OnBindingListChangedCore(ListChangedEventArgs e) {
			base.OnBindingListChangedCore(e);
			ValidationClient.OnControllerItemChanged(ConvertListArgsToControllerArgs(e));
			CheckCurrentControllerRowObject(e);
		}
		internal void SyncCurrentRowObject(ListChangedEventArgs e) {
			if(!KeepFocusedRowOnUpdate || !AllowCurrentControllerRow) return;
			if(CurrentControllerRowObject == null || object.ReferenceEquals(GetRow(CurrentControllerRow), CurrentControllerRowObject)) return;
			if(e != null && e.ListChangedType == ListChangedType.Reset) return;
			int controllerRow = FindRowByRowValue(CurrentControllerRowObject);
			if(controllerRow == DataController.InvalidRow) {
				if(!IsValidControllerRowHandle(CurrentControllerRow)) controllerRow = GetControllerRowHandle(VisibleCount - 1);
				if(controllerRow == DataController.InvalidRow) return;
			}
			if(currentControllerRowObjectLevel != GroupInfo.LevelCount) {
				controllerRow = FindRowLevel(controllerRow, currentControllerRowObjectLevel);
			}
			CurrentControllerRow = controllerRow;
		}
		protected int FindRowLevel(int controllerDataRow, int requiredLevel) {
			if(requiredLevel > GroupInfo.LevelCount) return controllerDataRow;
			GroupRowInfo info = GroupInfo.GetGroupRowInfoByControllerRowHandle(controllerDataRow);
			while(info != null) {
				if(info.Level == requiredLevel) return info.Handle;
				info = info.ParentGroup;
			}
			return controllerDataRow;
		}
		ListChangedEventArgs ConvertListArgsToControllerArgs(ListChangedEventArgs e) {
			int oldIndex = e.OldIndex < 0 ? InvalidRow : GetControllerRow(e.OldIndex),
				newIndex = e.NewIndex < 0 ? InvalidRow : GetControllerRow(e.NewIndex);
			return new ListChangedEventArgs(e.ListChangedType, newIndex, oldIndex);
		}
		public object GetCurrentRowValue(DataColumnInfo column) { return GetRowValue(CurrentControllerRow, column); }
		public object GetCurrentRowValue(int column) { return GetRowValue(CurrentControllerRow, column); }
		public object GetCurrentRowValue(string columnName) { return GetRowValue(CurrentControllerRow, columnName); }
		public void SetCurrentRowValue(DataColumnInfo column, object val) { SetRowValue(CurrentControllerRow, column, val); }
		public void SetCurrentRowValue(int column, object val) { SetRowValue(CurrentControllerRow, column, val); }
		public void SetCurrentRowValue(string columnName, object val) { SetRowValue(CurrentControllerRow, columnName, val); }
		public virtual void SyncCurrentRow() { }
		protected override void SetRowValueCore(int controllerRow, int column, object val) {
			if(controllerRow == CurrentControllerRow) {
				BeginCurrentRowEdit();
			}
			try {
				if(controllerRow == NewItemRow) {
					Helper.SetNewRowValue(column, val);
					return;
				}
				base.SetRowValueCore(controllerRow, column, val);
			}
			catch(Exception e) {
				OnPostRowCellException(controllerRow, column, GetRow(controllerRow), e);
			}
		}
		public virtual void CancelCurrentRowEdit() {
			if(!IsReady) return;
			bool prevNewItemRowEditing = IsNewItemRowEditing;
			CancelRowEdit(CurrentControllerRow);
			if(prevNewItemRowEditing) Helper.CancelNewItemRow();
			StopCurrentRowEditCore();
			if(!prevNewItemRowEditing) RaiseCurrentRowChanged();
		}
		public virtual void BeginCurrentRowEdit() {
			if(!IsReady) return;
			if(!IsCurrentRowEditing) {
				BeginRowEdit(CurrentControllerRow);
				this.currentRowEditing = true;
				ValidationClient.OnBeginCurrentRowEdit();
			}
		}
		public override int GetControllerRow(int listSourceRow) {
			if(Helper.DetachedListSourceRow == listSourceRow)
				return NewItemRow;
			return base.GetControllerRow(listSourceRow);
		}
		public override bool IsGroupRowHandle(int controllerRowHandle) {
			if(controllerRowHandle == FilterRow) return false;
			return base.IsGroupRowHandle(controllerRowHandle);
		}
		public bool IsCurrentRowModified { get { return IsCurrentRowEditing; } }
		public virtual bool IsCurrentRowEditing { get { return currentRowEditing; } }
		public bool EndCurrentRowEdit() {
			return EndCurrentRowEdit(false);
		}
		protected virtual bool RequireEndEditOnGroupRows { get { return true; } }
		int _lockEndEdit = 0;
		protected bool IsLockEndEdit { get { return _lockEndEdit != 0; } }
		protected void BeginLockEndEdit() {
			_lockEndEdit++;
		}
		protected void EndLockEndEdit() { _lockEndEdit--; }
		public virtual bool EndCurrentRowEdit(bool force) {
			if(!IsReady || IsLockEndEdit) return true;
			if(IsGroupRowHandle(CurrentControllerRow) && !IsCurrentRowEditing) return true;
			BeginLockEndEdit();
			bool prevEditing = IsNewItemRowEditing || IsCurrentRowEditing;
			bool fireEvents = true;
			int currentRow = CurrentControllerRow;
			bool prevNewItemRowEditing = IsNewItemRowEditing;
			fireEvents = force || IsCurrentRowEditing;
			object row = fireEvents ? GetRow(CurrentControllerRow) : CurrentControllerRow;
			bool hasRowReference = fireEvents;
			force = true;
			try {
				try {
					if(IsCurrentRowEditing) {
						ValuesKeeper.SaveValues();
						if(!OnCurrentRowValidating()) return false;
					} else {
						if(!force || currentRow == InvalidRow) return true;
						if(fireEvents) ValuesKeeper.SaveValues();
					}
					if(IsCurrentRowEditing || (!IsGroupRowHandle(CurrentControllerRow) || RequireEndEditOnGroupRows))
					EndRowEdit(CurrentControllerRow);
					if(prevNewItemRowEditing && Helper.CancelAddNew != null) Helper.CancelAddNew.EndNew(CurrentListSourceIndex);
				}
				catch(Exception e) {
					if(OnPostRowException(e) == ExceptionAction.CancelAction) {
						CancelCurrentRowEdit();
						return true;
					}
					BeginRowEdit(CurrentControllerRow); 
					if(fireEvents) ValuesKeeper.RestoreValues();
					return false;
				}
				StopCurrentRowEditCore();
				bool updateResult = true; 
				if(fireEvents) {
					if(!prevNewItemRowEditing) {
						updateResult = OnCurrentRowUpdated(prevEditing, currentRow, row);
						RaiseCurrentRowChanged(prevEditing);
					}
					else {
						VisualClient.UpdateScrollBar();
						updateResult = OnCurrentRowUpdated(prevEditing, currentRow, row);
					}
				}
				if(!updateResult || CurrentControllerRow == NewItemRow) {
					if(hasRowReference) CurrentControllerRow = FindRowByRowValue(row);
				}
				return updateResult;
			}
			finally {
				EndLockEndEdit();
			}
		}
		protected override void CheckInvalidCurrentRow() {
			if(!IsValidControllerRowHandle(CurrentControllerRow)) CurrentControllerRow = 0;
		}
		protected void RaiseCurrentRowChanged() { RaiseCurrentRowChanged(true); }
		protected virtual void RaiseCurrentRowChanged(bool prevEditing) {
			if(prevEditing)
				OnBindingListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, CurrentListSourceIndex, CurrentListSourceIndex));
		}
		protected virtual void StopCurrentRowEditCore() {
			this.currentRowEditing = false;
		}
		protected virtual bool OnCurrentRowValidating() {
			ValidateControllerRowEventArgs e = new ValidateControllerRowEventArgs(CurrentControllerRow, GetRow(CurrentControllerRow));
			ValidationClient.OnValidatingCurrentRow(e);
			return e.Valid;
		}
		protected virtual bool OnCurrentRowUpdated(bool prevEditing, int controllerRow, object row) {
			if(!prevEditing) return true;
			try {
				ControllerRowEventArgs e = new ControllerRowEventArgs(controllerRow, row);
				ValidationClient.OnCurrentRowUpdated(e);
			}
			catch(Exception ex) {
				if(!CatchRowUpdatedExceptions) throw;
				RaiseOnCatchException(new InternalExceptionEventArgs(ex));
				return false;
			}
			return true;
		}
		bool IColumnsServerActions.AllowAction(string fieldName, ColumnServerActionType action) {
			return AllowServerAction(fieldName, action);
		}
		protected virtual bool AllowServerAction(string fieldName, ColumnServerActionType action) {
			if(IsServerMode) {
				DataColumnInfo info = Columns[fieldName];
				if(info != null && info.Unbound) return false;
			}
			IColumnsServerActions serverActions = DataSource as IColumnsServerActions;
			if(serverActions != null) return serverActions.AllowAction(fieldName, action);
			return true;
		}
	}
	public abstract class BaseGridControllerEx : BaseGridController {
		protected override void OnItemChanged(ListChangedEventArgs e, DataControllerChangedItemCollection changedItems) {
			if(IsCurrentRowEditing) {
				int listSourceRow = GetChangedListSourceRow(e);
				if(CurrentListSourceIndex == listSourceRow) {
					changedItems.AddItem(GetControllerRow(listSourceRow), NotifyChangeType.ItemChanged, null);
					return;
				}
			}
			base.OnItemChanged(e, changedItems);
		}
		public virtual int EndNewRowEdit() {
			if(!IsNewItemRowEditing) return InvalidRow;
			EndRowEdit(NewItemRow);
			this.newItemRowEditing = false;
			Helper.RaiseOnEndNewItemRow();
			return VisibleCount - 1;
		}
		public virtual void CancelNewRowEdit() {
			if(!IsNewItemRowEditing) return;
			try {
				CancelRowEdit(NewItemRow);
			}
			finally {
				this.newItemRowEditing = false;
			}
			Helper.RaiseOnEndNewItemRow();
		}
		public override void AddNewRow() {
			if(!IsReady || !AllowNew) return;
			try {
				if(IsNewItemRowEditing) EndNewRowEdit();
				if(IsNewItemRowEditing) return;
				BeginLockEndEdit();
				this.newItemRowEditing = true;
				object addedRow = Helper.AddNewRow();
				Helper.UpdateDetachedIndex(addedRow);
				CurrentControllerRow = NewItemRow;
				Helper.RaiseOnStartNewItemRow();
				BeginCurrentRowEdit();
			}
			catch {
				this.newItemRowEditing = false;
				Helper.SetDetachedListSourceRow(DataController.InvalidRow);
				throw;
			}
			finally {
				EndLockEndEdit();
			}
		}
		protected override void StopCurrentRowEditCore() {
			base.StopCurrentRowEditCore();
			Helper.RaiseOnEndNewItemRow();
		}
		protected internal override void OnStartNewItemRow() {
			base.OnStartNewItemRow();
			ValidationClient.OnStartNewItemRow();
		}
		protected internal override void OnEndNewItemRow() {
			base.OnEndNewItemRow();
			ValidationClient.OnEndNewItemRow();
		}
		public override bool IsControllerRowValid(int controllerRow) {
			if(IsReady && controllerRow == NewItemRow && Helper.GetNewItemRowIndex() != InvalidRow) return true;
			return base.IsControllerRowValid(controllerRow);
		}
		public override int GetListSourceRowIndex(int controllerRow) {
			if(controllerRow == NewItemRow) {
				return Helper.GetNewItemRowIndex();
			}
			return base.GetListSourceRowIndex(controllerRow);
		}
	}
	public class GridDataController : BaseGridControllerEx {
		protected override void OnDataSourceChanged() {
			SetListSource(GetListSource());
			if(IsReady) ResetCurrentPosition();
		}
		protected virtual IList GetListSource() {
			if(DataSource == null) return null;
#if !SL && !DXPORTABLE
			try {
				DataSet ds = DataSource as DataSet;
				if(ds != null && !string.IsNullOrEmpty(DataMember)) {
					DataTable dt = ds.Tables[DataMember];
					if(dt != null) return dt.DefaultView;
				}
				IListSource ls = DataSource as IListSource;
				if(ls != null) return ListBindingHelper.GetList(DataSource, DataMember) as IList;
			}
			catch {
			}
#else
			IListSource ls = DataSource as IListSource;
			if(ls != null) return ls.GetList();
#endif
#if !SL
			DataTable table = DataSource as DataTable;
			if(table != null) return table.DefaultView;
#endif
			IList list = DataSource as IList;
			return list;
		}
	}
}
