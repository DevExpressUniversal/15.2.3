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
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxSpellChecker.Forms {
public partial class SpellCheckForm : SpellCheckerUserControl {
	protected SpellCheckerDialogSettings SettingsDialogFormElemets { get { return SpellChecker.SettingsDialogFormElements; } }
	protected override void PrepareChildControls() {
		PrepareCheckedDiv();
		PrepareChangeToPanel();
		PrepareAddToDictionaryButton();
		Localize();
		base.PrepareChildControls();
	}
	protected override ASPxEditBase[] GetChildDxEdits() {
		return new ASPxEditBase[] { txtChangeTo, SCSuggestionsListBox };
	}
	protected override ASPxButton[] GetChildDxButtons() {
		return new ASPxButton[] {
			btnAddToDictionary,
			btnChange,
			btnChangeAll,
			btnClose,
			btnIgnore,
			btnIgnoreAll,
			btnOptions
		};
	}
	protected void PrepareAddToDictionaryButton() {
		btnAddToDictionary.Enabled = SpellChecker.GetCustomDictionary() != null;
	}
	protected void PrepareCheckedDiv() {
		WebControl checkedDiv = RenderUtils.CreateDiv();
		checkedDiv.ID = "SCCheckedDiv";
		SCCheckedDivPlaceHolder.Controls.Add(checkedDiv);
		CheckedTextContainerStyle style = SpellChecker.GetCheckedTextContainerStyle();
		checkedDiv.Attributes.Add("class", style.CssClass);
		CssStyleCollection styles = style.GetStyleAttributes(Page);
		foreach (string key in styles.Keys)
			checkedDiv.Style.Add(key, styles[key]);
	}
	protected void PrepareChangeToPanel() {
		ClientIDHelper.EnableClientIDGeneration(txtChangeTo);
		ClientIDHelper.EnableClientIDGeneration(SCSuggestionsListBox);
	}
	protected void Localize() {
		btnIgnore.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.IgnoreOnceButton);
		btnIgnoreAll.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.IgnoreAllButton);
		btnAddToDictionary.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.AddToDictionaryButton);
		btnChange.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.ChangeButton);
		btnChangeAll.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.ChangeAllButton);
		btnOptions.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.OptionsButton);
		btnClose.Text = ASPxSpellCheckerLocalizer.GetString(ASPxSpellCheckerStringId.CloseButton);
	}
}
}
