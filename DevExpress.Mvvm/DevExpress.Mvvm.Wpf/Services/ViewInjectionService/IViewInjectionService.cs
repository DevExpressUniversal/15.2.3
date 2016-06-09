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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
namespace DevExpress.Mvvm {
	public interface IViewInjectionService {
		[EditorBrowsable(EditorBrowsableState.Never)]
		string RegionName { get; }
		IEnumerable<object> ViewModels { get; }
		object SelectedViewModel { get; set; }
		object GetKey(object viewModel);
		[EditorBrowsable(EditorBrowsableState.Never)]
		void Inject(object key, object viewModel, string viewName, Type viewType);
		bool Remove(object viewModel);
	}
	public static class ViewInjectionServiceExtensions {
		public static void Inject(this IViewInjectionService service, object key, object viewModel) {
			VerifyService(service);
			service.Inject(key, viewModel, string.Empty, null);
		}
		public static void Inject(this IViewInjectionService service, object key, object viewModel, string viewName) {
			VerifyService(service);
			service.Inject(key, viewModel, viewName, null);
		}
		public static void Inject(this IViewInjectionService service, object key, object viewModel, Type viewType) {
			VerifyService(service);
			service.Inject(key, viewModel, null, viewType);
		}
		public static object GetViewModel(this IViewInjectionService service, object key) {
			VerifyService(service);
			return service.ViewModels.FirstOrDefault(x => object.Equals(service.GetKey(x), key));
		}
		static void VerifyService(IViewInjectionService service) {
			if(service == null)
				throw new ArgumentNullException("service");
		}
	}
	public delegate void ViewModelClosingEventHandler(object sender, ViewModelClosingEventArgs e);
	public class ViewModelClosingEventArgs : CancelEventArgs {
		public object ViewModel { get; private set; }
		public ViewModelClosingEventArgs(object viewModel) {
			ViewModel = viewModel;
		}
	}
}
