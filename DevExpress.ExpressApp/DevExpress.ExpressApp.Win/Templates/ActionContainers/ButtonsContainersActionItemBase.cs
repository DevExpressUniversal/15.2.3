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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.ExpressApp.Win.Templates.ActionContainers {
	public abstract class ButtonsContainersActionItemBase : ActionBaseItem
#if DebugTest
, DevExpress.ExpressApp.Tests.IActionBaseItemUnitTestable
#endif
 {
		class InternalLayoutControlItem : LayoutControlItem {
			protected override Size GetContentMinMaxSize(bool getMin) {
				if(!getMin && Control is SimpleButton) {
					return Size.Empty;
				}
				return base.GetContentMinMaxSize(getMin);
			}
		}
		private static int ItemsCount = 0;
		private Control control;
		private LayoutControlItem layoutItem;
		private BarItem shortcutHandler;
		private bool visible;
		private void shortcutHandler_ItemClick(object sender, ItemClickEventArgs e) {
			ProcessShortcut();
		}
		private void SetLayoutItemVisibilityCore(bool hideComplitly) {
			LayoutItem.Owner.BeginUpdate();
			try {
				LayoutItem.Visibility = visible ? LayoutVisibility.Always : LayoutVisibility.Never;
				if(hideComplitly) {
					if(Visible) {
						LayoutItem.RestoreFromCustomization();
					}
					else {
						LayoutItem.HideToCustomization();
					}
				}
			}
			finally {
				LayoutItem.Owner.EndUpdate();
			}
		}
		protected ButtonsContainer owner;
		protected internal void Synchronize() {
			SynchronizeWithAction();
		}
		protected override void SetVisible(bool visible) {
			this.visible = visible;
			SetLayoutItemVisibility();
		}
		protected override void SetEnabled(bool enabled) {
			Control.Enabled = enabled;
			Control.TabStop = enabled;
		}
		protected override void SetImage(ImageInfo imageInfo) { }
		protected override void SetCaption(string caption) {
			LayoutItem.TextVisible = !String.IsNullOrEmpty(caption);
		}
		protected override void SetConfirmationMessage(string message) { }
		protected bool IsConfirmed() {
			bool result = true;
			string message = Action.GetFormattedConfirmationMessage();
			if(!string.IsNullOrEmpty(message)) {
				result = WinApplication.Messaging.GetUserChoice(message, Action.Caption, MessageBoxButtons.YesNo) == DialogResult.Yes;
			}
			return result;
		}
		protected abstract Control CreateControl();
		protected override void SetShortcut(string shortcutString) {
			shortcutHandler.ItemShortcut = ShortcutHelper.ParseBarShortcut(shortcutString);
		}
		protected virtual DevExpress.XtraLayout.Utils.Padding GetPadding() {
			return new DevExpress.XtraLayout.Utils.Padding(6, 0, 0, 0);
		}
		protected override ActionItemPaintStyle GetDefaultPaintStyle() {
			if((owner != null) && (owner.PaintStyle != ActionItemPaintStyle.Default)) {
				return owner.PaintStyle;
			}
			else {
				return base.GetDefaultPaintStyle();
			}
		}
		public ButtonsContainersActionItemBase(ActionBase action, ButtonsContainer owner)
			: base(action) {
			this.owner = owner;
			shortcutHandler = new BarButtonItem();
			shortcutHandler.ItemClick += new ItemClickEventHandler(shortcutHandler_ItemClick);
		}
		public bool Visible {
			get { return visible; }
		}
		public Control Control {
			get {
				if(control == null) {
					control = CreateControl();
					if(String.IsNullOrEmpty(control.Name)) {
						control.Name = "Control_" + ItemsCount++;
					}
					control.Tag = EasyTestTagHelper.FormatTestAction(control.Text);
				}
				return control;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public LayoutControlItem LayoutItem {
			get {
				if(layoutItem == null) {
					layoutItem = new InternalLayoutControlItem();
					layoutItem.BeginInit();
					layoutItem.Name = Action.Id + "_LayoutItem";
					layoutItem.Control = Control;
					layoutItem.TextVisible = false;
					layoutItem.Padding = GetPadding();
					layoutItem.Spacing = new DevExpress.XtraLayout.Utils.Padding(0);
					layoutItem.EndInit();
				}
				return layoutItem;
			}
		}
		public BarItem ShortcutHandler {
			get { return shortcutHandler; }
		}
		public void SetLayoutItemVisibility() {
			Guard.ArgumentNotNull(LayoutItem.Owner, "LayoutItem.Owner");
			ButtonsContainer buttonsContainer = (ButtonsContainer)LayoutItem.Owner;
			if(!buttonsContainer.IsUpdating) {
				SetLayoutItemVisibilityCore(buttonsContainer.HideItemsCompletely);
			}
		}
		public abstract void ProcessShortcut();
		public override void Dispose() {
			if(shortcutHandler != null) {
				shortcutHandler.ItemClick -= new ItemClickEventHandler(shortcutHandler_ItemClick);
				shortcutHandler.Dispose();
				shortcutHandler = null;
			}
			base.Dispose();
		}
#if DebugTest
		#region IActionBaseItemUnitTestable Members
		public bool ControlVisible {
			get { return LayoutItem.Visibility == LayoutVisibility.Always; }
		}
		public bool ControlEnabled {
			get { return Control.Enabled; }
		}
		public abstract string ControlCaption { get; }
		public bool SupportPaintStyle {
			get { return true; }
		}
		public virtual bool CaptionVisible {
			get { return !string.IsNullOrEmpty(ControlCaption); }
		}
		public abstract string ControlToolTip { get; }
		public abstract bool ImageVisible { get; }
		#endregion
		public void DebugTest_Synchronize() {
			Synchronize();
		}
#endif
	}
}
