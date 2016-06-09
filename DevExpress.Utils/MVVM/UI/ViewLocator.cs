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

namespace DevExpress.Utils.MVVM.UI {
	using System;
	sealed class ViewLocator : IViewLocator, IViewTypeResolver {
		internal static readonly IViewLocator Instance = new ViewLocator();
		object IViewLocator.Resolve(string viewName, params object[] parameters) {
			var viewActivator = GetViewActivator(Pop(ref parameters)) ?? ViewActivator.Instance;
			string viewType = RaiseQueryViewType(viewName, Pop(ref parameters), Pop(ref parameters));
			return viewActivator.CreateView(viewType, parameters);
		}
		public event QueryEventHandler<Services.QueryViewTypeEventArgs, string> QueryViewType;
		string RaiseQueryViewType(string viewName, object viewModel, object parameter) {
			QueryEventHandler<Services.QueryViewTypeEventArgs, string> handler = QueryViewType;
			var args = new Services.QueryViewTypeEventArgs(viewName, viewModel, parameter);
			if(handler != null)
				handler(this, args);
			return args.Result ?? viewName;
		}
		#region static
		static IViewActivator GetViewActivator(object parentViewModel) {
			var parentViewActivator = MVVMContext.GetService<IViewActivator>(parentViewModel);
			return parentViewActivator ?? MVVMContext.GetDefaultService<IViewActivator>();
		}
		static object Pop(ref object[] parameters) {
			if(parameters == null || parameters.Length == 0)
				return null;
			object result = parameters[0];
			object[] tmp = new object[parameters.Length - 1];
			Array.Copy(parameters, 1, tmp, 0, tmp.Length);
			parameters = tmp;
			return result;
		}
		#endregion static
	}
}
