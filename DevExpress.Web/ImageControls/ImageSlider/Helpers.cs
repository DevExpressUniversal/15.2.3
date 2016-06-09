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

using System.Collections.Generic;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class ImageSliderDataHelper {
		protected ASPxImageSliderBase ImageSlider { get; private set; }
		protected string ImageSourceFolder { get { return (ImageSlider as ASPxImageSlider).ImageSourceFolder; } }
		public bool HasImageFolderPath { get { return !string.IsNullOrEmpty(ImageSourceFolder); } }
		public bool HasNavigateUrl {
			get {
				foreach(ImageSliderItemBase item in ImageSlider.ItemsInternal)
					if(!string.IsNullOrEmpty(item.NavigateUrlInternal))
						return true;
				return false;
			}
		}
		public ImageSliderDataHelper(ASPxImageSliderBase imageSliderBase) {
			ImageSlider = imageSliderBase;
		}
		public string GetItemText(ImageSliderItemBase item) {
			return ImageSlider.HtmlEncode(item.TextInternal);
		}
		public string GetNavigateUrl(ImageSliderItemBase item, bool resolveUrl = true) {
			string url = string.Format(ImageSlider.NavigateUrlFormatStringInternal, item.NavigateUrlInternal);
			if(resolveUrl)
				return ResolveUrl(url);
			return url;
		}
		protected internal string GetThumbnailUrl(ImageSliderItemBase item) {
			TryCreateBinaryImages(item);
			string imageUrl = item.BinaryThumbnailUrlInternal;
			if(string.IsNullOrEmpty(imageUrl))
				imageUrl = string.Format(ImageSlider.ThumbnailUrlFormatStringInternal, item.ThumbnailUrlInternal);
			return ResolveUrl(ImageUtilsHelper.EncodeImageUrl(imageUrl));
		}
		protected internal string GetImageUrl(ImageSliderItemBase item, bool resolveUrl = true) {
			TryCreateBinaryImages(item);
			string imageUrl = item.BinaryImageUrlInternal;
			if(string.IsNullOrEmpty(imageUrl))
				imageUrl = ImageUtilsHelper.EncodeImageUrl(string.Format(ImageSlider.ImageUrlFormatStringInternal, item.ImageUrlInternal));
			return resolveUrl ? ResolveUrl(imageUrl) : imageUrl;
		}
		protected internal string GetLargeImageUrl(ImageZoomNavigatorItem item, bool resolveUrl = true) {
			TryCreateBinaryImages(item);
			string imageUrl = item.BinaryLargeImageUrlInternal;
			if(string.IsNullOrEmpty(imageUrl))
				imageUrl = ImageUtilsHelper.EncodeImageUrl(item.LargeImageUrl);
			return resolveUrl ? ResolveUrl(imageUrl) : imageUrl;
		}
		protected void TryCreateBinaryImages(ImageSliderItemBase item) {
			ImageSlider.CreateBinaryImages(item);
		}
		protected string ResolveUrl(string path) {
			return ImageSlider.ResolveClientUrl(path);
		}
		protected internal string GetBaseUrlForAlternateText(ImageSliderItemBase item) {
			if(!string.IsNullOrEmpty(item.BinaryImageUrlInternal))
				return item.BinaryImageUrlInternal;
			if(!string.IsNullOrEmpty(item.ImageUrlInternal))
				return item.ImageUrlInternal;
			if(!string.IsNullOrEmpty(item.BinaryThumbnailUrlInternal))
				return item.BinaryImageUrlInternal;
			if(!string.IsNullOrEmpty(item.ThumbnailUrlInternal))
				return item.ThumbnailUrlInternal;
			return string.Empty;
		}
	}
	public class ImageSliderTemplateHelper {
		const string ItemTemplateContainerID = "IT";
		const string ItemTextTemplateContainerID = "ITT";
		const string ItemThumbnailTemplateContainerID = "INBT";
		private bool? hasTemplates = null;
		protected ASPxImageSliderBase ImageSlider { get; private set; }
		protected TemplatesContainer TemplateContainer { get; private set; }
		protected ImageSliderItemCollectionBase Items { get { return ImageSlider.ItemsInternal; } }
		protected Dictionary<ImageSliderItemBase, Control> ItemTemplates { get; private set; }
		protected Dictionary<ImageSliderItemBase, Control> ItemTextTemplates { get; private set; }
		protected Dictionary<ImageSliderItemBase, Control> ItemThumbnailTemplates { get; private set; }
		public ImageSliderTemplateHelper(ASPxImageSliderBase imageSlider) {
			ImageSlider = imageSlider;
			TemplateContainer = new TemplatesContainer();
			ItemTemplates = new Dictionary<ImageSliderItemBase, Control>();
			ItemTextTemplates = new Dictionary<ImageSliderItemBase, Control>();
			ItemThumbnailTemplates = new Dictionary<ImageSliderItemBase, Control>();
		}
		public void AddTemplatesToHierarchy() {
			ItemTemplates.Clear();
			ItemTextTemplates.Clear();
			ItemThumbnailTemplates.Clear();
			TemplateContainer.Controls.Clear();
			ImageSlider.Controls.Add(TemplateContainer);
			PopulateTemplates();
		}
		protected void PopulateTemplates() {
			foreach(ImageSliderItemBase item in Items) {
				ITemplate template = GetItemTemplate(item);
				ITemplate textTemplate = GetItemTextTemplate(item);
				ITemplate thumbnailTemplate = GetItemThumbnailTemplate(item);
				if(template != null)
					ItemTemplates.Add(item, GetTemplateControl(template, item, ItemTemplateContainerID));
				if(textTemplate != null)
					ItemTextTemplates.Add(item, GetTemplateControl(textTemplate, item, ItemTextTemplateContainerID));
				if(thumbnailTemplate != null)
					ItemThumbnailTemplates.Add(item, GetTemplateControl(thumbnailTemplate, item, ItemThumbnailTemplateContainerID));
			}
		}
		protected Control GetTemplateControl(ITemplate template, ImageSliderItemBase item, string containerId) {
			ImageSliderItemTemplateContainerBase container = ImageSlider.CreateItemTemplateContainer(item);
			container.AddToHierarchy(TemplateContainer, containerId + item.Index);
			template.InstantiateIn(container);
			return TemplateContainer.Controls[TemplateContainer.Controls.Count - 1];
		}
		protected ITemplate GetItemTemplate(ImageSliderItemBase item) {
			ITemplate template = item.TemplateInternal;
			if(template == null)
				template = ImageSlider.ItemTemplateInternal;
			return template;
		}
		protected ITemplate GetItemTextTemplate(ImageSliderItemBase item) {
			ITemplate template = item.TextTemplateInternal;
			if(template == null)
				template = ImageSlider.ItemTextTemplateInternal;
			return template;
		}
		protected ITemplate GetItemThumbnailTemplate(ImageSliderItemBase item) {
			ITemplate template = item.ThumbnailTemplateInternal;
			if(template == null)
				template = ImageSlider.ItemThumbnailTemplateInternal;
			return template;
		}
		public bool HasTemplates() {
			if(!hasTemplates.HasValue) {
				hasTemplates = false;
				foreach(ImageSliderItemBase item in Items)
					if(GetItemTemplate(item) != null || GetItemTextTemplate(item) != null || GetItemThumbnailTemplate(item) != null)
						hasTemplates = true;
			}
			return hasTemplates.Value;
		}
		public bool ContainsItemTemplate(ImageSliderItemBase item) {
			return GetItemTemplate(item) != null ? true : false;
		}
		public bool ContainsItemTextTemplate(ImageSliderItemBase item) {
			return GetItemTextTemplate(item) != null ? true : false;
		}
		public bool ContainsItemThumbnailTemplate(ImageSliderItemBase item) {
			return GetItemThumbnailTemplate(item) != null ? true : false;
		}
		public string GetItemTemplateResult(ImageSliderItemBase item) {
			return RenderUtils.GetRenderResult(ItemTemplates[item]);
		}
		public string GetItemTextTemplateResult(ImageSliderItemBase item) {
			return RenderUtils.GetRenderResult(ItemTextTemplates[item]);
		}
		public string GetItemThumbnailTemplateResult(ImageSliderItemBase item) {
			return RenderUtils.GetRenderResult(ItemThumbnailTemplates[item]);
		}
	}
}
