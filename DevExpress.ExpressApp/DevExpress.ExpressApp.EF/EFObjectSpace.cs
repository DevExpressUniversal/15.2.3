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
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects.DataClasses;
using DevExpress.Xpo;
using DevExpress.Data;
using DevExpress.Data.Linq;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo.DB.Exceptions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Filtering;
using DevExpress.ExpressApp.EF.Utils;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp.EF {
	public class EFObjectSpace : BaseObjectSpace {
		public const Int32 UnableToOpenDatabaseErrorNumber = 4060;
		public static readonly Type CompositeKeyPropertyType = typeof(IList);
		private EFTypeInfoSource efTypeInfoSource;
		private ObjectContext objectContext;
		private Boolean isConnected;
		private CreateObjectContextHandler createObjectContextDelegate;
		private UpdateSchemaHandler updateSchemaDelegate;
		private List<IDisposable> disposableObjects;
		private Boolean isObjectReloading;
		private String GetMemberInfoNameExceptLast(IMemberInfo memberInfo) {
			String result = "";
			IList<IMemberInfo> memberInfoPath = memberInfo.GetPath();
			for(Int32 i = 0; i < memberInfoPath.Count - 1; i++) {
				result = result + memberInfoPath[i].Name + ".";
			}
			return result.TrimEnd('.');
		}
		private void DetectChanges() {
			if(!isObjectReloading) {
				objectContext.DetectChanges();
			}
		}
		private IList GetModifiedObjects() {
			HashSet<Object> modifiedObjects = new HashSet<Object>();
			DetectChanges();
			foreach(ObjectStateEntry objectStateEntry in objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified)) {
				if(objectStateEntry.Entity != null) {
					modifiedObjects.Add(objectStateEntry.Entity);
				}
				else if(objectStateEntry.IsRelationship) {
					DbDataRecord relationshipValues = null;
					if(objectStateEntry.State == EntityState.Added) {
						relationshipValues = objectStateEntry.CurrentValues;
					}
					else if(objectStateEntry.State == EntityState.Deleted) {
						relationshipValues = objectStateEntry.OriginalValues;
					}
					if(relationshipValues != null) {
						for(Int32 i = 0; i < relationshipValues.FieldCount; i++) {
							if(relationshipValues[i] is EntityKey) {
								Object obj = null;
								if(objectContext.TryGetObjectByKey((EntityKey)relationshipValues[i], out obj)) {
									modifiedObjects.Add(obj);
								}
							}
						}
					}
				}
			}
			return modifiedObjects.ToList();
		}
		private void Connection_StateChange(Object sender, StateChangeEventArgs e) {
			if((e.CurrentState == ConnectionState.Open) && !isConnected) {
				isConnected = true;
				OnConnected();
			}
		}
		private String GetEntitySetName(Type type) {
			return efTypeInfoSource.GetEntitySetName(type);
		}
		private CriteriaOperator GetActualExpression(ITypeInfo objectTypeInfo, CriteriaOperator expression) {
			CriteriaOperator result = expression;
			if(expression is OperandProperty) {
				IMemberInfo memberInfo = objectTypeInfo.FindMember(((OperandProperty)expression).PropertyName);
				if((memberInfo != null) && !String.IsNullOrWhiteSpace(memberInfo.Expression)) {
					result = CriteriaOperator.Parse(memberInfo.Expression);
					String memberInfoNameExceptLast = GetMemberInfoNameExceptLast(memberInfo);
					if(!String.IsNullOrWhiteSpace(memberInfoNameExceptLast)) {
						PropertyNameExtensionCriteriaProcessor criteriaProcessor = new PropertyNameExtensionCriteriaProcessor(memberInfoNameExceptLast);
						result.Accept(criteriaProcessor);
					}
				}
			}
			return result;
		}
		private Object GetSingleObject(IQueryable queryable) {
			try {
				foreach(Object obj in queryable) {
					return obj;
				}
			}
			catch(Exception exception) {
				SqlException sqlException = GetSqlException(exception);
				if((sqlException != null) && (sqlException.Number == UnableToOpenDatabaseErrorNumber)) {
					throw new UnableToOpenDatabaseException("", exception);
				}
				else {
					throw;
				}
			}
			return null;
		}
		private void ObjectContext_SavingChanges(Object sender, EventArgs e) {
			IList modifiedObjects = GetModifiedObjects();
			foreach(Object obj in modifiedObjects) {
				if(obj is IXafEntityObject) {
					((IXafEntityObject)obj).OnSaving();
				}
			}
		}
		private void ObjectContext_ObjectMaterialized(Object sender, ObjectMaterializedEventArgs e) {
			if(e.Entity is IXafEntityObject) {
				((IXafEntityObject)e.Entity).OnLoaded();
			}
		}
		private void ObjectStateManager_ObjectStateManagerChanged(Object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Add) {
				if(e.Element is INotifyPropertyChanged) {
					((INotifyPropertyChanged)e.Element).PropertyChanged -= Object_PropertyChanged;
					((INotifyPropertyChanged)e.Element).PropertyChanged += Object_PropertyChanged;
				}
				if(e.Element is IObjectSpaceLink) {
					((IObjectSpaceLink)e.Element).ObjectSpace = this;
				}
			}
			else if(e.Action == CollectionChangeAction.Remove) {
				if(e.Element is INotifyPropertyChanged) {
					((INotifyPropertyChanged)e.Element).PropertyChanged -= Object_PropertyChanged;
				}
				if(e.Element is IObjectSpaceLink) {
					((IObjectSpaceLink)e.Element).ObjectSpace = null;
				}
			}
			OnObjectStateManagerChanged(e);
		}
		private ObjectContext CreateObjectContext(IList<IDisposable> disposableObjects) {
			ObjectContext objectContext = createObjectContextDelegate(disposableObjects);
			objectContext.SavingChanges += new EventHandler(ObjectContext_SavingChanges);
			objectContext.ObjectMaterialized += new ObjectMaterializedEventHandler(ObjectContext_ObjectMaterialized);
			objectContext.Connection.StateChange += new StateChangeEventHandler(Connection_StateChange);
			objectContext.ObjectStateManager.ObjectStateManagerChanged += new CollectionChangeEventHandler(ObjectStateManager_ObjectStateManagerChanged);
			return objectContext;
		}
		private void DisposeObjectContext(ObjectContext objectContext, IList<IDisposable> disposableObjects) {
			if(objectContext != null) {
				objectContext.SavingChanges -= new EventHandler(ObjectContext_SavingChanges);
				objectContext.ObjectMaterialized -= new ObjectMaterializedEventHandler(ObjectContext_ObjectMaterialized);
				objectContext.Connection.StateChange -= new StateChangeEventHandler(Connection_StateChange);
				objectContext.ObjectStateManager.ObjectStateManagerChanged -= new CollectionChangeEventHandler(ObjectStateManager_ObjectStateManagerChanged);
				objectContext.Dispose();
			}
			if(disposableObjects != null) {
				foreach(IDisposable disposableObject in disposableObjects) {
					disposableObject.Dispose();
				}
			}
		}
		protected CriteriaOperator ProcessCriteria(Type objectType, CriteriaOperator criteria) {
			CriteriaOperator result = null;
			if(!ReferenceEquals(criteria, null)) {
				result = (CriteriaOperator)((ICloneable)criteria).Clone();
				ObjectMemberValueCriteriaProcessor objectMemberValueCriteriaProcessor = new ObjectMemberValueCriteriaProcessor(typesInfo, objectType);
				objectMemberValueCriteriaProcessor.Process(result);
			}
			return result;
		}
		protected ObjectQuery<T> GetObjectsQueryT<T>(IList<String> memberNames, CriteriaOperator criteria, IList<SortProperty> sorting) {
			ObjectQuery<T> objectQuery = null;
			if(updateSchemaDelegate != null) {
				updateSchemaDelegate();
			}
			CriteriaOperator workCriteria = ProcessCriteria(typeof(T), criteria);
			CriteriaToEFExpressionConverter converter = new CriteriaToEFExpressionConverter();
			objectQuery = objectContext.CreateQuery<T>(GetEntitySetName(typeof(T))).OfType<T>();
			if(memberNames != null) {
				ITypeInfo objectTypeInfo = typesInfo.FindTypeInfo(typeof(T));
				String[] referenceMemberNames = GetReferenceMemberNames(objectTypeInfo, memberNames);
				foreach(String referenceMemberName in referenceMemberNames) {
					IMemberInfo referenceMemberInfo = objectTypeInfo.FindMember(referenceMemberName);
					if(!referenceMemberInfo.IsDelayed) {
						objectQuery = objectQuery.Include(referenceMemberName);
					}
				}
			}
			objectQuery = (ObjectQuery<T>)objectQuery.AppendWhere(converter, workCriteria);
			if(sorting != null) {
				List<ServerModeOrderDescriptor> orderDescriptors = new List<ServerModeOrderDescriptor>();
				foreach(SortProperty sortProperty in sorting) {
					ServerModeOrderDescriptor orderDescriptor = new ServerModeOrderDescriptor(CriteriaOperator.Parse(sortProperty.PropertyName), (sortProperty.Direction == SortingDirection.Descending));
					orderDescriptors.Add(orderDescriptor);
				}
				objectQuery = (ObjectQuery<T>)objectQuery.MakeOrderBy(converter, orderDescriptors.ToArray());
			}
			return objectQuery;
		}
		protected internal ObjectQuery GetObjectsQuery(Type objectType, IList<String> memberNames, CriteriaOperator criteria, IList<SortProperty> sorting) {
			MethodInfo methodInfo = typeof(EFObjectSpace).GetMethod("GetObjectsQueryT", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(objectType);
			return (ObjectQuery)methodInfo.Invoke(this, new Object[] { memberNames, criteria, sorting });
		}
		protected internal ObjectQuery<DbDataRecord> GetDataRecordsQuery(Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting) {
			if(updateSchemaDelegate != null) {
				updateSchemaDelegate();
			}
			CriteriaOperator workCriteria = ProcessCriteria(objectType, criteria);
			CriteriaToEFSqlConverter converter = new CriteriaToEFSqlConverter("it", typesInfo, objectType);
			String selectString = "";
			if(expressions != null) {
				String projection = "";
				ITypeInfo objectTypeInfo = typesInfo.FindTypeInfo(objectType);
				foreach(DataViewExpression expression in expressions) {
					projection = projection + converter.Convert(GetActualExpression(objectTypeInfo, expression.Expression)) + " as [" + expression.Name + "], ";
				}
				projection = projection.TrimEnd(',', ' ');
				selectString = String.Format("select {0} from oftype({1}, {2}) as it", projection, GetEntitySetName(objectType), objectType.FullName);
			}
			String whereString = converter.Convert(workCriteria);
			if(!String.IsNullOrWhiteSpace(whereString)) {
				whereString = "where (" + whereString + ")";
			}
			String orderByString = "";
			if((sorting != null) && (sorting.Count > 0)) {
				foreach(SortProperty sortProperty in sorting) {
					String sortPropertyString = "(" + converter.Convert(sortProperty.Property) + ")";
					orderByString = orderByString + sortPropertyString;
					if(sortProperty.Direction == SortingDirection.Descending) {
						orderByString = orderByString + " desc";
					}
					orderByString = orderByString + ", ";
				}
				orderByString = "order by " + orderByString.TrimEnd(',', ' ');
			}
			return objectContext.CreateQuery<DbDataRecord>(selectString + " " + whereString + " " + orderByString);
		}
		protected Object CreateDetachedObjectT<T>() where T : class {
			return objectContext.CreateObject<T>();
		}
		protected Object CreateDetachedObject(Type type) {
			MethodInfo methodInfo = typeof(EFObjectSpace).GetMethod("CreateDetachedObjectT", BindingFlags.Instance | BindingFlags.NonPublic).MakeGenericMethod(type);
			return methodInfo.Invoke(this, new Object[] { });
		}
		protected override IList CreateCollection(Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction) {
			return new EFCollection(this, GetOriginalType(objectType), criteria, sorting, inTransaction);
		}
		protected override IList<T> CreateCollection<T>(CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction) {
			return new EFCollection<T>(this, criteria, sorting, inTransaction);
		}
		protected override IList CreateDataViewCore(Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting) {
			return new EFDataView(this, objectType, expressions, criteria, sorting);
		}
		protected override void DoCommit() {
			isCommitting = true;
			try {
				IList modifiedObjects = GetModifiedObjects();
				foreach(Object obj in modifiedObjects) {
					OnObjectSaving(obj);
				}
				objectContext.SaveChanges();
				foreach(Object obj in modifiedObjects) {
					OnObjectSaved(obj);
				}
			}
			catch(OptimisticConcurrencyException exception) {
				throw new UserFriendlyException(UserVisibleExceptionId.ObjectToSaveWasChanged, exception);
			}
			finally {
				isCommitting = false;
			}
		}
		protected override Object CreateObjectCore(Type type) {
			Object obj = CreateDetachedObject(type);
			objectContext.AddObject(GetEntitySetName(type), obj);
			ObjectStateEntry objectStateEntry;
			if(objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry)
					&& (objectStateEntry != null) && (objectStateEntry.State == EntityState.Unchanged)) {
				objectStateEntry.SetModified();
			}
			return obj;
		}
		protected override void DeleteCore(IList objects) {
			if(objects.Count > 0) {
				OnObjectDeleting(objects);
			}
			foreach(Object obj in objects) {
				objectContext.DeleteObject(obj);
			}
			if(objects.Count > 0) {
				OnObjectDeleted(objects);
			}
		}
		protected override void SetModified(Object obj, ObjectChangedEventArgs args) {
			if(obj != null) {
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(GetOriginalType(obj.GetType()));
				if(typeInfo.IsPersistent && (nonPersistentChangesEnabled || IsMemberNullOrPersistent(obj, args))) {
					ObjectStateEntry objectStateEntry = null;
					if(!objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry)) {
						String entitySetName = GetEntitySetName(GetOriginalType(obj.GetType()));
						objectContext.AddObject(entitySetName, obj);
						objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry);
					}
					if((objectStateEntry != null) && (objectStateEntry.State == EntityState.Unchanged)) {
						objectStateEntry.SetModified();
					}
					OnObjectChanged(args);
					SetIsModified(true);
				}
				else if((typeInfo.GetExtender<ComplexType>() != null) && (nonPersistentChangesEnabled || IsMemberNullOrPersistent(obj, args))) {
					OnObjectChanged(args);
					SetIsModified(true);
				}
				else {
					OnObjectChanged(args);
				}
			}
			else {
				if(nonPersistentChangesEnabled || IsMemberNullOrPersistent(null, args)) {
					SetIsModified(true);
				}
			}
		}
		protected override void Reload() {
			ObjectContext prevObjectContext = objectContext;
			List<IDisposable> prevDisposableObjects = new List<IDisposable>(disposableObjects);
			isReloading = true;
			try {
				disposableObjects.Clear();
				objectContext = CreateObjectContext(disposableObjects);
				SetIsModified(false);
			}
			finally {
				isReloading = false;
			}
			OnReloaded();
			DisposeObjectContext(prevObjectContext, prevDisposableObjects);
		}
		protected virtual void OnObjectStateManagerChanged(CollectionChangeEventArgs e) {
			if(ObjectStateManagerChanged != null) {
				ObjectStateManagerChanged(this, e);
			}
		}
		public EFObjectSpace(ITypesInfo typesInfo, EFTypeInfoSource efTypeInfoSource, CreateObjectContextHandler createObjectContextDelegate, UpdateSchemaHandler updateSchemaDelegate)
			: base(typesInfo, efTypeInfoSource) {
			this.efTypeInfoSource = efTypeInfoSource;
			this.createObjectContextDelegate = createObjectContextDelegate;
			this.updateSchemaDelegate = updateSchemaDelegate;
			disposableObjects = new List<IDisposable>();
			objectContext = CreateObjectContext(disposableObjects);
		}
		public EFObjectSpace(ITypesInfo typesInfo, EFTypeInfoSource efTypeInfoSource, CreateObjectContextHandler createObjectContextDelegate)
			: this(typesInfo, efTypeInfoSource, createObjectContextDelegate, null) {
		}
		public override void Dispose() {
			base.Dispose();
			isDisposed = true;
			efTypeInfoSource = null;
			DisposeObjectContext(objectContext, disposableObjects);
			objectContext = null;
			disposableObjects = null;
		}
		public override String Database {
			get {
				if(objectContext.Connection is EntityConnection) {
					return ((EntityConnection)objectContext.Connection).StoreConnection.Database;
				}
				else {
					return objectContext.Connection.Database;
				}
			}
		}
		public override Boolean IsConnected {
			get { return isConnected; }
		}
		public override IList ModifiedObjects {
			get { return GetModifiedObjects(); }
		}
		public ObjectContext ObjectContext {
			get { return objectContext; }
		}
		public override IDbConnection Connection {
			get { return objectContext.Connection; }
		}
		public Boolean AllowUpdateSchema {
			get { return (updateSchemaDelegate != null); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Boolean CanFilterByNonPersistentMembers {
			get { return false; }
		}
		public override void ApplyCriteria(Object collection, CriteriaOperator criteria) {
			if(collection is EFCollection) {
				((EFCollection)collection).Criteria = criteria;
			}
			else if(collection is EFServerCollection) {
				((EFServerCollection)collection).Criteria = criteria;
			}
			else if(collection is EFDataView) {
				((EFDataView)collection).Criteria = criteria;
			}
		}
		public override void ApplyFilter(Object collection, CriteriaOperator filter) {
		}
		public override Boolean CanApplyCriteria(Type collectionType) {
			return
				typeof(EFCollection).IsAssignableFrom(collectionType)
				||
				typeof(EFServerCollection).IsAssignableFrom(collectionType)
				||
				typeof(EFDataView).IsAssignableFrom(collectionType);
		}
		public override Boolean CanApplyFilter(Object collection) {
			return false;
		}
		public override Boolean Contains(Object obj) {
			if(obj == null) {
				throw new ArgumentNullException("obj");
			}
			Boolean result = false;
			if(obj is XafDataViewRecord) {
				result = (((XafDataViewRecord)obj).DataView.ObjectSpace == this);
			}
			else {
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(GetOriginalType(obj.GetType()));
				if(typeInfo.IsPersistent) {
					ObjectStateEntry objectStateEntry = null;
					result = objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry);
				}
				else {
					result = true;
				}
			}
			return result;
		}
		public override IObjectSpace CreateNestedObjectSpace() {
			return new EFObjectSpace(typesInfo, efTypeInfoSource, createObjectContextDelegate);
		}
		public override IDisposable CreateParseCriteriaScope() {
			return new ParseCriteriaScope(this);
		}
		public override Object CreateServerCollection(Type objectType, CriteriaOperator criteria) {
			EFServerCollection serverCollection = new EFServerCollection(this, objectType, criteria, null);
			return serverCollection;
		}
		public override void EnableObjectDeletionOnRemove(Object collection, Boolean deleteObjectOnRemove) {
			if(collection is EFCollection) {
				((EFCollection)collection).DeleteObjectOnRemove = deleteObjectOnRemove;
			}
		}
		public override Object Evaluate(Type objectType, CriteriaOperator expression, CriteriaOperator criteria) {
			Object result = null;
			List<DataViewExpression> dataViewExpressions = new List<DataViewExpression>();
			dataViewExpressions.Add(new DataViewExpression("A", expression));
			ObjectQuery<DbDataRecord> dataRecordsQuery = GetDataRecordsQuery(objectType, dataViewExpressions, criteria, null);
			foreach(DbDataRecord dbDataRecord in dataRecordsQuery) {
				if(dbDataRecord[0] != DBNull.Value) {
					result = dbDataRecord[0];
				}
			}
			return result;
		}
		public override Object FindObject(Type type, CriteriaOperator criteria, Boolean inTransaction) {
			IQueryable queryable = GetObjectsQuery(type, null, criteria, null);
			return GetSingleObject(queryable);
		}
		public override IList<SortProperty> GetCollectionSorting(Object collection) {
			if(collection is EFCollection) {
				return ((EFCollection)collection).Sorting;
			}
			else if(collection is EFServerCollection) {
				return ((EFServerCollection)collection).GetSorting();
			}
			else if(collection is EFDataView) {
				return ((EFDataView)collection).Sorting;
			}
			else {
				return null;
			}
		}
		public override CriteriaOperator GetCriteria(Object collection) {
			if(collection is EFCollection) {
				return ((EFCollection)collection).Criteria;
			}
			else if(collection is EFServerCollection) {
				return ((EFServerCollection)collection).Criteria;
			}
			else if(collection is EFDataView) {
				return ((EFDataView)collection).Criteria;
			}
			else {
				return null;
			}
		}
		public override String GetDisplayableProperties(Object collection) {
			if(collection is EFCollection) {
				return ((EFCollection)collection).DisplayableProperties;
			}
			else if(collection is EFServerCollection) {
				return ((EFServerCollection)collection).DisplayableProperties;
			}
			else if(collection is EFDataView) {
				String result = "";
				foreach(DataViewExpression expression in ((EFDataView)collection).Expressions) {
					result = result + expression.RawName + ";";
				}
				return result.TrimEnd(';');
			}
			else {
				return base.GetDisplayableProperties(collection);
			}
		}
		public override EvaluatorContextDescriptor GetEvaluatorContextDescriptor(Type type) {
			return new XafEvaluatorContextDescriptor(typesInfo.FindTypeInfo(type));
		}
		public override CriteriaOperator GetFilter(Object collection) {
			return null;
		}
		public override String GetKeyPropertyName(Type objectType) {
			ITypeInfo objectTypeInfo = TypesInfo.FindTypeInfo(GetOriginalType(objectType));
			if(objectTypeInfo.KeyMember != null) {
				return objectTypeInfo.KeyMember.Name;
			}
			else {
				return "";
			}
		}
		public override Type GetKeyPropertyType(Type objectType) {
			Type result = null;
			ITypeInfo typeInfo = TypesInfo.FindTypeInfo(GetOriginalType(objectType));
			if(typeInfo != null) {
				if(typeInfo.KeyMembers.Count > 1) {
					result = CompositeKeyPropertyType;
				}
				else if(typeInfo.KeyMember != null) {
					result = typeInfo.KeyMember.MemberType;
				}
			}
			return result;
		}
		public override String GetKeyValueAsString(Object obj) {
			return GetKeyValueAsString(typesInfo, obj);
		}
		public override Object GetObject(Object obj) {
			Object result = null;
			if(obj is XafDataViewRecord) {
				XafDataViewRecord dataViewRecord = (XafDataViewRecord)obj;
				ITypeInfo objectTypeInfo = TypesInfo.FindTypeInfo(GetOriginalType(dataViewRecord.ObjectType));
				if(objectTypeInfo.KeyMembers.Count > 1) {
					List<Object> keyMemberValues = new List<Object>();
					foreach(IMemberInfo keyMemberInfo in objectTypeInfo.KeyMembers) {
						keyMemberValues.Add(dataViewRecord[keyMemberInfo.Name]);
					}
					result = GetObjectByKey(dataViewRecord.ObjectType, keyMemberValues);
				}
				else {
					Object keyMemberValue = dataViewRecord[GetKeyPropertyName(dataViewRecord.ObjectType)];
					result = GetObjectByKey(dataViewRecord.ObjectType, keyMemberValue);
				}
			}
			else if(obj != null) {
				Type originalType = GetOriginalType(obj.GetType());
				ITypeInfo objectTypeInfo = typesInfo.FindTypeInfo(originalType);
				if(objectTypeInfo.IsPersistent) {
					ObjectStateEntry objectStateEntry;
					if(objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry)) {
						result = obj;
					}
					else {
						Object objectKeyValue = GetKeyValue(obj);
						result = GetObjectByKey(originalType, objectKeyValue);
					}
				}
				else {
					result = obj;
				}
			}
			return result;
		}
		public override Object GetObjectByKey(Type type, Object key) {
			Object result = null;
			if(key != null) {
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(type);
				EntityKey entityKey = null;
				CriteriaOperator criteria = null;
				if((typeInfo.KeyMembers.Count > 1) && CompositeKeyPropertyType.IsAssignableFrom(key.GetType())) {
					List<KeyValuePair<String, Object>> entityKeyValues = new List<KeyValuePair<String, Object>>();
					criteria = new GroupOperator(GroupOperatorType.And);
					for(Int32 i = 0; i < typeInfo.KeyMembers.Count; i++) {
						entityKeyValues.Add(new KeyValuePair<String, Object>(typeInfo.KeyMembers[i].Name, ((IList)key)[i]));
						((GroupOperator)criteria).Operands.Add(new BinaryOperator(typeInfo.KeyMembers[i].Name, ((IList)key)[i]));
					}
					entityKey = new EntityKey(objectContext.DefaultContainerName + "." + GetEntitySetName(type), entityKeyValues);
				}
				else {
					entityKey = new EntityKey(objectContext.DefaultContainerName + "." + GetEntitySetName(type), typeInfo.KeyMember.Name, key);
					criteria = new BinaryOperator(typeInfo.KeyMember.Name, key);
				}
				if(!objectContext.TryGetObjectByKey(entityKey, out result)) {
					IQueryable queryable = GetObjectsQuery(type, null, criteria, null);
					result = GetSingleObject(queryable);
				}
			}
			return result;
		}
		public override Object GetObjectKey(Type type, String objectKeyString) {
			Object result = null;
			Type keyPropertyType = GetKeyPropertyType(type);
			if(keyPropertyType == typeof(Int16)) {
				Int16 val = 0;
				Int16.TryParse(objectKeyString, out val);
				result = val;
			}
			else if(keyPropertyType == typeof(Int32)) {
				Int32 val = 0;
				Int32.TryParse(objectKeyString, out val);
				result = val;
			}
			else if(keyPropertyType == typeof(Int64)) {
				Int64 val = 0;
				Int64.TryParse(objectKeyString, out val);
				result = val;
			}
			else if(keyPropertyType == typeof(Guid)) {
				result = new Guid(objectKeyString);
			}
			else if(keyPropertyType == typeof(String)) {
				result = objectKeyString;
			}
			else if(keyPropertyType == CompositeKeyPropertyType) {
				result = ObjectKeyHelper.Instance.DeserializeObjectKey(objectKeyString, typeof(List<Object>));
			}
			return result;
		}
		public override IQueryable<T> GetObjectsQuery<T>(Boolean inTransaction = false) {
			return GetObjectsQueryT<T>(null, null, null);
		}
		public override ICollection GetObjectsToDelete(Boolean includeParent) {
			List<Object> objectsToDelete = new List<Object>();
			foreach(ObjectStateEntry objectStateEntry in objectContext.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted)) {
				if(objectStateEntry.Entity != null) {
					objectsToDelete.Add(objectStateEntry.Entity);
				}
			}
			return objectsToDelete;
		}
		public override ICollection GetObjectsToSave(Boolean includeParent) {
			return GetModifiedObjects();
		}
		public override Int32 GetTopReturnedObjectsCount(Object collection) {
			if(collection is EFCollection) {
				return ((EFCollection)collection).TopReturnedObjectsCount;
			}
			else if(collection is EFDataView) {
				return ((EFDataView)collection).TopReturnedObjectsCount;
			}
			else {
				return 0;
			}
		}
		public override Boolean IsCollectionLoaded(Object collection) {
			if(collection is EFCollection) {
				return ((EFCollection)collection).IsLoaded;
			}
			else if(collection is EFServerCollection) {
				return true;
			}
			else if(collection is EFDataView) {
				return ((EFDataView)collection).IsLoaded;
			}
			else {
				return true;
			}
		}
		public override Boolean IsDeletedObject(Object obj) {
			Boolean result = false;
			if((obj != null) && efTypeInfoSource.TypeIsKnown(GetOriginalType(obj.GetType()))) {
				ObjectStateEntry objectStateEntry;
				result = !objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry);
			}
			return result;
		}
		public override Boolean IsDeletionDeferredType(Type type) {
			return false;
		}
		public override Boolean IsDisposedObject(Object obj) {
			return (obj is IDisposableExt) && ((IDisposableExt)obj).IsDisposed;
		}
		public override Boolean IsNewObject(Object obj) {
			if(obj == null) {
				return false;
			}
			else if(obj is XafDataViewRecord) {
				return false;
			}
			else {
				ObjectStateEntry objectStateEntry;
				if(objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry)) {
					return (objectStateEntry.State == EntityState.Added);
				}
				else {
					return false;
				}
			}
		}
		public override Boolean IsObjectDeletionOnRemoveEnabled(Object collection) {
			if(collection is EFCollection) {
				return ((EFCollection)collection).DeleteObjectOnRemove;
			}
			else {
				return false;
			}
		}
		public override Boolean IsObjectToDelete(Object obj) {
			ObjectStateEntry objectStateEntry;
			if(objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry)) {
				return (objectStateEntry.State == EntityState.Deleted);
			}
			else {
				return false;
			}
		}
		public override Boolean IsObjectToSave(Object obj) {
			DetectChanges();
			ObjectStateEntry objectStateEntry;
			if(objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry)) {
				return (objectStateEntry.State & (EntityState.Added | EntityState.Deleted | EntityState.Modified)) != 0;
			}
			else {
				return false;
			}
		}
		public override CriteriaOperator ParseCriteria(String criteria) {
			using(IDisposable parseCriteriaScope = new ParseCriteriaScope(this)) {
				return CriteriaOperator.TryParse(criteria);
			}
		}
		public override void ReloadCollection(Object collection) {
			if(collection is EFCollection) {
				((EFCollection)collection).Reload();
			}
			else if(collection is EFServerCollection) {
				((EFServerCollection)collection).Reload();
			}
			else if(collection is EFDataView) {
				((EFDataView)collection).Reload();
			}
		}
		public override Object ReloadObject(Object obj) {
			DetectChanges();
			obj = GetObject(obj);
			ObjectStateEntry objectStateEntry;
			if(objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry) && (objectStateEntry.Entity != null)) {
				if(
					(objectStateEntry.State == EntityState.Unchanged)
					||
					(objectStateEntry.State == EntityState.Modified)
					||
					(objectStateEntry.State == EntityState.Deleted)
				) {
					isObjectReloading = true;
					try {
						objectContext.Refresh(RefreshMode.StoreWins, obj);
					}
					finally {
						isObjectReloading = false;
					}
					OnObjectReloaded(obj);
				}
			}
			return obj;
		}
		public override void RemoveFromModifiedObjects(Object obj) {
			DetectChanges();
			ObjectStateEntry objectStateEntry;
			if(objectContext.ObjectStateManager.TryGetObjectStateEntry(obj, out objectStateEntry) && (objectStateEntry.Entity != null)) {
				if(objectStateEntry.State != EntityState.Detached) {
					objectContext.ObjectStateManager.ChangeObjectState(obj, EntityState.Unchanged);
				}
			}
		}
		public override void SetCollectionSorting(Object collection, IList<SortProperty> sorting) {
			if(collection is EFCollection) {
				((EFCollection)collection).Sorting = sorting;
			}
			else if(collection is EFServerCollection) {
				((EFServerCollection)collection).SetSorting(sorting);
			}
			else if(collection is EFDataView) {
				((EFDataView)collection).Sorting = sorting;
			}
		}
		public override void SetDisplayableProperties(Object collection, String displayableProperties) {
			if(collection is EFCollection) {
				((EFCollection)collection).DisplayableProperties = displayableProperties;
			}
			else if(collection is EFServerCollection) {
				((EFServerCollection)collection).DisplayableProperties = displayableProperties;
			}
			else if(collection is EFDataView) {
				((EFDataView)collection).Expressions = ConvertExpressionsStringToExpressionsList(displayableProperties);
			}
			else {
				base.SetDisplayableProperties(collection, displayableProperties);
			}
		}
		public override void SetTopReturnedObjectsCount(Object collection, Int32 topReturnedObjects) {
			if(collection is EFCollection) {
				((EFCollection)collection).TopReturnedObjectsCount = topReturnedObjects;
			}
			else if(collection is EFDataView) {
				((EFDataView)collection).TopReturnedObjectsCount = topReturnedObjects;
			}
		}
		public event CollectionChangeEventHandler ObjectStateManagerChanged;
		public static String[] GetReferenceMemberNames(ITypeInfo typeInfo, IList<String> memberNames) {
			List<String> result = new List<String>();
			foreach(String memberName in memberNames) {
				IMemberInfo memberInfo = typeInfo.FindMember(memberName.Trim());
				Boolean isPersistentReference =
					memberInfo.IsPersistent
					&& (memberInfo.MemberTypeInfo != null)
					&& memberInfo.MemberTypeInfo.IsDomainComponent
					&& (memberInfo.MemberTypeInfo.GetExtender<ComplexType>() == null);
				if(isPersistentReference) {
					if(!result.Contains(memberInfo.Name)) {
						result.Add(memberInfo.Name);
					}
				}
				else if(memberInfo.GetPath().Count > 1) {
					IMemberInfo nextToLastMemberInfo = memberInfo.GetPath()[memberInfo.GetPath().Count - 2];
					isPersistentReference =
						nextToLastMemberInfo.IsPersistent
						&& (nextToLastMemberInfo.MemberTypeInfo != null)
						&& nextToLastMemberInfo.MemberTypeInfo.IsDomainComponent
						&& (nextToLastMemberInfo.MemberTypeInfo.GetExtender<ComplexType>() == null);
					if(isPersistentReference) {
						String referenceMemberName = "";
						for(Int32 i = 0; i < memberInfo.GetPath().Count - 1; i++) {
							referenceMemberName = referenceMemberName + memberInfo.GetPath()[i].Name + ".";
						}
						referenceMemberName = referenceMemberName.TrimEnd('.');
						if(!result.Contains(referenceMemberName)) {
							result.Add(referenceMemberName);
						}
					}
				}
			}
			return result.ToArray();
		}
		public static String[] GetReferenceMemberNames(ITypeInfo typeInfo, String memberNames) {
			return GetReferenceMemberNames(typeInfo, memberNames.Split(new String[] { ";" }, StringSplitOptions.RemoveEmptyEntries));
		}
		public static IList<IMemberInfo> GetDefaultDisplayableMembers(ITypeInfo typeInfo) {
			List<IMemberInfo> result = new List<IMemberInfo>();
			foreach(IMemberInfo memberInfo in typeInfo.Members) {
				if(memberInfo.IsVisible || (memberInfo == memberInfo.Owner.KeyMember)) {
					result.Add(memberInfo);
				}
			}
			return result;
		}
		public static SqlException GetSqlException(Exception exception) {
			SqlException result = null;
			while(exception != null) {
				if(exception is SqlException) {
					result = (SqlException)exception;
					break;
				}
				else {
					exception = exception.InnerException;
				}
			}
			return result;
		}
		public static String GetKeyValueAsString(ITypesInfo typesInfo, Object obj) {
			String result = "";
			Object keyValue = BaseObjectSpace.GetKeyValue(typesInfo, obj);
			if(keyValue != null) {
				Type objectType = null;
				if(obj is XafDataViewRecord) {
					objectType = ((XafDataViewRecord)obj).ObjectType;
				}
				else if(obj != null) {
					objectType = obj.GetType();
				}
				if(EFObjectSpace.CompositeKeyPropertyType.IsAssignableFrom(keyValue.GetType()) && (typesInfo.FindTypeInfo(objectType).KeyMembers.Count > 1)) {
					result = ObjectKeyHelper.Instance.SerializeObjectKey(keyValue);
				}
				else {
					result = keyValue.ToString();
				}
			}
			return result;
		}
	}
}
