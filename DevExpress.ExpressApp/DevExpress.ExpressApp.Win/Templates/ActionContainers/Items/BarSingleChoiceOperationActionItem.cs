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
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Templates.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers.Items {
	public class BarSingleChoiceOperationActionItem : BarSingleChoiceActionItemBase {
		private PopupMenu popupMenu;
		private bool canRebuildItems;
		protected PopupMenu PopupMenu { get { return popupMenu; } }
		protected virtual void ProcessDefaultClick(BarItemLink barItemLink) {
			if(!Action.ShowItemsOnClick) {
				ProcessDefaultClick();
			}
			else {
				if(barItemLink != null) {
					((BarButtonItemLink)barItemLink).ShowPopup();
				}
			}
		}
		private void PopupMenu_Popup(object sender, EventArgs e) {
			BindingHelper.EndCurrentEdit(Form.ActiveForm);
		}
		private void PopupMenu_BeforePopup(object sender, System.ComponentModel.CancelEventArgs e) {
			if(!canRebuildItems) {
				canRebuildItems = true;
				RebuildItems();
			}
		}
		private void RecreatePopupMenu() {
			UnlinkPopupMenu();
			popupMenu = new PopupMenu();
			if(Manager is RibbonBarManager) {
				popupMenu.Ribbon = ((RibbonBarManager)Manager).Ribbon;
			}
			else {
				popupMenu.Manager = Manager;
			}
			popupMenu.BeforePopup += PopupMenu_BeforePopup;
			popupMenu.Popup += PopupMenu_Popup;
		}
		protected override void UpdateBarItem(ChoiceActionBehaviorChangedType changedType) {
			base.UpdateBarItem(changedType);
			if(changedType == ChoiceActionBehaviorChangedType.ShowItemsOnClick) {
				((BarButtonItem)Control).ActAsDropDown = Action.ShowItemsOnClick;
			}
		}
		private void UpdateActAsDropDown(BarButtonItem item) {
			item.ActAsDropDown = Action.ShowItemsOnClick;
			item.AllowDrawArrow = true;
		}
		protected override BarItem CreateControlCore() {
			BarButtonItem item = new BarButtonItem();
			RecreatePopupMenu();
			item.DropDownControl = PopupMenu;
			UpdateActAsDropDown(item);
			item.ButtonStyle = BarButtonStyle.DropDown;
			item.Caption = Action.Caption;
			return item;
		}
		protected override void ItemClicked(BarItemLink barItemLink) {
			base.ItemClicked(barItemLink);
			ProcessDefaultClick(barItemLink);
		}
		protected override void RebuildItemsCore() {
			if(!canRebuildItems && !ChoiceActionItemsHelper.HasAnyItemShortcut(Action.Items)) return;
			CreateSubItems(PopupMenu.ItemLinks, Action.Items);
		}
		protected override void OnManagerChanged() {
			RecreatePopupMenu();
			if(Control is BarButtonItem) {
				((BarButtonItem)Control).DropDownControl = PopupMenu;
			}
			UpdateActAsDropDown((BarButtonItem)Control);
			base.OnManagerChanged();
		}
		protected override void BeginUpdate() {
			if(PopupMenu != null) {
				PopupMenu.BeginUpdate();
			}
		}
		protected override void EndUpdate() {
			if(PopupMenu != null) {
				PopupMenu.EndUpdate();
			}
		}
		private void UnlinkPopupMenu() {
			if(popupMenu != null) {
				popupMenu.Popup -= PopupMenu_Popup;
				popupMenu.BeforePopup -= PopupMenu_BeforePopup;
				popupMenu.Dispose();
				popupMenu = null;
			}
		}
		public override void Dispose() {
			UnlinkPopupMenu();
			if(Control != null) {
				((BarButtonItem)Control).DropDownControl = null;
			}
			base.Dispose();
		}
		public BarSingleChoiceOperationActionItem(SingleChoiceAction singleChoiceAction, BarManager manager)
			: base(singleChoiceAction, manager) {
			canRebuildItems = false;
		}
		public BarSingleChoiceOperationActionItem(SingleChoiceAction singleChoiceAction, BarManager manager, BarButtonStyle itemStyle)
			: this(singleChoiceAction, manager) {
			this.ItemStyle = itemStyle;
		}
	}
}
