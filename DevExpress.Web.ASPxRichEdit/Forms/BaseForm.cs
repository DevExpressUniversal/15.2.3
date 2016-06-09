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
using System.ComponentModel;
using System.Web.UI;
using DevExpress.Web.ASPxRichEdit.Localization;
using DevExpress.Web.Internal;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	[ToolboxItem(false)]
	public class RichEditDialogFormLayout : DialogFormLayoutBase {
		public override string GetClientInstanceName(string name) {
			return "dxreDialog_" + name;
		}
		public override string GetControlCssPrefix() {
			return "dxre-";
		}
		public override string GetLocalizedText(Enum value) {
			return ASPxRichEditLocalizer.GetString((ASPxRichEditStringId)value);
		}
	}
	[ToolboxItem(false)]
	public abstract class RichEditDialogBase : RichEditDialogUserControl {
		protected const string SubmitButtonId = "SubmitButton";
		protected ASPxPanel MainPanel { get; private set; }
		protected ASPxPanel FormLayoutWrapper { get; private set; }
		protected DialogFormLayoutBase MainFormLayout { get; private set; }
		protected List<ASPxButton> ButtonsList = new List<ASPxButton>();
		protected List<ASPxEditBase> Editors = new List<ASPxEditBase>();
		protected string GetClientInstanceName(string name) {
			return "dxreDialog_" + name;
		}
		protected override ASPxEditBase[] GetChildDxEdits() {
			PopulateEditList(Editors);
			return Editors.ToArray();
		}
		protected virtual void PopulateEditList(List<ASPxEditBase> list) {
		}
		protected override ASPxButton[] GetChildDxButtons() {
			PopulateButtonList(ButtonsList);
			return ButtonsList.ToArray();
		}
		protected virtual void PopulateButtonList(List<ASPxButton> list) {
		}
		protected override void CreateChildControls() {
			base.CreateChildControls();
			MainPanel = new ASPxPanel();
			MainPanel.ID = "dxpMainPanel";
			Controls.Add(MainPanel);
			FormLayoutWrapper = new ASPxPanel();
			MainPanel.Controls.Add(FormLayoutWrapper);
			MainFormLayout = new RichEditDialogFormLayout();
			FormLayoutWrapper.Controls.Add(MainFormLayout);
			LayoutGroup group = MainFormLayout.Items.CreateGroup();
			group.CssClass = "dxre-dialogContentGroup";
			group.GroupBoxDecoration = GroupBoxDecoration.None;
			PopulateContentGroup(group);
			group.GroupBoxStyle.CssClass = "dxre-dialogFirstLGB";
			List<Control> bottomControls = new List<Control>();
			PopulateBottomItemControls(bottomControls);
			MainFormLayout.Items.InsertTableView(controls: bottomControls.ToArray()).CssClass = "dxre-dialogFooter";
		}
		protected override void PrepareChildControls() {
			base.PrepareChildControls();
			MainPanel.EnableClientSideAPI = true;
			MainPanel.ClientInstanceName = GetClientInstanceName("MainPanel");
			FormLayoutWrapper.DefaultButton = SubmitButtonId;
			FormLayoutWrapper.CssClass = "dxre-dialogWrapperPanel";
			MainFormLayout.ClientInstanceName = GetClientInstanceName("FormLayout");
			RenderUtils.AppendDefaultDXClassName(MainFormLayout, GetDialogCssClassName());
			MainFormLayout.ApplyCommonSettings();
			Localize();
		}
		protected abstract void PopulateContentGroup(LayoutGroup group);
		protected abstract string GetDialogCssClassName();
		protected virtual void PopulateBottomItemControls(List<Control> controls) {
			ASPxButton submitButton = CreateDialogButton("BtnOk", ASPxRichEditStringId.OkButton);
			submitButton.ID = SubmitButtonId;
			ASPxButton cancelButton = CreateDialogButton("BtnCancel", ASPxRichEditStringId.CancelButton);
			controls.Add(submitButton);
			controls.Add(cancelButton);
		}
		protected virtual void Localize() {
		}
		protected ASPxButton CreateDialogButton(string name, ASPxRichEditStringId stringId) {
			ASPxButton result = new ASPxButton();
			result.CssClass = "dxreDlgFooterBtn";
			result.AutoPostBack = false;
			if(stringId != ASPxRichEditStringId.none)
				result.Text = ASPxRichEditLocalizer.GetString(stringId);
			result.ClientInstanceName = GetClientInstanceName(name);
			ButtonsList.Add(result);
			return result;
		}
	}
	public static class RichEditDialogsHelper {
		public static void LocalizeField(this ASPxPageControl control, string fieldName, ASPxRichEditStringId stringId) {
			control.TabPages.FindByName(fieldName).Text = ASPxRichEditLocalizer.GetString(stringId);
		}
		public static void SetupDefaultSettings(this ValidationSettings validationSettings) {
			validationSettings.RequiredField.IsRequired = true;
			validationSettings.ValidateOnLeave = false;
			validationSettings.Display = Display.Dynamic;
			validationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
			validationSettings.ErrorTextPosition = ErrorTextPosition.Right;
			validationSettings.RequiredField.ErrorText = ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.RequiredFieldError);
		}
		public static EmptyLayoutItem AddEmptyLine(this LayoutItemCollection itemCollection) {
			var emptyItem = itemCollection.Add<EmptyLayoutItem>("");
			emptyItem.ColSpan = ((LayoutGroup)itemCollection.Owner).ColCount;
			emptyItem.CssClass = "dxre-EmptyLine";
			return emptyItem;
		}
		public static void SetupDefaultSettings(this ASPxSpinEdit spinEdit, decimal minValue, decimal maxValue, string displayFormatString) {
			spinEdit.DecimalPlaces = 2;
			spinEdit.Increment = 0.1m;
			spinEdit.DisplayFormatString = displayFormatString;
			spinEdit.MinValue = minValue;
			spinEdit.MaxValue = maxValue;
		}
	}
}
