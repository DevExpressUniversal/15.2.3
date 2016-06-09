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
using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data.Linq;
namespace DevExpress.Web.Mvc {
	using DevExpress.Web;
	using DevExpress.Web.FilterControl;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	public abstract class ExtensionBase {
		ASPxWebControl control;
		SettingsBase settings;
		bool postDataLoaded = false;
		ViewContext viewContext;
		ModelMetadata metadata;
		Action<ExtensionBase> onBeforeCreateControl;
		public ExtensionBase(SettingsBase settings)
			: this(settings, null) {
		}
		public ExtensionBase(SettingsBase settings, ViewContext viewContext):
			this(settings, viewContext, null){
		}
		protected ExtensionBase(SettingsBase settings, ViewContext viewContext, ModelMetadata metadata)
			: this(settings, viewContext, metadata, null) {
		}
		protected ExtensionBase(SettingsBase settings, ViewContext viewContext, ModelMetadata metadata, Action<ExtensionBase> onBeforeCreateControl) {
			if(settings == null)
				throw new ArgumentNullException("Control's settings cannot be null.");
			this.viewContext = viewContext ?? HtmlHelperExtension.ViewContext; 
			this.metadata = metadata ?? ExtensionsHelper.GetMetadataByEditorName(settings.Name, ViewData);
			this.onBeforeCreateControl = onBeforeCreateControl;
			ApplySettings(settings);
			ValidateSettings();
			ApplyStyleSheetThemeToControl(Control);
			AssignInitialProperties();
			ApplyThemeToControl(Control);
			Init();
		}
		protected virtual void ApplySettings(SettingsBase settings) {
			this.settings = settings;
		}
		protected internal void ApplyStyleSheetThemeToControl(ASPxWebControl control) {
			if(control != null) {
				control.SkinID = settings.SkinID;
				if(settings.EnableTheming && !Utils.IsThemeSpecified(settings.Theme)) {
					Utils.ApplyStyleSheetTheme(control);
					EditorExtension editorExtension = this as EditorExtension;
					if(editorExtension != null) {
						if(string.IsNullOrEmpty(editorExtension.Settings.Properties.CssPostfix) && !string.IsNullOrEmpty(Control.CssPostfix)) {
							editorExtension.Settings.Properties.CssPostfix = Control.CssPostfix;
							editorExtension.Settings.Properties.CssFilePath = Control.CssFilePath;
						}
					}
				}
			}
		}
		protected internal void ApplyThemeToControl(ASPxWebControl control) {
			ApplyThemeToControl(control, settings.Theme);
		}
		protected internal void ApplyThemeToControl(ASPxWebControl control, string theme) {
			if(settings.EnableTheming && control != null)
				Utils.ApplyTheme(control, theme);
		}
		protected internal ASPxWebControl Control {
			get {
				if(control == null) {
					if(this.onBeforeCreateControl != null)
						this.onBeforeCreateControl(this);
					control = CreateControl();
				}
				return control;
			}
			protected set {
				control = value;
			}
		}
		protected ASPxDataWebControlBase DataControl {
			get { return Control as ASPxDataWebControlBase; }
		}
		protected internal virtual ICallbackEventHandler CallbackEventHandler {
			get { return (ICallbackEventHandler)Control; }
		}
		protected internal IPostBackDataHandler PostBackDataHandler {
			get { return (IPostBackDataHandler)Control; }
		}
		protected NameValueCollection PostDataCollection {
			get { return ((IPostDataCollection)Control).PostDataCollection; }
		}
		protected static HttpContext Context {
			get { return HttpContext.Current; }
		}
		protected static HttpRequest Request {
			get { return HttpContext.Current.Request; }
		}
		protected static HttpResponse Response {
			get { return HttpContext.Current.Response; }
		}
		protected static string ProcessedCallbackArgument {
			get { 
				return CallbackSeparatorPosition > -1 
					? MvcUtils.CallbackArgument.Substring(CallbackSeparatorPosition + 1)
					: string.Empty; 
			}  
		}
		static int CallbackID { 
			get {
				return CallbackSeparatorPosition > -1 
					? int.Parse(MvcUtils.CallbackArgument.Substring(1, CallbackSeparatorPosition - 1))
					: -1; 
			}
		}
		static int CallbackSeparatorPosition { 
			get { 
				return !string.IsNullOrEmpty(MvcUtils.CallbackArgument) 
					? MvcUtils.CallbackArgument.IndexOf(RenderUtils.CallBackSeparator)
					: -1; 
			} 
		}
		protected internal SettingsBase Settings {
			get { return settings; }
			protected set { settings = value; }
		}
		protected internal ViewContext ViewContext {
			get { return viewContext; }
		}
		protected internal System.Web.Mvc.IValueProvider ValueProvider {
			get { return ViewContext != null && ViewContext.Controller != null ? ViewContext.Controller.ValueProvider : null; }
		}
		protected internal ViewDataDictionary ViewData {
			get { return (ViewContext != null) ? ViewContext.ViewData : null; }
		}
		protected internal ControllerBase Controller {
			get { return (ViewContext != null) ? ViewContext.Controller : null; }
		}
		protected internal ControllerContext ControllerContext {
			get { return (Controller != null) ? Controller.ControllerContext : null; }
		}
		protected internal ModelStateDictionary ModelState {
			get {
				Controller controller = Controller as Controller;
				return (controller != null) ? controller.ModelState : null;
			}
		}
		protected ModelMetadata Metadata {
			get { return metadata; }
		}
		protected internal FormContext FormContext {
			get { return (ViewContext != null) ? ViewContext.FormContext : null; }
		}
		protected internal TextWriter Writer {
			get { return (ViewContext != null) ? ViewContext.Writer : Utils.Writer; }
		}
		protected internal bool IsPostDataLoaded { get { return this.postDataLoaded; } }
		protected virtual void AssignInitialProperties() {
			Control.AccessKey = settings.AccessKey;
			Control.ControlStyle.CopyFrom(settings.ControlStyle);
			AssignAttributes();
			Control.Enabled = settings.Enabled;
			Control.EnableTheming = settings.EnableTheming;
			Control.EncodeHtml = settings.EncodeHtml;
			Control.Height = settings.Height;
			Control.ID = settings.Name;
			foreach(string key in settings.Style.Keys)
				Control.Style.Add(key, settings.Style[key]);
			Control.TabIndex = settings.TabIndex;
			Control.ToolTip = settings.ToolTip;
			Control.Width = settings.Width;
		}
		protected void AssignAttributes() {
			foreach(string key in settings.Attributes.Keys) {
				var attributeValue = settings.Attributes[key];
				if(string.Equals("class", key, StringComparison.CurrentCultureIgnoreCase)) 
					RenderUtils.AppendDefaultDXClassName(Control, attributeValue);
				else
					Control.Attributes.Add(key, attributeValue);
			}
		}
		protected virtual void AssignRenderProperties() {
		}
		public void Render() {
			AssertModuleRegistration();
			if(!string.IsNullOrEmpty(Settings.Name)) {
				if(ExtensionsFactory.RenderedExtensions.ContainsKey(Settings.Name)) {
					ExtensionsFactory.RenderedExtensions.Clear(); 
					throw new Exception("An extension with '" + Settings.Name + "' name already rendered.");
				}
				ExtensionsFactory.RenderedExtensions.Add(Settings.Name, this);
			}
			MvcRenderMode savedRenderMode = MvcUtils.RenderMode;
			MvcUtils.RenderMode = IsSimpleIDsRenderMode() ? MvcRenderMode.RenderWithSimpleIDs : MvcRenderMode.Render;
			try {
				AssignRenderProperties();
				PrepareControlProperties();
				PrepareControl();
				if (IsCallback()) {
					LoadPostData();
					ProcessCallback(MvcUtils.CallbackArgument);
				}
				else {
					PreRender();
					RenderControl(Control);
				}
			}
			finally {
				DisposeControl();
				MvcUtils.RenderMode = savedRenderMode;
			}
		}
		protected internal virtual bool IsCallback(){
			return !string.IsNullOrEmpty(MvcUtils.CallbackName) && MvcUtils.CallbackName == Control.ID;
		}
		internal bool IsFilterControlCallback() {
			return MvcUtils.CallbackName == Control.ID + FilterControlCallbackPostfix;
		}
		internal static string FilterControlCallbackPostfix {
			get { return string.Format("_{0}_{1}", ASPxPopupFilterControl.PopupFilterControlFormID, ASPxPopupFilterControl.PopupFilterControlID); }
		}
		public MvcHtmlString GetHtml() {
			return Utils.GetInnerWriterOutput(() => { Render(); });
		}
		protected virtual void Init() {
			if(Settings.Init != null)
				Settings.Init(Control, EventArgs.Empty);
		}
		protected virtual void PreRender() {
			if(Settings.PreRender != null)
				Settings.PreRender(Control, EventArgs.Empty);
		}
		protected virtual void ProcessCallback(string callbackArgument) {
			CallbackEventHandler.RaiseCallbackEvent(callbackArgument);
			RenderCallbackResult();
		}
		protected virtual void RenderCallbackResult() {
			string callbackResult = CallbackEventHandler.GetCallbackResult();
			if(GetCallbackResultControl() == null) {
				RenderString(callbackResult);
				return;
			}
			RenderString(callbackResult);
			RenderString(Utils.CallbackHtmlContentPrefix);
			try {
				RenderCallbackResultControl();
			}
			catch(Exception errorInfo) {
				var stringWriter = ViewContext.Writer as StringWriter;
				if(stringWriter != null)
					stringWriter.GetStringBuilder().Clear();
				else {
					var switchWriter = Response.Output;
					Response.Output = ViewContext.Writer;
					Response.Clear();
					Response.Output = switchWriter;
				}
				if(HttpContext.Current != null)
					HttpContext.Current.AddError(errorInfo);
				var errorCallbackResult = new Hashtable();
				var callbackErrorHandler = ((IHandleCallbackError)Control);
				var errorMessage = callbackErrorHandler.OnCallbackException(errorInfo);
				callbackErrorHandler.HandleErrorOnGetCallbackResult(errorCallbackResult, errorMessage);
				RenderString(RenderUtils.CallBackResultPrefix + HtmlConvertor.ToJSON(errorCallbackResult));
			}
		}
		protected virtual Control GetCallbackResultControl() {
			return null;
		}
		protected virtual void RenderCallbackResultControl() {
			RenderControl(GetCallbackResultControl());
		}
		protected internal void LoadPostData() {
			if(postDataLoaded) return;
			LoadPostDataInternal();
			postDataLoaded = true;
		}
		protected virtual void LoadPostDataInternal() {
			PostBackDataHandler.LoadPostData("", PostDataCollection);
		}
		protected internal virtual void PrepareControlProperties() {
		}
		protected internal virtual void PrepareControl() {
		}
		protected internal virtual void DisposeControl() {
			Control.Dispose();
		}
		protected virtual bool IsSimpleIDsRenderModeSupported() {
			return false;
		}
		protected virtual bool IsSimpleIDsRenderMode() {
			return string.IsNullOrEmpty(Settings.Name) && IsSimpleIDsRenderModeSupported();
		}
		protected void RenderControl(Control control) {
			if(control == null) return;
			HtmlTextWriter writer = Utils.CreateHtmlTextWriter(Writer);
			control.RenderControl(writer);
		}
		protected void RenderString(string s) {
			if(string.IsNullOrEmpty(s)) return;
			Writer.Write(s);
		}
		protected internal virtual void RegisterStyleSheets() {
			if(GetRenderHtml() == string.Empty)
				Control.RegisterStyleSheets();
		}
		protected internal virtual void RegisterClientIncludeScripts() {
			if(GetRenderHtml() == string.Empty) {
				Control.RegisterClientIncludeScripts();
				Control.RegisterClientScriptBlocks();
			}
		}
		object lockGetRenderHtml = new object();
		private string GetRenderHtml() {
			lock(lockGetRenderHtml) {
				string html = DevExpress.Web.Internal.RenderUtils.GetRenderResult(Control);
				return html.Trim(new char[] { ' ', '\n', '\r' });
			}
		}
		protected void BindInternal(object dataObject) {
			if(DataControl == null) return;
			BindToDataSource(dataObject);
		}
		protected virtual void BindToLINQDataSourceInternal(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			if(DataControl == null)
				return;
			LinqServerModeDataSource datasource = new LinqServerModeDataSource();
			datasource.ContextTypeName = contextTypeName;
			datasource.TableName = tableName;
			if(selectingMethod != null)
				datasource.Selecting += selectingMethod;
			if(exceptionThrownMethod != null)
				datasource.ExceptionThrown += exceptionThrownMethod;
			DataControl.DataSource = datasource;
		}
		protected virtual void BindToEFDataSourceInternal(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			if(DataControl == null)
				return;
			EntityServerModeDataSource datasource = new EntityServerModeDataSource();
			datasource.ContextTypeName = contextTypeName;
			datasource.TableName = tableName;
			if(selectingMethod != null)
				datasource.Selecting += selectingMethod;
			if(exceptionThrownMethod != null)
				datasource.ExceptionThrown += exceptionThrownMethod;
			DataControl.DataSource = datasource;
		}
		protected void BindToSiteMapInternal(string fileName, bool showStartingNode) {
			if(DataControl == null)
				return;
			ASPxSiteMapDataSource dataSource = new ASPxSiteMapDataSource();
			dataSource.SiteMapFileName = fileName;
			dataSource.ShowStartingNode = showStartingNode;
			BindToDataSource(dataSource);
		}
		protected void BindToXMLInternal(string fileName, string xPath, string transformFileName) {
			if(DataControl == null)
				return;
			if(UrlUtils.IsAppRelativePath(fileName) && ViewContext != null)
				fileName = ViewContext.RequestContext.HttpContext.Server.MapPath(fileName);
			XmlDataSource dataSource = new XmlDataSource();
			dataSource.DataFile = fileName;
			dataSource.TransformFile = transformFileName;
			dataSource.XPath = xPath;
			BindToDataSource(dataSource);
		}
		protected virtual void BindToDataSource(object dataSource) {
			DataControl.DataSource = dataSource;
			if(IsBindInternalRequired())
				DataControl.DataBind();
		}
		protected internal virtual bool IsBindInternalRequired() {
			return true;
		}
		protected abstract ASPxWebControl CreateControl();
		protected virtual void ValidateSettings() {
			if(!IsSimpleIDsRenderModeSupported() && string.IsNullOrEmpty(Settings.Name))
				throw new Exception("The 'Name' property should be specified for this extension.");
		}
		void AssertModuleRegistration() {
#if !DEBUG
			if(Context != null) {
				if(HttpContext.Current.Application[ResourceManager.HandlerRegistrationFlag] == null || !(bool)HttpContext.Current.Application[ResourceManager.HandlerRegistrationFlag])
					throw new Exception(StringResources.Error_ModuleIsNotRegistered);
			}
#endif
		}
		protected internal static ContentResult GetCustomDataCallbackResult(object data) {
			Hashtable result = new Hashtable();
			result[CallbackResultProperties.Result] = data;
			result[CallbackResultProperties.ID] = CallbackID;
			return new ContentResult() { Content = RenderUtils.CallBackResultPrefix + HtmlConvertor.ToJSON(result) };
		}
	}
}
