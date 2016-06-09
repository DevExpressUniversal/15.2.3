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
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	public class ParsedThingArray : ParsedThingWithDataType {
		#region Fields
		IVariantArray array = VariantArray.Create(0, 0);
		public ParsedThingArray() {
		}
		public ParsedThingArray(IVariantArray array) {
			this.array = array;
		}
		#endregion
		#region Properties
		public IVariantArray ArrayValue { get { return array; } set { array = value; } }
		public int Width { get { return array.Width; }  }
		public int Height { get { return array.Height; }  }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			BuildExpressionStringCore(builder, context);
		}
		internal void BuildExpressionStringCore(StringBuilder builder, WorkbookDataContext context) {
			char decimalSeparator = context.GetDecimalSymbol();
			char elementDelimiter = (decimalSeparator == ',') ? '\\' : ',';
			char rowsDelimiter = (decimalSeparator == ';') ? '\\' : ';';
			builder.Append('{');
			int index = 0;
			int width = array.Width;
			int height = array.Height;
			for (int h = 0; h < height; h++) {
				for (int w = 0; w < width; w++) {
					VariantValue value = array[index];
					string element = value.IsError ? CellErrorFactory.GetErrorName(value.ErrorValue, context) : value.ToText(context).GetTextValue(context.StringTable);
					if (array[index].IsText)
						element = "\"" + element.Replace("\"", "\"\"") + "\"";
					builder.Append(element);
					if (w != width - 1)
						builder.Append(elementDelimiter);
					index++;
				}
				if (h != height - 1)
					builder.Append(rowsDelimiter);
			}
			builder.Append('}');
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			VariantValue result = VariantValue.FromArray(array);
			if (DataType == OperandDataType.Value) {
				if (context.Workbook.CalculationChain.Enabled || !context.ArrayFormulaProcessing)
					result = context.DereferenceValue(result, false);
			}
			stack.Push(result);
		}
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			stack.Push(ParsedThingBase.EmptyCellRangeList);
		}
		public override IParsedThing Clone() {
			ParsedThingArray clone = new ParsedThingArray();
			clone.DataType = DataType;
			clone.array = array.Clone();
			return clone;
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ParsedThingArray anotherPtg = (ParsedThingArray)obj;
			return this.array.Equals(anotherPtg.array);
		}
	}
}
