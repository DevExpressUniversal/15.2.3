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
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers.Items {
	public abstract class BarSingleChoiceActionItemBase : BarActionBaseItem
#if DebugTest
, DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable
#endif
 {
		private Dictionary<ChoiceActionItem, BarChoiceActionItem> choiceActionItemToWrapperMap;
		private BarButtonStyle itemStyle = BarButtonStyle.Default;
		private SingleChoiceActionHelper helper;
		protected new SingleChoiceAction Action {
			get { return (SingleChoiceAction)base.Action; }
		}
		protected internal Dictionary<ChoiceActionItem, BarChoiceActionItem> ChoiceActionItemToWrapperMap {
			get { return choiceActionItemToWrapperMap; }
		}
		protected virtual void UpdateBarItem(ChoiceActionBehaviorChangedType changedType) {
			if(changedType == ChoiceActionBehaviorChangedType.ImageMode) {
				SetImage(ImageLoader.Instance.GetImageInfo(Action.ImageName));
			}
		}
		private void Action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			SynchronizeWithAction();
		}
		private void Action_SelectedItemChanged(object sender, EventArgs e) {
			UpdateSelectedItem();
		}
		private void Action_BehaviorChanged(object sender, ChoiceActionBehaviorChangedEventArgs e) {
			UpdateBarItem(e.ChangedPropertyType);
		}
		private void UpdateSelectedItem() {
			if(Control != null && Action.DefaultItemMode == DefaultItemMode.LastExecutedItem) {
				SetCaption(Action.Caption);
				SetToolTip(Action.GetTotalToolTip());
				SetImage(ImageLoader.Instance.GetImageInfo(Action.ImageName));
			}
			if(Control != null && itemStyle == BarButtonStyle.Check) {
				Control.BeginUpdate();
				if(Action.SelectedItem != null && ChoiceActionItemToWrapperMap.ContainsKey(Action.SelectedItem)) {
					DropDownMenuButtonItem barButtonItem = ChoiceActionItemToWrapperMap[Action.SelectedItem] as DropDownMenuButtonItem;
					if(barButtonItem != null) {
						barButtonItem.BarButtonItem.Down = true;
					}
				}
				Control.EndUpdate();
			}
		}
		private void ClearItems() {
			BeginUpdate();
			try {
				foreach(BarChoiceActionItem item in ChoiceActionItemToWrapperMap.Values) {
					item.Dispose();
				}
				ChoiceActionItemToWrapperMap.Clear();
			}
			finally {
				EndUpdate();
			}
		}
		protected void RebuildItems() {
			if(Control == null) return;
			ClearItems();
			if(Action.Active) {
				BeginUpdate();
				try {
					RebuildItemsCore();
				}
				finally {
					EndUpdate();
				}
			}
		}
		protected abstract void RebuildItemsCore();
		protected virtual void BeginUpdate() { }
		protected virtual void EndUpdate() { }
		protected virtual void CreateSubItems(BarItemLinkCollection itemLinks, ChoiceActionItemCollection items) {
			foreach(ChoiceActionItem actionItem in items) {
				if(actionItem.Active) {
					BarChoiceActionItem barItem = null;
					if(actionItem.Items.Count > 0) {
						barItem = CreateDropDownMenuSubItem(actionItem);
					}
					else {
						barItem = CreateDropDownMenuButtonItem(actionItem);
					}
					ChoiceActionItemToWrapperMap.Add(actionItem, barItem);
					itemLinks.Add(barItem.Control, actionItem.BeginGroup);
					if(actionItem.Items.Count > 0) {
						CreateSubItems(((BarCustomContainerItem)barItem.Control).ItemLinks, actionItem.Items);
					}
				}
			}
		}
		protected virtual BarChoiceActionItem CreateDropDownMenuSubItem(ChoiceActionItem actionItem) {
			return new DropDownMenuSubItem(Action, actionItem, Manager);
		}
		protected virtual BarChoiceActionItem CreateDropDownMenuButtonItem(ChoiceActionItem actionItem) {
			BarChoiceActionItem barItem = new DropDownMenuButtonItem(Action, actionItem, Manager);
			barItem.Control.Tag = EasyTestTagHelper.FormatTestAction(Action.Caption + '.' + actionItem.GetItemPath());
			if(itemStyle == BarButtonStyle.Check) {
				DropDownMenuButtonItem barButtonItem = (DropDownMenuButtonItem)barItem;
				barButtonItem.BarButtonItem.ButtonStyle = BarButtonStyle.Check;
				barButtonItem.BarButtonItem.GroupIndex = Action.Id.GetHashCode();
				if(Action.SelectedItem == actionItem) {
					barButtonItem.BarButtonItem.Down = true;
				}
			}
			return barItem;
		}
		public void ProcessDefaultClick() {
			if(IsItemClickEnable) {
				ResetLastItemClickTime();
				BindingHelper.EndCurrentEdit(Form.ActiveForm);
				if(IsConfirmed()) {
					ChoiceActionItem defaultActionItem = helper.FindDefaultItem();
					PostLastEditAndExecuteAction(defaultActionItem);
				}
			}
		}
		private void PostLastEditAndExecuteAction(ChoiceActionItem item) {
			BindingHelper.EndCurrentEdit(Form.ActiveForm);
			Action.DoExecute(item);
		}
		protected override void OnManagerChanged() {
			base.OnManagerChanged();
			if(Manager != null) {
				RebuildItems();
			}
		}
		protected override void UnsubscribeFromActionItemsChangedEvent() {
			base.UnsubscribeFromActionItemsChangedEvent();
			if(Action != null) {
				Action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
			}
		}
		protected override void SetCaption(string caption) {
			Control.Caption = helper.GetActionCaption(caption);
		}
		protected override void SetImage(ImageInfo imageInfo) {
			imageInfo = helper.GetActionImageInfo(imageInfo);
			if(!imageInfo.IsEmpty) {
				Control.Glyph = imageInfo.Image;
			}
			ImageInfo largeImageInfo = helper.GetActionImageInfo(imageInfo, true);
			if(!largeImageInfo.IsEmpty) {
				Control.LargeGlyph = largeImageInfo.Image;
			}
		}
		protected override void SetVisible(bool visible) {
			RebuildItems();
			base.SetVisible(visible);
		}
		protected override void SynchronizeWithActionCore() {
			RebuildItems();
			base.SynchronizeWithActionCore();
		}
		public override void Dispose() {
			try {
				ClearItems();
				choiceActionItemToWrapperMap = null;
				if(Action != null) {
					Action.SelectedItemChanged -= new EventHandler(Action_SelectedItemChanged);
					Action.BehaviorChanged -= new EventHandler<ChoiceActionBehaviorChangedEventArgs>(Action_BehaviorChanged);
				}
				helper = null;
			}
			finally {
				base.Dispose();
			}
		}
		public BarSingleChoiceActionItemBase(SingleChoiceAction singleChoiceAction, BarManager manager)
			: base(singleChoiceAction, manager) {
			choiceActionItemToWrapperMap = new Dictionary<ChoiceActionItem, BarChoiceActionItem>();
			helper = new SingleChoiceActionHelper(singleChoiceAction);
			Action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(Action_ItemsChanged);
			Action.SelectedItemChanged += new EventHandler(Action_SelectedItemChanged);
			Action.BehaviorChanged += new EventHandler<ChoiceActionBehaviorChangedEventArgs>(Action_BehaviorChanged);
		}
		public BarButtonStyle ItemStyle {
			get {
				return itemStyle;
			}
			set {
				itemStyle = value;
			}
		}
#if DebugTest
		private BarItem GetItemByPath(string itemPath) {
			ChoiceActionItem actionItem = ((SingleChoiceAction)Action).FindItemByCaptionPath(itemPath);
			if(actionItem != null && ChoiceActionItemToWrapperMap.ContainsKey(actionItem)) {
				return ChoiceActionItemToWrapperMap[actionItem].Control;
			}
			return null;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemVisible(string itemPath) {
			BarItem barItem = GetItemByPath(itemPath);
			return barItem == null ? false : BarItemUnitTestHelper.IsBarItemVisible(barItem);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemEnabled(string itemPath) {
			BarItem barItem = GetItemByPath(itemPath);
			return barItem == null ? false : barItem.Enabled;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemBeginsGroup(string itemPath) {
			BarItem barItem = GetItemByPath(itemPath);
			return barItem == null ? false : barItem.Links[0].BeginGroup;
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemImageVisible(string itemPath) {
			BarItem barItem = GetItemByPath(itemPath);
			return barItem == null ? false : BarItemUnitTestHelper.IsBarItemImageVisible(barItem);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemSelected(string itemPath) {
			if(Action.ItemType == SingleChoiceActionItemType.ItemIsOperation) {
				return false;
			}
			else {
				BarItem barItem = GetItemByPath(itemPath);
				return barItem == null ? false : (barItem is BarButtonItem && ((BarButtonItem)barItem).Down);
			}
		}
#endif
	}
}
