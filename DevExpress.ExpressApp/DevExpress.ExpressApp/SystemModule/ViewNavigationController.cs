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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.SystemModule {
	public class ViewNavigationController : WindowController {
		public const int HistoryDepth = 10;
		private IContainer components;
		private SingleChoiceAction navigateBackAction;
		private SingleChoiceAction navigateForwardAction;
		private bool isNavigating = false;
		private IObjectSpace objectSpace;
		protected NavigationHistory<ViewShortcut> navigationHistory;
		protected SizeLimitedStack<ViewShortcut> navigationStack;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.navigateBackAction = new SingleChoiceAction(this.components);
			this.navigateForwardAction = new SingleChoiceAction(this.components);
			this.navigateBackAction.Caption = "Back";
			this.navigateBackAction.Category = "ViewsHistoryNavigation";
			this.navigateBackAction.Id = "NavigateBack";
			this.navigateBackAction.ToolTip = "Navigate to the previous view";
			this.navigateBackAction.ImageName = "MenuBar_BackFrame";
			this.navigateBackAction.Shortcut = "AltLeftArrow";
			this.navigateBackAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			this.navigateBackAction.Execute += new SingleChoiceActionExecuteEventHandler(this.navigateBackAction_OnExecute);
			this.navigateBackAction.EmptyItemsBehavior = EmptyItemsBehavior.Disable;
			this.navigateForwardAction.Caption = "Forward";
			this.navigateForwardAction.Category = "ViewsHistoryNavigation";
			this.navigateForwardAction.Id = "NavigateForward";
			this.navigateForwardAction.ToolTip = "Navigate to the next view";
			this.navigateForwardAction.ImageName = "MenuBar_ForwardFrame";
			this.navigateForwardAction.Shortcut = "AltRightArrow";
			this.navigateForwardAction.ItemType = SingleChoiceActionItemType.ItemIsOperation;
			this.navigateForwardAction.Execute += new SingleChoiceActionExecuteEventHandler(this.navigateForwardAction_OnExecute);
			this.navigateForwardAction.EmptyItemsBehavior = EmptyItemsBehavior.Disable;
		}
		private void RemoveShortcutFromStack(ViewShortcut shortcut) {
			string savedMode = shortcut[DetailView.ViewEditModeKeyName];
			shortcut[DetailView.ViewEditModeKeyName] = ViewEditMode.Edit.ToString();
			while(navigationStack.Remove(shortcut))
				;
			shortcut[DetailView.ViewEditModeKeyName] = ViewEditMode.View.ToString();
			while(navigationStack.Remove(shortcut))
				;
			shortcut[DetailView.ViewEditModeKeyName] = savedMode;
		}
		private void view_Closed(object sender, EventArgs e) {
			((View)sender).Closed -= new EventHandler(view_Closed);
			if(Active) {
				ProcessViewClosed((View)sender);
			}
		}
		private void window_ViewChanging(object sender, ViewChangingEventArgs e) {
			if(Window.View != null) {
				if(Window.View is DetailView) {
					DeleteLastItemIfItNeeds();
					Window.View.CurrentObjectChanged -= new EventHandler(view_CurrentObjectChanged);
				}
				Window.View.Closed -= new EventHandler(view_Closed);
				if(objectSpace != null) {
					objectSpace.Committing -= new EventHandler<CancelEventArgs>(objectSpace_Committing);
					objectSpace.Committed -= new EventHandler(objectSpace_Committed);
					objectSpace.Disposed -= new EventHandler(objectSpace_Disposed);
				}
				UpdateShortcutScrollPosition();
			}
		}
		private void UpdateShortcutScrollPosition() {
			if(IsNavigationAllowed()) {
				ViewShortcut currentShortcut = Window.View.CreateShortcut();
				if(navigationHistory.CurrentPositionIndex > -1) {
					foreach(HistoryItem<ViewShortcut> item in navigationHistory) {
						if(item.Item.Equals(currentShortcut)) {
							item.Item.ScrollPosition = currentShortcut.ScrollPosition;
						}
					}
				}
			}
		}
		private void window_ViewChanged(Object sender, ViewChangedEventArgs e) {
			AddToHistory(Window.View);
			if(Window.View != null) {
				Window.View.Closed += new EventHandler(view_Closed);
			}
			if(Window.View is ObjectView) {
				objectSpace = Window.View.ObjectSpace;
				objectSpace.Committing += new EventHandler<CancelEventArgs>(objectSpace_Committing);
				objectSpace.Committed += new EventHandler(objectSpace_Committed);
				objectSpace.Disposed += new EventHandler(objectSpace_Disposed);
				if(Window.View is DetailView) {
					Window.View.CurrentObjectChanged += new EventHandler(view_CurrentObjectChanged);
				}
			}
		}
		private void objectSpace_Disposed(object sender, EventArgs e) {
			IObjectSpace objectSpace = (IObjectSpace)sender;
			objectSpace.Committing -= new EventHandler<CancelEventArgs>(objectSpace_Committing);
			objectSpace.Committed -= new EventHandler(objectSpace_Committed);
			objectSpace.Disposed -= new EventHandler(objectSpace_Disposed);
		}
		private void view_CurrentObjectChanged(object sender, EventArgs e) {
			AddToHistory(Window.View);
		}
		private void objectSpace_Committing(Object sender, CancelEventArgs e) {
			IObjectSpace objectSpace = Window.View.ObjectSpace;
			ProcessDeletedObjects(objectSpace, objectSpace.GetObjectsToDelete(true));
			ArrayList deletedObjects = new ArrayList();
			foreach(Object obj in objectSpace.GetObjectsToSave(true)) {
				if(objectSpace.IsDeletedObject(obj)) {
					deletedObjects.Add(obj);
				}
			}
			ProcessDeletedObjects(objectSpace, deletedObjects);
		}
		private void objectSpace_Committed(object sender, EventArgs e) {
			ViewShortcut shortcutWithoutEditMode = new ViewShortcut();
			Window.View.CreateShortcut().CopyTo(shortcutWithoutEditMode);
			shortcutWithoutEditMode.Remove(DetailView.ViewEditModeKeyName);
			UpdateCurrentHistoryItem(GetCurrentViewCaption(), GetCurrentViewImageName(), shortcutWithoutEditMode);
		}
		private void navigateBackAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs e) {
			SingleChoiceAction action = sender as SingleChoiceAction;
			int indexOfAction = action.Items.IndexOf(e.SelectedChoiceActionItem);
			NavigateTo(navigationHistory.CurrentPositionIndex - indexOfAction - 1);
		}
		private void navigateForwardAction_OnExecute(Object sender, SingleChoiceActionExecuteEventArgs e) {
			SingleChoiceAction action = sender as SingleChoiceAction;
			int indexOfAction = action.Items.IndexOf(e.SelectedChoiceActionItem);
			NavigateTo(navigationHistory.CurrentPositionIndex + indexOfAction + 1);
		}
		private bool DeleteLastItemIfItNeeds() {
			if(IsNavigationAllowed()) {
				if(IsItemToRemove()) {
					navigationHistory.DeleteCurrentItem();
					navigationStack.Pop();
					RefreshActionsState();
					return true;
				}
			}
			return false;
		}
		protected bool IsNavigating {
			get { return isNavigating; }
		}
		protected NavigationHistory<ViewShortcut> NavigationHistory {
			get { return navigationHistory; }
		}
		protected string GetCurrentViewCaption() {
			string result = Window.View.Caption;
			if(Window.View is DetailView) {
				string resultDetail = Window.View.GetCurrentObjectCaption();
				if(!String.IsNullOrEmpty(resultDetail)) {
					result = resultDetail;
				}
			}
			return result;
		}
		protected string GetCurrentViewImageName() {
			if(Window.View.Model != null) {
				return Window.View.Model.ImageName;
			}
			return null;
		} 
		protected void UpdateCurrentHistoryItem(string caption, string imageName, ViewShortcut currentShortcut) {
			if(IsNavigationAllowed()) {
				if(currentShortcut.HasViewParameters && navigationHistory.CurrentPosition.Item.Equals(currentShortcut) || !navigationHistory.CurrentPosition.Item.HasViewParameters) {
					navigationHistory.UpdateCurrentItem(caption, imageName, currentShortcut);
					navigationStack.Pop();
					navigationStack.Push(currentShortcut);
				}
				if(!navigationHistory.CurrentPosition.Item.HasViewParameters) {
					navigationHistory.DeleteCurrentItem();
					navigationStack.Pop();
				}
			}
		}
		protected virtual Boolean IsNavigationAllowed() {
			return Window.IsMain;
		}
		protected virtual void AddToHistory(View view) {
			if(IsNavigationAllowed() && view != null) {
				ViewShortcut shortcut = view.CreateShortcut();
				shortcut.Remove(DetailView.ViewEditModeKeyName);
				List<string> ignoredParameters = new List<string>();
				ignoredParameters.Add(ViewShortcut.TemporaryObjectKeyParamName);
				ignoredParameters.AddRange(ViewShortcut.EqualsDefaultIgnoredParameters);
				if(navigationStack.Count == 0 || !navigationStack.Peek().Equals(shortcut, ignoredParameters)) {
					navigationStack.Push(shortcut);
					if(!isNavigating) {
						navigationHistory.Add(GetCurrentViewCaption(), GetCurrentViewImageName(), shortcut);
					}
					RefreshActionsState();
				}
			}
		}
		protected bool IsCurrentObjectDeleted() {
			DetailView currentDetailView = Window.View as DetailView;
			if(currentDetailView != null && navigationHistory.CurrentPosition.Item != null) {
				object obj = currentDetailView.CurrentObject;
				if(obj != null && currentDetailView.ObjectSpace.IsDeletedObject(obj)) {
					return true;
				}
			}
			return false;
		}
		protected Boolean IsCurrentObjectDisposed() {
			return (Window.View != null) && Window.View.ObjectSpace.IsDisposedObject(Window.View.CurrentObject);
		}
		protected bool IsCurrentObjectNew() {
			if(navigationHistory.CurrentPositionIndex > -1) {
				Boolean isNewObject;
				if(Boolean.TryParse(navigationHistory.CurrentPosition.Item[ViewShortcut.IsNewObject], out isNewObject)) {
					return isNewObject;
				}
			}
			return false;
		}
		protected bool IsItemToRemove() {
			return IsCurrentObjectNew();
		}
		protected virtual void ProcessViewClosed(View closedView) {
			if(IsNavigationAllowed()) {
				if(!DeleteLastItemIfItNeeds()) {
					if((navigationStack.Count > 1) && (Window.View.CurrentObject != null) && !IsCurrentObjectDisposed() && !IsCurrentObjectDeleted()) {
						Object currentObject = Window.View.CurrentObject;
						if(!Window.View.ObjectSpace.IsNewObject(currentObject)) {
							navigationStack.Pop();
						}
					}
				}
				ViewShortcut shortcut = null;
				if(!navigationStack.IsEmpty()) {
					shortcut = navigationStack.Peek();
				}
				if(shortcut != null && shortcut.HasViewParameters) {
					NavigateTo(shortcut);
				}
				else {
					Window.SetView(null);
				}
			}
			else {
				Window.Close();
			}
		}
#if DebugTest
		public virtual void RefreshActionsState() {
#else
		protected virtual void RefreshActionsState() {
#endif
			navigateBackAction.Active.SetItemValue("Main window", IsNavigationAllowed());
			navigateForwardAction.Active.SetItemValue("Main window", IsNavigationAllowed());
			navigateBackAction.Enabled.SetItemValue("Can back", navigationHistory.CanBack);
			navigateForwardAction.Enabled.SetItemValue("Can forward", navigationHistory.CanForward);
			UpdateActionsItems();
		}
		protected void UpdateActionsItems() {
			List<HistoryItem<ViewShortcut>> list = new List<HistoryItem<ViewShortcut>>();
			foreach(HistoryItem<ViewShortcut> item in NavigationHistory) {
				list.Add(item);
			}
			int currentIndex = NavigationHistory.CurrentPositionIndex;
			navigateBackAction.Items.Clear();
			int count = 0;
			while(++count <= HistoryDepth && NavigationHistory.CanBack) {
				NavigationHistory.Back();
				ChoiceActionItem item = new ChoiceActionItem(list[currentIndex - count].Caption, list[currentIndex - count].Item);
				item.ImageName = list[currentIndex - count].ImageName;
				navigateBackAction.Items.Add(item);
			}
			NavigationHistory.CurrentPositionIndex = currentIndex;
			navigateForwardAction.Items.Clear();
			count = 0;
			while(++count <= HistoryDepth && NavigationHistory.CanForward) {
				NavigationHistory.Forward();
				ChoiceActionItem item = new ChoiceActionItem(list[currentIndex + count].Caption, list[currentIndex + count].Item);
				item.ImageName = list[currentIndex + count].ImageName;
				navigateForwardAction.Items.Add(item);
			}
			NavigationHistory.CurrentPositionIndex = currentIndex;
		}
		protected internal virtual bool DoNavigate(ViewShortcut shortcut, int? historyItemIndex) {
			SecuritySystem.ReloadPermissions();
			isNavigating = true;
			bool setViewResult = false;
			try {
				setViewResult = Window.SetView(Application.ProcessShortcut(shortcut), null);
			}
			finally {
				if(setViewResult) {
					if(historyItemIndex.HasValue) {
						navigationHistory.CurrentPositionIndex = historyItemIndex.Value;
					}
					else {
						navigationHistory.SetCurrentPosition(shortcut);
					}
				}
				isNavigating = false;
			}
			RefreshActionsState();
			return setViewResult;
		}
		protected void NavigateTo(ViewShortcut shortcut) {
			if(DoNavigate(shortcut, null)) {
				DeleteLastItemIfItNeeds();
			}
		}
		protected void NavigateTo(int historyItemIndex) {
		   if(DoNavigate(navigationHistory.GetCurrentPositionByIndex(historyItemIndex).Item, historyItemIndex)){
			   DeleteLastItemIfItNeeds();
		   }
		}
		protected override void OnDeactivated() {
			if(Window.View != null) {
				Window.View.Closed -= new EventHandler(view_Closed);
			}
			Window.ViewChanging -= new EventHandler<ViewChangingEventArgs>(window_ViewChanging);
			Window.ViewChanged -= new EventHandler<ViewChangedEventArgs>(window_ViewChanged);
			navigationHistory.Clear();
			if(objectSpace != null) {
				objectSpace.Committing -= new EventHandler<CancelEventArgs>(objectSpace_Committing);
				objectSpace.Committed -= new EventHandler(objectSpace_Committed);
				objectSpace.Disposed -= new EventHandler(objectSpace_Disposed);
			}
			objectSpace = null;
			base.OnDeactivated();
		}
		protected override void OnFrameAssigned() {
			base.OnFrameAssigned();
			Active["SupportViewNavigationHistory"] = (Application.ShowViewStrategy != null) && Application.ShowViewStrategy.SupportViewNavigationHistory;
		}
		protected override void OnActivated() {
			base.OnActivated();
			Window.ViewChanging += new EventHandler<ViewChangingEventArgs>(window_ViewChanging);
			Window.ViewChanged += new EventHandler<ViewChangedEventArgs>(window_ViewChanged);
			AddToHistory(Window.View);
			RefreshActionsState();
		}
		protected virtual void ProcessDeletedObjects(IObjectSpace objectSpace, ICollection deletedObjects) {
			HistoryItem<ViewShortcut> currentItem = navigationHistory.CurrentPosition;
			if(IsNavigationAllowed()) {
				foreach(Object obj in deletedObjects) {
					for(int i = navigationHistory.Count - 1; i > -1; i--) {
						navigationHistory.CurrentPositionIndex = i;
						ViewShortcut itemShortcut = navigationHistory.CurrentPosition.Item;
						if(!string.IsNullOrEmpty(itemShortcut.ObjectKey)) {
							Object objectKey = objectSpace.GetObjectKey(itemShortcut.ObjectClass, itemShortcut.ObjectKey);
							ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(obj.GetType());
							if((itemShortcut.ObjectClass == typeInfo.Type) &&
								(itemShortcut.ObjectKey != null) &&
								objectKey.Equals(objectSpace.GetKeyValue(obj))) {
								RemoveShortcutFromStack(navigationHistory.CurrentPosition.Item);
								navigationHistory.DeleteCurrentItem();
							}
						}
					}
				}
			}
			if(navigationHistory.IndexOf(currentItem.Item) != -1) {
				navigationHistory.SetCurrentPosition(currentItem.Item);
			}
			else {
				navigationHistory.CurrentPositionIndex = navigationHistory.Count - 1;
			}
		}
		protected virtual NavigationHistory<ViewShortcut> CreateNavigationHistory() {
			return new NavigationHistory<ViewShortcut>(HistoryDepth);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(objectSpace != null) {
					objectSpace.Committing -= new EventHandler<CancelEventArgs>(objectSpace_Committing);
					objectSpace.Committed -= new EventHandler(objectSpace_Committed);
					objectSpace.Disposed -= new EventHandler(objectSpace_Disposed);
				}
			}
		}
		public ViewNavigationController() {
			navigationHistory = CreateNavigationHistory();
			navigationStack = new SizeLimitedStack<ViewShortcut>(HistoryDepth);
			InitializeComponent();
			RegisterActions(components);
		}
		public SingleChoiceAction NavigateBackAction {
			get { return navigateBackAction; }
		}
		public SingleChoiceAction NavigateForwardAction {
			get { return navigateForwardAction; }
		}
	}
}
