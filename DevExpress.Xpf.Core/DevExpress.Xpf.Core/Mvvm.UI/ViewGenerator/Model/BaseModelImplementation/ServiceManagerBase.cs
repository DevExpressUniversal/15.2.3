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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.Mvvm.UI.Native.ViewGenerator.Model {
	public sealed class ServiceManagerBase : IServiceProvider, System.ComponentModel.Design.IServiceContainer {
		readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
		readonly List<IServiceProvider> innerProviders = new List<IServiceProvider>();
		public bool Contains(Type serviceType) {
			Guard.ArgumentNotNull(serviceType, "serviceType");
			return services.ContainsKey(serviceType);
		}
		public object GetService(Type serviceType) {
			Guard.ArgumentNotNull(serviceType, "serviceType");
			object service;
			if(!services.TryGetValue(serviceType, out service)) {
				foreach(IServiceProvider provider in innerProviders) {
					service = provider.GetService(serviceType);
					if(service != null) return service;
				}
				return null;
			}
			Func<object> publishServiceCallback = service as Func<object>;
			if(publishServiceCallback != null) {
				service = publishServiceCallback();
				services[serviceType] = service;
			}
			return service;
		}
		public void Publish<TService>(Func<TService> serviceCallback) {
			Guard.ArgumentNotNull(serviceCallback, "serviceCallback");
			services[typeof(TService)] = (Func<object>)(() => serviceCallback());
		}
		public void Publish<TService>(object serviceInstance) {
			Guard.ArgumentNotNull(serviceInstance, "serviceInstance");
			services[typeof(TService)] = serviceInstance;
		}
		public void Publish(Type serviceType, Func<object> serviceCallback) {
			Guard.ArgumentNotNull(serviceType, "serviceType");
			Guard.ArgumentNotNull(serviceCallback, "serviceCallback");
			services[serviceType] = serviceCallback;
		}
		public void Publish(Type serviceType, object serviceInstance) {
			Guard.ArgumentNotNull(serviceType, "serviceType");
			Guard.ArgumentNotNull(serviceInstance, "serviceInstance");
			services[serviceType] = serviceInstance;
		}
		public void AddInnerProvider(IServiceProvider provider) {
			Guard.ArgumentNotNull(provider, "provider");
			innerProviders.Add(provider);
		}
		#region System.ComponentModel.Design.IServiceContainer
		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, object serviceInstance) {
			Publish(serviceType, serviceInstance);
		}
		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote) {
			Publish(serviceType, serviceInstance);
		}
		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback) {
			Publish(serviceType, () => callback(this, serviceType));
		}
		void System.ComponentModel.Design.IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			Publish(serviceType, () => callback(this, serviceType));
		}
		void System.ComponentModel.Design.IServiceContainer.RemoveService(Type serviceType) {
			throw new NotSupportedException();
		}
		void System.ComponentModel.Design.IServiceContainer.RemoveService(Type serviceType, bool promote) {
			throw new NotSupportedException();
		}
		#endregion
	}
}
