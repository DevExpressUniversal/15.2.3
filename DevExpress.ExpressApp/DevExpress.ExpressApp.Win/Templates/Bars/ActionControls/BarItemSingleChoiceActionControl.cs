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
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates.Bars.ActionControls {
	public abstract class BarItemSingleChoiceActionControl<TBarItem> : BarItemActionControl<TBarItem>, ISingleChoiceActionControl where TBarItem : BarItem {
		private static int newGroupIndex = 1;
		private int groupIndex = -1;
		private bool suppressExecute;
		private ChoiceActionItem lastSelectedItem;
		private void RaiseExecuteCore(ChoiceActionItem actionItem) {
			if(Execute != null) {
				SingleChoiceActionControlExecuteEventArgs args = actionItem != null ? new SingleChoiceActionControlExecuteEventArgs(actionItem) : new SingleChoiceActionControlExecuteEventArgs();
				try {
					Execute(this, args);
				}
				catch(ActionExecutionException e) {
					Application.OnThreadException(e.InnerException);
				}
			}
		}
		protected virtual void SetShowItemsOnClickCore(bool value) { }
		protected void RebuildItems() {
			BeginUpdate();
			try {
				ClearItems();
				BuildItems();
			}
			finally {
				EndUpdate();
			}
		}
		protected abstract void BeginUpdate();
		protected abstract void EndUpdate();
		protected abstract void ClearItems();
		protected abstract void BuildItems();
		protected virtual bool ShouldRebuildItems(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo) {
			foreach(KeyValuePair<object, ChoiceActionItemChangesType> pair in itemsChangedInfo) {
				if(
					ChangesTypeContains(pair.Value, ChoiceActionItemChangesType.Add)
					|| ChangesTypeContains(pair.Value, ChoiceActionItemChangesType.Remove)
					|| ChangesTypeContains(pair.Value, ChoiceActionItemChangesType.ItemsAdd)
					|| ChangesTypeContains(pair.Value, ChoiceActionItemChangesType.ItemsRemove)
				) {
					return true;
				}
			}
			return false;
		}
		protected bool ChangesTypeContains(ChoiceActionItemChangesType changesType, ChoiceActionItemChangesType toCheck) {
			return (changesType & toCheck) == toCheck;
		}
		protected abstract void UpdateCore(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo);
		protected abstract void SetSelectedItemCore(ChoiceActionItem selectedItem);
		protected void RaiseExecute(ChoiceActionItem actionItem) {
			RaiseExecute(actionItem, true);
		}
		protected void RaiseExecute(ChoiceActionItem actionItem, bool askConfirmation) {
			if(!suppressExecute) {
				BindingHelper.EndCurrentEdit(Form.ActiveForm);
				if(!askConfirmation || IsConfirmed()) {
					RaiseExecuteCore(actionItem);
				}
				else {
					SetSelectedItem(lastSelectedItem);
				}
			}
		}
		protected int GroupIndex {
			get {
				if(groupIndex < 0) {
					groupIndex = newGroupIndex;
					newGroupIndex++;
				}
				return groupIndex;
			}
		}
		protected ChoiceActionItemCollection ChoiceActionItems { get; set; }
		public BarItemSingleChoiceActionControl() { }
		public BarItemSingleChoiceActionControl(string actionId, TBarItem item) : base(actionId, item) { }
		public void SetShowItemsOnClick(bool value) {
			SetShowItemsOnClickCore(value);
		}
		public void SetChoiceActionItems(ChoiceActionItemCollection choiceActionItems) {
			Guard.ArgumentNotNull(choiceActionItems, "choiceActionItems");
			if(ChoiceActionItems != choiceActionItems) {
				ChoiceActionItems = choiceActionItems;
				RebuildItems();
			}
		}
		public void Update(IDictionary<object, ChoiceActionItemChangesType> itemsChangedInfo) {
			Guard.ArgumentNotNull(itemsChangedInfo, "itemsChangedInfo");
			if(ShouldRebuildItems(itemsChangedInfo)) {
				RebuildItems();
			}
			else {
				UpdateCore(itemsChangedInfo);
			}
		}
		public void SetSelectedItem(ChoiceActionItem selectedItem) {
			BeginUpdate();
			suppressExecute = true;
			try {
				SetSelectedItemCore(selectedItem);
				lastSelectedItem = selectedItem;
			}
			finally {
				EndUpdate();
				suppressExecute = false;
			}
		}
		public event EventHandler<SingleChoiceActionControlExecuteEventArgs> Execute;
	}
}
