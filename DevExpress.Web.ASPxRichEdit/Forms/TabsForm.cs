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
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class TabsForm : RichEditDialogBase {
		protected ASPxTextBox StopPositionTextBox { get; private set; }
		protected ASPxLabel ToBeClearedListLabel { get; private set; }
		protected ASPxRadioButtonList TabsAlignmentList { get; private set; }
		protected ASPxRadioButtonList TabsLeaderList { get; private set; }
		protected ASPxButton SetButton { get; private set; }
		protected ASPxButton ClearButton { get; private set; }
		protected ASPxButton ClearAllButton { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			group.ColCount = 2;
			StopPositionTextBox = group.Items.CreateTextBox("TxbTabStopPosition", location: LayoutItemCaptionLocation.Top, buffer: Editors);
			ASPxSpinEdit tabStopsSpinEdit = group.Items.CreateEditor<ASPxSpinEdit>("SpnDefaultTabStops", location: LayoutItemCaptionLocation.Top, cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			tabStopsSpinEdit.SetupDefaultSettings(0, 1000, UnitFormatString);
			group.Items.CreateEditor<ASPxListBox>("LbTabStopPosition", showCaption: false, buffer: Editors);
			ToBeClearedListLabel = new ASPxLabel();
			ToBeClearedListLabel.ClientInstanceName = GetClientInstanceName("LblToBeClearedList");
			LayoutItem toBeClearedArea = group.Items.CreateItem("ToBeCleared", ToBeClearedListLabel);
			toBeClearedArea.VerticalAlign = FormLayoutVerticalAlign.Top;
			toBeClearedArea.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
			LayoutGroup aligmentGroup = group.Items.Add<LayoutGroup>("", "Aligment");
			aligmentGroup.ColSpan = 2;
			TabsAlignmentList = aligmentGroup.Items.CreateEditor<ASPxRadioButtonList>("RblTabsAlignment", showCaption: false, buffer: Editors);
			LayoutGroup leaderGroup = group.Items.Add<LayoutGroup>("", "Leader");
			leaderGroup.ColSpan = 2;
			TabsLeaderList = leaderGroup.Items.CreateEditor<ASPxRadioButtonList>("RblTabsLeader", showCaption: false, buffer: Editors);
			SetButton = CreateDialogButton("BtnTabSet", ASPxRichEditStringId.Tabs_Set);
			SetButton.ClientEnabled = false;
			ClearButton = CreateDialogButton("BtnTabClear", ASPxRichEditStringId.Tabs_Clear);
			ClearButton.ClientEnabled = false;
			ClearAllButton = CreateDialogButton("BtnTabClearAll", ASPxRichEditStringId.Tabs_ClearAll);
			LayoutItem buttons = group.Items.CreateItem("", SetButton, ClearButton, ClearAllButton);
			buttons.ColSpan = 2;
			buttons.CssClass = "dxreDlgRight";
		}
		protected override void PopulateButtonList(List<ASPxButton> list) {
			list.Add(SetButton);
			list.Add(ClearButton);
			list.Add(ClearAllButton);
		}
		protected override void PopulateEditList(List<ASPxEditBase> list) {
			list.Add(ToBeClearedListLabel);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			StopPositionTextBox.DisplayFormatString = UnitFormatString;
			TabsAlignmentList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_Left), (int)XtraRichEdit.Model.TabAlignmentType.Left);
			TabsAlignmentList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_Center), (int)XtraRichEdit.Model.TabAlignmentType.Center);
			TabsAlignmentList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_Right), (int)XtraRichEdit.Model.TabAlignmentType.Right);
			TabsAlignmentList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_Decimal), (int)XtraRichEdit.Model.TabAlignmentType.Decimal);
			TabsAlignmentList.RepeatDirection = RepeatDirection.Horizontal;
			TabsAlignmentList.Border.BorderStyle = BorderStyle.None;
			TabsAlignmentList.RepeatColumns = 2;
			TabsAlignmentList.ValueType = typeof(int);
			TabsLeaderList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_None), (int)XtraRichEdit.Model.TabLeaderType.None);
			TabsLeaderList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_Hyphens), (int)XtraRichEdit.Model.TabLeaderType.Hyphens);
			TabsLeaderList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_EqualSign), (int)XtraRichEdit.Model.TabLeaderType.EqualSign);
			TabsLeaderList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_Dots), (int)XtraRichEdit.Model.TabLeaderType.Dots);
			TabsLeaderList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_Underline), (int)XtraRichEdit.Model.TabLeaderType.Underline);
			TabsLeaderList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_MiddleDots), (int)XtraRichEdit.Model.TabLeaderType.MiddleDots);
			TabsLeaderList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Tabs_ThickLine), (int)XtraRichEdit.Model.TabLeaderType.ThickLine);
			TabsLeaderList.RepeatDirection = RepeatDirection.Horizontal;
			TabsLeaderList.Border.BorderStyle = BorderStyle.None;
			TabsLeaderList.RepeatColumns = 3;
			TabsLeaderList.ValueType = typeof(int);
			SetupValidationSettings();
		}
		protected override void Localize() {
			MainFormLayout.LocalizeField("TxbTabStopPosition", ASPxRichEditStringId.Tabs_TabStopPosition);
			MainFormLayout.LocalizeField("SpnDefaultTabStops", ASPxRichEditStringId.Tabs_DefaultTabStops);
			MainFormLayout.LocalizeField("Aligment", ASPxRichEditStringId.Tabs_Alignment);
			MainFormLayout.LocalizeField("Leader", ASPxRichEditStringId.Tabs_Leader);
			MainFormLayout.LocalizeField("ToBeCleared", ASPxRichEditStringId.Tabs_TabStopsToBeCleared);
		}
		protected void SetupValidationSettings() {
			StopPositionTextBox.ValidationSettings.RegularExpression.ValidationExpression = "^-{0,1}\\d+\\.{0,1}\\d*$";
			StopPositionTextBox.ValidationSettings.Display = Display.Dynamic;
			StopPositionTextBox.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
			StopPositionTextBox.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Right;
			StopPositionTextBox.ValidationSettings.SetFocusOnError = true;
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgTabsForm";
		}
	}
}
