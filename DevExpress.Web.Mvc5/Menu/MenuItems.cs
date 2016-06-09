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
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	public class MVCxMenuItem: MenuItem {
		public MVCxMenuItem()
			: base() {
		}
		public MVCxMenuItem(string text)
			: base(text) {
		}
		public MVCxMenuItem(string text, string name)
			: base(text, name) {
		}
		public MVCxMenuItem(string text, string name, string imageUrl)
			: base(text, name, imageUrl) {
		}
		public MVCxMenuItem(string text, string name, string imageUrl, string navigateUrl)
			: base(text, name, imageUrl, navigateUrl) {
		}
		public MVCxMenuItem(string text, string name, string imageUrl, string navigateUrl, string target)
			: base(text, name, imageUrl, navigateUrl, target) {
		}
		public new MVCxMenuItemCollection Items {
			get { return (MVCxMenuItemCollection)base.Items; }
		}
		protected internal string TemplateContent { get; set; }
		protected internal Action<MenuItemTemplateContainer> TemplateContentMethod { get; set; }
		protected internal string TextTemplateContent { get; set; }
		protected internal Action<MenuItemTemplateContainer> TextTemplateContentMethod { get; set; }
		protected internal string SubMenuTemplateContent { get; set; }
		protected internal Action<MenuItemTemplateContainer> SubMenuTemplateContentMethod { get; set; }
		public void SetTemplateContent(Action<MenuItemTemplateContainer> contentMethod) {
			TemplateContentMethod = contentMethod;
		}
		public void SetTemplateContent(string content) {
			TemplateContent = content;
		}
		public void SetTextTemplateContent(Action<MenuItemTemplateContainer> contentMethod) {
			TextTemplateContentMethod = contentMethod;
		}
		public void SetTextTemplateContent(string content) {
			TextTemplateContent = content;
		}
		public void SetSubMenuTemplateContent(Action<MenuItemTemplateContainer> contentMethod) {
			SubMenuTemplateContentMethod = contentMethod;
		}
		public void SetSubMenuTemplateContent(string content) {
			SubMenuTemplateContent = content;
		}
		protected override MenuItemCollection CreateItemsCollection() {
			return new MVCxMenuItemCollection(this);
		}
	}
	public class MVCxMenuItemCollection: MenuItemCollection {
		public new MVCxMenuItem this[int index] {
			get { return (GetItem(index) as MVCxMenuItem); }
		}
		public MVCxMenuItemCollection()
			: base() {
		}
		public MVCxMenuItemCollection(MenuItem item)
			: base(item) {
		}
		public void Add(Action<MVCxMenuItem> method) {
			method(Add());
		}
		public void Add(MVCxMenuItem item) {
			base.Add(item);
		}
		public new MVCxMenuItem Add() {
			MVCxMenuItem item = new MVCxMenuItem();
			Add(item);
			return item;
		}
		public new MVCxMenuItem Add(string text) {
			return Add(text, "", "", "", "");
		}
		public new MVCxMenuItem Add(string text, string name) {
			return Add(text, name, "", "", "");
		}
		public new MVCxMenuItem Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public new MVCxMenuItem Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public new MVCxMenuItem Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			MVCxMenuItem menuItem = new MVCxMenuItem(text, name, imageUrl, navigateUrl, target);
			Add(menuItem);
			return menuItem;
		}
		public new MVCxMenuItem GetVisibleItem(int index) {
			return GetVisibleItem(index) as MVCxMenuItem;
		}
		public int IndexOf(MVCxMenuItem item) {
			return base.IndexOf(item);
		}
		public void Insert(int index, MVCxMenuItem item) {
			base.Insert(index, item);
		}
		public void Remove(MVCxMenuItem item) {
			base.Remove(item);
		}
		public new MVCxMenuItem FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		public new MVCxMenuItem FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxMenuItem);
		}
	}
}
