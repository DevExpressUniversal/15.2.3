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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.ExpressApp {
	public class ProxyCollection : IBindingList, ITypedList, ISupportInitialize, IDisplayNameProvider, IDisposableExt, IFilterParametersOwner {
		private Boolean isDisposed;
		private IObjectSpace objectSpace;
		private ITypeInfo typeInfo;
		private Object originalCollection;
		private IList originalList;
		private IBindingList originalBindingList;
		private Boolean isAddOrRemove;
		private Boolean isBindingListEmulationMode;
		private Boolean isFilteredMode;
		private CriteriaOperator filter;
		private ExpressionEvaluator expressionEvaluator;
		private IList proxyList;
		private IDictionary proxyListDictionary;
		private Boolean isProxyListInitializing;
		private Boolean isProxyListInitialized;
		private XafPropertyDescriptorCollection propertyDescriptorCollection;
		private Dictionary<String, PropertyDescriptorCollection> propertyDescriptorCollections;
		private List<IFilterParameter> parameters = new List<IFilterParameter>();
		private IList WorkingList {
			get {
				if(isFilteredMode || isBindingListEmulationMode) {
					InitProxyList();
					return proxyList;
				}
				else {
					return originalList;
				}
			}
		}
		private Boolean FitToFilter(Object obj) {
			return expressionEvaluator.Fit(obj);
		}
		private void ClearProxyList() {
			if(!isProxyListInitializing) {
				isProxyListInitialized = false;
				if(proxyList != null) {
					proxyList.Clear();
					proxyList = null;
					proxyListDictionary.Clear();
					proxyListDictionary = null;
				}
			}
		}
		private void InitProxyList() {
			if((isFilteredMode || isBindingListEmulationMode) && (originalList != null) && !isProxyListInitialized && !isProxyListInitializing) {
				isProxyListInitializing = true;
				try {
					proxyList = new List<Object>();
					proxyListDictionary = new Dictionary<Object, Object>();
					foreach(Object obj in originalList) {
						if(isFilteredMode) {
							if(FitToFilter(obj)) {
								proxyList.Add(obj);
								proxyListDictionary[obj] = null;
							}
						}
						else {
							proxyList.Add(obj);
							proxyListDictionary[obj] = null;
						}
					}
				}
				finally {
					isProxyListInitializing = false;
					isProxyListInitialized = true;
				}
			}
		}
		private void ResetProxyList() {
			ClearProxyList();
			InitProxyList();
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		private void bindingList_ListChanged(Object sender, ListChangedEventArgs e) {
			if((originalCollection != null) && !isAddOrRemove) {
				if(isFilteredMode) {
					if(e.ListChangedType == ListChangedType.ItemAdded) {
						Object obj = originalList[e.NewIndex];
						if(FitToFilter(obj)) {
							InitProxyList();
							if(proxyList != null) {
								Int32 indexInProxyList = proxyList.IndexOf(obj);
								if(indexInProxyList < 0) {
									indexInProxyList = proxyList.Add(obj);
									proxyListDictionary[obj] = null;
									OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, indexInProxyList));
								}
							}
						}
					}
					else if(e.ListChangedType == ListChangedType.ItemChanged) {
						Object obj = originalList[e.NewIndex];
						if(FitToFilter(obj)) {
							InitProxyList();
						}
						if((proxyList != null) && proxyListDictionary.Contains(obj)) {
							Int32 index = proxyList.IndexOf(obj);
							OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
						}
					}
					else if((e.ListChangedType == ListChangedType.ItemDeleted)
						|| (e.ListChangedType == ListChangedType.Reset)
						|| (e.ListChangedType == ListChangedType.ItemMoved)) {
						ResetProxyList();
					}
					else {
						OnListChanged(e);
					}
				}
				else {
					OnListChanged(e);
				}
			}
		}
		private void DoOnBindingListEmulationMode(Object obj) {
			if((originalList != null) && !isProxyListInitializing && !isAddOrRemove && typeInfo.Type.IsAssignableFrom(obj.GetType())) {
				Int32 indexInOriginalList = originalList.IndexOf(obj);
				if(indexInOriginalList >= 0) {
					InitProxyList();
					if(proxyList != null) {
						Int32 indexInProxyList = proxyList.IndexOf(obj);
						if(indexInProxyList >= 0) {
							OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, indexInProxyList));
						}
						else {
							if(!isFilteredMode || FitToFilter(obj)) {
								indexInProxyList = proxyList.Add(obj);
								proxyListDictionary[obj] = null;
								OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, indexInProxyList));
							}
						}
					}
				}
				else {
					InitProxyList();
					if(proxyList != null) {
						Int32 indexInProxyList = proxyList.IndexOf(obj);
						if(indexInProxyList >= 0) {
							proxyList.Remove(obj);
							proxyListDictionary.Remove(obj);
							OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, indexInProxyList));
						}
					}
				}
			}
		}
		private void objectSpace_ObjectChanged(Object sender, ObjectChangedEventArgs e) {
			DoOnBindingListEmulationMode(e.Object);
		}
		private void objectSpace_ObjectReloaded(Object sender, ObjectManipulatingEventArgs e) {
			DoOnBindingListEmulationMode(e.Object);
		}
		private void objectSpace_ObjectDeleted(Object sender, ObjectsManipulatingEventArgs e) {
			if((originalList != null) && isFilteredMode) {
				InitProxyList();
				if(proxyList != null) {
					foreach(Object obj in e.Objects) {
						Int32 index = proxyList.IndexOf(obj);
						if(index >= 0) {
							proxyList.RemoveAt(index);
							proxyListDictionary.Remove(obj);
							OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
						}
					}
				}
			}
		}
		private Boolean IsAccessibleMember(IMemberInfo memberInfo, ITypeInfo owner, Boolean includeInvisibleMember) {
			return includeInvisibleMember || memberInfo.IsVisible || owner.KeyMembers.Contains(memberInfo);
		}
		private XafPropertyDescriptorCollection CreatePropertyDescriptorCollection(ITypeInfo pdcTypeInfo, Boolean includeInvisibleMembers) {
			XafPropertyDescriptorCollection xafPropertyDescriptorCollection = new XafPropertyDescriptorCollection(pdcTypeInfo);
			foreach(IMemberInfo memberInfo in pdcTypeInfo.Members) {
				if(IsAccessibleMember(memberInfo, pdcTypeInfo, includeInvisibleMembers)) {
					xafPropertyDescriptorCollection.CreatePropertyDescriptor(memberInfo, memberInfo.Name);
				}
			}
			return xafPropertyDescriptorCollection;
		}
		private bool CollectionContainsDescriptor(PropertyDescriptorCollection propertyDescriptorCollection, String name) {
			foreach(PropertyDescriptor propertyDescriptor in propertyDescriptorCollection) {
				if(propertyDescriptor.Name == name) {
					return true;
				}
			}
			return false;
		}
		private void FillPropertyDescriptorCollection(XafPropertyDescriptorCollection xafPropertyDescriptorCollection, PropertyDescriptorCollection sourcePropertyDescriptors) {
			foreach(PropertyDescriptor propertyDescriptor in sourcePropertyDescriptors) {
				String name = propertyDescriptor.Name;
				IMemberInfo memberInfo = typeInfo.FindMember(name);
				if(memberInfo != null && !CollectionContainsDescriptor(xafPropertyDescriptorCollection, name)) {
					xafPropertyDescriptorCollection.CreatePropertyDescriptor(memberInfo, name);
				}
			}
		}
		private XafPropertyDescriptorCollection CreatePropertyDescriptorCollection() {
			XafPropertyDescriptorCollection pdcFromTypeInfo = CreatePropertyDescriptorCollection(typeInfo, typeInfo.IsInterface);
			ITypedList typedList = originalCollection as ITypedList;
			if(typedList != null) {
				XafPropertyDescriptorCollection xafPropertyDescriptorCollection = new XafPropertyDescriptorCollection(typeInfo);
				FillPropertyDescriptorCollection(xafPropertyDescriptorCollection, typedList.GetItemProperties(null));
				FillPropertyDescriptorCollection(xafPropertyDescriptorCollection, pdcFromTypeInfo);
				return xafPropertyDescriptorCollection;
			}
			else {
				return pdcFromTypeInfo;
			}
		}
		protected virtual void OnListChanged(ListChangedEventArgs eventArgs) {
			if(ListChanged != null) {
				ListChanged(this, eventArgs);
			}
		}
		public ProxyCollection(IObjectSpace objectSpace, ITypeInfo typeInfo, Object collection) {
			this.objectSpace = objectSpace;
			this.typeInfo = typeInfo;
			SetCollection(collection);
			if(objectSpace != null) {
				objectSpace.ObjectDeleted += new EventHandler<ObjectsManipulatingEventArgs>(objectSpace_ObjectDeleted);
			}
			propertyDescriptorCollection = CreatePropertyDescriptorCollection();
			propertyDescriptorCollections = new Dictionary<String, PropertyDescriptorCollection>();
		}
		public void SetCollection(Object collection) {
			if(originalCollection != collection) {
				if(originalBindingList != null) {
					originalBindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
					originalBindingList = null;
				}
				ClearProxyList();
				if(objectSpace != null) {
					objectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
					objectSpace.ObjectReloaded -= new EventHandler<ObjectManipulatingEventArgs>(objectSpace_ObjectReloaded);
				}
				originalCollection = collection;
				originalList = ListHelper.GetList(originalCollection);
				if((originalList == null) && ListHelper.IsGenericListSupported(originalCollection)) {
					originalList = ListHelper.CreateListWrapperForGenericList(originalCollection);
				}
				originalBindingList = ListHelper.GetBindingList(originalCollection);
				if(originalBindingList != null) {
					originalBindingList.ListChanged += new ListChangedEventHandler(bindingList_ListChanged);
				}
				else if(objectSpace != null) {
					objectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
					objectSpace.ObjectReloaded += new EventHandler<ObjectManipulatingEventArgs>(objectSpace_ObjectReloaded);
				}
				isBindingListEmulationMode = (originalBindingList == null) && (objectSpace != null);
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
			}
		}
		public void Refresh() {
			ClearProxyList();
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		public CriteriaOperator Filter {
			get { return filter; }
			set {
				if(!ReferenceEquals(filter, value)) {
					filter = value;
					Boolean needRiseReset = !isBindingListEmulationMode || isProxyListInitialized;
					ClearProxyList();
					if(ReferenceEquals(filter, null) || (objectSpace == null)) {
						expressionEvaluator = null;
					}
					else {
						expressionEvaluator = objectSpace.GetExpressionEvaluator(typeInfo.Type, filter);
					}
					isFilteredMode = (expressionEvaluator != null);
					if(needRiseReset) {
						OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
					}
				}
			}
		}
		public Object OriginalCollection {
			get { return originalCollection; }
		}
		public String DisplayableMembers {
			get { return propertyDescriptorCollection.DisplayableMembers; }
			set {
				if(propertyDescriptorCollection.DisplayableMembers != value) {
					propertyDescriptorCollection.DisplayableMembers = value;
					OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1, -1));
				}
			}
		}
		public void AddIndex(PropertyDescriptor property) {
			if(originalBindingList != null) {
				originalBindingList.AddIndex(property);
			}
		}
		public Object AddNew() {
			if(originalBindingList != null) {
				return originalBindingList.AddNew();
			}
			if((originalList != null) && (objectSpace != null)) {
				Object newObject = objectSpace.CreateObject(typeInfo.Type);
				Add(newObject);
				return newObject;
			}
			return null;
		}
		public void ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			if(originalBindingList != null) {
				originalBindingList.ApplySort(property, direction);
			}
		}
		public Int32 Find(PropertyDescriptor property, Object key) {
			if(originalBindingList != null) {
				return originalBindingList.Find(property, key);
			}
			else {
				return -1;
			}
		}
		public void RemoveIndex(PropertyDescriptor property) {
			if(originalBindingList != null) {
				originalBindingList.RemoveIndex(property);
			}
		}
		public void RemoveSort() {
			if(originalBindingList != null) {
				originalBindingList.RemoveSort();
			}
		}
		public Boolean AllowEdit {
			get {
				if(originalBindingList != null) {
					return originalBindingList.AllowEdit;
				}
				else {
					return true;
				}
			}
		}
		public Boolean AllowNew {
			get {
				Boolean result = false;
				if(originalBindingList != null) {
					result = originalBindingList.AllowNew;
				} else {
					result = !IsFixedSize && !IsReadOnly;
				}
				return result;
			}
		}
		public Boolean AllowRemove {
			get {
				Boolean result = false;
				if(originalBindingList != null) {
					result = originalBindingList.AllowRemove;
				}
				else {
					result = !IsFixedSize && !IsReadOnly;
				}
				return result;
			}
		}
		public Boolean IsSorted {
			get {
				if(originalBindingList != null) {
					return originalBindingList.IsSorted;
				}
				else {
					return false;
				}
			}
		}
		public ListSortDirection SortDirection {
			get {
				if(originalBindingList != null) {
					return originalBindingList.SortDirection;
				}
				else {
					return ListSortDirection.Ascending;
				}
			}
		}
		public PropertyDescriptor SortProperty {
			get {
				if(originalBindingList != null) {
					return originalBindingList.SortProperty;
				}
				else {
					return null;
				}
			}
		}
		public Boolean SupportsChangeNotification {
			get {
				if(originalBindingList != null) {
					return originalBindingList.SupportsChangeNotification;
				}
				return (originalList != null);
			}
		}
		public Boolean SupportsSearching {
			get {
				if(originalBindingList != null) {
					return originalBindingList.SupportsSearching;
				}
				else {
					return false;
				}
			}
		}
		public Boolean SupportsSorting {
			get {
				if(originalBindingList != null) {
					return originalBindingList.SupportsSorting;
				}
				else {
					return false;
				}
			}
		}
		public event ListChangedEventHandler ListChanged;
		public Int32 Add(Object obj) {
			Int32 result = -1;
			if(originalList != null) {
				if(isFilteredMode || isBindingListEmulationMode) {
					Int32 indexInOriginalList = originalList.IndexOf(obj);
					Int32 indexInProxyList = -1;
					if(indexInOriginalList < 0) {
						isAddOrRemove = true;
						try {
							indexInOriginalList = originalList.Add(obj);
						}
						finally {
							isAddOrRemove = false;
						}
					}
					if(indexInOriginalList >= 0) {
						InitProxyList();
						if(proxyList != null) {
							indexInProxyList = proxyList.IndexOf(obj);
							if(indexInProxyList < 0) {
								indexInProxyList = proxyList.Add(obj);
								proxyListDictionary[obj] = null;
								OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, indexInProxyList));
							}
						}
					}
					result = indexInProxyList;
				}
				else {
					result = originalList.IndexOf(obj);
					if(result < 0) {
						isAddOrRemove = true;
						try {
							result = originalList.Add(obj);
						}
						finally {
							isAddOrRemove = false;
						}
						if(result >= 0) {
							OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, result));
						}
					}
				}
			}
			return result;
		}
		public void Clear() {
			if(proxyList != null) {
				proxyList.Clear();
				proxyListDictionary.Clear();
			}
			if(originalList != null) {
				originalList.Clear();
			}
		}
		public Boolean Contains(Object obj) {
			if((obj != null) && (WorkingList != null)) {
				if(isFilteredMode || isBindingListEmulationMode) {
					return proxyListDictionary.Contains(obj);
				}
				else {
					return WorkingList.Contains(obj);
				}
			}
			else {
				return false;
			}
		}
		public Int32 IndexOf(Object obj) {
			if(WorkingList != null) {
				return WorkingList.IndexOf(obj);
			}
			else {
				return -1;
			}
		}
		public void Insert(Int32 index, Object obj) {
			if(originalList != null) {
				if(isFilteredMode || isBindingListEmulationMode) {
					Int32 indexInOriginalList = originalList.IndexOf(obj);
					if(indexInOriginalList < 0) {
						isAddOrRemove = true;
						try {
							indexInOriginalList = originalList.Add(obj);
						}
						finally {
							isAddOrRemove = false;
						}
					}
					if(indexInOriginalList >= 0) {
						InitProxyList();
						if(proxyList != null) {
							Int32 indexInProxyList = proxyList.IndexOf(obj);
							if(indexInProxyList < 0) {
								proxyList.Insert(index, obj);
								proxyListDictionary[obj] = null;
								OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
							}
						}
					}
				}
				else {
					if(originalList.IndexOf(obj) < 0) {
						isAddOrRemove = true;
						try {
							originalList.Insert(index, obj);
						}
						finally {
							isAddOrRemove = false;
						}
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
					}
				}
			}
		}
		public void Remove(Object obj) {
			if(originalList != null) {
				if(isFilteredMode || isBindingListEmulationMode) {
					InitProxyList();
					if(proxyList != null) {
						isAddOrRemove = true;
						try {
							originalList.Remove(obj);
						}
						finally {
							isAddOrRemove = false;
						}
						Int32 indexInProxyList = proxyList.IndexOf(obj);
						if(indexInProxyList >= 0) {
							proxyList.Remove(obj);
							proxyListDictionary.Remove(obj);
							OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, indexInProxyList));
						}
					}
				}
				else {
					Int32 indexInOriginalList = originalList.IndexOf(obj);
					if(indexInOriginalList >= 0) {
						isAddOrRemove = true;
						try {
							originalList.RemoveAt(indexInOriginalList);
						}
						finally {
							isAddOrRemove = false;
						}
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, indexInOriginalList));
					}
				}
			}
		}
		public void RemoveAt(Int32 index) {
			if(originalList != null) {
				if(isFilteredMode || isBindingListEmulationMode) {
					InitProxyList();
					if(proxyList != null) {
						Object obj = proxyList[index];
						isAddOrRemove = true;
						try {
							originalList.Remove(obj);
						}
						finally {
							isAddOrRemove = false;
						}
						proxyList.RemoveAt(index);
						proxyListDictionary.Remove(obj);
						OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
					}
				}
				else {
					isAddOrRemove = true;
					try {
						originalList.RemoveAt(index);
					}
					finally {
						isAddOrRemove = false;
					}
					OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
				}
			}
		}
		public Boolean IsFixedSize {
			get {
				if(originalList != null) {
					return originalList.IsFixedSize;
				}
				else {
					return true;
				}
			}
		}
		public Boolean IsReadOnly {
			get {
				if(originalList != null) {
					return originalList.IsReadOnly;
				}
				else {
					return true;
				}
			}
		}
		public Object this[Int32 index] {
			get {
				if(WorkingList != null) {
					return WorkingList[index];
				}
				else {
					return null;
				}
			}
			set { ; }
		}
		public void CopyTo(Array array, Int32 index) {
			if(WorkingList != null) {
				WorkingList.CopyTo(array, index);
			}
		}
		public Int32 Count {
			get {
				if(WorkingList != null) {
					return WorkingList.Count;
				}
				else {
					return 0;
				}
			}
		}
		public Boolean IsSynchronized {
			get {
				if(WorkingList != null) {
					return WorkingList.IsSynchronized;
				}
				else {
					return false;
				}
			}
		}
		public Object SyncRoot {
			get {
				if(WorkingList != null) {
					return WorkingList.SyncRoot;
				}
				else {
					return null;
				}
			}
		}
		public IEnumerator GetEnumerator() {
			if(WorkingList != null) {
				return WorkingList.GetEnumerator();
			}
			else {
				return null;
			}
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			PropertyDescriptorCollection result = null;
			if(listAccessors == null) {
				result = propertyDescriptorCollection;
			}
			else {
				if(listAccessors.Length > 0) {
					PropertyDescriptor listAccessor = listAccessors[listAccessors.Length - 1];
					if(!propertyDescriptorCollections.TryGetValue(listAccessor.Name, out result)) {
						Type propertyType = listAccessor.PropertyType;
						ITypeInfo propertyTypeInfo = XafTypesInfo.Instance.FindTypeInfo(propertyType);
						if(propertyTypeInfo != null) {
							if(propertyTypeInfo.IsDomainComponent) {
								result = CreatePropertyDescriptorCollection(propertyTypeInfo, false);
							}
							else if(propertyTypeInfo.IsListType) {
								Type ownerType = listAccessor.ComponentType;
								ITypeInfo ownerTypeInfo = XafTypesInfo.Instance.FindTypeInfo(ownerType);
								IMemberInfo memberInfo = ownerTypeInfo.FindMember(listAccessor.Name);
								if(memberInfo.ListElementType != null) {
									ITypeInfo listElementTypeInfo = XafTypesInfo.Instance.FindTypeInfo(memberInfo.ListElementType);
									if(listElementTypeInfo != null && listElementTypeInfo.IsDomainComponent) {
										result = CreatePropertyDescriptorCollection(listElementTypeInfo, false);
									}
								}
							}
						}
						propertyDescriptorCollections[listAccessor.Name] = result;
					}
				}
			}
			if(result == null) {
				result = new PropertyDescriptorCollection(null);
			}
			return result;
		}
		public String GetListName(PropertyDescriptor[] listAccessors) {
			return null;
		}
		public void BeginInit() {
			if(originalCollection is ISupportInitialize) {
				((ISupportInitialize)originalCollection).BeginInit();
			}
		}
		public void EndInit() {
			if(originalCollection is ISupportInitialize) {
				((ISupportInitialize)originalCollection).EndInit();
			}
		}
		public void Dispose() {
			if(!isDisposed) {
				isDisposed = true;
				ListChanged = null;
				if(objectSpace != null) {
					objectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(objectSpace_ObjectChanged);
					objectSpace.ObjectDeleted -= new EventHandler<ObjectsManipulatingEventArgs>(objectSpace_ObjectDeleted);
					objectSpace.ObjectReloaded -= new EventHandler<ObjectManipulatingEventArgs>(objectSpace_ObjectReloaded);
					objectSpace = null;
				}
				ClearProxyList();
				originalCollection = null;
				if(originalBindingList != null) {
					originalBindingList.ListChanged -= new ListChangedEventHandler(bindingList_ListChanged);
					originalBindingList = null;
				}
				if(propertyDescriptorCollections != null) {
					propertyDescriptorCollections.Clear();
					propertyDescriptorCollections = null;
				}
			}
		}
		public Boolean IsDisposed {
			get { return isDisposed; }
		}
		String IDisplayNameProvider.GetDataSourceDisplayName() {
			return CaptionHelper.GetClassCaption(typeInfo.FullName);
		}
		String IDisplayNameProvider.GetFieldDisplayName(String[] fieldAccessors) {
			Int32 memberCount = fieldAccessors.Length;
			ITypeInfo memberOwner = typeInfo;
			IMemberInfo lastMember = null;
			for(int i = 0; i < memberCount; i++) {
				String memberName = fieldAccessors[i];
				if(String.IsNullOrEmpty(memberName)
					|| memberName.Contains("!")
					|| memberName.ToLower() == "this"
					|| memberName.ToLower().EndsWith(".this")) {
					return null;
				}
				lastMember = memberOwner.FindMember(memberName);
				if(lastMember == null || !IsAccessibleMember(lastMember, memberOwner, false)) {
					return null;
				}
				if(lastMember.IsList) {
					memberOwner = lastMember.ListElementTypeInfo;
				}
				else {
					memberOwner = lastMember.MemberTypeInfo;
				}
			}
			return CaptionHelper.GetMemberCaption(lastMember.Owner, fieldAccessors[memberCount - 1]);
		}
		void IFilterParametersOwner.AddParameter(String name, Type type) {
			throw new InvalidOperationException();
		}
		bool IFilterParametersOwner.CanAddParameters {
			get { return false; }
		}
		IList<IFilterParameter> IFilterParametersOwner.Parameters {
			get { return parameters; }
		}
	}
}
