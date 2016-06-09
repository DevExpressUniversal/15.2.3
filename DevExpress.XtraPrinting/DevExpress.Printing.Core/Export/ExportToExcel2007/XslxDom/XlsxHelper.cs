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
using System.Xml;
using System.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraExport {
	public static class XlsxHelper {
		#region id
		public const byte PredefinedNumFmtCount = 163;
		public const string SheetRelID = "rId{0}";
		public const string SharedStringsRelID = "rIdSS";
		public const string WorkbookRelID = "rIdWB";
		public const string StylesRelID = "rIdSR";
		#endregion
		#region names consts
		public const string SharedStringsName = "sharedStrings.xml";
		public const string StyleSheetName = "styles.xml";
		public const string WorksheetName = @"worksheets/sheet{0}.xml";
		public const string DrawingName = @"../drawings/drawing{0}.xml";
		#endregion
		#region paths consts
		public const string ContentTypesPath = "[Content_Types].xml";
		public const string SharedStringsPath = @"xl/sharedStrings.xml";
		public const string WorksheetPath = @"xl/worksheets/sheet{0}.xml";
		public const string WorksheetRelsPath = @"xl/worksheets/_rels/sheet{0}.xml.rels";
		public const string WorkbookPath = @"xl/workbook.xml";
		public const string WorkbookRelsPath = @"xl/_rels/workbook.xml.rels";
		public const string RelsPath = @"_rels/.rels";
		public const string StyleSheetPath = @"xl/styles.xml";
		public const string DrawingPath = @"xl/drawings/drawing{0}.xml";
		public const string DrawingRelsPath = @"xl/drawings/_rels/drawing{0}.xml.rels";
		public const string Separator = @"/";
		#endregion
		#region target types
		public const string StylesTargetType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles";
		public const string WorksheetTargetType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet";
		public const string SharedStringsTargetType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings";
		public const string HyperlinkTargetType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink";
		public const string ImageTargetType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/image";
		public const string DrawingTargetType = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships/drawing";
		#endregion
		#region ns
		public const string MainNs = @"http://schemas.openxmlformats.org/spreadsheetml/2006/main";
		public const string RelationshipsNs = @"http://schemas.openxmlformats.org/officeDocument/2006/relationships";
		public const string ANs = @"http://schemas.openxmlformats.org/drawingml/2006/main";
		public const string XdrNs = @"http://schemas.openxmlformats.org/drawingml/2006/spreadsheetDrawing";
		#endregion
		public static void CreateDeclaration(XmlDocument document) {
			document.AppendChild(document.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
		}
		public static void AppendAttribute(XmlNode node, string name, string value) {
			AppendAttribute(node, name, value, string.Empty, string.Empty);
		}
		public static void AppendAttribute(XmlNode node, string name, string value, string prefix, string uri) {
			XmlAttribute attr = string.IsNullOrEmpty(uri) ?
				node.OwnerDocument.CreateAttribute(name) : node.OwnerDocument.CreateAttribute(name, uri);
			attr.Value = value;
			if(!string.IsNullOrEmpty(prefix))
				attr.Prefix = prefix;
			node.Attributes.Append(attr);
		}
		public static XmlNode AppendNode(XmlDocument document, XmlNode parentNode, string name, string value, string prefix, string ns) {
			XmlNode node = document.CreateElement(prefix, name, ns);
			node.InnerText = value;
			parentNode.AppendChild(node);
			return node;
		}
		public static XmlNode AppendXdrNode(XmlDocument document, XmlNode parentNode, string name, string value) {
			return AppendNode(document, parentNode, name, value, "xdr", XdrNs);
		}
		public static XmlNode AppendANode(XmlDocument document, XmlNode parentNode, string name, string value) {
			return AppendNode(document, parentNode, name, value, "a", ANs);
		}
		public static string IntToLetter(int intCol) {
			intCol--;
			int intFirstLetter = ((intCol) / 676) + 64;
			int intSecondLetter = ((intCol % 676) / 26) + 64;
			int intThirdLetter = (intCol % 26) + 65;
			if(intSecondLetter == 64 && intFirstLetter > 64) {
				intSecondLetter = 90;
				intFirstLetter--;
			}
			char firstLetter = (intFirstLetter > 64) ? (char)intFirstLetter : ' ';
			char secondLetter = (intSecondLetter > 64) ? (char)intSecondLetter : ' ';
			char thirdLetter = (char)intThirdLetter;
			return string.Concat(firstLetter, secondLetter, thirdLetter).Trim();
		}
		public static string ColorToRGBString(System.Drawing.Color lineColor) {
			return String.Format("{0:X2}{1:X2}{2:X2}", lineColor.R, lineColor.G, lineColor.B);
		}
		public static string LineStyleToString(System.Drawing.Drawing2D.DashStyle lineStyle) {
			switch(lineStyle) {
				case System.Drawing.Drawing2D.DashStyle.Dash:
					return "dash";
				case System.Drawing.Drawing2D.DashStyle.DashDot:
					return "dashDot";
				case System.Drawing.Drawing2D.DashStyle.DashDotDot:
					return "lgDashDotDot";
				case System.Drawing.Drawing2D.DashStyle.Dot:
					return "sysDot";
				case System.Drawing.Drawing2D.DashStyle.Solid:
				case System.Drawing.Drawing2D.DashStyle.Custom:
				default:
					return "solid";
			}
		}
	}
	public struct CellHorizontalAlignment {
		public const string Center = "center";
		public const string CenterContinuous = "centerContinuous";
		public const string Distributed = "distributed";
		public const string Fill = "fill";
		public const string General = "general";
		public const string Justify = "justify";
		public const string Left = "left";
		public const string Right = "right";
	}
	public struct CellVerticalAlignment {
		public const string Bottom = "bottom";
		public const string Center = "center";
		public const string Distributed = "distributed";
		public const string Justify = "justify";
		public const string Top = "top";
	}
	public struct TwoCellAnchorInfo {
		Point startCell, endCell;
		PointF startCellOffset, endCellOffset;
		public Point StartCell {
			get { return startCell; }
		}
		public Point EndCell {
			get { return endCell; }
		}
		public PointF StartCellOffset {
			get { return startCellOffset; }
		}
		public PointF EndCellOffset {
			get { return endCellOffset; }
		}
		public TwoCellAnchorInfo(Point startCell, PointF startCellOffset, Point endCell, PointF endCellOffset) {
			this.startCell = startCell;
			this.startCellOffset = startCellOffset;
			this.endCell = endCell;
			this.endCellOffset = endCellOffset;
		}
	}
	public static class TwoCellAnchorHelper {
		public static Tuple<int, float, int, float> GetCorrectCellPart(int startCell, float startCellOffset, int endCell, float endCellOffset, Func<int, float> getSize) {
			Tuple<int, float> startPart = GetCorrectStartCellPart(startCell, startCellOffset, endCell, endCellOffset, getSize);
			Tuple<int, float> endPart = GetCorrectEndCellPart(startCell, startCellOffset, endCell, endCellOffset, getSize);
			return Tuple.Create(startPart.Item1, startPart.Item2, endPart.Item1, endPart.Item2);
		}
		static Tuple<int, float> GetCorrectStartCellPart(int startCell, float startCellOffset, int endCell, float endCellOffset, Func<int, float> getSize) {
			int correctStartCell = startCell;
			float correctStartCellOffset = startCellOffset;
			if(startCellOffset != 0) {
				float size = startCellOffset;
				for(int i = startCell; i < endCell; i++) {
					float nodeSize = getSize(i + 1);
					if(nodeSize > size)
						break;
					else {
						correctStartCell = i + 1;
						correctStartCellOffset = size - nodeSize;
					}
					size -= nodeSize;
				}
			}
			return Tuple.Create(correctStartCell, correctStartCellOffset);
		}
		static Tuple<int, float> GetCorrectEndCellPart(int startCell, float startCellOffset, int endCell, float endCellOffset, Func<int, float> getSize) {
			int correctEndCell = endCell;
			float correctEndCellOffset = endCellOffset;
			if(endCellOffset != 0) {
				float size = endCellOffset;
				if(startCell == endCell - 1) {
					correctEndCellOffset = getSize(correctEndCell) - size;
					correctEndCell -= 1;
				} else {
					for(int i = endCell; i >= startCell; i--) {
						float nodeSize = getSize(i);
						if(nodeSize > size) {
							correctEndCell = i - 1;
							correctEndCellOffset = nodeSize - size;
							break;
						}
						size -= nodeSize;
					}
				}
			}
			return Tuple.Create(correctEndCell, correctEndCellOffset);
		}
		public static Tuple<int, float> GetCorrectMiddlePoint(int startCell, float startCellOffset, int endCell, float endCellOffset, Func<int, float> getSize) {
			float middlePointFromBegin, correctCoordinateOffset;
			int correctCoordinate;
			float fullSize = endCellOffset - startCellOffset;
			for(int i = startCell; i < endCell; i++) {		
				fullSize += getSize(i + 1);
			}
			middlePointFromBegin = fullSize / 2 + startCellOffset;
			correctCoordinate = startCell;
			correctCoordinateOffset = middlePointFromBegin;
			for(int i = startCell; i < endCell + 1; i++) {
				float currentRowHeight = getSize(i + 1);
				if(middlePointFromBegin > currentRowHeight) {
					middlePointFromBegin -= currentRowHeight;
				} else {
					correctCoordinate = i;
					correctCoordinateOffset = middlePointFromBegin;
					break;
				}
			}
			return Tuple.Create(correctCoordinate, correctCoordinateOffset);
		}
	}
}
