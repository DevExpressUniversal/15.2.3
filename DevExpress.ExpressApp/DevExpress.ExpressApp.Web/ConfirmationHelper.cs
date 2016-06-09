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
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web {
	public class ConfirmationsHelper {
		public const string OnClickAttributeName = "onclick";
		private static bool isConfirmationsEnabled = true;
		public static bool IsConfirmationsEnabled {
			get { return isConfirmationsEnabled; }
			set { isConfirmationsEnabled = value; }
		}
		public static string EncodeConfirmationMessage(string confirmationMessage) {
			return EncodeConfirmationMessage(confirmationMessage, true);
		}
		public static string EncodeConfirmationMessage(string confirmationMessage, bool addQuotes) {
			confirmationMessage = HttpUtility.HtmlAttributeEncode(confirmationMessage);
			confirmationMessage = confirmationMessage.Replace("'", "`");
			if(addQuotes) {
				confirmationMessage = String.Format(@"'{0}'", confirmationMessage);
				confirmationMessage = confirmationMessage.Replace("\r\n", @"\r\n\ ");
			}
			return confirmationMessage;
		}
		public static string GetConfirmationScript(string confirmationMessage) {
			if(IsConfirmationsEnabled && !string.IsNullOrEmpty(confirmationMessage)) {
				return string.Format("confirm(xafHtmlDecode('{0}'))", EncodeConfirmationMessage(confirmationMessage, false));
			}
			return null;
		}
		public static string FormatConfirmationMessage(string message) {
			string result = HttpUtility.HtmlAttributeEncode(message);
			result = result.Replace("\r\n", " \r\n");
			result = String.Format(@"'{0}'", RenderHelper.QuoteJScriptString(result));
			return result;
		}
		public static string GetConfirmationWithScriptBody(string confirmationMessage, string functionBody) {
			return GetConfirmationWithScriptBody(confirmationMessage, functionBody, true);
		}
		public static string GetConfirmationWithScriptBody(string confirmationMessage, string functionBody, bool addQuotes) {
			if(addQuotes) {
				confirmationMessage = FormatConfirmationMessage(confirmationMessage);				
			}
			if(IsConfirmationsEnabled && !string.IsNullOrEmpty(confirmationMessage)) {
				return string.Format(@"
					var confirmation = {0};
					if(confirmation == '' || confirm(xafHtmlDecode(confirmation))) {{ {1} }} else return false;",
						confirmationMessage, functionBody);
			}
			return functionBody;
		}
		public static void SetConfirmationScript(Control control, string confirmationMessage) {
			if(IsConfirmationsEnabled && !string.IsNullOrEmpty(confirmationMessage)) {
				if(control is ASPxButton) {
					((ASPxButton)control).ClientSideEvents.Click =
						@"function Confirm(s, e) { if(!s.enabled || !" + GetConfirmationScript(confirmationMessage) + ") { e.processOnServer = false; return; } }";
				}
				else if(control is ASPxHyperLink) {
					((ASPxHyperLink)control).ClientSideEvents.Click =
						@"function Confirm(s, e) { if(!" + GetConfirmationScript(confirmationMessage) + ") { e.processOnServer = false; return; } }";
				}
				else if(control is ASPxPopupMenu) {
					((ASPxPopupMenu)control).ClientSideEvents.ItemClick =
						@"function Confirm(s, e) { if(e.item.GetItemCount() != 0 || !" + GetConfirmationScript(confirmationMessage) + ") { e.item.SetChecked(false); e.processOnServer = false; return; } }";
				}
				else if(control is ASPxMenu) {
					((ASPxMenu)control).ClientSideEvents.ItemClick =
						@"function Confirm(s, e) { if(e.item.GetItemCount() != 0 || !" + GetConfirmationScript(confirmationMessage) + ") { e.item.SetChecked(false); e.processOnServer = false; return; } }";
				}
				else if(control is WebControl) {
					((WebControl)control).Attributes[ConfirmationsHelper.OnClickAttributeName] =
						"return " + GetConfirmationScript(confirmationMessage) + ";";
				}
				else if(control is HtmlControl) {
					((HtmlControl)control).Attributes[ConfirmationsHelper.OnClickAttributeName] =
						"return " + GetConfirmationScript(confirmationMessage) + ";";
				}
			}
		}
	}
}
