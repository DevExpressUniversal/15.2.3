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

using System.ServiceModel;
using System.ServiceModel.Channels;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.ReportServer.Printing;
using DevExpress.Utils;
namespace DevExpress.ReportServer.ServiceModel.Client {
	public class AuthenticationServiceClientFactory : ServiceClientFactory<IAuthenticationServiceClient, IAuthenticationServiceAsync> {
		readonly AuthenticationType authenticationType;
		readonly bool useSSL;
		public AuthenticationServiceClientFactory(string endpointConfigurationName)
			: this(endpointConfigurationName, null) { }
		public AuthenticationServiceClientFactory(string endpointConfigurationName, EndpointAddress remoteAddress)
			: base(endpointConfigurationName, remoteAddress) { }
		public AuthenticationServiceClientFactory(EndpointAddress remoteAddress, Binding binding)
			: base(remoteAddress, binding) {
			Guard.ArgumentNotNull(remoteAddress, "remoteAddress");
		}
		public AuthenticationServiceClientFactory(EndpointAddress address, AuthenticationType authenticationType)
			: base(address) {
			this.authenticationType = authenticationType;
			this.useSSL = address.Uri.Scheme == System.Uri.UriSchemeHttps;
		}
		protected override Binding CreateDefaultBinding(EndpointAddress remoteAddress) {
			var security = new BasicHttpSecurity();
			if(authenticationType == AuthenticationType.Windows) {
				security.Mode = useSSL ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportCredentialOnly;
				security.Transport = new HttpTransportSecurity { ClientCredentialType = HttpClientCredentialType.Windows };
			} else {
				security.Mode = useSSL ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None;
			}
			return new BasicHttpBinding { Security = security };
		}
		public override IAuthenticationServiceClient Create() {
			return new AuthenticationServiceClient(ChannelFactory.CreateChannel());
		}
	}
}
