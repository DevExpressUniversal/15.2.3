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
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web {
	public class ImageGalleryItem : DataViewItem {
		private ITemplate textTemplate = null;
		private ITemplate fullscreenTextTemplate = null;
		private byte[] imageContentBytes = null;
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(ImageGalleryThumbnailTemplateContainer))]
		public ITemplate TextTemplate {
			get { return textTemplate; }
			set {
				textTemplate = value;
				TemplatesChanged();
			}
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(ImageGalleryThumbnailTemplateContainer))]
		public ITemplate FullscreenViewerTextTemplate {
			get { return fullscreenTextTemplate; }
			set {
				fullscreenTextTemplate = value;
				TemplatesChanged();
			}
		}
		internal void ResetBinaryImageUrls() {
			BinaryImageUrl = string.Empty;
			BinaryThumbnailUrl = string.Empty;
			BinaryFullscreenViewerThumbnailUrl = string.Empty;
		}
		[
		NotifyParentProperty(true), DefaultValue(null), Localizable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public byte[] ImageContentBytes {
			get { return imageContentBytes; }
			set {
				imageContentBytes = value;
				ResetBinaryImageUrls();
			}
		}
		internal string BinaryImageUrl {
			get { return GetStringProperty("BinaryImageUrl", string.Empty); }
			set { SetStringProperty("BinaryImageUrl", string.Empty, value); }
		}
		internal string BinaryThumbnailUrl {
			get { return GetStringProperty("BinaryThumbnailUrl", string.Empty); }
			set { SetStringProperty("BinaryThumbnailUrl", string.Empty, value); }
		}
		internal string BinaryFullscreenViewerThumbnailUrl {
			get { return GetStringProperty("BinaryFullscreenViewerThumbnailUrl", string.Empty); }
			set { SetStringProperty("BinaryFullscreenViewerThumbnailUrl", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryItemFullscreenViewerThumbnailUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string FullscreenViewerThumbnailUrl
		{
			get { return GetStringProperty("FullscreenViewerThumbnailUrl", ""); }
			set { SetStringProperty("FullscreenViewerThumbnailUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryItemThumbnailUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ThumbnailUrl
		{
			get { return GetStringProperty("ThumbnailUrl", ""); }
			set { SetStringProperty("ThumbnailUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryItemImageUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ImageUrl
		{
			get { return GetStringProperty("ImageUrl", ""); }
			set { SetStringProperty("ImageUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryItemText"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true)]
		public string Text
		{
			get { return GetStringProperty("Text", ""); }
			set { SetStringProperty("Text", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryItemFullscreenViewerText"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true)]
		public string FullscreenViewerText
		{
			get { return GetStringProperty("FullscreenViewerText", ""); }
			set { SetStringProperty("FullscreenViewerText", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryItemNavigateUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string NavigateUrl
		{
			get { return GetStringProperty("NavigateUrl", string.Empty); }
			set { SetStringProperty("NavigateUrl", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageGalleryItemName"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true)]
		public string Name
		{
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ImageGalleryItemCollection Collection {
			get { return (ImageGalleryItemCollection)base.Collection; }
		}
		public ImageGalleryItem()
			: base() {
		}
		public ImageGalleryItem(string imageUrl)
			: this(imageUrl, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty) {
		}
		public ImageGalleryItem(string imageUrl, string text)
			: this(imageUrl, text, string.Empty, string.Empty, string.Empty, string.Empty) {
		}
		public ImageGalleryItem(string imageUrl, string text, string thumbnailUrl)
			: this(imageUrl, text, thumbnailUrl, string.Empty, string.Empty, string.Empty) {
		}
		public ImageGalleryItem(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl)
			: this(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl, string.Empty, string.Empty) {
		}
		public ImageGalleryItem(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl, string fullscreenViewerText)
			: this(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl, fullscreenViewerText, string.Empty) {
		}
		public ImageGalleryItem(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl, string fullscreenViewerText, string navigateUrl) {
			FullscreenViewerThumbnailUrl = fullscreenViewerThumbnailUrl;
			ThumbnailUrl = thumbnailUrl;
			ImageUrl = imageUrl;
			Text = text;
			FullscreenViewerText = fullscreenViewerText;
			NavigateUrl = navigateUrl;
		}
		public override void Assign(CollectionItem source) {
			if(source is ImageGalleryItem) {
				ImageGalleryItem src = source as ImageGalleryItem;
				if(src != null) {
					ImageUrl = src.ImageUrl;
					Text = src.Text;
					FullscreenViewerThumbnailUrl = src.FullscreenViewerThumbnailUrl;
					ThumbnailUrl = src.ThumbnailUrl;
					FullscreenViewerText = src.FullscreenViewerText;
					TextTemplate = src.TextTemplate;
					FullscreenViewerTextTemplate = src.FullscreenViewerTextTemplate;
					Name = src.Name;
					ImageContentBytes = src.ImageContentBytes;
					BinaryImageUrl = src.BinaryImageUrl;
					BinaryThumbnailUrl = src.BinaryThumbnailUrl;
					BinaryFullscreenViewerThumbnailUrl = src.BinaryFullscreenViewerThumbnailUrl;
				}
			}
			base.Assign(source);
		}
	}
	public class ImageGalleryItemCollection : DataViewItemCollection {
#if !SL
	[DevExpressWebLocalizedDescription("ImageGalleryItemCollectionItem")]
#endif
		public new ImageGalleryItem this[int index] {
			get { return (GetItem(index) as ImageGalleryItem); }
		}
		protected override Type GetKnownType() {
			return typeof(ImageGalleryItem);
		}
		public ImageGalleryItemCollection()
			: base() {
		}
		public ImageGalleryItemCollection(ASPxImageGallery imageGallery)
			: base(imageGallery) {
		}
		protected internal ASPxImageGallery ImageGallery {
			get { return Owner as ASPxImageGallery; }
		}
		public new ImageGalleryItem Add() {
			return Add(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public ImageGalleryItem Add(string imageUrl) {
			return Add(imageUrl, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public ImageGalleryItem Add(string imageUrl, string text) {
			return Add(imageUrl, text, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public ImageGalleryItem Add(string imageUrl, string text, string thumbnailUrl) {
			return Add(imageUrl, text, thumbnailUrl, string.Empty, string.Empty, string.Empty);
		}
		public ImageGalleryItem Add(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl) {
			return Add(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl, string.Empty, string.Empty);
		}
		public ImageGalleryItem Add(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl, string fullscreenViewerText) {
			return Add(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl, fullscreenViewerText, string.Empty);
		}
		public ImageGalleryItem Add(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl, string fullscreenViewerText, string navigateUrl) {
			ImageGalleryItem item = new ImageGalleryItem(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl, fullscreenViewerText, navigateUrl);
			Add(item);
			return item;
		}
	}
}
