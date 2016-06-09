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
using System.Collections.Generic;
using System.ComponentModel.Design;
using DevExpress.Compatibility.System.ComponentModel.Design;
namespace DevExpress.Services {
	public interface IProgressIndicationService {
		void Begin(string displayName, int minProgress, int maxProgress, int currentProgress);
		void SetProgress(int currentProgress);
		void End();
	}
}
namespace DevExpress.Services.Internal {
	public interface IContainerComponent {
		object Component { get; }
	}
	public class ServiceManager : IServiceContainer, IDisposable {
		bool isDisposed;
		Dictionary<Type, object> services = new Dictionary<Type, object>();
		public virtual bool IsServiceExists(Type serviceType) {
			return Services.ContainsKey(serviceType);
		}
		public Dictionary<Type, object> Services { get { return services; } }
		public bool IsDisposed { get { return isDisposed; } }
		public event EventHandler ServiceListChanged;
		protected internal virtual void RaiseServiceListChanged() {
			if (ServiceListChanged != null)
				ServiceListChanged(this, EventArgs.Empty);
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (services != null) {
					foreach (object instance in services.Values) {
						IDisposable disposable = instance as IDisposable;
						if (disposable != null)
							disposable.Dispose();
					}
					this.services = null;
				}
			}
			this.isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region IServiceProvider Members
		public object GetService(Type serviceType) {
			object result;
			if (Services.TryGetValue(serviceType, out result)) {
				ServiceCreatorCallback callback = result as ServiceCreatorCallback;
				if (callback != null) {
					result = callback(this, serviceType);
					if (result != null)
						Services[serviceType] = result;
				}
				return result;
			}
			else
				return null;
		}
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if (!IsServiceExists(serviceType)) {
				Services.Add(serviceType, callback);
				RaiseServiceListChanged();
			}
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if (!IsServiceExists(serviceType)) {
				Services.Add(serviceType, callback);
				RaiseServiceListChanged();
			}
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if (!IsServiceExists(serviceType)) {
				Services.Add(serviceType, serviceInstance);
				RaiseServiceListChanged();
			}
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if (!IsServiceExists(serviceType)) {
				Services.Add(serviceType, serviceInstance);
				RaiseServiceListChanged();
			}
		}
		public void RemoveService(Type serviceType, bool promote) {
			if (IsServiceExists(serviceType)) {
				Services.Remove(serviceType);
				RaiseServiceListChanged();
			}
		}
		public void RemoveService(Type serviceType) {
			if (IsServiceExists(serviceType)) {
				Services.Remove(serviceType);
				RaiseServiceListChanged();
			}
		}
		#endregion
	}
}
