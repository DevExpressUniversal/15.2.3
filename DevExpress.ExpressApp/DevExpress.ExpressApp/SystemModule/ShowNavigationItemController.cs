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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.DomainLogics;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Security;
namespace DevExpress.ExpressApp.SystemModule {
	public class CustomGenerateNavigationItemsEventArgs : EventArgs {
		private IModelRootNavigationItems navigationItems;
		public CustomGenerateNavigationItemsEventArgs(IModelRootNavigationItems navigationItems) {
			this.navigationItems = navigationItems;
		}
		public IModelRootNavigationItems NavigationItems { get { return navigationItems; } }
	}
	public class CustomShowNavigationItemEventArgs : HandledEventArgs {
		SingleChoiceActionExecuteEventArgs actionArguments;
		public CustomShowNavigationItemEventArgs(SingleChoiceActionExecuteEventArgs actionArguments) {
			this.actionArguments = actionArguments;
		}
		public SingleChoiceActionExecuteEventArgs ActionArguments {
			get { return actionArguments; }
		}
	}
	public class CustomUpdateSelectedItemEventArgs : HandledEventArgs {
		ChoiceActionItem proposedSelectedItem;
		public CustomUpdateSelectedItemEventArgs(ChoiceActionItem proposedSelectedItem) {
			this.proposedSelectedItem = proposedSelectedItem;
		}
		public ChoiceActionItem ProposedSelectedItem {
			get { return proposedSelectedItem; }
			set { proposedSelectedItem = value; }
		}
	}
	public class NavigationItemCreatedEventArgs : EventArgs {
		private ChoiceActionItem navigationItem;
		private IModelNavigationItem item;
		public NavigationItemCreatedEventArgs(IModelNavigationItem item, ChoiceActionItem navigationItem) {
			this.item = item;
			this.navigationItem = navigationItem;
		}
		public ChoiceActionItem NavigationItem {
			get { return navigationItem; }
		}
		public IModelNavigationItem ModelNavigationItem {
			get { return item; }
		}
	}
	public interface IModelApplicationNavigationItems {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelApplicationNavigationItemsNavigationItems")]
#endif
		IModelRootNavigationItems NavigationItems { get; }
	}
	[ModelNodesGenerator(typeof(NavigationItemNodeGenerator))]
	[ImageName("ModelEditor_Navigation_Items")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelRootNavigationItems")]
#endif
	public interface IModelRootNavigationItems : IModelNode {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelRootNavigationItemsItems")]
#endif
		IModelNavigationItems Items { get; }
		[Browsable(false)]
		IModelList<IModelNavigationItem> AllItems { get; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelRootNavigationItemsStartupNavigationItem")]
#endif
		[Category("Behavior")]
		[DataSourceProperty("AllItems")]
		IModelNavigationItem StartupNavigationItem { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRootNavigationItemsDefaultParentImageName"),
#endif
 Category("Appearance")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string DefaultParentImageName { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRootNavigationItemsDefaultLeafImageName"),
#endif
 Category("Appearance")]
		[Editor("DevExpress.ExpressApp.Win.Core.ModelEditor.ImageGalleryModelEditorControl, DevExpress.ExpressApp.Win" + XafAssemblyInfo.VersionSuffix + XafAssemblyInfo.AssemblyNamePostfix, typeof(System.Drawing.Design.UITypeEditor))]
		string DefaultLeafImageName { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRootNavigationItemsNavigationStyle"),
#endif
 DefaultValue(NavigationStyle.NavBar)]
		[Category("Appearance")]
		NavigationStyle NavigationStyle { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelRootNavigationItemsDefaultChildItemsDisplayStyle"),
#endif
 DefaultValue(ItemsDisplayStyle.LargeIcons)]
		[Category("Appearance")]
		ItemsDisplayStyle DefaultChildItemsDisplayStyle { get; set; }
	}
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelNavigationItems")]
#endif
	public interface IModelNavigationItems : IModelNode, IModelList<IModelNavigationItem> {
	}
	[ModelPersistentName("Item")]
#if !SL
	[DevExpressExpressAppLocalizedDescription("SystemModuleIModelNavigationItem")]
#endif
	public interface IModelNavigationItem : IModelNode, IModelBaseChoiceActionItem, IModelChoiceActionItemChildItemsDisplayStyle {
#if !SL
	[DevExpressExpressAppLocalizedDescription("IModelNavigationItemItems")]
#endif
		IModelNavigationItems Items { get; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelNavigationItemObjectKey"),
#endif
 Category("Data")]
		string ObjectKey { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelNavigationItemView"),
#endif
 ModelPersistentName("ViewId"), DataSourceProperty(ModelViewsDomainLogic.DataSourcePropertyPath)]
		[Category("Appearance")]
		IModelView View { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelNavigationItemQuickAccessItem"),
#endif
 Category("Behavior")]
		bool QuickAccessItem { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelNavigationItemVisible"),
#endif
 Category("Behavior")]
		[DefaultValue(true)]
		bool Visible { get; set; }  
	}
	public interface IModelClassNavigation {
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassNavigationIsNavigationItem"),
#endif
 ReadOnly(true)]
		[Category("Behavior")]
		bool IsNavigationItem { get; set; }
		[
#if !SL
	DevExpressExpressAppLocalizedDescription("IModelClassNavigationNavigationGroupName"),
#endif
 ReadOnly(true)]
		[Category("Appearance")]
		string NavigationGroupName { get; set; }
	}
	[DomainLogic(typeof(IModelRootNavigationItems))]
	public static class ModelNavigationItemsDomainLogic {
		private static IList<IModelNavigationItem> GetChildItems(IModelNavigationItem items) {
			List<IModelNavigationItem> retList = new List<IModelNavigationItem>();
			foreach(IModelNavigationItem item in items.Items) {
				retList.Add(item);
				retList.AddRange(GetChildItems(item));
			}
			return retList;
		}
		private static void CollectItemsWithView(IModelNavigationItems items, List<IModelNavigationItem> list) {
			foreach(IModelNavigationItem item in items) {
				if(item.View != null) { 
					list.Add(item);
				}
				CollectItemsWithView(item.Items, list);
			}
		}
		public static IModelList<IModelNavigationItem> Get_AllItems(IModelRootNavigationItems navigationItems) {
			List<IModelNavigationItem> list = new List<IModelNavigationItem>();
			CollectItemsWithView(navigationItems.Items, list);
			return new CalculatedModelNodeList<IModelNavigationItem>(list);
		}
		public static IModelNavigationItem Get_StartupNavigationItem(IModelRootNavigationItems navigationItems) {
			if(navigationItems.AllItems.Count > 0) {
				return navigationItems.AllItems[0];
			}
			return null;
		}
		public static bool TryFindRootNavigationItems(IModelBaseChoiceActionItem modelItem, out IModelRootNavigationItems result) {
			result = null;
			IModelNode currentModelNode = modelItem;
			int i = 0; 
			while(currentModelNode != null && i < 200) {
				if(currentModelNode is IModelRootNavigationItems) {
					result = (IModelRootNavigationItems)currentModelNode;
					return true;
				}
				currentModelNode = currentModelNode.Parent;
				i++;
			}
			return false;
		}
	}
	[DomainLogic(typeof(IModelBaseChoiceActionItem))]
	public static class ModelNavigationItemDomainLogic {
		public static String Get_Caption(IModelBaseChoiceActionItem item) {
			IModelNavigationItem navigationItem = item as IModelNavigationItem;
			if(navigationItem != null && navigationItem.View != null) {
				return navigationItem.View.Caption;
			}
			return item.Id;
		}
		public static String Get_ImageName(IModelBaseChoiceActionItem item) {
			IModelNavigationItem navigationItem = item as IModelNavigationItem;
			if(navigationItem == null) {
				return "";
			}
			String imageName = "";
			IModelView view = navigationItem.View;
			if(view != null) {
				imageName = view.ImageName;
			}
			if(String.IsNullOrEmpty(imageName) && (item.Application != null)) {
				IModelRootNavigationItems navigationItems = ((IModelApplicationNavigationItems)item.Application).NavigationItems;
				imageName = navigationItem.Items.Count == 0 ? navigationItems.DefaultLeafImageName : navigationItems.DefaultParentImageName;
			}
			return imageName;
		}
	}
	[DomainLogic(typeof(IModelChoiceActionItemChildItemsDisplayStyle))]
	public static class ModelChoiceActionItemChildItemsDisplayStyleDomainLogic {
		public static ItemsDisplayStyle Get_ChildItemsDisplayStyle(IModelChoiceActionItemChildItemsDisplayStyle item) {
			IModelNode node = (IModelNode)item;
			while(node != null && !(node is IModelRootNavigationItems)) {
				node = node.Parent;
			}
			if(node is IModelRootNavigationItems) {
				return ((IModelRootNavigationItems)node).DefaultChildItemsDisplayStyle;
			}
			return ItemsDisplayStyle.LargeIcons;
		}
	}
	[DomainLogic(typeof(IModelClassNavigation))]
	public static class ModelClassNavigationItemDomainLogic {
		public static bool Get_IsNavigationItem(IModelClass modelClass) {
			string groupName = "";
			System.Boolean? isNavigationItem = GetIsNavigationItem(modelClass.TypeInfo, out groupName);
			return isNavigationItem == true;
		}
		public static string Get_NavigationGroupName(IModelClass modelClass) {
			string groupName = "";
			System.Boolean? isNavigationItem = GetIsNavigationItem(modelClass.TypeInfo, out groupName);
			return isNavigationItem == true ? groupName : null;
		}
		internal static bool? GetIsNavigationItem(ITypeInfo classInfo, out string groupName) {
			NavigationItemAttribute navigationItemAttribute =
				classInfo.FindAttribute<NavigationItemAttribute>(false);
			if((navigationItemAttribute != null) && (classInfo.IsPersistent || !classInfo.IsInterface)) {
				groupName = navigationItemAttribute.GroupName;
				return navigationItemAttribute.IsNavigationItem;
			}
			if(classInfo.FindAttribute<DefaultClassOptionsAttribute>() != null) {
				groupName = NavigationItemAttribute.DefaultGroupName;
				return true;
			}
			groupName = "";
			return null;
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum SynchronizeItemsWithSecurityMode { SelectedItem, AllItems }
	public class ShowNavigationItemController : WindowController, IModelExtender {
		public const string SecurityVisibleKey = "HasRights";
		private SingleChoiceAction showNavigationItemAction;
		private IContainer components;
		private void showNavigationItemAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs args) {
			((SingleChoiceAction)sender).HandleException += new EventHandler<HandleExceptionEventArgs>(showNavigationItemAction_HandleException);
			ShowNavigationItem(args);
		}
		private void showNavigationItemAction_HandleException(object sender, HandleExceptionEventArgs e) {
			((SingleChoiceAction)sender).HandleException -= new EventHandler<HandleExceptionEventArgs>(showNavigationItemAction_HandleException);
			UpdateSelectedItem(Window.View);
		}
		private void showNavigationItemAction_ExecuteCompleted(object sender, ActionBaseEventArgs e) {
			((SingleChoiceAction)sender).HandleException -= new EventHandler<HandleExceptionEventArgs>(showNavigationItemAction_HandleException);
			if((Window != null) && ((SingleChoiceActionExecuteEventArgs)e).SelectedChoiceActionItem != null
				&& ((SingleChoiceActionExecuteEventArgs)e).SelectedChoiceActionItem.Data is ViewShortcut) {
				UpdateSelectedItem(Window.View);
			}
		}
		private void Window_ViewChanged(object sender, ViewChangedEventArgs e) {
			UpdateSelectedItem(Window.View);
		}
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.showNavigationItemAction = new DevExpress.ExpressApp.Actions.SingleChoiceAction(this.components);
			this.showNavigationItemAction.Caption = "Navigation";
			this.showNavigationItemAction.Category = "ViewsNavigation";
			this.showNavigationItemAction.Id = "ShowNavigationItem";
			this.showNavigationItemAction.ImageName = "Action_NavigationBar";
			this.showNavigationItemAction.ShowItemsOnClick = true;
			this.showNavigationItemAction.Execute += new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.showNavigationItemAction_OnExecute);
			this.showNavigationItemAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(showNavigationItemAction_ExecuteCompleted);
			this.TargetWindowType = DevExpress.ExpressApp.WindowType.Main;
			this.SynchronizeItemsWithSecurityMode = SynchronizeItemsWithSecurityMode.SelectedItem;
		}
		private ViewShortcut GetListViewShortcut(ViewShortcut detailViewShortcut) {
			if(detailViewShortcut.ObjectClass == null)
				return detailViewShortcut;
			if((ShowNavigationItemAction.SelectedItem != null) && (ShowNavigationItemAction.SelectedItem.Data is ViewShortcut)) {
				ViewShortcut selectedItemViewShortcut = Application.GetCompletedViewShortcut((ViewShortcut)ShowNavigationItemAction.SelectedItem.Data);
				if(selectedItemViewShortcut.ObjectClass == detailViewShortcut.ObjectClass) {
					return selectedItemViewShortcut;
				}
			}
			string listViewId = Application.FindListViewId(detailViewShortcut.ObjectClass);
			return new ViewShortcut(detailViewShortcut.ObjectClass, null, listViewId);
		}
		private ViewShortcut GetCompletedViewShortcut(ViewShortcut shortcut, bool removeCustomKeys, bool findListView) {
			ViewShortcut result;
			if(findListView) {
				result = GetListViewShortcut(shortcut);
			}
			else {
				result = shortcut;
			}
			result = Application.GetCompletedViewShortcut(result);
			if(!removeCustomKeys) {
				foreach(string key in shortcut.Keys) {
					if(!result.ContainsKey(key)) {
						result[key] = shortcut[key];
					}
				}
			}
			return result;
		}
		private ChoiceActionItem FindNavigationItemByViewShortcutInternal(ViewShortcut shortcut,
			ChoiceActionItemCollection actionItems, bool skipCustomKeys, ViewShortcut requestedShortcut) {
			ChoiceActionItem result = null;
			if(shortcut != requestedShortcut) {
				if(ShowNavigationItemAction.SelectedItem != null && ShowNavigationItemAction.SelectedItem.Data is ViewShortcut) {
					if(requestedShortcut.ViewId == ((ViewShortcut)ShowNavigationItemAction.SelectedItem.Data).ViewId) {
						return this.ShowNavigationItemAction.SelectedItem;
					}
				}
			}
			foreach(ChoiceActionItem item in actionItems) {
				if(item.Items.Count != 0) {
					result = FindNavigationItemByViewShortcutInternal(shortcut, item.Items, skipCustomKeys, requestedShortcut);
				}
				if(result != null)
					return result;
				if(item.Data != null && item.Data is ViewShortcut) {
					if(requestedShortcut.Equals(GetCompletedViewShortcut((ViewShortcut)item.Data, skipCustomKeys, false))) {
						return item;
					}
				}
			}
			return result;
		}
		private ChoiceActionItem FindNavigationItemByViewShortcutInternal(ViewShortcut shortcut, ChoiceActionItemCollection actionItems, bool skipCustomKeys) {
			ChoiceActionItem result = null;
			ViewShortcut requestedShortcut = GetCompletedViewShortcut(shortcut, skipCustomKeys, false);
			if(requestedShortcut != null) {
				result = FindNavigationItemByViewShortcutInternal(shortcut, actionItems, skipCustomKeys, requestedShortcut);
			}
			if(result == null) {
				requestedShortcut = GetCompletedViewShortcut(shortcut, skipCustomKeys, true);
				if(requestedShortcut != null) {
					result = FindNavigationItemByViewShortcutInternal(shortcut, actionItems, skipCustomKeys, requestedShortcut);
				}
			}
			return result;
		}
		private ChoiceActionItem FindNavigationItemByViewShortcutInternal(ViewShortcut shortcut, ChoiceActionItemCollection actionItems) {
			ChoiceActionItem result = FindNavigationItemByViewShortcutInternal(shortcut, actionItems, false);
			if(result != null)
				return result;
			return FindNavigationItemByViewShortcutInternal(shortcut, actionItems, true);
		}
		private ChoiceActionItem FindFirstNavigationItemWithView(ChoiceActionItemCollection items) {
			foreach(ChoiceActionItem item in items) {
				if(item.Active && item.Enabled) {
					if(item.Data is ViewShortcut) {
						return item;
					}
					ChoiceActionItem result = FindFirstNavigationItemWithView(item.Items);
					if(result != null) {
						return result;
					}
				}
			}
			return null;
		}
		private void FindNavigationItems(List<ChoiceActionItem> list, ChoiceActionItemCollection items, string itemId, string viewId) { 
			foreach(ChoiceActionItem item in items) {
				if(item.Id == itemId) {
					ViewShortcut shortcut = item.Data as ViewShortcut;
					if(shortcut != null && shortcut.ViewId == viewId) {
						list.Add(item);
					}
				}
				FindNavigationItems(list, item.Items, itemId, viewId);
			}
		}
		private ChoiceActionItem FindNavigationItem(IModelNavigationItem model) { 
			List<ChoiceActionItem> items = new List<ChoiceActionItem>();
			FindNavigationItems(items, ShowNavigationItemAction.Items, model.Id, model.View.Id);
			if(items.Count > 0) {
				if(items.Count > 1) {
					Tracing.Tracer.LogWarning("Several navigation items with Id '{0}' are found. The first one is chosen.", model.Id);
				}
				return items[0];
			}
			return null;
		}
		protected virtual IModelViews GetModelViews() {
			return Application.Model.Views;
		}
		protected virtual void SynchItemWithSecurity(ChoiceActionItem item) {
			Guard.ArgumentNotNull(item, "item");
			ViewShortcut viewShortcut = item.Data as ViewShortcut;
			if(viewShortcut != null) {
				IModelViews views = GetModelViews();
				bool hasRights = HasRights(item, views);
				item.Enabled.SetItemValue(SecurityVisibleKey, hasRights);
				if(hasRights && !item.Active[SecurityVisibleKey]) {
					item.Active.SetItemValue(SecurityVisibleKey, hasRights);
				}
			}
		}
		protected virtual void SynchItemsWithSecurity(ChoiceActionItemCollection items) {
			Tracing.Tracer.LockFlush();
			try {
				foreach(ChoiceActionItem item in items) {
					SynchItemWithSecurity(item);
					SynchItemsWithSecurity(item.Items);
				}
			}
			finally {
				Tracing.Tracer.ResumeFlush();
			}
		}
		protected virtual bool HasRights(ChoiceActionItem item, IModelViews views) {
			ViewShortcut shortcut = (ViewShortcut)item.Data;
			IModelView view = views[shortcut.ViewId];
			if(view == null) {
				throw new ArgumentException(String.Format("Cannot find the '{0}' view specified by the shortcut: {1}", shortcut.ViewId, shortcut.ToString()));
			}
			Type type = (view is IModelObjectView) ? ((IModelObjectView)view).ModelClass.TypeInfo.Type : null;
			if(type != null) {
				if(!string.IsNullOrEmpty(shortcut.ObjectKey) && !shortcut.ObjectKey.StartsWith(CriteriaWrapper.ParameterPrefix)) {
					try {
						using(IObjectSpace objectSpace = CreateObjectSpace(type)) {
							object targetObject = objectSpace.GetObjectByKey(type, objectSpace.GetObjectKey(type, shortcut.ObjectKey));
							return DataManipulationRight.CanRead(type, null, targetObject, null, objectSpace) && DataManipulationRight.CanNavigate(type, targetObject, objectSpace);
						}
					}
					catch { }
				}
				else {
					return DataManipulationRight.CanNavigate(type, null, null);
				}
			}
			return true;
		}
		protected virtual IObjectSpace CreateObjectSpace(Type type) {
			return Application.CreateObjectSpace(type);
		}
		protected internal void ProcessItem(IModelNavigationItem item, ChoiceActionItemCollection choiceActionItems) {
			ChoiceActionItem choiceActionItem;
			if(item.View != null) {
				choiceActionItem = new ChoiceActionItem(item, new ViewShortcut(item.View.Id, item.ObjectKey));
				choiceActionItem.Active[SecurityVisibleKey] = HasRights(choiceActionItem, item.Application.Views);
			}
			else {
				choiceActionItem = new ChoiceActionItem(item);
				choiceActionItem.ActiveItemsBehavior = ActiveItemsBehavior.RequireActiveItems;
			}
			choiceActionItem.Active["Visible"] = item.Visible;
			choiceActionItems.Add(choiceActionItem);
			foreach(IModelNavigationItem childItem in item.Items) {
				ProcessItem(childItem, choiceActionItem.Items);
			}
			OnNavigationItemCreated(item, choiceActionItem);
		}
		protected virtual void InitializeItems() {
			Tracing.Tracer.LockFlush();
			try {
				Tracing.Tracer.LogSubSeparator(string.Format("Initialize '{0}' action items", ShowNavigationItemAction.Id));
				ShowNavigationItemAction.BeginUpdate();
				ShowNavigationItemAction.Items.Clear();
				HandledEventArgs args = new HandledEventArgs(false);
				if(CustomInitializeItems != null) {
					args = new HandledEventArgs(true);
					CustomInitializeItems(this, args);
				}
				if(!args.Handled) {
					foreach(IModelNavigationItem item in ((IModelApplicationNavigationItems)Application.Model).NavigationItems.Items) {
						ProcessItem(item, ShowNavigationItemAction.Items);
					}
					OnItemsInitialized();
				}
				SynchronizeItemsWithSecurity(); 
				ShowNavigationItemAction.EndUpdate();
				if(ShowNavigationItemAction.SelectedItem == null || !ShowNavigationItemAction.SelectedItem.Enabled || !ShowNavigationItemAction.SelectedItem.Active) {
					ShowNavigationItemAction.SelectedItem = GetStartupNavigationItem();
				}
			}
			finally {
				Tracing.Tracer.ResumeFlush();
			}
		}
		protected virtual void ShowNavigationItem(SingleChoiceActionExecuteEventArgs args) {
			CustomShowNavigationItemEventArgs customProcessArgs = new CustomShowNavigationItemEventArgs(args);
			if(CustomShowNavigationItem != null) {
				CustomShowNavigationItem(this, customProcessArgs);
			}
			if(!customProcessArgs.Handled) {
				SecuritySystem.ReloadPermissions();
				if(SynchronizeItemsWithSecurityMode == SynchronizeItemsWithSecurityMode.SelectedItem) {
					if(args.SelectedChoiceActionItem != null) {
						SynchItemWithSecurity(args.SelectedChoiceActionItem);
					}
				}
				else {
					SynchItemsWithSecurity(showNavigationItemAction.Items);
				}
				if((args.SelectedChoiceActionItem != null) && args.SelectedChoiceActionItem.Enabled) {
					ShowNavigationItemCore(args);
				}
			}
		}
		protected virtual void ShowNavigationItemCore(SingleChoiceActionExecuteEventArgs args) {
			Guard.ArgumentNotNull(args.SelectedChoiceActionItem, "args.SelectedChoiceActionItem");
			ViewShortcut shortcut = args.SelectedChoiceActionItem.Data as ViewShortcut;
			if(shortcut != null) {
				args.ShowViewParameters.CreatedView = Application.ProcessShortcut(shortcut);
				args.ShowViewParameters.TargetWindow = TargetWindow.Current;
			}
		}
		protected virtual void OnNavigationItemCreated(IModelNavigationItem item, ChoiceActionItem navigationItem) {
			if(NavigationItemCreated != null) {
				NavigationItemCreated(this, new NavigationItemCreatedEventArgs(item, navigationItem));
			}
		}
		protected override void OnWindowChanging(Window window) {
			base.OnWindowChanging(window);
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.ViewChanged += new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			InitializeItems();
		}
		protected override void OnDeactivated() {
			Window.ViewChanged -= new EventHandler<ViewChangedEventArgs>(Window_ViewChanged);
			base.OnDeactivated();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.showNavigationItemAction.Execute -= new DevExpress.ExpressApp.Actions.SingleChoiceActionExecuteEventHandler(this.showNavigationItemAction_OnExecute);
				this.showNavigationItemAction.ExecuteCompleted -= new EventHandler<ActionBaseEventArgs>(showNavigationItemAction_ExecuteCompleted);
			}
			base.Dispose(disposing);
		}
		protected virtual void OnItemsInitialized() {
			if(ItemsInitialized != null) {
				ItemsInitialized(this, EventArgs.Empty);
			}
		}
		public ShowNavigationItemController()
			: base() {
			InitializeComponent();
			RegisterActions(components);
		}
		public virtual ChoiceActionItem GetStartupNavigationItem() {
			if(Active) {
				if(Application != null && Application.Model != null) {
					IModelNavigationItem startupNavigationItem = ((IModelApplicationNavigationItems)Application.Model).NavigationItems.StartupNavigationItem;
					if(startupNavigationItem != null) {
						if(startupNavigationItem.View == null) {
							throw new InvalidOperationException(string.Format("The '{0}' navigation item has no view", startupNavigationItem.Id));
						}
						ChoiceActionItem result = FindNavigationItem(startupNavigationItem);
						if(result != null && result.Enabled && result.Active) {
							return result;
						}
						return FindFirstNavigationItemWithView(ShowNavigationItemAction.Items);
					}
				}
			}
			return null;
		}
		public void SynchronizeItemsWithSecurity() {
			SecuritySystem.ReloadPermissions();
			SynchItemsWithSecurity(showNavigationItemAction.Items);
		}
		public void RecreateNavigationItems() {
			ViewShortcut currentViewShortcut = showNavigationItemAction.SelectedItem.Data as ViewShortcut;
			InitializeItems();
			if(currentViewShortcut != null) {
				UpdateSelectedItem(currentViewShortcut);
			}
		}
		public void UpdateSelectedItem(ViewShortcut currentViewShortcut) {
			ChoiceActionItem currentViewShortcutActionItem = FindNavigationItemByViewShortcut(currentViewShortcut);
			CustomUpdateSelectedItemEventArgs eventArgs = new CustomUpdateSelectedItemEventArgs(currentViewShortcutActionItem);
			if(CustomUpdateSelectedItem != null) {
				CustomUpdateSelectedItem(this, eventArgs);
			}
			if(eventArgs.Handled) {
				ShowNavigationItemAction.SelectedItem = eventArgs.ProposedSelectedItem;
			}
			else {
				ViewShortcut selectedShortcut = null;
				if(ShowNavigationItemAction.SelectedItem != null && ShowNavigationItemAction.SelectedItem.Data is ViewShortcut) {
					selectedShortcut = Application.GetCompletedViewShortcut((ViewShortcut)ShowNavigationItemAction.SelectedItem.Data);
				}
				if(!currentViewShortcut.Equals(selectedShortcut)) {
					if(currentViewShortcutActionItem != null && currentViewShortcutActionItem.Active && currentViewShortcutActionItem.Enabled) {
						ShowNavigationItemAction.SelectedItem = currentViewShortcutActionItem;
					}
				}
			}
		}
		public void UpdateSelectedItem(View view) {
			if(view != null && ShowNavigationItemAction != null) {
				ViewShortcut currentShortcut = view.CreateShortcut();
				UpdateSelectedItem(currentShortcut);
			}
		}
		public ChoiceActionItem FindNavigationItemByViewShortcut(ViewShortcut shortcut) {
			return FindNavigationItemByViewShortcutInternal(shortcut, ShowNavigationItemAction.Items);
		}
		[DefaultValue(SynchronizeItemsWithSecurityMode.SelectedItem), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public SynchronizeItemsWithSecurityMode SynchronizeItemsWithSecurityMode { get; set; }
#if !SL
	[DevExpressExpressAppLocalizedDescription("ShowNavigationItemControllerShowNavigationItemAction")]
#endif
		public SingleChoiceAction ShowNavigationItemAction {
			get { return showNavigationItemAction; }
		}
		public event EventHandler<CustomShowNavigationItemEventArgs> CustomShowNavigationItem;
		public event EventHandler<HandledEventArgs> CustomInitializeItems;
		public event EventHandler<EventArgs> ItemsInitialized;
		public event EventHandler<CustomUpdateSelectedItemEventArgs> CustomUpdateSelectedItem;
		public event EventHandler<NavigationItemCreatedEventArgs> NavigationItemCreated;
		public static IModelNavigationItem GenerateNavigationItem(IModelApplication modelApplication, string navigationItemGroupName, string navigationItemId, string navigationItemCaption, string viewId, string objectKey) {
			return GenerateNavigationItem(modelApplication, new ViewShortcut(viewId, objectKey), navigationItemGroupName, navigationItemId, navigationItemCaption);
		}
		public static IModelNavigationItem GenerateNavigationItem(IModelApplication modelApplication, ViewShortcut viewShortcut, string groupName) {
			return GenerateNavigationItem(modelApplication, viewShortcut, groupName, string.Empty, null);
		}
		public static IModelNavigationItem GenerateNavigationItem(IModelApplication modelApplication, ViewShortcut viewShortcut, string groupName, string navigationItemId, string caption) {
			if(string.IsNullOrEmpty(groupName)) {
				groupName = DevExpress.Persistent.Base.NavigationItemAttribute.DefaultGroupName;
			}
			IModelRootNavigationItems navigationItems = ((IModelApplicationNavigationItems)modelApplication).NavigationItems;
			IModelNavigationItem navigationGroup = navigationItems.Items[groupName] ?? navigationItems.Items.AddNode<IModelNavigationItem>(groupName);
			string itemId = string.IsNullOrEmpty(navigationItemId) ? viewShortcut.ViewId : navigationItemId;
			IModelNavigationItem navigationItem = navigationGroup.Items[itemId] ?? navigationGroup.Items.AddNode<IModelNavigationItem>(itemId);
			navigationItem.View = modelApplication.Views[viewShortcut.ViewId];
			if(viewShortcut.ObjectKey != null) {
				navigationItem.ObjectKey = viewShortcut.ObjectKey;
			}
			if(!string.IsNullOrEmpty(caption) && navigationItem.Caption != caption) {
				navigationItem.Caption = caption;
			}
			return navigationItem;
		}
		#region IExtendModel Members
		void IModelExtender.ExtendModelInterfaces(ModelInterfaceExtenders extenders) {
			extenders.Add<IModelApplication, IModelApplicationNavigationItems>();
			extenders.Add<IModelClass, IModelClassNavigation>();
		}
		#endregion
	}
	public class NavigationItemNodeGenerator : ModelNodesGeneratorBase {
		protected override void GenerateNodesCore(ModelNode node) {
			IModelRootNavigationItems navigationItems = (IModelRootNavigationItems)node;
			foreach(IModelClass modelClass in navigationItems.Application.BOModel) {
				IModelClassNavigation navigation = (IModelClassNavigation)modelClass;
				if(navigation.IsNavigationItem) {
					IModelListView defaultListView = modelClass.DefaultListView;
					string defaultListViewId = defaultListView != null ? defaultListView.Id : string.Empty;
					ShowNavigationItemController.GenerateNavigationItem(navigationItems.Application, new ViewShortcut(defaultListViewId, null), navigation.NavigationGroupName);
				}
			}
			((IModelRootNavigationItems)node).DefaultParentImageName = "BO_Folder";
			((IModelRootNavigationItems)node).DefaultLeafImageName = "BO_Unknown";
		}
	}
}
