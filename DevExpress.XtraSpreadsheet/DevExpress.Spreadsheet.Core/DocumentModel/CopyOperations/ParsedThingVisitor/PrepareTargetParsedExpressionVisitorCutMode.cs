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
using DevExpress.Office.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class PrepareTargetParsedExpressionVisitorCutMode : PrepareTargetParsedExpressionVisitor {
		#region Cstr
		public PrepareTargetParsedExpressionVisitorCutMode(WorkbookDataContext targetContext, Worksheet target, ICopyEverythingArguments args)
			: base(targetContext, target, args) {
		}
		bool ShouldAddSheetDefinition { get { return !Object.ReferenceEquals(SourceWorksheet, TargetWorksheet); } }
		#endregion
		public new WorkbookDataContext DataContext { get { throw new InvalidOperationException(); } }
		WorkbookDataContext CommonContext { get { return base.DataContext; } }
		protected internal override ParsedThingBase ProcessRef(ParsedThingRef thing) {
			IParsedThingWithSheetDefinition thingWithSheetDefinition = thing as IParsedThingWithSheetDefinition; 
			bool refersToSourceWorksheet = CheckParsedThingRefersToSourceWorksheet(thingWithSheetDefinition, CommonContext, SourceWorksheet);
			bool refersToInsideSourceRange = refersToSourceWorksheet && SourceRange.ContainsCell(thing.Position.ToKey(SourceWorksheet.SheetId)); 
			if (!refersToInsideSourceRange) {
				if (thingWithSheetDefinition == null && ShouldAddSheetDefinition) {
					int sheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(SourceWorksheet.Name);
					thing = new ParsedThingRef3d(thing.Position, sheetDefinitionIndex);
				}
				return thing; 
			}
			else if (thingWithSheetDefinition != null) {
				SheetDefinition sheetDefinitionOld = CommonContext.GetSheetDefinition(thingWithSheetDefinition.SheetDefinitionIndex);
				IWorksheet referencedSheetSource = sheetDefinitionOld.GetSheetStart(CommonContext);
				System.Diagnostics.Debug.Assert(object.ReferenceEquals(SourceWorksheet, referencedSheetSource));
				thingWithSheetDefinition.SheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(TargetWorksheet.Name);
			}
			bool shouldShiftPositionRelative = true;
			bool shouldShiftPositionAbsolute = false;
			if (refersToInsideSourceRange) {
				shouldShiftPositionRelative = true;
				shouldShiftPositionAbsolute = true;
			}
			CellPosition newPos = thing.Position;
			if (shouldShiftPositionRelative) { 
				newPos = newPos.GetShifted(Offset, TargetWorksheet);
			}
			if (shouldShiftPositionAbsolute)  
				newPos = newPos.GetShiftedAbsolute(Offset, TargetWorksheet);
			if (!newPos.IsValid)
				return thing.GetRefErrorEquivalent();
			thing.Position = newPos;
			return thing;
		}
		protected internal override ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			IParsedThingWithSheetDefinition thingWithSheetDefinition = thing as IParsedThingWithSheetDefinition; 
			bool shouldShiftCellOffset = true;
			bool shouldShiftCellOffsetPosition = false;
			if (CommonContext.SharedFormulaProcessing) {
				CellPosition sourceCellPosition = thing.Location.ToCellPosition(CommonContext);
				bool refersToSourceWorksheet = CheckParsedThingRefersToSourceWorksheet(thingWithSheetDefinition, CommonContext, SourceWorksheet);
				bool refersToInsideSourceRange = refersToSourceWorksheet && SourceRange.ContainsCell(sourceCellPosition.ToKey(SourceWorksheet.SheetId));
				if (!refersToInsideSourceRange) {
					if (thingWithSheetDefinition == null && ShouldAddSheetDefinition) {
						int sheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(SourceWorksheet.Name);
						thing = new ParsedThingRef3dRel(thing.Location, sheetDefinitionIndex);
					}
				}
				else if (thingWithSheetDefinition != null) {
					thingWithSheetDefinition.SheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(TargetWorksheet.Name);
				}
				if (refersToInsideSourceRange) {
					shouldShiftCellOffset = false;
					shouldShiftCellOffsetPosition = true;
				}
			}
			else if (CommonContext.DefinedNameProcessing) {
				if (thing.Location.ColumnType == CellOffsetType.Offset || thing.Location.RowType == CellOffsetType.Offset) {
					shouldShiftCellOffset = false;
					shouldShiftCellOffsetPosition = false;
				}
			}
			CellPositionOffset offsetInverted = new CellPositionOffset(-Offset.ColumnOffset, -Offset.RowOffset);
			CellPositionOffset offsetPositive = Offset;
			if (shouldShiftCellOffset)  
				thing.Location = thing.Location.GetShiftedOffsetOnly(offsetInverted);
			if (shouldShiftCellOffsetPosition)  
				thing.Location = thing.Location.GetShiftedCellOffsetPositionOnly(offsetPositive);
			return base.ProcessRefRel(thing);
		}
		protected internal override ParsedThingBase ProcessArea(ParsedThingArea thing) {
			IParsedThingWithSheetDefinition thingWithSheetDefinition = thing as IParsedThingWithSheetDefinition;
			CellRange thingTargetCellRage = thing.CellRange;
			CellRange sourceCellRange = thing.CellRange.Clone(SourceWorksheet) as CellRange;
			bool refersToSourceWorksheet = CheckParsedThingRefersToSourceWorksheet(thingWithSheetDefinition, CommonContext, SourceWorksheet);
			bool refersToInsideSourceRange = refersToSourceWorksheet && SourceRange.Includes(sourceCellRange);
			if (!refersToInsideSourceRange) {
				if (thingWithSheetDefinition == null && ShouldAddSheetDefinition) {
					int sheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(SourceWorksheet.Name);
					thing = new ParsedThingArea3d(thingTargetCellRage, sheetDefinitionIndex);
				}
				return thing; 
			}
			else if ((thingWithSheetDefinition != null)) {
				SheetDefinition sheetDefinitionOld = CommonContext.GetSheetDefinition(thingWithSheetDefinition.SheetDefinitionIndex);
				IWorksheet referencedSheetSource = sheetDefinitionOld.GetSheetStart(CommonContext);
				System.Diagnostics.Debug.Assert(object.ReferenceEquals(SourceWorksheet, referencedSheetSource));
				thingWithSheetDefinition.SheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(TargetWorksheet.Name);
			}
			bool shouldShiftPositionRelative = true;
			bool shouldShiftPositionAbsolute = false;
			if (refersToInsideSourceRange) {
				shouldShiftPositionRelative = true;
				shouldShiftPositionAbsolute = true;
			}
			CellRange newRange = thing.CellRange.Clone(TargetWorksheet) as CellRange;
			if (shouldShiftPositionRelative) { 
				newRange = newRange.GetShifted(Offset, TargetWorksheet);
			}
			if (shouldShiftPositionAbsolute)  
				newRange = newRange.GetShiftedAbsolute(Offset, TargetWorksheet);
			if (newRange == null)
				return thing.GetRefErrorEquivalent();
			thing.CellRange = newRange;
			return thing;
		}
		protected internal override ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			IParsedThingWithSheetDefinition thingWithSheetDefinition = thing as IParsedThingWithSheetDefinition;
			bool shouldShiftCellOffset = true;
			bool shouldShiftCellOffsetPosition = false;
			if (CommonContext.SharedFormulaProcessing || CommonContext.DefinedNameProcessing) { 
				CellRange areaRange = ConertTwoCellOffsetsToCellRange(thing.First, thing.Last, CommonContext, SourceWorksheet);
				bool refersToSourceWorksheet = CheckParsedThingRefersToSourceWorksheet(thingWithSheetDefinition, CommonContext, SourceWorksheet);
				bool refersToInsideSourceRange = refersToSourceWorksheet && SourceRange.Includes(areaRange);
				if (!refersToInsideSourceRange) {
					if (thingWithSheetDefinition == null && ShouldAddSheetDefinition) {
						int sheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(SourceWorksheet.Name);
						thing = new ParsedThingArea3dRel(thing.First, thing.Last, sheetDefinitionIndex);
					}
				}
				else if (thingWithSheetDefinition != null) {
					thingWithSheetDefinition.SheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(TargetWorksheet.Name);
				}
				if (refersToInsideSourceRange) {
					shouldShiftCellOffset = false;
					shouldShiftCellOffsetPosition = true;
				}
				else if (!refersToInsideSourceRange && CommonContext.DefinedNameProcessing) {
					shouldShiftCellOffset = false;
				}
			}
			CellPositionOffset offsetInverted = new CellPositionOffset(-Offset.ColumnOffset, -Offset.RowOffset);
			CellPositionOffset offsetPositive = Offset;
			if (shouldShiftCellOffset) { 
				thing.First = thing.First.GetShiftedOffsetOnly(offsetInverted);
				thing.Last = thing.Last.GetShiftedOffsetOnly(offsetInverted);
			}
			if (shouldShiftCellOffsetPosition) { 
				thing.First = thing.First.GetShiftedCellOffsetPositionOnly(offsetPositive);
				thing.Last = thing.Last.GetShiftedCellOffsetPositionOnly(offsetPositive);
			}
			return base.ProcessAreaRel(thing);
		}
		CellRange ConertTwoCellOffsetsToCellRange(CellOffset offset1, CellOffset cellOffset2, WorkbookDataContext CommonContext, ICellTable sourceWorksheet) {
			CellPosition sourceTopLeft = offset1.ToCellPosition(CommonContext);
			CellPosition sourceBottomRight = cellOffset2.ToCellPosition(CommonContext);
			return new CellRange(sourceWorksheet, sourceTopLeft, sourceBottomRight);
		}
		int GetSheetDefinitionForWorksheetSameWorkbook(string sheetName) {
			SheetDefinition sourceRangeWorksheetSheetDefinition = new SheetDefinition();
			sourceRangeWorksheetSheetDefinition.SheetNameStart = sheetName;
			int sheetDefinitionIndex = CommonContext.RegisterSheetDefinition(sourceRangeWorksheetSheetDefinition);
			System.Diagnostics.Debug.Assert(Object.ReferenceEquals(CommonContext.Workbook, TargetWorksheet.Workbook));
			return sheetDefinitionIndex;
		}
	}
}
