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
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region TableRenamedFormulaWalker
	public class TableRenamedFormulaWalker : WorkbookFormulaWalker {
		readonly string oldName;
		readonly string newName;
		public TableRenamedFormulaWalker(string oldName, string newName, WorkbookDataContext context)
			: base(context) {
			this.oldName = oldName;
			this.newName = newName;
		}
		public override void Visit(ParsedThingTable thing) {
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition.IsExternalReference)
				return;
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		void ProcessTableThing(ParsedThingTable thing) {
			if (StringExtensions.CompareInvariantCultureIgnoreCase(oldName, thing.TableName) == 0) {
				thing.TableName = newName;
				FormulaChanged = true;
			}
		}
	}
	#endregion
	#region TableRemovedFormulaWalker
	public class TableRemovedFormulaWalker : WorkbookFormulaWalker {
		readonly string tableName;
		public TableRemovedFormulaWalker(string tableName, WorkbookDataContext context)
			: base(context) {
			this.tableName = tableName;
		}
		public override void Walk(PivotCache cache) {
		}
		public override void Visit(ParsedThingTable thing) {
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition.IsExternalReference)
				return;
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		void ProcessTableThing(ParsedThingTable thing) {
			if (StringExtensions.CompareInvariantCultureIgnoreCase(tableName, thing.TableName) == 0) {
				ReplaceCurrentExpression(new ParsedThingAreaErr());
				FormulaChanged = true;
			}
		}
	}
	#endregion
	#region TableColumnRenamedFormulaWalkerBase (abstract class)
	public abstract class TableColumnRenamedFormulaWalkerBase : WorkbookFormulaWalker {
		protected TableColumnRenamedFormulaWalkerBase(WorkbookDataContext context)
			: base(context) {
		}
		public override void Visit(ParsedThingTable thing) {
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition.IsExternalReference)
				return;
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		protected void ProcessTableThingCore(ParsedThingTable thing, IDictionary<string, string> tableColumnsNamesRenamed) {
			string newName;
			if (!string.IsNullOrEmpty(thing.ColumnStart) && tableColumnsNamesRenamed.TryGetValue(thing.ColumnStart, out newName)) {
				thing.ColumnStart = newName;
				FormulaChanged = true;
			}
			if (!string.IsNullOrEmpty(thing.ColumnEnd) && tableColumnsNamesRenamed.TryGetValue(thing.ColumnEnd, out newName)) {
				thing.ColumnEnd = newName;
				FormulaChanged = true;
			}
		}
		protected abstract void ProcessTableThing(ParsedThingTable thing);
	}
	#endregion
	#region TableColumnRenamedFormulaWalker
	public class TableColumnRenamedFormulaWalker : TableColumnRenamedFormulaWalkerBase {
		#region Fileds
		readonly string tableName;
		readonly IDictionary<string, string> tableColumnsNamesRenamed;
		#endregion
		public TableColumnRenamedFormulaWalker(string tableName, IDictionary<string, string> tableColumnsNamesRenamed, WorkbookDataContext context)
			: base(context) {
			this.tableName = tableName;
			this.tableColumnsNamesRenamed = tableColumnsNamesRenamed;
		}
		protected override void ProcessTableThing(ParsedThingTable thing) {
			if (StringExtensions.CompareInvariantCultureIgnoreCase(tableName, thing.TableName) == 0)
				ProcessTableThingCore(thing, tableColumnsNamesRenamed);
		}
	}
	#endregion
	#region TablesColumnRenamedFormulaWalker
	public class TablesColumnRenamedFormulaWalker : TableColumnRenamedFormulaWalkerBase {
		readonly IDictionary<string, Dictionary<string, string>> tablesColumnsNamesRenamed;
		public TablesColumnRenamedFormulaWalker(WorkbookDataContext context)
			: base(context) {
			this.tablesColumnsNamesRenamed = new Dictionary<string, Dictionary<string, string>>(StringExtensions.ComparerInvariantCultureIgnoreCase);
		}
		public int TableCount { get { return tablesColumnsNamesRenamed.Count; } }
		public void RegisterTableColumnsNamesRenamed(string tableName, Dictionary<string, string> tableColumnsRenamed) {
			tablesColumnsNamesRenamed.Add(tableName, tableColumnsRenamed);
		}
		protected override void ProcessTableThing(ParsedThingTable thing) {
			string tableName = thing.TableName;
			if (tablesColumnsNamesRenamed.ContainsKey(tableName))
				ProcessTableThingCore(thing, tablesColumnsNamesRenamed[tableName]);
		}
	}
	#endregion
	#region TableConvertedToRangeFormulaWalker
	public class TableConvertedToRangeFormulaWalker : WorkbookFormulaWalker {
		readonly string tableName;
		public TableConvertedToRangeFormulaWalker(string tableName, WorkbookDataContext context)
			: base(context) {
			this.tableName = tableName;
		}
		public override void Walk(PivotCache cache) {
			cache.Source.OnTableConvertedToRange(tableName, Context.Workbook);
		}
		public override void Walk(ICell cell) {
			Context.PushCurrentCell(cell);
			try {
				base.Walk(cell);
			}
			finally {
				Context.PopCurrentCell();
			}
		}
		public override void Visit(ParsedThingTable thing) {
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		public override void Visit(ParsedThingTableExt thing) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(thing.SheetDefinitionIndex);
			if (sheetDefinition.IsExternalReference)
				return;
			ProcessTableThing(thing);
			base.Visit(thing);
		}
		void ProcessTableThing(ParsedThingTable thing) {
			if (StringExtensions.CompareInvariantCultureIgnoreCase(tableName, thing.TableName) == 0) {
				VariantValue evaluatedValue = thing.PreEvaluate(Context);
				ParsedExpression expression = BasicExpressionCreator.CreateExpressionForVariantValue(evaluatedValue, thing.DataType, Context);
				ReplaceCurrentExpression(expression[0]);
			}
		}
	}
	#endregion
}
