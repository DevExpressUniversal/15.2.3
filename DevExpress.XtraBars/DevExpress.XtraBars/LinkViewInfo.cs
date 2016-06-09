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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Painters;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.Forms;
using DevExpress.XtraBars.InternalItems;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Win;
using System.Collections.Generic;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Styles;
using DevExpress.Utils.Text;
using DevExpress.Skins;
using DevExpress.XtraBars.Utils;
using DevExpress.Utils.Menu;
using System.Reflection;
namespace DevExpress.XtraBars.ViewInfo {
	public enum BarIndent { Left, Right, Top, Bottom, DragBorder, SizeGrip };
	public enum BarLinkDrawMode { Horizontal, Vertical, InMenu, InMenuLarge, InMenuLargeWithText, InRadialMenu, InMenuGallery, Default };
	[Flags]
	public enum BarLinkParts { Bounds = 0, Caption = 1, Glyph = 2, Shortcut = 4, OpenArrow = 8, Border = 16, Select = 32, PushButton = 64, DropButton = 128, Description = 256, Checkbox = 512 };
	[Flags]
	public enum BarLinkState { Normal = 0, Highlighted = 1, Pressed = 2, Selected = 4, Checked = 8, Disabled = 16, DropDownHighlighted = 32, DropDownPressed = 64 };
	public class BarLinkRectangles {
		class BarLinkPartsComparer : IEqualityComparer<BarLinkParts> {
			public bool Equals(BarLinkParts x, BarLinkParts y) {
				return x == y;
			}
			public int GetHashCode(BarLinkParts obj) {
				return (int)obj;
			}
		}
		Dictionary<BarLinkParts, Rectangle> rectangles;
		ArrayList extRectangles;
		Rectangle realBounds;
		public BarLinkRectangles() { }
		public virtual Rectangle RealBounds { get { return realBounds; } set { realBounds = value; } }
		public Dictionary<BarLinkParts, Rectangle> Rectangles {
			get {
				if(rectangles == null) rectangles = new Dictionary<BarLinkParts, Rectangle>(new BarLinkPartsComparer());
				return rectangles;
			}
		}
		public virtual ArrayList ExtRectangles { 
			get {
				if(extRectangles == null) extRectangles = new ArrayList();
				return extRectangles; 
			} 
		}
		public virtual void AddExtRectangle(object r) {
			if(ExtRectangles == null) extRectangles = new ArrayList();
			ExtRectangles.Add(r);
		}
		public virtual bool IsExtRectanglesEmpty { get { return this.extRectangles == null || ExtRectangles.Count == 0; } }
		public virtual void Clear() {
			this.rectangles = null;
		}
		internal void ReverseRightToLeft(Rectangle rect) {
			if(IsExtRectanglesEmpty) return;
			for(int n = 0; n < ExtRectangles.Count; n++) {
				RectInfo rInfo = (RectInfo)ExtRectangles[n];
				Rectangle r = rInfo.Rect;
				Rectangle newBounds = new Rectangle(rect.Right - (r.Width + r.X), r.Y, r.Width, r.Height);
				rInfo.Rect = newBounds;
			}
		}
		public void OffsetExtRectangles(int x, int y) {
			if(IsExtRectanglesEmpty) return;
			for(int n = 0; n < ExtRectangles.Count; n++) {
				RectInfo rInfo = (RectInfo)ExtRectangles[n];
				Rectangle r = rInfo.Rect;
				r.Offset(x, y);
				rInfo.Rect = r;
			}
		}
		public virtual void Offset(BarLinkParts part, int x, int y) {
			Rectangle r = this[part];
			r.Offset(x, y);
			this[part] = r;
		}
		public virtual void SetWidthHeight(BarLinkParts part, int width, int height) {
			Rectangle r = this[part];
			r.Width = width;
			r.Height = height;
			this[part] = r;
		}
		public virtual void SetWidth(BarLinkParts part, int width) {
			SetWidthHeight(part, width, this[part].Height);
		}
		public virtual void AddWidth(BarLinkParts part, int delta) { AddWidthHeight(part, delta, 0); }
		public virtual void AddHeight(BarLinkParts part, int delta) { AddWidthHeight(part, 0, delta); }
		public virtual void AddWidthHeight(BarLinkParts part, int deltaX, int deltaY) {
			Rectangle r = this[part];
			r.Width += deltaX;
			r.Height += deltaY;
			this[part] = r;
		}
		public Rectangle this[BarLinkParts part] {
			get {
				Rectangle res;
				if(this.rectangles != null && this.rectangles.TryGetValue(part, out res)) return res;
				return Rectangle.Empty;
			}
			set {
				if(part == BarLinkParts.Select && value.X == -2) {
				}
				Rectangles[part] = value;
			}
		}
	}
	public class RadialMenuLinkMetrics {
		string[] wrappedText;
		Size[] wrappedTextSize;
		public RadialMenuLinkMetrics() {
			this.wrappedText = null;
			this.wrappedTextSize = null;
		}
		public string[] WrappedText {
			get { return wrappedText; }
			set { wrappedText = value; }
		}
		public Size[] WrappedTextSize {
			get { return wrappedTextSize; }
			set { wrappedTextSize = value; }
		}
		protected internal Size CalcLinkSize() {
			Size size = Size.Empty;
			for(int i = 0; i < WrappedTextSize.Length; i++) {
				if(size.Height > 0) size.Height += BarItemLinkTextHelper.LineIndent;
				if(WrappedTextSize[i].Width > 0) size.Height += WrappedTextSize[i].Height;
				size.Width = Math.Max(size.Width, WrappedTextSize[i].Width);
			}
			return size;
		}
	}
	public class BarLinkViewInfo : CustomViewInfo {
		BarLinkState linkState;
		protected BarLinkParts fDrawParts;
		BarLinkDrawMode drawMode;
		BarItemLink link;
		Brush barBackBrush;
		ObjectPainter borderPainter;
		Size fixedGlyphSize = Size.Empty;
		string caption = null;
		string shortCutDisplayText = null;
		BarLinkRectangles rects;
		bool isInnerLink = false;
		protected StringFormat fLinkCaptionStringFormat;
		bool isRecentLink, alwaysDrawGlyph, alwaysDrawAsInMenu, drawVerticalRotated, allowDrawBackground, allowDrawDisabled;
		BarLinkPainter painter;
		AppearanceObject descriptionAppearance;
		public event BarLinkGetValueEventHandler GlyphSizeEvent;
		Items2Panel itemsPanel;
		StringInfo htmlStringCaptionInfo;
		StringInfo htmlStringDescriptionInfo;
		RadialMenuLinkMetrics radialMenuLinkMetrics;
		public BarLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(link.Manager, parameters) {
			this.borderPainter = null;
			this.barBackBrush = null;
			this.alwaysDrawAsInMenu = false;
			this.allowDrawBackground = true;
			this.fLinkCaptionStringFormat = null;
			this.link = link;
			this.alwaysDrawGlyph = false;
			this.rects = new BarLinkRectangles();
			this.allowDrawDisabled = true;
			this.itemsPanel = new Items2Panel();
			Painter = Link.Manager.Helper.GetLinkPainter(Link.GetType());
			this.borderPainter = GetBorderPainter();
			this.htmlStringCaptionInfo = null;
			this.htmlStringDescriptionInfo = null;
			this.radialMenuLinkMetrics = null;
			Clear();
		}
		protected internal bool IsRightToLeft { 
			get {
				if(BarControlInfo != null)
					return BarControlInfo.IsRightToLeft;
				if(Manager != null)
					return Manager.IsRightToLeft;
				return false;
			} 
		}
		protected internal bool DrawNormalGlyphInCheckedState { get; set; }
		protected internal RadialMenuViewInfo RadialMenuWindow { get; set; }
		public Point MousePosition { get { return Link.ScreenToLinkPoint(Control.MousePosition); } }
		public virtual bool CanAnimate { get { return Link != null && Link.Enabled; } }
		protected internal bool IsInnerLink { get { return isInnerLink; } set { isInnerLink = value; } }
		protected internal Size FixedGlyphSize { get { return fixedGlyphSize; } set { fixedGlyphSize = value; } }
		public ObjectPainter BorderPainter { get { return borderPainter; } }
		public virtual bool IsDrawAnyGlyph { get { return Link.IsImageExist; }	}
		public virtual bool AllowDrawLighter { get { return Manager.GetController().PropertiesBar.AllowLinkLighting && Manager.PaintStyle.AllowLinkLighting; } }
		public virtual bool AlwaysDrawAsInMenu { get { return alwaysDrawAsInMenu; } set { alwaysDrawAsInMenu= value; } }
		public virtual bool AllowDrawBackground {
			get { return allowDrawBackground; } 
			set { allowDrawBackground = value; } 
		}
		public virtual bool AllowDrawDisabled { get { return allowDrawDisabled; } set { allowDrawDisabled = value; } }
		protected internal Items2Panel ItemsPanel { get { return itemsPanel; } }
		protected internal StringInfo HtmlStringCaptionInfo { get { return htmlStringCaptionInfo; } }
		protected internal StringInfo HtmlStringDescriptionInfo { get { return htmlStringDescriptionInfo; } }
		public virtual TextOptions HtmlTextOptions {
			get {
				TextOptions options = new TextOptions(this.OwnerAppearance);
				if(options.HotkeyPrefix == HKeyPrefix.Default) options.HotkeyPrefix = HKeyPrefix.Show;
				return options;
			}
		}
		public virtual bool DrawDisabled { 
			get {
				if(!AllowDrawDisabled)
					return false;
				if(Link.BarControl != null && !Link.BarControl.Enabled)
					return true;
				return (!Link.Enabled && (!Link.Manager.IsCustomizing || Link.Manager.Helper.CustomizationManager.IsHotCustomizing || Link.AllowDrawLinkDisabledInCustomizationMode)); 
			} 
		}
		public virtual bool ShouldDrawCheckedCaption(BarLinkViewInfo linkInfo, BarLinkState state) {
			if(state != BarLinkState.Checked || linkInfo.Bar == null || !linkInfo.Bar.IsStatusBar)
				return false;
			return  DrawParameters.CanDrawCheckedCaption;
		}
		public virtual string DisplayCaption {
			get { return caption == null ? Link.DisplayCaption : caption; }
			set { caption = value; }
		}
		public virtual bool IsItemShortcutExist {
			get { return !string.IsNullOrEmpty(ShortCutDisplayText) || Link.ItemShortcut.IsExist; }
		}
		public virtual string ShortCutDisplayText {
			get { return shortCutDisplayText == null ? Link.ShortCutDisplayText : shortCutDisplayText; }
			set { shortCutDisplayText = value; }
		}
		public virtual bool AlwaysDrawGlyph { get { return alwaysDrawGlyph; } set { alwaysDrawGlyph = value; } }
		public virtual bool IsDrawPart(BarLinkParts part) { 
			return (DrawParts & part) == part; 
		}
		public virtual BarItemPaintStyle GetPaintStyle() {
			return Link.Item.CalcRealPaintStyle(Link); 
		}
		BarLinkDrawMode forceDrawMode = BarLinkDrawMode.Default;
		public BarLinkDrawMode ForceDrawMode { get { return forceDrawMode; } set { forceDrawMode = value; } }
		public virtual BarLinkRectangles Rects { get { return rects; } }
		public virtual BarLinkDrawMode DrawMode { get { return drawMode; } }
		public virtual BarLinkParts DrawParts { get { return fDrawParts; } }
		public virtual BarItemCaptionAlignment GetCaptionAlignment() {
			if(!IsRightToLeft) return CaptionAlignment;
			if(IsLinkInMenu) return CaptionAlignment;
			if(CaptionAlignment == BarItemCaptionAlignment.Right) return BarItemCaptionAlignment.Left;
			if(CaptionAlignment == BarItemCaptionAlignment.Left) return BarItemCaptionAlignment.Right;
			return CaptionAlignment;
		}
		public virtual BarItemCaptionAlignment CaptionAlignment { get { return BarItemCaptionAlignment.Right; } }
		public virtual BarLinkState LinkState { 
			get { return linkState; } 
			set { linkState = value; }
		}
		public virtual StringFormat LinkCaptionStringFormat {
			get { 
				if(fLinkCaptionStringFormat != null) return fLinkCaptionStringFormat;
				if(IsDrawVerticalRotated) return DrawParameters.SingleLineVerticalStringFormat;
				if(Link.MaxLinkTextWidth != 0) {
					if(Link.LinkedObject is BarListItem) return DrawParameters.SingleLineEllipsisPathFormat;
					return DrawParameters.SingleLineEllipsisFormat;
				}
				return DrawParameters.SingleLineStringFormat; 
			}
		}
		public virtual Image GetLinkImage(BarLinkState state) {
			if(IsShouldUseLargeImages && Link.IsLargeImageExist) return GetLinkLargeImageCore(state);
			return GetLinkImageCore(state);
		}
		public virtual bool HasImage(BarLinkState state) {
			Image image = null;
			int index = -1;
			if(IsShouldUseLargeImages && Link.IsLargeImageExist) {
				CalcLinkLargeImage(state, ref index, ref image);
				return image != null || ImageCollection.GetImageListImage(Link.Item.Manager.LargeImages, index) != null;
			}
			else {
				CalcLinkImage(state, ref index, ref image);
				return image != null || ImageCollection.GetImageListImage(Link.Item.Manager.Images, index) != null;
			}
		}
		protected virtual Image GetLinkImageCore(BarLinkState state) {
			Image image = null;
			int index = -1;
			CalcLinkImage(state, ref index, ref image);
			if(image == null)
				image = ImageCollection.GetImageListImage(Link.Item.Manager.Images, index);
			if(image == null && state != BarLinkState.Normal) image = GetLinkImage(BarLinkState.Normal);
			return image;
		}
		protected virtual Image GetLinkLargeImageCore(BarLinkState state) {
			Image image = null;
			int index = -1;
			CalcLinkLargeImage(state, ref index, ref image);
			if(image == null)
				image = ImageCollection.GetImageListImage(Link.Item.Manager.LargeImages, index);
			if(image == null && state != BarLinkState.Normal) image = GetLinkLargeImageCore(BarLinkState.Normal);
			if(image == null) image = GetLinkImageCore(BarLinkState.Normal);
			return image;
		}
		protected virtual void CalcLinkLargeImage(BarLinkState state, ref int index, ref Image image) {
			switch(state) {
				case BarLinkState.Disabled: 
					image = Link.Item.LargeGlyphDisabled;
					index = Link.Item.LargeImageIndexDisabled;
					break;
				default:
					image = Link.GetLargeGlyph();
					index = Link.Item.LargeImageIndex;
					break;
			}
		}
		protected virtual void CalcLinkImage(BarLinkState state, ref int index, ref Image image) {
			switch(state) {
				case BarLinkState.Disabled: 
					image = Link.Item.GlyphDisabled; 
					index = Link.Item.ImageIndexDisabled;
					break;
				default:
					image = Link.GetGlyph();
					index = Link.ImageIndex;
					break;
			}
		}
		public BarItemLink Link { get { return link; } }
		public Rectangle Bounds { 
			get { return Rects[BarLinkParts.Bounds]; } 
			set {
				Rects[BarLinkParts.Bounds] = value;
			}
		}
		public Rectangle GlyphRect { 
			get { return Rects[BarLinkParts.Glyph]; } 
			set { Rects[BarLinkParts.Glyph] = value; }
		}
		public Rectangle SelectRect { 
			get { return Rects[BarLinkParts.Select]; } 
			set { Rects[BarLinkParts.Select] = value; }
		}
		public Rectangle CaptionRect { 
			get { return Rects[BarLinkParts.Caption]; } 
			set { Rects[BarLinkParts.Caption] = value; }
		}
		public virtual bool CanDrawAs(BarLinkState state) {
			switch(state) {
				case BarLinkState.Highlighted : return (Link.Enabled || Link.Manager.SelectionInfo.KeyboardHighlightedLink == Link) && !Link.Manager.IsCustomizing;
				case BarLinkState.Pressed : return Link.Enabled;
			}
			return true;
		}
		protected virtual ISpringLink SpringLink { get { return Link as ISpringLink; } }
		protected virtual bool IsSpringLink { 
			get {
				if(!IsLinkInMenu && SpringLink != null && SpringLink.SpringAllow && BarControl != null && BarControl.IsCanSpringLinks) return true;
				return false;
			}
		}
		protected virtual bool IsDefaultCaptionAlignment { 
			get { 
				if(IsLinkInMenu || !IsDrawPart(BarLinkParts.Glyph) || !IsDrawPart(BarLinkParts.Caption)) return true;
				return false;
			}
		}
		protected virtual bool IsHCaptionAlignment { 
			get {
				return CaptionAlignment == BarItemCaptionAlignment.Left || CaptionAlignment == BarItemCaptionAlignment.Right; } 
		}
		protected virtual bool IsVCaptionAlignment { get { return !IsDefaultCaptionAlignment && !IsHCaptionAlignment; } }
		protected internal virtual int CalcLinkIndent(BarIndent linkIndent) {
			return Manager.PaintStyle.CalcLinkIndent(this, linkIndent);
		}
		public virtual BarLinkState CalcRealPaintState() {
			return LinkState;
		}
		protected virtual void UpdatePainters() {
			this.borderPainter = GetBorderPainter();
			UpdateOwnerAppearance();
		}
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle r) {
			Clear();
			UpdatePainters();
			this.SourceObject = sourceObject;
			UpdateLinkInfo(sourceObject);
			Rects[BarLinkParts.Bounds] = r;
			g = GInfo.AddGraphics(g);
			try {
				switch(DrawMode) {
					case BarLinkDrawMode.InRadialMenu: CalcInRadialMenuViewInfo(g, sourceObject, r); break;
					case BarLinkDrawMode.Horizontal : CalcHorizontalViewInfo(g, sourceObject, r);break;
					case BarLinkDrawMode.Vertical : CalcVerticalViewInfo(g, sourceObject, r);break;
					case BarLinkDrawMode.InMenu: CalcInMenuViewInfo(g, sourceObject, r); break;
					case BarLinkDrawMode.InMenuGallery: CalcInGalleryMenuViewInfo(g, sourceObject, r); break;
					case BarLinkDrawMode.InMenuLargeWithText: 
					case BarLinkDrawMode.InMenuLarge: CalcInMenuViewInfoLarge(g, sourceObject, r); break;
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			ready = true;
		}
		protected virtual void CalcHtmlStringInfo(Graphics g) {
			this.htmlStringCaptionInfo = CalcCaptionHtmlStringInfo(g);
			this.htmlStringDescriptionInfo = CalcDescriptionHtmlStringInfo(g);
		}
		protected virtual StringInfo CalcCaptionHtmlStringInfo(Graphics g) {
			return StringPainter.Default.Calculate(g, OwnerAppearance, HtmlTextOptions, Link.Caption, Rects[BarLinkParts.Caption]);
		}
		protected virtual StringInfo CalcDescriptionHtmlStringInfo(Graphics g) {
			return StringPainter.Default.Calculate(g, AppearanceMenuItemDescription, HtmlTextOptions, Link.Description, Rects[BarLinkParts.Description]);
		}
		protected internal double Angle { get; set; }
		protected internal double AngleWidth { get; set; }
		protected virtual void CalcInRadialMenuViewInfo(Graphics g, object sourceObject, Rectangle r) {
			Rectangle c1Bounds = Rectangle.Empty, c2Bounds = Rectangle.Empty;
			CaptionRect = new Rectangle(Point.Empty, CalcRadialMenuLinkCaptionSize(g));
			Size glyphSize = GetLinkImage(LinkState) != null ? GlyphSize : Size.Empty;
			ItemsPanel.ArrangeItems(r, glyphSize, CaptionRect.Size, ref c1Bounds, ref c2Bounds);
			CaptionRect = c1Bounds.Size.IsEmpty ? RectangleHelper.GetCenterBounds(c2Bounds, CaptionRect.Size) : c2Bounds;
			Rects[BarLinkParts.Glyph] = c1Bounds;
		}
		protected void CalcInMenuViewInfoLarge(Graphics g, object sourceObject, Rectangle r) {
			CalcInMenuViewInfoLargeCore(g, sourceObject, r);
			CalcInMenuArrowViewInfo(g);
		}
		protected virtual void CalcInMenuViewInfoLargeCore(Graphics g, object sourceObject, Rectangle r) {
			int leftIndent = CalcLinkIndent(BarIndent.Left),
				rightIndent = CalcLinkIndent(BarIndent.Right);
			int vertIndent = CalcLinkIndent(BarIndent.Top);
			CalcInMenuBorder(g, sourceObject, ref r);
			Rects[BarLinkParts.Select] = r;
			int maxTextWidth = r.Width - (leftIndent + rightIndent);
			Rectangle glyph = Rectangle.Empty;
			glyph.Size = GlyphSize;
			if(Link.Manager is RibbonBarManager) glyph.Location = new Point(r.X + DrawParameters.Constants.SubMenuGlyphHorzIndent - 1, r.Y + vertIndent); 
			else glyph.Location = new Point(r.X + DrawParameters.Constants.SubMenuGlyphHorzIndent - 1, r.Y + (r.Height - glyph.Size.Height) / 2);
			GlyphRect = glyph;
			Rectangle text = Rectangle.Empty;
			Size captionSize = CalcCaptionSize(g);
			text.X = GlyphRect.Right + DrawParameters.Constants.SubMenuGlyphCaptionIndent + leftIndent;
			text.Y = r.Y + vertIndent;
			text.Height = r.Height - vertIndent * 2;
			text.Width = captionSize.Width + 1;
			if(IsDrawPart(BarLinkParts.Description)) {
				text.Height = captionSize.Height;
				Rectangle description = text;
				description.Width = r.Right - rightIndent - description.X;
				description.Y = text.Bottom + 1;
				description.Height = CalcDescriptionHeight(g);
				Rects[BarLinkParts.Description] = description;
			}
			Rects[BarLinkParts.Caption] = text;
			if(Link.Item.ShortcutKeyDisplayString != string.Empty)
				ShortCutDisplayText = Link.Item.ShortcutKeyDisplayString;
			if(IsItemShortcutExist) {
				text.X = text.Right;
				text.Width = PaintHelper.CalcTextSize(g, ShortCutDisplayText, Font, 0,
					DrawParameters.ShortCutStringFormat, Link).ToSize().Width + 2;
				Rects[BarLinkParts.Shortcut] = text;
			}
			if(Link.IsAllowHtmlText) CalcHtmlStringInfo(g);
		}
		protected virtual void CalcInMenuBorder(Graphics g, object sourceObject, ref Rectangle r) {
			CalcHorizontalBorder(g, sourceObject, ref r);
		}
		protected virtual void CalcVerticalBorder(Graphics g, object sourceObject, ref Rectangle r) {
			CalcHorizontalBorder(g, sourceObject, ref r);
		}
		public virtual ObjectPainter GetBorderPainter() {
			BorderStyles border = GetBorder();
			if(border == BorderStyles.Default) return GetDefaultBorderPainter();
			if(border == BorderStyles.Simple) return new LinkOffice2000BorderPainter();
			if(border == BorderStyles.Office2003) return new LinkOffice2003BorderPainter();
			return BorderHelper.GetPainter(border);
		}
		protected virtual ObjectPainter GetDefaultBorderPainter() {
			return Manager.PPainter.DefaultLinkBorderPainter;
		}
		protected virtual BorderStyles GetBorder() {
			if(IsLinkInMenu) return BorderStyles.NoBorder;
			BorderStyles border = Link.Item.Border;
			return border;
		}
		protected virtual void CalcHorizontalBorder(Graphics g, object sourceObject, ref Rectangle r) {
			ObjectPainter painter = GetBorderPainter();
			Rects[BarLinkParts.Border] = new Rectangle(r.X, Rects.RealBounds.Y, r.Width, Rects.RealBounds.Height); 
			r = ObjectPainter.GetObjectClientRectangle(g, painter, new StyleObjectInfoArgs(null, r, null, ObjectState.Normal));
		}
		protected virtual Rectangle CalcBorderSize() {
			Rectangle res = Rectangle.Empty;
			GInfo.AddGraphics(null);
			try {
				res = ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, GetBorderPainter(), new BorderObjectInfoArgs(null, Rectangle.Empty, null), Rectangle.Empty);
				res.X = Math.Abs(res.X);
				res.Y = Math.Abs(res.Y);
				res.Width -= res.X; res.Height -= res.Y;
			} 
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		protected Rectangle GetContentBounds(Rectangle r) {
			int leftIndent = CalcLinkIndent(BarIndent.Left),
				rightIndent = CalcLinkIndent(BarIndent.Right);
			int vertIndent = CalcLinkIndent(BarIndent.Top);
			return new Rectangle(r.X + leftIndent, r.Y + vertIndent, r.Width - leftIndent - rightIndent, r.Height - 2 * vertIndent);
		}
		protected virtual void CalcAlignedCaptionGlyphInfo(Graphics g, object sourceObject, Rectangle r) {
			Rectangle content = GetContentBounds(r);
			Rectangle tr, gl, temp;
			tr = gl = temp = Rectangle.Empty;
			Size captionSize = CalcCaptionSize(g);
			int maxTextWidth = ActualContentHorizontalAlignment == BarItemContentAlignment.Stretch ? content.Width : captionSize.Width;
			gl.Size = GlyphSize;
			if(IsHCaptionAlignment) {
				gl.Location = new Point(content.X, content.Y + (content.Height - gl.Size.Height) / 2);
				if(ActualContentHorizontalAlignment == BarItemContentAlignment.Stretch)
					maxTextWidth -= (gl.Width + DrawParameters.Constants.BarCaptionGlyphIndent);
				tr.Size = new Size(Math.Min(maxTextWidth, captionSize.Width), captionSize.Height);
				tr.X = gl.Right + DrawParameters.Constants.BarCaptionGlyphIndent;
				tr.Y = content.Y + (content.Height - captionSize.Height) / 2;
				if(GetCaptionAlignment() == BarItemCaptionAlignment.Left) {
					temp = tr;
					temp.X = gl.X;
					gl.X = temp.Right + DrawParameters.Constants.BarCaptionGlyphIndent;
					tr = temp;
				}
			} else {
				gl.Location = new Point(content.X + (content.Width - gl.Size.Width) / 2, content.Y);
				captionSize.Width ++;
				captionSize.Width = Math.Min(content.Width, captionSize.Width);
				tr.Size = captionSize;
				tr.Location = new Point(content.X + (content.Width - tr.Width) / 2, gl.Bottom + DrawParameters.Constants.BarCaptionGlyphIndent);
				if(GetCaptionAlignment() == BarItemCaptionAlignment.Top) {
					temp = tr;
					temp.Y = gl.Y;
					gl.Y += tr.Bottom - tr.Y;
					tr = temp;
				}
			}
			Rects[BarLinkParts.Glyph] = gl;
			Rects[BarLinkParts.Caption] = tr;
		}
		protected virtual int CalcMaxTextWidth(Graphics g, Rectangle bounds) {
			int leftIndent = CalcLinkIndent(BarIndent.Left),
				rightIndent = CalcLinkIndent(BarIndent.Right);
			return ActualContentHorizontalAlignment == BarItemContentAlignment.Stretch ? bounds.Width - (leftIndent + rightIndent) : CalcCaptionSize(g).Width;
		}
		protected virtual void CalcHorizontalCaptionGlyphInfo(Graphics g, object sourceObject, Rectangle r) {
			if(!IsDefaultCaptionAlignment) {
				r = UpdateBoundsByArrow(g, sourceObject, r);
				CalcAlignedCaptionGlyphInfo(g, sourceObject, r);
				ApplyContentAlignment(GetContentBounds(r));
				return;
			}
			Size captionSize = CalcCaptionSize(g);
			Rectangle content = GetContentBounds(r);
			int maxTextWidth = ActualContentHorizontalAlignment == BarItemContentAlignment.Stretch? content.Width : captionSize.Width;
			int captionX = content.X;
			if(IsDrawPart(BarLinkParts.Glyph)) {
				Rectangle gl = Rectangle.Empty;
				gl.Size = GlyphSize;
				gl.Location = new Point(content.X, content.Y + (content.Height - gl.Size.Height) / 2);
				Rects[BarLinkParts.Glyph] = gl;
				maxTextWidth -= (gl.Width + DrawParameters.Constants.BarCaptionGlyphIndent);
				captionX = gl.Right + DrawParameters.Constants.BarCaptionGlyphIndent;
			} 
			if(IsDrawPart(BarLinkParts.Caption)) {
				Rectangle tr = Rectangle.Empty;
				tr.Size = new Size(maxTextWidth, content.Height);
				tr.X = captionX;
				tr.Y = content.Y;
				Rects[BarLinkParts.Caption] = tr;
			}
			ApplyContentAlignment(content);
		}
		protected virtual BarItemContentAlignment ActualContentHorizontalAlignment { 
			get {
				BarItemContentAlignment align = Link.Item.ContentHorizontalAlignment;
				ISpringLink link = Link as ISpringLink;
				if(link != null && link.SpringAllow)
					return BarItemContentAlignment.Stretch;
				if(align != BarItemContentAlignment.Default)
					return align;
				if(!IsDrawPart(BarLinkParts.Glyph))
					return BarItemContentAlignment.Stretch;
				if(!IsDrawPart(BarLinkParts.Caption))
					return BarItemContentAlignment.Center;
				if(IsDrawPart(BarLinkParts.Caption) && IsDrawPart(BarLinkParts.Glyph))
					return BarItemContentAlignment.Stretch;
				return align;
			} 
		}
		protected virtual void ApplyContentAlignment(Rectangle content) {
			if(ActualContentHorizontalAlignment == BarItemContentAlignment.Center)
				AlignCenterCaptionGlyph(content);
			if(ActualContentHorizontalAlignment == BarItemContentAlignment.Far)
				AlignFarCaptionGlyph(content);
		}
		protected int CalcContentWidth() {
			if(IsDrawPart(BarLinkParts.Glyph) && IsDrawPart(BarLinkParts.Caption))
				return Math.Max(GlyphRect.Right, CaptionRect.Right) - Math.Min(GlyphRect.X, CaptionRect.X);
			int minX = IsDrawPart(BarLinkParts.Glyph) ? GlyphRect.X : CaptionRect.X;
			int maxX = IsDrawPart(BarLinkParts.Caption) ? CaptionRect.Right : GlyphRect.Right;
			return maxX - minX;
		}
		protected Rectangle OffsetRect(Rectangle rect, int delta) {
			return new Rectangle(rect.X + delta, rect.Y, rect.Width, rect.Height);
		}
		protected virtual void AlignFarCaptionGlyph(Rectangle content) {
			int totalWidth = CalcContentWidth();
			int delta = content.Width - totalWidth;
			if(IsDrawPart(BarLinkParts.Glyph))
				Rects[BarLinkParts.Glyph] = OffsetRect(GlyphRect, delta);
			if(IsDrawPart(BarLinkParts.Caption))
				Rects[BarLinkParts.Caption] = OffsetRect(CaptionRect, delta);
		}
		protected virtual void AlignCenterCaptionGlyph(Rectangle content) {
			int totalWidth = CalcContentWidth();
			if(content.Width <= totalWidth)
				return;
			int delta = (content.Width - totalWidth) / 2;
			if(IsDrawPart(BarLinkParts.Glyph))
				Rects[BarLinkParts.Glyph] = OffsetRect(GlyphRect, delta);
			if(IsDrawPart(BarLinkParts.Caption))
				Rects[BarLinkParts.Caption] = OffsetRect(CaptionRect, delta);
		}
		protected virtual bool ActAsDropDown { get { return false; } }
		protected virtual bool AllowDrawArrow { get { return false; } }
		protected virtual Rectangle UpdateBoundsByArrow(Graphics g, object sourceObject, Rectangle r) {
			if((DrawParts & BarLinkParts.OpenArrow) == 0 || IsLinkInMenu)
				return r;
			if(ActAsDropDown && AllowDrawArrow) {
				r.Width -= DrawParameters.Constants.GetBarMenuButtonArrowWidth() + DrawParameters.Constants.BarMenuButtonArrowTextIndent;
			}
			else {
				r.Width -= DrawParameters.Constants.GetBarButtonArrowWidth();
			}
			return r;
		}
		protected virtual void CalcHorizontalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			CalcHorizontalBorder(g, sourceObject, ref r);
			Rects[BarLinkParts.Select] = r;
			CalcHorizontalCaptionGlyphInfo(g, sourceObject, r);
			if(Link.IsAllowHtmlText) CalcHtmlStringInfo(g);
		}
		protected virtual void CalcAlignedVerticalCaptionGlyphInfo(Graphics g, object sourceObject, Rectangle r) {
			Rectangle tr, gl, temp;
			temp = tr = gl = Rectangle.Empty;
			int leftIndent = CalcLinkIndent(BarIndent.Left),
				rightIndent = CalcLinkIndent(BarIndent.Right);
			int vertIndent = CalcLinkIndent(BarIndent.Top);
			Size captionSize = CalcRealCaptionSize(g);
			captionSize.Height += VerticalLinkCaptionHeightMargin;
			gl.Size = GlyphSize;
			if(IsHCaptionAlignment) {
				int maxTextHeight = CalcAlignedVerticalCaptionMaxHeight(r, gl, leftIndent);
				gl.Location = new Point(r.X + vertIndent, r.Y + leftIndent);
				tr.Size = new Size(captionSize.Width, maxTextHeight);
				tr.Y = r.Bottom - (maxTextHeight + leftIndent);
				tr.X = r.X + (r.Width - captionSize.Width) / 2;
				if(GetCaptionAlignment() == BarItemCaptionAlignment.Left) {
					temp = tr;
					temp.Y = gl.Y;
					gl.Y += tr.Bottom - tr.Y;
					tr = temp;
				}
			}  else {
				gl.Location = new Point(r.X + vertIndent, r.Y + (r.Height - gl.Height) / 2);
				tr.Size = new Size(captionSize.Width, captionSize.Height);
				tr.Location = new Point(gl.Right + DrawParameters.Constants.BarCaptionGlyphIndent, r.Y + (r.Height - captionSize.Height) / 2);
				if(GetCaptionAlignment() == BarItemCaptionAlignment.Bottom) {
					temp = tr;
					temp.X = gl.X;
					gl.X += tr.Right - tr.X;
					tr = temp;
				}
			}
			Rects[BarLinkParts.Glyph] = gl;
			Rects[BarLinkParts.Caption] = tr;
		}
		protected virtual int CalcAlignedVerticalCaptionMaxHeight(Rectangle r, Rectangle gl, int leftIndent) {
			int res = r.Height - (leftIndent * 2) - gl.Height - DrawParameters.Constants.BarCaptionGlyphIndent;
			if(DrawParameters.BarManagerImageScaleFactor > 1) {
				res += VerticalLinkCaptionHeightMargin;
			}
			return res;
		}
		protected virtual int VerticalLinkCaptionHeightMargin { get { return 3; } }
		protected virtual void CalcVerticalCaptionGlyphInfo(Graphics g, object sourceObject, Rectangle r) {
			if(!IsDrawVerticalRotated) {
				CalcHorizontalCaptionGlyphInfo(g, sourceObject, r);
				return;
			}
			if(!IsDefaultCaptionAlignment) {
				CalcAlignedVerticalCaptionGlyphInfo(g, sourceObject, r);
				return;
			}
			Rectangle tr, gl;
			tr = gl = Rectangle.Empty;
			int leftIndent = CalcLinkIndent(BarIndent.Left),
				rightIndent = CalcLinkIndent(BarIndent.Right);
			int topIndent = CalcLinkIndent(BarIndent.Top);
			int bottomIndent = CalcLinkIndent(BarIndent.Bottom);
			Size captionSize = CalcRealCaptionSize(g);
			int maxTextHeight = r.Height - (leftIndent + rightIndent);
			if(IsDrawPart(BarLinkParts.Glyph)) {
				gl.Size = GlyphSize;
				gl.Location = new Point(r.X + bottomIndent, r.Y + leftIndent);
				maxTextHeight -= (gl.Height + DrawParameters.Constants.BarCaptionGlyphIndent);
			} 
			if(IsDrawPart(BarLinkParts.Caption)) {
				tr.Size = new Size(r.Width - topIndent - bottomIndent, maxTextHeight);
				tr.Y = r.Bottom - (maxTextHeight + rightIndent);
				tr.X = r.X + (bottomIndent);
			}
			Rects[BarLinkParts.Glyph] = gl;
			Rects[BarLinkParts.Caption] = tr;
		}
		protected virtual void CalcVerticalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			CalcVerticalBorder(g, sourceObject, ref r);
			Rects[BarLinkParts.Select] = r;
			CalcVerticalCaptionGlyphInfo(g, sourceObject, r);
		}
		protected virtual void CalcInMenuArrowViewInfo(Graphics g) {
			if(!IsDrawPart(BarLinkParts.OpenArrow)) return;
			Rectangle ar = Rects[BarLinkParts.Select];
			ar.X = (ar.Right - CalcLinkIndent(BarIndent.Right)) + 1;
			ar.Width = DrawParameters.Constants.GetSubMenuArrowWidth();
			Rects[BarLinkParts.OpenArrow] = ar;
		}
		protected void CalcInMenuViewInfo(Graphics g, object sourceObject, Rectangle r) {
			CalcInMenuViewInfoCore(g, sourceObject, r);
			CalcInMenuArrowViewInfo(g);
		}
		protected void CalcInGalleryMenuViewInfo(Graphics g, object sourceObject, Rectangle r) {
			Size captionSize = CalcRealCaptionSize(g);
			Size glyphSize = GlyphSize;
			Rectangle captionBounds = Rectangle.Empty, glyphBounds = Rectangle.Empty;
			CustomSubMenuBarControlViewInfo vi = BarControlInfo as CustomSubMenuBarControlViewInfo;
			if(vi != null && vi.GalleryLinkWidth.ContainsKey(Link))
				r.Width = vi.GalleryLinkWidth[Link];
			GalleryLinkPanel.ArrangeItems(r, glyphSize, captionSize, ref glyphBounds, ref captionBounds);
			Rects[BarLinkParts.Glyph] = glyphBounds;
			Rects[BarLinkParts.Caption] = captionBounds;
			Rects[BarLinkParts.Select] = r;
			Rects[BarLinkParts.Bounds] = r;
		}
		protected virtual void CalcInMenuViewInfoCore(Graphics g, object sourceObject, Rectangle r) {
			ResetReverseLinkRects();
			int leftIndent = CalcLinkIndent(BarIndent.Left),
				rightIndent = CalcLinkIndent(BarIndent.Right);
			int vertIndent = CalcLinkIndent(BarIndent.Top);
			CalcInMenuBorder(g, sourceObject, ref r);
			Rects[BarLinkParts.Select] = r;
			int maxTextWidth = r.Width - (leftIndent + rightIndent);
			Rectangle tr = Rectangle.Empty;
			tr.Size = GlyphSize;
			tr.Location = new Point(r.X + DrawParameters.Constants.SubMenuGlyphHorzIndent - 1, r.Y + (r.Height - tr.Size.Height) / 2);
			Rects[BarLinkParts.Glyph] = tr;
			tr.X = GlyphRect.Right + DrawParameters.Constants.SubMenuGlyphCaptionIndent + leftIndent;
			tr.Y = r.Y + vertIndent;
			tr.Height = r.Height - vertIndent * 2;
			tr.Width = CalcCaptionSize(g).Width + 1; 
			Rects[BarLinkParts.Caption] = tr;
			if(Link.Item.ShortcutKeyDisplayString != string.Empty)
				ShortCutDisplayText = Link.Item.ShortcutKeyDisplayString;
			if(IsItemShortcutExist) {
				tr.Width = PaintHelper.CalcTextSize(g, ShortCutDisplayText, Font, 0, 
					DrawParameters.ShortCutStringFormat, Link).ToSize().Width + 2;
				Rects[BarLinkParts.Shortcut] = tr;
			}
			if(Link.IsAllowHtmlText) CalcHtmlStringInfo(g);
		}
		public void ResetReverseLinkRects() {
			this.rightToLeftReversed = false;
		}
		bool rightToLeftReversed = false;
		public void ReverseLinkRects() {
			if(rightToLeftReversed) return;
			if(IsRightToLeft) {
				rightToLeftReversed = true;
				ReverseLinkRectsCore();
			}
		}
		protected virtual void ReverseLinkRectsCore() {
			Rects[BarLinkParts.Caption] = ReverseLinkRects(Bounds, Rects[BarLinkParts.Caption]);
			Rects[BarLinkParts.Glyph] = ReverseLinkRects(Bounds, Rects[BarLinkParts.Glyph]);
			Rects[BarLinkParts.Shortcut] = ReverseLinkRects(Bounds, Rects[BarLinkParts.Shortcut]);
			Rects[BarLinkParts.Description] = ReverseLinkRects(Bounds, Rects[BarLinkParts.Description]);
			Rects[BarLinkParts.OpenArrow] = ReverseLinkRects(Bounds, Rects[BarLinkParts.OpenArrow]);
			Rects[BarLinkParts.Checkbox] = ReverseLinkRects(Bounds, Rects[BarLinkParts.Checkbox]);
		}
		Rectangle ReverseLinkRects(Rectangle total, Rectangle current) {
			if(current == total || current.IsEmpty || total.Width <= current.Width) return current;
			int delta = current.X - total.X;
			Rectangle newBounds = new Rectangle(total.Right - delta - current.Width, current.Y, current.Width, current.Height);
			return newBounds;
		}
		protected int CalcLinkHeight(Graphics g, object sourceObject) {
			int res = 0;
			g = GInfo.AddGraphics(g);
			try {
				switch(DrawMode) {
					case BarLinkDrawMode.Horizontal : res = CalcLinkHorizontalHeight(g, sourceObject);break;
					case BarLinkDrawMode.Vertical : res = CalcLinkVerticalHeight(g, sourceObject);break;
					case BarLinkDrawMode.InMenu: res = CalcLinkInMenuHeight(g, sourceObject); break;
					case BarLinkDrawMode.InMenuGallery: res = CalcLinkInGalleryMenuHeight(g, sourceObject); break;
					case BarLinkDrawMode.InMenuLarge: res = CalcLinkInMenuHeightLarge(g, sourceObject); break;
					case BarLinkDrawMode.InMenuLargeWithText: res = CalcLinkInMenuHeightLargeDescription(g, sourceObject); break;
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		protected virtual int CalcLinkInMenuHeightLarge(Graphics g, object sourceObject) {
			return CalcLinkInMenuHeight(g, sourceObject);
		}
		protected virtual int CalcLinkInMenuHeightLargeDescription(Graphics g, object sourceObject) {
			int res = 0;
			if(IsDrawPart(BarLinkParts.Glyph)) {
				res = GlyphSize.Height;
			}
			res = Math.Max(CalcCaptionSize(g).Height + CalcDescriptionHeight(g) + 2, res);
			res = Math.Max(CalcLinkContentsHeight(g), res);
			res += CalcLinkIndent(BarIndent.Top) + CalcLinkIndent(BarIndent.Bottom);
			Rectangle bs = CalcBorderSize();
			res += bs.Height + bs.Y;
			return res;
		}
		protected virtual Size CalcLinkInMenuSizeLarge(Graphics g, object sourceObject) {
			return CalcLinkInMenuSize(g, sourceObject);
		}
		protected virtual Size CalcLinkInMenuSizeLargeDescription(Graphics g, object sourceObject) {
			Size res =  CalcLinkInMenuSizeLarge(g, sourceObject);
			res.Width = Math.Max(res.Width, 250);
			return res;
		}
		public virtual Size CalcLinkSize(Graphics g, object sourceObject) {
			UpdatePainters();
			Size res = Size.Empty;
			UpdateLinkInfo(sourceObject);
			g = GInfo.AddGraphics(g);
			try {
				switch(DrawMode) {
					case BarLinkDrawMode.InRadialMenu: res = CalcLinkInRadialMenuSize(g, sourceObject); break;
					case BarLinkDrawMode.Horizontal : res = CalcLinkHorizontalSize(g, sourceObject);break;
					case BarLinkDrawMode.Vertical : res = CalcLinkVerticalSize(g, sourceObject);break;
					case BarLinkDrawMode.InMenu: res = CalcLinkInMenuSize(g, sourceObject); break;
					case BarLinkDrawMode.InMenuGallery: res = CalcLinkInGalleryMenuSize(g, sourceObject); break;
					case BarLinkDrawMode.InMenuLarge: res = CalcLinkInMenuSizeLarge(g, sourceObject); break;
					case BarLinkDrawMode.InMenuLargeWithText: res = CalcLinkInMenuSizeLargeDescription(g, sourceObject); break;
				}
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return res;
		}
		void UpdateItemsPanel() {
			ItemsPanel.Content1HorizontalAlignment = ItemHorizontalAlignment.Center;
			ItemsPanel.Content2HorizontalAlignment = ItemHorizontalAlignment.Center;
			ItemsPanel.Content1VerticalAlignment = ItemVerticalAlignment.Center;
			ItemsPanel.Content2VerticalAlignment = ItemVerticalAlignment.Center;
			ItemsPanel.Content1Location = ItemLocation.Top;
			ItemsPanel.Content1Padding = new Padding(3);
			ItemsPanel.Content2Padding = new Padding(3);
			ItemsPanel.VerticalPadding = new Padding(3);
			ItemsPanel.VerticalIndent = 3;
			ItemsPanel.StretchContent1 = true;
			ItemsPanel.StretchContent2 = true;
		}
		protected virtual Size CalcLinkInRadialMenuSize(Graphics g, object sourceObject) {
			UpdateItemsPanel();
			CaptionRect = new Rectangle(Point.Empty, CalcRadialMenuLinkCaptionSize(g));
			Size glyphSize = GetLinkImage(LinkState) != null ? GlyphSize : Size.Empty;
			return ItemsPanel.CalcBestSize(glyphSize, CaptionRect.Size);
		}
		protected virtual BarLinkDrawMode CalcLinkDrawMode() {
			if(ForceDrawMode != BarLinkDrawMode.Default)
				return ForceDrawMode;
			BarLinkDrawMode res = BarLinkDrawMode.Horizontal;
			if(Link.IsLinkInMenu)
				res = IsLinkInGalleryMenu ? BarLinkDrawMode.InMenuGallery : BarLinkDrawMode.InMenu;
			if(AlwaysDrawAsInMenu) res = BarLinkDrawMode.InMenu;
			if(res != BarLinkDrawMode.InMenu && (BarControlInfo is BarControlViewInfo)) {
				BarControlViewInfo vi = BarControlInfo as BarControlViewInfo;
				if(vi != null && vi.IsVertical) {
					res = BarLinkDrawMode.Vertical;
					if(vi.RotateWhenVertical)
						drawVerticalRotated = true;
				}
			}
			if(this.RadialMenuWindow != null)
				res = BarLinkDrawMode.InRadialMenu;
			return res;
		}
		protected virtual bool IsLinkInGalleryMenu {
			get {
				if(!Link.IsLinkInMenu)
					return false;
				if(Link is BarHeaderItemLink ||  Link is BarScrollItemLink ||  Link is BarEditItemLink)
					return false;
				CustomPopupBarControl subControl = BarControl as CustomPopupBarControl;
				if(subControl == null) return false;
				return subControl.GetLinkMultiColumn(Link);
			}
		}
		protected virtual bool IsInMenuDefault {
			get { 
				if(!IsLinkInMenu)
					return false;
				if(BarControl == null)
					return true;
				CustomSubMenuBarControlViewInfo vi = BarControl.ViewInfo as CustomSubMenuBarControlViewInfo;
				if(vi == null)
					return true;
				return vi.DrawMode == MenuDrawMode.Default;
			}
		}
		public virtual void UpdateLinkInfo(object sourceObject) {
			this.fDrawParts = BarLinkParts.Bounds;
			this.isRecentLink = true;
			this.linkState = CalcLinkState();
			this.drawMode = CalcLinkDrawMode();
			BarItemPaintStyle ps = GetPaintStyle();
			if(ps == BarItemPaintStyle.Caption || ps == BarItemPaintStyle.CaptionGlyph ||
				(ps == BarItemPaintStyle.CaptionInMenu && IsLinkInMenu)) {
				if(!IsLinkInGalleryMenu || ShouldShowItemTextInGallery(Link))
					this.fDrawParts |= BarLinkParts.Caption;
			}
			if(DrawMode == BarLinkDrawMode.InMenuLargeWithText)
				this.fDrawParts |= BarLinkParts.Description;
			if(GetBorder() != BorderStyles.NoBorder) 
				this.fDrawParts |= BarLinkParts.Border;
			if(ps == BarItemPaintStyle.CaptionGlyph || (ps == BarItemPaintStyle.CaptionInMenu && !IsLinkInMenu) || AlwaysDrawGlyph) {
				this.fDrawParts |= BarLinkParts.Glyph;
			}
			if((ps == BarItemPaintStyle.CaptionInMenu && DrawMode == BarLinkDrawMode.InRadialMenu))
				this.fDrawParts |= BarLinkParts.Glyph;
			if(IsLinkInMenu) {
				if(Link.Item.ShowItemShortcut == DefaultBoolean.True || 
					(Link.Item.ShowItemShortcut == DefaultBoolean.Default && (DrawMode != BarLinkDrawMode.InMenuLarge && DrawMode != BarLinkDrawMode.InMenuLargeWithText))) 
					this.fDrawParts |= BarLinkParts.Shortcut;
			}
			if(sourceObject is SubMenuBarControl) {
				SubMenuBarControl frm = sourceObject as SubMenuBarControl;
				if(frm.ContainerLink != null) {
					this.isRecentLink = !frm.ContainerLink.IsNotRecentLink(this.Link);
				}
			}
			AppearanceObject app = OwnerAppearances != null? OwnerAppearances.GetAppearance(LinkState) : null;
			if(app == null)
				return;
			if(IsRightToLeft || (app.TextOptions.HotkeyPrefix != HKeyPrefix.Default || app.TextOptions.Trimming != Trimming.Default) && 
				this.fLinkCaptionStringFormat == null) {
					this.fLinkCaptionStringFormat = (StringFormat)LinkCaptionStringFormat.Clone();
			}
			if(app.TextOptions.HotkeyPrefix != HKeyPrefix.Default) {
				this.fLinkCaptionStringFormat.HotkeyPrefix = app.GetStringFormat().HotkeyPrefix;
			}
			if(app.TextOptions.Trimming != Trimming.Default) {
				this.fLinkCaptionStringFormat.Trimming = app.GetStringFormat().Trimming;
			}
			if(fLinkCaptionStringFormat != null) {
				var flags = fLinkCaptionStringFormat.FormatFlags;
				if(IsRightToLeft)
					flags |= StringFormatFlags.DirectionRightToLeft;
				else
					flags &= (~StringFormatFlags.DirectionRightToLeft);
				fLinkCaptionStringFormat.FormatFlags = flags;
			}
		}
		public virtual bool UpdateLinkState() {
			BarLinkState newState = CalcLinkState();
			if(CompareLinkState(newState, LinkState)) return false;
			LinkState = newState;
			BarControlViewInfo vi = BarControlInfo as BarControlViewInfo;
			if(vi != null) 
				vi.OnLinkStateUpdated(this);
			UpdateOwnerAppearance();
			return true;
		}
		protected virtual bool CompareLinkState(BarLinkState newState, BarLinkState current) {
			return newState == current;
		}
		protected virtual BarLinkState CalcLinkState() {
			BarLinkState res = BarLinkState.Normal;
			if(!Link.Enabled)
				return BarLinkState.Disabled;
			if(!Link.Manager.IsCustomizing && CanDrawAs(BarLinkState.Highlighted)) {
				if((Manager.SelectionInfo.HighlightedLink == Link &&
					(Manager.SelectionInfo.PressedLink == null || Manager.SelectionInfo.PressedLink == Link || IsLinkInMenu) &&
					Manager.SelectionInfo.EditingLink == null) 
					|| Manager.SelectionInfo.AllowPaintHighlightWhenEditing(Link)) {
					res |= BarLinkState.Highlighted;
				}
			}
			if(CanDrawAs(BarLinkState.Pressed) && !IsLinkInMenu && Link == Manager.SelectionInfo.PressedLink) {
				res |= BarLinkState.Pressed;
			}
			if((res & BarLinkState.Pressed) != 0) {
				if(Manager.SelectionInfo.LockNullPressed != 0) return BarLinkState.Pressed;
				if((res & BarLinkState.Highlighted) != 0) return BarLinkState.Pressed;
				return BarLinkState.Highlighted;
			}
			if((res & BarLinkState.Highlighted) != 0) {
				if(((Manager.SelectionInfo.PressedLink != null) && (Manager.SelectionInfo.HighlightedLink != null))
					&& (Manager.SelectionInfo.PressedLink.Holder == Manager.SelectionInfo.HighlightedLink.Holder)
					&& (Manager.SelectionInfo.LockNullPressed != 0)) {
					return (BarLinkState.Normal);
				}
				return BarLinkState.Highlighted;
			}
			return BarLinkState.Normal;
		}
		protected virtual int CalcLinkContentsHeight(Graphics g) {
			return 0;
		}
		protected virtual int CalcLinkHorizontalHeight(Graphics g, object sourceObject) {
			int res = 0;
			if(IsDefaultCaptionAlignment || IsHCaptionAlignment) {
					res = GlyphSize.Height;
				if(IsDrawPart(BarLinkParts.Caption)) {
					res = Math.Max(CalcCaptionSize(g).Height + 1, res);
				}
				if(res < 4) res = 8;
			} else {
				res = GlyphSize.Height + CalcCaptionSize(g).Height + DrawParameters.Constants.BarCaptionGlyphIndent;
			}
			res = Math.Max(CalcLinkContentsHeight(g), res);
			res += CalcLinkIndent(BarIndent.Top) + CalcLinkIndent(BarIndent.Bottom); 
			Rectangle bs = CalcBorderSize();
			res += bs.Height + bs.Y;
			return res;
		}
		protected virtual int CalcLinkVerticalHeight(Graphics g, object sourceObject) {
			int res = 0;
			if(!IsDrawVerticalRotated) return CalcLinkHorizontalHeight(g, sourceObject);
			if(IsDefaultCaptionAlignment || IsHCaptionAlignment) {
				if(IsDrawPart(BarLinkParts.Glyph)) {
					res = GlyphSize.Height;
					if(IsDrawPart(BarLinkParts.Caption)) res += DrawParameters.Constants.BarCaptionGlyphIndent;
				}
				if(IsDrawPart(BarLinkParts.Caption)) {
					res += CalcCaptionSize(g).Height;
				}
			} else {
				res = Math.Max(GlyphSize.Height, CalcCaptionSize(g).Height);
			}
			res += CalcLinkIndent(BarIndent.Left) + CalcLinkIndent(BarIndent.Right); 
			Rectangle bs = CalcBorderSize();
			res += bs.Height + bs.Y;
			return res;
		}
		protected virtual int CalcLinkInMenuHeight(Graphics g, object sourceObject) {
			return CalcLinkHorizontalHeight(g, sourceObject);
		}
		protected virtual int CalcLinkInGalleryMenuHeight(Graphics g, object sourceObject) {
			Size captionSize = CalcRealCaptionSize(g);
			Size glyphSize = GlyphSize;
			Size res = GalleryLinkPanel.CalcBestSize(glyphSize, captionSize);
			return res.Height;
		}
		protected virtual int CalcLinkHorizontalCalptionGlyphWidth(Graphics g, object sourceObject) {
			int res = 0;
			if(IsDefaultCaptionAlignment || IsHCaptionAlignment) {
				if(IsDrawPart(BarLinkParts.Glyph)) {
					res += GlyphSize.Width;
					if(IsDrawPart(BarLinkParts.Caption)) res += DrawParameters.Constants.BarCaptionGlyphIndent;
				}
				if(IsDrawPart(BarLinkParts.Caption)) {
					Size size = CalcRealCaptionSize(g);
					res += size.Width + 1;
				}
			} else {
				res = Math.Max(GlyphSize.Width, CalcRealCaptionSize(g).Width + 1);
			}
			return res;
		}
		protected virtual Size CalcLinkHorizontalSize(Graphics g, object sourceObject) {
			bool freeGraphics = g == null;
			Size res = Size.Empty;
			if(GetLinkWidth() > 0)
				res.Width = GetLinkWidth();
			else {
				res.Width += CalcLinkHorizontalCalptionGlyphWidth(g, sourceObject);
				res.Width += CalcLinkIndent(BarIndent.Left) + CalcLinkIndent(BarIndent.Right);
				Rectangle bs = CalcBorderSize();
				res.Width += bs.Width + bs.X;
			}
			if(Link.Item.Height != 0)
				res.Height = Link.Item.Height;
			else 
				res.Height = CalcLinkHeight(g, sourceObject);
			return res;
		}
		protected virtual int GetLinkWidth() {
			return Link.Width;
		}
		protected virtual int CalcLinkVerticalCalptionGlyphWidth(Graphics g, object sourceObject) {
			int res = 0;
			int glyphWidth = GlyphSize.Width;
			Size captionSize = CalcRealCaptionSize(g);
			if(IsDrawVerticalRotated) {
				if(IsDefaultCaptionAlignment || IsHCaptionAlignment) {
					if(IsDrawPart(BarLinkParts.Glyph)) {
						res = glyphWidth;
					}
					if(IsDrawPart(BarLinkParts.Caption)) {
						res = Math.Max(captionSize.Width + 1, res);
					}
				} else {
					res = glyphWidth + DrawParameters.Constants.BarCaptionGlyphIndent + captionSize.Width + 1;
				}
			} else {
				if(IsDefaultCaptionAlignment || IsHCaptionAlignment) {
					if(IsDrawPart(BarLinkParts.Glyph)) {
						res = glyphWidth;
						if(IsDrawPart(BarLinkParts.Caption)) res += DrawParameters.Constants.BarCaptionGlyphIndent;
					}
					if(IsDrawPart(BarLinkParts.Caption)) {
						res += captionSize.Width + 1;
					}
				} else {
					res = Math.Max(glyphWidth, captionSize.Width + 1);
				}
			}
			return res;
		}
		protected virtual Size CalcLinkVerticalSize(Graphics g, object sourceObject) {
			Size res = Size.Empty;
			res.Width += CalcLinkVerticalCalptionGlyphWidth(g, sourceObject);
			if(IsDrawVerticalRotated)
				res.Width += CalcLinkIndent(BarIndent.Top) + CalcLinkIndent(BarIndent.Bottom);
			else
				res.Width += CalcLinkIndent(BarIndent.Left) + CalcLinkIndent(BarIndent.Right);
			Rectangle bs = CalcBorderSize();
			res.Width += bs.Width + bs.X;
			res.Height += CalcLinkHeight(g, sourceObject);
			return res;
		}
		Items2Panel linkPanel;
		protected Items2Panel GalleryLinkPanel {
			get {
				if(linkPanel == null)
					linkPanel = new Items2Panel();
				return linkPanel;
			}
		}
		protected ItemHorizontalAlignment GetGalleryImageHorizontalAlignment(BarItemLink link) {
			CustomSubMenuBarControlViewInfo vi = BarControlInfo as CustomSubMenuBarControlViewInfo;
			if(vi == null) return ItemHorizontalAlignment.Center;
			return vi.GetGalleryImageHorizontalAlignment(link);
		}
		protected ItemVerticalAlignment GetGalleryImageVerticalAlignment(BarItemLink link) {
			CustomSubMenuBarControlViewInfo vi = BarControlInfo as CustomSubMenuBarControlViewInfo;
			if(vi == null) return ItemVerticalAlignment.Center;
			return vi.GetGalleryImageVerticalAlignment(link);
		}
		protected bool GetUseItemMaxWidth(BarItemLink link) {
			CustomSubMenuBarControlViewInfo vi = BarControlInfo as CustomSubMenuBarControlViewInfo;
			if(vi == null) return false;
			return vi.GetUseMaxItemWidth(link);
		}
		protected virtual void SetupLinkPanel() {
			GalleryLinkPanel.Content1Location = ItemLocation.Top;
			GalleryLinkPanel.Content1HorizontalAlignment = GetGalleryImageHorizontalAlignment(Link);
			GalleryLinkPanel.Content1VerticalAlignment = GetGalleryImageVerticalAlignment(Link);
			GalleryLinkPanel.Content2HorizontalAlignment = ItemHorizontalAlignment.Center;
			GalleryLinkPanel.Content1Padding = GalleryLinkPanel.VerticalPadding = new Padding(CalcLinkIndent( BarIndent.Left), 
																		CalcLinkIndent(BarIndent.Top), 
																		CalcLinkIndent(BarIndent.Right), 
																		CalcLinkIndent(BarIndent.Bottom));
			GalleryLinkPanel.VerticalPadding = GalleryLinkPanel.Content1Padding;
			GalleryLinkPanel.VerticalIndent = DrawParameters.Constants.BarCaptionGlyphIndent;
		}
		protected virtual Size CalcLinkInGalleryMenuSize(Graphics g, object sourceObject) {
			SetupLinkPanel();
			Size captionSize = ShouldShowItemTextInGallery(Link)? CalcRealCaptionSize(g): Size.Empty;
			if(captionSize.Width > 0)
				captionSize.Width += 1;
			Size glyphSize = GlyphSize;
			Size res = GalleryLinkPanel.CalcBestSize(glyphSize, captionSize);
			int itemMaxWidth = GetItemMaxWidth(Link);
			if(itemMaxWidth > 0)
				res.Width = Math.Min(res.Width, itemMaxWidth);
			return res;
		}
		protected virtual bool ShouldShowItemTextInGallery(BarItemLink link) {
			CustomSubMenuBarControlViewInfo vi = BarControlInfo as CustomSubMenuBarControlViewInfo;
			if(vi == null) return true;
			return vi.GetShowItemTextInGallery(link);
		}
		protected virtual int GetItemMaxWidth(BarItemLink link) {
			CustomSubMenuBarControlViewInfo vi = BarControlInfo as CustomSubMenuBarControlViewInfo;
			if(vi == null) return 0;
			return vi.GetItemMaxWidth(link);
		}
		protected virtual bool ShouldUseLargeImagesInGallery(BarItemLink link) {
			CustomSubMenuBarControlViewInfo vi = BarControlInfo as CustomSubMenuBarControlViewInfo;
			if(vi == null) return true;
			return vi.UseLargeImagesInGallery(link);
		}
		protected virtual Size CalcLinkInMenuSize(Graphics g, object sourceObject) {
			Size res = Size.Empty;
			if(AllowGlyph) res.Width = GlyphSize.Width + DrawParameters.Constants.SubMenuGlyphHorzIndent + DrawParameters.Constants.SubMenuGlyphCaptionIndent;
			Size size = CalcRealCaptionSize(g);
			res.Width += size.Width + 1;
			if(IsDrawPart(BarLinkParts.Shortcut)) {
				if(IsItemShortcutExist) {
					res.Width += DrawParameters.Constants.SubMenuCaptionIndentShortCut + PaintHelper.CalcTextSize(g, ShortCutDisplayText, Font, 0, 
						DrawParameters.ShortCutStringFormat, Link).ToSize().Width + 1;
				}
			}
			res.Width += CalcLinkIndent(BarIndent.Left) + (SkipRightIndentInMenu? 0: CalcLinkIndent(BarIndent.Right));
			Rectangle bs = CalcBorderSize();
			res.Width += bs.Width + bs.X;
			res.Height += CalcLinkHeight(g, sourceObject);
			return res;
		}
		protected virtual bool AllowGlyph { get { return true; } }
		public void UpdateViewInfo() {
			if(Link == null) return;
			UpdateViewInfo(Link.ScreenToLinkPoint(Control.MousePosition));
		}
		public virtual void UpdateViewInfo(Point mousePosition) {
		}
		public override void Clear() {
			base.Clear();
			this.fDrawParts = BarLinkParts.Bounds;
			this.drawMode = BarLinkDrawMode.Horizontal;
			this.isRecentLink = true;
			this.drawVerticalRotated = false;
			this.linkState = BarLinkState.Normal;
			this.Rects.Clear();
		}
		public virtual Brush GetForeBrush(GraphicsCache cache, BarLinkState state) {
			CustomControlViewInfo bv = BarControlInfo;
			if(AllowGetForeBrushInOwnerAppearance()) return OwnerAppearance.GetForeBrush(cache);
			return cache.GetSolidBrush(DrawParameters.GetLinkForeColor(this, bv == null ? DrawParameters.Colors[BarColor.LinkForeColor] : bv.Appearance.GetAppearance(state).ForeColor, state));
		}
		protected virtual bool AllowGetForeBrushInOwnerAppearance() {
			return OwnerAppearance != null && OwnerAppearance.Options.UseForeColor;
		}
		public virtual Brush GetBackBrush(GraphicsCache cache) {
			if(barBackBrush != null) return barBackBrush;
			return OwnerAppearance.GetBackBrush(cache);
		}
		public virtual Brush GetBackBrush(GraphicsCache cache, Rectangle r) {
			if(barBackBrush != null) return barBackBrush;
			return OwnerAppearance.GetBackBrush(cache, r);
		}
		StateAppearances appearances = null;
		AppearanceObject ownerAppearance = null;
		protected virtual BarLinkState GetStateForAppearance() {
			return LinkState;
		}
		protected virtual AppearanceObject GetOwnerAppearanceByState(BarLinkState state) {
			if(this.ownerAppearance != null)
				return this.ownerAppearance;
			return this.appearances.GetAppearance(state); 
		}
		protected internal virtual void UpdateOwnerAppearance() {
			this.appearances = UpdateOwnerAppearancesCore();
			this.descriptionAppearance = UpdateOwnerDescriptionAppearanceCore();
		}
		protected internal virtual void SetForcedOwnerAppearance(AppearanceObject appearance) {
			this.ownerAppearance = appearance;
		}
		protected internal virtual AppearanceObject UpdateOwnerDescriptionAppearanceCore() {
			this.linkState = CalcLinkState();
			this.drawMode = CalcLinkDrawMode();
			BarLinkState state = GetStateForAppearance();
			AppearanceObject obj = new AppearanceObject();
			AppearanceObject itemDescApp = Manager.GetController().AppearancesRibbon.ItemDescription;
			if (Link.Enabled) {
				if (state == BarLinkState.Highlighted || state == BarLinkState.Selected)
					itemDescApp = Manager.GetController().AppearancesRibbon.ItemDescriptionHovered;
				else if (state == BarLinkState.Pressed || state == BarLinkState.Checked)
					itemDescApp = Manager.GetController().AppearancesRibbon.ItemDescriptionPressed;
			}
			else {
				itemDescApp = Manager.GetController().AppearancesRibbon.ItemDescriptionDisabled;
			}
			AppearanceHelper.Combine(obj, itemDescApp, DrawParameters.Colors.MenuItemDescription.GetAppearance(state));
			if (!Link.Enabled && !itemDescApp.Options.UseForeColor)
				obj.ForeColor = DrawParameters.GetLinkForeColorDisabled(this, BarLinkState.Disabled);
			return obj;
		}
		protected bool IsLinkInMainMenu { get { return Bar != null && Bar.IsMainMenu; } }
		protected void UpdateHotKeyPrefix(StateAppearances app) {
			if(!(IsLinkInMainMenu || IsLinkInMenu) || !Link.IsAllowHtmlText)
				return;
			bool showKeybCues = GetShowKeyboardCues();
			HKeyPrefix prefix = Manager.SelectionInfo.IsAltKeyPressed || showKeybCues? HKeyPrefix.Show : HKeyPrefix.Hide;
			if(app.Normal.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				app.Normal.TextOptions.HotkeyPrefix = prefix;
			if(app.Hovered.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				app.Hovered.TextOptions.HotkeyPrefix = prefix;
			if(app.Pressed.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				app.Pressed.TextOptions.HotkeyPrefix = prefix;
			if(app.Disabled.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				app.Disabled.TextOptions.HotkeyPrefix = prefix;
		}
		private bool GetShowKeyboardCues() {
			PropertyInfo pi = typeof(Control).GetProperty("ShowKeyboardCues", BindingFlags.NonPublic | BindingFlags.Instance);
			return (bool)pi.GetValue(Manager.Form, null);
		}
		protected internal virtual StateAppearances UpdateOwnerAppearancesCore() {
			this.linkState = CalcLinkState();
			this.drawMode = CalcLinkDrawMode();
			BarLinkState state = GetStateForAppearance();
			StateAppearances obj = new StateAppearances();
			if(IsLinkInMenu && !(Link.BarControl is QuickCustomizationBarControl)) {
				AppearanceHelper.Combine(obj.Normal, GetItemAppearance(BarLinkState.Normal), GetMenuAppearance(BarLinkState.Normal, AppearanceMenu));
				AppearanceHelper.Combine(obj.Hovered, GetItemAppearance(BarLinkState.Highlighted), GetMenuAppearance(BarLinkState.Highlighted, AppearanceMenu));
				AppearanceHelper.Combine(obj.Pressed, GetItemAppearance(BarLinkState.Pressed), GetMenuAppearance(BarLinkState.Pressed, AppearanceMenu));
				AppearanceHelper.Combine(obj.Disabled, GetItemAppearance(BarLinkState.Disabled), GetMenuAppearance(BarLinkState.Disabled, AppearanceMenu));
			}
			else if(BarControlInfo != null) {
				AppearanceHelper.Combine(obj.Normal, GetItemAppearance(BarLinkState.Normal), GetBarControlAppearance(BarLinkState.Normal, BarControlInfo));
				AppearanceHelper.Combine(obj.Hovered, GetItemAppearance(BarLinkState.Highlighted), GetBarControlAppearance(BarLinkState.Highlighted, BarControlInfo));
				AppearanceHelper.Combine(obj.Pressed, GetItemAppearance(BarLinkState.Pressed), GetBarControlAppearance(BarLinkState.Pressed, BarControlInfo));
				AppearanceHelper.Combine(obj.Disabled, GetItemAppearance(BarLinkState.Disabled), GetBarControlAppearance(BarLinkState.Disabled, BarControlInfo));
			}
			else {
				AppearanceHelper.Combine(obj.Normal, GetItemAppearance(BarLinkState.Normal), DrawParameters.StateAppearance(BarAppearance.Bar).GetAppearance(BarLinkState.Normal));
				AppearanceHelper.Combine(obj.Hovered, GetItemAppearance(BarLinkState.Highlighted), DrawParameters.StateAppearance(BarAppearance.Bar).GetAppearance(BarLinkState.Highlighted));
				AppearanceHelper.Combine(obj.Pressed, GetItemAppearance(BarLinkState.Pressed), DrawParameters.StateAppearance(BarAppearance.Bar).GetAppearance(BarLinkState.Pressed));
				AppearanceHelper.Combine(obj.Disabled, GetItemAppearance(BarLinkState.Disabled), DrawParameters.StateAppearance(BarAppearance.Bar).GetAppearance(BarLinkState.Disabled));
			}
			UpdateHotKeyPrefix(obj);
			return obj;
		}
		protected internal virtual AppearanceObject UpdateOwnerAppearanceCore() {
			this.linkState = CalcLinkState();
			this.drawMode = CalcLinkDrawMode();
			BarLinkState state = GetStateForAppearance();
			AppearanceObject obj = new AppearanceObject();
			if(IsLinkInMenu && !(Link.BarControl is QuickCustomizationBarControl))
				AppearanceHelper.Combine(obj, GetItemAppearance(state), GetMenuAppearance(state, AppearanceMenu));
			else if(BarControlInfo != null) {
				AppearanceHelper.Combine(obj, GetItemAppearance(state), GetBarControlAppearance(state, BarControlInfo));
			} else {
				AppearanceHelper.Combine(obj, GetItemAppearance(state), DrawParameters.StateAppearance(BarAppearance.Bar).GetAppearance(state));
			}
			return obj;
		}
		protected AppearanceObject GetBarControlAppearance(BarLinkState state, CustomControlViewInfo barControlInfo) {
			return barControlInfo.Appearance.GetAppearance(state);
		}
		protected AppearanceObject GetMenuAppearance(BarLinkState state, MenuAppearance menu) {
			return menu.AppearanceMenu.GetAppearance(state);
		}
		protected internal AppearanceObject GetItemAppearance(BarLinkState state) {
			if(IsLinkInMenu) {
				return Link.Item.ItemInMenuAppearance.GetAppearance(state);
			}
			return Link.Item.ItemAppearance.GetAppearance(state);
		}
		public virtual StateAppearances OwnerAppearances {
			get {
				return this.appearances;
			}
		}
		public virtual AppearanceObject OwnerAppearance {
			get {
				if(this.ownerAppearance != null)
					return this.ownerAppearance;
				return this.appearances.GetAppearance(LinkState);
			}
		}
		public virtual void SetBackBrush(Brush brush) {
			this.barBackBrush = brush;
		}
		public virtual bool IsRecentLink { get { return isRecentLink; } }
		public virtual bool IsLinkInMenu { get { return AlwaysDrawAsInMenu || DrawMode == BarLinkDrawMode.InMenu || DrawMode == BarLinkDrawMode.InMenuLarge || DrawMode == BarLinkDrawMode.InMenuLargeWithText || DrawMode == BarLinkDrawMode.InRadialMenu; } }
		public virtual bool IsDrawVerticalRotated { get { return DrawMode == BarLinkDrawMode.Vertical && drawVerticalRotated; } }
		public virtual CustomControl BarControl {
			get { 
				if(BarControlInfo != null) return BarControlInfo.BarControl;
				return null;
			}
		}
		public Bar Bar { 
			get {
				CustomBarControl bc = BarControl as CustomBarControl;
				if(bc != null) return bc.Bar;
				return null;
			}
		}
		public CustomSubMenuBarControlViewInfo MenuInfo {
			get {
				return BarControlInfo as CustomSubMenuBarControlViewInfo;
			}
		}
		public virtual MenuAppearance AppearanceMenu {
			get {
				if(MenuInfo != null) return MenuInfo.AppearanceMenu;
				return DrawParameters.Colors.MenuAppearance;
			}
		}
		public StateAppearances AppearanceMenuItemCaption {
			get {
				return DrawParameters.Colors.MenuItemCaptionWithDescription;
			}
		}
		public AppearanceObject AppearanceMenuItemDescription {
			get {
				return descriptionAppearance;
			}
		}
		protected Font GetFontCore(BarLinkState state) {
			if(Link.HasFont || BarControlInfo == null) return Link.Font;
			Font font = OwnerAppearances.GetAppearance(state).GetFont();
			return font;
		}
		public Font Font {
			get {
				return GetFontByState(BarLinkState.Normal);
			}
		}
		public virtual Font GetFontByState(BarLinkState state) {
			if(DrawMode == BarLinkDrawMode.InMenuLargeWithText) {
				if(Link.HasFont || OwnerAppearance.Options.UseFont)
					return GetFontCore(state);
				return AppearanceMenuItemCaption.GetAppearance(state).Font;
			}
			if(DrawMode == BarLinkDrawMode.InRadialMenu) {
				return OwnerAppearances.GetAppearance(state).GetFont();
			}
			return GetFontCore(state);
		}
		public Font DescriptionFont { get { return AppearanceMenuItemDescription.Font; } }
		public virtual BarLinkPainter Painter {
			get { return painter; }
			set { painter = value;
			}
		}
		public virtual Rectangle ParentRectangle {
			get {
				BarControlRowViewInfo rv = ParentViewInfo as BarControlRowViewInfo;
				if(rv != null) return rv.Bounds;
				CustomControlViewInfo vi = ParentViewInfo as CustomControlViewInfo;
				if(vi != null) return vi.Bounds;
				return Rectangle.Empty;
			}
		}
		public virtual CustomControlViewInfo BarControlInfo {
			get {
				object info = ParentViewInfo;
				if(info is BarControlRowViewInfo) {
					info = (info as BarControlRowViewInfo).BarInfo;
				}
				if(info is CustomControlViewInfo) {
					return (info as CustomControlViewInfo);
				}
				return null;
			}
		}
		public virtual void UpdateLinkWidthInSubMenu(int linkWidth) {
			if(IsLinkInGalleryMenu)
				return;
			int delta = linkWidth - Bounds.Width;
			Rects.AddWidth(BarLinkParts.Select, delta);
			if(!Rects[BarLinkParts.Border].IsEmpty) 
				Rects.AddWidth(BarLinkParts.Border, delta);
			Rects.SetWidth(BarLinkParts.Bounds, linkWidth);
			if(!Rects[BarLinkParts.Shortcut].IsEmpty) {
				Rectangle sr = Rects[BarLinkParts.Shortcut];
				sr.X = (Bounds.Right - (sr.Width + CalcLinkIndent(BarIndent.Right)));
				Rects[BarLinkParts.Shortcut] = sr;
			}
			if(!Rects[BarLinkParts.Description].IsEmpty) {
				Rectangle r = Rects[BarLinkParts.Description];
				r.Width = (SelectRect.Right - CalcLinkIndent(BarIndent.Right) - r.X);
				Rects[BarLinkParts.Description] = r;
			}
		}
		public virtual Size LargeGlyphSize {
			get {
				Size res = Size.Empty;
				bool isDrawGlyph = IsDrawPart(BarLinkParts.Glyph);
				if(Link.IsLargeImageExist) res = GetLargeImagePartSize();
				if(Link.HasLargeGlyph) res = Link.GetLargeGlyphSize();
				if(res.IsEmpty && IsShouldUseDefaultLargeGlyphSize) res = Manager.GetController().PropertiesBar.DefaultLargeGlyphSize;
				if(!FixedGlyphSize.IsEmpty) res = FixedGlyphSize;
				res = UpdateGlyphSize(res, true);
				res = RaiseGlyphSizeEvent(res);
				return res; 
			}
		}	
		protected virtual bool IsShouldUseDefaultLargeGlyphSize {
			get {
				if(IsLinkInMenu || IsLinkInGalleryMenu) return true;
				if(!Link.IsLargeImageExist && Link.IsImageExist) return true;
				return false;
			}
		}
		public virtual bool IsShouldUseLargeImages {
			get {
				if(DrawMode == BarLinkDrawMode.InRadialMenu && Link.IsLargeImageExist)
					return true;
				if(DrawMode == BarLinkDrawMode.InMenuLarge || DrawMode == BarLinkDrawMode.InMenuLargeWithText)
					return true;
				if(DrawMode == BarLinkDrawMode.InMenuGallery && ShouldUseLargeImagesInGallery(Link))
					return true;
				if(!IsLinkInMenu)
					return Manager.GetProperties().LargeIcons;
				if(DrawMode == BarLinkDrawMode.InMenu && !IsInMenuDefault)
					return false;
				if(Manager.LargeIcons)
					return Manager.GetProperties().LargeIconsInMenu != DefaultBoolean.False;
				return Manager.GetProperties().LargeIconsInMenu == DefaultBoolean.True;
			}
		}
		protected internal virtual Size LinkGlyphSize { get { return GetGlyph().Size; } }
		protected internal virtual Size ImageUriImageSize { get { return Link.ImageUri.GetImage().Size; } }
		protected virtual Image GetGlyph() { return Link.Glyph; }
		public virtual Size GlyphSize {
			get {
				Size res = Size.Empty;
				Image glyph = GetGlyph();
				if(IsShouldUseLargeImages) return LargeGlyphSize;
				bool isDrawGlyph = IsDrawPart(BarLinkParts.Glyph);
				if(isDrawGlyph && Link.IsImageExist && glyph == null && (Link.ImageUri == null || !Link.ImageUri.HasImage))
					res = GetImagePartSize();
				if(isDrawGlyph && glyph != null)
					res = LinkGlyphSize;
				if(isDrawGlyph && Link.ImageUri != null && Link.ImageUri.HasImage)
					res = ImageUriImageSize;
				if(res.IsEmpty && (IsLinkInMenu || IsLinkInGalleryMenu)) 
					res = Manager.GetController().PropertiesBar.DefaultGlyphSize;
				if(!FixedGlyphSize.IsEmpty) res = FixedGlyphSize;
				if(!res.IsEmpty) res = UpdateGlyphSize(res, true);
				res = RaiseGlyphSizeEvent(res);
				if(!IsDrawPart(BarLinkParts.Glyph) && !IsLinkInMenu) return Size.Empty;
				return res; 
			}
		}
		protected virtual Size GetImagePartSize() {
			return GetImagePartSizeCore(Link.Item.Manager.Images);
		}
		protected virtual Size GetLargeImagePartSize() {
			return GetImagePartSizeCore(Link.Item.LargeImages);
		}
		protected virtual Size GetImagePartSizeCore(object images) {
			if(Link.Item.Manager.SharedImageCollectionImageSizeMode == SharedImageCollectionImageSizeMode.UseImageSize) {
				Image image = GetLinkImage(LinkState);
				return image.Size;
			}
			return ImageCollection.GetImageListSize(images);
		}
		public virtual Size UpdateGlyphSize(Size size, bool add) {
			if(size.IsEmpty) return size;
			int mul = add ? 1 : -1;
			if(add) size = ScaleSize(size);
			size.Width += DrawParameters.Constants.ImageShadowHIndent * mul ;
			size.Height += DrawParameters.Constants.ImageShadowHIndent * mul;
			size.Width += DrawParameters.Constants.ImageHIndent * mul;
			size.Height += DrawParameters.Constants.ImageVIndent * mul;
			if(!add) size = UnScaleSize(size);
			return size;
		}
		public Size ScaleSize(Size size) {
			if(!DrawParameters.ScaleImages) return size;
			size.Width = (int)(((float)size.Width) * DrawParameters.BarManagerImageScaleFactor);
			size.Height = (int)(((float)size.Height) * DrawParameters.BarManagerImageScaleFactor);
			return size;
		}
		public Size UnScaleSize(Size size) {
			if(!DrawParameters.ScaleImages) return size;
			size.Width = (int)(((float)size.Width) / DrawParameters.BarManagerImageScaleFactor);
			size.Height = (int)(((float)size.Height) / DrawParameters.BarManagerImageScaleFactor);
			return size;
		}
		protected internal virtual Size CalcRealCaptionSize(Graphics g) {
			Size size = CalcCaptionSize(g);
			if(DisplayCaption == null || DisplayCaption.Length == 0) 
				size.Width = 0;
			return size;
		}
		protected internal RadialMenuLinkMetrics RadialMenuLinkMetrics { get { return radialMenuLinkMetrics; } }
		protected internal void CreateRadialMenuLinkMetrics(Graphics g) {
			UpdatePainters();
			this.radialMenuLinkMetrics = new RadialMenuLinkMetrics();
			RadialMenuLinkMetrics.WrappedText = BarItemLinkTextHelper.WrapString(DisplayCaption, false);
			RadialMenuLinkMetrics.WrappedTextSize = new Size[RadialMenuLinkMetrics.WrappedText.Length];
			for(int i = 0; i < RadialMenuLinkMetrics.WrappedText.Length; i++) {
				RadialMenuLinkMetrics.WrappedTextSize[i] = CalcCaptionSize(g, RadialMenuLinkMetrics.WrappedText[i]);
			}
		}
		protected internal Size CalcRadialMenuLinkCaptionSize(Graphics g) {
			if(RadialMenuLinkMetrics == null) return CalcCaptionSize(g);
			return RadialMenuLinkMetrics.CalcLinkSize();
		}
		protected virtual Size CalcCaptionSize(Graphics g, string caption) {
			Size size = Size.Empty, sizeNormal = Size.Empty, sizeHot = Size.Empty, sizePressed = Size.Empty, sizeDisabled = Size.Empty;
			g = GInfo.AddGraphics(g);
			try {
				if(link.IsAllowHtmlText) {
					StringInfo info = StringPainter.Default.Calculate(g, OwnerAppearances.Normal, Link.Caption, 0);
					sizeNormal = info.Bounds.Size;
					sizeHot = CalcHtmlTextSizeByAppearance(g, sizeHot, BarLinkState.Highlighted);
					sizePressed = CalcHtmlTextSizeByAppearance(g, sizePressed, BarLinkState.Pressed);
					sizeDisabled = CalcHtmlTextSizeByAppearance(g, sizeDisabled, BarLinkState.Disabled);
				}
				else {
					sizeNormal = PaintHelper.CalcTextSize(g, caption, Font, 0, LinkCaptionStringFormat, Link).ToSize();
					sizeHot = CalcTextSizeByAppearance(g, caption, sizeHot, BarLinkState.Highlighted);
					sizePressed = CalcTextSizeByAppearance(g, caption, sizePressed, BarLinkState.Pressed);
					sizeDisabled = CalcTextSizeByAppearance(g, caption, sizeDisabled, BarLinkState.Disabled);
				}
				size.Width = Math.Max(sizeNormal.Width, Math.Max(sizeHot.Width, Math.Max(sizePressed.Width, sizeDisabled.Width)));
				size.Height = Math.Max(sizeNormal.Height, Math.Max(sizeHot.Height, Math.Max(sizePressed.Height, sizeDisabled.Height)));
				if(Link.MaxLinkTextWidth > 0) size.Width = Math.Min(size.Width, Link.MaxLinkTextWidth);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return size;
		}
		protected virtual Size CalcTextSizeByAppearance(Graphics g, string caption, Size size, BarLinkState state) {
			Font fnt = GetFontByState(state);
			if(!Font.Equals(fnt)) {
				return PaintHelper.CalcTextSize(g, caption, fnt, 0, LinkCaptionStringFormat, Link).ToSize();
			}
			return size;
		}
		protected virtual Size CalcHtmlTextSizeByAppearance(Graphics g, Size size, BarLinkState state) {
			AppearanceObject obj = GetOwnerAppearanceByState(state);
			if(obj.Options.UseFont && !OwnerAppearances.Normal.Font.Equals(obj.Font)) {
				size = StringPainter.Default.Calculate(g, obj, Link.Caption, 0).Bounds.Size;
				size.Height--;
			}
			return size;
		}
		protected virtual Size CalcCaptionSize(Graphics g) {
			string s = DisplayCaption;
			if(s == null || s.Length == 0) s = "Wg";
			return CalcCaptionSize(g, s);
		}
		protected virtual int CalcDescriptionHeight(Graphics g) {
			const string str = "Wg\xd\xaWg";
			int height = 0;
			if(Link.IsAllowHtmlText)
				height = StringPainter.Default.Calculate(g, OwnerAppearance, str, 0).Bounds.Height;
			else
				height = (int)AppearanceMenuItemDescription.CalcTextSize(g, str, 0).Height;
			return height + 1;
		}
		protected virtual Size RaiseGlyphSizeEvent(Size size) {
			if(GlyphSizeEvent != null) {
				BarLinkGetValueEventArgs e = new BarLinkGetValueEventArgs(this, size);
				GlyphSizeEvent(this, e);
				size = (Size)e.Value;
			}
			return size;
		}
		protected internal virtual MenuHeaderPainter GetMenuHeaderPainter() {
			return Manager.PPainter.DefaultMenuHeaderPainter;
		}
		internal bool SkipRightIndentInMenu { get; set; }
		protected internal Rectangle RadialMenuCaptionRect {
			get { return Rectangle.Inflate(CaptionRect, 1, 1); }
		}
		protected internal Rectangle RadialMenuGlyphRect {
			get {
				Rectangle r = Rects[BarLinkParts.Glyph];
				if(r.IsEmpty) return Rectangle.Empty;
				return Rectangle.Inflate(r, 1, 1);
			}
		}
		protected virtual int GetScaledWidth(int width) {
			if(!Manager.GetScaleEditors()) return width;
			float fDpi = BarUtilites.GetScreenDPI();
			return (int)(width * fDpi / 96.0);
		}
		public bool AllowDrawLinkSeparator { 
			get {
				CustomSubMenuBarControlViewInfo vi = BarControlInfo as CustomSubMenuBarControlViewInfo;
				return vi == null || !vi.GetLinkMultiColumn(Link);
			} 
		}
	}
	public class BarCloseLinkViewInfo : BarButtonLinkViewInfo {
		public BarCloseLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		protected internal override int CalcLinkIndent(BarIndent linkIndent) {
			return 0;
		}
	}
	public class BarBaseButtonLinkViewInfo : BarLinkViewInfo {
		public BarBaseButtonLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
			this.allowDrawCheckInMenu = true;
		}
		bool allowDrawCheckInMenu;
		public bool AllowDrawCheckInMenu { get { return allowDrawCheckInMenu; } set { allowDrawCheckInMenu = value; } }
		public new virtual BarBaseButtonItemLink Link { get { return base.Link as BarBaseButtonItemLink; } }
		protected override BarLinkState CalcLinkState() {
			BarLinkState state = base.CalcLinkState();
			if(Link.Item.Down) state |= BarLinkState.Checked;
			if(!IsLinkInMenu && (state & BarLinkState.Checked) != 0) return BarLinkState.Checked | state;
			if(IsDrawPart(BarLinkParts.OpenArrow) && (state & BarLinkState.Pressed) != 0) return BarLinkState.Pressed;
			return state;
		}
		public override BarLinkState CalcRealPaintState() {
			if((LinkState & BarLinkState.Checked) != 0) {
				if(IsLinkInMenu) {
					return LinkState & (~BarLinkState.Checked);
				}
				if((LinkState & BarLinkState.Highlighted) != 0) return BarLinkState.Pressed;
				return BarLinkState.Checked;
			}
			return base.CalcRealPaintState();
		}
	}
	public class BarToggleSwitchLinkViewInfo : BarLinkViewInfo {
		public BarToggleSwitchLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
			this.link = link;
		}
		BarItemLink link;
		public BarToggleSwitchItem Item { get { return (BarToggleSwitchItem)link.Item; } }
		public new BarToggleSwitchItemLink Link { get { return (BarToggleSwitchItemLink)link; } }
		public Rectangle ToggleBounds { get { return CalcToggleBounds(); } }
		public Rectangle ToggleContentBounds { get { return CalcToggleContentBounds(); } }
		protected virtual int ToggleHeight { get { return 20; } }
		protected virtual int ToggleWidth { get { return 66; } }
		protected virtual int Indent { get { return 2; } }
		public override Size CalcLinkSize(Graphics g, object sourceObject) {
			Size baseSize = base.CalcLinkSize(g, sourceObject);
			return new Size(baseSize.Width + ToggleWidth, Math.Max(baseSize.Height, ToggleHeight + 2 * Indent));
		}
		protected virtual Rectangle CalcToggleBounds(){
			Size size = new Size(ToggleWidth, ToggleHeight);
			Point location = new Point(Bounds.Right - ToggleWidth - Indent, Bounds.Y + (Bounds.Height - ToggleHeight) / 2);
			return new Rectangle(location, size);
		}
		protected virtual Rectangle CalcToggleContentBounds() {
			int width = EditorsSkins.GetSkin(Manager.GetController().LookAndFeel.ActiveLookAndFeel)[EditorsSkins.SkinToggleSwitch].Properties.GetInteger("SwitchWidth");
			Size toggleSize = new Size(width * ToggleBounds.Width / 100, ToggleBounds.Height);
			int x = Item.Checked ? ToggleBounds.Right - toggleSize.Width : ToggleBounds.X;
			return new Rectangle(new Point(x, ToggleBounds.Y), toggleSize);
		}
		public override Brush GetForeBrush(GraphicsCache cache, BarLinkState state) {
			return new SolidBrush(BarControlInfo == null ? DrawParameters.Colors[BarColor.LinkForeColor] : BarControlInfo.Appearance.GetAppearance(LinkState).ForeColor);
		}
		protected internal override void UpdateOwnerAppearance() {
			base.UpdateOwnerAppearance();
		}
	}
	public class BarCheckButtonLinkViewInfo : BarBaseButtonLinkViewInfo {
		public BarCheckButtonLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
			this.checkPainter = CreateCheckPainter();
		}
		protected override BarLinkState GetStateForAppearance() {
			if(GetCheckBoxVisibility() != CheckBoxVisibility.None)
				return BarLinkState.Normal;
			return base.GetStateForAppearance();
		}
		protected override Image GetGlyph() {
			Image res = base.GetGlyph();
			if(res != null)
				return res;
			if(IsLinkInMenu && IsDrawPart(BarLinkParts.Glyph))
				return PaintStyle.GetCheckGlyph();
			return null;
		}
		public override Brush GetForeBrush(GraphicsCache cache, BarLinkState state) {
			if(GetCheckBoxVisibility() != CheckBoxVisibility.None) {
				state = BarLinkState.Normal;
			}
			return base.GetForeBrush(cache, state);
		}
		public override Size GlyphSize {
			get {
				return base.GlyphSize;
			}
		}
		protected override bool AllowGetForeBrushInOwnerAppearance() {
			if(GetCheckBoxVisibility() != CheckBoxVisibility.None && IsLinkInMainMenu) {
				return false;
			}
			return base.AllowGetForeBrushInOwnerAppearance();
		}
		public override BarLinkState CalcRealPaintState() {
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return base.CalcRealPaintState();
			if((LinkState & BarLinkState.Checked) != 0) {
				if(IsLinkInMenu) {
					return LinkState & (~BarLinkState.Checked);
				}
				if((LinkState & BarLinkState.Pressed) != 0) return BarLinkState.Pressed;
				if((LinkState & BarLinkState.Highlighted) != 0) return BarLinkState.Highlighted;
				return BarLinkState.Checked;
			}
			return LinkState;
		}
		public virtual CheckBoxVisibility GetCheckBoxVisibility() {
			var checkItem = Link.Item as BarCheckItem;
			if(checkItem == null) return CheckBoxVisibility.None;
			return checkItem.CheckBoxVisibility;
		}
		BaseCheckObjectPainter checkPainter;
		public BaseCheckObjectPainter CheckPainter { get { return checkPainter; } }
		protected virtual BaseCheckObjectPainter CreateCheckPainter() {
			var lookAndFeel = Manager.GetController().LookAndFeel.ActiveLookAndFeel;
			return CheckPainterHelper.GetPainter(lookAndFeel);
		}
		protected override void UpdatePainters() {
			this.checkPainter = CreateCheckPainter();
			base.UpdatePainters();
		}
		protected virtual Size GetCheckSize() {
			return DrawParameters.GetCheckSize();
		}
		public override void UpdateLinkInfo(object sourceObject) {
			base.UpdateLinkInfo(sourceObject);
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return;
			fDrawParts |= BarLinkParts.Checkbox;
		}
		protected override void CalcVerticalCaptionGlyphInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcVerticalCaptionGlyphInfo(g, sourceObject, r);
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return;
			CalcVerticalCheckBoxInfoAndOffsetCaptionGlyph(r);
		}
		private void CalcVerticalCheckBoxInfoAndOffsetCaptionGlyph(Rectangle r) {
			if(!IsDrawPart(BarLinkParts.Checkbox)) return;
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return;
			int leftIndent = CalcLinkIndent(BarIndent.Left),
				rightIndent = CalcLinkIndent(BarIndent.Right);
			int checkIndent = (GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText) ? leftIndent : rightIndent;
			Size checkSize = GetCheckSize();
			int offset = (GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText) ? (checkSize.Height + checkIndent) : 0;
			if(offset != 0) {
				var glyph = Rects[BarLinkParts.Glyph];
				glyph.Offset(0, offset);
				Rects[BarLinkParts.Glyph] = glyph;
				var caption = Rects[BarLinkParts.Caption];
				caption.Offset(0, offset);
				Rects[BarLinkParts.Caption] = caption;
			}
			Rectangle tr = Rectangle.Empty;
			tr.Size = checkSize;
			if(GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText)
				tr.Y = r.Y + leftIndent;
			else
				tr.Y = r.Bottom - (checkSize.Height + rightIndent);
			tr.X = r.X + (r.Width - tr.Size.Width) / 2;
			Rects[BarLinkParts.Checkbox] = tr;
		}
		protected override void CalcHorizontalCaptionGlyphInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcHorizontalCaptionGlyphInfo(g, sourceObject, r);
			if(!IsDefaultCaptionAlignment || GetCheckBoxVisibility() == CheckBoxVisibility.None) return;
			CalcHorizontalCheckBoxInfoAndOffsetCaptionGlyph(r);
		}
		protected override void CalcAlignedCaptionGlyphInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcAlignedCaptionGlyphInfo(g, sourceObject, r);
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return;
			if(IsHCaptionAlignment) CalcHorizontalCheckBoxInfoAndOffsetCaptionGlyph(r);
		}
		private void CalcHorizontalCheckBoxInfoAndOffsetCaptionGlyph(Rectangle r) {
			if(!IsDrawPart(BarLinkParts.Checkbox)) return;
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return;
			int leftIndent = CalcLinkIndent(BarIndent.Left),
				rightIndent = CalcLinkIndent(BarIndent.Right);
			int checkIndent = (GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText) ? CalcLinkIndent(BarIndent.Left) : CalcLinkIndent(BarIndent.Right);
			Size checkSize = GetCheckSize();
			int offset = (GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText) ? (checkSize.Width + checkIndent) : 0;
			if(offset != 0) {
				var glyph = Rects[BarLinkParts.Glyph];
				glyph.Offset(offset, 0);
				Rects[BarLinkParts.Glyph] = glyph;
				var caption = Rects[BarLinkParts.Caption];
				caption.Offset(offset, 0);
				Rects[BarLinkParts.Caption] = caption;
			}
			Rectangle tr = Rectangle.Empty;
			tr.Size = checkSize;
			if(GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText)
				tr.X = r.X + leftIndent;
			else
				tr.X = r.Right - (checkSize.Width + rightIndent);
			tr.Y = r.Y + (r.Height - tr.Size.Height) / 2;
			Rects[BarLinkParts.Checkbox] = tr;
		}
		protected override Size CalcLinkHorizontalSize(Graphics g, object sourceObject) {
			var res = base.CalcLinkHorizontalSize(g, sourceObject);
			if(!IsDrawPart(BarLinkParts.Checkbox))  return res;
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return res;
			int checkIndent = (GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText) ? CalcLinkIndent(BarIndent.Left) : CalcLinkIndent(BarIndent.Right);
			Size checkSize = GetCheckSize();
			res.Width += checkSize.Width + checkIndent;
			return res;
		}
		protected override Size CalcLinkVerticalSize(Graphics g, object sourceObject) {
			var res = base.CalcLinkVerticalSize(g, sourceObject);
			if(!IsDrawPart(BarLinkParts.Checkbox)) return res;
			if(GetCheckBoxVisibility() == CheckBoxVisibility.None) return res;
			int checkIndent = (GetCheckBoxVisibility() == CheckBoxVisibility.BeforeText) ? CalcLinkIndent(BarIndent.Top) : CalcLinkIndent(BarIndent.Bottom);
			Size checkSize = GetCheckSize();
			res.Height += checkSize.Height + checkIndent;
			return res;
		}
	}
	public class BarDesignTimeLinkViewInfo : BarButtonLinkViewInfo {
		public BarDesignTimeLinkViewInfo(BarDrawParameters parameters, BarItemLink link)
			: base(parameters, link) {
		}
		protected override Size CalcLinkHorizontalSize(Graphics g, object sourceObject) {
			Size res = base.CalcLinkHorizontalSize(g, sourceObject);
			ISkinProviderEx skinProvider = PaintStyle as ISkinProviderEx;
			if(skinProvider != null && skinProvider.GetTouchUI())
				return res;
			res.Height = BarEmptyLinkViewInfo.CalcEmptyItemSize(Link.Bar).Height;
			return res;
		}
		protected override Size CalcLinkVerticalSize(Graphics g, object sourceObject) {
			Size res = base.CalcLinkVerticalSize(g, sourceObject);
			res.Width = BarEmptyLinkViewInfo.CalcEmptyItemSize(Link.Bar).Height;
			return res;
		}
		protected override BarLinkState CalcLinkState() { return BarLinkState.Normal; }
	}
	public class BarInMdiChildrenListButtonLinkViewInfo : BarButtonLinkViewInfo {
		public BarInMdiChildrenListButtonLinkViewInfo(BarDrawParameters parameters, BarItemLink link)
			: base(parameters, link) {
		}
		protected internal override Size LinkGlyphSize {
			get {
				return new Size(16, 16);
			}
		}
		protected internal override Size ImageUriImageSize { get { return new Size(16, 16); } }
	}
	public class BarButtonLinkViewInfo : BarBaseButtonLinkViewInfo {
		public BarButtonLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		protected override bool AllowDrawArrow { get { return Link.Item.AllowDrawArrow; } }
		protected override bool ActAsDropDown { get { return Link.Item.ActAsDropDown; } }
		protected override BarLinkState CalcLinkState() {
			BarLinkState state = base.CalcLinkState();
			if(Link.Opened) {
				BarItemLink hLink = Link.Manager.SelectionInfo.HighlightedLink;
				if(Manager.IsCustomizing && Link == Manager.SelectionInfo.CustomizeSelectedLink) return BarLinkState.Normal;
				if(hLink != null && hLink != Link) {
					if(Link.Manager.SelectionInfo.HighlightedLink.LiveLinkLevel <= Link.LiveLinkLevel) return BarLinkState.Normal;
				}
				return BarLinkState.Highlighted;
			}
			if(IsDrawPart(BarLinkParts.OpenArrow) && (state & BarLinkState.Pressed) != 0) return BarLinkState.Pressed;
			return state;
		}
		public new virtual BarButtonItemLink Link { get { return base.Link as BarButtonItemLink; } }
		public override bool IsDrawAnyGlyph { get { return base.Link.IsImageExist || ((Link.Item.IsCheckButtonStyle) && Link.Item.Down); }	}
		public virtual bool IsNeedOpenArrow {
			get {
				if(!Link.Item.IsDropDownButtonStyle)
					return false;
				if(!Link.Item.ActAsDropDown)
					return true;
				return IsLinkInMenu? Link.Item.AllowDrawArrowInMenu: Link.Item.AllowDrawArrow;
			}
		}
		public override void UpdateLinkInfo(object sourceObject) {
			base.UpdateLinkInfo(sourceObject);
			if(IsNeedOpenArrow) this.fDrawParts |= BarLinkParts.OpenArrow;
		}
		protected override Size CalcLinkHorizontalSize(Graphics g, object sourceObject) {
			Size size = base.CalcLinkHorizontalSize(g, sourceObject);
			if(IsDrawPart(BarLinkParts.OpenArrow)) {
				int width = DrawParameters.Constants.GetBarButtonArrowWidth();
				if(Link.Item.ActAsDropDown && Link.Item.AllowDrawArrow) 
					width = DrawParameters.Constants.GetBarMenuButtonArrowWidth() + DrawParameters.Constants.BarMenuButtonArrowTextIndent;
				size.Width += width;
			}
			return size;
		}
		protected override Size CalcLinkVerticalSize(Graphics g, object sourceObject) {
			Size size = base.CalcLinkVerticalSize(g, sourceObject);
			if(IsDrawPart(BarLinkParts.OpenArrow)) {
				size.Width += DrawParameters.Constants.GetBarButtonArrowWidth();
			}
			return size;
		}
		protected override void CalcVerticalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcVerticalViewInfo(g, sourceObject, r);
			if(!IsDrawPart(BarLinkParts.OpenArrow)) return;
			Rectangle ar = Rects[BarLinkParts.Select];
			if(IsDrawPart(BarLinkParts.OpenArrow)) {
				ar.X = ar.Right - DrawParameters.Constants.GetBarButtonArrowWidth();
				ar.Width = DrawParameters.Constants.GetBarButtonArrowWidth();
				if(IsDefaultCaptionAlignment) {
					Rectangle cr = CaptionRect;
					cr.Width -= ar.Width;
					CaptionRect = cr;
				}
			}
			Rects[BarLinkParts.OpenArrow] = ar;
		}
		protected override void CalcHorizontalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcHorizontalViewInfo(g, sourceObject, r);
			if(!IsDrawPart(BarLinkParts.OpenArrow)) return;
			Rectangle ar = Rects[BarLinkParts.Select];
			int width = DrawParameters.Constants.GetBarButtonArrowWidth();
			if(Link.Item.ActAsDropDown && Link.Item.AllowDrawArrow)
				width = DrawParameters.Constants.GetBarMenuButtonArrowWidth() * 2;
			ar.X = ar.Right - width;
			ar.Width = width;
			Rects[BarLinkParts.OpenArrow] = ar;
		}
		public override void UpdateLinkWidthInSubMenu(int linkWidth) {
			int delta = linkWidth - Bounds.Width;
			base.UpdateLinkWidthInSubMenu(linkWidth);
			Rectangle r = Rects[BarLinkParts.OpenArrow];
			if(r.IsEmpty) return;
			Rects.Offset(BarLinkParts.OpenArrow, delta, 0); 
		}
	}
	public class BarLargeButtonLinkViewInfo : BarButtonLinkViewInfo {
		public BarLargeButtonLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		public override Size CalcLinkSize(Graphics g, object sourceObject) {
			Size res = base.CalcLinkSize(g, sourceObject);
			res.Width = Math.Max(res.Width, Link.Item.MinSize.Width);
			res.Height = Math.Max(res.Height, Link.Item.MinSize.Height);
			return res;
		}
		public new virtual BarLargeButtonItemLink Link { get { return base.Link as BarLargeButtonItemLink; } }
		public override BarItemCaptionAlignment CaptionAlignment { get { return Link.Item.CaptionAlignment; } }
		protected override void CalcHorizontalCaptionGlyphInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcHorizontalCaptionGlyphInfo(g, sourceObject, r);
		}
		protected override void CalcLinkLargeImage(BarLinkState state, ref int index, ref Image image) {
			switch(state) {
				case BarLinkState.Pressed: 
				case BarLinkState.Highlighted: 
					image = Link.Item.LargeGlyphHot; 
					index = Link.Item.LargeImageIndexHot;
					break;
				case BarLinkState.Disabled: 
					image = Link.Item.LargeGlyphDisabled; 
					index = Link.Item.LargeImageIndexDisabled;
					break;
				default:
					image = Link.GetLargeGlyph();
					index = Link.Item.LargeImageIndex;
					break;
			}
		}
		public override bool IsShouldUseLargeImages { get { return base.IsShouldUseLargeImages || (Link.IsLargeImageExist && !IsLinkInMenu && IsDrawPart(BarLinkParts.Glyph)); } }
	}
	public class BarCustomContainerLinkViewInfo : BarLinkViewInfo {
		public BarCustomContainerLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		public override bool CanAnimate {
			get {
				if(!base.CanAnimate) return false;
				if(IsLinkInMenu) return true;
				if(Bar == null) return true;
				return Manager.SelectionInfo.AutoOpenMenuBar != Bar;
			}
		}
		public new virtual BarCustomContainerItemLink Link { get { return base.Link as BarCustomContainerItemLink; } }
		protected virtual bool IsNeedOpenArrow { get { return IsLinkInMenu || (Link.Bar != null && !Link.Bar.IsMainMenu); } }
		public override void UpdateLinkInfo(object sourceObject) {
			base.UpdateLinkInfo(sourceObject);
			if(IsNeedOpenArrow) this.fDrawParts |= BarLinkParts.OpenArrow;
		}
		protected override bool ActAsDropDown { get { return IsNeedOpenArrow; } }
		protected override bool AllowDrawArrow { get { return IsNeedOpenArrow; } }
		protected override BarLinkState CalcLinkState() {
			BarLinkState res = BarLinkState.Normal;
			if(!Link.Enabled)
				return BarLinkState.Disabled;
			if(Link.Opened) {
				BarItemLink hLink = Link.Manager.SelectionInfo.HighlightedLink;
				if(Manager.IsCustomizing && Link == Manager.SelectionInfo.CustomizeSelectedLink) return BarLinkState.Normal;
				if(hLink == null && Link.IsLinkInMenu) {
					Point pt = Link.BarControl.PointToClient(Control.MousePosition);
					CustomSubMenuBarControlViewInfo menuViewInfo = Link.BarControl.ViewInfo as CustomSubMenuBarControlViewInfo;
					if(menuViewInfo != null && menuViewInfo.LinksBounds.Contains(pt))
						return BarLinkState.Normal;
				}
				if(hLink != null && hLink != Link) {
					if(Link.Manager.SelectionInfo.HighlightedLink.LiveLinkLevel <= Link.LiveLinkLevel) return BarLinkState.Normal;
				}
				if(!Link.IsLinkInMenu)
					res = BarLinkState.Pressed;
				else 
				res = BarLinkState.Highlighted;
				return res;
			}
			if(!Link.Manager.IsCustomizing && CanDrawAs(BarLinkState.Highlighted)) {
				if((Manager.SelectionInfo.HighlightedLink == Link &&
					(Manager.SelectionInfo.PressedLink == null || Manager.SelectionInfo.PressedLink == Link || IsLinkInMenu) &&
					(Manager.SelectionInfo.EditingLink == null) || Manager.SelectionInfo.AllowPaintHighlightWhenEditing(Link))) {
					res |= BarLinkState.Highlighted;
				}
			}
			if(CanDrawAs(BarLinkState.Pressed) && !IsLinkInMenu && Link == Manager.SelectionInfo.PressedLink) {
				res = BarLinkState.Highlighted;
			}
			return res;
		}
		protected override void CalcVerticalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcVerticalViewInfo(g, sourceObject, r);
			if(!IsDrawPart(BarLinkParts.OpenArrow)) return;
			Rectangle ar = Rects[BarLinkParts.Select];
			if(IsDrawPart(BarLinkParts.OpenArrow)) {
				if(IsDrawVerticalRotated) {
					ar.Y = ar.Bottom - DrawParameters.Constants.GetBarMenuButtonArrowWidth() * 2;
					ar.Height = DrawParameters.Constants.GetBarMenuButtonArrowWidth() * 2;
				}
				else {
					ar.X = ar.Right - (DrawParameters.Constants.GetBarMenuButtonArrowWidth() * 2);
					ar.Width = (DrawParameters.Constants.GetBarMenuButtonArrowWidth() * 2);
				}
			}
			Rects[BarLinkParts.OpenArrow] = ar;
		}
		protected override void CalcHorizontalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcHorizontalViewInfo(g, sourceObject, r);
			if(!IsDrawPart(BarLinkParts.OpenArrow)) return;
			Rectangle ar = Rects[BarLinkParts.Select];
			ar.X = ar.Right - (DrawParameters.Constants.GetBarMenuButtonArrowWidth() * 2);
			ar.Width = (DrawParameters.Constants.GetBarMenuButtonArrowWidth() * 2);
			Rects[BarLinkParts.OpenArrow] = ar;
		}
		public override void UpdateLinkWidthInSubMenu(int linkWidth) {
			int delta = linkWidth - Bounds.Width;
			base.UpdateLinkWidthInSubMenu(linkWidth);
			Rectangle r = Rects[BarLinkParts.OpenArrow];
			if(r.IsEmpty) return;
			Rects.Offset(BarLinkParts.OpenArrow, delta, 0); 
		}
		protected override Size CalcLinkHorizontalSize(Graphics g, object sourceObject) {
			Size size = base.CalcLinkHorizontalSize(g, sourceObject);
			if(IsDrawPart(BarLinkParts.OpenArrow)) {
				size.Width += DrawParameters.Constants.GetBarMenuButtonArrowWidth() + DrawParameters.Constants.BarMenuButtonArrowTextIndent;
			}
			return size;
		}
		protected override Size CalcLinkVerticalSize(Graphics g, object sourceObject) {
			Size size = base.CalcLinkVerticalSize(g, sourceObject);
			if(IsDrawPart(BarLinkParts.OpenArrow)) {
				if(IsDrawVerticalRotated)
					size.Height += DrawParameters.Constants.GetBarMenuButtonArrowWidth() + DrawParameters.Constants.BarMenuButtonArrowTextIndent;
				else
					size.Width += DrawParameters.Constants.GetBarMenuButtonArrowWidth() + DrawParameters.Constants.BarMenuButtonArrowTextIndent;
			}
			return size;
		}
	}
	public class BarListLinkViewInfo : BarCustomContainerLinkViewInfo {
		public BarListLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		protected override bool IsNeedOpenArrow { get { return Link.Bar != null && !Link.Bar.IsMainMenu; } }
	}
	public class BarSubItemLinkViewInfo : BarLinkContainerLinkViewInfo {
		public BarSubItemLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		protected override bool IsNeedOpenArrow {
			get {
				BarSubItem subItem = Link.Item as BarSubItem;
				if (subItem != null)
					return subItem.IsNeedOpenArrow && base.IsNeedOpenArrow;
				return base.IsNeedOpenArrow;
			}			
		}		
	}
	public class BarQMenuAddRemoveButtonsLinkViewInfo : BarCustomContainerLinkViewInfo {
		public BarQMenuAddRemoveButtonsLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		protected override bool IsNeedOpenArrow { get { return true; } }
		public override Size GlyphSize {
			get { return Size.Empty; }
		}
		protected internal override int CalcLinkIndent(BarIndent indent) {
			if(indent == BarIndent.Left) return 0;
			return base.CalcLinkIndent(indent);
		}
	}
	public class BarLinkContainerLinkViewInfo : BarCustomContainerLinkViewInfo {
		public BarLinkContainerLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
	}
	public class BarHeaderLinkViewInfo : BarStaticLinkViewInfo  {
		public BarHeaderLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		public new virtual BarHeaderItemLink Link { get { return base.Link as BarHeaderItemLink; } }
		protected internal override MenuHeaderPainter GetMenuHeaderPainter() {
			return Manager.PaintStyle.PrimitivesPainter.DefaultMenuHeaderPainter;
		}
		protected internal override StateAppearances UpdateOwnerAppearancesCore() {
			StateAppearances obj = new StateAppearances();
			AppearanceHelper.Combine(obj.Normal, new AppearanceObject[] { AppearanceMenu.HeaderItemAppearance });
			AppearanceHelper.Combine(obj.Hovered, new AppearanceObject[] { AppearanceMenu.HeaderItemAppearance });
			AppearanceHelper.Combine(obj.Pressed, new AppearanceObject[] { AppearanceMenu.HeaderItemAppearance });
			AppearanceHelper.Combine(obj.Disabled, new AppearanceObject[] { AppearanceMenu.HeaderItemAppearance });
			return obj;
		}
		protected override BarLinkState CalcLinkState() {
			return BarLinkState.Normal;
		}
		protected virtual int HeaderTextHorizontalIndent { 
			get { return CalcLinkIndent(BarIndent.Left); } 
		}
		protected override void CalcInMenuViewInfoCore(Graphics g, object sourceObject, Rectangle r) {
			Rects[BarLinkParts.Select] = r;
			int maxTextWidth = r.Width;
			Rectangle tr = new Rectangle(Point.Empty, CalcCaptionSize(g));
			tr.Location = new Point(r.X + HeaderTextHorizontalIndent, r.Y + (r.Height - tr.Height) / 2);
			tr.Width = CalcCaptionSize(g).Width + 1; 
			Rects[BarLinkParts.Caption] = tr;
			if (Link.IsAllowHtmlText) CalcHtmlStringInfo(g);
		}
		protected override BorderStyles GetBorder() {
			return BorderStyles.Default;
		}
		protected override int CalcLinkInMenuHeight(Graphics g, object sourceObject) {
			return CalcLincInMenuHeightCore(g);
		}
		protected override int CalcLinkInMenuHeightLarge(Graphics g, object sourceObject) {
			return CalcLincInMenuHeightCore(g);
		}
		protected override int CalcLinkInMenuHeightLargeDescription(Graphics g, object sourceObject) {
			return CalcLincInMenuHeightCore(g);
		}
		private int CalcLincInMenuHeightCore(Graphics g) {
			int res = 0;
			if (Link.IsImageExist)
				res = GlyphSize.Height;
			if (IsDrawPart(BarLinkParts.Caption)) {
				res = Math.Max(CalcCaptionSize(g).Height + 1, res);
			}
			if (res < 4) res = 8;
			res = Math.Max(CalcLinkContentsHeight(g), res);
			res += CalcLinkIndent(BarIndent.Top) + CalcLinkIndent(BarIndent.Bottom);
			return GetMenuHeaderPainter().CalcElementHeight(g, this, CalcCaptionSize(g), res);
		}
		protected override bool AllowGlyph { get { return false; } }
	}
	public class BarStaticLinkViewInfo : BarLinkViewInfo {
		public BarStaticLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		public override bool CanAnimate { get { return false; } }
		public new virtual BarStaticItemLink Link { get { return base.Link as BarStaticItemLink; } }
		protected override BorderStyles GetBorder() {
			if(Bar != null && Bar.IsMainMenu && Link.Item.Border == BorderStyles.Default) return BorderStyles.NoBorder;
			return base.GetBorder();
		}
		public override void UpdateLinkInfo(object sourceObject) {
			if(this.fLinkCaptionStringFormat != null) this.fLinkCaptionStringFormat.Dispose();
			this.fLinkCaptionStringFormat = null;
			base.UpdateLinkInfo(sourceObject);
			if(((BarStaticItem)Link.Item).TextAlignment != StringAlignment.Near) {
				if(this.fLinkCaptionStringFormat == null)
					this.fLinkCaptionStringFormat = (StringFormat)LinkCaptionStringFormat.Clone();
				this.fLinkCaptionStringFormat.Alignment = ((BarStaticItem)Link.Item).TextAlignment;
			}
		}
		public override bool CanDrawAs(BarLinkState state) {
			switch(state) {
				case BarLinkState.Highlighted : return false;
				case BarLinkState.Pressed : return false;
			}
			return base.CanDrawAs(state);
		}
		protected internal override Size CalcRealCaptionSize(Graphics g) {
			Size size = CalcCaptionSize(g);
			if((DisplayCaption == null || DisplayCaption.Length == 0) && Link.Item.AutoSize != BarStaticItemSize.None)
				size.Width = 0;
			return size;
		}
		protected override Size CalcLinkHorizontalSize(Graphics g, object sourceObject) {
			Size size = base.CalcLinkHorizontalSize(g, sourceObject);
			size.Width += Link.Item.LeftIndent + Link.Item.RightIndent;
			if(IsSpringLink && SpringLink.SpringWidth > 0) size.Width = SpringLink.SpringWidth; 
			return size;
		}
		protected override void CalcHorizontalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			r.X += Link.Item.LeftIndent;
			r.Width -= (Link.Item.LeftIndent + Link.Item.RightIndent);
			Rects[BarLinkParts.Bounds] = r;
			base.CalcHorizontalViewInfo(g, sourceObject, r);
		}
		protected override Size CalcLinkVerticalSize(Graphics g, object sourceObject) {
			Size size = base.CalcLinkVerticalSize(g, sourceObject);
			if(IsDrawVerticalRotated) {
				size.Height += Link.Item.LeftIndent + Link.Item.RightIndent;
				if(IsSpringLink && SpringLink.SpringWidth > 0) size.Height = SpringLink.SpringWidth; 
			} else {
				size.Width += Link.Item.LeftIndent + Link.Item.RightIndent;
			}
			return size;
		}
		protected override void CalcVerticalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			if(IsDrawVerticalRotated) {
				r.Y += Link.Item.LeftIndent;
				r.Height -= (Link.Item.LeftIndent + Link.Item.RightIndent);
			} else {
				r.X += Link.Item.LeftIndent;
				r.Width -= (Link.Item.LeftIndent + Link.Item.RightIndent);
			}
			Rects[BarLinkParts.Bounds] = r;
			base.CalcVerticalViewInfo(g, sourceObject, r);
		}
		protected int BaseCalcLinkIndent(BarIndent linkIndent) {
			return base.CalcLinkIndent(linkIndent);
		}
		protected internal override int CalcLinkIndent(BarIndent linkIndent) {
			bool isMainMenu = Bar != null && Bar.IsMainMenu;
			if(Manager.DrawParameters.PaintStyle is SkinBarManagerPaintStyle) {
				if(isMainMenu)
					return 0;
				return base.CalcLinkIndent(linkIndent);
			}
			if(IsDrawVerticalRotated) {
				switch(linkIndent) {
					case BarIndent.Left : return 0;
					case BarIndent.Right : return 1;
				}
			}
			else {
				switch(linkIndent) {
					case BarIndent.Top : return 0;
					case BarIndent.Bottom : return 1;
				}
			}
			return base.CalcLinkIndent(linkIndent);
		}
		protected override Size CalcCaptionSize(Graphics g) {
			BarStaticItem st = Link.Item as BarStaticItem;
			if(st.AutoSize == BarStaticItemSize.Spring) {
				if(BarControl == null || !BarControl.IsCanSpringLinks)
					return base.CalcCaptionSize(g);
			}																		   
			if(st.AutoSize == BarStaticItemSize.Content) return base.CalcCaptionSize(g);
			Size size = base.CalcCaptionSize(g);
			int w = Link.Width;
			if(IsSpringLink) {
				if(SpringLink.SpringWidth > 0) {
					w = SpringLink.SpringWidth - SpringLink.SpringTempWidth - SpringLink.SpringMinWidth;
				} else {
					w = SpringLink.SpringMinWidth;
				}
			}
			if(IsDrawVerticalRotated) 
				size.Height = w;
			else
				size.Width = w;
			return size;
		}
	}
	public class BarQBarCustomizationLinkViewInfo : BarCustomContainerLinkViewInfo { 
		bool drawExpandMark;
		public BarQBarCustomizationLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
			this.drawExpandMark = false;
		}
		protected override int CalcLinkHorizontalHeight(Graphics g, object sourceObject) {
			int res = base.CalcLinkHorizontalHeight(g, sourceObject);
			res = Math.Max(res, PaintStyle.CalcBarCustomizationLinkMinHeight(g, sourceObject, BarControlInfo.IsVertical, Bar.IsMainMenu));
			return res;
		}
		public override bool CanAnimate { get { return true; } }
		public virtual bool DrawExpandMark {
			get { return drawExpandMark; }
			set {
				drawExpandMark = value;
			}
		}
		public override void UpdateLinkInfo(object sourceObject) {
			base.UpdateLinkInfo(sourceObject);
			this.fDrawParts |= BarLinkParts.OpenArrow;
		}
		protected override void CalcHorizontalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcHorizontalViewInfo(g, sourceObject, r);
			Rectangle ar = Rects[BarLinkParts.Select];
			Rects[BarLinkParts.OpenArrow] = ar;
		}
		protected override void CalcVerticalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcVerticalViewInfo(g, sourceObject, r);
			Rectangle ar = Rects[BarLinkParts.Select];
			Rects[BarLinkParts.OpenArrow] = ar;
		}
		protected override Size CalcLinkHorizontalSize(Graphics g, object sourceObject) {
			Size size = base.CalcLinkHorizontalSize(g, sourceObject);
			size.Width = DrawParameters.Constants.BarQuickButtonWidth;
			return size;
		}
		protected override Size CalcLinkVerticalSize(Graphics g, object sourceObject) {
			Size size = new Size(4, 0);
			size.Height = DrawParameters.Constants.BarQuickButtonWidth;
			return size;
		}
	}
	public class BarEmptyLinkViewInfo : BarStaticLinkViewInfo {
		static bool IsBarCollapsed(Bar bar) {
			return bar != null && bar.OptionsBar.BarState == BarState.Collapsed;
		}
		public static Size CalcEmptyItemSize(Bar bar) {
			if(bar != null && bar.IsStatusBar) {
				if(IsBarCollapsed(bar)) return new Size(2, 18);
				return new Size(20, 18);
			}
			if(IsBarCollapsed(bar)) return new Size(2, 20);
			return new Size(20, 20);
		}
		public BarEmptyLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		protected internal override int CalcLinkIndent(BarIndent linkIndent) {
			bool isMainMenu = Bar != null && Bar.IsMainMenu;
			if(Manager.DrawParameters.PaintStyle is SkinBarManagerPaintStyle) {
				if(isMainMenu)
					return BaseCalcLinkIndent(linkIndent);
			}
			return base.CalcLinkIndent(linkIndent);
		}
		protected override Size CalcLinkHorizontalSize(Graphics g, object sourceObject) {
			return new Size(CalcEmptyItemSize(Link.Bar).Width, CalcLinkHeight(g, sourceObject));
		}
		protected override Size CalcLinkVerticalSize(Graphics g, object sourceObject) {
			return CalcEmptyItemSize(Link.Bar);
		}
		protected override BarLinkState CalcLinkState() { return BarLinkState.Normal; }
	}
	public class BarEditLinkViewInfo : BarLinkViewInfo {
		BaseEditViewInfo editViewInfo;
		Rectangle editRectangle;
		int editHeight;
		public BarEditLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
			this.editViewInfo = null;
			CreateEditViewInfo();
		}
		public override bool CanAnimate { get { return false; } }
		public new virtual BarEditItemLink Link { get { return base.Link as BarEditItemLink; } }
		protected override int GetLinkWidth() {
			return Link.Item.Size.Width;
		}
		protected override int CalcLinkHorizontalHeight(Graphics g, object sourceObject) {
			GetEditSize(g);
			return base.CalcLinkHorizontalHeight(g, sourceObject);
		}
		public virtual Rectangle EditRectangle { 
			get { return editRectangle; }
			set { editRectangle = value; }
		}
		public virtual BaseEditViewInfo EditViewInfo { get { return editViewInfo; } }
		public virtual int EditHeight { get { return editHeight; } }
		public override void UpdateLinkInfo(object sourceObject) {
			base.UpdateLinkInfo(sourceObject);
			this.fDrawParts = this.fDrawParts & (~BarLinkParts.Shortcut);
			if(Link.PaintStyle == BarItemPaintStyle.Standard) {
				this.fDrawParts = this.fDrawParts & (~ (BarLinkParts.Caption | BarLinkParts.Glyph));
				if(IsLinkInMenu) {
					this.fDrawParts |= BarLinkParts.Caption | BarLinkParts.Glyph;
				} else {
					if(IsDrawVerticalRotated) this.fDrawParts |= BarLinkParts.Glyph;
				}
			}
		}
		public override bool CanDrawAs(BarLinkState state) {
			switch(state) {
				case BarLinkState.Highlighted:
					if(Link != null && (Link.Holder is PopupMenu || Link.Holder is BarSubItem))
						return true;
					return false;
				case BarLinkState.Pressed: return false;
			}
			return base.CanDrawAs(state);
		}
		protected override bool CompareLinkState(BarLinkState newState, BarLinkState current) {
			bool res = base.CompareLinkState(newState, current);
			Point pos = MousePosition;
			if(EditViewInfo != null) {
				if(UpdateEditState())
					res = false;
				if(EditViewInfo.UpdateObjectState(Control.MouseButtons, pos)) {
					res = false;
				}
			}
			return res;
		}
		public override BarLinkState CalcRealPaintState() {
			BarLinkState res = base.CalcRealPaintState();
			if(!IsDrawVerticalRotated) {
				if(!Link.Enabled && res == BarLinkState.Highlighted) return res;
				if(IsLinkInMenu && res == BarLinkState.Highlighted) return BarLinkState.Highlighted;
				if(res == BarLinkState.Pressed || res == BarLinkState.Highlighted) return BarLinkState.Normal;
			}
			return res;
		}
		protected internal virtual bool ShouldEditorUseParentBackground { 
			get { 
				return Manager.TransparentEditors && Manager.GetController().LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin; 
			} 
		}
		protected virtual void CreateEditViewInfo() {
			if(Link == null || Link.Edit == null) return;
			this.editViewInfo = null;
			if(IsDrawVerticalRotated) return;
			this.editViewInfo = Link.Edit.CreateViewInfo();
			this.editViewInfo.InplaceType = InplaceType.Bars;
			this.Link.Edit.BeginUpdate();
			try {
			this.Link.Edit.UseParentBackground = ShouldEditorUseParentBackground;
			}
			finally {
				this.Link.Edit.CancelUpdate();
			}
			EditViewInfo.DefaultBorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Default;
			EditViewInfo.AllowOverridedState = true;
			EditViewInfo.OverridedState = Link.Enabled ? ObjectState.Normal : ObjectState.Disabled;
			EditViewInfo.DetailLevel = DevExpress.XtraEditors.Controls.DetailLevel.Full;
			EditViewInfo.AllowDrawFocusRect = Link.Edit.AllowFocused;
			EditViewInfo.RightToLeft = Manager.IsRightToLeft;
		}
		public override void UpdateLinkWidthInSubMenu(int linkWidth) {
			int delta = Link.IsAutoFillWidthInMenu ? linkWidth - Bounds.Width : 0;
			base.UpdateLinkWidthInSubMenu(linkWidth);
			if(EditViewInfo == null) return;
			Size size = GetEditSize(GInfo.Graphics);
			Rectangle r = Rects[BarLinkParts.Bounds];
			if(r.Height > size.Height) {
				r.Y += (r.Height - size.Height) / 2;
				r.Height = size.Height;
			}
			EditRectangle = r;
			this.editRectangle.Width = size.Width + delta;
			this.editRectangle.X = r.Right - CalcRightIndent() - EditRectangle.Width;
			UpdateViewInfo(BarManager.zeroPoint);
		}
		protected virtual int CalcSpringEditWidth() {
			int w = 0;
			if(IsSpringLink) {
				if(SpringLink.SpringWidth > 0) {
					w = SpringLink.SpringWidth - SpringLink.SpringTempWidth + SpringLink.SpringMinWidth;
				} else {
					w = SpringLink.SpringMinWidth;
				}
			}
			return w;
		}
		protected override void CalcHorizontalCaptionGlyphInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcHorizontalCaptionGlyphInfo(g, sourceObject, r);
			if(Link.Item.CaptionAlignment == HorzAlignment.Far) {
				int rightIndent = CalcRightIndent();
				int right = r.Right;
				if(!CaptionRect.IsEmpty) {
					right = r.Right - rightIndent - CaptionRect.Width;
					Rects[BarLinkParts.Caption] = new Rectangle(right, CaptionRect.Y, CaptionRect.Width, CaptionRect.Height);
					right -= DrawParameters.Constants.BarCaptionGlyphIndent;
				}
				if(!GlyphRect.IsEmpty) {
					Rects[BarLinkParts.Glyph] = new Rectangle(right - GlyphRect.Width, GlyphRect.Y, GlyphRect.Width, GlyphRect.Height);
				}
			}
		}
		protected override void CalcHorizontalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			EditRectangle = Rectangle.Empty;
			base.CalcHorizontalViewInfo(g, sourceObject, r);
			if(EditViewInfo == null) return;
			r = Rects[BarLinkParts.Select];
			UpdateEditBounds(g, r, false);
		}
		protected override void CalcVerticalViewInfo(Graphics g, object sourceObject, Rectangle r) {
			EditRectangle = Rectangle.Empty;
			base.CalcVerticalViewInfo(g, sourceObject, r);
			if(IsDrawVerticalRotated || EditViewInfo == null) return;
			UpdateEditBounds(g, r, true);
		}
		protected virtual void UpdateEditBounds(Graphics g, Rectangle r, bool isVertical) {
			EditRectangle = r;
			this.editRectangle.Size = GetEditSize(g);
			if(!isVertical && IsSpringLink) editRectangle.Width = CalcSpringEditWidth();
			int editMaxWidth = r.Width - CalcLinkIndent(BarIndent.Left) - CalcRightIndent();
			if(CaptionRect.Width > 0)
				editMaxWidth -= CaptionRect.Width + CalcEditorIndent();
			if(GlyphRect.Width > 0)
				editMaxWidth -= GlyphRect.Width + DrawParameters.Constants.BarCaptionGlyphIndent;
			if(!Link.IsEditWidthSet)
				editRectangle.Width = editMaxWidth;
			else
				editRectangle.Width = Math.Min(editRectangle.Width, editMaxWidth);
			if(r.Height < editRectangle.Height) editRectangle.Height = r.Height;
			if(Link.Item.CaptionAlignment != HorzAlignment.Far)
				this.editRectangle.X = r.Right - CalcRightIndent() - EditRectangle.Width;
			else
				this.editRectangle.X += CalcLinkIndent(BarIndent.Left);
			this.editRectangle.Y += (r.Height - EditRectangle.Height) / 2;
			UpdateEditBoundsCore();
		}
		bool lockUpdateActiveEditor = false;
		internal void LockUpdateActiveEditor() {
			this.lockUpdateActiveEditor = true;
		}
		internal void UnlockUpdateActiveEditor() {
			this.lockUpdateActiveEditor = false;
		}
		protected internal void UpdateEditBoundsCore() {
			if(Link.Item != null) {
				this.editRectangle.X += Link.Item.EditorPadding.Left;
				this.editRectangle.Width -= Link.Item.EditorPadding.Horizontal;
			}
			UpdateViewInfo(BarManager.zeroPoint);
			if(Link.EditorActive && !this.lockUpdateActiveEditor) {
				Manager.SelectionInfo.ActiveEditor.Bounds = EditRectangle;
			}
		}
		protected override int CalcLinkContentsHeight(Graphics g) {
			int res = EditHeight;
			if(!IsLinkInMenu) {
				res -= (CalcLinkIndent(BarIndent.Top) + CalcLinkIndent(BarIndent.Bottom));
			}
			return res;
		}
		protected override Size CalcLinkVerticalSize(Graphics g, object sourceObject) {
			if(IsDrawVerticalRotated)
				return base.CalcLinkVerticalSize(g, sourceObject);
			return base.CalcLinkHorizontalSize(g, sourceObject);
		}
		protected override Size CalcLinkHorizontalSize(Graphics g, object sourceObject) {
			Size size = base.CalcLinkHorizontalSize(g, sourceObject);
			if(!IsSpringLink || SpringLink.SpringWidth < 20) return size;
			size.Width = SpringLink.SpringWidth;
			return size;
		}
		protected virtual Size GetEditSize(Graphics g) {
			if(Link != null && Link.Edit != null && Link.Edit.IsDisposed) return Size.Empty;
			this.editHeight = 0;
			if(IsDrawVerticalRotated || EditViewInfo == null) return Size.Empty;
			g = GInfo.AddGraphics(g);
			Size size = Size.Empty;
			try {
				UpdateEditViewInfo();
				Size minSize = EditViewInfo.CalcBestFit(g);
				size.Height = minSize.Height;
				size.Height = Math.Max(size.Height, Link.Item.EditHeight);
				if(EditViewInfo is ToggleSwitchViewInfo) {
					size.Width = minSize.Width;
					size.Width = Math.Max(size.Width, Link.EditWidth);
				}
				else
					size.Width = PaintStyle.CalcEditLinkWidth(Link.EditWidth);
				if(IsInnerLink) size.Width = Math.Min(70, size.Width);
				editHeight = size.Height;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			if(EditViewInfo.AllowScaleWidth)
				size.Width = GetScaledWidth(size.Width);
			return size;
		}
		protected internal override int CalcLinkIndent(BarIndent linkIndent) {
			int res = base.CalcLinkIndent(linkIndent);
			if(IsDrawVerticalRotated || EditViewInfo == null) return res;
			Size editSize = Size.Empty;
			editSize = GetEditSize(GInfo.Graphics);
			if(IsSpringLink) 
				editSize.Width = CalcSpringEditWidth();
			if(linkIndent == BarIndent.Right) {
				return CalcRightIndent() + editSize.Width + CalcEditorIndent();
			}
			return res;
		}
		int CalcEditorIndent() {
			if((DrawParts & (BarLinkParts.Caption | BarLinkParts.Glyph)) != 0) return 4;
			return 0;
		}
		int CalcRightIndent() {
			if(IsLinkInMenu) return 3;
			return base.CalcLinkIndent(BarIndent.Right);
		}
		public virtual bool CalcAlwaysHotTrack(Point mousePosition) {
			return (!Link.Manager.IsCustomizing && Bounds.Contains(mousePosition) || Link.Manager.SelectionInfo.KeyboardHighlightedLink == Link);
		}
		public virtual bool UpdateEditState() {
			if(EditViewInfo == null) return false;
			ObjectState state = ObjectState.Normal;
			if(!DrawDisabled) {
				BarLinkState realState = LinkState;
				if(Manager.SelectionInfo.EditingLink == Link || realState == BarLinkState.Pressed || realState == BarLinkState.Highlighted)
					state = ObjectState.Hot;
				EditViewInfo.Focused = Manager.SelectionInfo.KeyboardHighlightedLink == Link;
			} else {
				state = ObjectState.Disabled;
			}
			if(state == EditViewInfo.OverridedState) return false;
			EditViewInfo.OverridedState = state;
			if(EditViewInfo.IsReady) EditViewInfo.CalcViewInfo(null);
			UpdateEditViewInfo();
			return true;
		}
		public override void UpdateViewInfo(Point mousePosition) {
			base.UpdateViewInfo(mousePosition);
			if(EditViewInfo == null) return;
			if(Link.Manager.IsCustomizing) mousePosition = BarManager.zeroPoint;
			EditViewInfo.EditValue = Link.Item.EditValue;
			UpdateEditState();
			UpdateEditViewInfo();
			EditViewInfo.Bounds = EditRectangle;
			EditViewInfo.CalcViewInfo(null, Control.MouseButtons, mousePosition, EditViewInfo.Bounds);
		}
		protected virtual void UpdateEditViewInfo() {
			if(EditViewInfo == null) return;
			EditViewInfo.UpdatePaintAppearance();
			if(EditViewInfo.LookAndFeel != Manager.PaintStyle.LinksLookAndFeel && EditViewInfo.LookAndFeel.UseDefaultLookAndFeel) {
				EditViewInfo.LookAndFeel = Manager.PaintStyle.LinksLookAndFeel;
			}
		}
	}
	public class BarRecentExpanderItemLinkViewInfo : BarLinkViewInfo { 
		public BarRecentExpanderItemLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		protected override int CalcLinkInMenuHeight(Graphics g, object sourceObject) {
			return DrawParameters.Constants.SubMenuRecentExpanderHeight;
		}
		public override void UpdateLinkInfo(object sourceObject) {
			base.UpdateLinkInfo(sourceObject);
			base.fDrawParts = BarLinkParts.Bounds;
		}
	}
	public class BarScrollItemLinkViewInfo : BarLinkViewInfo { 
		public BarScrollItemLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		public new BarScrollItemLink Link { get { return base.Link as BarScrollItemLink; } }
		public override void UpdateLinkInfo(object sourceObject) {
			base.UpdateLinkInfo(sourceObject);
			base.fDrawParts = BarLinkParts.Bounds;
		}
		protected override Size CalcCaptionSize(Graphics g) { return Size.Empty;  }
		protected override int CalcLinkInMenuHeight(Graphics g, object sourceObject) {
			return DrawParameters.Constants.GetSubMenuScrollLinkHeight(); 
		}
		protected override int CalcLinkInMenuHeightLargeDescription(Graphics g, object sourceObject) {
			return DrawParameters.Constants.GetSubMenuScrollLinkHeight(); 
		}
		protected override int CalcDescriptionHeight(Graphics g) { return 0; }
	}
	public class BarQMenuCustomizationLinkViewInfo : BarButtonLinkViewInfo { 
		BarLinkViewInfo innerViewInfo;
		int glyphWidth;
		public new BarQMenuCustomizationItemLink Link { get { return base.Link as BarQMenuCustomizationItemLink; } }
		public BarQMenuCustomizationLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
			this.innerViewInfo = null;
			this.AlwaysDrawAsInMenu = true;
			this.glyphWidth = base.GlyphSize.Width;
			if(Link.Item.ButtonStyle != BarButtonStyle.Check) this.glyphWidth = 0;
			if(InnerLink != null) {
				this.innerViewInfo = InnerLink.CreateViewInfo();
				this.innerViewInfo.ParentViewInfo = null;
				this.innerViewInfo.IsInnerLink = true;
				this.innerViewInfo.FixedGlyphSize = new Size(16, 16);
				this.innerViewInfo.AlwaysDrawAsInMenu = true;
				this.innerViewInfo.AlwaysDrawGlyph = true;
				this.innerViewInfo.AllowDrawBackground = false;
				BarBaseButtonLinkViewInfo buttonViewInfo = this.innerViewInfo as BarBaseButtonLinkViewInfo;
				if(buttonViewInfo != null)
					buttonViewInfo.AllowDrawCheckInMenu = false;
			}
			if(this.InnerViewInfo != null && glyphWidth > 0) glyphWidth += DrawParameters.Constants.SubMenuQGlyphGlyphIndent;
		}
		public override CustomViewInfo ParentViewInfo {
			get {
				return base.ParentViewInfo;
			}
			set {
				base.ParentViewInfo = value;
				if(InnerViewInfo != null)
					InnerViewInfo.ParentViewInfo = value;
			}
		}
		public BarItemLink InnerLink { get { return Link.InnerLink; } }
		public override void UpdateLinkInfo(object sourceObject) {
			base.UpdateLinkInfo(sourceObject);
			if(InnerViewInfo != null) InnerViewInfo.UpdateLinkInfo(sourceObject);
		}
		public virtual int GlyphWidth { get { return glyphWidth; } }
		public BarLinkViewInfo InnerViewInfo { get { return innerViewInfo; } set { innerViewInfo = value; } }
		public override Size GlyphSize {
			get {
				Size glyphSize = base.GlyphSize;
				glyphSize.Width = this.glyphWidth;
				if(InnerViewInfo != null) {
					Size innerSize = InnerViewInfo.GlyphSize;
					glyphSize.Width += innerSize.Width;
					glyphSize.Height = Math.Max(glyphSize.Height, innerSize.Height);
				}
				glyphSize = RaiseGlyphSizeEvent(glyphSize);
				return glyphSize;
			}
		}
		public override Size CalcLinkSize(Graphics g, object sourceObject) {
			Size size = base.CalcLinkSize(g, sourceObject);
			if(InnerViewInfo != null) {
				Size innerSize = InnerViewInfo.CalcLinkSize(g, sourceObject);
				size.Height = Math.Max(innerSize.Height, size.Height);
				size.Width += (innerSize.Width - (GlyphSize.Width - DrawParameters.Constants.SubMenuQGlyphGlyphIndent));
			}
			return size;
		}
		public override void CalcViewInfo(Graphics g, object sourceObject, Rectangle r) {
			base.CalcViewInfo(g, sourceObject, r);
			if(InnerViewInfo != null) {
				Rectangle innerBounds = r;
				int delta = glyphWidth;
				innerBounds.X += delta;
				innerBounds.Width -= (delta - DrawParameters.Constants.SubMenuQGlyphGlyphIndent);
				InnerViewInfo.CalcViewInfo(g, sourceObject, innerBounds);
			}
		}
		public override void UpdateLinkWidthInSubMenu(int linkWidth) {
			base.UpdateLinkWidthInSubMenu(linkWidth);
			if(InnerViewInfo != null) {
				Rectangle innerBounds = Bounds;
				if(IsRightToLeft) {
					innerBounds.X = Bounds.X;
					innerBounds.Width = Bounds.Width - (GlyphRect.Width - InnerViewInfo.GlyphSize.Width + DrawParameters.Constants.SubMenuQGlyphGlyphIndent);
				}
				else {
				innerBounds.X = GlyphRect.Right - InnerViewInfo.GlyphSize.Width - DrawParameters.Constants.SubMenuQGlyphGlyphIndent;
				innerBounds.Width = Bounds.Right - innerBounds.X;
				}
				InnerViewInfo.CalcViewInfo(GInfo.Graphics, SourceObject, innerBounds);
				InnerViewInfo.UpdateLinkWidthInSubMenu(innerBounds.Width);
			}
		}
		protected override void ReverseLinkRectsCore() {
			base.ReverseLinkRectsCore();
			if(InnerViewInfo != null) {
				InnerViewInfo.ReverseLinkRects();
			}
		}
	}
	public class BarMdiButtonLinkViewInfo : BarLargeButtonLinkViewInfo { 
		public new virtual BarMdiButtonItemLink Link { get { return base.Link as BarMdiButtonItemLink; } }
		public BarMdiButtonLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		public override bool AllowDrawLighter { get { return false; } }
		protected internal override int CalcLinkIndent(BarIndent linkIndent) { return 0; }
	}
	public class BarSystemMenuLinkViewInfo : BarCustomContainerLinkViewInfo {
		public BarSystemMenuLinkViewInfo(BarDrawParameters parameters, BarItemLink link) : base(parameters, link) {
		}
		public new virtual BarSystemMenuItemLink Link { get { return base.Link as BarSystemMenuItemLink; } }
		protected override BarLinkState CalcLinkState() { return BarLinkState.Normal; }
		public override bool AllowDrawLighter { get { return false; } }
		protected internal override int CalcLinkIndent(BarIndent linkIndent) { return 0; }
	}
	public class RibbonExpandCollapseItemLinkViewInfo : BarButtonLinkViewInfo {
		public RibbonExpandCollapseItemLinkViewInfo(BarDrawParameters dp, BarItemLink link) : base(dp, link) { }
		ObjectState GetLinkObjectState() {
			if(LinkState == BarLinkState.Pressed)
				return ObjectState.Pressed;
			if(LinkState == BarLinkState.Highlighted)
				return ObjectState.Hot;
			return ObjectState.Normal;
		}
		public override bool UpdateLinkState() {
			bool res = base.UpdateLinkState();
			Link.UserGlyph = GetLinkGlyph();
			return res;
		}
		protected Bitmap GetLinkGlyph() {
			if(Link.Ribbon.IsExpandButtonInPanel)
				return (Bitmap)(Link.Ribbon.Minimized ? Link.Ribbon.ViewInfo.Panel.PanelItems.GetPinGlyphImage(GetLinkObjectState()) : Link.Ribbon.ViewInfo.Panel.PanelItems.GetCollapseButtonGlyphImage(GetLinkObjectState()));
			return (Bitmap)(Link.Ribbon.Minimized ? Link.Ribbon.ViewInfo.Header.GetExpandButtonGlyphImage(GetLinkObjectState()) : Link.Ribbon.ViewInfo.Header.GetCollapseButtonGlyphImage(GetLinkObjectState()));
		}
	}
	public class AutoHiddenPagesMenuItemLinkViewInfo : BarButtonLinkViewInfo {
		public AutoHiddenPagesMenuItemLinkViewInfo(BarDrawParameters dp, BarItemLink link) : base(dp, link) { }
		ObjectState GetLinkObjectState() {
			if(LinkState == BarLinkState.Pressed)
				return ObjectState.Pressed;
			if(LinkState == BarLinkState.Highlighted)
				return ObjectState.Hot;
			return ObjectState.Normal;
		}
		public override bool UpdateLinkState() {
			bool res = base.UpdateLinkState();
			Link.UserGlyph = (Bitmap)(Link.Ribbon.ViewInfo.Header.GetExpandButtonGlyphImage(GetLinkObjectState()));
			return res;
		}
	}
	public class SkinBarSubItemViewInfo : BarSubItemLinkViewInfo {
		public SkinBarSubItemViewInfo(BarDrawParameters parameters, BarItemLink link)
			: base(parameters, link) {
		}
	}
	public class BarItemLinkTextHelper {
		public const int LineIndent = 0;
		public static string[] WrapString(string text, bool forceWrap) {
			string[] lines = new string[] { string.Empty, string.Empty };
			if(text == null || text.Length == 0) return lines;
			lines[0] = text;
			int length = text.Length;
			System.Text.StringBuilder[] sbs = new System.Text.StringBuilder[2];
			sbs[0] = new System.Text.StringBuilder();
			sbs[1] = new System.Text.StringBuilder();
			int half = length / 2;
			bool hasCaret = text.IndexOfAny(new char[] { '\xd', '\xa' }) != -1;
			int line = 0;
			int skippedCarets = 0;
			int middleSpaceIndex = GetMiddleWhiteSpaceIndex(text, forceWrap);
			for(int n = 0; n < length; n++) {
				char ch = text[n];
				if(ch == '\xd' || ch == '\xa') {
					if(line == 0) {
						line++;
						skippedCarets = -1;
					}
					else {
						if(++skippedCarets == 1) sbs[line].Append(' ');
					}
					continue;
				}
				skippedCarets = 0;
				if(!hasCaret && line == 0) {
					if(n == middleSpaceIndex) {
						line++;
						continue;
					}
				}
				sbs[line].Append(ch);
			}
			lines[0] = sbs[0].ToString();
			lines[1] = sbs[1].ToString();
			return lines;
		}
		protected static int GetMiddleWhiteSpaceIndex(string text, bool forceWrap) {
			int middleSpace = -1;
			for(int i = 0; i < text.Length; i++) {
				char c = text[i];
				if(char.IsWhiteSpace(c)) {
					if(middleSpace == -1) middleSpace = i;
					else {
						if(Math.Abs(text.Length - 2 * middleSpace) > Math.Abs(text.Length - 2 * i))
							middleSpace = i;
					}
				}
			}
			if(middleSpace < 5 && !forceWrap) return -1;
			return middleSpace;
		}
	}
}
