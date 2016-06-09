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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardWin.Native {
	[DXToolboxItem(false)]
	public class DashboardPictureEdit : PictureEdit {
		const DashboardCommon.ImageSizeMode defaultImageSizeMode = DevExpress.DashboardCommon.ImageSizeMode.Clip;
		const string defaultImage = "DefaultImage";
		const string imageNotFoundImage = "ImageNotFound";
		const string corruptedImage = "ImageCorrupted";
		readonly ImageLoader imageLoader = new ImageLoader();
		bool hasImage;
		DashboardCommon.ImageSizeMode sizeMode;
		public DashboardCommon.ImageSizeMode SizeMode {
			get { return sizeMode; }
			set {
				if(value != sizeMode) {
					sizeMode = value;
					OnSizeModeChanged();
				}
			}
		}
		protected override bool AllowDrawImageDisabled { get { return false; } }
		protected override PictureMenu Menu { get { return null; } }
		public override bool EditorContainsFocus { get { return false; } }
		public bool HasImage { get { return hasImage; } }
		public DashboardPictureEdit() {
		}
		void OnSizeModeChanged() {
			if(hasImage)
				UpdatePictureEditSizeMode(sizeMode);
		}
		void UpdatePictureEditSizeMode(DashboardCommon.ImageSizeMode sizeMode) {
			switch(sizeMode) {
				case DashboardCommon.ImageSizeMode.Clip:
					Properties.SizeMode = PictureSizeMode.Clip;
					break;
				case DashboardCommon.ImageSizeMode.Stretch:
					Properties.SizeMode = PictureSizeMode.Stretch;
					break;
				case DashboardCommon.ImageSizeMode.Zoom:
					Properties.SizeMode = PictureSizeMode.Zoom;
					break;
				case DashboardCommon.ImageSizeMode.Squeeze:
					Properties.SizeMode = PictureSizeMode.Squeeze;
					break;
				default:
					throw new Exception(Helper.GetUnknownEnumValueMessage(sizeMode));
			}
		}
		Image GetColorImage(string imageName) {
			return GetColorImage(ResourceImageHelper.CreateBitmapFromResources(String.Format("DevExpress.DashboardWin.Images.{0}.png", imageName), Assembly.GetExecutingAssembly()));
		}
		Image GetColorImage(Bitmap bitmap) {
			Color newColor = DashboardSkins.GetSkin(LookAndFeel).CommonSkin.Colors["DisabledText"];
			for(int x = 0; x < bitmap.Width; x++)
				for(int y = 0; y < bitmap.Height; y++) {
					Color color = bitmap.GetPixel(x, y);
					color = Color.FromArgb(color.A, newColor);
					bitmap.SetPixel(x, y, color);
				}
			return bitmap;
		}
		public void UpdateImage(byte[] data, bool isInternalImage, string url) {
			Image image = null;
			hasImage = false;
			try {
				if (data != null){
					image = isInternalImage ? GetColorImage(ImageLoader.FromData(data)) : ImageLoader.FromData(data);
					hasImage = !isInternalImage;
				}
				else if (!String.IsNullOrEmpty(url)) {
					image = imageLoader.Load(url);
					hasImage = true;
				}
				else image = GetColorImage(defaultImage);
			}
			catch(FileNotFoundException ex) {
				ToolTip = string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.ImageNotFoundMessage), ex.Message);
				image = GetColorImage(imageNotFoundImage);
			}
			catch (OutOfMemoryException) {
				ToolTip = string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.ImageGeneralExceptionMessage), url);
				image = GetColorImage(corruptedImage);
			}
			catch (ArgumentException) {
				ToolTip = DashboardWinLocalizer.GetString(DashboardWinStringId.ImageCorruptedMessage);
				image = GetColorImage(corruptedImage);
			}
			finally {
				Image = image;
				UpdatePictureEditSizeMode(hasImage ? sizeMode : defaultImageSizeMode);
			}
		}
		public void SetAlignment(ImageHorizontalAlignment imageHorizontalAlignment, ImageVerticalAlignment imageVerticalAlignment) {
			if (hasImage) {
				if (imageHorizontalAlignment == ImageHorizontalAlignment.Left && imageVerticalAlignment == ImageVerticalAlignment.Top)
					Properties.PictureAlignment = ContentAlignment.TopLeft;
				else if (imageHorizontalAlignment == ImageHorizontalAlignment.Center && imageVerticalAlignment == ImageVerticalAlignment.Top)
					Properties.PictureAlignment = ContentAlignment.TopCenter;
				else if (imageHorizontalAlignment == ImageHorizontalAlignment.Right && imageVerticalAlignment == ImageVerticalAlignment.Top)
					Properties.PictureAlignment = ContentAlignment.TopRight;
				else if (imageHorizontalAlignment == ImageHorizontalAlignment.Left && imageVerticalAlignment == ImageVerticalAlignment.Center)
					Properties.PictureAlignment = ContentAlignment.MiddleLeft;
				else if (imageHorizontalAlignment == ImageHorizontalAlignment.Center && imageVerticalAlignment == ImageVerticalAlignment.Center)
					Properties.PictureAlignment = ContentAlignment.MiddleCenter;
				else if (imageHorizontalAlignment == ImageHorizontalAlignment.Right && imageVerticalAlignment == ImageVerticalAlignment.Center)
					Properties.PictureAlignment = ContentAlignment.MiddleRight;
				else if (imageHorizontalAlignment == ImageHorizontalAlignment.Left && imageVerticalAlignment == ImageVerticalAlignment.Bottom)
					Properties.PictureAlignment = ContentAlignment.BottomLeft;
				else if (imageHorizontalAlignment == ImageHorizontalAlignment.Center && imageVerticalAlignment == ImageVerticalAlignment.Bottom)
					Properties.PictureAlignment = ContentAlignment.BottomCenter;
				else if (imageHorizontalAlignment == ImageHorizontalAlignment.Right && imageVerticalAlignment == ImageVerticalAlignment.Bottom)
					Properties.PictureAlignment = ContentAlignment.BottomRight;
			}
		}
	}
}
