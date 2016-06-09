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
using System.Diagnostics;
using DevExpress.Office;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IParsedThing
	public interface IParsedThing : ICloneable<IParsedThing> {
		OperandDataType DataType { get; set; }
		void Visit(IParsedThingVisitor visitor);
		void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context);
		void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context);
		void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context);
		bool IsEqual(IParsedThing obj);
	}
	#endregion
	public interface IParsedThingWithSheetDefinition {
		int SheetDefinitionIndex { get; set; }
	}
	#region ParsedThingList
	public class ParsedThingList : List<IParsedThing> { }
	#endregion
	#region ParsedExpression
	public class ParsedExpression : ParsedThingList, ICloneable<ParsedExpression> {
		public string BuildExpressionString(WorkbookDataContext context) {
			Stack<int> stack = new Stack<int>();
			StringBuilder builder = new StringBuilder();
			StringBuilder spacesBuilder = new StringBuilder();
			for (int i = 0; i < Count; i++) {
				IParsedThing thing = this[i];
				thing.BuildExpressionString(stack, builder, spacesBuilder, context);
			}
			if (stack.Count != 1)
				DevExpress.Office.Utils.Exceptions.ThrowInternalException();
			return builder.ToString();
		}
		public VariantValue Evaluate(WorkbookDataContext context) {
			context.Workbook.SuppressCellCreation = true;
			Stack<VariantValue> stack = new Stack<VariantValue>();
			Stack<bool> gotoStack = new Stack<bool>();
			int i = 0;
			DocumentModel documentModel = context.Workbook;
			try {
					documentModel.RealTimeDataManager.OnStartExpressionEvaluation();
				while (i < Count) {
					IParsedThing thing = this[i];
					ParsedThingMemBase memThing = thing as ParsedThingMemBase;
					if (memThing != null) {
						Debug.Assert(Count - i > memThing.InnerThingCount);
						ParsedExpression memInner = new ParsedExpression();
						memInner.AddRange(this.GetRange(i + 1, memThing.InnerThingCount));
						i += memThing.InnerThingCount;
						stack.Push(memThing.EvaluateInner(memInner, context));
					}
					else {
						ParsedThingAttrIf attrIf = thing as ParsedThingAttrIf;
						if (attrIf != null)
							i += EvaluateAttrIf(stack, gotoStack, context, attrIf);
						else {
							ParsedThingAttrChoose attrChoose = thing as ParsedThingAttrChoose;
							if (attrChoose != null)
								i += EvaluateAttrChoose(stack, gotoStack, context, attrChoose);
							else {
								ParsedThingAttrGoto attrGoto = thing as ParsedThingAttrGoto;
								if (attrGoto != null)
									i += EvaluateAttrGoto(stack, gotoStack, attrGoto);
								else {
									if (gotoStack.Count > 0) {
										ParsedThingFunc funcThing = thing as ParsedThingFunc;
										if (funcThing != null) {
											int code = funcThing.FuncCode;
											if (code == 0x0001  || code == 0x0064)
												gotoStack.Pop();
										}
									}
									thing.Evaluate(stack, context);
									if (documentModel.CalculationChain.Enabled && stack.Count > 0 && stack.Peek() == VariantValue.ErrorGettingData) {
										return VariantValue.ErrorGettingData;
									}
								}
							}
						}
					}
					i++;
				}
				if (stack.Count != 1)
					DevExpress.Office.Utils.Exceptions.ThrowInternalException();
				VariantValue result = stack.Pop();
				if (result.IsEmpty)
					result = 0;
				return result;
			}
			finally {
				context.Workbook.SuppressCellCreation = false;
					documentModel.RealTimeDataManager.OnEndExpressionEvaluation();
			}
		}
		int EvaluateAttrIf(Stack<VariantValue> stack, Stack<bool> gotoStack, WorkbookDataContext context, ParsedThingAttrIf attrIf) {
			Debug.Assert(stack.Count > 0);
			int result = 0;
			VariantValue condition = stack.Peek();
			bool canUseOptimization = !condition.IsArray && !condition.IsCellRange && !condition.IsError;
			if (canUseOptimization) {
				condition = condition.ToBoolean(context);
				canUseOptimization = !condition.IsError;
			}
			if (canUseOptimization) {
				stack.Pop();
				stack.Push(condition.BooleanValue);
				if (!condition.BooleanValue) {
					result = attrIf.Offset;
					stack.Push(VariantValue.Empty);
				}
				gotoStack.Push(true);
			}
			else
				gotoStack.Push(false);
			return result;
		}
		int EvaluateAttrChoose(Stack<VariantValue> stack, Stack<bool> gotoStack, WorkbookDataContext context, ParsedThingAttrChoose attrChoose) {
			Debug.Assert(stack.Count > 0);
			int result = 0;
			VariantValue condition = stack.Peek();
			if (condition.IsArray) {
				gotoStack.Push(false);
				return 0;
			}
			int paramsCount = attrChoose.Offsets.Count;
			int numericIndex = 0;
			int fakeValuesCount = paramsCount;
			condition = condition.ToNumeric(context);
			stack.Pop();
			if (condition.IsError) {
				stack.Push(condition);
				result = attrChoose.Offsets[paramsCount - 1];
			}
			else {
				numericIndex = (int)condition.NumericValue;
				if (numericIndex < 1 || numericIndex > 254 || numericIndex >= paramsCount + 1) {
					stack.Push(VariantValue.ErrorInvalidValueInFunction);
					result = attrChoose.Offsets[paramsCount - 1];
				}
				else {
					if (numericIndex == paramsCount) {
						stack.Push(paramsCount);
						fakeValuesCount = paramsCount - 1; 
					}
					else {
						stack.Push(paramsCount - 1);
						fakeValuesCount = paramsCount - 2; 
					}
					if (numericIndex > 1)
						result = attrChoose.Offsets[numericIndex - 2];
				}
			}
			for (int i = 0; i < fakeValuesCount; i++)
				stack.Push(VariantValue.Empty);
			gotoStack.Push(true);
			return result;
		}
		int EvaluateAttrGoto(Stack<VariantValue> stack, Stack<bool> gotoStack, ParsedThingAttrGoto attrGoto) {
			int result = 0;
			if (gotoStack.Count > 0 && gotoStack.Peek()) {
				if (attrGoto.Offset != 1)
					stack.Push(VariantValue.Empty);
				result = attrGoto.Offset - 1;
			}
			return result;
		}
		public List<CellRangeBase> GetInvolvedCellRanges(WorkbookDataContext context) {
			FormulaInvolvedRangesCalculator calculator = new FormulaInvolvedRangesCalculator(context);
			return calculator.Calculate(this);
		}
		public ParsedExpression Clone() {
			ParsedExpression clone = new ParsedExpression();
			for (int i = 0; i < Count; i++) {
				clone.Add(this[i].Clone());
			}
			return clone;
		}
		public override bool Equals(object obj) {
			if (obj == null)
				return false;
			ParsedExpression anotherExpression = obj as ParsedExpression;
			if (anotherExpression == null)
				return false;
			int count = Count;
			if (count != anotherExpression.Count)
				return false;
			for (int i = 0; i < count; i++) {
				if (!this[i].IsEqual(anotherExpression[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public bool ContainsInternalFunction { get { return CheckProperty(FormulaProperties.HasInternalFunction); } }
		public bool IsVolatile { get { return CheckProperty(FormulaProperties.HasVolatileFunction); } }
		public bool ContainsCustomFunction { get { return CheckProperty(FormulaProperties.HasCustomFunction); } }
		public bool ContainsFunctionRTD { get { return CheckProperty(FormulaProperties.HasFunctionRTD); } }
		bool CheckProperty(FormulaProperties property) {
			return ((GetProperties() & property) > 0);
		}
		public FormulaProperties GetProperties() {
			if (Count > 0) {
				ParsedThingAttrSemi parsedThing = this[0] as ParsedThingAttrSemi;
				if (parsedThing != null)
					return parsedThing.FormulaProperties;
			}
			return FormulaProperties.None;
		}
#if DEBUGTEST
		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < Count; i++) {
				builder.AppendLine("#" + (i + 1) + ":" + this[i].ToString());
			}
			return builder.ToString();
		}
#endif
		public static bool IsNullOrEmpty(ParsedExpression expression) {
			return expression == null || expression.Count == 0;
		}
	}
	#endregion
	#region ParsedThingBase
	public abstract class ParsedThingBase : IParsedThing, ISupportsCopyFrom<ParsedThingBase> {
		static readonly CellRangeList emptyCellRangeList = new CellRangeList();
		public static CellRangeList EmptyCellRangeList { get { return emptyCellRangeList; } }
		OperandDataType dataType;
		#region IParsedThing Members
		public OperandDataType DataType {
			get { return GetOperandDataType(this.dataType); }
			set { this.dataType = value; }
		}
		public abstract IParsedThing Clone();
		public abstract void Visit(IParsedThingVisitor visitor);
		public virtual void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
		}
		public virtual void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
		}
		public virtual void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
		}
		#endregion
		public int GetDataType() {
			switch (DataType) {
				case OperandDataType.Reference:
					return 1;
				case OperandDataType.Value:
					return 2;
				case OperandDataType.Array:
					return 3;
			}
			return 0;
		}
		protected virtual OperandDataType GetOperandDataType(OperandDataType operandDataType) {
			return OperandDataType.None;
		}
		public virtual bool IsEqual(IParsedThing obj) {
			return this.GetType() == obj.GetType();
		}
		public void CopyFrom(ParsedThingBase value) {
			this.dataType = value.dataType;
		}
	}
	#endregion
	#region ParsedThingWithDataType
	public abstract class ParsedThingWithDataType : ParsedThingBase {
		protected ParsedThingWithDataType() {
		}
		protected ParsedThingWithDataType(OperandDataType dataType) {
			this.DataType = dataType;
		}
		protected override OperandDataType GetOperandDataType(OperandDataType operandDataType) {
			if (operandDataType == OperandDataType.None)
				return OperandDataType.Reference;
			return operandDataType;
		}
		public override string ToString() {
			return base.ToString() + ", Type:" + DataType;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			return this.DataType == (obj as ParsedThingWithDataType).DataType;
		}
	}
	#endregion
	#region Basic tokens
	public abstract class ParsedThingPositionBase : ParsedThingBase {
		CellPosition position;
		#region Properties
		public CellPosition Position { get { return position; } set { position = value; } }
		#endregion
	}
	public class ParsedThingExp : ParsedThingPositionBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			return this;
		}
	}
	public class ParsedThingDataTable : ParsedThingPositionBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			return this;
		}
	}
	public class ParsedThingParentheses : ParsedThingBase {
		#region Fields
		static ParsedThingParentheses instance = new ParsedThingParentheses();
		#endregion
		ParsedThingParentheses() {
		}
		#region Properties
		public static ParsedThingParentheses Instance {
			get {
				return instance;
			}
		}
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			Debug.Assert(stack.Count >= 1);
			int position = stack.Peek();
			builder.Insert(position, "(");
			builder.Insert(position, spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			builder.Append(")");
		}
		public override IParsedThing Clone() {
			return this;
		}
	}
	#endregion
	#region Auxiliary parsed things
	#region IHeplerParsedThing
	public interface IHeplerParsedThing {
	}
	#endregion
	#region ParsedThingVariantValue
	public class ParsedThingVariantValue : ValueParsedThing, IHeplerParsedThing {
		#region Fields
		VariantValue value;
		#endregion
		public ParsedThingVariantValue(VariantValue value) {
			this.value = value;
		}
		#region Properties
		public VariantValue Value { get { return value; } set { this.value = value; } }
		#endregion
		public override VariantValue GetValue() {
			return value;
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			switch (value.Type) {
				case VariantValueType.None:
					return;
				case VariantValueType.Array:
				case VariantValueType.CellRange:
					DevExpress.Office.Utils.Exceptions.ThrowInternalException();
					break;
				case VariantValueType.Error:
					builder.Append(CellErrorFactory.GetErrorName(value.ErrorValue, context));
					break;
				case VariantValueType.Boolean:
					builder.Append(value.BooleanValue ? VariantValue.TrueConstant : VariantValue.FalseConstant);
					break;
				case VariantValueType.InlineText:
					builder.Append("\"");
					builder.Append(value.InlineTextValue.Replace("\"", "\"\""));
					builder.Append("\"");
					break;
				case VariantValueType.SharedString:
					builder.Append(value.GetTextValue(context.StringTable));
					break;
				case VariantValueType.Numeric:
					builder.Append(value.ToText(context).InlineTextValue);
					break;
				default:
					DevExpress.Office.Utils.Exceptions.ThrowInternalException();
					break;
			}
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(value);
		}
		public override IParsedThing Clone() {
			return new ParsedThingVariantValue(value);
		}
	}
	#endregion
	#endregion
	#region BasicExpressionCreator
	public enum BasicExpressionCreatorParameter {
		None = 0,
		ShouldCreate3d = 1,
		ShouldCreateRelative = 2,
		ShouldEncloseUnion = 4,
	}
	public interface ICellRangeExpressionCreatorLogic {
		IParsedThing GetRefThing(CellPosition position);
		IParsedThing GetRef3DThing(CellPosition position, int sheetDefinitionIndex);
		IParsedThing GetAreaThing(CellRange range);
		IParsedThing GetArea3dThing(CellRange range, int sheetDefinitionIndex);
	}
	public class CellRangeExpressionCreatorLogic : ICellRangeExpressionCreatorLogic {
		readonly OperandDataType dataType;
		public CellRangeExpressionCreatorLogic(OperandDataType dataType) {
			this.dataType = dataType;
		}
		public IParsedThing GetRefThing(CellPosition position) { return new ParsedThingRef(position) { DataType = dataType }; }
		public IParsedThing GetRef3DThing(CellPosition position, int sheetDefinitionIndex) { return new ParsedThingRef3d(position, sheetDefinitionIndex) { DataType = dataType }; }
		public IParsedThing GetAreaThing(CellRange range) { return new ParsedThingArea(range) { DataType = dataType }; }
		public IParsedThing GetArea3dThing(CellRange range, int sheetDefinitionIndex) { return new ParsedThingArea3d(range, sheetDefinitionIndex) { DataType = dataType }; }
	}
	public class CellRangeRelativeExpressionCreatorLogic : ICellRangeExpressionCreatorLogic {
		readonly OperandDataType dataType;
		public CellRangeRelativeExpressionCreatorLogic(OperandDataType dataType) {
			this.dataType = dataType;
		}
		public IParsedThing GetRefThing(CellPosition position) { return new ParsedThingRefRel(position.ToCellOffset()) { DataType = dataType }; }
		public IParsedThing GetRef3DThing(CellPosition position, int sheetDefinitionIndex) { return new ParsedThingRef3dRel(position.ToCellOffset(), sheetDefinitionIndex) { DataType = dataType }; }
		public IParsedThing GetAreaThing(CellRange range) { return new ParsedThingAreaN(range.TopLeft.ToCellOffset(), range.BottomRight.ToCellOffset()) { DataType = dataType }; }
		public IParsedThing GetArea3dThing(CellRange range, int sheetDefinitionIndex) { return new ParsedThingArea3dRel(range.TopLeft.ToCellOffset(), range.BottomRight.ToCellOffset(), sheetDefinitionIndex) { DataType = dataType }; }
	}
	public static class BasicExpressionCreator {
		public static ParsedExpression CreateExpressionForVariantValue(VariantValue value, OperandDataType dataType, WorkbookDataContext context) {
			ParsedExpression result = new ParsedExpression();
			if (value.IsCellRange)
				CreateCellRangeExpression(result, value.CellRangeValue, BasicExpressionCreatorParameter.ShouldCreate3d | BasicExpressionCreatorParameter.ShouldEncloseUnion, dataType, context);
			else if (value.IsArray) {
				if (dataType == OperandDataType.Default)
					dataType = OperandDataType.Array;
				result.Add(new ParsedThingArray() { ArrayValue = value.ArrayValue, DataType = dataType });
			}
			else
				result.Add(ValueParsedThing.CreateInstance(value, context));
			return result;
		}
		public static void CreateCellRangeExpression(ParsedExpression expression, CellRangeBase range, BasicExpressionCreatorParameter parameter, OperandDataType dataType, WorkbookDataContext context) {
			if (dataType == OperandDataType.Default)
				dataType = OperandDataType.Reference;
			ICellRangeExpressionCreatorLogic logic = CreateLogic(parameter, dataType);
			CreateCellRangeExpression(expression, range, parameter, logic, context);
		}
		static void CreateCellRangeExpression(ParsedExpression expression, CellRangeBase range, BasicExpressionCreatorParameter parameter, ICellRangeExpressionCreatorLogic logic, WorkbookDataContext context) {
			if (range.RangeType == CellRangeType.UnionRange)
				CreateCellUnionExpression(expression, range as CellUnion, parameter, logic, context);
			else
				CreateCellSingleRangeExpression(expression, range as CellRange, parameter, logic, context);
		}
		static ICellRangeExpressionCreatorLogic CreateLogic(BasicExpressionCreatorParameter parameter, OperandDataType dataType) {
			ICellRangeExpressionCreatorLogic result;
			if ((parameter & BasicExpressionCreatorParameter.ShouldCreateRelative) > 0)
				result = new CellRangeRelativeExpressionCreatorLogic(dataType);
			else
				result = new CellRangeExpressionCreatorLogic(dataType);
			return result;
		}
		static void CreateCellSingleRangeExpression(ParsedExpression expression, CellRange range, BasicExpressionCreatorParameter parameter, ICellRangeExpressionCreatorLogic logic, WorkbookDataContext context) {
			expression.Add(CreateCellSingleRangePtg(range, parameter, logic, context));
		}
		public static IParsedThing CreateCellSingleRangePtg(CellRange range, BasicExpressionCreatorParameter parameter, WorkbookDataContext context) {
			ICellRangeExpressionCreatorLogic logic = CreateLogic(parameter, OperandDataType.Reference);
			return CreateCellSingleRangePtg(range, parameter, logic, context);
		}
		static IParsedThing CreateCellSingleRangePtg(CellRange range, BasicExpressionCreatorParameter parameter, ICellRangeExpressionCreatorLogic logic, WorkbookDataContext context) {
			if ((parameter & BasicExpressionCreatorParameter.ShouldCreate3d) > 0) {
				SheetDefinition sheetDefinition = new SheetDefinition();
				sheetDefinition.SheetNameStart = range.Worksheet.Name;
				int sheetDefinitionIndex = context.RegisterSheetDefinition(sheetDefinition);
				if (range.Width == 1 && range.Height == 1)
					return logic.GetRef3DThing(range.TopLeft, sheetDefinitionIndex);
				else
					return logic.GetArea3dThing(range, sheetDefinitionIndex);
			}
			else {
				if (range.Width == 1 && range.Height == 1)
					return logic.GetRefThing(range.TopLeft);
				else
					return logic.GetAreaThing(range);
			}
		}
		static void CreateCellUnionExpression(ParsedExpression expression, CellUnion range, BasicExpressionCreatorParameter parameter, ICellRangeExpressionCreatorLogic logic, WorkbookDataContext context) {
			List<CellRangeBase> innerCellRanges = range.InnerCellRanges;
			CreateCellRangeExpression(expression, innerCellRanges[0], parameter, logic, context);
			int count = innerCellRanges.Count;
			for (int i = 1; i < count; i++) {
				CreateCellRangeExpression(expression, innerCellRanges[i], parameter, logic, context);
				expression.Add(ParsedThingUnion.Instance);
			}
			if ((parameter & BasicExpressionCreatorParameter.ShouldEncloseUnion) > 0)
				expression.Add(ParsedThingParentheses.Instance);
		}
	}
	#endregion
}
