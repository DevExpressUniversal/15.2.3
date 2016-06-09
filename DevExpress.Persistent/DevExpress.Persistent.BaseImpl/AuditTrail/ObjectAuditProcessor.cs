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
using DevExpress.ExpressApp.DC.ClassGeneration;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
namespace DevExpress.Persistent.AuditTrail {
	public enum AuditOperationType { ObjectCreated = 1, InitialValueAssigned = 2, ObjectChanged = 3, ObjectDeleted = 4, AddedToCollection = 5, RemovedFromCollection = 6, CollectionObjectChanged = 7, AggregatedObjectChanged = 8, CustomData = 9 };
	public class ObjectValue {
		private object propertyValue;
		public object PropertyValue {
			get { return propertyValue; }
			set { propertyValue = value; }
		}
		private XPMemberInfo memberInfo;
		public XPMemberInfo MemberInfo {
			get { return memberInfo; }
			set { memberInfo = value; }
		}
	}
	public class DiffCollection : Dictionary<object, List<AuditDataItem>> {
		public const string NullObject = "NullObject";
		private List<AuditDataItem> GetObjectValueDiffs(object obj) {
			List<AuditDataItem> objectValueDiffs;
			if(!TryGetValue(obj, out objectValueDiffs)) {
				objectValueDiffs = new List<AuditDataItem>();
				Add(obj, objectValueDiffs);
			}
			return objectValueDiffs;
		}
		public void Add(AuditDataItem objectValueDiff) {
			object obj = objectValueDiff.AuditObject;
			if(obj == null) {
				obj = NullObject;
			}
			List<AuditDataItem> objectValueDiffs = GetObjectValueDiffs(obj);
			objectValueDiffs.Add(objectValueDiff);
		}
		public void AddRange(object obj, List<AuditDataItem> list) {
			if(obj == null) {
				obj = NullObject;
			}
			List<AuditDataItem> objectValueDiffs = GetObjectValueDiffs(obj);
			objectValueDiffs.AddRange(list);
		}
		public void MergeDiffs(DiffCollection diffsToMerge) {
			foreach(object obj in diffsToMerge.Keys) {
				AddRange(obj, diffsToMerge[obj]);
			}
		}
		public List<AuditDataItem> All() {
			List<AuditDataItem> result = new List<AuditDataItem>();
			foreach(List<AuditDataItem> list in Values) {
				result.AddRange(list);
			}
			return result;
		}
	}
	public interface IObjectAuditProcessorsFactory {
		bool IsSuitableAuditProcessor(ObjectAuditProcessor processor, ObjectAuditingMode mode);
		ObjectAuditProcessor CreateAuditProcessor(ObjectAuditingMode mode, Session session, AuditTrailSettings settings);
	}
	public class ObjectAuditProcessorsFactory : IObjectAuditProcessorsFactory {
		private Dictionary<ObjectAuditingMode, Type> processorTypes;
		public ObjectAuditProcessorsFactory() {
			processorTypes = new Dictionary<ObjectAuditingMode, Type>();
			processorTypes.Add(ObjectAuditingMode.Full, typeof(FullObjectAuditProcessor));
			processorTypes.Add(ObjectAuditingMode.Lightweight, typeof(LightweightObjectAuditProcessor));
			processorTypes.Add(ObjectAuditingMode.CreationOnly, typeof(CreationOnlyObjectAuditProcessor));
		}
		public virtual bool IsSuitableAuditProcessor(ObjectAuditProcessor processor, ObjectAuditingMode mode) {
			return processor.GetType() == processorTypes[mode];
		}
		public virtual ObjectAuditProcessor CreateAuditProcessor(ObjectAuditingMode mode, Session session, AuditTrailSettings settings) {
			return (ObjectAuditProcessor)TypeHelper.CreateInstance(processorTypes[mode], session, settings);
		}
	}
	public abstract class ObjectAuditProcessor : IDisposable {
		public class ListEventArgs : EventArgs {  
			public ArrayList List;
		}
		private Session session;
		protected bool isNestedUoWCommitting = false;
		private AuditTrailSettings settings;
		protected bool isPropertyChangesAuditEnabled = true;
		private DiffCollection customAuditDataItems = new DiffCollection();
		private DiffCollection newAndDeletedObjects = new DiffCollection();
		protected List<object> newObjects = new List<object>();
		protected List<object> deletedObjects = new List<object>();
		private AuditTrailStrategy strategy;
		private bool isCommiting;
		private Dictionary<XPBaseCollection, Object> subscribedCollections;
		private List<System.Collections.Specialized.INotifyCollectionChanged> notifyCollectionsChanged;
		private void session_BeforeCommitNestedUnitOfWork(object sender, SessionManipulationEventArgs e) {
			if(IsDisposed) {
				return; 
			}
			foreach(object obj in e.Session.GetObjectsToSave()) {
				if(!e.Session.IsNewObject(obj) && IsObjectToAudit(obj)) {
					object currentSessionObject = session.GetObjectByKey(e.Session.GetClassInfo(obj), e.Session.GetKeyValue(obj));
					if(currentSessionObject != null) {
						BeginObjectAudit(currentSessionObject);
					}
				}
			}
			isNestedUoWCommitting = true;
		}
		private void session_AfterCommitNestedUnitOfWork(object sender, SessionManipulationEventArgs e) {
			if(IsDisposed) {
				return; 
			}
			isNestedUoWCommitting = false;
		}
		private bool IsDeletionDeferredObject(object obj) {
			return session.GetClassInfo(obj).FindMember(GCRecordField.StaticName) != null;
		}
		private void session_ObjectLoading(object sender, ObjectManipulationEventArgs e) {
			if(isNestedUoWCommitting || IsDisposed)
				return;
			if(IsObjectToAudit(e.Object)) {
				ClearObjectPropertiesAuditInformation(e.Object);
			}
		}
		private void session_ObjectLoaded(object sender, ObjectManipulationEventArgs e) {
			if(isNestedUoWCommitting || IsDisposed)
				return;
			BeginObjectAudit(e.Object);
		}
		private void session_ObjectChanged(object sender, ObjectChangeEventArgs e) {
			if(IsDisposed)
				return;
			if((!isNestedUoWCommitting || string.IsNullOrEmpty(e.PropertyName)) && IsObjectToAudit(sender)) {
				bool isFirstTimeAuditedModification = !IsObjectAudited(sender);
				if(isFirstTimeAuditedModification) {
					BeginObjectAudit(sender);
				}
				ProcessAuditedObjectChange(sender, e, isFirstTimeAuditedModification);
			}
		}
		private void session_BeforeCommitTransaction(object sender, SessionManipulationEventArgs e) {
			if(IsDisposed) {
				return; 
			}
			CollectAffectedObjects();
			isCommiting = true;
		}
		private void SubscribeAllEvents() {
			session.ObjectLoading += new ObjectManipulationEventHandler(session_ObjectLoading);
			if(strategy == AuditTrailStrategy.OnObjectLoaded) {
				session.ObjectLoaded += new ObjectManipulationEventHandler(session_ObjectLoaded);
			}
			session.ObjectChanged += new ObjectChangeEventHandler(session_ObjectChanged);
			session.BeforeCommitTransaction += new SessionManipulationEventHandler(session_BeforeCommitTransaction);
			session.BeforeCommitNestedUnitOfWork += new SessionManipulationEventHandler(session_BeforeCommitNestedUnitOfWork);
			session.AfterCommitNestedUnitOfWork += new SessionManipulationEventHandler(session_AfterCommitNestedUnitOfWork);
		}
		private void UnsubscribeAllEvents() {
			UnsubscribeEventsCore();
			session.ObjectLoading -= new ObjectManipulationEventHandler(session_ObjectLoading);
			session.ObjectLoaded -= new ObjectManipulationEventHandler(session_ObjectLoaded);
			session.ObjectChanged -= new ObjectChangeEventHandler(session_ObjectChanged);
			session.BeforeCommitTransaction -= new SessionManipulationEventHandler(session_BeforeCommitTransaction);
			session.BeforeCommitNestedUnitOfWork -= new SessionManipulationEventHandler(session_BeforeCommitNestedUnitOfWork);
			session.AfterCommitNestedUnitOfWork -= new SessionManipulationEventHandler(session_AfterCommitNestedUnitOfWork);
		}
		private void CollectNewAndDeletedObjects() {
			newAndDeletedObjects.Clear();
			newObjects.Clear();
			deletedObjects.Clear();
			CollectNewAndDeletedObjects(newAndDeletedObjects);
		}
		private void CollectNewAndDeletedObjects(DiffCollection allDiffs) {
			foreach(Object obj in GetAuditedObjects()) {
				if(obj != null && !DiffCollection.NullObject.Equals(obj) && !IsObjectToSave(obj) && !IsObjectToDelete(obj) && session.IsNewObject(obj)) {
					ClearObjectPropertiesAuditInformation(obj);
				}
			}
			DiffCollection result = new DiffCollection();
			ArrayList notSavedObjectsToDelete = new ArrayList();
			ICollection sessionObjectsToSave = Session.GetObjectsToSave();
			foreach(object obj in sessionObjectsToSave) {
				if(IsObjectToAudit(obj)) {
					if(Session.IsNewObject(obj)) {
						if(IsObjectDeleted(obj)) {
							notSavedObjectsToDelete.Add(obj);
							ClearObjectPropertiesAuditInformation(obj);
						}
						else {
							result.Add(new AuditDataItem(obj, null, null, null, AuditOperationType.ObjectCreated));
							newObjects.Add(obj);
						}
					}
					else if(IsObjectDeleted(obj)) {
						deletedObjects.Add(obj);
						result.Add(new AuditDataItem(obj, null, null, null, AuditOperationType.ObjectDeleted));
					}
				}
			}
			ICollection sessionObjectsToDelete = Session.GetObjectsToDelete();
			ArrayList objectsToDelete = new ArrayList(sessionObjectsToDelete);
			foreach(object obj in objectsToDelete) {
				if(IsObjectToAudit(obj)) {
					if(notSavedObjectsToDelete.IndexOf(obj) == -1) {
						deletedObjects.Add(obj);
						result.Add(new AuditDataItem(obj, null, null, null, AuditOperationType.ObjectDeleted));
					}
				}
			}
			allDiffs.MergeDiffs(result);
		}
		private bool IsObjectToDelete(Object obj) {
			if(CustomGetSessionObjectsToDelete != null) {
				ListEventArgs args = new ListEventArgs();
				CustomGetSessionObjectsToDelete(this, args);
				return args.List.Contains(obj);
			}
			return Session.IsObjectToDelete(obj);
		}
		private bool IsObjectToSave(Object obj) {
			if(CustomGetSessionObjectsToSave != null) {
				ListEventArgs args = new ListEventArgs();
				CustomGetSessionObjectsToSave(this, args);
				return args.List.Contains(obj);
			}
			return Session.IsObjectToSave(obj);
		}
		private ICollection GetSessionObjectsToDelete() {
			if(CustomGetSessionObjectsToDelete != null) {
				ListEventArgs args = new ListEventArgs();
				CustomGetSessionObjectsToDelete(this, args);
				return args.List;
			}
			return Session.GetObjectsToDelete();
		}
		private ICollection GetSessionObjectsToSave() {
			if(CustomGetSessionObjectsToSave != null) {
				ListEventArgs args = new ListEventArgs();
				CustomGetSessionObjectsToSave(this, args);
				return args.List;
			}
			return Session.GetObjectsToSave();
		}
		private bool IsIntermediateObjectType(Type type) {
			return type != null && (typeof(IntermediateObject).IsAssignableFrom(type) || typeof(IDCIntermediateObject).IsAssignableFrom(type));
		}
		protected List<AuditTrailMemberInfo> GetPropertiesToAudit(Type type) {
			if(IsIntermediateObjectType(type)) {
				return new List<AuditTrailMemberInfo>();
			}
			AuditTrailClassInfo info = Settings.FindAuditTrailClassInfo(type);
			return info.properties;
		}
		private DiffCollection ProcessNewAndDeletedObjects(DiffCollection newAndDeletedObjects) {
			DiffCollection result = new DiffCollection();
			foreach(object obj in newAndDeletedObjects.Keys) {
				result.MergeDiffs(ResolveManyToManyDiffs(obj, newAndDeletedObjects[obj]));
			}
			return result;
		}
		protected virtual DiffCollection ResolveManyToManyDiffsForIntermediateObject(IntermediateObject intermediateObject, List<AuditDataItem> listToResolve) {
			return new DiffCollection();
		}
		protected virtual DiffCollection ResolveManyToManyDiffsForIntermediateObject(IDCIntermediateObject intermediateObject, List<AuditDataItem> listToResolve) {
			return new DiffCollection();
		}
		private DiffCollection ResolveManyToManyDiffs(object changedObject, List<AuditDataItem> listToResolve) {
			DiffCollection result = new DiffCollection();
			IntermediateObject intermediateObject = changedObject as IntermediateObject;
			if(intermediateObject == null) {
				IDCIntermediateObject dcIntermediateObject = changedObject as IDCIntermediateObject;
				if(dcIntermediateObject == null) {
					result.Add(changedObject, listToResolve);
				}
				else {
					result.MergeDiffs(ResolveManyToManyDiffsForIntermediateObject(dcIntermediateObject, listToResolve));
				}
			}
			else {
				result.MergeDiffs(ResolveManyToManyDiffsForIntermediateObject(intermediateObject, listToResolve));
			}
			return result;
		}
		private void collection_ListChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.Reset) {
				foreach(object collectionElement in ((IBindingList)sender)) {
					BeginObjectAudit(collectionElement);
				}
			}
		}
		private void collection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			switch(e.Action) {
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					foreach(object item in e.NewItems) {
						BeginObjectAudit(item);
					}
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
				case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
					if(e.OldItems != null) {
						foreach(object item in e.OldItems) {
							BeginObjectAudit(item);
						}
					}
					break;
			}
		}
		private void collection_CollectionChanged(object sender, XPCollectionChangedEventArgs e) {
			switch(e.CollectionChangedType) {
				case XPCollectionChangedType.BeforeAdd:
				case XPCollectionChangedType.BeforeRemove:
					BeginObjectAudit(e.ChangedObject);
					break;
			}
		}
		protected void SubscribeCollectionChanged(object collection) {
			if(collection is XPBaseCollection) {
				SubscribeCollectionChanged((XPBaseCollection)collection);
			}
			else {
				if(collection is DevExpress.Xpo.Helpers.XPAssociationList) {
					SubscribeCollectionChanged((DevExpress.Xpo.Helpers.XPAssociationList)collection);
				}
			}
		}
		protected void SubscribeCollectionChanged(DevExpress.Xpo.Helpers.XPAssociationList collection) {
			if(!notifyCollectionsChanged.Contains(collection)) {
				collection.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collection_CollectionChanged);
				notifyCollectionsChanged.Add(collection);
				if(collection.IsLoaded) {
					foreach(object collectionObj in collection) {
						BeginObjectAudit(collectionObj);
					}
				}
			}
		}
		protected void SubscribeCollectionChanged(XPBaseCollection collection) {
			if(!subscribedCollections.ContainsKey(collection)) {
				collection.CollectionChanged += new XPCollectionChangedEventHandler(collection_CollectionChanged);
				collection.ListChanged += new ListChangedEventHandler(collection_ListChanged);
				collection.Disposed += new EventHandler(collection_Disposed);
				subscribedCollections.Add(collection, null);
				if(collection.IsLoaded) {
					foreach(object collectionObj in collection) {
						BeginObjectAudit(collectionObj);
					}
				}
			}
		}
		private void collection_Disposed(object sender, EventArgs e) {
			XPBaseCollection collection = (XPBaseCollection)sender;
			collection.CollectionChanged -= new XPCollectionChangedEventHandler(collection_CollectionChanged);
			collection.ListChanged -= new ListChangedEventHandler(collection_ListChanged);
			collection.Disposed -= new EventHandler(collection_Disposed);
			if(subscribedCollections.ContainsKey(collection)) {
				subscribedCollections.Remove(collection);
			}
		}
		protected void UnsubscribeCollectionChanged() {
			foreach(XPBaseCollection collection in subscribedCollections.Keys) {
				collection.CollectionChanged -= new XPCollectionChangedEventHandler(collection_CollectionChanged);
				collection.ListChanged -= new ListChangedEventHandler(collection_ListChanged);
				collection.Disposed -= new EventHandler(collection_Disposed);
			}
			subscribedCollections.Clear();
			foreach(System.Collections.Specialized.INotifyCollectionChanged collection in notifyCollectionsChanged) {
				collection.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(collection_CollectionChanged);
			}
			notifyCollectionsChanged.Clear();
		}
		protected Dictionary<XPClassInfo, List<XPMemberInfo>> CollectReferencingObjects(object target) {
			if(target == null)
				throw new ArgumentNullException("target");
			XPClassInfo ci = Session.GetClassInfo(target);
			Dictionary<XPClassInfo, List<XPMemberInfo>> referenceMembersByMappingClasses = new Dictionary<XPClassInfo, List<XPMemberInfo>>();
			foreach(XPClassInfo possibleReferrer in Session.Dictionary.Classes) {
				if(!possibleReferrer.IsPersistent)
					continue;
				foreach(XPMemberInfo mi in possibleReferrer.ObjectProperties) {
					if(!ci.IsAssignableTo(mi.ReferenceType))
						continue;
					XPClassInfo referenceMappingClass = mi.GetMappingClass(possibleReferrer);
					List<XPMemberInfo> references;
					if(!referenceMembersByMappingClasses.TryGetValue(referenceMappingClass, out references)) {
						references = new List<XPMemberInfo>();
						referenceMembersByMappingClasses.Add(referenceMappingClass, references);
					}
					if(!references.Contains(mi))
						references.Add(mi);
				}
			}
			return referenceMembersByMappingClasses;
		}
		protected virtual void ClearObjectPropertiesAuditInformation(object obj) { }
		protected virtual IList GetAuditedObjects() { return new ArrayList(); }
		protected virtual void FillDiffsCore(DiffCollection diffs) { }
		protected virtual void ClearDiffsCore() { }
		protected virtual void UnsubscribeEventsCore() {
			UnsubscribeCollectionChanged();
		}
		protected virtual void ProcessAuditedObjectChange(object sender, ObjectChangeEventArgs e, bool isFirstTimeAuditedModification) { }
		protected virtual void BeginObjectAuditCore(object obj, List<AuditTrailMemberInfo> list) { }
		protected virtual void CollectAffectedObjects() {
			CollectNewAndDeletedObjects();
		}
		protected bool IsObjectDeleted(object obj) {
			if(IsDeletionDeferredObject(obj)) {
				return Session.IsObjectMarkedDeleted(obj);
			}
			else {
				return Session.IsObjectToDelete(obj);
			}
		}
		protected bool IsValueMember(object obj, XPMemberInfo xpMemberInfo) {
			if(xpMemberInfo.IsDelayed) {
				DelayedAttribute delayedAttribute = xpMemberInfo.GetAttributeInfo(typeof(DelayedAttribute)) as DelayedAttribute;
				if(delayedAttribute.FieldName != null) {
					XPMemberInfo delayedMemberInfo = xpMemberInfo.Owner.FindMember(delayedAttribute.FieldName);
					if(delayedMemberInfo != null) {
						XPDelayedProperty delayedMember = delayedMemberInfo.GetValue(obj) as XPDelayedProperty;
						return delayedMember != null ? delayedMember.IsLoaded : false;
					}
				}
				return false;
			}
			return true;
		}
		protected virtual bool IsObjectToAudit(object obj) {
			IXPInvalidateableObject dispObj = obj as IXPInvalidateableObject;
			bool isDisposed = (dispObj != null) && dispObj.IsInvalidated;
			return !isDisposed && (settings.IsTypeToAudit(obj.GetType()) || IsIntermediateObjectType(obj.GetType()));
		}
		public ObjectAuditProcessor(Session session, AuditTrailSettings settings) {
			Guard.ArgumentNotNull(settings, "settings");
			Guard.ArgumentNotNull(session, "session");
			this.settings = settings;
			this.session = session;
			SubscribeAllEvents();
			Strategy = AuditTrailStrategy.OnObjectLoaded;
			subscribedCollections = new Dictionary<XPBaseCollection, Object>();
			notifyCollectionsChanged = new List<System.Collections.Specialized.INotifyCollectionChanged>();
		}
		public abstract bool IsObjectAudited(object obj);
		public void EndObjectAudit(object obj) {
			if(IsDisposed) {
				return; 
			}
			ClearObjectPropertiesAuditInformation(obj);
		}
		public DiffCollection GetAllDiffs() {
			if(IsDisposed) {
				return new DiffCollection();
			}
			UnsubscribeAllEvents();
			if(!isCommiting) {
				CollectAffectedObjects();
			}
			DiffCollection allDiffs = new DiffCollection();
			FillDiffsCore(allDiffs);
			allDiffs.MergeDiffs(ProcessNewAndDeletedObjects(newAndDeletedObjects));
			allDiffs.MergeDiffs(customAuditDataItems);
			SubscribeAllEvents();
			return allDiffs;
		}
		public void BeginObjectAudit(object obj) {
			if(IsDisposed) {
				return;
			}
			Guard.ArgumentNotNull(obj, "obj");
			if(IsObjectToAudit(obj) && !IsObjectAudited(obj)) {
				BeginObjectAuditCore(obj, GetPropertiesToAudit(obj.GetType()));
			}
		}
		public void Dispose() {
			IsDisposed = true;
			UnsubscribeAllEvents();
			session = null;
		}
		public void ClearDiffs() {
			if(IsDisposed) {
				return; 
			}
			customAuditDataItems.Clear();
			newAndDeletedObjects.Clear();
			newObjects.Clear();
			deletedObjects.Clear();
			ClearDiffsCore();
		}
		public void AddAuditDataItem(AuditDataItem auditDataItem) {
			if(IsDisposed) {
				return; 
			}
			customAuditDataItems.Add(auditDataItem);
		}
		public AuditTrailStrategy Strategy {
			get { return strategy; }
			set {
				strategy = value;
				session.ObjectLoaded -= new ObjectManipulationEventHandler(session_ObjectLoaded);
				if(strategy == AuditTrailStrategy.OnObjectLoaded) {
					session.ObjectLoaded += new ObjectManipulationEventHandler(session_ObjectLoaded);
				}
			}
		}
		[DefaultValue(false)]
		public bool IsDisposed { get; private set; }
		public Session Session {
			get { return session; }
		}
		public AuditTrailSettings Settings {
			get { return settings; }
		}
		[Obsolete("This property is for internal use only.")]
		public bool IsPropertyChangesAuditEnabled {
			get { return isPropertyChangesAuditEnabled; }
			set { isPropertyChangesAuditEnabled = value; }
		}
		[Browsable(false)]
		public event EventHandler<ListEventArgs> CustomGetSessionObjectsToSave; 
		[Browsable(false)]
		public event EventHandler<ListEventArgs> CustomGetSessionObjectsToDelete; 
	}
	public class CreationOnlyObjectAuditProcessor : ObjectAuditProcessor {
		private ArrayList auditedObjects = new ArrayList();
		protected override void BeginObjectAuditCore(object obj, List<AuditTrailMemberInfo> list) {
			if(!auditedObjects.Contains(obj)) {
				auditedObjects.Add(obj);
			}
		}
		public CreationOnlyObjectAuditProcessor(Session session, AuditTrailSettings settings)
			: base(session, settings) {
		}
		public override bool IsObjectAudited(object obj) {
			return auditedObjects.Contains(obj);
		}
	}
	public class LightweightObjectAuditProcessor : ObjectAuditProcessor {
		private Dictionary<object, bool> auditedObjects = new Dictionary<object, bool>();
		private Dictionary<object, IList<string>> objectsAuditableProperties = new Dictionary<object, IList<string>>();
		protected override void BeginObjectAuditCore(object obj, List<AuditTrailMemberInfo> list) {
			if(!auditedObjects.ContainsKey(obj)) {
				auditedObjects.Add(obj, false);
				objectsAuditableProperties.Add(obj, new List<string>());
				foreach(AuditTrailMemberInfo memberInfo in list) {
					objectsAuditableProperties[obj].Add(memberInfo.Name);
					if((memberInfo.MemberInfo != null) && memberInfo.MemberInfo.IsCollection) {
						SubscribeCollectionChanged(memberInfo.MemberInfo.GetValue(obj));
					}
				}
			}
		}
		protected override void FillDiffsCore(DiffCollection diffs) {
			DiffCollection additionalDiffs = new DiffCollection();
			foreach(object obj in auditedObjects.Keys) {
				if(auditedObjects[obj]) {
					additionalDiffs.Add(new AuditDataItem(obj, null, null, null, AuditOperationType.ObjectChanged));
				}
			}
			diffs.MergeDiffs(additionalDiffs);
		}
		protected override void ProcessAuditedObjectChange(object sender, ObjectChangeEventArgs e, bool isFirstTimeAuditedModification) {
			if(!auditedObjects[e.Object] && (objectsAuditableProperties[e.Object].Contains(e.PropertyName) || (string.IsNullOrEmpty(e.PropertyName) && isNestedUoWCommitting))) {
				auditedObjects[e.Object] = true;
			}
		}
		protected override IList GetAuditedObjects() {
			return new ArrayList(auditedObjects.Keys);
		}
		protected override void ClearObjectPropertiesAuditInformation(object obj) {
			auditedObjects.Remove(obj);
			objectsAuditableProperties.Remove(obj);
		}
		public LightweightObjectAuditProcessor(Session session, AuditTrailSettings settings) : base(session, settings) { }
		public override bool IsObjectAudited(object obj) {
			return auditedObjects.ContainsKey(obj);
		}
	}
	public class FullObjectAuditProcessor : ObjectAuditProcessor {
		private Dictionary<object, List<ObjectValue>> oldObjectPropertiesValues = new Dictionary<object, List<ObjectValue>>();
		private void FillDiffList(List<ObjectValue> oldList, object newObject, DiffCollection diffToAddTo) {
			if((newObject is IntermediateObject))
				return;
			foreach(ObjectValue oldValue in oldList) {
				object newValue = oldValue.MemberInfo.GetValue(newObject);
				if(!Object.Equals(oldValue.PropertyValue, newValue)) {
					AuditOperationType auditOperationType;
					if(!deletedObjects.Contains(newObject)) {
						if(newObjects.Contains(newObject)) {
							auditOperationType = AuditOperationType.InitialValueAssigned;
						}
						else {
							auditOperationType = AuditOperationType.ObjectChanged;
						}
						diffToAddTo.Add(new AuditDataItem(newObject, oldValue.MemberInfo, oldValue.PropertyValue, newValue, auditOperationType));
					}
					if(oldValue.MemberInfo.IsAssociation) {
						ForceAssociatedObjectsChange(newObject, oldValue, newValue, diffToAddTo);
					}
					ForceParentObjectChange(newObject, diffToAddTo);
				}
			}
		}
		private bool IsCollection(XPMemberInfo memberInfo) {
			return memberInfo.IsCollection  || memberInfo.IsAssociationList;
		}
		private List<ObjectValue> GetInitialPropertiesValues(object obj, List<AuditTrailMemberInfo> membersInfo) {
			List<ObjectValue> valuesList = new List<ObjectValue>();
			if(isPropertyChangesAuditEnabled) {
				foreach(AuditTrailMemberInfo mi in membersInfo) {
					if(mi.MemberInfo == null) {
						continue;
					}
					if(!IsCollection(mi.MemberInfo)) {
						if(IsValueMember(obj, mi.MemberInfo)) {
							ObjectValue value = new ObjectValue();
							valuesList.Add(value);
							value.MemberInfo = mi.MemberInfo;
							value.PropertyValue = null;
							if(!Session.IsNewObject(obj)) {
								try {
									value.PropertyValue = mi.MemberInfo.GetValue(obj);
								}
								catch(Exception) { }
							}
						}
					}
					else {
						SubscribeCollectionChanged(mi.MemberInfo.GetValue(obj));
					}
				}
			}
			return valuesList;
		}
		private void AddCollectionChangeAuditDataItem(object collectionObject, AuditOperationType operationType, IXPSimpleObject owner, XPMemberInfo parentMemberInfo, DiffCollection diffToAddTo) {
			diffToAddTo.Add(new AuditDataItem(owner, parentMemberInfo, collectionObject, null, operationType));
			ForceParentObjectChange(owner, diffToAddTo);
		}
		private void AddDiffOfAssociatedCollectionChange(object changedObject, object collectionObject, XPMemberInfo collectionMemberInfo, AuditOperationType operationType, DiffCollection diffToAddTo) {
			object associatedCollection = null;
			if(changedObject != null) {
				associatedCollection = collectionMemberInfo.GetValue(changedObject) as XPBaseCollection;
				if(associatedCollection == null) {
					associatedCollection = collectionMemberInfo.GetValue(changedObject) as DevExpress.Xpo.Helpers.XPAssociationList;
				}
			}
			if(associatedCollection != null) {
				diffToAddTo.Add(new AuditDataItem(changedObject, collectionMemberInfo, collectionObject, null, operationType));
			}
		}
		private void ForceParentChangeForAggregated(object aggregatedObject, DiffCollection allDiffs) {
			Dictionary<XPClassInfo, List<XPMemberInfo>> classes = CollectReferencingObjects(aggregatedObject);
			foreach(XPClassInfo key in classes.Keys) {
				List<XPMemberInfo> members = classes[key];
				foreach(XPMemberInfo memberInfo in members) {
					if(memberInfo.IsAggregated) {
						InOperator criteria = new InOperator(new OperandValue(aggregatedObject), new OperandProperty(memberInfo.Name));
						ICollection referencedObjects = Session.GetObjects(key, criteria, null, 0, 0, false, false);
						if(referencedObjects.Count > 0) {
							foreach(object referencedObject in referencedObjects) {
								if(!IsParentChangeAllreadyInDiffs(allDiffs, referencedObject, memberInfo, aggregatedObject, AuditOperationType.AggregatedObjectChanged)) {
									allDiffs.Add(new AuditDataItem(referencedObject, memberInfo, aggregatedObject, null, AuditOperationType.AggregatedObjectChanged));
									ForceParentObjectChange(referencedObject, allDiffs);
								}
							}
						}
					}
				}
			}
		}
		private bool IsParentChangeAllreadyInDiffs(DiffCollection allDiffs, object owner, XPMemberInfo parentMemberInfo, object collectionObject, AuditOperationType operationType) {
			bool cancelAdd = false;
			List<AuditDataItem> currentList;
			if(allDiffs.TryGetValue(owner, out currentList)) {
				foreach(AuditDataItem item in currentList) {
					if(item.OperationType == operationType && item.MemberInfo == parentMemberInfo && item.OldValue == collectionObject) {
						cancelAdd = true;
						break;
					}
				}
			}
			return cancelAdd;
		}
		protected void ForceAssociatedObjectsChange(object collectionObject, ObjectValue oldValue, object newValue, DiffCollection diffToAddTo) {
			XPMemberInfo associatedMemberInfo = oldValue.MemberInfo.GetAssociatedMember();  
			if(IsCollection(associatedMemberInfo)) {
				AddDiffOfAssociatedCollectionChange(oldValue.PropertyValue, collectionObject, associatedMemberInfo, AuditOperationType.RemovedFromCollection, diffToAddTo);
				AddDiffOfAssociatedCollectionChange(newValue, collectionObject, associatedMemberInfo, AuditOperationType.AddedToCollection, diffToAddTo);
			}
		}
		protected void ForceParentObjectChange(object owner, DiffCollection diffToAddTo) {
			if(IsObjectToAudit(owner)) {
				ForceParentChangeForAggregated(owner as IXPSimpleObject, diffToAddTo);
				ForceParentChangeForCollection(owner as IXPSimpleObject, diffToAddTo);
			}
		}
		protected void ForceParentChangeForCollection(object collectionObject, DiffCollection allDiffs) {
			XPClassInfo classInfo = ((IXPSimpleObject)collectionObject).ClassInfo;
			foreach(XPMemberInfo info in classInfo.PersistentProperties) {
				if(info.IsAssociation) {
					IXPSimpleObject owner = info.GetValue(collectionObject) as IXPSimpleObject;
					if(owner != null) {
						XPMemberInfo parentMemberInfo = info.GetAssociatedMember(); 
						if((parentMemberInfo != null) && parentMemberInfo.IsAggregated) {
							if(!(IsParentChangeAllreadyInDiffs(allDiffs, owner, parentMemberInfo, collectionObject, AuditOperationType.CollectionObjectChanged))) {
								allDiffs.Add(new AuditDataItem(owner, parentMemberInfo, collectionObject, null, AuditOperationType.CollectionObjectChanged));
								ForceParentObjectChange(owner, allDiffs);
							}
						}
					}
				}
			}
		}
		protected override void BeginObjectAuditCore(object obj, List<AuditTrailMemberInfo> membersInfo) {
			oldObjectPropertiesValues[obj] = GetInitialPropertiesValues(obj, membersInfo);
		}
		protected override void FillDiffsCore(DiffCollection diffs) {
			ArrayList currentAuditObjects = new ArrayList(oldObjectPropertiesValues.Keys);
			foreach(object obj in currentAuditObjects) {
				FillDiffList(oldObjectPropertiesValues[obj], obj, diffs);
			}
		}
		protected override void ProcessAuditedObjectChange(object sender, ObjectChangeEventArgs e, bool isFirstTimeAuditedModification) {
			if(!string.IsNullOrEmpty(e.PropertyName)) {
				if(isPropertyChangesAuditEnabled) {
					List<ObjectValue> values = oldObjectPropertiesValues[sender];
					foreach(AuditTrailMemberInfo auditTrailMemberInfo in GetPropertiesToAudit(sender.GetType())) {
						if(auditTrailMemberInfo.Name == e.PropertyName) {
							bool isFound = false;
							foreach(ObjectValue objValue in values) {
								if(objValue.MemberInfo.Name == e.PropertyName) {
									if(isFirstTimeAuditedModification) {
										objValue.PropertyValue = e.OldValue;
									}
									isFound = true;
									break;
								}
							}
							if((auditTrailMemberInfo.MemberInfo != null) && !isFound) {
								ObjectValue value = new ObjectValue();
								values.Add(value);
								value.MemberInfo = auditTrailMemberInfo.MemberInfo;
								value.PropertyValue = e.OldValue;
							}
						}
					}
				}
			}
		}
		protected override DiffCollection ResolveManyToManyDiffsForIntermediateObject(IDCIntermediateObject intermediateObject, List<AuditDataItem> listToResolve) {
			DiffCollection result = new DiffCollection();
			DevExpress.Xpo.Helpers.IXPClassInfoProvider classInfoProvider = intermediateObject as DevExpress.Xpo.Helpers.IXPClassInfoProvider;
			if(classInfoProvider != null) {
				foreach(AuditDataItem item in listToResolve) {
					AuditOperationType auditOperationType = item.OperationType == AuditOperationType.ObjectCreated ? AuditOperationType.AddedToCollection : AuditOperationType.RemovedFromCollection;
					XPMemberInfo leftMemberInfo = null;
					XPMemberInfo rightMemberInfo = null;
					foreach(XPMemberInfo info in classInfoProvider.ClassInfo.PersistentProperties) {
						if(info.ReferenceType != null) {
							if(info.ReferenceType.ClassType == intermediateObject.LeftObject.GetType() || info.GetValue(intermediateObject) == intermediateObject.LeftObject) {
								leftMemberInfo = info;
							}
							if(info.ReferenceType.ClassType == intermediateObject.RightObject.GetType() || info.GetValue(intermediateObject) == intermediateObject.RightObject) {
								rightMemberInfo = info;
							}
						}
					}
					if(leftMemberInfo != null && rightMemberInfo != null) {
						AddCollectionChangeAuditDataItem(intermediateObject.LeftObject, auditOperationType, (IXPSimpleObject)intermediateObject.RightObject, leftMemberInfo, result);
						AddCollectionChangeAuditDataItem(intermediateObject.RightObject, auditOperationType, (IXPSimpleObject)intermediateObject.LeftObject, rightMemberInfo, result);
					}
				}
			}
			return result;
		}
		protected override DiffCollection ResolveManyToManyDiffsForIntermediateObject(IntermediateObject intermediateObject, List<AuditDataItem> listToResolve) {
			DiffCollection result = new DiffCollection();
			foreach(AuditDataItem item in listToResolve) {
				AuditOperationType auditOperationType;
				if(item.OperationType == AuditOperationType.ObjectCreated) {
					auditOperationType = AuditOperationType.AddedToCollection;
				}
				else {
					auditOperationType = AuditOperationType.RemovedFromCollection;
				}
				XPMemberInfo leftMemberInfo = null;
				XPMemberInfo rightMemberInfo = null;
				foreach(XPMemberInfo info in intermediateObject.ClassInfo.PersistentProperties) {
					if(info.ReferenceType != null) {
						if(info.GetValue(intermediateObject) == intermediateObject.LeftIntermediateObjectField) {
							leftMemberInfo = info;
						}
						if(info.GetValue(intermediateObject) == intermediateObject.RightIntermediateObjectField) {
							rightMemberInfo = info;
						}
					}
				}
				if(leftMemberInfo != null && rightMemberInfo != null) {
					AddCollectionChangeAuditDataItem(intermediateObject.LeftIntermediateObjectField, auditOperationType,
						intermediateObject.RightIntermediateObjectField as IXPSimpleObject, leftMemberInfo, result);
					AddCollectionChangeAuditDataItem(intermediateObject.RightIntermediateObjectField, auditOperationType,
						intermediateObject.LeftIntermediateObjectField as IXPSimpleObject, rightMemberInfo, result);
				}
			}
			return result;
		}
		protected override IList GetAuditedObjects() {
			return new ArrayList(oldObjectPropertiesValues.Keys);
		}
		protected override void ClearObjectPropertiesAuditInformation(object obj) {
			oldObjectPropertiesValues.Remove(obj);
		}
		public FullObjectAuditProcessor(Session session, AuditTrailSettings settings)
			: base(session, settings) {
		}
		public override bool IsObjectAudited(object obj) {
			return oldObjectPropertiesValues.ContainsKey(obj);
		}
	}
}
