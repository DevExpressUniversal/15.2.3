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
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Drawing;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using System.Linq;
namespace DevExpress.XtraBars.InternalItems {
	public class BarQMenuAddRemoveButtonsItemLink : BarSubItemLink {
		internal BarQMenuAddRemoveButtonsItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
		}
		protected override Rectangle CalcLinkOwnerRect(Rectangle sourceRect) {
			return sourceRect;
		}
		protected internal override bool AlwaysStartNewLine { get { return true; } }
		public override bool IsLinkInMenu { get { return true; } }
		public override string DisplayCaption { get { return Manager.GetString(BarString.AddOrRemove);} }
	}
	public class BarQBarCustomizationItemLink : BarCustomContainerItemLink {
		BarItemLinkReadOnlyCollection quickLinks;
		BarSubItem addRemoveButtons;
		BarSubItemLink addRemoveButtonsLink;
		internal BarQBarCustomizationItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
			this.quickLinks = new BarItemLinkReadOnlyCollection();
			this.addRemoveButtons = null;
		}
		protected override bool OpenSubToLeft { get { return true; } }
		protected override LocationInfo CalcOnBarLocationInfo(BarLinkViewInfo li, Rectangle linkBounds) {
			Point p1 = new Point(linkBounds.X + linkBounds.Width, linkBounds.Y + linkBounds.Height), p2 = linkBounds.Location;
			p2.X = p1.X;
			return new LocationInfo(p1, p2, true, true);
		}
		void CreateInternalLinks() {
			if(this.addRemoveButtons != null) return;
			this.addRemoveButtons = new BarQMenuAddRemoveButtonsItem(Manager);
			this.addRemoveButtonsLink = addRemoveButtons.CreateLink(null, this) as BarSubItemLink;
			this.addRemoveButtonsLink.BeginGroup = true;
			this.addRemoveButtonsLink.ownerControl = this;
		}
		protected virtual IList CreateBarsList() {
			if(Bar == null || !Bar.OptionsBar.AllowQuickCustomization) return null;
			return new Bar[] { Bar};
		}
		protected virtual void UpdateInternalLinks() {
			IList bars = CreateBarsList();
			this.addRemoveButtons.Enabled = bars != null;
			this.addRemoveButtons.ItemLinks.Clear();
			if(bars == null) return;
			for(int n = 0; n < bars.Count; n++) {
				Bar bar = bars[n] as Bar;
				bar.CreateQuickCustomizationLinksCore(addRemoveButtons, n == bars.Count - 1);
			}
		}
		public virtual BarItemLinkReadOnlyCollection QuickLinks { get { return quickLinks; } }
		public override bool CanOpenMenu {
			get { return true; }
		}
		protected override bool AllowMenuTimers {
			get { return false; }
		}
		protected override CustomPopupBarControl CreateSubBarControl() {
			UpdateLinks();
			return new QuickCustomizationBarControl(Manager, this);
		}
		public override BarItemLinkReadOnlyCollection VisibleLinks {
			get { return QuickLinks; }
		}
		protected virtual void UpdateLinks() {
			QuickLinks.ClearItems();
			if(BarControl == null || Bar == null) return;
			BarControlViewInfo vi = BarControl.ViewInfo as BarControlViewInfo;
			if(vi == null) return;
			CreateInternalLinks();
			UpdateInternalLinks();
			QuickLinks.AddLinkRange(vi.NotVisibleLinks);
			if(!Bar.OptionsBar.AllowQuickCustomization  || !Manager.AllowQuickCustomization) return;
			QuickLinks.AddItem(addRemoveButtonsLink);
		}
	}
	public class BarRecentExpanderItemLink : BarItemLink, IBarLinkTimer {
		internal BarRecentExpanderItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
		}
		public virtual new BarRecentExpanderItem Item { get { return base.Item as BarRecentExpanderItem; } }
		void IBarLinkTimer.OnTimerRun() { }
		bool IBarLinkTimer.CanStartTimer { get { return true; } }
		bool IBarLinkTimer.CanStopTimer { get { return true; } }
		int IBarLinkTimer.TickInterval { get { return Manager.SubMenuOpenCloseInterval * 2; } }
		bool IBarLinkTimer.OnTimerTick(bool sameLink) {
			if(!sameLink || BarControl == null || Manager.SelectionInfo.ShowNonRecentItems) return false;
			OnLinkAction(BarLinkAction.MouseClick, null);
			return false;
		}
		protected override void OnLinkAction(BarLinkAction action, object actionArgs) {
			switch(action) {
				case BarLinkAction.KeyboardHighlight:
					ShowNonRecentItems(true);
					break;
				case BarLinkAction.MouseClick : ShowNonRecentItems(false); 
					break;
			}
			base.OnLinkAction(action, actionArgs);
		}
		public virtual void ShowNonRecentItems(bool byKeyboard) {
			Manager.SelectionInfo.ShowNonRecentItems = true;
			BarCustomContainerItemLink link = Item.FormBarControl.ContainerLink;
			link.UpdateVisibleLinks();
			if(byKeyboard && link.SubControl != null) {
				Manager.SelectionInfo.MoveLinkSubSelectionVertical(link.SubControl.VisibleLinks, BarLinkNavigation.First);
			}
		}
	}
	#region BarQMenuCustomizationItemLink
	public class BarQMenuCustomizationItemLink : BarButtonItemLink {
		BarQBarCustomizationItemLink qBarLink;
		internal BarQMenuCustomizationItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
			qBarLink = null;
		}
		public virtual BarItemLink InnerLink { get { return Item.Tag as BarItemLink; } }
		public virtual BarQBarCustomizationItemLink QBarLink  { get { return qBarLink; } set { qBarLink = value; } }
	}
	#endregion
	public class BarScrollItemLink : BarItemLink, IBarLinkTimer {
		public int LinkHeight = 0;
		bool scrollDown;
		internal BarScrollItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
		}
		void IBarLinkTimer.OnTimerRun() { }
		bool IBarLinkTimer.CanStartTimer { get { return true; } }
		bool IBarLinkTimer.CanStopTimer { get { return false; } }
		int IBarLinkTimer.TickInterval { get { return 100; } } 
		bool IBarLinkTimer.OnTimerTick(bool sameLink) {
			if(!sameLink || BarControl == null) return false;
			OnLinkAction(BarLinkAction.MouseClick, null);
			return true;
		}
		public virtual bool ScrollDown { get { return scrollDown; } set { scrollDown = value; } }
		protected internal override bool AlwaysWorking {
			get { return true; }
		}
		protected override void OnLinkAction(BarLinkAction action, object actionArgs) {
			switch(action) {
				case BarLinkAction.KeyboardHighlight:
					Scroll(true);
					break;
				case BarLinkAction.MouseClick : 
					Scroll(false); 
					break;
			}
			base.OnLinkAction(action, actionArgs);
		}
		protected int GetDownDelta() {
			CustomPopupBarControl ctrl = BarControl as CustomPopupBarControl;
			CustomSubMenuBarControlViewInfo vi = (CustomSubMenuBarControlViewInfo)ctrl.ViewInfo;
			BarItemLink link = ctrl.VisibleLinks[ctrl.TopLinkIndex];
			SubMenuLinkInfo subInfo = vi.GetLinkViewInfo(link, vi.BarLinksViewInfo);
			if(subInfo == null || !subInfo.MultiColumn)
				return 1;
			for(int i = vi.BarLinksViewInfo.IndexOf(subInfo), count = 0; i < vi.BarLinksViewInfo.Count; i++, count++) {
				if(!vi.BarLinksViewInfo[i].MultiColumn || vi.BarLinksViewInfo[i].Row == subInfo.Row + 1)
					return count;
			}
			return 1;
		}
		protected int GetUpDelta() {
			CustomPopupBarControl ctrl = BarControl as CustomPopupBarControl;
			CustomSubMenuBarControlViewInfo vi = (CustomSubMenuBarControlViewInfo)ctrl.ViewInfo;
			BarItemLink link = ctrl.VisibleLinks[ctrl.TopLinkIndex];
			SubMenuLinkInfo subInfo = vi.GetLinkViewInfo(link, vi.BarLinksViewInfo);
			if(!subInfo.MultiColumn)
				return -1;
			int columnCount = vi.GetGalleryColumnCount(link);
			int linkIndex = ctrl.VisibleLinks.IndexOf(link) - 1;
			if(linkIndex <= 0 || !ctrl.GetLinkMultiColumn(ctrl.VisibleLinks[linkIndex]))
				return -1;
			return -columnCount;
		}
		public virtual void Scroll(bool byKeyboard) {
			CustomPopupBarControl ctrl = BarControl as CustomPopupBarControl;
			if(ctrl == null) return;
			ctrl.TopLinkIndex += (ScrollDown ? GetDownDelta() : GetUpDelta());
			ctrl.ForceMouseEnter();
			BarItemLinkCollection ownerItems = GetOwnerItemLinks(ctrl);
			if(ownerItems != null) {
				foreach(BarItemLink link in ownerItems) {
					BarSubItemLink sLink = link as BarSubItemLink;
					if(sLink != null)
						sLink.CloseMenu();
				}
			}
			if(Manager.SelectionInfo.HighlightTimerLink == null)
				Manager.SelectionInfo.InitHighligthTimer(this);
		}
		protected virtual BarItemLinkCollection GetOwnerItemLinks(CustomPopupBarControl ctrl) {
			if(ctrl.OwnerLink != null) {
				BarSubItem item = ctrl.OwnerLink.Item as BarSubItem;
				if(item != null)
					return item.ItemLinks;
			}
			PopupMenu menu = ctrl.PopupCreator as PopupMenu;
			if(menu != null)
				return menu.ItemLinks;
			return null;
		}
	}
	public class BarDesignTimeItemLink : BarButtonItemLink {
		internal BarDesignTimeItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
		}
		internal override bool AllowDrawLinkDisabledInCustomizationMode {
			get {
				return true;
			}
		}
		protected internal override bool AlwaysWorking { get { return true; } }
		public override int GetLinkHorzIndent() { return 0; }
		public override int GetLinkVertIndent() { return 0; }
		public override bool CanDrag { get { return false; } }
	}
	public class BarCloseItemLink : BarButtonItemLink {
		internal BarCloseItemLink(BarItemLinkReadOnlyCollection ALinks, BarItem AItem, object ALinkedObject) : base(ALinks, AItem, ALinkedObject) {
		}
		public override BarItemLinkAlignment Alignment { get { return BarItemLinkAlignment.Right; } }
		protected override void OnLinkClick() {
			base.OnLinkClick();
			if(Manager != null) Manager.OnCloseButtonClick();
		}
	}
	public class RibbonExpandCollapseItemLink : BarButtonItemLink {
		public RibbonExpandCollapseItemLink(BarItemLinkReadOnlyCollection links, BarItem item, object linkedObject) : base(links, item, linkedObject) { }
		protected override void OnLinkClick() {
			Ribbon.Minimized = !Ribbon.Minimized;
			base.OnLinkClick();
			Item.OnItemChanged(true);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsButtonGroup {
			get { return base.ActAsButtonGroup; }
			set { base.ActAsButtonGroup = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string KeyTip {
			get { return base.KeyTip; }
			set { base.KeyTip = value; }
		}
	}
	public class AutoHiddenPagesMenuItemLink : BarButtonItemLink {
		public AutoHiddenPagesMenuItemLink(BarItemLinkReadOnlyCollection links, BarItem item, object linkedObject) : base(links, item, linkedObject) {
			menu = new PopupMenu(Ribbon.Manager);
		}
		PopupMenu menu;
		protected override PopupControl DropDownControl { get { return menu; } }
		protected override void OnLinkClick() {
			menu.ClearLinks();
			foreach (BarButtonItem item in Ribbon.ViewInfo.Header.GetHidedPages()){
				menu.AddItem(item); 
			}
			menu.ShowPopup(Manager, Ribbon.PointToScreen(new Point(Bounds.X, Bounds.Bottom)));
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ActAsButtonGroup {
			get { return base.ActAsButtonGroup; }
			set { base.ActAsButtonGroup = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string KeyTip {
			get { return base.KeyTip; }
			set { base.KeyTip = value; }
		}
	}
}
