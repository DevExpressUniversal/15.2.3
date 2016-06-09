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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Web;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Internal {
	public class InputFileInfo {
		public TableCell ErrorCell;
		public TableRow ErrorRow;
		public WebControl InputControl;
		public WebControl FakeInputControl;
		public TableCell ClearBoxCell;
		public UploadControlButtonControl ClearButton;
		public TableCell TextBoxCell;
		public TableRow InputRow;
		public TableCell SeparatorCell;
		public TableRow SeparatorRow;
		public TableCell RemoveButtonSeparatorCell;
	}
	public class MainUploadControlBase : ASPxInternalWebControl {
		private const string TextboxInputClassName = "dxTI";
		private InputFileInfo templateInputFileInfo = null;
		private WebControl uploadIframe = null;
		private ASPxUploadControl upload = null;
		public MainUploadControlBase(ASPxUploadControl upload)
			: base() {
				this.upload = upload;
		}
		protected InputFileInfo TemplateInputFileInfo {
			get { return templateInputFileInfo; }
			set { this.templateInputFileInfo = value; }
		}
		protected WebControl UploadIframe {
			get { return uploadIframe; }
		}
		protected ASPxUploadControl Upload {
			get { return upload; }
		}
		protected WebControl InlineDropZoneDiv { get; set; }
		protected WebControl InlineDropZoneBorderWrapper { get; set; }
		protected WebControl InlineDropZoneBackgroundWrapper { get; set; }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.uploadIframe = null;
			TemplateInputFileInfo = null;
			InlineDropZoneDiv = null;
			InlineDropZoneBorderWrapper = null;
			InlineDropZoneBackgroundWrapper = null;
		}
		protected void CreateUploadIframe() {
			this.uploadIframe = RenderUtils.CreateFakeIFrame(Upload.GetUploadIframeID(), 100000, Upload.GetUploadIframeSrc());
			RenderUtils.SetStringAttribute(UploadIframe, "name", Upload.GetUploadIframeName());
			Controls.Add(UploadIframe);
		}
		protected void CreateInput(InputFileInfo info, WebControl parentControl, int index) {
			WebControl fileInputControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			fileInputControl.Attributes.Add("type", "file");
			fileInputControl.ID = Upload.GetUploadInputId(index);
			fileInputControl.CssClass = TextboxInputClassName;
			if(Upload.UploadMode != UploadControlUploadMode.Advanced)	   
				fileInputControl.Attributes.Add("name", Upload.GetUploadInputName(index));
			if(IsAccessibilityTitleNeeded(index))
				fileInputControl.Attributes.Add("title", Upload.BrowseButton.Text);
			parentControl.Controls.Add(fileInputControl);
			info.InputControl = fileInputControl;
		}
		protected bool IsAccessibilityTitleNeeded(int index) {
			return Upload.AccessibilityCompliant && !DesignMode;
		}
		protected void CreateInlineDropZoneDiv() {
			if(!Upload.AdvancedModeSettings.EnableDragAndDrop) return;
			InlineDropZoneDiv = RenderUtils.CreateDiv();
			InlineDropZoneBorderWrapper = RenderUtils.CreateDiv();
			InlineDropZoneBackgroundWrapper = RenderUtils.CreateDiv();
			InlineDropZoneDiv.ID = Upload.GetInlineDropZoneDivID();
			ASPxLabel textLabel = new ASPxLabel();
			textLabel.Text = Upload.AdvancedModeSettings.DropZoneText;
			InlineDropZoneBackgroundWrapper.Controls.Add(textLabel);
			InlineDropZoneBorderWrapper.Controls.Add(InlineDropZoneBackgroundWrapper);
			InlineDropZoneDiv.Controls.Add(InlineDropZoneBorderWrapper);
			Controls.Add(InlineDropZoneDiv);
		}
		protected void PrepareInlineDropZoneDiv() {
			RenderUtils.AppendDefaultDXClassName(InlineDropZoneDiv, UploadControlStyles.InlineDropZoneSystemStyleName);
			RenderUtils.AppendDefaultDXClassName(InlineDropZoneBorderWrapper, UploadControlStyles.InlineDropZoneBorderWrapperSystemStyleName);
			RenderUtils.AppendDefaultDXClassName(InlineDropZoneBackgroundWrapper, UploadControlStyles.InlineDropZoneBackgroundWrapperSystemStyleName);
			Upload.GetInlineDropZoneStyle().AssignToControl(InlineDropZoneDiv);
			InlineDropZoneDiv.Width = Upload.GetInlineDropZoneWidth();
			InlineDropZoneDiv.Height = Upload.GetInlineDropZoneHeight();
			RenderUtils.SetVisibility(InlineDropZoneDiv, false, true);
		}
		protected void PrepareInputFile(InputFileInfo info, int index) {
			if(info.InputControl != null)
				PrepareInput(info, index);
		}
		protected void PrepareInput(InputFileInfo info, int index) {
			ApplyValidationSettings(info);
			ApplyInputStyle(info);
			SetInputTabIndex(info);
			if(!Upload.Enabled)
				RenderUtils.SetStringAttribute(info.InputControl, "disabled", "disabled");
		}
		protected void ApplyValidationSettings(InputFileInfo info) {
			if(Upload.ValidationSettings.AllowedFileExtensions.Length > 0 && !Upload.IsAnyExtensionAllowed()) 
				RenderUtils.SetStringAttribute(info.InputControl, "accept", GetAllowedFileExtensionsString());
		}
		protected void ApplyInputStyle(InputFileInfo info) {
			if(Upload.IsNativeRender())
				PrepareNativeInputStyle(info.InputControl);
			else
				PrepareInputStyle(info.InputControl);
		}
		protected virtual void SetInputTabIndex(InputFileInfo info) {
			info.InputControl.TabIndex = Upload.TabIndex;
		}
		protected string GetAllowedFileExtensionsString() {
			string allowedFileExtensionsString = string.Empty;
			if(RenderUtils.IsHtml5Mode(Upload)) {
				for(int i = 0;i < Upload.ValidationSettings.AllowedFileExtensions.Length;i++) {
					allowedFileExtensionsString += Upload.ValidationSettings.AllowedFileExtensions[i] + "/*";
					if(i < Upload.ValidationSettings.AllowedFileExtensions.Length - 1)
						allowedFileExtensionsString += ",";
				}
			}
			else
				allowedFileExtensionsString = string.Join(",", Upload.ValidationSettings.AllowedFileExtensions);
			return allowedFileExtensionsString;
		}
		protected void PrepareInputStyle(WebControl inputControl) {
			RenderUtils.SetStyleStringAttribute(inputControl, "position", "absolute");
			RenderUtils.SetStyleStringAttribute(inputControl, "filter", "alpha(opacity=0)");
			RenderUtils.SetStyleStringAttribute(inputControl, "opacity", "0");
			RenderUtils.SetStyleStringAttribute(inputControl, "top", "-5000px");
			RenderUtils.SetStyleStringAttribute(inputControl, "z-index", "29999");
			if(Browser.IsIE) {
				RenderUtils.SetStyleStringAttribute(inputControl, "font-size", "0pt");
				RenderUtils.SetStyleStringAttribute(inputControl, "height", "20px");
			}
		}
		protected void PrepareNativeInputStyle(WebControl inputControl) {
			AppearanceStyleBase editAreaStyle = Upload.GetEditAreaStyle(true);
			editAreaStyle.AssignToControl(inputControl);
			inputControl.Width = !Upload.Width.IsEmpty ? Unit.Percentage(100) : Unit.Empty;
			if(!DesignMode || Upload.Size != 0)
				RenderUtils.SetStringAttribute(inputControl, "size", Upload.Size.ToString());
			RenderUtils.AppendDefaultDXClassName(inputControl, UploadControlStyles.EditAreaSystemStyleName);
		}
	}
	public class MainUploadControl : MainUploadControlBase {
		private const string BrowseButtonCellClassName = "dxBB";
		private const string ClearButtonCellClassName = "dxCB";
		private const string RemoveButtonCellClassName = "dxRB";
		private const string TextboxCellClassName = "dxTB";
		private const string TextboxFakeInputClassName = "dxTF";
		private AddUploadButtonsPanelControl addUploadButtonsPanel = null;
		private UploadingPanelControl uploadingPanelControl = null;
		private List<InputFileInfo> inputFilesInfo = null;
		private TableRow addUploadButtonsSeparatorRow = null;
		private TableCell addUploadButtonsSeparatorCell = null;
		private TableRow addUploadButtonsPanelRow = null;
		private TableCell addUploadButtonsPanelCell = null;
		private TableCell addUploadButtonsPanelEmptyCell = null;
		private WebControl commonErrorDiv = null;
		private Table platformErrorTable = null;
		private Table mainTable = null;
		private TableCell mainTableCell = null;
		private Table table = null;
		private Table uploadingPanelTable = null;
		private TableCell uploadingPanelCell = null;
		public MainUploadControl(ASPxUploadControl upload)
			: base(upload) { }
		protected AddUploadButtonsPanelControl AddUploadButtonsPanel {
			get { return addUploadButtonsPanel; }
		}
		protected UploadingPanelControl UploadingPanelControl {
			get { return uploadingPanelControl; }
		}
		protected TableRow AddUploadButtonsPanelRow {
			get { return addUploadButtonsPanelRow; }
		}
		protected TableCell AddUploadButtonsPanelCell {
			get { return addUploadButtonsPanelCell; }
		}
		protected TableRow AddUploadButtonsSeparatorRow {
			get { return addUploadButtonsSeparatorRow; }
		}
		protected TableCell AddUploadButtonsSeparatorCell {
			get { return addUploadButtonsSeparatorCell; }
		}
		protected TableCell AddUploadButtonsPanelEmptyCell {
			get { return addUploadButtonsPanelEmptyCell; }
		}
		protected WebControl CommonErrorDiv {
			get { return commonErrorDiv; }
		}
		protected Table PlatformErrorTable {
			get { return platformErrorTable; }
		}
		protected List<InputFileInfo> InputFilesInfo {
			get { return inputFilesInfo; }
		}
		protected Table MainTable {
			get { return mainTable; }
		}
		protected TableCell MainTableCell {
			get { return mainTableCell; }
		}
		protected Table Table {
			get { return table; }
		}
		protected Table UploadingPanelTable {
			get { return uploadingPanelTable; }
		}
		protected TableCell UploadingPanelCell {
			get { return uploadingPanelCell; }
		}
		protected bool IsRightToLeft {
			get { return (Upload as ISkinOwner).IsRightToLeft(); }
		}			
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.addUploadButtonsPanel = null;
			this.uploadingPanelControl = null;
			this.addUploadButtonsSeparatorRow = null;
			this.addUploadButtonsPanelRow = null;
			this.addUploadButtonsPanelCell = null;
			this.addUploadButtonsSeparatorCell = null;
			this.addUploadButtonsPanelEmptyCell = null;
			this.commonErrorDiv = null;
			this.platformErrorTable = null;
			this.mainTableCell = null;
			this.inputFilesInfo = null;
			this.table = null;
			this.uploadingPanelTable = null;
			this.uploadingPanelCell = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateInlineDropZoneDiv();
			CreateMainTable();
			CreateMainTableCell();
			if(Upload.AdvancedModeSettings.FileListPosition == UploadControlFileListPosition.Top)
				CreateFileList();
			CreateTable();
			CreateInputFiles();
			if(IsAddUploadButtonPanelShow())
				CreateAddUploadButtonsPanelCell();
			if(Upload.ShowProgressPanel)
				CreateProgressPanel();
			CreateCommonErrorDiv();
			CreatePlatformErrorTable();
			if(!Upload.DesignMode)
				CreateUploadIframe();
			if(Upload.AdvancedModeSettings.FileListPosition == UploadControlFileListPosition.Bottom)
				CreateFileList();
		}
		protected void CreateFileList() {
			if(Upload.AdvancedModeSettings.EnableFileList) {
				MainTableCell.Controls.Add(new UploadFileListControl(Upload));
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(MainTable != null) {
				PrepareMainTable();
				PrepareMainCell();
				PrepareTable();
				PrepareInputFiles();
				if(AddUploadButtonsPanelCell != null)
					PrepareAddUploadButtonsPanelCell();
				if(UploadingPanelTable != null)
					PrepareProgressPanel();
				if(CommonErrorDiv != null)
					PrepareCommonErrorDiv();
				if(PlatformErrorTable != null)
					PreparePlatformErrorTable();
				if(InlineDropZoneDiv != null)
					PrepareInlineDropZoneDiv();
			}
		}
		protected void CreateAddUploadButtonsPanelCell() {
			if(Upload.GetAddUploadButtonsSpacing() != Unit.Pixel(0)) {
				this.addUploadButtonsSeparatorRow = RenderUtils.CreateTableRow();
				Table.Rows.Add(AddUploadButtonsSeparatorRow);
				this.addUploadButtonsSeparatorCell = RenderUtils.CreateIndentCell();
				AddUploadButtonsSeparatorRow.Cells.Add(AddUploadButtonsSeparatorCell);
			}
			this.addUploadButtonsPanelRow = RenderUtils.CreateTableRow();
			Table.Rows.Add(AddUploadButtonsPanelRow);
			this.addUploadButtonsPanelCell = RenderUtils.CreateTableCell();
			AddUploadButtonsPanelRow.Cells.Add(AddUploadButtonsPanelCell);
			this.addUploadButtonsPanel = new AddUploadButtonsPanelControl(Upload);
			AddUploadButtonsPanelCell.Controls.Add(AddUploadButtonsPanel);
			if(IsRemoveButtonVisible() &&
				(Upload.GetAddUploadButtonsHorizontalPosition() == 
				AddUploadButtonsHorizontalPosition.InputRightSide)) {
				this.addUploadButtonsPanelEmptyCell = RenderUtils.CreateTableCell();
				AddUploadButtonsPanelRow.Cells.Add(AddUploadButtonsPanelEmptyCell);
			}
		}
		protected void CreateInputFiles() {
			this.inputFilesInfo = new List<InputFileInfo>();
			int startIndex = Upload.DesignMode ? 0 : -1;
			for(int i = startIndex; i < Upload.FileInputCount; i++) {
				InputFileInfo info = new InputFileInfo();
				CreateTextBoxCell(info, i);
				if(!Upload.IsNativeRender()) {
					if(Upload.ShowClearFileSelectionButton && Upload.ShowTextBox)
						CreateClearBoxCell(info, i);
					CreateInputFile(info, i);
				} else
					CreateNativeInputFile(info, i);
				if(IsRemoveButtonVisible()) {
					if(IsRemoveButtonSpacingExist())
						CreateRemoveButtonSeparatorCell(info, info.InputRow);
					CreateRemoveButton(info.InputRow, i);
				}
				CreateErrorCell(info, i);
				if(i == -1 || ((i < Upload.FileInputCount - 1) && Upload.GetFileInputSpacing() != Unit.Pixel(0)))
					CreateInputSeparatorCell(info);
				if(i != -1) 
					InputFilesInfo.Add(info);
				else
					TemplateInputFileInfo = info;
			}
		}
		protected void CreateInputFile(InputFileInfo info, int index) {
			if(!DesignMode)
				CreateInput(info, info.TextBoxCell, index);
			if(Upload.ShowTextBox)
				CreateFakeInput(info, info.TextBoxCell, index);
			CreateBrowseButton(info.InputRow, index);
			if(!Upload.ShowTextBox)
				CreateFillerCell(info.InputRow);
		}
		protected void CreateNativeInputFile(InputFileInfo info, int index) {
			CreateInput(info, info.TextBoxCell, index);
		}
		protected void CreateTextBoxCell(InputFileInfo info, int index) {
			info.InputRow = RenderUtils.CreateTableRow();
			info.InputRow.ID = Upload.GetFileInputRowId(index);
			Table.Rows.Add(info.InputRow);
			info.TextBoxCell = RenderUtils.CreateTableCell();
			info.TextBoxCell.ID = Upload.GetUploadTextBoxCellId(index);
			info.TextBoxCell.CssClass = TextboxCellClassName;
			if(!Upload.ShowTextBox)
				RenderUtils.AppendDefaultDXClassName(info.TextBoxCell, "dxTBHidden");
			info.InputRow.Cells.Add(info.TextBoxCell);
		}
		protected void CreateClearBoxCell(InputFileInfo info, int index) {
			info.ClearBoxCell = RenderUtils.CreateTableCell();
			info.InputRow.Cells.Add(info.ClearBoxCell);
			info.ClearBoxCell.ID = Upload.GetUploadClearBoxID(index);
			info.ClearBoxCell.CssClass = ClearButtonCellClassName;
			info.ClearButton = new UploadControlButtonControl(Upload, null, "", Upload.GetClearButtonImage(), ImagePosition.Right, index);
			info.ClearBoxCell.Controls.Add(info.ClearButton);
		}
		protected void CreateFakeInput(InputFileInfo info, TableCell parentCell, int index) {
			WebControl fakeFileInputControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			fakeFileInputControl.ID = Upload.GetUploadFakeInputId(index);
			fakeFileInputControl.CssClass = TextboxFakeInputClassName;
			fakeFileInputControl.Attributes.Add("type", "text");
			fakeFileInputControl.Attributes.Add("readonly", "readonly");
			if(DesignMode && Upload.IsNullTextEnabled())
				fakeFileInputControl.Attributes.Add("value", Upload.NullText);
			if(IsAccessibilityTitleNeeded(index))
				fakeFileInputControl.Attributes.Add("title", 
					ASPxperienceLocalizer.GetString(ASPxperienceStringId.UploadControl_AccessibilityTitleForFakeInput));	 
			parentCell.Controls.Add(fakeFileInputControl);
			info.FakeInputControl = fakeFileInputControl;
		}
		protected void CreateErrorCell(InputFileInfo info, int index) {
			if(Upload.GetIsShowErrorRow(index)) {
				info.ErrorRow = RenderUtils.CreateTableRow();
				Table.Rows.Add(info.ErrorRow);
				info.ErrorRow.ID = Upload.GetErrorRowID(index);
				info.ErrorCell = RenderUtils.CreateTableCell();
				info.ErrorRow.Cells.Add(info.ErrorCell);
				info.ErrorCell.Controls.Add(RenderUtils.CreateLiteralControl(Upload.GetErrorText(index)));
			}
		}
		protected void CreateRemoveButtonSeparatorCell(InputFileInfo info, TableRow row) {
			info.RemoveButtonSeparatorCell = RenderUtils.CreateIndentCell();
			row.Cells.Add(info.RemoveButtonSeparatorCell);
		}
		protected void CreateBrowseButton(TableRow row, int index) {
			ButtonTableCell cell = new ButtonTableCell(Upload, Upload.BrowseButton, Upload.GetBrowseButtonImage(), null, index, false);
			row.Cells.Add(cell);
			cell.ID = Upload.GetBrowseButtonID(index);
			cell.CssClass = BrowseButtonCellClassName;
		}
		protected void CreateFillerCell(TableRow row) {
			TableCell filler = new TableCell();
			filler.Width = Unit.Percentage(100);
			row.Cells.Add(filler);
		}
		protected void CreateRemoveButton(TableRow row, int index) {
			ButtonTableCell cell = new ButtonTableCell(Upload, Upload.RemoveButton, Upload.GetRemoveButtonImage(), null, index);
			row.Cells.Add(cell);
			cell.ID = Upload.GetRemoveButtonID(index);
			cell.CssClass = RemoveButtonCellClassName;
		}
		protected void CreateInputSeparatorCell(InputFileInfo info) {
			info.SeparatorRow = RenderUtils.CreateTableRow();
			info.SeparatorRow.CssClass = "dxucSR";
			Table.Rows.Add(info.SeparatorRow);
			info.SeparatorCell = RenderUtils.CreateIndentCell();
			info.SeparatorRow.Cells.Add(info.SeparatorCell);
		}
		protected void CreateMainTable() {
			this.mainTable = RenderUtils.CreateTable();
			Controls.Add(MainTable);
		}
		protected void CreateMainTableCell() {
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			this.mainTableCell = RenderUtils.CreateTableCell();
			row.Cells.Add(mainTableCell);
		}
		protected void CreateTable() {
			this.table = RenderUtils.CreateTable(Upload.IsBorderSeparate());
			this.mainTableCell.Controls.Add(Table);
		}
		protected void CreateProgressPanel() {
			this.uploadingPanelTable = RenderUtils.CreateTable();
			MainTableCell.Controls.Add(UploadingPanelTable);
			TableRow row = RenderUtils.CreateTableRow();
			UploadingPanelTable.Rows.Add(row);
			this.uploadingPanelCell = RenderUtils.CreateTableCell();
			row.Cells.Add(UploadingPanelCell);
			UploadingPanelCell.Controls.Add(new UploadingPanelControl(Upload));
		}
		protected void CreateCommonErrorDiv() {
			if(Upload.IsCommonErrorDivVisible()) {
				this.commonErrorDiv = RenderUtils.CreateDiv();
				CommonErrorDiv.ID = Upload.GetCommonErrorDivID();
				CommonErrorDiv.Controls.Add(RenderUtils.CreateLiteralControl(StringResources.UploadControl_SampleCommonErrorText));
				MainTableCell.Controls.Add(CommonErrorDiv);
			}
		}
		protected void CreatePlatformErrorTable() {
			if(Upload.UploadMode == UploadControlUploadMode.Advanced) {
				this.platformErrorTable = RenderUtils.CreateTable();
				PlatformErrorTable.ID = Upload.GetPlatformErrorTableID();
				TableRow row = RenderUtils.CreateTableRow();
				PlatformErrorTable.Rows.Add(row);
				TableCell textCell = RenderUtils.CreateTableCell();
				row.Cells.Add(textCell);
				textCell.Controls.Add(RenderUtils.CreateLiteralControl(Upload.GetPlatformErrorText()));
				MainTableCell.Controls.Add(PlatformErrorTable);
			}
		}
		protected void PrepareAddUploadButtonsPanelCell() {
			int colSpan = GetTableCellColumnSpan();
			if(AddUploadButtonsSeparatorRow != null) {
				AddUploadButtonsSeparatorRow.ID = Upload.GetAddUploadButtonsSeparatorRowID();
				RenderUtils.PrepareIndentCell(AddUploadButtonsSeparatorCell, Unit.Empty, Upload.GetAddUploadButtonsSpacing());
				if(Upload.FileInputCount == 0)
					RenderUtils.SetVisibility(AddUploadButtonsSeparatorRow, false, true);
				RenderUtils.SetStringAttribute(AddUploadButtonsSeparatorCell, "colspan", colSpan.ToString());
			}
			AddUploadButtonsHorizontalPosition position = Upload.GetAddUploadButtonsHorizontalPosition();
			string alignAttrValue =
				position == AddUploadButtonsHorizontalPosition.InputRightSide ? "right" : 
				position.ToString().ToLower();
			AddUploadButtonsPanelRow.ID = Upload.GetAddUploadButtonsPanelRowID();
			RenderUtils.SetStringAttribute(AddUploadButtonsPanelCell, "align", alignAttrValue);
			if(AddUploadButtonsPanelEmptyCell != null) {
				int emptyCellColSpan = IsRemoveButtonSpacingExist() ? 2 : 1;
				if(emptyCellColSpan > 1)
					RenderUtils.SetStringAttribute(AddUploadButtonsPanelEmptyCell, "colspan", emptyCellColSpan.ToString());
				RenderUtils.SetStringAttribute(AddUploadButtonsPanelCell, "colspan", (colSpan - emptyCellColSpan).ToString());
			}
			else
				RenderUtils.SetStringAttribute(AddUploadButtonsPanelCell, "colspan", colSpan.ToString());
		}
		protected void PrepareMainTable() {
			RenderUtils.AssignAttributes(Upload, MainTable);
			Upload.GetControlStyle().AssignToControl(MainTable, AttributesRange.Font);
			MainTable.Width = Upload.GetControlStyle().Width;
			MainTable.Height = Upload.GetControlStyle().Height;
			MainTable.TabIndex = 0;
			RenderUtils.SetVisibility(MainTable, Upload.IsClientVisible(), true);
			if(Upload.AccessibilityCompliant && Upload.IsAriaSupported())
				MainTable.Attributes.Add("role", "presentation");
		}
		protected void PrepareMainCell() {
			MainTableCell.Width = Unit.Percentage(100);
			Upload.GetControlStyle().AssignToControl(MainTable, AttributesRange.Common | AttributesRange.Font);
			Upload.GetControlStyle().AssignToControl(MainTableCell, AttributesRange.Cell);
			RenderUtils.SetVerticalAlign(MainTableCell, VerticalAlign.Top);
			RenderUtils.SetPaddings(MainTableCell, Upload.GetPaddings());
		}
		protected void PrepareTable() {
			Table.ID = Upload.GetUploadInputsTableID();
			Table.Width = Unit.Percentage(100);
			Upload.GetInputsTableStyle().AssignToControl(Table);
			if(Upload.AccessibilityCompliant && Upload.IsAriaSupported()) {
				Table.Attributes.Add("role", "presentation");
				if(Browser.IsIE)
					foreach(TableRow row in Table.Rows)
						row.Attributes.Add("role", "presentation");
			}
		}
		protected void PrepareInputFiles() {
			for (int i = -1; i < InputFilesInfo.Count; i++) {
				InputFileInfo info = (i == -1) ? TemplateInputFileInfo : InputFilesInfo[i];
				if(info == null)
					continue;
				PrepareTextBoxCell(info, i);
				if(info.ClearBoxCell != null)
					PrepareClearBoxCell(info, i);
				PrepareInputFile(info, i);
				RenderUtils.SetVisibility(info.InputRow, i != -1, true);
				if(IsRemoveButtonSpacingExist())
					PrepareRemoveButtonSeparatorCell(info);
				if(info.ErrorRow != null)
					PrepareErrorCell(info, i);
				if(info.SeparatorCell != null)
					PrepareInputSeparatorCell(info, i);
			}
		}
		protected new void PrepareInputFile(InputFileInfo info, int index) {
			base.PrepareInputFile(info, index);
			if(info.FakeInputControl != null)
				PrepareFakeInput(info, index);
		}
		protected void PrepareFakeInput(InputFileInfo info, int index) {
			AppearanceStyleBase editAreaStyle
				= (DesignMode && Upload.IsNullTextEnabled()) ? Upload.GetEditAreaNullStyle() : Upload.GetEditAreaStyle(false);
			editAreaStyle.AssignToControl(info.FakeInputControl);
			info.FakeInputControl.Width = !Upload.Width.IsEmpty ? Unit.Percentage(100) : Unit.Empty;
			RenderUtils.SetStringAttribute(info.FakeInputControl, "tabindex", "-1");
			if(!Upload.Enabled)
				RenderUtils.SetStringAttribute(info.FakeInputControl, "disabled", "disabled");
			if(Upload.Size != 0)
				RenderUtils.SetStringAttribute(info.FakeInputControl, "size", Upload.Size.ToString());
		}
		protected void PrepareTextBoxCell(InputFileInfo info, int index) {
			if(!Upload.IsNativeRender()) {
				AppearanceStyleBase textBoxStyle
					= (DesignMode && Upload.IsNullTextEnabled()) ? Upload.GetTextBoxNullStyle() : Upload.GetTextBoxStyle();
				textBoxStyle.AssignToControl(info.TextBoxCell);
				RenderUtils.SetPaddings(info.TextBoxCell, Upload.GetTextBoxPaddings());
				info.InputRow.Height = textBoxStyle.Height;
			}
			info.TextBoxCell.Width = Upload.GetTextBoxWidth();
			info.TextBoxCell.HorizontalAlign = IsRightToLeft ? HorizontalAlign.Right : HorizontalAlign.Left;
		}
		protected void PrepareClearBoxCell(InputFileInfo info, int index) {
			AppearanceStyleBase clearBoxStyle
				= (DesignMode && Upload.IsNullTextEnabled()) ? Upload.GetClearBoxNullStyle() : Upload.GetClearBoxStyle();
			clearBoxStyle.AssignToControl(info.ClearBoxCell);
			RenderUtils.SetPaddings(info.ClearBoxCell, Upload.GetTextBoxPaddings());
			info.ClearButton.ButtonImageID = Upload.GetClearButtonImageID(index);
			RenderUtils.SetStyleStringAttribute(info.ClearButton.ImageControl, "display", "inline");
			if(!Upload.IsClearFileSelectionButtonVisible())
				RenderUtils.SetStyleStringAttribute(info.ClearButton.Hyperlink, "visibility", "hidden");
			if(DesignMode)
				info.ClearBoxCell.Width = Unit.Percentage(0);
		}
		protected void PrepareErrorCell(InputFileInfo info, int index) {			
			RenderUtils.SetVisibility(info.ErrorRow, !Upload.GetIsValid(index), true);
			RenderUtils.SetStringAttribute(info.ErrorCell, "colspan", GetTableCellColumnSpan().ToString());
			Upload.GetErrorCellStyle().AssignToControl(info.ErrorCell);
		}
		protected void PrepareInputSeparatorCell(InputFileInfo info, int index) {
			if(index == -1)
				RenderUtils.SetVisibility(info.SeparatorRow, false, true);
			RenderUtils.PrepareIndentCell(info.SeparatorCell, Unit.Empty, Upload.GetFileInputSpacing());
			RenderUtils.SetStringAttribute(info.SeparatorCell, "colspan", GetTableCellColumnSpan().ToString());
		}
		protected void PrepareRemoveButtonSeparatorCell(InputFileInfo info) {
			RenderUtils.PrepareIndentCell(info.RemoveButtonSeparatorCell, Upload.GetRemoveButtonSpacing(), Unit.Empty);
		}
		protected void PrepareCommonErrorDiv() {
			Upload.GetErrorCellStyle().AssignToControl(CommonErrorDiv);
			RenderUtils.SetPaddings(CommonErrorDiv, Unit.Empty, Upload.GetAddUploadButtonsSpacing(), Unit.Empty,
				Unit.Empty);
			RenderUtils.SetVisibility(CommonErrorDiv, DesignMode, true);
		}
		protected void PreparePlatformErrorTable() {
			RenderUtils.SetVisibility(PlatformErrorTable, false, true);
			Upload.GetPlatformErrorPanelStyle().AssignToControl(PlatformErrorTable);
		}
		protected void PrepareProgressPanel() {
			base.PrepareControlHierarchy();
			UploadingPanelTable.ID = Upload.GetProgressPanelTableID();
			UploadingPanelTable.Width = Unit.Percentage(100);
			if (!Upload.IsVisibleUploadingPanel())
				RenderUtils.SetVisibility(UploadingPanelTable, false, true);
			RenderUtils.SetVerticalAlign(UploadingPanelCell, VerticalAlign.Middle);
		}
		protected int GetTableCellColumnSpan() {
			int colSpan = Upload.ShowClearFileSelectionButton ? 3 : 2;
			if(Upload.IsNativeRender())
				colSpan = 1;
			if(IsRemoveButtonVisible())
				colSpan += IsRemoveButtonSpacingExist() ? 2 : 1;
			return colSpan;
		}
		protected bool IsAddUploadButtonPanelShow() {
			return Upload.ShowAddRemoveButtons || Upload.IsUploadButtonVisible();
		}
		protected bool IsRemoveButtonVisible() {
			return Upload.ShowAddRemoveButtons;
		}
		protected bool IsRemoveButtonSpacingExist() {
			return (IsRemoveButtonVisible() && Upload.GetRemoveButtonSpacing() != Unit.Pixel(0));
		}
	}
	public class MainUploadControlHiddenUI : MainUploadControlBase {
		private WebControl mainDiv = null;
		private InputFileInfo inputFileInfo;
		public MainUploadControlHiddenUI (ASPxUploadControl upload)
			: base(upload) { }
		protected WebControl MainDiv {
			get { return mainDiv; }
		}
		protected InputFileInfo InputFileInfo {
			get { return inputFileInfo; }
		}
		public static void SetShowUIMode(ASPxUploadControl upload, bool showUI) {
			upload.ShowUI = showUI;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateInlineDropZoneDiv();
			CreateMainDiv();
			CreateInputFile();
			if(!Upload.DesignMode)
				CreateUploadIframe();
		}
		protected void CreateMainDiv() {
			this.mainDiv = RenderUtils.CreateDiv();
			Controls.Add(MainDiv);
		}
		protected void CreateInputFile() {
			if(!Upload.DesignMode)
				TemplateInputFileInfo = CreateInputFileInfo(-1);
			this.inputFileInfo = CreateInputFileInfo(0);
		}
		protected InputFileInfo CreateInputFileInfo(int index) {
			var info = new InputFileInfo();
			CreateInput(info, MainDiv, index);
			return info;
		}
		protected override void PrepareControlHierarchy() {
			PrepareMainDiv();
			PrepareInputFiles();
			if(InlineDropZoneDiv != null)
				PrepareInlineDropZoneDiv();
		}
		protected void PrepareMainDiv() {
			RenderUtils.AssignAttributes(Upload, MainDiv);
			RenderUtils.AppendDefaultDXClassName(MainDiv, UploadControlStyles.HiddenUIStyleName);
			MainDiv.Width = 1;
			MainDiv.Height = 1;
			MainDiv.TabIndex = 0;
		}
		protected void PrepareInputFiles() {
			if(!Upload.DesignMode)
				PrepareInputFile(TemplateInputFileInfo, -1);
			PrepareInputFile(InputFileInfo, 0);
		}
		protected override void SetInputTabIndex(InputFileInfo info) {
			info.InputControl.TabIndex = -1;
		}
	}
	public class AddUploadButtonsPanelControl : ASPxInternalWebControl {
		private Table mainTable = null;
		private TableCell separatorCell = null;
		private ASPxUploadControl upload = null;
		public AddUploadButtonsPanelControl(ASPxUploadControl upload)
			: base() {
			this.upload = upload;
		}
		public Table MainTable {
			get { return mainTable; }
		}
		public TableCell SeparatorCell {
			get { return separatorCell; }
		}
		public ASPxUploadControl Upload {
			get { return upload; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.mainTable = null;
			this.separatorCell = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.mainTable = RenderUtils.CreateTable();
			Controls.Add(MainTable);
			TableRow row = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(row);
			CreateButtonCells(row);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareButtons();
			if(SeparatorCell != null)
				PrepareSeparatorCells();
		}
		protected void CreateButtonCells(TableRow row) {
			if(Upload.ShowAddRemoveButtons) {
				ButtonTableCell addButtonCell = new ButtonTableCell(Upload, Upload.AddButton, Upload.GetAddButtonImage(), null);
				row.Cells.Add(addButtonCell);
				addButtonCell.ID = Upload.AddButton.GetButtonIDSuffix();
			}
			if(IsButtonsSpacingExist())
				CreateSeparatorCell(row);
			if(Upload.IsUploadButtonVisible()) {
				ButtonTableCell uploadButtonCell = new ButtonTableCell(Upload, Upload.UploadButton, Upload.GetUploadButtonImage(), null);
				row.Cells.Add(uploadButtonCell);
				uploadButtonCell.ID = Upload.UploadButton.GetButtonIDSuffix();
			}
		}
		protected void CreateSeparatorCell(TableRow row) {
			this.separatorCell = RenderUtils.CreateIndentCell();
			row.Cells.Add(SeparatorCell);
		}
		protected void PrepareButtons() {
		}
		protected void PrepareSeparatorCells() {
			RenderUtils.PrepareIndentCell(SeparatorCell, Upload.GetButtonSpacing(), Unit.Empty);
		}
		protected bool IsButtonsSpacingExist() {
			return (Upload.ShowAddRemoveButtons && Upload.IsUploadButtonVisible()
				&& Upload.GetButtonSpacing() != Unit.Pixel(0));
		}
	}
	public class UploadingPanelControl : ASPxInternalWebControl {
		private Table mainTable = null;
		private TableRow progressRow = null;
		private TableCell progressCell = null;
		private UploadProgressBar progressBar = null;
		private TableRow cancelSpacerRow = null;
		private TableCell cancelSpacerCell = null;
		private TableCell cancelPanelCell = null;
		private ASPxUploadControl upload = null;
		public UploadingPanelControl(ASPxUploadControl upload)
			: base() {
			this.upload = upload;
		}
		public Table MainTable {
			get { return mainTable; }
		}
		protected TableRow ProgressRow {
			get { return progressRow; }
		}
		protected TableCell ProgressCell {
			get { return progressCell; }
		}
		protected UploadProgressBar ProgressBar {
			get { return progressBar; }
		}
		public ASPxUploadControl Upload {
			get { return upload; }
		}
		protected TableRow CancelSpacerRow {
			get { return cancelSpacerRow; }
		}
		protected TableCell CancelSpacerCell {
			get { return cancelSpacerCell; }
		}
		protected TableCell CancelPanelCell {
			get { return cancelPanelCell; }
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.mainTable = null;
			this.progressRow = null;
			this.progressCell = null;
			this.progressBar = null;
			this.cancelSpacerRow = null;
			this.cancelSpacerCell = null;
			this.cancelPanelCell = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.mainTable = RenderUtils.CreateTable();
			Controls.Add(MainTable);
			CreateProgressRow();
			if(Upload.ShowCancelButton) {
				CreateCancelPanelCell();
				CreateCancelButtonCell();
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			MainTable.Width = Unit.Percentage(100);
			PrepareProgressRow();
			if(Upload.ShowCancelButton)
				PrepareCancelPanelCell();
		}
		protected void CreateProgressRow() {
			this.progressRow = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(ProgressRow);
			this.progressCell = RenderUtils.CreateTableCell();
			ProgressRow.Cells.Add(ProgressCell);
			this.progressBar = new UploadProgressBar(Upload);
			ProgressBar.RightToLeft = Upload.RightToLeft;
			ProgressBar.ID = Upload.GetProgressBarControlID();
			ProgressCell.Controls.Add(ProgressBar);
		}
		protected void CreateCancelPanelCell() {
			if(Upload.GetCancelButtonsSpacing() != Unit.Pixel(0)) {
				this.cancelSpacerRow = RenderUtils.CreateTableRow();
				MainTable.Rows.Add(CancelSpacerRow);
				this.cancelSpacerCell = RenderUtils.CreateIndentCell();
				cancelSpacerRow.Cells.Add(CancelSpacerCell);
			}
			TableRow cancelRow = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(cancelRow);
			this.cancelPanelCell = RenderUtils.CreateTableCell();
			cancelRow.Cells.Add(CancelPanelCell);
		}
		protected void CreateCancelButtonCell() {
			Table table = RenderUtils.CreateTable();
			CancelPanelCell.Controls.Add(table);
			TableRow tableRow = RenderUtils.CreateTableRow();
			table.Rows.Add(tableRow);
			ButtonTableCell cancelButtonCell = new ButtonTableCell(Upload, Upload.CancelButton, Upload.GetCancelButtonImage(), null);
			tableRow.Cells.Add(cancelButtonCell);
			cancelButtonCell.ID = Upload.CancelButton.GetButtonIDSuffix();
		}
		protected void PrepareProgressRow() {
			ProgressBar.ControlStyle.Reset();
			ProgressBar.IndicatorStyle.Reset();
			ProgressBar.Width = Upload.GetProgressWidth();
			ProgressBar.Height = Upload.GetProgressHeight();
			ProgressBar.ControlStyle.CopyFrom(Upload.GetProgressStyle());
			ProgressBar.IndicatorStyle.CopyFrom(Upload.GetProgressBarIndicatorStyle());
			if(DesignMode) {
				RenderUtils.SetStyleAttribute(ProgressCell, "padding-top", Upload.GetUploadingDesignSpacing(), "");
				ProgressBar.Position = Upload.GetProgressBarDesignValue();
			}
		}
		protected void PrepareCancelPanelCell() {
			if(CancelSpacerRow != null)
				RenderUtils.PrepareIndentCell(CancelSpacerCell, Unit.Empty, Upload.GetCancelButtonsSpacing());
			string alignAttrValue = Upload.GetCancelButtonHorizontalPosition().ToString().ToLower();
			RenderUtils.SetStringAttribute(CancelPanelCell, "align", alignAttrValue);
		}
	}
	public class ButtonTableCell : InternalTableCell {
		private UploadControlButtonPropertiesBase buttonProperties;
		private ImagePropertiesBase image;
		private UploadControlButtonControl buttonControl;
		private string onClickHandler = "";
		private ASPxUploadControl upload = null;
		private int buttonIndex = -1;
		private bool isAccessibilityEnabled = true;
		public ButtonTableCell(ASPxUploadControl upload, UploadControlButtonPropertiesBase buttonProperties, ImagePropertiesBase image,
			string onClickHandler)
			: this(upload, buttonProperties, image, onClickHandler, -1, true) {
		}
		public ButtonTableCell(ASPxUploadControl upload, UploadControlButtonPropertiesBase buttonProperties, ImagePropertiesBase image,
			string onClickHandler, int buttonIndex)
			: this(upload, buttonProperties, image, onClickHandler, buttonIndex, true) {
		}
		public ButtonTableCell(ASPxUploadControl upload, UploadControlButtonPropertiesBase buttonProperties, ImagePropertiesBase image,
			string onClickHandler, int buttonIndex, bool isAccessibilityEnabled)
			: base() {
			this.buttonProperties = buttonProperties;
			this.image = image;
			this.onClickHandler = onClickHandler;
			this.upload = upload;
			this.buttonIndex = buttonIndex;
			this.isAccessibilityEnabled = isAccessibilityEnabled;
		}
		public UploadControlButtonPropertiesBase ButtonProperties {
			get { return buttonProperties; }
		}
		protected ImagePropertiesBase Image {
			get { return image; }
		}
		protected UploadControlButtonControl ButtonControl {
			get { return buttonControl; }
		}
		protected string OnClickHandler {
			get { return onClickHandler; }
		}
		protected ASPxUploadControl Upload {
			get { return this.upload; }
		}
		protected int ButtonIndex {
			get { return buttonIndex; }
		}
		protected bool IsAccessibilityEnabled {
			get { return isAccessibilityEnabled; }
		}
		protected override void CreateControlHierarchy() {
			this.buttonControl = new UploadControlButtonControl(Upload, ButtonProperties, Upload.HtmlEncode(ButtonProperties.Text), Image,
				ButtonProperties.ImagePosition, ButtonIndex, IsAccessibilityEnabled);
			Controls.Add(ButtonControl);
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyleBase style = (ButtonProperties is BrowseButtonProperties) ? Upload.GetBrowseButtonStyle() : Upload.GetButtonStyle();
			ButtonControl.ButtonImageSpacing = Upload.GetButtonImageSpacing();
			ButtonControl.ButtonStyle = style;
			style.AssignToControl(this, AttributesRange.Common);
			if(!style.Width.IsEmpty) {
				ApplyWidth(style.Width);
			}
			RenderUtils.SetPaddings(this, (ButtonProperties is BrowseButtonProperties) ? Upload.GetBrowseButtonPaddings() : Upload.GetButtonPaddings());
			if (Upload.IsEnabled())
				RenderUtils.SetStringAttribute(this, "onclick", OnClickHandler);
			if (Upload.IsNativeRender())
				RenderUtils.AppendDefaultDXClassName(this, UploadControlStyles.ButtonSystemStyleName);
		}
		protected void ApplyWidth(Unit width) {
			var filler = RenderUtils.CreateDiv();
			filler.Width = width;
			Controls.Add(filler);
			this.Width = width;
		}
	}
	public class UploadControlButtonControl : SimpleButtonControl {
		private UploadControlButtonPropertiesBase buttonProperties = null;
		private ImagePropertiesBase imageProperies = null;
		private ASPxUploadControl upload = null;
		private int buttonIndex = -1;
		private bool isAccessibilityEnabled = true;
		protected internal new HyperLink Hyperlink {
			get { return base.Hyperlink; }
		}
		protected internal new Image ImageControl {
			get { return base.ImageControl; }
		}
		protected ASPxUploadControl Upload {
			get { return upload; }
		}
		protected UploadControlButtonPropertiesBase ButtonProperties {
			get { return buttonProperties; }
		}
		protected ImagePropertiesBase ImageProperties {
			get { return imageProperies; }
		}
		protected int ButtonIndex {
			get { return buttonIndex; }
		}
		protected bool IsAccessibilityEnabled {
			get { return isAccessibilityEnabled; }
		}
		public UploadControlButtonControl(ASPxUploadControl upload, UploadControlButtonPropertiesBase buttonProperties, string text, ImagePropertiesBase image,
			ImagePosition imagePosition, int buttonIndex)
			: this(upload, buttonProperties, text, image, imagePosition, buttonIndex, true) {
		}
		public UploadControlButtonControl(ASPxUploadControl upload, UploadControlButtonPropertiesBase buttonProperties, string text, ImagePropertiesBase image,
			ImagePosition imagePosition, int buttonIndex, bool isAccessibilityEnabled)
			: base(text, image, imagePosition, RenderUtils.AccessibilityEmptyUrl) {
			this.buttonProperties = buttonProperties;
			this.imageProperies = image;
			this.upload = upload;
			this.buttonIndex = buttonIndex;
			this.isAccessibilityEnabled = isAccessibilityEnabled;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(Hyperlink != null && !IsAccessibilityEnabled)
				Hyperlink.TabIndex = -1;
			if(ImageControl != null) {
				if(ButtonProperties != null)
					ImageControl.ID = Upload.GetButtonImageID(ButtonProperties, ButtonIndex);
				ImageProperties.AssignToControl(ImageControl, DesignMode, !Upload.Enabled);
			}
		}
	}
	[ToolboxItem(false)]
	public class UploadProgressBar : ASPxProgressBarBase {
		ASPxUploadControl upload = null;
		public UploadProgressBar(ASPxUploadControl upload)
			: base() {
			this.upload = upload;
			EnableViewState = false;
			ProgressBarSettings.Assign(Upload.ProgressBarSettings);
		}
		protected ASPxUploadControl Upload {
			get { return upload; }
		}
		protected override ISkinOwner GetParentSkinOwner() {
			return Upload;
		}
	}
	public class UploadFileListControl : ASPxInternalWebControl {
		ASPxUploadControl upload = null;
		private const string FileListTableClassName = "dxucFileList";
		public UploadFileListControl(ASPxUploadControl upload)
			: base() {
			this.upload = upload;
		}
		protected ASPxUploadControl Upload {
			get { return upload; }
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Ul;
			}
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = Upload.GetFileListTableID();
			Controls.Add(new UploadFileListRowTemplate(Upload));
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CssClass = FileListTableClassName;
			RenderUtils.SetStyleStringAttribute(this, "list-style-type", "none");
		}
	}
	public class UploadFileListRowTemplate : ASPxInternalWebControl {
		ASPxUploadControl upload = null;
		UploadFileListButton removeButton = null;
		ASPxLabel nameLabel = new ASPxLabel();
		WebControl fileNameContainerDiv = RenderUtils.CreateDiv();
		WebControl buttonDiv = RenderUtils.CreateDiv();
		WebControl progressBarDiv = RenderUtils.CreateDiv();
		private const string NameCellClassName = "dxucNameCell";
		private const string RemoveButtonCellSystemClassName = "dxRB";
		private const string ProgressBarDivClassName = "dxucBarCell";
		protected UploadFileListButton RemoveButton {
			get { return removeButton; }
		}
		protected WebControl FileNameContainerDiv {
			get { return fileNameContainerDiv; }
		}
		protected WebControl ButtonDiv {
			get { return buttonDiv; }
		}
		protected WebControl ProgressBarDiv {
			get { return progressBarDiv; }
		}
		protected ASPxLabel NameLabel {
			get { return nameLabel; }
		}
		protected ASPxUploadControl Upload {
			get { return upload; }
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Li;
			}
		}
		public UploadFileListRowTemplate(ASPxUploadControl upload)
			: base() {
			this.upload = upload;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Controls.Add(FileNameContainerDiv);
			this.removeButton = new UploadFileListButton(Upload, Upload.RemoveButton, Upload.GetRemoveButtonImage());
			Controls.Add(RemoveButton);
			Controls.Add(ProgressBarDiv);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.SetVisibility(this, false, true);
			ID = Upload.GetFileListRowTemplateID();
			FileNameContainerDiv.CssClass = NameCellClassName;
			ProgressBarDiv.CssClass = ProgressBarDivClassName;
			RenderUtils.AppendDefaultDXClassName(RemoveButton, RemoveButtonCellSystemClassName);
			UploadControlFileListItemStyle itemStyle = Upload.CreateFileListItemStyle();
			FileNameContainerDiv.Controls.Add(CreateFileNameDiv(itemStyle));
			UploadFileListProgressBar progressBar = new UploadFileListProgressBar(Upload);
			progressBar.DisplayMode = ProgressBarDisplayMode.Custom;
			progressBar.ShowPosition = false;
			ProgressBarDiv.Controls.Add(progressBar);
		}
		protected void CreateText(WebControl parent) {
			ASPxLabel nameLabel = new ASPxLabel();
			parent.Controls.Add(nameLabel);
		}
		protected WebControl CreateFileNameDiv(UploadControlFileListItemStyle ItemStyle) {
			WebControl nameDiv = RenderUtils.CreateDiv();
			ItemStyle.AssignToControl(nameDiv);
			CreateText(nameDiv);
			return nameDiv;
		}
	}
	[ToolboxItem(false)]
	public class UploadFileListButton : ASPxWebControl, ISkinOwner {
		private UploadControlButtonPropertiesBase buttonProperties;
		private ImagePropertiesBase image;
		private UploadControlButtonControl buttonControl;
		private ASPxUploadControl upload = null;
		public UploadFileListButton(ASPxUploadControl upload, UploadControlButtonPropertiesBase buttonProperties, ImagePropertiesBase image)
			: base() {
			this.buttonProperties = buttonProperties;
			this.image = image;
			this.upload = upload;
		}
		public UploadControlButtonPropertiesBase ButtonProperties {
			get { return buttonProperties; }
		}
		protected ImagePropertiesBase Image {
			get { return image; }
		}
		protected UploadControlButtonControl ButtonControl {
			get { return buttonControl; }
		}
		protected ASPxUploadControl Upload {
			get { return this.upload; }
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.buttonControl = new UploadControlButtonControl(Upload, ButtonProperties, Upload.HtmlEncode(ButtonProperties.Text), Image,
				ButtonProperties.ImagePosition, -1, false);
			Controls.Add(ButtonControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			AppearanceStyleBase style = Upload.GetButtonStyle();
			ButtonControl.ButtonImageSpacing = Upload.GetButtonImageSpacing();
			ButtonControl.ButtonStyle = style;
			style.AssignToControl(this, AttributesRange.Common);
			RenderUtils.SetPaddings(this, Upload.GetButtonPaddings());
			if(Upload.IsNativeRender())
				RenderUtils.AppendDefaultDXClassName(this, UploadControlStyles.ButtonSystemStyleName);
		}
	}
	public class UploadFileListProgressBar : ASPxProgressBarBase {
		ASPxUploadControl upload = null;
		private const string cssClassName = "dxucFL-Progress";
		public UploadFileListProgressBar(ASPxUploadControl upload)
			: base() {
			this.upload = upload;
			EnableViewState = false;
		}
		protected ASPxUploadControl Upload {
			get { return upload; }
		}
		protected override ISkinOwner GetParentSkinOwner() {
			return Upload;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ControlStyle.Reset();
			IndicatorStyle.Reset();
			ID = Upload.GetFileListProgressDefaultID();
			Width = Upload.GetFileListProgressWidth();
			Height = Upload.GetFileListProgressHeight();
			ControlStyle.CopyFrom(Upload.GetProgressStyle());
			IndicatorStyle.CopyFrom(Upload.GetProgressBarIndicatorStyle());
			this.ClientVisible = false;
			RenderUtils.AppendDefaultDXClassName(this, cssClassName);
		}
	}
}
