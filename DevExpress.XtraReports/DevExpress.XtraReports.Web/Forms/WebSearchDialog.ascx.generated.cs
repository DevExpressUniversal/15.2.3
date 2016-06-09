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

namespace DevExpress.XtraReports.Web.Forms {
	[System.CLSCompliantAttribute(false)]
	public partial class WebSearchDialog {
		protected global::DevExpress.Web.ASPxLabel labelFindWhat;
		protected global::DevExpress.Web.ASPxTextBox textBoxFindText;
		protected global::System.Web.UI.HtmlControls.HtmlTable Table2;
		protected global::DevExpress.Web.ASPxButton buttonFind;
		protected global::DevExpress.Web.ASPxCheckBox checkMatchWholeWord;
		protected global::DevExpress.Web.ASPxRadioButton radioUp;
		protected global::DevExpress.Web.ASPxCheckBox checkMatchCase;
		protected global::DevExpress.Web.ASPxRadioButton radioDown;
		protected global::System.Web.UI.HtmlControls.HtmlTable Table3;
		protected global::DevExpress.Web.ASPxButton buttonCancel;
		protected global::System.Web.UI.HtmlControls.HtmlTable Table1;
		protected global::DevExpress.Web.PanelContent PanelContent1;
		protected global::DevExpress.Web.ASPxPanel ASPxPopupControl1;
		protected global::DevExpress.Web.PopupControlContentControl PopupControlContentControl1;
		protected global::DevExpress.Web.ASPxPopupControl aspxPopupControl;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public WebSearchDialog() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/WebSearchDialog.ascx";
			if ((global::DevExpress.XtraReports.Web.Forms.WebSearchDialog.@__initialized == false)) {
				global::DevExpress.XtraReports.Web.Forms.WebSearchDialog.@__initialized = true;
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
		private global::DevExpress.Web.ASPxLabel @__BuildControllabelFindWhat() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.labelFindWhat = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "labelFindWhat";
			@__ctrl.Text = "Find what:";
			@__ctrl.AssociatedControlID = "FindText";
			@__ctrl.EncodeHtml = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control11() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "width: 1%");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			global::DevExpress.Web.ASPxLabel @__ctrl1;
			@__ctrl1 = this.@__BuildControllabelFindWhat();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                      "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control12() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                          &nbsp;"));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxTextBox @__BuildControltextBoxFindText() {
			global::DevExpress.Web.ASPxTextBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxTextBox();
			this.textBoxFindText = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "textBoxFindText";
			@__ctrl.Text = "";
			@__ctrl.TabIndex = 0;
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "display: block;");
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control13() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			global::DevExpress.Web.ASPxTextBox @__ctrl1;
			@__ctrl1 = this.@__BuildControltextBoxFindText();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        <div style=\"height: 1px; width: 150px; border-width: 0p" +
						"x; overflow: hidden\">\r\n                        </div>\r\n                      "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control10(System.Web.UI.HtmlControls.HtmlTableCellCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control11();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control12();
			@__ctrl.Add(@__ctrl2);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl3;
			@__ctrl3 = this.@__BuildControl__control13();
			@__ctrl.Add(@__ctrl3);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableRow @__BuildControl__control9() {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableRow();
			this.@__BuildControl__control10(@__ctrl.Cells);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control8(System.Web.UI.HtmlControls.HtmlTableRowCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control9();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTable @__BuildControlTable2() {
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTable();
			this.Table2 = @__ctrl;
			@__ctrl.ID = "Table2";
			this.@__BuildControl__control8(@__ctrl.Rows);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control7() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                  "));
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl1;
			@__ctrl1 = this.@__BuildControlTable2();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control14() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "width: 0px");
			@__ctrl.RowSpan = 2;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                  &nbsp;"));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbuttonFind() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.buttonFind = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "buttonFind";
			@__ctrl.Text = "Find Next";
			@__ctrl.AutoPostBack = false;
			@__ctrl.TabIndex = 4;
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "display: block");
			@__ctrl.EnableClientSideAPI = true;
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.False;
			@__ctrl.EncodeHtml = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control15() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("class", "dx-valt");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                    "));
			global::DevExpress.Web.ASPxButton @__ctrl1;
			@__ctrl1 = this.@__BuildControlbuttonFind();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                  "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control6(System.Web.UI.HtmlControls.HtmlTableCellCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control7();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control14();
			@__ctrl.Add(@__ctrl2);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl3;
			@__ctrl3 = this.@__BuildControl__control15();
			@__ctrl.Add(@__ctrl3);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableRow @__BuildControl__control5() {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableRow();
			this.@__BuildControl__control6(@__ctrl.Cells);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlcheckMatchWholeWord() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.checkMatchWholeWord = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "checkMatchWholeWord";
			@__ctrl.TabIndex = 1;
			@__ctrl.Text = "Match whole word only";
			@__ctrl.EncodeHtml = false;
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.False;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control22() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			global::DevExpress.Web.ASPxCheckBox @__ctrl1;
			@__ctrl1 = this.@__BuildControlcheckMatchWholeWord();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                      "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control23() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			@__ctrl.RowSpan = 2;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                      "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxRadioButton @__BuildControlradioUp() {
			global::DevExpress.Web.ASPxRadioButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxRadioButton();
			this.radioUp = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "radioUp";
			@__ctrl.TabIndex = 3;
			@__ctrl.Text = "Up";
			@__ctrl.GroupName = "Direction";
			@__ctrl.EncodeHtml = false;
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.False;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control24() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			global::DevExpress.Web.ASPxRadioButton @__ctrl1;
			@__ctrl1 = this.@__BuildControlradioUp();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                      "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control21(System.Web.UI.HtmlControls.HtmlTableCellCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control22();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control23();
			@__ctrl.Add(@__ctrl2);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl3;
			@__ctrl3 = this.@__BuildControl__control24();
			@__ctrl.Add(@__ctrl3);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableRow @__BuildControl__control20() {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableRow();
			this.@__BuildControl__control21(@__ctrl.Cells);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxCheckBox @__BuildControlcheckMatchCase() {
			global::DevExpress.Web.ASPxCheckBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxCheckBox();
			this.checkMatchCase = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "checkMatchCase";
			@__ctrl.TabIndex = 2;
			@__ctrl.Text = "Match case";
			@__ctrl.EncodeHtml = false;
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.False;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control27() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			global::DevExpress.Web.ASPxCheckBox @__ctrl1;
			@__ctrl1 = this.@__BuildControlcheckMatchCase();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                      "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxRadioButton @__BuildControlradioDown() {
			global::DevExpress.Web.ASPxRadioButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxRadioButton();
			this.radioDown = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "radioDown";
			@__ctrl.TabIndex = 3;
			@__ctrl.Text = "Down";
			@__ctrl.Checked = true;
			@__ctrl.GroupName = "Direction";
			@__ctrl.EncodeHtml = false;
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.False;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control28() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			global::DevExpress.Web.ASPxRadioButton @__ctrl1;
			@__ctrl1 = this.@__BuildControlradioDown();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                      "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control26(System.Web.UI.HtmlControls.HtmlTableCellCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control27();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control28();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableRow @__BuildControl__control25() {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableRow();
			this.@__BuildControl__control26(@__ctrl.Cells);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control19(System.Web.UI.HtmlControls.HtmlTableRowCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control20();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control25();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTable @__BuildControlTable3() {
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTable();
			this.Table3 = @__ctrl;
			@__ctrl.ID = "Table3";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "width: 100%;");
			this.@__BuildControl__control19(@__ctrl.Rows);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control18() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("class", "dx-valt");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                  "));
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl1;
			@__ctrl1 = this.@__BuildControlTable3();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbuttonCancel() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.buttonCancel = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "buttonCancel";
			@__ctrl.Text = "Cancel";
			@__ctrl.AutoPostBack = false;
			@__ctrl.TabIndex = 5;
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "display: block;");
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.False;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control29() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                    "));
			global::DevExpress.Web.ASPxButton @__ctrl1;
			@__ctrl1 = this.@__BuildControlbuttonCancel();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control17(System.Web.UI.HtmlControls.HtmlTableCellCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control18();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control29();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableRow @__BuildControl__control16() {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableRow();
			this.@__BuildControl__control17(@__ctrl.Cells);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control4(System.Web.UI.HtmlControls.HtmlTableRowCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control5();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control16();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTable @__BuildControlTable1() {
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTable();
			this.Table1 = @__ctrl;
			@__ctrl.ID = "Table1";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "width: 100%;");
			this.@__BuildControl__control4(@__ctrl.Rows);
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
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl1;
			@__ctrl1 = this.@__BuildControlTable1();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n          "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control3(DevExpress.Web.PanelCollection @__ctrl) {
			global::DevExpress.Web.PanelContent @__ctrl1;
			@__ctrl1 = this.@__BuildControlPanelContent1();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxPanel @__BuildControlASPxPopupControl1() {
			global::DevExpress.Web.ASPxPanel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxPanel();
			this.ASPxPopupControl1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "ASPxPopupControl1";
			@__ctrl.DefaultButton = "buttonFind";
			this.@__BuildControl__control3(@__ctrl.PanelCollection);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.PopupControlContentControl @__BuildControlPopupControlContentControl1() {
			global::DevExpress.Web.PopupControlContentControl @__ctrl;
			@__ctrl = new global::DevExpress.Web.PopupControlContentControl();
			this.PopupControlContentControl1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "PopupControlContentControl1";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n      "));
			global::DevExpress.Web.ASPxPanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlASPxPopupControl1();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n    "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control2(DevExpress.Web.ContentControlCollection @__ctrl) {
			global::DevExpress.Web.PopupControlContentControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlPopupControlContentControl1();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxPopupControl @__BuildControlaspxPopupControl() {
			global::DevExpress.Web.ASPxPopupControl @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxPopupControl();
			this.aspxPopupControl = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "aspxPopupControl";
			@__ctrl.HeaderText = "Search";
			@__ctrl.AllowDragging = true;
			@__ctrl.PopupHorizontalAlign = global::DevExpress.Web.PopupHorizontalAlign.WindowCenter;
			@__ctrl.PopupVerticalAlign = global::DevExpress.Web.PopupVerticalAlign.WindowCenter;
			@__ctrl.AutoUpdatePosition = true;
			@__ctrl.CloseAction = global::DevExpress.Web.CloseAction.CloseButton;
			@__ctrl.CssClass = "dxxrpcSearch";
			this.@__BuildControl__control2(@__ctrl.ContentCollection);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControlTree(WebSearchDialog @__ctrl) {
			global::DevExpress.Web.ASPxPopupControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlaspxPopupControl();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
