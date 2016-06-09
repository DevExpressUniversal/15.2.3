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
using System.Linq;
using DevExpress.Office.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class FakeCellFormatToCellFormatConverter {
		ModelPasteSpecialOptions options;
		bool sameDocumentModels;
		public FakeCellFormatToCellFormatConverter(ModelPasteSpecialOptions options, bool sameDocumentModels) {
			this.options = options;
			this.sameDocumentModels = sameDocumentModels;
		}
		ModelPasteSpecialOptions PasteSpecialOptions { get { return options; } }
		public void CopyActualFormattingFromFakeCellToCell(FakeCell source, ICell target) {
			if (sameDocumentModels && PasteSpecialOptions.ShouldCopyFormatAndStyle) {
				int targetNewFormatIndex = GetTargetCellFormatIndexFromSourceFakeCell(source);
				target.ChangeFormatIndex(targetNewFormatIndex, target.GetBatchUpdateChangeActions());
				return;
			}
		}
		int GetTargetCellFormatIndexFromSourceFakeCell(FakeCell fakeSourceCell) {
			System.Diagnostics.Debug.Assert(sameDocumentModels);
			Worksheet sourceSheet = fakeSourceCell.Worksheet;
			int defaultFormatIndex = (sourceSheet.Workbook as DocumentModel).StyleSheet.DefaultCellFormatIndex;
			Row row = sourceSheet.Rows.TryGetRow(fakeSourceCell.RowIndex);
			if (row != null && row.FormatIndex != defaultFormatIndex) {
				return row.FormatIndex;
			}
			IColumnRange column = sourceSheet.Columns.TryGetColumnRange(fakeSourceCell.ColumnIndex);
			if (column != null && column.FormatIndex != defaultFormatIndex) {
				return column.FormatIndex;
			}
			return defaultFormatIndex;
		}
		public void CopyFill(FakeCell source, ICell target) {
			if (target.ActualFill.BackColor != source.ActualFill.BackColor
				|| target.ActualFill.ForeColor != source.ActualFill.ForeColor
				|| target.ActualFill.PatternType != source.ActualFill.PatternType) { 
				target.Fill.BackColor = source.ActualBackgroundColor;
				target.Fill.ForeColor = source.ActualFill.ForeColor;
				target.Fill.PatternType = source.ActualFill.PatternType;
				target.ApplyFill = true;
			}
		}
		public int FindSourceStyle(FakeCell source) {
			if (source.FormatInfo.StyleIndex > 0)
				return source.FormatInfo.StyleIndex;
			Row sourceRow = source.Worksheet.Rows[source.RowIndex];
			if (sourceRow.FormatInfo.StyleIndex > 0)
				return sourceRow.FormatInfo.StyleIndex;
			Column sourceColumn = source.Worksheet.Columns.GetColumnRangeForReading(source.ColumnIndex);
			if (sourceColumn.FormatInfo.StyleIndex > 0)
				return sourceColumn.FormatInfo.StyleIndex;
			return 0;
		}
	}
}
