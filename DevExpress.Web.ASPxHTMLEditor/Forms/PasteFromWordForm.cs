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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxHtmlEditor.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxHtmlEditor.Forms {
	public class PasteFromWordForm : HtmlEditorDialogWithTemplates {
		protected WebControl PasteContainer { get; private set; }
		protected ASPxCheckBox StripFontCheckBox { get; private set; }
		protected override void PopulateContentGroup(LayoutGroup group) {
			PasteContainer = RenderUtils.CreateIFrame(HtmlEditor.ClientID + "_dxePasteFromWordContainer");
			PasteContainer.ClientIDMode = ClientIDMode.Static;
			group.Items.CreateItem("PasteContainer", PasteContainer);
		}
		protected override void PopulateBottomItemControls(List<Control> controls) {
			base.PopulateBottomItemControls(controls);
			if(HtmlEditor.SettingsDialogs.PasteFromWordDialog.ShowStripFontFamilySelector) {
				StripFontCheckBox = new ASPxCheckBox();
				controls.Insert(0, StripFontCheckBox);
				Editors.Add(StripFontCheckBox);
			}
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			MainFormLayout.ShowItemCaptionColon = false;
			string title = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.PasteRtf_Instructions);
			RenderUtils.SetAttribute(PasteContainer, "title", title, title);
			RenderUtils.AppendDefaultDXClassName(PasteContainer, "dxheDlgPasteContainer");
			SubmitButton.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ButtonOk);
			LayoutItem pasteContainerItem = MainFormLayout.FindItemOrGroupByName("PasteContainer") as LayoutItem;
			pasteContainerItem.CssClass = "dxheDlgPasteContainerCell";
			pasteContainerItem.Caption = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.PasteRtf_Instructions);
			pasteContainerItem.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
			pasteContainerItem.CaptionCellStyle.Border.BorderStyle = BorderStyle.None;
			if(StripFontCheckBox != null) {
				StripFontCheckBox.ClientInstanceName = GetClientInstanceName("StripFont");
				StripFontCheckBox.Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.PasteRtf_StripFont);
			}
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			return new ASPxEditBase[] { StripFontCheckBox };
		}
		protected override HtmlEditorDialogSettingsBase GetDialogSettings() {
			return HtmlEditor.SettingsDialogs.PasteFromWordDialog;
		}
		protected override string GetDialogCssClassName() {
			return "dxheDlgPasteFromWordForm";
		}
	}
}
