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
	public class ColumnDestination : LeafElementDestination {
		public ColumnDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string styleName = Importer.ReadAttribute(reader, "table:style-name");
			string visibility = Importer.GetAttribute(reader, "table:visibility", "visible");
			bool isHidden = !visibility.Equals("visible", System.StringComparison.OrdinalIgnoreCase);
			int numberColumnsRepeated = Importer.GetWpSTIntegerValue(reader, "table:number-columns-repeated", 1);
			Column column = new Column(Importer.CurrentSheet, Importer.IndexNextColumn, Importer.IndexNextColumn + numberColumnsRepeated - 1);
			Importer.IndexNextColumn += numberColumnsRepeated;
			Importer.ApplyColumnFormatOnColumn(column, styleName, isHidden);
			Importer.CurrentSheet.Columns.InnerList.Add(column);
			string defaultCellStyleName = Importer.ReadAttribute(reader, "table:default-cell-style-name");
			Importer.RegisterDefaultColumnCellFormat(defaultCellStyleName, column);
		}
	}
	public class TableColumnGroupDestination : ElementDestination {
		#region Static
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("table-column", OnColumn);
			result.Add("table-column-group", OnColumnGroup);
			return result;
		}
		#endregion
		bool isCollapsed;
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public TableColumnGroupDestination(OpenDocumentWorkbookImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			isCollapsed = !Importer.GetBoolean(reader, "table:display", true);
			++Importer.ColumnOutlineLevel;
		}
		public override void ProcessElementClose(XmlReader reader) {
			--Importer.ColumnOutlineLevel;
			if (isCollapsed)
				Importer.CurrentSheet.Columns.GetIsolatedColumn(Importer.IndexNextColumn - 1).IsCollapsed = true;
		}
		#region Handlers
		static Destination OnColumn(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new ColumnDestination(importer);
		}
		static Destination OnColumnGroup(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return new TableColumnGroupDestination(importer);
		}
		#endregion
	}
}
#endif
