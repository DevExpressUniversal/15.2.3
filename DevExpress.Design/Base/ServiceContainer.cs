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

namespace DevExpress.Design.UI {
	using System;
	using System.Collections.Generic;
	public class ServiceContainer : IServiceContainer {
		protected IServiceContainer parentContainer;
		public ServiceContainer(IServiceContainer parentContainer) {
			this.parentContainer = parentContainer;
		}		
		protected IDictionary<Type, Func<object>> serviceInitializers;
		public void Register<Service, ServiceProvider>(Func<ServiceProvider> initializer)
			where ServiceProvider : Service {
			AssertionException.IsNotNull(initializer);
			Register(typeof(Service), () => initializer());
		}
		public void Register<Service>(Func<Service> initializer){
			AssertionException.IsNotNull(initializer);
			Register(typeof(Service), () => initializer());
		}
		public void Register<Service, ServiceProvider>()
			where ServiceProvider : Service, new() {
			Register(typeof(Service), () => new ServiceProvider());
		}
		void Register(Type key, Func<object> initializer) {
			if(serviceInitializers == null)
				serviceInitializers = new Dictionary<Type, Func<object>>();
			serviceInitializers[key] = initializer;
		}
		public virtual Service Resolve<Service>() {
			if(serviceInitializers != null) {
				Func<object> initializer;
				if(serviceInitializers.TryGetValue(typeof(Service), out initializer))
					return (Service)initializer();
			}
			if(parentContainer != null)
				return parentContainer.Resolve<Service>();
			throw new ResolveServiceException<Service>();
		}
		protected class ResolveServiceException<Service> : Exception {
			public ResolveServiceException()
				: base(typeof(Service).ToString() + " service is not registered") {
			}
		}		
	}	
}
