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

using DevExpress.Services;
using DevExpress.Services.Implementation;
using DevExpress.Xpf.Scheduler.Services.Internal;
using DevExpress.XtraScheduler.Services;
using DevExpress.XtraScheduler.Services.Implementation;
using DevExpress.XtraScheduler.Services.Internal;
using System;
using System.ComponentModel.Design;
namespace DevExpress.Xpf.Scheduler {
	public partial class SchedulerControl {
		protected virtual void AddServices() {
			AddService(typeof(IMouseHandlerService), new MouseHandlerService(MouseHandler));
			AddService(typeof(IKeyboardHandlerService), new SchedulerKeyboardHandlerService(InnerControl));
			SchedulerStateService stateService = new SchedulerStateService();
			AddService(typeof(ISchedulerStateService), stateService);
			AddService(typeof(ISetSchedulerStateService), stateService);
			AddService(typeof(IFileOperationService), new FileOperationService(this));
		}
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if (innerControl != null)
				innerControl.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if (innerControl != null)
				innerControl.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if (innerControl != null)
				innerControl.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if (innerControl != null)
				innerControl.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if (innerControl != null)
				innerControl.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if (innerControl != null)
				innerControl.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public virtual T GetService<T>() {
			return (T)GetService(typeof(T));
		}
		public virtual object GetService(Type serviceType) {
			if (innerControl != null)
				return innerControl.GetService(serviceType);
			else
				return null;
		}
		#endregion
		public T ReplaceService<T>(T newService) where T : class {
			if (this.innerControl != null)
				return innerControl.ReplaceService<T>(newService);
			else
				return null;
		}
	}
}
