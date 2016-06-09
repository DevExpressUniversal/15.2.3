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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Native.HoverDecorators;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Commands;
namespace DevExpress.Snap.Native.LayoutUI {
	public class DropFieldTableHotZoneCalculator {
		public Dictionary<Page, HotZoneCollection> CalculateHotZones(RichEditView view) {
			Dictionary<Page, HotZoneCollection> result = new Dictionary<Page, HotZoneCollection>();
			foreach (PageViewInfo page in view.PageViewInfos)
				result.Add(page.Page, CalculateHotZonesByPage(page));
			return result;
		}
		HotZoneCollection CalculateHotZonesByPage(PageViewInfo page) {
			HotZoneCollection result = new HotZoneCollection();
			foreach (PageArea area in page.Page.Areas)
				foreach (Column column in area.Columns)
					if (column.InnerTables != null)
						result.AddRange(CalculateHotZonesByColumn(column));
			return result;
		}
		HotZoneCollection CalculateHotZonesByColumn(Column column) {
			HotZoneCollection result = new HotZoneCollection();
			TableViewInfoCollection tables = column.InnerTables;
			int count = tables.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(CalcHotZonesByTable(tables[i]));
			return result;
		}
		HotZoneCollection CalcHotZonesByTable(TableViewInfo table) {
			HotZoneCollection result = new HotZoneCollection();
			SnapBookmark bookmark = TableHelper.FindBookmarkForFirstRow(table.Table);
			if (bookmark == null)
				return result;
			SNListInfo listInfo = GetSNListInfo(bookmark);
			result.AddRange(CalcHotZonesByTableCore(table, listInfo));
			result.AddRange(CalcMasterDetailHotZones(table, listInfo));
			return result;
		}
		HotZoneCollection CalcHotZonesByTableCore(TableViewInfo table, SNListInfo listInfo) {
			HotZoneCollection result = new HotZoneCollection();
			TableRowViewInfoBase row = GetHotZoneRow(table);
			int count = row.Cells.Count;
			int top = table.GetTableTop();
			int bottom = table.GetTableBottom();
			Rectangle cellBounds;
			for (int i = 0; i < count; i++) {
				TableCellViewInfo cell = row.Cells[i];
				cellBounds = cell.GetBounds();
				result.Add(new DropFieldTableHotZone(Rectangle.FromLTRB(cellBounds.X, top, cellBounds.X, bottom), listInfo, cell.Cell, InsertColumnType.InsertToLeft));
			}
			cellBounds = row.Cells.Last.GetBounds();
			result.Add(new DropFieldTableHotZone(Rectangle.FromLTRB(cellBounds.Right, top, cellBounds.Right, bottom), listInfo, row.Cells.Last.Cell, InsertColumnType.InsertToRight));
			return result;
		}
		HotZoneCollection CalcMasterDetailHotZones(TableViewInfo table, SNListInfo listInfo) {
			HotZoneCollection result = new HotZoneCollection();
			int count = table.RowCount;
			for (int i = 0; i < count; i++) {
				TableRowViewInfoBase row = table.Rows[i];
				SnapBookmark bookmark = TableRowHelper.FindTemplateBookmark(row.Row);
				if (bookmark == null)
					continue;
				SnapTemplateInfo templateInfo = bookmark.TemplateInterval.TemplateInfo;
				if (templateInfo.TemplateType == SnapTemplateIntervalType.DataRow) {
					Rectangle rowBounds = row.GetBounds();
					result.Add(new DropFieldTableMasterDetailHotZone(Rectangle.FromLTRB(rowBounds.Left, rowBounds.Bottom, rowBounds.Right, rowBounds.Bottom), listInfo, row.Row));
				}
			}
			return result;
		}
		SNListInfo GetSNListInfo(SnapBookmark bookmark) {
			SnapPieceTable pieceTable = bookmark.PieceTable;
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(pieceTable, bookmark.TemplateInterval.Start);
			SnapListFieldInfo listFieldInfo = ListFieldSelectionController.FindSNListByRunIndex(pieceTable, start.RunIndex);
			DesignBinding designBinding = FieldsHelper.GetFieldDesignBinding(pieceTable.DocumentModel.DataSourceDispatcher, new SnapFieldInfo(pieceTable, listFieldInfo.Field));
			string dataSource = pieceTable.DocumentModel.DataSourceDispatcher.FindDataSourceName(designBinding.DataSource);
			return new SNListInfo(listFieldInfo.Field.GetTopLevelParent(), dataSource, designBinding.DataMember, listFieldInfo.Field.GetLevel() > 1);
		}
		TableRowViewInfoBase GetHotZoneRow(TableViewInfo table) {
			int count = table.RowCount;
			for (int i = 0; i < count; i++) {
				TableRowViewInfoBase rowInfo = table.Rows[i];
				SnapBookmark bookmark = TableRowHelper.FindTemplateBookmark(rowInfo.Row);
				if (bookmark == null)
					continue;
				SnapTemplateInfo templateInfo = bookmark.TemplateInterval.TemplateInfo;
				if (templateInfo.TemplateType == SnapTemplateIntervalType.DataRow)
					return rowInfo;
				else {
					TableRowViewInfoBase dataRow = FindNearestDataRow(rowInfo);
					if (dataRow != null)
						while (rowInfo != null) {
							if (TableHelper.CompareRowsByGrid(rowInfo.Row, dataRow.Row))
								return rowInfo;
							rowInfo = rowInfo.Next;
						}
				}
			}
			return table.Rows.First;
		}
		TableRowViewInfoBase FindNearestDataRow(TableRowViewInfoBase headerRow) {
			TableRowViewInfoBase next = headerRow.Next;
			while (next != null) {
				SnapBookmark nextBookmark = TableRowHelper.FindTemplateBookmark(next.Row);
				if (nextBookmark != null && nextBookmark.TemplateInterval.TemplateInfo.TemplateType == SnapTemplateIntervalType.DataRow)
					return next;
				next = next.Next;
			}
			return next;
		}
	}
	public class VisibleDropFieldTableHotZoneCalculator {
		const double minBorderDistance = 20;
		public HotZone CalcVisibleHotZone(HotZoneCollection hotZones, Point mousePosition) {
			foreach (DropFieldTableHotZoneBase hotZone in hotZones) {
				Rectangle renderBounds = hotZone.Bounds;
				if (renderBounds.Right == renderBounds.Left) {
					if (renderBounds.Top - minBorderDistance <= mousePosition.Y && renderBounds.Bottom + minBorderDistance >= mousePosition.Y)
						if (Math.Abs(renderBounds.X - mousePosition.X) <= minBorderDistance)
							return hotZone;
				}
				else {
					if (renderBounds.Left - minBorderDistance <= mousePosition.X && renderBounds.Right + minBorderDistance >= mousePosition.X)
						if (Math.Abs(renderBounds.Top - mousePosition.Y) <= minBorderDistance)
							return hotZone;
				}
			}
			return null;
		}
	}
}
