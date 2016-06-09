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
using System.Text;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region Area
	public class ParsedThingArea : ParsedThingRefBase {
		#region Fields
		CellRange cellRange;
		#endregion
		public ParsedThingArea() {
		}
		public ParsedThingArea(CellRange cellRange) {
			this.CellRange = cellRange;
		}
		public ParsedThingArea(CellRange cellRange, OperandDataType dataType)
			: base(dataType) {
			this.CellRange = cellRange;
		}
		public ParsedThingArea(CellPosition topLeft, CellPosition bottomRight) {
			this.cellRange = new CellRange(null, topLeft, bottomRight);
		}
		#region Properties
		public CellRange CellRange { get { return cellRange; } set { cellRange = value; } }
		public CellPosition TopLeft { get { return cellRange.TopLeft; } set { cellRange.TopLeft = value; } }
		public CellPosition BottomRight { get { return cellRange.BottomRight; } set { cellRange.BottomRight = value; } }
		#endregion
		protected override string GetExpressionString(WorkbookDataContext context) {
			return CellRange.ToString(context);
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		#region Evaluation
		public override VariantValue EvaluateToValue(WorkbookDataContext context) {
			VariantValue result = new VariantValue();
			CellRange range = PreEvaluateReference(context);
			result.CellRangeValue = range;
			if (DataType == OperandDataType.Value) {
				if (!context.ArrayFormulaProcessing)
					result = context.DereferenceValue(result, false);
			}
			else if (DataType == OperandDataType.Array) {
				RangeVariantArray array = new RangeVariantArray(range);
				result = VariantValue.FromArray(array);
			}
			return result;
		}
		protected internal override CellRange PreEvaluateReference(WorkbookDataContext context) {
			return CellRange.PrepareCellRangeBaseValue(context.CurrentWorksheet, TopLeft, BottomRight);
		}
		#endregion
		#region GetInvolvedCellRanges
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			CellRangeList result = new CellRangeList();
			CellRange range = PreEvaluateReference(context);
			if (DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
				CellPosition position = context.DereferenceToCellPosition(range);
				if (position.IsValid)
					result.Add(new CellRange(range.Worksheet, position, position));
			}
			else
				result.Add(range);
			stack.Push(result);
		}
		#endregion
		public override IParsedThing Clone() {
			ParsedThingArea clone = new ParsedThingArea();
			clone.DataType = DataType;
			clone.CellRange = cellRange.Clone();
			return clone;
		}
		internal protected override ParsedThingBase GetRefErrorEquivalent() {
			return new ParsedThingAreaErr();
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingArea anotherPtg = (ParsedThingArea)obj;
			return this.TopLeft.Equals(anotherPtg.TopLeft) && this.BottomRight.Equals(anotherPtg.BottomRight);
		}
	}
	#endregion
	#region AreaErr
	public class ParsedThingAreaErr : ParsedThingWithDataType {
		public ParsedThingAreaErr()
			: base() {
		}
		public ParsedThingAreaErr(OperandDataType dataType)
			: base(dataType) {
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			builder.Append(CellErrorFactory.GetErrorName(ReferenceError.Instance, context));
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(VariantValue.ErrorReference);
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			stack.Push(ParsedThingBase.EmptyCellRangeList);
		}
		public override IParsedThing Clone() {
			return new ParsedThingAreaErr(DataType);
		}
	}
	#endregion
	#region Area3d
	public class ParsedThingArea3d : ParsedThingArea, IParsedThingWithSheetDefinition {
		#region Fields
		int sheetDefinitionIndex;
		#endregion
		public ParsedThingArea3d() {
		}
		public ParsedThingArea3d(CellRange cellRange)
			: base(cellRange) {
		}
		public ParsedThingArea3d(CellRange cellRange, int sheetDefinitionIndex)
			: base(cellRange) {
			this.sheetDefinitionIndex = sheetDefinitionIndex;
		}
		public ParsedThingArea3d(CellRange cellRange, int sheetDefinitionIndex, OperandDataType dataType)
			: base(cellRange, dataType) {
			this.sheetDefinitionIndex = sheetDefinitionIndex;
		}
		#region Propertries
		public int SheetDefinitionIndex { get { return sheetDefinitionIndex; } set { sheetDefinitionIndex = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			sheetDefinition.BuildExpressionString(builder, context);
			if (!context.ImportExportMode || sheetDefinition.ValidReference)
				builder.Append(GetExpressionString(context));
		}
		public override VariantValue EvaluateToValue(WorkbookDataContext context) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			if (!sheetDefinition.ValidReference)
				return VariantValue.ErrorReference;
			VariantValue result = new VariantValue();
			CellRange range = PreEvaluateReference(context);
			result.CellRangeValue = range;
			result = sheetDefinition.AssignSheetDefinition(result, context);
			if (DataType == OperandDataType.Value) {
				if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
					result = context.DereferenceValue(result, false);
			}
			else if (DataType == OperandDataType.Array) {
				if (result.IsCellRange) {
					if (result.CellRangeValue.RangeType == CellRangeType.UnionRange)
						result = VariantValue.ErrorReference;
					else {
						RangeVariantArray array = new RangeVariantArray((CellRange)result.CellRangeValue);
						result = VariantValue.FromArray(array);
					}
				}
			}
			return result;
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			CellRangeList result = new CellRangeList();
			VariantValue resultRangeValue = sheetDefinition.AssignSheetDefinition(PreEvaluateReference(context), context);
			if (resultRangeValue.IsCellRange) {
				CellRangeBase resultRange = resultRangeValue.CellRangeValue;
				if (DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
					CellPosition position = context.DereferenceToCellPosition(resultRange);
					if (position.IsValid)
						result.Add(new CellRange(resultRange.Worksheet, position, position));
				}
				else
					if (DataType == OperandDataType.Array) {
						if (resultRange.RangeType != CellRangeType.UnionRange && resultRange.Worksheet != null)
							result.Add(resultRange);
					}
					else
						result.Add(resultRange);
			}
			stack.Push(result);
		}
		public override IParsedThing Clone() {
			return new ParsedThingArea3d((CellRange)CellRange.Clone(), sheetDefinitionIndex, DataType);
		}
		internal protected override ParsedThingBase GetRefErrorEquivalent() {
			return new ParsedThingAreaErr3d(sheetDefinitionIndex);
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingArea3d anotherPtg = (ParsedThingArea3d)obj;
			return this.sheetDefinitionIndex == anotherPtg.SheetDefinitionIndex;
		}
	}
	#endregion
	#region AreaErr3d
	public class ParsedThingAreaErr3d : ParsedThingErr3d {
		public ParsedThingAreaErr3d()
			: base() {
		}
		public ParsedThingAreaErr3d(int sheetDefinitionIndex)
			: base(sheetDefinitionIndex) {
		}
		public ParsedThingAreaErr3d(int sheetDefinitionIndex, OperandDataType dataType)
			: base(sheetDefinitionIndex, dataType) {
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(SheetDefinitionIndex);
			if (sheetDefinition.ValidReference || !context.ImportExportMode)
				sheetDefinition.BuildExpressionString(builder, context);
			builder.Append(CellErrorFactory.GetErrorName(ReferenceError.Instance, context));
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(SheetDefinitionIndex);
			stack.Push(sheetDefinition.Is3DReference ? VariantValue.ErrorName : VariantValue.ErrorReference);
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			stack.Push(ParsedThingBase.EmptyCellRangeList);
		}
		public override IParsedThing Clone() {
			return new ParsedThingAreaErr3d(SheetDefinitionIndex, DataType);
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			return obj is ParsedThingAreaErr3d;
		}
	}
	#endregion
	#region AreaRel (area inside sharedformula)
	public class ParsedThingAreaN : ParsedThingRefBase {
		public ParsedThingAreaN() {
		}
		public ParsedThingAreaN(CellOffset first, CellOffset last) {
			this.First = first;
			this.Last = last;
		}
		#region Properties
		public CellOffset First { get; set; }
		public CellOffset Last { get; set; }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		protected override string GetExpressionString(WorkbookDataContext context) {
			CellPosition positionTopLeft = First.ToCellPosition(context);
			CellPosition positionBottomRight = Last.ToCellPosition(context);
			CellRange range = CellRange.PrepareCellRangeBaseValue(context.CurrentWorksheet, positionTopLeft, positionBottomRight);
			return range.ToString(context);
		}
		public override VariantValue EvaluateToValue(WorkbookDataContext context) {
			VariantValue result = new VariantValue();
			CellRange range = PreEvaluateReference(context);
			result.CellRangeValue = range;
			if (DataType == OperandDataType.Value) {
				if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
					result = context.DereferenceValue(result, false);
			}
			else if (DataType == OperandDataType.Array) {
				RangeVariantArray array = new RangeVariantArray(range);
				result = VariantValue.FromArray(array);
			}
			return result;
		}
		protected internal override CellRange PreEvaluateReference(WorkbookDataContext context) {
			CellPosition topLeft = First.ToCellPosition(context);
			CellPosition bottomRight = Last.ToCellPosition(context);
			return CellRange.PrepareCellRangeBaseValue(context.CurrentWorksheet, topLeft, bottomRight);
		}
		#region GetInvolvedCellRanges
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			CellRangeList result = new CellRangeList();
			CellRange range = PreEvaluateReference(context);
			if (DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
				CellPosition position = context.DereferenceToCellPosition(range);
				if (position.IsValid)
					result.Add(new CellRange(range.Worksheet, position, position));
			}
			else
				result.Add(range);
			stack.Push(result);
		}
		#endregion
		public override IParsedThing Clone() {
			ParsedThingAreaN clone = new ParsedThingAreaN();
			clone.DataType = DataType;
			clone.First = First;
			clone.Last = Last;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingAreaN anotherPtg = (ParsedThingAreaN)obj;
			return this.First.Equals(anotherPtg.First) && this.Last.Equals(anotherPtg.Last);
		}
		internal protected override ParsedThingBase GetRefErrorEquivalent() {
			return new ParsedThingAreaErr();
		}
	}
	#endregion
	#region Area3dRel
	public class ParsedThingArea3dRel : ParsedThingAreaN, IParsedThingWithSheetDefinition {
		#region Fields
		int sheetDefinitionIndex;
		#endregion
		public ParsedThingArea3dRel() {
		}
		public ParsedThingArea3dRel(CellOffset first, CellOffset last, int sheetDefinitionIndex)
			: base(first, last) {
			this.sheetDefinitionIndex = sheetDefinitionIndex;
		}
		#region Propertries
		public int SheetDefinitionIndex { get { return sheetDefinitionIndex; } set { sheetDefinitionIndex = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			sheetDefinition.BuildExpressionString(builder, context);
			if (!context.ImportExportMode || sheetDefinition.ValidReference)
				builder.Append(GetExpressionString(context));
		}
		public override VariantValue EvaluateToValue(WorkbookDataContext context) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			if (!sheetDefinition.ValidReference)
				return VariantValue.ErrorReference;
			VariantValue result = new VariantValue();
			CellRange range = PreEvaluateReference(context);
			result.CellRangeValue = range;
			result = sheetDefinition.AssignSheetDefinition(result, context);
			if (DataType == OperandDataType.Value) {
				if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
					result = context.DereferenceValue(result, false);
			}
			else if (DataType == OperandDataType.Array) {
				if (result.IsCellRange) {
					if (result.CellRangeValue.RangeType == CellRangeType.UnionRange)
						result = VariantValue.ErrorReference;
					else {
						RangeVariantArray array = new RangeVariantArray((CellRange)result.CellRangeValue);
						result = VariantValue.FromArray(array);
					}
				}
			}
			return result;
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			CellRangeList result = new CellRangeList();
			VariantValue resultRangeValue = sheetDefinition.AssignSheetDefinition(PreEvaluateReference(context), context);
			if (resultRangeValue.IsCellRange) {
				CellRangeBase resultRange = resultRangeValue.CellRangeValue;
				if (DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
					CellPosition position = context.DereferenceToCellPosition(resultRange);
					if (position.IsValid)
						result.Add(new CellRange(resultRange.Worksheet, position, position));
				}
				else
					if (DataType == OperandDataType.Array) {
						if (resultRange.RangeType != CellRangeType.UnionRange && resultRange.Worksheet != null)
							result.Add(resultRange);
					}
					else
						result.Add(resultRange);
			}
			stack.Push(result);
		}
		public override IParsedThing Clone() {
			ParsedThingArea3dRel clone = new ParsedThingArea3dRel();
			clone.DataType = DataType;
			clone.First = First;
			clone.Last = Last;
			clone.sheetDefinitionIndex = sheetDefinitionIndex;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingArea3dRel anotherPtg = (ParsedThingArea3dRel)obj;
			return this.sheetDefinitionIndex == anotherPtg.sheetDefinitionIndex;
		}
		internal protected override ParsedThingBase GetRefErrorEquivalent() {
			return new ParsedThingAreaErr3d(sheetDefinitionIndex);
		}
	}
	#endregion
}
