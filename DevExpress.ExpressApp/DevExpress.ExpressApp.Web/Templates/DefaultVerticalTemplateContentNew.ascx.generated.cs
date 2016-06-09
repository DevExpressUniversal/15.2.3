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
	public partial class DefaultVerticalTemplateContentNew : DevExpress.ExpressApp.Web.Templates.TemplateContent {
		protected global::DevExpress.Web.ASPxHiddenField ClientParams;
		protected global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl PopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPPopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder SAC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPSAC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationActionContainer NC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPNC;
		protected global::System.Web.UI.WebControls.Panel navigation;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewImageControl VIC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVIC;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl VCC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVH;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder mainMenu;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel XafUpdatePanel1;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder SearchAC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel XafUpdatePanel2;
		protected global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl ErrorInfo;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPEI;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl VSC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVSC;
		protected global::DevExpress.ExpressApp.Web.Controls.AboutInfoControl AIC;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public DefaultVerticalTemplateContentNew() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/DefaultVerticalTemplateContentNew.ascx";
			if ((global::DevExpress.ExpressApp.Web.Templates.DefaultVerticalTemplateContentNew.@__initialized == false)) {
				global::DevExpress.ExpressApp.Web.Templates.DefaultVerticalTemplateContentNew.@__initialized = true;
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
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control3() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.IsDropDown = true;
			@__ctrl.DropDownMenuItemCssClass = "accountItem";
			@__ctrl.ContainerId = "Security";
			@__ctrl.DefaultItemCaption = "My Account";
			@__ctrl.DefaultItemImageName = "BO_Person";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control2(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control3();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlSAC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.SAC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "SAC";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
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
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationActionContainer @__BuildControlNC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationActionContainer();
			this.NC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "NC";
			@__ctrl.ContainerId = "ViewsNavigation";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.BackColor = global::System.Drawing.Color.White;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPNC() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPNC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPNC";
			@__ctrl.CssClass = "xafContent";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControlNC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.Panel @__BuildControlnavigation() {
			global::System.Web.UI.WebControls.Panel @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.Panel();
			this.navigation = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "navigation";
			@__ctrl.CssClass = "xafNav xafNavHidden";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlUPNC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n    "));
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                "));
			global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVIC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            "));
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                "));
			global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVCC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control5(DevExpress.Web.Border @__ctrl) {
			@__ctrl.BorderStyle = global::System.Web.UI.WebControls.BorderStyle.None;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control6(DevExpress.Web.Border @__ctrl) {
			@__ctrl.BorderStyle = global::System.Web.UI.WebControls.BorderStyle.None;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control4(DevExpress.Web.ASPxMenu @__ctrl) {
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ItemAutoWidth = false;
			@__ctrl.ClientInstanceName = "mainMenu";
			@__ctrl.Font.Size = new System.Web.UI.WebControls.FontUnit(new System.Web.UI.WebControls.Unit(14D, global::System.Web.UI.WebControls.UnitType.Pixel));
			@__ctrl.EnableAdaptivity = true;
			@__ctrl.ItemWrap = false;
			this.@__BuildControl__control5(@__ctrl.BorderLeft);
			this.@__BuildControl__control6(@__ctrl.BorderRight);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control8() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "ObjectsCreation";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control9() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Save";
			@__ctrl.DefaultActionID = "Save";
			@__ctrl.IsDropDown = true;
			@__ctrl.AutoChangeDefaultAction = true;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control10() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Edit";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control11() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "RecordEdit";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control12() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "View";
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
			@__ctrl.ContainerId = "Reports";
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
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control16() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "RecordsNavigation";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control7(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control8();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control9();
			@__ctrl.Add(@__ctrl2);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl3;
			@__ctrl3 = this.@__BuildControl__control10();
			@__ctrl.Add(@__ctrl3);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl4;
			@__ctrl4 = this.@__BuildControl__control11();
			@__ctrl.Add(@__ctrl4);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl5;
			@__ctrl5 = this.@__BuildControl__control12();
			@__ctrl.Add(@__ctrl5);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl6;
			@__ctrl6 = this.@__BuildControl__control13();
			@__ctrl.Add(@__ctrl6);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl7;
			@__ctrl7 = this.@__BuildControl__control14();
			@__ctrl.Add(@__ctrl7);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl8;
			@__ctrl8 = this.@__BuildControl__control15();
			@__ctrl.Add(@__ctrl8);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl9;
			@__ctrl9 = this.@__BuildControl__control16();
			@__ctrl.Add(@__ctrl9);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlmainMenu() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.mainMenu = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "mainMenu";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Buttons;
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			this.@__BuildControl__control4(@__ctrl.Menu);
			this.@__BuildControl__control7(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlXafUpdatePanel1() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.XafUpdatePanel1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "XafUpdatePanel1";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlmainMenu();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control18() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Search";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control19() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "FullTextSearch";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control17(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control18();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control19();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlSearchAC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.SearchAC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "SearchAC";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Buttons;
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			this.@__BuildControl__control17(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlXafUpdatePanel2() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.XafUpdatePanel2 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "XafUpdatePanel2";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlSearchAC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            "));
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                "));
			global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlErrorInfo();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                "));
			global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVSC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.AboutInfoControl @__BuildControlAIC() {
			global::DevExpress.ExpressApp.Web.Controls.AboutInfoControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.AboutInfoControl();
			this.AIC = @__ctrl;
			@__ctrl.ID = "AIC";
			@__ctrl.Text = "Copyright text";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControlTree(DefaultVerticalTemplateContentNew @__ctrl) {
			global::DevExpress.Web.ASPxHiddenField @__ctrl1;
			@__ctrl1 = this.@__BuildControlClientParams();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl2;
			@__ctrl2 = this.@__BuildControlUPPopupWindowControl();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl3;
			@__ctrl3 = this.@__BuildControlUPSAC();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::System.Web.UI.WebControls.Panel @__ctrl4;
			@__ctrl4 = this.@__BuildControlnavigation();
			@__parser.AddParsedSubObject(@__ctrl4);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl5;
			@__ctrl5 = this.@__BuildControlUPVIC();
			@__parser.AddParsedSubObject(@__ctrl5);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl6;
			@__ctrl6 = this.@__BuildControlUPVH();
			@__parser.AddParsedSubObject(@__ctrl6);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl7;
			@__ctrl7 = this.@__BuildControlXafUpdatePanel1();
			@__parser.AddParsedSubObject(@__ctrl7);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl8;
			@__ctrl8 = this.@__BuildControlXafUpdatePanel2();
			@__parser.AddParsedSubObject(@__ctrl8);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl9;
			@__ctrl9 = this.@__BuildControlUPEI();
			@__parser.AddParsedSubObject(@__ctrl9);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl10;
			@__ctrl10 = this.@__BuildControlUPVSC();
			@__parser.AddParsedSubObject(@__ctrl10);
			global::DevExpress.ExpressApp.Web.Controls.AboutInfoControl @__ctrl11;
			@__ctrl11 = this.@__BuildControlAIC();
			@__parser.AddParsedSubObject(@__ctrl11);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
		}
		private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			@__w.Write("\r\n<meta name=\"viewport\" content=\"width=device-width, user-scalable=no, maximum-sc" +
					"ale=1.0, minimum-scale=1.0\">\r\n\r\n");
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write(@"
<div id=""headerDivWithShadow"" style=""z-index: 2000"">
</div>
<div id=""TestheaderTableDiv"" style=""background-color: white; position: absolute; display: none; right: 0px; z-index: 100000"">
</div>
<div class=""white borderBottom width100"" id=""headerTableDiv"">
    <div class=""paddings ");
				 @__w.Write( AdditionalClass );
			@__w.Write("\" style=\"margin: auto\">\r\n        <table id=\"headerTable\" class=\"headerTable xafAl" +
					"ignCenter white width100 ");
																		 @__w.Write( AdditionalClass );
			@__w.Write(@""">
            <tbody>
                <tr>
                    <td class=""xafNavToggleConteiner"">
                        <div id=""toggleNavigation"" class=""xafNavToggle"">
                            <div id=""xafNavTogleActive"" class=""xafNavHidden ToggleNavigationImage"">
                            </div>
                            <div id=""xafNavTogle"" class=""xafNavVisible ToggleNavigationActiveImage"">
                            </div>
                        </div>
                    </td>
                    <td>
                        <div style=""height: 33px; margin-left: 5px; margin-right: 20px; border-right: 1px solid #c6c6c6"">
                        </div>
                    </td>
                    <td>
                        <img src=""Images/Logo.png"" />
                    </td>
                    <td class=""width100""></td>
                    <td>
                        <div id=""xafHeaderMenu"" class=""xafHeaderMenu"" style=""float: right;"">
                            ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("\r\n                        </div>\r\n                    </td>\r\n                </tr" +
					">\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n</div>\r\n<div id=\"mainDiv\"" +
					" class=\"xafAlignCenter paddings overflowHidden ");
														@__w.Write( AdditionalClass );
			@__w.Write("\">\r\n    ");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write(@"
    <div id=""content"" class=""overflowHidden"">
        <div id=""menuAreaDiv"" style=""z-index: 2500"">
            <table id=""menuInnerTable"" class=""width100"" style=""padding-bottom: 13px; padding-top: 13px;"">
                <tbody>
                    <tr>
                        <td class=""xafNavToggleConteiner"">
                            <div id=""toggleNavigation_m"" class=""xafNavToggle xafHidden"">
                                <div id=""xafNavTogleActive_m"" class=""xafNavHidden ToggleNavigationImage"">
                                </div>
                                <div id=""xafNavTogle_m"" class=""xafNavVisible ToggleNavigationActiveImage"">
                                </div>
                            </div>
                        </td>
                        <td>
                            <div id=""toggleSeparator_m"" class=""xafHidden"" style=""height: 33px; margin-left: 5px; margin-right: 20px; border-right: 1px solid #c6c6c6"">
                            </div>
                        </td>
                        <td style=""width: 30%"">
                            <table>
                                <tbody>
                                    <tr>
                                        <td>
                                            ");
			parameterContainer.Controls[4].RenderControl(@__w);
			@__w.Write("\r\n                                        </td>\r\n                                " +
					"        <td>\r\n                                            ");
			parameterContainer.Controls[5].RenderControl(@__w);
			@__w.Write(@"
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                        <td id=""menuCell"" style=""width: 70%;"">
                            <table id=""menuContainer"" style=""float: right;"">
                                <tbody>
                                    <tr>
                                        <td>
                                            ");
			parameterContainer.Controls[6].RenderControl(@__w);
			@__w.Write("\r\n                                        </td>\r\n                                " +
					"        <td>\r\n                                            ");
			parameterContainer.Controls[7].RenderControl(@__w);
			@__w.Write(@"
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
        <div id=""viewSite"" class=""width100 viewSite"" style=""float: left"">
            ");
			parameterContainer.Controls[8].RenderControl(@__w);
			@__w.Write("\r\n            ");
			parameterContainer.Controls[9].RenderControl(@__w);
			@__w.Write("\r\n        </div>\r\n    </div>\r\n</div>\r\n\r\n<div id=\"footer\" class=\"xafFooter width10" +
					"0\">\r\n    <div class=\"xafAlignCenter paddings ");
								@__w.Write( AdditionalClass );
			@__w.Write("\">\r\n        ");
			parameterContainer.Controls[10].RenderControl(@__w);
			@__w.Write("\r\n    </div>\r\n</div>\r\n");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
