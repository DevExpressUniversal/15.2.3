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
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Forms {
	public class ChangeElementPropertiesForm : HtmlEditorDialogWithTemplates, IStyleSettingsOwner {
		HtmlEditorDialogStyleLocalizationSettings localizationSettings;
		public HtmlEditorDialogStyleLocalizationSettings LocalizationSettings {
			get {
				if(localizationSettings == null)
					localizationSettings = CreateLocalizationSettings();
				return localizationSettings;
			}
		}
		protected MediaContentStyleSettingsControl StyleSettingsControl { get; private set; }
		protected ASPxComboBox DirectionComboBox { get; private set; }
		protected ASPxComboBox InputTypeComboBox { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			TabbedLayoutGroup tabbedGroup = group.Items.Add<TabbedLayoutGroup>("", "TabGroup");
			tabbedGroup.Width = Unit.Pixel(300);
			tabbedGroup.Height = Unit.Pixel(400);
			PopulateCommonSettingsGroup(tabbedGroup.Items.CreateGroup("CommonProperties"));
			LayoutGroup styleSettingsGroup = tabbedGroup.Items.CreateGroup("StyleProperties");
			StyleSettingsControl = new MediaContentStyleSettingsControl(this);
			styleSettingsGroup.Items.CreateItem("StyleSettings", StyleSettingsControl).ShowCaption = Utils.DefaultBoolean.False;
		}
		protected override HtmlEditorDialogSettingsBase GetDialogSettings() {
			return HtmlEditor.SettingsDialogs.ChangeElementPropertiesDialog;
		}
		protected override string GetDialogCssClassName() {
			return "dxhe-changeElementPropertiesDialog";
		}
		protected virtual void PopulateCommonSettingsGroup(LayoutGroup group) {
			LayoutGroup commonAttributesGroup = group.Items.CreateGroup("CommonAttributesGroup", true);
			commonAttributesGroup.Items.Add<EmptyLayoutItem>("");
			commonAttributesGroup.Items.CreateTextBox("Id", buffer: Editors);
			commonAttributesGroup.Items.CreateTextBox("Title", buffer: Editors);
			DirectionComboBox = commonAttributesGroup.Items.CreateComboBox("Direction", buffer: Editors);
			LayoutGroup tagSpecificAttributesGroup = group.Items.CreateGroup("TagSpecificItemsGroup", true);
			tagSpecificAttributesGroup.Items.CreateTextBox("Value", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateTextBox("Action", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateTextBox("Method", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateTextBox("Name", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateTextBox("For", buffer: Editors);
			InputTypeComboBox = tagSpecificAttributesGroup.Items.CreateComboBox("InputType", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateSpinEdit("TabIndex", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateTextBox("Accept", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateTextBox("Alt", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateTextBox("Src", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateSpinEdit("MaxLength", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateSpinEdit("Size", buffer: Editors);
			tagSpecificAttributesGroup.Items.CreateCheckBox("Disabled", buffer: Editors, showCaption: true);
			tagSpecificAttributesGroup.Items.CreateCheckBox("Checked", buffer: Editors, showCaption: true);
			tagSpecificAttributesGroup.Items.CreateCheckBox("Readonly", buffer: Editors, showCaption: true);
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			MainFormLayout.ApplyCommonSettings();
			MainFormLayout.LocalizeField("Id", ASPxHtmlEditorStringId.ChangeElementProperties_Id);
			MainFormLayout.LocalizeField("Value", ASPxHtmlEditorStringId.ChangeElementProperties_Value);
			MainFormLayout.LocalizeField("Title", ASPxHtmlEditorStringId.ChangeElementProperties_Title);
			MainFormLayout.LocalizeField("TabIndex", ASPxHtmlEditorStringId.ChangeElementProperties_TabIndex);
			MainFormLayout.LocalizeField("Accept", ASPxHtmlEditorStringId.ChangeElementProperties_Accept);
			MainFormLayout.LocalizeField("Alt", ASPxHtmlEditorStringId.ChangeElementProperties_Alt);
			MainFormLayout.LocalizeField("Src", ASPxHtmlEditorStringId.ChangeElementProperties_Src);
			MainFormLayout.LocalizeField("Direction", ASPxHtmlEditorStringId.ChangeElementProperties_Direction);
			MainFormLayout.LocalizeField("InputType", ASPxHtmlEditorStringId.ChangeElementProperties_InputType);
			MainFormLayout.LocalizeField("Action", ASPxHtmlEditorStringId.ChangeElementProperties_Action);
			MainFormLayout.LocalizeField("Method", ASPxHtmlEditorStringId.ChangeElementProperties_Method);
			MainFormLayout.LocalizeField("Name", ASPxHtmlEditorStringId.ChangeElementProperties_Name);
			MainFormLayout.LocalizeField("For", ASPxHtmlEditorStringId.ChangeElementProperties_For);
			MainFormLayout.LocalizeField("Disabled", ASPxHtmlEditorStringId.ChangeElementProperties_Disabled);
			MainFormLayout.LocalizeField("Checked", ASPxHtmlEditorStringId.ChangeElementProperties_Checked);
			MainFormLayout.LocalizeField("MaxLength", ASPxHtmlEditorStringId.ChangeElementProperties_MaxLength);
			MainFormLayout.LocalizeField("Size", ASPxHtmlEditorStringId.ChangeElementProperties_Size);
			MainFormLayout.LocalizeField("Readonly", ASPxHtmlEditorStringId.ChangeElementProperties_Readonly);
			LayoutGroup commonPropertiesGroup = MainFormLayout.FindItemOrGroupByName("CommonProperties") as LayoutGroup;
			LayoutGroup stylePropertiesGroup = MainFormLayout.FindItemOrGroupByName("StyleProperties") as LayoutGroup;
			MainFormLayout.LocalizeField("CommonProperties", ASPxHtmlEditorStringId.ChangeElementProperties_CommonSettingsTabName);
			MainFormLayout.LocalizeField("StyleProperties", ASPxHtmlEditorStringId.ChangeElementProperties_StyleSettingsTabName);
			commonPropertiesGroup.CssClass = "dxhe-emptyPaddings";
			stylePropertiesGroup.CssClass = "dxhe-emptyPaddings";
			InputTypeComboBox.Items.Add("Button", "button"); 
			InputTypeComboBox.Items.Add("Submit", "submit");
			InputTypeComboBox.Items.Add("Reset", "reset");
			InputTypeComboBox.Items.Add("Radio", "radio");
			InputTypeComboBox.Items.Add("Text", "text");
			InputTypeComboBox.Items.Add("Checkbox", "checkbox");
			InputTypeComboBox.Items.Add("File", "file");
			InputTypeComboBox.Items.Add("Hidden", "hidden");
			InputTypeComboBox.Items.Add("Image", "image");
			InputTypeComboBox.Items.Add("Password", "password");
			DirectionComboBox.Items.Add("Left", "ltr");
			DirectionComboBox.Items.Add("Right", "rtl");
			DirectionComboBox.Items.Add("Auto", "auto");
			LayoutGroup tagSpecificItemsGroup = MainFormLayout.FindItemOrGroupByName("TagSpecificItemsGroup") as LayoutGroup;
			foreach(LayoutItem item in tagSpecificItemsGroup.Items)
				item.ClientVisible = false;
		}
		HtmlEditorDialogStyleLocalizationSettings CreateLocalizationSettings() {
			return new HtmlEditorDialogStyleLocalizationSettings() {
				Margins = ASPxHtmlEditorStringId.InsertFlash_Margins,
				TopMargin = ASPxHtmlEditorStringId.InsertFlash_MarginTop,
				BottomMargin = ASPxHtmlEditorStringId.InsertFlash_MarginBottom,
				LeftMargin = ASPxHtmlEditorStringId.InsertFlash_MarginLeft,
				RightMargin = ASPxHtmlEditorStringId.InsertFlash_MarginRight,
				Border = ASPxHtmlEditorStringId.InsertFlash_Border,
				BorderWidth = ASPxHtmlEditorStringId.InsertFlash_BorderWidth,
				BorderColor = ASPxHtmlEditorStringId.InsertFlash_BorderColor,
				BorderStyle = ASPxHtmlEditorStringId.InsertFlash_BorderStyle,
				CssClassName = ASPxHtmlEditorStringId.InsertFlash_CssClass,
				PixelLabel = ASPxHtmlEditorStringId.InsertFlash_Pixels
			};
		}
		object IStyleSettingsOwner.GetCssClassItemsDataSource() {
			return HtmlEditor.SettingsDialogs.ChangeElementPropertiesDialog.CssClassItems;
		}
		HtmlEditorDialogStyleLocalizationSettings IStyleSettingsOwner.LocalizationSettings { get { return LocalizationSettings; } }
	}
}
