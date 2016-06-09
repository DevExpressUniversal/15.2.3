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
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Native;
using DevExpress.XtraReports.Web.Native.ClientControls;
using DevExpress.XtraReports.Web.Native.ClientControls.Services;
using DevExpress.XtraReports.Web.QueryBuilder;
using DevExpress.XtraReports.Web.QueryBuilder.Native;
using DevExpress.XtraReports.Web.QueryBuilder.Native.Services;
using DevExpress.XtraReports.Web.ReportDesigner.Native;
using DevExpress.XtraReports.Web.ReportDesigner.Native.DataContracts;
using DevExpress.XtraReports.Web.ReportDesigner.Native.Services;
using Constants = DevExpress.XtraReports.Web.Native.Constants.QueryBuilder;
using DevExpress.DataAccess.ConnectionParameters;
namespace DevExpress.XtraReports.Web {
	[DXWebToolboxItem(true)]
	[Designer("DevExpress.XtraReports.Web.Design.ASPxQueryBuilderDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)]
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "ASPxQueryBuilder.bmp")]
	[ToolboxTabName(AssemblyInfo.DXTabData)]
	public class ASPxQueryBuilder : ASPxWebClientUIControl {
		#region resources
		new const string WebCssResourcePath = WebResourceNames.WebCssResourcePath;
		const string
			WebScriptResourcePath = WebResourceNames.WebScriptResourcePath,
			DXTremeCssPostfix = WebResourceNames.DXTremeCssPostfix,
			DXTremeJSPostfix = WebResourceNames.DXTremeJSPostfix;
		internal const string
			CssResourcePathPrefix = WebCssResourcePath + "query-builder-",
			LightCssResourceName = CssResourcePathPrefix + ColorSchemeLight + DXTremeCssPostfix,
			LightCompactCssResourceName = CssResourcePathPrefix + ColorSchemeLightCompact + DXTremeCssPostfix,
			DarkCssResourceName = CssResourcePathPrefix + ColorSchemeDark + DXTremeCssPostfix,
			DarkCompactCssResourceName = CssResourcePathPrefix + ColorSchemeDarkCompact + DXTremeCssPostfix,
			ScriptResourceName = WebScriptResourcePath + "QueryBuilder.js",
			DXScriptResourceName = WebScriptResourcePath + "query-builder" + DXTremeJSPostfix;
		#endregion
		internal const string DefaultHandlerUri = "DXQB.axd";
		const string
			ClientSideCategoryName = "Client-Side",
			ValidateQueryByExecutionName = "ValidateQueryByExecution";
		readonly IQueryBuilderModelGenerator queryBuilderModelGenerator;
		readonly IJSContentGenerator<QueryBuilderModel> jsContentGenerator;
		readonly IHtmlContentGenerator htmlContentGenerator;
		TableQuery initialQuery = null;
		string connectionName;
		DataConnectionParametersBase connectionParameters = null;
		protected override bool HasRootTag() {
			return true;
		}
		protected override void RegisterDefaultRenderCssFile() {
			if(DesignMode) {
				return;
			}
			RegisterDevExtremeCss(Page, UsefulColorScheme);
			RegisterJQueryUICss(Page);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxQueryBuilder), ASPxQueryBuilder.CssResourcePathPrefix + UsefulColorScheme + DXTremeCssPostfix);
			ResourceManager.RegisterCssResource(Page, typeof(ASPxWebDocumentViewer), WebResourceNames.WebClientUIControl.CssResourceName);
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		#region ctors
		static ASPxQueryBuilder() {
			var subscriber = InitContainerAndCreateManagerSubscriber();
			ASPxHttpHandlerModule.Subscribe(subscriber);
		}
		public ASPxQueryBuilder()
			: this(DefaultQueryBuilderContainer.Current) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxQueryBuilder(IServiceProvider serviceProvider)
			: this(
			serviceProvider.GetService<IQueryBuilderModelGenerator>(),
			serviceProvider.GetService<IQueryBuilderHtmlContentGenerator>(),
			serviceProvider.GetService<IJSContentGenerator<QueryBuilderModel>>()
			) {
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxQueryBuilder(
			IQueryBuilderModelGenerator webDocumentViewerModelGenerator,
			IQueryBuilderHtmlContentGenerator queryBuilderHtmlContentGenerator,
			IJSContentGenerator<QueryBuilderModel> jsContentGenerator
			) {
			this.queryBuilderModelGenerator = webDocumentViewerModelGenerator;
			this.jsContentGenerator = jsContentGenerator;
			this.htmlContentGenerator = queryBuilderHtmlContentGenerator;
			Width = Constants.DefaultWidth;
			Height = Constants.DefaultHeight;
		}
		#endregion
		#region events
		static readonly object SaveQueryEvent = new object();
		public event SaveQueryEventHandler SaveQuery {
			add { Events.AddHandler(SaveQueryEvent, value); }
			remove { Events.RemoveHandler(SaveQueryEvent, value); }
		}
		void RaiseSaveQuery(TableQuery queryLayout, string selectStatement) {
			var ev = Events[SaveQueryEvent] as SaveQueryEventHandler;
			if(ev != null) {
				var args = new SaveQueryEventArgs(queryLayout, selectStatement);
				ev(this, args);
			}
		}
		[Category("Client-Side")]
		public event CustomJSPropertiesEventHandler CustomJSProperties {
			add { Events.AddHandler(EventCustomJsProperties, value); }
			remove { Events.RemoveHandler(EventCustomJsProperties, value); }
		}
		#endregion
		public new static void StaticInitialize() { }
		protected override string GetClientObjectClassName() {
			return "ASPxClientQueryBuilder";
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			QueryBuilderInput input = QueryBuilderInputLoader.FromString(eventArgument, this.ValidateQueryByExecution);
			RaiseSaveQuery(input.ResultQuery, input.SelectStatement);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!string.IsNullOrEmpty(this.connectionName) || this.connectionParameters != null) {
				QueryBuilderModel queryBuilderModel = this.queryBuilderModelGenerator.Generate(this.connectionName, this.connectionParameters, this.initialQuery);
				jsContentGenerator.Generate(stb, localVarName, queryBuilderModel);
			}
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
			RegisterIncludeScript(typeof(ASPxQueryBuilder), WebResourceNames.WebClientUIControl.ScriptResourceName);
			RegisterIncludeScript(typeof(ASPxQueryBuilder), ScriptResourceName);
			if(WebClientUIControlHelper.IsSupportedBrowser) {
				RegisterIncludeScript(typeof(ASPxQueryBuilder), WebResourceNames.WebClientUIControl.DXScriptResourceName);
				RegisterIncludeScript(typeof(ASPxQueryBuilder), DXScriptResourceName);
			}
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new QueryBuilderClientSideEvents();
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(!DesignMode) {
				string htmlContent =  this.htmlContentGenerator.GetContent();
				Controls.Add(new LiteralControl(htmlContent));
			}
		}
		[DefaultValue(typeof(Unit), "100%")]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[DefaultValue(typeof(Unit), "850px")]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[AutoFormatDisable]
		[Category(ClientSideCategoryName)]
		[DefaultValue("")]
		[Localizable(false)]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
		[MergableProperty(false)]
		[Category(ClientSideCategoryName)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatDisable]
		public QueryBuilderClientSideEvents ClientSideEvents {
			get { return (QueryBuilderClientSideEvents)ClientSideEventsInternal; }
		}
		[Category("Client-Side")]
		[Browsable(false)]
		[AutoFormatDisable]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Dictionary<string, object> JSProperties {
			get { return JSPropertiesInternal; }
		}
		public bool ValidateQueryByExecution {
			get { return GetBoolProperty(ValidateQueryByExecutionName, true); }
			set { SetBoolProperty(ValidateQueryByExecutionName, true, value); }
		}
		protected override bool IsCallBacksEnabled() {
			return true;
		}
		public void OpenConnection(string connectionName) {
			this.connectionName = connectionName;
			this.OpenConnection(connectionName, null);
		}
		public void OpenConnection(string connectionName, TableQuery tableQuery) {
			this.connectionName = connectionName;
			this.connectionParameters = null;
			this.initialQuery = tableQuery;
		}
		public void OpenConnection(DataConnectionParametersBase connectionParameters) {
			this.OpenConnection(connectionParameters, null);
		}
		public void OpenConnection(DataConnectionParametersBase connectionParameters, TableQuery tableQuery) {
			this.connectionName = null;
			this.connectionParameters = connectionParameters;
			this.initialQuery = tableQuery;
		}
		public string DesignerType {
			get { return "ASPxQueryBuilder"; }
		}
		static IHttpModuleSubscriber InitContainerAndCreateManagerSubscriber() {
			IServiceProvider serviceProvider = DefaultQueryBuilderContainer.Current;
			if(serviceProvider == null) {
				serviceProvider = QueryBuilderBootstrapper.CreateInitializedContainer();
				DefaultQueryBuilderContainer.Current = serviceProvider;
			}
			return new ManagerModuleSubscriber<IQueryBuilderRequestManager>(serviceProvider, DefaultHandlerUri);
		}
	}
	public delegate void SaveQueryEventHandler(object sender, SaveQueryEventArgs e);
	public class SaveQueryEventArgs : EventArgs {
		public TableQuery ResultQuery { get; private set; }
		public string SelectStatement { get; private set; }
		public SaveQueryEventArgs(TableQuery query, string selectStatement) {
			ResultQuery = query;
			SelectStatement = selectStatement;
		}
	}
}
