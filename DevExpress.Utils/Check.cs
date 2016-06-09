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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel.Design;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
namespace DevExpress.XtraEditors.Controls {
	public enum StyleIndeterminate { Unchecked, Inactive, InactiveChecked }
	public enum CheckStyles {
		Standard = -2,
		Radio = -1,
		Style1 = 0, Style2 = 1, Style3 = 2, Style4 = 3, Style5 = 4,
		Style6 = 5, Style7 = 6, Style8 = 7, Style9 = 8, Style10 = 9,
		Style11 = 10, Style12 = 11, Style13 = 12, Style14 = 13, Style15 = 14,
		Style16 = 15,
		UserDefined = 16
	}
}
namespace DevExpress.Utils.Drawing {
	public class BaseCheckObjectInfoArgs : StyleObjectInfoArgs {
		public int TextGlyphIndent = 3;
		UserLookAndFeel lookAndFeel;
		Rectangle captionRect;
		string caption;
		bool showHotKeyPrefix;
		HorzAlignment glyphAlignment;
		VertAlignment glyphVAlignment;
		bool autoHeight;
		bool allowHtmlString;
		DevExpress.Utils.Text.StringInfo stringInfo;
		Rectangle glyphRect;
		ArrayList defaultImages;
		public BaseCheckObjectInfoArgs(AppearanceObject style) : this(null, style) { }
		public BaseCheckObjectInfoArgs(GraphicsCache cache, AppearanceObject style)
			: base(cache) {
			this.SetAppearance(style);
			this.caption = "";
			this.showHotKeyPrefix = true;
			this.glyphAlignment = HorzAlignment.Near;
			this.glyphVAlignment = VertAlignment.Default;
			this.GlyphRect = this.captionRect = Rectangle.Empty;
			this.allowHtmlString = false;
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			BaseCheckObjectInfoArgs check = info as BaseCheckObjectInfoArgs;
			if(check == null) return;
			this.lookAndFeel = check.lookAndFeel;
			this.defaultImages = check.defaultImages;
			this.GlyphRect = check.GlyphRect;
			this.allowHtmlString = check.allowHtmlString;
			this.captionRect = check.captionRect; ;
			this.caption = check.caption;
			this.showHotKeyPrefix = check.showHotKeyPrefix;
			this.glyphAlignment = check.glyphAlignment;
			this.glyphVAlignment = check.glyphVAlignment;
			this.autoHeight = check.autoHeight;
		}
		public Rectangle GlyphRect { get { return glyphRect; } set { glyphRect = value; } }
		public object HtmlContext { get; set; }
		public DevExpress.Utils.Text.StringInfo StringInfo { get { return stringInfo; } set { stringInfo = value; } }
		public bool AllowHtmlString { get { return allowHtmlString; } set { allowHtmlString = value; } }
		public bool AutoHeight { get { return autoHeight; } set { autoHeight = value; } }
		public virtual bool ShowHotKeyPrefix { get { return showHotKeyPrefix; } set { showHotKeyPrefix = value; } }
		public override void OffsetContent(int x, int y) {
			base.OffsetContent(x, y);
			if(!glyphRect.IsEmpty) glyphRect.Offset(x, y);
			if(!captionRect.IsEmpty) captionRect.Offset(x, y);
			if(StringInfo != null) StringInfo.Offset(x, y);
		}
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } set { lookAndFeel = value; } }
		public Rectangle CaptionRect {
			get { return captionRect; }
			set {
				captionRect = value;
			}
		}
		public virtual ArrayList DefaultImages { get { return defaultImages; } set { defaultImages = value; } }
		public string Caption { get { return caption; } set { caption = value; } }
		public HorzAlignment GlyphAlignment { get { return glyphAlignment; } set { glyphAlignment = value; } }
		public VertAlignment GlyphVAlignment { get { return glyphVAlignment; } set { glyphVAlignment = value; } }
		public HorzAlignment GetGlyphAlignment()  {
			if(RightToLeft) {
				if(GlyphAlignment == HorzAlignment.Near) return HorzAlignment.Far;
				if(GlyphAlignment == HorzAlignment.Far) return HorzAlignment.Near;
			}
			return GlyphAlignment;
		}
		public TextGlyphDrawModeEnum GlyphDrawMode {
			get {
				if(GetGlyphAlignment() == HorzAlignment.Center) return TextGlyphDrawModeEnum.Glyph;
				return TextGlyphDrawModeEnum.TextGlyph;
			}
		}
	}
	public class CheckObjectInfoArgs : BaseCheckObjectInfoArgs  {	   
		CheckStyles checkStyle;
		StyleIndeterminate nullStyle;
		CheckState checkState;  
		object images;
		Image pictureChecked, pictureUnchecked, pictureGrayed;
		int imageIndexChecked, imageIndexUnchecked, imageIndexGrayed;
		public CheckObjectInfoArgs(AppearanceObject style) : this(null, style) { }
		public CheckObjectInfoArgs(GraphicsCache cache, AppearanceObject style) : base(cache, style) {			
			this.SetAppearance(style);
			this.nullStyle = StyleIndeterminate.Inactive;
			this.checkState = CheckState.Unchecked;
			this.checkStyle = CheckStyles.Standard;			
		}
		public override void Assign(ObjectInfoArgs info) {
			base.Assign(info);
			CheckObjectInfoArgs check = info as CheckObjectInfoArgs;
			if(check == null) return;
			this.pictureChecked = check.pictureChecked;
			this.pictureUnchecked = check.pictureUnchecked;
			this.pictureGrayed = check.pictureGrayed;
			this.checkStyle = check.checkStyle;
			this.nullStyle = check.nullStyle;
			this.checkState = check.checkState;
			this.imageIndexChecked = check.imageIndexChecked;
			this.imageIndexUnchecked = check.imageIndexUnchecked;
			this.imageIndexGrayed = check.imageIndexGrayed;
			this.images = check.images;
		}
		public CheckStyles CheckStyle { get { return checkStyle; } set { checkStyle = value; } }
		public object Images { get { return images; } set { images = value; } }
		public int ImageIndexChecked { get { return imageIndexChecked; } set { imageIndexChecked = value; } }
		public int ImageIndexUnchecked { get { return imageIndexUnchecked; } set { imageIndexUnchecked = value; } }
		public int ImageIndexGrayed { get { return imageIndexGrayed; } set { imageIndexGrayed = value; } }
		protected Image GetImage(Image img, int imageIndex) {
			if(img != null)
				return img;
			return ImageCollection.GetImageListImage(Images, imageIndex);
		}
		public virtual CheckState CheckState { get { return checkState; } set { checkState = value; } }
		public StyleIndeterminate NullStyle { get { return nullStyle; } set { this.nullStyle = value; } }
		public Image PictureChecked { get { return pictureChecked; } set { pictureChecked = value; } }
		public Image PictureGrayed { get { return pictureGrayed; } set { pictureGrayed = value; } }
		public Image PictureUnchecked { get { return pictureUnchecked; } set { pictureUnchecked = value; } }
		public Image GetCheckedGlyph() { return GetImage(PictureChecked, ImageIndexChecked); }
		public Image GetUncheckedGlyph() { return GetImage(PictureUnchecked, ImageIndexUnchecked); }
		public Image GetGrayedGlyph() { return GetImage(PictureGrayed, ImageIndexGrayed); }
	}
	public class BaseCheckObjectPainter : StyleObjectPainter {
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			BaseCheckObjectInfoArgs ee = e as BaseCheckObjectInfoArgs;
			UpdateCheckAppearance(ee);
			ee.CaptionRect = ee.GlyphRect = Rectangle.Empty;
			ee.StringInfo = null;
			Rectangle glyph = e.Bounds;
			Size cSize = Size.Empty;
			glyph.Size = CalcCheckSize(ee);
			if(ee.GlyphDrawMode == TextGlyphDrawModeEnum.Glyph) {
				int offsetX = (e.Bounds.Width - glyph.Width) / 2;
				glyph.X = glyph.X + (offsetX > 0 ? offsetX : 0);
			}
			else {
				int minWidth = e.Bounds.Width - (glyph.Width + ee.TextGlyphIndent);
				cSize = CalcCaptionSize(ee, minWidth);
				Rectangle caption = new Rectangle(e.Bounds.Location, cSize);
				caption.Width = minWidth;
				if(ee.GetGlyphAlignment() == HorzAlignment.Far) {
					glyph.X = e.Bounds.Right - glyph.Width;
					caption.X = e.Bounds.X;
				}
				else {
					caption.X = glyph.Right + ee.TextGlyphIndent;
				}
				caption.Width = Math.Min(cSize.Width, caption.Width);
				caption = CalcCaptionRect(ee, ee.Bounds, caption, glyph);
				ee.CaptionRect = caption;
				if(ee.AllowHtmlString) CalcHtmlCaptionBounds(ee);
			}
			UpdateGlyphY(ref glyph, cSize, ee);
			ee.GlyphRect = glyph;
			return e.Bounds;
		}
		protected virtual void UpdateGlyphY(ref Rectangle glyph, Size cSize, BaseCheckObjectInfoArgs e) {
			int offsetY = 0;
			if(e.GlyphVAlignment == VertAlignment.Top)
				offsetY = Math.Max((cSize.Height - glyph.Height) / 2, 0);
			else if(e.GlyphVAlignment == VertAlignment.Bottom) 
				offsetY = e.Bounds.Height - glyph.Height - (cSize.Height > 0 ? (cSize.Height - glyph.Height) / 2 : 0);
			else
				offsetY = (e.Bounds.Height - glyph.Height) / 2;
			glyph.Y = glyph.Y + (offsetY > 0 ? offsetY : 0);
		}
		public virtual void DrawSimpleCaption(BaseCheckObjectInfoArgs ee) {
			Brush brush = ee.Appearance.GetForeBrush(ee.Cache);
			System.Drawing.Text.HotkeyPrefix prev = ee.Appearance.GetStringFormat().HotkeyPrefix;
			StringFormat strFormat = ee.Appearance.GetStringFormat();
			strFormat.HotkeyPrefix = ee.ShowHotKeyPrefix ? System.Drawing.Text.HotkeyPrefix.Show : System.Drawing.Text.HotkeyPrefix.Hide;
			Rectangle r = ee.CaptionRect;
			if(ee.State == ObjectState.Disabled && IsAllowDrawShadedCaption) {
				Brush dbrush = SystemBrushes.ControlLightLight;
				r.Offset(1, 1);
				if(ee.IsDrawOnGlass && NativeVista.IsVista)
					ObjectPainter.DrawTextOnGlass(ee.Graphics, ee.Appearance.Font, ee.Caption, r, strFormat, SystemColors.ControlLightLight);
				else
					ee.Cache.DrawString(ee.Caption, ee.Appearance.Font, dbrush, r, strFormat);
				r.Offset(-1, -1);
			}
			if(ee.IsDrawOnGlass && NativeVista.IsVista)
				ObjectPainter.DrawTextOnGlass(ee.Graphics, ee.Appearance, ee.Caption, r, strFormat);
			else
				ee.Cache.DrawString(ee.Caption, ee.Appearance.Font, brush, r, strFormat);
			strFormat.HotkeyPrefix = prev;
		}
		protected virtual void CalcHtmlCaptionBounds(BaseCheckObjectInfoArgs ee) {
			ee.StringInfo = StringPainter.Default.Calculate(ee.Graphics, ee.Appearance, null, EditorsSkins.GetSkin(ee.LookAndFeel.ActiveLookAndFeel).Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor), ee.Caption, ee.CaptionRect, null, ee.HtmlContext);
		}
		protected virtual int CalcVAlignment(VertAlignment vAlignment, Rectangle bounds, Rectangle caption) {
			if(vAlignment == VertAlignment.Default || vAlignment == VertAlignment.Center) {
				int offset = (bounds.Height - caption.Height + 1) / 2;
				return bounds.Y + offset;
			}
			if(vAlignment == VertAlignment.Bottom)
				return bounds.Bottom - caption.Height;
			return caption.Y;
		}
		protected virtual int CalcHAlignment(BaseCheckObjectInfoArgs ee, Rectangle bounds, Rectangle caption, Rectangle glyph) {			
			if(ee.Appearance.TextOptions.HAlignment == HorzAlignment.Center)
				return caption.X + (bounds.Width - glyph.Width - caption.Width) / 2;
			if(ee.Appearance.TextOptions.HAlignment == HorzAlignment.Far || (ee.RightToLeft && ee.Appearance.TextOptions.HAlignment != HorzAlignment.Far)) {
				int right = (glyph.X > caption.X ? glyph.X : bounds.Right) - ee.TextGlyphIndent;
				return right - caption.Width;
			}
			return caption.X;
		}
		protected virtual Rectangle CalcCaptionRect(BaseCheckObjectInfoArgs ee, Rectangle bounds, Rectangle caption, Rectangle glyph) {
			Rectangle res = caption;
			res.Y = CalcVAlignment(ee.Appearance.TextOptions.VAlignment, bounds, caption);
			if(res.Width + glyph.Width >= bounds.Width) return res;
			res.X = CalcHAlignment(ee, bounds, caption, glyph);
			if(res.Y < caption.Y) res.Y = caption.Y; 
			return res;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) {
			return e.Bounds;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return client;
		}
		protected void UpdateCheckAppearance(BaseCheckObjectInfoArgs ee) {
			bool isNeedToClone = ee.Appearance.TextOptions.HotkeyPrefix != GetHKeyPrefix(ee);
			if(isNeedToClone) {
				ee.SetAppearance(ee.Appearance.Clone() as AppearanceObject);
				ee.Appearance.TextOptions.HotkeyPrefix = GetHKeyPrefix(ee);
			}
		}
		HKeyPrefix GetHKeyPrefix(BaseCheckObjectInfoArgs ee) {
			return (ee.ShowHotKeyPrefix ? HKeyPrefix.Show : HKeyPrefix.Hide);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			BaseCheckObjectInfoArgs ee = e as BaseCheckObjectInfoArgs;
			DrawCheckImage(ee);
			DrawCaption(ee);
		}
		public virtual void DrawCheckImage(BaseCheckObjectInfoArgs e) { }
		public virtual void DrawHtmlCaption(BaseCheckObjectInfoArgs e) {
			StringPainter.Default.UpdateLocation(e.StringInfo, e.CaptionRect);
			StringPainter.Default.DrawString(e.Cache, e.StringInfo);
		}
		protected virtual bool IsAllowDrawShadedCaption { get { return true; } }
		protected virtual void DrawStandardCheck(BaseCheckObjectInfoArgs e) { }
		public virtual void DrawCaption(ObjectInfoArgs e) {
			BaseCheckObjectInfoArgs ee = e as BaseCheckObjectInfoArgs;
			if(ee.CaptionRect.IsEmpty) return;
			ee.Appearance.TextOptions.RightToLeft = ee.RightToLeft;
			if(ee.AllowHtmlString)
				DrawHtmlCaption(ee);
			else
				DrawSimpleCaption(ee);
		}
		protected virtual Size CalcImageSize(BaseCheckObjectInfoArgs e) { return Size.Empty; }
		protected virtual Size CalcCheckSize(BaseCheckObjectInfoArgs e) { return Size.Empty; }
		protected virtual bool CanDrawCaption(BaseCheckObjectInfoArgs e) {
			return (e.GlyphDrawMode != TextGlyphDrawModeEnum.Glyph);
		}
		protected virtual Size CalcHtmlCaptionSize(BaseCheckObjectInfoArgs e, int textWidth) {
			return StringPainter.Default.Calculate(e.Graphics, e.Appearance, null, GetCalcText(e), textWidth, null, e.HtmlContext).Bounds.Size;
		}
		protected virtual Size CalcSimpleCaptionSize(BaseCheckObjectInfoArgs e, int textWidth) {
			Size res = Size.Empty;
			if(!CanDrawCaption(e)) return res;
			res = e.Appearance.CalcTextSize(e.Graphics, GetCalcText(e), textWidth).ToSize();			
			return res;
		}
		protected virtual string GetCalcText(BaseCheckObjectInfoArgs e) {
			return e.Caption;
		}
		protected virtual Size CalcCaptionSize(BaseCheckObjectInfoArgs e, int widthText) {
			if(!CanDrawCaption(e)) return Size.Empty;
			int textWidth = e.Appearance.TextOptions.WordWrap == WordWrap.Wrap ? widthText : 0;
			if(e.AllowHtmlString)
				return CalcHtmlCaptionSize(e, textWidth);
			return CalcSimpleCaptionSize(e, textWidth);
		}
	}
	public class CheckObjectPainter : BaseCheckObjectPainter {
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			CheckObjectInfoArgs ee = e as CheckObjectInfoArgs;
			UpdateGlyphs(ee);		   
			return base.CalcObjectBounds(ee);
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			CheckObjectInfoArgs ee = e as CheckObjectInfoArgs;
			UpdateCheckAppearance(ee);
			UpdateGlyphs(ee);
			Size kSize = CalcCheckSize(ee);
			Size tSize = CalcCaptionSize(ee, ee.CaptionRect.Width);
			if(ee.CheckStyle == CheckStyles.Standard && ee.LookAndFeel != null && ee.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.WindowsXP &&
				NativeVista.IsVista && !tSize.IsEmpty && kSize.Height > tSize.Height) kSize = new Size(tSize.Height, tSize.Height);
			Size res = kSize;
			if(tSize.Width > 0) {
				res.Width += tSize.Width + (kSize.Width > 0 ? ee.TextGlyphIndent : 0);
				res.Height = Math.Max(tSize.Height, res.Height);
			}
			e.Bounds = new Rectangle(Point.Empty, res);
			return e.Bounds;
		}			
		protected virtual ButtonState CalcButtonIndeterminateState(CheckObjectInfoArgs e) {
			ButtonState bs = ButtonState.Inactive;
			switch(e.NullStyle) {
				case StyleIndeterminate.Unchecked:
					bs = ButtonState.Normal;
					break;
				case StyleIndeterminate.InactiveChecked:
					bs = ButtonState.Inactive | ButtonState.Checked;
					break;
			}
			return bs;
		}
		protected virtual ButtonState CalcButtonState(CheckObjectInfoArgs e) {
			ButtonState bs = ButtonState.Normal;
			switch(e.CheckState) {
				case CheckState.Checked :
					bs = ButtonState.Checked; break;
				case CheckState.Indeterminate:
					bs = CalcButtonIndeterminateState(e);
					break;
			}
			if(e.State == ObjectState.Disabled) {
				bs |= ButtonState.Inactive;
			} else {
				if((e.State & (ObjectState.Pressed | ObjectState.Hot)) == (ObjectState.Pressed | ObjectState.Hot)) bs |= ButtonState.Pushed;
			}
			return bs;
		}
		protected override void DrawStandardCheck(BaseCheckObjectInfoArgs e) {
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			ButtonState state = CalcButtonState(args);
			if(args.CheckStyle == CheckStyles.Standard)
				args.Paint.DrawCheckBox(args.Graphics, args.GlyphRect, state);
			else
				args.Paint.DrawRadioButton(args.Graphics, args.GlyphRect, state);
		}
		public override void DrawCheckImage(BaseCheckObjectInfoArgs e) {
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			switch(args.CheckStyle) {
				case CheckStyles.Standard :
				case CheckStyles.Radio:
					DrawStandardCheck(args);
					return;
			}
			Image image = GetGlyph(args);
			if(image == null) return;
			bool drawDisabled = args.State == ObjectState.Disabled;
			if(args.CheckState == CheckState.Indeterminate && args.GetGrayedGlyph() == null) drawDisabled = true;
			args.Cache.Paint.DrawImage(args.Graphics, image, e.GlyphRect, new Rectangle(Point.Empty, image.Size), !drawDisabled);
		}
		public static bool RequireDefaultImages(CheckObjectInfoArgs e) {
			int style = (int)e.CheckStyle;
			return (style >= 0 && e.CheckStyle != CheckStyles.UserDefined);
		}
		protected virtual void UpdateGlyphs(CheckObjectInfoArgs e) {
			int style = (int)e.CheckStyle;
			if(!RequireDefaultImages(e)) return;
			style = style * 4;
			if(e.DefaultImages == null || style > e.DefaultImages.Count - 1) return;
			e.PictureUnchecked = e.DefaultImages[style] as Bitmap;
			e.PictureChecked = e.DefaultImages[style + 1] as Bitmap;
			e.PictureGrayed = e.DefaultImages[style + 3] as Bitmap;
		}
		protected virtual Image GetGlyph(BaseCheckObjectInfoArgs e) {
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			switch(args.CheckState) {
				case CheckState.Checked:
					return args.GetCheckedGlyph();
				case CheckState.Unchecked:
					return args.GetUncheckedGlyph();
				case CheckState.Indeterminate:
					if(args.GetGrayedGlyph() == null) return args.GetUncheckedGlyph();
					return args.GetGrayedGlyph();
			}
			return null;
		}
		protected override Size CalcImageSize(BaseCheckObjectInfoArgs e) {
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			Size size = Size.Empty;
			if(args.GetCheckedGlyph() != null) {
				size.Width = Math.Max(size.Width, args.GetCheckedGlyph().Width);
				size.Height = Math.Max(size.Height, args.GetCheckedGlyph().Height);
			}
			if(args.GetGrayedGlyph() != null) {
				size.Width = Math.Max(size.Width, args.GetGrayedGlyph().Width);
				size.Height = Math.Max(size.Height, args.GetGrayedGlyph().Height);
			}
			if(args.GetUncheckedGlyph() != null) {
				size.Width = Math.Max(size.Width, args.GetUncheckedGlyph().Width);
				size.Height = Math.Max(size.Height, args.GetUncheckedGlyph().Height);
			}
			return size;
		}
		protected override Size CalcCheckSize(BaseCheckObjectInfoArgs e) {
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			Size size = new Size(13, 13);
			switch(args.CheckStyle) {
				case CheckStyles.Standard : 
				case CheckStyles.Radio:
					return size;
			}
			Size glyphSize = CalcImageSize(args);
			return glyphSize;
		}
	}
	public class FlatCheckObjectPainter : CheckObjectPainter {
		protected override ButtonState CalcButtonState(CheckObjectInfoArgs e) {
			ButtonState bs = base.CalcButtonState(e) | ButtonState.Flat;
			return bs;
		}
	}
	public class Office2003CheckObjectPainter : WindowsXPCheckObjectPainter {
	}
	public class WindowsXPCheckObjectPainter : FlatCheckObjectPainter {
		protected XPCheckBoxPainter fPainter;
		protected XPCheckBoxInfoArgs fInfoArgs;
		public WindowsXPCheckObjectPainter() {
			this.fPainter = new XPCheckBoxPainter();
			this.fInfoArgs = new XPCheckBoxInfoArgs(false, CheckState.Unchecked);
		}
		protected virtual void UpdateCheckInfo(ObjectInfoArgs e) {
			CheckObjectInfoArgs ee = e as CheckObjectInfoArgs;
			fInfoArgs.IsRadioButton = (ee.CheckStyle == CheckStyles.Radio);
			fInfoArgs.CheckState = CalcCheckState(ee);
			fInfoArgs.State = ee.State;
			fInfoArgs.Bounds = ee.GlyphRect;
			fInfoArgs.Cache = ee.Cache;
		}
		protected virtual CheckState CalcCheckState(CheckObjectInfoArgs e) {
			if(e.CheckState != CheckState.Indeterminate) return e.CheckState;
			switch(e.NullStyle) {
				case StyleIndeterminate.Unchecked : return CheckState.Unchecked;
			}
			return CheckState.Indeterminate;
		}
		protected XPCheckBoxInfoArgs InfoArgs { get { return fInfoArgs; } }
		protected XPCheckBoxPainter CheckPainter { get { return fPainter; } }
		protected override Size CalcCheckSize(BaseCheckObjectInfoArgs e) {
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			if(DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled) {
				switch(args.CheckStyle) {
					case CheckStyles.Standard : 
					case CheckStyles.Radio:
						UpdateCheckInfo(args);
						return CheckPainter.CalcObjectMinBounds(InfoArgs).Size;
				}
			}
			return base.CalcCheckSize(e);
		}
		protected override void DrawStandardCheck(BaseCheckObjectInfoArgs e) {
			if(DevExpress.Utils.WXPaint.WXPPainter.Default.ThemesEnabled) {
				UpdateCheckInfo(e);
				CheckPainter.DrawObject(InfoArgs);
				InfoArgs.Cache = null;
			} else {
				base.DrawStandardCheck(e);
			}
		}
	}
	public class CheckPainterHelper {
		[ThreadStatic]
		static CheckObjectPainter[] painters;
		static CheckObjectPainter[] Painters {
			get {
				if(painters == null) CreatePainters();
				return painters;
			}
		}
		static void CreatePainters() {
			painters = new CheckObjectPainter[Enum.GetValues(typeof(ActiveLookAndFeelStyle)).Length];
			painters[(int)ActiveLookAndFeelStyle.WindowsXP] = new WindowsXPCheckObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.Office2003] = new Office2003CheckObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.Style3D] = new CheckObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.UltraFlat] = new FlatCheckObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.Flat] = new FlatCheckObjectPainter();
			painters[(int)ActiveLookAndFeelStyle.Skin] = new SkinCheckObjectPainter(null);
		}
		public static CheckObjectPainter GetPainter(ActiveLookAndFeelStyle style) {
			if(style == ActiveLookAndFeelStyle.Skin) return new SkinCheckObjectPainter(UserLookAndFeel.Default);
			return Painters[(int)style];
		}
		public static CheckObjectPainter GetPainter(UserLookAndFeel lookAndFeel) {
			ActiveLookAndFeelStyle style = lookAndFeel.ActiveStyle;
			if(style == ActiveLookAndFeelStyle.Skin) return new SkinCheckObjectPainter(lookAndFeel.ActiveLookAndFeel);
			return Painters[(int)style];
		}
	}
	public class SkinCheckObjectPainter : CheckObjectPainter {
		ISkinProvider provider;
		public SkinCheckObjectPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public ISkinProvider Provider { get { return provider; } }
		protected override void DrawStandardCheck(BaseCheckObjectInfoArgs e) {
			SkinElementPainter.Default.DrawObject(GetInfo(e));
		}
		protected override bool IsAllowDrawShadedCaption { get { return false; } }
		protected SkinElementInfo GetInfo(BaseCheckObjectInfoArgs e) {
			string name = EditorsSkins.SkinCheckBox;
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			if(args.CheckStyle == CheckStyles.Radio) name = EditorsSkins.SkinRadioButton;
			SkinElementInfo info = new SkinElementInfo(EditorsSkins.GetSkin(Provider)[name], args.GlyphRect);
			info.State = args.State;
			info.ImageIndex = SkinElementPainter.Default.CalcDefaultImageIndex(info.Element.Image, args.State);
			info.Cache = args.Cache;
			switch(CalcCheckState(args)) {
				case CheckState.Checked : info.ImageIndex += 4; break;
				case CheckState.Indeterminate : info.ImageIndex += 8; break;
			}
			return info;
		}
		protected override Size CalcCheckSize(BaseCheckObjectInfoArgs e) {
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			switch(args.CheckStyle) {
				case CheckStyles.Standard : 
				case CheckStyles.Radio:
					return SkinElementPainter.Default.CalcObjectMinBounds(GetInfo(args)).Size;
			}
			return base.CalcCheckSize(e);
		}
		protected virtual CheckState CalcCheckState(BaseCheckObjectInfoArgs e) {
			CheckObjectInfoArgs args = e as CheckObjectInfoArgs;
			if(args.CheckState != CheckState.Indeterminate) return args.CheckState;
			switch(args.NullStyle) {
				case StyleIndeterminate.Unchecked : return CheckState.Unchecked;
			}
			return CheckState.Indeterminate;
		}
	}
}
