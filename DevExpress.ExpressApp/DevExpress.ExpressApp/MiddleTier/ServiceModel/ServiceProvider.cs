#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
namespace DevExpress.ExpressApp.MiddleTier {
	public class ServiceProvider : IServiceProviderEx {
		private List<object> services = new List<object>();
		private void Service_CustomHandleException(object sender, CustomHandleExceptionEventArgs e) {
			RaiseCustomHandleException(sender, e);
		}
		private void RaiseCustomHandleException(object sender, CustomHandleExceptionEventArgs e) {
			CustomHandleServiceExceptionEventArgs args = new CustomHandleServiceExceptionEventArgs(sender, e.Exception);
			if(CustomHandleException != null) {
				if(!e.Exception.Data.Contains("ServiceProvider.Service_CustomHandleException.sender")) {
					e.Exception.Data.Add("ServiceProvider.Service_CustomHandleException.sender", sender.GetType().FullName);
				}
				CustomHandleException(this, args);
			}
			e.Handled = args.Handled;
		}
		public ServiceProvider() {}
		public void AddServices(IEnumerable<object> services) {
			foreach(object service in services) {
				AddService(service);
			}
		}
		public virtual void AddService(object service) {
			services.Add(service);
			if(service is IService) {
				((IService)service).Initialize(this);
				((IService)service).CustomHandleException += new EventHandler<CustomHandleExceptionEventArgs>(Service_CustomHandleException);
			}
		}
		public object GetService(Type serviceType) {
			return services.Find(item => serviceType.IsAssignableFrom(item.GetType()));
		}
		public T GetService<T>() {
			return (T)GetService(typeof(T));
		}
		public IList<object> Services { get { return services; } }
		public event EventHandler<CustomHandleServiceExceptionEventArgs> CustomHandleException;
	}
}
