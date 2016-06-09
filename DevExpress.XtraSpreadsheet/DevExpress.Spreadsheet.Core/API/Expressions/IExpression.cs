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
using System.Diagnostics;
using System.Globalization;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet.Functions;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.API.Native.Implementation;
using Model = DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.Spreadsheet.Formulas {
	#region IExpression
	public interface IExpression : ICloneable<IExpression> {
		void Visit(IExpressionVisitor visitor);
		void BuildExpressionString(StringBuilder builder, IWorkbook workbook);
		void BuildExpressionString(StringBuilder builder, IWorkbook workbook, IExpressionContext context);
		int BracketsCount { get; set; }
	}
	#endregion
	#region Expression (abstract class)
	public abstract class Expression : IExpression {
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ExpressionBracketsCount")]
#endif
		public int BracketsCount { get; set; }
		#endregion
		public abstract void Visit(IExpressionVisitor visitor);
		#region BuildExpressionString
		public void BuildExpressionString(StringBuilder builder, IWorkbook workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			Model.WorkbookDataContext workbookContext = workbook.Model.DocumentModel.DataContext;
			AppendBrackets(builder, '(');
			BuildExpressionStringCore(builder, workbook, workbookContext);
			AppendBrackets(builder, ')');
		}
		public void BuildExpressionString(StringBuilder builder, IWorkbook workbook, IExpressionContext context) {
			if (context == null)
				BuildExpressionString(builder, workbook);
			Guard.ArgumentNotNull(workbook, "workbook");
			Model.WorkbookDataContext workbookContext = workbook.Model.DocumentModel.DataContext;
			NativeFormulaEngine.PushSettingsToModelContext(workbook, context, workbookContext);
			AppendBrackets(builder, '(');
			try {
				BuildExpressionStringCore(builder, workbook, workbookContext);
			}
			finally {
				NativeFormulaEngine.PopSettingsFromModelContext(workbookContext);
			}
			AppendBrackets(builder, ')');
		}
		protected abstract void BuildExpressionStringCore(StringBuilder builder, IWorkbook workbook, Model.WorkbookDataContext context);
		void AppendBrackets(StringBuilder result, char ch) {
			result.Append(new String(ch, BracketsCount));
		}
		#endregion
		protected void CopyFrom(Expression value) {
			Guard.ArgumentNotNull(value, "value");
			BracketsCount = value.BracketsCount;
		}
		#region ICloneable<IExpression> Members
		IExpression ICloneable<IExpression>.Clone() {
			return CloneCore();
		}
		protected abstract IExpression CloneCore();
		#endregion
	}
	#endregion
	#region IParsedExpression
	public interface IParsedExpression {
		string ToString(IExpressionContext context);
		IExpression Expression { get; set; }
		List<Range> GetRanges();
		List<Range> GetRanges(IExpressionContext context);
	}
	#endregion
	#region ParsedExpression
	public class ParsedExpression : IParsedExpression {
		#region Fields
		readonly IWorkbook workbook;
		IExpression expression;
		#endregion
		public ParsedExpression(IWorkbook workbook) {
			Guard.ArgumentNotNull(workbook, "workbook");
			this.workbook = workbook;
		}
		NativeWorkbook NativeWorkbook { get { return (NativeWorkbook)workbook; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("ParsedExpressionExpression")]
#endif
		public IExpression Expression { get { return expression; } set { expression = value; } }
		#region IParsedExpression Members
		public string ToString(IExpressionContext context) {
			if (expression == null)
				return String.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			expression.BuildExpressionString(stringBuilder, workbook, context);
			return stringBuilder.ToString();
		}
		public List<Range> GetRanges() {
			return GetRanges(new ActiveSettingsBasedExpressionContext(NativeWorkbook));
		}
		public List<Range> GetRanges(IExpressionContext context) {
			ObtainRangesExpressionVisitor visitor = new ObtainRangesExpressionVisitor();
			return visitor.GetRanges(expression, context);
		}
		#endregion
		public override string ToString() {
			return ToString(new ActiveSettingsBasedExpressionContext(NativeWorkbook));
		}
		internal static ParsedExpression FromModelExporession(Model.ParsedExpression modelExpression, IWorkbook workbook) {
			if (modelExpression == null)
				return null;
			DevExpress.XtraSpreadsheet.Model.WorkbookDataContext dataContext = workbook.Model.DocumentModel.DataContext;
			ParsedThingConverter converter = new ParsedThingConverter(workbook, dataContext);
			Expression expression = converter.ConvertToExpression(modelExpression);
			ParsedExpression result = new ParsedExpression(workbook);
			result.Expression = expression;
			return result;
		}
	}
	#endregion
	public class CellCalculationArgs {
		public CellCalculationArgs(Spreadsheet.CellKey cellKey, Spreadsheet.CellValue value) {
			this.CellKey = cellKey;
			this.Value = value;
		}
		public int Column { get { return CellKey.ColumnIndex; } }
		public int Row { get { return CellKey.RowIndex; } }
		public int SheetId { get { return CellKey.SheetId; } }
		public Spreadsheet.CellKey CellKey { get; private set; }
		public bool Handled { get; set; }
		public Spreadsheet.CellValue Value { get; set; }
	}
}
#region WorkbookFormulas implementation
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	using DevExpress.Spreadsheet.Formulas;
	using DevExpress.XtraSpreadsheet;
	using DevExpress.Spreadsheet;
	using DevExpress.Spreadsheet.Functions;
	using DevExpress.XtraSpreadsheet.Localization;
	using System.Collections.Generic;
	#region ParsedThingConverter
	partial class ParsedThingConverter : Model.ParsedThingVisitor {
		readonly Model.WorkbookDataContext context;
		readonly IWorkbook workbook;
		Stack<Expression> stack;
		public ParsedThingConverter(IWorkbook workbook, Model.WorkbookDataContext context) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
			this.workbook = workbook;
		}
		public Expression ConvertToExpression(Model.ParsedThingList things) {
			stack = new Stack<Expression>();
			int i = 0;
			while (i < things.Count) {
				Model.IParsedThing thing = things[i];
				thing.Visit(this);
				i++;
			}
			if (stack.Count != 1)
				Exceptions.ThrowInternalException();
			return stack.Pop();
		}
		#region Binary
		void ProcessBinary(BinaryOperatorExpression expression) {
			System.Diagnostics.Debug.Assert(stack.Count > 1);
			expression.RightExpression = stack.Pop();
			expression.LeftExpression = stack.Pop();
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingAdd thing) {
			ProcessBinary(new AdditionExpression());
		}
		public override void Visit(Model.ParsedThingSubtract thing) {
			ProcessBinary(new SubtractionExpression());
		}
		public override void Visit(Model.ParsedThingMultiply thing) {
			ProcessBinary(new MultiplicationExpression());
		}
		public override void Visit(Model.ParsedThingDivide thing) {
			ProcessBinary(new DivisionExpression());
		}
		public override void Visit(Model.ParsedThingPower thing) {
			ProcessBinary(new PowerExpression());
		}
		public override void Visit(Model.ParsedThingConcat thing) {
			ProcessBinary(new ConcatenateExpression());
		}
		public override void Visit(Model.ParsedThingLess thing) {
			ProcessBinary(new LessExpression());
		}
		public override void Visit(Model.ParsedThingLessEqual thing) {
			ProcessBinary(new LessOrEqualExpression());
		}
		public override void Visit(Model.ParsedThingEqual thing) {
			ProcessBinary(new EqualityExpression());
		}
		public override void Visit(Model.ParsedThingGreaterEqual thing) {
			ProcessBinary(new GreaterOrEqualExpression());
		}
		public override void Visit(Model.ParsedThingGreater thing) {
			ProcessBinary(new GreaterExpression());
		}
		public override void Visit(Model.ParsedThingNotEqual thing) {
			ProcessBinary(new InequalityExpression());
		}
		public override void Visit(Model.ParsedThingIntersect thing) {
			ProcessBinary(new RangeIntersectionExpression());
		}
		public override void Visit(Model.ParsedThingUnion thing) {
			ProcessBinary(new RangeUnionExpression());
		}
		public override void Visit(Model.ParsedThingRange thing) {
			ProcessBinary(new RangeExpression());
		}
		#endregion
		#region Unary
		public override void Visit(Model.ParsedThingUnaryPlus thing) {
			System.Diagnostics.Debug.Assert(stack.Count > 0);
			stack.Push(new UnaryPlusExpression(stack.Pop()));
		}
		public override void Visit(Model.ParsedThingUnaryMinus thing) {
			System.Diagnostics.Debug.Assert(stack.Count > 0);
			stack.Push(new UnaryMinusExpression(stack.Pop()));
		}
		public override void Visit(Model.ParsedThingPercent thing) {
			System.Diagnostics.Debug.Assert(stack.Count > 0);
			stack.Push(new PercentExpression(stack.Pop()));
		}
		#endregion
		#region Attributes
		public override void Visit(Model.ParsedThingParentheses thing) {
			System.Diagnostics.Debug.Assert(stack.Count > 0);
			Expression expression = stack.Peek();
			expression.BracketsCount++;
		}
		#endregion
		#region Operand
		public override void Visit(Model.ParsedThingMissingArg thing) {
			stack.Push(MissingArgumentExpression.Instance);
		}
		void ProcessValueThing(Model.ValueParsedThing thing) {
			Model.VariantValue modelValue = thing.GetValue();
			CellValue cellValue = new CellValue(modelValue, context);
			stack.Push(new ConstantExpression(cellValue));
		}
		public override void Visit(Model.ParsedThingStringValue thing) {
			ProcessValueThing(thing);
		}
		public override void Visit(Model.ParsedThingError thing) {
			ProcessValueThing(thing);
		}
		public override void Visit(Model.ParsedThingNumeric thing) {
			ProcessValueThing(thing);
		}
		public override void Visit(Model.ParsedThingInteger thing) {
			ProcessValueThing(thing);
		}
		public override void Visit(Model.ParsedThingBoolean thing) {
			ProcessValueThing(thing);
		}
		public override void Visit(Model.ParsedThingVariantValue thing) {
			ProcessValueThing(thing);
		}
		SheetReference PrepareSheetReference(int sheetDefinitionIndex) {
			Model.SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			return new SheetReference(workbook, sheetDefinition);
		}
		public override void Visit(Model.ParsedThingArray thing) {
			ArrayConstantExpression expression = new ArrayConstantExpression(thing.ArrayValue);
			stack.Push(expression);
		}
		#region Ref
		public override void Visit(Model.ParsedThingRef thing) {
			Model.CellPosition position = thing.Position;
			CellReferenceExpression expression = new CellReferenceExpression(new CellArea(position, position));
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingRef3d thing) {
			Model.CellPosition position = thing.Position;
			SheetReference sheetReference = PrepareSheetReference(thing.SheetDefinitionIndex);
			CellReferenceExpression expression = new CellReferenceExpression(new CellArea(position, position), sheetReference);
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingRefRel thing) {
			Model.CellPosition position = thing.Location.ToCellPositionWithoutCorrection(context);
			CellReferenceExpression expression = new CellReferenceExpression(new CellArea(position, position));
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingRef3dRel thing) {
			Model.CellPosition position = thing.Location.ToCellPositionWithoutCorrection(context);
			SheetReference sheetReference = PrepareSheetReference(thing.SheetDefinitionIndex);
			CellReferenceExpression expression = new CellReferenceExpression(new CellArea(position, position), sheetReference);
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingRefErr thing) {
			stack.Push(new CellErrorReferenceExpression());
		}
		public override void Visit(Model.ParsedThingErr3d thing) {
			SheetReference sheetReference = PrepareSheetReference(thing.SheetDefinitionIndex);
			stack.Push(new CellErrorReferenceExpression(sheetReference));
		}
		#endregion
		#region Area
		public override void Visit(Model.ParsedThingArea thing) {
			CellReferenceExpression expression = new CellReferenceExpression(new CellArea(thing.TopLeft, thing.BottomRight));
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingArea3d thing) {
			SheetReference sheetReference = PrepareSheetReference(thing.SheetDefinitionIndex);
			CellReferenceExpression expression = new CellReferenceExpression(new CellArea(thing.TopLeft, thing.BottomRight), sheetReference);
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingAreaN thing) {
			Model.CellPosition topLeft = thing.First.ToCellPositionWithoutCorrection(context);
			Model.CellPosition bottomRight = thing.Last.ToCellPositionWithoutCorrection(context);
			CellArea cellArea = new CellArea(topLeft, bottomRight);
			cellArea.Normalize();
			CellReferenceExpression expression = new CellReferenceExpression(cellArea);
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingArea3dRel thing) {
			Model.CellPosition topLeft = thing.First.ToCellPositionWithoutCorrection(context);
			Model.CellPosition bottomRight = thing.Last.ToCellPositionWithoutCorrection(context);
			SheetReference sheetReference = PrepareSheetReference(thing.SheetDefinitionIndex);
			CellArea cellArea = new CellArea(topLeft, bottomRight);
			cellArea.Normalize();
			CellReferenceExpression expression = new CellReferenceExpression(cellArea, sheetReference);
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingAreaErr thing) {
			stack.Push(new CellErrorReferenceExpression());
		}
		public override void Visit(Model.ParsedThingAreaErr3d thing) {
			SheetReference sheetReference = PrepareSheetReference(thing.SheetDefinitionIndex);
			stack.Push(new CellErrorReferenceExpression(sheetReference));
		}
		#endregion
		#region DefinedName
		public override void Visit(Model.ParsedThingName thing) {
			DefinedNameReferenceExpression expression = new DefinedNameReferenceExpression(thing.DefinedName);
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingNameX thing) {
			SheetReference sheetReference = PrepareSheetReference(thing.SheetDefinitionIndex);
			DefinedNameReferenceExpression expression = new DefinedNameReferenceExpression(thing.DefinedName, sheetReference);
			stack.Push(expression);
		}
		#endregion
		#region Function
		void ProcessFunction(IFunction function, int parametersCount) {
			System.Diagnostics.Debug.Assert(stack.Count >= parametersCount);
			List<IExpression> innerExpressions = PrepareExpressionList(parametersCount);
			FunctionExpression functionExpression = new FunctionExpression(function, innerExpressions);
			stack.Push(functionExpression);
		}
		List<IExpression> PrepareExpressionList(int parametersCount) {
			System.Diagnostics.Debug.Assert(stack.Count >= parametersCount);
			List<IExpression> innerExpressions = new List<IExpression>();
			for (int i = 0; i < parametersCount; i++)
				innerExpressions.Insert(0, stack.Pop());
			return innerExpressions;
		}
		public override void Visit(Model.ParsedThingAddinFunc thing) {
			List<IExpression> innerExpressions = PrepareExpressionList(thing.ParamCount);
			UnknownFunctionExpression expression = new UnknownFunctionExpression(thing.Name, true, innerExpressions);
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingCustomFunc thing) {
			ICustomFunction function = workbook.Functions.CustomFunctions[thing.Name];
			if(function == null)
				function = workbook.Functions.GlobalCustomFunctions[thing.Name];
			if (function == null)
				Exceptions.ThrowInternalException();
			ProcessFunction(function, thing.ParamCount);
		}
		public override void Visit(Model.ParsedThingFunc thing) {
			Model.ISpreadsheetFunction modelFunction = thing.Function;
			System.Diagnostics.Debug.Assert(modelFunction.HasFixedParametersCount);
			IFunction function = workbook.Functions[modelFunction.Name];
			ProcessFunction(function, modelFunction.Parameters.Count);
		}
		public override void Visit(Model.ParsedThingFuncVar thing) {
			IFunction function = workbook.Functions[thing.Function.Name];
			ProcessFunction(function, thing.ParamCount);
		}
		public override void Visit(Model.ParsedThingUnknownFunc thing) {
			List<IExpression> innerExpressions = PrepareExpressionList(thing.ParamCount);
			UnknownFunctionExpression expression = new UnknownFunctionExpression(thing.Name, false, innerExpressions);
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingUnknownFuncExt thing) {
			List<IExpression> innerExpressions = PrepareExpressionList(thing.ParamCount);
			SheetReference sheetReference = PrepareSheetReference(thing.SheetDefinitionIndex);
			FunctionExternalExpression expression = new FunctionExternalExpression(thing.Name, sheetReference, innerExpressions);
			stack.Push(expression);
		}
		#endregion
		#region Table
		void ProcessTableReference(TableReferenceExpression expression, Model.ParsedThingTable thing) {
			expression.RowsRestriction = (TableRowsRestriction)thing.IncludedRows;
			expression.ColumnStart = thing.ColumnStart;
			expression.ColumnEnd = thing.ColumnEnd;
			stack.Push(expression);
		}
		public override void Visit(Model.ParsedThingTable thing) {
			TableReferenceExpression expression = new TableReferenceExpression(thing.TableName);
			ProcessTableReference(expression, thing);
		}
		public override void Visit(Model.ParsedThingTableExt thing) {
			SheetReference sheetReference = PrepareSheetReference(thing.SheetDefinitionIndex);
			TableReferenceExpression expression = new TableReferenceExpression(thing.TableName, sheetReference);
			ProcessTableReference(expression, thing);
		}
		#endregion
		#endregion
	}
	#endregion
}
#endregion
