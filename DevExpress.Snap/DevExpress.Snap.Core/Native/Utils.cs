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
using DevExpress.Data.Browsing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Native.Data;
using System.Drawing;
namespace DevExpress.Snap.Core.Native {
	public static class TableHelper {
		public static bool IsEmpty(this TableRow row) {
			TableCell firstCell = row.Cells.First;
			TableCell lastCell = row.Cells.Last;
			ParagraphIndex firstParagraphIndex = firstCell.StartParagraphIndex;
			ParagraphIndex endParagraphIndex = lastCell.EndParagraphIndex;
			PieceTable pieceTable = row.PieceTable;
			for (ParagraphIndex i = firstParagraphIndex; i <= endParagraphIndex; i++) {
				if (pieceTable.Paragraphs[i].Length > 1)
					return false;
			}
			return true;
		}
		public static void RemoveEmptyRows(this Table table) {
			List<TableRow> emptyRows = new List<TableRow>();
			table.Rows.ForEach(row => {
				if (row.IsEmpty())
					emptyRows.Add(row);
			});
			int count = emptyRows.Count;
			if (count == table.Rows.Count)
				table.PieceTable.DeleteTableWithContent(table);
			else {
				for (int i = 0; i < count; i++)
					table.PieceTable.DeleteTableRowWithContent(emptyRows[i]);
				table.NormalizeCellColumnSpans();
			}
		}
		public static bool ContainsFields(Table table) {
			return FindBookmarkForFirstRow(table) != null;
		}
		public static SnapBookmark FindBookmarkForFirstRow(Table table) {
			return TableRowHelper.FindTemplateBookmark(table.Rows.First);
		}
		public static SnapBookmark FindSnapBookmarkForFistRow(Table table) {
			return TableRowHelper.FindTemplateBookmark(table.Rows.First);
		}
		public static bool CompareRowsByGrid(TableRow row1, TableRow row2) {
			if (row1.GridBefore != row2.GridBefore)
				return false;
			int count = row1.Cells.Count;
			if (row2.Cells.Count != count)
				return false;
			for (int i = 0; i < count; i++)
				if (row1.Cells[i].ColumnSpan != row2.Cells[i].ColumnSpan)
					return false;
			return row1.GridAfter == row2.GridAfter;
		}
	}
	public static class TableRowHelper {
		public static SnapBookmark FindTemplateBookmark(TableRow row) {
			if (row == null)
				return null;
			SnapPieceTable pieceTable = (SnapPieceTable)row.PieceTable;
			SnapBookmarkController controller = new SnapBookmarkController(pieceTable);
			return controller.FindInnermostTemplateBookmarkByTableCell(row.Cells.First);
		}
		public static List<int> GetColumnSpans(TableRow row) {
			List<int> result = new List<int>();
			result.Add(row.GridBefore);
			int count = row.Cells.Count;
			for (int i = 0; i < count; i++)
				result.Add(row.Cells[i].ColumnSpan);
			result.Add(row.GridAfter);
			return result;
		}
		public static bool CheckColumnSpans(TableRow row, List<int> template) {
			int count = template.Count;
			if (row.Cells.Count != count - 2)
				return false;
			if (row.GridBefore != template[0] || row.GridAfter != template[count - 1])
				return false;
			for (int i = 0; i < count - 2; i++)
				if (row.Cells[i].ColumnSpan != template[i + 1])
					return false;
			return true;
		}
	}
	public static class TableCellHelper {
		public static void PrepareMasterCell(TableCell cell) {
			cell.Properties.BeginInit();
			cell.Properties.Borders.TopBorder.BeginInit();
			cell.Properties.Borders.TopBorder.OnBeginAssign();
			cell.Properties.Borders.TopBorder.Style = XtraRichEdit.Model.BorderLineStyle.Disabled;
			cell.Properties.Borders.TopBorder.OnEndAssign();
			cell.Properties.Borders.TopBorder.EndInit();
			cell.Properties.CellMargins.Left.BeginInit();
			cell.Properties.CellMargins.Left.OnBeginAssign();
			cell.Properties.CellMargins.Left.Type = WidthUnitType.ModelUnits;
			cell.Properties.CellMargins.Left.Value = 0;
			cell.Properties.CellMargins.Left.OnEndAssign();
			cell.Properties.CellMargins.Left.EndInit();
			cell.Properties.CellMargins.Right.BeginInit();
			cell.Properties.CellMargins.Right.OnBeginAssign();
			cell.Properties.CellMargins.Right.Type = WidthUnitType.ModelUnits;
			cell.Properties.CellMargins.Right.Value = 0;
			cell.Properties.CellMargins.Right.OnEndAssign();
			cell.Properties.CellMargins.Right.EndInit();
			cell.Properties.EndInit();
		}
	}
	public static class SnapDataFormats {
		public static readonly string Snx = "Snap XML";
		public static readonly Type SNDataInfo = typeof(SNDataInfo[]);
		public static readonly string SNDataInfoFullName = SNDataInfo.FullName;
	}
	public static class SnapExceptions {
		public static void ThrowInvalidOperationException(SnapStringId id) {
			Exceptions.ThrowInvalidOperationException(SnapLocalizer.Active.GetLocalizedString(id));
		}
	}
	public static class TemplatedFieldTypeQualifier {
		public static bool IsCheckBoxField(DataBrowser dataBrowser) {
			Type dataSourceType = dataBrowser.DataSourceType;
			Type underlyingType = Nullable.GetUnderlyingType(dataSourceType);
			if(underlyingType != null)
				dataSourceType = underlyingType;
			if (dataSourceType == typeof(bool))
				return true;
			object value = dataBrowser.GetValue();
			if(value == null)
				return false;
			Type valueType = value.GetType();
			underlyingType = Nullable.GetUnderlyingType(valueType);
			if (underlyingType != null)
				valueType = underlyingType;
			return valueType.Equals(typeof(bool));
		}
		public static bool IsHyperlinkField(DataBrowser dataBrowser) {
			Type dataSourceType = dataBrowser.DataSourceType;
			if (dataSourceType != typeof(string))
				return false;
			string value = dataBrowser.GetValue() as string;
			if (value == null || !Uri.IsWellFormedUriString(value, UriKind.Absolute))
				return false;
			Uri uri;
			if (!Uri.TryCreate(value, UriKind.Absolute, out uri))
				return false;
			return IsUriSchemeValid(uri);
		}
		static bool IsUriSchemeValid(Uri uri) {
			return uri.Scheme == Uri.UriSchemeHttp ||
				uri.Scheme == Uri.UriSchemeNntp ||
				uri.Scheme == Uri.UriSchemeHttps ||
				uri.Scheme == Uri.UriSchemeFile ||
				uri.Scheme == Uri.UriSchemeFtp ||
				uri.Scheme == Uri.UriSchemeMailto ||
				uri.Scheme == Uri.UriSchemeGopher ||
				uri.Scheme == Uri.UriSchemeNews;
		}
		public static bool IsImageField(DataBrowser dataBrowser) {
			Type dataSourceType = dataBrowser.DataSourceType;
			Type underlyingType = Nullable.GetUnderlyingType(dataBrowser.DataSourceType);
			if(underlyingType != null)
				dataSourceType = underlyingType;
			return typeof(Image).IsAssignableFrom(dataSourceType) || typeof(byte[]).Equals(dataSourceType);
		}
	}
	public static class ListUtils {
		public static bool AreEquals<T>(List<T> list1, List<T> list2) {
			if (Object.ReferenceEquals(list1, list2))
				return true;
			if (Object.ReferenceEquals(list1, null) || Object.ReferenceEquals(list2, null))
				return false;
			int count = list1.Count;
			if (count != list2.Count)
				return false;
			for (int i = 0; i < count; i++)
				if (!Object.Equals(list1[i], list2[i]))
					return false;
			return true;
		}
		public static int CalcHashCode<T>(List<T> list) {
			if (Object.ReferenceEquals(list, null))
				return 0;
			if (list.Count == 0)
				return 0;
			if (Object.ReferenceEquals(list[0], null))
				return 1;
			else
				return list[0].GetHashCode();
		}
	}
	public static class SnapSizeConverter {
		public static Size ModelUnitsToPixels(SnapDocumentModel documentModel, Size size, float dpi) {
			DevExpress.Office.DocumentModelUnitConverter unitConverter = documentModel.UnitConverter;
			int width = unitConverter.ModelUnitsToPixels(size.Width, dpi);
			int height = unitConverter.ModelUnitsToPixels(size.Height, dpi);
			return new Size(width, height);
		}
	}
}
