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
using DevExpress.ExpressApp.Localization;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security.Adapters;
namespace DevExpress.ExpressApp.Security {
	internal interface ICanInitializeNewUser {
		void InitializeNewUser(IObjectSpace objectSpace, object user);
	}
	[ToolboxItemFilter("Xaf", ToolboxItemFilterType.Require)]
	public abstract class SecurityStrategyBase : Component, ISupportChangePasswordOption, ISupportChangedNotification, ICanInitializeNewUser, IObjectSpaceLinks, ISecurityStrategyBase {
		[DefaultValue(false)]
		[Browsable(false)]
		private object user;
		private AuthenticationBase authentication;
		private bool isAuthenticated = false; 
		protected IObjectSpace logonObjectSpace;
		protected List<IObjectSpace> objectSpaces;
		protected Type userType;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(logonObjectSpace != null) {
					if(logonObjectSpace is IDisposable) {
						((IDisposable)logonObjectSpace).Dispose();
					}
					logonObjectSpace = null;
				}
				if(objectSpaces != null) {
					foreach(IObjectSpace objectSpace in objectSpaces) {
						if(objectSpace is IDisposable) {
							((IDisposable)objectSpace).Dispose();
						}
					}
					objectSpaces.Clear();
					objectSpaces = null;
				}
			}
			base.Dispose(disposing);
		}
		public SecurityStrategyBase() {
			objectSpaces = new List<IObjectSpace>();
		}
		public SecurityStrategyBase(Type userType, AuthenticationBase authentication) {
			this.userType = userType;
			this.Authentication = authentication;
			if(this.authentication == null) {
				this.authentication = new AuthenticationDummy();
			}
			objectSpaces = new List<IObjectSpace>();
		}
		public void Logon(IObjectSpace objectSpace) { 
			if(authentication == null) {
				throw new InvalidOperationException("authentication is null");
			}
			isAuthenticated = false;
			object currentUser = authentication.Authenticate(objectSpace);
			if(currentUser == null) {
				throw new InvalidOperationException("The Authentication.Authenticate method returned 'null'. It should return an object or raise an exception.");
			}
			if(!userType.IsAssignableFrom(currentUser.GetType())) {
				throw new InvalidCastException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.UnableToCast,
					currentUser.GetType(),
					userType));
			}
			Logon(currentUser);
			isAuthenticated = true;
			logonObjectSpace = objectSpace;
			if(LoggingOn != null) {
				LoggingOnEventArgs loggingOnArgs = new LoggingOnEventArgs(logonObjectSpace, userType, UserId);
				LoggingOn(this, loggingOnArgs);
			}
		}
		public void ClearSecuredLogonParameters() {
			if(authentication == null) {
				throw new InvalidOperationException("authentication is null");
			}
			authentication.ClearSecuredLogonParameters();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void Logon(object user) {
			this.user = user;
		}
		public virtual void Logoff() {
				if(authentication != null) {
					authentication.Logoff();
				}
				logonObjectSpace = null;
				objectSpaces.Clear();
				isAuthenticated = false;
				user = null;
			if(LoggingOff != null) {
				LoggingOff(this, EventArgs.Empty);
			}
		}
		public virtual IList<Type> GetBusinessClasses() {
			List<Type> result = new List<Type>();
			if(userType != null) {
				result.Add(userType);
			}
			if(authentication != null) {
				result = new List<Type>(CollectionsHelper.MergeCollections(result, authentication.GetBusinessClasses()));
			}
			return result;
		}
		public Type GetModuleType() {
			return typeof(DevExpress.ExpressApp.Security.SecurityModule);
		}
		[Browsable(false)]
		public object LogonParameters {
			get {
				if(authentication != null)
					return authentication.LogonParameters;
				else
					return null;
			}
		}
		[Browsable(false)]
		public bool NeedLogonParameters {
			get {
				if(authentication != null)
					return authentication.AskLogonParametersViaUI;
				else
					return false;
			}
		}
		[ReadOnly(true)]
		public virtual bool IsLogoffEnabled {
			get {
				if(authentication != null)
					return authentication.IsLogoffEnabled;
				else
					return false;
			}
		}
		[Browsable(false)]
		public AuthenticationBase Authentication {
			get { return authentication; }
			set {
				authentication = value;
				if(authentication != null) {
					authentication.Security = this;
					authentication.UserType = userType;
				}
			}
		}
		public IList<IObjectSpace> ObjectSpaces {
			get { return objectSpaces; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsAuthenticated {
			get { return isAuthenticated; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object User { get { return user; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string UserName { get { return string.Empty; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object UserId { get { return null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IObjectSpace LogonObjectSpace { get { return logonObjectSpace; } }
		Type ISecurityStrategyBase.UserType {
			get { return userType; }
		}
		protected virtual void OnChanged() {
			if(Changed != null) {
				Changed(this, EventArgs.Empty);
			}
		}
		void ISecurityStrategyBase.ReloadPermissions() {
			ReloadPermissionsCore();
			OnChanged();
		}
		protected abstract void ReloadPermissionsCore();
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<EventArgs> Changed;
		public event EventHandler<LoggingOnEventArgs> LoggingOn;
		public event EventHandler<EventArgs> LoggingOff;
		void ICanInitializeNewUser.InitializeNewUser(IObjectSpace objectSpace, object user) {
			InitializeNewUserCore(objectSpace, user);
		}
		protected virtual void InitializeNewUserCore(IObjectSpace objectSpace, object user) {
		}
		bool ISupportChangePasswordOption.IsSupportChangePassword {
			get { return Authentication != null && typeof(IAuthenticationStandard).IsAssignableFrom(Authentication.GetType()); }
		}
	}
}
