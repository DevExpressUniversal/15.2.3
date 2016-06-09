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

#if OPENDOCUMENT
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	public class RowDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("table-cell", OnCell);
			result.Add("covered-table-cell", OnCoveredTableCell);
			return result;
		}
		static RowDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (RowDestination)importer.PeekDestination();
		}
		#endregion
		bool suppressCellValueAssignment;
		int numberRowsRepeated;
		Row currentRow;
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public RowDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) { }
		public Row CurrentRow {
			get {
				if (currentRow == null)
					currentRow = Importer.CurrentSheet.Rows.GetRow(Importer.IndexNextRow);
				return currentRow;
			}
		}
		public override void ProcessElementOpen(XmlReader reader) {
			numberRowsRepeated = Importer.GetWpSTIntegerValue(reader, "table:number-rows-repeated", 1);
			Importer.IndexNextCell = 0;
			string styleName = Importer.ReadAttribute(reader, "table:style-name");
			string visibility = Importer.GetAttribute(reader, "table:visibility", "visible");
			bool isHidden = !visibility.Equals("visible", System.StringComparison.OrdinalIgnoreCase);
			Importer.ApplyRowFormatOnRow(this, styleName, isHidden);
			suppressCellValueAssignment = Importer.DocumentModel.SuppressCellValueAssignment;
			Importer.DocumentModel.SuppressCellValueAssignment = false;
			Importer.CurrentRowDefaultCellStyleName = Importer.ReadAttribute(reader, "table:default-cell-style-name");
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.IndexNextRow += numberRowsRepeated;
			Importer.DocumentModel.SuppressCellValueAssignment = suppressCellValueAssignment;
		}
		#region Handlers
		static Destination OnCell(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new CellDestination(importer, GetThis(importer));
		}
		static Destination OnCoveredTableCell(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new CellDestination(importer, GetThis(importer));
		}
		#endregion
	}
	public class TableRowGroupDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("table-row", OnRow);
			result.Add("table-row-group", OnTableRowGroup);
			return result;
		}
		#endregion
		bool isCollapsed;
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TableRowGroupDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) { }
		public override void ProcessElementOpen(XmlReader reader) {
			isCollapsed = !Importer.GetBoolean(reader, "table:display", true);
			++Importer.RowOutlineLevel;
		}
		public override void ProcessElementClose(XmlReader reader) {
			--Importer.RowOutlineLevel;
			if (isCollapsed)
				Importer.CurrentSheet.Rows.Last.IsCollapsed = true;
		}
		#region Handlers
		static Destination OnRow(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new RowDestination(importer);
		}
		static Destination OnTableRowGroup(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new TableRowGroupDestination(importer);
		}
		#endregion
	}
}
#endif
