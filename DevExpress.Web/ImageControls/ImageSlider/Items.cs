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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web {
	public class ImageSliderItemBase : CollectionItem {
		private object dataItem = null;
		private ITemplate template = null;
		private ITemplate textTemplate = null;
		private ITemplate thumbnailTemplate = null;
		protected ASPxImageSliderBase ImageSlider {
			get { return (Collection as ImageSliderItemCollectionBase).ImageSliderBase; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem {
			get { return dataItem; }
			set { SetDataItem(value); }
		}
		[TemplateContainer(typeof(ImageSliderItemTemplateContainerBase))]
		protected  internal ITemplate TemplateInternal {
			get { return template; }
			set {
				template = value;
				TemplatesChanged();
			}
		}
		[TemplateContainer(typeof(ImageSliderItemTemplateContainerBase))]
		protected internal ITemplate TextTemplateInternal {
			get { return textTemplate; }
			set {
				textTemplate = value;
				TemplatesChanged();
			}
		}
		[TemplateContainer(typeof(ImageSliderItemTemplateContainerBase))]
		protected internal ITemplate ThumbnailTemplateInternal {
			get { return thumbnailTemplate; }
			set {
				thumbnailTemplate = value;
				TemplatesChanged();
			}
		}
		protected internal string BinaryImageUrlInternal {
			get { return GetStringProperty("BinaryImageUrlInternal", string.Empty); }
			set { SetStringProperty("BinaryImageUrlInternal", string.Empty, value); }
		}
		protected internal string BinaryThumbnailUrlInternal {
			get { return GetStringProperty("BinaryThumbnailUrlInternal", string.Empty); }
			set { SetStringProperty("BinaryThumbnailUrlInternal", string.Empty, value); }
		}
		protected internal byte[] ImageContentBytesInternal { get; set; }
		protected internal byte[] ThumbnailContentBytesInternal { get; set; }
		protected internal string ImageUrlInternal {
			get { return GetStringProperty("ImageUrlInternal", string.Empty); }
			set { SetStringProperty("ImageUrlInternal", string.Empty, value); }
		}
		protected internal string ThumbnailUrlInternal {
			get { return GetStringProperty("ThumbnailUrlInternal", string.Empty); }
			set { SetStringProperty("ThumbnailUrlInternal", string.Empty, value); }
		}
		protected internal string NavigateUrlInternal {
			get { return GetStringProperty("NavigateUrlInternal", string.Empty); }
			set { SetStringProperty("NavigateUrlInternal", string.Empty, value); }
		}
		protected internal string TextInternal {
			get { return GetStringProperty("TextInternal", string.Empty); }
			set { SetStringProperty("TextInternal", string.Empty, value); }
		}
		protected internal string NameInternal {
			get { return GetStringProperty("NameInternal", string.Empty); }
			set { SetStringProperty("NameInternal", string.Empty, value); }
		}
		public ImageSliderItemBase()
			: base() {
		}
		public override void Assign(CollectionItem source) {
			if(source is ImageSliderItemBase) {
				ImageSliderItemBase src = source as ImageSliderItemBase;
				ImageUrlInternal = src.ImageUrlInternal;
				ThumbnailUrlInternal = src.ThumbnailUrlInternal;
				NavigateUrlInternal = src.NavigateUrlInternal;
				TextInternal = src.TextInternal;
				NameInternal = src.NameInternal;
				TemplateInternal = src.TemplateInternal;
				TextTemplateInternal = src.TextTemplateInternal;
				ThumbnailTemplateInternal = src.ThumbnailTemplateInternal;
				ImageContentBytesInternal = src.ImageContentBytesInternal;
				ThumbnailContentBytesInternal = src.ThumbnailContentBytesInternal;
				BinaryImageUrlInternal = src.BinaryImageUrlInternal;
				BinaryThumbnailUrlInternal = src.BinaryThumbnailUrlInternal;
			}
			base.Assign(source);
		}
		protected internal void SetDataItem(object value) {
			this.dataItem = value;
		}
		protected internal virtual string AlternateText {
			get {
				return TextInternal;
			}
		}
	}
	public class ImageSliderItem : ImageSliderItemBase {
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(ImageSliderItemTemplateContainerBase))]
		public ITemplate Template {
			get { return TemplateInternal; }
			set { TemplateInternal = value; }
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(ImageSliderItemTemplateContainerBase))]
		public ITemplate TextTemplate {
			get { return TextTemplateInternal; }
			set { TextTemplateInternal = value; }
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TemplateContainer(typeof(ImageSliderItemTemplateContainerBase))]
		public ITemplate ThumbnailTemplate {
			get { return ThumbnailTemplateInternal; }
			set { ThumbnailTemplateInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderItemBinaryImageUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(null), Localizable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string BinaryImageUrl {
			get {
				if(ImageSlider != null)
					return ImageSlider.ResolveUrl(BinaryImageUrlInternal);
				return BinaryImageUrlInternal;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderItemBinaryThumbnailUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(null), Localizable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string BinaryThumbnailUrl {
			get {
				if(ImageSlider != null)
					return ImageSlider.ResolveUrl(BinaryThumbnailUrlInternal);
				return BinaryThumbnailUrlInternal;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderItemImageContentBytes"),
#endif
		NotifyParentProperty(true), DefaultValue(null), Localizable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public byte[] ImageContentBytes {
			get { return ImageContentBytesInternal; }
			set { ImageContentBytesInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderItemThumbnailContentBytes"),
#endif
		NotifyParentProperty(true), DefaultValue(null), Localizable(false), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public byte[] ThumbnailContentBytes {
			get { return ThumbnailContentBytesInternal; }
			set { ThumbnailContentBytesInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderItemImageUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ImageUrl {
			get { return ImageUrlInternal; }
			set { ImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderItemThumbnailUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ThumbnailUrl {
			get { return ThumbnailUrlInternal; }
			set { ThumbnailUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderItemNavigateUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string NavigateUrl {
			get { return NavigateUrlInternal; }
			set { NavigateUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderItemText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string Text {
			get { return TextInternal; }
			set { TextInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageSliderItemName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string Name
		{
			get { return NameInternal; }
			set { NameInternal = value; }
		}
		public ImageSliderItem()
			: base() {
		}
		public ImageSliderItem(string imageUrl)
			: this(imageUrl, string.Empty, string.Empty, string.Empty, string.Empty) {
		}
		public ImageSliderItem(string imageUrl, string thumbnailUrl) :
			this(imageUrl, thumbnailUrl, string.Empty, string.Empty, string.Empty) {
		}
		public ImageSliderItem(string imageUrl, string thumbnailUrl, string navigateUrl) :
			this(imageUrl, thumbnailUrl, navigateUrl, string.Empty, string.Empty) {
		}
		public ImageSliderItem(string imageUrl, string thumbnailUrl, string navigateUrl, string text) :
			this(imageUrl, thumbnailUrl, navigateUrl, text, string.Empty) {
		}
		public ImageSliderItem(string imageUrl, string thumbnailUrl, string navigateUrl, string text, string name)
			: base() {
			ImageUrl = imageUrl;
			ThumbnailUrl = thumbnailUrl;
			NavigateUrl = navigateUrl;
			Text = text;
			Name = name;
		}
	}
	public class ImageSliderItemCollectionBase : Collection<ImageSliderItemBase> {
		protected internal ASPxImageSliderBase ImageSliderBase {
			get { return Owner as ASPxImageSliderBase; }
		}
		public ImageSliderItemCollectionBase()
			: base() {
		}
		public ImageSliderItemCollectionBase(ASPxImageSliderBase control)
			: base(control) {
		}
		protected override void OnChanged() {
			if(ImageSliderBase != null)
				ImageSliderBase.ItemsChanged();
		}
	}
	public class ImageSliderItemCollection : ImageSliderItemCollectionBase {
#if !SL
	[DevExpressWebLocalizedDescription("ImageSliderItemCollectionItem")]
#endif
		public new ImageSliderItem this[int index] {
			get { return (GetItem(index) as ImageSliderItem); }
		}
		protected override Type GetKnownType() {
			return typeof(ImageSliderItem);
		}
		public ImageSliderItemCollection()
			: base() {
		}
		public ImageSliderItemCollection(ASPxImageSliderBase control)
			: base(control) {
		}
		public ImageSliderItem Add() {
			return Add(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public ImageSliderItem Add(string imageUrl) {
			return Add(imageUrl, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public ImageSliderItem Add(string imageUrl, string thumbnailUrl) {
			return Add(imageUrl, thumbnailUrl, string.Empty, string.Empty, string.Empty);
		}
		public ImageSliderItem Add(string imageUrl, string thumbnailUrl, string navigateUrl) {
			return Add(imageUrl, thumbnailUrl, navigateUrl, string.Empty, string.Empty);
		}
		public ImageSliderItem Add(string imageUrl, string thumbnailUrl, string navigateUrl, string text) {
			return Add(imageUrl, thumbnailUrl, navigateUrl, text, string.Empty);
		}
		public ImageSliderItem Add(string imageUrl, string thumbnailUrl, string navigateUrl, string text, string name) {
			ImageSliderItem item = new ImageSliderItem(imageUrl, thumbnailUrl, navigateUrl, text, name);
			Add(item);
			return item;
		}
	}
}
