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
using System.Xml;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using System.Drawing;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Printing;
#if !SL
using System.Drawing.Printing;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region MasterPageStyleInfo
	public class MasterPageStyleInfo {
		readonly DocumentModel documentModel;
		string pageLayoutName = String.Empty;
		HeaderIndex headerIndex = HeaderIndex.Invalid;
		HeaderIndex headerLeftIndex = HeaderIndex.Invalid;
		FooterIndex footerIndex = FooterIndex.Invalid;
		FooterIndex footerLeftIndex = FooterIndex.Invalid;
		string nextMasterPageStyleName = String.Empty;
		public MasterPageStyleInfo(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "DocumentModel");
			this.documentModel = documentModel;
		}
		public string PageLayoutName { get { return pageLayoutName; } set { pageLayoutName = value; } }
		public SectionHeader Header {
			get {
				if (HeaderIndex == HeaderIndex.Invalid)
					return null;
				return documentModel.Headers[HeaderIndex];
			}
		}
		public SectionHeader HeaderLeft {
			get {
				if (HeaderLeftIndex == HeaderIndex.Invalid)
					return null;
				return documentModel.Headers[HeaderLeftIndex];
			}
		}
		public SectionFooter Footer {
			get {
				if (FooterIndex == FooterIndex.Invalid)
					return null;
				return documentModel.Footers[FooterIndex];
			}
		}
		public SectionFooter FooterLeft {
			get {
				if (FooterLeftIndex == FooterIndex.Invalid)
					return null;
				return documentModel.Footers[FooterLeftIndex];
			}
		}
		public HeaderIndex HeaderIndex { get { return headerIndex; } set { headerIndex = value; } }
		public HeaderIndex HeaderLeftIndex { get { return headerLeftIndex; } set { headerLeftIndex = value; } }
		public FooterIndex FooterIndex { get { return footerIndex; } set { footerIndex = value; } }
		public FooterIndex FooterLeftIndex { get { return footerLeftIndex; } set { footerLeftIndex = value; } }
		public bool IsHeadersAvailable { get { return headerIndex != HeaderIndex.Invalid || headerLeftIndex != HeaderIndex.Invalid; } }
		public bool IsFootersAvailable { get { return footerIndex != FooterIndex.Invalid || footerLeftIndex != FooterIndex.Invalid; } }
		public bool IsHeadersDifferent { get { return headerIndex != HeaderIndex.Invalid && headerLeftIndex != HeaderIndex.Invalid; } }
		public bool IsFootersDifferent { get { return footerIndex != FooterIndex.Invalid && footerLeftIndex != FooterIndex.Invalid; } }
		public string NextMasterPageStyleName {
			get { return nextMasterPageStyleName; }
			set { nextMasterPageStyleName = value; }
		}
		protected internal HeaderIndex CreateEmptyHeader() {
			return documentModel.Sections.First.Headers.CreateEmptyObject(HeaderFooterType.Odd);
		}
		protected internal FooterIndex CreateEmptyFooter() {
			return documentModel.Sections.First.Footers.CreateEmptyObject(HeaderFooterType.Odd);
		}
	}
	#endregion
	#region MasterPageStyles
	public class MasterPageStyles : Dictionary<string, MasterPageStyleInfo> {
	}
	#endregion
	#region MasterPageStyleDestination
	public class MasterPageStyleDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("header", OnHeaderDestination);
			result.Add("footer", OnFooterDestination);
			result.Add("header-left", OnHeaderLeftDestination);
			result.Add("footer-left", OnFooterLeftDestination);
			return result;
		}
		string styleName;
		MasterPageStyleInfo styleInfo;
		public MasterPageStyleDestination(OpenDocumentTextImporter importer)
			: base(importer) {
			this.styleName = String.Empty;
			this.styleInfo = new MasterPageStyleInfo(importer.DocumentModel);
		}
		public MasterPageStyleInfo StyleInfo { get { return styleInfo; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static MasterPageStyleDestination GetThis(OpenDocumentTextImporter importer) {
			return (MasterPageStyleDestination)importer.PeekDestination();
		}
		static Destination OnHeaderDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			if (!importer.DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return new EmptyDestianation(importer);
			MasterPageStyleInfo info = GetThis(importer).StyleInfo;
			info.HeaderIndex = info.CreateEmptyHeader();
			return new HeaderDestination(importer, GetThis(importer).StyleInfo, GetThis(importer).StyleInfo.Header.PieceTable);
		}
		static Destination OnFooterDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			if (!importer.DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return new EmptyDestianation(importer);
			MasterPageStyleInfo info = GetThis(importer).StyleInfo;
			info.FooterIndex = info.CreateEmptyFooter();
			return new FooterDestination(importer, GetThis(importer).StyleInfo, GetThis(importer).StyleInfo.Footer.PieceTable);
		}
		static Destination OnHeaderLeftDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			if (!importer.DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return new EmptyDestianation(importer);
			MasterPageStyleInfo info = GetThis(importer).StyleInfo;
			info.HeaderLeftIndex = info.CreateEmptyHeader();
			return new HeaderDestination(importer, GetThis(importer).StyleInfo, GetThis(importer).StyleInfo.HeaderLeft.PieceTable);
		}
		static Destination OnFooterLeftDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			if (!importer.DocumentModel.DocumentCapabilities.HeadersFootersAllowed)
				return new EmptyDestianation(importer);
			MasterPageStyleInfo info = GetThis(importer).StyleInfo;
			info.FooterLeftIndex = info.CreateEmptyFooter();
			return new FooterDestination(importer, GetThis(importer).StyleInfo, GetThis(importer).StyleInfo.FooterLeft.PieceTable);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.styleName = ImportHelper.GetStyleStringAttribute(reader, "name");
			this.styleInfo.PageLayoutName = ImportHelper.GetStyleStringAttribute(reader, "page-layout-name");
			this.styleInfo.NextMasterPageStyleName = ImportHelper.GetStyleStringAttribute(reader, "next-style-name");
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!String.IsNullOrEmpty(styleName))
				Importer.MasterPageStyles.Add(styleName, styleInfo);
		}
	}
	#endregion
	#region PageLayoutStyleInfo
	public class PageLayoutStyleInfo {
		MarginsInfo margins;
		PageInfo page;
		ColumnStyles columns;
		OpenDocumentPageUsageType pageType;
		Color backgroundColor;
		public PageLayoutStyleInfo(DocumentModel documentModel) {
			this.page = new PageInfo();
			this.margins = new MarginsInfo();
			this.columns = new ColumnStyles();
			this.pageType = OpenDocumentPageUsageType.All;
			this.backgroundColor = DXColor.Empty;
			Initialize(documentModel);
		}
		protected internal virtual void Initialize(DocumentModel documentModel) {
			DocumentCache cache = documentModel.Cache;
			this.page.CopyFrom(cache.PageInfoCache.DefaultItem);
			this.margins.CopyFrom(cache.MarginsInfoCache.DefaultItem);
			this.columns.Clear();
			this.backgroundColor = documentModel.DocumentProperties.PageBackColor;
		}
		public MarginsInfo Margins { get { return margins; } }
		public PageInfo Page { get { return page; } }
		public ColumnStyles Columns { get { return columns; } }
		public OpenDocumentPageUsageType PageType { get { return pageType; } set { pageType = value; } }
		public Color BackgroundColor { get { return backgroundColor; } set { backgroundColor = value; } }
	}
	#endregion
	#region PageLayoutStyles
	public class PageLayoutStyles : Dictionary<string, PageLayoutStyleInfo> {
	}
	#endregion
	#region PageLayoutDestination
	public class PageLayoutStyleDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		string styleName = String.Empty;
		PageLayoutStyleInfo styleInfo;
		public PageLayoutStyleDestination(OpenDocumentTextImporter importer)
			: base(importer) {
			styleInfo = new PageLayoutStyleInfo(importer.DocumentModel);
		}
		internal string StyleName { get { return styleName; } set { styleName = value; } }
		internal PageLayoutStyleInfo StyleInfo { get { return styleInfo; } }
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("page-layout-properties", OnPageLayoutProperties);
			result.Add("header-style", OnHeaderStyle);
			result.Add("footer-style", OnFooterStyle);
			return result;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnPageLayoutProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			PageLayoutStyleInfo styleInfo = GetThis(importer).StyleInfo;
			return new PageLayoutPropertiesDestination(importer, styleInfo);
		}
		static Destination OnHeaderStyle(OpenDocumentTextImporter importer, XmlReader reader) {
			PageLayoutStyleInfo styleInfo = GetThis(importer).StyleInfo;
			return new HeaderStyleDestination(importer, styleInfo);
		}
		static Destination OnFooterStyle(OpenDocumentTextImporter importer, XmlReader reader) {
			PageLayoutStyleInfo styleInfo = GetThis(importer).StyleInfo;
			return new FooterStyleDestination(importer, styleInfo);
		}
		static PageLayoutStyleDestination GetThis(OpenDocumentTextImporter importer) {
			return (PageLayoutStyleDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			StyleName = ImportHelper.GetStyleStringAttribute(reader, "name");
			string pageUsage = ImportHelper.GetStyleStringAttribute(reader, "page-usage");
			StyleInfo.PageType = ImportHelper.PageUsageTypeTable.GetEnumValue(pageUsage);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!Importer.PageLayoutStyles.ContainsKey(StyleName))
				Importer.PageLayoutStyles.Add(StyleName, StyleInfo);
		}
	}
	#endregion
	#region PageLayoutPropertiesDestination
	public class PageLayoutPropertiesDestination : ElementDestination {
		PageLayoutStyleInfo pageStyleInfo;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		public PageLayoutPropertiesDestination(OpenDocumentTextImporter importer, PageLayoutStyleInfo styleInfo)
			: base(importer) {
			this.pageStyleInfo = styleInfo;
		}
		internal PageLayoutStyleInfo PageStyleInfo { get { return pageStyleInfo; } }
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public ColumnStyles ColumnStyles { get { return PageStyleInfo.Columns; } }
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("columns", OnColumns);
			return result;
		}
		static PageLayoutPropertiesDestination GetThis(OpenDocumentTextImporter importer) {
			return (PageLayoutPropertiesDestination)importer.PeekDestination();
		}
		static Destination OnColumns(OpenDocumentTextImporter importer, XmlReader reader) {
			ColumnStyles columnStyles = GetThis(importer).ColumnStyles;
			return new ColumnsStyleDestination(importer, columnStyles);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ImportPageProperties(reader);
			ImportMargins(reader);
		}
		protected internal virtual void ImportPageProperties(XmlReader reader) {
			PageInfo page = PageStyleInfo.Page;
			ValueInfo pageHeight = ImportHelper.GetFoAttributeInfo(reader, "page-height");
			ValueInfo pageWidth = ImportHelper.GetFoAttributeInfo(reader, "page-width");
			if (!pageHeight.IsEmpty)
				page.Height = GetIntegerDocumentValue(pageHeight);
			if (!pageWidth.IsEmpty)
				page.Width = GetIntegerDocumentValue(pageWidth);
			ImportPaperKind(page, pageHeight, pageWidth);
			ImportPrintOrientation(reader, page);
			ImportPageBackgroundColor(reader);
		}
		void ImportPaperKind(PageInfo page, ValueInfo pageHeight, ValueInfo pageWidth) {
			if (pageHeight.Unit == "in" && pageWidth.Unit == "in" && pageHeight.IsValidNumber
							&& pageWidth.IsValidNumber && pageHeight.Value > 0 && pageWidth.Value > 0) {
				Size paperSizeInTwips = new Size(GetTwipsValue(pageWidth), GetTwipsValue(pageHeight));
				page.PaperKind = PaperSizeCalculator.CalculatePaperKind(paperSizeInTwips, PaperKind.Letter);
			}
		}
		protected internal virtual void ImportPrintOrientation(XmlReader reader, PageInfo page) {
			string landscape = ImportHelper.GetStyleStringAttribute(reader, "print-orientation").ToLower(CultureInfo.InvariantCulture);
			if (!String.IsNullOrEmpty(landscape))
				page.Landscape = landscape == "portrait" ? false : true;
		}
		protected internal virtual void ImportPageBackgroundColor(XmlReader reader) {
			string backColor = ImportHelper.GetFoStringAttribute(reader, "background-color");
			if (!String.IsNullOrEmpty(backColor)) {
				Color backgroundColor = ImportHelper.ConvertStringToColor(backColor);
				if (backgroundColor != DXColor.Empty) {
					PageStyleInfo.BackgroundColor = backgroundColor;
				}
			}
		}
		int GetTwipsValue(ValueInfo info) {
			return (int)Math.Round(info.Value * 1440);
		}
		protected internal virtual void ImportMargins(XmlReader reader) {
			MarginsInfo margins = PageStyleInfo.Margins;
			ValueInfo left = ImportHelper.GetFoAttributeInfo(reader, "margin-left");
			if (!left.IsEmpty)
				margins.Left = GetIntegerDocumentValue(left);
			ValueInfo top = ImportHelper.GetFoAttributeInfo(reader, "margin-top");
			if (!top.IsEmpty)
				margins.Top = GetIntegerDocumentValue(top);
			ValueInfo right = ImportHelper.GetFoAttributeInfo(reader, "margin-right");
			if (!right.IsEmpty)
				margins.Right = GetIntegerDocumentValue(right);
			ValueInfo bottom = ImportHelper.GetFoAttributeInfo(reader, "margin-bottom");
			if (!bottom.IsEmpty)
				margins.Bottom = GetIntegerDocumentValue(bottom);
		}
	}
	#endregion
	#region ColumnStyleInfoCollection
	public class ColumnStyles : DXCollection<ColumnStyleInfo> {
		int styleColumnCount;
		int styleColumnGap;
		bool drawVerticalSeparator;
		public int StyleColumnCount { get { return styleColumnCount; } set { styleColumnCount = value; } }
		public int StyleColumnGap { get { return styleColumnGap; } set { styleColumnGap = value; } }
		public bool DrawVerticalSeparator { get { return drawVerticalSeparator; } set { drawVerticalSeparator = value; } }
	}
	#endregion
	#region ColumnStyleInfo
	public class ColumnStyleInfo {
		float relativeWidth;
		float startIndent;
		float endIndent;
		public ColumnStyleInfo(float relativeWidth, float startIndent, float endIndent) {
			this.relativeWidth = relativeWidth;
			this.startIndent = startIndent;
			this.endIndent = endIndent;
		}
		public float RelativeWidth { get { return relativeWidth; } }
		public float StartIndent { get { return startIndent; } }
		public float EndIndent { get { return endIndent; } }
	}
	#endregion
	#region ColumnsStyleDestination
	public class ColumnsStyleDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		ColumnStyles columnStyles;
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("column", OnColumn);
			result.Add("column-sep", OnColumnSeparator);
			return result;
		}
		public ColumnsStyleDestination(OpenDocumentTextImporter importer, ColumnStyles columnStyles)
			: base(importer) {
			Guard.ArgumentNotNull(columnStyles, "columnStyles");
			this.columnStyles = columnStyles;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		internal ColumnStyles ColumnStyleInfos { get { return columnStyles; } }
		static Destination OnColumn(OpenDocumentTextImporter importer, XmlReader reader) {
			ColumnStyles columnInfos = GetThis(importer).ColumnStyleInfos;
			return new ColumnStyleDestination(importer, columnInfos);
		}
		static Destination OnColumnSeparator(OpenDocumentTextImporter importer, XmlReader reader) {
			ColumnStyles columnInfos = GetThis(importer).ColumnStyleInfos;
			return new ColumnSeparatorDestination(importer, columnInfos);
		}
		static ColumnsStyleDestination GetThis(OpenDocumentTextImporter importer) {
			return (ColumnsStyleDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			ColumnStyleInfos.StyleColumnCount = ImportHelper.GetFoIntegerAttribute(reader, "column-count", 0);
			ValueInfo gap = ImportHelper.GetFoAttributeInfo(reader, "column-gap");
			ColumnStyleInfos.StyleColumnGap = GetIntegerDocumentValue(gap);
		}
	}
	#endregion
	#region ColumnStyletination
	public class ColumnStyleDestination : LeafElementDestination {
		ColumnStyles columnStyles;
		public ColumnStyleDestination(OpenDocumentTextImporter importer, ColumnStyles columnStyles)
			: base(importer) {
			Guard.ArgumentNotNull(columnStyles, "columnStyles");
			this.columnStyles = columnStyles;
		}
		internal ColumnStyles ColumnStyles { get { return columnStyles; } }
		public override void ProcessElementOpen(XmlReader reader) {
			float width = ImportHelper.GetStyleAttributeInfo(reader, "rel-width").Value;
			float startIndent = GetFloatDocumentValue(ImportHelper.GetFoAttributeInfo(reader, "start-indent"));
			float endIndent = GetFloatDocumentValue(ImportHelper.GetFoAttributeInfo(reader, "end-indent"));
			ColumnStyles.Add(new ColumnStyleInfo(width, startIndent, endIndent));
		}
	}
	#endregion
	#region ColumnSeparatorDestination
	public class ColumnSeparatorDestination : LeafElementDestination {
		readonly ColumnStyles columnStyles;
		public ColumnSeparatorDestination(OpenDocumentTextImporter importer, ColumnStyles columnStyles)
			: base(importer) {
			Guard.ArgumentNotNull(columnStyles, "columnStyles");
			this.columnStyles = columnStyles;
		}
		internal ColumnStyles ColumnStyles { get { return columnStyles; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string style = ImportHelper.GetStyleStringAttribute(reader, "style");
			ColumnStyles.DrawVerticalSeparator = OpenDocumentHelper.ColumnSeparatorStyleTable.GetEnumValue(style, true);
		}
	}
	#endregion
	#region HeaderFooterStyleDestination ( abstract class )
	public abstract class HeaderFooterStyleDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		readonly PageLayoutStyleInfo pageLayoutStyleInfo;
		protected HeaderFooterStyleDestination(OpenDocumentTextImporter importer, PageLayoutStyleInfo pageLayoutStyleInfo)
			: base(importer) {
			Guard.ArgumentNotNull(pageLayoutStyleInfo, "pageLayoutStyleInfo");
			this.pageLayoutStyleInfo = pageLayoutStyleInfo;
		}
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("header-footer-properties", OnHeaderFooterProperties);
			return result;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public MarginsInfo MarginsInfo { get { return pageLayoutStyleInfo.Margins; } }
		static HeaderFooterStyleDestination GetThis(OpenDocumentTextImporter importer) {
			return (HeaderFooterStyleDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
		}
		static Destination OnHeaderFooterProperties(OpenDocumentTextImporter importer, XmlReader reader) {
			return new HeaderFooterPropertiesDestination(importer, GetThis(importer));
		}
		public abstract void SetFooterHeaderOffset(int value, int minHeight);
		public abstract string GetOffsetAttributeName { get; }
	}
	#endregion
	#region HeaderStyleDestination
	public class HeaderStyleDestination : HeaderFooterStyleDestination {
		public HeaderStyleDestination(OpenDocumentTextImporter importer, PageLayoutStyleInfo pageLayoutStyleInfo)
			: base(importer, pageLayoutStyleInfo) {
		}
		public override void SetFooterHeaderOffset(int value, int minHeight) {
			MarginsInfo.HeaderOffset = Math.Max(0, MarginsInfo.Top - minHeight);
		}
		public override string GetOffsetAttributeName { get { return "margin-bottom"; } }
	}
	#endregion
	#region FooterStyleDestination
	public class FooterStyleDestination : HeaderFooterStyleDestination {
		public FooterStyleDestination(OpenDocumentTextImporter importer, PageLayoutStyleInfo pageLayoutStyleInfo)
			: base(importer, pageLayoutStyleInfo) {
		}
		public override void SetFooterHeaderOffset(int value, int minHeight) {
			MarginsInfo.FooterOffset = MarginsInfo.Bottom;
		}
		public override string GetOffsetAttributeName { get { return "margin-top"; } }
	}
	#endregion
	#region HeaderFooterPropertiesDestination
	public class HeaderFooterPropertiesDestination : LeafElementDestination {
		HeaderFooterStyleDestination parentDestination;
		public HeaderFooterPropertiesDestination(OpenDocumentTextImporter importer, HeaderFooterStyleDestination parentDestination)
			: base(importer) {
			Guard.ArgumentNotNull(parentDestination, "headerFooterStyleDestination");
			this.parentDestination = parentDestination;
		}
		internal HeaderFooterStyleDestination HeaderFooterStyleDestination { get { return parentDestination; } }
		public override void ProcessElementOpen(XmlReader reader) {
			ValueInfo offset = ImportHelper.GetFoAttributeInfo(reader, parentDestination.GetOffsetAttributeName);
			ValueInfo minHeight = ImportHelper.GetFoAttributeInfo(reader, "min-height");
			if (!offset.IsEmpty)
				parentDestination.SetFooterHeaderOffset(GetIntegerDocumentValue(offset), GetIntegerDocumentValue(minHeight));
		}
	}
	#endregion
}
