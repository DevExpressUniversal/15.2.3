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
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraNavBar.ViewInfo;
namespace DevExpress.XtraNavBar {
	public enum SkinExplorerBarViewScrollStyle { Default, Buttons, ScrollBar }
	public enum NavBarViewKind { Default, ExplorerBar, SideBar, NavigationPane };
	public enum NavBarHitTest { None, GroupCaption, GroupClient, GroupCaptionButton, UpButton, DownButton, Link,
		LinkCaption, LinkImage, NavigationPaneSplitter, NavigationPaneHeader, NavigationPaneOverflowPanel,
		NavigationPaneOverflowPanelButton, GroupBottom, GroupBottomButton, ExpandButton, ContentButton
	}
	public enum NavBarState { 
		Normal, GroupPressed, LinkPressed, UpButtonPressed, DownButtonPressed, LinkDragging,
		NavigationPaneOveflowButton, NavigationPaneSizing, ExpandButtonPressed, ContentButtonPressed
	}
	[Flags]
	public enum NavBarDragDrop {
		None = 0,
		Default = 1,
		AllowDrag = 2,
		AllowDrop = 4,
		AllowOuterDrop = 8
	}
	public class NavBarHitInfo : ICloneable {
		NavBarControl navBar;
		NavBarHitTest hitTest;
		Point hitPoint;
		NavBarGroup hitGroup;
		NavBarItemLink hitLink;
		Rectangle expandButtonBounds = Rectangle.Empty;
		public NavBarHitInfo(NavBarControl navBar) {
			if(navBar == null) throw new ArgumentException("navBar can't be null");
			this.navBar = navBar;
			Clear();
		}
		protected virtual NavBarHitInfo CreateHitInfo() {
			return new NavBarHitInfo(NavBar);
		}
		protected virtual void Assign(NavBarHitInfo info) {
			this.hitGroup = info.hitGroup;
			this.hitLink = info.hitLink;
			this.hitPoint = info.hitPoint;
			this.hitTest = info.hitTest;
		}
		public virtual object Clone() {
			NavBarHitInfo info = CreateHitInfo();
			info.Assign(this);
			return info;
		}
		protected NavBarHitTest HitTestCore { get { return hitTest; } set { hitTest = value; } }
		protected Point HitPointCore { get { return hitPoint; } set { hitPoint = value; } }
		public Rectangle ExpandButtonBounds { get { return expandButtonBounds; } set { expandButtonBounds = value; } }
		public NavBarControl NavBar { get { return navBar; } }
		public NavBarHitTest HitTest { get { return hitTest; } }
		public Point HitPoint { get { return hitPoint; } set { hitPoint = value; } }
		public NavBarGroup Group { get { return hitGroup; } }
		public NavBarItemLink Link { get { return hitLink; } }
		protected void SetLink(NavBarItemLink link) { this.hitLink = link; }
		protected bool ContainsPoint(Rectangle bounds, Point p) {
			return !bounds.IsEmpty && bounds.Contains(p);
		}
		protected bool CheckAndSetHitTest(Rectangle bounds, Point p, NavBarHitTest hitTest) {
			if(ContainsPoint(bounds, p)) {
				this.hitTest = hitTest;
				return true;
			}
			return false;
		}
		internal void SetGroup(NavBarGroup group) {
			this.hitTest = NavBarHitTest.GroupCaption;
			this.hitGroup = group;
		}
		public virtual void Clear() {
			this.hitGroup = null;
			this.hitLink = null;
			this.hitTest = NavBarHitTest.None;
			this.hitPoint = Point.Empty;
		}
		public virtual bool InExpandButton { get { return HitTest == NavBarHitTest.ExpandButton; } }
		public virtual bool InLink {
			get { return HitTest == NavBarHitTest.Link || HitTest == NavBarHitTest.LinkCaption || HitTest == NavBarHitTest.LinkImage; }
		}
		public virtual bool InGroupButton { get { return HitTest == NavBarHitTest.GroupBottomButton || HitTest == NavBarHitTest.GroupCaptionButton; } }
		public virtual bool InGroup {
			get { return InGroupCaption || HitTest == NavBarHitTest.GroupClient || InLink || HitTest == NavBarHitTest.GroupBottom || HitTest == NavBarHitTest.GroupBottomButton; }
		}
		public virtual bool InGroupCaption {
			get { return HitTest == NavBarHitTest.GroupCaption || HitTest == NavBarHitTest.GroupCaptionButton; }
		}
		protected NavBarViewInfo ViewInfo { get { return NavBar.ViewInfo; } }
		public virtual void CalcHitInfo(Point p, NavBarHitTest[] validLinkHotTracks) {
			Clear();
			this.hitPoint = p;
			this.hitTest = NavBarHitTest.None;
			if(!ViewInfo.Bounds.Contains(p)) return;
			foreach(NavGroupInfoArgs e in ViewInfo.Groups) {
				if(CalcGroupHitInfo(e, p, validLinkHotTracks)) return;
			}
		}
		public virtual bool IsEquals(object obj) {
			NavBarHitInfo hi = obj as NavBarHitInfo;
			if(hi == null) return false;
			if(hi.Group != Group || hi.Link != Link) return false;
			if(hi.HitTest == HitTest) return true;
			return false;
		}
		protected virtual bool CalcGroupHitInfo(NavGroupInfoArgs e, Point p, NavBarHitTest[] validLinkHotTracks) {
			if(CheckAndSetHitTest(ViewInfo.UpButtonBounds, p, NavBarHitTest.UpButton)) {
				hitGroup = NavBar.ActiveGroup;
				return true;
			}
			if(CheckAndSetHitTest(ViewInfo.DownButtonBounds, p, NavBarHitTest.DownButton)) {
				hitGroup = NavBar.ActiveGroup;
				return true;
			}
			if(CheckAndSetHitTest(e.Bounds, p, NavBarHitTest.GroupCaption)) {
				hitGroup = e.Group;
				CheckAndSetHitTest(e.ButtonBounds, p, NavBarHitTest.GroupCaptionButton);
				return true;
			}
			if(CheckAndSetHitTest(e.FooterBounds, p, NavBarHitTest.GroupBottom)) {
				hitGroup = e.Group;
				CheckAndSetHitTest(e.ButtonBounds, p, NavBarHitTest.GroupBottomButton);
				return true;
			}
			if(e.ClientInfo.Bounds.IsEmpty || !e.ClientInfo.Bounds.Contains(p) || (NavBar.OptionsNavPane.NavPaneState == NavPaneState.Collapsed && !e.ClientInfo.InForm)) return false;
			hitGroup = e.Group;
			hitTest = NavBarHitTest.GroupClient;
			CalcGroupLinksHitInfo(e, p);
			if(Link != null) {
				if(validLinkHotTracks != null && Array.IndexOf(validLinkHotTracks, HitTest) == -1) {
					hitTest = NavBarHitTest.GroupClient;
					hitLink = null;
				}
			}
			return true;
		}
		protected virtual void CalcGroupLinksHitInfo(NavGroupInfoArgs e, Point p) {
			foreach(NavLinkInfoArgs li in e.Links) {
				if(!CheckAndSetHitTest(li.Bounds, p, NavBarHitTest.Link)) continue;
				hitLink = li.Link;
				if(CheckAndSetHitTest(li.RealCaptionRectangle, p, NavBarHitTest.LinkCaption)) return;
				CheckAndSetHitTest(li.ImageRectangle, p, NavBarHitTest.LinkImage);
				if(li.HitRectangle.Contains(p)) {
					if(p.Y > li.ImageRectangle.Bottom) 
						hitTest = NavBarHitTest.LinkCaption;
					else
						hitTest = NavBarHitTest.LinkImage;
					return;
				}
				return;
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class NavBarAppearances : BaseAppearanceCollection {
		 AppearanceObject navigationPaneHeader, groupBackground, groupHeader, groupHeaderPressed, 
			groupHeaderHotTracked, groupHeaderActive, item, itemPressed, itemHotTracked, itemActive, 
			itemDisabled, background, button, buttonPressed, buttonHotTracked, buttonDisabled, hint,
			linkDropTarget, navPaneCollapsedCaption, navPaneCollapsedCaptionHotTracked, navPaneCollapsedCaptionPressed, navPaneCollapsedCaptionExpanded;
		protected override void CreateAppearances() {
			this.navigationPaneHeader = CreateAppearance("NavigationPaneHeader");
			this.groupBackground = CreateAppearance("GroupBackground");
			this.groupHeader = CreateAppearance("GroupHeader");
			this.groupHeaderPressed = CreateAppearance("GroupHeaderPressed");
			this.groupHeaderHotTracked = CreateAppearance("GroupHeaderHotTracked");
			this.groupHeaderActive = CreateAppearance("GroupHeaderActive");
			this.item = CreateAppearance("Item");
			this.itemPressed = CreateAppearance("ItemPressed");
			this.itemHotTracked = CreateAppearance("ItemHotTracked");
			this.itemDisabled = CreateAppearance("ItemDisabled");
			this.itemActive = CreateAppearance("ItemActive");
			this.background = CreateAppearance("Background");
			this.button = CreateAppearance("Button");
			this.buttonPressed = CreateAppearance("ButtonPressed", this.button);
			this.buttonHotTracked = CreateAppearance("ButtonHotTracked", this.button);
			this.buttonDisabled = CreateAppearance("ButtonDisabled", this.button);
			this.hint = CreateAppearance("Hint");
			this.linkDropTarget = CreateAppearance("LinkDropTarget");
			this.navPaneCollapsedCaption = CreateAppearance("NavPaneCollapsedCaption");
			this.navPaneCollapsedCaptionHotTracked = CreateAppearance("NavPaneCollapsedCaptionHotTracked");
			this.navPaneCollapsedCaptionPressed = CreateAppearance("NavPaneCollapsedCaptionPressed");
			this.navPaneCollapsedCaptionExpanded = CreateAppearance("NavPaneCollapsedCaptionExpanded");
		}
		bool ShouldSerializeNavigationPaneHeader() { return NavigationPaneHeader.ShouldSerialize(); }
		void ResetNavigationPaneHeader() { NavigationPaneHeader.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject NavigationPaneHeader { get { return navigationPaneHeader; } }
		bool ShouldSerializeGroupBackground() { return GroupBackground.ShouldSerialize(); }
		void ResetGroupBackground() { GroupBackground.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupBackground { get { return groupBackground; } }
		bool ShouldSerializeGroupHeader() { return GroupHeader.ShouldSerialize(); }
		void ResetGroupHeader() { GroupHeader.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupHeader { get { return groupHeader; } }
		bool ShouldSerializeGroupHeaderPressed() { return GroupHeaderPressed.ShouldSerialize(); }
		void ResetGroupHeaderPressed() { GroupHeaderPressed.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupHeaderPressed { get { return groupHeaderPressed; } }
		bool ShouldSerializeGroupHeaderHotTracked() { return GroupHeaderHotTracked.ShouldSerialize(); }
		void ResetGroupHeaderHotTracked() { GroupHeaderHotTracked.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupHeaderHotTracked { get { return groupHeaderHotTracked; } }
		bool ShouldSerializeGroupHeaderActive() { return GroupHeaderActive.ShouldSerialize(); }
		void ResetGroupHeaderActive() { GroupHeaderActive.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupHeaderActive { get { return groupHeaderActive; } }
		bool ShouldSerializeItem() { return Item.ShouldSerialize(); }
		void ResetItem() { Item.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Item { get { return item; } }
		bool ShouldSerializeItemPressed() { return ItemPressed.ShouldSerialize(); }
		void ResetItemPressed() { ItemPressed.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemPressed { get { return itemPressed; } }
		bool ShouldSerializeItemHotTracked() { return ItemHotTracked.ShouldSerialize(); }
		void ResetItemHotTracked() { ItemHotTracked.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemHotTracked { get { return itemHotTracked; } }
		bool ShouldSerializeItemActive() { return ItemActive.ShouldSerialize(); }
		void ResetItemActive() { ItemActive.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemActive { get { return itemActive; } }
		bool ShouldSerializeItemDisabled() { return ItemDisabled.ShouldSerialize(); }
		void ResetItemDisabled() { ItemDisabled.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemDisabled { get { return itemDisabled; } }
		bool ShouldSerializeBackground() { return Background.ShouldSerialize(); }
		void ResetBackground() { Background.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Background { get { return background; } }
		bool ShouldSerializeButton() { return Button.ShouldSerialize(); }
		void ResetButton() { Button.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Button { get { return button; } }
		bool ShouldSerializeButtonPressed() { return ButtonPressed.ShouldSerialize(); }
		void ResetButtonPressed() { ButtonPressed.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ButtonPressed { get { return buttonPressed; } }
		bool ShouldSerializeButtonHotTracked() { return ButtonHotTracked.ShouldSerialize(); }
		void ResetButtonHotTracked() { ButtonHotTracked.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ButtonHotTracked { get { return buttonHotTracked; } }
		bool ShouldSerializeButtonDisabled() { return ButtonDisabled.ShouldSerialize(); }
		void ResetButtonDisabled() { ButtonDisabled.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ButtonDisabled { get { return buttonDisabled; } }
		bool ShouldSerializeHint() { return Hint.ShouldSerialize(); }
		void ResetHint() { Hint.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Hint { get { return hint; } }
		bool ShouldSerializeLinkDropTarget() { return LinkDropTarget.ShouldSerialize(); }
		void ResetLinkDropTarget() { LinkDropTarget.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject LinkDropTarget { get { return linkDropTarget; } }
		bool ShouldSerializeNavPaneContentButton() { return NavPaneContentButton.ShouldSerialize(); }
		void ResetNavPaneContentButton() { NavPaneContentButton.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject NavPaneContentButton { get { return navPaneCollapsedCaption; } }
		bool ShouldSerializeNavPaneContentButtonHotTracked() { return NavPaneContentButtonHotTracked.ShouldSerialize(); }
		void ResetNavPaneContentButtonHotTracked() { NavPaneContentButtonHotTracked.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject NavPaneContentButtonHotTracked { get { return navPaneCollapsedCaptionHotTracked; } }
		bool ShouldSerializeNavPaneContentButtonPressed() { return NavPaneContentButtonPressed.ShouldSerialize(); }
		void ResetNavPaneContentButtonPressed() { NavPaneContentButtonPressed.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject NavPaneContentButtonPressed { get { return navPaneCollapsedCaptionPressed; } }
		bool ShouldSerializeNavPaneContentButtonReleased() { return NavPaneContentButtonReleased.ShouldSerialize(); }
		void ResetNavPaneContentButtonReleased() { NavPaneContentButtonReleased.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject NavPaneContentButtonReleased { get { return navPaneCollapsedCaptionExpanded; } }
	}
	static class NavBarLocalizationHelper {
		public static string GetString(NavBarStringId id) {
			return NavBarLocalizer.Active.GetLocalizedString(id);
		}
	}
}
