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
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.Xpo.Exceptions;
using System.Collections.Generic;
using DevExpress.Xpo.DB.Exceptions;
using System.Threading;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Xpo.Helpers {
	public class ParentCriteriaGenerator : ClientCriteriaVisitorBase {
		protected readonly Session session;
		protected readonly Session parent;
		protected readonly NestedParentMap map;
		protected readonly ISecurityRuleProvider securityDictionary;
		protected readonly object securityContext;
		protected ParentCriteriaGenerator(NestedUnitOfWork nuow)
			: this(nuow, nuow.Parent, nuow.Map, null) {
		}
		protected ParentCriteriaGenerator(Session session, Session parentSession, NestedParentMap map, SecurityContext securityContext) {
			this.session = session;
			this.parent = parentSession;
			this.map = map;
			this.securityContext = securityContext;
		}
		protected override CriteriaOperator Visit(OperandValue theOperand) {
			if(session.Dictionary.QueryClassInfo(theOperand.Value) == null)
				return theOperand;
			else{
				object resultValue = theOperand.Value;
				ISessionProvider sessionObj = resultValue as ISessionProvider;
				if(sessionObj == null || sessionObj.Session != parent) {
					resultValue = NestedWorksHelper.GetParentObject(session, parent, map, resultValue);
					return theOperand is ConstantValue ? new ConstantValue(resultValue) : new OperandValue(resultValue);
				}
				return theOperand;
			}
		}
		public static CriteriaOperator GetNestedCriteria(NestedUnitOfWork nuow, CriteriaOperator op) {
			return new ParentCriteriaGenerator(nuow).Process(op);
		}
		public static CriteriaOperator GetNestedCriteria(Session session, Session parent, NestedParentMap map, SecurityContext securityContext, CriteriaOperator op) {
			return new ParentCriteriaGenerator(session, parent, map, securityContext).Process(op);
		}
	}
	public abstract class NestedParentMap {
		public static NestedParentMap Extract(NestedUnitOfWork source) {
			return source.Map;
		}
		protected NestedParentMap() { }
		public abstract void Add(object parent, object nested, bool hasValidKey);
		public abstract object GetParent(object nested);
		public abstract object GetNested(object parent);
		public abstract void KickOut(object nested);
	}
#if !SL
	public class WeakNestedParentMap : StrongNestedParentMap {
		Dictionary<ObjectRecord, ObjectRecord> Parents = new Dictionary<ObjectRecord, ObjectRecord>();
		Dictionary<ObjectRecord, ObjectRecord> Nesteds = new Dictionary<ObjectRecord, ObjectRecord>();
		public override void Add(object parent, object nested, bool hasValidKey) {
			if(hasValidKey) {
				ObjectRecord pRecord = ObjectRecord.GetObjectRecord(parent);
				ObjectRecord nRecord = ObjectRecord.GetObjectRecord(nested);
				Nesteds[pRecord] = nRecord;
				Parents[nRecord] = pRecord;
			} else {
				base.Add(parent, nested, hasValidKey);
			}
		}
		public override object GetParent(object nested) {
			object rv = base.GetParent(nested);
			if(rv != null)
				return rv;
			ObjectRecord nRecord = ObjectRecord.GetObjectRecord(nested);
			ObjectRecord pRecord;
			if(Parents.TryGetValue(nRecord, out pRecord))
				return pRecord.Object;
			else
				return null;
		}
		public override object GetNested(object parent) {
			object rv = base.GetNested(parent);
			if(rv != null)
				return rv;
			ObjectRecord pRecord = ObjectRecord.GetObjectRecord(parent);
			ObjectRecord nRecord;
			if(Nesteds.TryGetValue(pRecord, out nRecord))
				return nRecord.Object;
			else
				return null;
		}
		public override void KickOut(object nested) {
			base.KickOut(nested);
			ObjectRecord nRecord = ObjectRecord.GetObjectRecord(nested);
			ObjectRecord pRecord;
			if(!Parents.TryGetValue(nRecord, out pRecord))
				return;
			Parents.Remove(nRecord);
			Nesteds.Remove(pRecord);
		}
	}
#endif
	public class StrongNestedParentMap : NestedParentMap {
		ObjectDictionary<object> Parents = new ObjectDictionary<object>();
		ObjectDictionary<object> Nesteds = new ObjectDictionary<object>();
		public override void Add(object parent, object nested, bool hasValidKey) {
			Parents.Add(nested, parent);
			Nesteds.Add(parent, nested);
		}
		public override object GetParent(object nested) {
			object rv;
			if(Parents.TryGetValue(nested, out rv))
				return rv;
			else
				return null;
		}
		public override object GetNested(object parent) {
			object rv;
			if(Nesteds.TryGetValue(parent, out rv))
				return rv;
			else
				return null;
		}
		public override void KickOut(object nested) {
			object parent;
			if(!Parents.TryGetValue(nested, out parent))
				return;
			Parents.Remove(nested);
			Nesteds.Remove(parent);
		}
	}
	public class NestedLoader {
		readonly Queue<ObjectPair> toProcess = new Queue<ObjectPair>();
		readonly NestedParentMap Map;
		readonly SecurityContext SecurityContext;
		struct ObjectPair {
			public readonly object Source;
			public readonly object Destination;
			public readonly OptimisticLockingReadMergeBehavior LoadMerge;
			public ObjectPair(object source, object destination, OptimisticLockingReadMergeBehavior loadMerge) {
				Source = source;
				Destination = destination;
				LoadMerge = loadMerge;
			}
		}
		public readonly Session Owner;
		public readonly Session OwnerParent;
		public NestedLoader(Session owner, Session ownerParent, NestedParentMap map, SecurityContext securityContext) {
			this.Owner = owner;
			this.OwnerParent = ownerParent;
			this.Map = map;
			this.SecurityContext = securityContext;
		}
		public NestedLoader(Session owner, Session ownerParent, NestedParentMap map)
			: this(owner, ownerParent, map, null) {
		}
		public NestedLoader(NestedUnitOfWork owner)
			: this(owner, owner.Parent, NestedParentMap.Extract(owner)) {
		}
		void CloneObjects(out IList toFireLoaded) {
			toFireLoaded = new List<object>();
			ObjectDictionary<object> processedPairs = new ObjectDictionary<object>();
			List<ObjectPair> processed = new List<ObjectPair>();
			do {
				ObjectPair s = toProcess.Dequeue();
				toFireLoaded.Add(s.Destination);
				object source = s.Source;
				XPClassInfo ci = Owner.GetClassInfo(source);
				foreach (XPMemberInfo mi in ci.ObjectProperties) {
					if (mi.IsReadOnly)
						continue;
					if (mi.IsDelayed) {
						XPDelayedProperty container = XPDelayedProperty.GetDelayedPropertyContainer(source, mi);
						if (!container.IsLoaded)
							continue;
						if (container.Value == null)
							continue;
						if (!OwnerParent.IsNewObject(source) && !OwnerParent.IsNewObject(container.Value))
							continue;
					}
					object value = mi.GetValue(source);
					if (value != null) {
						object resObjectValue;
						if (processedPairs.TryGetValue(value, out resObjectValue)) continue;
						resObjectValue = GetNestedObjectCore(value, false);
						processedPairs.Add(value, resObjectValue);
					}
				}
				foreach (XPMemberInfo mi in ci.CollectionProperties) {
					XPBaseCollection col = (XPBaseCollection)mi.GetValue(source);
					if (col == null)
						continue;
					if (col.IsLoaded) {
						foreach (object obj in col.Helper.IntObjList) {
							IXPInvalidateableObject spoilableObject = obj as IXPInvalidateableObject;
							if (spoilableObject != null && spoilableObject.IsInvalidated) continue;
							object resObjectValue;
							if (processedPairs.TryGetValue(obj, out resObjectValue)) continue;
							resObjectValue = GetNestedObjectCore(obj, false);
							processedPairs.Add(obj, resObjectValue);
						}
					}
				}
				processed.Add(s);
			} while (toProcess.Count > 0);
			for (int i = 0; i < processed.Count; i++) {
				ObjectPair s = (ObjectPair)processed[i];
				Owner.TriggerObjectLoading(s.Destination);
				CloneData(s.Source, s.Destination, s.LoadMerge, processedPairs);
			}
			GC.KeepAlive(processedPairs);
		}
		void CloneData(object source, object destination, OptimisticLockingReadMergeBehavior loadMerge, ObjectDictionary<object> processedPairs) {
			XPClassInfo ci = Owner.GetClassInfo(source);
			ISecurityRule rule = null;
			if (SecurityContext != null) {
				rule = SecurityContext.SecurityRuleProvider.GetRule(ci);
			}
			bool isSystemClass = SecurityContext.IsSystemClass(ci);
			foreach (XPMemberInfo mi in ci.PersistentProperties) {
				if (mi.IsReadOnly)
					continue;
				object value;
				if (mi.IsDelayed && !OwnerParent.IsNewObject(source)) {
					if (mi.ReferenceType == null) {
						if (!ObjectCollectionLoader.AcceptLoadPropertyAndResetModified(Owner.TrackPropertiesModifications, loadMerge, destination, mi, ReturnArgument, null))
							continue;
						XPDelayedProperty.Init(Owner, destination, mi, null);
						continue;
					} else {
						bool skipInit = false;
						XPDelayedProperty srcDelayed = XPDelayedProperty.GetDelayedPropertyContainer(source, mi);
						object loadedValue = null;
						if (!srcDelayed.IsLoaded) {
							loadedValue = srcDelayed.InternalValue;
						} else {
							value = srcDelayed.Value;
							if (value != null) {
								if (OwnerParent.IsNewObject(value)) {
									skipInit = true;
								} else {
									value = mi.ReferenceType.GetId(value);
								}
							}
							loadedValue = value;
						}
						if (!skipInit) {
							if (!ObjectCollectionLoader.AcceptLoadPropertyAndResetModified(Owner.TrackPropertiesModifications, loadMerge, destination, mi, ReturnArgument, loadedValue))
								continue;
							XPDelayedProperty.Init(Owner, destination, mi, loadedValue);
							continue;
						}
					}
				}
				CriteriaOperator memberExpression;
				if (rule != null && !isSystemClass && !SecurityContext.IsSystemProperty(mi) && SecurityContext.GetSelectMemberExpression(rule, ci, mi, out memberExpression)) {
					value = SecurityContext.GetEvaluator(ci, memberExpression).Evaluate(source);
				} else {
					value = mi.GetValue(source);
				}
				if (mi.ReferenceType != null && value != null) {
					object processedObjectValue;
					bool raiseNotClonable = false;
					if (!processedPairs.TryGetValue(value, out processedObjectValue)) {
						raiseNotClonable = true;
					} else {
						value = NestedWorksHelper.TryGetNestedObject(OwnerParent, Map, value);
						raiseNotClonable = (value != processedObjectValue);
					}
					if (raiseNotClonable) throw new InvalidOperationException(Res.GetString(Res.NestedSession_NotClonable, ci.FullName, mi.Name));
				}				
				if (!ObjectCollectionLoader.AcceptLoadPropertyAndResetModified(Owner.TrackPropertiesModifications, loadMerge, destination, mi, ReturnArgument, value))
					continue;
				mi.SetValue(destination, value);
			}
		}
		static Func<object, object> ReturnArgument = arg => arg;
		object CreateNestedObject(object pObject, bool forceLoad) {
			XPClassInfo ci = Owner.GetClassInfo(pObject);
			if(Owner.ObjectLayer.IsStaticType(ci)) {
				Map.Add(pObject, pObject, true);
				return pObject;
			}
			if(SecurityContext != null) {
				ISecurityRule rule = SecurityContext.SecurityRuleProvider.GetRule(ci);
				if(rule != null) {
					if(!SecurityContext.IsSystemClass(ci) && !rule.ValidateObjectOnSelect(SecurityContext, ci, pObject)) return null;
				}
			}
			object nObject;
			bool foundById = false;
			bool isNew = OwnerParent.IsNewObject(pObject);
			if(isNew) {
				nObject = ci.CreateObject(Owner);
			} else {
				object key = ci.GetId(pObject);
				nObject = Owner.GetLoadedObjectByKey(ci, key);
				if(nObject == null) {
					nObject = ci.CreateObject(Owner);
					SessionIdentityMap.RegisterObject(Owner, nObject, key); 
				} else {
					Map.KickOut(nObject);
					foundById = true;
				}
			}
			Map.Add(pObject, nObject, !isNew);
			OptimisticLockingReadMergeBehavior loadMerge = OptimisticLockingReadMergeBehavior.Default;
			if (!foundById || ObjectCollectionLoader.NeedReload(Owner, ci, forceLoad, out loadMerge, () => { return IsObjectVersionChanged(nObject, pObject); })) {
				toProcess.Enqueue(new ObjectPair(pObject, nObject, loadMerge));
			}
			return nObject;
		}
		public bool IsObjectVersionChanged(object nestedObject, object parentObject) {
			XPClassInfo ci = Owner.GetClassInfo(parentObject);
			if (ci.OptimisticLockField == null) return false;
			return ((int?)ci.OptimisticLockField.GetValue(nestedObject)) != ((int?)ci.OptimisticLockField.GetValue(parentObject)) &&
				((int?)ci.OptimisticLockFieldInDataLayer.GetValue(nestedObject)) != ((int?)ci.OptimisticLockFieldInDataLayer.GetValue(parentObject));
		}
		object GetNestedObjectCore(object parentObject, bool forceLoad) {
			object rv = NestedWorksHelper.TryGetNestedObject(OwnerParent, Map, parentObject);
			if(rv != null) {
				IXPInvalidateableObject invalidatedObject = rv as IXPInvalidateableObject;
				if(invalidatedObject == null || !invalidatedObject.IsInvalidated) {
					OptimisticLockingReadMergeBehavior loadMerge = OptimisticLockingReadMergeBehavior.Default;
					if (ObjectCollectionLoader.NeedReload(Owner, Owner.GetClassInfo(parentObject), forceLoad, out loadMerge, () => { return IsObjectVersionChanged(rv, parentObject); })) {
						toProcess.Enqueue(new ObjectPair(parentObject, rv, loadMerge));
					}
					return rv;
				}
			}
			rv = CreateNestedObject(parentObject, forceLoad);
			return rv;
		}
		public ICollection[] GetNestedObjects(ICollection[] parentObjects) {
			return GetNestedObjects(parentObjects, null);
		}
		public ICollection[] GetNestedObjects(ICollection[] parentObjects, bool[] force) {
			ICollection[] result = new ICollection[parentObjects.Length];
			IList objectsToFireLoaded = null;
			SessionStateStack.Enter(Owner, SessionState.GetObjectsNonReenterant);
			try {
				SessionStateStack.Enter(OwnerParent, SessionState.LoadingObjectsIntoNestedUow);
				try {
					for(int i = 0; i < parentObjects.Length; i++) {
						bool forceLoad = force == null ? false : force[i];
						List<object> list = new List<object>(parentObjects[i].Count);
						foreach(object parentObj in parentObjects[i]) {
							if(parentObj == null) {
								list.Add(null);
								continue;
							}
							object obj = GetNestedObjectCore(parentObj, forceLoad);
							if(obj == null) continue;
							list.Add(obj);
						}
						result[i] = list;
					}
					if(toProcess.Count > 0) {
						CloneObjects(out objectsToFireLoaded);
					}
				} finally {
					SessionStateStack.Leave(OwnerParent, SessionState.LoadingObjectsIntoNestedUow);
				}
				if(objectsToFireLoaded != null) {
					foreach(object obj in objectsToFireLoaded) {
						XPClassInfo ci = Owner.GetClassInfo(obj);
						XPMemberInfo olf = ci.OptimisticLockField;
						if(olf != null) {
							ci.OptimisticLockFieldInDataLayer.SetValue(obj, olf.GetValue(obj));
						}
					}
				}
			} finally {
				SessionStateStack.Leave(Owner, SessionState.GetObjectsNonReenterant);
			}
			if(objectsToFireLoaded != null)
				Owner.TriggerObjectsLoaded(objectsToFireLoaded);
			return result;
		}
		public object GetNestedObject(object parentObject) {
			ICollection[] res = GetNestedObjects(new ICollection[] { new object[] { parentObject } });
			IEnumerator en = res[0].GetEnumerator();
			en.MoveNext();
			return en.Current;
		}
	}
	internal static class NestedWorksHelper {
		public static object CommitObject(Session session, Session parentSession, NestedParentMap map, object obj, XPMemberInfo[] membersNotToSave) {
			object parentObj = NestedWorksHelper.CreateParentObject(session, parentSession, map, obj);
			XPClassInfo ci = session.GetClassInfo(obj);
			parentSession.TriggerObjectLoading(parentObj);
			bool trackModifications = ci.TrackPropertiesModifications ?? parentSession.TrackPropertiesModifications;
			bool nestedTrackModifications = ci.TrackPropertiesModifications ?? session.TrackPropertiesModifications;
			foreach(XPMemberInfo mi in ci.PersistentProperties) {
				if(mi.IsDelayed && !XPDelayedProperty.GetDelayedPropertyContainer(obj, mi).IsLoaded)
					continue;
				if(mi.IsReadOnly)
					continue;
				if(membersNotToSave != null && Array.IndexOf<XPMemberInfo>(membersNotToSave, mi) >= 0) continue;
				object value = mi.GetValue(obj);
				if(mi.ReferenceType != null && value != null)
					value = NestedWorksHelper.CreateParentObject(session, parentSession, map, value);
				if(mi.ReferenceType != null) {
					object oldValue = mi.GetValue(parentObj);
					mi.SetValue(parentObj, value);
					if(trackModifications && !PersistentBase.CanSkipAssignement(oldValue, value))
						mi.SetModified(parentObj, oldValue);
					mi.ProcessAssociationRefChange(parentSession, parentObj, oldValue, value);
				} else {
					if(trackModifications) {
						object oldValue = mi.GetValue(parentObj);
						mi.SetValue(parentObj, value);
						if(!PersistentBase.CanSkipAssignement(oldValue, value))
							mi.SetModified(parentObj, oldValue);
					} else
						mi.SetValue(parentObj, value);
				}
			}
			if(parentObj is IntermediateObject && parentSession.IsNewObject(parentObj)) {
				IntermediateObject intObj = (IntermediateObject)parentObj;
				IntermediateClassInfo intObjClassInfo = (IntermediateClassInfo)parentSession.GetClassInfo(intObj);
				System.Diagnostics.Debug.Assert(intObj.LeftIntermediateObjectField != null);
				System.Diagnostics.Debug.Assert(intObj.RightIntermediateObjectField != null);
				XPBaseCollection leftC = (XPBaseCollection)intObjClassInfo.intermediateObjectFieldInfoRight.refProperty.GetValue(intObj.LeftIntermediateObjectField);
				XPRefCollectionHelperManyToMany leftHelper = (XPRefCollectionHelperManyToMany)leftC.Helper;
				leftHelper.AddIntermediateObject(intObj, intObj.RightIntermediateObjectField);
				leftC.BaseAdd(intObj.RightIntermediateObjectField);
			}
			return parentObj;
		}
		public static void CommitDeletedObject(Session session, Session parentSession, NestedParentMap map, object obj) {
			object parentObj = NestedWorksHelper.GetParentObject(session, parentSession, map, obj);
			if(parentObj == null)
				return;
			XPClassInfo ci = session.GetClassInfo(obj);
			bool trackModifications = ci.TrackPropertiesModifications ?? parentSession.TrackPropertiesModifications;
			bool nestedTrackModifications = ci.TrackPropertiesModifications ?? session.TrackPropertiesModifications;
			foreach(XPMemberInfo mi in ci.PersistentProperties) {
				if(nestedTrackModifications && trackModifications && mi.GetModified(obj) && !mi.GetModified(parentObj)) {
					mi.SetModified(parentObj, mi.GetValue(parentObj));
				}
			}
			if(obj is IntermediateObject) {
				IntermediateObject intParent = (IntermediateObject)parentObj;
				IntermediateClassInfo intObjClassInfo = (IntermediateClassInfo)parentSession.GetClassInfo(intParent);
				if(intParent.LeftIntermediateObjectField != null && intParent.RightIntermediateObjectField != null) {
					XPBaseCollection leftC = (XPBaseCollection)intObjClassInfo.intermediateObjectFieldInfoRight.refProperty.GetValue(intParent.LeftIntermediateObjectField);
					leftC.BaseRemove(intParent.RightIntermediateObjectField);
				}
			}
			parentSession.Delete(parentObj);
		}
		public static object CreateParentObject(Session session, Session parentSession, NestedParentMap map, object obj) {
			object parent = NestedWorksHelper.GetParentObject(session, parentSession, map, obj);
			if(parent != null)
				return parent;
			parent = session.GetClassInfo(obj).CreateObject(parentSession);
			map.Add(parent, obj, !session.IsNewObject(obj));
			return parent;
		}
		public static void ValidateVersions(Session session, Session parentSession, NestedParentMap map, ObjectSet lockedParentsObjects, ICollection nestedObjects, LockingOption lockingOption, bool objectsForDelete) {
			foreach(object obj in nestedObjects) {
				object parentObj = NestedWorksHelper.GetParentObject(session, parentSession, map, obj);
				if(parentObj == null) {
					if(!session.IsNewObject(obj))
						throw new LockingException();
					continue;
				}
				if(lockedParentsObjects.Contains(parentObj))
					continue;
				lockedParentsObjects.Add(parentObj);
				if (session.IsNewObject(obj) != parentSession.IsNewObject(parentObj))
					throw new LockingException();
				XPClassInfo ci = session.GetClassInfo(obj);
				if (lockingOption == LockingOption.Optimistic) {
					OptimisticLockingBehavior kind = ci.OptimisticLockingBehavior;
					switch (kind) {
						case OptimisticLockingBehavior.ConsiderOptimisticLockingField: {
								XPMemberInfo olf = ci.OptimisticLockField;
								if (olf == null)
									continue;
								int? parentV = (int?)olf.GetValue(parentObj);
								int? childV = (int?)ci.OptimisticLockFieldInDataLayer.GetValue(obj);
								if (parentV != childV) {
									throw new LockingException();
								}
							}
							break;
						case OptimisticLockingBehavior.LockAll:
						case OptimisticLockingBehavior.LockModified:
							if (LockingHelper.HasModified(ci, ci.PersistentProperties, obj, parentObj, objectsForDelete ? OptimisticLockingBehavior.LockAll : kind))
								throw new LockingException();
							break;
					}
				}
			}
		}
		public static object GetParentObject(Session session, Session parentSession, NestedParentMap map, object obj) {
			session.ThrowIfObjectFromDifferentSession(obj);
			object parent = map.GetParent(obj);
			if(parent != null)
				return parent;
			if(session.IsNewObject(obj))
				return parent;
			XPClassInfo ci = session.GetClassInfo(obj);
			object key = ci.GetId(obj);
			parent = parentSession.GetObjectByKey(ci, key);
			if(parent == null)
				return null;
			map.Add(parent, obj, true);
			return parent;
		}
		public static object GetNestedObject(Session session, Session parentSession, NestedParentMap map, object obj) {
			return GetNestedObject(session, parentSession, map, obj, null);
		}
		public static object GetNestedObject(Session session, Session parentSession, NestedParentMap map, object obj, SecurityContext securityContext) {
			object child = TryGetNestedObject(parentSession, map, obj);
			if(child == null) {
				return new NestedLoader(session, parentSession, map, securityContext).GetNestedObject(obj);
			}
			return child;
		}
		public static object TryGetNestedObject(Session parent, NestedParentMap map, object obj) {
			parent.ThrowIfObjectFromDifferentSession(obj);
			return map.GetNested(obj);
		}
	}
}
namespace DevExpress.Xpo {
	[ToolboxItem(false)]
	public class NestedUnitOfWork : UnitOfWork {
		Session parent;
#if !SL
	[DevExpressXpoLocalizedDescription("NestedUnitOfWorkParent")]
#endif
public Session Parent {
			get {
				return parent;
			}
		}
		NestedParentMap map;
		internal NestedParentMap Map {
			get {
				return map;
			}
		}
		protected internal NestedUnitOfWork(Session parent)
			: base(new SessionObjectLayer(parent)) {
			this.parent = parent;
			this.map = ((SessionObjectLayer)ObjectLayer).GetNestedParentMap(this);
		}
		public T GetParentObject<T>(T obj) {
			return (T)GetParentObject((object)obj);
		}
		public object GetParentObject(object obj) {
			return NestedWorksHelper.GetParentObject(this, Parent, Map, obj);
		}
		public object TryGetNestedObject(object obj) {
			return NestedWorksHelper.TryGetNestedObject(Parent, Map, obj);
		}
		public T GetNestedObject<T>(T obj) {
			return (T)GetNestedObject((object)obj);
		}
		public object GetNestedObject(object obj) {
			return NestedWorksHelper.GetNestedObject(this, Parent, Map, obj);
		}
		public object[] GetNestedObjects(params object[] parentObjects) {
			ICollection res = new NestedLoader(this, Parent, Map).GetNestedObjects(new ICollection[] { parentObjects })[0];
			return ListHelper.FromCollection(res).ToArray();
		}
		protected override void OnBeforeCommitTransaction() {
			base.OnBeforeCommitTransaction();
			Parent.OnBeforeCommitNestedUnitOfWork(new SessionManipulationEventArgs(this));
		}
		protected override void OnAfterCommitTransaction() {
			base.OnAfterCommitTransaction();
			Parent.OnAfterCommitNestedUnitOfWork(new SessionManipulationEventArgs(this));
		}
	}
}
