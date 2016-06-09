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
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
namespace DevExpress.Data.Utils.ServiceModel {
	public abstract class ServiceClientFactory<TClient, TChannel> : IServiceClientFactory<TClient> {
		#region Fields & Properties
		public const int DefaultBindingBufferSizeLimit = 4 * 1024 * 1024;
		Lazy<ChannelFactory<TChannel>> lazyChannelFactory;
		public ChannelFactory<TChannel> ChannelFactory {
			get { return lazyChannelFactory.Value; }
		}
		public ClientCredentials Credentials {
			get { return ChannelFactory.Credentials; }
		}
		#endregion
		#region ctor
		public ServiceClientFactory(string endpointConfigurationName)
			: this(endpointConfigurationName, null) {
		}
		public ServiceClientFactory(string endpointConfigurationName, EndpointAddress remoteAddress) {
			lazyChannelFactory = new Lazy<ChannelFactory<TChannel>>(() => new ChannelFactory<TChannel>(endpointConfigurationName, remoteAddress));
		}
		public ServiceClientFactory(EndpointAddress remoteAddress)
			: this(remoteAddress, null) {
		}
		public ServiceClientFactory(EndpointAddress remoteAddress, Binding binding) {
			lazyChannelFactory = new Lazy<ChannelFactory<TChannel>>(() => new ChannelFactory<TChannel>(binding ?? CreateDefaultBinding(remoteAddress), remoteAddress));
		}
		#endregion
		public abstract TClient Create();
		protected virtual Binding CreateDefaultBinding(EndpointAddress remoteAddress) {
			var securityMode = remoteAddress.Uri.Scheme == Uri.UriSchemeHttps ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.None;
			var binding = new BasicHttpBinding(securityMode) {
				MaxBufferSize = DefaultBindingBufferSizeLimit,
				MaxReceivedMessageSize = int.MaxValue,
				TransferMode = TransferMode.StreamedResponse
			};
#if !SILVERLIGHT
			binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
			binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
#endif
			return binding;
		}
	}
}
