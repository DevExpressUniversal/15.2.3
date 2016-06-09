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

using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base.Security;
using System;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.Security {
	public static class UserWithRolesExtensions {
		public static bool IsUserInRole(this IUserWithRoles user, string roleName) {
			Guard.ArgumentNotNull(user, "user");
			if(string.IsNullOrEmpty(roleName)) {
				throw new ArgumentException();
			}
			IRole role = Enumerator.Find<IRole>(user.Roles,
				delegate(IRole currentRole) {
					return (string.Compare(currentRole.Name, roleName, true) == 0);
				});
			return (role != null);
		}
	}
	public static class SecurityUserWithRolesExtensions {
		private static IList<SecuritySystemRole> CollectChildRoles(IList<SecuritySystemRole> parentRoles, IList<SecuritySystemRole> rolesToExclude) {
			IList<SecuritySystemRole> result = new List<SecuritySystemRole>();
			foreach(SecuritySystemRole parentRole in parentRoles) {
				foreach(SecuritySystemRole childRole in parentRole.ChildRoles) {
					if(!rolesToExclude.Contains(childRole)) {
						result.Add(childRole);
					}
				}
			}
			return result;
		}
		private static bool ContainsRole(SecuritySystemRole role, string roleName) {
			Guard.ArgumentNotNull(role, "role");
			IList<SecuritySystemRole> rolesToCheck = new List<SecuritySystemRole>();
			rolesToCheck.Add(role);
			List<SecuritySystemRole> checkedRoles = new List<SecuritySystemRole>();
			do {
				SecuritySystemRole foundRole = Enumerator.Find<SecuritySystemRole>(rolesToCheck,
					delegate(SecuritySystemRole currentRole) {
						return (string.Compare(currentRole.Name, roleName, true) == 0);
					});
				if(foundRole != null) {
					return true;
				}
				checkedRoles.AddRange(rolesToCheck);
				rolesToCheck = CollectChildRoles(rolesToCheck, checkedRoles);
			} while(rolesToCheck.Count > 0);
			return false;
		}
		public static bool IsUserInRole(this ISecurityUserWithRoles user, string roleName) {
			Guard.ArgumentNotNull(user, "user");
			if(string.IsNullOrEmpty(roleName)) {
				throw new ArgumentException();
			}
			ISecurityRole role = Enumerator.Find<ISecurityRole>(user.Roles,
				delegate(ISecurityRole currentRole) {
					return (string.Compare(currentRole.Name, roleName, true) == 0);
				});
			if(role != null) {
				return true;
			}
			foreach(ISecurityRole currentRole in user.Roles) {
				if(currentRole is SecuritySystemRole && ContainsRole((SecuritySystemRole)currentRole, roleName)) {
					return true;
				}
			}
			return false;
		}
	}   
}
