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
	public partial class DialogTemplateContentNew : DevExpress.ExpressApp.Web.Templates.TemplateContent {
		protected global::DevExpress.Web.ASPxHiddenField ClientParams;
		protected global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl PopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPPopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewImageControl VIC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVIC;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl VCC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVH;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder SAC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPSAC;
		protected global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl ErrorInfo;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPEI;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl VSC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVSC;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public DialogTemplateContentNew() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/DialogTemplateContentNew.ascx";
			if ((global::DevExpress.ExpressApp.Web.Templates.DialogTemplateContentNew.@__initialized == false)) {
				global::DevExpress.ExpressApp.Web.Templates.DialogTemplateContentNew.@__initialized = true;
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
		private global::DevExpress.Web.ASPxHiddenField @__BuildControlClientParams() {
			global::DevExpress.Web.ASPxHiddenField @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxHiddenField();
			this.ClientParams = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "ClientParams";
			@__ctrl.ClientInstanceName = "ClientParams";
			return @__ctrl;
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
		private global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__BuildControlVIC() {
			global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ViewImageControl();
			this.VIC = @__ctrl;
			@__ctrl.ID = "VIC";
			@__ctrl.CssClass = "ViewImage";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPVIC() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPVIC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPVIC";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            "));
			global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVIC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__BuildControlVCC() {
			global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl();
			this.VCC = @__ctrl;
			@__ctrl.ID = "VCC";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPVH() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPVH = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPVH";
			@__ctrl.ForeColor = ((System.Drawing.Color)(global::System.Drawing.Color.FromArgb(74, 74, 74)));
			@__ctrl.Font.Size = global::System.Web.UI.WebControls.FontUnit.XLarge;
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("Style", "white-space: normal;");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            "));
			global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVCC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control3() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Search";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control4() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "FullTextSearch";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control5() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "ObjectsCreation";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control6() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "PopupActions";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control2(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control3();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control4();
			@__ctrl.Add(@__ctrl2);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl3;
			@__ctrl3 = this.@__BuildControl__control5();
			@__ctrl.Add(@__ctrl3);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl4;
			@__ctrl4 = this.@__BuildControl__control6();
			@__ctrl.Add(@__ctrl4);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlSAC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.SAC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "SAC";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Buttons;
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			this.@__BuildControl__control2(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPSAC() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPSAC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPSAC";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlSAC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                            "));
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
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
			global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlErrorInfo();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__BuildControlVSC() {
			global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl();
			this.VSC = @__ctrl;
			@__ctrl.ID = "VSC";
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
			global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVSC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControlTree(DialogTemplateContentNew @__ctrl) {
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"

<meta name=""viewport"" content=""width=device-width, user-scalable=no, maximum-scale=1.0, minimum-scale=1.0"">
<style type=""text/css"">
    /*====================================================================================================================*/
</style>
"));
			global::DevExpress.Web.ASPxHiddenField @__ctrl1;
			@__ctrl1 = this.@__BuildControlClientParams();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl2;
			@__ctrl2 = this.@__BuildControlUPPopupWindowControl();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
<div class=""dialogContent"">
    <div>
        <table id=""headerTable"" class=""dialog headerTable gray borderBottom width100"" style=""position: initial; padding-left: 0px; padding-right: 0px"">
            <tbody>
                <tr>
                    <td>
                        <table class=""viewCaption"">
                            <tbody>
                                <tr>
                                    <td>
                                        "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl3;
			@__ctrl3 = this.@__BuildControlUPVIC();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                    </td>\r\n                                    " +
						"<td>\r\n                                        "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl4;
			@__ctrl4 = this.@__BuildControlUPVH();
			@__parser.AddParsedSubObject(@__ctrl4);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                    <td style=""width: 50%"">
                        <div style=""float: right"">
                            "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl5;
			@__ctrl5 = this.@__BuildControlUPSAC();
			@__parser.AddParsedSubObject(@__ctrl5);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        </div>\r\n                    </td>\r\n                </tr" +
						">\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n    <div id=\"viewSite\" cl" +
						"ass=\"white viewSite\">\r\n        "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl6;
			@__ctrl6 = this.@__BuildControlUPEI();
			@__parser.AddParsedSubObject(@__ctrl6);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl7;
			@__ctrl7 = this.@__BuildControlUPVSC();
			@__parser.AddParsedSubObject(@__ctrl7);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
    </div>
</div>

<script type=""text/javascript"">
    window.top.pageLoaded = false;
    window.NewStyle = true;
    document.body.className = ""dialog"";

    var maxInitHeaderCallCount = 5;
    function InitHeader() {
        window.setTimeout(function () {
            var headerTable = document.getElementById(""headerTable"");
            SetItemsHeight(headerTable, headerTable.scrollHeight);
        }, 0);
    }

    function SetItemsHeight(headerTable, headerTableScrollHeight) {
        if (headerTableScrollHeight > 0 || maxInitHeaderCallCount == 0) {
            SetItemsHeightCore(headerTable, headerTableScrollHeight);
        } else {
            if (maxInitHeaderCallCount > 0) {
                maxInitHeaderCallCount--;
                InitHeader();
            }
        }
    }
    function SetItemsHeightCore(headerTable, headerTableScrollHeight) {
        var viewSite = document.getElementById(""viewSite"");

        headerTable.style.height = headerTableScrollHeight + ""px"";
        viewSite.style.marginTop = headerTableScrollHeight + ""px"";
        headerTable.style.position = 'fixed';
    }

    $(window).load(function () {
        window.top.pageLoaded = true;
        InitHeader();
        PageLoaded();
    });
</script>
"));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
