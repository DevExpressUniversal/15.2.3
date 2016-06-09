#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Controls {
	public class ResizableControlContainer : IDisposable {
		public static int MaxWidth = 32767; 
		public static int MaxHeight = 32767; 
		private readonly static char[] sizeStringSeparator = new char[] { '/' };
		private ASPxCallbackPanel container;
		private WebControl control;
		private Panel blankPanel;
		private HtmlInputHidden sizeStorage;
		private int minWidth;
		private int minHeight;
		private void container_Load(object sender, EventArgs e) {
			if(WebWindow.CurrentRequestWindow != null) {
				Container.ClientSideEvents.Init = String.Format(
@"function(s, e) {{
    if(!window['{5}']) {{
        window['{5}'] = window.setInterval(function() {{ RCPVisibilityIntervalHandler('{0}', '{1}', '{2}', '{3}px', '{4}px', '{5}'); }}, 500);
    }}
}}
", Container.ClientID, Control.ClientID, sizeStorage.ClientID, MinWidth, MinHeight, Container.ClientID + "VT");
				Container.ClientSideEvents.EndCallback = String.Format(
@"function(s, e) {{
	var control = document.getElementById('{0}');
    if(control) {{
        control.style.visibility = 'visible';
    }}
}}
", Control.ClientID);
			}
		}
		private void container_Callback(object sender, CallbackEventArgsBase e) {
			OnCallback();
		}
		private void controlSizeStorage_ServerChange(object sender, EventArgs e) {
			SetControlSize(sizeStorage.Value);
		}
		private void OnMinWidthChanged() {
			blankPanel.Width = MinWidth;
		}
		private void OnMinHeightChanged() {
			blankPanel.Height = MinHeight;
		}
		protected void SetControlSize(string sizeString) {
			if(!string.IsNullOrEmpty(sizeString)) {
				string[] parameters = sizeString.Split(sizeStringSeparator, StringSplitOptions.RemoveEmptyEntries);
				int clientWidth, clientHeight;
				if(int.TryParse(parameters[0], out clientWidth) && int.TryParse(parameters[1], out clientHeight)) {
					SetControlSize(clientWidth, clientHeight);
				}
			}
		}
		protected virtual void SetControlSize(int width, int height) {
			if(Control != null) {
				Control.Width = Math.Max(MinWidth, Math.Min(width, MaxWidth));
				Control.Height = Math.Max(MinHeight, Math.Min(height, MaxHeight));
			}
		}
		protected virtual void OnCallback() {
			if(Callback != null) {
				Callback(this, EventArgs.Empty);
			}
		}
		public ResizableControlContainer(WebControl controlToResize) {
			control = controlToResize;
			control.Style["visibility"] = "hidden";
			container = new ASPxCallbackPanel();
			container.ID = "CPR";
			container.Load += container_Load;
			container.Callback += container_Callback;
			container.Controls.Add(control);
			blankPanel = new Panel();
			blankPanel.ID = "BP";
			blankPanel.Controls.Add(new LiteralControl("&nbsp;"));
			container.Controls.Add(blankPanel);
			sizeStorage = new HtmlInputHidden();
			sizeStorage.ID = "SS";
			sizeStorage.ServerChange += controlSizeStorage_ServerChange;
			container.Controls.Add(sizeStorage);
			MinWidth = 320;
			MinHeight = 240;
			if(WebWindow.CurrentRequestPage != null) {
				control.Visible = WebWindow.CurrentRequestPage.IsCallback;
				blankPanel.Visible = !WebWindow.CurrentRequestPage.IsCallback;
			}
		}
		public string GetScript() {
			if(Control == null) return string.Empty;
			return string.Format("UpdateResizableControlContainer('{0}', '{1}', '{2}', '{3}px', '{4}px')", Container.ClientID, Control.ClientID, sizeStorage.ClientID, MinWidth, MinHeight);
		}
		public void Dispose() {
			if(container != null) {
				container.Load -= container_Load;
				container.Callback -= container_Callback;
				container = null;
			}
			control = null;
			blankPanel = null;
			if(sizeStorage != null) {
				sizeStorage.ServerChange -= controlSizeStorage_ServerChange;
				sizeStorage = null;
			}
			Callback = null;
		}
		public ASPxCallbackPanel Container {
			get { return container; }
		}
		public WebControl Control {
			get { return control; }
		}
		public int MinWidth {
			get { return minWidth; }
			set {
				if(MinWidth != value) {
					minWidth = value;
					OnMinWidthChanged();
				}
			}
		}
		public int MinHeight {
			get { return minHeight; }
			set {
				if(MinHeight != value) {
					minHeight = value;
					OnMinHeightChanged();
				}
			}
		}
		public event EventHandler Callback;
	}
}
