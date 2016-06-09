#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Security;
using System.ServiceModel;
using System.Threading.Tasks;
using DevExpress.ReportServer.ServiceModel.Client;
namespace DevExpress.ReportServer.ServiceModel.ConnectionProviders {
	public abstract class ConnectionProvider {
		readonly Uri serverUri;
		readonly FormsAuthenticationEndpointBehavior cookieBehavior;
		readonly Lazy<ReportServerClientFactory> serverClientFactory;
		readonly Lazy<AuthenticationServiceClientFactory> authClientFactory;
		protected FormsAuthenticationEndpointBehavior CookieBehavior { get { return cookieBehavior; } }
		public ConnectionProvider(string serverAddress) {
			serverUri = new Uri(serverAddress, UriKind.Absolute);
			cookieBehavior = new FormsAuthenticationEndpointBehavior();
			serverClientFactory = new Lazy<ReportServerClientFactory>(CreateReportServerClientFactory);
			authClientFactory = new Lazy<AuthenticationServiceClientFactory>(CreateAuthClientFactory);
		}
		public Task<IReportServerClient> ConnectAsync() {
			return LoginAsync()
				.ContinueWith(task => {
					if(!task.Result)
						throw new SecurityException("Invalid user name or password.");
					return CreateClient();
				});
		}
		public abstract Task<bool> LoginAsync();
		public IReportServerClient CreateClient() {
			IReportServerClient client = serverClientFactory.Value.Create();
			return client;
		}
		protected abstract AuthenticationServiceClientFactory CreateAuthClientFactory();
		ReportServerClientFactory CreateReportServerClientFactory() {
			EndpointAddress serviceFacadeAddress = GetEndpointAddress("ReportServerFacade.svc");
			ReportServerClientFactory clientFactory = new ReportServerClientFactory(serviceFacadeAddress);
			clientFactory.ChannelFactory.Endpoint.Behaviors.Add(cookieBehavior);
			return clientFactory;
		}
		protected Task<bool> LoginCoreAsync(string userName, string password) {
			IAuthenticationServiceClient authClient = authClientFactory.Value.Create();
			return authClient.LoginAsync(userName, password, null);
		}
		protected EndpointAddress GetEndpointAddress(string epRelativeAddress) {
			Uri epUri = new Uri(serverUri, epRelativeAddress);
			return new EndpointAddress(epUri);
		}
	}
}
