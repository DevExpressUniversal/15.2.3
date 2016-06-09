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
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	[TypeConverter(typeof(ExpandableObjectConverter)), ControlBuilder(typeof(TitleIndexPropertiesBuilder))]
	public class TitleIndexItem : CollectionItem {
		private const string DefaultText = "Item";
		private object fDataItem = null;
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexItemDescription"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string Description {
			get { return GetStringProperty("Description", ""); }
			set { SetStringProperty("Description", "", value); }
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(false), Browsable(false)]
		public string GroupValueString {
			get { return ""; }
			set {
				DxObjectConverter converter = new DxObjectConverter();
				GroupValue = converter.ConvertFrom(value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexItemGroupValue"),
#endif
		DefaultValue(""), NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.UIObjectEditor, " + AssemblyInfo.SRAssemblyWebDesignFull,
		typeof(System.Drawing.Design.UITypeEditor)), TypeConverter(typeof(DxObjectConverter))]
		public object GroupValue {
			get { return GetObjectProperty("GroupValue", ""); }
			set {
				SetObjectProperty("GroupValue", "", value);
				GroupValueChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexItemName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexItemNavigateUrl"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string NavigateUrl {
			get { return GetStringProperty("NavigateUrl", ""); }
			set {
				SetStringProperty("NavigateUrl", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TitleIndexItemText"),
#endif
		DefaultValue(DefaultText), NotifyParentProperty(true), Localizable(true)]
		public string Text {
			get { return GetStringProperty("Text", DefaultText); }
			set { SetStringProperty("Text", DefaultText, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object DataItem {
			get { return fDataItem; }
			set { SetDataItem(value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ASPxTitleIndex TitleIndex {
			get {
				return (Collection != null) &&
					(Collection.Owner is ASPxTitleIndex) ? Collection.Owner as ASPxTitleIndex : null;
			}
		}
		public TitleIndexItem()
			: base() {
		}
		public TitleIndexItem(string text)
			: base() {
			Text = text;
		}
		public TitleIndexItem(string text, string navigateUrl)
			: this(text) {
			NavigateUrl = navigateUrl;
		}
		public TitleIndexItem(string text, string navigateUrl, string name)
			: this(text, navigateUrl) {
			Name = name;
		}
		public override void Assign(CollectionItem source) {
			if (source is TitleIndexItem) {
				TitleIndexItem src = source as TitleIndexItem;
				Description = src.Description;
				GroupValue = src.GroupValue;
				Name = src.Name;
				NavigateUrl = src.NavigateUrl;
				Text = src.Text;
			}
			base.Assign(source);
		}
		public Control FindControl(string id) {
			ASPxTitleIndex titleIndex = Collection.Owner as ASPxTitleIndex;
			return TemplateContainerBase.FindTemplateControl(titleIndex, titleIndex.GetItemTemplateContainerID(this), id);
		}
		public override string ToString() {
			return (Text != "") ? Text : GetType().Name;
		}
		protected internal void SetDataItem(object value) {
			fDataItem = value;
		}
		protected void GroupValueChanged() {
			if (TitleIndex != null)
				TitleIndex.GroupChanged();
		}
	}
	public class TitleIndexItemCollection : Collection<TitleIndexItem> {
		public TitleIndexItemCollection()
			: base() {
		}
		public TitleIndexItemCollection(ASPxTitleIndex titleIndex)
			: base(titleIndex) {
		}
		protected ASPxTitleIndex TitleIndex {
			get {
				if(Owner != null)
					return Owner as ASPxTitleIndex;
				return null;
			}
		}
		public virtual TitleIndexItem Add() {
			return AddInternal(new TitleIndexItem());
		}
		public virtual TitleIndexItem Add(string text) {
			return Add(text, "");
		}
		public virtual TitleIndexItem Add(string text, string navigateUrl) {
			return AddInternal(new TitleIndexItem(text, navigateUrl));
		}
		public virtual TitleIndexItem Add(string text, string navigateUrl, string name) {
			return AddInternal(new TitleIndexItem(text, navigateUrl, name));
		}
		public TitleIndexItem FindByName(string name) {
			return FindByIndex(IndexOfName(name));
		}
		public int IndexOfName(string name) {
			return IndexOf(delegate(TitleIndexItem item) {
				return item.Name == name;
			});
		}
		public TitleIndexItem FindByNameOrIndex(string nameOrIndex) {
			TitleIndexItem item = FindByName(nameOrIndex);
			if(item == null) {
				int index;
				if(Int32.TryParse(nameOrIndex, out index) && index >= 0 && index < Count)
					return this[index];
			}
			return item;
		}
		protected override void OnChanged() {
			if (TitleIndex != null)
				TitleIndex.ItemsChanged();
		}
	}
	public class TitleIndexPropertiesBuilder : ControlBuilder {
		private void InitializeProperties(Type type, IDictionary attribs) {
			object groupValue = attribs["GroupValue"];
			if (groupValue != null) {
				attribs["GroupValueString"] = groupValue;
				attribs.Remove("GroupValue");
			}
		}
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			InitializeProperties(type, attribs);
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
	}
}
namespace DevExpress.Web.Internal {
	public class TitleIndexNode : CollectionItem, IHierarchyData {
		private TitleIndexNodeCollection fChildNodes = null;
		private TitleIndexItem fItem = null;
		public TitleIndexItem Item {
			get {
				if (fItem == null)
					fItem = new TitleIndexItem();
				return fItem;
			}
		}
		public string Description {
			get { return Item.Description; }
			set { Item.Description = value; }
		}
		public object GroupingValue {
			get { return Item.GroupValue; }
			set { Item.GroupValue = value; }
		}
		public string Name {
			get { return Item.Name; }
			set { Item.Name = value; }
		}
		public string NavigateUrl {
			get { return Item.NavigateUrl; }
			set { Item.NavigateUrl = value; }
		}
		public string Text {
			get { return Item.Text; }
			set { Item.Text = value; }
		}
		protected internal TitleIndexNodeCollection ChildNodes {
			get {
				if (fChildNodes == null)
					fChildNodes = new TitleIndexNodeCollection(this);
				return fChildNodes;
			}
		}
		protected internal TitleIndexNode ParentNode {
			get {
				if (Collection is TitleIndexNodeCollection)
					return (Collection as TitleIndexNodeCollection).ParentItem;
				return null;
			}
		}
		protected internal TitleIndexNode NextSibling {
			get {
				int index = (Collection as TitleIndexNodeCollection).IndexOf(this);
				return index + 1 <= Collection.Count - 1 ? (Collection as TitleIndexNodeCollection)[index + 1] :
					null;
			}
		}
		protected internal TitleIndexNode PreviousSibling {
			get {
				int index = (Collection as TitleIndexNodeCollection).IndexOf(this);
				return index - 1 >= 0 ? (Collection as TitleIndexNodeCollection)[index - 1] : null;
			}
		}
		public TitleIndexNode()
			: base() {
		}
		public TitleIndexNode(TitleIndexItem item)
			: base() {
			fItem = item;
		}
		public TitleIndexNode(string text)
			: base() {
			Text = text;
		}
		public TitleIndexNode(string text, string navigateUrl)
			: this(text) {
			NavigateUrl = navigateUrl;
		}
		public TitleIndexNode(string text, string navigateUrl, string name)
			: this(text, navigateUrl) {
			Name = name;
		}
		public override void Assign(CollectionItem source) {
			if (source is TitleIndexNode) {
				TitleIndexNode src = source as TitleIndexNode;
				ChildNodes.Assign(src.ChildNodes);
				fItem = src.fItem;
			}
			base.Assign(source);
		}
		bool IHierarchyData.HasChildren {
			get { return ChildNodes.Count > 0; }
		}
		Object IHierarchyData.Item { get { return this; } }
		string IHierarchyData.Path {
			get { return ""; }
		}
		string IHierarchyData.Type {
			get { return this.GetType().Name; }
		}
		IHierarchicalEnumerable IHierarchyData.GetChildren() {
			return ChildNodes;
		}
		public TitleIndexNodeCollection GetAllItems() {
			return ChildNodes;
		}
		IHierarchyData IHierarchyData.GetParent() {
			return ParentNode;
		}
	}
	public class TitleIndexNodeCollection : HierarchicalCollection<TitleIndexNode> {
		public TitleIndexNodeCollection()
			: base() {
		}
		public TitleIndexNodeCollection(TitleIndexNode item)
			: base(item) {
		}
		protected internal TitleIndexNode ParentItem {
			get { return Owner as TitleIndexNode; }
		}
		public override void Add(TitleIndexNode node) {
			TitleIndexNode newNode = new TitleIndexNode();
			newNode.Assign(node);
			base.Add(newNode);
		}
		public TitleIndexNode Add() {
			return AddInternal(new TitleIndexNode());
		}
		public TitleIndexNode Add(string text) {
			return Add(text, "");
		}
		public TitleIndexNode Add(string text, string navigateUrl) {
			return AddInternal(new TitleIndexNode(text, navigateUrl));
		}
		public TitleIndexNode Add(string text, string navigateUrl, string name) {
			return AddInternal(new TitleIndexNode(text, navigateUrl, name));
		}
	}
}
