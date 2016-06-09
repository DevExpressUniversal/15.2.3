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

using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.Internal;
using DevExpress.Web.FormLayout.Internal.RuntimeHelpers;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Web.Localization;
using DevExpress.Web.ASPxHtmlEditor.Localization;
namespace DevExpress.Web.ASPxHtmlEditor.Forms {
	[ToolboxItem(false)]
	public class HtmlEditorSearchFormLayout : HtmlEditorDialogFormLayout {
		readonly ASPxHtmlEditor HtmlEditor;
		readonly string windowName = null;
		public HtmlEditorSearchFormLayout(ASPxHtmlEditor htmlEditor, string windowName) {
			HtmlEditor = htmlEditor;
			this.windowName = windowName;
		}
		public override string GetClientInstanceName(string name) {
			return HtmlEditor.ClientID + "_" + windowName + "_" + name;
		}
		public override string GetControlCssPrefix() {
			return "dxhe-search";
		}
	}
	[ToolboxItem(false)]
	public abstract class SearchControl : BaseControl {
		readonly string windowName = null;
		protected HtmlEditorSearchFormLayout MainFormLayout { get; private set; }
		public SearchControl(ASPxHtmlEditor htmlEditor, string windowName)
			: base(htmlEditor) {
			this.windowName = windowName;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			MainFormLayout = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			MainFormLayout = new HtmlEditorSearchFormLayout(HtmlEditor, this.windowName);
			Controls.Add(MainFormLayout);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			MainFormLayout.ApplyCommonSettings();
			MainFormLayout.UseDefaultPaddings = false;
			RenderUtils.AppendDefaultDXClassName(MainFormLayout, GetFormCssClassName());
			for(int i = 0; i < MainFormLayout.Items.Count; i++)
				MainFormLayout.Items[i].ParentContainerStyle.CssClass += string.Format(" dxhe-flItem{0}", i);
			HtmlEditorUserControl.PrepareChildDXControls(this.Controls, HtmlEditor);
		}
		string GetFormCssClassName() {
			return MainFormLayout.GetControlCssPrefix() + GetFormCssClassNameSuffix();
		}
		protected abstract string GetFormCssClassNameSuffix();
	}
	[ToolboxItem(false)]
	public class QuickSearchPanelControl : SearchControl {
		public QuickSearchPanelControl(ASPxHtmlEditor htmlEditor, string windowName)
			: base(htmlEditor, windowName) {
		}
		ASPxButton nextButton;
		ASPxButton prevButton;
		ASPxButtonEdit searchButtonEdit;
		protected override string GetFormCssClassNameSuffix() {
			return "Quick";
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.nextButton = null;
			this.prevButton = null;
			this.searchButtonEdit = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.searchButtonEdit = MainFormLayout.Items.CreateEditor<ASPxButtonEdit>("SrchBtnEdit", cssClassName: "dxhe-SearchField", showCaption: false);
			searchButtonEdit.NullText = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.QuickSearch_SearchFieldNullText);
			this.prevButton = new QuickSearchPrevButton();
			MainFormLayout.Items.CreateItem("", this.prevButton);
			this.nextButton = new QuickSearchNextButton();
			MainFormLayout.Items.CreateItem("", this.nextButton);
			MainFormLayout.Items.Add(new EmptyLayoutItem());
			MainFormLayout.Items.Add(new EmptyLayoutItem());
			MainFormLayout.Items.Add(new EmptyLayoutItem());
			MainFormLayout.Items.Add(new EmptyLayoutItem());
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			MainFormLayout.ColCount = MainFormLayout.Items.Count;
			MainFormLayout.SettingsItems.ShowCaption = Utils.DefaultBoolean.False;
			PrepareButton(this.prevButton, "PrevButton");
			PrepareButton(this.nextButton, "NextButton");
			this.searchButtonEdit.Buttons.Add(new ClearButton());
			this.searchButtonEdit.AutoPostBack = false;
			this.searchButtonEdit.ClearButton.DisplayMode = ClearButtonDisplayMode.Always;
		}
		void PrepareButton(ASPxButton button, string name) {
			button.AutoPostBack = false;
			button.ClientInstanceName = MainFormLayout.GetClientInstanceName(name);
		}
	}
	[ToolboxItem(false)]
	public class AdvancedSearchPanelControl : SearchControl {
		LayoutGroup mainGroup;
		ASPxListBox results;
		ASPxButtonEdit searchButtonEdit;
		ASPxTextBox replaceTextBox;
		protected override string GetFormCssClassNameSuffix() {
			return "Advanced";
		}
		public AdvancedSearchPanelControl(ASPxHtmlEditor htmlEditor, string windowName)
			: base(htmlEditor, windowName) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.searchButtonEdit = null;
			this.results = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.mainGroup = MainFormLayout.Items.Add<LayoutGroup>("");
			PopulateMainGroup(this.mainGroup);
			Controls.Add(CreateInfoFrame());
		}
		void PopulateMainGroup(LayoutGroup group) {
			this.searchButtonEdit = group.Items.CreateEditor<ASPxButtonEdit>("SrchBtnEdit", buffer: null);
			this.replaceTextBox = group.Items.CreateTextBox(name: "ReplaceTb", buffer: null);
			group.Items.Add<EmptyLayoutItem>("");
			this.results = group.Items.CreateEditor<ASPxListBox>(name: "ResultList", location: LayoutItemCaptionLocation.Top);
			group.Items.CreateCheckBox(name: "MatchCase", buffer: null).Text = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_MatchCase);
		}
		protected WebControl CreateInfoFrame() {
			WebControl result = RenderUtils.CreateDiv();
			AddLabel(result, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_ReplaceAllNotify));
			AddLabel(result, ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_ReplaceProcessNotify));
			result.CssClass = "dxhe-searchWarning";
			RenderUtils.SetVisibility(result, false, false, true);
			return result;
		}
		void AddLabel(WebControl container, string text) {
			Label result = RenderUtils.CreateLabel();
			result.Text = text;
			container.Controls.Add(result);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			MainFormLayout.SettingsItemCaptions.Location = LayoutItemCaptionLocation.Left;
			this.mainGroup.UseDefaultPaddings = false;
			this.mainGroup.GroupBoxDecoration = GroupBoxDecoration.None;
			this.mainGroup.FindItemOrGroupByName("SrchBtnEdit").Caption = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_Find);
			this.mainGroup.FindItemOrGroupByName("ReplaceTb").Caption = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_ReplaceWith);
			LayoutItem resultItem = (LayoutItem)this.mainGroup.FindItemOrGroupByName("ResultList");
			resultItem.Caption = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_Results);
			resultItem.CaptionCellStyle.CssClass = "dxhe-searchResultInfo";
			this.results.Rows = 6;
			this.results.AutoPostBack = false;
			this.results.EncodeHtml = false;
			this.results.EnableFocusedStyle = false;
			this.results.EnableSynchronization = Utils.DefaultBoolean.False;
			this.results.Width = Unit.Percentage(100);
			this.results.JSProperties["cpNarrowSearchWarning"] = ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_SearchLimit);
			this.replaceTextBox.AutoPostBack = false;
			this.searchButtonEdit.Buttons.Add(new ClearButton());
			this.searchButtonEdit.AutoPostBack = false;
			this.searchButtonEdit.ClearButton.DisplayMode = ClearButtonDisplayMode.Always;
		}
	}
	[ToolboxItem(false)]
	public class AdvancedSearchButtons : SearchControl {
		ASPxButton replaceAllBtn;
		ASPxButton replaceBtn;
		ASPxButton prevBtn;
		ASPxButton nextBtn;
		public AdvancedSearchButtons(ASPxHtmlEditor htmlEditor, string windowName)
			: base(htmlEditor, windowName) {
		}
		protected override string GetFormCssClassNameSuffix() {
			return "AdvancedButtons";
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.replaceAllBtn = null;
			this.replaceBtn = null;
			this.prevBtn = null;
			this.nextBtn = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.replaceAllBtn = new ASPxButton();
			this.replaceBtn = new ASPxButton();
			this.prevBtn = new ASPxButton();
			this.nextBtn = new ASPxButton();
			MainFormLayout.Items.CreateItem("", this.replaceAllBtn);
			MainFormLayout.Items.CreateItem("", this.replaceBtn);
			MainFormLayout.Items.Add<EmptyLayoutItem>("");
			MainFormLayout.Items.CreateItem("", this.prevBtn);
			MainFormLayout.Items.CreateItem("", this.nextBtn);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			MainFormLayout.ColCount = 5;
			MainFormLayout.SettingsItems.ShowCaption = Utils.DefaultBoolean.False;
			MainFormLayout.SettingsItems.Width = Unit.Percentage(20);
			MainFormLayout.Items[2].Width = Unit.Percentage(100);
			MainFormLayout.Items[2].ParentContainerStyle.CssClass = "dxhe-advancedSearchButtonSpacer";
			PrepareButton(this.replaceAllBtn, "ReplaceAllBtn", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_ReplaceAll));
			PrepareButton(this.replaceBtn, "ReplaceBtn", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_Replace));
			PrepareButton(this.prevBtn, "PrevButton", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_Previous));
			PrepareButton(this.nextBtn, "NextButton", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.AdvancedSearch_Next));
		}
		void PrepareButton(ASPxButton button, string name, string text) {
			button.AutoPostBack = false;
			button.Text = text;
			button.ClientInstanceName = MainFormLayout.GetClientInstanceName(name);
		}
	}
	[ToolboxItem(false)]
	public class QuickSearchPrevButton : ASPxButton {
		protected override ImagesBase CreateImages() {
			return new SearchPrevButtonImages(this);
		}
	}
	[ToolboxItem(false)]
	public class QuickSearchNextButton : ASPxButton {
		protected override ImagesBase CreateImages() {
			return new SearchNextButtonImages(this);
		}
	}
	public abstract class SearchButtonImages : ButtonImages {
		public SearchButtonImages(ASPxButton button)
			: base(button) {
		}
		protected override void PopulateImageInfoList(List<ImageInfo> list) {
			base.PopulateImageInfoList(list);
			list.Add(new ImageInfo(ButtonImages.ImageName, ImageFlags.IsPng, ASPxHtmlEditorLocalizer.GetString(GetAltText()), null, GetSpriteCssClassName(), "Web"));
		}
		protected abstract string GetSpriteCssClassName();
		protected abstract ASPxHtmlEditorStringId GetAltText();
	}
	public class SearchPrevButtonImages : SearchButtonImages {
		public SearchPrevButtonImages(ASPxButton button)
			: base(button) {
		}
		protected override string GetSpriteCssClassName() {
			return PagerImages.PrevButtonImageName;
		}
		protected override ASPxHtmlEditorStringId GetAltText() {
			return ASPxHtmlEditorStringId.ButtonOk;
		}
	}
	public class SearchNextButtonImages : SearchButtonImages {
		public SearchNextButtonImages(ASPxButton button)
			: base(button) {
		}
		protected override string GetSpriteCssClassName() {
			return PagerImages.NextButtonImageName;
		}
		protected override ASPxHtmlEditorStringId GetAltText() {
			return ASPxHtmlEditorStringId.ButtonOk;
		}
	}
}
