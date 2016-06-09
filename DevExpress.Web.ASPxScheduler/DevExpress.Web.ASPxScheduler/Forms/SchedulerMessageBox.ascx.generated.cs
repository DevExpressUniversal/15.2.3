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

namespace DevExpress.Web.ASPxScheduler.Forms.Internal {
	[System.CLSCompliantAttribute(false)]
	public partial class SchedulerMessageBox {
		protected global::DevExpress.Web.ASPxLabel lblMessage;
		protected global::DevExpress.Web.ASPxButton btnOk;
		protected global::DevExpress.Web.ASPxButton btnCancel;
		protected global::System.Web.UI.HtmlControls.HtmlGenericControl root;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public SchedulerMessageBox() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/SchedulerMessageBox.ascx";
			if ((global::DevExpress.Web.ASPxScheduler.Forms.Internal.SchedulerMessageBox.@__initialized == false)) {
				global::DevExpress.Web.ASPxScheduler.Forms.Internal.SchedulerMessageBox.@__initialized = true;
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
		private global::DevExpress.Web.ASPxLabel @__BuildControllblMessage() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblMessage = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.EnableClientSideAPI = true;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ID = "lblMessage";
			@__ctrl.Wrap = global::DevExpress.Utils.DefaultBoolean.True;
			@__ctrl.Text = "";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnOk() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnOk = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnOk";
			@__ctrl.UseSubmitBehavior = false;
			@__ctrl.AutoPostBack = false;
			@__ctrl.EnableViewState = false;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(91D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.EnableClientSideAPI = true;
			return @__ctrl;
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
			@__ctrl.UseSubmitBehavior = false;
			@__ctrl.AutoPostBack = false;
			@__ctrl.EnableViewState = false;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(91D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.CausesValidation = false;
			@__ctrl.EnableClientSideAPI = true;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlGenericControl @__BuildControlroot() {
			global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlGenericControl("div");
			this.root = @__ctrl;
			@__ctrl.ID = "root";
			global::DevExpress.Web.ASPxLabel @__ctrl1;
			@__ctrl1 = this.@__BuildControllblMessage();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.Web.ASPxButton @__ctrl2;
			@__ctrl2 = this.@__BuildControlbtnOk();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxButton @__ctrl3;
			@__ctrl3 = this.@__BuildControlbtnCancel();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Renderroot));
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__Renderroot(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n    ");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n    <table ");
   @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" style=\"width: 100%; height: 35px;\">\r\n        <tr>\r\n            <td class=\"dx-ac\"" +
					" style=\"width: 100%; height: 100%;\" ");
														 @__w.Write( DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(this, "center", null) );
			@__w.Write(">\r\n                <table class=\"dxscButtonTable\" style=\"height: 100%\">\r\n        " +
					"            <tr>\r\n                        <td class=\"dxscCellWithPadding\">\r\n    " +
					"                        ");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("\r\n                        </td>\r\n                        <td class=\"dxscCellWithP" +
					"adding\">\r\n                            ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("\r\n                        </td>\r\n                    </tr>\r\n                </tab" +
					"le>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControlTree(SchedulerMessageBox @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlroot();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n\r\n<script id=\"dxss_ASPxSchedulerMessageBox\" type=\"text/javascript\">\r\n    ASPxSc" +
						"hedulerMessageBox = ASPx.CreateClass(ASPxSchedulerMessageBoxBase, {\r\n        Ini" +
						"tialize: function () {\r\n            this.controls.btnOk.Click.AddHandler(ASPx.Cr" +
						"eateDelegate(this.OnBtnOk, this));\r\n            this.controls.btnCancel.Click.Ad" +
						"dHandler(ASPx.CreateDelegate(this.OnBtnCancel, this));\r\n        },\r\n        Upda" +
						"teMessage: function(message) { // from base\r\n            this.controls.lblMessag" +
						"e.SetText(message);\r\n        },\r\n        CalculateDesiredWidth: function () { //" +
						" from base\r\n            var measureDiv = this.CreateMeasureDiv(this.controls.lbl" +
						"Message.GetMainElement());\r\n            measureDiv.innerHTML = this.controls.lbl" +
						"Message.GetText();\r\n            var width = measureDiv.clientWidth;\r\n           " +
						" if (width + 50 > window.innerWidth) \r\n                width = window.innerWidth" +
						" - 50;\r\n            ASPx.SchedulerGlobals.RemoveChildFromParent(document.body, m" +
						"easureDiv);\r\n            return width;\r\n        },\r\n        OnBtnOk: function (s" +
						", e) {\r\n            this.Ok();\r\n        },\r\n        OnBtnCancel: function(s, e) " +
						"{\r\n            this.Cancel();\r\n        },\r\n        CreateMeasureDiv: function (e" +
						"lement) {\r\n            var result = document.createElement(\"div\");\r\n            " +
						"document.body.appendChild(result);\r\n            result.style.cssText = element.s" +
						"tyle.cssText;\r\n            result.style.position = \"absolute\";\r\n            resu" +
						"lt.style.whiteSpace = \"nowrap\";\r\n            result.style.left = \"0px\";\r\n       " +
						"     result.style.top = \"-100px\";\r\n            result.style.width = \"\";\r\n       " +
						"     result.className = element.className;\r\n            return result;\r\n        " +
						"}\r\n    });\r\n</script>\r\n"));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
