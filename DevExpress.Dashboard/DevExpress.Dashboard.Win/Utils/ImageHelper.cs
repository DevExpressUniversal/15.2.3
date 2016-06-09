#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Text;
using DevExpress.Utils;
namespace DevExpress.DashboardWin.Native {
	public static class ImageHelper {
		public static Image SelectOtherElementImage = GetImage("Bars.SelectOtherElement_16x16");
		public static Image ExportImage = GetImage("Bars.Print_16x16");
		public static Image ClearMasterFilterImage = GetImage("Bars.ClearMasterFilter_16x16");
		public static Image ClearSelectionImage = GetImage("Bars.ClearSelection_16x16");
		public static Image DrillUpImage = GetImage("Bars.DrillUp_16x16");
		public static Image MapInitialExtent = GetImage("Bars.InitialExtent_16x16");
		public static Image GetImage(string imageName) {
			return ResourceImageHelper.CreateBitmapFromResources(String.Format("DevExpress.DashboardWin.Images.{0}.png", imageName), Assembly.GetExecutingAssembly());
		}
		public static Image GetEditorsMenuImage(string imageName) {
			return ResourceImageHelper.CreateImageFromResources(string.Format("DevExpress.XtraEditors.FormatRule.MenuImages.{0}", imageName), typeof(DevExpress.XtraEditors.FormatConditionRuleBase).Assembly);
		}
		public static Image[] GetImages(params string[] imageNames) {
			int count = imageNames.Length;
			Image[] images = new Image[count];
			for(int i = 0; i < count; i++)
				images[i] = GetImage(imageNames[i]);
			return images;
		}
		public static Bitmap CreateBlendBitmap(Image original) {
			Bitmap bitmap = new Bitmap(original);
			for(int x = 0; x < bitmap.Width; x++)
				for(int y = 0; y < bitmap.Height; y++) {
					Color color = bitmap.GetPixel(x, y);
					color = Color.FromArgb(color.A / 2, color);
					bitmap.SetPixel(x, y, color);
				}
			return bitmap;
		}
		public static Bitmap ColorBitmap(Image original, Color newColor) {
			Bitmap bitmap = new Bitmap(original);
			for(int x = 0; x < bitmap.Width; x++)
				for(int y = 0; y < bitmap.Height; y++) {
					Color color = bitmap.GetPixel(x, y);
					color = Color.FromArgb(color.A, newColor);
					bitmap.SetPixel(x, y, color);
				}
			return bitmap;
		}
		public static Bitmap ColorBitmap(Image original, Color newColor, float opacity) {
			Bitmap bitmap = new Bitmap(original);
			for(int x = 0; x < bitmap.Width; x++)
				for(int y = 0; y < bitmap.Height; y++) {
					Color color = bitmap.GetPixel(x, y);
					color = Color.FromArgb(opacity >= 0 && opacity <= 255 ? (int)(color.A * opacity) : 255, newColor);
					bitmap.SetPixel(x, y, color);
				}
			return bitmap;
		}
		public static string GetImageFilterString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(FileFilters.BMP + FileFilters.Separator);
			stringBuilder.Append(FileFilters.JPEG + FileFilters.Separator);
			stringBuilder.Append(FileFilters.PNG + FileFilters.Separator);
			stringBuilder.Append(FileFilters.GIF + FileFilters.Separator);
			stringBuilder.Append(FileFilters.EMF + FileFilters.Separator);
			stringBuilder.Append(FileFilters.WMF + FileFilters.Separator);
			stringBuilder.Append(FileFilters.TIFF + FileFilters.Separator);
			stringBuilder.Append(FileFilters.ICO + FileFilters.Separator);
			stringBuilder.Append(FileFilters.AllImageFiles + FileFilters.Separator);
			stringBuilder.Append(FileFilters.AllFiles);
			return stringBuilder.ToString();
		}
	}
}
