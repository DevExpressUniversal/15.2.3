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
	public partial class DefaultVerticalTemplateContent : DevExpress.ExpressApp.Web.Templates.TemplateContent {
		protected global::DevExpress.Web.ASPxHiddenField ClientParams;
		protected global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl PopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPPopupWindowControl;
		protected global::DevExpress.Web.ASPxGlobalEvents GE;
		protected global::DevExpress.ExpressApp.Web.Controls.ThemedImageControl TIC;
		protected global::System.Web.UI.WebControls.HyperLink LogoLink;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder SAC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPSAC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder SHC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPSHC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationActionContainer NC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPNC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder VTC;
		protected global::DevExpress.Web.PanelContent PanelContent1;
		protected global::DevExpress.Web.ASPxRoundPanel TRP;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder DAC;
		protected global::System.Web.UI.HtmlControls.HtmlGenericControl TP;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPTP;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder TB;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPTB;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewImageControl VIC;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl VCC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationHistoryActionContainer VHC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder RNC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVH;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder EMA;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPEMA;
		protected global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl ErrorInfo;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPEI;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl VSC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder EditModeActions2;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVSC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.QuickAccessNavigationActionContainer QC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPQC;
		protected global::System.Web.UI.WebControls.Literal InfoMessagesPanel;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPIMP;
		protected global::DevExpress.ExpressApp.Web.Controls.AboutInfoControl AIC;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public DefaultVerticalTemplateContent() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/DefaultVerticalTemplateContent.ascx";
			if ((global::DevExpress.ExpressApp.Web.Templates.DefaultVerticalTemplateContent.@__initialized == false)) {
				global::DevExpress.ExpressApp.Web.Templates.DefaultVerticalTemplateContent.@__initialized = true;
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n        "));
			global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlPopupWindowControl();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n    "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.Web.ASPxGlobalEvents @__BuildControlGE() {
			global::DevExpress.Web.ASPxGlobalEvents @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxGlobalEvents();
			this.GE = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "GE";
			@__ctrl.ClientSideEvents.EndCallback = "AdjustSize";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.ThemedImageControl @__BuildControlTIC() {
			global::DevExpress.ExpressApp.Web.Controls.ThemedImageControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ThemedImageControl();
			this.TIC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "TIC";
			@__ctrl.DefaultThemeImageLocation = "App_Themes/{0}/Xaf";
			@__ctrl.ImageName = "Logo.png";
			@__ctrl.BorderWidth = new System.Web.UI.WebControls.Unit(0D, global::System.Web.UI.WebControls.UnitType.Pixel);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.HyperLink @__BuildControlLogoLink() {
			global::System.Web.UI.WebControls.HyperLink @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.HyperLink();
			this.LogoLink = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.NavigateUrl = "#";
			@__ctrl.ID = "LogoLink";
			global::DevExpress.ExpressApp.Web.Controls.ThemedImageControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlTIC();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control3() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Notifications";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control4() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Security";
			@__ctrl.IsDropDown = false;
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
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlSAC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.SAC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "SAC";
			@__ctrl.CssClass = "Security";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
			@__ctrl.ShowSeparators = true;
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
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                        "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlSAC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                    "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control6() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "RootObjectsCreation";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control7() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Appearance";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control8() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Search";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control9() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "FullTextSearch";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control5(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control6();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control7();
			@__ctrl.Add(@__ctrl2);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl3;
			@__ctrl3 = this.@__BuildControl__control8();
			@__ctrl.Add(@__ctrl3);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl4;
			@__ctrl4 = this.@__BuildControl__control9();
			@__ctrl.Add(@__ctrl4);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlSHC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.SHC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "SHC";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
			@__ctrl.CssClass = "TabsContainer";
			this.@__BuildControl__control5(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPSHC() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPSHC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPSHC";
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                        "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlSHC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                    "));
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
			@__ctrl.CssClass = "xafNavigationBarActionContainer";
			@__ctrl.ContainerId = "ViewsNavigation";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPNC() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPNC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPNC";
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                        "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControlNC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                    "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control12() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Tools";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control11(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control12();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlVTC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.VTC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "VTC";
			@__ctrl.Menu.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Vertical;
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
			@__ctrl.ShowSeparators = false;
			this.@__BuildControl__control11(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.Web.PanelContent @__BuildControlPanelContent1() {
			global::DevExpress.Web.PanelContent @__ctrl;
			@__ctrl = new global::DevExpress.Web.PanelContent();
			this.PanelContent1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "PanelContent1";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                        "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlVTC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                    "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control10(DevExpress.Web.PanelCollection @__ctrl) {
			global::DevExpress.Web.PanelContent @__ctrl1;
			@__ctrl1 = this.@__BuildControlPanelContent1();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.Web.ASPxRoundPanel @__BuildControlTRP() {
			global::DevExpress.Web.ASPxRoundPanel @__ctrl;
			@__ctrl = new global::DevExpress.Web.ASPxRoundPanel();
			this.TRP = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ApplyStyleSheetThemeInternal();
			@__ctrl.ID = "TRP";
			@__ctrl.HeaderText = "Tools";
			@__ctrl.CssClass = "TRP";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(170D, global::System.Web.UI.WebControls.UnitType.Pixel);
			this.@__BuildControl__control10(@__ctrl.PanelCollection);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control14() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Diagnostic";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control13(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control14();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlDAC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.DAC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "DAC";
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Vertical;
			@__ctrl.BorderWidth = new System.Web.UI.WebControls.Unit(0D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
			@__ctrl.ShowSeparators = false;
			this.@__BuildControl__control13(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.HtmlControls.HtmlGenericControl @__BuildControlTP() {
			global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl;
			@__ctrl = new global::System.Web.UI.HtmlControls.HtmlGenericControl("div");
			this.TP = @__ctrl;
			@__ctrl.ID = "TP";
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("class", "ToolsActionContainerPanel");
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            "));
			global::DevExpress.Web.ASPxRoundPanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlTRP();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl2;
			@__ctrl2 = this.@__BuildControlDAC();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            <br />\r\n                           " +
						"             "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPTP() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPTP = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPTP";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                        "));
			global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlTP();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                    "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control16(DevExpress.Web.Border @__ctrl) {
			@__ctrl.BorderStyle = global::System.Web.UI.WebControls.BorderStyle.None;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control17(DevExpress.Web.Border @__ctrl) {
			@__ctrl.BorderStyle = global::System.Web.UI.WebControls.BorderStyle.None;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control18(DevExpress.Web.Border @__ctrl) {
			@__ctrl.BorderStyle = global::System.Web.UI.WebControls.BorderStyle.None;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control15(DevExpress.Web.ASPxMenu @__ctrl) {
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ItemAutoWidth = false;
			@__ctrl.ClientInstanceName = "mainMenu";
			this.@__BuildControl__control16(@__ctrl.BorderTop);
			this.@__BuildControl__control17(@__ctrl.BorderLeft);
			this.@__BuildControl__control18(@__ctrl.BorderRight);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control20() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "ObjectsCreation";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control21() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Edit";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control22() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "RecordEdit";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control23() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "View";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control24() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Export";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control25() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Reports";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control26() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Filters";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control19(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control20();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control21();
			@__ctrl.Add(@__ctrl2);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl3;
			@__ctrl3 = this.@__BuildControl__control22();
			@__ctrl.Add(@__ctrl3);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl4;
			@__ctrl4 = this.@__BuildControl__control23();
			@__ctrl.Add(@__ctrl4);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl5;
			@__ctrl5 = this.@__BuildControl__control24();
			@__ctrl.Add(@__ctrl5);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl6;
			@__ctrl6 = this.@__BuildControl__control25();
			@__ctrl.Add(@__ctrl6);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl7;
			@__ctrl7 = this.@__BuildControl__control26();
			@__ctrl.Add(@__ctrl7);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlTB() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.TB = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.CssClass = "ACH MainToolbar";
			@__ctrl.ID = "TB";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.ToolBar;
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			this.@__BuildControl__control15(@__ctrl.Menu);
			this.@__BuildControl__control19(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPTB() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPTB = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPTB";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                    "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlTB();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__BuildControlVIC() {
			global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ViewImageControl();
			this.VIC = @__ctrl;
			@__ctrl.ID = "VIC";
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
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationHistoryActionContainer @__BuildControlVHC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationHistoryActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationHistoryActionContainer();
			this.VHC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "VHC";
			@__ctrl.CssClass = "NavigationHistoryLinks";
			@__ctrl.ContainerId = "ViewsHistoryNavigation";
			@__ctrl.Delimiter = " / ";
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control27(DevExpress.Web.ASPxMenu @__ctrl) {
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ItemAutoWidth = false;
			@__ctrl.HorizontalAlign = global::System.Web.UI.WebControls.HorizontalAlign.Right;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control29() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "RecordsNavigation";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control28(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control29();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlRNC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.RNC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "RNC";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			@__ctrl.UseLargeImage = true;
			@__ctrl.CssClass = "RecordsNavigationContainer";
			this.@__BuildControl__control27(@__ctrl.Menu);
			this.@__BuildControl__control28(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPVH() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPVH = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPVH";
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
                                                    <table id=""VH"" border=""0"" cellpadding=""0"" cellspacing=""0"" class=""MainContent"" width=""100%"">
                                                        <tr>
                                                            <td class=""ViewHeader"">
                                                                <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" class=""ViewHeader"">
                                                                    <tr>
                                                                        <td class=""ViewImage"">
                                                                            "));
			global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVIC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
                                                                        </td>
                                                                        <td class=""ViewCaption"">
                                                                            <h1>
                                                                                "));
			global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__ctrl2;
			@__ctrl2 = this.@__BuildControlVCC();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                                            </h" +
						"1>\r\n                                                                            " +
						""));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.NavigationHistoryActionContainer @__ctrl3;
			@__ctrl3 = this.@__BuildControlVHC();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                                        </td>\r\n" +
						"                                                                        <td alig" +
						"n=\"right\">\r\n                                                                    " +
						"        "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl4;
			@__ctrl4 = this.@__BuildControlRNC();
			@__parser.AddParsedSubObject(@__ctrl4);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control30(DevExpress.Web.ASPxMenu @__ctrl) {
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ItemAutoWidth = false;
			@__ctrl.HorizontalAlign = global::System.Web.UI.WebControls.HorizontalAlign.Right;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control32() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Save";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control33() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "UndoRedo";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control31(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control32();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control33();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlEMA() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.EMA = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "EMA";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			@__ctrl.CssClass = "EditModeActions";
			this.@__BuildControl__control30(@__ctrl.Menu);
			this.@__BuildControl__control31(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPEMA() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPEMA = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPEMA";
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                    "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlEMA();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                "));
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                                    "));
			global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlErrorInfo();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                                "));
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
		private void @__BuildControl__control34(DevExpress.Web.ASPxMenu @__ctrl) {
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ItemAutoWidth = false;
			@__ctrl.HorizontalAlign = global::System.Web.UI.WebControls.HorizontalAlign.Right;
			@__ctrl.Paddings.PaddingTop = new System.Web.UI.WebControls.Unit(15D, global::System.Web.UI.WebControls.UnitType.Pixel);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control36() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Save";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control37() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "UndoRedo";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control35(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control36();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control37();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlEditModeActions2() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.EditModeActions2 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "EditModeActions2";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Links;
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			@__ctrl.CssClass = "EditModeActions";
			this.@__BuildControl__control34(@__ctrl.Menu);
			this.@__BuildControl__control35(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPVSC() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPVSC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPVSC";
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                                    "));
			global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVSC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                                    "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl2;
			@__ctrl2 = this.@__BuildControlEditModeActions2();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                                "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.QuickAccessNavigationActionContainer @__BuildControlQC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.QuickAccessNavigationActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.QuickAccessNavigationActionContainer();
			this.QC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.CssClass = "Links NavigationLinks";
			@__ctrl.ID = "QC";
			@__ctrl.ContainerId = "ViewsNavigation";
			@__ctrl.PaintStyle = global::DevExpress.ExpressApp.Templates.ActionItemPaintStyle.Caption;
			@__ctrl.ShowSeparators = true;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPQC() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPQC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPQC";
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                                    "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.QuickAccessNavigationActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControlQC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                                "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.Literal @__BuildControlInfoMessagesPanel() {
			global::System.Web.UI.WebControls.Literal @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.Literal();
			this.InfoMessagesPanel = @__ctrl;
			@__ctrl.ID = "InfoMessagesPanel";
			@__ctrl.Text = "";
			@__ctrl.Visible = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPIMP() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPIMP = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPIMP";
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			global::System.Web.UI.WebControls.Literal @__ctrl1;
			@__ctrl1 = this.@__BuildControlInfoMessagesPanel();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                    "));
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
		private void @__BuildControlTree(DefaultVerticalTemplateContent @__ctrl) {
			global::DevExpress.Web.ASPxHiddenField @__ctrl1;
			@__ctrl1 = this.@__BuildControlClientParams();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl2;
			@__ctrl2 = this.@__BuildControlUPPopupWindowControl();
			@__parser.AddParsedSubObject(@__ctrl2);
			global::DevExpress.Web.ASPxGlobalEvents @__ctrl3;
			@__ctrl3 = this.@__BuildControlGE();
			@__parser.AddParsedSubObject(@__ctrl3);
			global::System.Web.UI.WebControls.HyperLink @__ctrl4;
			@__ctrl4 = this.@__BuildControlLogoLink();
			@__parser.AddParsedSubObject(@__ctrl4);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl5;
			@__ctrl5 = this.@__BuildControlUPSAC();
			@__parser.AddParsedSubObject(@__ctrl5);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl6;
			@__ctrl6 = this.@__BuildControlUPSHC();
			@__parser.AddParsedSubObject(@__ctrl6);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl7;
			@__ctrl7 = this.@__BuildControlUPNC();
			@__parser.AddParsedSubObject(@__ctrl7);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl8;
			@__ctrl8 = this.@__BuildControlUPTP();
			@__parser.AddParsedSubObject(@__ctrl8);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl9;
			@__ctrl9 = this.@__BuildControlUPTB();
			@__parser.AddParsedSubObject(@__ctrl9);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl10;
			@__ctrl10 = this.@__BuildControlUPVH();
			@__parser.AddParsedSubObject(@__ctrl10);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl11;
			@__ctrl11 = this.@__BuildControlUPEMA();
			@__parser.AddParsedSubObject(@__ctrl11);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl12;
			@__ctrl12 = this.@__BuildControlUPEI();
			@__parser.AddParsedSubObject(@__ctrl12);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl13;
			@__ctrl13 = this.@__BuildControlUPVSC();
			@__parser.AddParsedSubObject(@__ctrl13);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl14;
			@__ctrl14 = this.@__BuildControlUPQC();
			@__parser.AddParsedSubObject(@__ctrl14);
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl15;
			@__ctrl15 = this.@__BuildControlUPIMP();
			@__parser.AddParsedSubObject(@__ctrl15);
			global::DevExpress.ExpressApp.Web.Controls.AboutInfoControl @__ctrl16;
			@__ctrl16 = this.@__BuildControlAIC();
			@__parser.AddParsedSubObject(@__ctrl16);
			@__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
		}
		private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
			parameterContainer.Controls[0].RenderControl(@__w);
			@__w.Write("\r\n<div class=\"VerticalTemplate BodyBackColor\">\r\n     ");
			parameterContainer.Controls[1].RenderControl(@__w);
			@__w.Write("\r\n    ");
			parameterContainer.Controls[2].RenderControl(@__w);
			@__w.Write("\r\n    <table id=\"MT\" border=\"0\" width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" clas" +
					"s=\"dxsplControl_");
																					   @__w.Write( BaseXafPage.CurrentTheme );
			@__w.Write("\">\r\n        <tbody>\r\n            <tr>\r\n                <td style=\"vertical-align:" +
					" top; height: 10px;\" class=\"dxsplPane_");
																		@__w.Write( BaseXafPage.CurrentTheme );
			@__w.Write(@""">
                    <div id=""VerticalTemplateHeader"" class=""VerticalTemplateHeader"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" class=""Top"" width=""100%"">
                            <tr>
                                <td class=""Logo"">
                                    ");
			parameterContainer.Controls[3].RenderControl(@__w);
			@__w.Write("\r\n                                </td>\r\n                                <td clas" +
					"s=\"Security\">\r\n                                    ");
			parameterContainer.Controls[4].RenderControl(@__w);
			@__w.Write(@"
                                </td>
                            </tr>
                        </table>
                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" class=""ACPanel"">
                            <tr class=""Content"">
                                <td class=""Content WithPaddings"" align=""right"">
                                    ");
			parameterContainer.Controls[5].RenderControl(@__w);
			@__w.Write(@"
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td style=""vertical-align: top"">
                    <table id=""MRC"" style=""width: 100%"" cellpadding=""0"" cellspacing=""0"">
                        <tr>
                            <td id=""LPcell"" style=""width: 200px; vertical-align: top"">
                                <div id=""LP"" class=""LeftPane"">
                                    ");
			parameterContainer.Controls[6].RenderControl(@__w);
			@__w.Write("\r\n                                    ");
			parameterContainer.Controls[7].RenderControl(@__w);
			@__w.Write("\r\n                                </div>\r\n                            </td>\r\n    " +
					"                        <td id=\"separatorCell\" style=\"width: 6px; border-bottom-" +
					"style: none; border-top-style: none\"\r\n                                class=\"dxs" +
					"plVSeparator_");
											   @__w.Write( BaseXafPage.CurrentTheme );
			@__w.Write(" dxsplPane_");
																						 @__w.Write( BaseXafPage.CurrentTheme );
			@__w.Write("\">\r\n                                <div id=\"separatorButton\" class=\"dxsplVSepara" +
					"torButton_");
																			   @__w.Write( BaseXafPage.CurrentTheme );
			@__w.Write(@""" onmouseover=""OnMouseEnter('separatorButton')""
                                    onmouseout=""OnMouseLeave('separatorButton')"" onclick=""OnClick('LPcell','separatorImage',true)"">
                                    <div id=""separatorImage"" style=""width: 6px;"" class=""dxWeb_splVCollapseBackwardButton_");
																												 @__w.Write( BaseXafPage.CurrentTheme );
			@__w.Write(@""">
                                    </div>
                                </div>
                            </td>
                            <td style=""vertical-align: top;"">
                                <table style=""width: 100%;"" cellpadding=""0"" cellspacing=""0"">
                                    <tbody>
                                        <tr>
                                            <td>
                                                ");
			parameterContainer.Controls[8].RenderControl(@__w);
			@__w.Write("\r\n                                                ");
			parameterContainer.Controls[9].RenderControl(@__w);
			@__w.Write("\r\n                                            </td>\r\n                            " +
					"            </tr>\r\n                                        <tr>\r\n               " +
					"                             <td>\r\n                                             " +
					"   ");
			parameterContainer.Controls[10].RenderControl(@__w);
			@__w.Write(@"
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <div id=""CP"" style=""overflow: auto; width: 100%;"">
                                                    <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""MainContent"" width=""100%"">
                                                        <tr class=""Content"">
                                                            <td class=""Content"">
                                                                ");
			parameterContainer.Controls[11].RenderControl(@__w);
			@__w.Write("\r\n                                                                ");
			parameterContainer.Controls[12].RenderControl(@__w);
			@__w.Write(@"
                                                                <div id=""Spacer"" class=""Spacer"">
                                                                </div>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                ");
			parameterContainer.Controls[13].RenderControl(@__w);
			@__w.Write(@"
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style=""height: 20px; vertical-align: bottom"" class=""BodyBackColor"">
                    ");
			parameterContainer.Controls[14].RenderControl(@__w);
			@__w.Write(@"
                    <div id=""Footer"" class=""Footer"">
                        <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"">
                            <tr>
                                <td align=""left"">
                                    <div class=""FooterCopyright"">
                                        ");
			parameterContainer.Controls[15].RenderControl(@__w);
			@__w.Write(@"
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </tbody>
    </table>

</div>
");
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
