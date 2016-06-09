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
using System.Drawing.Printing;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class PageSetupForm : RichEditDialogBase {
		protected ASPxPageControl PageControl { get; private set; }
		protected DialogFormLayoutBase MarginsFormLayout { get; private set; }
		protected DialogFormLayoutBase PaperFormLayout { get; private set; }
		protected DialogFormLayoutBase LayoutFormLayout { get; private set; }
		protected ASPxRadioButtonList OrientationRadioButtonList { get; private set; }
		protected ASPxComboBox ApplyToComboBox { get; private set; }
		protected ASPxComboBox PaperSizeComboBox { get; private set; }
		protected ASPxComboBox SectionComboBox { get; private set; }
		protected ASPxCheckBox OddEvenCheckBox { get; private set; }
		protected ASPxCheckBox FirstPageCheckBox { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			PageControl = new ASPxPageControl();
			PageControl.ClientInstanceName = GetClientInstanceName("PageSetupPageControl");
			group.Items.CreateItem("content", PageControl).ShowCaption = Utils.DefaultBoolean.False;
			MarginsFormLayout = CreateMarginsFormLayout();
			PaperFormLayout = CreatePaperFormLayout();
			LayoutFormLayout = CreateLayoutFormLayout();
			PageControl.AddTab("", "0", MarginsFormLayout);
			PageControl.AddTab("", "1", PaperFormLayout);
			PageControl.AddTab("", "2", LayoutFormLayout);
			group.Items.AddEmptyLine();
			ApplyToComboBox = group.Items.CreateComboBox("CbxApplyTo", buffer: Editors, showCaption: true, cssClassName: "dxre-dialogLongEditor");
		}
		DialogFormLayoutBase CreateMarginsFormLayout() {
			var result = new RichEditDialogFormLayout();
			result.CssClass = "dxre-dialogLayoutWhithPaddings";
			result.ColCount = 2;
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Margins), buffer: Editors, colSpan: 2);
			ASPxSpinEdit topSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnTop", buffer: Editors, cssClassName: "dxre-dialogEditor");
			topSpinEdit.SetupDefaultSettings(-22, 22, UnitFormatString);
			ASPxSpinEdit bottomSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnBottom", buffer: Editors, cssClassName: "dxre-dialogEditor");
			bottomSpinEdit.SetupDefaultSettings(-22, 22, UnitFormatString);
			ASPxSpinEdit leftSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnLeft", buffer: Editors, cssClassName: "dxre-dialogEditor");
			leftSpinEdit.SetupDefaultSettings(0, 22, UnitFormatString);
			ASPxSpinEdit rightSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnRight", buffer: Editors, cssClassName: "dxre-dialogEditor");
			rightSpinEdit.SetupDefaultSettings(0, 22, UnitFormatString);
			result.Items.AddEmptyLine();
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Orientation), buffer: Editors, colSpan: 2);
			OrientationRadioButtonList = result.Items.CreateEditor<ASPxRadioButtonList>(buffer: Editors, showCaption: false, colSpan: 2);
			result.ApplyCommonSettings();
			return result;
		}
		DialogFormLayoutBase CreatePaperFormLayout() {
			var result = new RichEditDialogFormLayout();
			result.CssClass = "dxre-dialogLayoutWhithPaddings";
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.PaperSize), buffer: Editors);
			PaperSizeComboBox = result.Items.CreateComboBox("CbxPaperSize", buffer: Editors, showCaption: false, cssClassName: "dxre-dialogLongEditor");
			ASPxSpinEdit widthSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnWidth", buffer: Editors, cssClassName: "dxre-dialogShortEditor");
			widthSpinEdit.SetupDefaultSettings(0.1m, 22, UnitFormatString);
			ASPxSpinEdit heightSpinEdit = result.Items.CreateEditor<ASPxSpinEdit>("SpnHeight", buffer: Editors, cssClassName: "dxre-dialogShortEditor");
			heightSpinEdit.SetupDefaultSettings(0.1m, 22, UnitFormatString);
			result.ApplyCommonSettings();
			return result;
		}
		DialogFormLayoutBase CreateLayoutFormLayout() {
			var result = new RichEditDialogFormLayout();
			result.CssClass = "dxre-dialogLayoutWhithPaddings";
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Section), buffer: Editors);
			SectionComboBox = result.Items.CreateComboBox("CbxSectionStart", buffer: Editors, cssClassName: "dxre-dialogLongEditor");
			result.Items.AddEmptyLine();
			result.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.HeadersAndFooters), buffer: Editors);
			OddEvenCheckBox = result.Items.CreateCheckBox("ChkDifferentOddAndEven", buffer: Editors);
			FirstPageCheckBox = result.Items.CreateCheckBox("ChkDifferentFirstPage", buffer: Editors);
			result.ApplyCommonSettings();
			return result;
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			OrientationRadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Portrait), 0);
			OrientationRadioButtonList.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Landscape), 1);
			OrientationRadioButtonList.ValueType = typeof(int);
			OrientationRadioButtonList.SelectedIndex = 0;
			OrientationRadioButtonList.Border.BorderWidth = 0;
			OrientationRadioButtonList.ClientInstanceName = GetClientInstanceName("RblOrientation");
			var paperKindNames = Enum.GetNames(typeof(PaperKind));
			foreach(var name in paperKindNames) {
				var kind = (PaperKind)Enum.Parse(typeof(PaperKind), name);
				PaperSizeComboBox.Items.Add(RichEditLocalization.GetPaperKindDisplayName(kind), (int)kind);
			}
			PaperSizeComboBox.ValueType = typeof(int);
			SectionComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Continuous), (int)XtraRichEdit.Model.SectionStartType.Continuous);
			SectionComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.NewPage), (int)XtraRichEdit.Model.SectionStartType.NextPage);
			SectionComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.EvenPage), (int)XtraRichEdit.Model.SectionStartType.EvenPage);
			SectionComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.OddPage), (int)XtraRichEdit.Model.SectionStartType.OddPage);
			SectionComboBox.ValueType = typeof(int);
			ApplyToComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.WholeDocument), (int)XtraRichEdit.Forms.SectionPropertiesApplyType.WholeDocument);
			ApplyToComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CurrentSection), (int)XtraRichEdit.Forms.SectionPropertiesApplyType.CurrentSection);
			ApplyToComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.SelectedSections), (int)XtraRichEdit.Forms.SectionPropertiesApplyType.SelectedSections);
			ApplyToComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ThisPointForward), (int)XtraRichEdit.Forms.SectionPropertiesApplyType.ThisPointForward);
			ApplyToComboBox.ValueType = typeof(int);
		}
		protected override void Localize() {
			PageControl.LocalizeField("0", ASPxRichEditStringId.Margins);
			PageControl.LocalizeField("1", ASPxRichEditStringId.Paper);
			PageControl.LocalizeField("2", ASPxRichEditStringId.Layout);
			MarginsFormLayout.LocalizeField("SpnTop", ASPxRichEditStringId.Top);
			MarginsFormLayout.LocalizeField("SpnBottom", ASPxRichEditStringId.Bottom);
			MarginsFormLayout.LocalizeField("SpnLeft", ASPxRichEditStringId.Left);
			MarginsFormLayout.LocalizeField("SpnRight", ASPxRichEditStringId.Right);
			PaperFormLayout.LocalizeField("SpnWidth", ASPxRichEditStringId.Width);
			PaperFormLayout.LocalizeField("SpnHeight", ASPxRichEditStringId.Height);
			LayoutFormLayout.LocalizeField("CbxSectionStart", ASPxRichEditStringId.SectionStart);
			OddEvenCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.DifferentOddAndEven);
			FirstPageCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.DifferentFirstPage);
			MainFormLayout.LocalizeField("CbxApplyTo", ASPxRichEditStringId.ApplyTo);
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgPageSetupForm";
		}
	}
}
