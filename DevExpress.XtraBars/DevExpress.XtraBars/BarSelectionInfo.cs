#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraBars.Customization;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.Helpers.Docking;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraBars.MessageFilter;
using DevExpress.XtraBars.InternalItems;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Win;
using DevExpress.Utils.Controls;
using DevExpress.XtraBars.Design;
using DevExpress.XtraBars.Ribbon;
using System.Collections.Generic;
using DevExpress.Skins;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraBars.ViewInfo {
	public enum BarLinkSelectionType { Pressed, Highlighted, CustomizeSelected, DropMarked }
	public enum BarLinkNavigation { First, Up, Down, Last, Left, Right, PageUp, PageDown };
	public interface IBarLinkTimer {
		bool CanStartTimer { get; }
		bool CanStopTimer { get; }
		int TickInterval { get; }
		void OnTimerRun();
		bool OnTimerTick(bool sameLink);
	}
	public interface IBarPopupCollectionOwner {
		void OnCollectionChanged();
	}
	public class BarPopupCollection : CollectionBase {
		IBarPopupCollectionOwner owner;
		public BarPopupCollection(IBarPopupCollectionOwner owner) {
			this.owner = owner;
		}
		public IBarPopupCollectionOwner Owner { get { return owner; } }
		public IPopup this[int index] { get { return List[index] as IPopup; } }
		public int Add(IPopup popup) { return List.Add(popup); }
		public bool Contains(object popup) { return List.Contains(popup); }
		public int IndexOf(object popup) { return List.IndexOf(popup); }
		public IPopup LastPopup {
			get {  
				if(Count == 0) return null;
				return this[Count - 1];
			}
		}
		public IPopup FirstPopup {
			get {
				if(Count == 0) return null;
				return this[0];
			}
		}
		protected override void OnClear() {
			base.OnClear();
		}
		protected override void OnRemoveComplete(int index, object item) {
			if(Owner != null)
				Owner.OnCollectionChanged();
		}
		protected override void OnInsertComplete(int index, object item) {
			if(Owner != null)
				Owner.OnCollectionChanged();
		}
		public void RemoveRange(int index, int count) {
			InnerList.RemoveRange(index, count);
		}
		public IPopup RootPopup { get { return Count == 0 ? null : this[0]; } }
	}
	public class BarSelectionInfo : IPopupHelper, IBarPopupCollectionOwner {
		BarItemLink pressedLink, highlightTimerLink, highlightedLink, keyboardHighlightedLink, customizeSelectedLink, dropSelectedLink;
		BarEditItemLink editingLink;
		LinkDropTargetEnum dropSelectStyle;
		BarItem selectedItem;
		ICustomBarControl activeBarControl;
		Bar autoOpenMenuBar;
		BarManager manager;
		DockingManager dockManager;
		bool modalTextBoxActive, allowAnimation;
		internal int internalFocusLock;
		int lockNullPressed;
		internal Form prevFocusedForm;
		internal Control prevFocusedControl;
		object prevEditValue;
		internal bool showingEditor = false;
		BarItemLink toolTipLink;
		Timer highlightTimer;
		BarPopupCollection openedPopups;
		const int ToolTipShowDelay = 400;
		bool inKeyboardHighlighing, temporaryActive, modalDialogActive;
		public bool ShowNonRecentItems;
		protected internal BarSelectionInfo(BarManager barManager) {
			this.modalDialogActive = this.temporaryActive = false;
			this.lockNullPressed = 0;
			this.inKeyboardHighlighing = false;
			this.modalTextBoxActive = false;
			this.ModalTextBox = null;
			this.openedPopups = new BarPopupCollection(this);
			toolTipLink = null;
			highlightTimerLink = null;
			highlightTimer = new Timer();
			highlightTimer.Tick += new EventHandler(OnHighlightTimerTick);
			prevEditValue = null;
			prevFocusedControl = null;
			prevFocusedForm = null;
			internalFocusLock = 0;
			dockManager = null;
			editingLink = null;
			selectedItem = null;
			this.allowAnimation = false;
			this.dropSelectStyle = LinkDropTargetEnum.None;
			this.keyboardHighlightedLink = this.dropSelectedLink = this.customizeSelectedLink = this.highlightedLink = this.pressedLink = null;
			this.activeBarControl = null;
			this.autoOpenMenuBar = null;
			this.manager = barManager;
			ShowNonRecentItems = false;
		}
		internal bool ModalTextBoxActive {
			get { return modalTextBoxActive; }
			set { 
				if(ModalTextBoxActive == value) return;
				modalTextBoxActive = value; 
				if(!ModalTextBoxActive) UpdateZOrder();
			}
		}
		internal ModalTextBox ModalTextBox {
 			get;
			set;
		}
		public void UpdateZOrder() {
			UpdatePopupsZOrder();
			UpdateBarsZOrder();
		}
		public virtual bool ModalDialogActive { 
			get {
				if(Manager != null && Manager.Form != null) return modalDialogActive || !Manager.Form.Enabled;
				return modalDialogActive;
			}
			set {
				bool prevModalDialogActive = ModalDialogActive;
				if(ModalDialogActive == value) return;
				modalDialogActive = value;
				if(prevModalDialogActive == ModalDialogActive) return;
				UpdateEnableWindow();
				UpdateBarsZOrder();
			}
		}
		protected internal virtual void UpdateEnableWindow() {
			if(Manager == null || Manager.IsDestroying) return;
			foreach(Bar bar in Manager.Bars) {
				if(bar.DockStyle != BarDockStyle.None || bar.BarControl == null) continue;
				Form form = bar.BarControl.FindForm();
				if (form != null && form.Handle != IntPtr.Zero) BarNativeMethods.EnableWindow(form.Handle, !ModalDialogActive);
			}
		}
		protected internal virtual void UpdatePopupsZOrder() {
			if(Manager == null || Manager.IsDestroying) return;
			for(int n = 0; n < OpenedPopups.Count; n++) {
				IPopup popup = OpenedPopups[n] as IPopup;
				ControlForm form = popup.PopupForm as ControlForm;
				if(form != null) form.UpdateZ();
			}
		}
		protected virtual void UpdateBarsZOrder() {
			if(Manager == null || Manager.IsDestroying) return;
			for(int n = 0; n < Manager.Bars.Count; n++) {
				Bar bar = Manager.Bars[n];
				if(bar.BarControl == null || bar.DockStyle != BarDockStyle.None) continue;
				FloatingBarControl fc = bar.BarControl as FloatingBarControl;
				if(fc != null) fc.UpdateZ();
			}
		}
		public virtual bool TemporaryActive {
			get { return temporaryActive; }
			set {
				if(TemporaryActive == value) return;
				temporaryActive = value;
				if(!TemporaryActive && !Manager.IsRealBarsActive) Clear();
			}
		}
		public virtual int LockNullPressed { get { return lockNullPressed; } }
		public virtual BarPopupCollection OpenedPopups { get { return openedPopups; } }
		protected internal void OnLinkDelete(BarItemLink link) {
			if(CustomizeSelectedLink == link || KeyboardHighlightedLink == link || HighlightedLink == link) {
				if(Manager.IsCustomizing) {
					KeyboardHighlightedLink = null;
					HighlightedLink = null;
					CustomizeSelectedLink = null;
				} else
					Clear();
			}
		}
		protected virtual void ClearHighlightTimer(BarItemLink newLink) {
			IBarLinkTimer itimer = newLink as IBarLinkTimer;
			bool always = itimer != null && itimer.CanStartTimer;
			ClearHighlightTimer(always);
		}
		protected virtual void ClearHighlightTimer(bool always) {
			IBarLinkTimer itimer = HighlightTimerLink as IBarLinkTimer;
			if(!always && itimer != null && !itimer.CanStopTimer) return;
			this.highlightTimerLink = null;
			HighlightTimer.Stop();
		}
		protected internal virtual void InitHighligthTimer(BarItemLink link) {
			if(HighlightTimerLink != null) return;
			if(link == null || Manager.IsCustomizing) return;
			IBarLinkTimer itimer = link as IBarLinkTimer;
			if(itimer == null || !itimer.CanStartTimer) return;
			this.highlightTimerLink = link;
			this.HighlightTimer.Interval = itimer.TickInterval;
			this.HighlightTimer.Start();
			itimer.OnTimerRun();
		}
		protected virtual void OnHighlightTimerTick(object sender, EventArgs e) {
			BarItemLink link = HighlightTimerLink;
			IBarLinkTimer itimer = link as IBarLinkTimer;
			if(itimer != null) {
				if(itimer.OnTimerTick(HighlightedLink == link)) 
					HighlightTimer.Start();
				else 
					ClearHighlightTimer(true);
			}
		}
		public BarManager Manager { get { return manager; } }
		public virtual void Clear() {
			HideToolTip();
			ClearHighlightTimer(true);
			CloseAllPopups();
			AllowAnimation = false;
			ShowNonRecentItems = false;
			EditingLink = null;
			DockManager = null;
			ActiveBarControl = null;
			AutoOpenMenuBar = null;
			PressedLink = null;
			KeyboardHighlightedLink = HighlightedLink = null;
			CustomizeSelectedLink = null;
			DropSelectStyle = LinkDropTargetEnum.None;
		}
		public bool AllowAnimation { get { return allowAnimation; } set { allowAnimation = value; } }
		internal DockingManager DockManager { 
			get { return dockManager; } 
			set {
				if(DockManager == value) return;
				if(DockManager != null) {
					DockingManager man = DockManager;
					dockManager = null;
					man.StopMoving();
					man.Dispose();
				}
				dockManager = value;
			}
		}
		public virtual BarItemLink DropSelectedLink {
			get { return dropSelectedLink; }
			set {
				if(DropSelectedLink == value) return;
				BarItemLink prevLink = DropSelectedLink;
				dropSelectedLink = value;
				if(prevLink != null) {
					prevLink.CheckUpdateLinkState();
					prevLink.Invalidate();
				}
				if(DropSelectedLink != null) {
					CheckPopups(DropSelectedLink);
					DropSelectedLink.OnLinkActionCore(BarLinkAction.DropSelect, null);
					if(DropSelectedLink != null) DropSelectedLink.CheckUpdateLinkState();
				}
			}
		}
		public virtual LinkDropTargetEnum DropSelectStyle { 
			get { return dropSelectStyle; } 
			set { if(value == DropSelectStyle) return;
				dropSelectStyle = value;
				if(DropSelectStyle == LinkDropTargetEnum.None)
					DropSelectedLink = null;
				else {
					if(DropSelectedLink != null) 
						DropSelectedLink.Invalidate();
				}
			}
		}
		public Bar AutoOpenMenuBar {
			get { return autoOpenMenuBar;}
			set {
				if(AutoOpenMenuBar == value) return;
				autoOpenMenuBar = value;
			}
		}
		public BaseEdit ActiveEditor { get { return Manager.ActiveEditor; } }
		internal void SetEditingLink(BarEditItemLink link) { this.editingLink = link; }
		public BarEditItemLink EditingLink {
			get { return editingLink; }
			set {
				if(EditingLink == value) return;
				if(editingLink != null) {
					if(!CloseEditor()) return; 
				}
				editingLink = value;
			}
		}
		void ForceClosePopupEdit() { 
			PopupBaseEdit popupBase = ActiveEditor as PopupBaseEdit;
			if(popupBase != null)
				popupBase.ForceClosePopup();
		}
		public bool CloseEditor() { return CloseEditor(true);  }
		public virtual bool CloseEditor(bool restoreFocus) {
			ForceClosePopupEdit();
			if(ActiveEditor != null) {
				if(!ActiveEditor.DoValidate(PopupCloseMode.Immediate)) return false;
			}
			PostEditor();
			return HideEditor(restoreFocus);
		}
		public virtual void PostEditor() {
			if(ActiveEditor == null || EditingLink == null) return;
			BarEditItemLink editLink = EditingLink as BarEditItemLink;
			editLink.EditValue = ActiveEditor.EditValue;
			prevEditValue = null;
		}
		public virtual void CancelEditor() {
			if(ActiveEditor != null && EditingLink != null) {
				EditingLink.EditValue = prevEditValue;
				HideEditor();
			}
		}
		internal void RestoreFocus(bool clearPrev) {
			try {
				internalFocusLock++;
				if(prevFocusedForm != null && prevFocusedForm.Visible) {
					prevFocusedForm.Focus();
					if(prevFocusedControl != null && prevFocusedControl.Visible)
						prevFocusedControl.Focus();
				}
			}
			catch {
			}
			internalFocusLock--;
			prevFocusedForm = null;
			prevFocusedControl = null;
		}
		public bool HideEditor() {
			return HideEditor(true);
		}
		public virtual bool HideEditor(bool restoreFocus) {
			if(ActiveEditor != null) {
				BarEditItemLink prevEditingLink = EditingLink;
				BaseEdit actEditor = ActiveEditor;
				XtraForm.SuppressDeactivation = true;
				try {
					Manager.EditorHelper.HideEditorCore(ActiveEditor.Parent, false);
					PopupBaseEdit popup = actEditor as PopupBaseEdit;
					if(popup != null) popup.Closed -= new ClosedEventHandler(OnActiveEditor_Closed);
					actEditor.EditValueChanged -= new EventHandler(OnActiveEditor_EditValueChanged);
					actEditor.Leave -= new EventHandler(OnActiveEditor_LostFocus);
					if(restoreFocus)
						RestoreFocus(true);
					this.editingLink = null;
				}
				finally {
					XtraForm.SuppressDeactivation = false;
				}
				if(prevEditingLink != null) {
					if(prevEditingLink == PressedLink)
						pressedLink = null;
					if(prevEditingLink.Item != null) {
						bool res = prevEditingLink.Item.RaiseHiddenEditor(prevEditingLink);
						OnEditorHidden(prevEditingLink);
						if(prevEditingLink.Item != null && prevEditingLink.Item.Manager != Manager) {
							Manager.EditorHelper.DestroyEditorsCache();
						}
						return res;
					}
				}
			}
			return true;
		}
		protected internal virtual void OnEditorEnter(BarEditItemLink link) {
			if(!link.CloseEditor()) return;
			Clear();
		}
		protected internal virtual void OnEditorEscape(BarEditItemLink link) {
			if(link.Bar != null) Clear();
			else HighlightedLink = link;
		}
		protected virtual void OnEditorHidden(BarItemLink link) {
			link.CheckUpdateLinkState();
			link.Invalidate();
		}
		Control FindFocusedControl(Control control) {
			if(control.Focused) return control;
			if(control.ContainsFocus) {
				foreach(Control ctrl in control.Controls) {
					Control res = FindFocusedControl(ctrl);
					if(res != null) return res;
				}
				return control;
			}
			ContainerControl container = control as ContainerControl;
			if(container != null) return container.ActiveControl;
			return null;
		}
		void IPopupHelper.HidePopupForm(DevExpress.XtraEditors.Popup.PopupBaseForm popupForm) {
			this.internalFocusLock ++;
			try {
				popupForm.HidePopupForm();
			} finally {
				this.internalFocusLock --;
			}
		}
		protected internal virtual void SaveFocusedControl() {
			prevFocusedForm = ControlHelper.GetSafeActiveForm();
			prevFocusedControl = null;
			if(prevFocusedForm != null)
				prevFocusedControl = FindFocusedControl(prevFocusedForm);
		}
		public virtual void ShowEditor(BarEditItemLink link, BaseEdit edit) {
			bool shouldHideEditor = false;
			HideToolTip();
			if(GetEditContainer(link) == null) return;
			EditingLink = link;
			if(ActiveEditor != null) {
				HideEditor();
				EditingLink = link; 
			}
			prevEditValue = EditingLink.EditValue;
			SaveFocusedControl();
			ActiveBarControl = GetEditContainer(link) == null ? null : GetEditContainer(link);
			try {
				internalFocusLock++;
				this.showingEditor = true;
				KeyboardHighlightedLink = link;
				IPopupHelperController popupEdit = edit as IPopupHelperController;
				if(popupEdit != null) popupEdit.PopupHelper = this;
				Control control = GetEditContainer(link).Control;
				control.Select();
				if(ControlBase.GetValidationCanceled(control)) 
					shouldHideEditor = true;
				else 
					Manager.EditorHelper.ShowEditor(edit, GetEditContainer(link).Control, false);
				PopupControlContainer.NCActivate(Manager);
			}
			finally{
				this.showingEditor = false;
				internalFocusLock --;
			}
			if(!shouldHideEditor) {
				PopupBaseEdit popup = edit as PopupBaseEdit;
				if(popup != null) popup.Closed += new ClosedEventHandler(OnActiveEditor_Closed);
				edit.EditValueChanged += new EventHandler(OnActiveEditor_EditValueChanged);
				edit.Enter += new EventHandler(OnActiveEditor_LostFocus);
			}
			else {
				EditingLink = null;
			}
		}
		protected virtual ICustomBarControl GetEditContainer(BarEditItemLink link) {
			return link.BarControl;
		}
		protected virtual void OnActiveEditor_EditValueChanged(object sender, EventArgs e) {
		}
		protected virtual void OnActiveEditor_Closed(object sender, ClosedEventArgs e) {
			if(e.CloseMode == PopupCloseMode.Normal) {
				if(ActiveEditor != null && EditingLink != null && !EditingLink.Item.AutoHideEdit) {
					PostEditor();
					return;
				}
				if(!CloseEditor()) HideEditor();
			}
		}
		protected virtual void OnActiveEditor_LostFocus(object sender, EventArgs e) {
			if(ActiveEditor != null && !ActiveEditor.EditorContainsFocus) {
				HideEditor(false);
				ActiveBarControl = null;
			}
		}
		public ICustomBarControl ActiveBarControl {
			get { return activeBarControl; }
			set {
				if(ActiveBarControl == value) return;
				activeBarControl = value;
			}
		}
		internal BarItem SelectedItem {
			get { return selectedItem == null && CustomizeSelectedLink != null ? CustomizeSelectedLink.Item : selectedItem;}
			set { selectedItem = value; }
		}
		public BarItemLink CustomizeSelectedLink {
			get { return customizeSelectedLink; }
			set {
				if(CustomizeSelectedLink == value) {
					if(CustomizeSelectedLink is BarCustomContainerItemLink) {
						BarCustomContainerItemLink cLink = CustomizeSelectedLink as BarCustomContainerItemLink;
						cLink.Opened = !cLink.Opened;
					}
					return;
				}
				if(ModalTextBoxActive) return;
				DoSelectedLinkChanging(CustomizeSelectedLink, value);
			}
		}
		public Timer HighlightTimer { get { return highlightTimer; } }
		public BarItemLink ToolTipLink { get { return toolTipLink; } }
		public BarItemLink HighlightTimerLink { get { return highlightTimerLink; } }
		RibbonControl Ribbon {
			get {
				RibbonBarManager rm = Manager as RibbonBarManager;
				if(rm == null) return null;
				return rm.Ribbon;
			}
		}
		public BarItemLink HighlightedLink {
			get { return highlightedLink; }
			set {
				if(HighlightedLink == value) return;
				BarItemLink prevLink = HighlightedLink;
				OnHighlightedLinkChanging(HighlightedLink, value);
				if(prevLink != HighlightedLink)
					Manager.RaiseHighlightedLinkChanged(new HighlightedLinkChangedEventArgs(prevLink, HighlightedLink));
			}
		}
		internal void SetHighlightedLink(BarItemLink link) { this.highlightedLink = link; }
		public BarItemLink KeyboardHighlightedLink {
			get { return keyboardHighlightedLink; }
			set {
				if(KeyboardHighlightedLink == value) return;
				inKeyboardHighlighing = true;
				try {
					HighlightedLink = value;
					this.keyboardHighlightedLink = null;
					if(HighlightedLink == value) {
						keyboardHighlightedLink = value;
						if(KeyboardHighlightedLink != null)
							KeyboardHighlightedLink.CheckUpdateLinkState();
					}
				}
				finally {
					inKeyboardHighlighing = false;
				}
				if(KeyboardHighlightedLink != null) {
					KeyboardHighlightedLink.OnLinkActionCore(BarLinkAction.KeyboardHighlight, null);
				}
			}
		}
		protected virtual bool CanAnimate {
			get {
				if(Manager.PaintStyle is ISkinProvider && !WindowsFormsSettings.GetAllowHoverAnimation((ISkinProvider)Manager.PaintStyle))
					return false;
				if(Manager != null && !Manager.AllowItemAnimatedHighlighting) return false;
				return Manager != null && !Manager.IsDesignMode && Manager.PaintStyle.CanAnimate;
			}
		}
		protected virtual void AddLinkAnimation(BarItemLink link, ObjectState oldState, ObjectState newState, bool fade, bool fadeIn) {
			if(link.BarControl == null || link.LinkViewInfo == null) return;
			if(!CanAnimate) return;
			if(link.IsLinkInMenu || !link.LinkViewInfo.CanAnimate) return;
			if(fade)
				XtraAnimator.Current.AddBitmapFadeAnimation(link.BarControl as ISupportXtraAnimation, link.LinkViewInfo, Ribbon != null && Ribbon.ItemAnimationLength != -1? Ribbon.ItemAnimationLength: XtraAnimator.Current.CalcBarAnimationLength(oldState, newState),
					GetBackPainter(link.LinkViewInfo), GetForePainter(link.LinkViewInfo), fadeIn);
			else
				XtraAnimator.Current.AddBitmapAnimation(link.BarControl as ISupportXtraAnimation, link.LinkViewInfo, Ribbon != null && Ribbon.ItemAnimationLength != -1 ? Ribbon.ItemAnimationLength : XtraAnimator.Current.CalcBarAnimationLength(oldState, newState),
					GetBackPainter(link.LinkViewInfo), GetForePainter(link.LinkViewInfo), new BitmapAnimationImageCallback(OnGetLinkImage));
		}
		ObjectPaintInfo GetBackPainter(BarLinkViewInfo linkInfo) {
			if(linkInfo.IsLinkInMenu && linkInfo.BarControlInfo is CustomSubMenuBarControlViewInfo) {
				return new ObjectPaintInfo(new BarSubmenuBackgroundPainter(),
						new BarSubmenuBackgroundObjectInfoArgs(linkInfo.BarControlInfo.Painter as BarSubMenuPainter, linkInfo.BarControlInfo as CustomSubMenuBarControlViewInfo));
			}
			if(linkInfo.BarControlInfo is TabFormControlViewInfo) {
				return new ObjectPaintInfo(new TabFormControlBackgroundPainter(),
					new TabFormControlBackgroundObjectInfoArgs(linkInfo.BarControlInfo.Painter as TabFormControlPainter, linkInfo.BarControlInfo as TabFormControlViewInfo));
			}
			return new ObjectPaintInfo(new BarBackgroundPainter(),
				new BarBackgroundObjectInfoArgs(linkInfo.BarControlInfo.Painter as BarPainter, linkInfo.BarControlInfo as BarControlViewInfo, null));
		}
		ObjectPaintInfo GetForePainter(BarLinkViewInfo linkInfo) {
			return new ObjectPaintInfo(new BarLinkObjectPainter(), new BarLinkObjectInfoArgs(linkInfo));
		}
		Bitmap OnGetLinkImage(BaseAnimationInfo info) {
			BarLinkViewInfo linkInfo = info.AnimationId as BarLinkViewInfo;
			if(linkInfo == null || !linkInfo.CanAnimate) return null;
			return XtraAnimator.Current.CreateBitmap(GetBackPainter(linkInfo), GetForePainter(linkInfo));
		}
		protected virtual void OnHighlightedLinkChanging(BarItemLink oldLink, BarItemLink newLink) {
			ClearHighlightTimer(newLink);
			if(!CanHighlight(newLink)) {
				return;
			}
			if(newLink != null && Manager.IsCustomizing) newLink = null;
			CheckAndClosePopups(newLink);
			if(oldLink != null && !inKeyboardHighlighing) AddLinkAnimation(oldLink, ObjectState.Hot, ObjectState.Normal, true, false);
			this.keyboardHighlightedLink = null;
			this.highlightedLink = newLink;
			if(oldLink != null && oldLink.Item != null) {
				oldLink.CheckUpdateLinkState();
				oldLink.OnLinkActionCore(BarLinkAction.UnHighlight, newLink);
			}
			if(newLink != null && !inKeyboardHighlighing) AddLinkAnimation(newLink, ObjectState.Normal, ObjectState.Hot, false, false);
			if(newLink != null) {
				if(!inKeyboardHighlighing)
					newLink.OnLinkActionCore(BarLinkAction.Highlight, oldLink);
				this.highlightedLink = newLink;
				newLink.CheckUpdateLinkState();
			}
			if(!inKeyboardHighlighing) {
				InitHighligthTimer(HighlightedLink);
			}
			UpdatePopupHighlighting();
		}
		public virtual bool AllowPaintHighlightWhenEditing(BarItemLink link) {
			if(link != HighlightedLink) return false;
			if(EditingLink == link) return true;
			if(EditingLink != null && EditingLink.Item.IEBehavior && !EditingLink.IsLinkInMenu &&
				!link.IsLinkInMenu) return true;
			return false;
		}
		protected virtual bool CanHighlight(BarItemLink link) {
			if(ActiveBarControl is CustomBarControl) { 
				if(link == null || link.BarControl != ActiveBarControl) {
					if(EditingLink != null && EditingLink.Item.IEBehavior) {
						if(!EditingLink.IsLinkInMenu) {
							return true;
						}
					}
					return false;
				}
			}
			if(!inKeyboardHighlighing && EditingLink != null && EditingLink != link) {
				if(EditingLink.Item.IEBehavior || !EditingLink.IsLinkInMenu) return true;
				return false;
			}
			foreach(IPopup popup in OpenedPopups) {
				if(popup.LockHighlight(link)) {
					return false;
				}
			}
			if(link == null) {
				return true;
			}
			if(Manager.IsDragging) return false;
			if(!Manager.IsBarsActive) return false;
			if(Manager.IsCustomizing) return false;
			if(OpenedPopups.Count == 0) return true;
			if(AutoOpenMenuBar != null && AutoOpenMenuBar.BarControl != null) {
				if(AutoOpenMenuBar.BarControl.VisibleLinks.Contains(link)) return true;
			}
			foreach(IPopup popup in OpenedPopups) {
				if(popup.CanHighlight(link)) return true;
			}
			return false;
		}
		public virtual void CustomizeSelectLink(BarItemLink link) {
			CustomizeSelectedLink = link;
		}
		public void PressLink(BarItemLink link) { PressLinkCore(link, false); }
		public void PressLinkArrow(BarItemLink link) { PressLinkCore(link, true); }
		protected virtual void PressLinkCore(BarItemLink link, bool isArrow) {
			if(link == null || !link.AlwaysWorking) {
				if(Manager.IsCustomizing) {
					CustomizeSelectLink(link);
					return;
				}
			}
			if(link == PressedLink) return;
			if(link != null && EditingLink != null && EditingLink != link) {
				if(!CloseEditor(!(link is BarEditItemLink))) return;
			}
			DoPressedLinkChanging(PressedLink, link, isArrow);
		}
		public virtual bool IsButtonGroupLinksEquals(BarItemLink link1, BarItemLink link2) {
			if(link1 == null || link2 == null) return false;
			if(link1.Holder != link2.Holder) return false;
			BarButtonGroup group = link1.Holder as BarButtonGroup;
			if(group == null || link1.ClonedFromLink != link2.ClonedFromLink || link1.Bounds != link2.Bounds) return false;
			return true;
		}
		public virtual void UnPressLink(BarItemLink link) {
			if(Manager.IsCustomizing) {
				if(link == null || !link.AlwaysWorking) {
					if(PressedLink == null || !PressedLink.AlwaysWorking) return;
				}
			}
			if(link != null) { 
				if(link.IsLinkInMenu) { 
					Point local = link.BarControl.PointToClient(Cursor.Position);
					if(!link.LinkViewInfo.Rects[BarLinkParts.OpenArrow].Contains(local))
						ClickLink(link);
				}
				else if(PressedLink == link || IsButtonGroupLinksEquals(link, PressedLink))
					ClickLink(link);
			}
			if(PressedLink == null) return;
			if(Manager.IsDestroying) return;
			DoPressedLinkChanging(PressedLink, null, false);
		}
		public virtual void ClickLink(BarItemLink link) {
			if(Manager.IsCustomizing && (link == null || !link.AlwaysWorking)) return;
			if(!CanPress(link)) return;
			link.OnLinkActionCore(BarLinkAction.MouseClick, null);
		}
		System.ComponentModel.Design.IDesigner GetDesigner(object component) {
			IDesignerHost host = Manager.InternalGetService(typeof(IDesignerHost)) as IDesignerHost;
			if(host == null) return null;
			return host.GetDesigner(component as IComponent) as IDesigner;
		}
		public virtual void DoubleClickLink(BarItemLink link) {
			if(Manager.IsCustomizing) {
				if(Manager.IsDesignMode) {
					CustomizeSelectLink(link);
					IDesigner designer = GetDesigner(link.Item);
					if(designer != null) designer.DoDefaultAction();
					return;
				}
				PressLink(link);
				return;
			}
			if(!CanPress(link)) return;
			link.OnLinkActionCore(BarLinkAction.MouseDoubleClick, null);
		}
		public virtual bool CanResizeCustomizeSelectedLink(Point p, BarItemLink linkByPoint) {
			if(CustomizeSelectedLink == null || Manager.IsDragging || !Manager.IsCustomizing) return false;
			if(CustomizeSelectedLink.CanResize && CustomizeSelectedLink == linkByPoint) {
				LinkHitInfo hitInfo = Manager.Helper.DragManager.CalcLinkHitInfo(CustomizeSelectedLink, p);
				if(hitInfo.HitTest == LinkHitTest.LeftEdge ||
					hitInfo.HitTest == LinkHitTest.RightEdge) return true;
			}
			return false;
		}
		protected virtual void DoPressedLinkChanging(BarItemLink oldLink, BarItemLink newLink, bool isArrow) {
			pressedLink = null;
			if(oldLink != null && oldLink.Item != null && !oldLink.IsDisposed)
				oldLink.CheckUpdateLinkState();
			if(!CanPress(newLink)) {
				Manager.RaisePressedLinkChanged(new HighlightedLinkChangedEventArgs(oldLink, PressedLink));
				return;
			}
			pressedLink = newLink;
			if(PressedLink != null) {
				if(ActiveBarControl != PressedLink.BarControl) ActiveBarControl = null;
				CheckPopups(PressedLink);
				HighlightedLink = PressedLink;
				if(PressedLink == null) {
					return; 
				}
				if(PressedLink.RibbonItemInfo == null) {
					BarLinkViewInfo li = PressedLink.LinkViewInfo;
					if(li != null) {
						Rectangle r = li.Rects[BarLinkParts.OpenArrow];
						if(r.Contains(li.Link.ScreenToLinkPoint(Cursor.Position))) {
							isArrow = true;
						}
					}
				}
				if(isArrow) {
					pressedLink = null;
					newLink.OnLinkActionCore(BarLinkAction.PressArrow, null);
					newLink.CheckUpdateLinkState();
					Manager.RaisePressedLinkChanged(new HighlightedLinkChangedEventArgs(oldLink, PressedLink));
					return;
				}
				PressedLink.CheckUpdateLinkState();
				PressedLink.OnLinkActionCore(BarLinkAction.Press, null);
				pressedLink = newLink;
				newLink.CheckUpdateLinkState();
				Manager.RaisePressedLinkChanged(new HighlightedLinkChangedEventArgs(oldLink, PressedLink));
			} 
		}
		protected virtual void DoSelectedLinkChanging(BarItemLink oldLink, BarItemLink newLink) {
			customizeSelectedLink = null;
			if(oldLink != null && oldLink.Item != null) {
				if(oldLink.Item != null) oldLink.Item.LinkProvider = null;
				oldLink.CheckUpdateLinkState();
				oldLink.Invalidate();
			}
			if(!CanSelect(newLink)) return;
			customizeSelectedLink = newLink;
			if(CustomizeSelectedLink != null) {
				newLink.CheckUpdateLinkState();
				CustomizeSelectedLink.OnLinkActionCore(BarLinkAction.CustomizeSelect, null);
				if(Manager.IsDesignMode) {
					BarLinkInfoProvider.SetLinkInfo(newLink.Item, newLink);
					Manager.Helper.CustomizationManager.SelectObject(CustomizeSelectedLink.Item);
				}
			}
		}
		protected virtual bool CanSelect(BarItemLink link) {
			if(link == null) return false;
			if(!link.CanSelectInCustomization) return false;
			return true;
		}
		protected virtual bool CanPress(BarItemLink link) {
			if(link == null) return false;
			if(!link.CanPress) return false;
			return true;
		}
		public BarItemLink PressedLink { 
			get { return pressedLink; }
			set {
				if(PressedLink == value) return;
				if(value == null && lockNullPressed != 0) return;
				BarItemLink prevLink = PressedLink;
				pressedLink = value;
				if(prevLink != null && prevLink.Manager != null && prevLink.Item != null && !prevLink.Manager.IsDestroying) prevLink.CheckUpdateLinkState();
				if(PressedLink != null) PressedLink.CheckUpdateLinkState();
				Manager.RaisePressedLinkChanged(new HighlightedLinkChangedEventArgs(prevLink, PressedLink));
			}
		}
		public virtual void UpdatePopupHighlighting() {
			if(OpenedPopups.Count == 0) return;
			for(int n = OpenedPopups.Count - 1; n >= 0; n--) {
				IPopup popup = OpenedPopups[n] as IPopup;
				if(popup.OwnerLink != null) popup.OwnerLink.CheckUpdateLinkState();
			}
		}
		protected virtual void CheckPopups(BarItemLink newLink) {
			if(OpenedPopups.Count == 0 || newLink == null) return;
			for(int n = OpenedPopups.Count - 1; n >= 0; n--) {
				IPopup popup = OpenedPopups[n] as IPopup;
				if(popup.ContainsLink(newLink) || popup.OwnerLink == newLink) return;
				ClosePopup(popup);
			}
		}
		protected virtual void CheckAndClosePopups(BarItemLink newLink) {
			if(OpenedPopups.Count == 0 || newLink == null) return;
			for(int n = OpenedPopups.Count - 1; n >= 0; n--) {
				IPopup popup = OpenedPopups[n] as IPopup;
				if(popup.ContainsLink(newLink)) return;
				if(popup.RibbonToolbar != null && popup.RibbonToolbar.ContainsLink(newLink)) return;
				IBarLinkTimer timerLink = newLink as IBarLinkTimer;
				if((newLink is BarCustomContainerItemLink) || (timerLink != null && timerLink.CanStartTimer)) continue;
				if(popup.OwnerLink != null) {
					ClearHighlightTimer(true);
					InitHighligthTimer(popup.OwnerLink);
				} else
					ClosePopup(popup);
			}
		}
		protected internal virtual void CheckCanChildPopup(IPopup popup) {
			while(OpenedPopups.Count > 0) {
				IPopup parent = OpenedPopups[OpenedPopups.Count - 1] as IPopup;
				if(!parent.CanOpenAsChild(popup)) 
					ClosePopup(parent);
				else
					break;
			}
		}
		public virtual bool CanClosePopupsByTimer(IPopup popup) {
			for(int n = OpenedPopups.Count - 1; n >= 0; n--) {
				IPopup p = OpenedPopups[n] as IPopup;
				if(popup == p) return true;
				if(!p.CanCloseByTimer) return false;
			}
			return true;
		}
		public void OpenPopup(IPopup parentPopup, IPopup popup, LocationInfo locInfo) {
			OpenPopup(parentPopup, popup, locInfo, null);
		}
		public virtual bool CanOpenPopup(IPopup popup) {
			return popup != null && !OpenedPopups.Contains(popup) && popup.CanShowPopup;
		}
		protected internal SubMenuControlForm GetLastOpenedPopupForm() {
			return OpenedPopups.Count > 0? OpenedPopups[OpenedPopups.Count - 1].PopupForm as SubMenuControlForm: null;
		}
		protected internal IPopup GetPrevOpenedPopup(IPopup popup) {
			int index = OpenedPopups.IndexOf(popup);
			if(index <= 0)
				return null;
			return OpenedPopups[index - 1];
		}
		public virtual void OpenPopup(IPopup parentPopup, IPopup popup, LocationInfo locInfo, MethodInvoker createFormMethod) {
			if(!CanOpenPopup(popup)) return;
			if(OpenedPopups.Count > 0) CheckCanChildPopup(popup);
			if(createFormMethod != null) createFormMethod();
			HideToolTip();
			CloseEditor();
			if(!Manager.IsCustomizing) {
				MouseButtons buttons = Control.MouseButtons;
				if((buttons & MouseButtons.Right) != 0) manager.IgnoreMouseUp++;
				if((buttons & MouseButtons.Left) != 0) manager.IgnoreLeftMouseUp++;
			}
			popup.OpenPopup(locInfo, parentPopup);
			if(popup.IsPopupOpened) {
				OpenedPopups.Add(popup);
				ActiveBarControl = popup.CustomControl;
				if(popup.OwnerLink != null) popup.OwnerLink.Invalidate();
				if(popup.PopupForm != null) {
					popup.PopupForm.Update();
				}
				if(Manager != null && Manager.Form != null) Manager.Form.Update();
			}
		}
		public virtual void ClosePopupChildren(IPopup popup) {
			if(popup == null || !OpenedPopups.Contains(popup)) return;
			for(int n = OpenedPopups.Count - 1; n >= 0; n--) {
				IPopup p = OpenedPopups[n] as IPopup;
				if(p.ParentPopup == popup) ClosePopup(p);
			}
		}
		protected internal virtual void ActivatePreviousPopup(IPopup popupToClose) {
			int pIndex = OpenedPopups.IndexOf(ActiveBarControl);
			IPopup prevPopup = pIndex > 0 ? OpenedPopups[pIndex - 1] as IPopup : null;
			ActiveBarControl = prevPopup != null ? prevPopup.CustomControl : null;
			if(ActiveBarControl == null) {
				RestoreActiveBarControl(popupToClose);
			}
		}
		public virtual void ClosePopup(IPopup popup) {
			int index = OpenedPopups.IndexOf(popup);
			if(index == -1) return;
			HideEditor();
			try {
				for(int n = OpenedPopups.Count - 1; n >= index; n--) {
					IPopup p = OpenedPopups[n] as IPopup;
					if(p == ActiveBarControl) {
						ActivatePreviousPopup(p);
					}
					BarItemLink link = p.OwnerLink; 
					p.ClosePopup();
					OnPopupClosed(p, link);
				}
			}
			finally {
				popup.ParentPopup = null;
				if(OpenedPopups.Count >= index)
					OpenedPopups.RemoveRange(index, OpenedPopups.Count - index);
				if(OpenedPopups.Count == 0) AutoOpenMenuBar = null;
			}
			ClearHighlightTimer(true);
		}
		protected virtual void RestoreActiveBarControl(IPopup p) {
			if(p.OwnerLink != null) 
				ActiveBarControl = p.OwnerLink.BarControl;
		}
		protected virtual void OnPopupClosed(IPopup popup, BarItemLink link) {
			if(link != null) {
				if(PressedLink == link) PressedLink = null;
				link.CheckUpdateLinkState();
			}
		}
		protected internal virtual void OnCloseAll(BarMenuCloseType closeType) {
			if(!CloseEditor()) HideEditor();
			ActiveBarControl = null;
			HighlightedLink = null;
			CloseAllPopups(closeType);
		}
		protected internal void CloseDXToolbars() {
			for(int i = 0; i < Manager.Bars.Count;) {
				DXToolbar tb = Manager.Bars[i].Tag as DXToolbar;
				if(tb == null) { 
					i++;
					continue;
				}	
				tb.Dispose();
			}
		}
		public void CloseAllPopups() { CloseAllPopups(true); }
		public virtual void CloseAllPopups(BarMenuCloseType closeType) {
			CloseAllPopups(true, closeType);
		}
		public virtual void CloseAllPopups(bool killAutoOpenBar) {
			CloseAllPopups(killAutoOpenBar, BarMenuCloseType.All);
		}
		public virtual void CloseAllPopups(bool killAutoOpenBar, BarMenuCloseType closeType) {
			bool prevTemporaryActive = TemporaryActive;
			this.temporaryActive = false;
			CloseEditor(); 
			Bar autoOpenBar = AutoOpenMenuBar;
			if(killAutoOpenBar) ShowNonRecentItems = false;
			for(int n = OpenedPopups.Count - 1; n >= 0; n--) {
				if(OpenedPopups.Count == 0) break;
				if(closeType.HasFlag(BarMenuCloseType.KeepPopupContainer) && OpenedPopups[n].IsControlContainer)
					break;
				ClosePopup(OpenedPopups[n] as IPopup);
			}
			if(killAutoOpenBar)
				AutoOpenMenuBar = null;
			else
				AutoOpenMenuBar = autoOpenBar;
			if(prevTemporaryActive) Clear();
			if(closeType != BarMenuCloseType.AllExceptMiniToolbars && closeType != BarMenuCloseType.AllExceptMiniToolbarsAndDXToolbars)
				CloseDXToolbars();
		}
		public virtual bool ProcessKeyDown(KeyEventArgs e) { 
			this.altKeyPressed = false;
			if(ActiveBarControl == null) return false;
			bool res = false;
			if(ActiveBarControl.IsNeededKey(e)) {
				ActiveBarControl.ProcessKeyDown(e);
				res = true;
			}
			if(ActiveEditor != null) {
				this.altKeyPressed = IsAllowAltEditorKey(e);
				this.onlyAltPressed = true;
			}
			return res;
		}
		bool altKeyPressed = false;
		bool onlyAltPressed = false;
		bool IsAllowAltEditorKey(KeyEventArgs e) {
			PopupBaseEdit popup = ActiveEditor as PopupBaseEdit;
			return Manager.UseAltKeyForMenu && e.KeyData == Keys.Menu && !e.Handled && ActiveEditor != null && (popup == null || !popup.IsPopupOpen) && OpenedPopups.Count == 0;
		}
		public virtual bool ProcessKeyUp(KeyEventArgs e) { 
			bool prevAlt = this.altKeyPressed;
			this.altKeyPressed = false;
			if(ActiveBarControl == null) return false;
			bool isAlt = e.KeyCode == Keys.Alt || e.KeyCode == (Keys.RButton | Keys.ShiftKey);
			this.onlyAltPressed &= isAlt;
			if((!isAlt || !this.onlyAltPressed) && ActiveBarControl.IsNeededKey(e)) {
				ActiveBarControl.ProcessKeyUp(e);
				return true;
			}
			if(prevAlt && IsAllowAltEditorKey(e)) {
				CloseEditor();
				AltKeyPressed(true, true);
				return true;
			}
			return false;
		}
		public virtual void MoveBarSelection() { 
			if(ActiveBarControl == null) return;
			CloseAllPopups();
			CustomBarControl cb = ActiveBarControl as CustomBarControl;
			if(cb == null || cb.Bar == null) return;
			int bi = Manager.Bars.IndexOf(cb.Bar);
			if(bi == -1) return;
			cb = null;
			for(int c = 0; c < 2; c++) {
				for(int n = bi + 1; n < Manager.Bars.Count; n++) {
					Bar bar = Manager.Bars[n];
					if(!bar.Visible) continue;
					cb = bar.BarControl;
					if(cb != null) break;
				}
				if(cb == null) 
					bi = -1;
				else 
					break;
			}
			ActiveBarControl = cb;
			if(cb != null) {
				MoveLinkSelectionHorizontal(cb.GetVisibleLinks(), BarLinkNavigation.First);
			}
		}
		public virtual void MoveLinkSubSelectionHorizontal(IPopup popup, BarLinkNavigation nav) {
			CustomPopupBarControl control = popup.CustomControl as CustomPopupBarControl;
			if(control != null && control.HasGalleryItems) {
				MoveLinkSelectionHorizontalInGalleryMenu(popup, nav);
				return;
			}
			IPopup topPopup = OpenedPopups.RootPopup;
			Bar bar;
			if(topPopup == null) return;
			bar = topPopup.PopupCreator as Bar;
			if(nav == BarLinkNavigation.Right) {
				if(bar == null || bar.BarControl == null) return;
				HighlightedLink = topPopup.OwnerLink;
				MoveLinkSelectionHorizontal(bar.BarControl.GetVisibleLinks(), BarLinkNavigation.Right, Keys.None);
				return;
			}
			if(nav == BarLinkNavigation.Left) {
				if(popup != topPopup & OpenedPopups.Count > 1) {
					BarItemLink ownerLink = popup.OwnerLink;
					CustomLinksControl linksControl = ActiveBarControl as CustomLinksControl;
					bool shouldActivateParentKeyTip = false;
					if (linksControl != null && linksControl.KeyTipManager.Show)
					{
						linksControl.KeyTipManager.HideKeyTips();
						shouldActivateParentKeyTip = true;
					}
					ClosePopup(popup);
					if(shouldActivateParentKeyTip) linksControl.KeyTipManager.ActivateParentKeyTips();
					KeyboardHighlightedLink = ownerLink;
					return;
				}
				if(bar == null || bar.BarControl == null) return;
				HighlightedLink = topPopup.OwnerLink;
				MoveLinkSelectionHorizontal(bar.BarControl.GetVisibleLinks(), BarLinkNavigation.Left, Keys.None);
				return;
			}
		}
		protected virtual void MoveLinkSelectionHorizontalInGalleryMenu(IPopup popup, BarLinkNavigation nav) {
			CustomPopupBarControl ctrl = popup.CustomControl as CustomPopupBarControl;
			if(ctrl == null)
				return;
			CustomSubMenuBarControlViewInfo vi = ctrl.ViewInfo as CustomSubMenuBarControlViewInfo;
			if(vi == null)
				return;
			if(HighlightedLink == null) {
				HighlightedLink = ctrl.VisibleLinks[0];
				return;
			}
			int currentIndex = ctrl.VisibleLinks.IndexOf(HighlightedLink);
			SubMenuLinkInfo subInfo = vi.GetLinkViewInfo(HighlightedLink, vi.BarLinksViewInfo);
			if(subInfo == null || !subInfo.MultiColumn)
				return;
			if(nav == BarLinkNavigation.Left) {
				if(subInfo.Column == 0) {
					SubMenuLinkInfo next = vi.GetLastGalleryItemInRow(subInfo);
					HighlightedLink = next.LinkInfo.Link;
				}
				else {
					HighlightedLink = ctrl.VisibleLinks[currentIndex - 1];
				}
			}
			else if(nav == BarLinkNavigation.Right) {
				if(subInfo == vi.GetLastGalleryItemInRow(subInfo)) {
					HighlightedLink = ctrl.VisibleLinks[currentIndex - subInfo.Column];
				}
				else {
					HighlightedLink = ctrl.VisibleLinks[currentIndex + 1];
				}
			}
		}
		public void MoveLinkSelectionHorizontal(BarItemLinkReadOnlyCollection links, BarLinkNavigation nav) {
			MoveLinkSelectionHorizontal(links, nav, Keys.Tab);
		}
		public virtual void MoveLinkSelectionHorizontal(BarItemLinkReadOnlyCollection links, BarLinkNavigation nav, Keys key) {
			if(nav == BarLinkNavigation.Left) nav = BarLinkNavigation.Up;
			if(nav == BarLinkNavigation.Right) nav = BarLinkNavigation.Down;
			MoveLinkSubSelectionVertical(links, nav, key);
		}
		public void MoveLinkSubSelectionVertical(BarItemLinkReadOnlyCollection links, BarLinkNavigation nav) {
			MoveLinkSubSelectionVertical(links, nav, Keys.None);
		}
		protected internal virtual int GetTopLinkIndex(BarItemLinkReadOnlyCollection links) {
			for(int i = 0; i < links.Count; i++) {
				if(links[i].Bounds != Rectangle.Empty) {
					return links[i] is BarScrollItemLink ? i + 1 : i;
				}
			}
			return -1;
		}
		protected internal virtual int GetBottomLinkIndex(BarItemLinkReadOnlyCollection links) {
			bool findFirst = false;
			for(int i = 0; i < links.Count; i++) {
				if(links[i].Bounds != Rectangle.Empty)
					findFirst = true;
				else if(findFirst) {
					return links[i] is BarScrollItemLink? i - 2: i - 1;
				}
			}
			return links.Count - 1;
		}
		protected internal virtual int GetVisibleLinkCount(BarItemLinkReadOnlyCollection links) {
			int topIndex = GetTopLinkIndex(links);
			int bottomIndex = GetBottomLinkIndex(links);
			return bottomIndex - topIndex + 1;
		}
		protected virtual BarItemLink GetBottomCorrectLink(int index, BarItemLinkReadOnlyCollection links) { 
			for(int i = index; i >= 0; i--) {
				if(!(links[i] is BarScrollItemLink)) return links[i];
			}
			return null;
		}
		public virtual void MoveLinkSubSelectionVertical(BarItemLinkReadOnlyCollection links, BarLinkNavigation nav, Keys key) {
			if(!CloseEditor()) return;
			BarItemLink current = HighlightedLink, link = null;
			int currentIndex = links.IndexOf(current);
			if(nav == BarLinkNavigation.PageDown && currentIndex != -1) {
				int bottomLinkIndex = GetBottomLinkIndex(links);
				if(currentIndex < bottomLinkIndex)
					currentIndex = bottomLinkIndex;
				else { 
					currentIndex += GetVisibleLinkCount(links);
					if(currentIndex >= links.Count) currentIndex = links.Count - 1;
				}
				KeyboardHighlightedLink = GetBottomCorrectLink(currentIndex, links);
				return;
			}
			if(nav == BarLinkNavigation.PageUp && currentIndex != -1) {
				int topLinkIndex = GetTopLinkIndex(links);
				if(currentIndex > topLinkIndex)
					currentIndex = topLinkIndex;
				else {
					currentIndex -= GetVisibleLinkCount(links);
					if(currentIndex < 0) currentIndex = 0;
				}
				KeyboardHighlightedLink = links[currentIndex] is BarScrollItemLink ? links[currentIndex + 1] : links[currentIndex];
				return;
			}
			if(nav == BarLinkNavigation.Down && currentIndex == links.Count - 1) {
				nav = BarLinkNavigation.First;
			}
			if(nav == BarLinkNavigation.First || currentIndex == -1) {
				if(nav == BarLinkNavigation.Up)
					link = GetLastSelectableLink(links);
				else
					link = GetFirstSelectableLink(links);
				current = null;
				currentIndex = -1;
			} 
			if(nav == BarLinkNavigation.Up) {
				if(currentIndex == 0) nav = BarLinkNavigation.Last;
			}
			if(nav == BarLinkNavigation.Last) {
				int n = links.Count - 1;
				while(n > -1) {
					link = links[n--];
					if(!link.Item.CanKeyboardSelect || (link is BarRecentExpanderItemLink)) {
						link = null;
						continue;
					}
					break;
				}
				current = null;
				currentIndex = -1;
			}
			if(current != null) {
				for(int pass = 0; pass < links.Count; pass++) {
					currentIndex += (nav == BarLinkNavigation.Up ? GetDeltaTop(current) : GetDeltaDown(current));
					if(currentIndex < 0) currentIndex = 0;
					if(currentIndex > links.Count - 1) currentIndex = links.Count - 1;
					link = links[currentIndex];
					if(!link.Enabled || !link.Item.CanKeyboardSelect) {
						link = null;
						continue;
					}
					break;
				}
			}
			if(link == null) {
				if(links.Count > 0) link = links[0];
			}
			KeyboardHighlightedLink = link;
			if(link == null) return;
			if(key == Keys.Tab || key == Keys.Left || key == Keys.Right) link.Focus();
		}
		private int GetDeltaDown(BarItemLink current) {
			CustomPopupBarControl ctrl = current.BarControl as CustomPopupBarControl;
			if(ctrl == null || !ctrl.GetLinkMultiColumn(current))
				return 1;
			CustomSubMenuBarControlViewInfo vi = (CustomSubMenuBarControlViewInfo)ctrl.ViewInfo;
			SubMenuLinkInfo subInfo = vi.GetLinkViewInfo(current, vi.BarLinksViewInfo);
			int galleryItemEndIndex = vi.GetGalleryEndItemIndex(current);
			int galleryColumnCount = vi.GetGalleryColumnCount(current);
			int currentIndex = ctrl.VisibleLinks.IndexOf(current);
			int rowCount = vi.GetGalleryRowCount(current);
			if(subInfo.Row == rowCount - 1)
				return (galleryItemEndIndex - currentIndex + 1);
			if(currentIndex + galleryColumnCount <= galleryItemEndIndex)
				return galleryColumnCount;
			return galleryItemEndIndex - currentIndex;
		}
		private int GetDeltaTop(BarItemLink current) {
			CustomPopupBarControl ctrl = current.BarControl as CustomPopupBarControl;
			if(ctrl == null || !ctrl.GetLinkMultiColumn(current))
				return -1;
			CustomSubMenuBarControlViewInfo vi = (CustomSubMenuBarControlViewInfo)ctrl.ViewInfo;
			SubMenuLinkInfo subInfo = vi.GetLinkViewInfo(current, vi.BarLinksViewInfo);
			if(subInfo.Row > 0)
				return - vi.GetGalleryColumnCount(current);
			return - subInfo.Column - 1;
		}
		protected BarItemLink GetFirstSelectableLink(BarItemLinkReadOnlyCollection links) {
			BarItemLink link = null;
			for(int n = 0; n < links.Count; n++) {
				link = links[n];
				if(!link.Item.CanKeyboardSelect || (link is BarRecentExpanderItemLink) || (link is BarSystemMenuItemLink)) {
					link = null;
					continue;
				}
				return link;
			}
			return link;
		}
		protected BarItemLink GetLastSelectableLink(BarItemLinkReadOnlyCollection links) {
			BarItemLink link = null;
			for(int n = links.Count - 1; n >= 0; n--) {
				link = links[n];
				if(!link.Item.CanKeyboardSelect || (link is BarRecentExpanderItemLink) || (link is BarSystemMenuItemLink)) {
					link = null;
					continue;
				}
				return link;
			}
			return link;
		}
		public virtual bool CanSetLinkKeyboardSelection(BarItemLink link) {
			return true;
		}
		protected bool ignoreNextAltUp = false;
		public virtual bool AltKeyPressed(bool keyUp, bool isAltKey) {
			ShowHotKeyPrefix();
			if(!Manager.IsBarsActive || Manager.IsCustomizing) return false;
			if(ActiveEditor != null) return false;
			if(!keyUp && OpenedPopups.Count > 0) {
				ignoreNextAltUp = true;
				ActiveBarControl = null;
				OnCloseAll(BarMenuCloseType.All);
				HighlightedLink = null;
				return true;
			}
			if(isAltKey && !Manager.UseAltKeyForMenu) return false;
			if(!IsAllowActivateMenu) return false;
			return CheckActivateMainMenu(keyUp);
		}
		protected virtual void ShowHotKeyPrefix() {
			if(Manager.PaintStyle.DrawParameters.SingleLineStringFormat.HotkeyPrefix == System.Drawing.Text.HotkeyPrefix.Hide) {
				Manager.PaintStyle.DrawParameters.SingleLineStringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
				Manager.PaintStyle.DrawParameters.SingleLineVerticalStringFormat.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
				if(Manager.MainMenu != null)
					Manager.MainMenu.Invalidate();
			}
		}
		protected virtual bool HasOpenedPopups {
			get { return OpenedPopups.Count > 0; }
		}
		protected virtual bool IsAllowActivateMenu {
			get {
				if((Manager.Form is Form) && (Manager.Form as Form).IsMdiChild) return false;
				Bar menu = Manager.MainMenu;
				if(menu == null || !menu.Visible || menu.BarControl == null) return false;
				return true;
			}
		}
		protected virtual bool IsMainMenuActive { get { return ActiveBarControl != null; } }
		protected internal bool IsAltKeyPressed { get; set; }
		protected virtual void ActivateMainMenu() {
			IsAltKeyPressed = true;
			Bar menu = Manager.MainMenu;
			if(menu == null) return;
			if(menu.BarControl == null || !menu.BarControl.ViewInfo.IsReady) return;
			ActiveBarControl = menu.BarControl;
			MoveLinkSelectionHorizontal(menu.BarControl.GetVisibleLinks(), BarLinkNavigation.First);
			UpdateBars();
		}
		private void UpdateBars() {
			foreach(Bar bar in Manager.Bars) {
				if(bar.BarControl != null && bar.BarControl.Visible) {
					bar.BarControl.UpdateViewInfo();
					bar.BarControl.Invalidate();
				}
			}
		}
		protected virtual void DeactivateMainMenu() {
			Clear();
		}
		protected virtual bool CheckActivateMainMenu(bool keyUp) {
			if(!keyUp) {
				if(IsMainMenuActive) {
					DeactivateMainMenu();
					this.ignoreNextAltUp = true;
					return true;
				}
			}
			else {
				if(ignoreNextAltUp) {
					ignoreNextAltUp = false;
					return true;
				}
				ActivateMainMenu();
				return true;
			}
			return false;
		}
		public virtual void UnLockNullPressedChanging() {
			if(lockNullPressed > 0) lockNullPressed --;
		}
		public virtual void LockNullPressedChanging() {
			lockNullPressed ++;
		}
		BarShortcut currentShortcut = BarShortcut.Empty;
		int lastClick = System.Environment.TickCount;
		const int doubleKeyClickInterval = 2000;
		public virtual void SetCurrentShortcut(BarShortcut shortcut) {
			currentShortcut = shortcut;
			lastClick = System.Environment.TickCount;
		}
		public virtual BarShortcut CurrentShortcut { 
			get { return currentShortcut; }
		}
		public virtual BarManagerHookResult CheckShortcut(BarManager manager, KeyEventArgs e) {
			BarManagerHookResult res = BarManagerHookResult.NotProcessed;
			if((e.KeyData & (~Keys.Modifiers)) == 0) return res;
			if(e.KeyCode == Keys.Alt || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey) return res;
			if(!manager.IsBarsActive || manager.IsCustomizing) {
				SetCurrentShortcut(BarShortcut.Empty);
				return res;
			}
			int tc = System.Environment.TickCount - lastClick;
			BarShortcut sh;
			BarItem item;
			if(CurrentShortcut.IsExist && tc < doubleKeyClickInterval) {
				sh = new BarShortcut(CurrentShortcut.Key, e.KeyData);
				SetCurrentShortcut(BarShortcut.Empty);
				item = ItemByShortcut(sh);
				if(item != null && item.Enabled) {
					return OnShortcutItemClick(item, sh);
				}
				return BarManagerHookResult.ProcessedExit;
			}
			sh = new BarShortcut(e.KeyData);
			if(IsExistSecondShortcut(sh)) {
				SetCurrentShortcut(sh);
				throw new BarHideException();
			}
			SetCurrentShortcut(BarShortcut.Empty);
			item = ItemByShortcut(sh);
			return OnShortcutItemClick(item, sh);
		}
		protected virtual BarManagerHookResult OnShortcutItemClick(BarItem item, BarShortcut shortcut) {
			if(item == null || !item.Enabled) return BarManagerHookResult.NotProcessed;
			ShortcutItemClickEventArgs e = new ShortcutItemClickEventArgs(item, shortcut);
			Manager.RaiseShortcutItemClick(e);
			if(e.Cancel) return BarManagerHookResult.NotProcessed;
			item.OnClick(null);
			return BarManagerHookResult.ProcessedExit;
		}
		protected virtual BarItem ItemByShortcut(BarShortcut sh, BarItems items, bool skipDisabled) {
			if(items == null)
				return null;
			if(!sh.IsExist) return null;
			foreach(BarItem item in items) {
				if(item.GetRunTimeVisibility() && (!skipDisabled || item.Enabled)) {
					if(item.ItemShortcut == sh) return item;
				}
			}
			return null;
		}
		protected virtual BarItem ItemByShortcut(BarShortcut sh) {
			BarItem item = ItemByShortcut(sh, manager.Items, true);
			if(item != null)
				return item;
			item = ItemByShortcut(sh, manager.MergedItems, true);
			if (item != null)
				return item;
			item = ItemByShortcut(sh, manager.Items, false);
			if (item != null)
				return item;
			return ItemByShortcut(sh, manager.MergedItems, false);
		}
		protected virtual bool IsExistSecondShortcut(BarShortcut sh, BarItems items) {
			if(items == null)
				return false;
			foreach(BarItem item in items) {
				if(item.GetRunTimeVisibility()) {
					if(item.ItemShortcut.IsExist) {
						if(item.ItemShortcut.Key == sh.Key && item.ItemShortcut.SecondKey != Keys.None) return true;
					}
				}
			}
			return false;
		}
		protected virtual bool IsExistSecondShortcut(BarShortcut sh) {
			if(IsExistSecondShortcut(sh, manager.Items))
				return true;
			return IsExistSecondShortcut(sh, manager.MergedItems);
		}
		protected virtual IBarObject GetBarObject(Control ctrl) {
			while(ctrl != null) {
				IBarObject bo = ctrl as IBarObject;
				if(bo != null) return bo;
				ctrl = ctrl.Parent;
			}
			return null;
		}
		protected virtual bool CheckIsBarObject(Control ctrl) {
			IBarObject bo = GetBarObject(ctrl);
			if(bo != null) {
				if(bo.Manager == Manager) return true;
				if(Manager.Helper.CustomizationManager.DesignTimeManager != null) {
					if(manager.Helper.CustomizationManager.DesignTimeManager.DesignManager == bo.Manager) return true;
				}
			}
			return false;
		}
		public virtual bool ShouldInterceptKey(KeyEventArgs e) {
			if(Manager.IsDesignMode) return false;
			Form csForm = Manager.Helper.CustomizationManager.CustomizationForm;
			if(csForm == null) return true;
			Form actForm = Form.ActiveForm;
			if(actForm == null) return true;
			if(actForm is DevExpress.XtraBars.Customization.BarForm) return false;
			Control ctrl = actForm.ActiveControl;
			if (ctrl == null && !BarNativeMethods.IsWindowEnabled(csForm.Handle)) return false;
			if(ctrl == null) return true;
			if(csForm == ctrl || csForm.Contains(ctrl)) return false;
			if(csForm == actForm) return false;
			if(actForm is EditForm) return false;
			if(!csForm.Enabled) return false;
			return true;
		}
		public virtual bool ShouldInterceptMouse(Control ctrl) {
			if(Manager.IsDesignMode || Manager.Helper.CustomizationManager.IsHotCustomizing) return false;
			Form csForm = Manager.Helper.CustomizationManager.CustomizationForm;
			Control activeControl = Form.ActiveForm == null ? null : Form.ActiveForm.ActiveControl;
			Form form = Form.ActiveForm;
			if(csForm == null) return true;
			if (ctrl == null && !BarNativeMethods.IsWindowEnabled(csForm.Handle)) return false;
			if(Manager.Helper.DragManager.IsDragging) return false;
			if(ctrl == null) return true;
			if(CheckIsBarObject(ctrl)) return false;
			if(GetBarObject(ctrl) != null) return true;
			if(ctrl is CustomControl || ctrl is CustomFloatingForm || ctrl is DockingManager) return false;
			if(csForm == ctrl || csForm.Contains(ctrl)) return false;
			Form fForm = ctrl.FindForm();
			if(fForm is DevExpress.XtraEditors.Popup.PopupBaseForm) return false;
			if(fForm is SubMenuControlForm) return false;
			if(fForm == csForm) return false;
			if(fForm is EditForm) return false;
			if(fForm is XtraMessageBoxForm) return false;
			return true;
		}
		protected internal virtual void OnLinkClicked(BarItemLink barItemLink) { }
		protected internal virtual void OnItemClickClear(BarItemLink link) {
			Clear();
		}
		#region IBarPopupCollectionOwner Members
		void IBarPopupCollectionOwner.OnCollectionChanged() {
			OnOpenedPopupsCollectionChanged();
		}
		protected virtual void OnOpenedPopupsCollectionChanged() {
		}
		#endregion
		protected internal void ProcessRadialMenuOuterClick(BarManager manager, Control control, MouseInfoArgs mouseInfoArgs) {
			foreach(BarLinksHolder holder in manager.ItemHolders) {
				RadialMenu menu = holder as RadialMenu;
				if(menu != null) menu.ProcessMouseOuterClick(control, mouseInfoArgs);
			}
		}
		protected internal virtual void HideToolTip() {
			if(Manager.GetToolTipController().ActiveObject == Manager) 
				Manager.GetToolTipController().HideHint();
		}
		public System.Collections.Generic.List<IPopupControl> PopupNavigationList { get; set; }
	}
}
