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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Model.History;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertTableRowCommandBase
	public abstract class InsertTableColumnCommandBase : InsertTableElementCommandBase {
		InsertTableColumnCoreCommandBase insertColumnCommand;
		protected InsertTableColumnCommandBase(IRichEditControl control)
			: base(control) {
			this.insertColumnCommand = CreateInsertTableColumnCommand(Control);
		}
		#region Properties
		protected internal virtual InsertTableColumnCoreCommandBase InsertColumnCommand { get { return insertColumnCommand; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return InsertColumnCommand.MenuCaptionStringId; } }
		public override XtraRichEditStringId DescriptionStringId { get { return InsertColumnCommand.DescriptionStringId; } }
		public override string ImageName { get { return InsertColumnCommand.ImageName; } }
		#endregion
		protected abstract InsertTableColumnCoreCommandBase CreateInsertTableColumnCommand(IRichEditControl control);
		protected internal abstract TableCell FindPatternCell();
		protected internal override void ExecuteCore() {
			DocumentModel.History.BeginTransaction();
			try {
				TableCell patternCell = FindPatternCell();
				if (patternCell == null)
					return;
				InsertColumnCommand.PatternCell = patternCell;
				InsertColumnCommand.PerformModifyModel();
#if DEBUGTEST
				DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
			}
			finally {
				DocumentModel.History.EndTransaction();
			}
		}
	}
	#endregion
	#region InsertTableColumnToTheLeftCommand
	public class InsertTableColumnToTheLeftCommand : InsertTableColumnCommandBase {
		public InsertTableColumnToTheLeftCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableColumnToTheLeftCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableColumnToTheLeft; } }
		protected override InsertTableColumnCoreCommandBase CreateInsertTableColumnCommand(IRichEditControl control) {
			return new InsertTableColumnToTheLeftCoreCommand(control);
		}
		protected internal override TableCell FindPatternCell() {
			Selection selection = DocumentModel.Selection;
			return ActivePieceTable.FindParagraph(selection.First.NormalizedStart).GetCell();
		}
	}
	#endregion
	#region InsertTableColumnToTheRightCommand
	public class InsertTableColumnToTheRightCommand : InsertTableColumnCommandBase {
		public InsertTableColumnToTheRightCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertTableColumnToTheRightCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertTableColumnToTheRight; } }
		protected override InsertTableColumnCoreCommandBase CreateInsertTableColumnCommand(IRichEditControl control) {
			return new InsertTableColumnToTheRightCoreCommand(control);
		}
		protected internal override TableCell FindPatternCell() {
			Selection selection = DocumentModel.Selection;
			return ActivePieceTable.FindParagraph(selection.NormalizedVirtualEnd).GetCell();
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertTableColumnCoreCommandBase (abstract)
	public abstract class InsertTableColumnCoreCommandBase : InsertObjectCommandBase {
		TableCell patternCell;
		protected InsertTableColumnCoreCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal TableCell PatternCell { get { return patternCell; } set { patternCell = value; } }
		protected internal override bool ExtendSelection { get { return false; } }
		#endregion
		protected internal abstract TableCell InsertColumn(TableCell patternCell);
		protected internal override void ModifyModel() {
			ChangeSelection(InsertColumn(PatternCell));
#if DEBUGTEST
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
		}
		protected internal virtual void ChangeSelection(TableCell patternCell) {
			SelectTableColumnsHistoryItem item = new SelectTableColumnsHistoryItem(DocumentModel.ActivePieceTable);
			item.Control = Control;
			item.TableIndex = patternCell.Table.Index;			
			int startColumnIndex = patternCell.GetStartColumnIndexConsiderRowGrid();
			item.StartColumnIndex = startColumnIndex;
			item.EndColumnIndex = patternCell.GetEndColumnIndexConsiderRowGrid(startColumnIndex);
			DocumentModel.History.Add(item);
			item.Execute();			
		}
	}
	#endregion
	#region InsertTableColumnToTheLeftCoreCommand
	public class InsertTableColumnToTheLeftCoreCommand : InsertTableColumnCoreCommandBase {
		public InsertTableColumnToTheLeftCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableColumnToTheLeft; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableColumnToTheLeftDescription; } }
		public override string ImageName { get { return "InsertTableColumnsToTheLeft"; } }
		#endregion
		protected internal override TableCell InsertColumn(TableCell patternCell) {
			ActivePieceTable.InsertColumnToTheLeft(patternCell, GetForceVisible());
			return patternCell.Previous;
		}
	}
	#endregion
	#region InsertTableColumnToTheRightCoreCommand
	public class InsertTableColumnToTheRightCoreCommand : InsertTableColumnCoreCommandBase {
		public InsertTableColumnToTheRightCoreCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableColumnToTheRight; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTableColumnToTheRightDescription; } }
		public override string ImageName { get { return "InsertTableColumnsToTheRight"; } }
		#endregion
		protected internal override TableCell InsertColumn(TableCell patternCell) {
			ActivePieceTable.InsertColumnToTheRight(patternCell, GetForceVisible());
			return patternCell.Next;
		}
	}
	#endregion
}
