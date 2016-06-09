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
using System.Xml;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Export.Xl;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region CellAlignmentDestination
	public class CellAlignmentDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		internal static Dictionary<XlHorizontalAlignment, string> HorizontalAlignmentTable = CreateHorizontalAlignmentTable();
		static Dictionary<XlHorizontalAlignment, string> CreateHorizontalAlignmentTable() {
			Dictionary<XlHorizontalAlignment, string> result = new Dictionary<XlHorizontalAlignment, string>();
			result.Add(XlHorizontalAlignment.Center, "center");
			result.Add(XlHorizontalAlignment.CenterContinuous, "centerContinuous");
			result.Add(XlHorizontalAlignment.Distributed, "distributed");
			result.Add(XlHorizontalAlignment.Fill, "fill");
			result.Add(XlHorizontalAlignment.General, "general");
			result.Add(XlHorizontalAlignment.Justify, "justify");
			result.Add(XlHorizontalAlignment.Left, "left");
			result.Add(XlHorizontalAlignment.Right, "right");
			return result;
		}
		internal static Dictionary<XlVerticalAlignment, string> VerticalAlignmentTable = CreateVerticalAlignmentTable();
		static Dictionary<XlVerticalAlignment, string> CreateVerticalAlignmentTable() {
			Dictionary<XlVerticalAlignment, string> result = new Dictionary<XlVerticalAlignment, string>();
			result.Add(XlVerticalAlignment.Bottom, "bottom");
			result.Add(XlVerticalAlignment.Center, "center");
			result.Add(XlVerticalAlignment.Distributed, "distributed");
			result.Add(XlVerticalAlignment.Justify, "justify");
			result.Add(XlVerticalAlignment.Top, "top");
			return result;
		}
		internal static Dictionary<XlReadingOrder, string> ReadingOrderTable = CreateReadingOrderTable();
		static Dictionary<XlReadingOrder, string> CreateReadingOrderTable() {
			Dictionary<XlReadingOrder, string> result = new Dictionary<XlReadingOrder, string>();
			result.Add(XlReadingOrder.Context, "0");
			result.Add(XlReadingOrder.LeftToRight, "1");
			result.Add(XlReadingOrder.RightToLeft, "2");
			return result;
		}
		#endregion
		readonly ImportCellFormatInfo importCellFormatInfo;
		readonly CellAlignmentInfo info;
		public CellAlignmentDestination(SpreadsheetMLBaseImporter importer, ImportCellFormatInfo importCellFormatInfo)
			: base(importer) {
			Guard.ArgumentNotNull(importCellFormatInfo, "importCellFormatInfo");
			this.importCellFormatInfo = importCellFormatInfo;
			this.info = new CellAlignmentInfo();
		}
		protected internal ImportCellFormatInfo ImportCellFormatInfo { get { return importCellFormatInfo; } }
		protected internal CellAlignmentInfo Info { get { return info; } }
		public override void ProcessElementOpen(XmlReader reader) {
			Info.HorizontalAlignment = Importer.GetWpEnumValue<XlHorizontalAlignment>(reader, "horizontal", HorizontalAlignmentTable, XlHorizontalAlignment.General);
			Info.VerticalAlignment = Importer.GetWpEnumValue<XlVerticalAlignment>(reader, "vertical", VerticalAlignmentTable, XlVerticalAlignment.Bottom);
			Info.ReadingOrder = Importer.GetWpEnumValue<XlReadingOrder>(reader, "readingOrder", ReadingOrderTable, XlReadingOrder.Context);
			Info.TextRotation = Importer.DocumentModel.UnitConverter.DegreeToModelUnits(Importer.GetWpSTIntegerValue(reader, "textRotation", 0));
			Info.Indent = (byte)Importer.GetWpSTIntegerValue(reader, "indent", 0);
			Info.RelativeIndent = Importer.GetWpSTIntegerValue(reader, "relativeIndent", 0);
			Info.WrapText = Importer.GetWpSTOnOffValue(reader, "wrapText", false);
			Info.JustifyLastLine = Importer.GetWpSTOnOffValue(reader, "justifyLastLine", false);
			Info.ShrinkToFit = Importer.GetWpSTOnOffValue(reader, "shrinkToFit", false);
		}
		public override void ProcessElementClose(XmlReader reader) {
			int alignmentIndex = Importer.StyleSheet.RegisterCellAlignment(Info);
			importCellFormatInfo.AlignmentIndex = alignmentIndex;
		}
	}
	#endregion
	#region DifferentialCellAlignmentDestination
	public class DifferentialCellAlignmentDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly DifferentialFormat differentialFormat;
		public DifferentialCellAlignmentDestination(SpreadsheetMLBaseImporter importer, DifferentialFormat differentialFormat)
			: base(importer) {
			this.differentialFormat = differentialFormat;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			differentialFormat.BeginUpdate();
			try {
				XlHorizontalAlignment? horizontalAlignment = Importer.GetWpEnumOnOffNullValue<XlHorizontalAlignment>(reader, "horizontal", CellAlignmentDestination.HorizontalAlignmentTable);
				if (horizontalAlignment.HasValue)
					differentialFormat.Alignment.Horizontal = horizontalAlignment.Value;
				XlVerticalAlignment? verticalAlignment = Importer.GetWpEnumOnOffNullValue<XlVerticalAlignment>(reader, "vertical", CellAlignmentDestination.VerticalAlignmentTable);
				if (verticalAlignment.HasValue)
					differentialFormat.Alignment.Vertical = verticalAlignment.Value;
				XlReadingOrder? readingOrder = Importer.GetWpEnumOnOffNullValue<XlReadingOrder>(reader, "readingOrder", CellAlignmentDestination.ReadingOrderTable);
				if (readingOrder.HasValue)
					differentialFormat.Alignment.ReadingOrder = readingOrder.Value;
				int textRotation = Importer.GetWpSTIntegerValue(reader, "textRotation", Int32.MinValue);
				if (textRotation > Int32.MinValue)
					differentialFormat.Alignment.TextRotation = Importer.DocumentModel.UnitConverter.DegreeToModelUnits(textRotation);
				int indent = Importer.GetWpSTIntegerValue(reader, "indent", Int32.MinValue);
				if (indent >= byte.MinValue && indent <= byte.MaxValue)
					differentialFormat.Alignment.Indent = (byte)indent;
				int relativeIndent = Importer.GetWpSTIntegerValue(reader, "relativeIndent", Int32.MinValue);
				if (relativeIndent > Int32.MinValue)
					differentialFormat.Alignment.RelativeIndent = relativeIndent;
				bool? wrapText = Importer.GetWpSTOnOffNullValue(reader, "wrapText");
				if (wrapText.HasValue)
					differentialFormat.Alignment.WrapText = wrapText.Value;
				bool? justifyLastLine = Importer.GetWpSTOnOffNullValue(reader, "justifyLastLine");
				if (justifyLastLine.HasValue)
					differentialFormat.Alignment.JustifyLastLine = justifyLastLine.Value;
				bool? shrinkToFit = Importer.GetWpSTOnOffNullValue(reader, "shrinkToFit");
				if (shrinkToFit.HasValue)
					differentialFormat.Alignment.ShrinkToFit = shrinkToFit.Value;
			} finally {
				differentialFormat.EndUpdate();
			}
		}
	}
	#endregion
}
