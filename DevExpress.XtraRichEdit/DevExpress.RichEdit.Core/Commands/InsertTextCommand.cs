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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using CommandsSourceType = DevExpress.Utils.Commands.CommandSourceType;
using System.Globalization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
#if !SL
using System.Windows.Forms;
using DevExpress.Utils.Commands;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	public interface IInsertTextCommand {
		string Text { get; set; }
		CommandSourceType CommandSourceType { get; set; }
		void Execute();
	}
	#region InsertTextCommand
	public class InsertTextCommand : TransactedInsertObjectCommand, IInsertTextCommand {
		public InsertTextCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertText; } }
		public string Text {
			get {
				InsertTextCoreCommand command = (InsertTextCoreCommand)InsertObjectCommand;
				return command.Text;
			}
			set {
				InsertTextCoreCommand command = (InsertTextCoreCommand)InsertObjectCommand;
				command.Text = value;
			}
		}
		public override CommandSourceType CommandSourceType {
			get {
				InsertTextCoreCommand command = (InsertTextCoreCommand)InsertObjectCommand;
				return command.CommandSourceType;
			}
			set {
				InsertTextCoreCommand command = (InsertTextCoreCommand)InsertObjectCommand;
				command.CommandSourceType = value;
			}
		}
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new InsertTextCoreCommand(Control, String.Empty);
		}
	}
	#endregion
	#region OvertypeTextCommand
	public class OvertypeTextCommand : InsertTextCommand {
		public OvertypeTextCommand(IRichEditControl control)
			: base(control) {
		}
		public override RichEditCommandId Id { get { return RichEditCommandId.OvertypeText; } }
		protected internal override RichEditCommand CreateInsertObjectCommand() {
			return new OvertypeTextCoreCommand(Control, String.Empty);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region InsertTextCoreBaseCommand
	public abstract class InsertTextCoreBaseCommand : InsertObjectCommandBase {
		protected InsertTextCoreBaseCommand(IRichEditControl control)
			: base(control) {
		}
		protected virtual bool ResetMerging { get { return false; } }
		protected internal virtual bool AllowAutoCorrect { get { return CommandSourceType != CommandsSourceType.Unknown; } }
		protected internal override void ModifyModel() {
			if (!ActivePieceTable.Runs[DocumentModel.Selection.Interval.End.RunIndex].CanPlaceCaretBefore)
				return;
			if (AllowAutoCorrect)
				ActivePieceTable.ApplyChangesCore(DocumentModelChangeActions.ApplyAutoCorrect, RunIndex.DontCare, RunIndex.DontCare);
			InsertTextCore();
		}
		protected internal virtual void InsertTextCore() {
			string insertedText = GetInsertedText();
			if (String.IsNullOrEmpty(insertedText))
				return;
			bool forceVisible = GetForceVisible();
			if (ResetMerging)
				DocumentModel.ResetMerging();
			if (DocumentModel.Selection.Length > 0) {
				DocumentLogPosition insertPosition = DocumentModel.Selection.End;
				ActivePieceTable.InsertText(insertPosition, insertedText, forceVisible);
				OnTextInserted(insertPosition, insertedText.Length);
				return;
			}
			InputPosition pos = (CommandSourceType == CommandsSourceType.Keyboard) ? CaretPosition.GetInputPosition() : CaretPosition.TryGetInputPosition();
			if (pos != null) {
#if !SL && !DXPORTABLE
				if (CommandSourceType == CommandsSourceType.Keyboard) {
					CultureInfo culture = InputLanguage.CurrentInputLanguage.Culture;
					pos.CharacterFormatting.LangInfo = new LangInfo(culture, culture, culture);
				}
#endif
				DocumentLogPosition insertPosition = pos.LogPosition;
				ActivePieceTable.InsertText(pos, insertedText, forceVisible);
				OnTextInserted(insertPosition, insertedText.Length);
			}
			else {
				DocumentLogPosition insertPosition = DocumentModel.Selection.End;
				ActivePieceTable.InsertText(DocumentModel.Selection.End, insertedText, forceVisible);
				OnTextInserted(insertPosition, insertedText.Length);
			}
			if (ResetMerging)
				DocumentModel.ResetMerging();
		}
		protected virtual void OnTextInserted(DocumentLogPosition pos, int length) {
		}
		protected internal abstract string GetInsertedText();
	}
	#endregion
	#region InsertTextCoreCommand
	public class InsertTextCoreCommand : InsertTextCoreBaseCommand {
		string text;
		public InsertTextCoreCommand(IRichEditControl control, string text)
			: base(control) {
			this.text = text;
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertText; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertTextDescription; } }
		public string Text { get { return text; } set { text = value; } }
		#endregion
		protected internal override string GetInsertedText() {
			return Text;
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (state.Enabled)
				state.Enabled = IsContentEditable && !String.IsNullOrEmpty(Text);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}		
	}
	#endregion
	#region OvertypeTextCoreCommand
	public class OvertypeTextCoreCommand : InsertTextCoreCommand {
		public OvertypeTextCoreCommand(IRichEditControl control, string text)
			: base(control, text) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_OvertypeText; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_OvertypeTextDescription; } }
		#endregion
		protected override void OnTextInserted(DocumentLogPosition pos, int length) {
			base.OnTextInserted(pos, length);
			DocumentLogPosition deleteStart = pos + length;
			int deleteLength = CalculateDeletedLength(deleteStart, length);
			if(deleteLength > 0)
				ActivePieceTable.DeleteContent(pos + length, deleteLength, false, false, false, true, false);
		}
		int CalculateDeletedLength(DocumentLogPosition firstDeletePos, int length) {
			DocumentModelPosition startPos = PositionConverter.ToDocumentModelPosition(ActivePieceTable, firstDeletePos);
			int totalLength = -startPos.RunOffset;
			RunIndex runIndex = startPos.RunIndex;
			RunIndex maxRunIndex = new RunIndex(ActivePieceTable.Runs.Count - 1);
			while (runIndex <= maxRunIndex && totalLength < length) {
				TextRunBase run = ActivePieceTable.Runs[runIndex];
				if (!CanDeleteRun(run))
					return totalLength;
				totalLength += run.Length;
				runIndex++;
			}
			return Math.Min(totalLength, length);
		}
		protected internal override bool CanEditSelection() {
			if (!base.CanEditSelection())
				return false;
			DocumentLogPosition start = DocumentModel.Selection.End;
			int length = GetInsertedText().Length;
			int deletedLength = CalculateDeletedLength(start, length);
			return DocumentModel.ActivePieceTable.CanEditRange(start, deletedLength);
		}
		bool CanDeleteRun(TextRunBase run) {
			return (run is InlineObjectRun) || (run.GetType() == typeof(TextRun));
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
		}
	}
	#endregion
}
