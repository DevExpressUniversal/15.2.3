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
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Export;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal virtual void ExportRows() {
#if DATA_SHEET
			if (ActiveSheet.IsDataSheet)
				ExportVirtualRows();
			else
#endif
				ExportRowsCore();
		}
		#if BTREE
		protected internal virtual void ExportRowsCore() {
			foreach (ICell cell in ActiveSheet.Cells) {
				ExportCell(cell);
			}
			WriteShEndElement();
		}
#else
		protected internal virtual void ExportRowsCore() {
			IRowCollection rows = ActiveSheet.Rows;
			int rowCount = rows.Count;
			if (rowCount == 0)
				return;
			rows.ForEach(ExportRow);
		}
#endif
		protected internal virtual void ExportRow(Row row) {
			WriteShStartElement("row");
			try {
				ExportRowProperties(row);
				row.Cells.ForEach(ExportCell);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportRowProperties(Row row) {
			WriteShIntValue("r", row.Index + 1);
			if (row.IsCollapsed)
				WriteShBoolValue("collapsed", true);
			if (row.IsHidden)
				WriteShBoolValue("hidden", true);
			if (row.IsThickTopBorder)
				WriteShBoolValue("thickTop", row.IsThickTopBorder);
			if (row.IsThickBottomBorder)
				WriteShBoolValue("thickBot", row.IsThickBottomBorder);
			int outlineLevel = row.OutlineLevel;
			if (outlineLevel > 0)
				WriteShIntValue("outlineLevel", outlineLevel);
			if (row.ApplyStyle) {
				int index;
				if (!ExportStyleSheet.CellFormatTable.TryGetValue(row.FormatIndex, out index))
					Exceptions.ThrowInternalException();
				if (index > 0) {
					WriteShIntValue("s", index);
					WriteBoolValue("customFormat", row.ApplyStyle);
				}
			}
			if (RowExportHelper.GetIsCustomHeight(row, ActiveSheet)) {
				float height = Workbook.UnitConverter.ModelUnitsToPointsF(RowExportHelper.GetRowHeight(row, ActiveSheet));
				WriteStringValue("ht", height.ToString(CultureInfo.InvariantCulture));
				WriteBoolValue("customHeight", true);
			}
		}
		protected internal void ExportVirtualRows() {
			int currentRowIndex = -1;
			IEnumerator<ICellBase> enumerator = ActiveSheet.GetExistingCells().GetEnumerator();
			if(!enumerator.MoveNext())
				return;
			ICellBase currentCell = enumerator.Current;
			bool continueProcessing = true;
			while (continueProcessing) {
				WriteShStartElement("row");
				WriteShIntValue("r", currentCell.RowIndex + 1);
				try {
					currentRowIndex = currentCell.RowIndex;
					do {
						ExportCell(currentCell as ICell);
						if (enumerator.MoveNext())
							currentCell = enumerator.Current;
						else
							continueProcessing = false;
					} while (continueProcessing && currentCell.RowIndex == currentRowIndex);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
	}
}
