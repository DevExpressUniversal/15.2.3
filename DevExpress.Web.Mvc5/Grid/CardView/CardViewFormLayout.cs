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
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.Mvc.UI;
namespace DevExpress.Web.Mvc {
	public class MVCxCardViewFormLayoutProperties : CardViewFormLayoutProperties {
		public MVCxCardViewFormLayoutProperties()
			: base(null) {
		}
		public MVCxCardViewFormLayoutProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override LayoutGroup CreateRootGroup() {
			return new MVCxCardViewLayoutGroup(this);
		}
		public new MVCxCardViewLayoutItemCollection Items {
			get { return (MVCxCardViewLayoutItemCollection)base.Items; }
		}
		public Unit NestedExtensionWidth {
			get { return NestedControlWidth; }
			set { NestedControlWidth = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Unit NestedControlWidth {
			get { return base.NestedControlWidth; }
			set { base.NestedControlWidth = value; }
		}
	}
	public class MVCxCardViewLayoutItemCollection : CardViewLayoutItemCollection {
		public MVCxCardViewLayoutItemCollection()
			: base() {
		}
		public MVCxCardViewLayoutItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public MVCxCardViewLayoutGroup AddGroupItem() {
			return (MVCxCardViewLayoutGroup)Add(new MVCxCardViewLayoutGroup());
		}
		public MVCxCardViewLayoutGroup AddGroupItem(Action<MVCxCardViewLayoutGroup> method) {
			var item = new MVCxCardViewLayoutGroup();
			if(method != null)
				method(item);
			return (MVCxCardViewLayoutGroup)Add(item);
		}
		public MVCxCardViewLayoutGroup AddGroupItem(string caption) {
			return Add<MVCxCardViewLayoutGroup>(caption);
		}
		public MVCxCardViewTabbedLayoutGroup AddTabbedGroupItem() {
			return (MVCxCardViewTabbedLayoutGroup)Add(new MVCxCardViewTabbedLayoutGroup());
		}
		public MVCxCardViewTabbedLayoutGroup AddTabbedGroupItem(Action<MVCxCardViewTabbedLayoutGroup> method) {
			var item = new MVCxCardViewTabbedLayoutGroup();
			if(method != null)
				method(item);
			return (MVCxCardViewTabbedLayoutGroup)Add(item);
		}
		public MVCxCardViewTabbedLayoutGroup AddTabbedGroupItem(string caption) {
			return Add<MVCxCardViewTabbedLayoutGroup>(caption);
		}
		public MVCxCardViewColumnLayoutItem Add() {
			return Add((i) => { });
		}
		public MVCxCardViewColumnLayoutItem Add(string columnName) {
			return Add((i) => { i.ColumnName = columnName; });
		}
		public MVCxCardViewColumnLayoutItem Add(Action<MVCxCardViewColumnLayoutItem> method) {
			var htmlHelpertOwner = Owner as IFormLayoutHtmlHelperOwner;
			var htmlHelper = htmlHelpertOwner != null ? htmlHelpertOwner.HtmlHelper : null;
			var item = new MVCxCardViewColumnLayoutItem(htmlHelper);
			if(method != null)
				method(item);
			return (MVCxCardViewColumnLayoutItem)Add(item);
		}
		public CardViewCommandLayoutItem AddCommandItem() {
			return AddCommandItem((i) => { });
		}
		public CardViewCommandLayoutItem AddCommandItem(Action<CardViewCommandLayoutItem> method) {
			var item = new CardViewCommandLayoutItem();
			if(method != null)
				method(item);
			return (CardViewCommandLayoutItem)Add(item);
		}
		public EditModeCommandLayoutItem AddEditModeCommandItem() {
			return AddEditModeCommandItem((i) => { });
		}
		public EditModeCommandLayoutItem AddEditModeCommandItem(Action<EditModeCommandLayoutItem> method) {
			var item = new EditModeCommandLayoutItem();
			if(method != null)
				method(item);
			return (EditModeCommandLayoutItem)Add(item);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewColumnLayoutItem AddColumnItem(CardViewColumnLayoutItem layoutItem) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new EditModeCommandLayoutItem AddCommandItem(EditModeCommandLayoutItem commandItem) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewCommandLayoutItem AddCommandItem(CardViewCommandLayoutItem commandItem) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewLayoutGroup AddGroup(CardViewLayoutGroup layoutGroup) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewTabbedLayoutGroup AddTabbedGroup(CardViewTabbedLayoutGroup tabbedGroup) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewColumnLayoutItem AddColumnItem(string columnName) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewLayoutGroup AddGroup(string caption) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewTabbedLayoutGroup AddTabbedGroup(string caption) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewColumnLayoutItem AddColumnItem(string columnName, string caption) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewLayoutGroup AddGroup(string caption, string name) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new CardViewTabbedLayoutGroup AddTabbedGroup(string caption, string name) { return null; }
	}
	public class MVCxCardViewColumnLayoutItem : CardViewColumnLayoutItem, IFormLayoutHtmlHelperOwner {
		public MVCxCardViewColumnLayoutItem()
			: this((HtmlHelper)null) {
		}
		internal MVCxCardViewColumnLayoutItem(HtmlHelper htmlHelper)
			: base() {
			HtmlHelper = htmlHelper ?? HtmlHelperExtension.HtmlHelper;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Control GetNestedControl() {
			return base.GetNestedControl();
		}
		protected internal string TemplateContent { get; set; }
		protected internal Action<CardViewDataItemTemplateContainer> TemplateContentMethod { get; set; }
		HtmlHelper HtmlHelper { get; set; }
		public void SetTemplateContent(Action<CardViewDataItemTemplateContainer> contentMethod) {
			TemplateContentMethod = contentMethod;
		}
		public void SetTemplateContent(string content) {
			TemplateContent = content;
		}
		public override void Assign(CollectionItem source) {
			MVCxCardViewColumnLayoutItem columnLayoutItem = source as MVCxCardViewColumnLayoutItem;
			if(columnLayoutItem != null) {
				TemplateContent = columnLayoutItem.TemplateContent;
				TemplateContentMethod = columnLayoutItem.TemplateContentMethod;
				HtmlHelper = columnLayoutItem.HtmlHelper;
			}
			base.Assign(source);
		}
		HtmlHelper IFormLayoutHtmlHelperOwner.HtmlHelper { get { return HtmlHelper; } }
		ModelMetadata IFormLayoutHtmlHelperOwner.Metadata { get; set; }
	}
	public class MVCxCardViewLayoutGroup : CardViewLayoutGroup {
		public MVCxCardViewLayoutGroup()
			: base() {
		}
		public MVCxCardViewLayoutGroup(string caption)
			: base() {
		}
		protected internal MVCxCardViewLayoutGroup(FormLayoutProperties owner) : base(owner) { }
		protected override LayoutItemCollection CreateItems() {
			return new MVCxCardViewLayoutItemCollection(this);
		}
		public new MVCxCardViewLayoutItemCollection Items {
			get { return (MVCxCardViewLayoutItemCollection)base.Items; }
		}
		protected override void TemplatesChanged() {
			base.TemplatesChanged();
			MVCxCardView cardView = Owner != null ? Owner.DataOwner as MVCxCardView : null;
			if(cardView != null)
				cardView.TemplatesChanged();
		}
	}
	public class MVCxCardViewTabbedLayoutGroup : CardViewTabbedLayoutGroup {
		public MVCxCardViewTabbedLayoutGroup()
			: base() {
		}
		public MVCxCardViewTabbedLayoutGroup(string caption)
			: base() {
		}
		protected override LayoutItemCollection CreateItems() {
			return new MVCxCardViewLayoutItemCollection(this);
		}
		public new MVCxCardViewLayoutItemCollection Items {
			get { return (MVCxCardViewLayoutItemCollection)base.Items; }
		}
	}
}
