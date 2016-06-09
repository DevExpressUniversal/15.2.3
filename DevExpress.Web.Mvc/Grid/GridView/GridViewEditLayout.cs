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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.Mvc.UI;
namespace DevExpress.Web.Mvc {
	public class MVCxGridViewFormLayoutProperties : GridViewFormLayoutProperties {
		public MVCxGridViewFormLayoutProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override LayoutGroup CreateRootGroup() {
			return new MVCxGridViewLayoutGroup(this);
		}
		public new MVCxGridViewLayoutItemCollection Items {
			get { return (MVCxGridViewLayoutItemCollection)base.Items; }
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
	public class MVCxGridViewFormLayoutProperties<ModelType> : MVCxGridViewFormLayoutProperties {
		public MVCxGridViewFormLayoutProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override LayoutGroup CreateRootGroup() {
			return new MVCxGridViewLayoutGroup<ModelType>(this);
		}
		public new MVCxGridViewLayoutItemCollection<ModelType> Items {
			get { return (MVCxGridViewLayoutItemCollection<ModelType>)base.Items; }
		}
	}
	public class MVCxGridViewLayoutItemCollection : GridViewLayoutItemCollection {
		public MVCxGridViewLayoutItemCollection()
			: base() {
		}
		public MVCxGridViewLayoutItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public MVCxGridViewLayoutGroup AddGroupItem() {
			return (MVCxGridViewLayoutGroup)Add(new MVCxGridViewLayoutGroup());
		}
		public MVCxGridViewLayoutGroup AddGroupItem(Action<MVCxGridViewLayoutGroup> method) {
			var item = new MVCxGridViewLayoutGroup();
			if(method != null)
				method(item);
			return (MVCxGridViewLayoutGroup)Add(item);
		}
		public MVCxGridViewLayoutGroup AddGroupItem(string caption) {
			return Add<MVCxGridViewLayoutGroup>(caption);
		}
		public MVCxGridViewTabbedLayoutGroup AddTabbedGroupItem() {
			return (MVCxGridViewTabbedLayoutGroup)Add(new MVCxGridViewTabbedLayoutGroup());
		}
		public MVCxGridViewTabbedLayoutGroup AddTabbedGroupItem(Action<MVCxGridViewTabbedLayoutGroup> method) {
			var item = new MVCxGridViewTabbedLayoutGroup();
			if(method != null)
				method(item);
			return (MVCxGridViewTabbedLayoutGroup)Add(item);
		}
		public MVCxGridViewTabbedLayoutGroup AddTabbedGroupItem(string caption) {
			return Add<MVCxGridViewTabbedLayoutGroup>(caption);
		}
		public MVCxGridViewColumnLayoutItem Add() {
			return Add((i) => { });
		}
		public MVCxGridViewColumnLayoutItem Add(string columnName) {
			return Add((i) => { i.ColumnName = columnName; });
		}
		public MVCxGridViewColumnLayoutItem Add(Action<MVCxGridViewColumnLayoutItem> method) {
			var htmlHelpertOwner = Owner as IFormLayoutHtmlHelperOwner;
			var htmlHelper = htmlHelpertOwner != null ? htmlHelpertOwner.HtmlHelper : null;
			var item = new MVCxGridViewColumnLayoutItem(htmlHelper);
			if(method != null)
				method(item);
			return (MVCxGridViewColumnLayoutItem)Add(item);
		}
		public EditModeCommandLayoutItem AddCommandItem() {
			return AddCommandItem((i) => { });
		}
		public EditModeCommandLayoutItem AddCommandItem(Action<EditModeCommandLayoutItem> method) {
			var item = new EditModeCommandLayoutItem();
			if(method != null)
				method(item);
			return (EditModeCommandLayoutItem)Add(item);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewColumnLayoutItem AddColumnItem(GridViewColumnLayoutItem layoutItem) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new EditModeCommandLayoutItem AddCommandItem(EditModeCommandLayoutItem commandItem) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewLayoutGroup AddGroup(GridViewLayoutGroup layoutGroup) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewTabbedLayoutGroup AddTabbedGroup(GridViewTabbedLayoutGroup tabbedGroup) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewColumnLayoutItem AddColumnItem(string columnName) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewLayoutGroup AddGroup(string caption) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewTabbedLayoutGroup AddTabbedGroup(string caption) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewColumnLayoutItem AddColumnItem(string columnName, string caption) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewLayoutGroup AddGroup(string caption, string name) { return null; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new GridViewTabbedLayoutGroup AddTabbedGroup(string caption, string name) { return null; }
	}
	public class MVCxGridViewLayoutItemCollection<ModelType> : MVCxGridViewLayoutItemCollection {
		public MVCxGridViewLayoutItemCollection()
			: this(null) {
		}
		public MVCxGridViewLayoutItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public MVCxGridViewColumnLayoutItem Add<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			var item = Add<MVCxGridViewColumnLayoutItem, ValueType>(expression);
			item.ColumnName = ExtensionsHelper.GetFullHtmlFieldName(expression);
			return item;
		}
		public MVCxGridViewColumnLayoutItem Add<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<MVCxGridViewColumnLayoutItem> method) {
			var item = Add<ValueType>(expression);
			if(method != null)
				method(item);
			return item;
		}		
		public new MVCxGridViewLayoutGroup<ModelType> AddGroupItem() {
			return (MVCxGridViewLayoutGroup<ModelType>)Add(new MVCxGridViewLayoutGroup<ModelType>());
		}
		public MVCxGridViewLayoutGroup<ModelType> AddGroupItem(Action<MVCxGridViewLayoutGroup<ModelType>> method) {
			var item = new MVCxGridViewLayoutGroup<ModelType>();
			if(method != null)
				method(item);
			return (MVCxGridViewLayoutGroup<ModelType>)Add(item);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new MVCxGridViewLayoutGroup AddGroupItem(Action<MVCxGridViewLayoutGroup> method) {
			return base.AddGroupItem(method);
		}
		public new MVCxGridViewLayoutGroup<ModelType> AddGroupItem(string caption) {
			return Add<MVCxGridViewLayoutGroup<ModelType>>(caption);
		}
		public MVCxGridViewLayoutGroup<ModelType> AddGroupItem<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return AddGroupItem(expression, null);
		}
		public MVCxGridViewLayoutGroup<ModelType> AddGroupItem<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<MVCxGridViewLayoutGroup<ModelType>> method) {
			var item = Add<MVCxGridViewLayoutGroup<ModelType>, ValueType>(expression);
			if(method != null)
				method(item);
			return item;
		}
		public new MVCxGridViewTabbedLayoutGroup<ModelType> AddTabbedGroupItem() {
			return (MVCxGridViewTabbedLayoutGroup<ModelType>)Add(new MVCxGridViewTabbedLayoutGroup<ModelType>());
		}
		public MVCxGridViewTabbedLayoutGroup<ModelType> AddTabbedGroupItem(Action<MVCxGridViewTabbedLayoutGroup<ModelType>> method) {
			var item = new MVCxGridViewTabbedLayoutGroup<ModelType>();
			if(method != null)
				method(item);
			return (MVCxGridViewTabbedLayoutGroup<ModelType>)Add(item);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new MVCxGridViewTabbedLayoutGroup AddTabbedGroupItem(Action<MVCxGridViewTabbedLayoutGroup> method) {
			return base.AddTabbedGroupItem(method);
		}
		public new MVCxGridViewTabbedLayoutGroup<ModelType> AddTabbedGroupItem(string caption) {
			return Add<MVCxGridViewTabbedLayoutGroup<ModelType>>(caption);
		}
		public MVCxGridViewTabbedLayoutGroup<ModelType> AddTabbedGroupItem<ValueType>(Expression<Func<ModelType, ValueType>> expression) {
			return AddTabbedGroupItem(expression, null);
		}
		public MVCxGridViewTabbedLayoutGroup<ModelType> AddTabbedGroupItem<ValueType>(Expression<Func<ModelType, ValueType>> expression, Action<MVCxGridViewTabbedLayoutGroup<ModelType>> method) {
			var item = Add<MVCxGridViewTabbedLayoutGroup<ModelType>, ValueType>(expression);
			if(method != null)
				method(item);
			return item;
		}
		T Add<T, ValueType>(Expression<Func<ModelType, ValueType>> expression) where T : LayoutItemBase, new() {
			var item = new T();
			if(expression != null) {
				ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<ModelType>());
				FormLayoutItemHelper.ConfigureLayoutItemByMetadata(item, metadata);
			}
			Add(item);
			return item;
		}
	}
	public class MVCxGridViewColumnLayoutItem : GridViewColumnLayoutItem, IFormLayoutHtmlHelperOwner {
		public MVCxGridViewColumnLayoutItem()
			: this((HtmlHelper)null) {
		}
		internal MVCxGridViewColumnLayoutItem(HtmlHelper htmlHelper)
			: base() {
			HtmlHelper = htmlHelper ?? HtmlHelperExtension.HtmlHelper;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override Control GetNestedControl() {
			return base.GetNestedControl();
		}
		protected internal string TemplateContent { get; set; }
		protected internal Action<GridViewEditFormLayoutItemTemplateContainer> TemplateContentMethod { get; set; }
		HtmlHelper HtmlHelper { get; set; }
		public void SetTemplateContent(Action<GridViewEditFormLayoutItemTemplateContainer> contentMethod) {
			TemplateContentMethod = contentMethod;
		}
		public void SetTemplateContent(string content) {
			TemplateContent = content;
		}
		public override void Assign(CollectionItem source) {
			MVCxGridViewColumnLayoutItem columnLayoutItem = source as MVCxGridViewColumnLayoutItem;
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
	public class MVCxGridViewLayoutGroup : GridViewLayoutGroup {
		public MVCxGridViewLayoutGroup()
			: base() {
		}
		public MVCxGridViewLayoutGroup(string caption)
			: base() {
		}
		protected internal MVCxGridViewLayoutGroup(FormLayoutProperties owner) : base(owner) { }
		protected override LayoutItemCollection CreateItems() {
			return new MVCxGridViewLayoutItemCollection(this);
		}
		public new MVCxGridViewLayoutItemCollection Items {
			get { return (MVCxGridViewLayoutItemCollection)base.Items; }
		}
	}
	public class MVCxGridViewLayoutGroup<ModelType> : MVCxGridViewLayoutGroup {
		public MVCxGridViewLayoutGroup()
			: base() {
		}
		public MVCxGridViewLayoutGroup(string caption)
			: base() {
		}
		protected internal MVCxGridViewLayoutGroup(FormLayoutProperties owner)
			: base(owner) {
		}
		public new MVCxGridViewLayoutItemCollection<ModelType> Items {
			get { return (MVCxGridViewLayoutItemCollection<ModelType>)base.Items; }
		}
		protected override LayoutItemCollection CreateItems() {
			return new MVCxGridViewLayoutItemCollection<ModelType>(this);
		}
	}
	public class MVCxGridViewTabbedLayoutGroup : GridViewTabbedLayoutGroup {
		public MVCxGridViewTabbedLayoutGroup()
			: base() {
		}
		public MVCxGridViewTabbedLayoutGroup(string caption)
			: base() {
		}
		protected override LayoutItemCollection CreateItems() {
			return new MVCxGridViewLayoutItemCollection(this);
		}
		public new MVCxGridViewLayoutItemCollection Items {
			get { return (MVCxGridViewLayoutItemCollection)base.Items; }
		}
	}
	public class MVCxGridViewTabbedLayoutGroup<ModelType> : MVCxGridViewTabbedLayoutGroup {
		public MVCxGridViewTabbedLayoutGroup()
			: base() {
		}
		public MVCxGridViewTabbedLayoutGroup(string caption)
			: base() {
		}
		public new MVCxGridViewLayoutItemCollection<ModelType> Items {
			get { return (MVCxGridViewLayoutItemCollection<ModelType>)base.Items; }
		}
		protected override LayoutItemCollection CreateItems() {
			return new MVCxGridViewLayoutItemCollection<ModelType>(this);
		}
	}		
}
