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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Forms;
using DevExpress.Utils;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Ribbon.Internal;
namespace DevExpress.XtraBars.Controls {
	[ToolboxItem(false)]
	public class CustomPopupBarControl : CustomLinksControl, IPopup, IFormContainedControl, ISmartDesignerActionListOwner {
		int lockCloseUp = 0;
		public class InternalSubControlLinks : InternalControlLinks {
			public InternalSubControlLinks(CustomLinksControl owner) : base(owner) { }
			public override void CreateLinks() {
				this[ControlLink.UpScroll] = Manager.InternalItems.ScrollItem.CreateLink(null, Owner);
				this[ControlLink.DownScroll] = Manager.InternalItems.ScrollItem.CreateLink(null, Owner);
				(this[ControlLink.UpScroll] as BarScrollItemLink).ScrollDown = false;
				(this[ControlLink.DownScroll] as BarScrollItemLink).ScrollDown = true;
				base.CreateLinks();
			}
			public BarScrollItemLink UpScrollLink { get { return this[ControlLink.UpScroll] as BarScrollItemLink; } }
			public BarScrollItemLink DownScrollLink { get { return this[ControlLink.DownScroll] as BarScrollItemLink; } }
		}
		IPopup parentPopup = null;
		ControlForm form;
		int topLinkIndex;
		public event EventHandler PopupClose;
		internal CustomPopupBarControl(BarManager barManager, IList linksSource) : base(barManager, linksSource) {
			this.topLinkIndex = 0;
			this.form = null;
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}
		Rectangle IPopup.PopupChildForceBounds { get { return PopupChildForceBoundsCore; } }
		IPopup IPopup.ParentPopup { get { return parentPopup; } set { parentPopup = value; } }
		RibbonMiniToolbar IPopup.RibbonToolbar { get { return RibbonToolbarCore; } }
		protected virtual RibbonMiniToolbar RibbonToolbarCore { get { return null; } }
		protected virtual Rectangle PopupChildForceBoundsCore { get { return Rectangle.Empty; } }
		protected internal virtual void LockCloseUp() {
			this.lockCloseUp++;
		}
		protected internal virtual void UnLockCloseUp() {
			if(--this.lockCloseUp == 0) FireCloseUp();
		}
		protected internal bool IsCloseUpLocked { get { return lockCloseUp != 0; } }
		protected virtual void FireCloseUp() {
		}
		public override bool ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) {
			if(PopupHelper.IsBelowModalForm(this, BarManager.ClosePopupOnModalFormShow))
				return false;
			return base.ShouldCloseOnOuterClick(control, e);
		}
		protected override InternalControlLinks CreateInternalControlLinks() {
			return new InternalSubControlLinks(this); 
		}
		public new InternalSubControlLinks ControlLinks { get { return base.ControlLinks as InternalSubControlLinks; } }
		protected internal int TopLinkIndex {
			get { return topLinkIndex; }
			set {
				if(value < 0) value = 0;
				if(value > VisibleLinks.Count - 1) value = VisibleLinks.Count - 1;
				if(TopLinkIndex == value) return;
				this.topLinkIndex = value;
				HideKeyTips();
				UpdateViewInfo();
				Invalidate(ViewInfo.Bounds);
			}
		}
		protected override void OnMouseMoveCore(MouseEventArgs e) {
			if(SubMenuViewInfo != null && SubMenuViewInfo.ShowNavigationHeader) {
				SubMenuViewInfo.ProcessNavigationItemsMouseMove(e);
			}
			base.OnMouseMoveCore(e);
		}
		CustomSubMenuBarControlViewInfo SubMenuViewInfo {
			get { return ViewInfo as CustomSubMenuBarControlViewInfo; }
		}
		protected override void OnMouseDownCore(DXMouseEventArgs ee) {
			if(SubMenuViewInfo != null && SubMenuViewInfo.ShowNavigationHeader) {
				SubMenuViewInfo.ProcessNavigationItemsMouseDown(ee);
			}
			base.OnMouseDownCore(ee);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(Destroying) return;
			if(SubMenuViewInfo != null && SubMenuViewInfo.ShowNavigationHeader) {
				if(SubMenuViewInfo.ProcessNavigationItemsMouseUp(e))
					return;
			}
			base.OnMouseUp(e);
		}
		Form IPopup.PopupForm { get { return Form; } }
		public virtual void RaisePaintMenuBar(BarCustomDrawEventArgs e) {
		}
		protected internal virtual bool GetLinkMultiColumn(BarItemLink link) {
			if(link is BarHeaderItemLink || link is BarScrollItemLink || link is BarEditItemLink) return false;
			BarHeaderItem headerItem = GetHeaderItemForLink(link);
			if(headerItem != null && headerItem.MultiColumn != DefaultBoolean.Default)
				return headerItem.MultiColumn == DefaultBoolean.True;
			BarLinkContainerItemLink subLink = OwnerLink as BarLinkContainerItemLink;
			if(subLink != null)
				return ((BarLinkContainerItem)subLink.Item).MultiColumn == DefaultBoolean.True ? true : false;
			if(PopupCreator is PopupMenu)
				return ((PopupMenu)PopupCreator).MultiColumn == DefaultBoolean.True ? true : false;
			return false;
		}
		protected internal BarHeaderItem GetHeaderItemForLink(BarItemLink link) {
			int index = VisibleLinks.IndexOf(link);
			for(int i = index - 1; i >= 0; i--) {
				if(VisibleLinks[i] is BarHeaderItemLink)
					return (BarHeaderItem)(VisibleLinks[i].Item);
			}
			return null;
		}
		protected internal OptionsMultiColumn GetOptionsGalleryMenu() {
			BarLinkContainerItemLink link = OwnerLink as BarLinkContainerItemLink;
			if(link != null)
				return ((BarLinkContainerItem)link.Item).OptionsMultiColumn;
			if(PopupCreator is PopupMenu)
				return ((PopupMenu)PopupCreator).OptionsMultiColumn;
			return null;
		}
		protected internal override void UpdateVisibleLinks() {
			if(LockLayout == 0)
			this.topLinkIndex = 0;
			base.UpdateVisibleLinks();
			ControlLinks.UpdateOwner(this);
		}
		protected int CalcVisibleItemsCount() {
			int itemsCount = 0;
			for(int i = 0; i < VisibleLinks.Count; i++){
				if(VisibleLinks[i] is BarScrollItemLink) continue;
				if(VisibleLinks[i].Bounds.Height > 0)
					itemsCount++;
			}
			return itemsCount ;
		}
		protected int CalcVisibleScrollItemsCount() {
			int scrollButtonsCount = 0;
			for(int i = 0; i < VisibleLinks.Count; i++) {
				if(VisibleLinks[i] is BarScrollItemLink) 
					scrollButtonsCount++;
			}
			return scrollButtonsCount;
		}
		int maxTopItemIndex = 0;
		protected override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			int lineCount = - e.Delta / 120;
			if(VisibleLinks.Count == 0)
				return;
			if(lineCount > 0 && !VisibleLinks.Contains((BarItemLink)ControlLinks[ControlLink.DownScroll]))
				return;
			if(lineCount < 0 && !VisibleLinks.Contains((BarItemLink)ControlLinks[ControlLink.UpScroll]))
				return;
			TopLinkIndex += lineCount;
			TopLinkIndex = Math.Min(TopLinkIndex, maxTopItemIndex);
		}
		public virtual int MenuBarWidth { get { return 0; } }
		protected override LinksNavigation CreateLinksNavigator() {
			return new VerticalLinksNavigation(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Form != null) Form.Controls.Remove(this);
				ControlForm prevForm = Form;
				this.form = null;
				if(prevForm != null && !prevForm.Disposing) prevForm.Dispose();
			}
			base.Dispose(disposing);
		}
		public override bool IsSubMenu { get { return true; } }
		public ControlForm Form { get { return form; } set { form = value; } }
		Size IFormContainedControl.CalcSize(int width, int maxHeight) {
			return CalcSize(width, maxHeight);
		}
		void IFormContainedControl.CalcViewInfo() {
			CheckDirty();
		}
		void IFormContainedControl.SetParentForm(ControlForm form) { this.Form = form; }
		Size IFormContainedControl.FormMimimumSize { get { return FormMinimumSizeCore; } }
		protected virtual Size FormMinimumSizeCore { get { return Size.Empty; } }
		public virtual bool CanHighlight(BarItemLink link) { return VisibleLinks.Contains(link); }
		public virtual bool LockHighlight(BarItemLink newLink) { return false; }
		public virtual bool CanCloseByTimer { get { return false; } }
		public virtual bool CanOpenAsChild(IPopup popup) { return false; }
		void IPopup.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheel(e);
		}
		bool IPopup.ContainsLink(BarItemLink link) { return PopupContainsLink(link); }
		protected virtual bool PopupContainsLink(BarItemLink link) { return VisibleLinks.Contains(link) || ControlLinks.EmptyLink == link; } 
		Rectangle IPopup.Bounds { get { return new Rectangle(this.PointToScreen(Point.Empty), ClientSize); } }
		CustomControl IPopup.CustomControl { get { return this; } }
		bool IPopup.IsControlContainer { get { return IsControlContainerCore; } }
		protected virtual bool IsControlContainerCore { get { return false; } }
		protected int CalcMaxTopItemIndex() {
			return VisibleLinks.Count - CalcVisibleItemsCount() - CalcVisibleScrollItemsCount();
		}
		public virtual void OpenPopup(LocationInfo locInfo, IPopup parentPopup) {
			locInfo.WindowSize = Form.Size;
			this.parentPopup = parentPopup;
			Point location = locInfo.ShowPoint;
			Form.locationInfo = locInfo;
			if(!locInfo.ForceFormBounds.IsEmpty) Form.Size = locInfo.ForceFormBounds.Size;
			if(Manager.GetPopupShowMode(parentPopup) != PopupShowMode.Inplace || !Form.Visible)
				Form.Location = location;
			UpdateFormHeight(locInfo.CalcMaxHeight());
			this.Visible = true;
			ShowForm();
			maxTopItemIndex = CalcMaxTopItemIndex();
			this.AccessibilityNotifyClients(AccessibleEvents.SystemMenuPopupStart, -1);
		}
		protected virtual void UpdateFormHeight(int maxHeight) { } 
		protected virtual void ShowForm() {
			if(Form.Visible) {
				Form.LayoutChanged();
				return;
			}
			if(AllowAnimation) AnimateShowForm();
			Form.Visible = true;
		}
		protected virtual bool AllowAnimation { get { return false; } }
		protected virtual AnimationType GetAnimationType() { return Manager.GetProperties().MenuAnimationType; }
		protected virtual void AnimateShowForm() {
			AnimationType anType = GetAnimationType();
			if(Manager.IsDesignMode || anType == AnimationType.None || Manager.designTimeManager) return;
			Manager.SelectionInfo.AllowAnimation = false;
			Form.Animating = true;
			try {
				Form.LayoutChanged();
				Form.UpdateViewInfo();
				Animator an = new Animator(Manager.Form, Form, this);
				Form.Animator = an;
				an.Animate(anType);
			}
			finally {
				Form.Animator = null;
				Form.Animating = false;
			}
		}
		protected virtual void RemoveAnimatedItems() { }
		public virtual void ClosePopup() {
			RemoveAnimatedItems();
			if(!this.IsDisposed && Form != null) {
				this.AccessibilityNotifyClients(AccessibleEvents.SystemMenuEnd, -1);
			}
			BarManager manager = Manager;
			if(Form != null) {
				Form.Visible = false;
				if(manager != null) manager.SelectionInfo.UpdatePopupsZOrder();
			}
			if(KeyTipManager != null && KeyTipManager.Show) KeyTipManager.HideKeyTips();
			if(Manager.SelectionInfo.ActiveBarControl == this)
				Manager.SelectionInfo.ActivatePreviousPopup(this);
			RaisePopupClose();
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(Navigator.ProcessKeyDown(e, ThisActiveLink)) return;
			if(e.KeyData == Keys.F4 && Manager != null && Manager.SelectionInfo != null) {
				Manager.SelectionInfo.ClosePopup(this);
			}
		}
		public virtual BarItemLink OwnerLink { get { return null; } }
		public virtual object PopupCreator { 
			get {
				if(OwnerLink != null) {
					if(OwnerLink.Bar != null) return OwnerLink.Bar;
				}
				return null;
			}
		}
		public virtual bool IsPopupOpened { get { return Visible; } }
		public virtual bool CanShowPopup { get { return true; } }
		public virtual Rectangle PopupOwnerRectangle { get { return Rectangle.Empty ; } }
		protected virtual void RaisePopupClose() {
			if(PopupClose != null) PopupClose(this, EventArgs.Empty);
		}
		public override void LayoutChanged() {
			if(LockLayout != 0) return;
			base.LayoutChanged();
			if(Form == null) return;
			Form.LayoutChanged();
		}
		#region ISmartDesignerActionListOwner
		bool ISmartDesignerActionListOwner.AllowSmartTag(IComponent component) {
			return false;
		}
		#endregion
		protected internal virtual void OnMenuNavigationItemClick(MenuNavigationItem item) {
			int index = Manager.SelectionInfo.OpenedPopups.IndexOf(item.Popup);
			if(index >= Manager.SelectionInfo.OpenedPopups.Count)
				return;
			SubMenuControlForm form = (SubMenuControlForm)((IPopup)this).PopupForm;
			CustomPopupBarControl control = (CustomPopupBarControl)item.Popup.CustomControl;
			XtraAnimator.Current.AddAnimation(new SubMenuInplaceAnimationInfo(form, control, true));
		}
		protected internal bool HasGalleryItems {
			get { 
				BarLinkContainerItemLink subLink = OwnerLink as BarLinkContainerItemLink;
				if(subLink != null && ((BarLinkContainerItem)subLink.Item).MultiColumn == DefaultBoolean.True)
					return true;
				if(PopupCreator is PopupMenu && ((PopupMenu)PopupCreator).MultiColumn == DefaultBoolean.True)
					return true;
				foreach(BarItemLink link in VisibleLinks) {
					if(link is BarHeaderItemLink && ((BarHeaderItemLink)link).Item.MultiColumn == DefaultBoolean.True)
						return true;
				}
				return false;
			}
		}
	}
	[ToolboxItem(false)]
	public class PopupMenuBarControl : CustomPopupBarControl {
		PopupMenu menu;
		public PopupMenuBarControl(BarManager barManager, PopupMenu menu) : base(barManager, null) {
			this.menu = menu;
			ControlLinks.UpdateLinkedObject(Menu);
		}
		protected internal virtual bool AllowOpenPopup { get { return VisibleLinks.Count != 0; } }
		protected override bool AllowAnimation { get { return true; } }
		public override bool IsInterceptKey(KeyEventArgs e) {
			if(Manager.IsDesignMode && System.Windows.Forms.Form.ActiveForm is ModalTextBox)
				return false;
			return base.IsInterceptKey(e);
		}
		public override bool IsNeededKey(KeyEventArgs e) {
			if(Manager.IsDesignMode && System.Windows.Forms.Form.ActiveForm is ModalTextBox)
				return false;
			return base.IsNeededKey(e);
		}
		protected override void FireCloseUp() {
			if(Menu != null) Menu.RaiseDelayedCloseUp();
		}
		protected override RibbonMiniToolbar RibbonToolbarCore {
			get {
				return Menu.RibbonToolbar;
			}
		}
		protected override void UpdateFormHeight(int maxHeight) { 
			if(Form.Height <= maxHeight) return;
			Form.Height = maxHeight; 
			Form.locationInfo.WindowSize = new Size(Form.locationInfo.WindowSize.Width, Form.Height);
			Form.Location = Form.locationInfo.ShowPoint;
		} 
		public override void RaisePaintMenuBar(BarCustomDrawEventArgs e) {
			if(Menu != null) Menu.RaisePaintMenuBarCore(e);
		}
		public override int MenuBarWidth { 
			get { 
				if(Menu != null) return Menu.MenuBarWidth;
				return 0;
			}
		}
		public virtual PopupMenu Menu { get { return menu; } }
		protected override bool IsAllowShowMenusOnRightClick { 
			get {
				if(Menu is ApplicationMenu) return true;
				RibbonBarManager manager = Manager as RibbonBarManager;
				if(manager != null && Menu.AllowRibbonQATMenu) return true;
				return false; 
			} 
		} 
		public override bool CanOpenAsChild(IPopup popup) { 
			SubMenuBarControl sub = popup as SubMenuBarControl;
			if(sub != null && VisibleLinks.Contains(sub.ContainerLink)) return true;
			if(VisibleLinks.Contains(popup.OwnerLink)) return true;
			if(((IPopup)this) == popup.ParentPopup) return true;
			return false; 
		} 
		public override bool CanHighlight(BarItemLink link) { 
			if(OwnerLink == null) {
				return base.CanHighlight(link);
			}
			if(base.CanHighlight(link)) return true;
			return false; 
		}
		public override Rectangle PopupOwnerRectangle { 
			get { 
				if(OwnerLink != null) return OwnerLink.SourceRectangle;
				return base.PopupOwnerRectangle;
			}
		}
		public override BarItemLink OwnerLink { get { return (Menu as PopupControl).Activator as BarItemLink; } } 
		public override object PopupCreator { 
			get {
				object obj = base.PopupCreator;
				if(obj == null) return Menu;
				return obj;
			}
		}
		protected internal override void UpdateVisibleLinks() {
			base.UpdateVisibleLinks();
			UpdateMenuVisibleLinks();
		}
		bool updatingVisibleLinks = false;
		protected virtual void UpdateMenuVisibleLinks() {
			if(Menu == null || this.updatingVisibleLinks) return;
			this.updatingVisibleLinks = true;
			try {
				UpdateMenuVisibleLinksCore();
			}
			finally { this.updatingVisibleLinks = false; }
		}
		protected virtual void UpdateMenuVisibleLinksCore() {
			TotalVisibleLinks.ClearItems();
			AddVisibleLinks(VisibleLinks, Menu.ItemLinks);
			CopyLinks(TotalVisibleLinks, VisibleLinks, null);
			ControlLinks.UpdateOwner(this);
			SetLinksOwner(this);
			SetLinksOwner(TotalVisibleLinks, this);
			if(CanAddDesignTimeLink) {
				BarItemLink designLink = ControlLinks[ControlLink.DesignTimeLink];
				if(designLink != null) VisibleLinks.AddItem(designLink);
			}
		}
		protected virtual bool CanAddDesignTimeLink { get { return Menu != null && Menu.IsDesignMode; } }
		protected internal override void MakeLinkVisible(BarItemLink link) {
			CustomSubMenuBarControlViewInfo vi = ViewInfo as CustomSubMenuBarControlViewInfo;
			if(vi == null) return;
			vi.MakeLinkVisible(link);
		}
	}
	[ToolboxItem(false)]
	public class PopupContainerBarControl : CustomPopupBarControl {
		internal static int ButtonsIndent = 2;
		PopupControlContainer popupControl;
		SimpleButton closeButton;
		 protected internal PopupContainerBarControl(BarManager barManager, PopupControlContainer popupControl) : base(barManager, null) {
			this.TabStop = false;
			this.popupControl = popupControl;
			Controls.Add(popupControl);
			CreateButtons();
			popupControl.Visible = true;
			popupControl.Location = Point.Empty;
		}
		 protected override bool IsControlContainerCore { get { return true; } }
		 bool IsDXPopupMenu(Control ctrl, IPopup popup) {
			 PopupMenu menu = popup.PopupCreator as PopupMenu;
			 IDXMenuSupport menuSupport = ctrl as IDXMenuSupport;
			 if(menuSupport != null && menu != null && menuSupport.Menu == menu.MenuCreator)
				 return true;
			 return false;
		 }
		public override bool CanOpenAsChild(IPopup popup) {
			if(popup.ParentPopup == this)
				return true;
			object contextMenu = Manager.GetPopupContextMenu(PopupControl);
			if(contextMenu != null && contextMenu == popup.PopupCreator)
				return true;
			if(IsContextMenu(PopupControl, popup, 0))
				return true;
			return base.CanOpenAsChild(popup);
		}
		protected int MaxContextMenuNestedLevel { get { return 2; } }
		protected virtual bool IsContextMenu(Control control, IPopup popup, int level) {
			if(level > MaxContextMenuNestedLevel)
				return false;
			foreach(Control ctrl in control.Controls) {
				object contextMenu = Manager.GetPopupContextMenu(ctrl);
				if(contextMenu != null && contextMenu == popup.PopupCreator)
					return true;
				IDXDropDownControlOwner owner = ctrl as IDXDropDownControlOwner;
				if(owner != null && owner.DropDownControl == popup.PopupCreator)
					return true;
				if(IsDXPopupMenu(ctrl, popup))
					return true;
				if(IsContextMenu(ctrl, popup, level + 1))
					return true;
			}
			return false;
		}
		public bool ShowCloseButton { get { return PopupControl.ShowCloseButton; } }
		public bool ShowSizeGrip { get { return PopupControl.ShowSizeGrip; } }
		public override Color BackColor {
			get {
				if(Manager.GetController().LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
					return BarSkins.GetSkin(Manager.GetController().LookAndFeel).GetSystemColor(SystemColors.Control);
				return base.BackColor;
			}
			set {
				base.BackColor = value;
			}
		}
		public override Color ForeColor {
			get {
				if(Manager.GetController().LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
					return BarSkins.GetSkin(Manager.GetController().LookAndFeel).GetSystemColor(SystemColors.ControlText);
				return base.ForeColor;
			}
			set {
				base.ForeColor = value;
			}
		}
		protected void CreateButtons() {
			if(!ShowCloseButton) return;
			this.closeButton = new CloseButton();
			SetupButtons();
			Controls.Add(CloseButton);
		}
		protected virtual void SetupButtons() {
			if(CloseButton == null) return;
			CloseButton.AllowFocus = false;
			CloseButton.TabStop = false;
			((IDXFocusController)CloseButton).FocusOnMouseDown = false;
			CloseButton.Click += new EventHandler(OnCloseButtonClick);
			UpdateButtonAppearance(CloseButton);
		}
		protected virtual void UpdateButtonAppearance(SimpleButton button) {
			button.LookAndFeel.ParentLookAndFeel = Manager.GetController().LookAndFeel;
			button.GetViewInfo().Reset();
		}
		protected virtual void OnCloseButtonClick(object sender, EventArgs e) {
			PopupControl.HidePopup();
		}
		public SimpleButton CloseButton { get { return closeButton; } }
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			Refresh();
		}
		protected override bool AllowAnimation { get { return false; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				this.defaultCursor = null;
				this.Controls.Remove(PopupControl);
				if(ShowCloseButton) {
					this.Controls.Remove(CloseButton);
					CloseButton.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		public virtual PopupControlContainer PopupControl { get { return popupControl; } }
		public override bool CanHighlight(BarItemLink link) { 
			if(OwnerLink == null) return false;
			if(OwnerLink != null && OwnerLink.IsLinkInMenu) return false;
			if(base.CanHighlight(link)) return true;
			if(OwnerLink == null) return false;
			return false; 
		}
		protected override bool ShouldCloseOnChildClick {
			get { return PopupControl.CloseOnOuterMouseClick; }
		}
		void CloseChildContextMenus() {
			foreach(Control ctrl in PopupControl.Controls) {
				PopupMenuBase menu = Manager.GetPopupContextMenu(ctrl);
				if(menu != null)
					menu.HidePopup();
			}
		}
		public override bool ShouldCloseMenuOnClick(MouseInfoArgs e, Control child) {
			CloseChildContextMenus();
			if(!base.ShouldCloseMenuOnClick(e, child)) return false;
			if(Destroying) return true;
			return ShouldCloseOnOuterClick(child, e);
		}
		public override bool ShouldCloseOnLostFocus(Control newFocus) { return PopupControl.CloseOnLostFocus; }
		bool IsOwnerForControl(Control control) {
			if(control == null)
				return false;
			Form form = control is Form ? (Form)control : control.FindForm();
			while(form != null) {
				if(form.Owner == Form)
					return true;
				form = form.Owner;
			}
			return false;
		}
		public override bool ShouldCloseOnOuterClick(Control control, MouseInfoArgs e) {
			if(PopupHelper.IsBelowModalForm(this, BarManager.ClosePopupOnModalFormShow))
				return false;
			if(ClientRectangle.Contains(PointToClient(e.Location)))
				return false;
			if(IsOwnerForControl(control))
				return false;
			return true; 
		}
		public override bool LockHighlight(BarItemLink newLink) {
			if(newLink == null)
				return false;
			if(!LockHighlight(newLink, PopupControl, 0))
				return false;
			if(OwnerLink == null) 
				return true;
			if(OwnerLink.Bar != null)
				return OwnerLink.Bar.AutoPopupMode != BarAutoPopupMode.All;
			return newLink != OwnerLink; 
		}
		protected virtual bool LockHighlight(BarItemLink newLink, Control control, int level) {
			if(level >= MaxContextMenuNestedLevel)
				return true;
			foreach(Control ctrl in control.Controls) {
				PopupMenuBase menu = Manager.GetPopupContextMenu(ctrl);
				if(menu != null && menu.ItemLinks.Contains(newLink))
					return false;
				IPopup popup = newLink.BarControl as IPopup;
				if(popup != null && (IsDXPopupMenu(ctrl, popup) || popup.ParentPopup == this))
					return false;
				if(!LockHighlight(newLink, ctrl, level + 1))
					return false;
			}
			if(OwnerLink == null) 
				return true;
			if(OwnerLink.Bar != null)
				return OwnerLink.Bar.AutoPopupMode != BarAutoPopupMode.All;
			return newLink != OwnerLink; 
		}
		public override BarItemLink OwnerLink { get { return (PopupControl as PopupControl).Activator as BarItemLink; } } 
		public override object PopupCreator { 
			get {
				object obj = base.PopupCreator;
				if(obj == null) return PopupControl;
				return obj;
			}
		}
		public override Rectangle PopupOwnerRectangle { 
			get { 
				if(OwnerLink != null) return OwnerLink.SourceRectangle;
				return base.PopupOwnerRectangle;
			}
		}
		protected override Size FormMinimumSizeCore {
			get {
				return PopupControl.FormMinimumSize;
			}
		}
		public override Size CalcSize(int width, int maxHeight) {
			if(PopupControl == null) return Size.Empty;
			if(DownPoint != InvalidPoint) return new Size(width, Height);
			Size res = PopupControl.ClientSize;
			GraphicsInfo gInfo = new GraphicsInfo();
			gInfo.AddGraphics(null);
			try {
				PopupViewInfo.CalcButtonsBounds(gInfo.Graphics, this, Rectangle.Empty);
				res.Height += PopupViewInfo.ButtonsRect.Height;
				if(PopupViewInfo.ShowNavigationHeader) {
					res.Height += PopupViewInfo.CalcNavigationHeaderSize(gInfo.Graphics).Height;
				}
			}
			finally {
				gInfo.ReleaseGraphics();
			}
			return res;
		}
		public override Size CalcMinSize() {
			return CalcSize(0, -1);
		}
		public override void UpdateViewInfo() {
			ViewInfo.CalcViewInfo(null, this, ClientRectangle);
			if(ShowCloseButton)
				CloseButton.Bounds = PopupViewInfo.CloseButtonRect;
			int top = 0;
			if(PopupViewInfo.IsTopSizeBar)
				top = PopupViewInfo.ButtonsRect.Bottom;
			if(PopupViewInfo.ShowNavigationHeader)
				top += PopupViewInfo.NavigationHeaderBounds.Height;
			PopupControl.Location = new Point(PopupControl.Location.X, top);
			return;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyData == Keys.Escape || e.KeyData == (Keys.Alt | Keys.F4)) {
				Manager.SelectionInfo.ClosePopup(this);
				return;
			}
			base.OnKeyDown(e);
		}
		bool currentSizing;
		protected internal bool CurrentSizing { get { return currentSizing; } set { currentSizing = value; } }
		SizeGripPosition sizeGripPos = SizeGripPosition.RightBottom;
		protected internal SizeGripPosition CurrentSizeGripPosition {
			get { return sizeGripPos; }
			set { sizeGripPos = value; }
		}
		bool currentIsTopBar;
		protected internal bool CurrentIsTopSizeBar {
			get { return currentIsTopBar; }
			set { currentIsTopBar = value; }
		}
		protected internal PopupContainerControlViewInfo PopupViewInfo { get { return ViewInfo as PopupContainerControlViewInfo; } }
		Point downPoint = new Point(-10000, -10000);
		protected Point DownPoint { get { return downPoint; } set { downPoint = value; } }
		protected Point InvalidPoint { get { return new Point(-10000, -10000); } }
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(PopupViewInfo.SizeGripRect.Contains(e.Location)) {
				CurrentIsTopSizeBar = PopupViewInfo.IsTopSizeBar;
				CurrentSizeGripPosition = PopupViewInfo.SizeGripInfo.GripPosition;
				CurrentSizing = true;
				Form.SuppressUpdateLocationInfo = true;
				DownPoint = e.Location;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			DownPoint = InvalidPoint;
			Form.SuppressUpdateLocationInfo = false;
		}
		Cursor GetCursorBySizeGripPosition(SizeGripPosition pos) {
			if(pos == SizeGripPosition.RightBottom || pos == SizeGripPosition.LeftTop)
				return Cursors.SizeNWSE;
			return Cursors.SizeNESW;
		}
		Cursor defaultCursor = null;
		protected override void OnMouseLeave(EventArgs e) {
			base.OnMouseLeave(e);
			if(defaultCursor != null) {
				Cursor = defaultCursor;
				defaultCursor = null;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			if(ShowSizeGrip) {
				if(PopupViewInfo.SizeGripRect.Contains(e.Location)) {
					if(defaultCursor == null)
						defaultCursor = Cursor.Current;
					Cursor = GetCursorBySizeGripPosition(PopupViewInfo.SizeGripInfo.GripPosition);
				}
				else if(defaultCursor != null){
					Cursor = defaultCursor;
					defaultCursor = null;
				}
			}
			if(DownPoint == InvalidPoint) return;
			Point size = GetSizeDelta(e);
			if(PopupControl.Size.Width + size.X < PopupControl.FormMinimumSize.Width) size.X = 0;
			if(PopupControl.Size.Height + size.Y < PopupControl.FormMinimumSize.Height) size.Y = 0;
			Point loc = GetLocationDelta(size);
			PopupControl.Size = new Size(Math.Max(0, PopupControl.Size.Width + size.X), Math.Max(0, PopupControl.Size.Height + size.Y));
			Size = new Size(Math.Max(0, Size.Width + size.X), Math.Max(PopupViewInfo.ButtonsRect.Height , Size.Height + size.Y));
			Form.Location = new Point(Form.Location.X + loc.X, Form.Location.Y + loc.Y);
			Form.Size = new Size(Form.Size.Width + size.X, Form.Size.Height + size.Y);
			UpdateDownPoint(e.Location);
		}
		protected virtual void UpdateDownPoint(Point point) {
			if(CurrentSizeGripPosition == SizeGripPosition.LeftBottom)
				DownPoint = new Point(DownPoint.X, point.Y);
			else if(CurrentSizeGripPosition == SizeGripPosition.RightBottom)
				DownPoint = point;
			else if(CurrentSizeGripPosition == SizeGripPosition.LeftTop)
				DownPoint = new Point(DownPoint.X, DownPoint.Y);
			else
				DownPoint = new Point(point.X, DownPoint.Y);
		}
		protected virtual Point GetLocationDelta(Point size) {
			if(PopupViewInfo.SizeGripInfo.GripPosition == SizeGripPosition.RightBottom) return Point.Empty;
			if(PopupViewInfo.SizeGripInfo.GripPosition == SizeGripPosition.LeftBottom) return new Point(-size.X, 0);
			if(PopupViewInfo.SizeGripInfo.GripPosition == SizeGripPosition.RightTop) return new Point(0, -size.Y);
			return new Point(-size.X, -size.Y);
		}
		protected virtual Point GetSizeDelta(MouseEventArgs e) {
			if(PopupViewInfo.SizeGripInfo.GripPosition == SizeGripPosition.RightBottom) {
				return new Point(e.Location.X - DownPoint.X, e.Location.Y - DownPoint.Y);
			}
			else if(PopupViewInfo.SizeGripInfo.GripPosition == SizeGripPosition.LeftBottom) {
				return new Point(DownPoint.X - e.Location.X, e.Location.Y - DownPoint.Y);
			}
			else if(PopupViewInfo.SizeGripInfo.GripPosition == SizeGripPosition.RightTop) {
				return new Point(e.Location.X - DownPoint.X, DownPoint.Y - e.Location.Y);
			}
			return new Point(DownPoint.X - e.Location.X, DownPoint.Y - e.Location.Y);
		}
	}
	[ToolboxItem(false)]
	public class RibbonToolbarSubMenuBarControl : SubMenuBarControl {
		internal RibbonToolbarSubMenuBarControl(BarManager barManager, BarCustomContainerItemLink containerLink) : base(barManager, containerLink) { }
		protected override void AddVisibleLinks(BarItemLinkReadOnlyCollection visibleLinks, IList sourceLinks) {
			RaiseCustomizeQatMenu(sourceLinks);
			base.AddVisibleLinks(visibleLinks, sourceLinks);
		}
		protected virtual void RaiseCustomizeQatMenu(IList list) {
			BarItemLinkCollection links = list as BarItemLinkCollection;
			if(links == null || OwnerLink == null || OwnerLink.Ribbon == null) return;
			OwnerLink.Ribbon.RaiseCustomizeQatMenu(new CustomizeQatMenuEventArgs(links));
		}
	}
	[ToolboxItem(false)]
	public class SubMenuBarControl : CustomPopupBarControl {
		BarRecentExpanderItem recentExpander;
		BarCustomContainerItemLink containerLink;
		internal SubMenuBarControl(BarManager barManager, BarCustomContainerItemLink containerLink) : base(barManager, null) {
			SetStyle(ControlConstants.DoubleBuffer, true);
			this.containerLink = containerLink;
			this.recentExpander = new BarRecentExpanderItem(this, Manager);
			if(ContainerLink != null) {
				ControlLinks.UpdateLinkedObject(ContainerLink.Item);
			}
		}
		protected override void RemoveAnimatedItems() {
			((SubMenuBarControlViewInfo)ViewInfo).RemoveAnimatedItems(((SubMenuBarControlViewInfo)ViewInfo).BarLinksViewInfo);
		}
		public override bool IsInterceptKey(KeyEventArgs e) {
			if(Manager.IsDesignMode && System.Windows.Forms.Form.ActiveForm is ModalTextBox)
				return false;
			return base.IsInterceptKey(e);
		}
		public override bool IsNeededKey(KeyEventArgs e) {
			if(Manager.IsDesignMode && System.Windows.Forms.Form.ActiveForm is ModalTextBox)
				return false;
			return base.IsNeededKey(e);
		}
		protected override bool IsAllowShowMenusOnRightClick {
			get {
				RibbonBarManager manager = Manager as RibbonBarManager;
				if(manager == null)
					return false;
				PopupMenu menu = GetRootPopupMenu();
				if(menu != null)
					return menu.AllowRibbonQATMenu;
				if(OwnerLink != null)
					return OwnerLink.AllowRibbonQATMenu;
				return base.IsAllowShowMenusOnRightClick;
			}
		}
		protected virtual PopupMenu GetRootPopupMenu() { 
			CustomControl bc = this;
			while(true) {
				if(bc is PopupMenuBarControl) {
					return ((PopupMenuBarControl)bc).Menu;
				}
				SubMenuBarControl sb = bc as SubMenuBarControl;
				if(sb == null || sb.OwnerLink == null)
					break;
				bc = sb.OwnerLink.BarControl;
			}
			return null;
		}
		public new CustomSubMenuBarControlViewInfo ViewInfo { get { return base.ViewInfo as CustomSubMenuBarControlViewInfo; } }
		protected override void UpdateFormHeight(int maxHeight) { 
			if(Form.Height <= maxHeight) return;
			Form.Height = maxHeight; 
			Form.locationInfo.WindowSize = new Size(Form.locationInfo.WindowSize.Width, Form.Height);
			Form.Location = Form.locationInfo.ShowPoint;
		} 
		protected internal override void MakeLinkVisible(BarItemLink link) {
			if(ViewInfo == null) return;
			ViewInfo.MakeLinkVisible(link);
		}
		protected override bool AllowAnimation { 
			get { return Manager != null && Manager.SelectionInfo.AllowAnimation; } 
		}
		public override void RaisePaintMenuBar(BarCustomDrawEventArgs e) {
			if(ContainerLink != null) ContainerLink.Item.RaisePaintMenuBar(e);
		}
		public override int MenuBarWidth { 
			get { 
				if(ContainerLink != null) return ContainerLink.Item.MenuBarWidth;
				return 0;
			}
		}
		public override bool CanCloseByTimer { get { return true; } }
		public override BarItemLink OwnerLink { get { return ContainerLink; } }
		public override Rectangle PopupOwnerRectangle { get { return ContainerLink.SourceRectangle; } }
		public virtual BarCustomContainerItemLink ContainerLink { get { return containerLink; } }
		internal BarRecentExpanderItem RecentExpander { get { return recentExpander; } }
		public override bool CanOpenAsChild(IPopup popup) { 
			SubMenuBarControl sub = popup as SubMenuBarControl;
			if(sub != null && VisibleLinks.Contains(sub.ContainerLink)) return true;
			if(VisibleLinks.Contains(popup.OwnerLink)) return true;
			return false; 
		} 
		protected internal override void UpdateVisibleLinks() {
			base.UpdateVisibleLinks();
			UpdateContainerLinkVisibleLinks();
			ControlLinks.UpdateOwner(this);
			BarItemLink designLink = ControlLinks[ControlLink.DesignTimeLink];
			if(designLink != null) VisibleLinks.AddItem(designLink);
		}
		protected virtual void UpdateContainerLinkVisibleLinks() {
			if(ContainerLink == null) return;
			Manager.Helper.RecentHelper.ClearInternalGroups(ContainerLink.VisibleLinks);
			ContainerLink.ReallyVisibleLinks = Manager.Helper.RecentHelper.BuildVisibleLinksList(ContainerLink.VisibleLinks);
			TotalVisibleLinks.ClearItems();
			AddVisibleLinks(TotalVisibleLinks, ContainerLink.VisibleLinks);
			if(Manager.CanShowNonRecentItems) {
				CopyLinks(VisibleLinks, TotalVisibleLinks, null);
			} else {
				CopyLinks(VisibleLinks, TotalVisibleLinks, ContainerLink.ReallyVisibleLinks);
				Manager.Helper.RecentHelper.CreateInternalGroups(ContainerLink.VisibleLinks, ContainerLink.ReallyVisibleLinks);
				UpdateRecentExpander(ContainerLink);
			}
			SetLinksOwner(this);
			SetLinksOwner(TotalVisibleLinks, this);
		}
		int GetVisibleLinkCount(IEnumerable links) {
			int res = 0;
			foreach(BarItemLink link in links) {
				if(link.CanVisible)
					res++;
			}
			return res;
		}
		int GetVisibleLinkCount(BarCustomContainerItemLink link) {
			return GetVisibleLinkCount(link.VisibleLinks);
		}
		int GetReallyVisibleLinksCount(BarCustomContainerItemLink link) {
			return GetVisibleLinkCount(link.ReallyVisibleLinks);
		}
		protected virtual void UpdateRecentExpander(BarCustomContainerItemLink link) {
			if(RecentExpander == null) return;
			if(GetVisibleLinkCount(link) != GetReallyVisibleLinksCount(link)) {
				if(VisibleLinks.Count > 0 && VisibleLinks[VisibleLinks.Count - 1].Item == RecentExpander) return;
				VisibleLinks.AddItem(RecentExpander.CreateLink(null, this));
			}
		}
	}
	internal class SubMenuInplaceAnimationInfoBase : BaseAnimationInfo { 
		static int AnimationTime = 300 * (int)TimeSpan.TicksPerMillisecond;
		static int DeltaMs = 10 * (int)TimeSpan.TicksPerMillisecond;
		public SubMenuInplaceAnimationInfoBase(SubMenuControlForm form)
			: base(form, form, DeltaMs, AnimationTime / DeltaMs) {
			Form = form;
			Helper = new SplineAnimationHelper();
			Helper.Init(0.0, 1.0, 1.0);	
		}
		protected SplineAnimationHelper Helper { get; set; }
		protected SubMenuControlForm Form { get; set; }
		protected bool AnimationEnded { get; set; }
		public override void FrameStep() {
			if(IsFinalFrame && !AnimationEnded) {
				AnimationEnded = true;
				OnAnimationEnd();
				return;
			}
			FrameStepCore();
		}
		protected virtual void OnAnimationEnd() {
		}
		protected virtual void FrameStepCore() {
		}
	}
	internal class SubMenuControlFormAnimationInfo : SubMenuInplaceAnimationInfoBase {
		public SubMenuControlFormAnimationInfo(SubMenuControlForm form) : base(form) {
			StartBounds = Form.Bounds;
			EndBounds = CalcEndBounds();
		}
		protected virtual Rectangle CalcEndBounds() {
			int delta = Form.ContainedControl.Bounds.Y;
			Size sz = new Size(Form.ContainedControl.Width + delta * 2, Form.ContainedControl.Height + delta * 2);
			return new Rectangle(Form.Location.X + (Form.Width - sz.Width) / 2, Form.Location.Y, sz.Width, sz.Height);
		}
		Rectangle EndBounds { get; set; }
		Rectangle StartBounds { get; set; }
		protected override void FrameStepCore() {
			float k = (float)CurrentFrame / FrameCount;
			k = (float)Helper.CalcSpline(k);
			float x = StartBounds.X + (EndBounds.X - StartBounds.X) * k;
			float y = StartBounds.Y + (EndBounds.Y - StartBounds.Y) * k;
			float width = StartBounds.Width + (EndBounds.Width - StartBounds.Width) * k;
			float height = StartBounds.Height + (EndBounds.Height - StartBounds.Height) * k;
			Form.BoundsAnimating = true;
			Form.Bounds = new Rectangle((int)x, (int)y, (int)width, (int)height);
			Form.ViewInfo.CalcViewInfo(null, Form, Form.ClientRectangle);
			Form.Invalidate();
			Form.BoundsAnimating = false;
		}
		protected override void OnAnimationEnd() {
			base.OnAnimationEnd();
			Form.Update();
		}
	}
	internal class SubMenuInplaceAnimationInfo : SubMenuInplaceAnimationInfoBase { 
		public SubMenuInplaceAnimationInfo(SubMenuControlForm form, CustomPopupBarControl subControl, bool direction) : base(form) {
			Direction = direction;
			SubControl = subControl;
			SubControl.Size = SubControl.CalcMinSize();
			SubControl.Form = Form;
			PrevStartRect = form.ContainedControl.Bounds;
			NewStartRect = CalcNewStartRect();
			PrevEndRect = CalcPrevEndRect();
			NewEndRect = CalcNewEndRect();
			Form.Controls.Add(SubControl);
		}
		protected virtual Rectangle CalcNewEndRect() {
			return new Rectangle(PrevStartRect.X, PrevStartRect.Y, SubControl.Width, SubControl.Height);
		}
		protected virtual Rectangle CalcNewStartRect() {
			if(Direction)
				return new Rectangle(new Point(PrevStartRect.X - SubControl.Width, PrevStartRect.Y), SubControl.Size);
			return new Rectangle(new Point(PrevStartRect.Right + SubControl.Width, PrevStartRect.Y), SubControl.Size);
		}
		protected virtual Rectangle CalcPrevEndRect() {
			if(Direction)
				return new Rectangle(new Point(PrevStartRect.X + SubControl.Width, PrevStartRect.Y), Form.ContainedControl.Size);
			return new Rectangle(new Point(PrevStartRect.X - Form.ContainedControl.Width, PrevStartRect.Y), Form.ContainedControl.Size);
		}
		protected CustomPopupBarControl SubControl { get; set; }
		protected bool Direction { get; set; }
		protected Rectangle PrevStartRect { get; set; }
		protected Rectangle NewStartRect { get; set; }
		protected Rectangle PrevEndRect { get; set; }
		protected Rectangle NewEndRect { get; set; }
		protected Rectangle PrevCurrentRect { get; set; }
		protected Rectangle NewCurrentRect { get; set; }
		protected override void FrameStepCore() {
			float k = (float)CurrentFrame / FrameCount;
			k = (float)Helper.CalcSpline(k);
			float locX = PrevStartRect.X + (PrevEndRect.X - PrevStartRect.X) * k;
			PrevCurrentRect = new Rectangle((int)locX, PrevStartRect.Y, PrevStartRect.Width, PrevStartRect.Height);
			if(Direction)
				locX -= NewCurrentRect.Width;
			else
				locX += PrevCurrentRect.Width;
			NewCurrentRect = new Rectangle((int)locX, NewStartRect.Y, NewStartRect.Width, NewStartRect.Height);
			Form.ContainedControl.Bounds = PrevCurrentRect;
			SubControl.Bounds = NewCurrentRect;
		}
		protected virtual void OnEndAnimationCore() {
			((CustomPopupBarControl)Form.ContainedControl).Form = null;
			Form.ContainedControl = SubControl;
			SubControl.Form = Form;
			if(!Direction)
				return;
			int index = SubControl.Manager.SelectionInfo.OpenedPopups.IndexOf(SubControl);
			if(index >= SubControl.Manager.SelectionInfo.OpenedPopups.Count)
				return;
			IPopup childPopup = SubControl.Manager.SelectionInfo.OpenedPopups[index + 1];
			SubControl.Manager.SelectionInfo.ClosePopup(childPopup);
		}
		protected override void OnAnimationEnd() {
			OnEndAnimationCore();
			XtraAnimator.Current.AddAnimation(new SubMenuControlFormAnimationInfo(Form));
		}
	}
}
