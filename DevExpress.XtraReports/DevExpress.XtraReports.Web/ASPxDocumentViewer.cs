#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Drawing.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using DevExpress.DocumentServices.ServiceModel.ServiceOperations;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.DocumentViewer;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native;
using DevExpress.XtraReports.Web.Localization;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.DocumentViewer;
using Constants = DevExpress.XtraReports.Web.Native.Constants.DocumentViewer;
using ConstantsReportViewer = DevExpress.XtraReports.Web.Native.Constants.ReportViewer;
namespace DevExpress.XtraReports.Web {
	[DXWebToolboxItem(true)]
	[Designer("DevExpress.XtraReports.Web.Design.ASPxDocumentViewerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	[DefaultProperty("ReportTypeName")]
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ASPxDocumentViewer.bmp")]
	[ToolboxTabName(AssemblyInfo.DXTabReporting)]
	public class ASPxDocumentViewer : ASPxWebControl, IParentSkinOwner, IControlDesigner {
		#region resources
		new const string WebCssResourcePath = WebResourceNames.WebCssResourcePath;
		internal const string DocumentViewerNativeSpriteName = "dxxrdvSprite",
			SpriteCssResourceName = WebCssResourcePath + DocumentViewerNativeSpriteName + ".css",
			CssResourceName = WebCssResourcePath + "Default.css",
			SystemCssResourceName = WebCssResourcePath + "DocumentViewer.System.css";
		#endregion
		const string
			ReportTypeNameName = "ReportTypeName",
			ToolbarModeName = "ToolbarMode",
			AssociatedRibbonIDName = "AssociatedRibbonID",
			AutoHeightName = "AutoHeight",
			StylesCategoryName = "Styles",
			SettingsCategoryName = "Settings",
			ClientSideCategoryName = "Client-Side",
			RibbonCategoryName = "Ribbon",
			AppearanceCategoryName = "Appearance",
			ReportCategoryName = "Report";
		XtraReport report;
		bool shouldUpdateDocumentMapOnCallback;
		string command;
		internal bool CreatingBySwitchInDesignMode { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public DocumentViewerControl DocumentViewerInternal { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerSettingsRemoteSource")]
#endif
		[Category(ReportCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatDisable]
		public DocumentViewerRemoteSourceSettings SettingsRemoteSource { get; private set; }
		[Category(SettingsCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatDisable]
		public DocumentViewerReportViewerSettings SettingsReportViewer { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerSettingsDocumentMap")]
#endif
		[Category(SettingsCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatDisable]
		public DocumentViewerDocumentMapSettings SettingsDocumentMap { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerSettingsSplitter")]
#endif
		[Category(SettingsCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatDisable]
		public DocumentViewerSplitterSettings SettingsSplitter { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerSettingsRibbon")]
#endif
		[Category(SettingsCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatDisable]
		public DocumentViewerRibbonSettings SettingsRibbon { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerSettingsToolbar")]
#endif
		[Category(SettingsCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatDisable]
		public DocumentViewerReportToolbarProperties SettingsToolbar { get; private set; }
		[Category(SettingsCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatDisable]
		public ReportParametersPanelEditorCaptionSettings SettingsParametersPanelCaption { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerSettingsLoadingPanel")]
#endif
		[Category(SettingsCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatDisable]
		public new SettingsLoadingPanel SettingsLoadingPanel { get { return base.SettingsLoadingPanel; } }
		[Category(SettingsCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatDisable]
		public DropDownButton SettingsDropDownEditButton { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerLoadingPanelStyle")]
#endif
		[Category(StylesCategoryName)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatEnable]
		public new LoadingPanelStyle LoadingPanelStyle { get { return base.LoadingPanelStyle; } }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerStylesReportViewer")]
#endif
		[Category(StylesCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		public DocumentViewerViewerStyles StylesReportViewer { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerStylesToolbar")]
#endif
		[Category(StylesCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		public DocumentViewerReportToolbarStyles StylesToolbar { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerStylesDocumentMap")]
#endif
		[Category(StylesCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		public ReportDocumentMapStyles StylesDocumentMap { get; private set; }
		[Obsolete("Use the StylesParametersPanelEditors property instead.")]
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerStylesParametersPanelParameterEditors")]
#endif
		[Category(StylesCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		public ReportParametersPanelEditorStyles StylesParametersPanelParameterEditors {
			get { return StylesParametersPanelEditors; }
		}
		[Category(StylesCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		public ReportParametersPanelEditorStyles StylesParametersPanelEditors { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerStylesEditors")]
#endif
		[Category(StylesCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		public EditorStyles StylesEditors { get; private set; }
		[Category(StylesCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		public DocumentViewerParametersPanelButtonControlStyles StylesParametersPanelButtons { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerStylesSplitter")]
#endif
		[Category(StylesCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		public DocumentViewerSplitterStyles StylesSplitter { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerStylesRibbon")]
#endif
		[Category(StylesCategoryName)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[AutoFormatEnable]
		public DocumentViewerRibbonStyles StylesRibbon { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerToolbarItems")]
#endif
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DefaultValue((string)null)]
		[MergableProperty(false)]
		[AutoFormatDisable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ReportToolbarItemCollection ToolbarItems { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerClientSideEvents")]
#endif
		[MergableProperty(false), Category(ClientSideCategoryName)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatDisable]
		public DocumentViewerClientSideEvents ClientSideEvents {
			get { return (DocumentViewerClientSideEvents)base.ClientSideEventsInternal; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerLoadingPanelImage")]
#endif
		[Category("Images")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerReport")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public XtraReport Report {
			get { return report; }
			set {
				report = value;
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerReportTypeName")]
#endif
		[Category(ReportCategoryName)]
		[TypeConverter("DevExpress.Web.Design.Reports.Converters.ReportTypeNameConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		[DefaultValue("")]
		[AutoFormatDisable]
		[Localizable(false)]
		[NotifyParentProperty(true)]
		public string ReportTypeName {
			get { return GetStringProperty(ReportTypeNameName, ""); }
			set {
				SetStringProperty(ReportTypeNameName, "", value);
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerClientInstanceName")]
#endif
		[AutoFormatDisable]
		[Category(ClientSideCategoryName)]
		[DefaultValue("")]
		[Localizable(false)]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerAssociatedRibbonID")]
#endif
		[AutoFormatDisable]
		[Category(RibbonCategoryName)]
		[DefaultValue("")]
		[Localizable(false)]
		public string AssociatedRibbonID {
			get { return GetStringProperty(AssociatedRibbonIDName, ""); }
			set { SetStringProperty(AssociatedRibbonIDName, "", value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerToolbarMode")]
#endif
		[AutoFormatDisable]
		[Category(AppearanceCategoryName)]
		[DefaultValue(Constants.ToolbarModeDefault)]
		[Localizable(false)]
		public DocumentViewerToolbarMode ToolbarMode {
			get { return (DocumentViewerToolbarMode)GetEnumProperty(ToolbarModeName, Constants.ToolbarModeDefault); }
			set {
				SetEnumProperty(ToolbarModeName, Constants.ToolbarModeDefault, value);
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerAutoHeight")]
#endif
		[AutoFormatDisable]
		[Category("Layout")]
		[DefaultValue(Constants.AutoHeightDefault)]
		[Localizable(false)]
		public bool AutoHeight {
			get { return GetBoolProperty(AutoHeightName, Constants.AutoHeightDefault); }
			set {
				SetBoolProperty(AutoHeightName, Constants.AutoHeightDefault, value);
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerSpriteCssFilePath")]
#endif
		[Category("Images")]
		[DefaultValue("")]
		[Localizable(false)]
		[UrlProperty]
		[Editor(typeof(UrlEditor), typeof(UITypeEditor))]
		[AutoFormatEnable]
		[AutoFormatUrlProperty]
		[NotifyParentProperty(true)]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerImageFolder")]
#endif
		[Category("Images")]
		[DefaultValue("")]
		[UrlProperty]
		[AutoFormatEnable]
		[AutoFormatImageFolderProperty]
		[AutoFormatUrlProperty]
		[Localizable(false)]
		public string ImageFolder {
			get { return ImageFolderInternal; }
			set { ImageFolderInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerSpriteImageUrl")]
#endif
		[Category("Images")]
		[DefaultValue("")]
		[Localizable(false)]
		[UrlProperty]
		[AutoFormatEnable]
		[AutoFormatUrlProperty]
		[Editor(typeof(UrlEditor), typeof(UITypeEditor))]
		public string SpriteImageUrl {
			get { return SpriteImageUrlInternal; }
			set { SpriteImageUrlInternal = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public DocumentViewerRemoteSourceConfiguration ConfigurationRemoteSource { get; set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerAccessibilityCompliant")]
#endif
		[Category("Accessibility")]
		[DefaultValue(false)]
		[AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerJSProperties")]
#endif
		[Category("Client-Side")]
		[Browsable(false)]
		[AutoFormatDisable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		internal bool IsRightToLeftInternal {
			get { return IsRightToLeft(); }
		}
		static ASPxDocumentViewer() {
			ServiceOperationBase.DelayerFactory = new SynchronizedDelayerFactory(null);
		}
		public ASPxDocumentViewer() {
			ToolbarItems = new ReportToolbarItemCollection(this);
			SettingsRemoteSource = new DocumentViewerRemoteSourceSettings(this);
			SettingsReportViewer = new DocumentViewerReportViewerSettings(this);
			SettingsDocumentMap = new DocumentViewerDocumentMapSettings(this);
			SettingsSplitter = new DocumentViewerSplitterSettings(this);
			SettingsRibbon = new DocumentViewerRibbonSettings(this);
			SettingsToolbar = new DocumentViewerReportToolbarProperties(this);
			SettingsParametersPanelCaption = new ReportParametersPanelEditorCaptionSettings(this);
			SettingsDropDownEditButton = new DropDownButton(this);
			StylesReportViewer = new DocumentViewerViewerStyles(this);
			StylesToolbar = new DocumentViewerReportToolbarStyles(this);
			StylesDocumentMap = new ReportDocumentMapStyles(this);
			StylesEditors = new EditorStyles(this);
			StylesParametersPanelButtons = new DocumentViewerParametersPanelButtonControlStyles(this);
			StylesParametersPanelEditors = new ReportParametersPanelEditorStyles(this);
			StylesSplitter = new DocumentViewerSplitterStyles(this);
			StylesRibbon = new DocumentViewerRibbonStyles(this);
		}
		protected override string GetSkinControlName() {
			return "XtraReports";
		}
		protected override ImagesBase CreateImages() {
			return new DocumentViewerImages(this);
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ReportToolbar), ReportToolbar.CssResourceName);
		}
		protected override void RegisterSystemCssFile() {
			base.RegisterSystemCssFile();
			ResourceManager.RegisterCssResource(Page, typeof(ASPxDocumentViewer), ASPxDocumentViewer.SystemCssResourceName);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] {
					ToolbarItems,
					SettingsRemoteSource,
					SettingsReportViewer,
					SettingsParametersPanelCaption,
					SettingsDocumentMap,
					SettingsSplitter,
					SettingsRibbon,
					SettingsToolbar,
					SettingsDropDownEditButton,
					StylesReportViewer,
					StylesToolbar,
					StylesDocumentMap,
					StylesParametersPanelButtons,
					StylesEditors,
					StylesParametersPanelEditors,
					StylesSplitter,
					StylesRibbon
				});
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(SettingsSplitter.DocumentMapCollapsed)
				stb.AppendLine(localVarName + ".forceDocumentMapCollapsed = true;");
			if(ToolbarMode == DocumentViewerToolbarMode.Ribbon || ToolbarMode == DocumentViewerToolbarMode.ExternalRibbon) {
				stb.AppendFormat("{0}.pageCountText='{1}';", localVarName, ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonPageCountText));
				if(ToolbarMode == DocumentViewerToolbarMode.ExternalRibbon)
					stb.AppendFormat("{0}.externalRibbonID = '{1}';", localVarName, ExternalRibbonClientID);
			}
		}
		protected virtual string ExternalRibbonClientID {
			get {
				ASPxRibbon extRibbon = null;
				if(ToolbarMode == DocumentViewerToolbarMode.ExternalRibbon && Page != null)
					extRibbon = DocumentViewerRibbonHelper.GetRibbonControl(Page, AssociatedRibbonID);
				return extRibbon == null ? string.Empty : extRibbon.ClientID;
			}
		}
		#region events
		static readonly object CacheReportDocumentEvent = new object();
		static readonly object RestoreReportDocumentFromCacheEvent = new object();
		static readonly object CustomizeParameterEditorsEvent = new object();
		static readonly object DeserializeClientParametersEvent = new object();
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerCacheReportDocument")]
#endif
		public event CacheReportDocumentEventHandler CacheReportDocument {
			add { Events.AddHandler(CacheReportDocumentEvent, value); }
			remove { Events.AddHandler(CacheReportDocumentEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerRestoreReportDocumentFromCache")]
#endif
		public event RestoreReportDocumentFromCacheEventHandler RestoreReportDocumentFromCache {
			add { Events.AddHandler(RestoreReportDocumentFromCacheEvent, value); }
			remove { Events.RemoveHandler(RestoreReportDocumentFromCacheEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerCustomizeParameterEditors")]
#endif
		public event EventHandler<CustomizeParameterEditorsEventArgs> CustomizeParameterEditors {
			add { Events.AddHandler(CustomizeParameterEditorsEvent, value); }
			remove { Events.RemoveHandler(CustomizeParameterEditorsEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerDeserializeClientParameters")]
#endif
		public event EventHandler<DeserializeClientParameterEventArgs> DeserializeClientParameters {
			add { Events.AddHandler(DeserializeClientParametersEvent, value); }
			remove { Events.RemoveHandler(DeserializeClientParametersEvent, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxDocumentViewerCustomJSProperties")]
#endif
		[Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		internal void RaiseCacheReportDocument(object sender, CacheReportDocumentEventArgs e) {
			var handler = Events[CacheReportDocumentEvent] as CacheReportDocumentEventHandler;
			if(handler != null)
				handler(this, e);
		}
		internal void RaiseRestoreReportDocumentFromCache(object sender, RestoreReportDocumentFromCacheEventArgs e) {
			var handler = Events[RestoreReportDocumentFromCacheEvent] as RestoreReportDocumentFromCacheEventHandler;
			if(handler != null)
				handler(this, e);
		}
		internal void RaiseCustomizeParameterEditors(object sender, CustomizeParameterEditorsEventArgs e) {
			var handler = Events[CustomizeParameterEditorsEvent] as EventHandler<CustomizeParameterEditorsEventArgs>;
			if(handler != null) {
				handler(this, e);
			} else if(CheckIsCustomParameterType(e.Parameter)) {
				e.ShouldSetParameterValue = false;
			}
		}
		internal void RaiseDeserializeClientParameters(DeserializeClientParameterEventArgs e) {
			var handler = Events[DeserializeClientParametersEvent] as EventHandler<DeserializeClientParameterEventArgs>;
			if(handler != null)
				handler(this, e);
		}
		#endregion
		internal void ForceLayoutChanged() {
			LayoutChanged();
		}
		protected override bool HasContent() {
			return !CreatingBySwitchInDesignMode;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			DocumentViewerInternal = new DocumentViewerControl(this);
			Controls.Add(DocumentViewerInternal);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			DocumentViewerInternal = null;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientDocumentViewer";
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ReportViewer), ReportViewer.ScriptResourceName);
			RegisterIncludeScript(typeof(ReportDocumentMap), WebResourceNames.DocumentMap.ScriptResourceName);
			RegisterIncludeScript(typeof(ASPxDocumentViewer), WebResourceNames.DocumentViewer.ScriptResourceName);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new DocumentViewerClientSideEvents();
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		#region callback
		static string GetCommandName(string eventArgument) {
			var parts = eventArgument.Split('=');
			return parts.Length > 0
				? parts[0]
				: null;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			EnsureChildControls();
			command = GetCommandName(eventArgument);
			shouldUpdateDocumentMapOnCallback = command == ConstantsReportViewer.SubmitParametersCallbackName
				|| (command == "page" && DocumentViewerInternal.RemoteMode && !DocumentViewerInternal.Viewer.HasRemoteDocumentInformation);
			if(command == DocumentViewerReportParametersPanel.CascadeLookupsCallbackName) {
				DocumentViewerInternal.ParametersPanel.RaiseEditorsCallbackEventCore(eventArgument);
			} else {
				DocumentViewerInternal.Viewer.RaiseCallbackEventCore(eventArgument);
			}
		}
		protected override object GetCallbackResult() {
			EnsureChildControls();
			BeginRendering();
			try {
				var dict = new Dictionary<string, object>(2);
				if(command == DocumentViewerReportParametersPanel.CascadeLookupsCallbackName) {
					dict[DocumentViewerReportParametersPanel.CascadeLookupsCallbackName] = DocumentViewerInternal.ParametersPanel.GetCascadeLookupsCallbackResultCore();
				} else {
					dict["viewer"] = DocumentViewerInternal.Viewer.GetCallbackResultCore();
				}
				if(shouldUpdateDocumentMapOnCallback) {
					dict["documentMap"] = RenderUtils.GetRenderResult(DocumentViewerInternal.DocumentMap);
				}
				return dict;
			} finally {
				EndRendering();
			}
		}
		#endregion
		string IControlDesigner.DesignerType {
			get { return "DevExpress.Web.Design.Reports.DocumentViewer.DocumentViewerCommonFormDesigner"; }
		}
		bool CheckIsCustomParameterType(Parameters.Parameter parameter) {
			string result;
			return parameter.Value != null && SerializationService.SerializeObject(parameter.Value, out result, DocumentViewerInternal.Viewer.ForcedReport);
		}
	}
}
