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
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SharedFormulaToNormalRPNVisitor
	public class SharedFormulaToNormalRPNVisitor : ReplaceThingsPRNVisitor {
		#region Fields
		readonly WorkbookDataContext context;
		#endregion
		public SharedFormulaToNormalRPNVisitor(WorkbookDataContext context) {
			this.context = context;
		}
		public override void Visit(ParsedThingRefRel thing) {
			CellPosition result = thing.Location.ToCellPositionWithoutCorrection(context);
			if (!result.IsValid)
				ReplaceCurrentExpression(new ParsedThingRefErr());
			else
				ReplaceCurrentExpression(new ParsedThingRef(result) { DataType = thing.DataType });
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			CellPosition result = thing.Location.ToCellPositionWithoutCorrection(context);
			if (!result.IsValid)
				ReplaceCurrentExpression(new ParsedThingErr3d(thing.SheetDefinitionIndex));
			else
				ReplaceCurrentExpression(new ParsedThingRef3d(result, thing.SheetDefinitionIndex) { DataType = thing.DataType });
		}
		CellRange GetCellRangeFromAreaN(ParsedThingAreaN thing) {
			CellPosition topLeft = thing.First.ToCellPositionWithoutCorrection(context);
			if (!topLeft.IsValid)
				return null;
			CellPosition bottomRight = thing.Last.ToCellPositionWithoutCorrection(context);
			if (!bottomRight.IsValid)
				return null;
			return new CellRange(context.CurrentWorksheet, topLeft, bottomRight);
		}
		public override void Visit(ParsedThingAreaN thing) {
			CellRange result = GetCellRangeFromAreaN(thing);
			if (result != null)
				ReplaceCurrentExpression(new ParsedThingArea(result) { DataType = thing.DataType });
			else
				ReplaceCurrentExpression(new ParsedThingAreaErr());
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			CellRange result = GetCellRangeFromAreaN(thing);
			if (result != null)
				ReplaceCurrentExpression(new ParsedThingArea3d(result, thing.SheetDefinitionIndex) { DataType = thing.DataType });
			else
				ReplaceCurrentExpression(new ParsedThingAreaErr3d(thing.SheetDefinitionIndex));
		}
	}
	#endregion
	#region FormulaReferencedRangeRPNVisitor
	public class FormulaReferencedRangeRPNVisitor : ReferenceThingsRPNVisitor {
		#region Fields
		int filterSheetId;
		CellRange resultRange;
		int formulaWidth;
		int formulaHeight;
		IWorksheet currentSheet;
		#endregion
		public FormulaReferencedRangeRPNVisitor(int filterSheetId, int formulaWidth, int formulaHeight, WorkbookDataContext context)
			: base(context) {
			this.filterSheetId = filterSheetId;
			this.formulaWidth = formulaWidth;
			this.formulaHeight = formulaHeight;
		}
		public CellRange Perform(ParsedExpression expression) {
			Process(expression);
			return resultRange;
		}
		void AddRange(CellRange range) {
			if (resultRange == null)
				resultRange = range;
			else {
				VariantValue mergedRangeValue = resultRange.UnionWith(range);
				resultRange = (CellRange)mergedRangeValue.CellRangeValue;
			}
		}
		protected internal override ParsedThingBase ProcessArea(ParsedThingArea thing) {
			AddRange(new CellRange(DataContext.CurrentWorksheet, thing.TopLeft, thing.BottomRight));
			return thing;
		}
		protected internal override ParsedThingBase ProcessRef(ParsedThingRef thing) {
			AddRange(new CellRange(DataContext.CurrentWorksheet, thing.Position, thing.Position));
			return thing;
		}
		protected internal override ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			if (filterSheetId != (currentSheet == null ? DataContext.CurrentWorksheet : currentSheet).SheetId)
				return thing;
			CellPosition positionStart = thing.Location.ToCellPosition(DataContext);
			int column = positionStart.Column;
			int row = positionStart.Row;
			if (positionStart.ColumnType == PositionType.Relative)
				column += formulaWidth - 1;
			if (positionStart.RowType == PositionType.Relative)
				row += formulaHeight - 1;
			CellPosition positionEnd = new CellPosition(column, row, positionStart.ColumnType, positionStart.RowType);
			AddRange(new CellRange(currentSheet == null ? DataContext.CurrentWorksheet : currentSheet, positionStart, positionEnd));
			return thing;
		}
		protected internal override ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			if (filterSheetId != (currentSheet == null ? DataContext.CurrentWorksheet : currentSheet).SheetId)
				return thing;
			CellPosition firstPosition = thing.First.ToCellPosition(DataContext);
			CellPosition lastPosition = thing.Last.ToCellPosition(DataContext);
			int column = lastPosition.Column;
			int row = lastPosition.Row;
			if (lastPosition.ColumnType == PositionType.Relative)
				column += formulaWidth - 1;
			if (firstPosition.ColumnType == PositionType.Relative)
				column = Math.Max(column, firstPosition.Column + formulaWidth - 1);
			if (lastPosition.RowType == PositionType.Relative)
				row += formulaHeight - 1;
			if (firstPosition.RowType == PositionType.Relative)
				row = Math.Max(row, firstPosition.Row + formulaHeight - 1);
			lastPosition = new CellPosition(column, row, lastPosition.ColumnType, lastPosition.RowType);
			AddRange(new CellRange(currentSheet == null ? DataContext.CurrentWorksheet : currentSheet, firstPosition, lastPosition));
			return thing;
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			SheetDefinition sheetDefinition = DataContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			currentSheet = ShouldProcessSheetDefinition(sheetDefinition);
			if (currentSheet == null)
				return;
			ParsedThingBase newThing = ProcessRefRel(thing);
			if (!Object.ReferenceEquals(newThing, thing))
				ReplaceCurrentExpression(newThing);
			currentSheet = null;
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			SheetDefinition sheetDefinition = DataContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			currentSheet = ShouldProcessSheetDefinition(sheetDefinition);
			if (currentSheet == null)
				return;
			ParsedThingBase newThing = ProcessAreaRel(thing);
			if (!Object.ReferenceEquals(newThing, thing))
				ReplaceCurrentExpression(newThing);
			currentSheet = null;
		}
		protected override bool ShouldProcessSheetDefinitionCore(SheetDefinition expression, int sheetId) {
			return !expression.Is3DReference && filterSheetId == sheetId;
		}
	}
	#endregion
	#region SharedFormulaValidateRPNVisitor
	public class SharedFormulaValidateRPNVisitor : ParsedThingVisitor {
		#region Fields
		readonly CellRange formulaRange;
		bool result;
		#endregion
		public SharedFormulaValidateRPNVisitor(CellRange formulaRange) {
			this.formulaRange = formulaRange;
			result = true;
		}
		public bool Validate(ParsedExpression expression) {
			Process(expression);
			return result;
		}
		bool CheckColumn(int column) {
			return column >= 0 && column < (formulaRange.Worksheet as Worksheet).MaxColumnCount;
		}
		bool CheckRow(int row) {
			return row >= 0 && row < (formulaRange.Worksheet as Worksheet).MaxRowCount;
		}
		void CheckLocation(CellOffset location) {
			if (location.ColumnType == CellOffsetType.Offset) {
				result &= CheckColumn(location.Column + formulaRange.TopLeft.Column);
				result &= CheckColumn(location.Column + formulaRange.BottomRight.Column);
			}
			if (location.RowType == CellOffsetType.Offset) {
				result &= CheckRow(location.Row + formulaRange.TopLeft.Row);
				result &= CheckRow(location.Row + formulaRange.BottomRight.Row);
			}
		}
		public override void Visit(ParsedThingRefRel thing) {
			if (!result)
				return;
			CheckLocation(thing.Location);
		}
		public override void Visit(ParsedThingAreaN thing) {
			if (!result)
				return;
			CheckLocation(thing.First);
			CheckLocation(thing.Last);
		}
		public override void Visit(ParsedThingExp thing) {
			result = false;
		}
		public override void Visit(ParsedThingSxName thing) {
			result = false;
		}
		public override void Visit(ParsedThingIntersect thing) {
			result = false;
		}
		public override void Visit(ParsedThingUnion thing) {
			result = false;
		}
		public override void Visit(ParsedThingRange thing) {
			result = false;
		}
		public override void Visit(ParsedThingArray thing) {
			result = false;
		}
		public override void Visit(ParsedThingRefErr thing) {
			result = false;
		}
		public override void Visit(ParsedThingAreaErr thing) {
			result = false;
		}
		public override void Visit(ParsedThingRef3d thing) {
			result = false;
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			result = false;
		}
		public override void Visit(ParsedThingArea3d thing) {
			result = false;
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			result = false;
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			result = false;
		}
		public override void Visit(ParsedThingErr3d thing) {
			result = false;
		}
		public override void Visit(ParsedThingNameX thing) {
			result = false;
		}
		public override void VisitMem(ParsedThingMemBase thing) {
			result = false;
		}
	}
	#endregion
}
