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
namespace DevExpress.Data.ExpressionEditor {
	public partial class ItemClickHelper {
		public static ItemClickHelper Instance(string clickedText, IExpressionEditor editor) {
			if(clickedText == editor.GetResourceString("Functions.Text"))
				return FunctionsClickHelper.Instance(editor);
			else if(clickedText == editor.GetResourceString("Operators.Text"))
				return new OperatorsClickHelper(editor);
			else if(clickedText == editor.GetResourceString("Fields.Text"))
				return new FieldsClickHelper(editor);
			else if(clickedText == editor.GetResourceString("Constants.Text"))
				return new ConstantsClickHelper(editor);
			else if(clickedText == editor.GetResourceString("Variables.Text"))
				return new VariablesClickHelper(editor);
			else if(clickedText == editor.GetResourceString("Parameters.Text"))
				return new ParametersClickHelper(editor);
			return new ItemClickHelper(editor);
		}
		protected IExpressionEditor editor;
		protected Dictionary<string, string> itemsTable;
		public virtual ColumnSortOrder ParametersSortOrder { get { return ColumnSortOrder.None; } }
		public ItemClickHelper(IExpressionEditor editor) {
			this.editor = editor;
			this.itemsTable = new Dictionary<string, string>();
		}
		public void FillItems() {
			FillItemsTable();
		}
		protected virtual void FillItemsTable() {
		}
		protected virtual void AddItemTable(string key, string description, int offset) {
			itemsTable.Add(key, description);
		}
		public object[] GetItems() {
			List<object> list = new List<object>();
			foreach(string key in itemsTable.Keys)
				list.Add(key);
			return list.ToArray();
		}
		public virtual int GetCursorOffset(string item) {
			return 0;
		}
		public string GetDescription(string name) {
			string description = string.Empty;
			itemsTable.TryGetValue(name, out description);
			return description;
		}
		public virtual string GetSpecificItem(string textItem) {
			return textItem;
		}
	}
	class OperatorsClickHelper : ItemClickHelper {
		public OperatorsClickHelper(IExpressionEditor editor) : base(editor) { }
		protected override void FillItemsTable() {
			AddItemTable(" + ", editor.GetResourceString("Plus.Description"), 0);
			AddItemTable(" - ", editor.GetResourceString("Minus.Description"), 0);
			AddItemTable(" * ", editor.GetResourceString("Multiply.Description"), 0);
			AddItemTable(" / ", editor.GetResourceString("Divide.Description"), 0);
			AddItemTable(" % ", editor.GetResourceString("Modulo.Description"), 0);
			AddItemTable(" | ", editor.GetResourceString("BitwiseOr.Description"), 0);
			AddItemTable(" & ", editor.GetResourceString("BitwiseAnd.Description"), 0);
			AddItemTable(" ^ ", editor.GetResourceString("BitwiseXor.Description"), 0);
			AddItemTable(" == ", editor.GetResourceString("Equal.Description"), 0);
			AddItemTable(" != ", editor.GetResourceString("NotEqual.Description"), 0);
			AddItemTable(" < ", editor.GetResourceString("Less.Description"), 0);
			AddItemTable(" <= ", editor.GetResourceString("LessOrEqual.Description"), 0);
			AddItemTable(" >= ", editor.GetResourceString("GreaterOrEqual.Description"), 0);
			AddItemTable(" > ", editor.GetResourceString("Greater.Description"), 0);
			AddItemTable(" In ", editor.GetResourceString("In.Description"), 0);
			AddItemTable(" Like ", editor.GetResourceString("Like.Description"), 0);
			AddItemTable(" Between ", editor.GetResourceString("Between.Description"), 0);
			AddItemTable(" And ", editor.GetResourceString("And.Description"), 0);
			AddItemTable(" Or ", editor.GetResourceString("Or.Description"), 0);
			AddItemTable(" Not ", editor.GetResourceString("Not.Description"), 0);
		}
	}
	class FieldsClickHelper : ItemClickHelper {
		public override ColumnSortOrder ParametersSortOrder { get { return ColumnSortOrder.Ascending; } }
		public FieldsClickHelper(IExpressionEditor editor) : base(editor) { }
		protected override void FillItemsTable() {
			editor.EditorLogic.FillFieldsTable(itemsTable);
		}
	}
	class ConstantsClickHelper : ItemClickHelper {
		public ConstantsClickHelper(IExpressionEditor editor) : base(editor) { }
		protected override void FillItemsTable() {
			AddItemTable("True", editor.GetResourceString("True.Description"), 0);
			AddItemTable("False", editor.GetResourceString("False.Description"), 0);
			AddItemTable("?", editor.GetResourceString("Null.Description"), 0);
		}
	}
	class ParametersClickHelper : ItemClickHelper {
		public ParametersClickHelper(IExpressionEditor editor) : base(editor) { }
		protected override void FillItemsTable() {
			editor.EditorLogic.FillParametersTable(itemsTable);
		}
	}
	class VariablesClickHelper : ItemClickHelper {
		public VariablesClickHelper(IExpressionEditor editor) : base(editor) { }
		protected override void FillItemsTable() {
			AddItemTable("[DataSource.RowCount]", editor.GetResourceString("RowCount.Description"), 0);
			AddItemTable("[DataSource.CurrentRowIndex]", editor.GetResourceString("CurrentRowIndex.Description"), 0);
		}
	}
}
