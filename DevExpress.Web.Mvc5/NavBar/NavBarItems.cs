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
	public class MVCxNavBarItem : NavBarItem {
		public MVCxNavBarItem()
			: base() {
		}
		public MVCxNavBarItem(string text)
			: base(text) {
		}
		public MVCxNavBarItem(string text, string name)
			: base(text, name) {
		}
		public MVCxNavBarItem(string text, string name, string imageUrl)
			: base(text, name, imageUrl) {
		}
		public MVCxNavBarItem(string text, string name, string imageUrl, string navigateUrl)
			: base(text, name, imageUrl, navigateUrl) {
		}
		public MVCxNavBarItem(string text, string name, string imageUrl, string navigateUrl, string target)
			: base(text, name, imageUrl, navigateUrl, target) {
		}
		protected internal string TemplateContent { get; set; }
		protected internal Action<NavBarItemTemplateContainer> TemplateContentMethod { get; set; }
		protected internal string TextTemplateContent { get; set; }
		protected internal Action<NavBarItemTemplateContainer> TextTemplateContentMethod { get; set; }
		public void SetTemplateContent(Action<NavBarItemTemplateContainer> contentMethod) {
			TemplateContentMethod = contentMethod;
		}
		public void SetTemplateContent(string content) {
			TemplateContent = content;
		}
		public void SetTextTemplateContent(Action<NavBarItemTemplateContainer> contentMethod) {
			TextTemplateContentMethod = contentMethod;
		}
		public void SetTextTemplateContent(string content) {
			TextTemplateContent = content;
		}
	}
	public class MVCxNavBarItemCollection : NavBarItemCollection {
		public new MVCxNavBarItem this[int index] {
			get { return (GetItem(index) as MVCxNavBarItem); }
		}
		public MVCxNavBarItemCollection()
			: base() {
		}
		public MVCxNavBarItemCollection(NavBarGroup group)
			: base(group) {
		}
		public void Add(Action<MVCxNavBarItem> method) {
			method(Add());
		}
		public void Add(MVCxNavBarItem item) {
			base.Add(item);
		}
		public new MVCxNavBarItem Add() {
			MVCxNavBarItem item = new MVCxNavBarItem();
			Add(item);
			return item;
		}
		public new MVCxNavBarItem Add(string text) {
			return Add(text, "", "", "", "");
		}
		public new MVCxNavBarItem Add(string text, string name) {
			return Add(text, name, "", "", "");
		}
		public new MVCxNavBarItem Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public new MVCxNavBarItem Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public new MVCxNavBarItem Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			MVCxNavBarItem item = new MVCxNavBarItem(text, name, imageUrl, navigateUrl, target);
			Add(item);
			return item;
		}
		public new MVCxNavBarItem GetVisibleItem(int index) {
			return GetVisibleItem(index) as MVCxNavBarItem;
		}
		public int IndexOf(MVCxNavBarItem item) {
			return base.IndexOf(item);
		}
		public void Insert(int index, MVCxNavBarItem item) {
			base.Insert(index, item);
		}
		public void Remove(MVCxNavBarItem item) {
			base.Remove(item);
		}
		public new MVCxNavBarItem FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		public new MVCxNavBarItem FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxNavBarItem);
		}
	}
}
