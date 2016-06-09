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
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
using DevExpress.Web;
using System.Drawing;
namespace DevExpress.Web {
	public enum UploadControlFileUploadMode { BeforePageLoad, OnPageLoad };
	public enum UploadControlUploadMode { Standard, Advanced, Auto };
	public enum AddUploadButtonsHorizontalPosition { Left, Right, InputRightSide, Center };
	public enum CancelButtonHorizontalPosition { Left, Center, Right };
	public enum UploadControlFileListPosition { Top, Bottom };
	public enum UploadControlUploadStorage { NotSet, FileSystem, Azure, Amazon, Dropbox };
	[DXWebToolboxItem(true),
	DevExpress.Utils.Design.DXClientDocumentationProvider("#AspNet/clsDevExpressWebScriptsASPxClientUploadControltopic"),
	DefaultEvent("FileUploadComplete"), ControlValueProperty("FileBytes"),
	ToolboxData("<{0}:ASPxUploadControl Width=\"280px\" UploadMode=\"Auto\" runat=\"server\"></{0}:ASPxUploadControl>"),
	Designer("DevExpress.Web.Design.ASPxUploadControlDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxUploadControl.bmp")
	]
	public class ASPxUploadControl : ASPxWebControl, IAssociatedControlID {
		#region Nested types
		private static class CallbackResultProperty {
			public const string
				ErrorTexts = "errorTexts",
				IsValidArray = "isValidArray",
				CallbackDataArray = "callbackDataArray",
				CommonErrorText = "commonErrorText",
				CommonCallbackData = "commonCallbackData",
				CustomJSProperties = "customJSProperties";
		}
		#endregion
		protected internal const string UploadScriptsResourcePath = WebScriptsResourcePath + "Upload.js";
		protected const string UploadFileInputHandlerName = "aspxUUploadFileClick('{0}')";
		protected const string InputCountStateKey = "inputCount";
		private const string InputIdPostfix = "_Input";
		private const string InputRowIdPostfix = "FI";
		private const string FakeInputIdPostfix = "_FakeInput";
		private const string TemplateInputIdPostfix = "T";
		private const string TemplateErrorRowIdPostfix = "RT";
		protected internal const string ButtonImageIdPostfix = "Img";
		protected internal const string ButtonBackImageName = "ucButtonBack";
		protected internal const string ButtonHoverBackImageName = "ucButtonHoverBack";
		protected internal const int DefaultPacketSizeValue = 200000;
		protected internal const string DefaultTemporaryFolder = "~\\App_Data\\UploadTemp\\";
		protected internal const string TemporaryFileNamePrefix = "dxupload_";
		protected internal const string TemporaryFolderCleanerKey = "aspxDXTemporaryUploadFolderCleaner";
		protected internal const string SilverlightPluginLink = "http://go.microsoft.com/fwlink/?LinkID=149156";
		protected internal const string SLUploadHelperName = UploadHelpersResourcePath + "SL.ASPxUploadHelper.xap";
		protected internal const string UploadHelpersResourcePath = "DevExpress.Web.UploadControl.Helpers.";
		protected internal const string DesignerDefaultDropZoneText = "Drop file(s) here";
		protected internal const string SSLSecureBlankUrlForIE = "about:blank";
		protected internal readonly string SSLSecureBlankUrl = string.Empty;
		private MainUploadControlBase uploadControl = null;
		private AddButtonProperties addButton = null;
		private BrowseButtonProperties browseButton = null;
		private RemoveButtonProperties removeButton = null;
		private UploadButtonProperties uploadButton = null;
		private CancelButtonProperties cancelButton = null;
		private UploadProgressBarSettings progressBarSettings = null;
		private UploadAdvancedModeSettings advancedModeSettings = null;
		private UploadControlValidationSettings validationSettings = null;
		private UploadControlAzureSettings azureSettings = null;
		private UploadControlAmazonSettings amazonSettings = null;
		private UploadControlDropboxSettings dropboxSettings = null;
		private UploadControlFileSystemSettings fileSystemSettings = null;
		private string settingsID;
		private bool isValidInternal = true;
		private bool previewErrorInDesigner = false;
		private bool uploaded = false;
		private string uploadingKey = null;
		private List<UploadedFile> uploadedFiles = null;
		private List<string> errorTexts = null;
		private List<string> callbackDataArray = null;
		private string commonErrorText = "";
		private string commonCallbackData = "";
		private string[] allowedFileExtensions = null;
		private static readonly object EventFileUploadComplete = new object();
		private static readonly object EventFilesUploadComplete = new object();
		public ASPxUploadControl()
			: this(null) {
		}
		protected ASPxUploadControl(ASPxWebControl ownerControl)
			: base(ownerControl) {
			this.addButton = new AddButtonProperties(this);
			this.browseButton = new BrowseButtonProperties(this);
			this.removeButton = new RemoveButtonProperties(this);
			this.uploadButton = new UploadButtonProperties(this);
			this.cancelButton = new CancelButtonProperties(this);
			this.progressBarSettings = new UploadProgressBarSettings(this);
			this.advancedModeSettings = new UploadAdvancedModeSettings(this);
			this.validationSettings = new UploadControlValidationSettings(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlValidationSettings"),
#endif
		Category("Validation"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlValidationSettings ValidationSettings {
			get { return this.validationSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlNative"),
#endif
Category("Appearance"), DefaultValue(false), AutoFormatEnable,
		RefreshProperties(RefreshProperties.Repaint)]
		public new bool Native
		{
			get { return base.Native; }
			set
			{
				base.Native = value;
				LayoutChanged();
				if (value)
				{
					UploadMode = UploadControlUploadMode.Standard;
					PropertyChanged("UploadMode");
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public UploadControlClientSideEvents ClientSideEvents {
			get { return (UploadControlClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlClientEnabled"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientEnabled {
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlJSProperties"),
#endif
		Category("Client-Side"), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlFileUploadMode"),
#endif
		Category("Behavior"), DefaultValue(UploadControlFileUploadMode.BeforePageLoad), AutoFormatDisable, Localizable(false)]
		public UploadControlFileUploadMode FileUploadMode {
			get { return (UploadControlFileUploadMode)GetEnumProperty("FileUploadMode", UploadControlFileUploadMode.BeforePageLoad); }
			set { SetEnumProperty("FileUploadMode", UploadControlFileUploadMode.BeforePageLoad, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlUploadMode"),
#endif
		Category("Behavior"), DefaultValue(UploadControlUploadMode.Standard), AutoFormatDisable,
		Localizable(false), RefreshProperties(RefreshProperties.Repaint)]
		public UploadControlUploadMode UploadMode {
			get { return (UploadControlUploadMode)GetEnumProperty("UploadMode", UploadControlUploadMode.Standard); }
			set {
				SetEnumProperty("UploadMode", UploadControlUploadMode.Standard, value);
				if(value == UploadControlUploadMode.Advanced || value == UploadControlUploadMode.Auto) {
					Native = false;
					PropertyChanged("Native");
				}
				else {
					AdvancedModeSettings.EnableMultiSelect = false;
					AdvancedModeSettings.EnableDragAndDrop = false;
					PropertyChanged("AdvancedModeSettings");
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlUploadStorage"),
#endif
		Category("Behavior"), DefaultValue(UploadControlUploadStorage.NotSet), AutoFormatDisable, Localizable(false)]
		public UploadControlUploadStorage UploadStorage {
			get { return (UploadControlUploadStorage)GetEnumProperty("UploadStorage", UploadControlUploadStorage.NotSet); }
			set { SetEnumProperty("UploadStorage", UploadControlUploadStorage.NotSet, value); }
		}
		[
		Category("Behavior"), DefaultValue(false), AutoFormatDisable, Localizable(false)]
		public bool AutoStartUpload {
			get { return GetBoolProperty("AutoStart", false); }
			set { SetBoolProperty("AutoStart", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlShowAddRemoveButtons"),
#endif
		Category("Settings"), DefaultValue(false), AutoFormatDisable, Localizable(false),
		RefreshProperties(RefreshProperties.Repaint)]
		public bool ShowAddRemoveButtons {
			get { return GetBoolProperty("ShowAddRemoveButtons", false); }
			set {
				SetBoolProperty("ShowAddRemoveButtons", false, value);
				LayoutChanged();
				if(value) {
					AdvancedModeSettings.EnableMultiSelect = false;
					AdvancedModeSettings.EnableFileList = false;
					AdvancedModeSettings.EnableDragAndDrop = false;
					PropertyChanged("AdvancedModeSettings");
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlShowClearFileSelectionButton"),
#endif
		Category("Settings"), DefaultValue(true), AutoFormatDisable, Localizable(false),
		RefreshProperties(RefreshProperties.Repaint)]
		public bool ShowClearFileSelectionButton {
			get { return GetBoolProperty("ShowClearFileSelectionButton", true); }
			set {
				SetBoolProperty("ShowClearFileSelectionButton", true, value);
				LayoutChanged();
				if(value && Native) {
					Native = false;
					PropertyChanged("Native");
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlShowProgressPanel"),
#endif
		Category("Settings"), DefaultValue(false), AutoFormatDisable, Localizable(false)]
		public bool ShowProgressPanel {
			get { return GetBoolProperty("ShowProgressPanel", false); }
			set {
				SetBoolProperty("ShowProgressPanel", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlShowUploadButton"),
#endif
		Category("Settings"), DefaultValue(false), AutoFormatDisable, Localizable(false)]
		public bool ShowUploadButton {
			get { return GetBoolProperty("ShowUploadButton", false); }
			set {
				SetBoolProperty("ShowUploadButton", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlShowTextBox"),
#endif
		Category("Settings"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public bool ShowTextBox {
			get { return GetBoolProperty("ShowTextBox", true); }
			set {
				SetBoolProperty("ShowTextBox", true, value);
				LayoutChanged();
			}
		}
		protected internal bool ShowUI {
			get { return GetBoolProperty("ShowUI", true); }
			set {
				SetBoolProperty("ShowUI", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlAddButton"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AddButtonProperties AddButton {
			get { return addButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlBrowseButton"),
#endif
Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BrowseButtonProperties BrowseButton
		{
			get { return browseButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlRemoveButton"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RemoveButtonProperties RemoveButton {
			get { return removeButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlUploadButton"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadButtonProperties UploadButton {
			get { return uploadButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlCancelButton"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CancelButtonProperties CancelButton {
			get { return cancelButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlProgressBarSettings"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadProgressBarSettings ProgressBarSettings {
			get { return progressBarSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlAdvancedModeSettings"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadAdvancedModeSettings AdvancedModeSettings {
			get { return advancedModeSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlAzureSettings"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlAzureSettings AzureSettings {
			get {
				if(this.azureSettings == null)
					this.azureSettings = new UploadControlAzureSettings();
				return this.azureSettings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlAmazonSettings"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlAmazonSettings AmazonSettings {
			get {
				if(this.amazonSettings == null)
					this.amazonSettings = new UploadControlAmazonSettings();
				return this.amazonSettings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlDropboxSettings"),
#endif
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlDropboxSettings DropboxSettings {
			get {
				if(this.dropboxSettings == null)
					this.dropboxSettings = new UploadControlDropboxSettings();
				return this.dropboxSettings;
			}
		}
		[
		Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlFileSystemSettings FileSystemSettings {
			get {
				if(this.fileSystemSettings == null)
					this.fileSystemSettings = new UploadControlFileSystemSettings();
				return this.fileSystemSettings;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlFileInputCount"),
#endif
		DefaultValue(1), AutoFormatDisable, Localizable(false)]
		public int FileInputCount {
			get { return GetIntProperty("FileInputCount", 1); }
			set {
				CommonUtils.CheckNegativeValue(value, "FileInputCount");
				SetIntProperty("FileInputCount", 1, value);
				LayoutChanged();
				if(value != 1) {
					AdvancedModeSettings.EnableMultiSelect = false;
					PropertyChanged("AdvancedModeSettings");
				}
				ValidateFileInputCount();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlSize"),
#endif
		DefaultValue(0), AutoFormatDisable, RefreshProperties(RefreshProperties.Repaint)]
		public int Size {
			get { return GetIntProperty("Size", 0); }
			set {
				if(value > 0) {
					Width = Unit.Empty;
					PropertyChanged("Width");
				}
				CommonUtils.CheckNegativeValue(value, "Size");
				SetIntProperty("Size", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlNullText"),
#endif
		DefaultValue(""), AutoFormatEnable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return GetStringProperty("NullText", ""); }
			set { SetStringProperty("NullText", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlDialogTriggerID"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public string DialogTriggerID {
			get { return GetStringProperty("DialogTriggerID", ""); }
			set { SetStringProperty("DialogTriggerID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlAddUploadButtonsHorizontalPosition"),
#endif
		Category("Layout"), DefaultValue(AddUploadButtonsHorizontalPosition.Left), AutoFormatEnable, Localizable(false)]
		public AddUploadButtonsHorizontalPosition AddUploadButtonsHorizontalPosition {
			get { return (AddUploadButtonsHorizontalPosition)GetEnumProperty("AddUploadButtonsHorizontalPosition", AddUploadButtonsHorizontalPosition.Left); }
			set {
				SetEnumProperty("AddUploadButtonsHorizontalPosition", AddUploadButtonsHorizontalPosition.Left, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlCancelButtonHorizontalPosition"),
#endif
		Category("Layout"), DefaultValue(CancelButtonHorizontalPosition.Center), AutoFormatEnable, Localizable(false)]
		public CancelButtonHorizontalPosition CancelButtonHorizontalPosition {
			get { return (CancelButtonHorizontalPosition)GetEnumProperty("CancelButtonHorizontalPosition", CancelButtonHorizontalPosition.Center); }
			set {
				SetEnumProperty("CancelButtonHorizontalPosition", CancelButtonHorizontalPosition.Center, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlAddUploadButtonsSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable, Localizable(false)]
		public Unit AddUploadButtonsSpacing {
			get { return GetUnitProperty("AddUploadButtonsSpacing", Unit.Empty); }
			set {
				SetUnitProperty("AddUploadButtonsSpacing", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlButtonSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable, Localizable(false)]
		public Unit ButtonSpacing {
			get { return GetUnitProperty("ButtonSpacing", Unit.Empty); }
			set {
				SetUnitProperty("ButtonSpacing", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlCancelButtonSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable, Localizable(false)]
		public Unit CancelButtonSpacing {
			get { return GetUnitProperty("CancelButtonSpacing", Unit.Empty); }
			set {
				SetUnitProperty("CancelButtonSpacing", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlFileInputSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable, Localizable(false)]
		public Unit FileInputSpacing {
			get { return GetUnitProperty("FileInputSpacing", Unit.Empty); }
			set {
				SetUnitProperty("FileInputSpacing", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlRemoveButtonSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable, Localizable(false)]
		public Unit RemoveButtonSpacing {
			get { return GetUnitProperty("RemoveButtonSpacing", Unit.Empty); }
			set {
				SetUnitProperty("RemoveButtonSpacing", Unit.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((AppearanceStyle)ControlStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlImageFolder"),
#endif
Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatImageFolderProperty, AutoFormatUrlProperty]
		public string ImageFolder
		{
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlSpriteImageUrl"),
#endif
Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteImageUrl
		{
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlSpriteCssFilePath"),
#endif
Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath
		{
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlClearFileSelectionImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagePropertiesEx ClearFileSelectionImage {
			get { return Images.ClearButtonImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlWidth"),
#endif
		RefreshProperties(RefreshProperties.Repaint)]
		public override Unit Width {
			get { return base.Width; }
			set {
				if(!value.IsEmpty) {
					Size = 0;
					PropertyChanged("Size");
				}
				base.Width = value;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("Use the UploadedFiles[] property")]
		public bool HasFile {
			get { return ((UploadedFiles[0].PostedFile != null) && (UploadedFiles[0].PostedFile.ContentLength > 0)); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("Use the UploadedFiles[].FileBytes property")]
		public byte[] FileBytes {
			get { return (UploadedFiles[0] != null) ? UploadedFiles[0].FileBytes : null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("Use the UploadedFiles[].FilesContent property")]
		public Stream FileContent {
			get { return (UploadedFiles[0] != null) ? UploadedFiles[0].FileContent : null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("Use the UploadedFiles[].FilesName property")]
		public string FileName {
			get { return (UploadedFiles[0] != null) ? UploadedFiles[0].FileName : null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("Use the UploadedFiles[].PostedFile property")]
		public HttpPostedFile PostedFile {
			get { return (UploadedFiles[0] != null) ? UploadedFiles[0].PostedFileInternal.HttpPostedFile : null; }
		}
		[Browsable(false)]
		public UploadedFile[] UploadedFiles {
			get { return (uploadedFiles != null) ? uploadedFiles.ToArray() : null; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlNullTextStyle"),
#endif
Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlNullTextStyle NullTextStyle {
			get { return Styles.NullText; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlTextBoxStyle"),
#endif
Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlTextBoxStyle TextBoxStyle
		{
			get { return Styles.TextBox; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlBrowseButtonStyle"),
#endif
Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlBrowseButtonStyle BrowseButtonStyle
		{
			get { return Styles.BrowseButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlButtonStyle ButtonStyle {
			get { return Styles.Button; }
		}
		[Browsable(false), Obsolete("Use the ButtonStyle.DisabledStyle property instead."),
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlDisabledButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceSelectedStyle DisabledButtonStyle {
			get { return Styles.Button.DisabledStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlProgressBarStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProgressStyle ProgressBarStyle {
			get { return Styles.ProgressBar; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlProgressBarIndicatorStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProgressBarIndicatorStyle ProgressBarIndicatorStyle {
			get { return Styles.ProgressBarIndicator; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlDropZoneStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlDropZoneStyle DropZoneStyle {
			get { return Styles.DropZone; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlFileUploadComplete"),
#endif
		Category("Action")]
		public event EventHandler<FileUploadCompleteEventArgs> FileUploadComplete
		{
			add { Events.AddHandler(EventFileUploadComplete, value); }
			remove { Events.RemoveHandler(EventFileUploadComplete, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlFilesUploadComplete"),
#endif
		Category("Action")]
		public event EventHandler<FilesUploadCompleteEventArgs> FilesUploadComplete
		{
			add { Events.AddHandler(EventFilesUploadComplete, value); }
			remove { Events.RemoveHandler(EventFilesUploadComplete, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxUploadControlCustomJSProperties"),
#endif
		Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties
		{
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		[Browsable(false), Obsolete("Use the UploadedFiles[].IsValid property")]
		public bool IsValid {
			get { return UploadedFiles != null ? UploadedFiles[0].IsValid : true; }
		}
		protected internal bool ShowCancelButton {
			get { return GetBoolProperty("ShowCancelButton", true); }
			set { SetBoolProperty("ShowCancelButton", true, value); }
		}
		protected internal string ErrorText {
			get { return errorTexts[0]; }
			set {
				errorTexts[0] = value;
				LayoutChanged();
			}
		}
		protected internal string[] ErrorTexts {
			get { return (this.errorTexts != null) ? this.errorTexts.ToArray() : null; }
		}
		protected internal bool PreviewErrorInDesigner {
			get { return this.previewErrorInDesigner; }
			set {
				this.previewErrorInDesigner = value;
				LayoutChanged();
			}
		}
		protected internal string[] CallbackDataArray {
			get { return this.callbackDataArray.ToArray(); }
		}
		protected string CommonErrorText { get { return commonErrorText; } }
		protected string CommonCallbackData { get { return commonCallbackData; } }
		protected string SettingsID {
			get {
				if(string.IsNullOrEmpty(this.settingsID))
					this.settingsID = GetUploadSettingsID();
				return this.settingsID;
			}
		}
		protected string UploadingKey {
			get {
				if(string.IsNullOrEmpty(this.uploadingKey))
					this.uploadingKey = GetNewUploadingKey();
				return this.uploadingKey;
			}
		}
		protected bool IsCleanerActivated { get; set; }
		protected internal bool IsValidInternal {
			get { return isValidInternal; }
			set {
				isValidInternal = value;
				LayoutChanged();
			}
		}
		protected long MaxFileSize {
			get { return ValidationSettings.MaxFileSize; }
		}
		protected string[] AllowedContentTypes {
			get { return ValidationSettings.AllowedContentTypesInternal; }
		}
		protected string[] AllowedFileExtensions {
			get {
				if(this.allowedFileExtensions == null) {
					if(IsAnyExtensionAllowed())
						this.allowedFileExtensions = new string[] { };
					else {
						this.allowedFileExtensions = (string[])ValidationSettings.AllowedFileExtensions.Clone();
						for(int i = 0; i < this.allowedFileExtensions.Length; i++)
							this.allowedFileExtensions[i] = this.allowedFileExtensions[i].ToLowerInvariant();
					}
				}
				return this.allowedFileExtensions;
			}
		}
		protected UploadControlStyles Styles {
			get { return (UploadControlStyles)StylesInternal; }
		}
		protected MainUploadControlBase UploadControl {
			get { return this.uploadControl; }
		}
		public string GetRandomFileName() {
			return GetRandomFileName(0);
		}
		public string GetRandomFileName(int index) {
			string randomFileName = Path.GetRandomFileName();
			string ret = Path.GetFileNameWithoutExtension(randomFileName);
			return (UploadedFiles != null) ? ret + Path.GetExtension(UploadedFiles[index].FileName) : randomFileName;
		}
		[Obsolete("Use the UploadedFiles[].SaveAs method instead.")]
		public void SaveAs(string fileName) {
			SaveAs(fileName, true);
		}
		[Obsolete("Use the UploadedFiles[].SaveAs method instead.")]
		public void SaveAs(string fileName, bool allowOverwrite) {
			if(UploadedFiles == null)
				return;
			HttpRuntimeSection runtimeSection = null;
			if(WebConfigurationManager.GetSection("HttpRuntime") != null)
				runtimeSection = WebConfigurationManager.GetSection("HttpRuntime") as HttpRuntimeSection;
			if(!Path.IsPathRooted(fileName) && (runtimeSection != null) &&
				runtimeSection.RequireRootedSaveAsPath)
				throw new HttpException(string.Format(StringResources.UploadControl_SaveAsRequires, fileName));
			FileStream stream = new FileStream(fileName, allowOverwrite ? FileMode.Create : FileMode.CreateNew);
			try {
				stream.Write(UploadedFiles[0].FileBytes, 0, UploadedFiles[0].FileBytes.Length);
				stream.Flush();
			} finally {
				stream.Close();
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.uploadControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(ShowUI)
				this.uploadControl = new MainUploadControl(this);
			else
				this.uploadControl = new MainUploadControlHiddenUI(this);
			Controls.Add(UploadControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CorrectResponseCachePolicy();
			EnsureCleanerActivated();
		}
		void EnsureCleanerActivated() {
			if(!IsCleanerActivated) {
				TemporaryFolderCleaner.Activate();
				IsCleanerActivated = true;
			}
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new UploadControlClientSideEvents();
		}
		protected override Style CreateControlStyle() {
			return new AppearanceStyle();
		}
		protected override StylesBase CreateStyles() {
			return new UploadControlStyles(this);
		}
		protected override ImagesBase CreateImages() {
			return new UploadControlImages(this);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if((IsFileUploadingOnCallback() || IsHelperFileUploadingOnCallback()) && FileUploadMode == UploadControlFileUploadMode.BeforePageLoad)
				EnsureUploaded();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			EnsureUploaded();
		}
		protected override void OnUnload(EventArgs e) {
			base.OnUnload(e);
			if(this.uploaded && IsHelperFileUploadingOnCallback()) {
				string key = Request[RenderUtils.ProgressHandlerKeyQueryParamName];
				HelperUploadManager.RemoveUploadHelper(key);
			}
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(!DesignMode) {
				ValidateValidationSettings();
				CheckTemporaryFolderAccess();
			}
			if((Page != null) && (Page.Form != null) && string.IsNullOrEmpty(Page.Form.Enctype))
				Page.Form.Enctype = "multipart/form-data";
		}
		string IAssociatedControlID.ClientID() {
			return GetUploadInputName(0);
		}
		protected override bool HasHoverScripts() {
			return IsStateScriptEnabled() && !IsNativeRender();
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			base.AddHoverItems(helper);
			for(int i = 0; i < FileInputCount; i++) {
				helper.AddStyle(GetBrowseButtonHoverCssStyle(), GetBrowseButtonID(i), new string[0],
					GetBrowseButtonImage().GetHottrackedScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			}
		}
		protected override bool HasPressedScripts() {
			return IsStateScriptEnabled() && !IsNativeRender();
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			base.AddPressedItems(helper);
			for(int i = 0; i < FileInputCount; i++) {
				helper.AddStyle(GetBrowseButtonPressedCssStyle(), GetBrowseButtonID(i), new string[0],
					GetBrowseButtonImage().GetPressedScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			}
		}
		protected override bool HasDisabledScripts() {
			return IsEnabled();
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			base.AddDisabledItems(helper);
			helper.AddStyle(GetButtonDisabledCssStyle(), AddButton.GetButtonIDSuffix(), new string[0],
				GetAddButtonImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			helper.AddStyle(GetButtonDisabledCssStyle(), UploadButton.GetButtonIDSuffix(), new string[0],
				GetUploadButtonImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			helper.AddStyle(GetButtonDisabledCssStyle(), CancelButton.GetButtonIDSuffix(), new string[0],
				GetCancelButtonImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			for(int i = 0; i < FileInputCount; i++) {
				if(!IsNativeRender()) {
					helper.AddStyle(GetTextBoxDisabledCssStyle(), GetUploadTextBoxCellId(i), new string[] { "", FakeInputIdPostfix },
						null, "", IsEnabled());
					helper.AddStyle(GetClearBoxDisabledCssStyle(), GetUploadClearBoxID(i), new string[0],
						GetClearButtonImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
					helper.AddStyle(GetBrowseButtonDisabledCssStyle(), GetBrowseButtonID(i), new string[0],
						GetBrowseButtonImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
				}
				helper.AddStyle(GetButtonDisabledCssStyle(), GetRemoveButtonID(i), new string[0],
					GetRemoveButtonImage().GetDisabledScriptObject(Page), ButtonImageIdPostfix, IsEnabled());
			}
		}
		public override bool IsClientSideAPIEnabled() {
			if(ShowAddRemoveButtons || ShowUploadButton)
				return true;
			return base.IsClientSideAPIEnabled();
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientUploadControl";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(GetFileInputSpacing() != Unit.Pixel(0))
				stb.AppendFormat("{0}.fileInputSpacing = '{1}';\n", localVarName, GetFileInputSpacing().ToString());
			stb.AppendFormat("{0}.generalErrorText = {1};\n", localVarName, HtmlConvertor.ToScript(ValidationSettings.GeneralErrorText));
			stb.AppendFormat("{0}.unspecifiedErrorText = {1};\n", localVarName, HtmlConvertor.ToScript(ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_UnspecifiedError)));
			stb.AppendFormat("{0}.uploadWasCanceledErrorText = {1};\n", localVarName, HtmlConvertor.ToScript(ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_UploadWasCanceledError)));
			stb.AppendFormat("{0}.selectedSeveralFilesText = {1};\n", localVarName, HtmlConvertor.ToScript(ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_SelectedSeveralFiles)));
			if(ShowTextBox) {
				if(IsNullTextEnabled())
					stb.AppendFormat("{0}.nullText = {1};\n", localVarName, HtmlConvertor.ToScript(NullText));
				if(IsNullTextEnabled() || AdvancedModeSettings.EnableDragAndDrop)
					GetCreateNullTextState(stb, localVarName);
			}
			GetCreateInputFileValidationSettings(stb, localVarName);
			stb.AppendFormat("{0}.progressHandlerPage = '{1}';\n", localVarName, GetProgressHandlerPage());
			stb.AppendFormat("{0}.uploadingKey = '{1}';\n", localVarName, UploadingKey);
			if(IsUploadProcessingEnabled())
				stb.AppendFormat("{0}.uploadProcessingEnabled = true;\n", localVarName);
			if(IsAdvancedOrAutoUploadMode()) {
				stb.AppendFormat("{0}.advancedModeEnabled = true;\n", localVarName);
				stb.AppendFormat("{0}.slUploadHelperUrl = {1};\n", localVarName, HtmlConvertor.ToScript(ResourceManager.GetResourceUrl(Page, typeof(ASPxUploadControl), SLUploadHelperName)));
				if(IsFileListEnabled())
					stb.AppendFormat("{0}.enableFileList = true;\n", localVarName);
				if(AdvancedModeSettings.EnableMultiSelect) {
					stb.AppendFormat("{0}.enableMultiSelect = {1};\n", localVarName, HtmlConvertor.ToScript(AdvancedModeSettings.EnableMultiSelect));
					stb.AppendFormat("{0}.tooManyFilesErrorText = {1};\n", localVarName, HtmlConvertor.ToScript(ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_TooManyFilesError)));
				}
				if(AdvancedModeSettings.EnableDragAndDrop) {
					stb.AppendFormat("{0}.enableDragAndDrop = true;\n", localVarName);
					stb.AppendFormat("{0}.dropZoneText = {1};\n", localVarName, HtmlConvertor.ToScript(AdvancedModeSettings.DropZoneText));
					if(!AdvancedModeSettings.EnableMultiSelect)
						stb.AppendFormat("{0}.dragAndDropMoreThanOneFileError = {1};\n", localVarName, HtmlConvertor.ToScript(ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_DragAndDropMoreThanOneFileError)));
					if (!string.IsNullOrEmpty(AdvancedModeSettings.ExternalDropZoneID))
						stb.AppendFormat("{0}.externalDropZoneIDList = {1};\n", localVarName, HtmlConvertor.ToJSON(GetExternalDropZoneIDList(), true));
				}
				if(AdvancedModeSettings.PacketSize != DefaultPacketSizeValue)
					stb.AppendFormat("{0}.packetSize = {1};\n", localVarName, HtmlConvertor.ToScript(AdvancedModeSettings.PacketSize));
				if(UploadMode == UploadControlUploadMode.Auto)
					stb.AppendFormat("{0}.autoModeEnabled = true;\n", localVarName);
			}
			stb.AppendFormat("{0}.settingsID = {1};\n", localVarName, HtmlConvertor.ToScript(SettingsID));
			stb.AppendFormat("{0}.signature = {1};\n", localVarName, HtmlConvertor.ToScript(GetSignature()));
			if(AutoStartUpload)
				stb.AppendFormat("{0}.autoStart = true;\n", localVarName);
			if(!string.IsNullOrEmpty(DialogTriggerID))
				stb.AppendFormat("{0}.dialogTriggerIDList = {1};\n", localVarName, HtmlConvertor.ToJSON(GetDialogTriggerIDList(), true));
			if(NeedCreateStateObjects()) {
				if(ShowTextBox) {
					GetCreateDisabledTextBoxState(stb, localVarName);
					GetCreateDisabledClearBoxState(stb, localVarName);
				}
				GetCreateHoveredBrowseButtonState(stb, localVarName);
				GetCreatePressedBrowseButtonState(stb, localVarName);
				GetCreateDisabledBrowseButtonState(stb, localVarName);
			}
			GetCreateDisabledRemoveButtonState(stb, localVarName);
			if(AccessibilityCompliant)
				stb.AppendFormat("{0}.accessibilityCompliant = true;\n", localVarName);
		}
		protected void GetCreateInputFileValidationSettings(StringBuilder stb, string localVarName) {
			Hashtable settings = CreateValidationSettingsClientHashtable();
			if(settings.Count > 0)
				stb.AppendFormat("{0}.validationSettings = {1};\n", localVarName, HtmlConvertor.ToJSON(settings));
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			result.Add(InputCountStateKey, FileInputCount);
			return result;
		}
		protected virtual Hashtable CreateValidationSettingsClientHashtable() {
			Hashtable result = new Hashtable();
			if(AllowedFileExtensions.Length > 0) {
				result["allowedFileExtensions"] = AllowedFileExtensions;
				result["notAllowedFileExtensionErrorText"] = ValidationSettings.NotAllowedFileExtensionErrorText;
			}
			if(IsAdvancedOrAutoUploadMode()) {
				result["maxFileSize"] = ValidationSettings.MaxFileSize;
				result["maxFileSizeErrorText"] = ValidationSettings.MaxFileSizeErrorText;
				result["multiSelectionErrorText"] = ValidationSettings.MultiSelectionErrorText;
			}
			if(ValidationSettings.MaxFileCount > 0) {
				result["maxFileCount"] = ValidationSettings.MaxFileCount;
				result["maxFileCountErrorText"] = ValidationSettings.MaxFileCountErrorText;
			}
			result["invalidWindowsFileNameErrorText"] = ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_InvalidWindowsFileName);
			return result;
		}
		protected void GetCreateNullTextState(StringBuilder stb, string localVarName) {
			AppearanceStyleBase textBoxNullStyle = GetTextBoxNullStyle();
			AppearanceStyleBase editAreaNullStyle = GetEditAreaNullStyle();
			AppearanceStyleBase clearBoxNullStyle = GetClearBoxNullStyle();
			Hashtable result = new Hashtable();
			CssStyleCollection attributes;
			Hashtable textBox = new Hashtable();
			textBox["className"] = textBoxNullStyle.CssClass;
			attributes = textBoxNullStyle.GetStyleAttributes(Page);
			if(!textBoxNullStyle.Height.IsEmpty) {
				attributes.Remove(HtmlTextWriterStyle.Height);
				Hashtable inputRow = new Hashtable();
				inputRow["cssText"] = "height:" + textBoxNullStyle.Height;
				result["inputRow"] = inputRow;
			}
			textBox["cssText"] = attributes.Value;
			Hashtable editArea = new Hashtable();
			editArea["cssText"] = editAreaNullStyle.GetStyleAttributes(Page).Value;
			Hashtable clearBox = new Hashtable();
			clearBox["className"] = clearBoxNullStyle.CssClass;
			attributes = clearBoxNullStyle.GetStyleAttributes(Page);
			if(!clearBoxNullStyle.Height.IsEmpty)
				attributes.Remove(HtmlTextWriterStyle.Height);
			clearBox["cssText"] = attributes.Value;
			result["textBox"] = textBox;
			result["editArea"] = editArea;
			result["clearBox"] = clearBox;
			stb.AppendFormat("{0}.nullTextItem = {1};\n", localVarName, HtmlConvertor.ToJSON(result));
		}
		protected void GetCreateDisabledTextBoxState(StringBuilder stb, string localVarName) {
			DisabledStyle style = GetTextBoxDisabledCssStyle();
			string cssClass = style.CssClass;
			string styleAttributes = style.GetStyleAttributes(Page).Value;
			Hashtable result = new Hashtable();
			result["name"] = ClientID;
			result["element"] = GetUploadTextBoxCellId(-1);
			result["className"] = cssClass != null ? new string[] { cssClass } : new string[] { "" };
			result["cssText"] = styleAttributes != null ? new string[] { styleAttributes } : new string[] { "" };
			result["postfixes"] = new string[] { "", FakeInputIdPostfix };
			result["imageUrls"] = new string[] { "" };
			result["imagePostfixes"] = new string[] { "" };
			stb.AppendFormat("{0}.templateDisabledTextBoxItem = {1};\n", localVarName, HtmlConvertor.ToJSON(result));
		}
		protected Hashtable GetCreateButtonProperties(AppearanceSelectedStyle style, string elementID, object imageObjs) {
			string cssClass = style.CssClass;
			string styleAttributes = style.GetStyleAttributes(Page).Value;
			Hashtable result = new Hashtable();
			result["name"] = ClientID;
			result["element"] = elementID;
			result["className"] = cssClass != null ? new string[] { cssClass } : new string[] { "" };
			result["cssText"] = styleAttributes != null ? new string[] { styleAttributes } : new string[] { "" };
			result["postfixes"] = new string[] { "" };
			result["imageObjs"] = imageObjs is string ? new object[] { "" } : new object[] { imageObjs };
			result["imagePostfixes"] = new string[] { ButtonImageIdPostfix };
			return result;
		}
		protected void GetCreateDisabledClearBoxState(StringBuilder stb, string localVarName) {
			Hashtable properties = GetCreateButtonProperties(GetClearBoxDisabledCssStyle(), GetUploadClearBoxID(-1), GetClearButtonImage().GetDisabledScriptObject(Page));
			stb.AppendFormat("{0}.templateDisabledClearBoxItem = {1};\n", localVarName, HtmlConvertor.ToJSON(properties));
		}
		protected void GetCreateHoveredBrowseButtonState(StringBuilder stb, string localVarName) {
			Hashtable properties = GetCreateButtonProperties(GetBrowseButtonHoverCssStyle(), GetBrowseButtonID(-1), GetBrowseButtonImage().GetHottrackedScriptObject(Page));
			stb.AppendFormat("{0}.templateHoveredBrowseItem = {1};\n", localVarName, HtmlConvertor.ToJSON(properties));
		}
		protected void GetCreatePressedBrowseButtonState(StringBuilder stb, string localVarName) {
			Hashtable properties = GetCreateButtonProperties(GetBrowseButtonPressedCssStyle(), GetBrowseButtonID(-1), GetBrowseButtonImage().GetPressedScriptObject(Page));
			stb.AppendFormat("{0}.templatePressedBrowseItem = {1};\n", localVarName, HtmlConvertor.ToJSON(properties));
		}
		protected void GetCreateDisabledBrowseButtonState(StringBuilder stb, string localVarName) {
			Hashtable properties = GetCreateButtonProperties(GetBrowseButtonDisabledCssStyle(), GetBrowseButtonID(-1), GetBrowseButtonImage().GetDisabledScriptObject(Page));
			stb.AppendFormat("{0}.templateDisabledBrowseItem = {1};\n", localVarName, HtmlConvertor.ToJSON(properties));
		}
		protected void GetCreateDisabledRemoveButtonState(StringBuilder stb, string localVarName) {
			Hashtable properties = GetCreateButtonProperties(GetButtonDisabledCssStyle(), GetRemoveButtonID(-1), GetRemoveButtonImage().GetDisabledScriptObject(Page));
			stb.AppendFormat("{0}.templateDisabledRemoveItem = {1};\n", localVarName, HtmlConvertor.ToJSON(properties));
		}
		protected List<string> GetDialogTriggerIDList() {
			return GetCustomElementsIDList(DialogTriggerID);
		}
		protected List<string> GetExternalDropZoneIDList() {
			return GetCustomElementsIDList(AdvancedModeSettings.ExternalDropZoneID);
		}
		protected List<string> GetCustomElementsIDList(string ids) {
			List<string> result = new List<string>();
			foreach(string id in ids.Split(';')) {
				Control control = FindControlHelper.LookupControl(this, id);
				if(control != null)
					result.Add(control.ClientID);
				else
					result.Add(id);
			}
			return result;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxUploadControl), UploadScriptsResourcePath);
			RegisterAnimationScript();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ValidationSettings, AddButton, BrowseButton, RemoveButton, UploadButton, CancelButton, AdvancedModeSettings, ProgressBarSettings });
		}
		protected void RaiseFileUploadComplete(UploadedFile uploadedFile, ref string errorText, ref string callbackData) {
			FileUploadCompleteEventArgs args = new FileUploadCompleteEventArgs(callbackData, errorText, uploadedFile);
			OnFileUploadComplete(args);
			errorText = args.ErrorText;
			callbackData = args.CallbackData;
		}
		protected void RaiseFilesUploadComplete(ref string errorText, ref string callbackData) {
			FilesUploadCompleteEventArgs args = new FilesUploadCompleteEventArgs(callbackData, errorText);
			OnFilesUploadComplete(args);
			errorText = args.ErrorText;
			callbackData = args.CallbackData;
		}
		protected void Validate() {
			int validCount = 0;
			this.errorTexts = new List<string>();
			this.callbackDataArray = new List<string>();
			bool hasNonEmptyFiles = false;
			for(int i = 0; i < UploadedFiles.Length; i++) {
				UploadedFile file = UploadedFiles[i];
				string errorText = "";
				string callbackData = "";
				if(!file.IsEmpty()) {
					bool isValid = true;
					hasNonEmptyFiles = true;
					ValidateFileSize(ref isValid, ref errorText, i);
					ValidateContentType(ref isValid, ref errorText, i);
					ValidateFileExtension(ref isValid, ref errorText, i);
					file.IsValid = isValid;
					SyncronizeWithStorage(file);
					if(isValid) {
						try {
							RaiseFileUploadComplete(file, ref errorText, ref callbackData);
							validCount++;
						}
						catch(Exception e) {
							file.IsValid = false;
							errorText = HandleCallbackException(e);
						}
					}
				}
				else
					validCount++;
				this.errorTexts.Add(errorText);
				this.callbackDataArray.Add(callbackData);
			}
			if(hasNonEmptyFiles) {
				try {
					string commonErrorText = "";
					string commonCallbackData = "";
					RaiseFilesUploadComplete(ref commonErrorText, ref commonCallbackData);
					this.commonErrorText = commonErrorText;
					this.commonCallbackData = commonCallbackData;
				}
				catch(Exception e) {
					this.commonErrorText = HandleCallbackException(e);
				}
			}
			if(validCount < FileInputCount)
				IsValidInternal = false;
		}
		protected void ValidateContentType(ref bool isValid, ref string errorText, int index) {
			List<string> contentTypes = new List<string>(AllowedContentTypes);
			string contentType = UploadedFiles[index].ContentType;
			if(AllowedContentTypes.Length != 0 && !contentTypes.Contains(contentType)) {
				isValid = false;
				errorText = ValidationSettings.NotAllowedContentTypeErrorTextInternal;
			}
		}
		protected void ValidateFileExtension(ref bool isValid, ref string errorText, int index) {
			if(AllowedFileExtensions.Length == 0)
				return;
			List<string> fileExtensions = new List<string>(AllowedFileExtensions);
			string currentFileExtension = Path.GetExtension(UploadedFiles[index].FileName);
			if(!fileExtensions.Contains(currentFileExtension.ToLowerInvariant())) {
				isValid = false;
				errorText = ValidationSettings.NotAllowedFileExtensionErrorText;
			}
		}
		protected void ValidateFileSize(ref bool isValid, ref string errorText, int index) {
			if((MaxFileSize != 0) && (UploadedFiles[index].ContentLength > MaxFileSize)) {
				isValid = false;
				errorText = string.Format(ValidationSettings.MaxFileSizeErrorText, MaxFileSize);
			}
		}
		protected void ValidateValidationSettings() {
			int maxRequestLengthBytes = -1;
			try {
				maxRequestLengthBytes = HttpUtils.GetWebConfigMaxRequestLengthBytes();
			} catch { }
			if(maxRequestLengthBytes != -1) {
				if(UploadMode == UploadControlUploadMode.Standard) {
					if(ValidationSettings.MaxFileSize > 0 && maxRequestLengthBytes < ValidationSettings.MaxFileSize)
						throw new ArgumentException(StringResources.UploadControl_MaxFileSizeExceeded);
				} else {
					if(maxRequestLengthBytes < AdvancedModeSettings.PacketSize)
						throw new ArgumentException(StringResources.UploadControl_PacketSizeExceeded);
				}
			}
		}
		protected internal void ValidateFileInputCount() {
			if(ValidationSettings.MaxFileCount != 0 && FileInputCount > ValidationSettings.MaxFileCount)
				FileInputCount = ValidationSettings.MaxFileCount;
		}
		protected internal virtual string GetErrorText(int index) {
			string ret = "";
			if(index != -1) {
				if(DesignMode)
					ret = StringResources.UploadControl_SampleErrorText;
				else
					if(!IsValidInternal)
						ret = !string.IsNullOrEmpty(ErrorTexts[index]) ? ErrorTexts[index] :
							ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_EnctypeError);
			}
			return ret;
		}
		protected internal bool GetIsValid(int index) {
			bool ret = true;
			if(index != -1) {
				if(UploadedFiles == null)
					ret = !PreviewErrorInDesigner;
				else if(index < UploadedFiles.Length)
					ret = UploadedFiles[index].IsValid;
			}
			return ret;
		}
		protected internal string GetPlatformErrorText() {
			return IsUploadModeSupported()
				? string.Format(ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_PlatformErrorText), HttpUtility.HtmlEncode(SilverlightPluginLink))
				: ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_UploadModeNotSupported);
		}
		protected internal bool GetIsShowErrorRow(int index) {
			return DesignMode ? PreviewErrorInDesigner : ValidationSettings.ShowErrors;
		}
		protected internal string GetInputIdPostfix(int index) {
			return index != -1 ? index.ToString() : TemplateInputIdPostfix;
		}
		protected internal string GetClearButtonImageID(int index) {
			return GetUploadClearBoxID(index) + ButtonImageIdPostfix;
		}
		protected internal string GetButtonImageID(UploadControlButtonPropertiesBase buttonProperties, int index) {
			if(buttonProperties is BrowseButtonProperties)
				return GetBrowseButtonID(index) + ButtonImageIdPostfix;
			if(buttonProperties is RemoveButtonProperties)
				return GetRemoveButtonID(index) + ButtonImageIdPostfix;
			return buttonProperties.GetButtonIDSuffix() + ButtonImageIdPostfix;
		}
		protected internal string GetBrowseButtonID(int index) {
			return string.Format("{0}{1}", BrowseButton.GetButtonIDSuffix(), GetInputIdPostfix(index));
		}
		protected internal string GetRemoveButtonID(int index) {
			return string.Format("{0}{1}", RemoveButton.GetButtonIDSuffix(), GetInputIdPostfix(index));
		}
		protected internal string GetErrorRowID(int index) {
			return "ErrR" + (index != -1 ? index.ToString() : TemplateErrorRowIdPostfix);
		}
		protected internal static string GetProgressInfoKey(HttpRequest request) {
			return request.QueryString[RenderUtils.ProgressInfoQueryParamName];
		}
		protected internal static string GetUploadingCallbackParam(HttpRequest request) {
			return request.QueryString[RenderUtils.UploadingCallbackQueryParamName];
		}
		protected internal static string GetHelperUploadingCallbackParam(HttpRequest request) {
			return request.QueryString[RenderUtils.HelperUploadingCallbackQueryParamName];
		}
		protected internal string GetUploadIframeID() {
			return "UploadIframe";
		}
		protected internal string GetUploadIframeSrc() {
			if(Browser.IsIE) {
				if(Browser.MajorVersion < 8)
					return GetSSLSecureBlankUrl();
				return SSLSecureBlankUrlForIE;
			}
			return SSLSecureBlankUrl;
		}
		protected internal string GetUploadIframeName() {
			return ClientID + "_" + GetUploadIframeID();
		}
		protected internal string GetUploadTextBoxCellId(int index) {
			return "TextBox" + GetInputIdPostfix(index);
		}
		protected internal string GetUploadClearBoxID(int index) {
			return string.Format("ClearBox{0}", GetInputIdPostfix(index));
		}
		protected internal string GetUploadInputId(int index) {
			return GetUploadTextBoxCellId(index) + InputIdPostfix;
		}
		protected internal string GetUploadFakeInputId(int index) {
			return GetUploadTextBoxCellId(index) + FakeInputIdPostfix;
		}
		protected internal string GetUploadInputName(int index) {
			return this.ClientID + "_" + GetUploadInputId(index);
		}
		protected internal string GetFileInputRowId(int index) {
			return InputRowIdPostfix + GetInputIdPostfix(index);
		}
		protected internal string GetAddUploadButtonsSeparatorRowID() {
			return "AddUploadR";
		}
		protected internal string GetAddUploadButtonsPanelRowID() {
			return "AddUploadPanelR";
		}
		protected internal string GetCommonErrorDivID() {
			return "CErr";
		}
		protected internal string GetPlatformErrorTableID() {
			return "PlatformErrorPanel";
		}
		protected internal string GetUploadInputsTableID() {
			return "UploadInputs";
		}
		protected internal string GetProgressPanelTableID() {
			return "ProgressPanel";
		}
		protected internal string GetProgressBarControlID() {
			return "UCProgress";
		}
		protected internal string GetUploadingCancelRowID() {
			return "UploadingCancelR";
		}
		protected internal string GetInlineDropZoneDivID() {
			return "InlineDropZone";
		}
		protected internal string GetFileListRowTemplateID() {
			return "FileRT";
		}
		protected internal string GetFileListTableID() {
			return "FileList";
		}
		protected internal string GetOnUploadButtonClick() {
			return string.Format(UploadFileInputHandlerName, ClientID);
		}
		protected internal string GetClearButtonTooltip() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_ClearFileSelectionButtonToolTip);
		}
		protected virtual void OnFileUploadComplete(FileUploadCompleteEventArgs e) {
			EventHandler<FileUploadCompleteEventArgs> handler =
				Events[EventFileUploadComplete] as EventHandler<FileUploadCompleteEventArgs>;
			if(handler != null)
				handler(this, e);
		}
		protected virtual void OnFilesUploadComplete(FilesUploadCompleteEventArgs e) {
			EventHandler<FilesUploadCompleteEventArgs> handler =
				Events[EventFilesUploadComplete] as EventHandler<FilesUploadCompleteEventArgs>;
			if(handler != null)
				handler(this, e);
		}
		protected internal AddUploadButtonsHorizontalPosition GetAddUploadButtonsHorizontalPosition() {
			return AddUploadButtonsHorizontalPosition;
		}
		protected internal CancelButtonHorizontalPosition GetCancelButtonHorizontalPosition() {
			return CancelButtonHorizontalPosition;
		}
		protected internal Unit GetFileInputSpacing() {
			return FileInputSpacing.IsEmpty ? Styles.GetDefaultInputSpacing() : FileInputSpacing;
		}
		protected internal Unit GetButtonSpacing() {
			return ButtonSpacing.IsEmpty ? Styles.GetDefaultButtonSpacing() : ButtonSpacing;
		}
		protected internal Unit GetRemoveButtonSpacing() {
			return RemoveButtonSpacing.IsEmpty ? Styles.GetDefaultRemoveButtonSpacing() : RemoveButtonSpacing;
		}
		protected internal Unit GetAddUploadButtonsSpacing() {
			return AddUploadButtonsSpacing.IsEmpty ? Styles.GetDefaultAddUploadButtonsSpacing() : AddUploadButtonsSpacing;
		}
		protected internal Unit GetButtonImageSpacing() {
			return GetButtonStyle().ImageSpacing;
		}
		protected internal Unit GetCancelButtonsSpacing() {
			return CancelButtonSpacing.IsEmpty ? Styles.GetDefaultCancelButtonSpacing() : CancelButtonSpacing;
		}
		protected internal Unit GetUploadingDesignSpacing() {
			return Unit.Pixel(10);
		}
		protected internal int GetProgressBarDesignValue() {
			return 50;
		}
		protected internal Unit GetProgressWidth() {
			return Unit.Percentage(100);
		}
		protected internal Unit GetProgressHeight() {
			return Styles.ProgressBar.Height;
		}
		protected internal Unit GetInlineDropZoneWidth() {
			if(Styles.DropZone.Width.IsEmpty)
				return Width;
			return Styles.DropZone.Width;
		}
		protected internal Unit GetInlineDropZoneHeight() {
			if (Styles.DropZone.Height.IsEmpty)
				return Height;
			return Styles.DropZone.Height;
		}
		protected internal Paddings GetTextBoxPaddings() {
			return GetTextBoxStyle().Paddings;
		}
		protected internal Unit GetTextBoxWidth() {
			return Styles.BrowseButton.Width.IsEmpty ? Unit.Percentage(100) : Unit.Empty;
		}
		protected internal Paddings GetBrowseButtonPaddings() {
			return GetBrowseButtonStyle().Paddings;
		}
		protected internal Paddings GetButtonPaddings() {
			return GetButtonStyle().Paddings;
		}
		protected internal bool IsBorderSeparate() {
			return true;
		}
		protected internal bool IsClearFileSelectionButtonVisible() {
			return DesignMode ? ShowClearFileSelectionButton : false;
		}
		protected internal bool IsCommonErrorDivVisible() {
			return DesignMode ? PreviewErrorInDesigner : ValidationSettings.ShowErrors;
		}
		protected internal bool IsUploadButtonVisible() {
			bool ret = ShowUploadButton;
			if(ShowUploadButton)
				ret = (FileInputCount > 0 || ShowAddRemoveButtons) && !AutoStartUpload;
			return ret;
		}
		protected internal bool IsControlVisible() {
			return ShowAddRemoveButtons || FileInputCount > 0;
		}
		protected internal bool IsVisibleUploadingPanel() {
			return ShowProgressPanel && DesignMode;
		}
		protected internal bool IsFileListEnabled() {
			return AdvancedModeSettings.EnableFileList && ShowUI;
		}
		protected internal bool NeedCreateStateObjects() {
			return ShowUI && !IsNativeRender();
		}
		protected UploadControlImages Images {
			get { return (UploadControlImages)ImagesInternal; }
		}
		protected internal ImagePropertiesBase GetButtonImageInternal() {
			ImagePropertiesBase image = new ImagePropertiesBase();
			image.SpriteUrl = ImagesInternal.SpriteImageUrl;
			return image;
		}
		protected internal ImagePropertiesBase GetAddButtonImage() {
			ImagePropertiesBase image = GetButtonImageInternal();
			image.CopyFrom(AddButton.Image);
			return image;
		}
		protected internal ImagePropertiesBase GetBrowseButtonImage() {
			ImagePropertiesBase image = GetButtonImageInternal();
			image.CopyFrom(BrowseButton.Image);
			return image;
		}
		protected internal ImagePropertiesBase GetUploadButtonImage() {
			ImagePropertiesBase image = GetButtonImageInternal();
			image.CopyFrom(UploadButton.Image);
			return image;
		}
		protected internal ImagePropertiesBase GetCancelButtonImage() {
			ImagePropertiesBase image = GetButtonImageInternal();
			image.CopyFrom(CancelButton.Image);
			return image;
		}
		protected internal ImagePropertiesBase GetClearButtonImage() {
			ImagePropertiesBase image = GetButtonImageInternal();
			image.ToolTip = GetClearButtonTooltip();
			image.CopyFrom(Images.GetImageProperties(Page, UploadControlImages.ClearButtonImageName));
			image.CopyFrom(Images.ClearButtonImage);
			return image;
		}
		protected internal ImagePropertiesBase GetRemoveButtonImage() {
			ImagePropertiesBase image = GetButtonImageInternal();
			image.CopyFrom(RemoveButton.Image);
			return image;
		}
		protected internal ImagePropertiesBase GetFileListItemPendingImage() {
			ImagePropertiesEx image = new ImagePropertiesEx();
			image.CopyFrom(Images.FileListItemPendingImage);
			return image;
		}
		protected internal ImagePropertiesBase GetFileListItemUploadingImage() {
			ImagePropertiesEx image = new ImagePropertiesEx();
			image.CopyFrom(Images.FileListItemUploadingImage);
			return image;
		}
		protected internal ImagePropertiesBase GetFileListItemCompleteImage() {
			ImagePropertiesEx image = new ImagePropertiesEx();
			image.CopyFrom(Images.FileListItemCompleteImage);
			return image;
		}
		protected internal AppearanceStyleBase GetTextBoxNullBaseStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultTextBoxStyle();
			ret.CopyFrom(Styles.GetDefaultNullTextStyle());
			ret.CopyFrom(NullTextStyle);
			if(ret.Height.IsEmpty)
				ret.Height = TextBoxStyle.Height;
			if(IsBorderSeparate()) {
				if(IsRightToLeft())
					ret.BorderLeft.BorderWidth = Unit.Pixel(0);
				else
					ret.BorderRight.BorderWidth = Unit.Pixel(0);
			}
			return ret;
		}
		protected internal AppearanceStyleBase GetTextBoxNullStyle() {
			AppearanceStyleBase ret = GetTextBoxNullBaseStyle();
			if(ShowClearFileSelectionButton) {
				if(IsRightToLeft())
					ret.BorderLeft.BorderWidth = Unit.Pixel(0);
				else
					ret.BorderRight.BorderWidth = Unit.Pixel(0);
			}
			ret.Width = DesignMode ? Unit.Percentage(100) : GetTextBoxWidth();
			return ret;
		}
		protected internal AppearanceStyleBase GetEditAreaNullStyle() {
			return GetEditAreaStyleInternal(false, true);
		}
		protected internal AppearanceStyleBase GetClearBoxNullStyle() {
			AppearanceStyleBase ret = GetTextBoxNullBaseStyle();
			if(IsRightToLeft())
				ret.BorderRight.BorderWidth = Unit.Pixel(0);
			else
				ret.BorderLeft.BorderWidth = Unit.Pixel(0);
			ret.CssClass = RenderUtils.CombineCssClasses(ret.CssClass, "dxCB");
			return ret;
		}
		protected internal AppearanceStyleBase GetTextBoxBaseStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultTextBoxStyle();
			ret.CopyFrom(TextBoxStyle);
			MergeDisableStyle(ret, IsEnabled(), GetTextBoxDisabledStyle(), false);
			if(IsBorderSeparate()) {
				if(IsRightToLeft())
					ret.BorderLeft.BorderWidth = Unit.Pixel(0);
				else
					ret.BorderRight.BorderWidth = Unit.Pixel(0);
			}
			return ret;
		}
		protected internal AppearanceStyleBase GetTextBoxStyle() {
			AppearanceStyleBase ret = GetTextBoxBaseStyle();
			if(ShowClearFileSelectionButton) {
				if(IsRightToLeft())
					ret.BorderLeft.BorderWidth = Unit.Pixel(0);
				else
					ret.BorderRight.BorderWidth = Unit.Pixel(0);
			}
			ret.Width = Unit.Percentage(100);
			return ret;
		}
		protected internal AppearanceStyleBase GetTextBoxDisabledStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultTextBoxDisabledStyle();
			ret.CopyFrom(TextBoxStyle.DisabledStyle);
			return ret;
		}
		protected internal AppearanceStyleBase GetClearBoxStyle() {
			AppearanceStyleBase ret = GetTextBoxBaseStyle();
			if(IsRightToLeft())
				ret.BorderRight.BorderWidth = Unit.Pixel(0);
			else
				ret.BorderLeft.BorderWidth = Unit.Pixel(0);
			return ret;
		}
		protected internal AppearanceStyleBase GetClearBoxDisabledStyle() {
			return GetTextBoxDisabledStyle();
		}
		protected internal AppearanceStyleBase GetBrowseButtonStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultBrowseButtonStyle();
			ret.CopyFrom(BrowseButtonStyle);
			MergeDisableStyle(ret, IsEnabled(), GetBrowseButtonDisabledStyle(), false);
			return ret;
		}
		protected internal AppearanceStyleBase GetBrowseButtonHoverStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultBrowseButtonHoverStyle();
			ret.CopyFrom(BrowseButtonStyle.HoverStyle);
			return ret;
		}
		protected internal AppearanceStyleBase GetBrowseButtonPressedStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultBrowseButtonPressedStyle();
			ret.CopyFrom(BrowseButtonStyle.PressedStyle);
			return ret;
		}
		protected internal AppearanceStyleBase GetBrowseButtonDisabledStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultBrowseButtonDisabledStyle();
			ret.CopyFrom(BrowseButtonStyle.DisabledStyle);
			return ret;
		}
		protected internal AppearanceStyleBase GetButtonStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultButtonStyle();
			ret.CopyFrom(ButtonStyle);
			MergeDisableStyle(ret, IsEnabled(), GetButtonDisabledStyle(), false);
			return ret;
		}
		protected internal AppearanceStyleBase GetButtonDisabledStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultButtonDisabledStyle();
			ret.CopyFrom(ButtonStyle.DisabledStyle);
			return ret;
		}
		protected internal AppearanceStyleBase GetErrorCellStyle() {
			AppearanceStyleBase ret = Styles.GetDefaultErrorCellStyle();
			ret.CopyFrom(ValidationSettings.ErrorStyle);
			return ret;
		}
		protected internal AppearanceStyleBase GetEditAreaStyle(bool isNativeInput) {
			return GetEditAreaStyleInternal(isNativeInput, false);
		}
		protected internal AppearanceStyleBase GetEditAreaStyleInternal(bool isNativeInput, bool isNullStyle) {
			AppearanceStyleBase ret = Styles.GetDefaultEditAreaStyle();
			AppearanceStyleBase textBoxStyle = isNullStyle ? GetTextBoxNullStyle() : GetTextBoxStyle();
			MergeControlStyleForInput(ret);
			ret.CopyFontFrom(textBoxStyle);
			if(!isNullStyle)
				MergeDisableStyle(ret, IsEnabled(), GetTextBoxDisabledStyle(), false);
			ret.Width = !Width.IsEmpty ? Unit.Percentage(100) : Unit.Empty;
			if(!isNativeInput) {
				ret.BackColor = System.Drawing.Color.Transparent;
				ret.BorderWidth = Unit.Pixel(0);
			}
			if(!string.IsNullOrEmpty(textBoxStyle.Cursor))
				ret.Cursor = textBoxStyle.Cursor;
			return ret;
		}
		protected internal DisabledStyle GetTextBoxDisabledCssStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledStyle());
			style.CopyFrom(GetTextBoxDisabledStyle());
			return style;
		}
		protected internal DisabledStyle GetClearBoxDisabledCssStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledStyle());
			style.CopyFrom(GetClearBoxDisabledStyle());
			return style;
		}
		protected internal AppearanceSelectedStyle GetBrowseButtonHoverCssStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(GetBrowseButtonHoverStyle());
			return style;
		}
		protected internal AppearanceSelectedStyle GetBrowseButtonPressedCssStyle() {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(GetBrowseButtonPressedStyle());
			return style;
		}
		protected internal DisabledStyle GetBrowseButtonDisabledCssStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledStyle());
			style.CopyFrom(GetBrowseButtonDisabledStyle());
			return style;
		}
		protected internal DisabledStyle GetButtonDisabledCssStyle() {
			DisabledStyle style = new DisabledStyle();
			style.CopyFrom(GetDisabledStyle());
			style.CopyFrom(GetButtonDisabledStyle());
			return style;
		}
		protected internal ProgressStyle GetProgressStyle() {
			ProgressStyle style = new ProgressStyle();
			style.CopyFrom(Styles.GetDefaultProgressStyle());
			style.CopyFrom(ProgressBarStyle);
			return style;
		}
		protected internal ProgressBarIndicatorStyle GetProgressBarIndicatorStyle() {
			ProgressBarIndicatorStyle style = new ProgressBarIndicatorStyle();
			style.CopyFrom(Styles.GetDefaultProgressBarIndicatorStyle());
			style.CopyFrom(ProgressBarIndicatorStyle);
			return style;
		}
		protected internal AppearanceStyleBase GetInputsTableStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultInputsTableStyle());
			return style;
		}
		protected internal AppearanceStyleBase GetPlatformErrorPanelStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(Styles.GetDefaultPlatformErrorPanelStyle());
			return style;
		}
		protected internal UploadControlDropZoneStyle GetInlineDropZoneStyle() {
			UploadControlDropZoneStyle style = new UploadControlDropZoneStyle();
			style.CopyFrom(Styles.GetDefaultInlineDropZoneStyle());
			style.CopyFrom(DropZoneStyle);
			return style;
		}
		protected internal UploadControlFileListItemStyle GetFileListItemStyle() {
			return Styles.FileListItemStyle;
		}
		protected internal UploadControlFileListItemStyle CreateFileListItemStyle() {
			UploadControlFileListItemStyle style = new UploadControlFileListItemStyle();
			style.CopyFrom(Styles.GetDefaultFileListItemStyle());
			style.CopyFrom(Styles.FileListItemStyle);
			return style;
		}
		protected void PerformUpload() {
			Validate();
			FinishCallbackUpload();
		}
		protected void FinishCallbackUpload() {
			if(!(IsFileUploadingOnCallback() || IsHelperFileUploadingOnCallback()))
				return;
			this.uploaded = true;
			string responseString = HtmlEncode(GetResponseString());
			if(RenderUtils.Browser.IsIE && RenderUtils.Browser.Version == 8) 
				responseString = ProtectWhitespaceSeries(responseString);
			HttpUtils.WriteToResponse(responseString);
			HttpUtils.EndResponse();
		}
		protected void EnsureUploaded() {
			SynchronizeInputCount();
			if(this.uploaded || !IsUploadAllow())
				return;
			CreatePostedFiles();
			if(UploadedFiles != null)
				PerformUpload();
			else {
				ErrorText = ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_EnctypeError);
				IsValidInternal = false;
				FinishCallbackUpload();
			}
		}
		protected void CreatePostedFiles() {
			this.uploadedFiles = new List<UploadedFile>();
			if(IsHelperFileUploadingOnCallback()) {
				PartialUploadHelperBase uploadHelper = FindUploadHelper();
				foreach(HelperPostedFileBase helperFile in uploadHelper.Files.Values) {
					UploadedFile uploadedFile = new UploadedFile(helperFile);
					this.uploadedFiles.Add(uploadedFile);
				}
			}
			else {
				for(int i = 0; i < FileInputCount; i++) {
					UploadedFile uploadedFile = new UploadedFile(Request.Files[GetUploadInputName(i)]);
					this.uploadedFiles.Add(uploadedFile);
				}
			}
		}
		protected PartialUploadHelperBase FindUploadHelper() {
			string key = Request[RenderUtils.ProgressHandlerKeyQueryParamName];
			return HelperUploadManager.FindUploadHelper(key);
		}
		protected void SynchronizeInputCount() {
			if(ClientObjectState == null)
				return;
			FileInputCount = GetClientObjectStateValue<int>(InputCountStateKey);
		}
		protected void SyncronizeWithStorage(UploadedFile file) {
			if(IsDirectUpload()) {
				if(file.IsValid)
					CommitFileUpload(file);
				else
					AbortFileUpload(file);
			}
		}
		protected void CommitFileUpload(UploadedFile file) {
			if(IsHelperFileUploadingOnCallback())
				file.CommitChunkedUpload();
			else {
				UploadInternalSettingsBase uploadSettings = CreateUploadSettings();
				HelperPostedFileBase postedFile = uploadSettings.CreatePostedFile(file.FileName);
				postedFile.InternalFileName = GetFileNameInStorage(file.FileName);
				postedFile.SetContent(file.FileContent);
				file.FileNameInStorage = postedFile.InternalFileName;
			}
		}
		protected void AbortFileUpload(UploadedFile file) {
			if(IsHelperFileUploadingOnCallback())
				file.AbortChunkedUpload();
		}
		protected bool IsDirectUpload() {
			if(IsHelperFileUploadingOnCallback()) {
				PartialUploadHelperBase helper = FindUploadHelper();
				return helper.UploadSettings.IsDirectUpload();
			}
			return UploadStorage != UploadControlUploadStorage.NotSet;
		}
		protected internal static string GetFileNameInStorage(string fileName) {
			string namePrefix = Guid.NewGuid().ToString();
			return namePrefix + "_" + fileName;
		}
		protected string GetUploadSettingsID() {
			UploadInternalSettingsBase settings = CreateUploadSettings();
			return UploadSecurityHelper.GetUploadSettingsID(settings);
		}
		protected UploadInternalSettingsBase CreateUploadSettings() {
			switch(UploadStorage) {
				case UploadControlUploadStorage.NotSet:
					return new UploadDefaultInternalSettings(ValidationSettings.MaxFileSize, ValidationSettings.MaxFileCount,
						AdvancedModeSettings.TemporaryFolder);
				case UploadControlUploadStorage.FileSystem:
					return new UploadFileSystemInternalSettings(ValidationSettings.MaxFileSize, ValidationSettings.MaxFileCount,
						AdvancedModeSettings.TemporaryFolder, FileSystemSettings.UploadFolder);
				case UploadControlUploadStorage.Azure:
					return new UploadAzureInternalSettings(ValidationSettings.MaxFileSize, ValidationSettings.MaxFileCount,
						AzureSettings.StorageAccountName, AzureSettings.AccessKey, AzureSettings.ContainerName);
				case UploadControlUploadStorage.Amazon:
					return new UploadAmazonInternalSettings(ValidationSettings.MaxFileSize, ValidationSettings.MaxFileCount,
						AmazonSettings.AccessKeyID, AmazonSettings.SecretAccessKey, AmazonSettings.BucketName, AmazonSettings.Region);
				case UploadControlUploadStorage.Dropbox:
					return new UploadDropboxInternalSettings(ValidationSettings.MaxFileSize, ValidationSettings.MaxFileCount,
						DropboxSettings.AccessTokenValue, DropboxSettings.UploadFolder);
			}
			throw new Exception("UploadStorage property is set to the incorrect value");
		}
		protected string GetSignature() {
			return UploadSecurityHelper.GetSignature(UploadingKey, GetUploadSettingsID(), ClientID);
		}
		protected internal static string GetNewUploadingKey() {
			return Guid.NewGuid().ToString("N");
		}
		protected void CorrectResponseCachePolicy() {
			HttpRequest request = HttpUtils.GetRequest();
			if(request != null && request.Browser.Browser == "Firefox")
				HttpUtils.GetResponse().Cache.SetNoStore();
		}
		protected void CheckTemporaryFolderAccess() {
			if(IsAdvancedOrAutoUploadMode())
				FileUtils.CheckOrCreateDirectory(AdvancedModeSettings.TemporaryFolder, this, "AdvancedModeSettings.TemporaryFolder");
		}
		protected string GetProgressHandlerPage() {
			string handlerPage = "/" + RenderUtils.ProgressHandlerPage;
			if(Request != null && !string.Equals(Request.ApplicationPath, "/"))
				handlerPage = Request.ApplicationPath + handlerPage;
			return handlerPage;
		}
		protected string GetResponseString() {
			Hashtable result = new Hashtable();
			IDictionary<string, object> properties = GetCustomJSProperties();
			if(properties != null) {
				try {
					CheckCustomJSProperties(properties);
					result[CallbackResultProperty.CustomJSProperties] = properties;
				} catch(Exception e) {
					this.commonErrorText = e.Message;
				}
			}
			result[CallbackResultProperty.ErrorTexts] = ErrorTexts;
			result[CallbackResultProperty.CallbackDataArray] = CallbackDataArray;
			result[CallbackResultProperty.CommonErrorText] = CommonErrorText;
			result[CallbackResultProperty.CommonCallbackData] = CommonCallbackData;
			List<bool> isValidArray = new List<bool>();
			for(int i = 0; i < UploadedFiles.Length; i++)
				isValidArray.Add(UploadedFiles[i].IsValid);
			result[CallbackResultProperty.IsValidArray] = isValidArray.ToArray();
			return HtmlConvertor.ToJSON(result);
		}
		protected internal bool IsNullTextEnabled() {
			return !IsNativeRender() && !string.IsNullOrEmpty(NullText);
		}
		protected internal virtual bool IsUploadProcessingEnabled() {
			return ShowProgressPanel || !string.IsNullOrEmpty(ClientSideEvents.UploadingProgressChanged);
		}
		protected bool IsFileUploadingOnCallback() {
			if(Request != null)
				return GetUploadingCallbackParam(Request) == ClientID;
			return false;
		}
		protected bool IsHelperFileUploadingOnCallback() {
			if(Request != null)
				return GetHelperUploadingCallbackParam(Request) == ClientID;
			return false;
		}
		protected bool IsAdvancedOrAutoUploadMode() {
			return UploadMode == UploadControlUploadMode.Advanced || UploadMode == UploadControlUploadMode.Auto;
		}
		protected virtual bool IsUploadAllow() {
			bool isAllow = !DesignMode && (Page != null) && Page.IsPostBack && !Page.IsCallback;
			if(IsFileUploadingOnCallback()) {
				isAllow = isAllow && (Request.Files.Count > 0)
					&& (Request.Files[GetUploadInputName(0)] != null); 
			}
			else if(IsHelperFileUploadingOnCallback())
				isAllow = isAllow && FindUploadHelper() != null;
			return isAllow;
		}
		protected bool IsUploadModeSupported() {
			return !(UploadMode == UploadControlUploadMode.Advanced && 
						((Browser.Platform.IsMacOSMobile || Browser.Platform.IsAndroidMobile) 
						|| (Browser.IsSafari && Browser.MajorVersion == 5)));
		}
		protected string ProtectWhitespaceSeries(string text) {
			return text.Replace("&nbsp;", "&nbspx;").Replace(" ", "&nbsp;");
		}
		protected internal static string GetDefaultDropZoneText() {
			return ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_DropZone);
		}
		protected internal bool IsAnyExtensionAllowed() {
			return Array.IndexOf(ValidationSettings.AllowedFileExtensions, ".*") > -1;
		}
		protected string HandleCallbackException(Exception ex) {
			if(IsFileUploadingOnCallback() || IsHelperFileUploadingOnCallback())
				return ProcessCallbackException(ex);
			return ex.Message;
		}
		protected internal static string HandleCallbackExceptionInternal(Exception ex) {
			return ProcessCallbackExceptionInternal(ex, null);
		}
		protected override bool IsNativeSupported() {
			return true;
		}
		internal Unit GetFileListProgressWidth() {
			return Unit.Percentage(100);
		}
		internal Unit GetFileListProgressHeight() {
			return Unit.Pixel(6);
		}
		internal string GetFileListProgressDefaultID() {
			return "FL_Progress";
		}
	}
	public class UploadProgressBarSettings : ProgressBarSettings {
		public UploadProgressBarSettings(ASPxUploadControl upload)
			: base(upload) {
		}
	}
	public class UploadAdvancedModeSettings : PropertiesBase, IPropertiesOwner {
		private ImagePropertiesBase fileListItemPendingImageInternal = null;
		private ImagePropertiesBase fileListItemCompleteImageInternal = null;
		public UploadAdvancedModeSettings()
			: base() {
		}
		public UploadAdvancedModeSettings(IPropertiesOwner owner)
			: base(owner) {
				this.fileListItemPendingImageInternal = new ImagePropertiesBase();
				this.fileListItemCompleteImageInternal = new ImagePropertiesBase();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadAdvancedModeSettingsEnableMultiSelect"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable, Localizable(false),
		RefreshProperties(RefreshProperties.Repaint)]
		public virtual bool EnableMultiSelect {
			get { return GetBoolProperty("EnableMultiSelect", false); }
			set {
				SetBoolProperty("EnableMultiSelect", false, value);
				if(value && UploadControl != null)
					ForceAdvancedUploadMode();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadAdvancedModeSettingsPacketSize"),
#endif
		DefaultValue(ASPxUploadControl.DefaultPacketSizeValue), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public int PacketSize
		{
			get { return GetIntProperty("PacketSize", ASPxUploadControl.DefaultPacketSizeValue); }
			set
			{
				CommonUtils.CheckValueRange(value, 1, int.MaxValue, "PacketSize");
				SetIntProperty("PacketSize", ASPxUploadControl.DefaultPacketSizeValue, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadAdvancedModeSettingsTemporaryFolder"),
#endif
		DefaultValue(ASPxUploadControl.DefaultTemporaryFolder), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public string TemporaryFolder
		{
			get { return GetStringProperty("TemporaryFolder", ASPxUploadControl.DefaultTemporaryFolder); }
			set { SetStringProperty("TemporaryFolder", ASPxUploadControl.DefaultTemporaryFolder, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadAdvancedModeSettingsEnableDragAndDrop"),
#endif
		DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public virtual bool EnableDragAndDrop {
			get { return GetBoolProperty("EnableDragAndDrop", GetEnableDragAndDropDefaultValue()); }
			set { 
				SetBoolProperty("EnableDragAndDrop", GetEnableDragAndDropDefaultValue(), value);
				if(value && UploadControl != null)
					ForceAdvancedUploadMode();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadAdvancedModeSettingsExternalDropZoneID"),
#endif
		DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public string ExternalDropZoneID {
			get { return GetStringProperty("ExternalDropZoneID", ""); }
			set { SetStringProperty("ExternalDropZoneID", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadAdvancedModeSettingsDropZoneText"),
#endif
		DefaultValue(ASPxUploadControl.DesignerDefaultDropZoneText), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public string DropZoneText {
			get { return GetStringProperty("DropZoneText", ASPxUploadControl.GetDefaultDropZoneText()); }
			set { SetStringProperty("DropZoneText", ASPxUploadControl.GetDefaultDropZoneText(), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadAdvancedModeSettingsFileListPosition"),
#endif
		DefaultValue(UploadControlFileListPosition.Bottom), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public UploadControlFileListPosition FileListPosition {
			get { return (UploadControlFileListPosition)GetEnumProperty("FileListPosition", UploadControlFileListPosition.Bottom); }
			set { SetEnumProperty("FileListPosition", UploadControlFileListPosition.Bottom, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadAdvancedModeSettingsEnableFileList"),
#endif
		Category("Settings"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public bool EnableFileList {
			get { return GetBoolProperty("EnableFileList", false); }
			set {
				SetBoolProperty("EnableFileList", false, value);
				if(value && UploadControl != null)
					UploadControl.AdvancedModeSettings.EnableMultiSelect = true;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadAdvancedModeSettingsFileListItemStyle"),
#endif
		Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public UploadControlFileListItemStyle FileListItemStyle {
			get {
				if(UploadControl != null)
					return UploadControl.GetFileListItemStyle();
				return null;
			}
		}
		protected internal ImagePropertiesBase FileListItemUploadingImage {
			get {
				if(UploadControl != null)
					return UploadControl.GetFileListItemUploadingImage();
				return null; 
			}
		}
		protected internal ImagePropertiesBase FileListItemPendingImage {
			get {
				if(UploadControl != null)
					return UploadControl.GetFileListItemPendingImage();
				return null;  
			}
		}
		protected internal ImagePropertiesBase FileListItemCompleteImage {
			get {
				if(UploadControl != null)
					return UploadControl.GetFileListItemCompleteImage();
				return null;  
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				if(source is UploadAdvancedModeSettings) {
					UploadAdvancedModeSettings settings = source as UploadAdvancedModeSettings;
					EnableMultiSelect = settings.EnableMultiSelect;
					EnableDragAndDrop = settings.EnableDragAndDrop;
					EnableFileList = settings.EnableFileList;
					FileListPosition = settings.FileListPosition;
					ExternalDropZoneID = settings.ExternalDropZoneID;
					DropZoneText = settings.DropZoneText;
					PacketSize = settings.PacketSize;
					TemporaryFolder = settings.TemporaryFolder;
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal ASPxUploadControl UploadControl {
			get { return Owner as ASPxUploadControl; }
		}
		protected virtual bool GetEnableDragAndDropDefaultValue() {
			return false;
		}
		protected void ForceAdvancedUploadMode() {
			if(UploadControl.ShowAddRemoveButtons) {
				UploadControl.ShowAddRemoveButtons = false;
				UploadControl.PropertyChanged("ShowAddRemoveButtons");
			}
			if(UploadControl.UploadMode == UploadControlUploadMode.Standard) {
				UploadControl.UploadMode = UploadControlUploadMode.Auto;
				UploadControl.PropertyChanged("UploadMode");
			}
			if(UploadControl.FileInputCount != 1) {
				UploadControl.FileInputCount = 1;
				UploadControl.PropertyChanged("FileInputCount");
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class UploadControlAzureSettings : PropertiesBase, IPropertiesOwner {
		public UploadControlAzureSettings() : base() {
			InitializeSettings();
		}
		public UploadControlAzureSettings(IPropertiesOwner owner) : base(owner) {
			InitializeSettings();
		}
		void InitializeSettings() {
			StorageAccountName = string.Empty;
			AccessKey = string.Empty;
			ContainerName = string.Empty;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlAzureSettingsStorageAccountName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string StorageAccountName { get; set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlAzureSettingsAccessKey"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string AccessKey { get; set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlAzureSettingsContainerName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string ContainerName { get; set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			UploadControlAzureSettings settings = source as UploadControlAzureSettings;
			if(settings != null) {
				StorageAccountName = settings.StorageAccountName;
				AccessKey = settings.AccessKey;
				ContainerName = settings.ContainerName;
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class UploadControlAmazonSettings : PropertiesBase, IPropertiesOwner {
		public UploadControlAmazonSettings() : base() {
			InitializeSettings();
		}
		public UploadControlAmazonSettings(IPropertiesOwner owner) : base(owner) {
			InitializeSettings();
		}
		void InitializeSettings() {
			AccessKeyID = string.Empty;
			SecretAccessKey = string.Empty;
			BucketName = string.Empty;
			Region = string.Empty;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlAmazonSettingsAccessKeyID"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string AccessKeyID { get; set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlAmazonSettingsSecretAccessKey"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string SecretAccessKey { get; set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlAmazonSettingsBucketName"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string BucketName { get; set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlAmazonSettingsRegion"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string Region { get; set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			UploadControlAmazonSettings settings = source as UploadControlAmazonSettings;
			if(settings != null) {
				AccessKeyID = settings.AccessKeyID;
				SecretAccessKey = settings.SecretAccessKey;
				BucketName = settings.BucketName;
				Region = settings.Region;
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class UploadControlDropboxSettings : PropertiesBase, IPropertiesOwner {
		public UploadControlDropboxSettings() : base() {
			InitializeSettings();
		}
		public UploadControlDropboxSettings(IPropertiesOwner owner)
			: base(owner) {
			InitializeSettings();
		}
		void InitializeSettings() {
			AccessTokenValue = string.Empty;
			UploadFolder = string.Empty;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("UploadControlDropboxSettingsAccessTokenValue"),
#endif
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string AccessTokenValue { get; set; }
		[
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string UploadFolder { get; set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			UploadControlDropboxSettings settings = source as UploadControlDropboxSettings;
			if(settings != null) {
				AccessTokenValue = settings.AccessTokenValue;
				UploadFolder = settings.UploadFolder;
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class UploadControlFileSystemSettings : PropertiesBase, IPropertiesOwner {
		public UploadControlFileSystemSettings()
			: base() {
				InitializeSettings();
		}
		public UploadControlFileSystemSettings(IPropertiesOwner owner)
			: base(owner) {
				InitializeSettings();
		}
		void InitializeSettings() {
			UploadFolder = string.Empty;
		}
		[
		DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable, Localizable(false)]
		public string UploadFolder { get; set; }
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			UploadControlFileSystemSettings settings = source as UploadControlFileSystemSettings;
			if(settings != null) {
				UploadFolder = settings.UploadFolder;
			}
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
}
namespace DevExpress.Web.Internal {
	public abstract class UploadInternalSettingsBase {
		public long MaxFileSize { get; set; }
		public int MaxFileCount { get; set; }
		public UploadInternalSettingsBase(long maxFileSize, int maxFileCount) {
			MaxFileSize = maxFileSize;
			MaxFileCount = maxFileCount;
		}
		public HelperPostedFileBase CreatePostedFile(string fileName) {
			HelperPostedFileBase postedFile = CreatePostedFileCore(fileName);
			postedFile.Settings = this;
			return postedFile;
		}
		public virtual PartialUploadHelperBase CreatePartialUploadHelper(string uploadKey) {
			throw new NotImplementedException();
		}
		public virtual HelperPostedFileBase CreatePostedFileCore(string fileName) {
			throw new NotImplementedException();
		}
		public virtual bool IsDirectUpload() {
			return false;
		}
		protected bool EqualsInternal(UploadInternalSettingsBase settings) {
			return MaxFileSize == settings.MaxFileSize && MaxFileCount == settings.MaxFileCount;
		}
		protected static int GetHashCodeInternal(params object[] values) {
			int hash = 17;
			foreach(object value in values) {
				if(value != null)
					hash = hash * 23 + value.GetHashCode();
			}
			return hash;
		}
	}
	public class UploadDefaultInternalSettings : UploadInternalSettingsBase {
		public string TemporaryFolder { get; set; }
		public UploadDefaultInternalSettings(long maxFileSize, int maxFileCount, string temporaryFolder)
			: base(maxFileSize, maxFileCount) {
			TemporaryFolder = temporaryFolder;
		}
		public override PartialUploadHelperBase CreatePartialUploadHelper(string uploadKey) {
			return new FileSystemPartialUploadHelper(uploadKey);
		}
		public override HelperPostedFileBase CreatePostedFileCore(string fileName) {
			TemporaryPostedFile postedFile = new TemporaryPostedFile();
			postedFile.FileName = fileName;
			return postedFile;
		}
		public override int GetHashCode() {
			return GetHashCodeInternal(MaxFileSize, TemporaryFolder);
		}
		public override bool Equals(object obj) {
			UploadDefaultInternalSettings settings = obj as UploadDefaultInternalSettings;
			if(settings == null)
				return false;
			return TemporaryFolder == settings.TemporaryFolder && EqualsInternal(settings);
		}
	}
	public class UploadFileSystemInternalSettings : UploadDefaultInternalSettings {
		public string UploadFolder { get; set; }
		public UploadFileSystemInternalSettings(long maxFileSize, int maxFileCount, string temporaryFolder, string uploadFolder)
			: base(maxFileSize, maxFileCount, temporaryFolder) {
				UploadFolder = uploadFolder;
		}
		public override PartialUploadHelperBase CreatePartialUploadHelper(string uploadKey) {
			return new StoragePartialUploadHelper(uploadKey);
		}
		public override HelperPostedFileBase CreatePostedFileCore(string fileName) {
			FileSystemStorageProvider provider = new FileSystemStorageProvider(UploadFolder);
			StoragePostedFile postedFile = new StoragePostedFile(provider, fileName);
			return postedFile;
		}
		public override bool IsDirectUpload() {
			return true;
		}
		public override int GetHashCode() {
			return GetHashCodeInternal(MaxFileSize, TemporaryFolder, UploadFolder);
		}
		public override bool Equals(object obj) {
			UploadFileSystemInternalSettings settings = obj as UploadFileSystemInternalSettings;
			if(settings == null)
				return false;
			return base.Equals(obj) && settings.UploadFolder == UploadFolder;
		}
	}
	public class UploadAzureInternalSettings : UploadInternalSettingsBase {
		public string Account { get; set; }
		public string Key { get; set; }
		public string Container { get; set; }
		public UploadAzureInternalSettings(long maxFileSize, int maxFileCount, string account, string key, string container)
			: base(maxFileSize, maxFileCount) {
			Account = account;
			Key = key;
			Container = container;
		}
		public override PartialUploadHelperBase CreatePartialUploadHelper(string uploadKey) {
			return new StoragePartialUploadHelper(uploadKey);
		}
		public override HelperPostedFileBase CreatePostedFileCore(string fileName) {
			AzureBlobStorageHelper helper = new AzureBlobStorageHelper(Account, Key, Container);
			AzureUploadStorageProvider provider = new AzureUploadStorageProvider(helper);
			StoragePostedFile postedFile = new StoragePostedFile(provider, fileName);
			return postedFile;
		}
		public override bool IsDirectUpload() {
			return true;
		}
		public override int GetHashCode() {
			return GetHashCodeInternal(MaxFileSize, Account, Key, Container);
		}
		public override bool Equals(object obj) {
			UploadAzureInternalSettings settings = obj as UploadAzureInternalSettings;
			if(settings == null)
				return false;
			return Account == settings.Account && Key == settings.Key && Container == settings.Container
				&& EqualsInternal(settings);
		}
	}
	public class UploadAmazonInternalSettings : UploadInternalSettingsBase {
		public string AccessKey { get; set; }
		public string SecretAccessKey { get; set; }
		public string BucketName { get; set; }
		public string Region { get; set; }
		public UploadAmazonInternalSettings(long maxFileSize, int maxFileCount, string accessKey, string secretAccessKey, string bucketName, string region)
			: base(maxFileSize, maxFileCount) {
			AccessKey = accessKey;
			SecretAccessKey = secretAccessKey;
			BucketName = bucketName;
			Region = region;
		}
		public override PartialUploadHelperBase CreatePartialUploadHelper(string uploadKey) {
			return new StoragePartialUploadHelper(uploadKey);
		}
		public override HelperPostedFileBase CreatePostedFileCore(string fileName) {
			AmazonS3Helper helper = new AmazonS3Helper(AccessKey, SecretAccessKey, BucketName, Region);
			AmazonUploadStorageProvider provider = new AmazonUploadStorageProvider(helper);
			StoragePostedFile postedFile = new StoragePostedFile(provider, fileName);
			return postedFile;
		}
		public override bool IsDirectUpload() {
			return true;
		}
		public override int GetHashCode() {
			return GetHashCodeInternal(MaxFileSize, AccessKey, SecretAccessKey, BucketName, Region);
		}
		public override bool Equals(object obj) {
			UploadAmazonInternalSettings settings = obj as UploadAmazonInternalSettings;
			if(settings == null)
				return false;
			return AccessKey == settings.AccessKey && SecretAccessKey == settings.SecretAccessKey &&
				BucketName == settings.BucketName && Region == settings.Region && EqualsInternal(settings);
		}
	}
	public class UploadDropboxInternalSettings : UploadInternalSettingsBase {
		public string AccessTokenValue { get; set; }
		public string UploadFolder { get; set; }
		public UploadDropboxInternalSettings(long maxFileSize, int maxFileCount, string accessTokenValue, string uploadFolder)
			: base(maxFileSize, maxFileCount) {
				AccessTokenValue = accessTokenValue;
				UploadFolder = uploadFolder;
		}
		public override PartialUploadHelperBase CreatePartialUploadHelper(string uploadKey) {
			return new StoragePartialUploadHelper(uploadKey);
		}
		public override HelperPostedFileBase CreatePostedFileCore(string fileName) {
			DropboxHelper helper = new DropboxHelper(AccessTokenValue);
			DropboxUploadStorageProvider provider = new DropboxUploadStorageProvider(helper, UploadFolder);
			StoragePostedFile postedFile = new StoragePostedFile(provider, fileName);
			return postedFile;
		}
		public override bool IsDirectUpload() {
			return true;
		}
		public override int GetHashCode() {
			return GetHashCodeInternal(MaxFileSize, AccessTokenValue);
		}
		public override bool Equals(object obj) {
			UploadDropboxInternalSettings settings = obj as UploadDropboxInternalSettings;
			if(settings == null)
				return false;
			return AccessTokenValue == settings.AccessTokenValue && UploadFolder == settings.UploadFolder && EqualsInternal(settings);
		}
	}
}
