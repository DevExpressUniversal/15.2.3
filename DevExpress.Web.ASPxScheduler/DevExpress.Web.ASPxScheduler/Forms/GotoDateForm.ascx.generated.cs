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
	public partial class GotoDateForm {
		protected global::DevExpress.Web.ASPxLabel lblDate;
		protected global::DevExpress.Web.ASPxDateEdit edtDate;
		protected global::DevExpress.Web.ASPxLabel lblView;
		protected global::DevExpress.Web.ASPxComboBox cbView;
		protected global::DevExpress.Web.ASPxButton btnOk;
		protected global::DevExpress.Web.ASPxButton btnCancel;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public GotoDateForm() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/GotoDateForm.ascx";
			if ((global::DevExpress.Web.ASPxScheduler.Forms.Internal.GotoDateForm.@__initialized == false)) {
				global::DevExpress.Web.ASPxScheduler.Forms.Internal.GotoDateForm.@__initialized = true;
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
		private global::DevExpress.Web.ASPxLabel @__BuildControllblDate() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblDate = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblDate";
			@__ctrl.AssociatedControlID = "edtDate";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxDateEdit @__BuildControledtDate() {
			global::DevExpress.Web.ASPxDateEdit @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxDateEdit();
			this.edtDate = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "edtDate";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingedtDate);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingedtDate(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxDateEdit dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxDateEdit)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Date = ((System.DateTime)(((GotoDateFormTemplateContainer)Container).Date ));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblView() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblView = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblView";
			@__ctrl.AssociatedControlID = "cbView";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxComboBox @__BuildControlcbView() {
			global::DevExpress.Web.ASPxComboBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxComboBox();
			this.cbView = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "cbView";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingcbView);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingcbView(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxComboBox dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxComboBox)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.DataSource = ((object)(((GotoDateFormTemplateContainer)Container).ViewsDataSource ));
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
		private void @__BuildControlTree(GotoDateForm @__ctrl) {
			global::DevExpress.Web.ASPxLabel @__ctrl1;
			@__ctrl1 = this.@__BuildControllblDate();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.Web.ASPxDateEdit @__ctrl2;
			@__ctrl2 = this.@__BuildControledtDate();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxLabel @__ctrl3;
			@__ctrl3 = this.@__BuildControllblView();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::DevExpress.Web.ASPxComboBox @__ctrl4;
			@__ctrl4 = this.@__BuildControlcbView();
			@__parser.AddParsedSubObject(@__ctrl4);
			global::DevExpress.Web.ASPxButton @__ctrl5;
			@__ctrl5 = this.@__BuildControlbtnOk();
			@__parser.AddParsedSubObject(@__ctrl5);
			global::DevExpress.Web.ASPxButton @__ctrl6;
			@__ctrl6 = this.@__BuildControlbtnCancel();
			@__parser.AddParsedSubObject(@__ctrl6);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n    \r\n<table class=\"dxscBorderSpacing\" style=\"width:100%; height:100%\">\r\n    <t" +
					"r>\r\n        <td class=\"dxscCellWithPadding\">\r\n            ");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n        <td class=\"dxscCellWithPadding\" style=\"width:100%\">\r\n   " +
					"         ");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("\r\n        </td> \r\n    </tr>\r\n    <tr>\r\n        <td class=\"dxscCellWithPadding\">\r\n" +
					"            <span style=\"white-space: nowrap;\">\r\n            ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("\r\n            </span>\r\n        </td>\r\n        <td class=\"dxscCellWithPadding\" sty" +
					"le=\"width:100%\">\r\n            ");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write("\r\n        </td>\r\n    </tr>\r\n    <tr>\r\n        <td class=\"dx-ac dxscCellWithPaddin" +
					"g\" ");
									  @__w.Write( DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(this, "center", null) );
			@__w.Write(" colspan=\"2\" style=\"width: 100%\">\r\n            <table class=\"dxscButtonTable\">\r\n " +
					"               <tr>\r\n                    <td class=\"dxscCellWithPadding\">\r\n     " +
					"                   ");
			parameterContainer.Controls[4].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                    <td class=\"dxscCellWithPadding\">" +
					"\r\n                        ");
			parameterContainer.Controls[5].RenderControl(@__w);
			@__w.Write("\r\n                    </td>\r\n                </tr>\r\n            </table>\r\n       " +
					" </td>\r\n    </tr>\r\n</table>    \r\n\r\n\r\n");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
