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
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Text;
namespace DevExpress.Utils.Drawing {	
	public class ToggleObjectInfoArgs : BaseCheckObjectInfoArgs {
		Rectangle switchRectCore;
		Size minSizeCore;
		int switchWidthCore, textMarginCore;
		int stepCore;
		bool isAnimationCore;
		string textOff, textOn;
		bool showTextCore;
		bool isDragCore;
		public Rectangle SwitchRect { get { return switchRectCore; } set { switchRectCore = value; } }
		public Size MinSize { get { return minSizeCore; } set { minSizeCore = value; } }
		public int SwitchWidth { get { return switchWidthCore; } set { switchWidthCore = value; } }
		public int TextMargin { get { return textMarginCore; } set { textMarginCore = value; } }
		public int Step { get { return stepCore; } set { stepCore = value; } }
		public string TextOff { get { return textOff; } set { textOff = value; } }
		public string TextOn { get { return textOn; } set { textOn = value; } }
		public bool ShowText { get { return showTextCore; } set { showTextCore = value; } }
		public bool IsAnimation { get { return isAnimationCore; } set { isAnimationCore = value; } }
		public bool IsDrag { get { return isDragCore; } set { isDragCore = value; } }
		public ToggleObjectInfoArgs(AppearanceObject style) : this(null, style) { }
		public ToggleObjectInfoArgs(GraphicsCache cache, AppearanceObject style)
			: base(cache, style) {
			Caption = "Off";
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			ToggleObjectInfoArgs check = info as ToggleObjectInfoArgs;
			if(check == null) return;
			this.IsOn = check.IsOn;
			this.ImageList = check.ImageList;
			this.SwitchRect = check.SwitchRect;
			this.SwitchWidth = check.SwitchWidth;
			this.TextMargin = check.TextMargin;
			this.MinSize = check.MinSize;
			this.TextOff = check.TextOff;
			this.TextOn = check.TextOn;
			this.ShowText = check.ShowText;
			this.IsAnimation = check.IsAnimation;
			this.Step = check.Step;
			this.IsDrag = check.IsDrag;
		}
		ArrayList imageListCore;
		public ArrayList ImageList {
			get { return imageListCore; }
			set { imageListCore = value; }
		}
		bool isOnCore;
		public bool IsOn { get { return isOnCore; } set { isOnCore = value; } }
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			if(!switchRectCore.IsEmpty) switchRectCore.Offset(x, y);			
		}
	}
	public class ToggleObjectPainter : BaseCheckObjectPainter {
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			ToggleObjectInfoArgs ee = e as ToggleObjectInfoArgs;
			base.CalcObjectBounds(ee);			
			Point location = ee.GlyphRect.Location;
			if(location.X < 0) location.X = 0;
			ee.GlyphRect = new Rectangle(location, ee.GlyphRect.Size);
			ee.SwitchRect = CalcSwitchBounds(ee);
			return e.Bounds;
		}
		Rectangle CalcSwitchBounds(ToggleObjectInfoArgs ee) {
			Size switchSize = this.CalcSwitchSize(ee);
			int x = ee.IsOn ? (ee.GlyphRect.Right - switchSize.Width) : 0;
			if(ee.IsDrag || ee.IsAnimation)
				x = ee.Step + ee.SwitchRect.X;
			if(x < ee.GlyphRect.X)
				x = ee.GlyphRect.X;
			if(x > (ee.GlyphRect.Right - switchSize.Width))
				x = ee.GlyphRect.Right - switchSize.Width;
			return new Rectangle(new Point(x, ee.GlyphRect.Y), switchSize);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			ToggleObjectInfoArgs ee = e as ToggleObjectInfoArgs;
			UpdateCheckAppearance(ee);
			Size res = CalcCheckSize(ee);
			Size tSize = CalcCaptionSize(ee, ee.CaptionRect.Width);
			if(tSize.Width > 0) {
				res.Width += tSize.Width + (res.Width > 0 ? ee.TextGlyphIndent : 0);
				res.Height = Math.Max(tSize.Height, res.Height);
			}
			e.Bounds = new Rectangle(Point.Empty, res);
			return e.Bounds;
		}
		protected override int CalcVAlignment(VertAlignment vAlignment, Rectangle bounds, Rectangle caption) {
			if(vAlignment == VertAlignment.Default || vAlignment == VertAlignment.Center) {
				int offset = (bounds.Height - caption.Height) / 2;
				return bounds.Y + offset;
			}
			return base.CalcVAlignment(vAlignment, bounds, caption);
		}
		protected override Size CalcCaptionSize(BaseCheckObjectInfoArgs e, int widthText) {
			if((e as ToggleObjectInfoArgs).ShowText)
				return base.CalcCaptionSize(e, widthText);
			return Size.Empty;
		}
		public override void DrawCheckImage(BaseCheckObjectInfoArgs e) {
			DrawStandardCheck(e);
		}
		protected override Size CalcImageSize(BaseCheckObjectInfoArgs e) {
			ToggleObjectInfoArgs args = e as ToggleObjectInfoArgs;
			if(args == null) return Size.Empty;
			int heigth = args.Appearance.FontHeight + 2 * args.TextMargin;
			int width = (int)Math.Floor(heigth / PercentRatio);
			return new Size(width > args.MinSize.Width ? width : args.MinSize.Width, heigth > args.MinSize.Height ? heigth : args.MinSize.Height);
		}
		protected virtual double PercentRatio { get { return 0.30; } }
		protected virtual Size CalcSwitchSize(ToggleObjectInfoArgs e) {
			Size size = CalcImageSize(e);
			return new Size(size.Width * e.SwitchWidth / 100, size.Height);
		}
		protected override string GetCalcText(BaseCheckObjectInfoArgs e) {
			ToggleObjectInfoArgs args = e as ToggleObjectInfoArgs;
			Size sizeOff = System.Windows.Forms.TextRenderer.MeasureText(args.TextOff, args.Appearance.Font);
			Size sizeOn = System.Windows.Forms.TextRenderer.MeasureText(args.TextOn, args.Appearance.Font);
			if(sizeOff.Width > sizeOn.Width) return args.TextOff;
			return args.TextOn;
		}
		protected override Size CalcHtmlCaptionSize(BaseCheckObjectInfoArgs e, int textWidth) {
			ToggleObjectInfoArgs args = e as ToggleObjectInfoArgs;
			if(args != null){
				Size textOffSize = CalcCaptionSize(args, args.TextOff, textWidth);
				Size textOnSize = CalcCaptionSize(args, args.TextOn, textWidth);
				return textOffSize.Width > textOnSize.Width ? textOffSize : textOnSize;
			}
			return base.CalcHtmlCaptionSize(e, textWidth);
		}
		protected virtual Size CalcCaptionSize(ToggleObjectInfoArgs args, string text, int textWidth) {
			return StringPainter.Default.Calculate(args.Graphics, args.Appearance, null, text, textWidth, null, args.HtmlContext).Bounds.Size;
		}
		protected override Size CalcCheckSize(BaseCheckObjectInfoArgs e) {
			return CalcImageSize(e);
		}
		protected void DrawImage(Graphics g, Image im, Color color, Size realSize, Point location) {
			Image image = ImageStretch(im, realSize, new System.Windows.Forms.Padding(4));
			g.DrawImage(DevExpress.Utils.Helpers.ColoredImageHelper.GetColoredImage(image, color), location);
		}
		protected Image ImageStretch(Image im, Size size, System.Windows.Forms.Padding padding) {
			Bitmap image = new Bitmap(size.Width, size.Height);
			DevExpress.Utils.Helpers.PaintHelper.DrawImage(Graphics.FromImage(image), im, new Rectangle(Point.Empty, size), padding);
			return image;
		}
		protected override void DrawStandardCheck(BaseCheckObjectInfoArgs e) {
			ToggleObjectInfoArgs args = e as ToggleObjectInfoArgs;
			if(args.GlyphRect.Width == 0 || args.GlyphRect.Height == 0) return;
			using(Bitmap image = new Bitmap(e.GlyphRect.Width, e.GlyphRect.Height)) {
				using(Graphics g = Graphics.FromImage(image)) {
					DrawImage(g, (Image)args.ImageList[0], GetColorBorderGround(args.State == ObjectState.Disabled), args.GlyphRect.Size, Point.Empty);
					DrawImage(g, (Image)args.ImageList[1], GetColorBackGround(args.IsOn, args.State == ObjectState.Disabled), Rectangle.Inflate(args.GlyphRect, -3, -3).Size, new Point(3, 3));
					DrawImage(g, (Image)args.ImageList[2], GetColorForeGround(args.State), args.SwitchRect.Size, new Point(args.SwitchRect.X - args.GlyphRect.X, args.SwitchRect.Y - args.GlyphRect.Y));
					args.Cache.Paint.DrawImage(args.Graphics, CropImage(image, args.Bounds), args.GlyphRect.Location);
				}
			}
		}
		protected Image CropImage(Bitmap img, Rectangle crop) {
			int width = img.Width > crop.Width ? crop.Width : img.Width;
			int height = img.Height > crop.Height ? crop.Height : img.Height;
			return img.Clone(new Rectangle(0, 0, width, height), img.PixelFormat);
		}
		protected virtual Color GetColorBackGround(bool isOn, bool disable) {
			if(disable) return Color.FromArgb(240, 240, 240);
			return Color.FromArgb(190, 190, 190);
		}
		protected virtual Color GetColorForeGround(ObjectState state) {
			switch(state) {
				case ObjectState.Hot: return Color.FromArgb(45, 45, 45);
				case ObjectState.Disabled: return Color.FromArgb(160, 160, 160);
			}
			return Color.FromArgb(0, 0, 0);
		}
		protected virtual Color GetColorBorderGround(bool disable) {
			return Color.FromArgb(160, 160, 160);
		}
	}
	public class FlatToggleObjectPainter : ToggleObjectPainter { }
	public class Office2003ToggleObjectPainter : ToggleObjectPainter {
		protected override Color GetColorBackGround(bool isOn, bool disable) {
			if(disable) return Color.FromArgb(238, 238, 238);
			return Color.FromArgb(174, 179, 185);
		}
		protected override Color GetColorForeGround(ObjectState state) {
			switch(state) {
				case ObjectState.Hot: return Color.FromArgb(4, 34, 113);
				case ObjectState.Pressed: return Color.FromArgb(54, 89, 152);
				case ObjectState.Disabled: return Color.FromArgb(187, 187, 187);
			}
			return Color.FromArgb(73, 94, 150);
		}
		protected override Color GetColorBorderGround(bool disable) {
			return disable ? Color.FromArgb(177, 177, 177) : Color.FromArgb(142, 143, 143);
		}
	}
	public class WindowsXPToggleObjectPainter : Office2003ToggleObjectPainter { }
	public class TogglePainterHelper {
		[ThreadStatic]
		static ToggleObjectPainter[] painters;
		static ToggleObjectPainter[] Painters {
			get {
				if(painters == null) CreatePainters();
				return painters;
			}
		}
		static void CreatePainters() {
			painters = new ToggleObjectPainter[Enum.GetValues(typeof(ActiveLookAndFeelStyle)).Length];
			painters[(int)ActiveLookAndFeelStyle.WindowsXP] = new WindowsXPToggleObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.Office2003] = new Office2003ToggleObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.Style3D] = new ToggleObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.UltraFlat] = new FlatToggleObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.Flat] = new FlatToggleObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.Skin] = new SkinToggleObjectPainter(null);
		}
		public static ToggleObjectPainter GetPainter(ActiveLookAndFeelStyle style) {
			if(style == ActiveLookAndFeelStyle.Skin) return new SkinToggleObjectPainter(UserLookAndFeel.Default);
			return Painters[(int)style];
		}
		public static ToggleObjectPainter GetPainter(UserLookAndFeel lookAndFeel) {
			ActiveLookAndFeelStyle style = lookAndFeel.ActiveStyle;
			if(style == ActiveLookAndFeelStyle.Skin) return new SkinToggleObjectPainter(lookAndFeel.ActiveLookAndFeel);
			return Painters[(int)style];
		}
	}
	public class SkinToggleObjectPainter : ToggleObjectPainter {
		ISkinProvider provider;
		public SkinToggleObjectPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public ISkinProvider Provider { get { return provider; } }
		protected override void DrawStandardCheck(BaseCheckObjectInfoArgs e) {
			ToggleObjectInfoArgs args = e as ToggleObjectInfoArgs;
			if(args.GlyphRect.Width == 0 || args.GlyphRect.Height == 0) return;
			using(Bitmap image = new Bitmap(args.GlyphRect.Width, args.GlyphRect.Height)) {
				using(Graphics g = Graphics.FromImage(image)) {
					GraphicsCache savedCache = args.Cache;
					using(GraphicsCache cache = new GraphicsCache(g)) {
						args.Cache = cache;
						DrawToggleSwitch(args);
						DrawToggleSwitchThumb(args);
					}
					args.Cache = savedCache;
					args.Cache.Paint.DrawImage(args.Graphics, CropImage(image, args.Bounds), args.GlyphRect.Location);
				}
			}
		}
		protected virtual void DrawToggleSwitch(ToggleObjectInfoArgs args) {
			if(args.IsDrag || args.IsAnimation) {
				SkinElementPainter.Default.DrawObject(GetToggleSwitchSkinElementInfo(args, DefaultBoolean.False));
				SkinElementPainter.Default.DrawObject(GetToggleSwitchSkinElementInfo(args, DefaultBoolean.True));
				return;
			}
			SkinElementPainter.Default.DrawObject(GetToggleSwitchSkinElementInfo(args, DefaultBoolean.Default));
		}
		protected virtual void DrawToggleSwitchThumb(ToggleObjectInfoArgs args) {
			SkinElementPainter.Default.DrawObject(GetToggleSwitchThumbSkinElementInfo(args));
		}
		protected SkinElementInfo GetToggleSwitchSkinElementInfo(ToggleObjectInfoArgs e, DefaultBoolean IsOn) {
			Size size = e.GlyphRect.Size;
			bool on = IsOn == DefaultBoolean.True;
			if(IsOn == DefaultBoolean.True) size = new Size(e.SwitchRect.X - e.GlyphRect.X + e.SwitchRect.Width / 2, e.GlyphRect.Height);			 
			if(IsOn == DefaultBoolean.Default) on = e.IsOn;
			Rectangle bounds = new Rectangle(Point.Empty, size);
			SkinElementInfo info = GetSkinElementInfo(e, EditorsSkins.SkinToggleSwitch, bounds);
			info.Graphics.Clip = new Region(new Rectangle(Point.Empty, size));
			info.ImageIndex = CalcImageIndex(info.Element.Image, e.State, on);
			return info;
		}
		protected SkinElementInfo GetToggleSwitchThumbSkinElementInfo(ToggleObjectInfoArgs e) {
			Rectangle bounds = new Rectangle(new Point(e.SwitchRect.X - e.GlyphRect.X, e.SwitchRect.Y - e.GlyphRect.Y), e.SwitchRect.Size);
			SkinElementInfo info = GetSkinElementInfo(e, EditorsSkins.SkinToggleSwitchThumb, bounds);
			info.Cache.Graphics.Clip = new Region(bounds);
			info.ImageIndex = SkinElementPainter.Default.CalcDefaultImageIndex(info.Element.Image, e.State);
			return info;
		}
		protected SkinElement GetSkinElement(string name) {
			return EditorsSkins.GetSkin(Provider)[name];
		}
		protected SkinElementInfo GetSkinElementInfo(ToggleObjectInfoArgs e, string name, Rectangle bounds) {
			SkinElementInfo info = new SkinElementInfo(GetSkinElement(name), bounds);
			info.State = e.State;
			info.Cache = e.Cache;
			return info;
		}
		public int CalcImageIndex(SkinImage skinImage, ObjectState state, bool isOn) {
			if(skinImage != null && skinImage.ImageCount > 1) {
				if(state == ObjectState.Disabled) return 2;
				if(isOn)
					return 1;
			}
			return 0;
		}
		protected override bool IsAllowDrawShadedCaption { get { return false; } }
	}
}
