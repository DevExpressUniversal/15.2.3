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

using System;
using System.Windows;
using System.Collections.Specialized;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Utils;
using System.Windows.Markup;
using System.Collections;
using DevExpress.Mvvm.Native;
using System.Linq;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Ribbon {
	public abstract class RibbonControllerBase: BarManagerController {
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
		}
	}
	public class RibbonController: RibbonControllerBase {
		public override void Execute() {
			LogBase.Add(this, this, "Execute");
			base.Execute();
		}
	}	
	public class RibbonActionContainer: BarManagerActionContainer { }
	public class RibbonControllerActionBase: DependencyObject, IBarManagerControllerAction {
		public RibbonControl Ribbon {
			get { return RibbonControl.GetRibbon(this) ?? (((IControllerAction)this).Container as DependencyObject).With(RibbonControl.GetRibbon) ?? ((IControllerAction)this).Container.AssociatedObject.With(RibbonControl.GetRibbon); }
		}
		IActionContainer IControllerAction.Container { get; set; }
		void IControllerAction.Execute(DependencyObject context) {
			ExecuteCore();
		}
		protected virtual void ExecuteCore() {
		}
		object IBarManagerControllerAction.GetObject() {
			return GetObjectCore();
		}
		protected virtual object GetObjectCore() {
			return null;
		}
		protected RibbonPage FindRibbonPage(string name) {
			foreach(RibbonPageCategoryBase category in Ribbon.Categories) {
				RibbonPage page = category.Pages[name];
				if(page != null)
					return page;
			}
			return null;
		}
		protected RibbonPageGroup FindRibbonPageGroup(string name) {
			foreach(RibbonPageCategoryBase category in Ribbon.Categories)
				foreach(RibbonPage page in category.Pages) {
					RibbonPageGroup group = page.Groups[name];
					if(group != null)
						return group;
				}
			return null;
		}
		protected virtual BarItemLinkBase AddItemAndCreateLink(BarItem item) {
			if(!Ribbon.Manager.Items.Contains(item))
				Ribbon.Manager.Items.Add(item);
			return CreateLink(item);
		}
		protected virtual BarItemLinkBase CreateLink(BarItem item) {
			BarItemLinkBase link = item.CreateLink();
			link.Name = item.Name + "Link";
			return link;
		}
	}	
	public class TemplatedRibbonController: RibbonController {
		public static readonly DependencyProperty TemplateProperty = DependencyPropertyManager.Register("Template", typeof(DataTemplate), typeof(TemplatedRibbonController), new FrameworkPropertyMetadata(new PropertyChangedCallback(TemplatePropertyChanged)));
		static void TemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
		public DataTemplate Template {
			get { return (DataTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		protected override BarManagerActionContainer CreateActionContainer() {
			if(Template == null)
				throw new ArgumentNullException("TemplateProperty");
			BarManagerActionContainer res = (BarManagerActionContainer)Template.LoadContent();
			res.Controller = this;
			return res;
		}
		public void GetActionController() { }
		public override void Execute() {
			if(ActionContainer == null)
				throw new ArgumentNullException("ActionContainer");
			ActionContainer.Controller = this;
			ActionContainer.Execute();
		}
	}
	[ContentProperty("Category")]
	public class AddRibbonPageCategoryAction: RibbonControllerActionBase {
		public static readonly DependencyProperty CategoryProperty = DependencyPropertyManager.Register("Category", typeof(RibbonPageCategoryBase), typeof(AddRibbonPageCategoryAction), new FrameworkPropertyMetadata(null));
		public RibbonPageCategoryBase Category {
			get { return (RibbonPageCategoryBase)GetValue(CategoryProperty); }
			set { SetValue(CategoryProperty, value); }
		}
		protected override void ExecuteCore() {
			if(Category != null)
				Ribbon.Categories.Add(Category);
		}
	}
	public class InsertRibbonPageCategoryAction: AddRibbonPageCategoryAction {
		public static readonly DependencyProperty IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(InsertRibbonPageCategoryAction), new FrameworkPropertyMetadata(-1));
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		protected override void ExecuteCore() {
			if(Index < 0 || Category == null)
				return;
			Ribbon.Categories.Insert(Index, Category);
		}
	}
	public class RemoveRibbonPageCategoryAction: RibbonControllerActionBase {
		public static readonly DependencyProperty CategoryNameProperty = DependencyPropertyManager.Register("CategoryName", typeof(string), typeof(RemoveRibbonPageCategoryAction), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(RemoveRibbonPageCategoryAction), new FrameworkPropertyMetadata(-1));
		public string CategoryName {
			get { return (string)GetValue(CategoryNameProperty); }
			set { SetValue(CategoryNameProperty, value); }
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		protected override void ExecuteCore() {
			if(!string.IsNullOrEmpty(CategoryName))
				RemoveByName();
			else
				RemoveByIndex();
		}
		protected virtual void RemoveByIndex() {
			if(Index == -1)
				return;
			Ribbon.Categories.RemoveAt(Index);
		}
		protected virtual void RemoveByName() {
			if(string.IsNullOrEmpty(CategoryName))
				return;
			RibbonPageCategoryBase category = Ribbon.Categories[CategoryName];
			if(category != null)
				Ribbon.Categories.Remove(category);
		}
	}
	[ContentProperty("Page")]
	public class AddRibbonPageAction: RibbonControllerActionBase {
		public static readonly DependencyProperty PageProperty = DependencyPropertyManager.Register("Page", typeof(RibbonPage), typeof(AddRibbonPageAction), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty CategoryNameProperty = DependencyPropertyManager.Register("CategoryName", typeof(string), typeof(AddRibbonPageAction), new FrameworkPropertyMetadata(null));
		public string CategoryName {
			get { return (string)GetValue(CategoryNameProperty); }
			set { SetValue(CategoryNameProperty, value); }
		}
		public RibbonPage Page {
			get { return (RibbonPage)GetValue(PageProperty); }
			set { SetValue(PageProperty, value); }
		}
		protected override void ExecuteCore() {
			if(string.IsNullOrEmpty(CategoryName) || Page == null)
				return;
			RibbonPageCategoryBase category = Ribbon.Categories[CategoryName];
			if(category == null)
				return;
			category.Pages.Add(Page);
		}
	}
	public class InsertRibbonPageAction: AddRibbonPageAction {
		public static readonly DependencyProperty IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(InsertRibbonPageAction), new FrameworkPropertyMetadata(-1));
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		protected override void ExecuteCore() {
			if(Index < 0 || string.IsNullOrEmpty(CategoryName) || Page == null)
				return;
			RibbonPageCategoryBase category = Ribbon.Categories[CategoryName];
			if(category == null)
				return;
			category.Pages.Insert(Index, Page);
		}
	}
	public class RemoveRibbonPageAction: RemoveRibbonPageCategoryAction {
		public static readonly DependencyProperty PageNameProperty = DependencyPropertyManager.Register("PageName", typeof(string), typeof(RemoveRibbonPageAction), new FrameworkPropertyMetadata(null));
		public string PageName {
			get { return (string)GetValue(PageNameProperty); }
			set { SetValue(PageNameProperty, value); }
		}
		protected override void ExecuteCore() {
			if(string.IsNullOrEmpty(CategoryName))
				return;
			RibbonPageCategoryBase category = Ribbon.Categories[CategoryName];
			if(category == null)
				return;
			if(!string.IsNullOrEmpty(PageName))
				RemoveByName(category);
			else
				RemoveByIndex(category);
		}
		protected virtual void RemoveByIndex(RibbonPageCategoryBase category) {
			if(Index == -1)
				return;
			category.Pages.RemoveAt(Index);
		}
		protected virtual void RemoveByName(RibbonPageCategoryBase category) {
			if(string.IsNullOrEmpty(PageName))
				return;
			RibbonPage page = category.Pages[PageName];
			if(page != null)
				category.Pages.Remove(page);
		}
	}
	[ContentProperty("Group")]
	public class AddRibbonPageGroupAction: RibbonControllerActionBase {
		public static readonly DependencyProperty PageNameProperty = DependencyPropertyManager.Register("PageName", typeof(string), typeof(AddRibbonPageGroupAction), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty GroupProperty = DependencyPropertyManager.Register("Group", typeof(RibbonPageGroup), typeof(AddRibbonPageGroupAction), new FrameworkPropertyMetadata(null));
		public RibbonPageGroup Group {
			get { return (RibbonPageGroup)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public string PageName {
			get { return (string)GetValue(PageNameProperty); }
			set { SetValue(PageNameProperty, value); }
		}
		protected override void ExecuteCore() {
			if(string.IsNullOrEmpty(PageName) || Group == null)
				return;
			RibbonPage page = FindRibbonPage(PageName);
			if(page == null)
				return;
			page.Groups.Add(Group);
		}
	}
	public class InsertRibbonPageGroupAction: AddRibbonPageGroupAction {
		public static readonly DependencyProperty IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(InsertRibbonPageGroupAction), new FrameworkPropertyMetadata(null));
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		protected override void ExecuteCore() {
			if(Index < 0 || string.IsNullOrEmpty(PageName) || Group == null)
				return;
			RibbonPage page = FindRibbonPage(PageName);
			if(page == null)
				return;
			page.Groups.Insert(Index, Group);
		}
	}
	public class RemoveRibbonPageGroupAction: RibbonControllerActionBase {
		public static readonly DependencyProperty PageNameProperty = DependencyPropertyManager.Register("PageName", typeof(string), typeof(RemoveRibbonPageGroupAction), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty GroupNameProperty = DependencyPropertyManager.Register("GroupName", typeof(string), typeof(RemoveRibbonPageGroupAction), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(RemoveRibbonPageGroupAction), new FrameworkPropertyMetadata(null));
		public string PageName {
			get { return (string)GetValue(PageNameProperty); }
			set { SetValue(PageNameProperty, value); }
		}
		public string GroupName {
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		protected override void ExecuteCore() {
			if(string.IsNullOrEmpty(PageName))
				return;
			RibbonPage page = FindRibbonPage(PageName);
			if(page == null)
				return;
			if(!string.IsNullOrEmpty(GroupName))
				RemoveByName(page);
			else
				RemoveByIndex(page);
		}
		protected void RemoveByIndex(RibbonPage page) {
			if(Index == -1)
				return;
			page.Groups.RemoveAt(Index);
		}
		protected void RemoveByName(RibbonPage page) {
			if(string.IsNullOrEmpty(GroupName))
				return;
			RibbonPageGroup group = page.Groups[GroupName];
			if(group != null)
				page.Groups.Remove(group);
		}
	}
	public class AddRibbonItemLinkActionBase: RibbonControllerActionBase {
		public static readonly DependencyProperty ItemProperty = DependencyPropertyManager.Register("Item", typeof(BarItem), typeof(AddRibbonItemLinkActionBase), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty ItemNameProperty = DependencyPropertyManager.Register("ItemName", typeof(string), typeof(AddRibbonItemLinkActionBase), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty ItemLinkProperty = DependencyPropertyManager.Register("ItemLink", typeof(BarItemLink), typeof(AddRibbonItemLinkActionBase), new FrameworkPropertyMetadata(null));
		public BarItem Item {
			get { return (BarItem)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		public BarItemLink ItemLink {
			get { return (BarItemLink)GetValue(ItemLinkProperty); }
			set { SetValue(ItemLinkProperty, value); }
		}
		public string ItemName {
			get { return (string)GetValue(ItemNameProperty); }
			set { SetValue(ItemNameProperty, value); }
		}
		protected virtual void AddByName(string itemName) { }
	}
	public class InsertRibbonItemLinkActionBase: AddRibbonItemLinkActionBase {
		public static readonly DependencyProperty IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(InsertRibbonItemLinkActionBase), new FrameworkPropertyMetadata(null));
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		protected virtual void InsertByName(string itemName) { }
	}
	public class RemoveRibbonItemLinkActionBase: RibbonControllerActionBase {
		public static readonly DependencyProperty ItemLinkNameProperty = DependencyPropertyManager.Register("ItemLinkName", typeof(string), typeof(RemoveRibbonItemLinkActionBase), new FrameworkPropertyMetadata(null));
		public static readonly DependencyProperty IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), typeof(RemoveRibbonItemLinkActionBase), new FrameworkPropertyMetadata(null));
		public string ItemLinkName {
			get { return (string)GetValue(ItemLinkNameProperty); }
			set { SetValue(ItemLinkNameProperty, value); }
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		protected virtual void RemoveByName() { }
		protected virtual void RemoveByIndex() { }
	}
	public class AddQuickAccessToolbarItemLinkAction: AddRibbonItemLinkActionBase {
		protected override void ExecuteCore() {
			if(Ribbon.Toolbar == null)
				return;
			if(!string.IsNullOrEmpty(ItemName))
				AddByName(ItemName);
			else if(Item != null)
				Ribbon.Toolbar.ItemLinks.Add(AddItemAndCreateLink(Item));
			else if(ItemLink != null)
				Ribbon.Toolbar.ItemLinks.Add(ItemLink);
		}
		protected override void AddByName(string itemName) {
			BarItem item = Ribbon.Manager.Items[itemName];
			if(item != null)
				Ribbon.Toolbar.ItemLinks.Add(CreateLink(item));
		}
	}
	public class InsertQuickAccessToolbarItemLinkAction: InsertRibbonItemLinkActionBase {
		protected override void ExecuteCore() {
			if(Index < 0 || Ribbon.Toolbar == null)
				return;
			if(!string.IsNullOrEmpty(ItemName))
				InsertByName(ItemName);
			else if(Item != null)
				Ribbon.Toolbar.ItemLinks.Insert(Index, AddItemAndCreateLink(Item));
			else if(ItemLink != null)
				Ribbon.Toolbar.ItemLinks.Insert(Index, ItemLink);
		}
		protected override void InsertByName(string itemName) {
			BarItem item = Ribbon.Manager.Items[itemName];
			if(item != null)
				Ribbon.Toolbar.ItemLinks.Insert(Index, CreateLink(item));
		}
	}
	public class RemoveQuickAccessToolbarItemLinkAction: RemoveRibbonItemLinkActionBase {
		protected override void ExecuteCore() {
			if(!string.IsNullOrEmpty(ItemLinkName))
				RemoveByName();
			else
				RemoveByIndex();
		}
		protected override void RemoveByIndex() {
			if(Index == -1)
				return;
			Ribbon.Toolbar.ItemLinks.RemoveAt(Index);
		}
		protected override void RemoveByName() {
			if(string.IsNullOrEmpty(ItemLinkName))
				return;
			BarItemLink itemLink = Ribbon.Toolbar.ItemLinks[ItemLinkName];
			if(itemLink != null)
				Ribbon.Toolbar.ItemLinks.Remove(itemLink);
		}
	}
	public class AddPageHeaderItemLinkAction: AddRibbonItemLinkActionBase {
		protected override void ExecuteCore() {
			if(!string.IsNullOrEmpty(ItemName))
				AddByName(ItemName);
			else if(Item != null)
				Ribbon.PageHeaderItemLinks.Add(AddItemAndCreateLink(Item));
			else if(ItemLink != null)
				Ribbon.PageHeaderItemLinks.Add(ItemLink);
		}
		protected override void AddByName(string itemName) {
			BarItem item = Ribbon.Manager.Items[ItemName];
			if(item != null)
				Ribbon.PageHeaderItemLinks.Add(CreateLink(item));
		}
	}
	public class InsertPageHeaderItemLinkAction: InsertRibbonItemLinkActionBase {
		protected override void ExecuteCore() {
			if(Index < 0)
				return;
			if(!string.IsNullOrEmpty(ItemName))
				InsertByName(ItemName);
			else if(Item != null)
				Ribbon.PageHeaderItemLinks.Insert(Index, AddItemAndCreateLink(Item));
			else if(ItemLink != null)
				Ribbon.PageHeaderItemLinks.Insert(Index, ItemLink);
		}
		protected override void InsertByName(string itemName) {
			BarItem item = Ribbon.Manager.Items[ItemName];
			if(item != null)
				Ribbon.PageHeaderItemLinks.Insert(Index, CreateLink(item));
		}
	}
	public class RemovePageHeaderItemLinkAction: RemoveRibbonItemLinkActionBase {
		protected override void ExecuteCore() {
			if(!string.IsNullOrEmpty(ItemLinkName))
				RemoveByName();
			else
				RemoveByIndex();
		}
		protected override void RemoveByIndex() {
			if(Index == -1)
				return;
			Ribbon.PageHeaderItemLinks.RemoveAt(Index);
		}
		protected override void RemoveByName() {
			if(string.IsNullOrEmpty(ItemLinkName))
				return;
			BarItemLink itemLink = Ribbon.PageHeaderItemLinks[ItemLinkName];
			if(itemLink != null)
				Ribbon.PageHeaderItemLinks.Remove(itemLink);
		}
	}
	public class AddRibbonPageGroupItemLinkAction: AddRibbonItemLinkActionBase {
		public static readonly DependencyProperty GroupNameProperty = DependencyPropertyManager.Register("GroupName", typeof(string), typeof(AddRibbonPageGroupItemLinkAction), new FrameworkPropertyMetadata(null));
		public string GroupName {
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}		
		private RibbonPageGroup group { get; set; }
		protected override void ExecuteCore() {
			if(string.IsNullOrEmpty(GroupName))
				return;
			group = FindRibbonPageGroup(GroupName);
			if(group == null)
				return;
			if(!string.IsNullOrEmpty(ItemName))
				AddByName(ItemName);
			else if(Item != null)
				group.ItemLinks.Add(AddItemAndCreateLink(Item));
			else if(ItemLink != null)
				group.ItemLinks.Add(ItemLink);
		}
		protected override void AddByName(string itemName) {
			BarItem item = Ribbon.Manager.Items[ItemName];
			if(item != null)
				group.ItemLinks.Add(CreateLink(item));
		}
	}
	public class InsertRibbonPageGroupItemLinkAction: InsertRibbonItemLinkActionBase {
		public static readonly DependencyProperty GroupNameProperty = DependencyPropertyManager.Register("GroupName", typeof(string), typeof(InsertRibbonPageGroupItemLinkAction), new FrameworkPropertyMetadata(null));
		public string GroupName {
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}
		private RibbonPageGroup group { get; set; }
		protected override void ExecuteCore() {
			if(Index < 0 || string.IsNullOrEmpty(GroupName))
				return;
			group = FindRibbonPageGroup(GroupName);
			if(group == null)
				return;
			if(!string.IsNullOrEmpty(ItemName)) {
				BarItem item = Ribbon.Manager.Items[ItemName];
				if(item != null)
					group.ItemLinks.Insert(Index, CreateLink(item));
			}
			else if(Item != null)
				group.ItemLinks.Insert(Index, AddItemAndCreateLink(Item));
			else if(ItemLink != null)
				group.ItemLinks.Insert(Index, ItemLink);
		}
		protected override void InsertByName(string itemName) {
			BarItem item = Ribbon.Manager.Items[ItemName];
			if(item != null)
				group.ItemLinks.Insert(Index, CreateLink(item));
		}
	}
	public class RemoveRibbonPageGroupItemLinkAction: RemoveRibbonItemLinkActionBase {
		public static readonly DependencyProperty GroupNameProperty = DependencyPropertyManager.Register("GroupName", typeof(string), typeof(RemoveRibbonPageGroupItemLinkAction), new FrameworkPropertyMetadata(null));
		public string GroupName {
			get { return (string)GetValue(GroupNameProperty); }
			set { SetValue(GroupNameProperty, value); }
		}
		protected override void ExecuteCore() {
			if(string.IsNullOrEmpty(GroupName))
				return;
			RibbonPageGroup group = FindRibbonPageGroup(GroupName);
			if(group == null)
				return;
			if(!string.IsNullOrEmpty(ItemLinkName))
				RemoveByName(group);
			else
				RemoveByIndex(group);
		}
		protected void RemoveByIndex(RibbonPageGroup group) {
			if(Index == -1)
				return;
			group.ItemLinks.RemoveAt(Index);
		}
		protected void RemoveByName(RibbonPageGroup group) {
			if(string.IsNullOrEmpty(ItemLinkName))
				return;
			BarItemLink link = group.ItemLinks[ItemLinkName];
			if(link != null)
				group.ItemLinks.Remove(link);
		}
	}
	public class RibbonItemCollection: BarItemCollection {
		public RibbonItemCollection(RibbonControl ribbon) : base() { }
	}
	public interface IRibbonControllerAction: IBarManagerControllerAction { }
	public enum RibbonCollections {
		ToolbarItems,
		ToolbarItemLinks,
		PageHeaderItems,
		PageHeaderItemLinks,
		Categories
	}
	static class RibbonControlCollectionActionHelper {
		public static void RegisterDefaultGetters() {
			CollectionActionHelper.Instance.RegisterCollectionGetter<RibbonControl, RibbonPageCategoryBase>((action, ribbon) => ((RibbonControl)ribbon).Categories);
			CollectionActionHelper.Instance.RegisterCollectionGetter<RibbonPageCategoryBase, RibbonPage>((action, category) => ((RibbonPageCategoryBase)category).Pages);
			CollectionActionHelper.Instance.RegisterCollectionGetter<RibbonPage, RibbonPageGroup>((action, page) => ((RibbonPage)page).Groups);
			CollectionActionHelper.Instance.RegisterCollectionGetter<RibbonControl, IBarItem>((action, ribbon) => GetCorrectCollection(action, (RibbonControl)ribbon));
			CollectionActionHelper.Instance.RegisterDefaultContainerGetter<RibbonPageCategoryBase>((action, category) => GetDefaultPageCategoryContainer(action));
			CollectionActionHelper.Instance.RegisterDefaultContainerGetter<RibbonPage>((action, page) => GetDefaultPageContainer(action));
			CollectionActionHelper.Instance.RegisterDefaultContainerGetter<RibbonPageGroup>((action, group) => GetDefaultPageGroupContainer(action));
			CollectionActionHelper.Instance.RegisterDefaultContainerGetter<IBarItem>((action, item) => GetDefaultItemContainer(action, (IBarItem)item));
		}
		static object GetDefaultItemContainer(IBarManagerControllerAction action, IBarItem item) {
			var tag = (action as DependencyObject).With(CollectionAction.GetCollectionTag);
			if (!(tag is RibbonCollections))
				return null;
			if (((RibbonCollections)tag) == RibbonCollections.Categories)
				return null;
			return GetDefaultPageCategoryContainer(action);
		}
		static object GetDefaultPageGroupContainer(IBarManagerControllerAction action) {
			var category = GetDefaultPageContainer(action);
			if (category == null)
				return null;
			return category.Pages.FirstOrDefault();
		}
		static RibbonPageCategoryBase GetDefaultPageContainer(IBarManagerControllerAction action) {
			var ribbon = GetDefaultPageCategoryContainer(action);
			if (ribbon == null)
				return null;
			return ribbon.DefaultCategory ?? ribbon.Categories.FirstOrDefault();
		}
		static RibbonControl GetDefaultPageCategoryContainer(IBarManagerControllerAction action) {
			var ribbon = GetRibbon(action);
			if (ribbon != null)
				return ribbon;
			return BarNameScope.GetService<IElementRegistratorService>(action.Container).GetElements<IFrameworkInputElement>(ScopeSearchSettings.Local).OfType<RibbonControl>().FirstOrDefault();
		}
		static RibbonControl GetRibbon(IBarManagerControllerAction action) {
			return (action as DependencyObject).With(RibbonControl.GetRibbon) ?? (action.Container as DependencyObject).With(RibbonControl.GetRibbon) ?? (action.Container.AssociatedObject).With(RibbonControl.GetRibbon);
		}
		static IList GetCorrectCollection(IBarManagerControllerAction action, RibbonControl ribbon) {
			var tag = ((DependencyObject)action).GetValue(CollectionAction.CollectionTagProperty);
			if (tag == null || !(tag is RibbonCollections))
				return ribbon.ToolbarItems;
			var collectionKind = (RibbonCollections)tag;
			switch (collectionKind) {
				case RibbonCollections.ToolbarItems:
					return ribbon.ToolbarItems;
				case RibbonCollections.PageHeaderItems:
					return ribbon.PageHeaderItems;
				case RibbonCollections.ToolbarItemLinks:
					return ribbon.ToolbarItemLinks;
				case RibbonCollections.PageHeaderItemLinks:
					return ribbon.PageHeaderItemLinks;
				case RibbonCollections.Categories:
					return ribbon.Categories;
			}
			return null;
		}
	}
}
