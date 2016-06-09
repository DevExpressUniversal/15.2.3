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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.Persistent.Base {
	public interface IXpoCloneable {
		IXPSimpleObject CloneTo(Type targetType);
	}
	public class CloneObjectReferenceEntry {
		private IXPSimpleObject clonedObject;
		private XPMemberInfo memberInfo;
		private IXPSimpleObject referencedObject;
		public CloneObjectReferenceEntry(IXPSimpleObject clonedObject, XPMemberInfo memberInfo, IXPSimpleObject referencedObject) {
			this.clonedObject = clonedObject;
			this.memberInfo = memberInfo;
			this.referencedObject = referencedObject;
		}
		public IXPSimpleObject ClonedObject {
			get { return clonedObject; }
		}
		public XPMemberInfo MemberInfo {
			get { return memberInfo; }
		}
		public IXPSimpleObject ReferencedObject {
			get { return referencedObject; }
		}
	}
	public class Cloner {
		private DevExpress.ExpressApp.DC.ITypesInfo typesInfo;
		private Dictionary<IXPSimpleObject, IXPSimpleObject> sourceToCloneMap = null;
		private List<CloneObjectReferenceEntry> referencesList = null;
		public Cloner() {
			typesInfo = XafTypesInfo.Instance;
		}
#if DebugTest
		public Cloner(ITypesInfo typesInfo) {
			this.typesInfo = typesInfo;
		}
#endif
		protected Dictionary<IXPSimpleObject, IXPSimpleObject> SourceToCloneMap {
			get { return sourceToCloneMap; }
		}
		protected List<CloneObjectReferenceEntry> ReferencesList {
			get { return referencesList; }
		}
		private void RegisterReferencedObject(IXPSimpleObject clonedObject, XPMemberInfo memberInfo, IXPSimpleObject referencedObject) {
			if(clonedObject == null || referencedObject == null) {
				return;
			}
			referencesList.Add(new CloneObjectReferenceEntry(clonedObject, memberInfo, referencedObject));
		}
		private void UpdateReferencesToClonedObjects() {
			foreach(CloneObjectReferenceEntry referenceEntry in referencesList) {
				IXPSimpleObject clonedReferencedObject;
				if(sourceToCloneMap.TryGetValue(referenceEntry.ReferencedObject, out clonedReferencedObject)) {
					if(referenceEntry.ReferencedObject != clonedReferencedObject) {
						if(IsCollection(referenceEntry.MemberInfo)) {
							IList collection = ((IList)referenceEntry.MemberInfo.GetValue(referenceEntry.ClonedObject));
							collection.Remove(referenceEntry.ReferencedObject);
							collection.Add(clonedReferencedObject);
						}
						else {
							referenceEntry.MemberInfo.SetValue(referenceEntry.ClonedObject, clonedReferencedObject);
						}
					}
				}
			}
		}
		protected virtual void EndRecurrentClone() {
			sourceToCloneMap.Clear();
			sourceToCloneMap = null;
			referencesList.Clear();
			referencesList = null;
		}
		protected virtual bool StartRecurrentClone() {
			if(sourceToCloneMap == null) {
				sourceToCloneMap = new Dictionary<IXPSimpleObject, IXPSimpleObject>();
				referencesList = new List<CloneObjectReferenceEntry>();
				return true;
			}
			return false;
		}
		protected virtual void CheckValues(IXPSimpleObject sourceObject, Type targetType) {
			if(sourceObject == null) {
				throw new Exception("sourceObject refrence is null");
			}
			if(targetType == null) {
				throw new Exception("targetType isn't defined");
			}
			if(!typeof(IXPSimpleObject).IsAssignableFrom(targetType)) {
				throw new Exception("Can't clone non IXPSimpleObject objects");
			}
		}
		protected virtual IXPSimpleObject InternalCloneTo(IXPSimpleObject sourceObject, Type targetType) {
			IXPSimpleObject targetObject = null;
			if(sourceObject != null) {
				if(typeof(IXpoCloneable).IsAssignableFrom(sourceObject.GetType())) {
					targetObject = ((IXpoCloneable)sourceObject).CloneTo(targetType);
					sourceToCloneMap[sourceObject] = targetObject;
				}
				else {
					targetObject = DefaultCloneTo(sourceObject, targetType);
				}
			}
			return targetObject;
		}
		protected virtual string FormatNotClonedMemberValues(string currentValue, IXPSimpleObject sourceObject, List<XPMemberInfo> notClonedMembersInfo) {
			List<string> notClonedMemberValues = new List<string>();
			if(currentValue != null) {
				notClonedMemberValues.Add(currentValue);
			}
			foreach(XPMemberInfo mi in notClonedMembersInfo) {
				object val = GetSourceMemberValue(mi, sourceObject);
				notClonedMemberValues.Add(mi.Name + ": '" + (val == null ? "N/A" : val.ToString()) + "'");
			}
			return string.Join("\r\n", notClonedMemberValues.ToArray());
		}
		protected virtual XPMemberInfo GetNotClonedInfoMemberName(XPClassInfo targetType) {
			ITypeInfo typeInfo = typesInfo.FindTypeInfo(targetType.ClassType);
			if(typeInfo != null) {
				IEnumerable<NotClonedInfoAttribute> notClonedInfoAttributes = typeInfo.FindAttributes<NotClonedInfoAttribute>(true);
				if(notClonedInfoAttributes.Count() == 1) {
					XPMemberInfo mi = targetType.FindMember(notClonedInfoAttributes.First().MemberName);
					if(mi != null && mi.MemberType == typeof(string)) {
						return mi;
					}
				}
			}
			return null;
		}
		protected virtual bool ContainsMember(XPClassInfo classInfo, XPMemberInfo memberInfo) {
			foreach(XPMemberInfo mi in classInfo.Members) {
				if(mi.Name == memberInfo.Name && mi.MemberType == memberInfo.MemberType && mi.IsReadOnly == memberInfo.IsReadOnly) {
					return true;
				}
			}
			return false;
		}
		protected virtual void CheckMemberInObjects(XPMemberInfo memberInfo, IXPSimpleObject sourceObject, IXPSimpleObject targetObject) {
			if(sourceObject == null || targetObject == null) {
				throw new Exception("One or more arguments are null.");
			}
			if(!ContainsMember(sourceObject.ClassInfo, memberInfo)) {
				throw new Exception(string.Format("Source object doesn't contain member: {0}", memberInfo.Name));
			}
			if(!IsMemberCloneable(memberInfo)) {
				throw new Exception(string.Format(@"Member ""{0}"" can't be cloned.", memberInfo.Name));
			}
			if(!ContainsMember(targetObject.ClassInfo, memberInfo)) {
				throw new Exception(string.Format("Target object doesn't contain member: {0}", memberInfo.Name));
			}
		}
		protected XPMemberInfo FindAliasMemberInfo(XPMemberInfo sourceMemberInfo, bool findFirstMember) {
			Guard.ArgumentNotNull(sourceMemberInfo, "sourceMemberInfo");
			XPMemberInfo result = null;
			if(sourceMemberInfo.IsAliased) {
				PersistentAliasAttribute attr = (PersistentAliasAttribute)sourceMemberInfo.GetAttributeInfo(typeof(PersistentAliasAttribute));
				if(attr != null) {
					string aliasFullName = attr.AliasExpression;
					if(aliasFullName != null) {
						aliasFullName = aliasFullName.TrimStart('[').TrimEnd(']');
						string[] aliasMemberPath = aliasFullName.Split('.');
						XPClassInfo currentClass = sourceMemberInfo.Owner;
						XPMemberInfo currentMember = null;
						foreach(string memberName in aliasMemberPath) {
							if(currentClass == null) {
								currentMember = null;
								break;
							}
							currentMember = currentClass.FindMember(memberName);
							if(findFirstMember || currentMember == null) {
								break;
							}
							currentClass = currentMember.ReferenceType;
						}
						result = currentMember;
					}
				}
			}
			return result;
		}
		public virtual List<XPMemberInfo> GetAllPublicMembers(XPClassInfo classInfo) {
			List<XPMemberInfo> result = new List<XPMemberInfo>();
			XPClassInfo currentClassInfo = classInfo;
			while(currentClassInfo.ClassType.Assembly != typeof(XPObject).Assembly) {
				foreach(XPMemberInfo memberInfo in currentClassInfo.OwnMembers) {
					if(memberInfo.IsPublic) {
						result.Add(memberInfo);
					}
				}
				currentClassInfo = currentClassInfo.BaseClass;
			}
			return result;
		}
		public virtual bool IsMemberCloneable(XPMemberInfo memberInfo) {
			if(memberInfo.IsAssociation && memberInfo.GetAssociatedMember().MemberType.IsGenericType && memberInfo.GetAssociatedMember().MemberType.GetGenericTypeDefinition() == typeof(DevExpress.Xpo.Helpers.IPersistentInterfaceData<>)) {
				return false;
			}
			return IsMemberCloneableCore(memberInfo);
		}
		private bool IsMemberCloneableCore(XPMemberInfo memberInfo) {
			if(memberInfo == null) {
				return false;
			}
			if(HasNonCloneableAttribute(memberInfo)) { 
				return false;
			}
			bool result = memberInfo.IsPublic && !memberInfo.IsKey && (!memberInfo.IsReadOnly || memberInfo.IsAssociationList || (memberInfo.IsNonAssociationList && memberInfo.IsManyToManyAlias));
			if(memberInfo.IsAssociationList) {
				result &= memberInfo.IsAggregated || memberInfo.IsManyToMany;
			}
			if(!result && memberInfo.IsAliased) {
				XPMemberInfo aliasMemberInfo = FindAliasMemberInfo(memberInfo, false);
				result = aliasMemberInfo != null && (IsCollection(aliasMemberInfo) || !memberInfo.IsReadOnly) && IsMemberCloneableCore(aliasMemberInfo);
			}
			if(!result && !memberInfo.IsPublic) {
				result = memberInfo.IsPersistent && !memberInfo.IsKey;
			}
			return result;
		}
		private Type FindMemberInterface(XPMemberInfo memberInfo) {
			foreach(Type interfaceType in memberInfo.Owner.ClassType.GetInterfaces()) {
				PropertyInfo propertyInfo = interfaceType.GetProperty(memberInfo.Name);
				if(propertyInfo != null && propertyInfo.PropertyType == memberInfo.MemberType) {
					return interfaceType;
				}
			}
			return null;
		}
		private bool IsCloneable(XPMemberInfo sourceMemberInfo, XPMemberInfo targetMemberInfo) {
			if(sourceMemberInfo.Equals(targetMemberInfo)) {
				return !HasNonCloneableAttribute(targetMemberInfo);
			}
			else {
				Type targetInterfaceType = FindMemberInterface(targetMemberInfo);
				Type sourceInterfaceType = FindMemberInterface(sourceMemberInfo);
				if(targetInterfaceType != null && targetInterfaceType == sourceInterfaceType) {
					return !HasNonCloneableAttribute(targetMemberInfo) && !HasNonCloneableAttribute(sourceMemberInfo);
				}
				return false;
			}
		}
		public virtual List<XPMemberInfo> GetClonedMembers(List<XPMemberInfo> sourceMembersInfo, List<XPMemberInfo> targetMembersInfo) {
			Dictionary<string, XPMemberInfo> sourceMembersDictionary = new Dictionary<string, XPMemberInfo>();
			foreach(XPMemberInfo sourceMemberInfo in sourceMembersInfo) {
				sourceMembersDictionary[sourceMemberInfo.Name] = sourceMemberInfo;
			}
			List<XPMemberInfo> result = new List<XPMemberInfo>();
			foreach(XPMemberInfo memberInfo in targetMembersInfo) {
				XPMemberInfo sourceMemberInfo = null;
				if(sourceMembersDictionary.TryGetValue(memberInfo.Name, out sourceMemberInfo)) {
					if(IsCloneable(sourceMemberInfo, memberInfo)) {
						if(IsMemberCloneable(memberInfo) && !result.Contains(memberInfo)) {
							result.Add(memberInfo);
						}
						else if(!IsCollection(memberInfo) && memberInfo.IsReadOnly) {
							XPMemberInfo firstAliasMemberInfo = FindAliasMemberInfo(memberInfo, true);
							XPMemberInfo aliasMemberInfo = FindAliasMemberInfo(memberInfo, false);
							if(firstAliasMemberInfo != null && aliasMemberInfo != null && IsMemberCloneable(firstAliasMemberInfo) && IsMemberCloneable(aliasMemberInfo) && !result.Contains(memberInfo)) {
								result.Add(firstAliasMemberInfo);
							}
						}
					}
				}
			}
			return result;
		}
		public virtual List<XPMemberInfo> GetNotClonedMembers(List<XPMemberInfo> sourceMembersInfo, List<XPMemberInfo> targetMembersInfo) {
			List<XPMemberInfo> result = new List<XPMemberInfo>();
			foreach(XPMemberInfo memberInfo in sourceMembersInfo) {
				if(!targetMembersInfo.Contains(memberInfo) && IsMemberCloneable(memberInfo) && memberInfo.IsPublic) {
					result.Add(memberInfo);
				}
			}
			return result;
		}
		public virtual IXPSimpleObject CreateObject(Session session, Type type) {
			return (IXPSimpleObject)System.Activator.CreateInstance(type, session);
		}
		private object GetSourceMemberValue(XPMemberInfo targetMemberInfo, IXPSimpleObject obj) {
			foreach(XPMemberInfo realMemberInfo in obj.ClassInfo.Members) {
				if(targetMemberInfo.Name == realMemberInfo.Name && targetMemberInfo.MemberType == realMemberInfo.MemberType) {
					return realMemberInfo.GetValue(obj);
				}
			}
			return null;
		}
		public virtual void CopyMemberValue(XPMemberInfo memberInfo, IXPSimpleObject sourceObject, IXPSimpleObject targetObject) {
			CheckMemberInObjects(memberInfo, sourceObject, targetObject);
			object memberValue = GetSourceMemberValue(memberInfo, sourceObject);
			if(memberValue != null && memberValue is IXPSimpleObject) {
				ClearTargetObjectProperty(memberInfo, targetObject, memberInfo.IsAggregated);
				IXPSimpleObject clonedObjectReference = CloneReferenceMemberValue(targetObject, memberInfo, (IXPSimpleObject)memberValue, memberValue.GetType(), memberInfo.IsAggregated);
				memberInfo.SetValue(targetObject, clonedObjectReference);
			}
			else {
				memberInfo.SetValue(targetObject, memberValue);
			}
		}
		private IXPSimpleObject CloneReferenceMemberValue(IXPSimpleObject targetObject, XPMemberInfo memberInfo, IXPSimpleObject sourceObject, Type type, bool createNewObject) {
			if(sourceObject == null) {
				return null;
			}
			IXPSimpleObject result = null;
			if(sourceToCloneMap.ContainsKey(sourceObject)) {
				result = sourceToCloneMap[sourceObject];
			}
			else {
				if(createNewObject) {
					result = InternalCloneTo(sourceObject, type);
				}
				else {
					result = sourceObject;
				}
				RegisterReferencedObject(targetObject, memberInfo, result);
			}
			return result;
		}
		private void ClearTargetObjectProperty(XPMemberInfo memberInfo, IXPSimpleObject targetObject, bool isAggregated) {
			if(IsCollection(memberInfo)) {
				IList collection = ((IList)memberInfo.GetValue(targetObject));
				Object[] objects = new object[collection.Count];
				collection.CopyTo(objects, 0);
				if(isAggregated) {
					foreach(Object obj in objects) {
						if(obj is IXPSimpleObject && !sourceToCloneMap.ContainsValue((IXPSimpleObject)obj)) {
							((IXPSimpleObject)obj).Session.Delete(obj);
						}
					}
				}
				collection.Clear();
			}
			else if(isAggregated) {
				object targetMemberObject = memberInfo.GetValue(targetObject);
				if(targetMemberObject != null && targetMemberObject is IXPSimpleObject) {
					((IXPSimpleObject)targetMemberObject).Session.Delete(targetMemberObject);
				}
			}
		}
		public virtual void CopyCollection(XPMemberInfo memberInfo, IXPSimpleObject sourceObject, IXPSimpleObject targetObject, bool aggregated) {
			if(!IsCollection(memberInfo)) {
				throw new Exception(String.Format("The member '{0}' is not a collection.", memberInfo.Name));
			}
			bool isRecursionTop = StartRecurrentClone();
			try {
				ClearTargetObjectProperty(memberInfo, targetObject, aggregated);
				IList collection = ((IList)GetSourceMemberValue(memberInfo, sourceObject));
				Object[] objects = new object[collection.Count];
				collection.CopyTo(objects, 0);
				foreach(Object obj in objects) {
					if(obj is IXPSimpleObject && !sourceToCloneMap.ContainsValue((IXPSimpleObject)obj)) {
						IXPSimpleObject clonedObjectReference = CloneReferenceMemberValue(targetObject, memberInfo, (IXPSimpleObject)obj, obj.GetType(), aggregated);
						((IList)memberInfo.GetValue(targetObject)).Add(clonedObjectReference);
					}
				}
				if(isRecursionTop) {
					UpdateReferencesToClonedObjects();
				}
			}
			finally {
				if(isRecursionTop) {
					EndRecurrentClone();
				}
			}
		}
		public object CloneTo(object sourceObject, Type targetType) {
			IXPSimpleObject _sourceObject = sourceObject as IXPSimpleObject;
			if(_sourceObject == null) {
				throw new InvalidOperationException("IXPSimpleObject");
			}
			CheckValues(_sourceObject, targetType);
			bool isRecursionTop = StartRecurrentClone();
			try {
				IXPSimpleObject result = InternalCloneTo(_sourceObject, targetType);
				if(isRecursionTop) {
					UpdateReferencesToClonedObjects();
				}
				return result;
			}
			finally {
				if(isRecursionTop) {
					EndRecurrentClone();
				}
			}
		}
		public IXPSimpleObject DefaultCloneTo(IXPSimpleObject sourceObject, Type targetType) {
			bool isRecursionTop = StartRecurrentClone();
			IXPSimpleObject targetObject = null;
			try {
				CheckValues(sourceObject, targetType);
				targetObject = CreateObject(sourceObject.Session, targetType);
				sourceToCloneMap[sourceObject] = targetObject;
				List<XPMemberInfo> sourceMembersInfo = GetAllPublicMembers(sourceObject.ClassInfo);
				List<XPMemberInfo> targetMembersInfo = GetAllPublicMembers(targetObject.ClassInfo);
				List<XPMemberInfo> clonedMembers = GetClonedMembers(sourceMembersInfo, targetMembersInfo);
				if(targetObject is ISupportInitialize) {
					((ISupportInitialize)targetObject).BeginInit();
				}
				foreach(XPMemberInfo mi in clonedMembers) {
					if(mi.IsAggregated) {
						CloneMemberValue(sourceObject, targetObject, mi);
					}
				}
				foreach(XPMemberInfo mi in clonedMembers) {
					if(!mi.IsAggregated) {
						CloneMemberValue(sourceObject, targetObject, mi);
					}
				}
				XPMemberInfo notClonedInfoMemberInfo = GetNotClonedInfoMemberName(targetObject.ClassInfo);
				if(notClonedInfoMemberInfo != null) {
					string notClonedMemberValues = FormatNotClonedMemberValues(
						(string)notClonedInfoMemberInfo.GetValue(targetObject), sourceObject,
						GetNotClonedMembers(sourceMembersInfo, targetMembersInfo)
					);
					notClonedInfoMemberInfo.SetValue(targetObject, notClonedMemberValues);
				}
				if(isRecursionTop) {
					UpdateReferencesToClonedObjects();
				}
				return targetObject;
			}
			finally {
				if(isRecursionTop) {
					EndRecurrentClone();
				}
				if(targetObject!= null && targetObject is ISupportInitialize) {
					((ISupportInitialize)targetObject).EndInit();
				}
			}
		}
		private void CloneMemberValue(IXPSimpleObject sourceObject, IXPSimpleObject targetObject, XPMemberInfo mi) {
			if(IsCollection(mi)) {
				CheckMemberInObjects(mi, sourceObject, targetObject);
				if(GetSourceMemberValue(mi, sourceObject) != null) {
					CopyCollection(mi, sourceObject, targetObject, mi.IsAggregated);
				}
			}
			else {
				CopyMemberValue(mi, sourceObject, targetObject);
			}
		}
		private bool IsCollection(XPMemberInfo memberInfo) {
			return memberInfo.IsAssociationList || memberInfo.IsNonAssociationList;
		}
		private bool HasNonCloneableAttribute(XPMemberInfo xpMemberInfo) {
			ITypeInfo typeInfo = typesInfo.FindTypeInfo(xpMemberInfo.Owner.ClassType);
			if(typeInfo != null) {
				IMemberInfo memberInfo = typeInfo.FindMember(xpMemberInfo.Name);
				if(memberInfo != null) {
					return memberInfo.FindAttribute<NonCloneableAttribute>() != null;
				}
				else {
					return xpMemberInfo.FindAttributeInfo(typeof(NonCloneableAttribute)) != null;
				}
			}
			return false;
		}
	}
}
