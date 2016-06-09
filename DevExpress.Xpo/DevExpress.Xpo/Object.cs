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
using System.Reflection;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.Xpo.Exceptions;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Data.Filtering.Helpers;
using System.Xml.Serialization;
using System.ComponentModel;
#if SL
using DevExpress.Data.Browsing;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using System.Collections.Generic;
#endif
namespace DevExpress.Xpo.Helpers {
	public class FieldsClassBase : OperandProperty {
		public FieldsClassBase() : base(string.Empty) { }
		public FieldsClassBase(string propertyName) : base(propertyName) { }
		protected string GetNestedName(string nestedPropertyName) {
			if(PropertyName == null || PropertyName.Length == 0)
				return nestedPropertyName;
			else
				return PropertyName + '.' + nestedPropertyName;
		}
	}
	public sealed class XPPropertyDescriptor : PropertyDescriptor
#if !SL
		, IObjectChange
#endif
	{
		public const string ReferenceAsObjectTail = "!";
		public const string ReferenceAsKeyTail = "!Key";
		static bool _NonAggregatedPathsEditable;
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This property provides backward compatibility with XPO v1.5. It must not be switched to true in a situation other than that. If you set it to True to make subproperties of a referenced object editable, you must implement proper saving or canceling changes made to the referenced object, when it is being edited in UI as a part of its parent object.")]
		public static bool NonAggregatedPathsEditable {
			get { return _NonAggregatedPathsEditable; }
			set { _NonAggregatedPathsEditable = value; }
		}
		XPMemberInfo targetMember;
		XPClassInfo objectType;
		MemberInfoCollection path;
		Type propertyType;
		ValueAccessor accessor;
		abstract class ValueAccessor {
			public abstract string ChangedPropertyName { get;}
			public abstract object GetValue(object obj);
			public abstract bool SetValue(object obj, object value);
		}
		class MemberAccessor : ValueAccessor {
			protected readonly XPMemberInfo Member;
			bool upCast;
			public MemberAccessor(XPClassInfo objectType, XPMemberInfo member) {
				this.Member = member;
				upCast = !objectType.IsAssignableTo(member.Owner);
			}
			public override object GetValue(object obj) {
				if(obj == null || (upCast && !Member.Owner.Dictionary.GetClassInfo(obj).IsAssignableTo(Member.Owner)))
					return null;
				return Member.GetValue(obj);
			}
			public override bool SetValue(object obj, object value) {
				if(obj == null || (upCast && !Member.Owner.Dictionary.GetClassInfo(obj).IsAssignableTo(Member.Owner)))
					return false;
				if(PersistentBase.CanSkipAssignement(GetValue(obj), value))
					return false;
				IXPReceiveOnChangedFromXPPropertyDescriptor changeableOwner = obj as IXPReceiveOnChangedFromXPPropertyDescriptor;
				if(changeableOwner != null) {
					changeableOwner.BeforeChangeByXPPropertyDescriptor();
					try {
						Member.SetValue(obj, value);
						changeableOwner.FireChangedByXPPropertyDescriptor(ChangedPropertyName);
					} finally {
						changeableOwner.AfterChangeByXPPropertyDescriptor();
					}
				} else {
					Member.SetValue(obj, value);
				}
				return true;
			}
			public override string ChangedPropertyName {
				get { return Member.Name; }
			}
		}
		class RelationAsCollectionAccessor : MemberAccessor {
			Session session;
			public RelationAsCollectionAccessor(XPClassInfo objectType, XPMemberInfo member, Session session)
				: base(objectType, member) {
				this.session = session;
			}
			public override object GetValue(object obj) {
				XPCollection col = new XPCollection(session, Member.ReferenceType, false);
				GC.SuppressFinalize(col);
				col.BindingBehavior = CollectionBindingBehavior.AllowNew | CollectionBindingBehavior.AllowRemove;
				object val = base.GetValue(obj);
				if(val != null)
					col.BaseAdd(val);
				return col;
			}
			public override bool SetValue(object obj, object value) {
				throw new InvalidOperationException(Res.GetString(Res.Object_ReferencePropertyViaCollectionAccessor));
			}
		}
		class RelationAsKeyAccessor : MemberAccessor {
			Session session;
			public RelationAsKeyAccessor(XPClassInfo objectType, XPMemberInfo member, Session session)
				: base(objectType, member) {
				this.session = session;
			}
			public override object GetValue(object obj) {
				object target = base.GetValue(obj);
				if(target == null)
					return null;
				return session.GetKeyValue(target);
			}
			public override bool SetValue(object obj, object value) {
				if(value != null)
					value = session.GetObjectByKey(Member.ReferenceType, value);
				return base.SetValue(obj, value);
			}
		}
		class RelatedMemberAccessor : MemberAccessor {
			ValueAccessor next;
			string changedPropertyName;
			public RelatedMemberAccessor(XPClassInfo objectType, XPMemberInfo member, ValueAccessor next)
				: base(objectType, member) {
				this.next = next;
				this.changedPropertyName = null;
			}
			public override object GetValue(object obj) {
				return next.GetValue(base.GetValue(obj));
			}
			public override bool SetValue(object obj, object value) {
				bool result = next.SetValue(base.GetValue(obj), value);
				if(result) {
					IXPReceiveOnChangedFromXPPropertyDescriptor changeable = obj as IXPReceiveOnChangedFromXPPropertyDescriptor;
					if(changeable != null)
						changeable.FireChangedByXPPropertyDescriptor(ChangedPropertyName);
				}
				return result;
			}
			public override string ChangedPropertyName {
				get {
					if(changedPropertyName == null)
						changedPropertyName = Member.Name + '.' + next.ChangedPropertyName;
					return changedPropertyName;
				}
			}
		}
#if !CF && !SL
		class DesignAccessor : ValueAccessor {
			protected readonly Type MemberType;
			protected readonly Session Session;
			protected readonly XPClassInfo CollectionType;
			public DesignAccessor(Type memberType, Session session, XPClassInfo relationType) {
				this.MemberType = memberType;
				this.Session = session;
				this.CollectionType = relationType;
			}
			object GetDesingCollection(XPClassInfo classInfo) {
				return ((DesignTimeReflection)classInfo.Dictionary).GetDesingCollection(classInfo, Session);
			}
			public override object GetValue(object obj) {
				if(typeof(XPBaseCollection).IsAssignableFrom(MemberType))
					return GetDesingCollection(CollectionType);
				else if(MemberType == typeof(bool))
					return true;
				else if(MemberType == typeof(string))
					return "Test String";
				else if(MemberType == typeof(int))
					return 0;
				else if(MemberType == typeof(decimal))
					return Decimal.Zero;
				else if(MemberType.IsEnum)
					return 0;
				else
					return null;
			}
			public override bool SetValue(object obj, object value) { return false; }
			public override string ChangedPropertyName {
				get { return string.Empty; }
			}
		}
#endif
		string displayName;
		string memberName;
		public override string DisplayName {
			get {
				return displayName;
			}
		}
		public static string GetMemberName(string propertyName) {
			if(propertyName.EndsWith(XPPropertyDescriptor.ReferenceAsObjectTail))
				return propertyName.Substring(0, propertyName.Length - XPPropertyDescriptor.ReferenceAsObjectTail.Length);
			if(propertyName.EndsWith(XPPropertyDescriptor.ReferenceAsKeyTail))
				return propertyName.Substring(0, propertyName.Length - XPPropertyDescriptor.ReferenceAsKeyTail.Length);
			return propertyName;
		}
		public XPPropertyDescriptor(Session session, XPClassInfo objectType, string propertyName)
			: base(propertyName, new Attribute[] { }) {
			this.objectType = objectType;
			MemberInfoCollection memberPath = new MemberInfoCollection(objectType, GetMemberName(propertyName), true);
			this.targetMember = memberPath[memberPath.Count - 1];
			memberName = GetMemberName(propertyName);
			displayName = targetMember.DisplayName;
			if(displayName == String.Empty)
				displayName = memberName;
			memberPath.RemoveAt(memberPath.Count - 1);
			this.path = memberPath;
			XPClassInfo collectionType;
			XPClassInfo targetType = memberPath.Count == 0 ? objectType : memberPath[memberPath.Count - 1].Owner;
			if(propertyName.EndsWith(XPPropertyDescriptor.ReferenceAsKeyTail)) {
				collectionType = null;
				if(targetMember.ReferenceType == null)
					throw new InvalidPropertyPathException(propertyName);
				propertyType = targetMember.ReferenceType.KeyProperty.MemberType;
				accessor = new RelationAsKeyAccessor(targetType, targetMember, session);
			} else if(targetMember.ReferenceType == null || propertyName.EndsWith(XPPropertyDescriptor.ReferenceAsObjectTail)) {
				collectionType = null;
				propertyType = targetMember.MemberType;
				accessor = new MemberAccessor(targetType, targetMember);
			} else {
				collectionType = targetMember.ReferenceType;
				propertyType = typeof(XPCollection);
				accessor = new RelationAsCollectionAccessor(targetType, targetMember, session);
			}
#if !CF && !SL
			if(session.IsDesignMode) {
				accessor = new DesignAccessor(PropertyType, session, (targetMember.IsAssociationList || targetMember.IsNonAssociationList) ? targetMember.CollectionElementType : collectionType);
			} else
#endif
			{
				for(int i = memberPath.Count - 1; i >= 0; i--) {
					accessor = new RelatedMemberAccessor(i == 0 ? objectType : memberPath[i - 1].Owner, memberPath[i], accessor);
				}
			}
		}
		public override bool IsReadOnly {
			get {
				if(targetMember.IsReadOnly || propertyType == typeof(XPCollection))
					return true;
#if !CF
#if SL
				ReadOnlyAttribute attr = this.Attributes[typeof(ReadOnlyAttribute)] as ReadOnlyAttribute;
				if (attr != null && attr.IsReadOnly)
					return true;
#else
				if(((ReadOnlyAttribute)this.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly)
					return true;
#endif
#else
				for(int i = 0; i < AttributeArray.Length; i++) {
					ReadOnlyAttribute att = AttributeArray[i] as ReadOnlyAttribute;
					if(att != null)
						return att.IsReadOnly;
				}
#endif
				for(int i = _NonAggregatedPathsEditable ? 1 : 0; i < path.Count; i++) {
					if(!path[i].IsAggregated)
						return true;
				}
				return false;
			}
		}
#if SL
		protected override AttributeCollection CreateAttributeCollection() {
			List<Attribute> result = new List<Attribute>(base.CreateAttributeCollection());
			foreach(Attribute att in targetMember.Attributes)
				result.Add(att);
			return new AttributeCollection(result);
		}
#else
		protected override void FillAttributes(IList attributeList) {
			foreach(object att in targetMember.Attributes)
				attributeList.Add(att);
		}
#endif
		public override object GetValue(object component) {
			return accessor.GetValue(component);
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
			accessor.SetValue(component, Convert.IsDBNull(value) ? null : value);
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
		public override Type PropertyType {
			get {
				if(propertyType == null)
					return typeof(object);
				Type nullType = Nullable.GetUnderlyingType(propertyType);
				if(nullType == null)
					return propertyType;
				else
					return nullType;
			}
		}
		public override Type ComponentType { get { return objectType.ClassType; } }
		public XPMemberInfo MemberInfo { get { return targetMember; } }
#if !SL
		public override void AddValueChanged(object component, EventHandler handler) {
			XPBaseObject.AddChangedEventHandler(component, this);
			base.AddValueChanged(component, handler);
		}
		public override void RemoveValueChanged(object component, EventHandler handler) {
			base.RemoveValueChanged(component, handler);
			XPBaseObject.RemoveChangedEventHandler(component, this);
		}
		void IObjectChange.OnObjectChanged(object sender, ObjectChangeEventArgs arg) {
			if(arg.Reason == ObjectChangeReason.Reset || (arg.Reason == ObjectChangeReason.PropertyChanged && arg.PropertyName == memberName))
				OnValueChanged(sender, EventArgs.Empty);
		}
#endif
	}
	public sealed class XPPropertyDescriptorCollection : PropertyDescriptorCollection {
		XPClassInfo objectType;
		Session session;
		public XPPropertyDescriptorCollection(Session session, XPClassInfo objectType, ICollection descriptors)
			: base(null) {
			this.objectType = objectType;
			this.session = session;
			foreach(PropertyDescriptor pd in descriptors)
				Find(pd.Name, false);
		}
		public XPPropertyDescriptorCollection(Session session, XPClassInfo objectType, PropertyDescriptor[] descriptors)
			: base(null) {
			this.objectType = objectType;
			this.session = session;
			foreach(PropertyDescriptor pd in descriptors)
				Find(pd.Name, false);
		}
		public XPPropertyDescriptorCollection(Session session, XPClassInfo objectType)
			: base(null) {
			this.objectType = objectType;
			this.session = session;
			foreach(XPMemberInfo mi in objectType.Members)
				if(mi.IsPublic)
					Find(mi.Name, false);
		}
		public override PropertyDescriptor Find(string name, bool ignoreCase) {
			if(name == null || name.Length == 0)
				return null;
			PropertyDescriptor pd = base.Find(name, ignoreCase);
			if(pd == null) {
				MemberInfoCollection col = new MemberInfoCollection(objectType, XPPropertyDescriptor.GetMemberName(name), true, false);
				if(col.Count == 0)
					return null;
				if(name.EndsWith(XPPropertyDescriptor.ReferenceAsObjectTail) && col[col.Count - 1].ReferenceType == null)
					return null;
				if(name.EndsWith(XPPropertyDescriptor.ReferenceAsKeyTail) && (col[col.Count - 1].ReferenceType == null || !col[col.Count - 1].ReferenceType.IsPersistent))
					return null;
				pd = new XPPropertyDescriptor(session, objectType, name);
				Add(pd);
			}
			return pd;
		}
		public override PropertyDescriptor this[string itemIndex] {
			get { return FindCaseSmart(itemIndex); }
		}
#if !SL
		public override PropertyDescriptor this[int itemIndex] {
			get { return base[itemIndex]; }
		}
#endif
		public PropertyDescriptor FindCaseSmart(string name) {
			PropertyDescriptor rv = Find(name, false);
			if(rv != null)
				return rv;
			return Find(name, true);
		}
	}
	public interface IXPOServiceClass { }
	public interface IXPClassInfoProvider : IXPDictionaryProvider {
		XPClassInfo ClassInfo { get;}
	}
	public static class XPDelayedPropertyHelper {
		public static XPDelayedProperty GetDelayedPropertyContainer(object theObject, XPMemberInfo mi) {
			return XPDelayedProperty.GetDelayedPropertyContainer(theObject, mi);
		}
		public static object GetInternalValue(XPDelayedProperty container) {
			return container.InternalValue;
		}
	}
}
namespace DevExpress.Xpo {
	using System.Collections;
	using System.ComponentModel;
	using System.Reflection;
	using DevExpress.Xpo.Exceptions;
	using DevExpress.Xpo.Metadata;
	using DevExpress.Xpo.Metadata.Helpers;
	using DevExpress.Xpo.Helpers;
	using System.Collections.Specialized;
	using System.Collections.Generic;
#if SL
	using DevExpress.Xpf.Collections;
#endif
	public enum ObjectChangeReason { PropertyChanged, Reset, Delete, EndEdit, CancelEdit, BeginEdit, BeforePropertyDescriptorChangeWithinBeginEdit }
	[Serializable]
	public class ObjectChangeEventArgs : EventArgs {
		ObjectChangeReason reason;
		string propertyName;
		object oldValue;
		object newValue;
		object _Object;
		Session _Session;
		[Obsolete("Use ObjectChangeEventArgs(Session session, object theObject, ObjectChangeReason reason) instead")]
		public ObjectChangeEventArgs(ObjectChangeReason reason) {
			this.reason = reason;
		}
		[Obsolete("Use ObjectChangeEventArgs(Session session, object theObject, ObjectChangeReason reason, string propertyName, object oldValue, object newValue) instead")]
		public ObjectChangeEventArgs(ObjectChangeReason reason, string propertyName)
			: this(reason) {
			this.propertyName = propertyName;
		}
		[Obsolete("Use ObjectChangeEventArgs(Session session, object theObject, string propertyName, object oldValue, object newValue) instead")]
		public ObjectChangeEventArgs(string propertyName, object oldValue, object newValue)
			: this(ObjectChangeReason.PropertyChanged) {
			this.propertyName = propertyName;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public ObjectChangeEventArgs(Session session, object theObject, ObjectChangeReason reason, string propertyName, object oldValue, object newValue){
			this._Session = session;
			this._Object = theObject;
			this.reason = reason;
			this.propertyName = propertyName;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		public ObjectChangeEventArgs(Session session, object theObject, string propertyName, object oldValue, object newValue)
			: this(session, theObject, ObjectChangeReason.PropertyChanged, propertyName, oldValue, newValue) { }
		public ObjectChangeEventArgs(Session session, object theObject, ObjectChangeReason reason) : this(session, theObject, reason, null, null, null) { }
		public ObjectChangeReason Reason { get { return this.reason; } }
		public string PropertyName { get { return this.propertyName; } }
		public object OldValue {
			get { return this.oldValue; }
			set {
				this.oldValue = value;
			}
		}
		public object NewValue {
			get { return this.newValue; }
			set {
				this.newValue = value;
			}
		}
		public object Object {
			get { return this._Object; }
			set { this._Object = value; }
		}
		public Session Session {
			get { return this._Session; }
			set { this._Session = value; }
		}
	}
	public delegate void ObjectChangeEventHandler(object sender, ObjectChangeEventArgs e);
	public interface IXPSimpleObject : IXPClassInfoAndSessionProvider { }
	public interface IXPReceiveOnChangedFromDelayedProperty {
		void FireChangedByDelayedPropertySetter(XPMemberInfo member, object oldValue, object newValue);
	}
	public interface IXPReceiveOnChangedFromXPPropertyDescriptor {
		void FireChangedByXPPropertyDescriptor(string memberName);
		void BeforeChangeByXPPropertyDescriptor();
		void AfterChangeByXPPropertyDescriptor();
	}
	public interface IXPReceiveOnChangedFromArbitrarySource {
		void FireChanged(string propertyName);
	}
	public interface IXPObjectWithChangedEvent {
		event ObjectChangeEventHandler Changed;
	}
	[Obsolete("Use IXPReceiveOnChangedFromDelayedProperty and/or IXPReceiveOnChangedFromXPPropertyDescriptor and/or IXPCustomPropertyStore and/or IXPObjectWithChangedEvent interfaces instead", true)]
	public interface IXPChangeableObject : IXPReceiveOnChangedFromDelayedProperty, IXPReceiveOnChangedFromXPPropertyDescriptor, IXPObjectWithChangedEvent {
	}
	public interface IXPCustomPropertyStore {
		object GetCustomPropertyValue(XPMemberInfo property);
		bool SetCustomPropertyValue(XPMemberInfo property, object theValue);
	}
	public interface IXPModificationsStore {
		bool HasModifications();
		void ClearModifications();
		void SetPropertyModified(XPMemberInfo property, object oldValue);
		bool GetPropertyModified(XPMemberInfo property);
		object GetPropertyOldValue(XPMemberInfo property);
		void ResetPropertyModified(XPMemberInfo property);
	}
	[NonPersistent, MemberDesignTimeVisibility(false), OptimisticLocking(true)]
	public class PersistentBase: IXPObject, IXPCustomPropertyStore, IXPModificationsStore, IXPInvalidateableObject, IXPReceiveOnChangedFromDelayedProperty, IXPReceiveOnChangedFromArbitrarySource, INotifyPropertyChanged, IXPImmutableHashCode {
		Session _Session;
		XPClassInfo _ClassInfo;
		protected PersistentBase(Session session){
			Init(session, session.GetClassInfo(this.GetType()));
		}
		public PersistentBase(Session session, XPClassInfo classInfo) {
			Init(session, classInfo);
		}
		void Init(Session session, XPClassInfo classInfo) {
			this._Session = session;
			this._ClassInfo = classInfo;
			if(!session.IsObjectsLoading)
				AfterConstruction();
		}
		public virtual void AfterConstruction() {
			if(Session.IsUnitOfWork &&
#if !SL
				!Session.IsDesignMode &&
#endif
				ClassInfo.IsPersistent) {
				Session.Save(this);
			}
		}
		[MemberDesignTimeVisibility(false)]
		[Browsable(false)]
		public XPClassInfo ClassInfo { get { return _ClassInfo; } }
		[MemberDesignTimeVisibility(false)]
		[Browsable(false)]
		public Session Session {
			get {
				if(IsInvalidated)
					throw new ObjectDisposedException(this.ToString());
				return _Session;
			}
		}
		XPDictionary IXPDictionaryProvider.Dictionary {
			get { return ClassInfo.Dictionary; }
		}
		IObjectLayer IObjectLayerProvider.ObjectLayer {
			get { return Session.ObjectLayer; }
		}
		protected virtual void Invalidate(bool disposing) {	
			this._Session = null;
		}
		void IXPInvalidateableObject.Invalidate() {
			Invalidate(true);
		}
		protected virtual bool IsInvalidated { get { return _Session == null; } }
		bool IXPInvalidateableObject.IsInvalidated {
			get { return IsInvalidated; }
		}
		#region IXPModificationsStore Members
		Dictionary<XPMemberInfo, object> modificationsStore;
		Dictionary<XPMemberInfo, object> ModificationsStore {
			get {
				if (modificationsStore == null)
					modificationsStore = new Dictionary<XPMemberInfo, object>();
				return modificationsStore;
			}
		}
		internal bool HasModificationStore { get { return modificationsStore != null; } }
		void IXPModificationsStore.ClearModifications() {
			if (HasModificationStore) ModificationsStore.Clear();
		}
		void IXPModificationsStore.SetPropertyModified(XPMemberInfo property, object oldValue) {
			if (ModificationsStore.ContainsKey(property)) return;
			ModificationsStore.Add(property, oldValue);
		}
		bool IXPModificationsStore.GetPropertyModified(XPMemberInfo property) {
			return HasModificationStore && ModificationsStore.ContainsKey(property);
		}
		object IXPModificationsStore.GetPropertyOldValue(XPMemberInfo property) {
			object result;
			if (HasModificationStore && ModificationsStore.TryGetValue(property, out result)) return result;
			return null;
		}
		void IXPModificationsStore.ResetPropertyModified(XPMemberInfo property) {
			if (HasModificationStore)
				ModificationsStore.Remove(property);
		}
		bool IXPModificationsStore.HasModifications() {
			if(!HasModificationStore) return false;
			return ModificationsStore.Count > 0;
		}
		public static IXPModificationsStore GetModificationsStore(object theObject) {
			IXPModificationsStore store = theObject as IXPModificationsStore;
#if SL
			if (store == null)
				throw new ArgumentException("theObject");
#else
			if (store == null)
				store = (IXPModificationsStore)ObjectRecord.GetObjectRecord(theObject);
#endif
			return store;
		}
		#endregion
		#region IXPCustomPropertyStore impl
		Dictionary<XPMemberInfo, object> customPropertyStore;
		bool HasCustomPropertyStore {
			get {
				return customPropertyStore != null;
			}
		}
		internal Dictionary<XPMemberInfo, object> CustomPropertyStore {
			get {
				if(customPropertyStore == null)
					customPropertyStore = new Dictionary<XPMemberInfo, object>();
				return customPropertyStore;
			}
		}
		object IXPCustomPropertyStore.GetCustomPropertyValue(XPMemberInfo property) {
			return GetCustomPropertyValue(property);
		}
		internal object GetCustomPropertyValue(XPMemberInfo property) {
			object theValue = null;
			if(HasCustomPropertyStore) {
				CustomPropertyStore.TryGetValue(property, out theValue);
			}
			if(theValue == null) {
				theValue = CreateDefValue(property);
				if(theValue != null)
					CustomPropertyStore[property] = theValue;
			}
			return theValue;
		}
		object CreateDefValue(XPMemberInfo property) {
			if(property.IsCollection) {
				return CreateCollection(property);
			} else if(property.IsAssociationList) {
				return CreateAssociationList(property);
			} else if(property.IsManyToManyAlias) {
				ManyToManyAliasAttribute mm = (ManyToManyAliasAttribute)property.GetAttributeInfo(typeof(ManyToManyAliasAttribute));
				XPMemberInfo aliasedMI = ClassInfo.GetMember(mm.OneToManyCollectionName);
				IList aliasedCollection = (IList)aliasedMI.GetValue(this);
				XPMemberInfo skippedReference = aliasedMI.CollectionElementType.GetMember(mm.ReferenceInTheIntermediateTableName);
				return CreateManyToManyAliasList(aliasedCollection, skippedReference);
			} else if(property.IsDelayed) {
				return new XPDelayedProperty(Session, this, property);
			} else {
				return null;
			}
		}
		bool IXPCustomPropertyStore.SetCustomPropertyValue(XPMemberInfo property, object theValue) {
			return SetCustomPropertyValue(property, theValue);
		}
		internal bool SetCustomPropertyValue(XPMemberInfo property, object theValue) {
			if(property.IsDelayed && !property.IsAssociationList)
				throw new InvalidOperationException(string.Format(Res.GetString(Res.Object_DelayedPropertyContainerCannotBeAssigned), property.Owner.FullName, property.Name));
			if(HasCustomPropertyStore || theValue != null) {
				object oldValue = ((IXPCustomPropertyStore)this).GetCustomPropertyValue(property);
				if(CanSkipAssignement(theValue, oldValue))
					return false;
				if(ReferenceEquals(theValue, null)) {
					CustomPropertyStore.Remove(property);
				} else {
					CustomPropertyStore[property] = theValue;
				}
				OnChanged(property.Name, oldValue, theValue);
				return true;
			}
			return false;
		}
		#endregion
		public static IXPCustomPropertyStore GetCustomPropertyStore(object theObject) {
			IXPCustomPropertyStore store = theObject as IXPCustomPropertyStore;
#if SL
			if (store == null)
				throw new ArgumentException("theObject");
#else
			if(store == null)
				store = (IXPCustomPropertyStore)ObjectRecord.GetObjectRecord(theObject);
#endif
			return store;
		}
		internal static bool CanSkipAssignement(object oldValue, object newValue) {
			if(ReferenceEquals(oldValue, newValue))
				return true;
			else if(oldValue is ValueType && newValue is ValueType && Equals(oldValue, newValue))
				return true;
			else if(oldValue is string && newValue is string && Equals(oldValue, newValue))
				return true;
			else
				return false;
		}
		protected IList GetList(string propertyName) {
			XPMemberInfo mi = ClassInfo.GetMember(propertyName);
			return (IList)GetCustomPropertyValue(mi);
		}
		protected virtual IList CreateAssociationList(XPMemberInfo property) {
			return new XPAssociationList(Session, this, property);
		}
		protected virtual IList CreateManyToManyAliasList(IList aliasedCollection, XPMemberInfo skippedProperty) {
			return new XPManyToManyAliasList(Session, aliasedCollection, skippedProperty);
		}
		protected IList<T> GetList<T>(string propertyName) {
			XPMemberInfo property = ClassInfo.GetMember(propertyName);
			object value;
			CustomPropertyStore.TryGetValue(property, out value);
			IList<T> rv = (IList<T>)value;
			if(rv == null) {
				if(property.IsAssociationList) {
					rv = CreateAssociationList<T>(property);
				} else {
					ManyToManyAliasAttribute mm = (ManyToManyAliasAttribute)property.GetAttributeInfo(typeof(ManyToManyAliasAttribute));
					XPMemberInfo aliasedMI = ClassInfo.GetMember(mm.OneToManyCollectionName);
					IList aliasedCollection = (IList)aliasedMI.GetValue(this);
					XPMemberInfo skippedReference = aliasedMI.CollectionElementType.GetMember(mm.ReferenceInTheIntermediateTableName);
					rv = CreateManyToManyAliasList<T>(aliasedCollection, skippedReference);
				}
				CustomPropertyStore[property] = rv;
			}
			return rv;
		}
		protected virtual IList<T> CreateAssociationList<T>(XPMemberInfo property) {
			return new XPAssociationList<T>(Session, this, property);
		}
		protected virtual IList<T> CreateManyToManyAliasList<T>(IList aliasedCollection, XPMemberInfo skippedProperty) {
			return new XPManyToManyAliasList<T>(Session, aliasedCollection, skippedProperty);
		}
		protected virtual XPCollection CreateCollection(XPMemberInfo property) {
			XPCollection col = new XPCollection(this.Session, this, property);
			GC.SuppressFinalize(col);
			return col;
		}
		internal int GetImmutableHashCode() {
			return base.GetHashCode();
		}
		int IXPImmutableHashCode.GetImmutableHashCode() {
			return GetImmutableHashCode();
		}
		public static BinaryOperator operator ==(CriteriaOperator prop, PersistentBase obj) {
			return prop == new OperandValue(obj);
		}
		public static BinaryOperator operator !=(CriteriaOperator prop, PersistentBase obj) {
			return prop != new OperandValue(obj);
		}
		public override bool Equals(object obj) {
			return base.Equals(obj);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected virtual void OnChanged(string propertyName, object oldValue, object newValue) {
			if(IsLoading)
				return;
			TriggerObjectChanged(new ObjectChangeEventArgs(Session, this, propertyName, oldValue, newValue));
		}
		protected virtual void TriggerObjectChanged(ObjectChangeEventArgs args) {
			switch(args.Reason) {
				case ObjectChangeReason.Reset:
					RaisePropertyChangedEvent(null);
					break;
				case ObjectChangeReason.PropertyChanged:
					RaisePropertyChangedEvent(args.PropertyName);
					break;
			}
			Session.TriggerObjectChanged(this, args);
		}
		protected bool SetPropertyValue(string propertyName, object newValue) {
			XPMemberInfo mi = ClassInfo.GetMember(propertyName);
			if(mi.IsDelayed) {
				return ((XPDelayedProperty)GetCustomPropertyValue(mi)).SetValue(newValue);
			} else {
				return SetCustomPropertyValue(mi, newValue);
			}
		}
		protected bool SetPropertyValue<T>(string propertyName, T newValue) {
			return SetPropertyValue(propertyName, (object)newValue);
		}
		protected bool SetPropertyValue(string propertyName, ref DateTime propertyValueHolder, DateTime newValue) {
			return SetPropertyEquatableValue(propertyName, ref propertyValueHolder, newValue);
		}
		protected bool SetPropertyValue(string propertyName, ref decimal propertyValueHolder, decimal newValue) {
			return SetPropertyEquatableValue(propertyName, ref propertyValueHolder, newValue);
		}
		protected bool SetPropertyValue(string propertyName, ref int propertyValueHolder, int newValue) {
			return SetPropertyEquatableValue(propertyName, ref propertyValueHolder, newValue);
		}
		bool SetPropertyEquatableValue<T>(string propertyName, ref T propertyValueHolder, T newValue) where T : IEquatable<T> {
			if(Session.IsObjectsLoading)
				propertyValueHolder = newValue;
			else {
				T oldValue = propertyValueHolder;
				if(oldValue.Equals(newValue))
					return false;
				propertyValueHolder = newValue;
				OnChanged(propertyName, oldValue, newValue);
			}
			return true;
		}
		protected bool SetPropertyValue<T>(string propertyName, ref T propertyValueHolder, T newValue) {
			if(Session.IsObjectsLoading)
				propertyValueHolder = newValue;
			else {
				T oldValue = propertyValueHolder;
				if(CanSkipAssignement(newValue, oldValue))
					return false;
				propertyValueHolder = newValue;
				OnChanged(propertyName, oldValue, newValue);
			}
			return true;
		}
		protected bool SetDelayedPropertyValue(string propertyName, object newValue) {
			return SetPropertyValue(propertyName, newValue);
		}
		protected bool SetDelayedPropertyValue<T>(string propertyName, T newValue) {
			return SetPropertyValue(propertyName, newValue);
		}
		protected object GetPropertyValue(string propertyName) {
			XPMemberInfo mi = ClassInfo.GetMember(propertyName);
			object result = GetCustomPropertyValue(mi);
			if(mi.IsDelayed)
				result = ((XPDelayedProperty)result).Value;
			return result;
		}
		protected T GetPropertyValue<T>(string propertyName) {
			object result = GetPropertyValue(propertyName);
			if(result == null)
				return default(T);
			else
				return (T)result;
		}
		protected object GetDelayedPropertyValue(string propertyName) {
			return GetPropertyValue(propertyName);
		}
		protected T GetDelayedPropertyValue<T>(string propertyName) {
			return GetPropertyValue<T>(propertyName);
		}
		public override string ToString() {
#if !SL
			if(!IsInvalidated && Session.IsDesignMode)
				return base.ToString();
#endif
			object key = ClassInfo.GetId(this);
			if(key == null)
				key = string.Empty;
			return ClassInfo.FullName + '(' + key.ToString() + ')';
		}
		[MemberDesignTimeVisibility(false)]
		[Browsable(false)]
		public bool IsLoading {
			get {
				return Session.IsObjectsLoading;
			}
		}
		[MemberDesignTimeVisibility(false)]
		[Browsable(false)]
		protected bool IsSaving {
			get { return Session.IsObjectsSaving; }
		}
		[Browsable(false), MemberDesignTimeVisibility(false)]
		public bool IsDeleted {
			get {
				return Session.IsObjectMarkedDeleted(this);
			}
		}
		void IXPObject.OnLoading() {
			OnLoading();
		}
		void IXPObject.OnLoaded() {
			OnLoaded();
		}
		void IXPObject.OnSaving() {
			OnSaving();
		}
		void IXPObject.OnSaved() {
			OnSaved();
		}
		void IXPObject.OnDeleting() {
			OnDeleting();
		}
		void IXPObject.OnDeleted() {
			OnDeleted();
			TriggerObjectChanged(new ObjectChangeEventArgs(Session, this, ObjectChangeReason.Delete));
		}
		protected virtual void OnLoading() { }
		protected virtual void OnLoaded() { }
		protected virtual void OnSaving() { }
		protected virtual void OnSaved() { }
		protected virtual void OnDeleting() { }
		protected virtual void OnDeleted() { }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal virtual void OnSessionProcessingProcessProcessed() { }
		void IXPReceiveOnChangedFromDelayedProperty.FireChangedByDelayedPropertySetter(XPMemberInfo member, object oldValue, object newValue) {
			if(IsLoading)
				return;
			OnChanged(member.Name, oldValue, newValue);
		}
		void IXPReceiveOnChangedFromArbitrarySource.FireChanged(string propertyName) {
			if(IsLoading)
				return;
			OnChanged(propertyName, null, null);
		}
		PropertyChangedEventHandler propertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
			add { propertyChanged += value; }
			remove { propertyChanged -= value; }
		}
		protected void RaisePropertyChangedEvent(string propertyName) {
			if(propertyChanged != null)
				propertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#region Fields
#if !SL
	[DevExpressXpoLocalizedDescription("PersistentBaseFields")]
#endif
public static FieldsClass Fields { get { return new FieldsClass(); } }
		public class FieldsClass : FieldsClassBase {
			public FieldsClass() : base() { }
			public FieldsClass(string propertyName) : base(propertyName) { }
			public OperandProperty GCRecord { get { return new OperandProperty(GetNestedName(GCRecordField.StaticName)); } }
			public XPObjectType.FieldsClass ObjectType { get { return new XPObjectType.FieldsClass(GetNestedName(XPObjectType.ObjectTypePropertyName)); } }
			public OperandProperty OptimisticLockField { get { return new OperandProperty(GetNestedName(OptimisticLockingAttribute.DefaultFieldName)); } }
		}
		#endregion
		IDataLayer IDataLayerProvider.DataLayer {
			get { return Session.DataLayer; }
		}
	}
	[NonPersistent, MemberDesignTimeVisibility(false), OptimisticLocking(true)]
	public abstract class XPBaseObject : PersistentBase, IEditableObject,
#if !SL
		ICustomTypeDescriptor,
#endif
		IComparable, IXPReceiveOnChangedFromXPPropertyDescriptor {
#if !SL
		#region ICustomTypeDescriptor impl
#if CF
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return null;
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return null;
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return null;
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return null;
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return null;
		}
#else
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(this, true);
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(this, true);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(this, true);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(this, true);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}
#endif
		string ICustomTypeDescriptor.GetClassName() {
			return GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return GetType().Name;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return TypeDescriptor.GetEvents(this, true);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) {
			return TypeDescriptor.GetEvents(this, attributes, true);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return Session.GetProperties(ClassInfo);
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return Session.GetProperties(ClassInfo);
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return this;
		}
		#endregion
#endif
		[Flags]
		enum ObjectState { None = 0, Editing = 1, Changed = 4, TrackChangedEvent = 0x010000, ChangedInsideTrackChangedEvent = 0x020000, SkipChangedOnNextLoaded = 0x040000, NeedSaveAtTheEndOfEndEdit = 0x080000 }
		ObjectState _state;
		void IEditableObject.BeginEdit() {
			BeginEdit();
		}
		void IEditableObject.EndEdit() {
			EndEdit();
		}
		void IEditableObject.CancelEdit() {
			CancelEdit();
		}
		protected void RaiseChangeEvent(ObjectChangeEventArgs args) {
			RaiseChangedEvent(this, args);
		}
		bool GetState(ObjectState state) {
			return (this._state & state) == state;
		}
		void SetState(ObjectState state, bool value) {
			if(value)
				this._state |= state;
			else
				this._state &= ~state;
		}
		protected virtual void BeginEdit() {
			if(!GetState(ObjectState.Editing)) {
#if DEBUG
				if(IsLoading)
					throw new InvalidOperationException("Unexpected begin edit during loading process");
#endif
				SetState(ObjectState.Editing, true);
				SetState(ObjectState.Changed, false);
				if(!Session.IsDesignMode)
					SaveState();
				TriggerObjectChanged(new ObjectChangeEventArgs(Session, this, ObjectChangeReason.BeginEdit));
			}
		}
		List<object> _values;
		void SaveState() {
			_values = new List<object>();
			ObjectSet objects = new ObjectSet();
			SaveState(this, objects);
		}
		bool IsGoodForIEditableObjectSaveRestoreState(XPMemberInfo mi) {
			return !mi.IsReadOnly && !(mi is ServiceField);
		}
		void SaveState(object target, ObjectSet objects) {
			if(target == null)
				return;
			if(objects.Contains(target))
				return;
			objects.Add(target);
			XPClassInfo ci = Session.GetClassInfo(target);
			foreach(XPMemberInfo mi in ci.PersistentProperties) {
				if(!IsGoodForIEditableObjectSaveRestoreState(mi))
					continue;
				if(mi.IsDelayed) {
					XPDelayedProperty delayed = XPDelayedProperty.GetDelayedPropertyContainer(target, mi);
					_values.Add(delayed.IsLoaded);
					_values.Add(delayed.InternalValue);
				} else
					_values.Add(mi.GetValue(target));
			}
			foreach(XPMemberInfo mi in ci.ObjectProperties) {
				if(mi.IsAggregated)
					SaveState(mi.GetValue(target), objects);
			}
		}
		void RestoreState() {
			int index = 0;
			ObjectSet objects = new ObjectSet();
			SessionStateStack.Enter(Session, SessionState.CancelEdit);
			try {
				RestoreState(this, ref index, objects);
			} finally {
				SessionStateStack.Leave(Session, SessionState.CancelEdit);
			}
			Session.TriggerObjectsLoaded(objects);
			_values = null;
		}
		void RestoreState(object target, ref int index, ObjectSet objects) {
			if(target == null)
				return;
			if(objects.Contains(target))
				return;
			objects.Add(target);
			XPClassInfo ci = Session.GetClassInfo(target);
			Session.TriggerObjectLoading(target);
			foreach(XPMemberInfo mi in ci.PersistentProperties) {
				if(!IsGoodForIEditableObjectSaveRestoreState(mi))
					continue;
				if(mi.IsDelayed) {
					XPDelayedProperty delayed = XPDelayedProperty.GetDelayedPropertyContainer(target, mi);
					bool loaded = (bool)_values[index++];
					object value = _values[index++];
					if(loaded)
						delayed.Value = value;
					else
						delayed.InternalValue = value;
				} else {
					mi.SetValue(target, _values[index++]);
				}
			}
			foreach(XPMemberInfo mi in ci.ObjectProperties) {
				if(mi.IsAggregated)
					RestoreState(mi.GetValue(target), ref index, objects);
			}
		}
		protected virtual void EndEdit() {
			if(GetState(ObjectState.Editing)) {
				if(GetState(ObjectState.Changed) || Session.IsNewObject(this))
					SetState(ObjectState.NeedSaveAtTheEndOfEndEdit, true);
				SetState(ObjectState.Editing, false);
				SetState(ObjectState.Changed, false);
				_values = null;
				TriggerObjectChanged(new ObjectChangeEventArgs(Session, this, ObjectChangeReason.EndEdit));
				if(GetState(ObjectState.NeedSaveAtTheEndOfEndEdit)) {
					SetState(ObjectState.NeedSaveAtTheEndOfEndEdit, false);
					DoEndEditAction();
				}
			} else {
				TriggerObjectChanged(new ObjectChangeEventArgs(Session, this, ObjectChangeReason.EndEdit));
			}
		}
		public static bool AutoSaveOnEndEdit = true;
		protected virtual void DoEndEditAction() {
			if(AutoSaveOnEndEdit) {
				if(ClassInfo.IsPersistent) {
					Save();
				}
			}
		}
		protected virtual void CancelEdit() {
			if(GetState(ObjectState.Editing)) {
				if(GetState(ObjectState.Changed)) {
					RestoreState();
				}
				_values = null;
				SetState(ObjectState.Changed, false);
				SetState(ObjectState.Editing, false);
				TriggerObjectChanged(new ObjectChangeEventArgs(Session, this, ObjectChangeReason.CancelEdit));
			}
		}
		protected void OnChanged() {
			OnChanged(null, null, null);
		}
		protected void OnChanged(string propertyName) {
			OnChanged(propertyName, null, null);
		}
		protected override void OnChanged(string propertyName, object oldValue, object newValue) {
			if(IsLoading)
				return;
			SetState(ObjectState.Changed, true);
			if(GetState(ObjectState.TrackChangedEvent))
				SetState(ObjectState.ChangedInsideTrackChangedEvent, true);
			base.OnChanged(propertyName, oldValue, newValue);
		}
		[Obsolete("Use GetPropertyValue<T> instead", true), EditorBrowsable(EditorBrowsableState.Never)]
		protected object GetPropertyValueWithDefault(string propertyName, object defaultValue) {
			object result = GetPropertyValue(propertyName);
			if(result == null)
				return defaultValue;
			else
				return result;
		}
		protected XPBaseObject() : this(XpoDefault.GetSession()) { }
		protected XPBaseObject(Session session)
			: base(session) {
			CommonCtorCode();
		}
		protected XPBaseObject(Session session, XPClassInfo classInfo)
			: base(session, classInfo) {
			CommonCtorCode();
		}
		void CommonCtorCode() {
			if(IsLoading)
				SetState(ObjectState.SkipChangedOnNextLoaded, true);
		}
		[Browsable(false)]
		public object This { get { return this; } }
		public static void AddChangedEventHandler(object persistentObject, ObjectChangeEventHandler handler) {
			XPBaseObject obj = persistentObject as XPBaseObject;
#if !SL
			if(obj == null || obj.Session.GetIdentityMapBehavior() == IdentityMapBehavior.Weak)
				ObjectRecord.AddChangeHandler(persistentObject, handler);
			else
#else
			if (obj == null) return;
#endif
			obj.handler += handler;
		}
		public static void RemoveChangedEventHandler(object persistentObject, ObjectChangeEventHandler handler) {
			XPBaseObject obj = persistentObject as XPBaseObject;
#if !SL
			if(obj == null) {
				ObjectRecord.RemoveChangeHandler(persistentObject, handler);
			} else if(obj.IsInvalidated) {
			} else if(obj.Session.GetIdentityMapBehavior() == IdentityMapBehavior.Weak) {
				ObjectRecord.RemoveChangeHandler(persistentObject, handler);
			} else {
#else
			if (obj != null) {
				if (obj.IsInvalidated) return;
#endif
				obj.handler -= handler;
			}
		}
		public static void AddChangedEventHandler(object persistentObject, IObjectChange handler) {
			XPBaseObject obj = persistentObject as XPBaseObject;
#if !SL
			if(obj == null || obj.Session.GetIdentityMapBehavior() == IdentityMapBehavior.Weak)
				ObjectRecord.AddChangeHandler(persistentObject, ObjectRecord.GetObjectRecord(handler));
			else
#else
			if (obj == null) return;
#endif
			obj.handler += new ObjectChangeEventHandler(handler.OnObjectChanged);
		}
		public static void RemoveChangedEventHandler(object persistentObject, IObjectChange handler) {
			XPBaseObject obj = persistentObject as XPBaseObject;
#if !SL
			if(obj == null) {
				ObjectRecord.RemoveChangeHandler(persistentObject, ObjectRecord.GetObjectRecord(handler));
			} else if(obj.IsInvalidated) {
			} else if(obj.Session.GetIdentityMapBehavior() == IdentityMapBehavior.Weak) {
				ObjectRecord.RemoveChangeHandler(persistentObject, ObjectRecord.GetObjectRecord(handler));
			} else {
#else
			if (obj != null) {
				if (obj.IsInvalidated) return;
#endif
				obj.handler -= new ObjectChangeEventHandler(handler.OnObjectChanged);
			}
		}
		protected override void TriggerObjectChanged(ObjectChangeEventArgs args) {
			RaiseChangeEvent(args);
			if(IsInvalidated)   
				return;
			base.TriggerObjectChanged(args);
		}
		public static void RaiseChangedEvent(object persistentObject, ObjectChangeEventArgs args) {
			XPBaseObject obj = persistentObject as XPBaseObject;
#if !SL
			if(obj == null || obj.Session.GetIdentityMapBehavior() == IdentityMapBehavior.Weak) {
				ObjectRecord.OnChange(persistentObject, persistentObject, args);
			} else {
#else
			if(obj != null) {
#endif
				if(obj.handler != null)
					obj.handler(persistentObject, args);
			}
		}
		ObjectChangeEventHandler handler;
		public event ObjectChangeEventHandler Changed {
			add {
				AddChangedEventHandler(this, value);
			}
			remove {
				RemoveChangedEventHandler(this, value);
			}
		}
		public void Save() {
			Session.Save(this);
		}
		public void Delete() {
			Session.Delete(this);
		}
		public void Reload() {
			Session.Reload(this);
		}
		int IComparable.CompareTo(object value) {
			XPBaseObject another = value as XPBaseObject;
			if(another == null)
				return 1;
			if(Object.ReferenceEquals(another, this))
				return 0;
			int result = Comparer.Default.Compare(Session.GetKeyValue(this), Session.GetKeyValue(another));
			if(result != 0)
				return result;
			bool selfNew = Session.IsNewObject(this);
			bool valNew = Session.IsNewObject(another);
			if(selfNew) {
				if(valNew) {
					result = GetImmutableHashCode() - another.GetImmutableHashCode();
					if(result != 0)
						return result;
					else
						return -1;
				} else {
					return -1;
				}
			} else {
				if(valNew) {
					return 1;
				} else {
					return 0;
				}
			}
		}
		[Obsolete("Use the XPBaseObject.OnLoaded method instead.", true), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void AfterLoad() { }
		[Obsolete("Use the XPBaseObject.OnSaving method instead.", true), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void BeforeSave() { }
		[Obsolete("Use the XPBaseObject.OnLoading method instead.", true), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void BeginLoad() { }
		[Obsolete("Use the XPBaseObject.OnLoaded method instead.", true), EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void EndLoad() { }
		[Obsolete("Use the XPBaseObject.IsLoading property instead.", true), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), MemberDesignTimeVisibility(false)]
		public bool Loading { get { return IsLoading; } }
		protected override void OnDeleting() {
			SetState(ObjectState.Changed, true);
			base.OnDeleting();
		}
		protected override void OnSaved() {
			SetState(ObjectState.NeedSaveAtTheEndOfEndEdit, false);
			base.OnSaved();
			TriggerObjectChanged(new ObjectChangeEventArgs(Session, this, ObjectChangeReason.Reset));
		}
		protected override void OnLoaded() {
			base.OnLoaded();
			if(GetState(ObjectState.SkipChangedOnNextLoaded)) {
				SetState(ObjectState.SkipChangedOnNextLoaded, false);
			} else {
				TriggerObjectChanged(new ObjectChangeEventArgs(Session, this, ObjectChangeReason.Reset));
			}
		}
		protected XPCollection GetCollection(string propertyName) {
			return (XPCollection)GetPropertyValue(propertyName);
		}
		protected XPCollection<T> GetCollection<T>(string propertyName) where T : class {
			XPMemberInfo property = ClassInfo.GetMember(propertyName);
			object value;
			CustomPropertyStore.TryGetValue(property, out value);
			XPCollection<T> coll = (XPCollection<T>)value;
			if(coll == null) {
				coll = CreateCollection<T>(property);
				CustomPropertyStore[property] = coll;
			}
			return coll;
		}
		protected virtual XPCollection<T> CreateCollection<T>(XPMemberInfo property) {
			XPCollection<T> col = new XPCollection<T>(this.Session, this, property);
			GC.SuppressFinalize(col);
			return col;
		}
		public void SetMemberValue(string propertyName, object newValue) {
			XPMemberInfo memberInfo = ClassInfo.GetMember(propertyName);
			memberInfo.SetValue(this, newValue);
		}
		public object GetMemberValue(string propertyName) {
			XPMemberInfo memberInfo = ClassInfo.GetMember(propertyName);
			return memberInfo.GetValue(this);
		}
		protected override void Invalidate(bool disposing) {	
			base.Invalidate(disposing);
			this.handler = null;
		}
		public object Evaluate(CriteriaOperator expression) {
			return new ExpressionEvaluator(ClassInfo.GetEvaluatorContextDescriptor(), expression, Session.CaseSensitive, Session.Dictionary.CustomFunctionOperators).Evaluate(this);
		}
		public bool Fit(CriteriaOperator condition) {
			return new ExpressionEvaluator(ClassInfo.GetEvaluatorContextDescriptor(), condition, Session.CaseSensitive, Session.Dictionary.CustomFunctionOperators).Fit(this);
		}
		public object Evaluate(string expression) {
			return this.Evaluate(CriteriaOperator.Parse(expression));
		}
		public bool Fit(string condition) {
			return this.Fit(CriteriaOperator.Parse(condition));
		}
		public object EvaluateAlias(string memberName) {
			XPMemberInfo mi = ClassInfo.GetMember(memberName);
			PersistentAliasAttribute aa = (PersistentAliasAttribute)mi.GetAttributeInfo(typeof(PersistentAliasAttribute));
			return Evaluate(aa.Criteria);
		}
		protected virtual void FireChangedByCustomPropertyStore(XPMemberInfo member, object oldValue, object newValue) {
			if(IsLoading)
				return;
			OnChanged(member.Name, oldValue, newValue);
		}
		protected virtual void FireChangedByXPPropertyDescriptor(string memberName) {
			if(GetState(ObjectState.TrackChangedEvent) && GetState(ObjectState.ChangedInsideTrackChangedEvent))
				return;
			OnChanged(memberName);
		}
		protected virtual void BeforeChangeByXPPropertyDescriptor() {
			if(GetState(ObjectState.Editing)) {
				TriggerObjectChanged(new ObjectChangeEventArgs(Session, this, ObjectChangeReason.BeforePropertyDescriptorChangeWithinBeginEdit));
			}
			SetState(ObjectState.TrackChangedEvent, true);
		}
		protected virtual void AfterChangeByXPPropertyDescriptor() {
			SetState(ObjectState.TrackChangedEvent | ObjectState.ChangedInsideTrackChangedEvent, false);
		}
		void IXPReceiveOnChangedFromXPPropertyDescriptor.FireChangedByXPPropertyDescriptor(string memberName) {
			FireChangedByXPPropertyDescriptor(memberName);
		}
		void IXPReceiveOnChangedFromXPPropertyDescriptor.BeforeChangeByXPPropertyDescriptor() {
			BeforeChangeByXPPropertyDescriptor();
		}
		void IXPReceiveOnChangedFromXPPropertyDescriptor.AfterChangeByXPPropertyDescriptor() {
			AfterChangeByXPPropertyDescriptor();
		}
	}
	[Persistent, MemberDesignTimeVisibility(false)]
	public sealed class XPObjectType : IXPOServiceClass {
		XPDictionary dictionary;
		Type systemType;
		string typeName;
		string assemblyName;
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPObjectTypeTypeName"),
#endif
Size(254)]
		[Indexed(Unique = true)]
		public string TypeName {
			get {
				return typeName;
			}
			set {
				typeName = value;
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPObjectTypeAssemblyName"),
#endif
Size(254)]
		public string AssemblyName {
			get {
				return assemblyName;
			}
			set {
				assemblyName = value;
			}
		}
		[Browsable(false)]
		public bool IsValidType { get { return TypeClassInfo != null && TypeClassInfo.IsPersistent; } }
		[Browsable(false)]
		public Type SystemType {
			get {
				if(systemType == null) {
					systemType = TypeClassInfo.ClassType;
				}
				return systemType;
			}
		}
		XPClassInfo classInfo;
		[Browsable(false)]
		public XPClassInfo TypeClassInfo {
			get {
				if(classInfo == null)
					classInfo = dictionary.QueryClassInfo(AssemblyName, TypeName);
				return classInfo;
			}
		}
		public XPClassInfo GetClassInfo() {
			if(!this.IsValidType)
				throw new CannotLoadInvalidTypeException(this.TypeName);
			return TypeClassInfo;
		}
		public const string ObjectTypePropertyName = "ObjectType";
		public XPObjectType(Session session) : this(session.Dictionary, string.Empty, string.Empty) { }
		public XPObjectType(Session session, string assemblyName, string typeName)
			: this(session.Dictionary, assemblyName, typeName) {
		}
		public XPObjectType(XPDictionary dictionary, string assemblyName, string typeName) {
			this.dictionary = dictionary;
			this.assemblyName = assemblyName;
			this.typeName = typeName;
		}
		[Persistent("OID"), Key(AutoGenerate = true)]
		public Int32 Oid = -1;
		#region Fields
#if !SL
	[DevExpressXpoLocalizedDescription("XPObjectTypeFields")]
#endif
public static FieldsClass Fields { get { return new FieldsClass(); } }
		public class FieldsClass : FieldsClassBase {
			public FieldsClass() : base() { }
			public FieldsClass(string propertyName) : base(propertyName) { }
			public OperandProperty Oid { get { return new OperandProperty(GetNestedName("Oid")); } }
			public OperandProperty TypeName { get { return new OperandProperty(GetNestedName("TypeName")); } }
			public OperandProperty AssemblyName { get { return new OperandProperty(GetNestedName("AssemblyName")); } }
		}
		#endregion
	}
	public interface IXPObject : IXPSimpleObject {
		void OnLoading();
		void OnLoaded();
		void OnSaving();
		void OnSaved();
		void OnDeleting();
		void OnDeleted();
	}
	public interface IXPInvalidateableObject {
		bool IsInvalidated { get;}
		void Invalidate();
	}
	[NonPersistent, MemberDesignTimeVisibility(false), OptimisticLocking(false)]
	public abstract class XPLiteObject : XPBaseObject {
		protected XPLiteObject() : base() { }
		protected XPLiteObject(Session session) : base(session) { }
		protected XPLiteObject(Session session, XPClassInfo classInfo) : base(session, classInfo) { }
	}
	public class XPDelayedProperty {
		object _value;
		bool isLoaded;
		object owner;
		XPMemberInfo property;
		Session session;
		bool _IsChanged;
		public XPDelayedProperty() { }
		internal XPDelayedProperty(Session session, object theObject, XPMemberInfo mi) {
			this.session = session;
			this.owner = theObject;
			this.property = mi;
		}
		internal static string GetGroupName(XPMemberInfo mi) {
			return ((DelayedAttribute)mi.GetAttributeInfo(typeof(DelayedAttribute))).GroupName;
		}
		internal static bool UpdateModifiedOnly(XPMemberInfo mi) {
			return ((DelayedAttribute)mi.GetAttributeInfo(typeof(DelayedAttribute))).UpdateModifiedOnly;
		}
		internal static XPDelayedProperty GetDelayedPropertyContainer(object theObject, XPMemberInfo mi) {
			DelayedAttribute attr = (DelayedAttribute)mi.GetAttributeInfo(typeof(DelayedAttribute));
			if((attr.FieldName == null || attr.FieldName.Length == 0) && theObject is IXPCustomPropertyStore) {
				return (XPDelayedProperty)((IXPCustomPropertyStore)theObject).GetCustomPropertyValue(mi);
			}
			XPMemberInfo fi = mi.Owner.GetMember(attr.FieldName);
			XPDelayedProperty dp = fi.GetValue(theObject) as XPDelayedProperty;
			if(dp == null)
				throw new ArgumentException(Res.GetString(Res.Object_DelayedPropertyDoesNotContainProperObject, theObject.GetType().FullName, attr.FieldName));
			return dp;
		}
		internal static void Init(Session session, object theObject, XPMemberInfo mi, object val) {
			XPDelayedProperty delayedProp = GetDelayedPropertyContainer(theObject, mi);
			delayedProp.property = mi;
			delayedProp.owner = theObject;
			delayedProp._value = val;
			delayedProp.isLoaded = false;
			delayedProp.session = session;
		}
		internal object InternalValue {
			get {
				return this._value;
			}
			set {
				isLoaded = false;
				_IsChanged = false;
				this._value = value;
			}
		}
		internal static void PreFetchRefProps(Session session, IEnumerable objects, XPMemberInfo property) {
			System.Diagnostics.Debug.Assert(property.IsDelayed);
			XPClassInfo refType = property.ReferenceType;
			System.Diagnostics.Debug.Assert(refType != null);
			Dictionary<object, object> keysToLoad = new Dictionary<object, object>();
			List<XPDelayedProperty> containers = new List<XPDelayedProperty>();
			foreach(object obj in objects) {
				XPDelayedProperty prop = GetDelayedPropertyContainer(obj, property);
				System.Diagnostics.Debug.Assert(obj == prop.owner);
				if(prop.IsLoaded)
					continue;
				object key = prop._value;
				if(key == null) {
					prop.isLoaded = true;
					continue;
				}
				object alreadyLoadedObject = session.GetLoadedObjectByKey(refType, key);
				if(alreadyLoadedObject != null) {
					prop._value = alreadyLoadedObject;
					prop.isLoaded = true;
					continue;
				}
				keysToLoad[key] = key;
				containers.Add(prop);
			}
			if(containers.Count == 0)
				return;
			System.Diagnostics.Debug.Assert(keysToLoad.Count <= containers.Count);
			System.Diagnostics.Debug.Assert(keysToLoad.Count > 0);
			IList keysList = new List<object>(keysToLoad.Keys);
			List<ObjectsQuery> queries = new List<ObjectsQuery>();
			for(int i = 0; i < keysList.Count; ) {
				int inSize = XpoDefault.GetTerminalInSize(keysList.Count - i);
				queries.Add(new ObjectsQuery(refType, new InOperator(refType.KeyProperty.Name, DevExpress.Xpo.Generators.GetRangeHelper.GetRange(keysList, i, inSize)), null, 0, 0, null, false));
				i += inSize;
			}
			System.Diagnostics.Debug.Assert(queries.Count > 0);
			ICollection[] fetchedObjects = session.GetObjects(queries.ToArray());
			foreach(XPDelayedProperty prop in containers) {
				System.Diagnostics.Debug.Assert(prop.IsLoaded == false);
				System.Diagnostics.Debug.Assert(prop._value != null);
				prop._value = session.GetLoadedObjectByKey(refType, prop._value);
				prop.isLoaded = true;
			}
			GC.KeepAlive(fetchedObjects);
		}
		internal static void PreFetchGenericProps(Session session, IEnumerable objects, XPMemberInfo property) {
			System.Diagnostics.Debug.Assert(property.IsDelayed);
			System.Diagnostics.Debug.Assert(property.ReferenceType == null);
			List<object> list = new List<object>();
			foreach(object obj in objects) {
				XPDelayedProperty prop = GetDelayedPropertyContainer(obj, property);
				if(prop.IsLoaded)
					continue;
				System.Diagnostics.Debug.Assert(prop._value == null);
				list.Add(obj);
			}
			if(list.Count == 0)
				return;
			ObjectDictionary<object> fetchedValues = session.LoadDelayedProperties(list, property);
			foreach(KeyValuePair<object, object> entry in fetchedValues) {
				XPDelayedProperty prop = GetDelayedPropertyContainer(entry.Key, property);
				System.Diagnostics.Debug.Assert(prop.IsLoaded == false);
				System.Diagnostics.Debug.Assert(prop._value == null);
				prop._value = entry.Value;
				prop.isLoaded = true;
			}
		}
		void LoadValue() {
			if(isLoaded)
				return;
			if(owner == null)
				return;
			if(session.IsNewObject(owner)) {
				System.Diagnostics.Debug.Assert(_value == null);
				isLoaded = true;
				return;
			}
			XPClassInfo refType = property.ReferenceType;
			if(refType != null) {
				if(_value != null)
					_value = session.GetObjectByKey(refType, _value);
				isLoaded = true;
				return;
			}
			LoadDelayedGroup(session, owner, GetGroupName(property));
			System.Diagnostics.Debug.Assert(isLoaded);
		}
		static void LoadDelayedGroup(Session session, object owner, string groupName) {
			XPClassInfo ci = session.GetClassInfo(owner);
			MemberPathCollection propsList = new MemberPathCollection();
			List<XPDelayedProperty> containersList = new List<XPDelayedProperty>();
			foreach(XPMemberInfo mi in ci.PersistentProperties) {
				if(!mi.IsDelayed)
					continue;
				if(mi.ReferenceType != null)
					continue;
				if(GetGroupName(mi) != groupName)
					continue;
				XPDelayedProperty prop = GetDelayedPropertyContainer(owner, mi);
				if(prop.IsLoaded)
					continue;
				propsList.Add(new MemberInfoCollection(ci, mi));
				containersList.Add(prop);
			}
			System.Diagnostics.Debug.Assert(propsList.Count > 0);
			object[] loadedProps = session.LoadDelayedProperties(owner, propsList);
			for(int i = 0; i < propsList.Count; ++i) {
				XPDelayedProperty prop = containersList[i];
				prop._value = loadedProps[i];
				prop.isLoaded = true;
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XPDelayedPropertyValue")]
#endif
public object Value {
			get {
				LoadValue();
				return this._value;
			}
			set {
				SetValue(value);
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XPDelayedPropertyIsLoaded")]
#endif
public bool IsLoaded { get { return isLoaded; } }
		internal bool SetValue(object newValue) {
			object oldValue = Value;
			if(PersistentBase.CanSkipAssignement(oldValue, newValue))
				return false;
			this._value = newValue;
			this.isLoaded = true;
			_IsChanged = true;
			IXPReceiveOnChangedFromDelayedProperty changeableOwner = owner as IXPReceiveOnChangedFromDelayedProperty;
			if(changeableOwner != null) {
				changeableOwner.FireChangedByDelayedPropertySetter(property, oldValue, newValue);
			}
			return true;
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XPDelayedPropertyIsModified")]
#endif
public bool IsModified { get { return _IsChanged; } }
		internal void ResetIsModified() {
			_IsChanged = false;
		}
	}
	[NonPersistent, MemberDesignTimeVisibility(false), DeferredDeletionAttribute]
	public abstract class XPCustomObject : XPBaseObject {
		protected XPCustomObject() : base() { }
		protected XPCustomObject(Session session) : base(session) { }
		protected XPCustomObject(Session session, XPClassInfo classInfo) : base(session, classInfo) { }
	}
	[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
	[NonPersistent, MemberDesignTimeVisibility(false), DeferredDeletionAttribute]
	public class XPDataObject : XPBaseObject {
		public XPDataObject(Session session, XPClassInfo classInfo) : base(session, classInfo) { }
	}
	[NonPersistent, MemberDesignTimeVisibility(false)]
	public abstract class XPObject : XPCustomObject {
		protected XPObject() : base() { }
		protected XPObject(Session session) : base(session) { }
		protected XPObject(Session session, XPClassInfo classInfo) : base(session, classInfo) { }
		Int32 _oid = -1;
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPObjectOid"),
#endif
Persistent("OID"), Key(AutoGenerate = true)]
		public Int32 Oid {
			get {
				return _oid;
			}
			set {
				Int32 oldValue = Oid;
				if(oldValue == value)
					return;
				_oid = value;
				OnChanged("Oid", oldValue, value);
			}
		}
		#region Fields
#if !SL
	[DevExpressXpoLocalizedDescription("XPObjectFields")]
#endif
new public static FieldsClass Fields { get { return new FieldsClass(); } }
		new public class FieldsClass : XPCustomObject.FieldsClass {
			public FieldsClass() : base() { }
			public FieldsClass(string propertyName) : base(propertyName) { }
			public OperandProperty Oid { get { return new OperandProperty(GetNestedName("Oid")); } }
		}
		#endregion
	}
	[MemberDesignTimeVisibility(false)]
	public class XPWeakReference : XPCustomObject, IXPOServiceClass {
		[Key(true)]
		public Guid Oid;
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPWeakReferenceIsAlive"),
#endif
NonPersistent]
		public bool IsAlive {
			get {
				return Target != null;
			}
		}
		[
#if !SL
	DevExpressXpoLocalizedDescription("XPWeakReferenceTarget"),
#endif
NonPersistent]
		public object Target {
			get {
				if(TargetKeyValue == null)
					return null;
				if(!TargetType.IsValidType)
					return null;
				return this.Session.GetObjectByKey(TargetType.TypeClassInfo, TargetKeyValue);
			}
			set {
				if(value == null) {
					this.TargetType = null;
					this.TargetKeyValue = null;
				} else if(this.Session.IsNewObject(value)) {
					throw new ArgumentException(Res.GetString(Res.XPWeakReference_SavedObjectExpected));
				} else {
					this.TargetType = Session.GetObjectType(value);
					this.TargetKeyValue = Session.GetKeyValue(value);
				}
				OnChanged();
			}
		}
		public XPWeakReference() : base() { }
		public XPWeakReference(Session session) : base(session) { }
		public XPWeakReference(Session session, object target)
			: base(session) {
			this.Target = target;
		}
		public XPWeakReference(IXPSimpleObject target) : this(target.Session, target) { }
		[NonPersistent]
		protected object TargetKeyValue;
		[Persistent("TargetType")]
		[Obsolete("For internal use only (Medium Trust)", true), Browsable(false), MemberDesignTimeVisibility(false), EditorBrowsable(EditorBrowsableState.Never)]
		public XPObjectType TargetType_ {
			get { return TargetType; }
			set { TargetType = value; }
		}
		[Persistent("TargetKey")]
		[Obsolete("For internal use only (Medium Trust)", true), Browsable(false), MemberDesignTimeVisibility(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string TargetKey_ {
			get {
				return TargetKey;
			}
			set {
				TargetKey = value;
			}
		}
		XPObjectType _TargetType;
		[PersistentAlias("TargetType_")]
		protected XPObjectType TargetType {
			get { return _TargetType; }
			set { _TargetType = value; }
		}
		[PersistentAlias("TargetKey_")]
		protected virtual string TargetKey {
			get {
				return KeyToString(this.TargetKeyValue);
			}
			set {
				this.TargetKeyValue = StringToKey(value);
			}
		}
		public static string KeyToString(object key) {
			if(key == null)
				return null;
			ICollection elements = key as IdList;
			if(elements == null) {
				elements = new object[] { key };
			}
			if(elements.Count == 0)
				return null;
			string result = string.Empty;
			foreach(object obj in elements) {
				string subResult = SimpleKeyToString(obj);
				if(result.Length > 0) result += ',';
				result += subResult;
			}
			return result;
		}
		public static object StringToKey(string s) {
			if(s == null)
				return null;
			if(s.Length == 0)
				return null;
			ICollection strings = SplitStringToSimplePairs(s);
			IdList result = new IdList();
			foreach(object[] pair in strings) {
				result.Add(SimpleStringToKey((string)pair[0], (string)pair[1]));
			}
			if(result.Count == 0)
				return null;
			if(result.Count == 1)
				return result[0];
			return result;
		}
		static ICollection SplitStringToSimplePairs(string s) {
			ArrayList result = new ArrayList();
			DevExpress.Data.Filtering.Helpers.CriteriaLexer lexer = new DevExpress.Data.Filtering.Helpers.CriteriaLexer(new System.IO.StringReader(s));
			if(lexer.Advance()) {
				for(; ; ) {
					DevExpress.Data.Filtering.OperandProperty prop = lexer.CurrentValue as DevExpress.Data.Filtering.OperandProperty;
					if(ReferenceEquals(prop, null))
						throw new ArgumentException();
					if(!lexer.Advance())
						throw new ArgumentException();
					DevExpress.Data.Filtering.OperandValue val = lexer.CurrentValue as DevExpress.Data.Filtering.OperandValue;
					if(ReferenceEquals(val, null))
						throw new ArgumentException();
					result.Add(new object[] { prop.PropertyName, val.Value });
					if(!lexer.Advance())
						break;
					if(lexer.CurrentToken != ',')
						throw new ArgumentException();
					if(!lexer.Advance())
						throw new ArgumentException();
				}
			}
			return result;
		}
		static string SimpleKeyToString(object key) {
			if(key == null)
				throw new ArgumentNullException();
			string typeString;
			string val;
			if(key is Guid) {
				typeString = "Guid";
				val = ((Guid)key).ToString();
			} else if(key is IConvertible) {
				typeString = Type.GetTypeCode(key.GetType()).ToString();
				val = Convert.ToString(key, System.Globalization.CultureInfo.InvariantCulture);
			} else
				throw new ArgumentException();
			object q = new DevExpress.Data.Filtering.OperandProperty(typeString);
			object w = new DevExpress.Data.Filtering.OperandValue(val);
			return q.ToString() + w.ToString();
		}
		static object SimpleStringToKey(string typeCodeString, string value) {
			if(typeCodeString == "Guid")
				return new Guid(value);
			TypeCode typeCode = (TypeCode)Enum.Parse(typeof(TypeCode), typeCodeString, false);
			return Convert.ChangeType(value, typeCode, System.Globalization.CultureInfo.InvariantCulture);
		}
	}
}
#if PERSISTENT_EVENTS //!CF
namespace DevExpress.Xpo.Helpers.PersistentEvents {
	[MemberDesignTimeVisibility(false)]
	public class EventHandlerLink : XPWeakReference {
		public EventHandlerLink(string method, object target, object owner) {
			Method = method;
			this.Target = target;
		}
		public EventHandlerLink() {
		}
		public string Method;
		public Delegate GetHandler(Type delegateType) {
			return Delegate.CreateDelegate(delegateType, this.Target, Method);
		}
	}
	class EventCollectionHelper : XPRefCollectionHelper {
		public EventCollectionHelper(object owner)
			: base(null, owner, null) {
		}
	}
	public class EventHandlerCollection : XPCollection {
		Session session;
		Type delegateType;
		string method;
		object owner;
		public EventHandlerCollection(Session session, Type delegateType, string name, object owner)
			: base(new EventCollectionHelper(owner)) {
			Helper.ObjInfo = session.GetClassInfo(typeof(EventHandlerLink));
			this.session = session;
			this.delegateType = delegateType;
			this.method = method;
			this.owner = owner;
		}
		public void Add(Delegate value) {
			base.BaseAdd(new EventHandlerLink(value.Method.Name, value.Target, owner));
		}
		public void Remove(Delegate value) {
			foreach(EventHandlerLink obj in this)
				if(obj.Target == value.Target && value.Method.Name == obj.Method) {
					base.BaseRemove(obj);
					break;
				}
		}
		public Delegate Handler {
			get {
				Delegate handler = null;
				foreach(EventHandlerLink obj in this)
					handler = Delegate.Combine(handler, obj.GetHandler(delegateType));
				return handler;
			}
		}
	}
}
#endif
