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

using System.Collections.Generic;
using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.Utils.Internal;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxRichEdit.Export;
using DevExpress.XtraRichEdit.Commands;
using System.Web.UI;
using System.ComponentModel;
using DevExpress.Web.ASPxRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	[ToolboxItem(false)]
	public class RichEditControl : ASPxWebControlBase {
		public RichEditControl(ASPxRichEdit richEdit) {
			RichEdit = richEdit;
		}
		public ASPxRichEdit RichEdit { get; private set; }
		protected internal ASPxRibbon RibbonControl { get; set; }
		protected internal RichEditViewControl ViewControl { get; private set; }
		protected internal RichEditStatusBarControl StatusBarControl { get; private set; }
		protected internal RichEditDialogControl DialogControl { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateRibbon();
			CreateViewControl();
			CreateStatusBarControl();
			CreateDialogControl();
			CreatePopupMenuControl();
		}
		protected virtual void CreateRibbon() {
			if(RichEdit.RibbonMode == RichEditRibbonMode.ExternalRibbon) {
				RibbonControl = RibbonHelper.LookupRibbonControl(this, RichEdit.AssociatedRibbonID);
				if(RibbonControl != null) {
					RenderUtils.EnsureChildControlsRecursive(RibbonControl, false);
					RichEditRibbonHelper.UpdateRibbonTabCollection(RibbonControl.Tabs, RibbonControl.ContextTabCategories, RichEdit.CurrentSession, RichEdit.Settings);
				}
			} else if(RichEdit.RibbonMode != RichEditRibbonMode.None) {
				RibbonControl = CreateRibbonCore();
				RibbonControl.OneLineMode = IsOneLineRibbonMode();
				Controls.Add(RibbonControl);
			}
			if(RibbonControl != null)
				RenderUtils.EnsureChildControlsRecursive(RibbonControl, false);
		}
		bool IsOneLineRibbonMode() {
			bool isOneLineMode = RichEdit.RibbonMode == RichEditRibbonMode.OneLineRibbon;
			bool isAutoMode = RichEdit.RibbonMode == RichEditRibbonMode.Auto;
			bool isMobilePlatform = Browser.Platform.IsMobileUI;
			return isOneLineMode || (isAutoMode && isMobilePlatform);
		}
		protected ASPxRibbon CreateRibbonCore() {
			ASPxRibbon ribbon = new ASPxRibbon();
			ribbon.ID = ASPxRichEdit.RichEditRibbonContainerID;
			ribbon.ShowFileTab = false;
			ribbon.Width = Unit.Percentage(100);
			ribbon.EncodeHtml = RichEdit.EncodeHtml;
			ribbon.ParentSkinOwner = RichEdit;
			ribbon.ViewStateMode = ViewStateMode.Disabled;
			ribbon.Images.IconSet = RichEdit.Images.MenuIconSet;
			if(ribbon.Tabs.IsEmpty) {
				if (RichEdit.RibbonTabs.IsEmpty)
					ribbon.Tabs.AddRange(new RichEditDefaultRibbon(RichEdit).DefaultRibbonTabs);
				else
					ribbon.Tabs.Assign(RichEdit.RibbonTabs);
				RichEditRibbonHelper.UpdateRibbonTabCollection(ribbon.Tabs, ribbon.ContextTabCategories, RichEdit.CurrentSession, RichEdit.Settings);
			}
			if(ribbon.ContextTabCategories.IsEmpty) {
				if(RichEdit.RibbonContextTabCategories.IsEmpty)
					ribbon.ContextTabCategories.AddRange(new RichEditDefaultRibbon(RichEdit).DefaultRibbonContextTabCategories);
				else
					ribbon.ContextTabCategories.Assign(RichEdit.RibbonContextTabCategories);
				RichEditRibbonHelper.UpdateRibbonTabCollection(ribbon.Tabs, ribbon.ContextTabCategories, RichEdit.CurrentSession, RichEdit.Settings);
			}
			ribbon.ActiveTabIndex = RichEdit.ActiveTabIndex;
			return ribbon;
		}
		protected virtual void CreateViewControl() {
			ViewControl = new RichEditViewControl();
			Controls.Add(ViewControl);
		}
		protected virtual void CreateStatusBarControl() {
			if(RichEdit.ShowStatusBar) {
				StatusBarControl = new RichEditStatusBarControl(RichEdit);
				Controls.Add(StatusBarControl);
			}
		}
		protected virtual void CreateDialogControl() {
			DialogControl = new RichEditDialogControl(RichEdit);
			Controls.Add(DialogControl);
		}
		protected virtual void CreatePopupMenuControl() {
			RichEditPopupMenuControl popupMenu = new RichEditPopupMenuControl(RichEdit);
			Controls.Add(popupMenu);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, RichEditStyles.MainElementCssClass);
			RichEdit.GetControlStyle().AssignToControl(this, true);
			RenderUtils.AssignAttributes(RichEdit, this);
			if(RichEdit.RibbonMode == RichEditRibbonMode.ExternalRibbon || RichEdit.RibbonMode == RichEditRibbonMode.None)
				RenderUtils.AppendDefaultDXClassName(this, RichEditStyles.NotInternalRibbonClassName);
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		protected override bool HasRootTag() {
			return true;
		}
	}
	public class RichEditControlDesignTime : RichEditControl {
		public RichEditControlDesignTime(ASPxRichEdit richedit)
			: base(richedit) {
		}
		protected override void CreateRibbon() {
			if(RichEdit.RibbonMode == RichEditRibbonMode.ExternalRibbon)
				RibbonControl = RibbonHelper.LookupRibbonControl(this, RichEdit.AssociatedRibbonID);
			else if(RichEdit.RibbonMode != RichEditRibbonMode.None) {
				RibbonControl = CreateRibbonCore();
				RibbonControl.OneLineMode = RichEdit.RibbonMode == RichEditRibbonMode.OneLineRibbon;
				var row = RenderUtils.CreateTableRow();
				var cell = RenderUtils.CreateTableCell();
				cell.Controls.Add(RibbonControl);
				row.Controls.Add(cell);
				Controls.Add(row);
			}
			if(RibbonControl != null)
				RenderUtils.EnsureChildControlsRecursive(RibbonControl, false);
		}
		protected override void CreateViewControl() {
			RichEditViewControl viewControl = new RichEditViewControl();
			viewControl.Height = Unit.Percentage(100);
			WebControl fakePage = RenderUtils.CreateDiv();
			fakePage.Width = Unit.Pixel(800);
			fakePage.Height = Unit.Percentage(100);
			RenderUtils.AppendDefaultDXClassName(fakePage, RichEditStyles.PageClassName);
			viewControl.Controls.Add(fakePage);
			var row = RenderUtils.CreateTableRow();
			row.Height = Unit.Percentage(100);
			var cell = RenderUtils.CreateTableCell();
			cell.Controls.Add(viewControl);
			row.Controls.Add(cell);
			Controls.Add(row);
		}
		protected override void CreateStatusBarControl() {
			if(RichEdit.ShowStatusBar) {
				var statusBarControl = new RichEditStatusBarControl(RichEdit);
				var row = RenderUtils.CreateTableRow();
				var cell = RenderUtils.CreateTableCell();
				cell.Controls.Add(statusBarControl);
				row.Controls.Add(cell);
				Controls.Add(row);
			}
		}
		protected override void CreateDialogControl() {
			return;
		}
		protected override void CreatePopupMenuControl() {
			return;
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.AppendDefaultDXClassName(this, RichEditStyles.MainElementCssClass);
			RichEdit.GetControlStyle().AssignToControl(this, true);
			RenderUtils.AssignAttributes(RichEdit, this);
			Attributes["cellpadding"] = "0";
			Attributes["cellspacing"] = "0";
		}
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Table; } }
		protected override bool HasRootTag() { return true; }
	}
	[ToolboxItem(false)]
	public class RichEditViewControl : ASPxWebControlBase {
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = "View";
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, RichEditStyles.ViewClassName);
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		protected override bool HasRootTag() {
			return true;
		}
	}
	[ToolboxItem(false)]
	public class RichEditStatusBarControl : ASPxWebControlBase {
		public RichEditStatusBarControl(ASPxRichEdit owner) {
			RichEdit = owner;
		}
		protected ASPxRichEdit RichEdit { get; private set; }
		protected LoadingPanelControl LoadingPanel { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = "Bar";
			CreateLoadingPanel();
		}
		protected virtual void CreateLoadingPanel() {
			if(RichEdit.DesignMode) return;
			LoadingPanel = new RichEditLoadingPanelControl(false);
			LoadingPanel.EnableViewState = false;
			LoadingPanel.Image = RichEdit.Images.GetLoadingPanelImageProperties();
			LoadingPanel.Settings = new SettingsLoadingPanel(RichEdit);
			LoadingPanel.Settings.ImagePosition = ImagePosition.Right;
			WebControl LoadingPanelDiv = RenderUtils.CreateDiv();
			LoadingPanelDiv.ID = "LPC";
			LoadingPanelDiv.Controls.Add(LoadingPanel);
			Controls.Add(LoadingPanelDiv);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(!RichEdit.DesignMode)
				LoadingPanel.Style = RichEdit.Styles.GetLoadingPanelStyle();
			RenderUtils.AppendDefaultDXClassName(this, RichEditStyles.StatusBarClassName);
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		protected override bool HasRootTag() {
			return true;
		}
	}
	[ToolboxItem(false)]
	public class RichEditLoadingPanelControl : LoadingPanelControl, INamingContainer {
		public RichEditLoadingPanelControl(bool rtl) : base(rtl) { }
	}
	[ToolboxItem(false)]
	public class RichEditDialogControl : DevExpress.Web.ASPxPopupControl {
		ASPxRichEdit richedit;
		public RichEditDialogControl(ASPxRichEdit richedit)
			: base(null) {
			this.richedit = richedit;
			AllowDragging = true;
			EnableClientSideAPI = true;
			PopupAnimationType = AnimationType.Fade;
			Modal = true;
			CloseAction = CloseAction.CloseButton;
			CloseOnEscape = true;
			PopupHorizontalAlign = PopupHorizontalAlign.Center;
			PopupVerticalAlign = PopupVerticalAlign.Middle;
			PopupElementID = richedit.ID;
			PopupAction = PopupAction.None;
			ID = ASPxRichEdit.RicheditPopupDialogContainerID;
		}
		public ASPxRichEdit RichEdit {
			get { return richedit; }
			set { richedit = value; }
		}
		protected override bool LoadWindowsState(string state) {
			return false;
		}
		protected override StylesBase CreateStyles() {
			return new RichEditDialogFormStyles(this);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, RichEditStyles.DialogClassName);
		}
	}
	public class RichEditPopupMenuControl : ASPxInternalWebControl {
		public ASPxRichEdit RichEdit { get; private set; }
		public ASPxPopupMenu PopupMenu { get; private set; }
		public RichEditPopupMenuControl(ASPxRichEdit richedit) {
			RichEdit = richedit;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			PopupMenu = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			PopupMenu = new ASPxPopupMenu();
			PopupMenu.ID = ASPxRichEdit.RichEditPopupMenuContainerID;
			PopupMenu.ClientInstanceName = ASPxRichEdit.RichEditPopupMenuContainerID;
			PopupMenu.AccessibilityCompliant = !RenderUtils.Browser.Platform.IsTouchUI;
			PopupMenu.EnableScrolling = true;
			Controls.Add(PopupMenu);
			SetupMenuControl(PopupMenu);
			FillItems(PopupMenu);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PopupMenu.ParentStyles = RichEdit.StylesPopupMenu;
		}
		protected void SetupMenuControl(ASPxPopupMenu popupMenu) {
			popupMenu.CloseAction = PopupMenuCloseAction.OuterMouseClick;
			popupMenu.EncodeHtml = RichEdit.EncodeHtml;
			PopupMenu.EnableViewState = false;
			popupMenu.ClientSideEvents.ItemClick = "function(){}"; 
		}
		protected void FillItems(ASPxPopupMenu popupMenu) {
			RichEditPopupMenuItemCollection items = new RichEditPopupMenuItemCollection();
			items.CreateDefaultItems();
			foreach(RichEditPopupMenuItem item in items)
				if(item.Visible)
					PopupMenu.Items.Add(CreateMenuItem(item));
		}
		protected MenuItem CreateMenuItem(RichEditPopupMenuItem contextMenuItem) {
			MenuItem item = new MenuItem(contextMenuItem.Text, contextMenuItem.CommandName);
			item.BeginGroup = contextMenuItem.BeginGroup;
			item.Image.CopyFrom(GetItemImageProperty(RichEdit, contextMenuItem.ImageName));
			item.GroupName = contextMenuItem.CheckedGroupName;
			foreach(RichEditPopupMenuItem subItem in contextMenuItem.Items)
				item.Items.Add(CreateMenuItem(subItem));
			return item;
		}
		protected static ItemImagePropertiesBase GetItemImageProperty(ASPxRichEdit owner, string imageName) {
			var imageProperties = new ItemImageProperties();
			if(!string.IsNullOrEmpty(imageName))
				imageProperties.CopyFrom(owner.Images.GetImageProperties(owner.Page, imageName));
			return imageProperties;
		}
	}
}
