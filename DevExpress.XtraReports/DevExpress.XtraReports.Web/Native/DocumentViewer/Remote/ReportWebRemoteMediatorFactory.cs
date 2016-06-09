#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Net;
using System.Web.Security;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.ReportServer.Printing;
using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer.Remote {
	public class ReportWebRemoteMediatorFactory : IReportWebRemoteMediatorFactory {
		#region IReportWebRemoteMediatorFactory
		public IReportWebRemoteMediator Create(DocumentViewerRemoteSourceSettings settings, Func<string> retrieveToken, Func<ITokenStorage> createClientTokenStorage) {
			Guard.ArgumentNotNull(settings, "settings");
			IServiceClientFactory<IReportServiceClient> clientFactory = null;
			InstanceIdentity instanceIdentity = null;
			if(settings.AuthenticationType == AuthenticationType.None) {
				clientFactory = ClientFactory.GetReportServiceClientFactory(settings);
				instanceIdentity = new ReportNameIdentity(settings.ReportTypeName);
			} else {
				clientFactory = CreateFactoryAndRetrieveToken(settings, retrieveToken, createClientTokenStorage);
				instanceIdentity = new ReportIdentity(settings.ReportId);
			}
			return new DocumentViewerReportWebRemoteMediator(clientFactory, instanceIdentity);
		}
		#endregion
		static IServiceClientFactory<IReportServiceClient> CreateFactoryAndRetrieveToken(DocumentViewerRemoteSourceSettings settings, Func<string> retrieveToken, Func<ITokenStorage> createClientTokenStorage) {
			var factory = ClientFactory.GetReportServerClientFactory(settings);
			var tokenStorage = CreateTokenStorage(settings, createClientTokenStorage);
			var cookieValue = tokenStorage.Token;
			if(string.IsNullOrEmpty(cookieValue) && retrieveToken != null) {
				cookieValue = retrieveToken();
				tokenStorage.Token = cookieValue;
			}
			var endpointBehavior = new RSEndpointBehavior(cookieValue);
			endpointBehavior.CookieReceived += (_, e) => tokenStorage.Token = e.CookieValue;
			factory.ChannelFactory.Endpoint.Behaviors.Add(endpointBehavior);
			return factory;
		}
		static ITokenStorage CreateTokenStorage(DocumentViewerRemoteSourceSettings settings, Func<ITokenStorage> createClientTokenStorage) {
			Guard.ArgumentNotNull(settings, "settings");
			Guard.ArgumentNotNull(createClientTokenStorage, "createClientTokenStorage");
			switch(settings.TokenStorageMode) {
				case TokenStorageMode.Client:
					return createClientTokenStorage();
				case TokenStorageMode.Custom:
					if(settings.CustomTokenStorage == null) {
						throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RemoteSourceSettings_CustomTokenStorage_Error));
					}
					return settings.CustomTokenStorage;
				case TokenStorageMode.Session:
					var session = settings.OwnerControl != null ? HttpUtils.GetSession(settings.OwnerControl) : HttpUtils.GetSession();
					return new SessionTokenStorage(session);
				default:
					throw new NotSupportedException("TokenStorageMode: " + settings.TokenStorageMode.ToString());
			}
		}
	}
}
