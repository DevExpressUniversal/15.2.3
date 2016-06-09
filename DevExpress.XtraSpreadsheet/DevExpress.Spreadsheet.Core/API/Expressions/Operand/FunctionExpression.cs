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
using System.Globalization;
using System.Text;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Spreadsheet.Functions;
using DevExpress.Utils;
using System.ComponentModel;
using Model = DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.Spreadsheet.Formulas {
	#region FunctionExpressionBase(abstract class)
	public abstract class FunctionExpressionBase : Expression {
		IList<IExpression> innerExpressions;
		protected FunctionExpressionBase(IList<IExpression> innerExpressions) {
			this.innerExpressions = innerExpressions;
		}
		protected FunctionExpressionBase() {
		}
		#region Properties
		public IList<IExpression> InnerExpressions { get { return innerExpressions; } set { innerExpressions = value; } }
		#endregion
		protected abstract string GetFunctionName(Model.WorkbookDataContext context);
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			char separator = context.GetListSeparator();
			result.Append(GetFunctionName(context));
			result.Append('(');
			if (innerExpressions != null) {
				int count = this.innerExpressions.Count;
				for (int i = 0; i < count; i++) {
					if (i > 0)
						result.Append(separator);
					innerExpressions[i].BuildExpressionString(result, workbook);
				}
			}
			result.Append(')');
		}
		protected void CopyFrom(FunctionExpressionBase value) {
			base.CopyFrom(value);
			if (value.innerExpressions != null) {
				innerExpressions = new List<IExpression>();
				foreach (IExpression expression in value.InnerExpressions)
					innerExpressions.Add(expression.Clone());
			}
			else
				innerExpressions = null;
		}
	}
	#endregion
	#region FunctionExpression
	public class FunctionExpression : FunctionExpressionBase, ISupportsCopyFrom<FunctionExpression>, ICloneable<FunctionExpression> {
		IFunction function;
		public FunctionExpression(IFunction function, IList<IExpression> innerExpressions)
			: base(innerExpressions) {
			Guard.ArgumentNotNull(function, "function");
			this.function = function;
		}
		protected FunctionExpression() {
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("FunctionExpressionFunction")]
#endif
		public IFunction Function {
			get { return function; }
			set {
				Guard.ArgumentNotNull(value, "function");
				function = value;
			}
		}
		#endregion
		public override void Visit(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}
		protected override string GetFunctionName(Model.WorkbookDataContext context) {
			string functionName = function.Name;
			return Model.FormulaCalculator.GetFunctionName(functionName, context);
		}
		#region ISupportsCopyFrom<FunctionExpression> Members
		public void CopyFrom(FunctionExpression value) {
			base.CopyFrom(value);
			this.function = value.function;
		}
		#endregion
		#region ICloneable<FunctionExpression> Members
		public FunctionExpression Clone() {
			FunctionExpression result = new FunctionExpression();
			result.CopyFrom(this);
			return result;
		}
		protected override IExpression CloneCore() {
			return Clone();
		}
		#endregion
	}
	#endregion
	#region UnknownFunctionExpression
	public class UnknownFunctionExpression : FunctionExpressionBase, ISupportsCopyFrom<UnknownFunctionExpression>, ICloneable<UnknownFunctionExpression> {
		const string ADDIN_FUNCTION_PREFIX = "_xll.";
		string functionName;
		bool isAddinFunction;
		public UnknownFunctionExpression(string functionName, bool isAddinFunction, IList<IExpression> innerExpressions)
			: base(innerExpressions) {
			Guard.ArgumentIsNotNullOrEmpty(functionName, "function name");
			this.functionName = functionName;
			this.isAddinFunction = isAddinFunction;
		}
		protected UnknownFunctionExpression() {
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("UnknownFunctionExpressionIsAddinFunction")]
#endif
		public bool IsAddinFunction { get { return isAddinFunction; } set { isAddinFunction = value; } }
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("UnknownFunctionExpressionFunctionName")]
#endif
		public string FunctionName {
			get { return functionName; }
			set {
				Guard.ArgumentIsNotNullOrEmpty(value, "function name");
				functionName = value;
			}
		}
		#endregion
		public override void Visit(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}
		protected override string GetFunctionName(Model.WorkbookDataContext context) {
			if (IsAddinFunction)
				return ADDIN_FUNCTION_PREFIX + functionName;
			return FunctionName;
		}
		#region ISupportsCopyFrom<UnknownFunctionExpression> Members
		public void CopyFrom(UnknownFunctionExpression value) {
			base.CopyFrom(value);
			this.functionName = value.functionName;
			this.isAddinFunction = value.isAddinFunction;
		}
		#endregion
		#region ICloneable<UnknownFunctionExpression> Members
		public UnknownFunctionExpression Clone() {
			UnknownFunctionExpression result = new UnknownFunctionExpression();
			result.CopyFrom(this);
			return result;
		}
		protected override IExpression CloneCore() {
			return Clone();
		}
		#endregion
	}
	#endregion
	#region FunctionExternalExpression
	public class FunctionExternalExpression : FunctionExpressionBase, ISupportsCopyFrom<FunctionExternalExpression>, ICloneable<FunctionExternalExpression> {
		#region Fields
		SheetReference sheetReference;
		string functionName;
		#endregion
		public FunctionExternalExpression(string functionName, SheetReference sheetReference)
			: this(functionName, sheetReference, null) {
		}
		public FunctionExternalExpression(string functionName, SheetReference sheetReference, IList<IExpression> innerExpressions)
			: base(innerExpressions) {
			Guard.ArgumentIsNotNullOrEmpty(functionName, "function name");
			Guard.ArgumentNotNull(sheetReference, "sheet definition");
			this.functionName = functionName;
			this.sheetReference = sheetReference;
		}
		protected FunctionExternalExpression() {
		}
		#region Properties
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("FunctionExternalExpressionFunctionName")]
#endif
		public string FunctionName {
			get { return functionName; }
			set {
				Guard.ArgumentIsNotNullOrEmpty(value, "function name");
				functionName = value;
			}
		}
#if !SL
	[DevExpressSpreadsheetCoreLocalizedDescription("FunctionExternalExpressionSheetReference")]
#endif
		public SheetReference SheetReference {
			get { return sheetReference; }
			set {
				Guard.ArgumentNotNull(value, "sheet definition");
				sheetReference = value;
			}
		}
		#endregion
		public override void Visit(IExpressionVisitor visitor) {
			visitor.Visit(this);
		}
		protected override string GetFunctionName(Model.WorkbookDataContext context) {
			return FunctionName;
		}
		protected override void BuildExpressionStringCore(StringBuilder result, IWorkbook workbook, Model.WorkbookDataContext context) {
			sheetReference.BuildString(result, context);
			base.BuildExpressionStringCore(result, workbook, context);
		}
		#region ISupportsCopyFrom<FunctionExternalExpression> Members
		public void CopyFrom(FunctionExternalExpression value) {
			base.CopyFrom(value);
			this.functionName = value.functionName;
			this.sheetReference = value.sheetReference.Clone();
		}
		#endregion
		#region ICloneable<FunctionExternalExpression> Members
		public FunctionExternalExpression Clone() {
			FunctionExternalExpression result = new FunctionExternalExpression();
			result.CopyFrom(this);
			return result;
		}
		protected override IExpression CloneCore() {
			return Clone();
		}
		#endregion
	}
	#endregion
}
