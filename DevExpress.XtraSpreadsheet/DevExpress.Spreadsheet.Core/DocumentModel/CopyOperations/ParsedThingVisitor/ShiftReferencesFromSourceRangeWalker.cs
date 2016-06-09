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
using DevExpress.Office.Utils;
using DevExpress.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model.CopyOperation {
	public class ShiftReferencesFromSourceRangeToTargetAfterPasteCutRangeWalker : ReferenceThingsRPNVisitor, IWorkBookCellsWalker { 
		#region Fields
		readonly Dictionary<string, Dictionary<string, string>> tablesColumnNamesRenamed;
		CellPositionOffset offset;
		CellRange sourceRange;
		CellRange targetRange;
		#endregion
		public ShiftReferencesFromSourceRangeToTargetAfterPasteCutRangeWalker(CellPositionOffset offset, WorkbookDataContext targetDataContext, CellRange sourceRange, CellRange targetRange)
			: base(targetDataContext) {
			this.offset = offset;
			this.sourceRange = sourceRange;
			this.targetRange = targetRange;
			this.tablesColumnNamesRenamed = new Dictionary<string, Dictionary<string, string>>(StringExtensions.ComparerInvariantCultureIgnoreCase);
		}
		#region Properties
		bool ShouldAddSheetDefinition { get { return !Object.ReferenceEquals(sourceRange.Worksheet, targetRange.Worksheet); } }
		public new WorkbookDataContext DataContext { get { return base.DataContext; } }
		public WorkbookDataContext CommonContext { get { return base.DataContext; } }
		public IWorksheet SourceWorksheet { get { return sourceRange.Worksheet as IWorksheet; } }
		public IWorksheet TargetWorksheet { get { return targetRange.Worksheet as IWorksheet; } }
		public CellRange SourceRange { get { return sourceRange; } set { sourceRange = value; } }
		public CellRange TargetRange { get { return targetRange; } set { targetRange = value; } }
		#endregion
		public override bool CheckParsedThingRefersToSourceWorksheet(IParsedThingWithSheetDefinition thingWithSheetDefinitionOrNull, WorkbookDataContext context, ICellTable sourceWorksheet) {
			IWorksheet currentWorksheet = context.CurrentWorksheet;
			return CheckParsedThingRefersToSourceWorksheetCore(thingWithSheetDefinitionOrNull, context, sourceWorksheet, currentWorksheet);
		}
		public virtual void Walk(ICell cell) {
			FormulaChanged = false;
			if (!cell.HasFormula)
				return;
			bool cellInTargetOrSourceRange = targetRange.ContainsCell(cell) || sourceRange.ContainsCell(cell);
			if (cellInTargetOrSourceRange)
				return;
			try {
				CommonContext.PushCurrentCell(cell.Position);
				FormulaBase formula = cell.GetFormula();
				formula.UpdateFormula(this, cell);
				SharedFormulaRef asShared = formula as SharedFormulaRef;
				if (asShared != null) {
					UpdateSharedFormulaRef(asShared, cell);
				}
			}
			finally {
				CommonContext.PopCurrentCell();
			}
		}
		void UpdateSharedFormulaRef(SharedFormulaRef formula, ICell cell) {
			ParsedExpression preparedExpression = formula.GetExpression(DataContext);
			if (preparedExpression == null)
				return;
			ParsedExpression newExpression = preparedExpression.Clone(); 
			DataContext.PushSharedFormulaProcessing(true);
			try {
				newExpression = this.Process(newExpression);
			}
			finally {
				CommonContext.PopSharedFormulaProcessing();
			}
			if (this.FormulaChanged) {
				SharedFormulaToNormalRPNVisitor visitor = new SharedFormulaToNormalRPNVisitor(DataContext);
				ParsedExpression normalExpression = visitor.Process(newExpression);
				Formula newFormula = new Formula(cell, normalExpression);
				cell.TransactedSetFormula(newFormula);
				if (formula.HostSharedFormula.Range == null) {
					Worksheet sheet = CommonContext.CurrentWorksheet as Worksheet;
					SharedFormulaCollection collection = sheet.SharedFormulas;
					foreach (KeyValuePair<int, SharedFormula> pair in collection.InnerCollection) {
						if (object.ReferenceEquals(formula.HostSharedFormula, pair.Value)) {
							collection.Remove(pair.Key);
							break;
						}
					}
				}
			}
		}
		protected internal override ParsedThingBase ProcessRef(ParsedThingRef thing) {
			if (!sourceRange.ContainsCell(thing.Position.ToKey(CommonContext.CurrentWorksheet.SheetId)))
				return thing;
			CellPosition newPos = thing.Position.GetShiftedAny(offset, targetRange.Worksheet as Worksheet);
			FormulaChanged = true;
			if (!newPos.IsValid)
				return thing.GetRefErrorEquivalent();
			thing.Position = newPos;
			if (ShouldAddSheetDefinition) {
				int sheetDefinitionIndex = GetSheetDefinitionToTargetWorksheet();
				ParsedThingRef3d newThing = new ParsedThingRef3d(thing.Position, sheetDefinitionIndex);
				thing = newThing;
			}
			return thing;
		}
		protected internal override ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			IParsedThingWithSheetDefinition thingWithSheetDefinition = thing as IParsedThingWithSheetDefinition;
			CellPosition sourceLocation = thing.Location.ToCellPosition(CommonContext);
			CellKey location = sourceLocation.ToKey(SourceWorksheet.SheetId);
			bool refersToSourceWorksheet = CheckParsedThingRefersToSourceWorksheet(thingWithSheetDefinition, CommonContext, SourceWorksheet);
			bool refersToInsideSourceRange = refersToSourceWorksheet && SourceRange.ContainsCell(location);
			if (!refersToInsideSourceRange)
				return thing;
			CellOffset newOffset = new CellOffset();
			if (CommonContext.SharedFormulaProcessing)
				newOffset = thing.Location.GetShiftedAny(offset);
			else
				newOffset = thing.Location.GetShiftedCellOffsetPositionOnly(offset);
			bool offsetExceedsSheetBounds = false;
			if (offsetExceedsSheetBounds) {
				if (ShouldAddSheetDefinition) {
					int sheetIndex = GetSheetDefinitionToTargetWorksheet();
					return new ParsedThingErr3d(sheetIndex);
				}
				return thing.GetRefErrorEquivalent();
			}
			if (ShouldAddSheetDefinition) {
				int sheetDefinitionIndex = GetSheetDefinitionToTargetWorksheet();
				return new ParsedThingRef3dRel(newOffset, sheetDefinitionIndex);
			}
			if (!thing.Location.Equals(newOffset)) {
				thing.Location = newOffset;
				FormulaChanged = true;
			}
			return thing;
		}
		protected internal override ParsedThingBase ProcessArea(ParsedThingArea thing) {
			CellRange thingRangeWithoutSheet = thing.CellRange;
			if (sourceRange.ContainsRange(thingRangeWithoutSheet.TopLeft, thingRangeWithoutSheet.BottomRight)) {
				ICellTable unknownSheet = sourceRange.Worksheet; 
				CellRange shiftedRange = thing.CellRange.GetShiftedAny(offset, unknownSheet) as CellRange;
				if (shiftedRange == null) {
					FormulaChanged = true;
					return thing.GetRefErrorEquivalent();
				}
				if (!shiftedRange.Equals(thing.CellRange)) {
					thing.CellRange = shiftedRange;
					FormulaChanged = true;
				}
			}
			return thing;
		}
		protected internal override ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			IParsedThingWithSheetDefinition thingWithSheetDefinition = thing as IParsedThingWithSheetDefinition; 
			CellPosition sourceTopLeft = thing.First.ToCellPosition(CommonContext);
			CellPosition sourceBottomRight = thing.Last.ToCellPosition(CommonContext);
			CellRange expressionCellRange = new CellRange(SourceWorksheet, sourceTopLeft, sourceBottomRight);
			bool refersToSourceWorksheet = CheckParsedThingRefersToSourceWorksheet(thingWithSheetDefinition, CommonContext, SourceWorksheet);
			bool refersToInsideSourceRange = refersToSourceWorksheet && SourceRange.Includes(expressionCellRange);
			if (!refersToInsideSourceRange)
				return thing;
			if (ShouldAddSheetDefinition) {
				string targetWorksheetName = TargetWorksheet.Name; 
				if (thingWithSheetDefinition == null) {
					int sheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(targetWorksheetName);
					thing = new ParsedThingArea3dRel(thing.First, thing.Last, sheetDefinitionIndex);
				}
				else if (thingWithSheetDefinition != null) {
					SheetDefinition sheetDefinitionOld = CommonContext.GetSheetDefinition(thingWithSheetDefinition.SheetDefinitionIndex);
					thingWithSheetDefinition.SheetDefinitionIndex = GetSheetDefinitionForWorksheetSameWorkbook(targetWorksheetName);
				}
			}
			if (CommonContext.SharedFormulaProcessing) {
				thing.First = thing.First.GetShiftedAny(offset);
				thing.Last = thing.Last.GetShiftedAny(offset);
			}
			else {
				thing.First = thing.First.GetShiftedCellOffsetPositionOnly(offset);
				thing.Last = thing.Last.GetShiftedCellOffsetPositionOnly(offset);
			}
			this.FormulaChanged = true;
			return thing;
		}
		int GetSheetDefinitionToTargetWorksheet() {
			SheetDefinition targetRangeWorksheetSheetDefinition = new SheetDefinition();
			targetRangeWorksheetSheetDefinition.SheetNameStart = targetRange.Worksheet.Name;
			int sheetDefinitionIndex = this.CommonContext.RegisterSheetDefinition(targetRangeWorksheetSheetDefinition);
			System.Diagnostics.Debug.Assert(Object.ReferenceEquals(this.CommonContext.Workbook, targetRange.Worksheet.Workbook));
			return sheetDefinitionIndex;
		}
		public override CellRangeBase ProcessCellRange(CellRange range) {
			return range.GetShifted(offset);
		}
		#region IWorkBookCellsWalker Members
		public virtual void Walk(DocumentModel workBook) {
			workBook.Sheets.ForEach(Walk);
			ProcessDefinedNameCollection(workBook.DefinedNames); 
			workBook.PivotCaches.ForEach(Walk);
		}
		public virtual void Walk(Worksheet sheet) {
			CommonContext.PushCurrentWorksheet(sheet);
			try {
				sheet.Rows.ForEach(Walk);
				ProcessDefinedNameCollection(sheet.DefinedNames);
				ProcessDataValidationCollection(sheet.DataValidations);
			}
			finally {
				CommonContext.PopCurrentWorksheet();
			}
		}
		public virtual void Walk(Row row) {
			row.Cells.ForEach(Walk);
		}
		public virtual void Walk(DefinedNameBase definedNameBase) {
			CommonContext.PushDefinedNameProcessing(definedNameBase);
			try {
				FormulaChanged = false;
				DefinedName definedName = definedNameBase as DefinedName;
				if (definedName != null) {
					definedName.UpdateExpression(this);
				}
			}
			finally {
				CommonContext.PopDefinedNameProcessing();
			}
		}
		public void Walk(PivotCache cache) {
			cache.Source.UpdateExpression(this, this.TargetWorksheet.Workbook as DocumentModel);
		}
		public virtual void ProcessDefinedNameCollection(DefinedNameCollection collection) {
			foreach (DefinedNameBase definedName in collection)
				Walk(definedName);
		}
		void ProcessDataValidationCollection(DataValidationCollection collection) {
			collection.ShiftReferences(this);
		}
		#endregion
		int GetSheetDefinitionForWorksheetSameWorkbook(string sheetName) {
			SheetDefinition sourceRangeWorksheetSheetDefinition = new SheetDefinition();
			sourceRangeWorksheetSheetDefinition.SheetNameStart = sheetName;
			int sheetDefinitionIndex = CommonContext.RegisterSheetDefinition(sourceRangeWorksheetSheetDefinition);
			System.Diagnostics.Debug.Assert(Object.ReferenceEquals(CommonContext.Workbook, TargetWorksheet.Workbook));
			return sheetDefinitionIndex;
		}
		protected internal override IWorksheet ShouldProcessSheetDefinition(SheetDefinition sheetDefinition) {
			if (sheetDefinition.IsExternalReference)
				return null;
			return base.ShouldProcessSheetDefinition(sheetDefinition);
		}
		#region TablesColumnRenamedFormulaWalker
		public void RegisterTableColumnsNamesRenamed(string tableName, Dictionary<string, string> tableColumnsNamesRenamed) {
			tablesColumnNamesRenamed.Add(tableName, tableColumnsNamesRenamed);
		}
		public override void Visit(ParsedThingTable thing) {
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			SheetDefinition sheetDefinition = DataContext.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition.IsExternalReference)
				return;
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		void ProcessTableThing(ParsedThingTable thing) {
			string tableName = thing.TableName;
			if (tablesColumnNamesRenamed.ContainsKey(tableName)) {
				Dictionary<string, string> tableColumnNamesRenamed = tablesColumnNamesRenamed[tableName];
				string newName;
				if (!string.IsNullOrEmpty(thing.ColumnStart) && tableColumnNamesRenamed.TryGetValue(thing.ColumnStart, out newName)) {
					thing.ColumnStart = newName;
					FormulaChanged = true;
				}
				if (!string.IsNullOrEmpty(thing.ColumnEnd) && tableColumnNamesRenamed.TryGetValue(thing.ColumnEnd, out newName)) {
					thing.ColumnEnd = newName;
					FormulaChanged = true;
				}
			}
		}
		#endregion
	}
}
