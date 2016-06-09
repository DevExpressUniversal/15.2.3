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

using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
namespace DevExpress.XtraMap.Drawing {
	public static class BitmapUtils {
		static Bitmap ConvertToARGB(Image original) {
			MapUtils.DebugAssert(!IsArgbBitmap(original as Bitmap));
			Bitmap newImage = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
			newImage.SetResolution(original.HorizontalResolution, original.VerticalResolution);
			using(Graphics g = Graphics.FromImage(newImage))
				g.DrawImageUnscaled(original, 0, 0);
			return newImage;
		}
		static bool IsArgbBitmap(Bitmap bitmap) {
			return bitmap != null && bitmap.PixelFormat == PixelFormat.Format32bppArgb;
		}
		public static Bitmap ToArgbBitmap(Bitmap image, bool disposeIfNotArgb) {
			Bitmap bitmap = null;
			if(!IsArgbBitmap(image)) {
				bitmap = BitmapUtils.ConvertToARGB(image);
				if(disposeIfNotArgb){
					image.Dispose();
					image = null;
				}
			}
			else
				bitmap = (Bitmap)image;
			return bitmap;
		}
	}
}
