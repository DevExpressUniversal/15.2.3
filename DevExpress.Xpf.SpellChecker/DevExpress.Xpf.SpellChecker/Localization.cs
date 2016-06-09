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
using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Core;
namespace DevExpress.XtraSpellChecker.Localization {
	#region SpellCheckerLocalizer
	public class SpellCheckerLocalizer : XtraLocalizer<SpellCheckerStringId> {
		static SpellCheckerLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<SpellCheckerStringId>(CreateDefaultLocalizer()));
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(SpellCheckerStringId.Form_Spelling_NoSuggestions, "(No suggestions)");
			AddString(SpellCheckerStringId.Msg_CheckNotSelectedText, "Finished checking the selection. Do you want to check the rest of the text?");
			AddString(SpellCheckerStringId.Msg_FinishCheck, "Spell check complete.");
			AddString(SpellCheckerStringId.Msg_CanUseCurrentWord, "You have chosen a word that is not found in the main or custom dictionaries.  Do you want to use this word and continue checking?");
			AddString(SpellCheckerStringId.Msg_NotLoadedDictionaryException, "Not enough resources to load {0} dictionary.");
			AddString(SpellCheckerStringId.Menu_ItemCaption, "Check Spelling");
			AddString(SpellCheckerStringId.Menu_IgnoreAllItemCaption, "Ignore All");
			AddString(SpellCheckerStringId.Menu_AddToDictionaryCaption, "Add to Dictionary");
			AddString(SpellCheckerStringId.Menu_NoSuggestionsCaption, "(no spelling suggestions)");
			AddString(SpellCheckerStringId.Menu_DeleteRepeatedWord, "Delete Repeated Word");
			AddString(SpellCheckerStringId.Menu_IgnoreRepeatedWord, "Ignore");
			AddString(SpellCheckerStringId.Form_Spelling_Caption, "Spelling");
			AddString(SpellCheckerStringId.Form_SpellingOptions_Caption, "Spelling Options");
			AddString(SpellCheckerStringId.Form_EditCustomDictionary_Caption, "Edit Custom Dictionary");
			AddString(SpellCheckerStringId.Form_SpellingOptions_IgnoreWordsInUppercase, "Ignore Words In UPPERCASE");
			AddString(SpellCheckerStringId.Form_SpellingOptions_IgnoreWordsInMixedcase, "Ignore Words In MiXEdcAse");
			AddString(SpellCheckerStringId.Form_SpellingOptions_IgnoreWordsWithNumbers, "Ignore Words With Numbers");
			AddString(SpellCheckerStringId.Form_SpellingOptions_IgnoreRepeatedWords, "Ignore Repeated Words");
			AddString(SpellCheckerStringId.Form_SpellingOptions_IgnoreEmails, "Ignore E-Mails");
			AddString(SpellCheckerStringId.Form_SpellingOptions_IgnoreWebSites, "Ignore Internet and file addresses");
			AddString(SpellCheckerStringId.Form_SpellingOptions_GeneralOptionsGroup, "General Options");
			AddString(SpellCheckerStringId.Form_SpellingOptions_EditCustomDictionaryGroup, "Edit Custom Dictionary");
			AddString(SpellCheckerStringId.Form_SpellingOptions_EditCustomDictionary, "Add, change or remove words from your custom dictionary.");
			AddString(SpellCheckerStringId.Form_SpellingOptions_EditButton, "Edit...");
			AddString(SpellCheckerStringId.Form_SpellingOptions_InternationalDictionariesGroup, "International Dictionaries");
			AddString(SpellCheckerStringId.Form_SpellingOptions_InternationalDictionaries, "Choose which dictionary to use when checking your spelling.");
			AddString(SpellCheckerStringId.Form_SpellingOptions_Language, "Language:");
			AddString(SpellCheckerStringId.Form_SpellingOptions_OkButton, "OK");
			AddString(SpellCheckerStringId.Form_SpellingOptions_CancelButton, "Cancel");
			AddString(SpellCheckerStringId.Form_SpellingOptions_ApplyButton, "Apply");
			AddString(SpellCheckerStringId.Form_Spelling_RepeatedWord, "Repeated Word:");
			AddString(SpellCheckerStringId.Form_Spelling_NotInDictionary, "Not in Dictionary:");
			AddString(SpellCheckerStringId.Form_Spelling_Suggestions, "Suggestions:");
			AddString(SpellCheckerStringId.Form_Spelling_OptionsButton, "Options...");
			AddString(SpellCheckerStringId.Form_Spelling_ChangeTo, "Change To:");
			AddString(SpellCheckerStringId.Form_Spelling_IgnoreButton, "Ignore");
			AddString(SpellCheckerStringId.Form_Spelling_IgnoreAllButton, "Ignore All");
			AddString(SpellCheckerStringId.Form_Spelling_DeleteButton, "Delete");
			AddString(SpellCheckerStringId.Form_Spelling_ChangeButton, "Change");
			AddString(SpellCheckerStringId.Form_Spelling_ChangeAllButton, "Change All");
			AddString(SpellCheckerStringId.Form_Spelling_AddButton, "Add");
			AddString(SpellCheckerStringId.Form_Spelling_SuggestButton, "Suggest");
			AddString(SpellCheckerStringId.Form_Spelling_UndoLastButton, "Undo Last");
			AddString(SpellCheckerStringId.Form_Spelling_CancelButton, "Cancel");
			AddString(SpellCheckerStringId.Form_Spelling_CloseButton, "Close");
			AddString(SpellCheckerStringId.Form_EditCustomDictionary_OkButton, "OK");
			AddString(SpellCheckerStringId.Form_EditCustomDictionary_CancelButton, "Cancel");
		}
		#endregion
		public static XtraLocalizer<SpellCheckerStringId> CreateDefaultLocalizer() {
			return new SpellCheckerResLocalizer();
		}
		public static string GetString(SpellCheckerStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<SpellCheckerStringId> CreateResXLocalizer() {
			return new SpellCheckerResLocalizer();
		}
	}
	#endregion
	#region SpellCheckerResLocalizer
	public class SpellCheckerResLocalizer : XtraResXLocalizer<SpellCheckerStringId> {
		public SpellCheckerResLocalizer()
			: base(new SpellCheckerLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.SpellChecker.LocalizationRes", typeof(SpellCheckerResLocalizer).Assembly);
		}
	}
	#endregion
	#region SpellCheckerStringId
	public enum SpellCheckerStringId {
		MsgBoxCaption,
		Msg_CheckNotSelectedText,
		Msg_FinishCheck,
		Msg_CanUseCurrentWord,
		Msg_NotLoadedDictionaryException,
		Menu_ItemCaption,
		Menu_IgnoreAllItemCaption,
		Menu_AddToDictionaryCaption,
		Menu_NoSuggestionsCaption,
		Menu_DeleteRepeatedWord,
		Menu_IgnoreRepeatedWord,
		Form_Spelling_Caption,
		Form_Spelling_NoSuggestions,
		Form_SpellingOptions_Caption,
		Form_SpellingOptions_IgnoreWordsInUppercase,
		Form_SpellingOptions_IgnoreWordsInMixedcase,
		Form_SpellingOptions_IgnoreWordsWithNumbers,
		Form_SpellingOptions_IgnoreRepeatedWords,
		Form_SpellingOptions_IgnoreEmails,
		Form_SpellingOptions_IgnoreWebSites,
		Form_SpellingOptions_GeneralOptionsGroup,
		Form_SpellingOptions_EditCustomDictionaryGroup,
		Form_SpellingOptions_EditCustomDictionary,
		Form_SpellingOptions_EditButton,
		Form_SpellingOptions_InternationalDictionariesGroup,
		Form_SpellingOptions_InternationalDictionaries,
		Form_SpellingOptions_Language,
		Form_SpellingOptions_OkButton,
		Form_SpellingOptions_CancelButton,
		Form_SpellingOptions_ApplyButton,
		Form_Spelling_RepeatedWord,
		Form_Spelling_NotInDictionary,
		Form_Spelling_Suggestions,
		Form_Spelling_OptionsButton,
		Form_Spelling_ChangeTo,
		Form_Spelling_IgnoreButton,
		Form_Spelling_IgnoreAllButton,
		Form_Spelling_DeleteButton,
		Form_Spelling_ChangeButton,
		Form_Spelling_ChangeAllButton,
		Form_Spelling_AddButton,
		Form_Spelling_SuggestButton,
		Form_Spelling_UndoLastButton,
		Form_Spelling_CancelButton,
		Form_Spelling_CloseButton,
		Form_EditCustomDictionary_Caption,
		Form_EditCustomDictionary_OkButton,
		Form_EditCustomDictionary_CancelButton,
	}
	#endregion
	#region SpellCheckerStringIdConverter
	public class SpellCheckerStringIdConverter : StringIdConverter<SpellCheckerStringId> {
		static SpellCheckerStringIdConverter() {
			SpellCheckerLocalizer.GetString(SpellCheckerStringId.MsgBoxCaption);
		}
		protected override XtraLocalizer<SpellCheckerStringId> Localizer { get { return SpellCheckerLocalizer.Active; } }
	}
	#endregion
}
