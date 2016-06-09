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
using System.Drawing;
using System.Globalization;
using System.Xml;
namespace DevExpress.XtraExport {
	public class AlignmentNode : OpenXmlElement {
		public enum ReadingDirection {
			Context = 0,
			LeftToRight = 1,
			RightToLeft = 2
		}
		bool useDefaultHorizontalAlignment = true;
		bool useDefaultVerticalAlignment = true;
		StringAlignment horizontalAlignment;
		StringAlignment verticalAlignment;
		public AlignmentNode(XmlDocument document)
			: base(document) {
				WrapText = true;
				ShrinkToFit = true;
				ReadingOrder = ReadingDirection.Context;
		}
		public override string LocalName { get { return "alignment"; } }
		public StringAlignment HorizontalAlignment {
			get { return horizontalAlignment; }
			set { 
				horizontalAlignment = value;
				useDefaultHorizontalAlignment = false;
			}
		}
		public StringAlignment VerticalAlignment {
			get { return verticalAlignment; }
			set { 
				verticalAlignment = value;
				useDefaultVerticalAlignment = false;
			}
		}
		public bool WrapText {
			get;
			set;
		}
		public bool ShrinkToFit {
			get;
			set;
		}
		public ReadingDirection ReadingOrder {
			get;
			set;
		}
		string GetHorizontalAlignment() {
			if(useDefaultHorizontalAlignment)
				return CellHorizontalAlignment.General;
			switch(horizontalAlignment) {
				case StringAlignment.Center:
					return CellHorizontalAlignment.Center;
				case StringAlignment.Near:
					return CellHorizontalAlignment.Left;
				case StringAlignment.Far:
					return CellHorizontalAlignment.Right;
				default:
					throw new ArgumentException("horizontalAlignment");
			}
		}
		string GetVerticalAlignment() {
			if(useDefaultVerticalAlignment)
				return CellVerticalAlignment.Justify;
			switch(verticalAlignment) {
				case StringAlignment.Center:
					return CellVerticalAlignment.Center;
				case StringAlignment.Near:
					return CellVerticalAlignment.Top;
				case StringAlignment.Far:
					return CellVerticalAlignment.Bottom;
				default:
					throw new ArgumentException("verticalAlignment");
			}
		}
		void CollectAlignment() {
			XlsxHelper.AppendAttribute(this, "horizontal", GetHorizontalAlignment());
			XlsxHelper.AppendAttribute(this, "vertical", GetVerticalAlignment());
		}
		protected override void CollectDataBeforeWrite() {
			CollectAlignment();
			XlsxHelper.AppendAttribute(this, "wrapText", WrapText ? "1" : "0");
			XlsxHelper.AppendAttribute(this, "shrinkToFit", ShrinkToFit ? "1" : "0");
			if(ReadingOrder != ReadingDirection.Context)
				XlsxHelper.AppendAttribute(this, "readingOrder", ReadingOrder == ReadingDirection.LeftToRight ? "1" : "2");
		}
	}
	public class XfNode : DefaultXfNode {
		AlignmentNode alignmentNode;
		public XfNode(XmlDocument document)
			: base(document) {
			alignmentNode = new AlignmentNode(document);
			AppendChild(alignmentNode);
		}
		public AlignmentNode AlignmentNode {
			get { return alignmentNode; }
			set { alignmentNode = value; }
		}
		protected override void CollectDataBeforeWrite() {
			base.CollectDataBeforeWrite();
			XlsxHelper.AppendAttribute(this, "applyAlignment", "1");
			XlsxHelper.AppendAttribute(this, "applyNumberFormat", "1");
			XlsxHelper.AppendAttribute(this, "xfId", "0");
		}
	}
	public class DefaultXfNode : OpenXmlElement {
		int numFmtId;
		int fontId;
		int fillId;
		int borderId;
		public DefaultXfNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName { get { return "xf"; } }
		public int NumFmtId {
			get { return numFmtId; }
			set { numFmtId = value; }
		}
		public int FontId {
			get { return fontId; }
			set { fontId = value; }
		}
		public int FillId {
			get { return fillId; }
			set { fillId = value; }
		}
		public int BorderId {
			get { return borderId; }
			set { borderId = value; }
		}
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "numFmtId", numFmtId.ToString());
			XlsxHelper.AppendAttribute(this, "fontId", fontId.ToString());
			XlsxHelper.AppendAttribute(this, "fillId", fillId.ToString());
			XlsxHelper.AppendAttribute(this, "borderId", borderId.ToString());
		}
	}
	public class CellXfsNode : OpenXmlElement {
		public CellXfsNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName { get { return "cellXfs"; } }
		protected override bool IsCollectChildsCount {
			get { return true; }
		}
		public XfNode AppendXfNode() {
			XfNode node = new XfNode(OwnerDocument);
			AppendChild(node);
			return node;
		}
	}
	public class CellStyleXfsNode : OpenXmlElement {
		public CellStyleXfsNode(XmlDocument document)
			: base(document) {
			AppendChild(new DefaultXfNode(OwnerDocument));
		}
		public override string LocalName { get { return "cellStyleXfs"; } }
		protected override bool IsCollectChildsCount {
			get { return true; }
		}
	}
	public class NumFmtNode : OpenXmlElement {
		string formatCode;
		int numFmtId;
		public NumFmtNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "numFmt"; }
		}
		public string FormatCode {
			get { return formatCode; }
			set { formatCode = value; }
		}
		public int NumFmtId {
			get { return numFmtId; }
			set { numFmtId = value; }
		}
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "numFmtId", numFmtId.ToString());
			XlsxHelper.AppendAttribute(this, "formatCode", formatCode);
		}
	}
	public class FontNode : OpenXmlElement {
		Font font;
		ColorNode colorNode;
		public FontNode(XmlDocument document)
			: base(document) {
			colorNode = new ColorNode(document);
			AppendChild(colorNode);
		}
		public override string LocalName {
			get { return "font"; }
		}
		public Font Font {
			get { return font; }
			set { font = value; }
		}
		public ColorNode ColorNode {
			get { return colorNode; }
		}
		protected override void CollectDataBeforeWrite() {
			if(font.Bold)
				AppendChild(OwnerDocument.CreateNode(XmlNodeType.Element, "b", OwnerDocument.NamespaceURI));
			if(font.Italic)
				AppendChild(OwnerDocument.CreateNode(XmlNodeType.Element, "i", OwnerDocument.NamespaceURI));
			if(font.Strikeout)
				AppendChild(OwnerDocument.CreateNode(XmlNodeType.Element, "strike", OwnerDocument.NamespaceURI));
			if(font.Underline)
				AppendChild(OwnerDocument.CreateNode(XmlNodeType.Element, "u", OwnerDocument.NamespaceURI));
			XmlNode szNode = OwnerDocument.CreateNode(XmlNodeType.Element, "sz", OwnerDocument.NamespaceURI);
			AppendChild(szNode);
			XlsxHelper.AppendAttribute(szNode, "val", Math.Truncate(DevExpress.XtraPrinting.Native.FontSizeHelper.GetSizeInPoints(font)).ToString(CultureInfo.InvariantCulture));
			if(!colorNode.Color.IsEmpty)
				AppendChild(colorNode);
			XmlNode nameNode = OwnerDocument.CreateNode(XmlNodeType.Element, "name", OwnerDocument.NamespaceURI);
			AppendChild(nameNode);
			XlsxHelper.AppendAttribute(nameNode, "val", font.Name);
		}
	}
	public class NumFmtsNode : OpenXmlElement {
		Dictionary<string, int> formatToID = new Dictionary<string, int>();
		public NumFmtsNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "numFmts"; }
		}
		protected override bool IsCollectChildsCount {
			get { return true; }
		}
		public NumFmtNode AppendNumFmtNode() {
			NumFmtNode node = new NumFmtNode(OwnerDocument);
			AppendChild(node);
			return node;
		}
		public int AppendNumFmtNode(string formatString) {
			int id;
			if(formatToID.TryGetValue(formatString, out id))
				return id;
			NumFmtNode numFmtNode = AppendNumFmtNode();
			numFmtNode.FormatCode = formatString;
			numFmtNode.NumFmtId = ChildNodes.Count + XlsxHelper.PredefinedNumFmtCount;
			formatToID.Add(numFmtNode.FormatCode, numFmtNode.NumFmtId);
			return numFmtNode.NumFmtId;
		}
		public void AddStandardFormats() {
			FormatStringToExcelNumberFormatConverter converter = new FormatStringToExcelNumberFormatConverter();
			CultureInfo culture = CultureInfo.CurrentCulture;
			NumFmtNode numFmtNode1 = AppendNumFmtNode();
			numFmtNode1.NumFmtId = 5;
			numFmtNode1.FormatCode = converter.ConvertNumeric("c0", culture).FormatString;
			NumFmtNode numFmtNode2 = AppendNumFmtNode();
			numFmtNode2.NumFmtId = 7;
			numFmtNode2.FormatCode = converter.ConvertNumeric("c2", culture).FormatString;
		}
	}
	public class FontsNode : OpenXmlElement {		
		public FontsNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "fonts"; }
		}
		protected override bool IsCollectChildsCount {
			get { return true; }
		}
		public FontNode AppendFontNode() {
			FontNode node = new FontNode(OwnerDocument);
			AppendChild(node);
			return node;
		}
		public int AppendFontNode(ExportCacheCellStyle style) {
			FontNode fontNode = AppendFontNode();
			fontNode.Font = style.TextFont;
			fontNode.ColorNode.Color = style.TextColor;
			return ChildNodes.Count - 1;
		}
	}
	public class FgColorNode : ColorNode {
		public FgColorNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get {
				return "fgColor";
			}
		}
	}
	public class BgColorNode : ColorNode {
		public BgColorNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get {
				return "bgColor";
			}
		}
	}
	public class PatternFillNode : OpenXmlElement {
		FgColorNode fgColorNode;
		BgColorNode bgColorNode;
		BrushStyle brushStyle = BrushStyle.Clear;
		public PatternFillNode(XmlDocument document)
			: base(document) {
			fgColorNode = new FgColorNode(document);
			bgColorNode = new BgColorNode(document);
		}
		public override string LocalName {
			get { return "patternFill"; }
		}
		public FgColorNode FgColorNode {
			get { return fgColorNode; }
			set { fgColorNode = value; }
		}
		public BgColorNode BgColorNode {
			get { return bgColorNode; }
			set { bgColorNode = value; }
		}
		public BrushStyle BrushStyle {
			get { return brushStyle; }
			set { brushStyle = value; }
		}
		string GetPatternType() {
			switch(brushStyle) {
				case BrushStyle.Clear: return "none";
				case BrushStyle.Solid: return "solid";
				default: throw new ArgumentException("brushStyle");
			}
		}
		protected override void CollectDataBeforeWrite() {
			if(!fgColorNode.Color.IsEmpty)
				AppendChild(fgColorNode);
			if(!bgColorNode.Color.IsEmpty)
				AppendChild(bgColorNode);
			XlsxHelper.AppendAttribute(this, "patternType", GetPatternType());
		}
	}
	public class FillNode : OpenXmlElement {
		PatternFillNode patternFillNode;
		public FillNode(XmlDocument document)
			: base(document) {
			patternFillNode = new PatternFillNode(document);
			AppendChild(patternFillNode);
		}
		public override string LocalName {
			get { return "fill"; }
		}
		public PatternFillNode PatternFillNode {
			get { return patternFillNode; }
			set { patternFillNode = value; }
		}
	}
	public class FillsNode : OpenXmlElement {
		public FillsNode(XmlDocument document)
			: base(document) {
			CreateDefaultFills();
		}
		public override string LocalName {
			get { return "fills"; }
		}
		protected override bool IsCollectChildsCount {
			get { return true; }
		}
		void CreateDefaultFills() {
			AppendChild(new FillNode(OwnerDocument));
			AppendChild(new FillNode(OwnerDocument));
		}
		public int AppendFillNode(ExportCacheCellStyle style) {
			FillNode fillNode = new FillNode(OwnerDocument);
			AppendChild(fillNode);
			fillNode.PatternFillNode.BrushStyle = style.BrushStyle_;
			fillNode.PatternFillNode.FgColorNode.Color = style.BkColor;
			return ChildNodes.Count - 1;
		}
	}
	public class BorderSideNode : OpenXmlElement {
		public static int GetBorderWidth(int width, DevExpress.XtraPrinting.BorderDashStyle borderStyle) {
			return ExcelHelper.GetBorderWidth(width, borderStyle);
		}
		bool drawBorder;
		ColorNode colorNode;
		int width;
		DevExpress.XtraPrinting.BorderDashStyle borderDashStyle;
		public BorderSideNode(XmlDocument document)
			: base(document) {
			colorNode = new ColorNode(document);
		}
		public bool DrawBorder {
			get { return drawBorder; }
			set { drawBorder = value; }
		}
		public ColorNode ColorNode {
			get { return colorNode; }
		}
		public int Width {
			get { return width; }
			set { width = value; }
		}
		public DevExpress.XtraPrinting.BorderDashStyle BorderDashStyle {
			get { return borderDashStyle; }
			set { borderDashStyle = value; }
		}
		protected override void CollectDataBeforeWrite() {
			if(drawBorder)
				XlsxHelper.AppendAttribute(this, "style", GetBorderStyleName());
			if(!colorNode.Color.IsEmpty)
				AppendChild(colorNode);
		}
		string GetBorderStyleName() {
			string name = ExcelHelper.ConvertBorderStyle(width, borderDashStyle).ToString();
			return name.Substring(0, 1).ToLower() + name.Substring(1);
		}
	}
	public class LeftBorderNode : BorderSideNode {
		public LeftBorderNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get {
				return "left";
			}
		}
	}
	public class RightBorderNode : BorderSideNode {
		public RightBorderNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get {
				return "right";
			}
		}
	}
	public class TopBorderNode : BorderSideNode {
		public TopBorderNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get {
				return "top";
			}
		}
	}
	public class BottomBorderNode : BorderSideNode {
		public BottomBorderNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get {
				return "bottom";
			}
		}
	}
	public class DiagonalBorderNode : BorderSideNode {
		public DiagonalBorderNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get {
				return "diagonal";
			}
		}
	}
	public class BorderNode : OpenXmlElement {
		LeftBorderNode left;
		RightBorderNode right;
		TopBorderNode top;
		BottomBorderNode bottom;
		DiagonalBorderNode diagonal;
		public BorderNode(XmlDocument document)
			: base(document) {
			left = new LeftBorderNode(document);
			AppendChild(left);
			right = new RightBorderNode(document);
			AppendChild(right);
			top = new TopBorderNode(document);
			AppendChild(top);
			bottom = new BottomBorderNode(document);
			AppendChild(bottom);
			diagonal = new DiagonalBorderNode(document);
			AppendChild(diagonal);
		}
		public override string LocalName {
			get { return "border"; }
		}
		public LeftBorderNode Left {
			get { return left; }
			set { left = value; }
		}
		public RightBorderNode Right {
			get { return right; }
			set { right = value; }
		}
		public TopBorderNode Top {
			get { return top; }
			set { top = value; }
		}
		public BottomBorderNode Bottom {
			get { return bottom; }
			set { bottom = value; }
		}
		public DiagonalBorderNode Diagonal {
			get { return diagonal; }
			set { diagonal = value; }
		}
	}
	public class BordersNode : OpenXmlElement {
		public BordersNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "borders "; }
		}
		protected override bool IsCollectChildsCount {
			get { return true; }
		}
		void InitBorderSideNode(BorderSideNode node, ExportCacheCellBorderStyle style) {
			node.DrawBorder = !style.IsDefault && style.Width > 0;
			node.ColorNode.Color = style.Color_;
			node.Width = style.Width;
			node.BorderDashStyle = style.BorderDashStyle;
		}
		BorderNode CreateBorderNode(ExportCacheCellStyle style) {
			BorderNode node = new BorderNode(OwnerDocument);
			InitBorderSideNode(node.Bottom, style.BottomBorder);
			InitBorderSideNode(node.Top, style.TopBorder);
			InitBorderSideNode(node.Left, style.LeftBorder);
			InitBorderSideNode(node.Right, style.RightBorder);
			return node;
		}
		public int AppendBorderNode(ExportCacheCellStyle style) {
			BorderNode borderNode = CreateBorderNode(style);
			AppendChild(borderNode);
			return ChildNodes.Count - 1;
		}
	}
	public class ColorNode : OpenXmlElement {
		Color color;
		public ColorNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName { get { return "color"; } }
		public Color Color {
			get { return color; }
			set { color = value; }
		}
		protected override void CollectDataBeforeWrite() {
			string colorString = color.ToArgb().ToString("X8");
			XlsxHelper.AppendAttribute(this, "rgb", colorString);
		}
	}
	public class StyleSheetNode : OpenXmlElement {
		NumFmtsNode numFmtsNode;
		FontsNode fontsNode;
		FillsNode fillsNode;
		BordersNode bordersNode;
		CellXfsNode cellXfsNode;
		CellStyleXfsNode cellStyleXfsNode;
		public StyleSheetNode(XmlDocument document)
			: base(document) {
			numFmtsNode = new NumFmtsNode(document);
			fontsNode = new FontsNode(document);
			fillsNode = new FillsNode(document);
			bordersNode = new BordersNode(document);
			cellStyleXfsNode = new CellStyleXfsNode(document);
			cellXfsNode = new CellXfsNode(document);
		}
		public override string LocalName { get { return "styleSheet"; } }
		public NumFmtsNode NumFmtsNode {
			get { return numFmtsNode; }
			set { numFmtsNode = value; }
		}
		public FontsNode FontsNode {
			get { return fontsNode; }
			set { fontsNode = value; }
		}
		public FillsNode FillsNode {
			get { return fillsNode; }
			set { fillsNode = value; }
		}
		public CellXfsNode CellXfsNode {
			get { return cellXfsNode; }
			set { cellXfsNode = value; }
		}
		public BordersNode BordersNode {
			get { return bordersNode; }
			set { bordersNode = value; }
		}
		protected override void CollectDataBeforeWrite() {
			if(numFmtsNode.ChildNodes.Count > 0)
				AppendChild(numFmtsNode);
			AppendChild(fontsNode);
			AppendChild(fillsNode);
			AppendChild(bordersNode);
			AppendChild(cellStyleXfsNode);
			AppendChild(cellXfsNode);
		}
	}
	public class StyleSheetDocument : OpenXmlDocument {
		StyleSheetNode styleSheetNode;
		public StyleSheetDocument()
			: base() {
			styleSheetNode = new StyleSheetNode(this);
			AppendChild(styleSheetNode);
		}
		public StyleSheetNode StyleSheetNode { get { return styleSheetNode; } }
		public override string NamespaceURI {
			get { return XlsxHelper.MainNs; }
		}
	}
}
