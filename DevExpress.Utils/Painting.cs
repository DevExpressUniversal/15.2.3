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
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Security;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.Utils.Text;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.Utils.Paint {
	#region XPaint
	public enum DXDashStyle {
		Default = -2, 
		None = -1, 
		Solid = 0,
		Dash = 1,
		Dot = 2,
		DashDot = 3,
		DashDotDot = 4,
	}
	public class XPaint : IDisposable {
		public static bool DefaultAllowHtmlDraw = false;
		public static DXDashStyle FocusRectStyle = DXDashStyle.Default;
		public static XPaint CreateDefaultPainter() {
			switch(Environment.OSVersion.Platform) {
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
					return new XPaint();
				default:
					return new XPaintMixed();
			}
		}
		static XPaint graphics;
		public static XPaint Graphics {
			get { 
				if(graphics == null) graphics = CreateDefaultPainter(); 
				return graphics;
			}
		}
		public static void CreateCustomPainter(XPaint painter) {
			graphics = painter;
		}
		public static void ForceTextRenderPaint() {
			graphics = new XPaintTextRender();
		}
		public static XPaint GetCurrentPaint() {
			return graphics;
		}
		public static void RestorePaint(XPaint paint) {
			if(paint != null)
				graphics = paint;
		}
		public static void ForceGDIPlusPaint() {
			graphics = new XPaint();
		}
		public static void ForceAPIPaint() {
			graphics = new XPaintMixed();
		}
		public static Size TextSizeRound(SizeF size) {
			return new Size(TextSizeRound(size.Width), TextSizeRound(size.Height)); 
		}
		public static Rectangle TextSizeRound(RectangleF rect) {
			return new Rectangle((int)rect.X, (int)rect.Y, TextSizeRound(rect.Width), TextSizeRound(rect.Height)); 
		}
		public static int TextSizeRound(float size) {
			int res = (int)size;
			if(res != size) res ++;
			return res;
		}
		[ThreadStatic]
		static ImageAttributes disabledImageAttr;
		public static ImageAttributes DisabledImageAttr {
			get {
				if(disabledImageAttr == null) {
					disabledImageAttr = CreateImageAttributes(CreateDisabledImageColorMatrix());
				}
				return disabledImageAttr;
			}
		}
		[ThreadStatic]
		static ColorMatrix disabledImageColorMatrix;
		static ColorMatrix CreateDisabledImageColorMatrix() {
			float[][] array = new float[5][];
			array[0] = new float[5] { 0.2125f, 0.2125f, 0.2125f, 0, 0 };
			array[1] = new float[5] { 0.2577f, 0.2577f, 0.2577f, 0, 0 };
			array[2] = new float[5] { 0.0361f, 0.0361f, 0.0361f, 0, 0 };
			array[3] = new float[5] { 0, 0, 0, 1, 0 };
			array[4] = new float[5] { 0.38f, 0.38f, 0.38f, 0, 1 };
			return new ColorMatrix(array);
		}
		static ImageAttributes CreateImageAttributes(ColorMatrix matrix) {
			ImageAttributes attr = new ImageAttributes();
			attr.ClearColorKey();
			attr.SetColorMatrix(matrix);
			return attr;
		}
		public static ImageAttributes GetDisabledImageAttrWithOpacity(float opacity) {
			if(disabledImageColorMatrix == null) {
				disabledImageColorMatrix = CreateDisabledImageColorMatrix();
			}
			disabledImageColorMatrix.Matrix33 = opacity;
			return CreateImageAttributes(disabledImageColorMatrix);
		}
		public static ImageAttributes CreateImageAttributesWithOpacity(float opacity) {
			ColorMatrix colormatrix = new ColorMatrix();
			colormatrix.Matrix33 = opacity;
			ImageAttributes imgAttribute = new ImageAttributes();
			imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			return imgAttribute;
		}
		public static ColorMatrix GetColorMatrix(Color baseColor, Color currentColor) {
			ColorMatrix res = new ColorMatrix();
			if(baseColor != currentColor) {
				int sum = baseColor.R + baseColor.G + baseColor.B;
				if(sum == 0) {
					sum = currentColor.R + currentColor.G + currentColor.B;
					if(sum == 0)
						sum = 3 * 128;
				}
				float fl1 = ((float)currentColor.R) / (float)sum;
				float fl2 = ((float)currentColor.G) / (float)sum;
				float fl3 = ((float)currentColor.B) / (float)sum;
				res.Matrix00 = fl1;
				res.Matrix10 = fl1;
				res.Matrix20 = fl1;
				res.Matrix01 = fl2;
				res.Matrix11 = fl2;
				res.Matrix21 = fl2;
				res.Matrix02 = fl3;
				res.Matrix12 = fl3;
				res.Matrix22 = fl3;
			}
			return res;
		}
		public XPaint() { }
		public virtual void Dispose() {
		}
		public virtual void BeginPaint(Graphics g) {
		}
		public virtual void EndPaint(Graphics g) {
		}
		public virtual void BeginCalc(Graphics g) {
		}
		public virtual void EndCalc(Graphics g) {
		}
		static System.Reflection.FieldInfo nativeImage;
		public static bool IsImageValid(Image image) {
			if(nativeImage == null) {
				nativeImage = typeof(Image).GetField("nativeImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			}
			if(nativeImage == null) {
				try {
					if(image.Width > 0) return true;
					return true;
				}
				catch {
					return false;
				}
			}
			return ((IntPtr)nativeImage.GetValue(image)) != IntPtr.Zero;
		}
		protected bool CanDraw(Rectangle r) {
			if(r.Width < 1 || r.Height < 1) return false;
			return true;
		}
		public void DrawMultiColorString(GraphicsCache cache, Rectangle bounds, string text, string matchedText, AppearanceObject appearance, Color highlightText, Color highlight, bool invert) {
			DrawMultiColorString(cache, bounds, text, matchedText, appearance, highlightText, highlight, invert, -1);
		}
		public virtual void DrawMultiColorString(GraphicsCache cache, Rectangle bounds, string text, string matchedText, AppearanceObject appearance, StringFormat stringFormat, Color highlightText, Color highlight, bool invert, int containsStartIndex) {
			MultiColorDrawStringParams param = new MultiColorDrawStringParams(appearance);
			param.Text = text;
			param.Bounds = bounds;
			text = param.Text; 
			if(containsStartIndex > MultiColorDrawStringParams.MaxTextLength + matchedText.Length) containsStartIndex = -1;
			Color matchedTextColor = invert ? appearance.GetForeColor() : highlightText,
				  matchedBackColor = invert ? appearance.GetBackColor() : highlight,
				  textColor = invert ? highlightText : appearance.GetForeColor(),
				  backColor = invert ? highlight : appearance.GetBackColor();
			if(containsStartIndex > 0 && text.Length != matchedText.Length) {
				param.Ranges = new CharacterRangeWithFormat[] { 
					  new CharacterRangeWithFormat(0, containsStartIndex, textColor, backColor),
					  new CharacterRangeWithFormat(containsStartIndex, matchedText.Length, matchedTextColor, matchedBackColor),
					  new CharacterRangeWithFormat(containsStartIndex + matchedText.Length, text.Length - matchedText.Length - containsStartIndex, textColor, backColor)};
			}
			else {
				param.Ranges = new CharacterRangeWithFormat[] { 
					  new CharacterRangeWithFormat(0, matchedText.Length, matchedTextColor, matchedBackColor),
					  new CharacterRangeWithFormat(matchedText.Length, text.Length - matchedText.Length, textColor, backColor)};
			}
			param.StringFormat = stringFormat;
			MultiColorDrawString(cache, param);
		}
		public void DrawMultiColorString(GraphicsCache cache, Rectangle bounds, string text, string matchedText, AppearanceObject appearance, Color highlightText, Color highlight, bool invert, int containsStartIndex) {
			DrawMultiColorString(cache, bounds, text, matchedText, appearance, appearance.GetStringFormat(), highlightText, highlight, invert, containsStartIndex);
		}
		public virtual void MultiColorDrawString(GraphicsCache cache, MultiColorDrawStringParams e) {
			if(cache == null || e == null || e.Text == null || e.Text.Length == 0) return;
			Graphics g = cache.Graphics;
			int i, k;
			Rectangle rBounds = e.Bounds;
			GraphicsClip clip = new GraphicsClip();
			clip.RequireAPIClipping = true;
			clip.Initialize(cache);
			GraphicsClipState cs = clip.SaveClip();
			for(i = 0, k = 0; i < e.Ranges.Length; i++) {
				if(e.Ranges[i].CheckValid(e.Text.Length))
					k++;
			}
			CharacterRange[] ranges = new CharacterRange[k];
			for(i = 0, k = 0; i < e.Ranges.Length; i++) {
				if(e.Ranges[i].CheckValid(e.Text.Length)) {
					ranges[k] = new CharacterRange(e.Ranges[i].First, e.Ranges[i].Length);
					k++;
				}
			}
			e.StringFormat.SetMeasurableCharacterRanges(ranges);
			Region[] regions = g.MeasureCharacterRanges(e.Text, e.Appearance.GetFont(), rBounds, e.StringFormat);
			RectangleF bound;
			Brush backBrush = e.Appearance.GetBackBrush(cache);
			if(backBrush != null) {
				for(i = 0; i < regions.Length; i++) 
					cache.Graphics.SetClip(regions[i].GetBounds(cache.Graphics), CombineMode.Exclude);
				FillRectangle(cache.Graphics, backBrush, rBounds);
			}
			for(i = 0; i < regions.Length; i++) {
				bound = regions[i].GetBounds(cache.Graphics);
				cache.ClipInfo.SetClip(Rectangle.Ceiling(bound));
				for(int n = 0; n < regions.Length; n++) {
					if(n != i) cache.ClipInfo.ExcludeClip(regions[n]);
				}
				Rectangle r = new Rectangle((int)bound.X, (int)bound.Y, (int)bound.Width, (int)bound.Height);
				FillRectangle(cache.Graphics, cache.GetSolidBrush(e.Ranges[i].BackColor), r);
				MultiColorDrawStringCore(e, cache, rBounds, i);
			}
			for(i = 0; i < regions.Length; i++) {
				regions[i].Dispose();
			}
			clip.RestoreClipRelease(cs);
		}
		protected virtual void MultiColorDrawStringCore(MultiColorDrawStringParams e, GraphicsCache cache, Rectangle rBounds, int i) {
			cache.Graphics.DrawString(e.Text, ConvertFont(cache.Graphics,  e.Appearance.GetFont()), cache.GetSolidBrush(e.Ranges[i].ForeColor), rBounds, e.StringFormat);
		}
		public virtual void FillGradientRectangle(Graphics g, Brush brush, Rectangle r) {
			FillRectangle(g, brush, r);
		}
		public virtual void FillRegion(Graphics g, Brush brush, Region reg) {
			g.FillRegion(brush, reg);
		}
		public virtual void FillPolygon(Graphics g, Brush brush, Point[] points) {
			g.FillPolygon(brush, points);
		}
		public virtual void DrawCheckBox(Graphics g, Rectangle r, ButtonState state) {
			if(r.Width < 1 || r.Height < 1) return;
			ControlPaint.DrawCheckBox(g, r, state);
		}
		public virtual void DrawRadioButton(Graphics g, Rectangle r, ButtonState state) {
			if(r.Width < 1 || r.Height < 1) return;
			ControlPaint.DrawRadioButton(g, r, state);
		}
		public virtual void DrawImage(Graphics g, Image image, Rectangle destRect) {
			if(image == null) return;
			DrawImage(g, image, destRect, new Rectangle(Point.Empty, image.Size), true);
		}
		public virtual void DrawImage(Graphics g, Image image, Point dest) {
			if(image == null) return;
			DrawImage(g, image, new Rectangle(dest, image.Size), new Rectangle(Point.Empty, image.Size), true);
		}
		public virtual void DrawImage(GraphicsCache cache, object images, int imageIndex, Rectangle rect, bool enabled) {
			if(!enabled) {
				ImageList list = images as ImageList;
				if(list != null) {
					DrawImage(cache.Graphics, list.Images[imageIndex], rect, new Rectangle(Point.Empty, rect.Size), false);
					return;
				}
			}
			ImageCollection.DrawImageListImage(cache, images, imageIndex, rect, enabled);
		}
		public void DrawImage(GraphicsInfoArgs info, object images, int imageIndex, Rectangle rect, bool enabled) {
			DrawImage(info.Cache, images, imageIndex, rect, enabled);
		}
		public virtual void DrawImage(Graphics g, Image image, Rectangle destRect, Rectangle srcRect, ImageAttributes attributes) {
			if(g == null || image == null)
				return;
			if(attributes != null)
				g.DrawImage(image, destRect, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, attributes);
			else
				g.DrawImage(image, destRect, srcRect, GraphicsUnit.Pixel);
		}
		public void DrawImage(Graphics g, Image image, Rectangle destRect, Rectangle srcRect, bool enabled) {
			DrawImage(g, image, destRect, srcRect, enabled ? null : DisabledImageAttr);
		}
		public virtual void DrawImage(Graphics g, Image image, int x, int y, Rectangle srcRect) {
			DrawImage(g, image, x, y, srcRect, true);
		}
		public virtual void DrawImage(Graphics g, Image image, int x, int y, Rectangle srcRect, bool enabled) {
			DrawImage(g, image, x, y, srcRect, GraphicsUnit.Pixel, enabled);
		}
		public virtual void DrawImage(Graphics g, Image image, int x, int y, Rectangle srcRect,	GraphicsUnit srcUnit, bool enabled) {
			srcRect.Width = Math.Min(srcRect.Width, image.Width);
			srcRect.Height = Math.Min(srcRect.Height, image.Height);
			DrawImageCore(g, image, x, y, srcRect, srcUnit, enabled ? null : DisabledImageAttr);
		}
		[ThreadStatic]
		static ImageAttributes _imgAttr;
		static ImageAttributes imgAttr {
			get {
				if(_imgAttr == null) _imgAttr = new ImageAttributes();
				return _imgAttr;
			}
		}
		public virtual void DrawImage(Graphics g, Image image, Rectangle destRect, int srcX, int srcY,
			int srcWidth, int srcHeight, GraphicsUnit srcUnit, ImageAttributes attr) {
			if(g == null || image == null)
				return;
			if(attr == null) attr = imgAttr;
			g.DrawImage(image, destRect, srcX, srcY, srcWidth, srcHeight, srcUnit, attr);
		}
		protected virtual void DrawImageCore(System.Drawing.Graphics g, Image image, int x, int y, Rectangle srcRect, GraphicsUnit srcUnit, ImageAttributes attr) {
			if(attr == null) attr = imgAttr;
			g.DrawImage(image, new Point[] { new Point(x, y), new Point(x + srcRect.Width, y), new Point(x, y + srcRect.Height)}, srcRect, srcUnit, attr);
		}
		public virtual void DrawFocusRectangle(Graphics g, Rectangle r, Color foreColor, Color backColor) {
			if(!CanDraw(r) || FocusRectStyle == DXDashStyle.None) return;
			if(FocusRectStyle == DXDashStyle.Default) {
				using(HatchBrush hb = new HatchBrush(HatchStyle.Percent50, backColor, foreColor)) {
					g.FillRectangle(hb, new Rectangle(r.X, r.Y, 1, r.Height));
					g.FillRectangle(hb, new Rectangle(r.X + 1, r.Y, r.Width - 2, 1));
					g.FillRectangle(hb, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
					g.FillRectangle(hb, new Rectangle(r.X + 1, r.Bottom - 1, r.Width - 2, 1));
				}
			} else {
				using(Pen p = new Pen(foreColor)) {
					p.DashStyle = (DashStyle)FocusRectStyle;
					r.Height--; r.Width--;
					g.DrawRectangle(p, r);
				}
			}
		}
		public virtual void DrawRectangle(Graphics g, Brush brush, Rectangle r) {
			if(!CanDraw(r)) return;
			g.FillRectangle(brush, new Rectangle(r.X, r.Y, 1, r.Height));
			g.FillRectangle(brush, new Rectangle(r.X + 1, r.Y, r.Width - 2, 1));
			g.FillRectangle(brush, new Rectangle(r.Right - 1, r.Y, 1, r.Height));
			g.FillRectangle(brush, new Rectangle(r.X + 1, r.Bottom - 1, r.Width - 2, 1));
		}
		public virtual void DrawRectangle(Graphics g, Pen pen, Rectangle r) {
			if(!CanDraw(r)) return;
			r.Width--;
			r.Height--;
			g.DrawRectangle(pen, r);
		}
		public virtual void DrawPolygon(Graphics g, Pen pen, Point[] points) {
			g.DrawPolygon(pen, points);
		}
		public virtual void DrawLine(Graphics g, Pen pen, Point pt1, Point pt2) {
			g.DrawLine(pen, pt1, pt2);
		}
		public virtual void FillRectangle(Graphics g, Brush brush, int l, int t, int w, int h) {
			if(w < 1 || h < 1) return;
			g.FillRectangle(brush, l, t, w, h);
		}
		public virtual void FillRectangle(Graphics g, Brush brush, Rectangle r) {
			if(!CanDraw(r)) return;
			g.FillRectangle(brush, r);
		}
		public void FillRectangle(Graphics g, Brush brush, RectangleF r) {
			if(r.Width < 1 || r.Height < 1)
				return;
			g.FillRectangle(brush, r);
		}
		public virtual void DrawVString(GraphicsCache cache, string text, Font font, Brush foreBrush, Rectangle rect, StringFormat strFormat, int angle) {
			if(IsLockStringDrawing) return;
			if(angle == 0) {
				DrawString(cache, text, font, foreBrush, rect, strFormat);
				return;
			}
			Matrix savedMatrix = cache.Graphics.Transform; 
			using(Matrix matrix = GetMatrix(rect, angle, savedMatrix))
				cache.Graphics.Transform = matrix;
			if(angle == 90)
				rect.Offset(-rect.X, -rect.Y - rect.Width);
			else
				rect.Offset(-rect.X - rect.Height + 0, -rect.Y);
			if(angle == 90 || angle == 270) { 
				rect.Y -= 1; 
				rect.Width++;
			} 
			rect = new Rectangle(rect.X, rect.Y, rect.Height, rect.Width);
			cache.Graphics.DrawString(text, ConvertFont(cache.Graphics, font), foreBrush, rect, strFormat);
			using(savedMatrix)
				cache.Graphics.Transform = savedMatrix;
		}
		static Matrix GetMatrix(Rectangle rect, int angle, Matrix originalMatrix) {
			Matrix matrix;
			float dx = ((originalMatrix != null) ? originalMatrix.OffsetX : 0) + (float)rect.X;
			float dy = ((originalMatrix != null) ? originalMatrix.OffsetY : 0) + (float)rect.Y;
			switch(angle) {
				case 90:
					matrix = new Matrix(0, 1, -1, 0, dx, dy);
					break;
				case 270:
					matrix = new Matrix(0, -1, 1, 0, dx, dy);
					break;
				default:
					matrix = new Matrix(1, 0, 0, 1, dx, dy);
					matrix.Rotate(angle);
					break;
			}
			return matrix;
		}
		[ThreadStatic]
		static internal int lockStringDrawing;
		static internal bool IsLockStringDrawing { get { return lockStringDrawing != 0; } }
		public void DrawString(GraphicsCache cache, string s, Font font, Brush foreBrush, Rectangle r, StringFormat strFormat) {
			InternalDrawString(cache, s, font, r, foreBrush, strFormat);
		}
		protected virtual void InternalDrawString(GraphicsCache cache, string s, Font font, Rectangle r, Brush foreBrush, StringFormat strFormat) {
			if(IsLockStringDrawing) return;
			if(s == null || !CanDraw(r)) return;
			if(s.Length >= 65536)
				s = s.Substring(0, 65535);
			cache.Graphics.DrawString(s, ConvertFont(cache.Graphics, font), foreBrush, r, strFormat);
		}
		Font ConvertFont(Graphics g, Font font) {
			return font; 
		}
		public Size CalcTextSizeInt(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth) {
			bool isCropped;
			return CalcTextSizeInt(g, s, font, strFormat, maxWidth, int.MaxValue, out isCropped);
		}
		public Size CalcTextSizeInt(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth, int maxHeight) {
			bool isCropped;
			return CalcTextSizeInt(g, s, font, strFormat, maxWidth, maxHeight, out isCropped);
		}
		public Size CalcTextSizeInt(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth, int maxHeight, out bool isCropped) {
			SizeF size = CalcTextSize(g, s, font, strFormat, maxWidth, maxHeight, out isCropped);
			return TextSizeRound(size);
		}
		public virtual SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth) {
			SizeF size;
			if(s != null && s.Length >= 65536)
				s = s.Substring(0, 65535);
			size = g.MeasureString(s, ConvertFont(g, font), maxWidth, strFormat);
			if((int)size.Width != size.Width) size.Width++;
			return size;
		}
		public virtual SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth, int maxHeight) {
			return CalcTextSize(g, s, font, strFormat, maxWidth);
		}
		public virtual SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth, int maxHeight, out bool isCropped) {
			isCropped = false;
			return CalcTextSize(g, s, font, strFormat, maxWidth);
		}
	}
	public class XPaintTextRender : XPaint {
		public static bool UseExpandTabs = false;
		protected override void InternalDrawString(GraphicsCache cache, string s, Font font, Rectangle r, Brush foreBrush, StringFormat strFormat) {
			if(IsLockStringDrawing) return;
			if(!CanDraw(r)) return;
			SolidBrush brush = foreBrush as SolidBrush;
			Color foreColor = brush == null ? Color.Black : brush.Color;
			TextFormatFlags flags = GetTextFormatFlags(strFormat);
			TextRenderer.DrawText(cache.Graphics, s, font, r, foreColor, flags);
		}
		TextFormatFlags GetTextFormatFlags(StringFormat strFormat) {
			TextFormatFlags res = TextFormatFlags.Default;
			switch(strFormat.Trimming) {
				case StringTrimming.Word:
				case StringTrimming.Character:
				case StringTrimming.EllipsisCharacter: res |= TextFormatFlags.EndEllipsis; break;
				case StringTrimming.EllipsisPath: res |= TextFormatFlags.PathEllipsis; break;
				case StringTrimming.EllipsisWord: res |= TextFormatFlags.WordEllipsis; break;
				case StringTrimming.None: break;
			}
			switch(strFormat.HotkeyPrefix) {
				case System.Drawing.Text.HotkeyPrefix.None: res |= TextFormatFlags.NoPrefix; break;
				case System.Drawing.Text.HotkeyPrefix.Hide: res |= TextFormatFlags.HidePrefix; break;
			}
			StringFormatFlags flags = strFormat.FormatFlags;
			bool isRightToLeft = ((flags & StringFormatFlags.DirectionRightToLeft) != 0);
			switch(strFormat.Alignment) {
				case StringAlignment.Center: res |= TextFormatFlags.HorizontalCenter; break;
				case StringAlignment.Near: res |= (isRightToLeft ? TextFormatFlags.Right : TextFormatFlags.Left); break;
				case StringAlignment.Far: res |= (isRightToLeft ? TextFormatFlags.Left : TextFormatFlags.Right) ; break;
			}
			switch(strFormat.LineAlignment) {
				case StringAlignment.Center: res |= TextFormatFlags.VerticalCenter; break;
				case StringAlignment.Near: res |= TextFormatFlags.Top; break;
				case StringAlignment.Far: res |= TextFormatFlags.Bottom; break;
			}
			if((flags & StringFormatFlags.NoWrap) == 0) res |= TextFormatFlags.WordBreak;
			if(isRightToLeft) res |= TextFormatFlags.RightToLeft;
			res |= TextFormatFlags.NoPadding;
			res |= TextFormatFlags.PreserveGraphicsClipping;
			res |= TextFormatFlags.PreserveGraphicsTranslateTransform;
			res |= TextFormatFlags.TextBoxControl;
			if(UseExpandTabs)
				res |= TextFormatFlags.ExpandTabs;
			return res;
		}
		public override SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth) {
			bool isCropped;
			return CalcTextSize(g, s, font, strFormat, maxWidth, int.MaxValue, out isCropped);
		}
		public override SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth, int maxHeight) {
			bool isCropped;
			return CalcTextSize(g, s, font, strFormat, maxWidth, maxHeight, out isCropped);
		}
		public override SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth, int maxHeight, out bool isCropped) {
			if(maxWidth < 1) maxWidth = int.MaxValue;
			Size size = TextRenderer.MeasureText(g, s, font, new Size(maxWidth, 0), GetTextFormatFlags(strFormat));
			if(maxWidth > 0 && maxWidth != int.MaxValue) {
				size.Width = Math.Min(maxWidth, size.Width);
			}
			isCropped = false;
			if((font.Style & FontStyle.Italic) != 0) {
				Size mixed = TextUtils.GetStringSize(g, s, font, strFormat, maxWidth, maxHeight, out isCropped);
				if(size.Width < mixed.Width) size.Width = mixed.Width;
			}
			return new Size(size.Width, size.Height);
		}
	}
	public class XPaintMixed : XPaintTextRender {
		const char FirstHebrewCharUnicodeSymbol = (char)1424;
		const char SpecialCharUnicodeSymbolStart =(char)8192;
		const char SpecialCharUnicodeSymbolEnd = (char)11263;
		const int MaximumCharCountToCheck = 255;
		bool UseTextRender(string s, StringFormat sf) {
			if(string.IsNullOrEmpty(s)) return false;
			if((sf.FormatFlags & StringFormatFlags.DirectionRightToLeft) != 0) return true;
			int len = Math.Min(MaximumCharCountToCheck, s.Length);
			for(int i = 0; i < len; i++) {
				if((s[i] > FirstHebrewCharUnicodeSymbol && s[i] < SpecialCharUnicodeSymbolStart) || s[i] > SpecialCharUnicodeSymbolEnd) return true;
			}
			return false;
		}
		protected override void InternalDrawString(GraphicsCache cache, string s, Font font, Rectangle r, Brush foreBrush, StringFormat strFormat) {
			if(UseTextRender(s, strFormat)) {
				base.InternalDrawString(cache, s, font, r, foreBrush, strFormat);
				return;
			}
			if(IsLockStringDrawing) return;
			if(!CanDraw(r)) return;
			SolidBrush brush = foreBrush as SolidBrush;
			Color foreColor = brush == null ? Color.Black : brush.Color;
			TextUtils.DrawString(cache.Graphics, s, font, foreColor, cache.CalcRectangle(r), strFormat);
		}
		public override SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth) {
			bool isCropped;
			return CalcTextSize(g, s, font, strFormat, maxWidth, int.MaxValue, out isCropped);
		}
		public override SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth, int maxHeight) {
			bool isCropped;
			return CalcTextSize(g, s, font, strFormat, maxWidth, maxHeight, out isCropped);
		}
		public override SizeF CalcTextSize(Graphics g, string s, Font font, StringFormat strFormat, int maxWidth, int maxHeight, out bool isCropped) {
			if(UseTextRender(s, strFormat)) {
				return base.CalcTextSize(g, s, font, strFormat, maxWidth, maxHeight, out isCropped);
			}
			Size size = TextUtils.GetStringSize(g, s, font, strFormat, maxWidth, maxHeight, out isCropped);
			return new SizeF(size.Width, size.Height);
		}
		public override void DrawMultiColorString(GraphicsCache cache, Rectangle bounds, string text, string matchedText, AppearanceObject appearance, StringFormat stringFormat, Color highlightText, Color highlight, bool invert, int containsStartIndex) {
			if(cache == null || text == null || text.Length == 0) return;
			if(UseTextRender(text, stringFormat)) {
				base.DrawMultiColorString(cache, bounds, text, matchedText, appearance, stringFormat, highlightText, highlight, invert, containsStartIndex);
				return;
			}
			if(containsStartIndex == -1)
				containsStartIndex = 0;
			if(text.Length > MultiColorDrawStringParams.MaxTextLength) text = text.Substring(0, MultiColorDrawStringParams.MaxTextLength) + "...";
			TextHighLight par = new TextHighLight(containsStartIndex, matchedText.Length, highlight, highlightText);
			TextUtils.DrawString(cache.Graphics, text, appearance.Font, appearance.ForeColor, bounds, stringFormat,
				par);
		}
	}
	#endregion XPaint
	#region XPrintPaint
	public class XPrintPaint : XPaint  {
		public XPrintPaint() {
		}
		public override void DrawImage(GraphicsCache cache, object list, int imageIndex, Rectangle rect, bool enabled) {
			DrawImage(cache.Graphics, ImageCollection.GetImageListImage(list, imageIndex), rect, new Rectangle(Point.Empty, rect.Size), enabled);
		}
		public override void FillRegion(Graphics g, Brush brush, Region reg) {
			Matrix mat = new Matrix();
			mat.Scale(GraphicsDpi.Document / GraphicsDpi.Pixel, GraphicsDpi.Document / GraphicsDpi.Pixel);
			Region regNew = (Region)reg.Clone();
			regNew.Transform(mat);
			Brush newBrush = GetValidBrush(brush);
			g.FillRegion(newBrush, regNew);
			if(newBrush != brush) newBrush.Dispose();
			regNew.Dispose();
		}
		public override void FillPolygon(Graphics g, Brush brush, Point[] points) {
			Matrix mat = new Matrix();
			mat.Scale(GraphicsDpi.Document / GraphicsDpi.Pixel, GraphicsDpi.Document / GraphicsDpi.Pixel);
			Point[] pointsNew = (Point[])points.Clone();
			mat.TransformPoints(pointsNew);
			g.FillPolygon(brush, pointsNew);			
		}
		public override void DrawCheckBox(Graphics g, Rectangle r, ButtonState state) {
			ControlPaint.DrawCheckBox(g, Rectangle.Round(Convert(r)), state);
		}
		public override void DrawRadioButton(Graphics g, Rectangle r, ButtonState state) {
			Rectangle r2 = Rectangle.Round(Convert(r));
			r2.Size = r.Size;
			ControlPaint.DrawRadioButton(g, r2, state);
		}
		protected override void DrawImageCore(Graphics g, Image image, int x, int y, Rectangle srcRect,	GraphicsUnit srcUnit, ImageAttributes attr) {
			g.DrawImage(image, Convert(x), Convert(y), srcRect, srcUnit);
		}
		public override void DrawImage(Graphics g, Image image, Rectangle destRect, Rectangle srcRect, ImageAttributes attributes) {
			if(g == null || image == null)
				return;
			if(attributes != null) {
				g.DrawImage(image, Rectangle.Round(Convert(destRect)), srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, attributes);
			}
			else {
				g.DrawImage(image, Convert(destRect), srcRect, GraphicsUnit.Pixel);
			}
		}
		public override void FillGradientRectangle(Graphics g, Brush brush, Rectangle r) {
			Brush newBrush = GetValidBrush(brush);
			FillRectangle(g, newBrush, r);
			if(newBrush != brush) newBrush.Dispose();
		}
		public override void DrawFocusRectangle(Graphics g, Rectangle r, Color foreColor, Color backColor) {
		}
		public override void DrawRectangle(Graphics g, Brush brush, Rectangle r) {
			Brush newBrush = GetValidBrush(brush);
			using(Pen pen = new Pen(GetValidColor(brush))) {
				DrawRectangle(g, pen, r);
			}
		}
		public override void DrawRectangle(Graphics g, Pen pen, Rectangle r) {
			if(r.IsEmpty) return;
			r.Width--;
			r.Height--;
			RectangleF f = Convert(r);
			g.DrawRectangle(pen, f.X, f.Y, f.Width, f.Height);
		}
		public override void DrawLine(Graphics g, Pen pen, Point pt1, Point pt2) {
			g.DrawLine(pen, Convert(pt1), Convert(pt2));
		}
		public override void DrawPolygon(Graphics g, Pen pen, Point[] points) {
			Matrix mat = new Matrix();
			mat.Scale(GraphicsDpi.Document / GraphicsDpi.Pixel, GraphicsDpi.Document / GraphicsDpi.Pixel);
			Point[] pointsNew = (Point[])points.Clone();
			mat.TransformPoints(pointsNew);
			g.DrawPolygon(pen, pointsNew);			
		}
		public override void FillRectangle(Graphics g, Brush brush, int l, int t, int w, int h) {
			Brush newBrush = GetValidBrush(brush);
			g.FillRectangle(newBrush, Convert(l), Convert(t), Convert(w), Convert(h));
			if(newBrush != brush) newBrush.Dispose();
		}
		public override void FillRectangle(Graphics g, Brush brush, Rectangle r) {
			Brush newBrush = GetValidBrush(brush);
			g.FillRectangle(newBrush, Convert(r));
			if(newBrush != brush) newBrush.Dispose();
		}
		protected override void InternalDrawString(GraphicsCache cache, string s, Font font, Rectangle r, Brush foreBrush, StringFormat strFormat) {
			cache.Graphics.DrawString(s, font, foreBrush, Convert(r), strFormat);
		}
		static RectangleF Convert(Rectangle r) {
			return GraphicsUnitConverter.PixelToDoc(r);
		}
		static PointF Convert(Point p) {
			return GraphicsUnitConverter.PixelToDoc(p);
		}
		static float Convert(int pixel) { return GraphicsUnitConverter.PixelToDoc(pixel); }
		protected Color GetValidColor(Brush brush) {
			if(brush is LinearGradientBrush) {
				LinearGradientBrush lgb = brush as LinearGradientBrush;
				Color lColor = lgb.LinearColors[0];
				return lColor;
			}
			SolidBrush br = brush as SolidBrush;
			if(br == null) return Color.Black;
			Color clr = br.Color;
			if(clr == Color.Transparent) return Color.White;
			return Color.FromArgb(clr.R, clr.G, clr.B);
		}
		protected Brush GetValidBrush(Brush brush) {
			if(brush is LinearGradientBrush) {
				LinearGradientBrush lgb = brush as LinearGradientBrush;
				Color lColor = lgb.LinearColors[0];
				return new SolidBrush(lColor);
			}
			SolidBrush br = brush as SolidBrush;
			if(br == null || br.Color.A == 0) return brush;
			Color clr = br.Color;
			if(clr == Color.Transparent) return new SolidBrush(Color.White);
			return new SolidBrush(Color.FromArgb(clr.R, clr.G, clr.B));
		}
	}
	#endregion XPaint
	#region APIXPaint
	[SuppressUnmanagedCodeSecurity()]
	public class APIXPaint {
		static internal int ColorToGDIColor(Color color) {
			int rgb = color.ToArgb() & 0xffffff;
			return (rgb >> 16) + (rgb & 0xff00) + ((rgb & 0xff) << 16);
		}
		static internal Color GDIColorToColor(int color) {
			Color res = Color.FromArgb(color & 0xff, (color >> 8) & 0xff, (color >> 16) & 0xff);
			return res;
		}
	}
	#endregion APIXPaint
	#region Clipping
	public class ClipInfo : IDisposable {
		Region clipRegion;
		IntPtr _APIClip;
		public ClipInfo(Rectangle clip) : this(clip, Rectangle.Empty) { }
		public ClipInfo(Rectangle clip, Rectangle maxClip) {
			if(!maxClip.IsEmpty) {
				if(clip.Bottom > maxClip.Bottom) {
					clip.Height = maxClip.Bottom - clip.Top;
				}
			}
			this.clipRegion = new Region(clip);
			this._APIClip = NativeMethods.CreateRectRgn(clip.Left, clip.Top, clip.Right, clip.Bottom);
		}
		public Region ClipRegion { get { return clipRegion; } }
		public IntPtr APIClip { get { return _APIClip; } }
		public virtual void Dispose() {
			if(clipRegion != null)
				clipRegion.Dispose();
			this.clipRegion = null;
			if(_APIClip != IntPtr.Zero)
				NativeMethods.DeleteObject(_APIClip);
			this._APIClip = IntPtr.Zero;
		}
	}
	public class Clipping {
		protected Region fSaveClip;
		protected IntPtr fSaveClipAPI;
		public Rectangle MaxClipBounds;
		public Clipping() {
		}
		public bool IsNeedDrawRect(ref Rectangle r, Rectangle clipRectangle) {
			if(r.IsEmpty) return false;
			if(clipRectangle.IntersectsWith(r)) 
				return true;
			return false;
		}
		public bool PtInRect(Rectangle r, Point pt) {
			return (!r.IsEmpty && r.Contains(pt));
		}
		public void RestoreClip(Graphics g) {
			if(fSaveClip == null) return;
			g.Clip = fSaveClip;
			fSaveClip.Dispose();
			fSaveClip = null;
			RestoreClipAPI(g);
		}
		public void RestoreClipAPI(Graphics g) {
			IntPtr hdc = g.GetHdc();
			try {
				NativeMethods.SelectClipRgn(hdc, fSaveClipAPI);
				if(fSaveClipAPI != IntPtr.Zero)
					NativeMethods.DeleteObject(fSaveClipAPI);
				fSaveClipAPI = IntPtr.Zero;
			}
			finally {
				g.ReleaseHdc(hdc);
			}
		}
		public void SetClip(Graphics g, ClipInfo cli) {
			fSaveClip = g.Clip;
			g.Clip = cli.ClipRegion;
			SetClipAPI(g, cli);
		}
		public Region SetClip(GraphicsInfoArgs info, Rectangle r) {
			Graphics g = info.Graphics;
			if(!MaxClipBounds.IsEmpty) {
				if(r.Y < MaxClipBounds.Top) {
					r.Height -= (MaxClipBounds.Y - r.Y);
					r.Y = MaxClipBounds.Y;
				}
				if(r.Bottom > MaxClipBounds.Bottom) {
					r.Height = MaxClipBounds.Bottom - r.Top;
				}
				if(r.Right > MaxClipBounds.Right) {
					r.Width = MaxClipBounds.Right - r.Left;
				}
			}
			this.fSaveClip = g.Clip;
			Region rgn = new Region(r);
			rgn.Intersect(this.fSaveClip);
			g.Clip = rgn;
			rgn.Dispose();
			SetClipAPI(info, r);
			return fSaveClip;
		}
		protected virtual void SaveClipAPI(IntPtr hdc) {
			if(fSaveClipAPI == IntPtr.Zero)
				fSaveClipAPI = NativeMethods.CreateRectRgn(0, 0, 0, 0);
			if(NativeMethods.GetClipRgn(hdc, fSaveClipAPI) != 1) {
				NativeMethods.DeleteObject(fSaveClipAPI);
				fSaveClipAPI = IntPtr.Zero;
			}
		}
		protected void SetClipAPI(Graphics g, ClipInfo cli) {
			IntPtr hdc = g.GetHdc();
			try {
				SaveClipAPI(hdc);
				NativeMethods.SelectClipRgn(hdc, cli.APIClip);
			}
			finally {
				g.ReleaseHdc(hdc);
			}
		}
		public void SetClipAPI(GraphicsInfoArgs info, Rectangle r) {
			Graphics g = info.Graphics;
			r = info.Cache.CalcClipRectangle(r);
			IntPtr hdc = g.GetHdc();
			try {
				IntPtr api = NativeMethods.CreateRectRgn(r.Left, r.Top, r.Right, r.Bottom);
				SaveClipAPI(hdc);
				NativeMethods.SelectClipRgn(hdc, api);
				NativeMethods.DeleteObject(api);
			}
			finally {
				g.ReleaseHdc(hdc);
			}
		}
		public void MyRestoreClip(Graphics g, IntPtr rgn) {
		}
		public IntPtr MySaveClip(Graphics g) {
			return IntPtr.Zero; 
		}
		public Region SaveClip {
			get { return fSaveClip; }
			set {
				fSaveClip = value;
			}
		}
		public void ExcludeClip(GraphicsInfoArgs info, Rectangle rect) {
			rect = info.Cache.CalcClipRectangle(rect);
			IntPtr hdc = info.Graphics.GetHdc();
			info.Graphics.ReleaseHdc(hdc);
			try {
				NativeMethods.ExcludeClipRect(hdc, rect.X, rect.Y, rect.Right - 1, rect.Bottom - 1);
			} finally {
			}
		}
	}
	#endregion Clipping
	public class CharacterRangeWithFormat {
		public int First;
		public int Length;
		public Color ForeColor;
		public Color BackColor;
		public CharacterRangeWithFormat(int first, int length, Color foreColor, Color backColor) {
			this.First = first;
			this.Length = length;
			this.ForeColor = foreColor;
			this.BackColor = backColor;
		}
		public CharacterRangeWithFormat() : this(0, 0, SystemColors.WindowText, SystemColors.Window) {}
		public bool CheckValid(int textLength) {
			return (Length > 0 && First >= 0 && First + Length <= textLength);
		}
	}
	public class MultiColorDrawStringParams {
		public CharacterRangeWithFormat[] Ranges;
		string text;
		public Rectangle Bounds;
		AppearanceObject appearance;
		StringFormat stringFormat;
		public MultiColorDrawStringParams(AppearanceObject appearance) {
			this.appearance = appearance;
			Ranges = new CharacterRangeWithFormat[] { new CharacterRangeWithFormat()};
			Bounds = Rectangle.Empty;
			Text = "";
		}
		internal const int MaxTextLength = 500;
		public string Text {
			get { return text; }
			set {
				if(value != null && value.Length > MaxTextLength) value = value.Substring(0, MaxTextLength) + "...";
				text = value;
			}
		}
		public AppearanceObject Appearance { get { return appearance; } }
		public StringFormat StringFormat { 
			get {
				if(stringFormat == null) return Appearance.GetStringFormat();
				return stringFormat; 
			}
			set {
				stringFormat = value;
			}
		}
	}
	public class MyDebug {
		internal class AQ {
			public virtual void InitializeCommunication() {
			}
			public virtual void EnableProfiling(bool enable) {
			}
		}
		int ticks;
		static AQ manager = new AQ();
		static MyDebug() {
			manager.InitializeCommunication();
		}
		public void EnableProfiler() {
			manager.EnableProfiling(true);
		}
		public void DisableProfiler() {
			manager.EnableProfiling(false);
		}
		public void StartProfile() {
			ticks = System.Environment.TickCount;
		}
		public void EndProfile(string title) {
			ticks = System.Environment.TickCount - ticks;
			WriteString("OP '"+title+"', time: " + ticks.ToString());
		}
		public void Write(string str) {
			OutputDebugString(count.ToString() + "." + str);count++;
		}
		public void WriteString(string str) {
			Write(str + "\xd\xa");
		}
		public string GetEnumString(object obj) {
			System.Type typ = obj.GetType();
			return typ.GetMembers()[1 + Convert.ToInt32(obj.ToString())].Name;
		}
		static int count;
		[System.Runtime.InteropServices.DllImport("KERNEL32.dll")]
		public static extern void OutputDebugString(string lpOutputString);
	}
}
