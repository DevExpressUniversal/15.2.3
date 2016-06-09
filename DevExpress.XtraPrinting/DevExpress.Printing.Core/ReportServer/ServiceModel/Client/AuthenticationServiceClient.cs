﻿#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using DevExpress.Data.Utils.ServiceModel;
namespace DevExpress.ReportServer.ServiceModel.Client {
	public class AuthenticationServiceClient : ServiceClientBase, IAuthenticationServiceClient {
		protected new IAuthenticationServiceAsync Channel {
			get {
				return (IAuthenticationServiceAsync)base.Channel;
			}
		}
		public AuthenticationServiceClient(IAuthenticationServiceAsync channel) : base((IChannel)channel) { }
		public void Login(string userName, string password, object asyncState, Action<ScalarOperationCompletedEventArgs<bool>> onCompleted) {
			AsyncCallback callback = ar => EndScalarOperation(ar, Channel.EndLogin, () => (s, args) => onCompleted(args));
			Channel.BeginLogin(userName, password, callback, asyncState);
		}
		public Task<bool> LoginAsync(string userName, string password, object asyncState) {
			return Task.Factory.FromAsync<string, string, bool>(Channel.BeginLogin, Channel.EndLogin, userName, password, asyncState);
		}
	}
}
