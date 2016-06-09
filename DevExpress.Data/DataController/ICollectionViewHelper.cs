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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using DevExpress.Data.Async.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
#if !SILVERLIGHT
using System.Windows;
using System.Windows.Forms;
#else
using DevExpress.Data.Browsing;
using DevExpress.Xpf.Core;
#endif
using System.Threading;
namespace DevExpress.Data.Linq {
	public interface ISupportEditableCollectionView : IEditableCollectionView {
		bool IsSupportEditableCollectionView { get; }
	}
#if !DXPORTABLE
	public interface ICollectionViewWrapper : ICollectionView {
		ICollectionView WrappedView { get; }
	}
	public class ICollectionViewHelper : IListServer, IDisposable, IDataSync, IListServerCaps, ISupportEditableCollectionView, IListWrapper, IWeakEventListener
#if !SL && !DXPORTABLE
	, IItemProperties
#endif
	{
		object newItemPlaceHolder;
		ICollectionView collection;
		Type elementType;
		CriteriaOperator FilterCriteria;
		Func<object, bool> fitPredicate;
		CollectionViewListSourceGroupInfo rootGroup;
		IList<ServerModeSummaryDescriptor> GroupSummaryInfo;
		Func<IEnumerable, object>[] GroupSummaryEvaluators;
		IList<ServerModeSummaryDescriptor> TotalSummaryInfo;
		Predicate<object> lastFilter;
		List<GroupDescription> lastGroupDescriptions = new List<GroupDescription>();
		List<SortDescription> lastSortDescriptions = new List<SortDescription>();
		bool isInitialized;
		bool isFilterSortGroupLocked;
		protected Type ElementType {
			get {
				if(elementType == null || elementType == typeof(object) || elementType.FullName == ListDataControllerHelper.UseFirstRowTypeWhenPopulatingColumnsTypeName) { 
					IEnumerable sourceCollection = collection.SourceCollection;
					elementType = ListBindingHelper.GetListItemType(collection.SourceCollection);
					foreach(object entity in sourceCollection) {
						Type firstRowType = entity.GetType();
						if(elementType == null || elementType.IsAssignableFrom(firstRowType)) {
							elementType = firstRowType;
						}
						break;
					}
				}
				return elementType;
			}
			set {
				elementType = value;
			}
		}
		CollectionViewFilterSortGroupInfoChangedEventHandler filterSortGroupInfoChanged;
		public event CollectionViewFilterSortGroupInfoChangedEventHandler FilterSortGroupInfoChanged {
			add { filterSortGroupInfoChanged += value; }
			remove { filterSortGroupInfoChanged -= value; }
		}
		void ResetCaches() {
			fitPredicate = null;
			rootGroup = null;
		}
		public object NewItemPlaceHolder { get { return newItemPlaceHolder; } set { newItemPlaceHolder = value; } }
		protected IList List { get { return RootGroup.Rows; } }
		protected INotifyCollectionChanged CollectionSortDescriptions {
			get { return (INotifyCollectionChanged)this.Collection.SortDescriptions; }
		}
		protected INotifyCollectionChanged CollectionGroupDescriptions {
			get { return (INotifyCollectionChanged)this.Collection.GroupDescriptions; }
		}
		public ICollectionViewHelper(ICollectionView collection, object newItemPlaceHolder) : this(null, collection, newItemPlaceHolder) { }
		public ICollectionViewHelper(Type elementType, ICollectionView collection, object newItemPlaceHolder) {
			AllowSyncSortingAndGrouping = true;
			this.newItemPlaceHolder = newItemPlaceHolder;
			ElementType = elementType;
			this.collection = collection;
			AttachedProperty.SetAttachedProperty(Collection, "isFilterSortGroupLocked", false);
			CollectionChangedEventManager.AddListener(GetUnderlyingView(), this);
			SyncLastCollections();
		}
		ICollectionView GetUnderlyingView() {
			ICollectionViewWrapper wrapper = this.collection as ICollectionViewWrapper;
			return wrapper != null ? wrapper.WrappedView : this.collection;
		}
		public ICollectionView Collection { get { return collection; } }
		void FilterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if((bool)AttachedProperty.GetAttachedProperty(Collection, "isFilterSortGroupLocked"))
				return;
			bool needRefresh = false;
			if(Collection.CanFilter && lastFilter != Collection.Filter) {
				needRefresh = true;
			}
			if(Collection.CanSort && !CollectionExtensions.AreEqual(lastSortDescriptions, Collection.SortDescriptions)) {
				needRefresh = true;
			}
			if(Collection.CanGroup && !CollectionExtensions.AreEqual(lastGroupDescriptions, Collection.GroupDescriptions)) {
				needRefresh = true;
			}
			Reset();
			isFilterSortGroupLocked = true;
			try {
				RaiseFilterSortGroupInfoChanged(needRefresh);
			}
			finally {
				isFilterSortGroupLocked = false;
			}
			if(Collection.CanFilter)
				lastFilter = Collection.Filter;
			SyncLastCollections();
		}
		void RaiseFilterSortGroupInfoChanged(bool needRefresh) {
			if(filterSortGroupInfoChanged == null)
				return;
			int groupCount = 0;
			if(Collection.CanGroup)
				groupCount = Collection.GroupDescriptions.Count;
			filterSortGroupInfoChanged(this, new CollectionViewFilterSortGroupInfoChangedEventArgs(CreateSortInfo(), groupCount, Collection.CanFilter && lastFilter != Collection.Filter, needRefresh));
		}
		public void Initialize() {
			isInitialized = true;
			RaiseFilterSortGroupInfoChanged(true);
		}
		internal List<ListSortInfo> CreateSortInfo() {
			List<ListSortInfo> res = new List<ListSortInfo>();
			if(!Collection.CanSort)
				return res;
			List<SortDescription> sortDescriptions = new List<SortDescription>(Collection.SortDescriptions);
			if(Collection.CanGroup)
				foreach(GroupDescription groupDescription in Collection.GroupDescriptions) {
					PropertyGroupDescription propertyGroupDescription = groupDescription as PropertyGroupDescription;
					SortDescription sd = sortDescriptions.Find(sortDescription => {
						return sortDescription.PropertyName == propertyGroupDescription.PropertyName;
					});
					sortDescriptions.Remove(sd);
					if(string.IsNullOrEmpty(sd.PropertyName))
						sd = new SortDescription(propertyGroupDescription.PropertyName, ListSortDirection.Ascending);
					res.Add(new ListSortInfo(sd.PropertyName, sd.Direction));
				}
			for(int i = 0; i < sortDescriptions.Count; i++) {
				SortDescription sd = sortDescriptions[i];
				res.Add(new ListSortInfo(sd.PropertyName, sd.Direction));
			}
			return res;
		}
		internal ServerModeOrderDescriptor CreateServerModeOrderDescriptor(SortDescription sd) {
			return new ServerModeOrderDescriptor(new OperandProperty(sd.PropertyName), sd.Direction == ListSortDirection.Descending);
		}
		void Reset() {
			ResetCaches();
		}
		public virtual void Apply(Filtering.CriteriaOperator filterCriteria, ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount, ICollection<ServerModeSummaryDescriptor> groupSummaryInfo, ICollection<ServerModeSummaryDescriptor> totalSummaryInfo) {
			TotalSummaryInfo = totalSummaryInfo == null ? new List<ServerModeSummaryDescriptor>() : new List<ServerModeSummaryDescriptor>(totalSummaryInfo);
			GroupSummaryInfo = groupSummaryInfo == null ? new List<ServerModeSummaryDescriptor>() : new List<ServerModeSummaryDescriptor>(groupSummaryInfo);
			GroupSummaryEvaluators = null;
			if(!isInitialized) return;
			if(isFilterSortGroupLocked) {
				ResetCaches();
				return;
			}
			try {
				using(Collection.DeferRefresh()) {
					try {
						AttachedProperty.SetAttachedProperty(Collection, "isFilterSortGroupLocked", true);
						ResetCaches();
						if(!ReferenceEquals(FilterCriteria, filterCriteria)) {
							if(Collection.CanFilter) {
								Collection.Filter = null;
								Collection.Filter = filterPredicate;
								lastFilter = filterPredicate;
							}
						}
						FilterCriteria = filterCriteria;
						ICollection<ServerModeOrderDescriptor> SortInfo = sortInfo ?? new ServerModeOrderDescriptor[0];
						if(AllowSyncSortingAndGrouping && NeedSortGroupRefresh(SortInfo, groupCount)) {
							if(Collection.CanSort) {
								Collection.SortDescriptions.Clear();
							} else {
								if(SortInfo.Count > 0)
									throw new NotSupportedException("Sorting is not supported");
							}
							if(Collection.CanGroup) {
								Collection.GroupDescriptions.Clear();
							} else {
								if(groupCount > 0)
									throw new NotSupportedException("Grouping is not supported");
							}
							int groups = groupCount;
							foreach(ServerModeOrderDescriptor od in SortInfo) {
								string name = GetOperandPropertyName(od);
								ListSortDirection direction = od.IsDesc ? ListSortDirection.Descending : ListSortDirection.Ascending;
								SortDescription? sortDescriptionNullable = FindSortDescriptionByName(name);
								if(sortDescriptionNullable != null) {
									SortDescription sortDescription = sortDescriptionNullable.Value;
									int index = Collection.SortDescriptions.IndexOf(sortDescription);
									Collection.SortDescriptions.Remove(sortDescription);
									Collection.SortDescriptions.Insert(index, new SortDescription(name, direction));
								} else {
									Collection.SortDescriptions.Add(new SortDescription(name, direction));
								}
								if(groups > 0) {
									PropertyGroupDescription groupDescription = FindGroupDescriptionByName(name);
									if(groupDescription != null) {
										int index = Collection.GroupDescriptions.IndexOf(groupDescription);
										Collection.GroupDescriptions.Remove(groupDescription);
										Collection.GroupDescriptions.Insert(index, new PropertyGroupDescription(name));
									} else {
										Collection.GroupDescriptions.Add(new PropertyGroupDescription(name));
									}
									groups--;
								}
							}
							SyncLastCollections();
						}
					} finally {
						AttachedProperty.SetAttachedProperty(Collection, "isFilterSortGroupLocked", false);
					}
				}
			} catch(InvalidOperationException) {
			}
		}
		string GetOperandPropertyName(ServerModeOrderDescriptor od) {
			OperandProperty op = od.SortExpression as OperandProperty;
			if(ReferenceEquals(op, null)) {
				FunctionOperator fo = od.SortExpression as FunctionOperator;
				if(!ReferenceEquals(fo, null)) {
					op = fo.Operands.FirstOrDefault() as OperandProperty;
				}
			}
			if(ReferenceEquals(op, null))
				throw new NotSupportedException("Sorting by expression " + CriteriaOperator.ToString(od.SortExpression));
			return op.PropertyName;
		}
		SortDescription? FindSortDescriptionByName(string name) {
			foreach(SortDescription sortDescritption in Collection.SortDescriptions) {
				if(sortDescritption.PropertyName == name) return sortDescritption;
			}
			return null;
		}
		PropertyGroupDescription FindGroupDescriptionByName(string name) {
			foreach(GroupDescription groupDescritption in Collection.GroupDescriptions) {
				PropertyGroupDescription propertyGroupDescription = groupDescritption as PropertyGroupDescription;
				if(propertyGroupDescription != null && propertyGroupDescription.PropertyName == name)
					return propertyGroupDescription;
			}
			return null;
		}
		bool NeedSortGroupRefresh(ICollection<ServerModeOrderDescriptor> sortInfo, int groupCount) {
			if(Collection.GroupDescriptions == null)
				return true;
			if(sortInfo.Count != Collection.SortDescriptions.Count || groupCount != Collection.GroupDescriptions.Count)
				return true;
			int i = 0;
			foreach(ServerModeOrderDescriptor descriptor1 in sortInfo) {
				SortDescription descriptor2 = Collection.SortDescriptions[i];
				string name1 = GetOperandPropertyName(descriptor1);
				if(name1 != descriptor2.PropertyName || descriptor1.IsDesc != (descriptor2.Direction == ListSortDirection.Descending))
					return true;
				if(i < groupCount) {
					PropertyGroupDescription descriptor3 = Collection.GroupDescriptions[i] as PropertyGroupDescription;
					if(descriptor3.PropertyName != name1)
						return true;
				}
				i++;
			}
			return false;
		}
		private void SyncLastCollections() {
			if(Collection.CanSort) {
				lastSortDescriptions.Clear();
				lastSortDescriptions.AddRange(Collection.SortDescriptions);
			}
			if(Collection.CanGroup) {
				lastGroupDescriptions.Clear();
				lastGroupDescriptions.AddRange(Collection.GroupDescriptions);
			}
		}
		public event EventHandler<ServerModeExceptionThrownEventArgs> ExceptionThrown { add { } remove { } }
		public int FindIncremental(Filtering.CriteriaOperator expression, string value, int startIndex, bool searchUp, bool ignoreStartRow, bool allowLoop) {
			return DataController.InvalidRow;
		}
		public IList GetAllFilteredAndSortedRows() { return List; }
		public virtual bool PrefetchRows(ListSourceGroupInfo[] groupsToPrefetch, CancellationToken cancellationToken) {
			return List.Count >= 0;
		}
		public List<ListSourceGroupInfo> GetGroupInfo(ListSourceGroupInfo parentGroup) {
			if(parentGroup == null) {
				return RootGroup.ChildrenGroups;
			}
			CollectionViewListSourceGroupInfo info = parentGroup as CollectionViewListSourceGroupInfo;
			if(info == null)
				throw new NotSupportedException("parentGroup is not a CollectionViewListSourceGroupInfo");
			return info.ChildrenGroups;
		}
		CollectionViewListSourceGroupInfo RootGroup {
			get {
				if(this.rootGroup == null)
					this.rootGroup = CreateTopGroup();
				return this.rootGroup;
			}
		}
		CollectionViewListSourceGroupInfo CreateTopGroup() {
			CollectionViewListSourceGroupInfo g = new CollectionViewListSourceGroupInfo();
			g.Level = -1;
			if(Collection.Groups != null) {
				List<object> rows = new List<object>();
				foreach(object groupObject in Collection.Groups) {
					CollectionViewGroup group = groupObject as CollectionViewGroup;
					if(group == null) continue;
					CollectionViewListSourceGroupInfo info = CreateListSourceGroupInfo(group, 0);
					g.ChildrenGroups.Add(info);
					rows.AddRange(info.Rows.Cast<object>());
				}
				g.Rows = rows.ToArray();
			}
			else {
				g.Rows = Collection.Cast<object>().Where(x => !ReferenceEquals(x, NewItemPlaceHolder)).ToArray();
			}
			if(this.TotalSummaryInfo != null && TotalSummaryInfo.Count > 0) {
				foreach(var ev in MakeSummaryEvaluators(TotalSummaryInfo)) {
					g.Summary.Add(ev(g.Rows));
				}
			}
			return g;
		}
		CollectionViewListSourceGroupInfo CreateListSourceGroupInfo(CollectionViewGroup group, int level) {
			CollectionViewListSourceGroupInfo info = new CollectionViewListSourceGroupInfo();
			info.ChildDataRowCount = group.IsBottomLevel ? group.ItemCount : 0;
			info.GroupValue = group.Name;
			info.Level = level;
			if(group.IsBottomLevel) {
				info.Rows = group.Items.Where(x => !ReferenceEquals(x, NewItemPlaceHolder)).ToArray();
			}
			else {
				List<object> rows = new List<object>();
				foreach(object subGroupObject in group.Items) {
					CollectionViewGroup subGroup = subGroupObject as CollectionViewGroup;
					if(subGroup == null) continue;
					CollectionViewListSourceGroupInfo subInfo = CreateListSourceGroupInfo(subGroup, level + 1);
					info.ChildDataRowCount += subInfo.ChildDataRowCount;
					info.ChildrenGroups.Add(subInfo);
					rows.AddRange(subInfo.Rows.Cast<object>());
				}
				info.Rows = rows.ToArray();
			}
			if(this.GroupSummaryInfo != null) {
				if(GroupSummaryEvaluators == null)
					GroupSummaryEvaluators = MakeSummaryEvaluators(GroupSummaryInfo);
				foreach(var ev in GroupSummaryEvaluators) {
					info.Summary.Add(ev(info.Rows));
				}
			}
			return info;
		}
		Func<IEnumerable, object>[] MakeSummaryEvaluators(IEnumerable<ServerModeSummaryDescriptor> descriptors) {
			var aggDescriptor = new DefaultTopLevelCriteriaCompilerContextDescriptor(CriteriaCompilerDescriptor.Get(this.ElementType));
			List<Func<IEnumerable, object>> rv = new List<Func<IEnumerable, object>>();
			foreach(var sd in descriptors) {
				Func<IEnumerable, object> ev;
				try {
					var untyped = CriteriaCompiler.ToUntypedDelegate(new AggregateOperand(null, sd.SummaryExpression, sd.SummaryType, null), aggDescriptor);
					ev = rows => {
						try {
							return untyped(rows);
						} catch {
							return null;
						}
					};
				} catch {
					ev = rows => null;
				}
				rv.Add(ev);
			}
			return rv.ToArray();
		}
		public int GetRowIndexByKey(object key) { return IndexOf(key); }
		public object GetRowKey(int index) { return this[index]; }
		public List<object> GetTotalSummary() {
			return RootGroup.Summary;
		}
		public object[] GetUniqueColumnValues(CriteriaOperator expression, int maxCount, bool includeFilteredOut) {
			var selector = CriteriaCompiler.ToUntypedDelegate(expression, CriteriaCompilerDescriptor.Get(ElementType));
			return Collection.SourceCollection.Cast<object>().Select(selector).Distinct().OrderBy(x => x).ToArray();
		}
		public void Refresh() { Collection.Refresh(); }
		public bool Contains(object value) {
			return Collection.Contains(value);
		}
		public int IndexOf(object value) {
			return List.IndexOf(value);
		}
		public event EventHandler<ServerModeInconsistencyDetectedEventArgs> InconsistencyDetected { add { } remove { } }
		public int LocateByValue(CriteriaOperator expression, object value, int startIndex, bool searchUp) {
			var selector = CriteriaCompiler.ToUntypedDelegate(expression, CriteriaCompilerDescriptor.Get(ElementType));
			int to = searchUp ? -1 : List.Count;
			int delta = searchUp ? -1 : 1;
			for (int i = Math.Max(0, startIndex); i != to; i = i + delta)
				if (selector.Invoke(List[i]).Equals(value))
					return i;
			return -1;
		}
		public int Add(object value) { throw new NotSupportedException(); }
		public void Clear() { throw new NotSupportedException(); }
		public void Insert(int index, object value) { throw new NotSupportedException(); }
		public void Remove(object value) { EditableView.Remove(value); }
		public void RemoveAt(int index) { EditableView.RemoveAt(index); }
		public void CopyTo(Array array, int index) { throw new NotSupportedException(); }
		public IEnumerator GetEnumerator() { return Collection.GetEnumerator(); }
		public bool IsFixedSize { get { return true; } }
		public bool IsReadOnly { get { return true; } }
		public object this[int index] {
			get {
				return List[index];
			}
			set {
				throw new NotSupportedException();
			}
		}
		public int Count { get { return List.Count; } }
		public bool IsSynchronized { get { return false; } }
		public object SyncRoot { get { return this; } }
		public object GetGridSource() { return this; }
		public object GetGridSourceAsync() {
			AsyncListServerCore core = new AsyncListServerCore(System.Threading.SynchronizationContext.Current);
			core.ListServerGet += delegate(object sender, ListServerGetOrFreeEventArgs e) {
				e.ListServerSource = this;
			};
			return new AsyncListServer2DatacontrollerProxy(core);
		}
		public void Dispose() {
			Collection.Filter -= filterPredicate;
		}
		bool filterPredicate(object obj) {
			if(obj == null)
				return false;
			if(fitPredicate == null) {
				if(ReferenceEquals(FilterCriteria, null))
					fitPredicate = x => true;
				else {
					try {
						if(!Collection.CanFilter)
							throw new NotSupportedException("Source can't filterCriteria");
						fitPredicate = CriteriaCompiler.ToUntypedPredicate(FilterCriteria, CriteriaCompilerDescriptor.Get(this.ElementType));
					} catch {
						fitPredicate = x => false;
					}
				}
			}
			return fitPredicate(obj);
		}
		static bool alwaysFalse(object dummy) { return false; }
#region IDataSync Members
		int IDataSync.GroupCount { get { return Collection.CanGroup ? Collection.GroupDescriptions.Count : 0; } }
		bool IDataSync.HasFilter { get { return Collection.Filter != null; } }
		List<ListSortInfo> IDataSync.Sort { get { return CreateSortInfo(); } }
		public bool AllowSyncSortingAndGrouping { get; set; } 
		bool IDataSync.ResetCache() {
			Reset();
			return true;
		}
#endregion
#region IListServer2 Members
		bool IListServerCaps.CanFilter { get { return Collection.CanFilter; } }
		bool IListServerCaps.CanGroup { get { return Collection.CanGroup; } }
		bool IListServerCaps.CanSort { get { return Collection.CanSort; } }
#endregion
		public bool IsSupportEditableCollectionView { get { return Collection is IEditableCollectionView; } }
		IEditableCollectionView EditableView { get { return Collection as IEditableCollectionView; } }
		object IEditableCollectionView.AddNew() {
			AttachedProperty.SetAttachedProperty(Collection, "isFilterSortGroupLocked", true);
			try {
#if !SL
			if(EditableView.NewItemPlaceholderPosition == NewItemPlaceholderPosition.None)
				EditableView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
#endif
				return EditableView.AddNew();
			}
			finally {
				AttachedProperty.SetAttachedProperty(Collection, "isFilterSortGroupLocked", false);
			}
		}
		bool IEditableCollectionView.CanAddNew {
			get { return EditableView.CanAddNew; }
		}
		bool IEditableCollectionView.CanCancelEdit {
			get { return EditableView.CanCancelEdit; }
		}
		bool IEditableCollectionView.CanRemove {
			get { return EditableView.CanRemove; }
		}
		void IEditableCollectionView.CancelEdit() {
			if(!EditableView.IsAddingNew) 
				EditableView.CancelEdit();
		}
		void IEditableCollectionView.CancelNew() {
			if(EditableView.IsAddingNew)
				EditableView.CancelNew();
		}
		void IEditableCollectionView.CommitEdit() {
			if(EditableView.IsEditingItem)
				EditableView.CommitEdit();
		}
		void IEditableCollectionView.CommitNew() {
			if(EditableView.IsAddingNew)
				EditableView.CommitNew();
			Reset();
		}
		object IEditableCollectionView.CurrentAddItem {
			get { return EditableView.CurrentAddItem; }
		}
		object IEditableCollectionView.CurrentEditItem {
			get { return EditableView.CurrentEditItem; }
		}
		void IEditableCollectionView.EditItem(object item) {
			if(!EditableView.IsAddingNew)
				EditableView.EditItem(item);
		}
		bool IEditableCollectionView.IsAddingNew {
			get { return EditableView.IsAddingNew; }
		}
		bool IEditableCollectionView.IsEditingItem {
			get { return EditableView.IsEditingItem; }
		}
		NewItemPlaceholderPosition IEditableCollectionView.NewItemPlaceholderPosition {
			get {
				return EditableView.NewItemPlaceholderPosition;
			}
			set {
				EditableView.NewItemPlaceholderPosition = value;
			}
		}
		void IEditableCollectionView.Remove(object item) {
			EditableView.Remove(item);
		}
		void IEditableCollectionView.RemoveAt(int index) {
			EditableView.RemoveAt(index);
		}
#region IListWrapper Members
		Type IListWrapper.WrappedListType { get { return Collection.SourceCollection.GetType(); } }
#endregion
		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if(managerType == typeof(CollectionChangedEventManager)) {
				FilterCollectionChanged(sender, (NotifyCollectionChangedEventArgs)e);
				return true;
			}
			return false;
		}
#if !SL && !DXPORTABLE
		public ReadOnlyCollection<ItemPropertyInfo> ItemProperties {
			get { 
				if(Collection is IItemProperties)
					return ((IItemProperties)Collection).ItemProperties;
				return null;
			}
		}
#endif
	}
	public class CollectionViewListSourceGroupInfo : ListSourceGroupInfo {
		List<object> _Summary = new List<object>();
		public List<ListSourceGroupInfo> ChildrenGroups = new List<ListSourceGroupInfo>();
		public override List<object> Summary {
			get {
				return _Summary;
			}
		}
		public IList Rows;
	}
	static class AttachedProperty {
		static Dictionary<WeakReference, Dictionary<string, object>> Values = new Dictionary<WeakReference, Dictionary<string, object>>();
		static WeakReference GetKey(object o) {
			List<WeakReference> toDelete = new List<WeakReference>();
			try {
				foreach(WeakReference reference in Values.Keys)
					if(!reference.IsAlive)
						toDelete.Add(reference);
					else
						if(reference.Target == o)
							return reference;
				WeakReference reference2 = new WeakReference(o);
				Values.Add(reference2, new Dictionary<string, object>());
				return reference2;
			}
			finally {
				foreach(WeakReference reference in toDelete)
					Values.Remove(reference);
			}
		}
		public static void SetAttachedProperty(object o, string name, object value) {
			WeakReference reference = GetKey(o);
			Dictionary<string, object> values = Values[reference];
			if(values.ContainsKey(name))
				values[name] = value;
			else
				values.Add(name, value);
		}
		public static object GetAttachedProperty(object o, string name) {
			WeakReference reference = GetKey(o);
			object value;
			if(Values[reference].TryGetValue(name, out value))
				return value;
			return null;
		}
	}
	static class CollectionExtensions {
		public static bool AreEqual(IList list1, IList list2) {
			if(list1.Count != list2.Count)
				return false;
			for(int i = 0; i < list1.Count; i++)
				if(!list1[i].Equals(list2[i]))
					return false;
			return true;
		}
	}
#endif
}
