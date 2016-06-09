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
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.DataAccess.UI.Native {
	public static class ImageHelper {
		public static Image GetImage(string imageName) {
			return ResourceImageHelper.CreateBitmapFromResources(String.Format("DevExpress.DataAccess.UI.Images.{0}.png", imageName), Assembly.GetExecutingAssembly());
		}
		public static Image[] GetImages(params string[] imageNames) {
			int count = imageNames.Length;
			Image[] images = new Image[count];
			for (int i = 0; i < count; i++)
				images[i] = GetImage(imageNames[i]);
			return images;
		}
		public static Bitmap CreateBlendBitmap(Image original) {
			Bitmap bitmap = new Bitmap(original);
			for (int x = 0; x < bitmap.Width; x++)
				for (int y = 0; y < bitmap.Height; y++) {
					Color color = bitmap.GetPixel(x, y);
					color = Color.FromArgb(color.A / 2, color);
					bitmap.SetPixel(x, y, color);
				}
			return bitmap;
		}
		public static Bitmap ColorBitmap(Image original, Color newColor) {
			Bitmap bitmap = new Bitmap(original);
			for (int x = 0; x < bitmap.Width; x++)
				for (int y = 0; y < bitmap.Height; y++) {
					Color color = bitmap.GetPixel(x, y);
					color = Color.FromArgb(color.A, newColor);
					bitmap.SetPixel(x, y, color);
				}
			return bitmap;
		}
	}
}
