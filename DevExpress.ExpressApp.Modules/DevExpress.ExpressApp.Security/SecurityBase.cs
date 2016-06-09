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
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Localization;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Xpo.Helpers;
namespace DevExpress.ExpressApp.Security {
	public abstract class SecurityBase : SecurityStrategyBase, ISecurity, IExtensibleSecurity {
		private const bool isGrantedForNonExistentPermissionDefaultValue = false;
		private bool isGrantedForNonExistentPermission = isGrantedForNonExistentPermissionDefaultValue;
		private PermissionSet permissionSet;
		protected abstract PermissionSet ReloadPermissions();
		public SecurityBase() { }
		public SecurityBase(Type userType, AuthenticationBase authentication) : base(userType, authentication) {
		}
		public virtual bool IsGranted(IPermission permission) {
			if(permission == null || User == null) {
				return true;
			}
			if(permissionSet == null) {
				((ISecurityStrategyBase)this).ReloadPermissions();
			}
			IPermission currentPermission = permissionSet.GetPermission(permission.GetType());
			if(currentPermission != null) {
				return permission.IsSubsetOf(currentPermission);
			}
			return IsGrantedForNonExistentPermission;
		}
		public virtual bool IsSecurityMember(object theObject, string memberName) {
			return IsSecurityMember(theObject.GetType(), memberName);
		}
		public virtual bool IsSecurityMember(Type type, string memberName) {
			if(Authentication != null) {
				return Authentication.IsSecurityMember(type, memberName);
			}
			return false;
		}
		protected override void ReloadPermissionsCore() {
			if(User != null) {
				if(LogonObjectSpace != null) {
					LogonObjectSpace.ReloadObject(User);
				}
				else if(User is ISessionProvider) { 
					((ISessionProvider)User).Session.Reload(User);
				}
				permissionSet = ReloadPermissions();
			}
			else {
				permissionSet = new PermissionSet(PermissionState.None);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PermissionSet PermissionSet {
			get {
				if(permissionSet == null) {
					((ISecurityStrategyBase)this).ReloadPermissions();
				}
				return permissionSet;
			}
		}
		[DefaultValue(isGrantedForNonExistentPermissionDefaultValue)]
		public bool IsGrantedForNonExistentPermission {
			get { return isGrantedForNonExistentPermission; }
			set { isGrantedForNonExistentPermission = value; }
		}
	}
}
