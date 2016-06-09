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

using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Snap.Core.Commands;
namespace DevExpress.Snap.Core.Native {
	public class SnapCommandsCreationStrategy : ICommandsCreationStrategy {
		public PasteContentCommandBase CreateDefaultPasteCommand(XtraRichEdit.IRichEditControl control) {
			return new PasteSnxCommand(control);
		}
		public CopyAndSaveContentCommand CreateCopyContentCommand(XtraRichEdit.IRichEditControl control) {
			return new CopyAndSaveSnapContentCommand(control);
		}
		public Command CreatePasteContentCommand(IRichEditControl control, CopyAndSaveContentCommand copyCommand) {
			return new PasteSavedSnapContentCommand(control, (CopyAndSaveSnapContentCommand)copyCommand);
		}
		public DocumentModelCopyCommand CreateDocumentModelCopyCommand(PieceTable sourcePieceTable, DocumentModel target, DocumentModelCopyOptions options) {
			return new SnapDocumentModelCopyCommand((SnapPieceTable)sourcePieceTable, (SnapDocumentModel)target, options);
		}
		public NumberingListIndexCalculator CreateNumberingListIndexCalculator(DocumentModel model, NumberingType numberingListType) {
			return new SnapNumberingListIndexCalculator(model, numberingListType);
		}
		public DeleteTableRowsCommand CreateDeleteTableRowsCommand(IRichEditControl control) {
			return new SnapDeleteTableRowsCommand(control);
		}
		public DeleteCoreCommand CreateDeleteCoreCommand(IRichEditControl control) {
			return new SnapDeleteCoreCommand(control);
		}
		public DeleteBackCoreCommand CreateDeleteBackCoreCommand(IRichEditControl control) {
			return new SnapDeleteBackCoreCommand(control);
		}
		public InsertDeleteTableCellsDispatcherCommandBase CreateTableCellsCommand(IRichEditControl control) {
			return new SnapDeleteTableCellsDispatcherCommand(control);
		}
		public SplitTableCellsCommand CreateSplitTableCellsCommand(IRichEditControl control) {
			return new SnapSplitTableCellsCommand(control);
		}
		public ChangeTableRowHeightCommand CreateChangeTableRowHeightCommand(IRichEditControl control, TableRow row, int value) {
			return new SnapChangeTableRowHeightCommand(control, row, value);
		}
		public FontPropertiesModifier CreateFontPropertiesModifier(MergedCharacterProperties newValue) {
			return new SnapFontPropertiesModifier(newValue);
		}
	}
}
