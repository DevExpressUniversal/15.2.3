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
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal virtual void GenerateColumnsContent() {
			ColumnCollection columns = ActiveSheet.Columns;
			int rowCount = columns.Count;
			if (rowCount == 0)
				return;
			WriteShStartElement("cols");
			try {
				columns.ForEach(ExportColumn);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportColumn(Column column) {
			WriteShStartElement("col");
			try {
				ExportColumnProperties(column);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void ExportColumnProperties(Column column) {
			WriteShIntValue("min", column.StartIndex + 1);
			WriteShIntValue("max", column.EndIndex + 1);
			if(column.Width != ColumnWidthInfo.DefaultValue) {
				float value = (float)Math.Min(255, Workbook.GetService<IColumnWidthCalculationService>().AddGaps(ActiveSheet, column.Width));
				WriteStringValue("width", value.ToString(CultureInfo.InvariantCulture));
			}
			else if(column.Width == 0 && !column.IsCustomWidth && ActiveSheetDefaultColumnWidthInChars > 0)
				WriteStringValue("width", ActiveSheetDefaultColumnWidthInChars.ToString(CultureInfo.InvariantCulture));
			int index;
			if (!ExportStyleSheet.CellFormatTable.TryGetValue(column.FormatIndex, out index))
				Exceptions.ThrowInternalException();
			if (index > 0)
				WriteShIntValue("style", index);
			if (column.BestFit)
				WriteBoolValue("bestFit", column.BestFit);
			if (column.IsHidden)
				WriteBoolValue("hidden", column.IsHidden);
			if (column.IsCollapsed)
				WriteBoolValue("collapsed", column.IsCollapsed);
			if (column.IsCustomWidth)
				WriteBoolValue("customWidth", column.IsCustomWidth);
			if (column.OutlineLevel != 0)
				WriteShIntValue("outlineLevel", column.OutlineLevel);
		}
	}
}
