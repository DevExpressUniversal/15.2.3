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
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
namespace DevExpress.XtraReports.UserDesigner.Native
{
	public class XRServiceContainer : IServiceContainer, IDisposable 
	{
		IServiceContainer serviceContainer;
		public XRServiceContainer() {
			this.serviceContainer = new ServiceContainer();
		}
		public XRServiceContainer(IServiceProvider provider) {
			this.serviceContainer = new ServiceContainer(provider);
		}
		public XRServiceContainer(IServiceContainer parent) {
			this.serviceContainer = new ServiceContainer(parent);
		}
		#region System.ComponentModel.Design.IServiceContainer interface implementation
		public void AddService(System.Type serviceType, object serviceInstance) {
			if (IsServiceMissing(serviceType))
				serviceContainer.AddService(serviceType, serviceInstance);
		}
		public void AddService(System.Type serviceType, object serviceInstance, bool promote) {
			if (IsServiceMissing(serviceType))
				serviceContainer.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(System.Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback) {
			if (IsServiceMissing(serviceType))
				serviceContainer.AddService(serviceType, callback);
		}
		public void AddService(System.Type serviceType, System.ComponentModel.Design.ServiceCreatorCallback callback, bool promote) {
			if (IsServiceMissing(serviceType))
				serviceContainer.AddService(serviceType, callback, promote);
		}
		public void RemoveService(System.Type serviceType) {
			serviceContainer.RemoveService(serviceType);
		}
		public void RemoveService(System.Type serviceType, bool promote) {
			serviceContainer.RemoveService(serviceType, promote);
		}
		#endregion
		#region System.IServiceProvider interface implementation
		public object GetService(System.Type serviceType) {
			return serviceContainer.GetService(serviceType);
		}
		#endregion
		bool IsServiceMissing(Type serviceType) {
			return serviceContainer.GetService(serviceType) == null;
		}
		#region IDisposable Members
		public void Dispose() {
			Type serviceType = typeof(System.Windows.Forms.Design.ComponentTray).Assembly.GetType("System.Windows.Forms.Design.ISelectionUIService");
			if(serviceContainer != null && serviceType != null) {
				IDisposable serv = serviceContainer.GetService(serviceType) as IDisposable;
				if(serv != null) {
					serviceContainer.RemoveService(serviceType);
					serv.Dispose();
				}
			}
		}
		#endregion
	}
}
