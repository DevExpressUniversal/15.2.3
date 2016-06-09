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

using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class BookmarksForm : RichEditDialogBase {
		protected ASPxListBox BookmarksNameListBox { get; private set; }
		protected ASPxRadioButtonList SortByList { get; private set; }
		protected ASPxButton AddButton { get; private set; }
		protected ASPxButton DeleteButton { get; private set; }
		protected ASPxButton GoToButton { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			group.Items.CreateTextBox("TxbBookmarksName", location: LayoutItemCaptionLocation.Top, buffer: Editors);
			BookmarksNameListBox = group.Items.CreateEditor<ASPxListBox>("LsbBookmarkNames", showCaption: false, buffer: Editors);
			SortByList = group.Items.CreateEditor<ASPxRadioButtonList>("RblSortBy", location: LayoutItemCaptionLocation.Top, buffer: Editors);
			group.Items.AddEmptyLine();
			AddButton = CreateDialogButton("BtnAdd", ASPxRichEditStringId.Bookmarks_Add);
			AddButton.ClientEnabled = false;
			DeleteButton = CreateDialogButton("BtnDelete", ASPxRichEditStringId.Bookmarks_Delete);
			DeleteButton.ClientEnabled = false;
			GoToButton = CreateDialogButton("BtnGoTo", ASPxRichEditStringId.Bookmarks_GoTo);
			GoToButton.ClientEnabled = false;
			LayoutItem buttons = group.Items.CreateItem("", AddButton, DeleteButton, GoToButton);
			buttons.CssClass = "dxreDlgRight";
		}
		protected override void PopulateBottomItemControls(List<Control> controls) {
			ASPxButton closeButton = CreateDialogButton("BtnCancel", ASPxRichEditStringId.CloseButton);
			closeButton.ID = SubmitButtonId;
			controls.Add(closeButton);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			SortByList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Bookmarks_Name), 0);
			SortByList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Bookmarks_Location), 1);
			SortByList.RepeatDirection = RepeatDirection.Horizontal;
			SortByList.Border.BorderStyle = BorderStyle.None;
			SortByList.ValueType = typeof(int);
			SortByList.Value = 0;
		}
		protected override void Localize() {
			MainFormLayout.LocalizeField("TxbBookmarksName", ASPxRichEditStringId.Bookmarks_BookmarkName);
			MainFormLayout.LocalizeField("RblSortBy", ASPxRichEditStringId.Bookmarks_SortBy);
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgBookmarksForm";
		}
	}
}
