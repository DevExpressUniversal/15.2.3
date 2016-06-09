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

using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System;
using System.Net;
using System.Collections.Concurrent;
namespace DevExpress.ReportServer.ServiceModel.Client {
	public class FormsAuthenticationEndpointBehavior : IEndpointBehavior {
		readonly ConcurrentDictionary<string, FormsAuthenticationMessageInspector> messageInspectors;
		public FormsAuthenticationEndpointBehavior() {
			messageInspectors = new ConcurrentDictionary<string, FormsAuthenticationMessageInspector>();
		}
		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime) {
			FormsAuthenticationMessageInspector currentInspector;
			if(!messageInspectors.TryGetValue(endpoint.Address.Uri.Host, out currentInspector)) {
				currentInspector = CreateCurrentInspector();
				messageInspectors.TryAdd(endpoint.Address.Uri.Host, currentInspector);
			}
			clientRuntime.MessageInspectors.Add(currentInspector);
		}
		protected virtual FormsAuthenticationMessageInspector CreateCurrentInspector() {
			return new FormsAuthenticationMessageInspector();
		}
		#region Unused
		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) {
		}
		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher) {
		}
		public void Validate(ServiceEndpoint endpoint) {
		}
		#endregion
	}
}
