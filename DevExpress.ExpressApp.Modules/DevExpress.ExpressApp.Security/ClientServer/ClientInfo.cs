﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Helpers;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
namespace DevExpress.ExpressApp.Security.ClientServer {
	public interface IClientInfo {
		object ClientId { get; }
		object WorkspaceId { get; }
		object LogonParameters { get; } 
	}
	public interface IClientInfoFactory {
		IClientInfo CreateClientInfo(object logonParameters);
	}
	[Serializable]
	public class AnonymousLogonParameters {
		public static readonly object Instance = new AnonymousLogonParameters();
		public override bool Equals(object obj) {
			if(obj != null && obj.GetHashCode() == GetHashCode()) {
				return true;
			}
			return false;
		}
		public override int GetHashCode() {
			return typeof(AnonymousLogonParameters).GetHashCode();
		}
	}
	[Serializable]
	public class ClientInfo : IClientInfo {
		public ClientInfo(Guid clientId, Guid workspaceId, object logonParameters) {
			this.ClientId = clientId;
			this.WorkspaceId = workspaceId;
			this.LogonParameters = logonParameters;
		}
		public object ClientId {
			get;
			set; 
		}
		public object WorkspaceId {
			get;
			set; 
		}
		public object LogonParameters {
			get;
			set; 
		}
	}
	public class ClientInfoFactory : IClientInfoFactory {
		public readonly Guid ClientId = Guid.NewGuid();
		public IClientInfo CreateClientInfo(object logonParameters) {
			return new ClientInfo(ClientId, Guid.NewGuid(), logonParameters);
		}
	}
}
