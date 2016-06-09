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
using System.Drawing;
using System.Runtime.Serialization;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Design;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
using DevExpress.Persistent.Base.Security;
namespace DevExpress.ExpressApp.Security {
	[Serializable, DomainComponent]
	public class AuthenticationStandardLogonParameters : INotifyPropertyChanged, ISerializable, ICustomObjectSerialize {
		private string userName;
		private string password;
		protected AuthenticationStandardLogonParameters(SerializationInfo info, StreamingContext context) {
			userName = info.GetString("userName");
			password = info.GetString("password");
		}
		public AuthenticationStandardLogonParameters(string userName, string password) {
			this.userName = userName;
			this.password = password;
		}
		public AuthenticationStandardLogonParameters() { }
		[System.Security.SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("userName", userName);
			info.AddValue("password", password);
		}
		public void ReadPropertyValues(SettingsStorage storage) {
			userName = storage.LoadOption("", "UserName");
		}
		public void WritePropertyValues(SettingsStorage storage) {
			storage.SaveOption("", "UserName", userName);
		}
		public string UserName {
			get { return userName; }
			set { userName = value; RaisePropertyChanged("UserName"); }
		}
		[ModelDefault("IsPassword", "True")]
		public string Password {
			get { return password; }
			set { password = value; RaisePropertyChanged("Password"); }
		}
		protected void RaisePropertyChanged(string propertyName) {
			if(PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
	public interface IAuthenticationStandard { }
	[DXToolboxItem(true)]
	[DevExpress.Utils.ToolboxTabName(DevExpress.ExpressApp.XafAssemblyInfo.DXTabXafSecurity)]
	[ToolboxBitmap(typeof(DevExpress.ExpressApp.Security.AuthenticationStandard), "Resources.Toolbox_Authentication_Standard.ico")]
	public class AuthenticationStandard : AuthenticationBase, IAuthenticationStandard {
		private AuthenticationStandardLogonParameters logonParameters;
		private static object lockObject = new object();
		protected Type userType;
		protected Type logonParametersType;
		private AuthenticationStandardLogonParameters CreateLogonParametersObject(Type logonParametersType) {
			Guard.TypeArgumentIs(typeof(AuthenticationStandardLogonParameters), logonParametersType, "logonParametersType");
			return (AuthenticationStandardLogonParameters)TypeHelper.CreateInstance(logonParametersType);
		}
		public AuthenticationStandard() {
			lock(lockObject) {
				this.LogonParametersType = typeof(AuthenticationStandardLogonParameters);
			}
		}
		public AuthenticationStandard(Type userType, Type logonParametersType) {
			this.UserType = userType;
			this.LogonParametersType = logonParametersType;
		}
		public override void SetLogonParameters(object logonParameters) {
			Guard.ArgumentNotNull(logonParameters, "logonParameters");
			this.logonParameters = (AuthenticationStandardLogonParameters)logonParameters;
		}
		public override object Authenticate(IObjectSpace objectSpace) {
			Guard.ArgumentNotNull(logonParameters, "logonParameters");
			if(string.IsNullOrEmpty(logonParameters.UserName))
				throw new ArgumentException(SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.UserNameIsEmpty));
			IAuthenticationStandardUser user = (IAuthenticationStandardUser)objectSpace.FindObject(UserType, new BinaryOperator("UserName", logonParameters.UserName));
			if(user == null || !user.ComparePassword(logonParameters.Password)) {
				throw new AuthenticationException(logonParameters.UserName, SecurityExceptionLocalizer.GetExceptionMessage(SecurityExceptionId.RetypeTheInformation));
			}
			return user;
		}
		public override void ClearSecuredLogonParameters() {
			logonParameters.Password = string.Empty;
			base.ClearSecuredLogonParameters();
		}
		public override bool IsSecurityMember(Type type, string memberName) {
			if(typeof(IAuthenticationStandardUser).IsAssignableFrom(type)) {
				if(typeof(IAuthenticationStandardUser).GetMember(memberName).Length > 0) {
					return true;
				}
			}
			return false;
		}
		public override void Logoff() {
			base.Logoff();
			logonParameters = CreateLogonParametersObject(LogonParametersType);
		}
		[Browsable(false)]
		public override object LogonParameters {
			get {
				return logonParameters;
			}
		}
		public override bool AskLogonParametersViaUI {
			get {
				return true;
			}
		}
		public override bool IsLogoffEnabled {
			get { return true; }
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
		public override Type UserType {
			get { return userType; }
			set {
				userType = value;
				if(userType != null && !typeof(IAuthenticationStandardUser).IsAssignableFrom(userType)) {
					throw new ArgumentException(string.Format("AuthenticationStandard does not support the {0} user type.\nA class that implements the IAuthenticationStandardUser interface should be set for the UserType property.", userType));
				}
			}
		}
		[TypeConverter(typeof(BusinessClassTypeConverter<AuthenticationStandardLogonParameters>))]
		[RefreshProperties(RefreshProperties.All)]
		[Category("Behavior")]
		public Type LogonParametersType {
			get { return logonParametersType; }
			set {
				logonParametersType = value;
				if(value != null) {
					logonParameters = CreateLogonParametersObject(logonParametersType);
				}
			}
		}
	}
	public class InitializeDefaultLogonParametersEventArgs : EventArgs {
		public InitializeDefaultLogonParametersEventArgs(AuthenticationStandardLogonParameters defaultLogonParameters) {
			DefaultLogonParameters = defaultLogonParameters;
		}
		public AuthenticationStandardLogonParameters DefaultLogonParameters { get; set; }
	}
	public class AuthenticationStandard<UserType, LogonParametersType> : AuthenticationStandard{ 
		public AuthenticationStandard():base(typeof(UserType), typeof(LogonParametersType)) {}
	}
	public class AuthenticationStandard<UserType> : AuthenticationStandard<UserType, AuthenticationStandardLogonParameters>
		where UserType : IAuthenticationStandardUser {
	}
}
