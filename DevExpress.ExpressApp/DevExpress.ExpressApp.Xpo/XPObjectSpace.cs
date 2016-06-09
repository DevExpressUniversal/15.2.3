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
using System.Data;
using System.Data.Common;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.DC.Xpo;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Xpo {
	public delegate UnitOfWork CreateUnitOfWorkHandler();
	public class XPObjectSpace : BaseObjectSpace, IAsyncServerDataSourceCreator, IDataLockingManager {
		private const String ObjectSpaceKey = "ObjectSpace";
		private Dictionary<Object, Boolean> isNewCache = new Dictionary<Object, Boolean>();
		protected UnitOfWork session;
		protected internal XpoTypeInfoSource xpoTypeInfoSource;
		private void session_AfterConnect(Object sender, SessionManipulationEventArgs e) {
			OnConnected();
		}
		private void session_ObjectChanged(Object sender, ObjectChangeEventArgs e) {
			if(!IsCommitting) {
				if(e.Reason == ObjectChangeReason.PropertyChanged) {
					SetModified(sender, new ObjectChangedEventArgs(sender, e.PropertyName, e.OldValue, e.NewValue));
				}
				if(e.Reason == ObjectChangeReason.EndEdit) {
					OnObjectEndEdit(sender);
				}
				if(e.Reason == ObjectChangeReason.Reset) {
					OnObjectReloaded(sender);
				}
			}
		}
		private void session_ObjectSaving(Object sender, ObjectManipulationEventArgs e) {
			OnObjectSaving(e.Object);
			if(e.Object is IXafEntityObject) {
				((IXafEntityObject)e.Object).OnSaving();
			}
			Tracing.Tracer.LogLockedSectionEntering(GetType(), "session_ObjectSaving", isNewCache);
			lock(isNewCache) {
				Tracing.Tracer.LogLockedSectionEntered();
				isNewCache.Remove(e.Object);
			}
		}
		private void session_ObjectSaved(Object sender, ObjectManipulationEventArgs e) {
			OnObjectSaved(e.Object);
		}
		private void session_ObjectDeleting(Object sender, ObjectManipulationEventArgs e) {
			OnObjectDeleting(new Object[] { e.Object });
		}
		private void session_ObjectDeleted(Object sender, ObjectManipulationEventArgs e) {
			OnObjectDeleted(new Object[] { e.Object });
		}
		private void session_ObjectLoading(Object sender, ObjectManipulationEventArgs e) {
			if(e.Object is IObjectSpaceLink) {
				((IObjectSpaceLink)e.Object).ObjectSpace = this;
			}
		}
		private void session_ObjectLoaded(Object sender, ObjectManipulationEventArgs e) {
			if(e.Object is IXafEntityObject) {
				((IXafEntityObject)e.Object).OnLoaded();
			}
		}
		private String AddKeyMemberNamesToDisplayableProperties(ITypeInfo objectTypeInfo, String displayableProperties) {
			String result = displayableProperties;
			IList<String> displayablePropertiesList = displayableProperties.Replace("[", "").Replace("]", "").Split(';');
			foreach(IMemberInfo keyMemberInfo in objectTypeInfo.KeyMembers) {
				if(!displayablePropertiesList.Contains(keyMemberInfo.Name)) {
					result = result + ";" + keyMemberInfo.Name;
				}
			}
			return result;
		}
		private Boolean IsNewObjectInDifferentObjectSpace(Object obj)
		{
			Boolean result = false;
			if(!IsDisposedObject(obj)) {
				IObjectSpace objectSpace = FindObjectSpaceByObject(obj);
				if(objectSpace != null) {
					result = objectSpace.IsNewObject(obj);
				}
			}
			return result;
		}
		private IMemberInfo FindKeyProperty(Type type) {
			CheckIsDisposed();
			IMemberInfo result = null;
			ITypeInfo typeInfo = typesInfo.FindTypeInfo(type);
			if(typeInfo != null) {
				result = typeInfo.KeyMember;
			}
			return result;
		}
		private XPClassInfo FindXPClassInfo(Type type) {
			ITypeInfo typeInfo = typesInfo.FindTypeInfo(type);
			if(xpoTypeInfoSource.TypeIsKnown(typeInfo.Type)) {
				return xpoTypeInfoSource.GetEntityClassInfo(typeInfo.Type);
			}
			return null;
		}
		private XPClassInfo FindRealXPClassInfo(Type type) {
			XPClassInfo classInfo = null;
			ITypeInfo typeInfo = typesInfo.FindTypeInfo(type);
			if(xpoTypeInfoSource.TypeIsKnown(typeInfo.Type)) {
				if(typeInfo.IsInterface) {
					classInfo = session.Dictionary.QueryClassInfo(typeInfo.Type);
				}
				else {
					classInfo = FindXPClassInfo(typeInfo.Type);
				}
			}
			return classInfo;
		}
		private XPClassInfo FindObjectXPClassInfo(Object obj) {
			if(obj is XpoDataViewRecord) {
				return session.Dictionary.QueryClassInfo(((XpoDataViewRecord)obj).DataView.ObjectType);
			}
			else {
				return session.Dictionary.QueryClassInfo(obj);
			}
		}
		private Boolean IsPersistentType(Type type) {
			Boolean isPersistentType = false;
			if(type != null) {
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(type);
				if(typeInfo != null) {
					if(typeInfo.IsInterface) {
						isPersistentType = typeInfo.IsPersistent;
					}
					else {
						XPClassInfo classInfo = FindXPClassInfo(type);
						if(classInfo != null) {
							isPersistentType = classInfo.IsPersistent;
						}
					}
				}
			}
			return isPersistentType;
		}
		private void CheckIsRegisteredType(Type type, Boolean includeUnknownTypes) {
			if(type.IsInterface) {
				throw new ArgumentException(String.Format(
					@"The ""{0}"" interface is not registered within the business model. To avoid this error, register the specified interface via the XafTypesInfo.Instance.RegisterEntity(string, Type, Type) or  XafTypesInfo.Instance.RegisterSharedPart(Type) methods (http://documentation.devexpress.com/#Xaf/DevExpressExpressAppDCITypesInfo_RegisterEntitytopic).",
					type.FullName));
			}
			if(includeUnknownTypes || xpoTypeInfoSource.TypeIsKnown(type)) {
				if(!((TypesInfo)typesInfo).IsRegisteredEntity(type)) {
					throw new ArgumentException(String.Format(
						@"The ""{0}"" class is not registered within the business model. To avoid this error, use the Module Designer (http://documentation.devexpress.com/#Xaf/CustomDocument2828) to include the specified class into the module's AdditionalExportedTypes collection or manually register it via the XafTypesInfo.Instance.RegisterEntity(Type) method.",
						type.FullName));
				}
			}
		}
		private void ReloadObject(Object obj, HashSet<Object> reloadedObjects) {
			if((obj != null) && (session.Dictionary.QueryClassInfo(obj) != null) && !reloadedObjects.Contains(obj)) {
				session.Reload(obj);
				reloadedObjects.Add(obj);
				XPClassInfo xpClassInfo = FindObjectXPClassInfo(obj);
				foreach(XPMemberInfo xpMemberInfo in xpClassInfo.ObjectProperties) {
					if(xpMemberInfo.IsAggregated) {
						Object aggregatedObject = xpMemberInfo.GetValue(obj);
						ReloadObject(aggregatedObject, reloadedObjects);
					}
				}
				foreach(XPMemberInfo xpMemberInfo in xpClassInfo.AssociationListProperties) {
					if(xpMemberInfo.IsAggregated) {
						Object aggregatedCollection = xpMemberInfo.GetValue(obj);
						if(IsCollectionLoaded(aggregatedCollection)) {
							ReloadCollection(aggregatedCollection);
							IList list = ListHelper.GetList(aggregatedCollection);
							if(list != null) {
								ArrayList aggregatedCollectionObjects = new ArrayList(list);
								foreach(Object aggregatedCollectionObject in aggregatedCollectionObjects) {
									ReloadObject(aggregatedCollectionObject, reloadedObjects);
								}
							}
						}
					}
				}
			}
		}
		protected void DropIsNewCache() {
			Tracing.Tracer.LogLockedSectionEntering(GetType(), "DropIsNewCache", isNewCache);
			lock(isNewCache) {
				Tracing.Tracer.LogLockedSectionEntered();
				isNewCache.Clear();
			}
		}
		protected Object GetObjectInCurrentSessionFromNestedSession(Object obj) {
			Object result = null;
			IXPSimpleObject xpObj = obj as IXPSimpleObject;
			if((xpObj != null) && !IsDisposedObject(xpObj)) {
				if(xpObj.Session == Session) {
					result = obj;
				}
				else {
					NestedUnitOfWork nestedUOW = xpObj.Session as NestedUnitOfWork;
					if(nestedUOW != null) {
						IXPSimpleObject parentObj = (IXPSimpleObject)nestedUOW.GetParentObject(xpObj);
						if((parentObj != null) && !IsDisposedObject(parentObj) && (parentObj.Session == Session)) {
							result = parentObj;
						}
					}
				}
			}
			return result;
		}
		protected virtual void SetSession(UnitOfWork newSession) {
			if(session != newSession) {
				if(session != null) {
					((IWideDataStorage)session).SetWideDataItem(ObjectSpaceKey, null);
					session.AfterConnect -= session_AfterConnect;
					session.ObjectChanged -= session_ObjectChanged;
					session.ObjectSaving -= session_ObjectSaving;
					session.ObjectSaved -= session_ObjectSaved;
					session.ObjectDeleting -= session_ObjectDeleting;
					session.ObjectDeleted -= session_ObjectDeleted;
					session.ObjectLoading -= session_ObjectLoading;
					session.ObjectLoaded -= session_ObjectLoaded;
				}
				session = newSession;
				if(session != null) {
					((IWideDataStorage)session).SetWideDataItem(ObjectSpaceKey, this);
					session.AfterConnect += session_AfterConnect;
					session.ObjectChanged += session_ObjectChanged;
					session.ObjectSaving += session_ObjectSaving;
					session.ObjectSaved += session_ObjectSaved;
					session.ObjectDeleting += session_ObjectDeleting;
					session.ObjectDeleted += session_ObjectDeleted;
					session.ObjectLoading += session_ObjectLoading;
					session.ObjectLoaded += session_ObjectLoaded;
				}
			}
		}
		protected virtual UnitOfWork RecreateUnitOfWork() {
			if(createUnitOfWorkDelegate == null) {
				return new UnitOfWork(session.ObjectLayer);
			}
			return createUnitOfWorkDelegate();
		}
		protected override Object CreateObjectCore(Type type) {
			Object obj = null;
			ITypeInfo typeInfo = TypesInfo.FindTypeInfo(type);
			Type typeToInstantiate = typeInfo.Type;
			if(typeToInstantiate.IsInterface) {
				XPClassInfo classInfo = FindXPClassInfo(typeToInstantiate);
				if(classInfo != null && !classInfo.ClassType.IsInterface) {
					obj = classInfo.CreateNewObject(session);
				}
				else {
					throw new ArgumentException(String.Format(@"The ""{0}"" interface is not registered.", typeToInstantiate.FullName));
				}
			}
			else {
				obj = TypeHelper.CreateInstance(typeToInstantiate, session);
			}
			return obj;
		}
		protected virtual Object GetObjectInCurrentSession(Object obj, XPClassInfo classInfo) {
			if(!classInfo.IsPersistent) {
				return null;
			}
			Object result = GetObjectInCurrentSessionFromNestedSession(obj);
			if((result == null) && !IsNewObjectInDifferentObjectSpace(obj)) {
				result = GetObjectByKey(obj.GetType(), session.GetKeyValue(obj));
			}
			return result;
		}
		protected override void CheckLocking() {
			base.CheckLocking();
			if(Session.LockingOption != LockingOption.None) {
				IDictionary<XPClassInfo, List<Object>> objectToCheckKeysByClassInfo = GetObjectToLockingCheckKeys(session);
				foreach(KeyValuePair<XPClassInfo, List<Object>> item in objectToCheckKeysByClassInfo) {
					XPClassInfo classInfo = item.Key;
					XPMemberInfo optimisticLockFieldInDataLayerInfo = classInfo.OptimisticLockFieldInDataLayer;
					List<Object> objectToCheckKeys = item.Value;
					const int MaxObjectsInView = 1000;
					int currentIndex = 0;
					while(currentIndex < objectToCheckKeys.Count) {
						int restCount = objectToCheckKeys.Count - currentIndex;
						if(restCount > MaxObjectsInView) {
							restCount = MaxObjectsInView;
						}
						IEnumerable<Object> keysPortion = objectToCheckKeys.GetRange(currentIndex, restCount);
						currentIndex = currentIndex + restCount;
						XPView view = new XPView(session, classInfo);
						view.Criteria = new InOperator(classInfo.KeyProperty.Name, keysPortion);
						view.AddProperty(classInfo.KeyProperty.Name);
						view.AddProperty(classInfo.OptimisticLockFieldName);
						bool inTransactionMode = Session.InTransactionMode;
						try {
							Session.InTransactionMode = false;
							int count = view.Count; 
						}
						finally {
							Session.InTransactionMode = inTransactionMode;
						}
						foreach(ViewRecord record in view) {
							Object objectToCheck = session.GetLoadedObjectByKey(classInfo, record[classInfo.KeyProperty.Name]);
							Object lockFieldValue = optimisticLockFieldInDataLayerInfo.GetValue(objectToCheck);
							Object valueInDatabase = record[classInfo.OptimisticLockFieldName];
							if(EvalHelpers.CompareObjects(lockFieldValue, valueInDatabase, true, true, null) != 0) {
								String message = UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.ObjectToSaveWasChanged)
									+ String.Format(" ClassName: {0}. ID: {1}.", objectToCheck.GetType().FullName, record[classInfo.KeyProperty.Name]);
								Tracing.Tracer.LogText(message);
								throw new UserFriendlyException(message);
							}
						}
					}
				}
			}
		}
		private static IDictionary<XPClassInfo, List<Object>> GetObjectToLockingCheckKeys(Session session) {
			IDictionary<XPClassInfo, List<Object>> objectToSaveKeysByClassInfo = new Dictionary<XPClassInfo, List<Object>>();
			ICollection objectsToSave = session.GetObjectsToSave();
			foreach(Object objectToSave in objectsToSave) {
				if(session.IsNewObject(objectToSave)) {
					continue;
				}
				XPClassInfo classInfo = session.GetClassInfo(objectToSave);
				if(classInfo.OptimisticLockFieldInDataLayer != null && !String.IsNullOrEmpty(classInfo.OptimisticLockFieldName)) {
					List<Object> objectToSaveKeys;
					if(!objectToSaveKeysByClassInfo.TryGetValue(classInfo, out objectToSaveKeys)) {
						objectToSaveKeys = new List<Object>();
						objectToSaveKeysByClassInfo.Add(classInfo, objectToSaveKeys);
					}
					Object key = session.GetKeyValue(objectToSave);
					objectToSaveKeys.Add(key);
				}
			}
			return objectToSaveKeysByClassInfo;
		}
		protected virtual bool IsNewObjectCore(Object obj) {
			if(obj is XpoDataViewRecord) {
				return false;
			}
			else {
				return session.IsNewObject(obj);
			}
		}
		protected override void DoCommit() {
			isCommitting = true;
			try {
				session.CommitChanges();
			}
			catch(DevExpress.Xpo.DB.Exceptions.LockingException e) {
				throw new UserFriendlyException(UserVisibleExceptionId.ObjectToSaveWasChanged, e);
			}
			finally {
				isCommitting = false;
			}
			DropIsNewCache();
		}
		protected override void DeleteCore(IList objects) {
			session.Delete(objects);
		}
		protected override IList CreateCollection(Type objectType, CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction) {
			CheckIsDisposed();
			Guard.ArgumentNotNull(objectType, "objectType");
			IList result = null;
			XPClassInfo xpClassInfo = FindRealXPClassInfo(objectType);
			if(xpClassInfo == null) {
				CheckIsRegisteredType(objectType, false);
			}
			else {
				Type dataType = xpClassInfo.ClassType;
				if(dataType.IsInterface) {
					result = PersistentInterfaceHelper.CreateCollection(Session, dataType, criteria, CreateSortingCollection(sorting), inTransaction);
				}
				else {
					if(inTransaction) {
						result = new XPCollection(PersistentCriteriaEvaluationBehavior.InTransaction, session, xpClassInfo, criteria);
					}
					else {
						result = new XPCollection(session, xpClassInfo, criteria);
						((XPBaseCollection)result).Sorting = CreateSortingCollection(sorting);
						((XPBaseCollection)result).LoadingEnabled =
							!CollectionSourceBase.EmptyCollectionCriteria.Equals(criteria)
							&& IsPersistentType(dataType);
					}
				}
			}
			return result;
		}
		protected override IList<T> CreateCollection<T>(CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction) {
			CheckIsDisposed();
			IList<T> result = null;
			if(typeof(T).IsInterface) {
				result = PersistentInterfaceHelper.CreateCollection<T>(Session, criteria, CreateSortingCollection(sorting), inTransaction);
			}
			else {
				if(inTransaction) {
					result = new XPCollection<T>(PersistentCriteriaEvaluationBehavior.InTransaction, session, criteria);
				}
				else {
					result = new XPCollection<T>(session, criteria);
					((XPBaseCollection)result).Sorting = CreateSortingCollection(sorting);
					((XPBaseCollection)result).LoadingEnabled = !CollectionSourceBase.EmptyCollectionCriteria.Equals(criteria)
						&& IsPersistentType(typeof(T));
				}
			}
			return result;
		}
		protected override IList CreateDataViewCore(Type objectType, IList<DataViewExpression> expressions, CriteriaOperator criteria, IList<SortProperty> sorting) {
			return new XpoDataView(this, objectType, expressions, criteria, sorting);
		}
		protected override CriteriaOperator GetAssociatedCollectionCriteriaCore(Object obj, IMemberInfo memberInfo) {
			XPMemberInfo xpMember = session.Dictionary.GetClassInfo(obj.GetType()).FindMember(memberInfo.Name);
			if((xpMember != null) && xpMember.IsAssociation) {
				AssociatedCollectionCriteriaHelper.Result result = AssociatedCollectionCriteriaHelper.ResolveDataForCollection(obj, xpMember);
				return result.Criteria;
			}
			else if(memberInfo.IsList && (memberInfo.ListElementType != null) && (memberInfo.AssociatedMemberInfo != null)) {
				if(memberInfo.IsManyToMany) {
					return new ContainsOperator(memberInfo.AssociatedMemberInfo.Name, new BinaryOperator("This", obj));
				}
				else {
					return new BinaryOperator(memberInfo.AssociatedMemberInfo.Name, obj);
				}
			}
			else {
				return null;
			}
		}
		protected override ICollection<ICustomFunctionOperator> GetCustomFunctionOperators() {
			return session.Dictionary.CustomFunctionOperators;
		}
		protected override void Reload() {
			Session prevSession = session;
			isReloading = true;
			try {
				DropIsNewCache();
				SetSession(RecreateUnitOfWork());
				SetIsModified(false);
			}
			finally {
				isReloading = false;
			}
			OnReloaded();
			if(prevSession != null) {
				prevSession.Dispose();
			}
		}
		protected override void SetModified(Object obj, ObjectChangedEventArgs args) {
			CheckIsDisposed();
			if(obj != null) {
				XPClassInfo objClassInfo = FindObjectXPClassInfo(obj);
				if(objClassInfo != null && objClassInfo.IsPersistent && (nonPersistentChangesEnabled || IsMemberNullOrPersistent(obj, args))) {
					session.Save(obj);
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
		protected CreateUnitOfWorkHandler createUnitOfWorkDelegate;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Action<ResolveSessionEventArgs> AsyncServerModeSourceResolveSession;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Action<ResolveSessionEventArgs> AsyncServerModeSourceDismissSession;
		protected internal Object CreateServerCollection(Type objectType, CriteriaOperator criteria, Boolean asyncMode) {
			Object result = null;
			XPClassInfo xpClassInfo = FindRealXPClassInfo(objectType);
			if(asyncMode) {
				if(xpClassInfo != null) {
					if(AsyncServerModeSourceResolveSession == null) {
						throw new InvalidOperationException("Unable to create 'XPInstantFeedbackSource'. The 'AsyncServerModeSourceResolveSession' delegate is null.");
					}
					if(AsyncServerModeSourceDismissSession == null) {
						throw new InvalidOperationException("Unable to create 'XPInstantFeedbackSource'. The 'AsyncServerModeSourceDismissSession' delegate is null.");
					}
					result = new XPInstantFeedbackSource(xpClassInfo, "", criteria,
						AsyncServerModeSourceResolveSession, AsyncServerModeSourceDismissSession);
				}
			}
			else {
				XPServerCollectionSource serverCollectionSource = null;
				if(xpClassInfo != null) {
					Type dataType = xpClassInfo.ClassType;
					if(dataType.IsInterface) {
						Type interfaceDataType = PersistentInterfaceHelper.GetPersistentInterfaceDataType(dataType);
						XPServerCollectionSource collectionSource = new XPServerCollectionSource(session, interfaceDataType, criteria);
						return new ServerModeForPersistentInterfacesWrapper(dataType, (IXpoServerModeGridDataSource)((IListSource)collectionSource).GetList());
					}
					else {
						serverCollectionSource = new XPServerCollectionSource(session, xpClassInfo, criteria);
					}
				}
				else {
					serverCollectionSource = new XPServerCollectionSource(session, objectType, criteria);
				}
				serverCollectionSource.AllowNew = true;
				serverCollectionSource.AllowEdit = true;
				serverCollectionSource.AllowRemove = true;
				result = serverCollectionSource;
			}
			return result;
		}
		public XPObjectSpace(ITypesInfo typesInfo, XpoTypeInfoSource xpoTypeInfoSource, CreateUnitOfWorkHandler createUnitOfWorkDelegate)
			: base(typesInfo, xpoTypeInfoSource) {
			Guard.ArgumentNotNull(typesInfo, "typesInfo");
			Guard.ArgumentNotNull(xpoTypeInfoSource, "xpoTypeInfoSource");
			Guard.ArgumentNotNull(createUnitOfWorkDelegate, "createUnitOfWorkDelegate");
			LockingCheckEnabled = true;
			this.xpoTypeInfoSource = xpoTypeInfoSource;
			this.createUnitOfWorkDelegate = createUnitOfWorkDelegate;
			UnitOfWork unitOfWork = createUnitOfWorkDelegate();
			SetSession(unitOfWork);
		}
		public override void Dispose() {
			base.Dispose();
			AsyncServerModeSourceResolveSession = null;
			AsyncServerModeSourceDismissSession = null;
			if(!isDisposed) {
				isDisposed = true;
				DropIsNewCache();
				if(session != null) {
					Session sessionReference = session;
					SetSession(null);
					sessionReference.Dispose();
				}
				typesInfo = null;
				xpoTypeInfoSource = null;
			}
		}
		public override Object ReloadObject(Object obj) {
			Object currentObj = GetObject(obj);
			ReloadObject(currentObj, new HashSet<Object>());
			return currentObj;
		}
		public override object GetObjectByKey(Type type, Object key) {
			Object result = null;
			if(key != null) {
				XPClassInfo xpClassInfo = FindXPClassInfo(type);
				if(xpClassInfo != null) {
					Type queryableType = xpClassInfo.ClassType;
					if(queryableType.IsInterface) {
						queryableType = PersistentInterfaceHelper.GetPersistentInterfaceDataType(queryableType);
						object dataObject = session.GetObjectByKey(queryableType, key);
						result = PersistentInterfaceHelper.GetInterfaceInstance(type, dataObject);
					}
					else {
						result = session.GetObjectByKey(queryableType, key);
					}
				}
			}
			return result;
		}
		public override IQueryable<T> GetObjectsQuery<T>(Boolean inTransaction) {
			return new XPQuery<T>(session, inTransaction);
		}
		public override Boolean IsNewObject(Object obj) {
			CheckIsDisposed();
			if(obj != null && Contains(obj)) {
				Boolean result = false;
				Tracing.Tracer.LogLockedSectionEntering(GetType(), "IsNewObject", isNewCache);
				lock(isNewCache) {
					if(!isNewCache.TryGetValue(obj, out result)) {
						XPClassInfo xpClassInfo = FindObjectXPClassInfo(obj);
						if((xpClassInfo != null) && xpClassInfo.IsPersistent) {
							result = IsNewObjectCore(obj);
						}
						isNewCache.Add(obj, result);
					}
				}
				return result;
			}
			return false;
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Boolean TryGetObjectHandle(Object theObject, out String handle) {
			CheckIsDisposed();
			handle = null;
			if(theObject == null) {
				return false;
			}
			XPClassInfo classInfo = FindObjectXPClassInfo(theObject);
			if(classInfo == null || !classInfo.IsPersistent || IsNewObject(theObject)) {
				return false;
			}
			handle = ObjectHandleHelper.CreateObjectHandle(TypesInfo, theObject.GetType(), GetKeyValueAsString(theObject));
			return true;
		}
		public override Type GetKeyPropertyType(Type type) {
			IMemberInfo memberInfo = FindKeyProperty(type);
			return (memberInfo != null) ? memberInfo.MemberType : null;
		}
		public override String GetKeyPropertyName(Type type) {
			IMemberInfo memberInfo = FindKeyProperty(type);
			return (memberInfo != null) ? memberInfo.Name : String.Empty;
		}
		public override Object GetKeyValue(Object obj) {
			Guard.ArgumentNotNull(obj, "obj");
			if(obj is XpoDataViewRecord) {
				XpoDataViewRecord dataViewRecord = (XpoDataViewRecord)obj;
				ITypeInfo typeInfo = typesInfo.FindTypeInfo(dataViewRecord.ObjectType);
				if(dataViewRecord.ContainsMember(typeInfo.KeyMember.Name)) {
					return dataViewRecord[typeInfo.KeyMember.Name];
				}
				else {
					return null;
				}
			}
			else {
				XPClassInfo ci = FindObjectXPClassInfo(obj);
				if((ci != null) && (ci.KeyProperty != null)) {
					return session.GetKeyValue(obj);
				}
				else {
					return base.GetKeyValue(obj);
				}
			}
		}
		public override String GetKeyValueAsString(Object obj) {
			CheckIsDisposed();
			String result = "";
			Object keyValue = GetKeyValue(obj);
			if(keyValue != null) {
				if(keyValue is IdList) {
					result = ObjectKeyHelper.Instance.SerializeObjectKey(keyValue);
				}
				else {
					result = keyValue.ToString();
				}
			}
			return result;
		}
		public override object GetObjectKey(Type objectType, String objectKeyString) {
			CheckIsDisposed();
			Object result = null;
			XPClassInfo xpClassInfo = FindXPClassInfo(objectType);
			if(xpClassInfo != null) {
				Type queryableType = xpClassInfo.ClassType;
				if(queryableType.IsInterface) {
					queryableType = PersistentInterfaceHelper.GetPersistentInterfaceDataType(queryableType);
					xpClassInfo = Session.GetClassInfo(queryableType);
				}
				XPMemberInfo keyMember = xpClassInfo.KeyProperty;
				if(keyMember != null) {
					if(!keyMember.IsStruct) {
						if(keyMember.ReferenceType != null) {
							result = GetObjectKey(keyMember.ReferenceType.ClassType, objectKeyString);
						}
						else {
							try {
								result = ReflectionHelper.Convert(objectKeyString, keyMember.MemberType);
							}
							catch(Exception e) {
								throw new ArgumentException(string.Format(
									"Cannot convert the '{0}' value to the type of the '{1}' key property type, error: {2}",
									objectKeyString, objectType, e.Message));
							}
						}
					}
					else {
						try {
							result = ObjectKeyHelper.Instance.DeserializeObjectKey(objectKeyString, typeof(DevExpress.Xpo.Helpers.IdList));
						}
						catch(Exception e) {
							throw new ArgumentException(string.Format(
								"Cannot restore object key value from string {0}, error {1}:",
								objectKeyString, e.Message));
						}
					}
				}
			}
			return result;
		}
		public override Boolean CanInstantiate(Type type) {
			CheckIsDisposed();
			Guard.ArgumentNotNull(type, "type");
			Type originalType = xpoTypeInfoSource.GetOriginalType(type) ?? type;
			Boolean result = false;
			if(xpoTypeInfoSource.RegisteredEntities.Contains(originalType)) {
				ITypeInfo typeInfo = TypesInfo.FindTypeInfo(originalType);
				if(typeInfo.IsPersistent && !typeInfo.IsAbstract) {
					if(type.IsInterface) {
						result = true;
					}
					else {
						result = TypeHelper.CanCreateInstance(type, typeof(Session));
					}
				}
			}
			return result;
		}
		public override IObjectSpace CreateNestedObjectSpace() {
			CheckIsDisposed();
			XPNestedObjectSpace nestedObjectSpace = new XPNestedObjectSpace(this);
			nestedObjectSpace.NonPersistentChangesEnabled = nonPersistentChangesEnabled;
			nestedObjectSpace.AsyncServerModeSourceResolveSession = AsyncServerModeSourceResolveSession;
			nestedObjectSpace.AsyncServerModeSourceDismissSession = AsyncServerModeSourceDismissSession;
			return nestedObjectSpace;
		}
		public override Object Evaluate(Type objectType, CriteriaOperator expression, CriteriaOperator criteria) {
			Object result = null;
			if(IsPersistentType(objectType)) {
				XPClassInfo xpClassInfo = FindRealXPClassInfo(objectType);
				if(xpClassInfo != null) {
					Type dataType = xpClassInfo.ClassType;
					if(dataType.IsInterface) {
						xpClassInfo = Session.GetClassInfo(PersistentInterfaceHelper.GetPersistentInterfaceDataType(dataType));
					}
					result = Session.Evaluate(xpClassInfo, expression, criteria);
				}
			}
			return result;
		}
		public override Object CreateServerCollection(Type objectType, CriteriaOperator criteria) {
			return CreateServerCollection(objectType, criteria, false);
		}
		public override void ReloadCollection(Object collection) {
			if(collection is XPBaseCollection) {
				((XPBaseCollection)collection).Reload();
			}
			else if(collection is XPServerCollectionSource) {
				((XPServerCollectionSource)collection).Reload();
			}
			else if(collection is XpoDataView) {
				((XpoDataView)collection).Reload();
			}
			else if(collection is XPInstantFeedbackSource) {
				((XPInstantFeedbackSource)collection).Refresh();
			}
			else {
				Object morphed;
				if(PersistentInterfaceHelper.IsPersistentInterfaceMorpher(collection, out morphed)) {
					ReloadCollection(morphed);
				}
				else if(collection is IXPUnloadableAssociationList) {
					((IXPUnloadableAssociationList)collection).Unload();
				}
			}
		}
		public override Boolean IsCollectionLoaded(Object collection) {
			if(collection is XPBaseCollection) {
				return ((XPBaseCollection)collection).IsLoaded;
			}
			else if(collection is XPServerCollectionSource) {
				return true;
			}
			else if(collection is XpoDataView) {
				return ((XpoDataView)collection).IsLoaded;
			}
			Object morphed;
			if(PersistentInterfaceHelper.IsPersistentInterfaceMorpher(collection, out morphed)) {
				return IsCollectionLoaded(morphed);
			}
			else if(collection is IXPUnloadableAssociationList) {
				return ((IXPUnloadableAssociationList)collection).IsLoaded;
			}
			else {
				return false;
			}
		}
		public override Boolean CanApplyCriteria(Type collectionType) {
			return
				typeof(XPBaseCollection).IsAssignableFrom(collectionType)
				||
				typeof(XPServerCollectionSource).IsAssignableFrom(collectionType)
				||
				typeof(XpoDataView).IsAssignableFrom(collectionType)
				||
				typeof(XPInstantFeedbackSource).IsAssignableFrom(collectionType);
		}
		public override void ApplyCriteria(Object collection, CriteriaOperator criteria) {
			if(collection != null) {
				if(collection is XPBaseCollection) {
					XPBaseCollection xpCollection = (XPBaseCollection)collection;
					CriteriaOperator prevCriteria = xpCollection.Criteria;
					try {
						ITypeInfo typeInfo = TypesInfo.FindTypeInfo(xpCollection.GetObjectClassInfo().ClassType);
						xpCollection.Criteria = criteria;
						xpCollection.LoadingEnabled = !CollectionSourceBase.EmptyCollectionCriteria.Equals(criteria)
							&& typeInfo.IsPersistent;
					}
					catch {
						xpCollection.Criteria = prevCriteria;
						throw;
					}
				}
				else if(collection is XPServerCollectionSource) {
					XPServerCollectionSource xpServerCollection = (XPServerCollectionSource)collection;
					CriteriaOperator prevCriteria = xpServerCollection.FixedFilterCriteria;
					try {
						xpServerCollection.FixedFilterCriteria = criteria;
					}
					catch {
						xpServerCollection.FixedFilterCriteria = prevCriteria;
						throw;
					}
				}
				else if(collection is XpoDataView) {
					XpoDataView xpoDataView = (XpoDataView)collection;
					CriteriaOperator prevCriteria = xpoDataView.Criteria;
					try {
						xpoDataView.Criteria = criteria;
					}
					catch {
						xpoDataView.Criteria = prevCriteria;
						throw;
					}
				}
				else if(collection is XPInstantFeedbackSource) {
					XPInstantFeedbackSource xpAsyncServerModeSource = (XPInstantFeedbackSource)collection;
					CriteriaOperator prevCriteria = xpAsyncServerModeSource.FixedFilterCriteria;
					try {
						xpAsyncServerModeSource.FixedFilterCriteria = criteria;
					}
					catch {
						xpAsyncServerModeSource.FixedFilterCriteria = prevCriteria;
						throw;
					}
				}
				else if(collection is ServerModeForPersistentInterfacesWrapper) {
					ServerModeForPersistentInterfacesWrapper collectionWrapper = (ServerModeForPersistentInterfacesWrapper)collection;
					collectionWrapper.SetFixedCriteria(criteria);
				}
				else {
					Object morphed;
					if(PersistentInterfaceHelper.IsPersistentInterfaceMorpher(collection, out morphed)) {
						ApplyCriteria(morphed, criteria);
					}
					else {
						throw new CannotApplyCriteriaException(collection.GetType());
					}
				}
			}
		}
		public override Boolean CanApplyFilter(Object collection) {
			return
				(collection is XPBaseCollection)
				||
				(collection is XpoDataView);
		}
		public override void ApplyFilter(Object collection, CriteriaOperator filter) {
			if(collection != null) {
				if(collection is XPBaseCollection) {
					XPBaseCollection xpCollection = (XPBaseCollection)collection;
					CriteriaOperator prevCriteria = xpCollection.Filter;
					try {
						xpCollection.Filter = filter;
					}
					catch {
						xpCollection.Filter = prevCriteria;
						throw;
					}
				}
				else if(collection is XpoDataView) {
					XpoDataView xpoDataView = (XpoDataView)collection;
					CriteriaOperator prevCriteria = xpoDataView.Filter;
					try {
						xpoDataView.Filter = filter;
					}
					catch {
						xpoDataView.Filter = prevCriteria;
						throw;
					}
				}
				else {
					throw new CannotApplyCriteriaException(collection.GetType());
				}
			}
		}
		public override CriteriaOperator GetCriteria(Object collection) {
			if(collection is XPBaseCollection) {
				return ((XPBaseCollection)collection).GetRealFetchCriteria();
			}
			else if(collection is XPServerCollectionSource) {
				return ((XPServerCollectionSource)collection).FixedFilterCriteria;
			}
			else if(collection is XpoDataView) {
				return ((XpoDataView)collection).Criteria;
			}
			else {
				Object morphed;
				if(PersistentInterfaceHelper.IsPersistentInterfaceMorpher(collection, out morphed)) {
					return GetCriteria(morphed);
				}
			}
			return null;
		}
		public override CriteriaOperator GetFilter(Object collection) {
			if(collection is XPBaseCollection) {
				return ((XPBaseCollection)collection).Filter;
			}
			else if(collection is XpoDataView) {
				return ((XpoDataView)collection).Filter;
			}
			return null;
		}
		public override Int32 GetTopReturnedObjectsCount(Object collection) {
			Int32 topReturnedObjects = 0;
			if(collection is XPBaseCollection) {
				topReturnedObjects = ((XPBaseCollection)collection).TopReturnedObjects;
			}
			else if(collection is XpoDataView) {
				topReturnedObjects = ((XpoDataView)collection).TopReturnedObjectsCount;
			}
			else {
				Object morphed;
				if(PersistentInterfaceHelper.IsPersistentInterfaceMorpher(collection, out morphed)) {
					topReturnedObjects = GetTopReturnedObjectsCount(morphed);
				}
			}
			return topReturnedObjects;
		}
		public override void SetTopReturnedObjectsCount(Object collection, Int32 topReturnedObjects) {
			if(topReturnedObjects < 0) {
				throw new ArgumentException("The 'topReturnedObjects' argument value must be greater than or equal to 0", "topReturnedObjects");
			}
			if(collection is XPBaseCollection) {
				((XPBaseCollection)collection).TopReturnedObjects = topReturnedObjects;
			}
			else if(collection is XpoDataView) {
				((XpoDataView)collection).TopReturnedObjectsCount = topReturnedObjects;
			}
			else {
				Object morphed;
				if(PersistentInterfaceHelper.IsPersistentInterfaceMorpher(collection, out morphed)) {
					SetTopReturnedObjectsCount(morphed, topReturnedObjects);
				}
			}
		}
		public override Boolean IsObjectDeletionOnRemoveEnabled(Object collection) {
			Boolean isEnabled = false;
			if(collection is XPBaseCollection) {
				isEnabled = ((XPBaseCollection)collection).DeleteObjectOnRemove;
			}
			else {
				Object morphed;
				if(PersistentInterfaceHelper.IsPersistentInterfaceMorpher(collection, out morphed)) {
					isEnabled = IsObjectDeletionOnRemoveEnabled(morphed);
				}
			}
			return isEnabled;
		}
		public override void EnableObjectDeletionOnRemove(Object collection, Boolean enable) {
			if(collection is XPBaseCollection) {
				((XPBaseCollection)collection).DeleteObjectOnRemove = enable;
			}
			else {
				Object morphed;
				if(PersistentInterfaceHelper.IsPersistentInterfaceMorpher(collection, out morphed)) {
					EnableObjectDeletionOnRemove(morphed, enable);
				}
			}
		}
		public override IList<SortProperty> GetCollectionSorting(Object collection) {
			IList<SortProperty> result = null;
			if(collection is XPBaseCollection) {
				XPBaseCollection xpCollection = collection as XPBaseCollection;
				if(xpCollection.Sorting != null) {
					result = new List<SortProperty>(Enumerator.Convert<SortProperty>(xpCollection.Sorting));
				}
			}
			else if(collection is XPServerCollectionSource) {
				result = BaseObjectSpace.ConvertStringToSorting(((XPServerCollectionSource)collection).DefaultSorting);
			}
			else if(collection is XpoDataView) {
				return ((XpoDataView)collection).Sorting;
			}
			return result;
		}
		public override void SetCollectionSorting(Object collection, IList<SortProperty> sorting) {
			if(collection is XPBaseCollection) {
				((XPBaseCollection)collection).Sorting = CreateSortingCollection(sorting);
			}
			else if(collection is XPServerCollectionSource) {
				((XPServerCollectionSource)collection).DefaultSorting = BaseObjectSpace.ConvertSortingToString(sorting);
			}
			else if(collection is XpoDataView) {
				((XpoDataView)collection).Sorting = sorting;
			}
		}
		public override Boolean IsDeletionDeferredType(Type type) {
			return session.GetClassInfo(type).FindMember(GCRecordField.StaticName) != null;
		}
		public override Object GetObject(Object obj) {
			CheckIsDisposed();
			Object result = null;
			if(obj is XafDataViewRecord) {
				XafDataViewRecord dataViewRecord = (XafDataViewRecord)obj;
				Object keyPropertyValue = dataViewRecord[GetKeyPropertyName(dataViewRecord.ObjectType)];
				result = GetObjectByKey(dataViewRecord.ObjectType, keyPropertyValue);
			}
			else if(obj != null) {
				XPClassInfo classInfo = FindObjectXPClassInfo(obj);
				if((classInfo != null) && classInfo.IsPersistent) {
					result = GetObjectInCurrentSession(obj, classInfo);
				}
				else {
					result = obj;
				}
			}
			return result;
		}
		public override Boolean Contains(Object obj) {
			Guard.ArgumentNotNull(obj, "obj");
			CheckIsDisposed();
			if(IsDisposedObject(obj)) {
				return false;
			}
			else {
				XPClassInfo classInfo = FindObjectXPClassInfo(obj);
				if(classInfo != null && classInfo.IsPersistent) {
					if(obj is XpoDataViewRecord) {
						return ((XpoDataViewRecord)obj).DataView.ObjectSpace == this;
					}
					else if(obj is ISessionProvider) {
						return ((ISessionProvider)obj).Session == session;
					}
					else {
						return !session.IsNewObject(obj) || session.IsObjectToSave(obj) || session.IsObjectToDelete(obj);
					}
				}
				else { 
					return true;
				}
			}
		}
		public override Object FindObject(Type objectType, CriteriaOperator criteria, Boolean inTransaction) {
			CheckIsDisposed();
			Object result = null;
			XPClassInfo classInfo = FindRealXPClassInfo(objectType);
			if(classInfo == null) {
				CheckIsRegisteredType(objectType, true);
			}
			else if(classInfo.ClassType.IsInterface) {
				result = PersistentInterfaceHelper.FindObject(session, classInfo.ClassType, criteria, inTransaction);
			}
			else if(inTransaction) {
				result = session.FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, classInfo, criteria);
			}
			else {
				result = session.FindObject(classInfo, criteria);
			}
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void RemoveFromModifiedObjects(Object obj) {
			IList modifiedObjects = new ArrayList(ModifiedObjects);
			Boolean isLastModifiedObject = modifiedObjects.Count == 1 && modifiedObjects[0] == obj;
			session.RemoveFromSaveList(obj);
			if(isLastModifiedObject) {
				SetIsModified(false);
			}
		}
		public override Boolean IsObjectToSave(Object obj) {
			Guard.ArgumentNotNull(obj, "obj");
			if(!IsPersistentType(obj.GetType())) return false;
			return session.IsObjectToSave(obj);
		}
		public override Boolean IsObjectToDelete(Object obj) {
			Guard.ArgumentNotNull(obj, "obj");
			if(!IsPersistentType(obj.GetType())) return false;
			return session.IsObjectToDelete(obj);
		}
		public override Boolean IsDeletedObject(Object obj) {
			Boolean result = false;
			if(FindObjectXPClassInfo(obj) != null) {
				result = session.IsObjectMarkedDeleted(obj);
			}
			return result;
		}
		public override Boolean IsDisposedObject(Object obj) {
			if(obj is IXPInvalidateableObject) {
				return ((IXPInvalidateableObject)obj).IsInvalidated;
			}
			return false;
		}
		public override ICollection GetObjectsToSave(Boolean includeParent) {
			return session.GetObjectsToSave(includeParent);
		}
		public override ICollection GetObjectsToDelete(Boolean includeParent) {
			return session.GetObjectsToDelete(includeParent);
		}
		public override CriteriaOperator ParseCriteria(String criteria) {
			return Session.ParseCriteria(criteria);
		}
		public override IDisposable CreateParseCriteriaScope() {
			return session.CreateParseCriteriaSessionScope();
		}
		public override EvaluatorContextDescriptor GetEvaluatorContextDescriptor(Type type) {
			XPClassInfo classInfo = FindRealXPClassInfo(type);
			if(classInfo != null) {
				return classInfo.GetEvaluatorContextDescriptor();
			}
			else {
				return base.GetEvaluatorContextDescriptor(type);
			}
		}
		public override String GetDisplayableProperties(Object collection) {
			if(collection is XPBaseCollection) {
				return ((XPBaseCollection)collection).DisplayableProperties;
			}
			else if(collection is XPServerCollectionSource) {
				return ((XPServerCollectionSource)collection).DisplayableProperties;
			}
			else if(collection is XpoDataView) {
				String result = "";
				foreach(DataViewExpression expression in ((XpoDataView)collection).Expressions) {
					result = result + expression.RawName + ";";
				}
				return result.TrimEnd(';');
			}
			else if(collection is XPInstantFeedbackSource) {
				return ((XPInstantFeedbackSource)collection).DisplayableProperties;
			}
			else {
				return base.GetDisplayableProperties(collection);
			}
		}
		public override void SetDisplayableProperties(Object collection, String displayableProperties) {
			if(collection is XPBaseCollection) {
				((XPBaseCollection)collection).DisplayableProperties = displayableProperties;
			}
			else if(collection is XPServerCollectionSource) {
				((XPServerCollectionSource)collection).DisplayableProperties = displayableProperties;
			}
			else if(collection is XpoDataView) {
				((XpoDataView)collection).Expressions = ConvertExpressionsStringToExpressionsList(displayableProperties);
			}
			else if(collection is XPInstantFeedbackSource) {
				((XPInstantFeedbackSource)collection).DisplayableProperties =
					AddKeyMemberNamesToDisplayableProperties(typesInfo.FindTypeInfo(((XPInstantFeedbackSource)collection).ObjectType), displayableProperties);
			}
			else {
				base.SetDisplayableProperties(collection, displayableProperties);
			}
		}
		public void RaiseObjectPropertyChanged(Object obj, String propertyName) {
			ObjectChangeEventArgs args = new ObjectChangeEventArgs(Session, obj, ObjectChangeReason.PropertyChanged, propertyName, null, null);
			XPBaseObject.RaiseChangedEvent(obj, args);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Boolean IsIntermediateObject(Object obj) {
			return obj is IntermediateObject;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override void GetIntermediateObjectReferences(Object obj, out Object left, out Object right) {
			left = null;
			right = null;
			IntermediateObject intermediateObject = obj as IntermediateObject;
			if(intermediateObject != null) {
				left = intermediateObject.LeftIntermediateObjectField;
				right = intermediateObject.RightIntermediateObjectField;
			}
		}
		public Session Session {
			get { return session; }
		}
		public override String Database {
			get {
				String result = "";
				if(session != null) {
					IDbConnection connection = session.Connection;
					if(connection == null) {
						if(session.DataLayer is BaseDataLayer) {
							BaseDataLayer baseDataLayer = (BaseDataLayer)session.DataLayer;
							if(baseDataLayer.ConnectionProvider is ConnectionProviderSql) {
								connection = ((ConnectionProviderSql)baseDataLayer.ConnectionProvider).Connection;
							}
						}
					}
					if(connection is DbConnection) {
						result = ((DbConnection)connection).DataSource + "." + ((DbConnection)connection).Database;
					}
					else if(connection != null) {
						result = connection.Database;
					}
				}
				return result;
			}
		}
		public override Boolean IsConnected {
			get { return session.IsConnected; }
		}
		public override IList ModifiedObjects {
			get {
				CheckIsDisposed();
				ICollection sessionObjectsToSave = session.GetObjectsToSave();
				HashSet<Object> result = new HashSet<Object>(sessionObjectsToSave.Cast<Object>());
				foreach(Object obj in sessionObjectsToSave) {
					IntermediateObject intermediateObject = obj as IntermediateObject;
					if(intermediateObject != null) {
						Object intermediateObjectField = intermediateObject.LeftIntermediateObjectField;
						if(intermediateObjectField != null) {
							result.Add(intermediateObjectField);
						}
						intermediateObjectField = intermediateObject.RightIntermediateObjectField;
						if(intermediateObjectField != null) {
							result.Add(intermediateObjectField);
						}
					}
				}
				return new List<Object>(result);
			}
		}
		public override IDbConnection Connection {
			get { return session.DataLayer.Connection; }
		}
		public static CriteriaOperator CombineCriteria(params CriteriaOperator[] criteriaOperators) {
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
			if(operators.Count != 0) {
				totalCriteria = new GroupOperator(GroupOperatorType.And, operators);
			}
			return totalCriteria;
		}
		public static IObjectSpace FindObjectSpaceByObject(Object obj) {
			Object result = null;
			if(obj is ISessionProvider) {
				((IWideDataStorage)((ISessionProvider)obj).Session).TryGetWideDataItem(ObjectSpaceKey, out result);
			}
			return (IObjectSpace)result;
		}
		public static SortingCollection CreateSortingCollection(IList<SortProperty> sorting) {
			SortingCollection result = null;
			if(sorting != null) {
				result = new SortingCollection();
				for(Int32 i = 0; i < sorting.Count; i++) {
					result.Add(sorting[i]);
				}
			}
			return result;
		}
		Object IAsyncServerDataSourceCreator.Create(Type objectType, CriteriaOperator criteria) {
			return CreateServerCollection(objectType, criteria, true);
		}
		bool IDataLockingManager.IsActive {
			get { return session.TrackPropertiesModifications; }
		}
		DataLockingInfo IDataLockingManager.GetDataLockingInfo() {
			return DataLockingHelper.GetDataLockingInfo(session);
		}
		void IDataLockingManager.MergeData(DataLockingInfo dataLockingInfo) {
			DataLockingHelper.MergeData(session, dataLockingInfo);
		}
		void IDataLockingManager.RefreshData(DataLockingInfo dataLockingInfo) {
			DataLockingHelper.RefreshData(session, dataLockingInfo);
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	public class XPNestedObjectSpace : XPObjectSpace, INestedObjectSpace {
		private IObjectSpace parentObjectSpace;
		private Object GetObjectInCurrentSessionFromParentSession(Object obj) {
			Object result = null;
			Object parentObject = parentObjectSpace.GetObject(obj);
			if(parentObject != null) {
				result = ((NestedUnitOfWork)Session).GetNestedObject(parentObject);
			}
			return result;
		}
		protected override Object GetObjectInCurrentSession(Object obj, XPClassInfo classInfo) {
			Object result = GetObjectInCurrentSessionFromNestedSession(obj);
			if(result == null) {
				result = GetObjectInCurrentSessionFromParentSession(obj);
			}
			return result;
		}
		protected override Boolean IsNewObjectCore(Object obj) {
			if(obj is XpoDataViewRecord) {
				return false;
			}
			else {
				return (((NestedUnitOfWork)session).GetParentObject(obj) == null);
			}
		}
		protected override void CheckLocking() { }
		protected override UnitOfWork RecreateUnitOfWork() {
			return ((XPObjectSpace)parentObjectSpace).Session.BeginNestedUnitOfWork();
		}
		protected override void DoCommit() {
			Int32 deletedObjectsCount = Session.GetObjectsToDelete().Count;
			ICollection modifiedObjects = new ArrayList(Session.GetObjectsToSave());
			base.DoCommit();
			foreach(Object obj in modifiedObjects) {
				Object parentObject = parentObjectSpace.GetObject(obj);
				if(parentObject != null) {
					parentObjectSpace.SetModified(parentObject);
				}
			}
			if(modifiedObjects.Count > 0 || deletedObjectsCount > 0) {
				parentObjectSpace.SetModified(null);
			}
		}
		protected override void SetSession(UnitOfWork newSession) {
			base.SetSession(newSession);
			if(newSession != null) {
				newSession.LockingOption = LockingOption.None;
			}
		}
		public XPNestedObjectSpace(IObjectSpace parentObjectSpace)
			: base((TypesInfo)parentObjectSpace.TypesInfo, ((XPObjectSpace)parentObjectSpace).xpoTypeInfoSource, () => { return ((XPObjectSpace)parentObjectSpace).Session.BeginNestedUnitOfWork(); }) {
			this.parentObjectSpace = parentObjectSpace;
		}
		public IObjectSpace ParentObjectSpace {
			get { return parentObjectSpace; }
		}
		public override void Dispose() {
			parentObjectSpace = null;
			base.Dispose();
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public abstract class PersistentInterfaceHelper {
		private class InnerHelper<T> : PersistentInterfaceHelper {
			protected override object CreatePersistentInterfaceMorpher(Session session, CriteriaOperator criteria, SortingCollection sorting, bool inTransaction) {
				XPCollection<IPersistentInterfaceData<T>> persistentInterfaceDataCollection;
				if(inTransaction) {
					persistentInterfaceDataCollection = new XPCollection<IPersistentInterfaceData<T>>(PersistentCriteriaEvaluationBehavior.InTransaction, session, criteria);
				}
				else {
					persistentInterfaceDataCollection = new XPCollection<IPersistentInterfaceData<T>>(session, criteria);
					persistentInterfaceDataCollection.LoadingEnabled = !CollectionSourceBase.EmptyCollectionCriteria.Equals(criteria);
				}
				persistentInterfaceDataCollection.Sorting = sorting;
				return new PersistentInterfaceMorpher<T>(persistentInterfaceDataCollection);
			}
			protected override object GetMorphed(object obj) {
				PersistentInterfaceMorpher<T> morpher = obj as PersistentInterfaceMorpher<T>;
				if(morpher == null) {
					return null;
				}
				return morpher.Morphed;
			}
			protected override object FindObject(Session session, CriteriaOperator criteria, bool inTransaction) {
				Type dataType = typeof(IPersistentInterfaceData<T>);
				PersistentCriteriaEvaluationBehavior evaluationBehavior = PersistentCriteriaEvaluationBehavior.BeforeTransaction;
				if(inTransaction) {
					evaluationBehavior = PersistentCriteriaEvaluationBehavior.InTransaction;
				}
				IPersistentInterfaceData<T> dataObject = session.FindObject<IPersistentInterfaceData<T>>(evaluationBehavior, criteria);
				if(dataObject == null) {
					return null;
				}
				return dataObject.Instance;
			}
			protected override object GetInterfaceInstance(object dataObject) {
				IPersistentInterfaceData<T> data = dataObject as IPersistentInterfaceData<T>;
				if(data == null) {
					return null;
				}
				return data.Instance;
			}
		}
		private static void CheckInterfaceType(Type interfaceType) {
			Guard.ArgumentNotNull(interfaceType, "interfaceType");
			if(!interfaceType.IsInterface) {
				throw new ArgumentException(String.Format("The {0} type is not an interface.", interfaceType.FullName), "interfaceType");
			}
		}
		protected abstract object CreatePersistentInterfaceMorpher(Session session, CriteriaOperator criteria, SortingCollection sorting, bool inTransaction);
		protected abstract object GetMorphed(object morpher);
		protected abstract object FindObject(Session session, CriteriaOperator criteria, bool inTransaction);
		protected abstract object GetInterfaceInstance(object dataObject);
		protected static PersistentInterfaceHelper Create(Type persistentInterfaceType) {
			CheckInterfaceType(persistentInterfaceType);
			Type helperType = typeof(InnerHelper<>).MakeGenericType(new Type[] { persistentInterfaceType });
			return (PersistentInterfaceHelper)Activator.CreateInstance(helperType);
		}
		protected static PersistentInterfaceHelper Create<T>() {
			CheckInterfaceType(typeof(T));
			return new InnerHelper<T>();
		}
		public static Type GetPersistentInterfaceDataType(Type interfaceType) {
			CheckInterfaceType(interfaceType);
			return typeof(IPersistentInterfaceData<>).MakeGenericType(new Type[] { interfaceType });
		}
		public static IList CreateCollection(Session session, Type persistentInterfaceType, CriteriaOperator criteria, SortingCollection sorting, bool inTransaction) {
			PersistentInterfaceHelper helper = PersistentInterfaceHelper.Create(persistentInterfaceType);
			return (IList)helper.CreatePersistentInterfaceMorpher(session, criteria, sorting, inTransaction);
		}
		public static IList<T> CreateCollection<T>(Session session, CriteriaOperator criteria, SortingCollection sorting, bool inTransaction) {
			PersistentInterfaceHelper helper = PersistentInterfaceHelper.Create<T>();
			return (IList<T>)helper.CreatePersistentInterfaceMorpher(session, criteria, sorting, inTransaction);
		}
		public static bool IsPersistentInterfaceMorpher(object collection, out object morphed) {
			morphed = null;
			if(collection == null) {
				return false;
			}
			Type collectionType = collection.GetType();
			if(!collectionType.IsGenericType || (collectionType.GetGenericTypeDefinition() != typeof(PersistentInterfaceMorpher<>))) {
				return false;
			}
			Type persistentInterfaceType = collectionType.GetGenericArguments()[0];
			PersistentInterfaceHelper helper = PersistentInterfaceHelper.Create(persistentInterfaceType);
			morphed = helper.GetMorphed(collection);
			return true;
		}
		public static object FindObject(Session session, Type persistentInterfaceType, CriteriaOperator criteria, bool inTransaction) {
			PersistentInterfaceHelper helper = PersistentInterfaceHelper.Create(persistentInterfaceType);
			return helper.FindObject(session, criteria, inTransaction);
		}
		public static object GetInterfaceInstance(Type interfaceType, object dataObject) {
			PersistentInterfaceHelper helper = PersistentInterfaceHelper.Create(interfaceType);
			return helper.GetInterfaceInstance(dataObject);
		}
	}
}
