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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.SpellChecker;
using DevExpress.XtraSpellChecker;
namespace DevExpress.XtraRichEdit.Commands {
	#region ReplaceMisspellingCommand
	public class ReplaceMisspellingCommand : SpellCheckerCommandBase {
		public ReplaceMisspellingCommand(IRichEditControl control, string suggestion)
			: base(control) {
			Suggestion = suggestion;
		}
		public ReplaceMisspellingCommand(IRichEditControl control)
			: this(control, String.Empty) {
		}
		#region Properties
		public override string MenuCaption {
			get {
				if (!String.IsNullOrEmpty(Suggestion))
					return Suggestion;
				return base.MenuCaption;
			}
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeMistakenWord; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ChangeMistakenWordDescription; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.ReplaceMisspelling; } }
		public string Suggestion { get; set; }
		#endregion
		protected internal override void ExecuteCore() {
			DocumentModel.BeginUpdate();
			try {
				MisspelledInterval interval = GetMisspelledInterval();
				RichEditSpellCheckController controller = new RichEditSpellCheckController(Control);
				controller.ReplaceWord(interval.Interval.Start, interval.Interval.End, Suggestion);
				DocumentModel.Selection.Start = interval.Start + Suggestion.Length;
				DocumentModel.Selection.End = DocumentModel.Selection.Start;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		public override void ForceExecute(ICommandUIState state) {
			string value = state.EditValue as string;
			if (!String.IsNullOrEmpty(value))
				Suggestion = value;
			else if (String.IsNullOrEmpty(Suggestion)) {
				string[] suggestions = GetSuggestions(GetMisspelledInterval());
				if (suggestions != null && suggestions.Length > 0)
					Suggestion = suggestions[0];
				else
					Suggestion = String.Empty;
			}
			base.ForceExecute(state);
		}
		protected override bool CanExecuteCommand() {
			MisspelledInterval interval = GetMisspelledInterval();
			if (interval == null || interval.ErrorType != SpellingError.Misspelling || !ActivePieceTable.CanEditRange(interval.Start, interval.End))
				return false;
			return !String.IsNullOrEmpty(Suggestion) || HasSuggestions(interval);
		}
		string[] GetSuggestions(MisspelledInterval interval) {
			string misspellingWord = GetMisspelledWord(interval);
			System.Globalization.CultureInfo culture = GetActualCulture(interval);
			return SpellChecker.GetSuggestions(misspellingWord, culture);
		}
		bool HasSuggestions(MisspelledInterval interval) {
			string[] suggestions = GetSuggestions(interval);
			return suggestions != null && suggestions.Length > 0;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<string>();
		}
	}
	#endregion
}
