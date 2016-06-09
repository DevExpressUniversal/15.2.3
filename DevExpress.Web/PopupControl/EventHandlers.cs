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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.Web {
	public class PopupWindowEventArgs : EventArgs {
		private PopupWindow fWindow = null;
		public PopupWindow Window {
			get { return fWindow; }
		}
		public PopupWindowEventArgs(PopupWindow window)
			: base() {
			fWindow = window;
		}
	}
	public delegate void PopupWindowEventHandler(object source, PopupWindowEventArgs e);
	public class PopupControlCommandEventArgs : CommandEventArgs {
		private object fCommandSource = null;
		private PopupWindow fWindow = null;
		public object CommandSource {
			get { return fCommandSource; }
		}
		public PopupWindow Window {
			get { return fWindow; }
		}
		public PopupControlCommandEventArgs(object commandSource, CommandEventArgs originalArgs)
			: base(originalArgs) {
			fCommandSource = commandSource;
		}
		public PopupControlCommandEventArgs(PopupWindow window, object commandSource, CommandEventArgs originalArgs)
			: this(commandSource, originalArgs) {
			fWindow = window;
		}
	}
	public delegate void PopupControlCommandEventHandler(object source, PopupControlCommandEventArgs e);
	public class PopupWindowCallbackArgs : CallbackEventArgsBase{
		PopupWindow window;
		public PopupWindowCallbackArgs(PopupWindow window, string parameter) : base(parameter) {
			this.window = window;
		}
#if !SL
	[DevExpressWebLocalizedDescription("PopupWindowCallbackArgsWindow")]
#endif
		public PopupWindow Window{
			get { return window; }
		}
	}
	public delegate void PopupWindowCallbackEventHandler(object source, PopupWindowCallbackArgs e);
}
