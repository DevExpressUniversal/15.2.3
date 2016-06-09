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
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using System.Text;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Security {
	public class ParticularAccessItem {
		private ITypeInfo objectTypeInfo;
		public ParticularAccessItem(Type objectType, ObjectAccess particularAccess, ObjectAccessModifier modifier) {
			if(ObjectAccessPermission.SplitFlagsToArray(particularAccess).Length != 1) {
				throw new ArgumentException(
					string.Format("The '{0}' access is incorrect, only particular access is allowed",
					particularAccess));
			}
			this.objectTypeInfo = (objectType == null) ? null : XafTypesInfo.Instance.FindTypeInfo(objectType);
			this.Access = particularAccess;
			this.Modifier = modifier;
		}
		public ITypeInfo OwnerTypeInfo {
			get {
				return ((ReferenceToOwner == null) ? null : ReferenceToOwner.MemberTypeInfo);
			}
		}
		public Type ObjectTypeOwner {
			get {
				return ((ReferenceToOwner == null) ? null : ReferenceToOwner.MemberType);
			}
		}
		public IMemberInfo ReferenceToOwner {
			get {
				return ((ObjectTypeInfo == null) ? null : ObjectTypeInfo.ReferenceToOwner);
			}
		}
		public Type ObjectType {
			get { return (objectTypeInfo == null) ? null : objectTypeInfo.Type; }
		}
		public ITypeInfo ObjectTypeInfo {
			get { return objectTypeInfo; }
		}
		public readonly ObjectAccess Access;
		public ObjectAccessModifier Modifier;
		public bool IsSame(Type objectType, ObjectAccess access) {
			return objectType == ObjectType && Access == access;
		}
	}
	public class ObjectAccessItemList : IEnumerable<ParticularAccessItem> {
		internal List<ParticularAccessItem> items = new List<ParticularAccessItem>();
		private ITypesInfo typesInfo;
		public ObjectAccessItemList() {
			typesInfo = XafTypesInfo.Instance;
		}
		public ObjectAccessItemList(ObjectAccessItemList itemList) : this() {
			Add(itemList);
		}
		public void Add(ParticularAccessItem item) {
			if(item == null) {
				throw new ArgumentNullException();
			}
			items.Add(item);
		}
		public void Remove(ParticularAccessItem item) {
			if(item != null)
				items.Remove(item);
		}
		public void Add(ObjectAccessItemList itemList) {
			foreach(ParticularAccessItem item in itemList) {
				Add(item);
			}
		}
		public ParticularAccessItem FindExactItem(Type type, ObjectAccess access) {
			foreach(ParticularAccessItem item in items) {
				if(item.IsSame(type, access)) {
					return item;
				}
			}
			return null;
		}
		public ParticularAccessItem FindAccessItem(Type objectType, ObjectAccess access) {
			Type currentType = objectType;
			while(true) {
				if(currentType == typeof(object)) {
					if((objectType != null) && typesInfo.FindTypeInfo(objectType).IsPersistent) {
						ParticularAccessItem persistentBaseObjectItem = FindExactItem(PermissionTargetBusinessClassListConverter.PersistentBaseObjectType, access);
						if(persistentBaseObjectItem != null) {
							return persistentBaseObjectItem;
						}
					}
				}
				ParticularAccessItem item = FindExactItem(currentType, access);
				if(item != null) {
					return item;
				}
				if((currentType != null) && currentType.IsInterface) {
					currentType = typeof(object);
				}
				else {
					if((currentType == null) || (currentType == typeof(object))) {
						break;
					}
					currentType = currentType.BaseType;
				}
			}
			return null;
		}
		public void PackItems() {
			if(items.Count > 1) {
				ObjectAccessItemList result = new ObjectAccessItemList();
				foreach(ParticularAccessItem item in items) {
					ParticularAccessItem resultItem = result.FindExactItem(item.ObjectType, item.Access);
					if(resultItem == null) {
						result.Add(item);
					}
					else {
						if(item.Modifier == ObjectAccessModifier.Deny) {
							resultItem.Modifier = ObjectAccessModifier.Deny;
						}
					}
				}
				items.Clear();
				foreach(ParticularAccessItem item in result.items) {
					Add(item);
				}
			}
		}
		public bool IsEmpty {
			get { return items.Count == 0; }
		}
		public int Count {
			get { return items.Count; }
		}
		public void Clear() {
			items.Clear();
		}
		public ParticularAccessItem this[int index] {
			get { return items[index]; }
		}
		IEnumerator<ParticularAccessItem> IEnumerable<ParticularAccessItem>.GetEnumerator() {
			return items.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return items.GetEnumerator();
		}
		public ParticularAccessItem[] ToArray() {
			return items.ToArray();
		}
	}
	[Flags]
	public enum ObjectAccess { NoAccess = 0, Read = 1, Write = 2, Create = 4, Delete = 8, Navigate = 16,
		AllAccess = Read + Write + Create + Delete + Navigate, ChangeAccess = Write + Create + Delete }
	public enum ObjectAccessModifier { None, Allow, Deny }
	class ObjectAccessPermissionDebugView {
		private ObjectAccessPermission objectAccessPermission;
		public ObjectAccessPermissionDebugView(ObjectAccessPermission objectAccessPermission) {
			this.objectAccessPermission = objectAccessPermission;
		}
		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public ParticularAccessItem[] Items {
			get { return objectAccessPermission.AccessItemList.ToArray(); }
		}
	}
	public enum ObjectAccessCompareMode { Strict, ConsiderOwnerPermissions }
	public abstract class ObjectAccessComparerBase {
		private static ObjectAccessComparerBase currentComparer = new ObjectAccessComparer(ObjectAccessCompareMode.ConsiderOwnerPermissions);
		private static bool isCurrentComparerLocked = false;
		public static bool IsCurrentUser(TargetObjectContext targetObjectContext) {
			if((SecuritySystem.UserType != null) && (targetObjectContext != null) && ReflectionHelper.AreEqual(targetObjectContext.TargetObject, SecuritySystem.CurrentUser)) {
				return true;
			}
			return false;
		}
		private static ObjectAccessPermission CreatePermissionToOwner(IMemberInfo referenceToOwner, ObjectAccess sourceAccess, SecurityContextList sourceContexts) {
			ObjectAccess ownerAccess = sourceAccess;
			switch(sourceAccess) {
				case ObjectAccess.Create:
				case ObjectAccess.Delete:
				case ObjectAccess.Write:
					ownerAccess = ObjectAccess.Write;
					break;
			}
			Type resultOwnerType = referenceToOwner.MemberType;
			CollectionPropertyContext collectionPropertyContext = sourceContexts.CollectionPropertyContext;
			if(collectionPropertyContext != null) {
				if((collectionPropertyContext.MasterObject != null)
					&& referenceToOwner.MemberType.IsAssignableFrom(collectionPropertyContext.MasterObject.GetType())) {
					resultOwnerType = collectionPropertyContext.MasterObject.GetType();
				}
				else if(referenceToOwner.MemberType.IsAssignableFrom(collectionPropertyContext.MasterObjectType)) {
					resultOwnerType = collectionPropertyContext.MasterObjectType;
				}
			}
			SecurityContextList contexts = new SecurityContextList();
			if(collectionPropertyContext != null && collectionPropertyContext.MasterObject != null) {
				contexts.Add(new TargetObjectContext(collectionPropertyContext.MasterObject));
			}
			else if(sourceContexts.TargetObjectContext != null) {
				if(sourceContexts.TargetObjectContext.TargetObject != null && referenceToOwner.Owner.Type.IsAssignableFrom(sourceContexts.TargetObjectContext.TargetObject.GetType())) {
					object currentMasterObject = referenceToOwner.GetValue(sourceContexts.TargetObjectContext.TargetObject);
					if(currentMasterObject == null) {
					}
					else {
						contexts.Add(new TargetObjectContext(currentMasterObject));
					}
				}
			}
			return new ObjectAccessPermission(resultOwnerType, ownerAccess, contexts);
		}
		public static bool IsSubsetOf(ObjectAccessCompareMode compareMode, ObjectAccessPermission sourcePermission, ObjectAccessPermission targetPermission) {
			foreach(ParticularAccessItem item in sourcePermission.AccessItemList) {
				Type actualObjectType = item.ObjectType;
				if(sourcePermission.Contexts.TargetObjectContext != null && sourcePermission.Contexts.TargetObjectContext.TargetObject != null) {
					if(item.ObjectType.IsAssignableFrom(sourcePermission.Contexts.TargetObjectContext.TargetObject.GetType())) {
						actualObjectType = sourcePermission.Contexts.TargetObjectContext.TargetObject.GetType();
					}
				}
				ParticularAccessItem exactTargetItem = targetPermission.AccessItemList.FindExactItem(actualObjectType, item.Access);
				if((exactTargetItem != null) && (exactTargetItem.Modifier == ObjectAccessModifier.Deny)) {
					return false;
				}
				if((compareMode == ObjectAccessCompareMode.ConsiderOwnerPermissions) && (item.ReferenceToOwner != null)
						&& (item.ObjectType != item.ReferenceToOwner.MemberType)
						&& ((exactTargetItem == null) || (exactTargetItem.Modifier == ObjectAccessModifier.Allow))) {
					ObjectAccessPermission permissionToOwner = CreatePermissionToOwner(
						item.ReferenceToOwner, item.Access, sourcePermission.Contexts);
					if(!permissionToOwner.IsSubsetOf(targetPermission)) {
						return false;
					}
				}
				else {
					if(exactTargetItem != null) {
						if(exactTargetItem.Modifier == ObjectAccessModifier.Deny) {
							return false;
						}
					}
					else {
						ParticularAccessItem targetItem = targetPermission.AccessItemList.FindAccessItem(item.ObjectType, item.Access);
						if(targetItem == null) {
							return false;
						}
						else {
							if(targetItem.Modifier == ObjectAccessModifier.Deny) {
								return false;
							}
						}
					}
				}
			}
			return true;
		}
		public static ObjectAccessComparerBase CurrentComparer {
			get { return currentComparer; }
		}
		public static void SetCurrentComparer(ObjectAccessComparerBase comparer) {
			if(isCurrentComparerLocked) {
				throw new InvalidOperationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.TheValueWasLocked));
			}
			currentComparer = comparer;
		}
		public static void LockCurrentComparer() {
			isCurrentComparerLocked = true;
		}
		public abstract bool IsSubsetOf(ObjectAccessPermission sourcePermission, ObjectAccessPermission targetPermission);
		public abstract bool IsMemberModificationDenied(object targetObject, IMemberInfo memberDescriptor);
		public abstract bool IsMemberReadGranted(Type requestedType, string propertyName, SecurityContextList securityContexts);
#if DebugTest
		public static void DEBUG_ClearLock() {
			isCurrentComparerLocked = false;
		}
#endif
	}
	public class ObjectAccessComparer : ObjectAccessComparerBase {
		public static bool AllowModifyCurrentUserObjectDefault = true;
		public static bool AllowNavigateToCurrentUserObjectDefault = true;
		private ObjectAccessCompareMode objectAccessCompareMode = ObjectAccessCompareMode.ConsiderOwnerPermissions;
		private bool allowModifyCurrentUserObject = AllowModifyCurrentUserObjectDefault;
		private bool allowNavigateToCurrentUserObject = AllowNavigateToCurrentUserObjectDefault;
		public ObjectAccessComparer() {
		}
		public ObjectAccessComparer(ObjectAccessCompareMode objectAccessCompareMode) {
			this.objectAccessCompareMode = objectAccessCompareMode;
		}
		public override bool IsSubsetOf(ObjectAccessPermission sourcePermission, ObjectAccessPermission targetPermission) {
			if((sourcePermission.AccessItemList.Count == 1) && (SecuritySystem.UserType != null) 
				&& IsCurrentUser(sourcePermission.Contexts.TargetObjectContext)) {
				if((sourcePermission.AccessItemList[0].Access == ObjectAccess.Write) && allowModifyCurrentUserObject) {
					return true;
				}
				if((sourcePermission.AccessItemList[0].Access == ObjectAccess.Navigate) && allowNavigateToCurrentUserObject) {
					return true;
				}
			}
			return IsSubsetOf(ObjectAccessCompareMode, sourcePermission, targetPermission);
		}
		public override bool IsMemberModificationDenied(object targetObject, IMemberInfo memberDescriptor) {
			Guard.ArgumentNotNull(memberDescriptor, "memberDescriptor");
			List<IMemberInfo> path = new List<IMemberInfo>(memberDescriptor.GetPath());
			if(path.Count > 1) {
				object currentObject = targetObject;
				for (int i = 0; i < path.Count; i++) {
					IMemberInfo mi = path[i];
					Type intermediateMemberActualDeclaredType = mi.Owner.Type;
					if(currentObject != null) {
						intermediateMemberActualDeclaredType = currentObject.GetType();
					}
					if(!SecuritySystem.IsGranted(new ObjectAccessPermission(intermediateMemberActualDeclaredType, ObjectAccess.Write, new TargetObjectContext(currentObject)))) {
						return true;
					}
					if(SimpleTypes.IsSimpleType(mi.MemberType)) { 
						break;
					}
					if(currentObject != null) {
						currentObject = mi.GetValue(currentObject);
					}
				}
			}
			if(targetObject == null) {
				return false;
			}
			if((SecuritySystem.Instance is IExtensibleSecurity) && (SecuritySystem.UserType != null)) {
				if(!SecuritySystem.IsGranted(new ObjectAccessPermission(targetObject.GetType(), ObjectAccess.Write))
					&& SecuritySystem.IsGranted(new ObjectAccessPermission(targetObject.GetType(), ObjectAccess.Write, new TargetObjectContext(targetObject)))) {
					if(((IExtensibleSecurity)SecuritySystem.Instance).IsSecurityMember(targetObject.GetType(), memberDescriptor.Name)) {
							return true;
					}
				}
			}
			return false;
		}
		public override bool IsMemberReadGranted(Type requestedType, string propertyName, SecurityContextList securityContexts) {
			Guard.ArgumentNotNull(requestedType, "requestedType");
			Guard.ArgumentNotNullOrEmpty(propertyName, propertyName);
			Guard.ArgumentNotNull(XafTypesInfo.Instance, "XafTypesInfo.Instance");
			ITypeInfo requestedTypeInfo = XafTypesInfo.Instance.FindTypeInfo(requestedType);
			if(requestedTypeInfo == null) {
				throw new TypeWasNotFoundException(requestedType.FullName);
			}
			IMemberInfo memberDescriptor = requestedTypeInfo.FindMember(propertyName);
			if(memberDescriptor == null) {
				throw new MemberNotFoundException(requestedTypeInfo.Type, propertyName);
			}
			bool isRequestedTypeGrantedWithRead = SecuritySystem.IsGranted(new ObjectAccessPermission(requestedType, ObjectAccess.Read, securityContexts));
			if(!isRequestedTypeGrantedWithRead && (memberDescriptor.Owner != null)) {
				if(requestedType == memberDescriptor.Owner.Type) {
					return false;
				}
				if(!SecuritySystem.IsGranted(new ObjectAccessPermission(memberDescriptor.Owner.Type, ObjectAccess.Read, securityContexts))) {
					return false;
				}
			}
			foreach(IMemberInfo mi in memberDescriptor.GetPath()) {
				if(mi.MemberTypeInfo.IsPersistent
				  && !SecuritySystem.IsGranted(new ObjectAccessPermission(mi.MemberType, ObjectAccess.Read, securityContexts))) {
					return false;
				}
			}
			return true;
		}
		public ObjectAccessCompareMode ObjectAccessCompareMode {
			get { return objectAccessCompareMode; }
			set { objectAccessCompareMode = value; }
		}
		public bool AllowNavigateToCurrentUserObject {
			get { return allowNavigateToCurrentUserObject; }
			set { allowNavigateToCurrentUserObject = value; }
		}
		public bool AllowModifyCurrentUserObject {
			get { return allowModifyCurrentUserObject; }
			set { allowModifyCurrentUserObject = value; }
		}
	}
	public class SecurityContextList : LightDictionary<Type, SecurityContext> {
		protected override void OnChanging(ListChangedType listChangedType, Type key, SecurityContext oldValue, SecurityContext newValue) {
			if(key == null) {
				throw new ArgumentNullException("key");
			}
			if(listChangedType == ListChangedType.ItemAdded || listChangedType == ListChangedType.ItemChanged) {
				if(newValue != null) {
					if(!key.IsAssignableFrom(newValue.GetType())) {
						throw new InvalidCastException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToCast, newValue.GetType(), key));
					}
				}
			}
			base.OnChanging(listChangedType, key, oldValue, newValue);
		}
		public SecurityContextList() { }
		public SecurityContextList(IList<SecurityContext> contexts) : this() {
			if(contexts != null) {
				foreach(SecurityContext context in contexts) {
					this.Add(context.GetType(), context);
				}
			}
		}
		public void Add(SecurityContext context) {
			base.Add(context.GetType(), context);
		}
		public CollectionPropertyContext CollectionPropertyContext {
			get { return (CollectionPropertyContext)this[typeof(CollectionPropertyContext)]; }
			set { this[typeof(CollectionPropertyContext)] = value; }
		}
		public TargetObjectContext TargetObjectContext {
			get { return (TargetObjectContext)this[typeof(TargetObjectContext)]; }
			set { this[typeof(TargetObjectContext)] = value; }
		}
		public TargetMemberContext TargetMemberContext {
			get { return (TargetMemberContext)this[typeof(TargetMemberContext)]; }
			set { this[typeof(TargetMemberContext)] = value; }
		}
		public SecurityContextList Clone() {
			SecurityContextList result = new SecurityContextList();
			foreach(SecurityContext context in this) {
				if(context != null) {
					SecurityContext clonedContext = context.Clone();
					if(clonedContext != null) {
						result.Add(clonedContext.GetType(), clonedContext);
					}
				}
			}
			return result;
		}
	}
	public class SecurityContext {
		public virtual SecurityContext Clone() {
			return null;
		}
	}
	public class TargetObjectContext : SecurityContext{
		private object targetObject;
		public TargetObjectContext(object targetObject) {
			this.targetObject = targetObject;
		}
		public object TargetObject {
			get { return targetObject; }
		}
		public override SecurityContext Clone() {
			return new TargetObjectContext(targetObject);
		}
	}
	public class TargetMemberContext : SecurityContext {
		private string targetMember;
		public TargetMemberContext(string targetMember) {
			this.targetMember = targetMember;
		}
		public string TargetMember {
			get { return targetMember; }
		}
		public override SecurityContext Clone() {
			return new TargetMemberContext(targetMember);
		}
	}
	public class CollectionPropertyContext : SecurityContext{
		private object masterObject;
		private string collectionPropertyName;
		private Type masterObjectType;
		public CollectionPropertyContext(Type masterObjectType, object masterObject, string collectionPropertyName) {
			this.masterObjectType = masterObjectType;
			this.masterObject = masterObject;
			this.collectionPropertyName = collectionPropertyName;
		}
		public Type MasterObjectType {
			get { return masterObjectType; }
		}		
		public object MasterObject {
			get { return masterObject; }
		}
		public string CollectionPropertyName {
			get { return collectionPropertyName; }
		}
		public override SecurityContext Clone() {
			return new CollectionPropertyContext(masterObjectType, masterObject, collectionPropertyName);
		}
	}
	[DomainComponent]
	[DebuggerDisplay("Count = {Count}"), DebuggerTypeProxy(typeof(ObjectAccessPermissionDebugView))]
	public class ObjectAccessPermission : PermissionBase {
		public class IncorrectTypeTemplateObject {
			private string typeName;
			public IncorrectTypeTemplateObject(string typeName) {
				this.typeName = typeName;
			}
			public string TypeName {
				get {
					return (string.IsNullOrEmpty(typeName) ? CaptionHelper.NullValueText : typeName);
				}
			}
		}
		public static string IncorrectTypeTemplate = "Incorrect Type({TypeName})";
		private PermissionState defaultPermissionState;
		private Type objectType;
		private ObjectAccessItemList accessItemList = new ObjectAccessItemList();
		[NonSerialized]
		private SecurityContextList contexts;
		private ObjectAccessModifier GetAccess(ObjectAccess objectAccess) {
			ParticularAccessItem item = accessItemList.FindAccessItem(objectType, objectAccess);
			if(item == null) {
				return ObjectAccessModifier.None;
			}
			else {
				return item.Modifier;
			}
		}
		private void SetAccess(ObjectAccess objectAccess, ObjectAccessModifier modifier) {
			ParticularAccessItem item = accessItemList.FindAccessItem(objectType, objectAccess);
			if(item == null) {
				if(modifier != ObjectAccessModifier.None) {
					accessItemList.Add(new ParticularAccessItem(objectType, objectAccess, modifier));
				}
			}
			else {
				if(modifier == ObjectAccessModifier.None) {
					accessItemList.Remove(item);
				}
				else {
					item.Modifier = modifier;
				}
			}
		}
		private void UpdateObjectType() {
			foreach(ParticularAccessItem accessItem in accessItemList) {
				if(objectType == null) {
					objectType = accessItem.ObjectType;
				}
				else if((objectType != null) && (accessItem.ObjectType != null) && (objectType != accessItem.ObjectType)) {
					objectType = null;
					break;
				}
			}
		}
		private ObjectAccessPermission(ObjectAccessItemList accessItemList) {
			this.accessItemList.Add(accessItemList);
			UpdateObjectType();
		}
		private void UpdateAccessItemList(ObjectAccess objectAccess, ObjectAccessModifier modifier) {
			foreach(ObjectAccess particularAccess in SplitFlagsToArray(objectAccess)) {
				ParticularAccessItem item = accessItemList.FindExactItem(objectType, particularAccess);
				if(item != null) {
					item.Modifier = modifier;
				}
				else {
					accessItemList.Add(new ParticularAccessItem(objectType, particularAccess, modifier));
				}
			}
		}
		[Browsable(false)]
		protected internal int Count {
			get { return accessItemList.Count; }
		}
		public ObjectAccessPermission()
			: base() {
		}
		public ObjectAccessPermission(System.Security.Permissions.PermissionState permissionState)
			: this() {
			defaultPermissionState = permissionState;
		}
		public ObjectAccessPermission(Type objectType, ObjectAccess access)
			: this(objectType, access, ((access == ObjectAccess.NoAccess) ? ObjectAccessModifier.Deny : ObjectAccessModifier.Allow)) {
		}
		public ObjectAccessPermission(Type objectType, ObjectAccess access, params SecurityContext[] securityContexts)
			: this(objectType, access, new SecurityContextList(securityContexts)) {
		}
		public ObjectAccessPermission(Type objectType, ObjectAccess access, SecurityContextList contexts)
			: this(objectType, access) {
			this.contexts = contexts;
		}
		public ObjectAccessPermission(Type objectType, ObjectAccess objectAccess, ObjectAccessModifier modifier) {
			this.objectType = objectType;
			if(objectType == null) {
				throw new ArgumentNullException("objectType");
			}
			if(objectAccess == ObjectAccess.NoAccess) {
				if(modifier != ObjectAccessModifier.Deny) {
					throw new ArgumentException(
						string.Format("Only the '{0}' modifier is valid the '{1}' access",
						ObjectAccessModifier.Deny, ObjectAccess.NoAccess));
				}
				objectAccess = ObjectAccess.AllAccess;
			}
			UpdateAccessItemList(objectAccess, modifier);
		}
		public override IPermission Copy() {
			return new ObjectAccessPermission(accessItemList);
		}
		public override IPermission Union(IPermission target) {
			ObjectAccessPermission objectAccessPermission = target as ObjectAccessPermission;
			if(objectAccessPermission == null) {
				throw new ArgumentException(
					string.Format("Incorrect permission is passed: '{0}' instead of '{1}'",
					target.GetType(), GetType()));
			}
			ObjectAccessItemList resultAccessList = new ObjectAccessItemList();
			resultAccessList.Add(accessItemList);
			resultAccessList.Add(objectAccessPermission.accessItemList);
			resultAccessList.PackItems();
			return new ObjectAccessPermission(resultAccessList);
		}
		public void Overwrite(Type objectType, ObjectAccess objectAccess, ObjectAccessModifier modifier) {
			this.objectType = objectType;
			UpdateAccessItemList(objectAccess, modifier);
		}
		public override bool IsSubsetOf(IPermission target) {
			if(base.IsSubsetOf(target)) {
				return ObjectAccessComparerBase.CurrentComparer.IsSubsetOf(this, (ObjectAccessPermission)target);
			}
			return false;
		}
		private SecurityElement sourceSecurityElement;
		private string sourceObjectTypeName;
		public override void FromXml(SecurityElement element) {
			accessItemList.Clear();
			if(element.Children != null) {
				foreach(SecurityElement childElement in element.Children) {
					String objectTypeName = childElement.Attributes["objectType"].ToString();
					Type type = null;
					if(objectTypeName.ToLower() == "devexpress.xpo.ixpsimpleobject") {
						type = PermissionTargetBusinessClassListConverter.PersistentBaseObjectType;
					}
					else {
						type = ReflectionHelper.FindType(objectTypeName);
					}
					if(type == null) {
						sourceSecurityElement = element.Copy();
						sourceObjectTypeName = childElement.Attributes["objectType"].ToString();
					}
					objectType = type;
					ObjectAccess access = ObjectAccess.NoAccess;
					bool isAccessRead = false;
					try {
						access = (ObjectAccess)Enum.Parse(typeof(ObjectAccess), childElement.Attributes["access"].ToString());
						isAccessRead = true;
					}
					catch(Exception) {
					}
					if(isAccessRead) {
						try {
							ObjectAccessModifier modifier = (ObjectAccessModifier)Enum.Parse(
								typeof(ObjectAccessModifier), childElement.Attributes["modifier"].ToString());
							accessItemList.Add(new ParticularAccessItem(type, access, modifier));
						}
						catch(Exception) {
						}
					}
				}
				string assemblyName = element.Attributes["assembly"].ToString();
				if(assemblyName.Contains("DevExpress.ExpressApp.v8.1") || assemblyName.Contains("DevExpress.ExpressApp.v7.")) {
					List<Type> grantNavigateToTypes = new List<Type>();
					foreach(ParticularAccessItem currentItem in accessItemList) {
						if(currentItem.Access == ObjectAccess.Read && currentItem.Modifier == ObjectAccessModifier.Allow) {
							grantNavigateToTypes.Add(currentItem.ObjectType);
						}
					}
					foreach(Type currentType in grantNavigateToTypes) {
						accessItemList.Add(new ParticularAccessItem(currentType, ObjectAccess.Navigate, ObjectAccessModifier.Allow));
					}
				}
			}
		}
		public override SecurityElement ToXml() {
			if(objectType == null && sourceSecurityElement != null) {
				return sourceSecurityElement;
			}
			if(IsEmpty) {
				throw new UserFriendlyException(UserVisibleExceptionId.ObjectAccessPermissionCannotBeEmpty);
			}
			SecurityElement result = base.ToXml();
			foreach(ParticularAccessItem item in accessItemList) {
				SecurityElement itemElement = new SecurityElement("ParticularAccessItem");
				itemElement.AddAttribute("access", item.Access.ToString());
				itemElement.AddAttribute("objectType", (item.ObjectType != null) ? item.ObjectType.ToString() : "");
				itemElement.AddAttribute("modifier", item.Modifier.ToString());
				result.AddChild(itemElement);
			}
			return result;
		}
		public override IPermission Intersect(IPermission target) {
			throw new InvalidOperationException("Not implemented. Use the 'IsSubsetOf' method instead.");
		}
		public override string ToString() {
			SortedList<String, Type> objectTypes = new SortedList<String, Type>();
			foreach(ParticularAccessItem item in accessItemList) {
				string typeName = (item.ObjectType == null) ? "" : item.ObjectType.FullName;
				if(!objectTypes.ContainsKey(typeName)) {
					objectTypes.Add(typeName, item.ObjectType);
				}
			}
			EnumDescriptor enumDescriptor = new EnumDescriptor(typeof(ObjectAccessModifier));
			EnumDescriptor objectAccessEnumDescriptor = new EnumDescriptor(typeof(ObjectAccess));
			String result = "";
			ObjectAccess[] objectAccesses = new ObjectAccess[] { ObjectAccess.Create, ObjectAccess.Delete, ObjectAccess.Read, ObjectAccess.Write, ObjectAccess.Navigate };
			foreach(Type objectType in objectTypes.Values) {
				if(objectType != null) {
					result += CaptionHelper.GetClassCaption(objectType.FullName);
				}
				else {
					IncorrectTypeTemplateObject incorrectTypeTemplateObject;
					if(!string.IsNullOrEmpty(sourceObjectTypeName)) {
						incorrectTypeTemplateObject = new IncorrectTypeTemplateObject(sourceObjectTypeName);
					}
					else {
						incorrectTypeTemplateObject = new IncorrectTypeTemplateObject(CaptionHelper.NullValueText);
					}
					result += ObjectFormatter.Format(IncorrectTypeTemplate, incorrectTypeTemplateObject);
				}
				result += ":";
				foreach(ObjectAccess objectAccess in objectAccesses) {
					foreach(ParticularAccessItem item in accessItemList) {
						if((item.ObjectType == objectType) && (item.Access == objectAccess)) {
							string str = " " + objectAccessEnumDescriptor.GetCaption(objectAccess) + "({0})";
							if(item.Modifier != ObjectAccessModifier.None) {
								result += String.Format(str, enumDescriptor.GetCaption(item.Modifier));
							}
						}
					}
				}
				result += "; ";
			}
			return result.TrimEnd(' ');
		}
		[TypeConverter(typeof(PermissionTargetBusinessClassListConverter))]
		public Type ObjectType {
			get { return objectType; }
			set {
				for(int i = accessItemList.Count - 1; i >= 0; i--) {
					ParticularAccessItem accessItem = accessItemList[i];
					if(accessItemList[i].ObjectType == objectType) {
						ParticularAccessItem newAccessItem = new ParticularAccessItem(value, accessItem.Access, accessItem.Modifier);
						accessItemList.Remove(accessItem);
						accessItemList.Add(newAccessItem);
					}
				}
				objectType = value;
			}
		}
		public ObjectAccessModifier CreateAccess {
			get { return GetAccess(ObjectAccess.Create); }
			set { SetAccess(ObjectAccess.Create, value); }
		}
		public ObjectAccessModifier ReadAccess {
			get { return GetAccess(ObjectAccess.Read); }
			set { SetAccess(ObjectAccess.Read, value); }
		}
		public ObjectAccessModifier WriteAccess {
			get { return GetAccess(ObjectAccess.Write); }
			set { SetAccess(ObjectAccess.Write, value); }
		}
		public ObjectAccessModifier DeleteAccess {
			get { return GetAccess(ObjectAccess.Delete); }
			set { SetAccess(ObjectAccess.Delete, value); }
		}
		public ObjectAccessModifier NavigateAccess {
			get { return GetAccess(ObjectAccess.Navigate); }
			set { SetAccess(ObjectAccess.Navigate, value); }
		}
		public bool ContainsAllowAccess(ObjectAccess accessFlags) {
			foreach(ObjectAccess access in SplitFlagsToArray(accessFlags)) {
				if(GetAccess(access) == ObjectAccessModifier.Allow) {
					return true;
				}
			}
			return false;
		}
		[Browsable(false)]
		public bool IsEmpty {
			get { return (objectType == null) || accessItemList.IsEmpty; }
		}
		public static ObjectAccessPermission operator +(ObjectAccessPermission left, ObjectAccessPermission right) {
			return (ObjectAccessPermission)left.Union(right);
		}
		public static bool HasAccessTo(Type type, string propertyName, SecurityContextList securityContexts) {
			return ObjectAccessComparerBase.CurrentComparer.IsMemberReadGranted(type, propertyName, securityContexts);
		}
		[Browsable(false)]
		public SecurityContextList Contexts {
			get {
				if(this.contexts == null) {
					this.contexts = new SecurityContextList();
				}
				return contexts;
			}
		}
		[Browsable(false)]
		public ObjectAccessItemList AccessItemList {
			get { return accessItemList; }
		}
		public static ObjectAccess[] SplitFlagsToArray(ObjectAccess objectAccess) {
			List<ObjectAccess> result = new List<ObjectAccess>();
			if((objectAccess & ObjectAccess.Read) == ObjectAccess.Read) {
				result.Add(ObjectAccess.Read);
			}
			if((objectAccess & ObjectAccess.Write) == ObjectAccess.Write) {
				result.Add(ObjectAccess.Write);
			}
			if((objectAccess & ObjectAccess.Create) == ObjectAccess.Create) {
				result.Add(ObjectAccess.Create);
			}
			if((objectAccess & ObjectAccess.Delete) == ObjectAccess.Delete) {
				result.Add(ObjectAccess.Delete);
			}
			if((objectAccess & ObjectAccess.Navigate) == ObjectAccess.Navigate) {
				result.Add(ObjectAccess.Navigate);
			}
			return result.ToArray();
		}
	}
}
