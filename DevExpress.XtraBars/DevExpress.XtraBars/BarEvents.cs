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
using System.Drawing.Design;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars;
using DevExpress.Utils.Serializing;
using System.ComponentModel.Design;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraBars {
	public class LinkEventArgs : EventArgs {
		BarItemLink link;
		public LinkEventArgs(BarItemLink link) {
			this.link = link;
		}
		public BarItemLink Link { get { return link; } }
	}
	public class ItemCancelEventArgs : ItemClickEventArgs {
		bool cancel;
		public ItemCancelEventArgs(BarItem item, BarItemLink link, bool cancel) : base(item, link) {
			this.cancel = cancel;
		}
		public bool Cancel { 
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public class ShortcutItemClickEventArgs : EventArgs {
		BarItem item;
		BarShortcut shortcut;
		bool cancel = false;
		public ShortcutItemClickEventArgs(BarItem item, BarShortcut shortcut) {
			this.item = item;
			this.shortcut = shortcut;
		}
		public BarItem Item { get { return item; } }
		public BarShortcut Shortcut { get { return shortcut; } }
		public bool Cancel {
			get { return cancel; }
			set { cancel = value; }
		}
	}
	public class ListItemClickEventArgs : EventArgs {
		BarItem item;
		int index;
		public ListItemClickEventArgs(BarItem item, int index) {
			this.item = item;
			this.index = index;
		}
		public BarItem Item { get { return item; } }
		public int Index { get { return index; } }
	}
	public class ItemClickEventArgs : EventArgs {
		BarItem item;
		BarItemLink link;
		public ItemClickEventArgs(BarItem item, BarItemLink link) {
			this.item = item;
			this.link = link;
		}
		public BarItem Item { get { return item; } }
		public BarItemLink Link { get { return link; } }
	}
	public class CreateToolbarEventArgs : EventArgs {
		Bar bar;
		public CreateToolbarEventArgs(Bar bar) {
			this.bar = bar;
		}
		public Bar Bar { get { return bar; } }
	}
	public class QueryShowPopupMenuEventArgs : CancelEventArgs {
		PopupMenuBase menu;
		Control control;
		Point position;
		public QueryShowPopupMenuEventArgs(PopupMenuBase menu, Control control, Point position) {
			this.Cancel = false;
			this.control = control;
			this.position = position;
			this.menu = menu;
		}
		public Point Position { 
			get { return position; }
			set { position = value; 
			}
		}
		public Control Control { get { return control; } }
		public PopupMenuBase Menu { get { return menu; } }
	}
	public class BarCustomDrawEventArgs : EventArgs {
		Graphics graphics;
		Rectangle bounds;
		bool handled;
		public BarCustomDrawEventArgs(Graphics graphics, Rectangle bounds) {
			this.graphics = graphics;
			this.bounds = bounds;
			this.handled = false;
		}
		public virtual Graphics Graphics { get { return graphics; } }
		public virtual Rectangle Bounds { get { return bounds; } }
		public virtual bool Handled { get { return handled; } set { handled = value; } }
	}
	public class CreateCustomizationFormEventArgs : EventArgs {
		DevExpress.XtraBars.Customization.CustomizationForm customizationForm;
		public CreateCustomizationFormEventArgs(DevExpress.XtraBars.Customization.CustomizationForm customizationForm) {
			this.customizationForm = customizationForm;
		}
		public DevExpress.XtraBars.Customization.CustomizationForm CustomizationForm {
			get { return customizationForm;}
			set { customizationForm = value; }
		}
	}
	public class HighlightedLinkChangedEventArgs : EventArgs {
		BarItemLink prevLink, link;
		public HighlightedLinkChangedEventArgs(BarItemLink prevLink, BarItemLink link) {
			this.prevLink = prevLink;
			this.link = link;
		}
		public BarItemLink PrevLink { 
			get { return prevLink; }
		}
		public BarItemLink Link { 
			get { return link; }
		}
	}
	public class BarManagerMergeEventArgs : EventArgs {
		BarManager childManager;
		public BarManagerMergeEventArgs(BarManager childManager) {
			this.childManager = childManager;
		}
		public BarManager ChildManager { get { return childManager; } }
	}
	public class ShowToolbarsContextMenuEventArgs : EventArgs {
		BarItemLinkCollection itemLinks;
		public ShowToolbarsContextMenuEventArgs(BarItemLinkCollection itemLinks) {
			this.itemLinks = itemLinks;
		}
		public BarItemLinkCollection ItemLinks { get { return itemLinks; } }
	}
	public class BarItemCustomDrawEventArgs : EventArgs {
		public BarItemCustomDrawEventArgs(GraphicsCache cache) {
			Cache = cache;
		}
		public BarItemCustomDrawEventArgs(GraphicsCache cache, RibbonItemViewInfo itemInfo)
			: this(cache) {
			RibbonItemInfo = itemInfo;
		}
		public BarItemCustomDrawEventArgs(GraphicsCache cache, BarLinkViewInfo linkInfo)
			: this(cache) {
			LinkInfo = linkInfo;
		}
		public bool DrawOnlyText { get; internal set; }
		public RibbonItemViewInfo RibbonItemInfo { get; set; }
		RibbonSplitButtonItemViewInfo RibbonSplitItemInfo { get { return RibbonItemInfo as RibbonSplitButtonItemViewInfo; } }
		RibbonEditItemViewInfo RibbonEditItemInfo { get { return RibbonItemInfo as RibbonEditItemViewInfo; } }
		RibbonCheckItemViewInfo RibbonCheckItemInfo { get { return RibbonItemInfo as RibbonCheckItemViewInfo; } }
		BarCheckButtonLinkViewInfo CheckLinkInfo { get { return LinkInfo as BarCheckButtonLinkViewInfo; } }
		public BarLinkViewInfo LinkInfo { get; set; }
		public bool Handled { get; set; }
		public GraphicsCache Cache { get; private set; }
		public Graphics Graphics { get { return Cache.Graphics; } }
		public Rectangle DropDownBounds {
			get {
				if(LinkInfo != null)
					return LinkInfo.Rects[BarLinkParts.DropButton];
				if(RibbonSplitItemInfo != null)
					return RibbonSplitItemInfo.DropButtonBounds;
				return Rectangle.Empty;
			}
		}
		public Rectangle Bounds {
			get {
				return RibbonItemInfo != null ? RibbonItemInfo.Bounds : LinkInfo.Bounds;
			}
		}
		public Rectangle TextBounds {
			get {
				return RibbonItemInfo != null ? RibbonItemInfo.CaptionBounds : LinkInfo.CaptionRect;
			}
		}
		public Rectangle GlyphBounds {
			get {
				return RibbonItemInfo != null ? RibbonItemInfo.GlyphBounds : LinkInfo.GlyphRect;
			}
		}
		public string Text {
			get {
				return RibbonItemInfo != null ? RibbonItemInfo.Text : LinkInfo.DisplayCaption;
			}
		}
		public Image Glyph {
			get {
				return RibbonItemInfo != null ? RibbonItemInfo.ViewInfo.ItemCalculator.GetGlyphInfo(RibbonItemInfo, RibbonItemInfo.CalcState()).GetGlyph() : LinkInfo.GetLinkImage(LinkInfo.LinkState);
			}
		}
		protected BarLinkState CalcRibbonItemState() {
			ObjectState state = RibbonItemInfo.CalcState();
			if((state & ObjectState.Disabled) != 0)
				return BarLinkState.Disabled;
			else if((state & ObjectState.Pressed) != 0) {
				if(RibbonItemInfo.ViewInfo.PressedObject.HitTest == RibbonHitTest.ItemDrop)
					return BarLinkState.DropDownPressed;
				return BarLinkState.Pressed;
			}
			else if((state & ObjectState.Hot) != 0) {
				if(RibbonItemInfo.ViewInfo.HotObject.HitTest == RibbonHitTest.ItemDrop)
					return BarLinkState.DropDownHighlighted;
				return BarLinkState.Highlighted;
			}
			else if((state & ObjectState.Selected) != 0)
				return BarLinkState.Checked;
			return BarLinkState.Normal;
		}
		public BarLinkState State {
			get {
				if(RibbonItemInfo != null) {
					return CalcRibbonItemState();
				}
				return CalcLinkInfoState();
			}
		}
		private BarLinkState CalcLinkInfoState() {
			return LinkInfo.LinkState;
		}
		public void DrawText() {
			if(RibbonItemInfo != null) {
				RibbonItemInfo.GetInfo().DrawText(Cache, RibbonItemInfo);
			}
			else {
				LinkInfo.Painter.DrawLinkTextOnly(new BarLinkPaintArgs(Cache, LinkInfo, LinkInfo.BarControlInfo));
			}
		}
		public void DrawText(AppearanceObject appearance) {
			AppearanceObject obj = null;
			if(RibbonItemInfo != null) {
				obj = RibbonItemInfo.Appearance;
				try {
					RibbonItemInfo.SetPaintAppearance(appearance);
					RibbonItemInfo.GetInfo().DrawText(Cache, RibbonItemInfo);
					RibbonItemInfo.SetPaintAppearance(obj);
				}
				finally {
				}
			}
			else {
				try {
					LinkInfo.SetForcedOwnerAppearance(appearance);
					LinkInfo.Painter.DrawLinkTextOnly(new BarLinkPaintArgs(Cache, LinkInfo, LinkInfo.BarControlInfo));
				}
				finally {
					LinkInfo.SetForcedOwnerAppearance(null);
				}
			}
		}
		public void DrawGlyph() {
			if(RibbonItemInfo != null) {
				DevExpress.XtraBars.Ribbon.ViewInfo.RibbonItemViewInfoCalculator.GlyphInfo info = null;
				if(RibbonItemInfo.IsLargeButton)
					info = RibbonItemInfo.ViewInfo.ItemCalculator.GetLargeGlyphInfo(RibbonItemInfo, RibbonItemInfo.CalcState());
				else
					info = RibbonItemInfo.ViewInfo.ItemCalculator.GetGlyphInfo(RibbonItemInfo, RibbonItemInfo.CalcState());
				info.DrawGlyph(Cache, GlyphBounds, RibbonItemInfo.CalcState() != ObjectState.Disabled);
			}
			else {
				LinkInfo.Painter.DrawLinkGlyph(new BarLinkPaintArgs(Cache, LinkInfo, LinkInfo.BarControlInfo), LinkInfo.LinkState);
			}
		}
		public void DrawBackground() {
			if(RibbonItemInfo != null) {
				if(RibbonEditItemInfo != null)
					return;
				RibbonItemInfo.ViewInfo.ItemCalculator.DrawItemBackground(Cache, RibbonItemInfo);
			}
			else {
				if(!LinkInfo.AllowDrawBackground || LinkInfo.LinkState == BarLinkState.Normal || LinkInfo.LinkState == BarLinkState.Disabled)
					return;
				BarLinkPaintArgs e = new BarLinkPaintArgs(Cache, LinkInfo, LinkInfo.BarControlInfo);
				if(LinkInfo.IsLinkInMenu)
					LinkInfo.Painter.DrawLinkHighlightedBackgroundInMenu(e, LinkInfo.LinkState);
				else
					LinkInfo.Painter.DrawLinkHighlightedBackground(e, LinkInfo.LinkState);
			}
		}
		public void DrawArrow() {
			if(!ShouldDrawDropDown)
				return;
			if(RibbonItemInfo != null) {
				RibbonSplitItemInfo.ItemCalculator.DrawArrow(Cache, RibbonSplitItemInfo.DropButtonBounds, RibbonSplitItemInfo.CalcDropState());
			}
			else {
				BarLinkPaintArgs e = new BarLinkPaintArgs(Cache, LinkInfo, LinkInfo.BarControlInfo);
				if(LinkInfo.IsLinkInMenu)
					LinkInfo.Painter.DrawLinkArrowInMenu(e, LinkInfo.LinkState);
				else 
					LinkInfo.Painter.DrawLinkArrow(e, LinkInfo.LinkState);
			}
		}
		public bool ShouldDrawDropDown {
			get {
				if(RibbonItemInfo != null)
					return RibbonSplitItemInfo != null;
				return !LinkInfo.Rects[BarLinkParts.OpenArrow].IsEmpty;
			}
		}
		public bool ShouldDrawEditor {
			get {
				if(RibbonItemInfo != null)
					return RibbonEditItemInfo != null;
				return false;
			}
		}
		public bool ShouldDrawCheckBox {
			get {
				if(RibbonItemInfo != null)
					return RibbonCheckItemInfo != null && RibbonCheckItemInfo.GetCheckBoxVisibility() != CheckBoxVisibility.None;
				return CheckLinkInfo != null && CheckLinkInfo.GetCheckBoxVisibility() != CheckBoxVisibility.None;
			}
		}
		public void DrawCheckBox() {
			if(RibbonCheckItemInfo != null)
				RibbonCheckItemInfo.ViewInfo.ItemCalculator.DrawCheckBox(Cache, RibbonCheckItemInfo);
		}
		public void DrawDropDownBackground() {
			if(RibbonItemInfo != null) {
				if(!(RibbonItemInfo is RibbonSplitButtonItemViewInfo))
					return;
				RibbonItemInfo.ViewInfo.ItemCalculator.DrawItemDropDownBackground(Cache, RibbonItemInfo);
				RibbonSplitItemInfo.ViewInfo.ItemCalculator.DrawArrow(Cache, RibbonSplitItemInfo.DropButtonBounds, RibbonSplitItemInfo.CalcDropState());
			}
		}
		public void DrawEditor() {
			if(RibbonItemInfo != null) {
				RibbonItemInfo.ViewInfo.ItemCalculator.DrawEditor(Cache, RibbonEditItemInfo);
			}
		}
		public void Draw() {
			if(RibbonItemInfo != null) {
				RibbonItemInfo.ViewInfo.ItemCalculator.IsInCustomDrawEvent = true;
				try {
					RibbonItemInfo.GetInfo().Draw(Cache, RibbonItemInfo);
				}
				finally {
					RibbonItemInfo.ViewInfo.ItemCalculator.IsInCustomDrawEvent = false;
				}
			}
			else {
				LinkInfo.Painter.Draw(new GraphicsInfoArgs(Cache, LinkInfo.Bounds), LinkInfo, LinkInfo.BarControlInfo);
			}
		}
	}
	public delegate void ShowToolbarsContextMenuEventHandler(object sender, ShowToolbarsContextMenuEventArgs e);
	public delegate void ShortcutItemClickEventHandler(object sender, ShortcutItemClickEventArgs e);
	public delegate void BarManagerMergeEventHandler(object sender, BarManagerMergeEventArgs e);
	public delegate void HighlightedLinkChangedEventHandler(object sender, HighlightedLinkChangedEventArgs e);
	public delegate void CreateToolbarEventHandler(object sender, CreateToolbarEventArgs e);
	public delegate void QueryShowPopupMenuEventHandler(object sender, QueryShowPopupMenuEventArgs e);
	public delegate void CreateCustomizationFormEventHandler(object sender, CreateCustomizationFormEventArgs e);
	public delegate void ItemClickEventHandler(object sender, ItemClickEventArgs e);
	public delegate void ListItemClickEventHandler(object sender, ListItemClickEventArgs e);
	public delegate void ItemCancelEventHandler(object sender, ItemCancelEventArgs e);
	public delegate void LinkEventHandler(object sender, LinkEventArgs e);
	public delegate void BarCustomDrawEventHandler(object sender, BarCustomDrawEventArgs e);
	public delegate void BarItemCustomDrawEventHandler(object sender, BarItemCustomDrawEventArgs e);
}
