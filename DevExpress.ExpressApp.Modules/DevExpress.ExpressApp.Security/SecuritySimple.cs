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
using System.ComponentModel;
using System.Drawing;
using System.ComponentModel.Design;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Design;
using DevExpress.Persistent.Base.Security;
namespace DevExpress.ExpressApp.Security {
	[DXToolboxItem(true)] 
	[DevExpress.Utils.ToolboxTabName(DevExpress.ExpressApp.XafAssemblyInfo.DXTabXafSecurity)]
	[Designer("DevExpress.ExpressApp.Security.Design.SecuritySimpleDesigner, DevExpress.ExpressApp.Security.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.Security.SecuritySimple), "Resources.Toolbox_Security_Simple.ico")]
	public class SecuritySimple : SecurityBase  {
		private PermissionSet set = new PermissionSet(PermissionState.None);
		private bool allowNonAdministratorNavigateToUsers = true;
		protected override void InitializeNewUserCore(IObjectSpace objectSpace, object user) {
			if(user == null) {
				throw new ArgumentNullException("user");
			}
			ISimpleUser simpleUser = (ISimpleUser)user;
			simpleUser.IsActive = true;
			simpleUser.IsAdministrator = true;
		}
		protected override PermissionSet ReloadPermissions() {
			PermissionSet result = new PermissionSet(PermissionState.None);
			if(User.IsActive) {
				IsGrantedForNonExistentPermission = true;
				result.AddPermission(new ObjectAccessPermission(typeof(object), ObjectAccess.AllAccess));
				result.AddPermission(new ObjectAccessPermission(typeof(DevExpress.Persistent.Base.General.IPropertyBag), ObjectAccess.AllAccess));
				result.AddPermission(new EditModelPermission(User.IsAdministrator ? ModelAccessModifier.Allow : ModelAccessModifier.Deny));
				if(!User.IsAdministrator) {
					result.AddPermission(new ObjectAccessPermission(UserType, ObjectAccess.ChangeAccess, ObjectAccessModifier.Deny));
					if(!AllowNonAdministratorNavigateToUsers) {
						result.AddPermission(new ObjectAccessPermission(UserType, ObjectAccess.Navigate, ObjectAccessModifier.Deny));
					}
				}
			}
			else {
				IsGrantedForNonExistentPermission = false;
			}
			return result;
		}
		public SecuritySimple() { }
		public SecuritySimple(Type userType, AuthenticationBase authentication)
			: base(userType, authentication) {
			if(!typeof(ISimpleUser).IsAssignableFrom(userType)){
				throw new ArgumentException("UserType");
			}
		}
		public override void Logon(object user) {
			if(!((ISimpleUser)user).IsActive) {
				throw new AuthenticationException(((ISimpleUser)user).UserName);
			}
			base.Logon(user);
		}
		public override bool IsSecurityMember(Type type, string memberName) {
			if(typeof(ISimpleUser).IsAssignableFrom(type)) {
				if(typeof(ISimpleUser).GetMember(memberName).Length > 0) {
					return true;
				}
			}
			return base.IsSecurityMember(type, memberName);
		}
		public override string UserName {
			get { return (User == null) ? "" : ((ISimpleUser)User).UserName; }
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
		public new ISimpleUser User {
			get { return (ISimpleUser)base.User; }
		}
		[TypeConverter(typeof(BusinessClassTypeConverter<ISimpleUser>))]
		public Type UserType {
			get { return userType; }
			set {
				if(userType != null && !typeof(ISimpleUser).IsAssignableFrom(value)) {
					throw new ArgumentException("UserType");
				}
				userType = value;
				if(Authentication != null) {
					Authentication.UserType = userType;
				}
			}
		}
		[DefaultValue(true)]
		public bool AllowNonAdministratorNavigateToUsers {
			get { return allowNonAdministratorNavigateToUsers; }
			set { allowNonAdministratorNavigateToUsers = value; }
		}
	}
	public class SecuritySimple<UserType> : SecuritySimple where UserType : ISimpleUser {
		public SecuritySimple(AuthenticationBase authentication)
			:base(typeof(UserType), authentication) { }
		public virtual void Logon(UserType user) {
			base.Logon(user);
		}
	}
}
