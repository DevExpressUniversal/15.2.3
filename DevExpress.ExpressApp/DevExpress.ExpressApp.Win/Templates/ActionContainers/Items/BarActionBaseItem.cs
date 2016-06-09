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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.ExpressApp.Win.Utils;
using DevExpress.XtraBars;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers.Items {
	public abstract class BarActionBaseItem : ActionBaseItem
#if DebugTest
, DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable
#endif
 {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool DisableClickDelay = true; 
		private BarManager manager;
		protected BarItem control;
		private TimeLatch oneItemClickEnabler;
		private void CreateControl() {
			control = CreateControlCore();
			SetupControl();
			if(manager != null) {
				manager.Items.Add(Control);
			}
			if(ControlChanged != null) {
				ControlChanged(this, EventArgs.Empty);
			}
		}
		private void ItemClickedHandler(object sender, ItemClickEventArgs e) {
			Action.IsExecuting = true;
			try {
				ItemClicked(e.Link);
			}
			finally {
				if(Action != null) {
					Action.IsExecuting = false;
				}
			}
		}
		private void Manager_Disposed(object sender, EventArgs e) {
			UnsubscribeFromActionItemsChangedEvent();
			BarManager barManager = (BarManager)sender;
			barManager.Disposed -= new EventHandler(Manager_Disposed);
		}
		protected BarActionBaseItem(ActionBase action, BarManager manager)
			: base(action) {
			oneItemClickEnabler = new TimeLatch();
			this.manager = manager;
			this.manager.Disposed += new EventHandler(Manager_Disposed);
		}
#if DebugTest
		#region IActionBaseItemUnitTestable Members
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlVisible {
			get { return BarItemUnitTestHelper.IsBarItemVisible(Control); }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlEnabled {
			get { return Control.Enabled; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlCaption {
			get { return Control.Caption; }
		}
		string DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ControlToolTip {
			get { return Control.Hint; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.SupportPaintStyle {
			get { return true; }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.ImageVisible {
			get { return BarItemUnitTestHelper.IsBarItemImageVisible(Control); }
		}
		bool DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable.CaptionVisible {
			get { return BarItemUnitTestHelper.IsBarItemCaptionVisible(Control); }
		}
		#endregion
#endif
		protected void DisposeControl() {
			if(control != null) {
				control.ItemClick -= new ItemClickEventHandler(ItemClickedHandler);
				control.Tag = null;
				control.Dispose();
				control = null;
			}
		}
		protected void EnsureControl() {
			if(control == null) {
				CreateControl();
			}
		}
		protected virtual void SetupControl() {
			if(control != null) {
				control.Name = Action.TestName;
				if(Action.Id == DevExpress.ExpressApp.Win.SystemModule.ExitController.ExitActionId) {
					control.Name = Action.Id;
				}
				control.PaintStyle = BarItemPaintStyle.CaptionGlyph;
				control.MergeType = BarMenuMerge.Add;
				control.ItemClick += new ItemClickEventHandler(ItemClickedHandler);
				SynchronizeWithAction();
			}
		}
		protected abstract BarItem CreateControlCore();
		protected virtual void ItemClicked(BarItemLink barItemLink) {
			BindingHelper.EndCurrentEdit(Form.ActiveForm);
		}
		protected virtual bool IsConfirmed() {
			bool result = true;
			string message = Action.GetFormattedConfirmationMessage();
			if(!string.IsNullOrEmpty(message)) {
				result = WinApplication.Messaging.GetUserChoice(message, Action.Caption, MessageBoxButtons.YesNo) == DialogResult.Yes;
			}
			return result;
		}
		protected virtual void OnManagerChanged() {
			manager.Disposed += new EventHandler(Manager_Disposed);
		}
		protected virtual void UnsubscribeFromActionItemsChangedEvent() { }
		protected override void SetEnabled(bool enabled) {
			control.Enabled = enabled;
		}
		protected override void SetVisible(bool visible) {
			if(visible) {
				control.Visibility = BarItemVisibility.Always;
			}
			else {
				control.Visibility = BarItemVisibility.OnlyInCustomizing;
			}
		}
		protected override void SetCaption(string caption) {
			control.Caption = Action.Caption;
		}
		protected override void SetToolTip(string toolTip) {
			control.Hint = toolTip;
		}
		protected override void SetImage(ImageInfo imageInfo) {
			if(!imageInfo.IsEmpty) {
				control.Glyph = imageInfo.Image;
			}
			ImageInfo largeImage = ImageLoader.Instance.GetLargeImageInfo(Action.ImageName);
			if(!largeImage.IsEmpty) {
				control.LargeGlyph = largeImage.Image;
			}
		}
		protected override ImageInfo GetImageInfo(ActionItemPaintStyle paintStyle) {
			return GetImageInfo();
		}
		protected override void SetConfirmationMessage(string message) {
		}
		protected override void SetShortcut(string shortcutString) {
			control.ItemShortcut = ShortcutHelper.ParseBarShortcut(shortcutString);
		}
		protected override ActionItemPaintStyle GetDefaultPaintStyle() {
			return ActionItemPaintStyle.Image;
		}
		protected override void SetPaintStyle(ActionItemPaintStyle paintStyle) {
			if(control != null) {
				SetCaption(Action.Caption);
				switch(paintStyle) {
					case ActionItemPaintStyle.Caption:
						control.PaintStyle = BarItemPaintStyle.Caption;
						break;
					case ActionItemPaintStyle.CaptionAndImage:
						control.PaintStyle = BarItemPaintStyle.CaptionGlyph;
						break;
					case ActionItemPaintStyle.Image:
						control.PaintStyle = !String.IsNullOrEmpty(Action.ImageName) ? BarItemPaintStyle.Standard : BarItemPaintStyle.Caption;
						break;
				}
			}
		}
		protected void ResetLastItemClickTime() {
			oneItemClickEnabler.ResetLastEventTime();
		}
		protected bool IsItemClickEnable {
			get { return DisableClickDelay || oneItemClickEnabler.IsTimeIntervalExpired; }
		}
		public override void Dispose() {
			UnsubscribeFromActionItemsChangedEvent();
			DisposeControl();
			if(manager != null) {
				manager.Disposed -= new EventHandler(Manager_Disposed);
				manager = null;
			}
			oneItemClickEnabler = null;
			base.Dispose();
		}
		public BarItem Control {
			get {
				if(!IsDisposed) {
					EnsureControl();
					return control;
				}
				return null;
			}
		}
		public BarManager Manager {
			get {
				return manager;
			}
			set {
				if(manager != value) {
					manager.Disposed -= new EventHandler(Manager_Disposed);
					manager = value;
					OnManagerChanged();
				}
			}
		}
		public event EventHandler<EventArgs> ControlChanged;
	}
	public abstract class BarChoiceActionItem : ChoiceActionItemWrapper {
		private BarItem control;
		private BarManager barManager;
		private SingleChoiceAction action;
		private TimeLatch oneItemClickEnabler;
		private void CreateControl() {
			control = CreateControlCore();
			BarManager.Items.Add(Control);
			SetupControl();
		}
		private void PostLastEditAndExecuteAction(ChoiceActionItem item) {
			BindingHelper.EndCurrentEdit(Form.ActiveForm);
			Action.DoExecute(item);
		}
		public override void SetImageName(string imageName) {
			Control.Glyph = ImageLoader.Instance.GetImageInfo(imageName).Image;
			ImageInfo largeImageInfo = ImageLoader.Instance.GetLargeImageInfo(imageName);
			if(!largeImageInfo.IsEmpty) {
				Control.LargeGlyph = largeImageInfo.Image;
			}
		}
		public override void SetCaption(string caption) {
			Control.Caption = caption;
		}
		public override void SetData(object data) { }
		public override void SetShortcut(string shortcutString) {
			Control.ItemShortcut = ShortcutHelper.ParseBarShortcut(shortcutString);
		}
		public override void SetEnabled(bool enabled) {
			Control.Enabled = enabled;
		}
		public override void SetVisible(bool visible) {
			Control.Visibility = visible ? BarItemVisibility.OnlyInRuntime : BarItemVisibility.Never;
		}
		public override void SetToolTip(string toolTip) {
			Control.Hint = toolTip;
		}
		protected virtual void SetupControl() {
			if(control != null) {
				SubscribeItemClickEvent();
				SyncronizeWithItem();
			}
		}
		protected virtual bool IsConfirmed() {
			bool result = true;
			string message = Action.GetFormattedConfirmationMessage();
			if(!string.IsNullOrEmpty(message)) {
				result = WinApplication.Messaging.GetUserChoice(message, Action.Caption, MessageBoxButtons.YesNo) == DialogResult.Yes;
			}
			return result;
		}
		internal void UpdateControlStyle() {
			UnSubscribeItemClickEvent();
			SubscribeItemClickEvent();
		}
		private void SubscribeItemClickEvent() {
			if(control != null) {
				if(ProcessItemClick) {
					control.ItemClick += new ItemClickEventHandler(control_ItemClick);
				}
			}
		}
		private void UnSubscribeItemClickEvent() {
			if(control != null) {
				control.ItemClick -= new ItemClickEventHandler(control_ItemClick);
			}
		}
		private void control_ItemClick(object sender, ItemClickEventArgs e) {
			if(BarActionBaseItem.DisableClickDelay || oneItemClickEnabler.IsTimeIntervalExpired || ActionItem != Action.SelectedItem) {
				oneItemClickEnabler.ResetLastEventTime();
				if(IsConfirmed()) {
					PostLastEditAndExecuteAction(ActionItem);
					if(ActionItem != null && ActionItem.Data != null) {
						BarManager.CloseMenus();
						if(e.Item != null) {
							e.Item.Manager.CloseMenus();
						}
					}
				}
			}
		}
		protected abstract BarItem CreateControlCore();
		public override void Dispose() {
			base.Dispose();
			if(control != null) {
				BarManager.Items.Remove(control);
				UnSubscribeItemClickEvent();
				control.Dispose();
			}
		}
		public BarChoiceActionItem(SingleChoiceAction action, ChoiceActionItem actionItem, BarManager barManager)
			: base(actionItem, action) {
			this.action = action;
			this.barManager = barManager;
			this.oneItemClickEnabler = new TimeLatch();
		}
		public BarItem Control {
			get {
				if(!IsDisposed) {
					if(control == null) {
						CreateControl();
					}
					return control;
				}
				return null;
			}
		}
		public BarManager BarManager { get { return barManager; } }
		public SingleChoiceAction Action { get { return action; } }
		public virtual bool ProcessItemClick {
			get { return true; }
		}
	}
#if DebugTest
	internal class BarItemUnitTestHelper {
		public static bool IsBarItemVisible(BarItem item) {
			return item.Visibility == BarItemVisibility.Always || item.Visibility == BarItemVisibility.OnlyInRuntime;
		}
		public static bool IsBarItemImageVisible(BarItem item) {
			BarEditItem barEditItem = item as BarEditItem;
			if(barEditItem != null) {
				return (item.Glyph != null) && (item.PaintStyle != BarItemPaintStyle.Caption);
			}
			return (item.Glyph != null) && (item.PaintStyle != BarItemPaintStyle.Caption);
		}
		public static bool IsBarItemCaptionVisible(BarItem item) {
			if(item.PaintStyle == BarItemPaintStyle.Standard ) {
				return false;
			}
			else {
				return !string.IsNullOrEmpty(item.Caption);
			}
		}
	}
#endif
}
