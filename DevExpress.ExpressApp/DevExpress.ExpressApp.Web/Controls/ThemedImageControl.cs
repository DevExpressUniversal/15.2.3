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

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Web.Controls {
	[ToolboxItem(false)] 
	public class ThemedImageControl : Image {
		private string defaultThemeImageLocation;
		private string imageName;
		private bool useLargeImage = false;
		public ThemedImageControl() {
			ImageAlign = ImageAlign.Middle;
		}
		public string DefaultThemeImageLocation {
			get { return defaultThemeImageLocation; }
			set { defaultThemeImageLocation = value; }
		}
		public string ImageName {
			get { return imageName; }
			set { imageName = value; }
		}
		public bool UseLargeImage {
			get { return useLargeImage; }
			set { useLargeImage = value; }
		}
		private void UpdateImageUrl() {
			if (string.IsNullOrEmpty(ImageName) || Page == null) {
				Visible = false;
				return;
			}
			if (string.IsNullOrEmpty(DefaultThemeImageLocation)) {
				LoadImageFromXafResources();
			}
			else {
				if (DefaultThemeImageLocation.StartsWith("~")) {
					LoadImageFromFile();
				}
				else {
					LoadImageFromDXResources();
				}
			}
		}
		private void LoadImageFromXafResources() {
			ImageInfo imageInfo = UseLargeImage ? ImageLoader.Instance.GetLargeImageInfo(ImageName) : ImageLoader.Instance.GetImageInfo(ImageName);
			if (imageInfo.IsEmpty) {
				Visible = false;
			}
			else {
				ImageUrl = imageInfo.ImageUrl;
			}
		}
		private string GetImageLocation() {
			const string separator = "/";
			string path = string.Format(DefaultThemeImageLocation, DevExpress.ExpressApp.Web.Templates.BaseXafPage.CurrentTheme);
			if(!string.IsNullOrEmpty(path) && !path.EndsWith(separator)) {
				path += separator;
			}
			return string.IsNullOrEmpty(path) ? ImageName : path + ImageName;
		}
		private void LoadImageFromDXResources() {
			ImageUrl = DevExpress.Web.Internal.ResourceManager.GetResourceUrl(Page, GetImageLocation());
		}
		private void LoadImageFromFile() {
			ImageUrl = GetImageLocation();
		}
		protected override void Render(HtmlTextWriter writer) {
			UpdateImageUrl();
			if(Visible) {
				base.Render(writer);
		}
	}
}
}
