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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Web.UI;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Persistent.Base;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	public class CustomCreateActionControlEventArgs : HandledEventArgs {
		public CustomCreateActionControlEventArgs(ActionBase action, IActionContainer container) {
			Action = action;
			Container = container;
		}
		public ActionBase Action { get; private set; }
		public IActionContainer Container { get; private set; }
	}
	public class WebActionContainerHelper {
		public static void TryRegisterActionContainer(Control actionContainersHostControl, IEnumerable<IActionContainer> actionContainers) {
			Guard.ArgumentNotNull(actionContainersHostControl, "actionContainersHostControl");
			IDynamicContainersTemplate dynamicContainersTemplate = (IDynamicContainersTemplate)FindParent<IDynamicContainersTemplate>(actionContainersHostControl);
			if(dynamicContainersTemplate != null) {
				dynamicContainersTemplate.RegisterActionContainers(actionContainers);
			}
		}
		public static void UnregisterActionContainer(Control actionContainersHostControl, IEnumerable<IActionContainer> actionContainers) {
			Guard.ArgumentNotNull(actionContainersHostControl, "actionContainersHostControl");
			IDynamicContainersTemplate dynamicContainersTemplate = (IDynamicContainersTemplate)FindParent<IDynamicContainersTemplate>(actionContainersHostControl);
			if(dynamicContainersTemplate != null) {
				dynamicContainersTemplate.UnregisterActionContainers(actionContainers);
			}
		}
		public static InterfaceType FindParent<InterfaceType>(Control currentControl) {
			object result;
			if(currentControl == null) {
				result = null;
			}
			else if(currentControl.Parent is InterfaceType) {
				result = currentControl.Parent;
			}
			else {
				result = FindParent<InterfaceType>(currentControl.Parent);
			}
			return (InterfaceType)result;
		}
	}
	[ToolboxItem(false)] 
	[Designer("DevExpress.ExpressApp.Web.Design.ActionContainerHolderDesigner, DevExpress.ExpressApp.Web.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(IDesigner))]
	[ParseChildren(ChildrenAsProperties = true)]
	[PersistChildren(false)]
	public class ActionContainerHolder : System.Web.UI.WebControls.Panel, INamingContainer, IXafCallbackHandler, ISupportUpdate {
		private ASPxMenu menu;
		private WebActionContainersCollection actionContainers;
		private ActionContainerStyle containerStyle;
		private ActionContainerOrientation orientation;
		private bool showSeparators;
		private bool useLargeImage;
		private bool isControlsCreatingAllowed;
		protected Dictionary<ActionBase, MenuActionItemBase> actionItems;
		private static List<XafMenuItem> GetAllMenuItems(MenuItemCollection items) {
			List<XafMenuItem> result = new List<XafMenuItem>();
			foreach(MenuItem item in items) {
				if(item is XafMenuItem) {
					result.Add((XafMenuItem)item);
				}
				result.AddRange(GetAllMenuItems(item.Items));
			}
			return result;
		}
		private static void AssignMenuSkin(ASPxMenu menu, ActionContainerStyle containerStyle) {
			switch(containerStyle) {
				case ActionContainerStyle.Buttons:
					menu.SkinID = "MenuButtons";
					break;
				case ActionContainerStyle.Links:
					menu.SkinID = "MenuLinks";
					break;
				case ActionContainerStyle.ToolBar:
					menu.ShowAsToolbar = true;
					break;
			}
		}
		private static string GetCssClassByOrientation(ActionContainerOrientation orientation) {
			return orientation == ActionContainerOrientation.Horizontal ? "ACH" : "ACV";
		}
		private static Dictionary<string, string> GetClickHandlers(XafCallbackManager callbackManager, ASPxMenu menu, string uniqueID) {
			List<XafMenuItem> allMenuItems = GetAllMenuItems(menu.RootItem.Items);
			Dictionary<string, string> clickHandlers = new Dictionary<string, string>();
			foreach(XafMenuItem item in allMenuItems) {
				MenuActionItemBase menuActionItemBase = item.ActionProcessor as MenuActionItemBase;
				if(menuActionItemBase != null) {
					menuActionItemBase.SetClientClickHandler(callbackManager, uniqueID);
				}
				if(item.Provider != null) {
					clickHandlers.Add(item.IndexPath, item.Provider.GetScript(callbackManager, uniqueID, item.IndexPath));
				}
			}
			return clickHandlers;
		}
		private void container_ActionRegistered(object sender, ActionEventArgs e) {
			Clear();
		}
		private void container_ActionsClearing(object sender, EventArgs e) {
			UnsubsribeActionEvents((IActionContainer)sender);
			isControlsCreatingAllowed = false;
			Clear();
		}
		private void action_Changed(object sender, ActionChangedEventArgs e) {
			if(e.ChangedPropertyType == ActionChangedType.Active) {
				Clear();
			}
		}
		private void menu_CustomJSProperties(object sender, CustomJSPropertiesEventArgs e) {
			e.Properties["cpClickHandlers"] = GetClickHandlers(CallbackManager, Menu, UniqueID);
		}
		private void SetLastExecutedAction(ActionBase action) {
			WebActionContainer container = ActionContainers.FirstOrDefault(o => o.Actions.Contains(action));
			LastExecutedActions[container.ContainerId] = action.Id;
		}
		private string LastActionsDictionaryKey {
			get { return string.Format("LastActionID_{0}", ID); }
		}
		private Dictionary<string, string> LastExecutedActions {
			get {
				IValueManager<Dictionary<string, string>> valueManager = ValueManager.GetValueManager<Dictionary<string, string>>(LastActionsDictionaryKey);
				if(valueManager.Value == null) {
					valueManager.Value = new Dictionary<string, string>();
				}
				return valueManager.Value;
			}
		}
		private XafCallbackManager CallbackManager {
			get {
				Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), Page.GetType(), "Page");
				return ((ICallbackManagerHolder)Page).CallbackManager;
			}
		}
		private List<WebActionContainer> CreateContainers(string[] containerIds) {
			List<WebActionContainer> containers = new List<WebActionContainer>();
			foreach(string containerId in containerIds) {
				WebActionContainer webActionContainer = new WebActionContainer();
				webActionContainer.ContainerId = containerId;
				containers.Add(webActionContainer);
			}
			return containers;
		}
		private void PrepareContainersOnInit() {
			if(!DesignMode) {
				Menu.EnableClientSideAPI = true;
				Menu.EnableViewState = false;
				Menu.ID = "Menu";
				Menu.AccessibilityCompliant = true;
				Menu.ClientSideEvents.ItemClick =
@"function(s,e) {
    var indexPath = ''+ e.item.indexPath + '';
    var clickHandlerScript = s.cpClickHandlers[indexPath];
    if(clickHandlerScript) {
            if(typeof xaf == 'undefined' || !xaf.ConfirmUnsavedChangedController || xaf.ConfirmUnsavedChangedController.CanProcessCallbackForAction(s,e)){
                xafWaitForCallback(clickHandlerScript);
            }
    }
}";
				Menu.CustomJSProperties += new CustomJSPropertiesEventHandler(menu_CustomJSProperties);
				Menu.ItemWrap = false;
				Controls.Add(Menu);
			}
			if(ActionContainers.Count == 0 && !string.IsNullOrEmpty(categories)) {
				SetActionContainers(categories);
			}
			foreach(WebActionContainer actionContainer in ActionContainers) {
				actionContainer.ActionRegistered += new EventHandler<ActionEventArgs>(container_ActionRegistered);
				actionContainer.ActionsClearing += new EventHandler<EventArgs>(container_ActionsClearing);
			}
			WebActionContainerHelper.TryRegisterActionContainer(this, ActionContainers);
			if(!DesignMode) {
				CallbackManager.RegisterHandler(UniqueID, this);
				ActionContainers.SetReadOnly(true);
			}
		}
		#region CreateMenuItems()
		private void CreateMenuItems() {
			if(Menu == null) {
				return;
			}
			if(!Controls.Contains(Menu)) {
				Controls.Add(Menu);
			}
			Menu.JSProperties.Remove("cpDropDownContainers");
			foreach(WebActionContainer actionContainer in ActionContainers) {
				RegisterContainerActions(actionContainer);
			}
			UpdateMenuVisibility();
			OnMenuItemsCreated();
		}
		private ActionItemPaintStyle ConteinerPaintStyle(WebActionContainer container) {
			foreach(ActionBase action in container.Actions) {
				if(action.PaintStyle != ActionItemPaintStyle.Caption) {
					return ActionItemPaintStyle.CaptionAndImage;
				}
			}
			return ActionItemPaintStyle.Caption;
		}
		protected virtual void RegisterContainerActions(WebActionContainer container) {
			MenuItemCollection targetItemCollection = Menu.Items;
			if(container.IsDropDown && container.HasActiveActions) {
				MenuItem rootMenuItem = CreateRootMenuItem(container);
				ActionItemPaintStyle subMenuPaintStyle = ConteinerPaintStyle(container);
				if(subMenuPaintStyle == ActionItemPaintStyle.Caption) {
					rootMenuItem.SubMenuItemStyle.CssClass = container.DropDownMenuItemCssClass + " -subItem captionOnly";
				}
				else {
					rootMenuItem.SubMenuItemStyle.CssClass = container.DropDownMenuItemCssClass + " -subItem";
				}
				rootMenuItem.ItemStyle.CssClass = container.DropDownMenuItemCssClass;
				SetupDropDownMenuItems(rootMenuItem, container.ContainerId);
				Menu.Items.Add(rootMenuItem);
				SetupContainerItemForEasyTest(container, Menu, rootMenuItem);
				targetItemCollection = rootMenuItem.Items;
			}
			bool isFirstItem = true;
			foreach(ActionBase action in container.Actions) {
				action.Changed -= new EventHandler<ActionChangedEventArgs>(action_Changed);
				action.Changed += new EventHandler<ActionChangedEventArgs>(action_Changed);
				if(action.Active) {
					CustomCreateActionControlEventArgs customCreateActionControlEventArgs = new CustomCreateActionControlEventArgs(action, container);
					OnCustomCreateActionControl(customCreateActionControlEventArgs);
					if(!customCreateActionControlEventArgs.Handled) {
						MenuActionItemBase actionItem = AddActionItem(targetItemCollection, action, isFirstItem);
						if(actionItem.MenuItem.DropDownMode) {
							SetupDropDownMenuItems(actionItem.MenuItem, action.Id);
						}
						AddActionItemToTestableControls(container, actionItem);
						if(isFirstItem) {
							isFirstItem = false;
						}
					}
				}
			}
		}
		private void SetupDropDownMenuItems(MenuItem menuItem, string Id) {
			string dropDownCssClass = "dropDown" + Id;
			menuItem.SubMenuItemStyle.CssClass = menuItem.SubMenuItemStyle.CssClass + " " + dropDownCssClass;
			object dropDownHoverClasses;
			if(!menu.JSProperties.TryGetValue("cpDropDownHoverClasses", out dropDownHoverClasses) || !(dropDownHoverClasses is List<string>)) {
				dropDownHoverClasses = new List<string>();
			}
			((List<string>)dropDownHoverClasses).Add(dropDownCssClass);
			menu.JSProperties["cpDropDownHoverClasses"] = dropDownHoverClasses;
			Menu.ClientSideEvents.Init = @"function(s, e){
                                    if(typeof xaf != 'undefined' && typeof xaf.ASPxClientMenuController != 'undefined'){
                                        xaf.ASPxClientMenuController.SetupASPxClientMenu(s);
                                    }
                                }";
			menuItem.ItemStyle.CssClass = menuItem.ItemStyle.CssClass + " " + dropDownCssClass;
		}
		private MenuItem CreateRootMenuItem(WebActionContainer container) {
			MenuItem rootMenuItem;
			if(!string.IsNullOrEmpty(container.DefaultItemCaption)) {
				rootMenuItem = new MenuItem();
				rootMenuItem.Text = container.DefaultItemCaption;
				rootMenuItem.Image.Url = container.DefaultItemImageUrl;
				rootMenuItem.DropDownMode = false;
			}
			else {
				ActionBase defaultAction = GetDefaultAction(container);
				MenuActionItemBase defaultActionItem = CreateActionItem(defaultAction);
				if(defaultAction is SingleChoiceAction) {
					defaultActionItem.MenuItem.Items.Clear();
				}
				rootMenuItem = defaultActionItem.MenuItem;
				rootMenuItem.DropDownMode = true;
			}
			return rootMenuItem;
		}
		private ActionBase GetDefaultAction(WebActionContainer container) {
			ActionBase result = null;
			string lastExecutedActionId;
			if(container.AutoChangeDefaultAction && LastExecutedActions.TryGetValue(container.ContainerId, out lastExecutedActionId)) {
				result = container.Actions.FirstOrDefault(action => action.Id == lastExecutedActionId);
			}
			if(result == null) {
				result = container.Actions.FirstOrDefault(action => action.Id == container.DefaultActionID);
			}
			if(result == null) {
				result = container.Actions.FirstOrDefault(action => action.Active && action.Enabled);
			}
			return result;
		}
		private static void SetupContainerItemForEasyTest(WebActionContainer container, ASPxMenu menu, MenuItem rootMenuItem) {
			if(TestScriptsManager.EasyTestEnabled) {
				rootMenuItem.Name = container.ContainerId;
				object dropdownContainers;
				if(!menu.JSProperties.TryGetValue("cpDropDownContainers", out dropdownContainers) || !(dropdownContainers is List<string>)) {
					dropdownContainers = new List<string>();
					menu.JSProperties["cpDropDownContainers"] = dropdownContainers;
				}
				((List<string>)dropdownContainers).Add(rootMenuItem.Name);
			}
		}
		protected virtual MenuActionItemBase AddActionItem(MenuItemCollection targetItemCollection, ActionBase action, bool isFirstItem) {
			CreateCustomMenuActionItemEventArgs createCustomMenuActionItemEventArgs = new CreateCustomMenuActionItemEventArgs(action);
			OnCreateCustomMenuActionItem(createCustomMenuActionItemEventArgs);
			MenuActionItemBase actionItem = createCustomMenuActionItemEventArgs.ActionItem != null ? createCustomMenuActionItemEventArgs.ActionItem : CreateActionItem(action);
			actionItem.MenuItem.SlidingBeginGroup = isFirstItem;
			actionItems[action] = actionItem;
			AddMenuItem(actionItem, targetItemCollection);
			MenuActionItemCreatedEventArgs menuActionItemCreatedEventArgs = new MenuActionItemCreatedEventArgs(actionItem);
			OnMenuActionItemCreated(menuActionItemCreatedEventArgs);
			return actionItem;
		}
		protected virtual void AddMenuItem(MenuActionItemBase actionItem, MenuItemCollection itemsCollection) {
			itemsCollection.Add(actionItem.MenuItem);
		}
		private void AddActionItemToTestableControls(WebActionContainer container, MenuActionItemBase actionItem) {
			if(TestScriptsManager.EasyTestEnabled) {
				List<ITestable> testableControlList;
				if(!testableControls.TryGetValue(container, out testableControlList)) {
					testableControlList = new List<ITestable>();
					testableControls.Add(container, testableControlList);
				}
				testableControlList.Add(actionItem);
			}
		}
		private MenuActionItemBase CreateActionItem(ActionBase action) {
			MenuActionItemBase actionItem = null;
			if(action is SimpleAction) {
				actionItem = CreateSimpleActionItem((SimpleAction)action);
			}
			else if(action is SingleChoiceAction) {
				actionItem = CreateSingleChoiceActionItem((SingleChoiceAction)action);
			}
			else if(action is ParametrizedAction) {
				actionItem = CreateParametrizedActionItem((ParametrizedAction)action);
			}
			else if(action is PopupWindowShowAction) {
				actionItem = CreatePopupWindowShowActionItem((PopupWindowShowAction)action);
			}
			else if(action is ActionUrl && action.SelectionDependencyType == SelectionDependencyType.Independent) {
				actionItem = CreateActionUrlItem((ActionUrl)action);
			}
			else {
				actionItem = CreateDefaultActionItem(action);
			}
			actionItem.UseLargeImage = UseLargeImage;
			return actionItem;
		}
		protected virtual MenuActionItemBase CreateSimpleActionItem(SimpleAction simpleAction) {
			return new SimpleActionMenuActionItem(simpleAction);
		}
		protected virtual MenuActionItemBase CreateSingleChoiceActionItem(SingleChoiceAction singleChoiceAction) {
			if(singleChoiceAction.ItemType == SingleChoiceActionItemType.ItemIsMode) {
				if(singleChoiceAction.IsHierarchical()) {
					return new SingleChoiceActionItemAsHierarchicalModeActionMenuItem(singleChoiceAction);
				}
				else {
					SingleChoiceActionAsModeMenuActionItem actionItem = new SingleChoiceActionAsModeMenuActionItem(singleChoiceAction);
					actionItem.Orientation = Orientation == ActionContainerOrientation.Horizontal ? SingleChoiceActionItemOrientation.Horizontal : SingleChoiceActionItemOrientation.Vertical;
					return actionItem;
				}
			}
			else {
				return new SingleChoiceActionItemAsOperationActionMenuItem(singleChoiceAction);
			}
		}
		protected virtual MenuActionItemBase CreateParametrizedActionItem(ParametrizedAction parametrizedAction) {
			ParametrizedActionMenuActionItem actionItem = new ParametrizedActionMenuActionItem(parametrizedAction);
			actionItem.Orientation = Orientation;
			return actionItem;
		}
		protected virtual MenuActionItemBase CreatePopupWindowShowActionItem(PopupWindowShowAction popupWindowShowAction) {
			return new PopupWindowActionMenuActionItem(popupWindowShowAction);
		}
		protected virtual MenuActionItemBase CreateActionUrlItem(ActionUrl actionUrl) {
			return new ActionUrlItem(actionUrl);
		}
		protected virtual MenuActionItemBase CreateDefaultActionItem(ActionBase action) {
			return new DefaultMenuActionItem(action);
		}
		public void UpdateMenuVisibility() {
			if(Menu.Items.Count > 0) {
				UpdateGroupsAndIndexes(Menu);
				Menu.Visible = true;
				Style["display"] = "inherit";
			}
			else {
				Style["display"] = "none";
				if(WebApplicationStyleManager.IsNewStyle) {
					Style["padding-left"] = "inherit";
				}
				Menu.Visible = false;
			}
		}
		private static void UpdateGroupsAndIndexes(ASPxMenu menu) {
			bool isFirstItem = true;
			foreach(MenuItem item in menu.Items) {
				if(!isFirstItem && item is XafMenuItem && ((XafMenuItem)item).SlidingBeginGroup) {
					item.BeginGroup = true;
				}
				isFirstItem = false;
			}
		}
		#endregion
		private void UnsubsribeActionEvents(IActionContainer container) {
			if(container != null) {
				foreach(ActionBase action in container.Actions) {
					action.Changed -= new EventHandler<ActionChangedEventArgs>(action_Changed);
				}
			}
		}
		protected virtual void ClearChildControls(bool disposing) {
			if(testableControls != null) {
				testableControls.Clear();
			}
			if(Menu != null) {
				List<XafMenuItem> items = GetAllMenuItems(Menu.RootItem.Items);
				for(int i = 0; i < items.Count; i++) {
					IDisposable disposable = items[i].ActionProcessor as IDisposable;
					if(disposable != null) {
						disposable.Dispose();
					}
				}
				Menu.Items.Clear();
				if(disposing) {
					Menu.CustomJSProperties -= new CustomJSPropertiesEventHandler(menu_CustomJSProperties);
					Menu.Dispose();
					menu = null;
				}
			}
			ChildControlsCreated = false; 
			if(TestScriptsManager.EasyTestEnabled && !disposing && isControlsCreatingAllowed) {
				EnsureChildControls();
			}
		}
		protected internal new void EnsureChildControls() {
			base.EnsureChildControls();
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			CreateMenuItems();
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			PrepareContainersOnInit();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(TestScriptsManager.EasyTestEnabled) {
				isControlsCreatingAllowed = true;
				EnsureChildControls();
			}
		}
		protected override void Render(HtmlTextWriter writer) {
			if(DesignMode) {
				writer.Write(GetDesignTimeHtml());
			}
			else {
				EnsureChildControls();
				base.Render(writer);
			}
		}
		protected override void OnUnload(EventArgs e) {
			WebActionContainerHelper.UnregisterActionContainer(this, ActionContainers);
			base.OnUnload(e);
		}
		private void OnCustomCreateActionControl(CustomCreateActionControlEventArgs customCreateActionControlEventArgs) {
			if(CustomCreateActionControl != null) {
				CustomCreateActionControl(this, customCreateActionControlEventArgs);
			}
		}
		protected virtual void OnCreateCustomMenuActionItem(CreateCustomMenuActionItemEventArgs args) {
			if(CreateCustomMenuActionItem != null) {
				CreateCustomMenuActionItem(this, args);
			}
		}
		protected virtual void OnMenuActionItemCreated(MenuActionItemCreatedEventArgs args) {
			if(MenuActionItemCreated != null) {
				MenuActionItemCreated(this, args);
			}
		}
		private void OnMenuItemsCreated() {
			if(MenuItemsCreated != null) {
				MenuItemsCreated(this, EventArgs.Empty);
			}
		}
		public ActionContainerHolder() {
			actionContainers = new WebActionContainersCollection();
			actionContainers.Owner = this;
			actionItems = new Dictionary<ActionBase, MenuActionItemBase>();
			menu = RenderHelper.CreateASPxMenu();
			Orientation = ActionContainerOrientation.Horizontal;
			Wrap = false;
		}
		public bool HasActiveActions() {
			foreach(WebActionContainer container in ActionContainers) {
				if(container.HasActiveActions) {
					return true;
				}
			}
			return false;
		}
		public void SetActionContainers(string categories) {
			SetActionContainers(CreateContainers(categories.Split(';')));
		}
		public void SetActionContainers(IList<WebActionContainer> containers) {
			ClearContainers();
			foreach(WebActionContainer container in containers) {
				ActionContainers.Add(container);
			}
		}
		public IActionContainer FindActionContainerById(string containerId) {
			foreach(IActionContainer container in ActionContainers) {
				if(container.ContainerId == containerId) {
					return container;
				}
			}
			return null;
		}
		public void ClearContainers() {
			foreach(WebActionContainer container in ActionContainers) {
				container.Dispose();
			}
			ActionContainers.Clear();
		}
		public void Clear() {
			if(ChildControlsCreated) {
				ClearChildControls(false);
			}
		}
		public override void Dispose() {
			foreach(WebActionContainer container in ActionContainers) {
				UnsubsribeActionEvents(container);
				container.ActionRegistered -= new EventHandler<ActionEventArgs>(container_ActionRegistered);
				container.ActionsClearing -= new EventHandler<EventArgs>(container_ActionsClearing);
				container.Dispose();
			}
			ActionContainers.SetReadOnly(false);
			ActionContainers.Clear();
			ClearChildControls(true);
			base.Dispose();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public MenuActionItemBase FindActionItem(ActionBase action) {
			MenuActionItemBase result;
			actionItems.TryGetValue(action, out result);
			return result;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string GetDesignTimeHtml() {
			string result = @"<table cellspacing=""0"" cellpadding=""0"" border=""0""><tr>";
			bool isVertical = Orientation == ActionContainerOrientation.Vertical;
			foreach(WebActionContainer container in ActionContainers) {
				WebActionContainerDesignerHelper helper = new WebActionContainerDesignerHelper(container);
				result += "<td>" + helper.GetDesignTimeHtml() + "</td>" + (isVertical ? @"</tr><tr>" : "");
			}
			result += "</tr></table>";
			return result;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public ASPxMenu Menu {
			get { return menu; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		public WebActionContainersCollection ActionContainers {
			get { return actionContainers; }
		}
		public ActionContainerStyle ContainerStyle {
			get { return containerStyle; }
			set {
				containerStyle = value;
				AssignMenuSkin(Menu, containerStyle);
			}
		}
		public ActionContainerOrientation Orientation {
			get { return orientation; }
			set {
				if(string.IsNullOrEmpty(CssClass) || CssClass == GetCssClassByOrientation(orientation)) {
					CssClass = GetCssClassByOrientation(value);
				}
				orientation = value;
				Menu.Orientation = orientation == ActionContainerOrientation.Horizontal ? System.Web.UI.WebControls.Orientation.Horizontal : System.Web.UI.WebControls.Orientation.Vertical;
			}
		}
		public bool ShowSeparators {
			get { return showSeparators; }
			set {
				showSeparators = value;
				Menu.AutoSeparators = showSeparators ? AutoSeparatorMode.RootOnly : AutoSeparatorMode.None;
			}
		}
		public System.Web.UI.WebControls.Unit SeparatorHeight {
			get { return Menu.SeparatorHeight; }
			set { Menu.SeparatorHeight = value; }
		}
		public bool UseLargeImage {
			get { return useLargeImage; }
			set { useLargeImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Wrap {
			get { return base.Wrap; }
			set { base.Wrap = value; }
		}
		public event EventHandler<CustomCreateActionControlEventArgs> CustomCreateActionControl; 
		public event EventHandler<CreateCustomMenuActionItemEventArgs> CreateCustomMenuActionItem;
		public event EventHandler<MenuActionItemCreatedEventArgs> MenuActionItemCreated;
		public event EventHandler MenuItemsCreated;
		#region IXafCallbackHandler Members
		private void ExecuteMenuItem(IList<XafMenuItem> items, string indexPath) {
			foreach(XafMenuItem item in items) {
				if(item.IndexPath == indexPath) {
					ActionBase action = item.ActionProcessor.Action;
					SetLastExecutedAction(action);
					item.ActionProcessor.ProcessAction();
					Clear();
					EnsureChildControls();  
					return;
				}
			}
		}
		void IXafCallbackHandler.ProcessAction(string parameter) {
			EnsureChildControls(); 
			ExecuteMenuItem(GetAllMenuItems(Menu.Items), parameter);
		}
		#endregion
		#region ITestableContainer Members
		private Dictionary<IActionContainer, List<ITestable>> testableControls = new Dictionary<IActionContainer, List<ITestable>>();
		public ITestable[] GetTestableControls(IActionContainer container) {
			List<ITestable> result;
			if(!testableControls.TryGetValue(container, out result)) {
				return new ITestable[] { };
			}
			return result.ToArray();
		}
		#endregion
		#region ISupportUpdate Members
		public void BeginUpdate() {
		}
		public void EndUpdate() {
		}
		#endregion
#if DebugTest
		public static Dictionary<string, string> DebugTest_GetClickHandlers(XafCallbackManager callbackManager, ASPxMenu menu, string uniqueID) {
			return GetClickHandlers(callbackManager, menu, uniqueID);
		}
		public static List<XafMenuItem> DebugTest_GetAllMenuItems(MenuItemCollection items) {
			return GetAllMenuItems(items);
		}
		public void DebugTest_UpdateGroupsAndIndexes() {
			UpdateGroupsAndIndexes(Menu);
		}
		public void DebugTest_RegisterContainerActions(WebActionContainer container) {
			RegisterContainerActions(container);
		}
		public void DebugTest_PrepareContainersOnInit() {
			PrepareContainersOnInit();
		}
		public Dictionary<string, string> DebugTest_LastExecutedActions {
			get { return LastExecutedActions; }
		}
		public void DebugTest_ClearChildControls(bool disposing) {
			ClearChildControls(disposing);
		}
		public bool DebugTest_ChildControlsCreated { get { return ChildControlsCreated; } }
		public void DebugTest_EnsureChildControls() {
			EnsureChildControls();
		}
#endif
		#region Obsolete 15.1
		[Obsolete("Use the ActionContainerHolder() instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ActionContainerHolder(ControlRenderMode renderMode) {
			Initialize(RenderHelper.CreateASPxMenu());
		}
		[Obsolete(ObsoleteMessages.MemberIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void Initialize(ASPxMenu createdMenu) {
			actionContainers = new WebActionContainersCollection();
			actionContainers.Owner = this;
			actionItems = new Dictionary<ActionBase, MenuActionItemBase>();
			menu = createdMenu;
			Orientation = ActionContainerOrientation.Horizontal;
			this.Wrap = false;
		}
		[Obsolete("Use the ClearContainers method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearContiners() {
			ClearContainers();
		}
		[Obsolete("Use the Initialize method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void Initlialize(ASPxMenu createdMenu) {
			Initialize(createdMenu);
		}
		[Obsolete("Use the WebActionContainer.PaintStyle property instead.", true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ActionItemPaintStyle PaintStyle {
			get { return ActionItemPaintStyle.Default; }
			set {
				foreach(WebActionContainer container in ActionContainers) {
					container.PaintStyle = value;
				}
			}
		}
		[Obsolete("Use the ActionContainers.Add(WebActionContainer container) method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void AddActionContainer(WebActionContainer container) {
			ActionContainers.Add(container);
			Clear();
		}
		private string categories;
		[Obsolete("Use the ActionContainers collection property to define action containers."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string Categories {
			get { return categories; }
			set { categories = value; }
		}
		#endregion
	}
	public class WebActionContainerDesignerHelper {
		private WebActionContainer container;
		private string GetTwoActionsHtmlTemplate(bool isToolBar) {
			string borderStyle = isToolBar ? "solid 1px #888888" : "none";
			string style = isToolBar ? "; background-color: #eeeeee" : "";
			string result;
			if(Container.Owner.Orientation == ActionContainerOrientation.Horizontal) {
				result = @"
    <table cellspacing=""0"" cellpadding=""5"" border=""0"" style=""border-collapse: collapse" + style + @""">
    <tr><td style=""border:" + borderStyle + @""">{0}</td><td style=""border:" + borderStyle + @""">{1}</td></tr>
    </table>
    ";
			}
			else {
				result = @"
    <table cellspacing=""0"" cellpadding=""5"" border=""0"" style=""border-collapse: collapse" + style + @""">
    <tr><td style=""border:" + borderStyle + @""">{0}</td></tr>
    <tr><td style=""border:" + borderStyle + @""">{1}</td></tr>
    </table>
    ";
			}
			return result;
		}
		private string GetToolBarContainerHtml(string containerId) {
			if(this.Container.Owner.Orientation == ActionContainerOrientation.Horizontal) {
				return string.Format(GetOneActionHtmlTemplate(true), GetItemHtml(containerId, false, false));
			}
			else {
				return string.Format(GetTwoActionsHtmlTemplate(true), GetItemHtml(containerId + "1", false, false), GetItemHtml(containerId + "2", false, false));
			}
		}
		private string GetOneActionHtmlTemplate(bool isToolBar) {
			string style = isToolBar ? "; background-color: #eeeeee" : "";
			return @"
                <table cellspacing=""0"" cellpadding=""5"" border=""0"" style=""border-collapse: collapse" + style + @""">
                    <tr>
                        <td style=""border: none"">{0}</td>
                    </tr>
                </table>";
		}
		private string GetItemHtml(string caption, bool isButton, bool isLink) {
			string style = isButton ? "background-color: #eeeeee; border: solid 1px #888888" : "";
			string template = @"
    <table cellspacing=""3"" cellpadding=""2"" border=""0"" style=""" + style + @""">
    <tr>{0}{1}</tr>
    </table>
    ";
			return string.Format(template, GetImageCellHtml(), GetTextCellHtml(caption, isLink));
		}
		private string GetLinksContainerHtml(string containerId) {
			if(this.Container.Owner.Orientation == ActionContainerOrientation.Horizontal) {
				return string.Format(GetOneActionHtmlTemplate(false), GetItemHtml(containerId, false, true));
			}
			else {
				return string.Format(GetTwoActionsHtmlTemplate(false), GetItemHtml(containerId + "1", false, true), GetItemHtml(containerId + "2", false, true));
			}
		}
		private string GetButtonsContainerHtml(string containerId) {
			if(this.Container.Owner.Orientation == ActionContainerOrientation.Horizontal) {
				return string.Format(GetOneActionHtmlTemplate(false), GetItemHtml(containerId, true, false));
			}
			else {
				return string.Format(GetTwoActionsHtmlTemplate(false), GetItemHtml(containerId + "1", true, false), GetItemHtml(containerId + "2", true, false));
			}
		}
		private string GetImageCellHtml() {
			return @"<td style=""border: solid 1px #444444; width: 16px; height: 16px; background-color: #cccccc; color: black; font-size: 7px; font-family: Arial; text-align: center"">img</td>";
		}
		private string GetTextCellHtml(string text, bool isLink) {
			if(isLink) {
				return @"<td style=""padding-left: 5px; color: blue; font-size: 11px; font-family: Arial; text-decoration: underline"">" + text + "</td>";
			}
			else {
				return @"<td style=""padding-left: 5px; color: black; font-size: 11px; font-family: Arial;"">" + text + "</td>";
			}
		}
		protected WebActionContainer Container {
			get { return (WebActionContainer)container; }
		}
		public WebActionContainerDesignerHelper(WebActionContainer container) {
			this.container = container;
		}
		public string GetDesignTimeHtml() {
			string template = @"
                <table cellspacing=""0"" cellpadding=""0"" border=""0"">
                <tr><td>{0}</td></tr>
                </table>
                ";
			string containerHtml = string.Empty;
			switch(Container.Owner.ContainerStyle) {
				case ActionContainerStyle.Buttons:
					containerHtml = GetButtonsContainerHtml(Container.ContainerId);
					break;
				case ActionContainerStyle.Links:
					containerHtml = GetLinksContainerHtml(Container.ContainerId);
					break;
				case ActionContainerStyle.ToolBar:
					containerHtml = GetToolBarContainerHtml(Container.ContainerId);
					break;
			}
			return string.Format(template, containerHtml);
		}
	}
}
