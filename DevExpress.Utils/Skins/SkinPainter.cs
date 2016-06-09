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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.Xml;
namespace DevExpress.Skins {
	public class SkinHelperBase {
		SkinProductId id;
		public SkinHelperBase(SkinProductId id) {
			this.id = id;
		}
		Skin GetSkin(ISkinProvider provider) {
			return SkinManager.Default.GetSkin(id, provider);
		}
		public SkinElement GetSkinElement(ISkinProvider provider, string elementName) {
			Skin skin = GetSkin(provider);
			return skin != null ? skin[elementName] : null;
		}
		public SkinElementInfo GetSkinElementInfo(ISkinProvider skinProvider, string elementName, Rectangle bounds) {
			return new SkinElementInfo(GetSkinElement(skinProvider, elementName), bounds);
		}
		public void DrawSkinElement(ISkinProvider lookAndFeel, GraphicsCache cache, string elementName, Rectangle bounds) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetSkinElementInfo(lookAndFeel, elementName, bounds));
		}
		public void DrawSkinElement(ISkinProvider lookAndFeel, GraphicsCache cache, string elementName, Rectangle bounds, int imageIndex) {
			SkinElementInfo skinElInfo = GetSkinElementInfo(lookAndFeel, elementName, bounds);
			skinElInfo.ImageIndex = imageIndex;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, skinElInfo);
		}
		public SkinPaddingEdges GetSkinEdges(ISkinProvider lookAndFeel, string skinElementName) {
			SkinElement skinEl = GetSkinElement(lookAndFeel, skinElementName);
			SkinPaddingEdges edges = null;
			if(skinEl == null)
				edges = new SkinPaddingEdges();
			else
				if(skinEl.Image == null)
					edges = new SkinPaddingEdges();
				else
					edges = skinEl.Image.SizingMargins;
			return edges;
		}
		public Color GetColor(ISkinProvider lookAndFeel, string colorName) {
			Skin skin = GetSkin(lookAndFeel);
			return skin.Colors.GetColor(colorName);
		}
		public Int32 GetInteger(ISkinProvider lookAndFeel, string integerName) {
			Skin skin = GetSkin(lookAndFeel);
			return skin.Properties.GetInteger(integerName);
		}
	}
	[Flags]
	public enum SkinImagePart { 
		TopLeft = 1, 
		TopCenter = 2, 
		TopRight = 4, 
		MiddleLeft = 8, 
		MiddleCenter = 16, 
		MiddleRight = 32, 
		BottomLeft = 64, 
		BottomCenter = 128, 
		BottomRight = 256,
		LeftEdge = TopLeft | MiddleLeft | BottomLeft,
		RightEdge = TopRight | MiddleRight | BottomRight,
		TopEdge = TopLeft | TopCenter | TopRight,
		BottomEdge = BottomLeft | BottomCenter | BottomRight,
		All = LeftEdge | RightEdge | TopCenter | MiddleCenter | BottomCenter
	}
	public class SkinElementPainter : ObjectPainter {
		[Browsable(false)]
		public static bool CorrectByRTL { get; set; }
		static Color defaultColor = Color.FromArgb(1, 2, 3);
		public static Color DefaultColor { get { return defaultColor; } }
		static bool useAppearanceBackColor2;
		public static bool UseAppearanceBackColor2 { get { return useAppearanceBackColor2; } set { useAppearanceBackColor2 = value; } }
		static SkinElementPainter defaultPainter;
		static SkinElementPainter() {
			defaultPainter = new SkinElementPainter();
		}
		public static SkinElementPainter Default { get { return defaultPainter; } }
		public static Rectangle GetObjectClientRectangle(Graphics graphics, SkinElementPainter painter, ObjectInfoArgs e, Rectangle bounds) {
			Graphics prev = e.Graphics;
			e.Graphics = graphics;
			Rectangle res = painter.GetObjectClientRectangle(e, bounds);
			e.Graphics = prev;
			return res;
		}
		public override Rectangle GetFocusRectangle(ObjectInfoArgs e) {
			SkinElementInfo ee = e as SkinElementInfo;
			if(ee.Element == null) return e.Bounds;
			Rectangle res = ee.Element.Border.Thin.Deflate(e.Bounds);
			return ee.Element.ContentMarginsCore.GetRightToLeft(ee.RightToLeft).Deflate(res);
		}
		public virtual Rectangle GetObjectClientRectangle(ObjectInfoArgs e, Rectangle bounds) {
			SkinElementInfo ee = e as SkinElementInfo;
			if(ee.Element == null) return bounds;
			Rectangle res = ee.Element.Border.Thin.Deflate(bounds);
			SkinPaddingEdges cm = ee.Element.ContentMargins.GetRightToLeft(ee.RightToLeft);
			if(ee.AllowedParts == SkinImagePart.All)
				return cm.Deflate(res);
			if(ee.HasLeftEdge) {
				res.Width -= cm.Left;
				res.X += cm.Left;
			}
			if(ee.HasRightEdge) {
				res.Width -= cm.Right;
			}
			if(ee.HasTopEdge) {
				res.Y += cm.Top;
				res.Height -= cm.Top;
			}
			if(ee.HasBottomEdge) {
				res.Height -= cm.Bottom;
			}
			return res;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { return GetObjectClientRectangle(e, e.Bounds); }
		public override Rectangle CalcObjectBounds(ObjectInfoArgs e) {
			return Rectangle.Empty;
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElementInfo ee = e as SkinElementInfo;
			if(ee.Element == null) return client;
			SkinPaddingEdges cm = ee.Element.ContentMargins.GetRightToLeft(ee.RightToLeft);
			if(ee.AllowedParts == SkinImagePart.All)
				client = cm.Inflate(client);
			else {
				if(ee.HasLeftEdge) {
					client.Width += cm.Left;
					client.X -= cm.Left;
				}
				if(ee.HasRightEdge) {
					client.Width += cm.Right;
				}
				if(ee.HasTopEdge) {
					client.Y -= cm.Top;
					client.Height += cm.Top;
				}
				if(ee.HasBottomEdge) {
					client.Height += cm.Bottom;
				}
			}
			client = ee.Element.Border.Thin.Inflate(client);
			client.Height = Math.Max(client.Height, ee.Element.Size.MinSize.Height);
			client.Width = Math.Max(client.Width, ee.Element.Size.MinSize.Width);
			return client;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			SkinElementInfo ee = e as SkinElementInfo;
			if(ee.Element == null) return Rectangle.Empty;
			Size minSize = ee.Element.Size.MinSize;
			Size imageSize = GetImageMinSize(ee.Element.Image);
			minSize.Width = Math.Max(minSize.Width, imageSize.Width);
			minSize.Height = Math.Max(minSize.Height, imageSize.Height);
			imageSize = GetImageMinSize(ee.Element.Glyph);
			if(!imageSize.IsEmpty)
				imageSize = CalcBoundsByClientRectangle(ee, new Rectangle(Point.Empty, imageSize)).Size;
			minSize.Width = Math.Max(minSize.Width, imageSize.Width);
			minSize.Height = Math.Max(minSize.Height, imageSize.Height);
			return new Rectangle(Point.Empty, minSize);
		}
		Size GetImageMinSize(SkinImage image) {
			if(image == null) return Size.Empty;
			Size res = image.GetImageBounds(0).Size;
			if(res.IsEmpty || image.Stretch != SkinImageStretch.NoResize) return Size.Empty;
			return ScaleImage(image, res);
		}
		Size ScaleImage(SkinImage image, Size size) {
			if(!(image is SkinGlyph)) return size;
			if(size.Width < 1 || size.Height < 0) return size;
			return DpiProvider.Default.ScaleSkinGlyph(size);
		}
		public override void DrawObject(ObjectInfoArgs e) {
			SkinElementInfo ee = e as SkinElementInfo;
			if(ee.Element == null) return;
			DrawSkinBackground(e, ee.Element);
			DrawSkinImage(ee, ee.Element.Image);
			DrawSkinForeground(ee);
			if(SkinManager.IsSkinHitTestingEnabled && SkinManager.HitPoint != SkinManager.InvalidPoint && SkinManager.CurrentPaintControl != null) {
				Point pt = SkinManager.CurrentPaintControl.PointToClient(SkinManager.HitPoint);
				if(ee.Bounds.Contains(pt)) {
					SkinElement res = ee.Element;
					if(res.IsCustomElement) {
						if(res.Original != null)
							res = res.Original;
						else
							res = null;
					}
					if(res != null)
						SkinManager.HitElements.Add(res);   
				}
			}
		}
		public void DrawNoImage(ObjectInfoArgs e) {
			SkinElementInfo ee = e as SkinElementInfo;
			if(ee.Element == null) return;
			DrawSkinBackground(e, ee.Element);
			DrawSkinForeground(ee);
		}
		protected virtual void DrawSkinBackground(ObjectInfoArgs e, SkinElement element) {
			DrawSkinBorder(e, element.Border);
			FillSkinBackground(e, element);
		}
		protected virtual void DrawSkinForeground(SkinElementInfo ee) {
			DrawSkinImage(ee, ee.Element.Glyph);
		}
		protected virtual void DrawSkinBorder(ObjectInfoArgs e, SkinBorder border) {
			if(border.IsEmpty) return;
			Rectangle bounds = e.Bounds;
			if(((SkinElementInfo)e).CorrectImageFormRTL)
				bounds.X--;
			FillRectangle(e, border.GetTop(), new Rectangle(bounds.X, bounds.Y, bounds.Width, border.Thin.Top));
			FillRectangle(e, border.GetBottom(), new Rectangle(bounds.X, bounds.Bottom - border.Thin.Bottom, bounds.Width, border.Thin.Bottom));
			FillRectangle(e, border.GetLeft(), new Rectangle(bounds.X, bounds.Y + border.Thin.Top, border.Thin.Left, bounds.Height - (border.Thin.Top + border.Thin.Bottom)));
			FillRectangle(e, border.GetRight(), new Rectangle(bounds.Right - border.Thin.Right, bounds.Y + border.Thin.Top, border.Thin.Right, bounds.Height - (border.Thin.Top + border.Thin.Bottom)));
		}
		void FillRectangle(ObjectInfoArgs e, Color fillColor, Rectangle bounds) {
			if(bounds.Width < 1 || bounds.Height < 1 || fillColor == Color.Empty) return;
			e.Paint.FillRectangle(e.Graphics, e.Cache.GetSolidBrush(fillColor), bounds);
		}
		protected virtual void FillSkinBackground(ObjectInfoArgs e, SkinElement element) {
			Rectangle bounds = element.Border.Thin.Deflate(e.Bounds);
			if(((SkinElementInfo)e).CorrectImageFormRTL)
				bounds.X--;
			SkinElementInfo elementInfo = e as SkinElementInfo;
			Color c1 = element.Color.GetBackColor(), c2 = element.Color.GetBackColor2();
			if(c1 == DefaultColor) {
				elementInfo.BackAppearance.DrawBackground(e.Cache, bounds);
				return;
			}
			if(UseAppearanceBackColor2) c2 = elementInfo.BackAppearance.BackColor;
			if(c1 == Color.Empty) return;
			Brush brush;
			if(c2 != Color.Empty) brush = e.Cache.GetGradientBrush(bounds, c1, c2, element.Color.GradientMode);
			else brush = e.Cache.GetSolidBrush(c1);
			e.Paint.FillGradientRectangle(e.Graphics, brush, bounds);
		}
		bool IsFit(Rectangle dest, Size size) {
			return dest.Width >= size.Width && dest.Height >= size.Height;
		}
		public int CalcDefaultImageIndex(SkinImage skinImage, ObjectState state) {
			int imageIndex = 0;
			if(skinImage == null) return 0;
			if(skinImage.ImageCount > 1) {
				ObjectState tempState = state & (~ObjectState.Selected);
				if(tempState == ObjectState.Disabled) imageIndex = 3;
				else 
					if((tempState & ObjectState.Pressed) != 0) imageIndex = 2;
				else
					if((tempState & ObjectState.Hot) != 0) imageIndex = 1;
				if((state & ObjectState.Selected) != 0 && imageIndex == 0) {
					if(skinImage.ImageCount < 5) return 0;
					return 4;
				}
			}
			return imageIndex;
		}
		protected virtual void DrawSkinImage(SkinElementInfo elementInfo, SkinImage skinImage) {
			SkinElement element = elementInfo.Element;
			FlipType rtlFlipType = elementInfo.RightToLeftFlipType;
			if(skinImage == null) return;
			int imageIndex = 0;
			if(elementInfo.ImageIndex == -1 && skinImage.ImageCount > 1) {
				imageIndex = CalcDefaultImageIndex(skinImage, elementInfo.State);
			}
			if(elementInfo.ImageIndex != -1) imageIndex = elementInfo.ImageIndex;
			if(skinImage is SkinGlyph && elementInfo.GlyphIndex != -1) imageIndex = elementInfo.GlyphIndex;
			if(imageIndex >= skinImage.ImageCount) imageIndex = 0;
			if(elementInfo.RightToLeft && rtlFlipType == FlipType.VerticalFlip && skinImage.ImageCount > 1)
				imageIndex = skinImage.ImageCount - 1 - imageIndex;
			Rectangle imageBounds = skinImage.GetImageBounds(imageIndex);
			if(imageBounds.IsEmpty) return;
			Rectangle r = element.Border.Thin.Deflate(elementInfo.Bounds);
			if(skinImage.Stretch == SkinImageStretch.NoResize) {
				Rectangle dest = RectangleHelper.GetCenterBounds(r, ScaleImage(skinImage, imageBounds.Size));
				if(IsFit(dest, imageBounds.Size)) {
					DrawImageStretch(elementInfo, skinImage.GetImage(elementInfo.RightToLeft, rtlFlipType), imageBounds, dest, SkinImagePart.TopLeft);
				}
			}
			else {
				if(skinImage.SizingMargins.IsEmpty) {
					DrawImage(elementInfo, skinImage.GetImage(elementInfo.RightToLeft, rtlFlipType), imageBounds, r, skinImage.Stretch, SkinImagePart.MiddleCenter);
				}
				else {
					DrawImageStretchTile(elementInfo, skinImage.SizingMargins.GetRightToLeft(elementInfo.RightToLeft, rtlFlipType), skinImage.GetImage(elementInfo.RightToLeft, rtlFlipType), imageBounds, r, skinImage.Stretch);
				}
			}
		}
		void DoMargins(out int dstLower, out int dstHigher, int dstLength, int srcLower, int srcHigher) {
			if(srcLower + srcHigher <= dstLength) {
				dstLower = srcLower;
				dstHigher = srcHigher;
			} else if(dstLength <= 0) {
				dstLower = dstHigher = 0;
			} else {
				dstLower = dstLength * srcLower / (srcLower + srcHigher);
				if(dstLower == 0 && srcLower > 0 && dstLength >= 2) {
					dstLower++;
				}
				if(dstLower == dstLength && srcHigher > 0 && dstLower >= 2) {
					dstLower--;
				}
				dstHigher = dstLength - dstLower;
			}
		}
		protected virtual void DrawImageStretchTile(SkinElementInfo info, SkinPaddingEdges paddingEdgesData, Image image, Rectangle imageBounds, Rectangle destBounds, SkinImageStretch stretch) {
			int destMarginLeft, destMarginRight, destMarginTop, destMarginBottom;
			DoMargins(out destMarginLeft, out destMarginRight, destBounds.Width, paddingEdgesData.Left, paddingEdgesData.Right);
			DoMargins(out destMarginTop, out destMarginBottom, destBounds.Height, paddingEdgesData.Top, paddingEdgesData.Bottom);
			int imageLeftX = imageBounds.X;
			int imageCenterX = imageLeftX + paddingEdgesData.Left;
			int imageRightX = imageLeftX + imageBounds.Width - paddingEdgesData.Right;
			int imageTopY = imageBounds.Y;
			int imageCenterY = imageTopY + paddingEdgesData.Top;
			int imageBottomY = imageTopY + imageBounds.Height - paddingEdgesData.Bottom;
			int imageCenterWidth = imageRightX - imageCenterX;
			int imageCenterHeight = imageBottomY - imageCenterY;
			if(!info.HasLeftEdge)
				destMarginLeft = 0;
			if(!info.HasRightEdge)
				destMarginRight = 0;
			if(!info.HasTopEdge)
				destMarginTop = 0;
			if(!info.HasBottomEdge)
				destMarginBottom = 0;
			int dstLeftX = destBounds.X;
			int dstCenterX = dstLeftX + destMarginLeft;
			int dstRightX = dstLeftX + destBounds.Width - destMarginRight;
			int dstTopY = destBounds.Y;
			int dstCenterY = dstTopY + destMarginTop;
			int dstBottomY = dstTopY + destBounds.Height - destMarginBottom;
			int dstCenterWidth = dstRightX - dstCenterX;
			int dstCenterHeight = dstBottomY - dstCenterY;
			Color solid = info.Element.Color.GetSolidImageCenterColor();
			if(solid != Color.Empty)
				DrawImageSolidColorCenter(info, dstCenterX, dstCenterY, dstCenterWidth, dstCenterHeight);
			else 
				DrawImage(info, image, new Rectangle(imageCenterX, imageCenterY, imageCenterWidth, imageCenterHeight), new Rectangle(dstCenterX, dstCenterY, dstCenterWidth, dstCenterHeight), stretch, SkinImagePart.MiddleCenter);
			int imageSourceLeftWidth = paddingEdgesData.Left;
			int imageSourceTopHeight = Math.Min(paddingEdgesData.Top, destMarginTop);
			int imageSourceRightWidth = paddingEdgesData.Right;
			int imageSourceBottomHeight = Math.Min(paddingEdgesData.Bottom, destMarginBottom);
			DrawImage(info, image, new Rectangle(imageCenterX, imageTopY, imageCenterWidth, imageSourceTopHeight), new Rectangle(dstCenterX, dstTopY, dstCenterWidth, destMarginTop), stretch, SkinImagePart.TopCenter);
			DrawImage(info, image, new Rectangle(imageRightX, imageCenterY, imageSourceRightWidth, imageCenterHeight), new Rectangle(dstRightX, dstCenterY, destMarginRight, dstCenterHeight), stretch, SkinImagePart.MiddleRight);
			DrawImage(info, image, new Rectangle(imageCenterX, imageBottomY, imageCenterWidth, imageSourceBottomHeight), new Rectangle(dstCenterX, dstBottomY, dstCenterWidth, destMarginBottom), stretch, SkinImagePart.BottomCenter);
			DrawImage(info, image, new Rectangle(imageLeftX, imageCenterY, imageSourceLeftWidth, imageCenterHeight), new Rectangle(dstLeftX, dstCenterY, destMarginLeft, dstCenterHeight), stretch, SkinImagePart.MiddleLeft);
			DrawImage(info, image, new Rectangle(imageLeftX, imageTopY, imageSourceLeftWidth, imageSourceTopHeight), new Rectangle(dstLeftX, dstTopY, destMarginLeft, destMarginTop), SkinImageStretch.NoResize, SkinImagePart.TopLeft);
			DrawImage(info, image, new Rectangle(imageRightX, imageTopY, imageSourceRightWidth, imageSourceTopHeight), new Rectangle(dstRightX, dstTopY, destMarginRight, destMarginTop), SkinImageStretch.NoResize, SkinImagePart.TopRight);
			DrawImage(info, image, new Rectangle(imageRightX, imageBottomY, imageSourceRightWidth, imageSourceBottomHeight), new Rectangle(dstRightX, dstBottomY, destMarginRight, destMarginBottom), SkinImageStretch.NoResize, SkinImagePart.BottomRight);
			DrawImage(info, image, new Rectangle(imageLeftX, imageBottomY, imageSourceLeftWidth, imageSourceBottomHeight), new Rectangle(dstLeftX, dstBottomY, destMarginLeft, destMarginBottom), SkinImageStretch.NoResize, SkinImagePart.BottomLeft);
		}
		void DrawImageSolidColorCenter(SkinElementInfo info, int dstCenterX, int dstCenterY, int dstCenterWidth, int dstCenterHeight) {
			Rectangle cnt = new Rectangle(dstCenterX, dstCenterY, dstCenterWidth, dstCenterHeight);
			Color solid = info.Element.Color.GetSolidImageCenterColor();
			Color solid2 = info.Element.Color.GetSolidImageCenterColor2();
			if(solid2.IsEmpty)
				info.Cache.FillRectangle(solid, cnt);
			else {
				if(cnt.Width > 0 && cnt.Height > 0) {
					using(Brush brush = new LinearGradientBrush(Rectangle.Inflate(cnt, 1, 1), solid, solid2, info.Element.Color.SolidImageCenterGradientMode)) {
						info.Cache.FillRectangle(brush, cnt);
					}
				}
			}
		}
		void DrawImage(SkinElementInfo info, Image image, Rectangle imageBounds, Rectangle screenBounds, SkinImagePart part) {
			DrawImage(info, image, imageBounds, screenBounds, SkinImageStretch.Stretch, part);
		}
		void DrawImage(SkinElementInfo info, Image image, Rectangle imageBounds, Rectangle screenBounds, SkinImageStretch stretch, SkinImagePart part) {
			if(!info.AllowedParts.HasFlag(part))
				return;
			if(stretch == SkinImageStretch.Tile) 
				DrawImageTile(info, image, imageBounds, screenBounds, part);
			else
				DrawImageStretch(info, image, imageBounds, screenBounds, part);
		}
		bool allowExceptions;
		public bool AllowExceptions { get { return allowExceptions; } set { allowExceptions = value; } }
		void DrawImageTile(SkinElementInfo info, Image image, Rectangle imageBounds, Rectangle screenBounds, SkinImagePart part) {
			imageBounds.X = Math.Max(0, imageBounds.X);
			imageBounds.Y = Math.Max(0, imageBounds.Y);
			if(imageBounds.Width < 1 || imageBounds.Height < 1 || screenBounds.Width < 1 || screenBounds.Height < 1) return;
			if(info.CorrectImageFormRTL || SkinElementPainter.CorrectByRTL)
				CorrectImageFormRTLTile(ref imageBounds, ref screenBounds, part);
			try {
				using(TextureBrush brush = new TextureBrush(image, WrapMode.Tile, imageBounds)) {
					int xTransform = 0, yTransform = 0;
					xTransform = screenBounds.X;
					yTransform = screenBounds.Y;
					if(xTransform != 0 || yTransform != 0)
						brush.TranslateTransform(xTransform, yTransform);
					info.Graphics.FillRectangle(brush, screenBounds);
				}
			} catch {
				if(AllowExceptions) throw;
			}
		}
		void CorrectImageFormRTLTile(ref Rectangle imageBounds, ref Rectangle screenBounds, SkinImagePart part) {
			if(part == SkinImagePart.TopCenter || part == SkinImagePart.BottomCenter || part == SkinImagePart.MiddleCenter) {
				screenBounds.X--;
			}
		}
		static Size[] CorrectSize = new Size[] { 
			new Size(9, 3), new Size(9, 6), 
			new Size(3, 9), new Size(6, 9), new Size(9, 9), new Size(12, 9),
			new Size(3, 18), new Size(6, 18), new Size(9, 18), new Size(12, 18),
			new Size(11, 17), 
			new Size(1, 19), new Size(2, 19), new Size(3, 19), new Size(4, 19), new Size(6,19), new Size(8, 19), new Size(9, 19), new Size(12, 19), new Size(13, 19), 
			new Size(1, 23), new Size(2, 23), new Size(4, 23), new Size(8, 23), new Size(11, 23),
			new Size(9, 24), new Size(10, 24), new Size(13, 24), 
			new Size(5, 25), new Size(9, 25), new Size(10, 25), new Size(13, 25),
			new Size(1, 27), new Size(2, 27), new Size(3, 27), new Size(4, 27), new Size(6, 27), new Size(8, 27), new Size(12, 27)
		};
		static int[,] SizeTable = null;
		static int GetSizeCorrection(int width, int height) {
			if(SizeTable == null)
				InitSizeTable();
			if(width >= SizeTable.GetLength(0) || height >= SizeTable.GetLength(1))
				return 0;
			return SizeTable[width,height];
		}
		private static void InitSizeTable() {
			SizeTable = new int[15,30];
			for(int i = 0; i < 15; i++) { 
				for(int j = 0; j < 30; j++) { 
				 SizeTable[i,j] = ContainsCorrectSize(i, j)? -1: 0;
				}
			}
		}
		private static bool ContainsCorrectSize(int width, int height) {
			for(int i = 0; i < CorrectSize.Length; i++ ) {
				if(CorrectSize[i].Width == width && CorrectSize[i].Height == height)
					return true;
			}
			return false;
		}
		void CorrectImageFormRTL(ref Rectangle imageBounds, ref Rectangle screenBounds, SkinImagePart part) {
			screenBounds.X--;
			if(imageBounds.Width < 15 && imageBounds.Height < 30 && imageBounds.Size == screenBounds.Size) {
				imageBounds.X += GetSizeCorrection(imageBounds.Width, imageBounds.Height);
				return;
			}
			if((part == SkinImagePart.TopLeft || part == SkinImagePart.TopRight || part == SkinImagePart.BottomLeft || part == SkinImagePart.BottomRight)) {
				if((imageBounds.Width <= 5 && imageBounds.Height > 11 && imageBounds.Height < 25 && imageBounds.Width != 1))
						imageBounds.X--;
			}
			else if(part == SkinImagePart.TopCenter || part == SkinImagePart.BottomCenter) {
				if(imageBounds.Width != screenBounds.Width && imageBounds.Width > 1)
					imageBounds.Width--;
			}
			else if(part == SkinImagePart.MiddleLeft || part == SkinImagePart.MiddleRight) {
				if(part == SkinImagePart.MiddleLeft && imageBounds.Width > screenBounds.Width)
					imageBounds.Width = screenBounds.Width;
				if(imageBounds.Height != screenBounds.Height)
					imageBounds.X--;
			}
			else if(part == SkinImagePart.MiddleCenter) {
				if(imageBounds.Width != screenBounds.Width && imageBounds.Width > 1)
					imageBounds.Width--;
				else
					imageBounds.X--;
			}
		}
		void DrawImageStretch(SkinElementInfo info, Image image, Rectangle imageBounds, Rectangle screenBounds, SkinImagePart part) {
			if(imageBounds.Width < 1 || imageBounds.Height < 1 || screenBounds.Width < 1 || screenBounds.Height < 1) return;
			if(imageBounds.Width < screenBounds.Width && imageBounds.Width > 1) imageBounds.Width--;
			if(imageBounds.Height < screenBounds.Height && imageBounds.Height > 1) imageBounds.Height --;
			if(info.CorrectImageFormRTL || SkinElementPainter.CorrectByRTL) {
				CorrectImageFormRTL(ref imageBounds, ref screenBounds, part);
			}
			Color solid = info.Element.Color.GetSolidImageCenterColor();
			if(part == SkinImagePart.MiddleCenter && solid != Color.Empty)
				DrawImageSolidColorCenter(info, screenBounds.X, screenBounds.Y, screenBounds.Width, screenBounds.Height);
			else
				info.Graphics.DrawImage(image, screenBounds, imageBounds.X, imageBounds.Y, imageBounds.Width, imageBounds.Height, GraphicsUnit.Pixel, info.Attributes);
		}
		Matrix SaveTransform(Graphics graphics) {
			if(graphics.Transform != null) return graphics.Transform.Clone() as Matrix;
			return null;
		}
		void RestoreTransform(Graphics graphics, Matrix matrix) {
			if(matrix == null) return;
			graphics.Transform = matrix;
		}
		PointF? dpi;
		public PointF DPI {
			get {
				if(dpi == null) {
					using(Graphics g = GraphicsInfo.CreateTempEmptyGraphics()) {
						dpi = new PointF(g.DpiX, g.DpiY);
					}
				}
				return dpi.Value;
			}
		}
		public float DPIScale {
			get {
				float res = DPI.X / 96f;
				if(res < 1) return 1;
				if(res > 2) return 2;
				return res;
			}
		}
	}
	public class DPIHelper {
		PointF? dpi;
		public PointF DPI {
			get {
				if(dpi == null) {
					using(Graphics g = GraphicsInfo.CreateTempEmptyGraphics()) {
						dpi = new PointF(g.DpiX, g.DpiY);
					}
				}
				return dpi.Value;
			}
		}
		public float DPIScale {
			get {
				float res = DPI.X / 96f;
				if(res < 1) return 1;
				if(res > 2) return 2;
				return res;
			}
		}
	}
	public static class SkinElementColorer {
		public static SkinElementInfo PaintElementWithColor(SkinElementInfo info, Color baseColor, Color color) {
			SkinElement sourceElement = info.Element;
			SkinElement elementClone = PaintElementWithColor(sourceElement, baseColor, color);
			SkinElementInfo result = info.Clone(elementClone);
			return result;
		}
		public static SkinElementInfo PaintElementWithColor(SkinElementInfo info, Color color) {
			SkinElement sourceElement = info.Element;
			SkinElement elementClone = PaintElementWithColor(sourceElement, color);
			SkinElementInfo result = info.Clone(elementClone);
			return result;
		}
		public static SkinElement PaintElementWithColor(SkinElement sourceElement, Color color) {
			Color baseColor = GetBaseColor(sourceElement);
			return PaintElementWithColor(sourceElement, baseColor, color);
		}
		public static SkinElement PaintElementWithColor(SkinElement sourceElement, Color baseColor, Color color) {
			SkinElement elementClone = sourceElement.Copy(sourceElement.Owner);
			ColorMatrix colorMatrix = DevExpress.Utils.Paint.XPaint.GetColorMatrix(baseColor, color);
			PaintSkinImage(elementClone, sourceElement, colorMatrix);
			SetSkinColor(elementClone, sourceElement.Color, colorMatrix);
			return elementClone;
		}
		static Color GetBaseColor(SkinElement sourceElement) {
			return sourceElement.Properties.GetColor(SchedulerSkins.PropBaseColor);
		}
		public static Image GetColoredImage(SkinElement sourceElement, Color color) {
			Color baseColor = GetBaseColor(sourceElement);
			return GetColoredImage(sourceElement, baseColor, color);
		}
		public static Image GetColoredImage(SkinElement sourceElement, Color baseColor, Color color) {
			ColorMatrix colorMatrix = DevExpress.Utils.Paint.XPaint.GetColorMatrix(baseColor, color);
			return PaintSkinImageCore(sourceElement, colorMatrix);
		}
		internal static void PaintSkinImage(SkinElement targetElement, SkinElement sourceElement, ColorMatrix colorMatrix) {
			Image newImage = PaintSkinImageCore(sourceElement, colorMatrix);
			if (newImage == null)
				return;
			targetElement.Info.SetActualImage(newImage, true);
		}
		static Image PaintSkinImageCore(SkinElement sourceElement, ColorMatrix colorMatrix) {
			if (sourceElement.Info.HasImage) {
				Image originalImage = sourceElement.Info.GetActualImage();
				if (originalImage != null)
					return PaintImageWithColor(originalImage, colorMatrix);
			}
			return null;
		}
		static Bitmap PaintImageWithColor(Image originalImage, ColorMatrix colorMatrix) {
			Bitmap newImage = new Bitmap(originalImage.Width, originalImage.Height);
			using (Graphics g = Graphics.FromImage(newImage)) {
				g.Clear(Color.Transparent);
				using (ImageAttributes attributes = new ImageAttributes()) {
					attributes.SetColorMatrix(colorMatrix);
					g.DrawImage(originalImage, new Rectangle(Point.Empty, newImage.Size), 0, 0, newImage.Width, newImage.Height, GraphicsUnit.Pixel, attributes);
				}
			}
			return newImage;
		}
		static void SetSkinColor(SkinElement skinElement, SkinColor color, ColorMatrix colorMatrix) {
			if (color != null) {
				SkinColor colorClone = CloneSkinColor(color);				
				TransformSkinColor(colorClone, colorMatrix);
				colorClone.SetOwner(skinElement.Owner);
				skinElement.Info.Color = colorClone;				
			}
		}
		static void TransformSkinColor(SkinColor skinColor, ColorMatrix colorMatrix) {
			if (skinColor.BackColor != Color.Empty)
				skinColor.BackColor = ApplyColorTransform(skinColor.BackColor, colorMatrix);
			if (skinColor.BackColor2 != Color.Empty)
				skinColor.BackColor2 = ApplyColorTransform(skinColor.BackColor2, colorMatrix);
			if (skinColor.SolidImageCenterColor != Color.Empty)
				skinColor.SolidImageCenterColor = ApplyColorTransform(skinColor.SolidImageCenterColor, colorMatrix);
			if (skinColor.SolidImageCenterColor2 != Color.Empty)
				skinColor.SolidImageCenterColor2 = ApplyColorTransform(skinColor.SolidImageCenterColor2, colorMatrix);
		}
		static SkinColor CloneSkinColor(SkinColor sourceColor) {
			SkinColor colorClone = new SkinColor();
			colorClone.BackColor = sourceColor.BackColor;
			colorClone.BackColor2 = sourceColor.BackColor2;
			colorClone.FontBold = sourceColor.FontBold;
			colorClone.ForeColor = sourceColor.ForeColor;
			colorClone.GradientMode = sourceColor.GradientMode;
			colorClone.SolidImageCenterColor = sourceColor.SolidImageCenterColor;
			colorClone.SolidImageCenterColor2 = sourceColor.SolidImageCenterColor2;
			return colorClone;
		}
		public static Color ApplyColorTransform(Color color, ColorMatrix m) {
			int r = (int)Math.Min(byte.MaxValue, (color.R * m.Matrix00 + color.G * m.Matrix10 + color.B * m.Matrix20));
			int g = (int)Math.Min(byte.MaxValue, (color.R * m.Matrix01 + color.G * m.Matrix11 + color.B * m.Matrix21));
			int b = (int)Math.Min(byte.MaxValue, (color.R * m.Matrix02 + color.G * m.Matrix12 + color.B * m.Matrix22));
			return Color.FromArgb(r, g, b);
		}
	}
	public class SkinElementInfo : ObjectInfoArgs {
		ImageAttributes attributes;
		ColorMatrix matrix;
		SkinElement element;
		int imageIndex;
		AppearanceObject backAppearance;
		int glyphIndex = -1;
		public SkinElementInfo(SkinElement element, Rectangle bounds) {
			this.Bounds = bounds;
			this.element = element;
			this.backAppearance = AppearanceObject.ControlAppearance;
			AllowedParts = SkinImagePart.All;
		}
		public bool RightToLeft { get; set; }
		public FlipType RightToLeftFlipType { get; set; }
		public bool CorrectImageFormRTL { get; set; }
		public ColorMatrix Matrix { get { return matrix; } set { matrix = value; } }
		public AppearanceObject BackAppearance { get { return backAppearance; } set { backAppearance = value; } }
		public SkinElementInfo(SkinElement element) : this(element, Rectangle.Empty) { }
		public int ImageIndex { get { return imageIndex; } set { imageIndex = value; } }
		public SkinElement Element { get { return element; } set { element = value; } }
		public SkinImagePart AllowedParts { get; set; }
		public bool HasLeftEdge { get { return HasAllowedParts(new SkinImagePart[]{ SkinImagePart.TopLeft, SkinImagePart.MiddleLeft, SkinImagePart.BottomLeft}); } }
		public bool HasRightEdge { get { return HasAllowedParts(new SkinImagePart[]{ SkinImagePart.TopRight, SkinImagePart.MiddleRight, SkinImagePart.BottomRight}); } }
		public bool HasTopEdge { get { return HasAllowedParts(new SkinImagePart[] { SkinImagePart.TopRight, SkinImagePart.TopCenter, SkinImagePart.TopLeft}); } }
		public bool HasBottomEdge { get { return HasAllowedParts(new SkinImagePart[] { SkinImagePart.BottomRight, SkinImagePart.BottomCenter, SkinImagePart.BottomLeft }); } }
		internal bool HasAllowedParts(SkinImagePart[] parts) {
			for(int i = 0; i < parts.Length; i++)
				if(AllowedParts.HasFlag(parts[i]))
					return true;
			return false;
		}
		public ImageAttributes Attributes {
			get { return attributes; }
			set { attributes = value; }
		}
		public int GlyphIndex {
			get { return glyphIndex; }
			set { glyphIndex = value; }
		}
		internal SkinElementInfo Clone(SkinElement newElement) {
			SkinElementInfo clone = new SkinElementInfo(newElement, Bounds);
			clone.BackAppearance = this.BackAppearance;
			clone.Matrix = this.Matrix;
			clone.ImageIndex = this.ImageIndex;
			clone.GlyphIndex = this.GlyphIndex;
			clone.RightToLeft = this.RightToLeft;
			return clone;
		}
		[Browsable(false)]
		public bool HasActualImage {
			get { return Element != null && Element.HasImage; }
		}
		[Browsable(false)]
		public Image GetActualImage() {
			return Element.GetActualImage();
		}
		[Browsable(false)]
		public void SetActualImage(Image image, bool useOwnImage) {
			Element.SetActualImage(image, useOwnImage);
		}
	}
	public class SkinSizeGripObjectPainter : SizeGripObjectPainter {
		ISkinProvider provider;
		public SkinSizeGripObjectPainter(ISkinProvider provider) { this.provider = provider; }
		public ISkinProvider Provider { get { return provider; } }
		SkinElementInfo UpdateInfo(ObjectInfoArgs e, Rectangle bounds) {
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(Provider)[CommonSkins.SkinSizeGrip], bounds);
			return info;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) {
			return ObjectPainter.CalcObjectMinBounds(e.Graphics, SkinElementPainter.Default, UpdateInfo(e, e.Bounds));
		}
		protected override void DrawGrip(SizeGripObjectInfoArgs ee, Color backColor, Graphics g, Rectangle bounds) {
			GraphicsCache cache = new GraphicsCache(g);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, UpdateInfo(ee, bounds));
			cache.Dispose();
		}
	}
	public class SkinTextBorderPainter : SkinBorderPainter {
		public SkinTextBorderPainter(ISkinProvider provider) : base(provider) { }
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			return new SkinElementInfo(CommonSkins.GetSkin(Provider)[CommonSkins.SkinTextBorder]);
		}
		protected override SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = base.UpdateInfo(e);
			info.ImageIndex = -1;
			return info;
		}
	}
	public abstract class SkinCustomPainter : ObjectPainter {
		ISkinProvider provider;
		public SkinCustomPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public ISkinProvider Provider { get { return provider; } }
		protected abstract SkinElementInfo CreateInfo(ObjectInfoArgs e);
		protected virtual SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			StyleObjectInfoArgs ee = e as StyleObjectInfoArgs;
			SkinElementInfo info = CreateInfo(e);
			if(ee != null)
				info.BackAppearance = ee.BackAppearance;
			info.Bounds = e.Bounds;
			info.State = e.State;
			info.Cache = e.Cache;
			return info;
		}
		public override Rectangle CalcObjectMinBounds(ObjectInfoArgs e) { 
			return SkinElementPainter.Default.CalcObjectMinBounds(UpdateInfo(e));
		}
		public override void DrawObject(ObjectInfoArgs e) { 
			SkinElementPainter.Default.DrawObject(UpdateInfo(e));
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			return SkinElementPainter.Default.GetObjectClientRectangle(UpdateInfo(e));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { 
			return SkinElementPainter.Default.CalcBoundsByClientRectangle(UpdateInfo(e), client);
		}
	}
	public abstract class SkinBorderPainter : BorderPainter {
		ISkinProvider provider;
		public  SkinBorderPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public ISkinProvider Provider { get { return provider; } }
		protected abstract SkinElementInfo CreateInfo(ObjectInfoArgs e);
		protected virtual SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			StyleObjectInfoArgs ee = e as StyleObjectInfoArgs;
			SkinElementInfo info = CreateInfo(e);
			if(ee != null)
				info.BackAppearance = ee.BackAppearance;
			info.Bounds = e.Bounds;
			info.State = e.State;
			info.Cache = e.Cache;
			return info;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			SkinElementInfo info = UpdateInfo(e);
			Rectangle res = SkinElementPainter.Default.GetObjectClientRectangle(info);
			int indent = GetIndent(info);
			res.Inflate(-indent, -indent);
			return res;
		}
		protected virtual void DrawObjectCore(SkinElementInfo info, ObjectInfoArgs e) {
			SkinElementPainter.Default.DrawObject(info);
		}
		public override void DrawObject(ObjectInfoArgs e) { 
			SkinElementInfo info = UpdateInfo(e);
			DrawObjectCore(info, e);
			int indent = GetIndent(info);
			if(indent == 0) return;
			SkinElement element = info.Element;
			Rectangle r = SkinElementPainter.Default.GetObjectClientRectangle(info);
			Pen pen = e.Cache.GetPen(element.Color.GetForeColor());
			if(pen == null) return;
			for(int n = 0; n < indent; n++) {
				e.Cache.Paint.DrawRectangle(e.Graphics, pen, r);
				r.Inflate(-1, -1);
			}
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { 
			SkinElementInfo info = UpdateInfo(e);
			Rectangle res = SkinElementPainter.Default.CalcBoundsByClientRectangle(info, client);
			int indent = GetIndent(info);
			res.Inflate(indent, indent);
			return res;
		}
		protected virtual int GetIndent(SkinElementInfo info) {
			return info.Element.Properties.GetInteger("Indent");
		}
	}
	public class SkinButtonObjectPainter : ButtonObjectPainter {
		ISkinProvider provider;
		public SkinButtonObjectPainter(ISkinProvider provider) {
			this.provider = provider;
		}
		public ISkinProvider Provider { get { return provider; } }
		protected virtual SkinElementInfo UpdateInfo(ObjectInfoArgs e) {
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(Provider)[CommonSkins.SkinButton], e.Bounds);
			info.State = e.State;
			info.Cache = e.Cache;
			info.ImageIndex = -1;
			return info;
		}
		public override Rectangle GetObjectClientRectangle(ObjectInfoArgs e) { 
			return SkinElementPainter.Default.GetObjectClientRectangle(UpdateInfo(e));
		}
		public override void DrawObject(ObjectInfoArgs e) { 
			SkinElementPainter.Default.DrawObject(UpdateInfo(e));
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) { 
			return SkinElementPainter.Default.CalcBoundsByClientRectangle(UpdateInfo(e), client);
		}
		public override Color GetForeColor(ObjectInfoArgs e) {
			if(e.State == ObjectState.Disabled) {
				Color color = CommonSkins.GetSkin(Provider)[CommonSkins.SkinButton].Properties.GetColor(CommonColors.DisabledText);
				return color;
			}
			return base.GetForeColor(e);
		}
	}
	public class SkinElementWithoutGlyphPainter : SkinElementPainter {
		static SkinElementPainter defaultPainter;
		static SkinElementWithoutGlyphPainter() {
			defaultPainter = new SkinElementWithoutGlyphPainter();
		}
		public static new SkinElementPainter Default { get { return defaultPainter; } }
		protected override void DrawSkinForeground(SkinElementInfo ee) { }
	}
}
