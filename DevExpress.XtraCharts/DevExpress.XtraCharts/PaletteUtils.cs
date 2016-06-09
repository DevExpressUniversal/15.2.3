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
using System;
namespace DevExpress.XtraCharts.Native {
	public class ColorHSL {
		static byte GetComponent(float q, float p, float t) {
			const float oneDivSix = 1.0f / 6.0f;
			const float twoDivThree = 2.0f / 3.0f;
			while (t < 0.0f)
				t += 1.0f;
			while (t > 1.0f)
				t -= 1.0f;
			float result;
			if (t < oneDivSix)
				result = p + ((q - p) * 6.0f * t);
			else if (t < 0.5f)
				result = q;
			else if (t < twoDivThree)
				result = p + ((q - p) * (twoDivThree - t) * 6.0f);
			else
				result = p;
			return (byte)Math.Round(result * 255.0f);
		}
		public static explicit operator ColorHSL(Color color) {
			return new ColorHSL(color.GetHue(), color.GetSaturation(), color.GetBrightness());
		}
		public static explicit operator Color(ColorHSL color) {
			const float oneDivThree = 1.0f / 3.0f;
			float q = color.luminance < 0.5f ? (color.luminance * (1.0f + color.saturation)) :
				color.luminance + color.saturation - (color.luminance * color.saturation);
			float p = 2.0f * color.luminance - q;
			float hueScaled = color.hue / 360.0f;
			return Color.FromArgb(255, GetComponent(q, p, hueScaled + oneDivThree),
				GetComponent(q, p, hueScaled), GetComponent(q, p, hueScaled - oneDivThree));
		}
		const float minLuminance = 0.5f;
		const float maxLuminance = 0.8f;
		float hue;
		float saturation;
		float luminance;
		public float Hue {
			get { return hue; }
			set { hue = value; }
		}
		public float Saturation {
			get { return saturation; }
			set { saturation = value; }
		}
		public float Luminance {
			get { return luminance; }
			set { luminance = value; }
		}
		public float MinLuminance { get { return Math.Min(minLuminance, luminance * 0.9f); } }
		public float MaxLuminance { get { return Math.Max(maxLuminance, luminance + (1.0f - luminance) * 0.15f); } }
		public ColorHSL(float hue, float saturation, float luminance) {
			this.hue = hue;
			this.saturation = saturation;
			this.luminance = luminance;
		}
	}
	public static class PaletteUtils {
		public static Image CreateEditorImage(Palette palette, int maxPaletteEntriesCount = 0) {
			const int imageSize = 10;
			Bitmap image = null;
			try {
				int allowedEntriesCount = palette.Count;
				if (maxPaletteEntriesCount != 0)
					allowedEntriesCount = Math.Min(maxPaletteEntriesCount, palette.Count);
				image = new Bitmap(allowedEntriesCount * (imageSize + 1) - 1, imageSize);
				using (Graphics g = Graphics.FromImage(image)) {
					Rectangle rect = new Rectangle(Point.Empty, new Size(imageSize, imageSize));
					for (int i = 0; i < allowedEntriesCount; i++, rect.X += 11) {
						using (Brush brush = new SolidBrush(palette[i].Color))
							g.FillRectangle(brush, rect);
						Rectangle penRect = rect;
						penRect.Width--;
						penRect.Height--;
						using (Pen pen = new Pen(Color.Gray))
							g.DrawRectangle(pen, penRect);
					}
				}
			}
			catch {
				if (image != null) {
					image.Dispose();
					image = null;
				}
			}
			return image;
		}
	}
}
