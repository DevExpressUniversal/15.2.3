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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraBars.InternalItems;
namespace DevExpress.XtraBars.Ribbon{
	public class RadialMenuViewInfo {
		internal static object ContentChangeAnimationId = new object();
		internal static object GlyphAnimationId = new object();
		internal static object LinkSelectionAnimationId = new object();
		internal static object LinkArrowSelectionAnimationId = new object();
		internal static object LinkBackgroundSelectionAnimationId = new object();
		internal static object LinkCaptionSelectionAnimationId = new object();
		internal static int GlyphFadeInAnimationLength = 200;
		internal static int GlyphFadeOutAnimationLength = 500;
		internal static int LinkSelectionFadeOutAnimationLength = 700;
		internal static int LinkArrowSelectionAnimationLength = 500;
		RadialMenuLayoutCalculator calculator;
		public RadialMenuViewInfo(RadialMenuWindow window) {
			Window = window;
			GInfo = new GraphicsInfo();
			HoverInfo = null;
			PressedInfo = null;
			this.calculator = CreateRadialMenuLayoutCalculator(window);
		}
		protected virtual RadialMenuLayoutCalculator CreateRadialMenuLayoutCalculator(RadialMenuWindow window) {
			return new RadialMenuLayoutCalculator(window);
		}
		protected internal bool IsReady { get; set; }
		public RadialMenuWindow Window { get; set; }
		public RadialMenu Menu { get { return Window.Menu; } }
		public Rectangle Bounds { get; private set; }
		protected GraphicsInfo GInfo { get; private set; }
		protected internal Size CalcMaximumSize(BarLinksHolder holder) {
			RadialMenuState? prevState = ForceMenuState;
			ForceMenuState = RadialMenuState.Expanded;
			try {
				return CalcBestSize(holder);
			} finally {
				ForceMenuState = prevState;
			}
		}
		public ISkinProvider Provider {
			get {
				if(Menu.Manager == null)
					return null;
				BarAndDockingController controller = Menu.Manager.GetController();
				return controller != null ? controller.LookAndFeel : null;
			}
		}
		bool IsClassicColors { get { return Menu.PaintStyle == PaintStyle.Classic; } }
		public Color BackColor {
			get {
				if(!Menu.BackColor.IsEmpty)
					return Menu.BackColor;
				if(IsClassicColors)
					return RadialMenu.DefaultBackColor;
				return LookAndFeelHelper.GetSystemColorEx(Provider, SystemColors.Window);
			}
		}
		public Color ForeColor {
			get {
				if(IsClassicColors)
					return Color.Black;
				return LookAndFeelHelper.GetSystemColorEx(Provider, SystemColors.WindowText);
			}
		}
		public Color GetTextColor(BarLinkState state) {
			if(state == BarLinkState.Disabled)
				return LookAndFeelHelper.GetSystemColorEx(Provider, SystemColors.GrayText);
			return ForeColor;
		}
		public Color GetLinkColor(BarLinkViewInfo linkInfo) {
			AppearanceObject obj = linkInfo.GetItemAppearance(BarLinkState.Normal);
			if(!obj.BorderColor.IsEmpty)
				return obj.BorderColor;
			return MenuColor;
		}
		public Color MenuColor {
			get {
				if(!Menu.MenuColor.IsEmpty)
					return Menu.MenuColor;
				if(IsClassicColors)
					return RadialMenu.DefaultMenuColor;
				Skin skin = RibbonSkins.GetSkin(Provider);
				return skin.Colors[RibbonSkins.SkinRadialMenuColor];
			}
		}
		public Color GetLinkHoverColor(BarLinkViewInfo linkInfo) {
			AppearanceObject obj = linkInfo.GetItemAppearance(BarLinkState.Highlighted);
			if(!obj.BorderColor.IsEmpty)
				return obj.BorderColor;
			return MenuHoverColor;
		}
		public Color GetLinkBackgroundColor(BarLinkViewInfo linkInfo) {
			AppearanceObject obj = linkInfo.GetItemAppearance(BarLinkState.Normal);
			if(!obj.BackColor.IsEmpty)
				return obj.BackColor;
			return BackColor;
		}
		public Color GetLinkBackgroundHoverColor(BarLinkViewInfo linkInfo) {
			AppearanceObject obj = linkInfo.GetItemAppearance(BarLinkState.Highlighted);
			if(!obj.BackColor.IsEmpty)
				return obj.BackColor;
			return BackColor;
		}
		public Color GetLinkCaptionColor(BarLinkViewInfo linkInfo) {
			AppearanceObject obj = linkInfo.GetItemAppearance(BarLinkState.Normal);
			if(!obj.ForeColor.IsEmpty)
				return obj.ForeColor;
			return ForeColor;
		}
		public Color GetLinkCaptionHoverColor(BarLinkViewInfo linkInfo) {
			AppearanceObject obj = linkInfo.GetItemAppearance(BarLinkState.Highlighted);
			if(!obj.ForeColor.IsEmpty)
				return obj.ForeColor;
			return ForeColor;
		}
		private Color MenuHoverColor {
			get {
				if(!Menu.SubMenuHoverColor.IsEmpty)
					return Menu.SubMenuHoverColor;
				if(IsClassicColors)
					return RadialMenu.DefaultMenuHoverColor;
				return TransformColor(MenuColor, -50);
			}
		}
		public Color BorderColor {
			get {
				if(!Menu.BorderColor.IsEmpty)
					return Menu.BorderColor;
				if(IsClassicColors)
					return RadialMenu.DefaultBorderColor;
				Color menuColor = MenuColor;
				return TransformColor(MenuColor, 50);
			}
		}
		protected Color TransformColor(Color color, int delta) {
			return Color.FromArgb(CheckColorRange(color.R + delta), CheckColorRange(color.G + delta), CheckColorRange(color.B + delta));
		}
		protected int CheckColorRange(int value) {
			if(value < 0) return 0;
			if(value > 255) return 255;
			return value;
		}
		public virtual Size CalcBestSize(BarLinksHolder holder) {
			return MenuState == RadialMenuState.Collapsed ? CalcCollapsedBestSize() : CalcExpandedBestSize();
		}
		RadialMenuHitInfo hoverInfo;
		public RadialMenuHitInfo HoverInfo {
			get { return hoverInfo; }
			set {
				if(value == null)
					value = new RadialMenuHitInfo();
				if(HoverInfo == value)
					return;
				RadialMenuHitInfo prev = HoverInfo;
				hoverInfo = value;
				OnHoverInfoChanged(prev, HoverInfo);
			}
		}
		protected internal virtual bool GetAllowGlyphSkinning(BarLinkViewInfo linkInfo) {
			if(!Menu.AllowGlyphSkinning || linkInfo == null) return false;
			return linkInfo.Link != null && linkInfo.Link.Item != null && linkInfo.Link.Item.AllowGlyphSkinning != DefaultBoolean.False;
		}
		protected internal bool ShouldDrawLinkBound(BarLinkViewInfo linkInfo) {
			if(HasArrow(linkInfo))
				return true;
			return !linkInfo.GetItemAppearance(BarLinkState.Normal).BorderColor.IsEmpty || !linkInfo.GetItemAppearance(BarLinkState.Highlighted).BorderColor.IsEmpty;
		}
		protected internal bool ShouldDrawLinkBackgroundCustomColor(BarLinkViewInfo linkInfo) {
			return !linkInfo.GetItemAppearance(BarLinkState.Normal).BackColor.IsEmpty || !linkInfo.GetItemAppearance(BarLinkState.Highlighted).BackColor.IsEmpty;
		}
		protected internal bool ShouldDrawLinkCaptionCustomColor(BarLinkViewInfo linkInfo) {
			if(linkInfo.LinkState == BarLinkState.Disabled)
				return false;
			return !linkInfo.GetItemAppearance(BarLinkState.Normal).ForeColor.IsEmpty || !linkInfo.GetItemAppearance(BarLinkState.Highlighted).ForeColor.IsEmpty;
		}
		protected virtual void OnHoverInfoChanged(RadialMenuHitInfo prev, RadialMenuHitInfo next) {
			if(prev != null) {
				if(prev.HitTest == RadialMenuHitTest.Glyph) {
					AddGlyphSelectionAnimation(1.0f, 0.0f, true);
				} else if(prev.HitTest == RadialMenuHitTest.Link && prev.LinkInfo.Link.Item != null) {
					AddLinkSelectionAnimation(prev.LinkInfo, 1.0f, 0.0f, true);
					AddLinkBackgroundSelectionAnimation(prev.LinkInfo, GetLinkBackgroundHoverColor(prev.LinkInfo), GetLinkBackgroundColor(prev.LinkInfo), true);
					AddLinkCaptionSelectionAnimation(prev.LinkInfo, GetLinkCaptionHoverColor(prev.LinkInfo), GetLinkCaptionColor(prev.LinkInfo), true);
				} else if(prev.HitTest == RadialMenuHitTest.LinkArrow && prev.LinkInfo.Link.Item != null) {
					AddLinkArrowSelectionAnimation(prev.LinkInfo, GetLinkHoverColor(prev.LinkInfo), GetLinkColor(prev.LinkInfo), true);
				}
			}
			if(next != null) {
				if(next.HitTest == RadialMenuHitTest.Glyph) {
					AddGlyphSelectionAnimation(0.0f, 1.0f, false);
				} else if(next.HitTest == RadialMenuHitTest.Link) {
					AddLinkSelectionAnimation(next.LinkInfo, 0.0f, 1.0f, false);
					AddLinkBackgroundSelectionAnimation(next.LinkInfo, GetLinkBackgroundColor(next.LinkInfo), GetLinkBackgroundHoverColor(next.LinkInfo), false);
					AddLinkCaptionSelectionAnimation(next.LinkInfo, GetLinkCaptionColor(next.LinkInfo), GetLinkCaptionHoverColor(next.LinkInfo), false);
				} else if(next.HitTest == RadialMenuHitTest.LinkArrow) {
					AddLinkArrowSelectionAnimation(next.LinkInfo, GetLinkColor(next.LinkInfo), GetLinkHoverColor(next.LinkInfo), false);
				}
			}
		}
		private void AddLinkSelectionAnimation(BarLinkViewInfo barLinkViewInfo, object animationId, Color start, Color end, bool reverse) {
			LinkAnimationId animId = new LinkAnimationId(barLinkViewInfo.Link, animationId);
			ColorAnimationInfo info = XtraAnimator.Current.Get(Window, animId) as ColorAnimationInfo;
			int ms = LinkArrowSelectionAnimationLength;
			if(info != null) {
				start = info.CurrentColor;
				ms = Math.Max(100, ms - (int)((info.CurrentTick - info.BeginTick) / TimeSpan.TicksPerMillisecond));
			}
			XtraAnimator.Current.Animations.Remove(Window, animId);
			XtraAnimator.Current.AddAnimation(new ColorAnimationInfo(Window, animId, ms, start, end));
		}
		void AddLinkBackgroundSelectionAnimation(BarLinkViewInfo linkInfo, Color start, Color end, bool reverse) {
			if(!ShouldDrawLinkBackgroundCustomColor(linkInfo))
				return;
			AddLinkSelectionAnimation(linkInfo, RadialMenuViewInfo.LinkBackgroundSelectionAnimationId, start, end, reverse);
		}
		void AddLinkCaptionSelectionAnimation(BarLinkViewInfo linkInfo, Color start, Color end, bool reverse) {
			if(!ShouldDrawLinkCaptionCustomColor(linkInfo))
				return;
			AddLinkSelectionAnimation(linkInfo, RadialMenuViewInfo.LinkCaptionSelectionAnimationId, start, end, reverse);
		}
		void AddLinkArrowSelectionAnimation(BarLinkViewInfo linkInfo, Color start, Color end, bool reverse) {
			if(!ShouldDrawLinkBound(linkInfo))
				return;
			AddLinkSelectionAnimation(linkInfo, RadialMenuViewInfo.LinkArrowSelectionAnimationId, start, end, reverse);
		}
		private void AddLinkSelectionAnimation(BarLinkViewInfo barLinkViewInfo, float start, float end, bool fadeOut) {
			LinkAnimationId animId = new LinkAnimationId(barLinkViewInfo.Link, RadialMenuViewInfo.LinkSelectionAnimationId);
			XtraAnimator.Current.Animations.Remove(Window, animId);
			if(!fadeOut) {
				Window.Invalidate();
				return;
			}
			XtraAnimator.Current.AddAnimation(new FloatAnimationInfo(Window, animId, RadialMenuViewInfo.LinkSelectionFadeOutAnimationLength, start, end, true));
		}
		protected virtual void AddGlyphSelectionAnimation(float start, float end, bool fadeOut) {
			FloatAnimationInfo info = XtraAnimator.Current.Get(Window, GlyphAnimationId) as FloatAnimationInfo;
			int ms = fadeOut ? GlyphFadeOutAnimationLength : GlyphFadeInAnimationLength;
			if(info != null) {
				start = info.Value;
				ms = Math.Max(100, ms - (int)((info.CurrentTick - info.BeginTick) / TimeSpan.TicksPerMillisecond));
			}
			XtraAnimator.Current.Animations.Remove(Window, GlyphAnimationId);
			XtraAnimator.Current.AddAnimation(new FloatAnimationInfo(Window, GlyphAnimationId, ms, start, end, true));
		}
		RadialMenuHitInfo pressedInfo;
		public RadialMenuHitInfo PressedInfo {
			get { return pressedInfo; }
			set {
				if(value == null)
					value = new RadialMenuHitInfo();
				if(PressedInfo == value)
					return;
				RadialMenuHitInfo prev = PressedInfo;
				pressedInfo = value;
				OnPressedInfoChanged(prev, PressedInfo);
			}
		}
		protected virtual void OnPressedInfoChanged(RadialMenuHitInfo prev, RadialMenuHitInfo next) {
		}
		public virtual RadialMenuHitInfo CalcHitInfo(Point pt) {
			RadialMenuHitInfo info = new RadialMenuHitInfo();
			info.HitPoint = pt;
			if(info.ContainsSet(CenterPoint, GlyphRadius, RadialMenuHitTest.Glyph))
				return info;
			if(MenuState == RadialMenuState.Collapsed)
				return info;
			foreach(BarLinkViewInfo linkInfo in LinksInfo) {
				float startAngle = (float)Math.Round((linkInfo.Angle - linkInfo.AngleWidth / 2), 6);
				float endAngle = (float)Math.Round((linkInfo.Angle + linkInfo.AngleWidth / 2) , 6);
				if(info.ContainsSet(CenterPoint, startAngle, endAngle, MenuRadius, RadialMenuHitTest.Link)) {
					info.LinkInfo = linkInfo;
					info.ContainsSet(CenterPoint, startAngle, endAngle, MenuInnerRadius, MenuRadius, RadialMenuHitTest.LinkArrow);
					return info;
				}
			}
			return info;
		}
		protected internal PointF RotateVector(Point v, float angle) {
			float ca = (float)Math.Cos(angle);
			float sa = (float)Math.Sin(angle);
			return new PointF(ca * v.X + sa * v.Y, -sa * v.X + ca * v.Y);
		}
		protected internal PointF RotateVector(PointF v, float angle) {
			float ca = (float)Math.Cos(angle);
			float sa = (float)Math.Sin(angle);
			return new PointF(ca * v.X + sa * v.Y, -sa * v.X + ca * v.Y);
		}
		protected internal PointF RotatePoint(Point p, float angle) {
			Point v = new Point(p.X - CenterPoint.X, p.Y - CenterPoint.Y);
			PointF res = RotateVector(v, angle);
			res.X += CenterPoint.X; res.Y += CenterPoint.Y;
			return res;
		}
		protected internal PointF RotatePoint(PointF p, float angle) {
			PointF v = new PointF(p.X - CenterPoint.X, p.Y - CenterPoint.Y);
			PointF res = RotateVector(v, angle);
			res.X += CenterPoint.X; res.Y += CenterPoint.Y;
			return res;
		}
		protected internal RadialMenuState? ForceMenuState {
			get;
			set;
		}
		protected internal RadialMenuState MenuState {
			get {
				return ForceMenuState.HasValue ? ForceMenuState.Value : Menu.State;
			}
		}
		public List<BarLinkViewInfo> LinksInfo { get; private set; }
		protected internal List<BarLinkViewInfo> PrevLinksInfo { get; private set; }
		protected internal virtual float GetSweepAngle(BarLinkViewInfo info) {
			return Rad2Degree((float)info.AngleWidth) - Rad2Degree(2.0f / MenuRadius);
		}
		protected internal virtual float GetStartAngle(BarLinkViewInfo info) {
			return Rad2Degree((float)(Math.PI * 2 - info.Angle)) - GetSweepAngle(info) / 2;
		}
		float Rad2Degree(float value) {
			return (float)(value * 180 / Math.PI);
		}
		protected internal virtual float GetOffsetAngle() {
			BoundsAnimationInfo ainfo = XtraAnimator.Current.Get((ISupportXtraAnimation)Window, RadialMenuWindow.BoundAnimationId) as BoundsAnimationInfo;
			float res = 0.0f;
			if(ainfo != null) {
				if(Menu.State == RadialMenuState.Expanded)
					res = (float)(-45.0f * (1.0f - (float)ainfo.CurrentFrame / ainfo.FrameCount));
				else
					res = (float)(-45.0f * (float)ainfo.CurrentFrame / ainfo.FrameCount);
			}
			return res;
		}
		protected internal virtual int GetLinkGlyphVerticalOffset(RadialMenuGraphicsInfoArgs e) {
			if(e.DrawOptions.UseLinkVerticalOffset)
				return LinkVerticalOffset;
			FloatAnimationInfo info = XtraAnimator.Current.Get(Window, RadialMenuWindow.TextFadeAnimationId) as FloatAnimationInfo;
			return info == null ? 0 : (int)((1.0f - info.Value) * LinkVerticalOffset);
		}
		protected internal virtual int GetTextAlpha() {
			FloatAnimationInfo info = XtraAnimator.Current.Get(Window, RadialMenuWindow.TextFadeAnimationId) as FloatAnimationInfo;
			return info == null ? 255 : (int)(info.Value * 255.0f);
		}
		protected virtual Size CalcExpandedBestSize() {
			return new Size(CalcMenuRadius() * 2, CalcMenuRadius() * 2);
		}
		public Point CenterPoint { get { return new Point(Window.Size.Width / 2, Window.Size.Height / 2); } }
		protected Size LinkBestSize { get; set; }
		public int MenuRadius { get; private set; }
		public int MenuItemsCenterRadius { get { return MenuRadius - RadialMenuConstants.MenuBoundsWidth - RadialMenuConstants.MenuItemsToBoundsIndent - calculator.CalcLinkBoundCircleRadius(LinkBestSize); } }
		public int MenuInnerRadius { get { return MenuRadius - RadialMenuConstants.MenuBoundsWidth; } }
		public Rectangle MenuInnerBounds {
			get {
				int mr = (int)(MenuInnerRadius * (float)Bounds.Width / Window.Size.Width);
				return new Rectangle(Window.Size.Width / 2 - mr, Window.Size.Height / 2 - mr, mr * 2, mr * 2);
			}
		}
		public Rectangle MenuItemSelectionBounds {
			get {
				int mr = (int)(MenuItemSelectionRadius * (float)Bounds.Width / Window.Size.Width);
				return new Rectangle(Window.Size.Width / 2 - mr, Window.Size.Height / 2 - mr, mr * 2, mr * 2);
			}
		}
		public virtual int MenuItemSelectionRadius { get { return MenuRadius - RadialMenuConstants.MenuBoundsWidth - RadialMenuConstants.MenuItemSelectionToBoundsIndent; } }
		public virtual int RealRadius { get { return MenuRadius; } }
		protected virtual List<BarLinkViewInfo> CreateLinksViewInfo(BarLinksHolder holder) {
			List<BarLinkViewInfo> res = new List<BarLinkViewInfo>();
			FillLinksViewInfo(holder, res);
			return res;
		}
		protected virtual bool ShouldMakeLinkVisible(BarItemLink link, BarLinkViewInfo linkInfo) {
			if(linkInfo == null || !link.Visible)
				return false;
			if(Menu.IsCustomizationMode)
				return true;
			return link.Item.GetRunTimeVisibility();
		}
		protected virtual void FillLinksViewInfo(BarLinksHolder holder, List<BarLinkViewInfo> links) {
			foreach(BarItemLink link in holder.ItemLinks) {
				BarLinkViewInfo linkInfo = link.CreateViewInfo();
				link.RadialMenu = Menu;
				linkInfo.RadialMenuWindow = this;
				linkInfo.UpdateLinkInfo(this);
				if(!ShouldMakeLinkVisible(link, linkInfo))
					continue;
				links.Add(linkInfo);
			}
			if(Menu.IsCustomizationMode) {
				BarLinkViewInfo linkInfo = Menu.Manager.InternalItems.DesignTimeItem.CreateLink(null, Menu).CreateViewInfo();
				linkInfo.Link.RadialMenu = Menu;
				linkInfo.RadialMenuWindow = this;
				linkInfo.UpdateLinkInfo(this);
				links.Add(linkInfo);
			}
		}
		public bool IsCustomizationMode { get { return Menu != null && Menu.IsCustomizationMode; } }
		public virtual bool ShouldDrawLinkBackground { get { return RealRadius >= RadialMenuConstants.MenuCenterGlyphRadius + RadialMenuConstants.MenuItemsToBoundsIndent; } }
		public virtual Rectangle ContentBounds {
			get {
				int infl = RadialMenuConstants.MenuBoundsWidth + RadialMenuConstants.MenuItemsToCenterGlyphIndent;
				Rectangle bounds = new Rectangle(Point.Empty, Window.Bounds.Size);
				return Rectangle.Inflate(bounds, -infl, -infl);
			}
		}
		public virtual Rectangle FreeSpaceBounds {
			get {
				Rectangle rect = new Rectangle(CenterPoint, Size.Empty);
				return Rectangle.Inflate(rect, Menu.InnerRadius, Menu.InnerRadius); 
			}
		}
		protected virtual Size CalcCollapsedBestSize() {
			return RadialMenuConstants.GlyphSize;
		}
		protected internal Rectangle GlyphBounds { get; private set; }
		protected internal Rectangle GlyphClientBounds { get; private set; }
		protected internal Rectangle GlyphSelectionBounds {
			get {
				Rectangle res = GlyphBounds;
				res.Inflate(6, 6);
				return res;
			}
		}
		protected internal int GlyphRadius { get { return GlyphBounds.Width / 2; } }
		[ThreadStatic]
		static Bitmap backArrowIcon;
		internal static Bitmap BackArrowIcon {
			get {
				if(backArrowIcon == null) {
					backArrowIcon = ResourceImageHelper.CreateImageFromResources("DevExpress.XtraBars.Images.BackArrow.png", typeof(RibbonControl).Assembly) as Bitmap;
				}
				return backArrowIcon;
			}
		}
		protected internal Image Glyph {
			get {
				if(Menu.ActualLinksHolder != Menu)
					return BackArrowIcon;
				if(Menu.Glyph != null)
					return Menu.Glyph;
				return RadialMenu.DefaultGlyph ?? RibbonControl.DefaultApplicationIcon;
			}
		}
		public virtual void CalcViewInfo(Graphics g, Rectangle bounds) {
			CalcViewInfo(g, bounds, Menu.ActualLinksHolder);
		}
		public virtual void CalcViewInfo(Graphics g, Rectangle bounds, BarLinksHolder holder) {
			if(IsReady)
				return;
			GInfo.AddGraphics(g);
			try {
				UpdateAppearance();
				Bounds = Rectangle.Inflate(bounds, -1, -1);
				GlyphBounds = CalcGlyphBounds(Bounds);
				GlyphClientBounds = CalcGlyphClientBounds(GlyphBounds, Glyph);
				CalcRadialMenuContentBestSize(holder);
				IsReady = true;
			} finally {
				LinksHolderInViewInfo = holder;
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual void CalcRadialMenuContentBestSize(BarLinksHolder holder) {
			this.LinksInfo = CreateLinksViewInfo(holder);
			this.LinkBestSize = calculator.CalcLinkBestSize(LinksInfo);
			this.MenuRadius = CalcMenuRadius();
			CalcLinksLayout(Bounds);
		}
		protected internal virtual Rectangle CalcGlyphClientBounds(Rectangle glyphBounds, Image glyph) {
			return new Rectangle(glyphBounds.X + (glyphBounds.Width - glyph.Width) / 2, glyphBounds.Y + (glyphBounds.Height - glyph.Size.Height) / 2, glyph.Width, glyph.Height);
		}
		protected virtual void UpdateAppearance() {
		}
		protected virtual Rectangle CalcGlyphBounds(Rectangle rect) {
			rect = new Rectangle(Point.Empty, Window.Size);
			if(Glyph == null)
				return Rectangle.Empty;
			return new Rectangle(rect.X + (rect.Width - RadialMenuConstants.GlyphSize.Width) / 2, rect.Y + (rect.Height - RadialMenuConstants.GlyphSize.Height) / 2, RadialMenuConstants.GlyphSize.Width, RadialMenuConstants.GlyphSize.Height);
		}
		protected virtual void CalcLinksLayout(Rectangle rect) {
			int itemSpringConut = calculator.CalcItemSpringCount(LinksInfo);
			Point centerPoint = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
			double deltaAngleStandard = Math.PI * 2 / Math.Max(RadialMenuConstants.MinimumSegmentCount, LinksInfo.Count);
			double deltaAngleSpring = itemSpringConut == 0 ? deltaAngleStandard : (Math.PI * 2 - ((LinksInfo.Count - itemSpringConut) * deltaAngleStandard)) / itemSpringConut;
			double startAngle = Math.PI / 2;
			double angle = startAngle;
			for(int i = 0; i < LinksInfo.Count; i++) {
				BarItem item = LinksInfo[i].Link.Item as BarItem;
				if(item == null) continue;
				LinksInfo[i].Angle = calculator.IsAutoSize(item) ? angle : angle + (deltaAngleSpring / 2 - deltaAngleStandard / 2);
				LinksInfo[i].AngleWidth = calculator.IsAutoSize(item) ? deltaAngleSpring : deltaAngleStandard;
				angle -= LinksInfo[i].AngleWidth;
				Point linkCenterPoint = new Point(centerPoint.X + (int)(MenuItemsCenterRadius * Math.Cos(LinksInfo[i].Angle)), centerPoint.Y - (int)(MenuItemsCenterRadius * Math.Sin(LinksInfo[i].Angle)));
				Rectangle linkBounds = new Rectangle(linkCenterPoint.X - LinkBestSize.Width / 2, linkCenterPoint.Y - LinkBestSize.Height / 2, LinkBestSize.Width, LinkBestSize.Height);
				LinksInfo[i].CalcViewInfo(GInfo.Graphics, this, linkBounds);
				LinksInfo[i].Bounds = linkBounds;
			}
		}
		Image renderImage;
		protected internal Image RenderImage {
			get { return renderImage; }
			set {
				if(RenderImage == value)
					return;
				if(RenderImage != null)
					RenderImage.Dispose();
				renderImage = value;
			}
		}
		Image nextRenderImage;
		protected internal Image NextRenderImage {
			get { return nextRenderImage; }
			set {
				if(NextRenderImage == value)
					return;
				if(NextRenderImage != null)
					NextRenderImage.Dispose();
				nextRenderImage = value;
			}
		}
		protected internal int LinkVerticalOffset { get { return 13; } }
		protected internal virtual void RenderContentToImage() {
			RadialMenuDrawOptions options = new RadialMenuDrawOptions();
			options.AllowDrawLinkText = Menu.State == RadialMenuState.Collapsed;
			options.AllowDrawGlyph = false;
			options.UseLinkVerticalOffset = Menu.State == RadialMenuState.Expanded;
			RenderImage = RenderContentToImage(Menu, RenderImage, options);
		}
		protected BarLinksHolder LinksHolderInViewInfo { get; set; }
		protected internal virtual Image RenderContentToImage(BarLinksHolder holder, Image image, RadialMenuDrawOptions options) {
			if(holder != LinksHolderInViewInfo) {
				IsReady = false;
				CalcViewInfo(null, Bounds, holder);
			}
			Size maxSize = Window.Size;
			if(image == null || image.Size != maxSize) {
				image = new Bitmap(maxSize.Width, maxSize.Height);
			}
			RadialMenuState? prevForceState = ForceMenuState;
			using(Graphics g = Graphics.FromImage(image)) {
				g.Clear(Color.Transparent);
				using(GraphicsCache cache = new GraphicsCache(g)) {
					ForceMenuState = RadialMenuState.Expanded;
					try {
						Window.Painter.Draw(new RadialMenuGraphicsInfoArgs(cache, Bounds, this, options));
					} finally {
						ForceMenuState = prevForceState;
					}
				}
			}
			return image;
		}
		internal PointF[] GetArrowPoints(BarLinkViewInfo info) {
			PointF[] pt = new PointF[] { PointF.Empty, PointF.Empty, PointF.Empty };
			float arrowRadius = MenuRadius - RadialMenuConstants.MenuBoundsWidth / 2 - 2;
			const int arrowHeight = 5;
			int arrowSideWidth = (int)((arrowHeight / Math.Tan(Math.PI / 4)) + 1);
			pt[0].X = CenterPoint.X;
			pt[0].Y = CenterPoint.Y - arrowRadius - arrowHeight;
			pt[1].X = CenterPoint.X - arrowSideWidth;
			pt[1].Y = pt[0].Y + arrowHeight;
			pt[2].X = CenterPoint.X + arrowSideWidth;
			pt[2].Y = pt[1].Y;
			pt[0] = RotatePoint(pt[0], (float)(info.Angle - Math.PI / 2));
			pt[1] = RotatePoint(pt[1], (float)(info.Angle - Math.PI / 2));
			pt[2] = RotatePoint(pt[2], (float)(info.Angle - Math.PI / 2));
			return pt;
		}
		protected internal virtual bool HasArrow(BarLinkViewInfo info) {
			return info.Link.Item is BarLinksHolder;
		}
		protected internal virtual void OnActualLinksHolderChanged(BarLinksHolder prev, BarLinksHolder next) {
			RadialMenuDrawOptions options = new RadialMenuDrawOptions() { AllowDrawGlyph = false, AllowDrawLinkBound = false, UseLinkVerticalOffset = false };
			RenderImage = RenderContentToImage(prev, RenderImage, options);
			SavePreviousLinks();
			NextRenderImage = RenderContentToImage(next, NextRenderImage, options);
			IsReady = false;
			XtraAnimator.Current.AddAnimation(new RadialMenuContentChangeAnimationInfo(Window, RadialMenuViewInfo.ContentChangeAnimationId, this, 400));
		}
		protected virtual void SavePreviousLinks() {
			if(PrevLinksInfo == null)
				PrevLinksInfo = new List<BarLinkViewInfo>();
			PrevLinksInfo.Clear();
			foreach(BarLinkViewInfo link in LinksInfo) {
				PrevLinksInfo.Add(link);
			}
		}
		protected internal virtual BarLinkViewInfo GetLinkViewInfo(BarItemLink barItemLink) {
			if(LinksInfo == null) return null;
			foreach(BarLinkViewInfo linkInfo in LinksInfo) {
				if(linkInfo.Link == barItemLink)
					return linkInfo;
			}
			return null;
		}
		protected internal virtual void ClearLinks() {
   			if(LinksInfo != null) LinksInfo.Clear();
			if(PrevLinksInfo != null) PrevLinksInfo.Clear();
		}
		protected internal virtual bool ShouldUseLinkAppearance(BarLinkViewInfo linkInfo, BarLinkState linkState) {
			return linkInfo.GetItemAppearance(linkState).Options.UseForeColor;
		}
		int bestRadius;
		protected internal virtual int CalcMenuRadius() {
			if(Menu.MenuRadius > 0) return Menu.MenuRadius;
			if(Menu.IsDesignMode && Menu.Ribbon != null) return calculator.CalcBestRadius(GetLargestRadiusHolder());
			if(Menu.IsCustomizationMode) return calculator.CalcBestRadius(GetLargestRadiusHolder());
			if(bestRadius == 0) bestRadius = calculator.CalcBestRadius(GetLargestRadiusHolder());
			return bestRadius;
		}
		protected virtual BarLinksHolder GetLargestRadiusHolder() {
			List<BarLinksHolder> holders = new List<BarLinksHolder>();
			FillHoldersList(ref holders, Menu);
			BarLinksHolder largestHolder = Menu;
			int largestRadius = calculator.CalcBestRadius(largestHolder);
			foreach(BarLinksHolder holder in holders) {
				int radius = calculator.CalcBestRadius(holder);
				if(radius > largestRadius) {
					largestHolder = holder;
					largestRadius = radius;
				}
			}
			return largestHolder;
		}
		protected virtual void FillHoldersList(ref List<BarLinksHolder> holders, BarLinksHolder holder) {
			holders.Add(holder);
			foreach(BarItemLink link in holder.ItemLinks) {
				BarLinksHolder h = link.Item as BarLinksHolder;
				if(h == null) continue;
				FillHoldersList(ref holders, h);
			}
		}
	}
	public static class RadialMenuConstants {
		public static int MenuCenterGlyphRadius { get { return 19; } }
		public static int MenuItemsToBoundsIndent { get { return 8; } }
		public static int MenuItemsToCenterGlyphIndent { get { return 5; } }
		public static int MenuBoundsWidth { get { return 22; } }
		public static int MenuRadiusMinimum { get { return 106 + MenuBoundsWidth; } }
		public static int MenuItemSelectionToBoundsIndent { get { return 6; } }
		internal static Size GlyphSize { get { return new Size(38, 38); } }
		internal static int MinimumSegmentCount { get { return 8; } }
	}
	public class RadialMenuLayoutCalculator {
		public RadialMenuLayoutCalculator(RadialMenuWindow window) {
			Window = window;
			GInfo = new GraphicsInfo();
		}
		public RadialMenuWindow Window { get; set; }
		public RadialMenu Menu { get { return Window.Menu; } }
		public Rectangle Bounds { get; protected set; }
		public List<BarLinkViewInfo> LinksInfo { get; protected set; }
		public GraphicsInfo GInfo { get; private set; }
		public int MenuRadius { get; protected set; }
		public int MenuItemsCenterRadius { get { return MenuRadius - RadialMenuConstants.MenuBoundsWidth - RadialMenuConstants.MenuItemsToBoundsIndent - CalcLinkBoundCircleRadius(LinkBestSize); } }
		public int CalcLinkBoundCircleRadius(Size linkBestSize) { return (int)Math.Sqrt(linkBestSize.Width * linkBestSize.Width + linkBestSize.Height * linkBestSize.Height) / 2; }
		protected internal virtual Rectangle GlyphBounds { get; protected set; }
		protected virtual Size LinkBestSize { get; set; }
		public int CalcBestRadius(BarLinksHolder holder) {
			MenuRadius = RadialMenuConstants.MenuRadiusMinimum;
			Bounds = CalcWindowSize();
			GlyphBounds = CalcGlyphRectangle();
			LinksInfo = CreateLinksViewInfo(holder);
			LinkBestSize = CalcLinkBestSize(LinksInfo);
			CalcLinksLayout();
			CalcParameters(holder);
			return MenuRadius;
		}
		protected virtual List<BarLinkViewInfo> CreateLinksViewInfo(BarLinksHolder holder) {
			List<BarLinkViewInfo> linksInfo = new List<BarLinkViewInfo>();
			foreach(BarItemLink link in holder.ItemLinks) {
				BarLinkViewInfo linkInfo = link.CreateViewInfo();
				linksInfo.Add(linkInfo);
			}
			if(Menu.IsCustomizationMode) {
				linksInfo.Add(Menu.Manager.InternalItems.DesignTimeItem.CreateLink(null, Menu).CreateViewInfo());
			}
			return linksInfo;
		}
		protected virtual Rectangle CalcWindowSize() {
			return new Rectangle(Point.Empty, new Size(MenuRadius * 2, MenuRadius * 2));
		}
		protected virtual Rectangle CalcGlyphRectangle() {
			return Rectangle.Inflate(new Rectangle(new Point(Bounds.Width / 2, Bounds.Height / 2), Size.Empty), RadialMenuConstants.GlyphSize.Width, RadialMenuConstants.GlyphSize.Height);
		}
		protected virtual void CalcParameters(BarLinksHolder holder) {
			int eps = 4;
			int min = MenuRadius + RadialMenuConstants.MenuBoundsWidth;
			if(holder.ItemLinks.Count <= 1) return;
			int max = (int)(1.5 * CalcMaxRadius(holder));
			while(Math.Abs(max - min) > eps) {
				MenuRadius = (max - min) / 2 + min;
				Bounds = CalcWindowSize();
				GlyphBounds = CalcGlyphRectangle();
				LinksInfo = CreateLinksViewInfo(holder);
				LinkBestSize = CalcLinkBestSize(LinksInfo);
				CalcLinksLayout();
				if(HasLinkCollision()) min = MenuRadius;
				else max = MenuRadius;
			}
			MenuRadius += RadialMenuConstants.MenuBoundsWidth;
		}
		protected internal virtual int CalcItemSpringCount(List<BarLinkViewInfo> linksInfo) {
			int count = 0;
			foreach(BarLinkViewInfo itemInfo in linksInfo) {
				BarItem item = itemInfo.Link.Item as BarItem;
				if(item == null) continue;
				if(IsAutoSize(item)) count++;
			}
			return count;
		}
		protected internal virtual bool IsAutoSize(BarItem item) {
			if(item is BarDesignTimeItem) return false;
			switch(Menu.GetAutoSize(item)) {
				case RadialMenuContainerItemAutoSize.Default: return IsAutoSizeInMenu;
				case RadialMenuContainerItemAutoSize.Spring: return true;
				case RadialMenuContainerItemAutoSize.None: return false;
				default: return false;
			}
		}
		protected virtual bool IsAutoSizeInMenu {
			get {
				BarLinksHolder holder = Menu.ActualLinksHolder;
				if(holder is RadialMenu && Menu.ItemAutoSize == RadialMenuItemAutoSize.Spring) return true;
				if(!(holder is BarLinkContainerItem)) return false;
				if(Menu.GetItemAutoSize((BarLinkContainerItem)holder) == RadialMenuContainerItemAutoSize.Spring) return true;
				if(Menu.GetItemAutoSize((BarLinkContainerItem)holder) == RadialMenuContainerItemAutoSize.Default && Menu.ItemAutoSize == RadialMenuItemAutoSize.Spring) return true;
				return false;
			}
		}
		protected virtual int CalcMaxRadius(BarLinksHolder holder) {
			double angle = 2 * Math.PI / holder.ItemLinks.Count;
			double distanceToItems = CalcLinkBoundCircleRadius() / Math.Tan(angle / 2) - CalcLinkBoundCircleRadius();
			distanceToItems = Math.Max(distanceToItems, RadialMenuConstants.MenuCenterGlyphRadius + RadialMenuConstants.MenuItemsToCenterGlyphIndent);
			int menuRadius = Math.Max((int)distanceToItems + CalcLinkBoundCircleRadius() * 2 + RadialMenuConstants.MenuItemsToBoundsIndent + RadialMenuConstants.MenuBoundsWidth, RadialMenuConstants.MenuRadiusMinimum);
			return menuRadius;
		}
		protected virtual bool HasLinkCollision() {
			for(int i = 0; i < LinksInfo.Count; i++) {
				BarLinkViewInfo li = LinksInfo[i], nli = LinksInfo[(i + 1) % LinksInfo.Count];
				if(RectangleHelper.IntersectsWith(li.Bounds, nli.Bounds, GlyphBounds)) return true;
				if(RectangleHelper.IntersectsWith(li.GlyphRect, nli.Bounds)) return true;
				if(RectangleHelper.IntersectsWith(li.Bounds, nli.GlyphRect)) return true;
			}
			return false;
		}
		protected virtual int CalcLinkBoundCircleRadius() {
			Size maxItemSize = CalcLinkBestSize(LinksInfo);
			return (int)Math.Sqrt(maxItemSize.Width * maxItemSize.Width + maxItemSize.Height * maxItemSize.Height) / 2;
		}
		protected internal virtual Size CalcLinkBestSize(List<BarLinkViewInfo> linksInfo) {
			Size res = Size.Empty;
			foreach(BarLinkViewInfo linkInfo in linksInfo) {
				linkInfo.CreateRadialMenuLinkMetrics(GInfo.Graphics);
				Size sz = linkInfo.CalcLinkSize(GInfo.Graphics, this);
				res.Width = Math.Max(sz.Width, res.Width);
				res.Height = Math.Max(sz.Height, res.Height);
			}
			return res;
		}
		protected virtual void CalcLinksLayout() {
			int itemSpringConut = CalcItemSpringCount(LinksInfo);
			Point centerPoint = new Point(Bounds.X + Bounds.Width / 2, Bounds.Y + Bounds.Height / 2);
			double deltaAngleStandard = Math.PI * 2 / Math.Max(RadialMenuConstants.MinimumSegmentCount, LinksInfo.Count);
			double deltaAngleSpring = itemSpringConut == 0 ? deltaAngleStandard : (Math.PI * 2 - ((LinksInfo.Count - itemSpringConut) * deltaAngleStandard)) / itemSpringConut;
			double startAngle = Math.PI / 2;
			double angle = startAngle;
			for(int i = 0; i < LinksInfo.Count; i++) {
				BarItem item = LinksInfo[i].Link.Item as BarItem;
				if(item == null) continue;
				LinksInfo[i].CreateRadialMenuLinkMetrics(GInfo.Graphics);
				LinksInfo[i].Angle = IsAutoSize(item) ? angle : angle + (deltaAngleSpring / 2 - deltaAngleStandard / 2);
				LinksInfo[i].AngleWidth = IsAutoSize(item) ? deltaAngleSpring : deltaAngleStandard;
				angle -= LinksInfo[i].AngleWidth;
				Size linkSize = IsWrappedText(LinksInfo[i]) ? CalcLinkTextSizeWrapped(LinksInfo[i]) : CalcLinkTextSizeNormal(LinksInfo[i]);
				Point linkCenterPoint = new Point(centerPoint.X + (int)(MenuItemsCenterRadius * Math.Cos(LinksInfo[i].Angle)), centerPoint.Y - (int)(MenuItemsCenterRadius * Math.Sin(LinksInfo[i].Angle)));
				Size ls = CalcLinkGlyphSize(LinksInfo[i]);
				int y = ls.IsEmpty ? linkCenterPoint.Y - linkSize.Height / 2 : linkCenterPoint.Y + (ls.Height + linkSize.Height) / 2 - linkSize.Height;
				Rectangle linkBounds = new Rectangle(linkCenterPoint.X - linkSize.Width / 2, y, linkSize.Width, linkSize.Height);
				LinksInfo[i].CalcViewInfo(GInfo.Graphics, this, linkBounds);
				LinksInfo[i].Bounds = linkBounds;
				Point glyphLocation = new Point(linkCenterPoint.X - ls.Width / 2, LinksInfo[i].Bounds.Y - ls.Height - 5);
				LinksInfo[i].GlyphRect = new Rectangle(glyphLocation, ls.IsEmpty ? Size.Empty : ls);
			}
		}
		protected virtual Size CalcLinkGlyphSize(BarLinkViewInfo li) {
			if(li.Link.LargeGlyph != null || (li.Link.ImageUri != null && li.Link.ImageUri.HasLargeImage)) return new Size(32, 32);
			if(li.Link.Glyph != null || (li.Link.ImageUri != null && li.Link.ImageUri.HasImage)) return new Size(16, 16);
			return Size.Empty;
		}
		protected virtual bool IsWrappedText(BarLinkViewInfo linkInfo) { 
			return linkInfo.RadialMenuLinkMetrics.WrappedText != null; 
		}
		protected virtual Size CalcLinkTextSizeWrapped(BarLinkViewInfo linkInfo) {
			Size sz = new Size(linkInfo.CaptionRect.Width, linkInfo.RadialMenuLinkMetrics.WrappedTextSize[0].Height);
			int countPart = 0; int maxWidth = 0; int maxHeight = 0;
			foreach(Size s in linkInfo.RadialMenuLinkMetrics.WrappedTextSize) {
				if(s.Width == 0) continue;
				countPart++;
				if(maxWidth < s.Width) maxWidth = s.Width;
				if(maxHeight < s.Height) maxHeight = s.Height;
			}
			return new Size(maxWidth, countPart * maxHeight);
		}
		protected virtual Size CalcLinkTextSizeNormal(BarLinkViewInfo linkInfo){ 
			return linkInfo.CalcLinkSize(GInfo.Graphics, this); 
		}
	}
	public enum RadialMenuHitTest { None, Glyph, Link, LinkArrow }
	public class RadialMenuHitInfo {
		public Point HitPoint { get; set; }
		public RadialMenuHitTest HitTest { get; set; }
		public BarLinkViewInfo LinkInfo { get; set; }
		public bool ContainsSet(Rectangle bounds, RadialMenuHitTest hitTest) {
			if(bounds.Contains(HitPoint)) {
				HitTest = hitTest;
				return true;
			}
			return false;
		}
		public bool ContainsSet(Point centerPoint, float startAngle, float endAngle, float startRadius, float endRadius, RadialMenuHitTest hitTest) {
			Point v = new Point(HitPoint.X - centerPoint.X, HitPoint.Y - centerPoint.Y);
			float radsq = v.X * v.X + v.Y * v.Y;
			if(startRadius * startRadius > radsq || endRadius * endRadius < radsq)
				return false;
			return ContainsSet(centerPoint, startAngle, endAngle, endRadius, hitTest);
		}
		public bool ContainsSet(Point centerPoint, float startAngle, float endAngle, float radius, RadialMenuHitTest hitTest) {
			if(!ContainsSet(centerPoint, radius, RadialMenuHitTest.None))
				return false;
			float angle = PointToAngle(centerPoint);
			if(startAngle < 0) startAngle = (float)Math.PI * 2 + startAngle;
			if(endAngle < 0) endAngle = (float)Math.PI * 2 + endAngle;
			if(startAngle > endAngle)
				endAngle += (float)Math.PI * 2;
			if(startAngle < (float)Math.PI * 2 && endAngle > (float)Math.PI * 2 && angle < (float)Math.PI / 2) {
				angle += (float)Math.PI * 2;
			}
			if(angle > startAngle && angle < endAngle) {
				HitTest = hitTest;
				return true;
			}
			return false;
		}
		protected internal virtual float PointToAngle(Point centerPoint) {
			float angle = 0.0f;
			PointF pt = new PointF(HitPoint.X - centerPoint.X, -HitPoint.Y + centerPoint.Y);
			if(pt.X == 0) {
				if(pt.Y > 0.0f)
					angle = (float)Math.PI / 2;
				else if(pt.Y < 0)
					angle = (float)(3 * Math.PI / 2);
				else
					angle = 0;
			} else if(pt.X > 0) {
				angle = (float)Math.Atan(pt.Y / pt.X);
				if(pt.Y < 0)
					angle += (float)(2 * Math.PI);
			} else {
				angle = (float)Math.Atan(pt.Y / pt.X) + (float)Math.PI;
			}
			return angle;
		}
		public bool ContainsSet(Point centerPoint, float radius, RadialMenuHitTest hitTest) {
			Point v = new Point(HitPoint.X - centerPoint.X, HitPoint.Y - centerPoint.Y);
			if(radius * radius > (v.X * v.X + v.Y * v.Y)) {
				HitTest = hitTest;
				return true;
			}
			return false;
		}
		public override bool Equals(object obj) {
			RadialMenuHitInfo info = (RadialMenuHitInfo)obj;
			if(HitTest != info.HitTest)
				return false;
			if(HitTest == RadialMenuHitTest.Link || HitTest == RadialMenuHitTest.LinkArrow) {
				return LinkInfo == info.LinkInfo;
			}
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public enum RadialMenuContentChangeState { HidePrevContent, HidePrevBounds, ShowNextContent, ShowNextBounds }
	public class RadialMenuContentChangeAnimationInfo : BaseAnimationInfo {
		public RadialMenuContentChangeAnimationInfo(ISupportXtraAnimation anim, object animationId, RadialMenuViewInfo menuInfo, int ms)
			: base(anim, animationId, 10, (int)(TimeSpan.TicksPerMillisecond * ms / 10)) {
			State = RadialMenuContentChangeState.HidePrevContent;
			Helper = new SplineAnimationHelper();
			Helper.Init(0.0f, 1.0f, 1.0f);
			MenuInfo = menuInfo;
			PrevContentBounds = MenuInfo.MenuInnerBounds;
			PrevContentOpacity = 1.0f;
			LinkBoundsInnerRadius = LinkBoundsStartInnerRadius;
			ClientBacgroundRadius = LinkBoundsStartInnerRadius;
		}
		protected RadialMenuViewInfo MenuInfo { get; set; }
		protected SplineAnimationHelper Helper { get; set; }
		public Rectangle PrevContentBounds { get; protected set; }
		public float PrevContentOpacity { get; protected set; }
		public float NextContentOpacity { get; protected set; }
		public Rectangle NextContentBounds { get; protected set; }
		protected float LinkBoundsStartOuterRadius {
			get { return MenuInfo.MenuRadius; }
		}
		protected float LinkBoundsStartInnerRadius {
			get { return MenuInfo.MenuInnerRadius; }
		}
		protected float LinkBoundsEndRadius { get { return MenuInfo.MenuInnerRadius * 0.9f; } }
		protected float LinkBoundsOuterRadius { get; set; }
		protected float LinkBoundsInnerRadius { get; set; }
		protected float ClientBacgroundRadius { get; set; }
		protected Rectangle CreateRectFromEllipse(Point center, float radius) {
			int r = (int)radius;
			return new Rectangle(center.X - r, center.Y - r, r * 2, r * 2);
		}
		public Rectangle LinkBoundsOuterRect {
			get { return CreateRectFromEllipse(MenuInfo.CenterPoint, LinkBoundsOuterRadius); }
		}
		public Rectangle LinkBoundsInnerRect {
			get { return CreateRectFromEllipse(MenuInfo.CenterPoint, LinkBoundsInnerRadius); }
		}
		public Rectangle ClientBackgroundBounds {
			get { return CreateRectFromEllipse(MenuInfo.CenterPoint, ClientBacgroundRadius); }
		}
		protected Rectangle PrevContentStartBounds { get { return MenuInfo.Bounds; } }
		protected Rectangle NextContentEndBounds { get { return MenuInfo.Bounds; } }
		protected Rectangle NextContentStartBounds { get { return PrevContentEndBounds; } }
		protected Rectangle PrevContentEndBounds {
			get {
				Rectangle rect = MenuInfo.Bounds;
				rect.Inflate(-20, -20);
				return rect;
			}
		}
		public RadialMenuContentChangeState State { get; internal set; }
		protected int HidePrevContentEndFrame { get { return FrameCount / 4; } }
		protected int HidePrevBoundsEndFrame { get { return HidePrevContentEndFrame + FrameCount / 4; } }
		protected int ShowNextBoundsEndFrame { get { return HidePrevBoundsEndFrame + FrameCount / 4; } }
		public override void FrameStep() {
			switch(State) {
				case RadialMenuContentChangeState.HidePrevContent:
					HidePrevContent();
					break;
				case RadialMenuContentChangeState.HidePrevBounds:
					HidePrevBounds();
					break;
				case RadialMenuContentChangeState.ShowNextBounds:
					ShowNextBounds();
					break;
				case RadialMenuContentChangeState.ShowNextContent:
					ShowNextContent();
					break;
			}
		}
		protected virtual void ShowNextContent() {
			if(IsFinalFrame) {
				NextContentBounds = NextContentEndBounds;
				NextContentOpacity = 1.0f;
				ClientBacgroundRadius = MenuInfo.MenuInnerRadius;
			} else {
				float k = GetProgressValue((float)(CurrentFrame - ShowNextBoundsEndFrame) / (FrameCount - ShowNextBoundsEndFrame));
				NextContentOpacity = k;
				NextContentBounds = CalcRectangle(NextContentStartBounds, NextContentEndBounds, k);
				ClientBacgroundRadius = LinkBoundsEndRadius + k * (LinkBoundsStartInnerRadius - LinkBoundsEndRadius);
			}
		}
		protected virtual void ShowNextBounds() {
			if(CurrentFrame > ShowNextBoundsEndFrame) {
				State = RadialMenuContentChangeState.ShowNextContent;
				LinkBoundsOuterRadius = LinkBoundsStartOuterRadius;
				LinkBoundsInnerRadius = LinkBoundsStartInnerRadius;
			} else {
				float k = GetProgressValue((float)(CurrentFrame - HidePrevBoundsEndFrame) / (ShowNextBoundsEndFrame - HidePrevBoundsEndFrame));
				LinkBoundsOuterRadius = LinkBoundsEndRadius + k * (LinkBoundsStartOuterRadius - LinkBoundsEndRadius);
				LinkBoundsInnerRadius = LinkBoundsEndRadius + k * (LinkBoundsStartInnerRadius - LinkBoundsEndRadius);
			}
		}
		protected virtual void HidePrevBounds() {
			if(CurrentFrame > HidePrevBoundsEndFrame) {
				State = RadialMenuContentChangeState.ShowNextBounds;
				LinkBoundsOuterRadius = LinkBoundsEndRadius;
				LinkBoundsInnerRadius = LinkBoundsEndRadius;
			} else {
				float k = GetProgressValue((float)(CurrentFrame - HidePrevContentEndFrame) / (HidePrevBoundsEndFrame - HidePrevContentEndFrame));
				LinkBoundsOuterRadius = LinkBoundsStartOuterRadius + k * (LinkBoundsEndRadius - LinkBoundsStartOuterRadius);
				LinkBoundsInnerRadius = LinkBoundsStartInnerRadius + k * (LinkBoundsEndRadius - LinkBoundsStartInnerRadius);
			}
		}
		protected float GetProgressValue(float k) {
			k = Math.Min(k, 1.0f);
			return k;
		}
		protected virtual void HidePrevContent() {
			if(CurrentFrame > HidePrevContentEndFrame) {
				State = RadialMenuContentChangeState.HidePrevBounds;
				PrevContentBounds = PrevContentEndBounds;
				PrevContentOpacity = 0.0f;
				LinkBoundsOuterRadius = LinkBoundsStartOuterRadius;
			} else {
				float k = GetProgressValue((float)CurrentFrame / HidePrevContentEndFrame);
				PrevContentOpacity = 1.0f - k;
				PrevContentBounds = CalcRectangle(PrevContentStartBounds, PrevContentEndBounds, k);
				ClientBacgroundRadius = LinkBoundsStartInnerRadius + k * (LinkBoundsEndRadius - LinkBoundsStartInnerRadius);
			}
		}
		protected Rectangle CalcRectangle(Rectangle start, Rectangle end, float k) {
			return new Rectangle(
					start.X + (int)((end.X - start.X) * k),
					start.Y + (int)((end.Y - start.Y) * k),
					start.Width + (int)((end.Width - start.Width) * k),
					start.Height + (int)((end.Height - start.Height) * k)
				);
		}
	}
	public class TextOpacityAnimationInfo : FloatAnimationInfo {
		public TextOpacityAnimationInfo(ISupportXtraAnimation obj, object animationId, int delay, int ms, float start, float end, bool useSpline)
			: base(obj, animationId, ms, start, end, useSpline) {
			Delay = delay;
		}
		public int Delay { get; private set; }
		public int DelayFrameCount { get { return (int)(Delay * TimeSpan.TicksPerMillisecond / DeltaTick); } }
		public override void FrameStep() {
			if((CurrentTick - BeginTick) < TimeSpan.TicksPerMillisecond * Delay) {
				Value = Start;
				return;
			}
			FrameStepCore((CurrentFrame - DelayFrameCount) / (float)(FrameCount - DelayFrameCount));
		}
	}
	internal class LinkAnimationId {
		public LinkAnimationId(BarItemLink link, object animId) {
			Link = link;
			AnimationId = animId;
		}
		public BarItemLink Link { get; set; }
		public object AnimationId { get; set; }
		public override bool Equals(object obj) {
			LinkAnimationId id = obj as LinkAnimationId;
			if(id == null) return false;
			return id.Link == Link && id.AnimationId == AnimationId;
		}
		public override int GetHashCode() {
			return Link.GetHashCode() ^ AnimationId.GetHashCode();
		}
	}
	public class RadialMenuDrawOptions {
		public RadialMenuDrawOptions() {
			AllowDrawGlyph = true;
			AllowDrawLinkBound = true;
			AllowDrawLinkText = true;
			UseLinkVerticalOffset = false;
		}
		public bool AllowDrawGlyph { get; set; }
		public bool AllowDrawLinkText { get; set; }
		public bool AllowDrawLinkBound { get; set; }
		public bool UseLinkVerticalOffset { get; set; }
	}
	public class RadialMenuGraphicsInfoArgs : GraphicsInfoArgs {
		public RadialMenuGraphicsInfoArgs(GraphicsCache cache, Rectangle bounds, RadialMenuViewInfo viewInfo, RadialMenuDrawOptions options)
			: this(cache, bounds, viewInfo) {
			DrawOptions = options;
		}
		public RadialMenuGraphicsInfoArgs(GraphicsCache cache, Rectangle bounds, RadialMenuViewInfo viewInfo)
			: base(cache, bounds) {
			DrawOptions = new RadialMenuDrawOptions();
			ViewInfo = viewInfo;
		}
		public RadialMenuViewInfo ViewInfo { get; set; }
		public RadialMenuDrawOptions DrawOptions { get; set; }
	}
}
