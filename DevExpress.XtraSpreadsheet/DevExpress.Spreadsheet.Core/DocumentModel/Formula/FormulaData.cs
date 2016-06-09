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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region FormulaData
	public class FormulaData : ISupportsCopyFrom<FormulaData>, ICloneable<FormulaData> {
		#region Fields
		readonly DocumentModel documentModel;
		readonly IExpressionFilter filter;
		ISupportsInvalidate parent;
		string tempBody;
		ParsedExpression expression;
		VariantValue cachedValue;
		#endregion
		public FormulaData(DocumentModel documentModel, IExpressionFilter filter) {
			this.documentModel = documentModel;
			this.filter = filter;
			this.parent = null;
			cachedValue = VariantValue.Empty;
		}
		public FormulaData(DocumentModel documentModel)
			: this(documentModel, EmptyExpressionFilter.Instance) {
		}
		#region Properties
		protected internal ISupportsInvalidate Parent { get { return parent; } set { parent = value; } }
		public WorkbookDataContext Context { get { return documentModel.DataContext; } }
		public string TempBody { get { return tempBody; } }
		public string FormulaBody {
			get { return GetFormulaBody(); }
			set {
				if (value == null)
					value = string.Empty;
				SetFormulaBody(value);
			}
		}
		public VariantValue CachedValue {
			get {
				if (cachedValue.IsEmpty)
					cachedValue = PrepareCachedValue();
				return cachedValue;
			}
			set { cachedValue = value; }
		}
		public ParsedExpression Expression {
			get {
				PrepareExpression();
				return expression;
			}
			set {
				SetExpression(value);
			}
		}
		protected internal VariantValue CurrentValue {
			get {
				return PrepareCachedValue();
			}
		}
		#endregion
		#region Expression
		void SetExpression(ParsedExpression value) {
			if (expression == null)
				SetExpressionCore(value);
			else {
				FormulaDataExpressionPropertyChangedHistoryItem historyItem = new FormulaDataExpressionPropertyChangedHistoryItem(documentModel, this, expression, value);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void InvalidateCachedValue() {
			cachedValue = VariantValue.Empty;
		}
		protected internal void SetExpressionCore(ParsedExpression value) {
			expression = value;
			InvalidateCachedValue();
			if (parent != null)
				parent.Invalidate();
		}
		protected internal void PrepareExpression() {
			if (expression == null && !string.IsNullOrEmpty(tempBody))
				ParseTemporarilySavedBody();
		}
		protected internal ParsedExpression ParseExpression(string formula) {
			WorkbookDataContext context = documentModel.DataContext;
			context.PushCurrentCell(0, 0);
			try {
				return context.ParseExpression(formula, OperandDataType.None, false);
			}
			finally {
				context.PopCurrentCell();
			}
		}
		protected VariantValue PrepareCachedValue() {
			PrepareExpression();
			if (expression == null)
				return VariantValue.Empty;
			WorkbookDataContext context = Context;
			context.PushCurrentCell(0, 0);
			try {
				return expression.Evaluate(context);
			}
			finally {
				context.PopCurrentCell();
			}
		}
		public void SetRange(CellRangeBase cellRange) {
			ParsedExpression rangeExpression = new ParsedExpression();
			BasicExpressionCreator.CreateCellRangeExpression(rangeExpression, cellRange, BasicExpressionCreatorParameter.ShouldCreate3d | BasicExpressionCreatorParameter.ShouldEncloseUnion, OperandDataType.Reference, Context);
			Expression = rangeExpression;
		}
		public void SetVariantValue(VariantValue value) {
			Expression = CreateParsedExpression(value);
		}
		public void SetVariantValueWithoutHistory(VariantValue value) {
			ParsedExpression expression = CreateParsedExpression(value);
			SetExpressionCore(expression);
		}
		ParsedExpression CreateParsedExpression(VariantValue value) {
			return BasicExpressionCreator.CreateExpressionForVariantValue(value, OperandDataType.Default, Context);
		}
		#endregion
		#region Formula
		protected internal void SetFormulaBodyTemporarily(string formulaBody) {
			this.tempBody = formulaBody;
		}
		protected internal void ParseTemporarilySavedBody() {
			string formula = tempBody;
			this.tempBody = string.Empty;
			WorkbookDataContext context = documentModel.DataContext;
			context.SetImportExportSettings();
			try {
				SetFormulaBody(formula);
			}
			finally {
				context.SetWorkbookDefinedSettings();
			}
		}
		protected void SetFormulaBody(string value) {
			tempBody = string.Empty;
			ParsedExpression newExpression = ParseExpression(value);
			if (newExpression == null || !filter.CheckExpression(newExpression))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorFormula));
			SetExpression(newExpression);
		}
		protected string GetFormulaBody() {
			if (!string.IsNullOrEmpty(tempBody))
				return tempBody;
			if (expression == null)
				return string.Empty;
			WorkbookDataContext context = Context;
			context.PushCurrentCell(0, 0);
			try {
				return expression.BuildExpressionString(context);
			}
			finally {
				context.PopCurrentCell();
			}
		}
		#endregion
		#region ISupportsCopyFrom<FormulaData> Members
		public void CopyFrom(FormulaData value) {
			Expression = value.Expression;
			CachedValue = value.CachedValue;
			tempBody = value.tempBody;
		}
		#endregion
		#region ICloneable<FormulaData> Members
		public FormulaData Clone() {
			FormulaData clone = new FormulaData(documentModel, filter);
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region Notifications
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			ProcessNotification(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			ProcessNotification(context);
		}
		public void ProcessNotification(InsertRemoveRangeNotificationContextBase context) {
			if (Expression == null)
				return;
			ParsedExpression newExpression = context.Visitor.Process(Expression.Clone());
			if (context.Visitor.FormulaChanged)
				SetExpression(newExpression);
		}
		#endregion
	} 
	#endregion
	#region IExpressionFilter
	public interface IExpressionFilter {
		bool CheckExpression(ParsedExpression expression);
	} 
	#endregion
	#region EmptyExpressionFilter
	public class EmptyExpressionFilter : IExpressionFilter {
		#region Fields
		static EmptyExpressionFilter instance;
		static readonly object syncRoot = new object();
		#endregion
		EmptyExpressionFilter() {
		}
		#region Properties
		public static EmptyExpressionFilter Instance {
			get {
				if (instance == null) {
					lock (syncRoot) {
						if (instance == null)
							instance = new EmptyExpressionFilter();
					}
				}
				return instance;
			}
		}
		#endregion
		#region IExpressionFilter Members
		public bool CheckExpression(ParsedExpression expression) {
			return true;
		}
		#endregion
	}
	#endregion
}
