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

using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel.Design;
using DevExpress.Data.Browsing.Design;
using DevExpress.Design.VSIntegration;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Utils.UI;
using System.ComponentModel;
namespace DevExpress.DashboardWin.Design {
	class ServicesList : IDisposable {
		readonly IDesignerHost host;
		List<Tuple<object, Type>> services = new List<Tuple<object, Type>>();
		public ServicesList(IDesignerHost host) {
			this.host = host;
			Initialize();
		}
		public void AddService(object service) {
			AddService(service.GetType(), service);
		}
		public void AddService(Type serviceType, object service) {
			host.AddService(serviceType, service);
			services.Add(new Tuple<object, Type>(service, serviceType));
		}
		public void Dispose() {
			foreach(Tuple<object, Type> serviceTypeTuple in services) {
				host.RemoveService(serviceTypeTuple.Item2);
				IDisposable disposableService = serviceTypeTuple.Item1 as IDisposable;
				if(disposableService != null)
					disposableService.Dispose();
			}
			services.Clear();
			services = null;
		}
		void Initialize() {
			AddService(typeof(IDataContextService), new DataContextService());
			AddService(typeof(IDragDropService), new DragDropService(host));
			AddService(typeof(ILookAndFeelService), new VSLookAndFeelService(host));
			if(host != null) {
				DashboardComponentDesigner componentDesigner = host.GetDesigner(host.RootComponent) as DashboardComponentDesigner;
				AddService(new SelectedContextService(componentDesigner.Designer, host));
				DashboardParameterService parameterService = componentDesigner.Designer.ServiceProvider.RequestServiceStrictly<IParameterService>() as DashboardParameterService;
				if(parameterService != null) {
					AddService(typeof(IDashboardParameterService), parameterService);
					AddService(typeof(IParameterService), parameterService);
					AddService(typeof(IParameterCreator), parameterService);					
				}
			}
		   AddService(typeof(IDTEService), new DTEService(host));			
		}
	}
}
