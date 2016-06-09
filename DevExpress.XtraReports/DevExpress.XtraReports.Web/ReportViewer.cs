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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Navigation;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Forms;
using DevExpress.XtraReports.Web.Localization;
using DevExpress.XtraReports.Web.Native;
using Constants = DevExpress.XtraReports.Web.Native.Constants.ReportViewer;
namespace DevExpress.XtraReports.Web {
	public delegate void CacheReportDocumentEventHandler(object sender, CacheReportDocumentEventArgs e);
	public delegate void RestoreReportDocumentFromCacheEventHandler(object sender, RestoreReportDocumentFromCacheEventArgs e);
	public delegate string ReportViewerCallback();
#if !DEBUG
#endif // DEBUG
	[Designer("DevExpress.XtraReports.Web.Design.ReportViewerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	[DefaultProperty("ReportTypeName")]
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ReportViewer.bmp")]
	[ToolboxTabName(AssemblyInfo.DXTabReporting)]
	[ToolboxItem(false)]
	public class ReportViewer : ASPxWebControl, IParentSkinOwner {
		#region resources
		const string WebScriptResourcePath = WebResourceNames.WebScriptResourcePath;
		internal const string
			ScriptResourceName = WebScriptResourcePath + "ReportViewer.js",
			OperaScriptResourceName = WebScriptResourcePath + "ReportViewerOpera.js",
			PrintHelperScriptResourceName = WebScriptResourcePath + "PrintHelper.js",
			SearcherScriptResourceName = WebScriptResourcePath + "Searcher.js";
		#endregion
		#region static
		const string EnableRequestParametersPropertyName = "EnableRequestParameters";
		const bool DefaultEnableRequestParameters = false;
		const bool DefaultUseIFrame = true;
		const string CacheKey = "cacheKey";
		const string CurrentPageIndexKey = "currentPageIndex";
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void AssignStyles(WebControl source, WebControl destination) {
			destination.ControlStyle.CopyFrom(source.ControlStyle);
			destination.CssClass = source.CssClass;
			destination.CopyBaseAttributes(source);
		}
		static readonly Border DefaultBookmarkBorder = new Border(Color.Gray, BorderStyle.Dashed, new Unit(3, UnitType.Pixel));
		public static void WritePdfTo(HttpResponse response, XtraReport report) {
			CreateReportViewer(report).WritePdfTo(response);
		}
		public static void WriteRtfTo(HttpResponse response, XtraReport report) {
			CreateReportViewer(report).WriteRtfTo(response);
		}
		public static void WriteXlsTo(HttpResponse response, XtraReport report) {
			CreateReportViewer(report).WriteXlsTo(response);
		}
		public static void WriteXlsxTo(HttpResponse response, XtraReport report) {
			CreateReportViewer(report).WriteXlsxTo(response);
		}
		public static void WriteMhtTo(HttpResponse response, XtraReport report) {
			CreateReportViewer(report).WriteMhtTo(response);
		}
		public static void WriteHtmlTo(HttpResponse response, XtraReport report) {
			CreateReportViewer(report).WriteHtmlTo(response);
		}
		#endregion
		XtraReport report;
		XtraReport reportNotFromCache;
		bool pageByPage;
		string reportName = string.Empty;
		WebControl bookmarkDivControl;
		string cacheKey;
		int currentPageIndex = -1;
		Dictionary<string, ReportViewerCallback> callBacks;
		ReportWebMediator reportWebMediator;
		ContentBase content;
		bool useIFrame = DefaultUseIFrame;
		bool isDataBound;
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal bool UseClientParameters { get; set; }
		#region Suppressed properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Cursor {
			get { return base.Cursor; }
			set { base.Cursor = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override BackgroundImage BackgroundImage {
			get { return base.BackgroundImage; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Enabled {
			get { return base.Enabled; }
			set { base.Enabled = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
		#endregion
		#region style properties
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerImageFolder")]
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
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerSpriteImageUrl")]
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
		[
#if !SL
	DevExpressXtraReportsWebLocalizedDescription("ReportViewerSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty,
		Editor(typeof(UrlEditor), typeof(UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerSearchDialogEditorsImages")]
#endif
		[Category("Images")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorImages SearchDialogEditorsImages { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerSearchDialogFormImages")]
#endif
		[Category("Images")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImagesDialogForm SearchDialogFormImages { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerSearchDialogEditorsStyles")]
#endif
		[Category("Styles")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public EditorStyles SearchDialogEditorsStyles { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerSearchDialogButtonStyles")]
#endif
		[Category("Styles")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonControlStyles SearchDialogButtonStyles { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerSearchDialogFormStyles")]
#endif
		[Category("Styles")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public PopupControlStyles SearchDialogFormStyles { get; private set; }
		#endregion
		#region Loading Panel properties
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerSettingsLoadingPanel")]
#endif
		[Category("Settings")]
		[AutoFormatEnable]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerLoadingPanelDelay")]
#endif
		[Category("Behavior")]
		[DefaultValue(SettingsLoadingPanel.DefaultDelay)]
		[AutoFormatDisable]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerLoadingPanelText")]
#endif
		[Category("Loading Panel")]
		[DefaultValue(StringResources.LoadingPanelText)]
		[AutoFormatEnable]
		[Localizable(true)]
		public string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerLoadingPanelImage")]
#endif
		[Category("Loading Panel")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatEnable]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerLoadingPanelImagePosition")]
#endif
		[Category("Loading Panel")]
		[DefaultValue(ImagePosition.Left)]
		[AutoFormatEnable]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerLoadingPanelStyle")]
#endif
		[Category("Loading Panel")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatEnable]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerShowLoadingPanelImage")]
#endif
		[Category("Loading Panel")]
		[DefaultValue(true)]
		[AutoFormatEnable]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		#endregion
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerClientInstanceName")]
#endif
		[AutoFormatDisable]
		[Category("Client-Side")]
		[DefaultValue("")]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		internal bool FileImageCache {
			get { return ReportWebMediator.FileImageCache; }
		}
		XtraReport ReportInternal {
			get { return report; }
			set {
				report = value;
				if(reportWebMediator != null)
					reportWebMediator.AssignReport(report);
			}
		}
		internal XtraReport ForcedReport {
			get {
				ForceReportInstance();
				return ReportInternal;
			}
		}
		internal virtual bool ShouldRequestParametersFirst {
			get {
				return ForcedShouldRequestParametersFirst
					|| (Report != null
					&& EnableRequestParameters
					&& Report.RequestParameters
					&& ReportHasVisibleParameters);
			}
		}
		internal virtual bool RemoteMode {
			get { return false; }
			set { throw new NotSupportedException(); }
		}
		internal bool ForcedShouldRequestParametersFirst { get; set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerEnableRequestParameters")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[AutoFormatDisable]
		[DefaultValue(DefaultEnableRequestParameters)]
		public bool EnableRequestParameters {
			get { return GetBoolProperty(EnableRequestParametersPropertyName, DefaultEnableRequestParameters); }
			set { SetBoolProperty(EnableRequestParametersPropertyName, DefaultEnableRequestParameters, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerReport")]
#endif
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public XtraReport Report {
			get { return ReportInternal ?? reportNotFromCache; }
			set {
				reportNotFromCache = value;
				OnReportChange();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerReportName")]
#endif
		[SRCategory(ReportStringId.CatData)]
		[DefaultValue("")]
		[TypeConverter("DevExpress.Web.Design.Reports.Converters.ReportTypeNameConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string ReportName {
			get { return reportName; }
			set {
				reportName = value;
				OnReportChange();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerPageByPage")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[AutoFormatDisable]
		[DefaultValue(Constants.DefaultPageByPage)]
		public bool PageByPage {
			get { return pageByPage; }
			set {
				pageByPage = value;
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerPrintUsingAdobePlugIn")]
#endif
		[AutoFormatDisable]
		[SRCategory(ReportStringId.CatBehavior)]
		[DefaultValue(Constants.DefaultPrintUsingAdobePlugIn)]
		public bool PrintUsingAdobePlugIn { get; set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerAutoSize")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[RefreshProperties(RefreshProperties.All)]
		[AutoFormatDisable]
		[DefaultValue(Constants.DefaultAutoSize)]
		public bool AutoSize { get; set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerTableLayout")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[RefreshProperties(RefreshProperties.All)]
		[AutoFormatDisable]
		[DefaultValue(Constants.DefaultTableLayout)]
		public bool TableLayout { get; set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerImagesEmbeddingMode")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[RefreshProperties(RefreshProperties.All)]
		[AutoFormatDisable]
		[DefaultValue(Constants.DefaultImagesEmbeddingMode)]
		public ImagesEmbeddingMode ImagesEmbeddingMode {
			get { return (ImagesEmbeddingMode)GetEnumProperty(Constants.ImagesEmbeddingModeName, Constants.DefaultImagesEmbeddingMode); }
			set { SetEnumProperty(Constants.ImagesEmbeddingModeName, Constants.DefaultImagesEmbeddingMode, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerWidth")]
#endif
		[AutoFormatDisable]
		[TypeConverter("DevExpress.Web.Design.Reports.Converters.ReportViewerSizeConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerHeight")]
#endif
		[AutoFormatDisable]
		[TypeConverter("DevExpress.Web.Design.Reports.Converters.ReportViewerSizeConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerClientSideEvents")]
#endif
		[Category("Client-Side")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatDisable]
		[MergableProperty(false)]
		public ReportViewerClientSideEvents ClientSideEvents {
			get { return (ReportViewerClientSideEvents)base.ClientSideEventsInternal; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerPaddings")]
#endif
		[SRCategory(ReportStringId.CatLayout)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return ((AppearanceStyle)ControlStyle).Paddings; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerBookmarkSelectionBorder")]
#endif
		[SRCategory(ReportStringId.CatAppearance)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Border BookmarkSelectionBorder {
			get { return ((ReportViewerStyle)ControlStyle).BookmarkSelectionBorder; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerEnableCallbackCompression")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[DefaultValue(true)]
		[AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerShouldDisposeReport")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(Constants.DefaultShouldDisposeReport)]
		[AutoFormatEnable]
		public bool ShouldDisposeReport {
			get { return GetBoolProperty(Constants.ShouldDisposeReportName, Constants.DefaultShouldDisposeReport); }
			set { SetBoolProperty(Constants.ShouldDisposeReportName, Constants.DefaultShouldDisposeReport, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ReportViewerEnableReportMargins")]
#endif
		[SRCategory(ReportStringId.CatAppearance)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(Constants.DefaultEnableReportMargins)]
		public bool EnableReportMargins {
			get { return GetBoolProperty(Constants.EnableReportMarginsName, Constants.DefaultEnableReportMargins); }
			set { SetBoolProperty(Constants.EnableReportMarginsName, Constants.DefaultEnableReportMargins, value); }
		}
		internal XtraReport ActualReport {
			get { return ReportInternal; }
		}
		Dictionary<string, ReportViewerCallback> CallBacks {
			get {
				if(callBacks == null) {
					callBacks = new Dictionary<string, ReportViewerCallback>();
					RegisterCallbacks(callBacks);
				}
				return callBacks;
			}
		}
		internal int PageCount {
			get {
				return ReportViewer.IsReportReady(ReportInternal)
					? XtraReportAccessor.GetReportDocument(ReportInternal).PageCount
					: 0;
			}
		}
		internal int CurrentPageIndex {
			get {
				if(currentPageIndex < 0) {
					currentPageIndex = GetClientObjectStateValue<int>(CurrentPageIndexKey);
				}
				currentPageIndex = GetPageIndex(currentPageIndex);
				return currentPageIndex;
			}
			set {
				currentPageIndex = value;
			}
		}
		protected ReportWebMediator ReportWebMediator {
			get {
				if(reportWebMediator == null) {
					var clientParameters = GetClientObjectStateValue<Hashtable>(Constants.ClientParametersKey);
					var clientDrillDownKeys = GetClientObjectStateValue<Hashtable>(Constants.ClientDrillDownKey);
					reportWebMediator = CreateReportWebMediator(ForcedReport, clientParameters, clientDrillDownKeys, OnDeserializeClientParameters);
				}
				return reportWebMediator;
			}
		}
#if DEBUGTEST
		internal ReportWebMediator ReportWebMediator_TEST {
			get { return ReportWebMediator; }
		}
#endif
		protected ContentBase Content {
			get {
				if(content == null) {
					content = useIFrame
						? (ContentBase)new ContentUseIFrame(this)
						: new ContentUseDiv(this);
				}
				return content;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DefaultValue(DefaultUseIFrame)]
		public bool UseIFrame {
			get { return useIFrame; }
			set {
				if(useIFrame != value) {
					if(content != null) {
						content.ResetControlHierarchy();
						LayoutChanged();
						content = null;
					}
					useIFrame = value;
				}
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(true)]
		protected internal virtual bool AllowNullReport {
			get { return false; }
		}
		bool ReportHasVisibleParameters {
			get {
				return new NestedParameterPathCollector()
					.EnumerateParameters(Report)
					.Any(x => x.Parameter.Visible);
			}
		}
		internal bool ReportHasVisibleParametersInternal {
			get { return ForcedReport != null && ReportHasVisibleParameters; }
		}
		#region events
		static readonly object CacheReportDocumentEvent = new object();
		static readonly object RestoreReportDocumentFromCacheEvent = new object();
		static readonly object DeserializeClientParametersEvent = new object();
		public event CacheReportDocumentEventHandler CacheReportDocument {
			add { Events.AddHandler(CacheReportDocumentEvent, value); }
			remove { Events.RemoveHandler(CacheReportDocumentEvent, value); }
		}
		public event RestoreReportDocumentFromCacheEventHandler RestoreReportDocumentFromCache {
			add { Events.AddHandler(RestoreReportDocumentFromCacheEvent, value); }
			remove { Events.RemoveHandler(RestoreReportDocumentFromCacheEvent, value); }
		}
		public event EventHandler<DeserializeClientParameterEventArgs> DeserializeClientParameters {
			add { Events.AddHandler(DeserializeClientParametersEvent, value); }
			remove { Events.RemoveHandler(DeserializeClientParametersEvent, value); }
		}
		protected virtual void OnCacheReportDocument() {
			var handler = Events[CacheReportDocumentEvent] as CacheReportDocumentEventHandler;
			if(handler != null) {
				var arg = new CacheReportDocumentEventArgs(ReportInternal);
				handler(this, arg);
				cacheKey = arg.Key;
			}
		}
		protected virtual void OnRestoreReportDocumentFromCache(RestoreReportDocumentFromCacheEventArgs e) {
			var handler = Events[RestoreReportDocumentFromCacheEvent] as RestoreReportDocumentFromCacheEventHandler;
			if(handler != null)
				handler(this, e);
		}
		void OnDeserializeClientParameters(DeserializeClientParameterEventArgs e) {
			var handler = Events[DeserializeClientParametersEvent] as EventHandler<DeserializeClientParameterEventArgs>;
			if(handler != null)
				handler(this, e);
		}
		#endregion
		public ReportViewer() {
			AutoSize = Constants.DefaultAutoSize;
			TableLayout = Constants.DefaultTableLayout;
			PageByPage = Constants.DefaultPageByPage;
			PrintUsingAdobePlugIn = Constants.DefaultPrintUsingAdobePlugIn;
			SearchDialogEditorsStyles = new EditorStyles(this);
			SearchDialogEditorsImages = new EditorImages(this);
			SearchDialogButtonStyles = new ButtonControlStyles(this);
			SearchDialogFormStyles = new PopupControlStyles(this);
			SearchDialogFormImages = new ImagesDialogForm(this);
		}
		protected override void OnInit(EventArgs e) {
			if(!DesignMode) {
				PrepareControl();
				if(BinaryStorage.ProcessRequest()) {
					return;
				}
			}
			base.OnInit(e);
		}
		protected override void EnsurePreRender() {
			if(!DesignMode && !ShouldRequestParametersFirst && !useIFrame)
				ForcePSDocument();
			base.EnsurePreRender();
		}
		void OnReportChange() {
			CurrentPageIndex = -1;
			LayoutChanged();
		}
		protected override void OnLoad(EventArgs e) {
			if(Page != null && Page.Header != null && !Page.IsCallback) {
				if(Browser.IsIE)
					Page.Header.Controls.Add(RenderUtils.CreateLiteralControl(@"<style id=""DXRPrintHideContent"" type=""text/css"" media=""print"" disabled=""true"" > * { display:none; }</style>"));
			}
			if(reportNotFromCache != null)
				DataBind();
			base.OnLoad(e);
		}
		internal protected virtual void ForcePSDocument() {
			if(DesignMode) {
				return;
			}
			ForceReportInstance();
			if(UseClientParameters)
				ReportWebMediator.AssignClientStateToReport();
			if(ReportInternal == null || !ReportWebMediator.ClientParametersWereChanged) {
				var eventArgs = new RestoreReportDocumentFromCacheEventArgs(ForcedReport, GetCacheKeyFromStateObject());
				OnRestoreReportDocumentFromCache(eventArgs);
				if(eventArgs.IsRestored) {
					ReportInternal = eventArgs.Report;
					return;
				}
			}
			if(ReportInternal == null)
				return;
			if(XtraReportAccessor.GetReportDocument(ReportInternal).PageCount == 0)
				ReportInternal.CreateDocument();
			OnCacheReportDocument();
		}
		void ForceReportInstance() {
			if(ReportInternal != null)
				return;
			DataBind();
			ReportInternal = reportNotFromCache;
			if(ReportInternal != null || string.IsNullOrEmpty(reportName)) {
				return;
			}
			try {
				var reportResolver = new ReportResolver();
				ReportInternal = reportResolver.Resolve(reportName, IsMvcRender() ? null : Page);
				reportNotFromCache = ReportInternal;
			} catch { }
		}
		protected virtual string GetCacheKeyFromStateObject() {
			var encodedCacheKey = GetClientObjectStateValueString(CacheKey);
			return encodedCacheKey != null ? Uri.UnescapeDataString(encodedCacheKey) : string.Empty;
		}
		static void AppendStateObjectKeyAssignmentScriptForCallback(StringBuilder builder, string localVarName, string key, object value, bool convertToJson = false) {
			if(convertToJson) {
				value = Uri.EscapeDataString(HtmlConvertor.ToJSON(value, false, false, true));
			}
			const string SetStateObjectKeyFormatStringForCallback = "{0}.setStateObjectKey(\"{1}\",{2});";
			builder.AppendFormat(SetStateObjectKeyFormatStringForCallback, localVarName, key, value);
			builder.AppendLine();
		}
		string GetStateObjectKeysAssignmentsScriptForCallback(string localVarName) {
			var builder = new StringBuilder();
			var encodedCacheKey = cacheKey != null ? Uri.EscapeDataString(cacheKey) : string.Empty;
			AppendStateObjectKeyAssignmentScriptForCallback(builder, localVarName, CacheKey, "\"" + encodedCacheKey + "\"");
			AppendStateObjectKeyAssignmentScriptForCallback(builder, localVarName, Constants.ClientParametersKey, GetParameterValues(), convertToJson: true);
			AppendStateObjectKeyAssignmentScriptForCallback(builder, localVarName, Constants.ClientDrillDownKey, GetDrillDownKeys(), convertToJson: true);
			if(PageByPage) {
				AppendStateObjectKeyAssignmentScriptForCallback(builder, localVarName, CurrentPageIndexKey, CurrentPageIndex);
			}
			return builder.ToString();
		}
		IDictionary<string, object> GetParameterValues() {
			return UseClientParameters
				? ReportWebMediator.GetParameterValues()
				: new Dictionary<string, object>();
		}
		protected virtual IDictionary<string, bool> GetDrillDownKeys() {
			return ReportWebMediator.GetDrillDownKeys();
		}
		int GetPageIndex(int value) {
			return ReportInternal != null && PageCount > 0
				? Math.Max(0, Math.Min(value, PageCount - 1))
				: value;
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			if(string.IsNullOrEmpty(eventArgument))
				return;
			InitWebEventInfo(eventArgument);
			ForcePSDocument();
			ExportStreamInfo exportStreamInfo = ReportWebMediator.GetExportInfoAsync().Result;
			ExportStreamCache.WriteResponse(Page.Response, exportStreamInfo);
		}
		void InitWebEventInfo(string eventArgument) {
			ReportWebMediator.InitEventInfo(eventArgument);
		}
		public override void DataBind() {
			if(!isDataBound) {
				base.DataBind();
				isDataBound = true;
			}
		}
		protected override void Render(HtmlTextWriter writer) {
			if(!DesignMode || IsAutoFormatPreview) {
				if(ReportInternal != null && ReportInternal.ExportOptions.Html.RemoveSecondarySymbols)
					writer = new WebCompressedHtmlTextWriter(writer.InnerWriter);
				else if(writer is Html32TextWriter) {
					writer = new HtmlTextWriter(writer.InnerWriter);
				}
				base.Render(writer);
			}
		}
		protected override void PrepareControlHierarchy() {
			Content.PrepareControlHierarchy();
			if(PageByPage) {
				AssignStyles(this, bookmarkDivControl);
				BookmarkSelectionBorder.MergeWith(DefaultBookmarkBorder);
				BookmarkSelectionBorder.AssignToControl(bookmarkDivControl);
				bookmarkDivControl.Style["pointer-events"] = "none";
				bookmarkDivControl.Style[HtmlTextWriterStyle.Display] = "none";
			}
			base.PrepareControlHierarchy();
		}
		protected override void CreateControlHierarchy() {
			Content.CreateControlHierarchy();
			base.CreateControlHierarchy();
			if(PageByPage)
				bookmarkDivControl = CreateDivControl("Bookmark");
			if(IsAutoFormatPreview) {
				WebSearchDialog control = CreateSearchDialogControl();
				Controls.Add(control);
				control.ForceInitialize();
			}
		}
		WebSearchDialog CreateSearchDialogControl() {
			return new WebSearchDialog {
				ID = "SearchDialog",
				ReportViewer = this
			};
		}
		protected override void PrepareLoadingPanel(LoadingPanelControl loadingPanel) {
			base.PrepareLoadingPanel(loadingPanel);
			if(IsAutoFormatPreview)
				loadingPanel.DesignModeVisible = true;
		}
		internal WebControl CreateDivControl(string id) {
			WebControl div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(div);
			div.ID = id;
			return div;
		}
		protected override void OnUnload(EventArgs e) {
			XtraReport savedReport = reportNotFromCache;
			base.OnUnload(e);
			if(ReportInternal == savedReport)
				ReportInternal = reportNotFromCache;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public override void Dispose() {
			if(ReportInternal != null) {
				if(ReportInternal != reportNotFromCache)
					ReportInternal.Dispose();
				ReportInternal = null;
			}
			if(ShouldDisposeReport && reportNotFromCache != null) {
				reportNotFromCache.Dispose();
				reportNotFromCache = null;
			}
			base.Dispose();
		}
		protected virtual ReportWebMediator CreateReportWebMediator(XtraReport report, Hashtable clientParameters, Hashtable clientDrillDownKeys, Action<DeserializeClientParameterEventArgs> deserializeClientParameter) {
			return new ReportWebMediator(report, clientParameters, clientDrillDownKeys, deserializeClientParameter);
		}
		#region WriteTo* public methods
		public void WriteHtmlTo(string filePath) {
			ForcePSDocument();
			if(ReportInternal != null)
				ReportInternal.ExportToHtml(filePath);
		}
		public void WriteMhtTo(HttpResponse response) {
			WriteTo(response, "mht");
		}
		public void WriteHtmlTo(HttpResponse response) {
			WriteTo(response, "html");
		}
		public void WriteTiffImageTo(HttpResponse response) {
			WriteTo(response, "tiff");
		}
		public void WriteGifImageTo(HttpResponse response) {
			WriteTo(response, "gif");
		}
		public void WriteJpgImageTo(HttpResponse response) {
			WriteTo(response, "jpg");
		}
		public void WritePngImageTo(HttpResponse response) {
			WriteTo(response, "png");
		}
		public void WriteBmpImageTo(HttpResponse response) {
			WriteTo(response, "bmp");
		}
		public void WritePdfTo(HttpResponse response) {
			WriteTo(response, "pdf");
		}
		public void WriteXlsTo(HttpResponse response) {
			WriteTo(response, "xls");
		}
		public void WriteXlsxTo(HttpResponse response) {
			WriteTo(response, "xlsx");
		}
		public void WriteRtfTo(HttpResponse response) {
			WriteTo(response, "rtf");
		}
		public void WriteTextTo(HttpResponse response) {
			WriteTo(response, "txt");
		}
		public void WriteTextTo(HttpResponse response, string exportType, DevExpress.XtraPrinting.TextExportOptions options) {
			WriteTo(response, "txt", exportType, options);
		}
		void WriteTo(HttpResponse response, string documentType) {
			WriteTo(response, documentType, null, null);
		}
		void WriteTo(HttpResponse response, string documentType, string exportType, DevExpress.XtraPrinting.ExportOptionsBase options) {
			ForcePSDocument();
			ExportStreamCache.WriteTo(response, ReportInternal, documentType, exportType, options);
		}
		#endregion
		protected override Style CreateControlStyle() {
			return new ReportViewerStyle();
		}
		protected override StylesBase CreateStyles() {
			return new StylesBase(this);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ReportViewerClientSideEvents();
		}
		protected override LoadingPanelStyle GetLoadingPanelStyle() {
			LoadingPanelStyle style = DefaultLoadingPanelStyle;
			style.CopyFrom(base.GetLoadingPanelStyle());
			return style;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool HasLoadingPanel() {
			return Content.ShouldAddLoadingPanel || IsAutoFormatPreview;
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientReportViewer";
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ReportViewer), PrintHelperScriptResourceName);
			RegisterIncludeScript(typeof(ReportViewer), SearcherScriptResourceName);
			RegisterIncludeScript(typeof(ReportViewer), ScriptResourceName);
			RegisterIncludeScript(typeof(ReportViewer), OperaScriptResourceName, Browser.IsOpera);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!PrintUsingAdobePlugIn)
				Content.ActionBuilder.AddAssignment("printUsingAdobePlugIn", false);
			Content.GetCreateClientObjectScript(localVarName, clientName);
			stb.AppendLine(Content.ActionBuilder.RenderToScript(localVarName));
		}
		protected override Hashtable GetClientObjectState() {
			var encodedCacheKey = cacheKey != null ? Uri.EscapeDataString(cacheKey) : string.Empty;
			var result = new Hashtable();
			result.Add(CurrentPageIndexKey, CurrentPageIndex);
			result.Add(CacheKey, encodedCacheKey);
			result.Add(Constants.ClientParametersKey, GetParameterValues());
			result.Add(Constants.ClientDrillDownKey, GetDrillDownKeys());
			return result;
		}
		protected override object GetCallbackResult() {
			if(!ReportWebMediator.HasEventInfo)
				throw new InvalidOperationException("Invalid callback argument.");
			ForcePSDocument();
			string resultBody = CallBacks[ReportWebMediator.Event.EventId]();
			if(string.IsNullOrEmpty(resultBody))
				throw new InvalidOperationException("Invalid callback result.");
			const string JsLocalVarName = "this";
			var builder = new StringBuilder();
			AppendEncodedCallbackParam(builder, ReportWebMediator.Event.EventId);
			AppendEncodedCallbackParam(builder, GetStateObjectKeysAssignmentsScriptForCallback(JsLocalVarName));
			AppendEncodedCallbackParam(builder, Content.ActionBuilder.RenderToScript(JsLocalVarName));
			builder.Append(resultBody);
			return builder.ToString();
		}
		protected override string OnCallbackException(Exception e) {
			return HttpUtils.IsCustomErrorEnabled() ? base.OnCallbackException(e) : e.ToString();
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			InitWebEventInfo(eventArgument);
		}
		string GetPageCallBackResult(int pageIndex, bool addFindTextScript) {
			Content.AddPageLoadScript(pageIndex);
			if(addFindTextScript)
				Content.AddSelectTextScript(ReportWebMediator.Event.EventArgument);
			return RenderPageCallBackResult(pageIndex);
		}
		string RenderPageCallBackResult(int pageIndex) {
			if(PageCount == 0)
				return FetchEmptyContent();
			if(PageCount <= pageIndex)
				throw new ArgumentException("pageIndex");
			ImageCache.Clean(this);
			return new ReportRenderHelper(this).WritePage(pageIndex);
		}
		protected virtual string FetchEmptyContent() {
			return "<table></table>";
		}
		string GetWholeDocumentCallBackResult() {
			if(ReportInternal == null)
				return string.Empty;
			ForceLoadWholeDocument();
			if(PageCount == 0)
				return FetchEmptyContent();
			Content.ActionBuilder.AddAction("onPageLoad", 1);
			string content = new ReportRenderHelper(this).WriteWholeDocument();
			Content.ActionBuilder.AddAction("setViewSize");
			return content;
		}
		#region callback handlers
		internal protected virtual string CallbackRemotePage() {
			throw new NotSupportedException();
		}
		string CallbackPage() {
			return PageByPage ? GetPageCallBackResult(CurrentPageIndex, false) : GetWholeDocumentCallBackResult();
		}
		string CallbackPrint() {
			string pageIndexString = ReportWebMediator.Event.PageIndexString;
			int pageIndex = 0;
			pageIndex = !string.IsNullOrEmpty(pageIndexString)
				&& ReportViewer.IsReportReady(ForcedReport)
				&& int.TryParse(pageIndexString, out pageIndex)
				? GetPageIndex(pageIndex) : -1;
			return new ReportRenderHelper(this).WriteForPrinting(pageIndex);
		}
		string CallbackSearch() {
			string text = ReportWebMediator.Event.Txt;
			if(!ReportViewer.IsReportReady(ReportInternal))
				throw new InvalidOperationException("Report is not ready");
			if(string.IsNullOrEmpty(text))
				throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.SearchDialog_EnterText));
			ForceLoadWholeDocument();
			var selector = new TextBrickSelector(text, ConvertFromStringToBoolean(ReportWebMediator.Event.Word, false), ConvertFromStringToBoolean(ReportWebMediator.Event.Case, false), null);
			Document document = XtraReportAccessor.GetReportDocument(ReportInternal);
			BrickPagePairCollection bpPairs = NavigateHelper.SelectBrickPagePairs(document, selector, new BrickPagePairComparer(document.Pages));
			bool searchUp = ConvertFromStringToBoolean(ReportWebMediator.Event.Up, false);
			int pageIndex = searchUp ? SearchUp(bpPairs, CurrentPageIndex) : SearchDown(bpPairs, CurrentPageIndex);
			if(pageIndex >= 0)
				CurrentPageIndex = pageIndex;
			bool wasTextFounded = false;
			if(pageIndex >= 0)
				wasTextFounded = true;
			else
				Content.ActionBuilder.AddAction("alert", ASPxReportsLocalizer.GetString(ASPxReportsStringId.SearchDialog_Finished));
			return GetPageCallBackResult(CurrentPageIndex, wasTextFounded);
		}
		void ForceLoadWholeDocument() {
			PrintingSystemAccessor.ForceLoadDocument(ReportInternal.PrintingSystem);
		}
		string CallbackSearchControl() {
			WebSearchDialog searchControl = CreateSearchDialogControl();
			PrepareUserControl(searchControl, this, string.Empty, true);
			return RenderUtils.GetRenderResult(searchControl);
		}
		string CallbackBookmark() {
			int pageIndex = CurrentPageIndex;
			Content.AddPageLoadScript(pageIndex);
			Content.ActionBuilder.AddAction("HighlightBookmark", ReportWebMediator.Event.Path);
			return RenderPageCallBackResult(pageIndex);
		}
		#endregion
		protected virtual void RegisterCallbacks(Dictionary<string, ReportViewerCallback> callBacks) {
			callBacks["page"] = CallbackPage;
			callBacks["print"] = CallbackPrint;
			callBacks["search"] = CallbackSearch;
			callBacks["searchControl"] = CallbackSearchControl;
			callBacks["bookmark"] = CallbackBookmark;
			callBacks[Constants.SubmitParametersCallbackName] = CallbackPage;
		}
		internal static bool IsReportReady(XtraReport report) {
			return report != null && XtraReportAccessor.GetReportDocument(report) != null && XtraReportAccessor.GetReportDocument(report).PageCount > 0;
		}
		protected internal IImageRepository CreateImageRepository(bool printing) {
			var supportsCssImages = !printing;
			supportsCssImages &= ImagesEmbeddingMode == ImagesEmbeddingMode.Auto
				? !Browser.IsIE || Browser.MajorVersion > 8
				: ImagesEmbeddingMode == ImagesEmbeddingMode.Base64;
			return supportsCssImages
				? (IImageRepository)new CssImageRepository()
				: new WebImageRepository(this);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void PrepareControl() {
			cacheKey = GetCacheKeyFromStateObject();
		}
		static ReportViewer CreateReportViewer(XtraReport report) {
			return new ReportViewer {
				Report = report
			};
		}
		static bool ConvertFromStringToBoolean(string text, bool defaultValue) {
			if(string.IsNullOrEmpty(text)) {
				return defaultValue;
			}
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(bool));
			return (bool)converter.ConvertFromString(null, CultureInfo.InvariantCulture, text);
		}
		static LoadingPanelStyle DefaultLoadingPanelStyle {
			get {
				var style = new LoadingPanelStyle {
					ForeColor = Color.Black,
					BackColor = Color.White,
					HorizontalAlign = HorizontalAlign.Center,
					ImageSpacing = 5
				};
				style.Paddings.Padding = 5;
				style.Border.BorderStyle = BorderStyle.Solid;
				style.Border.BorderColor = Color.FromArgb(0x7B, 0x7B, 0x7B);
				style.Border.BorderWidth = 1;
				style.Font.Name = "Tahoma";
				style.Font.Size = 10;
				return style;
			}
		}
		static void AppendEncodedCallbackParam(StringBuilder builder, string param) {
			builder.AppendFormat("{0}|{1}", param.Length, param);
		}
		static int SearchDown(BrickPagePairCollection bpPairs, int pageIndex) {
			for(int i = 0; i < bpPairs.Count; i++) {
				if(bpPairs[i].PageIndex > pageIndex) {
					return bpPairs[i].PageIndex;
				}
			}
			return -1;
		}
		static int SearchUp(BrickPagePairCollection bpPairs, int pageIndex) {
			for(int i = bpPairs.Count - 1; i >= 0; i--) {
				if(bpPairs[i].PageIndex < pageIndex) {
					return bpPairs[i].PageIndex;
				}
			}
			return -1;
		}
	}
}
