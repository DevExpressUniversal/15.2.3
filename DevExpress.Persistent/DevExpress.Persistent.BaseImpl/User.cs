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
using System.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[ImageName("BO_User"), System.ComponentModel.DisplayName("User")]
	public class User : Person, IUserWithRoles, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser {
		public const string ruleId_RoleRequired = "Role required";
		public const string ruleId_UserNameRequired = "User Name required";
		public const string ruleId_UserNameIsUnique = "User Name is unique";
		private UserImpl userImpl;
		private UserImpl user {
			get {
				if(userImpl == null)
					userImpl = new UserImpl(this);
				return userImpl;
			}
		}
		private List<IPermission> permissions;
		public User(Session session)
			: base(session) {
			permissions = new List<IPermission>();
		}
		public void ReloadPermissions() {
			Roles.Reload();
			foreach(Role role in Roles) {
				role.PersistentPermissions.Reload();
			}
		}
		public bool ComparePassword(string password) {
			return user.ComparePassword(password);
		}
		public void SetPassword(string password) {
			user.SetPassword(password);
		}
#if MediumTrust
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		[Persistent]
		public string StoredPassword {
			get { return user.StoredPassword; }
			set {
				string oldValue = user.StoredPassword;
				user.StoredPassword = value;
				OnChanged("StoredPassword", oldValue, user.StoredPassword);
			}
		}
#else
		[Persistent]
		private string StoredPassword {
			get { return user.StoredPassword; }
			set {
				string oldValue = user.StoredPassword;
				user.StoredPassword = value;
				OnChanged("StoredPassword", oldValue, user.StoredPassword);
			}
		}
#endif
		[RuleRequiredField(User.ruleId_RoleRequired, DefaultContexts.Save, TargetCriteria = "IsActive=True", CustomMessageTemplate = "An active user must have at least one role assigned")]
		[Association("User-Role")]
		public XPCollection<Role> Roles {
			get { return GetCollection<Role>("Roles"); }
		}
		IList<IRole> IUserWithRoles.Roles {
			get {
				IList<IRole> result = new List<IRole>();
				foreach(IRole role in Roles) {
					result.Add(role);
				}
				return new ReadOnlyCollection<IRole>(result);
			}
		}
		[RuleRequiredField(User.ruleId_UserNameRequired, "Save", "The user name must not be empty")]
		[RuleUniqueValue(User.ruleId_UserNameIsUnique, "Save", "The login with the entered UserName was already registered within the system")]
		public string UserName {
			get { return user.UserName; }
			set {
				string oldValue = user.UserName;
				user.UserName = value;
				OnChanged("UserName", oldValue, user.UserName);
			}
		}
		public bool ChangePasswordOnFirstLogon {
			get { return user.ChangePasswordAfterLogon; }
			set {
				bool oldValue = user.ChangePasswordAfterLogon;
				user.ChangePasswordAfterLogon = value;
				OnChanged("ChangePasswordOnFirstLogon", oldValue, user.ChangePasswordAfterLogon);
			}
		}
		public bool IsActive {
			get { return user.IsActive; }
			set {
				bool oldValue = user.IsActive;
				user.IsActive = value;
				OnChanged("IsActive", oldValue, user.IsActive);
			}
		}
		public IList<IPermission> Permissions {
			get {
				permissions.Clear();
				foreach(Role role in Roles) {
					permissions.AddRange(role.Permissions);
				}
				return permissions.AsReadOnly();
			}
		}
	}
}
