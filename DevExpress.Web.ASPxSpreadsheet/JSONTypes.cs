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

using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Web.ASPxSpreadsheet.Internal.JSONTypes {
	public class JSONCellPosition {
		public JSONCellPosition(CellPosition cell) {
			this.col = cell.Column;
			this.row = cell.Row;
		}
		public JSONCellPosition(int col, int row) {
			this.col = col;
			this.row = row;
		}
		public int col { get; private set; }
		public int row { get; private set; }
	}
	public class JSONCellSize {
		public JSONCellSize(int width, int height) {
			this.width = width;
			this.height = height;
		}
		public int width { get; private set; }
		public int height { get; private set; }
	}
	public class JSONSheetInfo {
		public JSONSheetInfo(string name, int id) {
			this.Name = name;
			this.Id = id;
		}
		public string Name { get; private set; }
		public int Id { get; private set; }
	}
	public class JSONTilePosition {
		public JSONTilePosition(int rowIndex, int colIndex) {
			this.rowIndex = rowIndex;
			this.colIndex = colIndex;
		}
		public int rowIndex { get; private set; }
		public int colIndex { get; private set; }
	}
	public class JSONTileInfo {
		public JSONTileInfo(ArrayList grid, ArrayList columnHeaderTiles, ArrayList rowHeaderTiles) {
			this.grid = grid;
			this.columnHeader = columnHeaderTiles;
			this.rowHeader = rowHeaderTiles;
		}
		public ArrayList grid { get; private set; }
		public ArrayList columnHeader { get; private set; }
		public ArrayList rowHeader { get; private set; }
	}
	public class JSONHistory {
		public JSONHistory(bool canUndo, bool canRedo) {
			this.canUndo = canUndo;
			this.canRedo = canRedo;
		}
		public bool canUndo { get; private set; }
		public bool canRedo { get; private set; }
	}
	public class JSONTabControlInfo {
		public JSONTabControlInfo(string visibleSheets, string hiddenSheets) {
			this.visibleSheets = visibleSheets;
			this.hiddenSheets = hiddenSheets;
		}
		public string visibleSheets { get; private set; }
		public string hiddenSheets { get; private set; }
	}
	public class JSONTileIndicesRange {
		public JSONTileIndicesRange(int top, int right, int bottom, int left) {
			this.top = top;
			this.right = right;
			this.bottom = bottom;
			this.left = left;
		}
		public int top { get; private set; }
		public int right { get; private set; }
		public int bottom { get; private set; }
		public int left { get; private set; }
	}
	public class JSONChartInfo {
		public JSONChartInfo(int id, string range, string view, string title, string hAxisTitle, string vAxisTitle) {
			this.Id = id;
			this.Range = range;
			this.View = view;
			this.Title = title;
			this.HAxisTitle = hAxisTitle;
			this.VAxisTitle = vAxisTitle;
		}
		public int Id { get; private set; }
		public string Range { get; private set; }
		public string View { get; private set; }
		public string Title { get; private set; }
		public string HAxisTitle { get; private set; }
		public string VAxisTitle { get; private set; }
	}
	public class JSONPrintOptions {
		public JSONPrintOptions(bool showGridlines, bool showHeadings) {
			this.showGridlines = showGridlines;
			this.showHeadings = showHeadings;
		}
		public bool showGridlines { get; private set; }
		public bool showHeadings { get; private set; }
	}
	public class JSONCalculation {
		public JSONCalculation(string calcMode, bool updateCalcMode) {
			this.calcMode = calcMode;
			this.updateCalcMode = updateCalcMode;
		}
		public string calcMode { get; private set; }
		public bool updateCalcMode { get; private set; }
	}
	public class CellPositionJSON : IEnumerable {
		public int ColumnIndex { get; private set; }
		public int RowIndex { get; private set; }
		public CellPositionJSON(int columnIndex, int rowIndex) {
			ColumnIndex = columnIndex;
			RowIndex = rowIndex;
		}
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			yield return ColumnIndex;
			yield return RowIndex;
		}
		#endregion
	}
	public class AutoFilterJSON {
		public int ColumnIndex { get; private set; }
		public int RowIndex { get; private set; }
		public bool IsDefault { get; private set; }
		public byte ColumnType { get; private set; }
		public string ImageName { get; private set; }
		public AutoFilterJSON(int columnIndex, int rowIndex, bool isDefault, byte columnType, string imageType) {
			ColumnIndex = columnIndex;
			RowIndex = rowIndex;
			IsDefault = isDefault;
			ColumnType = columnType;
			ImageName = imageType;
		}
		public List<object> Serialize() {
			return new List<object>() {
				ColumnIndex,
				RowIndex,
				ColumnType,
				ImageName,
				IsDefault
			};
		}
	}
	public class TableInfoJSON {
		public string Name { get; private set; }
		public byte Properties { get; private set; }
		public List<int> Range { get; private set; }
		public TableInfoJSON(string name, byte options, List<int> range) {
			Name = name;
			Properties = options;
			Range = range;
		}
		public List<object> Serialize() {
			return new List<object>() {
				Name, 
				Properties,
				Range
			};
		}
	}
	public class JSONPanesVisibleRange {
		public JSONPanesVisibleRange(JSONTileIndicesRange mainPanel) {
			MainPane = mainPanel;
		}
		public JSONPanesVisibleRange(JSONTileIndicesRange mainPane, JSONTileIndicesRange topRightPane, JSONTileIndicesRange bottomLeftPane, JSONTileIndicesRange frozenPane) {
			MainPane = mainPane;
			TopRightPane = topRightPane;
			BottomLeftPane = bottomLeftPane;
			FrozenPane = frozenPane;
		}
		public JSONPanesVisibleRange(JSONTileIndicesRange mainPane, JSONTileIndicesRange secondPane, bool isColumnFrozen) {
			MainPane = mainPane;
			if(isColumnFrozen)
				BottomLeftPane = secondPane;
			else
				TopRightPane = secondPane;
		}
		public JSONTileIndicesRange MainPane { get; private set; }
		public JSONTileIndicesRange TopRightPane { get; private set; }
		public JSONTileIndicesRange BottomLeftPane { get; private set; }
		public JSONTileIndicesRange FrozenPane { get; private set; }
	}
	public class JSONPanesRenderInfo {
		public JSONPanesRenderInfo() { }
		public JSONPanesRenderInfo(JSONTileInfo mainPanel) {
			MainPane = mainPanel;
		}
		public JSONPanesRenderInfo(JSONTileInfo mainPane, JSONTileInfo topRightPane, JSONTileInfo bottomLeftPane, JSONTileInfo frozenPane) {
			MainPane = mainPane;
			TopRightPane = topRightPane;
			BottomLeftPane = bottomLeftPane;
			FrozenPane = frozenPane;
		}
		public JSONPanesRenderInfo(JSONTileInfo mainPane, JSONTileInfo secondPane, bool isColumnFrozen) {
			MainPane = mainPane;
			if(isColumnFrozen)
				BottomLeftPane = secondPane;
			else
				TopRightPane = secondPane;
		}
		public void SetFieldByType(JSONTileInfo paneRenderInfo, PanesType paneType) {
			switch(paneType) {
				case PanesType.MainPane:
					MainPane = paneRenderInfo;
					break;
				case PanesType.TopRightPane:
					TopRightPane = paneRenderInfo;
					break;
				case PanesType.BottomLeftPane:
					BottomLeftPane = paneRenderInfo;
					break;
				case PanesType.FrozenPane:
					FrozenPane = paneRenderInfo;
					break;
			}
		}
		public JSONTileInfo MainPane { get; private set; }
		public JSONTileInfo TopRightPane { get; private set; }
		public JSONTileInfo BottomLeftPane { get; private set; }
		public JSONTileInfo FrozenPane { get; private set; }
	}
	public class JSONFrozenPaneSettings {
		public int width { get; private set; }
		public int height { get; private set; }
		public int mode { get; private set; }
		public JSONCellPosition frozenCell { get; private set; }
		public JSONCellPosition topLeftCell { get; private set; }
		public JSONFrozenPaneSettings(int width, int height, int mode, CellPosition frozenCell, CellPosition topLeftCell) {
			this.width = width;
			this.height = height;
			this.mode = mode;
			this.frozenCell = new JSONCellPosition(frozenCell);
			this.topLeftCell = new JSONCellPosition(topLeftCell);
		}
	}
}
