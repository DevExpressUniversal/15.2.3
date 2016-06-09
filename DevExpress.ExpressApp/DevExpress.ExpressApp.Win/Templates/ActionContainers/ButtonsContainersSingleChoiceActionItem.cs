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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
	public class ButtonsSingleChoiceActionItem : ButtonsContainersSimpleActionItem
#if DebugTest
, DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable
#endif
 {
		private BarSingleChoiceOperationActionItem underlayingActionItem;
		private BarManager barManager;
		private BarButtonStyle buttonStyle = BarButtonStyle.Default;
		private bool showItemsOnClick = false;
		private SingleChoiceActionHelper helper;
		private void action_BehaviorChanged(object sender, ChoiceActionBehaviorChangedEventArgs e) {
			if(e.ChangedPropertyType == ChoiceActionBehaviorChangedType.ShowItemsOnClick) {
				ShowItemsOnClick = ((SingleChoiceAction)Action).ShowItemsOnClick;
				UpdateControlStyle((DropDownButton)Control);
				UpdateUnderlayingActionItems();
			}
		}
		private void UpdateUnderlayingActionItems() {
			if(underlayingActionItem != null) {
				foreach(BarChoiceActionItem item in underlayingActionItem.ChoiceActionItemToWrapperMap.Values) {
					item.UpdateControlStyle();
				}
			}
		}
		private void UpdateControlStyle(DropDownButton control) {
			DropDownArrowStyle dropDownArrowStyle = DropDownArrowStyle.Default;
			bool actAsDropDown = true;
			if(((SingleChoiceAction)Action).ShowItemsOnClick) {
				dropDownArrowStyle = DropDownArrowStyle.Show;
			}
			else if(!IsMoreThanOneActionItem()) {
				dropDownArrowStyle = DropDownArrowStyle.Hide;
				actAsDropDown = false;
			}
			control.DropDownArrowStyle = dropDownArrowStyle;
			control.ActAsDropDown = actAsDropDown;
		}
		private void action_ItemsChanged(object sender, ItemsChangedEventArgs e) {
			UpdateControlStyle((DropDownButton)Control);
		}
		private void button_Click(object sender, EventArgs e) {
			((BarButtonItem)underlayingActionItem.Control).PerformClick();
		}
		private int ActiveItemsCount(ChoiceActionItemCollection choiceActionItemCollection) {
			int activeCount = 0;
			foreach(ChoiceActionItem actionItem in choiceActionItemCollection) {
				if(actionItem.Active && actionItem.Enabled) {
					activeCount++;
					activeCount += ActiveItemsCount(actionItem.Items);
				}
			}
			return activeCount;
		}
		private bool IsMoreThanOneActionItem() {
			return ActiveItemsCount(((SingleChoiceAction)Action).Items) > 1;
		}
		protected override Control CreateControl() {
			DropDownButton button = new DropDownButton();
			button.MenuManager = barManager;
			SetupControl(button);
			underlayingActionItem = new BarSingleChoiceOperationActionItem((SingleChoiceAction)Action, barManager);
			underlayingActionItem.ItemStyle = buttonStyle;
			UpdateControlStyle(button);
			BarButtonItem barItem = (BarButtonItem)underlayingActionItem.Control;
			button.DropDownControl = (PopupMenu)barItem.DropDownControl;
			button.Click += new EventHandler(button_Click);
			return button;
		}
		protected override void SetCaption(string caption) {
			string actionCaption = caption;
			if(!string.IsNullOrEmpty(caption) || string.IsNullOrEmpty(Action.Caption)) {
				actionCaption = helper.GetActionCaption(caption);
			}
			base.SetCaption(actionCaption);
		}
		protected override void SetImage(ImageInfo imageInfo) {
			base.SetImage(helper.GetActionImageInfo(imageInfo));
		}
		protected override void ProcessClick() {
			underlayingActionItem.ProcessDefaultClick();
		}
		public ButtonsSingleChoiceActionItem(SingleChoiceAction action, ButtonsContainer owner, BarManager barManager)
			: base(action, owner) {
			this.barManager = barManager;
			helper = new SingleChoiceActionHelper(action);
			action.ItemsChanged += new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
			action.BehaviorChanged += new EventHandler<ChoiceActionBehaviorChangedEventArgs>(action_BehaviorChanged);
		}
		public override void Dispose() {
			try {
				((SingleChoiceAction)Action).ItemsChanged -= new EventHandler<ItemsChangedEventArgs>(action_ItemsChanged);
				((SingleChoiceAction)Action).BehaviorChanged -= new EventHandler<ChoiceActionBehaviorChangedEventArgs>(action_BehaviorChanged);
				underlayingActionItem.Dispose();
				Control.Click -= new EventHandler(button_Click);
			}
			finally {
				base.Dispose();
			}
		}
		public bool ShowItemsOnClick {
			get { return showItemsOnClick; }
			set { showItemsOnClick = value; }
		}
		public BarButtonStyle ButtonStyle {
			get { return buttonStyle; }
			set { buttonStyle = value; }
		}
#if DebugTest
		#region ISingleChoiceActionItemUnitTestable Members
		public bool ItemVisible(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemVisible(itemPath);
		}
		public bool ItemEnabled(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemEnabled(itemPath);
		}
		public bool ItemBeginsGroup(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemBeginsGroup(itemPath);
		}
		public bool ItemImageVisible(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemImageVisible(itemPath);
		}
		public bool ItemSelected(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemSelected(itemPath);
		}
		#endregion
		public override string ControlToolTip {
			get { return Control.ToolTip; }
		}
		public override bool ImageVisible {
			get { return Control.Image != null; }
		}
		public override string ControlCaption {
			get { return Control.Text; }
		}
		public Dictionary<ChoiceActionItem, BarChoiceActionItem> ChoiceActionItems {
			get { return underlayingActionItem.ChoiceActionItemToWrapperMap; }
		}
#endif
	}
	public class ButtonsSingleChoiceComboBoxActionItem : ButtonsContainersActionItemBase
#if DebugTest
, DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable
#endif
 {
		private BarManager barManager;
		private BarSingleChoiceComboBoxActionItem underlayingActionItem;
		private void Items_CollectionChanged(object sender, CollectionChangeEventArgs e) {
			RepositoryItemImageComboBox repositoryItem = (RepositoryItemImageComboBox)underlayingActionItem.Control.Edit;
			Control.Properties.Assign(repositoryItem);
		}
		private void Properties_EditValueChanged(object sender, EventArgs e) {
			underlayingActionItem.Control.EditValue = Control.EditValue;
		}
		private void editItem_EditValueChanged(object sender, EventArgs e) {
			Control.EditValue = underlayingActionItem.Control.EditValue;
		}
		protected override DevExpress.XtraLayout.Utils.Padding GetPadding() {
			return new DevExpress.XtraLayout.Utils.Padding(6, 0, 1, 1);
		}
		protected override Control CreateControl() {
			underlayingActionItem = new BarSingleChoiceComboBoxActionItem((SingleChoiceAction)Action, barManager);
			underlayingActionItem.Control.EditValueChanged += new EventHandler(editItem_EditValueChanged);
			RepositoryItemImageComboBox repositoryItem = (RepositoryItemImageComboBox)underlayingActionItem.Control.Edit;
			ImageComboBoxEdit editor = (ImageComboBoxEdit)repositoryItem.CreateEditor();
			editor.MinimumSize = new Size(150, editor.MinimumSize.Height);
			editor.Properties.Assign(repositoryItem);
			editor.EditValue = ((SingleChoiceAction)Action).SelectedItem;
			repositoryItem.Items.CollectionChanged += new CollectionChangeEventHandler(Items_CollectionChanged);
			editor.Properties.EditValueChanged += new EventHandler(Properties_EditValueChanged);
			editor.Tag = EasyTestTagHelper.FormatTestAction(Action.Caption);
			return editor;
		}
		protected override void SetCaption(string caption) {
			if(!String.IsNullOrEmpty(caption)) {
				LayoutItem.Text = caption;
			}
			base.SetCaption(caption);
		}
		protected override void SetToolTip(string toolTip) {
			Control.ToolTip = toolTip;
		}
		protected override void SetImage(ImageInfo imageInfo) {
			LayoutItem.Image = !imageInfo.IsEmpty ? imageInfo.Image : null;
			base.SetImage(imageInfo);
		}
		public ButtonsSingleChoiceComboBoxActionItem(SingleChoiceAction action, ButtonsContainer owner, BarManager barManager)
			: base(action, owner) {
			this.barManager = barManager;
		}
		public override void Dispose() {
			base.Dispose();
			underlayingActionItem.Control.EditValueChanged -= new EventHandler(editItem_EditValueChanged);
			RepositoryItemImageComboBox repositoryItem = (RepositoryItemImageComboBox)underlayingActionItem.Control.Edit;
			repositoryItem.Items.CollectionChanged -= new CollectionChangeEventHandler(Items_CollectionChanged);
			Control.Properties.EditValueChanged -= new EventHandler(Properties_EditValueChanged);
		}
		public override void ProcessShortcut() { }
		public new ImageComboBoxEdit Control {
			get { return (ImageComboBoxEdit)base.Control; }
		}
#if DebugTest
		public override string ControlCaption {
			get { return ((DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable)underlayingActionItem).ControlCaption; }
		}
		public override string ControlToolTip {
			get { return ((DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable)underlayingActionItem).ControlToolTip; }
		}
		public override bool ImageVisible {
			get { return ((DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable)underlayingActionItem).ImageVisible; }
		}
		#region ISingleChoiceActionItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemVisible(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemVisible(itemPath);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemEnabled(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemEnabled(itemPath);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemBeginsGroup(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemBeginsGroup(itemPath);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemImageVisible(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemImageVisible(itemPath);
		}
		bool DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable.ItemSelected(string itemPath) {
			return ((DevExpress.ExpressApp.Tests.ISingleChoiceActionItemUnitTestable)underlayingActionItem).ItemSelected(itemPath);
		}
		#endregion
#endif
	}
}
