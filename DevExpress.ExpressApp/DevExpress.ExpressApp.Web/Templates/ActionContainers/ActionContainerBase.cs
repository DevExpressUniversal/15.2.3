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
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.ExpressApp.Web.UnitTesting;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public enum ActionContainerStyle { Links, Buttons, ToolBar };
	public abstract class WebActionBaseItem : ActionBaseItem, ITestable {
		protected override void SetShortcut(string shortcutString) { }
		public abstract string TestCaption { get; }
		public abstract string ClientId { get; }
		public abstract IJScriptTestControl TestControl { get; }
		public virtual TestControlType TestControlType {
			get { 
				return TestControlType.Action; 
			}
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		protected void OnControlInitialized(Control control) {
			if(control != null && ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		protected override bool ImageExist(string imageName) {
			return base.ImageExist(imageName) && !ImageLoader.Instance.GetImageInfo(imageName).IsUrlEmpty;
		}
		public WebActionBaseItem(ActionBase action)
			: base(action) {
		}
	}
	public abstract class ControlActionItem : WebActionBaseItem {
		public abstract Control Control { get; }
		public ControlActionItem(ActionBase action)
			: base(action) {
		}
	}
	public abstract class ButtonActionItem : ControlActionItem
#if DebugTest
, DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable
#endif
 {
		private ASPxButton button;
		protected override void SetEnabled(bool enabled) {
			button.ClientEnabled = enabled;
		}
		protected override void SetVisible(bool visible) {
			button.Visible = visible;
		}
		protected override void SetCaption(string caption) {
			button.Text = caption;
		}
		protected override void SetToolTip(string toolTip) {
			button.ToolTip = toolTip;
		}
		protected override void SetImage(ImageInfo imageInfo) {
			if(!imageInfo.IsEmpty) {
				ASPxImageHelper.SetImageProperties(button.Image, imageInfo, imageInfo.ImageName);
			}
			else {
				ASPxImageHelper.ClearImageProperties(button.Image);
			}
		}
		protected override void SetConfirmationMessage(string message) {
			ConfirmationsHelper.SetConfirmationScript(button, message);
		}
		public ButtonActionItem(ActionBase action)
			: base(action) {
			button = RenderHelper.CreateASPxButton();
			button.ID = WebIdHelper.GetCorrectedActionId(action);
			button.Style["vertical-align"] = "middle";
			button.Text = "";
			SynchronizeWithAction();
		}
		public override void Dispose() {
			try {
				if(button != null) {
					button.Dispose();
					button = null;
				}
			}
			finally {
				base.Dispose();
			}
		}
		public ASPxButton Button {
			get { return button; }
		}
		public override Control Control {
			get { return Button; }
		}
		public override string TestCaption {
			get { return Action.Caption; }
		}
		public override string ClientId {
			get { return button.ClientID; }
		}
		public override IJScriptTestControl TestControl {
			get {
				return new ASPxButtonTestControlScriptsDeclaration();
			}
		}
#if DebugTest
		#region IActionBaseItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlVisible {
			get { return Button.Visible; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlEnabled {
			get { return Button.ClientEnabled; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlCaption {
			get { return Button.Text; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlToolTip {
			get { return Button.ToolTip; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.SupportPaintStyle {
			get { return true; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ImageVisible {
			get { return !Button.Image.IsEmpty; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.CaptionVisible {
			get { return !string.IsNullOrEmpty(Button.Text); }
		}
		#endregion
#endif
	}
	public static class JSUpdateScriptHelper {
		public static string GetMenuId(ASPxMenuBase menu) {
			return string.IsNullOrEmpty(menu.ClientInstanceName) ? menu.ClientID : menu.ClientInstanceName;
		}
		public static string GetASPxMenuItemUpdateScript(ASPxMenuBase menu, string menuItemName, bool enabled) {
			return GetASPxMenuItemUpdateScript(GetMenuId(menu), menuItemName, enabled);
		}
		public static string GetASPxMenuItemUpdateScript(ASPxMenuBase menu, string menuItemName, bool enabled, bool visible) {
			return GetASPxMenuItemUpdateScript(GetMenuId(menu), menuItemName, enabled, visible);
		}
		public static string GetASPxMenuItemUpdateScript(string menuClientID, string menuItemName, bool enabled) {
			return string.Format(@"SetMenuItemEnabled(window.{0}, '{1}', {2});", menuClientID, menuItemName, enabled ? "true" : "false");
		}
		public static string GetASPxMenuItemUpdateScript(string menuClientID, string menuItemName, bool enabled, bool visible) {
			return GetASPxMenuItemUpdateScript(menuClientID, menuItemName, enabled) + string.Format(@"SetMenuItemVisible(window.{0}, '{1}', {2});", menuClientID, menuItemName, visible ? "true" : "false");
		}
		internal static string GetMenuUpdateScript(ASPxMenuBase menu, bool enabled) {
			string resultTemplate = @"if(window.{0}) {{ ";
			resultTemplate += GetMenuItemsUpdateScriptTemplate(menu.Items, "{0}", enabled);
			resultTemplate += "}}";
			return string.Format(resultTemplate, GetMenuId(menu)); ;
		}
		private static string GetMenuItemsUpdateScriptTemplate(DevExpress.Web.MenuItemCollection items, string parent, bool enabled) {
			string result = string.Empty;
			for(int i = 0; i < items.Count; i++) {
				string newParent = string.Format("{0}.GetItem({1})", parent, i);
				result += string.Format("{0}.SetEnabled({1});", newParent, (items[i].ClientEnabled && enabled) ? "true" : "false");
				result += GetMenuItemsUpdateScriptTemplate(items[i].Items, newParent, enabled);
			}
			return result;
		}
	}
	public class SimpleActionItem : ButtonActionItem
#if DebugTest
, DevExpress.ExpressApp.Tests.ISimpleActionItemUnitTestable
#endif
 {
		private void Button_Click(object sender, EventArgs e) {
			((SimpleAction)Action).DoExecute();
		}
		public override void Dispose() {
			try {
				if(Button != null) {
					Button.Click -= new EventHandler(Button_Click);
				}
			}
			finally {
				base.Dispose();
			}
		}
		public SimpleActionItem(SimpleAction action)
			: base(action) {
			Button.Click += new EventHandler(Button_Click);
		}
	}
	public class PopupWindowShowActionItem : ButtonActionItem
#if DebugTest
, DevExpress.ExpressApp.Tests.IPopupWindowShowActionItemUnitTestable
#endif
 {
		private const string sessionPopupActionPrefix = "SessionPopupAction";
		private string confirmationScript = "true";
		private void Button_Click(object sender, EventArgs e) {
			WebApplication.Instance.PopupWindowManager.ShowPopup((PopupWindowShowAction)Action, ((WebControl)sender).ClientID);
		}
		private void Button_PreRender(object sender, EventArgs e) {
			Button.PreRender -= new EventHandler(Button_PreRender);
			Button.ClientSideEvents.Click =
				"function button" + Button.GetHashCode().ToString() + "click(sender, e) {" +
				"e.processOnServer=false;" +
				"if(" + confirmationScript + ") {e.processOnServer=true;}}";
		}
		protected override void SetConfirmationMessage(string message) {
			confirmationScript = (ConfirmationsHelper.IsConfirmationsEnabled && !string.IsNullOrEmpty(message) ? ConfirmationsHelper.GetConfirmationScript(message) : "true");
		}
		public override void Dispose() {
			if(Button != null) {
				Button.Click -= new EventHandler(Button_Click);
				Button.PreRender -= new EventHandler(Button_PreRender);
			}
			base.Dispose();
		}
		public PopupWindowShowActionItem(PopupWindowShowAction action)
			: base(action) {
			Button.PreRender += new EventHandler(Button_PreRender);
			Button.Click += new EventHandler(Button_Click);
		}
		public override IJScriptTestControl TestControl {
			get {
				return new ASPxPopupWindowButtonTestControlScriptsDeclaration();
			}
		}
	}
	public class DefaultActionItem : ControlActionItem {
		private Label label;
		protected override void SetEnabled(bool enabled) { }
		protected override void SetVisible(bool visible) {
			label.Visible = visible;
		}
		protected override void SetCaption(string caption) {
			label.Text = caption;
		}
		protected override void SetToolTip(string toolTip) {
			label.ToolTip = toolTip;
		}
		protected override void SetImage(ImageInfo image) { }
		protected override void SetPaintStyle(ActionItemPaintStyle paintStyle) { }
		protected override void SetConfirmationMessage(string message) { }
		public DefaultActionItem(ActionBase action)
			: base(action) {
			label = new Label();
			label.ForeColor = Color.Gray;
			SynchronizeWithAction();
		}
		public override void Dispose() {
			try {
				if(label != null) {
					label.Dispose();
					label = null;
				}
			}
			finally {
				base.Dispose();
			}
		}
		public Label Label {
			get { return label; }
		}
		public override Control Control {
			get { return Label; }
		}
		public override string TestCaption {
			get { return Action.Caption; }
		}
		public override string ClientId {
			get { return label.ClientID; }
		}
		public override IJScriptTestControl TestControl {
			get { return new JSLabelTestControl(); }
		}
	}
	public class ActionUrlItem : TemplatedMenuActionItem
#if DebugTest
, DevExpress.ExpressApp.Tests.IActionUrlItemUnitTestable
#endif
 {
		ASPxHyperLink link;
		public ActionUrlItem(ActionUrl action)
			: base(action) {
			SynchronizeWithAction();
		}
		protected override Control CreateControlCore() {
			return Link;
		}
		protected virtual Control CreateTemplateControl() {
			link = RenderHelper.CreateASPxHyperLink();
			link.Style.Add("display", "block");
			link.Style.Add("padding", "4px");
			link.CssClass = "ActionUrlItem";
			link.EnableClientSideAPI = true;
			link.NavigateUrl = ((ActionUrl)Action).UrlFormatString;
			if (((ActionUrl)Action).OpenInNewWindow) {
				link.Target = "_blank";
			}
			return link;
		}
		public ASPxHyperLink Link {
			get {
				if (link == null) {
					link = CreateTemplateControl() as ASPxHyperLink;
				}
				return link;
		}
		}
		public override string TestCaption {
			get { return Action.Caption; }
		}
		public override string ClientId {
			get { return Link.ClientID; }
		}
		public override IJScriptTestControl TestControl {
			get {
				return new JSLabelTestControl(); ;
			}
		}
		protected override void SetEnabled(bool enabled) {
			Link.Enabled = enabled;
		}
		protected override void SetVisible(bool visible) {
			Link.Visible = visible;
		}
		protected override void SetCaption(string caption) {
			if(Action.PaintStyle != ActionItemPaintStyle.Image || !Action.HasImage) {
				Link.Text = caption;
			}
			else {
				Link.Text = string.Empty;
			}
		}
		protected override void SetToolTip(string toolTip) {
			Link.ToolTip = toolTip;
		}
		protected override void SetImage(ImageInfo imageInfo) {
			if(!imageInfo.IsEmpty && (Action.PaintStyle != ActionItemPaintStyle.Caption || string.IsNullOrEmpty(Action.Caption))) {
				Link.ImageUrl = imageInfo.ImageUrl;
				Link.ImageHeight = imageInfo.Height;
				Link.ImageWidth = imageInfo.Width;
			}
			else {
				Link.ImageUrl = string.Empty;
			}
		}
		protected override void SetConfirmationMessage(string message) {
			ConfirmationsHelper.SetConfirmationScript(Link, message);
		}
		public override void SetClientClickHandler(XafCallbackManager callbackManager, string controlID) { }
		public override void ProcessAction() { }
#if DebugTest
		#region IActionBaseItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlVisible {
			get { return Link.Visible; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlEnabled {
			get { return Link.Enabled; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlCaption {
			get { return Link.Text; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlToolTip {
			get { return Link.ToolTip; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.SupportPaintStyle {
			get { return true; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ImageVisible {
			get { return !string.IsNullOrEmpty(Link.ImageUrl); }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.CaptionVisible {
			get { return !string.IsNullOrEmpty(Link.Text); }
		}
		#endregion
#endif
	}
	public class TreeSingleChoiceActionItemOld : ControlActionItem {
		private SingleChoiceAction action;
		private ASPxMenu menu;
		private DevExpress.Web.MenuItem rootItem;
		private DevExpress.Web.MenuItem arrowItem;
		private Dictionary<DevExpress.Web.MenuItem, MenuChoiceActionItem> menuItemToMenuChoiceActionItemMap;
		private void menu_PreRender(object sender, EventArgs e) {
			this.menu.PreRender -= new EventHandler(menu_PreRender);
			SynchronizeWithAction();
			this.menu.ClientSideEvents.Init = string.Format(@"function(s, e){{ for(var i = 0; i < s.GetItemCount(); i++) s.GetItem(i).SetEnabled({0});}}", Action.Enabled ? "true" : "false");
		}
		private MenuChoiceActionItem CreateMenuChoiceActionItem(ChoiceActionItem actionItem, ChoiceActionBase action) {
			MenuChoiceActionItem result = new MenuChoiceActionItem(actionItem, action);
			menuItemToMenuChoiceActionItemMap[result.MenuItem] = result;
			return result;
		}
		private void CreateSubItems(DevExpress.Web.MenuItem parentMenuItem, ChoiceActionItemCollection choiceItems, ChoiceActionBase action) {
			foreach(ChoiceActionItem actionItem in choiceItems) {
				MenuChoiceActionItem menuItem = CreateMenuChoiceActionItem(actionItem, action);
				parentMenuItem.Items.Add(menuItem.MenuItem);
				CreateSubItems(menuItem.MenuItem, actionItem.Items, action);
			}
		}
		private void UpdateSelectedItem(DevExpress.Web.MenuItemCollection menuItems) {
			foreach(DevExpress.Web.MenuItem menuItem in menuItems) {
				MenuChoiceActionItem actionMenuItem = null;
				if(menuItemToMenuChoiceActionItemMap.TryGetValue(menuItem, out actionMenuItem)) {
					if(actionMenuItem.ActionItem == Action.SelectedItem) {
						actionMenuItem.MenuItem.Checked = true;
						menu.SelectedItem = actionMenuItem.MenuItem;
					}
					else {
						actionMenuItem.MenuItem.Checked = false;
					}
				}
				UpdateSelectedItem(menuItem.Items);
			}
		}
		private void UpdateSelectedItem() {
			UpdateSelectedItem(menu.Items);
		}
		private void RebuildContent() {
			if(this.arrowItem != null) {
				this.arrowItem.Items.Clear();
				this.menu.Items.Remove(arrowItem);
				this.arrowItem = null;
			}
			else {
				if(this.rootItem != null) {
					this.rootItem.Items.Clear();
				}
			}
			foreach(MenuChoiceActionItem item in menuItemToMenuChoiceActionItemMap.Values) {
				item.Dispose();
			}
			menuItemToMenuChoiceActionItemMap.Clear();
			if(Action != null && Action.Active) {
				if(this.rootItem != null) {
					this.rootItem.Text = Action.Caption;
					this.rootItem.ToolTip = Action.GetTotalToolTip();
					ASPxImageHelper.SetImageProperties(this.rootItem.Image, Action.ImageName);
				}
				if(Action.Items.Count > 0) {
					if(IsArrowRequire) {
						CreateArrow();
						CreateSubItems(this.arrowItem, Action.Items, Action);
					}
					else if(!IsNoItemsRequire) {
						CreateSubItems(this.rootItem, Action.Items, Action);
					}
					rootItem.TextTemplate = new MenuItemTextTemplate();
				}
				if(Action.ItemType == SingleChoiceActionItemType.ItemIsMode) {
					UpdateSelectedItem();
				}
			}
		}
		private bool IsArrowRequire {
			get {
				return !action.IsHierarchical() && action.Items.Count > 1;
			}
		}
		private bool IsNoItemsRequire {
			get {
				return !action.IsHierarchical() && action.ItemType == SingleChoiceActionItemType.ItemIsOperation && action.Items.Count == 1;
			}
		}
		private void CreateArrow() {
			this.arrowItem = new DevExpress.Web.MenuItem();
			this.arrowItem.Text = "";
			this.menu.Items.Add(this.arrowItem);
		}
		private void menu_ItemClick(object source, MenuItemEventArgs e) {
			if(e.Item == rootItem) {
				for(int i = 0; i < Action.Items.Count; i++) {
					if(Action.Items[i].Enabled) {
						Action.DoExecute(Action.Items[i]);
						break;
					}
				}
			}
			if(Action != null && menuItemToMenuChoiceActionItemMap.ContainsKey(e.Item) && (e.Item.Items.Count == 0)) {
				Action.DoExecute(menuItemToMenuChoiceActionItemMap[e.Item].ActionItem);
				if(Action.ItemType == SingleChoiceActionItemType.ItemIsMode) {
					UpdateSelectedItem();
				}
			}
		}
		public TreeSingleChoiceActionItemOld(SingleChoiceAction action)
			: base(action) {
			menuItemToMenuChoiceActionItemMap = new Dictionary<DevExpress.Web.MenuItem, MenuChoiceActionItem>();
			this.action = action;
			this.menu = RenderHelper.CreateASPxMenu();
			menu.SkinID = "SingleChoiceAction";
			this.menu.CssClass = "xafTreeAction";
			this.menu.AllowSelectItem = action.ItemType != SingleChoiceActionItemType.ItemIsMode;
			this.menu.ItemClick += new MenuItemEventHandler(menu_ItemClick);
			this.menu.EnableClientSideAPI = true;
			this.menu.ClientSideEvents.ItemClick = @"function(s, e){ if(e.item.GetItemCount() != 0) { e.processOnServer = false; } }";
			this.menu.PreRender += new EventHandler(menu_PreRender);
			this.rootItem = new DevExpress.Web.MenuItem();
			this.arrowItem = null;
			this.Action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
			this.menu.Items.Add(rootItem);
			RebuildContent();
		}
		void Action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			RebuildContent();
		}
		public override Control Control {
			get { return menu; }
		}
		public override string TestCaption {
			get { return Action.Caption; }
		}
		public override string ClientId {
			get { return JSUpdateScriptHelper.GetMenuId(menu); }
		}
		public override IJScriptTestControl TestControl {
			get { return new JSASPxMenuTestControl(); }
		}
		protected override void SetEnabled(bool enabled) {
			foreach(DevExpress.Web.MenuItem item in menu.Items) {
				item.Enabled = true;
			}
		}
		protected override void SetVisible(bool visible) {
			this.menu.Visible = visible;
		}
		protected override void SetCaption(string caption) {
			this.rootItem.Text = caption;
		}
		protected override void SetToolTip(string toolTip) {
			this.rootItem.ToolTip = toolTip;
		}
		protected override void SetImage(ImageInfo imageInfo) {
			ASPxImageHelper.SetImageProperties(this.rootItem.Image, imageInfo, imageInfo.ImageName);
		}
		protected override void SetConfirmationMessage(string message) {
			ConfirmationsHelper.SetConfirmationScript(menu, message);
		}
		public override void Dispose() {
			foreach(MenuChoiceActionItem menuItem in menuItemToMenuChoiceActionItemMap.Values) {
				menuItem.Dispose();
			}
			menuItemToMenuChoiceActionItemMap.Clear();
			if(this.menu != null) {
				this.menu.ItemClick -= new MenuItemEventHandler(menu_ItemClick);
				this.menu.Dispose();
				this.menu = null;
			}
			if(this.rootItem != null) {
				rootItem.Items.Clear();
				this.rootItem = null;
			}
			if(this.arrowItem != null) {
				arrowItem.Items.Clear();
				arrowItem = null;
			}
			if(this.Action != null) {
				this.Action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
			}
			base.Dispose();
			action = null;
		}
		public new SingleChoiceAction Action {
			get {
				return (SingleChoiceAction)base.Action;
			}
		}
	}
	public class ActionContainerActionVisibilityHelper {
		public bool IsActionVisible(ActionBase action) {
			switch(action.SelectionDependencyType) {
				case SelectionDependencyType.Independent: {
						return action.Active;
					}
				case SelectionDependencyType.RequireSingleObject: {
						return action.Active && (action.SelectionContext != null) &&
							((SelectionType.FocusedObject & action.SelectionContext.SelectionType) == SelectionType.FocusedObject);
					}
				case SelectionDependencyType.RequireMultipleObjects: {
						return action.Active && (action.SelectionContext != null) &&
							(action.SelectionContext.SelectionType != SelectionType.None);
					}
			}
			return true;
		}
	}
	public static class ActionExceptionHandler {
		public static void HandleException(HandleExceptionEventArgs e) {
			if(!e.Handled) {
				ErrorHandling.Instance.SetPageError(e.Exception);
				e.Handled = true;
			}
		}
	}
	public class MenuItemTextTemplate : ITemplate {
		private void htmlImage_PreRender(object sender, EventArgs e) {
			HtmlImage htmlImage = (HtmlImage)sender;
			htmlImage.Src = htmlImage.Page.ClientScript.GetWebResourceUrl(typeof(MenuItemTextTemplate), "DevExpress.ExpressApp.Web.Resources.ActionContainers.ArrowDn.gif");
			htmlImage.Width = 7;
			htmlImage.Height = 4;
			htmlImage.Border = 0;
		}
		#region ITemplate Members
		public void InstantiateIn(Control container) {
			DevExpress.Web.MenuItemTemplateContainer templateContainer = (DevExpress.Web.MenuItemTemplateContainer)container;
			Table table = RenderHelper.CreateTable();
			table.CssClass = "xafMenuItemTable";
			table.Rows.Add(new TableRow());
			if(!string.IsNullOrEmpty(templateContainer.Item.Text)) {
				TableCell tableCell = new TableCell();
				tableCell.CssClass = "xafTextCell";
				table.Rows[0].Cells.Add(tableCell);
				tableCell.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(templateContainer.Item.Text)));
			}
			if(templateContainer.Item.Items.Count > 0) {
				TableCell tableCell = new TableCell();
				tableCell.CssClass = "xafDropDownCell";
				table.Rows[0].Cells.Add(tableCell);
				HtmlImage htmlImage = new HtmlImage();
				htmlImage.PreRender += new EventHandler(htmlImage_PreRender);
				tableCell.Controls.Add(htmlImage);
			}
			container.Controls.Add(table);
		}
		#endregion
	}
	public class TreeSingleChoiceActionItem : ControlActionItem
#if DebugTest
, DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable
#endif
 {
		private SingleChoiceAction action;
		private TreeSingleChoiceActionControl control;
		private MenuChoiceActionHelper helper;
		private bool isMenuItemClickProcessing = false;
		private void action_SelectedItemChanged(object sender, EventArgs e) {
			if(!isMenuItemClickProcessing) {
				UpdateSelectedItem();
			}
		}
		private void Menu_ItemClick(object source, MenuItemEventArgs e) {
			isMenuItemClickProcessing = true;
			Action.DoExecute(helper.GetActionItemByMenuItem(e.Item));
			UpdateSelectedItem();
			isMenuItemClickProcessing = false;
		}
		private void MainButton_Click(object sender, EventArgs e) {
			Action.DoExecute(Action.Items.FirstActiveItem);
			UpdateSelectedItem();
		}
		private void Action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			RebuildContent();
		}
		private void UpdateSelectedItem(DevExpress.Web.MenuItemCollection menuItems) {
			helper.UpdateSelectedItem(menuItems);
		}
		private void UpdateSelectedItem() {
			if(Action != null && Action.ItemType == SingleChoiceActionItemType.ItemIsMode) {
				UpdateSelectedItem(control.Menu.Items);
			}
		}
		private void RebuildContent() {
			helper.ClearMap(); 
			if(Action != null ) {
				control.MainButton.Text = Action.Caption;
				ASPxImageHelper.SetImageProperties(control.MainButton.Image, Action.ImageName);
				if(Action.Items.Count > 0) {
					helper.RebuildMenu(control.Menu.Items);
				}
				UpdateSelectedItem();
				control.MainButton.ToolTip = Action.GetTotalToolTip();
			}
		}
		public TreeSingleChoiceActionItem(SingleChoiceAction action)
			: base(action) {
			this.action = action;
			this.control = new TreeSingleChoiceActionControl();
			this.control.ID = WebIdHelper.GetCorrectedActionId(action);
			this.control.MainButton.Click += new EventHandler(MainButton_Click);
			this.control.Menu.ItemClick += new MenuItemEventHandler(Menu_ItemClick);
			this.helper = new MenuChoiceActionHelper(action);
			control.Menu.AllowSelectItem = action.ItemType != SingleChoiceActionItemType.ItemIsMode;
			this.Action.SelectedItemChanged += new EventHandler(action_SelectedItemChanged);
			this.Action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
			RebuildContent();
			SynchronizeWithAction();
		}
		protected override void SetEnabled(bool enabled) {
			control.Enabled = enabled;
		}
		protected override void SetVisible(bool visible) {
			control.Visible = visible;
		}
		protected override void SetCaption(string caption) {
			if(Action.PaintStyle != ActionItemPaintStyle.Image) {
				control.MainButton.Text = caption;
			}
			else {
				control.MainButton.Text = "";
			}
		}
		protected override void SetToolTip(string toolTip) {
			control.MainButton.ToolTip = Action.GetTotalToolTip();
		}
		protected override void SetImage(ImageInfo imageInfo) {
			if(Action.PaintStyle != ActionItemPaintStyle.Caption) {
				ASPxImageHelper.SetImageProperties(control.MainButton.Image, imageInfo, imageInfo.ImageName);
			}
			else {
				ASPxImageHelper.ClearImageProperties(control.MainButton.Image);
			}
		}
		protected override void SetConfirmationMessage(string message) {
			((TreeSingleChoiceActionControl)Control).SetConfirmationMessage(message);
		}
		public override Control Control {
			get { return control; }
		}
		public override string TestCaption {
			get { return ((ITestable)control).TestCaption; }
		}
		public override string ClientId {
			get { return ((ITestable)control).ClientId; }
		}
		public override IJScriptTestControl TestControl {
			get {
				return ((ITestable)control).TestControl;
			}
		}
		public override void Dispose() {
			if(helper != null) {
				helper.ClearMap(); 
				helper = null;
			}
			if(control != null) {
				control.MainButton.Click -= new EventHandler(MainButton_Click);
				control.Menu.ItemClick -= new MenuItemEventHandler(Menu_ItemClick);
				control.Dispose();
			}
			if(this.action != null) {
				this.action.SelectedItemChanged -= new EventHandler(action_SelectedItemChanged);
				this.action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
			}
			base.Dispose();
			action = null;
		}
		public new SingleChoiceAction Action {
			get { return (SingleChoiceAction)base.Action; }
		}
#if DebugTest
		#region ISingleChoiceActionItemUnitTestable Members
		private DevExpress.Web.MenuItem FindItem(string path) {
			return ASPxMenuTestHelper.FindItem(path, control.Menu);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemVisible(string itemPath) {
			return FindItem(itemPath).Visible;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemEnabled(string itemPath) {
			return FindItem(itemPath).ClientEnabled;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemBeginsGroup(string itemPath) {
			return FindItem(itemPath).BeginGroup;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemImageVisible(string itemPath) {
			return !FindItem(itemPath).Image.IsEmpty;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemSelected(string itemPath) {
			return FindItem(itemPath).Selected;
		}
		#endregion
		#region IActionBaseItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlVisible {
			get { return control.MainButton.Visible; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlEnabled {
			get { return control.MainButton.ClientEnabled; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlCaption {
			get { return control.MainButton.Text; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlToolTip {
			get { return control.MainButton.ToolTip; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.SupportPaintStyle {
			get { return true; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ImageVisible {
			get { return !control.MainButton.Image.IsEmpty; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.CaptionVisible {
			get { return !string.IsNullOrEmpty(control.MainButton.Text); }
		}
		#endregion
#endif
	}
	public class ListChoiceActionItem : ChoiceActionItemWrapper {
		private ListEditItem listEditItem;
		public override void SetImageName(string imageName) {
			string imageUrl = null;
			if(!string.IsNullOrEmpty(imageName)) {
				ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
				if(!imageInfo.IsEmpty) {
					imageUrl = imageInfo.ImageUrl;
				}
			}
			listEditItem.ImageUrl = imageUrl;
		}
		public override void SetCaption(string caption) {
			listEditItem.Text = caption;
		}
		public override void SetData(object data) { }
		public override void SetShortcut(string shortcutString) { }
		public override void SetEnabled(bool enabled) { }
		public override void SetVisible(bool visible) { }
		public override void SetToolTip(string toolTip) { }
		public ListChoiceActionItem(ChoiceActionItem actionItem, ChoiceActionBase action)
			: base(actionItem, action) {
			listEditItem = new ListEditItem();
			if(!string.IsNullOrEmpty(actionItem.Id)) {
				listEditItem.Value = WebIdHelper.GetCorrectedId(actionItem.Id, WebIdHelper.GetCorrectedActionId(action));
			}
			else {
				listEditItem.Value = actionItem.GetHashCode().ToString();
			}
			SyncronizeWithItem();
		}
		public ListEditItem ListEditItem { get { return listEditItem; } }
		public override void Dispose() {
			base.Dispose();
			listEditItem = null;
		}
	}
	public class SingleChoiceActionItemAsOperationList : ControlActionItem
#if DebugTest
, DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable
#endif
 {
		public const string DuplicateItemErrorMessage = "The '{0}' action contains several items with the caption '{1}'.";
		private ASPxComboBox dropDownList;
		private Dictionary<ListEditItem, ListChoiceActionItem> items;
		private void dropDownList_SelectedIndexChanged(object source, EventArgs e) {
			if(dropDownList.SelectedIndex != -1 && items.ContainsKey(dropDownList.SelectedItem)) {
				((SingleChoiceAction)Action).DoExecute(items[dropDownList.SelectedItem].ActionItem);
			}
		}
		private void Action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			RebuildContent();
			SynchronizeWithAction();
		}
		private ListChoiceActionItem CreateListChoiceActionItem(ChoiceActionItem actionItem, ChoiceActionBase action) {
			ListChoiceActionItem result = new ListChoiceActionItem(actionItem, action);
			items.Add(result.ListEditItem, result);
			return result;
		}
		private void RebuildContent() {
			dropDownList.SelectedIndexChanged -= new EventHandler(dropDownList_SelectedIndexChanged);
			dropDownList.Items.Clear();
			foreach(ListChoiceActionItem listChoiceActionItem in items.Values) {
				listChoiceActionItem.Dispose();
			}
			items.Clear();
			if(Action.ItemType == SingleChoiceActionItemType.ItemIsOperation) {
				dropDownList.Items.Add(Action.Caption + "...");
			}
			foreach(ChoiceActionItem item in Action.Items) {
				if(item.Active && item.Enabled) {
					ListChoiceActionItem editorItem = CreateListChoiceActionItem(item, Action);
					dropDownList.Items.Add(editorItem.ListEditItem);
				}
			}
			if(Action.ItemType == SingleChoiceActionItemType.ItemIsOperation) {
				dropDownList.SelectedIndex = 0;
				dropDownList.ClientSideEvents.CloseUp = @"function CloseUp_" + dropDownList.ClientID + @"(s, e) {
					s.SelectIndex(0, false);
				}";
			}
			else if((Action.ItemType == SingleChoiceActionItemType.ItemIsMode) && (Action.SelectedItem != null)) {
				dropDownList.SelectedIndex = dropDownList.Items.IndexOfValue(Action.SelectedItem.GetHashCode().ToString());
			}
			dropDownList.SelectedIndexChanged += new EventHandler(dropDownList_SelectedIndexChanged);
		}
		protected override void SetEnabled(bool enabled) {
			dropDownList.ClientEnabled = enabled;
		}
		protected override void SetVisible(bool visible) {
			dropDownList.Visible = visible;
		}
		protected override void SetCaption(string caption) {
			RebuildContent();
		}
		protected override void SetToolTip(string toolTip) {
			dropDownList.ToolTip = toolTip;
		}
		protected override void SetImage(ImageInfo imageInfo) {
		}
		protected override void SetConfirmationMessage(string message) {
			ConfirmationsHelper.SetConfirmationScript(dropDownList, message);
		}
		public SingleChoiceActionItemAsOperationList(SingleChoiceAction action)
			: base(action) {
			items = new Dictionary<ListEditItem, ListChoiceActionItem>(Action.Items.Count);
			Action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
			dropDownList = RenderHelper.CreateASPxComboBox();
			dropDownList.Style["vertical-align"] = "middle";
			dropDownList.AutoPostBack = true;
			dropDownList.ID = WebIdHelper.GetCorrectedActionId(Action);
			RebuildContent();
			SynchronizeWithAction();
		}
		public new SingleChoiceAction Action {
			get { return (SingleChoiceAction)base.Action; }
		}
		public override void Dispose() {
			try {
				foreach(ListChoiceActionItem item in items.Values) {
					item.Dispose();
				}
				items.Clear();
				if(dropDownList != null) {
					dropDownList.SelectedIndexChanged -= new EventHandler(dropDownList_SelectedIndexChanged);
					dropDownList.Dispose();
					dropDownList = null;
				}
				if(Action != null) {
					Action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
				}
			}
			finally {
				base.Dispose();
			}
		}
		public override Control Control {
			get { return dropDownList; }
		}
		public override string TestCaption {
			get { return Action.Caption; }
		}
		public override string ClientId {
			get { return dropDownList.ClientID; }
		}
		public override IJScriptTestControl TestControl {
			get { return new JSASPxComboBoxTestControl(); }
		}
#if DebugTest
		#region ISingleChoiceActionItemUnitTestable Members
		private ListEditItem GetItemByPath(string itemPath) {
			ChoiceActionItem actionItem = Action.FindItemByCaptionPath(itemPath);
			if(actionItem != null) {
				foreach(ListChoiceActionItem itemWrapper in items.Values) {
					if(itemWrapper.ActionItem == actionItem) {
						return itemWrapper.ListEditItem;
					}
				}
			}
			return null;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemVisible(string itemPath) {
			return GetItemByPath(itemPath) != null;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemEnabled(string itemPath) {
			return GetItemByPath(itemPath) != null;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemBeginsGroup(string itemPath) {
			ChoiceActionItem actionItem = Action.FindItemByCaptionPath(itemPath);
			return actionItem == null ? false : actionItem.BeginGroup;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemImageVisible(string itemPath) {
			ListEditItem listItem = GetItemByPath(itemPath);
			return listItem == null ? false : !string.IsNullOrEmpty(listItem.ImageUrl);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemSelected(string itemPath) {
			return false;
		}
		#endregion
		#region IActionBaseItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlVisible {
			get { return dropDownList.Visible; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlEnabled {
			get { return dropDownList.ClientEnabled; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlCaption {
			get {
				if(Action.ItemType == SingleChoiceActionItemType.ItemIsOperation) {
					return dropDownList.Items[0].Text.Substring(0, dropDownList.Items[0].Text.Length - 3);
				}
				return Action.Caption;
			}
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlToolTip {
			get { return dropDownList.ToolTip; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.SupportPaintStyle {
			get { return false; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ImageVisible {
			get { return false; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.CaptionVisible {
			get { return true; }
		}
		#endregion
#endif
	}
}
