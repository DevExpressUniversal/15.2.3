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

using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.DocumentServices.ServiceModel.Client;
using DevExpress.ReportServer.ServiceModel.DataContracts;
using DevExpress.Xpf.Printing;
namespace DevExpress.ReportServer.ServiceModel.Client {
	[TypeConverter(typeof(ServiceClientFactoryConverter))]
	public class ReportServerClientFactory : ServiceClientFactory<IReportServerClient, IReportServerFacadeAsync>, IReportServiceClientFactory {
		readonly string endpointConfigurationName;
		readonly string endpointAddress;
		readonly bool useSSL;
		internal string EndpointConfigurationName { get { return endpointConfigurationName; } }
		internal string EndpointAddress { get { return endpointAddress; } }
		static ReportServerClientFactory() {
			ServiceKnownTypeProvider.Register<TransientReportId>();
			ServiceKnownTypeProvider.Register<ReportIdentity>();
			ServiceKnownTypeProvider.Register<ReportLayoutRevisionIdentity>();
			ServiceKnownTypeProvider.Register<GeneratedReportIdentity>();
		}
		public ReportServerClientFactory(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress) {
			this.endpointConfigurationName = endpointConfigurationName;
			this.endpointAddress = remoteAddress.Uri.OriginalString;
		}
		public ReportServerClientFactory(string endpointConfigurationName) : base(endpointConfigurationName) {
			this.endpointConfigurationName = endpointConfigurationName;
		}
		public ReportServerClientFactory(EndpointAddress remoteAddress, Binding binding) : base(remoteAddress, binding) {
			this.endpointAddress = remoteAddress.Uri.OriginalString;
		}
		public ReportServerClientFactory(EndpointAddress remoteAddress) : base(remoteAddress) {
			this.endpointAddress = remoteAddress.Uri.OriginalString;
			this.useSSL = remoteAddress.Uri.Scheme == System.Uri.UriSchemeHttps;
		}
		public override IReportServerClient Create() {
			return new ReportServerClient(ChannelFactory.CreateChannel());
		}
		IReportServiceClient IServiceClientFactory<IReportServiceClient>.Create() {
			return Create();
		}
	}
}
namespace DevExpress.ReportServer.ServiceModel.Client {
	using System;
	using System.ComponentModel.Design.Serialization;
	using System.Globalization;
	using System.Reflection;
	public class ServiceClientFactoryConverter : TypeConverter {
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			if(destinationType == typeof(InstanceDescriptor) && value is ReportServerClientFactory) {
				var factory = (ReportServerClientFactory)value;
				ConstructorInfo constructor = typeof(ReportServerClientFactory).GetConstructor(new Type[] { typeof(string) });
				if(constructor != null) {
					return new InstanceDescriptor(constructor, new object[] { factory.EndpointConfigurationName });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
