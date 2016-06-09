#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.XtraReports.Web.Forms {
public partial class WebSearchDialog : UserControl, IDialogFormElementRequiresLoad {
	public ReportViewer ReportViewer { get; set; }
	#region IDialogFormElementRequiresLoad
	void IDialogFormElementRequiresLoad.ForceInit() {
		FrameworkInitialize();
	}
	void IDialogFormElementRequiresLoad.ForceLoad() {
		OnLoad(EventArgs.Empty);
	}
	#endregion
	internal void ForceInitialize() {
		FrameworkInitialize();
		InitSkins();
	}
	protected override void Render(HtmlTextWriter writer) {
		AssignScripts();
		base.Render(writer);
	}
	protected override void OnLoad(EventArgs e) {
		base.OnLoad(e);
		AssignStrings();
		InitSkins();
		PrepareControl();
	}
	void InitSkins() {
		aspxPopupControl.ParentSkinOwner = ReportViewer;
		aspxPopupControl.ParentStyles = ReportViewer.SearchDialogFormStyles;
		AssignButtonsSkin(new[] { buttonFind, buttonCancel });
		AssignEditorsSkin(new ASPxEditBase[] { labelFindWhat, textBoxFindText, checkMatchCase, checkMatchWholeWord, checkMatchWholeWord, radioUp, radioDown });
	}
	void PrepareControl() {
		RenderUtils.SetTableSpacings(Table1, 1, 3);
		RenderUtils.SetTableBorder(Table1, 0);
		RenderUtils.SetTableSpacings(Table3, 0, 0);
		RenderUtils.SetTableBorder(Table3, 0);
		if(!RenderUtils.Browser.IsIE)
			checkMatchWholeWord.ClientEnabled = false;
	}
	void AssignButtonsSkin(ASPxButton[] buttons) {
		foreach(ASPxButton button in buttons) {
			button.ParentSkinOwner = aspxPopupControl;
			button.ParentStyles = ReportViewer.SearchDialogButtonStyles;
		}
	}
	void AssignEditorsSkin(ASPxEditBase[] editors) {
		foreach(ASPxEditBase editor in editors) {
			editor.ParentSkinOwner = aspxPopupControl;
			editor.ParentStyles = ReportViewer.SearchDialogEditorsStyles;
			editor.ParentImages = ReportViewer.SearchDialogEditorsImages;
		}
	}
	void AssignStrings() {
		aspxPopupControl.HeaderText = GetString(ASPxReportsStringId.SearchDialog_Header);
		buttonFind.Text = GetString(ASPxReportsStringId.SearchDialog_FindNext);
		buttonCancel.Text = GetString(ASPxReportsStringId.SearchDialog_Cancel);
		labelFindWhat.Text = GetString(ASPxReportsStringId.SearchDialog_FindWhat);
		checkMatchCase.Text = GetString(ASPxReportsStringId.SearchDialog_Case);
		checkMatchWholeWord.Text = GetString(ASPxReportsStringId.SearchDialog_WholeWord);
		radioUp.Text = GetString(ASPxReportsStringId.SearchDialog_Up);
		radioDown.Text = GetString(ASPxReportsStringId.SearchDialog_Down);
	}
	void AssignScripts() {
		aspxPopupControl.ClientSideEvents.Init = GetFunctionCallScript(
			"ASPx.RVSDLoaded",
			ReportViewer,
			aspxPopupControl,
			textBoxFindText,
			buttonFind,
			checkMatchWholeWord,
			checkMatchCase,
			radioUp,
			radioDown);
		buttonFind.ClientSideEvents.Click = GetFunctionCallScript("ASPx.RVSDFind", aspxPopupControl);
		buttonCancel.ClientSideEvents.Click = GetFunctionCallScript("ASPx.RVSDClose", aspxPopupControl);
	}
	static string GetString(ASPxReportsStringId id) {
		return ASPxReportsLocalizer.GetString(id);
	}
	static string GetFunctionCallScript(string function, params WebControl[] controls) {
		var ids = controls.Select(x => '\'' + x.ClientID + '\'');
		string body = function + '(' + string.Join(", ", ids) + ')';
		return GetClientScript(body);
	}
	static string GetClientScript(string body) {
		return "function(s, e) { " + body + '}';
	}
}
}
