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
using DevExpress.Web;
namespace DevExpress.Web.Mvc {
	public class MVCxTab: Tab {
		public MVCxTab()
			: base() {
		}
		public MVCxTab(string text)
			: base(text) {
		}
		public MVCxTab(string text, string name)
			: base(text, name) {
		}
		public MVCxTab(string text, string name, string imageUrl)
			: base(text, name, imageUrl) {
		}
		public MVCxTab(string text, string name, string imageUrl, string navigateUrl)
			: base(text, name, imageUrl, navigateUrl) {
		}
		public MVCxTab(string text, string name, string imageUrl, string navigateUrl, string target)
			: base(text, name, imageUrl, navigateUrl, target) {
		}
		protected internal string ActiveTabTemplateContent { get; set; }
		protected internal Action<TabControlTemplateContainer> ActiveTabTemplateContentMethod { get; set; }
		protected internal string TabTemplateContent { get; set; }
		protected internal Action<TabControlTemplateContainer> TabTemplateContentMethod { get; set; }
		protected internal string ActiveTabTextTemplateContent { get; set; }
		protected internal Action<TabControlTemplateContainer> ActiveTabTextTemplateContentMethod { get; set; }
		protected internal string TabTextTemplateContent { get; set; }
		protected internal Action<TabControlTemplateContainer> TabTextTemplateContentMethod { get; set; }
		public void SetActiveTabTemplateContent(Action<TabControlTemplateContainer> contentMethod) {
			ActiveTabTemplateContentMethod = contentMethod;
		}
		public void SetActiveTabTemplateContent(string content) {
			ActiveTabTemplateContent = content;
		}
		public void SetTabTemplateContent(Action<TabControlTemplateContainer> contentMethod) {
			TabTemplateContentMethod = contentMethod;
		}
		public void SetTabTemplateContent(string content) {
			TabTemplateContent = content;
		}
		public void SetActiveTabTextTemplateContent(Action<TabControlTemplateContainer> contentMethod) {
			ActiveTabTextTemplateContentMethod = contentMethod;
		}
		public void SetActiveTabTextTemplateContent(string content) {
			ActiveTabTextTemplateContent = content;
		}
		public void SetTabTextTemplateContent(Action<TabControlTemplateContainer> contentMethod) {
			TabTextTemplateContentMethod = contentMethod;
		}
		public void SetTabTextTemplateContent(string content) {
			TabTextTemplateContent = content;
		}
	}
	public class MVCxTabCollection: TabCollection {
		public new MVCxTab this[int index] {
			get { return (GetItem(index) as MVCxTab); }
		}
		public MVCxTabCollection()
			: base() {
		}
		public void Add(Action<MVCxTab> method) {
			method(Add());
		}
		public void Add(MVCxTab tab) {
			base.Add(tab);
		}
		public new MVCxTab Add() {
			MVCxTab tab = new MVCxTab();
			Add(tab);
			return tab;
		}
		public new MVCxTab Add(string text) {
			return Add(text, "", "");
		}
		public new MVCxTab Add(string text, string name) {
			return Add(text, name, "");
		}
		public new MVCxTab Add(string text, string name, string imageUrl) {
			return Add(text, name, imageUrl, "", "");
		}
		public new MVCxTab Add(string text, string name, string imageUrl, string navigateUrl) {
			return Add(text, name, imageUrl, navigateUrl, "");
		}
		public new MVCxTab Add(string text, string name, string imageUrl, string navigateUrl, string target) {
			MVCxTab tab = new MVCxTab(text, name, imageUrl, navigateUrl, target);
			Add(tab);
			return tab;
		}
		public new MVCxTab GetVisibleTab(int index) {
			return GetVisibleItem(index) as MVCxTab;
		}
		public int IndexOf(MVCxTab tab) {
			return base.IndexOf(tab);
		}
		public void Insert(int index, MVCxTab tab) {
			base.Insert(index, tab);
		}
		public void Remove(MVCxTab tab) {
			base.Remove(tab);
		}
		public new MVCxTab FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		public new MVCxTab FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxTab);
		}
	}
	public class MVCxTabPage: TabPage {
		public MVCxTabPage()
			: base() {
		}
		public MVCxTabPage(string text)
			: base(text) {
		}
		public MVCxTabPage(string text, string name)
			: base(text, name) {
		}
		public MVCxTabPage(string text, string name, string imageUrl)
			: base(text, name, imageUrl) {
		}
		[EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Web.UI.ControlCollection Controls {
			get {
				return base.Controls;
			}
		}
		protected internal string Content { get; set; }
		protected internal Action ContentMethod { get; set; }
		protected internal string ActiveTabTemplateContent { get; set; }
		protected internal Action<PageControlTemplateContainer> ActiveTabTemplateContentMethod { get; set; }
		protected internal string TabTemplateContent { get; set; }
		protected internal Action<PageControlTemplateContainer> TabTemplateContentMethod { get; set; }
		protected internal string ActiveTabTextTemplateContent { get; set; }
		protected internal Action<PageControlTemplateContainer> ActiveTabTextTemplateContentMethod { get; set; }
		protected internal string TabTextTemplateContent { get; set; }
		protected internal Action<PageControlTemplateContainer> TabTextTemplateContentMethod { get; set; }
		public void SetContent(Action contentMethod) {
			ContentMethod = contentMethod;
		}
		public void SetContent(string content) {
			Content = content;
		}
		public void SetActiveTabTemplateContent(Action<PageControlTemplateContainer> contentMethod) {
			ActiveTabTemplateContentMethod = contentMethod;
		}
		public void SetActiveTabTemplateContent(string content) {
			ActiveTabTemplateContent = content;
		}
		public void SetTabTemplateContent(Action<PageControlTemplateContainer> contentMethod) {
			TabTemplateContentMethod = contentMethod;
		}
		public void SetTabTemplateContent(string content) {
			TabTemplateContent = content;
		}
		public void SetActiveTabTextTemplateContent(Action<PageControlTemplateContainer> contentMethod) {
			ActiveTabTextTemplateContentMethod = contentMethod;
		}
		public void SetActiveTabTextTemplateContent(string content) {
			ActiveTabTextTemplateContent = content;
		}
		public void SetTabTextTemplateContent(Action<PageControlTemplateContainer> contentMethod) {
			TabTextTemplateContentMethod = contentMethod;
		}
		public void SetTabTextTemplateContent(string content) {
			TabTextTemplateContent = content;
		}
	}
	public class MVCxTabPageCollection: TabPageCollection {
		public new MVCxTabPage this[int index] {
			get { return (GetItem(index) as MVCxTabPage); }
		}
		public MVCxTabPageCollection()
			: base() {
		}
		public void Add(Action<MVCxTabPage> method) {
			method(Add());
		}
		public void Add(MVCxTabPage tabPage) {
			base.Add(tabPage);
		}
		public new MVCxTabPage Add() {
			MVCxTabPage tabPage = new MVCxTabPage();
			Add(tabPage);
			return tabPage;
		}
		public new MVCxTabPage Add(string text) {
			return Add(text, "", "");
		}
		public new MVCxTabPage Add(string text, string name) {
			return Add(text, name, "");
		}
		public new MVCxTabPage Add(string text, string name, string imageUrl) {
			MVCxTabPage tabPage = new MVCxTabPage(text, name, imageUrl);
			Add(tabPage);
			return tabPage;
		}
		public new MVCxTabPage GetVisibleTabPage(int index) {
			return GetVisibleItem(index) as MVCxTabPage;
		}
		public int IndexOf(MVCxTabPage tabPage) {
			return base.IndexOf(tabPage);
		}
		public void Insert(int index, MVCxTabPage tabPage) {
			base.Insert(index, tabPage);
		}
		public void Remove(MVCxTabPage tabPage) {
			base.Remove(tabPage);
		}
		public new MVCxTabPage FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		public new MVCxTabPage FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(MVCxTabPage);
		}
	}
}
