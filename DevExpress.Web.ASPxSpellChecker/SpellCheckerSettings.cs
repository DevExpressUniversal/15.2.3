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

using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxSpellChecker.Localization;
namespace DevExpress.Web.ASPxSpellChecker {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ASPxSpellCheckerBaseSettings : PropertiesBase, IPropertiesOwner {
		public ASPxSpellCheckerBaseSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
	}
	public class ASPxSpellCheckerSpellingSettings : ASPxSpellCheckerBaseSettings {
		public ASPxSpellCheckerSpellingSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSpellingSettingsIgnoreWordsWithNumber"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public virtual bool IgnoreWordsWithNumber {
			get { return GetBoolProperty("IgnoreWordsWithNumber", true); }
			set {
				if(value != IgnoreWordsWithNumber)
					SetBoolProperty("IgnoreWordsWithNumber", true, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSpellingSettingsIgnoreUpperCaseWords"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public virtual bool IgnoreUpperCaseWords {
			get { return GetBoolProperty("IgnoreUpperCaseWords", true); }
			set {
				if(IgnoreUpperCaseWords != value)
					SetBoolProperty("IgnoreUpperCaseWords", true, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSpellingSettingsIgnoreMixedCaseWords"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public virtual bool IgnoreMixedCaseWords {
			get { return GetBoolProperty("IgnoreMixedCaseWords", true); }
			set {
				if(IgnoreMixedCaseWords != value)
					SetBoolProperty("IgnoreMixedCaseWords", true, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSpellingSettingsIgnoreEmails"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public virtual bool IgnoreEmails {
			get { return GetBoolProperty("IgnoreEmails", true); }
			set {
				if(IgnoreEmails != value)
					SetBoolProperty("IgnoreEmails", true, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSpellingSettingsIgnoreUrls"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public virtual bool IgnoreUrls {
			get { return GetBoolProperty("IgnoreUrls", true); }
			set {
				if(IgnoreUrls != value)
					SetBoolProperty("IgnoreUrls", true, value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerSpellingSettingsIgnoreMarkupTags"),
#endif
		DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool IgnoreMarkupTags {
			get { return GetBoolProperty("IgnoreTagContent", false); }
			set {
				if(IgnoreMarkupTags != value)
					SetBoolProperty("IgnoreTagContent", false, value);
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxSpellCheckerSpellingSettings src = source as ASPxSpellCheckerSpellingSettings;
				if (src != null) {
					IgnoreWordsWithNumber = src.IgnoreWordsWithNumber;
					IgnoreUpperCaseWords = src.IgnoreUpperCaseWords;
					IgnoreMixedCaseWords = src.IgnoreMixedCaseWords;
					IgnoreEmails = src.IgnoreEmails;
					IgnoreUrls = src.IgnoreUrls;
					IgnoreMarkupTags = src.IgnoreMarkupTags;
				}
			} finally {
				EndUpdate();
			}
		}
	}
	public class ASPxSpellCheckerTextSettings : ASPxSpellCheckerBaseSettings {
		public ASPxSpellCheckerTextSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerTextSettingsFinishSpellChecking"),
#endif
		DefaultValue(StringResources.SpellChecker_FinishSpellChecking), NotifyParentProperty(true), Localizable(true)]
		public string FinishSpellChecking {
			get { return GetStringProperty("FinishSpellChecking", ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.FinishSpellChecking)); }
			set {
				if(value == FinishSpellChecking) return;
				SetStringProperty("FinishSpellChecking", ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.FinishSpellChecking), value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerTextSettingsSpellCheckFormCaption"),
#endif
		DefaultValue(StringResources.SpellChecker_SpellCheckFormCaption), NotifyParentProperty(true), Localizable(true)]
		public string SpellCheckFormCaption {
			get { return GetStringProperty("SpellCheckFormCaption", ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.SpellCheckFormCaption)); }
			set {
				if(value == SpellCheckFormCaption) return;
				SetStringProperty("SpellCheckFormCaption", ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.SpellCheckFormCaption), value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerTextSettingsOptionsFormCaption"),
#endif
		DefaultValue(StringResources.SpellChecker_OptionsFormCaption), NotifyParentProperty(true), Localizable(true)]
		public string OptionsFormCaption {
			get { return GetStringProperty("OptionsFormCaption", ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.OptionsFormCaption)); }
			set {
				if(value == OptionsFormCaption) return;
				SetStringProperty("OptionsFormCaption", ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.OptionsFormCaption), value);
			}
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("ASPxSpellCheckerTextSettingsNoSuggestionsText"),
#endif
		DefaultValue(StringResources.SpellChecker_NoSuggestions), NotifyParentProperty(true), Localizable(true)]
		public string NoSuggestionsText {
			get { return GetStringProperty("NoSuggestionsText", ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.NoSuggestionsText)); }
			set {
				if(value == OptionsFormCaption) return;
				SetStringProperty("NoSuggestionsText", ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.NoSuggestionsText), value);
			}
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				ASPxSpellCheckerTextSettings src = source as ASPxSpellCheckerTextSettings;
				if (src != null) {
					FinishSpellChecking = src.FinishSpellChecking;
					NoSuggestionsText = src.NoSuggestionsText;
					OptionsFormCaption = src.OptionsFormCaption;
					SpellCheckFormCaption = src.SpellCheckFormCaption;
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal string GetFinishSpellChecking() {
			if(!string.IsNullOrEmpty(FinishSpellChecking)) return FinishSpellChecking;
			return ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.FinishSpellChecking);
		}
		protected internal string GetSpellCheckFormCaption() {
			if(!string.IsNullOrEmpty(SpellCheckFormCaption)) return SpellCheckFormCaption;
			return ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.SpellCheckFormCaption);
		}
		protected internal string GetOptionsFormCaption() {
			if(!string.IsNullOrEmpty(OptionsFormCaption)) return OptionsFormCaption;
			return ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.OptionsFormCaption);
		}
		protected internal string GetNoSuggestionsText() {
			if(!string.IsNullOrEmpty(NoSuggestionsText)) return NoSuggestionsText;
			return ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.NoSuggestionsText);
		}
	}
	public class SpellCheckerFormsSettings : ASPxSpellCheckerBaseSettings {
		public SpellCheckerFormsSettings(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("SpellCheckerFormsSettingsSpellCheckFormPath"),
#endif
DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpellCheckFormPath
		{
			get { return GetStringProperty("SpellCheckFormPath", ""); }
			set { SetStringProperty("SpellCheckFormPath", "", value); }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("SpellCheckerFormsSettingsSpellCheckOptionsFormPath"),
#endif
DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		UrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string SpellCheckOptionsFormPath
		{
			get { return GetStringProperty("SpellCheckOptionsFormPath", ""); }
			set { SetStringProperty("SpellCheckOptionsFormPath", "", value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				SpellCheckerFormsSettings src = source as SpellCheckerFormsSettings;
				if(src != null) {
					SpellCheckFormPath = src.SpellCheckFormPath;
					SpellCheckOptionsFormPath = src.SpellCheckOptionsFormPath;
				}
			}
			finally {
				EndUpdate();
			}
		}
		protected internal string GetFormPath(string formName) {
			return GetStringProperty(string.Format("{0}Path", formName), "");
		}
		protected internal void SetFormPath(string formName, string value) {
			SetStringProperty(string.Format("{0}Path", formName), "", value);
		}
	}
	public class SpellCheckerDialogSettings: ASPxSpellCheckerBaseSettings {
		public SpellCheckerDialogSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("SpellCheckerDialogSettingsShowOptionsButton"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowOptionsButton
		{
			get { return GetBoolProperty("ShowOptionsButton", true); }
			set { SetBoolProperty("ShowOptionsButton", true, value); }
		}
		[
#if !SL
	DevExpressWebASPxSpellCheckerLocalizedDescription("SpellCheckerDialogSettingsShowAddToDictionaryButton"),
#endif
		DefaultValue(true), NotifyParentProperty(true)]
		public bool ShowAddToDictionaryButton
		{
			get { return GetBoolProperty("ShowAddToDictionaryButton", true); }
			set { SetBoolProperty("ShowAddToDictionaryButton", true, value); }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				SpellCheckerDialogSettings settings = source as SpellCheckerDialogSettings;
				if(settings != null) {
					ShowAddToDictionaryButton = settings.ShowAddToDictionaryButton;
					ShowOptionsButton = settings.ShowOptionsButton;
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
}
