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
using DevExpress.Utils;
using DevExpress.ExpressApp.MiddleTier;
using DevExpress.Xpo;
using DevExpress.Xpo.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.DB;
using System.ComponentModel;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public class ServerSecurityLogger : LoggerBase, IServerSecurity {
		#region constants
		public const int IsGrantedPermissionRequest = 100;
		public const int LogonCode = 101;
		public const int LogoffCode = 102;
		public const int GetIsAuthenticatedCode = 103;
		public const int GetUserIdCode = 104;
		public const int GetUserNameCode = 105;
		public const int GetLogonParametersCode = 106;
		public const int GetNeedLogonParametersCode = 107;
		public const int GetIsLogoffEnabledCode = 108;
		public const int GetModuleTypeCode = 109;
		public const int GetBusinessClassesCode = 110;
		public const int GetUserTypeCode = 111;
		public const int GetSelectObjectCriteriaCode = 112;
		public const int GetSelectMemberCriteriaCode = 113;
		public const int GetRoleTypeCode = 115;
		public const int CanReadMembersPermissionRequest = 116;
		#endregion
		private readonly IServerSecurity serverSecurity;
		public ServerSecurityLogger(IServerSecurity serverSecurity, ILogger logger) : base(logger) {
			Guard.ArgumentNotNull(serverSecurity, "serverSecurity");
			this.serverSecurity = serverSecurity;
		}
		public void Logon(IClientInfo clientInfo) {
			Execute<object>(delegate() {
				serverSecurity.Logon(clientInfo);
				return null;
			}, "Logon", LogonCode, clientInfo);
		}
		public void Logoff(IClientInfo clientInfo) {
			Execute<object>(delegate() {
				serverSecurity.Logoff(clientInfo);
				return null;
			}, "Logoff", LogoffCode, clientInfo);
		}
		public bool IsGranted(IClientInfo clientInfo, IPermissionRequest permissionRequest) {
			return Execute<bool>(delegate() {
				return serverSecurity.IsGranted(clientInfo, permissionRequest);
			}, "IsGranted", IsGrantedPermissionRequest, clientInfo);
		}
		public IList<bool> IsGranted(IClientInfo clientInfo, IList<IPermissionRequest> permissionRequests) {
			return Execute<IList<bool>>(delegate() {
				return serverSecurity.IsGranted(clientInfo, permissionRequests);
			}, "IsGranted", IsGrantedPermissionRequest, clientInfo);
		}
		public object GetUserId(IClientInfo clientInfo) {
			return Execute<object>(delegate() {
				return serverSecurity.GetUserId(clientInfo);
			}, "GetUserId", GetUserIdCode, clientInfo);
		}
		public string GetUserName(IClientInfo clientInfo) {
			return Execute<string>(delegate() {
				return serverSecurity.GetUserName(clientInfo);
			}, "GetUserName", GetUserNameCode, clientInfo);
		}
		public object GetLogonParameters(IClientInfo clientInfo) {
			return Execute<object>(delegate() {
				return serverSecurity.GetLogonParameters(clientInfo);
			}, "GetLogonParameters", GetLogonParametersCode, clientInfo);
		}
		public bool GetNeedLogonParameters(IClientInfo clientInfo) {
			return Execute<bool>(delegate() {
				return serverSecurity.GetNeedLogonParameters(clientInfo);
			}, "GetNeedLogonParameters", GetLogonParametersCode, clientInfo);
		}
		public bool GetIsLogoffEnabled(IClientInfo clientInfo) {
			return Execute<bool>(delegate() {
				return serverSecurity.GetIsLogoffEnabled(clientInfo);
			}, "GetIsLogoffEnabled", GetIsLogoffEnabledCode, clientInfo);
		}
		public Type GetUserType(IClientInfo clientInfo) {
			return Execute<Type>(delegate() {
				return serverSecurity.GetUserType(clientInfo);
			}, "GetUserType", GetUserTypeCode, clientInfo);
		}
		public Type GetRoleType(IClientInfo clientInfo) {
			return Execute<Type>(delegate() {
				return serverSecurity.GetRoleType(clientInfo);
			}, "GetRoleType", GetRoleTypeCode, clientInfo);
		}
	}
}
