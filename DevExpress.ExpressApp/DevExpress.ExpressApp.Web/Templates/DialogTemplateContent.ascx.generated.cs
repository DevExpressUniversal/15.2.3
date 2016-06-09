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
	public partial class DialogTemplateContent : DevExpress.ExpressApp.Web.Templates.TemplateContent {
		protected global::DevExpress.Web.ASPxHiddenField ClientParams;
		protected global::DevExpress.ExpressApp.Web.Controls.XafPopupWindowControl PopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPPopupWindowControl;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewImageControl VIC;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl VCC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVH;
		protected global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl ErrorInfo;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPEI;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder OCC;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder SAC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPSAC;
		protected global::System.Web.UI.WebControls.TableCell TableCell1;
		protected global::System.Web.UI.WebControls.TableRow TableRow5;
		protected global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl VSC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPVSC;
		protected global::System.Web.UI.WebControls.TableCell ViewSite;
		protected global::System.Web.UI.WebControls.TableRow TableRow2;
		protected global::System.Web.UI.WebControls.Table Table1;
		protected global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder PAC;
		protected global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel UPPAC;
		private static bool @__initialized;
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public DialogTemplateContent() {
			((global::System.Web.UI.UserControl)(this)).AppRelativeVirtualPath = "~/DialogTemplateContent.ascx";
			if ((global::DevExpress.ExpressApp.Web.Templates.DialogTemplateContent.@__initialized == false)) {
				global::DevExpress.ExpressApp.Web.Templates.DialogTemplateContent.@__initialized = true;
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
		private global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__BuildControlVIC() {
			global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ViewImageControl();
			this.VIC = @__ctrl;
			@__ctrl.ID = "VIC";
			@__ctrl.Control.UseLargeImage = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__BuildControlVCC() {
			global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl();
			this.VCC = @__ctrl;
			@__ctrl.ID = "VCC";
			@__ctrl.DetailViewCaptionMode = global::DevExpress.ExpressApp.Web.Controls.DetailViewCaptionMode.ViewAndObjectCaption;
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                    <div style=\"display: none\">\r\n                        "));
			global::DevExpress.ExpressApp.Web.Controls.ViewImageControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVIC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        "));
			global::DevExpress.ExpressApp.Web.Controls.ViewCaptionControl @__ctrl2;
			@__ctrl2 = this.@__BuildControlVCC();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                    </div>\r\n                "));
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                "));
			global::DevExpress.ExpressApp.Web.Templates.Controls.ErrorInfoControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlErrorInfo();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control5() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "ObjectsCreation";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control4(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control5();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlOCC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.OCC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "OCC";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Buttons;
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "float: left");
			this.@__BuildControl__control4(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control7() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Search";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control8() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "FullTextSearch";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control6(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control7();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control8();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlSAC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.SAC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "SAC";
			@__ctrl.CssClass = "HContainer";
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Buttons;
			this.@__BuildControl__control6(@__ctrl.ActionContainers);
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
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
                                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" >
                                                <tr>
                                                    <td align=""left"" valign=""top"">
                                                        "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlOCC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                    </td>\r\n                    " +
						"                                <td align=\"right\" valign=\"top\">\r\n               " +
						"                                         "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl2;
			@__ctrl2 = this.@__BuildControlSAC();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                                    </td>\r\n                    " +
						"                            </tr>\r\n                                            <" +
						"/table>\r\n                                        "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.TableCell @__BuildControlTableCell1() {
			global::System.Web.UI.WebControls.TableCell @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.TableCell();
			this.TableCell1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "TableCell1";
			@__ctrl.HorizontalAlign = global::System.Web.UI.WebControls.HorizontalAlign.Center;
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl1;
			@__ctrl1 = this.@__BuildControlUPSAC();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control3(System.Web.UI.WebControls.TableCellCollection @__ctrl) {
			global::System.Web.UI.WebControls.TableCell @__ctrl1;
			@__ctrl1 = this.@__BuildControlTableCell1();
			@__ctrl.Add(@__ctrl1);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.TableRow @__BuildControlTableRow5() {
			global::System.Web.UI.WebControls.TableRow @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.TableRow();
			this.TableRow5 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "TableRow5";
			this.@__BuildControl__control3(@__ctrl.Cells);
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
			@__ctrl.UpdatePanelForASPxGridListCallback = false;
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                            "));
			global::DevExpress.ExpressApp.Web.Controls.ViewSiteControl @__ctrl1;
			@__ctrl1 = this.@__BuildControlVSC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                        "));
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
		private void @__BuildControl__control9(System.Web.UI.WebControls.TableCellCollection @__ctrl) {
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
			this.@__BuildControl__control9(@__ctrl.Cells);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control2(System.Web.UI.WebControls.TableRowCollection @__ctrl) {
			global::System.Web.UI.WebControls.TableRow @__ctrl1;
			@__ctrl1 = this.@__BuildControlTableRow5();
			@__ctrl.Add(@__ctrl1);
			global::System.Web.UI.WebControls.TableRow @__ctrl2;
			@__ctrl2 = this.@__BuildControlTableRow2();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::System.Web.UI.WebControls.Table @__BuildControlTable1() {
			global::System.Web.UI.WebControls.Table @__ctrl;
			@__ctrl = new global::System.Web.UI.WebControls.Table();
			this.Table1 = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "Table1";
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.BorderWidth = new System.Web.UI.WebControls.Unit(0D, global::System.Web.UI.WebControls.UnitType.Pixel);
			@__ctrl.CellPadding = 0;
			@__ctrl.CellSpacing = 0;
			this.@__BuildControl__control2(@__ctrl.Rows);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control10(DevExpress.Web.ASPxMenu @__ctrl) {
			@__ctrl.Width = new System.Web.UI.WebControls.Unit(100D, global::System.Web.UI.WebControls.UnitType.Percentage);
			@__ctrl.ItemAutoWidth = false;
			@__ctrl.HorizontalAlign = global::System.Web.UI.WebControls.HorizontalAlign.Right;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control12() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "PopupActions";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__BuildControl__control13() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer();
			@__ctrl.ContainerId = "Diagnostic";
			@__ctrl.IsDropDown = false;
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControl__control11(DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainersCollection @__ctrl) {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl1;
			@__ctrl1 = this.@__BuildControl__control12();
			@__ctrl.Add(@__ctrl1);
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.WebActionContainer @__ctrl2;
			@__ctrl2 = this.@__BuildControl__control13();
			@__ctrl.Add(@__ctrl2);
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__BuildControlPAC() {
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder();
			this.PAC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "PAC";
			@__ctrl.ContainerStyle = global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerStyle.Buttons;
			@__ctrl.Orientation = global::DevExpress.ExpressApp.Model.ActionContainerOrientation.Horizontal;
			this.@__BuildControl__control10(@__ctrl.Menu);
			this.@__BuildControl__control11(@__ctrl.ActionContainers);
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__BuildControlUPPAC() {
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl;
			@__ctrl = new global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel();
			this.UPPAC = @__ctrl;
			@__ctrl.ApplyStyleSheetSkin(this.Page);
			@__ctrl.ID = "UPPAC";
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                                "));
			global::DevExpress.ExpressApp.Web.Templates.ActionContainers.ActionContainerHolder @__ctrl1;
			@__ctrl1 = this.@__BuildControlPAC();
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                            "));
			return @__ctrl;
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		private void @__BuildControlTree(DialogTemplateContent @__ctrl) {
			global::DevExpress.Web.ASPxHiddenField @__ctrl1;
			@__ctrl1 = this.@__BuildControlClientParams();
			System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
			@__parser.AddParsedSubObject(@__ctrl1);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n<div class=\"Dialog\" id=\"DialogContent\">\r\n     "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl2;
			@__ctrl2 = this.@__BuildControlUPPopupWindowControl();
			@__parser.AddParsedSubObject(@__ctrl2);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n    <table Width=\"100%\" height=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0" +
						"\">\r\n        <tr>\r\n            <td valign=\"top\" height=\"100%\">\r\n                "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl3;
			@__ctrl3 = this.@__BuildControlUPVH();
			@__parser.AddParsedSubObject(@__ctrl3);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                <table class=\"DialogContent Content\" border=\"0\" cellpadding=\"0\"" +
						" cellspacing=\"0\" width=\"100%\">\r\n                    <tr>\r\n                      " +
						"  <td class=\"ContentCell\">\r\n                            "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl4;
			@__ctrl4 = this.@__BuildControlUPEI();
			@__parser.AddParsedSubObject(@__ctrl4);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                            "));
			global::System.Web.UI.WebControls.Table @__ctrl5;
			@__ctrl5 = this.@__BuildControlTable1();
			@__parser.AddParsedSubObject(@__ctrl5);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl(@"
                        </td>
                    </tr>
                </table>
                <table class=""DockBottom"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                    <tr>
                        <td>
                            "));
			global::DevExpress.ExpressApp.Web.Templates.XafUpdatePanel @__ctrl6;
			@__ctrl6 = this.@__BuildControlUPPAC();
			@__parser.AddParsedSubObject(@__ctrl6);
			@__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n                        </td>\r\n                    </tr>\r\n                </tab" +
						"le>\r\n            </td>\r\n        </tr>\r\n    </table>\r\n</div>"));
		}
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		protected override void FrameworkInitialize() {
			base.FrameworkInitialize();
			this.@__BuildControlTree(this);
		}
	}
}
