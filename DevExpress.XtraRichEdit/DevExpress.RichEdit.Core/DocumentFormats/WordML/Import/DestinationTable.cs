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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.OpenXml;
using System.Xml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region DestinationTable
	public class TableDestination : DevExpress.XtraRichEdit.Import.OpenXml.TableDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tr", OnRow);
			result.Add("tblPr", OnTableProperties);
			return result;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TableDestination(WordProcessingMLBaseImporter importer, TableCell parentCell)
			: base(importer, parentCell) {
		}
		public TableDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		static Destination OnRow(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableRowDestination(importer, GetThis(importer).Table);
		}
	}
	#endregion
	#region TableRowDestination
	public class TableRowDestination : DevExpress.XtraRichEdit.Import.OpenXml.TableRowDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tc", OnCell);
			result.Add("trPr", OnTableRowProperties);
			return result;
		}
		public TableRowDestination(WordProcessingMLBaseImporter importer, Table table)
			: base(importer, table) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnCell(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellDestination(importer, GetThis(importer).Row);
		}
	}
	#endregion
	#region TableCellDestination
	public class TableCellDestination : DevExpress.XtraRichEdit.Import.OpenXml.TableCellDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("tcPr", OnCellProperies);
			result.Add("bookmarkStart", OnBookmarkStart);
			result.Add("bookmarkEnd", OnBookmarkEnd);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("altChunk", OnAltChunk);
			return result;
		}
		public TableCellDestination(WordProcessingMLBaseImporter importer, TableRow row)
			: base(importer, row) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static TableCellDestination GetThis(WordProcessingMLBaseImporter importer) {
			return (TableCellDestination)importer.PeekDestination();
		}
		static Destination OnCellProperies(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TableCellPropertiesDestination(importer, GetThis(importer).Cell.Properties);
		}
		static Destination OnParagraph(WordProcessingMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).EndParagraphIndex = importer.Position.ParagraphIndex;
			return new DevExpress.XtraRichEdit.Import.WordML.ParagraphDestination(importer);
		}
		static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			TableCellDestination destination = GetThis(importer);
			return new TableDestination(importer, destination.Cell);
		}
		static Destination OnAltChunk(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AltChunkDestination(importer);
		}
	}
	#endregion
	#region TableCellPropertiesDestination
	public class TableCellPropertiesDestination : DevExpress.XtraRichEdit.Import.OpenXml.TableCellPropertiesDestinationCore {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("tcW", OnTableCellWidth);
			result.Add("tcBorders", OnTableCellBorders);
			result.Add("vmerge", OnTableCellMerge);
			result.Add("gridSpan", OnTableCellColumnSpan);
			result.Add("shd", OnTableCellShading);
			result.Add("tcMar", OnTableCellMargins);
			result.Add("tcFitText", OnTableCellFitText);
			result.Add("noWrap", OnTableCellNoWrap);
			result.Add("hideMark", OnTableCellHideMark);
			result.Add("textDirection", OnTableCellTextDirection);
			result.Add("vAlign", OnTableCellVerticalAlignment);
			result.Add("cnfStyle", OnTableCellConditionalFormatting);
			return result;
		}
		public TableCellPropertiesDestination(WordProcessingMLBaseImporter importer, TableCellProperties cellProperties)
			: base(importer, cellProperties) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
}
