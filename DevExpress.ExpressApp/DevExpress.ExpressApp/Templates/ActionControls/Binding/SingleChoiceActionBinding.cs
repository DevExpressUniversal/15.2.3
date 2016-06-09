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
using DevExpress.ExpressApp.Actions;
namespace DevExpress.ExpressApp.Templates.ActionControls.Binding {
	public class SingleChoiceActionBinding : ActionBinding {
		public static void Register() {
			ActionBindingFactory.Instance.Register("SingleChoiceActionBinding", CanCreate, Create);
		}
		private static bool CanCreate(ActionBase action, IActionControl actionControl) {
			return action is SingleChoiceAction && actionControl is ISingleChoiceActionControl;
		}
		private static ActionBinding Create(ActionBase action, IActionControl actionControl) {
			return new SingleChoiceActionBinding((SingleChoiceAction)action, (ISingleChoiceActionControl)actionControl);
		}
		private bool IsDependentToolTip() {
			return !Action.ShowItemsOnClick && Action.ItemType != SingleChoiceActionItemType.ItemIsMode;
		}
		private bool IsDependentImage() {
			return Action.ImageMode == ImageMode.UseItemImage;
		}
		private void UpdateItemDependentProperties() {
			UpdateControlCaption();
			if(IsDependentToolTip()) {
				UpdateControlToolTip();
			}
			if(IsDependentImage()) {
				UpdateControlImage();
			}
		}
		private void Action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			UpdateItemDependentProperties();
			ActionControl.Update(e.ChangedItemsInfo);
		}
		private void Action_SelectedItemChanged(object sender, EventArgs e) {
			if(Action.DefaultItemMode == DefaultItemMode.LastExecutedItem) {
				UpdateItemDependentProperties();
			}
			ActionControl.SetSelectedItem(Action.SelectedItem);
		}
		private void Action_BehaviorChanged(object sender, ChoiceActionBehaviorChangedEventArgs e) {
			if(e.ChangedPropertyType == ChoiceActionBehaviorChangedType.ShowItemsOnClick) {
				UpdateControlShowItemsOnClick();
			}
		}
		private void ActionControl_Execute(object sender, SingleChoiceActionControlExecuteEventArgs e) {
			ChoiceActionItem actionItem = e.ChoiceActionItem;
			if(e.IsDefaultChoiceActionItem) {
				actionItem = GetHelper().FindDefaultItem();
			}
			Action.DoExecute(actionItem);
		}
		protected SingleChoiceActionHelper GetHelper() {
			return new SingleChoiceActionHelper(Action);
		}
		protected override void UpdateControlCaption() {
			string caption = GetHelper().GetActionCaption(Action.Caption);
			ActionControl.SetCaption(caption);
		}
		protected override void UpdateControlImage() {
			ActionControl.SetImage(GetHelper().GetActionImageName());
		}
		protected virtual void UpdateControlShowItemsOnClick() {
			ActionControl.SetShowItemsOnClick(Action.ShowItemsOnClick);
		}
		public SingleChoiceActionBinding(SingleChoiceAction action, ISingleChoiceActionControl actionControl)
			: base(action, actionControl) {
			UpdateControlShowItemsOnClick();
			Action.ItemsChanged += Action_ItemsChanged;
			Action.SelectedItemChanged += Action_SelectedItemChanged;
			Action.BehaviorChanged += Action_BehaviorChanged;
			ActionControl.Execute += ActionControl_Execute;
			ActionControl.SetChoiceActionItems(Action.Items);
			ActionControl.SetSelectedItem(Action.SelectedItem);
		}
		public override void Dispose() {
			Action.ItemsChanged -= Action_ItemsChanged;
			Action.SelectedItemChanged -= Action_SelectedItemChanged;
			ActionControl.Execute -= ActionControl_Execute;
			base.Dispose();
		}
		public new SingleChoiceAction Action {
			get { return (SingleChoiceAction)base.Action; }
		}
		public new ISingleChoiceActionControl ActionControl {
			get { return (ISingleChoiceActionControl)base.ActionControl; }
		}
	}
}
