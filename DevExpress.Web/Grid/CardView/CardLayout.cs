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

using DevExpress.Web.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.Utils;
namespace DevExpress.Web {
	public class CardViewCustomCommandButton : GridCustomCommandButton {
		public CardViewCustomCommandButton()
			: base() {
		}
		protected CardViewCustomCommandButtonCollection ButtonCollection { get { return Collection as CardViewCustomCommandButtonCollection; } }
		protected internal CardViewCommandLayoutItem LayoutItem { get { return ButtonCollection != null ? ButtonCollection.LayoutItem : null; } }
	}
	public class CardViewCustomCommandButtonCollection : Collection<CardViewCustomCommandButton> {
		public CardViewCustomCommandButtonCollection(IWebControlObject owner)
			: base(owner) {
		}
		[Browsable(false)]
		public CardViewCustomCommandButton this[string ID_Text] {
			get {
				var button = this.FirstOrDefault(b => b.ID == ID_Text);
				if(button == null)
					button = this.FirstOrDefault(b => b.Text == ID_Text);
				return button;
			}
		}
		protected internal CardViewCommandLayoutItem LayoutItem { get { return Owner as CardViewCommandLayoutItem; } }
		public override string ToString() { return string.Empty; }
		protected override void OnChanged() {
			Owner.LayoutChanged();
		}
	}
	public class CardViewLayoutItemCollection : GridLayoutItemCollection {
		public CardViewLayoutItemCollection() : base() { }
		public CardViewLayoutItemCollection(IWebControlObject owner) : base(owner) { }
		public CardViewColumnLayoutItem AddColumnItem(CardViewColumnLayoutItem layoutItem) {
			return (CardViewColumnLayoutItem)base.Add(layoutItem);
		}
		public EditModeCommandLayoutItem AddCommandItem(EditModeCommandLayoutItem commandItem) {
			return (EditModeCommandLayoutItem)base.Add(commandItem);
		}
		public CardViewCommandLayoutItem AddCommandItem(CardViewCommandLayoutItem commandItem) {
			return (CardViewCommandLayoutItem)base.Add(commandItem);
		}
		public CardViewLayoutGroup AddGroup(CardViewLayoutGroup layoutGroup) {
			return (CardViewLayoutGroup)base.Add(layoutGroup);
		}
		public CardViewTabbedLayoutGroup AddTabbedGroup(CardViewTabbedLayoutGroup tabbedGroup) {
			return (CardViewTabbedLayoutGroup)base.Add(tabbedGroup);
		}
		public CardViewColumnLayoutItem AddColumnItem(string columnName) {
			return AddColumnItem(columnName, null);
		}
		public CardViewLayoutGroup AddGroup(string caption) {
			return base.Add<CardViewLayoutGroup>(caption);
		}
		public CardViewTabbedLayoutGroup AddTabbedGroup(string caption) {
			return base.Add<CardViewTabbedLayoutGroup>(caption);
		}
		public CardViewColumnLayoutItem AddColumnItem(string columnName, string caption) {
			return (CardViewColumnLayoutItem)AddColumnItem(new CardViewColumnLayoutItem(), columnName, caption);
		}
		public CardViewLayoutGroup AddGroup(string caption, string name) {
			return base.Add<CardViewLayoutGroup>(caption, name);
		}
		public CardViewTabbedLayoutGroup AddTabbedGroup(string caption, string name) {
			return base.Add<CardViewTabbedLayoutGroup>(caption, name);
		}
		protected override Type[] GetKnownTypes() {
			return new Type[] {
				typeof(CardViewColumnLayoutItem),
				typeof(CardViewCommandLayoutItem),
				typeof(EditModeCommandLayoutItem),
				typeof(CardViewLayoutGroup),
				typeof(CardViewTabbedLayoutGroup),
				typeof(EmptyLayoutItem)
			};
		}
	}
	public class CardViewColumnLayoutItem: ColumnLayoutItem {
		public CardViewColumnLayoutItem()
			: base() {
		}
		protected internal CardViewColumnLayoutItem(IWebGridDataColumn column)
			: base(column) {
		}
		[Browsable(false), AutoFormatEnable, DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(CardViewDataItemTemplateContainer), BindingDirection.TwoWay), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override ITemplate Template {
			get { return base.Template; }
			set { base.Template = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public CardViewColumn Column {
			get { return ColumnInternal as CardViewColumn; }
		}
		protected override string GetColumnCaption() {
			return Column.ToString();
		}
		protected internal override bool AllowEllipsisInText {
			get {
				if(Column == null)
					return false;
				if(Column.Settings.AllowEllipsisInText == DefaultBoolean.Default && Column.CardView != null)
					return Column.CardView.SettingsBehavior.AllowEllipsisInText;
				return Column.Settings.AllowEllipsisInText == DefaultBoolean.True;
			}
		}
	}
	public class CardViewCommandLayoutItem : CommandLayoutItem {
		public CardViewCommandLayoutItem()
			: base() {
			CustomButtons = new CardViewCustomCommandButtonCollection(this);
		}
		[ Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowDeleteButton {
			get { return GetBoolProperty("ShowDeleteButton", false); }
			set {
				if(ShowDeleteButton == value) return;
				SetBoolProperty("ShowDeleteButton", false, value);
				LayoutChanged();
			}
		}
		[ Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowEditButton {
			get { return GetBoolProperty("ShowEditButton", false); }
			set {
				if(ShowEditButton == value) return;
				SetBoolProperty("ShowEditButton", false, value);
				LayoutChanged();
			}
		}
		[ Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowNewButton {
			get { return GetBoolProperty("ShowNewButton", false); }
			set {
				if(ShowNewButton == value) return;
				SetBoolProperty("ShowNewButton", false, value);
				LayoutChanged();
			}
		}
		[ Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowSelectButton {
			get { return GetBoolProperty("ShowSelectButton", false); }
			set {
				if(ShowSelectButton == value) return;
				SetBoolProperty("ShowSelectButton", false, value);
				LayoutChanged();
			}
		}
		[ Category("Buttons"), DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowSelectCheckbox {
			get { return GetBoolProperty("ShowSelectCheckbox", false); }
			set {
				if(ShowSelectCheckbox == value) return;
				SetBoolProperty("ShowSelectCheckbox", false, value);
				LayoutChanged();
			}
		}
		[ Category("Buttons"), DefaultValue(GridCommandButtonRenderMode.Default), NotifyParentProperty(true)]
		public GridCommandButtonRenderMode ButtonRenderMode {
			get { return (GridCommandButtonRenderMode)GetEnumProperty("ButtonRenderMode", GridCommandButtonRenderMode.Default); }
			set {
				if(value == ButtonRenderMode) return;
				SetEnumProperty("ButtonRenderMode", GridCommandButtonRenderMode.Default, value);
				LayoutChanged();
			}
		}
		[ Category("Data"), PersistenceMode(PersistenceMode.InnerProperty), 
		MergableProperty(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable, 
		NotifyParentProperty(true), TypeConverter(typeof(DevExpress.Utils.Design.UniversalCollectionTypeConverter))]
		public CardViewCustomCommandButtonCollection CustomButtons { get; private set; }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var commandItem = source as CardViewCommandLayoutItem;
			if(commandItem != null) {
				ShowDeleteButton = commandItem.ShowDeleteButton;
				ShowEditButton = commandItem.ShowEditButton;
				ShowNewButton = commandItem.ShowNewButton;
				ShowSelectButton = commandItem.ShowSelectButton;
				ShowSelectCheckbox = commandItem.ShowSelectCheckbox;
				CustomButtons.Assign(commandItem.CustomButtons);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] { CustomButtons }); }
		protected internal override bool NeedAdditionalTableForRender() {
			return FormLayout.DesignMode;
		}
	}
	public class CardViewLayoutGroup : GridLayoutGroup {
		public CardViewLayoutGroup() : base() { }
		public CardViewLayoutGroup(string caption) : base(caption) { }
		protected internal CardViewLayoutGroup(FormLayoutProperties owner) : base(owner) { }
		protected override LayoutItemCollection CreateItems() {
			return new CardViewLayoutItemCollection(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewLayoutGroupItems"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), NotifyParentProperty(true),
		AutoFormatEnable, Themeable(true), Browsable(false)]
		public new CardViewLayoutItemCollection Items {
			get { return (CardViewLayoutItemCollection)base.Items; }
		}
	}
	public class CardViewTabbedLayoutGroup : GridTabbedLayoutGroup {
		public CardViewTabbedLayoutGroup() : base() { }
		public CardViewTabbedLayoutGroup(string caption) : base(caption) { }
		protected override LayoutItemCollection CreateItems() {
			return new CardViewLayoutItemCollection(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewTabbedLayoutGroupItems"),
#endif
 PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), NotifyParentProperty(true),
		AutoFormatEnable, Themeable(true), Browsable(false)]
		public new CardViewLayoutItemCollection Items {
			get { return (CardViewLayoutItemCollection)base.Items; }
		}
	}
	public class CardViewFormLayoutProperties : GridFormLayoutProperties {
		public CardViewFormLayoutProperties(IPropertiesOwner owner) : base(owner) { }
		public CardViewFormLayoutProperties() : this(null) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewFormLayoutPropertiesItems"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public new CardViewLayoutItemCollection Items { get { return (CardViewLayoutItemCollection)base.Items; } }
		protected override LayoutGroup CreateRootGroup() { return new CardViewLayoutGroup(this); }
	}
}
