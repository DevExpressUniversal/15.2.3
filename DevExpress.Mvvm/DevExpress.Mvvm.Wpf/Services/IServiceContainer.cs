#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Linq;
using System.Text;
using DevExpress.Mvvm.Native;
namespace DevExpress.Mvvm {
	public enum ServiceSearchMode { PreferLocal, LocalOnly, PreferParents }
	public interface IServiceContainer {
		void Clear();
		void RegisterService(object service, bool yieldToParent = false);
		void RegisterService(string key, object service, bool yieldToParent = false);
		void UnregisterService(object service);
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		T GetService<T>(string key, ServiceSearchMode searchMode, out bool serviceHasKey) where T : class;
		T GetService<T>(ServiceSearchMode searchMode = ServiceSearchMode.PreferLocal) where T : class;
		T GetService<T>(string key, ServiceSearchMode searchMode = ServiceSearchMode.PreferLocal) where T : class;
	}
	public static class ServiceContainerExtensions {
		public static T GetRequiredService<T>(this IServiceContainer serviceContainer, string key) where T : class {
			return serviceContainer.GetRequiredService<T>(key, ServiceSearchMode.PreferLocal);
		}
		public static T GetRequiredService<T>(this IServiceContainer serviceContainer) where T : class {
			return serviceContainer.GetRequiredService<T>(null, ServiceSearchMode.PreferLocal);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static T GetRequiredService<T>(this IServiceContainer serviceContainer, string key, ServiceSearchMode searchMode) where T : class {
			VerifyServiceContainer(serviceContainer);
			return CheckService(serviceContainer.GetService<T>(key, searchMode));
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static T GetRequiredService<T>(this IServiceContainer serviceContainer, ServiceSearchMode searchMode) where T : class {
			VerifyServiceContainer(serviceContainer);
			return CheckService(serviceContainer.GetService<T>(searchMode));
		}
		static void VerifyServiceContainer(IServiceContainer serviceContainer) {
			if(serviceContainer == null)
				throw new ArgumentNullException("serviceContainer");
		}
		static T CheckService<T>(T service) where T : class {
			if(service == null)
				throw new ServiceNotFoundException();
			return service;
		}
	}
	public class ServiceNotFoundException : Exception {
		public ServiceNotFoundException() :
			base("The target service is not found") {
		}
	}
}
