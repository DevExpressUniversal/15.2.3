#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using DevExpress.Data.Utils;
using DevExpress.Utils.IoC;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using IServiceContainer = System.ComponentModel.Design.IServiceContainer;
namespace DevExpress.XtraReports.Web.Native.ClientControls {
	public static class DefaultContainerHelper {
		public static void Register<T>(T singleton, IServiceProvider serviceProvider, Action<Action<IServiceContainerRegistrator>> emptyServiceProviderRegistration) {
			if(serviceProvider == null) {
				emptyServiceProviderRegistration(x => x.RegisterInstance(singleton));
				return;
			}
			var integrityContainer = serviceProvider as IntegrityContainer;
			if(integrityContainer != null) {
				integrityContainer.RegisterInstance(singleton);
			}
			var serviceContainer = serviceProvider as IServiceContainer;
			if(serviceContainer != null) {
				serviceContainer.AddService(typeof(T), singleton);
				return;
			}
			throw new NotSupportedException(string.Format("Cannot register the '{0}' service in the current service provider.", typeof(T).FullName));
		}
		public static void Register<T, TImpl>(IServiceProvider serviceProvider, Action<Action<IServiceContainerRegistrator>> emptyServiceProviderRegistration)
			where TImpl : T {
			if(serviceProvider == null) {
				emptyServiceProviderRegistration(x => x.RegisterTransient<T, TImpl>());
				return;
			}
			var integrityContainer = serviceProvider as IntegrityContainer;
			if(integrityContainer != null) {
				integrityContainer.RegisterType<T, TImpl>().AsTransient();
			}
			var serviceContainer = serviceProvider as IServiceContainer;
			if(serviceContainer != null) {
				serviceContainer.AddService(typeof(T), serviceContainer.GetService<TImpl>());
				return;
			}
			throw new NotSupportedException(string.Format("Cannot register the '{0}' service in the current service provider.", typeof(TImpl).FullName));
		}
	}
}
