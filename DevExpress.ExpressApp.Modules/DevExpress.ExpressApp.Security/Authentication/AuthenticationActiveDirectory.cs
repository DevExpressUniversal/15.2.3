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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Security.Principal;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base.Security;
namespace DevExpress.ExpressApp.Security {
	public class CustomCreateUserEventArgs : HandledEventArgs {
		private string userName;
		private object user;
		private IObjectSpace objectSpace;
		public CustomCreateUserEventArgs(IObjectSpace objectSpace, string userName) {
			this.userName = userName;
			this.objectSpace = objectSpace;
		}
		public string UserName {
			get { return userName; }
		}
		public object User {
			get { return user; }
			set { user = value; }
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
	}
	[Designer("DevExpress.ExpressApp.Security.Design.AuthenticationActiveDirectoryDesigner, DevExpress.ExpressApp.Security.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[DXToolboxItem(true)] 
	[DevExpress.Utils.ToolboxTabName(DevExpress.ExpressApp.XafAssemblyInfo.DXTabXafSecurity)]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.Security.AuthenticationActiveDirectory), "Resources.Toolbox_Authentication_ActiveDirectory.ico")]
	public class AuthenticationActiveDirectory : AuthenticationBase
	{
		private bool createUserAutomatically = false;
		protected Type userType;
		protected Type logonParametersType;
		private object logonParameters = null;
		private object CreateLogonParameters(Type type) {
			if(type != null) {
				return TypeHelper.CreateInstance(type);
			}
			else {
				return null;
			}
		}
		public AuthenticationActiveDirectory() {
		}
		public AuthenticationActiveDirectory(Type userType, Type logonParametersType) {
			this.UserType = userType;
			this.LogonParametersType = logonParametersType;
		}
		public override void Logoff() {
			base.Logoff();
			logonParameters = CreateLogonParameters(LogonParametersType);
		}
		protected virtual string GetUserName() {
			string result = WindowsIdentity.GetCurrent().Name;
			return result;
		}
		public override void SetLogonParameters(object logonParameters) {
			this.logonParameters = logonParameters;
		}
		public override object Authenticate(IObjectSpace objectSpace) {
			string userName = GetUserName();
			object user = objectSpace.FindObject(UserType, new BinaryOperator("UserName", userName));
			if(user == null) {
				if(createUserAutomatically) {
					CustomCreateUserEventArgs args = new CustomCreateUserEventArgs(objectSpace, userName);
					if(CustomCreateUser != null) {
						CustomCreateUser(this, args);
						user = (IAuthenticationActiveDirectoryUser)args.User;
					}
					if(!args.Handled) {
						user = objectSpace.CreateObject(UserType);
						((IAuthenticationActiveDirectoryUser)user).UserName = userName;
						if(Security is ICanInitializeNewUser) {
							((ICanInitializeNewUser)Security).InitializeNewUser(objectSpace, user);
						}
					}
					bool strictSecurityStrategyBehavior = SecurityModule.StrictSecurityStrategyBehavior;
					SecurityModule.StrictSecurityStrategyBehavior = false; 
					objectSpace.CommitChanges();
					SecurityModule.StrictSecurityStrategyBehavior = strictSecurityStrategyBehavior;
				}
			}
			if(user == null) {
				throw new AuthenticationException(userName);
			}
			return user;
		}
		public override bool IsSecurityMember(Type type, string memberName) {
			if(typeof(IAuthenticationActiveDirectoryUser).IsAssignableFrom(type)) {
				if(typeof(IAuthenticationActiveDirectoryUser).GetMember(memberName).Length > 0) {
					return true;
				}
			}
			return false;
		}
		[Browsable(false)]
		public override object LogonParameters {
			get {
				return logonParameters;
			}
		}
		public override bool AskLogonParametersViaUI {
			get {
				return logonParameters != null && logonParameters.GetType() != typeof(object);
			}
		}
		[Category("Behavior")]
		public bool CreateUserAutomatically {
			get { return createUserAutomatically; }
			set { createUserAutomatically = value; }
		}
		public override bool IsLogoffEnabled {
			get {	return false; }
		}
		public override IList<Type> GetBusinessClasses() {
			List<Type> result = new List<Type>();
			if(UserType != null) {
				result.Add(UserType);
			}
			if(LogonParametersType != null) {
				result.Add(LogonParametersType);
			}
			return result;
		}
		[Category("Behavior")]
		[DefaultValue(null)]
		public override Type UserType {
			get { return userType; }
			set {
				userType = value;
				if(userType != null && !typeof(IAuthenticationActiveDirectoryUser).IsAssignableFrom(userType)) {
					throw new ArgumentException(string.Format("AuthenticationActiveDirectory does not support the {0} user type.\nA class that implements the IAuthenticationActiveDirectoryUser interface should be set for the UserType property.", userType));
				}
			}
		}
		[Category("Behavior"), RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(BusinessClassTypeConverter<Object>))]
		public Type LogonParametersType {
			get { return logonParametersType; }
			set {
				logonParametersType = value;
				logonParameters = CreateLogonParameters(logonParametersType);
			}
		}
		public event EventHandler<CustomCreateUserEventArgs> CustomCreateUser;
	}
	public class AuthenticationActiveDirectory<UserType, LogonParametersType> : AuthenticationActiveDirectory {
		public AuthenticationActiveDirectory() : base(typeof(UserType), typeof(LogonParametersType)) { }
	}
	public class AuthenticationActiveDirectory<UserType> : AuthenticationActiveDirectory<UserType, object>
		where UserType : IAuthenticationActiveDirectoryUser 
	{
	}
}
