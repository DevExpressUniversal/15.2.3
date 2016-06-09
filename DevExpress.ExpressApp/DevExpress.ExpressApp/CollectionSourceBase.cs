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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp {
	public enum CollectionSourceMode { Normal, Proxy }
	public enum CollectionSourceDataAccessMode { Client, Server, DataView }
	public abstract class CollectionSourceBase : IDisposable {
		public static CriteriaOperator EmptyCollectionCriteria = new BinaryOperator(new ConstantValue(1), new ConstantValue(2), BinaryOperatorType.Equal);
		private Int32 applyCriteriaLockCount;
		private Boolean isCriteriaChanged;
		protected IObjectSpace objectSpace;
		protected Boolean canApplySorting;
		protected Object originalCollection;
		protected ProxyCollection proxyCollection;
		private Object collection;
		private Boolean isCriteriaLocked;
		private Boolean? allowAdd = null;
		private Boolean? allowRemove = null;
		private String defaultDisplayableProperties = "";
		protected String displayableProperties = "";
		private LightDictionary<String, CriteriaOperator> criteria = new LightDictionary<String, CriteriaOperator>();
		private BindingList<SortProperty> sorting;
		private Boolean isSortingChanging;
		private Int32 topReturnedObjects = 0;
		private Boolean deleteObjectOnRemove;
		private Boolean isCollectionResetting;
		protected CollectionSourceMode mode;
		protected CollectionSourceDataAccessMode dataAccessMode;
		protected internal Object OriginalCollection {
			get { return originalCollection; }
		}
		private void objectSpace_Reloaded(Object sender, EventArgs e) {
			OnObjectSpaceReloaded();
			if(objectSpace != null) {
				ResetCollection();
			}
		}
		private void criteria_Changing(Object sender, EventArgs e) {
			OnCriteriaChanging();
		}
		private void criteria_Changed(Object sender, EventArgs e) {
			isCriteriaChanged = true;
			OnCriteriaChanged();
		}
		private void sorting_ListChanged(Object sender, ListChangedEventArgs e) {
			ApplySorting();
		}
		private void OnCriteriaChanged() {
			if(!IsCriteriaChanging() && isCriteriaChanged) {
				isCriteriaChanged = false;
				if(NeedResetCollectionOnCriteriaChanged()) {
					ResetCollection();
				}
				else {
					ApplyCriteria();
				}
			}
		}
		private void ApplyCriteria() {
			if(collection != null) {
				OnCriteriaApplying();
				CriteriaOperator externalCriteria = null;
				try {
					externalCriteria = GetExternalCriteria();
					ApplyCriteriaCore(externalCriteria);
					OnCriteriaApplied();
				}
				catch(Exception e) {
					throw new Exception(String.Format("An error occurs while applying the '{0}' criteria: '{1}'",
						ReferenceEquals(externalCriteria, null) ? "" : externalCriteria.ToString(), e.Message), e);
				}
			}
		}
		private void ApplySorting() {
			if(canApplySorting && !isSortingChanging) {
				ApplySorting(sorting);
			}
		}
		private void SetTopReturnedObjects(Int32 topReturnedObjects) {
			if(topReturnedObjects < 0) {
				throw new ArgumentException("The 'topReturnedObjects' argument value must be greater than or equal to 0", "topReturnedObjects");
			}
			this.topReturnedObjects = topReturnedObjects;
			ApplyTopReturnedObjects();
		}
		private Boolean IsCriteriaChanging() {
			return applyCriteriaLockCount > 0;
		}
		protected virtual void ApplyTopReturnedObjects() { }
		protected virtual void ApplyDeleteObjectOnRemove() { }
		protected virtual Boolean NeedResetCollectionOnCriteriaChanged() {
			return false;
		}
		protected CriteriaOperator GetExternalCriteria() {
			return CombineCriteria(criteria.GetValues().ToArray());
		}
		protected internal Boolean GetAllowAdd(out String diagnosticInfo) {
			diagnosticInfo = "";
			if(allowAdd == null) {
				return DefaultAllowAdd(out diagnosticInfo);
			}
			else {
				if(!(Boolean)allowAdd) {
					diagnosticInfo = "AllowAdd is forbidden via the SetAllowAdd method";
				}
				return (Boolean)allowAdd;
			}
		}
		protected internal Boolean GetAllowRemove(out String diagnosticInfo) {
			diagnosticInfo = "";
			if(allowRemove == null) {
				return DefaultAllowRemove(out diagnosticInfo);
			}
			else {
				if(!(Boolean)allowRemove) {
					diagnosticInfo = "AllowRemove is forbidden via the SetAllowRemove method";
				}
				return (Boolean)allowRemove;
			}
		}
		protected internal virtual void SetObjectSpace(IObjectSpace objectSpace) {
			if(this.objectSpace != null) {
				this.objectSpace.Reloaded -= new EventHandler(objectSpace_Reloaded);
			}
			this.objectSpace = objectSpace;
			if(this.objectSpace != null) {
				if(objectSpace.IsDisposed) {
					throw new ObjectDisposedException(GetType().FullName);
				}
				this.objectSpace.Reloaded += new EventHandler(objectSpace_Reloaded);
			}
		}
		protected internal void SetIsCriteriaLocked(Boolean value) {
			isCriteriaLocked = value;
		}
		protected internal void InitCollection() {
			if(collection == null) {
				ResetCollection();
			}
		}
		protected internal virtual CriteriaOperator GetTotalCriteria() {
			return null;
		}
		protected abstract Object CreateCollection();
		protected virtual void DisposePreviousCollection(Object previousCollection) {
		}
		protected abstract void ApplyCriteriaCore(CriteriaOperator criteria);
		protected virtual void ApplySorting(IList<SortProperty> sorting) {
			objectSpace.SetCollectionSorting(originalCollection, sorting);
		}
		protected virtual String GetDisplayableProperties() {
			String result = displayableProperties;
			if(mode == CollectionSourceMode.Proxy) {
				if(proxyCollection != null) {
					result = proxyCollection.DisplayableMembers;
				}
			}
			else if(mode == CollectionSourceMode.Normal) {
				if(originalCollection != null) {
					result = objectSpace.GetDisplayableProperties(originalCollection);
				}
			}
			return result;
		}
		private String CombineDisplayableProperties() {
			String result = "";
			if(dataAccessMode == CollectionSourceDataAccessMode.DataView) {
				if(!String.IsNullOrWhiteSpace(defaultDisplayableProperties)) {
					result = defaultDisplayableProperties + ";" + displayableProperties;
				}
				else {
					result = displayableProperties;
				}
			}
			else {
				List<String> displayablePropertiesList = new List<String>();
				if(!String.IsNullOrWhiteSpace(defaultDisplayableProperties)) {
					displayablePropertiesList.AddRange(defaultDisplayableProperties.Split(';'));
				}
				if(!String.IsNullOrWhiteSpace(displayableProperties)) {
					foreach(String displayableProperty in displayableProperties.Split(';')) {
						IMemberInfo displayablePropertyInfo = ObjectTypeInfo.FindMember(displayableProperty);
						if((displayablePropertyInfo != null) && !displayablePropertiesList.Contains(displayablePropertyInfo.BindingName)) {
							displayablePropertiesList.Add(displayablePropertyInfo.BindingName);
						}
					}
				}
				result = String.Join(";", displayablePropertiesList);
			}
			return result;
		}
		protected virtual void SetDisplayablePropertiesToCollection() {
			if((proxyCollection != null) || (originalCollection != null)) {
				String combinedDisplayableProperties = CombineDisplayableProperties();
				objectSpace.SetDisplayableProperties(originalCollection, combinedDisplayableProperties);
				objectSpace.SetDisplayableProperties(proxyCollection, combinedDisplayableProperties);
			}
		}
		protected virtual void OnObjectSpaceReloaded() {
		}
		protected virtual Boolean DefaultAllowAdd(out String diagnosticInfo) {
			Boolean result = true;
			diagnosticInfo = "";
			IList list = ListHelper.GetList(collection);
			if(list != null) {
				if(list.IsReadOnly || list.IsFixedSize) {
					result = false;
					diagnosticInfo = "Collection is readOnly";
				}
			}
			else {
				result = false;
				diagnosticInfo = "Collection is null or not loaded";
			}
			return result;
		}
		protected virtual Boolean DefaultAllowRemove(out String diagnosticInfo) {
			Boolean result = true;
			diagnosticInfo = "";
			IList list = ListHelper.GetList(collection);
			if(list != null) {
				if(list.IsReadOnly || list.IsFixedSize) {
					result = false;
					diagnosticInfo = "Collection is readOnly";
				}
				IBindingList bindingList = ListHelper.GetBindingList(collection);
				if(bindingList != null) {
					result = bindingList.AllowRemove;
					diagnosticInfo = "Value from bindingList.AllowRemove";
				}
			}
			else {
				result = false;
				diagnosticInfo = "Collection is null or not loaded";
			}
			return result;
		}
		protected virtual void OnCriteriaApplying() {
			if(CriteriaApplying != null) {
				CriteriaApplying(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCriteriaApplied() {
			if(CriteriaApplied != null) {
				CriteriaApplied(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCollectionChanging() {
			if(CollectionChanging != null) {
				CollectionChanging(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCollectionChanged() {
			if(CollectionChanged != null) {
				CollectionChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCriteriaChanging() {
		}
		protected virtual void OnCollectionReloading() {
			if(CollectionReloading != null) {
				CollectionReloading(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCollectionReloaded() {
			if(CollectionReloaded != null) {
				CollectionReloaded(this, EventArgs.Empty);
			}
		}
		protected CollectionSourceBase(IObjectSpace objectSpace, CollectionSourceDataAccessMode dataAccessMode, CollectionSourceMode mode) {
			this.dataAccessMode = dataAccessMode;
			this.mode = mode;
			if((dataAccessMode == CollectionSourceDataAccessMode.Server) || (dataAccessMode == CollectionSourceDataAccessMode.DataView)) {
				this.mode = CollectionSourceMode.Normal;
			}
			if(objectSpace == null) {
				throw new ArgumentNullException("objectSpace");
			}
			SetObjectSpace(objectSpace);
			criteria = new LightDictionary<String, CriteriaOperator>();
			criteria.Changing += new EventHandler(criteria_Changing);
			criteria.Changed += new EventHandler(criteria_Changed);
			sorting = new BindingList<SortProperty>();
			sorting.ListChanged += new ListChangedEventHandler(sorting_ListChanged);
		}
		protected CollectionSourceBase(IObjectSpace objectSpace, Boolean isServerMode, CollectionSourceMode mode)
			: this(objectSpace, isServerMode ? CollectionSourceDataAccessMode.Server : CollectionSourceDataAccessMode.Client, mode) {
		}
		protected CollectionSourceBase(IObjectSpace objectSpace, CollectionSourceMode mode)
			: this(objectSpace, CollectionSourceDataAccessMode.Client, mode) {
		}
		protected CollectionSourceBase(IObjectSpace objectSpace)
			: this(objectSpace, CollectionSourceMode.Normal) {
		}
		public virtual void Dispose() {
			SetObjectSpace(null);
			if(Disposed != null) {
				Disposed(this, EventArgs.Empty);
			}
			CollectionChanging = null;
			CollectionChanged = null;
			CollectionReloading = null;
			CollectionReloaded = null;
			CriteriaApplying = null;
			CriteriaApplied = null;
			collection = null;
			if(proxyCollection != null) {
				proxyCollection.Dispose();
				proxyCollection = null;
			}
			if(originalCollection != null) {
				DisposePreviousCollection(originalCollection);
				originalCollection = null;
			}
			if(sorting != null) {
				sorting.ListChanged -= new ListChangedEventHandler(sorting_ListChanged);
				sorting = null;
			}
			Disposed = null;
		}
		public virtual Boolean? IsObjectFitForCollection(Object obj) {
			return null;
		}
		public virtual Int32 GetCount() {
			Int32 result = 0;
			if(objectSpace.IsCollectionLoaded(originalCollection)) {
				result = ListHelper.GetList(collection).Count;
			}
			else {
				try {
					result = objectSpace.GetObjectsCount(ObjectTypeInfo.Type, GetTotalCriteria());
				}
				catch {
					IList list = ListHelper.GetList(collection);
					if(list != null) {
						result = list.Count;
					}
				}
			}
			return result;
		}
		public virtual void Reload() {
			if(originalCollection != null) {
				OnCollectionReloading();
				objectSpace.ReloadCollection(originalCollection);
				if((mode == CollectionSourceMode.Proxy) && !(originalCollection is IBindingList)) {
					proxyCollection.Refresh();
				}
				OnCollectionReloaded();
			}
		}
		public virtual void Add(Object obj) {
			if((List != null) && !List.IsReadOnly && !List.IsFixedSize) {
				List.Add(obj);
			}
		}
		public virtual void Remove(Object obj) {
			if((List != null) && !List.IsReadOnly && !List.IsFixedSize) {
				List.Remove(obj);
			}
		}
		public void BeginUpdateCriteria() {
			applyCriteriaLockCount++;
		}
		public void EndUpdateCriteria() {
			if(IsCriteriaChanging()) {
				applyCriteriaLockCount--;
			}
			OnCriteriaChanged();
		}
		public void ResetCollection() {
			if(!isCollectionResetting) {
				isCollectionResetting = true;
				try {
					OnCollectionChanging();
					Object previousCollection = collection;
					Object previousOriginalCollection = originalCollection;
					ProxyCollection previousProxyCollection = proxyCollection;
					collection = null;
					proxyCollection = null;
					originalCollection = CreateCollection();
					if(originalCollection != null) {
						if(mode == CollectionSourceMode.Proxy) {
							if((originalCollection == previousOriginalCollection) && (previousProxyCollection != null)
								&& (previousProxyCollection.OriginalCollection == previousOriginalCollection)) {
								proxyCollection = previousProxyCollection;
								collection = proxyCollection;
								if(!(originalCollection is IBindingList)) {
									proxyCollection.Refresh();
								}
							}
							else {
								proxyCollection = new ProxyCollection(objectSpace, ObjectTypeInfo, originalCollection);
								collection = proxyCollection;
							}
						}
						else if(mode == CollectionSourceMode.Normal) {
							collection = originalCollection;
						}
					}
					if(collection != previousCollection) {
						defaultDisplayableProperties = objectSpace.GetDisplayableProperties(collection);
					}
					ApplyTopReturnedObjects();
					ApplyDeleteObjectOnRemove();
					if(!String.IsNullOrEmpty(displayableProperties)) {
						SetDisplayablePropertiesToCollection();
					}
					ApplySorting();
					ApplyCriteria();
					if(previousCollection != collection) {
						OnCollectionChanged();
					}
					if((previousProxyCollection != proxyCollection) && (previousProxyCollection != null)) {
						previousProxyCollection.Dispose();
					}
					DisposePreviousCollection(previousOriginalCollection);
				}
				finally {
					isCollectionResetting = false;
				}
			}
		}
		public void SetCriteria(string criteriaId, string criteria) {
			Guard.ArgumentNotNullOrEmpty(criteriaId, "criteriaId");
			CriteriaOperator criteriaOperator = null;
			using(ObjectSpace.CreateParseCriteriaScope()) {
				criteriaOperator = CriteriaWrapper.ParseCriteriaWithReadOnlyParameters(criteria, ObjectTypeInfo.Type);
			}
			Criteria[criteriaId] = criteriaOperator;
		}
		public void SetAllowAdd(Boolean allowAdd) {
			this.allowAdd = allowAdd;
		}
		public void SetAllowRemove(Boolean allowRemove) {
			this.allowRemove = allowRemove;
		}
		public virtual Boolean CanApplyCriteria {
			get { return true; }
		}
		public Boolean CanApplySorting {
			get { return canApplySorting; }
			set
			{
				if(canApplySorting != value) {
					canApplySorting = value;
					ApplySorting();
				}
			}
		}
		public abstract ITypeInfo ObjectTypeInfo { get; }
		public CollectionSourceMode Mode {
			get { return mode; }
		}
		public Boolean IsLoaded {
			get { return (collection != null); }
		}
		public Object Collection {
			get {
				InitCollection();
				return collection;
			}
		}
		public IList List {
			get { return ListHelper.GetList(Collection); }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		public LightDictionary<String, CriteriaOperator> Criteria {
			get { return criteria; }
		}
		public IList<SortProperty> Sorting {
			get { return sorting; }
			set {
				if(value == null) {
					sorting.Clear();
				}
				else {
					isSortingChanging = true;
					try {
						sorting.Clear();
						foreach(SortProperty sortProperty in value) {
							sorting.Add(sortProperty);
						}
					}
					finally {
						isSortingChanging = false;
						ApplySorting();
					}
				}
			}
		}
		public CollectionSourceDataAccessMode DataAccessMode {
			get { return dataAccessMode; }
		}
		public Boolean IsServerMode {
			get { return dataAccessMode == CollectionSourceDataAccessMode.Server; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Boolean IsCriteriaLocked {
			get { return isCriteriaLocked; }
		}
		public Boolean AllowAdd {
			get {
				String diagnosticInfo;
				return GetAllowAdd(out diagnosticInfo);
			}
		}
		public Boolean AllowRemove {
			get {
				String diagnosticInfo;
				return GetAllowRemove(out diagnosticInfo);
			}
		}
		public String DisplayableProperties {
			get { return GetDisplayableProperties(); }
			set {
				displayableProperties = value;
				if(!String.IsNullOrEmpty(displayableProperties)) {
					SetDisplayablePropertiesToCollection();
				}
			}
		}
		public Int32 TopReturnedObjects {
			get { return topReturnedObjects; }
			set { SetTopReturnedObjects(value); }
		}
		public Boolean DeleteObjectOnRemove {
			get { return deleteObjectOnRemove; }
			set {
				deleteObjectOnRemove = value;
				ApplyDeleteObjectOnRemove();
			}
		}
		public Boolean IsCollectionResetting {
			get { return isCollectionResetting; }
		}
		public event EventHandler CollectionChanging;
		public event EventHandler CollectionChanged;
		public event EventHandler CollectionReloading;
		public event EventHandler CollectionReloaded;
		public event EventHandler CriteriaApplying;
		public event EventHandler CriteriaApplied;
		public event EventHandler Disposed;
		protected static CriteriaOperator CombineCriteria(params CriteriaOperator[] criteriaOperators) {
			CriteriaOperator totalCriteria = null;
			List<CriteriaOperator> operators = new List<CriteriaOperator>();
			foreach(CriteriaOperator op in criteriaOperators) {
				if(!ReferenceEquals(null, op)) {
					if(op.Equals(CollectionSourceBase.EmptyCollectionCriteria)) {
						return CollectionSourceBase.EmptyCollectionCriteria;
					}
					operators.Add(op);
				}
			}
			if(operators.Count == 1) {
				totalCriteria = operators[0];
			}
			else if(operators.Count > 0) {
				totalCriteria = new GroupOperator(GroupOperatorType.And, operators);
			}
			return totalCriteria;
		}
	}
}
