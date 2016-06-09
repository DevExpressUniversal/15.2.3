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
using System.Resources;
using System.Globalization;
using System.Windows.Forms;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraSpellChecker.Localization {
	[DXToolboxItem(false)]
	public class SpellCheckerLocalizer : XtraLocalizer<SpellCheckerStringId> {
		static SpellCheckerLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<SpellCheckerStringId>(CreateDefaultLocalizer()));
		}
#if !SL
	[DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerLocalizerActive")]
#endif
		public new static XtraLocalizer<SpellCheckerStringId> Active { 
			get { return XtraLocalizer<SpellCheckerStringId>.Active; }
			set { XtraLocalizer<SpellCheckerStringId>.Active = value; }
		}
		public override XtraLocalizer<SpellCheckerStringId> CreateResXLocalizer() {
			return new SpellCheckerResLocalizer();
		}
		public static XtraLocalizer<SpellCheckerStringId> CreateDefaultLocalizer() { return new SpellCheckerResLocalizer(); }
		#region PopulateStringTable
		protected override void PopulateStringTable() {
				AddString(SpellCheckerStringId.ListBoxNoSuggestions, "(No suggestions)");
				AddString(SpellCheckerStringId.MsgBoxCheckNotSelectedText, "Finished checking the selection. Do you want to check the rest of the text?");
				AddString(SpellCheckerStringId.MsgBoxFinishCheck, "The spelling check is complete.");
				AddString(SpellCheckerStringId.MsgCanUseCurrentWord, "You have chosen a word that is not found in the main or custom dictionaries.  Do you want to use this word and continue checking?");
				AddString(SpellCheckerStringId.MnuItemCaption, "Check Spelling");
				AddString(SpellCheckerStringId.MnuIgnoreAllItemCaption, "&Ignore All");
				AddString(SpellCheckerStringId.MnuAddtoDictionaryCaption, "&Add to Dictionary");
				AddString(SpellCheckerStringId.MnuNoSuggestionsCaption, "(no spelling suggestions)");
				AddString(SpellCheckerStringId.MnuDeleteRepeatedWord, "&Delete Repeated Word");
				AddString(SpellCheckerStringId.MnuIgnoreRepeatedWord, "&Ignore");
				AddString(SpellCheckerStringId.MsgNotLoadedDictionaryException, "Not enough resources to load {0} dictionary.");		}
		#endregion
	}
	public class SpellCheckerResLocalizer : SpellCheckerLocalizer {
		ResourceManager manager = null;
		public SpellCheckerResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
			if(manager != null) this.manager.ReleaseAllResources();
			this.manager = new ResourceManager("DevExpress.XtraSpellChecker.LocalizationRes", typeof(SpellCheckerResLocalizer).Assembly);
		}
		protected virtual ResourceManager Manager { get { return manager; } }
#if !SL
	[DevExpressXtraSpellCheckerLocalizedDescription("SpellCheckerResLocalizerLanguage")]
#endif
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; }}
		public override string GetLocalizedString(SpellCheckerStringId id) {
			string resStr = "SpellCheckerStringId." + id.ToString();
			string ret = Manager.GetString(resStr);
			if(ret == null) ret = "";
			return ret;
		}
	}
	#region enum SpellCheckerStringId
	public enum SpellCheckerStringId {
		ListBoxNoSuggestions,
		MsgBoxCheckNotSelectedText,
		MsgBoxCaption,
		MsgBoxFinishCheck,
		MsgCanUseCurrentWord,
		MsgNotLoadedDictionaryException,
		MnuItemCaption,
		MnuIgnoreAllItemCaption,
		MnuAddtoDictionaryCaption,
		MnuNoSuggestionsCaption,
		MnuDeleteRepeatedWord,
		MnuIgnoreRepeatedWord,
	}
	#endregion
}
