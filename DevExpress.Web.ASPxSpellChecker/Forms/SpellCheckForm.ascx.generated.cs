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

namespace DevExpress.Web.ASPxSpellChecker.Forms {
	[System.CLSCompliantAttribute(false)]
	public partial class SpellCheckForm {
		protected global::System.Web.UI.WebControls.PlaceHolder SCCheckedDivPlaceHolder;
		protected global::DevExpress.Web.ASPxButton btnIgnore;
		protected global::DevExpress.Web.ASPxButton btnIgnoreAll;
		protected global::DevExpress.Web.ASPxButton btnAddToDictionary;
		protected global::DevExpress.Web.ASPxTextBox txtChangeTo;
		protected global::DevExpress.Web.ASPxListBox SCSuggestionsListBox;
		protected global::DevExpress.Web.PanelContent PanelContent1;
		protected global::DevExpress.Web.ASPxPanel ChangeToPanel;
		protected global::DevExpress.Web.ASPxButton btnChange;
		protected global::DevExpress.Web.ASPxButton btnChangeAll;
		protected global::DevExpress.Web.ASPxButton btnOptions;
		protected global::DevExpress.Web.ASPxButton btnClose;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public SpellCheckForm() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/SpellCheckForm.ascx";
			if ((global::DevExpress.Web.ASPxSpellChecker.Forms.SpellCheckForm.@__initialized == false)) {
				global::DevExpress.Web.ASPxSpellChecker.Forms.SpellCheckForm.@__initialized = true;
			}
		}
		protected System.Web.Profile.DefaultProfile Profile {
			get {
				return ((System.Web.Profile.DefaultProfile)(this.Context.Profile));
			}
		}
		protected System.Web.HttpApplication ApplicationInstance {
			get {
				return ((System.Web.HttpApplication)(this.Context.ApplicationInstance));
			}
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.WebControls.PlaceHolder @__BuildControlSCCheckedDivPlaceHolder() {
			global::System.Web.UI.WebControls.PlaceHolder @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.PlaceHolder();
			this.SCCheckedDivPlaceHolder = @__ctrl;
			@__ctrl.ID = "SCCheckedDivPlaceHolder";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control2(DevExpress.Web.ButtonClientSideEvents @__ctrl) {
			@__ctrl.Click = "function(s, e) {ASPx.SCIgnore();}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnIgnore() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnIgnore = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnIgnore";
			@__ctrl.CssClass = "dxwscDlgSpellCheckBtn";
			@__ctrl.AutoPostBack = false;
			@__ctrl.UseSubmitBehavior = false;
			this.@__BuildControl__control2(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control3(DevExpress.Web.ButtonClientSideEvents @__ctrl) {
			@__ctrl.Click = "function(s, e) {ASPx.SCIgnoreAll();}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnIgnoreAll() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnIgnoreAll = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnIgnoreAll";
			@__ctrl.CssClass = "dxwscDlgSpellCheckBtn";
			@__ctrl.AutoPostBack = false;
			@__ctrl.UseSubmitBehavior = false;
			this.@__BuildControl__control3(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control4(DevExpress.Web.ButtonClientSideEvents @__ctrl) {
			@__ctrl.Click = "function(s, e) {ASPx.SCAddToDictionary();}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnAddToDictionary() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnAddToDictionary = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnAddToDictionary";
			@__ctrl.CssClass = "dxwscDlgSpellCheckBtn";
			@__ctrl.AutoPostBack = false;
			@__ctrl.UseSubmitBehavior = false;
			this.@__BuildControl__control4(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control6(DevExpress.Web.TextBoxClientSideEvents @__ctrl) {
			@__ctrl.KeyPress = "function(s, e) {ASPx.SCTextBoxKeyPress(s, e);}";
			@__ctrl.KeyDown = "function(s, e) {ASPx.SCTextBoxKeyDown(s,e);}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxTextBox @__BuildControltxtChangeTo() {
			global::DevExpress.Web.ASPxTextBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxTextBox();
			this.txtChangeTo = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "txtChangeTo";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ClientInstanceName = "_dxeSCTxtChangeTo";
			this.@__BuildControl__control6(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control7(DevExpress.Web.ListBoxClientSideEvents @__ctrl) {
			@__ctrl.ItemDoubleClick = "function(s, e) {ASPx.SCListBoxItemDoubleClick(s, e);}";
			@__ctrl.SelectedIndexChanged = "function(s, e) {ASPx.SCListBoxItemChanged(s, e);}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxListBox @__BuildControlSCSuggestionsListBox() {
			global::DevExpress.Web.ASPxListBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxListBox();
			this.SCSuggestionsListBox = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "SCSuggestionsListBox";
			@__ctrl.ClientInstanceName = "_dxeSCSuggestionsListBox";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.Height = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Pixel);
			this.@__BuildControl__control7(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.PanelContent @__BuildControlPanelContent1() {
			global::DevExpress.Web.PanelContent @__ctrl;
			@__ctrl = new global::DevExpress.Web.PanelContent();
			this.PanelContent1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "PanelContent1";
			global::DevExpress.Web.ASPxTextBox @__ctrl1;
			@__ctrl1 = this.@__BuildControltxtChangeTo();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.Web.ASPxListBox @__ctrl2;
			@__ctrl2 = this.@__BuildControlSCSuggestionsListBox();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__RenderPanelContent1));
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__RenderPanelContent1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n                                    <table style=\"width:100%\" ");
													  @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                                        <tr>\r\n                                " +
					"            <td style=\"width:100%\" class=\"dxwscDlgVerticalSeparator dx-valt\">\r\n " +
					"                                               ");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write(@"
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class=""dxwscDlgListBoxContainer dx-valt"">
                                                ");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write(" \r\n                                            </td>\r\n                           " +
					"             </tr>\r\n                                    </table>                " +
					"                \r\n                                ");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control5(DevExpress.Web.PanelCollection @__ctrl) {
			global::DevExpress.Web.PanelContent @__ctrl1;
			@__ctrl1 = this.@__BuildControlPanelContent1();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxPanel @__BuildControlChangeToPanel() {
			global::DevExpress.Web.ASPxPanel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxPanel();
			this.ChangeToPanel = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "ChangeToPanel";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.DefaultButton = "btnChange";
			this.@__BuildControl__control5(@__ctrl.PanelCollection);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control8(DevExpress.Web.ButtonClientSideEvents @__ctrl) {
			@__ctrl.Click = "function(s, e) { ASPx.SCChange();}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnChange() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnChange = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnChange";
			@__ctrl.CssClass = "dxwscDlgSpellCheckBtn";
			@__ctrl.ClientInstanceName = "_dxeSCBtnChange";
			@__ctrl.AutoPostBack = false;
			@__ctrl.UseSubmitBehavior = false;
			this.@__BuildControl__control8(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control9(DevExpress.Web.ButtonClientSideEvents @__ctrl) {
			@__ctrl.Click = "function(s, e) { ASPx.SCChangeAll();}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnChangeAll() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnChangeAll = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnChangeAll";
			@__ctrl.CssClass = "dxwscDlgSpellCheckBtn";
			@__ctrl.ClientInstanceName = "_dxeSCBtnChangeAll";
			@__ctrl.AutoPostBack = false;
			@__ctrl.UseSubmitBehavior = false;
			this.@__BuildControl__control9(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control10(DevExpress.Web.ButtonClientSideEvents @__ctrl) {
			@__ctrl.Click = "function(s, e) {ASPx.SCShowOptionsForm(true);}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnOptions() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnOptions = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnOptions";
			@__ctrl.CssClass = "dxwscDlgFooterBtn";
			@__ctrl.AutoPostBack = false;
			@__ctrl.UseSubmitBehavior = false;
			this.@__BuildControl__control10(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control11(DevExpress.Web.ButtonClientSideEvents @__ctrl) {
			@__ctrl.Click = "function(s, e) {ASPx.SCDialogComplete(false);}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnClose() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnClose = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnClose";
			@__ctrl.CssClass = "dxwscDlgFooterBtn";
			@__ctrl.AutoPostBack = false;
			@__ctrl.UseSubmitBehavior = false;
			this.@__BuildControl__control11(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControlTree(SpellCheckForm @__ctrl) {
			global::System.Web.UI.WebControls.PlaceHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlSCCheckedDivPlaceHolder();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.Web.ASPxButton @__ctrl2;
			@__ctrl2 = this.@__BuildControlbtnIgnore();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxButton @__ctrl3;
			@__ctrl3 = this.@__BuildControlbtnIgnoreAll();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::DevExpress.Web.ASPxButton @__ctrl4;
			@__ctrl4 = this.@__BuildControlbtnAddToDictionary();
			@__parser.AddParsedSubObject(@__ctrl4);
			global::DevExpress.Web.ASPxPanel @__ctrl5;
			@__ctrl5 = this.@__BuildControlChangeToPanel();
			@__parser.AddParsedSubObject(@__ctrl5);
			global::DevExpress.Web.ASPxButton @__ctrl6;
			@__ctrl6 = this.@__BuildControlbtnChange();
			@__parser.AddParsedSubObject(@__ctrl6);
			global::DevExpress.Web.ASPxButton @__ctrl7;
			@__ctrl7 = this.@__BuildControlbtnChangeAll();
			@__parser.AddParsedSubObject(@__ctrl7);
			global::DevExpress.Web.ASPxButton @__ctrl8;
			@__ctrl8 = this.@__BuildControlbtnOptions();
			@__parser.AddParsedSubObject(@__ctrl8);
			global::DevExpress.Web.ASPxButton @__ctrl9;
			@__ctrl9 = this.@__BuildControlbtnClose();
			@__parser.AddParsedSubObject(@__ctrl9);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n\r\n<table id=\"dxMainSpellCheckFormTable\" ");
							  @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" class=\"dxwscDlgMainSpellCheckFormTable\"> \r\n    <tr>\r\n        <td class=\"dxwscDlg" +
					"ContentSCFormContainer\">\r\n            <table id=\"dxSpellCheckForm\" class=\"dxwscD" +
					"lgSpellCheckForm\" ");
																@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                <tr>\r\n                    <td colspan=\"2\">\r\n\t\t\t\t\t\t");
@__w.Write(DevExpress.Web.ASPxSpellChecker.Localization.ASPxSpellCheckerLocalizer.GetString(DevExpress.Web.ASPxSpellChecker.Localization.ASPxSpellCheckerStringId.NotInDictionary));
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n       " +
					"             <td class=\"dxwscDlgCheckedDivContainer dx-valt\">\r\n                 " +
					"       ");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxwscDlgButtonTableCo" +
					"ntainer dx-valt\">\r\n                        <table id=\"topButtonsTable\" class=\"dx" +
					"wscDlgButtonTable\" ");
																		@__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                            <tr>\r\n                                <td>\r\n      " +
					"                              ");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("\r\n                                </td>\r\n                            </tr>\r\n     " +
					"                       <tr>\r\n                                <td class=\"dxwscDlg" +
					"VerticalSeparator dx-valt\">\r\n                                    ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("                    \r\n                                </td>\r\n                    " +
					"        </tr>\r\n                            ");
							   if(SettingsDialogFormElemets.ShowAddToDictionaryButton) { 
			@__w.Write("\r\n                            <tr>\r\n                                <td class=\"dx" +
					"wscDlgVerticalSeparator dx-valt\">\r\n                                    ");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write("\r\n                                </td>\r\n                            </tr>\r\n     " +
					"                       ");
							   } 
			@__w.Write("\r\n                        </table>\r\n                    </td>\r\n                </" +
					"tr>\r\n                <tr>\r\n                    <td colspan=\"2\" class=\"dxwscDlgCh" +
					"angeToText\">\r\n                        ");
				 @__w.Write(DevExpress.Web.ASPxSpellChecker.Localization.ASPxSpellCheckerLocalizer.GetString(DevExpress.Web.ASPxSpellChecker.Localization.ASPxSpellCheckerStringId.ChangeTo));
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n       " +
					"             <td class=\"dxwscDlgChangeToPanelContainer\">\r\n                      " +
					"  ");
			parameterContainer.Controls[4].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxwscDlgButtonTableCo" +
					"ntainer dx-valt\">\r\n                        <table id=\"bottomButtonsTable\" class=" +
					"\"dxwscDlgButtonTable\" ");
																		   @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n                            <tr>\r\n                                <td class=\"d" +
					"x-valt\">\r\n                                    ");
			parameterContainer.Controls[5].RenderControl(@__w);
			@__w.Write("\r\n                                </td>\r\n                            </tr>\r\n     " +
					"                       <tr>\r\n                                <td class=\"dxwscDlg" +
					"VerticalSeparator dx-valt\">\r\n                                    ");
			parameterContainer.Controls[6].RenderControl(@__w);
			@__w.Write(@"
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            </td>
    </tr>
    <tr>
        <td class=""dxwscDlgFooter"">
            ");
			   if(SettingsDialogFormElemets.ShowOptionsButton) { 
			parameterContainer.Controls[7].RenderControl(@__w);
			@__w.Write("\r\n            ");
			   } 
			parameterContainer.Controls[8].RenderControl(@__w);
			@__w.Write("\r\n         </td>\r\n    </tr>\r\n</table>\r\n");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
