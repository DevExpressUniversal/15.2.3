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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraSpellChecker;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.SpellChecker;
using System.Globalization;
#if !SL
using System.Windows.Forms;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Commands {
	#region SpellingCommandBase (abstract class)
	public abstract class SpellCheckerCommandBase : RichEditMenuItemSimpleCommand {
		protected SpellCheckerCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected ISpellChecker SpellChecker { get { return DocumentServer.SpellChecker; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable && SpellChecker != null && CanExecuteCommand();
			state.Visible = true;
		}
		protected MisspelledInterval GetMisspelledInterval() {
			DocumentLogPosition logPosition = ActivePieceTable.DocumentModel.Selection.Interval.NormalizedStart.LogPosition;
			MisspelledIntervalCollection intervals = ActivePieceTable.SpellCheckerManager.MisspelledIntervals;
			return intervals.FindInerval(logPosition);
		}
		protected string GetMisspelledWord(MisspelledInterval interval) {
			return ActivePieceTable.GetFilteredPlainText(interval.Interval.Start, interval.Interval.End, ActivePieceTable.VisibleTextFilter.IsRunVisible);
		}
		protected CultureInfo GetActualCulture(MisspelledInterval interval) {
			if (DocumentServer.Options.SpellChecker.AutoDetectDocumentCulture)
				return GetLocalCulture(interval.Interval.Start, interval.Interval.End);
			return DocumentServer.SpellChecker.Culture;
		}
		CultureInfo GetLocalCulture(DocumentModelPosition start, DocumentModelPosition end) {
			LangInfo? langInfo = DocumentModel.ActivePieceTable.GetLanguageInfo(start, end);
			if (!langInfo.HasValue)
				return null;
			LangInfo value = langInfo.Value;
			return value.Latin != null ? value.Latin : DocumentServer.SpellChecker.Culture;
		}
		protected abstract bool CanExecuteCommand();
	}
	#endregion
	#region CheckSpellingCommand
	public class CheckSpellingCommand : SpellCheckerCommandBase {
		public CheckSpellingCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CheckSpellingCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_CheckSpelling; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CheckSpellingCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_CheckSpellingDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CheckSpellingCommandImageName")]
#endif
		public override string ImageName { get { return "SpellCheck"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("CheckSpellingCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.CheckSpelling; } }
		protected internal override void ExecuteCore() {
			SpellChecker.Check(Control);
		}
		protected override bool CanExecuteCommand() {
			return true;
		}
	}
	#endregion
	#region IgnoreAllMisspellingsCommand
	public class IgnoreAllMisspellingsCommand : SpellCheckerCommandBase {
		public IgnoreAllMisspellingsCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_IgnoreAllMistakenWords; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_IgnoreAllMistakenWordsDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.IgnoreAllMisspellings; } }
		protected internal override void ExecuteCore() {
			MisspelledInterval interval = GetMisspelledInterval();
			ActivePieceTable.SpellCheckerManager.MisspelledIntervals.Remove(interval);
			string word = GetMisspelledWord(interval);
			SpellChecker.IgnoreAll(Control, word);
		}
		protected override bool CanExecuteCommand() {
			MisspelledInterval interval = GetMisspelledInterval();
			return interval != null && interval.ErrorType == SpellingError.Misspelling;
		}
	}
	#endregion
	#region IgnoreMisspellingCommand
	public class IgnoreMisspellingCommand : SpellCheckerCommandBase {
		public IgnoreMisspellingCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_IgnoreMistakenWord; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_IgnoreMistakenWordDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.IgnoreMisspelling; } }
		protected internal override void ExecuteCore() {
			MisspelledInterval interval = GetMisspelledInterval();
			string word = GetMisspelledWord(interval);
			SpellChecker.Ignore(Control, word, new DocumentPosition(interval.Interval.Start.Clone()), new DocumentPosition(interval.Interval.End.Clone()));   
		}
		protected override bool CanExecuteCommand() {
			return GetMisspelledInterval() != null;
		}
	}
	#endregion
	#region AddWordToDictionaryCommand
	public class AddWordToDictionaryCommand : SpellCheckerCommandBase {
		public AddWordToDictionaryCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_AddWordToDictionary; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_AddWordToDictionaryDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.AddWordToDictionary; } }
		protected internal override void ExecuteCore() {
			MisspelledInterval interval = GetMisspelledInterval();
			string word = GetMisspelledWord(interval);
			SpellChecker.AddToDictionary(word, GetActualCulture(interval));
		}
		protected override bool CanExecuteCommand() {
			MisspelledInterval interval = GetMisspelledInterval();
			return interval != null && interval.ErrorType == SpellingError.Misspelling && SpellChecker.CanAddToDictionary(GetActualCulture(interval));
		}
	}
	#endregion
	#region DeleteRepeatedWordCommand
	public class DeleteRepeatedWordCommand : SpellCheckerCommandBase {
		public DeleteRepeatedWordCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteRepeatedWordDescription; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_DeleteRepeatedWord; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.DeleteRepeatedWord; } }
		protected override bool CanExecuteCommand() {
			MisspelledInterval interval = GetMisspelledInterval();
			return interval != null && interval.ErrorType == SpellingError.Repeating && ActivePieceTable.CanEditRange(interval.Start, interval.End);
		}
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				MisspelledInterval interval = GetMisspelledInterval();
				RichEditSpellCheckController controller = new RichEditSpellCheckController(Control);
				DocumentModelPosition start = interval.Interval.Start.Clone();
				DocumentModelPosition end = interval.Interval.End.Clone();
				controller.PreparePositionsForDelete(start, end);
				ActivePieceTable.DeleteContent(start.LogPosition, end.LogPosition - start.LogPosition, false);
			}
			finally {
				DocumentModel.EndUpdate();
			}
			ActiveView.EnforceFormattingCompleteForVisibleArea();
		}
	}
	#endregion
}
