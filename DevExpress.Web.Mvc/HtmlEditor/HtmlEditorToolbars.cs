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
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxHtmlEditor;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	public class MVCxHtmlEditorToolbar : HtmlEditorToolbar {
		public MVCxHtmlEditorToolbar() : base() { }
		public MVCxHtmlEditorToolbar(string name) : base(name) { }
		public MVCxHtmlEditorToolbar(string name, params HtmlEditorToolbarItem[] items) : base(name, items) { }
		public new MVCxHtmlEditorToolbarItemCollection Items {
			get { return (MVCxHtmlEditorToolbarItemCollection)base.Items; }
		}
		protected override HtmlEditorToolbarItemCollection CreateToolbarItemCollection() {
			return new MVCxHtmlEditorToolbarItemCollection(this);
		}
		public static new MVCxHtmlEditorToolbar CreateStandardToolbar1() {
			MVCxHtmlEditorToolbar toolbar = new MVCxHtmlEditorToolbar("StandardToolbar1", StandardToolbartemsHelper.GetToolbar1Items());
			toolbar.CreateDefaultItems();
			return toolbar;
		}
		public static new MVCxHtmlEditorToolbar CreateStandardToolbar2() {
			return CreateStandardToolbar2(false);
		}
		public static new MVCxHtmlEditorToolbar CreateStandardToolbar2(bool rtl) {
			MVCxHtmlEditorToolbar toolbar = new MVCxHtmlEditorToolbar("StandardToolbar2", StandardToolbartemsHelper.GetToolbar2Items(rtl));
			toolbar.CreateDefaultItems();
			return toolbar;
		}
		public static new MVCxHtmlEditorToolbar CreateTableToolbar() {
			MVCxHtmlEditorToolbar toolbar = new MVCxHtmlEditorToolbar("TableToolbar", StandardToolbartemsHelper.GetTableToolbarItems());
			toolbar.Items.ForEach(delegate(HtmlEditorToolbarItem item) { item.Text = ""; });
			return toolbar;
		}
	}
	public class MVCxHtmlEditorToolbarCollection : HtmlEditorToolbarCollection {
		public new MVCxHtmlEditorToolbar this[int index] {
			get { return (MVCxHtmlEditorToolbar)(this as IList)[index]; }
			set { (this as IList)[index] = value; }
		}
		public new MVCxHtmlEditorToolbar this[string name] {
			get {
				return (MVCxHtmlEditorToolbar)Find(delegate(HtmlEditorToolbar toolbar) {
					return toolbar.Name == name;
				});
			}
		}
		public void Add(Action<MVCxHtmlEditorToolbar> method) {
			method(Add());
		}
		public new MVCxHtmlEditorToolbar Add() {
			return (MVCxHtmlEditorToolbar)AddInternal(new MVCxHtmlEditorToolbar());
		}
		public new MVCxHtmlEditorToolbar Add(string name) {
			return (MVCxHtmlEditorToolbar)AddInternal(new MVCxHtmlEditorToolbar(name));
		}
		public new MVCxHtmlEditorToolbar Add(string name, params HtmlEditorToolbarItem[] items) {
			return (MVCxHtmlEditorToolbar)AddInternal(new MVCxHtmlEditorToolbar(name, items));
		}
		public override void CreateDefaultToolbars(bool rtl) {
			Add(MVCxHtmlEditorToolbar.CreateStandardToolbar1());
			Add(MVCxHtmlEditorToolbar.CreateStandardToolbar2(rtl));
		}
		protected internal bool CollectionHasBeenChanged { get; private set; }
		protected override void OnChanged() {
			base.OnChanged();
			CollectionHasBeenChanged = true;
		}
	}
	public class MVCxHtmlEditorToolbarItemCollection : HtmlEditorToolbarItemCollection {
		public MVCxHtmlEditorToolbarItemCollection()
			: base() {
		}
		public MVCxHtmlEditorToolbarItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public void Add<T>(Action<T> method) where T: HtmlEditorToolbarItem, new() {
			T item = new T();
			AddInternal(item);
			method(item);
		}
	}
}
