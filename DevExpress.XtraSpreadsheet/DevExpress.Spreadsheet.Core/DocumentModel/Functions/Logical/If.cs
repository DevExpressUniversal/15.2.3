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
using DevExpress.Office;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FunctionIfBase
	public abstract class FunctionIfBase : WorksheetFunctionBase {
		protected int GetArrayWidth(VariantValue value) {
			if (value.IsArray)
				return value.ArrayValue.Width;
			if (value.IsCellRange)
				return value.CellRangeValue.Width;
			return 1;
		}
		protected int GetArrayHeight(VariantValue value) {
			if (value.IsArray)
				return value.ArrayValue.Height;
			if (value.IsCellRange)
				return value.CellRangeValue.Height;
			return 1;
		}
		protected VariantValue PrepareArrayElement(VariantValue element, int y, int x) {
			if (element.IsArray)
				return element.ArrayValue.GetValue(y, x);
			if (element.IsCellRange)
				return element.CellRangeValue.GetCellValueRelative(x, y);
			return element;
		}
	}
	#endregion
	#region FunctionIf
	public class FunctionIf : FunctionIfBase {
		static readonly FunctionParameterCollection functionParameters = PrepareParameters();
		public override string Name { get { return "IF"; } }
		public override int Code { get { return 0x0001; } }
		public override FunctionParameterCollection Parameters { get { return functionParameters; } }
		public override OperandDataType ReturnDataType { get { return OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array; } }
		protected override VariantValue EvaluateCore(IList<VariantValue> arguments, WorkbookDataContext context) {
			VariantValue valueIfFalse = arguments.Count < 3 ? false : arguments[2];
			VariantValue condition = arguments[0];
			if (condition.IsArray)
				return EvaluateArrayValue(condition.ArrayValue, arguments[1], valueIfFalse, context);
			return EvaluateSingleValue(condition.ToBoolean(context), arguments[1], valueIfFalse, context);
		}
		VariantValue EvaluateArrayValue(IVariantArray array, VariantValue valueIfTrue, VariantValue valueIfFalse, WorkbookDataContext context) {
			CalculatedIfFunctionVariantArray calculatedArray = new CalculatedIfFunctionVariantArray(array, valueIfTrue, valueIfFalse, context);
			return VariantValue.FromArray(calculatedArray);
		}
		protected VariantValue EvaluateSingleValue(VariantValue conditionValue, VariantValue valueIfTrue, VariantValue valueIfFalse, WorkbookDataContext context) {
			if (conditionValue.IsError)
				return conditionValue;
			if (conditionValue.BooleanValue)
				return valueIfTrue;
			else
				return valueIfFalse;
		}
		static FunctionParameterCollection PrepareParameters() {
			FunctionParameterCollection collection = new FunctionParameterCollection();
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Array));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array, FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			collection.Add(new FunctionParameter(OperandDataType.Value | OperandDataType.Reference | OperandDataType.Array, FunctionParameterOption.NonRequired | FunctionParameterOption.DoNotDereferenceEmptyValueAsZero));
			return collection;
		}
	}
	#endregion
	#region CalculatedIfFunctionVariantArray
	public class CalculatedIfFunctionVariantArray : IVariantArray {
		readonly IVariantArray array;
		readonly VariantValue valueIfTrue;
		readonly VariantValue valueIfFalse;
		readonly WorkbookDataContext context;
		int width;
		int height;
		public CalculatedIfFunctionVariantArray(IVariantArray array, VariantValue valueIfTrue, VariantValue valueIfFalse, WorkbookDataContext context) {
			Guard.ArgumentNotNull(array, "array");
			Guard.ArgumentNotNull(context, "context");
			this.array = array;
			this.valueIfFalse = valueIfFalse;
			this.valueIfTrue = valueIfTrue;
			this.context = new WorkbookDataContext(context);
			CalculateDimensions();
		}
		void CalculateDimensions() {
			width = array.Width;
			height = array.Height;
			width = Math.Max(width, GetArrayWidth(valueIfTrue));
			height = Math.Max(height, GetArrayHeight(valueIfTrue));
			width = Math.Max(width, GetArrayWidth(valueIfFalse));
			height = Math.Max(height, GetArrayHeight(valueIfFalse));
		}
		#region IVariantArray Members
		public long Count { get { return width * height; } }
		public int Width { get { return width; } }
		public int Height { get { return height; } }
		public VariantValue this[int index] { get { return GetValue(index / Width, index % Width); } }
		public bool IsHorizontal { get { return Height == 1 && Width != 1; } }
		public bool IsVertical { get { return Width == 1 && Height != 1; } }
		#endregion
		protected int GetArrayWidth(VariantValue value) {
			if (value.IsArray)
				return value.ArrayValue.Width;
			if (value.IsCellRange)
				return value.CellRangeValue.Width;
			return 1;
		}
		protected int GetArrayHeight(VariantValue value) {
			if (value.IsArray)
				return value.ArrayValue.Height;
			if (value.IsCellRange)
				return value.CellRangeValue.Height;
			return 1;
		}
		public VariantValue GetValue(int y, int x) {
			if (y < 0)
				return VariantValue.ErrorValueNotAvailable;
			if (y >= Height) {
				if (Height != 1)
					return VariantValue.ErrorValueNotAvailable;
				y = 0;
			}
			if (x < 0)
				return VariantValue.ErrorValueNotAvailable;
			if (x >= Width) {
				if (Width != 1)
					return VariantValue.ErrorValueNotAvailable;
				x = 0;
			}
			VariantValue conditionValue = array.GetValue(y, x).ToBoolean(context);
			if (conditionValue.IsError)
				return conditionValue;
			else if (conditionValue.BooleanValue)
				return PrepareArrayElement(valueIfTrue, y, x);
			else
				return PrepareArrayElement(valueIfFalse, y, x);
		}
		VariantValue PrepareArrayElement(VariantValue element, int y, int x) {
			if (element.IsArray)
				return element.ArrayValue.GetValue(y, x);
			if (element.IsCellRange)
				return element.CellRangeValue.GetCellValueRelative(x, y);
			return element;
		}
		IVariantArray ICloneable<IVariantArray>.Clone() {
			return new CalculatedIfFunctionVariantArray(array.Clone(), valueIfTrue, valueIfFalse, context);
		}
	}
	#endregion
}
