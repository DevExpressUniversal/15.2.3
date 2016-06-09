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
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp {
	public abstract class XafDataView : IBindingList, IList, ICollection, IEnumerable, ITypedList, IDisposable {
		protected IObjectSpace objectSpace;
		protected Type objectType;
		protected ITypeInfo objectTypeInfo;
		protected List<DataViewExpression> expressions;
		private Dictionary<String, DataViewExpression> expressionsByName;
		protected CriteriaOperator criteria;
		protected List<SortProperty> sorting;
		protected Int32 topReturnedObjectsCount;
		protected List<XafDataViewRecord> objects;
		protected Dictionary<Object, XafDataViewRecord> objectsDictionary;
		private XafDataViewExpressionDescriptorCollection propertyDescriptorCollection;
		private Boolean isDisposed;
		private void SetExpressions(IList<DataViewExpression> expressions) {
			this.expressions.Clear();
			expressionsByName.Clear();
			propertyDescriptorCollection.Clear();
			if(expressions != null) {
				this.expressions.AddRange(expressions);
			}
			Int32 expressionIndex = 0;
			ExpressionTypeResolver expressionTypeResolver = new ExpressionTypeResolver(objectSpace.TypesInfo.FindTypeInfo(objectType));
			foreach(DataViewExpression dataViewExpression in this.expressions) {
				expressionsByName[dataViewExpression.Name] = dataViewExpression;
				Type memberType = expressionTypeResolver.GetExpressionType(dataViewExpression.Expression);
				propertyDescriptorCollection.Add(new XafDataViewExpressionDescriptor(dataViewExpression.Name, memberType, expressionIndex));
				expressionIndex++;
			}
		}
		protected void AddDataViewRecordToObjects(XafDataViewRecord dataViewRecord) {
			objects.Add(dataViewRecord);
			Object keyMembersValue = objectSpace.GetKeyValue(dataViewRecord);
			if(keyMembersValue != null) {
				objectsDictionary[keyMembersValue] = dataViewRecord;
			}
		}
		protected void ClearObjects() {
			if(objects != null) {
				objects.Clear();
				objectsDictionary.Clear();
			}
			objects = null;
			objectsDictionary = null;
		}
		protected abstract void InitObjects();
		protected internal void RaiseListChangedEvent(ListChangedEventArgs eventArgs) {
			if(ListChanged != null) {
				ListChanged(this, eventArgs);
			}
		}
		protected internal XafDataViewRecord FindDataViewRecordByObject(Object obj) {
			XafDataViewRecord result = null;
			Object objectKeyValue = objectSpace.GetKeyValue(obj);
			if(objectKeyValue != null) {
				InitObjects();
				objectsDictionary.TryGetValue(objectKeyValue, out result);
			}
			return result;
		}
		public XafDataView(IObjectSpace objectSpace, Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting) {
			this.objectSpace = objectSpace;
			this.objectType = objectType;
			this.expressions = new List<DataViewExpression>();
			this.criteria = criteria;
			this.sorting = new List<SortProperty>();
			if(sorting != null) {
				this.sorting.AddRange(sorting);
			}
			objectTypeInfo = objectSpace.TypesInfo.FindTypeInfo(objectType);
			propertyDescriptorCollection = new XafDataViewExpressionDescriptorCollection(objectTypeInfo);
			expressionsByName = new Dictionary<String, DataViewExpression>();
			SetExpressions(expressions);
		}
		public virtual void Dispose() {
			isDisposed = true;
			ListChanged = null;
			if(objects != null) {
				objects.Clear();
				objectsDictionary.Clear();
				objects = null;
				objectsDictionary = null;
			}
			objectSpace = null;
			propertyDescriptorCollection = null;
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public Type ObjectType {
			get { return objectType; }
		}
		public IList<DataViewExpression> Expressions {
			get { return expressions.AsReadOnly(); }
			set {
				ClearObjects();
				SetExpressions(value);
				RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, -1, -1));
				RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
			}
		}
		public CriteriaOperator Criteria {
			get { return criteria; }
			set {
				if(!ReferenceEquals(criteria, value)) {
					criteria = value;
					ClearObjects();
					RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
				}
			}
		}
		public IList<SortProperty> Sorting {
			get { return sorting.AsReadOnly(); }
			set {
				sorting.Clear();
				if(value != null) {
					sorting.AddRange(value);
				}
				ClearObjects();
				RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
			}
		}
		public Int32 TopReturnedObjectsCount {
			get { return topReturnedObjectsCount; }
			set { topReturnedObjectsCount = value; }
		}
		public Boolean IsLoaded {
			get { return (objects != null); }
		}
		public XafDataViewRecord this[Int32 index] {
			get {
				InitObjects();
				if((index >= 0) && (index < objects.Count)) {
					return objects[index];
				}
				else {
					return null;
				}
			}
		}
		public void Reload() {
			ClearObjects();
			RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
		}
		public DataViewExpression FindExpression(String name) {
			DataViewExpression result = null;
			if(expressionsByName.ContainsKey(name)) {
				result = expressionsByName[name];
			}
			return result;
		}
		void IBindingList.AddIndex(PropertyDescriptor property) {
		}
		Object IBindingList.AddNew() {
			throw new Exception("AddNew is not allowed.");
		}
		void IBindingList.ApplySort(PropertyDescriptor memberDescriptor, ListSortDirection direction) {
			sorting.Clear();
			SortProperty sortProperty = new SortProperty(memberDescriptor.Name, (direction == ListSortDirection.Ascending) ? SortingDirection.Ascending : SortingDirection.Descending);
			sorting.Add(sortProperty);
			ClearObjects();
			RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
		}
		void IBindingList.RemoveSort() {
			if(sorting.Count > 0) {
				sorting.Clear();
				ClearObjects();
				RaiseListChangedEvent(new ListChangedEventArgs(ListChangedType.Reset, 0));
			}
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) {
		}
		Int32 IBindingList.Find(PropertyDescriptor property, Object key) {
			InitObjects();
			for(Int32 i = 0; i < objects.Count; i++) {
				Object propertyValue = property.GetValue(objects[i]);
				if((propertyValue != null) && (propertyValue != DBNull.Value)) {
					if(propertyValue.Equals(key)) {
						return i;
					}
				}
				else {
					if(key == null) {
						return i;
					}
				}
			}
			return -1;
		}
		public Boolean AllowNew {
			get { return false; }
		}
		public Boolean AllowEdit {
			get { return false; }
		}
		public Boolean AllowRemove {
			get { return false; }
		}
		Boolean IBindingList.IsSorted {
			get { return (sorting.Count > 0); }
		}
		Boolean IBindingList.SupportsSorting {
			get { return true; }
		}
		PropertyDescriptor IBindingList.SortProperty {
			get {
				if(sorting.Count > 0) {
					return ((ITypedList)this).GetItemProperties(null).Find(sorting[0].PropertyName, false);
				}
				else {
					return null;
				}
			}
		}
		ListSortDirection IBindingList.SortDirection {
			get {
				if((sorting.Count > 0) && (sorting[0].Direction == SortingDirection.Descending)) {
					return ListSortDirection.Descending;
				}
				else {
					return ListSortDirection.Ascending;
				}
			}
		}
		Boolean IBindingList.SupportsSearching {
			get { return true; }
		}
		Boolean IBindingList.SupportsChangeNotification {
			get { return true; }
		}
		public event ListChangedEventHandler ListChanged;
		public Int32 Add(Object obj) {
			throw new NotSupportedException();
		}
		public void Insert(Int32 index, Object obj) {
			throw new NotSupportedException();
		}
		public void Remove(Object obj) {
			throw new NotSupportedException();
		}
		public void RemoveAt(Int32 index) {
			throw new NotSupportedException();
		}
		public void Clear() {
			throw new NotSupportedException();
		}
		public Boolean Contains(Object obj) {
			InitObjects();
			return objects.Contains(obj);
		}
		public Int32 IndexOf(Object obj) {
			if(obj is XafDataViewRecord) {
				InitObjects();
				return objects.IndexOf((XafDataViewRecord)obj);
			}
			else {
				return -1;
			}
		}
		public Boolean IsReadOnly {
			get { return true; }
		}
		public Boolean IsFixedSize {
			get { return true; }
		}
		Object IList.this[Int32 index] {
			get {
				InitObjects();
				if((index >= 0) && (index < objects.Count)) {
					return objects[index];
				}
				else {
					return null;
				}
			}
			set {
				throw new Exception("List is read only");
			}
		}
		public void CopyTo(Array array, Int32 index) {
			InitObjects();
			((IList)objects).CopyTo(array, index);
		}
		public Int32 Count {
			get {
				if(isDisposed) {
					return 0;
				}
				else {
					InitObjects();
					return objects.Count;
				}
			}
		}
		public Boolean IsSynchronized {
			get { return false; }
		}
		public Object SyncRoot {
			get { return this; }
		}
		IEnumerator IEnumerable.GetEnumerator() {
			InitObjects();
			return objects.GetEnumerator();
		}
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if((listAccessors != null) && (listAccessors.Length > 0)) {
				throw new Exception("listAccessors != null");
			}
			else {
				return propertyDescriptorCollection;
			}
		}
		String ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return "";
		}
	}
}
