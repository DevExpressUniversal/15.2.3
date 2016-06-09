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
using System.Collections;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public class NewsItem: DataViewItem {
		private ImageProperties fImage = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsItemHeaderText"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true)]
		public string HeaderText {
			get { return GetStringProperty("HeaderText", ""); }
			set { SetStringProperty("HeaderText", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsItemDate"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(DateTime), "1/1/0001")]
		public DateTime Date {
			get { return (DateTime)GetObjectProperty("Date", DateTime.MinValue); }
			set { SetObjectProperty("Date", DateTime.MinValue, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsItemText"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", ""); }
			set { SetStringProperty("Text", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsItemName"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsItemNavigateUrl"),
#endif
		NotifyParentProperty(true), DefaultValue(""), Localizable(false)]
		public string NavigateUrl {
			get { return GetStringProperty("NavigateUrl", ""); }
			set { SetStringProperty("NavigateUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("NewsItemImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public ImageProperties Image {
			get {
				if(fImage == null)
					fImage = new ImageProperties(this);
				return fImage;
			}
		}
		public NewsItem()
			: base() {
		}
		public NewsItem(string headerText)
			: this(headerText, "", "", "", "" ,DateTime.MinValue) {
		}
		public NewsItem(string headerText, string text)
			: this(headerText, text, "", "", "", DateTime.MinValue) {
		}
		public NewsItem(string headerText, string text, string navigateUrl)
			: this(headerText, text, navigateUrl, "", "", DateTime.MinValue) {
		}
		public NewsItem(string headerText, string text, string navigateUrl, string name)
			: this(headerText, text, navigateUrl, name, "", DateTime.MinValue) {
		}
		public NewsItem(string headerText, string text, string navigateUrl, string name, string imageUrl)
			: this(headerText, text, navigateUrl, name, imageUrl, DateTime.MinValue) {
		}
		public NewsItem(string headerText, string text, string navigateUrl, string name, string imageUrl, DateTime date)
			: base() {
			HeaderText = headerText;
			Text = text;
			NavigateUrl = navigateUrl;
			Name = name;
			Image.Url = imageUrl;
			Date = date;
		}
		public override void Assign(CollectionItem source) {
			if (source is NewsItem) {
				NewsItem src = source as NewsItem;
				HeaderText = src.HeaderText;
				Text = src.Text;
				NavigateUrl = src.NavigateUrl;
				Image.Assign(src.Image);
				Date = src.Date;
			}
			base.Assign(source);
		}
		public override string ToString() {
			return (HeaderText != "") ? HeaderText : GetType().Name;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { Image };
		}
	}
	public class NewsItemCollection : DataViewItemCollection {
#if !SL
	[DevExpressWebLocalizedDescription("NewsItemCollectionItem")]
#endif
		public new NewsItem this[int index] {
			get { return (GetItem(index) as NewsItem); }
		}
		protected internal ASPxNewsControl NewsControl {
			get { return Owner as ASPxNewsControl; }
		}
		public NewsItemCollection()
			: base() {
		}
		public NewsItemCollection(ASPxNewsControl newsControl)
			: base(newsControl) {
		}
		public void Add(NewsItem item) {
			base.Add(item);
		}
		public new NewsItem Add() {
			NewsItem item = new NewsItem();
			Add(item);
			return item;
		}
		public NewsItem Add(string headerText) {
			return Add(headerText, "", "", "", "", DateTime.MinValue);
		}
		public NewsItem Add(string headerText, string text) {
			return Add(headerText, text, "", "", "", DateTime.MinValue);
		}
		public NewsItem Add(string headerText, string text, string navigateUrl) {
			return Add(headerText, text, navigateUrl, "", "", DateTime.MinValue);
		}
		public NewsItem Add(string headerText, string text, string navigateUrl, string name) {
			return Add(headerText, text, navigateUrl, name, "", DateTime.MinValue);
		}
		public NewsItem Add(string headerText, string text, string navigateUrl, string name, string imageUrl) {
			return Add(headerText, text, navigateUrl, name, imageUrl, DateTime.MinValue);
		}
		public NewsItem Add(string headerText, string text, string navigateUrl, string name, string imageUrl, DateTime date) {
			NewsItem item = new NewsItem(headerText, text, navigateUrl, name, imageUrl, date);
			Add(item);
			return item;
		}
		public NewsItem FindByDate(DateTime date) {
			int index = IndexOfDate(date);
			return index != -1 ? this[index] : null;
		}
		public NewsItem FindByHeaderText(string headerText) {
			int index = IndexOfHeaderText(headerText);
			return index != -1 ? this[index] : null;
		}
		public NewsItem FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		public NewsItem FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		public int IndexOf(NewsItem item) {
			return base.IndexOf(item);
		}
		public int IndexOfDate(DateTime date) {
			for (int i = 0; i < Count; i++)
				if ((GetItem(i) as NewsItem).Date == date)
					return i;
			return -1;
		}
		public int IndexOfHeaderText(string headerText) {
			for (int i = 0; i < Count; i++)
				if ((GetItem(i) as NewsItem).HeaderText == headerText)
					return i;
			return -1;
		}
		public int IndexOfName(string name) {
			for(int i = 0; i < Count; i++)
				if(this[i].Name.Equals(name))
					return i;
			return -1;
		}
		public int IndexOfText(string text) {
			for (int i = 0; i < Count; i++)
				if ((GetItem(i) as NewsItem).Text == text)
					return i;
			return -1;
		}
		public void Insert(int index, NewsItem item) {
			base.Insert(index, item);
		}
		public void Remove(NewsItem item) {
			base.Remove(item);
		}
		protected override Type GetKnownType() {
			return typeof(NewsItem);
		}
	}
}
