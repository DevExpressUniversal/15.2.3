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

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using DevExpress.Web;
using System.ComponentModel;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.Design;
using System.Drawing.Design;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web.Internal;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	[ToolboxItem(false)]
	public class TreeSingleChoiceActionControl : Table, INamingContainer, ITestable {
		bool isPrerendered = false;
		private const string XafControlsFolder = "Xaf";
		private string imageFolder = "~/App_Themes/XafDefault/{0}/";
		private string cssPostfix;
		private string cssFilePath;
		private ASPxButton mainButton;
		private ASPxButton dropDownButton;
		private ASPxPopupMenu menu;
		private string GetButtonCssClassName(bool isFocused, ASPxButton button) {
			string cssPostfix = ((ISkinOwner)button).GetCssPostFix();
			return "dxbButton" + (isFocused ? "Hover" : "") + (string.IsNullOrEmpty(cssPostfix) ? "" : "_" + cssPostfix);
		}
		private string GetFocusChangedBody(bool isFocused, ASPxButton button) {
			string cssClassName = GetButtonCssClassName(false, button);
			if(isFocused) {
				cssClassName += " " + GetButtonCssClassName(true, button);
			}
			return "function(s, e) { s.GetMainElement().rows[0].cells[0].className = '" + cssClassName + "'; }";
		}
		protected override void OnPreRender(EventArgs e) {
			isPrerendered = true;
			MainButton.ClientSideEvents.GotFocus = GetFocusChangedBody(true, MainButton);
			MainButton.ClientSideEvents.LostFocus = GetFocusChangedBody(false, MainButton);
			DropDownButton.ClientSideEvents.GotFocus = GetFocusChangedBody(true, DropDownButton);
			DropDownButton.ClientSideEvents.LostFocus = GetFocusChangedBody(false, DropDownButton);
			base.OnPreRender(e);
		}
		protected override void Render(HtmlTextWriter writer) {
			if(!isPrerendered) {
				OnPreRender(EventArgs.Empty);
			}
			Menu.ClientSideEvents.Init = "function(s, e) { " + JSUpdateScriptHelper.GetMenuUpdateScript(Menu, MainButton.ClientEnabled) + " }";
			base.Render(writer);
			RenderUtils.WriteScriptHtml(writer, @"window." + ClientID + @" =  new TreeSingleChoiceActionClientControl('" + ClientID + "');");
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			Menu.PopupElementID = dropDownButton.ID;
			Menu.PopupAction = PopupAction.LeftMouseClick;
			Menu.PopupHorizontalAlign = PopupHorizontalAlign.LeftSides;
			Menu.PopupVerticalAlign = PopupVerticalAlign.Below;
		}
		protected override void OnUnload(EventArgs e) {
			OnControlInitialized(this);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		public void SetConfirmationMessage(string message) {
			ConfirmationsHelper.SetConfirmationScript(MainButton, message);
			ConfirmationsHelper.SetConfirmationScript(Menu, message);
		}
		public override void Dispose() {
			base.Dispose();
		}
		public TreeSingleChoiceActionControl() {
			CellPadding = 0;
			CellSpacing = 0;
			CssClass = "TreeAction";
			mainButton = RenderHelper.CreateASPxButton();
			mainButton.ID = "MB";
			dropDownButton = RenderHelper.CreateASPxButton();
			dropDownButton.ID = "DDB";
			dropDownButton.Image.Url = string.Format(ImageFolder, XafControlsFolder) + "dropDownArrow.gif";
			dropDownButton.Image.Width = Unit.Pixel(5);
			dropDownButton.Image.Height = Unit.Pixel(4);
			dropDownButton.Image.UrlDisabled = string.Format(ImageFolder, XafControlsFolder) + "dropDownArrowDisabled.gif";
			menu = RenderHelper.CreateASPxPopupMenu();
			menu.ID = "M";
			menu.ClientSideEvents.ItemClick = @"function(s, e){ document.isMenuClicked = true; if(e.item.GetItemCount() != 0) { e.processOnServer = false;} }";
			Rows.Add(new TableRow());
			Rows[0].Cells.Add(new TableCell());
			Rows[0].Cells.Add(new TableCell());
			Rows[0].Cells[0].Controls.Add(mainButton);
			Rows[0].Cells[1].Controls.Add(dropDownButton);
			Rows[0].Cells[1].Controls.Add(menu);
		}
		public string ImageFolder {
			get { return imageFolder; }
			set { imageFolder = value; }
		}
		public string CssPostfix {
			get {
				return cssPostfix;
			}
			set {
				cssPostfix = value;
				mainButton.CssPostfix = value;
				dropDownButton.CssPostfix = value;
			}
		}
		public string CssFilePath {
			get {
				return cssFilePath;
			}
			set {
				cssFilePath = value;
				mainButton.CssFilePath = value;
				dropDownButton.CssFilePath = value;
			}
		}
		public ASPxButton MainButton {
			get { return mainButton; }
			set { mainButton = value; }
		}
		public ASPxButton DropDownButton {
			get { return dropDownButton; }
			set { dropDownButton = value; }
		}
		public ASPxPopupMenu Menu {
			get { return menu; }
			set { menu = value; }
		}
		public override string SkinID {
			get {
				return base.SkinID;
			}
			set {
				base.SkinID = value;
				mainButton.SkinID = value;
				dropDownButton.SkinID = value;
			}
		}
		public override bool Enabled {
			get { return base.Enabled; }
			set {
				base.Enabled = true;
				mainButton.ClientEnabled = value;
				dropDownButton.ClientEnabled = value;
			}
		}
		#region ITestable Members
		string ITestable.TestCaption {
			get { return MainButton.Text; }
		}
		string ITestable.ClientId {
			get { return ClientID; }
		}
		IJScriptTestControl ITestable.TestControl {
			get { return new JSASPxTreeSingleChoiceActionControl(); }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		public virtual TestControlType TestControlType {
			get {
				return TestControlType.Action;
			}
		}
		#endregion
	}
	public class JSASPxTreeSingleChoiceActionControl : IJScriptTestControl {
		public const string ClassName = "ASPxTreeSingleChoiceActionControl";
		public JSASPxTreeSingleChoiceActionControl() { }
		#region IJScriptTestControl Members
		public string JScriptClassName {
			get { return ClassName; }
		}
		public TestScriptsDeclarationBase ScriptsDeclaration {
			get {
				ASPxStandardTestControlScriptsDeclaration result = new ASPxStandardTestControlScriptsDeclaration();
				result.ActFunctionBody = @"
                    if(this.control) {
                        this.control.Act(value);
					}
					else {
						this.LogOperationError('The item ' + value + ' is not found.');						
					}
				";
				result.IsEnabledFunctionBody = @"
					return this.control.GetEnabled();
                    ";
				result.AdditionalFunctions = new JSFunctionDeclaration[] { 
					new JSFunctionDeclaration("GetHint", null, @"
                    if(this.control) {
                        return this.control.GetMainButton().mainElement.title;
					}
					return '';
					")
				};
				return result;
			}
		}
		#endregion
	}
}
