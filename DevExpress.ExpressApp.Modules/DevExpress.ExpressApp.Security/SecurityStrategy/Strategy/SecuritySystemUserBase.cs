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

using DevExpress.Persistent.Base;
using DevExpress.Persistent.Base.Security;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security.Strategy {
	[ImageName("BO_User"), System.ComponentModel.DefaultProperty("UserName"), Persistent("SecuritySystemUser")]
	public abstract class SecuritySystemUserBase : XPCustomObject, ISecurityUser, IAuthenticationActiveDirectoryUser, IAuthenticationStandardUser {
		private string userName = String.Empty;
		private bool isActive = true;
		private string storedPassword;
		private bool changePasswordOnFirstLogon = false;
		[Browsable(false)]
		[Size(SizeAttribute.Unlimited)]
		[Persistent]
		[SecurityBrowsable]
		protected string StoredPassword {
			get { return storedPassword; }
			set { SetPropertyValue("StoredPassword", ref storedPassword, value); }
		}
		public SecuritySystemUserBase(Session session) : base(session) { }
#if MediumTrust
		private Guid oid = Guid.Empty;
		[Browsable(false), Key(true), NonCloneable]
		public Guid Oid {
			get { return oid; }
			set { oid = value; }
		}
#else
		[Persistent("Oid"), Key(true), Browsable(false), MemberDesignTimeVisibility(false)]
		private Guid oid = Guid.Empty;
		[PersistentAlias("oid"), Browsable(false)]
		public Guid Oid {
			get { return oid; }
		}
#endif
		public bool ComparePassword(string password) {
			return SecurityUserBase.ComparePassword(this.storedPassword, password);
		}
		public void SetPassword(string password) {
			StoredPassword = SecurityUserBase.GeneratePassword(password);
		}
		public bool ChangePasswordOnFirstLogon {
			get { return changePasswordOnFirstLogon; }
			set { SetPropertyValue("ChangePasswordOnFirstLogon", ref changePasswordOnFirstLogon, value); }
		}
		[RuleRequiredField("SecuritySystemUser_UserName_RuleRequiredField", DefaultContexts.Save)]
		[RuleUniqueValue("SecuritySystemUser_UserName_RuleUniqueValue", DefaultContexts.Save, "The login with the entered user name was already registered within the system")]
		public string UserName {
			get { return userName; }
			set { SetPropertyValue("UserName", ref userName, value); }
		}
		public bool IsActive {
			get { return isActive; }
			set { SetPropertyValue("IsActive", ref isActive, value); }
		}
	}
}
