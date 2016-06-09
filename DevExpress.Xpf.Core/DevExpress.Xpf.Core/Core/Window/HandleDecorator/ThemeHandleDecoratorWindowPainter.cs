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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
#if !DXWINDOW
using DevExpress.Utils;
#else
using DevExpress.Internal.DXWindow.Data;
#endif
using Thickness = System.Windows.Thickness;
using WPFMedia = System.Windows.Media;
using System.Reflection;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow.Core.HandleDecorator {
#else
namespace DevExpress.Xpf.Core.HandleDecorator {
#endif
	public class ThemeElementPainter {
		Decorator ownerCore;
		Color targetColor;
		public ThemeElementImage Image { get; set; }
		public double ScaleFactor { get; set; }
		public ThemeElementPainter(Decorator owner) {
			ownerCore = owner;
			ScaleFactor = 1;
		}
		public int GetOffsetByWindowType(HandleDecoratorWindowTypes windowType) {
			return (int)(GetOffsetByWindowTypeCore(windowType) * ScaleFactor);
		}
		int GetOffsetByWindowTypeCore(HandleDecoratorWindowTypes windowType) {
			if(ownerCore == null) return 0;
			if(loadDefault) return defaultDecoratorOffset;
			switch(windowType) {
				case HandleDecoratorWindowTypes.Left:
					return (int)ownerCore.DecoratorOffset.Left;
				case HandleDecoratorWindowTypes.Right:
					return (int)ownerCore.DecoratorOffset.Right;
				case HandleDecoratorWindowTypes.Top:
					return (int)ownerCore.DecoratorOffset.Top;
				case HandleDecoratorWindowTypes.Bottom:
					return (int)ownerCore.DecoratorOffset.Bottom;
				default: return 0;
			}
		}
		bool CheckThemeEditorIsRunning() {
#if !DXWINDOW
			Process[] processes = Process.GetProcessesByName(themeEditorProcessName);
			if(processes.Length > 0) return true;
#endif
			return false;
		}
		public void DrawObject(ThemeElementInfo info, Graphics g) {
			targetColor = GetTargetColor(info.Active);
			DrawThemeImage(info, GetElementImageByWindowType(info.WindowType), g);
		}
		Color GetTargetColor(bool active) {
			if(ownerCore == null) return Color.FromArgb(0, 0, 0, 0);
			if(active) return GetRenderedColor(ownerCore.ActiveColor);
			return GetRenderedColor(ownerCore.InactiveColor);
		}
		public Color GetRenderedColor(WPFMedia.SolidColorBrush inputColor) { 
			if(!CheckThemeEditorIsRunning()) return Color.FromArgb(inputColor.Color.A, inputColor.Color.R, inputColor.Color.G, inputColor.Color.B);
			WPFMedia.DrawingVisual drawingVisual = new WPFMedia.DrawingVisual();
			WPFMedia.DrawingContext drawingContext = drawingVisual.RenderOpen();
			drawingContext.DrawRectangle(inputColor, null, new System.Windows.Rect(0, 0, 1, 1));
			drawingContext.Close();
			WPFMedia.Imaging.RenderTargetBitmap bmp = new WPFMedia.Imaging.RenderTargetBitmap(1, 1, 120, 96, WPFMedia.PixelFormats.Pbgra32);
			bmp.Render(drawingVisual);
			WPFMedia.Imaging.BitmapEncoder image = new WPFMedia.Imaging.BmpBitmapEncoder();
			image.Frames.Add(WPFMedia.Imaging.BitmapFrame.Create(bmp));
			MemoryStream stream = new MemoryStream();
			image.Save(stream);
			Bitmap b = new Bitmap(stream);
			Color result = b.GetPixel(0, 0);
			b.Dispose();
			stream.Dispose();
			bmp = null;
			image = null;
			drawingVisual = null;
			drawingContext = null;
			return result;
		}
		ThemeElementImage leftImage, rightImage, topImage, bottomImage;
		const int defaultDecoratorOffset = 40;
		const string themeEditorProcessName = "DevExpress.Xpf.ThemeEditor.Launcher";
		const string decoratorBmpPath1 = "DevExpress.Xpf.Themes.";
		const string decoratorBmpPath2 = ".Images.HandleDecorator.";
#if !DXWINDOW
		const string decoratorDefaultPath = "DevExpress.Xpf.Core.Core.Window.HandleDecorator.DefaultImages.";
#else
		const string decoratorDefaultPath = "DevExpress.DemoData.DXWindow.Core.HandleDecorator.DefaultImages.";
#endif
		const string decTopBmp = "decorator_top.png";
		const string decBottomBmp = "decorator_bottom.png";
		const string decLeftBmp = "decorator_left.png";
		const string decRightBmp = "decorator_right.png";
		public const string strLoadDefault = "loadDefault";
		PaddingEdges DefaultTopAndBottomMargins = new PaddingEdges() {
			Top = 0,
			Bottom = 0,
			Left = 0,
			Right = 0
		};
		PaddingEdges DefaultLeftAndRightMargins = new PaddingEdges() {
			Top = 80,
			Bottom = 80,
			Left = 0,
			Right = 0
		};
		bool loadDefault = false;
		public void ClearImages() {
			leftImage = rightImage = topImage = bottomImage = null;
		}
		Image GetImageFromThemeAssembly(string path, Assembly themeAssembly) {
			Image result = null;
			if(themeAssembly == null) {
				loadDefault = true;
				return GetDefaultDecoratorImage(path);
			}
			Stream stream = themeAssembly.GetManifestResourceStream(path);
			if(stream != null) {
				result = ResourceImageHelper.CreateImageFromResources(path, themeAssembly);
				stream.Dispose();
				loadDefault = false;
			} else {
				loadDefault = true;
				result = GetDefaultDecoratorImage(path);
			}
			return result;
		}
		Image GetDefaultDecoratorImage(string path) {
			string imagePath = decoratorDefaultPath;
			if(path.EndsWith(decTopBmp)) imagePath += decTopBmp;
			if(path.EndsWith(decBottomBmp)) imagePath += decBottomBmp;
			if(path.EndsWith(decLeftBmp)) imagePath += decLeftBmp;
			if(path.EndsWith(decRightBmp)) imagePath += decRightBmp;
			return ResourceImageHelper.CreateImageFromResources(imagePath, Assembly.GetExecutingAssembly());
		}
		PaddingEdges GetSizingMargins(Thickness margins, bool leftOrRight) {
			PaddingEdges result;
			if(!loadDefault)
				result = new PaddingEdges() {
					Left = (int)margins.Left,
					Right = (int)margins.Right,
					Bottom = (int)margins.Bottom,
					Top = (int)margins.Top
				};
			else if(leftOrRight) 
				result = DefaultLeftAndRightMargins;
			else 
				result = DefaultTopAndBottomMargins;
			return result;;
		}
		public ThemeElementImage GetElementImageByWindowType(HandleDecoratorWindowTypes windowType) {
			Assembly themeAssembly = null;
#if !DXWINDOW
			if(ownerCore.CurrentThemeName != strLoadDefault)
				themeAssembly = AssemblyHelper.GetThemeAssembly(ownerCore.CurrentThemeName);
#endif
			switch(windowType) {
				case HandleDecoratorWindowTypes.Left:
					if(leftImage == null)
						leftImage = new ThemeElementImage() {
							ImageCount = 2,
							Image = GetImageFromThemeAssembly(decoratorBmpPath1 + ownerCore.CurrentThemeName + decoratorBmpPath2 + decLeftBmp, themeAssembly),
							Stretch = ThemeImageStretch.Stretch,
							Layout = ThemeImageLayout.Horizontal,
							SizingMargins = GetSizingMargins(ownerCore.DecoratorLeftMargins, true)
						};
					return leftImage;
				case HandleDecoratorWindowTypes.Right:
					if(rightImage == null)
						rightImage = new ThemeElementImage() {
							ImageCount = 2,
							Image = GetImageFromThemeAssembly(decoratorBmpPath1 + ownerCore.CurrentThemeName + decoratorBmpPath2 + decRightBmp, themeAssembly),
							Stretch = ThemeImageStretch.Stretch,
							Layout = ThemeImageLayout.Horizontal,
							SizingMargins = GetSizingMargins(ownerCore.DecoratorRightMargins, true)
						};
					return rightImage;
				case HandleDecoratorWindowTypes.Top:
					if(topImage == null)
						topImage = new ThemeElementImage() {
							ImageCount = 2,
							Image = GetImageFromThemeAssembly(decoratorBmpPath1 + ownerCore.CurrentThemeName + decoratorBmpPath2 + decTopBmp, themeAssembly),
							Stretch = ThemeImageStretch.Stretch,
							Layout = ThemeImageLayout.Vertical,
							SizingMargins = GetSizingMargins(ownerCore.DecoratorTopMargins, false)
						};
					return topImage;
				case HandleDecoratorWindowTypes.Bottom:
					if(bottomImage == null)
						bottomImage = new ThemeElementImage() {
							ImageCount = 2,
							Image = GetImageFromThemeAssembly(decoratorBmpPath1 + ownerCore.CurrentThemeName + decoratorBmpPath2 + decBottomBmp, themeAssembly),
							Stretch = ThemeImageStretch.Stretch,
							Layout = ThemeImageLayout.Vertical,
							SizingMargins = GetSizingMargins(ownerCore.DecoratorBottomMargins, false)
						};
					return bottomImage;
				default:
					return new ThemeElementImage() {
						ImageCount = 1,
						Image = new Bitmap(1, 1),
						SizingMargins = new PaddingEdges() { Left = 0, Right = 0, Bottom = 0, Top = 0 },
						Stretch = ThemeImageStretch.Stretch
					};
			}
		}
		void DrawThemeImage(ThemeElementInfo elementInfo, ThemeElementImage themeImage, Graphics graphics) {
			if(themeImage == null) return;
			int imageIndex = 0;
			if(elementInfo.ImageIndex != -1) imageIndex = elementInfo.ImageIndex;
			if(imageIndex >= themeImage.ImageCount) imageIndex = 0;
			Rectangle imageBounds = themeImage.GetImageBounds(imageIndex);
			if(imageBounds.IsEmpty) return;
			Rectangle r = new Rectangle(0, 0, elementInfo.Width, elementInfo.Height);
			if(themeImage.SizingMargins.IsEmpty) {
				DrawImage(elementInfo, themeImage.Image, imageBounds, r, themeImage.Stretch, graphics);
			} else {
				DrawImageStretchTile(elementInfo, themeImage.SizingMargins, themeImage.Image, imageBounds, r, themeImage.Stretch, graphics);
			}
		}
		void DrawImageStretchTile(ThemeElementInfo info, PaddingEdges paddingEdgesData, Image image, Rectangle imageBounds, Rectangle destBounds, ThemeImageStretch stretch, Graphics graphics) {
			int destMarginLeft, destMarginRight, destMarginTop, destMarginBottom;
			DoMargins(out destMarginLeft, out destMarginRight, destBounds.Width, paddingEdgesData.Left, paddingEdgesData.Right);
			DoMargins(out destMarginTop, out destMarginBottom, destBounds.Height, (int)(paddingEdgesData.Top * ScaleFactor), (int)(paddingEdgesData.Bottom * ScaleFactor));
			int imageLeftX = imageBounds.X;
			int imageCenterX = imageLeftX + paddingEdgesData.Left;
			int imageRightX = imageLeftX + imageBounds.Width - paddingEdgesData.Right;
			int imageTopY = imageBounds.Y;
			int imageCenterY = imageTopY + paddingEdgesData.Top;
			int imageBottomY = imageTopY + imageBounds.Height - paddingEdgesData.Bottom;
			int imageCenterWidth = imageRightX - imageCenterX;
			int imageCenterHeight = imageBottomY - imageCenterY;
			int dstLeftX = destBounds.X;
			int dstCenterX = dstLeftX + destMarginLeft;
			int dstRightX = dstLeftX + destBounds.Width - destMarginRight;
			int dstTopY = destBounds.Y;
			int dstCenterY = dstTopY + destMarginTop;
			int dstBottomY = dstTopY + destBounds.Height - destMarginBottom;
			int dstCenterWidth = dstRightX - dstCenterX;
			int dstCenterHeight = dstBottomY - dstCenterY;
			DrawImage(info, image, new Rectangle(imageCenterX, imageCenterY, imageCenterWidth, imageCenterHeight), new Rectangle(dstCenterX, dstCenterY, dstCenterWidth, dstCenterHeight), stretch, graphics);
			DrawImage(info, image, new Rectangle(imageCenterX, imageTopY, imageCenterWidth, paddingEdgesData.Top), new Rectangle(dstCenterX, dstTopY, dstCenterWidth, destMarginTop), stretch, graphics);
			DrawImage(info, image, new Rectangle(imageRightX, imageCenterY, paddingEdgesData.Right, imageCenterHeight), new Rectangle(dstRightX, dstCenterY, destMarginRight, dstCenterHeight), stretch, graphics);
			DrawImage(info, image, new Rectangle(imageCenterX, imageBottomY, imageCenterWidth, paddingEdgesData.Bottom), new Rectangle(dstCenterX, dstBottomY, dstCenterWidth, destMarginBottom), stretch, graphics);
			DrawImage(info, image, new Rectangle(imageLeftX, imageCenterY, paddingEdgesData.Left, imageCenterHeight), new Rectangle(dstLeftX, dstCenterY, destMarginLeft, dstCenterHeight), stretch, graphics);
			DrawImage(info, image, new Rectangle(imageLeftX, imageTopY, paddingEdgesData.Left, paddingEdgesData.Top), new Rectangle(dstLeftX, dstTopY, destMarginLeft, destMarginTop), stretch, graphics);
			DrawImage(info, image, new Rectangle(imageRightX, imageTopY, paddingEdgesData.Right, paddingEdgesData.Top), new Rectangle(dstRightX, dstTopY, destMarginRight, destMarginTop), stretch, graphics);
			DrawImage(info, image, new Rectangle(imageRightX, imageBottomY, paddingEdgesData.Right, paddingEdgesData.Bottom), new Rectangle(dstRightX, dstBottomY, destMarginRight, destMarginBottom), stretch, graphics);
			DrawImage(info, image, new Rectangle(imageLeftX, imageBottomY, paddingEdgesData.Left, paddingEdgesData.Bottom), new Rectangle(dstLeftX, dstBottomY, destMarginLeft, destMarginBottom), stretch, graphics);
		}
		void DrawImage(ThemeElementInfo info, Image image, Rectangle imageBounds, Rectangle screenBounds, ThemeImageStretch stretch, Graphics graphics) {
			if(stretch == ThemeImageStretch.Tile)
				DrawImageTile(info, image, imageBounds, screenBounds, graphics);
			else
				DrawImageStretch(info, image, imageBounds, screenBounds, graphics);
		}
		void DrawImageTile(ThemeElementInfo info, Image image, Rectangle imageBounds, Rectangle screenBounds, Graphics graphics) {
			imageBounds.X = Math.Max(0, imageBounds.X);
			imageBounds.Y = Math.Max(0, imageBounds.Y);
			if(imageBounds.Width < 1 || imageBounds.Height < 1 || screenBounds.Width < 1 || screenBounds.Height < 1) return;
			try {
				using(TextureBrush brush = new TextureBrush(image, WrapMode.Tile, imageBounds)) {
					int xTransform = 0, yTransform = 0;
					xTransform = screenBounds.X;
					yTransform = screenBounds.Y;
					if((xTransform != 0 || yTransform != 0))
						brush.TranslateTransform(xTransform, yTransform);
					graphics.FillRectangle(brush, screenBounds);
				}
			} catch { }
		}
		void DrawImageStretch(ThemeElementInfo info, Image image, Rectangle imageBounds, Rectangle screenBounds, Graphics graphics) {
			if(imageBounds.Width < 1 || imageBounds.Height < 1 || screenBounds.Width < 1 || screenBounds.Height < 1) return;
			if(imageBounds.Width < screenBounds.Width && imageBounds.Width > 1) imageBounds.Width--;
			if(imageBounds.Height < screenBounds.Height && imageBounds.Height > 1) imageBounds.Height--;
			float fr = (float)targetColor.R / 255;
			float fg = (float)targetColor.G / 255;
			float fb = (float)targetColor.B / 255;
			float[][] ptsArray =	{
				new float[] {1, 0, 0, 0, 0},
				new float[] {0, 1, 0, 0, 0},
				new float[] {0, 0, 1, 0, 0},
				new float[] {0, 0, 0, 1, 0},
				new float[] {fr, fg, fb, 0, 1},
			 };
			ColorMatrix colorMatrix = new ColorMatrix(ptsArray);
			ImageAttributes imgAttribs = new ImageAttributes();
			imgAttribs.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);
			graphics.DrawImage(image, screenBounds, imageBounds.X, imageBounds.Y, imageBounds.Width, imageBounds.Height, GraphicsUnit.Pixel, imgAttribs);
			imgAttribs.Dispose();
			colorMatrix = null;
			ptsArray = null;
		}
		static void DoMargins(out int dstLower, out int dstHigher, int dstLength, int srcLower, int srcHigher) {
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
		Size GetImageSize(HandleDecoratorWindowTypes windowType) {
			return HandleDecoratorWindowLayoutCalculator.GetImageSize(GetElementImageByWindowType(windowType));
		}
		public void CalculateAndSetScaleFactor(Size size) {
			int offset = GetOffsetByWindowTypeCore(HandleDecoratorWindowTypes.Left) + GetOffsetByWindowTypeCore(HandleDecoratorWindowTypes.Right);
			int drawingWidth = GetImageSize(HandleDecoratorWindowTypes.Left).Width + GetImageSize(HandleDecoratorWindowTypes.Right).Width + (offset > 0 ? offset : 0);
			offset = GetOffsetByWindowTypeCore(HandleDecoratorWindowTypes.Top) + GetOffsetByWindowTypeCore(HandleDecoratorWindowTypes.Bottom);
			int drawingHeight = GetImageSize(HandleDecoratorWindowTypes.Top).Height + GetImageSize(HandleDecoratorWindowTypes.Bottom).Height + (offset > 0 ? offset : 0);
			double scale = Math.Min((double)size.Width / drawingWidth, (double)size.Height / drawingHeight);
			if(scale < 0.25d || scale >= 1)
				ScaleFactor = 1;
			else if(scale < 0.5d)
				ScaleFactor = 0.25d;
			else
				ScaleFactor = 0.5d;
		}
	}
	public class ThemeElementImage {
		Image image;
		public Image Image {
			get { return image; }
			set { image = value; }
		}
		PaddingEdges sizingMargins;
		public PaddingEdges SizingMargins {
			get { return sizingMargins; }
			set { sizingMargins = value; }
		}
		int imageCount;
		public int ImageCount {
			get { return imageCount; }
			set { imageCount = value; }
		}
		ThemeImageLayout layout = ThemeImageLayout.Horizontal;
		public ThemeImageLayout Layout { get { return layout; } set { layout = value; } }
		ThemeImageStretch stretch;
		public ThemeImageStretch Stretch { get { return stretch; } set { stretch = value; } }
		internal Size GetImageSize() {
			if(Image == null || ImageCount == 0) return Size.Empty;
			Size size = Image.Size;
			if(ImageCount == 1) return size;
			if(Layout == ThemeImageLayout.Horizontal)
				size.Width /= ImageCount;
			else
				size.Height /= ImageCount;
			return size;
		}
		public Rectangle GetImageBounds(int index) {
			if(Image == null || index < 0 || index >= ImageCount) return Rectangle.Empty;
			Size size = GetImageSize();
			Rectangle res = new Rectangle(Point.Empty, size);
			if(ImageCount == 1) return res;
			if(Layout == ThemeImageLayout.Horizontal)
				res.X = size.Width * index;
			else
				res.Y = size.Height * index;
			return res;
		}
	}
	public enum ThemeImageLayout { Horizontal, Vertical };
	public enum ThemeImageStretch { Tile, NoResize, Stretch };
	public class PaddingEdges {
		int left, right, top, bottom;
		public int Left {
			get { return left; }
			set { left = value; }
		}
		public int Right {
			get { return right; }
			set { right = value; }
		}
		public int Top {
			get { return top; }
			set { top = value; }
		}
		public int Bottom {
			get { return bottom; }
			set { bottom = value; }
		}
		public bool IsEmpty {
			get { return left == 0 && top == 0 && right == 0 && bottom == 0; }
		}
	}
	public class ThemeElementInfo {
		public HandleDecoratorWindowTypes WindowType { get; set; }
		public bool Active { get; set; }
		public Rectangle Bounds { get; set; }
		public int Width {
			get { return Bounds.Width; }
		}
		public int Height {
			get { return Bounds.Height; }
		}
		int imageIndex;
		public int ImageIndex {
			get { return imageIndex; }
			set { imageIndex = value; }
		}
		public ImageAttributes Attributes {
			get { return null; }
		}
	}
}
