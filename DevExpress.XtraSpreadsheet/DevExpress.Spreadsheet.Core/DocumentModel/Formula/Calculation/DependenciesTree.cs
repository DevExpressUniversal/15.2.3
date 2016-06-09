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

using DevExpress.Office.Utils;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Utils.Trees;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	#region CellPrecedentsRTree
	public class CellPrecedentsRTree {
		readonly RTree2D<IChainPrecedent> tree = new RTree2D<IChainPrecedent>();
		Dictionary<CellKey, IChainPrecedent> cellSinglePrecedents = new Dictionary<CellKey, IChainPrecedent>();
		public int Count { get { return tree.Count; } }
		internal RTree2D<IChainPrecedent> InnerTree { get { return tree; } }
		public Dictionary<CellKey, IChainPrecedent> CellSinglePrecedents { get { return cellSinglePrecedents; } }
		public void Insert(IChainPrecedent item, CellRange range) {
			if (range.CellCount == 1)
				InsertToSinglePrecedents(item, range);
			else
				InsertToTree(item, range);
		}
		void InsertToSinglePrecedents(IChainPrecedent item, CellRange range) {
			CellKey precedentCell = new CellKey(range.SheetId, range.TopLeft.Column, range.TopLeft.Row);
			IChainPrecedent cellPrecedents = null;
			if (CellSinglePrecedents.TryGetValue(precedentCell, out cellPrecedents)) {
				if (cellPrecedents.AllowsMerging)
					cellPrecedents.MergeWith(item);
				else
					CellSinglePrecedents[precedentCell] = new ChainPrecedentList(cellPrecedents, item);
			}
			else
				CellSinglePrecedents.Add(precedentCell, item);
		}
		void InsertToTree(IChainPrecedent item, CellRange range) {
			tree.Insert(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height, item);
		}
		public void Remove(CellRange range, IChainPrecedent item) {
			if (range.CellCount == 1) {
				CellKey precedentCell = new CellKey(range.SheetId, range.TopLeft.Column, range.TopLeft.Row);
				IChainPrecedent cellPrecedents = null;
				if (CellSinglePrecedents.TryGetValue(precedentCell, out cellPrecedents)) {
					if (!cellPrecedents.AllowsMerging || !cellPrecedents.Remove(item)) 
						CellSinglePrecedents.Remove(precedentCell);
				}
			}
			else
				RemoveFromTree(range, item);
		}
		void RemoveFromTree(CellRangeBase range, IChainPrecedent item) {
			NodeBase nodeData = new NodeBase(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height);
			tree.Delete(nodeData, item, true);
		}
		public void Clear() {
			tree.Clear();
		}
		#region Search
		public IList<IChainPrecedent> Search(CellRange range) {
			IList<IChainPrecedent> result = SearchTree(range);
			AddSinglePrecedents(result, range);
			return result;
		}
		public IList<IChainPrecedent> Search(CellKey key) {
			IList<IChainPrecedent> list = SearchTree(key.ColumnIndex, key.RowIndex);
			AddSinglePrecedents(list, key);
			return list;
		}
		void AddSinglePrecedents(IList<IChainPrecedent> where, CellRange range) {
			foreach (KeyValuePair<CellKey, IChainPrecedent> pair in CellSinglePrecedents) {
				if (range.ContainsCell(pair.Key))
					where.Add(pair.Value);
			}
		}
		IList<IChainPrecedent> SearchTree(CellRange range) {
			if (tree.Count <= 0)
				return new List<IChainPrecedent>();
			NodeBase nodeData = new NodeBase(range.TopLeft.Column, range.TopLeft.Row, range.Width, range.Height);
			return tree.Search(nodeData);
		}
		IList<IChainPrecedent> SearchTree(int columnIndex, int rowIndex) {
			if (tree.Count <= 0)
				return new List<IChainPrecedent>();
			NodeBase nodeData = new NodeBase(columnIndex, rowIndex, 1, 1);
			return tree.Search(nodeData);
		}
		void AddSinglePrecedents(IList<IChainPrecedent> where, CellKey key) {
			IChainPrecedent singlePrecedents;
			if (CellSinglePrecedents.TryGetValue(key, out singlePrecedents))
				where.Add(singlePrecedents);
		}
		#endregion
	}
	#endregion
	#region CellDependentsTreeBuilder
	public class CellDependentsTreeBuilder {
		readonly DocumentModel documentModel;
		readonly ChainBaisedCalculationLogic logic;
		public CellDependentsTreeBuilder(DocumentModel documentModel, ChainBaisedCalculationLogic logic) {
			Guard.ArgumentNotNull(logic, "logic");
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.logic = logic;
		}
		public void CalculateChain(bool registerCells) {
			ProcessWorkbook(registerCells);
		}
		bool ProcessWorkbook(bool registerCells) {
			int count = documentModel.SheetCount;
			for (int i = 0; i < count; i++) {
				Worksheet sheet = documentModel.Sheets[i];
				CellPrecedentsRTree sheetTree = new CellPrecedentsRTree();
				logic.PrecedentsTrees.Add(sheet.SheetId, sheetTree);
			}
			for (int i = 0; i < count; i++) {
				Worksheet sheet = documentModel.Sheets[i];
				foreach (CellBase info in sheet.GetExistingCells())
					if (!ProcessCellCore((ICell)info, registerCells))
						return false;
				foreach (SharedFormula sharedFormula in documentModel.Sheets[i].SharedFormulas)
					ProcessSharedFormula(sharedFormula);
			}
			return true;
		}
		public void ProcessSharedFormula(SharedFormula formula) {
			if (formula.IsVolatile())
				return;
			List<PrecedentPair> precedentPairs = formula.GetInvolvedCellRanges();
			if (precedentPairs.Count <= 0)
				return;
			foreach (PrecedentPair precedentPair in precedentPairs)
				ProcessChainPrecedentPair(precedentPair);
		}
		void ProcessChainPrecedentPair(PrecedentPair precedentPair) {
			CellRange cellRange = precedentPair.Range;
			ICellTable sheet = cellRange.Worksheet;
			if (sheet != null && !(sheet is ExternalWorksheet)) {
				IChainPrecedent precedent = precedentPair.Precedent;
				CellPrecedentsRTree tree = GetPrecedentsTree(sheet.SheetId);
				tree.Insert(precedent, cellRange);
			}
		}
		public bool ProcessCell(ICell cell) {
			CellPrecedentsRTree sheetTree = null;
			int sheetId = cell.Sheet.SheetId;
			if (!logic.PrecedentsTrees.TryGetValue(sheetId, out sheetTree)) {
				sheetTree = new CellPrecedentsRTree();
				logic.PrecedentsTrees.Add(sheetId, sheetTree);
			}
			return ProcessCellCore(cell, true);
		}
		bool ProcessCellCore(ICell cell, bool registerCell) {
			if (cell.HasFormula) {
				if (!ProcessCellFormula(cell, registerCell))
					return false;
			}
			return true;
		}
		bool ProcessCellFormula(ICell cell, bool registerCell) {
			FormulaBase formula = cell.GetFormula();
			if (registerCell)
				logic.Chain.RegisterCell(cell);
			if (formula.IsVolatile()) {
				FormulaFactory.SetFormulaCalculateAlways(cell, true);
				return true;
			}
			if (!cell.HasFormula) 
				return true;
			List<PrecedentPair> precedentPairs = formula.GetInvolvedCellRanges(cell);
			if (precedentPairs.Count <= 0)
				return true;
			int count = precedentPairs.Count;
			for (int i = 0; i < count; i++) {
				ProcessChainPrecedentPair(precedentPairs[i]);
			}
			return true;
		}
		CellPrecedentsRTree GetPrecedentsTree(int sheetId) {
			CellPrecedentsRTree tree = null;
			if (!logic.PrecedentsTrees.TryGetValue(sheetId, out tree)) {
				tree = new CellPrecedentsRTree();
				logic.PrecedentsTrees.Add(sheetId, tree);
			}
			return tree;
		}
		#region RemoveCellFormula
		public void RemoveCellFormula(ICell cell) {
			logic.Chain.UnRegisterCell(cell);
			FormulaBase formula = cell.Formula;
			if (formula.IsVolatile()) {
				return;
			}
			List<PrecedentPair> precedentPairs = formula.GetInvolvedCellRanges(cell);
			if (precedentPairs == null || precedentPairs.Count <= 0)
				return;
			foreach (PrecedentPair precedentPair in precedentPairs) {
				RemoveCellFormulaFormPrecedentPair(precedentPair);
			}
		}
		void RemoveCellFormulaFormPrecedentPair(PrecedentPair precedentPair) {
			CellRange cellRange = precedentPair.Range;
			ICellTable sheet = cellRange.Worksheet;
			if (sheet == null || sheet is ExternalWorksheet)
				return;
			IChainPrecedent precedent = precedentPair.Precedent;
			CellPrecedentsRTree tree = GetPrecedentsTree(sheet.SheetId);
			tree.Remove((CellRange)cellRange, precedent);
		}
		#endregion
		#region RemoveSharedFormula
		public void RemoveSharedFormula(SharedFormula sharedFormula) {
			if (sharedFormula.IsVolatile())
				return;
			List<PrecedentPair> precedentPairs = sharedFormula.GetInvolvedCellRanges();
			if (precedentPairs == null || precedentPairs.Count <= 0)
				return;
			foreach (PrecedentPair precedentPair in precedentPairs) {
				RemoveCellFormulaFormPrecedentPair(precedentPair);
			}
		}
		#endregion
	}
	#endregion
	#region CellPrecedentsHolder
	public class CellPrecedentsHolder {
		ISheetPosition affectedRange;
		IList<IChainPrecedent> precedents;
		public CellPrecedentsHolder(ISheetPosition affectedRange, IList<IChainPrecedent> precedents) {
			this.affectedRange = affectedRange;
			this.precedents = precedents;
		}
		public ISheetPosition AffectedRange { get { return affectedRange; } }
		public IList<IChainPrecedent> Precedents { get { return precedents; } }
	}
	#endregion
	#region IChainPrecedent
	public interface IChainPrecedent {
		bool AllowsMerging { get; }
		void MarkUpForRecalculation();
		void MergeWith(IChainPrecedent item);
		void AddItemsTo(IList<ICell> where, ISheetPosition affectedRange);
		bool Remove(IChainPrecedent cell);
		CellRangeBase ToRange(ISheetPosition affectedRange);
	}
	#endregion
	#region CellList
	public class ChainPrecedentList : IChainPrecedent {
		List<IChainPrecedent> innerList;
		public ChainPrecedentList() {
			innerList = new List<IChainPrecedent>();
		}
		public ChainPrecedentList(IChainPrecedent item1, IChainPrecedent item2)
			: this() {
			innerList.Add(item1);
			innerList.Add(item2);
		}
		internal List<IChainPrecedent> InnerList { get { return innerList; } }
		#region IChainPrecedent Members
		public void MarkUpForRecalculation() {
			foreach (IChainPrecedent item in innerList)
				item.MarkUpForRecalculation();
		}
		public bool AllowsMerging { get { return true; } }
		public void MergeWith(IChainPrecedent item) {
			innerList.Add(item);
		}
		public void AddItemsTo(IList<ICell> where, ISheetPosition affectedRange) {
			foreach (IChainPrecedent item in innerList)
				item.AddItemsTo(where, affectedRange);
		}
		public CellRangeBase ToRange(ISheetPosition affectedRange) {
			CellRangeBase result = null;
			foreach (IChainPrecedent item in innerList) {
				result = CellRangeBase.MergeRanges(result, item.ToRange(affectedRange));
			}
			return result;
		}
		public bool Remove(IChainPrecedent cell) {
			innerList.Remove(cell);
			return true;
		}
		#endregion
	}
	#endregion
	#region SharedFormulaEnlargedRangeChainPrecedent
	public class SharedFormulaEnlargedRangeChainPrecedent : IChainPrecedent {
		#region Fields
		SharedFormula hostSharedFormula; 
		CellRange sourceRange;
		#endregion
		public SharedFormulaEnlargedRangeChainPrecedent(SharedFormula hostSharedFormula, CellRange sourceRange) {
			Guard.ArgumentNotNull(sourceRange, "sourceRange");
			Guard.ArgumentNotNull(hostSharedFormula, "hostSharedFormula");
			this.hostSharedFormula = hostSharedFormula;
			this.sourceRange = sourceRange;
		}
		public CellRange SourceRange {
			[System.Diagnostics.DebuggerStepThrough]
			get { return sourceRange; }
		}
		public SharedFormula HostSharedFormula {
			[System.Diagnostics.DebuggerStepThrough]
			get { return hostSharedFormula; }
		}
		#region IChainPrecedent Members
		public bool AllowsMerging { get { return false; } }
		public virtual void MarkUpForRecalculation() {
			foreach (ICell cell in hostSharedFormula.GetGuestCellsEnumerable())
				cell.MarkUpForRecalculation();
		}
		public virtual void MergeWith(IChainPrecedent item) {
			throw new InvalidOperationException();
		}
		public virtual void AddItemsTo(IList<ICell> where, ISheetPosition affectedRange) {
			CellRange affectedFormulaRange = hostSharedFormula.GetFormulaAffectedRangeByReferencedRange(sourceRange, affectedRange);
			foreach (ICellBase affectedCellBase in affectedFormulaRange.GetExistingCellsEnumerable()) {
				ICell affectedCell = affectedCellBase as ICell;
				if (affectedCell != null && affectedCell.FormulaType == FormulaType.Shared) {
					SharedFormulaRef formulaRef = (SharedFormulaRef)affectedCell.Formula;
					if (Object.ReferenceEquals(formulaRef.HostSharedFormula, hostSharedFormula))
						where.Add(affectedCell);
				}
			}
		}
		public CellRangeBase ToRange(ISheetPosition affectedRange) {
			CellRange affectedFormulaRange = hostSharedFormula.GetFormulaAffectedRangeByReferencedRange(sourceRange, affectedRange);
			CellRangeBase result = null;
			foreach (ICellBase affectedCellBase in affectedFormulaRange.GetExistingCellsEnumerable()) {
				ICell affectedCell = affectedCellBase as ICell;
				if (affectedCell != null && affectedCell.FormulaType == FormulaType.Shared) {
					SharedFormulaRef formulaRef = (SharedFormulaRef)affectedCell.Formula;
					if (Object.ReferenceEquals(formulaRef.HostSharedFormula, hostSharedFormula))
						result = CellRangeBase.MergeRanges(result, affectedCell.GetRange());
				}
			}
			return result;
		}
		public virtual bool Remove(IChainPrecedent cell) {
			return false;
		}
		#endregion
		public override bool Equals(object obj) {
			SharedFormulaEnlargedRangeChainPrecedent other = obj as SharedFormulaEnlargedRangeChainPrecedent;
			if (other == null)
				return false;
			if (!object.ReferenceEquals(hostSharedFormula, other.hostSharedFormula))
				return false;
			return sourceRange.EqualsPosition(other.SourceRange);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ sourceRange.GetHashCode() ^ hostSharedFormula.GetHashCode();
		}
	}
	#endregion
	#region RangeChainPrecedent
	public class ArrayFormulaRangeChainPrecedent : IChainPrecedent {
		public CellRange Range { get; set; }
		public ArrayFormulaRangeChainPrecedent(CellRange range) {
			this.Range = range;
		}
		#region IChainPrecedent Members
		public bool AllowsMerging { get { return false; } }
		public void MarkUpForRecalculation() {
			foreach (ICellBase cellBase in Range.GetExistingCellsEnumerable()) {
				ICell cell = cellBase as ICell;
				if (cell != null)
					cell.MarkUpForRecalculation();
			}
		}
		public void MergeWith(IChainPrecedent item) {
			throw new InvalidOperationException();
		}
		public void AddItemsTo(IList<ICell> where, ISheetPosition affectedRange) {
			foreach (ICellBase cellBase in Range.GetExistingCellsEnumerable()) {
				ICell cell = cellBase as ICell;
				if (cell != null)
					where.Add(cell);
			}
		}
		public bool Remove(IChainPrecedent cell) {
			return false;
		}
		public CellRangeBase ToRange(ISheetPosition affectedRange) {
			return Range.Clone();
		}
		#endregion
		public override bool Equals(object obj) {
			ArrayFormulaRangeChainPrecedent other = obj as ArrayFormulaRangeChainPrecedent;
			if (other == null)
				return false;
			return Range.EqualsPosition(other.Range);
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ Range.GetHashCode();
		}
	}
	#endregion
	#region PrecedentPair
	public class PrecedentPair {
		public CellRange Range { get; set; }
		public IChainPrecedent Precedent { get; set; }
		public PrecedentPair(CellRange range, IChainPrecedent precedent) {
			this.Range = range;
			this.Precedent = precedent;
		}
		public override string ToString() {
			return Range.ToString() + ", " + Precedent.ToString();
		}
	}
	#endregion
	public class FormulaInvolvedRangesCalculator : ParsedThingVisitor {
		readonly WorkbookDataContext context;
		readonly Stack<CellRangeList> stack;
		long memThingCounter = -1;
		ParsedThingMemBase currentMemThing;
		public FormulaInvolvedRangesCalculator(WorkbookDataContext context) {
			this.context = context;
			this.stack = new Stack<CellRangeList>();
		}
		protected Stack<CellRangeList> Stack { get { return stack; } }
		public virtual CellRangeList Calculate(ParsedExpression expression) {
			Guard.ArgumentNotNull(expression, "expression");
			for (int i = 0; i < expression.Count; i++) {
				IParsedThing thing = expression[i];
				thing.Visit(this);
				if (memThingCounter == 0) {
					stack.Push(currentMemThing.ConvertInvolvedRangesToDataType(stack.Pop(), context));
					currentMemThing = null;
				}
				memThingCounter--;
			}
			if (stack.Count != 1)
				Exceptions.ThrowInternalException();
			return Stack.Pop();
		}
		public override void VisitMem(ParsedThingMemBase thing) {
			System.Diagnostics.Debug.Assert(currentMemThing == null);
			currentMemThing = thing;
			memThingCounter = thing.InnerThingCount;
		}
		public override void VisitUnary(ParsedThingBase thing) {
		}
		public override void VisitBinary(ParsedThingBase thing) {
			System.Diagnostics.Debug.Assert(Stack.Count >= 2);
			CellRangeList list2 = Stack.Pop();
			CellRangeList list1 = Stack.Pop();
			CellRangeList result = new CellRangeList();
			result.AddRange(list1);
			result.AddRange(list2);
			Stack.Push(result);
		}
		void ProcessBinaryReference(BinaryReferenceParsedThing thing) {
			System.Diagnostics.Debug.Assert(Stack.Count >= 2);
			CellRangeList list2 = Stack.Pop();
			CellRangeList list1 = Stack.Pop();
			CellRangeList result = new CellRangeList();
			if (list1.Count == 1 && list2.Count == 1) {
				VariantValue evaluationResult = thing.EvaluateCellRanges(context, list1[0], list2[0]);
				if (evaluationResult.IsCellRange)
					result.Add(evaluationResult.CellRangeValue);
			}
			else {
				result.AddRange(list1);
				result.AddRange(list2);
			}
			Stack.Push(result);
		}
		public override void Visit(ParsedThingRange thing) {
			ProcessBinaryReference(thing);
		}
		public override void Visit(ParsedThingIntersect thing) {
			ProcessBinaryReference(thing);
		}
		public override void Visit(ParsedThingUnion thing) {
			ProcessBinaryReference(thing);
		}
		#region Operand
		public override void VisitOperand(ParsedThingBase thing) {
			stack.Push(ParsedThingBase.EmptyCellRangeList);
		}
		public override void Visit(ParsedThingTable thing) {
			CellRangeList result = new CellRangeList();
			VariantValue value = thing.PreEvaluate(context);
			if (!value.IsError && value.IsCellRange) {
				CellRangeBase range = value.CellRangeValue;
				if (thing.DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
					CellPosition position = context.DereferenceToCellPosition(range);
					if (position.IsValid)
						result.Add(new CellRange(range.Worksheet, position, position));
				}
				else
					result.Add(range);
			}
			Stack.Push(result);
		}
		#region Reference
		public override void Visit(ParsedThingRef thing) {
			CellRange range = thing.PreEvaluateReference(context);
			CellRangeList result = new CellRangeList();
			result.Add(range);
			Stack.Push(result);
		}
		public override void Visit(ParsedThingRefRel thing) {
			CellRange range = thing.PreEvaluateReference(context);
			CellRangeList result = new CellRangeList();
			result.Add(range);
			Stack.Push(result);
		}
		public override void Visit(ParsedThingRef3d thing) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			CellRangeList result = new CellRangeList();
			VariantValue resultRangeValue = sheetDefinition.AssignSheetDefinition(thing.PreEvaluateReference(context), context);
			if (resultRangeValue.IsCellRange) {
				CellRangeBase resultRange = resultRangeValue.CellRangeValue;
				if (thing.DataType == OperandDataType.Value) {
					CellPosition position = context.DereferenceToCellPosition(resultRange);
					if (position.IsValid)
						result.Add(new CellRange(resultRange.Worksheet, position, position));
				}
				else
					if (thing.DataType == OperandDataType.Array) {
						if (resultRange.RangeType != CellRangeType.UnionRange && resultRange.Worksheet != null)
							result.Add(resultRange);
					}
					else
						result.Add(resultRange);
			}
			Stack.Push(result);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			CellRangeList result = new CellRangeList();
			VariantValue resultRangeValue = sheetDefinition.AssignSheetDefinition(thing.PreEvaluateReference(context), context);
			if (resultRangeValue.IsCellRange) {
				CellRangeBase resultRange = resultRangeValue.CellRangeValue;
				if (thing.DataType == OperandDataType.Value) {
					CellPosition position = context.DereferenceToCellPosition(resultRange);
					if (position.IsValid)
						result.Add(new CellRange(resultRange.Worksheet, position, position));
				}
				else
					if (thing.DataType == OperandDataType.Array) {
						if (resultRange.RangeType != CellRangeType.UnionRange && resultRange.Worksheet != null)
							result.Add(resultRange);
					}
					else
						result.Add(resultRange);
			}
			Stack.Push(result);
		}
		#endregion
		#region Area
		public override void Visit(ParsedThingArea thing) {
			CellRangeList result = new CellRangeList();
			CellRange range = thing.PreEvaluateReference(context);
			if (thing.DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
				CellPosition position = context.DereferenceToCellPosition(range);
				if (position.IsValid)
					result.Add(new CellRange(range.Worksheet, position, position));
			}
			else
				result.Add(range);
			Stack.Push(result);
		}
		public override void Visit(ParsedThingArea3d thing) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			CellRangeList result = new CellRangeList();
			VariantValue resultRangeValue = sheetDefinition.AssignSheetDefinition(thing.PreEvaluateReference(context), context);
			if (resultRangeValue.IsCellRange) {
				CellRangeBase resultRange = resultRangeValue.CellRangeValue;
				if (thing.DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
					CellPosition position = context.DereferenceToCellPosition(resultRange);
					if (position.IsValid)
						result.Add(new CellRange(resultRange.Worksheet, position, position));
				}
				else
					if (thing.DataType == OperandDataType.Array) {
						if (resultRange.RangeType != CellRangeType.UnionRange && resultRange.Worksheet != null)
							result.Add(resultRange);
					}
					else
						result.Add(resultRange);
			}
			Stack.Push(result);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			CellRangeList result = new CellRangeList();
			VariantValue resultRangeValue = sheetDefinition.AssignSheetDefinition(thing.PreEvaluateReference(context), context);
			if (resultRangeValue.IsCellRange) {
				CellRangeBase resultRange = resultRangeValue.CellRangeValue;
				if (thing.DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
					CellPosition position = context.DereferenceToCellPosition(resultRange);
					if (position.IsValid)
						result.Add(new CellRange(resultRange.Worksheet, position, position));
				}
				else
					if (thing.DataType == OperandDataType.Array) {
						if (resultRange.RangeType != CellRangeType.UnionRange && resultRange.Worksheet != null)
							result.Add(resultRange);
					}
					else
						result.Add(resultRange);
			}
			Stack.Push(result);
		}
		public override void Visit(ParsedThingAreaN thing) {
			CellRangeList result = new CellRangeList();
			CellRange range = thing.PreEvaluateReference(context);
			if (thing.DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
				CellPosition position = context.DereferenceToCellPosition(range);
				if (position.IsValid)
					result.Add(new CellRange(range.Worksheet, position, position));
			}
			else
				result.Add(range);
			Stack.Push(result);
		}
		#endregion
		#region Function
		public override void VisitFunction(ParsedThingFunc thing) {
			int parametersCount = thing.ParamCount;
			System.Diagnostics.Debug.Assert(Stack.Count >= parametersCount);
			CellRangeList resultList = new CellRangeList();
			for (int i = parametersCount - 1; i >= 0; i--) {
				List<CellRangeBase> value = Stack.Pop();
				resultList.AddRange(value);
			}
			Stack.Push(resultList);
		}
		#endregion
		#region DefinedName
		CellRangeList ProcessDefinedName(ParsedThingName thing) {
			CellRangeList result = new CellRangeList();
			DefinedNameBase definedNameObject = thing.GetDefinedName(context);
			if (definedNameObject != null && definedNameObject.Expression != null) {
				context.PushDefinedNameProcessing(definedNameObject);
				try {
					List<CellRangeBase> innerCellRanges = definedNameObject.Expression.GetInvolvedCellRanges(context);
					if (thing.DataType == OperandDataType.Value && !context.ArrayFormulaProcessing)
						foreach (CellRangeBase innerRange in innerCellRanges)
							PrepareDefinedNameInnerRange(result, innerRange);
					else
						result.AddRange(innerCellRanges);
				}
				finally {
					context.PopDefinedNameProcessing();
				}
			}
			return result;
		}
		void PrepareDefinedNameInnerRange(List<CellRangeBase> ranges, CellRangeBase rangeBase) {
			if (rangeBase.RangeType == CellRangeType.UnionRange) {
				CellUnion unionRange = (CellUnion)rangeBase;
				foreach (CellRangeBase innerRange in unionRange.InnerCellRanges)
					PrepareDefinedNameInnerRange(ranges, innerRange);
			}
			else {
				CellRange range = (CellRange)rangeBase;
				CellPosition position = context.DereferenceToCellPosition(range);
				if (position.IsValid)
					ranges.Add(new CellRange(range.Worksheet, position, position));
			}
		}
		public override void Visit(ParsedThingName thing) {
			Stack.Push(ProcessDefinedName(thing));
		}
		public override void Visit(ParsedThingNameX thing) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition.Is3DReference)
				Stack.Push(ParsedThingBase.EmptyCellRangeList);
			else {
				sheetDefinition.PushSettings(context);
				try {
					Stack.Push(ProcessDefinedName(thing));
				}
				finally {
					context.PopCurrentContextData();
				}
			}
		}
		#endregion
		#endregion
	}
	#region IFormulaInvolvedRange
	public interface IFormulaInvolvedRange {
		CellRangeBase Range { get; }
		void Visit(IFormulaInvolvedRangeVisitor visitor);
	}
	#endregion
	public interface IFormulaInvolvedRangeVisitor {
		void Visit(FormulaInvolvedRange referencedRange);
		void Visit(ArrayFormulaInvolvedRange referencedRange);
	}
	public class FormulaInvolvedRangeList : List<IFormulaInvolvedRange> {
		public static FormulaInvolvedRangeList Empty = new FormulaInvolvedRangeList();
	}
	public class FormulaInvolvedRange : IFormulaInvolvedRange {
		readonly CellRangeBase range;
		public FormulaInvolvedRange(CellRangeBase range) {
			this.range = range;
		}
		public CellRangeBase Range { get { return range; } }
		public void Visit(IFormulaInvolvedRangeVisitor visitor) {
			visitor.Visit(this);
		}
	}
	public class ArrayFormulaInvolvedRange : IFormulaInvolvedRange {
		readonly CellRangeBase range;
		public ArrayFormulaInvolvedRange(CellRangeBase range) {
			this.range = range;
		}
		public CellRangeBase Range { get { return range; } }
		public void Visit(IFormulaInvolvedRangeVisitor visitor) {
			visitor.Visit(this);
		}
	}
}
