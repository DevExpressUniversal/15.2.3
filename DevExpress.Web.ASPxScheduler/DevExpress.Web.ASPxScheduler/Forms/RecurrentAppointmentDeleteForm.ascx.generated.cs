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
	public partial class RecurrentAppointmentDeleteForm {
		protected global::DevExpress.Web.ASPxImage imgWarning;
		protected global::DevExpress.Web.ASPxLabel lblConfirm;
		protected global::DevExpress.Web.ASPxRadioButtonList rbAction;
		protected global::DevExpress.Web.ASPxButton btnOk;
		protected global::DevExpress.Web.ASPxButton btnCancel;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public RecurrentAppointmentDeleteForm() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/RecurrentAppointmentDeleteForm.ascx";
			if ((global::DevExpress.Web.ASPxScheduler.Forms.Internal.RecurrentAppointmentDeleteForm.@__initialized == false)) {
				global::DevExpress.Web.ASPxScheduler.Forms.Internal.RecurrentAppointmentDeleteForm.@__initialized = true;
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
		private global::DevExpress.Web.ASPxImage @__BuildControlimgWarning() {
			global::DevExpress.Web.ASPxImage @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxImage();
			this.imgWarning = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "imgWarning";
			@__ctrl.EnableViewState = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblConfirm() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblConfirm = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblConfirm";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control2(DevExpress.Web.BorderWrapper @__ctrl) {
			@__ctrl.BorderStyle = global::System.Web.UI.WebControls.BorderStyle.None;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ListEditItem @__BuildControl__control4() {
			global::DevExpress.Web.ListEditItem @__ctrl;
			@__ctrl = new global::DevExpress.Web.ListEditItem();
			@__ctrl.ValueString = "1";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ListEditItem @__BuildControl__control5() {
			global::DevExpress.Web.ListEditItem @__ctrl;
			@__ctrl = new global::DevExpress.Web.ListEditItem();
			@__ctrl.ValueString = "2";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control3(DevExpress.Web.ListEditItemCollection @__ctrl) {
			global::DevExpress.Web.ListEditItem @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control4();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.Web.ListEditItem @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control5();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxRadioButtonList @__BuildControlrbAction() {
			global::DevExpress.Web.ASPxRadioButtonList @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxRadioButtonList();
			this.rbAction = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "rbAction";
			@__ctrl.ValueType = typeof(int);
			this.@__BuildControl__control2(@__ctrl.Border);
			this.@__BuildControl__control3(@__ctrl.Items);
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
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.UseSubmitBehavior = false;
			@__ctrl.AutoPostBack = false;
			@__ctrl.EnableViewState = false;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(91D, global::System.Web.UI.WebControls.UnitType.Pixel);
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
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.UseSubmitBehavior = false;
			@__ctrl.AutoPostBack = false;
			@__ctrl.EnableViewState = false;
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(91D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.CausesValidation = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControlTree(RecurrentAppointmentDeleteForm @__ctrl) {
			global::DevExpress.Web.ASPxImage @__ctrl1;
			@__ctrl1 = this.@__BuildControlimgWarning();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.Web.ASPxLabel @__ctrl2;
			@__ctrl2 = this.@__BuildControllblConfirm();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxRadioButtonList @__ctrl3;
			@__ctrl3 = this.@__BuildControlrbAction();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::DevExpress.Web.ASPxButton @__ctrl4;
			@__ctrl4 = this.@__BuildControlbtnOk();
			@__parser.AddParsedSubObject(@__ctrl4);
			global::DevExpress.Web.ASPxButton @__ctrl5;
			@__ctrl5 = this.@__BuildControlbtnCancel();
			@__parser.AddParsedSubObject(@__ctrl5);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n\r\n<table class=\"dxscBorderSpacing\" style=\"width:100%; height:100%\">\r\n    <tr>\r\n" +
					"        <td class=\"dxscCellWithPadding\" rowspan=\"2\"  style=\"vertical-align:top;\"" +
					">\r\n            ");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n        <td class=\"dxscCellWithPadding\" style=\"width:100%;\">\r\n  " +
					"          ");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <td class=\"dxscCellWithPadding\" sty" +
					"le=\"width:100%;\">\r\n            ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <td class=\"dx-ac dxscCellWithPaddin" +
					"g\" ");
									  @__w.Write( DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(this, "center", null) );
			@__w.Write(" style=\"width:100%\" colspan=\"2\">\r\n            <table class=\"dxscButtonTable\">\r\n  " +
					"              <tr>\r\n                    <td class=\"dxscCellWithPadding\">\r\n      " +
					"                  ");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscCellWithPadding\">" +
					"\r\n                        ");
			parameterContainer.Controls[4].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n    </tr>\r\n</table>    \r\n");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
