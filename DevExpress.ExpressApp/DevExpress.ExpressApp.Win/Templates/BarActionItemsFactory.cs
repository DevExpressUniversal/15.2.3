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
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates {
	public enum BarActionItemsFactoryMode { Default, Menu }
	public interface IBarActionItemsFactoryProvider {
		BarActionItemsFactory CreateBarActionItemsFactory(BarManager barManager);
	}
	public class BarActionItemsFactoryProvider : IBarActionItemsFactoryProvider {
		private static Dictionary<BarManager, BarActionItemsFactory> barManagerToBarActionItemsFactoryMap = new Dictionary<BarManager, BarActionItemsFactory>();
		private static IBarActionItemsFactoryProvider instance = new BarActionItemsFactoryProvider();
		private static void barManager_Disposed(object sender, EventArgs e) {
			BarManager barManager = (BarManager)sender;
			UnsubscribeFromBarManager(barManager);
			BarActionItemsFactory factory;
			if(barManagerToBarActionItemsFactoryMap.TryGetValue(barManager, out factory)) {
				factory.Dispose();
				barManagerToBarActionItemsFactoryMap.Remove(barManager);
			}
		}
		private static void SubscribeToBarManager(BarManager barManager) {
			barManager.Disposed += new EventHandler(barManager_Disposed);
		}
		private static void UnsubscribeFromBarManager(BarManager barManager) {
			barManager.Disposed -= new EventHandler(barManager_Disposed);
		}
		public static BarActionItemsFactory GetBarActionItemsFactory(BarManager barManager) {
			BarActionItemsFactory factory;
			if(!barManagerToBarActionItemsFactoryMap.TryGetValue(barManager, out factory)) {
				factory = Instance.CreateBarActionItemsFactory(barManager);
				SubscribeToBarManager(barManager);
				barManagerToBarActionItemsFactoryMap.Add(barManager, factory);
			}
			return factory;
		}
		public static void ReplaceManager(BarManager oldManager, BarManager newManager) {
			if(oldManager != newManager) {
				UnsubscribeFromBarManager(oldManager);
				SubscribeToBarManager(newManager);
				BarActionItemsFactory factory;
				if(barManagerToBarActionItemsFactoryMap.TryGetValue(oldManager, out factory)) {
					factory.Manager = newManager;
					barManagerToBarActionItemsFactoryMap[newManager] = factory;
					barManagerToBarActionItemsFactoryMap.Remove(oldManager);
				}
				newManager.DockManager = oldManager.DockManager; 
			}
		}
		public static void Reset() {
			foreach(BarActionItemsFactory factory in Factories) {
				UnsubscribeFromBarManager(factory.Manager);
				barManagerToBarActionItemsFactoryMap.Remove(factory.Manager);
				factory.Dispose();
			}
		}
		public BarActionItemsFactory CreateBarActionItemsFactory(BarManager barManager) {
			return new BarActionItemsFactory(barManager);
		}
		public static IBarActionItemsFactoryProvider Instance {
			get { return instance; }
			set { instance = value; }
		}
		public static IList<BarActionItemsFactory> Factories {
			get {
				List<BarActionItemsFactory> list = new List<BarActionItemsFactory>();
				list.AddRange(barManagerToBarActionItemsFactoryMap.Values);
				return list;
			}
		}
	}
	public class BarActionItemsFactory : IDisposable {
		private BarManager manager;
		private List<BarActionBaseItem> actionItems;
		private BarActionItemsFactoryMode mode;
		private void actionItem_ControlChanged(object sender, EventArgs e) {
			if(BarItemChanged != null) {
				BarItemChanged(this, new BarItemChangedEventArgs((BarActionBaseItem)sender));
			}
		}
		private void singleChoiceAction_ItemTypeChanged(object sender, EventArgs e) {
			OnSingleChoiceActionChanged((SingleChoiceAction)sender);
		}
		private void singleChoiceAction_BehaviorChanged(object sender, ChoiceActionBehaviorChangedEventArgs e) {
			if(e.ChangedPropertyType == ChoiceActionBehaviorChangedType.ShowItemsOnClick) {
				OnSingleChoiceActionChanged((SingleChoiceAction)sender);
			}
		}
		private void OnSingleChoiceActionChanged(SingleChoiceAction action) {
			BarActionBaseItem changedActionItem = FindActionItem(action);
			if(changedActionItem != null) {
				action.ItemTypeChanged -= new EventHandler(singleChoiceAction_ItemTypeChanged);
				action.BehaviorChanged -= new EventHandler<ChoiceActionBehaviorChangedEventArgs>(singleChoiceAction_BehaviorChanged);
				changedActionItem.ControlChanged -= new EventHandler<EventArgs>(actionItem_ControlChanged);
				changedActionItem.Dispose();
				actionItems.Remove(changedActionItem);
				if(ActionItemChanged != null) {
					ActionItemChanged(this, new ActionItemChangedEventArgs(action));
				}
			}
		}
		private BarActionBaseItem FindActionItem(ActionBase action) {
			foreach(BarActionBaseItem actionItem in actionItems) {
				if(actionItem.Action == action) {
					return actionItem;
				}
			}
			return null;
		}
		private void UpdateItemName(BarItem result) {
			int i = 0;
			string name = result.Name;
			while(IsDuplicated(result)) {
				i++;
				result.Name = name + "_" + i.ToString();
			}
		}
		private bool IsDuplicated(BarItem item) {
			foreach(BarItem barItem in Manager.Items) {
				if(item != barItem && item.Name == barItem.Name)
					return true;
			}
			return false;
		}
		protected virtual BarActionBaseItem CreateActionItem(ActionBase action) {
			BarActionBaseItem result = null;
			if(action is SimpleAction) {
				result = GetSimpleActionItem((SimpleAction)action);
			}
			else if(action is PopupWindowShowAction) {
				result = GetPopupWindowShowActionItem((PopupWindowShowAction)action);
			}
			else if(action is SingleChoiceAction) {
				result = CreateSingleChoiceActionItem((SingleChoiceAction)action);
			}
			else if(action is ParametrizedAction) {
				result = CreateParametrizedActionItem(action);
			}
			else if(action is ActionUrl) {
				result = CreateUrlActionItem(action);
			}
			return result;
		}
		protected virtual BarActionBaseItem GetSimpleActionItem(SimpleAction action) {
			return new BarSimpleButtonItem(action, Manager);
		}
		protected virtual BarActionBaseItem GetPopupWindowShowActionItem(PopupWindowShowAction action) {
			return new BarSimpleButtonItem(action, Manager);
		}
		protected virtual BarActionBaseItem CreateSingleChoiceActionItem(SingleChoiceAction singleChoiceAction) {
			BarActionBaseItem result = null;
			if(singleChoiceAction.ItemType == SingleChoiceActionItemType.ItemIsMode) {
				if(singleChoiceAction.IsHierarchical()) {
					result = new BarSingleChoiceOperationActionItem(singleChoiceAction, Manager, BarButtonStyle.Check);
				}
				else if(mode == BarActionItemsFactoryMode.Default) {
					result = new BarSingleChoiceComboBoxActionItem(singleChoiceAction, Manager);
				}
				else if(mode == BarActionItemsFactoryMode.Menu) {
					result = new DropDownModeActionItem(singleChoiceAction, Manager);
				}
			}
			else {
				result = new BarSingleChoiceOperationActionItem(singleChoiceAction, Manager, BarButtonStyle.Default);
			}
			return result;
		}
		protected virtual BarActionBaseItem CreateParametrizedActionItem(ActionBase action) {
			return new ParametrizedActionItem((ParametrizedAction)action, Manager);
		}
		protected virtual BarActionBaseItem CreateUrlActionItem(ActionBase action) {
			return new UrlItem((ActionUrl)action, Manager);
		}
		protected virtual void OnManagerChanged() {
			foreach(BarActionBaseItem actionItem in ActionItems) {
				actionItem.Manager = Manager;
			}
		}
		protected virtual void OnCustomizeActionControl(CustomizeActionControlEventArgs args) {
			if(CustomizeActionControl != null) {
				CustomizeActionControl(this, args);
			}
		}
		public BarActionItemsFactory(BarManager barManager) {
			Guard.ArgumentNotNull(barManager, "barManager");
			this.manager = barManager;
			actionItems = new List<BarActionBaseItem>();
		}
		public BarActionBaseItem GetBarItem(ActionBase action) {
			Guard.ArgumentNotNull(action, "action");
			BarActionBaseItem actionItem = FindActionItem(action);
			if(actionItem == null) {
				BarActionBaseItem defaultActionItem = CreateActionItem(action);
				CustomizeActionControlEventArgs args = new CustomizeActionControlEventArgs(defaultActionItem, action);
				OnCustomizeActionControl(args);
				actionItem = args.ActionControl;
				actionItems.Add(actionItem);
				BarItem barItem = actionItem.Control;
				UpdateItemName(barItem);
				barItem.Tag = action;
				SingleChoiceAction singleChoiceAction = action as SingleChoiceAction;
				if(singleChoiceAction != null) {
					singleChoiceAction.ItemTypeChanged -= new EventHandler(singleChoiceAction_ItemTypeChanged);
					singleChoiceAction.ItemTypeChanged += new EventHandler(singleChoiceAction_ItemTypeChanged);
					singleChoiceAction.BehaviorChanged -= new EventHandler<ChoiceActionBehaviorChangedEventArgs>(singleChoiceAction_BehaviorChanged);
					singleChoiceAction.BehaviorChanged += new EventHandler<ChoiceActionBehaviorChangedEventArgs>(singleChoiceAction_BehaviorChanged);
				}
				actionItem.ControlChanged += new EventHandler<EventArgs>(actionItem_ControlChanged);
			}
			return actionItem;
		}
		public ActionBase FindActionByItem(BarItem barItem) {
			return barItem.Tag as ActionBase;
		}
		public virtual void Dispose() {
			if(actionItems != null) {
				foreach(BarActionBaseItem actionItem in actionItems) {
					SingleChoiceAction singleChoiceAction = actionItem.Action as SingleChoiceAction;
					if(singleChoiceAction != null) {
						singleChoiceAction.ItemTypeChanged -= new EventHandler(singleChoiceAction_ItemTypeChanged);
						singleChoiceAction.BehaviorChanged -= new EventHandler<ChoiceActionBehaviorChangedEventArgs>(singleChoiceAction_BehaviorChanged);
					}
					actionItem.ControlChanged -= new EventHandler<EventArgs>(actionItem_ControlChanged);
					actionItem.Dispose();
				}
				actionItems.Clear();
				actionItems = null;
			}
		}
		public BarManager Manager {
			get { return manager; }
			set {
				if(manager != value) {
					manager = value;
					OnManagerChanged();
				}
			}
		}
		public BarActionItemsFactoryMode Mode {
			get { return mode; }
			set { mode = value; }
		}
		public List<BarActionBaseItem> ActionItems {
			get { return actionItems; }
		}
		public static event EventHandler<CustomizeActionControlEventArgs> CustomizeActionControl;
		public event EventHandler<BarItemChangedEventArgs> BarItemChanged;
		public event EventHandler<ActionItemChangedEventArgs> ActionItemChanged;
	}
	public class ActionItemChangedEventArgs : EventArgs {
		private ActionBase action;
		public ActionItemChangedEventArgs(ActionBase action) {
			this.action = action;
		}
		public ActionBase Action {
			get { return action; }
		}
	}
	public class BarItemChangedEventArgs : EventArgs {
		private BarActionBaseItem actionItem;
		public BarItemChangedEventArgs(BarActionBaseItem actionItem) {
			this.actionItem = actionItem;
		}
		public BarActionBaseItem ActionItem {
			get { return actionItem; }
		}
	}
}
