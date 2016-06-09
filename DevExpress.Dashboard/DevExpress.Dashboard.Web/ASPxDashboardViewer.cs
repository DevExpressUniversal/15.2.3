#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardWeb.Localization;
using DevExpress.DashboardWeb.Native;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.DashboardWeb {
	public enum DashboardTheme {
		Light,
		Dark
	}
	[
#if !DEBUG
#endif
	ToolboxBitmap(typeof(ResFinder), "Bitmaps256.ASPxDashboardViewer.bmp"),
	Designer("DevExpress.DashboardWeb.Design.ASPxDashboardViewerDesigner, " + AssemblyInfo.SRAssemblyDashboardWebDesign),
	ToolboxTabName(AssemblyInfo.DXTabData),
	DXWebToolboxItem(true),
	DXClientDocumentationProvider("#Dashboard/DevExpressDashboardWebScripts")
	]
	public class ASPxDashboardViewer : ASPxWebClientUIControl, IRequiresLoadPostDataControl, IOfficeCoreReference, IJSONCustomObject {
		const DashboardTheme DefaultDashboardTheme = DashboardTheme.Light;
		const double DefaultSessionTimeout = 5 * 60;
		const bool DefaultRedrawOnResize = true;
		const bool DefaultAllowExportDashboard = true;
		const bool DefaultAllowExportDashboardItems = false;
		const bool DefaultRegisterJQuery = false;
		const bool DefaultUseDataAccessApi = true;
		const bool DefaultCalculateHiddenTotals = false;
		const bool DefaultHandleServerErrors = false;
		const string PropertyDashboardId = "DashboardId";
		const string PropertyRegisterJQuery = "RegisterJQuery";
		const string PropertyRedrawOnResize = "RedrawOnResize";
		const string PropertyAllowExportDashboard = "AllowExportDashboard";
		const string PropertyAllowExportDashboardItems = "AllowExportDashboardItems";
		const string PropertyCalculateHiddenTotals = "CalculateHiddenTotals";
		const string PropertyHandleServerErrors = "HandleServerErrors";
		const string PropertySessionTimeout = "SessionTimeout";
		const string CategoryData = "Data";
		const string CategoryClientSide = "Client-Side";
		const string CategoryBehavior = "Behavior";
		const string CategoryAppearance = "Appearance";
		internal const string DashboardViewerImagesPath = "DevExpress.DashboardWeb.Images.";
		internal const string DevExtremeScriptsResourcePath = "DevExpress.DashboardWeb.ClientControls.Scripts.";
		internal const string DevExtremeDashboardScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-dashboard"
#if DEBUG
			+ ".debug"
#endif
			+ ".js";
		internal const string JQueryCheckerResourceName = DevExtremeScriptsResourcePath + "jquerychecker.js";		
		internal const string DashboardViewerClientDataAPIScriptResourceName = "DevExpress.DashboardWeb.Scripts.ClientDataAPI.js";
		internal const string DashboardViewerScriptResourceName = "DevExpress.DashboardWeb.Scripts.DashboardViewer.js";
		internal const string DevExtremeCssResourcePath = "DevExpress.DashboardWeb.Css.";
		internal const string DashboardViewerCommonCssResourceName = DevExtremeCssResourcePath + "dx.dashboard.common.css";
		internal const string DashboardViewerLightCssResourceName = DevExtremeCssResourcePath + "dx.dashboard."+ ColorSchemeLight + ".css";
		internal const string DashboardViewerDarkCssResourceName = DevExtremeCssResourcePath + "dx.dashboard." + ColorSchemeDark + ".css";
		const string DashboardViewerThemeCssResourceName = DevExtremeCssResourcePath + "dx.dashboard.{0}.css";
		const string UnknownFileFormat = "unknown";
		readonly static object singleFilterDefaultValue = new object();
		readonly static object filterElementDefaultValues = new object();
		readonly static object rangeFilterDefaultValue = new object();
		readonly static object dashboardLoading = new object();
		readonly static object dataLoading = new object();
		readonly static object configureDataConnection = new object();
		readonly static object customFilterExpression = new object();
		readonly static object customParameters = new object();
		readonly static object dashboardLoaded = new object();
		readonly static object validateCustomSqlQuery = new object();
		static Unit DefaultWidth = new Unit(800);
		static Unit DefaultHeight = new Unit(600);
		static Unit FullScreenMeasure = Unit.Percentage(100);
		readonly ASPxDashboardService service;
		readonly DashboardExportOptions exportOptions = new DashboardExportOptions();
		DashboardServiceResult dashboardServiceResult;
		bool fullscreenMode;
		List<JSONPropertyInfo> jSonPropertyInfo;
		int IJSONCustomObject.PropertiesCount { get { return JSonPropertyInfo.Count; } }
		List<JSONPropertyInfo> JSonPropertyInfo {
			get {
				if(jSonPropertyInfo == null) {
					jSonPropertyInfo = new List<JSONPropertyInfo>();
					jSonPropertyInfo.Add(new JSONPropertyInfo("dashboardId", () => ActualDashboardId));
					jSonPropertyInfo.Add(new JSONPropertyInfo("fullScreen", () => fullscreenMode));
					jSonPropertyInfo.Add(new JSONPropertyInfo("redrawOnResize", () => RedrawOnResize));
					jSonPropertyInfo.Add(new JSONPropertyInfo("allowExport", () => AllowExportDashboard));
					jSonPropertyInfo.Add(new JSONPropertyInfo("allowItemsExport", () => AllowExportDashboardItems));
					jSonPropertyInfo.Add(new JSONPropertyInfo("encodeHtml", () => EncodeHtml));
					jSonPropertyInfo.Add(new JSONPropertyInfo("exportOptions", () => exportOptions));
					jSonPropertyInfo.Add(new JSONPropertyInfo("calculateHiddenTotals", () => CalculateHiddenTotals));
				}
				return jSonPropertyInfo;
			}
		}
		IDashboardService DashboardService { get { return (IDashboardService)service; } }
		string ActualDashboardId {
			get {
				if(!string.IsNullOrEmpty(DashboardId))
					return DashboardId;
				object source = DashboardSource;
				string str = source as string;
				if(!string.IsNullOrEmpty(str))
					return GetResolvedXmlPath(str);
				Type type = source as Type;
				if(type != null)
					return type.AssemblyQualifiedName;
				return null;
			}
		}
		protected DashboardServiceResult DashboardServiceResult { get { return dashboardServiceResult; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use DashboardSource property instead.", false)
		]
		public string DashboardXmlFile {
			get { return DashboardSource as string; }
			set { DashboardSource = value; }
		}
		bool ShouldSerializeDashboardXmlFile() {
			return false;
		}
		[
		Category(CategoryBehavior),
		Editor("DevExpress.DashboardWin.Design.DashboardSourceUrlEditor," + AssemblyInfo.SRAssemblyDashboardWinDesign, typeof(UITypeEditor)),
		TypeConverter(typeof(DashboardSourceTypeConverter)),
		DefaultValue("")
		]
		public object DashboardSource {
			get {
				return DashboardSourceTypeConverter.DashboardSourceFromString(IsMvcRender() ? null : this, GetStringProperty("DashboardSource", null), null);
			}
			set {
				SetStringProperty("DashboardSource", "", DashboardSourceTypeConverter.DashboardSourceToString(value));
				ResetControlHierarchy();
			}
		}
		[
		Category(CategoryBehavior),
		DefaultValue("")
		]
		public string DashboardId {
			get { return GetStringProperty(PropertyDashboardId, string.Empty); }
			set {
				if(value != DashboardId) {
					SetStringProperty(PropertyDashboardId, string.Empty, value);
					ResetControlHierarchy();
				}
			}
		}
		[
		Category(CategoryBehavior),
		DefaultValue(false)
		]
		public bool FullscreenMode {
			get { return fullscreenMode; }
			set {
				SetFullScreenMode(value, true);
			}
		}
		public override Unit Width {
			get { return base.Width; }
			set { SetWidth(value, true); }
		}
		public override Unit Height {
			get { return base.Height; }
			set { SetHeight(value, true); }
		}
		[
		Category(CategoryBehavior),
		DefaultValue(DefaultRedrawOnResize)
		]
		public bool RedrawOnResize {
			get { return GetBoolProperty(PropertyRedrawOnResize, DefaultRedrawOnResize); }
			set {
				if(value != RedrawOnResize)
					SetBoolProperty(PropertyRedrawOnResize, DefaultRedrawOnResize, value);
			}
		}
		[
		Category(CategoryBehavior),
		DefaultValue(DefaultAllowExportDashboard)
		]
		public bool AllowExportDashboard {
			get { return GetBoolProperty(PropertyAllowExportDashboard, DefaultAllowExportDashboard); }
			set {
				if(value != AllowExportDashboard)
					SetBoolProperty(PropertyAllowExportDashboard, DefaultAllowExportDashboard, value);
			}
		}
		[
		Category(CategoryBehavior),
		DefaultValue(DefaultAllowExportDashboardItems)
		]
		public bool AllowExportDashboardItems {
			get { return GetBoolProperty(PropertyAllowExportDashboardItems, DefaultAllowExportDashboardItems); }
			set {
				if(value != AllowExportDashboardItems)
					SetBoolProperty(PropertyAllowExportDashboardItems, DefaultAllowExportDashboardItems, value);
			}
		}
		[
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		Category(CategoryBehavior)
		]
		public DashboardExportOptions ExportOptions { get { return exportOptions; } }
		[
		Category(CategoryClientSide),
		DefaultValue(""),
		AutoFormatDisable
		]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
		Category(CategoryClientSide),
		DefaultValue(DefaultRegisterJQuery),
		Obsolete("Instead, use the embedRequiredClientLibraries attribute contained within the devExpress section's settings element in the Web.config file."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool RegisterJQuery {
			get { return RegisterJQueryInternal; }
			set { RegisterJQueryInternal = value; }
		}
		protected bool RegisterJQueryInternal {
			get { return GetBoolProperty(PropertyRegisterJQuery, DefaultRegisterJQuery); }
			set {
				if(value != RegisterJQueryInternal)
					SetBoolProperty(PropertyRegisterJQuery, DefaultRegisterJQuery, value);
			}
		}
		[
		Category(CategoryAppearance),
		Obsolete("This property is now obsolete. Use the ASPxWebClientUIControl.ColorScheme property instead.", false),
		DefaultValue(DefaultDashboardTheme)
		]
		public DashboardTheme DashboardTheme {
			get { return (ColorScheme == ColorSchemeDark) ? DashboardTheme.Dark : DashboardTheme.Light; }
			set { ColorScheme = (value == DashboardTheme.Dark) ? ColorSchemeDark : ColorSchemeLight; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public override string Theme { get { return base.Theme; } set { base.Theme = value; } }
		[
		Category(CategoryBehavior),
		DefaultValue(DefaultSessionTimeout)
		]
		public double SessionTimeout {
			get { return (double)GetDecimalProperty(PropertySessionTimeout, (decimal)DefaultSessionTimeout); }
			set { SetDecimalProperty(PropertySessionTimeout, (decimal)DefaultSessionTimeout, (decimal)value); }
		}
		[
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)
		]
		public DashboardClientSideEvents ClientSideEvents {
			get { return (DashboardClientSideEvents)base.ClientSideEventsInternal; }
		}
		#region CustomJSProperties
		[Category(CategoryClientSide), Browsable(false), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]		
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		[Category(CategoryClientSide)]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		#endregion
		[
		Category(CategoryClientSide),
		DefaultValue(DefaultUseDataAccessApi),
		Obsolete("This property is now obsolete. You no longer need to set it to true in order to use data access API."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool UseDataAccessApi {
			get { return true; }
			set { }
		}
		[
		Category(CategoryClientSide),
		DefaultValue(DefaultCalculateHiddenTotals)
		]
		public bool CalculateHiddenTotals {
			get { return GetBoolProperty(PropertyCalculateHiddenTotals, DefaultCalculateHiddenTotals); }
			set { SetBoolProperty(PropertyCalculateHiddenTotals, DefaultCalculateHiddenTotals, value); }
		}
		[
		Category(CategoryBehavior),
		DefaultValue(DefaultHandleServerErrors)
		]
		public bool HandleServerErrors {
			get { return GetBoolProperty(PropertyHandleServerErrors, DefaultHandleServerErrors); }
			set { SetBoolProperty(PropertyHandleServerErrors, DefaultHandleServerErrors, value); }
		}
		public event SingleFilterDefaultValueEventHandler SingleFilterDefaultValue {
			add { Events.AddHandler(singleFilterDefaultValue, value); }
			remove { Events.RemoveHandler(singleFilterDefaultValue, value); }
		}
		public event FilterElementDefaultValuesEventHandler FilterElementDefaultValues {
			add { Events.AddHandler(filterElementDefaultValues, value); }
			remove { Events.RemoveHandler(filterElementDefaultValues, value); }
		}
		public event RangeFilterDefaultValueEventHandler RangeFilterDefaultValue {
			add { Events.AddHandler(rangeFilterDefaultValue, value); }
			remove { Events.RemoveHandler(rangeFilterDefaultValue, value); }
		}
		public event DashboardLoadingEventHandler DashboardLoading {
			add { Events.AddHandler(dashboardLoading, value); }
			remove { Events.RemoveHandler(dashboardLoading, value); }
		}
		public event DashboardLoadedWebEventHandler DashboardLoaded {
			add { Events.AddHandler(dashboardLoaded, value); }
			remove { Events.RemoveHandler(dashboardLoaded, value); }
		}
		public event ValidateDashboardCustomSqlQueryWebEventHandler ValidateCustomSqlQuery {
			add { Events.AddHandler(validateCustomSqlQuery, value); }
			remove { Events.RemoveHandler(validateCustomSqlQuery, value); }
		}
		[Category(CategoryData)]
		public event DataLoadingWebEventHandler DataLoading {
			add { Events.AddHandler(dataLoading, value); }
			remove { Events.RemoveHandler(dataLoading, value); }
		}
		[Category(CategoryData)]
		public event ConfigureDataConnectionWebEventHandler ConfigureDataConnection {
			add { Events.AddHandler(configureDataConnection, value); }
			remove { Events.RemoveHandler(configureDataConnection, value); }
		}
		[Category(CategoryData)]
		public event CustomFilterExpressionWebEventHandler CustomFilterExpression {
			add { Events.AddHandler(customFilterExpression, value); }
			remove { Events.RemoveHandler(customFilterExpression, value); }
		}
		[Category(CategoryData)]
		public event CustomParametersWebEventHandler CustomParameters {
			add { Events.AddHandler(customParameters, value); }
			remove { Events.RemoveHandler(customParameters, value); }
		}
		public ASPxDashboardViewer()
			: this(true) {
		}
		protected ASPxDashboardViewer(bool isServiceRequired) {
			Width = DefaultWidth;
			Height = DefaultHeight;
			if(isServiceRequired) {
				service = new ASPxDashboardService();
				service.SingleFilterDefaultValue += OnSingleFilterDefaultValue;
				service.FilterElementDefaultValues += OnFilterElementDefaultValues;
				service.RangeFilterDefaultValue += OnRangeFilterDefaultValue;
				service.DashboardLoadingEvent += OnDashboardLoading;
				service.DataLoadingEvent += OnDataLoading;
				service.ConfigureDataConnectionEvent += OnConfigureDataConnection;
				service.CustomFilterExpressionEvent += OnCustomFilterExpression;
				service.CustomParametersEvent += OnCustomParameters;
				service.DashboardLoadedEvent += OnDashboardLoaded;
				service.ValidateCustomSqlQuery += OnValidateCustomSqlQuery;
			}
		}
		public override void Dispose() {
			if(service != null) {
				service.SingleFilterDefaultValue -= OnSingleFilterDefaultValue;
				service.FilterElementDefaultValues -= OnFilterElementDefaultValues;
				service.RangeFilterDefaultValue -= OnRangeFilterDefaultValue;
				service.DashboardLoadingEvent -= OnDashboardLoading;
				service.DataLoadingEvent -= OnDataLoading;
				service.ConfigureDataConnectionEvent -= OnConfigureDataConnection;
				service.CustomFilterExpressionEvent -= OnCustomFilterExpression;
				service.CustomParametersEvent -= OnCustomParameters;
				service.DashboardLoadedEvent -= OnDashboardLoaded;
				service.ValidateCustomSqlQuery -= OnValidateCustomSqlQuery;
				service.Dispose();
			}
			base.Dispose();
		}
		string IJSONCustomObject.GetPropertyName(int index) {
			return JSonPropertyInfo[index].Name;
		}
		object IJSONCustomObject.GetPropertyValue(int index) {
			return JSonPropertyInfo[index].Getter();
		}
		bool CheckDashboardId(string dashboardId) {
			return !string.IsNullOrEmpty(dashboardId) && dashboardId == ActualDashboardId;
		}
		void SetFullScreenMode(bool value, bool refreshSize) {
			if(value != fullscreenMode) {
				fullscreenMode = value;
				PropertyChanged("FullscreenMode");
				if(refreshSize) {
					SetWidth(FullscreenMode ? FullScreenMeasure : DefaultWidth, false);
					SetHeight(FullscreenMode ? FullScreenMeasure : DefaultHeight, false);
					ResetControlHierarchy();
				}
			}
		}
		void SetWidth(Unit value, bool resetFullScreenMode) {
			if(value != base.Width) {
				base.Width = value;
				PropertyChanged("Width");
				if(resetFullScreenMode)
					SetFullScreenMode(false, false);
			}
		}
		void SetHeight(Unit value, bool resetFullScreenMode) {
			if(value != base.Height) {
				base.Height = value;
				PropertyChanged("Height");
				if(resetFullScreenMode)
					SetFullScreenMode(false, false);
			}
		}
		void OnSingleFilterDefaultValue(object sender, SingleFilterDefaultValueEventArgs e) {
			SingleFilterDefaultValueEventHandler handler = (SingleFilterDefaultValueEventHandler)Events[singleFilterDefaultValue];
			if(handler != null)
				handler(sender, e);
		}
		void OnFilterElementDefaultValues(object sender, FilterElementDefaultValuesEventArgs e) {
			FilterElementDefaultValuesEventHandler handler = (FilterElementDefaultValuesEventHandler)Events[filterElementDefaultValues];
			if(handler != null)
				handler(this, e);
		}
		void OnRangeFilterDefaultValue(object sender, RangeFilterDefaultValueEventArgs e) {
			RangeFilterDefaultValueEventHandler handler = (RangeFilterDefaultValueEventHandler)Events[rangeFilterDefaultValue];
			if (handler != null)
				handler(this, e);
		}
		void OnValidateCustomSqlQuery(object sender, ValidateDashboardCustomSqlQueryEventArgs e) {
			if (!CheckDashboardId(e.DashboardId))
				return;
			ValidateDashboardCustomSqlQueryWebEventHandler handler = (ValidateDashboardCustomSqlQueryWebEventHandler)Events[validateCustomSqlQuery];
			if (handler != null)
				handler(sender, new ValidateDashboardCustomSqlQueryWebEventArgs(e));
		}
		void OnDashboardLoading(object sender, DashboardLoadingServerEventArgs e) {
			if(!CheckDashboardId(e.DashboardId))
				return;
			if(e.DashboardId == DashboardId) {
				DashboardLoadingEventHandler handler = (DashboardLoadingEventHandler)Events[dashboardLoading];
				if(handler != null) {
					DashboardLoadingEventArgs args = new DashboardLoadingEventArgs(e.DashboardId);
					handler(this, args);
					e.DashboardXml = args.DashboardXml;
					e.DashboardType = args.DashboardType;
				}
			} else {
				object source = DashboardSource;
				e.DashboardType = source as Type;
				e.DashboardXml = LoadDashboardFromFile(GetResolvedXmlPath(source as string));
			}
		}
		void OnDataLoading(object sender, DataLoadingServerEventArgs e) {
			if(!CheckDashboardId(e.DashboardId))
				return;
			DataLoadingWebEventHandler handler = (DataLoadingWebEventHandler)Events[dataLoading];
			if(handler != null) {
				DataLoadingWebEventArgs args = new DataLoadingWebEventArgs(e.DashboardId, e.DataSourceComponentName, e.DataSourceName) {
					Data = e.Data
				};
				handler(this, args);
				e.Data = args.Data;
			}
		}
		void OnConfigureDataConnection(object sender, ConfigureDataConnectionServerEventArgs e) {
			if(!CheckDashboardId(e.DashboardId))
				return;
			ConfigureDataConnectionWebEventHandler handler = (ConfigureDataConnectionWebEventHandler)Events[configureDataConnection];
			if(handler != null) {
				ConfigureDataConnectionWebEventArgs args = new ConfigureDataConnectionWebEventArgs(e.DashboardId, e.ConnectionName, e.DataSourceName,  e.ConnectionParameters);
				handler(this, args);
				e.ConnectionParameters = args.ConnectionParameters;
			}
		}
		void OnCustomFilterExpression(object sender, CustomFilterExpressionServerEventArgs e) {
			if(!CheckDashboardId(e.DashboardId))
				return;
			CustomFilterExpressionWebEventHandler handler = (CustomFilterExpressionWebEventHandler)Events[customFilterExpression];
			if(handler != null) {
				CustomFilterExpressionWebEventArgs args = new CustomFilterExpressionWebEventArgs(e.DashboardId, e.DataSourceComponentName, e.DataSourceName, e.TableName) {
					FilterExpression = e.FilterExpression
				};
				handler(this, args);
				e.FilterExpression = args.FilterExpression;
			}
		}
		void OnCustomParameters(object sender, CustomParametersServerEventArgs e) {
			if(!CheckDashboardId(e.DashboardId))
				return;
			CustomParametersWebEventHandler handler = (CustomParametersWebEventHandler)Events[customParameters];
			if(handler != null) {
				CustomParametersWebEventArgs args = new CustomParametersWebEventArgs(e.DashboardId, e.Parameters);
				handler(this, args);
				e.Parameters = args.Parameters;
			}
		}
		void OnDashboardLoaded(object sender, DashboardLoadedServerEventArgs e) {
			DashboardLoadedWebEventHandler handler = (DashboardLoadedWebEventHandler)Events[dashboardLoaded];
			if(handler != null) {
				DashboardLoadedWebEventArgs args = new DashboardLoadedWebEventArgs(e.DashboardId, e.Dashboard);
				handler(this, args);
			}
		}
		protected virtual string LoadDashboardFromFile(string filePath) {
			if(File.Exists(filePath)) {
				try {
					using(Stream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {
						using(StreamReader reader = new StreamReader(fileStream)) {
							fileStream.Seek(0, SeekOrigin.Begin);
							return reader.ReadToEnd();
						}
					}
				} catch {
				}
			}
			return null;
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		protected override bool HasHoverScripts() {
			return true;
		}
		protected override bool HasSelectedScripts() {
			return true;
		}
		protected override bool HasContent() {
			return true;
		}
		protected override bool HasSpriteCssFile() {
			return false;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientDashboardViewer";
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new DashboardClientSideEvents();
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.AppendLine(string.Format("{0}.initOptions = {1};\n", localVarName, HtmlConvertor.ToJSON(this)));
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterJQueryScript(Page, RegisterJQueryInternal);
			RegisterIncludeScript(typeof(ASPxDashboardViewer), JQueryCheckerResourceName);
			RegisterGlobalizeScript(Page, RegisterJQueryInternal);
			RegisterGlobalizeCulturesScript(Page, RegisterJQueryInternal);
			RegisterDevExtremeBaseScript(Page);
			RegisterDevExtremeWebWidgetsScript(Page);
			RegisterDevExtremeVizWidgetsScript(Page);
			RegisterIncludeScript(typeof(ASPxDashboardViewer), DevExtremeDashboardScriptResourceName);
			RegisterIncludeScript(typeof(ASPxWebControl), StateControllerScriptResourceName);
			RegisterIncludeScript(typeof(ASPxDashboardViewer), DashboardViewerClientDataAPIScriptResourceName);
			RegisterIncludeScript(typeof(ASPxDashboardViewer), DashboardViewerScriptResourceName);
		}
		public override void RegisterStyleSheets() {
			if(DesignMode)
				return;
			base.RegisterStyleSheets();
		}
		protected override void RegisterDefaultRenderCssFile() {
			if(DesignMode)
				return;
			RegisterDevExtremeCss(Page, UsefulColorScheme);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxDashboardViewer), DashboardViewerCommonCssResourceName);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxDashboardViewer), string.Format(DashboardViewerThemeCssResourceName, UsefulColorScheme));
			ResourceManager.RegisterScriptBlock(Page, "DashboardColorScheme", string.Format("<script>DevExpress.viz.currentTheme('generic', '{0}')</script>", UsefulColorScheme));
		}
		protected void ProcessClientRequest(Hashtable data) {
			string task = (string)data["Task"];
			switch(task) {
				case "Initialize": {
						InitializeSessionArgs args = RequestArgsParser.ParseInitalizeSessionArgs(data);
						args.Settings.SessionTimeout = SessionTimeout > TimeSpan.MaxValue.TotalSeconds ? TimeSpan.MaxValue : TimeSpan.FromSeconds(SessionTimeout);
						args.Settings.HandleServerErrors = HandleServerErrors;
						dashboardServiceResult = DashboardService.Initialize(args);						
						break;
					}
				case "PerformAction": {
						dashboardServiceResult = DashboardService.PerformAction(RequestArgsParser.ParsePerformActionArgs(data));
						break;
					}
				case "ReloadData": {
						dashboardServiceResult = DashboardService.ReloadData(RequestArgsParser.ParseReloadDataArgs(data));
						break;
					}
				case "Export": {
						ExportArgs args = RequestArgsParser.ParseExportArgs(data);
						ExportInfo exportInfo = args.ExportInfo;
						exportInfo.FontInfo = exportOptions.FontInfo.GetDashboardFontInfo();
						dashboardServiceResult = DashboardService.Export(args);
						CreateExportResponse(exportInfo.ExportOptions.FileName, exportInfo.ExportOptions, dashboardServiceResult, args.Stream);
						break;
					}
				default:
					dashboardServiceResult = null;
					break;
			}
			if(HandleServerErrors && dashboardServiceResult.Error != null)
				throw new HttpException(dashboardServiceResult.Error.Message, dashboardServiceResult.Error);
		}
		void OnClientEvent(string eventArgument) {
			ProcessClientRequest((Hashtable)HtmlConvertor.FromJSON(eventArgument));
		}
		void CreateExportResponse(string fileName, DashboardReportOptions options, DashboardServiceResult serviceResult, Stream stream) {
			FormatOptions opts = options.FormatOptions;
			string fileFormat = UnknownFileFormat;
			switch(opts.Format) {
				case DashboardExportFormat.PDF:
					fileFormat = "pdf";
					break;
				case DashboardExportFormat.Image:
					ImageFormat imageFormat = opts.ImageOptions.Format;
					if(imageFormat.Equals(ImageFormat.Png))
						fileFormat = "png";
					if(imageFormat.Equals(ImageFormat.Jpeg))
						fileFormat = "jpeg";
					if(imageFormat.Equals(ImageFormat.Gif))
						fileFormat = "gif";
					break;
				case DashboardExportFormat.Excel:
					switch(opts.ExcelOptions.Format) {
						case ExcelFormat.Xls:
							fileFormat = "xls";
							break;
						case ExcelFormat.Xlsx:
							fileFormat = "xlsx";
							break;
						case ExcelFormat.Csv:
							fileFormat = "csv";
							break;
						default:
							fileFormat = UnknownFileFormat;
							break;
					}
					break;
				default:
					throw new NotSupportedException();
			}
			string contentTypePrefix = opts.Format == DashboardExportFormat.Image ? "image" : "application";
			string contentType = String.Format("{0}/{1}", contentTypePrefix, fileFormat);
			stream.Seek(0, SeekOrigin.Begin);
			if(serviceResult.ResultCode != DashboardServiceResultCode.Success) {
				stream.SetLength(0);
				StreamWriter writer = new StreamWriter(stream);
				writer.WriteLine(serviceResult.InternalErrorType);
				writer.WriteLine(serviceResult.Error.Message);
				writer.Flush();
				fileFormat = "txt";
				contentType = "text/plain";
				fileName = "ExportError";
			}
			StreamToResponse(stream, fileName, true, fileFormat, contentType);
		}
		protected virtual void StreamToResponse(Stream stream, string fileName, bool saveAsFile, string fileFormat, string contentType) {
			HttpUtils.WriteFileToResponse(Page, stream, fileName, true, fileFormat, contentType, false);
			stream.Dispose();
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			OnClientEvent(eventArgument);
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			OnClientEvent(eventArgument);
		}
		protected override object GetCallbackResult() {
			JSONOptions options = new JSONOptions { ProcessStructs = true };
			return string.Format("({0})", HtmlConvertor.ToJSON(dashboardServiceResult, options));
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			DataBind();
			if(!DesignMode) {
				BrowserInfo browserInfo = RenderUtils.Browser;
				if(Context != null && browserInfo.IsIE && browserInfo.Version < 8)
					Controls.Add(new LiteralControl(DashboardWebLocalizer.GetString(DashboardWebStringId.IncompatibleBrowser)));
			}
		}
		protected string GetResolvedXmlPath(string xml) {
			if(string.IsNullOrEmpty(xml))
				return null;
			return UrlUtils.ResolvePhysicalPath(xml);
		}
		protected override string GetStartupScript() {
			StringBuilder script = new StringBuilder();
			script.Append(base.GetStartupScript());
			script.Append("Globalize.culture('");
			script.Append(Thread.CurrentThread.CurrentCulture.Name);
			script.Append("');");
			return script.ToString();
		}
	}
	internal class ResFinder {
	}
}
