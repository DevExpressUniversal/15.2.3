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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using DevExpress.Web.ASPxSpellChecker;
using DevExpress.Web;
using DevExpress.Web.ASPxSpellChecker.Localization;
namespace DevExpress.Web.ASPxSpellChecker.Forms {
public partial class SpellCheckOptionsForm : SpellCheckerUserControl {
	protected override void PrepareChildControls() {
		PrepareFormCheckBoxes();
		PrepareLanguagesCombo();
		Localize();
		base.PrepareChildControls();
	}
	protected override ASPxEditBase[] GetChildDxEdits() {
		return new ASPxEditBase[] { chkbEmails, 
									chkbMixedCase, 
									chkbNumbers, 
									chkbUpperCase, 
									chkbUrls, 
									comboLanguage, 
									chkbTags 
		};
	}
	protected override ASPxButton[] GetChildDxButtons() {
		return new ASPxButton[] {
			btnCancel,
			btnOK
		};
	}
	protected void PrepareFormCheckBoxes() {
		ASPxSpellChecker spellChecker = ((ASPxSpellChecker)NamingContainer);
		chkbEmails.Checked = spellChecker.SettingsSpelling.IgnoreEmails;
		chkbMixedCase.Checked = spellChecker.SettingsSpelling.IgnoreMixedCaseWords;
		chkbNumbers.Checked = spellChecker.SettingsSpelling.IgnoreWordsWithNumber;
		chkbUpperCase.Checked = spellChecker.SettingsSpelling.IgnoreUpperCaseWords;
		chkbUrls.Checked = spellChecker.SettingsSpelling.IgnoreUrls;
		chkbTags.Checked = spellChecker.SettingsSpelling.IgnoreMarkupTags;
	}
	protected void PrepareLanguagesCombo() {
		comboLanguage.Items.Clear();
		for (int i = 0; i < SpellChecker.Dictionaries.Count; i++) {
			if (comboLanguage.Items.FindByText(SpellChecker.Dictionaries[i].GetCulture().DisplayName) == null)
				comboLanguage.Items.Add(SpellChecker.Dictionaries[i].GetCulture().DisplayName);
		}
		comboLanguage.SelectedItem = comboLanguage.Items.FindByText(SpellChecker.GetCulture().DisplayName);
	}
	protected void Localize() {
		pnlOptions.HeaderText = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.GeneralOptions);
		chkbUpperCase.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.IgnoreUppercase);
		chkbMixedCase.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.IgnoreMixedCase);
		chkbNumbers.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.IgnoreNumbers);
		chkbEmails.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.IgnoreEmails);
		chkbUrls.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.IgnoreUrls);
		chkbTags.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.IgnoreTags);
		pnlLanguageSelection.HeaderText = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.InternationalDictionaries);
		lblChooseDictionary.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.ChooseDictionary);
		lblLanguage.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.Language);
		btnOK.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.OkButton);
		btnCancel.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.CancelButton);
	}
}
}
