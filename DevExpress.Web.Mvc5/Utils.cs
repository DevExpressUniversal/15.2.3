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
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using System.Web.Util;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.UI;
using System.Globalization;
using System.Configuration;
using System.Web.UI.WebControls;
namespace DevExpress.Web.Mvc {
	using System.ComponentModel.DataAnnotations;
	using DevExpress.Web;
	using DevExpress.Web.Mvc.Internal;
	public static class DevExpressHelper {
		public static string Theme {
			get { return ASPxWebControl.GlobalTheme; }
			set { ASPxWebControl.GlobalTheme = value; }
		}
		public static string StyleSheetTheme {
			get { return ASPxWebControl.GlobalStyleSheetTheme; }
			set { ASPxWebControl.GlobalStyleSheetTheme = value; }
		}
		public static string ClientUIControlColorScheme {
			get { return ASPxWebClientUIControl.GlobalColorScheme; }
			set { ASPxWebClientUIControl.GlobalColorScheme = value; }
		}
		public static bool IsCallback {
			get { return MvcUtils.IsCallback(); }
		}
		public static string CallbackName {
			get { return MvcUtils.CallbackName; }
		}
		public static string CallbackArgument {
			get { return MvcUtils.CallbackArgument; }
		}
		public static string GetUrl(object routeValues) {
			return Utils.GetUrl(routeValues);
		}
		[Obsolete("Use the ViewContext.Writer.Write(string value) method instead.")]
		public static void WriteToResponse(string value) {
			Utils.Writer.Write(value);
		}
		[Obsolete("Use the ViewContext.Writer.Write(string format, params object[] arg) method instead.")]
		public static void WriteToResponse(string format, params object[] arg) {
			Utils.Writer.Write(format, arg);
		}
		[Obsolete("Use the ViewContext.Writer.WriteLine(string value) method instead.")]
		public static void WriteLineToResponse(string value) {
			Utils.Writer.WriteLine(value);
		}
		[Obsolete("Use the ViewContext.Writer.WriteLine(string format, params object[] arg) method instead.")]
		public static void WriteLineToResponse(string format, params object[] arg) {
			Utils.Writer.WriteLine(format, arg);
		}
	}
}
namespace DevExpress.Web.Mvc.Internal {
	public static class Utils {
		public const string CallbackHtmlContentPlaceholder = "<%html%>";
		public const string CallbackHtmlContentPrefix = "/*DXHTML*/";
		const string InnerWriterContextKey = "DXInnerWriter";
		const string InnerWriterCountContextKey = "DXInnerWriterCount";
		public const string UtilsScriptResourceName = "DevExpress.Web.Mvc.Scripts.Utils.js";
		public const string GridLookupScriptResourceName = "DevExpress.Web.Mvc.Scripts.GridLookup.js";
		public const string GridViewScriptResourceName = "DevExpress.Web.Mvc.Scripts.GridView.js";
		public const string FormLayoutScriptResourceName = "DevExpress.Web.Mvc.Scripts.FormLayout.js";
		public const string HtmlEditorScriptResourceName = "DevExpress.Web.Mvc.Scripts.HtmlEditor.js";
		public const string SpellCheckerScriptResourceName = "DevExpress.Web.Mvc.Scripts.SpellChecker.js";
		public const string FileManagerScriptResourceName = "DevExpress.Web.Mvc.Scripts.FileManager.js";
		public const string CallbackPanelScriptResourceName = "DevExpress.Web.Mvc.Scripts.CallbackPanel.js";
		public const string NavBarScriptResourceName = "DevExpress.Web.Mvc.Scripts.NavBar.js";
		public const string PopupControlScriptResourceName = "DevExpress.Web.Mvc.Scripts.PopupControl.js";
		public const string TabControlScriptResourceName = "DevExpress.Web.Mvc.Scripts.TabControl.js";
		public const string TreeViewScriptResourceName = "DevExpress.Web.Mvc.Scripts.TreeView.js";
		public const string UploadControlScriptResourceName = "DevExpress.Web.Mvc.Scripts.UploadControl.js";
		public const string LabelScriptResourceName = "DevExpress.Web.Mvc.Scripts.Label.js";
		public const string ComboBoxScriptResourceName = "DevExpress.Web.Mvc.Scripts.ComboBox.js";
		public const string CalendarScriptResourceName = "DevExpress.Web.Mvc.Scripts.Calendar.js";
		public const string CaptchaScriptResourceName = "DevExpress.Web.Mvc.Scripts.Captcha.js";
		public const string FilterControlScriptResourceName = "DevExpress.Web.Mvc.Scripts.FilterControl.js";
		public const string ListBoxScriptResourceName = "DevExpress.Web.Mvc.Scripts.ListBox.js";
		public const string ChartScriptResourceName = "DevExpress.Web.Mvc.Scripts.Chart.js";
		public const string ButtonScriptResourceName = "DevExpress.Web.Mvc.Scripts.Button.js";
		public const string BinaryImageResourceName = "DevExpress.Web.Mvc.Scripts.BinaryImage.js";
		public const string ReportScriptResourceName = "DevExpress.Web.Mvc.Scripts.Report.js";
		public const string ReportDesignerScriptResourceName = "DevExpress.Web.Mvc.Scripts.ReportDesigner.js";
		public const string WebDocumentViewerScriptResourceName = "DevExpress.Web.Mvc.Scripts.WebDocumentViewer.js";
		public const string PivotGridScriptResourceName = "DevExpress.Web.Mvc.Scripts.PivotGrid.js";
		public const string SchedulerScriptResourceName = "DevExpress.Web.Mvc.Scripts.Scheduler.js";
		public const string DockPanelScriptResourceName = "DevExpress.Web.Mvc.Scripts.DockPanel.js";
		public const string DockManagerScriptResourceName = "DevExpress.Web.Mvc.Scripts.DockManager.js";
		public const string DataViewScriptResourceName = "DevExpress.Web.Mvc.Scripts.DataView.js";
		public const string TreeListScriptResourceName = "DevExpress.Web.Mvc.Scripts.TreeList.js";
		public const string ImageGalleryScriptResourceName = "DevExpress.Web.Mvc.Scripts.ImageGallery.js";
		public const string ImageZoomScriptResourceName = "DevExpress.Web.Mvc.Scripts.ImageZoom.js";
		public const string ImageZoomNavigatorScriptResourceName = "DevExpress.Web.Mvc.Scripts.ImageZoomNavigator.js";
		public const string TokenBoxScriptResourceName = "DevExpress.Web.Mvc.Scripts.TokenBox.js";
		public const string RoundPanelScriptResourceName = "DevExpress.Web.Mvc.Scripts.RoundPanel.js";
		public const string SpreadsheetScriptResourceName = "DevExpress.Web.Mvc.Scripts.Spreadsheet.js";
		public const string RichEditScriptResourceName = "DevExpress.Web.Mvc.Scripts.RichEdit.js";
		public const string CardViewScriptResourceName = "DevExpress.Web.Mvc.Scripts.CardView.js";
		public const string GridAdapterScriptResourceName = "DevExpress.Web.Mvc.Scripts.GridAdapter.js";
		public static int MvcAssemblyMajorVersion {
			get {
				int assemblyMajorVersion = 0;
				try {
					assemblyMajorVersion = typeof(Controller).Assembly.GetName().Version.Major;
				} catch(System.Security.SecurityException) {
					string webPagesVersion = ConfigurationManager.AppSettings["webpages:Version"];
					if(!string.IsNullOrEmpty(webPagesVersion))
						assemblyMajorVersion = int.Parse(webPagesVersion.Split('.')[0]) + 2;
				}
				return assemblyMajorVersion;
			}
		}
		public static TextWriter Writer {
			get { return (HtmlHelperExtension.ViewContext != null) ? HtmlHelperExtension.ViewContext.Writer : HttpContext.Current.Response.Output; }
		}
		public static MvcHtmlString GetInnerWriterOutput(Action renderDelegate) {
			renderDelegate();
			return MvcHtmlString.Create(string.Empty);
		}
		public static HtmlTextWriter CreateHtmlTextWriter() {
			return CreateHtmlTextWriter(Writer);
		}
		public static HtmlTextWriter CreateHtmlTextWriter(TextWriter writer) {
			return HttpContext.Current.Request.Browser.CreateHtmlTextWriter(writer);
		}
		public static string GetErrorMessage(this ModelState modelState) {
			if(modelState.Errors.Count == 0)
				return string.Empty;
			ModelError error = modelState.Errors[0];
			if(error.Exception != null)
				return error.Exception.Message;
			return error.ErrorMessage;
		}
		public static string GetUrl(object routeValues) {
			if(HttpContext.Current != null) {
				RequestContext requestContext = null;
				if(HttpContext.Current.Handler is MvcHandler)
					requestContext = ((MvcHandler)HttpContext.Current.Handler).RequestContext;
				else {
					HttpContextWrapper contextWrapper = new HttpContextWrapper(HttpContext.Current);
					requestContext = new RequestContext(contextWrapper, new RouteData());
				}
				UrlHelper urlHelper = new UrlHelper(requestContext, RouteTable.Routes);
				return urlHelper.RouteUrl(string.Empty, CreateRouteValueDictionary(routeValues));
			}
			return string.Empty;
		}
		static RouteValueDictionary CreateRouteValueDictionary(object routeValues) {
			RouteValueDictionary routeValueDictionary = null;
			if (routeValues is IDictionary<string, object>)
				routeValueDictionary = new RouteValueDictionary((IDictionary<string, object>)routeValues);
			else
				routeValueDictionary = new RouteValueDictionary(routeValues);
			return routeValueDictionary;
		}
		public static bool IsThemeSpecified(string inlineTheme) {
			return !string.IsNullOrEmpty(Utils.GetFinalDxTheme(inlineTheme)) || !string.IsNullOrEmpty(Utils.GetFinalMsTheme(inlineTheme));
		}
		private static bool ApplyMSStyleSheetTheme(Control control, string styleSheetTheme) {
			string finalMSTheme = GetFinalTheme(styleSheetTheme, true);
			return ApplyMSTheme(control, finalMSTheme);
		}
		private static bool ApplyMSTheme(Control control, string theme) {
			string finalMSTheme = GetFinalMsTheme(theme);
			return ApplyMSThemeCore(control, finalMSTheme);
		}
		static bool ApplyMSThemeCore(Control control, string finalMSTheme) {
			if(string.IsNullOrEmpty(finalMSTheme))
				return false;
			using(DummyPage dummyPage = new DummyPage()) {
				return dummyPage.ApplyTheme(finalMSTheme, control);
			}
		}
		private static bool ApplyDXStyleSheetTheme(Control control, string param = null) {
			return ApplyDXTheme(control, null, true);
		}
		private static bool ApplyDXTheme(Control control, string theme) {
			return ApplyDXTheme(control, theme, false);
		}
		private static bool ApplyDXTheme(Control control, string theme, bool isStyleSheetTheme) {
			if(!isStyleSheetTheme && string.IsNullOrEmpty(Utils.GetFinalDxTheme(theme)))
				return false;
			ASPxWebControl aspxWebControl = control as ASPxWebControl;
			try {
				if(isStyleSheetTheme)
					aspxWebControl.ApplyStyleSheetThemeInternal();
				else {
					aspxWebControl.Theme = theme;
					aspxWebControl.ApplyThemeInternal();
				}
			}
			catch(ArgumentException e) {
				if(e.ParamName != ThemableControlBuilder.ThemeAttributeName)
					throw e;
				return false;
			}
			return true;
		}
		public static string GetFinalMsStyleSheetTheme() {
			return GetFinalTheme(null, true);
		}
		public static string GetFinalMsTheme(string inlineTheme) {
			return GetFinalTheme(inlineTheme, false);
		}		
		static string GetFinalTheme(string inlineTheme, bool isStyleSheetTheme) {
			if(!string.IsNullOrEmpty(inlineTheme))
				return inlineTheme;
			try {
				string globalTheme = GetFinalDxTheme(inlineTheme, isStyleSheetTheme);
				if(!string.IsNullOrEmpty(globalTheme))
					return globalTheme;
				PagesSection section = WebConfigurationManager.GetSection("system.web/pages") as PagesSection;
				if(section != null) {
					string msTheme = isStyleSheetTheme ? section.StyleSheetTheme : section.Theme;
					if(!string.IsNullOrEmpty(msTheme))
						return msTheme;
				}
			}
			catch(System.Security.SecurityException) { }
			return string.Empty;
		}
		public static string GetFinalDxStyleSheetTheme() {
			return GetFinalDxTheme(null, true);
		}
		public static string GetFinalDxTheme(string inlineTheme) {
			return GetFinalDxTheme(inlineTheme, false);
		}
		private static string GetFinalDxTheme(string inlineTheme, bool isStyleSheetTheme) {
			if(!string.IsNullOrEmpty(inlineTheme))
				return inlineTheme;
			return isStyleSheetTheme ? DevExpressHelper.StyleSheetTheme : DevExpressHelper.Theme;
		}
		public static void ApplyStyleSheetTheme(ASPxWebControl aspxWebControl) {
			ApplyTheme(aspxWebControl, Utils.ApplyMSStyleSheetTheme, Utils.ApplyDXStyleSheetTheme, null, true);
		}
		public static void ApplyTheme(ASPxWebControl aspxWebControl, string theme) {
			ApplyTheme(aspxWebControl, Utils.ApplyMSTheme, Utils.ApplyDXTheme, theme, false);
		}
		private static void ApplyTheme(ASPxWebControl aspxWebControl, Func<ASPxWebControl, string, bool> applyMSTheme, Func<ASPxWebControl, string, bool> applyDXTheme,
			string theme, bool isStyleSheetTheme) {
			if(!applyDXTheme(aspxWebControl, theme) && !applyMSTheme(aspxWebControl, theme)) {
				if(isStyleSheetTheme) {
					string dxStyleSheetTheme = GetFinalDxStyleSheetTheme();
					theme = !string.IsNullOrEmpty(dxStyleSheetTheme) ? dxStyleSheetTheme : GetFinalMsStyleSheetTheme();
				}
				if(!string.IsNullOrEmpty(theme))
					throw new ArgumentException(string.Format("Cannot find the '{0}' theme.", theme), ThemableControlBuilder.ThemeAttributeName);
			}
		}
		public static RequestContext GetRequestContext() {
			return new RequestContext(
				new HttpContextWrapper(HttpContext.Current),
				new RouteData()
			);
		}
		public static string GetFormCollectionValue(string key) {
			NameValueCollection formCollection = GetUnvalidateFormCollection();
			return formCollection != null ? formCollection[key] : null;
		}
		static NameValueCollection GetUnvalidateFormCollection() {
			object unvalidatedRequest = HttpUtils.GetUnvalidatedRequest(HttpContext.Current.Request);
			if(unvalidatedRequest == null)
				return null;
			var formProperty = unvalidatedRequest.GetType().GetProperty("Form");
			if(formProperty == null)
				return null;
			return formProperty.GetValue(unvalidatedRequest, null) as NameValueCollection;
		}
	}
	public static class ExportUtils {
		public static ActionResult Export(ExtensionBase extension, Action<Stream> write, string fileName, bool saveAsFile, string fileFormat) {
			FileStreamResult result = new FileStreamResult(new MemoryStream(), HttpUtils.GetContentType(fileFormat));
			write(result.FileStream);
			PrepareDownloadResult(extension, fileName, saveAsFile, fileFormat, ref result);
			return result;
		}
		public static void PrepareDownloadResult(ExtensionBase extension, string fileName, bool saveAsFile, string fileFormat, ref FileStreamResult result) {
			result.FileStream.Position = 0;
			if(saveAsFile)
				result.FileDownloadName = GetFileName(extension, fileName, "." + fileFormat);
		}
		static string GetFileName(ExtensionBase extension, string fileName, string fileExtension) {
			if(!string.IsNullOrEmpty(fileName)) {
				string fileNamePattern = @"([^\s]+(\.(?i)(" + fileExtension.Substring(1) + "))$)";
				if(!Regex.IsMatch(fileName, fileNamePattern))
					fileName += fileExtension;
				return Utils.MvcAssemblyMajorVersion > 3 || RenderUtils.Browser.IsChrome || RenderUtils.Browser.IsFirefox 
					? fileName : HttpUtils.EncodeFileName(fileName).Replace("%2c", ",");
			}
			if(!string.IsNullOrEmpty(extension.Settings.Name))
				return extension.Settings.Name + fileExtension;
			return extension.GetType().Name + fileExtension;
		}
	}
	public class MvcUrlResolutionService : IUrlResolutionService {
		UrlHelper urlHelper;
		public static void Initialize() {
			if(MvcUtils.MvcUrlResolutionService != null)
				return;
			UrlHelper urlHelper = new UrlHelper(Utils.GetRequestContext());
			MvcUtils.MvcUrlResolutionService = new MvcUrlResolutionService(urlHelper);
		}
		public MvcUrlResolutionService(UrlHelper urlHelper) {
			this.urlHelper = urlHelper;
		}
		public string ResolveClientUrl(string relativeUrl) {
			return this.urlHelper.Content(relativeUrl);
		}
	}
	public class DevExpressRequestValidator: RequestValidator {
		public DevExpressRequestValidator() { }
		public static bool IsValidRequestString(string value) {
			int validationFailureIndex;
			var validator = new DevExpressRequestValidator();
			return validator.IsValidRequestString(HttpContext.Current, value, RequestValidationSource.Form, null, out validationFailureIndex);
		}
		public static void ValidateRequestString(string value) {
			if(!IsValidRequestString(value))
				throw new HttpRequestValidationException(string.Format("A potentially dangerous Request value was detected from the client (Property=\"{0}\")", value));
		}
		protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex) {
			if(string.IsNullOrEmpty(value)) {
				validationFailureIndex = 0;
				return true;
			}
			return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
		}
	}
	public class MvcPostDataCollection : NameValueCollection, IUnvalidatedValueProvider, System.Web.Mvc.IValueProvider {
		string lockPostfix;
		public MvcPostDataCollection(System.Web.Mvc.IValueProvider provider, bool isBatchEditCollection, string postfix)
			: this(provider) {
			lockPostfix = postfix;
			IsBatchEditCollection = isBatchEditCollection;
		}
		public MvcPostDataCollection(System.Web.Mvc.IValueProvider provider) {
			ValueProvider = provider;
		}
		protected internal System.Web.Mvc.IValueProvider ValueProvider { get; private set; }
		protected internal bool IsBatchEditCollection { get; private set; }
		public override string Get(string name) {
			if(!ValueProvider.ContainsPrefix(name))
				return null;
			return ValueProvider.GetValue(name).AttemptedValue;
		}
		string GetCorrectPrefix(string prefix){
			return prefix + this.lockPostfix;
		}
		[Obsolete("Use this[string name] operator insted."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string this[int index] { get { return base[index]; } }
		[Obsolete("Use Get(string name) method insted."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new string Get(int index) {
			return base.Get(index);
		}
		#region IValueProvider Members
		bool System.Web.Mvc.IValueProvider.ContainsPrefix(string prefix) {
			prefix = GetCorrectPrefix(prefix);
			return ValueProvider.ContainsPrefix(prefix);
		}
		ValueProviderResult System.Web.Mvc.IValueProvider.GetValue(string key) {
			key = GetCorrectPrefix(key);
			return ValueProvider.GetValue(key);
		}
		#endregion
		#region IUnvalidatedValueProvider Members
		ValueProviderResult IUnvalidatedValueProvider.GetValue(string key, bool skipValidation) {
			key = GetCorrectPrefix(key);
			var unvalidatedValueProvider = (ValueProvider as IUnvalidatedValueProvider);
			return unvalidatedValueProvider != null ? unvalidatedValueProvider.GetValue(key, skipValidation) : null;
		}
		#endregion
	}
	public class ExtensionsHelper {
		public const string UnobtrusiveValidationRulesClientFieldName = "unobtrusiveValidationAttributes";
		protected static readonly IDictionary<string, object> EmptyAttributesDictionary = new Dictionary<string, object>();
		public static ModelMetadata GetMetadataByEditorName(string name, ViewDataDictionary viewData) {
			if(viewData == null || string.IsNullOrEmpty(name))
				return null;
			var experession = string.Empty;
			if(viewData.ModelMetadata == null || viewData.ModelMetadata.PropertyName != name)
				experession = GetPropertyNameOfModel(name, viewData);
			return ModelMetadata.FromStringExpression(experession, viewData);
		}
		static string GetPropertyNameOfModel(string editorName, ViewDataDictionary viewData) { 
			string htmlFieldPrefix = viewData.TemplateInfo.HtmlFieldPrefix + ".";
			string modelPropertyName = editorName;
			if(modelPropertyName.IndexOf(htmlFieldPrefix) == 0)
				modelPropertyName = modelPropertyName.Remove(0, htmlFieldPrefix.Length);
			return modelPropertyName;
		}
		public static ModelMetadata GetMetadataForColumn(string fieldName) {
			if(HtmlHelperExtension.HtmlHelper == null) return null;
			return ExtensionsHelper.GetMetadataForEnumerableTypes(fieldName, HtmlHelperExtension.HtmlHelper.ViewData);
		}
		public static void SetUnobtrusiveValidationAttributes(WebControl control, string name, ModelMetadata metadata) {
			IDictionary<string, object> unobtrusiveAttributes = GetUnobtrusiveValidationAttributes(name, metadata);
			if(unobtrusiveAttributes != null) {
				foreach(var attribute in unobtrusiveAttributes) {
					control.Attributes.Add(attribute.Key, attribute.Value.ToString());
				}
			}
		}
		public static IDictionary<string, object> GetUnobtrusiveValidationAttributes(string name, ModelMetadata metadata) {
			if(HtmlHelperExtension.HtmlHelper == null)
				return EmptyAttributesDictionary;
			return HtmlHelperExtension.HtmlHelper
				.GetUnobtrusiveValidationAttributes(name, metadata)
				.ToDictionary(r => r.Key, r => DecodeRuleParameters(r.Value));
		}
		static object DecodeRuleParameters(object parameters) {
			if(parameters == null) return null;
			var convertibleObj = parameters as IConvertible;
			var stringValue = convertibleObj != null ? convertibleObj.ToString(CultureInfo.InvariantCulture) : parameters.ToString();
			return HttpUtility.HtmlDecode(stringValue);
		}
		public static IDictionary<string, object> GetUnobtrusiveValidationRulesForColumnEditor(string fieldName, ViewContext viewContext, Func<string, string> getEditorIdByFieldNameMethod) {
			if(viewContext == null)
				return new Dictionary<string, object>();
			FormContext sourceFormContext = viewContext.FormContext;
			if(sourceFormContext == null)
				viewContext.FormContext = new FormContext();
			try {
				return GetUnobtrusiveValidationRulesForColumnEditor(fieldName, viewContext.ViewData, getEditorIdByFieldNameMethod);
			}
			finally {
				viewContext.FormContext = sourceFormContext;
			}
		}
		static IDictionary<string, object> GetUnobtrusiveValidationRulesForColumnEditor(string fieldName, ViewDataDictionary viewData, Func<string, string> getEditorIdByFieldName) {
			ModelMetadata rowMetadata = GetMetadataForEnumerableTypes(fieldName, viewData);
			if (rowMetadata == null)
				return EmptyAttributesDictionary;
			IDictionary<string, object> validationRules = GetUnobtrusiveValidationAttributes(fieldName, rowMetadata);
			PrepareRemoteValidationRules(rowMetadata, getEditorIdByFieldName, validationRules);
			return validationRules;
		}
		static void PrepareRemoteValidationRules(ModelMetadata metadata, Func<string, string> getEditorIdByFieldNameMethod, IDictionary<string, object> validationRules) {
			RemoteAttribute remoteAttribute = AttributeMapper.GetAttribute<RemoteAttribute>(metadata);
			if(remoteAttribute == null)
				return;
			List<string> targetFields = new List<string>() { metadata.PropertyName };
			if(!string.IsNullOrEmpty(remoteAttribute.AdditionalFields))
				targetFields.AddRange(remoteAttribute.AdditionalFields.Split(',').Select(s => s.Trim()));
			Hashtable fieldEditorsMap = new Hashtable();
			foreach(var fieldName in targetFields) {
				string editorId = getEditorIdByFieldNameMethod(fieldName);
				if(!string.IsNullOrEmpty(editorId))
					fieldEditorsMap[fieldName] = editorId;
			}
			if(fieldEditorsMap.Count > 0)
				validationRules.Add("data-val-remote-dxfieldsmap", fieldEditorsMap);
		}
		static ModelMetadata GetMetadataForEnumerableTypes(string accessPropertyName, ViewDataDictionary viewData) {
			Type modelType = viewData.ModelMetadata != null ? viewData.ModelMetadata.ModelType : null;
			if(modelType == null || !modelType.IsGenericType || string.IsNullOrEmpty(accessPropertyName))
				return null;
			if(!ReflectionUtils.IsGenericIEnumerable(modelType))
				return null;
			Type rowType = modelType.GetGenericArguments().Last();
			List<PropertyInfo> propertyInfoList = GetPropertyInfoListByType(rowType, accessPropertyName);
			if(propertyInfoList == null || propertyInfoList.Count == 0)
				return null;
			ParameterExpression parameter = Expression.Parameter(modelType, "c");
			MethodCallExpression getFirstElementExpression = Expression.Call(typeof(Enumerable), "FirstOrDefault", new Type[] { rowType }, parameter);
			LambdaExpression accessPropertyExpression = Expression.Lambda(GetPropertyAccessExpressionBody(getFirstElementExpression, propertyInfoList), parameter);
			MethodInfo methodInfo = typeof(ModelMetadata).GetMethod("FromLambdaExpression");
			methodInfo = methodInfo.MakeGenericMethod(modelType, propertyInfoList.Last().PropertyType);
			var parameterizedViewData = Activator.CreateInstance(typeof(ViewDataDictionary<>).MakeGenericType(modelType), viewData);
			return (ModelMetadata)methodInfo.Invoke(null, new object[] { accessPropertyExpression, parameterizedViewData });
		}
		static Expression GetPropertyAccessExpressionBody(Expression bodyAccessParameter, List<PropertyInfo> propertyInfoList) {
			Expression expressionBody = bodyAccessParameter;
			for(int i = 0; i < propertyInfoList.Count(); i++) {
				expressionBody = Expression.Property(expressionBody, propertyInfoList[i]);
			}
			return expressionBody;
		}
		static List<PropertyInfo> GetPropertyInfoListByType(Type type, string propertyName) {
			string[] properties = propertyName.Split('.');
			List<PropertyInfo> propertyInfoList = new List<PropertyInfo>();
			Type currentType = type;
			for(int i = 0; i < properties.Length && currentType != null; i++) {
				var propertyInfo = currentType.GetProperties().FirstOrDefault(p => p.Name == properties[i]);
				currentType = propertyInfo != null ? propertyInfo.PropertyType : null;
				if(propertyInfo != null)
					propertyInfoList.Add(propertyInfo);
			}
			return propertyInfoList;
		}
		public static void AssignValidationSettingsToColumnEditor(DevExpress.Web.ASPxEdit editor, string fieldName, ViewContext viewContext, bool showModelErrorsForEditors) {
			if(viewContext == null || viewContext.ViewData == null)
				return;
			ModelStateDictionary modelState = viewContext.ViewData.ModelState;
			editor.ValidationSettings.EnableCustomValidation |= viewContext.ClientValidationEnabled && viewContext.UnobtrusiveJavaScriptEnabled;
			if(showModelErrorsForEditors && editor.IsValid && modelState != null && !modelState.IsValidField(fieldName)) {
				editor.IsValid = false;
				editor.ErrorText = modelState[fieldName].GetErrorMessage();
				editor.ValidationSettings.ErrorDisplayMode = DevExpress.Web.ErrorDisplayMode.ImageWithTooltip;
			}
		}
		public static void ConfigureEditPropertiesByMetadata(EditPropertiesBase properties, ModelMetadata metadata) {
			if(properties == null || metadata == null)
				return;
			properties.DisplayFormatString = metadata.DisplayFormatString ?? string.Empty;
			SetNullTextForEditProperties(properties, metadata.NullDisplayText ?? string.Empty);
			AttributeMapper.PrepareEditorProperties(properties, metadata);
		}
		static void SetNullTextForEditProperties(EditPropertiesBase properties, string nullText) {
			properties.NullDisplayText = nullText;
			if (properties is TextBoxProperties)
				((TextBoxProperties)properties).NullText = nullText;
			if (properties is ComboBoxProperties)
				((ComboBoxProperties)properties).NullText = nullText;
			if (properties is DropDownEditProperties)
				((DropDownEditProperties)properties).NullText = nullText;
			if (properties is ColorEditProperties)
				((ColorEditProperties)properties).NullText = nullText;
			if (properties is DateEditProperties)
				((DateEditProperties)properties).NullText = nullText;
			if (properties is TokenBoxProperties)
				((TokenBoxProperties)properties).NullText = nullText;
			if (properties is SpinEditProperties)
				((SpinEditProperties)properties).NullText = nullText;
		}
		public static string GetFullHtmlFieldName(LambdaExpression expression) {
			if (expression == null) 
				return string.Empty;
			if (expression.Body.NodeType == ExpressionType.Convert && expression.Body is UnaryExpression)
				expression = Expression.Lambda(((UnaryExpression)expression.Body).Operand, expression.Parameters);
			string fieldName = ExpressionHelper.GetExpressionText(expression);
			return HtmlHelperExtension.HtmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(fieldName);
		}
		public static System.Web.Mvc.IValueProvider CreateDefaultValueProvider() {
			var fakeControllerContext = new ControllerContext(new HttpContextWrapper(HttpContext.Current), new System.Web.Routing.RouteData(), new FakeController());
			var valueProviderCollection = ValueProviderFactories.Factories
				.Select(factory => factory.GetValueProvider(fakeControllerContext))
				.Where(factory => factory != null)
				.ToList<System.Web.Mvc.IValueProvider>();
			return new ValueProviderCollection(valueProviderCollection);
		}
		class FakeController : ControllerBase {
			protected override void ExecuteCore() { }
		}
	}
	public static class DialogHelper {
		public static void ForceOnInit(Control control) {
			IDialogFormElementRequiresLoad element = control as IDialogFormElementRequiresLoad;
			if(element != null)
				element.ForceInit();
		}
		public static void ForceOnLoad(Control control) {
			IDialogFormElementRequiresLoad element = control as IDialogFormElementRequiresLoad;
			if(element != null)
				element.ForceLoad();
			foreach(Control childControl in control.Controls)
				ForceOnLoad(childControl);
		}
	}
}
