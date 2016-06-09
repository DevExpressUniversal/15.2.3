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

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor {
	public enum DropDownItemClickMode {
		ShowDropDown				= 0,
		ExecuteAction			   = 1,
		ExecuteSelectedItemAction   = 2
	}
	public abstract class ToolbarDropDownBase : HtmlEditorToolbarItem {
		protected const string CustomItemPickerClientID = "ItemPicker";
		Collection items = null;
		bool dropDownItemsPopulated = false;
		public ToolbarDropDownBase()
			: base() {
		}
		public ToolbarDropDownBase(string text)
			: base(text) {
		}
		public ToolbarDropDownBase(string text, string commandName)
			: base(text, commandName) {
		}
		public ToolbarDropDownBase(string text, string commandName, string toolTip)
			: base(text, commandName, toolTip) {
		}
		public virtual void CreateDefaultItems() { }
		protected internal virtual Collection ItemsInternal {
			get {
				if(items == null)
					items = CreateDropDownItemCollection();
				return items;
			}
		}
		public override void Assign(CollectionItem source) {
			ToolbarDropDownBase dropDownBtn = source as ToolbarDropDownBase;
			if(dropDownBtn != null) {
				ItemsInternal.Assign(dropDownBtn.ItemsInternal);
				SelectedItemIndexInternal = dropDownBtn.SelectedItemIndexInternal;
				ClickModeInternal = dropDownBtn.ClickModeInternal;
			}
			base.Assign(source);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				ItemsInternal
			});
		}
		internal virtual bool IsMenu { get { return false; } }
		protected virtual Collection CreateDropDownItemCollection() {
			return new HtmlEditorToolbarItemCollection(this);
		}
		protected internal int SelectedItemIndexInternal {
			get {
				int index = GetIntProperty("SelectedItemIndex", 0);
				return index > ItemsInternal.Count - 1 ? 0 : index;
			}
			set { SetIntProperty("SelectedItemIndex", 0, value); }
		}
		protected internal DropDownItemClickMode ClickModeInternal {
			get { return (DropDownItemClickMode)GetEnumProperty("ClickMode", GetDefaultClickModeValue()); }
			set { SetEnumProperty("ClickMode", GetDefaultClickModeValue(), value); }
		}
		protected internal bool DropDownItemsPopulated { get { return dropDownItemsPopulated; } set { dropDownItemsPopulated = value; } }
		protected internal bool IsItemsImageLess() {
			foreach(ToolbarCustomItem item in ItemsInternal)
				if(!item.GetImageProperties().IsEmpty)
					return false;
			return true;
		}
		protected virtual DropDownItemClickMode GetDefaultClickModeValue() {
			return DropDownItemClickMode.ShowDropDown;
		}
		protected internal virtual ToolbarCustomItem GetSelectedItem() {
			return null;
		}
		public string GetButtonTemplateID() {
			return CustomItemPickerClientID;
		}
		public virtual string GetDropDownTemplateID() {
			return CustomItemPickerClientID;
		}
		protected internal virtual string GetDesignTimeValue(HtmlEditorDocumentStyles docStyle) {
			return string.Empty;
		}
	}
	public abstract class ToolbarComboBoxBase : ToolbarDropDownBase {
		private ToolbarComboBoxProperties properties = null;
		public ToolbarComboBoxBase()
			: base() {
		}
		public ToolbarComboBoxBase(string text)
			: base(text) {
		}
		public ToolbarComboBoxBase(string text, string commandName)
			: base(text, commandName) {
		}
		public ToolbarComboBoxBase(string text, string commandName, string toolTip)
			: base(text, commandName, toolTip) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarComboBoxBaseDropDownHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public virtual Unit DropDownHeight {
			get { return Properties.DropDownHeight; }
			set { Properties.DropDownHeight = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarComboBoxBaseDropDownWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public virtual Unit DropDownWidth {
			get { return Properties.DropDownWidth; }
			set { Properties.DropDownWidth = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarComboBoxBaseDefaultCaption"),
#endif
		DefaultValue(""), AutoFormatDisable, Themeable(false)]
		public virtual string DefaultCaption {
			get { return Properties.DefaultCaption; }
			set { Properties.DefaultCaption = value; }
		}
		protected internal override Collection ItemsInternal {
			get { return Properties.Items; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarComboBoxBaseWidth"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, Themeable(false)]
		public virtual Unit Width {
			get { return GetUnitProperty("Width", GetDefaultWidth()); }
			set {
				SetUnitProperty("Width", GetDefaultWidth(), value);
				LayoutChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ToolbarItemImageProperties Image { get { return base.Image; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text { get { return base.Text; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ViewStyle ViewStyle { get { return base.ViewStyle; } set { base.ViewStyle = value; } }
		protected virtual Unit GetDefaultWidth() {
			return Unit.Empty;
		}
		protected virtual string GetDefaultCaption() {
			return "";
		}
		protected internal ToolbarComboBoxProperties Properties {
			get {
				if(properties == null)
					properties = CreateComboBoxProperties(this);
				return properties;
			}
		}
		protected internal virtual ToolbarComboBoxProperties CreateComboBoxProperties(IPropertiesOwner owner) {
			return new ToolbarComboBoxProperties(owner, GetDefaultCaption());
		}
		protected internal virtual ToolbarComboBoxControl CreateComboBoxInstance(ASPxWebControl owner, ToolbarComboBoxProperties properties) {
			return new ToolbarComboBoxControl(owner, properties);
		}
		public override void Assign(CollectionItem source) {
			ToolbarComboBoxBase dropDown = source as ToolbarComboBoxBase;
			if(dropDown != null) {
				DefaultCaption = dropDown.DefaultCaption;
				DropDownHeight = dropDown.DropDownHeight;
				DropDownWidth = dropDown.DropDownWidth;
				Width = dropDown.Width;
				ItemsInternal.Assign(dropDown.ItemsInternal);
				Properties.Assign(dropDown.Properties);
			}
			base.Assign(source);
		}
		protected internal override bool IsImageVisible() {
			return false;
		}
		protected internal override bool IsTextVisible() {
			return false;
		}
		protected override Collection CreateDropDownItemCollection() {
			return null;
		}
		protected override string[] GetDesignTimeHiddenPropertyNames() {
			return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "Properties" });
		}
		protected override PropertiesBase GetDesignTimeItemEditProperties() {
			return Properties;
		}
	}
	public abstract class ToolbarColorButtonBase : ToolbarDropDownBase {
		protected internal static string[] ItemIdPostfixes = new string[] { "" };
		protected internal static string[] ItemImageIdPostfixes = new string[] { "_P" };
		string color = "";
		DefaultBoolean enableCustomColors = DefaultBoolean.Default;
		ColorNestedControlProperties colorNestedControlProperties;
		public ToolbarColorButtonBase(string text, string commandName, string toolTip, string color)
			: base(text, commandName, toolTip) {
			this.color = color;
		}
		protected internal ColorNestedControlProperties ColorNestedControlProperties {
			get {
				if(colorNestedControlProperties == null)
					colorNestedControlProperties = new ColorNestedControlProperties();
				return colorNestedControlProperties;
			}
		}
		protected internal string Color {
			get { return color; }
		}
		protected internal override Collection ItemsInternal {
			get { return Items; }
		}
		public string GetMainTemplateID() {
			return CommandName;
		}
		public string GetClientMainTemplateID() {
			return string.Format("ITTCNT{0}_{1}", Index, GetMainTemplateID()); 
		}
		public override string GetDropDownTemplateID() {
			return CommandName;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarColorButtonBaseColumnCount"),
#endif
		DefaultValue(ColorTable.DefaultColumnCount),
		NotifyParentProperty(true), AutoFormatDisable]
		public int ColumnCount {
			get { return ColorNestedControlProperties.ColumnCount; }
			set { ColorNestedControlProperties.ColumnCount = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarColorButtonBaseItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		MergableProperty(false), AutoFormatDisable, Themeable(false)]
		public ColorEditItemCollection Items {
			get { return ColorNestedControlProperties.Items; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarColorButtonBaseEnableCustomColors"),
#endif
		DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true), AutoFormatDisable]
		public DefaultBoolean EnableCustomColors {
			get { return enableCustomColors; }
			set { enableCustomColors = value; }
		}
		public override void CreateDefaultItems() {
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ToolbarColorButtonBase src = source as ToolbarColorButtonBase;
			if(src != null) {
				ColorNestedControlProperties.Assign(src.ColorNestedControlProperties);
				EnableCustomColors = src.EnableCustomColors;
			}
		}
	}
	public abstract class ToolbarCustomDropDownBase : ToolbarDropDownBase {
		object dataSource;
		public ToolbarCustomDropDownBase()
			: base() {
		}
		public ToolbarCustomDropDownBase(string commandName)
			: this() {
			CommandName = commandName;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDropDownBaseSelectedItemIndex"),
#endif
		DefaultValue(0), NotifyParentProperty(true)]
		public int SelectedItemIndex { get { return SelectedItemIndexInternal; } set { SelectedItemIndexInternal = value; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDropDownBaseCommandName"),
#endif
		Category("Behavior"), DefaultValue(""), NotifyParentProperty(true), Localizable(true),
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override string CommandName {
			get { return base.CommandName; }
			set { base.CommandName = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDropDownBaseClickMode"),
#endif
		Category("Behavior"), DefaultValue(DropDownItemClickMode.ShowDropDown), AutoFormatDisable]
		public virtual DropDownItemClickMode ClickMode { get { return ClickModeInternal; } set { ClickModeInternal = value; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDropDownBaseDataSourceID"),
#endif
		Category("Data"), DefaultValue(""), IDReferenceProperty(typeof(DataSourceControl)),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string DataSourceID {
			get { return GetStringProperty("DataSourceID", ""); }
			set { SetStringProperty("DataSourceID", "", value); }
		}
		[Category("Data"), Browsable(false)]
		public object DataSource {
			get { return dataSource; }
			set { dataSource = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDropDownBaseTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string TextField {
			get { return GetStringProperty("TextField", ""); }
			set { SetStringProperty("TextField", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDropDownBaseValueField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ValueField {
			get { return GetStringProperty("ValueField", ""); }
			set { SetStringProperty("ValueField", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDropDownBaseImageUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ImageUrlField {
			get { return GetStringProperty("ImageUrlField", ""); }
			set { SetStringProperty("ImageUrlField", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ToolbarCustomDropDownBaseTooltipField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable,
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string TooltipField {
			get { return GetStringProperty("TooltipField", ""); }
			set { SetStringProperty("TooltipField", "", value); }
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			ToolbarCustomDropDownBase src = source as ToolbarCustomDropDownBase;
			if(src != null) {
				DataSourceID = src.DataSourceID;
				DataSource = src.DataSource;
				TextField = src.TextField;
				ValueField = src.ValueField;
				ImageUrlField = src.ImageUrlField;
				TooltipField = src.TooltipField;
			}
		}
	}
}
