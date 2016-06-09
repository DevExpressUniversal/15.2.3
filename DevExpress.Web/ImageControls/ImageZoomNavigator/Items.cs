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
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web {
	public class ImageZoomNavigatorItem : ImageSliderItemBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorItemImageUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ImageUrl {
			get { return ImageUrlInternal; }
			set { ImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorItemLargeImageUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string LargeImageUrl {
			get { return GetStringProperty("LargeImageUrl", string.Empty); }
			set { SetStringProperty("LargeImageUrl", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorItemThumbnailUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string ThumbnailUrl {
			get { return ThumbnailUrlInternal; }
			set { ThumbnailUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorItemZoomWindowText"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string ZoomWindowText {
			get { return GetStringProperty("ZoomWindowText", string.Empty); }
			set { SetStringProperty("ZoomWindowText", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ImageZoomNavigatorItemExpandWindowText"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string ExpandWindowText {
			get { return GetStringProperty("ExpandWindowText", string.Empty); }
			set { SetStringProperty("ExpandWindowText", string.Empty, value); }
		}
		protected internal string BinaryLargeImageUrlInternal {
			get { return GetStringProperty("BinaryLargeImageUrlInternal", string.Empty); }
			set { SetStringProperty("BinaryLargeImageUrlInternal", string.Empty, value); }
		}
		protected internal override string AlternateText {
			get {
				return ZoomWindowText;
			}
		}
		public override void Assign(CollectionItem source) {
			if(source is ImageZoomNavigatorItem) {
				ImageZoomNavigatorItem src = source as ImageZoomNavigatorItem;
				LargeImageUrl = src.LargeImageUrl;
				ZoomWindowText = src.ZoomWindowText;
				ExpandWindowText = src.ExpandWindowText;
				BinaryLargeImageUrlInternal = src.BinaryLargeImageUrlInternal;
			}
			base.Assign(source);
		}
		public ImageZoomNavigatorItem()
			: base() {
		}
		public ImageZoomNavigatorItem(string imageUrl)
			: this(imageUrl, string.Empty, string.Empty, string.Empty, string.Empty) {
		}
		public ImageZoomNavigatorItem(string imageUrl, string largeImageUrl)
			: this(imageUrl, largeImageUrl, string.Empty, string.Empty, string.Empty) {
		}
		public ImageZoomNavigatorItem(string imageUrl, string largeImageUrl, string thumbnailUrl)
			: this(imageUrl, largeImageUrl, thumbnailUrl, string.Empty, string.Empty) {
		}
		public ImageZoomNavigatorItem(string imageUrl, string largeImageUrl, string thumbnailUrl, string zoomWindowText)
			: this(imageUrl, largeImageUrl, thumbnailUrl, zoomWindowText, string.Empty) {
		}
		public ImageZoomNavigatorItem(string imageUrl, string largeImageUrl, string thumbnailUrl, string zoomWindowText, string expandWindowText)
			: base() {
			ImageUrl = imageUrl;
			LargeImageUrl = largeImageUrl;
			ThumbnailUrl = thumbnailUrl;
			ZoomWindowText = zoomWindowText;
			ExpandWindowText = expandWindowText;
		}
	}
	public class ImageZoomNavigatorItemCollection : ImageSliderItemCollectionBase {
#if !SL
	[DevExpressWebLocalizedDescription("ImageZoomNavigatorItemCollectionItem")]
#endif
		public new ImageZoomNavigatorItem this[int index] {
			get { return (GetItem(index) as ImageZoomNavigatorItem); }
		}
		protected override Type GetKnownType() {
			return typeof(ImageZoomNavigatorItem);
		}
		public ImageZoomNavigatorItemCollection()
			: base() {
		}
		public ImageZoomNavigatorItemCollection(ASPxImageSliderBase control)
			: base(control) {
		}
		public ImageZoomNavigatorItem Add() {
			return Add(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public ImageZoomNavigatorItem Add(string imageUrl) {
			return Add(imageUrl, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public ImageZoomNavigatorItem Add(string imageUrl, string largeImageUrl) {
			return Add(imageUrl, largeImageUrl, string.Empty, string.Empty, string.Empty);
		}
		public ImageZoomNavigatorItem Add(string imageUrl, string largeImageUrl, string thumbnailUrl) {
			return Add(imageUrl, largeImageUrl, thumbnailUrl, string.Empty, string.Empty);
		}
		public ImageZoomNavigatorItem Add(string imageUrl, string largeImageUrl, string thumbnailUrl, string zoomWindowText) {
			return Add(imageUrl, largeImageUrl, thumbnailUrl, zoomWindowText, string.Empty);
		}
		public ImageZoomNavigatorItem Add(string imageUrl, string largeImageUrl, string thumbnailUrl, string zoomWindowText, string expandWindowText) {
			ImageZoomNavigatorItem item = new ImageZoomNavigatorItem(imageUrl, largeImageUrl, thumbnailUrl, zoomWindowText, expandWindowText);
			Add(item);
			return item;
		}
	}
}
