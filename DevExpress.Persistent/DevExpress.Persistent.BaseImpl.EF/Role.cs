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
using System.ComponentModel;
using System.Data.Entity.Core.Objects.DataClasses;
using System.ComponentModel.DataAnnotations;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp;
namespace DevExpress.Persistent.BaseImpl.EF {
	public class Role : ISecurityRole, IOperationPermissionProvider, ISecuritySystemRole {
		private const bool AutoAssociationPermissionsDefaultValue = true;
		public Role() {
			Users = new List<User>();
			TypePermissions = new List<TypePermissionObject>();
			ChildRoles = new List<Role>();
			ParentRoles = new List<Role>();
		}
		static Role() {
			AutoAssociationPermissions = AutoAssociationPermissionsDefaultValue;
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Name { get; set; }
		public Boolean IsAdministrative { get; set; }
		public Boolean CanEditModel { get; set; }
		public virtual IList<User> Users { get; set; }
		[Aggregated]
		public virtual IList<TypePermissionObject> TypePermissions { get; set; }
		[Aggregated]
		public virtual IList<Role> ChildRoles { get; set; }
		public virtual IList<Role> ParentRoles { get; set; }
		private enum AssociationPermissionsCheckingMode { SetTypeForAssociation, SetTypeForAggregated, SetMemberForAssociation, SetMemberForAggregated }
		String ISecurityRole.Name {
			get { return Name; }
		}
		IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren() {
			List<IOperationPermissionProvider> result = new List<IOperationPermissionProvider>();
			result.AddRange(new EnumerableConverter<IOperationPermissionProvider, Role>(ChildRoles));
			return result;
		}
		IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions() {
			List<IOperationPermission> result = new List<IOperationPermission>();
			foreach(TypePermissionObject persistentPermission in TypePermissions) {
				result.AddRange(persistentPermission.GetPermissions());
			}
			if(IsAdministrative) {
				result.Add(new IsAdministratorPermission());
			}
			if(CanEditModel) {
				result.Add(new ModelOperationPermission());
			}
			return result;
		}
		private TypePermissionObject GetAssociatedTypePermissionObject(IMemberInfo member) {
			IMemberInfo associationMemberInfo = member.AssociatedMemberInfo;
			Type associatedObjectType = associationMemberInfo.Owner.Type;
			IEnumerable<TypePermissionObject> associatedTypePermissionObjects = TypePermissions.Where(t => t.TargetType == associatedObjectType);
			if(associatedTypePermissionObjects.Count() > 1) {
				return null;
			}
			TypePermissionObject associatedTypePermissionObject = associatedTypePermissionObjects.FirstOrDefault();
			if(associatedTypePermissionObject == null) {
				associatedTypePermissionObject = new TypePermissionObject();
				associatedTypePermissionObject.TargetType = associatedObjectType;
				associatedTypePermissionObject.Role = this;
				associatedTypePermissionObject.IsCalledSetAssociationPermissions = true;
			}
			return associatedTypePermissionObject;
		}
		private SecuritySystemMemberPermissionsObject GetAssociatedMemberPermissionsObject(TypePermissionObject associatedTypePermissionObject, IMemberInfo member) {
			if(associatedTypePermissionObject == null) {
				return null;
			}
			IEnumerable<SecuritySystemMemberPermissionsObject> associatedMemberPermissionsObjects = associatedTypePermissionObject.MemberPermissions.Where(t => t.Members.Contains(member.AssociatedMemberInfo.Name));
			if(associatedMemberPermissionsObjects.Count() > 1) {
				return null;
			}
			SecuritySystemMemberPermissionsObject associatedMemberPermissionsObject = associatedMemberPermissionsObjects.FirstOrDefault();
			if(associatedMemberPermissionsObject == null) {
				associatedMemberPermissionsObject = new SecuritySystemMemberPermissionsObject();
				associatedMemberPermissionsObject.Members = member.AssociatedMemberInfo.Name;
				((IOwnerInitializer)associatedMemberPermissionsObject).SetMasterObject(associatedTypePermissionObject);
				associatedMemberPermissionsObject.Owner = associatedTypePermissionObject;
				associatedTypePermissionObject.MemberPermissions.Add(associatedMemberPermissionsObject);
				associatedMemberPermissionsObjects = new List<SecuritySystemMemberPermissionsObject>() { associatedMemberPermissionsObject };
			}
			return associatedMemberPermissionsObject;
		}
		private IEnumerable<IMemberInfo> GetMemberInfos(ITypeInfo typeInfo, SecuritySystemMemberPermissionsObject memberPermissionsObject) {
			List<IMemberInfo> memberInfos = new List<IMemberInfo>();
			if(memberPermissionsObject != null && typeInfo != null) {
				string[] memberNames = memberPermissionsObject.Members.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries);
				foreach(string memberName in memberNames) {
					IMemberInfo memberInfo = typeInfo.Members.Where(t => t.Name == memberName.Trim()).FirstOrDefault();
					if(memberInfo != null) {
						memberInfos.Add(memberInfo);
					}
				}
			}
			return memberInfos;
		}
		private bool CheckTypePermissionObjectCompatibility(TypePermissionObject typePermissionObject, out ITypeInfo typeInfo) {
			if(XafTypesInfo.Instance == null) {
				typeInfo = null;
				return false;
			}
			if(typePermissionObject == null) {
				typeInfo = null;
				return false;
			}
			typeInfo = XafTypesInfo.Instance.FindTypeInfo(typePermissionObject.TargetType);
			if(typeInfo == null) {
				return false;
			}
			if(TypePermissions.Count(p => p.TargetType == typePermissionObject.TargetType) > 1) {
				return false;
			}
			return true;
		}
		private bool CheckConditionsForSetAssociationPermissions(TypePermissionObject typePermissionObject, TypePermissionObject associatedTypePermissionObject, SecuritySystemMemberPermissionsObject memberPermissionObject, SecuritySystemMemberPermissionsObject associatedMemberPermissionObject, AssociationPermissionsCheckingMode mode) {
			if(associatedTypePermissionObject == null || typePermissionObject == null) {
				return false;
			}
			switch(mode) {
				case AssociationPermissionsCheckingMode.SetTypeForAssociation:
					if(associatedMemberPermissionObject == null) {
						return false;
					}
					if(!String.IsNullOrEmpty(associatedMemberPermissionObject.Criteria)) {
						return false;
					}
					if(associatedMemberPermissionObject.Members.Contains(";")) {
						return false;
					}
					break;
				case AssociationPermissionsCheckingMode.SetMemberForAssociation:
					if(associatedMemberPermissionObject == null) {
						return false;
					}
					if(typePermissionObject.MemberPermissions.Count(p => p.Members == memberPermissionObject.Members) > 1) {
						return false;
					}
					if(!String.IsNullOrEmpty(memberPermissionObject.Criteria)) {
						return false;
					}
					if(!String.IsNullOrEmpty(associatedMemberPermissionObject.Criteria)) {
						return false;
					}
					if(associatedMemberPermissionObject.Members.Contains(";")) {
						return false;
					}
					break;
				case AssociationPermissionsCheckingMode.SetMemberForAggregated:
					if(typePermissionObject.MemberPermissions.Count(p => p.Members == memberPermissionObject.Members) > 1) {
						return false;
					}
					break;
			}
			return true;
		}
		public void SetAssociationTypePermissions(TypePermissionObject typePermissionObject, string operations) {
			List<TypePermissionObject> associatedTypePermissionObjects = new List<TypePermissionObject>();
			TypePermissionObject associatedTypePermissionObject = null;
			try {
				ITypeInfo typeInfo;
				if(!CheckTypePermissionObjectCompatibility(typePermissionObject, out typeInfo)) {
					return;
				}
				IEnumerable<IMemberInfo> memberInfos = typeInfo.Members;
				foreach(IMemberInfo member in memberInfos) {
					if(member.IsAssociation) {
						associatedTypePermissionObject = GetAssociatedTypePermissionObject(member);
						if(member.IsAggregated) {
							if(!member.MemberType.IsGenericType || !member.MemberType.GetGenericTypeDefinition().IsEquivalentTo(typeof(IList<>))) {
								continue;
							}
							if(!CheckConditionsForSetAssociationPermissions(typePermissionObject, associatedTypePermissionObject, null, null, AssociationPermissionsCheckingMode.SetTypeForAggregated)) {
								continue;
							}
							associatedTypePermissionObjects.Add(associatedTypePermissionObject);
							associatedTypePermissionObject.IsCalledSetAssociationPermissions = true;
							if(operations.Contains("Read")) {
								associatedTypePermissionObject.AllowRead = typePermissionObject.AllowRead;
							}
							if(operations.Contains("Write")) {
								associatedTypePermissionObject.AllowWrite = typePermissionObject.AllowWrite;
								associatedTypePermissionObject.AllowCreate = typePermissionObject.AllowWrite;
								associatedTypePermissionObject.AllowDelete = typePermissionObject.AllowWrite;
							}
						}
						else {
							SecuritySystemMemberPermissionsObject associatedMemberPermissionsObject = GetAssociatedMemberPermissionsObject(associatedTypePermissionObject, member);
							if(!CheckConditionsForSetAssociationPermissions(typePermissionObject, associatedTypePermissionObject, null, associatedMemberPermissionsObject, AssociationPermissionsCheckingMode.SetTypeForAssociation)) {
								continue;
							}
							associatedTypePermissionObjects.Add(associatedTypePermissionObject);
							associatedTypePermissionObject.IsCalledSetAssociationPermissions = true;
							if(operations.Contains("Read")) {
								associatedMemberPermissionsObject.AllowRead = typePermissionObject.AllowRead;
							}
							if(operations.Contains("Write")) {
								associatedMemberPermissionsObject.AllowWrite = typePermissionObject.AllowWrite;
							}
						}
					}
				}
			}
			finally {
				typePermissionObject.IsCalledSetAssociationPermissions = false;
				foreach(TypePermissionObject associatedTypePermissionObjectItem in associatedTypePermissionObjects) {
					if(!TypePermissions.Contains(associatedTypePermissionObjectItem)) {
						TypePermissions.Add(associatedTypePermissionObjectItem); 
					}
					associatedTypePermissionObjectItem.IsCalledSetAssociationPermissions = false;
				}
			}
		}
		public void SetAssociationMemberPermissions(TypePermissionObject typePermissionObject, SecuritySystemMemberPermissionsObject memberPermissionObject, string operations) {
			List<TypePermissionObject> associatedTypePermissionObjects = new List<TypePermissionObject>();
			TypePermissionObject associatedTypePermissionObject = null;
			try {
				ITypeInfo typeInfo;
				if(!CheckTypePermissionObjectCompatibility(typePermissionObject, out typeInfo)) {
					return;
				}
				if(memberPermissionObject == null) {
					return;
				}
				IEnumerable<IMemberInfo> memberInfos = GetMemberInfos(typeInfo, memberPermissionObject);
				foreach(IMemberInfo member in memberInfos) {
					if(member.IsAssociation) {
						associatedTypePermissionObject = GetAssociatedTypePermissionObject(member);
						if(member.IsAggregated) {
							if(!member.MemberType.IsGenericType || !member.MemberType.GetGenericTypeDefinition().IsEquivalentTo(typeof(IList<>))) {
								continue;
							}
							if(!CheckConditionsForSetAssociationPermissions(typePermissionObject, associatedTypePermissionObject, memberPermissionObject, null, AssociationPermissionsCheckingMode.SetMemberForAggregated)) {
								continue;
							}
							associatedTypePermissionObjects.Add(associatedTypePermissionObject);
							associatedTypePermissionObject.IsCalledSetAssociationPermissions = true;
							if(operations.Contains("Read")) {
								associatedTypePermissionObject.AllowRead = memberPermissionObject.AllowRead;
							}
							if(operations.Contains("Write")) {
								associatedTypePermissionObject.AllowWrite = memberPermissionObject.AllowWrite;
								associatedTypePermissionObject.AllowCreate = memberPermissionObject.AllowWrite;
								associatedTypePermissionObject.AllowDelete = memberPermissionObject.AllowWrite;
							}
						}
						else {
							SecuritySystemMemberPermissionsObject associatedMemberPermissionsObject = GetAssociatedMemberPermissionsObject(associatedTypePermissionObject, member);
							if(!CheckConditionsForSetAssociationPermissions(typePermissionObject, associatedTypePermissionObject, memberPermissionObject, associatedMemberPermissionsObject, AssociationPermissionsCheckingMode.SetMemberForAssociation)) {
								continue;
							}
							associatedTypePermissionObjects.Add(associatedTypePermissionObject);
							associatedTypePermissionObject.IsCalledSetAssociationPermissions = true;
							if(operations.Contains("Read")) {
								associatedMemberPermissionsObject.AllowRead = memberPermissionObject.AllowRead;
							}
							if(operations.Contains("Write")) {
								associatedMemberPermissionsObject.AllowWrite = memberPermissionObject.AllowWrite;
							}
						}
					}
				}
			}
			finally {
				typePermissionObject.IsCalledSetAssociationPermissions = false;
				foreach(TypePermissionObject associatedTypePermissionObjectItem in associatedTypePermissionObjects) {
					if(!TypePermissions.Contains(associatedTypePermissionObjectItem)) {
						TypePermissions.Add(associatedTypePermissionObjectItem);
					}
					associatedTypePermissionObjectItem.IsCalledSetAssociationPermissions = false;
				}
			}
		}
		public TypePermissionObject FindTypePermissionObject<T>() {
			return FindTypePermissionObject(typeof(T));
		}
		public TypePermissionObject FindTypePermissionObject(Type type) {
			return TypePermissions.Where(t =>
					t.TargetType == type).FirstOrDefault();
		}
		public TypePermissionObject SetTypePermissions<T>(string operations, SecuritySystemModifier securitySystemModifier) {
			return SetTypePermissions(typeof(T), operations, securitySystemModifier);
		}
		public TypePermissionObject SetTypePermissions(Type type, string operations, SecuritySystemModifier securitySystemModifier) {
			TypePermissionObject typePermissionObject = TypePermissions.Where(t =>
				t.TargetType == type).FirstOrDefault();
			if(typePermissionObject == null) {
				typePermissionObject = new TypePermissionObject();
				typePermissionObject.TargetType = type;
				typePermissionObject.Role = this;
				TypePermissions.Add(typePermissionObject);
			}
			bool modifier;
			if(securitySystemModifier == SecuritySystemModifier.Allow) {
				modifier = true;
			}
			else {
				modifier = false;
			}
			string[] operationItems = operations.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries);
			foreach(string operation in operationItems) {
				switch(operation.Trim()) {
					case SecurityOperations.Read:
						typePermissionObject.AllowRead = modifier;
						break;
					case SecurityOperations.Write:
						typePermissionObject.AllowWrite = modifier;
						break;
					case SecurityOperations.Create:
						typePermissionObject.AllowCreate = modifier;
						break;
					case SecurityOperations.Delete:
						typePermissionObject.AllowDelete = modifier;
						break;
					case SecurityOperations.Navigate:
						typePermissionObject.AllowNavigate = modifier;
						break;
					default:
						throw DevExpress.ExpressApp.Utils.Guard.CreateArgumentOutOfRangeException(operation, "operations");
				}
			}
			return typePermissionObject;
		}
		public void SetTypePermissionsRecursively(Type targetType, string operations, SecuritySystemModifier modifier) {
			Guard.ArgumentNotNull(targetType, "targetType");
			foreach(Type type in SecurityStrategy.GetSecuredTypes()) {
				if((targetType == type) || targetType.IsAssignableFrom(type)) {
					SetTypePermissions(type, operations, modifier);
				}
			}
		}
		public void SetTypePermissionsRecursively<T>(string operations, SecuritySystemModifier modifier) {
			SetTypePermissionsRecursively(typeof(T), operations, modifier);
		}
		public SecuritySystemMemberPermissionsObject AddMemberAccessPermission(Type type, string memberName, string operations) {
			return AddMemberAccessPermission(type, memberName, operations, null);
		}
		public SecuritySystemMemberPermissionsObject AddMemberAccessPermission<T>(string memberName, string operations) {
			return AddMemberAccessPermission(typeof(T), memberName, operations, null);
		}
		public SecuritySystemMemberPermissionsObject AddMemberAccessPermission<T>(string memberName, string operations, string criteria) {
			return AddMemberAccessPermission(typeof(T), memberName, operations, criteria);
		}
		public SecuritySystemMemberPermissionsObject AddMemberAccessPermission(Type type, string memberName, string operations, string criteria) {
			TypePermissionObject typePermissionObject = TypePermissions.Where(t =>
			 t.TargetType == type).FirstOrDefault();
			if(typePermissionObject == null) {
				typePermissionObject = new TypePermissionObject();
				typePermissionObject.TargetType = type;
				typePermissionObject.Role = this;
				TypePermissions.Add(typePermissionObject);
			}
			var memberPermission = new SecuritySystemMemberPermissionsObject();
			memberPermission.Owner = typePermissionObject;
			memberPermission.Criteria = criteria;
			memberPermission.Members = memberName;
			string[] operationItems = operations.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries);
			foreach(string operation in operationItems) {
				switch(operation.Trim()) {
					case SecurityOperations.Read:
						memberPermission.AllowRead = true;
						break;
					case SecurityOperations.Write:
						memberPermission.AllowWrite = true;
						break;
					default:
						throw DevExpress.ExpressApp.Utils.Guard.CreateArgumentOutOfRangeException(operation, "operations");
				}
			}
			typePermissionObject.MemberPermissions.Add(memberPermission);
			return memberPermission;
		}
		public void AddObjectAccessPermission<T>(string criteria, string operations) {
			AddObjectAccessPermission(typeof(T), criteria, operations);
		}
		public void AddObjectAccessPermission(Type type, string criteria, string operations) {
			TypePermissionObject typePermissionObject = TypePermissions.Where(t =>
			t.TargetType == type).FirstOrDefault();
			if(typePermissionObject == null) {
				typePermissionObject = new TypePermissionObject();
				typePermissionObject.TargetType = type;
				typePermissionObject.Role = this;
				TypePermissions.Add(typePermissionObject);
			}
			var objectPermission = new SecuritySystemObjectPermissionsObject();
			objectPermission.Owner = typePermissionObject;
			objectPermission.Criteria = criteria;
			string[] operationItems = operations.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries);
			foreach(string operation in operationItems) {
				switch(operation.Trim()) {
					case SecurityOperations.Read:
						objectPermission.AllowRead = true;
						break;
					case SecurityOperations.Write:
						objectPermission.AllowWrite = true;
						break;
					case SecurityOperations.Delete:
						objectPermission.AllowDelete = true;
						break;
					case SecurityOperations.Navigate:
						objectPermission.AllowNavigate = true;
						break;
					default:
						throw DevExpress.ExpressApp.Utils.Guard.CreateArgumentOutOfRangeException(operation, "operations");
				}
			}
			typePermissionObject.ObjectPermissions.Add(objectPermission);
		}
		[DefaultValue(AutoAssociationPermissionsDefaultValue)]
		public static bool AutoAssociationPermissions { get; set; }
	}
}
