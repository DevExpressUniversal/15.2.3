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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public enum BinaryImageButtonPanelPosition { Top, Bottom };
	public enum BinaryImageButtonPosition { Left, Center, Right };
	public class BinaryImageEditingSettings : PropertiesBase {
		const bool AllowDropOnPreviewDefaultValue = true;
		const bool AllowEditDefaultValue = false;
		BinaryImageUploadSettings binaryImageUploadSettings;
		BinaryImageButtonPanelSettings buttonPanelSettings;
		public BinaryImageEditingSettings()
			: base() { }
		public BinaryImageEditingSettings(IPropertiesOwner owner)
			: base(owner) { }
		[AutoFormatEnable, Category("Settings"), 
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditingSettingsAllowDropOnPreview"),
#endif
		DefaultValue(AllowDropOnPreviewDefaultValue), NotifyParentProperty(true)]
		public bool AllowDropOnPreview {
			get { return GetBoolProperty("AllowDropOnPreview", AllowDropOnPreviewDefaultValue); }
			set {
				if(value == AllowDropOnPreview)
					return;
				SetBoolProperty("AllowDropOnPreview", AllowDropOnPreviewDefaultValue, value);
				Changed();
			}
		}
		[AutoFormatDisable, Category("Layout"), DefaultValue(StringResources.BinaryImage_DropZone), Localizable(true), NotifyParentProperty(true)]
		public string DropZoneText {
			get { return GetStringProperty("DropZoneText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_DropZone)); }
			set { SetStringProperty("DropZoneText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_DropZone), value); }
		}
		[AutoFormatDisable, Category("Settings"), 
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditingSettingsEnabled"),
#endif
		DefaultValue(AllowEditDefaultValue), NotifyParentProperty(true)]
		public bool Enabled {
			get { return GetBoolProperty("Enabled", AllowEditDefaultValue); }
			set {
				if(Enabled == value)
					return;
				ASPxEdit ownerEditor = Owner as ASPxEdit;
				bool requirePreserveValue = ownerEditor != null && ownerEditor.Value != null;
				object preservedValue = null;
				if(requirePreserveValue) {
					preservedValue = ownerEditor.Value;
				}
				SetBoolProperty("Enabled", AllowEditDefaultValue, value);
				if(requirePreserveValue) {
					ownerEditor.Value = preservedValue;
				}
				Changed();
			}
		}
		[AutoFormatDisable, Category("Settings"), 
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditingSettingsEmptyValueText"),
#endif
		DefaultValue(StringResources.BinaryImage_Empty), Localizable(true), NotifyParentProperty(true)]
		public string EmptyValueText {
			get { return GetStringProperty("EmptyValueText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_Empty)); }
			set { SetStringProperty("EmptyValueText", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_Empty), value); }
		}
		[AutoFormatDisable, Category("Settings"), 
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditingSettingsButtonPanelSettings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public BinaryImageButtonPanelSettings ButtonPanelSettings {
			get { return buttonPanelSettings ?? (buttonPanelSettings = new BinaryImageButtonPanelSettings(this.Owner)); }
		}
		[AutoFormatDisable, Category("Settings"), 
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditingSettingsUploadSettings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public BinaryImageUploadSettings UploadSettings {
			get {
				return binaryImageUploadSettings ?? (binaryImageUploadSettings = new BinaryImageUploadSettings(this.Owner));
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as BinaryImageEditingSettings;
			if(src != null) {
				AllowDropOnPreview = src.AllowDropOnPreview;
				DropZoneText = src.DropZoneText;
				Enabled = src.Enabled;
				EmptyValueText = src.EmptyValueText;
				ButtonPanelSettings.Assign(src.ButtonPanelSettings);
				UploadSettings.Assign(src.UploadSettings);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new List<IStateManager>(base.GetStateManagedObjects()) {
				ButtonPanelSettings,
				UploadSettings,
			}.ToArray();
		}
	}
	public class BinaryImageUploadSettings : PropertiesBase {
		protected internal const int DefaultTempFileExpirationTime = 20;
		public BinaryImageUploadSettings(IPropertiesOwner owner) : base(owner) { }
		private BinaryImageUploadValidationSettings uploadValidationSettings;
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageUploadSettingsTemporaryFolder"),
#endif
		DefaultValue(ASPxUploadControl.DefaultTemporaryFolder), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public string TemporaryFolder {
			get { return GetStringProperty("TemporaryFolder", ASPxUploadControl.DefaultTemporaryFolder); }
			set { SetStringProperty("TemporaryFolder", ASPxUploadControl.DefaultTemporaryFolder, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageUploadSettingsTempFileExpirationTime"),
#endif
 NotifyParentProperty(true),
		DefaultValue(DefaultTempFileExpirationTime), AutoFormatDisable, Localizable(false)]
		public int TempFileExpirationTime {
			get { return GetIntProperty("TempFileExpirationTime", DefaultTempFileExpirationTime); }
			set { SetIntProperty("TempFileExpirationTime", DefaultTempFileExpirationTime, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageUploadSettingsUploadValidationSettings"),
#endif
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), Localizable(false), NotifyParentProperty(true)]
		public BinaryImageUploadValidationSettings UploadValidationSettings {
			get { return uploadValidationSettings ?? (uploadValidationSettings = new BinaryImageUploadValidationSettings(Owner)); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageUploadSettingsUploadMode"),
#endif
		 AutoFormatDisable, DefaultValue(UploadControlUploadMode.Auto), NotifyParentProperty(true)]
		public UploadControlUploadMode UploadMode {
			get { return (UploadControlUploadMode)GetEnumProperty("UploadMode", UploadControlUploadMode.Auto); }
			set { SetEnumProperty("UploadMode", UploadControlUploadMode.Auto, value); }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as BinaryImageUploadSettings;
			if(src != null) {
				TemporaryFolder = src.TemporaryFolder;
				TempFileExpirationTime = src.TempFileExpirationTime;
				UploadMode = src.UploadMode;
				UploadValidationSettings.Assign(src.UploadValidationSettings);
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			var stateManagers = new []{ UploadValidationSettings };
			return base.GetStateManagedObjects().Union(stateManagers).ToArray();
		}
	}
	public class BinaryImageEditProperties : EditProperties, IImageEditProperties, IImageExportSettings {
		const int DefaultExportWidth = 60;
		const int DefaultExportHeight = 40;
		BinaryImageEditingSettings editingSettings;
		ExportBinaryImageSettings exportImageSettings;
		public BinaryImageEditProperties()
			: base() { }
		public BinaryImageEditProperties(IPropertiesOwner owner)
			: base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesBinaryStorageMode"),
#endif
		 Category("Behavior"), DefaultValue(BinaryStorageMode.Default), AutoFormatDisable, NotifyParentProperty(true)]
		public BinaryStorageMode BinaryStorageMode {
			get { return (BinaryStorageMode)GetEnumProperty("BinaryStorageMode", BinaryStorageMode.Default); }
			set { SetEnumProperty("BinaryStorageMode", BinaryStorageMode.Default, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesStoreContentBytesInViewState"),
#endif
		 Category("Behavior"), DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreContentBytesInViewState {
			get { return GetBoolProperty("StoreContentBytesInViewState", false); }
			set { SetBoolProperty("StoreContentBytesInViewState", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesEnableServerResize"),
#endif
		 DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool EnableServerResize {
			get { return GetBoolProperty("EnableServerResize", false); }
			set {
				SetBoolProperty("EnableServerResize", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesImageSizeMode"),
#endif
		 DefaultValue(ImageSizeMode.ActualSizeOrFit), AutoFormatDisable, NotifyParentProperty(true)]
		public ImageSizeMode ImageSizeMode {
			get { return (ImageSizeMode)GetEnumProperty("ImageSizeMode", ImageSizeMode.ActualSizeOrFit); }
			set {
				SetEnumProperty("ImageSizeMode", ImageSizeMode.ActualSizeOrFit, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesExportImageSettings"),
#endif
		Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ExportBinaryImageSettings ExportImageSettings {
			get {
				if(exportImageSettings == null)
					exportImageSettings = new ExportBinaryImageSettings(this);
				return exportImageSettings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesAlternateText"),
#endif
		Category("Accessibility"), DefaultValue(""), AutoFormatDisable, Localizable(true), NotifyParentProperty(true)]
		public string AlternateText {
			get { return GetStringProperty("AlternateText", ""); }
			set { SetStringProperty("AlternateText", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesAlternateTextField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string AlternateTextField {
			get { return GetStringProperty("AlternateTextField", ""); }
			set { SetStringProperty("AlternateTextField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesAlternateTextFormatString"),
#endif
		DefaultValue("{0}"), Localizable(true), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string AlternateTextFormatString {
			get { return GetStringProperty("AlternateTextFormatString", "{0}"); }
			set { SetStringProperty("AlternateTextFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesDescriptionUrl"),
#endif
		Category("Accessibility"), DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string DescriptionUrl {
			get { return GetStringProperty("DescriptionUrl", ""); }
			set { SetStringProperty("DescriptionUrl", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesDescriptionUrlField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string DescriptionUrlField {
			get { return GetStringProperty("DescriptionUrlField", ""); }
			set { SetStringProperty("DescriptionUrlField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesDescriptionUrlFormatString"),
#endif
		DefaultValue("{0}"), Localizable(true), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string DescriptionUrlFormatString {
			get { return GetStringProperty("DescriptionUrlFormatString", "{0}"); }
			set { SetStringProperty("DescriptionUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesEmptyImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties EmptyImage {
			get { return Images.ImageEmpty; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesImageAlign"),
#endif
		Category("Layout"), DefaultValue(ImageAlign.NotSet), NotifyParentProperty(true), AutoFormatEnable]
		public ImageAlign ImageAlign {
			get { return (ImageAlign)GetEnumProperty("ImageAlign", ImageAlign.NotSet); }
			set { SetEnumProperty("ImageAlign", ImageAlign.NotSet, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesImageHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit ImageHeight {
			get { return GetUnitProperty("ImageHeight", Unit.Empty); }
			set { SetUnitProperty("ImageHeight", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesImageUrlFormatString"),
#endif
		DefaultValue("{0}"), AutoFormatDisable, NotifyParentProperty(true),
		Localizable(false)]
		public string ImageUrlFormatString {
			get { return GetStringProperty("ImageUrlFormatString", "{0}"); }
			set { SetStringProperty("ImageUrlFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesImageWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		public Unit ImageWidth {
			get { return GetUnitProperty("ImageWidth", Unit.Empty); }
			set { SetUnitProperty("ImageWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesIsPng"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true), 
		Obsolete("This property was only required for old browsers (such as IE6), which are not supported now.")]
		public bool IsPng {
			get { return GetBoolProperty("IsPng", false); }
			set { SetBoolProperty("IsPng", false, value); }
		}
		[
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		string IImageEditProperties.SpriteCssClass {
			get { return GetStringProperty("SpriteCssClass", ""); }
			set { SetStringProperty("SpriteCssClass", "", value); }
		}
		[
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		Unit IImageEditProperties.SpriteLeft {
			get { return GetUnitProperty("SpriteLeft", Unit.Empty); }
			set { SetUnitProperty("SpriteLeft", Unit.Empty, value); }
		}
		[
		DefaultValue(typeof(Unit), ""), AutoFormatDisable, NotifyParentProperty(true)]
		Unit IImageEditProperties.SpriteTop {
			get { return GetUnitProperty("SpriteTop", Unit.Empty); }
			set { SetUnitProperty("SpriteTop", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesToolTip"),
#endif
		Category("Accessibility"), DefaultValue(""), AutoFormatDisable, Localizable(true), NotifyParentProperty(true)]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", ""); }
			set { SetStringProperty("ToolTip", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesToolTipField"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ToolTipField {
			get { return GetStringProperty("ToolTipField", ""); }
			set { SetStringProperty("ToolTipField", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesToolTipFormatString"),
#endif
		DefaultValue("{0}"), Localizable(true), AutoFormatDisable, NotifyParentProperty(true),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public string ToolTipFormatString {
			get { return GetStringProperty("ToolTipFormatString", "{0}"); }
			set { SetStringProperty("ToolTipFormatString", "{0}", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesShowLoadingImage"),
#endif
		Category("Layout"), DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool ShowLoadingImage {
			get { return GetBoolProperty("ShowLoadingImage", false); }
			set { SetBoolProperty("ShowLoadingImage", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesLoadingImageUrl"),
#endif
		Category("Layout"), DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string LoadingImageUrl {
			get { return GetStringProperty("LoadingImageUrl", ""); }
			set { SetStringProperty("LoadingImageUrl", "", value); }
		}
		public new BinaryImageClientSideEvents ClientSideEvents {
			get { return (BinaryImageClientSideEvents)base.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesDeleteButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public BinaryImageDeleteButtonImageProperties DeleteButtonImage {
			get {
				return Images.BinaryImageDeleteButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesOpenDialogButtonImage"),
#endif
		Category("Images"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public BinaryImageOpenDialogButtonImageProperties OpenDialogButtonImage {
			get {
				return Images.BinaryImageOpenDialogButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesButtonPanelStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BinaryImageButtonPanelStyle ButtonPanelStyle {
			get { return Styles.BinaryImageButtonPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesButtonStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonStyle ButtonStyle {
			get { return Styles.BinaryImageButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesDropZoneStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle DropZoneStyle {
			get { return Styles.BinaryImageDropZone; }
		}
		[NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle EmptyValueTextStyle {
			get { return Styles.BinaryImageEmptyValueText; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesProgressBarStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProgressBarStyle ProgressBarStyle {
			get { return Styles.ProgressBar; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("BinaryImageEditPropertiesProgressBarIndicatorStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProgressBarIndicatorStyle ProgressBarIndicatorStyle {
			get { return Styles.ProgressBarIndicator; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ReadOnlyStyle ReadOnlyStyle { get { return base.ReadOnlyStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorDecorationStyle FocusedStyle { get { return base.FocusedStyle; } }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				BinaryImageEditProperties src = source as BinaryImageEditProperties;
				if(src != null) {
					AlternateText = src.AlternateText;
					AlternateTextField = src.AlternateTextField;
					AlternateTextFormatString = src.AlternateTextFormatString;
					BinaryStorageMode = src.BinaryStorageMode;
					ClientSideEvents.Assign(src.ClientSideEvents);
					DescriptionUrl = src.DescriptionUrl;
					DescriptionUrlField = src.DescriptionUrlField;
					DescriptionUrlFormatString = src.DescriptionUrlFormatString;
					EditingSettings.Assign(src.EditingSettings);
					EnableServerResize = src.EnableServerResize;
					ExportImageSettings.Assign(src.ExportImageSettings);
					ImageAlign = src.ImageAlign;
					ImageHeight = src.ImageHeight;
					ImageSizeMode = src.ImageSizeMode;
					ImageUrlFormatString = src.ImageUrlFormatString;
					ImageWidth = src.ImageWidth;
					LoadingImageUrl = src.LoadingImageUrl;
					ShowLoadingImage = src.ShowLoadingImage;
					StoreContentBytesInViewState = src.StoreContentBytesInViewState;
					ToolTip = src.ToolTip;
					ToolTipField = src.ToolTipField;
					ToolTipFormatString = src.ToolTipFormatString;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override ASPxEditBase CreateEditInstance() { return new ASPxBinaryImage(); }
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			if(args.DisplayText != null || CommonUtils.IsNullValue(args.EditValue) && EmptyImage.IsEmpty)
				return base.CreateDisplayControlInstance(args);
			return new BinaryImageDisplayControl(this, args.EditValue as byte[], args.ValueProvider);
		}
		[AutoFormatDisable, Category("Settings"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public BinaryImageEditingSettings EditingSettings {
			get { return editingSettings ?? (editingSettings = new BinaryImageEditingSettings(this.Owner)); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("EditPropertiesValidationSettings"),
#endif
		Category("Validation"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new BinaryImageValidationSettings ValidationSettings {
			get { return (BinaryImageValidationSettings)base.ValidationSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableFocusedStyle {
			get { return base.EnableFocusedStyle; }
			set { base.EnableFocusedStyle = value; }
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new BinaryImageClientSideEvents();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new List<IStateManager>(base.GetStateManagedObjects()) {
				EditingSettings
			}.ToArray();
		}
		protected internal int GetExportHeight() {
			if(ExportImageSettings.Height > 0)
				return ExportImageSettings.Height;
			if(ImageHeight.Type == UnitType.Pixel && ImageHeight.Value > 0)
				return (int)ImageHeight.Value;
			return DefaultExportHeight;
		}
		protected internal int GetExportWidth() {
			if(ExportImageSettings.Width > 0)
				return ExportImageSettings.Width;
			if(ImageWidth.Type == UnitType.Pixel && ImageWidth.Value > 0)
				return (int)ImageWidth.Value;
			return DefaultExportWidth;
		}
		int IImageExportSettings.Width { get { return GetExportWidth(); } }
		int IImageExportSettings.Height { get { return GetExportHeight(); } }
		XtraPrinting.ImageSizeMode IImageExportSettings.SizeMode { get { return ExportImageSettings.SizeMode; } }
		bool IImageEditProperties.ApplyImageAlignToDisplayControl { get { return !EditingSettings.Enabled; } }
		public override EditorType GetEditorType() { return EditorType.Blob; }
	}
	public class BinaryImageButtonPanelSettings : PropertiesBase {
		public BinaryImageButtonPanelSettings(IPropertiesOwner owner)
			: base(owner) { }
		[
		NotifyParentProperty(true), DefaultValue(BinaryImageButtonPanelPosition.Bottom), AutoFormatEnable]
		public BinaryImageButtonPanelPosition Position {
			get { return (BinaryImageButtonPanelPosition)GetEnumProperty("Position", BinaryImageButtonPanelPosition.Bottom); }
			set {
				if(Position == value) return;
				SetEnumProperty("Position", BinaryImageButtonPanelPosition.Bottom, value);
				Changed();
			}
		}
		[
		NotifyParentProperty(true), DefaultValue(BinaryImageButtonPosition.Center), AutoFormatEnable]
		public BinaryImageButtonPosition ButtonPosition {
			get { return (BinaryImageButtonPosition)GetEnumProperty("ButtonPosition", BinaryImageButtonPosition.Center); }
			set {
				if(ButtonPosition == value) return;
				SetEnumProperty("ButtonPosition", BinaryImageButtonPosition.Center, value);
				Changed();
			}
		}
		[
		NotifyParentProperty(true), DefaultValue(ElementVisibilityMode.Faded), AutoFormatEnable]
		public ElementVisibilityMode Visibility {
			get { return (ElementVisibilityMode)GetEnumProperty("Visibility", ElementVisibilityMode.Faded); }
			set {
				if(Visibility == value) return;
				SetEnumProperty("Visibility", ElementVisibilityMode.Faded, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				BinaryImageButtonPanelSettings src = source as BinaryImageButtonPanelSettings;
				if(src != null) {
					ButtonPosition = src.ButtonPosition;
					Position = src.Position;
					Visibility = src.Visibility;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class BinaryImageUploadValidationSettings : UploadControlValidationSettings {
		static readonly string[] DefaultAllowedFileExtensions =  { ".jpg",".jpeg",".gif",".png" };
		public BinaryImageUploadValidationSettings(ASPxUploadControl uploadControl)
			: base(uploadControl) {
		}
		protected internal BinaryImageUploadValidationSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[AutoFormatDisable, Localizable(false),
		NotifyParentProperty(true), TypeConverter(typeof(StringListConverter))]
		public override string[] AllowedFileExtensions {
			get { return (string[])GetObjectProperty("AllowedFileExtensions", GetDefaultAllowedFileExtensions()); }
			set {
				SetObjectProperty("AllowedFileExtensions", GetDefaultAllowedFileExtensions(), value);
				Changed();
			}
		}
		protected override string[] GetDefaultAllowedFileExtensions() { return DefaultAllowedFileExtensions; }
	}
}
