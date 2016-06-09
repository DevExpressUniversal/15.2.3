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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.Xpo.Exceptions;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Data.Filtering.Helpers;
using System.Resources;
#if SL
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpo.Helpers {
	using System.Collections;
	using System.ComponentModel;
	using System.Text;
	using System.Reflection;
	using System.Data;
	using System.Threading;
	using DevExpress.Xpo.Generators;
	using DevExpress.Xpo.DB;
	using DevExpress.Xpo.DB.Exceptions;
	public interface ISessionProvider : IObjectLayerProvider, IDataLayerProvider {
		Session Session {
			get;
		}
	}
#if !CF && !SL
	[ToolboxItem(false)]
	public sealed class DefaultSession : Session {
		public DefaultSession(ISite site)
			: base() {
			this.Site = site;
		}
		public override XPDictionary Dictionary {
			get {
				if(dict == null) {
					dict = CreateDesignTimeDictionary(Site);
				}
				return dict;
			}
		}
	}
#endif
	public sealed class SessionIdentityMap {
		ObjectIdentityMap loadedObjects;
		public SessionIdentityMap(IObjectLayerProvider objectLayerProvider, IdentityMapBehavior cacheBehavior) {
#if !SL
			if(cacheBehavior == IdentityMapBehavior.Weak)
				loadedObjects = new WeakObjectIdentityMap(objectLayerProvider);
			else
#endif
				loadedObjects = new StrongObjectIdentityMap(objectLayerProvider);
		}
		public void Clear() {
			loadedObjects.Clear();
		}
		public void Compact() {
			loadedObjects.Compact();
		}
		IObjectMap GetMap(XPClassInfo ci) {
			return loadedObjects.GetObjects(ci);
		}
		object GetLoadedObjectByKey(XPClassInfo ci, object key) {
			return loadedObjects.Get(ci, key);
		}
		void RegisterObject(object theObject, object key) {
			loadedObjects.Add(theObject, key);
		}
		void UnregisterObject(object theObject) {
			loadedObjects.Remove(theObject);
		}
		public static SessionIdentityMap Extract(Session session) {
			if(session._IdentityMap == null)
				session._IdentityMap = new SessionIdentityMap(session, session.GetIdentityMapBehavior());
			return session._IdentityMap;
		}
		public static IObjectMap GetMap(Session session, XPClassInfo ci) {
			return Extract(session).GetMap(ci);
		}
		public static void RegisterObject(Session session, object theObject, object key) {
			Extract(session).RegisterObject(theObject, key);
		}
		public static void UnregisterObject(Session session, object theObject) {
			Extract(session).UnregisterObject(theObject);
		}
		public static object GetLoadedObjectByKey(Session session, XPClassInfo ci, object key) {
			return Extract(session).GetLoadedObjectByKey(ci, key);
		}
	}
	sealed class StrongSession : Session {
		public StrongSession(IObjectLayer layer)
			: base(layer) {
			IdentityMapBehavior = IdentityMapBehavior.Strong;
		}
	}
	public interface IWideDataStorage {
		bool WideDataContainsKey(object key);
		void SetWideDataItem(object key, object value);
		object GetWideDataItem(object key);
		bool TryGetWideDataItem(object key, out object value);
	}
	public sealed class XPObjectTypesManager {
		IObjectLayer managedLayer;
		Dictionary<XPClassInfo, XPObjectType> loadedTypes;
		IObjectMap objects;
		public XPObjectTypesManager(Session session)
			: this(session.ObjectLayer) {
		}
		public XPObjectTypesManager(IObjectLayer layer) {
			this.managedLayer = layer;
		}
		Dictionary<XPClassInfo, XPObjectType> GetAllTypes() {
			if(loadedTypes == null) {
				XPClassInfo objectType = managedLayer.Dictionary.GetClassInfo(typeof(XPObjectType));
				loadedTypes = managedLayer.GetObjectLayerWideObjectTypes();
				if(loadedTypes == null) {
					lock(managedLayer.Dictionary) {
						loadedTypes = managedLayer.GetObjectLayerWideObjectTypes();
						if(loadedTypes == null) {
							managedLayer.RegisterStaticTypes(objectType);
							loadedTypes = new Dictionary<XPClassInfo, XPObjectType>();
							XPObjectType type = new XPObjectType(managedLayer.Dictionary, objectType.AssemblyName, objectType.FullName);
							loadedTypes[type.TypeClassInfo] = type;
							managedLayer.SetObjectLayerWideObjectTypes(loadedTypes);
							try {
								using(Session session = new StrongSession(managedLayer)) {
									ICollection list = session.GetObjects(objectType, null, null, 0, true, false);
									FillLoadedTypes(list);
								}
							} catch(SchemaCorrectionNeededException) {
							}
						}
					}
				}
				objects = managedLayer.GetStaticCache(objectType);
			}
			return loadedTypes;
		}
		void FillLoadedTypes(ICollection objectTypesList) {
			List<object> suspectObjectTypes = ListHelper.FromCollection(objectTypesList);
			List<XPClassInfo> justProcessedClassInfos = new List<XPClassInfo>(objectTypesList.Count);
			for(; ; ) {
				bool touched = false;
				for(int i = suspectObjectTypes.Count - 1; i >= 0; --i) {
					XPObjectType type = (XPObjectType)suspectObjectTypes[i];
					if(type.IsValidType) {
						loadedTypes.Add(type.TypeClassInfo, type);
						justProcessedClassInfos.Add(type.TypeClassInfo);
						suspectObjectTypes.RemoveAt(i);
						touched = true;
					}
				}
				if(!touched)
					break;
				Dictionary<XPClassInfo, XPClassInfo> touchedHolder = new Dictionary<XPClassInfo, XPClassInfo>();
				foreach(XPClassInfo ci in justProcessedClassInfos)
					ci.TouchRecursive(touchedHolder);
				justProcessedClassInfos.Clear();
			}
		}
		public void EnsureIsTypedObjectValid() {
			GetAllTypes();
		}
		public Dictionary<XPClassInfo, XPObjectType> AllTypes {
			get {
				return GetAllTypes();
			}
		}
		public XPObjectType GetObjectType(Int32 id) {
			GetAllTypes();
			XPObjectType type = (XPObjectType)objects.Get(id);
			if(type != null)
				return type;
			lock(managedLayer.Dictionary) {
				using(Session session = new StrongSession(managedLayer))
					type = session.GetObjectByKey<XPObjectType>(id);
				if(type != null && type.IsValidType) {
					loadedTypes.Add(type.TypeClassInfo, type);
					return type;
				}
				throw new TypeNotFoundException(id);
			}
		}
		public XPObjectType GetObjectType(XPClassInfo objectType) {
			XPObjectType type;
			if(!GetAllTypes().TryGetValue(objectType, out type)) {
				lock(managedLayer.Dictionary) {
					using(Session session = new StrongSession(managedLayer))
						type = session.FindObject<XPObjectType>(XPObjectType.Fields.TypeName == objectType.FullName);
					if(type == null) {
						objectType.GetRefTypes();	
						type = new XPObjectType(managedLayer.Dictionary, objectType.AssemblyName, objectType.FullName);
						managedLayer.CreateObjectType(type);
					}
					if(!GetAllTypes().ContainsKey(objectType))
						loadedTypes.Add(objectType, type);
				}
			}
			return type;
		}
		public XPObjectType TryGetObjectType(XPClassInfo objectType) {
			XPObjectType type;
			if(!GetAllTypes().TryGetValue(objectType, out type)) {
				return null;
			}
			return type;
		}
	}
	class LockingHelper {
		public static CriteriaOperator GetLockingCriteria(int? dbVersion, XPMemberInfo optimisticLock) {
			if (optimisticLock == null)
				return null;
			OperandProperty prop = new OperandProperty(optimisticLock.Name);
			if (dbVersion.HasValue)
				return prop == dbVersion.Value;
			else
				return prop.IsNull();
		}
		static bool IsPropertyToCheckLocking(OptimisticLockingBehavior option, XPMemberInfo mi, bool isModified) {
			if (mi.IsKey || (mi.IsDelayed && mi.ReferenceType == null))
				return false;
			if (option == OptimisticLockingBehavior.LockModified && !isModified)
				return false;
			return true;
		}
		public static ObjectGeneratorCriteriaSet GetLockingCriteria(XPClassInfo ci, IEnumerable properties, object theObject, OptimisticLockingBehavior option) {
			return GetLockingCriteria(ci, properties, theObject, option, null);
		}
		public static ObjectGeneratorCriteriaSet GetLockingCriteria(XPClassInfo ci, IEnumerable properties, object theObject, OptimisticLockingBehavior option, IList exclude) {
			if (option == OptimisticLockingBehavior.NoLocking || option == OptimisticLockingBehavior.ConsiderOptimisticLockingField) return null;
			ObjectGeneratorCriteriaSet result = new ObjectGeneratorCriteriaSet();			
			XPMemberInfo gc = ci.FindMember(GCRecordField.StaticName);
			bool isGCDeleted = gc != null && gc.GetValue(theObject) != null;
			foreach (XPMemberInfo mi in properties) {
				if (exclude != null && exclude.Contains(mi)) 
					continue; 
				bool isModified = mi.GetModified(theObject);
				if (!IsPropertyToCheckLocking(option, mi, isModified || isGCDeleted))
					continue;
				string tableName = string.IsNullOrEmpty(mi.Owner.TableName) ? ci.IdClass.TableName : mi.Owner.TableName;
				OperandProperty property = new OperandProperty(mi.Name);
				object value = null;
				if (isModified) {
					value = mi.GetOldValue(theObject);
				} else {
					if (mi.IsDelayed) {
						XPDelayedProperty delayedProperty = XPDelayedProperty.GetDelayedPropertyContainer(theObject, mi);
						if (delayedProperty.IsLoaded) {
							value = delayedProperty.Value;
						} else {
							property = new OperandProperty(string.Concat(mi.Name, ".", mi.ReferenceType.KeyProperty.Name));
							value = delayedProperty.InternalValue;
						}
					} else
						value = mi.GetValue(theObject);
				}
				result.UpdateCriteria(tableName, (value == null) ? property.IsNull() : (CriteriaOperator)(property == new OperandValue(value)));
			}
			return result;
		}
		public static object GetNestedValue(XPClassInfoStub classInfoStub, XPObjectStub obj, int memberIndex, out bool isModified) {
			object value = obj.Data[memberIndex];
			XPClassInfoStubOldValueContainer oldValue = obj.OldData == null ? null : obj.OldData[memberIndex];
			if (oldValue == null) {
				isModified = false;
				return value;
			}
			isModified = true;
			return oldValue.Data;
		}
		static object GetNestedValue(object theObject, XPMemberInfo mi, bool isModified) {
			object value = null;
			if (isModified) {
				value = mi.GetOldValue(theObject);
			} else {
				if (mi.IsDelayed) {
					XPDelayedProperty delayedProperty = XPDelayedProperty.GetDelayedPropertyContainer(theObject, mi);
					value = delayedProperty.IsLoaded ? delayedProperty.Value : delayedProperty.InternalValue;
				} else
					value = mi.GetValue(theObject);
			}
			return value;
		}
		static object GetParentValue(object parentObject, XPMemberInfo mi) {
			object parentValue = null;
			if (mi.GetModified(parentObject) || !mi.IsDelayed) {
				parentValue = mi.GetValue(parentObject);
			} else {
				XPDelayedProperty delayedProperty = XPDelayedProperty.GetDelayedPropertyContainer(parentObject, mi);
				parentValue = delayedProperty.IsLoaded ? delayedProperty.Value : delayedProperty.InternalValue;
			}
			return parentValue;
		}
		public static bool HasModified(XPClassInfoStub classInfoStub, XPObjectStub obj, IEnumerable properties, object parentObject, OptimisticLockingBehavior option) {
			if (option == OptimisticLockingBehavior.NoLocking || option == OptimisticLockingBehavior.ConsiderOptimisticLockingField) return false;
			int gcMemberIndex = classInfoStub.GetMemberIndex(GCRecordField.StaticName, false);
			bool isGCDeleted = gcMemberIndex >= 0 && obj.Data[gcMemberIndex] != null;
			foreach (XPMemberInfo mi in properties) {
				int memberIndex = classInfoStub.GetMemberIndex(mi.Name, false);
				if (memberIndex < 0 || !obj.Changed[memberIndex])
					continue;
				bool isModified;
				object value = GetNestedValue(classInfoStub, obj, memberIndex, out isModified);
				if (!IsPropertyToCheckLocking(option, mi, isModified || isGCDeleted))
					continue;
				if (mi.ReferenceType != null && value is XPObjectStub)
					value = ((XPObjectStub)value).Key;
				object parentValue = GetParentValue(parentObject, mi);
				if (IsModifiedInDataLayer(mi, value, parentValue))
					return true;
			}
			return false;
		}
		public static bool HasModified(XPClassInfo ci, IEnumerable properties, object theObject, object parentObject, OptimisticLockingBehavior option) {
			if (option == OptimisticLockingBehavior.NoLocking || option == OptimisticLockingBehavior.ConsiderOptimisticLockingField) return false;
			XPMemberInfo gc = ci.FindMember(GCRecordField.StaticName);
			bool isGCDeleted = gc != null && gc.GetValue(theObject) != null;
			foreach (XPMemberInfo mi in properties) {
				bool isModified = mi.GetModified(theObject);
				if (!IsPropertyToCheckLocking(option, mi, isModified || isGCDeleted))
					continue;
				object value = GetNestedValue(theObject, mi, isModified);
				object parentValue = GetParentValue(parentObject, mi);
				if (IsModifiedInDataLayer(mi, value, parentValue))
					return true;
			}
			return false;
		}
		static bool BytesAreEquals(byte[] oldValue, byte[] newValue) {
			if (oldValue == newValue) return true;
			if (oldValue.Length != newValue.Length) return false;
			int length = oldValue.Length;
			for (int i = 0; i < length; i++) {
				if (oldValue[i] != newValue[i]) return false;
			}
			return true;
		}
		public static bool IsModifiedInDataLayer(XPMemberInfo member, object nestedValue, object parentValue) {
			if (ReferenceEquals(nestedValue, parentValue))
				return false;
			else if (nestedValue is ValueType && parentValue is ValueType)
				return !Equals(nestedValue, parentValue);
			else if (nestedValue is string && parentValue is string)
				return !Equals(nestedValue, parentValue);
			else if (nestedValue is byte[] && parentValue is byte[])
				return !BytesAreEquals((byte[])nestedValue, (byte[])parentValue);
			else {
				if (nestedValue != null && parentValue != null && member.ReferenceType != null) {
					Type nestedType = nestedValue.GetType();
					Type parentType = parentValue.GetType();
					if (!nestedType.IsValueType && !nestedType.IsEnum && !nestedType.IsPrimitive && !nestedType.IsGenericType)
						nestedValue = member.ReferenceType.KeyProperty.GetValue(nestedValue);
					if (!parentType.IsValueType && !parentType.IsEnum && !parentType.IsPrimitive && !parentType.IsGenericType)
						parentValue = member.ReferenceType.KeyProperty.GetValue(parentValue);
					if (object.Equals(nestedValue, parentValue)) {
						return false;
					}
				}
			}
			return true;
		}
	}
	class ProcessingSave {
		List<object> processedObjectsList;
		Session session;
		public ProcessingSave(Session session, BatchWideDataHolder4Modification batchWideData) {
			this.session = session;
			this.BatchWideData = batchWideData;
			processedObjectsList = ListHelper.FromCollection(SaveOrderer.PrepareSaveOrder(session));
		}
		Session Session {
			get { return session; }
		}
		List<object> insertList = new List<object>();
		readonly BatchWideDataHolder4Modification BatchWideData;
		class InsertUpdate {
			public readonly object TheObject;
			public readonly MemberInfoCollection Updatable;
			public InsertUpdate(object theObject, MemberInfoCollection updatable) {
				TheObject = theObject;
				Updatable = updatable;
			}
		}
		void InternalInsertObject(List<ModificationStatement> insertList, List<InsertUpdate> updateList, object theObject) {
			XPClassInfo classInfo = Session.GetClassInfo(theObject);
			XPMemberInfo optimisticLock = classInfo.OptimisticLockField;
			if(optimisticLock != null && optimisticLock.GetValue(theObject) == null)
				optimisticLock.SetValue(theObject, 0);
			XPMemberInfo key = classInfo.KeyProperty;
			MemberInfoCollection insertable = new MemberInfoCollection(classInfo);
			MemberInfoCollection updatable = new MemberInfoCollection(classInfo);
			foreach(XPMemberInfo updateMember in Session.GetPropertiesListForUpdateInsert(theObject, false, false)) {
				if(key.MappingField == updateMember.MappingField && key != updateMember)
					continue;
				if(updateMember.ReferenceType != null) {
					object referredObject = updateMember.GetValue(theObject);
					if(referredObject != null && Session.IsNewObject(referredObject) && !BatchWideData.IsObjectAlreadyInserted(referredObject))
						updatable.Add(updateMember);
					else
						insertable.Add(updateMember);
				} else
					insertable.Add(updateMember);
			}
			if(key.IsAutoGenerate) {
				if(key.MemberType == typeof(Guid)) {
					object keyValue = key.GetValue(theObject);
					if(keyValue == null || (Guid)keyValue == Guid.Empty)
						key.SetValue(theObject, XpoDefault.NewGuid());
				} else if(!key.IsIdentity)
					throw new KeysAutogenerationNonSupportedTypeException(classInfo.FullName);
			} else {
				Session.CheckDuplicateObjectInIdentityMap(theObject);
			}
			BatchWideData.RegisterInsertedObject(theObject);
			List<ModificationStatement> queries = InsertQueryGenerator.GenerateInsert(Session.Dictionary, BatchWideData, theObject, insertable);
			InsertStatement rootQuery = (InsertStatement)queries[0];
			if(!ReferenceEquals(rootQuery.IdentityParameter, null))
				this.insertList.Add(theObject);
			insertList.AddRange(queries);
			if(updatable.Count > 0)
				updateList.Add(new InsertUpdate(theObject, updatable));
		}
		void InternalUpdateObject(List<ModificationStatement> updateList, object theObject) {
			Session.CheckDuplicateObjectInIdentityMap(theObject);
			XPClassInfo ci = Session.GetClassInfo(theObject);
			XPMemberInfo optimisticLock = ci.OptimisticLockField;
			MemberInfoCollection propsList = Session.GetPropertiesListForUpdateInsert(theObject, true, false);
			if (Session.LockingOption == LockingOption.Optimistic) {
				switch (ci.OptimisticLockingBehavior) {
					case OptimisticLockingBehavior.ConsiderOptimisticLockingField:
						if (optimisticLock != null) {
							updateList.AddRange(UpdateQueryGenerator.
								GenerateUpdate(Session.Dictionary, BatchWideData, theObject, propsList, ObjectGeneratorCriteriaSet.GetCriteriaSet(ci.IdClass.TableName, LockingHelper.GetLockingCriteria((int?)ci.OptimisticLockFieldInDataLayer.GetValue(theObject), optimisticLock))));
							return;
						}
						break;
					case OptimisticLockingBehavior.LockAll:
					case OptimisticLockingBehavior.LockModified:
						updateList.AddRange(UpdateQueryGenerator.
							GenerateUpdate(Session.Dictionary, BatchWideData, theObject, propsList, LockingHelper.GetLockingCriteria(ci, ci.PersistentProperties, theObject, ci.OptimisticLockingBehavior)));
						return;
				}
			}
			updateList.AddRange(UpdateQueryGenerator.GenerateUpdate(Session.Dictionary, BatchWideData, theObject, propsList));
		}
		public List<ModificationStatement> Process() {
			Dictionary<XPClassInfo, object> classes = new Dictionary<XPClassInfo, object>();
			List<XPClassInfo> usedClasses = new List<XPClassInfo>();
			foreach(object theObject in processedObjectsList) {
				XPClassInfo ci = Session.GetClassInfo(theObject);
				if(!classes.ContainsKey(ci)) {
					classes.Add(ci, null);
					usedClasses.Add(ci);
				}
			}
			Session.UpdateSchema(usedClasses.ToArray());
			List<ModificationStatement> insertList = new List<ModificationStatement>();
			List<InsertUpdate> updateList = new List<InsertUpdate>();
			List<object> updateObjectList = new List<object>();
			foreach(object theObject in processedObjectsList) {
				if(Session.IsNewObject(theObject))
					InternalInsertObject(insertList, updateList, theObject);
				else
					updateObjectList.Add(theObject);
			}
			foreach(InsertUpdate update in updateList)
				insertList.AddRange(UpdateQueryGenerator.GenerateUpdate(Session.Dictionary, BatchWideData, update.TheObject, update.Updatable));
			foreach(object theObject in updateObjectList)
				InternalUpdateObject(insertList, theObject);
			return insertList;
		}
		public void ProcessResults(ModificationResult result) {
			if(this.insertList.Count > 0) {
				SessionStateStack.Enter(Session, SessionState.ApplyIdentities);
				try {
					for(int i = 0; i < this.insertList.Count; i++) {
						Session.SetKeyValue(this.insertList[i], ((OperandValue)result.Identities[i]).Value);
					}
				} finally {
					SessionStateStack.Leave(Session, SessionState.ApplyIdentities);
				}
			}
			if(BatchWideData.InsertedObjects != null)
				foreach(object obj in BatchWideData.InsertedObjects)
					Session.RegisterInsertedObject(obj);
			if(BatchWideData.DeletedObjects != null) {
				foreach(object obj in BatchWideData.DeletedObjects) {
					if(!Session.IsNewObject(obj))
						SessionIdentityMap.UnregisterObject(Session, obj);
					IXPInvalidateableObject spoilableObject = obj as IXPInvalidateableObject;
					if(spoilableObject != null)
						spoilableObject.Invalidate();
				}
			}
		}
	}
	public class SaveOrderer {
		public readonly XPDictionary Dictionary;
		public readonly IDictionary NonSavedObjects;
		public readonly ObjectDictionary<ObjectSet> ReferredBy;
		public readonly ObjectDictionary<ObjectSet> References;
		public readonly List<object> Result;
		List<object> NonReferencedObjects;
		SaveOrderer(XPDictionary dictionary, ICollection newObjectsToSave) {
			this.Dictionary = dictionary;
			Result = new List<object>(newObjectsToSave.Count);
			ReferredBy = new ObjectDictionary<ObjectSet>(newObjectsToSave.Count);
			References = new ObjectDictionary<ObjectSet>(newObjectsToSave.Count);
			foreach(object obj in newObjectsToSave) {
				ReferredBy.Add(obj, new ObjectSet());
			}
			foreach(object obj in newObjectsToSave) {
				ObjectSet refers = new ObjectSet();
				References.Add(obj, refers);
				XPClassInfo ci = Dictionary.GetClassInfo(obj);
				foreach(XPMemberInfo r in ci.ObjectProperties) {
					if(r.IsDelayed && !XPDelayedProperty.GetDelayedPropertyContainer(obj, r).IsLoaded)
						continue;
					object referredObj = r.GetValue(obj);
					if(referredObj == null)
						continue;
					if(!ReferredBy.ContainsKey(referredObj))
						continue;
					refers.Add(referredObj);
					ReferredBy[referredObj].Add(obj);
				}
			}
			NonReferencedObjects = new List<object>();
			foreach(object obj in References.Keys) {
				if(References[obj].Count == 0)
					NonReferencedObjects.Add(obj);
			}
		}
		public void MarkObjectAsSaved(object obj) {
			ObjectSet referredBy;
			if(!ReferredBy.TryGetValue(obj, out referredBy))
				return;
			foreach(object r in referredBy) {
				ObjectSet list = References[r];
				list.Remove(obj);
				if(list.Count == 0)
					NonReferencedObjects.Add(r);
			}
			foreach(object r in References[obj]) {
				ReferredBy[r].Remove(obj);
			}
			ReferredBy.Remove(obj);
			References.Remove(obj);
			Result.Add(obj);
		}
		public void Do() {
			ProcessGoodObjects();
			Dictionary<object, ObjectSet>.Enumerator enFirst = References.GetEnumerator();
			while(enFirst.MoveNext()) {
				EleminateLoop(enFirst.Current.Key);
				enFirst = References.GetEnumerator();
			}
		}
		void ProcessGoodObjects() {
			for(int i = 0; i < NonReferencedObjects.Count; i++)
				MarkObjectAsSaved(NonReferencedObjects[i]);
			NonReferencedObjects.Clear();
		}
		void EleminateLoop(object nextObject) {
			List<object> list = new List<object>();
			ObjectDictionary<int> passedObjects = new ObjectDictionary<int>();
			for(int index = 0; ; index++) {
				passedObjects[nextObject] = index;
				list.Add(nextObject);
				IEnumerator enNext = References[nextObject].GetEnumerator();
				if(!enNext.MoveNext())
					throw new InvalidOperationException(Res.GetString(Res.Session_InternalXPOError));
				nextObject = enNext.Current;
				if(passedObjects.ContainsKey(nextObject)) {
					NonReferencedObjects.Add(nextObject);
					for(int i = 0; i < NonReferencedObjects.Count; i++) {
						object obj = NonReferencedObjects[i];
						MarkObjectAsSaved(obj);
						int j;
						if(passedObjects.TryGetValue(obj, out j)) {
							if(j < index)
								index = j;
						}
					}
					NonReferencedObjects.Clear();
					index--;
					if(index < 0)
						return;
					nextObject = list[index];
					list.RemoveRange(index, list.Count - index);
					index--;
				}
			}
		}
		static List<object> DoOrder(XPDictionary dictionary, ICollection newObjectsToSave) {
			SaveOrderer instance = new SaveOrderer(dictionary, newObjectsToSave);
			instance.Do();
			return instance.Result;
		}
		public static IList PrepareSaveOrder(Session session) {
			List<object> newObjects = new List<object>();
			List<object> oldObjects = new List<object>();
			foreach(object obj in session.GetObjectsToSave()) {
				if(session.IsObjectToDelete(obj)) {
				} else if(session.IsNewObject(obj)) {
					newObjects.Add(obj);
				} else {
					oldObjects.Add(obj);
				}
			}
			List<object> result = new List<object>();
			result.AddRange(DoOrder(session.Dictionary, newObjects));
			result.AddRange(oldObjects);
			return result;
		}
	}
	[Flags]
	public enum SessionState {
		Empty = 0,
		GetObjectsNonReenterant = 1,
		BeginTransaction = 2,
		CommitTransactionNonReenterant = 4,
		CommitChangesToDataLayer = 8,
		RollbackTransaction = 16,
		LoadingObjectsIntoNestedUow = 32,
		ReceivingObjectsFromNestedUow = 64,
		CancelEdit = 128,
		CreateObjectLoadingEnforcer = 256,
		OptimistickLockFieldsProcessing = 512,
		ApplyIdentities = 1024,
		LoadingStates = GetObjectsNonReenterant | ReceivingObjectsFromNestedUow | CancelEdit | CreateObjectLoadingEnforcer | OptimistickLockFieldsProcessing | ApplyIdentities,
		SavingStates = CommitTransactionNonReenterant | LoadingObjectsIntoNestedUow | OptimistickLockFieldsProcessing,
		ProhibitedForGetObjects = BeginTransaction | RollbackTransaction | CommitChangesToDataLayer | GetObjectsNonReenterant | CancelEdit | CreateObjectLoadingEnforcer | OptimistickLockFieldsProcessing,
		ProhibitedForCancelEdit = ProhibitedForGetObjects | LoadingObjectsIntoNestedUow,
		ProhibitedGeneric = BeginTransaction | RollbackTransaction | SavingStates | LoadingStates,
		SessionObjectsToSaveOrDeleteChanging = 2048,
		ProhibitedSessionObjectsToSaveOrDeleteChanging = CommitChangesToDataLayer | GetObjectsNonReenterant | LoadingObjectsIntoNestedUow,
		CommitChangesToDataLayerInner = 4096,
		CrossThreadFailure = 8192,
	}
	public static class SessionStateStack {
		public static void ThrowIfCantEnter(Session session, SessionState newState) {
			SessionState prohibited;
			switch(newState) {
				case SessionState.BeginTransaction:
				case SessionState.CommitTransactionNonReenterant:
				case SessionState.RollbackTransaction:
				case SessionState.LoadingObjectsIntoNestedUow:
				case SessionState.ReceivingObjectsFromNestedUow:
					prohibited = SessionState.ProhibitedGeneric;
					break;
				case SessionState.GetObjectsNonReenterant:
					prohibited = SessionState.ProhibitedForGetObjects;
					break;
				case SessionState.CancelEdit:
					prohibited = SessionState.ProhibitedForCancelEdit;
					break;
				case SessionState.CommitChangesToDataLayer:
				case SessionState.CommitChangesToDataLayerInner:
				case SessionState.CreateObjectLoadingEnforcer:
				case SessionState.OptimistickLockFieldsProcessing:
				case SessionState.ApplyIdentities:
					prohibited = SessionState.LoadingStates;
					break;
				case SessionState.SessionObjectsToSaveOrDeleteChanging:
					prohibited = SessionState.ProhibitedSessionObjectsToSaveOrDeleteChanging;
					break;
				default:
					throw new ArgumentException(string.Format(Res.GetString(Res.Session_UnexpectedState), newState.ToString()), "newState");
			}
			prohibited |= newState | SessionState.CrossThreadFailure;
			if((session._StateStack & prohibited) != SessionState.Empty) {
				string message = string.Format(Res.GetString(Res.Session_EnteringTheX0StateFromTheX1StateIsProhibit), newState, session._StateStack & prohibited, session._StateStack, session);
				if(newState == SessionState.GetObjectsNonReenterant) {
					if(IsInAnyOf(session, SessionState.LoadingStates)) {
						message += Res.GetString(Res.Session_MostProbablyYouAreTryingToInitiateAnObjectEx);
					} else if(IsInAnyOf(session, SessionState.SavingStates)) {
						message += Res.GetString(Res.Session_MostProbablyYouAreTryingToInitiateAnObject);
					}
				}
				throw new InvalidOperationException(message);
			}
		}
		static bool _SuppressCrossThreadFailuresDetection;
		[Obsolete("CrossThread Failures Detection suppressed")]
		public static bool SuppressCrossThreadFailuresDetection {
			get { return _SuppressCrossThreadFailuresDetection; }
			set { _SuppressCrossThreadFailuresDetection = value; }
		}
		public static void Enter(Session session, SessionState newState) {
			if(!_SuppressCrossThreadFailuresDetection) {
				if(session._StateStack == SessionState.Empty || newState == SessionState.GetObjectsNonReenterant || newState == SessionState.CommitTransactionNonReenterant) {
					Thread currentThread = Thread.CurrentThread;
					Thread previousThread = session._ExchangeThreadWatch(currentThread);
					if(previousThread != null && previousThread != currentThread) {
						session._StateStack |= SessionState.CrossThreadFailure;
						throw new InvalidOperationException(Res.GetString(Res.Session_CrossThreadFailureDetected, session));
					}
				}
			}
			ThrowIfCantEnter(session, newState);
			session._StateStack |= newState;
		}
		public static void Leave(Session session, SessionState state) {
			System.Diagnostics.Debug.Assert((session._StateStack & state) == state);
			session._StateStack &= ~state;
			if(session._StateStack == SessionState.Empty) {
				Thread previousThread = session._ExchangeThreadWatch(null);
				if(previousThread != null && previousThread != Thread.CurrentThread) {
					session._StateStack |= SessionState.CrossThreadFailure;
					throw new InvalidOperationException(Res.GetString(Res.Session_CrossThreadFailureDetected, session));
				}
			}
		}
		public static bool IsInAnyOf(Session session, SessionState states) {
			return (session._StateStack & states) != SessionState.Empty;
		}
	}
}
namespace DevExpress.Xpo {
	using System.Threading;
	using System.Collections;
	using System.Data;
	using System.Globalization;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using System.ComponentModel.Design;
	using System.Drawing;
	using DevExpress.Xpo.Metadata.Helpers;
	using DevExpress.Xpo.Helpers;
	using System.ComponentModel;
	using DevExpress.Xpo.Generators;
	using DevExpress.Xpo.DB;
	using DevExpress.Data.Filtering;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using DevExpress.Data.Filtering.Exceptions;
	using DevExpress.Xpo.Logger;
#if SL
	using DevExpress.Xpf.Collections;
	using DevExpress.Xpf.ComponentModel;
#endif
#if !CF && !SL
	using System.Diagnostics;
#endif
	public class ResolveSessionEventArgs : EventArgs {
		ISessionProvider provider;
		public ISessionProvider Session {
			get { return provider; }
			set { provider = value; }
		}
		public object Tag;
	}
	public delegate void ResolveSessionEventHandler(object sender, ResolveSessionEventArgs e);
	public class ObjectManipulationEventArgs : EventArgs {
		readonly object theObject;
		readonly Session session;
		public object Object { get { return theObject; } }
		public Session Session { get { return session; } }
		public ObjectManipulationEventArgs(object theObject) {
			this.theObject = theObject;
		}
		public ObjectManipulationEventArgs(Session session, object theObject) {
			this.theObject = theObject;
			this.session = session;
		}
	}
	public delegate void ObjectManipulationEventHandler(object sender, ObjectManipulationEventArgs e);
	public class ObjectsManipulationEventArgs : EventArgs {
		readonly ICollection objects;
		readonly Session session;
		public ICollection Objects { get { return objects; } }
		public Session Session { get { return session; } }
		public ObjectsManipulationEventArgs(Session session, ICollection objects) {
			this.session = session;
			this.objects = objects;
		}
	}
	public delegate void ObjectsManipulationEventHandler(object sender, ObjectsManipulationEventArgs e);
#if !SL
	public class SchemaInitEventArgs : EventArgs {
		XPClassInfo table;
		IDbCommand command;
		public XPClassInfo Table { get { return table; } }
		public IDbCommand Command { get { return command; } }
		public SchemaInitEventArgs(XPClassInfo table, IDbCommand command) {
			this.table = table;
			this.command = command;
		}
	}
	public delegate void SchemaInitEventHandler(object sender, SchemaInitEventArgs e);
#endif
	public class SessionManipulationEventArgs : EventArgs {
		Session session;
		public Session Session { get { return session; } }
		public SessionManipulationEventArgs(Session session) {
			this.session = session;
		}
	}
	public class SessionOperationFailEventArgs : EventArgs {
		Exception exception;
		bool handled;
		public Exception Exception { get { return exception; } }
		public bool Handled {
			get { return handled; }
			set { handled = value; }
		}
		public SessionOperationFailEventArgs(Exception exception) {
			this.exception = exception;
		}
	}
	public delegate void SessionManipulationEventHandler(object sender, SessionManipulationEventArgs e);
	public delegate object ProcessReferenceHandler(object theObject, XPMemberInfo member);
	public delegate void SessionOperationFailEventHandler(object sender, SessionOperationFailEventArgs e);
	public enum LockingOption { None, Optimistic };
#if !SL && !CF
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabOrmComponents)]
	[Designer("DevExpress.Utils.Design.BaseComponentDesignerSimple, " + AssemblyInfo.SRAssemblyDesign, typeof(IDesigner))]
	[DefaultProperty("Connection")]
	[DefaultEvent("SchemaInit")]
#endif
	[Description("Loads and saves persistent objects.")]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "Session")]
	public class Session : Component,
#if !SL
 ISupportInitialize,
#else
		IDisposable,
#endif
 IPersistentValueExtractor, ISessionProvider, IWideDataStorage, ICommandChannel {
		bool? trackPropertiesModifications;
		public const string LogCategory = "Session";
		public const string LogParam_SessionType = "sessionType";
		public const string LogParam_SessionID = "sessionID";
		public const string LogParam_ClassInfo = "classInfo";
		public const string LogParam_Expression = "expression";
		public const string LogParam_Criteria = "criteria";
		public const string LogParam_InTransaction = "inTransaction";
		public const string LogParam_Condition = "condition";
		public const string LogParam_Sorting = "sorting";
		public const string LogParam_SkipSelectedRecords = "skipSelectedRecords";
		public const string LogParam_TopSelectedRecords = "topSelectedRecords";
		public const string LogParam_SelectDeleted = "selectDeleted";
		public const string LogParam_TheObject = "theObject";
		public const string LogParam_Props = "propeties";
		public const string LogParam_Objects = "objects";
		public const string LogParam_Property = "property";
		public const string LogParam_ObjectID = "objectID";
		public const string LogParam_AlwaysGetFromDb = "alwaysGetFromDb";
		public const string LogParam_CriteriaEvaluationBehavior = "criteriaEvaluationBehavior";
		public const string LogParam_Members = "members";
		public const string LogParam_SprocName = "sprocName";
		public const string LogParam_Parameters = "parameters";
		public const string LogParam_Sql = "sql";
		public const string LogParam_GroupProps = "groupProperties";
		public const string LogParam_GroupCriteria = "groupCriteria";
		public const string LogParam_IsInTransactionMode = "IsInTransactionMode";
		internal SessionState _StateStack;
		Thread _ThreadWatch;
		internal Thread _ExchangeThreadWatch(Thread newThreadWatchValue) {
			return Interlocked.Exchange(ref _ThreadWatch, newThreadWatchValue);
		}
#if !SL
		IDbConnection _connection;
		bool closeConnectionOnDisconnect;
#endif
		IdentityMapBehavior _IdentityMapBehavior = IdentityMapBehavior.Default;
		bool? caseSensitive;
		string _connectionString;
		IDisposable[] _DisposeOnDisconnect;
		AutoCreateOption _autoCreateOption = AutoCreateOption.DatabaseAndSchema;
		LockingOption _lockingOption = LockingOption.Optimistic;
		OptimisticLockingReadBehavior _OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.Default;
		bool trackingChanges = false;
#if !CF && !SL
		protected static XPDictionary CreateDesignTimeDictionary(IServiceProvider provider) {
			return new DesignTimeReflection(provider);
		}
#endif
#if !SL
		protected virtual void BeginInit() { }
		protected virtual void EndInit() { }
		void ISupportInitialize.BeginInit() {
			BeginInit();
		}
		void ISupportInitialize.EndInit() {
			EndInit();
		}
#endif
		static bool inTransactionMode = false;
#if !SL
	[DevExpressXpoLocalizedDescription("SessionInTransactionMode")]
#endif
		public static bool InTransactionMode { get { return inTransactionMode; } set { inTransactionMode = value; } }
		protected virtual bool IsInTransactionMode { get { return InTransactionMode; } }
#if !SL
	[DevExpressXpoLocalizedDescription("SessionCaseSensitive")]
#endif
		public bool CaseSensitive {
			get { return caseSensitive.HasValue ? caseSensitive.Value : XpoDefault.DefaultCaseSensitive; }
			set { caseSensitive = value; }
		}
		public bool TrackPropertiesModifications {
			get { return trackPropertiesModifications.HasValue ? trackPropertiesModifications.Value : XpoDefault.TrackPropertiesModifications; }
			set { trackPropertiesModifications = value; }
		}
#if !CF
		bool ShouldSerializeCaseSensitive() {
			return caseSensitive.HasValue;
		}
		void ResetCaseSensitive() {
			caseSensitive = null;
		}
#endif
		public static void SetAsyncRequestPriority(object asyncResult, AsyncRequestPriority priority) {
			AsyncRequest request = asyncResult as AsyncRequest;
			if(request == null)
				return;
			request.SetPriority(priority);
		}
		public static void CancelAsyncRequest(object asyncResult) {
			AsyncRequest request = asyncResult as AsyncRequest;
			if(request == null)
				return;
			request.Cancel();
		}
		public static WaitForAsyncOperationResult WaitForAsyncRequestComplete(Session session, object asyncResult) {
			AsyncRequest request = asyncResult as AsyncRequest;
			if(request == null)
				return WaitForAsyncOperationResult.OperationComplete;
			return session.AsyncExecuteQueue.WaitForAsyncOperationEnd(request);
		}
		XPObjectTypesManager typesManager;
		IObjectLayer objectLayer;
		internal SessionIdentityMap _IdentityMap;
		public List<object[]> SelectData(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, bool selectDeleted, int topSelectedRecords, SortingCollection sorting) {
			return SelectData(classInfo, properties, criteria, selectDeleted, 0, topSelectedRecords, sorting);
		}
		public List<object[]> SelectData(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, bool selectDeleted, int skipSelectedRecords, int topSelectedRecords, SortingCollection sorting) {
			return SelectData(classInfo, properties, criteria, null, null, selectDeleted, skipSelectedRecords, topSelectedRecords, sorting);
		}
		static ExpandedCriteriaHolder ExpandToLogical(CriteriaOperator criteria, XPClassInfo classInfo, bool doDetectPostProcessing, IPersistentValueExtractor persistentValueExtractor) {
			ExpandedCriteriaHolder criteriaExpanded = PersistentCriterionExpander.ExpandToLogical(persistentValueExtractor, classInfo, criteria, doDetectPostProcessing);
			if(criteriaExpanded.RequiresPostProcessing)
				throw new ArgumentException(Res.GetString(Res.PersistentAliasExpander_NonPersistentCriteria, CriteriaOperator.ToString(criteria), criteriaExpanded.PostProcessingCause));
			else
				return criteriaExpanded;
		}
		static ExpandedCriteriaHolder ExpandToValue(CriteriaOperator criteria, XPClassInfo classInfo, bool doDetectPostProcessing, IPersistentValueExtractor persistentValueExtractor) {
			ExpandedCriteriaHolder criteriaExpanded = PersistentCriterionExpander.ExpandToValue(persistentValueExtractor, classInfo, criteria, doDetectPostProcessing);
			if(criteriaExpanded.RequiresPostProcessing)
				throw new ArgumentException(Res.GetString(Res.PersistentAliasExpander_NonPersistentCriteria, CriteriaOperator.ToString(criteria), criteriaExpanded.PostProcessingCause));
			else
				return criteriaExpanded;
		}
		static bool IsNull(object op) {
			return op == null;
		}
		public List<object[]> SelectData(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, bool selectDeleted, int topSelectedRecords, SortingCollection sorting) {
			return SelectData(classInfo, properties, criteria, groupProperties, groupCriteria, selectDeleted, 0, topSelectedRecords, sorting);
		}
		public List<object[]> SelectData(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, bool selectDeleted, int skipSelectedRecords, int topSelectedRecords, SortingCollection sorting) {
			return LogManager.Log<List<object[]>>(LogCategory, () => {
				if(IsInTransactionMode) {
					return InTransactionLoader.SelectData(this, classInfo, properties, criteria, groupProperties, groupCriteria, selectDeleted, skipSelectedRecords, topSelectedRecords, sorting);
				}
				return SelectDataInternal(classInfo, properties, criteria, groupProperties, groupCriteria, selectDeleted, skipSelectedRecords, topSelectedRecords, sorting);
			}, (d) => {
				return CreateLogMessage("Executing SelectData()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_IsInTransactionMode, this.IsInTransactionMode,
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Props, LogMessage.CriteriaOperatorCollectionToString(properties),
				LogParam_Criteria, ReferenceEquals(criteria, null) ? "null" : criteria.ToString(),
				LogParam_GroupProps, LogMessage.CriteriaOperatorCollectionToString(groupProperties),
				LogParam_GroupCriteria, ReferenceEquals(groupCriteria, null) ? "null" : groupCriteria.ToString(),
				LogParam_SelectDeleted, selectDeleted,
				LogParam_SkipSelectedRecords, skipSelectedRecords,
				LogParam_TopSelectedRecords, topSelectedRecords,
				LogParam_Sorting, LogHelpers.SortingCollectionToString(sorting)
			});
			});
		}
		public List<object[]> SelectDataInTransaction(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, bool selectDeleted, int skipSelectedRecords, int topSelectedRecords, SortingCollection sorting) {
			return LogManager.Log<List<object[]>>(LogCategory, () => {
				return InTransactionLoader.SelectData(this, classInfo, properties, criteria, groupProperties, groupCriteria, selectDeleted, skipSelectedRecords, topSelectedRecords, sorting);
			}, (d) => {
				return CreateLogMessage("Executing SelectDataInTransaction()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Props, LogMessage.CriteriaOperatorCollectionToString(properties),
				LogParam_Criteria, ReferenceEquals(criteria, null) ? "null" : criteria.ToString(),
				LogParam_GroupProps, LogMessage.CriteriaOperatorCollectionToString(groupProperties),
				LogParam_GroupCriteria, ReferenceEquals(groupCriteria, null) ? "null" : groupCriteria.ToString(),
				LogParam_SelectDeleted, selectDeleted,
				LogParam_SkipSelectedRecords, skipSelectedRecords,
				LogParam_TopSelectedRecords, topSelectedRecords,
				LogParam_Sorting, LogHelpers.SortingCollectionToString(sorting)
			});
			});
		}
		internal virtual List<object[]> SelectDataInternal(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, bool selectDeleted, int skipSelectedRecords, int topSelectedRecords, SortingCollection sorting) {
			List<object[]> result = PrepareSelectData(classInfo, ref properties, ref criteria, ref groupProperties, ref groupCriteria, ref sorting, true, this);
			if(result == null)
				result = ObjectLayer.SelectData(this, new ObjectsQuery(classInfo, criteria, sorting, skipSelectedRecords, topSelectedRecords, new CollectionCriteriaPatcher(selectDeleted, TypesManager), true), properties, groupProperties, groupCriteria);
			return result;
		}
		internal ObjectsQuery[] PrepareQueries(ObjectsQuery[] queries, bool doDetectPostProcessing, IPersistentValueExtractor persistentValueExtractor) {
			int queriesCount = queries == null ? 0 : queries.Length;
			ObjectsQuery[] result = new ObjectsQuery[queriesCount];
			TypesManager.EnsureIsTypedObjectValid();
			for(int i = 0; i < queriesCount; i++) {
				result[i] = PrepareQuery(queries[i], doDetectPostProcessing, persistentValueExtractor);
			}
			if(queriesCount > 0) {
				XPClassInfo[] classInfoList = new XPClassInfo[queriesCount];
				for(int i = 0; i < queriesCount; i++) {
					classInfoList[i] = result[i].ClassInfo;
				}
				UpdateSchema(classInfoList);
			}
			return result;
		}
		ObjectsQuery PrepareQuery(ObjectsQuery objectsQuery, bool doDetectPostProcessing, IPersistentValueExtractor persistentValueExtractor) {
			XPClassInfo classInfo = objectsQuery.ClassInfo;
			CriteriaOperator criteria = objectsQuery.Criteria;
			if(!IsNull(criteria)) {
				ExpandedCriteriaHolder criteriaHolder = ExpandToLogical(criteria, classInfo, doDetectPostProcessing, persistentValueExtractor);
				criteria = criteriaHolder.ExpandedCriteria;
			}
			SortingCollection sorting = objectsQuery.Sorting;
			if(!IsNull(sorting)) {
				SortingCollection expanded = new SortingCollection();
				for(int i = 0; i < sorting.Count; i++)
					expanded.Add(new SortProperty(ExpandToValue(sorting[i].Property, classInfo, doDetectPostProcessing, persistentValueExtractor).ExpandedCriteria, sorting[i].Direction));
				sorting = expanded;
			}
			return new ObjectsQuery(classInfo, criteria, sorting, objectsQuery.SkipSelectedRecords, objectsQuery.TopSelectedRecords, objectsQuery.CollectionCriteriaPatcher, objectsQuery.Force);
		}
		internal List<object[]> PrepareSelectData(XPClassInfo classInfo, ref CriteriaOperatorCollection properties, ref CriteriaOperator criteria, ref CriteriaOperatorCollection groupProperties, ref CriteriaOperator groupCriteria, ref SortingCollection sorting, bool doDetectPostProcessing, IPersistentValueExtractor persistentValueExtractor) {
			UpdateSchema(classInfo);
			TypesManager.EnsureIsTypedObjectValid();
			return PrepareSelectDataInternal(classInfo, ref properties, ref criteria, ref groupProperties, ref groupCriteria, ref sorting, doDetectPostProcessing, persistentValueExtractor);
		}
		internal static List<object[]> PrepareSelectDataInternal(XPClassInfo classInfo, ref CriteriaOperatorCollection properties, ref CriteriaOperator criteria, ref CriteriaOperatorCollection groupProperties, ref CriteriaOperator groupCriteria, ref SortingCollection sorting, bool doDetectPostProcessing, IPersistentValueExtractor persistentValueExtractor) {
			if(!IsNull(criteria)) {
				ExpandedCriteriaHolder criteriaHolder = ExpandToLogical(criteria, classInfo, doDetectPostProcessing, persistentValueExtractor);
				if(criteriaHolder.IsFalse)
					return new List<object[]>(0);
				else
					criteria = criteriaHolder.ExpandedCriteria;
			}
			if(!IsNull(properties)) {
				CriteriaOperatorCollection expanded = new CriteriaOperatorCollection();
				for(int i = 0; i < properties.Count; i++)
					expanded.Add(ExpandToValue(properties[i], classInfo, doDetectPostProcessing, persistentValueExtractor).ExpandedCriteria);
				properties = expanded;
			}
			if(!IsNull(groupProperties)) {
				CriteriaOperatorCollection expanded = new CriteriaOperatorCollection();
				for(int i = 0; i < groupProperties.Count; i++)
					expanded.Add(ExpandToValue(groupProperties[i], classInfo, doDetectPostProcessing, persistentValueExtractor).ExpandedCriteria);
				groupProperties = expanded;
			}
			if(!IsNull(groupCriteria)) {
				ExpandedCriteriaHolder criteriaHolder = ExpandToLogical(groupCriteria, classInfo, doDetectPostProcessing, persistentValueExtractor);
				if(criteriaHolder.IsFalse)
					return new List<object[]>(0);
				else
					groupCriteria = criteriaHolder.ExpandedCriteria;
			}
			if(!IsNull(sorting)) {
				SortingCollection expanded = new SortingCollection();
				for(int i = 0; i < sorting.Count; i++)
					expanded.Add(new SortProperty(ExpandToValue(sorting[i].Property, classInfo, doDetectPostProcessing, persistentValueExtractor).ExpandedCriteria, sorting[i].Direction));
				sorting = expanded;
			}
			return null;
		}
		public object SelectDataAsync(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, bool selectDeleted, int skipSelectedRecords, int topSelectedRecords, SortingCollection sorting, AsyncSelectDataCallback callback) {
			if(IsInTransactionMode) {
				InTransactionLoader.SelectDataAsync(this, classInfo, properties, criteria, groupProperties, groupCriteria, selectDeleted, skipSelectedRecords, topSelectedRecords, sorting, callback);
				return null;
			}
			return SelectDataAsyncInternal(classInfo, properties, criteria, groupProperties, groupCriteria, selectDeleted, skipSelectedRecords, topSelectedRecords, sorting, callback);
		}
		internal virtual object SelectDataAsyncInternal(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, bool selectDeleted, int skipSelectedRecords, int topSelectedRecords, SortingCollection sorting, AsyncSelectDataCallback callback) {
			if(SynchronizationContext.Current == null)
				throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Async_OperationCannotBePerformedBecauseNoSyncContext));
			if(callback == null)
				throw new ArgumentNullException();
			List<object[]> result = PrepareSelectData(classInfo, ref properties, ref criteria, ref groupProperties, ref groupCriteria, ref sorting, true, this);
			if(result != null) {
				return ObjectLayer.SelectDataAsync(this, null, null, null, null, callback);
			} else {
				return ObjectLayer.SelectDataAsync(this, new ObjectsQuery(classInfo, criteria, sorting, skipSelectedRecords, topSelectedRecords, new CollectionCriteriaPatcher(selectDeleted, TypesManager), true), properties, groupProperties, groupCriteria, callback);
			}
		}
		public object Evaluate(Type objectType, CriteriaOperator expression, CriteriaOperator criteria) {
			return Evaluate(GetClassInfo(objectType), expression, criteria);
		}
		class FalseCriteriaTopLevelEvaluatorDescriptor : EvaluatorContextDescriptor {
			public override IEnumerable GetCollectionContexts(object source, string collectionName) {
				return new object[0];
			}
			public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
				return new EvaluatorContext(this, source);
			}
			public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) {
				return null;
			}
			public override IEnumerable GetQueryContexts(object source, string queryTypeName, CriteriaOperator condition, int top) {
				return new object[0];
			}
			public override bool IsTopLevelCollectionSource {
				get {
					return true;
				}
			}
		}
		public object Evaluate(XPClassInfo classInfo, CriteriaOperator expression, CriteriaOperator criteria) {
			return Evaluate(classInfo, expression, criteria, false);
		}
		public object EvaluateInTransaction(XPClassInfo classInfo, CriteriaOperator expression, CriteriaOperator criteria) {
			return Evaluate(classInfo, expression, criteria, true);
		}
		object Evaluate(XPClassInfo classInfo, CriteriaOperator expression, CriteriaOperator criteria, bool inTransaction) {
			return LogManager.Log<object>(LogCategory, () => {
				List<object[]> result = null;
				if(inTransaction) {
					CriteriaOperatorCollection props = new CriteriaOperatorCollection(1);
					props.Add(expression);
					result = SelectDataInTransaction(classInfo, props, criteria, null, null, false, 0, 1, null);
				}else{
					result = PrepareEvaluate(classInfo, ref expression, ref criteria, this, CaseSensitive);
					UpdateSchema(false, classInfo);
					TypesManager.EnsureIsTypedObjectValid();
					CriteriaOperatorCollection props = new CriteriaOperatorCollection(1);
					props.Add(expression);
					if(result == null) {
						result = SelectData(classInfo, props, criteria, null, null, false, 0, 1, null);
					}
				}
				return (result == null) || (result.Count == 0) || (result[0] == null) || (result[0].Length == 0) ? null : result[0][0];
			}, (d) => {
				return CreateLogMessage("Executing Evaluate()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_IsInTransactionMode, this.IsInTransactionMode,
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Expression, ReferenceEquals(expression, null) ? "null" : expression.ToString(),
				LogParam_Criteria, ReferenceEquals(criteria, null) ? "null" : criteria.ToString(),
				LogParam_InTransaction, inTransaction
			});
			});
		}
		internal static List<object[]> PrepareEvaluate(XPClassInfo classInfo, ref CriteriaOperator expression, ref CriteriaOperator criteria, IPersistentValueExtractor extractor, bool caseSensitive) {
			ExpandedCriteriaHolder criteriaHolder = ExpandToLogical(criteria, classInfo, true, extractor);
			if(criteriaHolder.IsFalse) {
				ExpressionEvaluator evaluator = new ExpressionEvaluator(new FalseCriteriaTopLevelEvaluatorDescriptor(), expression, caseSensitive, classInfo.Dictionary.CustomFunctionOperators);
				object rv = evaluator.Evaluate(null);
				return new List<object[]>(new object[][] { new object[] { rv } });
			} else {
				expression = ExpandToValue(expression, classInfo, true, extractor).ExpandedCriteria;
				criteria = ExpandedCriteriaHolder.IfNeededConvertToLogicalOperator(criteria);
				return null;
			}
		}
		public object Evaluate<ClassType>(CriteriaOperator expression, CriteriaOperator criteria) {
			return Evaluate(typeof(ClassType), expression, criteria);
		}
		AsyncExecuteQueue asyncExecuteQueue;
		internal AsyncExecuteQueue AsyncExecuteQueue {
			get {
				if(asyncExecuteQueue == null)
					asyncExecuteQueue = new AsyncExecuteQueue();
				return asyncExecuteQueue;
			}
		}
		internal WaitForAsyncOperationResult WaitForAsyncOperationEnd() {
			if(asyncExecuteQueue != null)
				return asyncExecuteQueue.WaitForAsyncOperationEnd();
			else
				return WaitForAsyncOperationResult.EmptyQueue;
		}
		internal ICollection GetObjects(ObjectsQuery query) {
			return GetObjects(new ObjectsQuery[] { query })[0];
		}
		public virtual ICollection[] GetObjects(ObjectsQuery[] queries) {
			return LogManager.Log<ICollection[]>(LogCategory, () => {
				if(IsInTransactionMode) {
					return InTransactionLoader.GetObjects(this, queries);
				}
				return GetObjectsInternal(queries);
			}, (d) => {
				LogMessage lm = new LogMessage(LogMessageType.SessionEvent, "Executing GetObjects()", d);
				lm.ParameterList.Add(new LogMessageParameter(LogParam_SessionType, this.GetType().ToString()));
				lm.ParameterList.Add(new LogMessageParameter(LogParam_SessionID, this.ToString()));
				lm.ParameterList.Add(new LogMessageParameter(LogParam_IsInTransactionMode, IsInTransactionMode));
				for(int i = 0; i < queries.Length; i++) {
					lm.ParameterList.Add(new LogMessageParameter("query #" + i, LogHelpers.QueryToString(queries[i])));
				}
				return lm;
			});
		}
		internal virtual ICollection[] GetObjectsInternal(ObjectsQuery[] queries) {
			TypesManager.EnsureIsTypedObjectValid();
			return ObjectLayer.LoadObjects(this, queries);
		}
		public virtual object GetObjectsAsync(ObjectsQuery[] queries, AsyncLoadObjectsCallback callback) {
			return LogManager.Log<object>(LogCategory, () => {
				if(IsInTransactionMode) {
					InTransactionLoader.GetObjectsAsync(this, queries, callback);
					return null;
				}
				return GetObjectsInternalAsync(queries, callback);
			}, (d) => {
				LogMessage lm;
				lm = new LogMessage(LogMessageType.SessionEvent, "Executing GetObjectsAsync()", d);
				lm.ParameterList.Add(new LogMessageParameter(LogParam_SessionType, this.GetType().ToString()));
				lm.ParameterList.Add(new LogMessageParameter(LogParam_SessionID, this.ToString()));
				lm.ParameterList.Add(new LogMessageParameter(LogParam_IsInTransactionMode, IsInTransactionMode));
				for(int i = 0; i < queries.Length; i++) {
					lm.ParameterList.Add(new LogMessageParameter("query #" + i, LogHelpers.QueryToString(queries[i])));
				}
				return lm;
			});
		}
		internal virtual object GetObjectsInternalAsync(ObjectsQuery[] queries, AsyncLoadObjectsCallback callback) {
			TypesManager.EnsureIsTypedObjectValid();
			return ObjectLayer.LoadObjectsAsync(this, queries, callback);
		}
		public object GetObjectsAsync(XPClassInfo classInfo, CriteriaOperator criteria, SortingCollection sorting, int topSelectedRecords, bool selectDeleted, bool force, AsyncLoadObjectsCallback callback) {
			return GetObjectsAsync(classInfo, criteria, sorting, 0, topSelectedRecords, selectDeleted, force, callback);
		}
		public object GetObjectsAsync(XPClassInfo classInfo, CriteriaOperator criteria, SortingCollection sorting, int skipSelectedRecords, int topSelectedRecords, bool selectDeleted, bool force, AsyncLoadObjectsCallback callback) {
			return GetObjectsAsync(new ObjectsQuery[] { new ObjectsQuery(classInfo, criteria, sorting, skipSelectedRecords, topSelectedRecords, new CollectionCriteriaPatcher(selectDeleted, TypesManager), force) }, callback);
		}
		[Obsolete("Use the method's overload with 'bool selectDeleted' parameter instead of 'CollectionCriteriaPatcher collectionCriteriaPatcher'")]
		public ICollection GetObjects(XPClassInfo classInfo, CriteriaOperator criteria, SortingCollection sorting, int topSelectedRecords, CollectionCriteriaPatcher collectionCriteriaPatcher, bool force) {
			return GetObjects(new ObjectsQuery(classInfo, criteria, sorting, topSelectedRecords, collectionCriteriaPatcher, force));
		}
		public ICollection GetObjects(XPClassInfo classInfo, CriteriaOperator criteria, SortingCollection sorting, int topSelectedRecords, bool selectDeleted, bool force) {
			return GetObjects(classInfo, criteria, sorting, 0, topSelectedRecords, selectDeleted, force);
		}
		public ICollection GetObjects(XPClassInfo classInfo, CriteriaOperator criteria, SortingCollection sorting, int skipSelectedRecords, int topSelectedRecords, bool selectDeleted, bool force) {
			return GetObjects(new ObjectsQuery(classInfo, criteria, sorting, skipSelectedRecords, topSelectedRecords, new CollectionCriteriaPatcher(selectDeleted, TypesManager), force));
		}
		[Obsolete("Use the method's overload without 'bool caseSensitive' parameter")]
		public ICollection GetObjectsInTransaction(XPClassInfo classInfo, CriteriaOperator condition, bool selectDeleted, bool caseSensitive) {
			return InTransactionLoader.GetObjects(this, new ObjectsQuery[] { new ObjectsQuery(classInfo, condition, null, 0, new CollectionCriteriaPatcher(selectDeleted, TypesManager), false) }, caseSensitive)[0];
		}
		public ICollection GetObjectsInTransaction(XPClassInfo classInfo, CriteriaOperator condition, bool selectDeleted) {
			return GetObjectsInTransaction(classInfo, condition, null, 0, 0, selectDeleted);
		}
		public ICollection GetObjectsInTransaction(XPClassInfo classInfo, CriteriaOperator condition, SortingCollection sorting, int skipSelectedRecords, int topSelectedRecords, bool selectDeleted) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				return InTransactionLoader.GetObjects(this, new ObjectsQuery[] { new ObjectsQuery(classInfo, condition, sorting, skipSelectedRecords, topSelectedRecords, new CollectionCriteriaPatcher(selectDeleted, TypesManager), false) })[0];
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsInTransaction()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Condition, ReferenceEquals(condition, null) ? "null" : condition.ToString(),
				LogParam_Sorting, LogHelpers.SortingCollectionToString(sorting),
				LogParam_SkipSelectedRecords, skipSelectedRecords,
				LogParam_TopSelectedRecords, topSelectedRecords,
				LogParam_SelectDeleted, selectDeleted
			});
			});
		}
		public object GetObjectsInTransactionAsync(XPClassInfo classInfo, CriteriaOperator condition, bool selectDeleted, AsyncLoadObjectsCallback callback) {
			return GetObjectsInTransactionAsync(classInfo, condition, null, 0, 0, selectDeleted, callback);
		}
		public object GetObjectsInTransactionAsync(XPClassInfo classInfo, CriteriaOperator condition, SortingCollection sorting, int skipSelectedRecords, int topSelectedRecords, bool selectDeleted, AsyncLoadObjectsCallback callback) {
			return LogManager.Log<object>(LogCategory, () => {
				InTransactionLoader.GetObjectsAsync(this, new ObjectsQuery[] { new ObjectsQuery(classInfo, condition, sorting, skipSelectedRecords, topSelectedRecords, new CollectionCriteriaPatcher(selectDeleted, TypesManager), false) }, callback);
				return null;
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsInTransactionAsync()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Condition, ReferenceEquals(condition, null) ? "null" : condition.ToString(),
				LogParam_Sorting, LogHelpers.SortingCollectionToString(sorting),
				LogParam_SkipSelectedRecords, skipSelectedRecords,
				LogParam_TopSelectedRecords, topSelectedRecords,
				LogParam_SelectDeleted, selectDeleted
			});
			});
		}
		[Browsable(false)]
		public bool IsObjectsLoading {
			get {
				return SessionStateStack.IsInAnyOf(this, SessionState.LoadingStates);
			}
		}
		[Browsable(false)]
		public bool IsObjectsSaving {
			get {
				return SessionStateStack.IsInAnyOf(this, SessionState.SavingStates);
			}
		}
		protected internal virtual object[] LoadDelayedProperties(object theObject, MemberPathCollection props) {
			return LogManager.Log<object[]>(LogCategory, () => {
				ThrowIfObjectFromDifferentSession(theObject);
				return ((IObjectLayerEx)ObjectLayer).LoadDelayedProperties(this, theObject, props);
			}, (d) => {
				return CreateLogMessage("Executing LoadDelayedProperties()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_TheObject, GetObjectString(theObject),
				LogParam_Props, LogMessage.CollectionToString<MemberInfoCollection>(props, mic => mic.ToString() )
			});
			});
		}
		protected internal virtual ObjectDictionary<object> LoadDelayedProperties(IList objects, XPMemberInfo property) {
			return LogManager.Log<ObjectDictionary<object>>(LogCategory, () => {
				return ((IObjectLayerEx)ObjectLayer).LoadDelayedProperties(this, objects, property);
				;
			}, (d) => {
				return CreateLogMessage("Executing LoadDelayedProperties()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Objects, LogMessage.CollectionToString(objects, theObject => GetObjectString(theObject)),
				LogParam_Property, property == null ? "null" : property.Name
			});
			});
		}
		public void Reload(object theObject) {
			LogManager.Log(LogCategory, () => {
				ThrowIfObjectFromDifferentSession(theObject);
				RemoveFromLists(theObject);
				if(IsNewObject(theObject))
					return;
				XPClassInfo ci = GetClassInfo(theObject);
				if(GetObjects(ci, new BinaryOperator(new OperandProperty(ci.KeyProperty.Name), new OperandValue(theObject), BinaryOperatorType.Equal), null, 0, 1, true, true).Count == 0)
					throw new CannotLoadObjectsException(ci.FullName + "(" + GetKeyValue(theObject) + ")");
				if(ci.HasModifications(theObject)) {
					objectsMarkedSaved.Add(theObject);
				}
			}, (d) => {
				return CreateLogMessage("Executing Reload(theObject)", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_TheObject, GetObjectString(theObject)
			});
			}, null);
		}
		public void Reload(object theObject, bool forceAggregatesReload) {
			Reload(theObject);
			if(forceAggregatesReload) {
				foreach(XPMemberInfo mi in GetClassInfo(theObject).ObjectProperties) {
					if(mi.IsAggregated) {
						object nestedObject = mi.GetValue(theObject);
						if(nestedObject != null) {
							Reload(nestedObject, forceAggregatesReload);
						}
					}
				}
				foreach(XPMemberInfo c in GetClassInfo(theObject).AssociationListProperties) {
					if(c.IsAggregated) {
						IList collection = (IList)c.GetValue(theObject);
						if(c.IsCollection) {
							((XPBaseCollection)collection).Reload();
						} else if(collection is IXPUnloadableAssociationList) {
							((IXPUnloadableAssociationList)collection).Unload();
						}
						foreach(object nestedObject in ListHelper.FromCollection(collection)) {
							if(nestedObject != null) {
								Reload(nestedObject, forceAggregatesReload);
							}
						}
					}
				}
			}
		}
		static bool _SuppressThrowingAssociationCollectionWithDisabledLoading;
#if !SL
	[DevExpressXpoLocalizedDescription("SessionSuppressThrowingAssociationCollectionWithDisabledLoading")]
#endif
		[Obsolete("Using that property will suppress detection of some potential problems with associated collections. Use it at your own risk.")]
		public static bool SuppressThrowingAssociationCollectionWithDisabledLoading {
			get { return _SuppressThrowingAssociationCollectionWithDisabledLoading; }
			set { _SuppressThrowingAssociationCollectionWithDisabledLoading = value; }
		}
		void PreProcessSavedList() {
			ObjectSet processedItems = new ObjectSet();
			while(GetObjectsToSave().Count != processedItems.Count) {
				foreach(object obj in ListHelper.FromCollection(GetObjectsToSave())) {
					if(processedItems.Contains(obj))
						continue;
					processedItems.Add(obj);
					XPClassInfo ci = GetClassInfo(obj);
					TriggerObjectSaving(obj);
					if(ci.IsTypedObject)
						ci.GetMember(XPObjectType.ObjectTypePropertyName).SetValue(obj, GetObjectType(obj));
					foreach(XPMemberInfo mi in ci.ObjectProperties) {
						if(mi.IsDelayed && !XPDelayedProperty.GetDelayedPropertyContainer(obj, mi).IsLoaded)
							continue;
						object refObject = mi.GetValue(obj);
						if(refObject == null)
							continue;
						ThrowIfObjectFromDifferentSession(refObject);
						if(IsNewObject(refObject) || (mi.IsAggregated && !IsUnitOfWork)) {
							Save(refObject);
						}
					}
					foreach(XPMemberInfo mi in ci.CollectionProperties) {
						XPBaseCollection collection = (XPBaseCollection)mi.GetValue(obj);
						if(collection == null)
							continue;
						if(collection.IsLoaded && !collection.LoadingEnabled && !_SuppressThrowingAssociationCollectionWithDisabledLoading)
							throw new InvalidOperationException(Res.GetString(Res.Session_AssociationCollectionWithDisabledLoading));
						XPRefCollectionHelper.GetRefCollectionHelperChecked<XPRefCollectionHelper>(collection, obj, mi).Save();
					}
				}
			}
			foreach(object obj in new ArrayList(GetObjectsToDelete())) {
				if(processedItems.Contains(obj))
					continue;
				processedItems.Add(obj);
				XPClassInfo ci = GetClassInfo(obj);
				foreach(XPMemberInfo mi in ci.CollectionProperties) {
					XPBaseCollection collection = (XPBaseCollection)mi.GetValue(obj);
					if(collection == null)
						continue;
					if(collection.IsLoaded && !collection.LoadingEnabled && !_SuppressThrowingAssociationCollectionWithDisabledLoading)
						throw new InvalidOperationException(Res.GetString(Res.Session_AssociationCollectionWithDisabledLoading));
					XPRefCollectionHelper.GetRefCollectionHelperChecked<XPRefCollectionHelper>(collection, obj, mi).Save();
				}
			}
		}
		void ProcessingProcessNextObject(ObjectSet markedObjectsHolder, object theObject) {
			ThrowIfObjectFromDifferentSession(theObject);
			XPClassInfo ci = GetClassInfo(theObject);
			ci.CheckAbstractReference();
			if(!markedObjectsHolder.Contains(theObject)) {
				SessionStateStack.ThrowIfCantEnter(this, SessionState.SessionObjectsToSaveOrDeleteChanging);
				markedObjectsHolder.Add(theObject);
				PersistentBase pb = theObject as PersistentBase;
				if(pb != null)
					pb.OnSessionProcessingProcessProcessed();
			}
		}
		void ProcessingProcessObjectListOrCollection(ObjectSet markedObjectsHolder, object theObjectOrCollection) {
			if(theObjectOrCollection is ICollection) {
				foreach(object obj in (ICollection)theObjectOrCollection)
					ProcessingProcessNextObject(markedObjectsHolder, obj);
			} else
				ProcessingProcessNextObject(markedObjectsHolder, theObjectOrCollection);
		}
		void ProcessingProcess(ObjectSet markedObjectsHolder, object theObject) {
			if(!TrackingChanges) {
				BeginTrackingChanges();
				if(IsUnitOfWork) {
					ProcessingProcessObjectListOrCollection(markedObjectsHolder, theObject);
				} else {
					try {
						ProcessingProcessObjectListOrCollection(markedObjectsHolder, theObject);
						FlushChanges();
					} catch(Exception e) {
						if(TrackingChanges) {
							try {
								DropChanges();
							} catch(Exception e2) {
								throw new ExceptionBundleException(e, e2);
							}
						}
						throw;
					}
				}
			} else {
				ProcessingProcessObjectListOrCollection(markedObjectsHolder, theObject);
			}
		}
		public void Save(object theObject) {
			ThrowIfCommitChangesToDataLayerInner();
			ProcessingProcess(objectsMarkedSaved, theObject);
		}
		public void Save(ICollection objects) {
			this.Save((object)objects);
		}
		ObjectSet objectsMarkedSaved = new ObjectSet();
		ObjectSet objectsMarkedDeleted = new ObjectSet();
		internal Dictionary<XPRefCollectionHelper, object> collectionsMarkedSaved = new Dictionary<XPRefCollectionHelper, object>();
		Random gcRecordIDGenerator;
		void DeleteObject(object theObject) {
			if(theObject == null)
				return;
			XPClassInfo classInfo = GetClassInfo(theObject);
			if(classInfo.IsGCRecordObject) {
				XPMemberInfo gcRecord = classInfo.GetMember(GCRecordField.StaticName);
				if(gcRecord.GetValue(theObject) != null)
					return; 
				TriggerObjectDeleting(theObject);
				if(gcRecordIDGenerator == null)
					gcRecordIDGenerator = new Random();
				gcRecord.SetValue(theObject, gcRecordIDGenerator.Next(1, int.MaxValue));
				DeleteCore(classInfo, theObject);
				TriggerObjectDeleted(theObject);
				Save(theObject);
			} else {
				if(IsObjectToDelete(theObject))
					return;
				TriggerObjectDeleting(theObject);
				ProcessingProcess(objectsMarkedDeleted, theObject);
				DeleteCore(classInfo, theObject);
				TriggerObjectDeleted(theObject);
			}
		}
		void DeleteObjectOrCollection(object theObject) {
			ICollection collection = theObject as ICollection;
			if(collection == null) {
				DeleteObject(theObject);
			} else {
				foreach(object obj in ListHelper.FromCollection(collection))
					DeleteObject(obj);
			}
		}
		void DeleteCore(XPClassInfo classInfo, object theObject) {
			foreach(XPMemberInfo mi in classInfo.AssociationListProperties) {
				if(mi.IsAggregated) {
					if(mi.IsCollection) {
						XPBaseCollection collection = (XPBaseCollection)mi.GetValue(theObject);
						if(!ReferenceEquals(null, collection.Criteria) || collection.SkipReturnedObjects > 0 || collection.TopReturnedObjects > 0) {
							InvalidOperationException ex = new InvalidOperationException(Res.GetString(Res.Collections_WantNotDeleteFilteredAggregateCollection, theObject, mi.Name));
							if(collection.GetObjectClassInfo().IsGCRecordObject || mi.GetAssociatedMember().HasAttribute(typeof(NoForeignKeyAttribute))) {
								throw ex;
							} else {
								try {
									throw ex;
								} catch { }
							}
						}
						var toDelete = new ArrayList(collection);
						foreach(object aggregated in toDelete) {
							Delete(aggregated);
						}
						if(!collection.SelectDeleted) {
							foreach(object aggregated in toDelete) {
								collection.BaseRemove(aggregated);
							}
						}
					} else {
						IList list = (IList)mi.GetValue(theObject);
						foreach(object aggregated in ListHelper.FromCollection(list)) {
							Delete(aggregated);
						}
					}
				} else
					if(mi.IsManyToMany) {
						XPBaseCollection collection = (XPBaseCollection)mi.GetValue(theObject);
						if(!collection.SelectDeleted) {
							foreach(object aggregated in new ArrayList(collection)) {
								collection.BaseRemove(aggregated);
							}
						}
					}
			}
			foreach(XPMemberInfo mi in classInfo.ObjectProperties) {
				if(mi.IsAggregated) {
					object aggregated = mi.GetValue(theObject);
					Delete(aggregated);
				}
				if(mi.IsAssociation) {
					object associatedObject = mi.GetValue(theObject);
					if(associatedObject != null) {
						XPMemberInfo assocList = mi.GetAssociatedMember();
						IList associatedCollection = (IList)assocList.GetValue(associatedObject);
						if(associatedCollection != null) {
							associatedCollection.Remove(theObject);
						}
					}
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RemoveFromDeleteList(object theObject) {
			if(theObject == null)
				return;
			objectsMarkedDeleted.Remove(theObject);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RemoveFromSaveList(object theObject) {
			if(theObject == null)
				return;
			objectsMarkedSaved.Remove(theObject);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void RemoveFromLists(object theObject) {
			RemoveFromDeleteList(theObject);
			RemoveFromSaveList(theObject);
		}
		public void Delete(object theObject) {
			LogManager.Log(LogCategory, () => {
				ThrowIfCommitChangesToDataLayerInner();
				if(TrackingChanges) {
					DeleteObjectOrCollection(theObject);
				} else {
					BeginTrackingChanges();
					try {
						DeleteObjectOrCollection(theObject);
						if(!IsUnitOfWork) {
							FlushChanges();
						}
					} catch(Exception e) {
						if(TrackingChanges) {
							try {
								DropChanges();
							} catch(Exception e2) {
								throw new ExceptionBundleException(e, e2);
							}
						}
						throw;
					}
				}
			}, (d) => {
				return CreateLogMessage("Executing Delete(theObject)", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_TheObject, GetObjectString(theObject)
			});
			}, null);
		}
		public void Delete(ICollection objects) {
			this.Delete((object)objects);
		}
		public PurgeResult PurgeDeletedObjects() {
			return PurgeDeletedObjects(ObjectLayer);
		}
		public static PurgeResult PurgeDeletedObjects(IObjectLayer objectLayer) {
			return LogManager.Log<PurgeResult>(LogCategory, () => {
				return objectLayer.Purge();
			}, (d) => {
				return new LogMessage(LogMessageType.SessionEvent, "Executing PurgeDeletedObjects()", d);
			});
		}
		void InternalBeforeSave(object theObject) {
			XPClassInfo classInfo = GetClassInfo(theObject);
			classInfo.CheckAbstractReference();
			XPMemberInfo key = classInfo.KeyProperty; 
			if(classInfo.IsTypedObject || classInfo.IsGCRecordObject || classInfo.HasPurgebleObjectReferences())
				GetObjectType(classInfo);
		}
		internal void CheckDuplicateObjectInIdentityMap(object theObject) {
			XPClassInfo ci = GetClassInfo(theObject);
			object cachedObj = SessionIdentityMap.GetLoadedObjectByKey(this, ci, ci.GetId(theObject));
			if(cachedObj != null && !object.ReferenceEquals(cachedObj, theObject))
				throw new DifferentObjectsWithSameKeyException();
		}
		internal void RegisterInsertedObject(object theObject) {
			SessionIdentityMap.RegisterObject(this, theObject, Dictionary.GetId(theObject));
		}
#if !SL
		[Obsolete("Please use DataLayer.SchemaInit event handler")]
		[Description("Occurs after database schema is initialized or updated")]
		public event SchemaInitEventHandler SchemaInit {
			add { ((IObjectLayerEx)ObjectLayer).SchemaInit += value; }
			remove { ((IObjectLayerEx)ObjectLayer).SchemaInit -= value; }
		}
#endif
		protected XPDictionary dict;
		bool isDisposed = false;
#if !SL
		protected override void Dispose(bool disposing) {
			isDisposed = true;
			if(disposing) {
				Disconnect();
			} else if(IsConnected) {
				PerformanceCounters.SessionCount.Decrement();
				PerformanceCounters.SessionDisconnected.Increment();
			}
			base.Dispose(disposing);
		}
#else
		new public void Dispose() {
			isDisposed = true;
			Disconnect();
		}
#endif
		void TriggerObjectsSaved(ICollection objects) {
			OnObjectsSaved(new ObjectsManipulationEventArgs(this, objects));
			foreach(object theObject in objects)
				TriggerObjectSaved(theObject);
		}
		protected virtual void OnObjectsSaved(ObjectsManipulationEventArgs e) {
			if(ObjectsSaved != null)
				ObjectsSaved(this, e);
		}
		internal void TriggerObjectsLoaded(ICollection objects) {
			OnObjectsLoaded(new ObjectsManipulationEventArgs(this, objects));
			foreach(object theObject in objects)
				TriggerObjectLoaded(theObject);
			UnMuteCollections();
		}
		protected virtual void OnObjectsLoaded(ObjectsManipulationEventArgs e) {
			if(ObjectsLoaded != null)
				ObjectsLoaded(this, e);
		}
		internal void TriggerObjectSaving(object theObject) {
			InternalBeforeSave(theObject);
			OnObjectSaving(new ObjectManipulationEventArgs(this, theObject));
			IXPObject xpObj = theObject as IXPObject;
			if(xpObj != null)
				xpObj.OnSaving();
		}
		protected virtual void OnObjectSaving(ObjectManipulationEventArgs e) {
			if(ObjectSaving != null)
				ObjectSaving(this, e);
		}
		void TriggerObjectSaved(object theObject) {
			IXPObject xpObj = theObject as IXPObject;
			if(xpObj != null)
				xpObj.OnSaved();
			OnObjectSaved(new ObjectManipulationEventArgs(this, theObject));
		}
		protected virtual void OnObjectSaved(ObjectManipulationEventArgs e) {
			if(ObjectSaved != null)
				ObjectSaved(this, e);
		}
		internal void TriggerObjectLoading(object theObject) {
			OnObjectLoading(new ObjectManipulationEventArgs(this, theObject));
			IXPObject xpObj = theObject as IXPObject;
			if(xpObj != null)
				xpObj.OnLoading();
		}
		protected virtual void OnObjectLoading(ObjectManipulationEventArgs e) {
			if(ObjectLoading != null)
				ObjectLoading(this, e);
		}
		void TriggerObjectLoaded(object theObject) {
			IXPObject xpObj = theObject as IXPObject;
			if(xpObj != null)
				xpObj.OnLoaded();
			OnObjectLoaded(new ObjectManipulationEventArgs(this, theObject));
		}
		protected virtual void OnObjectLoaded(ObjectManipulationEventArgs e) {
			if(ObjectLoaded != null)
				ObjectLoaded(this, e);
		}
		internal void TriggerObjectDeleting(object theObject) {
			OnObjectDeleting(new ObjectManipulationEventArgs(this, theObject));
			IXPObject xpObj = theObject as IXPObject;
			if(xpObj != null)
				xpObj.OnDeleting();
		}
		protected virtual void OnObjectDeleting(ObjectManipulationEventArgs e) {
			if(ObjectDeleting != null)
				ObjectDeleting(this, e);
		}
		internal void TriggerObjectDeleted(object theObject) {
			IXPObject xpObj = theObject as IXPObject;
			if(xpObj != null)
				xpObj.OnDeleted();
			OnObjectDeleted(new ObjectManipulationEventArgs(this, theObject));
		}
		protected virtual void OnObjectDeleted(ObjectManipulationEventArgs e) {
			if(ObjectDeleted != null)
				ObjectDeleted(this, e);
		}
		protected virtual void OnBeforeConnect() {
			if(BeforeConnect != null)
				BeforeConnect(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnAfterConnect() {
			if(AfterConnect != null)
				AfterConnect(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnBeforeDisconnect() {
			if(BeforeDisconnect != null)
				BeforeDisconnect(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnAfterDisconnect() {
			if(AfterDisconnect != null)
				AfterDisconnect(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnBeforeBeginTransaction() {
			if(BeforeBeginTransaction != null)
				BeforeBeginTransaction(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnAfterBeginTransaction() {
			if(AfterBeginTransaction != null)
				AfterBeginTransaction(this, new SessionManipulationEventArgs(this));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void OnBeforePreProcessCommitedList() {
			if(BeforePreProcessCommitedList != null)
				BeforePreProcessCommitedList(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnBeforeCommitTransaction() {
			if(BeforeCommitTransaction != null)
				BeforeCommitTransaction(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnAfterCommitTransaction() {
			if(AfterCommitTransaction != null)
				AfterCommitTransaction(this, new SessionManipulationEventArgs(this));
		}
		protected internal virtual void OnBeforeCommitNestedUnitOfWork(SessionManipulationEventArgs e) {
			if(BeforeCommitNestedUnitOfWork != null)
				BeforeCommitNestedUnitOfWork(this, e);
		}
		protected internal virtual void OnAfterCommitNestedUnitOfWork(SessionManipulationEventArgs e) {
			if(AfterCommitNestedUnitOfWork != null)
				AfterCommitNestedUnitOfWork(this, e);
		}
		protected virtual void OnBeforeRollbackTransaction() {
			if(BeforeRollbackTransaction != null)
				BeforeRollbackTransaction(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnAfterRollbackTransaction() {
			if(AfterRollbackTransaction != null)
				AfterRollbackTransaction(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnBeforeBeginTrackingChanges() {
			OnBeforeBeginTransaction();
			OnBeforeBeginTrackingChangesInternal();
		}
		protected void OnBeforeBeginTrackingChangesInternal() {
			if(BeforeBeginTrackingChanges != null)
				BeforeBeginTrackingChanges(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnAfterBeginTrackingChanges() {
			OnAfterBeginTrackingChangesInternal();
			OnAfterBeginTransaction();
		}
		protected void OnAfterBeginTrackingChangesInternal() {
			if(AfterBeginTrackingChanges != null)
				AfterBeginTrackingChanges(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnBeforeFlushChanges() {
			OnBeforeCommitTransaction();
			OnBeforeFlushChangesInternal();
		}
		protected void OnBeforeFlushChangesInternal() {
			if(BeforeFlushChanges != null)
				BeforeFlushChanges(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnAfterFlushChanges() {
			OnAfterFlushChangesInternal();
			OnAfterCommitTransaction();
		}
		protected void OnAfterFlushChangesInternal() {
			if(AfterFlushChanges != null)
				AfterFlushChanges(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnBeforeDropChanges() {
			OnBeforeRollbackTransaction();
			OnBeforeDropChangesInternal();
		}
		protected void OnBeforeDropChangesInternal() {
			if(BeforeDropChanges != null)
				BeforeDropChanges(this, new SessionManipulationEventArgs(this));
		}
		protected virtual void OnAfterDropChanges() {
			OnAfterDropChangesInternal();
			OnAfterRollbackTransaction();
		}
		protected void OnAfterDropChangesInternal() {
			if(AfterDropChanges != null)
				AfterDropChanges(this, new SessionManipulationEventArgs(this));
		}
		protected virtual bool OnFailedCommitTransaction(Exception ex) {
			SessionOperationFailEventArgs args = new SessionOperationFailEventArgs(ex);
			if(FailedCommitTransaction != null)
				FailedCommitTransaction(this, args);
			return args.Handled;
		}
		protected virtual bool OnFailedFlushChanges(Exception ex) {
			return OnFailedFlushChangesInternal(ex) || OnFailedCommitTransaction(ex);
		}
		protected bool OnFailedFlushChangesInternal(Exception ex) {
			SessionOperationFailEventArgs args = new SessionOperationFailEventArgs(ex);
			if(FailedFlushChanges != null)
				FailedFlushChanges(this, args);
			return args.Handled;
		}
		bool? isObjectModifiedOnNonPersistentPropertyChange;
		public bool? IsObjectModifiedOnNonPersistentPropertyChange {
			get { return isObjectModifiedOnNonPersistentPropertyChange; }
			set { isObjectModifiedOnNonPersistentPropertyChange = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void TriggerObjectChanged(object changedObject, ObjectChangeEventArgs e) {
			if(e.Reason == ObjectChangeReason.PropertyChanged) {
				XPClassInfo ci = GetClassInfo(changedObject);
				XPMemberInfo mi = null;
				if(e.PropertyName != null && e.PropertyName.Length > 0 && !ReferenceEquals(e.OldValue, e.NewValue)) {
					mi = ci.FindMember(e.PropertyName);
					if(mi != null) {
						if(ci.TrackPropertiesModifications ?? TrackPropertiesModifications)
							mi.SetModified(changedObject, e.OldValue);
					}
				}
				if(this.IsUnitOfWork && ci.IsPersistent &&
					(mi == null || mi.IsPersistent 
					|| (!mi.IsPersistent && (IsObjectModifiedOnNonPersistentPropertyChange ?? XpoDefault.IsObjectModifiedOnNonPersistentPropertyChange)))) {
					Save(changedObject);
				}
				OnObjectChanged(changedObject, e);
				if(mi != null) {
					mi.ProcessAssociationRefChange(this, changedObject, e.OldValue, e.NewValue);
				}
			} else {
				OnObjectChanged(changedObject, e);
			}
		}
		protected virtual void OnObjectChanged(object changedObject, ObjectChangeEventArgs e) {
			if(ObjectChanged != null)
				ObjectChanged(changedObject, e);
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SessionDefaultSession")]
#endif
		public static Session DefaultSession {
			get {
				return XpoDefault.Session;
			}
		}
		[Obsolete("Use SessionIdentityMap.Extract method instead", true), EditorBrowsable(EditorBrowsableState.Never)]
		protected SessionIdentityMap Cache {
			get {
				return SessionIdentityMap.Extract(this);
			}
		}
		void SessionInitCommon() {
			this._connectionString = string.Empty;
#if !SL
			this._connection = null;
			this.closeConnectionOnDisconnect = false;
#endif
		}
		void SessionInitDisconnected(XPDictionary dictionary) {
			this.objectLayer = null;
			this.dict = dictionary;
			this._DisposeOnDisconnect = null;
			SessionInitCommon();
		}
		void SessionInitConnected(IObjectLayer layer, IDisposable[] disposeOnDisconnect) {
			if(layer == null)
				throw new ArgumentNullException("layer");
			OnBeforeConnect();
			this._DisposeOnDisconnect = disposeOnDisconnect;
			this.objectLayer = layer;
			this.dict = this.objectLayer.Dictionary;
			SessionInitCommon();
			OnAfterConnect();
		}
		public Session()
			: this((XPDictionary)null) {
		}
		public Session(IContainer container)
			: this() {
			container.Add(this);
		}
		public Session(XPDictionary dictionary)
			: base() {
			SessionInitDisconnected(dictionary);
		}
		public Session(IDataLayer layer, params IDisposable[] disposeOnDisconnect)
			: this(SimpleObjectLayer.FromDataLayer(layer), disposeOnDisconnect) {
		}
		public Session(IObjectLayer layer, params IDisposable[] disposeOnDisconnect)
			: base() {
			SessionInitConnected(layer, disposeOnDisconnect);
#if !SL
			PerformanceCounters.SessionCount.Increment();
			PerformanceCounters.SessionConnected.Increment();
#endif
		}
		public object GetLoadedObjectByKey(Type classType, object id) {
			return GetLoadedObjectByKey(Dictionary.GetClassInfo(classType), id);
		}
		public object GetLoadedObjectByKey(XPClassInfo classInfo, object id) {
			if(classInfo == null)
				throw new ArgumentNullException(LogParam_ClassInfo);
			if(classInfo.KeyProperty == null)
				classInfo.CheckAbstractReference();
			if(id == null)
				return null;
			if(!classInfo.KeyProperty.MemberType.IsAssignableFrom(id.GetType()) && !((typeof(ArrayList).IsAssignableFrom(id.GetType()) ||
 typeof(List<object>).IsAssignableFrom(id.GetType())) && classInfo.KeyProperty.IsStruct) && !(classInfo.KeyProperty.ReferenceType != null && classInfo.KeyProperty.ReferenceType.KeyProperty.MemberType.IsAssignableFrom(id.GetType())))
				throw new ArgumentException(Res.GetString(Res.Session_IncompatibleIdType, id.GetType().FullName, classInfo.FullName, classInfo.KeyProperty.Name, classInfo.KeyProperty.MemberType.FullName));
			return SessionIdentityMap.GetLoadedObjectByKey(this, classInfo, classInfo.KeyProperty.ExpandId(id));
		}
		public object GetObjectByKey(Type classType, object id) {
			return GetObjectByKey(Dictionary.GetClassInfo(classType), id);
		}
		public object GetObjectByKey(XPClassInfo classInfo, object id) {
			return GetObjectByKey(classInfo, id, false);
		}
		public object GetObjectByKey(Type classType, object id, bool alwaysGetFromDb) {
			return GetObjectByKey(Dictionary.GetClassInfo(classType), id, alwaysGetFromDb);
		}
		public ICollection[] GetObjectsByKey(ObjectsByKeyQuery[] queries, bool alwaysGetFromDb) {
			return LogManager.Log<ICollection[]>(LogCategory, () => {
				List<List<object>> existsObjectsList = new List<List<object>>();
				List<int?> reloadQueryIndexList = new List<int?>();
				List<ObjectsByKeyQuery> reloadQueryList = new List<ObjectsByKeyQuery>();
				List<ObjectsByKeyQuery> newQueryList = new List<ObjectsByKeyQuery>();
				for(int i = 0; i < queries.Length; i++) {
					List<object> existsObjects = new List<object>();
					List<object> requestIdList = new List<object>();
					List<object> reloadIdList = new List<object>();
					ObjectsByKeyQuery query = queries[i];
					foreach(object id in query.IdCollection) {
						object obj = GetLoadedObjectByKey(query.ClassInfo, id);
						if(obj == null)
							requestIdList.Add(id);
						else {
							if(alwaysGetFromDb) {
								RemoveFromLists(obj);
								reloadIdList.Add(id);
								continue;
							}
							existsObjects.Add(obj);
						}
					}
					existsObjectsList.Add(existsObjects);
					if(reloadIdList.Count > 0) {
						reloadQueryIndexList.Add(reloadQueryList.Count);
						reloadQueryList.Add(new ObjectsByKeyQuery(query.ClassInfo, reloadIdList.ToArray()));
					} else {
						reloadQueryIndexList.Add(null);
					}
					newQueryList.Add(new ObjectsByKeyQuery(query.ClassInfo, requestIdList.ToArray()));
				}
				ICollection[] reloadedCollections = null;
				if(reloadQueryList.Count > 0) {
					reloadedCollections = ObjectLayer.GetObjectsByKey(this, reloadQueryList.ToArray());
					for(int i = 0; i < reloadedCollections.Length; i++) {
						if(reloadedCollections[i].Count != reloadQueryList[i].IdCollection.Count)
							throw new CannotLoadObjectsException(reloadQueryList[i].ClassInfo.FullName);
						int objectIndex = 0;
						foreach(object obj in reloadedCollections[i]) {
							if(obj == null)
								throw new CannotLoadObjectsException(reloadQueryList[i].ClassInfo.FullName + "(" + ((object[])reloadQueryList[i].IdCollection)[objectIndex].ToString() + ")");
							objectIndex++;
						}
					}
				}
				ICollection[] loadedCollections = ObjectLayer.GetObjectsByKey(this, OptimizeGetObjectsByKeyQueries(newQueryList));
				ICollection[] resultCollections = new ICollection[loadedCollections.Length];
				for(int i = 0; i < loadedCollections.Length; i++) {
					List<object> existsObjects = existsObjectsList[i];
					ICollection loadedCollection = loadedCollections[i];
					object[] result;
					if(reloadQueryIndexList[i].HasValue) {
						ICollection reloadedCollection = reloadedCollections[i];
						result = new object[existsObjects.Count + loadedCollection.Count + reloadedCollection.Count];
						existsObjects.CopyTo(result, 0);
						loadedCollection.CopyTo(result, existsObjects.Count);
						reloadedCollection.CopyTo(result, existsObjects.Count + loadedCollection.Count);
					} else {
						result = new object[existsObjects.Count + loadedCollection.Count];
						existsObjects.CopyTo(result, 0);
						loadedCollection.CopyTo(result, existsObjects.Count);
					}
					resultCollections[i] = result;
				}
				ICollection[] rv = new ICollection[queries.Length];
				for(int i = 0; i < queries.Length; ++i) {
					List<object> objs = new List<object>(queries[i].IdCollection.Count);
					XPClassInfo ci = queries[i].ClassInfo;
					foreach(object key in queries[i].IdCollection) {
						objs.Add(GetLoadedObjectByKey(ci, key));
					}
					rv[i] = objs.ToArray();
				}
				GC.KeepAlive(existsObjectsList);
				GC.KeepAlive(loadedCollections);
				GC.KeepAlive(resultCollections);
				GC.KeepAlive(reloadedCollections);
				return rv;
			}, (d) => {
				LogMessage lm = new LogMessage(LogMessageType.SessionEvent, "Executing GetObjectsByKey()", d);
				lm.ParameterList.Add(new LogMessageParameter(LogParam_SessionType, GetType().ToString()));
				lm.ParameterList.Add(new LogMessageParameter(LogParam_SessionID, this.ToString()));
				lm.ParameterList.Add(new LogMessageParameter(LogParam_AlwaysGetFromDb, alwaysGetFromDb));
				for(int i = 0; i < queries.Length; i++) {
					lm.ParameterList.Add(new LogMessageParameter("query #" + i, LogHelpers.ObjectsByKeyQueryToString(queries[i])));
				}
				return lm;
			});
		}
		ObjectsByKeyQuery[] OptimizeGetObjectsByKeyQueries(List<ObjectsByKeyQuery> queries) {
			ObjectsByKeyQuery[] result = new ObjectsByKeyQuery[queries.Count];
			for(int i = 0; i < queries.Count; i++) {
				Dictionary<object, bool> existsKeys = new Dictionary<object, bool>();
				foreach(object id in queries[i].IdCollection) {
					if(id == null)
						continue;
					existsKeys[id] = true;
				}
				object[] resultIdCollection = new object[existsKeys.Count];
				existsKeys.Keys.CopyTo(resultIdCollection, 0);
				result[i] = new ObjectsByKeyQuery(queries[i].ClassInfo, resultIdCollection);
			}
			return result;
		}
		public ICollection GetObjectsByKey(XPClassInfo classInfo, ICollection idCollection, bool alwaysGetFromDb) {
			return GetObjectsByKey(new ObjectsByKeyQuery[] { new ObjectsByKeyQuery(classInfo, idCollection) }, alwaysGetFromDb)[0];
		}
		public object GetObjectByKey(XPClassInfo classInfo, object id, bool alwaysGetFromDb) {
			return LogManager.Log<object>(LogCategory, () => {
				object result = null;
				TypesManager.EnsureIsTypedObjectValid();
				if(id != null) {
					result = GetLoadedObjectByKey(classInfo, id);
					if(result != null) {
						if(alwaysGetFromDb)
							Reload(result);
					} else {
						ICollection coll = ObjectLayer.GetObjectsByKey(this, new ObjectsByKeyQuery[] { new ObjectsByKeyQuery(classInfo, new object[] { id }) })[0];
						IEnumerator enm = coll.GetEnumerator();
						result = enm.MoveNext() ? enm.Current : null;
					}
				}
				return result;
			}, (d) => {
				return CreateLogMessage("Executing GetObjectByKey()", d, new object[] { 
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_ObjectID, id == null ? "null" : id.ToString(),
				LogParam_AlwaysGetFromDb, alwaysGetFromDb
			});
			});
		}
		public ClassType GetLoadedObjectByKey<ClassType>(object id) {
			return (ClassType)GetLoadedObjectByKey(typeof(ClassType), id);
		}
		public ClassType GetObjectByKey<ClassType>(object id) {
			return (ClassType)GetObjectByKey(typeof(ClassType), id);
		}
		public ClassType GetObjectByKey<ClassType>(object id, bool alwaysGetFromDb) {
			return (ClassType)GetObjectByKey(typeof(ClassType), id, alwaysGetFromDb);
		}
		public object FindObject(Type classType, CriteriaOperator criteria) {
			return FindObject(classType, criteria, false);
		}
		public object FindObject(XPClassInfo classInfo, CriteriaOperator criteria) {
			return FindObject(classInfo, criteria, false);
		}
		private ObjectsQuery GetQueryForFindObject(XPClassInfo classInfo, CriteriaOperator criteria, bool selectDeleted) {
			return new ObjectsQuery(classInfo, criteria, null, 0, 1, new CollectionCriteriaPatcher(selectDeleted, TypesManager), false);
		}
		public object FindObject(XPClassInfo classInfo, CriteriaOperator criteria, bool selectDeleted) {
			return LogManager.Log<object>(LogCategory, () => {
				ObjectsQuery query = GetQueryForFindObject(classInfo, criteria, selectDeleted);
				ICollection coll = GetObjects(query);
				IEnumerator e = coll.GetEnumerator();
				return e.MoveNext() ? e.Current : null;
			}, (d) => {
				return CreateLogMessage("Executing FindObject()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_IsInTransactionMode, this.IsInTransactionMode,
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Criteria, ReferenceEquals(criteria, null) ? "null" : criteria.ToString(),
				LogParam_SelectDeleted, selectDeleted
			});
			});
		}
		static void FindObjectAsyncResultProcess(ICollection[] res, Exception ex, AsyncFindObjectCallback callback) {
			if(ex != null) {
				try {
					callback(null, ex);
				} catch(Exception) { }
				return;
			}
			object resultObject = null;
			try {
				if(res != null && res.Length > 0 && res[0] != null) {
					IEnumerator e = res[0].GetEnumerator();
					resultObject = e.MoveNext() ? e.Current : null;
				}
			} catch(Exception iex) {
				try {
					callback(null, iex);
				} catch(Exception) { }
			}
			try {
				callback(resultObject, null);
			} catch(Exception) { }
		}
		public object FindObjectAsync<ClassType>(CriteriaOperator criteria, AsyncFindObjectCallback callback) {
			return FindObjectAsync(GetClassInfo<ClassType>(), criteria, false, callback);
		}
		public object FindObjectAsync<ClassType>(CriteriaOperator criteria, bool selectDeleted, AsyncFindObjectCallback callback) {
			return FindObjectAsync(GetClassInfo<ClassType>(), criteria, selectDeleted, callback);
		}
		public object FindObjectAsync<ClassType>(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, CriteriaOperator criteria, AsyncFindObjectCallback callback) {
			return FindObjectAsync(criteriaEvaluationBehavior, GetClassInfo<ClassType>(), criteria, false, callback);
		}
		public object FindObjectAsync(XPClassInfo classInfo, CriteriaOperator criteria, AsyncFindObjectCallback callback) {
			return FindObjectAsync(classInfo, criteria, false, callback);
		}
		public object FindObjectAsync(XPClassInfo classInfo, CriteriaOperator criteria, bool selectDeleted, AsyncFindObjectCallback callback) {
			ObjectsQuery query = GetQueryForFindObject(classInfo, criteria, selectDeleted);
			return GetObjectsAsync(new ObjectsQuery[] { query }, (res, ex) => { FindObjectAsyncResultProcess(res, ex, callback); });
		}
		public object FindObjectAsync(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, XPClassInfo classInfo, CriteriaOperator criteria, AsyncFindObjectCallback callback) {
			return FindObjectAsync(criteriaEvaluationBehavior, classInfo, criteria, false, callback);
		}
		public object FindObjectAsync(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, XPClassInfo classInfo, CriteriaOperator criteria, bool selectDeleted, AsyncFindObjectCallback callback) {
			return LogManager.Log<object>(LogCategory, () => {
				AsyncLoadObjectsCallback loadObjects = (res, ex) => { FindObjectAsyncResultProcess(res, ex, callback); };
				switch(criteriaEvaluationBehavior) {
					case PersistentCriteriaEvaluationBehavior.BeforeTransaction: {
							ObjectsQuery query = GetQueryForFindObject(classInfo, criteria, selectDeleted);
							return GetObjectsInternalAsync(new ObjectsQuery[] { query }, loadObjects);
						}
					case PersistentCriteriaEvaluationBehavior.InTransaction: {
							ObjectsQuery query = GetQueryForFindObject(classInfo, criteria, selectDeleted);
							return InTransactionLoader.GetObjectsAsync(this, new ObjectsQuery[] { query }, loadObjects);
						}
					default:
						throw new InvalidOperationException(Res.GetString(Res.Session_UnexpectedPersistentCriteriaEvaluationBehavior, criteriaEvaluationBehavior.ToString()));
				}
			}, (d) => {
				return CreateLogMessage("Executing FindObjectAsync()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_CriteriaEvaluationBehavior, criteriaEvaluationBehavior.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Criteria, ReferenceEquals(criteria, null) ? "null" : criteria.ToString(),
				LogParam_SelectDeleted, selectDeleted
				});
			});
		}
		public object FindObject(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, XPClassInfo classInfo, CriteriaOperator criteria) {
			return FindObject(criteriaEvaluationBehavior, classInfo, criteria, false);
		}
		public object FindObject(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, XPClassInfo classInfo, CriteriaOperator criteria, bool selectDeleted) {
			return LogManager.Log<object>(LogCategory, () => {
				switch(criteriaEvaluationBehavior) {
					case PersistentCriteriaEvaluationBehavior.BeforeTransaction: {
							ObjectsQuery query = GetQueryForFindObject(classInfo, criteria, selectDeleted);
							ICollection coll = GetObjectsInternal(new ObjectsQuery[] { query })[0];
							IEnumerator e = coll.GetEnumerator();
							return e.MoveNext() ? e.Current : null;
						}
					case PersistentCriteriaEvaluationBehavior.InTransaction: {
							ObjectsQuery query = GetQueryForFindObject(classInfo, criteria, selectDeleted);
							ICollection coll = InTransactionLoader.GetObjects(this, new ObjectsQuery[] { query })[0];
							IEnumerator e = coll.GetEnumerator();
							return e.MoveNext() ? e.Current : null;
						}
					default:
						throw new InvalidOperationException(Res.GetString(Res.Session_UnexpectedPersistentCriteriaEvaluationBehavior, criteriaEvaluationBehavior.ToString()));
				}
			}, (d) => {
				return CreateLogMessage("Executing FindObject()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_CriteriaEvaluationBehavior, criteriaEvaluationBehavior.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Criteria, ReferenceEquals(criteria, null) ? "null" : criteria.ToString(),
				LogParam_SelectDeleted, selectDeleted
				});
			});
		}
		public object FindObject(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Type objType, CriteriaOperator criteria) {
			return FindObject(criteriaEvaluationBehavior, this.GetClassInfo(objType), criteria);
		}
		public object FindObject(Type classType, CriteriaOperator criteria, bool selectDeleted) {
			return FindObject(Dictionary.GetClassInfo(classType), criteria, selectDeleted);
		}
		public ClassType FindObject<ClassType>(CriteriaOperator criteria) {
			return (ClassType)FindObject(typeof(ClassType), criteria, false);
		}
		public ClassType FindObject<ClassType>(CriteriaOperator criteria, bool selectDeleted) {
			return (ClassType)FindObject(typeof(ClassType), criteria, selectDeleted);
		}
		public ClassType FindObject<ClassType>(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, CriteriaOperator criteria) {
			return (ClassType)FindObject(criteriaEvaluationBehavior, typeof(ClassType), criteria);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Please use GetObjectsToSave() instead", true)]
		public ICollection ObjectsSavedInCurrentTransaction {
			get { return GetObjectsToSave(); }
		}
		public ICollection GetObjectsToSave() {
			return objectsMarkedSaved;
		}
		public ICollection GetObjectsToDelete() {
			return objectsMarkedDeleted;
		}
		public virtual ICollection GetObjectsToSave(bool includeParent) {
			ICollection myObjects = GetObjectsToSave();
			if(!includeParent)
				return myObjects;
			IObjectLayerOnSession objectLayerOnSession = ObjectLayer as IObjectLayerOnSession;
			if(objectLayerOnSession == null)
				return myObjects;
			ICollection parentObjects = objectLayerOnSession.GetParentObjectsToSave(this);
			if(parentObjects.Count == 0)
				return myObjects;
			ObjectSet combinedList = new ObjectSet(myObjects.Count + parentObjects.Count);
			foreach(object obj in myObjects) {
				combinedList.Add(obj);
			}
			foreach(object obj in parentObjects) {
				combinedList.Add(obj);
			}
			return combinedList;
		}
		public virtual ICollection GetObjectsToDelete(bool includeParent) {
			ICollection myObjects = GetObjectsToDelete();
			if(!includeParent)
				return myObjects;
			IObjectLayerOnSession objectLayerOnSession = ObjectLayer as IObjectLayerOnSession;
			if(objectLayerOnSession == null)
				return myObjects;
			ICollection parentObjects = objectLayerOnSession.GetParentObjectsToDelete(this);
			if(parentObjects.Count == 0)
				return myObjects;
			ObjectSet combinedList = new ObjectSet(myObjects.Count + parentObjects.Count);
			foreach(object obj in myObjects) {
				combinedList.Add(obj);
			}
			foreach(object obj in parentObjects) {
				combinedList.Add(obj);
			}
			return combinedList;
		}
		public ICollection CollectReferencingObjects(object target, PersistentCriteriaEvaluationBehavior behavior, bool selectDeleted) {
			if(target == null)
				throw new ArgumentNullException("target");
			ThrowIfObjectFromDifferentSession(target);
			XPClassInfo ci = GetClassInfo(target);
			ci.CheckAbstractReference();
			TypesManager.EnsureIsTypedObjectValid();
			Dictionary<XPClassInfo, List<XPMemberInfo>> referenceMembersByMappingClasses = new Dictionary<XPClassInfo, List<XPMemberInfo>>();
			foreach(XPClassInfo possibleReferrer in Dictionary.Classes) {
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
			ObjectSet resultCollector = new ObjectSet();
			foreach(XPClassInfo referrer in referenceMembersByMappingClasses.Keys) {
				List<CriteriaOperator> list = new List<CriteriaOperator>();
				foreach(XPMemberInfo mi in (IList)referenceMembersByMappingClasses[referrer]) {
					list.Add(new OperandProperty(mi.Name));
				}
				InOperator criteria = new InOperator(new OperandValue(target), list);
				ICollection referringObjects;
				if(behavior == PersistentCriteriaEvaluationBehavior.InTransaction)
					referringObjects = GetObjectsInTransaction(referrer, criteria, selectDeleted);
				else
					referringObjects = GetObjectsInternal(new ObjectsQuery[] { new ObjectsQuery(referrer, criteria, null, 0, 0, new CollectionCriteriaPatcher(selectDeleted, TypesManager), false) })[0];
				foreach(object obj in referringObjects) {
					resultCollector.Add(obj);
				}
			}
			return resultCollector;
		}
		public ICollection CollectReferencingObjects(object target) {
			return CollectReferencingObjects(target, PersistentCriteriaEvaluationBehavior.BeforeTransaction, false);
		}
		public IList ProcessReferences(object target, ProcessReferenceHandler process) {
			List<object> result = ListHelper.FromCollection(CollectReferencingObjects(target));
			XPClassInfo ci = GetClassInfo(target);
			foreach(object obj in result) {
				foreach(XPMemberInfo mi in GetClassInfo(obj).ObjectProperties) {
					if(!ci.IsAssignableTo(mi.ReferenceType))
						continue;
					if(!ReferenceEquals(target, mi.GetValue(obj)))
						continue;
					process(obj, mi);
				}
			}
			return result;
		}
		public bool IsObjectToSave(object theObject) {
			if(theObject == null)
				throw new ArgumentNullException(LogParam_TheObject);
			ThrowIfObjectFromDifferentSession(theObject);
			return objectsMarkedSaved.Contains(theObject);
		}
		public bool IsObjectToDelete(object theObject) {
			if(theObject == null)
				throw new ArgumentNullException(LogParam_TheObject);
			ThrowIfObjectFromDifferentSession(theObject);
			return objectsMarkedDeleted.Contains(theObject);
		}
		public bool IsNewObject(object theObject) {
			ThrowIfObjectFromDifferentSession(theObject);
			XPClassInfo ci = GetClassInfo(theObject);
			return SessionIdentityMap.GetLoadedObjectByKey(this, ci, ci.GetId(theObject)) != theObject;
		}
		public virtual bool IsObjectToSave(object theObject, bool includeParent) {
			if(IsObjectToSave(theObject))
				return true;
			if(!includeParent)
				return false;
			IObjectLayerOnSession objectLayerOnSession = ObjectLayer as IObjectLayerOnSession;
			if(objectLayerOnSession == null)
				return false;
			return objectLayerOnSession.IsParentObjectToSave(this, theObject);
		}
		public virtual bool IsObjectToDelete(object theObject, bool includeParent) {
			if(IsObjectToDelete(theObject))
				return true;
			if(!includeParent)
				return false;
			IObjectLayerOnSession objectLayerOnSession = ObjectLayer as IObjectLayerOnSession;
			if(objectLayerOnSession == null)
				return false;
			return objectLayerOnSession.IsParentObjectToDelete(this, theObject);
		}
		public virtual bool IsNewObject(object theObject, bool includeParent) {
			return IsNewObject(theObject);
		}
		public bool IsObjectMarkedDeleted(object theObject) {
			if(IsObjectToDelete(theObject))
				return true;
			XPMemberInfo gc = GetClassInfo(theObject).FindMember(GCRecordField.StaticName);
			if(gc == null)
				return false;
			else
				return gc.GetValue(theObject) != null;
		}
		protected internal virtual ICollection GetTouchedClassInfosIncludeParent() {
			Dictionary<XPClassInfo, object> rv = new Dictionary<XPClassInfo, object>();
			foreach(object obj in GetObjectsToSave()) {
				rv[GetClassInfo(obj)] = null;
			}
			foreach(object obj in GetObjectsToDelete()) {
				rv[GetClassInfo(obj)] = null;
			}
			IObjectLayerOnSession objectLayerOnSession = ObjectLayer as IObjectLayerOnSession;
			if(objectLayerOnSession != null) {
				foreach(XPClassInfo ci in objectLayerOnSession.GetParentTouchedClassInfos(this)) {
					rv[ci] = null;
				}
			}
			return rv.Keys;
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearDatabase() {
			DropIdentityMap();
			((IObjectLayerForTests)ObjectLayer).ClearDatabase();
			DropIdentityMap();
		}
		protected virtual NestedUnitOfWork CreateNestedUnitOfWork() {
			return new NestedUnitOfWork(this);
		}
		public NestedUnitOfWork BeginNestedUnitOfWork() {
			return CreateNestedUnitOfWork();
		}
		public virtual void BeginTransaction() {
			LogManager.Log(LogCategory, () => {
				BeginTrackingChanges();
			}, (d) => {
				return CreateLogMessage("Executing BeginTransaction()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString()
			});
			}, null);
		}
		public void BeginTrackingChanges() {
			OnBeforeBeginTrackingChanges();
			if(TrackingChanges)
				throw new TransactionSequenceException(Res.GetString(Res.Session_TranSequenceBegin));
			SessionStateStack.Enter(this, SessionState.BeginTransaction);
			try {
				Connect();
				System.Diagnostics.Debug.Assert(GetObjectsToSave().Count == 0);
				System.Diagnostics.Debug.Assert(GetObjectsToDelete().Count == 0);
				trackingChanges = true;
			} finally {
				SessionStateStack.Leave(this, SessionState.BeginTransaction);
			}
			OnAfterBeginTrackingChanges();
		}
		void FlushChangesInsideTransaction() {
			List<object> fullListForDelete = new List<object>();
			foreach(object obj in GetObjectsToDelete()) {
				System.Diagnostics.Debug.Assert(!GetClassInfo(obj).IsGCRecordObject);
				fullListForDelete.Add(obj);
			}
			List<object> completeListForSave = new List<object>();
			foreach(object obj in GetObjectsToSave()) {
				if(!IsObjectToDelete(obj))
					completeListForSave.Add(obj);
			}
			ObjectLayer.CommitChanges(this, fullListForDelete, completeListForSave);
		}
		void ThrowIfCommitChangesToDataLayerInner() {
			if(SessionStateStack.IsInAnyOf(this, SessionState.CommitChangesToDataLayerInner)) {
				throw new InvalidOperationException(Res.GetString(Res.Session_ObjectModificationIsNotAllowed));
			}
		}
		public virtual object CommitTransactionAsync(AsyncCommitCallback callback) {
			return FlushChangesAsync(callback);
		}
		public object FlushChangesAsync(AsyncCommitCallback callback) {
			return LogManager.Log<object>(LogCategory, () => {
				try {
					IList objectsToFireSaved = BeginFlushChanges();
					List<object> fullListForDelete = new List<object>();
					foreach(object obj in GetObjectsToDelete()) {
						System.Diagnostics.Debug.Assert(!GetClassInfo(obj).IsGCRecordObject);
						fullListForDelete.Add(obj);
					}
					List<object> completeListForSave = new List<object>();
					foreach(object obj in GetObjectsToSave()) {
						if(!IsObjectToDelete(obj))
							completeListForSave.Add(obj);
					}
					SessionStateStack.Enter(this, SessionState.CommitChangesToDataLayer);
					return ObjectLayer.CommitChangesAsync(this, fullListForDelete, completeListForSave, new AsyncCommitCallback(delegate(Exception commitEx) {
						SessionStateStack.Leave(this, SessionState.CommitChangesToDataLayer);
						if(commitEx == null) {
							try {
								EndFlushChanges(objectsToFireSaved);
								callback(null);
							} catch(Exception ex) {
								try {
									callback(OnFailedFlushChanges(ex) ? null : ex);
								} catch(Exception) { }
							}
							return;
						}
						try {
							callback(OnFailedFlushChanges(commitEx) ? null : commitEx);
						} catch(Exception) { }
					}));
				} catch(Exception syncEx) {
					if(!OnFailedFlushChanges(syncEx)) {
						throw;
					}
					return null;
				}
			}, (d) => {
				LogMessage message = new LogMessage(LogMessageType.SessionEvent, "Executing FlushChangesAsync()", d);
				message.ParameterList.Add(new LogMessageParameter(LogParam_SessionType, this.GetType().ToString()));
				message.ParameterList.Add(new LogMessageParameter(LogParam_SessionID, this.ToString()));
				return message;
			});
		}
		public virtual void CommitTransaction() {
			LogManager.Log(LogCategory, () => {
				FlushChanges();
			}, (d) => {
				return CreateLogMessage("Executing CommitTransaction()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString()
			});
			}, null);
		}
		public void FlushChanges() {
			try {
				WaitForAsyncOperationEnd();
				IList objectsToFireSaved = BeginFlushChanges();
				SessionStateStack.Enter(this, SessionState.CommitTransactionNonReenterant);
				try {
					SessionStateStack.Enter(this, SessionState.CommitChangesToDataLayer);
					try {
						FlushChangesInsideTransaction();
					} finally {
						SessionStateStack.Leave(this, SessionState.CommitChangesToDataLayer);
					}
				} finally {
					SessionStateStack.Leave(this, SessionState.CommitTransactionNonReenterant);
				}
				EndFlushChanges(objectsToFireSaved);
			} catch(Exception ex) {
				if(!OnFailedFlushChanges(ex)) {
					throw;
				}
			}
		}
		protected virtual IList BeginFlushChanges() {
			IList objectsToFireSaved = null;
			if(!IsUnitOfWork && !TrackingChanges)
				throw new TransactionSequenceException(Res.GetString(Res.Session_TranSequenceCommit));
			ThrowIfCommitChangesToDataLayerInner();
			SessionStateStack.Enter(this, SessionState.CommitTransactionNonReenterant);
			try {
				PreProcessSavedList();
				OnBeforeFlushChanges();
				ICollection objectsToSave = GetObjectsToSave();
				objectsToFireSaved = new List<object>(objectsToSave.Count);
				foreach(object obj in objectsToSave) {
					if(!IsObjectToDelete(obj))
						objectsToFireSaved.Add(obj);
				}
				SessionStateStack.Enter(this, SessionState.OptimistickLockFieldsProcessing);
				try {
					foreach(object obj in objectsToSave) {
						XPClassInfo ci = GetClassInfo(obj);
						XPMemberInfo olf = ci.OptimisticLockField;
						if(olf == null)
							continue;
						int? oldV = (int?)olf.GetValue(obj);
						int? newV;
						if(oldV.HasValue && oldV < int.MaxValue)
							newV = oldV.Value + 1;
						else
							newV = 0;
						olf.SetValue(obj, newV);
						if (ci.TrackPropertiesModifications ?? TrackPropertiesModifications)
							olf.SetModified(obj, oldV);
					}
				} finally {
					SessionStateStack.Leave(this, SessionState.OptimistickLockFieldsProcessing);
				}
			} finally {
				SessionStateStack.Leave(this, SessionState.CommitTransactionNonReenterant);
			}
			return objectsToFireSaved;
		}
		void EndFlushChanges(IList objectsToFireSaved) {
			SessionStateStack.Enter(this, SessionState.CommitTransactionNonReenterant);
			try {
				SessionStateStack.Enter(this, SessionState.OptimistickLockFieldsProcessing);
				try {
					foreach(object obj in objectsToFireSaved) {
						XPClassInfo ci = GetClassInfo(obj);
						foreach(XPMemberInfo mi in ci.PersistentProperties) {
							if(!mi.IsDelayed)
								continue;
							XPDelayedProperty container = XPDelayedProperty.GetDelayedPropertyContainer(obj, mi);
							container.ResetIsModified();
						}
						XPMemberInfo olf = ci.OptimisticLockField;
						if(olf != null) {
							ci.OptimisticLockFieldInDataLayer.SetValue(obj, olf.GetValue(obj));
						}
						ci.ClearModifications(obj);
					}
				} finally {
					SessionStateStack.Leave(this, SessionState.OptimistickLockFieldsProcessing);
				}
				foreach(XPRefCollectionHelper c in collectionsMarkedSaved.Keys)
					c.ClearChangesCache();
				collectionsMarkedSaved.Clear();
				objectsMarkedSaved.Clear();
				objectsMarkedDeleted.Clear();
				trackingChanges = false;
			} finally {
				SessionStateStack.Leave(this, SessionState.CommitTransactionNonReenterant);
			}
			TriggerObjectsSaved(objectsToFireSaved);
			OnAfterFlushChanges();
		}
		void RollbackVersions() {
			foreach(KeyValuePair<object, int?> ent in objectsMarkedSaved) {
				if(ent.Value != null) {
					XPMemberInfo lockField = GetClassInfo(ent.Key).OptimisticLockField;
					if(lockField != null)
						lockField.SetValue(ent.Key, ent.Value);
				}
			}
		}
		public virtual void RollbackTransaction() {
			LogManager.Log(LogCategory, () => {
				DropChanges();
			}, (d) => {
				return CreateLogMessage("Executing RollbackTransaction()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString()
			});
			}, null);
		}
		public void DropChanges() {
			OnBeforeDropChanges();
			if(!IsUnitOfWork && !TrackingChanges)
				throw new TransactionSequenceException(Res.GetString(Res.Session_TranSequenceRollback));
			SessionStateStack.Enter(this, SessionState.RollbackTransaction);
			try {
				collectionsMarkedSaved.Clear();
				objectsMarkedSaved.Clear();
				objectsMarkedDeleted.Clear();
				trackingChanges = false;
			} finally {
				SessionStateStack.Leave(this, SessionState.RollbackTransaction);
			}
			OnAfterDropChanges();
		}
		public void ExplicitBeginTransaction() {
			((ICommandChannel)this).Do(CommandChannelHelper.Command_ExplicitBeginTransaction, null);
		}
#if !SL
		public void ExplicitBeginTransaction(IsolationLevel il) {
			((ICommandChannel)this).Do(CommandChannelHelper.Command_ExplicitBeginTransaction, il);
		}
#endif
		public void ExplicitCommitTransaction() {
			((ICommandChannel)this).Do(CommandChannelHelper.Command_ExplicitCommitTransaction, null);
		}
		public void ExplicitRollbackTransaction() {
			((ICommandChannel)this).Do(CommandChannelHelper.Command_ExplicitRollbackTransaction, null);
		}
		public XPObjectType GetObjectType(Int32 id) {
			return TypesManager.GetObjectType(id);
		}
		public XPObjectType GetObjectType(object theObject) {
			return GetObjectType(GetClassInfo(theObject));
		}
		public XPObjectType GetObjectType(XPClassInfo objectType) {
			return TypesManager.GetObjectType(objectType);
		}
		public XPClassInfo GetClassInfo(string assemblyName, string className) {
			return Dictionary.GetClassInfo(assemblyName, className);
		}
		public XPClassInfo GetClassInfo(Type classType) {
			return Dictionary.GetClassInfo(classType);
		}
		public XPClassInfo GetClassInfo<ClassType>() {
			return this.GetClassInfo(typeof(ClassType));
		}
		public XPClassInfo GetClassInfo(object theObject) {
			return Dictionary.GetClassInfo(theObject);
		}
		IDictionary props;
		public PropertyDescriptorCollection GetProperties(XPClassInfo classInfo) {
			if(props == null)
				props = new Dictionary<object, object>();
			PropertyDescriptorCollection prop = (PropertyDescriptorCollection)props[classInfo];
			if(prop == null) {
				prop = new XPPropertyDescriptorCollection(this, classInfo);
				props[classInfo] = prop;
			}
			return prop;
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SessionAutoCreateOption")]
#endif
		[DefaultValue(AutoCreateOption.DatabaseAndSchema)]
		public AutoCreateOption AutoCreateOption {
			get {
				IObjectLayerEx objectLayerEx = objectLayer as IObjectLayerEx;
				return objectLayerEx == null ? _autoCreateOption : objectLayerEx.AutoCreateOption;
			}
			set {
				if(IsConnected)
					throw new CannotChangePropertyWhenSessionIsConnectedException("autocreate");
				_autoCreateOption = value;
			}
		}
#if !SL
#if !SL
	[DevExpressXpoLocalizedDescription("SessionConnection")]
#endif
		[DefaultValue(null)]
		public IDbConnection Connection {
			get {
				if(this._connection != null)
					return this._connection;
				IObjectLayerEx objectLayerEx = objectLayer as IObjectLayerEx;
				if(objectLayerEx != null)
					return objectLayerEx.Connection;
				return null;
			}
			set {
				if(IsConnected)
					throw new CannotChangePropertyWhenSessionIsConnectedException("connection");
				this._connectionString = string.Empty;
				this._connection = value;
			}
		}
#endif
		bool ShouldSerializeConnectionString() {
			return this._connectionString != null && this._connectionString.Length > 0;
		}
#if !SL
#if !SL
	[DevExpressXpoLocalizedDescription("SessionConnectionString")]
#endif
		public string ConnectionString {
			get {
				if(this._connection != null)
					return this._connection.ConnectionString;
				if(this.IsConnected && this.Connection != null && this.Connection.ConnectionString != null)
					return this.Connection.ConnectionString;
				return this._connectionString;
			}
			set {
				if(IsConnected)
					throw new CannotChangePropertyWhenSessionIsConnectedException("connection string");
				this.Connection = null;
				_connectionString = value;
			}
		}
#endif
#if !SL
	[DevExpressXpoLocalizedDescription("SessionLockingOption")]
#endif
		[DefaultValue(LockingOption.Optimistic)]
		public LockingOption LockingOption {
			get { return _lockingOption; }
			set {
				_lockingOption = value;
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("SessionOptimisticLockingReadBehavior"),
#endif
 DefaultValue(OptimisticLockingReadBehavior.Default)]
		public OptimisticLockingReadBehavior OptimisticLockingReadBehavior {
			get { return _OptimisticLockingReadBehavior; }
			set {
				_OptimisticLockingReadBehavior = value;
			}
		}
		[Obsolete("Use GetIdentityMapBehavior() instead")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public CacheBehavior GetCacheBehavior() {
			return (CacheBehavior)(int)GetIdentityMapBehavior();
		}
		public IdentityMapBehavior GetIdentityMapBehavior() {
			IdentityMapBehavior cacheBehavior = IdentityMapBehavior;
			if(cacheBehavior == IdentityMapBehavior.Default)
				cacheBehavior = XpoDefault.IdentityMapBehavior;
			return cacheBehavior;
		}
		[DefaultValue(CacheBehavior.Default)]
		[Obsolete("Use IdentityMapBehavior instead")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CacheBehavior CacheBehavior {
			get { return (CacheBehavior)(int)IdentityMapBehavior; }
			set {
				IdentityMapBehavior = (IdentityMapBehavior)(int)value;
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("SessionIdentityMapBehavior"),
#endif
 DefaultValue(IdentityMapBehavior.Default)]
		public IdentityMapBehavior IdentityMapBehavior {
			get { return _IdentityMapBehavior; }
			set {
				_IdentityMapBehavior = value;
			}
		}
		public void Connect(IDataLayer layer, params IDisposable[] disposeOnDisconnect) {
			Connect(SimpleObjectLayer.FromDataLayer(layer), disposeOnDisconnect);
		}
		public void Connect(IObjectLayer layer, params IDisposable[] disposeOnDisconnect) {
			if(isDisposed)
				throw new ObjectDisposedException(this.GetType().FullName);
			if(IsConnected)
				throw new InvalidOperationException(Res.GetString(Res.Session_AlreadyConnected));
			SessionInitConnected(layer, disposeOnDisconnect);
		}
#if !SL
		void ConnectOldStyle() {
			OnBeforeConnect();
			this.closeConnectionOnDisconnect = false;
			if(this._connection != null) {
				closeConnectionOnDisconnect = _connection.State != ConnectionState.Open;
				this.objectLayer = SimpleObjectLayer.FromDataLayer(XpoDefault.GetDataLayer(this._connection, this.dict, this.AutoCreateOption, out this._DisposeOnDisconnect));
			} else {
				this.objectLayer = SimpleObjectLayer.FromDataLayer(XpoDefault.GetDataLayer(this.ConnectionString, this.dict, this.AutoCreateOption, out this._DisposeOnDisconnect));
			}
			this.dict = this.objectLayer.Dictionary;
			OnAfterConnect();
		}
#endif
		public void Connect() {
			if(this.isDisposed)
				throw new ObjectDisposedException(this.ToString());
			if(IsConnected)
				return;
#if !SL
			if(this._connection != null || (this._connectionString != null && this._connectionString.Length > 0)) {
				ConnectOldStyle();
			} else
#endif
 {
				IObjectLayer layer = XpoDefault.ObjectLayer;
				if(layer == null) {
					layer = SimpleObjectLayer.FromDataLayer(XpoDefault.DataLayer);
				}
				if(layer != null) {
					if(dict != null && dict != layer.Dictionary)
						throw new InvalidOperationException(Res.GetString(Res.Session_DictConstractor));
					Connect(layer);
				} else {
#if !SL
					ConnectOldStyle();
#endif
				}
			}
#if !SL
			PerformanceCounters.SessionCount.Increment();
			PerformanceCounters.SessionConnected.Increment();
#endif
		}
		[Obsolete("Use DropIdentityMap() instead")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void DropCache() {
			DropIdentityMap();
		}
		public void DropIdentityMap() {
			if(TrackingChanges)
				DropChanges();
			if(_IdentityMap != null) {
				_IdentityMap.Clear();
				_IdentityMap = null;
			}
			typesManager = null;
		}
		void Clear() {
			DropIdentityMap();
			trackingChanges = false;
			if(this._DisposeOnDisconnect != null) {
				foreach(IDisposable disp in this._DisposeOnDisconnect) {
					if(disp != null) {
						disp.Dispose();
					}
				}
			}
			this.objectLayer = null;
			this._DisposeOnDisconnect = null;
#if !SL
			if(_connection != null && this.closeConnectionOnDisconnect) {
				try {
					this._connection.Close();
				} catch { }
			}
#endif
		}
		public void Disconnect() {
			if(InTransaction)
				RollbackTransaction();
			if(IsConnected) {
				OnBeforeDisconnect();
				Clear();
				OnAfterDisconnect();
#if !SL
				PerformanceCounters.SessionCount.Decrement();
				PerformanceCounters.SessionDisconnected.Increment();
#endif
			}
		}
		Dictionary<object, object> wideDataDictionary;
		Dictionary<object, object> WideDataDictionary {
			get {
				if(wideDataDictionary == null)
					wideDataDictionary = new Dictionary<object, object>();
				return wideDataDictionary;
			}
		}
		bool IWideDataStorage.WideDataContainsKey(object key) {
			return WideDataDictionary.ContainsKey(key);
		}
		void IWideDataStorage.SetWideDataItem(object key, object value) {
			WideDataDictionary[key] = value;
		}
		object IWideDataStorage.GetWideDataItem(object key) {
			return WideDataDictionary[key];
		}
		bool IWideDataStorage.TryGetWideDataItem(object key, out object value) {
			return WideDataDictionary.TryGetValue(key, out value);
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IObjectLayer ObjectLayer {
			get {
				if(!IsConnected) {
#if !SL
					if(IsDesignMode)
						throw new InvalidOperationException();
#endif
					Connect();
				}
				return objectLayer;
			}
		}
		public UpdateSchemaResult UpdateSchema(bool dontCreateIfFirstTableNotExist, params XPClassInfo[] types) {
			return ((IObjectLayerEx)ObjectLayer).UpdateSchema(dontCreateIfFirstTableNotExist, types);
		}
		public void UpdateSchema(params XPClassInfo[] types) {
			UpdateSchema(false, types);
		}
		public void UpdateSchema(params Type[] types) {
			UpdateSchema(Dictionary.CollectClassInfos(types));
		}
		public void UpdateSchema(params Assembly[] assemblies) {
			UpdateSchema(Dictionary.CollectClassInfos(assemblies));
		}
#if !CF && !SL
		Assembly[] GetNonXpoAssemblies(Assembly[] input) {
			if(input == null)
				throw new ArgumentNullException("input");
			List<Assembly> result = new List<Assembly>(input.Length);
			foreach(Assembly a in input) {
				if(a != typeof(Session).Assembly)
					result.Add(a);
			}
			return result.ToArray();
		}
		public void UpdateSchema() {
			UpdateSchema(GetNonXpoAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
		}
		public void CreateObjectTypeRecords() {
			CreateObjectTypeRecords(false);
		}
		public void CreateObjectTypeRecords(bool createOnlyNecessary) {
			CreateObjectTypeRecords(createOnlyNecessary, GetNonXpoAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
		}
#endif
		public void CreateObjectTypeRecords(params XPClassInfo[] types) {
			CreateObjectTypeRecords(false, types);
		}
		public void CreateObjectTypeRecords(bool createOnlyNecessary, params XPClassInfo[] types) {
			if(types == null)
				return;
			foreach(XPClassInfo type in types) {
				if(type.ClassType is IXPOServiceClass || (createOnlyNecessary && !type.IsTypedObject && !type.IsGCRecordObject && !type.HasPurgebleObjectReferences()))
					continue;
				GetObjectType(type);
			}
		}
		public void CreateObjectTypeRecords(params Type[] types) {
			CreateObjectTypeRecords(false, types);
		}
		public void CreateObjectTypeRecords(bool createOnlyNecessary, params Type[] types) {
			CreateObjectTypeRecords(createOnlyNecessary, Dictionary.CollectClassInfos(types));
		}
		public void CreateObjectTypeRecords(params Assembly[] assemblies) {
			CreateObjectTypeRecords(false, assemblies);
		}
		public void CreateObjectTypeRecords(bool createOnlyNecessary, params Assembly[] assemblies) {
			CreateObjectTypeRecords(createOnlyNecessary, Dictionary.CollectClassInfos(assemblies));
		}
		[Browsable(false)]
		public virtual XPDictionary Dictionary {
			get {
				if(IsConnected)
					return dict;
#if !CF && !SL
				if(IsDesignMode2) {
					if(dict == null) {
						if(this is DefaultSession)
							dict = CreateDesignTimeDictionary(Site);
						else
							throw new InvalidOperationException();
					}
				} else
#endif
					Connect();
				return dict;
			}
		}
		[Browsable(false)]
		public bool IsConnected {
			get { return objectLayer != null; }
		}
		[Browsable(false)]
		public XPObjectTypesManager TypesManager {
			get {
				if(typesManager == null)
					typesManager = new XPObjectTypesManager(this);
				return typesManager;
			}
		}
		[Browsable(false)]
		public virtual bool InTransaction {
			get { return TrackingChanges; }
		}
		[Browsable(false)]
		public bool TrackingChanges {
			get { return trackingChanges; }
		}
		[Obsolete("Use ObjectSaving event instead", true), EditorBrowsable(EditorBrowsableState.Never)]
		public event ObjectManipulationEventHandler BeforeSave {
			add { ObjectSaving += value; }
			remove { ObjectSaving -= value; }
		}
		public event ObjectManipulationEventHandler ObjectSaving;
		public event ObjectManipulationEventHandler ObjectSaved;
		public event ObjectManipulationEventHandler ObjectLoading;
		public event ObjectManipulationEventHandler ObjectLoaded;
		public event ObjectManipulationEventHandler ObjectDeleting;
		public event ObjectManipulationEventHandler ObjectDeleted;
		public event ObjectChangeEventHandler ObjectChanged;
		public event ObjectsManipulationEventHandler ObjectsSaved;
		public event ObjectsManipulationEventHandler ObjectsLoaded;
		[Description("Occurs before the connection to a database is established via the Connect method")]
		public event SessionManipulationEventHandler BeforeConnect;
		[Description("Occurs after the connection to a database has been established via the Connect method")]
		public event SessionManipulationEventHandler AfterConnect;
		[Description("Occurs before the connection to a database is detached via the Disconnect method")]
		public event SessionManipulationEventHandler BeforeDisconnect;
		[Description("Occurs after the connection to a database is detached via the Disconnect method")]
		public event SessionManipulationEventHandler AfterDisconnect;
		[Description("Occurs before the transaction starts via the BeginTransaction method")]
		public event SessionManipulationEventHandler BeforeBeginTransaction;
		[Description("Occurs after the transaction has started via the BeginTransaction method")]
		public event SessionManipulationEventHandler AfterBeginTransaction;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event SessionManipulationEventHandler BeforePreProcessCommitedList;
		[Description("Occurs before the transaction is completed via the CommitTransaction method")]
		public event SessionManipulationEventHandler BeforeCommitTransaction;
		[Description("Occurs after the transaction has completed via the CommitTransaction method")]
		public event SessionManipulationEventHandler AfterCommitTransaction;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event SessionManipulationEventHandler BeforeCommitNestedUnitOfWork;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event SessionManipulationEventHandler AfterCommitNestedUnitOfWork;
		[Description("Occurs before the transaction is finished via the RollbackTransaction method")]
		public event SessionManipulationEventHandler BeforeRollbackTransaction;
		[Description("Occurs after the transaction has completed via the RollbackTransaction method")]
		public event SessionManipulationEventHandler AfterRollbackTransaction;
		[Description("Occurs before the tracking changes mode starts via the BeginTrackingChanges method")]
		public event SessionManipulationEventHandler BeforeBeginTrackingChanges;
		[Description("Occurs after the tracking changes mode starts via the BeginTrackingChanges method")]
		public event SessionManipulationEventHandler AfterBeginTrackingChanges;
		[Description("Occurs before changes are flushed via the FlushChanges method")]
		public event SessionManipulationEventHandler BeforeFlushChanges;
		[Description("Occurs after changes are flushed via the FlushChanges method")]
		public event SessionManipulationEventHandler AfterFlushChanges;
		[Description("Occurs before changes are dropped via the DropChanges method")]
		public event SessionManipulationEventHandler BeforeDropChanges;
		[Description("Occurs after changes are dropped via the DropChanges method")]
		public event SessionManipulationEventHandler AfterDropChanges;
		public event SessionOperationFailEventHandler FailedCommitTransaction;
		public event SessionOperationFailEventHandler FailedFlushChanges;
		public object GetKeyValue(object theObject) {
			if(theObject == null)
				throw new ArgumentNullException(LogParam_TheObject);
			return Dictionary.GetId(theObject);
		}
		public void SetKeyValue(object theObject, object keyValue) {
			if(theObject == null)
				throw new ArgumentNullException(LogParam_TheObject);
			this.GetClassInfo(theObject).KeyProperty.SetValue(theObject, keyValue);
		}
		[Obsolete("Use BulkLoad method instead", true), EditorBrowsable(EditorBrowsableState.Never)]
		public void Load(params XPBaseCollection[] collections) {
			BulkLoad(collections);
		}
		public void BulkLoad(params XPBaseCollection[] collections) {
			BulkLoad((IXPBulkLoadableCollection[])collections);
		}
		public void BulkLoad(params IXPBulkLoadableCollection[] collections) {
			if(collections == null)
				throw new ArgumentNullException("collections");
			List<ObjectsQuery> queries = new List<ObjectsQuery>();
			List<IXPBulkLoadableCollection> colls = new List<IXPBulkLoadableCollection>();
			for(int i = 0; i < collections.Length; i++) {
				if(collections[i] == null)
					continue;
				ObjectsQuery query = collections[i].BeginLoad();
				if(query == null)
					continue;
				queries.Add(query);
				colls.Add(collections[i]);
			}
			ICollection[] obejcts = GetObjects(queries.ToArray());
			for(int i = 0; i < obejcts.Length; i++) {
				colls[i].EndLoad(obejcts[i]);
			}
		}
		protected virtual internal bool IsUnitOfWork {
			get { return false; }
		}
		volatile static int seq = 0;
		int seqNum = ++seq;
		public override string ToString() {
			return base.ToString() + '(' + seqNum.ToString() + ')';
		}
		protected internal virtual MemberInfoCollection GetPropertiesListForUpdateInsert(object theObject, bool isUpdate, bool addDelayedReference) {
			return XPClassInfo.GetPropertiesListForUpdateInsert(this, theObject, isUpdate, addDelayedReference);
		}
		object IPersistentValueExtractor.ExtractPersistentValue(object criterionValue) {
			ThrowIfObjectFromDifferentSession(criterionValue);
			if(Dictionary.QueryClassInfo(criterionValue) == null)
				return criterionValue;
			else if(IsNewObject(criterionValue, true))
				return null;
			else
				return GetKeyValue(criterionValue);
		}
		Session ISessionProvider.Session {
			get { return this; }
		}
		[Obsolete("Use PreFetch method instead", true), EditorBrowsable(EditorBrowsableState.Never)]
		public void Load<T>(IEnumerable<T> objects, params string[] collections) where T : class {
			PreFetch(objects, collections);
		}
		public void PreFetch<T>(IEnumerable<T> objects, params string[] collections) where T : class {
			PreFetch(GetClassInfo<T>(), objects, collections);
		}
		[Obsolete("Use PreFetch method instead", true), EditorBrowsable(EditorBrowsableState.Never)]
		public void Load(XPClassInfo classInfo, IEnumerable objects, params string[] collections) {
			PreFetch(classInfo, objects, collections);
		}
		public void PreFetch(XPClassInfo classInfo, IEnumerable objects, params string[] collections) {
			foreach(string path in collections) {
				string[] membersPath = MemberInfoCollection.SplitPath(path);
				if(membersPath.Length == 0)
					continue;
				XPMemberInfo member = classInfo.GetMember(membersPath[0]);
				ICollection uniqueObject = null;
				if(member.IsAssociationList || member.IsManyToManyAlias) {
					ObjectsUniquer(ref uniqueObject, objects);
					XPMemberInfo coreMember = member.IsAssociationList ? member : classInfo.GetMember(((ManyToManyAliasAttribute)member.GetAttributeInfo(typeof(ManyToManyAliasAttribute))).OneToManyCollectionName);
					PreFetchCore(classInfo, uniqueObject, coreMember);
					if(membersPath.Length > 1) {
						List<object> subObjects = new List<object>();
						foreach(object obj in uniqueObject) {
							ICollection collection = (ICollection)member.GetValue(obj);
							subObjects.AddRange(ListHelper.FromCollection(collection));
						}
						PreFetch(member.CollectionElementType, subObjects, string.Join(".", membersPath, 1, membersPath.Length - 1));
					}
				} else {
					if(member.IsPersistent && member.IsDelayed) {
						ObjectsUniquer(ref uniqueObject, objects);
						if(member.ReferenceType == null) {
							XPDelayedProperty.PreFetchGenericProps(this, uniqueObject, member);
						} else {
							XPDelayedProperty.PreFetchRefProps(this, uniqueObject, member);
						}
					}
					if(membersPath.Length > 1 && member.ReferenceType != null) {
						IList subObjects = new List<object>();
						foreach(object obj in objects) {
							subObjects.Add(member.GetValue(obj));
						}
						PreFetch(member.ReferenceType, subObjects, string.Join(".", membersPath, 1, membersPath.Length - 1));
					}
				}
			}
		}
		void ObjectsUniquer(ref ICollection uniqueObjects, IEnumerable nonUniqueObjects) {
			if(uniqueObjects != null)
				return;
			ObjectSet result = new ObjectSet();
			foreach(object obj in nonUniqueObjects) {
				if(obj == null)
					continue;
				ThrowIfObjectFromDifferentSession(obj);
				if(IsNewObject(obj))
					continue;
				result.Add(obj);
			}
			uniqueObjects = result;
		}
		internal void ThrowIfObjectFromDifferentSession(object obj) {
			ISessionProvider sessionObj = obj as ISessionProvider;
			if(sessionObj != null && sessionObj.Session != this)
				throw new SessionMixingException(sessionObj.Session, obj);
		}
		void PreFetchCore(XPClassInfo classInfo, IEnumerable objects, XPMemberInfo member) {
			if(member.IsManyToManyAlias) {
				ManyToManyAliasAttribute mm = (ManyToManyAliasAttribute)member.GetAttributeInfo(typeof(ManyToManyAliasAttribute));
				XPMemberInfo aliasedMI = classInfo.GetMember(mm.OneToManyCollectionName);
				PreFetchCore(classInfo, objects, aliasedMI);
				return;
			}
			List<object> ids = new List<object>();
			ObjectDictionary<ObjectSet> objectsByOwners = new ObjectDictionary<ObjectSet>();
			foreach(object obj in objects) {
				ThrowIfObjectFromDifferentSession(obj);
				IXPPrefetchableAssociationList coll = member.GetValue(obj) as IXPPrefetchableAssociationList;
				if(coll == null)
					continue;
				if(!coll.NeedPrefetch())
					continue;
				ids.Add(obj);
				objectsByOwners.Add(obj, new ObjectSet());
			}
			if(ids.Count == 0)
				return;
			List<ObjectsQuery> queries = new List<ObjectsQuery>();
			int n;
			XPClassInfo loadedClassInfo = member.IsManyToMany ? member.IntermediateClass : member.CollectionElementType;
			for(int i = 0; i < ids.Count; i += n) {
				n = XpoDefault.GetTerminalInSize(ids.Count - i);
				queries.Add(new ObjectsQuery(loadedClassInfo, new InOperator(member.GetAssociatedMember().Name, GetRangeHelper.GetRange(ids, i, n)), null, 0, 0, new CollectionCriteriaPatcher(false, TypesManager), false));
			}
			ICollection[] results = GetObjects(queries.ToArray());
			XPMemberInfo relatedMember = member.IsManyToMany ? member.IntermediateClass.GetMember(member.GetAssociatedMember().Name) : member.GetAssociatedMember();
			XPMemberInfo gcMember = loadedClassInfo.FindMember(GCRecordField.StaticName);
			foreach(ICollection list in results) {
				foreach(object obj in list) {
					if(IsObjectToDelete(obj, true))
						continue;
					if(gcMember != null && gcMember.GetValue(obj) != null)
						continue;
					object parent = relatedMember.GetValue(obj);
					if(parent == null)
						continue;
					ObjectSet hint;
					if(!objectsByOwners.TryGetValue(parent, out hint))
						continue;
					hint.Add(obj);
				}
			}
			foreach(object obj in GetObjectsToSave(true)) {
				if(IsObjectToDelete(obj, true))
					continue;
				if(!GetClassInfo(obj).IsAssignableTo(loadedClassInfo))
					continue;
				if(gcMember != null && gcMember.GetValue(obj) != null)
					continue;
				object parent = relatedMember.GetValue(obj);
				if(parent == null)
					continue;
				ObjectSet hint;
				if(!objectsByOwners.TryGetValue(parent, out hint))
					continue;
				hint.Add(obj);
			}
			foreach(KeyValuePair<object, ObjectSet> pair in objectsByOwners) {
				IXPPrefetchableAssociationList coll = (IXPPrefetchableAssociationList)member.GetValue(pair.Key);
				coll.FinishPrefetch(pair.Value);
			}
		}
		public void PreFetch(IEnumerable objects, XPMemberInfo collectionInObjects, IEnumerable collectionsContent) {
			if(collectionInObjects == null)
				throw new ArgumentNullException("collectionInObjects");
			if(!collectionInObjects.IsAssociationList)
				throw new ArgumentException(Res.GetString(Res.Session_AssociationListExpected), "collectionInObjects");
			IXPBulkLoadableCollection bl1 = objects as IXPBulkLoadableCollection;
			IXPBulkLoadableCollection bl2 = collectionsContent as IXPBulkLoadableCollection;
			if(bl1 != null && bl2 != null && bl1 != bl2)
				BulkLoad(bl1, bl2);
			ObjectDictionary<ObjectSet> map = new ObjectDictionary<ObjectSet>();
			foreach(object owner in objects) {
				IXPPrefetchableAssociationList pref = collectionInObjects.GetValue(owner) as IXPPrefetchableAssociationList;
				if(pref == null)
					continue;
				if(!pref.NeedPrefetch())
					continue;
				map.Add(owner, new ObjectSet());
			}
			if(map.Count == 0)
				return;
			XPMemberInfo assMember = collectionInObjects.GetAssociatedMember();
			foreach(object obj in collectionsContent) {
				object owner = assMember.GetValue(obj);
				if(owner == null)
					continue;
				ObjectSet set;
				if(!map.TryGetValue(owner, out set))
					continue;
				set.Add(obj);
			}
			foreach(KeyValuePair<object, ObjectSet> pair in map) {
				IXPPrefetchableAssociationList pref = (IXPPrefetchableAssociationList)collectionInObjects.GetValue(pair.Key);
				pref.FinishPrefetch(pair.Value);
			}
		}
		public void PreFetch<T>(IEnumerable<T> objects, string collectionInObjects, IEnumerable collectionsContent) {
			XPMemberInfo cMember = GetClassInfo<T>().GetMember(collectionInObjects);
			PreFetch(objects, cMember, collectionsContent);
		}
		protected internal virtual bool SuppressExceptionOnReferredObjectAbsentInDataStore {
			get { return GlobalSuppressExceptionOnReferredObjectAbsentInDataStore; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool GlobalSuppressExceptionOnReferredObjectAbsentInDataStore = false;
#if SL
		internal bool IsDesignMode {
			get {
				return false;
			}
		}
		bool IsDesignMode2 {
			get { return DesignMode; }
		}
#else
		bool? _isDesignMode;
		internal bool IsDesignMode {
			get {
				if(_isDesignMode == true)
					return true;
				return DevExpress.Data.Helpers.IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _isDesignMode);
			}
		}
		bool IsDesignMode2 {
			get {
				return IsDesignMode;
			}
		}
#endif
		IDictionary<XPBaseCollection, object> mutedCollections;
		internal void MuteCollection(XPBaseCollection c) {
			if(mutedCollections == null)
				mutedCollections = new Dictionary<XPBaseCollection, object>();
			else if(mutedCollections.ContainsKey(c))
				return;
			mutedCollections.Add(c, c);
			c.SuspendChangedEvents();
		}
		void UnMuteCollections() {
			if(mutedCollections == null)
				return;
			ICollection<XPBaseCollection> toUnMute = mutedCollections.Keys;
			mutedCollections = null;
			List<Exception> exceptions = new List<Exception>();
			foreach(XPBaseCollection unMuted in toUnMute) {
				try {
					unMuted.ResumeChangedEvents();
				} catch(Exception e) {
					exceptions.Add(e);
				}
			}
			if(exceptions.Count > 0)
				throw new ExceptionBundleException(exceptions.ToArray());
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IDataLayer DataLayer {
			get {
				IObjectLayerEx objectLayerEx = ObjectLayer as IObjectLayerEx;
				return objectLayerEx == null ? null : objectLayerEx.DataLayer;
			}
		}
		public ICollection<T> GetObjectsFromSprocParametrized<T>(string sprocName, params SprocParameter[] parameters) {
			return ListHelper.FromCollection<T>(GetObjectsFromSprocParametrized(GetClassInfo<T>(), sprocName, parameters));
		}
		public ICollection GetObjectsFromSprocParametrized(XPClassInfo classInfo, string sprocName, params SprocParameter[] parameters) {
			return GetObjectsFromSprocParametrized(classInfo, classInfo.PersistentProperties as List<XPMemberInfo>, sprocName, parameters);
		}
		public ICollection<T> GetObjectsFromSprocParametrized<T>(List<XPMemberInfo> members, string sprocName, params SprocParameter[] parameters) {
			return ListHelper.FromCollection<T>(GetObjectsFromSprocParametrized(GetClassInfo<T>(), members, sprocName, parameters));
		}
		public ICollection GetObjectsFromSprocParametrized(XPClassInfo classInfo, List<XPMemberInfo> members, string sprocName, params SprocParameter[] parameters) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict = CheckClassInfoAndMembers(classInfo, members);
				SelectedData sprocResultData = ExecuteSprocParametrized(sprocName, parameters);
				return GetObjectsFromData(classInfo, members, null, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromSprocParametrized()", d, new object[] { 
					LogParam_SessionType, this.GetType().ToString(), LogParam_SessionID, this.ToString(), LogParam_ClassInfo, classInfo.FullName,
					LogParam_Members, LogMessage.CollectionToString<XPMemberInfo>(members, mi => mi == null ? "null" : mi.Name ),
					LogParam_SprocName, sprocName,
					LogParam_Parameters, LogMessage.CriteriaOperatorCollectionToString<OperandValue>(parameters)
				});
			});
		}
		public ICollection<T> GetObjectsFromSproc<T>(string sprocName, params OperandValue[] parameters) {
			return ListHelper.FromCollection<T>(GetObjectsFromSproc(GetClassInfo<T>(), sprocName, parameters));
		}
		public ICollection GetObjectsFromSproc(XPClassInfo classInfo, string sprocName, params OperandValue[] parameters) {
			return GetObjectsFromSproc(classInfo, classInfo.PersistentProperties as List<XPMemberInfo>, sprocName, parameters);
		}
		public ICollection<T> GetObjectsFromSproc<T>(List<XPMemberInfo> members, string sprocName, params OperandValue[] parameters) {
			return ListHelper.FromCollection<T>(GetObjectsFromSproc(GetClassInfo<T>(), members, sprocName, parameters));
		}
		public ICollection GetObjectsFromSproc(XPClassInfo classInfo, List<XPMemberInfo> members, string sprocName, params OperandValue[] parameters) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict = CheckClassInfoAndMembers(classInfo, members);
				SelectedData sprocResultData = ExecuteSproc(sprocName, parameters);
				return GetObjectsFromData(classInfo, members, null, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromSproc()", d, new object[] { 
					LogParam_SessionType, this.GetType().ToString(), LogParam_SessionID, this.ToString(), LogParam_ClassInfo, classInfo.FullName,
					LogParam_Members, LogMessage.CollectionToString<XPMemberInfo>(members, mi => mi == null ? "null" : mi.Name ),
					LogParam_SprocName, sprocName,
					LogParam_Parameters, LogMessage.CriteriaOperatorCollectionToString<OperandValue>(parameters)
				});
			});
		}
		public ICollection<T> GetObjectsFromQuery<T>(string sql) {
			return ListHelper.FromCollection<T>(GetObjectsFromQuery(GetClassInfo<T>(), sql));
		}
		public ICollection<T> GetObjectsFromQuery<T>(string sql, object[] parameterValues) {
			return ListHelper.FromCollection<T>(GetObjectsFromQuery(GetClassInfo<T>(), sql, parameterValues));
		}
		public ICollection<T> GetObjectsFromQuery<T>(string sql, string[] parameterNames, object[] parameterValues) {
			return ListHelper.FromCollection<T>(GetObjectsFromQuery(GetClassInfo<T>(), sql, parameterNames, parameterValues));
		}
		public ICollection GetObjectsFromQuery(XPClassInfo classInfo, string sql) {
			return GetObjectsFromQuery(classInfo, classInfo.PersistentProperties as List<XPMemberInfo>, sql);
		}
		public ICollection GetObjectsFromQuery(XPClassInfo classInfo, string sql, object[] parameterValues) {
			return GetObjectsFromQuery(classInfo, classInfo.PersistentProperties as List<XPMemberInfo>, sql, parameterValues);
		}
		public ICollection GetObjectsFromQuery(XPClassInfo classInfo, string sql, string[] parameterNames, object[] parameterValues) {
			return GetObjectsFromQuery(classInfo, classInfo.PersistentProperties as List<XPMemberInfo>, sql, parameterNames, parameterValues);
		}
		public ICollection<T> GetObjectsFromQuery<T>(List<XPMemberInfo> members, string sql) {
			return ListHelper.FromCollection<T>(GetObjectsFromQuery(GetClassInfo<T>(), members, sql));
		}
		public ICollection<T> GetObjectsFromQuery<T>(List<XPMemberInfo> members, string sql, object[] parameterValues) {
			return ListHelper.FromCollection<T>(GetObjectsFromQuery(GetClassInfo<T>(), members, sql, parameterValues));
		}
		public ICollection<T> GetObjectsFromQuery<T>(List<XPMemberInfo> members, string sql, string[] parameterNames, object[] parameterValues) {
			return ListHelper.FromCollection<T>(GetObjectsFromQuery(GetClassInfo<T>(), members, sql, parameterNames, parameterValues));
		}
		public ICollection GetObjectsFromQuery(XPClassInfo classInfo, List<XPMemberInfo> members, string sql) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict = CheckClassInfoAndMembers(classInfo, members);
				SelectedData sprocResultData = ExecuteQuery(sql);
				return GetObjectsFromData(classInfo, members, null, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromQuery()", d, new object[] { 
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Members, LogMessage.CollectionToString<XPMemberInfo>(members, arg => arg == null ? "null" : arg.Name),
				LogParam_Sql, sql
				});
			});
		}
		static string ParametersToString(string[] parameterNames, object[] parameterValues) {
			return LogMessage.CollectionToString(parameterNames, parameterValues, (name, value) => string.Format("{0}:{1}", string.IsNullOrEmpty(name) ? "_" : name, value == null ? "null" : value.ToString()));
		}
		static string ParametersToString(object[] parameterValues) {
			return LogMessage.CollectionToString<object>(parameterValues, arg => arg == null ? "null" : arg.ToString() );
		}
		public ICollection GetObjectsFromQuery(XPClassInfo classInfo, List<XPMemberInfo> members, string sql, object[] parameterValues) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict = CheckClassInfoAndMembers(classInfo, members);
				SelectedData sprocResultData = ExecuteQuery(sql, parameterValues);
				return GetObjectsFromData(classInfo, members, null, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromQuery()", d, new object[] { 
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Members, LogMessage.CollectionToString<XPMemberInfo>(members, arg => arg == null ? "null" : arg.Name ),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterValues)
				});
			});
		}
		public ICollection GetObjectsFromQuery(XPClassInfo classInfo, List<XPMemberInfo> members, string sql, string[] parameterNames, object[] parameterValues) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict = CheckClassInfoAndMembers(classInfo, members);
				SelectedData sprocResultData = ExecuteQuery(sql, parameterNames, parameterValues);
				return GetObjectsFromData(classInfo, members, null, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromQuery()", d, new object[] { 
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Members, LogMessage.CollectionToString<XPMemberInfo>(members, arg => arg == null ? "null" : arg.Name),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterNames, parameterValues)
				});
			});
		}
		static Dictionary<XPMemberInfo, int> CheckClassInfoAndMembers(XPClassInfo classInfo, List<XPMemberInfo> members) {
			if(members == null)
				return null;
			Dictionary<XPMemberInfo, int> referenceIndexDict = null;
			foreach(XPMemberInfo mi in members) {
				if(mi.Owner != classInfo && !classInfo.IsAssignableTo(mi.Owner))
					throw new InvalidOperationException(Res.GetString(Res.Session_NotClassMember, mi.Name, classInfo.FullName));
				if(mi.ReferenceType != null) {
					if(referenceIndexDict == null) {
						referenceIndexDict = new Dictionary<XPMemberInfo, int>();
					}
					referenceIndexDict.Add(mi, referenceIndexDict.Count);
				}
				if(mi.IsCollection || mi.IsAssociationList)
					throw new NotSupportedException(Res.GetString(Res.DirectSQL_CollectionMembersAreNotSupported));
			}
			return referenceIndexDict;
		}
		public ICollection<T> GetObjectsFromSprocParametrized<T>(LoadDataMemberOrderItem[] members, string sprocName, params SprocParameter[] parameters) {
			return ListHelper.FromCollection<T>(GetObjectsFromSprocParametrized(GetClassInfo<T>(), members, sprocName, parameters));
		}
		public ICollection GetObjectsFromSprocParametrized(XPClassInfo classInfo, LoadDataMemberOrderItem[] members, string sprocName, params SprocParameter[] parameters) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict;
				List<XPMemberInfo> memberInfos = CheckClassInfoAndMembers(classInfo, members, out referenceIndexDict);
				SelectedData sprocResultData = ExecuteSprocParametrized(sprocName, parameters);
				return GetObjectsFromData(classInfo, memberInfos, members, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromSprocParametrized()", d, new object[] { 
					LogParam_SessionType, this.GetType().ToString(),
					LogParam_SessionID, this.ToString(),
					LogParam_ClassInfo, classInfo.FullName,
					LogParam_Members, LogMessage.CollectionToString<LoadDataMemberOrderItem>(members, arg => arg.ClassMemberName ),
					LogParam_SprocName, sprocName,
					LogParam_Parameters, LogMessage.CriteriaOperatorCollectionToString<OperandValue>(parameters)
				});
			});
		}
		public ICollection<T> GetObjectsFromSproc<T>(LoadDataMemberOrderItem[] members, string sprocName, params OperandValue[] parameters) {
			return ListHelper.FromCollection<T>(GetObjectsFromSproc(GetClassInfo<T>(), members, sprocName, parameters));
		}
		public ICollection GetObjectsFromSproc(XPClassInfo classInfo, LoadDataMemberOrderItem[] members, string sprocName, params OperandValue[] parameters) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict;
				List<XPMemberInfo> memberInfos = CheckClassInfoAndMembers(classInfo, members, out referenceIndexDict);
				SelectedData sprocResultData = ExecuteSproc(sprocName, parameters);
				return GetObjectsFromData(classInfo, memberInfos, members, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromSproc()", d, new object[] { 
					LogParam_SessionType, this.GetType().ToString(),
					LogParam_SessionID, this.ToString(),
					LogParam_ClassInfo, classInfo.FullName,
					LogParam_Members, LogMessage.CollectionToString<LoadDataMemberOrderItem>(members, arg => arg.ClassMemberName ),
					LogParam_SprocName, sprocName,
					LogParam_Parameters, LogMessage.CriteriaOperatorCollectionToString<OperandValue>(parameters)
				});
			});
		}
		public ICollection<T> GetObjectsFromQuery<T>(LoadDataMemberOrderItem[] members, string sql) {
			return ListHelper.FromCollection<T>(GetObjectsFromQuery(GetClassInfo<T>(), members, sql));
		}
		public ICollection<T> GetObjectsFromQuery<T>(LoadDataMemberOrderItem[] members, string sql, object[] parameterValues) {
			return ListHelper.FromCollection<T>(GetObjectsFromQuery(GetClassInfo<T>(), members, sql, parameterValues));
		}
		public ICollection<T> GetObjectsFromQuery<T>(LoadDataMemberOrderItem[] members, string sql, string[] parameterNames, object[] parameterValues) {
			return ListHelper.FromCollection<T>(GetObjectsFromQuery(GetClassInfo<T>(), members, sql, parameterNames, parameterValues));
		}
		public ICollection GetObjectsFromQuery(XPClassInfo classInfo, LoadDataMemberOrderItem[] members, string sql) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict;
				List<XPMemberInfo> memberInfos = CheckClassInfoAndMembers(classInfo, members, out referenceIndexDict);
				SelectedData sprocResultData = ExecuteQuery(sql);
				return GetObjectsFromData(classInfo, memberInfos, members, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromQuery()", d, new object[] { 
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Members, LogMessage.CollectionToString<LoadDataMemberOrderItem>(members, arg => arg.ClassMemberName ),
				LogParam_Sql, sql
				});
			});
		}
		public ICollection GetObjectsFromQuery(XPClassInfo classInfo, LoadDataMemberOrderItem[] members, string sql, object[] parameterValues) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict;
				List<XPMemberInfo> memberInfos = CheckClassInfoAndMembers(classInfo, members, out referenceIndexDict);
				SelectedData sprocResultData = ExecuteQuery(sql, parameterValues);
				return GetObjectsFromData(classInfo, memberInfos, members, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromQuery()", d, new object[] { 
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Members, LogMessage.CollectionToString<LoadDataMemberOrderItem>(members, arg => arg.ClassMemberName ),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterValues)
				});
			});
		}
		public ICollection GetObjectsFromQuery(XPClassInfo classInfo, LoadDataMemberOrderItem[] members, string sql, string[] parameterNames, object[] parameterValues) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				Dictionary<XPMemberInfo, int> referenceIndexDict;
				List<XPMemberInfo> memberInfos = CheckClassInfoAndMembers(classInfo, members, out referenceIndexDict);
				SelectedData sprocResultData = ExecuteQuery(sql, parameterNames, parameterValues);
				return GetObjectsFromData(classInfo, memberInfos, members, sprocResultData, referenceIndexDict);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsFromQuery()", d, new object[] { 
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_Members, LogMessage.CollectionToString<LoadDataMemberOrderItem>(members, arg => arg.ClassMemberName ),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterNames, parameterValues)
				});
			});
		}
		static List<XPMemberInfo> CheckClassInfoAndMembers(XPClassInfo classInfo, LoadDataMemberOrderItem[] members, out Dictionary<XPMemberInfo, int> referenceIndexDict) {
			referenceIndexDict = null;
			List<XPMemberInfo> memberInfos = new List<XPMemberInfo>(members.Length);
			for(int i = 0; i < members.Length; i++) {
				XPMemberInfo mi = classInfo.FindMember(members[i].ClassMemberName);
				if(mi.ReferenceType != null) {
					if(referenceIndexDict == null) {
						referenceIndexDict = new Dictionary<XPMemberInfo, int>();
					}
					referenceIndexDict.Add(mi, referenceIndexDict.Count);
				}
				if(mi.IsCollection || mi.IsAssociationList)
					throw new NotSupportedException(Res.GetString(Res.DirectSQL_CollectionMembersAreNotSupported));
				memberInfos.Add(mi);
			}
			return memberInfos;
		}
		ICollection GetObjectsFromData(XPClassInfo classInfo, List<XPMemberInfo> memberInfos, LoadDataMemberOrderItem[] membersOrder, SelectedData sprocResultData, Dictionary<XPMemberInfo, int> referenceIndexDict) {
			if(classInfo.IsPersistent)
				throw new ArgumentException(Res.GetString(Res.MetaData_PersistentReferenceFound, classInfo.FullName), "classInfo");
			if(sprocResultData == null || sprocResultData.ResultSet == null || sprocResultData.ResultSet.Length == 0)
				return new object[0];
			SelectStatementResult sprocResult = sprocResultData.ResultSet[0];
			if(sprocResult == null || sprocResult.Rows == null || sprocResult.Rows.Length == 0)
				return new object[0];
			SelectStatementResultRow[] rows = sprocResult.Rows;
			object[] result = new object[rows.Length];
			List<object> primaryObjects = referenceIndexDict == null ? null : new List<object>();
			List<object>[] referenceKeyList = referenceIndexDict == null ? null : new List<object>[referenceIndexDict.Count];
			if(referenceIndexDict != null) {
				for(int i = 0; i < referenceKeyList.Length; i++) {
					referenceKeyList[i] = new List<object>();
				}
			}
			for(int r = 0; r < rows.Length; r++) {
				SelectStatementResultRow row = rows[r];
				object theObject = classInfo.CreateNewObject(this);
				if(memberInfos != null) {
					bool allNulls = true;
					object[] keys = referenceIndexDict == null ? null : new object[referenceIndexDict.Count];
					if(membersOrder == null) {
						if(memberInfos.Count != row.Values.Length)
							throw new InvalidOperationException(Res.GetString(Res.DirectSQL_WrongColumnCount));
					}
					int minCount = memberInfos.Count;
					for(int m = 0; m < minCount; m++) {
						XPMemberInfo mi = memberInfos[m];
						object value = row.Values[membersOrder == null ? m : membersOrder[m].IndexInResultSet];
						if(mi.ReferenceType != null) {
							if(referenceIndexDict == null)
								throw new InvalidOperationException(Res.GetString(Res.Session_InternalXPOError));
							if(value == null || value is DBNull)
								continue;
							allNulls = false;
							keys[referenceIndexDict[mi]] = value;
							continue;
						}
						if(value is DBNull)
							value = null;
						mi.SetValue(theObject, mi.Converter == null ? value : mi.Converter.ConvertFromStorageType(value));
					}
					if(!allNulls) {
						primaryObjects.Add(theObject);
						for(int i = 0; i < referenceKeyList.Length; i++) {
							referenceKeyList[i].Add(keys[i]);
						}
					}
				}
				result[r] = theObject;
			}
			if(referenceIndexDict != null && primaryObjects.Count > 0) {
				List<ObjectsByKeyQuery> queries = new List<ObjectsByKeyQuery>(referenceIndexDict.Count);
				foreach(KeyValuePair<XPMemberInfo, int> referenceIndex in referenceIndexDict) {
					queries.Add(new ObjectsByKeyQuery(referenceIndex.Key.ReferenceType, referenceKeyList[referenceIndex.Value]));
				}
				ICollection[] referenceObjects = GetObjectsByKey(queries.ToArray(), false);
				foreach(KeyValuePair<XPMemberInfo, int> referenceIndexPair in referenceIndexDict) {
					int primaryObjectIndex = 0;
					foreach(object referenceObject in referenceObjects[referenceIndexPair.Value]) {
						referenceIndexPair.Key.SetValue(primaryObjects[primaryObjectIndex++], referenceObject);
					}
				}
			}
			TriggerObjectsLoaded(result);
			return result;
		}
		public ICollection GetObjectsByKeyFromSproc(XPClassInfo classInfo, bool alwaysGetFromDb, string sprocName, params OperandValue[] parameters) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				SelectedData sprocResultData = ExecuteSproc(sprocName, parameters);
				return GetObjectsByKeyFromQuery(classInfo, alwaysGetFromDb, sprocResultData);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsByKeyFromSproc()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_AlwaysGetFromDb, alwaysGetFromDb,
				LogParam_SprocName, sprocName,
				LogParam_Parameters, LogMessage.CriteriaOperatorCollectionToString<OperandValue>(parameters)
				});
			});
		}
		public ICollection GetObjectsByKeyFromSprocParametrized(XPClassInfo classInfo, bool alwaysGetFromDb, string sprocName, params SprocParameter[] parameters) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				SelectedData sprocResultData = ExecuteSprocParametrized(sprocName, parameters);
				return GetObjectsByKeyFromQuery(classInfo, alwaysGetFromDb, sprocResultData);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsByKeyFromSprocParametrized()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_AlwaysGetFromDb, alwaysGetFromDb,
				LogParam_SprocName, sprocName,
				LogParam_Parameters, LogMessage.CriteriaOperatorCollectionToString<OperandValue>(parameters)
				});
			});
		}
		public ICollection GetObjectsByKeyFromQuery(XPClassInfo classInfo, bool alwaysGetFromDb, string sql) {
			return LogManager.Log<ICollection>(LogCategory, () => {
				SelectedData sprocResultData = ExecuteQuery(sql);
				return GetObjectsByKeyFromQuery(classInfo, alwaysGetFromDb, sprocResultData);
			}, (d) => {
				return CreateLogMessage("Executing GetObjectsByKeyFromQuery()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_ClassInfo, classInfo.FullName,
				LogParam_AlwaysGetFromDb, alwaysGetFromDb,
				LogParam_Sql, sql
				});
			});
		}
		ICollection GetObjectsByKeyFromQuery(XPClassInfo classInfo, bool alwaysGetFromDb, SelectedData sprocResultData) {
			if(sprocResultData == null || sprocResultData.ResultSet == null || sprocResultData.ResultSet.Length == 0)
				return new object[0];
			SelectStatementResult sprocResult = sprocResultData.ResultSet[0];
			if(sprocResult == null || sprocResult.Rows == null || sprocResult.Rows.Length == 0)
				return new object[0];
			SelectStatementResultRow[] rows = sprocResult.Rows;
			object[] idCollection = new object[rows.Length];
			for(int i = 0; i < rows.Length; i++) {
				idCollection[i] = rows[i].Values[0];
			}
			return GetObjectsByKey(classInfo, idCollection, alwaysGetFromDb);
		}
		public SelectedData ExecuteSproc(string sprocName, params OperandValue[] parameters) {
			return LogManager.Log<SelectedData>(LogCategory, () => {
				return CommandChannelHelper.ExecuteSproc(this, sprocName, parameters);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteSproc()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_SprocName, sprocName,
				LogParam_Parameters, LogMessage.CollectionToString<OperandValue>(parameters, ov => ov.ToString() )
				});
			});
		}
		public SelectedData ExecuteSprocParametrized(string sprocName, params SprocParameter[] parameters) {
			return LogManager.Log<SelectedData>(LogCategory, () => {
				return CommandChannelHelper.ExecuteSprocParametrized(this, sprocName, parameters);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteSprocParametrized()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_SprocName, sprocName,
				LogParam_Parameters, LogMessage.CollectionToString<SprocParameter>(parameters, ov => ov.ToString() )
				});
			});
		}
		public int ExecuteNonQuery(string sql) {
			return LogManager.Log<int>(LogCategory, () => {
				return CommandChannelHelper.ExecuteNonQuery(this, sql);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteNonQuery()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql
				});
			});
		}
		public int ExecuteNonQuery(string sql, string[] parameterNames, object[] parameterValues) {
			QueryParameterCollection paramValues = new QueryParameterCollection();
			foreach(object item in parameterValues)
				paramValues.Add(new OperandValue(item));
			return LogManager.Log<int>(LogCategory, () => {
				return CommandChannelHelper.ExecuteNonQueryWithParams(this, sql, paramValues, parameterNames);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteNonQuery()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterNames, parameterValues)
				});
			});
		}
		public int ExecuteNonQuery(string sql, object[] parameterValues) {
			QueryParameterCollection paramValues = new QueryParameterCollection();
			foreach(object item in parameterValues)
				paramValues.Add(new OperandValue(item));
			return LogManager.Log<int>(LogCategory, () => {
				return CommandChannelHelper.ExecuteNonQueryWithParams(this, sql, paramValues, new string[0]);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteNonQuery()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterValues)
				});
			});
		}
		public object ExecuteScalar(string sql) {
			return LogManager.Log<object>(LogCategory, () => {
				return CommandChannelHelper.ExecuteScalar(this, sql);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteScalar()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql
				});
			});
		}
		public object ExecuteScalar(string sql, string[] parameterNames, object[] parameterValues) {
			QueryParameterCollection paramValues = new QueryParameterCollection();
			foreach(object item in parameterValues)
				paramValues.Add(new OperandValue(item));
			return LogManager.Log<object>(LogCategory, () => {
				return CommandChannelHelper.ExecuteScalarWithParams(this, sql, paramValues, parameterNames);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteScalar()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql, 
				LogParam_Parameters, ParametersToString(parameterNames, parameterValues)
				});
			});
		}
		public object ExecuteScalar(string sql, object[] parameterValues) {
			QueryParameterCollection paramValues = new QueryParameterCollection();
			foreach(object item in parameterValues)
				paramValues.Add(new OperandValue(item));
			return LogManager.Log<object>(LogCategory, () => {
				return CommandChannelHelper.ExecuteScalarWithParams(this, sql, paramValues, new string[0]);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteScalar()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql, 
				LogParam_Parameters, ParametersToString(parameterValues)
				});
			});
		}
		public SelectedData ExecuteQuery(string sql) {
			return LogManager.Log<SelectedData>(LogCategory, () => {
				return CommandChannelHelper.ExecuteQuery(this, sql);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteQuery()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql
				});
			});
		}
		public SelectedData ExecuteQuery(string sql, string[] parameterNames, object[] parameterValues) {
			QueryParameterCollection paramValues = new QueryParameterCollection();
			foreach(object item in parameterValues)
				paramValues.Add(new OperandValue(item));
			return LogManager.Log<SelectedData>(LogCategory, () => {
				return CommandChannelHelper.ExecuteQueryWithParams(this, sql, paramValues, parameterNames);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteQuery()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterNames, parameterValues)
				});
			});
		}
		public SelectedData ExecuteQuery(string sql, object[] parameterValues) {
			QueryParameterCollection paramValues = new QueryParameterCollection();
			foreach(object item in parameterValues)
				paramValues.Add(new OperandValue(item));
			return LogManager.Log<SelectedData>(LogCategory, () => {
				return CommandChannelHelper.ExecuteQueryWithParams(this, sql, paramValues, new string[0]);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteQuery()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterValues)
				});
			});
		}
		public SelectedData ExecuteQueryWithMetadata(string sql) {
			return LogManager.Log<SelectedData>(LogCategory, () => {
				return CommandChannelHelper.ExecuteQueryWithMetadata(this, sql);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteQueryWithMetadata()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql
				});
			});
		}
		public SelectedData ExecuteQueryWithMetadata(string sql, string[] parameterNames, object[] parameterValues) {
			QueryParameterCollection paramValues = new QueryParameterCollection();
			foreach(object item in parameterValues)
				paramValues.Add(new OperandValue(item));
			return LogManager.Log<SelectedData>(LogCategory, () => {
				return CommandChannelHelper.ExecuteQueryWithMetadataWithParams(this, sql, paramValues, parameterNames);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteQueryWithMetadata()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterNames, parameterValues)
				});
			});
		}
		public SelectedData ExecuteQueryWithMetadata(string sql, object[] parameterValues) {
			QueryParameterCollection paramValues = new QueryParameterCollection();
			foreach(object item in parameterValues)
				paramValues.Add(new OperandValue(item));
			return LogManager.Log<SelectedData>(LogCategory, () => {
				return CommandChannelHelper.ExecuteQueryWithMetadataWithParams(this, sql, paramValues, new string[0]);
			}, (d) => {
				return CreateLogMessage("Executing ExecuteQueryWithMetadata()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString(),
				LogParam_Sql, sql,
				LogParam_Parameters, ParametersToString(parameterValues)
				});
			});
		}
		object ICommandChannel.Do(string command, object args) {
			ICommandChannel commandChannel = ObjectLayer as ICommandChannel;
			if(commandChannel == null) {
				if(ObjectLayer == null) {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupported, command));
				} else {
					throw new NotSupportedException(string.Format(CommandChannelHelper.Message_CommandIsNotSupportedEx, command, ObjectLayer.GetType().FullName));
				}
			}
			return commandChannel.Do(command, args);
		}
		protected delegate object[] LogParametersHandler();
		protected LogMessage CreateLogMessage(string title, TimeSpan duration, params object[] parameters) {
			LogMessage message = new LogMessage(LogMessageType.SessionEvent, title, duration);
			if(parameters != null) {
				for(int i = 0; i < parameters.Length; i += 2) {
					message.ParameterList.Add(new LogMessageParameter((string)parameters[i], parameters[i + 1]));
				}
			}
			return message;
		}
		protected string GetObjectString(object theObject) {
			if(theObject == null)
				return "null";
			Type objectType = theObject.GetType();
			XPClassInfo classInfo = Dictionary.QueryClassInfo(objectType);
			if(classInfo == null || classInfo.KeyProperty == null)
				return objectType.ToString();
			return string.Format("{0}({1})", objectType, classInfo.GetId(theObject));
		}
	}
	[Description("Loads and saves persistent objects keeping track of every change to every persistent object during a transaction.")]
#if !SL
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabOrmComponents)]
#endif
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "UnitOfWork")]
	public class UnitOfWork : Session {
		protected internal override bool IsUnitOfWork {
			get {
				return true;
			}
		}
		public UnitOfWork()
			: base() { }
		public UnitOfWork(IContainer container)
			: base(container) { }
		public UnitOfWork(XPDictionary dictionary)
			: base(dictionary) { }
		public UnitOfWork(IDataLayer layer, params IDisposable[] disposeOnDisconnect)
			: base(layer, disposeOnDisconnect) { }
		public UnitOfWork(IObjectLayer layer, params IDisposable[] disposeOnDisconnect)
			: base(layer, disposeOnDisconnect) { }
		public void CommitChanges() {
			WaitForAsyncOperationEnd();
			if(!InTransaction)
				return;
			CommitTransaction();
		}
		public object CommitChangesAsync(AsyncCommitCallback callback) {
			if(!InTransaction) {
				callback(null);
				return null;
			}
			return CommitTransactionAsync(callback);
		}
		public void ReloadChangedObjects() {
			if(!InTransaction)
				return;
			ObjectSet objects2reload = new ObjectSet();
			foreach(object obj in GetObjectsToDelete()) {
				objects2reload.Add(obj);
			}
			foreach(object obj in GetObjectsToSave()) {
				objects2reload.Add(obj);
			}
			foreach(object obj in objects2reload) {
				try {
					Reload(obj);
				} catch(CannotLoadObjectsException) { }
			}
			System.Diagnostics.Debug.Assert(GetObjectsToDelete().Count == 0 && GetObjectsToSave().Count == 0);
			RollbackTransaction();	
		}
	}
#if !SL
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabOrmComponents)]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "ExplicitUnitOfWork")]
	public class ExplicitUnitOfWork : UnitOfWork {
		public ExplicitUnitOfWork()
			: base() { }
		public ExplicitUnitOfWork(IContainer container)
			: base(container) { }
		public ExplicitUnitOfWork(XPDictionary dictionary)
			: base(dictionary) { }
		public ExplicitUnitOfWork(IDataLayer layer, params IDisposable[] disposeOnDisconnect)
			: base(layer, disposeOnDisconnect) { }
		public ExplicitUnitOfWork(IObjectLayer layer, params IDisposable[] disposeOnDisconnect)
			: base(layer, disposeOnDisconnect) { }
		bool inTransaction;
#if !SL
	[DevExpressXpoLocalizedDescription("ExplicitUnitOfWorkInTransaction")]
#endif
		public override bool InTransaction {
			get {
				return inTransaction || TrackingChanges;
			}
		}
		public override void BeginTransaction() {
			LogManager.Log(LogCategory, () => {
				OnBeforeBeginTransaction();
				if(inTransaction)
					throw new TransactionSequenceException(Res.GetString(Res.Session_TranSequenceBegin));
				ExplicitBeginTransaction();
				inTransaction = true;
				OnAfterBeginTransaction();
			}, (d) => {
				return CreateLogMessage("Executing BeginTransaction()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString()
				});
			}, null);
		}
		public override void RollbackTransaction() {
			LogManager.Log(LogCategory, () => {
				OnBeforeRollbackTransaction();
				if(TrackingChanges)
					DropChanges();
				if(inTransaction) {
					ExplicitRollbackTransaction();
					inTransaction = false;
				}
				OnAfterRollbackTransaction();
			}, (d) => {
				return CreateLogMessage("Executing RollbackTransaction()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString()
				});
			}, null);
		}
		public override void CommitTransaction() {
			LogManager.Log(LogCategory, () => {
				OnBeforeCommitTransaction();
				if(TrackingChanges) {
					FlushChanges();
				}
				if(inTransaction) {
					ExplicitCommitTransaction();
					inTransaction = false;
				}
				OnAfterCommitTransaction();
			}, (d) => {
				return CreateLogMessage("Executing CommitTransaction()", d, new object[] {
				LogParam_SessionType, this.GetType().ToString(),
				LogParam_SessionID, this.ToString()
				});
			}, (ex) => {
				return !OnFailedCommitTransaction(ex);
			});
		}
		public override object CommitTransactionAsync(AsyncCommitCallback callback) {
			try {
				OnBeforeCommitTransaction();
				if(TrackingChanges) {
					return FlushChangesAsync(new AsyncCommitCallback(delegate(Exception ex) {
						try {
							if(ex == null) {
								if(inTransaction) {
									ExplicitCommitTransaction();
									inTransaction = false;
								}
								OnAfterCommitTransaction();
							}
						} catch(Exception e) {
							if(!OnFailedCommitTransaction(e)) {
								callback(e);
							}
							return;
						}
						if(ex == null || !OnFailedCommitTransaction(ex)) {
							callback(ex);
						}
					}));
				} else {
					if(inTransaction) {
						ExplicitCommitTransaction();
						inTransaction = false;
					}
					OnAfterCommitTransaction();
				}
			} catch(Exception syncEx) {
				if(!OnFailedCommitTransaction(syncEx)) {
					throw;
				}
			}
			return null;
		}
		protected override bool IsInTransactionMode {
			get { return false; }
		}
		protected override void OnBeforeBeginTrackingChanges() {
			OnBeforeBeginTrackingChangesInternal();
		}
		protected override void OnAfterBeginTrackingChanges() {
			OnAfterBeginTrackingChangesInternal();
		}
		protected override void OnBeforeFlushChanges() {
			OnBeforeFlushChangesInternal();
		}
		protected override void OnAfterFlushChanges() {
			OnAfterFlushChangesInternal();
		}
		protected override void OnBeforeDropChanges() {
			OnBeforeDropChangesInternal();
		}
		protected override void OnAfterDropChanges() {
			OnAfterDropChangesInternal();
		}
		protected override bool OnFailedFlushChanges(Exception ex) {
			return OnFailedFlushChangesInternal(ex);
		}
		protected override IList BeginFlushChanges() {
			if(!inTransaction) {
				BeginTransaction();
			}
			return base.BeginFlushChanges();
		}
		internal override List<object[]> SelectDataInternal(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, bool selectDeleted, int skipSelectedRecords, int topSelectedRecords, SortingCollection sorting) {
			List<object[]> result = PrepareSelectData(classInfo, ref properties, ref criteria, ref groupProperties, ref groupCriteria, ref sorting, true, this);
			if(result == null) {
				ObjectsQuery preparedQuery = new ObjectsQuery(classInfo, criteria, sorting, skipSelectedRecords, topSelectedRecords, new CollectionCriteriaPatcher(selectDeleted, TypesManager), true);
				if(IsFlushRequired(new ObjectsQuery[] { preparedQuery })) {
					FlushChanges();
				}
				result = ObjectLayer.SelectData(this, preparedQuery, properties, groupProperties, groupCriteria);
			}
			return result;
		}
		internal override object SelectDataAsyncInternal(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, bool selectDeleted, int skipSelectedRecords, int topSelectedRecords, SortingCollection sorting, AsyncSelectDataCallback callback) {
			if(SynchronizationContext.Current == null)
				throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Async_OperationCannotBePerformedBecauseNoSyncContext));
			if(callback == null)
				throw new ArgumentNullException();
			List<object[]> result = PrepareSelectData(classInfo, ref properties, ref criteria, ref groupProperties, ref groupCriteria, ref sorting, true, this);
			if(result != null) {
				return ObjectLayer.SelectDataAsync(this, null, properties, groupProperties, groupCriteria, callback);
			} else {
				ObjectsQuery preparedQuery = new ObjectsQuery(classInfo, criteria, sorting, skipSelectedRecords, topSelectedRecords, new CollectionCriteriaPatcher(selectDeleted, TypesManager), true);
				if(IsFlushRequired(new ObjectsQuery[] { preparedQuery })) {
					return FlushChangesAsync(new AsyncCommitCallback(delegate(Exception ex) {
						if(ex != null) {
							callback(null, ex);
							return;
						}
						try {
							ObjectLayer.SelectDataAsync(this, preparedQuery, properties, groupProperties, groupCriteria, callback);
						} catch(Exception e) {
							callback(null, e);
						}
					}));
				}
				return ObjectLayer.SelectDataAsync(this, preparedQuery, properties, groupProperties, groupCriteria, callback);
			}
		}
		internal override ICollection[] GetObjectsInternal(ObjectsQuery[] queries) {
			if(IsFlushRequired(queries)) {
				FlushChanges();
			}
			return base.GetObjectsInternal(queries);
		}
		internal override object GetObjectsInternalAsync(ObjectsQuery[] queries, AsyncLoadObjectsCallback callback) {
			if(SynchronizationContext.Current == null)
				throw new InvalidOperationException(Xpo.Res.GetString(Xpo.Res.Async_OperationCannotBePerformedBecauseNoSyncContext));
			if(callback == null)
				throw new ArgumentNullException();
			if(IsFlushRequired(queries)) {
				return FlushChangesAsync(new AsyncCommitCallback(delegate(Exception ex) {
					if(ex != null) {
						callback(null, ex);
						return;
					}
					try {
						GetObjectsInternalAsync(queries, callback);
					} catch(Exception e) {
						callback(null, e);
					}
				}));
			}
			return base.GetObjectsInternalAsync(queries, callback);
		}
		bool IsFlushRequired(ObjectsQuery[] queries) {
			bool hasModifiedNodes = false;
			if(TrackingChanges) {
				ICollection objectsToSave = GetObjectsToSave(true);
				ICollection objectsToDelete = GetObjectsToDelete(true);
				if(objectsToSave.Count != 0 || objectsToDelete.Count != 0) {
					ObjectsQuery[] preparedQueries = PrepareQueries(queries, false, this);
					for(int i = 0; i < preparedQueries.Length; i++) {
						AnalyzeResult analyzeResult;
						string[] modifiedNodes = InTransactionLoader.GetModifiedNodes(this, objectsToSave, objectsToDelete, preparedQueries[i], out analyzeResult);
						if(modifiedNodes.Length != 0) {
							hasModifiedNodes = true;
							break;
						}
					}
				}
			}
			return hasModifiedNodes;
		}
	}
#endif
	public struct LoadDataMemberOrderItem {
		public readonly int IndexInResultSet;
		public readonly string ClassMemberName;
		public LoadDataMemberOrderItem(int indexInResultSet, string classMemberName) {
			this.IndexInResultSet = indexInResultSet;
			this.ClassMemberName = classMemberName;
		}
	}
}
