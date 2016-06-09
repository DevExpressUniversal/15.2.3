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
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers.Items {
	public class DropDownModeActionItem : BarActionBaseItem
#if DebugTest
, DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable
#endif
 {
		private Dictionary<ChoiceActionItem, DropDownMenuButtonItem> choiceActionItemItems;
		private bool isSelectedItemUpdating;
		private void RefreshControlItems() {
			if(control != null) {
				Control.BeginUpdate();
				try {
					ClearDropDown();
					FillDropDown();
				}
				finally {
					Control.EndUpdate();
				}
			}
		}
		private void FillDropDown() {
			if(Action.Active) {
				foreach(ChoiceActionItem actionItem in Action.Items) {
					if(actionItem.Active) {
						DropDownMenuButtonItem item = CreateDropDownMenuButtonItem(Action, actionItem, Manager);
						choiceActionItemItems[actionItem] = item;
						item.BarButtonItem.ButtonStyle = BarButtonStyle.Check;
						item.BarButtonItem.GroupIndex = Action.Id.GetHashCode();
						Control.AddItem(item.BarButtonItem).BeginGroup = actionItem.BeginGroup;
						if(Action.SelectedItem == actionItem) {
							item.BarButtonItem.Down = true;
						}
						item.BarButtonItem.DownChanged += new ItemClickEventHandler(BarButtonItem_DownChanged);
					}
				}
				Control.Hint = Action.GetTotalToolTip();
			}
		}
		private void ClearDropDown() {
			foreach(DropDownMenuButtonItem item in choiceActionItemItems.Values) {
				item.BarButtonItem.DownChanged -= new ItemClickEventHandler(BarButtonItem_DownChanged);
				item.Dispose();
			}
			choiceActionItemItems.Clear();
		}
		private void UpdateSelectedItem() {
			if(!isSelectedItemUpdating && control != null && Action.SelectedItem != null && choiceActionItemItems.ContainsKey(Action.SelectedItem)) {
				isSelectedItemUpdating = true;
				Control.BeginUpdate();
				try {
					choiceActionItemItems[Action.SelectedItem].BarButtonItem.Down = true;
				}
				finally {
					isSelectedItemUpdating = false;
					Control.EndUpdate();
				}
			}
		}
		private void BarButtonItem_DownChanged(object sender, ItemClickEventArgs e) {
			UpdateSelectedItem();
		}
		private void Action_Changed(object sender, ItemsChangedEventArgs e) {
			RefreshControlItems();
		}
		private void Action_SelectedItemChanged(object sender, EventArgs e) {
			UpdateSelectedItem();
		}
		private void Control_Popup(object sender, EventArgs e) {
			BindingHelper.EndCurrentEdit(Form.ActiveForm);
		}
		private new SingleChoiceAction Action {
			get { return (SingleChoiceAction)base.Action; }
		}
		protected override void SetVisible(bool visible) {
			base.SetVisible(visible);
			RefreshControlItems();
		}
		protected override BarItem CreateControlCore() {
			BarSubItem control = new BarSubItem();
			control.Popup += new EventHandler(Control_Popup);
			control.Caption = Action.Caption;
			return control;
		}
		protected virtual DropDownMenuButtonItem CreateDropDownMenuButtonItem(SingleChoiceAction action, ChoiceActionItem actionItem, BarManager barManager) {
			return new DropDownMenuButtonItem(action, actionItem, barManager);
		}
		protected override ActionItemPaintStyle GetDefaultPaintStyle() {
			return ActionItemPaintStyle.CaptionAndImage;
		}
		protected override void UnsubscribeFromActionItemsChangedEvent() {
			base.UnsubscribeFromActionItemsChangedEvent();
			if(Action != null) {
				Action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(Action_Changed);
			}
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			RefreshControlItems();
		}
		public override void Dispose() {
			try {
				if(Control != null) {
					Control.Popup -= new EventHandler(Control_Popup);
					ClearDropDown();
				}
				if(Action != null) {
					Action.SelectedItemChanged -= new EventHandler(Action_SelectedItemChanged);
				}
			}
			finally {
				base.Dispose();
			}
		}
		public DropDownModeActionItem(SingleChoiceAction singleChoiceAction, BarManager manager)
			: base(singleChoiceAction, manager) {
			choiceActionItemItems = new Dictionary<ChoiceActionItem, DropDownMenuButtonItem>();
			Action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(Action_Changed);
			Action.SelectedItemChanged += new EventHandler(Action_SelectedItemChanged);
		}
		public new BarSubItem Control {
			get { return (BarSubItem)base.Control; }
		}
#if DebugTest
		#region ISingleChoiceActionItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemVisible(string itemPath) {
			BarButtonItem barItem = GetItemByPath(itemPath);
			return barItem == null ? false : BarItemUnitTestHelper.IsBarItemVisible(barItem);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemEnabled(string itemPath) {
			BarButtonItem barItem = GetItemByPath(itemPath);
			return barItem == null ? false : barItem.Enabled;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemBeginsGroup(string itemPath) {
			ChoiceActionItem item = Action.FindItemByCaptionPath(itemPath);
			return item == null ? false : item.BeginGroup;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemImageVisible(string itemPath) {
			BarButtonItem barItem = GetItemByPath(itemPath);
			return barItem == null ? false : BarItemUnitTestHelper.IsBarItemImageVisible(barItem);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemSelected(string itemPath) {
			BarButtonItem barItem = GetItemByPath(itemPath);
			return barItem == null ? false : barItem.Down;
		}
		private BarButtonItem GetItemByPath(string itemPath) {
			ChoiceActionItem item = Action.FindItemByCaptionPath(itemPath);
			if(item != null && choiceActionItemItems.ContainsKey(item)) {
				return choiceActionItemItems[item].BarButtonItem;
			}
			return null;
		}
		#endregion
#endif
	}
}
