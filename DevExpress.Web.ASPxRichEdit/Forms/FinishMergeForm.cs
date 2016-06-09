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
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public enum MailMergeExportRange {
		AllRecord,
		CurrentRecord,
		Range
	}
	public class FinishMergeForm : RichEditDialogBase {
		protected ASPxRadioButtonList RadioButtonList { get; private set; }
		protected ASPxComboBox MergeModeComboBox { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			RadioButtonList = group.Items.CreateEditor<ASPxRadioButtonList>(buffer: Editors, showCaption: false);
			LayoutGroup rangeGroup = group.Items.Add<LayoutGroup>("", "RangeGroup");
			rangeGroup.Items.CreateSpinEdit(name: "SpnFrom", showButtons: true, buffer: Editors).MinValue = 1;
			rangeGroup.Items.CreateSpinEdit(name: "SpnCount", showButtons: true, buffer: Editors).MinValue = 1;
			MergeModeComboBox = group.Items.CreateComboBox(name: "CmbMergeMode", buffer: Editors);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			PrepareRadioButtonList();
			PrepareMergeModeComboBox();
		}
		void PrepareRadioButtonList() {
			RadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.FinishMerge_AllRecord), (int)MailMergeExportRange.AllRecord);
			RadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.FinishMerge_CurrentRecord), (int)MailMergeExportRange.CurrentRecord);
			RadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.FinishMerge_Range), (int)MailMergeExportRange.Range);
			RadioButtonList.ClientInstanceName = GetClientInstanceName("RdBttnListSwitcher");
			RadioButtonList.SelectedIndex = 0;
			RadioButtonList.Border.BorderWidth = 0;
			RadioButtonList.ValueType = typeof(Int32);
		}
		void PrepareMergeModeComboBox() {
			MergeModeComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.FinishMerge_NewParagraph), (int)XtraRichEdit.API.Native.MergeMode.NewParagraph);
			MergeModeComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.FinishMerge_NewSection), (int)XtraRichEdit.API.Native.MergeMode.NewSection);
			MergeModeComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.FinishMerge_JoinTables), (int)XtraRichEdit.API.Native.MergeMode.JoinTables);
			MergeModeComboBox.ValueType = typeof(int);
			MergeModeComboBox.SelectedIndex = 0;
		}
		protected override void Localize() {
			MainFormLayout.LocalizeField("SpnFrom", ASPxRichEditStringId.FinishMerge_From);
			MainFormLayout.LocalizeField("SpnCount", ASPxRichEditStringId.FinishMerge_Count);
			MainFormLayout.LocalizeField("CmbMergeMode", ASPxRichEditStringId.FinishMerge_MergeMode);
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgFinishMergeForm";
		}
	}
}
