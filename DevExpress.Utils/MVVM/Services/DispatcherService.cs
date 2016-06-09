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

namespace DevExpress.Utils.MVVM.Services {
	using System;
	using System.Threading;
	public class DispatcherService {
		SynchronizationContext context;
		protected DispatcherService() {
			context = SynchronizationContext.Current;
		}
		public static DispatcherService Create() {
			IMVVMServiceTypesResolver typesResolver = MVVMTypesResolver.Instance as IMVVMServiceTypesResolver;
			return DynamicServiceSource.Create<DispatcherService>(new Type[] { 
				typesResolver.GetIDispatcherServiceType()});
		}
		public void BeginInvoke(Action action) {
			if(action != null && context != null)
				context.Post((s) => action(), null);
		}
		internal static IDisposable Register(IPOCOInterfaces pocoInterfaces) {
			var dispatcherService = Create();
			object serviceContainer = pocoInterfaces.GetDefaultServiceContainer();
			pocoInterfaces.RegisterService(serviceContainer, dispatcherService);
			return dispatcherService
				.GetDisposableToken(pocoInterfaces, serviceContainer);
		}
		#region Unregister
		IDisposable GetDisposableToken(IPOCOInterfaces pocoInterfaces, object serviceContainer) {
			return new DisposableToken(this, pocoInterfaces, serviceContainer);
		}
		class DisposableToken : IDisposable {
			DispatcherService service;
			IPOCOInterfaces pocoInterfaces;
			object serviceContainer;
			public DisposableToken(DispatcherService service, IPOCOInterfaces pocoInterfaces, object serviceContainer) {
				this.service = service;
				this.pocoInterfaces = pocoInterfaces;
				this.serviceContainer = serviceContainer;
			}
			void IDisposable.Dispose() {
				if(service != null) {
					service.context = null;
					if(pocoInterfaces != null && serviceContainer != null)
						pocoInterfaces.UnregisterService(serviceContainer, service);
					this.service = null;
					this.pocoInterfaces = null;
					this.serviceContainer = null;
				}
				GC.SuppressFinalize(this);
			}
		}
		#endregion
	}
}
