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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Utils;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web {
	public static class RenderHelper {
		private const string ForceButtonClickFunctionName = "ForceButtonClick";
		public const string EventCancelBubbleCommand = "event.cancelBubble = true;";
		public static FontStyle GetFontStyle(WebControl control) {
			if(control.Font.Bold) return FontStyle.Bold;
			if(control.Font.Italic) return FontStyle.Italic;
			if(control.Font.Underline) return FontStyle.Underline;
			if(control.Font.Strikeout) return FontStyle.Strikeout;
			return FontStyle.Regular;
		}
		public static void ResetFontStyle(WebControl control) {
			control.Style.Remove("font-weight");
			control.Style.Remove("font-style");
			control.Style.Remove("text-decoration");
			control.Font.Bold = false;
			control.Font.Italic = false;
			control.Font.Underline = false;
			control.Font.Strikeout = false;
		}
		public static void SetFontStyle(WebControl control, FontStyle fontStyle) {
			if((fontStyle & FontStyle.Bold) == FontStyle.Bold) {
				control.Style["font-weight"] = "bold";
				control.Font.Bold = true;
			}
			if((fontStyle & FontStyle.Italic) == FontStyle.Italic) {
				control.Style["font-style"] = "italic";
				control.Font.Italic = true;
			}
			if((fontStyle & FontStyle.Underline) == FontStyle.Underline) {
				control.Style["text-decoration"] = "underline";
				control.Font.Underline = true;
			}
			if((fontStyle & FontStyle.Strikeout) == FontStyle.Strikeout) {
				control.Style["text-decoration"] = "line-through";
				control.Font.Strikeout = true;
			}
		}
		public static TableEx CreateTable() {
			TableEx result = new TableEx();
			result.CellSpacing = 0;
			result.CellPadding = 1; 
			result.BorderWidth = 0;
			return result;
		}
		public static void SetToolTip(HtmlControl control, string toolTip) {
			control.Attributes["title"] = toolTip;
		}
		public static void SetToolTip(WebControl control, string toolTip) {
			control.ToolTip = toolTip;
		}
		public static void SetupASPxWebControl(ASPxWebControl control) {
			control.EnableViewState = false;
			ASPxEdit aspxEdit = control as ASPxEdit;
			if (aspxEdit != null) {
				SetupValidationSettings(aspxEdit.ValidationSettings);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void SetupValidationSettings(ValidationSettings settings) {
			settings.Display = Display.Dynamic;
			settings.ErrorText = UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.MaskValidationErrorMessage);
			settings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
			settings.ErrorFrameStyle.Wrap = DevExpress.Utils.DefaultBoolean.True;
			settings.ErrorTextPosition = ErrorTextPosition.Left;
		}
		public static ASPxButton CreateASPxButton() {
			ASPxButton result = new ASPxButton();
			result.UseSubmitBehavior = false;
			result.AutoPostBack = false;
			result.EnableClientSideAPI = true;
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxComboBox CreateASPxComboBox() {
			ASPxComboBox result = new ASPxComboBox();
			result.EncodeHtml = false;
			result.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxTextBox CreateASPxTextBox() {
			ASPxTextBox result = new ASPxTextBox();
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxSpinEdit CreateASPxSpinEdit() {
			ASPxSpinEdit result = new ASPxSpinEdit();
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxDateEdit CreateASPxDateEdit() {
			ASPxDateEdit result = new ASPxDateEdit();
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxMenu CreateASPxMenu() {
			ASPxMenu result = new ASPxMenu();
			result.AccessibilityCompliant = true;
			result.ShowAsToolbar = true; 
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxPopupMenu CreateASPxPopupMenu() {
			ASPxPopupMenu result = new ASPxPopupMenu();
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxPageControl CreateASPxPageControl() {
			ASPxPageControl result = new ASPxPageControl();
			result.EnableClientSideAPI = true;
			result.SettingsLoadingPanel.ShowImage = false;
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxCheckBox CreateASPxCheckBox() {
			ASPxCheckBox result = new ASPxCheckBox();
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxMemo CreateASPxMemo() {
			ASPxMemo result = new ASPxMemo();
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxImage CreateASPxImage() {
			ASPxImage result = new ASPxImage();
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxHyperLink CreateASPxHyperLink() {
			ASPxHyperLink result = new ASPxHyperLink();
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxLabel CreateASPxLabel() {
			ASPxLabel result = new ASPxLabel();
			result.EnableClientSideAPI = true;
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxNavBar CreateASPxNavBar() {
			ASPxNavBar result = new ASPxNavBar();
			SetupASPxWebControl(result);
			return result;
		}
		public static ASPxRoundPanel CreateASPxRoundPanel() {
			ASPxRoundPanel result = new ASPxRoundPanel();
			SetupASPxWebControl(result);
			return result;
		}
		public static string GetForceButtonClickFunctionName() {
			return ForceButtonClickFunctionName;
		}
		public static string QuoteJScriptString(string value) {
			return QuoteJScriptString(value, false);
		}
		public static string QuoteJScriptString(string value, bool forUrl) {
			StringBuilder builder = null;
			if(string.IsNullOrEmpty(value)) {
				return string.Empty;
			}
			int startIndex = 0;
			int count = 0;
			for(int i = 0; i < value.Length; i++) {
				switch(value[i]) {
					case '%': {
							if(!forUrl) {
								break;
							}
							if(builder == null) {
								builder = new StringBuilder(value.Length + 6);
							}
							if(count > 0) {
								builder.Append(value, startIndex, count);
							}
							builder.Append("%25");
							startIndex = i + 1;
							count = 0;
							continue;
						}
					case '\'': {
							if(builder == null) {
								builder = new StringBuilder(value.Length + 5);
							}
							if(count > 0) {
								builder.Append(value, startIndex, count);
							}
							builder.Append(@"\'");
							startIndex = i + 1;
							count = 0;
							continue;
						}
					case '\\': {
							if(builder == null) {
								builder = new StringBuilder(value.Length + 5);
							}
							if(count > 0) {
								builder.Append(value, startIndex, count);
							}
							builder.Append(@"\\");
							startIndex = i + 1;
							count = 0;
							continue;
						}
					case '\t': {
							if(builder == null) {
								builder = new StringBuilder(value.Length + 5);
							}
							if(count > 0) {
								builder.Append(value, startIndex, count);
							}
							builder.Append(@"\t");
							startIndex = i + 1;
							count = 0;
							continue;
						}
					case '\n': {
							if(builder == null) {
								builder = new StringBuilder(value.Length + 5);
							}
							if(count > 0) {
								builder.Append(value, startIndex, count);
							}
							builder.Append(@"\n");
							startIndex = i + 1;
							count = 0;
							continue;
						}
					case '\r': {
							if(builder == null) {
								builder = new StringBuilder(value.Length + 5);
							}
							if(count > 0) {
								builder.Append(value, startIndex, count);
							}
							builder.Append(@"\r");
							startIndex = i + 1;
							count = 0;
							continue;
						}
					case '"': {
							if(builder == null) {
								builder = new StringBuilder(value.Length + 5);
							}
							if(count > 0) {
								builder.Append(value, startIndex, count);
							}
							builder.Append("\\\"");
							startIndex = i + 1;
							count = 0;
							continue;
						}
				}
				count++;
			}
			if(builder == null) {
				return value;
			}
			if(count > 0) {
				builder.Append(value, startIndex, count);
			}
			return builder.ToString();
		}
		#region Obsolete 15.1
		[Obsolete("The Lightweight render mode is used. Do not use this property any more."), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static ControlRenderMode RenderMode {
			get { return ControlRenderMode.Lightweight; }
			set { }
		}
		[Obsolete("Use the CreateASPxMenu() method instead."), EditorBrowsable(EditorBrowsableState.Never)]
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
		public static ASPxMenu CreateASPxMenu(ControlRenderMode renderMode) {
			return CreateASPxMenu();
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetForceButtonClickFunctionName(ASPxButton button) {
			Guard.ArgumentNotNullOrEmpty(button.ClientID, "button.ClientID");
			return ForceButtonClickFunctionName + "_" + button.ClientID;
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetForceButtonClickFunctionBody(ASPxButton button) {
			Guard.ArgumentNotNullOrEmpty(button.ClientID, "button.ClientID");
			return "";
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetPostBackEventReference(String controlClientId, string argument) {
			return (("xafDoPostBack('" + controlClientId + "','") + QuoteJScriptString(argument) + "')");
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetCallbackEventReference(string controlClientId, string argument, string clientCallback, string context) {
			return GetCallbackEventReference(controlClientId, argument, clientCallback, context, false);
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetCallbackEventReference(string controlClientId, string argument, string clientCallback, string context, bool useAsync) {
			return GetCallbackEventReference(controlClientId, argument, clientCallback, context, null, useAsync);
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetCallbackEventReference(string controlClientId, string argument, string clientCallback, string context, string clientErrorCallback, bool useAsync) {
			if(argument == null) {
				argument = "null";
			}
			else if(argument.Length == 0) {
				argument = "\"\"";
			}
			if(context == null) {
				context = "null";
			}
			else if(context.Length == 0) {
				context = "\"\"";
			}
			return ("xafDoCallback('" + controlClientId + "'," + argument + "," + clientCallback + "," + context + "," + ((clientErrorCallback == null) ? "null" : clientErrorCallback) + "," + (useAsync ? "true" : "false") + ")");
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetCallbackEventReference(Control control) {
			return GetCallbackEventReference(control, "");
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetCallbackEventReference(Control control, string param) {
			return GetCallbackEventReference(control, param, "xafEvalFunc");
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetCallbackEventReference(Control control, string param, string callbackFunction) {
			return GetCallbackEventReference(control, param, callbackFunction, null);
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetCallbackEventReference(Control control, string param, string callbackFunction, string context) {
			return GetCallbackEventReference(control.UniqueID, param, callbackFunction, context);
		}
		#endregion
	}
	[ToolboxItem(false)]
	public class TableEx : Table {
		private bool isPrerendered;
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			isPrerendered = true;
		}
		protected override void Render(HtmlTextWriter writer) {
			if(!isPrerendered) {
				OnPreRender(EventArgs.Empty);
			}
			base.Render(writer);
		}
	}
}
