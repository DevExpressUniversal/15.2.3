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
	public partial class SpellCheckOptionsForm {
		protected global::DevExpress.Web.ASPxCheckBox chkbUpperCase;
		protected global::DevExpress.Web.ASPxCheckBox chkbMixedCase;
		protected global::DevExpress.Web.ASPxCheckBox chkbNumbers;
		protected global::DevExpress.Web.ASPxCheckBox chkbEmails;
		protected global::DevExpress.Web.ASPxCheckBox chkbUrls;
		protected global::DevExpress.Web.ASPxCheckBox chkbTags;
		protected global::DevExpress.Web.ASPxRoundPanel pnlOptions;
		protected global::DevExpress.Web.ASPxLabel lblChooseDictionary;
		protected global::DevExpress.Web.ASPxLabel lblLanguage;
		protected global::DevExpress.Web.ASPxComboBox comboLanguage;
		protected global::DevExpress.Web.ASPxRoundPanel pnlLanguageSelection;
		protected global::DevExpress.Web.ASPxButton btnOK;
		protected global::DevExpress.Web.ASPxButton btnCancel;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public SpellCheckOptionsForm() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/SpellCheckOptionsForm.ascx";
			if ((global::DevExpress.Web.ASPxSpellChecker.Forms.SpellCheckOptionsForm.@__initialized == false)) {
				global::DevExpress.Web.ASPxSpellChecker.Forms.SpellCheckOptionsForm.@__initialized = true;
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
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkbUpperCase() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkbUpperCase = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "chkbUpperCase";
			@__ctrl.ClientInstanceName = "chkbUpperCase";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkbMixedCase() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkbMixedCase = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "chkbMixedCase";
			@__ctrl.ClientInstanceName = "chkbMixedCase";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkbNumbers() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkbNumbers = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "chkbNumbers";
			@__ctrl.ClientInstanceName = "chkbNumbers";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkbEmails() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkbEmails = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "chkbEmails";
			@__ctrl.ClientInstanceName = "chkbEmails";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkbUrls() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkbUrls = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "chkbUrls";
			@__ctrl.ClientInstanceName = "chkbUrls";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlchkbTags() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.chkbTags = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "chkbTags";
			@__ctrl.ClientInstanceName = "chkbTags";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.PanelContent @__BuildControl__control3() {
			global::DevExpress.Web.PanelContent @__ctrl;
			@__ctrl = new global::DevExpress.Web.PanelContent();
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                    <table>\r\n                                  " +
						"      <tr>\r\n                                            <td>\r\n                  " +
						"                              "));
			global::DevExpress.Web.ASPxCheckBox @__ctrl1;
			@__ctrl1 = this.@__BuildControlchkbUpperCase();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            </td>\r\n                            " +
						"            </tr>\r\n                                        <tr>\r\n               " +
						"                             <td>\r\n                                             " +
						"   "));
			global::DevExpress.Web.ASPxCheckBox @__ctrl2;
			@__ctrl2 = this.@__BuildControlchkbMixedCase();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            </td>\r\n                            " +
						"            </tr>\r\n                                        <tr>\r\n               " +
						"                             <td>\r\n                                             " +
						"   "));
			global::DevExpress.Web.ASPxCheckBox @__ctrl3;
			@__ctrl3 = this.@__BuildControlchkbNumbers();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            </td>\r\n                            " +
						"            </tr>\r\n                                        <tr>\r\n               " +
						"                             <td>\r\n                                             " +
						"   "));
			global::DevExpress.Web.ASPxCheckBox @__ctrl4;
			@__ctrl4 = this.@__BuildControlchkbEmails();
			@__parser.AddParsedSubObject(@__ctrl4);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            </td>\r\n                            " +
						"            </tr>\r\n                                        <tr>\r\n               " +
						"                             <td>\r\n                                             " +
						"   "));
			global::DevExpress.Web.ASPxCheckBox @__ctrl5;
			@__ctrl5 = this.@__BuildControlchkbUrls();
			@__parser.AddParsedSubObject(@__ctrl5);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            </td>\r\n                            " +
						"            </tr>\r\n                                        <tr>\r\n               " +
						"                             <td>\r\n                                             " +
						"  "));
			global::DevExpress.Web.ASPxCheckBox @__ctrl6;
			@__ctrl6 = this.@__BuildControlchkbTags();
			@__parser.AddParsedSubObject(@__ctrl6);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            </td>\r\n                            " +
						"            </tr>\r\n                                    </table>\r\n               " +
						"                 "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control2(DevExpress.Web.PanelCollection @__ctrl) {
			global::DevExpress.Web.PanelContent @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control3();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxRoundPanel @__BuildControlpnlOptions() {
			global::DevExpress.Web.ASPxRoundPanel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxRoundPanel();
			this.pnlOptions = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "pnlOptions";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			this.@__BuildControl__control2(@__ctrl.PanelCollection);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblChooseDictionary() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblChooseDictionary = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblChooseDictionary";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblLanguage() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblLanguage = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblLanguage";
			@__ctrl.AssociatedControlID = "comboLanguage";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxComboBox @__BuildControlcomboLanguage() {
			global::DevExpress.Web.ASPxComboBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxComboBox();
			this.comboLanguage = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "comboLanguage";
			@__ctrl.ClientInstanceName = "comboLanguage";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.PanelContent @__BuildControl__control5() {
			global::DevExpress.Web.PanelContent @__ctrl;
			@__ctrl = new global::DevExpress.Web.PanelContent();
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                    <table style=\"width:100%;\">\r\n              " +
						"                          <tr>\r\n                                            <td " +
						"colspan=\"2\">\r\n                                                "));
			global::DevExpress.Web.ASPxLabel @__ctrl1;
			@__ctrl1 = this.@__BuildControllblChooseDictionary();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class=""dxwscDlgLanguageLabelCell"">
                                                "));
			global::DevExpress.Web.ASPxLabel @__ctrl2;
			@__ctrl2 = this.@__BuildControllblLanguage();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            </td>\r\n                            " +
						"                <td class=\"dxwscDlgLanguageComboCell\">\r\n                        " +
						"                        "));
			global::DevExpress.Web.ASPxComboBox @__ctrl3;
			@__ctrl3 = this.@__BuildControlcomboLanguage();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            </td>\r\n                            " +
						"            </tr>\r\n                                    </table>\r\n               " +
						"                 "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control4(DevExpress.Web.PanelCollection @__ctrl) {
			global::DevExpress.Web.PanelContent @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control5();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxRoundPanel @__BuildControlpnlLanguageSelection() {
			global::DevExpress.Web.ASPxRoundPanel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxRoundPanel();
			this.pnlLanguageSelection = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "pnlLanguageSelection";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			this.@__BuildControl__control4(@__ctrl.PanelCollection);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control6(DevExpress.Web.ButtonClientSideEvents @__ctrl) {
			@__ctrl.Click = "function(s, e) {ASPx.SCDialogComplete(true)}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnOK() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnOK = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnOK";
			@__ctrl.AutoPostBack = false;
			@__ctrl.CssClass = "dxwscDlgFooterBtn";
			@__ctrl.UseSubmitBehavior = false;
			this.@__BuildControl__control6(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control7(DevExpress.Web.ButtonClientSideEvents @__ctrl) {
			@__ctrl.Click = "function(s, e) {ASPx.SCDialogComplete(false)}";
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnCancel() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnCancel = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnCancel";
			@__ctrl.AutoPostBack = false;
			@__ctrl.CssClass = "dxwscDlgFooterBtn";
			@__ctrl.UseSubmitBehavior = false;
			this.@__BuildControl__control7(@__ctrl.ClientSideEvents);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControlTree(SpellCheckOptionsForm @__ctrl) {
			global::DevExpress.Web.ASPxRoundPanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlpnlOptions();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.Web.ASPxRoundPanel @__ctrl2;
			@__ctrl2 = this.@__BuildControlpnlLanguageSelection();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxButton @__ctrl3;
			@__ctrl3 = this.@__BuildControlbtnOK();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::DevExpress.Web.ASPxButton @__ctrl4;
			@__ctrl4 = this.@__BuildControlbtnCancel();
			@__parser.AddParsedSubObject(@__ctrl4);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n\r\n<table id=\"dxMainSpellCheckOptionsFormTable\" ");
									 @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" class=\"dxwscDlgMainSpellCheckOptionsFormTable\">\r\n    <tr>\r\n        <td class=\"dx" +
					"wscDlgContentSCOptionsFormContainer\">\r\n            <table id=\"dxOptionsForm\" cla" +
					"ss=\"dxwscDlgOptionsForm\" ");
														  @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" style=\"width:100%\">\r\n                <tr>\r\n                    <td>\r\n           " +
					"             ");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n       " +
					"             <td class=\"dxwscDlgLanguagePanel\">\r\n                        ");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>  \r\n            </table>       " +
					" \r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <td class=\"dxwscDlgFooter\">\r\n    " +
					"        ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("\r\n            ");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n    </tr>\r\n</table>\r\n\r\n");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
