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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Services.Internal;
using System.ComponentModel.Design;
namespace DevExpress.Design.Mvvm.Wizards.UI {
	public class ServiceCache : ServiceManager, DevExpress.Design.UI.IServiceContainer {
		DevExpress.Design.UI.IServiceContainer parentContainer;
		public ServiceCache(){		   
		}
		public ServiceCache(DevExpress.Design.UI.IServiceContainer parentContainer) {
			this.parentContainer = parentContainer;
		}	   
		#region IServiceContainer Members
		public void Register<Service, ServiceProvider>() where ServiceProvider : Service, new() {
		   AddService(typeof(Service), new  ServiceCreatorCallback( (System.ComponentModel.Design.IServiceContainer container, Type type)=> new ServiceProvider()));
		}
		public void Register<Service, ServiceProvider>(Func<ServiceProvider> initializer) where ServiceProvider : Service {
			AddService(typeof(Service), new ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type type) => initializer()));
		}
		public void Register<Service>(Func<Service> initializer) {
			AddService(typeof(Service), new ServiceCreatorCallback((System.ComponentModel.Design.IServiceContainer container, Type type) => initializer()));
		}
		public Service Resolve<Service>() {
			if(base.IsServiceExists(typeof(Service)))
				return (Service)GetService(typeof(Service));
			if (parentContainer != null)
				return parentContainer.Resolve<Service>();
			return default(Service);
		}
		#endregion
	}
}
