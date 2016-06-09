#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.Utils;
using System;
using System.Linq;
using System.ComponentModel.Design;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.ServiceModel.Design {
	public class DashboardComponentSynchronizer : IDisposable {
		readonly IServiceProvider serviceProvider;
		readonly IDesignerHost designerHost;
		public DashboardComponentSynchronizer(IServiceProvider serviceProvider, IDesignerHost designerHost) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			Guard.ArgumentNotNull(designerHost, "designerHost");
			this.serviceProvider = serviceProvider;
			this.designerHost = designerHost;
			IDashboardLoadingService loadingService = serviceProvider.RequestServiceStrictly<IDashboardLoadingService>();
			loadingService.DashboardBeginInitialize += OnDashboardBeginInitialize;
			loadingService.DashboardEndInitialize += OnDashboardEndInitialize;
		}
		public void Dispose() {
			IDashboardLoadingService loadingService = serviceProvider.RequestService<IDashboardLoadingService>();
			if (loadingService != null) {
				loadingService.DashboardBeginInitialize -= OnDashboardBeginInitialize;
				loadingService.DashboardEndInitialize -= OnDashboardEndInitialize;
			}
		}
		void OnDashboardBeginInitialize(object sender, EventArgs e) {
			SynchronizeDashboardComponents();
		}
		void OnDashboardEndInitialize(object sender, EventArgs e) {
			SynchronizeDashboardComponents();
		}
		void SynchronizeDashboardComponents() {
			if (!designerHost.InTransaction) { 
				IDashboardOwnerService ownerService = serviceProvider.RequestServiceStrictly<IDashboardOwnerService>();
				List<IDashboardComponent> oldComponents = designerHost.Container.Components.OfType<IDashboardComponent>().ToList();
				List<IDashboardComponent> newComponents = ownerService.Dashboard.DashboardComponents.ToList();
				foreach (IDashboardComponent component in oldComponents) {
					int index = newComponents.IndexOf(component);
					if (index < 0)
						designerHost.DestroyComponent(component);
					else
						newComponents.RemoveAt(index);
				}
				foreach (IDashboardComponent component in newComponents)
					designerHost.Container.Add(component, component.ComponentName);
			}
		}
	}
}
