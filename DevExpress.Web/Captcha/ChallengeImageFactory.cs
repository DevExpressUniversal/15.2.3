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
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
namespace DevExpress.Web.Captcha {
	struct ColorF {
		float a;
		float r;
		float g;
		float b;
		public float A { get { return this.a; } }
		public float R { get { return this.r; } }
		public float G { get { return this.g; } }
		public float B { get { return this.b; } }
		public ColorF(float a, float r, float g, float b) {
			this.a = a;
			this.r = r;
			this.g = g;
			this.b = b;
		}
		public ColorF(Color color)
			: this(color.A, color.R, color.G, color.B) {
		}
		public Color ToColor() {
			return Color.FromArgb((int)A, (int)R, (int)G, (int)B);
		}
		public bool Transparent {
			get { return A + R + G + B == 0.0f; }
		}
	}
	public class ChallengeImageFactory : IImageFactory {
		const float EstimateFontSize = 19.0f;
		const float ApproximateSerifRadius = 0.8f;
		const float MaxOffset = 100.0f;
		const float EstimateWavePeriod = 11.0f;
		const float EstimateWaveAmplitude = 5.0f;
		const float ScaleFactorDivisor = 1.9f;
		const int MinWavePeriodRandomFactor = 1;
		const int MaxWavePeriodRandomFactor = 3;
		const int DesignModeWavePeriodRandomFactor = 2;
		const int MinWaveRandomFactor = 0;
		const int MaxWaveRandomFactor = 100;
		const int DesignModeWaveRandomFactor = 50;
		const int TextPathPaddings = 2;
		ASPxCaptcha owner;
		IRandomNumberGenerator randomGenerator;
		public ChallengeImageFactory(ASPxCaptcha owner, IRandomNumberGenerator randomGenerator) {
			this.owner = owner;
			this.randomGenerator = randomGenerator;
		}
		byte[] IImageFactory.GetImage(string text) {
			byte[] imageData = null;
			Bitmap image = GenerateImage(text);
			ImageFormat imageFormat = this.owner.DesignMode ? ImageFormat.Bmp : ImageFormat.Png;
			using (MemoryStream stream = new MemoryStream()) {
				image.Save(stream, imageFormat);
				imageData = stream.ToArray();
			}
			return imageData;
		}
		protected Bitmap GenerateImage(string text) {
			GraphicsPath textPath = GetTextPath(text);
			int imageWidth = this.owner.ChallengeImage.Width;
			int imageHeight = this.owner.ChallengeImage.Height;
			Brush textBrush = new SolidBrush(this.owner.ChallengeImage.ForegroundColor);
			float scaleFactor = 0.0f;
			textPath.FillMode = FillMode.Winding;
			CenterTextPath(ref textPath, imageWidth, imageHeight, out scaleFactor);
			Bitmap foregroundImage = new Bitmap(imageWidth, imageHeight);
			Color backgroundColor = this.owner.DesignMode ? 
				this.owner.ChallengeImage.BackgroundColor : Color.Transparent;
			using (Bitmap textImage = new Bitmap(imageWidth, imageHeight)) {
				using (Graphics graphics = Graphics.FromImage(textImage)) {
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.Clear(backgroundColor);
					graphics.FillPath(textBrush, textPath);
					graphics.Flush();
				}
				foregroundImage = ApplyWaveFilter(textImage, scaleFactor);
			}
			textPath.Dispose();
			return this.owner.DesignMode ? ApplyDesignModeImageFix(foregroundImage, backgroundColor) :
				foregroundImage;
		}
		protected Bitmap ApplyDesignModeImageFix(Bitmap foregroundImage, Color backgroundColor) {
			Bitmap designModeImage = new Bitmap(foregroundImage.Width, foregroundImage.Height);
			using (Graphics graphics = Graphics.FromImage(designModeImage)) {
				graphics.Clear(backgroundColor);
				graphics.DrawImageUnscaled(foregroundImage, 0, 0);
				graphics.Flush();
			}
			return designModeImage;
		}
		protected int CaptchaFontStyleToInt(CaptchaFontStyle captchaFontStyle) {
			if (captchaFontStyle == CaptchaFontStyle.Regular)
				return (int)FontStyle.Regular;
			if (captchaFontStyle == CaptchaFontStyle.Bold)
				return (int)FontStyle.Bold;
			return (int)FontStyle.Italic;
		}
		protected GraphicsPath GetFlattenedPath(GraphicsPath source) {
			GraphicsPath flattenedPath = new GraphicsPath(source.PathPoints, source.PathTypes);
			flattenedPath.Flatten();
			return flattenedPath;
		}
		protected void AddCharToPath(ref GraphicsPath path, char character, float x, float y,
			FontFamily fontFamily) {
			using (StringFormat stringFormat = new StringFormat()) {
				path.AddString(character.ToString(), fontFamily,
					CaptchaFontStyleToInt(this.owner.ChallengeImage.FontStyle), EstimateFontSize,
					new PointF(x, y), stringFormat);
			}
		}
		protected void OffsetPath(ref GraphicsPath path, float offset) {
			PointF[] translatedPathPoints = new PointF[path.PathPoints.Length];
			byte[] pathTypes = path.PathTypes;
			for (int i = 0; i < path.PathPoints.Length; i++) {
				translatedPathPoints[i].X = path.PathPoints[i].X - offset;
				translatedPathPoints[i].Y = path.PathPoints[i].Y;
			}
			path.Dispose();
			path = new GraphicsPath(translatedPathPoints, pathTypes);
		}
		protected GraphicsPath GetTextPath(string text) {
			float offset = 0.0f;
			GraphicsPath resultPath = new GraphicsPath();
			GraphicsPath leftChar = new GraphicsPath();
			GraphicsPath rightChar;
			RectangleF leftCharBounds;
			FontFamily fontFamily;
			try {
				fontFamily = new FontFamily(this.owner.ChallengeImage.FontFamily);
			} catch {
				fontFamily = new FontFamily(CaptchaImageProperties.DefaultFontFamily);
			}
			AddCharToPath(ref leftChar, text[0], 0.0f, 0.0f, fontFamily);
			resultPath.AddPath(leftChar, false);
			for (int i = 1; i < text.Length; i++) {
				leftCharBounds = leftChar.GetBounds();
				rightChar = new GraphicsPath();
				AddCharToPath(ref rightChar, text[i], leftCharBounds.X + leftCharBounds.Width,
					0.0f, fontFamily);
				offset = GetRightCharOffset(leftChar, rightChar);
				OffsetPath(ref rightChar, offset);
				resultPath.AddPath(rightChar, false);
				leftChar.Dispose();
				leftChar = rightChar;
			}
			leftChar.Dispose();
			return resultPath;
		}
		protected float GetRightCharOffset(GraphicsPath leftChar, GraphicsPath rightChar) {
			using (GraphicsPath flattenedRightChar = new GraphicsPath(rightChar.PathPoints, rightChar.PathTypes)) {
				using (GraphicsPath flattenedLeftChar = new GraphicsPath(leftChar.PathPoints, leftChar.PathTypes)) {
					flattenedRightChar.Flatten();
					flattenedLeftChar.Flatten();
					RectangleF leftCharBounds = flattenedLeftChar.GetBounds();
					RectangleF rightCharBounds = flattenedRightChar.GetBounds();
					float offset = rightCharBounds.X - leftCharBounds.X - leftCharBounds.Width;
					float searchBound = 0.0f;
					using (Pen pen = new Pen(Color.Black)) {
						for (; offset < MaxOffset; offset += 1.0f) {
							searchBound = rightCharBounds.X + offset;
							for (int i = 0; i < flattenedRightChar.PathPoints.Length; i++) {
								if (flattenedRightChar.PathPoints[i].X > searchBound)
									continue;
								if (flattenedLeftChar.IsOutlineVisible(flattenedRightChar.PathPoints[i].X - offset,
									flattenedRightChar.PathPoints[i].Y, pen)) {
									return offset + ApproximateSerifRadius;
								}
							}
							searchBound = leftCharBounds.X + leftCharBounds.Width - offset;
							for (int i = 0; i < flattenedLeftChar.PathPoints.Length; i++) {
								if (flattenedLeftChar.PathPoints[i].X < searchBound)
									continue;
								if (flattenedRightChar.IsOutlineVisible(flattenedLeftChar.PathPoints[i].X + offset,
									flattenedLeftChar.PathPoints[i].Y, pen)) {
									return offset + ApproximateSerifRadius;
								}
							}
						}
					}
				}
			}
			return 0.0f;
		}
		protected void CenterTextPath(ref GraphicsPath textPath, int foregroundWidth,
			int foregroundHeight, out float scaleFactor) {
			RectangleF textPathBounds = textPath.GetBounds();
			float scaleFactorX = foregroundWidth / (textPathBounds.X * 2 + textPathBounds.Width +
				TextPathPaddings);
			float scaleFactorY = foregroundHeight / (textPathBounds.Y * 2 + textPathBounds.Height +
				TextPathPaddings);
			scaleFactor = Math.Min(scaleFactorX, scaleFactorY);
			Matrix transformMatrix = new Matrix();
			transformMatrix.Scale(scaleFactor, scaleFactor);
			textPath.Transform(transformMatrix);
			RectangleF scaledTextPathBounds = textPath.GetBounds();
			transformMatrix.Reset();
			float translateX = (foregroundWidth - scaledTextPathBounds.Width) / 2 - scaledTextPathBounds.X;
			float translateY = (foregroundHeight - scaledTextPathBounds.Height) / 2 - scaledTextPathBounds.Y;
			transformMatrix.Translate(translateX, translateY);
			textPath.Transform(transformMatrix);
		}
		protected Bitmap ApplyWaveFilter(Bitmap sourceImage, float scaleFactor) {
			int wavePeriodRandomFactor = this.owner.DesignMode ? DesignModeWavePeriodRandomFactor :
				this.randomGenerator.Next(MinWavePeriodRandomFactor, MaxWavePeriodRandomFactor);
			float period = (scaleFactor / ScaleFactorDivisor) * EstimateWavePeriod * wavePeriodRandomFactor;
			float amplitude = (scaleFactor / ScaleFactorDivisor) * EstimateWaveAmplitude;
			float randomFactor = this.owner.DesignMode ? DesignModeWaveRandomFactor :
				this.randomGenerator.Next(MinWaveRandomFactor, MaxWaveRandomFactor);
			float transformedY = 0.0f;
			Bitmap transformedImage = new Bitmap(sourceImage.Width, sourceImage.Height);
			Color bilinearFilteredPixel;
			for (int x = 0; x < sourceImage.Width - 1; x++) {
				for (int y = 0; y < sourceImage.Height; y++) {
					transformedY = y + amplitude * (float)Math.Sin(randomFactor + x / period);
					transformedY = Math.Min(transformedY, sourceImage.Height - 2);
					transformedY = Math.Max(transformedY, 0);
					bilinearFilteredPixel = GetBilinearFilteredPixel(sourceImage, (float)x, transformedY);
					transformedImage.SetPixel(x, y, bilinearFilteredPixel);
				}
			}
			return transformedImage;
		}
		protected Color GetBilinearFilteredPixel(Bitmap sourceImage, float u, float v) {
			int x = (int)Math.Floor(u);
			int y = (int)Math.Floor(v);
			ColorF topLeftColor = new ColorF(sourceImage.GetPixel(x, y));
			ColorF topRightColor = new ColorF(sourceImage.GetPixel(x + 1, y));
			ColorF bottomLeftColor = new ColorF(sourceImage.GetPixel(x, y + 1));
			ColorF bottomRightColor = new ColorF(sourceImage.GetPixel(x + 1, y + 1));
			ColorF topFiltered = InterpolateColors(topLeftColor, topRightColor, u - x);
			ColorF bottomFiltered = InterpolateColors(bottomLeftColor, bottomRightColor, u - x);
			return InterpolateColors(topFiltered, bottomFiltered, 1 - (v - y)).ToColor();
		}
		ColorF InterpolateColors(ColorF color1, ColorF color2, float weight) {
			float a = weight * color1.A + (1 - weight) * color2.A;
			if (color1.Transparent)
				return new ColorF(a, color2.R, color2.G, color2.B);
			if (color2.Transparent)
				return new ColorF(a, color1.R, color1.G, color1.B);
			float r = weight * color1.R + (1 - weight) * color2.R;
			float g = weight * color1.G + (1 - weight) * color2.G;
			float b = weight * color1.B + (1 - weight) * color2.B;
			return new ColorF(a, r, g, b);
		}
	}
}
