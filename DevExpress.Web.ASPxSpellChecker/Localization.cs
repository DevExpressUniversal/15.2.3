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
using System.Collections.Generic;
using System.Text;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Web.Localization;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpellChecker.Localization {
	public enum ASPxSpellCheckerStringId {
		OptionsFormCaption, SpellCheckFormCaption, FinishSpellChecking, NoSuggestionsText,
		NotInDictionary, 
		IgnoreOnceButton, IgnoreAllButton, AddToDictionaryButton, 
		ChangeTo, 
		ChangeButton, ChangeAllButton, 
		GeneralOptions, 
		IgnoreUppercase, IgnoreMixedCase, IgnoreNumbers, IgnoreEmails, IgnoreUrls, IgnoreTags,
		InternationalDictionaries, ChooseDictionary, Language,
		OptionsButton, CloseButton, OkButton, CancelButton
	}
	public class ASPxSpellCheckerResLocalizer : ASPxResLocalizerBase<ASPxSpellCheckerStringId> {
		public ASPxSpellCheckerResLocalizer()
			: base(new ASPxSpellCheckerLocalizer()) {
		}
		protected override string GlobalResourceAssemblyName {
			get { return AssemblyInfo.SRAssemblySpellCheckerWeb; }
		}
		protected override string ResxName {
			get { return "LocalizationRes"; }
		}
	}
	public class ASPxSpellCheckerLocalizer : XtraLocalizer<ASPxSpellCheckerStringId> {
		static ASPxSpellCheckerLocalizer() {			
			ASPxActiveLocalizerProvider<ASPxSpellCheckerStringId> provider = new ASPxActiveLocalizerProvider<ASPxSpellCheckerStringId>(CreateResLocalizerInstance);
			SetActiveLocalizerProvider(provider);
		}
		static XtraLocalizer<ASPxSpellCheckerStringId> CreateResLocalizerInstance() {
			return new ASPxSpellCheckerResLocalizer();
		}
		public static string GetString(ASPxSpellCheckerStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ASPxSpellCheckerStringId> CreateResXLocalizer() {
			return CreateResLocalizerInstance();
		}
		protected override void PopulateStringTable() {
			AddString(ASPxSpellCheckerStringId.OptionsFormCaption, StringResources.SpellChecker_OptionsFormCaption);
			AddString(ASPxSpellCheckerStringId.SpellCheckFormCaption, StringResources.SpellChecker_SpellCheckFormCaption);
			AddString(ASPxSpellCheckerStringId.FinishSpellChecking, StringResources.SpellChecker_FinishSpellChecking);
			AddString(ASPxSpellCheckerStringId.NoSuggestionsText, StringResources.SpellChecker_NoSuggestions);
			AddString(ASPxSpellCheckerStringId.NotInDictionary, StringResources.SpellChecker_NotInDictionary);
			AddString(ASPxSpellCheckerStringId.IgnoreOnceButton, StringResources.SpellChecker_IgnoreOnceButton);
			AddString(ASPxSpellCheckerStringId.IgnoreAllButton, StringResources.SpellChecker_IgnoreAllButton);
			AddString(ASPxSpellCheckerStringId.AddToDictionaryButton, StringResources.SpellChecker_AddToDictionaryButton);
			AddString(ASPxSpellCheckerStringId.ChangeTo, StringResources.SpellChecker_ChangeTo);
			AddString(ASPxSpellCheckerStringId.ChangeButton, StringResources.SpellChecker_ChangeButton);
			AddString(ASPxSpellCheckerStringId.ChangeAllButton, StringResources.SpellChecker_ChangeAllButton);			
			AddString(ASPxSpellCheckerStringId.GeneralOptions, StringResources.SpellChecker_GeneralOptions);
			AddString(ASPxSpellCheckerStringId.IgnoreUppercase, StringResources.SpellChecker_IgnoreUppercase);
			AddString(ASPxSpellCheckerStringId.IgnoreMixedCase, StringResources.SpellChecker_IgnoreMixedCase);
			AddString(ASPxSpellCheckerStringId.IgnoreNumbers, StringResources.SpellChecker_IgnoreNumbers);
			AddString(ASPxSpellCheckerStringId.IgnoreEmails, StringResources.SpellChecker_IgnoreEmails);
			AddString(ASPxSpellCheckerStringId.IgnoreUrls, StringResources.SpellChecker_IgnoreUrls);
			AddString(ASPxSpellCheckerStringId.IgnoreTags, StringResources.SpellChecker_IgnoreTags);
			AddString(ASPxSpellCheckerStringId.InternationalDictionaries, StringResources.SpellChecker_InternationalDictionaries);
			AddString(ASPxSpellCheckerStringId.ChooseDictionary, StringResources.SpellChecker_ChooseDictionary);
			AddString(ASPxSpellCheckerStringId.Language, StringResources.SpellChecker_Language);
			AddString(ASPxSpellCheckerStringId.OptionsButton, StringResources.SpellChecker_OptionsButton);
			AddString(ASPxSpellCheckerStringId.CloseButton, StringResources.SpellChecker_CloseButton);
			AddString(ASPxSpellCheckerStringId.OkButton, StringResources.SpellChecker_OkButton);
			AddString(ASPxSpellCheckerStringId.CancelButton, StringResources.SpellChecker_CancelButton);
		}
	}
}
