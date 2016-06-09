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

using DevExpress.Office;
using System.Collections.Generic;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	public abstract class ParsedThingRefBase : ParsedThingWithDataType {
		protected ParsedThingRefBase()
			: base() {
		}
		protected ParsedThingRefBase(OperandDataType dataType)
			: base(dataType) {
		}
		protected internal abstract CellRange PreEvaluateReference(WorkbookDataContext context);
		protected abstract string GetExpressionString(WorkbookDataContext context);
		#region BuildExpressionString
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			builder.Append(GetExpressionString(context));
			spacesBuilder.Remove(0, spacesBuilder.Length);
		}
		#endregion
		#region Evaluate
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(EvaluateToValue(context));
		}
		public virtual VariantValue EvaluateToValue(WorkbookDataContext context) {
			VariantValue result = new VariantValue();
			result.CellRangeValue = PreEvaluateReference(context);
			if (DataType == OperandDataType.Value) {
				if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
					result = context.DereferenceValue(result, false);
			}
			else
				if (DataType == OperandDataType.Array) {
					result = context.DereferenceValue(result, false);
					VariantArray array = VariantArray.Create(1, 1);
					array.SetValue(0, 0, result);
					result = VariantValue.FromArray(array);
				}
			return result;
		}
		#endregion
		#region GetInvolvedCellRanges
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			CellRange range = PreEvaluateReference(context);
			CellRangeList result = new CellRangeList();
			result.Add(range);
			stack.Push(result);
		}
		#endregion
		internal protected virtual ParsedThingBase GetRefErrorEquivalent() {
			return new ParsedThingRefErr();
		}
	}
	#region Ref
	public class ParsedThingRef : ParsedThingRefBase {
		#region Fields
		CellPosition position;
		#endregion
		public ParsedThingRef() {
		}
		public ParsedThingRef(CellPosition position) {
			this.position = position;
		}
		public ParsedThingRef(CellPosition position, OperandDataType dataType)
			: base(dataType) {
			this.position = position;
		}
		#region Properties
		public CellPosition Position { get { return position; } set { position = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		protected override string GetExpressionString(WorkbookDataContext context) {
			return Position.ToString(context);
		}
		protected internal override CellRange PreEvaluateReference(WorkbookDataContext context) {
			return new CellRange(context.CurrentWorksheet, Position, Position);
		}
		public override IParsedThing Clone() {
			ParsedThingRef clone = new ParsedThingRef();
			clone.DataType = DataType;
			clone.Position = Position;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingRef anotherPtg = (ParsedThingRef)obj;
			return this.Position.Equals(anotherPtg.Position);
		}
	}
	#endregion
	#region RefErr(#REF!)
	public class ParsedThingRefErr : ParsedThingWithDataType {
		public ParsedThingRefErr() {
		}
		public ParsedThingRefErr(OperandDataType dataType)
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
			stack.Push(EvaluateToValue(context));
		}
		public VariantValue EvaluateToValue(WorkbookDataContext context) {
			return VariantValue.ErrorReference;
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			stack.Push(ParsedThingBase.EmptyCellRangeList);
		}
		public override IParsedThing Clone() {
			ParsedThingRefErr clone = new ParsedThingRefErr();
			clone.DataType = DataType;
			return clone;
		}
	}
	#endregion
	#region Ref3d
	public class ParsedThingRef3d : ParsedThingRef, IParsedThingWithSheetDefinition {
		#region Fields
		int sheetDefinitionIndex;
		#endregion
		public ParsedThingRef3d() {
		}
		public ParsedThingRef3d(CellPosition position)
			: base(position) {
		}
		public ParsedThingRef3d(CellPosition position, int sheetDefinitionIndex)
			: base(position) {
			this.sheetDefinitionIndex = sheetDefinitionIndex;
		}
		public ParsedThingRef3d(CellPosition position, int sheetDefinitionIndex, OperandDataType dataType)
			: base(position, dataType) {
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
			CellRange range = PreEvaluateReference(context);
			VariantValue result = new VariantValue();
			result.CellRangeValue = range;
			result = sheetDefinition.AssignSheetDefinition(result, context);
			if (DataType == OperandDataType.Value) {
				if (result.IsCellRange && result.CellRangeValue.Worksheet == null)
					result = VariantValue.ErrorReference;
				else
					if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
						result = context.DereferenceValue(result, false);
			}
			else if (DataType == OperandDataType.Array) {
				if (result.IsCellRange) {
					if (result.CellRangeValue.RangeType == CellRangeType.UnionRange)
						result = VariantValue.ErrorReference;
					else
						if (result.CellRangeValue.Worksheet == null)
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
				if (DataType == OperandDataType.Value) {
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
			ParsedThingRef3d clone = new ParsedThingRef3d();
			clone.DataType = DataType;
			clone.Position = Position;
			clone.sheetDefinitionIndex = sheetDefinitionIndex;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingRef3d anotherPtg = (ParsedThingRef3d)obj;
			return this.sheetDefinitionIndex == anotherPtg.SheetDefinitionIndex;
		}
		internal protected override ParsedThingBase GetRefErrorEquivalent() {
			return new ParsedThingErr3d(sheetDefinitionIndex);
		}
	}
	#endregion
	#region RefErr3d (=Sheet1!#REF!)
	public class ParsedThingErr3d : ParsedThingWithDataType, IParsedThingWithSheetDefinition {
		#region Fields
		int sheetDefinitionIndex;
		#endregion
		#region Properties
		public int SheetDefinitionIndex { get { return sheetDefinitionIndex; } set { sheetDefinitionIndex = value; } }
		#endregion
		public ParsedThingErr3d() {
		}
		public ParsedThingErr3d(int sheetDefinitionIndex)
			: this(sheetDefinitionIndex, OperandDataType.Reference) {
			this.sheetDefinitionIndex = sheetDefinitionIndex;
		}
		public ParsedThingErr3d(int sheetDefinitionIndex, OperandDataType dataType)
			: base(dataType) {
			this.sheetDefinitionIndex = sheetDefinitionIndex;
		}
		#region Properties
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition.ValidReference || !context.ImportExportMode)
				sheetDefinition.BuildExpressionString(builder, context);
			builder.Append(CellErrorFactory.GetErrorName(ReferenceError.Instance, context));
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(EvaluateToValue(context));
		}
		public VariantValue EvaluateToValue(WorkbookDataContext context) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			return sheetDefinition.Is3DReference ? VariantValue.ErrorName : VariantValue.ErrorReference;
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			stack.Push(ParsedThingBase.EmptyCellRangeList);
		}
		public override IParsedThing Clone() {
			ParsedThingErr3d clone = new ParsedThingErr3d(sheetDefinitionIndex);
			clone.DataType = DataType;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingErr3d anotherPtg = (ParsedThingErr3d)obj;
			return this.sheetDefinitionIndex == anotherPtg.sheetDefinitionIndex;
		}
	}
	#endregion
	#region ParsedThingRefRel
	public class ParsedThingRefRel : ParsedThingRefBase, ISupportsCopyFrom<ParsedThingRefRel> {
		#region Fields
		CellOffset location;
		#endregion
		public ParsedThingRefRel() {
		}
		public ParsedThingRefRel(CellOffset offset) {
			this.location = offset;
		}
		public ParsedThingRefRel(CellOffset offset, OperandDataType dataType)
			: base(dataType) {
			this.location = offset;
		}
		#region Properties
		public CellOffset Location { get { return location; } set { location = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		protected override string GetExpressionString(WorkbookDataContext context) {
			CellPosition position = Location.ToCellPosition(context);
			return position.ToString(context);
		}
		protected internal override CellRange PreEvaluateReference(WorkbookDataContext context) {
			CellPosition position = location.ToCellPosition(context);
			return new CellRange(context.CurrentWorksheet, position, position);
		}
		public override IParsedThing Clone() {
			ParsedThingRefRel clone = new ParsedThingRefRel(Location);
			clone.CopyFrom(this);
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingRefRel anotherPtg = (ParsedThingRefRel)obj;
			return this.location.Equals(anotherPtg.location);
		}
		public void CopyFrom(ParsedThingRefRel value) {
			base.CopyFrom(value);
			this.location = value.location;
		}
	}
	#endregion
	#region ParsedThingRef3dRel
	public class ParsedThingRef3dRel : ParsedThingRefRel, IParsedThingWithSheetDefinition, ISupportsCopyFrom<ParsedThingRef3dRel> {
		#region Fields
		int sheetDefinitionIndex;
		#endregion
		public ParsedThingRef3dRel() {
		}
		public ParsedThingRef3dRel(CellOffset offset, int sheetDefinitionIndex)
			: base(offset) {
			this.sheetDefinitionIndex = sheetDefinitionIndex;
		}
		public ParsedThingRef3dRel(CellOffset offset, int sheetDefinitionIndex, OperandDataType dataType)
			: base(offset, dataType) {
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
			CellRange range = PreEvaluateReference(context);
			VariantValue result = new VariantValue();
			result.CellRangeValue = range;
			result = sheetDefinition.AssignSheetDefinition(result, context);
			if (DataType == OperandDataType.Value) {
				if (result.IsCellRange && result.CellRangeValue.Worksheet == null)
					result = VariantValue.ErrorReference;
				else
					if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
						result = context.DereferenceValue(result, false);
			}
			else if (DataType == OperandDataType.Array) {
				if (result.IsCellRange) {
					if (result.CellRangeValue.RangeType == CellRangeType.UnionRange)
						result = VariantValue.ErrorReference;
					else
						if (result.CellRangeValue.Worksheet == null)
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
				if (DataType == OperandDataType.Value) {
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
			ParsedThingRef3dRel clone = new ParsedThingRef3dRel();
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(ParsedThingRef3dRel value) {
			base.CopyFrom(value);
			this.sheetDefinitionIndex = value.sheetDefinitionIndex;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingRef3dRel anotherPtg = obj as ParsedThingRef3dRel;
			if (anotherPtg == null)
				return false;
			return this.sheetDefinitionIndex == anotherPtg.sheetDefinitionIndex;
		}
		internal protected override ParsedThingBase GetRefErrorEquivalent() {
			return new ParsedThingErr3d(sheetDefinitionIndex);
		}
	}
	#endregion
}
