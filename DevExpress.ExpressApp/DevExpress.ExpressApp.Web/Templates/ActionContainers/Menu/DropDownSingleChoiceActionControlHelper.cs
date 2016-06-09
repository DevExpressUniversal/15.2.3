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
using System.Web;
using DevExpress.ExpressApp.Actions;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Templates.ActionContainers.Menu {
	public class DropDownSingleChoiceActionControlHelper : IDisposable {
		private SingleChoiceAction action;
		private DropDownSingleChoiceActionControlBase control;
		private readonly ICollection<ListChoiceActionItem> actionItemWrappers = new List<ListChoiceActionItem>();
		private void action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			RefreshItems();
		}
		private void action_SelectedItemChanged(object sender, EventArgs e) {
			UpdateSelectedItem();
		}
		private void comboBox_SelectedIndexChanged(object sender, EventArgs e) {
			ASPxComboBox comboBox = Control.ComboBox;
			if(HttpContext.Current == null || HttpContext.Current.Request.Form["__EVENTTARGET"] == comboBox.UniqueID) { 
				ChoiceActionItem actionItem = GetActionItemByListItem(comboBox.SelectedItem);
				if(actionItem != null) {
					OnDropDownSelectedItemChanged(actionItem);
				}
			}
		}
		private void RefreshItems() {
			ClearItems();
			PopulateItems();
			UpdateSelectedItem();
		}
		private void ClearItems() {
			Control.ComboBox.Items.Clear();
			foreach(ListChoiceActionItem wrapper in actionItemWrappers) {
				wrapper.Dispose(); 
			}
			actionItemWrappers.Clear();
		}
		private void PopulateItems() {
			foreach(ChoiceActionItem item in Action.Items) {
				if(item.Enabled && item.Active) {
					AddItem(item);
				}
			}
		}
		private void AddItem(ChoiceActionItem actionItem) {
			ListChoiceActionItem wrapper = new ListChoiceActionItem(actionItem, Action);
			Control.ComboBox.Items.Add(wrapper.ListEditItem);
			actionItemWrappers.Add(wrapper);
		}
		private void UpdateSelectedItem() {
			ASPxComboBox comboBox = Control.ComboBox;
			ListEditItem selectedItem = FindListEditItem(Action.SelectedItem);
			if(comboBox.SelectedItem != selectedItem) {
				comboBox.SelectedItem = selectedItem;
			}
		}
		private ListEditItem FindListEditItem(ChoiceActionItem actionItem) {
			foreach(ListChoiceActionItem wrapper in actionItemWrappers) {
				if(wrapper.ActionItem == actionItem) {
					return wrapper.ListEditItem;
				}
			}
			return null;
		}
		protected virtual void OnDropDownSelectedItemChanged(ChoiceActionItem item) {
			if(DropDownSelectedItemChanged != null) {
				DropDownSelectedItemChanged(this, new DropDownSelectedItemChangedEventArgs(item));
			}
		}
		public DropDownSingleChoiceActionControlHelper(SingleChoiceAction action, DropDownSingleChoiceActionControlBase control) {
			this.action = action;
			this.control = control;
			action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
			action.SelectedItemChanged += new EventHandler(action_SelectedItemChanged);
			control.ComboBox.SelectedIndexChanged += new EventHandler(comboBox_SelectedIndexChanged);
			RefreshItems();
		}
		public ChoiceActionItem GetActionItemByListItem(ListEditItem listItem) {
			foreach(ListChoiceActionItem wrapper in actionItemWrappers) {
				if(wrapper.ListEditItem == listItem) {
					return wrapper.ActionItem;
				}
			}
			return null;
		}
		public void Dispose() {
			ClearItems();
			if(action != null) {
				action.ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
				action.SelectedItemChanged -= new EventHandler(action_SelectedItemChanged);
				action = null;
			}
			if(control != null) {
				control.ComboBox.SelectedIndexChanged -= new EventHandler(comboBox_SelectedIndexChanged);
				control = null;
			}
			DropDownSelectedItemChanged = null;
		}
		public SingleChoiceAction Action {
			get { return action; }
		}
		public DropDownSingleChoiceActionControlBase Control {
			get { return control; }
		}
		public event EventHandler<DropDownSelectedItemChangedEventArgs> DropDownSelectedItemChanged;
#if DebugTest
		internal ListEditItem GetListEditItemByPath(string itemPath) {
			ChoiceActionItem actionItem = Action.FindItemByCaptionPath(itemPath);
			return FindListEditItem(actionItem);
		}
#endif
	}
}
