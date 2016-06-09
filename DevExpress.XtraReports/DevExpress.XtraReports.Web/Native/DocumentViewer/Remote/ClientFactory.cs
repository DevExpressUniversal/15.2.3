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
using System.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.ReportServer.Printing;
using DevExpress.ReportServer.ServiceModel.Client;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer.Remote {
	public static class ClientFactory {
		public static AuthenticationServiceClientFactory CreateAuthFactory(DocumentViewerRemoteSourceSettings settings) {
			ValidateSettings(settings);
			var serviceUri = !string.IsNullOrEmpty(settings.ServerUri)
				? GetAuthEndpointUri(settings.ServerUri, settings.AuthenticationType)
				: null;
			var serviceAddress = serviceUri != null
				? new EndpointAddress(serviceUri)
				: null;
			string configurationName = GetAuthEndpointName(settings.EndpointConfigurationName, settings.AuthenticationType, serviceUri);
			if(serviceAddress != null && !string.IsNullOrEmpty(configurationName)) {
				return new AuthenticationServiceClientFactory(configurationName, serviceAddress);
			}
			return serviceAddress != null
				? new AuthenticationServiceClientFactory(serviceAddress, settings.AuthenticationType)
				: new AuthenticationServiceClientFactory(configurationName);
		}
		public static IReportServiceClientFactory GetReportServiceClientFactory(DocumentViewerRemoteSourceSettings settings) {
			ValidateSettings(settings);
			if(!string.IsNullOrEmpty(settings.EndpointConfigurationName) && !string.IsNullOrEmpty(settings.ServerUri)) {
				return new ReportServiceClientFactory(EndpointWithSslPrefix(settings.ServerUri, settings.EndpointConfigurationName), new EndpointAddress(settings.ServerUri));
			}
			return !string.IsNullOrEmpty(settings.EndpointConfigurationName)
				? new ReportServiceClientFactory(EndpointWithSslPrefix(settings.ServerUri, settings.EndpointConfigurationName))
				: new ReportServiceClientFactory(new EndpointAddress(settings.ServerUri));
		}
		public static ReportServerClientFactory GetReportServerClientFactory(DocumentViewerRemoteSourceSettings settings) {
			ValidateSettings(settings);
			if(!string.IsNullOrEmpty(settings.EndpointConfigurationName) && !string.IsNullOrEmpty(settings.ServerUri)) {
				return new ReportServerClientFactory(EndpointWithSslPrefix(settings.ServerUri, settings.EndpointConfigurationName), GetFacadeEndpointAddress(settings.ServerUri));
			}
			return !string.IsNullOrEmpty(settings.EndpointConfigurationName)
				? new ReportServerClientFactory(EndpointWithSslPrefix(settings.ServerUri, settings.EndpointConfigurationName))
				: new ReportServerClientFactory(GetFacadeEndpointAddress(settings.ServerUri));
		}
		static string EndpointWithSslPrefix(Uri uri, string configurationName) {
			return uri != null && uri.Scheme == Uri.UriSchemeHttps
				? configurationName + "_SSL"
				: configurationName;
		}
		static string EndpointWithSslPrefix(string optionalUri, string endpointBehavior) {
			var uri = string.IsNullOrEmpty(optionalUri)
				? null
				: new Uri(optionalUri);
			return EndpointWithSslPrefix(uri, endpointBehavior);
		}
		static EndpointAddress GetFacadeEndpointAddress(string serverUri) {
			var baseUri = new Uri(serverUri, UriKind.Absolute);
			var facadeUri = new Uri(baseUri, "ReportServerFacade.svc");
			return new EndpointAddress(facadeUri);
		}
		static string GetAuthEndpointName(string endpointConfigurationPrefix, AuthenticationType authenticationType, Uri uri) {
			if(string.IsNullOrEmpty(endpointConfigurationPrefix)) {
				return null;
			}
			const string authenticationPattern = "_Authentication";
			const string formsAuthenticationPattern = "_Forms";
			const string winAuthenticationPattern = "_Win";
			var configurationName = endpointConfigurationPrefix;
			configurationName += authenticationPattern;
			if(authenticationType == AuthenticationType.Windows)
				configurationName += winAuthenticationPattern;
			else if(authenticationType == AuthenticationType.Forms)
				configurationName += formsAuthenticationPattern;
			return EndpointWithSslPrefix(uri, configurationName);
		}
		static Uri GetAuthEndpointUri(string uri, AuthenticationType authenticationType) {
			const string winAuthenticationServicePath = "WindowsAuthentication/AuthenticationService.svc";
			const string formsAuthenticationServicePath = "AuthenticationService.svc";
			var baseUri = new Uri(uri, UriKind.Absolute);
			var servicePath = authenticationType == AuthenticationType.Windows
				? winAuthenticationServicePath
				: formsAuthenticationServicePath;
			return new Uri(baseUri, servicePath);
		}
		static void ValidateSettings(DocumentViewerRemoteSourceSettings settings) {
			if(string.IsNullOrEmpty(settings.ServerUri) && string.IsNullOrEmpty(settings.EndpointConfigurationName)) {
				throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RemoteSourceSettings_Error));
			}
		}
	}
}
