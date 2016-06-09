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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Templates.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.Bars.ActionControls {
	public class BarButtonItemSingleChoiceActionControl : BarItemSingleChoiceActionControl<BarButtonItem> {
		private SingleChoiceActionItemType itemType;
		private bool canRebuildItems = false;
		private Dictionary<BarItem, ChoiceActionItem> actionItemByBarItem = new Dictionary<BarItem, ChoiceActionItem>();
		private Dictionary<ChoiceActionItem, BarButtonItem> barItemByActionItem = new Dictionary<ChoiceActionItem, BarButtonItem>();
		private void BarItem_ItemClick(object sender, ItemClickEventArgs e) {
			if(!BarItem.ActAsDropDown) {
				RaiseExecute(null);
			}
		}
		private void PopupMenu_BeforePopup(object sender, CancelEventArgs e) {
			if(!canRebuildItems) {
				canRebuildItems = true;
				RebuildItems();
			}
		}
		private void OnChoiceBarItemClick(object sender, ItemClickEventArgs e) {
			ChoiceActionItem actionItem = actionItemByBarItem[e.Item];
			RaiseExecute(actionItem);
		}
		private PopupMenu CreatePopupMenu() {
			PopupMenu popupMenu = new PopupMenu();
			if(BarItem.Manager is RibbonBarManager) {
				popupMenu.Ribbon = ((RibbonBarManager)BarItem.Manager).Ribbon;
			}
			else {
				popupMenu.Manager = BarItem.Manager;
			}
			return popupMenu;
		}
		private PopupMenu PopupMenu {
			get { return (PopupMenu)BarItem.DropDownControl; }
		}
		protected override void OnEndInit() {
			base.OnEndInit();
			BarItem.ButtonStyle = BarButtonStyle.DropDown;
			BarItem.AllowDrawArrow = true;
			BarItem.DropDownControl = CreatePopupMenu();
			BarItem.ItemClick += BarItem_ItemClick;
			PopupMenu.BeforePopup += PopupMenu_BeforePopup;
		}
		protected override void SetShowItemsOnClickCore(bool value) {
			BarItem.ActAsDropDown = value;
		}
		protected override void BeginUpdate() {
			BarItem.BeginUpdate();
			PopupMenu.BeginUpdate();
		}
		protected override void EndUpdate() {
			PopupMenu.EndUpdate();
			BarItem.CancelUpdate();
		}
		protected override void ClearItems() {
			PopupMenu.ClearLinks();
			foreach(BarButtonItem barItem in barItemByActionItem.Values) {
				barItem.ItemClick -= OnChoiceBarItemClick;
				barItem.Dispose();
			}
			actionItemByBarItem.Clear();
			barItemByActionItem.Clear();
		}
		protected override void BuildItems() {
			if(!canRebuildItems && !ChoiceActionItemsHelper.HasAnyItemShortcut(ChoiceActionItems)) {
				return;
			}
			CreateSubItems(PopupMenu.ItemLinks, ChoiceActionItems);
		}
		private void CreateSubItems(BarItemLinkCollection itemLinks, ChoiceActionItemCollection items) {
			foreach(ChoiceActionItem actionItem in items) {
				BarButtonItem barItem = new BarButtonItem();
				actionItemByBarItem.Add(barItem, actionItem);
				barItemByActionItem.Add(actionItem, barItem);
				barItem.Manager = BarItem.Manager;
				itemLinks.Add(barItem, actionItem.BeginGroup);
				barItem.Visibility = actionItem.Active ? BarItemVisibility.Always : BarItemVisibility.Never;
				barItem.Enabled = actionItem.Enabled;
				barItem.Caption = actionItem.Caption;
				barItem.Hint = actionItem.ToolTip;
				barItem.Glyph = GetImage(actionItem.ImageName);
				barItem.LargeGlyph = GetLargeImage(actionItem.ImageName);
				barItem.ItemShortcut = ShortcutHelper.ParseBarShortcut(actionItem.Shortcut);
				barItem.ItemClick += OnChoiceBarItemClick;
				if(ItemType == SingleChoiceActionItemType.ItemIsMode) {
					barItem.ButtonStyle = actionItem.Items.Count > 0 ? BarButtonStyle.CheckDropDown : BarButtonStyle.Check;
					barItem.AllowAllUp = true;
					barItem.GroupIndex = GroupIndex;
				}
				else {
					barItem.ButtonStyle = actionItem.Items.Count > 0 ? BarButtonStyle.DropDown : BarButtonStyle.Default;
				}
			}
			foreach(ChoiceActionItem actionItem in items) {
				if(actionItem.Items.Count > 0) {
					PopupMenu popupMenu = CreatePopupMenu();
					barItemByActionItem[actionItem].DropDownControl = popupMenu;
					CreateSubItems(popupMenu.ItemLinks, actionItem.Items);
				}
			}
		}
		protected override void UpdateCore(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo) {
			foreach(KeyValuePair<object, ChoiceActionItemChangesType> pair in itemsChangedInfo) {
				ChoiceActionItem actionItem = pair.Key as ChoiceActionItem;
				BarButtonItem barItem;
				if(actionItem != null && barItemByActionItem.TryGetValue(actionItem, out barItem)) {
					Update(barItem, actionItem, pair.Value);
				}
			}
		}
		private void Update(BarButtonItem barItem, ChoiceActionItem actionItem, ChoiceActionItemChangesType changesType) {
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Active)) {
				barItem.Visibility = actionItem.Active ? BarItemVisibility.Always : BarItemVisibility.Never;
			}			
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Enabled)) {
				barItem.Enabled = actionItem.Enabled;
			}
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Caption)) {
				barItem.Caption = actionItem.Caption;
			}
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.ToolTip)) {
				barItem.Hint = actionItem.ToolTip;
			}
			if(ChangesTypeContains(changesType, ChoiceActionItemChangesType.Image)) {
				barItem.Glyph = GetImage(actionItem.ImageName);
				barItem.LargeGlyph = GetLargeImage(actionItem.ImageName);
			}
		}
		protected override void SetSelectedItemCore(ChoiceActionItem selectedItem) {
			if(ItemType == SingleChoiceActionItemType.ItemIsMode) {
				BarButtonItem barItem;
				if(selectedItem != null && barItemByActionItem.TryGetValue(selectedItem, out barItem)) {
					barItem.Down = true;
				}
				else {
					foreach(BarButtonItem barButtonItem in barItemByActionItem.Values) {
						if(barButtonItem.Down) {
							barButtonItem.Down = false;
						}
					}
				}
			}
		}
		public BarButtonItemSingleChoiceActionControl() { }
		public BarButtonItemSingleChoiceActionControl(string actionId, BarButtonItem item) : base(actionId, item) { }
		[Description("Specifies whether the Single Choice Action's items represent a mode or an operation.")]
		public SingleChoiceActionItemType ItemType {
			get { return itemType; }
			set {
				if(ItemType != value) {
					CheckCanSet("ItemType");
					itemType = value;
				}
			}
		}
	}
}
