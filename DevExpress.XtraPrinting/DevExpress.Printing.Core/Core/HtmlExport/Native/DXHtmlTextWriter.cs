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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.XtraPrinting.HtmlExport.Controls;
namespace DevExpress.XtraPrinting.HtmlExport.Native {
	public class DXHtmlTextWriter : TextWriter {
		static Dictionary<string, DXHtmlTextWriterAttribute> attributeLookup;
		List<RenderAttribute> attributesList = new List<RenderAttribute>();
		static AttributeInformation[] attributeNameLookup;
		Layout currentLayout = new Layout(DXWebHorizontalAlign.NotSet, true);
		Layout currentWrittenLayout;
		List<TagStackEntry> endTags = new List<TagStackEntry>(0x10);
		int inlineCount;
		bool isDescendant;
		List<DXWebRenderStyle> styleList = new List<DXWebRenderStyle>(20);
		int tagIndex;
		DXHtmlTextWriterTag tagKey;
		static Dictionary<string, DXHtmlTextWriterTag> tagsLookup = new Dictionary<string, DXHtmlTextWriterTag>();
		string tagName;
		static TagInformation[] tagNameLookup = new TagInformation[0x61];
		public const string DefaultTabString = "\t";
		internal const string DesignerRegionAttributeName = "_designerRegion";
		public const char DoubleQuoteChar = '"';
		public const string EndTagLeftChars = "</";
		public const char EqualsChar = '=';
		public const string EqualsDoubleQuoteString = "=\"";
		int indentLevel;
		public const string SelfClosingChars = " /";
		public const string SelfClosingTagEnd = " />";
		public const char SemicolonChar = ';';
		public const char SingleQuoteChar = '\'';
		public const char SlashChar = '/';
		public const char SpaceChar = ' ';
		public const char StyleEqualsChar = ':';
		bool tabsPending;
		string tabString;
		public const char TagLeftChar = '<';
		public const char TagRightChar = '>';
		TextWriter writer;
		static DXHtmlTextWriter() {
			RegisterTag(string.Empty, DXHtmlTextWriterTag.Unknown, TagType.Other);
			RegisterTag("a", DXHtmlTextWriterTag.A, TagType.Inline);
			RegisterTag("acronym", DXHtmlTextWriterTag.Acronym, TagType.Inline);
			RegisterTag("address", DXHtmlTextWriterTag.Address, TagType.Other);
			RegisterTag("area", DXHtmlTextWriterTag.Area, TagType.NonClosing);
			RegisterTag("b", DXHtmlTextWriterTag.B, TagType.Inline);
			RegisterTag("base", DXHtmlTextWriterTag.Base, TagType.NonClosing);
			RegisterTag("basefont", DXHtmlTextWriterTag.Basefont, TagType.NonClosing);
			RegisterTag("bdo", DXHtmlTextWriterTag.Bdo, TagType.Inline);
			RegisterTag("bgsound", DXHtmlTextWriterTag.Bgsound, TagType.NonClosing);
			RegisterTag("big", DXHtmlTextWriterTag.Big, TagType.Inline);
			RegisterTag("blockquote", DXHtmlTextWriterTag.Blockquote, TagType.Other);
			RegisterTag("body", DXHtmlTextWriterTag.Body, TagType.Other);
			RegisterTag("br", DXHtmlTextWriterTag.Br, TagType.Other);
			RegisterTag("button", DXHtmlTextWriterTag.Button, TagType.Inline);
			RegisterTag("caption", DXHtmlTextWriterTag.Caption, TagType.Other);
			RegisterTag("center", DXHtmlTextWriterTag.Center, TagType.Other);
			RegisterTag("cite", DXHtmlTextWriterTag.Cite, TagType.Inline);
			RegisterTag("code", DXHtmlTextWriterTag.Code, TagType.Inline);
			RegisterTag("col", DXHtmlTextWriterTag.Col, TagType.NonClosing);
			RegisterTag("colgroup", DXHtmlTextWriterTag.Colgroup, TagType.Other);
			RegisterTag("del", DXHtmlTextWriterTag.Del, TagType.Inline);
			RegisterTag("dd", DXHtmlTextWriterTag.Dd, TagType.Inline);
			RegisterTag("dfn", DXHtmlTextWriterTag.Dfn, TagType.Inline);
			RegisterTag("dir", DXHtmlTextWriterTag.Dir, TagType.Other);
			RegisterTag("div", DXHtmlTextWriterTag.Div, TagType.Other);
			RegisterTag("dl", DXHtmlTextWriterTag.Dl, TagType.Other);
			RegisterTag("dt", DXHtmlTextWriterTag.Dt, TagType.Inline);
			RegisterTag("em", DXHtmlTextWriterTag.Em, TagType.Inline);
			RegisterTag("embed", DXHtmlTextWriterTag.Embed, TagType.NonClosing);
			RegisterTag("fieldset", DXHtmlTextWriterTag.Fieldset, TagType.Other);
			RegisterTag("font", DXHtmlTextWriterTag.Font, TagType.Inline);
			RegisterTag("form", DXHtmlTextWriterTag.Form, TagType.Other);
			RegisterTag("frame", DXHtmlTextWriterTag.Frame, TagType.NonClosing);
			RegisterTag("frameset", DXHtmlTextWriterTag.Frameset, TagType.Other);
			RegisterTag("h1", DXHtmlTextWriterTag.H1, TagType.Other);
			RegisterTag("h2", DXHtmlTextWriterTag.H2, TagType.Other);
			RegisterTag("h3", DXHtmlTextWriterTag.H3, TagType.Other);
			RegisterTag("h4", DXHtmlTextWriterTag.H4, TagType.Other);
			RegisterTag("h5", DXHtmlTextWriterTag.H5, TagType.Other);
			RegisterTag("h6", DXHtmlTextWriterTag.H6, TagType.Other);
			RegisterTag("head", DXHtmlTextWriterTag.Head, TagType.Other);
			RegisterTag("hr", DXHtmlTextWriterTag.Hr, TagType.NonClosing);
			RegisterTag("html", DXHtmlTextWriterTag.Html, TagType.Other);
			RegisterTag("i", DXHtmlTextWriterTag.I, TagType.Inline);
			RegisterTag("iframe", DXHtmlTextWriterTag.Iframe, TagType.Other);
			RegisterTag("img", DXHtmlTextWriterTag.Img, TagType.NonClosing);
			RegisterTag("input", DXHtmlTextWriterTag.Input, TagType.NonClosing);
			RegisterTag("ins", DXHtmlTextWriterTag.Ins, TagType.Inline);
			RegisterTag("isindex", DXHtmlTextWriterTag.Isindex, TagType.NonClosing);
			RegisterTag("kbd", DXHtmlTextWriterTag.Kbd, TagType.Inline);
			RegisterTag("label", DXHtmlTextWriterTag.Label, TagType.Inline);
			RegisterTag("legend", DXHtmlTextWriterTag.Legend, TagType.Other);
			RegisterTag("li", DXHtmlTextWriterTag.Li, TagType.Inline);
			RegisterTag("link", DXHtmlTextWriterTag.Link, TagType.NonClosing);
			RegisterTag("map", DXHtmlTextWriterTag.Map, TagType.Other);
			RegisterTag("marquee", DXHtmlTextWriterTag.Marquee, TagType.Other);
			RegisterTag("menu", DXHtmlTextWriterTag.Menu, TagType.Other);
			RegisterTag("meta", DXHtmlTextWriterTag.Meta, TagType.NonClosing);
			RegisterTag("nobr", DXHtmlTextWriterTag.Nobr, TagType.Inline);
			RegisterTag("noframes", DXHtmlTextWriterTag.Noframes, TagType.Other);
			RegisterTag("noscript", DXHtmlTextWriterTag.Noscript, TagType.Other);
			RegisterTag("object", DXHtmlTextWriterTag.Object, TagType.Other);
			RegisterTag("ol", DXHtmlTextWriterTag.Ol, TagType.Other);
			RegisterTag("option", DXHtmlTextWriterTag.Option, TagType.Other);
			RegisterTag("p", DXHtmlTextWriterTag.P, TagType.Inline);
			RegisterTag("param", DXHtmlTextWriterTag.Param, TagType.Other);
			RegisterTag("pre", DXHtmlTextWriterTag.Pre, TagType.Other);
			RegisterTag("ruby", DXHtmlTextWriterTag.Ruby, TagType.Other);
			RegisterTag("rt", DXHtmlTextWriterTag.Rt, TagType.Other);
			RegisterTag("q", DXHtmlTextWriterTag.Q, TagType.Inline);
			RegisterTag("s", DXHtmlTextWriterTag.S, TagType.Inline);
			RegisterTag("samp", DXHtmlTextWriterTag.Samp, TagType.Inline);
			RegisterTag("script", DXHtmlTextWriterTag.Script, TagType.Other);
			RegisterTag("select", DXHtmlTextWriterTag.Select, TagType.Other);
			RegisterTag("small", DXHtmlTextWriterTag.Small, TagType.Other);
			RegisterTag("span", DXHtmlTextWriterTag.Span, TagType.Inline);
			RegisterTag("strike", DXHtmlTextWriterTag.Strike, TagType.Inline);
			RegisterTag("strong", DXHtmlTextWriterTag.Strong, TagType.Inline);
			RegisterTag("style", DXHtmlTextWriterTag.Style, TagType.Other);
			RegisterTag("sub", DXHtmlTextWriterTag.Sub, TagType.Inline);
			RegisterTag("sup", DXHtmlTextWriterTag.Sup, TagType.Inline);
			RegisterTag("table", DXHtmlTextWriterTag.Table, TagType.Other);
			RegisterTag("tbody", DXHtmlTextWriterTag.Tbody, TagType.Other);
			RegisterTag("td", DXHtmlTextWriterTag.Td, TagType.Inline);
			RegisterTag("textarea", DXHtmlTextWriterTag.Textarea, TagType.Inline);
			RegisterTag("tfoot", DXHtmlTextWriterTag.Tfoot, TagType.Other);
			RegisterTag("th", DXHtmlTextWriterTag.Th, TagType.Inline);
			RegisterTag("thead", DXHtmlTextWriterTag.Thead, TagType.Other);
			RegisterTag("title", DXHtmlTextWriterTag.Title, TagType.Other);
			RegisterTag("tr", DXHtmlTextWriterTag.Tr, TagType.Other);
			RegisterTag("tt", DXHtmlTextWriterTag.Tt, TagType.Inline);
			RegisterTag("u", DXHtmlTextWriterTag.U, TagType.Inline);
			RegisterTag("ul", DXHtmlTextWriterTag.Ul, TagType.Other);
			RegisterTag("var", DXHtmlTextWriterTag.Var, TagType.Inline);
			RegisterTag("wbr", DXHtmlTextWriterTag.Wbr, TagType.NonClosing);
			RegisterTag("xml", DXHtmlTextWriterTag.Xml, TagType.Other);
			attributeLookup = new Dictionary<string, DXHtmlTextWriterAttribute>();
			attributeNameLookup = new AttributeInformation[0x36];
			RegisterAttribute("abbr", DXHtmlTextWriterAttribute.Abbr, true);
			RegisterAttribute("accesskey", DXHtmlTextWriterAttribute.Accesskey, true);
			RegisterAttribute("align", DXHtmlTextWriterAttribute.Align, false);
			RegisterAttribute("alt", DXHtmlTextWriterAttribute.Alt, true);
			RegisterAttribute("autocomplete", DXHtmlTextWriterAttribute.AutoComplete, false);
			RegisterAttribute("axis", DXHtmlTextWriterAttribute.Axis, true);
			RegisterAttribute("background", DXHtmlTextWriterAttribute.Background, true, true);
			RegisterAttribute("bgcolor", DXHtmlTextWriterAttribute.Bgcolor, false);
			RegisterAttribute("border", DXHtmlTextWriterAttribute.Border, false);
			RegisterAttribute("bordercolor", DXHtmlTextWriterAttribute.Bordercolor, false);
			RegisterAttribute("cellpadding", DXHtmlTextWriterAttribute.Cellpadding, false);
			RegisterAttribute("cellspacing", DXHtmlTextWriterAttribute.Cellspacing, false);
			RegisterAttribute("checked", DXHtmlTextWriterAttribute.Checked, false);
			RegisterAttribute("class", DXHtmlTextWriterAttribute.Class, true);
			RegisterAttribute("cols", DXHtmlTextWriterAttribute.Cols, false);
			RegisterAttribute("colspan", DXHtmlTextWriterAttribute.Colspan, false);
			RegisterAttribute("content", DXHtmlTextWriterAttribute.Content, true);
			RegisterAttribute("coords", DXHtmlTextWriterAttribute.Coords, false);
			RegisterAttribute("dir", DXHtmlTextWriterAttribute.Dir, false);
			RegisterAttribute("disabled", DXHtmlTextWriterAttribute.Disabled, false);
			RegisterAttribute("for", DXHtmlTextWriterAttribute.For, false);
			RegisterAttribute("headers", DXHtmlTextWriterAttribute.Headers, true);
			RegisterAttribute("height", DXHtmlTextWriterAttribute.Height, false);
			RegisterAttribute("href", DXHtmlTextWriterAttribute.Href, true, true);
			RegisterAttribute("id", DXHtmlTextWriterAttribute.Id, false);
			RegisterAttribute("line", DXHtmlTextWriterAttribute.Line, false);
			RegisterAttribute("longdesc", DXHtmlTextWriterAttribute.Longdesc, true, true);
			RegisterAttribute("maxlength", DXHtmlTextWriterAttribute.Maxlength, false);
			RegisterAttribute("multiple", DXHtmlTextWriterAttribute.Multiple, false);
			RegisterAttribute("name", DXHtmlTextWriterAttribute.Name, false);
			RegisterAttribute("nowrap", DXHtmlTextWriterAttribute.Nowrap, false);
			RegisterAttribute("onclick", DXHtmlTextWriterAttribute.Onclick, true);
			RegisterAttribute("onchange", DXHtmlTextWriterAttribute.Onchange, true);
			RegisterAttribute("readonly", DXHtmlTextWriterAttribute.ReadOnly, false);
			RegisterAttribute("rel", DXHtmlTextWriterAttribute.Rel, false);
			RegisterAttribute("rows", DXHtmlTextWriterAttribute.Rows, false);
			RegisterAttribute("rowspan", DXHtmlTextWriterAttribute.Rowspan, false);
			RegisterAttribute("rules", DXHtmlTextWriterAttribute.Rules, false);
			RegisterAttribute("scope", DXHtmlTextWriterAttribute.Scope, false);
			RegisterAttribute("selected", DXHtmlTextWriterAttribute.Selected, false);
			RegisterAttribute("shape", DXHtmlTextWriterAttribute.Shape, false);
			RegisterAttribute("size", DXHtmlTextWriterAttribute.Size, false);
			RegisterAttribute("src", DXHtmlTextWriterAttribute.Src, false, true);
			RegisterAttribute("style", DXHtmlTextWriterAttribute.Style, false);
			RegisterAttribute("tabindex", DXHtmlTextWriterAttribute.Tabindex, false);
			RegisterAttribute("target", DXHtmlTextWriterAttribute.Target, false);
			RegisterAttribute("title", DXHtmlTextWriterAttribute.Title, true);
			RegisterAttribute("type", DXHtmlTextWriterAttribute.Type, false);
			RegisterAttribute("usemap", DXHtmlTextWriterAttribute.Usemap, false);
			RegisterAttribute("valign", DXHtmlTextWriterAttribute.Valign, false);
			RegisterAttribute("value", DXHtmlTextWriterAttribute.Value, true);
			RegisterAttribute("vcard_name", DXHtmlTextWriterAttribute.VCardName, false);
			RegisterAttribute("width", DXHtmlTextWriterAttribute.Width, false);
			RegisterAttribute("wrap", DXHtmlTextWriterAttribute.Wrap, false);
			RegisterAttribute("_designerRegion", DXHtmlTextWriterAttribute.DesignerRegion, false);
		}
		public DXHtmlTextWriter(TextWriter writer)
			: this(writer, "\t") {
		}
		public DXHtmlTextWriter(TextWriter writer, string tabString)
			: base(CultureInfo.InvariantCulture) {
			this.writer = writer;
			this.tabString = tabString;
			indentLevel = 0;
			tabsPending = false;
			isDescendant = base.GetType() != typeof(DXHtmlTextWriter);
			inlineCount = 0;
			this.NewLine = "\r\n";
		}
		public virtual void AddAttribute(string name, string value) {
			DXHtmlTextWriterAttribute attributeKey = GetAttributeKey(name);
			value = EncodeAttributeValue(attributeKey, value);
			AddAttribute(name, value, attributeKey);
		}
		public virtual void AddAttribute(DXHtmlTextWriterAttribute key, string value) {
			int index = (int)key;
			if((index >= 0) && (index < attributeNameLookup.Length)) {
				AttributeInformation information = attributeNameLookup[index];
				AddAttribute(information.name, value, key, information.encode, information.isUrl);
			}
		}
		public virtual void AddAttribute(string name, string value, bool fEndode) {
			value = EncodeAttributeValue(value, fEndode);
			AddAttribute(name, value, GetAttributeKey(name));
		}
		protected virtual void AddAttribute(string name, string value, DXHtmlTextWriterAttribute key) {
			AddAttribute(name, value, key, false, false);
		}
		public virtual void AddAttribute(DXHtmlTextWriterAttribute key, string value, bool fEncode) {
			int index = (int)key;
			if((index >= 0) && (index < attributeNameLookup.Length)) {
				AttributeInformation information = attributeNameLookup[index];
				AddAttribute(information.name, value, key, fEncode, information.isUrl);
			}
		}
		private void AddAttribute(string name, string value, DXHtmlTextWriterAttribute key, bool encode, bool isUrl) {
			RenderAttribute attribute = new RenderAttribute();
			attribute.name = name;
			attribute.value = value;
			attribute.key = key;
			attribute.encode = encode;
			attribute.isUrl = isUrl;
			bool replaced = false;
			for(int i = 0; i < attributesList.Count; i++) {
				RenderAttribute searchAttribute = attributesList[i];
				if(searchAttribute.name == name) {
					attributesList[i] = attribute;
					replaced = true;
					break;
				}
			}
			if(!replaced)
				attributesList.Add(attribute);
		}
		public virtual void AddStyleAttribute(string name, string value) {
			AddStyleAttribute(name, value, DXCssTextWriter.GetStyleKey(name));
		}
		public virtual void AddStyleAttribute(DXHtmlTextWriterStyle key, string value) {
			AddStyleAttribute(DXCssTextWriter.GetStyleName(key), value, key);
		}
		protected virtual void AddStyleAttribute(string name, string value, DXHtmlTextWriterStyle key) {
			DXWebRenderStyle style;
			if(string.IsNullOrEmpty(value))
				return;
			style.name = name;
			style.key = key;
			string str = value;
			if(DXCssTextWriter.IsStyleEncoded(key))
				str = DXHttpUtility.HtmlAttributeEncode(value);
			style.value = str;
			styleList.Add(style);
		}
		public virtual void BeginRender() {
		}
#if DXRESTRICTED
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (writer != null) {
						writer.Dispose();
						writer = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
#else
		public override void Close() {
			writer.Close();
		}
#endif
		protected string EncodeAttributeValue(string value, bool fEncode) {
			if(value == null) {
				return null;
			}
			if(!fEncode) {
				return value;
			}
			return DXHttpUtility.HtmlAttributeEncode(value);
		}
		protected virtual string EncodeAttributeValue(DXHtmlTextWriterAttribute attrKey, string value) {
			bool fEncode = true;
			if((DXHtmlTextWriterAttribute.Accesskey <= attrKey) && ((int)attrKey < attributeNameLookup.Length)) {
				fEncode = attributeNameLookup[(int)attrKey].encode;
			}
			return EncodeAttributeValue(value, fEncode);
		}
		protected string EncodeUrl(string url) {
			if(!DXWebStringUtil.IsUncSharePath(url)) {
				return DXHttpUtility.UrlPathEncode(url);
			}
			return url;
		}
		public virtual void EndRender() {
		}
		public virtual void EnterStyle(DXWebStyle style) {
			EnterStyle(style, DXHtmlTextWriterTag.Span);
		}
		public virtual void EnterStyle(DXWebStyle style, DXHtmlTextWriterTag tag) {
			if(!style.IsEmpty || (tag != DXHtmlTextWriterTag.Span)) {
				style.AddAttributesToRender(this);
				RenderBeginTag(tag);
			}
		}
		public virtual void ExitStyle(DXWebStyle style) {
			ExitStyle(style, DXHtmlTextWriterTag.Span);
		}
		public virtual void ExitStyle(DXWebStyle style, DXHtmlTextWriterTag tag) {
			if(!style.IsEmpty || (tag != DXHtmlTextWriterTag.Span))
				RenderEndTag();
		}
		protected virtual void FilterAttributes() {
			int index = 0;
			for(int i = 0; i < styleList.Count; i++) {
				DXWebRenderStyle style = styleList[i];
				if(OnStyleAttributeRender(style.name, style.value, style.key))
					styleList[index++] = style;
			}
			styleList.RemoveRange(index, styleList.Count - index);
			int num3 = 0;
			for(int i = 0; i < attributesList.Count; i++) {
				RenderAttribute attribute = attributesList[i];
				if(OnAttributeRender(attribute.name, attribute.value, attribute.key))
					attributesList[num3++] = attribute;
			}
		}
		public override void Flush() {
			writer.Flush();
		}
		protected DXHtmlTextWriterAttribute GetAttributeKey(string attrName) {
			if(!string.IsNullOrEmpty(attrName)) {
				DXHtmlTextWriterAttribute obj2;
				if(attributeLookup.TryGetValue(attrName.ToLowerInvariant(), out obj2))
					return obj2;
			}
			return ~DXHtmlTextWriterAttribute.Accesskey;
		}
		protected string GetAttributeName(DXHtmlTextWriterAttribute attrKey) {
			if((attrKey >= DXHtmlTextWriterAttribute.Accesskey) && ((int)attrKey < attributeNameLookup.Length)) {
				return attributeNameLookup[(int)attrKey].name;
			}
			return string.Empty;
		}
		protected DXHtmlTextWriterStyle GetStyleKey(string styleName) {
			return DXCssTextWriter.GetStyleKey(styleName);
		}
		protected string GetStyleName(DXHtmlTextWriterStyle styleKey) {
			return DXCssTextWriter.GetStyleName(styleKey);
		}
		protected virtual DXHtmlTextWriterTag GetTagKey(string tagName) {
			if(!string.IsNullOrEmpty(tagName)) {
				object obj2 = tagsLookup[tagName.ToLowerInvariant()];
				if(obj2 != null) {
					return (DXHtmlTextWriterTag)obj2;
				}
			}
			return DXHtmlTextWriterTag.Unknown;
		}
		protected virtual string GetTagName(DXHtmlTextWriterTag tagKey) {
			int index = (int)tagKey;
			if(index >= 0 && index < tagNameLookup.Length) {
				return tagNameLookup[index].name;
			}
			return string.Empty;
		}
		protected bool IsAttributeDefined(DXHtmlTextWriterAttribute key) {
			foreach(RenderAttribute attribute in attributesList)
				if(attribute.key == key)
					return true;
			return false;
		}
		protected bool IsAttributeDefined(DXHtmlTextWriterAttribute key, out string value) {
			value = null;
			foreach(RenderAttribute attribute in attributesList)
				if(attribute.key == key) {
					value = attribute.value;
					return true;
				}
			return false;
		}
		protected bool IsStyleAttributeDefined(DXHtmlTextWriterStyle key) {
			foreach(DXWebRenderStyle style in styleList)
				if(style.key == key)
					return true;
			return false;
		}
		protected bool IsStyleAttributeDefined(DXHtmlTextWriterStyle key, out string value) {
			value = null;
			foreach(DXWebRenderStyle style in styleList)
				if(style.key == key) {
					value = style.value;
					return true;
				}
			return false;
		}
		public virtual bool IsValidFormAttribute(string attribute) {
			return true;
		}
		protected virtual bool OnAttributeRender(string name, string value, DXHtmlTextWriterAttribute key) {
			return true;
		}
		protected virtual bool OnStyleAttributeRender(string name, string value, DXHtmlTextWriterStyle key) {
			return true;
		}
		protected virtual bool OnTagRender(string name, DXHtmlTextWriterTag key) {
			return true;
		}
		internal virtual void OpenDiv() {
			OpenDiv(currentLayout, (currentLayout != null) && (currentLayout.Align != DXWebHorizontalAlign.NotSet), (currentLayout != null) && !currentLayout.Wrap);
		}
		private void OpenDiv(Layout layout, bool writeHorizontalAlign, bool writeWrapping) {
			WriteBeginTag("div");
			if(writeHorizontalAlign) {
				string str;
				switch(layout.Align) {
					case DXWebHorizontalAlign.Center:
						str = "text-align:center";
						break;
					case DXWebHorizontalAlign.Right:
						str = "text-align:right";
						break;
					default:
						str = "text-align:left";
						break;
				}
				WriteAttribute("style", str);
			}
			if(writeWrapping)
				WriteAttribute("mode", layout.Wrap ? "wrap" : "nowrap");
			Write('>');
			currentWrittenLayout = layout;
		}
		protected virtual void OutputTabs() {
			if(tabsPending) {
				for(int i = 0; i < indentLevel; i++)
					writer.Write(tabString);
				tabsPending = false;
			}
		}
		protected string PopEndTag() {
			if(endTags.Count <= 0) {
				throw new InvalidOperationException("HTMLTextWriterUnbalancedPop");
			}
			TagStackEntry tag = endTags[endTags.Count - 1];
			endTags.RemoveAt(endTags.Count - 1);
			TagKey = tag.tagKey;
			return tag.endTagText;
		}
		protected void PushEndTag(string endTag) {
			TagStackEntry tag = new TagStackEntry();
			tag.tagKey = tagKey;
			tag.endTagText = endTag;
			endTags.Add(tag);
		}
		protected static void RegisterAttribute(string name, DXHtmlTextWriterAttribute key) {
			RegisterAttribute(name, key, false);
		}
		private static void RegisterAttribute(string name, DXHtmlTextWriterAttribute key, bool encode) {
			RegisterAttribute(name, key, encode, false);
		}
		private static void RegisterAttribute(string name, DXHtmlTextWriterAttribute key, bool encode, bool isUrl) {
			string str = name.ToLowerInvariant();
			attributeLookup.Add(str, key);
			if((int)key < attributeNameLookup.Length) {
				attributeNameLookup[(int)key] = new AttributeInformation(name, encode, isUrl);
			}
		}
		protected static void RegisterStyle(string name, DXHtmlTextWriterStyle key) {
			DXCssTextWriter.RegisterAttribute(name, key);
		}
		protected static void RegisterTag(string name, DXHtmlTextWriterTag key) {
			RegisterTag(name, key, TagType.Other);
		}
		private static void RegisterTag(string name, DXHtmlTextWriterTag key, TagType type) {
			string str = name.ToLowerInvariant();
			tagsLookup.Add(str, key);
			string closingTag = null;
			if((type != TagType.NonClosing) && (key != DXHtmlTextWriterTag.Unknown))
				closingTag = string.Format("</{0}>", str);
			if ((int)key < tagNameLookup.Length)
				tagNameLookup[(int)key] = new TagInformation(name, type, closingTag);
		}
		protected virtual string RenderAfterContent() {
			return null;
		}
		protected virtual string RenderAfterTag() {
			return null;
		}
		protected virtual string RenderBeforeContent() {
			return null;
		}
		protected virtual string RenderBeforeTag() {
			return null;
		}
		public virtual void RenderBeginTag(string tagName) {
			TagName = tagName;
			RenderBeginTag(tagKey);
		}
		public virtual void RenderBeginTag(DXHtmlTextWriterTag tagKey) {
			TagKey = tagKey;
			bool flag = TagKey != DXHtmlTextWriterTag.Unknown;
			if(isDescendant) {
				flag = OnTagRender(tagName, this.tagKey);
				FilterAttributes();
				string str = RenderBeforeTag();
				if(str != null) {
					if(tabsPending)
						OutputTabs();
					writer.Write(str);
				}
			}
			TagInformation information = tagNameLookup[tagIndex];
			TagType tagType = information.tagType;
			bool flag2 = flag && (tagType != TagType.NonClosing);
			string endTag = flag2 ? information.closingTag : null;
			if(flag) {
				if(tabsPending)
					OutputTabs();
				writer.Write('<');
				writer.Write(tagName);
				string str3 = null;
				foreach(RenderAttribute attribute in attributesList) {
					if(attribute.key == DXHtmlTextWriterAttribute.Style)
						str3 = attribute.value;
					else {
						writer.Write(' ');
						writer.Write(attribute.name);
						if(attribute.value != null) {
							writer.Write("=\"");
							string url = attribute.value;
							if(attribute.isUrl && (attribute.key != DXHtmlTextWriterAttribute.Href || !url.StartsWith("javascript:", StringComparison.Ordinal)))
								url = EncodeUrl(url);
							if(attribute.encode)
								WriteHtmlAttributeEncode(url);
							else
								writer.Write(url);
							writer.Write('"');
						}
					}
				}
				if((styleList.Count > 0) || (str3 != null)) {
					writer.Write(' ');
					writer.Write("style");
					writer.Write("=\"");
					DXCssTextWriter.WriteAttributes(writer, styleList);
					if(str3 != null)
						writer.Write(str3);
					writer.Write('"');
				}
				if(tagType == TagType.NonClosing)
					writer.Write(" />");
				else
					writer.Write('>');
			}
			string str5 = RenderBeforeContent();
			if(str5 != null) {
				if(tabsPending)
					OutputTabs();
				writer.Write(str5);
			}
			if(flag2) {
				if(tagType == TagType.Inline)
					inlineCount++;
				else {
					WriteLine();
					Indent++;
				}
				if(endTag == null)
					endTag = string.Format("</{0}>", tagName);
			}
			if(isDescendant) {
				string str6 = RenderAfterTag();
				if(str6 != null)
					endTag = (endTag == null) ? str6 : (str6 + endTag);
				string str7 = RenderAfterContent();
				if(str7 != null)
					endTag = (endTag == null) ? str7 : (str7 + endTag);
			}
			PushEndTag(endTag);
			styleList.Clear();
			attributesList.Clear();
		}
		public virtual void RenderEndTag() {
			string str = PopEndTag();
			if(!string.IsNullOrEmpty(str)) {
				if(tagNameLookup[tagIndex].tagType == TagType.Inline) {
					inlineCount--;
					Write(str);
				} else {
					Indent--;
					Write(str);
					WriteLine();
				}
			}
		}
		public override void Write(bool value) {
			if(tabsPending)
				OutputTabs();
			writer.Write(value);
		}
		public override void Write(char value) {
			if(tabsPending)
				OutputTabs();
			writer.Write(value);
		}
		public override void Write(char[] buffer) {
			if(tabsPending)
				OutputTabs();
			writer.Write(buffer);
		}
		public override void Write(double value) {
			if(tabsPending)
				OutputTabs();
			writer.Write(value);
		}
		public override void Write(int value) {
			if(tabsPending)
				OutputTabs();
			writer.Write(value);
		}
		public override void Write(long value) {
			if(tabsPending)
				OutputTabs();
			writer.Write(value);
		}
		public override void Write(object value) {
			if(tabsPending)
				OutputTabs();
			writer.Write(value);
		}
		public override void Write(float value) {
			if(tabsPending)
				OutputTabs();
			writer.Write(value);
		}
		public override void Write(string s) {
			if(tabsPending)
				OutputTabs();
			writer.Write(s);
		}
		public override void Write(string format, params object[] arg) {
			if(tabsPending)
				OutputTabs();
			writer.Write(format, arg);
		}
		public override void Write(string format, object arg0) {
			if(tabsPending)
				OutputTabs();
			writer.Write(format, arg0);
		}
		public override void Write(char[] buffer, int index, int count) {
			if(tabsPending)
				OutputTabs();
			writer.Write(buffer, index, count);
		}
		public override void Write(string format, object arg0, object arg1) {
			if(tabsPending)
				OutputTabs();
			writer.Write(format, arg0, arg1);
		}
		public virtual void WriteAttribute(string name, string value) {
			WriteAttribute(name, value, false);
		}
		public virtual void WriteAttribute(string name, string value, bool fEncode) {
			writer.Write(' ');
			writer.Write(name);
			if(value != null) {
				writer.Write("=\"");
				if(fEncode)
					WriteHtmlAttributeEncode(value);
				else
					writer.Write(value);
				writer.Write('"');
			}
		}
		public virtual void WriteBeginTag(string tagName) {
			if(tabsPending)
				OutputTabs();
			writer.Write('<');
			writer.Write(tagName);
		}
		public virtual void WriteBreak() {
			Write("<br />");
		}
		public virtual void WriteEncodedText(string text) {
			if(text == null)
				throw new ArgumentNullException("text");
			int length = text.Length;
			int startIndex = 0;
			while(startIndex < length) {
				int index = text.IndexOf('\x00a0', startIndex);
				if(index < 0) {
					DXHttpUtility.HtmlEncode((startIndex == 0) ? text : text.Substring(startIndex, length - startIndex), this as TextWriter);
					startIndex = length;
				} else {
					if(index > startIndex)
						DXHttpUtility.HtmlEncode(text.Substring(startIndex, index - startIndex), this);
					Write("&nbsp;");
					startIndex = index + 1;
				}
			}
		}
		public virtual void WriteEncodedUrl(string url) {
			int index = url.IndexOf('?');
			if(index != -1) {
				WriteUrlEncodedString(url.Substring(0, index), false);
				Write(url.Substring(index));
			} else
				WriteUrlEncodedString(url, false);
		}
		public virtual void WriteEncodedUrlParameter(string urlText) {
			WriteUrlEncodedString(urlText, true);
		}
		public virtual void WriteEndTag(string tagName) {
			if(tabsPending)
				OutputTabs();
			writer.Write('<');
			writer.Write('/');
			writer.Write(tagName);
			writer.Write('>');
		}
		public virtual void WriteFullBeginTag(string tagName) {
			if(tabsPending) {
				OutputTabs();
			}
			writer.Write('<');
			writer.Write(tagName);
			writer.Write('>');
		}
		internal void WriteHtmlAttributeEncode(string s) {
			DXHttpUtility.HtmlAttributeEncode(s, writer);
		}
		public override void WriteLine() {
			writer.WriteLine();
			tabsPending = true;
		}
		public override void WriteLine(bool value) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(value);
			tabsPending = true;
		}
		public override void WriteLine(char value) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(value);
			tabsPending = true;
		}
		public override void WriteLine(int value) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(value);
			tabsPending = true;
		}
		public override void WriteLine(char[] buffer) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(buffer);
			tabsPending = true;
		}
		public override void WriteLine(double value) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(value);
			tabsPending = true;
		}
		public override void WriteLine(long value) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(value);
			tabsPending = true;
		}
		public override void WriteLine(object value) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(value);
			tabsPending = true;
		}
		public override void WriteLine(float value) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(value);
			tabsPending = true;
		}
		public override void WriteLine(string s) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(s);
			tabsPending = true;
		}
		public override void WriteLine(string format, params object[] arg) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(format, arg);
			tabsPending = true;
		}
		public override void WriteLine(string format, object arg0) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(format, arg0);
			tabsPending = true;
		}
		public override void WriteLine(char[] buffer, int index, int count) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(buffer, index, count);
			tabsPending = true;
		}
		public override void WriteLine(string format, object arg0, object arg1) {
			if(tabsPending)
				OutputTabs();
			writer.WriteLine(format, arg0, arg1);
			tabsPending = true;
		}
		public void WriteLineNoTabs(string s) {
			writer.WriteLine(s);
			tabsPending = true;
		}
		internal void WriteObsoleteBreak() {
			Write("<br>");
		}
		public virtual void WriteStyleAttribute(string name, string value) {
			WriteStyleAttribute(name, value, false);
		}
		public virtual void WriteStyleAttribute(string name, string value, bool fEncode) {
			writer.Write(name);
			writer.Write(':');
			if(fEncode)
				WriteHtmlAttributeEncode(value);
			else
				writer.Write(value);
			writer.Write(';');
		}
		protected void WriteUrlEncodedString(string text, bool argument) {
			int length = text.Length;
			for(int i = 0; i < length; i++) {
				char ch = text[i];
				if(DXHttpUtility.IsSafe(ch))
					Write(ch);
				else if(!argument && (((ch == '/') || (ch == ':')) || ((ch == '#') || (ch == ','))))
					Write(ch);
				else if((ch == ' ') && argument)
					Write('+');
				else if((ch & 0xff80) == 0) {
					Write('%');
					Write(DXHttpUtility.IntToHex((ch >> 4) & '\x000f'));
					Write(DXHttpUtility.IntToHex(ch & '\x000f'));
				} else
					Write(DXHttpUtility.UrlEncodeNonAscii(char.ToString(ch), Encoding.UTF8));
			}
		}
		public override Encoding Encoding {
			get { return writer.Encoding; }
		}
		public int Indent {
			get { return indentLevel; }
			set {
				if(value < 0)
					value = 0;
				indentLevel = value;
			}
		}
		public TextWriter InnerWriter {
			get { return writer; }
			set { writer = value; }
		}
		public override string NewLine {
			get { return writer.NewLine; }
			set { writer.NewLine = value; }
		}
		internal virtual bool RenderDivAroundHiddenInputs {
			get { return true; }
		}
		protected DXHtmlTextWriterTag TagKey {
			get { return tagKey; }
			set {
				tagIndex = (int)value;
				if(tagIndex < 0 || tagIndex >= tagNameLookup.Length)
					throw new ArgumentOutOfRangeException("value");
				tagKey = value;
				if(value != DXHtmlTextWriterTag.Unknown)
					tagName = tagNameLookup[tagIndex].name;
			}
		}
		protected string TagName {
			get { return tagName; }
			set {
				tagName = value;
				tagKey = GetTagKey(tagName);
				tagIndex = (int)tagKey;
			}
		}
		struct AttributeInformation {
			public string name;
			public bool isUrl;
			public bool encode;
			public AttributeInformation(string name, bool encode, bool isUrl) {
				this.name = name;
				this.encode = encode;
				this.isUrl = isUrl;
			}
		}
		internal class Layout {
			DXWebHorizontalAlign align;
			bool wrap;
			public Layout(DXWebHorizontalAlign alignment, bool wrapping) {
				Align = alignment;
				Wrap = wrapping;
			}
			public DXWebHorizontalAlign Align {
				get { return align; }
				set { align = value; }
			}
			public bool Wrap {
				get { return wrap; }
				set { wrap = value; }
			}
		}
		struct RenderAttribute {
			public string name;
			public string value;
			public DXHtmlTextWriterAttribute key;
			public bool encode;
			public bool isUrl;
			public override string ToString() {
				return string.Format("RenderAttribute [{0}:{1}]", name, value);
			}
		}
		struct TagInformation {
			public string name;
			public DXHtmlTextWriter.TagType tagType;
			public string closingTag;
			public TagInformation(string name, DXHtmlTextWriter.TagType tagType, string closingTag) {
				this.name = name;
				this.tagType = tagType;
				this.closingTag = closingTag;
			}
		}
		struct TagStackEntry {
			public DXHtmlTextWriterTag tagKey;
			public string endTagText;
			public override string ToString() {
				return endTagText;
			}
		}
		enum TagType {
			Inline,
			NonClosing,
			Other
		}
	}
}
