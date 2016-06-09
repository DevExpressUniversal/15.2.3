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
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Data;
using DevExpress.Web.Localization;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.Web.FilterControl {
	public class WebFilterControlPopupButtons : InternalTable {
		ASPxPopupFilterControl filterControl;
		ASPxButton okButton;
		ASPxButton cancelButton;		
		const string okButtonId = "FCOKBTN";
		const string cancelButtonId = "FCCANCELBTN";
		public WebFilterControlPopupButtons(ASPxPopupFilterControl filterControl) {
			this.filterControl = filterControl;
			CellSpacing = 0;
			CellPadding = 0;
		}
		protected ASPxPopupFilterControl FilterControl { get { return filterControl; } }
		protected WebFilterControlRenderHelper RenderHelper { get { return FilterControl.RenderHelper; } }
		protected IPopupFilterControlOwner FilterPopupOwner { get { return FilterControl.FilterPopupOwner; } }
		protected ASPxButton OKButton { get { return okButton; } }
		protected ASPxButton CancelButton { get { return cancelButton; } }
		protected override void CreateControlHierarchy() {
			TableRow row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			cell.Width = Unit.Percentage(100);
			this.okButton = CreateButton(row, okButtonId, RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_OK));
			CreateSpacer(row);
			this.cancelButton = CreateButton(row, cancelButtonId, RenderHelper.GetLocalizedString(ASPxEditorsStringId.FilterControl_Cancel));
		}
		protected virtual ASPxButton CreateButton(TableRow row, string id, string text) {
			ASPxButton button = new ASPxButton();
			button.ID = id;
			button.ParentSkinOwner = FilterControl;
			button.AutoPostBack = false;
			button.EnableViewState = false;
			button.EnableClientSideAPI = false;
			button.UseSubmitBehavior = false;
			button.Text = text;
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			cell.Controls.Add(button);
			return button;
		}
		protected virtual void CreateSpacer(TableRow row) {
			TableCell cell = RenderUtils.CreateTableCell();			
			row.Cells.Add(cell);
			cell.Controls.Add(RenderUtils.CreateLiteralControl("&nbsp;"));
		}
		protected override void PrepareControlHierarchy() {
			if (RenderHelper.Enabled) {
				OKButton.ClientSideEvents.Click = FilterControl.RenderHelper.GetScriptForApplyOnClick(FilterPopupOwner.GetJavaScriptForApplyFilterControl());
				CancelButton.ClientSideEvents.Click = FilterControl.RenderHelper.GetScriptForCancelOnClick(FilterPopupOwner.GetJavaScriptForCloseFilterControl());
			}
		}
	}
	[ToolboxItem(false), ViewStateModeById]
	public class WebFilterControlPopup : DevExpress.Web.ASPxPopupControl {
		IPopupFilterControlOwner filterPopupOwner;
		ASPxPopupFilterControl filterControl;
		WebControl buttonArea;
		Table mainAreaTable;
		TableCell mainAreaCell;
		public WebFilterControlPopup(IPopupFilterControlOwner filterPopupOwner) : base(filterPopupOwner.OwnerControl) {
			this.filterPopupOwner = filterPopupOwner;
			ShowOnPageLoad = false;
			PopupAnimationType = AnimationType.Fade;
			EnableClientSideAPI = false;
			ParentSkinOwner = OwnerControl;
			Modal = true;
			ShowPageScrollbarWhenModal = true;
			AllowDragging = true;
			CloseAction = CloseAction.CloseButton;
			Width = 300;
		}		
		protected override object SaveViewState() { return null; }
		protected IPopupFilterControlOwner FilterPopupOwner { get { return filterPopupOwner; } }
		protected ASPxPopupFilterControl FilterControl { get { return filterControl; } }
		protected IPopupFilterControlStyleOwner FilterPopupStyleOwner { get { return FilterPopupOwner as IPopupFilterControlStyleOwner; } }
		protected internal override Paddings GetContentPaddings(PopupWindow window) {
			Paddings paddings = new Paddings();
			paddings.Padding = Unit.Pixel(0);
			return paddings;
		}
		protected override void ClearControlFields() {
			this.filterControl = null;
		}
		protected override void CreateControlHierarchy() {
			ID = ASPxPopupFilterControl.PopupFilterControlFormID;
			Font.CopyFrom(this.OwnerControl.Font);
			ShowHeader = true;
			HeaderText = GetLocalizedText(FilterPopupOwner.FilterPopupHeaderText, ASPxEditorsStringId.FilterControl_PopupHeaderText);
			base.CreateControlHierarchy();
			Controls.Clear();
			CreateMainArea();
			CreateButtonArea();
			EnsureChildControlsRecursive(this, false);
		}
		void CreateMainArea() {
			this.mainAreaTable = RenderUtils.CreateTable();			
			Controls.Add(this.mainAreaTable);
			TableRow row = RenderUtils.CreateTableRow();
			this.mainAreaTable.Rows.Add(row);
			this.mainAreaCell = RenderUtils.CreateTableCell();
			row.Cells.Add(this.mainAreaCell);
			this.filterControl = CreatePopupFilterControl(FilterPopupOwner);
			ClientIDHelper.EnableClientIDGeneration(FilterControl);
			FilterControl.ID = ASPxPopupFilterControl.PopupFilterControlID;
			this.mainAreaCell.Controls.Add(FilterControl);
		}
		protected virtual ASPxPopupFilterControl CreatePopupFilterControl(IPopupFilterControlOwner filterPopupOwner) {
			return new ASPxPopupFilterControl(filterPopupOwner);
		}
		void CreateButtonArea() {
			this.buttonArea = new WebControl(HtmlTextWriterTag.Div);
			Controls.Add(this.buttonArea);
			this.buttonArea.Controls.Add(new WebFilterControlPopupButtons(FilterControl));
		}
		protected override void PrepareControlHierarchy() {
			if(FilterPopupStyleOwner != null) {
				CloseButtonImage.CopyFrom(FilterPopupStyleOwner.CloseButtonImage);				
				CloseButtonStyle.CopyFrom(FilterPopupStyleOwner.CloseButtonStyle);
				HeaderStyle.CopyFrom(FilterPopupStyleOwner.HeaderStyle);
				ModalBackgroundStyle.CopyFrom(FilterPopupStyleOwner.ModalBackgroundStyle);
				FilterPopupStyleOwner.MainAreaStyle.AssignToControl(this.mainAreaCell, true);
				FilterPopupStyleOwner.ButtonAreaStyle.AssignToControl(this.buttonArea, true);
			}
			base.PrepareControlHierarchy();
			ClientSideEvents.Init = "ASPx.FCPopupInit";
			ClientSideEvents.CloseButtonClick = string.Format("function (s, e) {{ {0} }}", FilterPopupOwner.GetJavaScriptForCloseFilterControl());
			PopupElementID = FilterPopupOwner.MainElementID;
			PopupHorizontalAlign = PopupHorizontalAlign.Center;
			PopupVerticalAlign = PopupVerticalAlign.Middle;
			PopupHorizontalOffset =  0;
			PopupVerticalOffset = 0;
			this.mainAreaTable.Width = Unit.Percentage(100);
			this.mainAreaCell.Height = 100;			
			this.mainAreaCell.VerticalAlign = VerticalAlign.Top;
		}
		protected override bool NeedCreateHierarchyOnInit() {
			return false;
		}
		protected string GetLocalizedText(string text, ASPxEditorsStringId defaultTextId) {
			return string.IsNullOrEmpty(text) ? ASPxEditorsLocalizer.GetString(defaultTextId) : text;
		}
	}
	public class InternalWebCheckBox : InternalWebControl {
		bool isChecked = false;
		public InternalWebCheckBox() 
			: base(HtmlTextWriterTag.Input) {
		}
		protected override void AddAttributesToRender(HtmlTextWriter writer) {
			base.AddAttributesToRender(writer);
			writer.AddAttribute(HtmlTextWriterAttribute.Type, "checkbox");
			if(IsChecked) {
				writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
			}
		}
		public bool IsChecked {
			get { return isChecked; }
			set { isChecked = value; }
		}
	}
	public class WebFilterControlPopupRow : InternalTable {
		IFilterControlRowOwner filterRowOwner;
		HyperLink linkClearFilter;
		HyperLink linkShowFilterPopup;
		HyperLink linkFilterText;
		Image imageShowFilterPopup;
		InternalWebCheckBox checkBoxFilterEnabled;
		TableRow mainRow;
		public WebFilterControlPopupRow(IFilterControlRowOwner filterRowOwner) {
			this.filterRowOwner = filterRowOwner;
			EnableViewState = false;			
			CellPadding = 0;
			CellSpacing = 0;
		}
		protected IFilterControlRowOwner FilterRowOwner { get { return filterRowOwner; } }
		protected HyperLink LinkClearFilter { get { return linkClearFilter; } }
		protected HyperLink LinkShowFilterPopup { get { return linkShowFilterPopup; } }
		protected HyperLink LinkFilterText { get { return linkFilterText; } }
		protected Image ImageShowFilterPopup { get { return imageShowFilterPopup; } }
		protected InternalWebCheckBox CheckBoxFilterEnabled { get { return checkBoxFilterEnabled; } }
		protected TableRow MainRow { get { return mainRow; } }
		protected bool HasFilter { get { return !string.IsNullOrEmpty(FilterRowOwner.FilterExpression); } }
		protected bool CanChangeFilterExpression { get { return FilterRowOwner.GetFilterColumns().Count > 0; } }
		protected override void CreateControlHierarchy() {
			this.mainRow = RenderUtils.CreateTableRow();
			Rows.Add(MainRow);
			if(FilterRowOwner.IsFilterEnabledSupported && HasFilter) {
				this.checkBoxFilterEnabled = AddCheckBoxToControl();
			}
			this.imageShowFilterPopup = AddImageToControl();
			if(!HasFilter) {
				this.linkShowFilterPopup = AddLinkControl(GetLocalizedText(FilterRowOwner.ShowFilterBuilderText, ASPxEditorsStringId.FilterControl_ShowFilterControl));
			} else {
				string defaultText = GetFilterCriteriaText(FilterRowOwner);
				string defaultTooltip = GetFilterCriteriaText(FilterRowOwner, false);
				string text = CustomizeCriteriaText(defaultText);
				string truncated = TruncateCriteriaText(text);
				this.linkFilterText = AddLinkControl(truncated);
				if(text != truncated)
					LinkFilterText.ToolTip = defaultText != text ? text : defaultTooltip;
				if(FilterRowOwner.IsRightToLeft && text == defaultText)
					LinkFilterText.Attributes["dir"] = "ltr";
			}
			TableCell cell = AddCell();
			cell.Text = "&nbsp;";
			cell.Width = Unit.Percentage(100);
			if(HasFilter && CanChangeFilterExpression) {
				this.linkClearFilter = AddLinkControl(GetLocalizedText(FilterRowOwner.ClearButtonText, ASPxEditorsStringId.FilterControl_ClearFilter));
			}
		}
		protected virtual string GetFilterCriteriaText(IFilterControlOwner filterOwner, bool encodeValue = true) {
			var textGenerator = CreateFilterCriteriaDisplayTextGenerator(filterOwner, encodeValue);
			return textGenerator.ToString();
		}
		protected virtual WebFilterCriteriaDisplayTextGenerator CreateFilterCriteriaDisplayTextGenerator(IFilterControlOwner filterOwner, bool encodeValue) {
			return new WebFilterCriteriaDisplayTextGenerator(filterOwner, filterOwner.FilterExpression, encodeValue);
		}
		string CustomizeCriteriaText(string defaultText) {
			CustomFilterExpressionDisplayTextEventArgs e = new CustomFilterExpressionDisplayTextEventArgs(FilterRowOwner.FilterExpression, defaultText);
			FilterRowOwner.RaiseCustomFilterExpressionDisplayText(e);
			var text = e.DisplayText;
			if(e.EncodeHtml && text != defaultText)
				text = HttpUtility.HtmlEncode(text);
			return text;
		}
		Image AddImageToControl() {
			TableCell cell = AddCell();
			Image image = RenderUtils.CreateImage();
			cell.Controls.Add(image);
			return image;
		}
		InternalWebCheckBox AddCheckBoxToControl() {
			TableCell cell = AddCell();
			InternalWebCheckBox check = new InternalWebCheckBox();
			cell.Controls.Add(check);
			return check;
		}
		protected string GetLocalizedText(string text, ASPxEditorsStringId defaultTextId) {
			return string.IsNullOrEmpty(text) ? ASPxEditorsLocalizer.GetString(defaultTextId) : text;
		}
		protected virtual HyperLink AddLinkControl(string text) {
			TableCell cell = AddCell();			
			HyperLink link = RenderUtils.CreateHyperLink();
			if(IsEnabled && CanChangeFilterExpression)
				link.NavigateUrl = RenderUtils.AccessibilityEmptyUrl;
			link.Text = text;
			cell.Controls.Add(link);
			return link;
		}
		protected virtual TableCell AddCell() {
			TableCell cell = RenderUtils.CreateTableCell();
			MainRow.Cells.Add(cell);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareCheckBox();
			PrepareImage();
			PrepareLinkControl(LinkClearFilter, FilterRowOwner.GetJavaScriptForClearFilter());
			WebControl expressionLink = LinkShowFilterPopup ?? LinkFilterText;
			if(expressionLink != null) {
				PrepareLinkControl(expressionLink, FilterRowOwner.GetJavaScriptForShowFilterControl());				
				FilterRowOwner.AssignExpressionCellStyleToControl((TableCell)expressionLink.Parent);
			}
			if(LinkClearFilter != null)
				FilterRowOwner.AssignClearButtonCellStyleToControl((TableCell)LinkClearFilter.Parent);				
			Width = Unit.Percentage(100);
			FilterRowOwner.AssignFilterStyleToControl(this);
			FilterRowOwner.AppendDefaultDXClassName(MainRow);
		}
		protected void PrepareLinkControl(WebControl control, string onClickJavaScript) {
			if(control == null) return;
			if(IsEnabled && CanChangeFilterExpression)
				control.Attributes["onclick"] = onClickJavaScript;
			FilterRowOwner.AssignLinkStyleToControl(control);
		}
		protected virtual void PrepareCheckBox() {
			if(CheckBoxFilterEnabled != null) {
				CheckBoxFilterEnabled.IsChecked = FilterRowOwner.IsFilterEnabled;
				if(IsEnabled)
					CheckBoxFilterEnabled.Attributes["onclick"] = FilterRowOwner.GetJavaScriptForSetFilterEnabledForCheckbox();
				else
					CheckBoxFilterEnabled.Enabled = false;
				FilterRowOwner.AssignCheckBoxCellStyleToControl((TableCell)CheckBoxFilterEnabled.Parent);
			}
		}
		protected void PrepareImage() {
			FilterRowOwner.CreateFilterImage.AssignToControl(ImageShowFilterPopup, false);			
			FilterRowOwner.AssignImageCellStyleToControl((TableCell)ImageShowFilterPopup.Parent);
			if(IsEnabled && CanChangeFilterExpression)
				ImageShowFilterPopup.Attributes["onclick"] = FilterRowOwner.GetJavaScriptForShowFilterControl();
			else
				RenderUtils.SetCursor((WebControl)ImageShowFilterPopup.Parent, "default");				
		}
		string TruncateCriteriaText(string text) {
			const int maxTextLength = 100;
			if(text.Length > maxTextLength) {
				int pos = text.IndexOf(' ', maxTextLength);
				if(pos < 0)
					pos = maxTextLength;
				return text.Substring(0, pos) + "&hellip;";
			}
			return text;
		}
	}
}
