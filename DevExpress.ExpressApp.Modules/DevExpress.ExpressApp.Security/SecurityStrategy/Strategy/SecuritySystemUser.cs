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
using System.Collections.ObjectModel;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Validation;
namespace DevExpress.ExpressApp.Security.Strategy {
	[MapInheritance(MapInheritanceType.ParentTable), System.ComponentModel.DisplayName("User")]
	public class SecuritySystemUser : SecuritySystemUserBase, IOperationPermissionProvider, ISecurityUserWithRoles, ICanInitialize {
		public SecuritySystemUser(Session session) : base(session) { }
		protected virtual IEnumerable<ISecurityRole> GetSecurityRoles() {
			IList<ISecurityRole> result = new List<ISecurityRole>();
			foreach(SecuritySystemRole role in Roles) {
				result.Add(role);
			}
			return new ReadOnlyCollection<ISecurityRole>(result);
		}
		protected virtual IEnumerable<IOperationPermission> GetPermissions() {
			return new IOperationPermission[0];
		}
		protected virtual IEnumerable<IOperationPermissionProvider> GetChildPermissionProviders() {
			return new EnumerableConverter<IOperationPermissionProvider, SecuritySystemRole>(Roles);
		}
		[Association("Users-Roles")]
		[RuleRequiredField("SecuritySystemUser_Roles_RuleRequiredField", DefaultContexts.Save, TargetCriteria = "IsActive=True", CustomMessageTemplate = "An active user must have at least one role assigned")]
		public XPCollection<SecuritySystemRole> Roles {
			get {
				return GetCollection<SecuritySystemRole>("Roles");
			}
		}
		IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions() {
			return GetPermissions();
		}
		IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren() {
			return GetChildPermissionProviders();
		}
		IList<ISecurityRole> ISecurityUserWithRoles.Roles {
			get {
				IList<ISecurityRole> result = new List<ISecurityRole>();
				foreach(SecuritySystemRole role in GetSecurityRoles()) {
					result.Add(role);
				}
				return new ReadOnlyCollection<ISecurityRole>(result);
			}
		}
		void ICanInitialize.Initialize(IObjectSpace objectSpace, SecurityStrategyComplex security) {
			if((security.RoleType != null) && typeof(SecuritySystemRole).IsAssignableFrom(security.RoleType)) {
				SecuritySystemRole newUserRole = (SecuritySystemRole)objectSpace.FindObject(security.RoleType, new BinaryOperator("Name", security.NewUserRoleName));
				if(newUserRole == null) {
					newUserRole = (SecuritySystemRole)objectSpace.CreateObject(security.RoleType);
					newUserRole.Name = security.NewUserRoleName;
					newUserRole.IsAdministrative = true;
				}
				newUserRole.Users.Add(this);
			}
		}
	}
}
