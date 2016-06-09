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
	#region SheetDefinitionWalkerBase
	public abstract class SheetDefinitionWalkerBase : WorkbookFormulaWalker {
		protected SheetDefinitionWalkerBase(WorkbookDataContext context)
			: base(context) {
		}
		protected abstract int ProcessSheetDefinition(int sheetDefinitionIndex);
		public override void Visit(ParsedThingArea3d expression) {
			int updatedSheetDefinitionIndex = ProcessSheetDefinition(expression.SheetDefinitionIndex);
			if (expression.SheetDefinitionIndex != updatedSheetDefinitionIndex) {
				expression.SheetDefinitionIndex = updatedSheetDefinitionIndex;
				FormulaChanged = true;
			}
			base.Visit(expression);
		}
		public override void Visit(ParsedThingArea3dRel expression) {
			int updatedSheetDefinitionIndex = ProcessSheetDefinition(expression.SheetDefinitionIndex);
			if (expression.SheetDefinitionIndex != updatedSheetDefinitionIndex) {
				expression.SheetDefinitionIndex = updatedSheetDefinitionIndex;
				FormulaChanged = true;
			}
			base.Visit(expression);
		}
		public override void Visit(ParsedThingAreaErr3d expression) {
			int updatedSheetDefinitionIndex = ProcessSheetDefinition(expression.SheetDefinitionIndex);
			if (expression.SheetDefinitionIndex != updatedSheetDefinitionIndex) {
				expression.SheetDefinitionIndex = updatedSheetDefinitionIndex;
				FormulaChanged = true;
			}
			base.Visit(expression);
		}
		public override void Visit(ParsedThingRef3d expression) {
			int updatedSheetDefinitionIndex = ProcessSheetDefinition(expression.SheetDefinitionIndex);
			if (expression.SheetDefinitionIndex != updatedSheetDefinitionIndex) {
				expression.SheetDefinitionIndex = updatedSheetDefinitionIndex;
				FormulaChanged = true;
			}
			base.Visit(expression);
		}
		public override void Visit(ParsedThingRef3dRel expression) {
			int updatedSheetDefinitionIndex = ProcessSheetDefinition(expression.SheetDefinitionIndex);
			if (expression.SheetDefinitionIndex != updatedSheetDefinitionIndex) {
				expression.SheetDefinitionIndex = updatedSheetDefinitionIndex;
				FormulaChanged = true;
			}
			base.Visit(expression);
		}
		public override void Visit(ParsedThingErr3d expression) {
			int updatedSheetDefinitionIndex = ProcessSheetDefinition(expression.SheetDefinitionIndex);
			if (expression.SheetDefinitionIndex != updatedSheetDefinitionIndex) {
				expression.SheetDefinitionIndex = updatedSheetDefinitionIndex;
				FormulaChanged = true;
			}
			base.Visit(expression);
		}
		public override void Visit(ParsedThingNameX expression) {
			int updatedSheetDefinitionIndex = ProcessSheetDefinition(expression.SheetDefinitionIndex);
			if (expression.SheetDefinitionIndex != updatedSheetDefinitionIndex) {
				expression.SheetDefinitionIndex = updatedSheetDefinitionIndex;
				FormulaChanged = true;
			}
			base.Visit(expression);
		}
		public override void Visit(ParsedThingUnknownFuncExt expression) {
			int updatedSheetDefinitionIndex = ProcessSheetDefinition(expression.SheetDefinitionIndex);
			if (expression.SheetDefinitionIndex != updatedSheetDefinitionIndex) {
				expression.SheetDefinitionIndex = updatedSheetDefinitionIndex;
				FormulaChanged = true;
			}
			base.Visit(expression);
		}
	}
	#endregion
	public class SheetRemovedFormulaWalker : SheetDefinitionWalkerBase {
		#region inner classes
		abstract class RemoveSheetFrom3DExpressionHelper {
			public static RemoveSheetFrom3DExpressionHelper Instance(SheetDefinition expression, WorkbookDataContext context, int deletedSheetId) {
				string sheetName = context.Workbook.Sheets.GetById(deletedSheetId).Name;
				if (StringExtensions.CompareInvariantCultureIgnoreCase(expression.SheetNameStart, sheetName) == 0)
					return new RemoveStartSheetFrom3DExpressionHelper(expression, context);
				if (StringExtensions.CompareInvariantCultureIgnoreCase(expression.SheetNameEnd, sheetName) == 0)
					return new RemoveEndSheetFrom3DExpressionHelper(expression, context);
				return null;
			}
			#region Fields
			SheetDefinition sheetDefinition;
			List<IWorksheet> sheets;
			#endregion
			public RemoveSheetFrom3DExpressionHelper(SheetDefinition sheetDefinition, WorkbookDataContext context) {
				this.sheetDefinition = sheetDefinition.Clone();
				sheets = sheetDefinition.GetReferencedSheets(context);
			}
			#region Properties
			protected SheetDefinition SheetDefinition { get { return sheetDefinition; } }
			protected List<IWorksheet> Sheets { get { return sheets; } }
			#endregion
			public SheetDefinition ProcessExpression() {
				if (Sheets.Count == 1)
					ProcessAloneSheetReference();
				else if (Sheets.Count == 2)
					ProcessDoubleSheetReference();
				else
					Process3DSheetReference();
				return sheetDefinition;
			}
			protected abstract void ProcessAloneSheetReference();
			protected abstract void ProcessDoubleSheetReference();
			protected abstract void Process3DSheetReference();
		}
		class RemoveStartSheetFrom3DExpressionHelper : RemoveSheetFrom3DExpressionHelper {
			public RemoveStartSheetFrom3DExpressionHelper(SheetDefinition expression, WorkbookDataContext context)
				: base(expression, context) {
			}
			protected override void ProcessAloneSheetReference() {
				SheetDefinition.ValidReference = false;
			}
			protected override void ProcessDoubleSheetReference() {
				SheetDefinition.SheetNameStart = SheetDefinition.SheetNameEnd;
				SheetDefinition.SheetNameEnd = string.Empty;
			}
			protected override void Process3DSheetReference() {
				SheetDefinition.SheetNameStart = Sheets[1].Name;
			}
		}
		class RemoveEndSheetFrom3DExpressionHelper : RemoveSheetFrom3DExpressionHelper {
			public RemoveEndSheetFrom3DExpressionHelper(SheetDefinition expression, WorkbookDataContext context)
				: base(expression, context) {
			}
			protected override void ProcessAloneSheetReference() {
				DevExpress.Office.Utils.Exceptions.ThrowInternalException();
			}
			protected override void ProcessDoubleSheetReference() {
				SheetDefinition.SheetNameEnd = string.Empty;
			}
			protected override void Process3DSheetReference() {
				SheetDefinition.SheetNameEnd = Sheets[Sheets.Count - 2].Name;
			}
		}
		#endregion
		readonly int sheetId;
		public SheetRemovedFormulaWalker(int sheetId, WorkbookDataContext context)
			: base(context) {
			this.sheetId = sheetId;
		}
		public int SheetId { get { return sheetId; } }
		public override void Walk(Worksheet sheet) {
			if (sheet.SheetId == sheetId)
				return;
			base.Walk(sheet);
		}
		protected override int ProcessSheetDefinition(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = Context.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition.IsExternalReference)
				return sheetDefinitionIndex;
			RemoveSheetFrom3DExpressionHelper removeSheetHelper = RemoveSheetFrom3DExpressionHelper.Instance(sheetDefinition, Context, sheetId);
			if (removeSheetHelper != null) {
				SheetDefinition updatedSheetDefinition = removeSheetHelper.ProcessExpression();
				int updatedSheetDefinitionIndex = Context.RegisterSheetDefinition(updatedSheetDefinition);
				if (updatedSheetDefinitionIndex != sheetDefinitionIndex) {
					FormulaChanged = true;
					return updatedSheetDefinitionIndex;
				}
			}
			return sheetDefinitionIndex;
		}
		public override void Visit(ParsedThingTable thing) {
			Table table = Context.GetDefinedTableRange(thing.TableName);
			if (Context.Workbook.Sheets.GetById(sheetId).Tables.Contains(table))
				ReplaceCurrentExpression(new ParsedThingRefErr());
			base.Visit(thing);
		}
	}
}
