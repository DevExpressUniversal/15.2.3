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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Office.History;
using DevExpress.Office.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region UndoCommand
	public class UndoCommand : UndoRedoCommandBase {
		public UndoCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.FileUndo; } }
		public override OfficeStringId OfficeMenuCaptionStringId { get { return OfficeStringId.MenuCmd_Undo; } }
		public override OfficeStringId OfficeDescriptionStringId { get { return OfficeStringId.MenuCmd_UndoDescription; } }
		protected override bool UseOfficeTextsAndImage { get { return true; } }
		public override string ImageName { get { return "Undo"; } }
		#endregion
		protected internal override void PerformHistoryOperation(DocumentHistory history) {
			history.Undo();
		}
		protected internal override bool CanPerformHistoryOperation(DocumentHistory history) {
			return history.CanUndo;
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			CheckExecutedAtUIThread();
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.Undo, !InnerControl.IsAnyInplaceEditorActive && CanPerformHistoryOperation(DocumentModel.History));
		}
	}
	#endregion
	#region HistoryCommandBase (abstract class)
	public abstract class HistoryCommandBase : SpreadsheetMenuItemSimpleCommand {
		protected HistoryCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			DocumentModel documentModel = DocumentModel;
			documentModel.BeginUpdate();
			try {
				PerformHistoryOperation(documentModel.History);
				InvalidateDocumentLayout();
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal virtual void InvalidateDocumentLayout() {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable && CanPerformHistoryOperation(DocumentModel.History);
			state.Visible = true;
		}
		protected internal abstract void PerformHistoryOperation(DocumentHistory history);
		protected internal abstract bool CanPerformHistoryOperation(DocumentHistory history);
	}
	#endregion
	#region UndoRedoCommandBase
	public abstract class UndoRedoCommandBase : HistoryCommandBase, IAffectedCellsRepository {
		readonly List<CellContentSnapshot> affectedCells = new List<CellContentSnapshot>();
		protected UndoRedoCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			DocumentModel documentModel = DocumentModel;
			try {
				documentModel.AffectedCellsRepository = this;
				base.ExecuteCore();
			}
			finally {
				documentModel.AffectedCellsRepository = null;
			}
			foreach (CellContentSnapshot item in affectedCells)
				documentModel.InternalAPI.RaiseCellValueChanged(item);
		}
		#region IAffectedCellsRepository Members
		public void Add(CellContentSnapshot item) {
			affectedCells.Add(item);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IAffectedCellsRepository {
		void Add(CellContentSnapshot item);
	}
	public partial class DocumentModel {
		protected internal IAffectedCellsRepository AffectedCellsRepository { get; set; }
	}
}
