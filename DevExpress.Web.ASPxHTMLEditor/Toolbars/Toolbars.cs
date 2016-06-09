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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	using DevExpress.Web.Design;
	public enum ViewStyle { Image, Text, ImageAndText };
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class HtmlEditorToolbarCollection : Collection<HtmlEditorToolbar> {
		public HtmlEditorToolbarCollection()
			: base() {
		}
		public HtmlEditorToolbarCollection(IWebControlObject owner)
			: base(owner) {
		}
		public HtmlEditorToolbar this[string name] {
			get {
				return Find(delegate(HtmlEditorToolbar toolbar) {
					return toolbar.Name == name;
				});
			}
		}
		public HtmlEditorToolbar Add(){
			return AddInternal(new HtmlEditorToolbar());
		}
		public HtmlEditorToolbar Add(string name){
			return AddInternal(new HtmlEditorToolbar(name));
		}
		public HtmlEditorToolbar Add(string name, params HtmlEditorToolbarItem[] items) {
			return AddInternal(new HtmlEditorToolbar(name, items));
		}
		public void CreateDefaultToolbars() {
			CreateDefaultToolbars(false);
		}
		public virtual void CreateDefaultToolbars(bool rtl) {
			Add(HtmlEditorToolbar.CreateStandardToolbar1());
			Add(HtmlEditorToolbar.CreateStandardToolbar2(rtl));
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	public class HtmlEditorToolbar : CollectionItem {
		private HtmlEditorToolbarItemCollection items = null;
		public HtmlEditorToolbar()
			: base() {
		}
		public HtmlEditorToolbar(string name)
			: this() {
			Name = name;
		}
		public HtmlEditorToolbar(string name, params HtmlEditorToolbarItem[] items)
			: this(name) {
			Items.AddRange(items);
		}
		public static HtmlEditorToolbar CreateStandardToolbar1() {
			return new HtmlEditorToolbar("StandardToolbar1", StandardToolbartemsHelper.GetToolbar1Items()).CreateDefaultItems();
		}
		public static HtmlEditorToolbar CreateStandardToolbar2() {
			return CreateStandardToolbar2(false);
		}
		public static HtmlEditorToolbar CreateStandardToolbar2(bool rtl) {
			return new HtmlEditorToolbar("StandardToolbar2", StandardToolbartemsHelper.GetToolbar2Items(rtl)).CreateDefaultItems();
		}
		public static HtmlEditorToolbar CreateTableToolbar() {
			return new HtmlEditorToolbar("TableToolbar", StandardToolbartemsHelper.GetTableToolbarItems()).Items
				.ForEach(delegate(HtmlEditorToolbarItem item) { item.Text = ""; })
				.Toolbar;
		}
		internal static HtmlEditorToolbar GetToolbar(object owner) {
			ToolbarItemBase item = owner as ToolbarItemBase;
			if(item != null)
				return GetToolbar(item.Collection);
			Collection collection = owner as Collection;
			if(collection != null)
				return GetToolbar(collection.Owner);
			return owner as HtmlEditorToolbar;
		}
		protected internal HtmlEditorBarDockControl BarDock {
			get {
				HtmlEditorToolbarCollection collection = Collection as HtmlEditorToolbarCollection;
				return (collection != null) ? collection.Owner as HtmlEditorBarDockControl : null;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorToolbarCaption"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string Caption {
			get { return GetStringProperty("Caption", ""); }
			set { SetStringProperty("Caption", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorToolbarName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string Name {
			get { return GetStringProperty("Name", ""); }
			set { SetStringProperty("Name", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorToolbarVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Visible {
			get { return GetVisible(); }
			set { SetVisible(value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorToolbarVisibleIndex"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleIndex {
			get { return GetVisibleIndex(); }
			set { SetVisibleIndex(value); }
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Caption))
				return Caption;
			if(!string.IsNullOrEmpty(Name))
				return Name;
			return base.ToString();
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorToolbarItems"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public HtmlEditorToolbarItemCollection Items {
			get {
				if(items == null)
					items = CreateToolbarItemCollection();
				return items;
			}
		}
		public HtmlEditorToolbar CreateDefaultItems() {
			Items.ForEach(delegate(HtmlEditorToolbarItem item) {
				ToolbarDropDownBase dropDownItem = item as ToolbarDropDownBase;
				if(dropDownItem != null)
					dropDownItem.CreateDefaultItems();
			});
			return this;
		}
		public override void Assign(CollectionItem source) {
			HtmlEditorToolbar toolbar = source as HtmlEditorToolbar;
			if(toolbar != null) {
				Caption = toolbar.Caption;
				Name = toolbar.Name;
				Visible = toolbar.Visible;
				Items.Assign(toolbar.Items);
			}
		}
		protected internal virtual HtmlEditorToolbarItemCollection CreateToolbarItemCollection() {
			return new HtmlEditorToolbarItemCollection(this);
		}
		protected internal EditorImages GetEditorImages() {
			return BarDock.GetToolbarEditorImages();
		}
		protected internal bool IsHasRenderItems {
			get {
				foreach(HtmlEditorToolbarItem toolbarItem in Items)
					if(!IsEmptyToolbarItem(toolbarItem))
						return true;
				return false;
			}
		}
		protected internal bool IsEmptyToolbarItem(HtmlEditorToolbarItem toolbarItem) {
			ToolbarDropDownBase ddbaseItem = toolbarItem as ToolbarDropDownBase;
			if(ddbaseItem != null) {
				if((ddbaseItem.IsMenu && !(ddbaseItem is ToolbarCustomDropDownBase)) && ddbaseItem.ItemsInternal.Count == 0)
					return true;
				ToolbarCustomDropDownBase cddItem = ddbaseItem as ToolbarCustomDropDownBase;
				if(cddItem != null && cddItem.ItemsInternal.Count == 0 && cddItem.DataSource == null && string.IsNullOrEmpty(cddItem.DataSourceID))
					return true;
			}
			return false;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				Items });
		}
		protected override IList GetDesignTimeItems() {
			return Items;
		}
		protected override string GetDesignTimeCaption() {
			return ToString();
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return new string[] { "Items" };
		}
	}
	public abstract class ToolbarItemCollectionBase<T> : Collection<T> where T : ToolbarItemBase {
		public ToolbarItemCollectionBase()
			: base() {
		}
		public ToolbarItemCollectionBase(IWebControlObject owner)
			: base(owner) {
		}
		public HtmlEditorToolbar Toolbar { get { return HtmlEditorToolbar.GetToolbar(Owner); } }
		public T FindByText(string text) {
			return Find(delegate(T item) {
				return item.Text == text;
			});
		}
		protected override void OnChanged() {
			if(Owner != null)
				Owner.LayoutChanged();
			base.OnChanged();
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class HtmlEditorToolbarItemCollection : ToolbarItemCollectionBase<HtmlEditorToolbarItem> {
		public HtmlEditorToolbarItemCollection()
			: base() {
		}
		public HtmlEditorToolbarItemCollection(IWebControlObject owner)
			: base(owner) {
		}
		public HtmlEditorToolbarItem FindByCommandName(string commandName) {
			return Find(delegate(HtmlEditorToolbarItem item) {
				return item.CommandName == commandName;
			});
		}
		public new HtmlEditorToolbarItemCollection ForEach(Action<HtmlEditorToolbarItem> action) {
			return (HtmlEditorToolbarItemCollection)base.ForEach(action);
		}
	}
	public abstract class ToolbarItemBase : CollectionItem {
		ToolbarItemImageProperties image = null;
		public ToolbarItemBase()
			: this("", "") {
		}
		public ToolbarItemBase(string text)
			: this(text, "") {
		}
		public ToolbarItemBase(string text, string toolTip)
			: base() {
			Text = text;
			ToolTip = toolTip;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarItemBaseImage"),
#endif
		Category("Images"), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ToolbarItemImageProperties Image
		{
			get
			{
				if (image == null)
					image = new ToolbarItemImageProperties(this);
				return image;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarItemBaseText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public virtual string Text
		{
			get { return GetStringProperty("Text", ""); }
			set {
				SetStringProperty("Text", string.Empty, value);
				LayoutChanged();
			}
		}
		protected internal virtual string GetText() {
			return Text;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarItemBaseToolTip"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public virtual string ToolTip
		{
			get { return GetStringProperty("ToolTip", ""); }
			set
			{
				SetStringProperty("ToolTip", "", value);
				LayoutChanged();
			}
		}
		protected internal virtual string GetToolTip() {
			return ToolTip;
		}
		protected internal virtual string GetNotAllowedMessage() {
			return string.Empty;
		}
		protected internal virtual string GetCommandName() {
			return string.Empty;
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ToolbarItemBase button = source as ToolbarItemBase;
			if(button != null) {
				Image.Assign(button.Image);
				Text = button.Text;
				ToolTip = button.ToolTip;
			}
		}
		protected internal HtmlEditorToolbar Toolbar { get { return HtmlEditorToolbar.GetToolbar(Collection); } }
		protected internal ToolbarItemImageProperties GetImageProperties() {
			string resourceName = GetResourceImageName();
			if(string.IsNullOrEmpty(resourceName) || Toolbar == null || Toolbar.BarDock == null)
				return Image;
			else {
				ToolbarItemImageProperties image = Toolbar.BarDock.GetImageProperties(resourceName);
				image.CopyFrom(Image);
				return image;
			}
		}
		protected virtual string GetResourceImageName() {
			return string.Empty;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Image });
		}
		protected bool IsRightToLeftInternal() {
			return (Toolbar != null && Toolbar.BarDock != null && ((ISkinOwner)Toolbar.BarDock).IsRightToLeft());
		}
	}
	public abstract class HtmlEditorToolbarItem : ToolbarItemBase {
		public HtmlEditorToolbarItem()
			: this("", "") {
		}
		public HtmlEditorToolbarItem(string text)
			: this(text, text) {
		}
		public HtmlEditorToolbarItem(string text, string commandName)
			: this(text, commandName, "") {
		}
		public HtmlEditorToolbarItem(string text, string commandName, string toolTip)
			: base(text, toolTip) {
			CommandName = commandName;
		}
		public HtmlEditorToolbarItem(string text, string commandName, string toolTip, ViewStyle viewStyle)
			: this(text, commandName, toolTip) {
			ViewStyle = viewStyle;
		}
		bool checkable = false;
		protected internal bool Checkable { get { return checkable; } set { checkable = value; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorToolbarItemBeginGroup"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool BeginGroup {
			get { return GetBoolProperty("BeginGroup", false); }
			set {
				SetBoolProperty("BeginGroup", false, value);
				LayoutChanged();
			}
		}
		[DefaultValue(""), NotifyParentProperty(true), Localizable(true),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string CommandName {
			get { return GetStringProperty("CommandName", ""); }
			set { SetStringProperty("CommandName", "", value); }
		}
		protected internal override string GetCommandName() {
			return CommandName;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorToolbarItemViewStyle"),
#endif
		Category("Behavior"), DefaultValue(ViewStyle.Image), AutoFormatDisable]
		public virtual ViewStyle ViewStyle {
			get { return (ViewStyle)GetEnumProperty("ViewStyle", ViewStyle.Image); }
			set {
				SetEnumProperty("ViewStyle", ViewStyle.Image, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorToolbarItemVisible"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool Visible {
			get { return GetVisible(); }
			set {
				SetVisible(value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorToolbarItemVisibleIndex"),
#endif
		NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int VisibleIndex {
			get { return GetVisibleIndex(); }
			set { SetVisibleIndex(value); }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			HtmlEditorToolbarItem tollbarButton = source as HtmlEditorToolbarItem;
			if(tollbarButton != null) {
				BeginGroup = tollbarButton.BeginGroup;
				CommandName = tollbarButton.CommandName;
				ViewStyle = tollbarButton.ViewStyle;
				Visible = tollbarButton.Visible;
			}
			base.Assign(source);
		}
		protected internal virtual bool IsTextVisible() {
			return ViewStyle != ViewStyle.Image;
		}
		protected internal override string GetText() {
			return IsTextVisible()
				? base.GetText()
				: "";
		}
		protected internal virtual bool IsImageVisible() {
			return ViewStyle != ViewStyle.Text;
		}
		protected internal virtual bool GetDesignTimeCheckedState(HtmlEditorDocumentStyles docStyle) {
			return false;
		}
	}
}
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public static class StandardToolbartemsHelper {
		public static HtmlEditorToolbarItem[] GetToolbar1Items() {
			return new HtmlEditorToolbarItem[] { 
				new ToolbarCutButton(),
				new ToolbarCopyButton(),
				new ToolbarPasteButton(),
				new ToolbarPasteFromWordButton(),
				new ToolbarUndoButton(true),
				new ToolbarRedoButton(),
				new ToolbarRemoveFormatButton(true),
				new ToolbarSuperscriptButton(true),
				new ToolbarSubscriptButton(),
				new ToolbarInsertOrderedListButton(true),
				new ToolbarInsertUnorderedListButton(),
				new ToolbarIndentButton(true, true),
				new ToolbarOutdentButton(false, true),
				new ToolbarInsertLinkDialogButton(true),
				new ToolbarUnlinkButton(),
				new ToolbarInsertImageDialogButton(),
				new ToolbarTableOperationsDropDownButton(true),
				new ToolbarFindAndReplaceDialogButton(true),
				new ToolbarFullscreenButton(true)
			};
		}
		public static HtmlEditorToolbarItem[] GetToolbar2Items() {
			return GetToolbar2Items(false);
		}
		public static HtmlEditorToolbarItem[] GetToolbar2Items(bool rtl) {
			List<HtmlEditorToolbarItem> items = new List<HtmlEditorToolbarItem>();
			items.AddRange(new HtmlEditorToolbarItem[] { 
				new ToolbarParagraphFormattingEdit(),
				new ToolbarFontNameEdit(),
				new ToolbarFontSizeEdit(),
				new ToolbarBoldButton(true),
				new ToolbarItalicButton(),
				new ToolbarUnderlineButton(),
				new ToolbarStrikethroughButton()
			});
			if(rtl) {
				items.AddRange(new HtmlEditorToolbarItem[] { 
					new ToolbarJustifyRightButton(true),
					new ToolbarJustifyCenterButton(),
					new ToolbarJustifyLeftButton()
				});
			}
			else {
				items.AddRange(new HtmlEditorToolbarItem[] { 
					new ToolbarJustifyLeftButton(true),
					new ToolbarJustifyCenterButton(),
					new ToolbarJustifyRightButton()
				});
			}
			items.AddRange(new HtmlEditorToolbarItem[] { 
				new ToolbarBackColorButton(true),
				new ToolbarFontColorButton()
			});
			return items.ToArray();
		}
		public static HtmlEditorToolbarItem[] GetTableToolbarItems() {
			return new HtmlEditorToolbarItem[] { 
				new ToolbarInsertTableDialogButton(true),
				new ToolbarTablePropertiesDialogButton(true),
				new ToolbarTableRowPropertiesDialogButton(),
				new ToolbarTableColumnPropertiesDialogButton(),
				new ToolbarTableCellPropertiesDialogButton(),
				new ToolbarInsertTableRowAboveButton(true),
				new ToolbarInsertTableRowBelowButton(),
				new ToolbarInsertTableColumnToLeftButton(),
				new ToolbarInsertTableColumnToRightButton(),
				new ToolbarSplitTableCellHorizontallyButton(true),
				new ToolbarSplitTableCellVerticallyButton(),
				new ToolbarMergeTableCellRightButton(),
				new ToolbarMergeTableCellDownButton(),
				new ToolbarDeleteTableButton(true),
				new ToolbarDeleteTableRowButton(),
				new ToolbarDeleteTableColumnButton()
			};
		}
	}
}
