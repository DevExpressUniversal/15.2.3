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
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	public class MVCxImageGalleryItem : ImageGalleryItem {
		public MVCxImageGalleryItem()
			: base() {
		}
		public MVCxImageGalleryItem(string imageUrl)
			: base(imageUrl) {
		}
		public MVCxImageGalleryItem(string imageUrl, string text)
			: base(imageUrl, text) {
		}
		public MVCxImageGalleryItem(string imageUrl, string text, string thumbnailUrl)
			: base(imageUrl, text, thumbnailUrl) {
		}
		public MVCxImageGalleryItem(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl)
			: base(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl) {
		}
		public MVCxImageGalleryItem(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl, string fullscreenViewerText)
			: base(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl, fullscreenViewerText) {
		}
		public MVCxImageGalleryItem(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl, string fullscreenViewerText, string navigateUrl)
			: base(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl, fullscreenViewerText, navigateUrl) {
		}
		protected internal string TextTemplateContent { get; set; }
		protected internal Action<ImageGalleryThumbnailTemplateContainer> TextTemplateContentMethod { get; set; }
		protected internal string FullscreenViewerTextTemplateContent { get; set; }
		protected internal Action<ImageGalleryThumbnailTemplateContainer> FullscreenViewerTextTemplateContentMethod { get; set; }
		public void SetTextTemplateContent(Action<ImageGalleryThumbnailTemplateContainer> contentMethod) {
			TextTemplateContentMethod = contentMethod;
		}
		public void SetTextTemplateContent(string content) {
			TextTemplateContent = content;
		}
		public void SetFullscreenViewerTextTemplateContent(Action<ImageGalleryThumbnailTemplateContainer> contentMethod) {
			FullscreenViewerTextTemplateContentMethod = contentMethod;
		}
		public void SetFullscreenViewerTextTemplateContent(string content) {
			FullscreenViewerTextTemplateContent = content;
		}
	}
	public class MVCxImageGalleryItemCollection : ImageGalleryItemCollection {
		public MVCxImageGalleryItemCollection()
			: base() {
		}
		public new MVCxImageGalleryItem this[int index] {
			get { return GetItem(index) as MVCxImageGalleryItem; }
		}
		public void Add(Action<MVCxImageGalleryItem> method) {
			method(Add());
		}
		public void Add(MVCxImageGalleryItem item) {
			base.Add(item);
		}
		public new MVCxImageGalleryItem Add() {
			MVCxImageGalleryItem item = new MVCxImageGalleryItem();
			Add(item);
			return item;
		}
		public new MVCxImageGalleryItem Add(string imageUrl) {
			return Add(imageUrl, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public new MVCxImageGalleryItem Add(string imageUrl, string text) {
			return Add(imageUrl, text, string.Empty, string.Empty, string.Empty, string.Empty);
		}
		public new MVCxImageGalleryItem Add(string imageUrl, string text, string thumbnailUrl) {
			return Add(imageUrl, text, thumbnailUrl, string.Empty, string.Empty, string.Empty);
		}
		public new MVCxImageGalleryItem Add(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl) {
			return Add(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl, string.Empty, string.Empty);
		}
		public new MVCxImageGalleryItem Add(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl, string fullscreenViewerText) {
			return Add(imageUrl, text, thumbnailUrl, fullscreenViewerThumbnailUrl, fullscreenViewerText, string.Empty);
		}
		public new MVCxImageGalleryItem Add(string imageUrl, string text, string thumbnailUrl, string fullscreenViewerThumbnailUrl, string fullscreenViewerText, string navigateUrl) {
			MVCxImageGalleryItem item = new MVCxImageGalleryItem(imageUrl, thumbnailUrl, fullscreenViewerThumbnailUrl, fullscreenViewerText, navigateUrl);
			Add(item);
			return item;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxImageGalleryItem);
		}
	}
}
