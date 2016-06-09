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
using DevExpress.Web;
using DevExpress.ExpressApp.Utils;
using System.Web.UI.WebControls;
namespace DevExpress.ExpressApp.Web {
	public static class ASPxImageHelper {
		public static void SetImageProperties(ImagePropertiesBase properties, ImageInfo imageInfo) {
			SetImageProperties(properties, imageInfo, ImageInfo.Empty);
		}
		public static void SetImageProperties(ImagePropertiesBase properties, ImageInfo imageInfo, ImageInfo disabledImageInfo) {
			if(!imageInfo.IsUrlEmpty) {
				properties.Url = imageInfo.ImageUrl;
				properties.Height = imageInfo.Height;
				properties.Width = imageInfo.Width;
			}
			if(!disabledImageInfo.IsUrlEmpty) {
				if(properties is ItemImagePropertiesBase) {
					ItemImagePropertiesBase itemImageProperties = (ItemImagePropertiesBase)properties;
					itemImageProperties.UrlDisabled = disabledImageInfo.ImageUrl;
				}
				else if(properties is ButtonImagePropertiesBase) {
					ButtonImagePropertiesBase buttonImageProperties = (ButtonImagePropertiesBase)properties;
					buttonImageProperties.UrlDisabled = disabledImageInfo.ImageUrl;
				}
			}
		}
		public static void SetImageProperties(ImagePropertiesBase properties, string imageName) {
			if(!string.IsNullOrEmpty(imageName)) {
				ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName, true);
				SetImageProperties(properties, imageInfo, imageName);
			}
		}
		internal static void SetImageProperties(ImagePropertiesBase properties, ImageInfo imageInfo, string imageName) {
			if(!string.IsNullOrEmpty(imageName)) {
				ImageInfo disabledImageInfo = ImageLoader.Instance.GetImageInfo(imageName, false);
				SetImageProperties(properties, imageInfo, disabledImageInfo);
			}
		}
		public static void ClearImageProperties(ImagePropertiesBase properties) {
			properties.Url = null;
			properties.Height = Unit.Empty;
			properties.Width = Unit.Empty;
			if(properties is ItemImagePropertiesBase) {
				ItemImagePropertiesBase itemImageProperties = (ItemImagePropertiesBase)properties;
				itemImageProperties.UrlDisabled = null;
			}
			else if(properties is ButtonImagePropertiesBase) {
				ButtonImagePropertiesBase buttonImageProperties = (ButtonImagePropertiesBase)properties;
				buttonImageProperties.UrlDisabled = null;
			}
		}
		public static void SetImageProperties(ASPxImage image, ImageInfo imageInfo) {
			image.ImageUrl = imageInfo.ImageUrl;
			image.Height = imageInfo.Height;
			image.Width = imageInfo.Width;
		}
	}
}
