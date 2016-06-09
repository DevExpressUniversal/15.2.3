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
using System.Text;
using System.Drawing;
using DevExpress.XtraRichEdit.Services;
using System.IO;
using System.Text.RegularExpressions;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.Data.Utils;
using DevExpress.XtraRichEdit.Model;
using System.Globalization;
using DevExpress.Office.Services;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Text;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Html {
	#region HtmlTag
	public class HtmlTag : TagBase {
		public HtmlTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void ApplyProperties() {
			base.ApplyProperties();
			Importer.RootDoubleFontSize = Importer.Position.CharacterFormatting.DoubleFontSize;
		}
	}
	#endregion
	#region HeadTag
	public class HeadTag : TagBase {
		public HeadTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override void ApplyTagProperties() {
		}
	}
	#endregion
	#region IgnoredTag (abstract class)
	public abstract class IgnoredTag : TagBase {
		protected IgnoredTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override bool ShouldBeIgnored { get { return true; } }
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void EmptyTagProcess() {
			Importer.CloseProcess(this);
		}
	}
	#endregion
	#region TitleTag
	public class TitleTag : IgnoredTag {
		public TitleTag(HtmlImporter importer)
			: base(importer) {
		}
	}
	#endregion
	#region LinkTag
	public class LinkTag : TagBase {
		#region Field
		string type = String.Empty;
		string href = String.Empty;
		List<string> mediaDescriptors = new List<string>();
		#endregion
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("href"), HrefKeyword);
			table.Add(ConvertKeyToUpper("type"), TypeKeyword);
			table.Add(ConvertKeyToUpper("media"), MediaKeyword);
			return table;
		}
		static internal void HrefKeyword(HtmlImporter importer, string value, TagBase tag) {
			LinkTag linkTag = (LinkTag)tag;
			linkTag.href = value;
		}
		static internal void TypeKeyword(HtmlImporter importer, string value, TagBase tag) {
			LinkTag linkTag = (LinkTag)tag;
			linkTag.type = value;
		}
	   static internal void MediaKeyword(HtmlImporter importer, string value, TagBase tag) {
			LinkTag linkTag = (LinkTag)tag;
			linkTag.mediaDescriptors = ParseMediaAttribute(value.ToUpper(CultureInfo.InvariantCulture));
		}
		public LinkTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void OpenTagProcessCore() {
			if (type.ToUpper(CultureInfo.InvariantCulture) == "TEXT/CSS" && (mediaDescriptors.Contains("SCREEN") || mediaDescriptors.Contains("ALL") || mediaDescriptors.Count == 0)) {
				IUriStreamService streamService = Importer.DocumentModel.GetService<IUriStreamService>();
				if (streamService == null)
					return;
				string uri = GetAbsoluteUri(Importer.AbsoluteBaseUri, Uri.UnescapeDataString(href));
				Stream stream = streamService.GetStream(uri);
				if (stream != null) {
					StreamReader reader = new StreamReader(stream);
					Importer.ParseCssElementCollection(reader);
				}
			}
		}
	}
	#endregion
	#region MetaTag
	public class MetaTag : TagBase {
		#region Field
		string charset;
		#endregion
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("charset"), CharsetKeyword);
			table.Add(ConvertKeyToUpper("content"), ContentKeyword);
			table.Add(ConvertKeyToUpper("http-equiv"), HttpEquivKeyword);
			table.Add(ConvertKeyToUpper("name"), NameKeyword);
			return table;
		}
		static internal void CharsetKeyword(HtmlImporter importer, string value, TagBase tag) {
			MetaTag metaTag = (MetaTag)tag;
			metaTag.charset = value;
		}
		static internal void ContentKeyword(HtmlImporter importer, string value, TagBase tag) {
			MetaTag metaTag = (MetaTag)tag;
			string[] values = value.Split(';');
			for (int i = 0; i < values.Length; i++) {
				Match match = AttributePattern.regex.Match(values[i]);
				if (match.Groups["attrName"].Value.ToUpper(CultureInfo.InvariantCulture) == "CHARSET" && match.Groups["attrEq"].Value == "=")
					metaTag.charset = match.Groups["attrValue"].Value;
			}
		}
		static internal void HttpEquivKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void NameKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		public MetaTag(HtmlImporter importer)
			: base(importer) {
		}
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		protected internal override void ApplyTagProperties() {
		}
		protected internal override void OpenTagProcessCore() {
			if (String.IsNullOrEmpty(charset))
				return;
			if (Importer.Options.IgnoreMetaCharset)
				return;
			EncodingInfo[] infos = DXEncoding.GetEncodings();
			foreach (EncodingInfo info in infos) {
				if (StringExtensions.CompareInvariantCultureIgnoreCase(charset, info.Name) == 0) {
					Importer.CodePage = info.CodePage;
					break;
				}
			}
		}
	}
	#endregion
	#region BodyTag
	public class BodyTag : TagBase {
		Color bgColor;
		Color textColor;
		bool useTextColor;
		bool useBgColor;
		static AttributeKeywordTranslatorTable attributeTable = AddAttributes();
		static AttributeKeywordTranslatorTable AddAttributes() {
			AttributeKeywordTranslatorTable table = CreateAttributeTable();
			table.Add(ConvertKeyToUpper("alink"), ActiveLinkKeyword);
			table.Add(ConvertKeyToUpper("background"), BackGroundKeyword);
			table.Add(ConvertKeyToUpper("bgcolor"), BackGroundColorKeyword);
			table.Add(ConvertKeyToUpper("link"), LinkColorKeyword);
			table.Add(ConvertKeyToUpper("text"), TextColorKeyword);
			table.Add(ConvertKeyToUpper("vlink"), OpenLinkColorKeyword);
			return table;
		}
		static internal void ActiveLinkKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void BackGroundKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void BackGroundColorKeyword(HtmlImporter importer, string value, TagBase tag) {
			BodyTag bodyTag = (BodyTag)tag;
			bodyTag.bgColor = GetColorValue(value);
			bodyTag.useBgColor = true;
		}
		static internal void OpenLinkColorKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		static internal void TextColorKeyword(HtmlImporter importer, string value, TagBase tag) {
			BodyTag bodyTag = (BodyTag)tag;
			bodyTag.textColor = MarkupLanguageColorParser.ParseColor(value);
			bodyTag.useTextColor = true;
		}
		static internal void LinkColorKeyword(HtmlImporter importer, string value, TagBase tag) {
		}
		protected internal override AttributeKeywordTranslatorTable AttributeTable { get { return attributeTable; } }
		public BodyTag(HtmlImporter importer)
			: base(importer) {
				if (Importer.Position.CharacterFormatting.Options.UseForeColor) {
					this.textColor = Importer.Position.CharacterFormatting.ForeColor;
					this.useTextColor = true;
				}
		}
		protected internal override void ApplyTagProperties() {
			if (this.useTextColor)
				Importer.Position.CharacterFormatting.ForeColor = textColor;
		}
		protected internal override ParagraphFormattingOptions ApplyCssProperties() {
			ParagraphFormattingOptions options = base.ApplyCssProperties();
			ApplyDocumentModelWebSettings();
			IgnoredMarginPropertiesFromBlockTags();
			Color pageBackColor = Importer.Position.TableProperties.BackgroundColor;
			if ((!DXColor.IsEmpty(pageBackColor)) && Importer.Position.TableProperties.UseBackgroundColor) {
				this.bgColor = pageBackColor;
				this.useBgColor = true;
			}
			return options;
		}
		void ApplyDocumentModelWebSettings() {
			WebSettings htmlSettings = Importer.DocumentModel.WebSettings;
			ParagraphFormattingBase paragraphFormatting = Importer.Position.ParagraphFormatting;
			if (paragraphFormatting.Options.UseLeftIndent)
				htmlSettings.LeftMargin = paragraphFormatting.LeftIndent;
			if (paragraphFormatting.Options.UseRightIndent)
				htmlSettings.RightMargin = paragraphFormatting.RightIndent;
			if (paragraphFormatting.Options.UseSpacingBefore)
				htmlSettings.TopMargin = paragraphFormatting.SpacingBefore;
			if (paragraphFormatting.Options.UseSpacingAfter)
				htmlSettings.BottomMargin = paragraphFormatting.SpacingAfter;
		}
		protected internal override void OpenTagProcessCore() {
			base.OpenTagProcessCore();
			ApplyPageBackColorProperties();
		}
		protected internal virtual void ApplyPageBackColorProperties() {
			if (DXColor.IsEmpty(bgColor) && !useBgColor)
				return;
			DocumentProperties properties = DocumentModel.DocumentProperties;
			properties.PageBackColor = bgColor;
			properties.DisplayBackgroundShape = true;
		}
	}
	#endregion 
}
