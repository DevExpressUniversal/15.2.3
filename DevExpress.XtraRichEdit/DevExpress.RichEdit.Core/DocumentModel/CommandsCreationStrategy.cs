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

using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.XtraRichEdit.Model {
	public interface ICommandsCreationStrategy {
		PasteContentCommandBase CreateDefaultPasteCommand(IRichEditControl control);
		CopyAndSaveContentCommand CreateCopyContentCommand(IRichEditControl control);
		Command CreatePasteContentCommand(IRichEditControl control, CopyAndSaveContentCommand copyCommand);
		DocumentModelCopyCommand CreateDocumentModelCopyCommand(PieceTable sourcePieceTable, DocumentModel target, DocumentModelCopyOptions options);
		NumberingListIndexCalculator CreateNumberingListIndexCalculator(DocumentModel model, NumberingType numberingListType);
		DeleteTableRowsCommand CreateDeleteTableRowsCommand(IRichEditControl control);
		DeleteCoreCommand CreateDeleteCoreCommand(IRichEditControl control);
		DeleteBackCoreCommand CreateDeleteBackCoreCommand(IRichEditControl control);
		InsertDeleteTableCellsDispatcherCommandBase CreateTableCellsCommand(IRichEditControl control);
		SplitTableCellsCommand CreateSplitTableCellsCommand(IRichEditControl control);
		ChangeTableRowHeightCommand CreateChangeTableRowHeightCommand(IRichEditControl control, TableRow row, int value);
		FontPropertiesModifier CreateFontPropertiesModifier(MergedCharacterProperties newValue);
	}
	public class RichEditCommandsCreationStrategy : ICommandsCreationStrategy {
		public PasteContentCommandBase CreateDefaultPasteCommand(IRichEditControl control) {
			return null;
		}
		public CopyAndSaveContentCommand CreateCopyContentCommand(IRichEditControl control) {
			return new CopyAndSaveContentCommand(control);
		}
		public Command CreatePasteContentCommand(IRichEditControl control, CopyAndSaveContentCommand copyCommand) {
			return new PasteSavedContentCommand(control, copyCommand);
		}
		public DocumentModelCopyCommand CreateDocumentModelCopyCommand(PieceTable sourcePieceTable, DocumentModel target, DocumentModelCopyOptions options) {
			return new DocumentModelCopyCommand(sourcePieceTable, target, options);
		}
		public NumberingListIndexCalculator CreateNumberingListIndexCalculator(DocumentModel model, NumberingType numberingListType) {
			return new NumberingListIndexCalculator(model, numberingListType);
		}
		public DeleteTableRowsCommand CreateDeleteTableRowsCommand(IRichEditControl control) {
			return new DeleteTableRowsCommand(control);
		}
		public virtual DeleteCoreCommand CreateDeleteCoreCommand(IRichEditControl control) {
			return new DeleteCoreCommand(control);
		}
		public DeleteBackCoreCommand CreateDeleteBackCoreCommand(IRichEditControl control) {
			return new DeleteBackCoreCommand(control);
		}
		public InsertDeleteTableCellsDispatcherCommandBase CreateTableCellsCommand(IRichEditControl control) {
			return new DeleteTableCellsDispatcherCommand(control);
		}
		public SplitTableCellsCommand CreateSplitTableCellsCommand(IRichEditControl control) {
			return new SplitTableCellsCommand(control);
		}
		public ChangeTableRowHeightCommand CreateChangeTableRowHeightCommand(IRichEditControl control, TableRow row, int value) {
			return new ChangeTableRowHeightCommand(control, row, value);
		}
		public FontPropertiesModifier CreateFontPropertiesModifier(MergedCharacterProperties newValue) {
			return new FontPropertiesModifier(newValue);
		}
	}
	public class RichEditCommentCommandsCreationStrategy : RichEditCommandsCreationStrategy {
		public override DeleteCoreCommand CreateDeleteCoreCommand(IRichEditControl control) {
			return new CommentDeleteCoreCommand(control);
		}
	}
}
