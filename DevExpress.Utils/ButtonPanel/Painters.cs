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
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.ButtonsPanelControl;
namespace DevExpress.XtraEditors.ButtonPanel {
	public class ButtonsPanelControlPainter : BaseButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new BaseButtonPainter();
		}
	}
	public class ButtonsPanelControlSkinPainter : BaseButtonsPanelSkinPainter {
		public ButtonsPanelControlSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new ButtonControlSkinPainter(Provider);
		}
	}
	public class ButtonControlSkinPainter : BaseButtonSkinPainter {
		public ButtonControlSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
	}
	public class BaseButtonsPanelPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DrawButtons(e.Cache, e as IButtonsPanelViewInfo);
		}
		public virtual BaseButtonPainter GetButtonPainter() {
			return new BaseButtonPainter();
		}
		public void CalcButtonState(IButtonsPanelViewInfo info) {
			foreach(BaseButtonInfo button in info.Buttons) {
				button.State = CalcButtonState(button.Button, info.Panel);
			}
		}
		protected virtual void DrawButtons(GraphicsCache cache, IButtonsPanelViewInfo info) {
			BaseButtonPainter painter = GetButtonPainter();
			if(info.Buttons == null) return;
			foreach(BaseButtonInfo buttonInfo in info.Buttons) {
				buttonInfo.State = CalcButtonState(buttonInfo.Button, info.Panel);
				BaseButtonPainter actualPainter = painter.GetButtonPainter(buttonInfo.Button.GetType());
				ObjectPainter.DrawObject(cache, actualPainter, buttonInfo);
			}
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		public virtual ObjectState CalcButtonState(IBaseButton button, IButtonsPanel panel) {
			ObjectState state = ObjectState.Normal;
			if(!button.Properties.Enabled)
				state |= ObjectState.Disabled;
			if(panel.IsSelected)
				state |= ObjectState.Selected;
			if(button == panel.Handler.HotButton)
				state |= ObjectState.Hot;
			if(button == panel.Handler.PressedButton)
				state |= ObjectState.Pressed;
			if(button.IsChecked.HasValue && button.IsChecked.Value && !(button is DefaultButton))
				state |= ObjectState.Pressed;
			return state;
		}
	}
	public class BaseButtonsPanelSkinPainter : BaseButtonsPanelPainter {
		ISkinProvider providerCore;
		public BaseButtonsPanelSkinPainter(ISkinProvider provider) {
			providerCore = provider;
		}
		public ISkinProvider Provider {
			get { return providerCore; }
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new BaseButtonSkinPainter(Provider);
		}
	}
	public class BaseButtonsPanelWindowsXpPainter : BaseButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new ButtonPainterForWindowsXPStyle();
		}
	}
	public class BaseButtonsPanelOffice2000Painter : BaseButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new ButtonOffice2000Painter();
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 3, 3);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -3, -3);
		}
	}
	public class BaseButtonsPanelOffice2003Painter : BaseButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new ButtonOffice2003Painter();
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 1, 1);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -1, -1);
		}
	}
	public class GroupBoxButtonsPanelSkinPainter : BaseButtonsPanelSkinPainter {
		public GroupBoxButtonsPanelSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		public override BaseButtonPainter GetButtonPainter() {
			return new GroupBoxButtonSkinPainter(Provider);
		}
	}
	public class GroupBoxButtonsPanelPainter : BaseButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new GroupBoxButtonPainter();
		}
	}
	public class GroupBoxButtonsPanelWindowsXpPainter : BaseButtonsPanelPainter {
		public override BaseButtonPainter GetButtonPainter() {
			return new GroupBoxButtonWindowsXpPainter();
		}
	}
	public class GroupBoxButtonPainter : BaseButtonPainter {
		ObjectPainter backgroundPainterCore;
		public GroupBoxButtonPainter() {
			backgroundPainterCore = CreateBackgroundPainter();
		}
		protected virtual ObjectPainter CreateBackgroundPainter() {
			return new ExpandButtonBackgroundFlatPainter(); ;
		}
		public ObjectPainter BackgroundPainter { get { return backgroundPainterCore; } }
		protected override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			PaintAppearance.ForeColor = SystemColors.ControlText;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 2, 2);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -2, -2);
		}
		protected override void RegisterPainters() {
			RegisterPainter<BaseSeparator>(new BaseSeparatorPainter());
			RegisterPainter<ExpandButton>(new GroupBoxExpandButtonFlatPainter());
			RegisterPainter<LayoutViewCardExpandButton>(new GroupBoxExpandButtonFlatPainter());
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			var rotateHelper = new RotateObjectPaintHelper();
			RotateFlipType rotateType = rotateHelper.RotateFlipTypeByRotationAngle(info.ButtonOwner.ButtonRotationAngle);
			rotateHelper.DrawRotated(cache, info, BackgroundPainter, rotateType);
		}
	}
	public class GroupBoxButtonWindowsXpPainter : GroupBoxButtonPainter {
		protected override void RegisterPainters() {
			RegisterPainter<BaseSeparator>(new BaseSeparatorPainter());
			RegisterPainter<ExpandButton>(new GroupBoxExpandButtonWindowsXpPainter());
			RegisterPainter<LayoutViewCardExpandButton>(new GroupBoxExpandButtonWindowsXpPainter());
		}
	}
	public class GroupBoxButtonSkinPainter : BaseButtonSkinPainter {
		public GroupBoxButtonSkinPainter(ISkinProvider provider) :
			base(provider) {
		}
		protected override SkinElement GetBackground() {
			SkinElement skinElement = GetExpandButtonSkinElement();
			if(skinElement.HasImage)
				return skinElement;
			else
				return base.GetBackground();
		}
		protected override AppearanceDefault CreateDefaultAppearance() {
			return base.CreateDefaultAppearance();
		}
		protected override void UpdatePaintAppearance() {
			paintAppearanceCore = new FrozenAppearance();
			if(Info == null || Info.ButtonPanelOwner == null || Info.ButtonPanelOwner.AppearanceButton == null) return;
			AppearanceObject buttonStateAppearance = Info.GetStateAppearance();
			if((Info.State & ObjectState.Pressed) != 0)
				AppearanceHelper.Apply(buttonStateAppearance, DefaultAppearancePressed);
			else if((Info.State & ObjectState.Hot) != 0)
				AppearanceHelper.Apply(buttonStateAppearance, DefaultAppearanceHot);
			else
				AppearanceHelper.Apply(buttonStateAppearance, DefaultAppearanceNormal);
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { buttonStateAppearance, Info.Button.Properties.Appearance }, DefaultAppearance);
		}
		AppearanceDefault defaultAppearanceNormalCore;
		public AppearanceDefault DefaultAppearanceNormal {
			get {
				if(defaultAppearanceNormalCore == null)
					defaultAppearanceNormalCore = CreateDefaultAppearanceNormal();
				return defaultAppearanceNormalCore;
			}
		}
		AppearanceDefault CreateDefaultAppearanceNormal() {
			return new AppearanceDefault(GetExpandButtonSkinElement().Color.GetForeColor(), Color.Empty);
		}
		AppearanceDefault defaultAppearanceHotCore;
		public AppearanceDefault DefaultAppearanceHot {
			get {
				if(defaultAppearanceHotCore == null)
					defaultAppearanceHotCore = CreateDefaultAppearanceHot();
				return defaultAppearanceHotCore;
			}
		}
		AppearanceDefault CreateDefaultAppearanceHot() {
			return new AppearanceDefault(GetExpandButtonSkinElement().Properties.GetColor(CommonSkins.SkinGroupPanelExpandButtonForeColorHot), Color.Empty);
		}
		AppearanceDefault defaultAppearancePressedCore;
		public AppearanceDefault DefaultAppearancePressed {
			get {
				if(defaultAppearancePressedCore == null)
					defaultAppearancePressedCore = CreateDefaultAppearancePressed();
				return defaultAppearancePressedCore;
			}
		}
		AppearanceDefault CreateDefaultAppearancePressed() {
			return new AppearanceDefault(GetExpandButtonSkinElement().Properties.GetColor(CommonSkins.SkinGroupPanelExpandButtonForeColorPressed), Color.Empty);
		}
		protected virtual SkinElement GetExpandButtonSkinElement() {
			return GetExpandButtonSkin()[CommonSkins.SkinGroupPanelExpandButton];
		}
		protected override SkinElementPainter GetBackgroundSkinElementPainter() {
			return SkinElementWithoutGlyphPainter.Default;
		}
		protected virtual Skin GetExpandButtonSkin() {
			return CommonSkins.GetSkin(SkinProvider);
		}
	}
	public class GroupBoxExpandButtonFlatPainter : GroupBoxButtonPainter {
		ObjectPainter contentPainter;
		protected ObjectPainter ContentPainter { get { return contentPainter; } }
		protected virtual Size DefaultSize { get { return new Size(11, 11); } }
		public GroupBoxExpandButtonFlatPainter() {
			contentPainter = CreateContentPainter();
		}
		protected virtual ObjectPainter CreateContentPainter() {
			return new ExpandButtonGlyphPainter();
		}
		protected override ObjectPainter CreateBackgroundPainter() {
			return new ExpandButtonBackgroundFlatPainter();
		}
		protected override void RegisterPainters() { }
		public override void DrawObject(ObjectInfoArgs e) {
			CalcObjectBoundsWithDefaultSize(e);
			base.DrawObject(e);
		}
		protected override void DrawContent(GraphicsCache cache, BaseButtonInfo info) {
			var rotateHelper = new RotateObjectPaintHelper();
			RotateFlipType rotateType = rotateHelper.RotateFlipTypeByRotationAngle(info.ButtonOwner.ButtonRotationAngle);
			rotateHelper.DrawRotated(cache, info, contentPainter, rotateType);
		}
		protected virtual void CalcObjectBoundsWithDefaultSize(ObjectInfoArgs e) {
			Point location = new Point(e.Bounds.X + (e.Bounds.Width - DefaultSize.Width) / 2, e.Bounds.Y + (e.Bounds.Height - DefaultSize.Height) / 2);
			e.Bounds = new Rectangle(location, DefaultSize);
		}
	}
	public class GroupBoxExpandButtonWindowsXpPainter : GroupBoxButtonWindowsXpPainter {
		protected override void RegisterPainters() { }
		protected virtual Size DefaultSize { get { return new Size(11, 11); } }
		protected override ObjectPainter CreateBackgroundPainter() {
			return new ExpandButtonWindowsXpPainter();
		}
		public override void DrawObject(ObjectInfoArgs e) {
			CalcObjectBoundsWithDefaultSize(e);
			base.DrawObject(e);
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			(BackgroundPainter as XPObjectPainter).DrawArgs = new DevExpress.Utils.WXPaint.WXPPainterArgs("treeview", DevExpress.Utils.WXPaint.XPConstants.BP_RADIOBUTTON, 0);
			base.DrawBackground(cache, info);
		}
		protected override void DrawContent(GraphicsCache cache, BaseButtonInfo info) {
		}
		protected virtual void CalcObjectBoundsWithDefaultSize(ObjectInfoArgs e) {
			Point location = new Point(e.Bounds.X + (e.Bounds.Width - DefaultSize.Width) / 2, e.Bounds.Y + (e.Bounds.Height - DefaultSize.Height) / 2);
			e.Bounds = new Rectangle(location, DefaultSize);
		}
	}
	public class GroupBoxExpandButtonOffice2003Painter : ButtonOffice2003Painter {
		protected override void RegisterPainters() { }
		public override void DrawObject(ObjectInfoArgs e) {
			UpdateForeColor(e);
			base.DrawObject(e);
		}
		protected void UpdateForeColor(ObjectInfoArgs e) {
			BaseButtonInfo info = e as BaseButtonInfo;
			if(!info.Pressed && !info.Hot)
				info.PaintAppearance.ForeColor = Color.White;
			else
				info.PaintAppearance.ForeColor = Color.Black;
		}
		protected override void DrawContent(GraphicsCache cache, BaseButtonInfo info) {
			var rotateHelper = new RotateObjectPaintHelper();
			RotateFlipType rotateType = rotateHelper.RotateFlipTypeByRotationAngle(info.ButtonOwner.ButtonRotationAngle);
			rotateHelper.DrawRotated(cache, info, new ExpandButtonOffice2003GlyphPainter(), rotateType);
		}
	}
	public class GroupBoxExpandButtonSkinPainter : GroupBoxButtonSkinPainter {
		public GroupBoxExpandButtonSkinPainter(ISkinProvider provider) :
			base(provider) {
		}
		protected override void RegisterPainters() { }
		protected override void RegisterPainters(ISkinProvider provider) { }
		protected internal override ImageCollection GetGlyphs(IBaseButton Button) {
			SkinElement skinElement = GetExpandButtonSkinElement();
			ImageCollection result = new ImageCollection();
			using(ImageCollection images = new ImageCollection()) {
				Image imageStrip = null;
				int imageCount = 0;
				if(skinElement.Glyph != null) {
					imageStrip = skinElement.Glyph.Image;
					imageCount = skinElement.Glyph.ImageCount;
				}
				else if(skinElement.Image != null) {
					imageStrip = skinElement.Image.Image;
					imageCount = skinElement.Image.ImageCount;
				}
				if(imageStrip == null) return base.GetGlyphs(Button);
				images.ImageSize = new Size(imageStrip.Width, imageStrip.Height / imageCount);
				images.AddImageStripVertical(imageStrip);
				result.ImageSize = images.ImageSize;
				if(imageCount == 6) {
					result.AddImage(images.Images[0]);
					result.AddImage(images.Images[1]);
					result.AddImage(images.Images[2]);
					result.AddImage(images.Images[3]);
					result.AddImage(images.Images[3]);
					result.AddImage(images.Images[4]);
					result.AddImage(images.Images[5]);
					result.AddImage(images.Images[3]);
				}
				else {
					foreach(Image item in images.Images) {
						result.AddImage(item);
					}
				}
			}
			return result;
		}
		protected override bool CanFlipGlyphRTL() {
			return true;
		}
	}
	public class LayoutViewExpandButtonSkinPainter : GroupBoxExpandButtonSkinPainter {
		public LayoutViewExpandButtonSkinPainter(ISkinProvider provider) :
			base(provider) {
		}
		protected override SkinElement GetExpandButtonSkinElement() {
			return GridSkins.GetSkin(SkinProvider)[GridSkins.SkinLayoutViewCardExpandButton] ?? base.GetExpandButtonSkinElement();
		}
		protected override bool CanFlipGlyphRTL() {
			return true;
		}
		protected override void DrawStandartBackground(GraphicsCache cache, BaseButtonInfo info) {
			var skinElement = GetBackground();
			SkinElementInfo elementInfo = new SkinElementInfo(skinElement, info.Bounds);
			if(skinElement.Image != null && skinElement.Glyph == null) return;
			elementInfo.ImageIndex = -1;
			elementInfo.State = info.State;
			if(info.State != ObjectState.Normal) elementInfo.ImageIndex = -1;
			ObjectPainter.DrawObject(cache, GetBackgroundSkinElementPainter(), elementInfo);
		}
	}
	public class RibbonPageGroupButtonSkinPainter : BaseButtonSkinPainter {
		public RibbonPageGroupButtonSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override void RegisterPainters() { }
		protected override void RegisterPainters(ISkinProvider provider) { }
		protected override SkinElement GetBackground() {
			SkinElement skinElement = GetExpandButtonSkinElement();
			return skinElement;
		}
		protected virtual SkinElement GetExpandButtonSkinElement() {
			return GetExpandButtonSkin()[RibbonSkins.SkinTabPanelGroupButton];
		}
		protected override SkinElementPainter GetBackgroundSkinElementPainter() {
			return SkinElementWithoutGlyphPainter.Default;
		}
		protected virtual Skin GetExpandButtonSkin() {
			return RibbonSkins.GetSkin(SkinProvider);
		}
		protected internal override ImageCollection GetGlyphs(IBaseButton Button) {
			SkinElement skinElement = GetExpandButtonSkinElement();
			ImageCollection result = new ImageCollection();
			using(ImageCollection images = new ImageCollection()) {
				Image imageStrip = null;
				int imageCount = 0;
				if(skinElement.Glyph != null) {
					imageStrip = skinElement.Glyph.Image;
					imageCount = skinElement.Glyph.ImageCount;
				}
				else {
					imageStrip = skinElement.Image.Image;
					imageCount = skinElement.Image.ImageCount;
				}
				if(imageStrip == null) return base.GetGlyphs(Button);
				images.ImageSize = new Size(imageStrip.Width, imageStrip.Height / imageCount);
				images.AddImageStripVertical(imageStrip);
				result = images;
			}
			return result;
		}
		protected override bool CanFlipGlyphRTL() {
			return true;
		}
		protected override int GetImageIndex(BaseButtonInfo info) {
			int result = base.GetImageIndex(info);
			if(info.Button.Properties.Glyphs != null)
				result = result % ImageCollection.GetImageListImageCount(info.Button.Properties.Glyphs);
			return result;
		}
	}
	public class BaseButtonPainter : ObjectPainter {
		Dictionary<Type, BaseButtonPainter> painters;
		public BaseButtonPainter() {
			painters = new Dictionary<Type, BaseButtonPainter>();
			RegisterPainters();
		}
		protected virtual void RegisterPainters() {
			RegisterPainter<BaseSeparator>(new BaseSeparatorPainter());
			RegisterPainter<ExpandButton>(new GroupBoxExpandButtonFlatPainter());
			RegisterPainter<LayoutViewCardExpandButton>(new GroupBoxExpandButtonFlatPainter());
		}
		protected void RegisterPainter<T>(BaseButtonPainter painter) where T : IBaseButton {
			painters.Add(typeof(T), painter);
		}
		public BaseButtonPainter GetButtonPainter(Type obj) {
			return painters.ContainsKey(obj) ? painters[obj] : this;
		}
		public virtual int ImageToTextInterval { get { return 2; } }
		BaseButtonInfo infoCore = null;
		protected AppearanceObject paintAppearanceCore;
		public override void DrawObject(ObjectInfoArgs e) {
			infoCore = e as BaseButtonInfo;
			UpdatePaintAppearance();
			DrawBackground(e.Cache, infoCore);
			DrawContent(e.Cache, infoCore);
		}
		protected internal AppearanceObject PaintAppearance {
			get { return paintAppearanceCore; }
		}
		protected virtual BaseButtonInfo Info {
			get { return infoCore; }
		}
		protected virtual void DrawStandartBackground(GraphicsCache cache, BaseButtonInfo info) {
			info.PaintAppearance.BackColor = SystemColors.Control;
			if(info.Hot)
				info.PaintAppearance.BackColor = SystemColors.HotTrack;
			if(info.Pressed)
				info.PaintAppearance.BackColor = SystemColors.Highlight;
			if(info.Disabled)
				info.PaintAppearance.BackColor = SystemColors.InactiveCaption;
			info.PaintAppearance.DrawBackground(info.Cache, info.Bounds);
		}
		Padding Margin {
			get {
				IButtonsPanelOwnerEx owner = Info.ButtonPanelOwner as IButtonsPanelOwnerEx;
				if(owner != null)
					return owner.ButtonBackgroundImageMargin;
				return Padding.Empty;
			}
		}
		protected virtual void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			int stateIndex = GetStateIndex(info);
			Image buttonGlyphs = GetButtonGlyphs();
			if(buttonGlyphs == null) DrawStandartBackground(cache, info);
			else {
				int w = buttonGlyphs.Width / 3;
				Rectangle sourceRect = new Rectangle(stateIndex * w, 0, w, buttonGlyphs.Height);
				Bitmap im = new Bitmap(w, buttonGlyphs.Height);
				Graphics g = Graphics.FromImage(im);
				if(info.Disabled) {
					cache.Paint.DrawImage(g, buttonGlyphs, new Rectangle(0, 0, im.Width, im.Height), sourceRect, DevExpress.Utils.Paint.XPaint.DisabledImageAttr);
					DevExpress.Utils.Helpers.PaintHelper.DrawImage(info.Graphics, im, Info.Bounds, Margin);
				}
				else {
					g.DrawImage(buttonGlyphs, new Rectangle(0, 0, im.Width, im.Height), sourceRect, GraphicsUnit.Pixel);
					DevExpress.Utils.Helpers.PaintHelper.DrawImage(info.Graphics, im, Info.Bounds, Margin);
				}
			}
		}
		protected virtual void DrawContent(GraphicsCache cache, BaseButtonInfo info) {
			if(info == null) return;
			CheckForeColor(info);
			DrawImage(cache, info);
			DrawText(cache, info);
		}
		protected virtual void DrawText(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasCaption) {
				if(PaintAppearance != null) {
					Size textSize = GetTextSize(cache, info);
					Rectangle textBounds = info.TextBounds;
					if(Info.IsVertical) {
						textSize = new Size(textSize.Height, textSize.Width);
						textBounds = new Rectangle(textBounds.Location, new Size(textBounds.Height, textBounds.Width));
					}
					Rectangle destRect = PlacementHelper.Arrange(textSize, info.TextBounds, ContentAlignment.MiddleCenter);
					if(info.Disabled)
						DrawTextCore(cache, info.PaintAppearance, info, destRect);
					else
						DrawTextCore(cache, PaintAppearance, info, destRect);
				}
				else DrawTextCore(cache, info.PaintAppearance, info, info.TextBounds);
			}
		}
		protected virtual void UpdatePaintAppearance() {
			paintAppearanceCore = new FrozenAppearance();
			if(Info != null)
				paintAppearanceCore.Assign(Info.Button.Properties.Appearance);
			if(Info == null || Info.ButtonPanelOwner == null || Info.ButtonPanelOwner.AppearanceButton == null) return;
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Info.GetStateAppearance(), Info.Button.Properties.Appearance }, DefaultAppearance);
		}
		protected void DrawTextCore(GraphicsCache cache, AppearanceObject appearance, BaseButtonInfo info, Rectangle destRect) {
			int angle = info.ButtonOwner != null ? (int)info.ButtonOwner.ButtonRotationAngle : 0;
			if(angle == 0 && info.GetAllowHtmlDraw())
				StringPainter.Default.DrawString(cache, appearance, info.Caption, destRect, appearance.TextOptions, info);
			else {
				DrawRotatedText(cache, appearance, info.Caption, destRect, angle);
			}
		}
		protected virtual void DrawRotatedText(GraphicsCache cache, AppearanceObject appearance, string text, Rectangle rect, int angle) {
			using(StringFormat format = appearance.GetStringFormat(appearance.TextOptions).Clone() as StringFormat) {
				appearance.DrawVString(cache, text, appearance.GetFont(), appearance.GetForeBrush(cache), rect, format, angle);
			}
		}
		protected virtual Size GetTextSize(GraphicsCache cache, BaseButtonInfo info) {
			StringInfo textInfo = null;
			if(info.GetAllowHtmlDraw())
				textInfo = StringPainter.Default.Calculate(cache.Graphics, PaintAppearance, PaintAppearance.TextOptions, info.Caption, Rectangle.Empty, cache.Paint, info);
			return textInfo != null ? textInfo.Bounds.Size : Size.Round(PaintAppearance.CalcTextSize(cache.Graphics, info.Caption, 0));
		}
		protected virtual void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			if(!info.HasImage) return;
			Image image = GetActualImage(info);
			Rectangle r = info.ImageBounds;
			RotateObjectPaintHelper rotateHelper = new RotateObjectPaintHelper();
			ImagePainterInfo painterInfo = new ImagePainterInfo(info.Cache, r, r.Size, image, AppearanceObject.EmptyAppearance);
			if(info.GetAllowGlyphSkinning())
				painterInfo.ImageAttributes = ImageColorizer.GetColoredAttributes(info.GetGlyphSkinningColor());
			if((info.State & ObjectState.Disabled) != 0)
				painterInfo.State = ObjectState.Disabled;
			ObjectPainter imagePainter = info.GetAllowGlyphSkinning() ?
				(ObjectPainter)new ColoredImagePainter() : (ObjectPainter)new ImagePainter();
			RotationAngle angle = RotationAngle.None;
			bool canFlipRTL = CanFlipGlyphRTL();
			if(Info.ButtonOwner != null) {
				canFlipRTL &= Info.ButtonOwner.RightToLeft;
				angle = info.ButtonOwner.ButtonRotationAngle;
			}
			rotateHelper.DrawRotated(info.Cache, painterInfo, imagePainter, rotateHelper.RotateFlipTypeByRotationAngle(angle, canFlipRTL));
		}
		protected virtual bool CanFlipGlyphRTL() {
			return false;
		}
		protected Image GetActualImage(BaseButtonInfo info) {
			if(info.Button.Properties.Glyphs != null)
				return ImageCollection.GetImageListImage(info.Button.Properties.Glyphs, GetImageIndex(info));
			return info.GetImageByUri() ?? info.Image;
		}
		protected virtual int GetImageIndex(BaseButtonInfo info) {
			int imageIndex = 0;
			if(info.Selected)
				imageIndex = 4;
			if(info.Hot)
				imageIndex = 1;
			if(info.Pressed)
				imageIndex = 2;
			if(info.Button.Properties.Checked)
				imageIndex = 2;
			if(info.Disabled)
				imageIndex = 3;
			if(info.Pressed && (info.Button is DefaultButton))
				imageIndex = 0;
			if((info.Button is BasePinButton || info.Button is BaseMaximizeButton) && info.Hot) {
				imageIndex += info.Button.Properties.Checked ? 1 : 0;
			}
			if(info.Selected && info.Button is DefaultButton) {
				imageIndex = info.Button.Properties.Checked ? 3 : 1;
			}
			return imageIndex;
		}
		protected virtual void DrawBorder(GraphicsCache cache, BaseButtonInfo info) { }
		protected virtual void CheckForeColorDisabled(BaseButtonInfo info) {
			info.PaintAppearance.ForeColor = SystemColors.GrayText;
		}
		protected virtual void CheckForeColor(BaseButtonInfo info) {
			if(info.Disabled)
				CheckForeColorDisabled(info);
			if((info.State & (ObjectState.Hot | ObjectState.Pressed | ObjectState.Disabled)) == 0)
				info.UpdatePaintAppearance(this);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 1, 1);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return Rectangle.Inflate(e.Bounds, -1, -1);
		}
		[ThreadStatic]
		static System.Windows.Forms.ImageList images;
		internal static System.Windows.Forms.ImageList Images {
			get {
				if(images == null) {
					images = ResourceImageHelper.CreateImageListFromResources(
						"DevExpress.Utils.ButtonPanel.ButtonImages.bmp",
						typeof(BaseButtonPainter).Assembly, new Size(10, 10), Color.Magenta);
				}
				return images;
			}
		}
		protected internal virtual ImageCollection GetGlyphs(IBaseButton Button) {
			if(!(Button is DefaultButton)) return null;
			ImageCollection result = new ImageCollection();
			result.ImageSize = Images.ImageSize;
			if(Button is BasePinButton) {
				result.Images.Add(Images.Images[3]);
				result.Images.Add(Images.Images[8]);
				result.Images.Add(Images.Images[4]);
				result.Images.Add(Images.Images[9]);
			}
			if(Button is BaseCloseButton) {
				result.Images.Add(Images.Images[2]);
				result.Images.Add(Images.Images[7]);
				result.Images.Add(Images.Images[2]);
				result.Images.Add(Images.Images[7]);
			}
			if(Button is BaseMaximizeButton) {
				result.Images.Add(Images.Images[1]);
				result.Images.Add(Images.Images[6]);
				result.Images.Add(Images.Images[0]);
				result.Images.Add(Images.Images[5]);
			}
			return result;
		}
		protected virtual int GetStateIndex(BaseButtonInfo info) {
			int index = 0;
			if(info.Hot)
				index = 1;
			if(info.Pressed)
				index = 2;
			return index;
		}
		protected virtual Image GetColoredGlyphs(Color color) { return null; }
		protected virtual Color GetBackgroundColor(int index) {
			Color color = GetColor();
			if(index == 1)
				return GetHotColor(color);
			if(index == 2)
				return GetPressedColor(color);
			return color;
		}
		protected virtual Color GetForegroundColor(int index) {
			Color color = GetColor();
			if(index == 1)
				return GetHotColor(color);
			if(index == 2)
				return GetInvertedColor();
			return color;
		}
		protected virtual Color GetColor() {
			if(PaintAppearance != null && !PaintAppearance.ForeColor.IsEmpty)
				return PaintAppearance.ForeColor;
			return Color.Empty;
		}
		protected virtual Color GetInvertedColor() { return Color.Black; }
		protected virtual Color GetPressedColor(Color defaultColor) { return SystemColors.Highlight; }
		protected virtual Color GetHotColor(Color defaultColor) { return SystemColors.HotTrack; }
		protected virtual Image GetButtonGlyphs() {
			if(Info != null && Info.ButtonPanelOwner != null && ImageCollection.GetImageListImageCount(Info.ButtonPanelOwner.ButtonBackgroundImages) > 2) {
				Size imageSize = ImageCollection.GetImageListSize(Info.ButtonPanelOwner.ButtonBackgroundImages);
				Bitmap glyphs = new Bitmap(imageSize.Width * 3, imageSize.Height);
				using(Graphics g = Graphics.FromImage(glyphs)) {
					for(int i = 0; i < 3; i++)
						g.DrawImageUnscaled(ImageCollection.GetImageListImage(Info.ButtonPanelOwner.ButtonBackgroundImages, i), new Point(i * imageSize.Width, 0));
				}
				return glyphs;
			}
			return null;
		}
	}
	public class BaseSeparatorPainter : BaseButtonPainter {
		static Margins DefaultMargins = new Margins(5, 5, 5, 5);
		protected override void RegisterPainters() { }
		protected override void DrawText(GraphicsCache cache, BaseButtonInfo info) { }
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			if(info.HasImage) return;
			Color actualColor = GetSeparatorColor(info);
			cache.FillRectangle(cache.GetSolidBrush(actualColor), info.ImageBounds);
		}
		protected virtual Color GetSeparatorColor(BaseButtonInfo info) {
			if(!info.Button.Properties.Appearance.ForeColor.IsEmpty)
				return info.Button.Properties.Appearance.ForeColor;
			return info.GetGlyphSkinningColor();
		}
		protected override void DrawImage(GraphicsCache cache, BaseButtonInfo info) {
			if(!info.HasImage) return;
			Rectangle srcImageRect = new Rectangle(Point.Empty, info.ImageBounds.Size);
			IButtonsPanel owner = GetOwner(info as ObjectInfoArgs);
			using(Bitmap bmp = new Bitmap(info.Image)) {
				using(Graphics g = Graphics.FromImage(bmp)) {
					if(!owner.IsHorizontal)
						bmp.RotateFlip(RotateFlipType.Rotate90FlipX);
					var attributes = ImageColorizer.GetColoredAttributes(info.GetGlyphSkinningColor());
					cache.Paint.DrawImage(info.Graphics, bmp, info.ImageBounds, srcImageRect, attributes);
				}
			}
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			IButtonsPanel panel = GetOwner(e);
			if(panel.Orientation == Orientation.Vertical) {
				client.Offset(0, -DefaultMargins.Top);
				return new Rectangle(client.Location, new Size(client.Width, client.Height + DefaultMargins.Top + DefaultMargins.Bottom));
			}
			client.Offset(-DefaultMargins.Left, 0);
			return new Rectangle(client.Location, new Size(client.Width + DefaultMargins.Left + DefaultMargins.Right, client.Height));
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			IButtonsPanel panel = GetOwner(e);
			Rectangle bounds = e.Bounds;
			if(panel.Orientation == Orientation.Vertical) {
				bounds.Offset(0, DefaultMargins.Top);
				return new Rectangle(bounds.Location, new Size(bounds.Width, bounds.Height - DefaultMargins.Top - DefaultMargins.Bottom));
			}
			bounds.Offset(DefaultMargins.Left, 0);
			return new Rectangle(bounds.Location, new Size(bounds.Width - DefaultMargins.Left - DefaultMargins.Right, bounds.Height));
		}
		IButtonsPanel GetOwner(ObjectInfoArgs e) {
			BaseButtonInfo info = e as BaseButtonInfo;
			BaseButton button = info.Button as BaseButton;
			IButtonsPanel panel = button.GetOwner();
			return panel;
		}
	}
	public class ButtonPainterForWindowsXPStyle : BaseButtonPainter {
		protected override void RegisterPainters() {
			RegisterPainter<BaseSeparator>(new BaseSeparatorPainter());
			RegisterPainter<ExpandButton>(new GroupBoxExpandButtonWindowsXpPainter());
			RegisterPainter<LayoutViewCardExpandButton>(new GroupBoxExpandButtonOffice2003Painter());
		}
		protected override int GetImageIndex(BaseButtonInfo info) {
			int imageIndex = 0;
			if(info.Selected)
				imageIndex = 4;
			if(info.Hot)
				imageIndex = 1;
			if(info.Pressed)
				imageIndex = 2;
			if(info.Button.Properties.Checked)
				imageIndex = 2;
			if(info.Disabled)
				imageIndex = 3;
			if(info.Pressed && (info.Button is DefaultButton))
				imageIndex = 0;
			if((info.Button is BasePinButton || info.Button is BaseMaximizeButton) && info.Hot) {
				imageIndex += info.Button.Properties.Checked ? 1 : 0;
			}
			if(info.Selected && !info.Hot && info.Button is DefaultButton) {
				imageIndex = info.Button.Properties.Checked ? 2 : 0;
			}
			if(info.Pressed && info.Selected)
				if(info.Button.IsChecked != null)
					imageIndex = ((bool)info.Button.IsChecked) ? 3 : 1;
				else imageIndex = 1;
			return imageIndex;
		}
	}
	public class BaseButtonSkinPainter : BaseButtonPainter {
		ISkinProvider providerCore;
		public BaseButtonSkinPainter(ISkinProvider provider)
			: base() {
			this.providerCore = provider;
			RegisterPainters(provider);
		}
		protected override void RegisterPainters() { }
		protected virtual void RegisterPainters(ISkinProvider provider) {
			RegisterPainter<BaseSeparator>(new BaseSeparatorPainter());
			RegisterPainter<ExpandButton>(new GroupBoxExpandButtonSkinPainter(provider));
			RegisterPainter<LayoutViewCardExpandButton>(new LayoutViewExpandButtonSkinPainter(provider));
			RegisterPainter<RibbonPageGroupButton>(new RibbonPageGroupButtonSkinPainter(provider));
		}
		protected ISkinProvider SkinProvider {
			get { return providerCore; }
		}
		protected virtual Skin GetSkin() {
			return DockingSkins.GetSkin(SkinProvider);
		}
		protected virtual Skin GetSkin(ObjectInfoArgs e) {
			return GetSkin();
		}
		protected virtual SkinElement GetBackground() {
			return GetSkin()[DockingSkins.SkinDockWindowButton];
		}
		protected virtual SkinElement GetBackground(ObjectInfoArgs e) {
			return GetBackground();
		}
		protected virtual SkinElement GetBackground(TabButtonsPanelState state) {
			return GetBackground();
		}
		protected override void DrawStandartBackground(GraphicsCache cache, BaseButtonInfo info) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetBackground(), info.Bounds);
			elementInfo.ImageIndex = -1;
			elementInfo.State = info.State;
			if(info.State != ObjectState.Normal) elementInfo.ImageIndex = -1;
			ObjectPainter.DrawObject(cache, GetBackgroundSkinElementPainter(), elementInfo);
		}
		protected virtual SkinElementPainter GetBackgroundSkinElementPainter() {
			return SkinElementPainter.Default;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElement element = GetBackground(e);
			BaseButtonInfo buttonInfo = e as BaseButtonInfo;
			if(element != null) {
				SkinPaddingEdges margins = element.ContentMargins;
				if(buttonInfo.IsVertical) {
					if(buttonInfo.ButtonOwner.ButtonRotationAngle == RotationAngle.Rotate270) {
						client.X -= margins.Top; client.Width += margins.Top + margins.Bottom;
						client.Y -= margins.Left; client.Height += margins.Left + margins.Right;
						return client;
					}
					if(buttonInfo.ButtonOwner.ButtonRotationAngle == RotationAngle.Rotate90) {
						client.X -= margins.Bottom; client.Width += margins.Bottom + margins.Top;
						client.Y -= margins.Left; client.Height += margins.Left + margins.Right;
						return client;
					}
				}
				return element.ContentMargins.Inflate(client);
			}
			return base.CalcBoundsByClientRectangle(e, client);
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			SkinElement element = GetBackground(e);
			BaseButtonInfo buttonInfo = e as BaseButtonInfo;
			Rectangle client = e.Bounds;
			if(element != null) {
				SkinPaddingEdges margins = element.ContentMargins;
				if(buttonInfo.IsVertical) {
					if(buttonInfo.ButtonOwner.ButtonRotationAngle == RotationAngle.Rotate270) {
						client.X += margins.Top; client.Width -= margins.Top + margins.Bottom;
						client.Y += margins.Left; client.Height -= margins.Left + margins.Right;
						return client;
					}
					if(buttonInfo.ButtonOwner.ButtonRotationAngle == RotationAngle.Rotate90) {
						client.X += margins.Bottom; client.Width -= margins.Bottom + margins.Top;
						client.Y += margins.Left; client.Height -= margins.Left + margins.Right;
						return client;
					}
				}
				return element.ContentMargins.Deflate(e.Bounds);
			}
			return base.GetObjectClientRectangle(e);
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault appearance = CreateDefaultAppearance();
				SkinElement backElement = GetBackground();
				if(backElement != null)
					backElement.ApplyForeColorAndFont(appearance);
				return appearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearance() {
			return new AppearanceDefault();
		}
		protected override void CheckForeColorDisabled(BaseButtonInfo info) {
			Skin skin = CommonSkins.GetSkin(SkinProvider);
			if(skin != null)
				info.PaintAppearance.ForeColor = skin.Colors.GetColor(CommonColors.DisabledText);
		}
		protected override void CheckForeColor(BaseButtonInfo info) {
			base.CheckForeColor(info);
		}
		protected virtual ImageCollection GetSkinButtonImages() {
			SkinElement normal = GetBarSkinElement(BarSkins.SkinDockWindowButtons);
			SkinElement hot = GetBarSkinElement(BarSkins.SkinDockWindowButtonsHot);
			SkinElement pressed = GetBarSkinElement(BarSkins.SkinDockWindowButtonsPressed);
			SkinElement selected = GetBarSkinElement(BarSkins.SkinDockWindowButtonsSelected);
			ImageCollection result = new ImageCollection();
			bool verticalLayout = (normal.Image.Layout == SkinImageLayout.Vertical);
			if(verticalLayout) {
				result.ImageSize = new Size(normal.Image.Image.Width, normal.Image.Image.Height / normal.Image.ImageCount);
				result.Images.AddImageStripVertical(normal.Image.Image);
				result.Images.AddImageStripVertical(hot.Image.Image);
				result.Images.AddImageStripVertical(pressed.Image.Image);
				result.Images.AddImageStripVertical((selected ?? normal).Image.Image);
			}
			else {
				result.ImageSize = new Size(normal.Image.Image.Width / normal.Image.ImageCount, normal.Image.Image.Height);
				result.Images.AddImageStrip(normal.Image.Image);
				result.Images.AddImageStrip(hot.Image.Image);
				result.Images.AddImageStrip(pressed.Image.Image);
				result.Images.AddImageStrip((selected ?? normal).Image.Image);
			}
			return result;
		}
		protected virtual SkinElement GetActionsBarButtonSkinElement() {
			return GetSkin()[SkinProvider.SkinName];
		}
		protected override Color GetColor() {
			Color color = base.GetColor();
			if(color != Color.Empty) return color;
			SkinElement element = GetActionsBarButtonSkinElement();
			if(element != null)
				color = element.Color.GetForeColor();
			return color;
		}
		SkinElement GetBarSkinElement(string elementName) {
			return BarSkins.GetSkin(SkinProvider)[elementName];
		}
		protected override int GetImageIndex(BaseButtonInfo info) {
			if(!(info.Button is DefaultButton))
				return base.GetImageIndex(info);
			int imageIndex = 0;
			int checkedIndex = 0;
			if(info.Button.Properties.Checked)
				checkedIndex = 4;
			imageIndex = checkedIndex;
			if(info.Selected)
				imageIndex = checkedIndex + 3;
			if(info.Hot)
				imageIndex = checkedIndex + 1;
			if(info.Pressed)
				imageIndex = checkedIndex + 2;
			return imageIndex;
		}
		protected internal override ImageCollection GetGlyphs(IBaseButton Button) {
			if(!(Button is DefaultButton)) return null;
			using(ImageCollection images = GetSkinButtonImages()) {
				ImageCollection result = new ImageCollection();
				result.ImageSize = images.ImageSize;
				if(Button is BasePinButton) {
					result.Images.Add(images.Images[3]);
					result.Images.Add(images.Images[11]);
					result.Images.Add(images.Images[19]);
					result.Images.Add(images.Images[27]);
					result.Images.Add(images.Images[4]);
					result.Images.Add(images.Images[12]);
					result.Images.Add(images.Images[20]);
					result.Images.Add(images.Images[28]);
				}
				if(Button is BaseCloseButton) {
					result.Images.Add(images.Images[2]);
					result.Images.Add(images.Images[10]);
					result.Images.Add(images.Images[18]);
					result.Images.Add(images.Images[26]);
					result.Images.Add(images.Images[2]);
					result.Images.Add(images.Images[10]);
					result.Images.Add(images.Images[18]);
					result.Images.Add(images.Images[26]);
				}
				if(Button is BaseMaximizeButton) {
					result.Images.Add(images.Images[1]);
					result.Images.Add(images.Images[9]);
					result.Images.Add(images.Images[17]);
					result.Images.Add(images.Images[25]);
					result.Images.Add(images.Images[0]);
					result.Images.Add(images.Images[8]);
					result.Images.Add(images.Images[16]);
					result.Images.Add(images.Images[24]);
				}
				return result;
			}
		}
	}
	public class ButtonOffice2000Painter : BaseButtonPainter {
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			DrawBorder(cache, info);
		}
		protected override void DrawBorder(GraphicsCache cache, BaseButtonInfo info) {
			if(!info.Hot) return;
			cache.FillRectangle(Brushes.Black, new Rectangle(info.Bounds.Left, info.Bounds.Top, info.Bounds.Width, 1));
			cache.FillRectangle(Brushes.Black, new Rectangle(info.Bounds.Left, info.Bounds.Bottom - 1, info.Bounds.Width, 1));
			cache.FillRectangle(Brushes.Black, new Rectangle(info.Bounds.Left, info.Bounds.Top, 1, info.Bounds.Height));
			cache.FillRectangle(Brushes.Black, new Rectangle(info.Bounds.Right - 1, info.Bounds.Top, 1, info.Bounds.Height));
		}
		protected override int GetImageIndex(BaseButtonInfo info) {
			int imageIndex = 0;
			if(info.Selected)
				imageIndex = 4;
			if((info.Button is BasePinButton || info.Button is BaseMaximizeButton) && info.Hot) {
				imageIndex += info.Button.Properties.Checked ? 1 : 0;
			}
			if(info.Button.Properties.Checked)
				imageIndex = 2;
			if(info.Selected && info.Button is DefaultButton) {
				imageIndex = info.Button.Properties.Checked ? 3 : 1;
			}
			return imageIndex;
		}
	}
	public class ButtonOffice2003Painter : BaseButtonPainter {
		protected override void RegisterPainters() {
			RegisterPainter<BaseSeparator>(new BaseSeparatorPainter());
			RegisterPainter<ExpandButton>(new GroupBoxExpandButtonOffice2003Painter());
			RegisterPainter<LayoutViewCardExpandButton>(new GroupBoxExpandButtonOffice2003Painter());
		}
		protected override void DrawBackground(GraphicsCache cache, BaseButtonInfo info) {
			if(info.State == ObjectState.Normal || info.State == ObjectState.Selected) return;
			if(info.Disabled) {
				info.PaintAppearance.BackColor = Office2003Colors.Default[Office2003Color.ButtonDisabled];
				info.PaintAppearance.DrawBackground(info.Cache, info.Bounds);
				return;
			}
			Brush backBrush = Brushes.Transparent;
			if(info.Pressed && info.Hot)
				backBrush = cache.GetGradientBrush(info.Bounds, Office2003Colors.Default[Office2003Color.Button1Pressed], Office2003Colors.Default[Office2003Color.Button2Pressed], LinearGradientMode.Vertical);
			else
				backBrush = cache.GetGradientBrush(info.Bounds, Office2003Colors.Default[Office2003Color.Button1Pressed], Office2003Colors.Default[Office2003Color.Button1Hot], LinearGradientMode.Vertical);
			cache.Graphics.FillRectangle(backBrush, info.Bounds);
			DrawBorder(cache, info);
		}
		protected override void DrawBorder(GraphicsCache cache, BaseButtonInfo info) {
			if(info.Disabled) return;
			Brush borderBrish = cache.GetSolidBrush(Office2003Colors.Default[Office2003Color.Border]);
			cache.FillRectangle(borderBrish, new Rectangle(info.Bounds.Left, info.Bounds.Top, info.Bounds.Width, 1));
			cache.FillRectangle(borderBrish, new Rectangle(info.Bounds.Left, info.Bounds.Bottom - 1, info.Bounds.Width, 1));
			cache.FillRectangle(borderBrish, new Rectangle(info.Bounds.Left, info.Bounds.Top, 1, info.Bounds.Height));
			cache.FillRectangle(borderBrish, new Rectangle(info.Bounds.Right - 1, info.Bounds.Top, 1, info.Bounds.Height));
		}
		protected override int GetImageIndex(BaseButtonInfo info) {
			int imageIndex = 0;
			if(info.Selected)
				imageIndex = 4;
			if(info.Button.Properties.Checked)
				imageIndex = 2;
			if((info.Button is BasePinButton || info.Button is BaseMaximizeButton) && info.Hot) {
				imageIndex += info.Button.Properties.Checked ? 1 : 0;
			}
			if(info.Selected && info.Button is DefaultButton) {
				imageIndex = info.Button.Properties.Checked ? 3 : 1;
			}
			return imageIndex;
		}
	}
}
