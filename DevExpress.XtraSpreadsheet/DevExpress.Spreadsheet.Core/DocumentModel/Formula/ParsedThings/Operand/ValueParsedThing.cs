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
using System.Diagnostics;
using System.Text;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region Value tokens
	public abstract class ValueParsedThing : ParsedThingBase {
		#region GetInvolvedCellRanges
		public override void GetInvolvedCellRanges(Stack<CellRangeList> stack, WorkbookDataContext context) {
			stack.Push(ParsedThingBase.EmptyCellRangeList);
		}
		#endregion
		public abstract VariantValue GetValue();
		internal static ValueParsedThing CreateInstance(VariantValue value) {
			System.Diagnostics.Debug.Assert(value.Type != VariantValueType.SharedString);
			return CreateInstance(value, null);
		}
		internal static ValueParsedThing CreateInstance(VariantValue value, WorkbookDataContext context) {
			switch (value.Type) {
				case VariantValueType.None:
				case VariantValueType.Missing:
					return ParsedThingMissingArg.Instance;
				case VariantValueType.InlineText:
				case VariantValueType.SharedString:
					return new ParsedThingStringValue(value.GetTextValue(context.StringTable));
				case VariantValueType.Numeric:
					if (value.CanBeStoredAsUInt16())
						return new ParsedThingInteger() { Value = (int)value.NumericValue };
					else
						return new ParsedThingNumeric() { Value = value.NumericValue };
				case VariantValueType.Boolean:
					return new ParsedThingBoolean(value.BooleanValue);
				case VariantValueType.Error:
					return new ParsedThingError() { Value = value };
				default:
				case VariantValueType.Array:
				case VariantValueType.CellRange:
					throw new System.ArgumentException("Invalid value data");
			}
		}
		public override void BuildExpressionString(Stack<int> stack, StringBuilder builder, StringBuilder spacesBuilder, WorkbookDataContext context) {
			stack.Push(builder.Length);
			builder.Append(spacesBuilder.ToString());
			spacesBuilder.Remove(0, spacesBuilder.Length);
			BuildExpressionStringCore(builder, context);
		}
		internal virtual void BuildExpressionStringCore(StringBuilder builder, WorkbookDataContext context) {
		}
		public override string ToString() {
			return base.ToString() + ", Value:" + GetValue().ToString();
		}
		public override bool IsEqual(IParsedThing obj) {
			if (!base.IsEqual(obj))
				return false;
			ValueParsedThing anotherPtg = (ValueParsedThing)obj;
			return this.GetValue().Equals(anotherPtg.GetValue());
		}
	}
	public class ParsedThingMissingArg : ValueParsedThing {
		#region Fields
		static ParsedThingMissingArg instance = new ParsedThingMissingArg();
		#endregion
		ParsedThingMissingArg() {
		}
		#region Properties
		public static ParsedThingMissingArg Instance {
			get {
				return instance;
			}
		}
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(VariantValue.Missing);
		}
		public override IParsedThing Clone() {
			return this;
		}
		public override VariantValue GetValue() {
			return VariantValue.Missing;
		}
	}
	public class ParsedThingStringValue : ValueParsedThing {
		#region Fields
		string value;
		#endregion
		public ParsedThingStringValue() {
		}
		public ParsedThingStringValue(string value) {
			this.value = value;
		}
		#region Properties
		public string Value {
			get { return value; }
			set { this.value = value; }
		}
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		internal override void BuildExpressionStringCore(StringBuilder builder, WorkbookDataContext context) {
			builder.Append("\"");
			builder.Append(value.Replace("\"", "\"\""));
			builder.Append("\"");
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(value);
		}
		public override IParsedThing Clone() {
			return new ParsedThingStringValue(Value);
		}
		public override VariantValue GetValue() {
			return value;
		}
	}
	public class ParsedThingError : ValueParsedThing {
		#region Fields
		ICellError error;
		#endregion
		public ParsedThingError() {
		}
		public ParsedThingError(ICellError error) {
			this.error = error;
		}
		#region Properties
		public ICellError Error { get { return error; } set { error = value; } }
		#endregion
		public VariantValue Value { get { return error.Value; } set { this.error = value.ErrorValue; } }
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		internal override void BuildExpressionStringCore(StringBuilder builder, WorkbookDataContext context) {
			builder.Append(CellErrorFactory.GetErrorName(error, context));
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(Value);
		}
		public override IParsedThing Clone() {
			ParsedThingError clone = new ParsedThingError();
			clone.Error = Error;
			return clone;
		}
		public override VariantValue GetValue() {
			return Value;
		}
	}
	public class ParsedThingBoolean : ValueParsedThing {
		#region Fields
		bool value;
		#endregion
		public ParsedThingBoolean() {
		}
		public ParsedThingBoolean(bool value) {
			this.value = value;
		}
		#region Properties
		public bool Value { get { return value; } set { this.value = value; } }
		#endregion
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		internal override void BuildExpressionStringCore(StringBuilder builder, WorkbookDataContext context) {
			builder.Append(value ? VariantValue.TrueConstant : VariantValue.FalseConstant);
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(value);
		}
		public override IParsedThing Clone() {
			ParsedThingBoolean clone = new ParsedThingBoolean(Value);
			return clone;
		}
		public override VariantValue GetValue() {
			return value;
		}
	}
	public class ParsedThingInteger : ValueParsedThing {
		UInt16 value;
		public ParsedThingInteger() {
		}
		public ParsedThingInteger(int value) {
			this.value = (ushort)value;
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		public int Value {
			get { return this.value; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue);
				this.value = (ushort)value;
			}
		}
		internal override void BuildExpressionStringCore(StringBuilder builder, WorkbookDataContext context) {
			builder.Append(value.ToString(context.Culture));
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(value);
		}
		public override IParsedThing Clone() {
			ParsedThingInteger clone = new ParsedThingInteger();
			clone.Value = Value;
			return clone;
		}
		public override VariantValue GetValue() {
			return value;
		}
	}
	public class ParsedThingNumeric : ValueParsedThing {
		double value;
		public ParsedThingNumeric() {
		}
		public ParsedThingNumeric(double value) {
			this.value = value;
		}
		public double Value {
			get { return this.value; }
			set {
				DevExpress.XtraExport.Xls.XNumChecker.CheckValue(value);
				this.value = value;
			}
		}
		public override void Visit(IParsedThingVisitor visitor) {
			visitor.Visit(this);
		}
		internal override void BuildExpressionStringCore(StringBuilder builder, WorkbookDataContext context) {
			builder.Append(context.ConvertNumberToText(value));
		}
		public override void Evaluate(Stack<VariantValue> stack, WorkbookDataContext context) {
			stack.Push(value);
		}
		public override IParsedThing Clone() {
			ParsedThingNumeric clone = new ParsedThingNumeric();
			clone.Value = Value;
			return clone;
		}
		public override VariantValue GetValue() {
			return value;
		}
	}
	#endregion
}
