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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Core.Design;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Templates;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
	[ToolboxItem(false)] 
	public class ButtonsContainer : LayoutControl, IActionContainer {
		private bool isUpdating;
		private BarManager barManager;
		private ActionItemPaintStyle paintStyle = ActionItemPaintStyle.Default;
		private string containerId;
		private Dictionary<ActionBase, ButtonsContainersActionItemBase> actionItems;
		private bool hideItemsCompletely;
		private string actionId;
		private void SetDialogButtons(ButtonsContainersActionItemBase actionItem) {
			if(actionItem.Action.Active && actionItem.Control is SimpleButton) {
				Form form = FindForm();
				if(form != null) {
					SimpleButton simpleButton = (SimpleButton)actionItem.Control;
					if((actionItem.Action.ActionMeaning & ActionMeaning.Accept) == ActionMeaning.Accept) {
						form.AcceptButton = simpleButton;
					}
					if((actionItem.Action.ActionMeaning & ActionMeaning.Cancel) == ActionMeaning.Cancel) {
						form.CancelButton = simpleButton;
					}
				}
			}
		}
		private ButtonsContainersActionItemBase CreateActionItem(ActionBase action) {
			ButtonsContainersActionItemBase actionItem = null;
			if(action is SimpleAction) {
				actionItem = new ButtonsContainersSimpleActionItem(action, this);
			}
			else {
				if(action is ParametrizedAction) {
					actionItem = new ButtonsContainersParametrizedActionItem((ParametrizedAction)action, this);
				}
				else {
					if(action is PopupWindowShowAction) {
						actionItem = new ButtonsContainersPopupWindowShowActionItem((PopupWindowShowAction)action, this);
					}
				}
			}
			if(actionItem != null) {
				barManager.Items.Add(actionItem.ShortcutHandler);
			}
			else {
				if(action is SingleChoiceAction) {
					actionItem = CreateSingleChoiceActionItem((SingleChoiceAction)action);
				}
			}
			return actionItem;
		}
		private ButtonsContainersActionItemBase CreateSingleChoiceActionItem(SingleChoiceAction singleChoiceAction) {
			ButtonsContainersActionItemBase result = null;
			if(singleChoiceAction.ItemType == SingleChoiceActionItemType.ItemIsMode) {
				if(singleChoiceAction.IsHierarchical()) {
					result = new ButtonsSingleChoiceActionItem(singleChoiceAction, this, barManager);
					((ButtonsSingleChoiceActionItem)result).ButtonStyle = BarButtonStyle.Check;
					((ButtonsSingleChoiceActionItem)result).ShowItemsOnClick = true;
				}
				else {
					result = new ButtonsSingleChoiceComboBoxActionItem(singleChoiceAction, this, barManager);
				}
			}
			else {
				result = new ButtonsSingleChoiceActionItem(singleChoiceAction, this, barManager);
				((ButtonsSingleChoiceActionItem)result).ShowItemsOnClick = singleChoiceAction.ShowItemsOnClick;
			}
			return result;
		}
		private void OnActionItemAdded(ButtonsContainersActionItemBase actionItem) {
			if(ActionItemAdded != null) {
				ActionItemAdded(this, new ActionItemEventArgs(actionItem));
			}
		}
		private void OnActionItemAdding(ActionItemEventArgs args) {
			if(ActionItemAdding != null) {
				ActionItemAdding(this, args);
			}
		}
		private void AddButton(ButtonsContainersActionItemBase actionItem) {
			Root.AddItem(actionItem.LayoutItem);
			actionItem.Synchronize();
			SetDialogButtons(actionItem);
		}
		private void ButtonsContainer_Changed(object sender, EventArgs e) {
			if(OnlyOneActionAllowed && Actions.Count != 0) {
				LayoutControlItem layoutItem = ActionItems[Actions[0]].LayoutItem;
				if(layoutItem.Padding.Left != 0) {
					Changed -= new EventHandler(ButtonsContainer_Changed);
					base.BeginUpdate();
					DevExpress.XtraLayout.Utils.Padding padding = layoutItem.Padding;
					layoutItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, padding.Right, padding.Top, padding.Bottom);
					base.EndUpdate();
					Changed += new EventHandler(ButtonsContainer_Changed);
				}
			}
		}
		private void ClearActionItems() {
			if(ActionItems != null) {
				foreach(ButtonsContainersActionItemBase item in ActionItems.Values) {
					item.Dispose();
				}
				actionItems.Clear();
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				Changed -= new EventHandler(ButtonsContainer_Changed);
				ClearActionItems();
				actionItems = null;
			}
			base.Dispose(disposing);
		}
		public override void Clear() {
			base.Clear();
			ClearActionItems();
		}
		#region IActionContainer Members
		public void Register(ActionBase action) {
			if(OnlyOneActionAllowed && action.Id != ActionId) {
				return;
			}
			base.BeginUpdate();
			try {
				ButtonsContainersActionItemBase actionItem = CreateActionItem(action);
				ActionItemEventArgs args = new ActionItemEventArgs(actionItem);
				OnActionItemAdding(args);
				actionItem = args.Item;
				ActionItems.Add(action, actionItem);
				AddButton(actionItem);
				OnActionItemAdded(actionItem);
			}
			finally {
				base.EndUpdate();
			}
		}
		[Browsable(false), DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden)]
		public ReadOnlyCollection<ActionBase> Actions {
			get { return actionItems != null ? new List<ActionBase>(actionItems.Keys).AsReadOnly() : new List<ActionBase>().AsReadOnly(); }
		}
		[DefaultValue(null), TypeConverter(typeof(ContainerIdConverter)), Category("Design")]
		public string ContainerId {
			get { return containerId; }
			set { containerId = value; }
		}
		#endregion
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool HideItemsCompletely {
			get { return hideItemsCompletely; }
			set { hideItemsCompletely = value; }
		}
		public Dictionary<ActionBase, ButtonsContainersActionItemBase> ActionItems {
			get { return actionItems; }
		}
		public event EventHandler<ActionItemEventArgs> ActionItemAdded;
		public event EventHandler<ActionItemEventArgs> ActionItemAdding;
		public ActionContainerOrientation Orientation {
			get { return Root.DefaultLayoutType == LayoutType.Horizontal ? ActionContainerOrientation.Horizontal : ActionContainerOrientation.Vertical; }
			set { Root.DefaultLayoutType = value == ActionContainerOrientation.Horizontal ? LayoutType.Horizontal : LayoutType.Vertical; }
		}
		public string ActionId {
			get { return actionId; }
			set { actionId = value; }
		}
		public ActionItemPaintStyle PaintStyle {
			get { return paintStyle; }
			set { paintStyle = value; }
		}
		private bool OnlyOneActionAllowed {
			get { return !string.IsNullOrEmpty(ActionId); }
		}
		public BarManager BarManager {
			get { return barManager; }
			set { barManager = value; }
		}
		public bool IsUpdating {
			get { return isUpdating; }
		}
		void ISupportUpdate.BeginUpdate() {
			BeginUpdate();
			isUpdating = true;
		}
		void ISupportUpdate.EndUpdate() {
			isUpdating = false;
			foreach(ButtonsContainersActionItemBase item in ActionItems.Values) {
				item.SetLayoutItemVisibility();
			}
			EndUpdate();
		}
		public ButtonsContainer() {
			actionItems = new Dictionary<ActionBase, ButtonsContainersActionItemBase>();
			barManager = new BarManager();
			barManager.Form = this;
			barManager.DockingEnabled = false;
			AutoScroll = false;
			AllowCustomization = false;
			BackColor = Color.Transparent;
			OptionsView.AutoSizeInLayoutControl = AutoSizeModes.ResizeToMinSize;
			OptionsView.UseSkinIndents = false;
			OptionsItemText.AlignControlsWithHiddenText = false;
			Root.DefaultLayoutType = LayoutType.Horizontal;
			Root.TextVisible = false;
			Root.GroupBordersVisible = false;
			Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0);
			Root.Spacing = new DevExpress.XtraLayout.Utils.Padding(0);
			Root.OptionsItemText.TextAlignMode = TextAlignModeGroup.AutoSize;
			Changed += new EventHandler(ButtonsContainer_Changed);
		}
	}
	public class ActionItemEventArgs : EventArgs {
		public ActionItemEventArgs(ButtonsContainersActionItemBase item) {
			Item = item;
		}
		public ButtonsContainersActionItemBase Item { get; set; }
	}
}
