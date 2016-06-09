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

using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet {
	public abstract class GrowthStrategy {
		#region static
		public static GrowthStrategy CreateInstance(bool horizontalMode, Worksheet templateSheet) {
			if (horizontalMode)
				return new HorizontalGrowthStrategy(templateSheet);
			return new VerticalGrowthStrategy(templateSheet);
		}
		protected static void CopySheetColumnWidths(CellRange sourceRange, Worksheet targetSheet) {
			Worksheet sourceSheet = sourceRange.Worksheet as Worksheet;
			CellIntervalRange columnRange = CellIntervalRange.CreateColumnInterval(sourceSheet, sourceRange.TopLeft.Column, PositionType.Absolute, sourceRange.BottomRight.Column, PositionType.Absolute);
			var ranges = new DevExpress.XtraSpreadsheet.Model.CopyOperation.SourceTargetRangesForCopy(columnRange, columnRange.Clone(targetSheet));
			var operation = new Model.CopyOperation.RangeCopyOperation(ranges, ModelPasteSpecialFlags.ColumnWidths);
			operation.Execute();
			targetSheet.ActiveView.ShowGridlines = sourceSheet.ActiveView.ShowGridlines;
			targetSheet.ActiveView.ShowRowColumnHeaders = sourceSheet.ActiveView.ShowRowColumnHeaders;
			foreach (Column column in sourceSheet.Columns.GetExistingColumns())
				if (column.IsHidden)
					targetSheet.Columns.GetIsolatedColumn(column.Index).IsHidden = true;
		}
		protected static void CopySheetRowHights(CellRange sourceRange, Worksheet targetSheet) {
			CopyRowsHeights(sourceRange, new CellRange(targetSheet, sourceRange.TopLeft, sourceRange.BottomRight));
		}
		protected static void CopyColumnWidths(CellRange sourceRange, CellRange targetRange) {
			Worksheet sourceSheet = sourceRange.Worksheet as Worksheet;
			Worksheet targetSheet = targetRange.Worksheet as Worksheet;
			CellIntervalRange columnRange = CellIntervalRange.CreateColumnInterval(sourceSheet, sourceRange.TopLeft.Column, PositionType.Absolute, sourceRange.BottomRight.Column, PositionType.Absolute);
			CellIntervalRange columnTargetRange = CellIntervalRange.CreateColumnInterval(targetSheet, targetRange.TopLeft.Column, PositionType.Absolute, targetRange.BottomRight.Column, PositionType.Absolute);
			var ranges = new DevExpress.XtraSpreadsheet.Model.CopyOperation.SourceTargetRangesForCopy(columnRange, columnTargetRange);
			var operation = new Model.CopyOperation.RangeCopyOperation(ranges, ModelPasteSpecialFlags.ColumnWidths);
			operation.Execute();
			targetSheet.ActiveView.ShowGridlines = sourceSheet.ActiveView.ShowGridlines;
			targetSheet.ActiveView.ShowRowColumnHeaders = sourceSheet.ActiveView.ShowRowColumnHeaders;
		}
		protected static void CopyRowsHeights(CellRange sourceRange, CellRange targetRange) {
			Worksheet sourceWorksheet = sourceRange.Worksheet as Worksheet;
			IRowCollection sourceRows = sourceWorksheet.Rows;
			IEnumerable<Row> existingRowEnumerable = sourceRows.GetExistingRows(sourceRange.TopLeft.Row, sourceRange.BottomRight.Row, false);
			foreach (Row sourceRow in existingRowEnumerable) {
				if (!sourceRow.IsCustomHeight && !sourceRow.IsHidden)
					continue;
				int sourceRowRelativePosition = sourceRow.Index - sourceRange.TopLeft.Row;
				int targetRowIndex = targetRange.TopLeft.Row + sourceRowRelativePosition;
				Worksheet targetWorksheet = targetRange.Worksheet as Worksheet;
				Row targetRow = targetWorksheet.Rows[targetRowIndex];
				targetRow.CopyFrom(sourceRow);
			}
		}
		#endregion
		#region fields
		int end;
		int offset;
		Dictionary<RangeFunctionPosition, TrackedRange> trackedPositions;
		Dictionary<RangeFunctionPosition, int> referencesOnTrackedPositions;
		Worksheet templateSheet;
		CellPosition currentWatchingPosition;
		#endregion
		protected GrowthStrategy(Worksheet templateSheet) {
			this.templateSheet = templateSheet;
		}
		#region Properties
		protected int End { get { return end; } set { end = value; } }
		public Worksheet TemplateSheet { get { return templateSheet; } set { templateSheet = value; } }
		internal Dictionary<RangeFunctionPosition, TrackedRange> TrackedPositions { get { return trackedPositions; } }
		internal Dictionary<RangeFunctionPosition, int> ReferencesOnTrackedPositions { get { return referencesOnTrackedPositions; } }
		protected CellPosition CurrentWatchingPosition { get { return currentWatchingPosition; } set { currentWatchingPosition = value; } }
		#endregion
		protected abstract void CopyStaticRangeFormatting(Worksheet targetSheet);
		protected abstract void CopyChangedRangeFormatting(CellRange sourceRange, CellRange targetRange);
		protected abstract void SetWatchingPosition(CellPosition position, int offset);
		protected abstract CellPosition GetTargetPosition(CellPosition position, CellRange sourceRange, CellRange targetRange);
		internal protected abstract CellRange AppendRange(Worksheet targetSheet, CellRange sourceRange);
		internal protected abstract CellRange GetSubrangeByCriteria(CellRange range, int topLeft, int bottomRight);
		internal protected abstract CellRange ExcludeGroupHeaderFooterRanges(List<GroupInfo> groupInfo, CellRange detailRange);
		internal protected abstract int GetPositionCriterion(CellPosition position);
		internal void BeginGrow(Worksheet targetSheet) {
			end = 0;
			CopyStaticRangeFormatting(targetSheet);
		}
		internal CellRangeBase GetRangeByPosition(CellPosition cellPosition) {
			TrackedRange result = null;
			RangeFunctionPosition position = new RangeFunctionPosition(cellPosition, currentWatchingPosition);
			if(trackedPositions.TryGetValue(position, out result)) {
				CellRange resultRange = result.CellRange;
				result.References--;
				if(result.References == 0)
					trackedPositions[position] = null;
				return resultRange;
			}
			return null;
		}
		internal void SetOffset(CellRange range, CellRange sourceRange) {
			offset = GetPositionCriterion(sourceRange.TopLeft) - GetPositionCriterion(range.TopLeft);
		}
		internal void SetWatchingPosition(CellPosition position) {
			SetWatchingPosition(position, offset);
		}
		internal void DetectedTrackedPositions() {
			trackedPositions = new Dictionary<RangeFunctionPosition, TrackedRange>();
			referencesOnTrackedPositions = new Dictionary<RangeFunctionPosition, int>();
			CellRange range = templateSheet.GetPrintRange();
			foreach (ICellBase cellBase in range.GetExistingCellsEnumerable()) {
				ICell cell = cellBase as ICell;
				if (cell == null || !cell.HasFormula)
					continue;
				CustomFunctionFinderVisitor visitor = new CustomFunctionFinderVisitor(cell.Context);
				visitor.Process(cell.GetFormula().Expression);
				if(visitor.References.Count != 0) {
					foreach(CellRangeBase reference in visitor.References) {
						AddTrackedPosition(reference.TopLeft, cell.Position);
						if(reference.Width > 1 || reference.Height > 1)
							AddTrackedPosition(reference.BottomRight, cell.Position);
					}
				}
			}
		}
		void AddTrackedPosition(CellPosition trackedPosition, CellPosition watchingPosition) {
			RangeFunctionPosition rangeFunctionPosition = new RangeFunctionPosition(trackedPosition, watchingPosition);
			if(!trackedPositions.ContainsKey(rangeFunctionPosition)) {
				trackedPositions.Add(rangeFunctionPosition, new TrackedRange(null));
				referencesOnTrackedPositions.Add(rangeFunctionPosition, 0);
			}
			trackedPositions[rangeFunctionPosition].References++;
			referencesOnTrackedPositions[rangeFunctionPosition]++;
		}
		protected void CopyRange(CellRange sourceRange, CellRange targetRange, Worksheet targetSheet) {
			List<RangeFunctionPosition> positions = new List<RangeFunctionPosition>();
			positions.AddRange(TrackedPositions.Keys);
			foreach (RangeFunctionPosition trackedPosition in positions) {
				CellPosition position = trackedPosition.TrackedPosition;
				if(sourceRange.ContainsCell(position.Column, position.Row)) {
					CellPosition targetPosition = GetTargetPosition(position, sourceRange, targetRange);
					if(TrackedPositions[trackedPosition] == null) {
						TrackedPositions[trackedPosition] = new TrackedRange(new CellRange(targetSheet, targetPosition, targetPosition));
						TrackedPositions[trackedPosition].References = ReferencesOnTrackedPositions[trackedPosition];
					}
					else {
						if(TrackedPositions[trackedPosition].CellRange != null)
							TrackedPositions[trackedPosition].CellRange.BottomRight = targetPosition;
						else
							TrackedPositions[trackedPosition].CellRange = new CellRange(targetSheet, targetPosition, targetPosition);
					}
				}
			}
			try {
				bool suppressAutoStartCalculation = targetSheet.Workbook.SuppressAutoStartCalculation;
				targetSheet.Workbook.SuppressAutoStartCalculation = true;
				try {
					var ranges = new DevExpress.XtraSpreadsheet.Model.CopyOperation.SourceTargetRangesForCopy(sourceRange, targetRange);
					var copy = new Model.CopyOperation.RangeCopyOperation(ranges, ModelPasteSpecialFlags.All);
					copy.Execute();
				}
				finally {
					targetSheet.Workbook.SuppressAutoStartCalculation = suppressAutoStartCalculation;
				}
			}
			catch {
			}
			finally {
				CopyChangedRangeFormatting(sourceRange, targetRange);
			}
		}
	}
}
