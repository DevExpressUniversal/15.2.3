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
	using DevExpress.Web.Internal;
	public class TabPage : TabBase {
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public override ITemplate ActiveTabTemplate {
			get { return base.ActiveTabTemplate; }
			set { base.ActiveTabTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public override ITemplate TabTemplate {
			get { return base.TabTemplate; }
			set { base.TabTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public override ITemplate ActiveTabTextTemplate {
			get { return base.ActiveTabTextTemplate; }
			set { base.ActiveTabTextTemplate = value; }
		}
		[Browsable(false), DefaultValue(null), NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(PageControlTemplateContainer))]
		public override ITemplate TabTextTemplate {
			get { return base.TabTextTemplate; }
			set { base.TabTextTemplate = value; }
		}
		public TabPage()
			: base() {
		}
		public TabPage(string text)
			: this(text, "", "") {
		}
		public TabPage(string text, string name)
			: this(text, name, "") {
		}
		public TabPage(string text, string name, string imageUrl)
			: base(text, name, imageUrl) {
		}
		public override Control FindControl(string id) {
			Control control = base.FindControl(id);
			if (control != null)
				return control;
			return ContentControl.FindControl(id);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override object DataItem {
			get { return base.DataItem; }
			set { base.DataItem = value; }
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class TabPageCollection : TabCollectionBase {
#if !SL
	[DevExpressWebLocalizedDescription("TabPageCollectionItem")]
#endif
		public new TabPage this[int index] {
			get { return (GetItem(index) as TabPage); }
		}
		public TabPageCollection()
			: base() {
		}
		public TabPageCollection(ASPxPageControl pageControl)
			: base(pageControl) {
		}
		public void Add(TabPage tabPage) {
			base.Add(tabPage);
		}
		public TabPage Add() {
			TabPage tabPage = new TabPage();
			Add(tabPage);
			return tabPage;
		}
		public TabPage Add(string text) {
			return Add(text, "", "");
		}
		public TabPage Add(string text, string name) {
			return Add(text, name, "");
		}
		public TabPage Add(string text, string name, string imageUrl) {
			TabPage tabPage = new TabPage(text, name, imageUrl);
			Add(tabPage);
			return tabPage;
		}
		public TabPage GetVisibleTabPage(int index) {
			return GetVisibleItem(index) as TabPage;
		}
		public int GetVisibleTabPageCount() {
			return GetVisibleItemCount();
		}
		public int IndexOf(TabPage tabPage) {
			return base.IndexOf(tabPage);
		}
		public void Insert(int index, TabPage tabPage) {
			base.Insert(index, tabPage);
		}
		public void Remove(TabPage tabPage) {
			base.Remove(tabPage);
		}
		public TabPage FindByName(string name) {
			int index = IndexOfName(name);
			return index != -1 ? this[index] : null;
		}
		public TabPage FindByText(string text) {
			int index = IndexOfText(text);
			return index != -1 ? this[index] : null;
		}
		protected override Type GetKnownType() {
			return typeof(TabPage);
		}
	}
}
