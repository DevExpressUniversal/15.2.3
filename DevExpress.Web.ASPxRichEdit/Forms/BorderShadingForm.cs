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
using System.Drawing;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxRichEdit.Forms {
	public class BorderShadingForm : RichEditDialogBase {
		protected ASPxPageControl PageControl { get; private set; }
		protected DialogFormLayoutBase BordersFormLayout { get; private set; }
		protected DialogFormLayoutBase ShadingFormLayout { get; private set; }
		protected DialogFormLayoutBase PreviewFormLayout { get; private set; }
		protected ASPxComboBox WidthComboBox { get; private set; }
		protected ASPxComboBox StyleComboBox { get; private set; }
		protected ASPxColorEdit BorderColorEdit { get; private set; }
		protected ASPxColorEdit FillColorEdit { get; private set; }
		protected override void CreateChildControls() {
			base.CreateChildControls();
			PreviewFormLayout = CreatePreviewFormLayout();
			MainPanel.Controls.Add(PreviewFormLayout);
		}
		protected override void PopulateContentGroup(LayoutGroup group) {
			PageControl = new ASPxPageControl();
			PageControl.ClientInstanceName = GetClientInstanceName("PageControl");
			group.Items.CreateItem("content", PageControl).ShowCaption = Utils.DefaultBoolean.False;
			BordersFormLayout = CreateBordersFormLayout();
			ShadingFormLayout = CreateShadingFormLayout();
			PageControl.AddTab("", "BordersTab", BordersFormLayout);
			PageControl.AddTab("", "ShadingTab", ShadingFormLayout);
		}
		DialogFormLayoutBase CreateBordersFormLayout() {
			var result = new RichEditDialogFormLayout();
			result.ColCount = 2;
			LayoutGroup presets = result.Items.Add<LayoutGroup>("", "PresetsGroup");
			presets.GroupBoxDecoration = GroupBoxDecoration.None;
			presets.ColCount = 2;
			PopulatePresets(presets);
			LayoutGroup options = result.Items.Add<LayoutGroup>("", "OptionsGroup");
			options.GroupBoxDecoration = GroupBoxDecoration.None;
			StyleComboBox = options.Items.CreateComboBox("CmbBorderStyle", location: LayoutItemCaptionLocation.Top, buffer: Editors, cssClassName: "dxre-dialogEditor");
			BorderColorEdit = options.Items.CreateEditor<ASPxColorEdit>("CeBorderColor", buffer: Editors, location: LayoutItemCaptionLocation.Top, cssClassName: "dxre-dialogEditor");
			WidthComboBox = options.Items.CreateComboBox("CmbWidth", location: LayoutItemCaptionLocation.Top, buffer: Editors, cssClassName: "dxre-dialogEditor");
			result.ApplyCommonSettings();
			return result;
		}
		DialogFormLayoutBase CreateShadingFormLayout() {
			var result = new RichEditDialogFormLayout();
			FillColorEdit = result.Items.CreateEditor<ASPxColorEdit>("CeFillColor", buffer: Editors, location: LayoutItemCaptionLocation.Top, cssClassName: "dxre-dialogLongEditor");
			result.ApplyCommonSettings();
			return result;
		}
		DialogFormLayoutBase CreatePreviewFormLayout() {
			var result = new RichEditDialogFormLayout();
			result.CssClass = "dxreTableBorderPreview";
			LayoutGroup preview = result.Items.Add<LayoutGroup>("", "PreviewGroup");
			preview.GroupBoxDecoration = GroupBoxDecoration.HeadingLine;
			preview.VerticalAlign = FormLayoutVerticalAlign.Middle;
			preview.ColCount = 4;
			preview.Items.CreateLabel(ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.BorderShading_BordersDescription), colSpan: 4);
			preview.Items.AddEmptyLine();
			CreateBorderButton(preview, RichEditIconImages.BorderTop);
			var container = RenderUtils.CreateDiv();
			container.ClientIDMode = System.Web.UI.ClientIDMode.Static;
			container.ID = RichEdit.ClientID + "_borderContainerPreview";
			container.CssClass = "dxreTablePreviewContainer";
			var previewItem = preview.Items.CreateItem("", container);
			previewItem.ShowCaption = DefaultBoolean.False;
			previewItem.Paddings.PaddingLeft = Unit.Pixel(5);
			previewItem.Paddings.PaddingBottom = Unit.Pixel(10);
			previewItem.ColSpan = 3;
			previewItem.RowSpan = 3;
			CreateBorderButton(preview, RichEditIconImages.BorderInsideHorizontal);
			CreateBorderButton(preview, RichEditIconImages.BorderBottom);
			preview.Items.CreateItem("").ShowCaption = DefaultBoolean.False;
			CreateBorderButton(preview, RichEditIconImages.BorderLeft);
			CreateBorderButton(preview, RichEditIconImages.BorderInsideVertical);
			CreateBorderButton(preview, RichEditIconImages.BorderRight);
			result.ApplyCommonSettings();
			return result;
		}
		protected void PopulatePresets(LayoutGroup presets) {
			string[] presetImages = {
							   RichEditIconImages.BordersNone,
							   RichEditIconImages.BordersBox,
							   RichEditIconImages.BordersAll,
							   RichEditIconImages.BordersGrid,
							   RichEditIconImages.BordersCustom
						   };
			ASPxRichEditStringId[] presetName = {
							   ASPxRichEditStringId.BorderShading_BordersNone,
							   ASPxRichEditStringId.BorderShading_BordersBox,
							   ASPxRichEditStringId.BorderShading_BordersAll,
							   ASPxRichEditStringId.BorderShading_BordersGrid,
							   ASPxRichEditStringId.BorderShading_BordersCustom
							};
			for(int index = 0; index < presetImages.Length; index++) {
				ASPxButton button = new ASPxButton();
				button.GroupName = "presetGroup";
				button.Image.CopyFrom(GetItemImageProperty(presetImages[index]));
				button.ClientInstanceName = GetClientInstanceName("BtnPreset") + ASPxRichEditLocalizer.GetString(presetName[index]);
				button.AutoPostBack = false;
				button.ID = "BtnPreset " + index.ToString();
				ASPxLabel lable = new ASPxLabel();
				lable.Text = ASPxRichEditLocalizer.GetString(presetName[index]);
				presets.Items.CreateItem("", button).ShowCaption = DefaultBoolean.False;
				presets.Items.CreateItem("", lable).ShowCaption = DefaultBoolean.False;
			}
		}
		protected void CreateBorderButton(LayoutGroup layout, string imageName) {
			ASPxButton borderButton = new ASPxButton();
			borderButton.Image.CopyFrom(GetItemImageProperty(imageName));
			borderButton.ClientInstanceName = GetClientInstanceName("Btn" + imageName);
			borderButton.AutoPostBack = false;
			borderButton.CssClass = "dxreBorderButton";
			borderButton.GroupName = imageName;
			var buttonItem = layout.Items.CreateItem("", borderButton);
			buttonItem.ShowCaption = DefaultBoolean.False;
			buttonItem.HorizontalAlign = FormLayoutHorizontalAlign.Right;
			buttonItem.VerticalAlign = FormLayoutVerticalAlign.Top;
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			PrepareWidthComboBox(WidthComboBox);
			PrepareLineStyleComboBox(StyleComboBox);
			BorderColorEdit.EnableCustomColors = true;
			BorderColorEdit.Value = Color.Black;
			FillColorEdit.EnableCustomColors = true;
			FillColorEdit.EnableAutomaticColorItem = true;
			FillColorEdit.AutomaticColor = Color.Empty;
			FillColorEdit.AutomaticColorItemValue = "Auto";
		}
		protected void PrepareWidthComboBox(ASPxComboBox width) {
			double[] values = { 0.25, 0.5, 0.75, 1, 2.25, 3, 4.5, 6 };
			width.ValueType = typeof(double);
			width.Value = 0.5;
			width.DisplayFormatString = "{0}pt";
			for(int i = 0; i < values.Length; i++) {
				WidthComboBox.Items.Add(values[i].ToString(), values[i]);
			}
		}
		protected void PrepareLineStyleComboBox(ASPxComboBox style) {
			style.ValueType = typeof(int);
			style.Items.Add(Enum.GetName(typeof(XtraRichEdit.Model.BorderLineStyle), XtraRichEdit.Model.BorderLineStyle.Single), (int)XtraRichEdit.Model.BorderLineStyle.Single);
			style.Items.Add(Enum.GetName(typeof(XtraRichEdit.Model.BorderLineStyle), XtraRichEdit.Model.BorderLineStyle.Dotted), (int)XtraRichEdit.Model.BorderLineStyle.Dotted);
			style.Items.Add(Enum.GetName(typeof(XtraRichEdit.Model.BorderLineStyle), XtraRichEdit.Model.BorderLineStyle.Dashed), (int)XtraRichEdit.Model.BorderLineStyle.Dashed);
			style.Items.Add(Enum.GetName(typeof(XtraRichEdit.Model.BorderLineStyle), XtraRichEdit.Model.BorderLineStyle.Double), (int)XtraRichEdit.Model.BorderLineStyle.Double);
			style.Value = (int)XtraRichEdit.Model.BorderLineStyle.Single;
		}
		protected override void Localize() {
			PageControl.LocalizeField("BordersTab", ASPxRichEditStringId.BorderShading_Borders);
			PageControl.LocalizeField("ShadingTab", ASPxRichEditStringId.BorderShading_Shading);
			PreviewFormLayout.LocalizeField("PreviewGroup", ASPxRichEditStringId.Preview);
			BordersFormLayout.LocalizeField("CmbBorderStyle", ASPxRichEditStringId.Style);
			BordersFormLayout.LocalizeField("CeBorderColor", ASPxRichEditStringId.Color);
			BordersFormLayout.LocalizeField("CmbWidth", ASPxRichEditStringId.Width);
			ShadingFormLayout.LocalizeField("CeFillColor", ASPxRichEditStringId.BorderShading_Fill);
		}
		protected ItemImagePropertiesBase GetItemImageProperty(string imageName) {
			var imageProperties = new ItemImageProperties();
			if(!string.IsNullOrEmpty(imageName))
				imageProperties.CopyFrom(RichEdit.Images.GetImageProperties(RichEdit.Page, imageName));
			return imageProperties;
		}
		protected override string GetDialogCssClassName() {
			return "dxreDlgBorderShadingForm";
		}
	}
}
