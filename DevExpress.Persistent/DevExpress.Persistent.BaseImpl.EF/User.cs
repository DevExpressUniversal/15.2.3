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
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
namespace DevExpress.Persistent.BaseImpl.EF {
	public class User : ISecurityUser, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser, IOperationPermissionProvider, ISecurityUserWithRoles, INotifyPropertyChanged, ICanInitialize {
		private Boolean changePasswordOnFirstLogon;
		public const string ruleId_RoleRequired = "Role required";
		public const string ruleId_UserNameRequired = "User Name required";
		public const string ruleId_UserNameIsUnique = "User Name is unique";
		public User() {
			IsActive = true;
			Roles = new List<Role>();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		[RuleRequiredField(User.ruleId_UserNameRequired, "Save", "The user name must not be empty")]
		[RuleUniqueValue(User.ruleId_UserNameIsUnique, "Save", "The login with the entered UserName was already registered within the system")]
		public String UserName { get; set; }
		public Boolean IsActive { get; set; }
		public Boolean ChangePasswordOnFirstLogon {
			get { return changePasswordOnFirstLogon; }
			set {
				changePasswordOnFirstLogon = value;
				if(PropertyChanged != null) {
					PropertyChangedEventArgs args = new PropertyChangedEventArgs("ChangePasswordOnFirstLogon");
					PropertyChanged(this, args);
				}
			}
		}
		[Browsable(false), SecurityBrowsable]
		public String StoredPassword { get; set; }
		[RuleRequiredField(User.ruleId_RoleRequired, DefaultContexts.Save, TargetCriteria = "IsActive=True", CustomMessageTemplate = "An active user must have at least one role assigned")]
		public virtual IList<Role> Roles { get; set; }
		Boolean ISecurityUser.IsActive {
			get { return IsActive; }
		}
		String ISecurityUser.UserName {
			get { return UserName; }
		}
		String IAuthenticationActiveDirectoryUser.UserName {
			get { return UserName; }
			set { UserName = value; }
		}
		Boolean IAuthenticationStandardUser.ComparePassword(String password) {
			PasswordCryptographer passwordCryptographer = new PasswordCryptographer();
			return passwordCryptographer.AreEqual(StoredPassword, password);
		}
		Boolean IAuthenticationStandardUser.ChangePasswordOnFirstLogon {
			get { return ChangePasswordOnFirstLogon; }
			set { ChangePasswordOnFirstLogon = value;}
		}
		String IAuthenticationStandardUser.UserName {
			get { return UserName; }
		}
		IEnumerable<IOperationPermissionProvider> IOperationPermissionProvider.GetChildren() {
			return new EnumerableConverter<IOperationPermissionProvider, Role>(Roles);
		}
		IEnumerable<IOperationPermission> IOperationPermissionProvider.GetPermissions() {
			return new IOperationPermission[0];
		}
		IList<ISecurityRole> ISecurityUserWithRoles.Roles {
			get {
				IList<ISecurityRole> result = new List<ISecurityRole>();
				foreach(Role role in Roles) {
					result.Add(role);
				}
				return new ReadOnlyCollection<ISecurityRole>(result);
			}
		}
		void ICanInitialize.Initialize(IObjectSpace objectSpace, SecurityStrategyComplex security) {
			if((security.RoleType != null) && typeof(Role).IsAssignableFrom(security.RoleType)) {
				Role newUserRole = (Role)objectSpace.FindObject(security.RoleType, new BinaryOperator("Name", security.NewUserRoleName));
				if(newUserRole == null) {
					newUserRole = (Role)objectSpace.CreateObject(security.RoleType);
					newUserRole.Name = security.NewUserRoleName;
					newUserRole.IsAdministrative = true;
				}
				newUserRole.Users.Add(this);
			}
		}
		public void SetPassword(String password) {
			PasswordCryptographer passwordCryptographer = new PasswordCryptographer();
			StoredPassword = passwordCryptographer.GenerateSaltedPassword(password);
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
