#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Demos.Win {
	public class ImageFixedSizeAttribute : ImageEditorAttribute {
		public ImageFixedSizeAttribute(int width, int height)
			: base(ImageEditorMode.PictureEdit, ImageEditorMode.PictureEdit) {
			ImageSizeMode = ImageSizeMode.Normal;
			ListViewImageEditorCustomHeight = height;
			DetailViewImageEditorFixedWidth = width;
			DetailViewImageEditorFixedHeight = height;
		}
	}
	public class OriginalImageSizeAttribute : ImageEditorAttribute {
		public OriginalImageSizeAttribute()
			: base(ImageEditorMode.PictureEdit, ImageEditorMode.PictureEdit) {
			ImageSizeMode = ImageSizeMode.Normal;
		}
	}
	[DefaultProperty("ImageName")]
	[DomainComponent]
	[System.ComponentModel.DisplayName("Image")]
	public class ImagePreviewObject : IPictureItem {
		private BindingList<CategoryString> categoryStrings;
		private ImageSourceBrowserBase owner;
		private string imageName;
		protected virtual Image GetSmallImage(string name, bool isEnabled) {
			return owner.ImageSource.FindImageInfo(name + ImageLoader.SmallImageSuffix, isEnabled).Image;
		}
		protected virtual Image GetImage(string name, bool isEnabled) {
			return owner.ImageSource.FindImageInfo(name, isEnabled).Image;
		}
		protected virtual Image GetLargeImage(string name, bool isEnabled) {
			return owner.ImageSource.FindImageInfo(name + "_32x32", isEnabled).Image;
		}
		protected virtual Image GetDialogImage(string name, bool isEnabled) {
			return owner.ImageSource.FindImageInfo(name + ImageLoader.DialogImageSuffix, isEnabled).Image;
		}
		public ImagePreviewObject(ImageSourceBrowserBase owner, string imageName) {
			this.owner = owner;
			this.imageName = imageName;
		}
		public string ImageName {
			get { return imageName; }
		}
		[ImageFixedSize(48, 48), VisibleInListView(true)]
		[System.ComponentModel.DisplayName("Small")]
		public Image Image12x12 {
			get { return GetSmallImage(imageName, true); }
		}
		[ImageFixedSize(48, 48), VisibleInListView(true)]
		[System.ComponentModel.DisplayName("Original")]
		public Image Image16x16 {
			get { return GetImage(imageName, true); }
		}
		[ImageFixedSize(48, 48), VisibleInListView(true)]
		[System.ComponentModel.DisplayName("Large")]
		public Image Image32x32 {
			get { return GetLargeImage(imageName, true); }
		}
		[ImageFixedSize(48, 48), VisibleInListView(true)]
		[System.ComponentModel.DisplayName("Dialog")]
		public Image Image48x48 {
			get { return GetDialogImage(imageName, true); }
		}
		[OriginalImageSize, VisibleInListView(false)]
		[VisibleInDetailView(false)]
		public Image OriginalImage {
			get {
				Image imageThumbnail = GetLargeImage(imageName, true);
				if(imageThumbnail == null) {
					return owner.ImageSource.FindImageInfo(imageName, true).Image;
				}
				return imageThumbnail;
			}
		}
		public BindingList<CategoryString> Categories {
			get {
				if(categoryStrings == null) {
					categoryStrings = new BindingList<CategoryString>();
					foreach(string categoryName in owner.AllCategories.Keys) {
						if(owner.AllCategories[categoryName].Contains(ImageName)) {
							categoryStrings.Add(new CategoryString(owner, categoryName));
						}
					}
				}
				return categoryStrings;
			}
		}
		#region IPictureItem Members
		string IPictureItem.ID {
			get { return ImageName; }
		}
		Image IPictureItem.Image {
			get {
				if(Image32x32 != null) {
					return Image32x32;
				}
				return OriginalImage;
			}
		}
		string IPictureItem.Text {
			get { return ImageName; }
		}
		#endregion
	}
}
