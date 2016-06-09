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
using DevExpress.Data.Filtering;
namespace DevExpress.Data.ExpressionEditor {
	public partial class ItemClickHelper {
		public class FunctionInfo {
			string function;
			string description;
			int cursorOffset;
			FunctionEditorCategory category;
			public string Function { get { return function; } }
			public string Description { get { return description; } }
			public int CursorOffset { get { return cursorOffset; } }
			public FunctionEditorCategory Category { get { return category; } }
			public FunctionInfo(string function, string description, int cursorOffset, FunctionEditorCategory category) {
				this.function = function;
				this.description = description;
				this.cursorOffset = cursorOffset;
				this.category = category;
			}
		}
		class FunctionsClickHelper : ItemClickHelper {
			public static FunctionsClickHelper Instance(IExpressionEditor editor) {
				ExpressionEditorLogic.FunctionTypeItem selectedItem = editor.FunctionsTypes.SelectedItem as ExpressionEditorLogic.FunctionTypeItem;
				editor.ShowFunctionsTypes();
				return new FunctionsClickHelper(editor, selectedItem.Category);
			}
			Dictionary<string, int> indexToOffset = new Dictionary<string, int>();
			Dictionary<string, FunctionEditorCategory> indexToCategory = new Dictionary<string, FunctionEditorCategory>();
			List<FunctionInfo> functions = new List<FunctionInfo>();
			readonly FunctionEditorCategory category;
			public FunctionsClickHelper(IExpressionEditor editor, FunctionEditorCategory category)
				: base(editor) {
				this.category = category;
			}
			void FillFunctions() {
				functions.AddRange(DevExpress.Data.Filtering.Helpers.FunctionOperatorHelper.GetAllFunctionInfo(editor));
			}
			protected override void AddItemTable(string key, string Description, int offset) {
				base.AddItemTable(key, Description, offset);
				indexToOffset.Add(key, offset);
			}
			protected override void FillItemsTable() {
				if(functions.Count == 0)
					FillFunctions();
				FunctionEditorCategory availableCategories = category == FunctionEditorCategory.All ? editor.EditorLogic.AvailableCategories : category;
				foreach(FunctionInfo function in functions)
					if((availableCategories & function.Category) != 0) {
						AddItemTable(function.Function, function.Description, function.CursorOffset);
						indexToCategory.Add(function.Function, function.Category);
					}
			}
			public override int GetCursorOffset(string item) {
				int offset = 0;
				if(indexToOffset.TryGetValue(item, out offset))
					return offset;
				return 0;
			}
			public override string GetSpecificItem(string textItem) {
				FunctionEditorCategory itemCategory;
				if(indexToCategory.TryGetValue(textItem, out itemCategory) && itemCategory == FunctionEditorCategory.Aggregate)
					return "[]." + base.GetSpecificItem(textItem);
				return base.GetSpecificItem(textItem);
			}
		}
	}
	public enum FunctionEditorCategory {
		DateTime = 1,
		Logical = 2,
		Math = 4,
		String = 8,
		Aggregate = 16,
		All = 32
	}
}
