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
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Xpo;
namespace DevExpress.Persistent.BaseImpl {
	[System.ComponentModel.DefaultProperty("UserName")]
	[ImageName("BO_User"), System.ComponentModel.DisplayName("User")]
	public class SimpleUser : BaseObject, ISimpleUser, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser {
		private UserImpl userImpl;
		private UserImpl user {
			get {
				if(userImpl == null)
					userImpl = new UserImpl(this);
				return userImpl;
			}
		}
		public SimpleUser(Session session) : base(session) { }
		public bool ComparePassword(string password) {
			return user.ComparePassword(password);
		}
		public void SetPassword(string password) {
			user.SetPassword(password);
		}
		public string UserName {
			get { return user.UserName; }
			set {
				string oldValue = user.UserName;
				user.UserName = value;
				OnChanged("UserName", oldValue, user.UserName);
			}
		}
		public string FullName {
			get { return user.FullName; }
			set {
				string oldValue = user.FullName;
				user.FullName = value;
				OnChanged("FullName", oldValue, user.FullName);
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
		public bool IsAdministrator {
			get { return user.IsAdministrator; }
			set {
				bool oldValue = user.IsAdministrator;
				user.IsAdministrator = value;
				OnChanged("IsAdministrator", oldValue, user.IsAdministrator);
			}
		}
		public bool ChangePasswordOnFirstLogon {
			get { return user.ChangePasswordAfterLogon; }
			set {
				bool oldValue = user.ChangePasswordAfterLogon;
				user.ChangePasswordAfterLogon = value;
				OnChanged("ChangePasswordAfterLogon", oldValue, user.ChangePasswordAfterLogon);
			}
		}
		[Persistent]
		protected string Password {
			get { return user.StoredPassword; }
			set {
				string oldValue = user.StoredPassword;
				user.StoredPassword = value;
				OnChanged("StoredPassword", oldValue, user.StoredPassword);
			}
		}
	}
	[System.ComponentModel.DefaultProperty("UserName")]
	[ImageName("BO_User"), System.ComponentModel.DisplayName("User")]
	public class BasicUser : XPObject, ISimpleUser, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser {
		private UserImpl userImpl;
		private UserImpl user {
			get {
				if(userImpl == null)
					userImpl = new UserImpl(this);
				return userImpl;
			}
		}
		public BasicUser(Session session) : base(session) { }
		public bool ComparePassword(string password) {
			return user.ComparePassword(password);
		}
		public void SetPassword(string password) {
			user.SetPassword(password);
		}
		public string UserName {
			get { return user.UserName; }
			set {
				string oldValue = user.UserName;
				user.UserName = value;
				OnChanged("UserName", oldValue, user.UserName);
			}
		}
		public string FullName {
			get { return user.FullName; }
			set {
				string oldValue = user.FullName;
				user.FullName = value;
				OnChanged("FullName", oldValue, user.FullName);
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
		public bool IsAdministrator {
			get { return user.IsAdministrator; }
			set {
				bool oldValue = user.IsAdministrator;
				user.IsAdministrator = value;
				OnChanged("IsAdministrator", oldValue, user.IsAdministrator);
			}
		}
		public bool ChangePasswordOnFirstLogon {
			get { return user.ChangePasswordAfterLogon; }
			set {
				bool oldValue = user.ChangePasswordAfterLogon;
				user.ChangePasswordAfterLogon = value;
				OnChanged("ChangePasswordAfterLogon", oldValue, user.ChangePasswordAfterLogon);
			}
		}
#if MediumTrust
		[Persistent]
		public string Password {
			get { return user.StoredPassword; }
			set {
				string oldValue = user.StoredPassword;
				user.StoredPassword = value;
				OnChanged("StoredPassword", oldValue, user.StoredPassword);
			}
		}
#else
		[Persistent]
		protected string Password {
			get { return user.StoredPassword; }
			set {
				string oldValue = user.StoredPassword;
				user.StoredPassword = value;
				OnChanged("StoredPassword", oldValue, user.StoredPassword);
			}
		}
#endif
	}
}
