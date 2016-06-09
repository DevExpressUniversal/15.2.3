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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.ExpressApp.Web.Localization;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public interface IPictureEditShowingStrategy : IDisposable {
		ASPxWebControl CreateControl(Control content);
		string GetIsVisibleScript();
		string GetShowScript();
		string GetHideScript();
		void RegisterOnShownScript(string script);
		void RegisterOnClosingScript(string script);
	}
	public class ShowInPopupStrategy : IPictureEditShowingStrategy {
		private ASPxPopupControl popupControl;
		#region IDropDownImageShowingStrategy Members
		public ASPxWebControl CreateControl(Control content) {
			popupControl = CreatePopupControl();
			popupControl.Controls.Add(content);
			return popupControl;
		}
		private ASPxPopupControl CreatePopupControl() {
			ASPxPopupControl popupControl = new ASPxPopupControl();
			popupControl.ID = "PPC";
			popupControl.Modal = false;
			popupControl.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Left;
			popupControl.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			popupControl.PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
			popupControl.AllowDragging = true;
			popupControl.EnableViewState = false;
			popupControl.CloseAction = CloseAction.CloseButton;
			popupControl.HeaderText = ASPxImagePropertyEditorLocalizer.Active.GetLocalizedString("PopupHeader");
			return popupControl;
		}
		public string GetIsVisibleScript() {
			return String.Format("{0}.GetVisible()", popupControl.ClientID);
		}
		public string GetShowScript() {
			return String.Format("{0}.Show();", popupControl.ClientID);
		}
		public string GetHideScript() {
			return String.Format("{0}.Hide(); ", popupControl.ClientID);
		}
		public void RegisterOnShownScript(string script) {
			popupControl.ClientSideEvents.Shown = script;
		}
		public void RegisterOnClosingScript(string script) {
			popupControl.ClientSideEvents.Closing = script;
		}
		#endregion
		public void Dispose() {
			if(popupControl != null) {
				popupControl.Dispose();
				popupControl = null;
			}
		}
	}
	public class ShowInPanelStrategy : IPictureEditShowingStrategy {
		private ASPxPanel panel;
		#region IDropDownImageShowingStrategy Members
		public ASPxWebControl CreateControl(Control content) {
			panel = new ASPxPanel();
			panel.ID = "PNL";
			panel.ClientVisible = false;
			panel.Controls.Add(content);
			return panel;
		}
		public string GetIsVisibleScript() {
			return String.Format("{0}.GetVisible()", panel.ClientID);
		}
		public string GetShowScript() {
			return String.Format("{0}.SetVisible(true); ", panel.ClientID);
		}
		public string GetHideScript() {
			return String.Format("{0}.SetVisible(false); ", panel.ClientID);
		}
		public void RegisterOnShownScript(string script) {
		}
		public void RegisterOnClosingScript(string script) {
		}
		#endregion
		public void Dispose() {
			if(panel != null) {
				panel.Dispose();
				panel = null;
			}
		}
	}
}
