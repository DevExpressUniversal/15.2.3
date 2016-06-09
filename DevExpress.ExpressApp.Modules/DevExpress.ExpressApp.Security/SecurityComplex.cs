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
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using System.Collections.Generic;
using DevExpress.Persistent.Base.Security;
using System.ComponentModel;
using System.Drawing;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Design;
namespace DevExpress.ExpressApp.Security {
	public interface IRoleTypeProvider { 
		Type RoleType { get; } 
	}
	public interface ISecurityComplex : ISecurityStrategyBase {
	}
	[DXToolboxItem(true)] 
	[DevExpress.Utils.ToolboxTabName(DevExpress.ExpressApp.XafAssemblyInfo.DXTabXafSecurity)]
	[Designer("DevExpress.ExpressApp.Security.Design.SecurityComplexDesigner, DevExpress.ExpressApp.Security.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.Security.SecurityComplex), "Resources.Toolbox_Security_Complex.ico")]
	public class SecurityComplex : SecurityBase, ISecurityComplex, IRoleTypeProvider {
		private Type roleType;
		protected override PermissionSet ReloadPermissions() {
			User.ReloadPermissions();
			PermissionSet result = new PermissionSet(PermissionState.None);
			foreach(IPermission currentPermission in User.Permissions) {
				result.AddPermission(currentPermission);
			}
			return result;
		}
		protected override void InitializeNewUserCore(IObjectSpace objectSpace, object user) {
			if(user == null) {
				throw new ArgumentNullException("user");
			}
			IUser complexUser = (IUser)user;
			complexUser.IsActive = true;
			IRole defaultRole = (IRole)objectSpace.FindObject(roleType, new BinaryOperator("Name", "Default"));
			if(defaultRole == null) {
				defaultRole = (IRole)objectSpace.CreateObject(roleType);
				defaultRole.Name = "Default";
				if(defaultRole is ICustomizableRole) {
					((ICustomizableRole)defaultRole).AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
				}
			}
			if(defaultRole is ISupportAddUser) {
				((ISupportAddUser)defaultRole).AddUser(complexUser);
			}
		}
		public SecurityComplex() { }
		public SecurityComplex(Type userType, Type roleType, AuthenticationBase authentication)
			: base(userType, authentication) {
			if(!typeof(IUser).IsAssignableFrom(userType)){
				throw new ArgumentException("UserType");
			}
			this.roleType = roleType;
			IsGrantedForNonExistentPermission = false;
		}
		public override void Logon(object user) {
			if(!((IUser)user).IsActive) {
				throw new AuthenticationException(((IUser)user).UserName);
			}
			base.Logon(user);
		}
		public override IList<Type> GetBusinessClasses() {
			List<Type> result = new List<Type>(base.GetBusinessClasses());
			if(UserType != null && !result.Contains(UserType)) {
				result.Add(UserType);
			}
			if(RoleType != null && !result.Contains(RoleType)) {
				result.Add(RoleType);
			}
			return result;
		}
		public override bool IsSecurityMember(Type type, string memberName) {
			if(typeof(IUser).IsAssignableFrom(type)) {
				if(typeof(IUser).GetMember(memberName).Length > 0) {
					return true;
				}
			}
			return base.IsSecurityMember(type, memberName);
		}
		public override string UserName {
			get { 
				return (User != null) ? ((IUser)User).UserName : ""; 
			}
		}
		public override object UserId {
			get {
				if((User != null) && (LogonObjectSpace != null)) {
					return LogonObjectSpace.GetKeyValue(User);
				}
				else if(User != null) {
					return BaseObjectSpace.GetKeyValue(XafTypesInfo.Instance, User);
				}
				else {
					return base.UserId;
				}
			}
		}
		[Browsable(false)]
		public new IUser User {
			get { return (IUser)base.User; }
		}
		[TypeConverter(typeof(BusinessClassTypeConverter<IUser>))]
		public Type UserType {
			get { return userType; }
			set {
				if(userType != null && !typeof(IUser).IsAssignableFrom(userType)) {
					throw new ArgumentException("UserType");
				}
				userType = value;
				if(Authentication != null) {
					Authentication.UserType = userType;
				}
			}
		}
		[TypeConverter(typeof(BusinessClassTypeConverter<IRole>))]
		public Type RoleType {
			get { return roleType; }
			set {
				roleType = value;
			}
		}
	}
	public class SecurityComplex<UserType, RoleType> : SecurityComplex where UserType : IUser where RoleType : IRole {
		public SecurityComplex(AuthenticationBase authentication) : base(typeof(UserType), typeof(RoleType), authentication) { }
		public virtual void Logon(UserType user) {
			base.Logon(user);
		}
	}
}
