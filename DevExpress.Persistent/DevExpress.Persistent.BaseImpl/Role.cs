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
using System.ComponentModel;
using System.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[DefaultProperty("Name")]
	public class RoleBase : BaseObject {
		private RoleImpl role = new RoleImpl();
		public RoleBase(Session session) : base(session) { }
		public PersistentPermission AddPermission(IPermission permission) {
			PersistentPermission result = new PersistentPermission(Session, permission);
			PersistentPermissions.Add(result);
			return result;
		}
		[MemberDesignTimeVisibility(false), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorCollectionReturnsNewValueOnEachAccess))]
		public ReadOnlyCollection<IPermission> Permissions {
			get {
				IList<IPersistentPermission> result = new List<IPersistentPermission>();
				foreach(IPersistentPermission persistentPermission in PersistentPermissions) {
					result.Add(persistentPermission);
				}
				return role.GetPermissions(result);
			}
		}
		public string Name {
			get { return role.Name; }
			set {
				string oldValue = role.Name;
				role.Name = value;
				OnChanged("Name", oldValue, role.Name);
			}
		}
		[Aggregated, Association("Role-PersistentPermissions")]
		public XPCollection<PersistentPermission> PersistentPermissions {
			get { return GetCollection<PersistentPermission>("PersistentPermissions"); }
		}
	}
	[RuleCriteria(Role.ruleId_RoleIsNotReferenced, DefaultContexts.Delete, "Users.Count == 0",
		"Cannot delete the role because there are users that reference it", SkipNullOrEmptyValues = true)]
	public class Role : RoleBase, IRole, ICustomizableRole, ISupportAddUser {
		public const string ruleId_RoleIsNotReferenced = "Role is not referenced";
		public Role(Session session) : base(session) { }
		[Association("User-Role")]
		public XPCollection<User> Users {
			get { return GetCollection<User>("Users"); }
		}
		IList<IUser> IRole.Users {
			get {
				IList<IUser> result = new List<IUser>();
				foreach(IUser user in Users) {
					result.Add(user);
				}
				return new ReadOnlyCollection<IUser>(result);
			}
		}
		void ICustomizableRole.AddPermission(IPermission permission) {
			base.AddPermission(permission);
		}
		void ISupportAddUser.AddUser(object user) {
			Users.Add((User)user);
		}
	}
}
