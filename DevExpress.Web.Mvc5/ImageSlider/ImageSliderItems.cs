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
	public class MVCxImageSliderItem : ImageSliderItem {
		public MVCxImageSliderItem()
			: base() {
		}
		public MVCxImageSliderItem(string imageUrl)
			: base(imageUrl) {
		}
		public MVCxImageSliderItem(string imageUrl, string thumbnailUrl)
			: base(imageUrl, thumbnailUrl) {
		}
		public MVCxImageSliderItem(string imageUrl, string thumbnailUrl, string navigateUrl)
			: base(imageUrl, thumbnailUrl, navigateUrl) {
		}
		public MVCxImageSliderItem(string imageUrl, string thumbnailUrl, string navigateUrl, string text)
			: base(imageUrl, thumbnailUrl, navigateUrl, text) {
		}
		public MVCxImageSliderItem(string imageUrl, string thumbnailUrl, string navigateUrl, string text, string name)
			: base(imageUrl, thumbnailUrl, navigateUrl, text, name) {
		}
		protected internal string TemplateContent { get; set; }
		protected internal Action<ImageSliderItemTemplateContainer> TemplateContentMethod { get; set; }
		protected internal string TextTemplateContent { get; set; }
		protected internal Action<ImageSliderItemTemplateContainer> TextTemplateContentMethod { get; set; }
		protected internal string ThumbnailTemplateContent { get; set; }
		protected internal Action<ImageSliderItemTemplateContainer> ThumbnailTemplateContentMethod { get; set; }
		public void SetTemplateContent(Action<ImageSliderItemTemplateContainer> contentMethod) {
			TemplateContentMethod = contentMethod;
		}
		public void SetTemplateContent(string content) {
			TemplateContent = content;
		}
		public void SetTextTemplateContent(Action<ImageSliderItemTemplateContainer> contentMethod) {
			TextTemplateContentMethod = contentMethod;
		}
		public void SetTextTemplateContent(string content) {
			TextTemplateContent = content;
		}
		public void SetThumbnailTemplateContent(Action<ImageSliderItemTemplateContainer> contentMethod) {
			ThumbnailTemplateContentMethod = contentMethod;
		}
		public void SetThumbnailTemplateContent(string content) {
			ThumbnailTemplateContent = content;
		}
	}
	public class MVCxImageSliderItemCollection : ImageSliderItemCollection {
		public MVCxImageSliderItemCollection()
			: base() {
		}
		public new MVCxImageSliderItem this[int index] {
			get { return (GetItem(index) as MVCxImageSliderItem); }
		}
		public void Add(Action<MVCxImageSliderItem> method) {
			method(Add());
		}
		public void Add(MVCxImageSliderItem item) {
			base.Add(item);
		}
		public new MVCxImageSliderItem Add() {
			MVCxImageSliderItem item = new MVCxImageSliderItem();
			Add(item);
			return item;
		}
		public new MVCxImageSliderItem Add(string imageUrl) {
			return Add(imageUrl, "", "", "", "");
		}
		public new MVCxImageSliderItem Add(string imageUrl, string thumbnailUrl) {
			return Add(imageUrl, thumbnailUrl, "", "", "");
		}
		public new MVCxImageSliderItem Add(string imageUrl, string thumbnailUrl, string navigateUrl) {
			return Add(imageUrl, thumbnailUrl, navigateUrl, "", "");
		}
		public new MVCxImageSliderItem Add(string imageUrl, string thumbnailUrl, string navigateUrl, string text) {
			return Add(imageUrl, thumbnailUrl, navigateUrl, text, "");
		}
		public new MVCxImageSliderItem Add(string imageUrl, string thumbnailUrl, string navigateUrl, string text, string name) {
			MVCxImageSliderItem item = new MVCxImageSliderItem(imageUrl, thumbnailUrl, navigateUrl, text, name);
			Add(item);
			return item;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxImageSliderItem);
		}
	}
}
