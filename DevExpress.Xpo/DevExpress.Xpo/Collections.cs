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
using System.Runtime.Serialization;
using System.ComponentModel;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.Xpo.Metadata;
using System.Globalization;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo.Generators;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Helpers;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Xpo.Helpers {
	public interface IXPImmutableHashCode {
		int GetImmutableHashCode();
	}
	public class ObjectDictionary<T> : Dictionary<object, T> {
		sealed class ObjectComparer : IEqualityComparer<object> {
			bool IEqualityComparer<object>.Equals(object x, object y) {
				return x == y;
			}
			int IEqualityComparer<object>.GetHashCode(object obj) {
				var bo = obj as IXPImmutableHashCode;
				if(bo != null)
					return bo.GetImmutableHashCode();
				else
					return obj.GetHashCode();
			}
			public static ObjectComparer Instance = new ObjectComparer();
		}
		public ObjectDictionary() : base(ObjectComparer.Instance) { }
		public ObjectDictionary(int capacity) : base(capacity, ObjectComparer.Instance) { }
	}
	public class ObjectSet : ICollection {
		readonly ObjectDictionary<Byte> Dict;
		public ObjectSet() : this(0) { }
		public ObjectSet(int capacity) {
			this.Dict = new ObjectDictionary<byte>(capacity);
		}
		public void Add(object item) {
			Dict[item] = 0;
		}
		public void Clear() {
			Dict.Clear();
		}
		public bool Contains(object item) {
			return Dict.ContainsKey(item);
		}
		public void CopyTo(Array array, int arrayIndex) {
			((ICollection)Dict.Keys).CopyTo(array, arrayIndex);
		}
		public int Count {
			get { return Dict.Count; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public bool Remove(object item) {
			return Dict.Remove(item);
		}
		public IEnumerator GetEnumerator() {
			return Dict.Keys.GetEnumerator();
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public object SyncRoot {
			get { return this; }
		}
	}
	public class ObjectList : IList {
		public IList List;
		public const int TerminalSize = 128;
		public ObjectList() : this(0) { }
		public ObjectList(int suggestedSize) {
			this.List = new ArrayList(suggestedSize);
			ClearIndex();
		}
		public ObjectList(ICollection fillBy)
			: this(fillBy.Count) {
			foreach(object obj in fillBy)
				this.Add(obj);
		}
		public int Add(object value) {
			int index = Count;
			Insert(index, value);
			return index;
		}
		public void Clear() {
			List.Clear();
			ClearIndex();
		}
		public bool Contains(object value) {
			return IndexOf(value) >= 0;
		}
		ObjectDictionary<int> _Index;
		int firstInvalidIndex;
		void ClearIndex() {
			_Index = null;
			firstInvalidIndex = 0;
		}
		void InvalidateIndexAt(int index, object valueForRemove) {
			if(firstInvalidIndex > index)
				firstInvalidIndex = index;
			if(firstInvalidIndex <= 0)
				_Index = null;
			if(_Index != null && valueForRemove != null)
				_Index.Remove(valueForRemove);
		}
		public int IndexOf(object value) {
			if(value == null || Count <= 0)
				return -1;
			if(ReferenceEquals(this[0], value))
				return 0;
			if(_Index == null && Count > TerminalSize) {
				_Index = new ObjectDictionary<int>(Count);
				firstInvalidIndex = 0;
			}
			if(_Index == null) {
				for(int i = 0; i < Count; ++i) {
					if(ReferenceEquals(this[i], value)) {
						return i;
					}
				}
				return -1;
			} else {
				int ind;
				if(_Index.TryGetValue(value, out ind) && ind < firstInvalidIndex && ReferenceEquals(value, this[ind])) {
					return ind;
				}
				while(firstInvalidIndex < Count) {
					int currentIndex = firstInvalidIndex;
					_Index[this[currentIndex]] = currentIndex;
					++firstInvalidIndex;
					if(ReferenceEquals(this[currentIndex], value))
						return currentIndex;
				}
				return -1;
			}
		}
		public void Insert(int index, object value) {
			System.Diagnostics.Debug.Assert(!this.Contains(value));
			List.Insert(index, value);
			InvalidateIndexAt(index, null);
		}
		public bool IsFixedSize {
			get { return false; }
		}
		public bool IsReadOnly {
			get { return false; }
		}
		public void Remove(object value) {
			int index = IndexOf(value);
			if(index < 0)
				return;
			RemoveAt(index);
		}
		public void RemoveAt(int index) {
			object oldValue = this[index];
			List.RemoveAt(index);
			InvalidateIndexAt(index, oldValue);
		}
		public object this[int index] {
			get {
				return List[index];
			}
			set {
				object oldValue = this[index];
				List[index] = value;
				InvalidateIndexAt(index, oldValue);
			}
		}
		public void CopyTo(Array array, int index) {
			List.CopyTo(array, index);
		}
		public int Count {
			get { return List.Count; }
		}
		public bool IsSynchronized {
			get { return false; }
		}
		public object SyncRoot {
			get { return this; }
		}
		public IEnumerator GetEnumerator() {
			return List.GetEnumerator();
		}
	}
	public static class XPCollectionCompareHelper {
		static public IComparer CreateComparer(SortingCollection sortColumns, CriteriaCompilerDescriptor descriptor, CriteriaCompilerAuxSettings settings) {
			if(settings == null)
				throw new ArgumentNullException("settings");
			IComparer cmp = new DummyComparer();
			int n = sortColumns.Count;
			while(n-- > 0) {
				SortProperty col = sortColumns[n];
				var lmbda = CriteriaCompiler.ToLambda(col.Property, descriptor, settings);
				Type pType = lmbda.Body.Type;
				Type oType = descriptor.ObjectType;
				if(!typeof(IComparable).IsAssignableFrom(NullableHelpers.GetBoxedType(pType)))
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotResolvePropertyTypeComparer, col.PropertyName, pType.FullName));
				Type comparerType;
				if(pType == typeof(string)) {
					if(settings.CaseSensitive) {
						comparerType = typeof(StringComparerSensitive<>).MakeGenericType(oType);
					} else {
						comparerType = typeof(StringComparerInSensitive<>).MakeGenericType(oType);
					}
				} else {
					comparerType = typeof(DefaultComparer<,>).MakeGenericType(oType, pType);
				}
				cmp = (IComparer)Activator.CreateInstance(comparerType, lmbda.Compile(), cmp, col.Direction);
			}
			return cmp;
		}
		public static IComparer CreateDesignTimeComparer() {
			return new DummyComparer();
		}
		class DummyComparer : IComparer {
			public int Compare(object a, object b) {
				return 0;
			}
		}
		public abstract class BaseComparer<O, P> : IComparer {
			Func<O, P> PropertyValueGet;
			IComparer next;
			SortingDirection dir;
			public BaseComparer(Func<O, P> propertyValueGet, IComparer next, SortingDirection dir) {
				this.PropertyValueGet = propertyValueGet;
				this.next = next;
				this.dir = dir;
			}
			int IComparer.Compare(object a, object b) {
				int res = Compare(PropertyValueGet((O)a), PropertyValueGet((O)b));
				if(res == 0)
					return next.Compare(a, b);
				return dir == SortingDirection.Ascending ? res : -res;
			}
			protected abstract int Compare(P a, P b);
		}
		sealed class DefaultComparer<O, P> : BaseComparer<O, P> {
			public DefaultComparer(Func<O, P> pd, IComparer next, SortingDirection dir) : base(pd, next, dir) { }
			protected override int Compare(P a, P b) {
				return Comparer<P>.Default.Compare(a, b);
			}
		}
		public sealed class StringComparerInSensitive<O>: StringComparerBase<O> {
			public StringComparerInSensitive(Func<O, string> pd, IComparer next, SortingDirection dir)
				: base(pd, next, dir, System.StringComparer.CurrentCultureIgnoreCase) {
			}
		}
		public sealed class StringComparerSensitive<O>: StringComparerBase<O> {
			public StringComparerSensitive(Func<O, string> pd, IComparer next, SortingDirection dir)
				: base(pd, next, dir, System.StringComparer.CurrentCulture) {
			}
		}
		public abstract class StringComparerBase<O>: BaseComparer<O, string> {
			readonly System.StringComparer Comparer;
			public StringComparerBase(Func<O, string> pd, IComparer next, SortingDirection dir, System.StringComparer sCmp)
				: base(pd, next, dir) {
				this.Comparer = sCmp;
			}
			protected sealed override int Compare(string a, string b) {
				return Comparer.Compare(a, b);
			}
		}
		static object GetObjectFromPuncturedCollection(IList objects, int puncturedAt, int x) {
			if(puncturedAt < 0 || x < puncturedAt)
				return objects[x];
			else
				return objects[x + 1];
		}
		static public int GetPos(IComparer cmp, object obj, IList objects, int puncturedAt, int dreamPos) {
			int min = 0;
			int max = puncturedAt < 0 ? objects.Count : objects.Count - 1;
			if(dreamPos >= 0) {
				if(min <= dreamPos && dreamPos < max) {
					int comparisionResult = cmp.Compare(GetObjectFromPuncturedCollection(objects, puncturedAt, dreamPos), obj);
					if(comparisionResult == 0)
						return dreamPos;
					else if(comparisionResult < 0)
						min = dreamPos + 1;
					else
						max = dreamPos;
				}
				if(min < dreamPos && dreamPos <= max) {
					int comparisionResult = cmp.Compare(GetObjectFromPuncturedCollection(objects, puncturedAt, dreamPos - 1), obj);
					if(comparisionResult == 0)
						return dreamPos;
					else if(comparisionResult < 0)
						min = dreamPos;
					else
						max = dreamPos - 1;
				}
			}
			while(min < max) {
				int x = (min + max) >> 1;
				int comparisionResult = cmp.Compare(GetObjectFromPuncturedCollection(objects, puncturedAt, x), obj);
				if(comparisionResult == 0 && dreamPos >= 0)
					comparisionResult = x < dreamPos ? -1 : 1;
				if(comparisionResult < 0) {
					min = x + 1;
				} else if(comparisionResult > 0) {
					max = x;
				} else
					return x;
			}
			return min;
		}
	}
	public class XPCollectionHelper {
		abstract class ChangeSubscriber {
			public abstract void Add(object obj);
			public abstract void Remove(object obj);		
		}
#if !SL
		class WeakChangeSubscriber : ChangeSubscriber {
			ObjectRecord eventHandler;
			public WeakChangeSubscriber(IObjectChange change) {
				eventHandler = ObjectRecord.GetObjectRecord(change);
			}
			public override void Add(object obj) {
				ObjectRecord.AddChangeHandler(obj, eventHandler);
			}
			public override void Remove(object obj) {
				ObjectRecord.RemoveChangeHandler(obj, eventHandler);
			}
		}
#endif
		class StrongChangeSubscriber : ChangeSubscriber {
			ObjectChangeEventHandler eventHandler;
			public StrongChangeSubscriber(IObjectChange change) {
				eventHandler = new ObjectChangeEventHandler(change.OnObjectChanged);
			}
			public override void Add(object obj) {
				XPBaseObject.AddChangedEventHandler(obj, eventHandler);
			}
			public override void Remove(object obj) {
				XPBaseObject.RemoveChangedEventHandler(obj, eventHandler);
			}
		}
		ChangeSubscriber _EventHandler;
		protected readonly XPBaseCollection ParentCollection;
		ChangeSubscriber EventHandler {
			get {
				if(_EventHandler == null) {
#if !SL
					if(Session.GetIdentityMapBehavior() == IdentityMapBehavior.Weak)
						_EventHandler = new WeakChangeSubscriber(ParentCollection);
					else
#endif
						_EventHandler = new StrongChangeSubscriber(ParentCollection);
				}
				return _EventHandler;
			}
		}
		public XPClassInfo ObjectClassInfo;
		public Session Session;
		IList objList;
		public IList ObjList {
			get {
				if(objList == null)
					objList = new ObjectList();
				return objList;
			}
		}
		public XPCollectionHelper(XPBaseCollection owner) {
			this.ParentCollection = owner;
		}
		public virtual int Add(object newObject) {
			return InternalAddObject(newObject);
		}
		public virtual void Remove(object theObject) {
			ObjList.Remove(theObject);
			EventHandler.Remove(theObject); ;
		}
		public virtual void BeforeAfterRemove(object theObject) {
		}
		public virtual XPClassInfo FetchObjectsClassInfo { get { return ObjectClassInfo; } }
		public virtual int InternalAddObject(object newObject) {
			int index = ObjList.Add(newObject);
			EventHandler.Add(newObject);
			return index;
		}
		public void ClearObjList() {
			int count = ObjList.Count;
			for(int i = 0; i < count; i++) {
				EventHandler.Remove(ObjList[i]); ;
			}
			ObjList.Clear();
		}
		public virtual ICollection IntObjList { get { return ObjList; } }
		public virtual void Reload() { }
		public virtual CriteriaOperator PatchCriteriaFromUserToFetch(CriteriaOperator fetchCriteria) {
			return fetchCriteria;
		}
		public virtual IEnumerable PatchLoadedCollectionWithChangesWhileNotLoaded(IEnumerable objects) {
			return objects;
		}
		public virtual IEnumerable GetContentWithoutQueryIfPossible() { return null; }
		public bool LoadCollectionOnModify {
			get { return LoadCollectionOnModifyCore || !ParentCollection.LoadingEnabled; }
		}
		protected virtual bool LoadCollectionOnModifyCore {
			get { return true; }
		}
		public virtual bool IsThereForDelete(object theObject) {
			return ObjList.Contains(theObject);
		}
		public virtual CriteriaOperator GetHardcodedCriterion() { return null; }
		public virtual void KickOutCancelledAddNew(object sender) {
		}
	}
	public abstract class XPRefCollectionHelper: XPCollectionHelper {
		public readonly object OwnerObject;
		public readonly XPMemberInfo RefProperty;
		protected XPRefCollectionHelper(XPBaseCollection parentCollection, object ownerObject, XPMemberInfo refProperty)
			: base(parentCollection) {
			this.OwnerObject = ownerObject;
			this.RefProperty = refProperty;
		}
		public override IEnumerable GetContentWithoutQueryIfPossible() {
			if(Session.ObjectLayer.CanLoadCollectionObjects) {
				return Session.ObjectLayer.LoadCollectionObjects(Session, RefProperty, OwnerObject);
			}
			return base.GetContentWithoutQueryIfPossible();
		}
		public abstract void ClearChangesCache();
		public abstract void Save();
		public static T GetRefCollectionHelperChecked<T>(XPBaseCollection collection, object ownerObject, XPMemberInfo collectionMember) where T: XPRefCollectionHelper {
			T xrch = collection.Helper as T;
			if(xrch == null)
				throw new InvalidOperationException(Res.GetString(Res.Collections_GeneralPurposeCollectionInsteadOfRefCollection, ownerObject, collectionMember.Name));
			return xrch;
		}
	}
	public sealed class XPRefCollectionHelperOneToMany : XPRefCollectionHelper {
		ObjectSet added;
		ObjectSet removed;
		public XPRefCollectionHelperOneToMany(XPBaseCollection parentCollection, object ownerObject, XPMemberInfo refProperty)
			: base(parentCollection, ownerObject, refProperty) {
		}
		public override CriteriaOperator GetHardcodedCriterion() {
			return new OperandProperty(RefProperty.GetAssociatedMember().Name) == new OperandValue(OwnerObject);
		}
		public override IEnumerable PatchLoadedCollectionWithChangesWhileNotLoaded(IEnumerable objects) {
			if(added == null && removed == null)
				return base.PatchLoadedCollectionWithChangesWhileNotLoaded(objects);
			List<object> rv = new List<object>();
			ObjectSet alreadyAdded = null;
			if(objects != null) {
				foreach(object obj in objects) {
					if(removed != null && removed.Contains(obj))
						continue;
					rv.Add(obj);
					if(added != null && added.Contains(obj)) {
						if(alreadyAdded == null)
							alreadyAdded = new ObjectSet();
						alreadyAdded.Add(obj);
					}
				}
			}
			if(added != null) {
				foreach(object obj in added) {
					if(alreadyAdded != null && alreadyAdded.Contains(obj))
						continue;
					rv.Add(obj);
				}
			}
			return rv;
		}
		public override void ClearChangesCache() {
			added = null;
			removed = null;
		}
		public override void Save() {
			if(!Session.IsUnitOfWork && RefProperty.IsAggregated) {
				ParentCollection.Load();
			}
			if(removed != null) {
				foreach(object o in removed) {
					IXPInvalidateableObject spoilableObject = o as IXPInvalidateableObject;
					if(spoilableObject != null && spoilableObject.IsInvalidated)
						continue;
					if(Session.IsNewObject(o))
						continue;
					Session.Save(o);
				}
			}
			if(RefProperty.IsAggregated && !Session.IsUnitOfWork) {
				foreach(object o in ObjList)
					Session.Save(o);
			} else {
				if(added != null)
					foreach(object o in added)
						Session.Save(o);
			}
			Session.collectionsMarkedSaved[this] = this;
		}
		void SetObjectPropertyValue(object theObject, object newValue) {
			GetObjectProperty().SetValue(theObject, newValue);
		}
		object GetObjectPropertyValue(object theObject) {
			return GetObjectProperty().GetValue(theObject);
		}
		XPMemberInfo GetObjectProperty() {
			return RefProperty.GetAssociatedMember(); ;
		}
		public override int Add(object newObject) {
			if(!ReferenceEquals(GetObjectPropertyValue(newObject), OwnerObject))
				SetObjectPropertyValue(newObject, OwnerObject);
			if(added == null)
				added = new ObjectSet(4);
			added.Add(newObject);
			if(removed != null)
				removed.Remove(newObject);
			return base.Add(newObject);
		}
		public override void Remove(object theObject) {
			if(theObject == null)
				throw new ArgumentNullException("theObject");
			System.Diagnostics.Debug.Assert(!ParentCollection.IsLoaded || ObjList.IndexOf(theObject) >= 0);
			base.Remove(theObject);
			if(added != null)
				added.Remove(theObject);
			if(removed == null)
				removed = new ObjectSet(4);
			removed.Add(theObject);
		}
		public override void BeforeAfterRemove(object theObject) {
			base.BeforeAfterRemove(theObject);
			if(ReferenceEquals(GetObjectPropertyValue(theObject), OwnerObject) && XPAssociationList.CanNullifyAssociatedMember(Session, GetObjectProperty(), theObject)) {
				SetObjectPropertyValue(theObject, null);
			}
		}
		public override void Reload() {
			ClearChangesCache();
			base.Reload();
		}
		protected override bool LoadCollectionOnModifyCore {
			get {
				return false;
			}
		}
		public object AssocRefChangeRemovingObject;
		public override bool IsThereForDelete(object theObject) {
			if(ParentCollection.IsLoaded)
				return base.IsThereForDelete(theObject);
			if(base.IsThereForDelete(theObject))
				return true;
			if(ReferenceEquals(AssocRefChangeRemovingObject, theObject))
				return true;
			return ReferenceEquals(OwnerObject, GetObjectPropertyValue(theObject));
		}
		public override void KickOutCancelledAddNew(object removedNew) {
			base.KickOutCancelledAddNew(removedNew);
			if(removed != null)
				removed.Remove(removedNew);
		}
	}
	public sealed class XPRefCollectionHelperManyToMany : XPRefCollectionHelper {
		ObjectDictionary<IntermediateObject> hash;
		List<IntermediateObject> added;
		List<IntermediateObject> removed;
		ObjectDictionary<IntermediateObject> Hash {
			get {
				if(hash == null)
					hash = new ObjectDictionary<IntermediateObject>();
				return hash;
			}
		}
		public override ICollection IntObjList {
			get {
				ArrayList list = new ArrayList();
				foreach(object obj in base.IntObjList)
					list.Add(GetIntermediateObject(obj));
				return list;
			}
		}
		public XPRefCollectionHelperManyToMany(XPBaseCollection parentCollection, object ownerObject, XPMemberInfo refProperty)
			: base(parentCollection, ownerObject, refProperty) {
		}
		public override CriteriaOperator GetHardcodedCriterion() {
			return new OperandProperty(RefProperty.GetAssociatedMember().Name) == new OperandValue(OwnerObject);
		}
		IntermediateObject GetIntermediateObject(object referred) {
			IntermediateObject rv;
			Hash.TryGetValue(referred, out rv);
			return rv;
		}
		public override CriteriaOperator PatchCriteriaFromUserToFetch(CriteriaOperator fetchCriteria) {
			return TopLevelPropertiesPrefixer.PatchAliasPrefix(RefProperty.Name + ".", fetchCriteria);
		}
		internal void AddIntermediateObject(IntermediateObject iObject, object referred) {
			if(GetIntermediateObject(referred) == null)
				Hash.Add(referred, iObject);
		}
		IntermediateObject CreateIntermediateObject(object referred) {
			IntermediateObject obj = GetIntermediateObject(referred);
			if(obj == null) {
				IntermediateClassInfo objInfo = RefProperty.IntermediateClass;
				obj = new IntermediateObject(Session, objInfo);
				objInfo.GetFieldInfo(RefProperty.GetAssociatedMember()).SetValue(obj, OwnerObject);
				objInfo.GetFieldInfo(RefProperty).SetValue(obj, referred);
			}
			return obj;
		}
		object GetReferredObject(IntermediateObject io) {
			return io.ClassInfo.GetMember(RefProperty.Name).GetValue(io);
		}
		XPBaseCollection GetReferredCollection(IntermediateObject io) {
			return GetReferredCollection((GetReferredObject(io)));
		}
		XPBaseCollection GetReferredCollection(object obj) {
			return ((XPBaseCollection)RefProperty.GetAssociatedMember().GetValue(obj));
		}
		XPRefCollectionHelperManyToMany GetReferredHelper(IntermediateObject io) {
			return (XPRefCollectionHelperManyToMany)GetReferredCollection(io).Helper;
		}
		public override void ClearChangesCache() {
#if DEBUGTEST && !SL
			if(Session.IsUnitOfWork && (added != null || removed != null))
				NUnit.Framework.Assert.Fail("added or removed should not be used within unit of work!!!");
#endif
			if(added != null) {
				foreach(IntermediateObject io in added) {
					XPRefCollectionHelperManyToMany refHelper = GetReferredHelper(io);
					if(refHelper.added != null) {
						refHelper.added.Remove(io);
					}
				}
				added = null;
			}
			if(removed != null) {
				foreach(IntermediateObject obj in removed) {
					Hash.Remove(GetReferredObject(obj));
					XPRefCollectionHelperManyToMany refHelper = GetReferredHelper(obj);
					refHelper.Hash.Remove(OwnerObject);
					if(refHelper.removed != null) {
						refHelper.removed.Remove(obj);
					}
				}
				removed = null;
			}
		}
		public override void Save() {
#if DEBUGTEST && !SL
			if(Session.IsUnitOfWork && (added != null || removed != null))
				NUnit.Framework.Assert.Fail("added or removed should not be used within unit of work!!!");
#endif
			if(added != null)
				foreach(IntermediateObject iob in added)
					iob.Save();
			if(removed != null) {
				foreach(IntermediateObject iob in removed) {
					if(((IXPInvalidateableObject)iob).IsInvalidated)
						continue;
					iob.Delete();
				}
			}
			Session.collectionsMarkedSaved[this] = this;
		}
		public override int Add(object newObject) {
			IntermediateObject iObj = CreateIntermediateObject(newObject);
			if(Session.IsUnitOfWork) {
				iObj.Save();
			} else {
				int index;
				if(removed != null && (index = removed.IndexOf(iObj)) >= 0)
					removed.RemoveAt(index);
				else {
					if(added == null)
						added = new List<IntermediateObject>(4);
					added.Add(iObj);
				}
			}
			int res = base.Add(iObj);
			XPBaseCollection anotherSideCollection = GetReferredCollection(newObject);
			((XPRefCollectionHelperManyToMany)anotherSideCollection.Helper).AddIntermediateObject(iObj, OwnerObject);
			anotherSideCollection.BaseAdd(OwnerObject);
			return res;
		}
		public override void Remove(object theObject) {
			if(theObject == null)
				throw new ArgumentNullException("theObject");
			if(ObjList.IndexOf(theObject) >= 0) {
				base.Remove(theObject);
				IntermediateObject obj = GetIntermediateObject(theObject);
				if(Session.IsUnitOfWork) {
					Hash.Remove(GetReferredObject(obj));
					obj.Delete();
				} else {
					int index;
					if(added != null && (index = added.IndexOf(obj)) >= 0) {
						added.RemoveAt(index);
						Hash.Remove(theObject);
					} else {
						if(removed == null)
							removed = new List<IntermediateObject>(4);
						removed.Add(obj);
					}
				}
				GetReferredCollection(theObject).BaseRemove(OwnerObject);
			}
		}
		public override XPClassInfo FetchObjectsClassInfo { get { return RefProperty.IntermediateClass; } }
		public override int InternalAddObject(object newObject) {
			XPMemberInfo m1 = Session.GetClassInfo(newObject).GetMember(RefProperty.Name);
			object reffered = m1.GetValue(newObject);
			AddIntermediateObject((IntermediateObject)newObject, reffered);
			return base.InternalAddObject(reffered);
		}
	}
	public interface IXPClassInfoAndSessionProvider : IXPClassInfoProvider, ISessionProvider { }
}
namespace DevExpress.Xpo {
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Reflection;
	using System.Drawing;
	using System.Runtime.Serialization;
	using DevExpress.Xpo.Metadata;
	using DevExpress.Xpo.Helpers;
	using DevExpress.Xpo.Exceptions;
	using DevExpress.Xpo.Metadata.Helpers;
	using DevExpress.Data.Filtering;
	using DevExpress.Data.Filtering.Helpers;
	using DevExpress.Xpo.DB;
	using System.Collections.Generic;
	using System.ComponentModel.Design.Serialization;
	using System.ComponentModel;
#if SL
	using DevExpress.Data.Browsing;
	using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
	using DevExpress.Xpf.ComponentModel;
#endif
	public class ObjectsQuery {
		XPClassInfo classInfo;
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectsQueryClassInfo")]
#endif
		public XPClassInfo ClassInfo {
			get { return classInfo; }
			set { classInfo = value; }
		}
		CriteriaOperator criteria;
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectsQueryCriteria")]
#endif
		public CriteriaOperator Criteria {
			get { return criteria; }
			set { criteria = value; }
		}
		SortingCollection sorting;
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectsQuerySorting")]
#endif
		public SortingCollection Sorting {
			get { return sorting; }
			set { sorting = value; }
		}
		int topSelectedRecords;
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectsQueryTopSelectedRecords")]
#endif
		public int TopSelectedRecords {
			get { return topSelectedRecords; }
			set { topSelectedRecords = value; }
		}
		int skipSelectedRecords;
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectsQuerySkipSelectedRecords")]
#endif
		public int SkipSelectedRecords {
			get { return skipSelectedRecords; }
			set { skipSelectedRecords = value; }
		}
		CollectionCriteriaPatcher collectionCriteriaPatcher;
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectsQueryCollectionCriteriaPatcher")]
#endif
		public CollectionCriteriaPatcher CollectionCriteriaPatcher {
			get { return collectionCriteriaPatcher; }
			set { collectionCriteriaPatcher = value; }
		}
		bool force;
#if !SL
	[DevExpressXpoLocalizedDescription("ObjectsQueryForce")]
#endif
		public bool Force {
			get { return force; }
			set { force = value; }
		}
		public ObjectsQuery(XPClassInfo classInfo, CriteriaOperator criteria, SortingCollection sorting, int topSelectedRecords, CollectionCriteriaPatcher collectionCriteriaPatcher, bool force)
			: this(classInfo, criteria, sorting, 0, topSelectedRecords, collectionCriteriaPatcher, force) { }
		public ObjectsQuery(XPClassInfo classInfo, CriteriaOperator criteria, SortingCollection sorting, int skipSelectedRecords, int topSelectedRecords, CollectionCriteriaPatcher collectionCriteriaPatcher, bool force) {
			if(classInfo == null)
				throw new ArgumentNullException("classInfo");
			this.classInfo = classInfo;
			this.criteria = criteria;
			this.sorting = sorting;
			this.topSelectedRecords = topSelectedRecords;
			this.skipSelectedRecords = skipSelectedRecords;
			this.collectionCriteriaPatcher = collectionCriteriaPatcher;
			this.force = force;
		}
	}
#if !SL
	[ListBindable(BindableSupport.No)]
	[Editor("DevExpress.Xpo.Design.SortingCollectionEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.Drawing.Design.UITypeEditor, System.Drawing")]
#endif
	public sealed class SortingCollection: IList, IEnumerable<SortProperty> {
		List<SortProperty> list = new List<SortProperty>();
		EventHandler changed;
		internal event EventHandler Changed {
			add { changed += value; }
			remove { changed -= value; }
		}
		public SortingCollection() { }
		public SortingCollection(params SortProperty[] sortProperties) {
			if(sortProperties != null)
				AddRange(sortProperties);
		}
		public SortingCollection(object owner, params SortProperty[] sortProperties) : this(sortProperties) { }
		public void AddRange(SortProperty[] sortProperties) {
			foreach(SortProperty sp in sortProperties)
				if(sp != null)
					list.Add(sp);
			FireChanded();
		}
		public void Add(SortingCollection sortProperties) {
			foreach(SortProperty sp in sortProperties)
				list.Add(sp);
			FireChanded();
		}
		public void Add(SortProperty sortProperty) {
			list.Add(sortProperty);
			FireChanded();
		}
		public SortProperty this[int index] { get { return list[index]; } }
		int IList.Add(object value) {
			int pos = ((IList)list).Add(value);
			FireChanded();
			return pos;
		}
		void FireChanded() {
			if(changed != null)
				changed(this, EventArgs.Empty);
		}
		public void Clear() {
			list.Clear();
			FireChanded();
		}
		bool IList.Contains(object value) {
			return ((IList)list).Contains(value);
		}
		int IList.IndexOf(object value) {
			return ((IList)list).IndexOf(value);
		}
		void IList.Insert(int index, object value) {
			((IList)list).Insert(index, value);
			FireChanded();
		}
		bool IList.IsFixedSize {
			get { return false; }
		}
		bool IList.IsReadOnly {
			get { return false; }
		}
		void IList.Remove(object value) {
			((IList)list).Remove(value);
			FireChanded();
		}
		public void RemoveAt(int index) {
			((IList)list).RemoveAt(index);
			FireChanded();
		}
		object IList.this[int index] {
			get {
				return ((IList)list)[index];
			}
			set {
				((IList)list)[index] = value;
				FireChanded();
			}
		}
		void ICollection.CopyTo(Array array, int index) {
			((IList)list).CopyTo(array, index);
		}
#if !SL
	[DevExpressXpoLocalizedDescription("SortingCollectionCount")]
#endif
		public int Count {
			get { return list.Count; }
		}
		bool ICollection.IsSynchronized {
			get { return false; }
		}
		object ICollection.SyncRoot {
			get { return ((IList)list).SyncRoot; }
		}
		public IEnumerator GetEnumerator() {
			return list.GetEnumerator();
		}
		IEnumerator<SortProperty> IEnumerable<SortProperty>.GetEnumerator() {
			return list.GetEnumerator();
		}
	}
	[FlagsAttribute]
	public enum CollectionBindingBehavior { AllowNone = 0, AllowNew = 1, AllowRemove = 2 };
	public enum XPCollectionChangedType { BeforeAdd, AfterAdd, BeforeRemove, AfterRemove };
	public class XPCollectionChangedEventArgs : EventArgs {
		public XPCollectionChangedType CollectionChangedType;
		public object ChangedObject;
		public int NewIndex;
		public XPCollectionChangedEventArgs(XPCollectionChangedType collectionChangedType, object changedObject, int newIndex) {
			this.CollectionChangedType = collectionChangedType;
			this.ChangedObject = changedObject;
			this.NewIndex = newIndex;
		}
		public XPCollectionChangedEventArgs(XPCollectionChangedType collectionChangedType, object changedObject) : this(collectionChangedType, changedObject, -1) { }
	}
	public delegate void XPCollectionChangedEventHandler(object sender, XPCollectionChangedEventArgs e);
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabOrmComponents)]
	[DefaultProperty("Session"), DefaultEvent("ListChanged")]
#if !SL
	[DesignerSerializer("DevExpress.Xpo.Design.XPCollectionSerializer, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
#endif
	[Description("Represents a collection of persistent objects. Can serve as a data source for data-aware controls.")]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "XPCollection")]
	public class XPCollection : XPBaseCollection {
		protected XPCollection(XPCollectionHelper helper)
			: base(helper) { }
		public XPCollection()
			: base() {
		}
		public XPCollection(IContainer container)
			: this() {
			container.Add(this);
		}
		public XPCollection(Session session, object theOwner, XPMemberInfo refProperty)
			: base(session, theOwner, refProperty) { }
		public XPCollection(Type objType)
			: this(objType, null) { }
		public XPCollection(Type objType, CriteriaOperator theCriteria, params SortProperty[] sortProperties)
			: this(XpoDefault.GetSession(), objType, theCriteria, sortProperties) { }
		public XPCollection(Session session, Type objType)
			: this(session, objType, (CriteriaOperator)null) { }
		public XPCollection(Session session, XPClassInfo objType)
			: this(session, objType, (CriteriaOperator)null) { }
		public XPCollection(Session session, Type objType, CriteriaOperator theCriteria, params SortProperty[] sortProperties)
			: this(session, session.GetClassInfo(objType), theCriteria, sortProperties) { }
		public XPCollection(Session session, XPClassInfo objType, CriteriaOperator theCriteria, params SortProperty[] sortProperties)
			: base(session, objType, theCriteria, sortProperties) { }
		public XPCollection(Session session, XPClassInfo objType, bool loadingEnabled)
			: base(session, objType, loadingEnabled) { }
		public XPCollection(Session session, Type objType, bool loadingEnabled)
			: this(session, session.Dictionary.GetClassInfo(objType), loadingEnabled) { }
		public XPCollection(Session session, XPClassInfo objType, IEnumerable originalCollection, CriteriaOperator copyFilter, bool caseSensitive)
			: base(session, objType, session, originalCollection, copyFilter, caseSensitive) { }
		public XPCollection(Session session, XPClassInfo objType, IEnumerable originalCollection, CriteriaOperator copyFilter)
			: this(session, objType, originalCollection, copyFilter, false) { }
		public XPCollection(Session session, XPClassInfo objType, IEnumerable originalCollection)
			: this(session, objType, originalCollection, (CriteriaOperator)null) { }
		public XPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter)
			: this(session, originalCollection, copyFilter, false) { }
		public XPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter, bool caseSensitive)
			: base(session, originalCollection.GetObjectClassInfo(), originalCollection.Session, originalCollection, copyFilter, caseSensitive) { }
		public XPCollection(XPBaseCollection originalCollection, CriteriaOperator filter)
			: this(originalCollection.Session, originalCollection, filter) { }
		public XPCollection(XPBaseCollection originalCollection, CriteriaOperator filter, bool caseSensitive)
			: this(originalCollection.Session, originalCollection, filter, caseSensitive) { }
		public XPCollection(XPBaseCollection originalCollection)
			: this(originalCollection, null) { }
		public XPCollection(Session session, XPBaseCollection originalCollection)
			: this(session, originalCollection, null) { }
		public XPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, XPClassInfo objType, CriteriaOperator condition, bool selectDeleted)
			: base(criteriaEvaluationBehavior, session, objType, condition, selectDeleted) { }
		public XPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, XPClassInfo objType, CriteriaOperator condition)
			: this(criteriaEvaluationBehavior, session, objType, condition, false) { }
		public XPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, Type objType, CriteriaOperator condition)
			: this(criteriaEvaluationBehavior, session, session.GetClassInfo(objType), condition) { }
		Type objectType;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DefaultValue(null)]
		public Type ObjectType {
			get { return ObjectClassInfo != null ? ObjectClassInfo.ClassType : null; }
			set {
#if !SL
				if(!this.Initializing && !this.IsDesignMode2)
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "ObjectType"));
#endif
				objectType = value;
				InvokeListChanged(ListChangedType.PropertyDescriptorChanged, -1);
			}
		}
		protected override void RenewObjectClassInfoOnSessionChange() {
			base.RenewObjectClassInfoOnSessionChange();
			if(Helper.ObjectClassInfo != null) {
				objectType = Helper.ObjectClassInfo.ClassType;
				Helper.ObjectClassInfo = null;
			}
		}
		public override XPClassInfo GetObjectClassInfo() {
			if(Helper.ObjectClassInfo == null && objectType != null && Session != null) {
				Helper.ObjectClassInfo =
#if !CF && !SL
					IsDesignMode ? DesignDictionary.GetClassInfo(objectType) :
#endif
					Session.GetClassInfo(objectType);
			}
			return Helper.ObjectClassInfo;
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPCollectionObjectClassInfo"),
#endif
 DefaultValue(null)]
#if !SL
		[TypeConverter("DevExpress.Xpo.Design.ObjectClassInfoTypeConverter, " + AssemblyInfo.SRAssemblyXpoDesignFull)]
		[MergableProperty(false)]
#endif
		[RefreshProperties(RefreshProperties.All)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XPClassInfo ObjectClassInfo {
			get { return GetObjectClassInfo(); }
			set {
#if !SL
				if(!this.Initializing && !this.IsDesignMode2)
					throw new InvalidOperationException(Res.GetString(Res.Collections_CannotAssignProperty, "ObjectClassInfo"));
#endif
				if(Helper.ObjectClassInfo != value) {
					Helper.ObjectClassInfo = value;
					objectType = null;
					if(!this.Initializing) {
						Sorting.Clear();
						_Criteria = null;
						Clear();
						DisplayableProperties = null;
					}
					InvokeListChanged(ListChangedType.PropertyDescriptorChanged, -1);
				}
			}
		}
		public object Lookup(object key) {
			object obj = this.Session.GetObjectByKey(ObjectClassInfo, key);
			if(obj != null && BaseIndexOf(obj) >= 0)
				return obj;
			return null;
		}
		public int Add(object newObject) {
			return BaseAdd(newObject);
		}
		public int IndexOf(object theObject) {
			return BaseIndexOf(theObject);
		}
		public void Remove(object theObject) {
			BaseRemove(theObject);
		}
		[System.Runtime.CompilerServices.IndexerNameAttribute("Object"), Browsable(false)]
		public object this[int index] {
			get {
				return BaseIndexer(index);
			}
		}
		public void AddRange(ICollection objects) {
			BaseAddRange(objects);
		}
	}
	[DefaultProperty("Session"), DefaultEvent("ListChanged")]
#if !SL
	[DesignerSerializer("DevExpress.Xpo.Design.XPCollectionSerializer, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
#endif
	public class XPCollection<T> : XPBaseCollection, IList<T>  {	
		public XPCollection(Session session, object theOwner, XPMemberInfo refProperty)
			: base(session, theOwner, refProperty) { }
		public XPCollection()
			: base() { }
		public XPCollection(CriteriaOperator theCriteria, params SortProperty[] sortProperties)
			: this(XpoDefault.GetSession(), theCriteria, sortProperties) { }
		public XPCollection(Session session)
			: this(session, (CriteriaOperator)null) { }
		public XPCollection(Session session, CriteriaOperator theCriteria, params SortProperty[] sortProperties)
			: base(session, session.GetClassInfo<T>(), theCriteria, sortProperties) { }
		public XPCollection(Session session, bool loadingEnabled)
			: base(session, session.GetClassInfo<T>(), loadingEnabled) { }
		public XPCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter, bool caseSensitive)
			: base(session, session.GetClassInfo<T>(), session, originalCollection, copyFilter, caseSensitive) { }
		public XPCollection(Session session, IEnumerable originalCollection, CriteriaOperator copyFilter)
			: this(session, originalCollection, copyFilter, false) { }
		public XPCollection(Session session, IEnumerable originalCollection)
			: this(session, originalCollection, null) { }
		public XPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter, bool caseSensitive)
			: base(session, session.GetClassInfo<T>(), originalCollection.Session, originalCollection, copyFilter, caseSensitive) { }
		public XPCollection(Session session, XPBaseCollection originalCollection, CriteriaOperator copyFilter)
			: this(session, originalCollection, copyFilter, false) { }
		public XPCollection(XPBaseCollection originalCollection, CriteriaOperator filter)
			: this(originalCollection.Session, originalCollection, filter) { }
		public XPCollection(XPBaseCollection originalCollection, CriteriaOperator filter, bool caseSensitive)
			: this(originalCollection.Session, originalCollection, filter, caseSensitive) { }
		public XPCollection(XPBaseCollection originalCollection)
			: this(originalCollection, null) { }
		public XPCollection(Session session, XPBaseCollection originalCollection)
			: this(session, originalCollection, null) { }
		public XPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition, bool selectDeleted)
			: base(criteriaEvaluationBehavior, session, session.GetClassInfo<T>(), condition, selectDeleted) { }
		public XPCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, CriteriaOperator condition)
			: this(criteriaEvaluationBehavior, session, condition, false) { }
		protected override void RenewObjectClassInfoOnSessionChange() {
			base.RenewObjectClassInfoOnSessionChange();
			Helper.ObjectClassInfo = null;
		}
		public override XPClassInfo GetObjectClassInfo() {
			if(Helper.ObjectClassInfo == null)
				Helper.ObjectClassInfo =
#if !CF && !SL
					IsDesignMode ? DesignDictionary.GetClassInfo(typeof(T)) :
#endif
					Session.GetClassInfo(typeof(T));
			return Helper.ObjectClassInfo;
		}
		public T Lookup(object key) {
			object obj = this.Session.GetObjectByKey(GetObjectClassInfo(), key);
			if(obj != null && BaseIndexOf(obj) >= 0)
				return (T)obj;
			return default(T); 
		}
		public void Add(T newObject) {
			BaseAdd(newObject);
		}
		public int IndexOf(T theObject) {
			return BaseIndexOf(theObject);
		}
		public bool Remove(T theObject) {
			return BaseRemove(theObject);
		}
		[System.Runtime.CompilerServices.IndexerNameAttribute("Object"), Browsable(false)]
		public T this[int index] {
			get {
				return (T)BaseIndexer(index);
			}
		}
		void IList<T>.Insert(int index, T item) {
			Add(item);
		}
		void IList<T>.RemoveAt(int index) {
			((IList)this).RemoveAt(index);
		}
		T IList<T>.this[int index] {
			get {
				return this[index];
			}
			set {
				((IList)this)[index] = value;
			}
		}
		void ICollection<T>.Clear() {
			((IList)this).Clear();
		}
		bool ICollection<T>.Contains(T item) {
			return ((IList)this).Contains(item);
		}
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
			((IList)this).CopyTo(array, arrayIndex);
		}
		int ICollection<T>.Count {
			get {
				return Count;
			}
		}
		bool ICollection<T>.IsReadOnly {
			get {
				return ((IList)this).IsReadOnly;
			}
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			foreach(T item in (IList)this)
				yield return item;
		}
		public void AddRange(IEnumerable<T> objects) {
			ICollection goodInput = objects as ICollection;
			if(goodInput != null)
				BaseAddRange(goodInput);
			else
				BaseAddRange(objects.ToArray());
		}
	}
	public enum PersistentCriteriaEvaluationBehavior { BeforeTransaction, InTransaction }
#if !SL
	[Designer("DevExpress.Xpo.Design.SessionOwnerDesigner, " + AssemblyInfo.SRAssemblyXpoDesignFull)]
#endif
	public abstract class XPBaseCollection : Component, IBindingList, ITypedList, ISupportInitialize, IXPClassInfoAndSessionProvider, IObjectChange, IFilteredXtraBindingList, IXPPrefetchableAssociationList, IXPBulkLoadableCollection {
		internal protected readonly XPCollectionHelper Helper;
		bool? _CaseSensitive;
		bool _DeleteObjectOnRemove = false;
#if !CF && !SL
		XPDictionary designDictionary;
		internal XPDictionary DesignDictionary {
			get {
				if(designDictionary == null) {
					if(!IsDesignMode)
						throw new InvalidOperationException();
					designDictionary = new DesignTimeReflection(Session.Site);
				}
				return designDictionary;
			}
		}
#endif
		CollectionBindingBehavior bindingBehavior;
		string _DisplayableProperties;
		bool hasChangesDuringInit;
		protected bool Initializing;
		void ISupportInitialize.BeginInit() {
			Initializing = true;
			hasChangesDuringInit = false;
		}
		void ISupportInitialize.EndInit() {
			Clear();
			Initializing = false;
			if(hasChangesDuringInit) {
				InvokeListChanged(ListChangedType.PropertyDescriptorChanged, -1);
				Reset();
			}
			hasChangesDuringInit = false;
		}
		IList filtered;
		IList sorted;
		IComparer sortedComparer;
		bool _loadingEnabled;
		int suspendChangesCount = 0;
		bool IsFilterFit(object theObject) {
			if(this.newAdding == theObject)
				return true;
			if(!this.CanFireChangedEvents)
				return true;
			return validator(theObject);
		}
		protected IList Objects {
			get {
				if(isDisposed) {
					return new object[0];
				}
				if(!isInAsyncLoading)
					Load();
				return EnsureFilteredAndSorted();
			}
		}
		bool IsFilteringAndSortingEnsured() {
			return ReferenceEquals(_Filter, null) == (filtered == null) && (Sorting.Count == 0) == (sorted == null);
		}
		IList EnsureFilteredAndSorted() {
			IList list = Helper.ObjList;
			if(!ReferenceEquals(_Filter, null)) {
				if(filtered == null) {
					filtered = new ObjectList();
					int count = Helper.ObjList.Count;
					for(int i = 0; i < count; i++) {
						object obj = Helper.ObjList[i];
						if(IsFilterFit(obj))
							filtered.Add(obj);
					}
				}
				list = filtered;
			}
			if(Sorting.Count > 0) {
				if(sorted == null) {
					sortedComparer = CreateComparer();
					if(list.Count > 1) {
						ArrayList helper = new ArrayList(list);
						helper.Sort(sortedComparer);
						sorted = new ObjectList(helper);
					} else {
						sorted = new ObjectList(list);
					}
				}
				list = sorted;
			}
			return list;
		}
		void ClearFilteredSortedStrategy() {
			filtered = null;
			sorted = null;
			sortedComparer = null;
			strategy = null;
		}
		protected void Clear() {
			_IsLoaded = false;
			_ClearCount++;
			Helper.ClearObjList();
			ClearFilteredSortedStrategy();
		}
		protected virtual void InvokeListChanged(ListChangedType changeType, int newIndex) {
			InvokeListChanged(changeType, newIndex, -1);
		}
		static readonly object EventListChanged = new Object();
		[Description("Occurs when the collection changes or an object in the collection changes")]
		public event ListChangedEventHandler ListChanged {
			add {
				Events.AddHandler(EventListChanged, value);
			}
			remove {
				Events.RemoveHandler(EventListChanged, value);
			}
		}
		void InvokeListChanged(ListChangedType changeType, int newIndex, int oldIndex) {
			if(changeType == ListChangedType.Reset) {
				filtered = null;
				sorted = null;
				sortedComparer = null;
				strategy = null;
			}
			if(Initializing)
				hasChangesDuringInit = true;
			else {
				if(rootDesignCollection != this) {
					OnListChanged(changeType, newIndex, oldIndex);
				}
			}
		}
		protected virtual void GoMuteIfNeeded() {
			if(!SessionStateStack.IsInAnyOf(Session, SessionState.ReceivingObjectsFromNestedUow | SessionState.GetObjectsNonReenterant))
				return;
			Session.MuteCollection(this);
		}
		void OnListChanged(ListChangedType changeType, int newIndex, int oldIndex) {
			if(Events[EventListChanged] == null)
				return;
			GoMuteIfNeeded();
			if(!CanFireChangedEvents) {
				switch(changeType) {
					default:
						return;
					case ListChangedType.PropertyDescriptorAdded:
					case ListChangedType.PropertyDescriptorChanged:
					case ListChangedType.PropertyDescriptorDeleted:
						break;
				}
			}
			if(Events[EventListChanged] != null)
				((ListChangedEventHandler)Events[EventListChanged])(this, new ListChangedEventArgs(changeType, newIndex, oldIndex));
		}
		class UpdateStrategy {
			public XPBaseCollection collection;
			public UpdateStrategy(XPBaseCollection collection) {
				this.collection = collection;
			}
			public virtual void Add(object obj, int index) {
				collection.InvokeListChanged(ListChangedType.ItemAdded, FixIndex(index, obj));
			}
			public virtual void Remove(object obj, int index) {
				collection.InvokeListChanged(ListChangedType.ItemDeleted, FixIndex(index, obj));
			}
			public virtual void Change(object obj, int index) {
				collection.InvokeListChanged(ListChangedType.ItemChanged, FixIndex(index, obj));
			}
			int FixIndex(int index, object obj) {
				if(index == -1)
					index = collection.Helper.ObjList.IndexOf(obj);
				return index;
			}
		}
		class FilterUpdateStrategy : UpdateStrategy {
			UpdateStrategy parent;
			public FilterUpdateStrategy(UpdateStrategy parent)
				: base(parent.collection) {
				this.parent = parent;
			}
			public override void Add(object obj, int index) {
				if(IsFit(obj))
					parent.Add(obj, collection.filtered.Add(obj));
			}
			bool IsFit(object obj) {
				return collection.IsFilterFit(obj);
			}
			public override void Remove(object obj, int index) {
				index = collection.filtered.IndexOf(obj);
				if(index >= 0) {
					collection.filtered.RemoveAt(index);
					parent.Remove(obj, index);
				}
			}
			public override void Change(object obj, int index) {
				index = collection.filtered.IndexOf(obj);
				if(index >= 0) {
					if(IsFit(obj))
						parent.Change(obj, index);
					else {
						collection.filtered.RemoveAt(index);
						parent.Remove(obj, index);
					}
				} else {
					if(IsFit(obj))
						parent.Add(obj, collection.filtered.Add(obj));
				}
			}
		}
		class SortingUpdateStrategy : UpdateStrategy {
			UpdateStrategy parent;
			public SortingUpdateStrategy(UpdateStrategy parent)
				: base(parent.collection) {
				this.parent = parent;
			}
			public override void Add(object obj, int index) {
				int newIndex;
				if(collection.newAdding == obj || !collection.CanFireChangedEvents)
					newIndex = collection.sorted.Add(obj);
				else {
					newIndex = XPCollectionCompareHelper.GetPos(collection.sortedComparer, obj, collection.sorted, -1, collection.sorted.Count);
					collection.sorted.Insert(newIndex, obj);
				}
				parent.Add(obj, newIndex);
			}
			public override void Remove(object obj, int index) {
				int oldIndex = collection.sorted.IndexOf(obj);
				collection.sorted.RemoveAt(oldIndex);
				parent.Remove(obj, oldIndex);
			}
			public override void Change(object obj, int index) {
				int oldIndex = collection.sorted.IndexOf(obj);
				if(collection.newAdding == null && collection.CanFireChangedEvents) {
					IComparer cmp = collection.sortedComparer;
					int newIndex = XPCollectionCompareHelper.GetPos(cmp, obj, collection.sorted, oldIndex, oldIndex);
					if(newIndex != oldIndex) {
						collection.sorted.RemoveAt(oldIndex);
						collection.sorted.Insert(newIndex, obj);
						collection.InvokeListChanged(ListChangedType.ItemMoved, newIndex, oldIndex);
					} else
						parent.Change(obj, oldIndex);
				} else
					parent.Change(obj, oldIndex);
			}
		}
		UpdateStrategy strategy;
		UpdateStrategy Strategy {
			get {
				if(strategy == null) {
					if(IsLoaded)
						EnsureFilteredAndSorted();
					strategy = new UpdateStrategy(this);
					if(sorted != null)
						strategy = new SortingUpdateStrategy(strategy);
					if(filtered != null)
						strategy = new FilterUpdateStrategy(strategy);
				}
				return strategy;
			}
		}
		protected virtual void OnCollectionChanged(XPCollectionChangedEventArgs args) {
			GoMuteIfNeeded();
			if(CanFireChangedEvents && !Initializing && Events[EventCollectionChanged] != null)
				((XPCollectionChangedEventHandler)Events[EventCollectionChanged])(this, args);
		}
		public void SuspendChangedEvents() {
			suspendChangesCount++;
		}
		public void ResumeChangedEvents() {
			suspendChangesCount--;
			if(CanFireChangedEvents)
				Reset();
		}
		protected bool CanFireChangedEvents { get { return suspendChangesCount == 0; } }
		bool isDisposed = false;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Clear();
			}
			base.Dispose(disposing);
			isDisposed = true;
		}
		bool CanAddNewObject() {
			return (GetObjectClassInfo() != null) ? !GetObjectClassInfo().IsAbstract : false;
		}
		void InitData() {
			_SelectDeleted = false;
			bindingBehavior = CollectionBindingBehavior.AllowNew | CollectionBindingBehavior.AllowRemove;
			_DisplayableProperties = string.Empty;
			Initializing = false;
			_SkipReturnedObjects = 0;
			_TopReturnedObjects = 0;
			hasChangesDuringInit = false;
		}
		internal void SortingChanged() {
			Reset();
		}
		void Reset() {
			InvokeListChanged(ListChangedType.Reset, -1);
		}
		bool? _isDesignMode;
		protected bool IsDesignMode {
			get {
				return Session.IsDesignMode;
			}
		}
		protected bool IsDesignMode2 {
			get {
				return DevExpress.Data.Helpers.IsDesignModeHelper.GetIsDesignModeBypassable(this, ref _isDesignMode);
			}
		}
		object newAdding;
		IDictionary objectsEditingStarted;
		static bool _EnableObjectChangedNotificationsWhileEditing;
#if !SL
	[DevExpressXpoLocalizedDescription("XPBaseCollectionEnableObjectChangedNotificationsWhileEditing")]
#endif
		[Obsolete("That property breaks XPCollection's compatibility with DataSet. Use it at your own risk.")]
		public static bool EnableObjectChangedNotificationsWhileEditing {
			get { return _EnableObjectChangedNotificationsWhileEditing; }
			set { _EnableObjectChangedNotificationsWhileEditing = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void SuppressEvents(object sender) {
			if(_EnableObjectChangedNotificationsWhileEditing)
				return;
			if(objectsEditingStarted == null)
				objectsEditingStarted = new Dictionary<object, object>();
			objectsEditingStarted[sender] = sender;
		}
		void UnsuppressEvents(object sender) {
			if(objectsEditingStarted == null)
				return;
			objectsEditingStarted.Remove(sender);
			if(objectsEditingStarted.Count == 0)
				objectsEditingStarted = null;
		}
		bool IsEventsSuppressed(object sender) {
			if(objectsEditingStarted == null)
				return false;
			return objectsEditingStarted.Contains(sender);
		}
		void IObjectChange.OnObjectChanged(object sender, ObjectChangeEventArgs args) {
			switch(args.Reason) {
				case ObjectChangeReason.BeforePropertyDescriptorChangeWithinBeginEdit:
					SuppressEvents(sender);
					break;
				case ObjectChangeReason.Delete:
					if(!SelectDeleted) {
						BaseRemove(sender);
						UnsuppressEvents(sender);
					} else {
						if(!IsEventsSuppressed(sender))
							Strategy.Change(sender, -1);
					}
					break;
				case ObjectChangeReason.PropertyChanged:
				case ObjectChangeReason.Reset: {
						if(!IsEventsSuppressed(sender))
							Strategy.Change(sender, -1);
						break;
					}
				case ObjectChangeReason.EndEdit:
					if(sender == newAdding) {
						newAdding = null;
						UnsuppressEvents(sender);
						Strategy.Change(sender, -1);
					} else if(IsEventsSuppressed(sender)) {
						UnsuppressEvents(sender);
						Strategy.Change(sender, -1);
					}
					break;
				case ObjectChangeReason.CancelEdit:
					UnsuppressEvents(sender);
					if(sender == newAdding) {
						newAdding = null;
						if(rootDesignCollection == this || !IsDesignMode) {
							BaseRemove(sender);
							if(Session.IsUnitOfWork && Session.IsObjectToSave(sender) && !IsDesignMode) {
								Session.Delete(sender);
								Session.RemoveFromSaveList(sender);
							}
							Helper.KickOutCancelledAddNew(sender);
							rootDesignCollection = null;
						}
					}
					break;
			}
		}
		protected XPBaseCollection(XPCollectionHelper helper)
			: base() {
			Helper = helper;
			InitData();
		}
		protected XPBaseCollection(Session session, object theOwner, XPMemberInfo refProperty)
			: base() {
			if(!refProperty.IsCollection)
				throw new InvalidOperationException(Res.GetString(Res.Collections_NotCollectionProperty, refProperty.Owner.FullName, refProperty.Name));
			if(refProperty.IsManyToMany)
				this.Helper = new XPRefCollectionHelperManyToMany(this, theOwner, refProperty);
			else {
				this.Helper = new XPRefCollectionHelperOneToMany(this, theOwner, refProperty);
			}
			this.Helper.Session = session;
			this.Helper.ObjectClassInfo = refProperty.CollectionElementType;
			this._loadingEnabled = true;
			InitData();
		}
		protected XPBaseCollection() : this(true) { }
		protected XPBaseCollection(bool loadingEnabled)
			: base() {
			this.Helper = new XPCollectionHelper(this);
			this._loadingEnabled = loadingEnabled;
			InitData();
		}
		protected XPBaseCollection(Session session, XPClassInfo objType, CriteriaOperator theCriteria, params SortProperty[] sortProperties)
			: this() {
			this.Helper.Session = session;
			if(sortProperties != null && sortProperties.Length > 0)
				this.Sorting.AddRange(sortProperties);
			this.Helper.ObjectClassInfo = objType;
			this._Criteria = theCriteria;
		}
		protected XPBaseCollection(Session session, XPClassInfo objType, bool loadingEnabled)
			: this(loadingEnabled) {
			this.Helper.Session = session;
			this.Helper.ObjectClassInfo = objType;
		}
		protected XPBaseCollection(Session session, XPClassInfo objType, Session originalSession, IEnumerable originalCollection, CriteriaOperator copyFilter, bool caseSensitive)
			: this(session, objType, false) {
			IEnumerable objects;
			NestedUnitOfWork mySessionAsNested = session as NestedUnitOfWork;
			if(mySessionAsNested == null || ReferenceEquals(mySessionAsNested, originalSession)) {
				objects = originalCollection;
			} else {
				objects = originalCollection.Cast<object>().Select(o => mySessionAsNested.GetNestedObject(o));
			}
			this._CaseSensitive = caseSensitive;
			ICollection list = FilterList(session, objType, objects, copyFilter, caseSensitive);
			this.BaseAddRange(list);
		}
		void PersistentCriteriaEvaluationBehaviorFillInTransaction(CriteriaOperator condition) {
			this.BaseAddRange(Session.GetObjectsInTransaction(GetObjectClassInfo(), condition, SelectDeleted));
		}
		void PersistentCriteriaEvaluationBehaviorFillBeforeTransaction(CriteriaOperator condition) {
			ICollection hintCollection = Session.GetObjectsInternal(new ObjectsQuery[] { new ObjectsQuery(GetObjectClassInfo(), condition, null, 0, 0, new CollectionCriteriaPatcher(SelectDeleted, Session.TypesManager), false) })[0];
			this.BaseAddRange(hintCollection);
		}
		protected XPBaseCollection(PersistentCriteriaEvaluationBehavior criteriaEvaluationBehavior, Session session, XPClassInfo objType, CriteriaOperator condition, bool selectDeleted)
			: this(session, objType, false) {
			this.SelectDeleted = selectDeleted;
			switch(criteriaEvaluationBehavior) {
				case PersistentCriteriaEvaluationBehavior.InTransaction:
					PersistentCriteriaEvaluationBehaviorFillInTransaction(condition);
					break;
				case PersistentCriteriaEvaluationBehavior.BeforeTransaction:
					PersistentCriteriaEvaluationBehaviorFillBeforeTransaction(condition);
					break;
				default:
					throw new NotImplementedException(Res.GetString(Res.Collections_CriteriaEvaluationBehaviorIsNotSupported, criteriaEvaluationBehavior.ToString()));
			}
		}
		long _ClearCount;
		[Browsable(false)]
		public long ClearCount {
			get {
				return _ClearCount;
			}
		}
		bool _IsLoaded;
		[Browsable(false)]
		public bool IsLoaded {
			get {
				return _IsLoaded;
			}
		}
		public ObjectsQuery BeginLoad() {
			return BeginLoad(false);
		}
		public ObjectsQuery BeginLoad(bool force) {
			if((!IsLoaded || force) && !Initializing && !IsDesignMode && GetObjectClassInfo() != null) {
				IEnumerable objs = GetHintContentCore();
				if(objs == null)
					objs = Helper.GetContentWithoutQueryIfPossible();
				if(objs != null) {
					EndLoad(objs);
				} else if(LoadingEnabled) {
					SortingCollection sorting = new SortingCollection();
					if((TopReturnedObjects != 0 || SkipReturnedObjects != 0) && Sorting != null) {
						foreach(SortProperty prop in Sorting) {
							sorting.Add(new SortProperty(Helper.PatchCriteriaFromUserToFetch(prop.Property), prop.Direction));
						}
					}
					CriteriaOperator fetchCriteria = GetRealFetchCriteria();
					return new ObjectsQuery(GetRealFetchClassInfo(),
						fetchCriteria,
						sorting,
						SkipReturnedObjects,
						TopReturnedObjects,
						new Generators.CollectionCriteriaPatcher(SelectDeleted, Session.TypesManager), false);
				}
				_IsLoaded = true;
			}
			return null;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public XPClassInfo GetRealFetchClassInfo() {
			return Helper.FetchObjectsClassInfo;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CriteriaOperator GetRealFetchCriteria() {
			return Helper.GetHardcodedCriterion() & Helper.PatchCriteriaFromUserToFetch(Criteria);
		}
		public int EvaluateDatastoreCount() {
			return ((int?)Session.Evaluate(GetRealFetchClassInfo(), new AggregateOperand(null, null, Aggregate.Count, null), GetRealFetchCriteria())) ?? 0;
		}
		IEnumerable GetHintContentCore() {
			IEnumerable hint = HintCollection;
			HintCollection = null;
			if(hint == null)
				return null;
			if(TopReturnedObjects != 0)
				return null;
			if(SkipReturnedObjects != 0)
				return null;
			CriteriaOperator op = Criteria;
			if(!SelectDeleted && GetRealFetchClassInfo().IsGCRecordObject) {
				var gcMember = GetRealFetchClassInfo().GetMember(GCRecordField.StaticName);
				hint = hint.Cast<object>().Where(theObject => gcMember.GetValue(theObject) == null);
			}
			ICollection col = FilterList(Session, GetRealFetchClassInfo(), hint, op, CaseSensitive);
			return col;
		}
		static ICollection FilterList(Session session, XPClassInfo objType, IEnumerable originalCollection, CriteriaOperator copyFilter, bool caseSensitive) {
			var copyValidator = CreatePredicate(session, objType, copyFilter, caseSensitive);
			IList list = new List<object>();
			foreach(object obj in originalCollection) {
				if(copyValidator(obj)) {
					list.Add(obj);
				}
			}
			return list;
		}
		internal static Func<object, bool> CreatePredicate(Session session, XPClassInfo objType, CriteriaOperator criteria, bool caseSensitive) {
			if(ReferenceEquals(criteria, null))
				return x => true;
			return CriteriaCompiler.ToUntypedPredicate(criteria, objType.GetCriteriaCompilerDescriptor(session), new CriteriaCompilerAuxSettings(caseSensitive, session.Dictionary.CustomFunctionOperators));
		}
		public void EndLoad(IEnumerable objects) {
			IEnumerable result = Helper.PatchLoadedCollectionWithChangesWhileNotLoaded(objects);
			Clear();
			foreach(object obj in result)
				Helper.InternalAddObject(obj);
			_IsLoaded = true;
		}
		public virtual void Load() {
			ObjectsQuery query = BeginLoad();
			if(query != null)
				EndLoad(Session.GetObjects(query));
		}
		bool isInAsyncLoading;
		public virtual bool LoadAsync() {
			return LoadAsync(null);
		}
		public virtual bool LoadAsync(AsyncLoadObjectsCallback callback) {
			if (isInAsyncLoading) return false;
			ObjectsQuery query = BeginLoad(true);
			if (query != null) {
				isInAsyncLoading = true;
				Session.GetObjectsAsync(new ObjectsQuery[] { query }, new AsyncLoadObjectsCallback(delegate(ICollection[] result, Exception e) {
					isInAsyncLoading = false;
					if (e == null) EndLoad(result[0]);
					else EndLoad(new object[0]);
					InvokeListChanged(ListChangedType.Reset, -1);
					if (callback != null) {
						callback(new ICollection[] { this }, e);
					}
				}));
				return true;
			}
			return false;
		}
		XPPropertyDescriptorCollection GetProps(IList objects) {
			XPClassInfo targetClass = GetTargetClass();
			foreach(object obj in objects) {
				XPClassInfo classInfo = Session.GetClassInfo(obj);
				if(!targetClass.IsAssignableTo(classInfo))
					targetClass = classInfo;
			}
			return new XPPropertyDescriptorCollection(Session, targetClass, new PropertyDescriptor[] { });
		}
		XPClassInfo GetTargetClass() {
			return GetObjectClassInfo();
		}
		IComparer CreateComparer() {
			if(IsDesignMode)
				return XPCollectionCompareHelper.CreateDesignTimeComparer();
			return XPCollectionCompareHelper.CreateComparer(Sorting, GetObjectClassInfo().GetCriteriaCompilerDescriptor(Session), new CriteriaCompilerAuxSettings(CaseSensitive, Session.Dictionary.CustomFunctionOperators));
		}
		public virtual void Reload() {
			Clear();
			Helper.Reload();
			if(!Initializing)
				Reset();
		}
		public virtual int BaseIndexOf(object value) {
			return Objects.IndexOf(value);
		}
		object nowAdding = null;
		public virtual int BaseAdd(object newObject) {
			if(isDisposed) throw new ObjectDisposedException(this.ToString());
			if(nowAdding == newObject)
				return -1;
			if(nowAdding != null)
				throw new InvalidOperationException(Res.GetString(Res.Collections_RecurringObjectAdd));
			nowAdding = newObject;
			try {
				this.Session.ThrowIfObjectFromDifferentSession(newObject);
				if(!IsDesignMode && !Session.GetClassInfo(newObject).IsAssignableTo(GetObjectClassInfo()))
					throw new InvalidCastException(Res.GetString(Res.Collections_InvalidCastOnAdd, newObject.GetType().FullName, GetObjectClassInfo() == null ? String.Empty : GetObjectClassInfo().FullName));
				if(Helper.LoadCollectionOnModify) {
					Load();
				}
				if(IsLoaded)
					EnsureFilteredAndSorted();
				if(!Helper.ObjList.Contains(newObject)) {
					OnCollectionChanged(new XPCollectionChangedEventArgs(XPCollectionChangedType.BeforeAdd, newObject));
					int pos = Helper.Add(newObject); 
					if(IsLoaded && !IsFilteringAndSortingEnsured())
						throw new InvalidOperationException("Collection was reset during object adding");
					Strategy.Add(newObject, pos);
					int index = IsLoaded ? this.BaseIndexOf(newObject) : Helper.ObjList.IndexOf(newObject);
					OnCollectionChanged(new XPCollectionChangedEventArgs(XPCollectionChangedType.AfterAdd, newObject, index));
					return index;
				} else {
					int index = IsLoaded ? this.BaseIndexOf(newObject) : Helper.ObjList.IndexOf(newObject);
					return index;
				}
			} finally {
				nowAdding = null;
			}
		}
		object nowRemoving;
		public virtual bool BaseRemove(object theObject) {
			if(isDisposed) throw new ObjectDisposedException(this.ToString());
			if(theObject == null)
				return false;
			if(nowRemoving == theObject)
				return false;
			nowRemoving = theObject;
			try {
				if(Helper.LoadCollectionOnModify) {
					Load();
				}
				if(IsLoaded)
					EnsureFilteredAndSorted();
				if(!Helper.IsThereForDelete(theObject))
					return false;
				int itemIndex = IsLoaded ? BaseIndexOf(theObject) : Helper.ObjList.IndexOf(theObject);
				OnCollectionChanged(new XPCollectionChangedEventArgs(XPCollectionChangedType.BeforeRemove, theObject, itemIndex));
				Helper.Remove(theObject);
				if(IsLoaded && !IsFilteringAndSortingEnsured())
					throw new InvalidOperationException("Collection was reset during object removing");
				Strategy.Remove(theObject, itemIndex);
				Helper.BeforeAfterRemove(theObject);
				OnCollectionChanged(new XPCollectionChangedEventArgs(XPCollectionChangedType.AfterRemove, theObject));
				if(DeleteObjectOnRemove && !IsDesignMode)
					Session.Delete(theObject);
				return true;
			} finally {
				nowRemoving = null;
			}
		}
		public virtual object BaseIndexer(int index) {
			if(isDisposed) return null;
			return Objects[index];
		}
		public void BaseAddRange(ICollection objects) {
			if(objects.Count <= 1) {
				AddRangeCore(objects);
			} else {
				SuspendChangedEvents();
				try {
					AddRangeCore(objects);
				} finally {
					ResumeChangedEvents();
				}
			}
		}
		private void AddRangeCore(ICollection c) {
			foreach(object obj in c)
				BaseAdd(obj);
		}
		SortingCollection _Sorting;
#if !SL
	[DevExpressXpoLocalizedDescription("XPBaseCollectionSorting")]
#endif
#if !SL
		[MergableProperty(false)]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SortingCollection Sorting {
			get {
				if(_Sorting == null)
					SetSorting(null);
				return _Sorting;
			}
			set {
				SetSorting(value);
				SortingChanged();
			}
		}
		void sortingCollection_ListChanged(object sender, EventArgs e) {
			if(IsLoaded)
				SortingChanged();
		}
		void SetSorting(SortingCollection value) {
			if(value == null)
				value = new SortingCollection();
			if(ReferenceEquals(_Sorting, value))
				return;
			if(!ReferenceEquals(_Sorting, null)) {
				_Sorting.Changed -= new EventHandler(sortingCollection_ListChanged);
			}
			_Sorting = value;
			_Sorting.Changed += new EventHandler(sortingCollection_ListChanged);
		}
		bool _SelectDeleted;
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPBaseCollectionSelectDeleted"),
#endif
 DefaultValue(false)]
		public bool SelectDeleted {
			get { return _SelectDeleted; }
			set {
				if(_SelectDeleted != value) {
					_SelectDeleted = value;
					Reload();
				}
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPBaseCollectionBindingBehavior"),
#endif
 DefaultValue(CollectionBindingBehavior.AllowNew | CollectionBindingBehavior.AllowRemove)]
#if !SL
		[Editor("DevExpress.Xpo.Design.FlagsEnumEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.Drawing.Design.UITypeEditor, System.Drawing")]
#endif
		public CollectionBindingBehavior BindingBehavior {
			get { return bindingBehavior; }
			set { bindingBehavior = value; }
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPBaseCollectionCaseSensitive"),
#endif
 DefaultValue(false)]
		public bool CaseSensitive {
			get { return _CaseSensitive.HasValue ? _CaseSensitive.Value : Session.CaseSensitive; }
			set {
				_CaseSensitive = value;
				if(!this.Initializing)
					SortingChanged();
				Reset();
			}
		}
		internal CriteriaOperator _Criteria;
#if !SL
	[DevExpressXpoLocalizedDescription("XPBaseCollectionCriteria")]
#endif
#if !SL
		[Editor("DevExpress.Xpo.Design.XPCollectionCriteriaEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.Drawing.Design.UITypeEditor, System.Drawing")]
		[TypeConverter("DevExpress.Xpo.Design.CriteriaEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull)]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator Criteria {
			get { return _Criteria; }
			set {
				if(!ReferenceEquals(Criteria, value)) {
					_Criteria = value;
					Reload();
				}
			}
		}
		[Browsable(false)]
		[DefaultValue(null)]
		public string CriteriaString {
			get {
				CriteriaOperator criteria = Criteria;
				return (object)criteria == null ? null : criteria.ToString();
			}
			set {
				if(CriteriaString != value)
					Criteria = CriteriaOperator.Parse(value);
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPBaseCollectionDeleteObjectOnRemove"),
#endif
 DefaultValue(false)]
		public bool DeleteObjectOnRemove {
			get { return _DeleteObjectOnRemove; }
			set { _DeleteObjectOnRemove = value; }
		}
		Func<object, bool> validator;
		CriteriaOperator _Filter;
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CriteriaOperator Filter {
			get { return _Filter; }
			set {
				if(!ReferenceEquals(_Filter, value)) {
					_Filter = value;
					if(ReferenceEquals(_Filter, null)) {
						validator = null;
					} else {
						validator = CreatePredicate(this.Session, this.GetObjectClassInfo(), _Filter, CaseSensitive);
					}
					if(!this.Initializing)
						ClearFilteredSortedStrategy();
					Reset();
				}
			}
		}
#if !SL
#if !SL
	[DevExpressXpoLocalizedDescription("XPBaseCollectionSession")]
#endif
		[TypeConverter("DevExpress.Xpo.Design.SessionReferenceConverter, " + AssemblyInfo.SRAssemblyXpoDesignFull)]
#endif
		[RefreshProperties(RefreshProperties.All)]
		public Session Session {
			get {
				if(Helper.Session == null) {
					Helper.Session = DoResolveSession();
				}
				return Helper.Session;
			}
			set {
				if(Helper.Session != value) {
					Helper.Session = value;
					RenewObjectClassInfoOnSessionChange();
					Clear();
					InvokeListChanged(ListChangedType.PropertyDescriptorChanged, -1);
					Reset();
				}
			}
		} 
#if !CF && !SL
		bool ShouldSerializeSession() {
			return Helper.Session != null && !(Helper.Session is DefaultSession);
		}
		bool ShouldSerializeCaseSensitive() {
			return _CaseSensitive.HasValue;
		}
		void ResetCaseSensitive() {
			_CaseSensitive = null;
		}
#endif
		Session DoResolveSession() {
#if !CF && !SL
			if(IsDesignMode2) {
				return new DevExpress.Xpo.Helpers.DefaultSession(this.Site);
			}
#endif
			ResolveSessionEventArgs args = new ResolveSessionEventArgs();
			OnResolveSession(args);
			if(args.Session != null) {
				Session resolved = args.Session.Session;
				if(resolved != null) {
					return resolved;
				}
			}
			return XpoDefault.GetSession();
		}
		protected virtual void OnResolveSession(ResolveSessionEventArgs args) {
			if(Events[EventResolveSession] != null)
				((ResolveSessionEventHandler)Events[EventResolveSession])(this, args);
		}
		static readonly object EventResolveSession = new object();
		public event ResolveSessionEventHandler ResolveSession {
			add {
				Events.AddHandler(EventResolveSession, value);
			}
			remove {
				Events.RemoveHandler(EventResolveSession, value);
			}
		}
		public abstract XPClassInfo GetObjectClassInfo();
		protected virtual void RenewObjectClassInfoOnSessionChange() {
			itemProperties = null;
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XPBaseCollectionDisplayableProperties")]
#endif
#if !SL
		[Editor("DevExpress.Xpo.Design.DisplayablePropertiesEditor, " + AssemblyInfo.SRAssemblyXpoDesignFull, "System.Drawing.Design.UITypeEditor, System.Drawing")]
#endif
		[Localizable(false)]
		public string DisplayableProperties {
			get {
				if(_DisplayableProperties == null || _DisplayableProperties.Length == 0)
					_DisplayableProperties = StringListHelper.DelimitedText(ClassMetadataHelper.GetDefaultDisplayableProperties(GetObjectClassInfo()), ";");
				return _DisplayableProperties;
			}
			set {
				if(_DisplayableProperties != value) {
					_DisplayableProperties = value;
					itemProperties = null;
					InvokeListChanged(ListChangedType.PropertyDescriptorChanged, -1);
				}
			}
		}
		public static 
#if SL
			DevExpress.Xpo.DB.StringCollection 
#else
			StringCollection
#endif
			GetDefaultDisplayableProperties(XPClassInfo objectInfo) {
			return ClassMetadataHelper.GetDefaultDisplayableProperties(objectInfo);
		}
#if !CF
		bool ShouldSerializeDisplayableProperties() {
			if(_DisplayableProperties == null)
				return false;
#if SL
			DevExpress.Xpo.DB.StringCollection list;
#else
			StringCollection list;
#endif
			list = ClassMetadataHelper.GetDefaultDisplayableProperties(GetObjectClassInfo());
			string[] props = new string[list.Count];
			list.CopyTo(props, 0);
			return String.Join(";", props) != DisplayableProperties;
		}
#endif
		int _TopReturnedObjects;
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPBaseCollectionTopReturnedObjects"),
#endif
 DefaultValue(0)]
		public Int32 TopReturnedObjects {
			get { return _TopReturnedObjects; }
			set { _TopReturnedObjects = value; }
		}
		int _SkipReturnedObjects;
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPBaseCollectionSkipReturnedObjects"),
#endif
 DefaultValue(0)]
		public Int32 SkipReturnedObjects {
			get { return _SkipReturnedObjects; }
			set { _SkipReturnedObjects = value; }
		}
		#region ITypedList impl
		class CollectionItemProperties : DevExpress.Xpo.Helpers.ClassMetadataHelper.ItemProperties {
			public CollectionItemProperties(XPBaseCollection context) : base(context) { }
			public override string GetDisplayableProperties() {
				return ((XPBaseCollection)Context).DisplayableProperties;
			}
		}
		DevExpress.Xpo.Helpers.ClassMetadataHelper.ItemProperties itemProperties;
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(itemProperties == null)
				itemProperties = new CollectionItemProperties(this);
			return ClassMetadataHelper.GetItemProperties(itemProperties, listAccessors);
		}
		string ITypedList.GetListName(PropertyDescriptor[] listAccessors) {
			return ClassMetadataHelper.GetListName(listAccessors);
		}
		#endregion
		#region ICollection Impl
		[Browsable(false)]
		public int Count {
			get {
				return Objects.Count;
			}
		}
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot { get { return this; } }
		void ICollection.CopyTo(Array array, int index) {
			Objects.CopyTo(array, index);
		}
		#endregion
		#region IEnumerable Impl
		IEnumerator IEnumerable.GetEnumerator() {
			return Objects.GetEnumerator();
		}
		#endregion
		#region IList Impl
		bool IList.IsFixedSize { get { return false; } }
		bool IList.IsReadOnly { get { return false; } }
		object IList.this[int index] {
			get { return rootDesignCollection == this ? BaseIndexer(0) : BaseIndexer(index); }
			set { }
		}
		int IList.Add(object value) { return BaseAdd(value); }
		void IList.Clear() {
			this.Clear();
			Reset();
		}
		bool IList.Contains(object value) {
			return Objects.Contains(value);
		}
		int IList.IndexOf(object value) { return BaseIndexOf(value); }
		void IList.Insert(int index, object value) {
			BaseAdd(value);
		}
		void IList.Remove(object value) { BaseRemove(value); }
		void IList.RemoveAt(int index) {
			BaseRemove(BaseIndexer(index));
		}
		#endregion
		#region IBinding Impl
		bool IBindingList.AllowEdit { get { return true; } }
		bool IBindingList.AllowNew {
			get {
				return (this.BindingBehavior & CollectionBindingBehavior.AllowNew) != 0 && CanAddNewObject();
			}
		}
		bool IBindingList.AllowRemove { get { return (this.BindingBehavior & CollectionBindingBehavior.AllowRemove) != 0; } }
		bool IBindingList.IsSorted { get { return Sorting.Count > 0; } }
		ListSortDirection IBindingList.SortDirection {
			get { return Sorting.Count == 0 || Sorting[0].Direction != SortingDirection.Descending ? ListSortDirection.Ascending : ListSortDirection.Descending; }
		}
		PropertyDescriptor IBindingList.SortProperty {
			get { return (Sorting.Count > 0 && Objects.Count > 0) ? GetProps(Objects).Find(Sorting[0].PropertyName, false) : null; }
		}
		bool IBindingList.SupportsChangeNotification { get { return true; } }
		bool IBindingList.SupportsSearching { get { return true; } }
		bool IBindingList.SupportsSorting { get { return true; } }
		void IBindingList.AddIndex(PropertyDescriptor property) { }
		static XPBaseCollection rootDesignCollection;
		object IBindingList.AddNew() {
			if(!IsDesignMode && (this.BindingBehavior & CollectionBindingBehavior.AllowNew) == 0)
				throw new NotSupportedException();
			object obj = null;
			if(GetObjectClassInfo() != null) {
				if(IsDesignMode) {
					if(rootDesignCollection == null)
						rootDesignCollection = this;
					obj = new IntermediateObject(this.Session, GetObjectClassInfo());
				} else {
					obj = CreateAddNewInstance();
				}
				newAdding = obj;
				return BaseAdd(obj) >= 0 ? obj : null;
			} else
				return null;
		}
		protected virtual object CreateAddNewInstance() {
			return GetObjectClassInfo().CreateNewObject(this.Session);
		}
		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction) {
			XPPropertyDescriptor prop = property as XPPropertyDescriptor;
			string name = prop != null ? XPPropertyDescriptor.GetMemberName(prop.Name) : property.Name;
			Sorting = new SortingCollection(new SortProperty(new OperandProperty(name),
				direction == ListSortDirection.Ascending ? SortingDirection.Ascending : SortingDirection.Descending));
		}
		int IBindingList.Find(PropertyDescriptor property, object key) {
			foreach(object o in this) {
				if(property.GetValue(o).Equals(key))
					return BaseIndexOf(o);
			}
			return -1;
		}
		void IBindingList.RemoveIndex(PropertyDescriptor property) { }
		void IBindingList.RemoveSort() { Sorting = new SortingCollection(); }
		#endregion
		static readonly object EventCollectionChanged = new object();
		[Description("Occurs when the XPCollection content changes")]
		public event XPCollectionChangedEventHandler CollectionChanged {
			add {
				Events.AddHandler(EventCollectionChanged, value);
			}
			remove {
				Events.RemoveHandler(EventCollectionChanged, value);
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPBaseCollectionLoadingEnabled"),
#endif
 DefaultValue(true)]
		public bool LoadingEnabled {
			get {
				return this._loadingEnabled;
			}
			set {
				if(LoadingEnabled == value)
					return;
				this._loadingEnabled = value;
				if(LoadingEnabled && this.IsLoaded) {
					Reload();
				}
			}
		}
		IEnumerable _HintCollection;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IEnumerable HintCollection {
			get { return _HintCollection; }
			set { _HintCollection = value; }
		}
		bool IXPPrefetchableAssociationList.NeedPrefetch() {
			return !IsLoaded && HintCollection == null && LoadingEnabled;
		}
		void IXPPrefetchableAssociationList.FinishPrefetch(ICollection hint) {
			this.HintCollection = hint;
		}
		public void PreFetch(params string[] collections) {
			Session.PreFetch(GetObjectClassInfo(), this, collections);
		}
		#region ISessionProvider Members
		Session ISessionProvider.Session {
			get { return Session; }
		}
		#endregion
		volatile static int seq = 0;
		int seqNum = ++seq;
		public override string ToString() {
			string state;
			if(isDisposed)
				state = "Disposed";
			else if(this.IsLoaded)
				state = "Count(" + this.Count.ToString() + ')';
			else
				state = "NotLoaded";
			return base.ToString() + '(' + seqNum.ToString() + ") " + state;
		}
		IObjectLayer IObjectLayerProvider.ObjectLayer {
			get {
				if(Session == null)
					return null;
				return Session.ObjectLayer;
			}
		}
		IDataLayer IDataLayerProvider.DataLayer {
			get {
				if(Session == null)
					return null;
				return Session.DataLayer;
			}
		}
		XPDictionary IXPDictionaryProvider.Dictionary {
			get {
				if(Session == null)
					return null;
				return
#if !CF && !SL
					IsDesignMode ? DesignDictionary :
#endif
					Session.Dictionary;
			}
		}
		XPClassInfo IXPClassInfoProvider.ClassInfo {
			get { return GetObjectClassInfo(); }
		}
	}
}
