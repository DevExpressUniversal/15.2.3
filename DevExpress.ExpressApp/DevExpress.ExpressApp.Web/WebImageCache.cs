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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Web {
	public class WebImageCache : IDisposable {
		private string id;
		private Dictionary<string, ImageInfo> images = new Dictionary<string, ImageInfo>();
		private void ImageResourceHttpHandler_QueryImageInfo(object sender, ImageInfoEventArgs e) {
			if(e.ImageInfo.IsEmpty) {
				e.ImageInfo = images.ContainsKey(e.Url) ? images[e.Url] : new ImageInfo();
			}
		}
		protected Dictionary<string, ImageInfo> Images {
			get {
				return images;
			}
		}
		public WebImageCache() {
			id = string.Format("WIC{0}_", GetHashCode());
			ImageResourceHttpHandler.QueryImageInfo += new EventHandler<ImageInfoEventArgs>(ImageResourceHttpHandler_QueryImageInfo);
		}
		public ImageInfo GetImageInfo(Image image) {
			return GetImageInfo(image, WebImageHelper.GetImageHash(image));
		}
		public ImageInfo GetImageInfo(Image image, string imageKey) {
			Guard.ArgumentNotNull(image, "image");
			Guard.ArgumentNotNull(imageKey, "imageKey");
			string key = id + imageKey;
			string imageUrl = ImageResourceHttpHandler.GetWebResourceUrl(key);
			ImageInfo imageInfo = new ImageInfo(imageKey, image, imageUrl);
			lock(images) {
				if(!images.ContainsKey(key)) {
					images.Add(key, imageInfo);
				}
			}
			return imageInfo;
		}
		#region IDisposable Members
		public void Dispose() {
			ImageResourceHttpHandler.QueryImageInfo -= new EventHandler<ImageInfoEventArgs>(ImageResourceHttpHandler_QueryImageInfo);
			images.Clear();
		}
		#endregion
	}
}
