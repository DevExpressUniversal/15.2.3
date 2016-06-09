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
	public partial class HorizontalSameDayAppointmentTemplate {
		protected global::System.Web.UI.HtmlControls.HtmlTableCell statusContainer;
		protected global::System.Web.UI.HtmlControls.HtmlTableCell startTimeClockContainer;
		protected global::DevExpress.Web.ASPxLabel lblStartTime;
		protected global::System.Web.UI.HtmlControls.HtmlTableCell endTimeClockContainer;
		protected global::DevExpress.Web.ASPxLabel lblEndTime;
		protected global::System.Web.UI.HtmlControls.HtmlTable imageContainer;
		protected global::DevExpress.Web.ASPxLabel lblTitle;
		protected global::System.Web.UI.HtmlControls.HtmlGenericControl appointmentDiv;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public HorizontalSameDayAppointmentTemplate() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/HorizontalSameDayAppointmentTemplate.ascx";
			if ((global::DevExpress.Web.ASPxScheduler.Forms.Internal.HorizontalSameDayAppointmentTemplate.@__initialized == false)) {
				global::DevExpress.Web.ASPxScheduler.Forms.Internal.HorizontalSameDayAppointmentTemplate.@__initialized = true;
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
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControlstatusContainer() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			this.statusContainer = @__ctrl;
			@__ctrl.ID = "statusContainer";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "vertical-align: top");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("    \r\n            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControlstartTimeClockContainer() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			this.startTimeClockContainer = @__ctrl;
			@__ctrl.ID = "startTimeClockContainer";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("class", "dxscCellWithPadding");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(" \r\n                        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblStartTime() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblStartTime = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.EnableViewState = false;
			@__ctrl.EncodeHtml = true;
			@__ctrl.ID = "lblStartTime";
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindinglblStartTime);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindinglblStartTime(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxLabel dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxLabel)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Text = global::System.Convert.ToString(((HorizontalAppointmentTemplateContainer)Container).Items.StartTimeText.Text, global::System.Globalization.CultureInfo.CurrentCulture);
			dataBindingExpressionBuilderTarget.Visible = ((bool)(((HorizontalAppointmentTemplateContainer)Container).Items.StartTimeText.Visible));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTableCell @__BuildControlendTimeClockContainer() {
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTableCell("td");
			this.endTimeClockContainer = @__ctrl;
			@__ctrl.ID = "endTimeClockContainer";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("class", "dxscCellWithPadding");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblEndTime() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblEndTime = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.EnableViewState = false;
			@__ctrl.EncodeHtml = true;
			@__ctrl.ID = "lblEndTime";
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindinglblEndTime);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindinglblEndTime(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxLabel dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxLabel)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Text = global::System.Convert.ToString(((HorizontalAppointmentTemplateContainer)Container).Items.EndTimeText.Text, global::System.Globalization.CultureInfo.CurrentCulture);
			dataBindingExpressionBuilderTarget.Visible = ((bool)(((HorizontalAppointmentTemplateContainer)Container).Items.EndTimeText.Visible));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlTable @__BuildControlimageContainer() {
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlTable();
			this.imageContainer = @__ctrl;
			@__ctrl.ID = "imageContainer";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "vertical-align: middle;");
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::DevExpress.Web.ASPxLabel @__BuildControllblTitle() {
			global::DevExpress.Web.ASPxLabel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxLabel();
			this.lblTitle = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.EnableViewState = false;
			@__ctrl.EncodeHtml = true;
			@__ctrl.ID = "lblTitle";
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindinglblTitle);
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindinglblTitle(object sender, System.EventArgs e) {
			DevExpress.Web.ASPxLabel dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((DevExpress.Web.ASPxLabel)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			dataBindingExpressionBuilderTarget.Text = global::System.Convert.ToString(((HorizontalAppointmentTemplateContainer)Container).Items.Title.Text, global::System.Globalization.CultureInfo.CurrentCulture);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private global::System.Web.UI.HtmlControls.HtmlGenericControl @__BuildControlappointmentDiv() {
			global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlGenericControl("div");
			this.appointmentDiv = @__ctrl;
			@__ctrl.ID = "appointmentDiv";
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControlstatusContainer();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl2;
			@__ctrl2 = this.@__BuildControlstartTimeClockContainer();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxLabel @__ctrl3;
			@__ctrl3 = this.@__BuildControllblStartTime();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::System.Web.UI.HtmlControls.HtmlTableCell @__ctrl4;
			@__ctrl4 = this.@__BuildControlendTimeClockContainer();
			@__parser.AddParsedSubObject(@__ctrl4);
			global::DevExpress.Web.ASPxLabel @__ctrl5;
			@__ctrl5 = this.@__BuildControllblEndTime();
			@__parser.AddParsedSubObject(@__ctrl5);
			global::System.Web.UI.HtmlControls.HtmlTable @__ctrl6;
			@__ctrl6 = this.@__BuildControlimageContainer();
			@__parser.AddParsedSubObject(@__ctrl6);
			global::DevExpress.Web.ASPxLabel @__ctrl7;
			@__ctrl7 = this.@__BuildControllblTitle();
			@__parser.AddParsedSubObject(@__ctrl7);
			@__ctrl.DataBinding += new System.EventHandler(this.@__DataBindingappointmentDiv);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__RenderappointmentDiv));
			return @__ctrl;
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__RenderappointmentDiv(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n    <table ");
   @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 0, 0) );
			@__w.Write(" style=\"width: 100%\">\r\n        <tr>\r\n            ");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n        </tr>\r\n        <tr>\r\n            <td>\r\n                <table ");
			   @__w.Write( DevExpress.Web.Internal.RenderUtils.GetTableSpacings(this, 1, 0) );
			@__w.Write(" style=\"width: 100%\">\r\n                    <tr class=\"dx-al\" ");
							  @__w.Write( DevExpress.Web.Internal.RenderUtils.GetAlignAttributes(this, "left", "middle") );
			@__w.Write(" style=\"vertical-align: middle;\">\r\n                        ");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("\r\n                        <td class=\"dxscCellWithPadding\">\r\n                     " +
					"       ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("            \r\n                        </td>\r\n                        ");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write("\r\n                        <td class=\"dxscCellWithPadding\">\r\n                     " +
					"       ");
			parameterContainer.Controls[4].RenderControl(@__w);
			@__w.Write("\r\n                        </td>\r\n                        <td class=\"dxscCellWithP" +
					"adding\">\r\n                            ");
			parameterContainer.Controls[5].RenderControl(@__w);
			@__w.Write("\r\n                        </td>\r\n                        <td class=\"dxscCellWithP" +
					"adding\" style=\"width: 100%\">\r\n                            ");
			parameterContainer.Controls[6].RenderControl(@__w);
			@__w.Write("            \r\n                        </td>\r\n                    </tr>\r\n         " +
					"       </table>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n");
		}
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		public void @__DataBindingappointmentDiv(object sender, System.EventArgs e) {
			System.Web.UI.HtmlControls.HtmlGenericControl dataBindingExpressionBuilderTarget;
			System.Web.UI.Control Container;
			dataBindingExpressionBuilderTarget = ((System.Web.UI.HtmlControls.HtmlGenericControl)(sender));
			Container = ((System.Web.UI.Control)(dataBindingExpressionBuilderTarget.BindingContainer));
			((System.Web.UI.IAttributeAccessor)(dataBindingExpressionBuilderTarget)).SetAttribute("class", global::System.Convert.ToString(((HorizontalAppointmentTemplateContainer)Container).Items.AppointmentStyle.CssClass , global::System.Globalization.CultureInfo.CurrentCulture));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[System.CodeDom.Compiler.GeneratedCode("Suppress FxCop check", "")]
		private void @__BuildControlTree(HorizontalSameDayAppointmentTemplate @__ctrl) {
			global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlappointmentDiv();
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
