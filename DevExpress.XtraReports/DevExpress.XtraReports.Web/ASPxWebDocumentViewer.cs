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
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Localization;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native;
using DevExpress.XtraReports.Web.WebDocumentViewer.Native.Services;
using Constants = DevExpress.XtraReports.Web.Native.Constants.WebDocumentViewer;
namespace DevExpress.XtraReports.Web {
	[ToolboxItem(false)]
	[Designer("DevExpress.XtraReports.Web.Design.ASPxWebDocumentViewerDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ASPxDocumentViewer.bmp")]
	[ToolboxTabName(AssemblyInfo.DXTabReporting)]
	public class ASPxWebDocumentViewer : ASPxWebClientUIControl, IControlDesigner {
		#region resources
		new const string WebCssResourcePath = WebResourceNames.WebCssResourcePath;
		const string
			WebScriptResourcePath = WebResourceNames.WebScriptResourcePath,
			DXTremeCssPostfix = WebResourceNames.DXTremeCssPostfix,
			DXTremeJSPostfix = WebResourceNames.DXTremeJSPostfix;
		internal const string
			CssResourcePathPrefix = WebCssResourcePath + "web-document-viewer-",
			LightCssResourceName = CssResourcePathPrefix + ColorSchemeLight + DXTremeCssPostfix,
			LightCompactCssResourceName = CssResourcePathPrefix + ColorSchemeLightCompact + DXTremeCssPostfix,
			DarkCssResourceName = CssResourcePathPrefix + ColorSchemeDark + DXTremeCssPostfix,
			DarkCompactCssResourceName = CssResourcePathPrefix + ColorSchemeDarkCompact + DXTremeCssPostfix,
			ScriptResourceName = WebScriptResourcePath + "WebDocumentViewer.js",
			DXScriptResourceName = WebScriptResourcePath + "web-document-viewer" + DXTremeJSPostfix;
		#endregion
		internal const string DefaultHandlerUri = "DXXRDV.axd";
		const string
			ClientSideCategoryName = "Client-Side",
			ReportCategoryName = "Report",
			ReportSourceKindName = "ReportSourceKind",
			ReportSourceIdName = "ReportSourceId";
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxWebDocumentViewerCanProcessHandlerRequest")]
#endif
		public static event EventHandler<CanProcessHandlerRequestEventArgs> CanProcessHandlerRequest {
			add { ManagerSubscriber.CanProcessHandlerRequest += value; }
			remove { ManagerSubscriber.CanProcessHandlerRequest -= value; }
		}
		public static RequestEvent RequestEvent {
			get { return ManagerSubscriber.RequestEvent; }
			set { ManagerSubscriber.RequestEvent = value; }
		}
		readonly IWebDocumentViewerModelGenerator webDocumentViewerModelGenerator;
		readonly IJSContentGenerator<WebDocumentViewerModel> jsContentGenerator;
		readonly IHtmlContentGenerator htmlContentGenerator;
		XtraReport report;
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[MergableProperty(false)]
		[AutoFormatDisable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ClientControlsMenuItemCollection<WebDocumentViewerMenuItem> MenuItems { get; private set; }
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxWebDocumentViewerWidth")]
#endif
		[DefaultValue(typeof(Unit), "100%")]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxWebDocumentViewerHeight")]
#endif
		[DefaultValue(typeof(Unit), "1100px")]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxWebDocumentViewerClientSideEvents")]
#endif
		[MergableProperty(false)]
		[Category(ClientSideCategoryName)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatDisable]
		public WebDocumentViewerClientSideEvents ClientSideEvents {
			get { return (WebDocumentViewerClientSideEvents)ClientSideEventsInternal; }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxWebDocumentViewerClientInstanceName")]
#endif
		[AutoFormatDisable]
		[Category(ClientSideCategoryName)]
		[DefaultValue("")]
		[Localizable(false)]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
		[AutoFormatDisable]
		[Category(ReportCategoryName)]
		[DefaultValue(Constants.ReportSourceKindDefault)]
		[Localizable(false)]
		public ReportSourceKind ReportSourceKind {
			get { return (ReportSourceKind)GetEnumProperty(ReportSourceKindName, Constants.ReportSourceKindDefault); }
			set {
				SetEnumProperty(ReportSourceKindName, Constants.ReportSourceKindDefault, value);
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxWebDocumentViewerReportSourceId")]
#endif
		[AutoFormatDisable]
		[Category(ReportCategoryName)]
		[TypeConverter("DevExpress.Web.Design.Reports.Converters.ReportTypeNameConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		[DefaultValue("")]
		[Localizable(false)]
		public string ReportSourceId {
			get { return (string)GetStringProperty(ReportSourceIdName, ""); }
			set {
				SetEnumProperty(ReportSourceIdName, "", value);
				LayoutChanged();
			}
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxWebDocumentViewerJSProperties")]
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
		static ASPxWebDocumentViewer() {
			if(WebDocumentViewerBootstrapper.SessionState == System.Web.SessionState.SessionStateBehavior.Disabled) {
				var subscriber = CreateModuleSubscriber();
				ASPxHttpHandlerModule.Subscribe(subscriber);
			} else {
				var subscriber = CreateHandlerSubscriber();
				ASPxHttpHandlerModule.Subscribe(subscriber);
			}
		}
		public ASPxWebDocumentViewer()
			: this(DefaultWebDocumentViewerContainer.Current) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxWebDocumentViewer(IServiceProvider serviceProvider)
			: this(
			serviceProvider.GetService<IWebDocumentViewerModelGenerator>(),
			serviceProvider.GetService<IWebDocumentViewerHtmlContentGenerator>(),
			serviceProvider.GetService<IJSContentGenerator<WebDocumentViewerModel>>(),
			serviceProvider.GetService<IStoragesCleaner>()) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxWebDocumentViewer(
			IWebDocumentViewerModelGenerator webDocumentViewerModelGenerator,
			IWebDocumentViewerHtmlContentGenerator webDocumentViewerHtmlContentGenerator,
			IJSContentGenerator<WebDocumentViewerModel> jsContentGenerator,
			IStoragesCleaner cleaner) {
			this.webDocumentViewerModelGenerator = webDocumentViewerModelGenerator;
			this.jsContentGenerator = jsContentGenerator;
			this.htmlContentGenerator = webDocumentViewerHtmlContentGenerator;
			MenuItems = new ClientControlsMenuItemCollection<WebDocumentViewerMenuItem>(this);
			Width = Constants.DefaultWidth;
			Height = Constants.DefaultHeight;
			cleaner.SafeStart();
		}
		#endregion
		public new static void StaticInitialize() { }
		#region events
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("ASPxWebDocumentViewerCustomJSProperties")]
#endif
		[Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		#endregion
		public void OpenReport(XtraReport report) {
			if(this.report == report) {
				return;
			}
			this.report = report;
			LayoutChanged();
		}
		public void OpenReportXmlLayout(byte[] reportXmlLayout) {
			using(var stream = new MemoryStream(reportXmlLayout)) {
				OpenReport(XtraReport.FromStream(stream, true));
			}
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			WebDocumentViewerModel webDocumentViewerModel = GetModel();
			if(webDocumentViewerModel != null) {
				jsContentGenerator.Generate(stb, localVarName, webDocumentViewerModel);
			}
		}
		protected virtual WebDocumentViewerModel GetModel() {
			if(report != null && !string.IsNullOrEmpty(ReportSourceId)) {
				throw new InvalidOperationException(ASPxReportsLocalizer.GetString(ASPxReportsStringId.WebDocumentViewer_OpenReport_Error));
			}
			var menuItemModels = MenuItems.Select(x => x.ToModel());
			if(report != null) {
				var reportModel = new ReportModelInfo(report);
				var model = webDocumentViewerModelGenerator.Generate(reportModel, menuItemModels);
				model.ReportInfo.PageHeight = report.PageHeight;
				model.ReportInfo.PageWidth = report.PageWidth;
				return model;
			}
			if(ReportSourceKind == ReportSourceKind.ReportType && !string.IsNullOrEmpty(ReportSourceId)) {
				var reportResolver = new ReportResolver();
				var resolvedReport = reportResolver.Resolve(ReportSourceId, Page);
				var resolvedReportModel = new ReportModelInfo(resolvedReport);
				var model = webDocumentViewerModelGenerator.Generate(resolvedReportModel, menuItemModels);
				model.ReportInfo.PageHeight = resolvedReport.PageHeight;
				model.ReportInfo.PageWidth = resolvedReport.PageWidth;
				return model;
			}
			return null;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new WebDocumentViewerClientSideEvents();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientWebDocumentViewer";
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterDefaultRenderCssFile() {
			if(DesignMode) {
				return;
			}
			RegisterDevExtremeCss(Page, UsefulColorScheme);
			RegisterJQueryUICss(Page);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxWebDocumentViewer), CssResourcePathPrefix + UsefulColorScheme + DXTremeCssPostfix);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxWebDocumentViewer), WebResourceNames.WebClientUIControl.CssResourceName);
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
			RegisterIncludeScript(typeof(ASPxWebDocumentViewer), WebResourceNames.WebClientUIControl.ScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebDocumentViewer), ScriptResourceName);
			if(WebClientUIControlHelper.IsSupportedBrowser) {
				RegisterIncludeScript(typeof(ASPxWebDocumentViewer), WebResourceNames.WebClientUIControl.DXScriptResourceName);
				RegisterIncludeScript(typeof(ASPxWebDocumentViewer), DXScriptResourceName);
			}
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
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { MenuItems });
		}
		string IControlDesigner.DesignerType {
			get { return "DevExpress.Web.Design.Reports.WebDocumentViewer.WebDocumentViewerMenuItemsOwner"; }
		}
		static IServiceProvider InitContainer() {
			IServiceProvider serviceProvider = DefaultWebDocumentViewerContainer.Current;
			if(serviceProvider == null) {
				serviceProvider = WebDocumentViewerBootstrapper.CreateInitializedContainer();
				DefaultWebDocumentViewerContainer.Current = serviceProvider;
			}
			return serviceProvider;
		}
		static IHttpModuleSubscriber CreateModuleSubscriber() {
			var serviceProvider = InitContainer();
			return new ManagerModuleSubscriber<IWebDocumentViewerRequestManager>(serviceProvider, DefaultHandlerUri);
		}
		static IHttpHandlerSubscriber CreateHandlerSubscriber() {
			var serviceProvider = InitContainer();
			return new ManagerHandlerSubscriber<IWebDocumentViewerRequestManager>(serviceProvider, DefaultHandlerUri, WebDocumentViewerBootstrapper.SessionState);
		}
	}
}
