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
namespace DevExpress.Spreadsheet.Formulas {
	#region IExpressionVisitor
	public interface IExpressionVisitor {
		#region Unary
		void Visit(UnaryMinusExpression expression);
		void Visit(UnaryPlusExpression expression);
		void Visit(PercentExpression expression);
		#endregion
		#region Binary
		void Visit(AdditionExpression expression);
		void Visit(SubtractionExpression expression);
		void Visit(MultiplicationExpression expression);
		void Visit(DivisionExpression expression);
		void Visit(PowerExpression expression);
		void Visit(EqualityExpression expression);
		void Visit(InequalityExpression expression);
		void Visit(GreaterExpression expression);
		void Visit(GreaterOrEqualExpression expression);
		void Visit(LessExpression expression);
		void Visit(LessOrEqualExpression expression);
		void Visit(ConcatenateExpression expression);
		void Visit(RangeExpression expression);
		void Visit(RangeUnionExpression expression);
		void Visit(RangeIntersectionExpression expression);
		#endregion
		#region Operand
		void Visit(MissingArgumentExpression expression);
		void Visit(ConstantExpression expression);
		void Visit(FunctionExpression expression);
		void Visit(UnknownFunctionExpression expression);
		void Visit(FunctionExternalExpression expression);
		void Visit(CellReferenceExpression expression);
		void Visit(CellErrorReferenceExpression expression);
		void Visit(DefinedNameReferenceExpression expression);
		void Visit(TableReferenceExpression expression);
		void Visit(ArrayConstantExpression expression);
		#endregion
	}
	#endregion
	#region ExpressionVisitor
	public abstract class ExpressionVisitor : IExpressionVisitor {
		#region IExpressionVisitor Members
		#region Unary
		public virtual void VisitUnary(UnaryOperatorExpression expression) {
			expression.InnerExpression.Visit(this);
		}
		public virtual void Visit(UnaryMinusExpression expression) {
			VisitUnary(expression);
		}
		public virtual void Visit(UnaryPlusExpression expression) {
			VisitUnary(expression);
		}
		public virtual void Visit(PercentExpression expression) {
			VisitUnary(expression);
		}
		#endregion
		#region Binary
		public virtual void VisitBinary(BinaryOperatorExpression expression) {
			expression.LeftExpression.Visit(this);
			expression.RightExpression.Visit(this);
		}
		public virtual void Visit(AdditionExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(SubtractionExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(MultiplicationExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(DivisionExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(PowerExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(EqualityExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(InequalityExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(GreaterExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(GreaterOrEqualExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(LessExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(LessOrEqualExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(ConcatenateExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(RangeExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(RangeUnionExpression expression) {
			VisitBinary(expression);
		}
		public virtual void Visit(RangeIntersectionExpression expression) {
			VisitBinary(expression);
		}
		#endregion
		public virtual void VisitFunction(FunctionExpressionBase expression) {
			foreach (IExpression innerExpression in expression.InnerExpressions)
				innerExpression.Visit(this);
		}
		public virtual void Visit(FunctionExpression expression) {
			VisitFunction(expression);
		}
		public virtual void Visit(UnknownFunctionExpression expression) {
			VisitFunction(expression);
		}
		public virtual void Visit(FunctionExternalExpression expression) {
			VisitFunction(expression);
		}
		public virtual void Visit(MissingArgumentExpression expression) { }
		public virtual void Visit(ConstantExpression expression) { }
		public virtual void Visit(CellReferenceExpression expression) { }
		public virtual void Visit(CellErrorReferenceExpression expression) { }
		public virtual void Visit(DefinedNameReferenceExpression expression) { }
		public virtual void Visit(TableReferenceExpression expression) { }
		public virtual void Visit(ArrayConstantExpression expression) { }
		#endregion
	}
 	#endregion
	public class ObtainRangesExpressionVisitor : ExpressionVisitor {
		List<Range> collector;
		IWorkbook workbook;
		Model.WorkbookDataContext workbookContext;
		IExpressionContext context;
		public List<Range> GetRanges(IExpression expression, IExpressionContext context){
			this.context = context;
			collector = new List<Range>();
			workbook = context.Sheet.Workbook;
			workbookContext = workbook.Model.DocumentModel.DataContext;
			NativeFormulaEngine.PushSettingsToModelContext(workbook, context, workbookContext);
			try {
				expression.Visit(this);
			}
			finally {
				NativeFormulaEngine.PopSettingsFromModelContext(workbookContext);
			}
			return collector;
		}
		public override void Visit(CellReferenceExpression expression) {
			base.Visit(expression);
			Model.CellRange modelRange = expression.CellArea.ModelRange.Clone();
			SheetReference sheetReference = expression.SheetReference;
			if (sheetReference != null) {
				Model.SheetDefinition sheetDefinition = sheetReference.ToSheetDefinition();
				sheetDefinition.AssignSheetDefinition(modelRange, workbookContext);
			}
			else
				modelRange.Worksheet = workbookContext.CurrentWorksheet;
			Range range = ConvertModelRangeToRange(modelRange, workbook);
			if (range != null)
				collector.Add(range);
		}
		Range ConvertModelRangeToRange(Model.CellRange modelRange, IWorkbook workbook){
			if (modelRange.Worksheet as Model.Worksheet == null)
				return null;
			NativeWorksheet nativeSheet = workbook.Worksheets[modelRange.Worksheet.Name] as NativeWorksheet;
			if (nativeSheet == null)
				return null;
			return new NativeRange(modelRange, nativeSheet); 
		}
		public override void Visit(DefinedNameReferenceExpression expression) {
			base.Visit(expression);
			Model.ParsedThingName nameThing = expression.ToModelThing();
			Model.DefinedNameBase definedName = nameThing.GetDefinedName(workbookContext);
			if(definedName == null)
				return;
			ParsedExpression innerExpression = null;
			workbookContext.PushDefinedNameProcessing(definedName);
			try {
				innerExpression = ParsedExpression.FromModelExporession(definedName.Expression, workbook);
				if (innerExpression == null)
					return;
				collector.AddRange(innerExpression.GetRanges(context));
			}
			finally {
				workbookContext.PopDefinedNameProcessing();
			}
		}
		public override void Visit(TableReferenceExpression expression) {
			base.Visit(expression);
			if (expression.SheetReference != null)
				return;
			Model.ParsedThingTable tableThing = expression.ToModelThing();
			Model.VariantValue value = tableThing.PreEvaluate(workbookContext);
			if (!value.IsCellRange)
				return;
			Model.CellRange modelRange = value.CellRangeValue as Model.CellRange;
			if (modelRange == null)
				return;
			Range range = ConvertModelRangeToRange(modelRange, workbook);
			if (range != null)
				collector.Add(range);
		}
	}
}
