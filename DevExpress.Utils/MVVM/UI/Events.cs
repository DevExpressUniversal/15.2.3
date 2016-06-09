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

namespace DevExpress.Utils.MVVM {
	using System;
	public class QueryEventArgs<TResult> : EventArgs {
		public QueryEventArgs(object viewModel, object parameter) {
			this.ViewModel = viewModel;
			this.Parameter = parameter;
		}
		public object ViewModel { get; private set; }
		public object Parameter { get; private set; }
		public TResult Result { get; set; }
	}
	public delegate void QueryEventHandler<TEventArgs, TResult>(
		object sender, TEventArgs e) where TEventArgs : QueryEventArgs<TResult>;
}
namespace DevExpress.Utils.MVVM.Services {
	using System.Windows.Forms;
	public class QueryViewEventArgs : QueryEventArgs<Control> {
		public QueryViewEventArgs(string viewType, object viewModel, object parameter)
			: base(viewModel, parameter) {
			this.ViewType = viewType;
		}
		public string ViewType { get; private set; }
	}
	public class QueryViewTypeEventArgs : QueryEventArgs<string> {
		public QueryViewTypeEventArgs(string viewName, object viewModel, object parameter)
			: base(viewModel, parameter) {
			this.ViewName = viewName;
		}
		public string ViewName { get; private set; }
	}
}
