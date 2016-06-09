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
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
#if !SL
using System.Data;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.XtraScheduler.Data {
	#region DataManager<T>
	public abstract class DataManager<T> : IDisposable, IBatchUpdateable, IBatchUpdateHandler where T : IPersistentObject {
		#region Fields
		SchedulerDataController dataController;
		BatchUpdateHelper batchUpdateHelper;
		bool isDisposed;
		bool autoReloadWasRaised;
		#endregion
		protected DataManager() {
			this.batchUpdateHelper = new BatchUpdateHelper(this);
			this.dataController = CreateDataController();
			SubscribeDataControllerEvents();
		}
		#region Properties
		internal bool IsDisposed { get { return isDisposed; } }
		internal SchedulerDataController DataController { get { return dataController; } }
		protected internal IList ListSource { get { return dataController.ListSource; } set { dataController.ListSource = value; } }
		protected internal string KeyFieldName { get { return dataController.KeyFieldName; } set { dataController.KeyFieldName = value; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.dataController != null) {
					UnsubscribeDataControllerEvents();
					this.dataController.Dispose();
					this.dataController = null;
				}
				this.batchUpdateHelper = null;
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~DataManager() {
			Dispose(false);
		}
		#endregion
		#region IBatchUpdateable implementation
		public void BeginUpdate() {
			batchUpdateHelper.BeginUpdate();
		}
		public void EndUpdate() {
			batchUpdateHelper.EndUpdate();
		}
		public void CancelUpdate() {
			batchUpdateHelper.CancelUpdate();
		}
		BatchUpdateHelper IBatchUpdateable.BatchUpdateHelper { get { return batchUpdateHelper; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsUpdateLocked { get { return batchUpdateHelper.IsUpdateLocked; } }
		#endregion
		#region IBatchUpdateHandler implementation
		void IBatchUpdateHandler.OnFirstBeginUpdate() {
			autoReloadWasRaised = false;
		}
		void IBatchUpdateHandler.OnBeginUpdate() {
		}
		void IBatchUpdateHandler.OnEndUpdate() {
		}
		void IBatchUpdateHandler.OnLastEndUpdate() {
			if (autoReloadWasRaised)
				RaiseAutoReload(this, new ListChangedEventArgs(ListChangedType.Reset, 0, 0));
		}
		void IBatchUpdateHandler.OnCancelUpdate() {
		}
		void IBatchUpdateHandler.OnLastCancelUpdate() {
		}
		#endregion
		#region Events
		ListChangedEventHandler autoReload;
		public event ListChangedEventHandler AutoReload { add { autoReload += value; } remove { autoReload -= value; } }
		protected internal virtual void RaiseAutoReload(object sender, ListChangedEventArgs e) {
			if (autoReload != null)
				autoReload(this, e);
		}
		#endregion
		public void TryRePopulateColumns() {
			DataController.TryRePopulateColumns();
		}
		protected internal virtual void SubscribeDataControllerEvents() {
			DataController.BindingListChanged += new ListChangedEventHandler(OnAutoReload);
		}
		protected internal virtual void UnsubscribeDataControllerEvents() {
			DataController.BindingListChanged -= new ListChangedEventHandler(OnAutoReload);
		}
		protected internal virtual void OnAutoReload(object sender, ListChangedEventArgs e) {
			if (IsUpdateLocked) {
				autoReloadWasRaised = true;
				return;
			}
			RaiseAutoReload(sender, e);
		}
		#region CommitNewObject        
		public virtual void CommitNewObject(T obj, MappingCollection mappings) {
			UnsubscribeDataControllerEvents();
			try {
				BeginInsertNewObjectRow(obj);
				CommitExistingObjectCore(obj, mappings);
				EndInsertNewObjectRow(obj);
			}
			finally {
				SubscribeDataControllerEvents();
			}
		}
		#endregion
		public virtual void UpdateAfterInsert(T obj, MappingCollection mappings) {
			LoadExistingObjectProperties(obj, mappings);
		}
		#region CommitExistingObject
		public virtual void CommitExistingObject(T obj, MappingCollection mappings) {
			UnsubscribeDataControllerEvents();
			try {
				BeginExistingObjectRowEdit(obj);
				CommitExistingObjectCore(obj, mappings);
				EndExistingObjectRowEdit(obj);
			}
			finally {
				SubscribeDataControllerEvents();
			}
		}
		#endregion
		#region RollbackExistingObject
		public virtual void RollbackExistingObject(T obj, MappingCollection mappings) {
			UnsubscribeDataControllerEvents();
			try {
				LoadExistingObjectProperties(obj, mappings);
			}
			finally {
				SubscribeDataControllerEvents();
			}
		}
		#endregion
		#region DeleteExistingObject
		public virtual void DeleteExistingObject(T obj, NotificationCollection<T> collection, MappingCollection mappings) {
			UnsubscribeDataControllerEvents();
			try {
				DeleteExistingObjectRow(obj, collection, mappings);
			}
			finally {
				SubscribeDataControllerEvents();
			}
		}
		#endregion
		#region ClearAllObjects
		public virtual void ClearAllObjects(MappingCollection mappings) {
			UnsubscribeDataControllerEvents();
			try {
				ClearAllObjectsCore(mappings);
			}
			finally {
				SubscribeDataControllerEvents();
			}
		}
		#endregion
		protected internal virtual void CommitExistingObjectCore(T obj, MappingCollection mappings) {
			dataController.BeginTrackRowCommit((int)obj.RowHandle, mappings);
			try {
				int count = mappings.Count;
				for (int i = 0; i < count; i++)
					CommitObjectProperty(obj, mappings[i]);
			}
			finally {
				dataController.EndTrackRowCommit((int)obj.RowHandle);
			}
		}
		protected internal virtual void CommitObjectProperty(T obj, MappingBase mapping) {
			object propertyValue = mapping.GetValue(obj);
			if (mapping.CommitToDataSource)
				dataController.SetRowValue((int)obj.RowHandle, mapping.Member, propertyValue);
		}
		#region BeginInsertNewObjectRow
		protected internal virtual void BeginInsertNewObjectRow(T obj) {
			XtraSchedulerDebug.Assert((int)obj.RowHandle == -1);
			dataController.AddNewRow();
			int rowHandle = ListSourceDataController.NewItemRow;
			obj.RowHandle = rowHandle;
		}
		#endregion
		#region EndInsertNewObjectRow
		protected internal virtual void EndInsertNewObjectRow(T obj) {
#if DEBUG
			object row = dataController.GetRow((int)obj.RowHandle);
#endif
			dataController.EndNewRowEdit();
			obj.RowHandle = dataController.ListSource.Count - 1;
#if DEBUG
			if (!XtraSchedulerDebug.SkipInsertionCheck)
				XtraSchedulerDebug.Assert((int)obj.RowHandle == dataController.FindRowByRowValue(row));
#endif
		}
		#endregion
		#region BeginExistingObjectRowEdit
		protected internal virtual void BeginExistingObjectRowEdit(T obj) {
			dataController.ProtectedBeginRowEdit((int)obj.RowHandle);
		}
		#endregion
		#region EndExistingObjectRowEdit
		protected internal virtual void EndExistingObjectRowEdit(T obj) {
			dataController.ProtectedEndRowEdit((int)obj.RowHandle);
		}
		#endregion
		protected internal virtual void PerformDeleteRow(int rowHandle, MappingCollection mappings) {
			dataController.BeforeDeleteRow(rowHandle, mappings);
			dataController.DeleteRow(rowHandle);
		}
		protected internal virtual void DeleteExistingObjectRow(T obj, NotificationCollection<T> collection, MappingCollection mappings) {
			PerformDeleteRow((int)obj.RowHandle, mappings);
			AdjustParentRowHandle(obj, (int)obj.RowHandle);
			RenumberRowHandles(collection, (int)obj.RowHandle);
		}
		protected internal virtual void RenumberRowHandles(NotificationCollection<T> collection, int deletedRowHandle) {
			int count = collection.Count;
			for (int i = 0; i < count; i++)
				AdjustRowHandle(collection[i], deletedRowHandle);
		}
		protected internal virtual void AdjustRowHandle(T obj, object deletedRowHandle) {
			int intRowHandle = (int)obj.RowHandle;
			if (intRowHandle > (int)deletedRowHandle)
				obj.RowHandle = intRowHandle - 1;
		}
		protected internal virtual void AdjustParentRowHandle(T obj, object deletedRowHandle) {
		}
		protected internal virtual void ClearAllObjectsCore(MappingCollection mappings) {
			for (int i = dataController.ListSourceRowCount - 1; i >= 0; i--)
				PerformDeleteRow(i, mappings);
		}
		protected internal virtual void LoadExistingObjectProperties(T obj, MappingCollection mappings) {
			obj.BeginUpdate();
			try {
				int count = mappings.Count;
				for (int i = 0; i < count; i++)
					LoadObjectProperty(obj, mappings[i]);
			}
			finally {
				obj.CancelUpdate();
			}
		}
		protected internal virtual void LoadObjectProperty(T obj, MappingBase mapping) {
			object propertyValue = dataController.GetRowValue((int)obj.RowHandle, mapping.Member);
			mapping.SetValue(obj, propertyValue);
		}
		protected virtual SchedulerDataController CreateDataController() {
			return new SchedulerDataController();
		}
		protected internal virtual int SourceObjectCount { get { return dataController.ListSourceRowCount; } }
		protected internal virtual object GetSourceObjectHandle(int sourceObjectIndex) {
			return sourceObjectIndex;
		}
		#region Direct Data Access Methods
		public virtual object GetObjectRow(T obj) {
			if (obj == null)
				Exceptions.ThrowArgumentException("obj", obj);
			return dataController.GetRow((int)obj.RowHandle);
		}
		public virtual object GetObjectValue(T obj, string columnName) {
			if (obj == null)
				Exceptions.ThrowArgumentException("obj", obj);
			if (String.IsNullOrEmpty(columnName))
				Exceptions.ThrowArgumentException("columnName", columnName);
			return dataController.GetRowValue((int)obj.RowHandle, columnName);
		}
		public virtual void SetObjectValue(T obj, string columnName, object val) {
			if (obj == null)
				Exceptions.ThrowArgumentException("obj", obj);
			if (String.IsNullOrEmpty(columnName))
				Exceptions.ThrowArgumentException("columnName", columnName);
			UnsubscribeDataControllerEvents();
			try {
				dataController.SetRowValue((int)obj.RowHandle, columnName, val);
				dataController.ProtectedEndRowEdit((int)obj.RowHandle);
			}
			finally {
				SubscribeDataControllerEvents();
			}
		}
		#endregion
		protected internal virtual object GetRowValue(object rowHandle, string columnName) {
			return DataController.GetRowValue((int)rowHandle, columnName);
		}
		protected internal DataFieldInfoCollection GetFieldInfos() {
			return DataController.GetFieldInfos();
		}
	}
	#endregion
	#region SchedulerDataController
	public class SchedulerDataController : ListSourceDataController {
		readonly string[] DummyPropertyDescriptors = new string[] { "SimpleListPropertyDescriptor", "NoQueryablePropertyDescriptor",
			"NoEnumerablePropertyDescriptor", "NoSourcePropertyDescriptor", "NoQueryPropertyDescriptor" };
		#region Fields
		string keyFieldName = String.Empty;
		bool isRepopulateColumnsNeeded;
		bool isDummySource;
		#endregion
		#region Events
		ListChangedEventHandler onBindingListChanged;
		public event ListChangedEventHandler BindingListChanged { add { onBindingListChanged += value; } remove { onBindingListChanged -= value; } }
		protected
#if DXCommon
		internal
#endif
		override void RaiseOnBindingListChanged(ListChangedEventArgs e) {
			base.RaiseOnBindingListChanged(e);
			if (!IsUpdateLocked) {
				if (onBindingListChanged != null)
					onBindingListChanged(this, e);
			}
		}
		#endregion
		#region Properties
		public virtual string KeyFieldName { get { return keyFieldName; } set { keyFieldName = value; } }
		#endregion
		protected internal virtual void ProtectedBeginRowEdit(int rowIndex) {
			base.BeginRowEdit(rowIndex);
		}
		protected internal virtual void ProtectedEndRowEdit(int rowIndex) {
			base.EndRowEdit(rowIndex);
		}
		protected internal virtual void BeforeDeleteRow(int rowIndex, MappingCollection mappings) {
		}
		protected internal virtual void BeginTrackRowCommit(int rowIndex, MappingCollection mappings) {
		}
		protected internal virtual void EndTrackRowCommit(int rowIndex) {
		}
		protected internal virtual DataFieldInfoCollection GetFieldInfos() {
			DataFieldInfoCollection result = new DataFieldInfoCollection();
			int count = Columns.Count;
			for (int i = 0; i < count; i++) {
				DataColumnInfo info = Columns[i];
				result.Add(new DataFieldInfo(info.Name, info.Type));
			}
			return result;
		}
		public override int EndNewRowEdit() {
			int newRowIndex = base.EndNewRowEdit();
			if (Helper.CancelAddNew != null)
				Helper.CancelAddNew.EndNew(newRowIndex);
			return newRowIndex;
		}
		protected override void OnListSourceChanged() {
			base.OnListSourceChanged();
			ScheduleRepopulateColumnsIfNeeded();
		}
		void ScheduleRepopulateColumnsIfNeeded() {
			if (IsDataSourceEmpty()) {
				SetRepopulateColumnsConditions(true);
			}
		}
		bool ShouldTryRepopulateColumns() {
			return IsDataSourceEmpty() || this.isDummySource;
		}
		void SetRepopulateColumnsConditions(bool value) {
			if (value) {
				this.isRepopulateColumnsNeeded = true;
				this.isDummySource = false;
				if (Columns.Count == 1) {
					Type propertyDescriptorType = Columns[0].PropertyDescriptor.GetType();
					foreach (string dummyPropertyDescriptor in DummyPropertyDescriptors) {
						if (propertyDescriptorType.Name == dummyPropertyDescriptor) {
							this.isDummySource = true;
							break;
						}
					}
				}
			}
			else {
				this.isRepopulateColumnsNeeded = false;
				this.isDummySource = false;
			}
		}
		bool IsDataSourceEmpty() {
			return ListSource != null && ListSource.Count == 0;
		}
		public void TryRePopulateColumns() {
			if (!this.isRepopulateColumnsNeeded) return;
			if (IsDataSourceEmpty())
				return;
			if (!ShouldTryRepopulateColumns()) return;
			RePopulateColumns();
			SetRepopulateColumnsConditions(false);
		}
	}
	#endregion
	public interface ISchedulerUnboundDataKeeper {
		IList List { get; }
		void Activate(MappingCollection mappings);
	}
#if !SL
	#region UnboundDataKeeper
	public class UnboundDataKeeper : ISchedulerUnboundDataKeeper, IDisposable {
		#region Fields
		DataTable table;
		bool isDisposed;
		#endregion
		#region Properties
		public IList List { get { return table != null ? table.DefaultView : null; } }
		internal DataTable Table { get { return table; } }
		internal bool IsDisposed { get { return isDisposed; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				DeleteTable();
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~UnboundDataKeeper() {
			Dispose(false);
		}
		#endregion
		internal void CreateTable(MappingCollection mappings) {
			this.table = new DataTable();
			this.table.Locale = CultureInfo.InvariantCulture;
			table.BeginInit();
			try {
				DataColumnCollection columns = table.Columns;
				int count = mappings.Count;
				for (int i = 0; i < count; i++) {
					MappingBase mapping = mappings[i];
					try {
						columns.Add(mapping.Member, mapping.Type);
					}
					catch {
					}
				}
			}
			finally {
				table.EndInit();
			}
		}
		void DeleteTable() {
			if (table != null) {
				table.Dispose();
				table = null;
			}
		}
		public void Activate(MappingCollection mappings) {
			DeleteTable();
			CreateTable(mappings);
		}
		public void Deactivate() {
			DeleteTable();
		}
	}
	#endregion
#else
	public class UnboundDataKeeper : ISchedulerUnboundDataKeeper, IDisposable {
	#region Fields
		List<object> list;
		bool isDisposed;
		#endregion
	#region Properties
		public IList List { get { return list; } }
		internal bool IsDisposed { get { return isDisposed; } }
		#endregion
	#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				list = null;
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~UnboundDataKeeper() {
			Dispose(false);
		}
		#endregion
		public void Activate(MappingCollection mappings) {
			this.list = new List<object>();
		}
		public void Deactivate() {
			this.list = null;
		}
	}
#endif
}
