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
using System.Linq;
using System.Text;
namespace DevExpress.Mvvm {
	public interface INavigationService {
#if !NETFX_CORE
		void ClearCache();
		void ClearNavigationHistory();
		void Navigate(string viewType, object viewModel, object param, object parentViewModel, bool saveToJournal);
		bool CanNavigate { get; }
		void GoBack(object param);
		void GoForward(object param);
#else
		void Navigate(string target, object param = null, object parentViewModel = null);
		void Navigate(string target, object param, object parentViewModel, bool saveToJournal);
#endif
		void GoBack();
		void GoForward();
		bool CanGoBack { get; }
		bool CanGoForward { get; }
#if !NETFX_CORE
		event EventHandler CanGoBackChanged;
		event EventHandler CanGoForwardChanged;
#endif
		object Current { get; }
#if !NETFX_CORE
		event EventHandler CurrentChanged;
#endif
	}
#if !NETFX_CORE
	public static class INavigationServiceExtensions {
		public static void Navigate(this INavigationService service, string viewType, object param = null, object parentViewModel = null) {
			NavigateCore(service, viewType, null, param, parentViewModel, true);
		}
		public static void Navigate(this INavigationService service, object viewModel, object param = null, object parentViewModel = null) {
			NavigateCore(service, null, viewModel, param, parentViewModel, true);
		}
		public static void Navigate(this INavigationService service, string viewType, object param, object parentViewModel, bool saveToJournal) {
			NavigateCore(service, viewType, null, param, parentViewModel, saveToJournal);
		}
		static void NavigateCore(INavigationService service, string viewType, object viewModel, object param, object parentViewModel, bool saveToJournal) {
			VerifyService(service);
			service.Navigate(viewType, viewModel, param, parentViewModel, saveToJournal);
		}
		static void VerifyService(INavigationService service) {
			if(service == null)
				throw new ArgumentNullException("service");
		}
	}
#endif
}
