#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

namespace DevExpress.ExpressApp.Web.Templates {
	public partial class LogonTemplateContent : DevExpress.ExpressApp.Web.Templates.TemplateContent {
		protected global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl PopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPPopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewImageControl viewImageControl;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl viewCaptionControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPHeader;
		protected global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl ErrorInfo;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPEI;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl viewSiteControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVSC;
		protected global::System.Web.UI.WebControls.TableCell ViewSite;
		protected global::System.Web.UI.WebControls.TableRow TableRow2;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder PopupActions;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPPopupActions;
		protected global::System.Web.UI.WebControls.TableCell TableCell4;
		protected global::System.Web.UI.WebControls.TableRow TableRow3;
		protected global::System.Web.UI.WebControls.Table Table1;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public LogonTemplateContent() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/LogonTemplateContent.ascx";
			if ((global::DevExpress.ExpressApp.Web.Templates.LogonTemplateContent.@__initialized == false)) {
				global::DevExpress.ExpressApp.Web.Templates.LogonTemplateContent.@__initialized = true;
			}
		}
		protected System.Web.Profile.DefaultProfile Profile {
			get {
				return ((System.Web.Profile.DefaultProfile)(this.Context.Profile));
			}
		}
		protected override bool SupportAutoEvents {
			get {
				return false;
			}
		}
		protected System.Web.HttpApplication ApplicationInstance {
			get {
				return ((System.Web.HttpApplication)(this.Context.ApplicationInstance));
			}
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl @__BuildControlPopupWindowControl() {
			global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl();
			this.PopupWindowControl = @__ctrl;
			@__ctrl.ID = "PopupWindowControl";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPPopupWindowControl() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPPopupWindowControl = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPPopupWindowControl";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n    "));
			global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlPopupWindowControl();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__BuildControlviewImageControl() {
			global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ViewImageControl();
			this.viewImageControl = @__ctrl;
			@__ctrl.ID = "viewImageControl";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__BuildControlviewCaptionControl() {
			global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl();
			this.viewCaptionControl = @__ctrl;
			@__ctrl.ID = "viewCaptionControl";
			@__ctrl.DetailViewCaptionMode = global::DevExpress.ExpressApp.Web.Controls.DetailViewCaptionMode.ViewCaption;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPHeader() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPHeader = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPHeader";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n    <div class=\"Header\">\r\n        <table cellpadding=\"0\" cellspacing=\"0\" border" +
						"=\"0\">\r\n            <tr>\r\n                <td class=\"ViewImage\">\r\n               " +
						"     "));
			global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlviewImageControl();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                </td>\r\n                <td class=\"ViewCaption\">\r\n              " +
						"      <h1>\r\n                        "));
			global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__ctrl2;
			@__ctrl2 = this.@__BuildControlviewCaptionControl();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                    </h1>\r\n                </td>\r\n            </tr>\r\n        </" +
						"table>\r\n    </div>\r\n"));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl @__BuildControlErrorInfo() {
			global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl();
			this.ErrorInfo = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "ErrorInfo";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("Style", "margin: 10px 0px 10px 0px");
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPEI() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPEI = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPEI";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                "));
			global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlErrorInfo();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__BuildControlviewSiteControl() {
			global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl();
			this.viewSiteControl = @__ctrl;
			@__ctrl.ID = "viewSiteControl";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPVSC() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPVSC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPVSC";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                            "));
			global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlviewSiteControl();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.TableCell @__BuildControlViewSite() {
			global::System.Web.UI.WebControls.TableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.TableCell();
			this.ViewSite = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "ViewSite";
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlUPVSC();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control3(System.Web.UI.WebControls.TableCellCollection @__ctrl) {
			global::System.Web.UI.WebControls.TableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControlViewSite();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.TableRow @__BuildControlTableRow2() {
			global::System.Web.UI.WebControls.TableRow @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.TableRow();
			this.TableRow2 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "TableRow2";
			this.@__BuildControl__control3(@__ctrl.Cells);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control5(DevExpress.Web.ASPxMenu @__ctrl) {
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ItemAutoWidth = false;
			@__ctrl.HorizontalAlign = global::System.Web.UI.WebControls.HorizontalAlign.Right;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control7() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "PopupActions";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control6(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control7();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlPopupActions() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.PopupActions = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "PopupActions";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("Style", "margin-left: 10px; display: inline");
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Buttons;
			this.@__BuildControl__control5(@__ctrl.Menu);
			this.@__BuildControl__control6(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPPopupActions() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPPopupActions = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPPopupActions";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                            "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlPopupActions();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.TableCell @__BuildControlTableCell4() {
			global::System.Web.UI.WebControls.TableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.TableCell();
			this.TableCell4 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "TableCell4";
			@__ctrl.HorizontalAlign = global::System.Web.UI.WebControls.HorizontalAlign.Right;
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("Style", "padding: 20px 0px 20px 0px");
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlUPPopupActions();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control4(System.Web.UI.WebControls.TableCellCollection @__ctrl) {
			global::System.Web.UI.WebControls.TableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControlTableCell4();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.TableRow @__BuildControlTableRow3() {
			global::System.Web.UI.WebControls.TableRow @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.TableRow();
			this.TableRow3 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "TableRow3";
			this.@__BuildControl__control4(@__ctrl.Cells);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control2(System.Web.UI.WebControls.TableRowCollection @__ctrl) {
			global::System.Web.UI.WebControls.TableRow @__ctrl1;
			@__ctrl1 = this.@__BuildControlTableRow2();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.WebControls.TableRow @__ctrl2;
			@__ctrl2 = this.@__BuildControlTableRow3();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.Table @__BuildControlTable1() {
			global::System.Web.UI.WebControls.Table @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.Table();
			this.Table1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "Table1";
			@__ctrl.CssClass = "Logon";
			@__ctrl.BorderWidth = new System.Web.UI.WebControls.Unit(0D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.CellPadding = 0;
			@__ctrl.CellSpacing = 0;
			this.@__BuildControl__control2(@__ctrl.Rows);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControlTree(LogonTemplateContent @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlUPPopupWindowControl();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl2;
			@__ctrl2 = this.@__BuildControlUPHeader();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n<table class=\"DialogContent Content LogonContent\" border=\"0\" cellpadding=\"0\" ce" +
						"llspacing=\"0\"\r\n    width=\"100%\">\r\n    <tr>\r\n        <td class=\"LogonContentCell\"" +
						" align=\"center\">\r\n            "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl3;
			@__ctrl3 = this.@__BuildControlUPEI();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
			global::System.Web.UI.WebControls.Table @__ctrl4;
			@__ctrl4 = this.@__BuildControlTable1();
			@__parser.AddParsedSubObject(@__ctrl4);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        </td>\r\n    </tr>\r\n</table>\r\n"));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
