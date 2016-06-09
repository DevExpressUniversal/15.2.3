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
using System.Text;
using DevExpress.ExpressApp.Security;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public interface IRequestSecurityStrategy : IRequestSecurity, ISecurityStrategyBase {
	}
	public abstract class ServerSecurityBase : IServerSecurity {
		protected abstract IRequestSecurityStrategy GetSecurity(IClientInfo clientInfo);
		protected abstract void Logon(IClientInfo clientInfo);
		protected abstract void Logoff(IClientInfo clientInfo);
		#region IServerSecurity
		void IServerSecurity.Logon(IClientInfo clientInfo) {
			Logon(clientInfo);
		}
		void IServerSecurity.Logoff(IClientInfo clientInfo) {
			Logoff(clientInfo);
		}
		bool IServerSecurity.IsGranted(IClientInfo clientInfo, IPermissionRequest permissionRequest) {	  
			return GetSecurity(clientInfo).IsGranted(permissionRequest);
		}
		IList<bool> IServerSecurity.IsGranted(IClientInfo clientInfo, IList<IPermissionRequest> permissionRequests) {	  
			return GetSecurity(clientInfo).IsGranted(permissionRequests);
		}
		object IServerSecurity.GetUserId(IClientInfo clientInfo) {
			return GetSecurity(clientInfo).UserId;
		}
		string IServerSecurity.GetUserName(IClientInfo clientInfo) {
			return GetSecurity(clientInfo).UserName;
		}
		object IServerSecurity.GetLogonParameters(IClientInfo clientInfo) {
			return GetSecurity(clientInfo).LogonParameters;
		}
		bool IServerSecurity.GetNeedLogonParameters(IClientInfo clientInfo) {
			return GetSecurity(clientInfo).NeedLogonParameters;
		}
		bool IServerSecurity.GetIsLogoffEnabled(IClientInfo clientInfo) {
			return GetSecurity(clientInfo).IsLogoffEnabled;
		}
		Type IServerSecurity.GetUserType(IClientInfo clientInfo) {
			return GetSecurity(clientInfo).UserType;
		}
		Type IServerSecurity.GetRoleType(IClientInfo clientInfo) {
			IRoleTypeProvider roleTypeProvider = GetSecurity(clientInfo) as IRoleTypeProvider;
			if(roleTypeProvider != null) {
				return roleTypeProvider.RoleType;
			}
			return null;
		}
		#endregion
	}
}
