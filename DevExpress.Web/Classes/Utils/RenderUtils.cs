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
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.Web.Localization;
using System.Web.UI.HtmlControls;
namespace DevExpress.Web.Internal {
	public interface IStopLoadPostDataOnCallbackMarker {
	}
	public enum ListType { Bulleted, Ordered }
	public class RenderUtils {
		internal const string StyleSheetsContainerControlID = "StyleSheetsContainer";
		internal const string IECompatibilityMetaID = "IECompatibilityMeta";
		internal const string IECompatibilityMetaObsoleteID = "IE8CompatibilityMeta";
		internal const string IECompatibilityMetaKey = "X-UA-Compatible";
		internal const string IECompatibilityMetaValueFormat = "IE={0}";
		internal const string MSTouchDraggableClassName = "dxMSTouchDraggable";
		public const char IndexSeparator = 'i';
		public const char CallBackSeparator = ':';
		public const string CallBackResultPrefix = "/*DX*/";
		public const string DefaultStyleNamePrefix = "dx";
		public const string DefaultCustomStyleNamePrefix = "dxh";
		public const string DefaultInputCellStyleName = "dxic";
		public const string IncludedScriptIDPrefix = "dxis_";
		public const string StartupScriptIDPrefix = "dxss_";
		public const string AccessibilityEmptyUrl = "javascript:;";
		public const string AccessibilityMarkerClass = "dxalink";
		public const string CallbackControlIDParamName = "__CALLBACKID";
		public const string CallbackControlParamParamName = "__CALLBACKPARAM";
		public const string PostbackEventArgumentParamName = "__EVENTARGUMENT";
		public const int LoadingPanelZIndex = 30000;
		public const int LoadingDivZIndex = 29999;
		public const int MenuZIndex = 20000;
		public const int PopupControlZIndex = 10000;
		public const int InvalidDimension = -10000;
		public const string DefaultUserControlFileExtension = ".ascx";
		public const string CSDefaultUserControlCodeBehindFileExtension = ".ascx.cs";
		public const string CSDefaultUserControlDesignerFileExtension = ".ascx.designer.cs";
		public const string VBDefaultUserControlCodeBehindFileExtension = ".ascx.vb";
		public const string VBDefaultUserControlDesignerFileExtension = ".ascx.designer.vb";
		public const string DialogFormCallbackStatus = "DialogForm";
		public const string DefaultTempDirectory = "DXTempFolder";
		public const string DefaultFormsAppRelativeDirectoryPathTemplate = "~/DevExpress/{0}Forms/";
		public const string DefaultSPFormsAppRelativeDirectoryPathTemplate = "~/resources/DevExpress/{0}Forms/";
		public const string DefaultUserControlDesignersRelativeNamespace = "Designers.";
		public const string DummyPostBackArgument = "[PostBackArgumentPlaceholder]";
		public const string MVCQueryParamName = "DXMVC";
		public const string ProgressInfoQueryParamName = "DXProgressInfo";
		public const string ProgressHandlerPage = "ASPxUploadProgressHandlerPage.ashx";
		public const string ProgressHandlerKeyQueryParamName = "DXProgressHandlerKey";
		public const string UploadingCallbackQueryParamName = "DXUploadingCallback";
		public const string HelperUploadingCallbackQueryParamName = "DXHelperUploadingCallback";
		protected internal const string FrameworksScriptsResourcePath = "DevExpress.Web.Scripts.Frameworks.";
		protected internal const string GlobalizeScriptResourceName = FrameworksScriptsResourcePath + "globalize.js";
		protected internal const string GlobalizeCulturesScriptResourceName = FrameworksScriptsResourcePath + "globalize.cultures.js";
		protected internal const string JQueryScriptResourceName = FrameworksScriptsResourcePath + "jquery-1.11.3.min.js";
		protected internal const string JQueryUIScriptResourceName = FrameworksScriptsResourcePath + "jquery-ui-1.10.3.custom.min.js";
		protected internal const string JQueryValidateScriptResourceName = FrameworksScriptsResourcePath + "jquery.validate.min.js";
		protected internal const string JQueryUnobtrusiveScriptResourceName = FrameworksScriptsResourcePath + "jquery.validate.unobtrusive.min.js";
		protected internal const string JQueryUnobtrusiveAjaxScriptResourceName = FrameworksScriptsResourcePath + "jquery.unobtrusive-ajax.min.js";
		protected internal const string KnockoutScriptResourceName = FrameworksScriptsResourcePath + "knockout-3.3.0.js";
		protected internal const string FrameworksCssResourcePath = "DevExpress.Web.Css.Frameworks.";
		protected internal const string JQueryUICssResourceName = FrameworksCssResourcePath + "jquery-ui-1.10.3.custom.min.css";
		protected internal const string DevExtremeScriptsResourcePath = "DevExpress.Web.Scripts.DevExtreme.";
		protected internal const string DevExtremeCoreScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-core.js";
		protected internal const string DevExtremeWidgetsBaseScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-widgets-base.js";
		protected internal const string DevExtremeWidgetsWebScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-widgets-web.js";
		protected internal const string DevExtremeVizCoreScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-viz-core.js";
		protected internal const string DevExtremeVizChartsScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-viz-charts.js";
		protected internal const string DevExtremeVizGaugesScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-viz-gauges.js";
		protected internal const string DevExtremeVizRangeSelectorScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-viz-rangeselector.js";
		protected internal const string DevExtremeVizSparklinesScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-viz-sparklines.js";
		protected internal const string DevExtremeVizVectorMapScriptResourceName = DevExtremeScriptsResourcePath + "dx.module-viz-vectormap.js";
		protected internal const string DevExtremeCssResourcePath = "DevExpress.Web.Css.DevExtreme.";
		protected internal const string DevExtremeCommonCssResourceName = DevExtremeCssResourcePath + "dx.common.css";
		protected internal const string DevExtremeLightCssResourceName = DevExtremeCssResourcePath + "dx.light.css";
		protected internal const string DevExtremeDarkCssResourceName = DevExtremeCssResourcePath + "dx.dark.css";
		protected internal const string DevExtremeLightCompactCssResourceName = DevExtremeCssResourcePath + "dx.light.compact.css";
		protected internal const string DevExtremeDarkCompactCssResourceName = DevExtremeCssResourcePath + "dx.dark.compact.css";
		protected internal const string DevExtremeThemeCssResourceFormat = DevExtremeCssResourcePath + "dx.{0}.css";
		protected internal const string DevExtremeCommonOverridesCssResourceName = DevExtremeCssResourcePath + "dx.common-overrides.css";
		private static BrowserInfo browserInfo;
		public static bool RaiseChangedOnLoadPostData = false;
		public static bool PreventLoadPostDataOnLoad {
			get { return HttpUtils.GetContextValue("PreventLoadPostDataOnLoad", false); }
			set { HttpUtils.SetContextValue("PreventLoadPostDataOnLoad", value); }
		}
		public static BrowserInfo Browser {
			get {
				if(browserInfo == null)
					browserInfo = new BrowserInfo();
				return browserInfo;
			}
		}
		[Obsolete("Use RenderUtils.Browser.IsIE instead.")]
		public static bool IsIE {
			get { return Browser.IsIE; }
		}
		[Obsolete("Use (RenderUtils.Browser.IsIE && RenderUtils.Browser.MajorVersion == 7) instead. Make sure that you need (IE ver == 7), but not (IE ver >= 7) check.")]
		public static bool IsIE7 {
			get { return Browser.IsIE && Browser.MajorVersion == 7; }
		}
		[Obsolete("Use (RenderUtils.Browser.IsIE && RenderUtils.Browser.MajorVersion == 8) instead.")]
		public static bool IsIE8 {
			get { return Browser.IsIE && Browser.MajorVersion == 8; }
		}
		[Obsolete("Use (RenderUtils.Browser.IsIE && RenderUtils.Browser.Version < 7) instead.")]
		public static bool IsIEVersionLessThan7 {
			get { return Browser.IsIE && Browser.Version < 7; }
		}
		[Obsolete("Use RenderUtils.Browser.IsMozilla instead.")]
		public static bool IsMozilla {
			get { return Browser.IsMozilla; }
		}
		[Obsolete("Use RenderUtils.Browser.IsNetscape instead.")]
		public static bool IsNetscape {
			get { return Browser.IsNetscape; }
		}
		[Obsolete("Use RenderUtils.Browser.IsFirefox instead.")]
		public static bool IsFirefox {
			get { return Browser.IsFirefox; }
		}
		[Obsolete("Use (RenderUtils.Browser.IsFirefox && RenderUtils.Browser.MajorVersion == 3) instead. Make sure that you need (Firefox ver == 3), but not (Firefox ver >= 3) check.")]
		public static bool IsFirefox3 {
			get { return Browser.IsFirefox && Browser.MajorVersion == 3; }
		}
		[Obsolete("Use RenderUtils.Browser.IsOpera instead.")]
		public static bool IsOpera {
			get { return Browser.IsOpera; }
		}
		[Obsolete("Use (RenderUtils.Browser.IsOpera && RenderUtils.Browser.MajorVersion == 8) instead.")]
		public static bool IsOpera8 {
			get { return Browser.IsOpera && Browser.MajorVersion == 8; }
		}
		[Obsolete("Use RenderUtils.Browser.Family.IsNetscape instead.")]
		public static bool IsNetscapeFamily {
			get { return Browser.Family.IsNetscape; }
		}
		[Obsolete("Use RenderUtils.Browser.Family.IsWebKit instead.")]
		public static bool IsSafariFamily {
			get { return Browser.Family.IsWebKit; }
		}
		[Obsolete("Use RenderUtils.Browser.IsSafari instead.")]
		public static bool IsSafari {
			get { return Browser.IsSafari; }
		}
		[Obsolete("Use (RenderUtils.Browser.IsSafari && RenderUtils.Browser.MajorVersion == 3) instead. Make sure that you need (Safari ver == 3), but not (Safari ver >= 3) check.")]
		public static bool IsSafari3 {
			get { return Browser.IsSafari && Browser.MajorVersion == 3; }
		}
		[Obsolete("Use (RenderUtils.Browser.IsSafari && RenderUtils.Browser.Version >= 3) instead.")]
		public static bool IsSafariVersionNonLessThan3 {
			get { return Browser.IsSafari && Browser.Version >= 3; }
		}
		[Obsolete("Use RenderUtils.Browser.IsChrome instead.")]
		public static bool IsChrome {
			get { return Browser.IsChrome; }
		}
		public static bool IsAnyCallback(Page page) {
			return (page != null && page.IsCallback) || HttpUtils.IsUpdatePanelCallback() || HttpUtils.IsMicrosoftAjaxCallback() || HttpUtils.IsUploadControlCallback() || MvcUtils.IsCallback();
		}
		public static bool IsOverflowStyleSeparated {
			get { return Browser.IsIE || Browser.IsSafari && Browser.Version >= 3 || Browser.IsChrome; }
		}
		public static bool IsSecureConnection {
			get {
				if(HttpContext.Current != null && HttpContext.Current.Request != null)
					return HttpContext.Current.Request.IsSecureConnection;
				return false;
			}
		}
		public static string GetClientIDPrefix(Control control) {
			string clientID = control.ClientID;
			return (clientID.Length == control.ID.Length) ? "" :
				clientID.Substring(0, clientID.Length - control.ID.Length - 1);
		}
		public static string GetRenderResult(Control control) {
			return GetRenderResult(control, null);
		}
		public static string GetRenderResult(Control control, string suffix) {
			using(StringWriter sw = new StringWriter(CultureInfo.InvariantCulture)) {
				using(HtmlTextWriter writer = new HtmlTextWriter(sw))
					control.RenderControl(writer);
				sw.Write(suffix);
				return sw.ToString();
			}
		}
		public static string GetControlChildrenRenderResult(Control control) {
			StringBuilder sb = new StringBuilder();
			int count = control.Controls.Count;
			for(int i = 0; i < count; i++)
				sb.Append(RenderUtils.GetRenderResult(control.Controls[i]));
			return sb.ToString();
		}
		public static WebControl CreateWebControl(HtmlTextWriterTag tag) {
			WebControl control = new InternalWebControl(tag);
			return control;
		}
		public static WebControl CreateWebControl(string tag) {
			WebControl control = new InternalWebControl(tag);
			return control;
		}
		public static WebControl CreateAnchor(string name) {
			HyperLink anchor = new HyperLink();
			anchor.EnableViewState = false;
			anchor.Attributes.Add("name", name);
			anchor.Font.Size = FontUnit.Point(0);
			if(!Browser.IsOpera) {
				WebControl div = CreateWebControl(HtmlTextWriterTag.Div);
				div.Style.Add("padding", "0px");
				div.Controls.Add(anchor);
				return div;
			} else
				return anchor;
		}
		public static WebControl CreateParagraph() {
			return CreateWebControl(HtmlTextWriterTag.P);
		}
		public static WebControl CreateList(ListType listType) {
			WebControl list = CreateWebControl(listType == ListType.Bulleted ? HtmlTextWriterTag.Ul : HtmlTextWriterTag.Ol);
			if(Browser.IsIE)
				SetPadding(list, 0);
			return list;
		}
		public static WebControl CreateListItem() {
			return CreateWebControl(HtmlTextWriterTag.Li);
		}
		public static Label CreateLabel() {
			Label label = new InternalLabel();
			label.EnableViewState = false;
			return label;
		}
		public static LiteralControl CreateLiteralControl(string text) {
			LiteralControl control = new LiteralControl(text);
			control.EnableViewState = false;
			return control;
		}
		public static LiteralControl CreateLiteralControl() {
			return CreateLiteralControl("");
		}
		public static LiteralControl CreateBr() {
			return CreateLiteralControl("<br />");
		}
		public static WebControl CreateDiv() {
			return CreateWebControl(HtmlTextWriterTag.Div);
		}
		public static WebControl CreateDiv(params string[] cssClasses) {
			var div = CreateWebControl(HtmlTextWriterTag.Div);
			div.CssClass = CombineCssClasses(cssClasses);
			return div;
		}
		public static HyperLink CreateHyperLink() {
			return CreateHyperLink(true);
		}
		public static InternalHyperLink CreateHyperLink(bool needResolveClientUrl) {
			return CreateHyperLink(needResolveClientUrl, false);
		}
		public static InternalHyperLink CreateHyperLink(bool needResolveClientUrl, bool isAlwaysHyperLink) {
			InternalHyperLink link = new InternalHyperLink(needResolveClientUrl);
			link.EnableViewState = false;
			link.IsAlwaysHyperLink = isAlwaysHyperLink;
			return link;
		}
		public static HiddenField CreateHiddenField() {
			return CreateHiddenField(string.Empty);
		}
		public static HiddenField CreateHiddenField(string id) {
			return CreateHiddenField(id, string.Empty);
		}
		public static HiddenField CreateHiddenField(string id, string value) {
			HiddenField field = new InternalHiddenField();
			field.EnableViewState = false;
			field.ID = id;
			field.Value = value;
			return field;
		}
		public static LiteralControl CreateHiddenFieldLiteral(string id, string value) {
			return new LiteralControl(GetHiddenFieldHtml(id, value));
		}
		public static Image CreateImage() {
			InternalImage image = new InternalImage();
			image.EnableViewState = false;
			image.GenerateEmptyAlternateText = true;
			return image;
		}
		public static InternalTable CreateTable(bool borderSeparate) {
			InternalTable table = new InternalTable();
			table.CellPadding = 0;
			table.CellSpacing = 0;
			if(borderSeparate)
				table.Style.Add("border-collapse", "separate");
			return table;
		}
		public static InternalTable CreateTable() {
			return CreateTable(false);
		}
		public static TableRow CreateTableRow() {
			TableRow row = new InternalTableRow();
			return row;
		}
		public static TableCell CreateTableCell() {
			TableCell cell = new InternalTableCell();
			return cell;
		}
		public static TableCell CreateTableHeaderCell(string scope) {
			TableCell cell = new TableHeaderCell();
			RenderUtils.SetStringAttribute(cell, "scope", scope);
			RenderUtils.SetStyleStringAttribute(cell, "font-weight", "normal");
			return cell;
		}
		public static TableCell CreateIndentCell() {
			return CreateTableCell();
		}
		public static WebControl CreateEmptySpaceControl() {
			return CreateEmptySpaceControl(1, 1);
		}
		public static WebControl CreateEmptySpaceControl(Unit width, Unit height) {
			WebControl control = CreateWebControl(HtmlTextWriterTag.Div);
			control.Width = width;
			control.Height = height;
			SetStyleStringAttribute(control, "overflow", "hidden");
			SetStyleStringAttribute(control, "font", "0");
			return control;
		}
		public static Table CreateEmptySpaceTable() {
			Table table = CreateTable();
			table.CellPadding = 0;
			table.CellSpacing = 0;
			TableRow row = CreateTableRow();
			TableCell cell = CreateTableCell();
			row.Cells.Add(cell);
			table.Rows.Add(row);
			return table;
		}
		public static TableCell CreateTemplateCell(ITemplate template, Control container) {
			TableCell cell = CreateTableCell();
			template.InstantiateIn(container);
			cell.Controls.Add(container);
			return cell;
		}
		public static WebControl CreateTemplateDiv(ITemplate template, Control container) {
			WebControl div = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			template.InstantiateIn(container);
			div.Controls.Add(container);
			return div;
		}
		public static WebControl CreateTemplateSpan(ITemplate template, Control container) {
			WebControl span = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			template.InstantiateIn(container);
			span.Controls.Add(container);
			return span;
		}
		public static void CreateTemplate(ITemplate template, TemplateContainerBase container, string containerID, WebControl parent) {
			template.InstantiateIn(container);
			container.ID = containerID;
			parent.Controls.Add(container);
		}
		public static WebControl CreateFakeIFrame(string id, int zIndex) {
			return CreateFakeIFrame(id, zIndex, "javascript:false");
		}
		public static WebControl CreateFakeIFrame(string id, int zIndex, string src) {
			WebControl iFrame = CreateWebControl(HtmlTextWriterTag.Iframe);
			iFrame.ID = id;
			if(!string.IsNullOrEmpty(src) || !RenderUtils.IsHtml5Mode(iFrame))
				iFrame.Attributes.Add("src", src);
			if(RenderUtils.IsHtml5Mode(iFrame))
				iFrame.Style.Add("overflow", "hidden");
			else
				iFrame.Attributes.Add("scrolling", "no");
			if(RenderUtils.IsHtml5Mode(iFrame))
				iFrame.Style.Add("border", "0");
			else
				iFrame.Attributes.Add("frameborder", "0");
			iFrame.Style.Add("position", "absolute");
			iFrame.Style.Add("display", "none");
			iFrame.Style.Add("z-index", zIndex.ToString());
			iFrame.Style.Add("filter", "progid:DXImageTransform.Microsoft.Alpha(Style=0, Opacity=0)");
			iFrame.Attributes.Add("title", ASPxperienceLocalizer.GetString(ASPxperienceStringId.AccessibilityIFrameTitle));
			return iFrame;
		}
		public static WebControl CreateIFrame() {
			return CreateIFrame(string.Empty);
		}
		public static WebControl CreateIFrame(string id) {
			return CreateIFrame(id, string.Empty);
		}
		public static WebControl CreateIFrame(string id, string name) {
			return CreateIFrame(id, name, string.Empty);
		}
		public static WebControl CreateIFrame(string id, string name, string src) {
			WebControl iFrame = CreateWebControl(HtmlTextWriterTag.Iframe);
			if(!string.IsNullOrEmpty(id))
				iFrame.ID = id;
			if(!string.IsNullOrEmpty(name))
				SetStringAttribute(iFrame, "name", name);
			if(string.IsNullOrEmpty(src))
				src = "javascript:false";
			iFrame.Attributes.Add("src", src);
			if(!RenderUtils.IsHtml5Mode(iFrame))
				iFrame.Attributes.Add("frameborder", "0");
			return iFrame;
		}
		public static WebControl CreateUploadForm(string id) {
			WebControl form = CreateWebControl(HtmlTextWriterTag.Form);
			form.ID = id;
			form.Attributes.Add("enctype", "multipart/form-data");
			return form;
		}
		public static Control CreateClearElement() {
			return CreateLiteralControl("<b class=\"dx-clear\"></b>");
		}
		public static void PrepareHyperLink(HyperLink link, string text, string navigateUrl, string target, string toolTip, bool enabled) {
			PrepareHyperLink(link, text, navigateUrl, target, toolTip, enabled, false);
		}
		public static void PrepareHyperLink(HyperLink link, string text, string navigateUrl, string target, string toolTip, bool enabled, bool requiresTooltip) {
			if(text != "")
				link.Text = text;
			link.Enabled = enabled;
			link.NavigateUrl = navigateUrl;
			link.Target = target;
			link.ToolTip = toolTip;
			InternalHyperLink internalHyperLink = link as InternalHyperLink;
			if(internalHyperLink != null)
				internalHyperLink.RequiresTooltip = requiresTooltip;
		}
		public static void PrepareHyperLinkForAccessibility(HyperLink link, bool enabled, bool isAccessibilityEnabled, bool focusable) {
			PrepareHyperLinkForAccessibility((WebControl)link, enabled, isAccessibilityEnabled, focusable);
		}
		public static void PrepareHyperLinkForAccessibility(WebControl link, bool enabled, bool isAccessibilityEnabled, bool focusable) {
			if(!focusable) {
				if(!Browser.IsOpera && !Browser.Family.IsWebKit)
					link.TabIndex = -1;
			} else if(enabled && isAccessibilityEnabled) {
				link.CssClass = CombineCssClasses(link.CssClass, AccessibilityMarkerClass);
			}
		}
		public static void PrepareHyperLinkStyle(WebControl control, Style style) {
			PrepareHyperLinkStyleCore(control, style, false);
		}
		public static void PrepareHyperLinkChildStyle(WebControl control, Style style) {
			PrepareHyperLinkStyleCore(control, style, true);
		}
		static void PrepareHyperLinkStyleCore(WebControl control, Style style, bool skipForeColor) {
			if(!skipForeColor)
				control.ForeColor = style.ForeColor;
			CssStyleCollection cssStyleCollection = style.GetStyleAttributes(control);
			string textDecoration = cssStyleCollection[HtmlTextWriterStyle.TextDecoration];
			SetTextDecoration(control, textDecoration);
		}
		public static void PrepareTable(Table table, int cellPadding, int cellSpacing, Unit width, Unit height) {
			table.CellPadding = cellPadding;
			table.CellSpacing = cellSpacing;
			if(!width.IsEmpty)
				table.Width = width;
			if(!height.IsEmpty)
				table.Height = height;
		}
		public static void PrepareIndentCell(TableCell cell, Unit width, bool allowMakeInvisible) {
			PrepareIndentCell(cell, width, Unit.Empty, allowMakeInvisible);
		}
		public static void PrepareIndentCell(TableCell cell, Unit width, Unit height) {
			PrepareIndentCell(cell, width, height, false);
		}
		public static void PrepareIndentCell(TableCell cell, Unit width, Unit height, bool allowMakeInvisible) {
			WebControl spaceControl = (cell.Controls.Count > 0) ? (cell.Controls[0] as WebControl) : null;
			if(spaceControl != null) {
				cell.Width = width;
				cell.Height = height;
				spaceControl.Width = width.IsEmpty ? 1 : width;
				spaceControl.Height = height.IsEmpty ? 1 : height;
			} else
				PrepareSpaceControl(cell, width, height, allowMakeInvisible);
		}
		public static void PrepareSpaceControl(WebControl control, Unit width, bool allowMakeInvisible) {
			PrepareSpaceControl(control, width, Unit.Empty, allowMakeInvisible);
		}
		public static void PrepareSpaceControl(WebControl control, Unit width, Unit height, bool allowMakeInvisible) {
			if(allowMakeInvisible && (width.IsEmpty || width.Value <= 0) && (height.IsEmpty || height.Value <= 0)) {
				control.Visible = false;
				return;
			}
			if(!width.IsEmpty || !height.IsEmpty)
				SetStyleStringAttribute(control, "font", "0");
			if(!width.IsEmpty)
				control.Style.Add("min-width", width.ToString());
			if(!height.IsEmpty)
				control.Style.Add("height", height.ToString());
		}
		public static void PrepareInput(WebControl input, string type, string name, string value) {
			SetStringAttribute(input, "type", type);
			SetStringAttribute(input, "name", name);
			SetStringAttribute(input, "value", value);
		}
		public static void PrepareTextArea(WebControl textAreaControl, int columns, int rows) {
			SetAttribute(textAreaControl, "cols", columns > 0 ? columns.ToString() : "", IsHtml5Mode(textAreaControl) ? "" : "unique");
			if(rows > 0) {
				if(Browser.Family.IsNetscape && rows > 1)
					--rows;
				SetAttribute(textAreaControl, "rows", rows.ToString(), "unique");
			} else
				SetAttribute(textAreaControl, "rows", "", IsHtml5Mode(textAreaControl) ? "" : "unique");
		}
		public static void PutInControlsSequentially(params Control[] controls) {
			if(controls.Length < 2)
				return;
			Control parent = controls[0];
			for(int i = 1; i < controls.Length; i++) {
				var current = controls[i];
				parent.Controls.Add(current);
				parent = current;
			}
		}
		public static string GetDefaultCursor() {
			return "default";
		}
		public static string GetPointerCursor() {
			return "pointer";
		}
		public static void SetScrollBars(WebControl control, ScrollBars scrollBars) {
			if(scrollBars == ScrollBars.None)
				SetScrollBars(control, ScrollBarMode.Hidden, ScrollBarMode.Hidden);
			else if(scrollBars == ScrollBars.Horizontal)
				SetScrollBars(control, ScrollBarMode.Visible, ScrollBarMode.Hidden);
			else if(scrollBars == ScrollBars.Vertical)
				SetScrollBars(control, ScrollBarMode.Hidden, ScrollBarMode.Visible);
			else if(scrollBars == ScrollBars.Both)
				SetScrollBars(control, ScrollBarMode.Visible, ScrollBarMode.Visible);
			else if(scrollBars == ScrollBars.Auto)
				SetScrollBars(control, ScrollBarMode.Auto, ScrollBarMode.Auto);
		}
		public static void SetScrollBars(WebControl control, ScrollBarMode horizontal, ScrollBarMode vertical) {
			SetScrollBars(control, horizontal, vertical, false);
		}
		public static void SetScrollBars(WebControl control, ScrollBarMode horizontal, ScrollBarMode vertical, bool designMode) {
			if(horizontal == ScrollBarMode.Hidden && vertical == ScrollBarMode.Hidden)
				return;
			Func<ScrollBarMode, string> getOverflow = (ScrollBarMode mode) => {
				if(mode == ScrollBarMode.Hidden)
					return "hidden";
				else if(mode == ScrollBarMode.Visible)
					return "scroll";
				else if(mode == ScrollBarMode.Auto)
					return "auto";
				return string.Empty;
			};
			if(horizontal == vertical) {
				control.Style[HtmlTextWriterStyle.Overflow] = getOverflow(horizontal);
				return;
			}
			if(designMode) {
				control.Style[HtmlTextWriterStyle.Overflow] = getOverflow(ScrollBarMode.Hidden);
				return;
			}
			control.Style[HtmlTextWriterStyle.OverflowX] = getOverflow(horizontal);
			control.Style[HtmlTextWriterStyle.OverflowY] = getOverflow(vertical);
		}
		public static void SetAttribute(WebControl control, string name, object value, object defaultValue) {
			if(value != null && defaultValue != null && !value.Equals(defaultValue))
				control.Attributes.Add(name, HtmlConvertor.ToHtml(value));
			else
				control.Attributes.Remove(name);
		}
		public static void SetStringAttribute(WebControl control, string name, string value) {
			SetAttribute(control, name, value, "");
		}
		public static void SetStyleAttribute(WebControl control, string name, object value, object defaultValue) {
			SetStyleAttribute(control, name, value, defaultValue, false);
		}
		struct StyleCache {
			public class StyleCacheComparer : IEqualityComparer<StyleCache> {
				public bool Equals(StyleCache x, StyleCache y) {
					return x.style == y.style && x.name == y.name && x.value.Equals(y.value) && x.makeImportant == y.makeImportant && ((x.defaultValue == null && y.defaultValue == null) || x.defaultValue.Equals(y.defaultValue));
				}
				public int GetHashCode(StyleCache obj) {
					return (obj.style != null ? obj.style.GetHashCode() : 0) ^ obj.name.GetHashCode() ^ obj.makeImportant.GetHashCode();
				}
			}
			bool makeImportant;
			string style;
			string name;
			object value;
			object defaultValue;
			public StyleCache(string style, string name, object value, bool makeImportant, object defaultValue) {
				this.style = style;
				this.name = name;
				this.value = value;
				this.makeImportant = makeImportant;
				this.defaultValue = defaultValue;
			}
		}
		static Dictionary<StyleCache, string> styleChangeCache = new Dictionary<StyleCache, string>(new StyleCache.StyleCacheComparer());
		public static void SetStyleAttribute(WebControl control, string name, object value, object defaultValue, bool makeImportant) {
			string style;
			StyleCache key = new StyleCache(control.Style.Value, name, value, makeImportant, defaultValue);
			if(!styleChangeCache.TryGetValue(key, out style)) {
				if(!value.Equals(defaultValue)) {
					string valueString = HtmlConvertor.ToHtml(value);
					if(makeImportant)
						MakeCssRuleImportant(ref name, ref valueString);
					control.Style.Add(name, valueString);
				} else
					control.Style.Remove(name);
				lock(styleChangeCache) {
					if(styleChangeCache.Count > 1000)
						styleChangeCache.Clear();
					styleChangeCache[key] = control.Style.Value;
				}
			} else
				if(control.Style.Value != style)
					control.Style.Value = style;
		}
		public static void SetStyleColorAttribute(WebControl control, string name, System.Drawing.Color value) {
			SetStyleColorAttribute(control, name, value, false);
		}
		static object colorEmpty = System.Drawing.Color.Empty;
		public static void SetStyleColorAttribute(WebControl control, string name, System.Drawing.Color value, bool makeImportant) {
			SetStyleAttribute(control, name, value.IsEmpty ? colorEmpty : value, colorEmpty, makeImportant);
		}
		public static void SetStyleStringAttribute(WebControl control, string name, string value) {
			SetStyleStringAttribute(control, name, value, false);
		}
		public static void SetStyleStringAttribute(WebControl control, string name, string value, bool makeImportant) {
			SetStyleAttribute(control, name, value, "", makeImportant);
		}
		public static void SetStyleUnitAttribute(WebControl control, string name, Unit value) {
			SetStyleUnitAttribute(control, name, value, false);
		}
		static object unitEmpty = Unit.Empty;
		public static void SetStyleUnitAttribute(WebControl control, string name, Unit value, bool makeImportant) {
			SetStyleAttribute(control, name, value.IsEmpty ? unitEmpty : value, unitEmpty, makeImportant);
		}
		public static void MergeImageWithItemToolTip(Image image, string itemToolTip) {
			if(!string.IsNullOrEmpty(itemToolTip) && string.IsNullOrEmpty(image.ToolTip))
				image.ToolTip = itemToolTip;
		}
		public static void MakeCssRuleImportant(ref string key, ref string value) {
			if(key == "background-image" || key == "list-style-image") {
				key = " " + key;
				value = string.Format("url({0})", value);
			}
			value += "!important";
		}
		public static void MakeCssAttributesImportant(CssStyleCollection attributes, IUrlResolutionService urlResolver) {
			Dictionary<string, string> collection = new Dictionary<string, string>();
			foreach(string key in attributes.Keys)
				collection.Add(key, attributes[key]);
			attributes.Clear();
			foreach(string key in collection.Keys) {
				string ruleName = key;
				string ruleValue = collection[key];
				MakeCssRuleImportant(ref ruleName, ref ruleValue);
				attributes.Add(ruleName, ruleValue);
			}
		}
		public static void SetDisabledAttribute(WebControl control, AppearanceStyleBase disabledStyle) {
			if(((control.Page != null) && (control.Page.Site != null) && control.Page.Site.DesignMode) || disabledStyle == null)
				control.Enabled = false;
			else {
				AppearanceStyle style = new AppearanceStyle();
				style.CopyFrom(control.ControlStyle);
				style.CopyFrom(disabledStyle);
				style.AssignToControl(control);
			}
		}
		public static void SetPreventSelectionAttribute(WebControl control) {
			if(Browser.Family.IsNetscape)
				control.Style.Add("-moz-user-select", "none");
			else if(Browser.Family.IsWebKit)
				control.Style.Add("-khtml-user-select", "none");
		}
		public static void SetOpacity(CssStyleCollection attributes, int opacity) {
			if(0 <= opacity && opacity <= 100) {
				if(Browser.IsIE && Browser.MajorVersion < 9)
					attributes.Add("filter",
						"progid:DXImageTransform.Microsoft.Alpha(Style=0, Opacity=" + opacity.ToString() + ")");
				else
					attributes.Add("opacity", ((float)opacity / 100).ToString(CultureInfo.InvariantCulture));
			}
		}
		public static void SetOpacity(WebControl control, int opacity) {
			SetOpacity(control.Style, opacity);
		}
		public static void SetPadding(WebControl control, Unit padding) {
			SetStyleUnitAttribute(control, "padding", padding);
		}
		public static void SetVerticalPaddings(WebControl control, Unit paddingTop, Unit paddingBottom) {
			SetStyleUnitAttribute(control, "padding-top", paddingTop);
			SetStyleUnitAttribute(control, "padding-bottom", paddingBottom);
		}
		public static void SetVerticalPaddings(WebControl control, Unit padding) {
			SetVerticalPaddings(control, padding, padding);
		}
		public static void SetHorizontalPaddings(WebControl control, Unit paddingLeft, Unit paddingRight) {
			SetStyleUnitAttribute(control, "padding-left", paddingLeft);
			SetStyleUnitAttribute(control, "padding-right", paddingRight);
		}
		public static void SetHorizontalPaddings(WebControl control, Unit padding) {
			SetHorizontalPaddings(control, padding, padding);
		}
		public static void SetPaddings(WebControl control, Unit paddingLeft, Unit paddingTop,
			Unit paddingRight, Unit paddingBottom) {
			SetHorizontalPaddings(control, paddingLeft, paddingRight);
			SetVerticalPaddings(control, paddingTop, paddingBottom);
		}
		public static void SetPaddings(WebControl control, Paddings paddings) {
			SetPaddings(control, paddings.GetPaddingLeft(), paddings.GetPaddingTop(),
				paddings.GetPaddingRight(), paddings.GetPaddingBottom());
		}
		public static void ClearPaddings(WebControl control) {
			AppendDefaultDXClassName(control, "dx-noPadding");
		}
		public static void SetBorderBox(WebControl control) {
			AppendDefaultDXClassName(control, "dx-borderBox");
		}
		public static void AppendFullTransparentCssClass(WebControl control) {
			AppendDefaultDXClassName(control, "dx-ft");
		}
		public static void AlignBlockLevelElement(WebControl control, HorizontalAlign hAlign) {
			if(hAlign == HorizontalAlign.NotSet || hAlign == HorizontalAlign.Justify)
				return;
			else if(hAlign == HorizontalAlign.Center) {
				SetStyleAttribute(control, "margin-left", "auto", "");
				SetStyleAttribute(control, "margin-right", "auto", "");
			} else {
				string shiftedTo = hAlign == HorizontalAlign.Left ? "left" : "right";
				SetStyleAttribute(control, "float", shiftedTo, "");
			}
		}
		public static void SetHorizontalAlign(WebControl control, HorizontalAlign align) {
			if(align != HorizontalAlign.NotSet)
				SetStyleStringAttribute(control, "text-align", align.ToString().ToLower());
		}
		public static void SetVerticalAlign(WebControl control, VerticalAlign align) {
			if(align != VerticalAlign.NotSet)
				SetStyleStringAttribute(control, "vertical-align", align.ToString().ToLower());
		}
		public static void SetVerticalAlignClass(WebControl control, VerticalAlign align) {
			SetVerticalAlignClass(control, align, VerticalAlign.Middle);
		}
		public static void SetVerticalAlignClass(WebControl control, VerticalAlign align, VerticalAlign defaultAlign) {
			if(align == VerticalAlign.NotSet)
				align = VerticalAlign.Middle;
			string className = string.Empty;
			if(align == VerticalAlign.Middle)
				className = "dx-vam";
			else if(align == VerticalAlign.Top)
				className = "dx-vat";
			else if(align == VerticalAlign.Bottom)
				className = "dx-vab";
			if(!string.IsNullOrEmpty(className))
				RenderUtils.AppendDefaultDXClassName(control, className);
		}
		public static void SetMargins(WebControl control, Unit marginLeft, Unit marginTop,
			Unit marginRight, Unit marginBottom) {
			SetHorizontalMargins(control, marginLeft, marginRight);
			SetVerticalMargins(control, marginTop, marginBottom);
		}
		public static void SetMargins(WebControl control, Margins margins) {
			SetMargins(control, margins.GetMarginLeft(), margins.GetMarginTop(),
				margins.GetMarginRight(), margins.GetMarginBottom());
		}
		public static void SetMargins(WebControl control, Paddings margins) {
			SetMargins(control, margins.GetPaddingLeft(), margins.GetPaddingTop(),
				margins.GetPaddingRight(), margins.GetPaddingBottom());
		}
		public static void SetHorizontalMargins(WebControl control, Unit marginLeft, Unit marginRight) {
			SetStyleUnitAttribute(control, "margin-left", marginLeft);
			SetStyleUnitAttribute(control, "margin-right", marginRight);
		}
		public static void SetVerticalMargins(WebControl control, Unit marginTop, Unit marginBottom) {
			SetStyleUnitAttribute(control, "margin-top", marginTop);
			SetStyleUnitAttribute(control, "margin-bottom", marginBottom);
		}
		public static void SetCursor(WebControl control, string value) {
			SetStyleStringAttribute(control, "cursor", value);
		}
		public static void SetLineHeight(WebControl control, Unit lineHeight) {
			SetStyleUnitAttribute(control, "line-height", lineHeight);
		}
		private static string GetDisplayStyleAttrValue(WebControl control) {
			string display = control.Attributes.CssStyle[HtmlTextWriterStyle.Display];
			if(string.IsNullOrEmpty(display))
				display = control.Attributes.CssStyle["display"];
			return display;
		}
		private static string GetVisibilityStyleAttrValue(WebControl control) {
			string visibility = control.Attributes.CssStyle[HtmlTextWriterStyle.Visibility];
			if(string.IsNullOrEmpty(visibility))
				visibility = control.Attributes.CssStyle["visibility"];
			return visibility;
		}
		public static void SetVisibility(WebControl control, bool visible, bool useDisplayAttribute) {
			SetVisibility(control, visible, useDisplayAttribute, false);
		}
		public static void SetVisibility(WebControl control, bool visible, bool useDisplayAttribute, bool useVisibilityAttribute) {
			if(!useDisplayAttribute && !useVisibilityAttribute)
				control.Visible = visible;
			else {
				if(useDisplayAttribute) {
					if(visible) {
						string displayAttrValue = GetDisplayStyleAttrValue(control);
						if(displayAttrValue == null || displayAttrValue.ToLowerInvariant() == "none")
							displayAttrValue = "";
						SetStyleStringAttribute(control, "display", displayAttrValue);
					} else
						SetStyleStringAttribute(control, "display", "none");
				}
				if(useVisibilityAttribute) {
					if(visible) {
						string visibilityAttrValue = GetVisibilityStyleAttrValue(control);
						if(visibilityAttrValue == null || visibilityAttrValue.ToLowerInvariant() == "hidden")
							visibilityAttrValue = "";
						SetStyleStringAttribute(control, "visibility", visibilityAttrValue);
					} else
						SetStyleStringAttribute(control, "visibility", "hidden");
				}
			}
		}
		public static void SetTextDecoration(WebControl control, string textDecoration) {
			if(string.IsNullOrEmpty(textDecoration))
				return;
			if(textDecoration.IndexOf("none") != -1)
				control.Style.Add(HtmlTextWriterStyle.TextDecoration, "none");
			else if(textDecoration.IndexOf("underline") != -1)
				control.Style.Add(HtmlTextWriterStyle.TextDecoration, "underline");
		}
		public static void ResetWrap(WebControl control) {
			RenderUtils.SetStyleStringAttribute(control, "white-space", string.Empty);
			RenderUtils.RemoveDefaultDXClassName(control, "dx-wrap");
			RenderUtils.RemoveDefaultDXClassName(control, "dx-nowrap");
		}
		public static void SetWrap(WebControl control, DefaultBoolean wrap) {
			SetWrap(control, wrap, false);
		}
		public static void SetWrap(WebControl control, DefaultBoolean wrap, bool designMode) {
			if(wrap == DefaultBoolean.Default)
				ResetWrap(control);
			else {
				string cssClass = wrap == DefaultBoolean.True ? "dx-wrap" : "dx-nowrap";
				AppendDefaultDXClassName(control, cssClass);
				if(designMode && wrap == DefaultBoolean.True)
					RenderUtils.SetStyleStringAttribute(control, "display", "inline", true);
			}
		}
		public static string GetWrapStyleValue(DefaultBoolean wrap) {
			if(wrap == DefaultBoolean.False)
				return "nowrap";
			else if(wrap == DefaultBoolean.True)
				return "normal";
			else
				return string.Empty;
		}
		public static void AllowEllipsisInText(WebControl control, bool value = true) {
			var action = value ? (Action<WebControl, string>) AppendDefaultDXClassName : RemoveDefaultDXClassName;
			action(control, "dx-ellipsis");
		}
		public static void CollapseAndRemovePadding(Table table) {
			table.CellPadding = 0;
			SetStyleStringAttribute(table, "border-collapse", "collapse");
		}
		public static string CombineCssClasses(params string[] cssClasses) {
			HybridDictionary map = new HybridDictionary();
			int length = 0;
			foreach(string item in cssClasses) {
				if(string.IsNullOrEmpty(item))
					continue;
				string[] parts = item.Split(' ');
				foreach(string part in parts) {
					if(part.Length < 1 || map.Contains(part))
						continue;
					if(length > 0)
						length++;
					length += part.Length;
					map.Add(part, true);
				}
			}
			StringBuilder builder = new StringBuilder(length);
			foreach(string item in map.Keys) {
				if(builder.Length > 0)
					builder.Append(' ');
				builder.Append(item);
			}
			return builder.ToString();
		}
		public static void AppendDefaultDXClassName(WebControl control, string dxStyleName) {
			if(string.IsNullOrEmpty(dxStyleName))
				return;
			control.CssClass = CombineCssClasses(control.CssClass, dxStyleName);
		}
		public static void AppendCssClass(Style style, string cssClass) {
			if(!string.IsNullOrEmpty(cssClass))
				style.CssClass = CombineCssClasses(style.CssClass, cssClass);
		}
		public static void RemoveDefaultDXClassName(WebControl control, string dxStyleName) {
			if(string.IsNullOrEmpty(dxStyleName))
				return;
			control.CssClass = control.CssClass.Replace(" " + dxStyleName, string.Empty);
			control.CssClass = control.CssClass.Replace(dxStyleName, string.Empty);
		}
		public static void AssignStyles(WebControl source, WebControl destination, string styleName, string defaultValue) {
			if(source == null || destination == null || string.IsNullOrEmpty(styleName))
				return;
			string newValue = source.Style[styleName] ?? defaultValue;
			SetStyleAttribute(destination, styleName, newValue, defaultValue);
		}
		public static void AssignStyles(WebControl source, WebControl destination, string[] styleNames, string defaultValue) {
			foreach(string styleName in styleNames)
				AssignStyles(source, destination, styleName, defaultValue);
		}
		public static void AppendMSTouchDraggableClassNameIfRequired(WebControl control) {
			if(Browser.Platform.IsMSTouchUI)
				AppendDefaultDXClassName(control, MSTouchDraggableClassName);
		}
		public static void AssignAttributes(WebControl source, WebControl destination) {
			AssignAttributes(source, destination, false, false, false, false);
		}
		public static void AssignAttributes(WebControl source, WebControl destination, bool skipID) {
			AssignAttributes(source, destination, skipID, false, false, false);
		}
		public static void AssignAttributes(WebControl source, WebControl destination, bool skipID, bool inlineBlock) {
			AssignAttributes(source, destination, skipID, inlineBlock, false, false);
		}
		public static void AssignAttributes(WebControl source, WebControl destination, bool skipID, bool inlineBlock, bool skipSizes) {
			AssignAttributes(source, destination, skipID, inlineBlock, skipSizes, false);
		}
		public static void AssignAttributes(WebControl source, WebControl destination, bool skipID, bool inlineBlock, bool skipSizes, bool skipToolTip) {
			if(!skipSizes) {
				destination.Width = source.Width;
				destination.Height = source.Height;
			}
			destination.AccessKey = source.AccessKey;
			destination.TabIndex = source.TabIndex;
			if(!skipToolTip)
				destination.ToolTip = source.ToolTip;
			foreach(string key in source.Attributes.Keys) {
				if(key == "style")
					continue;
				destination.Attributes.Add(key, source.Attributes[key]);
			}
			if(string.IsNullOrEmpty(destination.Style.Value))
				destination.Style.Value = source.Style.Value;
			else
				foreach(string key in source.Style.Keys) {
					string val = source.Style[key];
					if(destination.Style[key] != val)
						destination.Style.Add(key, val);
				}
			if(!skipID) {
				if(source.Attributes["id"] != null)
					destination.Attributes.Add("id", source.Attributes["id"]);
				else {
					string id = source.ClientID;
					if(id != "") {
						destination.ID = null;
						destination.Attributes.Add("id", id);
					}
				}
			}
			if(inlineBlock && Browser.IsIE &&
				(source.BorderStyle != BorderStyle.NotSet || !source.BorderWidth.IsEmpty || !source.Height.IsEmpty || !source.Width.IsEmpty))
				RenderUtils.SetStyleStringAttribute(destination, "display", "inline-block");
		}
		public static void MoveTabIndexToInput(WebControl source, WebControl input) {
			input.TabIndex = source.TabIndex;
			source.TabIndex = 0;
		}
		public static string CreateClientEventHandler(string content) {
			StringBuilder stb = new StringBuilder("function (s, e) {");
			stb.Append(content);
			stb.Append(";}");
			return stb.ToString();
		}
		public static string GetCallbackEventReference(Page page, Control control, string argument, string clientCallback, string context) {
			if(page != null)
				return page.ClientScript.GetCallbackEventReference(control, argument, clientCallback, context, true);
			return "";
		}
		public static string GetCallbackEventReference(Page page, Control control, string argument, string clientCallback, string context, string clientErrorCallback) {
			if(page != null)
				return page.ClientScript.GetCallbackEventReference(control, argument, clientCallback, context, clientErrorCallback, true);
			return "";
		}
		public static string GetPostBackEventReference(Page page, Control control, string argument) {
			if(page != null)
				return page.ClientScript.GetPostBackEventReference(control, argument);
			return "";
		}
		public static string GetPostBackEventReference(Page page, PostBackOptions options) {
			return GetPostBackEventReference(page, options, false);
		}
		public static string GetPostBackEventReference(Page page, PostBackOptions options, bool registerForEventValidation) {
			if(page != null)
				return page.ClientScript.GetPostBackEventReference(options, registerForEventValidation);
			return "";
		}
		public static void RegisterRequiresControlState(Page page, Control control) {
			if(page != null)
				page.RegisterRequiresControlState(control);
		}
		public static void RegisterRequiresPostBack(Page page, Control control) {
			if(page != null)
				page.RegisterRequiresPostBack(control);
		}
		public static void EnsureChildControlsRecursive(Control control, bool skipContentContainers) {
			EnsureChildControlsRecursive(control, delegate(Control childControl) {
				return skipContentContainers && (childControl is IContentContainer || childControl is TemplateContainerBase);
			});
		}
		public static void EnsureChildControlsRecursive(Control control, Predicate<Control> skipControlCondition) {
			IASPxWebControl iaspxWebControl = control as IASPxWebControl;
			if(iaspxWebControl != null)
				iaspxWebControl.EnsureChildControls();
			if(control.HasControls()) {
				ControlCollection controls = control.Controls;
				for(int i = 0; i < controls.Count; i++) {
					Control childControl = controls[i];
					if(skipControlCondition(childControl))
						continue;
					EnsureChildControlsRecursive(childControl, skipControlCondition);
				}
			}
		}
		internal static void EnsurePrepareChildControlsRecursive(Control control, bool skipContentContainers) {
			IASPxWebControl iaspxWebControl = control as IASPxWebControl;
			if(iaspxWebControl != null)
				iaspxWebControl.PrepareControlHierarchy();
			if(control.HasControls()) {
				ControlCollection controls = control.Controls;
				for(int i = 0; i < controls.Count; i++) {
					Control childControl = controls[i];
					if(skipContentContainers) {
						if(childControl is IContentContainer)
							continue;
						if(childControl is TemplateContainerBase)
							continue;
					}
					EnsurePrepareChildControlsRecursive(childControl, skipContentContainers);
				}
			}
		}
		public static void LoadPostDataRecursive(Control parent, NameValueCollection postCollection) {
			LoadPostDataRecursive(parent, postCollection, false);
		}
		public static void LoadPostDataRecursive(Control parent, NameValueCollection postCollection, bool force) {
			LoadPostDataRecursive(parent, postCollection, force, null);
		}
		public static void LoadPostDataRecursive(Control parent, NameValueCollection postCollection, bool force, Func<Control, bool> skipLoadMethod) {
			if(parent is IStopLoadPostDataOnCallbackMarker)
				return;
			foreach(Control control in parent.Controls) {
				if(control is CheckBoxList || control is RadioButtonList) {
					LoadPostDataSpecial(control, postCollection);
					continue;
				}
				string name = control.UniqueID;
				if((skipLoadMethod == null || !skipLoadMethod(control)) && ((postCollection[name] != null) || (control is IRequiresLoadPostDataControl))) {
					IPostBackDataHandlerEx postbackHandlerEx = control as IPostBackDataHandlerEx;
					IPostBackDataHandler postbackHandler = control as IPostBackDataHandler;
					if(force && postbackHandlerEx != null) {
						postbackHandlerEx.ForceLoadPostData(name, postCollection);
					} else {
						if(postbackHandler != null)
							postbackHandler.LoadPostData(name, postCollection);
					}
				}
				if(control.HasControls())
					LoadPostDataRecursive(control, postCollection, force, skipLoadMethod);
			}
		}
		static void LoadPostDataSpecial(Control control, NameValueCollection postCollection) {
			IPostBackDataHandler postbackHandler = control as IPostBackDataHandler;
			if(postbackHandler == null)
				return;
			string uniqueId = control.UniqueID;
			foreach(string key in postCollection.AllKeys) {
				if(key.StartsWith(uniqueId))
					postbackHandler.LoadPostData(key, postCollection);
			}
		}
		public static string GetBackToTopFunctionReference(string anchorName, double offsetToTop) {
			return string.Format("backtotop(\"{0}\",{1});", anchorName, offsetToTop.ToString());
		}
		public static string GetBackToTopScript() {
			return "var __dxLastScrollY = -1;\n" +
				"function backtotop(anchorStr, offsetY){\n" +
				"  scrollY = (document.documentElement.scrollTop || document.body.scrollTop);\n" +
				"  if (__dxLastScrollY != scrollY){\n" +
				"    location.href=anchorStr;\n" +
				"    scrollX = (document.documentElement.scrollLeft || document.body.scrollLeft);\n" +
				"    scrollY = (document.documentElement.scrollTop || document.body.scrollTop);\n" +
				"    __dxLastScrollY = scrollY - offsetY;\n" +
				"    window.scrollTo(scrollX, __dxLastScrollY);\n" +
				"  }\n" +
				"}\n";
		}
		public static string GetClientDateFormatInfoScript() {
			IDictionary diff = CreateCultureInfoDiff(CultureInfo.CurrentCulture);
			if(diff.Count < 1)
				return string.Empty;
			StringBuilder script = new StringBuilder();
			script.Append("(function(){");
			script.AppendFormat("var a = {0};", HtmlConvertor.ToJSON(diff));
			script.Append("for(var b in a) ASPx.CultureInfo[b] = a[b];");
			script.Append("})();");
			return script.ToString();
		}
		static IDictionary defaultCultureInfoBag = null;
		static IDictionary DefaultCultureInfoBag {
			get {
				if(defaultCultureInfoBag == null)
					defaultCultureInfoBag = CreateDefaultCultureInfoBag();
				return defaultCultureInfoBag;
			}
		}
		static IDictionary CreateDefaultCultureInfoBag() {
			var bag = new Hashtable();
			bag.Add("twoDigitYearMax", 2029);
			bag.Add("ts", ":");
			bag.Add("ds", "/");
			bag.Add("am", "AM");
			bag.Add("pm", "PM");
			bag.Add("monthNames", new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "" }); 
			bag.Add("genMonthNames", new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December", "" }); 
			bag.Add("abbrMonthNames", new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec", "" }); 
			bag.Add("abbrDayNames", new string[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" });
			bag.Add("dayNames", new string[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" });
			bag.Add("numDecimalPoint", ".");
			bag.Add("numPrec", 2);
			bag.Add("numGroupSeparator", ",");
			bag.Add("numGroups", new int[] { 3 });
			bag.Add("numNegPattern", 1);
			bag.Add("numPosInf", "Infinity");
			bag.Add("numNegInf", "-Infinity");
			bag.Add("numNan", "NaN");
			bag.Add("currency", "$");
			bag.Add("currDecimalPoint", ".");
			bag.Add("currPrec", 2);
			bag.Add("currGroupSeparator", ",");
			bag.Add("currGroups", new int[] { 3 });
			bag.Add("currPosPattern", 0);
			bag.Add("currNegPattern", 0);
			bag.Add("percentPattern", 0);
			bag.Add("shortTime", "h:mm tt");
			bag.Add("longTime", "h:mm:ss tt");
			bag.Add("shortDate", "M/d/yyyy");
			bag.Add("longDate", "dddd, MMMM d, yyyy");
			bag.Add("monthDay", "MMMM d");
			bag.Add("yearMonth", "MMMM yyyy");
			return bag;
		}
		static IDictionary CreateCultureInfoDiff(CultureInfo culture) {
			var bag = new Hashtable();
			var userDateTime = culture.DateTimeFormat;
			var userNumbers = culture.NumberFormat;
			if(userDateTime.Calendar.TwoDigitYearMax != (int)DefaultCultureInfoBag["twoDigitYearMax"])
				bag.Add("twoDigitYearMax", userDateTime.Calendar.TwoDigitYearMax);
			if(userDateTime.TimeSeparator != DefaultCultureInfoBag["ts"].ToString())
				bag.Add("ts", userDateTime.TimeSeparator);
			if(userDateTime.DateSeparator != DefaultCultureInfoBag["ds"].ToString())
				bag.Add("ds", userDateTime.DateSeparator);
			if(userDateTime.AMDesignator != DefaultCultureInfoBag["am"].ToString())
				bag.Add("am", userDateTime.AMDesignator);
			if(userDateTime.PMDesignator != DefaultCultureInfoBag["pm"].ToString())
				bag.Add("pm", userDateTime.PMDesignator);
			if(!AreStringArraysEqual(userDateTime.MonthNames, (string[])DefaultCultureInfoBag["monthNames"]))
				bag.Add("monthNames", userDateTime.MonthNames);
			if(!AreStringArraysEqual(userDateTime.MonthGenitiveNames, (string[])DefaultCultureInfoBag["genMonthNames"]))
				bag.Add("genMonthNames", userDateTime.MonthGenitiveNames);
			if(!AreStringArraysEqual(userDateTime.AbbreviatedMonthGenitiveNames, (string[])DefaultCultureInfoBag["abbrMonthNames"]))
				bag.Add("abbrMonthNames", userDateTime.AbbreviatedMonthNames);
			if(!AreStringArraysEqual(userDateTime.AbbreviatedDayNames, (string[])DefaultCultureInfoBag["abbrDayNames"]))
				bag.Add("abbrDayNames", userDateTime.AbbreviatedDayNames);
			if(!AreStringArraysEqual(userDateTime.DayNames, (string[])DefaultCultureInfoBag["dayNames"]))
				bag.Add("dayNames", userDateTime.DayNames);
			if(userNumbers.NumberDecimalSeparator != DefaultCultureInfoBag["numDecimalPoint"].ToString())
				bag.Add("numDecimalPoint", userNumbers.NumberDecimalSeparator);
			if(userNumbers.NumberDecimalDigits != (int)DefaultCultureInfoBag["numPrec"])
				bag.Add("numPrec", userNumbers.NumberDecimalDigits);
			if(userNumbers.NumberGroupSeparator != DefaultCultureInfoBag["numGroupSeparator"].ToString())
				bag.Add("numGroupSeparator", userNumbers.NumberGroupSeparator);
			if(userNumbers.NumberGroupSizes.Length > 1)
				bag.Add("numGroups", userNumbers.NumberGroupSizes);
			if(userNumbers.NumberNegativePattern != (int)DefaultCultureInfoBag["numNegPattern"])
				bag.Add("numNegPattern", userNumbers.NumberNegativePattern);
			if(userNumbers.PositiveInfinitySymbol != DefaultCultureInfoBag["numPosInf"].ToString())
				bag.Add("numPosInf", userNumbers.PositiveInfinitySymbol);
			if(userNumbers.NegativeInfinitySymbol != DefaultCultureInfoBag["numNegInf"].ToString())
				bag.Add("numNegInf", userNumbers.NegativeInfinitySymbol);
			if(userNumbers.NaNSymbol != DefaultCultureInfoBag["numNan"].ToString())
				bag.Add("numNan", userNumbers.NaNSymbol);
			if(userNumbers.CurrencySymbol != DefaultCultureInfoBag["currency"].ToString())
				bag.Add("currency", userNumbers.CurrencySymbol);
			if(userNumbers.CurrencyDecimalSeparator != DefaultCultureInfoBag["currDecimalPoint"].ToString())
				bag.Add("currDecimalPoint", userNumbers.CurrencyDecimalSeparator);
			if(userNumbers.CurrencyDecimalDigits != (int)DefaultCultureInfoBag["currPrec"])
				bag.Add("currPrec", userNumbers.CurrencyDecimalDigits);
			if(userNumbers.CurrencyGroupSeparator != DefaultCultureInfoBag["currGroupSeparator"].ToString())
				bag.Add("currGroupSeparator", userNumbers.CurrencyGroupSeparator);
			if(userNumbers.CurrencyGroupSizes.Length > 1)
				bag.Add("currGroups", userNumbers.CurrencyGroupSizes);
			if(userNumbers.CurrencyPositivePattern != (int)DefaultCultureInfoBag["currPosPattern"])
				bag.Add("currPosPattern", userNumbers.CurrencyPositivePattern);
			if(userNumbers.CurrencyNegativePattern != (int)DefaultCultureInfoBag["currNegPattern"])
				bag.Add("currNegPattern", userNumbers.CurrencyNegativePattern);
			if(userNumbers.PercentPositivePattern != (int)DefaultCultureInfoBag["percentPattern"])
				bag.Add("percentPattern", userNumbers.PercentPositivePattern);
			if(userDateTime.ShortTimePattern != DefaultCultureInfoBag["shortTime"].ToString())
				bag.Add("shortTime", userDateTime.ShortTimePattern);
			if(userDateTime.LongTimePattern != DefaultCultureInfoBag["longTime"].ToString())
				bag.Add("longTime", userDateTime.LongTimePattern);
			if(userDateTime.ShortDatePattern != DefaultCultureInfoBag["shortDate"].ToString())
				bag.Add("shortDate", userDateTime.ShortDatePattern);
			if(userDateTime.LongDatePattern != DefaultCultureInfoBag["longDate"].ToString())
				bag.Add("longDate", userDateTime.LongDatePattern);
			if(userDateTime.MonthDayPattern != DefaultCultureInfoBag["monthDay"].ToString())
				bag.Add("monthDay", userDateTime.MonthDayPattern);
			if(userDateTime.YearMonthPattern != DefaultCultureInfoBag["yearMonth"].ToString())
				bag.Add("yearMonth", userDateTime.YearMonthPattern);
			return bag;
		}
		public static bool AreStringArraysEqual(string[] a, string[] b) {
			if(a.Length != b.Length)
				return false;
			for(int i = 0; i < a.Length; i++) {
				if(a[i] != b[i])
					return false;
			}
			return true;
		}
		public static string WrapCallWithSetTimeout(string jsCall) {
			return WrapCallWithSetTimeout(jsCall, false);
		}
		public static string WrapCallWithSetTimeout(string jsCall, bool javaSriptProtocolRequired) {
			string result = javaSriptProtocolRequired ? "javascript:" : "";
			return result + "setTimeout('" + jsCall + "', 0)";
		}
		private static Regex singleQuoteReplacementRegex;
		private static Regex SingleQuoteReplacementRegex {
			get {
				if(singleQuoteReplacementRegex == null)
					singleQuoteReplacementRegex = new Regex("(?<!\\\\)'", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
				return singleQuoteReplacementRegex;
			}
		}
		protected static string GetPostBackEventReference(Control targetControl, string postBackArg, bool causesValidation,
			string validationGroup, string actionUrl, bool clientSubmit) {
			PostBackOptions options = new PostBackOptions(targetControl, postBackArg);
			options.ClientSubmit = false;
			if(targetControl.Page != null) {
				options.PerformValidation = causesValidation;
				options.ValidationGroup = validationGroup;
				options.ActionUrl = actionUrl;
				options.ClientSubmit = clientSubmit;
			}
			string ret = GetPostBackEventReference(targetControl.Page, options);
			if(!string.IsNullOrEmpty(ret)) {
				ret = SingleQuoteReplacementRegex.Replace(ret, "\"");
				return ret;
			}
			return string.Empty;
		}
		public static string GetPostBackEventReference(Control targetControl, string postBackArg, bool causesValidation,
			string validationGroup, string actionUrl, bool clientSubmit,
			bool replaceArgument, bool wrapWithSetTimeout, bool wrapWithAnonymFunc) {
			if(!replaceArgument) {
				string ret = GetPostBackEventReference(targetControl, postBackArg, causesValidation, validationGroup, actionUrl, clientSubmit);
				if(!string.IsNullOrEmpty(ret)) {
					if(wrapWithSetTimeout)
						ret = RenderUtils.WrapCallWithSetTimeout(ret);
					return wrapWithAnonymFunc ? ("function() { " + ret + "; }") : ret;
				}
			} else {
				string ret = GetPostBackEventReference(targetControl, DummyPostBackArgument, causesValidation, validationGroup, actionUrl, clientSubmit);
				if(!string.IsNullOrEmpty(ret)) {
					string replaceArg = wrapWithSetTimeout ? "\"' + postBackArg + '\"" : " postBackArg";
					ret = ret.Replace("\"" + DummyPostBackArgument + "\"", replaceArg).Replace("'" + DummyPostBackArgument + "'", replaceArg);
					if(wrapWithSetTimeout)
						ret = RenderUtils.WrapCallWithSetTimeout(ret);
					return "function(postBackArg) { " + ret + "; }";
				}
			}
			return string.Empty;
		}
		public static string GetLinkHtml(string href) {
			return String.Format("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />", href);
		}
		public static string GetStyleImportHtml(string href) {
			return String.Format("<style>\n  @import url('{0}');\n</style>", href);
		}
		public static string GetScriptHtml(string script) {
			if(string.IsNullOrEmpty(script))
				return "";
			string id = StartupScriptIDPrefix + Math.Abs(script.GetHashCode()).ToString();
			return String.Format("<script id=\"{0}\" type=\"text/javascript\">\n<!--\n{1}\n//-->\n</script>", id, script);
		}
		public static string GetIncludeScriptHtml(string url) {
			string id = IncludedScriptIDPrefix + Math.Abs(url.GetHashCode()).ToString();
			return GetIncludeScriptHtml(url, id);
		}
		public static string GetIncludeScriptHtml(string url, string id) {
			return String.Format("<script id=\"{0}\" src=\"{1}\" type=\"text/javascript\"></script>", id, HttpUtility.HtmlEncode(url));
		}
		public static string GetHiddenFieldHtml(string name, string value) {
			return string.Format("<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\" />", name, HttpUtility.HtmlAttributeEncode(value));
		}
		public static void WriteScriptHtml(HtmlTextWriter writer, string script) {
			writer.Write(GetScriptHtml(script));
		}
		public static string GetCookie(HttpRequest request, string name) {
			if(request != null) {
				string cookieName = HttpUtility.UrlEncodeUnicode(name);
				HttpCookie cookie = request.Cookies[cookieName.Replace("%u00", "%")]; 
				if(cookie == null)
					cookie = request.Cookies[cookieName];
				if(cookie != null)
					return HttpUtility.UrlDecode(cookie.Value);
				return string.Empty;
			}
			return "";
		}
		public static void SetCookie(HttpRequest request, HttpResponse response, string name, string value) {
			if(response != null) {
				try {
					string cookieName = HttpUtility.UrlEncodeUnicode(name);
					HttpCookie cookie = response.Cookies[cookieName.Replace("%u00", "%")]; 
					if(cookie != null)
						response.Cookies.Remove(cookieName.Replace("%u00", "%"));
					cookie = response.Cookies[cookieName];
					if(cookie != null)
						response.Cookies.Remove(cookieName);
					cookie = new HttpCookie(cookieName, HttpUtility.UrlEncode(value));
					cookie.Expires = DateTime.Now.AddYears(1);
					cookie.Path = "/";
					response.Cookies.Add(cookie);
				} catch {
				}
			}
		}
		public static string GetSessionValue(Page page, string name) {
			return (page != null && page.Session != null && page.Session[name] != null) ? page.Session[name].ToString() : "";
		}
		public static void SetSessionValue(Page page, string name, string value) {
			if(page != null && page.Session != null)
				page.Session[name] = value;
		}
		public static ISkinOwner FindParentSkinOwner(Control control) {
			if(control == null)
				return null;
			control = control.Parent;
			while(control != null) {
				if(control is IParentSkinOwner)
					return (ISkinOwner)control;
				control = control.Parent;
			}
			return null;
		}
		public static string GetReferentControlClientID(Control referer, string referentControlID, ControlResolveEventInitiator controlResolveEventInitiator) {
			Control referentControl = GetReferentControl(referer, referentControlID, controlResolveEventInitiator);
			return referentControl != null ? referentControl.ClientID : referentControlID;
		}
		public static Control GetReferentControl(Control referer, string referentControlID, ControlResolveEventInitiator controlResolveEventInitiator) {
			Control referentControl = null;
			if(controlResolveEventInitiator != null) {
				ControlResolveEventArgs args = new ControlResolveEventArgs(referentControlID);
				controlResolveEventInitiator(args);
				referentControl = args.ResolvedControl;
			}
			if(referentControl == null) {
				Control searchContainer = referer.NamingContainer;
				while(searchContainer != null) {
					referentControl = searchContainer.FindControl(referentControlID);
					if(referentControl != null)
						break;
					searchContainer = searchContainer.NamingContainer;
				}
			}
			if(referentControl == null)
				referentControl = FindControlRecursive(referer, referentControlID);
			return referentControl;
		}
		private static Control FindControlRecursive(Control sourceControl, string requiredControlID) {
			if(sourceControl != null) {
				foreach(Control currentControl in sourceControl.Controls) {
					if(currentControl != null) {
						Control requiredControl = currentControl.FindControl(requiredControlID) ??
							FindControlRecursive(currentControl, requiredControlID);
						if(requiredControl != null && requiredControl.ID == requiredControlID)
							return requiredControl;
					}
				}
			}
			return null;
		}
		public static string CheckEmptyRenderText(string text) {
			if(string.IsNullOrEmpty(text.Replace(" ", "")))
				return "&nbsp;";
			return text;
		}
		internal static void EnsureIECompatibilityMeta() {
			if(HttpContext.Current == null)
				return; 
			var response = HttpUtils.GetResponse();
			var handler = HttpContext.Current.CurrentHandler;
			var isWebFormPage = handler is Page;
			var isMvcPage = handler != null && handler.GetType().FullName == "System.Web.Mvc.MvcHandler"; 
			var isPageRequest = isWebFormPage || isMvcPage;
			if(response == null || !isPageRequest || !Browser.IsIECompatibilityMode)
				return;
			response.AddHeader(IECompatibilityMetaKey, string.Format(IECompatibilityMetaValueFormat, Browser.GetIECompatibilityHeader()));
		}
		public static HtmlHead FindHead(Control parent) {
			if(parent == null)
				return null;
			foreach(Control child in parent.Controls) {
				HtmlHead head = child as HtmlHead;
				if(head == null)
					head = FindHead(child);
				if(head != null)
					return head;
			}
			return null;
		}
		const string Nbsp = "&nbsp;";
		const string SpaceNbsp = "&nbsp; ";
		public static string ProtectTextWhitespaces(string text) {
			if(!string.IsNullOrEmpty(text)) {
				if(text[0] == ' ')
					text = Nbsp + text.Substring(1);
				if(text[text.Length - 1] == ' ')
					text = text.Substring(0, text.Length - 1) + Nbsp;
				text = text.Replace("  ", SpaceNbsp);
			}
			return text;
		}
		public static bool HasHorzScroll(ScrollBars scrollBars) {
			return scrollBars == ScrollBars.Horizontal || scrollBars == ScrollBars.Both || scrollBars == ScrollBars.Auto;
		}
		public static bool HasVertScroll(ScrollBars scrollBars) {
			return scrollBars == ScrollBars.Vertical || scrollBars == ScrollBars.Both || scrollBars == ScrollBars.Auto;
		}
		public static bool IsHtml5Mode(Control control) {
			if(control != null && control.Page != null && control.Page.Site != null)
				return false;
			if(HttpContext.Current == null)
				return true;
			return ConfigurationSettings.DoctypeMode == DoctypeMode.Html5;
		}
		public static void ReplaceAlignAttributes(WebControl control) {
			if(!string.IsNullOrEmpty(control.Attributes["align"])) {
				SetHorizontalAlignCssAttributes(control, control.Attributes["align"]);
				control.Attributes.Remove("align");
			}
			if(!string.IsNullOrEmpty(control.Attributes["valign"])) {
				control.Style.Add("vertical-align", control.Attributes["valign"]);
				control.Attributes.Remove("valign");
			}
		}
		public static void SetHorizontalAlignClass(WebControl control, HorizontalAlign align) {
			SetHorizontalAlignCssAttributes(control, align.ToString());
		}
		public static void SetHorizontalAlignCssAttributes(WebControl control, string align) {
			switch(align.ToLower()) {
				case "left":
					AppendDefaultDXClassName(control, "dx-al");
					break;
				case "right":
					AppendDefaultDXClassName(control, "dx-ar");
					break;
				case "center":
					AppendDefaultDXClassName(control, "dx-ac");
					break;
			}
		}
		public static string GetTableSpacings(Control control, int cellPadding, int cellSpacing) {
			return IsHtml5Mode(control)
				? string.Empty
				: string.Format("cellpadding=\"{0}\" cellspacing=\"{1}\"", cellPadding, cellSpacing);
		}
		public static void SetTableSpacings(HtmlTable table, int cellPadding, int cellSpacing) {
			if(!IsHtml5Mode(table)) {
				table.CellPadding = cellPadding;
				table.CellSpacing = cellSpacing;
			}
		}
		public static string GetTableBorder(Control control, int borderWidth) {
			return IsHtml5Mode(control)
				? string.Empty
				: string.Format("border=\"{0}\"", borderWidth);
		}
		public static void SetTableBorder(HtmlTable table, int borderWidth) {
			if(!IsHtml5Mode(table))
				table.Border = borderWidth;
		}
		public static void ApplyCellPadding(Table table, int cellPadding) {
			if(IsHtml5Mode(table)) {
				if(cellPadding > 0) {
					string paddingCssClass = string.Format("dx-p{0}", cellPadding);
					foreach(TableRow row in table.Rows)
						foreach(TableCell cell in row.Cells)
							cell.CssClass = CombineCssClasses(cell.CssClass, paddingCssClass);
				}
			} else
				table.CellPadding = cellPadding;
		}
		public static void ApplyCellPaddingAndSpacing(WebControl table) {
			table.Attributes.Add("cellpadding", "0");
			table.Attributes.Add("cellspacing", "0");
			SetStyleStringAttribute(table, "border-collapse", "collapse");
		}
		public static string GetAlignAttributes(Control control, string horizontalAlign, string verticalAlign) {
			if(IsHtml5Mode(control))
				return string.Empty;
			string result = string.Empty;
			if(!string.IsNullOrEmpty(horizontalAlign))
				result += string.Format("align=\"{0}\"", horizontalAlign);
			if(!string.IsNullOrEmpty(verticalAlign))
				result += string.IsNullOrEmpty(result) ? string.Empty : " " + string.Format("valign=\"{0}\"", verticalAlign);
			return result;
		}
		public static void SetAlignAttributes(HtmlTableCell cell, string horizontalAlign, string verticalAlign) {
			if(!IsHtml5Mode(cell)) {
				if(!string.IsNullOrEmpty(horizontalAlign))
					cell.Align = horizontalAlign;
				if(!string.IsNullOrEmpty(verticalAlign))
					cell.VAlign = verticalAlign;
			}
		}
	}
}
