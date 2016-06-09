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
using System;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Data.ExpressionEditor {
	public interface IExpressionEditor {
		string GetResourceString(string stringId);
		void ShowFunctionsTypes();
		void HideFunctionsTypes();
		ExpressionEditorLogic EditorLogic { get; }
		IMemoEdit ExpressionMemoEdit { get; }
		ISelector ListOfInputTypes { get; }
		ISelector ListOfInputParameters { get; }
		ISelector FunctionsTypes { get; }
		string FilterCriteriaInvalidExpressionMessage { get; }
		string FilterCriteriaInvalidExpressionExMessage { get; }
		void ShowError(string error);
		string GetFunctionTypeStringID(string functionType);
		void SetDescription(string description);
	}
	public interface ISelector {
		void SetItemsSource(object[] items, ColumnSortOrder sortOrder);
		int ItemCount { get; }
		object SelectedItem { get; }
		int SelectedIndex { get; set; }
	}
	public interface IMemoEdit {
		int SelectionStart { get; set; }
		int SelectionLength { get; set; }
		string Text { get; set; }
		string SelectedText { get; }
		int GetLineLength(int lineIndex);
		void Paste(string text);
		void Focus();
	}
	public class StandardOperations {
		public const string Plus = " + ";
		public const string Minus = " - ";
		public const string Multiply = " * ";
		public const string Divide = " / ";
		public const string Modulo = " % ";
		public const string Equal = " == ";
		public const string NotEqual = " != ";
		public const string Less = " < ";
		public const string LessOrEqual = " <= ";
		public const string LargerOrEqual = " >= ";
		public const string Larger = " > ";
		public const string And = " And ";
		public const string Or = " Or ";
		public const string Not = " Not ";
	}
	public abstract class ExpressionEditorLogic {
		public class FunctionTypeItem {
			public FunctionEditorCategory Category { get; set; }
			public string Name { get; set; }
			public override string ToString() {
				return Name;
			}
		}
		static string GetFunctionType(IExpressionEditor editor, string functionType) {
			return editor.GetResourceString(editor.GetFunctionTypeStringID(functionType));
		}
		protected readonly IExpressionEditor editor;
		protected readonly object contextInstance;
		protected IMemoEdit ExpressionMemoEdit { get { return editor.ExpressionMemoEdit; } }
		ItemClickHelper itemClickHelper;
		FunctionEditorCategory availableCategories;
		ISelector ListOfInputTypes { get { return editor.ListOfInputTypes; } }
		ISelector ListOfInputParameters { get { return editor.ListOfInputParameters; } }
		ISelector FunctionsTypes { get { return editor.FunctionsTypes; } }
		internal FunctionEditorCategory AvailableCategories { get { return availableCategories; } }
		protected ExpressionEditorLogic(IExpressionEditor editor, object contextInstance) {
			this.editor = editor;
			this.contextInstance = contextInstance;
		}
		public void Initialize() {
			ListOfInputTypes.SetItemsSource(GetListOfInputTypesObjects(), ColumnSortOrder.None);
			FunctionTypeItem[] items = GetFunctionTypes();
			FunctionEditorCategory availableCategories = (FunctionEditorCategory)0;
			int count = items.Length;
			for (int i = 0; i < count; i++)
				availableCategories |= items[i].Category;
			this.availableCategories = availableCategories;
			FunctionsTypes.SetItemsSource(items, ColumnSortOrder.None);
			FunctionsTypes.SelectedIndex = 0;
			ResetMemoText();
		}
		public string GetExpression() {
			return ConvertToFields(ExpressionMemoEdit.Text);
		}
		public void ResetMemoText() {
			ExpressionMemoEdit.Text = ConvertToCaption(GetExpressionMemoEditText());
			if(ListOfInputTypes.ItemCount > 0)
				ListOfInputTypes.SelectedIndex = 0;
		}
		public void OnInputTypeChanged() {
			editor.HideFunctionsTypes();
			RefreshInputParameters();
		}
		public void OnFunctionTypeChanged() {
			RefreshInputParameters();
		}
		public void OnInputParametersChanged() {
			if(ListOfInputParameters.SelectedItem == null)
				return;
			editor.SetDescription(itemClickHelper.GetDescription(ListOfInputParameters.SelectedItem.ToString()));
		}
		public bool CanCloseWithOKResult() {
			return ValidateExpression();
		}
		public void OnLoad() {
			ExpressionMemoEdit.SelectionStart = ExpressionMemoEdit.Text.Length;
			ExpressionMemoEdit.SelectionLength = 0;
		}
		public void OnInsertInputParameter() {
			if(ListOfInputParameters.SelectedItem == null)
				return;
			InsertTextInExpressionMemo(itemClickHelper.GetSpecificItem(ListOfInputParameters.SelectedItem.ToString()));
			ExpressionMemoEdit.SelectionStart -= itemClickHelper.GetCursorOffset(ListOfInputParameters.SelectedItem.ToString());
		}
		public void OnWrapExpression() {
			bool noSelect = string.IsNullOrEmpty(ExpressionMemoEdit.SelectedText);
			InsertTextInExpressionMemo("(" + ExpressionMemoEdit.SelectedText + ")");
			if(noSelect)
				ExpressionMemoEdit.SelectionStart--;
		}
		public void OnInsertOperation(string operation) {
			InsertTextInExpressionMemo(operation);
		}
		protected internal abstract void FillParametersTable(Dictionary<string, string> itemsTable);
		protected internal abstract void FillFieldsTable(Dictionary<string, string> itemsTable);
		protected abstract object[] GetListOfInputTypesObjects();
		protected abstract string GetExpressionMemoEditText();
		protected virtual string ConvertToCaption(string expression) { return expression; }
		protected virtual bool ValidateExpression() {
			try {
				CriteriaOperator.Parse(ExpressionMemoEdit.Text, null);
				ValidateExpressionEx(ExpressionMemoEdit.Text);
			} catch(CriteriaParserException exception) {
				ShowError(exception);
				int verticalSelectionOffset = exception.Line * 2;
				for(int i = 0; i < exception.Line; i++)
					verticalSelectionOffset += ExpressionMemoEdit.GetLineLength(i);
				ExpressionMemoEdit.SelectionStart = verticalSelectionOffset + exception.Column;
				ExpressionMemoEdit.SelectionLength = 1;
				ExpressionMemoEdit.Focus();
				return false;
			} catch(Exception e) {
				ShowError(e);
				ExpressionMemoEdit.Focus();
				return false;
			}
			return true;
		}
		protected virtual void ValidateExpressionEx(string expression) {
		}
		protected virtual void ShowError(Exception exception) {
			string message = "";
			CriteriaParserException ce = exception as CriteriaParserException;
			if(ce != null)
				message = string.Format(editor.FilterCriteriaInvalidExpressionMessage, ce.Line + 1, ce.Column);
			else
				message = string.Format(editor.FilterCriteriaInvalidExpressionExMessage, exception.Message);
			editor.ShowError(message);
		}
		public virtual string ConvertToFields(string expression) { return expression; }
		public void InsertTextInExpressionMemo(string text) {
			int resultStart = ExpressionMemoEdit.SelectionStart + ExpressionMemoEdit.SelectionLength + text.Length;
			ExpressionMemoEdit.Paste(text);
			ExpressionMemoEdit.Focus();
			ExpressionMemoEdit.SelectionStart = resultStart;
			ExpressionMemoEdit.SelectionLength = 0;
		}
		protected virtual ItemClickHelper GetItemClickHelper(string selectedItemText, IExpressionEditor editor) {
			return ItemClickHelper.Instance(selectedItemText, editor);
		}
		protected virtual void RefreshInputParameters() {
			if(ListOfInputTypes.SelectedItem != null) {
				itemClickHelper = GetItemClickHelper(ListOfInputTypes.SelectedItem.ToString(), editor);
				itemClickHelper.FillItems();
				ListOfInputParameters.SetItemsSource(itemClickHelper.GetItems(), itemClickHelper.ParametersSortOrder);
			}
		}
		FunctionTypeItem[] GetFunctionTypes() {
			List<FunctionTypeItem> result = new List<FunctionTypeItem>();
			foreach(FunctionEditorCategory category in GetFunctionsTypeNames())
				result.Add(new FunctionTypeItem() { Category = category, Name = GetFunctionType(editor, category.ToString() + "Items") });
			return result.ToArray();
		}
		protected virtual IList<FunctionEditorCategory> GetFunctionsTypeNames() {
			return new List<FunctionEditorCategory> {
				FunctionEditorCategory.All,
				FunctionEditorCategory.DateTime,
				FunctionEditorCategory.Logical,
				FunctionEditorCategory.Math,
				FunctionEditorCategory.String
			};
		}
	}
	public static class UnboundExpressionConvertHelper {
		class DataColumnInfoAdapter : IBoundProperty {
			IDataColumnInfo info;
			public DataColumnInfoAdapter(IDataColumnInfo info) { this.info = info; }
			public string Name { get { return info.FieldName; } }
			public string DisplayName { get { return info.Caption; } }
			public Type Type { get { throw new NotImplementedException(); } }
			public bool HasChildren { get { return true; } }
			List<IBoundProperty> GetChildren(IBoundProperty parent) {
				if(info.Columns == null)
					return new List<IBoundProperty>();
				return info.Columns
					   .Select(c => new DataColumnInfoAdapter(c) { Parent = parent })
					   .Cast<IBoundProperty>()
					   .ToList();
			}
			public List<IBoundProperty> ChildrenWithoutParent { get { return GetChildren(null); } }
			public List<IBoundProperty> Children { get { return GetChildren(this); } }
			public bool IsAggregate { get { throw new NotImplementedException(); } }
			public bool IsList { get { return false; } }
			public IBoundProperty Parent { get; private set; }
		}
		static string ConvertString(IDataColumnInfo columnInfo, string expression, bool fromEditor) {
			try {
				return CriteriaOperator.ToString(ConvertCriteria(columnInfo, expression, fromEditor));
			} catch {
				return expression;
			}
		}
		public static string ConvertToCaption(IDataColumnInfo columnInfo, string expression) {
			return ConvertString(columnInfo, expression, false);
		}
		public static string ConvertToFields(IDataColumnInfo columnInfo, string expression) {
			return ConvertString(columnInfo, expression, true);
		}
		static CriteriaOperator ConvertCriteria(IDataColumnInfo columnInfo, string expression, bool fromEditor) {
			CriteriaOperator op = CriteriaOperator.Parse(expression);
			if(ReferenceEquals(op, null))
				return null;
			op.Accept(new DisplayNameVisitor {
				Columns = new DataColumnInfoAdapter(columnInfo).ChildrenWithoutParent,
				FromEditor = fromEditor
			});
			return op;
		}
		public static void ValidateExpressionFields(IDataColumnInfo columnInfo, string expression) {
			if(columnInfo.Controller == null)
				return;
			columnInfo.Controller.ValidateExpression(ConvertCriteria(columnInfo, expression, true));
		}
	}
	public class ExpressionEditorLogicEx : ExpressionEditorLogic {
		IDataColumnInfo ColumnInfo { get { return contextInstance as IDataColumnInfo; } }
		public ExpressionEditorLogicEx(IExpressionEditor editor, IDataColumnInfo columnInfo)
			: base(editor, columnInfo) {
		}
		protected override object[] GetListOfInputTypesObjects() {
			if(ColumnInfo == null || ColumnInfo.Columns.Count == 0)
				return new object[] {
					editor.GetResourceString("Functions.Text"),
					editor.GetResourceString("Operators.Text"),
					editor.GetResourceString("Constants.Text")};
			else
				return new object[] {
					editor.GetResourceString("Functions.Text"),
					editor.GetResourceString("Operators.Text"),
					editor.GetResourceString("Fields.Text"),
					editor.GetResourceString("Constants.Text")};
		}
		protected override string GetExpressionMemoEditText() {
			return ColumnInfo == null ? string.Empty : ColumnInfo.UnboundExpression;
		}
		protected override string ConvertToCaption(string expression) {
			return UnboundExpressionConvertHelper.ConvertToCaption(ColumnInfo, expression);
		}
		protected override void ValidateExpressionEx(string expression) {
			expression = ConvertToFields(expression);
			UnboundExpressionConvertHelper.ValidateExpressionFields(ColumnInfo, expression);
		}
		public override string ConvertToFields(string expression) {
			return UnboundExpressionConvertHelper.ConvertToFields(ColumnInfo, expression);
		}
		protected internal override void FillFieldsTable(Dictionary<string, string> itemsTable) {
			if(ColumnInfo == null) return;
			foreach(IDataColumnInfo info in ColumnInfo.Columns) {
				if (info.FieldType == null)
					continue;
				string escaped = UnboundExpressionConvertHelper.ConvertToCaption(ColumnInfo, EscapeFieldName(info.FieldName));
				itemsTable[escaped] = string.Format(editor.GetResourceString("GridFields Description Prefix"),
					info.Name, info.Caption, info.FieldType.ToString());
			}
		}
		private static string EscapeFieldName(string fieldName) {
			return new OperandProperty(fieldName).ToString();
		}
		protected internal override void FillParametersTable(Dictionary<string, string> itemsTable) { }
	}
}
