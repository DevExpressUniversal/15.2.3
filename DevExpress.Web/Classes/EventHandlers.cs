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
namespace DevExpress.Web {
	public enum ClientLayoutMode { Loading, Saving }
	public class ASPxClientLayoutArgs : EventArgs {
		private string layoutData;
		private ClientLayoutMode layoutMode;
		public ASPxClientLayoutArgs(ClientLayoutMode layoutMode) : this(layoutMode, string.Empty) { }
		public ASPxClientLayoutArgs(ClientLayoutMode layoutMode, string layoutData) {
			this.layoutMode = layoutMode;
			this.layoutData = layoutData;
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxClientLayoutArgsLayoutMode")]
#endif
		public ClientLayoutMode LayoutMode {
			get { return layoutMode; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxClientLayoutArgsLayoutData")]
#endif
		public string LayoutData {
			get { return layoutData; }
			set {
				if(value == null)
					value = string.Empty;
				layoutData = value;
			}
		}
	}
	public delegate void ASPxClientLayoutHandler(object sender, ASPxClientLayoutArgs e);
	public class CallbackEventArgsBase : EventArgs {
		private string parameter;
#if !SL
	[DevExpressWebLocalizedDescription("CallbackEventArgsBaseParameter")]
#endif
		public string Parameter {
			get { return parameter; }
		}
		public CallbackEventArgsBase(string parameter) {
			this.parameter = parameter;
		}
	}
	public delegate void CallbackEventHandlerBase(object sender, CallbackEventArgsBase e);
	public class CustomJSPropertiesEventArgs : EventArgs {
		private Dictionary<string, object> properties;
		public CustomJSPropertiesEventArgs()
			: base() {
			this.properties = new Dictionary<string, object>();
		}
		public CustomJSPropertiesEventArgs(Dictionary<string, object> properties)
			: base() {
			this.properties = properties;
		}
		public Dictionary<string, object> Properties {
			get { return properties; }
		}
	}
	public delegate void CustomJSPropertiesEventHandler(object sender, CustomJSPropertiesEventArgs e);
	public class CustomDataCallbackEventArgs : CallbackEventArgsBase {
		private string result;
		public string Result {
			get { return result; }
			set { result = value; }
		}
		public CustomDataCallbackEventArgs(string parameter)
			: base(parameter) {
		}
	}
	public delegate void CustomDataCallbackEventHandler(object sender, CustomDataCallbackEventArgs e);
}
