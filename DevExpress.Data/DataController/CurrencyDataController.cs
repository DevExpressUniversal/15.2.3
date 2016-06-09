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
using System.Data;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Details;
using DevExpress.Data.Selection;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Data {
	public class CurrencyDataController : BaseGridController {
		BindingContext bindingContext = null;
		CurrencyManager currencyManager = null;
		int lockPositionChange = 0, lockCurrencyResetEvent = 0;
		public CurrencyDataController() : base() {
		}
		public override void Dispose() {
			ClearClients();
			CurrencyManager = null;
			base.Dispose();
		}
		public override void RefreshData() {
			this.refreshFired = false;
			if(IsReady && CurrencyManager != null) {
				CurrencyManager.Refresh();
				if(this.refreshFired) return;
			} 
			base.RefreshData();
		}
		bool refreshFired = false;
		protected override void DoRefresh(bool useRowsKeeper) {
			this.refreshFired = true;
			base.DoRefresh(useRowsKeeper);
		}
		public override bool IsControllerRowValid(int controllerRow) {
			if(IsReady && controllerRow == NewItemRow && Helper.GetNewItemRowIndex() != InvalidRow) return true;
			return base.IsControllerRowValid(controllerRow);
		}
		public override int GetListSourceRowIndex(int controllerRow) {
			if(controllerRow == NewItemRow) {
				if(this.cancelingRowEdit != 0) return InvalidRow;
				return Helper.GetNewItemRowIndex();
			}
			return base.GetListSourceRowIndex(controllerRow);
		}
		protected override void OnRefreshed() { 
			base.OnRefreshed();
			SyncCurrentRow();
		}
		protected override SelectionController CreateSelectionController() { return new CurrencySelectionController(this); }
		protected override BindingContext BindingContext { 
			get { return bindingContext; } 
			set { SetDataSource(value, DataSource, DataMember); }
		}
		protected CurrencyManager CurrencyManager {
			get { return currencyManager; }
			set {
				if(CurrencyManager == value) return;
				if(CurrencyManager != null) {
					CurrencyManager.CurrentChanged -=new EventHandler(OnCurrencyManager_CurrentChanged);
					CurrencyManager.ItemChanged -= new ItemChangedEventHandler(OnCurrencyManager_ItemChanged);
					CurrencyManager.PositionChanged -= new EventHandler(OnCurrencyManager_PositionChanged);
				}
				currencyManager = value;
				if(CurrencyManager != null) {
					CurrencyManager.CurrentChanged += new EventHandler(OnCurrencyManager_CurrentChanged);
					CurrencyManager.ItemChanged += new ItemChangedEventHandler(OnCurrencyManager_ItemChanged);
					CurrencyManager.PositionChanged += new EventHandler(OnCurrencyManager_PositionChanged);
				}
				OnCurrencyManagerChanged();
			}
		}
		public override void SetDataSource(BindingContext context, object dataSource, string dataMember) {
			if(dataMember == null) dataMember = string.Empty;
			if(context == BindingContext && CompareDataSource(dataSource, dataMember)) return;
			if(CurrencyManager != null) EndCurrentRowEdit();
			this.bindingContext = context;
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			OnDataSourceChanged();
			CurrencyManagerEndEdit();
		}
		protected override void OnPostRefresh(bool useRowsKeeper) {
			base.OnPostRefresh(useRowsKeeper);
			if(!MaintainVisibleRowBindingOnFilterChange) return;
			if(CurrentControllerRow == FilterRow) {
				int cr = GetControllerRow(CurrencyManagerPosition);
				if(cr < 0 || GetVisibleIndex(cr) < 0) {
					SetCurrencyManagerPositionCore(GetListSourceRowIndex(0));
				}
			}
		}
		public override void SyncCurrentRow() {
			SetCurrencyManagerPositionCore(GetListSourceRowIndex(CurrentControllerRow));
		}
		protected int CurrencyManagerPosition {
			get { return CurrencyManager == null ? 0 : CurrencyManager.Position; }
			set {
				if(CurrencyManager == null) return;
				if(value < 0 || value >= CurrencyManager.Count) value = 0;
				if(CurrencyManager.Position != value) { 
					try {
					CurrencyManager.Position = value;
					} catch(IndexOutOfRangeException e) {
						if(!IsCurrentRowEditing) throw e;
					}
				}
			}
		}
		protected override void OnDataSourceChanged() {
			CurrencyManager = GetCurrencyManager();
			if(IsReady)
				ResetCurrentPosition();
		}
		protected virtual void OnCurrencyManagerChanged() {
			SetListSource(GetCurrencyManagerListSource());
		}
		protected IList GetCurrencyManagerListSource() {
			return CurrencyManager == null ? null : CurrencyManager.List;
		}
		protected CurrencyManager GetCurrencyManager() {
			if(BindingContext == null || DataSource == null) return null;
			return BindingContext[DataSource, DataMember] as CurrencyManager;
		}
		void OnCurrencyManager_CurrentChanged(object sender, EventArgs e) {
		}
		protected override void RaiseCurrentRowChanged(bool prevEditing) {
			if(prevEditing)
				OnBindingListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, CurrencyManagerPosition, CurrencyManagerPosition));
		}
		bool fieldRequested = false;
		System.Reflection.FieldInfo suspendPushDataInCurrentChangedInfo = null;
		protected bool GetInListItemChanged() {
			if(!fieldRequested && suspendPushDataInCurrentChangedInfo == null)
				suspendPushDataInCurrentChangedInfo = typeof(CurrencyManager).GetField("suspendPushDataInCurrentChanged", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			this.fieldRequested = true;
			if(suspendPushDataInCurrentChangedInfo != null && CurrencyManager != null) return (bool)suspendPushDataInCurrentChangedInfo.GetValue(CurrencyManager);
			return true;
		}
		protected void LockPositionChange() {
			this.lockPositionChange++;
		}
		protected void UnlockPositionChange() {
			this.lockPositionChange--;
		}
		protected void SetCurrencyManagerPositionCore(int position) {
			if(position == InvalidRow || position == NewItemRow) return;
			LockPositionChange();
			try {
				CurrencyManagerPosition = position;
			}
			finally {
				UnlockPositionChange();
			}
		}
		protected override void CheckCurrentControllerRowObject(ListChangedEventArgs e) {
			if(IsPositionNotificationLocked) return; 
			base.CheckCurrentControllerRowObject(e);
		}
		bool syncPositionOnItemChanged = false;
		protected bool IsPositionNotificationLocked { get { return this.lockPositionChange != 0; } }
		void OnCurrencyManager_PositionChanged(object sender, EventArgs e) {
			if(IsPositionNotificationLocked) return;
			if(IsFromCancelEdit()) {
				this.syncPositionOnItemChanged = true;
				return;
			}
			if(IsFromListChanged()) {
				if(ListSourceRowCount == 0 && CurrencyManagerPosition == this.tempDeatchedListSourceRow) 
					NotifyClientOnNextChange = false;
				else
					NotifyClientOnNextChange = true;
			}
			StopCurrentRowEditCore();
			BeginLockEndEdit();
			try {
				if(CurrencyManagerPosition == this.tempDeatchedListSourceRow) {
					CurrentControllerRow = NewItemRow;
					return;
				}
				CurrentControllerRow = GetControllerRow(CurrencyManagerPosition);
			}
			finally {
				EndLockEndEdit();
			}
		}
		bool IsFromListChanged() {
			return CheckStackFrame("List_ListChanged", typeof(CurrencyManager));
		}
		bool IsFromCancelEdit() {
			return CheckStackFrame("CancelCurrentEdit", typeof(CurrencyManager));
		}
		bool CheckStackFrame(string methodName, Type type) {
			return StackTraceHelper.CheckStackFrame(methodName, type);
		}
		void OnCurrencyManager_ItemChanged(object sender, ItemChangedEventArgs e) {
			if(syncPositionOnItemChanged) {
				if(CheckStackFrame("FinishAddNew", typeof(DataView))) return;
				this.syncPositionOnItemChanged = false;
				if(IsFromCancelEdit()) {
					CheckEndNewItemRow();
				}
				SyncWithCurrencyManager();
			}
			if(e.Index == -1) {
				if(IsCurrencyResetEventLocked) return;
				if(GetCurrencyManagerListSource() != ListSource) {
					OnCurrencyManagerChanged();
				} else {
					if(Helper.SupportsNotification) return;
					OnBindingListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
				}
			} else {
				if(Helper.SupportsNotification && GetInListItemChanged()) return;
				OnBindingListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, e.Index, e.Index));
			}
		}
		void CheckEndNewItemRow() {
			if(IsNewItemRowEditing) {
				CancelCurrentRowEdit();
			}
		}
		public override void DeleteRow(int controllerRow) {
			if(controllerRow == NewItemRow) {
				CancelCurrentRowEdit();
				return;
			}
			BeginDelete();
			try {
			base.DeleteRow(controllerRow);
		}
			finally {
				EndDelete();
			}
		}
		void InvokeSyncWithCurrencyManager() {
			if(ValidationClientIsAllowBeginInvoke) {
				ValidationClient.BoundControl.BeginInvoke(new MethodInvoker(SyncWithCurrencyManager));
			}
			else {
				SyncWithCurrencyManager();
			}
		}
		void SyncWithCurrencyManager() {
			bool prevEditing = this.currentRowEditing;
			this.currentRowEditing = false;
			try {
				int newRow = GetControllerRow(CurrencyManagerPosition);
				if(IsGroupRowHandle(CurrentControllerRow)) {
					if(GetControllerRowByGroupRow(CurrentControllerRow) == newRow) return;
					GroupRowInfo group = GroupInfo.GetGroupRowInfoByHandle(CurrentControllerRow);
					if(group != null) {
						int res = FindRowLevel(newRow, group.Level);
						if(IsValidControllerRowHandle(res)) newRow = res;
					}
				}
				CurrentControllerRow = newRow;
			}
			finally {
				this.currentRowEditing = prevEditing;
			}
		}
		bool requireSyncCurrencyManager = false;
		protected override void OnBindingListChangingEnd() {
			base.OnBindingListChangingEnd();
			if(this.requireSyncCurrencyManager)
				SyncWithCurrencyManager();
		}
		protected override void OnBindingListChangingStart() {
			base.OnBindingListChangingStart();
			this.requireSyncCurrencyManager = false;
		}
		protected override void OnActionItemMoved(int oldIndex, int newIndex) {
			base.OnActionItemMoved(oldIndex, newIndex);
			this.requireSyncCurrencyManager = true;
		}
		protected override void OnActionItemDeleted(int index) {
			base.OnActionItemDeleted(index);
			this.requireSyncCurrencyManager = true;
		}
		protected override void OnActionItemAdded(int index) {
			base.OnActionItemAdded(index);
			this.requireSyncCurrencyManager = true;
		}
		int deleteProgress = 0;
		int deleteSavedVisibleIndex = 0;
		bool IsDeletingRow { get { return deleteProgress != 0; } }
		void BeginDelete() {
			if(this.deleteProgress++ == 0) {
				this.deleteSavedVisibleIndex = GetVisibleIndex(CurrentControllerRow);
			}
		}
		void EndDelete() {
			if(--this.deleteProgress == 0) {
				int row = GetControllerRowHandle(deleteSavedVisibleIndex);
				if(!IsValidControllerRowHandle(row)) row = GetControllerRowHandle(deleteSavedVisibleIndex - 1);
				if(!IsValidControllerRowHandle(row)) row = GetControllerRowHandle(VisibleCount - 1);
				if(!IsValidControllerRowHandle(row)) row = 0;
				CurrentControllerRow = row;
				SyncCurrentRow();
			}
		}
		public override void DeleteRows(int[] controllerRows) {
			int current = CurrentControllerRow;
			BeginDelete();
			try {
				LockCurrencyResetEvent();
				try {
					base.DeleteRows(controllerRows);
				}
				finally {
					UnlockCurrencyResetEvent();
				}
			}
			finally {
				EndDelete();
			}
		}
		public override bool IsValidControllerRowHandle(int controllerRowHandle) {
			if(controllerRowHandle == NewItemRow) {
				return IsReady && IsNewItemRowEditing;
			}
			return base.IsValidControllerRowHandle(controllerRowHandle);
		}
		int tempDeatchedListSourceRow = InvalidRow;
		public override void AddNewRow() {
			if(!IsReady || CurrencyManager == null || !AllowNew) return;
			if(IsCurrentRowEditing) {
				if(!EndCurrentRowEdit()) return;
			}
			try {
				LockCurrencyResetEvent();
				BeginLockEndEdit();
				this.newItemRowEditing = true;
				SetTempDetachedListSourceRow(CurrencyManager.List.Count);
				Helper.SetDetachedListSourceRow(CurrencyManager.List.Count);
				OnCurrencyManagerAddNew();
				Helper.RaiseOnStartNewItemRow();
			}
			catch {
				this.newItemRowEditing = false;
				Helper.SetDetachedListSourceRow(DataController.InvalidRow);
				throw;
			}
			finally {
				SetTempDetachedListSourceRow(InvalidRow);
				UnlockCurrencyResetEvent();
				EndLockEndEdit();
			}
			BeginCurrentRowEdit();
		}
		protected virtual void OnCurrencyManagerAddNew() {
			CurrencyManager.AddNew();
		}
		protected virtual void SetTempDetachedListSourceRow(int listSourceRow) {
			this.tempDeatchedListSourceRow = listSourceRow;
		}
		internal bool finishingNewItemRowEdit = false;
		public override bool EndCurrentRowEdit(bool force) {
			if(!IsReady || IsLockEndEdit) return true;
			BeginLockEndEdit();
			bool forceFinishing = false;
			bool prevEditing = IsNewItemRowEditing || IsCurrentRowEditing;
			bool fireEvents = true;
			int currentRow = CurrentControllerRow;
			object row = GetRow(CurrentControllerRow);
			bool prevNewItemRowEditing = IsNewItemRowEditing;
			fireEvents = force || IsCurrentRowEditing;
			force = true;
			try {
				this.finishingNewItemRowEdit = IsNewItemRowEditing;
				try {
					if(IsCurrentRowEditing) {
						ValuesKeeper.SaveValues();
						if(!OnCurrentRowValidating()) return false;
					} else {
						if(!force || currentRow == InvalidRow) return true;
						if(fireEvents) ValuesKeeper.SaveValues();
						forceFinishing = true;
					}
					CurrencyManagerEndEdit();
					if(!forceFinishing) EndRowEdit(CurrentControllerRow);
				}
				catch(Exception e) {
					this.finishingNewItemRowEdit = false;
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
				if(!updateResult) {
					CurrentControllerRow = FindRowByRowValue(row);
				}
				return updateResult;
			}
			finally {
				this.finishingNewItemRowEdit = false;
				EndLockEndEdit();
			}
		}
		protected virtual void CurrencyManagerEndEdit() {
			if(!IsReady) return;
			LockPositionChange();
			try {
				if(CurrencyManager != null && CurrencyManager.Position >= 0) CurrencyManager.EndCurrentEdit();
			} finally {
				UnlockPositionChange();
			}
		}
		int cancelingRowEdit = 0;
		public override void CancelCurrentRowEdit() {
			if(!IsReady) return;
			bool prevNewItemRowEditing = IsNewItemRowEditing;
			LockPositionChange();
			try {
			LockCurrencyResetEvent();
			this.cancelingRowEdit++;
			try {
				CurrencyManager.CancelCurrentEdit();
				}
				finally {
				this.cancelingRowEdit--;
				UnlockCurrencyResetEvent();
			}
			}
			finally {
				UnlockPositionChange();
			}
			StopCurrentRowEditCore();
			SyncWithCurrencyManager();
			if(!prevNewItemRowEditing) RaiseCurrentRowChanged();
		}
		protected internal override void OnStartNewItemRow() { 
			base.OnStartNewItemRow();
			BeginLockEndEdit();
			try {
				if(CurrentControllerRow == InvalidRow) CurrentControllerRow = NewItemRow;
				ValidationClient.OnStartNewItemRow();
			}
			finally {
				EndLockEndEdit();
			}
		}
		protected internal override void OnEndNewItemRow() { 
			base.OnEndNewItemRow();
			ValidationClient.OnEndNewItemRow();
		}
		protected override void StopCurrentRowEditCore() {
			base.StopCurrentRowEditCore();
			if(!IsCurrencyResetEventLocked)	Helper.RaiseOnEndNewItemRow();
		}
		protected internal override void OnItemDeleting(int listSourceRow) {
			base.OnItemDeleting(listSourceRow);
#if DXWhibey
			if(IsDeletingRow) return; 
#endif
			int controllerRow = GetControllerRow(listSourceRow);
			if(IsValidControllerRowHandle(controllerRow) && controllerRow == CurrentControllerRow) {
				int visibleIndex = GetVisibleIndex(controllerRow);
				if(visibleIndex >= VisibleCount - 1)
					visibleIndex = visibleIndex - 1;
				else
					visibleIndex = visibleIndex + 1;
				visibleIndex = Math.Max(0, visibleIndex);
				CurrentControllerRow = GetControllerRowHandle(visibleIndex);
			}
			if(Helper.SupportsNotification) {
				return;
			}
			if(CurrencyManagerPosition > 0 && CurrencyManagerPosition > CurrencyManager.Count - 2) {
				int newRow = Math.Max(CurrentControllerRow - 1, 0);
				if(newRow == CurrentControllerRow) newRow = 1;
				CurrentControllerRow = Math.Max(0, Math.Min(VisibleCount - 1, newRow));
			}
		}
		protected internal override void OnItemDeleted(int listSourceRow) {
			base.OnItemDeleted(listSourceRow);
			if(IsDeletingRow) return;
			int currencyControllerRow = GetControllerRow(CurrencyManagerPosition);
			if(CurrentControllerRow != currencyControllerRow) {
				CurrentControllerRow = currencyControllerRow;
			}
			if(IsValidControllerRowHandle(CurrentControllerRow)) return;
			if(VisibleCount >= 0) CurrentControllerRow = GetVisibleIndex(VisibleCount - 1);
		}
		protected override bool CanFilterAddedRow(int listSourceRow) { 
			return true;
		}
		protected override void OnItemChanged(ListChangedEventArgs e, DataControllerChangedItemCollection changedItems) {
			if(IsCurrentRowEditing) {
				int listSourceRow = GetChangedListSourceRow(e);
				if(CurrencyManagerPosition == listSourceRow) {
					changedItems.AddItem(GetControllerRow(listSourceRow), NotifyChangeType.ItemChanged, null);
					return;
				}
			}
			base.OnItemChanged(e, changedItems);
		}
		delegate void ListChangedDelegate(ListChangedEventArgs e);
		static bool _DisableThreadingProblemsDetection = false;
		[Obsolete("Threading problems detection disabled")]
		public static bool DisableThreadingProblemsDetection {
			get { return _DisableThreadingProblemsDetection; }
			set { _DisableThreadingProblemsDetection = value; }
		}
		static void ThrowCrossThreadException() {
			throw new InvalidOperationException("Cross thread operation detected. To suppress this exception, set DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true");
		}
		protected internal override void RaiseOnBindingListChanged(ListChangedEventArgs e) {
			if(ValidationClient.BoundControl != null && ValidationClient.BoundControl.InvokeRequired) {
				if(!_DisableThreadingProblemsDetection) {
					if(ValidationClientIsAllowBeginInvoke)
					ValidationClient.BoundControl.BeginInvoke(new MethodInvoker(ThrowCrossThreadException));
					ThrowCrossThreadException();
				}
				if(ValidationClientIsAllowBeginInvoke)
				ValidationClient.BoundControl.BeginInvoke(new ListChangedDelegate(OnBindingListChanged), new object[] { e });
				System.Threading.Thread.Sleep(1);
				return;
			}
			RaiseOnBeforeListChanged(e);
			try {
				if(e.ListChangedType == ListChangedType.Reset && IsCurrencyResetEventLocked && IsNewItemRowEditing) return;
				OnBindingListChanged(e);
				if(this.finishingNewItemRowEdit) return;
				CurrentControllerRow = CurrentControllerRow;
			}
			finally {
				RaiseOnListChanged(e);
			}
		}
		protected bool IsCurrencyResetEventLocked { get { return lockCurrencyResetEvent != 0; } }
		protected void LockCurrencyResetEvent() {
			this.lockCurrencyResetEvent ++;
		}
		protected void UnlockCurrencyResetEvent() {
			this.lockCurrencyResetEvent --;
		}
	}
	public static class StackTraceHelper {
		public static bool CheckStackFrame(string methodName, Type type, int start, int maxCount) {
			System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
			if(st.FrameCount > 6) {
				for(int n = start; n < Math.Min(maxCount, st.FrameCount); n++) {
					System.Diagnostics.StackFrame sf = st.GetFrame(n);
					System.Reflection.MemberInfo mi = sf.GetMethod();
					if(mi != null && mi.Name == methodName && (type == null || mi.DeclaringType.Equals(type))) return true;
				}
			}
			return false;
		}
		public static bool CheckStackFrame(string methodName, Type type) {
			return CheckStackFrame(methodName, type, 3, 15);
		}
		public static bool CheckStackFrameByName(string methodName, string typeName, int start, int maxCount) {
			System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
			if(st.FrameCount > 6) {
				for(int n = start; n < Math.Min(maxCount, st.FrameCount); n++) {
					System.Diagnostics.StackFrame sf = st.GetFrame(n);
					System.Reflection.MemberInfo mi = sf.GetMethod();
					if(mi != null && mi.Name == methodName && (mi.DeclaringType.Name == typeName)) return true;
				}
			}
			return false;
		}
		public static bool CheckStackFrameByName(string methodName, string typeName) {
			return CheckStackFrameByName(methodName, typeName, 3, 15);
		}
	}
}
