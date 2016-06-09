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
	public partial class InplaceEditor {
		protected global::DevExpress.Web.ASPxMemo memSubject;
		protected global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton btnSave;
		protected global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton btnCancel;
		protected global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton btnEditForm;
		protected global::System.Web.UI.HtmlControls.HtmlTableCell buttonContainer;
		protected global::System.Web.UI.HtmlControls.HtmlTable mainContainer;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public InplaceEditor() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/InplaceEditor.ascx";
			if ((global::DevExpress.Web.ASPxScheduler.Forms.Internal.InplaceEditor.@__initialized == false)) {
				global::DevExpress.Web.ASPxScheduler.Forms.Internal.InplaceEditor.@__initialized = true;
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
		private global::DevExpress.Web.ASPxMemo @__BuildControlmemSubject() {
			global::DevExpress.Web.ASPxMemo @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxMemo();
			this.memSubject = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "memSubject";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.Rows = 5;
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingmemSubject);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingmemSubject(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxMemo dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxMemo)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Text = global::System.Convert.ToString( ((AppointmentInplaceEditorTemplateContainer)Container).Appointment.Subject , global::System.Globalization.CultureInfo.CurrentCulture);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControl__control5() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("class", "dx-p2");
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "width:100%");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
			global::DevExpress.Web.ASPxMemo @__ctrl1;
			@__ctrl1 = this.@__BuildControlmemSubject();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton @__BuildControlbtnSave() {
			global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton();
			this.btnSave = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "btnSave";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(19D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.Height = new System.Web.UI.WebControls.Unit(19D, global::System.Web.UI.WebControls.UnitType.Pixel);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton @__BuildControlbtnCancel() {
			global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton();
			this.btnCancel = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "btnCancel";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(19D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.Height = new System.Web.UI.WebControls.Unit(19D, global::System.Web.UI.WebControls.UnitType.Pixel);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton @__BuildControlbtnEditForm() {
			global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton();
			this.btnEditForm = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ClientInstanceName = "_dx";
			@__ctrl.ID = "btnEditForm";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(19D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.Height = new System.Web.UI.WebControls.Unit(19D, global::System.Web.UI.WebControls.UnitType.Pixel);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControlbuttonContainer() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			this.buttonContainer = @__ctrl;
			@__ctrl.ID = "buttonContainer";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("class", "dx-p2");
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "vertical-align: top");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n\r\n        <div>\r\n\t\t\t"));
			global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton @__ctrl1;
			@__ctrl1 = this.@__BuildControlbtnSave();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(" \r\n        </div>\r\n        \r\n        <div style=\"padding-top:1px;\">\r\n\t\t\t"));
			global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton @__ctrl2;
			@__ctrl2 = this.@__BuildControlbtnCancel();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(" \r\n        </div>\r\n\r\n        <div style=\"padding-top:6px;\">\r\n\t\t\t"));
			global::DevExpress.Web.ASPxScheduler.Controls.NoBorderButton @__ctrl3;
			@__ctrl3 = this.@__BuildControlbtnEditForm();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        </div>\r\n        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control4(System.Web.UI.HtmlControls.HtmlTableCellCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control5();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl2;
			@__ctrl2 = this.@__BuildControlbuttonContainer();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableRow @__BuildControl__control3() {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableRow();
			this.@__BuildControl__control4(@__ctrl.Cells);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControl__control2(System.Web.UI.HtmlControls.HtmlTableRowCollection @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTableRow @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control3();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTable @__BuildControlmainContainer() {
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTable();
			this.mainContainer = @__ctrl;
			@__ctrl.ID = "mainContainer";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "width:100%; height:100%");
			this.@__BuildControl__control2(@__ctrl.Rows);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControlTree(InplaceEditor @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl1;
			@__ctrl1 = this.@__BuildControlmainContainer();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
