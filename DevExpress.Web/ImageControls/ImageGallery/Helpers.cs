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

using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public abstract class ImageGalleryBaseHelper {
		protected ASPxImageGallery ImageGallery { get; private set; }
		public ImageGalleryBaseHelper(ASPxImageGallery imageGallery) {
			ImageGallery = imageGallery;
		}
	}
	public class ImageGalleryDataHelper : ImageGalleryBaseHelper {
		public ImageGalleryDataHelper(ASPxImageGallery imageGallery) : base(imageGallery) { }
		public ImageGalleryItemCollection Items {
			get { return ImageGallery.Items; }
		}
		public string GetNavigateUrl(ImageGalleryItem item) {
			return string.Format(ImageGallery.NavigateUrlFormatString, item.NavigateUrl);
		}
		public string GetThumbnailText(ImageGalleryItem item) {
			return ImageGallery.HtmlEncode(item.Text);
		}
		public ElementVisibilityMode GetThumbnailTextVisibility() {
			ElementVisibilityMode visibility = ImageGallery.TextVisibility;
			if((visibility == ElementVisibilityMode.Faded || visibility == ElementVisibilityMode.OnMouseOver) && RenderUtils.Browser.Platform.IsTouchUI)
				visibility = ElementVisibilityMode.None;
			return visibility;
		}
		public string GetDataViewItemName(ImageGalleryItem item) {
			if(ImageGallery.SettingsFullscreenViewer.Visible)
				return string.Empty;
			return item.Name;
		}
		public ITemplate GetThumbnailTextTemplate(ImageGalleryItem item) {
			ITemplate template = ImageGallery.ItemTextTemplate;
			if(item.TextTemplate != null)
				template = item.TextTemplate;
			return template;
		}
		private bool? hasFVTextTemplate = null;
		public bool HasFullscreenViewerTextTemplate {
			get {
				if(ImageGallery.FullscreenViewerItemTextTemplate != null)
					return true;
				if(!hasFVTextTemplate.HasValue)
					hasFVTextTemplate = CreateHasFullscreenViewerItemTextTemplateValue();
				return hasFVTextTemplate.Value;
			}
		}
		public bool HasFullscreenViewerStaticTextTemplate {
			get { return GetFullscreenViewerStaticTextTemplate() != null; }
		}
		public bool HasFullscreenViewerTemplates {
			get { return HasFullscreenViewerTextTemplate || HasFullscreenViewerStaticTextTemplate; }
		}
		public bool CanRenderFullscreenViewerTextArea {
			get {
				if(ImageGallery.SettingsFullscreenViewer.ShowTextArea && (HasFullscreenViewerText || HasFullscreenViewerTextTemplate || HasFullscreenViewerStaticTextTemplate))
					return true;
				return false;
			}
		}
		private bool? hasFullscreenViewerText = null;
		public bool HasFullscreenViewerText {
			get {
				if(!hasFullscreenViewerText.HasValue)
					hasFullscreenViewerText = CreateHasFullscreenViewerTextValue();
				return hasFullscreenViewerText.Value;
			}
		}
		public ITemplate GetFullscreenViewerStaticTextTemplate() {
			return ImageGallery.FullscreenViewerTextTemplate;
		}
		public ITemplate GetFullscreenViewerTextTemplate(ImageGalleryItem item) {
			ITemplate template = ImageGallery.FullscreenViewerItemTextTemplate;
			if(item.FullscreenViewerTextTemplate != null)
				template = item.FullscreenViewerTextTemplate;
			return template;
		}
		public string GetFullscreenViewerText(ImageGalleryItem item) {
			string text = item.Text;
			if(!string.IsNullOrEmpty(item.FullscreenViewerText))
				text = item.FullscreenViewerText;
			return text;
		}
		private bool CreateHasFullscreenViewerTextValue() {
			foreach(ImageGalleryItem item in Items)
				if(!string.IsNullOrEmpty(item.Text) || !string.IsNullOrEmpty(item.FullscreenViewerText))
					return true;
			return false;
		}
		private bool CreateHasFullscreenViewerItemTextTemplateValue() {
			foreach(ImageGalleryItem item in Items)
				if(item.FullscreenViewerTextTemplate != null)
					return true;
			return false;
		}
		public string GetFullscreenViewerImageUrl(ImageGalleryItem item) {
			string imageUrl = item.BinaryImageUrl;
			if(string.IsNullOrEmpty(imageUrl))
				imageUrl = string.Format(ImageGallery.ImageUrlFormatString, item.ImageUrl);
			return imageUrl;
		}
		public string GetFullscreenViewerThumbnailUrl(ImageGalleryItem item) {
			string imageUrl = item.BinaryFullscreenViewerThumbnailUrl;
			if(string.IsNullOrEmpty(imageUrl))
				imageUrl = string.Format(ImageGallery.FullscreenViewerThumbnailUrlFormatString, item.FullscreenViewerThumbnailUrl);
			if(string.IsNullOrEmpty(imageUrl))
				imageUrl = GetFullscreenViewerImageUrl(item);
			return imageUrl;
		}
		public string GetThumbnailUrl(ImageGalleryItem item) {
			string imageUrl = item.BinaryThumbnailUrl;
			if(string.IsNullOrEmpty(imageUrl))
				imageUrl = string.Format(ImageGallery.ThumbnailUrlFormatString, item.ThumbnailUrl);
			if(string.IsNullOrEmpty(imageUrl))
				imageUrl = GetFullscreenViewerImageUrl(item);
			return imageUrl;
		}
	}
	public class ImageGalleryContstants {
		internal const string FullscreenViewerItemTextTemplateClass = "dxigFVIT";
		internal const string FullscreenViewerCell = "FVCell";
		internal const string FullscreenViewerPopupID = "Popup";
		internal const string FullscreenViewerCloseButtonID = "ClsBtn";
		internal const string FullscreenViewerImageSliderID = "Slider";
		internal const string FullscreenViewerNavigationBarID = "NavigationBar";
		internal const string FullscreenViewerPrevButtonID = "prevBtn";
		internal const string FullscreenViewerNextButtonID = "nextBtn";
		internal const string OnImageSliderItemClick = "function() {{ASPx.IGItemClick(\"{0}\")}}";
		internal const string OnImageLoad = "_aspxIGImageLoad(this,\"{0}\",{1},{2},{3},{4},{5},{6},\"{7}\")";
		internal const int DefaultItemSpacing = 6;
		internal const int DefaultPagerPanelSpacing = 6;
		internal const string DefaultThumbnailWidth = "200";
		internal const string DefaultThumbnailHeight = "200";
		internal const string DefaultFullscreenViewerWidth = "1200";
		internal const string DefaultFullscreenViewerHeight = "1200";
		internal const string DefaultFullscreenViewerThumbnailWidth = "90";
		internal const string DefaultFullscreenViewerThumbnailHeight = "90";
		internal static string GetFullscreenViewerCloseButtonID() { return string.Format("{0}_{1}", FullscreenViewerPopupID, FullscreenViewerCloseButtonID); }
		internal static string GetFullscreenViewerPrevButtonID() { return string.Format("{0}_{1}", FullscreenViewerPopupID, FullscreenViewerPrevButtonID); }
		internal static string GetFullscreenViewerNextButtonID() { return string.Format("{0}_{1}", FullscreenViewerPopupID, FullscreenViewerNextButtonID); }
	}
}
