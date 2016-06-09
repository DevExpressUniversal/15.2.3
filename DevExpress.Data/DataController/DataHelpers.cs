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
using System.Globalization;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using DevExpress.Data.Linq;
using System.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using System.Diagnostics;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Collections;
using DevExpress.Utils;
#if !SILVERLIGHT
using System.Data;
using DevExpress.Data.Details;
using System.Collections.Specialized;
#else
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
using DevExpress.Data.Browsing;
#endif
using System.Linq.Expressions;
namespace DevExpress.Data.Helpers {
	public interface INotificationProvider : IDisposable {
		bool IsSupportNotifications(object list);
		void SubscribeNotifications(ListChangedEventHandler handler);
		void UnsubscribeNotifications(ListChangedEventHandler handler);
		INotificationProvider Clone(object list);
	}
	public class EmptyNotificationProvider : INotificationProvider {
		bool INotificationProvider.IsSupportNotifications(object list) { return false; }
		void INotificationProvider.SubscribeNotifications(ListChangedEventHandler handler) { }
		void INotificationProvider.UnsubscribeNotifications(ListChangedEventHandler handler) { }
		public virtual void Dispose() { }
		public INotificationProvider Clone(object list) { return this; }
	}
	public class BindingListNotificationProvider : INotificationProvider {
		IBindingList list;
		#region INotificationProvider Members
		bool INotificationProvider.IsSupportNotifications(object list) {
			IBindingList ilist = list as IBindingList;
			return ilist != null && ilist.SupportsChangeNotification;
		}
		void INotificationProvider.SubscribeNotifications(ListChangedEventHandler handler) {
			if(list != null) list.ListChanged += handler;
		}
		void INotificationProvider.UnsubscribeNotifications(ListChangedEventHandler handler) {
			if(list != null) list.ListChanged -= handler;
		}
		public virtual void Dispose() {
			this.list = null;
		}
		public INotificationProvider Clone(object list) {
			BindingListNotificationProvider res = new BindingListNotificationProvider();
			res.list = list as IBindingList;
			return res;
		}
		#endregion
	}
#if !SL
	public class NotifyCollectionChangedProvider : INotificationProvider {
		IList list;
		bool supportPropertiesChanged = false;
		class EventInfo {
			public NotifyCollectionChangedEventHandler CCHandler;
			public ListChangedEventHandler Handler;
		}
		List<EventInfo> events;
		#region INotificationProvider Members
		bool INotificationProvider.IsSupportNotifications(object list) {
			INotifyCollectionChanged cc = list as INotifyCollectionChanged;
			return (cc != null);
		}
		void INotificationProvider.SubscribeNotifications(ListChangedEventHandler handler) {
			INotifyCollectionChanged cc = this.list as INotifyCollectionChanged;
			if(cc != null) {
				NotifyCollectionChangedEventHandler ccHandler = (sender, e) =>
				{
					ListChangedType type = ConvertType(e.Action);
					IList items = e.NewItems != null && e.NewItems.Count > 0 ? e.NewItems : e.OldItems;
					SubscribePropertiesChanged(e, items);
					if(e.Action == NotifyCollectionChangedAction.Reset && supportPropertiesChanged) 
						SubscribePropertiesChanged(this.list, false);
					if(items == null || items.Count < 2) {
						handler(list, new ListChangedEventArgs(type, e.NewStartingIndex, e.OldStartingIndex));
						return;
					}
					for(int n = items.Count - 1; n >= 0; n--) {
						handler(list, new ListChangedEventArgs(type, e.NewStartingIndex < 0 ? -1 : e.NewStartingIndex + n, e.OldStartingIndex < 0 ? -1 : e.OldStartingIndex + n));
					}
				};
				if(events == null) events = new List<EventInfo>();
				events.Add(new EventInfo() { Handler = handler, CCHandler = ccHandler });
				cc.CollectionChanged += ccHandler;
			}
		}
		void SubscribePropertiesChanged(IList items, bool unsubscribeOnly) {
			if(items == null) return;
			for(int n = 0; n < items.Count; n++) {
				INotifyPropertyChanged pc = items[n] as INotifyPropertyChanged;
				if(pc != null) {
					pc.PropertyChanged -= new PropertyChangedEventHandler(OnItemPropertyChanged);
					if(!unsubscribeOnly)
						pc.PropertyChanged += new PropertyChangedEventHandler(OnItemPropertyChanged);
				}
			}
		}
		void SubscribePropertiesChanged(NotifyCollectionChangedEventArgs e, IList items) {
			if(items == null || !supportPropertiesChanged) return;
			if(e.Action == NotifyCollectionChangedAction.Add) {
				SubscribePropertiesChanged(items, false);
			}
			if(e.Action == NotifyCollectionChangedAction.Remove) {
				SubscribePropertiesChanged(items, true);
				return;
			}
			if(e.Action == NotifyCollectionChangedAction.Replace) {
				SubscribePropertiesChanged(e.OldItems, true);
				SubscribePropertiesChanged(e.NewItems, false);
			}
		}
		int lastChangedIndex = -1;
		PropertyDescriptorCollection itemTypeProperties;
		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(list.Count < lastChangedIndex && lastChangedIndex > -1) {
				object item = list[lastChangedIndex];
				if(item != sender) lastChangedIndex = -1;
			}
			else {
				lastChangedIndex = -1;
			}
			if(lastChangedIndex == -1) lastChangedIndex = list.IndexOf(sender);
			if(lastChangedIndex == -1) return;
			if(events != null && events.Count > 0) {
				if(this.itemTypeProperties == null) {
					this.itemTypeProperties = TypeDescriptor.GetProperties(sender.GetType());
				}
				PropertyDescriptor propDesc = this.itemTypeProperties.Find(e.PropertyName, true);
				events[0].Handler(list, new ListChangedEventArgs(ListChangedType.ItemChanged, lastChangedIndex, propDesc));
			}
		}
		ListChangedType ConvertType(NotifyCollectionChangedAction action) {
			switch(action) {
				case NotifyCollectionChangedAction.Add: return ListChangedType.ItemAdded;
				case NotifyCollectionChangedAction.Move: return ListChangedType.ItemMoved;
				case NotifyCollectionChangedAction.Remove: return ListChangedType.ItemDeleted;
				case NotifyCollectionChangedAction.Replace: return ListChangedType.ItemChanged;
				case NotifyCollectionChangedAction.Reset: return ListChangedType.Reset;
			}
			return ListChangedType.Reset;
		}
		void INotificationProvider.UnsubscribeNotifications(ListChangedEventHandler handler) {
			INotifyCollectionChanged cc = list as INotifyCollectionChanged;
			if(cc != null) {
				if(events == null) return;
				for(int n = 0; n < events.Count; n++) {
					EventInfo ei = events[n];
					if(ei.Handler == handler) {
						cc.CollectionChanged -= ei.CCHandler;
						events.RemoveAt(n);
						break;
				}
			}
		}
		}
		INotificationProvider INotificationProvider.Clone(object list) {
			NotifyCollectionChangedProvider res = new NotifyCollectionChangedProvider();
			res.list = list as IList;
			res.supportPropertiesChanged = this.supportPropertiesChanged;
			return res;
		}
		#endregion
		public bool SupportPropertiesChanged { get { return supportPropertiesChanged; } set { supportPropertiesChanged = value; } }
		void IDisposable.Dispose() {
			if(events != null) {
				foreach(EventInfo ei in events) {
					INotifyCollectionChanged cc = this.list as INotifyCollectionChanged;
					cc.CollectionChanged -= ei.CCHandler;
				}
				events.Clear();
			}
			if(SupportPropertiesChanged && list != null) SubscribePropertiesChanged(this.list, true);
			this.list = null;
		}
	}
#endif
	public class DataControllerNotificationProviders {
		static DataControllerNotificationProviders _default;
		public static DataControllerNotificationProviders Default {
			get {
				if(_default == null) _default = new DataControllerNotificationProviders();
				return _default;
			}
		}
		List<INotificationProvider> providers = new List<INotificationProvider>();
		public DataControllerNotificationProviders() {
			providers.Add(new BindingListNotificationProvider());
#if !SL            
			providers.Add(new NotifyCollectionChangedProvider());
#endif
		}
		public void AddProvider(INotificationProvider provider) {
			foreach(INotificationProvider p in providers) {
				if(p.GetType().Equals(provider.GetType())) return;
			}
			providers.Add(provider);
		}
		public void RemoveProvider(INotificationProvider provider) {
			for(int n = 0; n < providers.Count; n++) {
				INotificationProvider p = providers[n];
				if(p.GetType().Equals(provider.GetType())) {
					providers.RemoveAt(n);
					return;
				}
			}
		}
		public INotificationProvider FindProvider(object list) {
			if(list == null) return null;
			for(int n = 0; n < providers.Count; n++) {
				INotificationProvider res = providers[n];
				if(res.IsSupportNotifications(list)) return res.Clone(list);
			}
			return new EmptyNotificationProvider();
		}
	}
	public interface IDXCloneable {
		object DXClone();
	}
	public class MultiThreadClient : IDataControllerThreadClient {
		List<IDataControllerThreadClient> listeners;
		public MultiThreadClient() {
		}
		public List<IDataControllerThreadClient> Listeners {
			get {
				if(listeners == null) listeners = new List<IDataControllerThreadClient>();
				return listeners;
			}
		}
		#region IDataControllerThreadClient Members
		void IDataControllerThreadClient.OnAsyncBegin() {
			for(int n = 0; n < Listeners.Count; n++) {
				Listeners[n].OnAsyncBegin();
			}
		}
		void IDataControllerThreadClient.OnAsyncEnd() {
			for(int n = 0; n < Listeners.Count; n++) {
				Listeners[n].OnAsyncEnd();
			}
		}
		void IDataControllerThreadClient.OnRowLoaded(int controllerRowHandle) {
			for(int n = 0; n < Listeners.Count; n++) {
				Listeners[n].OnRowLoaded(controllerRowHandle);
			}
		}
		void IDataControllerThreadClient.OnTotalsReceived() {
			for(int n = 0; n < Listeners.Count; n++) {
				Listeners[n].OnTotalsReceived();
			}
		}
		#endregion
		public void Add(IDataControllerThreadClient client) {
			if(!Listeners.Contains(client)) Listeners.Add(client);
		}
		public void Remove(IDataControllerThreadClient client) {
			if(Listeners.Contains(client)) Listeners.Remove(client);
		}
	}
	public enum ColumnServerActionType { Sort, Group, Filter, Summary };
	public interface IColumnsServerActions {
		bool AllowAction(string fieldName, ColumnServerActionType action);
	}
	public class BaseDataControllerHelper : IRelationListEx, IDisposable {
		int detachedListSourceRow = DataController.InvalidRow;
		DataControllerBase controller;
		Dictionary<string, DataColumnInfo> savedColumns;
		PropertyDescriptorCollection descriptorCollection;
		bool newItemRowAdding = false;
		bool addNewRowProcess = false;
		INotificationProvider notificationProvider;
		public BaseDataControllerHelper(DataControllerBase controller) {
			this.savedColumns = new Dictionary<string, DataColumnInfo>();
			this.controller = controller;
		}
		public virtual void Dispose() {
			if(NotificationProvider != null) NotificationProvider.Dispose();
			notificationProvider = null;
		}
		protected internal virtual int DetachedListSourceRow {
			get { return detachedListSourceRow; }
			set { detachedListSourceRow = value; }
		}
		public object AddNewRow() {
			if(!AllowNew) return null;
			if(this.addNewRowProcess) return null;
			this.addNewRowProcess = true;
			try {
				return AddNewRowCore();
			}
			finally {
				this.addNewRowProcess = false;
			}
		}
		protected virtual object AddNewRowCore() { return null; }
		public bool IsAddNewRowProcess { get { return addNewRowProcess; } }
		public virtual bool CaseSensitive { get { return false; } }
		public int DetachedCount { get { return DetachedListSourceRow != DataController.InvalidRow ? 1 : 0; } }
		public virtual int Count { get { return List == null ? 0 : Math.Max(0, List.Count - DetachedCount); } }
		public DataControllerBase Controller { get { return controller; } }
		public IList List { get { return Controller.ListSource; } }
		public INotificationProvider NotificationProvider {
			get {
				if(List == null) {
					notificationProvider = null;
					return null;
				}
				if(notificationProvider != null) return notificationProvider;
				notificationProvider = Controller.GetNotificationProviders().FindProvider(List);
				return notificationProvider;
			}
		}
		public ITypedList TypedList { get { return List as ITypedList; } }
		public ICancelAddNew CancelAddNew { get { return List as ICancelAddNew; } }
		public IBindingList BindingList { get { return List as IBindingList; } }
		public IEditableCollectionView EditableView { 
			get {
				if(List is ISupportEditableCollectionView)
					return ((ISupportEditableCollectionView)List).IsSupportEditableCollectionView ? (IEditableCollectionView)List : null;
				return List as IEditableCollectionView; 
			} 
		}
		public DataColumnInfoCollection Columns { get { return Controller.Columns; } }
		public DataColumnInfoCollection DetailColumns { get { return Controller.DetailColumns; } }
		public virtual object GetRowKey(int listSourceRow) {
#if !SL
			IListServer listKey = List as IListServer;
			if(listKey != null) return listKey.GetRowKey(listSourceRow);
#endif
			return GetRow(listSourceRow);
		}
		public virtual IDataErrorInfo GetRowErrorInfo(int listSourceRow) {
			return GetRow(listSourceRow) as IDataErrorInfo;
		}
		public virtual DevExpress.XtraEditors.DXErrorProvider.IDXDataErrorInfo GetRowDXErrorInfo(int listSourceRow) {
			return GetRow(listSourceRow) as DevExpress.XtraEditors.DXErrorProvider.IDXDataErrorInfo;
		}
		public object GetRow(int listSourceRow) { return GetRow(listSourceRow, null); }
		public virtual object GetRow(int listSourceRow, OperationCompleted completed) {
			return null;
		}
		public object GetNewRowValue(int column) { 
			if(!Controller.IsColumnValid(column)) return null;
			if(DetachedListSourceRow != DataController.InvalidRow )
				return GetRowValue(DetachedListSourceRow, column, null);
			if(EditableView != null)
				return GetRowValue(GetNewRow(), column);
			return null;
		}
		public virtual object GetRowValue(object row, int column) {
			DataColumnInfo columnInfo = Columns[column];
			if(row == null || columnInfo.Unbound)
				return null;
			return columnInfo.PropertyDescriptor.GetValue(row);
		}
		protected void SetNewRowValue(object rowObject, DataColumnInfo columnInfo, object val) {
			if(columnInfo == null || columnInfo.ReadOnly || rowObject == null || columnInfo.Unbound) return;
			object convertedValue = columnInfo.ConvertValue(val, true, Controller);
			columnInfo.PropertyDescriptor.SetValue(rowObject, convertedValue);
		}
		public object GetNewRowDetailValue(DataColumnInfo info) {
			if(info == null) return null;
			return DetachedListSourceRow == DataController.InvalidRow ? null : GetRowValueDetail(DetachedListSourceRow, info);
		}
		public object GetNewRow() {
			if(DetachedListSourceRow != DataController.InvalidRow && DetachedListSourceRow <= Count) 
				return GetRow(DetachedListSourceRow);
			if(EditableView != null )
				return EditableView.CurrentAddItem;
			return null;
		}
		public int GetNewItemRowIndex() {
			if(DetachedListSourceRow != DataController.InvalidRow) return DetachedListSourceRow;
			return DataController.InvalidRow;
		}
		public void SetNewRowValue(int column, object val) { 
			if(DetachedListSourceRow != DataController.InvalidRow)
				SetRowValue(DetachedListSourceRow, column, val);
			else if(EditableView != null) {
				SetNewRowValue(GetNewRow(), Columns[column], val);
			}
		}
		public object GetRowValue(int listSourceRow, int column) { return GetRowValue(listSourceRow, column, null); }
		public Delegate GetGetRowValue(DataColumnInfo columnInfo, Type expectedReturnType) {
			if(columnInfo == null)
				throw new ArgumentNullException("columnInfo");
			Columns.ValidateColumnInfo(columnInfo);
			return GetGetRowValueCore(columnInfo, expectedReturnType);
		}
		protected virtual Delegate GetGetRowValueCore(DataColumnInfo columnInfo, Type expectedReturnType) {
			throw new NotImplementedException();
		}
		public virtual object GetRowValue(int listSourceRow, int column, OperationCompleted completed) {
			return null;
		}
		public virtual void SetRowValue(int listSourceRow, int column, object val) { }
		public virtual IList<DataColumnInfo> RePopulateColumns() {
			foreach(DataColumnInfo info in Columns) {
				this.savedColumns[info.Name] = info;
			}
			foreach(DataColumnInfo info in DetailColumns) {
				this.savedColumns[info.Name] = info;
			}
			Columns.Clear();
			DetailColumns.Clear();
			Controller.RaiseBeforePopulateColumns(EventArgs.Empty);
			PopulateColumns();
			List<DataColumnInfo> res = new List<DataColumnInfo>();
			if(this.savedColumns.Count > 0) {
				foreach (DataColumnInfo value in savedColumns.Values)
					res.Add(value);
				this.savedColumns.Clear();
			}
			return res;
		}
		public bool AllowNew { get { return (EditableView != null && EditableView.CanAddNew) || (BindingList != null && BindingList.AllowNew); } }
		  public bool AllowEdit {
	  		get {
				  if (List is ISupportEditableCollectionView)
					  return EditableView != null;
	  			if (BindingList != null)
					  return BindingList.AllowEdit;
				  return true;
	  		}
		  }
		public bool AllowRemove { get { return BindingList == null || BindingList.AllowRemove; 	} }
		public void DeleteRow(int listSourceRow) {
			if(AllowRemove) {
				int controllerRow = Controller.GetControllerRow(listSourceRow);
				if(!Controller.RaiseRowDeleting(listSourceRow)) return;
				Controller.OnItemDeleting(listSourceRow);
				object row = null;
				try {
					row = List[listSourceRow];
					List.RemoveAt(listSourceRow);
					if(!SupportsNotification) OnBindingListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, listSourceRow, listSourceRow));
				} finally {
					Controller.OnItemDeleted(listSourceRow);
				}
				Controller.RaiseRowDeleted(controllerRow, listSourceRow, row);
			}
		}
		public PropertyDescriptorCollection DescriptorCollection {
			get { return descriptorCollection; }
		}
		internal int lastPropertyDescriptorCount = 0;
		public virtual void PopulateColumns() {
			this.lastPropertyDescriptorCount = 0;
			if(List == null) return;
			PropertyDescriptorCollection properties = GetPropertyDescriptorCollection();
			this.descriptorCollection = null;
			if(properties != null) {
				this.lastPropertyDescriptorCount = properties.Count;
				foreach(PropertyDescriptor descriptor in properties)
					PopulateColumn(descriptor);
			}
			ComplexColumnInfoCollection complex = GetComplexColumns();
			if(complex != null) {
				foreach(ComplexColumnInfo complexColumn in complex) {
					PopulateColumn(CreateComplexDescriptor(complexColumn));
				}
			}
			UnboundColumnInfoCollection unboundColumns = GetUnboundColumns();
			if(unboundColumns != null) {
				foreach(UnboundColumnInfo info in unboundColumns) {
					if(Columns[info.Name] != null) continue;
					PopulateColumn(CreateUnboundPropertyDescriptor(info));
				}
			}
			this.descriptorCollection = new PropertyDescriptorCollection(null);
			foreach(DataColumnInfo column in Columns) {
				this.descriptorCollection.Add(column.PropertyDescriptor);
			}
			foreach(DataColumnInfo column in DetailColumns) {
				this.descriptorCollection.Add(column.PropertyDescriptor);
			}
		}
		protected virtual PropertyDescriptor CreateComplexDescriptor(ComplexColumnInfo complexColumn) {
			if(Controller.ComplexUseLinqDescriptors) return new DevExpress.Data.Access.ComplexPropertyDescriptorLinq(Controller, complexColumn.Name);
			return new DevExpress.Data.Access.ComplexPropertyDescriptorReflection(Controller, complexColumn.Name);
		}
		protected virtual Access.UnboundPropertyDescriptor CreateUnboundPropertyDescriptor(UnboundColumnInfo info) {
			return new DevExpress.Data.Access.UnboundPropertyDescriptor(Controller, info);
		}
		public virtual UnboundColumnInfoCollection GetUnboundColumns() {
			if(Controller.DataClient != null) return Controller.DataClient.GetUnboundColumns();
			return null;
		}
		public virtual ComplexColumnInfoCollection GetComplexColumns() {
			if(Controller.DataClient2 != null) return Controller.DataClient2.GetComplexColumns();
			return null;
		}
		bool isSubscribedToEvents;
		protected bool IsSubscribedToEvents {
			get { return isSubscribedToEvents; }
			set { isSubscribedToEvents = value; }
		}
		public virtual void SubscribeEvents() {
			if(!SupportsNotification || Controller.IsUpdateLocked) return;
			SubscribeEventsCore();
		}
		public virtual void UnsubscribeEvents() {
			if(!SupportsNotification || Controller.IsUpdateLocked) return;
			UnsubscribeEventsCore();
		}
		protected void SubscribeEventsCore() {
			if(IsSubscribedToEvents) return;
			Controller.SubscribeListChanged(NotificationProvider, List);
			IsSubscribedToEvents = true;
		}
		protected void UnsubscribeEventsCore() {
			if(!IsSubscribedToEvents) return;
			Controller.UnsubscribeListChanged(NotificationProvider, List);
			IsSubscribedToEvents = false;
		}
		protected virtual bool CanPopulate(PropertyDescriptor descriptor) { return true; } 
		protected virtual bool CanPopulateDetailDescriptor(PropertyDescriptor descriptor) { 
			return descriptor.IsBrowsable; 
		} 
		protected virtual void PopulateColumn(PropertyDescriptor descriptor) {
			if(!CanPopulate(descriptor)) return;
			DataColumnInfo info;
			this.savedColumns.TryGetValue(descriptor.Name, out info);
			if(info == null) {
				info = CreateDataColumn(descriptor);
			} else {
				info.SetPropertyDescriptor(descriptor);
				this.savedColumns.Remove(descriptor.Name);
			}
			if(IsDetailDescriptor(descriptor)) {
				if(CanPopulateDetailDescriptor(descriptor))
					DetailColumns.Add(info);
			}
			else
				Columns.Add(info);
			Controller.OnColumnPopulated(info);
		}
		protected virtual DataColumnInfo CreateDataColumn(PropertyDescriptor descriptor) {
			return new DataColumnInfo(descriptor);
		}
		protected virtual bool IsDetailDescriptor(PropertyDescriptor descriptor) {
			if(typeof(Array).IsAssignableFrom(descriptor.PropertyType)) return false;
			if(typeof(IList).IsAssignableFrom(descriptor.PropertyType)) return true;
			if(!Controller.AllowIEnumerableDetails) return false;
			if(!typeof(IEnumerable).IsAssignableFrom(descriptor.PropertyType)) return false;
			if(typeof(string).IsAssignableFrom(descriptor.PropertyType)) return false;
			return true;
		}
		protected virtual PropertyDescriptorCollection GetPropertyDescriptorCollection() {
			return null;
		}
		protected void OnBindingListChanged(object sender, ListChangedEventArgs e) {
			OnBindingListChanged(e);
		}
		protected internal virtual void OnBindingListChanged(ListChangedEventArgs e) {
			Controller.RaiseOnBindingListChanged(e);
		}
		public bool SupportsNotification { 
			get { 
				return NotificationProvider != null && NotificationProvider.IsSupportNotifications(List) && Controller.AllowNotifications; 
			} 
		}
		public virtual IRelationList RelationList {
			get {
				IRelationList list = List as IRelationList;
				if(list != null) return list;
				return this;
			}
		}
		public virtual IRelationListEx RelationListEx {
			get {
				IRelationListEx list = List as IRelationListEx;
				if(list != null) return list;
				return this;
			}
		}
		protected internal virtual void SetDetachedListSourceRow(int listSourceRow) {
			DetachedListSourceRow = listSourceRow;
		}
		protected internal virtual void RaiseOnStartNewItemRow() {
			this.newItemRowAdding = true;
			Controller.OnStartNewItemRow();
		}
		protected internal virtual void RaiseOnEndNewItemRow() {
			if(!this.newItemRowAdding) return;
			OnEndNewItemRow();
		}
		protected virtual void OnEndNewItemRow() {
			detachedListSourceRow = DataController.InvalidRow;
			this.newItemRowAdding = false;
			if(EditableView != null)
				EditableView.CommitNew();
			Controller.OnEndNewItemRow();
		}
		#region IRelationListEx Members
		string IRelationListEx.GetRelationDisplayName(int listSourceRow, int relationIndex) { return ((IRelationList)this).GetRelationName(listSourceRow, relationIndex); }
		int IRelationList.RelationCount { get { return ((IRelationListEx)this).GetRelationCount(DataController.InvalidRow); } }
		string IRelationList.GetRelationName(int listSourceRow, int relationIndex) { return ""; }
		bool IRelationList.IsMasterRowEmpty(int listSourceRow, int relationIndex) { return false; }
		IList IRelationList.GetDetailList(int listSourceRow, int relationIndex) { 	return null; }
		int IRelationListEx.GetRelationCount(int listSourceRow) { return 0; }
		#endregion
		public virtual object GetRowValueDetail(int listSourceRow, DataColumnInfo detailColumn) { return null; }
		protected internal virtual void UpdateDetachedIndex(object addedRow) {
			if(List == null) return;
			int index = List.IndexOf(addedRow);
			if(index != -1 && detachedListSourceRow != DataController.InvalidRow) 
				detachedListSourceRow = index;
		}
		protected internal virtual void CancelNewItemRow() {
			if(CancelAddNew != null && DetachedListSourceRow != DataController.InvalidRow) 
				CancelAddNew.CancelNew(DetachedListSourceRow);
			if(EditableView != null)
				EditableView.CancelNew();
		}
	}
	public class ListDataControllerHelper 
		: BaseListDataControllerHelper, IRelationListEx 
		{
		public static string UseFirstRowTypeWhenPopulatingColumnsTypeName = DefaultUseFirstRowTypeWhenPopulatingColumnsTypeName;
#if DEBUGTEST
		public
#endif
		const string DefaultUseFirstRowTypeWhenPopulatingColumnsTypeName = "System.ServiceModel.DomainServices.Client.Entity";
		public static PropertyDescriptorCollection GetExpandoObjectProperties(DataControllerBase controller, object row) {
			if(row == null)
				return null;
			IDictionary<string, object> properties = row as IDictionary<string, object>;
			if(properties == null)
				return null;
			List<PropertyDescriptor> list = new List<PropertyDescriptor>();
			foreach(KeyValuePair<string, object> pair in properties) {
				list.Add(new DevExpress.Data.Access.ExpandoPropertyDescriptor(controller, pair.Key, pair.Value == null ? null : pair.Value.GetType()));
			}
			return new PropertyDescriptorCollection(list.ToArray());
		}
		public static Type GetIndexerPropertyType(Type listType) {
			System.Reflection.PropertyInfo[] props = listType.GetProperties();
			for(int i = 0; i < props.Length; i++) {
				if("Item".Equals(props[i].Name) && props[i].PropertyType != typeof(object)) {
					return props[i].PropertyType;
				}
			}
			return null;
		}
		public static Type GetRowType(Type listType, out bool isGenericIListRowType) {
			isGenericIListRowType = false;
			Type rowType = GetIndexerPropertyType(listType);
			if(rowType == null) {
				rowType = GenericTypeHelper.GetGenericIListType(listType);
				isGenericIListRowType = true;
			}
			return rowType;
		}
		public static Type GetListType(object dataSource) {
			if(dataSource is IListWrapper) {
				return ((IListWrapper)dataSource).WrappedListType;
			}
			if(dataSource == null) return typeof(object);
			return dataSource.GetType();
		}
		public ListDataControllerHelper(DataControllerBase controller) : base(controller) { }
		protected override object AddNewRowCore() { 
			if(BindingList != null)
				return BindingList.AddNew();
			if(EditableView != null)
				return EditableView.AddNew();
			return null;
		}
		PropertyDescriptorCollection GetPropertyDescriptorCollectionCore() {
			if(TypedList != null) return TypedList.GetItemProperties(null);
			bool isGenericIListRowType;
			Type rowType = GetRowType(out isGenericIListRowType);
			object row = null;
			if(rowType != null && isGenericIListRowType && typeof(ICustomTypeDescriptor).IsAssignableFrom(rowType) && List.Count > 0) {
				row = GetRow(0);
			}
			if(rowType == null) {
				row = GetFirstRow();
				rowType = row == null ? null : row.GetType();
				if(rowType == null) rowType = Controller.ForcedDataRowType;
			}
			if(DevExpress.Data.Access.ExpandoPropertyDescriptor.IsDynamicType(rowType)) {
				return GetExpandoObjectProperties(Controller, row == null ? GetFirstRow() : row);
			}
			PropertyDescriptorCollection coll =  null;
			if(!Controller.AlwaysUsePrimitiveDataSource && rowType != null && !rowType.Equals(typeof(string)) && !rowType.IsPrimitive() && !rowType.IsDefined(typeof(Access.DataPrimitiveAttribute), true)) {
				if(row == null) {
					if(Controller.UseFirstRowTypeWhenPopulatingColumns(rowType)) {
						object firstRow = GetFirstRow();
						if(firstRow != null) {
							Type firstRowType = firstRow.GetType();
							if(rowType == null || rowType.IsAssignableFrom(firstRowType))
								rowType = firstRow.GetType();
						}
					}
					else {
						PropertyDescriptorCollection collection = TryGetItemProperties();
						if(collection != null)
							return collection;
					}
					coll = GetTypeProperties(rowType);
				} else
					coll = TypeDescriptor.GetProperties(row);
			}
			if(coll == null || coll.Count == 0) coll = CreateSimplePropertyDescriptor();
			return coll;
		}
		protected virtual PropertyDescriptorCollection GetTypeProperties(Type rowType) {
			return TypeDescriptor.GetProperties(rowType);
		}
		protected virtual PropertyDescriptorCollection TryGetItemProperties() {
#if !SL && !DXPORTABLE
			if(List.GetType().GetInterface("System.ComponentModel.IItemProperties") == null)
				return null;
			return GetItemProperties();
#else
			return null;
#endif
		}
#if !SL && !DXPORTABLE
		PropertyDescriptorCollection GetItemProperties() {
			IItemProperties itemProperties = List as IItemProperties;
			if(itemProperties == null)
				return null;
			IList<ItemPropertyInfo> propertyInfos = itemProperties.ItemProperties;
			if(propertyInfos == null)
				return null;
			List<PropertyDescriptor> descriptors = new List<PropertyDescriptor>();
			foreach(ItemPropertyInfo item in propertyInfos)
				if(item.Descriptor is PropertyDescriptor)
					descriptors.Add((PropertyDescriptor)item.Descriptor);
			return new PropertyDescriptorCollection(descriptors.ToArray());
		}
#endif
		object GetFirstRow() {
			if(List.Count == 0 && EditableView != null)
				return EditableView.CurrentAddItem;
			return List.Count > 0 ? GetRow(0) : null;
		}
		protected override PropertyDescriptorCollection GetPropertyDescriptorCollection() {
			PropertyDescriptorCollection coll = GetPropertyDescriptorCollectionCore(); 
			if(coll != null) {
				IDataControllerData2 dc2 = Controller.DataClient as IDataControllerData2;
				if(dc2 != null)
					coll = dc2.PatchPropertyDescriptorCollection(coll);
				if(dc2 == null || dc2.CanUseFastProperties)
					coll = DevExpress.Data.Access.DataListDescriptor.GetFastProperties(coll);
			}
			return coll;
		}
		public Type GetRowType(out bool isGenericIListRowType) {
			isGenericIListRowType = false;
			if(List == null) return null;
			Type listType = GetListType();
			if(List.Count == 0 && listType.IsArray) {
				return listType.GetElementType();
			}
			return GetRowType(listType, out isGenericIListRowType);
		}
		public Type GetIndexerPropertyType() {
			return GetIndexerPropertyType(GetListType());
		}
		Type GetListType() {
			return GetListType(List);
		}
		protected virtual PropertyDescriptorCollection CreateSimplePropertyDescriptor() {
			return new PropertyDescriptorCollection(new PropertyDescriptor[] { new DevExpress.Data.Access.SimpleListPropertyDescriptor() });
		}
		protected internal override void OnBindingListChanged(ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.ItemAdded && IsAddNewRowProcess) {
				SetDetachedListSourceRow(e.NewIndex);
			}
			if(e.ListChangedType == ListChangedType.ItemAdded || e.ListChangedType == ListChangedType.ItemDeleted) {
				if(e.NewIndex == DetachedListSourceRow) return;
			}
			base.OnBindingListChanged(e);
		}
		protected override void OnEndNewItemRow() {
			base.OnEndNewItemRow();
			if(beforeAddRowCount < Count) {
				int index = detachedRowPosition == DataController.InvalidRow ? Count - 1 : detachedRowPosition;
				OnBindingListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
			}
		}
		int beforeAddRowCount = -1;
		int detachedRowPosition = DataController.InvalidRow;
		protected internal override void RaiseOnStartNewItemRow() {
			this.beforeAddRowCount = Count;
			this.detachedRowPosition = DetachedListSourceRow;
			base.RaiseOnStartNewItemRow();
		}
		#region IRelationListEx Members
		bool IsValidRelationIndex(int relationIndex) {
			return relationIndex >= 0 && relationIndex < DetailColumns.Count;
		}
		DataColumnInfo GetDetailInfo(int relationIndex) {
			if(!IsValidRelationIndex(relationIndex)) return null;
			return DetailColumns[relationIndex];
		}
		string IRelationListEx.GetRelationDisplayName(int listSourceRow, int relationIndex) { 
			DataColumnInfo detail = GetDetailInfo(relationIndex);
			if(detail != null) return detail.Caption;
			return "";
		}
		int IRelationList.RelationCount { get { return ((IRelationListEx)this).GetRelationCount(DataController.InvalidRow); } }
		string IRelationList.GetRelationName(int listSourceRow, int relationIndex) { 
			DataColumnInfo detail = GetDetailInfo(relationIndex);
			return detail == null ? null: detail.Name;
		}
		bool IRelationList.IsMasterRowEmpty(int listSourceRow, int relationIndex) { 
			if(!IsValidRelationIndex(relationIndex)) return false;
			IList list = ((IRelationList)this).GetDetailList(listSourceRow, relationIndex);
			if(list == null || IsEmptyDetail(list)) return true;
			return false;
		}
		bool IsEmptyDetail(IList list) {
			if(list is ListIEnumerable) {
				return ((ListIEnumerable)list).IsEmpty();
			}
			return list.Count == 0;
		}
		IList IRelationList.GetDetailList(int listSourceRow, int relationIndex) { 
			DataColumnInfo detail = GetDetailInfo(relationIndex);
			object row = GetRow(listSourceRow);
			if(detail == null || row == null || row is NotLoadedObject) return null;
			object list = detail.PropertyDescriptor.GetValue(row);
			if(list is IList) return (IList)list;
			if(list is IEnumerable) return new ListIEnumerable((IEnumerable)list);
			return null;
		}
		int IRelationListEx.GetRelationCount(int listSourceRow) { return DetailColumns.Count; }
		#endregion
	}
#if !SILVERLIGHT && !DXPORTABLE
	public abstract class BaseDataViewControllerHelper : ListDataControllerHelper, IRelationListEx {
		public BaseDataViewControllerHelper(DataControllerBase controller) : base(controller) { }
		public override void SetRowValue(int listSourceRow, int column, object val) {
			if(val == null) val = DBNull.Value;
			base.SetRowValue(listSourceRow, column, val);
		}
		protected override object AddNewRowCore() { 
			return View.AddNew();
		}
		public override bool CaseSensitive { 
			get { 
				if(View != null && View.Table != null) return View.Table.CaseSensitive;
				return true;
			}
		}
		public abstract DataView View { get; }
		public override object GetRowKey(int listSourceRow) { return GetDataRow(listSourceRow); }
		protected override void PopulateColumn(PropertyDescriptor descriptor) {
			base.PopulateColumn(descriptor);
			DataColumnInfo colInfo = Columns[descriptor.Name];
			if(colInfo == null) colInfo = DetailColumns[descriptor.Name];
			if(colInfo != null) colInfo.DataIndex = View.Table.Columns.IndexOf(colInfo.Name);
		}
		protected virtual DataRow GetDataRow(int listSourceRow) {
			if(View == null || listSourceRow < 0 || listSourceRow >= View.Count) return null;
			return GetDataRowCore(listSourceRow);
		}
		DataRow GetDataRowCore(int listSourceRow) {
			if(listSourceRow < 0 || listSourceRow >= View.Count) return null;
			DataRowView rv = GetRow(listSourceRow) as DataRowView;
			return rv != null ? rv.Row : null;
		}
		public override IDataErrorInfo GetRowErrorInfo(int listSourceRow) {
			DataRow row = GetDataRow(listSourceRow);
			if(row != null && row.HasErrors) return GetRow(listSourceRow) as IDataErrorInfo;
			return null;
		}
		protected internal override void SetDetachedListSourceRow(int listSourceRow) { }
		protected internal override void RaiseOnStartNewItemRow() { }
		protected internal override void RaiseOnEndNewItemRow() { }
		bool CanIgnoreMoveEvent() {
			System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
			if(st.FrameCount > 4) {
				System.Diagnostics.StackFrame sf = st.GetFrame(4);
				System.Reflection.MemberInfo mi = sf.GetMethod();
				if(mi != null && mi.Name == "FinishAddNew") return true;
			}
			return false;
		}
		bool CanIgnoreItemChanged() {
			if(StackTraceHelper.CheckStackFrame("Update", typeof(System.Data.Common.DbDataAdapter), 1, 20)) return true;
			return false;
		}
		DataRow detachedRow = null;
		protected internal override void OnBindingListChanged(ListChangedEventArgs e) {
			DataRow prevDetachedRow = this.detachedRow;
			this.detachedRow = null;
			if(e.ListChangedType == ListChangedType.ItemMoved) {
				if(prevDetachedRow != null && (GetDataRow(e.NewIndex) == prevDetachedRow || GetDataRow(e.OldIndex) == prevDetachedRow) && CanIgnoreMoveEvent()) return;
			}
			if(e.ListChangedType == ListChangedType.ItemChanged && prevDetachedRow != null && e.OldIndex < 0) {
				if(GetDataRow(e.NewIndex) == prevDetachedRow) {
					CurrencyDataController cdc = Controller as CurrencyDataController;
					if(cdc != null && cdc.finishingNewItemRowEdit) {
						this.detachedRow = prevDetachedRow;
						return;
					}
				}
			}
			if(DetachedListSourceRow != DataController.InvalidRow) {
				if(e.ListChangedType != ListChangedType.ItemChanged || CanIgnoreItemChanged()) {
					DetachedListSourceRow = DataController.InvalidRow;
				}
				else {
					if(e.NewIndex >= 0 && e.NewIndex < DetachedListSourceRow) {
						base.OnBindingListChanged(e);
						return;
					}
				}
				Controller.OnEndNewItemRow();
				if(e.ListChangedType == ListChangedType.ItemDeleted) return;
				if(e.ListChangedType == ListChangedType.ItemAdded) {
					this.detachedRow = GetDataRow(e.NewIndex);
				}
			}
			if(e.ListChangedType == ListChangedType.ItemAdded) {
				DataRow row = GetDataRow(e.NewIndex);
				if(row != null && row.RowState == DataRowState.Detached) {
					DetachedListSourceRow = e.NewIndex;
					Controller.OnStartNewItemRow();
					return;
				}
			}
			base.OnBindingListChanged(e);
		}
		bool IsValidRelation(int relationIndex) {
			return relationIndex >= 0 && relationIndex < ((IRelationListEx)this).GetRelationCount(DataController.InvalidRow);
		}
		DataRelation GetDataRelation(int relationIndex) {
			if(!IsValidRelation(relationIndex)) return null;
			return View.Table.ChildRelations[relationIndex];
		}
	#region IRelationListEx Members
		string IRelationList.GetRelationName(int listSourceRow, int relationIndex) { 
			if(!IsValidRelation(relationIndex)) return string.Empty;
			return DetailColumns[relationIndex].Name;
		}
		int IRelationListEx.GetRelationCount(int listSourceRow) { return DetailColumns.Count; }
		bool IRelationList.IsMasterRowEmpty(int listSourceRow, int relationIndex) { 
			DataColumnInfo detail = GetDetailInfo(relationIndex);
			if(detail != null) {
				DataRelation relation = View.Table.ChildRelations[detail.Name];
				if(relation != null) {
					DataRow row = GetDataRow(listSourceRow);
					if(row != null && row.RowState != DataRowState.Deleted) {
						DataRow[] rows = row.GetChildRows(relation);
						if(rows != null && rows.Length > 0) return false;
					}
				}
				else {
					return false;
				}
			}
			return true;
		}
		DataColumnInfo GetDetailInfo(int relationIndex) {
			if(!IsValidRelation(relationIndex)) return null;
			return DetailColumns[relationIndex];
		}
		IList IRelationList.GetDetailList(int listSourceRow, int relationIndex) { 	
			DataColumnInfo detail = GetDetailInfo(relationIndex);
			object row = GetRow(listSourceRow);
			if(detail == null || row == null) return null;
			return detail.PropertyDescriptor.GetValue(row) as IList;
		}
		#endregion
	}
#if DXWhidbey
	public class BindingSourceDataControllerHelper : BaseDataViewControllerHelper {
		public BindingSourceDataControllerHelper(DataControllerBase controller) : base(controller) { }
		public override DataView View {
			get { return List.SyncRoot as DataView; }
		}
	}
#endif 
#endif
	public class BaseListDataControllerHelper : BaseDataControllerHelper {
		public BaseListDataControllerHelper(DataControllerBase controller) : base(controller) { }
		public override object GetRow(int listSourceRow, OperationCompleted completed) { 
			if(listSourceRow < 0 || listSourceRow >= List.Count) return null;
			return List[listSourceRow];
		}
		public override object GetRowValue(int listSourceRow, int column, OperationCompleted completed) {
			DataColumnInfo columnInfo = Columns[column];
			object row = columnInfo.Unbound ? listSourceRow : GetRow(listSourceRow);
			return columnInfo.PropertyDescriptor.GetValue(row);
		}
		protected static object KillDBNull(object nullableSomethig) {
			if(nullableSomethig is DBNull)
				return null;
			if(nullableSomethig is UnboundErrorObject)
				return null;
			return nullableSomethig;
		}
		protected static Func<string[]> GetExceptionAuxInfoGetter(DataColumnInfo columnInfo, Type expectedReturnType) {
			return () => GetColumnAuxInfo(columnInfo).Concat(new string[] { "expectedReturnType: " + expectedReturnType.FullName }).ToArray();
		}
		internal static IEnumerable<string> GetColumnAuxInfo(DataColumnInfo columnInfo) {
			yield return "column Index: " + columnInfo.Index;
			yield return "column Name: '" + columnInfo.Name + "'";
			yield return "column Caption: '" + columnInfo.Caption + "'";
			yield return "column Type: " + (columnInfo.Type == null ? "null" : columnInfo.Type.FullName);
			yield return "column Unbound: " + columnInfo.Unbound.ToString();
			yield return "column UnboundExpression: \"" + columnInfo.UnboundExpression + "\"";
			if(columnInfo.PropertyDescriptor == null) {
				yield return "column PropertyDescriptor: none";
			} else {
				yield return "PropertyDescriptor: " + columnInfo.PropertyDescriptor.GetType().FullName;
				yield return "PropertyDescriptor Name: '" + columnInfo.PropertyDescriptor.Name + "'";
				yield return "PropertyDescriptor PropertyType: " + columnInfo.PropertyDescriptor.PropertyType.FullName;
				yield return "PropertyDescriptor ComponentType:" + (columnInfo.PropertyDescriptor.ComponentType == null ? "null" : columnInfo.PropertyDescriptor.ComponentType.FullName);
			}
		}
		protected override Delegate GetGetRowValueCore(DataColumnInfo columnInfo, Type expectedReturnType) {
			if(!columnInfo.Unbound) {
				Type rowType, valueType;
				var dlg = PropertyDescriptorCriteriaCompilationSupport.TryGetFastGetter(columnInfo.PropertyDescriptor, out rowType, out valueType);
				if(dlg != null) {
					Func<int, object> getRow = i => this.GetRow(i);
					Delegate convertedFastGetter = dlg.ConvertFunc(rowType, valueType, typeof(object), expectedReturnType, GetExceptionAuxInfoGetter(columnInfo, expectedReturnType));
					if(valueType == typeof(object) && expectedReturnType == typeof(object)) {
						var objectFastGetter = (Func<object, object>)convertedFastGetter;
						Func<object, object> dbNullSafeObjectFastGetter = row => KillDBNull(objectFastGetter(row));
						convertedFastGetter = dbNullSafeObjectFastGetter;
					}
					Delegate nullProofFastGetter = GenericDelegateHelper.HedgeNullArg(convertedFastGetter, typeof(object), expectedReturnType);
					return getRow.ApplyChain(nullProofFastGetter, expectedReturnType);
				}
			}
			Func<int, object> slowCore;
			var capturedPd = columnInfo.PropertyDescriptor;
			if(columnInfo.Unbound) {
				slowCore = i => KillDBNull(capturedPd.GetValue(i));
			} else {
				slowCore = i => KillDBNull(capturedPd.GetValue(GetRow(i)));
			}
			if(expectedReturnType == typeof(string)) {
				Func<int, string> stringSlowCore = i => {
					object o = slowCore(i);
					if(o == null)
						return null;
					else
						return o.ToString();
				};
				return stringSlowCore;
			}
			return slowCore.ConvertFuncResult(expectedReturnType, GetExceptionAuxInfoGetter(columnInfo, expectedReturnType));
		}
		public override object GetRowValueDetail(int listSourceRow, DataColumnInfo detailColumn) {
			if(detailColumn.PropertyDescriptor != null) return detailColumn.PropertyDescriptor.GetValue(GetRow(listSourceRow));
			return null;
		}
		public override void SetRowValue(int listSourceRow, int column, object val) { 
			DataColumnInfo columnInfo = Columns[column];
			if(columnInfo == null || columnInfo.ReadOnly) return;
			object rowObject = columnInfo.Unbound ? listSourceRow : GetRow(listSourceRow);
			if(rowObject == null) return;
			object convertedValue = columnInfo.ConvertValue(val, true, Controller);
			columnInfo.PropertyDescriptor.SetValue(rowObject, convertedValue);
			if(!SupportsNotification)
				OnBindingListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, listSourceRow, listSourceRow));
		}
	}
#if !SL && !DXPORTABLE
	public class DataViewDataControllerHelper : BaseDataViewControllerHelper {
		public override void Dispose() {
			this.getRowDelegate = null;
			base.Dispose();
		}
		protected override PropertyDescriptorCollection GetPropertyDescriptorCollection() {
			return TypedList.GetItemProperties(null);
		}
		protected internal override void CancelNewItemRow() { }
	#region FastAccess
		FakeGetRow getRowDelegate;
		delegate DataRow FakeGetRow(DataView view, int row);
		DataRow dummy(int row) { return null; }
		void InitFastGetRow() {
			if(View == null) return;
			if(DevExpress.Data.Helpers.SecurityHelper.IsPartialTrust) return;
			MethodInfo getRowMethod = View.GetType().GetMethod("GetRow", BindingFlags.Instance | BindingFlags.NonPublic);
			if(getRowMethod == null) return;
			this.getRowDelegate = (FakeGetRow)Delegate.CreateDelegate(typeof(FakeGetRow), null, getRowMethod);
		}
		#endregion
		public DataViewDataControllerHelper(DataControllerBase controller) : base(controller) { 
			InitFastGetRow();
		}
		public override DataView View { get { return Controller.ListSource as DataView; } }
		protected override DataRow GetDataRow(int listSourceRow) {
			if(listSourceRow < 0 || listSourceRow >= View.Count) return null;
			if(this.getRowDelegate != null) return this.getRowDelegate(View, listSourceRow);
			return base.GetDataRow(listSourceRow);
		}
		public override object GetRowValue(int listSourceRow, int column, OperationCompleted completed) {
			DataColumnInfo columnInfo = Columns[column];
			if(columnInfo.Unbound) return columnInfo.PropertyDescriptor.GetValue(listSourceRow);
			if(this.getRowDelegate != null) {
				int dataIndex = columnInfo.DataIndex;
				if(dataIndex > -1) {
					DataRow row = GetDataRow(listSourceRow);
					if(row != null) {
						if(row.RowState != DataRowState.Deleted)
							return row[columnInfo.DataIndex];
					}
					else 
						return null;
				}
			}
			return base.GetRowValue(listSourceRow, column, null);
		}
		protected override Delegate GetGetRowValueCore(DataColumnInfo columnInfo, Type expectedReturnType) {
			if(this.getRowDelegate != null) {
				if(!columnInfo.Unbound) {
					int dataIndex = columnInfo.DataIndex;
					if(dataIndex >= 0) {
						Func<int, object> core = sri => {
							DataRow row = GetDataRow(sri);
							if(row == null)
								return null;
							if(row.RowState != DataRowState.Deleted)
								return KillDBNull(row[dataIndex]);
							return null;
						};
						return core.ConvertFuncResult(expectedReturnType, GetExceptionAuxInfoGetter(columnInfo, expectedReturnType));
					}
				}
			}
			return base.GetGetRowValueCore(columnInfo, expectedReturnType);
		}
	}
#endif
	public class FilterHelper {
		DataControllerBase controller;
		public DataControllerBase Controller { get { return controller; } }
#if SILVERLIGHT
		public FilterHelper(DataControllerBase controller) {
			this.controller = controller;
		}
		public virtual CriteriaOperator CalcColumnFilterCriteriaByValue(int column, object columnValue, bool equal, bool roundDateTime, IFormatProvider provider) {
			if (!Controller.IsColumnValid(column)) return null;
			DataColumnInfo colInfo = Controller.Columns[column];
			bool reqDisplayText = IsRequiredDisplayText(column);
			Type columnType = reqDisplayText ? typeof(string) : colInfo.Type;
			string columnName = colInfo.Name;
			CriteriaOperator criterion = CalcColumnFilterCriteriaByValue(columnName, columnType, columnValue, roundDateTime, provider);
			if (!equal)
				criterion = !criterion;
			return criterion;
		}
#endif
		VisibleListSourceRowCollection visibleListSourceRows;
		public FilterHelper(DataControllerBase controller, VisibleListSourceRowCollection visibleListSourceRows) {
			this.controller = controller;
			this.visibleListSourceRows = visibleListSourceRows;
		}
		protected bool IsRequiredDisplayText(int column) {
			if(!Controller.IsColumnValid(column)) return false;
			return IsRequiredDisplayText(Controller.Columns[column]);
		}
		protected bool IsRequiredDisplayText(DataColumnInfo column) {
			return column != null && Controller.SortClient != null && Controller.SortClient.RequireDisplayText(column);
		}
		public virtual VisibleListSourceRowCollection VisibleListSourceRows { get { return visibleListSourceRows; } }
		protected virtual object[] GetColumnValues(int column, int maxCount, bool includeFilteredOut, bool roundDateTime, bool displayText, bool implyNullLikeEmptyStringWhenFiltering) {
			if(!Controller.IsReady) return null;
			if(maxCount == 0) return null;
			int realCount = 0;
			int count = GetValuesCount(includeFilteredOut);
			if(!Controller.IsColumnValid(column) || count < 1) return null;
			DataColumnInfo colInfo = Controller.Columns[column];
			bool isDateTimeColumn = !displayText && (colInfo.Type.Equals(typeof(DateTime)) || colInfo.Type.Equals(typeof(DateTime?)));
			object[] allValues = new object[count];
			for(int n = 0; n < count; n++) {
				int row = GetRow(includeFilteredOut, n);
				object val = Controller.Helper.GetRowValue(row, column, null);
				if(displayText) val = Controller.SortClient.GetDisplayText(row, colInfo, val, colInfo.Name);
				if(implyNullLikeEmptyStringWhenFiltering) {
					if(val != null && !(val is DBNull) && !(val is System.IComparable)) continue;
				} else {
					if(val == null || val is DBNull || !(val is System.IComparable)) continue;
				}
				if(isDateTimeColumn & roundDateTime && (val is DateTime)) {
					DateTime org = (DateTime)val;
					val = new DateTime(org.Year, org.Month, org.Day);
				}
				allValues[realCount ++] = val;
				if(maxCount != -1 && realCount == maxCount) break;
			}
			if(realCount != count) {
				if(realCount == 0) return null;
				object[] res = new object[realCount];
				Array.Copy(allValues, 0, res, 0, realCount);
				return res;
			}
			return allValues;
		}
		protected virtual object[] GetColumnValues(int column, int maxCount, bool includeFilteredOut, bool roundDateTime, bool displayText) {
			return GetColumnValues(column, maxCount, includeFilteredOut, roundDateTime, displayText, false);
		}
		protected virtual int GetValuesCount(bool includeFilteredOut) {
			return includeFilteredOut ? Controller.ListSourceRowCount : Controller.GetVisibleListSourceRowCount(VisibleListSourceRows);
		}
		protected virtual int GetRow(bool includeFilteredOut, int index) {
			return includeFilteredOut ? index : VisibleListSourceRows.GetListSourceRow(index);
		}
		public virtual object[] GetUniqueColumnValues(int column, int maxCount, bool includeFilteredOut, bool roundDataTime, OperationCompleted completed, bool implyNullLikeEmptyStringWhenFiltering) {
			if(maxCount == 0) return null;
			bool displayText = false;
			if(!Controller.IsReady || !Controller.IsColumnValid(column)) return null;
			displayText = IsRequiredDisplayText(column);
			object[] allValues = GetColumnValues(column, maxCount, includeFilteredOut, roundDataTime, displayText, implyNullLikeEmptyStringWhenFiltering);
			if(allValues == null || allValues.Length < 2) return allValues;
			try {
				Array.Sort(allValues, Controller.ValueComparer);
			} catch {
			}
			int uniqCount = 0, count = allValues.Length;
			object[] uniqValues = new object[count];
			object lastValue = null, curValue;
			for(int n = 0; n < count; n++) {
				curValue = allValues[n];
				if(n == 0 || Controller.ValueComparer.Compare(curValue, lastValue) != 0) {
					uniqValues[uniqCount++] = curValue;
				}
				lastValue = curValue;
			}
			allValues = new object[uniqCount];
			Array.Copy(uniqValues, 0, allValues, 0, uniqCount);
			return allValues;
		}
		public virtual object[] GetUniqueColumnValues(int column, int maxCount, bool includeFilteredOut, bool roundDataTime, OperationCompleted completed) {
			return GetUniqueColumnValues(column, maxCount, includeFilteredOut, roundDataTime, completed, false);
		}
#if !SL
		public virtual CriteriaOperator CalcColumnFilterCriteriaByValue(int column, object columnValue, bool equal, bool roundDateTime, IFormatProvider provider) {
			if (!Controller.IsColumnValid(column)) return null;
			DataColumnInfo colInfo = Controller.Columns[column];
			bool reqDisplayText = IsRequiredDisplayText(column);
			Type columnType = reqDisplayText ? typeof(string) : colInfo.Type;
			string columnName = colInfo.Name;
			CriteriaOperator criterion = CalcColumnFilterCriteriaByValue(columnName, columnType, columnValue, roundDateTime, provider);
			if (!equal)
				criterion = !criterion;
			return criterion;
		}
#endif
		public string CalcColumnFilterStringByValue(int column, object columnValue, bool equal, bool roundDateTime, IFormatProvider provider) {
			return CriteriaOperator.ToString(CalcColumnFilterCriteriaByValue(column, columnValue, equal, roundDateTime, provider));
		}
		public static CriteriaOperator CalcColumnFilterCriteriaByValue(string columnName, Type columnFilteredType, object value, bool roundDateTime, IFormatProvider provider) {
			OperandProperty property = new OperandProperty(columnName);
			if (value == null || value is DBNull) {
				return property.IsNull();
			}
#if DXWhidbey
			Type underlyingType = Nullable.GetUnderlyingType(columnFilteredType);
			if (underlyingType != null)
				columnFilteredType = underlyingType;
#endif
			if (roundDateTime && columnFilteredType == typeof(DateTime)) {
				DateTime min = ConvertToDate(value, provider);
				min = new DateTime(min.Year, min.Month, min.Day);
				try {
					DateTime next = min.AddDays(1);
					return (property >= min) & (property < next);
				}
				catch {
					return property >= min;
				}
			}
			value = CorrectFilterValueType(columnFilteredType, value, provider);
			return property == new OperandValue(value);
		}
		public static object CorrectFilterValueType(Type columnFilteredType, object filteredValue) {
			return CorrectFilterValueType(columnFilteredType, filteredValue, CultureInfo.CurrentCulture);
		}
		public static object CorrectFilterValueType(Type columnFilteredType, object filteredValue, IFormatProvider provider) {
			if(filteredValue == null)
				return filteredValue;
			if(columnFilteredType == null)
				return filteredValue;
#if DXWhidbey
			Type underlyingFilteredType = Nullable.GetUnderlyingType(columnFilteredType);
			if(underlyingFilteredType != null)
				columnFilteredType = underlyingFilteredType;
#endif
			Type currentType = filteredValue.GetType();
			if(columnFilteredType.IsAssignableFrom(currentType))
				return filteredValue;
			try {
				if(columnFilteredType == typeof(Guid)) {
					if(filteredValue is string)
						return new Guid((string)filteredValue);
					else if(filteredValue is byte[])
						return new Guid((byte[])filteredValue);
				}
				if(columnFilteredType == typeof(TimeSpan)) {
					if(filteredValue is string)
						return TimeSpan.Parse((string)filteredValue, provider);
					if(filteredValue is DateTime)
						return TimeSpan.FromTicks(((DateTime)filteredValue).Ticks);
				}
				return Convert.ChangeType(filteredValue, columnFilteredType, provider);
			} catch { }
			return filteredValue;
		}
		public static DateTime ConvertToDate(object val, IFormatProvider provider) {
			DateTime res = DateTime.MinValue;
			if(val == null) return res;
			try {
				if(!(val is DateTime)) {
					if(provider != null) 
						res = DateTime.Parse(val.ToString(), provider);
					else
						res = DateTime.Parse(val.ToString());
				}
				else
					res = (DateTime)val;
			}
			catch { }
			return res;
		}
		public static string ConvertDateToString(object val, IFormatProvider provider) {
			string res = string.Empty;
			try {
				res = string.Format(CultureInfo.InvariantCulture.DateTimeFormat, "{0:" + "MM/dd/yyyy HH:mm:ss.fffff" + "}", ConvertToDate(val, provider));
			}
			catch {
				res = val == null ? "" : val.ToString();
			}
			return res;
		}
	}
	public static class MultiselectRoundedDateTimeFilterHelper {
		public static CriteriaOperator DatesToCriteria(string columnName, IEnumerable<DateTime> dates) {
			OperandProperty property = new OperandProperty(columnName);
			CriteriaOperator rv = null;
			DateTime? intervalStart = null;
			DateTime? intervalEnd = null;
			foreach(DateTime dt in dates.GroupBy((d => d.Date)).Select(g => g.Key).OrderBy(d => d).Concat(new DateTime[] { DateTime.MaxValue })) {
				if(intervalStart.HasValue) {
					if(intervalEnd == dt) {
						intervalEnd = (dt >= DateTime.MaxValue.Date) ? (DateTime?)null : dt.AddDays(1);
						continue;
					}
					CriteriaOperator criterion = property >= intervalStart.Value;
					if(intervalEnd.HasValue)
						criterion &= property < intervalEnd.Value;
					rv |= criterion;
				}
				intervalStart = dt;
				intervalEnd = (dt >= DateTime.MaxValue.Date) ? (DateTime?)null : dt.AddDays(1);
			}
			System.Diagnostics.Debug.Assert(intervalStart == DateTime.MaxValue);
			System.Diagnostics.Debug.Assert(!intervalEnd.HasValue);
			return rv;
		}
		class DTDescriptor: CriteriaCompilerDescriptor {
			protected readonly string PropertyName;
			public DTDescriptor(string propertyName) {
				this.PropertyName = propertyName;
			}
			public override Type ObjectType {
				get { return typeof(DateTime); }
			}
			public override System.Linq.Expressions.Expression MakePropertyAccess(System.Linq.Expressions.Expression baseExpression, string propertyPath) {
				if(propertyPath == PropertyName)
					return baseExpression;
				else
					return System.Linq.Expressions.Expression.Constant(null);
			}
			public override CriteriaCompilerRefResult DiveIntoCollectionProperty(System.Linq.Expressions.Expression baseExpression, string collectionPropertyPath) {
				if(string.IsNullOrEmpty(collectionPropertyPath))
					throw new InvalidOperationException("Internal error: string.IsNullOrEmpty(collectionPropertyPath) within " + this.GetType().FullName);
				int dotPos = EvaluatorProperty.GetPropertySeparatorDotPos(collectionPropertyPath);
				string subProperty = dotPos >= 0 ? collectionPropertyPath.Substring(dotPos + 1) : null;
				return new CriteriaCompilerRefResult(new CriteriaCompilerLocalContext(System.Linq.Expressions.Expression.Constant(null), CriteriaCompilerContextDescriptorReflective.Instance), subProperty);
			}
			public override System.Linq.Expressions.LambdaExpression MakeFreeJoinLambda(string joinTypeName, CriteriaOperator condition, OperandParameter[] conditionParameters, Aggregate aggregateType, CriteriaOperator aggregateExpression, OperandParameter[] aggregateExpresssionParameters, Type[] invokeTypes) {
				object value = null;
				if(aggregateType == Aggregate.Exists)
					value = false;
				var parms = invokeTypes.Select(t => System.Linq.Expressions.Expression.Parameter(t, "")).ToArray();
				return System.Linq.Expressions.Expression.Lambda(System.Linq.Expressions.Expression.Constant(value), parms);
			}
		}
		public static IEnumerable<DateTime> GetCheckedDates(CriteriaOperator criteria, string dateTimePropertyName, IEnumerable<DateTime> dates) {
			Func<DateTime, bool> pr = CriteriaCompiler.ToPredicate<DateTime>(criteria, new DTDescriptor(dateTimePropertyName));
			return dates.Where(pr);
		}
	}
}
