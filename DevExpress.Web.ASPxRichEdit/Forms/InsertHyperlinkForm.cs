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

using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class InsertHyperlinkForm : RichEditDialogBase {
		protected RichEditInsertLinkDialogSettings SettingsInsertLinkDialog {
			get { return RichEdit.SettingsDialogs.InsertLinkDialog; }
		}
		protected DialogFormLayoutBase ContentLayout { get; private set; }
		protected ASPxRoundPanel ContentPanel { get; private set; }
		protected ASPxLabel DisplayPropertiesLable { get; private set; }
		protected ASPxTextBox EmailTextBox { get; private set; }
		protected ASPxTextBox UrlTextBox { get; private set; }
		protected ASPxRadioButtonList LinkTo { get; private set; }
		protected ASPxComboBox CbBookmarkNames { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			LinkTo = group.Items.CreateEditor<ASPxRadioButtonList>("RblLinkTo", buffer: Editors, showCaption: false);
			ContentPanel = new ASPxRoundPanel();
			ContentPanel.ShowHeader = false;
			var content = group.Items.CreateItem("generalContent", ContentPanel);
			content.ShowCaption = DefaultBoolean.False;
			ContentLayout = new RichEditDialogFormLayout();
			ContentLayout.ClientInstanceName = "InsertHyperlinkContent";
			CbBookmarkNames = ContentLayout.Items.CreateEditor<ASPxComboBox>("CbBookmarkNames", buffer: Editors);
			UrlTextBox = ContentLayout.Items.CreateTextBox("TxbURL", buffer: Editors);
			EmailTextBox = ContentLayout.Items.CreateTextBox("TxbEmailTo", buffer: Editors, clientVisible: false);
			ContentLayout.Items.CreateTextBox("TxbSubject", buffer: Editors, clientVisible: false);
			ContentLayout.Items.AddEmptyLine();
			DisplayPropertiesLable = ContentLayout.Items.CreateLabel(buffer: Editors);
			DisplayPropertiesLable.Font.Bold = true;
			ContentLayout.Items.CreateTextBox("TxbText", buffer: Editors);
			ContentLayout.Items.CreateTextBox("TxbToolTip", buffer: Editors);
			ContentLayout.ApplyCommonSettings();
			ContentPanel.Controls.Add(ContentLayout);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			PrepareLinkTo();
			ContentPanel.Width = Unit.Percentage(100);
			ContentLayout.Width = Unit.Percentage(100);
			SetupValidationSettings();
		}
		void PrepareLinkTo() {
			LinkTo.RepeatDirection = RepeatDirection.Horizontal;
			LinkTo.Border.BorderStyle = BorderStyle.None;
			LinkTo.EnableFocusedStyle = false;
			LinkTo.ValueType = typeof(int);
			LinkTo.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Hyperlink_WebPage), 0);
			LinkTo.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Hyperlink_PlaceInThisDocument), 1);
			LinkTo.RepeatColumns = 2;
			if(SettingsInsertLinkDialog.ShowEmailAddressSection) {
				LinkTo.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Hyperlink_EmailAddress), 2);
				LinkTo.RepeatColumns = 3;
			}
		}
		protected override void Localize() {
			ContentLayout.LocalizeField("TxbURL", ASPxRichEditStringId.Hyperlink_Url);
			ContentLayout.LocalizeField("TxbEmailTo", ASPxRichEditStringId.Hyperlink_EmailTo);
			ContentLayout.LocalizeField("TxbSubject", ASPxRichEditStringId.Hyperlink_Subject);
			ContentLayout.LocalizeField("TxbText", ASPxRichEditStringId.Hyperlink_Text);
			ContentLayout.LocalizeField("TxbToolTip", ASPxRichEditStringId.Hyperlink_ToolTip);
			ContentLayout.LocalizeField("CbBookmarkNames", ASPxRichEditStringId.Hyperlink_Bookmark);
			DisplayPropertiesLable.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Hyperlink_DisplayProperties);
		}
		protected void SetupValidationSettings() {
			EmailTextBox.ValidationSettings.ValidationGroup = "_dxeTxbEmailToGroup";
			EmailTextBox.ValidationSettings.RegularExpression.ValidationExpression = "\\w+([-+.\']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
			EmailTextBox.ValidationSettings.RegularExpression.ErrorText = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Hyperlink_InvalidEmail);
			EmailTextBox.ValidationSettings.SetupDefaultSettings();
			UrlTextBox.ValidationSettings.SetupDefaultSettings();
			CbBookmarkNames.ValidationSettings.SetupDefaultSettings();
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgHyperlinkForm";
		}
	}
}
