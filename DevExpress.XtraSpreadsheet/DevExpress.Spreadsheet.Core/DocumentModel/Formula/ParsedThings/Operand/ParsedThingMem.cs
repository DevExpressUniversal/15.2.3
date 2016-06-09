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
using DevExpress.XtraSpreadsheet.Import.Xls;
namespace DevExpress.XtraSpreadsheet.Model {
	public abstract class ParsedThingMemBase : ParsedThingWithDataType {
		#region Fields
		int innerThingCount;
		#endregion
		#region Properties
		public int InnerThingCount { get { return innerThingCount; } set { innerThingCount = value; } }
		#endregion
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			throw new ArgumentException();
		}
		public VariantValue EvaluateInner(ParsedExpression innerExpression, WorkbookDataContext context) {
			VariantValue result = innerExpression.Evaluate(context);
			return ConvertResultToDataType(result, context);
		}
		public VariantValue ConvertResultToDataType(VariantValue result, WorkbookDataContext context) {
			if (DataType == OperandDataType.Value) {
				if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
					result = context.DereferenceValue(result, false);
			}
			else if (DataType == OperandDataType.Array) {
				if (result.IsCellRange) {
					CellRangeBase cellRange = result.CellRangeValue;
					if (cellRange.RangeType == CellRangeType.UnionRange)
						result = VariantValue.ErrorInvalidValueInFunction;
					else {
						RangeVariantArray array = new RangeVariantArray(cellRange.GetFirstInnerCellRange());
						result = VariantValue.FromArray(array);
					}
				}
			}
			return result;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingMemBase anotherPtg = (ParsedThingMemBase)obj;
			return this.innerThingCount == anotherPtg.innerThingCount;
		}
		internal CellRangeList ConvertInvolvedRangesToDataType(CellRangeList cellRangeList, WorkbookDataContext context) {
			if (DataType == OperandDataType.Value && !context.ArrayFormulaProcessing) {
				CellRangeList result = new CellRangeList();
				foreach (CellRangeBase range in cellRangeList) {
					CellPosition position = context.DereferenceToCellPosition(range);
					if (position.IsValid)
						result.Add(new CellRange(range.Worksheet, position, position));
				}
				return result;
			}
			return cellRangeList;
		}
	}
	public class ParsedThingMemArea : ParsedThingMemBase {
		List<CellRangeInfo> ranges = new List<CellRangeInfo>();
		public IList<CellRangeInfo> Ranges { get { return ranges; } }
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public CellRangeBase GetCellRange(Worksheet sheet) {
			CellRangeBase result = null;
			int count = ranges.Count;
			if (count > 0) {
				result = XlsCellRangeFactory.CreateCellRange(sheet, ranges[0].First, ranges[0].Last);
				for (int i = 1; i < count; i++) {
					CellRangeBase range = XlsCellRangeFactory.CreateCellRange(sheet, ranges[i].First, ranges[i].Last);
					VariantValue tempValue = result.ConcatinateWith(range);
					result = tempValue.CellRangeValue;
				}
			}
			return result;
		}
		public void SetCellRange(CellRangeBase range) {
			ranges.Clear();
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion unionRange = range as CellUnion;
				foreach (CellRange item in unionRange.InnerCellRanges)
					ranges.Add(XlsCellRangeFactory.CreateCellRangeInfo(item));
			}
			else
				ranges.Add(XlsCellRangeFactory.CreateCellRangeInfo(range));
		}
		public override IParsedThing Clone() {
			ParsedThingMemArea clone = new ParsedThingMemArea();
			clone.DataType = DataType;
			clone.InnerThingCount = InnerThingCount;
			foreach (CellRangeInfo info in ranges)
				clone.Ranges.Add(info.Clone());
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingMemArea anotherPtg = (ParsedThingMemArea)obj;
			int count = ranges.Count;
			if (count != anotherPtg.ranges.Count)
				return false;
			for (int i = 0; i < count; i++) {
				if (!ranges[i].Equals(anotherPtg.ranges[i]))
					return false;
			}
			return true;
		}
	}
	public class ParsedThingMemErr : ParsedThingMemBase {
		byte errorValue;
		public VariantValue Value {
			get { return ErrorConverter.ErrorCodeToValue(this.errorValue); }
			set { this.errorValue = (byte)ErrorConverter.ValueToErrorCode(value); }
		}
		public byte ErrorValue { get { return errorValue; } set { errorValue = value; } }
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingMemErr clone = new ParsedThingMemErr();
			clone.DataType = DataType;
			clone.InnerThingCount = InnerThingCount;
			clone.errorValue = errorValue;
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingMemErr anotherPtg = (ParsedThingMemErr)obj;
			return this.errorValue == anotherPtg.errorValue;
		}
	}
	public class ParsedThingMemNoMem : ParsedThingMemBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingMemNoMem clone = new ParsedThingMemNoMem();
			clone.DataType = DataType;
			clone.InnerThingCount = InnerThingCount;
			return clone;
		}
	}
	public class ParsedThingMemFunc : ParsedThingMemBase {
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override IParsedThing Clone() {
			ParsedThingMemFunc clone = new ParsedThingMemFunc();
			clone.DataType = DataType;
			clone.InnerThingCount = InnerThingCount;
			return clone;
		}
	}
}
