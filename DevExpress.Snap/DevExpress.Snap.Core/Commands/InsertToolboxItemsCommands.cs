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

using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Commands {
	public abstract class InsertCommandBase : SnapMenuItemSimpleCommand {
		protected InsertCommandBase(IRichEditControl richEditControl)
			: base(richEditControl) {
		}
		protected abstract string FieldType { get; }
		protected virtual bool SelectAfterInsertion { get { return false; } }
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				Selection selection = DocumentModel.Selection;
				PieceTable pieceTable = selection.PieceTable;
				DocumentLogPosition start = selection.NormalizedStart;
				if (selection.Length > 0) {
					int selectionLength = selection.Length;
					if(start + selectionLength > ActivePieceTable.DocumentEndLogPosition) {
						selectionLength = ActivePieceTable.DocumentEndLogPosition - start;
					}
					pieceTable.DeleteContent(start, selectionLength, false);
					if(selectionLength != selection.Length) {
						RunIndex runIndex = selection.Interval.NormalizedStart.RunIndex;
						UpdateSelection(selection, runIndex, runIndex);
					}
					start = selection.NormalizedStart;
				}
				Field field = pieceTable.CreateField(start, 0);
				pieceTable.InsertTextCore(pieceTable.FindParagraphIndex(start + 1), start + 1, FieldType);
				pieceTable.FieldUpdater.UpdateFieldAndNestedFields(field);
				if(SelectAfterInsertion)
					UpdateSelection(selection, field.FirstRunIndex, field.LastRunIndex);
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void UpdateSelection(Selection selection, RunIndex start, RunIndex end) {
			selection.BeginUpdate();
			selection.Start = DocumentModel.ActivePieceTable.GetRunLogPosition(start);
			selection.End = DocumentModel.ActivePieceTable.GetRunLogPosition(end);
			selection.EndUpdate();
		}
		protected override void UpdateUIStateCore(Utils.Commands.ICommandUIState state) {
			ApplyCommandsRestriction(state, CharacterFormatting);
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertCheckBoxCommand_MenuCaption, Localization.SnapStringId.InsertCheckBoxCommand_Description)]
	public class InsertCheckBoxCommand : InsertCommandBase {
		public InsertCheckBoxCommand(IRichEditControl richEditControl)
			: base(richEditControl) {
		}
		protected override string FieldType {
			get { return SNCheckBoxField.FieldType; }
		}
		public override string ImageName {
			get {
				return "CheckBox";
			}
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertBarCodeCommand_MenuCaption, Localization.SnapStringId.InsertBarCodeCommand_Description)]
	public class InsertBarCodeCommand : InsertCommandBase {
		public InsertBarCodeCommand(IRichEditControl richEditControl)
			: base(richEditControl) {
		}
		protected override string FieldType {
			get { return SNBarCodeField.FieldType; }
		}
		public override string ImageName {
			get {
				return "Barcode";
			}
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertSparklineCommand_MenuCaption, Localization.SnapStringId.InsertSparklineCommand_Description)]
	public class InsertSparklineCommand : InsertCommandBase {
		public InsertSparklineCommand(IRichEditControl richEditControl)
			: base(richEditControl) {
		}
		protected override string FieldType {
			get { return SNSparklineField.FieldType; }
		}
		public override string ImageName {
			get {
				return "Sparkline";
			}
		}
	}
	[CommandLocalization(Localization.SnapStringId.InsertChartCommand_MenuCaption, Localization.SnapStringId.InsertChartCommand_Description)]
	public class InsertChartCommand : InsertCommandBase {
		public InsertChartCommand(IRichEditControl richEditControl)
			: base(richEditControl) {
		}
		protected override string FieldType { get { return SNChartField.FieldType; } }
		public override string ImageName { get { return "Chart"; } }
		protected override bool SelectAfterInsertion { get { return true; } }
	}
	[CommandLocalization(Localization.SnapStringId.InsertIndexCommand_MenuCaption, Localization.SnapStringId.InsertIndexCommand_Description)]
	public class InsertIndexCommand : InsertCommandBase {
		public InsertIndexCommand(IRichEditControl richEditControl)
			: base(richEditControl) {
		}
		protected override string FieldType { get { return SNIndexField.FieldType; } }
		public override string ImageName { get { return "Index"; } }
	}
}
