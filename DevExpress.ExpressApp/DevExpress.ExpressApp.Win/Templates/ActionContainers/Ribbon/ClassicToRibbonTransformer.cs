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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.Win.Templates.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
	public class TransformingEventArgs : HandledEventArgs {
		private XafRibbonControl ribbon;
		private XtraForm sourceForm;
		public TransformingEventArgs(XafRibbonControl ribbon, XtraForm sourceForm) {
			this.ribbon = ribbon;
			this.sourceForm = sourceForm;
		}
		public XafRibbonControl Ribbon {
			get { return ribbon; }
		}
		public XtraForm SourceForm {
			get { return sourceForm; }
		}
	}
	public class ResolveBarItemTargetPageEventArgs : EventArgs {
		private BarItemLink itemLink;
		private string targetPageCaption;
		public ResolveBarItemTargetPageEventArgs(BarItemLink itemLink, string targetPageCaption) {
			this.itemLink = itemLink;
			this.targetPageCaption = targetPageCaption;
		}
		public BarItemLink ItemLink {
			get { return itemLink; }
		}
		public string TargetPageCaption {
			get { return targetPageCaption; }
			set { targetPageCaption = value; }
		}
	}
	public class ResolveMainMenuItemTargetPageEventArgs : EventArgs {
		private BarSubItem menuItem;
		private string targetPageCaption;
		public ResolveMainMenuItemTargetPageEventArgs(BarSubItem menuItem) {
			this.menuItem = menuItem;
			if(menuItem is MainMenuItem) {
				this.targetPageCaption = ((MainMenuItem)menuItem).VisibleInRibbon ? menuItem.Caption : "";
			}
			else {
				this.targetPageCaption = menuItem.Caption;
			}
		}
		public BarSubItem MenuItem {
			get { return menuItem; }
		}
		public string TargetPageCaption {
			get { return targetPageCaption; }
			set { targetPageCaption = value; }
		}
	}
	public class CustomTransformBarLinkContainerEventArgs : HandledEventArgs {
		private RibbonPage targetPage;
		private XafBarLinkContainerItem barLinkContainer;
		public CustomTransformBarLinkContainerEventArgs(RibbonPage targetPage, XafBarLinkContainerItem barLinkContainer) {
			this.targetPage = targetPage;
			this.barLinkContainer = barLinkContainer;
		}
		public RibbonPage TargetPage {
			get { return targetPage; }
			set { targetPage = value; }
		}
		public XafBarLinkContainerItem BarLinkContainer {
			get { return barLinkContainer; }
		}
	}
	public class BarItemAddingEventArgs : EventArgs {
		private BarItem item;
		private BarItemLink link;
		private ActionBase action;
		private RibbonPageGroup group;
		public BarItemAddingEventArgs(BarItemLink link, RibbonPageGroup group, ActionBase action) {
			this.link = link;
			this.item = link.Item;
			this.action = action;
			this.group = group;
		}
		public BarItem Item {
			get { return item; }
			set { item = value; }
		}
		public BarItemLink Link {
			get { return link; }
		}
		public ActionBase Action {
			get { return action; }
		}
		public RibbonPageGroup Group {
			get { return group; }
			set { group = value; }
		}
	}
	public class BarItemAddedEventArgs : EventArgs {
		private BarItem item;
		private BarItemLink link;
		private ActionBase action;
		private RibbonPageGroup group;
		public BarItemAddedEventArgs(BarItemLink link, RibbonPageGroup group, ActionBase action) {
			this.link = link;
			this.item = link.Item;
			this.action = action;
			this.group = group;
		}
		public BarItem Item {
			get { return item; }
		}
		public BarItemLink Link {
			get { return link; }
		}
		public ActionBase Action {
			get { return action; }
		}
		public RibbonPageGroup Group {
			get { return group; }
		}
	}
	public interface IClassicToRibbonTransformerHolder {
		ClassicToRibbonTransformer RibbonTransformer { get; }
	}
	public interface ISupportClassicToRibbonTransform : IBarManagerHolder {
		RibbonFormStyle FormStyle { get; set; }
		IModelOptionsRibbon RibbonOptions { get; }
	}
	public interface ITargetRibbonElement {
		Color TargetPageCategoryColor { get; }
		string TargetPageCaption { set; get; }
		string TargetPageCategoryCaption { set; get; }
	}
	public class ClassicToRibbonTransformer : RibbonConverter, IDisposable {
		private string defaultActionsGroupCaption = "Actions";
		private string defaultHomePageCaption = "Home";
		private XtraForm form;
		private XafRibbonControl ribbon;
		private RibbonPageGroup CreateNonActionsGroup() {
			DefaultActionsGroupCaption = CaptionHelper.GetLocalizedText("Ribbon", "ActionsPageGroup", "Actions");
			RibbonPageGroup defaultPageGroup = new RibbonPageGroup(DefaultActionsGroupCaption);
			defaultPageGroup.ShowCaptionButton = false;
			defaultPageGroup.Name = DefaultActionsGroupCaption;
			return defaultPageGroup;
		}
		private RibbonPageGroup AddNonActionsGroup(RibbonPage targetPage) {
			RibbonPageGroup defaultPageGroup = CreateNonActionsGroup();
			targetPage.Groups.Add(defaultPageGroup);
			return defaultPageGroup;
		}
		private RibbonPageCategory AddPageCategory(ITargetRibbonElement ribbonElement) {
			RibbonPageCategory category = new RibbonPageCategory(ribbonElement.TargetPageCategoryCaption, ribbonElement.TargetPageCategoryColor);
			category.Name = ribbonElement.TargetPageCategoryCaption;
			Ribbon.PageCategories.Add(category);
			return category;
		}
		private RibbonPage CreateRibbonDefaultPage() {
			DefaultHomePageCaption = CaptionHelper.GetLocalizedText("Ribbon", "HomePage", "Home");
			return InsertPage(DefaultHomePageCaption, 0);
		}
		private void ribbon_UnMerge(object sender, RibbonMergeEventArgs e) {
			OnUnMergeRibbon(e);
		}
		private void ribbon_Merge(object sender, RibbonMergeEventArgs e) {
			OnMergeRibbon(e);
		}
		private void ribbon_MinimizedChanged(object sender, EventArgs e) {
			((ISupportClassicToRibbonTransform)form).RibbonOptions.MinimizeRibbon = ribbon.Minimized;
		}
		private void ribbon_ToolbarCustomized(object sender, ToolbarCustomizedEventArgs e) {
			BarActionItemsFactory factory = BarActionItemsFactoryProvider.GetBarActionItemsFactory(Ribbon.Manager);
			if(factory != null) {
				ActionBase action = factory.FindActionByItem(e.Link.Item);
				if(action != null) {
					action.QuickAccess = e.Action == CollectionChangeAction.Add;
				}
			}
		}
		private bool GroupContainsItem(RibbonPageGroup group, BarItem barItem) {
			bool result = false;
			foreach(BarItemLink link in FindBarItemLinksByCaption(group.ItemLinks, barItem.Caption)) {
				result = link.Item == barItem;
				break;
			}
			return result;
		}
		private IEnumerable<BarItemLink> FindBarItemLinksByCaption(BarItemLinkCollection links, string caption) {
			foreach(BarItemLink link in links) {
				if(link.Caption == caption) {
					yield return link;
				}
			}
		}
		protected RibbonPage InsertPage(string pageName, int index) {
			RibbonPage page = new RibbonPage(pageName);
			page.Name = pageName;
			Ribbon.Pages.Insert(index, page);
			return page;
		}
		protected RibbonPage AddPage(string pageName) {
			RibbonPage page = new RibbonPage(pageName);
			page.Name = pageName;
			Ribbon.Pages.Add(page);
			return page;
		}
		protected void SortActionContainerLinks(BarItemLinkCollection links) {
			List<ActionContainerBarItem> list = new List<ActionContainerBarItem>();
			foreach(BarItemLink link in links) {
				list.Add((ActionContainerBarItem)link.Item);
			}
			list.Sort(new ApplicationMenuItemComparer());
			links.Clear();
			foreach(ActionContainerBarItem item in list) {
				links.Add(item);
			}
		}
		private BarItem FindInvalidBarItem(BarItemLinkCollection links) {
			foreach(BarItemLink link in links) {
				if(!(link.Item is ActionContainerBarItem)) {
					return link.Item;
				}
			}
			return null;
		}
		protected void AddBarItemToPageGroup(BarItemLink childLink, RibbonPageGroup group, ActionBase action) {
			BarItemAddingEventArgs addingArgs = new BarItemAddingEventArgs(childLink, group, action);
			OnBarItemAdding(addingArgs);
			if(addingArgs.Group != null && !GroupContainsItem(addingArgs.Group, addingArgs.Item)) {
				BarItemLink link = addingArgs.Group.ItemLinks.Add(addingArgs.Item, childLink.BeginGroup);
				BarItemAddedEventArgs addedArgs = new BarItemAddedEventArgs(link, addingArgs.Group, action);
				OnBarItemAdded(addedArgs);
			}
		}
		protected virtual void OnBarItemAdding(BarItemAddingEventArgs args) {
			if(BarItemAdding != null) {
				BarItemAdding(this, args);
			}
		}
		protected virtual void OnBarItemAdded(BarItemAddedEventArgs args) {
			if(BarItemAdded != null) {
				BarItemAdded(this, args);
			}
		}
		protected virtual void OnUnMergeRibbon(RibbonMergeEventArgs e) {
			if(ribbon.StatusBar != null && e.MergedChild.StatusBar != null) {
				ribbon.StatusBar.MergeStatusBar(e.MergedChild.StatusBar);
			}
			PopupMenu popupMenu = GetApplicationMenuControl();
			if(popupMenu != null) {
				popupMenu.UnMerge();
			}
		}
		protected virtual void OnMergeRibbon(RibbonMergeEventArgs e) {
			if(ribbon.StatusBar != null) {
				ribbon.StatusBar.UnMergeStatusBar();
			}
			PopupMenu childPopupMenu = e.MergedChild.ApplicationButtonDropDownControl as PopupMenu;
			PopupMenu popupMenu = GetApplicationMenuControl();
			if(popupMenu != null && childPopupMenu != null) {
				popupMenu.Merge(childPopupMenu);
				BarItem invalidBarItem = FindInvalidBarItem(popupMenu.ItemLinks);
				if(invalidBarItem != null) {
					throw new InvalidOperationException(String.Format(
						 @"BarItem of the ""{0}"" type is expected, but was ""{1}"". It seems that the new template type ({2}) is used for the Detail Form while for the Main Form a template of the old type ({3}) is used. To avoid this error, use templates of the same types only. Refer to http://documentation.devexpress.com/#Xaf/CustomDocument2618 for more details.",
						 typeof(ActionContainerBarItem), invalidBarItem.GetType(), e.MergedChild.Parent.GetType(), e.MergeOwner.Parent.GetType()));
				}
				SortActionContainerLinks(popupMenu.ItemLinks);
			}
		}
		protected virtual void OnCustomTransformBarLinkContainer(CustomTransformBarLinkContainerEventArgs e) {
			if(CustomTransformBarLinkContainer != null) {
				CustomTransformBarLinkContainer(this, e);
			}
		}
		protected virtual void OnResolveBarItemTargetPage(ResolveBarItemTargetPageEventArgs e) {
			if(ResolveBarItemTargetPage != null) {
				ResolveBarItemTargetPage(this, e);
			}
		}
		protected virtual void OnResolveMainMenuItemTargetPage(ResolveMainMenuItemTargetPageEventArgs e) {
			if(ResolveMainMenuItemTargetPage != null) {
				ResolveMainMenuItemTargetPage(this, e);
			}
		}
		protected virtual void AddBarLinkContainerToPageAsPageGroup(RibbonPage targetPage, XafBarLinkContainerItem barLinkContainer) {
			CustomTransformBarLinkContainerEventArgs args = new CustomTransformBarLinkContainerEventArgs(targetPage, barLinkContainer);
			OnCustomTransformBarLinkContainer(args);
			targetPage = args.TargetPage;
			if(!args.Handled && targetPage != null) {
				string groupName = string.IsNullOrEmpty(barLinkContainer.TargetPageGroupCaption) ? barLinkContainer.Caption : barLinkContainer.TargetPageGroupCaption;
				ActionContainersRibbonPageGroup group = targetPage.GetGroupByName(groupName) as ActionContainersRibbonPageGroup;
				if(group == null) {
					group = new ActionContainersRibbonPageGroup(groupName);
					group.MergeOrder = targetPage.Groups.Count;
					targetPage.Groups.Add(group);
				}
				group.RegisterBarLinkContainer(barLinkContainer);
				ActionContainerBarItem actionContainer = barLinkContainer as ActionContainerBarItem;
				foreach(BarItemLink childLink in barLinkContainer.ItemLinks) {
					ActionBase action = actionContainer != null ? actionContainer.FindActionByItem(childLink.Item) : null;
					AddBarItemToPageGroup(childLink, group, action);
				}
			}
		}
		protected virtual void AddBarLinkContainerToQuickAccess(XafBarLinkContainerItem barLinkContainer) {
			ActionContainerBarItem actionContainer = barLinkContainer as ActionContainerBarItem;
			if(actionContainer != null) {
				foreach(BarItemLink childLink in barLinkContainer.ItemLinks) {
					ActionBase action = actionContainer.FindActionByItem(childLink.Item);
					if(action != null && action.QuickAccess && FindBarItemLinkByCaption(ribbon.Toolbar.ItemLinks, childLink.Item.Caption) == null) {
						ribbon.Toolbar.ItemLinks.Add(childLink.Item);
					}
				}
			}
		}
		protected virtual void TransformBarItemLinks(BarItemLinkCollection links, RibbonPage targetPage) {
			foreach(BarItemLink childLink in links) {
				string targetPageCaption = targetPage.Text;
				if(childLink.Item is XafBarLinkContainerItem) {
					XafBarLinkContainerItem barLinkContainer = (XafBarLinkContainerItem)childLink.Item;
					if(PageHasContainer(GetRibbonDefaultPage(), barLinkContainer)) {
						targetPageCaption = null;
					}
					else if(!string.IsNullOrEmpty(barLinkContainer.TargetPageCaption)) {
						targetPageCaption = barLinkContainer.TargetPageCaption;
					}
				}
				ResolveBarItemTargetPageEventArgs args = new ResolveBarItemTargetPageEventArgs(childLink, targetPageCaption);
				OnResolveBarItemTargetPage(args);
				if(!string.IsNullOrEmpty(args.TargetPageCaption)) {
					RibbonPage targetRibbonPage = FindPageByName(args.TargetPageCaption);
					if(targetRibbonPage == null) {
						targetRibbonPage = AddPage(args.TargetPageCaption);
					}
					if(childLink.Item is XafBarLinkContainerItem) {
						AddBarLinkContainerToPageAsPageGroup(targetRibbonPage, (XafBarLinkContainerItem)childLink.Item);
					}
					else {
						RibbonPageGroup group = GetNonActionsGroup(targetRibbonPage);
						AddBarItemToPageGroup(childLink, group, null);
					}
				}
			}
			foreach(BarItemLink childLink in links) {
				if(childLink.Item is XafBarLinkContainerItem) {
					AddBarLinkContainerToQuickAccess((XafBarLinkContainerItem)childLink.Item);
				}
			}
		}
		private bool PageHasContainer(RibbonPage ribbonPage, XafBarLinkContainerItem barLinkContainer) {
			bool result = false;
			foreach(RibbonPageGroup group in ribbonPage.Groups) {
				ActionContainersRibbonPageGroup acPageGroup = group as ActionContainersRibbonPageGroup;
				if(acPageGroup != null) {
					result = acPageGroup.BarLinkContainers.Contains(barLinkContainer);
					if(result) {
						break;
					}
				}
			}
			return result;
		}
		protected virtual RibbonPageGroup GetNonActionsGroup(RibbonPage targetPage) {
			RibbonPageGroup result = targetPage.GetGroupByName(DefaultActionsGroupCaption);
			if(result == null) {
				result = AddNonActionsGroup(targetPage);
			}
			return result;
		}
		protected virtual RibbonPage GetRibbonDefaultPage() {
			RibbonPage defaultPage = FindPageByName(DefaultHomePageCaption);
			if(defaultPage == null) {
				defaultPage = CreateRibbonDefaultPage();
			}
			return defaultPage;
		}
		protected virtual RibbonPage GetTargetPage(ITargetRibbonElement ribbonElement) {
			string targetPageCaption = ribbonElement.TargetPageCaption;
			if(string.IsNullOrEmpty(targetPageCaption) && !string.IsNullOrEmpty(ribbonElement.TargetPageCategoryCaption)) {
				targetPageCaption = ribbonElement.TargetPageCategoryCaption;
			}
			RibbonPage targetPage;
			if(string.IsNullOrEmpty(targetPageCaption)) {
				targetPage = GetRibbonDefaultPage();
			}
			else {
				targetPage = FindPageByName(targetPageCaption);
				if(targetPage == null) {
					targetPage = AddPage(targetPageCaption);
				}
				if(!string.IsNullOrEmpty(ribbonElement.TargetPageCategoryCaption) && targetPage.Category.Name != ribbonElement.TargetPageCategoryCaption) {
					RibbonPageCategory category = GetTargetPageCategory(ribbonElement);
					category.Pages.Add(targetPage);
				}
			}
			return targetPage;
		}
		protected virtual RibbonPageCategory GetTargetPageCategory(ITargetRibbonElement ribbonElement) {
			RibbonPageCategory result = FindPageCategoryByName(ribbonElement.TargetPageCategoryCaption);
			if(result == null) {
				result = AddPageCategory(ribbonElement);
			}
			return result;
		}
		protected virtual void TransformMainMenu(Bar mainMenuBar) {
			foreach(BarItemLink itemLink in mainMenuBar.ItemLinks) {
				if(itemLink.Item is BarSubItem) {
					BarSubItem barSubItem = (BarSubItem)itemLink.Item;
					ResolveMainMenuItemTargetPageEventArgs args = new ResolveMainMenuItemTargetPageEventArgs(barSubItem);
					OnResolveMainMenuItemTargetPage(args);
					if(!string.IsNullOrEmpty(args.TargetPageCaption)) {
						RibbonPage page = FindPageByName(args.TargetPageCaption);
						if(page == null) {
							page = AddPage(args.TargetPageCaption);
						}
						if(barSubItem.ItemLinks.Count > 0) {
							TransformBarItemLinks(barSubItem.ItemLinks, page);
						}
					}
					foreach(BarItemLink childLink in barSubItem.ItemLinks) {
						if(childLink.Item is XafBarLinkContainerItem) {
							AddBarLinkContainerToQuickAccess((XafBarLinkContainerItem)childLink.Item);
						}
					}
				}
			}
		}
		protected virtual void TransformBar(XafBar bar) {
			if(bar.ItemLinks.Count > 0) {
				TransformBarItemLinks(bar.ItemLinks, GetTargetPage(bar));
			}
		}
		protected virtual void TransformStatusBar(Bar statusBar) {
			RibbonStatusBar newBar = new RibbonStatusBar(ribbon);
			newBar.Dock = DockStyle.Bottom;
			foreach(BarItemLink itemLink in statusBar.ItemLinks) {
				BarItemLink newItemLink = newBar.ItemLinks.Add(itemLink.Item);
				newItemLink.BeginGroup = itemLink.BeginGroup;
			}
		}
		protected virtual XafRibbonControl CreateRibbonControl() {
			XafRibbonControl ribbonControl = new XafRibbonControl();
			ribbonControl.RibbonStyle = RibbonControlStyle.Office2010;
			return ribbonControl;
		}
		protected virtual PopupMenu GetApplicationMenuControl() {
			PopupMenu applicationMenu = (PopupMenu)ribbon.ApplicationButtonDropDownControl;
			if(applicationMenu == null) {
				applicationMenu = new ApplicationMenu();
				applicationMenu.Manager = ribbon.Manager;
				applicationMenu.MenuDrawMode = MenuDrawMode.LargeImagesText;
				ribbon.ApplicationButtonDropDownControl = applicationMenu;
			}
			return applicationMenu;
		}
		protected virtual void CreateApplicationMenu() {
			PopupMenu applicationMenu = null;
			foreach(BarItem item in ribbon.Manager.Items) {
				ActionContainerBarItem acItem = item as ActionContainerBarItem;
				if(acItem != null && acItem.ApplicationMenuItem) {
					applicationMenu = GetApplicationMenuControl();
					applicationMenu.ItemLinks.Add(item);
				}
			}
			if(applicationMenu != null) {
				SortActionContainerLinks(applicationMenu.ItemLinks);
			}
		}
		protected override bool CanConvert(BarItem item) {
			if(item is XafBarLinkContainerItem) {
				return true;
			}
			else {
				return base.CanConvert(item);
			}
		}
		public RibbonPage FindPageByName(string pageName) {
			foreach(RibbonPage page in ribbon.TotalPageCategory.Pages) {
				if(page.Name == pageName) {
					return page;
				}
			}
			return null;
		}
		public RibbonPageCategory FindPageCategoryByName(string categoryName) {
			foreach(RibbonPageCategory pageCategory in ribbon.PageCategories) {
				if(pageCategory.Name == categoryName) {
					return pageCategory;
				}
			}
			return null;
		}
		public BarItemLink FindBarItemLinkByCaption(BarItemLinkCollection links, string caption) {
			foreach(BarItemLink link in links) {
				if(link.Caption == caption) {
					return link;
				}
			}
			return null;
		}
		public virtual void Transform() {
			Transform(true);
		}
		public virtual void Transform(bool mergeRibbon) {
			ISupportClassicToRibbonTransform transformable = (ISupportClassicToRibbonTransform)form;
			transformable.BarManager.ForceInitialize();
			if(transformable.FormStyle == RibbonFormStyle.Ribbon) {
				if(form is RibbonForm && ((RibbonForm)form).Ribbon != null) {
					((RibbonForm)form).Ribbon = null;
				}
				ribbon = CreateRibbonControl();
				if(transformable.RibbonOptions != null) {
					ribbon.RibbonStyle = transformable.RibbonOptions.RibbonControlStyle;
					ribbon.Minimized = transformable.RibbonOptions.MinimizeRibbon;
				}
				IXafDocumentsHostWindow documentsHostWindow = Form as IXafDocumentsHostWindow;
				if(documentsHostWindow == null || documentsHostWindow.UIType != UIType.StandardMDI) {
					ribbon.MdiMergeStyle = RibbonMdiMergeStyle.Always;
				}
				ribbon.Size = new System.Drawing.Size(442, 141);
				ribbon.Manager.AllowMergeInvisibleLinks = true;
				if(mergeRibbon) {
					ribbon.Merge += new RibbonMergeEventHandler(ribbon_Merge);
					ribbon.UnMerge += new RibbonMergeEventHandler(ribbon_UnMerge);
				}
				ribbon.MinimizedChanged += new EventHandler(ribbon_MinimizedChanged);
				ribbon.ToolbarCustomized += new EventHandler<ToolbarCustomizedEventArgs>(ribbon_ToolbarCustomized);
				ConvertRefProperties(transformable.BarManager, ribbon);
				ConvertItems(transformable.BarManager, ribbon);
				TransformingEventArgs transformingEventArgs = new TransformingEventArgs(ribbon, form);
				OnTransforming(transformingEventArgs);
				if(!transformingEventArgs.Handled) {
					Bar mainMenu = null;
					foreach(Bar bar in transformable.BarManager.Bars) {
						if(bar.IsStatusBar) {
							TransformStatusBar(bar);
						}
						else {
							if(bar.IsMainMenu) {
								mainMenu = bar;
							}
							else {
								TransformBar((XafBar)bar);
							}
						}
					}
					if(mainMenu != null) {
						TransformMainMenu(mainMenu);
					}
				}
				CreateApplicationMenu();
				BarAndDockingController controller = transformable.BarManager.Controller;
				transformable.BarManager.Controller = null;
				transformable.BarManager.Form = null;
				BarManager oldBarManager = transformable.BarManager;
				BarActionItemsFactoryProvider.ReplaceManager(oldBarManager, ribbon.Manager);
				ribbon.Controller = controller;
				CleanUp(oldBarManager);
				if(form is IDocumentsHostWindow) {
					((IDocumentsHostWindow)form).DocumentManager.MenuManager = ribbon.Manager;
				}
				ribbon.Dock = DockStyle.Top;
				form.Controls.Add(ribbon);
				RibbonForm ribbonForm = form as RibbonForm;
				if(ribbonForm != null) {
					RibbonStatusBar oldStatusBar = ribbonForm.StatusBar;
					if(oldStatusBar != null) {
						form.Controls.Remove(oldStatusBar);
					}
				}
				form.Controls.Add(ribbon.StatusBar);
				ribbon.SendToBack();
				ribbon.SelectedPage = ribbon.Pages[DefaultHomePageCaption];
				ribbon.ForceInitialize();
				OnTransformed();
			}
		}
		protected virtual void OnTransforming(TransformingEventArgs e) {
			if(Transforming != null) {
				Transforming(this, e);
			}
		}
		protected virtual void OnTransformed() {
			if(Transformed != null) {
				Transformed(this, EventArgs.Empty);
			}
		}
		public ClassicToRibbonTransformer(XtraForm form) {
			if(!(form is ISupportClassicToRibbonTransform)) {
				throw new InvalidOperationException("The form being transformed must implement the 'ISupportClassicToRibbonTransform' interface.");
			}
			this.form = form;
		}
		public XafRibbonControl Ribbon {
			get { return ribbon; }
		}
		public XtraForm Form {
			get { return form; }
		}
		public string DefaultActionsGroupCaption {
			get { return defaultActionsGroupCaption; }
			set { defaultActionsGroupCaption = value; }
		}
		public string DefaultHomePageCaption {
			get { return defaultHomePageCaption; }
			set { defaultHomePageCaption = value; }
		}
		public event EventHandler<ResolveBarItemTargetPageEventArgs> ResolveBarItemTargetPage;
		public event EventHandler<ResolveMainMenuItemTargetPageEventArgs> ResolveMainMenuItemTargetPage;
		public event EventHandler<CustomTransformBarLinkContainerEventArgs> CustomTransformBarLinkContainer;
		public event EventHandler<TransformingEventArgs> Transforming;
		public event EventHandler<EventArgs> Transformed;
		public event EventHandler<BarItemAddingEventArgs> BarItemAdding;
		public event EventHandler<BarItemAddedEventArgs> BarItemAdded;
		#region IDisposable Members
		public void Dispose() {
			form = null;
			if(ribbon != null) {
				ribbon.Merge -= new RibbonMergeEventHandler(ribbon_Merge);
				ribbon.UnMerge -= new RibbonMergeEventHandler(ribbon_UnMerge);
				ribbon.MinimizedChanged -= new EventHandler(ribbon_MinimizedChanged);
				ribbon.ToolbarCustomized -= new EventHandler<ToolbarCustomizedEventArgs>(ribbon_ToolbarCustomized);
				ribbon = null;
			}
		}
		#endregion
	}
}
