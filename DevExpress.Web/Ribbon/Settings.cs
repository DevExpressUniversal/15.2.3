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
using System.Text;
using System.Web.UI;
using DevExpress.Web;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class RibbonBaseSettings : PropertiesBase {
		public RibbonBaseSettings(ASPxRibbon ribbon) {
			Ribbon = ribbon;
		}
		protected ASPxRibbon Ribbon { get; private set; }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class RibbonDataFields : RibbonBaseSettings, IDataSourceViewSchemaAccessor {
		const string DefaultNameField = "Name";
		const string DefaultTextField = "Text";
		public RibbonDataFields(ASPxRibbon ribbon) 
			:base(ribbon) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonDataFieldsNameField"),
#endif
		DefaultValue(DefaultNameField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string NameField
		{
			get { return GetStringProperty("NameField", DefaultNameField); }
			set
			{
				SetStringProperty("NameField", DefaultNameField, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonDataFieldsTextField"),
#endif
		DefaultValue(DefaultTextField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string TextField
		{
			get { return GetStringProperty("TextField", DefaultTextField); }
			set
			{
				SetStringProperty("TextField", DefaultTextField, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var field = source as RibbonDataFields;
			if(field != null){
				NameField = field.NameField;
				TextField = field.TextField;
			}
		}
		protected override void Changed() {
			if(Ribbon != null)
				Ribbon.OnDataFieldChangedInternal();
		}
		#region IDataSourceViewSchemaAccessor Members
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema {
			get { return ((IDataSourceViewSchemaAccessor)Ribbon).DataSourceViewSchema; }
			set { ((IDataSourceViewSchemaAccessor)Ribbon).DataSourceViewSchema = value; }
		}
		#endregion
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonTabDataFields : RibbonDataFields {
		const string DefaultIsContextField = "IsContext";
		const string DefaultCategoryNameField = "CategoryName";
		const string DefaultColorField = "Color";
		public RibbonTabDataFields(ASPxRibbon ribbon)
			: base(ribbon) { }
		[DefaultValue(DefaultCategoryNameField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ContextTabCategoryNameField {
			get { return GetStringProperty("ContextTabCategoryNameField", DefaultCategoryNameField); }
			set {
				SetStringProperty("ContextTabCategoryNameField", DefaultCategoryNameField, value);
				Changed();
			}
		}
		[DefaultValue(DefaultColorField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ColorField {
			get { return GetStringProperty("ColorField", DefaultColorField); }
			set {
				SetStringProperty("ColorField", DefaultColorField, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var tabData = source as RibbonTabDataFields;
			if(tabData != null) {
				ContextTabCategoryNameField = tabData.ContextTabCategoryNameField;
				ColorField = tabData.ColorField;
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class RibbonGroupDataFields : RibbonDataFields {
		const string DefaultImageUrlField = "ImageUrl";
		const string DefaultShowDialogBoxLauncherField = "ShowDialogBoxLauncher";
		public RibbonGroupDataFields(ASPxRibbon ribbon)
			:base(ribbon) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonGroupDataFieldsImageUrlField"),
#endif
		DefaultValue(DefaultImageUrlField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ImageUrlField
		{
			get { return GetStringProperty("ImageUrlField", DefaultImageUrlField); }
			set
			{
				SetStringProperty("ImageUrlField", DefaultImageUrlField, value);
				Changed();
			}
		}
		[DefaultValue(DefaultShowDialogBoxLauncherField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ShowDialogBoxLauncherField {
			get { return GetStringProperty("ShowDialogBoxLauncherField", DefaultShowDialogBoxLauncherField); }
			set {
				SetStringProperty("ShowDialogBoxLauncherField", DefaultShowDialogBoxLauncherField, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var groupData = source as RibbonGroupDataFields;
			if(groupData != null) {
				ImageUrlField = groupData.ImageUrlField;
				ShowDialogBoxLauncherField = groupData.ShowDialogBoxLauncherField;
			}
		}
	}
	public class RibbonItemDataFields : RibbonDataFields {
		const string DefaultLargeImageUrlField = "LargeImageUrl";
		const string DefaultSmallImageUrlField = "SmallImageUrl";
		const string DefaultSizeField = "Size";
		const string DefaultItemTypeField = "ItemType";
		const string DefaultBeginGroupField = "BeginGroup";
		const string DefaultNavigateUrlField = "NavigateUrl";
		const string DefaultToolTipField = "ToolTip";
		const string DefaultOptionGroupNameField = "OptionGroupName";
		public RibbonItemDataFields(ASPxRibbon ribbon)
			:base(ribbon) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemDataFieldsItemTypeField"),
#endif
		DefaultValue(DefaultItemTypeField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ItemTypeField
		{
			get { return GetStringProperty("ItemTypeField", DefaultItemTypeField); }
			set
			{
				SetStringProperty("ItemTypeField", DefaultItemTypeField, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemDataFieldsNavigateUrlField"),
#endif
		DefaultValue(DefaultNavigateUrlField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string NavigateUrlField
		{
			get { return GetStringProperty("NavigateUrlField", DefaultNavigateUrlField); }
			set
			{
				SetStringProperty("NavigateUrlField", DefaultNavigateUrlField, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemDataFieldsLargeImageUrlField"),
#endif
		DefaultValue(DefaultLargeImageUrlField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string LargeImageUrlField
		{
			get { return GetStringProperty("LargeImageUrlField", DefaultLargeImageUrlField); }
			set
			{
				SetStringProperty("LargeImageUrlField", DefaultLargeImageUrlField, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemDataFieldsSmallImageUrlField"),
#endif
		DefaultValue(DefaultSmallImageUrlField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string SmallImageUrlField
		{
			get { return GetStringProperty("SmallImageUrlField", DefaultSmallImageUrlField); }
			set
			{
				SetStringProperty("SmallImageUrlField", DefaultSmallImageUrlField, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemDataFieldsSizeField"),
#endif
		DefaultValue(DefaultSizeField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string SizeField
		{
			get { return GetStringProperty("SizeField", DefaultSizeField); }
			set
			{
				SetStringProperty("SizeField", DefaultSizeField, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemDataFieldsBeginGroupField"),
#endif
		DefaultValue(DefaultBeginGroupField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string BeginGroupField
		{
			get { return GetStringProperty("BeginGroupField", DefaultBeginGroupField); }
			set
			{
				SetStringProperty("BeginGroupField", DefaultBeginGroupField, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemDataFieldsToolTipField"),
#endif
		DefaultValue(DefaultToolTipField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ToolTipField
		{
			get { return GetStringProperty("ToolTipField", DefaultToolTipField); }
			set
			{
				SetStringProperty("ToolTipField", DefaultToolTipField, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RibbonItemDataFieldsOptionGroupNameField"),
#endif
		DefaultValue(DefaultOptionGroupNameField), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string OptionGroupNameField
		{
			get { return GetStringProperty("OptionGroupNameField", DefaultOptionGroupNameField); }
			set
			{
				SetStringProperty("OptionGroupNameField", DefaultOptionGroupNameField, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var itemDataField = source as RibbonItemDataFields;
			if(itemDataField != null) {
				ItemTypeField = itemDataField.ItemTypeField;
				LargeImageUrlField = itemDataField.LargeImageUrlField;
				SmallImageUrlField = itemDataField.SmallImageUrlField;
				SizeField = itemDataField.SizeField;
				BeginGroupField = itemDataField.BeginGroupField;
				NavigateUrlField = itemDataField.NavigateUrlField;
				ToolTipField = itemDataField.ToolTipField;
				OptionGroupNameField = itemDataField.OptionGroupNameField;
			}
		}
	}
	public class RibbonSettingsPopupMenu : RibbonBaseSettings {
		public RibbonSettingsPopupMenu(ASPxRibbon ribbon) 
			:base(ribbon) { }
		[DefaultValue(PopupMenuCloseAction.OuterMouseClick), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public PopupMenuCloseAction CloseAction {
			get { return (PopupMenuCloseAction)GetEnumProperty("CloseAction", PopupMenuCloseAction.OuterMouseClick); }
			set { SetEnumProperty("CloseAction", PopupMenuCloseAction.OuterMouseClick, value); }
		}
		[Category("Behavior"), DefaultValue(false), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public bool EnableScrolling {
			get { return GetBoolProperty("EnableScrolling", false); }
			set {
				SetBoolProperty("EnableScrolling", false, value);
				Ribbon.LayoutChanged();
			}
		}
		[DefaultValue(PopupHorizontalAlign.LeftSides), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public PopupHorizontalAlign PopupHorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.LeftSides); }
			set { SetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.LeftSides, value); }
		}
		[DefaultValue(PopupVerticalAlign.Below), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public PopupVerticalAlign PopupVerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.Below); }
			set { SetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.Below, value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as RibbonSettingsPopupMenu;
			if(src != null) {
				CloseAction = src.CloseAction;
				EnableScrolling = src.EnableScrolling;
				PopupHorizontalAlign = src.PopupHorizontalAlign;
				PopupVerticalAlign = src.PopupVerticalAlign;
			}
		}
	}
}
