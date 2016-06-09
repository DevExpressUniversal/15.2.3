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
using DevExpress.XtraExport;
using System.Xml;
using System.Globalization;
using System.Drawing.Printing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraExport {
	public abstract class RefNode : OpenXmlElement {
		int col1, row1, col2, row2;
		public RefNode(XmlDocument document)
			: base(document) {
		}
		public int Col1 { get { return col1; } set { col1 = value; } }
		public int Col2 { get { return col2; } set { col2 = value; } }
		public int Row1 { get { return row1; } set { row1 = value; } }
		public int Row2 { get { return row2; } set { row2 = value; } }
		protected override void CollectDataBeforeWrite() {
			string cell1 = XlsxHelper.IntToLetter(col1) + row1.ToString();
			string cell2 = XlsxHelper.IntToLetter(col2) + row2.ToString();
			if(col2 > 0 && row2 > 0)
				XlsxHelper.AppendAttribute(this, "ref", string.Format("{0}:{1}", cell1, cell2));
			else
				XlsxHelper.AppendAttribute(this, "ref", string.Format("{0}", cell1));
		}
	}
	public class DimensionNode : RefNode {
		public DimensionNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "dimension"; }
		}
	}
	public class SheetViewNode : OpenXmlElement {
		bool showGridLines = true;
		DevExpress.Utils.DefaultBoolean rightToLeft;
		public SheetViewNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "sheetView"; }
		}
		public bool ShowGridLines { get { return showGridLines; } set { showGridLines = value; } }
		public DevExpress.Utils.DefaultBoolean RightToLeft { get { return rightToLeft; } set { rightToLeft = value; } }
		protected override void CollectDataBeforeWrite() {
			if(!showGridLines)
				XlsxHelper.AppendAttribute(this, "showGridLines", "0");
			if(rightToLeft != Utils.DefaultBoolean.Default)
				XlsxHelper.AppendAttribute(this, "rightToLeft", rightToLeft == Utils.DefaultBoolean.True ? "1" : "0");
			XlsxHelper.AppendAttribute(this, "workbookViewId", "0");
		}
	}
	public class SheetViewsNode : OpenXmlElement {
		SheetViewNode sheetViewNode;
		public SheetViewsNode(XmlDocument document)
			: base(document) {
			sheetViewNode = new SheetViewNode(document);
			AppendChild(sheetViewNode);
		}
		public override string LocalName {
			get { return "sheetViews"; }
		}
		public SheetViewNode SheetViewNode { get { return sheetViewNode; } }
	}
	public class SheetPropertyNode : OpenXmlElement {
		PageSetUpProperty pageSetUpProperty;
		public SheetPropertyNode(XmlDocument document)
			: base(document) {
			pageSetUpProperty = new PageSetUpProperty(document);
			AppendChild(pageSetUpProperty);
		}
		public override string LocalName {
			get { return "sheetPr"; }
		}
		public PageSetUpProperty PageSetUpPr { get { return pageSetUpProperty; } }
	}
	public class PageSetUpProperty : OpenXmlElement {
		bool fitToPage = true;
		public PageSetUpProperty(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "pageSetUpPr"; }
		}
		public bool FitToPage { get { return fitToPage; } set { fitToPage = value; } }
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "fitToPage", Convert.ToInt16(fitToPage).ToString());
		}
	}
	public class PageMarginsNode : OpenXmlElement {
		MarginsF margins;
		public PageMarginsNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "pageMargins"; }
		}
		public MarginsF Margins { get { return margins; } set { margins = value; } }
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "left", margins.Left.ToString("0.0000", CultureInfo.InvariantCulture));
			XlsxHelper.AppendAttribute(this, "right", margins.Right.ToString("0.0000", CultureInfo.InvariantCulture));
			XlsxHelper.AppendAttribute(this, "top", margins.Top.ToString("0.0000", CultureInfo.InvariantCulture));
			XlsxHelper.AppendAttribute(this, "bottom", margins.Bottom.ToString("0.0000", CultureInfo.InvariantCulture));
			XlsxHelper.AppendAttribute(this, "header", "0");
			XlsxHelper.AppendAttribute(this, "footer", "0");
		}
	}
	public class PageSetupNode : OpenXmlElement {
		int paperSize;
		bool landscape;
		int countFitToPagesWide;
		int countFitToPagesLong;
		public PageSetupNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "pageSetup"; }
		}
		public int PaperSize { get { return paperSize; } set { paperSize = value; } }
		public bool IsLandscape { get { return landscape; } set { landscape = value; } }
		public int CountFitToPagesWide { get { return countFitToPagesWide; } set { countFitToPagesWide = value; } }
		public int CountFitToPagesLong { get { return countFitToPagesLong; } set { countFitToPagesLong = value; } }
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "paperSize", paperSize.ToString());
			XlsxHelper.AppendAttribute(this, "fitToWidth", countFitToPagesWide.ToString());
			XlsxHelper.AppendAttribute(this, "fitToHeight", countFitToPagesLong.ToString());
			XlsxHelper.AppendAttribute(this, "orientation", landscape ? "landscape" : "portrait");
		}
	}
	public class ColNode : OpenXmlElement {
		int minMax;
		int widthInPixels;
		public ColNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "col"; }
		}
		public int MinMax { get { return minMax; } set { minMax = value; } }
		public int WidthInPixels { get { return widthInPixels; } set { widthInPixels = value; } }
		double CalculateWidth() {
			return Math.Truncate(widthInPixels / 7.0 * 100.0 + 0.5) / 100.0;
		}
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "min", minMax.ToString());
			XlsxHelper.AppendAttribute(this, "max", minMax.ToString());
			XlsxHelper.AppendAttribute(this, "width", CalculateWidth().ToString(CultureInfo.InvariantCulture));
			XlsxHelper.AppendAttribute(this, "customWidth", "1");
		}
	}
	public class ColsNode : CachedXmlElement {
		public ColsNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "cols"; }
		}
		public ColNode GetColNodeWithCache(int col) {
			ColNode colNode = (ColNode)GetNodeFromCache(col);
			if (colNode == null) {
				colNode = new ColNode(OwnerDocument);
				AppendChild(colNode);
				AddNodeToCache(col, colNode);
				colNode.MinMax = col;
			}
			return colNode;
		}
	}
	public class HyperlinkNodeBase : OpenXmlElement {
		int col;
		int row;
		string display;
		public HyperlinkNodeBase(XmlDocument document)
			: base(document) {
		}
		public int Row {
			get { return row; }
			set { row = value; }
		}
		public int Col {
			get { return col; }
			set { col = value; }
		}
		public override string LocalName {
			get { return "hyperlink"; }
		}
		public string Display {
			get { return display; }
			set { display = value; }
		}
		protected override void CollectDataBeforeWrite() {
			string cell = XlsxHelper.IntToLetter(col) + row.ToString();
			XlsxHelper.AppendAttribute(this, "ref", cell);
			if(!string.IsNullOrEmpty(display))
				XlsxHelper.AppendAttribute(this, "display", display);
		}
	}
	public class HyperlinkNode : HyperlinkNodeBase {
		string rId;
		public HyperlinkNode(XmlDocument document)
			: base(document) {
		}
		public string RId {
			get { return rId; }
			set { rId = value; }
		}
		protected override void CollectDataBeforeWrite() {
			base.CollectDataBeforeWrite();
			string cell = XlsxHelper.IntToLetter(Col) + Row.ToString();
			XlsxHelper.AppendAttribute(this, "id", rId, "r", XlsxHelper.RelationshipsNs);
		}
	}
	public class FileHyperlinkNode : HyperlinkNodeBase {
		string sheetName;
		int destCol;
		int destRow;
		string hyperlinkText;
		public FileHyperlinkNode(XmlDocument document)
			: base(document) {
		}
		public int DestCol {
			get { return destCol; }
			set { destCol = value; }
		}
		public int DestRow {
			get { return destRow; }
			set { destRow = value; }
		}
		public string SheetName {
			get { return sheetName; }
			set { sheetName = value; }
		}
		public string HyperlinkText {
			get { return hyperlinkText; }
			set { hyperlinkText = value; }
		}
		protected override void CollectDataBeforeWrite() {
			base.CollectDataBeforeWrite();
			string cell = XlsxHelper.IntToLetter(destCol) + destRow.ToString();
			XlsxHelper.AppendAttribute(this, "location", string.IsNullOrEmpty(sheetName) ? cell : sheetName + '!' + cell);
		}
	}
	public class HyperlinksNode : OpenXmlElement {
		public HyperlinksNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "hyperlinks"; }
		}
		public HyperlinkNode AppendHyperlinkNode() {
			HyperlinkNode node = new HyperlinkNode(OwnerDocument);
			AppendChild(node);
			return node;
		}
		public FileHyperlinkNode AppendFileHyperlinkNode() {
			FileHyperlinkNode node = new FileHyperlinkNode(OwnerDocument);
			AppendChild(node);
			return node;
		}
		public FileHyperlinkNode AppendFileHyperlinkNode(string hyperlinkText) {
			FileHyperlinkNode node = AppendFileHyperlinkNode();
			node.HyperlinkText = hyperlinkText;
			return node;
		}
		public FileHyperlinkNode AppendFileHyperlinkNode(FileHyperlinkNode baseNode) {
			FileHyperlinkNode node = AppendFileHyperlinkNode();
			node.HyperlinkText = baseNode.HyperlinkText;
			node.DestCol = baseNode.DestCol;
			node.DestRow = baseNode.DestRow;
			return node;
		}
		public void RemoveInvalidLinks() {
			for(int i = this.ChildNodes.Count - 1; i >= 0; i--) {
				if(!IsValidHyperlink(this.ChildNodes[i]))
					this.RemoveChild(this.ChildNodes[i]);
			}
		}
		bool IsValidHyperlink(XmlNode node) {
			if(node is HyperlinkNodeBase && ((node as HyperlinkNodeBase).Row == 0 || (node as HyperlinkNodeBase).Col == 0)) return false;
			if(node is FileHyperlinkNode && ((node as FileHyperlinkNode).DestRow == 0 || (node as FileHyperlinkNode).DestCol == 0)) return false;
			return true;
		}
	}
	public class MergeCellNode : RefNode {
		public MergeCellNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "mergeCell"; }
		}
	}
	public class MergeCellsNode : OpenXmlElement {
		public MergeCellsNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "mergeCells"; }
		}
		protected override bool IsCollectChildsCount {
			get { return ChildNodes.Count > 0; }
		}
		public MergeCellNode AppendMergeCellNode() {
			MergeCellNode node = new MergeCellNode(OwnerDocument);
			AppendChild(node);
			return node;
		}
	}
	public class DrawingNode : OpenXmlElement {
		string id;
		public DrawingNode(XmlDocument document)
			: base(document) {
		}
		public override string LocalName {
			get { return "drawing"; }
		}
		public string Id {
			get { return id; }
			set { id = value; }
		}
		protected override void CollectDataBeforeWrite() {
			XlsxHelper.AppendAttribute(this, "id", id, "r", XlsxHelper.RelationshipsNs);
		}
	}
	public class WorksheetNode : OpenXmlElement {
		string drawingId;
		DimensionNode dimensionNode;
		SheetViewsNode sheetViewsNode;
		SheetPropertyNode sheetPropertyNode;
		ColsNode colsNode;
		XlsxSheetData sheetDataNode;
		MergeCellsNode mergeCellsNode;
		HyperlinksNode hyperlinksNode;
		PageMarginsNode pageMarginsNode;
		PageSetupNode pageSetupNode;
		public WorksheetNode(XmlDocument document)
			: base(document) {
			XlsxHelper.AppendAttribute(this, "xmlns:r", XlsxHelper.RelationshipsNs);
			sheetPropertyNode = new SheetPropertyNode(document);
			AppendChild(sheetPropertyNode);
			dimensionNode = new DimensionNode(document);
			AppendChild(dimensionNode);
			sheetViewsNode = new SheetViewsNode(document);
			AppendChild(sheetViewsNode);
			colsNode = new ColsNode(document);
			sheetDataNode = new XlsxSheetData(document);
			mergeCellsNode = new MergeCellsNode(document);
			hyperlinksNode = new HyperlinksNode(document);
			pageMarginsNode = new PageMarginsNode(document);
			pageSetupNode = new PageSetupNode(document);
		}
		public override string LocalName {
			get { return "worksheet"; }
		}
		public DimensionNode DimensionNode { get { return dimensionNode; } }
		public SheetViewsNode SheetViewsNode { get { return sheetViewsNode; } }
		public ColsNode Cols { get { return colsNode; } }
		public XlsxSheetData SheetDataNode { get { return sheetDataNode; } }
		public MergeCellsNode MergeCellsNode { get { return mergeCellsNode; } }
		public PageMarginsNode PageMarginsNode { get { return pageMarginsNode; } }
		public PageSetupNode PageSetupNode { get { return pageSetupNode; } }
		public SheetPropertyNode SheetPropertyNode { get { return sheetPropertyNode; } }
		public HyperlinksNode HyperlinksNode {
			get { return hyperlinksNode; }
			set { hyperlinksNode = value; }
		}
		public string DrawingId {
			get { return drawingId; }
			set { drawingId = value; }
		}
		protected override void CollectDataBeforeWrite() {
			if(colsNode.ChildNodes.Count > 0)
				AppendChild(colsNode);
			AppendChild(sheetDataNode);
			if(mergeCellsNode.ChildNodes.Count > 0)
				AppendChild(mergeCellsNode);
			hyperlinksNode.RemoveInvalidLinks();
			if(hyperlinksNode.ChildNodes.Count > 0)
				AppendChild(hyperlinksNode);
			AppendChild(pageMarginsNode);
			AppendChild(pageSetupNode);
			if(!string.IsNullOrEmpty(drawingId))
				AppendDrawingNode(drawingId);
		}
		void AppendDrawingNode(string id) {
			DrawingNode drawingNode = new DrawingNode(OwnerDocument);
			AppendChild(drawingNode);
			drawingNode.Id = id;
		}
	}
	public class WorksheetDocument : OpenXmlDocument {
		WorksheetNode worksheetNode;
		public WorksheetDocument() {
			worksheetNode = new WorksheetNode(this);
			AppendChild(worksheetNode);
		}
		public override string NamespaceURI {
			get { return XlsxHelper.MainNs; }
		}
		public WorksheetNode WorksheetNode { get { return worksheetNode; } }
	}
	public class XlsxSheetData : XmlElement {
		Dictionary<int, XlsxRow> indexToRow = new Dictionary<int, XlsxRow>();
		Dictionary<decimal, XlsxCell> indexToRowCell = new Dictionary<decimal, XlsxCell>();
		List<XlsxRow> rows = new List<XlsxRow>();
		public XlsxSheetData(XmlDocument document) : base(document.Prefix, "default", document.NamespaceURI, document) { }
		public XlsxRow GetRowNodeWithCache(int row) {
			XlsxRow res;
			if(this.indexToRow.TryGetValue(row, out res)) return res;
			return AddRow(row);
		}
		public List<XlsxRow> Rows { get { return rows; } }
		public XlsxRow AddRow(int row) {
			XlsxRow res = new XlsxRow();
			res.R = row;
			this.rows.Add(res);
			this.indexToRow.Add(row, res);
			return res;
		}
		public XlsxCell GetCNodeWithCache(int col, int row) {
			decimal rowIndex = (decimal)col * 10000000 + (decimal)row;
			XlsxCell cell;
			if(indexToRowCell.TryGetValue(rowIndex, out cell)) return cell;
			XlsxRow rowNode = GetRowNodeWithCache(row);
			cell = rowNode.AddCellNode(col);
			indexToRowCell.Add(rowIndex, cell);
			return cell;
		}
		public override void WriteTo(XmlWriter writer) {
			writer.WriteStartElement("sheetData");
			WriteRows(writer);
			writer.WriteEndElement();
		}
		void WriteRows(XmlWriter writer) {
			for(int n = 0; n < rows.Count; n++) {
				rows[n].WriteTo(writer);
			}
		}
	}
	public class XlsxRow : IXmlWriteTo {
		int spans1;
		int spans2;
		int r;
		int htInPixels;
		int outlineLevel;
		List<XlsxCell> cells;
		public string LocalName {
			get { return "row"; }
		}
		public int Spans1 { get { return spans1; } set { spans1 = value; } }
		public int Spans2 { get { return spans2; } set { spans2 = value; } }
		public int R { get { return r; } set { r = value; } }
		public int HtInPixels { get { return htInPixels; } set { htInPixels = value; } }
		public int OutlineLevel { get { return outlineLevel; } set { outlineLevel = value; } }
		public List<XlsxCell> Cells { 
			get {
				if(cells == null) cells = new List<XlsxCell>();
				return cells; 
			} 
		}
		double HtInPoints() {
			return htInPixels * 0.75;
		}
		public XlsxCell AddCellNode(int col) {
			XlsxCell node = new XlsxCell();
			node.Row = R;
			node.Col = col;
			if(cells == null) cells = new List<XlsxCell>();
			cells.Add(node);
			return node;
		}
		public virtual void WriteTo(XmlWriter writer) {
			writer.WriteStartElement("row");
			writer.WriteAttributeString("r", r.ToString());
			if(spans1 != 0 && spans2 != 0)
				writer.WriteAttributeString("spans", string.Format("{0}:{1}", spans1, spans2));
			if(htInPixels != 0) {
				writer.WriteAttributeString("ht", HtInPoints().ToString(CultureInfo.InvariantCulture));
				writer.WriteAttributeString("customHeight", "1");
			}
			WriteCells(writer);
			writer.WriteEndElement();
		}
		void WriteCells(XmlWriter writer) {
			if(cells == null) return;
			for(int n = 0; n < cells.Count; n++) {
				cells[n].WriteTo(writer);
			}
		}
	}
	public class XlsxCell : IXmlWriteTo {
		int row;
		int col;
		int styleIndex = -1;
		int stringIndex;
		object objectValue;
		bool isSharedString;
		public int Row { get { return row; } set { row = value; } }
		public int Col { get { return col; } set { col = value; } }
		public int StyleIndex {
			get { return styleIndex; }
			set { styleIndex = value; }
		}
		public object ObjectValue { get { return objectValue; } set { objectValue = value; } }
		public int StringIndex { get { return stringIndex; } set { stringIndex = value; } }
		public bool IsSharedString { get { return isSharedString; } set { isSharedString = value; } }
		string CollectSharedStringData(XmlWriter writer) {
			writer.WriteAttributeString("t", "s");
			return stringIndex.ToString();
		}
		string CollectBooleanData(XmlWriter writer) {
			writer.WriteAttributeString("t", "b");
			return (bool)objectValue ? "1" : "0";
		}
		string CollectNumberData(XmlWriter writer) {
			if(objectValue is double) {
				double d = (double)objectValue;
				if(double.IsNaN(d))
					return CollectErrorData(writer, "#NUM!");
				if(double.IsInfinity(d))
					return CollectErrorData(writer, "#DIV/0!");
			}
			if(objectValue is decimal && ((decimal)objectValue) == 0m)
				return "0";
			return Convert.ToString(objectValue, CultureInfo.InvariantCulture);
		}
		string CollectDateTimeData(XmlWriter writer) {
			double date = ExportUtils.ToOADate((DateTime)objectValue);
			if(date >= 2 && date <= 60) date--;
			return Convert.ToString(date, CultureInfo.InvariantCulture);
		}
		string CollectTimeSpanData(XmlWriter writer) {
			return Convert.ToString(ExportUtils.TimeSpanStartDate.Add((TimeSpan)objectValue).ToOADate(), CultureInfo.InvariantCulture);
		}
		string CollectErrorData(XmlWriter writer) {
			return CollectErrorData(writer, "#VALUE!");
		}
		string CollectErrorData(XmlWriter writer, string error) {
			writer.WriteAttributeString("t", "e");
			return error;
		}
		string CollectDBNullData(XmlWriter writer) {
			return "";
		}
		void WriteValueData(XmlWriter writer) {
			if(!isSharedString && objectValue == null)
				return;
			string value = "";
			if(isSharedString)
				value = CollectSharedStringData(writer);
			else if(objectValue is System.Boolean)
				value = CollectBooleanData(writer);
			else if(ExportUtils.IsIntegerValue(objectValue) || ExportUtils.IsDoubleValue(objectValue))
				value = CollectNumberData(writer);
			else if(objectValue is DateTime)
				value = CollectDateTimeData(writer);
			else if(objectValue is TimeSpan)
				value = CollectTimeSpanData(writer);
			else if(objectValue is System.DBNull)
				value = CollectDBNullData(writer);
			else
				value = CollectErrorData(writer);
			writer.WriteStartElement("v");
			writer.WriteValue(value);
			writer.WriteEndElement();
		}
		public void WriteTo(XmlWriter writer) {
			writer.WriteStartElement("c");
			writer.WriteAttributeString("r", XlsxHelper.IntToLetter(Col) + Row.ToString());
			if(styleIndex > 0)
				writer.WriteAttributeString("s", styleIndex.ToString());
			WriteValueData(writer);
			writer.WriteEndElement();
		}
	}
}
