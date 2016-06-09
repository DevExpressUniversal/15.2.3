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
using DevExpress.XtraScheduler.Native;
using DevExpress.Services.Internal;
using System.ComponentModel.Design;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Implementation;
using System.Drawing;
namespace DevExpress.XtraScheduler.Reporting {
	#region InnerServiceProvider
	public class InnerServiceProvider : IServiceContainer, IDisposable {
		ServiceManager serviceManager;
		public InnerServiceProvider() {
			serviceManager = CreateServiceManager();
			AddDefaultServices();
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (serviceManager != null) {
					serviceManager.Dispose();
					serviceManager = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~InnerServiceProvider() {
			Dispose(false);
		}
		#endregion
		protected internal virtual ServiceManager CreateServiceManager() {
			return new ServiceManager();
		}
		protected internal virtual void AddDefaultServices() {
			AddService(typeof(IHeaderCaptionService), new HeaderCaptionService());
			AddService(typeof(IHeaderToolTipService), new HeaderToolTipService());
		}
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			serviceManager.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			serviceManager.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			serviceManager.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			serviceManager.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			serviceManager.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			serviceManager.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public virtual object GetService(Type serviceType) {
			return serviceManager.GetService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			return GetService(serviceType);
		}
		#endregion
	}
	#endregion
}
