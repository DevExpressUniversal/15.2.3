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

using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Helpers;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public abstract class DocumentSelectorHeaderButtonSkinPainter : BaseButtonSkinPainter {
		public DocumentSelectorHeaderButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override Skin GetSkin() {
			return MetroUISkins.GetSkin(SkinProvider);
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault(Color.Empty, Color.Transparent, WindowsUIViewPainter.DefaultFont);
		}
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			if(info.Button is Docking2010.Views.WindowsUI.DocumentSelectorButton)
				DrawDocumentSelectorImage(cache, info);
			else
				base.DrawImage(cache, info);
		}
		protected virtual void DrawDocumentSelectorImage(GraphicsCache cache, BaseButtonInfo info) {
			Color foreColor = info.PaintAppearance.ForeColor;
			if(PaintAppearance != null && !info.Disabled)
				foreColor = PaintAppearance.ForeColor;
			Image image = GetActualImage(info);
			Rectangle r = new Rectangle(Point.Empty, info.ImageBounds.Size);
			var attributes = ImageColorizer.GetColoredAttributes(foreColor);
			cache.Paint.DrawImage(info.Graphics, image, info.ImageBounds, r, attributes);
		}
		protected override void CheckForeColor(BaseButtonInfo info) {
			Color color = GetBackground().Color.GetForeColor();
			if(info.Hot)
				info.PaintAppearance.ForeColor = GetHotColor(color);
			if(info.Pressed)
				info.PaintAppearance.ForeColor = GetPressedColor(color);
			if((info.Hot || info.Pressed) && (PaintAppearance != null))
				PaintAppearance.ForeColor = info.PaintAppearance.ForeColor;
			base.CheckForeColor(info);
		}
	}
	public class PageGroupButtonSkinPainter : DocumentSelectorHeaderButtonSkinPainter {
		public PageGroupButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetBackground() {
			return GetSkin()[MetroUISkins.SkinPageGroupItemHeaderButton];
		}
		protected override Color GetHotColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(MetroUISkins.PageGroupButtonHotColor, defaultColor);
		}
		protected override Color GetPressedColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(MetroUISkins.PageGroupButtonPressedColor, defaultColor);
		}
	}
	public class TabbedGroupTabButtonSkinPainter : DocumentSelectorHeaderButtonSkinPainter {
		public TabbedGroupTabButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetBackground() {
			return GetSkin()[MetroUISkins.SkinTabbedGroupItemHeaderButton] ?? GetSkin()[MetroUISkins.SkinPageGroupItemHeaderButton];
		}
		protected override Color GetHotColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(MetroUISkins.TabbedGroupButtonHotColor, MetroUISkins.PageGroupButtonHotColor, defaultColor);
		}
		protected override Color GetPressedColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(MetroUISkins.TabbedGroupButtonPressedColor, MetroUISkins.PageGroupButtonPressedColor, defaultColor);
		}
		protected override void DrawText(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasCaption) {
				if(PaintAppearance != null) {
					Size textSize = GetTextSize(cache, info);
					Rectangle destRect = PlacementHelper.Arrange(textSize, info.TextBounds, ContentAlignment.MiddleLeft);
					if(info.Disabled)
						DrawTextCore(cache, info.PaintAppearance, info, destRect);
					else
						DrawTextCore(cache, PaintAppearance, info, destRect);
				}
				else DrawTextCore(cache, info.PaintAppearance, info, info.TextBounds);
			}
		}
	}
	public class TabbedGroupTileButtonPainter : BaseButtonPainter, ITabbedGroupTileButtonPainter {
		public virtual System.Windows.Forms.Padding ContentMargin {
			get { return new Padding(14); }
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			PaintAppearance.DrawBackground(cache, Rectangle.Inflate(info.Bounds, -4, -4));
		}
	}
	public class TabbedGroupTileButtonSkinPainter : DocumentSelectorHeaderButtonSkinPainter, ITabbedGroupTileButtonPainter {
		public TabbedGroupTileButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public virtual System.Windows.Forms.Padding ContentMargin {
			get { return new Padding(14); }
		}
		protected override SkinElement GetBackground() {
			var skin = GetSkin();
			return skin[MetroUISkins.SkinTabbedGroupItemHeaderTile] ?? skin[MetroUISkins.SkinTabbedGroupItemHeaderButton] ?? skin[MetroUISkins.SkinPageGroupItemHeaderButton];
		}
		protected override Color GetHotColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(new object[] { MetroUISkins.TabbedGroupTileHotColor, MetroUISkins.TabbedGroupButtonHotColor, MetroUISkins.PageGroupButtonHotColor }, defaultColor);
		}
		protected override Color GetPressedColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(new object[] { MetroUISkins.TabbedGroupTilePressedColor, MetroUISkins.TabbedGroupButtonPressedColor, MetroUISkins.PageGroupButtonPressedColor }, defaultColor);
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			if(PaintAppearance.BackColor != DefaultAppearance.BackColor || PaintAppearance.BackColor != Color.Transparent)
				PaintAppearance.DrawBackground(cache, Rectangle.Inflate(info.Bounds, -4, -4));
			else base.DrawBackground(cache, info);
		}
		protected override Size GetTextSize(GraphicsCache cache, BaseButtonInfo info) {
			StringInfo textInfo = null;
			if(info.ButtonPanelOwner != null && info.ButtonPanelOwner.AllowHtmlDraw)
				textInfo = StringPainter.Default.Calculate(cache.Graphics, PaintAppearance, PaintAppearance.TextOptions, info.Caption, info.TextBounds, cache.Paint, info);
			return textInfo != null ? textInfo.Bounds.Size : Size.Round(PaintAppearance.CalcTextSize(cache.Graphics, info.Caption, info.TextBounds.Width));
		}
		protected override void DrawText(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasCaption) {
				if(PaintAppearance != null) {
					if(info.Disabled)
						DrawTextCore(cache, info.PaintAppearance, info, info.TextBounds);
					else
						DrawTextCore(cache, PaintAppearance, info, info.TextBounds);
				}
				else DrawTextCore(cache, info.PaintAppearance, info, info.TextBounds);
			}
		}
	}
	public class ActionsBarButtonPainter : BaseButtonPainter {
		protected override void RegisterPainters() {
			RegisterPainter<WindowsUISeparator>(new BaseSeparatorPainter());
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			var windwosUIButtonPanelOwner = info.ButtonPanelOwner as IWindowsUIButtonPanelOwner;
			if(windwosUIButtonPanelOwner != null && !windwosUIButtonPanelOwner.UseButtonBackgroundImages)
				return;
			int stateIndex = GetStateIndex(info);
			Color bgColor = GetBackgroundColor(stateIndex);
			Image coloredGlyphs = GetColoredGlyphs(bgColor);
			int w = coloredGlyphs.Width / 3;
			Rectangle sourceRect = new Rectangle(stateIndex * w, 0, w, coloredGlyphs.Height);
			if(info.Disabled)
				cache.Paint.DrawImage(info.Graphics, coloredGlyphs, info.ImageBounds, sourceRect, XPaint.DisabledImageAttr);
			else
				cache.Graphics.DrawImage(coloredGlyphs, info.ImageBounds, sourceRect, GraphicsUnit.Pixel);
		}
		protected override Image GetColoredGlyphs(Color color) {
			return ColoredElementsCache.GetActionsBarImage(color, GetButtonGlyphs);
		}
		protected override Image GetButtonGlyphs() {
			return Docking2010.Resources.CommonResourceLoader.GetImage("ButtonGlyphs");
		}
		protected override Color GetColor() {
			if(Info != null && Info.PaintAppearance != null && !Info.PaintAppearance.ForeColor.IsEmpty)
				return Info.PaintAppearance.ForeColor;
			return base.GetColor();
		}
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasImage && info.Button.Properties.UseImage) {
				Image image = GetActualImage(info);
				Rectangle srcRect = new Rectangle(Point.Empty, image.Size);
				Rectangle destRect = PlacementHelper.Arrange(image.Size, info.ImageBounds, ContentAlignment.MiddleCenter);
				int stateIndex = GetStateIndex(info);
				Color bgColor = GetForegroundColor(stateIndex);
				using(Image coloredImage = ColoredImageHelper.GetColoredImage(image, bgColor)) {
					if(info.Disabled)
						cache.Paint.DrawImage(info.Graphics, coloredImage, destRect, srcRect, ColoredImageHelper.DisabledImageAttr);
					else
						cache.Graphics.DrawImage(coloredImage, destRect, srcRect, GraphicsUnit.Pixel);
				}
			}
		}
	}
	public class ActionsBarButtonSeparatorPainter : BaseSeparatorPainter {
		ISkinProvider providerCore;
		public ActionsBarButtonSeparatorPainter(ISkinProvider provider) {
			providerCore = provider;
		}
		protected ISkinProvider SkinProvider {
			get { return providerCore; }
		}
		protected override Color GetSeparatorColor(BaseButtonInfo info) {
			if(!info.Button.Properties.Appearance.ForeColor.IsEmpty)
				return info.Button.Properties.Appearance.ForeColor;
			SkinElement header = GetHeaderElement();
			if(header != null)
				return header.Color.GetForeColor();
			return base.GetSeparatorColor(info);
		}
		protected virtual SkinElement GetHeaderElement() {
			return GetSkin()[MetroUISkins.SkinHeader];
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(SkinProvider);
		}
	}
	public class ActionsBarButtonSkinPainter : BaseButtonSkinPainter {
		public ActionsBarButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override void RegisterPainters() {
			RegisterPainter<WindowsUISeparator>(new ActionsBarButtonSeparatorPainter(SkinProvider));
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			var windwosUIButtonPanelOwner = info.ButtonPanelOwner as IWindowsUIButtonPanelOwner;
			if(windwosUIButtonPanelOwner != null && !windwosUIButtonPanelOwner.UseButtonBackgroundImages)
				return;
			int stateIndex = GetStateIndex(info);
			Color bgColor = GetBackgroundColor(stateIndex);
			Image coloredGlyphs = GetColoredGlyphs(bgColor);
			int w = coloredGlyphs.Width / 3;
			Rectangle sourceRect = new Rectangle(stateIndex * w, 0, w, coloredGlyphs.Height);
			Rectangle destRect = PlacementHelper.Arrange(new Size(w, coloredGlyphs.Height), info.ImageBounds, ContentAlignment.MiddleCenter);
			if(info.Disabled)
				cache.Paint.DrawImage(info.Graphics, coloredGlyphs, destRect, sourceRect, ColoredImageHelper.DisabledImageAttr);
			else
				cache.Graphics.DrawImage(coloredGlyphs, destRect, sourceRect, GraphicsUnit.Pixel);
		}
		protected override Image GetColoredGlyphs(Color color) {
			return ColoredElementsCache.GetActionsBarImage(color, GetButtonGlyphs);
		}
		protected override Color GetColor() {
			Color color = Color.Empty;
			if(Info != null && Info.PaintAppearance != null && !Info.PaintAppearance.ForeColor.IsEmpty)
				return Info.PaintAppearance.ForeColor;
			SkinElement element = GetActionsBarButtonSkinElement();
			if(element == null) {
				element = GetActionsBarSkinElement();
				if(element != null)
					color = element.Color.GetForeColor();
			}
			else color = element.Color.GetForeColor();
			return color;
		}
		protected override Color GetInvertedColor() {
			Color color = Color.Empty;
			if(Info != null && Info.PaintAppearance != null && !Info.PaintAppearance.BackColor.IsEmpty)
				return Info.PaintAppearance.BackColor;
			SkinElement element = GetActionsBarSkinElement();
			if(element != null)
				color = element.Color.GetBackColor();
			return color;
		}
		protected override Color GetPressedColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(MetroUISkins.ActionsBarButtonPressedColor, defaultColor);
		}
		protected override Color GetHotColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(MetroUISkins.ActionsBarButtonHotColor, defaultColor);
		}
		protected override Image GetButtonGlyphs() {
			Image image = base.GetButtonGlyphs();
			if(image != null) return image;
			return Docking2010.Resources.CommonResourceLoader.GetImage("ButtonGlyphs");
		}
		protected override Skin GetSkin() {
			return MetroUISkins.GetSkin(SkinProvider);
		}
		protected override SkinElement GetActionsBarButtonSkinElement() {
			return GetSkin()[MetroUISkins.SkinActionsBarButton];
		}
		protected virtual SkinElement GetActionsBarSkinElement() {
			return GetSkin()[MetroUISkins.SkinActionsBar];
		}
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasImage && info.Button.Properties.UseImage) {
				Image image = GetActualImage(info);
				Rectangle srcRect = new Rectangle(Point.Empty, image.Size);
				Rectangle destRect = PlacementHelper.Arrange(image.Size, info.ImageBounds, ContentAlignment.MiddleCenter);
				int stateIndex = GetStateIndex(info);
				Color bgColor = GetForegroundColor(stateIndex);
				using(Image coloredImage = ColoredImageHelper.GetColoredImage(image, bgColor)) {
					if(info.Disabled)
						cache.Paint.DrawImage(info.Graphics, coloredImage, destRect, srcRect, ColoredImageHelper.DisabledImageAttr);
					else
						cache.Graphics.DrawImage(coloredImage, destRect, srcRect, GraphicsUnit.Pixel);
				}
			}
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			SkinElement element = GetActionsBarButtonSkinElement();
			return SkinElementPainter.Default.CalcObjectMinBounds(new SkinElementInfo(element));
		}
	}
	public class HeaderButtonSkinPainter : ActionsBarButtonSkinPainter {
		public HeaderButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override Image GetColoredGlyphs(Color color) {
			return ColoredElementsCache.GetHeaderImage(color, GetButtonGlyphs);
		}
		protected override Color GetColor() {
			if(infoCore !=null && infoCore.PaintAppearance != null && !infoCore.PaintAppearance.ForeColor.IsEmpty)
				return infoCore.PaintAppearance.ForeColor;
			SkinElement element = GetHeaderElement();
			if(element != null)
				return element.Color.GetForeColor();
			return Color.Empty;
		}
		public override void DrawObject(ObjectInfoArgs e) {
			infoCore = e as BaseButtonInfo;
			CheckForeColor(infoCore);
			base.DrawObject(e);
		}
		protected override Color GetBackgroundColor(int index) {
			return GetColor();
		}
		protected override Color GetPressedColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(MetroUISkins.HeaderButtonPressedColor, defaultColor);
		}
		protected override Color GetHotColor(Color defaultColor) {
			return GetSkin().Colors.GetColor(MetroUISkins.HeaderButtonHotColor, defaultColor);
		}
		protected virtual SkinElement GetHeaderElement() {
			return GetSkin()[MetroUISkins.SkinHeader];
		}
		BaseButtonInfo infoCore;
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasImage) {
				Image image = GetActualImage(info);
				Rectangle srcRect = new Rectangle(Point.Empty, image.Size);
				Rectangle destRect = PlacementHelper.Arrange(image.Size, info.ImageBounds, ContentAlignment.MiddleCenter);
				Color color = GetColorForImage(info);
				using(Image coloredImage = ColoredImageHelper.GetColoredImage(image, color)) {
					if(info.Disabled)
						cache.Paint.DrawImage(info.Graphics, coloredImage, destRect, srcRect, ColoredImageHelper.DisabledImageAttr);
					else
						cache.Graphics.DrawImage(coloredImage, destRect, srcRect, GraphicsUnit.Pixel);
				}
			}
		}
		protected Color GetColorForImage(BaseButtonInfo info) {
			Color color = GetColor();
			if(info.Pressed) {
				if(color.R > 230 || color.G > 230 || color.B > 230)
					color = Color.Black;
				else
					color = Color.White;
				if(info.Button is IHeaderButton) {
					if(((IHeaderButton)info.Button).Owner is IContentContainerInternal)
						color = ((IContentContainerInternal)((IHeaderButton)info.Button).Owner).Manager.View.GetBackColor();
					if(!info.PaintAppearance.BackColor.IsEmpty)
						color = info.PaintAppearance.BackColor;
				}
				color = GetPressedImageColor(color);
			}
			return color;
		}
		protected virtual Color GetPressedImageColor(Color color) { return color; }
		AppearanceDefault defaultAppearanceCore;
		public override AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearanceCore == null)
					defaultAppearanceCore = new AppearanceDefault(GetColor(), Color.Empty, CustomHeaderButtonSkinPainter.DefaultFont);
				return defaultAppearanceCore;
			}
		}
	}
	public class CustomHeaderButtonSkinPainter : HeaderButtonSkinPainter {
		WindowsUIButtonInfo infoCore;
		static CustomHeaderButtonSkinPainter() {
			DefaultFont = SegoeUIFontsCache.GetFont("Segoe UI", 12f);
		}
		public CustomHeaderButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override Image GetColoredGlyphs(Color color) {
			return ColoredElementsCache.GetCustomButtonImage(color, GetButtonGlyphs);
		}
		public static Font DefaultFont { get; set; }
		public override int ImageToTextInterval { get { return 5; } }
		new WindowsUIButtonInfo Info {
			get { return infoCore; }
		}
		protected internal new AppearanceObject PaintAppearance {
			get { return paintAppearanceCore; }
		}
		public override void DrawObject(ObjectInfoArgs e) {
			infoCore = e as WindowsUIButtonInfo;
			UpdatePaintAppearance();
			base.DrawObject(e);
		}
		protected override SkinElement GetActionsBarButtonSkinElement() {
			return GetSkin()[MetroUISkins.SkinActionsBarButton];
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return Rectangle.Empty;
		}
		protected override void UpdatePaintAppearance() {
			if(Info == null) return;
			paintAppearanceCore = new FrozenAppearance();
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Info.StateAppearances, Info.Button.Properties.Appearance }, DefaultAppearance);
		}
		protected override Color GetColor() {
			Color color = Color.Empty;
			if(PaintAppearance != null && !PaintAppearance.ForeColor.IsEmpty)
				return PaintAppearance.ForeColor;
			return base.GetColor();
		}
		protected override Color GetPressedImageColor(Color color) {
			if(PaintAppearance != null && !PaintAppearance.BackColor.IsEmpty)
				return PaintAppearance.BackColor;
			return color;
		}
		AppearanceDefault defaultAppearanceCore;
		public override AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearanceCore == null)
					defaultAppearanceCore = new AppearanceDefault(GetColor(), Color.Empty, CustomHeaderButtonSkinPainter.DefaultFont);
				return defaultAppearanceCore;
			}
		}
	}
	public class OverviewButtonSkinPainter : ActionsBarButtonSkinPainter {
		AppearanceObject overviewButtonPaintAppearanceCore;
		public OverviewButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public AppearanceObject OverviewButtonPaintAppearance {
			get { return overviewButtonPaintAppearanceCore; }
			protected set { overviewButtonPaintAppearanceCore = value; }
		}
		protected override Image GetColoredGlyphs(Color color) {
			return ColoredElementsCache.GetHeaderImage(color, GetButtonGlyphs);
		}
		protected override Image GetButtonGlyphs() {
			return Docking2010.Resources.CommonResourceLoader.GetImage("OverviewButtonGlyphs");
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			int stateIndex = GetStateIndex(info);
			Color bgColor = OverviewButtonPaintAppearance.BackColor;
			if(OverviewButtonPaintAppearance.BackColor.IsEmpty)
				bgColor = GetBackgroundColor(stateIndex);
			Image coloredGlyphs = GetColoredGlyphs(bgColor);
			int w = coloredGlyphs.Width / 3;
			Rectangle sourceRect = new Rectangle(stateIndex * w, 0, w, coloredGlyphs.Height);
			DevExpress.Utils.Helpers.PaintHelper.DrawImage(cache.Graphics, coloredGlyphs, info.Bounds, GetBackgroundImageStretchMargins()); 
		}
		protected virtual void UpdatePaintAppearance(BaseButtonInfo info) {
			OverviewButtonInfo buttonInfo = info as OverviewButtonInfo;
			overviewButtonPaintAppearanceCore = new FrozenAppearance();
			if(buttonInfo != null) {
				AppearanceHelper.Combine(overviewButtonPaintAppearanceCore,
				new AppearanceObject[] { buttonInfo.StateAppearances, buttonInfo.PaintAppearance }, DefaultAppearance);
			}
		}
		protected override Color GetColor() {
			SkinElement element = GetOverviewTileElement();
			return element.Color.GetBackColor();
		}
		protected override Color GetPressedColor(Color defaultColor) {
			return GetSkin().Properties.GetColor(MetroUISkins.OverviewTilePressedColor, defaultColor);
		}
		protected override Color GetHotColor(Color defaultColor) {
			return GetSkin().Properties.GetColor(MetroUISkins.OverviewTileHotColor, defaultColor);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BaseButtonInfo info = e as BaseButtonInfo;
			UpdatePaintAppearance(info);
			if(info.Pressed) {
				DrawPressedButton(e.Cache, info);
			}
			else {
				DrawBackground(e.Cache, info);
				DrawContent(e.Cache, info);
			}
		}
		protected override void DrawText(GraphicsCache cache, BaseButtonInfo info) {
			if(!info.HasCaption) return;
			OverviewButtonInfo overviewButtonInfo = info as OverviewButtonInfo;
			if(overviewButtonInfo != null && overviewButtonInfo.OverviewContainerProperties.CanHtmlDraw) {
				StringPainter.Default.DrawString(cache, OverviewButtonPaintAppearance, info.Caption,
					info.TextBounds, OverviewButtonPaintAppearance.TextOptions, overviewButtonInfo.Button);
			}
			else {
				OverviewButtonPaintAppearance.DrawString(cache, info.Caption, info.TextBounds);
			}
		}
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasImage) {
				Image image = GetActualImage(info);
				Rectangle r = new Rectangle(Point.Empty, info.ImageBounds.Size);
				cache.Paint.DrawImage(cache.Graphics, image, info.ImageBounds, r, !info.Disabled);
			}
		}
		protected Padding GetBackgroundImageStretchMargins() {
			return new Padding(0);
		}
		protected virtual void DrawPressedButton(GraphicsCache graphicsCache, BaseButtonInfo info) {
			Rectangle buttonBounds = new Rectangle(0, 0, info.Bounds.Width, info.Bounds.Height);
			if(buttonBounds.Height == 0 || buttonBounds.Width == 0) return;
			using(Bitmap buttonImage = new Bitmap(buttonBounds.Width, buttonBounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
				using(Graphics g = Graphics.FromImage(buttonImage)) {
					using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(g, buttonBounds)) {
						bg.Graphics.TranslateTransform(-info.Bounds.X, -info.Bounds.Y);
						using(GraphicsCache bufferedCache = new GraphicsCache(bg.Graphics)) {
							DrawBackground(bufferedCache, info);
							DrawContent(bufferedCache, info);
						}
						bg.Render();
					}
				}
				Rectangle pressedButtonBounds = info.Bounds;
				pressedButtonBounds.Inflate(-2, -2);
				graphicsCache.Graphics.DrawImage(buttonImage, pressedButtonBounds);
			}
		}
		protected virtual SkinElement GetOverviewTileElement() {
			return GetSkin()[MetroUISkins.SkinOverviewTile];
		}
	}
}
