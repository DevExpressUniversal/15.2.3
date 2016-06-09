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
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.Utils;
namespace DevExpress.DocumentServices.ServiceModel.Native {
	public class ServiceClientCreator<TClient, TFactory> where TFactory : IServiceClientFactory<TClient> {
		readonly Func<EndpointAddress, TFactory> createFactory;
		TFactory factory;
		string uri;
		public string ServiceUri {
			get { return uri; }
			set {
				if(!string.IsNullOrEmpty(value) && Factory != null)
					throw new InvalidOperationException("Use either service uri or client factory, but not both of them.");
				uri = value;
			}
		}
		public TFactory Factory {
			get { return factory; }
			set {
				if(value != null && !string.IsNullOrEmpty(ServiceUri))
					throw new InvalidOperationException("Use either service uri or client factory, but not both of them.");
				factory = value;
			}
		}
		public bool CanCreateClient {
			get { return !string.IsNullOrEmpty(ServiceUri) || Factory != null; }
		}
		public ServiceClientCreator(Func<EndpointAddress, TFactory> createFactory) {
			Guard.ArgumentNotNull(createFactory, "createFactory");
			this.createFactory = createFactory;
		}
		public TClient Create() {
			if(!CanCreateClient)
				throw new InvalidOperationException("Cannot create a service client object because neither service uri nor service client factory is set.");
			TFactory theFactory = Factory != null ? Factory : createFactory(new EndpointAddress(ServiceUri));
			return theFactory.Create();
		}
	}
}
