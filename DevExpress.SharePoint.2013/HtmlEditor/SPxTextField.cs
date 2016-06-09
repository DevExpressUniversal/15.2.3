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
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
namespace DevExpress.SharePoint {
	using DevExpress.SharePoint.Internal;
	using DevExpress.Web.Internal;
	using Microsoft.SharePoint.WebControls;
	public class SPxTextField : SPxHtmlEditor {
		const string FeatureID = "4971A9F1-E555-4ae0-93FD-DC934598FBF7";
		const int DefaultFontHeight = 16;
		const int DefaultToolbarHeight = 30;
		static string[] CommandsInCompatibleMode = new string[] { "bold", "italic", "underline", "strikethrough" ,"fontsize", 
			"fontname", "forecolor", "backcolor", "justifycenter", "justifyleft", "justifyright", "justifyfull" };
		const string OnSubmitFunctionString = @"
            function aspxSPxHETextSubmit(dxEditorID,nativeEditorID){
                var dxEditor = ASPxClientControl.GetControlCollection().Get(dxEditorID);
                var nativeEditorInput = ASPx.GetElementById(nativeEditorID);
                if(!nativeEditorInput) 
                    nativeEditorInput = ASPx.GetElementById(nativeEditorID + '_spSave');
                if(nativeEditorInput){
                    nativeEditorInput.value = dxEditor.GetHtml();
                    if(RTE && RTE.RichTextEditor)
            		    RTE.RichTextEditor.transferContentsToInputField = function(){ return; }
            	}
            }";
		const string OnSubmitHandlerString = "aspxSPxHETextSubmit('{0}', '{1}');";
		const string NativeTextBoxID = "TextField";
		const string NativeEditorContainerID = "nativeEditorContainer";
		SPFieldMultiLineText multiLineTextField = null;
		HtmlControl nativeEditorContainer = null;
		TextBox nativeTextBox = null;
		RichTextField richTextField = null;
		public SPxTextField()
			: base() {
		}
		protected SPFieldMultiLineText MultiLineTextField {get {return multiLineTextField; }}
		protected HtmlControl NativeEditorContainer { get { return nativeEditorContainer; } }
		protected TextBox NativeTextBox { get { return nativeTextBox; } }
		protected RichTextField RichTextField { get { return richTextField; } }
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if (SharePointHelper.IsFeatureEnabled(FeatureID, Context)) {
				this.nativeEditorContainer = (HtmlControl)this.Parent.FindControl(NativeEditorContainerID);
				if (NativeEditorContainer != null) {
					this.nativeTextBox = (TextBox)NativeEditorContainer.FindControl(NativeTextBoxID);
					NativeEditorContainer.Style.Add(HtmlTextWriterStyle.Display, "none");
				}
				this.richTextField = (RichTextField)this.Parent.NamingContainer;
				this.multiLineTextField = (SPFieldMultiLineText)RichTextField.Field;
				RegisterOnSubmitScript();
				InitializeProperties();
			} else
				Visible = false;
		}
		protected override void FillAllowableToolbarCommandNames(List<string> commandNames) {
			if ((MultiLineTextField != null) && MultiLineTextField.RichTextMode == SPRichTextMode.Compatible)
				commandNames.AddRange(CommandsInCompatibleMode);
		}
		protected override void FillCssFilePaths(List<string> cssFilePaths) {
			cssFilePaths.Add(string.Format("/_layouts/{0}/styles/core.css", LCID));
		}
		protected void InitializeProperties() {
			Page.PreRenderComplete += new EventHandler(Page_PreRenderComplete);
			if (MultiLineTextField.NumberOfLines > 0)
				Height = 40 + MultiLineTextField.NumberOfLines * DefaultFontHeight + 
					Toolbars.Count * DefaultToolbarHeight;
			if (MultiLineTextField.RichTextMode == SPRichTextMode.Compatible)
				AllowHtmlSourceEditing = false;
			if (RichTextField.DisplaySize > 680)
				Width = RichTextField.DisplaySize * DefaultFontHeight/2;
		}
		private void Page_PreRenderComplete(object sender, EventArgs e) {
			if (!Page.IsPostBack && !Page.IsCallback && (RichTextField.ControlMode != SPControlMode.Display) &&
				MultiLineTextField.AppendOnly && (RichTextField.List != null) &&
				RichTextField.List.EnableVersioning)
				Html = string.Empty;
			else
				Html = NativeTextBox.Text;
		}
		private void RegisterOnSubmitScript() {
			string script = "function RTE_GiveEditorFirstFocus(strBaseElementID) {return;}function RTE_TransferIFrameContentsToTextArea(strBaseElementID) {return;}function RTE_TransferTextAreaContentsToIFrame(strBaseElementID) {return;}";
			if (!Page.ClientScript.IsClientScriptBlockRegistered(Page.GetType(), "TransferFunc"))
				Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "TransferFunc", script, true);
			if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(SPxTextField), "SPxTextField")) {
				Page.ClientScript.RegisterClientScriptBlock(typeof(SPxTextField), "SPxTextField",
					OnSubmitFunctionString, true);
			}
			Page.ClientScript.RegisterOnSubmitStatement(typeof(SPxTextField), ClientID,
				string.Format(OnSubmitHandlerString, ClientID, NativeTextBox.ClientID));
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.Append(localVarName + ".inSharePoint = true;\n");
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			Utils.RegisterCSS(Page);
		}
	}
}
