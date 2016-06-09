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

using DevExpress.ExpressApp.Security.Strategy.PermissionMatrix;
using DevExpress.ExpressApp.Utils;
using System;
namespace DevExpress.ExpressApp.Security.Strategy {
	public class TypePermissionsObjectOwnerHelper {
		public static SecuritySystemTypePermissionObject EnsureTypePermissionObject(SecuritySystemTypePermissionsObjectOwner owner, Type targetType) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(targetType, "targetType");
			SecuritySystemTypePermissionObject typePermissionObject = FindTypePermissionObject(owner, targetType);
			if(typePermissionObject == null) {
				typePermissionObject = new SecuritySystemTypePermissionObject(owner.Session);
				typePermissionObject.TargetType = targetType;
				owner.TypePermissions.Add(typePermissionObject);
			}
			return typePermissionObject;
		}
		public static SecuritySystemTypePermissionObject EnsureTypePermissionObject<T>(SecuritySystemTypePermissionsObjectOwner owner) {
			return EnsureTypePermissionObject(owner, typeof(T));
		}
		public static SecuritySystemTypePermissionObject FindTypePermissionObject(SecuritySystemTypePermissionsObjectOwner owner, Type targetType) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNull(targetType, "targetType");
			foreach(SecuritySystemTypePermissionObject typePermissionObject in owner.TypePermissions) {
				if(typePermissionObject.TargetType == targetType) {
					return typePermissionObject;
				}
			}
			return null;
		}
		public static SecuritySystemTypePermissionObject FindTypePermissionObject<T>(SecuritySystemTypePermissionsObjectOwner owner) {
			return FindTypePermissionObject(owner, typeof(T));
		}
		public static SecuritySystemTypePermissionObject SetTypePermissions(SecuritySystemTypePermissionsObjectOwner owner, Type targetType, string operations, SecuritySystemModifier modifier) {
			Guard.ArgumentNotNullOrEmpty(operations, "operations");
			SecuritySystemTypePermissionObject typePermissionObject = null;
			if(modifier == SecuritySystemModifier.Allow) {
				typePermissionObject = EnsureTypePermissionObject(owner, targetType);
				PermissionMatrixGrantDenyController.SetTypeOperations(typePermissionObject, operations, true);
			}
			else if(modifier == SecuritySystemModifier.Deny) {
				typePermissionObject = FindTypePermissionObject(owner, targetType);
				if(typePermissionObject != null) {
					PermissionMatrixGrantDenyController.SetTypeOperations(typePermissionObject, operations, false);
				}
			}
			else {
				Guard.CreateArgumentOutOfRangeException(modifier, "modifier");
			}
			return typePermissionObject;
		}
		public static SecuritySystemTypePermissionObject SetTypePermissions<T>(SecuritySystemTypePermissionsObjectOwner owner, string securityOperations, SecuritySystemModifier modifier) {
			return SetTypePermissions(owner, typeof(T), securityOperations, modifier);
		}
		public static void SetTypePermissionsRecursively(SecuritySystemTypePermissionsObjectOwner owner, Type targetType, string operations, SecuritySystemModifier modifier) {
			Guard.ArgumentNotNull(targetType, "targetType");
			foreach(Type type in SecurityStrategy.GetSecuredTypes()) {
				if((targetType == type) || targetType.IsAssignableFrom(type)) {
					SetTypePermissions(owner, type, operations, modifier);
				}
			}
		}
		public static void SetTypePermissionsRecursively<T>(SecuritySystemTypePermissionsObjectOwner owner, string operations, SecuritySystemModifier modifier) {
			SetTypePermissionsRecursively(owner, typeof(T), operations, modifier);
		}
		public static SecuritySystemObjectPermissionsObject AddObjectAccessPermission(SecuritySystemTypePermissionsObjectOwner owner, Type targetType, string criteria, string operations) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNullOrEmpty(operations, "operations");
			SecuritySystemTypePermissionObject typePermissions = EnsureTypePermissionObject(owner, targetType);
			SecuritySystemObjectPermissionsObject objectPermission = new SecuritySystemObjectPermissionsObject(owner.Session);
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
						throw Guard.CreateArgumentOutOfRangeException(operation, "operations");
				}
			}
			objectPermission.Criteria = criteria;
			typePermissions.ObjectPermissions.Add(objectPermission);
			return objectPermission;
		}
		public static SecuritySystemObjectPermissionsObject AddObjectAccessPermission<T>(SecuritySystemTypePermissionsObjectOwner owner, string criteria, string operations) {
			return AddObjectAccessPermission(owner, typeof(T), criteria, operations);
		}
		public static SecuritySystemMemberPermissionsObject AddMemberAccessPermission(SecuritySystemTypePermissionsObjectOwner owner, Type targetType, string members, string operations, string criteria) {
			Guard.ArgumentNotNull(owner, "owner");
			Guard.ArgumentNotNullOrEmpty(members, "members");
			Guard.ArgumentNotNullOrEmpty(operations, "operations");
			bool allowRead = false;
			bool allowWrite = false;
			string[] operationItems = operations.Split(ServerPermissionRequestProcessor.Delimiters, StringSplitOptions.RemoveEmptyEntries);
			foreach(string operation in operationItems) {
				switch(operation) {
					case SecurityOperations.Read:
						allowRead = true;
						break;
					case SecurityOperations.Write:
						allowWrite = true;
						break;
					default:
						throw Guard.CreateArgumentOutOfRangeException(operation, "operations");
				}
			}
			SecuritySystemTypePermissionObject typePermissions = EnsureTypePermissionObject(owner, targetType);
			SecuritySystemMemberPermissionsObject memberPermissions = new SecuritySystemMemberPermissionsObject(owner.Session);
			typePermissions.MemberPermissions.Add(memberPermissions);
			memberPermissions.Members = members;
			memberPermissions.Criteria = criteria;
			memberPermissions.AllowRead = allowRead;
			memberPermissions.AllowWrite = allowWrite;		   
			return memberPermissions;
		}
		public static SecuritySystemMemberPermissionsObject AddMemberAccessPermission(SecuritySystemTypePermissionsObjectOwner owner, Type targetType, string members, string operations) {
			return AddMemberAccessPermission(owner, targetType, members, operations, null);
		}
		public static SecuritySystemMemberPermissionsObject AddMemberAccessPermission<T>(SecuritySystemTypePermissionsObjectOwner owner, string members, string operations) {
			return AddMemberAccessPermission(owner, typeof(T), members, operations);
		}
		public static SecuritySystemMemberPermissionsObject AddMemberAccessPermission<T>(SecuritySystemTypePermissionsObjectOwner owner, string members, string operations, string criteria) {
			return AddMemberAccessPermission(owner, typeof(T), members, operations, criteria);
		}
	}
}
