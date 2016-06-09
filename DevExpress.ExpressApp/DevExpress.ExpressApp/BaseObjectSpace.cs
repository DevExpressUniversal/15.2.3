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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.Serialization;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp {
	public class BaseObjectSpace : IObjectSpace, IDisposable {
		protected internal ITypesInfo typesInfo;
		protected IEntityStore entityStore;
		protected Boolean isCommitting;
		private Boolean isModified;
		private Boolean isDeleting;
		protected Boolean isDisposed;
		private Boolean lockingCheckEnabled;
		protected Boolean nonPersistentChangesEnabled;
		protected Boolean isReloading = false;
		protected internal Object owner;
		protected void Object_PropertyChanged(Object sender, PropertyChangedEventArgs e) {
			IMemberInfo memberInfo = typesInfo.FindTypeInfo(GetOriginalType(sender.GetType())).FindMember(e.PropertyName);
			OnObjectChanged(new ObjectChangedEventArgs(sender, memberInfo, null, null));
		}
		protected virtual void OnCommitted() {
			if(Committed != null) {
				Committed(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCommitting(CancelEventArgs args) {
			if(Committing != null) {
				Committing(this, args);
			}
		}
		protected ConfirmationResult RaiseConfirmation(ConfirmationType confirmationType, ConfirmationResult confirmationResult) {
			ConfirmationResult result = confirmationResult;
			if(ConfirmationRequired != null) {
				ConfirmationEventArgs eventArgs = new ConfirmationEventArgs(confirmationType, confirmationResult);
				ConfirmationRequired(this, eventArgs);
				result = eventArgs.ConfirmationResult;
			}
			Tracing.Tracer.LogVerboseText("The '{0}' confirmation was acquired with the '{1}' result", confirmationType, result);
			return result;
		}
		protected virtual void OnConnected() {
			if(Connected != null) {
				Connected(this, EventArgs.Empty);
			}
		}
		protected virtual void OnCustomCommitChanges(HandledEventArgs args) {
			if(CustomCommitChanges != null) {
				CustomCommitChanges(this, args);
			}
		}
		protected virtual void OnCustomDeleteObjects(CustomDeleteObjectsEventArgs args) {
			if(CustomDeleteObjects != null) {
				CustomDeleteObjects(this, args);
			}
		}
		protected virtual void OnCustomRefresh(CompletedEventArgs args) {
			if(CustomRefresh != null) {
				CustomRefresh(this, args);
			}
		}
		protected virtual void OnCustomRollBack(CompletedEventArgs args) {
			if(CustomRollBack != null) {
				CustomRollBack(this, args);
			}
		}
		protected virtual void OnModifiedChanged() {
			if(ModifiedChanged != null) {
				ModifiedChanged(this, EventArgs.Empty);
			}
		}
		protected virtual void OnObjectChanged(ObjectChangedEventArgs args) {
			if(ObjectChanged != null) {
				ObjectChanged(this, args);
			}
		}
		protected virtual void OnObjectDeleted(IList objects) {
			if(ObjectDeleted != null) {
				ObjectDeleted(this, new ObjectsManipulatingEventArgs(objects));
			}
		}
		protected virtual void OnObjectDeleting(IList objects) {
			if(ObjectDeleting != null) {
				ObjectDeleting(this, new ObjectsManipulatingEventArgs(objects));
			}
		}
		protected virtual void OnObjectEndEdit(Object obj) {
			if(ObjectEndEdit != null) {
				ObjectEndEdit(this, new ObjectManipulatingEventArgs(obj));
			}
		}
		protected virtual void OnObjectReloaded(Object obj) {
			if(ObjectReloaded != null) {
				ObjectReloaded(this, new ObjectManipulatingEventArgs(obj));
			}
		}
		protected virtual void OnObjectSaved(Object obj) {
			if(ObjectSaved != null) {
				ObjectSaved(this, new ObjectManipulatingEventArgs(obj));
			}
		}
		protected virtual void OnObjectSaving(Object obj) {
			if(ObjectSaving != null) {
				ObjectSaving(this, new ObjectManipulatingEventArgs(obj));
			}
		}
		protected virtual void OnRefreshing(CancelEventArgs args) {
			if(Refreshing != null) {
				Refreshing(this, args);
			}
		}
		protected virtual void OnReloaded() {
			if(Reloaded != null) {
				Reloaded(this, EventArgs.Empty);
			}
		}
		protected virtual void OnRollingBack(CancelEventArgs args) {
			if(RollingBack != null) {
				RollingBack(this, args);
			}
		}
		protected virtual void CheckLocking() {
		}
		protected virtual IList CreateCollection(Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction) {
			return null;
		}
		protected virtual IList<T> CreateCollection<T>(CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction) {
			return null;
		}
		protected virtual IList CreateDataViewCore(Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting) {
			throw new NotSupportedException();
		}
		protected virtual Object CreateObjectCore(Type type) {
			return null;
		}
		protected virtual void DoCommit() {
		}
		protected virtual void DeleteCore(IList objects) {
		}
		protected virtual CriteriaOperator GetAssociatedCollectionCriteriaCore(Object obj, IMemberInfo memberInfo) {
			if(memberInfo.IsList && (memberInfo.ListElementType != null) && (memberInfo.AssociatedMemberInfo != null)) {
				if(memberInfo.IsManyToMany) {
					return new ContainsOperator(memberInfo.AssociatedMemberInfo.Name, new BinaryOperator(memberInfo.Owner.KeyMember.Name, GetKeyValue(obj)));
				}
				else {
					return new BinaryOperator(memberInfo.AssociatedMemberInfo.Name + "." + memberInfo.Owner.KeyMember.Name, GetKeyValue(obj));
				}
			}
			else {
				return null;
			}
		}
		protected virtual ICollection<ICustomFunctionOperator> GetCustomFunctionOperators() {
			return null;
		}
		protected virtual void SetModified(Object obj, ObjectChangedEventArgs args) {
			SetIsModified(true);
		}
		protected virtual void Reload() {
		}
		protected Type GetOriginalType(Type type) {
			Type result = type;
			if((entityStore != null) && (type != null)) {
				result = entityStore.GetOriginalType(type);
			}
			return result;
		}
		protected void CheckIsDisposed() {
			if(isDisposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}
		}
		protected void SetIsModified(Boolean isModified) {
			if(this.isModified != isModified) {
				this.isModified = isModified;
				OnModifiedChanged();
			}
		}
		protected Boolean IsMemberNullOrPersistent(Object obj, ObjectChangedEventArgs args) {
			IMemberInfo memberInfo = args.MemberInfo;
			if((memberInfo == null) && (obj != null)) {
				memberInfo = typesInfo.FindTypeInfo(obj.GetType()).FindMember(args.PropertyName);
			}
			return (memberInfo == null) || memberInfo.IsPersistent;
		}
		public BaseObjectSpace(ITypesInfo typesInfo, IEntityStore entityStore) {
			this.typesInfo = typesInfo;
			this.entityStore = entityStore;
			nonPersistentChangesEnabled = false;
		}
		public virtual void Dispose() {
			if(Disposed != null) {
				Disposed(this, EventArgs.Empty);
			}
			Committed = null;
			Committing = null;
			ConfirmationRequired = null;
			Connected = null;
			CustomCommitChanges = null;
			CustomDeleteObjects = null;
			CustomRefresh = null;
			CustomRollBack = null;
			Disposed = null;
			ModifiedChanged = null;
			ObjectChanged = null;
			ObjectDeleted = null;
			ObjectDeleting = null;
			ObjectEndEdit = null;
			ObjectReloaded = null;
			ObjectSaved = null;
			ObjectSaving = null;
			Refreshing = null;
			Reloaded = null;
			RollingBack = null;
		}
		public virtual String Database {
			get { return ""; }
		}
		public virtual Boolean IsConnected {
			get { return true; }
		}
		public virtual IList ModifiedObjects {
			get { return new List<Object>(); }
		}
		public virtual void ApplyCriteria(Object collection, CriteriaOperator criteria) {
		}
		public virtual void ApplyFilter(Object collection, CriteriaOperator filter) {
		}
		public virtual Boolean CanApplyCriteria(Type collectionType) {
			return false;
		}
		public virtual Boolean CanApplyFilter(Object collection) {
			return false;
		}
		public virtual Boolean CanInstantiate(Type type) {
			Boolean result = false;
			if(entityStore != null) {
				Type originalType = GetOriginalType(type);
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(originalType);
				result =
					(typeInfo != null)
					&& entityStore.RegisteredEntities.Contains(originalType)
					&& !typeInfo.IsAbstract
					&& TypeHelper.CanCreateInstance(originalType);
			}
			return result;
		}
		public void CommitChanges() {
			CheckIsDisposed();
			CancelEventArgs args = new CancelEventArgs(false);
			OnCommitting(args);
			if(!args.Cancel) {
				if(lockingCheckEnabled) {
					CheckLocking();
				}
				HandledEventArgs customCommitArgs = new HandledEventArgs(false);
				OnCustomCommitChanges(customCommitArgs);
				if(!customCommitArgs.Handled) {
					DoCommit();
				}
				SetIsModified(false);
				OnCommitted();
			}
		}
		public virtual Boolean Contains(Object obj) {
			return false;
		}
		public virtual IList CreateCollection(Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting) {
			return CreateCollection(objectType, criteria, sorting, false);
		}
		public IList CreateCollection(Type objectType, CriteriaOperator criteria) {
			return CreateCollection(objectType, criteria, null);
		}
		public IList CreateCollection(Type objectType) {
			return CreateCollection(objectType, null, null);
		}
		public IList CreateDataView(Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting) {
			return CreateDataViewCore(objectType, expressions, criteria, sorting);
		}
		public IList CreateDataView(Type objectType, String expressions, CriteriaOperator criteria, IList<SortProperty> sorting) {
			return CreateDataView(objectType, ConvertExpressionsStringToExpressionsList(expressions), criteria, sorting);
		}
		public virtual IObjectSpace CreateNestedObjectSpace() {
			return null;
		}
		public virtual Object CreateObject(Type type) {
			Guard.ArgumentNotNull(type, "type");
			CheckIsDisposed();
			Object obj = CreateObjectCore(type);
			if(obj is IObjectSpaceLink) {
				((IObjectSpaceLink)obj).ObjectSpace = this;
			}
			if(obj is IXafEntityObject) {
				((IXafEntityObject)obj).OnCreated();
			}
			SetModified(obj);
			return obj;
		}
		public ObjectType CreateObject<ObjectType>() {
			return (ObjectType)CreateObject(typeof(ObjectType));
		}
		public virtual IDisposable CreateParseCriteriaScope() {
			return null;
		}
		public virtual Object CreateServerCollection(Type objectType, CriteriaOperator criteria) {
			return null;
		}
		public virtual void Delete(IList objects) {
			CheckIsDisposed();
			CustomDeleteObjectsEventArgs customDeleteArgs = new CustomDeleteObjectsEventArgs(objects);
			OnCustomDeleteObjects(customDeleteArgs);
			if(!customDeleteArgs.Handled) {
				IList typedObjects = new List<Object>();
				foreach(Object obj in objects) {
					if(obj is XafDataViewRecord) {
						typedObjects.Add(GetObject(obj));
					}
					else {
						typedObjects.Add(obj);
					}
				}
				isDeleting = true;
				try {
					DeleteCore(typedObjects);
				}
				finally {
					isDeleting = false;
				}
				SetIsModified(true);
			}
		}
		public void Delete(Object obj) {
			if(obj is IList) {
				Delete((IList)obj);
			}
			else {
				Delete(new Object[] { obj });
			}
		}
		public virtual void EnableObjectDeletionOnRemove(Object collection, Boolean enable) {
		}
		public virtual Object Evaluate(Type objectType, CriteriaOperator expression, CriteriaOperator criteria) {
			return null;
		}
		public virtual Object FindObject(Type objectType, CriteriaOperator criteria, Boolean inTransaction) {
			return null;
		}
		public Object FindObject(Type objectType, CriteriaOperator criteria) {
			return FindObject(objectType, criteria, true);
		}
		public ObjectType FindObject<ObjectType>(CriteriaOperator criteria, Boolean inTransaction) {
			return (ObjectType)FindObject(typeof(ObjectType), criteria, inTransaction);
		}
		public ObjectType FindObject<ObjectType>(CriteriaOperator criteria) {
			return FindObject<ObjectType>(criteria, true);
		}
		public CriteriaOperator GetAssociatedCollectionCriteria(Object obj, IMemberInfo memberInfo) {
			Object lastObject = memberInfo.GetOwnerInstance(obj);
			if(lastObject != null) {
				return GetAssociatedCollectionCriteriaCore(lastObject, memberInfo.LastMember);
			}
			else {
				return null;
			}
		}
		public virtual IList<SortProperty> GetCollectionSorting(Object collection) {
			return null;
		}
		public virtual CriteriaOperator GetCriteria(Object collection) {
			return null;
		}
		public virtual String GetDisplayableProperties(Object collection) {
			String result = "";
			if(collection is ProxyCollection) {
				result = ((ProxyCollection)collection).DisplayableMembers;
			}
			return result;
		}
		public virtual EvaluatorContextDescriptor GetEvaluatorContextDescriptor(Type type) {
			return new EvaluatorContextDescriptorDefault(type);
		}
		public ExpressionEvaluator GetExpressionEvaluator(Type type, CriteriaOperator criteria) {
			EvaluatorContextDescriptor evaluatorContextDescriptor = GetEvaluatorContextDescriptor(type);
			return GetExpressionEvaluator(evaluatorContextDescriptor, criteria);
		}
		public ExpressionEvaluator GetExpressionEvaluator(EvaluatorContextDescriptor evaluatorContextDescriptor, CriteriaOperator criteria) {
			return new ExpressionEvaluator(evaluatorContextDescriptor, criteria, false, GetCustomFunctionOperators());
		}
		public virtual CriteriaOperator GetFilter(Object collection) {
			return null;
		}
		public virtual String GetKeyPropertyName(Type type) {
			return "";
		}
		public virtual Type GetKeyPropertyType(Type type) {
			return null;
		}
		public virtual Object GetKeyValue(Object obj) {
			return GetKeyValue(typesInfo, obj);
		}
		public virtual String GetKeyValueAsString(Object obj) {
			return null;
		}
		public virtual Object GetObject(Object obj) {
			return null;
		}
		public ObjectType GetObject<ObjectType>(ObjectType obj) {
			return (ObjectType)GetObject((Object)obj);
		}
		public Object GetObjectByHandle(String handle) {
			CheckIsDisposed();
			Type objectType;
			String keyString;
			ObjectHandleHelper.ParseObjectHandle(TypesInfo, handle, out objectType, out keyString);
			return GetObjectByKey(objectType, GetObjectKey(objectType, keyString));
		}
		public virtual Object GetObjectByKey(Type type, Object key) {
			return null;
		}
		public ObjectType GetObjectByKey<ObjectType>(Object key) {
			return (ObjectType)GetObjectByKey(typeof(ObjectType), key);
		}
		public String GetObjectHandle(Object obj) {
			CheckIsDisposed();
			Guard.ArgumentNotNull(obj, "obj");
			return ObjectHandleHelper.CreateObjectHandle(TypesInfo, GetObjectType(obj), GetKeyValueAsString(obj));
		}
		public virtual Object GetObjectKey(Type objectType, String objectKeyString) {
			return null;
		}
		public IList GetObjects(Type type, CriteriaOperator criteria, Boolean inTransaction) {
			return CreateCollection(type, criteria, null, inTransaction);
		}
		public IList GetObjects(Type type, CriteriaOperator criteria) {
			return GetObjects(type, criteria, false);
		}
		public IList GetObjects(Type type) {
			return GetObjects(type, null);
		}
		public IList<T> GetObjects<T>(CriteriaOperator criteria, Boolean inTransaction) {
			return CreateCollection<T>(criteria, null, inTransaction);
		}
		public IList<T> GetObjects<T>(CriteriaOperator criteria) {
			return GetObjects<T>(criteria, false);
		}
		public IList<T> GetObjects<T>() {
			return GetObjects<T>(null);
		}
		public virtual Int32 GetObjectsCount(Type objectType, CriteriaOperator criteria) {
			Int32 result = 0;
			Object evaluationResult = Evaluate(objectType, new AggregateOperand("", Aggregate.Count), criteria);
			if(evaluationResult != null) {
				result = (Int32)evaluationResult;
			}
			return result;
		}
		public CriteriaOperator GetObjectsCriteria(ITypeInfo objectTypeInfo, IList objects) {
			CriteriaOperator objectsCriteria = null;
			if(objectTypeInfo.KeyMembers.Count == 1) {
				ArrayList keys = new ArrayList();
				foreach(Object obj in objects) {
					keys.Add(GetKeyValue(obj));
				}
				objectsCriteria = new InOperator(objectTypeInfo.KeyMembers[0].Name, keys);
			}
			else {
				objectsCriteria = new GroupOperator(GroupOperatorType.Or);
				foreach(Object obj in objects) {
					GroupOperator objectCriteria = new GroupOperator(GroupOperatorType.And);
					foreach(IMemberInfo keyMemberInfo in objectTypeInfo.KeyMembers) {
						objectCriteria.Operands.Add(new BinaryOperator(keyMemberInfo.Name, keyMemberInfo.GetValue(obj)));
					}
					((GroupOperator)objectsCriteria).Operands.Add(objectCriteria);
				}
			}
			return objectsCriteria;
		}
		public virtual IQueryable<T> GetObjectsQuery<T>(Boolean inTransaction = false) {
			return null;
		}
		public virtual ICollection GetObjectsToDelete(Boolean includeParent) {
			return new List<Object>();
		}
		public virtual ICollection GetObjectsToSave(Boolean includeParent) {
			return new List<Object>();
		}
		public Type GetObjectType(Object obj) {
			Type result = null;
			if(obj is XafDataViewRecord) {
				result = ((XafDataViewRecord)obj).ObjectType;
			}
			else if(obj != null) {
				result = GetOriginalType(obj.GetType());
			}
			return result;
		}
		public virtual Int32 GetTopReturnedObjectsCount(Object collection) {
			return 0;
		}
		public virtual Boolean IsCollectionLoaded(Object collection) {
			return false;
		}
		public virtual Boolean IsDeletedObject(Object obj) {
			return false;
		}
		public virtual Boolean IsDeletionDeferredType(Type type) {
			return false;
		}
		public virtual Boolean IsDisposedObject(Object obj) {
			return false;
		}
		public Boolean IsKnownType(Type type) {
			Boolean result = false;
			if(entityStore != null) {
				Type originalType = GetOriginalType(type);
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(originalType);
				result = (typeInfo != null) && entityStore.RegisteredEntities.Contains(originalType);
			}
			return result;
		}
		public virtual Boolean IsNewObject(Object obj) {
			return false;
		}
		public virtual Boolean IsObjectDeletionOnRemoveEnabled(Object collection) {
			return false;
		}
		public Boolean? IsObjectFitForCriteria(Object obj, CriteriaOperator criteria) {
			if(obj != null) {
				try {
					return GetExpressionEvaluator(obj.GetType(), criteria).Fit(obj);
				}
				catch(Exception e) {
					Tracing.Tracer.LogError(e);
					return null;
				}
			}
			return false;
		}
		public virtual Boolean? IsObjectFitForCriteria(Type objectType, Object obj, CriteriaOperator criteria) {
			if((obj != null) && objectType.IsAssignableFrom(obj.GetType())) {
				return IsObjectFitForCriteria(obj, criteria);
			}
			return false;
		}
		public virtual Boolean IsObjectToDelete(Object obj) {
			return false;
		}
		public virtual Boolean IsObjectToSave(Object obj) {
			return false;
		}
		public virtual CriteriaOperator ParseCriteria(String criteria) {
			return CriteriaOperator.TryParse(criteria);
		}
		public Boolean Refresh() {
			CheckIsDisposed();
			Boolean result = false;
			CancelEventArgs args = new CancelEventArgs(false);
			OnRefreshing(args);
			if(!args.Cancel) {
				CompletedEventArgs customRefreshArgs = new CompletedEventArgs();
				OnCustomRefresh(customRefreshArgs);
				if(!customRefreshArgs.Handled) {
					if(IsModified) {
						ConfirmationResult confirmationResult = RaiseConfirmation(ConfirmationType.NeedSaveChanges, ConfirmationResult.No);
						if(confirmationResult == ConfirmationResult.Yes) {
							CommitChanges();
							Reload();
							result = true;
						}
						else if(confirmationResult == ConfirmationResult.No) {
							Reload();
							result = true;
						}
						else if(confirmationResult == ConfirmationResult.Cancel) {
							result = false;
						}
					}
					else {
						Reload();
						result = true;
					}
				}
				else {
					result = customRefreshArgs.IsCompleted;
				}
			}
			return result;
		}
		public virtual void ReloadCollection(Object collection) {
		}
		public virtual Object ReloadObject(Object obj) {
			return obj;
		}
		public virtual void RemoveFromModifiedObjects(Object obj) {
		}
		public Boolean Rollback() {
			CheckIsDisposed();
			Boolean result = false;
			CancelEventArgs args = new CancelEventArgs(false);
			OnRollingBack(args);
			if(!args.Cancel) {
				CompletedEventArgs customRollbackArgs = new CompletedEventArgs();
				OnCustomRollBack(customRollbackArgs);
				if(!customRollbackArgs.Handled) {
					if(IsModified) {
						ConfirmationResult confirmationResult = RaiseConfirmation(ConfirmationType.CancelChanges, ConfirmationResult.Yes);
						if(confirmationResult == ConfirmationResult.Yes) {
							Reload();
							result = true;
						}
						else if(confirmationResult == ConfirmationResult.No) {
							result = false;
						}
					}
					else {
						Reload();
						result = true;
					}
				}
				else {
					result = customRollbackArgs.IsCompleted;
				}
			}
			return result;
		}
		public virtual void SetCollectionSorting(Object collection, IList<SortProperty> sorting) {
		}
		public virtual void SetDisplayableProperties(Object collection, String displayableProperties) {
			if(collection is ProxyCollection) {
				((ProxyCollection)collection).DisplayableMembers = displayableProperties;
			}
		}
		public void SetModified(Object obj, IMemberInfo memberInfo) {
			SetModified(obj, new ObjectChangedEventArgs(obj, memberInfo, null, null));
		}
		public void SetModified(Object obj) {
			SetModified(obj, (IMemberInfo)null);
		}
		public virtual void SetTopReturnedObjectsCount(Object collection, Int32 topReturnedObjectsCount) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Boolean IsIntermediateObject(Object obj) {
			return false;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void GetIntermediateObjectReferences(Object obj, out Object left, out Object right) {
			left = null;
			right = null;
		}
		public Boolean IsCommitting {
			get { return isCommitting; }
		}
		public Boolean IsDeleting {
			get { return isDeleting; }
		}
		public Boolean IsModified {
			get { return isModified; }
		}
		public Boolean IsDisposed {
			get { return isDisposed; }
		}
		public Boolean IsReloading {
			get { return isReloading; }
		}
		public Boolean LockingCheckEnabled {
			get { return lockingCheckEnabled; }
			set { lockingCheckEnabled = value; }
		}
		public Boolean NonPersistentChangesEnabled {
			get { return nonPersistentChangesEnabled; }
			set { nonPersistentChangesEnabled = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual Boolean CanFilterByNonPersistentMembers {
			get { return true; }
		}
		public Object Owner {
			get { return owner; }
			set { owner = value; }
		}
		public ITypesInfo TypesInfo {
			get { return typesInfo; }
		}
		public virtual IDbConnection Connection {
			get { return null; }
		}
		public event EventHandler Committed;
		public event EventHandler<CancelEventArgs> Committing;
		public event EventHandler<ConfirmationEventArgs> ConfirmationRequired;
		public event EventHandler Connected;
		public event EventHandler<HandledEventArgs> CustomCommitChanges;
		public event EventHandler<CustomDeleteObjectsEventArgs> CustomDeleteObjects;
		public event EventHandler<HandledEventArgs> CustomRefresh;
		public event EventHandler<HandledEventArgs> CustomRollBack;
		public event EventHandler Disposed;
		public event EventHandler ModifiedChanged;
		public event EventHandler<ObjectChangedEventArgs> ObjectChanged;
		public event EventHandler<ObjectsManipulatingEventArgs> ObjectDeleted;
		public event EventHandler<ObjectsManipulatingEventArgs> ObjectDeleting;
		public event EventHandler<ObjectManipulatingEventArgs> ObjectEndEdit;
		public event EventHandler<ObjectManipulatingEventArgs> ObjectReloaded;
		public event EventHandler<ObjectManipulatingEventArgs> ObjectSaved;
		public event EventHandler<ObjectManipulatingEventArgs> ObjectSaving;
		public event EventHandler<CancelEventArgs> Refreshing;
		public event EventHandler Reloaded;
		public event EventHandler<CancelEventArgs> RollingBack;
		private static Object GetMemberValue(Object obj, IMemberInfo memberInfo) {
			Object result = null;
			if(obj is XafDataViewRecord) {
				if(((XafDataViewRecord)obj).ContainsMember(memberInfo.Name)) {
					result = ((XafDataViewRecord)obj)[memberInfo.Name];
				}
			}
			else {
				result = memberInfo.GetValue(obj);
			}
			return result;
		}
		public static String ConvertSortingToString(IList<SortProperty> sorting) {
			List<String> sortingStringItems = new List<String>();
			if(sorting != null) {
				foreach(SortProperty sortProperty in sorting) {
					String sortingStringItem = sortProperty.PropertyName;
					if(sortProperty.Direction == SortingDirection.Descending) {
						sortingStringItem += " DESC";
					}
					sortingStringItems.Add(sortingStringItem);
				}
			}
			return String.Join("; ", sortingStringItems.ToArray());
		}
		public static IList<SortProperty> ConvertStringToSorting(String sortingString) {
			List<SortProperty> result = new List<SortProperty>();
			String[] sortingStringItems = sortingString.Split(new String[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);
			foreach(String sortingStringItemRaw in sortingStringItems) {
				String sortingStringItem = sortingStringItemRaw.Trim();
				String propertyName = "";
				SortingDirection direction = SortingDirection.Ascending;
				if(sortingStringItem.ToLower().EndsWith(" desc")) {
					direction = SortingDirection.Descending;
					propertyName = sortingStringItem.Remove(sortingStringItem.Length - 5);
				}
				else {
					propertyName = sortingStringItem;
				}
				SortProperty sortProperty = null;
				if(propertyName.StartsWith("[")) {
					sortProperty = new SortProperty(propertyName, direction);
				}
				else {
					sortProperty = new SortProperty("[" + propertyName + "]", direction);
				}
				result.Add(sortProperty);
			}
			return result;
		}
		public static IList<DataViewExpression> ConvertExpressionsStringToExpressionsList(String expressions) {
			List<DataViewExpression> result = new List<DataViewExpression>();
			foreach(String expression in expressions.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries)) {
				result.Add(new DataViewExpression(expression, expression));
			}
			return result;
		}
		public static Boolean ObjectKeyValuesEqual(ITypeInfo objectTypeInfo, Object objectKeyValue1, Object objectKeyValue2) {
			Boolean result = true;
			if(objectTypeInfo.KeyMembers.Count > 1) {
				IList objectKeyValues1 = objectKeyValue1 as IList;
				IList objectKeyValues2 = objectKeyValue2 as IList;
				if((objectKeyValues1 != null) && (objectKeyValues2 != null) && (objectKeyValues1.Count == objectKeyValues2.Count)) {
					for(Int32 i = 0; i < objectKeyValues1.Count; i++) {
						Object valueA = objectKeyValues1[i];
						Object valueB = objectKeyValues2[i];
						Boolean isEqual =
							(valueA == valueB)
							||
							(valueA != null) && valueA.Equals(valueB);
						if(!isEqual) {
							result = false;
							break;
						}
					}
				}
				else {
					result = false;
				}
			}
			else {
				Boolean isEqual = (objectKeyValue1 != null) && objectKeyValue1.Equals(objectKeyValue2);
				if(!isEqual) {
					result = false;
				}
			}
			return result;
		}
		public static Object GetKeyValue(ITypesInfo typesInfo, Object obj) {
			if(obj == null) {
				throw new ArgumentNullException();
			}
			Object result = null;
			Type objectType = null;
			if(obj is XafDataViewRecord) {
				objectType = ((XafDataViewRecord)obj).ObjectType;
			}
			else if(obj != null) {
				objectType = obj.GetType();
			}
			TypeInfo objectTypeInfo = (TypeInfo)typesInfo.FindTypeInfo(objectType);
			if(objectTypeInfo != null) {
				if(objectTypeInfo.KeyMembers.Count > 1) {
					List<Object> keyMemberValues = new List<Object>();
					foreach(IMemberInfo keyMemberInfo in objectTypeInfo.KeyMembers) {
						keyMemberValues.Add(GetMemberValue(obj, keyMemberInfo));
					}
					result = keyMemberValues;
				}
				else {
					IMemberInfo keyMemberInfo = objectTypeInfo.KeyMember;
					if(keyMemberInfo != null) {
						result = GetMemberValue(obj, keyMemberInfo);
					}
				}
			}
			return result;
		}
	}
	[Serializable]
	public class CannotApplyCriteriaException : InvalidOperationException {
		private static String CannotApplyCriteriaExceptionMessage = "Cannot apply criteria to the collection of the '{0}' type.";
		protected CannotApplyCriteriaException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public CannotApplyCriteriaException(Type listType) : base(String.Format(CannotApplyCriteriaExceptionMessage, listType.FullName)) { }
	}
	public interface IXafEntityObject {
		void OnCreated();
		void OnSaving();
		void OnLoaded();
	}
}
