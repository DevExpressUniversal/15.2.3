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
	public partial class NestedFrameControlNew : DevExpress.ExpressApp.Web.NestedFrameControlBase {
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder ObjectsCreation;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel XafUpdatePanel1;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder ToolBar;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPToolBar;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl viewSiteControl;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public NestedFrameControlNew() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/NestedFrameControlNew.ascx";
			if ((global::DevExpress.ExpressApp.Web.Templates.NestedFrameControlNew.@__initialized == false)) {
				global::DevExpress.ExpressApp.Web.Templates.NestedFrameControlNew.@__initialized = true;
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
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control3() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "ObjectsCreation";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control4() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Link";
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
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlObjectsCreation() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.ObjectsCreation = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "ObjectsCreation";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
			@__ctrl.CssClass = "nf_leftMenu_AC";
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			@__ctrl.Menu.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.Menu.ItemAutoWidth = false;
			this.@__BuildControl__control2(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlXafUpdatePanel1() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.XafUpdatePanel1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "XafUpdatePanel1";
			@__ctrl.CssClass = "ToolBarUpdatePanel";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlObjectsCreation();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control6(DevExpress.Web.Border @__ctrl) {
			@__ctrl.BorderStyle = global::System.Web.UI.WebControls.BorderStyle.None;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control7(DevExpress.Web.Border @__ctrl) {
			@__ctrl.BorderStyle = global::System.Web.UI.WebControls.BorderStyle.None;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control5(DevExpress.Web.ASPxMenu @__ctrl) {
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ItemAutoWidth = false;
			@__ctrl.ClientInstanceName = "nestedFrameMenu";
			@__ctrl.Font.Size = new System.Web.UI.WebControls.FontUnit(new System.Web.UI.WebControls.Unit(14D, global::System.Web.UI.WebControls.UnitType.Pixel));
			@__ctrl.EnableAdaptivity = true;
			@__ctrl.ItemWrap = false;
			this.@__BuildControl__control6(@__ctrl.BorderLeft);
			this.@__BuildControl__control7(@__ctrl.BorderRight);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control9() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Edit";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control10() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "RecordEdit";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control11() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "View";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control12() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Reports";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control13() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Export";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control14() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Diagnostic";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control15() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Filters";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control8(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control9();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control10();
			@__ctrl.Add(@__ctrl2);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl3;
			@__ctrl3 = this.@__BuildControl__control11();
			@__ctrl.Add(@__ctrl3);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl4;
			@__ctrl4 = this.@__BuildControl__control12();
			@__ctrl.Add(@__ctrl4);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl5;
			@__ctrl5 = this.@__BuildControl__control13();
			@__ctrl.Add(@__ctrl5);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl6;
			@__ctrl6 = this.@__BuildControl__control14();
			@__ctrl.Add(@__ctrl6);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl7;
			@__ctrl7 = this.@__BuildControl__control15();
			@__ctrl.Add(@__ctrl7);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlToolBar() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.ToolBar = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "ToolBar";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
			@__ctrl.CssClass = "nf_rightMenu_AC";
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			@__ctrl.Menu.ItemAutoWidth = false;
			this.@__BuildControl__control5(@__ctrl.Menu);
			this.@__BuildControl__control8(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPToolBar() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPToolBar = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPToolBar";
			@__ctrl.CssClass = "ToolBarUpdatePanel";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlToolBar();
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
			@__ctrl.Control.CssClass = "NestedFrameViewSite";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControlTree(NestedFrameControlNew @__ctrl) {
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n\r\n<meta name=\"viewport\" content=\"width=device-width, user-scalable=no, maximum-" +
						"scale=1.0, minimum-scale=1.0\">\r\n<style type=\"text/css\">\r\n</style>\r\n<div class=\"N" +
						"estedFrame\">\r\n    <div class=\"nf_Menu\">\r\n        <div class=\"nf_leftMenu\">\r\n    " +
						"        "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlXafUpdatePanel1();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        </div>\r\n        <div class=\"nf_rightMenu\">\r\n            "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl2;
			@__ctrl2 = this.@__BuildControlUPToolBar();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        </div>\r\n    </div>\r\n    <b class=\"dx-clear\"></b>\r\n    "));
			global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__ctrl3;
			@__ctrl3 = this.@__BuildControlviewSiteControl();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n\r\n</div>\r\n\r\n"));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
