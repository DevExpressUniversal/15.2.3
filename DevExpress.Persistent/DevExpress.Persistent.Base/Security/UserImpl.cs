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

using System.Security;
using System.Collections.Generic;
namespace DevExpress.Persistent.Base.Security {
	public class UserImpl {
		private object owner;
		private string userName;
		private string storedPassword;
		private string fullName;
		private bool isActive = true;
		private bool isAdministrator = false;
		private bool changePasswordAfterLogon = false;
		public UserImpl(object owner) {
			this.owner = owner;
		}
		public static bool ComparePassword(string storedPassword, string password) {
			return new PasswordCryptographer().AreEqual(storedPassword, password);
		}
		public static string GeneratePassword(string password) {
			return new PasswordCryptographer().GenerateSaltedPassword(password);
		}
		public static IList<IPermission> GetUserPermissions(object user) {
			List<IPermission> result = new List<IPermission>();
			IUserWithRoles userWithRoles = user as IUserWithRoles;
			if(userWithRoles != null) {
				foreach(IRole role in userWithRoles.Roles) {
					result.AddRange(role.Permissions);
				}
			}
			return result.AsReadOnly();
		}
		public bool ComparePassword(string password) {
			return ComparePassword(this.storedPassword, password);
		}
		public void SetPassword(string password) {
			this.storedPassword = GeneratePassword(password);
		}
		public string UserName {
			get { return userName; }
			set { userName = value; }
		}
		public string FullName {
			get { return fullName; }
			set { fullName = value; }
		}
		public bool IsActive {
			get { return isActive; }
			set { isActive = value; }
		}
		public bool IsAdministrator {
			get { return isAdministrator; }
			set { isAdministrator = value; }
		}
		public bool ChangePasswordAfterLogon {
			get { return changePasswordAfterLogon; }
			set { changePasswordAfterLogon = value; }
		}
		public string StoredPassword {
			get { return storedPassword; }
			set { storedPassword = value; }
		}
		public IList<IPermission> Permissions {
			get {
				return GetUserPermissions(owner);
			}
		}
	}
}
