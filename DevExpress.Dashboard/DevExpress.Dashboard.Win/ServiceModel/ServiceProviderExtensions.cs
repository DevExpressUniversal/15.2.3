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

using DevExpress.DashboardCommon;
using System;
using System.ComponentModel.Design;
namespace DevExpress.DashboardWin.ServiceModel {
	public static class ServiceProviderExtensions {
		public static TService RequestService<TService>(this IServiceProvider serviceProvider) {
			return (TService)serviceProvider.GetService(typeof(TService));
		}
		public static TService RequestServiceStrictly<TService>(this IServiceProvider serviceProvider) {
			TService service = serviceProvider.RequestService<TService>();
			if (service == null)
				throw new InvalidOperationException(string.Format("The {0} service is unavailable.", typeof(TService)));
			return service;
		}
		public static void AddService<TService>(this IServiceContainer serviceContainer, TService service) {
			serviceContainer.AddService(typeof(TService), service);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004")]
		public static void RemoveService<TService>(this IServiceContainer serviceContainer) {
			TService service = serviceContainer.RequestService<TService>();
			if (service != null) {
				IDisposable disp = service as IDisposable;
				if (disp != null)
					disp.Dispose();
				serviceContainer.RemoveService(typeof(TService));
			}
		}
		public static void ReplaceService<TService>(this IServiceContainer serviceContainer, TService newService) {
			serviceContainer.RemoveService<TService>();
			serviceContainer.AddService<TService>(newService);
		}
	}
}
