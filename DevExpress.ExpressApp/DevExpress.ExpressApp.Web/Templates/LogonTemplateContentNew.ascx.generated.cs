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
	public partial class LogonTemplateContentNew : DevExpress.ExpressApp.Web.Templates.TemplateContent {
		protected global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl PopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPPopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPHeader;
		protected global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl ErrorInfo;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPEI;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl viewSiteControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVSC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder PopupActions;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPPopupActions;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public LogonTemplateContentNew() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/LogonTemplateContentNew.ascx";
			if ((global::DevExpress.ExpressApp.Web.Templates.LogonTemplateContentNew.@__initialized == false)) {
				global::DevExpress.ExpressApp.Web.Templates.LogonTemplateContentNew.@__initialized = true;
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
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPHeader() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPHeader = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPHeader";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
    <div class=""white borderBottom width100"" id=""headerTableDiv"">
        <div class=""paddings sizeLimit"" style=""margin: auto"">
            <table id=""headerTable"" class=""headerTable xafAlignCenter white width100 sizeLimit"" style=""height: 60px;"">
                <tbody>
                    <tr>
                        <td>
                            <img src=""Images/Logo.png"" />
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
"));
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
			@__ctrl.CssClass = "LogonContentWidth";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                    "));
			global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlErrorInfo();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                "));
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                "));
			global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlviewSiteControl();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control2(DevExpress.Web.ASPxMenu @__ctrl) {
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ItemAutoWidth = false;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control4() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "PopupActions";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control3(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control4();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlPopupActions() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.PopupActions = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "PopupActions";
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Buttons;
			this.@__BuildControl__control2(@__ctrl.Menu);
			this.@__BuildControl__control3(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPPopupActions() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPPopupActions = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPPopupActions";
			@__ctrl.CssClass = "right";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlPopupActions();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControlTree(LogonTemplateContentNew @__ctrl) {
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n<meta name=\"viewport\" content=\"width=device-width, user-scalable=no, maximum-sc" +
						"ale=1.0, minimum-scale=1.0\">\r\n<style type=\"text/css\">\r\n    .right\r\n    {\r\n      " +
						"  float: right;\r\n    }\r\n\r\n    .CardGroupContent\r\n    {\r\n        padding: 0px !im" +
						"portant;\r\n    }\r\n\r\n    .CardGroupBase\r\n    {\r\n        border: none !important;\r\n" +
						"    }\r\n\r\n    .menuButtons .dxm-item\r\n    {\r\n        border: none !important;\r\n  " +
						"      background-color: #2c86d3 !important;\r\n        padding-left: 3px !importan" +
						"t;\r\n        padding-right: 3px !important;\r\n    }\r\n\r\n        .menuButtons .dxm-i" +
						"tem .dx-vam\r\n        {\r\n            color: white;\r\n        }\r\n\r\n        .menuBut" +
						"tons .dxm-item.dxm-hovered, .menuButtons .dxm-item.dxm-hovered a.dx\r\n        {\r\n" +
						"            border: none !important;\r\n            background-color: #2c86d3 !imp" +
						"ortant;\r\n        }\r\n\r\n        .menuButtons .dxm-noImages .dxm-item .dxm-content," +
						" .menuButtons .dxm-item.dxm-noImage .dxm-content\r\n        {\r\n            padding" +
						": 14px 38px 14px 38px !important;\r\n            -webkit-box-shadow: none;\r\n      " +
						"      border: none;\r\n        }\r\n\r\n    .LogonItemClassCSS\r\n    {\r\n        padding" +
						"-bottom: 15px;\r\n    }\r\n\r\n    .LogonTextClassCSS\r\n    {\r\n        padding-top: 0px" +
						";\r\n        padding-bottom: 38px;\r\n        text-align: left;\r\n    }\r\n\r\n        .L" +
						"ogonTextClassCSS .StaticText\r\n        {\r\n            font-size: 25px;\r\n         " +
						"   color: #4a4a4a;\r\n        }\r\n\r\n        .LogonTextClassCSS .dxm-tmpl td.dxic > " +
						"input, .WebEditorCell td.dxic > input\r\n        {\r\n            padding-top: 8px !" +
						"important;\r\n            padding-bottom: 8px !important;\r\n        }\r\n\r\n\r\n    .Pas" +
						"swordHintClassCSS\r\n    {\r\n        text-align: left;\r\n        padding-bottom: 20p" +
						"x;\r\n    }\r\n\r\n        .PasswordHintClassCSS .StaticText\r\n        {\r\n            f" +
						"ont-size: 12px;\r\n        }\r\n</style>\r\n"));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlUPPopupWindowControl();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl2;
			@__ctrl2 = this.@__BuildControlUPHeader();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n\r\n<div style=\"top: 25%; width: 100%; position: absolute\">\r\n    <table class=\"Lo" +
						"gonMainTable LogonContentWidth\">\r\n        <tr>\r\n            <td>\r\n              " +
						"  "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl3;
			@__ctrl3 = this.@__BuildControlUPEI();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n            <td>\r\n             " +
						"   <table class=\"LogonContent LogonContentWidth\">\r\n                    <tr>\r\n   " +
						"                     <td class=\"LogonContentCell\">\r\n                            " +
						""));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl4;
			@__ctrl4 = this.@__BuildControlUPVSC();
			@__parser.AddParsedSubObject(@__ctrl4);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n\r\n                            "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl5;
			@__ctrl5 = this.@__BuildControlUPPopupActions();
			@__parser.AddParsedSubObject(@__ctrl5);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        </td>\r\n                    </tr>\r\n                </tab" +
						"le>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n\r\n</div>\r\n"));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
