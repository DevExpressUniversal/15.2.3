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
using System.Security;
namespace DevExpress.ExpressApp.Security {
	public interface ISecurityStrategyBase {
		void Logon(IObjectSpace objectSpace);
		void Logoff();
		void ClearSecuredLogonParameters();
		void ReloadPermissions();
		Type GetModuleType(); 
		IList<Type> GetBusinessClasses(); 
		bool IsAuthenticated { get; }
		Type UserType { get; }
		object User { get; }
		string UserName { get; }
		object UserId { get; }
		object LogonParameters { get; }
		bool NeedLogonParameters { get; } 
		bool IsLogoffEnabled { get; } 
		IObjectSpace LogonObjectSpace { get; } 
	}
	public interface ISecurity : ISecurityStrategyBase {
		bool IsGranted(IPermission permission);
	}
	public class LoggingOnEventArgs : EventArgs {
		public IObjectSpace LogonObjectSpace { get; set; }
		public Type UserType { get; set; }
		public object UserID { get; set; }
		public LoggingOnEventArgs(IObjectSpace logonObjectSpace,Type userType,object userID) {
			this.LogonObjectSpace = logonObjectSpace;
			this.UserType = userType;
			this.UserID = userID;
		}
	}
	public interface IExtensibleSecurity {
		bool IsSecurityMember(Type type, string memberName);
		bool IsSecurityMember(object theObject, string memberName);
	}
	public interface ISupportChangedNotification {
		event EventHandler<EventArgs> Changed;
	}
	public interface IRequestSecurity {
		bool IsGranted(IPermissionRequest permissionRequest);
		IList<bool> IsGranted(IList<IPermissionRequest> permissionRequests);
	}
	public interface IPermissionRequest {
		object GetHashObject(); 
	}
	public interface IOperationPermission { 
		string Operation { get; }
	}
	public interface ITypeOperationPermission : IOperationPermission {
		Type ObjectType { get; }
	}
}
