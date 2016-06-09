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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Fields;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using xtraPrinting = DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Snap.API.Native;
using DevExpress.XtraGrid.Views.BandedGrid;
using System.Drawing;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using XtrModel = DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils.Commands;
using DevExpress.Utils;
using XGViewPrinting = DevExpress.XtraGrid.Views.Printing;
using XGPrinting = DevExpress.XtraGrid.Printing;
using XPrinting = DevExpress.XtraPrinting;
using XEditors = DevExpress.XtraEditors;
using RegExp = System.Text.RegularExpressions;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Native.GridControlHelper {
	public static class GridControlHelper {
		#region Auxiliary classes
		class GridElementsBaseProp {
			public GridElementsBaseProp() { }
			private int columnSpan = 0;
			public int ColumnSpan {
				get { return columnSpan; }
				set { columnSpan = value; }
			}
			private int rowSpan = 0;
			public int RowSpan {
				get { return rowSpan; }
				set { rowSpan = value; }
			}
			private int columnIndex = 0;
			public int ColumnIndex {
				get { return columnIndex; }
				set { columnIndex = value; }
			}
			private int rowIndex = 0;
			public int RowIndex {
				get { return rowIndex; }
				set { rowIndex = value; }
			}
			private int width = 0;
			public int Width {
				get { return width; }
				set {
					if(width != value)
						ResetWidthInPercent();
					width = value;
				}
			}
			private int tableWidth = 0;
			public int TableWidth {
				get { return tableWidth; }
				set {
					if(tableWidth != value)
						ResetWidthInPercent();
					tableWidth = value;
				}
			}
			private float widthInPercent = -1;
			public float WidthInPercent {
				get {
					if(widthInPercent < 0)
						widthInPercent = Width * 100 / TableWidth;
					return widthInPercent;
				}
			}
			private void ResetWidthInPercent() {
				widthInPercent = -1;
			}
		}
		class GridBandProp : GridElementsBaseProp {
			public GridBandProp() { }
		}
		class GridColumnProp : GridElementsBaseProp {
			public GridColumnProp() { }
			private int ownerBandStartIndex = -1;
			public int OwnerBandStartIndex {
				get { return ownerBandStartIndex; }
				set { ownerBandStartIndex = value; }
			}
			private int ownerBandEndIndex = -1;
			public int OwnerBandEndIndex {
				get { return ownerBandEndIndex; }
				set { ownerBandEndIndex = value; }
			}
		}
		class TupleWraper {
			private TupleWraper() { }
			public TupleWraper(object tuple) {
				bandCell = tuple as Tuple<GridBand, GridBandProp>;
				contentCell = tuple as Tuple<ColumnInfo, GridColumnProp>;
			}
			private Tuple<GridBand, GridBandProp> bandCell = null;
			private Tuple<ColumnInfo, GridColumnProp> contentCell = null;
			public bool AutoFillDown {
				get {
					if(bandCell != null)
						return bandCell.Item1.AutoFillDown;
					if(contentCell != null) {
						BandedGridColumn bandGridClmn = contentCell.Item1.Column as BandedGridColumn;
						return bandGridClmn != null && bandGridClmn.AutoFillDown;
					}
					return false;
				}
			}
			public int ColumnSpan {
				get {
					if(bandCell != null)
						return bandCell.Item2.ColumnSpan;
					if(contentCell != null)
						return contentCell.Item2.ColumnSpan;
					return 0;
				}
				set {
					if(bandCell != null)
						bandCell.Item2.ColumnSpan = value;
					if(contentCell != null)
						contentCell.Item2.ColumnSpan = value;
				}
			}
			public int RowSpan {
				get {
					if(bandCell != null)
						return bandCell.Item2.RowSpan;
					if(contentCell != null)
						return contentCell.Item2.RowSpan;
					return 0;
				}
				set {
					if(bandCell != null)
						bandCell.Item2.RowSpan = value;
					if(contentCell != null)
						contentCell.Item2.RowSpan = value;
				}
			}
			public int RowCount {
				get {
					if(bandCell != null)
						return bandCell.Item1.RowCount;
					if(contentCell != null) {
						BandedGridColumn bndGrColumn = contentCell.Item1.Column as BandedGridColumn;
						if(bndGrColumn != null)
							return bndGrColumn.RowCount;
					}
					return 1;
				}
			}
			public int ColumnIndex {
				get {
					if(bandCell != null)
						return bandCell.Item2.ColumnIndex;
					if(contentCell != null)
						return contentCell.Item2.ColumnIndex;
					return 0;
				}
				set {
					if(bandCell != null)
						bandCell.Item2.ColumnIndex = value;
					if(contentCell != null)
						contentCell.Item2.ColumnIndex = value;
				}
			}
			public int RowIndex {
				get {
					if(bandCell != null)
						return bandCell.Item2.RowIndex;
					if(contentCell != null)
						return contentCell.Item2.RowIndex;
					return 0;
				}
				set {
					if(bandCell != null)
						bandCell.Item2.RowIndex = value;
					if(contentCell != null)
						contentCell.Item2.RowIndex = value;
				}
			}
			public string Caption {
				get {
					if(bandCell != null && bandCell.Item1.OptionsBand.ShowCaption)
						return bandCell.Item1.GetTextCaption();
					if(contentCell != null && contentCell.Item1.Column.OptionsColumn.ShowCaption)
						return contentCell.Item1.Column.GetTextCaption();
					return String.Empty;
				}
			}
		}
		class TableDimension {
			public TableDimension() { }
			public int ColumnCount { get; set; }
			public int RowCount { get; set; }
		}
		class GridTableInfo {
			public GridTableInfo() { }
			private Tuple<GridBand, GridBandProp>[,] bandsHeaderTable = null;
			private Tuple<ColumnInfo, GridColumnProp>[,] contentColumnsTable = null;
			public TableDimension BandsHeaderTableDimension { get; set; }
			public TableDimension ContentColumnsTableDimension { get; set; }
			public Tuple<GridBand, GridBandProp>[,] BandsHeaderTable {
				get { return bandsHeaderTable; }
				set { bandsHeaderTable = value; }
			}
			public Tuple<ColumnInfo, GridColumnProp>[,] ContentColumnsTable {
				get { return contentColumnsTable; }
				set { contentColumnsTable = value; }
			}
			private bool? isThereSummary;
			public bool IsThereSummary {
				get {
					if(isThereSummary.HasValue)
						return isThereSummary.Value;
					isThereSummary = false;
					if(ContentColumnsTableDimension.ColumnCount == 0 || ContentColumnsTableDimension.RowCount == 0)
						return false;
					for(int i = 0; i < ContentColumnsTableDimension.ColumnCount; i++) {
						for(int j = 0; j < ContentColumnsTableDimension.RowCount; j++) {
							Tuple<ColumnInfo, GridColumnProp> columnWithProp = ContentColumnsTable[i, j];
							if(columnWithProp != null && columnWithProp.Item1.Column.Summary.Count != 0) {
								isThereSummary = true;
								return true;
							}
						}
					}
					return false;
				}
			}
			private int tableWidth = 0;
			public int TableWidth {
				get { return tableWidth; }
				set { tableWidth = value; }
			}
			private List<float> columnsWidth = new List<float>();
			public void CalcColumnsWidth() {
				columnsWidth.Clear();
				if(ContentColumnsTableDimension == null || ContentColumnsTableDimension.ColumnCount == 0)
					return;
				for(int i = 0; i < ContentColumnsTableDimension.ColumnCount; i++) {
					float clmnWidth = 0;
					for(int j = 0; j < ContentColumnsTableDimension.RowCount; j++) {
						Tuple<ColumnInfo, GridColumnProp> contentColumn = ContentColumnsTable[i, j];
						if(contentColumn == null)
							continue;
						if(contentColumn.Item2.ColumnSpan != 0)
							continue;
						clmnWidth = Math.Max(clmnWidth, contentColumn.Item2.WidthInPercent);
					}
					columnsWidth.Add(clmnWidth);
				}
			}
			public float GetColumnWidthInPercent(int columnIndex) {
				if(columnIndex >= columnsWidth.Count)
					return -1;
				return columnsWidth[columnIndex];
			}
		}
		struct ColumnInfo {
			public GridColumn Column { get; set; }
			public bool IsFake { get; set; }
		}
		#endregion
		static readonly Regex summaryFormatFinder = new Regex(@"(\{0(?:(?:,\-?\d+)?(?::(.+))?)?\})");
		static readonly Regex groupFormatFinder = new Regex(@"\{(\d)\}");
		public static void ExportToSnapReport(this GridControl gridControl, SnapControl snapControl) {
			snapControl.CreateNewDocument();
			snapControl.DataSource = gridControl.DataSource;
			SnapDocument document = snapControl.Document;
			document.ModifyDefaultStyles(gridControl.LevelTree.LevelTemplate as GridView);
			document.CreateAdditionalStyles(gridControl.LevelTree.LevelTemplate as GridView);
			document.BeginUpdate();
			ExportGridContentToSnapReport(gridControl.LevelTree, document);
			document.EndUpdate();
		}
		static void ExportGridContentToSnapReport(GridLevelNode node, SnapDocument document) {
			Stack<Tuple<GridLevelNode, SnapList, SnapDocument>> grdLvlNodeStack = new Stack<Tuple<GridLevelNode, SnapList, SnapDocument>>();
			GridLevelNode curNode = node;
			while(curNode != null) {
				GridView grid = curNode.LevelTemplate as GridView;
				if(grid != null && grid.VisibleColumns.Count != 0) {
					SnapList list = document.CreateSnList(document.Range.End, grid.Name);
					list.BeginUpdate();
					GridTableInfo grTableInf = CalculateGridTableInfo(grid);
					ApplyDataSource(list, curNode);
					ApplyGroups(list, grid, grTableInf);
					ApplySorting(list, grid);
					ApplyFilter(list, grid);
					Table table = null;
					SnapDocument template = null;
					MakeTemplate(list, grid, grTableInf, out table, out template);
					if(grid.OptionsPrint.PrintFooter && grTableInf.IsThereSummary)
						MakeReportFooter(list, grid, grTableInf);
					if(grid.OptionsDetail.EnableMasterViewMode && curNode.HasChildren) {
						GridLevelNode firstNode = curNode.Nodes.FirstOrDefault<GridLevelNode>(e => e.LevelTemplate is GridView);
						if(firstNode != null) {
							TableRow detail = table.Rows.Append();
							table.MergeCells(detail.FirstCell, detail.LastCell);
							grdLvlNodeStack.Push(new Tuple<GridLevelNode, SnapList, SnapDocument>(curNode, list, document));
							curNode = firstNode;
							document = template;
							continue;
						}
					}
					list.ApplyTableStyles(curNode.Level + 1);
					list.ApplyAdditionalStyles(grid);
					list.EndUpdate();
				}
				do {
					GridLevelNode nextNode = curNode.NextNode();
					if(nextNode != null) {
						curNode = nextNode;
						break;
					}
					curNode = null;
					Tuple<GridLevelNode, SnapList, SnapDocument> lvlNodeAndList = grdLvlNodeStack.Count != 0 ? grdLvlNodeStack.Pop() : null;
					if(lvlNodeAndList != null) {
						curNode = lvlNodeAndList.Item1;
						lvlNodeAndList.Item2.ApplyTableStyles(curNode.Level + 1);
						lvlNodeAndList.Item2.ApplyAdditionalStyles(curNode.LevelTemplate as GridView);
						lvlNodeAndList.Item2.EndUpdate();
						document = lvlNodeAndList.Item3;
					}
				} while(curNode != null);
			}
		}
		static void ApplyAdditionalStyles(this SnapList list, GridView grid) {
			if(grid != null && !grid.IsTherePreviewRow())
				return;
			SnapNativeDocument template = (SnapNativeDocument)list.RowTemplate;
			NativeSnapList nativeList = (NativeSnapList)list;
			template.BeginUpdate();
			try {
				template.DocumentModel.BeginUpdate();
				try {
					SnapDocumentModel styleSourceModel = nativeList.Document.DocumentModel;
					SnapDocumentModel result = template.DocumentModel;
					XtrModel.TableCellStyle previewTblCellStyle = styleSourceModel.TableCellStyles.GetStyleByName("Preview");
					if(previewTblCellStyle != null) {
						XtrModel.TableCellStyle newStyle = previewTblCellStyle.CopyTo(result);
						foreach(var table in result.MainPieceTable.Tables) {
							table.Rows.Last.Cells.ForEach(cell => DevExpress.Snap.Core.Native.StyleHelper.ApplyTableCellStyleCore(newStyle.StyleName, cell));
						}
					}
				}
				finally {
					template.DocumentModel.EndUpdate();
				}
			}
			finally {
				template.EndUpdate();
			}
		}
		static void CreateAdditionalStyles(this SnapDocument document, GridView grid) {
			XtrModel.DocumentModel docModel = ((SnapNativeDocument)document).DocumentModel;
			AppearanceDefaultInfo[] appearanceDefInfos = grid.BaseInfo.GetDefaultPrintAppearance();
			AppearanceDefaultInfo previewADefInfo = GetAppearanceDefaultInfoByName(appearanceDefInfos, "Preview");
			AppearanceDefaultInfo linesADefInfo = GetAppearanceDefaultInfoByName(appearanceDefInfos, "Lines");
			XtrModel.TableCellStyle previewStyle = new XtrModel.TableCellStyle(docModel, null, "Preview");
			ModifyTblCellStyle(previewStyle, previewADefInfo, linesADefInfo, grid.OptionsPrint.PrintHorzLines, grid.OptionsPrint.PrintVertLines);
			docModel.TableCellStyles.Add(previewStyle);
		}
		static void ModifyDefaultStyles(this SnapDocument document, GridView grid) {
			XtrModel.DocumentModel docModel = ((SnapNativeDocument)document).DocumentModel;
			AppearanceDefaultInfo[] appearanceDefInfos = grid.BaseInfo.GetDefaultPrintAppearance();
			AppearanceDefaultInfo headerPanelADefInfo = GetAppearanceDefaultInfoByName(appearanceDefInfos, "HeaderPanel");
			AppearanceDefaultInfo footerPanelADefInfo = GetAppearanceDefaultInfoByName(appearanceDefInfos, "FooterPanel");
			AppearanceDefaultInfo filterPanelADefInfo = GetAppearanceDefaultInfoByName(appearanceDefInfos, "FilterPanel");
			AppearanceDefaultInfo rowADefInfo = GetAppearanceDefaultInfoByName(appearanceDefInfos, "Row");
			AppearanceDefaultInfo linesADefInfo = GetAppearanceDefaultInfoByName(appearanceDefInfos, "Lines");
			AppearanceDefaultInfo groupHeaderPanelADefInfo = GetAppearanceDefaultInfoByName(appearanceDefInfos, "GroupRow");
			AppearanceDefaultInfo groupFooterPanelADefInfo = GetAppearanceDefaultInfoByName(appearanceDefInfos, "GroupFooter");
			for(int i = 1; i <= 2; i++) {
				string baseStyleName = String.Format("List{0}", i);
				XtrModel.TableStyle listStl = GetTblStyleByName(docModel.TableStyles, baseStyleName);
				ModifyTblStyle(listStl, rowADefInfo, linesADefInfo, grid.OptionsPrint.PrintHorzLines, grid.OptionsPrint.PrintVertLines);
				string listHeaderName = String.Format("{0}-Header", baseStyleName);
				XtrModel.TableCellStyle listHeaderStyle = GetTblCellStyleByName(docModel.TableCellStyles, listHeaderName);
				ModifyTblCellStyle(listHeaderStyle, headerPanelADefInfo, linesADefInfo, true, true);
				string listFooterName = String.Format("{0}-Footer", baseStyleName);
				XtrModel.TableCellStyle listFooterStyle = GetTblCellStyleByName(docModel.TableCellStyles, listFooterName);
				ModifyTblCellStyle(listFooterStyle, footerPanelADefInfo, filterPanelADefInfo, true, true);
				for(int j = 1; j <= 2; j++) {
					string listGroupHeaderName = String.Format("{0}-GroupHeader{1}", baseStyleName, j);
					XtrModel.TableCellStyle listGroupHeaderStyle = GetTblCellStyleByName(docModel.TableCellStyles, listGroupHeaderName);
					ModifyTblCellStyle(listGroupHeaderStyle, groupHeaderPanelADefInfo, linesADefInfo, true, true);
					string listGroupFooterName = String.Format("{0}-GroupFooter{1}", baseStyleName, j);
					XtrModel.TableCellStyle listGroupFooterStyle = GetTblCellStyleByName(docModel.TableCellStyles, listGroupFooterName);
					ModifyTblCellStyle(listGroupFooterStyle, groupFooterPanelADefInfo, linesADefInfo, true, true);
				}
			}
		}
		static void ModifyTblCellStyle(XtrModel.TableCellStyle tblCellStyle, AppearanceDefaultInfo appInfo, AppearanceDefaultInfo lineInfo, bool showHLines, bool showVLines) {
			tblCellStyle.BeginUpdate();
			Color hLineColor = showHLines ? lineInfo.DefaultAppearance.BackColor : appInfo.DefaultAppearance.BackColor;
			Color vLineColor = showVLines ? lineInfo.DefaultAppearance.BackColor : appInfo.DefaultAppearance.BackColor;
			tblCellStyle.TableCellProperties.Borders.LeftBorder.Color = vLineColor;
			tblCellStyle.TableCellProperties.Borders.TopBorder.Color = hLineColor;
			tblCellStyle.TableCellProperties.Borders.RightBorder.Color = vLineColor;
			tblCellStyle.TableCellProperties.Borders.BottomBorder.Color = hLineColor;
			tblCellStyle.TableCellProperties.Borders.LeftBorder.Width = 1;
			tblCellStyle.TableCellProperties.Borders.TopBorder.Width = 1;
			tblCellStyle.TableCellProperties.Borders.RightBorder.Width = 1;
			tblCellStyle.TableCellProperties.Borders.BottomBorder.Width = 1;
			tblCellStyle.TableCellProperties.Borders.LeftBorder.Style = XtrModel.BorderLineStyle.Single;
			tblCellStyle.TableCellProperties.Borders.TopBorder.Style = XtrModel.BorderLineStyle.Single;
			tblCellStyle.TableCellProperties.Borders.RightBorder.Style = XtrModel.BorderLineStyle.Single;
			tblCellStyle.TableCellProperties.Borders.BottomBorder.Style = XtrModel.BorderLineStyle.Single;
			tblCellStyle.TableCellProperties.Borders.InsideHorizontalBorder.Color = hLineColor;
			tblCellStyle.TableCellProperties.Borders.InsideHorizontalBorder.Width = 1;
			tblCellStyle.TableCellProperties.Borders.InsideHorizontalBorder.Style = XtrModel.BorderLineStyle.Single;
			tblCellStyle.TableCellProperties.Borders.InsideVerticalBorder.Color = vLineColor;
			tblCellStyle.TableCellProperties.Borders.InsideVerticalBorder.Width = 1;
			tblCellStyle.TableCellProperties.Borders.InsideVerticalBorder.Style = XtrModel.BorderLineStyle.Single;
			tblCellStyle.TableCellProperties.BackgroundColor = appInfo.DefaultAppearance.BackColor;
			tblCellStyle.TableCellProperties.ForegroundColor = appInfo.DefaultAppearance.ForeColor;
			tblCellStyle.CharacterProperties.ForeColor = appInfo.DefaultAppearance.ForeColor;
			tblCellStyle.TableCellProperties.VerticalAlignment = ConvertVAlignments(appInfo.DefaultAppearance.VAlignment);
			tblCellStyle.ParagraphProperties.Alignment = ConvertHAlignments(appInfo.DefaultAppearance.HAlignment);
			tblCellStyle.EndUpdate();
		}
		static void ModifyTblStyle(XtrModel.TableStyle tblStyle, AppearanceDefaultInfo rowInfo, AppearanceDefaultInfo lineInfo, bool showHLines, bool showVLines) {
			tblStyle.BeginUpdate();
			Color hLineColor = showHLines ? lineInfo.DefaultAppearance.ForeColor : rowInfo.DefaultAppearance.BackColor;
			Color vLineColor = showVLines ? lineInfo.DefaultAppearance.ForeColor : rowInfo.DefaultAppearance.BackColor;
			tblStyle.TableProperties.Borders.LeftBorder.Color = vLineColor;
			tblStyle.TableProperties.Borders.TopBorder.Color = hLineColor;
			tblStyle.TableProperties.Borders.RightBorder.Color = vLineColor;
			tblStyle.TableProperties.Borders.BottomBorder.Color = hLineColor;
			tblStyle.TableProperties.Borders.LeftBorder.Width = 1;
			tblStyle.TableProperties.Borders.TopBorder.Width = 1;
			tblStyle.TableProperties.Borders.RightBorder.Width = 1;
			tblStyle.TableProperties.Borders.BottomBorder.Width = 1;
			tblStyle.TableProperties.Borders.LeftBorder.Style = XtrModel.BorderLineStyle.Single;
			tblStyle.TableProperties.Borders.TopBorder.Style = XtrModel.BorderLineStyle.Single;
			tblStyle.TableProperties.Borders.RightBorder.Style = XtrModel.BorderLineStyle.Single;
			tblStyle.TableProperties.Borders.BottomBorder.Style = XtrModel.BorderLineStyle.Single;
			tblStyle.TableProperties.Borders.InsideHorizontalBorder.Color = hLineColor;
			tblStyle.TableProperties.Borders.InsideVerticalBorder.Color = vLineColor;
			tblStyle.TableProperties.Borders.InsideHorizontalBorder.Width = 1;
			tblStyle.TableProperties.Borders.InsideVerticalBorder.Width = 1;
			tblStyle.TableProperties.Borders.InsideHorizontalBorder.Style = XtrModel.BorderLineStyle.Single;
			tblStyle.TableProperties.Borders.InsideVerticalBorder.Style = XtrModel.BorderLineStyle.Single;
			tblStyle.TableProperties.BackgroundColor = rowInfo.DefaultAppearance.BackColor;
			tblStyle.CharacterProperties.ForeColor = rowInfo.DefaultAppearance.ForeColor;
			tblStyle.TableCellProperties.VerticalAlignment = ConvertVAlignments(rowInfo.DefaultAppearance.VAlignment);
			tblStyle.ParagraphProperties.Alignment = ConvertHAlignments(rowInfo.DefaultAppearance.HAlignment);
			tblStyle.EndUpdate();
		}
		static XtrModel.ParagraphAlignment ConvertHAlignments(HorzAlignment alignment) {
			if(alignment == HorzAlignment.Near)
				return XtrModel.ParagraphAlignment.Left;
			if(alignment == HorzAlignment.Center)
				return XtrModel.ParagraphAlignment.Center;
			if(alignment == HorzAlignment.Far)
				return XtrModel.ParagraphAlignment.Right;
			return XtrModel.ParagraphAlignment.Left;
		}
		static XtrModel.VerticalAlignment ConvertVAlignments(VertAlignment alignment) {
			if(alignment == VertAlignment.Top)
				return XtrModel.VerticalAlignment.Top;
			if(alignment == VertAlignment.Bottom)
				return XtrModel.VerticalAlignment.Bottom;
			return XtrModel.VerticalAlignment.Center;
		}
		static XtrModel.TableStyle GetTblStyleByName(XtrModel.TableStyleCollection tblStyleCollection, string styleName) {
			if(tblStyleCollection == null || tblStyleCollection.Count == 0 || String.IsNullOrEmpty(styleName))
				return null;
			for(int i = 0; i < tblStyleCollection.Count; i++) {
				if(String.Equals(tblStyleCollection[i].StyleName, styleName))
					return tblStyleCollection[i];
			}
			return null;
		}
		static XtrModel.TableCellStyle GetTblCellStyleByName(XtrModel.TableCellStyleCollection tblCellStyleCollection, string styleName) {
			if(tblCellStyleCollection == null || tblCellStyleCollection.Count == 0 || String.IsNullOrEmpty(styleName))
				return null;
			for(int i = 0; i < tblCellStyleCollection.Count; i++) {
				if(String.Equals(tblCellStyleCollection[i].StyleName, styleName))
					return tblCellStyleCollection[i];
			}
			return null;
		}
		static AppearanceDefaultInfo GetAppearanceDefaultInfoByName(AppearanceDefaultInfo[] appearanceDefInfos, string name) {
			if(appearanceDefInfos == null || appearanceDefInfos.Length == 0 || String.IsNullOrEmpty(name))
				return null;
			for(int i = 0; i < appearanceDefInfos.Length; i++) {
				if(String.Equals(appearanceDefInfos[i].Name, name))
					return appearanceDefInfos[i];
			}
			return null;
		}
		static void ApplyDataSource(SnapList list, GridLevelNode node) {
			if(node.IsRootLevel)
				list.DataMember = node.LevelTemplate.GridControl.DataMember;
			else
				list.DataMember = node.RelationName;
		}
		static void ApplySorting(SnapList list, GridView grid) {
			foreach(GridColumn col in grid.SortedColumns) {
				list.Sorting.Add(new SnapListGroupParam(col.FieldName, col.SortOrder));
			}
		}
		static void ApplyFilter(SnapList list, GridView grid) {
			string filter = grid.ActiveFilterString;
			if(!String.IsNullOrEmpty(filter))
				list.Filters.Add(filter);
		}
		static void ApplyGroups(SnapList list, GridView grid, GridTableInfo grTableInf) {
			bool alignGroupSummaryInGroupRow = grid.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True;
			foreach(GridColumn col in grid.GroupedColumns) {
				SnapListGroupInfo group = list.Groups.CreateSnapListGroupInfo(new SnapListGroupParam(col.FieldName, col.SortOrder));
				SnapDocument groupHeader = group.CreateHeader();
				int columnCount = alignGroupSummaryInGroupRow ? grTableInf.ContentColumnsTableDimension.ColumnCount : 1;
				Table grpHeaderTbl = groupHeader.Tables.Create(groupHeader.Range.End, 1, columnCount);
				GridTableInfo grTblInfoForAdjustSize = alignGroupSummaryInGroupRow ? grTableInf : null;
				AdjustSize(grpHeaderTbl, grTblInfoForAdjustSize);
				TableCell grpHeaderCell = grpHeaderTbl.Cell(0, 0);
				string colCaption = "{0}";
				string colValue = "{1}";
				string summary = "{2}";
				string imageText = "[#image]";
				string groupFormat = grid.GroupFormat.Replace(imageText, String.Empty);
				MatchCollection grPatterns = groupFormatFinder.Matches(groupFormat);
				int prevPos = 0;
				foreach(RegExp.Match pattern in grPatterns) {
					string textBefore = groupFormat.Substring(prevPos, pattern.Index - prevPos);
					prevPos = pattern.Index + pattern.Length;
					if(!String.IsNullOrEmpty(textBefore) && !alignGroupSummaryInGroupRow)
						groupHeader.InsertText(grpHeaderCell.ContentRange.End, textBefore);
					if(String.Equals(pattern.Value, colCaption) && !alignGroupSummaryInGroupRow)
						groupHeader.InsertText(grpHeaderCell.ContentRange.End, col.GetTextCaption());
					if(String.Equals(pattern.Value, colValue)) {
						SnapText snTxt = groupHeader.CreateSnText(grpHeaderCell.ContentRange.End, col.FieldName.CorrectFieldName());
						snTxt.ApplyTextFormat(col);
					}
					if(String.Equals(pattern.Value, summary))
						ApplySummary(grpHeaderTbl, group, grid, grTableInf);
				}
				if(prevPos < groupFormat.Length - 1 && !alignGroupSummaryInGroupRow) {
					string textAfter = groupFormat.Substring(prevPos, groupFormat.Length - prevPos);
					if(!String.IsNullOrEmpty(textAfter))
						groupHeader.InsertText(grpHeaderCell.ContentRange.End, textAfter);
				}
				list.Groups.Add(group);
			}
		}
		static bool ShoodCreateTableWithinGroupFooter(SnapDocument footer, GridTableInfo grTableInf) {
			if(footer == null)
				return false;
			if(footer.Tables.Count == 0)
				return true;
			if(footer.Tables.First.Rows.Count != grTableInf.ContentColumnsTableDimension.RowCount)
				return true;
			if(footer.Tables.First.Rows.First.Cells.Count != grTableInf.ContentColumnsTableDimension.ColumnCount)
				return true;
			return false;
		}
		static Table GetSummaryFooterTable(SnapListGroupInfo group, GridTableInfo grTableInf) {
			SnapDocument footer = group.Footer;
			Table footerTbl = null;
			if(footer == null)
				footer = group.CreateFooter();
			if(ShoodCreateTableWithinGroupFooter(footer, grTableInf)) {
				footerTbl = footer.Tables.Create(footer.Range.Start, grTableInf.ContentColumnsTableDimension.RowCount, grTableInf.ContentColumnsTableDimension.ColumnCount);
				AdjustSize(footerTbl, grTableInf);
			}
			else {
				footerTbl = footer.Tables.FirstOrDefault(t =>
									t.Rows.Count == grTableInf.ContentColumnsTableDimension.RowCount
								&& t.Rows.First.Cells.Count == grTableInf.ContentColumnsTableDimension.ColumnCount);
			}
			return footerTbl;
		}
		static void ApplySummary(Table headerTbl, SnapListGroupInfo group, GridView grid, GridTableInfo grTableInf) {
			bool alignGroupSummaryInGroupRow = grid.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True;
			Table footerTbl = null;
			if(ShoudCreateGroupsFooter(grid)) {
				if(alignGroupSummaryInGroupRow)
					footerTbl = headerTbl;
				else
					footerTbl = GetSummaryFooterTable(group, grTableInf);
			}
			int groupHeaderSummaryCount = 0;
			foreach(GridSummaryItem item in grid.GroupSummary) {
				GridGroupSummaryItem summary = item as GridGroupSummaryItem;
				if(summary == null)
					continue;
				if(summary.ShowInGroupColumnFooter == null) {
					TableCell grpHeaderCell = headerTbl.Cell(0, 0);
					if(groupHeaderSummaryCount != 0)
						group.Header.InsertText(grpHeaderCell.ContentRange.End, ", ");
					bool isGroup = !alignGroupSummaryInGroupRow;
					BuildSummaryTemplate(group.Header, grpHeaderCell, summary, SummaryRunning.Group, isGroup);
					groupHeaderSummaryCount++;
					continue;
				}
				if(!summary.ShowInGroupColumnFooter.Visible || footerTbl == null)
					continue;
				SnapDocument footer = alignGroupSummaryInGroupRow ? group.Header : group.Footer;
				TableCell footerTblCell = GetTableCellByGridColumn(summary.ShowInGroupColumnFooter, footerTbl, grTableInf);
				if(footerTblCell == null)
					continue;
				BuildSummaryTemplate(footer, footerTblCell, summary, SummaryRunning.Group, false);
			}
			if(footerTbl != null)
				MergeCells(grTableInf.ContentColumnsTable, footerTbl, 0, grTableInf.ContentColumnsTableDimension);
		}
		static TableCell GetTableCellByGridColumn(GridColumn grdColumn, Table footerTbl, GridTableInfo grTableInf) {
			if(grdColumn == null || footerTbl == null || grTableInf == null)
				return null;
			for(int i = 0; i < grTableInf.ContentColumnsTableDimension.RowCount; i++) {
				for(int j = 0; j < grTableInf.ContentColumnsTableDimension.ColumnCount; j++) {
					Tuple<ColumnInfo, GridColumnProp> grClmProp = grTableInf.ContentColumnsTable[j, i];
					if(grClmProp == null)
						continue;
					if(grClmProp.Item1.Column == grdColumn)
						return footerTbl.Cell(i, j);
				}
			}
			return null;
		}
		static bool ShoudCreateGroupsFooter(GridView grdView) {
			if(grdView == null || grdView.GroupSummary.Count == 0)
				return false;
			foreach(GridSummaryItem item in grdView.GroupSummary) {
				GridGroupSummaryItem summary = item as GridGroupSummaryItem;
				if(summary == null)
					continue;
				if(summary.ShowInGroupColumnFooter != null)
					return true;
			}
			return false;
		}
		static void AdjustSize(Table table, GridTableInfo grTableInf) {
			table.SetPreferredWidth(50 * 100, WidthType.FiftiethsOfPercent);
			table.TableLayout = TableLayoutType.Fixed;
			if(grTableInf != null)
				AdjustSizeForEachCell(table, grTableInf);
			else
				table.ForEachCell(AssignPreferredWidth);
		}
		static void AssignPreferredWidth(TableCell cell, int rowIndex, int cellIndex) {
			cell.PreferredWidthType = WidthType.FiftiethsOfPercent;
			cell.PreferredWidth = (float)(50.0 * 100.0 / cell.Row.Cells.Count);
		}
		static void AdjustSizeForEachCell(Table table, GridTableInfo grTableInf) {
			TableRowCollection rows = table.Rows;
			int rowCount = rows.Count;
			for(int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
				TableCellCollection cells = rows[rowIndex].Cells;
				int cellCount = cells.Count;
				for(int cellIndex = 0; cellIndex < cellCount; cellIndex++) {
					TableCell cell = cells[cellIndex];
					cell.PreferredWidthType = WidthType.FiftiethsOfPercent;
					cell.PreferredWidth = grTableInf.GetColumnWidthInPercent(cellIndex) * 50;
				}
			}
		}
		static void BuildSummaryTemplate(SnapDocument template, TableCell cell, GridSummaryItem source, SummaryRunning running, bool group) {
			string displayFormat = source.DisplayFormat;
			if(string.IsNullOrEmpty(displayFormat))
				displayFormat = source.GetDisplayFormatByType(source.SummaryType, group);
			RegExp.Match match = summaryFormatFinder.Match(displayFormat);
			string beforeField = GetStringBeforeField(displayFormat, match);
			if(!String.IsNullOrEmpty(beforeField))
				template.InsertText(cell.ContentRange.End, beforeField);
			string fieldName = source.FieldName.Contains(" ") ? String.Format("\"{0}\"", source.FieldName) : source.FieldName;
			SnapText snText = template.CreateSnText(cell.ContentRange.End, fieldName);
			snText.BeginUpdate();
			snText.SummaryRunning = running;
			snText.SummaryFunc = source.SummaryType;
			snText.FormatString = GetFormatString(match, displayFormat);
			snText.EndUpdate();
			string textAfter = GetStringAfterField(displayFormat, match);
			if(!String.IsNullOrEmpty(textAfter))
				template.InsertText(cell.ContentRange.End, textAfter);
		}
		static string GetStringBeforeField(string displayFormat, RegExp.Match match) {
			if(String.IsNullOrEmpty(displayFormat) || match == null || match.Groups.Count == 0)
				return string.Empty;
			return displayFormat.Substring(0, match.Groups[0].Index);
		}
		static string GetStringAfterField(string displayFormat, RegExp.Match match) {
			if(String.IsNullOrEmpty(displayFormat) || match == null || match.Groups.Count == 0)
				return string.Empty;
			int startPos = match.Groups[0].Index + match.Groups[0].Length;
			int length = displayFormat.Length - startPos;
			return displayFormat.Substring(startPos, length);
		}
		static string GetFormatString(RegExp.Match match, string sourceDisplayFormat) {
			if(match == null || match.Groups.Count == 0)
				return sourceDisplayFormat;
			return match.Groups[2].Value;
		}
		static Type GetColType(GridColumn gridCol) {
			return gridCol.ColumnType;
		}
		static GridTableInfo CalculateGridTableInfo(GridView grid) {
			GridTableInfo tableInfo = new GridTableInfo();
			InitTableInfo(grid, tableInfo);
			CalcColumnAndRowSpans(tableInfo.BandsHeaderTable, tableInfo.BandsHeaderTableDimension);
			if(grid is BandedGridView)
				CalcColumnAndRowSpans(tableInfo.ContentColumnsTable, tableInfo.ContentColumnsTableDimension);
			int tableWidht = CalculateTableWidthByBands(tableInfo.BandsHeaderTable, tableInfo.BandsHeaderTableDimension);
			if(tableWidht == 0)
				tableWidht = CalculateTableWidthByColumns(grid);
			SetTableWidthToAllElement(tableInfo, tableWidht);
			tableInfo.CalcColumnsWidth();
			tableInfo.TableWidth = tableWidht;
			return tableInfo;
		}
		static void CalcColumnAndRowSpans(object[,] table, TableDimension tblDim) {
			int currentBandStartIndex = 0;
			int nextBandStartIndex = 0;
			do {
				nextBandStartIndex = GetNextRootBandStartIndex(table, tblDim, currentBandStartIndex);
				int currentBandEndIndex = nextBandStartIndex > 0 ? nextBandStartIndex - 1 : tblDim.ColumnCount - 1;
				for(int i = 0; i < tblDim.RowCount; i++) {
					int curCellColIndex = currentBandStartIndex;
					int nextCellColIndex = 0;
					do {
						nextCellColIndex = GetNextCellColumnIndexInRowWithinBand(table, currentBandStartIndex, currentBandEndIndex, i, curCellColIndex);
						int curCellEndColIndex = nextCellColIndex >= 0 ? nextCellColIndex - 1 : currentBandEndIndex;
						if(curCellEndColIndex >= 0) {
							if(IsCellValid(table, curCellColIndex, i)) {
								TupleWraper tupleWrap = new TupleWraper(table[curCellColIndex, i]);
								int cellCount = curCellEndColIndex - tupleWrap.ColumnIndex;
								System.Diagnostics.Debug.Assert(cellCount >= 0);
								if(cellCount > 0)
									tupleWrap.ColumnSpan = cellCount + 1;
							}
						}
						curCellColIndex = nextCellColIndex;
					} while(nextCellColIndex >= 0);
				}
				for(int i = 0; i < tblDim.RowCount; i++) {
					int curCellColumnIndex = GetFirstCellColumnIndexInRowWithinBand(table, currentBandStartIndex, currentBandEndIndex, i);
					while(curCellColumnIndex >= 0) {
						TupleWraper tupleWrap = new TupleWraper(table[curCellColumnIndex, i]);
						if(tupleWrap.AutoFillDown) {
							int endRowIndexForSpan = i;
							for(int j = i + 1; j < tblDim.RowCount; j++) {
								int curCellEndColumnIndex = curCellColumnIndex + tupleWrap.ColumnSpan - 1;
								if(IsThereCellWithinRange(table, currentBandStartIndex, currentBandEndIndex, j, curCellColumnIndex, curCellEndColumnIndex))
									break;
								endRowIndexForSpan = j;
							}
							int rowCountWithinCell = endRowIndexForSpan - i;
							System.Diagnostics.Debug.Assert(rowCountWithinCell >= 0);
							if(rowCountWithinCell > 0)
								tupleWrap.RowSpan = rowCountWithinCell + 1;
						}
						else if(tupleWrap.RowCount > 1)
							tupleWrap.RowSpan = tupleWrap.RowCount;
						curCellColumnIndex = GetNextCellColumnIndexInRowWithinBand(table, currentBandStartIndex, currentBandEndIndex, i, curCellColumnIndex);
					}
				}
				currentBandStartIndex = nextBandStartIndex;
			} while(nextBandStartIndex >= 0);
		}
		static int CalculateTableWidthByBands(Tuple<GridBand, GridBandProp>[,] bandTable, TableDimension tblDim) {
			if(tblDim.ColumnCount == 0)
				return 0;
			int res = 0;
			int currentBandStartIndex = 0;
			int nextBandStartIndex = 0;
			do {
				nextBandStartIndex = GetNextRootBandStartIndex(bandTable, tblDim, currentBandStartIndex);
				if(bandTable[currentBandStartIndex, 0].Item1.Visible)
					res += bandTable[currentBandStartIndex, 0].Item1.Width;
				currentBandStartIndex = nextBandStartIndex;
			} while(nextBandStartIndex >= 0);
			return res;
		}
		static int CalculateTableWidthByColumns(GridView grid) {
			int res = 0;
			foreach(GridColumn column in grid.VisibleColumns)
				res += column.Width;
			return res;
		}
		static void SetTableWidthToAllElement(GridTableInfo tableInfo, int tableWidth) {
			for(int i = 0; i < tableInfo.BandsHeaderTableDimension.ColumnCount; i++) {
				for(int j = 0; j < tableInfo.BandsHeaderTableDimension.RowCount; j++) {
					if(tableInfo.BandsHeaderTable[i, j] != null)
						tableInfo.BandsHeaderTable[i, j].Item2.TableWidth = tableWidth;
				}
			}
			for(int i = 0; i < tableInfo.ContentColumnsTableDimension.ColumnCount; i++) {
				for(int j = 0; j < tableInfo.ContentColumnsTableDimension.RowCount; j++) {
					if(tableInfo.ContentColumnsTable[i, j] != null)
						tableInfo.ContentColumnsTable[i, j].Item2.TableWidth = tableWidth;
				}
			}
		}
		static bool IsThereCellWithinRange(object[,] table, int bandStartIndex, int bandEndIndex, int rowIndex, int startColumnIndex, int endColumnIndex) {
			if(startColumnIndex == endColumnIndex) {
				if(table[startColumnIndex, rowIndex] != null)
					return true;
			}
			int prevCellStartColumnIndex = GetPrevCellColumnIndexInRowWithinBand(table, bandStartIndex, bandEndIndex, rowIndex, endColumnIndex);
			prevCellStartColumnIndex = prevCellStartColumnIndex < 0 ? GetFirstCellColumnIndexInRowWithinBand(table, bandStartIndex, bandEndIndex, rowIndex) : prevCellStartColumnIndex;
			if(prevCellStartColumnIndex >= 0) {
				int prevCellColumnSpan = GetColumnSpan(table, prevCellStartColumnIndex, rowIndex);
				int prevCellEndColumnIndex = prevCellColumnSpan == 0 ? prevCellStartColumnIndex : prevCellStartColumnIndex + prevCellColumnSpan - 1;
				if(AreRangesIntersect(startColumnIndex, endColumnIndex, prevCellStartColumnIndex, prevCellEndColumnIndex))
					return true;
			}
			int nextCellStartColumnIndex = GetNextCellColumnIndexInRowWithinBand(table, bandStartIndex, bandEndIndex, rowIndex, startColumnIndex);
			if(nextCellStartColumnIndex >= 0) {
				if(IsIndexWithinRange(nextCellStartColumnIndex, startColumnIndex, endColumnIndex))
					return true;
			}
			return false;
		}
		static int GetColumnSpan(object[,] table, int columnIndex, int rowIndex) {
			TupleWraper tpWrap = new TupleWraper(table[columnIndex, rowIndex]);
			return tpWrap.ColumnSpan;
		}
		static int GetRowSpan(object[,] table, int columnIndex, int rowIndex) {
			TupleWraper tpWrap = new TupleWraper(table[columnIndex, rowIndex]);
			return tpWrap.RowSpan;
		}
		static bool AreRangesIntersect(int range1Start, int range1End, int range2Start, int range2End) {
			if(IsIndexWithinRange(range1Start, range2Start, range2End))
				return true;
			if(IsIndexWithinRange(range1End, range2Start, range2End))
				return true;
			if(IsIndexWithinRange(range2Start, range1Start, range1End))
				return true;
			return false;
		}
		static bool IsIndexWithinRange(int index, int rangeStart, int rangeEnd) {
			return rangeStart <= index && index <= rangeEnd;
		}
		static int GetNextRootBandStartIndex(object[,] table, TableDimension tblDim, int startIndex) {
			Tuple<ColumnInfo, GridColumnProp>[,] contentTable = table as Tuple<ColumnInfo, GridColumnProp>[,];
			if(contentTable != null) {
				int currentBandEndIndex = contentTable[startIndex, 0].Item2.OwnerBandEndIndex;
				return currentBandEndIndex != tblDim.ColumnCount - 1 ? currentBandEndIndex + 1 : -1;
			}
			for(int i = startIndex + 1; i < tblDim.ColumnCount; i++) {
				if(IsCellValid(table, i, 0))
					return i;
			}
			return -1;
		}
		static int GetNextCellColumnIndexInRowWithinBand(object[,] table, int bandStartIndex, int bandEndIndex, int rowIndex, int colIndex) {
			if(colIndex < bandStartIndex)
				colIndex = bandStartIndex;
			if(colIndex >= bandEndIndex)
				return -1;
			for(int i = colIndex + 1; i <= bandEndIndex; i++) {
				if(IsCellValid(table, i, rowIndex))
					return i;
			}
			return -1;
		}
		static int GetPrevCellColumnIndexInRowWithinBand(object[,] table, int bandStartIndex, int bandEndIndex, int rowIndex, int colIndex) {
			if(colIndex <= bandStartIndex)
				return -1;
			if(colIndex > bandEndIndex)
				colIndex = bandEndIndex;
			for(int i = colIndex - 1; i >= bandStartIndex; i--) {
				if(IsCellValid(table, i, rowIndex))
					return i;
			}
			return -1;
		}
		static int GetFirstCellColumnIndexInRowWithinBand(object[,] table, int bandStartIndex, int bandEndIndex, int rowIndex) {
			for(int i = bandStartIndex; i <= bandEndIndex; i++) {
				if(IsCellValid(table, i, rowIndex))
					return i;
			}
			return -1;
		}
		static void InitTableInfo(GridView grid, GridTableInfo tableInfo) {
			tableInfo.BandsHeaderTableDimension = new TableDimension();
			tableInfo.ContentColumnsTableDimension = new TableDimension();
			Dictionary<GridBand, GridBandProp> bandsWithProp = new Dictionary<GridBand, GridBandProp>();
			Dictionary<ColumnInfo, GridColumnProp> columnWithProp = new Dictionary<ColumnInfo, GridColumnProp>();
			BandedGridView bandGridView = grid as BandedGridView;
			if(bandGridView != null) {
				int curRowIndex = 0;
				int curColumnIndex = 0;
				Stack<GridBand> bandsStack = new Stack<GridBand>();
				GridBand curBand = bandGridView.Bands.FirstVisibleBand;
				while(curBand != null) {
					ProcessGridBand(curBand, tableInfo.BandsHeaderTableDimension, curColumnIndex, curRowIndex, bandsWithProp);
					if(curBand.HasChildren) {
						GridBand firstVisibleBnd = curBand.Children.FirstVisibleBand;
						if(firstVisibleBnd != null) {
							bandsStack.Push(curBand);
							curBand = firstVisibleBnd;
							curRowIndex = bandsStack.Count;
							continue;
						}
					}
					ProcessBandedGridColumns(curBand, tableInfo.ContentColumnsTableDimension, columnWithProp);
					curColumnIndex = tableInfo.ContentColumnsTableDimension.ColumnCount > 0 ? tableInfo.ContentColumnsTableDimension.ColumnCount - 1 : curColumnIndex;
					do {
						GridBand nextBand = curBand.NextVisibleBand();
						if(nextBand != null) {
							curBand = nextBand;
							curColumnIndex++;
							break;
						}
						curBand = bandsStack.Count != 0 ? bandsStack.Pop() : null;
						curRowIndex = bandsStack.Count;
					} while(curBand != null);
				}
				tableInfo.BandsHeaderTableDimension.ColumnCount = tableInfo.ContentColumnsTableDimension.ColumnCount;
			}
			else {
				int groupedColCount = 0;
				bool alignGroupSummaryInGroupRow = grid.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True;
				int colCount = alignGroupSummaryInGroupRow ? grid.GroupedColumns.Count : 0;
				for(int i = 0; i < grid.VisibleColumns.Count; i++) {
					GridColumn col = grid.VisibleColumns[i];
					if(!ShouldShowColumn(col, grid))
						continue;
					if(grid.GroupedColumns.Contains(col) && alignGroupSummaryInGroupRow) {
						columnWithProp.Add(new ColumnInfo() { Column = col }, new GridColumnProp() { RowIndex = 0, ColumnIndex = groupedColCount, Width = col.Width });
						groupedColCount++;
					}
					else {
						columnWithProp.Add(new ColumnInfo() { Column = col }, new GridColumnProp() { RowIndex = 0, ColumnIndex = colCount, Width = col.Width });
						colCount++;
					}
				}
				tableInfo.BandsHeaderTableDimension.ColumnCount = 0;
				tableInfo.BandsHeaderTableDimension.RowCount = 0;
				tableInfo.ContentColumnsTableDimension.ColumnCount = colCount;
				tableInfo.ContentColumnsTableDimension.RowCount = 1;
			}
			tableInfo.BandsHeaderTable = new Tuple<GridBand, GridBandProp>[tableInfo.BandsHeaderTableDimension.ColumnCount, tableInfo.BandsHeaderTableDimension.RowCount];
			tableInfo.ContentColumnsTable = new Tuple<ColumnInfo, GridColumnProp>[tableInfo.ContentColumnsTableDimension.ColumnCount, tableInfo.ContentColumnsTableDimension.RowCount];
			foreach(KeyValuePair<GridBand, GridBandProp> bandAndProp in bandsWithProp)
				tableInfo.BandsHeaderTable[bandAndProp.Value.ColumnIndex, bandAndProp.Value.RowIndex] = new Tuple<GridBand, GridBandProp>(bandAndProp.Key, bandAndProp.Value);
			foreach(KeyValuePair<ColumnInfo, GridColumnProp> colAndProp in columnWithProp)
				tableInfo.ContentColumnsTable[colAndProp.Value.ColumnIndex, colAndProp.Value.RowIndex] = new Tuple<ColumnInfo, GridColumnProp>(colAndProp.Key, colAndProp.Value);
		}
		static bool IsTherePreviewRow(this GridView view) {
			return view.OptionsPrint.PrintPreview && !String.IsNullOrEmpty(view.PreviewFieldName);
		}
		static void ProcessGridBand(GridBand grBand, TableDimension bandsDim, int columnIndex, int rowIndex, Dictionary<GridBand, GridBandProp> bandsWithProp) {
			bandsDim.RowCount = Math.Max(bandsDim.RowCount, grBand.BandLevel + 1);
			bandsWithProp.Add(grBand, new GridBandProp() { RowIndex = rowIndex, ColumnIndex = columnIndex, Width = grBand.Width });
		}
		static void ProcessBandedGridColumns(GridBand grBand, TableDimension contentDim, Dictionary<ColumnInfo, GridColumnProp> columnWithProp) {
			if(grBand.Columns.VisibleColumnCount == 0)
				return;
			Dictionary<int, List<BandedGridColumn>> bandClmsByRows = GetBandedColumnsGroupedByRows(grBand);
			int curRowCount = bandClmsByRows.Count;
			List<int> columnsBorderPositions = CalcColumnsBorderPositions(bandClmsByRows);
			int curColCount = columnsBorderPositions.Count - 1;
			int ownerBandStartIndex = contentDim.ColumnCount;
			int ownerBandEndIndex = contentDim.ColumnCount + curColCount - 1;
			for(int i = 0; i < curRowCount; i++) {
				List<BandedGridColumn> row = bandClmsByRows[i];
				int sumColWidthInCurRow = 0;
				for(int j = 0; j < row.Count; j++) {
					BandedGridColumn col = row[j];
					int colIndex = CalcColumnIndex(columnsBorderPositions, sumColWidthInCurRow);
					int colWidth = CalcColumnWidth(columnsBorderPositions, colIndex);
					ColumnInfo columnInfo = new ColumnInfo() { Column = col };
					GridColumnProp columnProp = new GridColumnProp() { RowIndex = i, ColumnIndex = colIndex + contentDim.ColumnCount, OwnerBandStartIndex = ownerBandStartIndex, OwnerBandEndIndex = ownerBandEndIndex, Width = colWidth };
					columnWithProp.Add(columnInfo, columnProp);
					sumColWidthInCurRow += col.Width;
					int nextColIndex = CalcColumnIndex(columnsBorderPositions, sumColWidthInCurRow);
					if(nextColIndex - colIndex > 1) {
						for(int s = colIndex + 1; s < nextColIndex; s++) {
							BandedGridColumn fakeCol = new BandedGridColumn();
							int spanColWidth = CalcColumnWidth(columnsBorderPositions, s);
							ColumnInfo fakeColumnInf = new ColumnInfo() { Column = fakeCol, IsFake = true };
							GridColumnProp fakeColumnProp = new GridColumnProp() { RowIndex = i, ColumnIndex = s + contentDim.ColumnCount, OwnerBandStartIndex = ownerBandStartIndex, OwnerBandEndIndex = ownerBandEndIndex, Width = spanColWidth };
							columnWithProp.Add(fakeColumnInf, fakeColumnProp);
						}
					}
				}
			}
			contentDim.ColumnCount += curColCount;
			contentDim.RowCount = Math.Max(contentDim.RowCount, curRowCount);
		}
		static List<int> CalcColumnsBorderPositions(Dictionary<int, List<BandedGridColumn>> bandClmsByRows) {
			List<int> columnBordersPositions = new List<int>() { 0 };
			for(int i = 0; i < bandClmsByRows.Count; i++) {
				List<BandedGridColumn> row = bandClmsByRows[i];
				int totalColumnWidth = 0;
				for(int j = 0; j < row.Count; j++) {
					totalColumnWidth += row[j].Width;
					if(columnBordersPositions.Contains(totalColumnWidth))
						continue;
					columnBordersPositions.Add(totalColumnWidth);
				}
			}
			columnBordersPositions.Sort();
			return columnBordersPositions;
		}
		static int CalcColumnIndex(List<int> colBordersPositions, int sumColWidht) {
			if(colBordersPositions.Count == 0)
				return -1;
			for(int i = 0; i < colBordersPositions.Count; i++) {
				if(sumColWidht == colBordersPositions[i])
					return i;
			}
			return -1;
		}
		static int CalcColumnWidth(List<int> colBordersPositions, int colIndex) {
			if(colBordersPositions.Count == 0 || colIndex == colBordersPositions.Count - 1)
				return 0;
			return colBordersPositions[colIndex + 1] - colBordersPositions[colIndex];
		}
		static void FillHeaderCaptions(object[,] table, SnapDocument header, Table caption, TableDimension tableDimension, int shiftRow) {
			for(int i = 0; i < tableDimension.ColumnCount; i++) {
				for(int j = 0; j < tableDimension.RowCount; j++) {
					if(!IsCellValid(table, i, j))
						continue;
					DocumentPosition insertPos = GetInsertPosition(table, caption, i, j, shiftRow);
					string text = GetHeaderText(table, i, j);
					header.InsertText(insertPos, text);
				}
			}
		}
		static bool IsCellValid(object[,] table, int columnIndex, int rowIndex) {
			if(table[columnIndex, rowIndex] == null)
				return false;
			Tuple<ColumnInfo, GridColumnProp> contentCell = table[columnIndex, rowIndex] as Tuple<ColumnInfo, GridColumnProp>;
			if(contentCell != null) {
				return !contentCell.Item1.IsFake;
			}
			return true;
		}
		static DocumentPosition GetInsertPosition(object[,] table, Table caption, int columnIndex, int rowIndex, int shiftRow) {
			TupleWraper tpWrap = new TupleWraper(table[columnIndex, rowIndex]);
			return caption.Cell(tpWrap.RowIndex + shiftRow, tpWrap.ColumnIndex).Range.Start;
		}
		static string GetHeaderText(object[,] table, int columnIndex, int rowIndex) {
			TupleWraper tpWrap = new TupleWraper(table[columnIndex, rowIndex]);
			return tpWrap.Caption;
		}
		static void MakeTemplate(SnapList list, GridView grid, GridTableInfo grTableInf, out Table table, out SnapDocument template) {
			template = list.RowTemplate;
			SnapDocument header = list.ListHeader;
			int previewRowCount = grid.IsTherePreviewRow() ? 1 : 0;
			table = template.Tables.Create(template.Range.End, grTableInf.ContentColumnsTableDimension.RowCount + previewRowCount, grTableInf.ContentColumnsTableDimension.ColumnCount);
			BandedGridView bnddGridView = grid as BandedGridView;
			bool printBands = bnddGridView != null && bnddGridView.OptionsPrint.PrintBandHeader;
			int bandsHeaderRowCount = printBands ? grTableInf.BandsHeaderTableDimension.RowCount : 0;
			int captionRowCount = grTableInf.ContentColumnsTableDimension.RowCount + bandsHeaderRowCount;
			int captionColumnCount = grTableInf.ContentColumnsTableDimension.ColumnCount;
			Table caption = header.Tables.Create(header.Range.End, captionRowCount, captionColumnCount);
			AdjustSize(table, grTableInf);
			AdjustSize(caption, grTableInf);
			SnapSection currentSection = list.Document.GetSection(list.Document.CreatePosition(list.RowTemplate.Range.Start.ToInt()));
			int totalWidth = grTableInf.TableWidth;
			if(totalWidth > grid.ViewRect.Width) {
				currentSection.Page.Landscape = true;
			}
			float workingPageAreaWidth = CalcWorkingPageAreaWidth(currentSection);
			if(printBands)
				FillHeaderCaptions(grTableInf.BandsHeaderTable, header, caption, grTableInf.BandsHeaderTableDimension, 0);
			FillHeaderCaptions(grTableInf.ContentColumnsTable, header, caption, grTableInf.ContentColumnsTableDimension, bandsHeaderRowCount);
			FillContentTable(grid, template, table, grTableInf, workingPageAreaWidth);
			if(printBands)
				MergeCells(grTableInf.BandsHeaderTable, caption, 0, grTableInf.BandsHeaderTableDimension);
			MergeCells(grTableInf.ContentColumnsTable, caption, bandsHeaderRowCount, grTableInf.ContentColumnsTableDimension);
			MergeCells(grTableInf.ContentColumnsTable, table, 0, grTableInf.ContentColumnsTableDimension, grid.IsTherePreviewRow());
		}
		static float CalcWorkingPageAreaWidth(SnapSection currentSection) {
			if(currentSection == null)
				return 0f;
			NativeSectionPage nativePage = (NativeSectionPage)currentSection.Page;
			NativeSectionMargins nativeMargins = (NativeSectionMargins)currentSection.Margins;
			float width = nativePage.Page.Width - nativeMargins.Margins.Left - nativeMargins.Margins.Right;
			return width;
		}
		static void FillPreviewRow(GridView grid, SnapDocument template, Table table, GridTableInfo grTableInf, float workingPageAreaWidth) {
			if(!grid.IsTherePreviewRow())
				return;
			TableCell cell = table.Cell(grTableInf.ContentColumnsTableDimension.RowCount, 0);
			GridColumn previewClmn = grid.GetGridColumnByFieldName(grid.PreviewFieldName);
			if(previewClmn == null) {
				DocumentPosition pos = cell.ContentRange.Start;
				SnapText snTxt = template.CreateSnText(pos, grid.PreviewFieldName);
				return;
			}
			GridColumnProp grdClmnProp = new GridColumnProp() { Width = previewClmn.Width, TableWidth = grTableInf.TableWidth };
			FillCell(template, previewClmn, grdClmnProp, cell, workingPageAreaWidth);
		}
		static GridColumn GetGridColumnByFieldName(this GridView grid, string fieldName) {
			if(String.IsNullOrEmpty(fieldName))
				return null;
			foreach(GridColumn clmn in grid.Columns) {
				if(String.Equals(clmn.FieldName, fieldName))
					return clmn;
			}
			return null;
		}
		static void FillContentTable(GridView grid, SnapDocument template, Table table, GridTableInfo grTableInf, float workingPageAreaWidth) {
			for(int i = 0; i < grTableInf.ContentColumnsTableDimension.ColumnCount; i++) {
				for(int j = 0; j < grTableInf.ContentColumnsTableDimension.RowCount; j++) {
					if(!IsCellValid(grTableInf.ContentColumnsTable, i, j))
						continue;
					FillCell(template, grTableInf.ContentColumnsTable[i, j].Item1.Column, grTableInf.ContentColumnsTable[i, j].Item2, table.Cell(j, i), workingPageAreaWidth);
				}
			}
			FillPreviewRow(grid, template, table, grTableInf, workingPageAreaWidth);
		}
		static void FillCell(SnapDocument template, GridColumn column, GridColumnProp columnProp, TableCell cell, float workingPageAreaWidth) {
			if(column == null)
				return;
			DocumentPosition pos = cell.ContentRange.Start;
			Type colType = GetColType(column);
			string fieldName = column.FieldName.CorrectFieldName();
			if(colType == typeof(byte[]) || colType == typeof(System.Drawing.Image)) {
				var repPicEdit = column.RealColumnEdit as RepositoryItemPictureEdit;
				if(repPicEdit != null) {
					SnapImage snImg = template.CreateSnImage(pos, fieldName);
					if(snImg != null) {
						int curColWidthPerc = (int)columnProp.WidthInPercent;
						float celWidth = curColWidthPerc * workingPageAreaWidth / 100;
						snImg.BeginUpdate();
						snImg.Width = (int)celWidth;
						snImg.Height = (int)(celWidth * 0.7);
						snImg.UpdateMode = UpdateMergeImageFieldMode.KeepSize;
						snImg.ImageSizeMode = xtraPrinting.ImageSizeMode.Squeeze;
						snImg.EndUpdate();
					}
				}
				else {
					var repImgEdit = column.RealColumnEdit as RepositoryItemImageEdit;
					if(repImgEdit != null) {
						string imgPopupPicTxt = XEditors.Controls.Localizer.GetLocalizedString(XEditors.Controls.StringId.ImagePopupPicture.ToString()); 
						template.InsertText(pos, imgPopupPicTxt);
					}
					else {
						template.CreateSnImage(pos, fieldName);
					}
				}
			}
			else if(colType == typeof(bool))
				template.CreateSnCheckBox(pos, fieldName);
			else {
				SnapText snTxt = template.CreateSnText(pos, fieldName);
				snTxt.ApplyTextFormat(column);
			}
		}
		static string CorrectFieldName(this string fieldName) {
			return fieldName.Contains(" ") ? String.Format("\"{0}\"", fieldName) : fieldName;
		}
		static void ApplyTextFormat(this SnapText snTxt, GridColumn column) {
			snTxt.BeginUpdate();
			snTxt.FormatString = GetFormatString(column, column.RealColumnEdit);
			if(String.IsNullOrEmpty(snTxt.FormatString) && column.RealColumnEdit is RepositoryItemProgressBar) {
				RepositoryItemProgressBar repItmProgressBar = column.RealColumnEdit as RepositoryItemProgressBar;
				if(repItmProgressBar.PercentView && repItmProgressBar.DisplayFormat.FormatType == FormatType.None)
					snTxt.TextAfterIfFieldNotBlank = "%";
			}
			if(column.RealColumnEdit is RepositoryItemRichTextEdit)
				snTxt.TextFormat = "rtf";
			snTxt.EndUpdate();
		}
		static string GetFormatString(GridColumn grdColumn, RepositoryItem repItem) {
			var ce = repItem as RepositoryItemTextEdit;
			if(ce != null)
				if(ce.Mask.UseMaskAsDisplayFormat && !String.Equals(ce.Mask.EditMask, "N00")) return ce.Mask.EditMask;
			string format = grdColumn.DisplayFormat.FormatString;
			if(!string.IsNullOrEmpty(format)) return format;
			if(ce != null) {
				string formatRepository = ce.DisplayFormat.FormatString;
				if(!string.IsNullOrEmpty(formatRepository)) return formatRepository;
			}
			return format;
		}
		static void PrepareTableCellForVerticalMerging(object[,] tableInf, Table table, int rowShift, TableDimension tblDim) {
			NativeTable nativeTable = (NativeTable)table;
			XtrModel.Table modelTable = nativeTable.DocumentModel.ActivePieceTable.Tables[nativeTable.ModelTable.Index];
			nativeTable.BeginUpdate();
			for(int i = 0; i < tblDim.RowCount; i++) {
				for(int j = 0; j < tblDim.ColumnCount; j++) {
					if(!IsCellValid(tableInf, j, i))
						continue;
					int rowSpan = GetRowSpan(tableInf, j, i);
					if(rowSpan == 0)
						continue;
					if(rowSpan != 0) {
						XtrModel.TableCell modelCell = modelTable.GetCell(i + rowShift, j);
						modelCell.VerticalMerging = XtrModel.MergingState.Restart;
						for(int rowIndex = i + 1; rowIndex < i + rowSpan; rowIndex++) {
							modelCell = modelTable.GetCell(rowIndex + rowShift, j);
							modelCell.VerticalMerging = XtrModel.MergingState.Continue;
						}
					}
				}
			}
			nativeTable.EndUpdate();
		}
		static void MergeCells(object[,] tableInf, Table table, int rowShift, TableDimension tblDim, bool mergePreviewRow = false) {
			PrepareTableCellForVerticalMerging(tableInf, table, rowShift, tblDim);
			for(int i = tblDim.RowCount - 1; i >= 0; i--) {
				for(int j = tblDim.ColumnCount - 1; j >= 0; j--) {
					if(!IsCellValid(tableInf, j, i))
						continue;
					int columnSpan = GetColumnSpan(tableInf, j, i);
					if(columnSpan == 0)
						continue;
					table.MergeCells(table.Cell(i + rowShift, j), table.Cell(i + rowShift, j + columnSpan - 1));
				}
			}
			if(mergePreviewRow)
				MergePreviewRowCells(table, tblDim);
		}
		static void MergePreviewRowCells(Table table, TableDimension tblDim) { 
			int rowIndex = tblDim.RowCount;
			int colIndex = tblDim.ColumnCount - 1;
			table.MergeCells(table.Cell(rowIndex, 0), table.Cell(rowIndex, colIndex));
		}
		static Dictionary<int, List<BandedGridColumn>> GetBandedColumnsGroupedByRows(GridBand gridBand) {
			Dictionary<int, List<BandedGridColumn>> rowsColumns = new Dictionary<int, List<BandedGridColumn>>();
			if(gridBand == null || gridBand.Columns.VisibleColumnCount == 0)
				return rowsColumns;
			int rowIndex = 0;
			int addedVisibleColCount = 0;
			do {
				List<BandedGridColumn> rowColumns = new List<BandedGridColumn>();
				GetColumnsByRowIndex(gridBand, rowIndex, rowColumns);
				if(rowColumns.Count != 0)
					rowsColumns.Add(rowIndex, rowColumns);
				rowIndex++;
				addedVisibleColCount += rowColumns.Count;
			} while(addedVisibleColCount != gridBand.Columns.VisibleColumnCount);
			return rowsColumns;
		}
		static void GetColumnsByRowIndex(GridBand gridBand, int rowIndex, List<BandedGridColumn> rowColumns) {
			for(int i = 0; i < gridBand.Columns.Count; i++) {
				BandedGridColumn column = gridBand.Columns[i];
				if(column.RowIndex == rowIndex && ShouldShowColumn(column, gridBand.View))
					rowColumns.Add(column);
			}
		}
		static bool ShouldShowColumn(GridColumn clmn, GridView grView) {
			if(clmn == null || grView == null || !clmn.Visible)
				return false;
			bool clmGrouped = grView.GroupedColumns.Contains(clmn);
			bool showGroupedClms = grView.OptionsView.ShowGroupedColumns;
			bool alignGrSumInGrRow = grView.OptionsBehavior.AlignGroupSummaryInGroupRow == DefaultBoolean.True;
			if(!clmGrouped || showGroupedClms || alignGrSumInGrRow)
				return true;
			return false;
		}
		static void MakeReportFooter(SnapList list, GridView grid, GridTableInfo grTableInf) {
			SnapDocument footer = list.ListFooter;
			Table table = footer.Tables.Create(footer.Range.Start, grTableInf.ContentColumnsTableDimension.RowCount, grTableInf.ContentColumnsTableDimension.ColumnCount);
			AdjustSize(table, grTableInf);
			for(int i = 0; i < grTableInf.ContentColumnsTableDimension.ColumnCount; i++) {
				for(int j = 0; j < grTableInf.ContentColumnsTableDimension.RowCount; j++) {
					Tuple<ColumnInfo, GridColumnProp> columnWithProp = grTableInf.ContentColumnsTable[i, j];
					if(columnWithProp == null || columnWithProp.Item1.Column.Summary == null || columnWithProp.Item1.Column.Summary.Count == 0)
						continue;
					TableCell currentCell = table.Cell(j, i);
					for(int sumIdx = 0; sumIdx < columnWithProp.Item1.Column.Summary.Count; sumIdx++) {
						BuildSummaryTemplate(footer, currentCell, columnWithProp.Item1.Column.Summary[sumIdx], SummaryRunning.Report, false);
						bool isLastSummaryItem = sumIdx == columnWithProp.Item1.Column.Summary.Count - 1;
						if(!isLastSummaryItem) {
							DocumentPosition pos = currentCell.ContentRange.End;
							footer.Paragraphs.Insert(pos);
						}
					}
				}
			}
			MergeCells(grTableInf.ContentColumnsTable, table, 0, grTableInf.ContentColumnsTableDimension);
		}
		static GridBand NextVisibleBand(this GridBand grBand) {
			if(grBand.Index == grBand.Collection.Count - 1)
				return null;
			for(int i = grBand.Index + 1; i < grBand.Collection.Count; i++) {
				GridBand band = grBand.Collection[i];
				if(band.Visible)
					return band;
			}
			return null;
		}
		static GridLevelNode NextNode(this GridLevelNode grLvlNode) {
			if(grLvlNode.OwnerCollection == null)
				return null;
			int curNodeIndex = grLvlNode.OwnerCollection.IndexOf(grLvlNode);
			if(curNodeIndex == grLvlNode.OwnerCollection.Count - 1)
				return null;
			return grLvlNode.OwnerCollection[curNodeIndex + 1];
		}
	}
}
