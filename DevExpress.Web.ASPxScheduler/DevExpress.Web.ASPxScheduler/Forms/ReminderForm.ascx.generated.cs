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
	public partial class ReminderForm {
		protected global::DevExpress.Web.ASPxListBox lbItems;
		protected global::DevExpress.Web.ASPxButton btnDismissAll;
		protected global::DevExpress.Web.ASPxButton btnDismiss;
		protected global::DevExpress.Web.ASPxLabel lblClickSnooze;
		protected global::DevExpress.Web.ASPxComboBox cbSnooze;
		protected global::DevExpress.Web.ASPxButton btnSnooze;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public ReminderForm() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/ReminderForm.ascx";
			if ((global::DevExpress.Web.ASPxScheduler.Forms.Internal.ReminderForm.@__initialized == false)) {
				global::DevExpress.Web.ASPxScheduler.Forms.Internal.ReminderForm.@__initialized = true;
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
		private global::DevExpress.Web.ASPxListBox @__BuildControllbItems() {
			global::DevExpress.Web.ASPxListBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxListBox();
			this.lbItems = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lbItems";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "padding-bottom:15px;");
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnDismissAll() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnDismissAll = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnDismissAll";
			@__ctrl.AutoPostBack = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnDismiss() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnDismiss = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnDismiss";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(80D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.AutoPostBack = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblClickSnooze() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblClickSnooze = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "lblClickSnooze";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxComboBox @__BuildControlcbSnooze() {
			global::DevExpress.Web.ASPxComboBox @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxComboBox();
			this.cbSnooze = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "cbSnooze";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxButton @__BuildControlbtnSnooze() {
			global::DevExpress.Web.ASPxButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxButton();
			this.btnSnooze = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "btnSnooze";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(80D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.AutoPostBack = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControlTree(ReminderForm @__ctrl) {
			global::DevExpress.Web.ASPxListBox @__ctrl1;
			@__ctrl1 = this.@__BuildControllbItems();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.Web.ASPxButton @__ctrl2;
			@__ctrl2 = this.@__BuildControlbtnDismissAll();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxButton @__ctrl3;
			@__ctrl3 = this.@__BuildControlbtnDismiss();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::DevExpress.Web.ASPxLabel @__ctrl4;
			@__ctrl4 = this.@__BuildControllblClickSnooze();
			@__parser.AddParsedSubObject(@__ctrl4);
			global::DevExpress.Web.ASPxComboBox @__ctrl5;
			@__ctrl5 = this.@__BuildControlcbSnooze();
			@__parser.AddParsedSubObject(@__ctrl5);
			global::DevExpress.Web.ASPxButton @__ctrl6;
			@__ctrl6 = this.@__BuildControlbtnSnooze();
			@__parser.AddParsedSubObject(@__ctrl6);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n\r\n<table class=\"dxscBorderSpacing\" ");
						 @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" style=\"width:100%; padding-bottom:15px;\">\r\n    <tr><td> \r\n         ");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n    </td></tr>\r\n</table>\r\n<table class=\"dxscButtonTable\" style=\"width: 100%\" ");
										   @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(">\r\n    <tr>\r\n        <td style=\"width:100%;\">");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("</td>\r\n        <td class=\"dx-ar\" style=\"width:80px;\" ");
									  @__w.Write( DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(this, "right", null) );
			@__w.Write(">\r\n            ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("</td>\r\n    </tr>\r\n    <tr>\r\n        <td colspan=\"2\" style=\"padding:8px 0 4px 0;\">" +
					"");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write("</td>\r\n    </tr>\r\n    <tr>\r\n        <td style=\"width:100%;padding-right:20px;\">");
			parameterContainer.Controls[4].RenderControl(@__w);
			@__w.Write("</td>\r\n        <td style=\"width:80px;\">");
			parameterContainer.Controls[5].RenderControl(@__w);
			@__w.Write("</td>\r\n    </tr>\r\n</table>\r\n\r\n\r\n\r\n\r\n\r\n");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
