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
namespace DevExpress.XtraSpreadsheet.Model {
	#region IParsedThingVisitor
	public interface IParsedThingVisitor {
		void Visit(ParsedThingExp thing);
		void Visit(ParsedThingDataTable thing);
		void Visit(ParsedThingParentheses thing);
		#region Binary
		void Visit(ParsedThingAdd thing);
		void Visit(ParsedThingSubtract thing);
		void Visit(ParsedThingMultiply thing);
		void Visit(ParsedThingDivide thing);
		void Visit(ParsedThingPower thing);
		void Visit(ParsedThingConcat thing);
		void Visit(ParsedThingLess thing);
		void Visit(ParsedThingLessEqual thing);
		void Visit(ParsedThingEqual thing);
		void Visit(ParsedThingGreaterEqual thing);
		void Visit(ParsedThingGreater thing);
		void Visit(ParsedThingNotEqual thing);
		void Visit(ParsedThingIntersect thing);
		void Visit(ParsedThingUnion thing);
		void Visit(ParsedThingRange thing);
		#endregion
		#region Unary
		void Visit(ParsedThingUnaryPlus thing);
		void Visit(ParsedThingUnaryMinus thing);
		void Visit(ParsedThingPercent thing);
		#endregion
		#region Elf
		void Visit(ParsedThingElfLel thing);
		void Visit(ParsedThingElfRw thing);
		void Visit(ParsedThingElfCol thing);
		void Visit(ParsedThingElfRwV thing);
		void Visit(ParsedThingElfColV thing);
		void Visit(ParsedThingElfRadical thing);
		void Visit(ParsedThingElfRadicalS thing);
		void Visit(ParsedThingElfColS thing);
		void Visit(ParsedThingElfColSV thing);
		void Visit(ParsedThingElfRadicalLel thing);
		void Visit(ParsedThingSxName thing);
		#endregion
		#region Attributes
		void Visit(ParsedThingAttrSemi thing);
		void Visit(ParsedThingAttrIf thing);
		void Visit(ParsedThingAttrChoose thing);
		void Visit(ParsedThingAttrGoto thing);
		void Visit(ParsedThingAttrSum thing);
		void Visit(ParsedThingAttrBaxcel thing);
		void Visit(ParsedThingAttrBaxcelVolatile thing);
		void Visit(ParsedThingAttrSpace thing);
		void Visit(ParsedThingAttrSpaceSemi thing);
		#endregion
		#region Operand
		void Visit(ParsedThingMissingArg thing);
		void Visit(ParsedThingStringValue thing);
		void Visit(ParsedThingError thing);
		void Visit(ParsedThingBoolean thing);
		void Visit(ParsedThingInteger thing);
		void Visit(ParsedThingNumeric thing);
		void Visit(ParsedThingArray thing);
		void Visit(ParsedThingFunc thing);
		void Visit(ParsedThingFuncVar thing);
		void Visit(ParsedThingUnknownFunc thing);
		void Visit(ParsedThingCustomFunc thing);
		void Visit(ParsedThingUnknownFuncExt thing);
		void Visit(ParsedThingAddinFunc thing);
		void Visit(ParsedThingName thing);
		void Visit(ParsedThingNameX thing);
		void Visit(ParsedThingMemArea thing);
		void Visit(ParsedThingMemErr thing);
		void Visit(ParsedThingMemNoMem thing);
		void Visit(ParsedThingMemFunc thing);
		void Visit(ParsedThingArea thing);
		void Visit(ParsedThingAreaErr thing);
		void Visit(ParsedThingArea3d thing);
		void Visit(ParsedThingAreaErr3d thing);
		void Visit(ParsedThingAreaN thing);
		void Visit(ParsedThingArea3dRel thing);
		void Visit(ParsedThingRef thing);
		void Visit(ParsedThingRefErr thing);
		void Visit(ParsedThingRef3d thing);
		void Visit(ParsedThingErr3d thing);
		void Visit(ParsedThingRefRel thing);
		void Visit(ParsedThingRef3dRel thing);
		void Visit(ParsedThingTable thing);
		void Visit(ParsedThingTableExt thing);
		void Visit(ParsedThingVariantValue thing);
		#endregion
	}
	#endregion
	#region ParsedThingVisitor
	public abstract class ParsedThingVisitor : IParsedThingVisitor {
		public virtual ParsedExpression Process(ParsedExpression expression) {
			if (expression == null)
				return null;
			foreach (IParsedThing thing in expression)
				thing.Visit(this);
			return expression;
		}
		public virtual void Visit(ParsedThingDataTable thing) { }
		public virtual void Visit(ParsedThingExp thing) { }
		public virtual void Visit(ParsedThingParentheses thing) { }
		#region Binary
		public virtual void VisitBinary(ParsedThingBase thing) {
		}
		public virtual void Visit(ParsedThingAdd thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingSubtract thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingMultiply thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingDivide thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingPower thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingConcat thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingLess thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingLessEqual thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingEqual thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingGreaterEqual thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingGreater thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingNotEqual thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingIntersect thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingUnion thing) {
			VisitBinary(thing);
		}
		public virtual void Visit(ParsedThingRange thing) {
			VisitBinary(thing);
		}
		#endregion
		#region Unary
		public virtual void VisitUnary(ParsedThingBase thing) {
		}
		public virtual void Visit(ParsedThingUnaryPlus thing) {
			VisitUnary(thing);
		}
		public virtual void Visit(ParsedThingUnaryMinus thing) {
			VisitUnary(thing);
		}
		public virtual void Visit(ParsedThingPercent thing) {
			VisitUnary(thing);
		}
		#endregion
		#region Elf
		public virtual void Visit(ParsedThingElfLel thing) { }
		public virtual void Visit(ParsedThingElfRw thing) { }
		public virtual void Visit(ParsedThingElfCol thing) { }
		public virtual void Visit(ParsedThingElfRwV thing) { }
		public virtual void Visit(ParsedThingElfColV thing) { }
		public virtual void Visit(ParsedThingElfRadical thing) { }
		public virtual void Visit(ParsedThingElfRadicalS thing) { }
		public virtual void Visit(ParsedThingElfColS thing) { }
		public virtual void Visit(ParsedThingElfColSV thing) { }
		public virtual void Visit(ParsedThingElfRadicalLel thing) { }
		public virtual void Visit(ParsedThingSxName thing) { }
		#endregion
		#region Attributes
		public virtual void VisitAttribute(ParsedThingAttrBase thing) {
		}
		public virtual void Visit(ParsedThingAttrSemi thing) {
			VisitAttribute(thing);
		}
		public virtual void Visit(ParsedThingAttrIf thing) {
			VisitAttribute(thing);
		}
		public virtual void Visit(ParsedThingAttrChoose thing) {
			VisitAttribute(thing);
		}
		public virtual void Visit(ParsedThingAttrGoto thing) {
			VisitAttribute(thing);
		}
		public virtual void Visit(ParsedThingAttrSum thing) {
			VisitAttribute(thing);
		}
		public virtual void Visit(ParsedThingAttrBaxcel thing) {
			VisitAttribute(thing);
		}
		public virtual void Visit(ParsedThingAttrBaxcelVolatile thing) {
			VisitAttribute(thing);
		}
		public virtual void Visit(ParsedThingAttrSpace thing) {
			VisitAttribute(thing);
		}
		public virtual void Visit(ParsedThingAttrSpaceSemi thing) {
			VisitAttribute(thing);
		}
		#endregion
		#region Operand
		public virtual void VisitOperand(ParsedThingBase thing) {
		}
		public virtual void Visit(ParsedThingMissingArg thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingStringValue thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingError thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingBoolean thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingInteger thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingNumeric thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingArray thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingName thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingNameX thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingRef thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingRefErr thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingRef3d thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingErr3d thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingRefRel thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingRef3dRel thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingArea thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingAreaErr thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingArea3d thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingAreaN thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingAreaErr3d thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingArea3dRel thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingTable thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingTableExt thing) {
			VisitOperand(thing);
		}
		public virtual void Visit(ParsedThingVariantValue thing) {
			VisitOperand(thing);
		}
		#endregion
		#region Function
		public virtual void VisitFunction(ParsedThingFunc thing) {
		}
		public virtual void Visit(ParsedThingFunc thing) {
			VisitFunction(thing);
		}
		public virtual void Visit(ParsedThingFuncVar thing) {
			VisitFunction(thing);
		}
		public virtual void Visit(ParsedThingUnknownFunc thing) {
			VisitFunction(thing);
		}
		public virtual void Visit(ParsedThingCustomFunc thing) {
			VisitFunction(thing);
		}
		public virtual void Visit(ParsedThingUnknownFuncExt thing) {
			VisitFunction(thing);
		}
		public virtual void Visit(ParsedThingAddinFunc thing) {
			VisitFunction(thing);
		}
		#endregion
		#region Mem
		public virtual void VisitMem(ParsedThingMemBase thing) {
		}
		public virtual void Visit(ParsedThingMemArea thing) {
			VisitMem(thing);
		}
		public virtual void Visit(ParsedThingMemErr thing) {
			VisitMem(thing);
		}
		public virtual void Visit(ParsedThingMemNoMem thing) {
			VisitMem(thing);
		}
		public virtual void Visit(ParsedThingMemFunc thing) {
			VisitMem(thing);
		}
		#endregion
	}
	#endregion
	#region ReplaceThingsPRNVisitor
	public abstract class ReplaceThingsPRNVisitor : ParsedThingVisitor {
		#region Fields
		int currentIndex;
		ParsedExpression expression;
		bool formulaChanged;
		bool isValidFormula = true;
		Worksheet currentSourceWorksheet = null;
		#endregion
		public bool FormulaChanged { get { return formulaChanged; } protected set { formulaChanged = value; } }
		public bool IsValidFormula { get { return isValidFormula; } set { isValidFormula = value; } }
		public override ParsedExpression Process(ParsedExpression expression) {
			formulaChanged = false;
			isValidFormula = true;
			this.expression = expression;
			currentIndex = 0;
			while (isValidFormula && currentIndex < expression.Count) {
				expression[currentIndex].Visit(this);
				currentIndex++;
			}
			return isValidFormula ? this.expression : null;
		}
		public void ReplaceCurrentExpression(IParsedThing newExpression) {
			expression[currentIndex] = newExpression;
			formulaChanged = true;
		}
		public void InvalidateFormula() {
			isValidFormula = false;
			formulaChanged = true;
		}
		public void PushCurrentSourceWorksheet(Worksheet worksheet) {
			this.currentSourceWorksheet = worksheet;
		}
		public void PopCurrentSourceWorksheet() {
			this.currentSourceWorksheet = null;
		}
		public virtual bool CheckParsedThingRefersToSourceWorksheet(IParsedThingWithSheetDefinition thingWithSheetDefinitionOrNull, WorkbookDataContext context, ICellTable sourceWorksheet) {
			System.Diagnostics.Debug.Assert(currentSourceWorksheet != null);
			IWorksheet currentWorksheet = currentSourceWorksheet;
			return CheckParsedThingRefersToSourceWorksheetCore(thingWithSheetDefinitionOrNull, context, sourceWorksheet, currentWorksheet);
		}
		protected bool CheckParsedThingRefersToSourceWorksheetCore(IParsedThingWithSheetDefinition thingWithSheetDefinitionOrNull, WorkbookDataContext context, ICellTable sourceWorksheet, ICellTable currentWorksheet) {
			bool refersToSourceWorksheet = false;
			if (thingWithSheetDefinitionOrNull == null) {
				bool noSheetDefinitionCurrentWorksheetIsSourceSheet = Object.ReferenceEquals(currentWorksheet, sourceWorksheet);
				refersToSourceWorksheet = noSheetDefinitionCurrentWorksheetIsSourceSheet;
			}
			else {
				SheetDefinition sheetDefinition = context.GetSheetDefinition(thingWithSheetDefinitionOrNull.SheetDefinitionIndex);
				bool withSheetDefinitionRefersToSourceSheet = string.Equals(sheetDefinition.SheetNameStart, sourceWorksheet.Name, StringComparison.OrdinalIgnoreCase);
				bool isExternal = sheetDefinition.IsExternalReference;
				refersToSourceWorksheet = withSheetDefinitionRefersToSourceSheet && !isExternal;
			}
			return refersToSourceWorksheet;
		}
	}
	#endregion
	#region WorkbookFormulaWalker
	public abstract class WorkbookFormulaWalker : ReplaceThingsPRNVisitor, IWorkBookCellsWalker {
		#region Fields
		readonly WorkbookDataContext context;
		#endregion
		protected WorkbookFormulaWalker(WorkbookDataContext context) {
			this.context = context;
		}
		#region Properties
		public WorkbookDataContext Context { get { return context; } }
		#endregion
		#region IWorkBookCellsWalker Members
		public virtual void Walk(DocumentModel workBook) {
			workBook.Sheets.ForEach(Walk);
			ProcessDefinedNameCollection(workBook.DefinedNames);
			foreach (PivotCache cache in workBook.PivotCaches)
				Walk(cache);
		}
		public virtual void Walk(Worksheet sheet) {
			context.PushCurrentWorksheet(sheet);
			try {
				sheet.Rows.ForEach(Walk);
				ProcessDefinedNameCollection(sheet.DefinedNames);
			}
			finally {
				context.PopCurrentWorksheet();
			}
		}
		public virtual void Walk(Row row) {
			row.Cells.ForEach(Walk);
		}
		public virtual void Walk(ICell cell) {
			FormulaChanged = false;
			if (!cell.HasFormula)
				return;
			FormulaBase formula = cell.GetFormula();
			formula.UpdateFormula(this, cell);
		}
		public virtual void Walk(DefinedNameBase definedNameBase) {
			FormulaChanged = false;
			DefinedName definedName = definedNameBase as DefinedName;
			if (definedName != null) {
				definedName.UpdateExpression(this);
			}
		}
		public virtual void Walk(PivotCache cache) {
			cache.Source.UpdateExpression(this, context.Workbook);
		}
		public virtual void ProcessDefinedNameCollection(DefinedNameCollection collection) {
			foreach (DefinedNameBase definedName in collection)
				Walk(definedName);
		}
		#endregion
	}
	#endregion
	#region ReferenceThingsRPNVisitor
	public abstract class ReferenceThingsRPNVisitor : ReplaceThingsPRNVisitor {
		readonly WorkbookDataContext dataContext;
		protected ReferenceThingsRPNVisitor(WorkbookDataContext dataContext) {
			Guard.ArgumentNotNull(dataContext, "dataContext");
			this.dataContext = dataContext;
		}
		public virtual WorkbookDataContext DataContext { get { return dataContext; } }
		public virtual CellRangeBase ProcessCellRange(CellRange range) {
			return range;
		}
		protected internal virtual ParsedThingBase ProcessRef(ParsedThingRef thing) {
			return thing;
		}
		protected internal virtual ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			return thing;
		}
		protected internal virtual ParsedThingBase ProcessArea(ParsedThingArea thing) {
			return thing;
		}
		protected internal virtual ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			return thing;
		}
		protected virtual internal IWorksheet ShouldProcessSheetDefinition(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = dataContext.GetSheetDefinition(sheetDefinitionIndex);
			return ShouldProcessSheetDefinition(sheetDefinition);
		}
		protected virtual internal IWorksheet ShouldProcessSheetDefinition(SheetDefinition sheetDefinition) {
			if (sheetDefinition.Is3DReference || !sheetDefinition.ValidReference)
				return null;
			IWorksheet sheet = sheetDefinition.GetSheetStart(dataContext);
			System.Diagnostics.Debug.Assert(sheet != null);
			if (!ShouldProcessSheetDefinitionCore(sheetDefinition, sheet.SheetId))
				return null;
			return sheet;
		}
		protected virtual bool ShouldProcessSheetDefinitionCore(SheetDefinition expression, int sheetId) {
			return true;
		}
		#region Ref
		public override void Visit(ParsedThingRef thing) {
			ParsedThingBase newThing = ProcessRef(thing);
			if (!Object.ReferenceEquals(newThing, thing))
				ReplaceCurrentExpression(newThing);
		}
		public override void Visit(ParsedThingRefRel thing) {
			ParsedThingBase newThing = ProcessRefRel(thing);
			if (!Object.ReferenceEquals(newThing, thing))
				ReplaceCurrentExpression(newThing);
		}
		public override void Visit(ParsedThingRef3d thing) {
			IWorksheet sheet = ShouldProcessSheetDefinition(thing.SheetDefinitionIndex);
			if (sheet == null)
				return;
			dataContext.PushCurrentWorksheet(sheet);
			try {
				ParsedThingBase newThing = ProcessRef(thing);
				if (!Object.ReferenceEquals(newThing, thing))
					ReplaceCurrentExpression(newThing);
			}
			finally {
				dataContext.PopCurrentWorksheet();
			}
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			IWorksheet sheet = ShouldProcessSheetDefinition(thing.SheetDefinitionIndex);
			if (sheet == null)
				return;
			dataContext.PushCurrentWorksheet(sheet);
			try {
				ParsedThingBase newThing = ProcessRefRel(thing);
				if (!Object.ReferenceEquals(newThing, thing))
					ReplaceCurrentExpression(newThing);
			}
			finally {
				dataContext.PopCurrentWorksheet();
			}
		}
		#endregion
		#region Area
		public override void Visit(ParsedThingArea thing) {
			ParsedThingBase newThing = ProcessArea(thing);
			if (!Object.ReferenceEquals(newThing, thing))
				ReplaceCurrentExpression(newThing);
		}
		public override void Visit(ParsedThingArea3d thing) {
			IWorksheet sheet = ShouldProcessSheetDefinition(thing.SheetDefinitionIndex);
			if (sheet == null)
				return;
			dataContext.PushCurrentWorksheet(sheet);
			try {
				ParsedThingBase newThing = ProcessArea(thing);
				if (!Object.ReferenceEquals(newThing, thing))
					ReplaceCurrentExpression(newThing);
			}
			finally {
				dataContext.PopCurrentWorksheet();
			}
		}
		public override void Visit(ParsedThingAreaN thing) {
			ParsedThingBase newThing = ProcessAreaRel(thing);
			if (!Object.ReferenceEquals(newThing, thing))
				ReplaceCurrentExpression(newThing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			IWorksheet sheet = ShouldProcessSheetDefinition(thing.SheetDefinitionIndex);
			if (sheet == null)
				return;
			dataContext.PushCurrentWorksheet(sheet);
			try {
				ParsedThingBase newThing = ProcessAreaRel(thing);
				if (!Object.ReferenceEquals(newThing, thing))
					ReplaceCurrentExpression(newThing);
			}
			finally {
				dataContext.PopCurrentWorksheet();
			}
		}
		#endregion
	}
	#endregion
	#region GetFormulaRangesRPNVisitor
	public class GetFormulaRangesRPNVisitor : ParsedThingVisitor {
		readonly WorkbookDataContext context;
		readonly List<DefinedNameBase> definedNames;
		readonly List<CellRangeBase> resultRangeList;
		readonly DefinedNameBase hostDefinedName;
		IWorksheet filterSheet;
		public GetFormulaRangesRPNVisitor(WorkbookDataContext context, DefinedNameBase hostDefinedName) {
			this.context = context;
			resultRangeList = new List<CellRangeBase>();
			definedNames = new List<DefinedNameBase>();
			this.hostDefinedName = hostDefinedName;
		}
		public WorkbookDataContext Context { get { return context; } }
		public CellRangeBase Perform(ParsedExpression expression, IWorksheet filterSheet) {
			if (expression == null)
				return null;
			this.filterSheet = filterSheet;
			Process(expression);
			int i = 0;
			while (i < definedNames.Count) {
				Process(definedNames[i].Expression);
				i++;
			}
			if (resultRangeList.Count <= 0)
				return null;
			if (resultRangeList.Count == 1)
				return resultRangeList[0];
			return new CellUnion(resultRangeList);
		}
		#region Area
		protected internal virtual ParsedThingBase ProcessArea(ParsedThingArea thing) {
			resultRangeList.Add(thing.CellRange.Clone(filterSheet));
			return thing;
		}
		protected internal virtual ParsedThingBase ProcessAreaRel(ParsedThingAreaN thing) {
			CellPosition topLeft = thing.First.ToCellPosition(Context);
			CellPosition bottomRight = thing.Last.ToCellPosition(Context);
			CellRange range = new CellRange(filterSheet, topLeft, bottomRight);
			resultRangeList.Add(range);
			return thing;
		}
		public override void Visit(ParsedThingArea thing) {
			ProcessArea(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			ProcessAreaRel(thing);
		}
		public override void Visit(ParsedThingArea3d thing) {
			if (!ShouldProcessSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ProcessArea(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			if (!ShouldProcessSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ProcessAreaRel(thing);
		}
		#endregion
		#region Ref
		protected internal virtual ParsedThingBase ProcessRef(ParsedThingRef thing) {
			CellRange range = new CellRange(filterSheet, thing.Position, thing.Position);
			resultRangeList.Add(range);
			return thing;
		}
		protected internal virtual ParsedThingBase ProcessRefRel(ParsedThingRefRel thing) {
			CellPosition position = thing.Location.ToCellPosition(Context);
			CellRange range = new CellRange(filterSheet, position, position);
			resultRangeList.Add(range);
			return thing;
		}
		public override void Visit(ParsedThingRef thing) {
			ProcessRef(thing);
		}
		public override void Visit(ParsedThingRefRel thing) {
			ProcessRefRel(thing);
		}
		public override void Visit(ParsedThingRef3d thing) {
			if (!ShouldProcessSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ProcessRef(thing);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			if (!ShouldProcessSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ProcessRefRel(thing);
		}
		#endregion
		protected internal virtual bool ShouldProcessSheetDefinition(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition == null || sheetDefinition.IsExternalReference)
				return false;
			List<IWorksheet> sheets = sheetDefinition.GetReferencedSheets(Context);
			if (!sheets.Contains(filterSheet))
				return false;
			return true;
		}
		#region DefinedName
		protected internal virtual void ProcessDefinedName(DefinedNameBase definedNameObject) {
			if (definedNameObject != null && definedNameObject != hostDefinedName) {
				if (!this.definedNames.Contains(definedNameObject))
					this.definedNames.Add(definedNameObject);
			}
		}
		public override void Visit(ParsedThingName thing) {
			DefinedNameBase definedNameObject = context.GetDefinedName(thing.DefinedName);
			ProcessDefinedName(definedNameObject);
		}
		public override void Visit(ParsedThingNameX thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition == null || sheetDefinition.IsExternalReference)
				return;
			DefinedNameBase definedNameObject = context.GetDefinedName(thing.DefinedName, sheetDefinition);
			ProcessDefinedName(definedNameObject);
		}
		#endregion
	}
	#endregion
	#region FormulaFunctionsVisitor
	public abstract class FormulaFunctionsVisitor {
		protected void Process(ParsedExpression expression) {
			Stack<int> paramCounts = new Stack<int>();
			int count = 0;
			int level = 0;
			for (int i = expression.Count - 1; i >= 0; i--) {
				IParsedThing thing = expression[i];
				bool shouldProcessStack = false;
				if (IsFunction(thing)) {
					paramCounts.Push(count);
					count = GetFuncParamCount(thing);
					level++;
					ProcessFunction(thing as ParsedThingFunc, level);
					shouldProcessStack = true;
				}
				else if (count > 0) {
					count += GetCountDelta(thing);
					if (count < 0)
						count = 0;
					shouldProcessStack = true;
				}
				if (shouldProcessStack)
					while (count == 0 && level > 0) {
						count = paramCounts.Pop();
						level--;
						if (count > 0)
							count--;
					}
			}
		}
		protected abstract void ProcessFunction(ParsedThingFunc thing, int level); 
		int GetFuncParamCount(IParsedThing thing) {
			ParsedThingFuncVar funcVar = thing as ParsedThingFuncVar;
			if (funcVar != null)
				return funcVar.ParamCount;
			ParsedThingFunc func = thing as ParsedThingFunc;
			return func.Function.Parameters.Count;
		}
		int GetCountDelta(IParsedThing thing) {
			if (IsFunction(thing))
				return GetFuncParamCount(thing) - 1;
			if (IsBinaryOperator(thing))
				return 1;
			if (IsUnaryOperator(thing) || IsAttribute(thing) || IsMemToken(thing) || IsDisplayToken(thing) || IsControlToken(thing))
				return 0;
			return -1;
		}
		bool IsFunction(IParsedThing thing) {
			return thing is ParsedThingFunc;
		}
		bool IsBinaryOperator(IParsedThing thing) {
			return thing is BinaryParsedThing;
		}
		bool IsUnaryOperator(IParsedThing thing) {
			return thing is UnaryParsedThing;
		}
		bool IsControlToken(IParsedThing thing) {
			return thing is ParsedThingAttrIf ||
				thing is ParsedThingAttrChoose ||
				thing is ParsedThingAttrGoto;
		}
		bool IsAttribute(IParsedThing thing) {
			return thing is ParsedThingAttrBase;
		}
		bool IsDisplayToken(IParsedThing thing) {
			return thing is ParsedThingAttrSpaceBase || thing is ParsedThingParentheses;
		}
		bool IsMemToken(IParsedThing thing) {
			return thing is ParsedThingMemBase;
		}
	}
	public class FunctionsNestedLevelVisitor : FormulaFunctionsVisitor {
		#region Fields
		int maxNestedLevel = 0;
		#endregion
		public int Calculate(ParsedExpression expression) {
			Process(expression);
			return maxNestedLevel;
		}
		protected override void ProcessFunction(ParsedThingFunc thing, int level) {
			maxNestedLevel = Math.Max(level, maxNestedLevel);
		}
	}
	#endregion
	#region FormulaFindFunctionVisitor
	public class FormulaFindFunctionVisitor : ParsedThingVisitor {
		#region fields
		ParsedExpression expression;
		int functionCode;
		bool hasFunction;
		#endregion
		public FormulaFindFunctionVisitor(ParsedExpression expression)
			: base() {
			this.expression = expression;
		}
		public override void Visit(ParsedThingFuncVar thing) {
			base.Visit(thing);
			if (thing.FuncCode == functionCode)
				hasFunction = true;
		}
		public bool FindFunction(int functionCode) {
			this.functionCode = functionCode;
			this.hasFunction = false;
			this.Process(expression);
			return this.hasFunction;
		}
	}
	#endregion
	#region ExternalLinkRemovedFormulaWalker
	public class ExternalLinkRemovedFormulaWalker : WorkbookFormulaWalker {
		readonly int linkIndex;
		readonly List<DefinedName> deletingDefinedNames;
		public ExternalLinkRemovedFormulaWalker(int linkIndex, WorkbookDataContext context)
			: base(context) {
			this.linkIndex = linkIndex;
			deletingDefinedNames = new List<DefinedName>();
		}
		public override void Walk(DocumentModel workBook) {
			ProcessDefinedNameCollection(workBook.DefinedNames);
			workBook.Sheets.ForEach(ProcessSheetDefinedNames);
			DeleteDefinedNames(workBook);
			workBook.Sheets.ForEach(Walk);
		}
		void DeleteDefinedNames(DocumentModel workBook) {
			foreach (DefinedName definedName in deletingDefinedNames) {
				int scopedSheetId = definedName.ScopedSheetId;
				if (scopedSheetId < 0)
					workBook.RemoveDefinedName(definedName);
				else
					workBook.Sheets.GetById(scopedSheetId).RemoveDefinedName(definedName);
			}
		}
		void ProcessSheetDefinedNames(Worksheet sheet) {
			ProcessDefinedNameCollection(sheet.DefinedNames);
		}
		public override void Walk(Worksheet sheet) {
			Context.PushCurrentWorksheet(sheet);
			try {
				sheet.Rows.ForEach(Walk);
			}
			finally {
				Context.PopCurrentWorksheet();
			}
		}
		public override void Walk(ICell cell) {
			Context.PushCurrentCell(cell);
			try {
				base.Walk(cell);
			}
			finally {
				Context.PopCurrentCell();
			}
		}
		public override void Walk(DefinedNameBase definedNameBase) {
			Context.PushDefinedNameProcessing(definedNameBase);
			try {
				base.Walk(definedNameBase);
			}
			finally {
				Context.PopDefinedNameProcessing();
			}
		}
		bool LookupLinkInSheetDefinition(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(sheetDefinitionIndex);
			return sheetDefinition.ExternalReferenceIndex == linkIndex + 1;
		}
		void ReplaceToCalculatedValue(VariantValue value) {
			if (!value.IsCellRange) {
				ParsedExpression expression = BasicExpressionCreator.CreateExpressionForVariantValue(value, OperandDataType.Default, Context);
				ReplaceCurrentExpression(expression[0]);
			}
			else {
				if (Context.DefinedNameProcessing)
					deletingDefinedNames.Add((DefinedName)Context.DefinedNameProcessingStack.Peek());
				InvalidateFormula();
			}
		}
		public override void Visit(ParsedThingArea3d thing) {
			if (!LookupLinkInSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ReplaceToCalculatedValue(thing.EvaluateToValue(Context));
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			if (!LookupLinkInSheetDefinition(thing.SheetDefinitionIndex))
				return;
			if (Context.DefinedNameProcessing)
				deletingDefinedNames.Add((DefinedName)Context.DefinedNameProcessingStack.Peek());
			else
				ReplaceToCalculatedValue(thing.EvaluateToValue(Context));
		}
		public override void Visit(ParsedThingAreaErr3d thing) {
			if (!LookupLinkInSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ReplaceToCalculatedValue(thing.EvaluateToValue(Context));
		}
		public override void Visit(ParsedThingErr3d thing) {
			if (!LookupLinkInSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ReplaceToCalculatedValue(thing.EvaluateToValue(Context));
		}
		public override void Visit(ParsedThingNameX thing) {
			if (!LookupLinkInSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ReplaceToCalculatedValue(thing.EvaluateToValue(Context));
		}
		public override void Visit(ParsedThingRef3d thing) {
			if (!LookupLinkInSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ReplaceToCalculatedValue(thing.EvaluateToValue(Context));
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			if (!LookupLinkInSheetDefinition(thing.SheetDefinitionIndex))
				return;
			if (Context.DefinedNameProcessing)
				deletingDefinedNames.Add((DefinedName)Context.DefinedNameProcessingStack.Peek());
			else
				ReplaceToCalculatedValue(thing.EvaluateToValue(Context));
		}
		public override void Visit(ParsedThingTableExt thing) {
			if (!LookupLinkInSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ReplaceToCalculatedValue(thing.EvaluateToValue(Context));
		}
		public override void Visit(ParsedThingUnknownFuncExt thing) {
			if (!LookupLinkInSheetDefinition(thing.SheetDefinitionIndex))
				return;
			ReplaceCurrentExpression(new ParsedThingError(NameError.Instance));
		}
	}
	#endregion
	#region DefinedNameReferencedRangesRPNVisitor
	public class DefinedNameReferencedRangesRPNVisitor : ParsedThingVisitor {
		readonly WorkbookDataContext context;
		readonly HashSet<DefinedNameReference> definedNames;
		readonly List<CellRange> resultRangeList;
		public DefinedNameReferencedRangesRPNVisitor(WorkbookDataContext context) {
			this.context = context;
			resultRangeList = new List<CellRange>();
			definedNames = new HashSet<DefinedNameReference>();
		}
		public WorkbookDataContext Context { get { return context; } }
		public ReferencesCache Perform(ParsedExpression expression) {
			if (expression == null)
				return null;
			ReferencesCache result = new ReferencesCache();
			context.PushCurrentWorkbook(context.CurrentWorkbook, false);
			try {
				Process(expression);
			}
			finally {
				context.PopCurrentWorkbook();
			}
			if (definedNames.Count > 0)
				result.DefinedNameReferences = new List<DefinedNameReference>(definedNames);
			if (resultRangeList.Count > 0)
				result.RangeReferences = resultRangeList;
			return result;
		}
		protected internal virtual void ProcessRef3dBase(ParsedThingRefBase thing, int sheetDefinitionIndex) {
			CellRange reference = thing.PreEvaluateReference(context);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition.IsExternalReference)
				return;
			if (sheetDefinition.IsCurrentSheetReference) {
				reference.Worksheet = null;
				resultRangeList.Add(reference);
			}
			else {
				VariantValue resultRangeValue = sheetDefinition.AssignSheetDefinition(reference, context);
				if (resultRangeValue.IsCellRange)
					resultRangeValue.CellRangeValue.AddRanges(resultRangeList);
			}
		}
		protected internal virtual void ProcessRefBase(ParsedThingRefBase thing) {
			resultRangeList.Add(thing.PreEvaluateReference(context));
		}
		#region Area
		public override void Visit(ParsedThingArea thing) {
			ProcessRefBase(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			ProcessRefBase(thing);
		}
		public override void Visit(ParsedThingArea3d thing) {
			ProcessRef3dBase(thing, thing.SheetDefinitionIndex);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			ProcessRef3dBase(thing, thing.SheetDefinitionIndex);
		}
		#endregion
		#region Ref
		public override void Visit(ParsedThingRef thing) {
			ProcessRefBase(thing);
		}
		public override void Visit(ParsedThingRefRel thing) {
			ProcessRefBase(thing);
		}
		public override void Visit(ParsedThingRef3d thing) {
			ProcessRef3dBase(thing, thing.SheetDefinitionIndex);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			ProcessRef3dBase(thing, thing.SheetDefinitionIndex);
		}
		#endregion
		#region DefinedName
		public override void Visit(ParsedThingName thing) {
			DefinedNameReference definedNameReference = new DefinedNameReference(thing.DefinedName, -1);
			if (!this.definedNames.Contains(definedNameReference))
				this.definedNames.Add(definedNameReference);
		}
		public override void Visit(ParsedThingNameX thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition == null || sheetDefinition.IsExternalReference || sheetDefinition.Is3DReference)
				return;
			int sheetId;
			if (sheetDefinition.IsCurrentSheetReference)
				sheetId = -2;
			else {
				IWorksheet sheet = sheetDefinition.GetSheetStart(context);
				if(sheet == null)
					return;
				sheetId = sheet.SheetId;
			}
			DefinedNameReference definedNameReference = new DefinedNameReference(thing.DefinedName, sheetId);
			if (!this.definedNames.Contains(definedNameReference))
				this.definedNames.Add(definedNameReference);
		}
		#endregion
	}
	#endregion
	public abstract class StackMachineVisitor<T> : ParsedThingVisitor {
		readonly Stack<T> stack;
		protected StackMachineVisitor() {
			this.stack = new Stack<T>();
		}
		protected Stack<T> Stack { get { return stack; } }
		public virtual T Calculate(ParsedExpression expression) {
			Guard.ArgumentNotNull(expression, "expression");
			for (int i = 0; i < expression.Count; i++) {
				IParsedThing thing = expression[i];
				thing.Visit(this);
			}
			if (stack.Count != 1)
				Exceptions.ThrowInternalException();
			return Stack.Pop();
		}
		protected abstract T ProcessBinaryValue(ParsedThingBase thing, T left, T right);
		protected abstract T ProcessUnaryValue(ParsedThingBase thing, T value);
		protected abstract T ProcessOperand(ParsedThingBase thing);
		protected abstract T ProcessFunction(ParsedThingFunc thing, List<T> arguments);
		public override void VisitBinary(ParsedThingBase thing) {
			base.VisitBinary(thing);
			System.Diagnostics.Debug.Assert(stack.Count >= 2);
			T right = stack.Pop();
			T left = stack.Pop();
			stack.Push(ProcessBinaryValue(thing, left, right));
		}
		public override void VisitUnary(ParsedThingBase thing) {
			base.VisitUnary(thing);
			System.Diagnostics.Debug.Assert(stack.Count >= 1);
			T value = stack.Pop();
			stack.Push(ProcessUnaryValue(thing, value));
		}
		public override void VisitOperand(ParsedThingBase thing) {
			base.VisitOperand(thing);
			stack.Push(ProcessOperand(thing));
		}
		public override void VisitFunction(ParsedThingFunc thing) {
			base.VisitFunction(thing);
			int paramCount = GetFuncParamCount(thing);
			System.Diagnostics.Debug.Assert(stack.Count >= paramCount);
			List<T> arguments = new List<T>();
			for (int i = 0; i < paramCount; i++)
				arguments.Add(stack.Pop());
			stack.Push(ProcessFunction(thing, arguments));
		}
		int GetFuncParamCount(ParsedThingFunc thing) {
			ParsedThingFuncVar funcVar = thing as ParsedThingFuncVar;
			if (funcVar != null)
				return funcVar.ParamCount;
			return thing.Function.Parameters.Count;
		}
	}
}
