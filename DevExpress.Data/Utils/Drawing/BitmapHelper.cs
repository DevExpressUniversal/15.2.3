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
using System.Drawing;
using DevExpress.XtraPrinting.Native;
#if !SL
using System.Drawing.Imaging;
#endif
#if SL
using System.Windows.Media;
using DevExpress.Xpf.Drawing;
using DevExpress.XtraPrinting.Stubs;
#endif
namespace DevExpress.Utils {
	public class BitmapCreator {
		const int defaultResolutionLimit = 300;
		public static Bitmap CreateBitmap(Image original, Color backColor) {
			Bitmap newBitmap = new Bitmap(original.Width, original.Height);
			newBitmap.SetResolution(original.HorizontalResolution, original.VerticalResolution);
			return CopyContent(original, newBitmap, backColor);
		}
		public static Bitmap CreateBitmapWithResolutionLimit(Image original, Color backColor) {
			Bitmap newBitmap = CreateClearBitmap(original, defaultResolutionLimit);
			return CopyContent(original, newBitmap, backColor);
		}
		public static Bitmap CreateClearBitmap(Image original, float resolutionLimit) {
			int width = original.Width;
			int height = original.Height;
			float horizontalResolution = original.HorizontalResolution;
			float verticalResolution = original.VerticalResolution;
			if(IsMetafile(original)) {
				Correct(ref width, ref horizontalResolution, resolutionLimit);
				Correct(ref height, ref verticalResolution, resolutionLimit);
			}
			Bitmap newBitmap = new Bitmap(width, height);
			newBitmap.SetResolution(horizontalResolution, verticalResolution);
			return newBitmap;
		}
		static bool IsMetafile(Image original) {
#if !SL 
			return original is System.Drawing.Imaging.Metafile;
#else
			return false;
#endif
		}
		static Bitmap CopyContent(Image original, Bitmap newBitmap, Color backColor) {
			using(Graphics bmpGr = Graphics.FromImage(newBitmap)) {
				if(!DXColor.IsEmpty(backColor))
					bmpGr.Clear(backColor);
				bmpGr.DrawImage(original, 0, 0);
			}
			return newBitmap;
		}
		static void Correct(ref int size, ref float resolution, float resolutionLimit) {
			if(resolution > defaultResolutionLimit) {
				size = (int)((float)size / resolution * resolutionLimit);
				resolution = defaultResolutionLimit;
			}
		}
#if !SL
		public static void TransformBitmap(Image original, Bitmap bitmap, ImageAttributes attributes) {
			Graphics graphics = Graphics.FromImage(bitmap);
			graphics.Clear(Color.Transparent);
			graphics.DrawImage(original, new Rectangle(Point.Empty, bitmap.Size), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
			graphics.Dispose();
		}
		public static ImageAttributes CreateTransparencyAttributes(int imageTransparency) {
			float tpVal = 1.0f - imageTransparency / 255.0f;
			float[][] pts = {
								new float [] {1, 0, 0, 0, 0},
								new float [] {0, 1, 0, 0, 0},
								new float [] {0, 0, 1, 0, 0},
								new float [] {0, 0, 0, tpVal, 0},
								new float [] {0, 0, 0, 0, 1}
							};
			ImageAttributes attributes = new ImageAttributes();
			attributes.SetColorMatrix(new ColorMatrix(pts), ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
			return attributes;
		}
#endif
	}
}
