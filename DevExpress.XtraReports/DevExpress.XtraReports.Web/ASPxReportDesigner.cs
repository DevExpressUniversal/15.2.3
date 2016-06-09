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
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.ReportDesigner.Native;
using DevExpress.XtraReports.Web.ReportDesigner.Native.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
using Constants = DevExpress.XtraReports.Web.Native.Constants.ReportDesigner;
using DevExpress.XtraReports.Web.QueryBuilder;
using DevExpress.XtraReports.Native;
using System.Web.SessionState;
namespace DevExpress.XtraReports.Web {
	[DXWebToolboxItem(true)]
	[Designer("DevExpress.XtraReports.Web.Design.ASPxReportDesignerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ASPxReportDesigner.bmp")]
	[ToolboxTabName(AssemblyInfo.DXTabReporting)]
	public class ASPxReportDesigner : ASPxWebClientUIControl, IControlDesigner {
		#region resources
		const string
			DXTremeCssPostfix = WebResourceNames.DXTremeCssPostfix,
			DXTremeJSPostfix = WebResourceNames.DXTremeJSPostfix,
			WebScriptResourcePath = WebResourceNames.WebScriptResourcePath;
		new const string WebCssResourcePath = WebResourceNames.WebCssResourcePath;
		internal const string
			CssResourcePathPrefix = WebCssResourcePath + "report-designer-",
			LightCssResourceName = CssResourcePathPrefix + ColorSchemeLight + DXTremeCssPostfix,
			LightCompactCssResourceName = CssResourcePathPrefix + ColorSchemeLightCompact + DXTremeCssPostfix,
			DarkCssResourceName = CssResourcePathPrefix + ColorSchemeDark + DXTremeCssPostfix,
			DarkCompactCssResourceName = CssResourcePathPrefix + ColorSchemeDarkCompact + DXTremeCssPostfix,
			ScriptResourceName = WebScriptResourcePath + "ReportDesigner.js",
			DXScriptResourceName = WebScriptResourcePath + "report-designer" + DXTremeJSPostfix;
		#endregion
		internal const string DefaultHandlerUri = "DXXRD.axd";
		const string
			ClientSideCategoryName = "Client-Side",
			ShouldDisposeReportName = "ShouldDisposeReport",
			ShouldDisposeDataSourcesName = "ShouldSharedDataSources";
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerCanProcessHandlerRequest")]
#endif
		public static event EventHandler<CanProcessHandlerRequestEventArgs> CanProcessHandlerRequest {
			add { ManagerSubscriber.CanProcessHandlerRequest += value; }
			remove { ManagerSubscriber.CanProcessHandlerRequest -= value; }
		}
		public static RequestEvent RequestEvent {
			get { return ManagerSubscriber.RequestEvent; }
			set { ManagerSubscriber.RequestEvent = value; }
		}
		public static bool ShouldClearReportScripts {
			get { return ReportLayoutJsonSerializer.ShouldClearScripts; }
			set { ReportLayoutJsonSerializer.ShouldClearScripts = value; }
		}
		readonly IReportDesignerModelGenerator reportDesignerModelGenerator;
		readonly IHtmlContentGenerator htmlContentGenerator;
		readonly IJSContentGenerator<ReportDesignerModel> jsContentGenerator;
		XtraReport report;
		string reportUrl = "";
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerClientInstanceName")]
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
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerWidth")]
#endif
		[DefaultValue(typeof(Unit), "100%")]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerHeight")]
#endif
		[DefaultValue(typeof(Unit), "850px")]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[MergableProperty(false)]
		[AutoFormatDisable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public ClientControlsMenuItemCollection<ClientControlsMenuItem> MenuItems { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerDataSources")]
#endif
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IDictionary<string, object> DataSources { get; private set; }
		private IDictionary<string, string> subreports { get; set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerSubreports")]
#endif
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Obsolete("Use ReportStorageWebExtention or ReportStorageWebService")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IDictionary<string, string> Subreports {
			get { return subreports; }
			private set { subreports = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerClientSideEvents")]
#endif
		[MergableProperty(false)]
		[Category(ClientSideCategoryName)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatDisable]
		public ReportDesignerClientSideEvents ClientSideEvents {
			get { return (ReportDesignerClientSideEvents)ClientSideEventsInternal; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerShouldDisposeReport")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(Constants.ShouldDisposeReportDefault)]
		[AutoFormatEnable]
		public bool ShouldDisposeReport {
			get { return GetBoolProperty(ShouldDisposeReportName, Constants.ShouldDisposeReportDefault); }
			set { SetBoolProperty(ShouldDisposeReportName, Constants.ShouldDisposeReportDefault, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerShouldDisposeDataSources")]
#endif
		[SRCategory(ReportStringId.CatBehavior)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[DefaultValue(Constants.ShouldDisposeDataSourcesDefault)]
		[AutoFormatEnable]
		public bool ShouldDisposeDataSources {
			get { return GetBoolProperty(ShouldDisposeDataSourcesName, Constants.ShouldDisposeDataSourcesDefault); }
			set { SetBoolProperty(ShouldDisposeDataSourcesName, Constants.ShouldDisposeDataSourcesDefault, value); }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShouldShareReportDataSources { get; set; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool TryAddDefaultDataSerializer { get; set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerJSProperties")]
#endif
		[Category("Client-Side")]
		[Browsable(false)]
		[AutoFormatDisable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		#region ctors
		static ASPxReportDesigner() {
			ASPxWebClientUIControl.StaticInitialize();
			ASPxWebDocumentViewer.StaticInitialize();
			ASPxQueryBuilder.StaticInitialize();
			if(ReportDesignerBootstrapper.SessionState == SessionStateBehavior.Disabled) {
				var managerSubscriber = CreateModuleSubscriber(DefaultWebDocumentViewerContainer.Current, DefaultQueryBuilderContainer.Current);
				ASPxHttpHandlerModule.Subscribe(managerSubscriber);
			} else {
				var managerSubscriber = CreateHandlerSubscriber(DefaultWebDocumentViewerContainer.Current, DefaultQueryBuilderContainer.Current);
				ASPxHttpHandlerModule.Subscribe(managerSubscriber);
			}
		}
		public ASPxReportDesigner()
			: this(DefaultReportDesignerContainer.Current) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxReportDesigner(IServiceProvider serviceProvider)
			: this(
			serviceProvider.GetService<IReportDesignerModelGenerator>(),
			serviceProvider.GetService<IReportDesignerHtmlContentGenerator>(),
			serviceProvider.GetService<IJSContentGenerator<ReportDesignerModel>>()) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxReportDesigner(
			IReportDesignerModelGenerator reportDesignerModelGenerator,
			IReportDesignerHtmlContentGenerator reportDesignerHtmlContentGenerator,
			IJSContentGenerator<ReportDesignerModel> jsContentGenerator
			) {
			this.reportDesignerModelGenerator = reportDesignerModelGenerator;
			this.htmlContentGenerator = reportDesignerHtmlContentGenerator;
			this.jsContentGenerator = jsContentGenerator;
			MenuItems = new ClientControlsMenuItemCollection<ClientControlsMenuItem>(this);
			Width = Constants.WidthDefault;
			Height = Constants.HeightDefault;
			ShouldShareReportDataSources = Constants.ShouldShareReportDataSourcesDefault;
			TryAddDefaultDataSerializer = Constants.TryAddDefaultDataSerializerDefault;
			var sharedDataSources = new ObservableDictionary<string, object>();
			sharedDataSources.CollectionChanged += LayoutChanged;
			DataSources = sharedDataSources;
			var subreports = new ObservableDictionary<string, string>();
			subreports.CollectionChanged += LayoutChanged;
			this.subreports = subreports;
		}
		#endregion
		#region events
		static readonly object SaveReportLayoutEvent = new object();
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerSaveReportLayout")]
#endif
		[Obsolete("Use ReportStorageWebExtention")]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event SaveReportLayoutEventHandler SaveReportLayout {
			add { Events.AddHandler(SaveReportLayoutEvent, value); }
			remove { Events.RemoveHandler(SaveReportLayoutEvent, value); }
		}
		void RaiseSaveReportLayout(byte[] reportLayout, string parameter) {
			var ev = Events[SaveReportLayoutEvent] as SaveReportLayoutEventHandler;
			if(ev != null) {
				var args = new SaveReportLayoutEventArgs(reportLayout, parameter);
				ev(this, args);
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxReportDesignerCustomJSProperties")]
#endif
		[Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		#endregion
		public new static void StaticInitialize() { }
		public void OpenReport(XtraReport report) {
			if(this.report == report) {
				return;
			}
			this.report = report;
			LayoutChanged();
		}
		public void OpenReport(string reportUrl) {
			if(this.reportUrl == reportUrl) {
				return;
			}
			this.reportUrl = reportUrl;
			OpenReportXmlLayout(ReportStorageService.GetData(reportUrl));
		}
		public void OpenReportXmlLayout(byte[] reportLayout) {
			using(var stream = new MemoryStream(reportLayout)) {
				OpenReport(XtraReport.FromStream(stream, true));
			}
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(!DesignMode) {
				string htmlContent = htmlContentGenerator.GetContent();
				Controls.Add(new LiteralControl(htmlContent));
			}
		}
		protected override void PrepareControlHierarchy() {
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientReportDesigner";
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(report != null) {
				var menuItemModels = MenuItems.Select(x => x.ToModel());
				var settings = new ReportDesignerModelSettings(shouldShareReportDataSources: ShouldShareReportDataSources, tryAddDefaultDataSerializer: TryAddDefaultDataSerializer);
				ReportDesignerModel reportDesignerModel = reportDesignerModelGenerator.Generate(report, DataSources, this.subreports, menuItemModels, settings);
				reportDesignerModel.ReportUrl = this.reportUrl;
				jsContentGenerator.Generate(stb, localVarName, reportDesignerModel);
			}
		}
		protected override void RegisterDefaultRenderCssFile() {
			if(DesignMode) {
				return;
			}
			RegisterDevExtremeCss(Page, UsefulColorScheme);
			RegisterJQueryUICss(Page);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxReportDesigner), ASPxReportDesigner.CssResourcePathPrefix + UsefulColorScheme + DXTremeCssPostfix);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxWebDocumentViewer), WebResourceNames.WebClientUIControl.CssResourceName);
		}
		protected override void RegisterDefaultSpriteCssFile() {
		}
		protected override void RegisterSystemCssFile() {
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterJQueryScript(Page);
			RegisterJQueryUIScript(Page);
			RegisterGlobalizeScript(Page);
			RegisterGlobalizeCulturesScript(Page);
			RegisterKnockoutScript(Page);
			RegisterDevExtremeBaseScript(Page);
			RegisterDevExtremeWebWidgetsScript(Page);
			RegisterAceScript(Page);
			RegisterIncludeScript(typeof(ASPxReportDesigner), WebResourceNames.WebClientUIControl.ScriptResourceName);
			RegisterIncludeScript(typeof(ASPxReportDesigner), ScriptResourceName);
			if(WebClientUIControlHelper.IsSupportedBrowser) {
				RegisterIncludeScript(typeof(ASPxReportDesigner), WebResourceNames.WebClientUIControl.DXScriptResourceName);
				RegisterIncludeScript(typeof(ASPxReportDesigner), DXScriptResourceName);
			}
		}
		void RegisterAceScript(Page Page) {
			if(!ConfigurationSettings.EmbedRequiredClientLibraries && !GlobalEmbedRequiredClientLibraries) {
				return;
			}
			RegisterIncludeScript(typeof(ASPxReportDesigner), WebResourceNames.Frameworks.AceScriptResourceName);
			RegisterIncludeScript(typeof(ASPxReportDesigner), WebResourceNames.Frameworks.AceExtLanguageToolsScriptResourceName);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new ReportDesignerClientSideEvents();
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { MenuItems });
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			var input = ReportDesignerInputLoader.FromString(eventArgument);
			RaiseSaveReportLayout(input.ReportLayout, input.Parameter);
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
		public override void Dispose() {
			base.Dispose();
			if(ShouldDisposeReport && report != null && !report.IsDisposed) {
				report.Dispose();
			}
			if(ShouldDisposeDataSources) {
				foreach(var disposable in DataSources.Values.OfType<IDisposable>()) {
					disposable.Dispose();
				}
			}
		}
		static IServiceProvider InitContainer(IServiceProvider webDocumentPreviewServiceProvider, IServiceProvider queryBuilderServiceProvider) {
			var serviceProvider = DefaultReportDesignerContainer.Current;
			if(serviceProvider == null) {
				serviceProvider = ReportDesignerBootstrapper.CreateInitializedServiceIntegrityContainer(
					() => webDocumentPreviewServiceProvider.GetService<IReportManagementService>(),
					() => queryBuilderServiceProvider.GetService<ISqlDataSourceWizardService>()
					);
				DefaultReportDesignerContainer.Current = serviceProvider;
			}
			return serviceProvider;
		}
		static IHttpModuleSubscriber CreateModuleSubscriber(IServiceProvider webDocumentPreviewServiceProvider, IServiceProvider queryBuilderServiceProvider) {
			var serviceProvider = InitContainer(webDocumentPreviewServiceProvider, queryBuilderServiceProvider);
			return new ManagerModuleSubscriber<IReportDesignerRequestManager>(serviceProvider, DefaultHandlerUri);
		}
		static IHttpHandlerSubscriber CreateHandlerSubscriber(IServiceProvider webDocumentPreviewServiceProvider, IServiceProvider queryBuilderServiceProvider) {
			var serviceProvider = InitContainer(webDocumentPreviewServiceProvider, queryBuilderServiceProvider);
			return new ManagerHandlerSubscriber<IReportDesignerRequestManager>(serviceProvider, DefaultHandlerUri, ReportDesignerBootstrapper.SessionState);
		}
		string IControlDesigner.DesignerType {
			get { return "DevExpress.Web.Design.Reports.ReportDesigner.ReportDesignerMenuItemsOwner"; }
		}
	}
}
