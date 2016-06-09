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

using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraMap {
	public class MapOverlayImageItem : MapOverlayItemBase, IImageContainer, IOwnedElement, IImageUriLoaderClient {
		const int DefaultImageIndex = -1;
		Uri imageUri;
		Image image;
		int imageIndex = DefaultImageIndex;
		object actualImageList;
		object owner;
		Image loadedImage;
		protected MapOverlay Overlay { get { return owner as MapOverlay; } }
		[Category(SRCategoryNames.Data), DefaultValue(null)]
		public Uri ImageUri {
			get { return imageUri; }
			set {
				if(imageUri == value)
					return;
				imageUri = value;
				OnImageUriChanged();
			}
		}
		[Category(SRCategoryNames.Data), DefaultValue(null)]
		public Image Image {
			get { return image; }
			set {
				if(image == value)
					return;
				image = value;
				OnChanged();
			}
		}
		[Category(SRCategoryNames.Data), DefaultValue(DefaultImageIndex)]
		public int ImageIndex {
			get { return imageIndex; }
			set {
				if(imageIndex == value)
					return;
				imageIndex = value;
				OnChanged();
			}
		}
		#region IImageContainer implementation
		void IImageContainer.UpdateImage(object imageList) {
			this.actualImageList = imageList;
		}
		#endregion
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if(owner == value)
					return;
				owner = value;
				OwnerChanged();
			}
		}
		#endregion
		#region IImageUriLoaderClient implementation
		void IImageUriLoaderClient.OnImageLoaded(Image image) {
			this.loadedImage = image;
			OnChanged();
		}
		#endregion
		void OnImageUriChanged() {
			if(ImageUri == null)
				this.loadedImage = null;
			else
				ImageCacheContainer.QueryImage(ImageUri, this);
			OnChanged();
		}
		void OwnerChanged() {
			if(Overlay != null && Overlay.Map != null)
				MapUtils.UpdateImageContainer(this, Overlay.Map.ImageList);
		}
		protected internal override OverlayItemViewInfoBase CreateViewinfo(InnerMap map, MapUIElementsPainter painter) {
			return new OverlayImageItemViewInfo(map, this, painter.OverlayImageItemPainter);
		}
		protected internal Image GetActualImage() {
			if(loadedImage != null) return loadedImage;
			if(Image != null) return Image;
			if(ImageIndex != DefaultImageIndex && actualImageList != null)
				return ImageCollection.GetImageListImage(actualImageList, ImageIndex);
			return null;
		}
		public override string ToString() {
			return "(MapOverlayImageItem)";
		}
	}
}
