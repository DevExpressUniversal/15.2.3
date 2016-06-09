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
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class ColumnsForm : RichEditDialogBase {
		protected ASPxRoundPanel PresetsPanel { get; private set; }
		protected ASPxRoundPanel WidthSpacingPanel { get; private set; }
		protected DialogFormLayoutBase WidthSpacingLayout { get; private set; }
		protected ASPxCheckBox EqualWidthCheckBox { get; private set; }
		protected ASPxComboBox ApplyToComboBox { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			PresetsPanel = CreatePresets();
			LayoutItem presets = group.Items.CreateItem("Presets", PresetsPanel);
			presets.ShowCaption = DefaultBoolean.False;
			group.Items.AddEmptyLine();
			ASPxSpinEdit numberOfColumns = group.Items.CreateEditor<ASPxSpinEdit>("SpnNumberOfColumns", cssClassName: "dxre-dialogShortEditor", buffer: Editors);
			numberOfColumns.Increment = numberOfColumns.MinValue = 1;
			numberOfColumns.MaxValue = 100;
			group.Items.AddEmptyLine();
			WidthSpacingPanel = new ASPxRoundPanel();
			LayoutItem widthSpacing = group.Items.CreateItem("WidthSpacing", WidthSpacingPanel);
			widthSpacing.ShowCaption = DefaultBoolean.False;
			WidthSpacingLayout = new RichEditDialogFormLayout();
			WidthSpacingLayout.ClientInstanceName = GetClientInstanceName("WidthSpacingLayout");
			WidthSpacingLayout.Items.Add<EmptyLayoutItem>("", "ColumnsEditor").CssClass = "dxreDlgColumnsEditor";
			WidthSpacingLayout.Items.Add<EmptyLayoutItem>("").CssClass = "dxre-Separator";
			EqualWidthCheckBox = WidthSpacingLayout.Items.CreateCheckBox("ChkEqualWidth", buffer: Editors);
			WidthSpacingPanel.Controls.Add(WidthSpacingLayout);
			group.Items.AddEmptyLine();
			ApplyToComboBox = group.Items.CreateComboBox("CbxApplyTo", buffer: Editors);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			PresetsPanel.CornerRadius = WidthSpacingPanel.CornerRadius = 0;
			PresetsPanel.Width = WidthSpacingPanel.Width = WidthSpacingLayout.Width = Unit.Percentage(100);
			ApplyToComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.WholeDocument), (int)XtraRichEdit.Forms.SectionPropertiesApplyType.WholeDocument);
			ApplyToComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.CurrentSection), (int)XtraRichEdit.Forms.SectionPropertiesApplyType.CurrentSection);
			ApplyToComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.SelectedSections), (int)XtraRichEdit.Forms.SectionPropertiesApplyType.SelectedSections);
			ApplyToComboBox.Items.Add(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.ThisPointForward), (int)XtraRichEdit.Forms.SectionPropertiesApplyType.ThisPointForward);
			ApplyToComboBox.ValueType = typeof(int);
		}
		protected override void Localize() {
			MainFormLayout.LocalizeField("SpnNumberOfColumns", ASPxRichEditStringId.Columns_NumberOfColumns);
			MainFormLayout.LocalizeField("CbxApplyTo", ASPxRichEditStringId.ApplyTo);
			EqualWidthCheckBox.Text = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Columns_EqualWidth);
			PresetsPanel.HeaderText = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Columns_Presets);
			WidthSpacingPanel.HeaderText = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.Columns_WidthAndSpacing);
		}
		protected ASPxRoundPanel CreatePresets() {
			ASPxRoundPanel result = new ASPxRoundPanel();
			string[] presetImages = {
							   RichEditIconImages.OneColumn,
							   RichEditIconImages.TwoColumns,
							   RichEditIconImages.ThreeColumns,
							   RichEditIconImages.LeftColumns,
							   RichEditIconImages.RightColumns
						   };
			WebControl table = RenderUtils.CreateTable();
			table.CssClass = "dxreDlgTableWidth";
			WebControl row = RenderUtils.CreateTableRow();
			table.Controls.Add(row);
			for(int index = 0; index < presetImages.Length; index++) {
				WebControl cell = RenderUtils.CreateTableCell();
				cell.CssClass = "dxreDlgCenter";
				row.Controls.Add(cell);
				ASPxButton button = new ASPxButton();
				button.GroupName = "presetGroup";
				button.Image.CopyFrom(GetItemImageProperty(presetImages[index]));
				button.ClientInstanceName = GetClientInstanceName("BtnPreset") + index.ToString();
				button.AutoPostBack = false;
				button.ID = "BtnPreset " + index.ToString();
				cell.Controls.Add(button);
			}
			result.Controls.Add(table);
			return result;
		}
		protected ItemImagePropertiesBase GetItemImageProperty(string imageName) {
			var imageProperties = new ItemImageProperties();
			if(!string.IsNullOrEmpty(imageName))
				imageProperties.CopyFrom(RichEdit.Images.GetImageProperties(RichEdit.Page, imageName));
			return imageProperties;
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgColumnsForm";
		}
	}
}
