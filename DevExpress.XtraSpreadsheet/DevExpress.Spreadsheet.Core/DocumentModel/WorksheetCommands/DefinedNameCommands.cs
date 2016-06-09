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

using DevExpress.Office.History;
using DevExpress.XtraSpreadsheet.Model.History;
namespace DevExpress.XtraSpreadsheet.Model {
	#region WorkbookCommand (abstract class)
	public abstract class WorkbookCommand {
		readonly DocumentModel documentModel;
		DevExpress.Office.History.HistoryTransaction transaction;
		protected WorkbookCommand(DocumentModel documentModel) {
			this.documentModel = documentModel;
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public object Result { get; protected set; }
		public void Execute() {
			BeginExecute();
			try {
				ExecuteCore();
			}
			finally {
				EndExecute();
			}
		}
		protected internal virtual void BeginExecute() {
			DocumentModel.BeginUpdate();
			this.transaction = new DevExpress.Office.History.HistoryTransaction(DocumentModel.History);
		}
		protected internal virtual void EndExecute() {
			ApplyChanges();
			this.transaction.Dispose();
			DocumentModel.EndUpdate();
		}
		protected internal abstract void ExecuteCore();
		protected internal abstract void ApplyChanges();
	}
	#endregion
	#region DefinedNameRenamedCommand
	public class DefinedNameRenamedCommand : WorkbookCommand {
		readonly DefinedName definedName;
		readonly string newName;
		public DefinedNameRenamedCommand(DefinedName definedName, string newName)
			: base(definedName.Workbook) {
			this.definedName = definedName;
			this.newName = newName;
		}
		protected internal override void ExecuteCore() {
			string oldName = definedName.Name;
			DefinedNameRenamedFormulaWalker modifier = new DefinedNameRenamedFormulaWalker(oldName, newName, definedName.ScopedSheetId, DocumentModel.DataContext);
			modifier.Walk(DocumentModel);
			DocumentHistory history = DocumentModel.History;
			DefinedNameChangeNameHistoryItem historyItem = new DefinedNameChangeNameHistoryItem(definedName, oldName, newName);
			history.Add(historyItem);
			historyItem.Execute();
			DocumentModel.InternalAPI.OnAfterDefinedNameRenamed(definedName.Workbook, definedName, oldName, newName);
			DocumentModel.RaiseSchemaChanged();
		}
		protected internal override void ApplyChanges() {
		}
	}
	#endregion
	#region DefinedNameWorkbookCreateCommand
	public class DefinedNameWorkbookCreateCommand : WorkbookCommand {
		readonly string name;
		readonly string reference;
		readonly ParsedExpression expression;
		DefinedNameWorkbookCreateCommand(DocumentModel workbook, string name)
			: base(workbook) {
			this.name = name;
		}
		public DefinedNameWorkbookCreateCommand(DocumentModel workbook, string name, string reference)
			: this(workbook, name) {
			this.reference = reference;
		}
		public DefinedNameWorkbookCreateCommand(DocumentModel workbook, string name, ParsedExpression expression)
			: this(workbook, name) {
			this.expression = expression;
		}
		protected internal override void ExecuteCore() {
			DefinedName definedName;
			if (expression == null)
				definedName = new DefinedName(DocumentModel, name, reference, -1);
			else
				definedName = new DefinedName(DocumentModel, name, expression, -1);
			DocumentModel.DefinedNames.Add(definedName);
			Result = definedName;
		}
		protected internal override void ApplyChanges() {
		}
	}
	#endregion
	#region DefinedNameWorkbookRemoveCommand
	public class DefinedNameWorkbookRemoveCommand : WorkbookCommand {
		readonly DefinedName definedName;
		public DefinedNameWorkbookRemoveCommand(DocumentModel workbook, DefinedName definedName)
			: base(workbook) {
			this.definedName = definedName;
		}
		protected internal override void ExecuteCore() {
			DocumentModel.DefinedNames.Remove(definedName.Name);
		}
		protected internal override void ApplyChanges() {
		}
	}
	#endregion
	#region DefinedNameWorkbookCollectionClearCommand
	public class DefinedNameWorkbookCollectionClearCommand : WorkbookCommand {
		public DefinedNameWorkbookCollectionClearCommand(DocumentModel workbook)
			: base(workbook) {
		}
		protected internal override void ExecuteCore() {
			DocumentModel.DefinedNames.Clear();
		}
		protected internal override void ApplyChanges() {
		}
	}
	#endregion
	#region DefinedNameWorksheetCreateCommand
	public class DefinedNameWorksheetCreateCommand : SpreadsheetModelCommand {
		readonly string name;
		readonly string reference;
		readonly ParsedExpression expression;
		public DefinedNameWorksheetCreateCommand(Worksheet worksheet, string name, string reference)
			: base(worksheet) {
			this.name = name;
			this.reference = reference;
		}
		public DefinedNameWorksheetCreateCommand(Worksheet worksheet, string name, ParsedExpression expression)
			: base(worksheet) {
			this.name = name;
			this.expression = expression;
		}
		protected internal override void ExecuteCore() {
			DefinedName definedName;
			if (expression == null)
				definedName = new DefinedName(DocumentModel, name, reference, Worksheet.SheetId);
			else
				definedName = new DefinedName(DocumentModel, name, expression, Worksheet.SheetId);
			Worksheet.DefinedNames.Add(definedName);
			Result = definedName;
		}
	}
	#endregion
	#region DefinedNameWorksheetRemoveCommand
	public class DefinedNameWorksheetRemoveCommand : SpreadsheetModelCommand {
		readonly DefinedName definedName;
		public DefinedNameWorksheetRemoveCommand(Worksheet worksheet, DefinedName definedName)
			: base(worksheet) {
			this.definedName = definedName;
		}
		protected internal override void ExecuteCore() {
			Worksheet.DefinedNames.Remove(definedName.Name);
		}
	}
	#endregion
	#region DefinedNameWorksheetCollectionClearCommand
	public class DefinedNameWorksheetCollectionClearCommand : SpreadsheetModelCommand {
		public DefinedNameWorksheetCollectionClearCommand(Worksheet worksheet)
			: base(worksheet) {
		}
		protected internal override void ExecuteCore() {
			Worksheet.DefinedNames.Clear();
		}
	}
	#endregion
}
