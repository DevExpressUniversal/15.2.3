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
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Web.UI;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.ASPxSpellChecker;
using DevExpress.Web.Internal;
using System;
using System.Linq;
using System.Collections.Generic;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public class ASPxHtmlEditorDefaulAllowedExtensions {
		public static readonly string[] Images = new string[] { ".jpe", ".jpeg", ".jpg", ".gif", ".png" };
		public static readonly string[] Documents = new string[] { ".rtf", ".pdf", ".doc", ".docx", ".odt", ".txt", ".xls", 
			".xlsx", ".ods", ".ppt", ".pptx", ".odp" };
		public static readonly string[] Flash = new string[] { ".swf" };
		public static readonly string[] Video = new string[] { ".mp4", ".ogg" };
		public static readonly string[] Audio = new string[] { ".mp3", ".ogg" };
	}
}
namespace DevExpress.Web.ASPxHtmlEditor {
	public class ASPxHtmlEditorSettingsBase : PropertiesBase, IPropertiesOwner {
		public ASPxHtmlEditorSettingsBase(IPropertiesOwner owner)
			: base(owner) {
		}
		protected ASPxHtmlEditor HtmlEditor {
			get { return (ASPxHtmlEditor)Owner; }
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
		protected void MakeHtmlDirty() {
			if(Owner != null) {
				ASPxHtmlEditor htmlEditor = (ASPxHtmlEditor)Owner;
				htmlEditor.Document.RequireHtmlCorrection();
			}
		}
	}
	public class ASPxHtmlEditorSettings : ASPxHtmlEditorSettingsBase {
		public ASPxHtmlEditorSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		HtmlEditorHtmlViewSettings settingsHtmlView;
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsAllowContextMenu"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(DefaultBoolean.True), NotifyParentProperty(true)]
		public DefaultBoolean AllowContextMenu {
			get { return GetDefaultBooleanProperty("AllowContextMenu", DefaultBoolean.True); }
			set {
				SetDefaultBooleanProperty("AllowContextMenu", DefaultBoolean.True, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsAllowInsertDirectImageUrls"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowInsertDirectImageUrls {
			get { return GetBoolProperty("AllowInsertDirectImageUrls", true); }
			set { SetBoolProperty("AllowInsertDirectImageUrls", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsAllowDesignView"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowDesignView {
			get { return GetBoolProperty("AllowDesignView", true); }
			set {
				SetBoolProperty("AllowDesignView", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsAllowHtmlView"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowHtmlView {
			get { return GetBoolProperty("AllowHtmlView", true); }
			set {
				SetBoolProperty("AllowHtmlView", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsAllowPreview"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(true), NotifyParentProperty(true)]
		public bool AllowPreview {
			get { return GetBoolProperty("AllowPreview", true); }
			set {
				SetBoolProperty("AllowPreview", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSettingsAllowCustomColorsInColorPickers"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowCustomColorsInColorPickers {
			get { return GetBoolProperty("AllowCustomColorsInColorPickers", false); }
			set { SetBoolProperty("AllowCustomColorsInColorPickers", false, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowScriptExecutionInPreview {
			get { return GetBoolProperty("AllowScriptExecutionInPreview", false); }
			set { SetBoolProperty("AllowScriptExecutionInPreview", false, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowTagInspector {
			get { return GetBoolProperty("ShowTagInspector", false); }
			set { SetBoolProperty("ShowTagInspector", false, value); }
		}
		[Category("Behavior"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public HtmlEditorHtmlViewSettings SettingsHtmlView {
			get {
				if(settingsHtmlView == null)
					settingsHtmlView = new HtmlEditorHtmlViewSettings(Owner);
				return settingsHtmlView;
			}
		}
		internal bool AllowDesignViewInternal {
			get {
				return (!AllowDesignView && !AllowHtmlView && !AllowPreview) ? true : AllowDesignView;
			}
		}
		internal bool AllowHtmlViewInternal {
			get { return AllowHtmlView; }
		}
		internal bool AllowPreviewInternal {
			get { return AllowPreview; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxHtmlEditorSettings src = source as ASPxHtmlEditorSettings;
				if(src != null) {
					AllowContextMenu = src.AllowContextMenu;
					AllowInsertDirectImageUrls = src.AllowInsertDirectImageUrls;
					AllowDesignView = src.AllowDesignView;
					AllowHtmlView = src.AllowHtmlView;
					AllowPreview = src.AllowPreview;
					AllowCustomColorsInColorPickers = src.AllowCustomColorsInColorPickers;
					AllowScriptExecutionInPreview = src.AllowScriptExecutionInPreview;
					ShowTagInspector = src.ShowTagInspector;
					SettingsHtmlView.Assign(src.SettingsHtmlView);
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { SettingsHtmlView };
		}
	}
	public enum HtmlEditorEnterMode { 
		Default = 0, 
		BR = 1, 
		P = 2
	}
	public enum AllowedDocumentType { 
		XHTML = 0, 
		HTML5 = 1, 
		Both = 2 
	}
	public enum HtmlEditorPasteMode {
		SourceFormatting = 0,
		MergeFormatting = 1,
		PlainText = 2
	}
	public enum HtmlEditorFilterMode {
		BlackList = 0,
		WhiteList = 1
	}
	public enum ResourcePathMode { 
		Absolute = 0, 
		Relative = 1, 
		RootRelative = 2, 
		NotSet = 3
	}
	public enum HtmlEditorHtmlEditingMode {
		Simple = 0,
		Advanced = 1,
		Auto = 2
	}
	public class HtmlEditorContentElementFiltering : ASPxHtmlEditorSettingsBase {
		public HtmlEditorContentElementFiltering(IPropertiesOwner owner)
			: base(owner) {
		}
		[AutoFormatDisable, NotifyParentProperty(true), TypeConverter(typeof(StringListConverter))]
		public string[] Tags {
			get { return (string[])GetObjectProperty("Tags", GetDefaultElements()); }
			set {
				SetObjectProperty("Tags", GetDefaultElements(), value);
				MakeHtmlDirty();
			}
		}
		[AutoFormatDisable, DefaultValue(HtmlEditorFilterMode.BlackList), NotifyParentProperty(true)]
		public HtmlEditorFilterMode TagFilterMode {
			get { return (HtmlEditorFilterMode)GetEnumProperty("TagFilterMode", HtmlEditorFilterMode.BlackList); }
			set { SetEnumProperty("TagFilterMode", HtmlEditorFilterMode.BlackList, value); }
		}
		[AutoFormatDisable, NotifyParentProperty(true), TypeConverter(typeof(StringListConverter))]
		public string[] Attributes {
			get { return (string[])GetObjectProperty("Attributes", GetDefaultElements()); }
			set {
				SetObjectProperty("Attributes", GetDefaultElements(), value);
				MakeHtmlDirty();
			}
		}
		[AutoFormatDisable, DefaultValue(HtmlEditorFilterMode.BlackList), NotifyParentProperty(true)]
		public HtmlEditorFilterMode AttributeFilterMode {
			get { return (HtmlEditorFilterMode)GetEnumProperty("AttributeFilterMode", HtmlEditorFilterMode.BlackList); }
			set { SetEnumProperty("AttributeFilterMode", HtmlEditorFilterMode.BlackList, value); }
		}
		[AutoFormatDisable, NotifyParentProperty(true), TypeConverter(typeof(StringListConverter))]
		public string[] StyleAttributes {
			get { return (string[])GetObjectProperty("StyleAttributes", GetDefaultElements()); }
			set {
				SetObjectProperty("StyleAttributes", GetDefaultElements(), value);
				MakeHtmlDirty();
			}
		}
		[AutoFormatDisable, DefaultValue(HtmlEditorFilterMode.BlackList), NotifyParentProperty(true)]
		public HtmlEditorFilterMode StyleAttributeFilterMode {
			get { return (HtmlEditorFilterMode)GetEnumProperty("StyleAttributeFilterMode", HtmlEditorFilterMode.BlackList); }
			set { SetEnumProperty("StyleAttributeFilterMode", HtmlEditorFilterMode.BlackList, value); }
		}
		protected virtual string[] GetDefaultElements() {
			return new string[0];
		}
		public override string ToString() {
			return string.Empty;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				HtmlEditorContentElementFiltering src = source as HtmlEditorContentElementFiltering;
				if(src != null) {
					Tags = src.Tags;
					TagFilterMode = src.TagFilterMode;
					Attributes = src.Attributes;
					AttributeFilterMode = src.AttributeFilterMode;
					StyleAttributes = src.StyleAttributes;
					StyleAttributeFilterMode = src.StyleAttributeFilterMode;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class ASPxHtmlEditorHtmlEditingSettings : HtmlEditorHtmlEditingSettings, IHtmlProcessingSettings {
		public ASPxHtmlEditorHtmlEditingSettings()
			: this(null) {
		}
		public ASPxHtmlEditorHtmlEditingSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorHtmlEditingSettingsEnterMode"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(HtmlEditorEnterMode.P), NotifyParentProperty(true)]
		public HtmlEditorEnterMode EnterMode {
			get { return (HtmlEditorEnterMode)GetEnumProperty("EnterMode", HtmlEditorEnterMode.P); }
			set { SetEnumProperty("EnterMode", HtmlEditorEnterMode.P, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true),
		Obsolete("Use the ResourcePathMode property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool UseAbsoluteResourcePaths {
			get { return ResourcePathMode == ResourcePathMode.Absolute; }
			set {
				if(value) 
					ResourcePathMode = ResourcePathMode.Absolute; 
			}
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(HtmlEditorPasteMode.SourceFormatting), NotifyParentProperty(true)]
		public HtmlEditorPasteMode PasteMode {
			get { return (HtmlEditorPasteMode)GetEnumProperty("PasteMode", HtmlEditorPasteMode.SourceFormatting); }
			set { SetEnumProperty("PasteMode", HtmlEditorPasteMode.SourceFormatting, value); }
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool EnablePasteOptions {
			get { return GetBoolProperty("EnablePasteOptions", false); }
			set { SetBoolProperty("EnablePasteOptions", false, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxHtmlEditorHtmlEditingSettings src = source as ASPxHtmlEditorHtmlEditingSettings;
				if(src != null) {
					PasteMode = src.PasteMode;
					EnablePasteOptions = src.EnablePasteOptions;
					EnterMode = src.EnterMode;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class ASPxHtmlEditorLoadingPanelSettings : SettingsLoadingPanel {
		public ASPxHtmlEditorLoadingPanelSettings(ASPxHtmlEditor editor)
			: base(editor) {
		}
	}
	public class ASPxHtmlEditorUploadValidationSettingsBase : UploadControlValidationSettings {
		const long DefaultFileSize = 30 * 1024 * 1024;
		protected ASPxHtmlEditorUploadSettingsBase ParentSettings { get; private set; }
		public ASPxHtmlEditorUploadValidationSettingsBase(IPropertiesOwner owner, ASPxHtmlEditorUploadSettingsBase parentSettings)
			: base(owner) {
				ParentSettings = parentSettings;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ShowErrors {
			get { return base.ShowErrors; }
			set { }
		}
		public override string[] AllowedFileExtensions {
			get { return ParentSettings.GetSelectorSettings().CommonSettings.AllowedFileExtensions; }
			set { ParentSettings.GetSelectorSettings().CommonSettings.AllowedFileExtensions = value; }
		}
		protected new bool ShouldSerializeAllowedFileExtensions() {
			return !System.Linq.Enumerable.SequenceEqual(AllowedFileExtensions, GetDefaultAllowedFileExtensions());
		}
		protected new void ResetAllowedFileExtensions() {
			AllowedFileExtensions = GetDefaultAllowedFileExtensions();
		}
		[
		DefaultValue(StringResources.HtmlEditorText_InvalidUrl), AutoFormatDisable, Localizable(true),
		NotifyParentProperty(true)]
		public string InvalidUrlErrorText {
			get { return GetStringProperty("InvalidUrlErrorText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InvalidUrlErrorText)); }
			set { SetStringProperty("InvalidUrlErrorText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.InvalidUrlErrorText), value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorUploadValidationSettingsBaseMaxFileSize"),
#endif
		DefaultValue(DefaultFileSize), AutoFormatDisable, Localizable(false),
		NotifyParentProperty(true)]
		public override long MaxFileSize {
			get { return base.MaxFileSize; }
			set { base.MaxFileSize = value; }
		}
		protected override long GetMaxFileSizeDefaultValue() {
			return DefaultFileSize;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is ASPxHtmlEditorUploadValidationSettingsBase) {
					ASPxHtmlEditorUploadValidationSettingsBase src = source as ASPxHtmlEditorUploadValidationSettingsBase;
					InvalidUrlErrorText = src.InvalidUrlErrorText;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ASPxHtmlEditorImageUploadValidationSettings : ASPxHtmlEditorUploadValidationSettingsBase {
		public ASPxHtmlEditorImageUploadValidationSettings(IPropertiesOwner owner, ASPxHtmlEditorImageUploadSettings parentSettings)
			: base(owner, parentSettings) {
		}
		protected override string[] GetDefaultAllowedFileExtensions() {
			return ASPxHtmlEditorDefaulAllowedExtensions.Images;
		}
	}
	public class ASPxHtmlEditorFlashUploadValidationSettings : ASPxHtmlEditorUploadValidationSettingsBase {
		public ASPxHtmlEditorFlashUploadValidationSettings(IPropertiesOwner owner, ASPxHtmlEditorFlashUploadSettings parentSettings)
			: base(owner, parentSettings) {
		}
		protected override string[] GetDefaultAllowedFileExtensions() {
			return ASPxHtmlEditorDefaulAllowedExtensions.Flash;
		}
	}
	public class ASPxHtmlEditorVideoUploadValidationSettings : ASPxHtmlEditorUploadValidationSettingsBase {
		public ASPxHtmlEditorVideoUploadValidationSettings(IPropertiesOwner owner, ASPxHtmlEditorVideoUploadSettings parentSettings)
			: base(owner, parentSettings) {
		}
		protected override string[] GetDefaultAllowedFileExtensions() {
			return ASPxHtmlEditorDefaulAllowedExtensions.Video;
		}
	}
	public class ASPxHtmlEditorAudioUploadValidationSettings : ASPxHtmlEditorUploadValidationSettingsBase {
		public ASPxHtmlEditorAudioUploadValidationSettings(IPropertiesOwner owner, ASPxHtmlEditorAudioUploadSettings parentSettings)
			: base(owner, parentSettings) {
		}
		protected override string[] GetDefaultAllowedFileExtensions() {
			return ASPxHtmlEditorDefaulAllowedExtensions.Audio;
		}
	}
	public abstract class ASPxHtmlEditorUploadSettingsBase : ASPxHtmlEditorSettingsBase {
		const string DefaultTemporaryFolder = "~\\App_Data\\UploadTemp\\";
		const int DefaultPacketSizeValue = 200000;
		protected internal ASPxHtmlEditorUploadValidationSettingsBase ValidationSettingsInternal { get; private set; }
		public ASPxHtmlEditorUploadSettingsBase(IPropertiesOwner owner)
			: base(owner) {
				ValidationSettingsInternal = CreateValidationSettings(owner);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorUploadSettingsBaseUseAdvancedUploadMode"),
#endif
Category("Behavior"), DefaultValue(true), AutoFormatDisable,
		Localizable(false), NotifyParentProperty(true)]
		public bool UseAdvancedUploadMode {
			get { return GetBoolProperty("UseAdvancedUploadMode", true); }
			set { SetBoolProperty("UseAdvancedUploadMode", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorUploadSettingsBaseUploadFolder"),
#endif
		DefaultValue("~/"), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string UploadFolder {
			get { return GetStringProperty("UploadFolder", "~/"); }
			set {
				UrlUtils.ValidateFolderUrl(ref value);
				SetStringProperty("UploadFolder", "~/", value);
			}
		}
		[DefaultValue(""), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string UploadFolderUrlPath {
			get { return GetStringProperty("UploadFolderUrlPath", ""); }
			set {
				if(value != UploadFolderUrlPath)
					SetStringProperty("UploadFolderUrlPath", "", value);
			}
		}
		[DefaultValue(DefaultTemporaryFolder), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string AdvancedUploadModeTemporaryFolder {
			get { return GetStringProperty("AdvancedUploadModeTemporaryFolder", DefaultTemporaryFolder); }
			set { SetStringProperty("AdvancedUploadModeTemporaryFolder", DefaultTemporaryFolder, value); }
		}
		[DefaultValue(DefaultPacketSizeValue), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public int AdvancedUploadModePacketSize {
			get { return GetIntProperty("AdvancedUploadModePacketSize", DefaultPacketSizeValue); }
			set {
				CommonUtils.CheckValueRange(value, 1, int.MaxValue, "AdvancedUploadModePacketSize");
				SetIntProperty("AdvancedUploadModePacketSize", DefaultPacketSizeValue, value);
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxHtmlEditorUploadSettingsBase src = source as ASPxHtmlEditorUploadSettingsBase;
				if(src != null) {
					ValidationSettingsInternal.Assign(src.ValidationSettingsInternal);
					UseAdvancedUploadMode = src.UseAdvancedUploadMode;
					UploadFolder = src.UploadFolder;
					AdvancedUploadModeTemporaryFolder = src.AdvancedUploadModeTemporaryFolder;
					AdvancedUploadModePacketSize = src.AdvancedUploadModePacketSize;
					UploadFolderUrlPath = src.UploadFolderUrlPath;
				}
			} finally {
				EndUpdate();
			}
		}
		protected abstract ASPxHtmlEditorUploadValidationSettingsBase CreateValidationSettings(IPropertiesOwner owner);
		protected internal abstract HtmlEditorSelectorSettings GetSelectorSettings();
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { ValidationSettingsInternal };
		}
	}
	public class ASPxHtmlEditorImageUploadSettings : ASPxHtmlEditorUploadSettingsBase {
		public ASPxHtmlEditorImageUploadSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ASPxHtmlEditorUploadValidationSettingsBase CreateValidationSettings(IPropertiesOwner owner) {
			return new ASPxHtmlEditorImageUploadValidationSettings(owner, this);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageUploadSettingsValidationSettings"),
#endif
		Category("Validation"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(false),
		NotifyParentProperty(true)]
		public ASPxHtmlEditorImageUploadValidationSettings ValidationSettings {
			get { return ValidationSettingsInternal as ASPxHtmlEditorImageUploadValidationSettings; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageUploadSettingsUploadImageFolder"),
#endif
		DefaultValue("~/"), AutoFormatDisable, Localizable(false), NotifyParentProperty(true),
		Obsolete("Use the UploadFolder property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string UploadImageFolder {
			get { return UploadFolder; }
			set { UploadFolder = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorImageUploadSettingsUseAdvancedUploadMode"),
#endif
Category("Behavior"), DefaultValue(true), AutoFormatDisable,
		Localizable(false), NotifyParentProperty(true)]
		public new bool UseAdvancedUploadMode {
			get { return base.UseAdvancedUploadMode; }
			set { base.UseAdvancedUploadMode = value; }
		}
		protected internal override HtmlEditorSelectorSettings GetSelectorSettings() {
			return (Owner as ASPxHtmlEditor).SettingsDialogs.InsertImageDialog.SettingsImageSelector;
		}
	}
	public class ASPxHtmlEditorFlashUploadSettings : ASPxHtmlEditorUploadSettingsBase {
		public ASPxHtmlEditorFlashUploadSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ASPxHtmlEditorUploadValidationSettingsBase CreateValidationSettings(IPropertiesOwner owner) {
			return new ASPxHtmlEditorFlashUploadValidationSettings(owner, this);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorFlashUploadSettingsValidationSettings"),
#endif
		Category("Validation"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(false),
		NotifyParentProperty(true)]
		public ASPxHtmlEditorFlashUploadValidationSettings ValidationSettings {
			get { return ValidationSettingsInternal as ASPxHtmlEditorFlashUploadValidationSettings; }
		}
		protected internal override HtmlEditorSelectorSettings GetSelectorSettings() {
			return (Owner as ASPxHtmlEditor).SettingsDialogs.InsertFlashDialog.SettingsFlashSelector;
		}
	}
	public class ASPxHtmlEditorVideoUploadSettings : ASPxHtmlEditorUploadSettingsBase {
		public ASPxHtmlEditorVideoUploadSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ASPxHtmlEditorUploadValidationSettingsBase CreateValidationSettings(IPropertiesOwner owner) {
			return new ASPxHtmlEditorVideoUploadValidationSettings(owner, this);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorVideoUploadSettingsValidationSettings"),
#endif
		Category("Validation"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(false),
		NotifyParentProperty(true)]
		public ASPxHtmlEditorVideoUploadValidationSettings ValidationSettings {
			get { return ValidationSettingsInternal as ASPxHtmlEditorVideoUploadValidationSettings; }
		}
		protected internal override HtmlEditorSelectorSettings GetSelectorSettings() {
			return (Owner as ASPxHtmlEditor).SettingsDialogs.InsertVideoDialog.SettingsVideoSelector;
		}
	}
	public class ASPxHtmlEditorAudioUploadSettings : ASPxHtmlEditorUploadSettingsBase {
		public ASPxHtmlEditorAudioUploadSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected override ASPxHtmlEditorUploadValidationSettingsBase CreateValidationSettings(IPropertiesOwner owner) {
			return new ASPxHtmlEditorAudioUploadValidationSettings(owner, this);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorAudioUploadSettingsValidationSettings"),
#endif
		Category("Validation"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(false),
		NotifyParentProperty(true)]
		public ASPxHtmlEditorAudioUploadValidationSettings ValidationSettings {
			get { return ValidationSettingsInternal as ASPxHtmlEditorAudioUploadValidationSettings; }
		}
		protected internal override HtmlEditorSelectorSettings GetSelectorSettings() {
			return (Owner as ASPxHtmlEditor).SettingsDialogs.InsertAudioDialog.SettingsAudioSelector;
		}
	}
	public class HtmlEditorFileManagerCommonSettings : FileManagerSettings {
		const string DefaultThumbnailFolder = "~/thumb";
		public HtmlEditorFileManagerCommonSettings(IPropertiesOwner owner) : base(owner) {}
		protected override string GetDefaultThumbnailFolder() {
			return DefaultThumbnailFolder;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerCommonSettingsThumbnailFolder"),
#endif
		DefaultValue("~/thumb"), AutoFormatDisable, NotifyParentProperty(true)]
		public new string ThumbnailFolder {
			get { return base.ThumbnailFolder; }
			set { base.ThumbnailFolder = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool EnableMultiSelect {
			get { return base.EnableMultiSelect; }
			set { base.EnableMultiSelect = value; }
		}
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerCommonSettingsAllowedFileExtensions")]
#endif
		public override string[] AllowedFileExtensions {
			get { return base.AllowedFileExtensions; }
			set { base.AllowedFileExtensions = value; }
		}
		protected bool ShouldSerializeAllowedFileExtensions() {
			return !System.Linq.Enumerable.SequenceEqual(AllowedFileExtensions, GetDefaultAllowedFileExtensions());
		}
		protected void ResetAllowedFileExtensions() {
			AllowedFileExtensions = GetDefaultAllowedFileExtensions();
		}
		protected override string[] GetDefaultAllowedFileExtensions() {
			return ASPxHtmlEditorDefaulAllowedExtensions.Images;
		}
	}
	public class HtmlEditorDocumentSelectorCommonSettings : HtmlEditorFileManagerCommonSettings {		
		public HtmlEditorDocumentSelectorCommonSettings(IPropertiesOwner owner) : base(owner) { }
#if !SL
	[DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentSelectorCommonSettingsAllowedFileExtensions")]
#endif
		public override string[] AllowedFileExtensions {
			get { return base.AllowedFileExtensions; }
			set { base.AllowedFileExtensions = value; }
		}
		protected override string[] GetDefaultAllowedFileExtensions() {
			return ASPxHtmlEditorDefaulAllowedExtensions.Documents;
		}
	}
	public class HtmlEditorFlashSelectorCommonSettings : HtmlEditorFileManagerCommonSettings {
		public HtmlEditorFlashSelectorCommonSettings(IPropertiesOwner owner) : base(owner) { }
		protected override string[] GetDefaultAllowedFileExtensions() {
			return ASPxHtmlEditorDefaulAllowedExtensions.Flash;
		}
	}
	public class HtmlEditorVideoSelectorCommonSettings : HtmlEditorFileManagerCommonSettings {
		public HtmlEditorVideoSelectorCommonSettings(IPropertiesOwner owner) : base(owner) { }
		protected override string[] GetDefaultAllowedFileExtensions() {
			return ASPxHtmlEditorDefaulAllowedExtensions.Video;
		}
	}
	public class HtmlEditorAudioSelectorCommonSettings : HtmlEditorFileManagerCommonSettings {
		public HtmlEditorAudioSelectorCommonSettings(IPropertiesOwner owner) : base(owner) { }
		protected override string[] GetDefaultAllowedFileExtensions() {
			return ASPxHtmlEditorDefaulAllowedExtensions.Audio;
		}
	}
	public class HtmlEditorFileManagerEditingSettings : FileManagerSettingsEditing {
		public HtmlEditorFileManagerEditingSettings(IPropertiesOwner owner) : base(owner) {}
	}
	public class HtmlEditorFileManagerFoldersSettings : FileManagerSettingsFolders {
		public HtmlEditorFileManagerFoldersSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerFoldersSettingsShowLockedFolderIcons"),
#endif
		DefaultValue(false)]
		public override bool ShowLockedFolderIcons {
			get { return base.ShowLockedFolderIcons; }
			set { base.ShowLockedFolderIcons = value; }
		}
		protected override bool GetDefaultShowLockedFolderIcons() {
			return false;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerFoldersSettingsVisible"),
#endif
 DefaultValue(false)]
		public override bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		protected override bool GetDefaultVisible() {
			return false;
		}
	}
	public class HtmlEditorFileManagerToolbarSettings : FileManagerSettingsToolbar {
		public HtmlEditorFileManagerToolbarSettings(IPropertiesOwner owner) : base(owner) { }
		[DefaultValue(false)]
		public override bool ShowPath {
			get { return base.ShowPath; }
			set { base.ShowPath = value; }
		}
		protected override bool GetDefaultShowPath() {
			return false;
		}
	}
	public class HtmlEditorDocumentSelectorToolbarSettings : HtmlEditorFileManagerToolbarSettings {
		public HtmlEditorDocumentSelectorToolbarSettings(IPropertiesOwner owner) : base(owner) { }
		[DefaultValue(true)]
		public override bool ShowPath {
			get { return base.ShowPath; }
			set { base.ShowPath = value; }
		}
		protected override bool GetDefaultShowPath() {
			return true;
		}
	}
	public class HtmlEditorFileManagerBreadcrumbsSettings : FileManagerSettingsBreadcrumbs {
		public HtmlEditorFileManagerBreadcrumbsSettings(IPropertiesOwner owner) : base(owner) { }
		[DefaultValue(true)]
		public override bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		[DefaultValue(BreadcrumbsPosition.Bottom)]
		public override BreadcrumbsPosition Position {
			get { return base.Position; }
			set { base.Position = value; }
		}
		protected override bool GetDefaultVisible() {
			return true;
		}
		protected override BreadcrumbsPosition GetDefaultPosition() {
			return BreadcrumbsPosition.Bottom;
		}
	}
	public class HtmlEditorDocumentSelectorBreadcrumbsSettings : HtmlEditorFileManagerBreadcrumbsSettings {
		public HtmlEditorDocumentSelectorBreadcrumbsSettings(IPropertiesOwner owner) : base(owner) { }
		[DefaultValue(false)]
		public override bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		[DefaultValue(BreadcrumbsPosition.Top)]
		public override BreadcrumbsPosition Position {
			get { return base.Position; }
			set { base.Position = value; }
		}
		protected override bool GetDefaultVisible() {
			return false;
		}
		protected override BreadcrumbsPosition GetDefaultPosition() {
			return BreadcrumbsPosition.Top;
		}
	}
	public class HtmlEditorFileManagerValidationSettings : FileManagerValidationSettings {
		const long DefaultMaxFileSize = 30 * 1024 * 1024;
		public HtmlEditorFileManagerValidationSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerValidationSettingsMaxFileSize"),
#endif
		DefaultValue(DefaultMaxFileSize), AutoFormatDisable, Localizable(false),
		NotifyParentProperty(true)]
		public override long MaxFileSize {
			get { return base.MaxFileSize; }
			set { base.MaxFileSize = value; }
		}
		protected override long GetMaxFileSizeDefaultValue() {
			return DefaultMaxFileSize;
		}
	}
	public class HtmlEditorFileManagerUploadSettings : FileManagerSettingsUpload {
		public HtmlEditorFileManagerUploadSettings(IPropertiesOwner owner) : base(owner) { }
		[DefaultValue(false)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		protected override bool GetDefaultEnabled() {
			return false;
		}
		[DefaultValue(false)]
		public override bool ShowUploadPanel {
			get { return base.ShowUploadPanel; }
			set {  base.ShowUploadPanel = value; }
		}
		protected override bool GetDefaultShowUploadPanel() {
			return false;
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerUploadSettingsValidationSettings"),
#endif
		Category("Validation"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Localizable(false),
		NotifyParentProperty(true)]
		public new HtmlEditorFileManagerValidationSettings ValidationSettings {
			get {
				return ValidationSettingsInternal as HtmlEditorFileManagerValidationSettings;
			}
		}
		protected override FileManagerValidationSettings CreateValidationSettings() {
			return new HtmlEditorFileManagerValidationSettings(this);
		}
	}
	public class HtmlEditorDocumentSelectorUploadSettings : HtmlEditorFileManagerUploadSettings {
		public HtmlEditorDocumentSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
		[DefaultValue(true)]
		public override bool ShowUploadPanel {
			get { return base.ShowUploadPanel; }
			set { base.ShowUploadPanel = value; }
		}
		protected override bool GetDefaultShowUploadPanel() {
			return true;
		}
	}
	public class HtmlEditorDocumentSelectorFoldersSettings : HtmlEditorFileManagerFoldersSettings {
		public HtmlEditorDocumentSelectorFoldersSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorDocumentSelectorFoldersSettingsVisible"),
#endif
 DefaultValue(true)]
		public override bool Visible {
			get { return base.Visible; }
			set { base.Visible = value; }
		}
		protected override bool GetDefaultVisible() {
			return true;
		}
	}
	public class HtmlEditorDocumentSelectorFileListSettings : HtmlEditorFileManagerFileListSettings {
		public HtmlEditorDocumentSelectorFileListSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerFileListSettingsShowParentFolder"),
#endif
 DefaultValue(false)]
		public override bool ShowParentFolder {
			get { return base.ShowParentFolder; }
			set { base.ShowParentFolder = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerFileListSettingsShowFolders"),
#endif
 DefaultValue(false)]
		public override bool ShowFolders {
			get { return base.ShowFolders; }
			set { base.ShowFolders = value; }
		}
		protected override bool GetDefaultShowParentFolder() {
			return false;
		}
		protected override bool GetDefaultShowFolders() {
			return false;
		}
	}
	public class HtmlEditorImageSelectorUploadSettings : HtmlEditorFileManagerUploadSettings {
		public HtmlEditorImageSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
	}
	public class HtmlEditorFlashSelectorUploadSettings : HtmlEditorFileManagerUploadSettings {
		public HtmlEditorFlashSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
	}
	public class HtmlEditorVideoSelectorUploadSettings : HtmlEditorFileManagerUploadSettings {
		public HtmlEditorVideoSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
	}
	public class HtmlEditorAudioSelectorUploadSettings : HtmlEditorFileManagerUploadSettings {
		public HtmlEditorAudioSelectorUploadSettings(IPropertiesOwner owner) : base(owner) { }
	}
	public class HtmlEditorFileManagerPermissionsSettings : FileManagerSettingsPermissions {
		public HtmlEditorFileManagerPermissionsSettings(IPropertiesOwner owner) : base(owner) { }
	}
	public class HtmlEditorFileManagerSettingsBase : ASPxHtmlEditorSettingsBase {
		public HtmlEditorFileManagerSettingsBase(IPropertiesOwner owner)
			: base(owner) {
			CommonSettings = CreateCommonSettings();
			EditingSettings = CreateEditingSettings();
			FoldersSettings = CreateFoldersSettings();
			ToolbarSettings = CreateToolbarSettings();
			UploadSettings = CreateUploadSettings();
			PermissionSettings = CreatePermissionsSettings();
			FileListSettings = CreateFileListSettings();
			BreadcrumbsSettings = CreateBreadCrumbsSettings();
			SettingsAmazon = CreateAmazonSettings();
			SettingsAzure = CreateAzureSettings();
			SettingsDataSource = CreateDataSourceSettings();
			SettingsDropbox = CreateDropboxSettings();
			ClientSideEvents = new FileManagerClientSideEvents();
		}
		protected virtual FileManagerAmazonProviderSettings CreateAmazonSettings() {
			return new FileManagerAmazonProviderSettings(Owner);
		}
		protected virtual FileManagerAzureProviderSettings CreateAzureSettings() {
			return new FileManagerAzureProviderSettings(Owner);
		}
		protected virtual FileManagerSettingsDataSource CreateDataSourceSettings() {
			return new FileManagerSettingsDataSource(Owner);
		}
		protected virtual FileManagerDropBoxProviderSettings CreateDropboxSettings() {
			return new FileManagerDropBoxProviderSettings(Owner);
		}
		protected virtual HtmlEditorFileManagerCommonSettings CreateCommonSettings() {
			return new HtmlEditorFileManagerCommonSettings(Owner);
		}
		protected virtual HtmlEditorFileManagerEditingSettings CreateEditingSettings() {
			return new HtmlEditorFileManagerEditingSettings(Owner);
		}
		protected virtual HtmlEditorFileManagerFoldersSettings CreateFoldersSettings() {
			return new HtmlEditorFileManagerFoldersSettings(Owner);
		}
		protected virtual HtmlEditorFileManagerToolbarSettings CreateToolbarSettings() {
			return new HtmlEditorFileManagerToolbarSettings(Owner);
		}
		protected virtual HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new HtmlEditorFileManagerUploadSettings(Owner);
		}
		protected virtual HtmlEditorFileManagerPermissionsSettings CreatePermissionsSettings() {
			return new HtmlEditorFileManagerPermissionsSettings(Owner);
		}
		protected virtual HtmlEditorFileManagerFileListSettings CreateFileListSettings() {
			return new HtmlEditorFileManagerFileListSettings(Owner);
		}
		protected virtual HtmlEditorFileManagerBreadcrumbsSettings CreateBreadCrumbsSettings() {
			return new HtmlEditorFileManagerBreadcrumbsSettings(Owner);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerSettingsBaseEnabled"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool Enabled
		{
			get { return GetBoolProperty("Enabled", false); }
			set { SetBoolProperty("Enabled", false, value); }
		}
		[Category("Data"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public FileSystemProviderBase CustomFileSystemProvider { get; set; }
		[Category("Data"), TypeConverter("DevExpress.Web.Design.FileManagerFileSystemProviderTypeNameConverter, " + AssemblyInfo.SRAssemblyWebDesignFull),
		DefaultValue(""), AutoFormatDisable, Localizable(false), NotifyParentProperty(true)]
		public string CustomFileSystemProviderTypeName {
			get { return GetStringProperty("CustomFileSystemProviderTypeName", string.Empty); }
			set { SetStringProperty("CustomFileSystemProviderTypeName", string.Empty, value); }
		}
		[DefaultValue(FileManagerProviderType.NotSet), Category("Settings"), AutoFormatDisable, NotifyParentProperty(true)]
		public FileManagerProviderType ProviderType {
			get { return (FileManagerProviderType)GetEnumProperty("ProviderType", FileManagerProviderType.NotSet); }
			set {
				if(ProviderType == value)
					return;
				SetEnumProperty("ProviderType", FileManagerProviderType.NotSet, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerSettingsBaseCommonSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerCommonSettings CommonSettings { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerSettingsBaseEditingSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerEditingSettings EditingSettings { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerSettingsBaseFoldersSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerFoldersSettings FoldersSettings { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerSettingsBaseToolbarSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerToolbarSettings ToolbarSettings { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerSettingsBaseUploadSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerUploadSettings UploadSettings { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerSettingsBasePermissionSettings"),
#endif
AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerPermissionsSettings PermissionSettings { get; private set; }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerSettingsBaseFileListSettings"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerFileListSettings FileListSettings { get; private set; }
		[
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorFileManagerBreadcrumbsSettings BreadcrumbsSettings { get; private set; }
		[AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerClientSideEvents ClientSideEvents { get; private set; }
		[AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerAmazonProviderSettings SettingsAmazon { get; private set; }
		[AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerAzureProviderSettings SettingsAzure { get; private set; }
		[AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerSettingsDataSource SettingsDataSource { get; private set; }
		[AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FileManagerDropBoxProviderSettings SettingsDropbox { get; private set; }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				HtmlEditorFileManagerSettingsBase src = source as HtmlEditorFileManagerSettingsBase;
				if(src != null) {
					Enabled = src.Enabled;
					CommonSettings.Assign(src.CommonSettings);
					EditingSettings.Assign(src.EditingSettings);
					FoldersSettings.Assign(src.FoldersSettings);
					ToolbarSettings.Assign(src.ToolbarSettings);
					UploadSettings.Assign(src.UploadSettings);
					PermissionSettings.Assign(src.PermissionSettings);
					FileListSettings.Assign(src.FileListSettings);
					BreadcrumbsSettings.Assign(src.BreadcrumbsSettings);
					SettingsAmazon.Assign(src.SettingsAmazon);
					SettingsAzure.Assign(src.SettingsAzure);
					SettingsDataSource.Assign(src.SettingsDataSource);
					SettingsDropbox.Assign(src.SettingsDropbox);
					ClientSideEvents.Assign(src.ClientSideEvents);
					CustomFileSystemProvider = src.CustomFileSystemProvider;
					ProviderType = src.ProviderType;
					CustomFileSystemProviderTypeName = src.CustomFileSystemProviderTypeName;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { CommonSettings, EditingSettings, FoldersSettings, ToolbarSettings, 
				UploadSettings, PermissionSettings, FileListSettings, BreadcrumbsSettings, SettingsDataSource, ClientSideEvents };
		}
	}
	public class HtmlEditorSelectorSettings : HtmlEditorFileManagerSettingsBase {
		public HtmlEditorSelectorSettings(IPropertiesOwner owner) : base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorSelectorSettingsRootFolderUrlPath"),
#endif
		Localizable(false), AutoFormatDisable, DefaultValue(""), NotifyParentProperty(true)]
		public string RootFolderUrlPath
		{
			get
			{
				return GetStringProperty("RootFolderUrlPath", "");
			}
			set
			{
				if (value == RootFolderUrlPath)
					return;
				SetStringProperty("RootFolderUrlPath", "", value);
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				HtmlEditorSelectorSettings src = source as HtmlEditorSelectorSettings;
				if(src != null) {
					RootFolderUrlPath = src.RootFolderUrlPath;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class HtmlEditorImageSelectorSettings : HtmlEditorSelectorSettings {
		public HtmlEditorImageSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new HtmlEditorImageSelectorUploadSettings(Owner);
		}
	}
	public class HtmlEditorFlashSelectorSettings : HtmlEditorSelectorSettings {
		public HtmlEditorFlashSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new HtmlEditorFlashSelectorUploadSettings(Owner);
		}
		protected override HtmlEditorFileManagerCommonSettings CreateCommonSettings() {
			return new HtmlEditorFlashSelectorCommonSettings(Owner);
		}
	}
	public class HtmlEditorVideoSelectorSettings : HtmlEditorSelectorSettings {
		public HtmlEditorVideoSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new HtmlEditorVideoSelectorUploadSettings(Owner);
		}
		protected override HtmlEditorFileManagerCommonSettings CreateCommonSettings() {
			return new HtmlEditorVideoSelectorCommonSettings(Owner);
		}
	}
	public class HtmlEditorAudioSelectorSettings : HtmlEditorSelectorSettings {
		public HtmlEditorAudioSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new HtmlEditorAudioSelectorUploadSettings(Owner);
		}
		protected override HtmlEditorFileManagerCommonSettings CreateCommonSettings() {
			return new HtmlEditorAudioSelectorCommonSettings(Owner);
		}
	}
	public class HtmlEditorDocumentSelectorSettings : HtmlEditorSelectorSettings {
		public HtmlEditorDocumentSelectorSettings(IPropertiesOwner owner) : base(owner) { }
		protected override HtmlEditorFileManagerCommonSettings CreateCommonSettings() {
			return new HtmlEditorDocumentSelectorCommonSettings(Owner);
		}
		protected override HtmlEditorFileManagerUploadSettings CreateUploadSettings() {
			return new HtmlEditorDocumentSelectorUploadSettings(Owner);
		}
		protected override HtmlEditorFileManagerFoldersSettings CreateFoldersSettings() {
			return new HtmlEditorDocumentSelectorFoldersSettings(Owner);
		}
		protected override HtmlEditorFileManagerFileListSettings CreateFileListSettings() {
			return new HtmlEditorDocumentSelectorFileListSettings(Owner);
		}
		protected override HtmlEditorFileManagerBreadcrumbsSettings CreateBreadCrumbsSettings() {
			return new HtmlEditorDocumentSelectorBreadcrumbsSettings(Owner);
		}
		protected override HtmlEditorFileManagerToolbarSettings CreateToolbarSettings() {
			return new HtmlEditorDocumentSelectorToolbarSettings(Owner);
		}
	}
	public class HtmlEditorFileManagerFileListSettings : FileManagerSettingsFileList {
		public HtmlEditorFileManagerFileListSettings(IPropertiesOwner owner) : base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerFileListSettingsShowParentFolder"),
#endif
 DefaultValue(true)]
		public override bool ShowParentFolder {
			get { return base.ShowParentFolder; }
			set { base.ShowParentFolder = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorFileManagerFileListSettingsShowFolders"),
#endif
 DefaultValue(true)]
		public override bool ShowFolders {
			get { return base.ShowFolders; }
			set { base.ShowFolders = value; }
		}
		protected override bool GetDefaultShowParentFolder() {
			return true;
		}
		protected override bool GetDefaultShowFolders() {
			return true;
		}
	}
	public class ASPxHtmlEditorTextSettings : ASPxHtmlEditorSettingsBase {
		public ASPxHtmlEditorTextSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorTextSettingsDesignViewTab"),
#endif
		AutoFormatDisable, DefaultValue(StringResources.HtmlEditorText_Design), NotifyParentProperty(true)]
		public string DesignViewTab {
			get { return GetStringProperty("DesignViewTab", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DesignViewTab)); }
			set { SetStringProperty("DesignViewTab", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DesignViewTab), value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorTextSettingsHtmlViewTab"),
#endif
		AutoFormatDisable, DefaultValue(StringResources.HtmlEditorText_HTML), NotifyParentProperty(true)]
		public string HtmlViewTab {
			get { return GetStringProperty("HtmlViewTab", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.HtmlViewTab)); }
			set { SetStringProperty("HtmlViewTab", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.HtmlViewTab), value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorTextSettingsPreviewTab"),
#endif
		AutoFormatDisable, DefaultValue(StringResources.HtmlEditorText_Preview), NotifyParentProperty(true)]
		public string PreviewTab {
			get { return GetStringProperty("PreviewTab", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.PreviewTab)); }
			set { SetStringProperty("PreviewTab", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.PreviewTab), value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxHtmlEditorTextSettings src = source as ASPxHtmlEditorTextSettings;
				if(src != null) {
					DesignViewTab = src.DesignViewTab;
					HtmlViewTab = src.HtmlViewTab;
					PreviewTab = src.PreviewTab;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	public class ASPxHtmlEditorSpellCheckerSpellingSettings : ASPxSpellCheckerSpellingSettings {
		public ASPxHtmlEditorSpellCheckerSpellingSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSpellCheckerSpellingSettingsIgnoreMarkupTags"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public override bool IgnoreMarkupTags {
			get { return GetBoolProperty("IgnoreTagContent", true); }
			set {
				if(IgnoreMarkupTags != value)
					SetBoolProperty("IgnoreTagContent", true, value);
			}
		}
	}
	public class ASPxHtmlEditorSpellCheckerSettings : ASPxHtmlEditorSettingsBase {
		SpellCheckerDictionaryCollection dictionaries;
		ASPxHtmlEditorSpellCheckerSpellingSettings settingsSpelling;
		ASPxSpellCheckerTextSettings settingsText;
		public ASPxHtmlEditorSpellCheckerSettings(IPropertiesOwner owner)
			: base(owner) {
			this.dictionaries = new SpellCheckerDictionaryCollection(HtmlEditor);
			this.settingsText = new ASPxSpellCheckerTextSettings(owner);
			this.settingsSpelling = new ASPxHtmlEditorSpellCheckerSpellingSettings(owner);
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSpellCheckerSettingsLevenshteinDistance"),
#endif
		DefaultValue(4), AutoFormatDisable, NotifyParentProperty(true)]
		public int LevenshteinDistance {
			get { return GetIntProperty("LevenshteinDistance", 4); }
			set {
				CommonUtils.CheckNegativeValue(value, "LevenshteinDistance");
				SetIntProperty("LevenshteinDistance", 4, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSpellCheckerSettingsMaximumErrorCountInResponse"),
#endif
		DefaultValue(100), AutoFormatDisable, NotifyParentProperty(true)]
		public int MaximumErrorCountInResponse {
			get { return GetIntProperty("MaximumErrorCountInResponse", 100); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "MaximumErrorCountInResponse");
				SetIntProperty("MaximumErrorCountInResponse", 100, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSpellCheckerSettingsSuggestionCount"),
#endif
		DefaultValue(5), AutoFormatDisable, NotifyParentProperty(true)]
		public int SuggestionCount {
			get { return GetIntProperty("SuggestionCount", 5); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "SuggestionCount");
				SetIntProperty("SuggestionCount", 5, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSpellCheckerSettingsCulture"),
#endif
		DefaultValue(null), AutoFormatDisable, NotifyParentProperty(true)]
		public CultureInfo Culture {
			get { return (CultureInfo)GetObjectProperty("Culture", null); }
			set { SetObjectProperty("Culture", null, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSpellCheckerSettingsDictionaries"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false),
		NotifyParentProperty(true),		
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public SpellCheckerDictionaryCollection Dictionaries {
			get { return dictionaries; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSpellCheckerSettingsSettingsSpelling"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true),		
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxHtmlEditorSpellCheckerSpellingSettings SettingsSpelling {
			get { return this.settingsSpelling; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("ASPxHtmlEditorSpellCheckerSettingsSettingsText"),
#endif
		AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxSpellCheckerTextSettings SettingsText { get { return settingsText; } }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxHtmlEditorSpellCheckerSettings src = source as ASPxHtmlEditorSpellCheckerSettings;
				if(src != null) {
					SuggestionCount = src.SuggestionCount;
					LevenshteinDistance = src.LevenshteinDistance;
					MaximumErrorCountInResponse = src.MaximumErrorCountInResponse;
					Culture = src.Culture;
					Dictionaries.Assign(src.Dictionaries);
					SettingsSpelling.Assign(src.SettingsSpelling);
					SettingsText.Assign(src.SettingsText);
				}
			} finally {
				EndUpdate();
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return new IStateManager[] { SettingsSpelling, SettingsText, Dictionaries };
		}
	}
	public class HtmlEditorResizeSettings : ASPxHtmlEditorSettingsBase {
		public HtmlEditorResizeSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorResizeSettingsAllowResize"),
#endif
		Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool AllowResize {
			get { return GetBoolProperty("AllowResize", false); }
			set {
				SetBoolProperty("AllowResize", false, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorResizeSettingsMaxHeight"),
#endif
		Category("Layout"), AutoFormatDisable, DefaultValue(0), NotifyParentProperty(true)]
		public int MaxHeight {
			get { return GetIntProperty("MaxHeight", 0); }
			set { SetIntProperty("MaxHeight", 0, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorResizeSettingsMaxWidth"),
#endif
		Category("Layout"), AutoFormatDisable, DefaultValue(0), NotifyParentProperty(true)]
		public int MaxWidth {
			get { return GetIntProperty("MaxWidth", 0); }
			set { SetIntProperty("MaxWidth", 0, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorResizeSettingsMinHeight"),
#endif
		Category("Layout"), AutoFormatDisable, DefaultValue(0), NotifyParentProperty(true)]
		public int MinHeight {
			get { return GetIntProperty("MinHeight", 0); }
			set { SetIntProperty("MinHeight", 0, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorResizeSettingsMinWidth"),
#endif
		Category("Layout"), AutoFormatDisable, DefaultValue(0), NotifyParentProperty(true)]
		public int MinWidth {
			get { return GetIntProperty("MinWidth", 0); }
			set { SetIntProperty("MinWidth", 0, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				HtmlEditorResizeSettings src = source as HtmlEditorResizeSettings;
				if(src != null) {
					AllowResize = src.AllowResize;
					MaxHeight = src.MaxHeight;
					MaxWidth = src.MaxWidth;
					MinHeight = src.MinHeight;
					MinWidth = src.MinWidth;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class InsertImageCssClassItems : Collection<InsertImageCssClassItem> {
		public InsertImageCssClassItems()
			: base() {
		}
		public InsertImageCssClassItems(IWebControlObject owner)
			: base(owner) {
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class InsertMediaCssClassItems : Collection<InsertMediaCssClassItem> {
		public InsertMediaCssClassItems()
			: base() {
		}
		public InsertMediaCssClassItems(IWebControlObject owner)
			: base(owner) {
		}
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class DialogCssClassItems : Collection<DialogCssClassItem> {
		public DialogCssClassItems()
			: base() {
		}
		public DialogCssClassItems(IWebControlObject owner)
			: base(owner) {
		}
	}
	public class DialogCssClassItem : CollectionItem {
		public DialogCssClassItem()
			: base() {
		}
		[
		NotifyParentProperty(true), AutoFormatDisable]
		public string Text {
			get { return GetStringProperty("Text", ""); }
			set { SetStringProperty("Text", "", value); }
		}
		[
		NotifyParentProperty(true), AutoFormatDisable]
		public string CssClass {
			get { return GetStringProperty("CssClass", ""); }
			set { SetStringProperty("CssClass", "", value); }
		}
		public override void Assign(CollectionItem source) {
			DialogCssClassItem src = source as DialogCssClassItem;
			if(src != null) {
				Text = src.Text;
				CssClass = src.CssClass;
			}
			base.Assign(source);
		}
		public override string ToString() {
			return string.IsNullOrEmpty(Text) ? GetType().Name : Text;
		}
	}
	public class InsertMediaCssClassItem : DialogCssClassItem {
		public InsertMediaCssClassItem()
			: base() {
		}
		[
		NotifyParentProperty(true), AutoFormatDisable]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		NotifyParentProperty(true), AutoFormatDisable]
		public new string CssClass {
			get { return base.CssClass; }
			set { base.CssClass = value; }
		}
	}
	public class InsertImageCssClassItem : InsertMediaCssClassItem {
		public InsertImageCssClassItem()
			: base() {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("InsertImageCssClassItemText"),
#endif
		NotifyParentProperty(true), AutoFormatDisable]
		public new string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("InsertImageCssClassItemCssClass"),
#endif
		NotifyParentProperty(true), AutoFormatDisable]
		public new string CssClass {
			get { return base.CssClass; }
			set { base.CssClass = value; }
		}
	}
	public class HtmlEditorRequiredFieldValidationPattern : RequiredFieldValidationPattern {
		public HtmlEditorRequiredFieldValidationPattern(IPropertiesOwner owner, IValidationSettings validationSettings)
			: base(owner, validationSettings) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorRequiredFieldValidationPatternErrorText"),
#endif
		Category("Appearance"), DefaultValue(StringResources.HtmlEditorText_RequiredHtmlContentError),
		NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public override string ErrorText {
			get { return GetStringProperty("ErrorText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RequiredHtmlContentError)); }
			set { SetStringProperty("ErrorText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.RequiredHtmlContentError), value); }
		}
	}
	public class HtmlEditorValidationSettings : ASPxHtmlEditorSettingsBase, IValidationSettings {
		HtmlEditorErrorFrameStyle errorFrameStyle;
		HtmlEditorErrorFrameCloseButtonStyle errorFrameCloseButtonStyle;
		HtmlEditorRequiredFieldValidationPattern requiredField;
		public HtmlEditorValidationSettings()
			: this(null) {
		}
		public HtmlEditorValidationSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorValidationSettingsErrorText"),
#endif
		NotifyParentProperty(true), AutoFormatEnable,
		DefaultValue(StringResources.HtmlEditorText_DefaultErrorText), Localizable(true)]
		public virtual string ErrorText {
			get {
				string errorTextFromContext = GetErrorTextFromContext();
				if(!string.IsNullOrEmpty(errorTextFromContext))
					return errorTextFromContext;
				return GetStringProperty("ErrorText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DefaultErrorText));
			}
			set {
				SetErrorTextInContext(value);
				SetStringProperty("ErrorText", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DefaultErrorText), value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorValidationSettingsErrorFrameStyle"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorErrorFrameStyle ErrorFrameStyle {
			get {
				if(errorFrameStyle == null)
					errorFrameStyle = new HtmlEditorErrorFrameStyle();
				return errorFrameStyle;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorValidationSettingsErrorFrameCloseButtonStyle"),
#endif
		Category("Appearance"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HtmlEditorErrorFrameCloseButtonStyle ErrorFrameCloseButtonStyle {
			get {
				if(errorFrameCloseButtonStyle == null)
					errorFrameCloseButtonStyle = new HtmlEditorErrorFrameCloseButtonStyle();
				return errorFrameCloseButtonStyle;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorValidationSettingsRequiredField"),
#endif
		Category("Behavior"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public HtmlEditorRequiredFieldValidationPattern RequiredField {
			get {
				if(requiredField == null)
					requiredField = new HtmlEditorRequiredFieldValidationPattern(this, this);
				return requiredField;
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorValidationSettingsValidationGroup"),
#endif
		Category("Behavior"), NotifyParentProperty(true), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ValidationGroup {
			get { return GetStringProperty("ValidationGroup", ""); }
			set { SetStringProperty("ValidationGroup", "", value); }
		}
		protected internal bool Display {
			get { return DevExpress.Web.Internal.ValidationSummaryCollection.Instance.EditorsAllowedToShowErrors(ValidationGroup); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is HtmlEditorValidationSettings) {
					HtmlEditorValidationSettings src = source as HtmlEditorValidationSettings;
					ErrorText = src.ErrorText;
					ErrorFrameCloseButtonStyle.Assign(src.ErrorFrameCloseButtonStyle);
					ErrorFrameStyle.Assign(src.ErrorFrameStyle);
					RequiredField.Assign(src.RequiredField);
					ValidationGroup = src.ValidationGroup;
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal string GetClientValidationPatternsArray() {
			StringBuilder sb = new StringBuilder("[");
			if(RequiredField.IsRequired)
				sb.Append(RequiredField.GetClientInstanceCreationScript());
			sb.Append("]");
			return sb.ToString();
		}
		protected internal bool HasValidationPatterns() {
			return RequiredField.IsRequired;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ErrorFrameCloseButtonStyle, ErrorFrameStyle, RequiredField });
		}
		private string GetErrorTextFromContext() {
			ASPxEdit edit = Owner as ASPxEdit;
			if(edit == null)
				return null;
			else {
				string key = GetErrorTextContextKey(edit);
				return HttpUtils.GetContextValue(key, string.Empty);
			}
		}
		private void SetErrorTextInContext(string errorText) {
			ASPxEdit edit = Owner as ASPxEdit;
			if(edit != null) {
				string key = GetErrorTextContextKey(edit);
				HttpUtils.SetContextValue(key, errorText);
			}
		}
		protected virtual string GetErrorTextContextKey(ASPxEdit owner) {
			return owner.UniqueID + "_ErrorText";
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class HtmlEditorHtmlViewSettings : ASPxHtmlEditorSettingsBase {
		public HtmlEditorHtmlViewSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowCollapseTagButtons {
			get { return GetBoolProperty("ShowCollapseTagButtons", false); }
			set {
				SetBoolProperty("ShowCollapseTagButtons", false, value);
				Changed();
			}
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool ShowLineNumbers {
			get { return GetBoolProperty("ShowLineNumbers", false); }
			set {
				SetBoolProperty("ShowLineNumbers", false, value);
				Changed();
			}
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool HighlightActiveLine {
			get { return GetBoolProperty("HighlightActiveLine", false); }
			set {
				SetBoolProperty("HighlightActiveLine", false, value);
				Changed();
			}
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool HighlightMatchingTags {
			get { return GetBoolProperty("HighlightMatchingTags", false); }
			set {
				SetBoolProperty("HighlightMatchingTags", false, value);
				Changed();
			}
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(true), NotifyParentProperty(true)]
		public bool EnableTagAutoClosing {
			get { return GetBoolProperty("EnableTagAutoClosing", true); }
			set {
				SetBoolProperty("EnableTagAutoClosing", true, value);
				Changed();
			}
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(false), NotifyParentProperty(true)]
		public bool EnableAutoCompletion {
			get { return GetBoolProperty("EnableAutoCompletion", false); }
			set {
				SetBoolProperty("EnableAutoCompletion", false, value);
				Changed();
			}
		}
		[Category("Behavior"), AutoFormatDisable, DefaultValue(HtmlEditorHtmlEditingMode.Advanced), NotifyParentProperty(true)]
		public HtmlEditorHtmlEditingMode Mode {
			get { return (HtmlEditorHtmlEditingMode)GetEnumProperty("Mode", HtmlEditorHtmlEditingMode.Advanced); }
			set {
				SetEnumProperty("Mode", HtmlEditorHtmlEditingMode.Advanced, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				HtmlEditorHtmlViewSettings src = source as HtmlEditorHtmlViewSettings;
				if(src != null) {
					ShowCollapseTagButtons = src.ShowCollapseTagButtons;
					ShowLineNumbers = src.ShowLineNumbers;
					HighlightActiveLine = src.HighlightActiveLine;
					HighlightMatchingTags = src.HighlightMatchingTags;
					EnableTagAutoClosing = src.EnableTagAutoClosing;
					EnableAutoCompletion = src.EnableAutoCompletion;
					Mode = src.Mode;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
}
