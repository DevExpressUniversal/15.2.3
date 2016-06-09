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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers {
	[ToolboxItem(false)] 
	[PersistChildren(false), ParseChildren(false)]
	[Designer("DevExpress.ExpressApp.Web.Design.QuickAccessNavigationActionContainerDesigner, DevExpress.ExpressApp.Web.Design" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.ComponentModel.Design.IDesigner))]
	public class QuickAccessNavigationActionContainer : Panel, IActionContainer, ITestable {
		private string containerId;
		private ActionItemPaintStyle paintStyle;
		private SingleChoiceAction navigationAction;
		private SingleChoiceActionMenuContainerControl menu;
		private int updateLockCounter;
		private void menu_ItemClick(object sender, MenuItemEventArgs e) {
			navigationAction.DoExecute(((ChoiceActionItem)e.Item.DataItem));
		}
		private void RebuildLinks() {
			IList<ChoiceActionItem> actionItems = GetVisibleActionItems();
			Visible = actionItems.Count != 0;
			menu.Register(actionItems, navigationAction);
		}
		private IList<ChoiceActionItem> GetVisibleActionItems() {
			List<ChoiceActionItem> actionItems = new List<ChoiceActionItem>();
			if(navigationAction.Active) {
				foreach(ChoiceActionItem group in navigationAction.Items) {
					foreach(ChoiceActionItem navigationItem in group.Items) {
						if(navigationItem.Active) {
							IModelNavigationItem modelNavigationItem = navigationItem.Model as IModelNavigationItem;
							if(DesignMode || modelNavigationItem != null && IsItemVisibleInContainer(modelNavigationItem)) {
								actionItems.Add(navigationItem);
							}
						}
					}
				}
			}
			return actionItems;
		}
		protected virtual bool IsItemVisibleInContainer(IModelNavigationItem modelNavigationItem) {
			return modelNavigationItem.QuickAccessItem;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			WebActionContainerHelper.TryRegisterActionContainer(this, new IActionContainer[] { this });
		}
		protected override void OnUnload(EventArgs e) {
			OnControlInitialized(this);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		public QuickAccessNavigationActionContainer() {
			HorizontalAlign = HorizontalAlign.Center;
			paintStyle = ActionItemPaintStyle.Default;
			menu = new SingleChoiceActionMenuContainerControl();
			menu.ID = "MC";
			menu.Style["display"] = "inline-block";
			menu.ItemClick += new EventHandler<MenuItemEventArgs>(menu_ItemClick);
			Controls.Add(menu);
		}
		public virtual void BeginUpdate() {
			updateLockCounter++;
		}
		public virtual void EndUpdate() {
			if(updateLockCounter > 0) {
				updateLockCounter--;
			}
			if(updateLockCounter == 0) {
				RebuildLinks();
			}
		}
		public void Register(ActionBase action) {
			if(action is SingleChoiceAction) {
				navigationAction = (SingleChoiceAction)action;
				if(updateLockCounter == 0) {
					RebuildLinks();
				}
			}
			else {
				throw new UserFriendlyException(string.Format(UserVisibleExceptionLocalizer.GetExceptionMessage(UserVisibleExceptionId.ActionIsNotSingleChoiceAction), action.Id, containerId, typeof(SingleChoiceAction).FullName));
			}
		}
		public override void Dispose() {
			try {
				if(menu != null) {
					menu.ItemClick -= new EventHandler<MenuItemEventArgs>(menu_ItemClick);
					menu.Dispose();
					menu = null;
				}
			}
			finally {
				base.Dispose();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void CreateDesignTimeContent() {
			if(DesignMode) {
				if(!Controls.Contains(menu)) {
					Controls.Add(menu);
				}
				SingleChoiceAction navigationAction = new SingleChoiceAction(null, "Navigation", "");
				ChoiceActionItem group1 = new ChoiceActionItem("Common", "");
				group1.Items.Add(new ChoiceActionItem("Contacts", ""));
				group1.Items.Add(new ChoiceActionItem("Tasks", ""));
				ChoiceActionItem group2 = new ChoiceActionItem("Mail", "");
				group2.Items.Add(new ChoiceActionItem("Inbox", ""));
				group2.Items.Add(new ChoiceActionItem("Drafts", ""));
				group2.Items.Add(new ChoiceActionItem("Outbox", ""));
				group2.Items.Add(new ChoiceActionItem("Sent Items", ""));
				ChoiceActionItem group3 = new ChoiceActionItem("Calendar", "");
				group3.Items.Add(new ChoiceActionItem("Appointments", ""));
				group3.Items.Add(new ChoiceActionItem("Events", ""));
				group3.Items.Add(new ChoiceActionItem("Meetings", ""));
				navigationAction.Items.AddRange(new ChoiceActionItem[] { group1, group2, group3 });
				Register(navigationAction);
			}
		}
		[DefaultValue(null), TypeConverter(typeof(DevExpress.ExpressApp.Core.Design.ContainerIdConverter)), Category("Design")]
		public string ContainerId {
			get { return containerId; }
			set { containerId = value; }
		}
		public ActionItemPaintStyle PaintStyle {
			get { return paintStyle; }
			set {
				paintStyle = value;
				menu.ShowImages = value == ActionItemPaintStyle.CaptionAndImage || value == ActionItemPaintStyle.Image;
			}
		}
		public bool ShowSeparators {
			get { return menu.ShowSeparators; }
			set { menu.ShowSeparators = value; }
		}
		public ReadOnlyCollection<ActionBase> Actions {
			get { return new ReadOnlyCollection<ActionBase>(new ActionBase[] { navigationAction }); }
		}
		#region ITestable Members
		public string TestCaption {
			get { return "QuickNavigation"; } 
		}
		public string ClientId {
			get { return menu.Controls[0].ClientID; }
		}
		public IJScriptTestControl TestControl {
			get { return new NavigationLinksActionContainerJScriptTestControl(); }
		}
		public virtual TestControlType TestControlType {
			get { return TestControlType.Action; }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		#endregion
	}
	[ToolboxItem(false)]
	public class SingleChoiceActionMenuContainerControl : Panel, INamingContainer, IXafCallbackHandler {
		private ASPxMenu menu;
		private bool showImages;
		private bool isPostBackRequired;
		private List<MenuChoiceActionItem> menuActionItems;
		private Dictionary<ChoiceActionItem, DevExpress.Web.MenuItem> actionItemToMenuItemMap;
		private void menu_ItemClick(object source, MenuItemEventArgs e) {
			OnItemClick(e);
		}
		private void OnItemClick(MenuItemEventArgs args) {
			if(ItemClick != null) {
				ItemClick(this, args);
			}
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			ICallbackManagerHolder holder = Page as ICallbackManagerHolder;
			if(holder != null) {
				holder.CallbackManager.RegisterHandler(UniqueID, this);
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			ICallbackManagerHolder holder = Page as ICallbackManagerHolder;
			if(holder != null) {
				menu.ClientSideEvents.ItemClick = string.Format(
@"function(s, e) {{
    e.processOnServer = false;
    if(typeof xaf == 'undefined' || !xaf.ConfirmUnsavedChangedController || xaf.ConfirmUnsavedChangedController.CanProcessCallbackForNavigation(s, e)) {{
        {0}
    }}
}}", holder.CallbackManager.GetScript(UniqueID, "e.item.name", String.Empty, isPostBackRequired));
			}
		}
		public SingleChoiceActionMenuContainerControl() {
			actionItemToMenuItemMap = new Dictionary<ChoiceActionItem, DevExpress.Web.MenuItem>();
			menuActionItems = new List<MenuChoiceActionItem>();
			showImages = true;
			menu = RenderHelper.CreateASPxMenu();
			menu.ID = "M";
			menu.SkinID = "MenuLinks";
			menu.EnableClientSideAPI = true;
			menu.ItemClick += new MenuItemEventHandler(menu_ItemClick);
			Controls.Add(menu);
		}
		public void Register(IList<ChoiceActionItem> items, ChoiceActionBase navigationAction) {
			isPostBackRequired = navigationAction.Model.GetValue<bool>("IsPostBackRequired");
			menu.Items.Clear();
			foreach(ChoiceActionItem item in items) {
				MenuChoiceActionItem menuActionItem = new MenuChoiceActionItem(item, navigationAction, showImages);
				menuActionItems.Add(menuActionItem); 
				DevExpress.Web.MenuItem menuItem = menuActionItem.MenuItem;
				menuItem.DataItem = item;
				menu.Items.Add(menuItem);
				actionItemToMenuItemMap.Add(item, menuItem);
			}
		}
		public DevExpress.Web.MenuItem GetMenuItem(ChoiceActionItem actionItem) {
			return actionItemToMenuItemMap.ContainsKey(actionItem) ? actionItemToMenuItemMap[actionItem] : null;
		}
		public override void Dispose() {
			if(actionItemToMenuItemMap != null) {
				actionItemToMenuItemMap.Clear();
				actionItemToMenuItemMap = null;
			}
			if(menuActionItems != null) {
				foreach(MenuChoiceActionItem item in menuActionItems) {
					item.Dispose();
				}
				menuActionItems.Clear();
				menuActionItems = null;
			}
			if(menu != null) {
				menu.Items.Clear();
				menu.ItemClick -= new MenuItemEventHandler(menu_ItemClick);
				menu = null;
			}
			base.Dispose();
		}
		public bool ShowImages {
			get { return showImages; }
			set { showImages = value; }
		}
		public bool ShowSeparators {
			get { return menu.AutoSeparators != AutoSeparatorMode.None; }
			set { menu.AutoSeparators = value ? AutoSeparatorMode.RootOnly : AutoSeparatorMode.None; }
		}
		public DevExpress.Web.MenuItemCollection Items {
			get { return menu.Items; }
		}
		public event EventHandler<MenuItemEventArgs> ItemClick;
		#region IXafCallbackHandler Members
		public void ProcessAction(string parameter) {
			foreach(DevExpress.Web.MenuItem item in menu.Items) {
				if(item.Name == parameter) {
					OnItemClick(new MenuItemEventArgs(item));
				}
			}
		}
		#endregion
	}
	#region Obsolete 15.2
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class LinkActionControl : HtmlAnchor{
		private string imageUrl;
		private Unit imageWidth;
		private Unit imageHeight;
		protected override void RenderBeginTag(HtmlTextWriter writer) {
			Attributes["id"] = ClientID;
			base.RenderBeginTag(writer);
			if(!string.IsNullOrEmpty(imageUrl)) {
				System.Web.UI.WebControls.Image image = new System.Web.UI.WebControls.Image();
				image.ImageUrl = ImageUrl;
				image.Width = ImageWidth;
				image.Height = ImageHeight;
				image.RenderControl(writer);
			}
		}
		public LinkActionControl() { }
		public Unit ImageWidth {
			get { return imageWidth; }
			set { imageWidth = value; }
		}
		public Unit ImageHeight {
			get { return imageHeight; }
			set { imageHeight = value; }
		}
		public string ImageUrl {
			get { return imageUrl; }
			set { imageUrl = value; }
		}
		public string Text {
			get { return InnerText; }
			set { InnerText = value; }
		}
	}
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class LinkChoiceActionItem : ChoiceActionItemWrapper {
		private LinkActionControl linkActionControl;
		private SingleChoiceAction action;
		private void linkActionControl_ServerClick(object sender, EventArgs e) {
			OnLinkChoiceActionItemExecuting();
			action.DoExecute(ActionItem);
		}
		protected virtual void OnLinkChoiceActionItemExecuting() {
			if(LinkChoiceActionItemExecuting != null) {
				LinkChoiceActionItemExecuting(this, new LinkChoiceActionItemExecutingEventArgs(ActionItem));
			}
		}
		public override void SetImageName(string imageName) {
			if(ShowImage) {
				ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
				if(!imageInfo.IsEmpty) {
					linkActionControl.ImageUrl = imageInfo.ImageUrl;
					linkActionControl.ImageWidth = imageInfo.Width;
					linkActionControl.ImageHeight = imageInfo.Height;
				}
			}
			else {
				linkActionControl.ImageUrl = null;
				linkActionControl.ImageWidth = Unit.Empty;
				linkActionControl.ImageHeight = Unit.Empty;
			}
		}
		public override void SetCaption(string caption) {
			linkActionControl.Text = caption;
		}
		public override void SetData(object data) { }
		public override void SetShortcut(string shortcutString) { }
		public override void SetEnabled(bool enabled) {
			if(linkActionControl != null) {
				linkActionControl.ServerClick -= new EventHandler(linkActionControl_ServerClick);
			}
			linkActionControl.Disabled = !(enabled && action.Enabled);
			if(!linkActionControl.Disabled) {
				linkActionControl.ServerClick += new EventHandler(linkActionControl_ServerClick);
			}
		}
		public override void SetVisible(bool visible) {
			linkActionControl.Visible = visible;
		}
		public override void SetToolTip(string toolTip) {
			RenderHelper.SetToolTip(linkActionControl, toolTip);
		}
		public LinkChoiceActionItem(SingleChoiceAction action, ChoiceActionItem actionItem)
			: base(actionItem, action) {
			this.action = action;
			this.linkActionControl = new LinkActionControl();
			ShowImage = false;
			SyncronizeWithItem();
		}
		public override void Dispose() {
			base.Dispose();
			if(linkActionControl != null) {
				linkActionControl.ServerClick -= new EventHandler(linkActionControl_ServerClick);
				linkActionControl.Dispose();
				linkActionControl = null;
			}
			action = null;
		}
		public LinkActionControl Control {
			get { return linkActionControl; }
		}
		public event EventHandler<LinkChoiceActionItemExecutingEventArgs> LinkChoiceActionItemExecuting;
	}
	[Obsolete(ObsoleteMessages.TypeIsNotUsedAnymore), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class LinkChoiceActionItemExecutingEventArgs : EventArgs {
		public ChoiceActionItem item;
		public LinkChoiceActionItemExecutingEventArgs(ChoiceActionItem item) {
			this.item = item;
		}
		public ChoiceActionItem Item {
			get { return item; }
		}
	}
	#endregion
}
