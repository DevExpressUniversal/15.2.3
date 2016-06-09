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
	public class MVCxNavBarGroup: NavBarGroup {
		public MVCxNavBarGroup()
			: base() {
		}
		public MVCxNavBarGroup(string text)
			: base(text) {
		}
		public MVCxNavBarGroup(string text, string name)
			: base(text, name) {
		}
		public MVCxNavBarGroup(string text, string name, string imageUrl)
			: base(text, name, imageUrl) {
		}
		public MVCxNavBarGroup(string text, string name, string imageUrl, string navigateUrl)
			: base(text, name, imageUrl, navigateUrl) {
		}
		public MVCxNavBarGroup(string text, string name, string imageUrl, string navigateUrl, string target)
			: base(text, name, imageUrl, navigateUrl, target) {
		}
		public new MVCxNavBarItemCollection Items {
			get { return (MVCxNavBarItemCollection)base.Items; }
		}
		protected internal string ContentTemplateContent { get; set; }
		protected internal Action<NavBarGroupTemplateContainer> ContentTemplateContentMethod { get; set; }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<NavBarGroupTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string HeaderTemplateCollapsedContent { get; set; }
		protected internal Action<NavBarGroupTemplateContainer> HeaderTemplateCollapsedContentMethod { get; set; }
		protected internal string ItemTemplateContent { get; set; }
		protected internal Action<NavBarItemTemplateContainer> ItemTemplateContentMethod { get; set; }
		protected internal string ItemTextTemplateContent { get; set; }
		protected internal Action<NavBarItemTemplateContainer> ItemTextTemplateContentMethod { get; set; }
		public void SetContentTemplateContent(Action<NavBarGroupTemplateContainer> contentMethod) {
			ContentTemplateContentMethod = contentMethod;
		}
		public void SetContentTemplateContent(string content) {
			ContentTemplateContent = content;
		}
		public void SetHeaderTemplateContent(Action<NavBarGroupTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		public void SetHeaderTemplateCollapsedContent(Action<NavBarGroupTemplateContainer> contentMethod) {
			HeaderTemplateCollapsedContentMethod = contentMethod;
		}
		public void SetHeaderTemplateCollapsedContent(string content) {
			HeaderTemplateCollapsedContent = content;
		}
		public void SetItemTemplateContent(Action<NavBarItemTemplateContainer> contentMethod) {
			ItemTemplateContentMethod = contentMethod;
		}
		public void SetItemTemplateContent(string content) {
			ItemTemplateContent = content;
		}
		public void SetItemTextTemplateContent(Action<NavBarItemTemplateContainer> contentMethod) {
			ItemTextTemplateContentMethod = contentMethod;
		}
		public void SetItemTextTemplateContent(string content) {
			ItemTextTemplateContent = content;
		}
		protected override NavBarItemCollection CreateItemsCollection() {
			return new MVCxNavBarItemCollection(this);
		}
	}
	public class MVCxNavBarGroupCollection: NavBarGroupCollection {
		public new MVCxNavBarGroup this[int index] {
			get { return (GetItem(index) as MVCxNavBarGroup); }
		}
		public MVCxNavBarGroupCollection()
			: base() {
		}
		public void Add(Action<MVCxNavBarGroup> method) {
			method(Add());
		}
		public void Add(MVCxNavBarGroup group) {
			base.Add(group);
		}
		public new MVCxNavBarGroup Add() {
			MVCxNavBarGroup group = new MVCxNavBarGroup();
			Add(group);
			return group;
		}
		public new MVCxNavBarGroup Add(string text) {
			return Add(text, "", "", "", "");
		}
		public new MVCxNavBarGroup Add(string text, string name) {
			return Add(text, name, "", "", "");
		}
		public new MVCxNavBarGroup Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public new MVCxNavBarGroup Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public new MVCxNavBarGroup Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			MVCxNavBarGroup group = new MVCxNavBarGroup(text, name, imageUrl, navigateUrl, target);
			Add(group);
			return group;
		}
		public new MVCxNavBarGroup GetVisibleItem(int index) {
			return GetVisibleItem(index) as MVCxNavBarGroup;
		}
		public int IndexOf(MVCxNavBarGroup group) {
			return base.IndexOf(group);
		}
		public void Insert(int index, MVCxNavBarGroup group) {
			base.Insert(index, group);
		}
		public void Remove(MVCxNavBarGroup item) {
			base.Remove(item);
		}
		public new MVCxNavBarGroup FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		public new MVCxNavBarGroup FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxNavBarGroup);
		}
	}
}
