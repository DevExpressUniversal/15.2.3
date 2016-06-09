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
using System.Threading.Tasks;
using System.Web.UI;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.Internal;
using DevExpress.Web.Office.Internal;
using System.Collections.Specialized;
using System.Collections;
using DevExpress.Web.ASPxRichEdit.Export;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using System.Drawing.Printing;
using System.Drawing;
using DevExpress.Office.Utils;
using System.IO;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.XtraRichEdit;
using DevExpress.Office.Localization;
using DevExpress.Web.Office;
using DevExpress.Web.Localization;
namespace DevExpress.Web.ASPxRichEdit {
	public enum RichEditRibbonMode {
		Ribbon,
		ExternalRibbon,
		None,
		Auto,
		OneLineRibbon
	}
	public enum RichEditUnit {
		Centimeter = 0,
		Inch = 1
	}
	[DXWebToolboxItem(true),
	Designer("DevExpress.Web.ASPxRichEdit.Design.ASPxRichEditDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxRichEdit"),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), "ASPxRichEdit.bmp"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	ToolboxData(@"<{0}:ASPxRichEdit runat=""server"" WorkDirectory=""~\App_Data\WorkDirectory""></{0}:ASPxRichEdit>")]
	public partial class ASPxRichEdit : ASPxDataWebControl, OfficeWorkSessionControl, IParentSkinOwner, IControlDesigner, IAutoSaveControl {
		protected internal const string 
			CssResourcePath = "DevExpress.Web.ASPxRichEdit.Css.",
			ScriptsResourcePath = "DevExpress.Web.ASPxRichEdit.Scripts.",
			UIScriptsResourcePath = ScriptsResourcePath + "UI.",
			ImagesResourcePath = "DevExpress.Web.ASPxRichEdit.Images.",
			SystemCssResourceName = CssResourcePath + "System.css",
			DefaultCssResourceName = CssResourcePath + "Default.css",
			SpriteCssResourceName = CssResourcePath + "sprite.css",
			IconSpriteCssResourceName = CssResourcePath + "ISprite.css",
			GrayScaleIconSpriteCssResourceName = CssResourcePath + "GISprite.css",
			GrayScaleWithWhiteHottrackIconSpriteCssResourceName = CssResourcePath + "GWISprite.css",
			RichEditScriptResourceName = ScriptsResourcePath + "RichEdit.js",
			CompiledScriptResourceName = ScriptsResourcePath + "compiled.js",
			InputControllerScriptResourceName = ScriptsResourcePath + "InputController.js",
			DialogsScriptResourceName = UIScriptsResourcePath + "Dialogs.js",
			FileManagerScriptResourceName = UIScriptsResourcePath + "FileManager.js",
			FolderManagerScriptResourceName = UIScriptsResourcePath + "FolderManager.js",
			CursorImageName = ImagesResourcePath + "Cursor",
			CursorTouchImageName = ImagesResourcePath + "CursorTouch",
			SpriteImageName = ImagesResourcePath + "sprite",
			IconSpriteImageName = ImagesResourcePath + "ISprite",
			GrayScaleIconSpriteImageName = ImagesResourcePath + "GISprite",
			GrayScaleWithWhiteHottrackIconSpriteImageName = ImagesResourcePath + "GWISprite",
			EmptyImageResourceName = ImagesResourcePath + "1x1",
			ImageLoadingResourceName = ImagesResourcePath + "reImageLoading.gif",
			RichEditRibbonContainerID = "R",
			RicheditPopupDialogContainerID = "REDC",
			RichEditPopupMenuContainerID = "REPUM",
			InternalCallbackPostfix = "%Rich%Edit",
			ClientGuidParamsPostfix = "cguid",
			SyncCommandsParamsPostfix = "scmds",
			InternalServiceCallbackParamsPostfix = "isc";
		RichEditRibbonTabCollection ribbonTabs;
		RichEditRibbonContextTabCategoryCollection ribbonContextTabCategories;
		Control currentDialogForm;
		Guid clientGuid;
		ASPxRichEditSettings settings;
		RichEditDocumentSelectorSettings settingsDocumentSelector;
		RichEditDialogSettings settingsDialogs;
		RichEditRulerImages imagesRuler;
		RichEditRulerStyles stylesRuler;
		RichEditRibbonStyles stylesRibbon;
		RichEditMenuStyles stylesPopupMenu;
		RichEditButtonStyles stylesButton;
		RichEditEditorsStyles stylesEditors;
		RichEditFileManagerStyles stylesFileManager;
		RichEditStatusBarStyles stylesStatusBar;
		private static readonly object EventCallback = new object();		
		private static readonly object CalculateDocumentVariableEventKey = new object();
		private static readonly object EventSaving = new object();
		static ASPxRichEdit() {
			ASPxHttpHandlerModule.Subscribe(new DocumentWorkSessionManagerSubscriber(), true);
		}
		public ASPxRichEdit() {
			this.settings = new ASPxRichEditSettings(this);
			this.settingsDocumentSelector = new RichEditDocumentSelectorSettings(this);
			this.settingsDialogs = new RichEditDialogSettings(this);
			this.imagesRuler = new RichEditRulerImages(this);
			this.stylesRuler = new RichEditRulerStyles(this);
			this.stylesButton = new RichEditButtonStyles(this);
			this.stylesEditors = new RichEditEditorsStyles(this);
			this.stylesPopupMenu = new RichEditMenuStyles(this);
			this.stylesRibbon = new RichEditRibbonStyles(this);
			this.stylesFileManager = new RichEditFileManagerStyles(this);
			this.stylesStatusBar = new RichEditStatusBarStyles(this);
			if(!DevExpress.Office.AzureCompatibility.Enable)
				DevExpress.Office.AzureCompatibility.Enable = HttpUtils.IsAzureEnvironment();
		}
		[
		Category("Ribbon"), DefaultValue(RichEditRibbonMode.Auto), AutoFormatDisable, Localizable(false)]
		public RichEditRibbonMode RibbonMode {
			get { return (RichEditRibbonMode)GetEnumProperty("RibbonMode", RichEditRibbonMode.Auto); }
			set {
				SetEnumProperty("RibbonMode", RichEditRibbonMode.Auto, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditRibbonTabs"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false), Category("Ribbon"),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public RichEditRibbonTabCollection RibbonTabs {
			get {
				if(ribbonTabs == null)
					ribbonTabs = new RichEditRibbonTabCollection(this);
				return ribbonTabs;
			}
		}
		[
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false), Category("Ribbon")]
		public RichEditRibbonContextTabCategoryCollection RibbonContextTabCategories {
			get {
				if(ribbonContextTabCategories == null)
					ribbonContextTabCategories = new RichEditRibbonContextTabCategoryCollection(this);
				return ribbonContextTabCategories;
			}
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditActiveTabIndex"),
#endif
		Category("Ribbon"), DefaultValue(1), AutoFormatDisable]
		public int ActiveTabIndex {
			get { return GetIntProperty("ActiveTabIndex", 1); }
			set { SetIntProperty("ActiveTabIndex", 1, value); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditAssociatedRibbonID"),
#endif
		Category("Ribbon"), DefaultValue(""), AutoFormatDisable,
		TypeConverter("DevExpress.Web.Design.RibbonControlIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull),
		NotifyParentProperty(true), Localizable(false)]
		public string AssociatedRibbonID {
			get { return GetStringProperty("AssociatedRibbonID", string.Empty); }
			set { SetStringProperty("AssociatedRibbonID", string.Empty, value); }
		}
		[Browsable(false), Bindable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[
		Category("StatusBar"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ShowStatusBar {
			get { return GetBoolProperty("ShowStatusBar", true); }
			set { SetBoolProperty("ShowStatusBar", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditSettingsDocumentSelector"),
#endif
		Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditDocumentSelectorSettings SettingsDocumentSelector {
			get { return this.settingsDocumentSelector; }
		}
		[Category("Behavior"), DefaultValue(false), Localizable(false), AutoFormatDisable]
		public bool ReadOnly {
			get { return GetBoolProperty("ReadOnly", false); }
			set { SetBoolProperty("ReadOnly", false, value); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), Category("Settings"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public RichEditDialogSettings SettingsDialogs { get { return settingsDialogs; } }
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditImages Images {
			get { return (RichEditImages)ImagesInternal; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditImagesRuler"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditRulerImages ImagesRuler {
			get { return (RichEditRulerImages)imagesRuler; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditStyles Styles {
			get { return StylesInternal as RichEditStyles; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditStylesRuler"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditRulerStyles StylesRuler {
			get { return stylesRuler; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditStylesRibbon"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditRibbonStyles StylesRibbon {
			get { return stylesRibbon; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditStylesPopupMenu"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditMenuStyles StylesPopupMenu {
			get { return stylesPopupMenu; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditStylesButton"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditButtonStyles StylesButton {
			get { return stylesButton; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditStylesEditors"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditEditorsStyles StylesEditors {
			get { return stylesEditors; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditStylesFileManager"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditFileManagerStyles StylesFileManager {
			get { return stylesFileManager; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditStylesStatusBar"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RichEditStatusBarStyles StylesStatusBar {
			get { return stylesStatusBar; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public RichEditClientSideEvents ClientSideEvents {
			get { return (RichEditClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditEnableClientSideAPI"),
#endif
		Category("Client-Side"), DefaultValue(false), AutoFormatDisable]
		public bool EnableClientSideAPI {
			get { return base.EnableClientSideAPIInternal; }
			set { base.EnableClientSideAPIInternal = value; }
		}
		[Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ASPxRichEditSettings Settings { get { return settings; } }
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditWorkDirectory"),
#endif
		DefaultValue(""), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(false)]
		public string WorkDirectory {
			get { return GetStringProperty("WorkDirectory", string.Empty); }
			set { SetStringProperty("WorkDirectory", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditShowConfirmOnLosingChanges"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowConfirmOnLosingChanges {
			get { return GetBoolProperty("ShowConfirmOnLosingChanges", true); }
			set {
				if(value == ShowConfirmOnLosingChanges)
					return;
				SetBoolProperty("ShowConfirmOnLosingChanges", true, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditConfirmOnLosingChanges"),
#endif
 DefaultValue(""), NotifyParentProperty(true), Localizable(true)]
		public string ConfirmOnLosingChanges {
			get { return GetStringProperty("ConfirmOnLosingChanges", string.Empty); }
			set {
				if(value == ConfirmOnLosingChanges)
					return;
				SetStringProperty("ConfirmOnLosingChanges", string.Empty, value);
			}
		}
		public void MailMerge(XtraRichEdit.API.Native.Document document) {
			CurrentSession.RichEdit.MailMerge(document);
		}
		public void MailMerge(Stream stream, DocumentFormat documentFormat) {
			CurrentSession.RichEdit.MailMerge(stream, documentFormat);
		}
		public void MailMerge(string fileName, DocumentFormat documentFormat) {
			CurrentSession.RichEdit.MailMerge(fileName, documentFormat);
		}
		public void MailMerge(XtraRichEdit.API.Native.MailMergeOptions mailMergeOptions, XtraRichEdit.API.Native.Document document) {
			CurrentSession.RichEdit.MailMerge(mailMergeOptions, document);
		}
		public void MailMerge(XtraRichEdit.API.Native.MailMergeOptions mailMergeOptions, Stream stream, DocumentFormat documentFormat) {
			CurrentSession.RichEdit.MailMerge(mailMergeOptions, stream, documentFormat);
		}
		public void MailMerge(XtraRichEdit.API.Native.MailMergeOptions mailMergeOptions, string fileName, DocumentFormat documentFormat) {
			CurrentSession.RichEdit.MailMerge(mailMergeOptions, fileName, documentFormat);
		}
		public XtraRichEdit.API.Native.MailMergeOptions CreateMailMergeOptions() {
			return CurrentSession.RichEdit.CreateMailMergeOptions();
		}
		protected int MailMergeRecordCount { get; private set; }
		protected object CurrentDataObject { get; private set; }
		protected List<string> MergeFieldNamesList { get; private set; }
		protected internal int MailMergeActiveRecordIndex {
			get { return GetIntProperty("MailMergeActiveRecordIndex", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "MailMergeActiveRecordIndex");
				SetIntProperty("MailMergeActiveRecordIndex", 0, value);
				PerformSelect();
			}
		}
		[ DefaultValue(false), NotifyParentProperty(true)]
		public bool ViewMergedData {
			get { return GetBoolProperty("ViewMergedData", false); }
			set { SetBoolProperty("ViewMergedData", false, value); }
		}
		protected override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			if(data == null)
				return;
			if(CurrentSession.RichEdit.Options.MailMerge.DataSource == null)
				CurrentSession.RichEdit.Options.MailMerge.DataSource = DataUtils.ConvertEnumerableToList(data);
			MailMergeRecordCount = GetRecordCount(data);
			MergeFieldNamesList = GetMergeFieldNamesList(data);
			CurrentDataObject = GetMailMergeDataObject(data);
		}
		protected int GetRecordCount(IEnumerable data) {
			return data is IList ? ((IList)data).Count : data.Cast<object>().Count();
		}
		protected List<string> GetMergeFieldNamesList(IEnumerable data) {
			List<string> list = new List<string>();
			var columns = new DevExpress.Data.Helpers.MasterDetailHelper().GetDataColumnInfo(null, data, null, false);
			foreach (DevExpress.Data.DataColumnInfo column in columns)
				list.Add(column.Name);
			return list;
		}
		protected object GetMailMergeDataObject(IEnumerable data) {
			if (data != null && MailMergeRecordCount > 0) {
				int activeRecordIndex = GetActiveRecordIndex(MailMergeRecordCount);
				return data is IList ? ((IList)data)[activeRecordIndex] : data.Cast<object>().ElementAt(activeRecordIndex);
			}
			return null;
		}
		protected internal List<string> GetMergeFieldNamesList() {
			PerformSelect();
			return MergeFieldNamesList;
		}
		protected internal object GetMergeFieldValue(string fieldName) {
			return GetFieldValue(CurrentDataObject, fieldName, false, "");
		}
		protected int GetActiveRecordIndex(int recordCount) {
			return MailMergeActiveRecordIndex < recordCount ? MailMergeActiveRecordIndex : recordCount - 1;
		}
		[ DefaultValue(AutoSaveMode.Default), NotifyParentProperty(true)]
		public AutoSaveMode AutoSaveMode {
			get { return (AutoSaveMode)GetEnumProperty("AutoSaveMode", AutoSaveMode.Default); }
			set { 
				bool turnOn = value == Office.Internal.AutoSaveMode.On && AutoSaveMode != value;
				if(turnOn)
					WorkSessionProcessing.Start();
				SetEnumProperty("AutoSaveMode", AutoSaveMode.Default, value); 
			}
		}
		[ DefaultValue(typeof(TimeSpan), AutoSaveDefaultSettings.DefaultAutoSaveTimeoutSecondsString), NotifyParentProperty(true)]
		public TimeSpan AutoSaveTimeout  {
			get { return (TimeSpan)(GetObjectProperty("AutoSaveTimeout", AutoSaveDefaultSettings.DefaultAutoSaveTimeout)); }
			set { SetObjectProperty("AutoSaveTimeout", AutoSaveDefaultSettings.DefaultAutoSaveTimeout, value); }
		}
		protected string DocumentPath {
			get { 
				EnsureWorkSession();
				return CurrentSession.RichEdit.DocumentModel.DocumentSaveOptions.CurrentFileName;
			}
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase Callback {
			add { Events.AddHandler(EventCallback, value); }
			remove { Events.RemoveHandler(EventCallback, value); }
		}
		[Category("Action")]
		public event CalculateDocumentVariableEventHandler CalculateDocumentVariable {
			add { Events.AddHandler(CalculateDocumentVariableEventKey, value); }
			remove { Events.RemoveHandler(CalculateDocumentVariableEventKey, value); }
		}
		[
#if !SL
	DevExpressWebASPxRichEditLocalizedDescription("ASPxRichEditSaving"),
#endif
		Category("Action")]
		public event DocumentSavingEventHandler Saving {
			add { Events.AddHandler(EventSaving, value); }
			remove { Events.RemoveHandler(EventSaving, value); }
		}
		protected internal static void RaiseCallbackErrorInternal(object sender, Exception exc) {
			ASPxWebControl.AddErrorForHandler(exc);
			ASPxWebControl.RaiseCallbackErrorInternal(sender);
			ASPxWebControl.ClearErrorForHandler();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] {
				Settings,
				SettingsDocumentSelector,
				SettingsDialogs,
				ImagesRuler,
				StylesRuler,
				StylesButton,
				StylesEditors,
				StylesFileManager,
				StylesPopupMenu,
				StylesRibbon,
				StylesStatusBar,
				RibbonTabs,
				RibbonContextTabCategories
			});
		}
		protected new internal string GetClientInstanceName() {
			return base.GetClientInstanceName();
		}
		#region OfficeWorkSessionControl Members
		Guid OfficeWorkSessionControl.GetWorkSessionID() {
			return WorkSessionGuid;
		}
		void OfficeWorkSessionControl.AttachToWorkSession(Guid workSessionID) {
			Open(workSessionID);
		}
		#endregion
		Guid workSessionGuid = Guid.Empty;
		protected internal Guid WorkSessionGuid { 
			get { return workSessionGuid; } 
			set {
				WorkSessions.OnControlDetachFromWorkSession(workSessionGuid);
				workSessionGuid = value;
				if(value != Guid.Empty)
					SyncWorkSessionSettings();	
			} 
		}
		protected Guid TemporaryWorkSessionGuid { get; private set; }
		protected internal RichEditWorkSession CurrentSession { 
			get {
				var session = (RichEditWorkSession)WorkSessions.Get(WorkSessionGuid, true);
				if(session == null) {
					var isNewDocument = WorkSessionGuid == Guid.Empty;
					OpenWorkSession(Guid.Empty, DocumentContentContainer.Empty, ClientID);
					if(!isNewDocument)
						TemporaryWorkSessionGuid = WorkSessionGuid;
				}
				return (RichEditWorkSession)WorkSessions.Get(WorkSessionGuid, true); 
			} 
		}
		void CloseTemporaryWorkSession() {
			if(TemporaryWorkSessionGuid != Guid.Empty && TemporaryWorkSessionGuid != WorkSessionGuid) {
				WorkSessions.CloseWorkSession(TemporaryWorkSessionGuid);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DocumentId { 
			get { return CurrentSession.DocumentPathOrID; } 
			set {
				if(value == DocumentId) return;
				WorkSessions.CheckDocumentIsUnique(value);
				EnsureWorkSession();
				CurrentSession.DocumentPathOrID = value; 
			}
		}
		protected Guid ClientGuid {
			get {
				if(clientGuid == Guid.Empty)
					clientGuid = Guid.NewGuid();
				return clientGuid;
			}
			private set { clientGuid = value; }
		}
		protected Control CurrentDialogForm {
			get { return currentDialogForm; }
			set { currentDialogForm = value; }
		}
		protected string CurrentDialogName { get { return GetClientObjectStateValue<string>("currentDialog"); } }
		protected internal int? CurrentColumnsCount { get { return GetClientObjectStateValue<int?>("columnsCount"); } }
		protected Control ColumnsEditorForm { get; set; }
		public void New() {
			CreateNewWorkSession();
		}
		public void Save() {
			bool calledFromSavingEventHanlder = RaiseSavingCallIsLocked();
			DocumentSavingEventArgs args = calledFromSavingEventHanlder ? null : RaiseSaving(CurrentSession.DocumentPathOrID, MultiUserConflict.None);
			if(calledFromSavingEventHanlder || !args.Handled) {
				CurrentSession.SaveInTheSameFile();
			}
			if (args.Handled && CurrentSession.Modified) {
				CurrentSession.Modified = false;
			}
		}
		public void SaveCopy(string documentPath) {
			var saveOptions = CurrentSession.RichEdit.DocumentModel.DocumentSaveOptions;
			string currentDocPathBackUp = saveOptions.CurrentFileName;
			CurrentSession.RichEdit.SaveDocument(documentPath, CurrentSession.RichEdit.DocumentModel.AutodetectDocumentFormat(documentPath, false));
			saveOptions.CurrentFileName = currentDocPathBackUp;
		}
		public void SaveCopy(Stream stream, DocumentFormat format) { 
			CurrentSession.RichEdit.SaveDocument(stream, format);
		}
		public byte[] SaveCopy(DocumentFormat format) {
			using(Stream stream = new MemoryStream()) {
				CurrentSession.RichEdit.SaveDocument(stream, format);
				stream.Position = 0;
				return CommonUtils.GetBytesFromStream(stream);
			}
		}
		public void ExportToPdf(Stream stream) {
			if (CurrentSession != null) {
				CurrentSession.RichEdit.DocumentModel.ToggleAllFieldCodes(false);
				CurrentSession.RichEdit.ExportToPdf(stream);
			}
		}
		public void ExportToPdf(Stream stream, DevExpress.XtraPrinting.PdfExportOptions pdfExportOptions) {
			if (CurrentSession != null) {
				CurrentSession.RichEdit.DocumentModel.ToggleAllFieldCodes(false);
				CurrentSession.RichEdit.ExportToPdf(stream, pdfExportOptions);
			}
		}
		 public void Open(string pathToDocument) {
			var documentContentContainer = new DocumentContentContainer(pathToDocument);
			OpenCore(documentContentContainer);
		}
		public void Open(string pathToDocument, DocumentFormat format) {
			var documentContentContainer = new DocumentContentContainer(pathToDocument, format.ToString());
			OpenCore(documentContentContainer);
		}
		public void Open(string documentId, DocumentFormat format, Func<Stream> contentAccessor) {
			if(!ConnectToOpenedDocumentById(documentId)) {
				Stream stream = contentAccessor();
				var documentContentContainer = new DocumentContentContainer(stream, format.ToString(), documentId);
				OpenCore(documentContentContainer);
			}
		}
		public void Open(string documentId, DocumentFormat format, Func<byte[]> contentAccessor) {
			if(!ConnectToOpenedDocumentById(documentId)) {
				byte[] array = contentAccessor();
				var documentContentContainer = new DocumentContentContainer(array, format.ToString(), documentId);
				OpenCore(documentContentContainer);
			}
		}
		public void Open(RichEditDocumentInfo documentInfo) {
			Open(documentInfo.WorkSessionGuid);
		}
		public void Open(Guid workSessionGuid) {
			WorkSessionGuid = workSessionGuid;
			CurrentSession.AttachClient(ClientGuid, WorkDirectory, ReadOnly, Settings.Behavior, Settings.DocumentCapabilities);
		}
		protected void OpenCore(DocumentContentContainer documentContentContainer) {
			 if (!IsInternalServiceCallback()) {
				OpenWorkSession(WorkSessionGuid, documentContentContainer, ClientID);
				ResetControlHierarchy();
			}
		}
		protected bool ConnectToOpenedDocumentById(string documentId) {
			RichEditDocumentInfo documentInfo = DocumentManager.FindDocument(documentId) as RichEditDocumentInfo;
			bool documentWithThisDocumentIdAlreadyOpened = documentInfo != null;
			if(documentWithThisDocumentIdAlreadyOpened)
				Open(documentInfo);
			return documentWithThisDocumentIdAlreadyOpened;
		}
		protected void OpenWorkSession(Guid sessionGuidId, DocumentContentContainer documentContentContainer, string clientId) {
			WorkSessionGuid = WorkSessions.OpenWorkSession(sessionGuidId, documentContentContainer, (Guid newGuid) => {
				var session = new RichEditWorkSession(documentContentContainer, newGuid, Settings.DocumentCapabilities);
				session.EditorClientGuid = ClientGuid;
				return session;
			});
			var behaviorOptions = new ASPxRichEditBehaviorSettings();
			behaviorOptions.Assign(Settings.Behavior);
			var documentCapabilities = new DocumentCapabilitiesOptions();
			documentCapabilities.Assign(Settings.DocumentCapabilities);
			CurrentSession.AttachClient(ClientGuid, WorkDirectory, ReadOnly, behaviorOptions, documentCapabilities);
			CloseTemporaryWorkSession();
		}
		public void CreateDefaultRibbonTabs(bool clearExistingTabs) {
			if(RibbonMode == RichEditRibbonMode.ExternalRibbon) {
				ASPxRibbon ribbon = RibbonHelper.LookupRibbonControl(this, AssociatedRibbonID);
				if(ribbon != null) {
					RichEditRibbonHelper.AddTabCollectionToControl(ribbon, new RichEditDefaultRibbon(this).DefaultRibbonTabs, clearExistingTabs);
					RichEditRibbonHelper.AddContextCategoriesToControl(ribbon, new RichEditDefaultRibbon(this).DefaultRibbonContextTabCategories, clearExistingTabs);
				}
			} else if(RibbonMode != RichEditRibbonMode.None) {
				if(clearExistingTabs)
					RibbonTabs.Clear();
				RibbonTabs.CreateDefaultRibbonTabs();
			}
		}
		protected override void OnPreRender(EventArgs e) {
			base.OnPreRender(e);
			if(!DesignMode) {
				CheckWorkDirectoryAccess();
			}
		}
		static object locker = new object();
		protected void CheckWorkDirectoryAccess() {
			lock(locker) {
				if(!string.IsNullOrEmpty(this.WorkDirectory))
					FileUtils.CheckOrCreateDirectory(this.WorkDirectory, this, "WorkDirectory");
			}
		}
		public bool IsInternalServiceCallback() {
			if(Page != null && (Page.IsCallback || Page.IsPostBack)) {
				return (CallbackArgumentsReader != null && !CallbackArgumentsReader.IsCustomCallback) ||
					GetClientObjectStateValue<bool>(InternalServiceCallbackParamsPostfix);
			}
			return false;
		}
		public string GetWorkDirectory() {
			return UrlUtils.NormalizeRelativePath(this.WorkDirectory);
		}
		protected bool HasWorkDirectory() {
			return !string.IsNullOrEmpty(this.WorkDirectory);
		}
		protected string GetConfirmUpdate() {
			if(!string.IsNullOrEmpty(ConfirmOnLosingChanges))
				return ConfirmOnLosingChanges;
			return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ConfirmOnLosingChanges);
		}
		protected void CloseCurrentSession() {
			WorkSessions.CloseWorkSession(WorkSessionGuid);
		}
		protected void EnsureWorkSession() {
			if(WorkSessionGuid == Guid.Empty)
				CreateNewWorkSession();
			CloseTemporaryWorkSession();
		}
		protected void CreateNewWorkSession() {
			OpenWorkSession(Guid.Empty, DocumentContentContainer.Empty, ClientID);
		}
		protected void SyncWorkSessionSettings() {
			if(CurrentSession != null)
				CurrentSession.SyncSettingWithControl(this);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			if(Page == null)
				return;
			if(Page.IsCallback || Page.IsPostBack)
				LoadWorkSessionIdFromRequest();
		}
		protected void LoadWorkSessionIdFromRequest() {
			if(ClientObjectState == null) return;
			WorkSessionGuid = DocumentRequestHelper.GetSessionGuid(GetClientObjectStateValue<string>(DocumentRequestHelper.SessionGuidStateKey));
			if(WorkSessionGuid != Guid.Empty) {
				ClientGuid = Guid.Parse(GetClientObjectStateValue<string>(ClientGuidParamsPostfix));
				if (CurrentSession != null) {
					if (!IsCallback)
						ApplyRequestCommands();
				}
				else
					WorkSessionGuid = Guid.Empty;
			}
		}
		protected void ApplyRequestCommands() {
			var syncParamsQuery = GetClientObjectStateValue<string>(SyncCommandsParamsPostfix);
			if (!string.IsNullOrEmpty(syncParamsQuery))
				CommandFactory.ExecuteCommands(CurrentSession, ClientGuid, this, new NameValueCollection() { { "commands", syncParamsQuery } });
		}
		protected override ImagesBase CreateImages() {
			return new RichEditImages(this);
		}
		protected internal RichEditControl RichEditControl { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			RichEditControl = CreateRichEditControl();
			Controls.Add(RichEditControl);
			EnsureWorkSession();
			CreateDialogs();
		}
		protected RichEditControl CreateRichEditControl() {
			return DesignMode ? new RichEditControlDesignTime(this) : new RichEditControl(this);
		}
		protected virtual void CreateDialogs() {
			if(!DesignMode) {
				if(!string.IsNullOrEmpty(CurrentDialogName)) {
					CurrentDialogForm = CreateDialogFromControl(CurrentDialogName, this);
					CurrentDialogForm.EnableViewState = false;
					if(Page != null && Page.IsCallback) {
						CurrentDialogForm.DataBind();
						RenderUtils.LoadPostDataRecursive(CurrentDialogForm, Request.Params, true);
					} else
						CurrentDialogForm.Visible = false;
				}
				if(CurrentColumnsCount != null) {
					ColumnsEditorForm = CreateColumnsEditorControl(this);
					if(Page != null && Page.IsCallback) {
						ColumnsEditorForm.DataBind();
						RenderUtils.LoadPostDataRecursive(ColumnsEditorForm, Request.Params, true);
					}
				}
			}
		}
		protected internal Control CreateDialogFromControl(string dialogName, WebControl parent) {
			Control control = CreateDefaultForm(dialogName);
			PrepareUserControl(control, parent, dialogName, true);
			return control;
		}
		protected internal Control CreateColumnsEditorControl(WebControl parent) {
			Control control = new Forms.ColumnsEditorForm();
			PrepareUserControl(control, parent, "columnsEditor", true);
			return control;
		}
		protected Control CreateDefaultForm(string formName) {
			switch(formName) {
				case "openfiledialog":
					if(!Settings.Behavior.OpenAllowed || !HasWorkDirectory())
						throw new UnauthorizedAccessException();
					return CreateOpenFileDialog();
				case "savefiledialog":
					if(!Settings.Behavior.SaveAsAllowed || !HasWorkDirectory())
						throw new UnauthorizedAccessException();
					return new Forms.SaveFileForm();
				case "fontdialog":
					return new Forms.FontForm();
				case "paragraphdialog":
					return new Forms.ParagraphForm();
				case "pagesetupdialog":
					return new Forms.PageSetupForm();
				case "columnsdialog":
					return new Forms.ColumnsForm();
				case "inserttabledialog":
					return new Forms.InsertTableForm();
				case "insertimagedialog":
					return CreateInsertImageDialog();
				case "errordialog":
					return new Forms.ErrorForm();
				case "numberinglistdialog":
					return new Forms.NumberingListForm();
				case "hyperlinkdialog":
					return new Forms.InsertHyperlinkForm();
				case "tabsdialog":
					return new Forms.TabsForm();
				case "simplenumberinglistdialog":
					return new Forms.SimpleNumberingListForm();
				case "bulletedlistdialog":
					return new Forms.BulletedListForm();
				case "multilevelnumberinglistdialog":
					return new Forms.MultiLevelNumberingListForm();
				case "symbolsdialog":
					return new Forms.SymbolsForm();
				case "insertmergefielddialog":
					return new Forms.InsertMergeFieldForm();
				case "finishandmergedialog":
					return new Forms.FinishMergeForm();
				case "bookmarksdialog":
					return new Forms.BookmarksForm();
				case "tablepropertiesdialog":
					return new Forms.TablePropertiesForm();
				case "inserttablecellsdialog":
					return new Forms.InsertTableCellsForm();
				case "deletetablecellsdialog":
					return new Forms.DeleteTableCellsForm();
				case "splittablecellsdialog":
					return new Forms.SplitTableCellsForm();
				case "bordershadingdialog":
					return new Forms.BorderShadingForm();
				default:
					throw new ArgumentException();
			}
		}
		protected virtual Control CreateInsertImageDialog() {
			return new Forms.InsertImageForm();
		}
		protected virtual Control CreateOpenFileDialog() {
			return new Forms.OpenFileForm();
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(Page != null && RibbonMode != RichEditRibbonMode.ExternalRibbon)
				RibbonHelper.SyncRibbonControlCollection(RibbonTabs, RichEditControl.RibbonControl, postCollection);
			return base.LoadPostData(postCollection);
		}
		protected RichEditCallbackArgumentsReader CallbackArgumentsReader { get; set; }
		protected override void RaiseCallbackEvent(string eventArgument) {
			CallbackArgumentsReader = new RichEditCallbackArgumentsReader(eventArgument);
			if (CallbackArgumentsReader.IsCustomCallback) {
				ApplyRequestCommands();
				RaiseCustomCallback(CallbackArgumentsReader.PerformCallbackArg);
			}
		}
		protected void RaiseCustomCallback(string eventArgument) {
			CallbackEventHandlerBase handler = Events[EventCallback] as CallbackEventHandlerBase;
			if(handler != null) {
				CallbackEventArgsBase e = new CallbackEventArgsBase(eventArgument);
				handler(this, e);
			}
		}
		private int raiseSavingCallLockCount = 0;
		protected void LockRaiseSavingCall() {
			raiseSavingCallLockCount++;
		}
		protected void UnlockRaiseSavingCall() {
			raiseSavingCallLockCount--;
		}
		protected bool RaiseSavingCallIsLocked() {
			return raiseSavingCallLockCount > 0;
		}
		protected internal DocumentSavingEventArgs RaiseSaving(string documentID, MultiUserConflict multiUserConflict) {
			LockRaiseSavingCall();
			try {
				var args = new DocumentSavingEventArgs(documentID, multiUserConflict);
				var handler = Events[EventSaving] as DocumentSavingEventHandler;
				if(handler != null)
					handler(this, args);
				return args;
			} finally {
				UnlockRaiseSavingCall();
			}
		}
		protected override object GetCallbackResult() {
			if(CallbackArgumentsReader.IsCustomCallback)
				return GetCustomCallbackResult();
			else if(CallbackArgumentsReader.IsCommandCallback) {
				string response = (string)CommandFactory.ExecuteCommands(CurrentSession, ClientGuid, this, 
					new NameValueCollection() { { "commands", CallbackArgumentsReader.CommandCallbackArg } }).GetResponseResult();
				return RichEditCallbackArgumentsReader.CommandCallbackPrefix + "|" + response;
			}
			else {
				EnsureChildControls();
				if(CallbackArgumentsReader.IsFileManagerCallback)
					return GetFileManagerCallbackResult(CallbackArgumentsReader.FileManagerCallbackData, CurrentDialogName);
				if(CallbackArgumentsReader.IsColumnsEditorCallback)
					return GetColumnsEditorRenderResult(Convert.ToInt32(CallbackArgumentsReader.ColumnsCount));
				if(CallbackArgumentsReader.IsUploadImageCallback)
					return InsertImageFromCallbackResult(CallbackArgumentsReader.ImageUrl);
				if(CallbackArgumentsReader.IsSymbolListCallback)
					return RichEditSymbolsManager.GetResult(CallbackArgumentsReader.SymbolFontName);
				return GetDialogFormRenderResult(RenderUtils.DialogFormCallbackStatus);
			}
		}
		protected object GetCustomCallbackResult() {
			Hashtable result = new Hashtable();
			result["sessionGuid"] = WorkSessionGuid.ToString();
			CommandManager manager = new CommandManager(CurrentSession, ClientGuid, this);
			StartCommand command = new StartCommand(manager, manager.WorkSession.RichEdit.Model.DocumentModel, Settings);
			result["startResponse"] = manager.ExecuteCommands(new[] { command }, true).GetResponseResult();
			return result;
		}
		protected object GetFileManagerCallbackResult(string callbackArgs, string dialogName) {
			switch(dialogName) {
				case "savefiledialog":
					RichEditFolderManager folderManager = GetDialogFileManager(dialogName) as RichEditFolderManager;
					folderManager.IsRichEditCallback = this.IsCallback;
					return folderManager.GetCallbackResult(callbackArgs);
				default:
					RichEditFileManager fileManager = GetDialogFileManager(dialogName) as RichEditFileManager;
					fileManager.IsRichEditCallback = this.IsCallback;
					return fileManager.GetCallbackResult(callbackArgs);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string AddImageToCache(Stream stream) {
			Hashtable result = new Hashtable();
			if(stream.Length == 0)
				result["id"] = null;
			else {
				OfficeImage image = CurrentSession.RichEdit.DocumentModel.CreateImage(stream);
				result["id"] = image.ImageCacheKey;
				result["originalWidth"] = image.SizeInTwips.Width;
				result["originalHeight"] = image.SizeInTwips.Height;
			}
			return HtmlConvertor.ToJSON(result);
		}
		protected string InsertImageFromCallbackResult(string url) {
			RichEditDownloadImageHelper helper = new RichEditDownloadImageHelper(this);
			DownloadImageErrorType errorType = DownloadImageErrorType.None;
			string result = helper.AddImageToCacheFromUrl(url, ref errorType);
			return GetInsertImageFromUrlCallbackResult(errorType, result);
		}
		protected string GetInsertImageFromUrlCallbackResult(DownloadImageErrorType errorType, string saveResult) {
			StringBuilder retStringBuilder = new StringBuilder(RichEditCallbackArgumentsReader.SaveImageToServerCallbackPrefix + RenderUtils.CallBackSeparator);
			switch(errorType) {
				case DownloadImageErrorType.None:
					retStringBuilder.Append(RichEditCallbackArgumentsReader.SaveImageToServerNewUrlCallbackPrefix +
						RenderUtils.CallBackSeparator + saveResult);
					break;
				case DownloadImageErrorType.ImageFileSizeError:
					retStringBuilder.Append(RichEditCallbackArgumentsReader.SaveImageToServerErrorCallbackPrefix +
						RenderUtils.CallBackSeparator + "");
					break;
				case DownloadImageErrorType.InvalidImageUrl:
					retStringBuilder.Append(RichEditCallbackArgumentsReader.SaveImageToServerErrorCallbackPrefix +
						RenderUtils.CallBackSeparator + "");
					break;
				case DownloadImageErrorType.InternalError:
					retStringBuilder.Append(RichEditCallbackArgumentsReader.SaveImageToServerErrorCallbackPrefix +
						RenderUtils.CallBackSeparator + saveResult);
					break;
			}
			return retStringBuilder.ToString();
		}
		protected Control GetDialogFileManager(string dialogName) {
			return FindControlHelper.LookupControlRecursive(
				GetCurrentDialogControl(dialogName),
				"FileManager"
			);
		}
		protected object GetDialogFormRenderResult(string dialogName) {
			return GetControlRenderResult(GetCurrentDialogControl(dialogName));
		}
		protected object GetColumnsEditorRenderResult(int columnsCount) {
			return GetControlRenderResult(ColumnsEditorForm);
		}
		protected string GetControlRenderResult(Control control) {
			string content = "";
			BeginRendering();
			try {
				if(control != null)
					content = RenderUtils.GetRenderResult(control);
			} finally {
				EndRendering();
			}
			return content;
		}
		protected virtual Control GetCurrentDialogControl(string dialogName) {
			return CurrentDialogForm;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			RichEditControl = null;
		}
		protected override StylesBase CreateStyles() {
			return new RichEditStyles(this);
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxRichEdit), SystemCssResourceName);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxRichEdit), IconSpriteCssResourceName);
		}
		protected override void RegisterDefaultRenderCssFile() {
			base.RegisterDefaultRenderCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxRichEdit), DefaultCssResourceName);
		}
		protected override void RegisterCustomSpriteCssFile(string spriteCssFile) {
			base.RegisterCustomSpriteCssFile(spriteCssFile);
			if(Images.MenuIconSet != MenuIconSetType.NotSet)
				Images.RegisterIconSpriteCssFile(Page);
		}
		protected override void RegisterDefaultSpriteCssFile() {
			base.RegisterDefaultSpriteCssFile();
			Images.RegisterIconSpriteCssFile(Page);
		}
		protected override string GetSkinControlName() {
			return "RichEdit";
		}
		protected object GetRulerStyles() {
			Dictionary<string, object> styles = new Dictionary<string, object>();
			ImageProperties sprite = ImagesRuler.GetImageProperties(Page, RichEditRulerImages.FirstLineIdentDragHandleImageName);
			sprite.MergeWith(ImagesRuler.FirstLineIdentDragHandle);
			ResolveImageUrl(sprite);
			styles.Add("firstLineIdent", CreateClientStyleObject(StylesRuler.GetDefaultFirstLineIdentDragHandleStyle(), StylesRuler.FirstLineIdentDragHandle));
			styles.Add("firstLineIdentImage", sprite.GetScriptObject(Page));
			sprite = ImagesRuler.GetImageProperties(Page, RichEditRulerImages.LeftIdentDragHandleImageName);
			sprite.MergeWith(ImagesRuler.LeftIdentDragHandle);
			ResolveImageUrl(sprite);
			styles.Add("leftIdent", CreateClientStyleObject(StylesRuler.GetDefaultLeftIdentDragHandleStyle(), StylesRuler.LeftIdentDragHandle));
			styles.Add("leftIdentImage", sprite.GetScriptObject(Page));
			sprite = ImagesRuler.GetImageProperties(Page, RichEditRulerImages.RightIdentDragHandleImageName);
			sprite.MergeWith(ImagesRuler.RightIdentDragHandle);
			ResolveImageUrl(sprite);
			styles.Add("rightIdent", CreateClientStyleObject(StylesRuler.GetDefaultRightIdentDragHandleStyle(), StylesRuler.RightIdentDragHandle));
			styles.Add("rightIdentImage", sprite.GetScriptObject(Page));
			Dictionary<string, object> tabImages = new Dictionary<string, object>();
			sprite = ImagesRuler.GetImageProperties(Page, RichEditRulerImages.LeftTabDragHandleImageName);
			sprite.MergeWith(ImagesRuler.LeftTabDragHandle);
			ResolveImageUrl(sprite);
			tabImages.Add("left", sprite.GetScriptObject(Page));
			sprite = ImagesRuler.GetImageProperties(Page, RichEditRulerImages.RightTabDragHandleImageName);
			sprite.MergeWith(ImagesRuler.RightTabDragHandle);
			ResolveImageUrl(sprite);
			tabImages.Add("right", sprite.GetScriptObject(Page));
			sprite = ImagesRuler.GetImageProperties(Page, RichEditRulerImages.CenterTabDragHandleImageName);
			sprite.MergeWith(ImagesRuler.CenterTabDragHandle);
			ResolveImageUrl(sprite);
			tabImages.Add("center", sprite.GetScriptObject(Page));
			sprite = ImagesRuler.GetImageProperties(Page, RichEditRulerImages.DecimalTabDragHandleImageName);
			sprite.MergeWith(ImagesRuler.DecimalTabDragHandle);
			ResolveImageUrl(sprite);
			tabImages.Add("decimal", sprite.GetScriptObject(Page));
			styles.Add("tabImages", tabImages);
			styles.Add("tab", CreateClientStyleObject(StylesRuler.GetDefaultTabDragHandleStyle(), StylesRuler.TabDragHandle));
			styles.Add("line", CreateClientStyleObject(StylesRuler.GetDefaultLineStyle(), StylesRuler.Line));
			styles.Add("control", CreateClientStyleObject(StylesRuler.GetControlStyle(), StylesRuler.Control));
			styles.Add("wrapper", CreateClientStyleObject(StylesRuler.GetDefaultWrapperStyle()));
			return styles;
		}
		void ResolveImageUrl(ImageProperties properties) {
			if(Page != null)
				properties.Url = Page.ResolveClientUrl(properties.Url);
		}
		protected object CreateClientStyleObject(AppearanceStyle style, AppearanceStyle copyStyle = null) {
			if(copyStyle != null)
				style.CopyFrom(copyStyle);
			var CssClass = style.CssClass;
			var cssText = style.GetStyleAttributes(Page).Value;
			if(string.IsNullOrEmpty(cssText))
				return new { className = CssClass };
			return new { className = CssClass, cssText = cssText };
		}
		protected object CreateClientStyleObject(string className, string cssText) {
			if(string.IsNullOrEmpty(cssText))
				return new { className = className };
			return new { className = className, cssText = cssText };
		}
		protected object GetRulerTitles() {
			Dictionary<string, string> titles = new Dictionary<string, string>();
			titles.Add("firstLineIdent", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerFirstLineIdentTitle));
			titles.Add("hangingIdent", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerHangingIdentTitle));
			titles.Add("leftIdent", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerLeftIdentTitle));
			titles.Add("rightIdent", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerRightIdentTitle));
			titles.Add("marginLeft", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerMarginLeftTitle));
			titles.Add("marginRight", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerMarginRightTitle));
			titles.Add("tabLeft", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerLeftTabTitle));
			titles.Add("tabRight", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerRightTabTitle));
			titles.Add("tabCenter", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerCenterTabTitle));
			titles.Add("tabDecimal", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RulerDecimalTabTitle));
			return titles;
		}
		protected string GetBookmarksSettings() {
			Dictionary<string, object> settings = new Dictionary<string, object>();
			settings.Add("allowNameResolution", Settings.Bookmarks.AllowNameResolution);
			settings.Add("visibility", (int)Settings.Bookmarks.Visibility);
			settings.Add("color", System.Drawing.ColorTranslator.ToHtml(Settings.Bookmarks.Color));
			return HtmlConvertor.ToJSON(settings, true);
		}
		protected string GetRulerClientSettings() {
			Dictionary<string, object> settings = new Dictionary<string, object>();
			settings.Add("titles", GetRulerTitles());
			settings.Add("styles", GetRulerStyles());
			settings.Add("visibility", (int)Settings.HorizontalRuler.Visibility);
			settings.Add("showLeftIndent", Settings.HorizontalRuler.ShowLeftIndent);
			settings.Add("showRightIndent", Settings.HorizontalRuler.ShowRightIndent);
			settings.Add("showTabs", Settings.HorizontalRuler.ShowTabs);
			return HtmlConvertor.ToJSON(settings, true);
		}
		protected string GetUnitAbbreviations() {
			Dictionary<string, object> abbreviations = new Dictionary<string, object>();
			string unitAbbreviation = Settings.Unit == RichEditUnit.Inch ?
				OfficeLocalizer.GetString(OfficeStringId.UnitAbbreviation_Inch) : OfficeLocalizer.GetString(OfficeStringId.UnitAbbreviation_Centimeter);
			abbreviations.Add("unit", unitAbbreviation);
			abbreviations.Add("percent", OfficeLocalizer.GetString(OfficeStringId.UnitAbbreviation_Percent));
			return HtmlConvertor.ToJSON(abbreviations, true);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			EnsureWorkSession();
			CommandManager manager = new CommandManager(CurrentSession, ClientGuid, this);
			StartCommand command = new StartCommand(manager, manager.WorkSession.RichEdit.Model.DocumentModel, Settings);
			DocumentHandlerResponse initResponse = manager.ExecuteCommands(new[] { command }, false);
			stb.AppendLine(string.Format("{0}.startResponse = {1};", localVarName, HtmlConvertor.ToScript(initResponse.GetResponseResult())));
			if(Settings.Unit != RichEditUnit.Inch)
				stb.AppendFormat("{0}.units = {1};\n", localVarName, (int)Settings.Unit);
			stb.AppendFormat("{0}.hasWorkDirectory={1};\n", localVarName, HtmlConvertor.ToScript(HasWorkDirectory()));
			stb.AppendFormat("{0}.rulerSettings = {1};\n", localVarName, GetRulerClientSettings());
			stb.AppendFormat("{0}.bookmarksSettings = {1};\n", localVarName, GetBookmarksSettings());
			if(RibbonMode == RichEditRibbonMode.ExternalRibbon) {
				ASPxRibbon externalRibbon = RibbonHelper.LookupRibbonControl(this, AssociatedRibbonID);
				stb.AppendFormat("{0}.ribbonClientID='{1}';\n", localVarName, IsMvcRender() ? AssociatedRibbonID : externalRibbon != null ? externalRibbon.ClientID : string.Empty);
				if (IsMvcRender()) {
					ASPxRibbon ribbon = new ASPxRibbon();
					ribbon.ParentSkinOwner = this;
					AppendRibbonToCreateClientObjectScript(stb, localVarName, ribbon, Images.MenuIconSet);
				}
				else
					AppendRibbonToCreateClientObjectScript(stb, localVarName, externalRibbon, externalRibbon.Images.IconSet);
			}
			else if(RibbonMode != RichEditRibbonMode.None)
				AppendRibbonToCreateClientObjectScript(stb, localVarName, RichEditControl.RibbonControl, Images.MenuIconSet);
			if(ShowConfirmOnLosingChanges)
				stb.AppendFormat("{0}.confirmUpdate={1};\n", localVarName, HtmlConvertor.ToScript(GetConfirmUpdate()));
			stb.AppendFormat("{0}.clientGuid={1};\n", localVarName, HtmlConvertor.ToScript(ClientGuid.ToString()));
			stb.AppendFormat("{0}.unitAbbreviations={1};\n", localVarName, GetUnitAbbreviations());
			if(ReadOnly)
				stb.AppendFormat("{0}.readOnly=true;\n", localVarName);
			if(WorkSessionGuid == TemporaryWorkSessionGuid)
				stb.AppendFormat("{0}.workSessionIsLost=true;\n", localVarName);
			stb.AppendFormat("{0}.mailMergeOptions = {1};\n", localVarName, GetMailMergeClientOptions());
		}
		protected void AppendRibbonToCreateClientObjectScript(StringBuilder stb, string localVarName, ISkinOwner skinOwner, MenuIconSetType iconSet) {
			var ribbonImages = new RichEditRibbonImages(skinOwner, iconSet);
			stb.AppendFormat("{0}.paragraphRibbonStyleCssClass={1};\n", localVarName, HtmlConvertor.ToScript(ribbonImages.GetImageProperties(Page, RichEditRibbonImages.ParagraphStyleLarge).SpriteProperties.CssClass));
			stb.AppendFormat("{0}.characterRibbonStyleCssClass={1};\n", localVarName, HtmlConvertor.ToScript(ribbonImages.GetImageProperties(Page, RichEditRibbonImages.CharacterStyleLarge).SpriteProperties.CssClass));
			stb.AppendFormat("{0}.tableRibbonStyleCssClass={1};\n", localVarName, HtmlConvertor.ToScript(ribbonImages.GetImageProperties(Page, RichEditRibbonImages.TableStyleLarge).SpriteProperties.CssClass));
		}
		protected string GetMailMergeClientOptions() {
			Dictionary<string, object> options = new Dictionary<string, object>();
			options.Add("isEnabled", MailMergeRecordCount > 0);
			options.Add("viewMergedData", ViewMergedData);
			options.Add("activeRecordIndex", MailMergeActiveRecordIndex);
			options.Add("recordCount", MailMergeRecordCount);
			return HtmlConvertor.ToJSON(options, true);
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterDialogUtilsScripts();
			RegisterFormatterScript();
			RegisterDateFormatterScript();
			RegisterIncludeScript(typeof(ASPxRichEdit), RichEditScriptResourceName);
			RegisterIncludeScript(typeof(ASPxRichEdit), DialogsScriptResourceName);
			RegisterIncludeScript(typeof(ASPxRichEdit), CompiledScriptResourceName);
		}
		protected override void RegisterCustomWebResourceHandlers() {
			base.RegisterCustomWebResourceHandlers();
			ResourceManager.RegisterCustomWebResourceHandler("ImageLoadingUrl",
				GetImageLoadingUrlResourceDelegate(ResourceManager.GetResourceUrl(Page, typeof(ASPxRichEdit), ImageLoadingResourceName)));
		}
		static Function<string, string> GetImageLoadingUrlResourceDelegate(string imageLoadingUrl) {
			return delegate(string p) {
				return imageLoadingUrl;
			};
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			RegisterScriptBlock("RichEditDialogTitles", RenderUtils.GetScriptHtml(GetDialogTitles()));
			RegisterScriptBlock("RichEditErrorDialogTexts", RenderUtils.GetScriptHtml(GetErrorDialogTexts()));
			RegisterScriptBlock("RichEditDialogOtherLabels", RenderUtils.GetScriptHtml(GetDialogOtherLabels()));
		}
		protected string GetDialogTitles() {
			return "ASPxClientRichEdit.ASPxRichEditDialogList.Titles=" + HtmlConvertor.ToJSON(new Hashtable() {
				{ "Columns", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ColumnsTitle) },
				{ "Error", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ErrorTitle) },
				{ "Font", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.FontTitle) },
				{ "InsertTable", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.InsertTableTitle) },
				{ "InsertTableCells", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.InsertTableCellsTitle) },
				{ "DeleteTableCells", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.DeleteTableCellsTitle) },
				{ "SplitTableCells", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.SplitTableCellsTitle) },
				{ "TableProperties", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.TablePropertiesTitle) },
				{ "BorderShading", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.BorderShadingTitle) },
				{ "InsertMergeField", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.InsertMergeFieldTitle) },
				{ "ExportRange", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ExportRangeTitle) },
				{ "Bookmark", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.BookmarkTitle) },
				{ "InsertImage", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.InsertImageTitle) },
				{ "OpenFile", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.OpenFileTitle) },
				{ "PageSetup", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PageSetupTitle) },
				{ "Paragraph", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ParagraphTitle) },
				{ "SaveAsFile", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.SaveAsFileTitle) },
				{ "BulletedAndNumbering", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.BulletedAndNumberingTitle) },
				{ "CustomizeNumberedList", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CustomizeNumberedListTitle) },
				{ "CustomizeBulletedList", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CustomizeBulletedListTitle) },
				{ "CustomizeOutlineNumbered", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CustomizeOutlineNumberedTitle) },
				{ "Hyperlink", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.HyperlinkTitle) },
				{ "Tabs", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.TabsTitle) },
				{ "Symbols", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.SymbolsTitle) }
			});
		}
		protected string GetErrorDialogTexts() {
			string clipboardAccessDeniedError = Browser.Platform.IsTouchUI ? ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ClipboardAccessDeniedErrorTouch)
				: string.Format(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ClipboardAccessDeniedError), Browser.Platform.IsMacOS ? "Command" : "Ctrl");
			return "ASPxClientRichEdit.ASPxRichEditDialogList.ErrorTexts=" + HtmlConvertor.ToJSON(new Hashtable() {
				{ (int)ErrorMessageText.ClipboardAccessDenied, clipboardAccessDeniedError },
				{ (int)ErrorMessageText.ModelIsChanged, ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ModelIsChangedError) },
				{ (int)ErrorMessageText.OpeningAndOverstoreImpossible, ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.OpeningAndOverstoreImpossibleError) },
				{ (int)ErrorMessageText.InnerException, ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.InnerExceptionsError) },
				{ (int)ErrorMessageText.AuthException, ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.AuthExceptionsError) },
				{ (int)ErrorMessageText.SessionHasExpired, ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.SessionHasExpiredError) },
				{ (int)ErrorMessageText.CantOpenFile, ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CantOpenDocumentError) },
				{ (int)ErrorMessageText.CantSaveFile, ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CantSaveDocumentError) },
				{ (int)ErrorMessageText.DocVariableException, ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.DocVariableExceptionError) },
				{ (int)ErrorMessageText.PathTooLongException, ASPxperienceLocalizer.GetString(ASPxperienceStringId.FileManager_ErrorPathToLong) }
			});
		}
		protected string GetDialogOtherLabels() {
			return "ASPxClientRichEdit.ASPxRichEditDialogList.OtherLabels=" + HtmlConvertor.ToJSON(new Hashtable() {
				{ "None", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.OtherLabels_None) },
				{ "All", ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.OtherLabels_All) }
			});
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientRichEdit";
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new RichEditClientSideEvents();
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool IsScriptEnabled() {
			return true;
		}
		protected internal void RaiseCalculateDocumentVariable(CalculateDocumentVariableEventArgs args) {
			CalculateDocumentVariableEventHandler handler = (CalculateDocumentVariableEventHandler)Events[CalculateDocumentVariableEventKey];
			if(handler != null)
				handler(this, args);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.ASPxRichEdit.Design.RichEditCommonFormDesigner"; } }
	}
	internal class ToolboxBitmapAccess { }
}
