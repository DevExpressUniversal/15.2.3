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

using DevExpress.XtraSpreadsheet.Utils;
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ParsedThingName : ParsedThingWithDataType {
		#region Fields
		string definedName;
		#endregion
		public ParsedThingName() {
		}
		public ParsedThingName(string definedName) {
			this.definedName = definedName;
		}
		public ParsedThingName(string definedName, OperandDataType dataType)
			: base(dataType) {
			this.definedName = definedName;
		}
		#region Properties
		public string DefinedName { get { return definedName; } set { definedName = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, System.Text.StringBuilder builder, System.Text.StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			builder.Append(definedName);
		}
		protected virtual VariantValue PreEvaluate(WorkbookDataContext context) {
			DefinedNameBase definedNameObject = GetDefinedName(context);
			if (definedNameObject == null)
				return VariantValue.ErrorName;
			return definedNameObject.Evaluate(context);
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(EvaluateToValue(context));
		}
		public virtual VariantValue EvaluateToValue(WorkbookDataContext context) {
			VariantValue result = PreEvaluate(context);
			return EvaluateCore(result, context);
		}
		protected VariantValue EvaluateCore(VariantValue value, WorkbookDataContext context) {
			if (!value.IsError) {
				if (DataType == OperandDataType.Value) {
					if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
						value = context.DereferenceValue(value, false);
				}
				else
					if (DataType == OperandDataType.Array)
						if (value.IsCellRange) {
							RangeVariantArray array = new RangeVariantArray((CellRange)value.CellRangeValue);
							value = VariantValue.FromArray(array);
						}
			}
			return value;
		}
		protected internal virtual DefinedNameBase GetDefinedName(WorkbookDataContext context) {
			DefinedNameBase definedNameObject = context.GetDefinedName(definedName);
			if (definedNameObject == null)
				return null;
			if (context.DefinedNameProcessingStack.Contains(definedNameObject))
				return null; 
			return definedNameObject;
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			stack.Push(GetInvolvedCellRangesCore(context));
		}
		protected internal CellRangeList GetInvolvedCellRangesCore(WorkbookDataContext context) {
			CellRangeList result = new CellRangeList();
			DefinedNameBase definedNameObject = GetDefinedName(context);
			if (definedNameObject != null && definedNameObject.Expression != null) {
				context.PushDefinedNameProcessing(definedNameObject);
				try {
					List<CellRangeBase> innerCellRanges = definedNameObject.Expression.GetInvolvedCellRanges(context);
					if (DataType == OperandDataType.Value && !context.ArrayFormulaProcessing)
						foreach (CellRangeBase innerRange in innerCellRanges)
							PrepareInnerRange(result, innerRange, context);
					else
						result.AddRange(innerCellRanges);
				}
				finally {
					context.PopDefinedNameProcessing();
				}
			}
			return result;
		}
		void PrepareInnerRange(List<CellRangeBase> ranges, CellRangeBase rangeBase, WorkbookDataContext context) {
			if (rangeBase.RangeType == CellRangeType.UnionRange) {
				CellUnion unionRange = (CellUnion)rangeBase;
				foreach (CellRangeBase innerRange in unionRange.InnerCellRanges)
					PrepareInnerRange(ranges, innerRange, context);
			}
			else {
				CellRange range = (CellRange)rangeBase;
				CellPosition position = context.DereferenceToCellPosition(range);
				if (position.IsValid)
					ranges.Add(new CellRange(range.Worksheet, position, position));
			}
		}
		public override IParsedThing Clone() {
			ParsedThingName clone = new ParsedThingName();
			clone.DataType = DataType;
			clone.DefinedName = DefinedName;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingName anotherPtg = (ParsedThingName)obj;
			return StringExtensions.CompareInvariantCultureIgnoreCase(this.definedName, anotherPtg.definedName) == 0;
		}
	}
	public class ParsedThingNameX : ParsedThingName, IParsedThingWithSheetDefinition {
		#region Fields
		int sheetDefinitionIndex;
		#endregion
		public ParsedThingNameX() {
		}
		public ParsedThingNameX(string definedName, int sheetDefinitionIndex)
			: base(definedName) {
			this.sheetDefinitionIndex = sheetDefinitionIndex;
		}
		public ParsedThingNameX(string definedName, int sheetDefinitionIndex, OperandDataType dataType)
			: base(definedName, dataType) {
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
			if (sheetDefinition == null)
				builder.Append(CellErrorFactory.GetErrorName(ReferenceError.Instance, context));
			else {
				sheetDefinition.BuildExpressionString(builder, context);
				if (WorkbookDataContext.IsIdent(DefinedName))
					builder.Append(DefinedName);
				else
					builder.AppendFormat("'{0}'", DefinedName);
			}
		}
		public override VariantValue EvaluateToValue(WorkbookDataContext context) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition == null)
				return VariantValue.ErrorReference;
			VariantValue result;
			if (sheetDefinition.Is3DReference)
				result = VariantValue.ErrorName;
			else {
				IModelWorkbook wb = sheetDefinition.GetWorkbook(context);
				External.DdeExternalWorkbook ddeExternalBook = wb as External.DdeExternalWorkbook;
				if (ddeExternalBook != null)
					result = EvaluateDde(ddeExternalBook, context);
				else {
					sheetDefinition.PushSettings(context);
					try {
						result = EvaluateCore(PreEvaluate(context), context);
					}
					finally {
						context.PopCurrentContextData();
					}
				}
			}
			return result;
		}
		VariantValue EvaluateDde(External.DdeExternalWorkbook ddeBook, WorkbookDataContext context) {
			External.ExternalWorksheet sheet = ddeBook.Sheets[DefinedName];
			if (sheet == null)
				return VariantValue.ErrorReference;
			External.DdeExternalWorksheet ddeSheet = (External.DdeExternalWorksheet)sheet;
			VariantValue result = new VariantValue();
			result.CellRangeValue = new CellRange(ddeSheet,
													new CellPosition(0, 0, PositionType.Absolute, PositionType.Absolute),
													new CellPosition(ddeSheet.ColumnCount, ddeSheet.RowCount, PositionType.Absolute, PositionType.Absolute));
			context.PushCurrentCell(0, 0);
			try {
				return EvaluateCore(result, context);
			}
			finally {
				context.PopCurrentCell();
			}
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition.Is3DReference)
				stack.Push(ParsedThingBase.EmptyCellRangeList);
			else {
				sheetDefinition.PushSettings(context);
				try {
					base.GetInvolvedCellRanges(stack, context);
				}
				finally {
					context.PopCurrentContextData();
				}
			}
		}
		public override IParsedThing Clone() {
			ParsedThingNameX clone = new ParsedThingNameX();
			clone.DataType = DataType;
			clone.DefinedName = DefinedName;
			clone.sheetDefinitionIndex = sheetDefinitionIndex;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingNameX anotherPtg = (ParsedThingNameX)obj;
			return this.sheetDefinitionIndex == anotherPtg.SheetDefinitionIndex;
		}
		public override int GetHashCode() {
			CombinedHashCode result = new CombinedHashCode();
			result.AddInt(sheetDefinitionIndex);
			result.AddObject(DefinedName);
			return result.CombinedHash32;
		}
	}
}
