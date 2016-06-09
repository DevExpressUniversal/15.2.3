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
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	using System.Collections;
	using DevExpress.ASPxSpellChecker.Native;
	using DevExpress.Utils;
	using DevExpress.Web;
	using DevExpress.Web.ASPxHtmlEditor.Localization;
	using DevExpress.Web.ASPxHtmlEditor.Rendering;
	using DevExpress.Web.ASPxSpellChecker.Internal;
	public abstract class BaseControl : ASPxInternalWebControl {
		private ASPxHtmlEditor htmlEditor;
		public BaseControl(ASPxHtmlEditor htmlEditor) {
			this.htmlEditor = htmlEditor;
		}
		protected ASPxHtmlEditor HtmlEditor { get { return htmlEditor; } }
		protected ASPxHtmlEditorRenderHelper RenderHelper { get { return HtmlEditor.RenderHelper; } }
		protected ASPxHtmlEditorScripts Scripts { get { return RenderHelper.Scripts; } }
	}
	public class HtmlEditorErrorFrameControl : ASPxInternalWebControl {
		private ASPxHtmlEditor htmlEditor;
		private Table mainTable;
		private TableCell mainTableCell;
		private Table errorTable;
		private TableCell errorTextCell;
		private TableCell errorSpacingCell;
		private TableCell errorCloseButtonCell;
		private SimpleButtonControl errorFrameCloseButton;
		public HtmlEditorErrorFrameControl(ASPxHtmlEditor htmlEditor) {
			this.htmlEditor = htmlEditor;
		}
		protected ASPxHtmlEditor HtmlEditor {
			get { return this.htmlEditor; }
		}
		protected bool IsRightToLeft {
			get { return (HtmlEditor as ISkinOwner).IsRightToLeft(); }
		}
		protected Table MainTable { get { return mainTable; } }
		protected TableCell MainTableCell { get { return mainTableCell; } }
		protected Table ErrorTable { get { return errorTable; } }
		protected TableCell ErrorTextCell { get { return errorTextCell; } }
		protected TableCell ErrorSpacingCell { get { return errorSpacingCell; } }
		protected TableCell ErrorCloseButtonCell { get { return errorCloseButtonCell; } }
		protected SimpleButtonControl ErrorFrameCloseButton { get { return errorFrameCloseButton; } }
		protected override void ClearControlFields() {
			this.mainTable = null;
			this.mainTableCell = null;
			this.errorTable = null;
			this.errorTextCell = null;
			this.errorSpacingCell = null;
			this.errorCloseButtonCell = null;
			this.errorFrameCloseButton = null;
		}
		protected override void CreateControlHierarchy() {
			CreateMainTable();
			CreateTextAndButtonCells();
		}
		protected void CreateMainTable() {
			this.mainTable = RenderUtils.CreateTable();
			Controls.Add(MainTable);
			MainTable.ID = HtmlEditor.GetErrorFrameID();
			TableRow mainRow = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(mainRow);
			this.mainTableCell = RenderUtils.CreateTableCell();
			mainRow.Cells.Add(MainTableCell);
			RenderUtils.AppendDefaultDXClassName(MainTableCell, HtmlEditor.GetCssClassNamePrefix());
		}
		protected void CreateTextAndButtonCells() {
			this.errorTable = RenderUtils.CreateTable();
			MainTableCell.Controls.Add(ErrorTable);
			TableRow row = RenderUtils.CreateTableRow();
			ErrorTable.Rows.Add(row);
			this.errorTextCell = RenderUtils.CreateTableCell();
			row.Cells.Add(ErrorTextCell);
			ErrorTextCell.ID = HtmlEditor.GetErrorTextCellID();
			this.errorSpacingCell = RenderUtils.CreateTableCell();
			row.Cells.Add(ErrorSpacingCell);
			this.errorCloseButtonCell = RenderUtils.CreateTableCell();
			row.Cells.Add(ErrorCloseButtonCell);
			ErrorCloseButtonCell.ID = HtmlEditor.GetErrorFrameCloseButtonID();
			this.errorFrameCloseButton = new SimpleButtonControl();
			ErrorCloseButtonCell.Controls.Add(ErrorFrameCloseButton);
		}
		protected override void PrepareControlHierarchy() {
			PrepareMainTable();
			PrepareTextAndButtonCells();
		}
		protected void PrepareMainTable() {
			AppearanceStyleBase style = HtmlEditor.GetErrorFrameStyle();
			style.AssignToControl(MainTable, AttributesRange.Common | AttributesRange.Font);
			RenderUtils.SetStyleStringAttribute(MainTable, "Width", "100%");
			style.AssignToControl(MainTableCell, AttributesRange.Cell);
			RenderUtils.SetVisibility(MainTable, HtmlEditor.IsErrorFrameVisible(), true);
			if(IsRightToLeft)
				MainTable.Attributes["dir"] = "rtl";
		}
		protected void PrepareTextAndButtonCells() {
			RenderUtils.SetStyleStringAttribute(ErrorTable, "width", "100%");
			RenderUtils.SetStyleStringAttribute(ErrorTextCell, "width", "100%");
			ErrorTextCell.Controls.Add(RenderUtils.CreateLiteralControl(HtmlEditor.HtmlEncode(HtmlEditor.ErrorText)));
			HtmlEditor.GetErrorFrameCloseButtonStyle().AssignToControl(ErrorCloseButtonCell);
			RenderUtils.SetStringAttribute(ErrorCloseButtonCell, "onclick", HtmlEditor.GetErrorFrameCloseButtonOnClick());
			ErrorSpacingCell.Width = HtmlEditor.GetErrorFrameImageSpacing();
			ErrorSpacingCell.Text = "&nbsp;";
			ErrorFrameCloseButton.ButtonImageID = HtmlEditor.GetErrorFrameCloseButtonImageID();
			ErrorFrameCloseButton.ButtonImage = HtmlEditor.GetErrorFrameCloseButtonImage();
			if(HtmlEditor.IsAccessibilityCompliantRender(true))
				ErrorFrameCloseButton.ButtonUrl = RenderUtils.AccessibilityEmptyUrl;
		}
	}
	[ToolboxItem(false)]
	public sealed class StatusBarTabControl : ASPxTabControl {
		public StatusBarTabControl(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
			ParentSkinOwner = htmlEditor;
		}
		public override TabPosition TabPosition {
			get { return TabPosition.Bottom; }
			set { }
		}
		internal new TCControlBase MainControl {
			get { return base.MainControl; }
		}
		protected override bool HasContentArea() {
			return false;
		}
	}
	public class StatusBarControl : BaseControl {
		private Table table;
		private TableCell tabControlCell;
		private Image sizeGrip;
		private WebControl sizeGripContainer;
		private StatusBarTabControl tabControl;
		private Tab designViewTab;
		private Tab htmlViewTab;
		private Tab previewTab;
		public StatusBarControl(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected internal StatusBarTabControl TabControl {
			get { return tabControl; }
		}
		protected override void ClearControlFields() {
			this.table = null;
			this.tabControlCell = null;
			this.tabControl = null;
			this.designViewTab = null;
			this.htmlViewTab = null;
			this.previewTab = null;
			this.sizeGrip = null;
			this.sizeGripContainer = null;
		}
		protected override void CreateControlHierarchy() {
			if(HtmlEditor.ShowViewSwitcher) {
				CreateTableStructure();
				CreateTabControl();
			}
			if(HtmlEditor.SettingsResize.AllowResize)
				CreateSizeGrip();
		}
		private void CreateTableStructure() {
			this.table = RenderUtils.CreateTable(false);
			Controls.Add(this.table);
			this.table.Width = Unit.Percentage(100);
			TableRow row = RenderUtils.CreateTableRow();
			this.table.Rows.Add(row);
			this.tabControlCell = RenderUtils.CreateTableCell();
			this.tabControlCell.Width = Unit.Percentage(100);
			row.Cells.Add(this.tabControlCell);
		}
		private void CreateTabControl() {
			this.tabControl = new StatusBarTabControl(HtmlEditor);
			this.TabControl.EncodeHtml = HtmlEditor.EncodeHtml;
			this.tabControl.ID = RenderHelper.TabControlID;
			this.tabControlCell.Controls.Add(this.tabControl);
			if(HtmlEditor.Settings.AllowDesignViewInternal) {
				this.designViewTab = new Tab();
				this.tabControl.Tabs.Add(this.designViewTab);
			}
			if(HtmlEditor.Settings.AllowHtmlViewInternal) {
				this.htmlViewTab = new Tab();
				this.tabControl.Tabs.Add(this.htmlViewTab);
			}
			if(HtmlEditor.Settings.AllowPreviewInternal) {
				this.previewTab = new Tab();
				this.tabControl.Tabs.Add(this.previewTab);
			}
		}
		private void CreateSizeGrip() {
			this.sizeGrip = RenderUtils.CreateImage();
			this.sizeGrip.ID = RenderHelper.SizeGripID;
			if(this.tabControl != null) {
				this.sizeGripContainer = RenderUtils.CreateDiv();
				this.sizeGripContainer.Controls.Add(this.sizeGrip);
				Controls.Add(this.sizeGripContainer);
			}
			else
				Controls.Add(this.sizeGrip);
		}
		protected override void PrepareControlHierarchy() {
			if(HtmlEditor.ShowViewSwitcher) {
				PrepareTableStructure();
				PrepareTabControl();
				PrepareTabs();
			}
			if(this.sizeGrip != null)
				PrepareSizeGrip();
		}
		private void PrepareTableStructure() {
			HtmlEditor.GetStatusBarStyle().AssignToControl(this.tabControlCell);
			HtmlEditor.GetStatusBarPaddings().AssignToControl(this.tabControlCell);
		}
		private void PrepareTabControl() {
			this.tabControl.Width = Unit.Percentage(100);
			this.tabControl.Paddings.Assign(HtmlEditor.GetStatusBarTabControlPaddings());
			this.tabControl.ClientSideEvents.ActiveTabChanged = Scripts.ChangeViewHandler;
			this.tabControl.ClientSideEvents.ActiveTabChanging = Scripts.ChangingViewHandler;
			this.tabControl.TabStyle.Assign(HtmlEditor.GetStatusBarTabStyle());
			this.tabControl.ActiveTabStyle.Assign(HtmlEditor.GetStatusBarActiveTabStyle());
			this.tabControl.ContentStyle.Assign(HtmlEditor.GetStatusBarContentStyle());
			this.tabControl.TabSpacing = HtmlEditor.GetStatusBarTabSpacing();
			SetActiveTab();
		}
		private void SetActiveTab() {
			switch(HtmlEditor.ActiveView) {
				case HtmlEditorView.Design:
					this.tabControl.ActiveTab = this.designViewTab;
					break;
				case HtmlEditorView.Html:
					this.tabControl.ActiveTab = this.htmlViewTab;
					break;
				case HtmlEditorView.Preview:
					this.tabControl.ActiveTab = this.previewTab;
					break;
				default:
					throw new InvalidOperationException("Invalid view mode.");
			}
		}
		private void PrepareTabs() {
			if(this.designViewTab != null) {
				this.designViewTab.Name = RenderHelper.DesignViewTabName;
				this.designViewTab.Text = HtmlEditor.SettingsText.DesignViewTab;
			}
			if(this.htmlViewTab != null) {
				this.htmlViewTab.Name = RenderHelper.HtmlViewTabName;
				this.htmlViewTab.Text = HtmlEditor.SettingsText.HtmlViewTab;
			}
			if(this.previewTab != null) {
				this.previewTab.Name = RenderHelper.PreviewTabName;
				this.previewTab.Text = HtmlEditor.SettingsText.PreviewTab;
			}
		}
		private void PrepareSizeGrip() {
			HtmlEditor.GetStatusBarSizeGripStyle().AssignToControl(this.sizeGrip);
			HtmlEditor.GetStatusBarSizeGripImageProperties().AssignToControl(this.sizeGrip, DesignMode);
			if(this.tabControl != null)
				HtmlEditor.StylesStatusBar.GetSizeGripContainerStyle().AssignToControl(this.sizeGripContainer);
			RenderUtils.AppendMSTouchDraggableClassNameIfRequired(this.sizeGrip);
		}
		internal HtmlEditorView GetModeByTab(Tab tab) {
			if(tab == this.designViewTab)
				return HtmlEditorView.Design;
			else if(tab == this.htmlViewTab)
				return HtmlEditorView.Html;
			else if(tab == this.previewTab)
				return HtmlEditorView.Preview;
			else
				throw new ArgumentException("tab");
		}
	}
	[ToolboxItem(false)]
	public sealed class HtmlViewEditControl : ASPxMemo {
		private ASPxHtmlEditor htmlEditor;
		private const string TextChangedEventHandler = "ASPx.HEHtmlViewHtmlChanged('{0}');";
		public HtmlViewEditControl(ASPxHtmlEditor htmlEditor) {
			this.htmlEditor = htmlEditor;
			EnableFocusedStyle = false;
			ParentSkinOwner = htmlEditor;
		}
		internal ASPxHtmlEditor HtmlEditor { get { return htmlEditor; } }
		protected override bool SendValueToServer { get { return false; } }
		protected override DevExpress.Web.Internal.MemoControl CreateMemoControl() {
			return new HtmlViewEditMemoControl(this);
		}
		protected override string GetOnTextChanged() {
			return string.Format(TextChangedEventHandler, htmlEditor.ClientID);
		}
	}
	public class HtmlViewEditMemoControl : DevExpress.Web.Internal.MemoControl {
		private WebControl designTimeContentControl;
		private HtmlEditorErrorFrameControl errorFrameControl;
		private Table textAreaTable;
		private TableCell textAreaCell;
		public HtmlViewEditMemoControl(HtmlViewEditControl htmlViewEditControl)
			: base(htmlViewEditControl) {
		}
		internal ASPxHtmlEditor HtmlEditor { get { return (Edit as HtmlViewEditControl).HtmlEditor; } }
		private Table TextAreaTable { get { return textAreaTable; } }
		private TableCell TextAreaCell { get { return textAreaCell; } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.designTimeContentControl = null;
			this.errorFrameControl = null;
			this.textAreaTable = null;
			this.textAreaCell = null;
		}
		protected override void CreateMainCellContent(TableCell mainCell) {
			if(!Edit.DesignMode) {
				CreateTextAreaCell(mainCell);
				base.CreateMainCellContent(TextAreaCell);
			}
			else {
				if(HtmlEditor.IsErrorFrameVisible())
					CreateErrorFrame(mainCell);
				CreateDesignTimeMainCellContent(mainCell);
			}
		}
		protected void CreateTextAreaCell(TableCell cell) {
			this.textAreaTable = RenderUtils.CreateTable();
			cell.Controls.Add(TextAreaTable);
			TableRow row = RenderUtils.CreateTableRow();
			TextAreaTable.Rows.Add(row);
			this.textAreaCell = RenderUtils.CreateTableCell();
			row.Cells.Add(TextAreaCell);
		}
		protected void CreateErrorFrame(Control container) {
			this.errorFrameControl = new HtmlEditorErrorFrameControl(HtmlEditor);
			container.Controls.Add(this.errorFrameControl);
		}
		protected override void PrepareMainCellContent() {
			if(HtmlEditor.TabIndex > 0)
				Edit.TabIndex = HtmlEditor.TabIndex;
			if(!Edit.DesignMode) {
				PrepareTextAreaCell();
				base.PrepareMainCellContent();
			}
			else
				PrepareDesignTimeMainCellContent();
		}
		protected void PrepareTextAreaCell() {
			TextAreaTable.Width = Unit.Percentage(100);
			HtmlViewEditControl htmlViewEdit = (HtmlViewEditControl)Edit;
			AppearanceStyle htmlViewAreaStyle = htmlViewEdit.HtmlEditor.GetHtmlViewAreaStyle();
			htmlViewAreaStyle.AssignToControl(TextAreaCell, AttributesRange.Cell);
		}
		private void CreateDesignTimeMainCellContent(TableCell mainCell) {
			this.designTimeContentControl = RenderUtils.CreateDiv();
			mainCell.Controls.Add(this.designTimeContentControl);
		}
		protected override void PrepareMainCell(TableCell editCell) {
			base.PrepareMainCell(editCell);
			editCell.VerticalAlign = VerticalAlign.Top;
			RenderUtils.SetStyleStringAttribute(editCell, "padding", "0px");
		}
		private void PrepareDesignTimeMainCellContent() {
			this.designTimeContentControl.Width = Unit.Percentage(100);
			this.designTimeContentControl.Height = Unit.Percentage(100);
			HtmlViewEditControl htmlViewEdit = (HtmlViewEditControl)Edit;
			AppearanceStyle htmlViewAreaStyle = htmlViewEdit.HtmlEditor.GetHtmlViewAreaStyle();
			htmlViewAreaStyle.Paddings.Assign(Paddings.NullPaddings);
			htmlViewAreaStyle.Border.Assign(Border.NullBorder);
			htmlViewAreaStyle.BackgroundImage.Reset();
			htmlViewAreaStyle.BackColor = System.Drawing.Color.Transparent;
			htmlViewAreaStyle.AssignToControl(this.designTimeContentControl);
			this.designTimeContentControl.Controls.Clear();
			this.designTimeContentControl.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(htmlViewEdit.HtmlEditor.Html)));
		}
	}
	public class HtmlViewSourceEditorControl : BaseControl {
		private WebControl designTimeContentControl;
		WebControl sourceEditorContainer;
		private Table mainTable;
		private TableCell mainCell;
		public HtmlViewSourceEditorControl(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected WebControl SourceEditorContainer { get { return sourceEditorContainer; } }
		protected Table MainTable { get { return mainTable; } }
		protected TableCell MainCell { get { return mainCell; } }
		protected override void ClearControlFields() {
			this.mainCell = null;
			this.mainTable = null;
			this.sourceEditorContainer = null;
		}
		protected override void CreateControlHierarchy() {
			CreateMainTable();
			if(!DesignMode)
				CreateSourceEditorContainer(MainCell);
			else {
				if(HtmlEditor.IsErrorFrameVisible())
					CreateErrorFrame(MainCell);
				CreateDesignTimeEditArea(MainCell);
			}
		}
		protected void CreateMainTable() {
			this.mainTable = RenderUtils.CreateTable(true);
			Controls.Add(MainTable);
			TableRow row = RenderUtils.CreateTableRow();
			mainTable.Rows.Add(row);
			mainCell = RenderUtils.CreateTableCell();
			row.Cells.Add(MainCell);
		}
		protected void CreateErrorFrame(Control container) {
			container.Controls.Add(new HtmlEditorErrorFrameControl(HtmlEditor));
		}
		protected virtual void CreateSourceEditorContainer(Control container) {
			this.sourceEditorContainer = RenderUtils.CreateDiv();
			container.Controls.Add(SourceEditorContainer);
		}
		protected void CreateDesignTimeEditArea(Control container) {
			this.designTimeContentControl = new WebControl(HtmlTextWriterTag.Div);
			container.Controls.Add(designTimeContentControl);
		}
		protected override void PrepareControlHierarchy() {
			PrepareMainTable();
			if(!DesignMode)
				PrepareSourceEditor();
			else
				PrepareDesignTimeEditArea();
		}
		protected void PrepareSourceEditor() {
			SourceEditorContainer.ID = RenderHelper.SourceEditorID;
			AppearanceStyleBase viewAreaStyle = HtmlEditor.GetHtmlViewAreaStyle();
			SourceEditorContainer.CssClass = "dxheSourceEditorSys";
			viewAreaStyle.AssignToControl(SourceEditorContainer);
		}
		protected void PrepareMainTable() {
			MainTable.Width = Unit.Percentage(100);
			MainTable.ID = RenderHelper.HtmlViewEditID;
			MainTable.Height = Unit.Percentage(100);
			MainCell.Width = Unit.Percentage(100);
		}
		private void PrepareDesignTimeEditArea() {
			this.designTimeContentControl.Width = Unit.Percentage(100);
			this.designTimeContentControl.Height = Unit.Percentage(100);
			AppearanceStyle htmlViewAreaStyle = HtmlEditor.GetHtmlViewAreaStyle();
			htmlViewAreaStyle.Paddings.Assign(Paddings.NullPaddings);
			htmlViewAreaStyle.Border.Assign(Border.NullBorder);
			htmlViewAreaStyle.BackgroundImage.Reset();
			htmlViewAreaStyle.BackColor = System.Drawing.Color.Transparent;
			htmlViewAreaStyle.AssignToControl(this.designTimeContentControl);
			this.designTimeContentControl.Controls.Clear();
			this.designTimeContentControl.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(HtmlEditor.Html)));
		}
	}
	public class ViewAreaControl : BaseControl {
		private string cellID = "";
		private string iframeID = "";
		private string iframeName = "";
		private string iframeSrc = "";
		private AppearanceStyleBase areaStyle;
		private bool isVisible = false;
		private WebControl iframe;
		private Table mainTable;
		private TableCell mainCell;
		private WebControl designTimeEditAreaControl;
		private HtmlEditorErrorFrameControl errorFrameControl;
		public ViewAreaControl(ASPxHtmlEditor htmlEditor, string iframeID, string iframeName, string iframeSrc,
			AppearanceStyleBase areaStyle, string cellID, bool isVisible)
			: base(htmlEditor) {
			this.iframeID = iframeID;
			this.iframeName = iframeName;
			this.iframeSrc = iframeSrc;
			this.areaStyle = areaStyle;
			this.cellID = cellID;
			this.isVisible = isVisible;
		}
		protected string CellID { get { return cellID; } }
		protected string IFrameID { get { return iframeID; } }
		protected string IFrameName { get { return iframeName; } }
		protected string IFrameSrc { get { return iframeSrc; } }
		protected AppearanceStyleBase AreaStyle { get { return areaStyle; } }
		protected new bool IsVisible { get { return isVisible; } }
		protected WebControl IFrame { get { return iframe; } }
		protected Table MainTable { get { return mainTable; } }
		protected TableCell MainCell { get { return mainCell; } }
		protected override void ClearControlFields() {
			this.mainCell = null;
			this.mainTable = null;
			this.iframe = null;
			this.designTimeEditAreaControl = null;
			this.errorFrameControl = null;
		}
		protected override void CreateControlHierarchy() {
			CreateMainTable();
			if(!DesignMode)
				CreateIFrame(MainCell);
			else {
				if(HtmlEditor.IsErrorFrameVisible())
					CreateErrorFrame(MainCell);
				CreateDesignTimeEditArea(MainCell);
			}
		}
		protected void CreateMainTable() {
			this.mainTable = RenderUtils.CreateTable(true);
			Controls.Add(MainTable);
			TableRow row = RenderUtils.CreateTableRow();
			mainTable.Rows.Add(row);
			mainCell = RenderUtils.CreateTableCell();
			mainCell.ID = CellID;
			row.Cells.Add(MainCell);
		}
		protected void CreateErrorFrame(Control container) {
			this.errorFrameControl = new HtmlEditorErrorFrameControl(HtmlEditor);
			container.Controls.Add(this.errorFrameControl);
		}
		protected virtual void CreateIFrame(Control container) {
			this.iframe = RenderUtils.CreateIFrame(IFrameID, IFrameName, IFrameSrc);
			container.Controls.Add(IFrame);
		}
		protected void CreateDesignTimeEditArea(Control container) {
			this.designTimeEditAreaControl = new WebControl(HtmlTextWriterTag.Div);
			this.designTimeEditAreaControl.Width = Unit.Percentage(100);
			this.designTimeEditAreaControl.Height = Unit.Percentage(100);
			container.Controls.Add(this.designTimeEditAreaControl);
		}
		protected override void PrepareControlHierarchy() {
			PrepareMainTable();
			PrepareMainCell();
			if(IFrame != null)
				PrepareIFrame(IFrame, AreaStyle);
			if (this.designTimeEditAreaControl != null)
				PrepareDesignTimeEditArea();
		}
		protected void PrepareIFrame(WebControl iFrame, AppearanceStyleBase style) {
			if(iFrame == null)
				return;
			style.AssignToControl(iFrame);
			Paddings.NullPaddings.AssignToControl(iFrame);
			Border.NullBorder.AssignToControl(iFrame);
			iFrame.Width = Unit.Percentage(100);
			iFrame.Height = 0;
			RenderUtils.SetStyleStringAttribute(MainTable, "display", "none");
			if(HtmlEditor.TabIndex > 0)
				iframe.TabIndex = HtmlEditor.TabIndex;
		}
		protected void PrepareMainTable() {
			MainTable.Width = Unit.Percentage(100);
			MainCell.Width = Unit.Percentage(100);
			if(!Browser.IsOpera) {
				MainTable.Height = Unit.Percentage(100);
				MainCell.Height = Unit.Percentage(100);
				MainCell.VerticalAlign = VerticalAlign.Top;
			}
			RenderUtils.SetVisibility(MainTable, IsVisible || Browser.IsOpera, true); 
		}
		protected void PrepareMainCell() {
			AreaStyle.AssignToControl(MainCell, true);
		}
		private void PrepareDesignTimeEditArea() {
			this.designTimeEditAreaControl.Controls.Clear();
			string html = HtmlEditor.Html;
			this.designTimeEditAreaControl.Controls.Add(new LiteralControl(html));
		}
	}
	public class DesignViewAreaControl : ViewAreaControl {
		HtmlEditorPasteOptionsBarControl pasteOptionsBar;
		BaseControl tagInspector;
		public DesignViewAreaControl(ASPxHtmlEditor htmlEditor, string iframeID, string iframeName, string iframeSrc,
			AppearanceStyleBase areaStyle, string cellID, bool isVisible)
			: base(htmlEditor, iframeID, iframeName, iframeSrc, areaStyle, cellID, isVisible) {
				pasteOptionsBar = null;
		}
		protected override void CreateControlHierarchy() {
			CreateMainTable();
			if(HtmlEditor.Settings.ShowTagInspector) {
				this.tagInspector = (DesignMode ? (new DesignTagInspectorControl(HtmlEditor)) : (BaseControl)(new TagInspectorControl(HtmlEditor)));
				MainCell.Controls.Add(this.tagInspector);
			}
			if(!DesignMode) {
				if(HtmlEditor.SettingsHtmlEditing.EnablePasteOptions) {
					this.pasteOptionsBar = new HtmlEditorPasteOptionsBarControl(HtmlEditor);
					MainCell.Controls.Add(this.pasteOptionsBar);
				}
				CreateIFrame(MainCell);
			}
			else {
				if(HtmlEditor.IsErrorFrameVisible())
					CreateErrorFrame(MainCell);
				CreateDesignTimeEditArea(MainCell);
			}
		}
		protected override void CreateIFrame(Control container) {
			base.CreateIFrame(container);
			if(Browser.IsOpera) { 
				RenderUtils.SetStringAttribute(IFrame, "onload", HtmlEditor.GetDesignViewIframeOnLoadHandler());
				IFrame.Attributes["src"] = "javascript:void(0)";
			}
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.tagInspector = null;
			this.pasteOptionsBar = null;
		}
	}
	public class DesignTagInspectorControl : BaseControl {
		protected WebControl CurrentTagWrapper { get; private set; }
		protected Image RemoveButton { get; private set; }
		protected Image ChangeButton { get; private set; }
		protected TableRow LayoutTableMainRow { get; private set; }
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		public DesignTagInspectorControl(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected void CreateLayoutTableHierarchy() {
			Table LayoutTable = RenderUtils.CreateTable();
			Controls.Add(LayoutTable);
			LayoutTableMainRow = new TableRow();
			LayoutTable.Rows.Add(LayoutTableMainRow);
			CreateHierarchy();
		}
		void AddCellContentControl(WebControl Control) {
			TableCell cell = new TableCell();
			LayoutTableMainRow.Cells.Add(cell);
			cell.Controls.Add(Control);
		}
		void CreateHierarchy() {
			CreateDivTag();
			CreateSeparator();
			CreateDivTag();
			CreateSeparator();
			CreateDivTag();
			CreateSeparator();
			CreateCurrentTagWrapper();
		}
		void CreateCurrentTagWrapper(){
			CurrentTagWrapper = RenderUtils.CreateDiv();
			RemoveButton = RenderUtils.CreateImage();
			ChangeButton = RenderUtils.CreateImage();
			RemoveButton.Style["vertical-align"] = "middle";
			ChangeButton.Style["vertical-align"] = "middle";
			CurrentTagWrapper.Controls.Add(CreateSelectedTag());
			CurrentTagWrapper.Controls.Add(RemoveButton);
			CurrentTagWrapper.Controls.Add(ChangeButton);
			AddCellContentControl(CurrentTagWrapper);
		}
		void CreateSeparator() {
			WebControl SeparatorImageWrapper = RenderUtils.CreateDiv();
			SeparatorImageWrapper.CssClass = "dxhe-tiSeparator";
			Image SeparatorImage = RenderUtils.CreateImage();
			HtmlEditor.GetTagInspectorSeparatorImageProperties().AssignToControl(SeparatorImage, true);
			SeparatorImageWrapper.Controls.Add(SeparatorImage);
			AddCellContentControl(SeparatorImageWrapper);
		}
		void CreateDivTag() {
			Label link = new Label();
			HtmlEditor.StylesTagInspector.GetTagStyle().AssignToControl(link);
			link.Text="div";
			AddCellContentControl(link);
		}
		Label CreateSelectedTag() {
			Label selectedTag = new Label();
			HtmlEditor.StylesTagInspector.GetSelectedTagStyle().AssignToControl(selectedTag);
			selectedTag.Text = "div";
			selectedTag.Style["vertical-align"] = "middle";
			return selectedTag;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateLayoutTableHierarchy();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CssClass = HtmlEditorStyles.TagInspectorWrapperCssClass;
			Style["line-height"] = "normal !important";
			HtmlEditor.StylesTagInspector.GetSelectedTagContainerStyle().AssignToControl(CurrentTagWrapper);
			MergeImagesProperties(ChangeButton, HtmlEditorImages.TagInspectorChangeElementButtonImageName, HtmlEditor.Images.TagInspectorChangeElementButton);
			MergeImagesProperties(RemoveButton, HtmlEditorImages.TagInspectorRemoveElementButtonImageName, HtmlEditor.Images.TagInspectorRemoveElementButton);
		}
		protected void MergeImagesProperties(Image image, string defaultImageName, CheckedButtonImageProperties customProperties) {
			CheckedButtonImageProperties res = new CheckedButtonImageProperties();
			res.CopyFrom(HtmlEditor.Images.GetImageProperties(Page, defaultImageName));
			res.CopyFrom(customProperties);
			res.AssignToControl(image, true);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			CurrentTagWrapper = null;
			ChangeButton = null;
			RemoveButton = null;
			LayoutTableMainRow = null;
		}
	}
	public class TagInspectorControl : BaseControl {
		protected WebControl ControlsWrapper { get; private set; }
		protected WebControl TagsContainer { get; private set; }
		protected ASPxButton RemoveElementButton { get; private set; }
		protected ASPxButton ChangeElementButton { get; private set; }
		protected WebControl CurrentTagWrapper { get; private set; }
		protected override bool HasRootTag() {
			return true;
		}
		protected override HtmlTextWriterTag TagKey {
			get {
				return HtmlTextWriterTag.Div;
			}
		}
		public TagInspectorControl(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			ID = RenderHelper.TagInspectorWrapperID;
			ControlsWrapper = RenderUtils.CreateDiv();
			ControlsWrapper.ID = RenderHelper.TagInspectorControlsWrapperID;
			TagsContainer = RenderUtils.CreateDiv();
			TagsContainer.ID = RenderHelper.TagInspectorTagsContainerID;
			CurrentTagWrapper = new InternalHyperLink();
			RemoveElementButton = new ASPxButton();
			ChangeElementButton = new ASPxButton();
			ControlsWrapper.Controls.Add(CurrentTagWrapper);
			ControlsWrapper.Controls.Add(RemoveElementButton);
			ControlsWrapper.Controls.Add(ChangeElementButton);
			TagsContainer.Controls.Add(ControlsWrapper);
			Controls.Add(TagsContainer);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			TagsContainer.CssClass = HtmlEditorStyles.TagInspectorControlsTagsContainerCssClass;
			CurrentTagWrapper.CssClass = "dxhe-tiTagName dxhe-tiSelected";
			CssClass = HtmlEditorStyles.TagInspectorWrapperCssClass;
			HtmlEditor.StylesTagInspector.GetSelectedTagContainerStyle().AssignToControl(ControlsWrapper);
			RenderUtils.SetVisibility(ControlsWrapper, false, false, true);
			PrepareButton(RemoveElementButton, RenderHelper.TagInspectorTagRemoveButtonID, "dxhe-tiDeleteButton", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.DeleteElement));
			PrepareButton(ChangeElementButton, RenderHelper.TagInspectorTagPropertiesButtonID, "dxhe-tiPropertyButton", ASPxHtmlEditorLocalizer.GetString(ASPxHtmlEditorStringId.ChangeElementProperties));
			MergeImagesProperties(ChangeElementButton.Image, HtmlEditorImages.TagInspectorChangeElementButtonImageName, HtmlEditor.Images.TagInspectorChangeElementButton);
			MergeImagesProperties(RemoveElementButton.Image, HtmlEditorImages.TagInspectorRemoveElementButtonImageName, HtmlEditor.Images.TagInspectorRemoveElementButton);
		}
		void MergeImagesProperties(CheckedButtonImageProperties buttonImage, string defaultImageName, CheckedButtonImageProperties customProperties) {
			CheckedButtonImageProperties res = new CheckedButtonImageProperties();
			res.CopyFrom(HtmlEditor.Images.GetImageProperties(Page, defaultImageName));
			res.CopyFrom(customProperties);
			buttonImage.Assign(res);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			ControlsWrapper = null;
			TagsContainer = null;
			RemoveElementButton = null;
			ChangeElementButton = null;
		}
		void PrepareButton(ASPxButton button, string id, string cssClass, string toolTip) {
			button.AutoPostBack = false;
			button.ID = id;
			button.ToolTip = toolTip;
			button.CssClass = cssClass;
			button.AllowFocus = false;
		}
	}
	public class HtmlEditorControl : BaseControl {
		private Table mainTable;
		private TableCell mainCell;
		private Table contentTable;
		private TableCell toolbarCell;
		private TableCell contentAreaCell;
		private TableCell statusBarCell;
		private HtmlEditorBarDockControl barDockControl;
		private DesignViewAreaControl designViewArea;
		private HtmlViewEditControl htmlViewMemoControl;
		private HtmlViewSourceEditorControl htmlViewSourceEditor;
		private ViewAreaControl previewViewControl;
		private StatusBarControl statusBarControl;
		private KeyboardSupportInputHelper fakeFocusInput;
		private HtmlEditorPopupForm dialogPopupForm;
		private HtmlEditorContextMenu contextMenu;
		public HtmlEditorControl(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
		}
		protected Table MainTable { get { return mainTable; } }
		protected TableCell MainCell { get { return mainCell; } }
		protected Table ContentTable { get { return contentTable; } }
		protected TableCell ToolbarCell { get { return toolbarCell; } }
		HtmlEditorSystemPopup systemPopupControl;
		protected TableCell ContentAreaCell { get { return contentAreaCell; } }
		protected TableCell StatusBarCell { get { return statusBarCell; } }
		protected HtmlEditorContextMenu ContextMenu { get { return contextMenu; } }
		protected internal HtmlEditorBarDockControl BarDockControl { get { return barDockControl; } }
		protected ViewAreaControl DesignViewArea { get { return designViewArea; } }
		protected ASPxMemo HtmlViewMemoControl { get { return htmlViewMemoControl; } }
		protected HtmlViewSourceEditorControl HtmlViewSourceEditor { get { return htmlViewSourceEditor; } }
		protected ViewAreaControl PreviewViewControl { get { return previewViewControl; } }
		protected internal StatusBarControl StatusBar { get { return statusBarControl; } }
		protected WebControl FakeFocusInput { get { return fakeFocusInput; } }
		protected ASPxPopupControl DialogPopupForm { get { return dialogPopupForm; } }
		protected override void ClearControlFields() {
			this.mainTable = null;
			this.mainCell = null;
			this.contentTable = null;
			this.toolbarCell = null;
			this.contentAreaCell = null;
			this.statusBarCell = null;
			this.systemPopupControl = null;
			this.barDockControl = null;
			this.designViewArea = null;
			this.htmlViewMemoControl = null;
			this.htmlViewSourceEditor = null;
			this.previewViewControl = null;
			this.statusBarControl = null;
			this.dialogPopupForm = null;
			this.fakeFocusInput = null;
			this.contextMenu = null;
		}
		protected override void CreateControlHierarchy() {
			if(!DesignMode)
				CreateFakeFocusInput();
			CreateMainTable();
			CreateContentTable(MainCell);
			if(HtmlEditor.Settings.AllowDesignViewInternal && (HtmlEditor.Toolbars.Count == 0 || HtmlEditor.Toolbars.GetVisibleItemCount() > 0))
				CreateBarDockControl(ContentTable);
			CreateAreaControls(ContentTable);
			if(HtmlEditor.ShowViewSwitcher || HtmlEditor.SettingsResize.AllowResize)
				CreateStatusBar(ContentTable);
			if(!DesignMode)
				CreateSystemPopup();
			if(HtmlEditor.Settings.AllowDesignViewInternal || HtmlEditor.Settings.AllowHtmlView) {
				if(HtmlEditor.Settings.AllowDesignViewInternal)
					CreateDialogPopupForm();
				CreateContextMenuControl();
			}
			CreateTemplateErrorFrame();
		}
		private void CreateMainTable() {
			this.mainTable = RenderUtils.CreateTable(true);
			Controls.Add(MainTable);
			TableRow mainRow = RenderUtils.CreateTableRow();
			MainTable.Rows.Add(mainRow);
			this.mainCell = RenderUtils.CreateTableCell();
			MainCell.ID = RenderHelper.HtmlEditorMainCellID;
			mainRow.Cells.Add(MainCell);
		}
		private void CreateContentTable(Control container) {
			this.contentTable = RenderUtils.CreateTable();
			container.Controls.Add(ContentTable);
		}
		private void CreateBarDockControl(Table tableContainer) {
			bool isBarDockVisible = HtmlEditor.ActiveView == HtmlEditorView.Design;
			if(!isBarDockVisible && DesignMode)
				return;
			TableRow row = RenderUtils.CreateTableRow();
			row.ID = RenderHelper.ToolbarRowID;
			RenderUtils.SetVisibility(row, isBarDockVisible, true);
			tableContainer.Rows.Add(row);
			this.toolbarCell = RenderUtils.CreateTableCell();
			row.Cells.Add(ToolbarCell);
			this.barDockControl = new HtmlEditorBarDockControl(HtmlEditor);
			BarDockControl.EnableViewState = false;
			BarDockControl.ID = RenderHelper.BarDockControlID;
			BarDockControl.EncodeHtml = HtmlEditor.EncodeHtml;
			BarDockControl.Toolbars.Assign(HtmlEditor.Toolbars);
			ToolbarCell.Controls.Add(BarDockControl);
			BarDockControl.EditorImages.Assign(HtmlEditor.ImagesEditors);
			BarDockControl.EditorStyles.Assign(HtmlEditor.StylesEditors);
		}
		private void CreateAreaControls(Table tableContainer) {
			TableRow editAreaRow = RenderUtils.CreateTableRow();
			tableContainer.Rows.Add(editAreaRow);
			this.contentAreaCell = RenderUtils.CreateTableCell();
			ContentAreaCell.ID = RenderHelper.EditAreaCellID;
			editAreaRow.Cells.Add(ContentAreaCell);
			if(DesignMode)
				CreateDesignTimeAreaControls();
			else
				CreateRuntimeAreaControls();
		}
		private void CreateSystemPopup() {
			this.systemPopupControl = new HtmlEditorSystemPopup(HtmlEditor);
			this.systemPopupControl.ID = RenderHelper.SystemPopupID;
			Controls.Add(this.systemPopupControl);
		}
		private void CreateDesignTimeAreaControls() {
			switch(HtmlEditor.ActiveView) {
				case HtmlEditorView.Design:
					CreateDesignViewArea(ContentAreaCell);
					break;
				case HtmlEditorView.Html:
					CreateHtmlViewArea(ContentAreaCell);
					break;
				case HtmlEditorView.Preview:
					CreatePreviewIFrame(ContentAreaCell);
					break;
				default:
					throw new InvalidOperationException();
			}
		}
		private void CreateRuntimeAreaControls() {
			if(HtmlEditor.Settings.AllowDesignViewInternal)
				CreateDesignViewArea(ContentAreaCell);
			if(HtmlEditor.Settings.AllowHtmlViewInternal)
				CreateHtmlViewArea(ContentAreaCell);
			if(HtmlEditor.Settings.AllowPreviewInternal)
				CreatePreviewIFrame(ContentAreaCell);
		}
		private void CreateStatusBar(Table tableContainer) {
			TableRow statusBarRow = RenderUtils.CreateTableRow();
			tableContainer.Rows.Add(statusBarRow);
			this.statusBarCell = RenderUtils.CreateTableCell();
			statusBarRow.Cells.Add(StatusBarCell);
			this.statusBarCell.ID = RenderHelper.StatusBarCellID;
			this.statusBarControl = new StatusBarControl(HtmlEditor);
			StatusBarCell.Controls.Add(this.statusBarControl);
		}
		private void CreateDesignViewArea(Control container) {
			this.designViewArea = new DesignViewAreaControl(HtmlEditor, RenderHelper.DesignViewIFrameID,
				RenderHelper.DesignViewIFrameName, string.Empty,
				DesignMode ? HtmlEditor.StylesDocument.MergeStyles(HtmlEditor.GetDesignViewAreaStyle()) : HtmlEditor.GetDesignViewAreaStyle(),
				RenderHelper.DesignViewCell, HtmlEditor.ActiveView == HtmlEditorView.Design);
			container.Controls.Add(DesignViewArea);
		}
		private void CreateHtmlViewArea(Control container) {
			if(HtmlEditor.IsAdvancedHtmlEditingMode()) {
				this.htmlViewSourceEditor = new HtmlViewSourceEditorControl(HtmlEditor);
				container.Controls.Add(HtmlViewSourceEditor);
			}
			else {
				this.htmlViewMemoControl = new HtmlViewEditControl(HtmlEditor);
				container.Controls.Add(HtmlViewMemoControl);
			}
		}
		private void CreatePreviewIFrame(Control container) {
			this.previewViewControl = new ViewAreaControl(HtmlEditor, RenderHelper.PreviewIFrameID,
				RenderHelper.PreviewIFrameName, string.Empty,
				DesignMode ? HtmlEditor.StylesDocument.MergeStyles(HtmlEditor.GetPreviewAreaStyle()) : HtmlEditor.GetPreviewAreaStyle(),
				RenderHelper.PreviewCellID, HtmlEditor.ActiveView == HtmlEditorView.Preview);
			container.Controls.Add(PreviewViewControl);
		}
		private void CreateFakeFocusInput() {
			WebControl inputDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			RenderUtils.SetStyleStringAttribute(inputDiv, "position", "relative");
			RenderUtils.SetStyleStringAttribute(inputDiv, "width", "0px");
			RenderUtils.SetStyleStringAttribute(inputDiv, "height", "0px");
			Controls.Add(inputDiv);
			this.fakeFocusInput = new KeyboardSupportInputHelper(RenderHelper.FakeFocusInputID);
			RenderUtils.SetAttribute(FakeFocusInput, "disabled", "disabled", "");
			inputDiv.Controls.Add(FakeFocusInput);
		}
		protected void CreateTemplateErrorFrame() {
			if(!DesignMode && HtmlEditor.IsErrorFrameRequired()) {
				HtmlEditorErrorFrameControl errorFrame = new HtmlEditorErrorFrameControl(HtmlEditor);
				Controls.Add(errorFrame);
			}
		}
		private void CreateDialogPopupForm() {
			if(!DesignMode) {
				this.dialogPopupForm = new HtmlEditorPopupForm(HtmlEditor);
				DialogPopupForm.ID = RenderHelper.DialogPopupFormID;
				DialogPopupForm.ParentStyles = HtmlEditor.StylesDialogForm;
				DialogPopupForm.EnableViewState = false;
				DialogPopupForm.RenderIFrameForPopupElements = HtmlEditor.RenderIFrameForPopupElements;
				if(HtmlEditor.RenderIFrameForPopupElements == DefaultBoolean.True)
					DialogPopupForm.ShowShadow = false;
				Controls.Add(DialogPopupForm);
			}
		}
		private void CreateContextMenuControl() {
			if(!DesignMode && HtmlEditor.Settings.AllowContextMenu == DefaultBoolean.True) {
				this.contextMenu = new HtmlEditorContextMenu(HtmlEditor, RenderHelper.ContextMenuID);				
				Controls.Add(ContextMenu);
			}
		}
		protected override void PrepareControlHierarchy() {
			PrepareMainTable();
			PrepareContentTable();
			PrepareToolbarsCell();
			PrepareBarDockControl();
			PrepareAreaControls();
			PrepareStatusBar();
			if(DialogPopupForm != null)
				DialogPopupForm.CloseButtonImage.CopyFrom(HtmlEditor.Images.DialogFormCloseButton);
		}
		private void PrepareMainTable() {
			RenderUtils.AssignAttributes(HtmlEditor, MainTable);
			MainTable.TabIndex = 0;
			RenderUtils.SetVisibility(MainTable, HtmlEditor.IsClientVisible(), true);
			HtmlEditor.GetControlStyle().AssignToControl(MainTable);
			if(HtmlEditor.IsRightToLeft()) {
				RenderUtils.AppendDefaultDXClassName(MainTable, HtmlEditorStyles.RightToLeftCssClass);
				MainTable.Attributes["dir"] = "ltr";
			}
			MainTable.Width = HtmlEditor.SettingsResize.AllowResize && HtmlEditor.Width.Type == UnitType.Pixel ?
				GetRealSize(HtmlEditor.GetControlStyle().Width, HtmlEditor.SettingsResize.MaxWidth, HtmlEditor.SettingsResize.MinWidth) :
				HtmlEditor.GetControlStyle().Width;
			MainTable.Height = HtmlEditor.SettingsResize.AllowResize && HtmlEditor.Height.Type == UnitType.Pixel ?
				GetRealSize(HtmlEditor.GetControlStyle().Height, HtmlEditor.SettingsResize.MaxHeight, HtmlEditor.SettingsResize.MinHeight) :
				HtmlEditor.GetControlStyle().Height;
			RenderUtils.SetPaddings(MainCell, HtmlEditor.GetPaddings());
			RenderUtils.AppendDefaultDXClassName(MainCell, RenderUtils.DefaultStyleNamePrefix);
			MainCell.Width = Unit.Percentage(100);
			MainCell.Height = Unit.Percentage(100);
		}
		Unit GetRealSize(Unit size, int maxSize, int minSize) {
			int curSize = (int)size.Value;
			if(HtmlEditor.SettingsResize.MinWidth != 0)
				curSize = Math.Max(minSize, curSize);
			if(HtmlEditor.SettingsResize.MaxWidth != 0)
				curSize = Math.Min(maxSize, curSize);
			return Unit.Pixel(curSize);
		}
		private void PrepareContentTable() {
			AppearanceStyle contentAreaStyle = HtmlEditor.GetContentAreaStyle();
			if(HtmlEditor.ShowViewSwitcher)
				contentAreaStyle.BorderBottom.BorderStyle = BorderStyle.None;
			if(HtmlEditor.ShowViewSwitcher || (!HtmlEditor.ShowViewSwitcher && HtmlEditor.SettingsResize.AllowResize))
				contentAreaStyle.Paddings.PaddingBottom = 0;
			contentAreaStyle.AssignToControl(ContentAreaCell, true);
			ContentTable.Width = Unit.Percentage(100);
			RenderUtils.AppendDefaultDXClassName(ContentAreaCell, "dxheContentAreaSys");
			if(!Browser.IsIE || DesignMode) {
				ContentTable.Height = Unit.Percentage(100);
				ContentAreaCell.Height = Unit.Percentage(100);
				ContentAreaCell.VerticalAlign = VerticalAlign.Top;
			}
		}
		private void PrepareToolbarsCell() {
			if(ToolbarCell != null && Browser.IsIE && ToolbarCell != null)
				ToolbarCell.VerticalAlign = VerticalAlign.Top;
			if(Browser.IsIE && ToolbarCell != null)
				RenderUtils.SetPadding(ToolbarCell, 0);
		}
		private void PrepareBarDockControl() {
			if(BarDockControl == null)
				return;
			if(!DesignMode) {
				BarDockControl.ClientSideEvents.Command = RenderUtils.CreateClientEventHandler(
					string.Format("ASPx.HEToolbarCommand('{0}', e.item, e.value)", HtmlEditor.ClientID));
				if(BarDockControl.IsSaveSelectionBeforeFocusNeeded()) {
					BarDockControl.ClientSideEvents.DropDownItemBeforeFocus = RenderUtils.CreateClientEventHandler(
						string.Format("ASPx.HEToolbarDropDownItemBeforeFocus('{0}', e.toolbar, e.item)", HtmlEditor.ClientID));
					BarDockControl.ClientSideEvents.DropDownItemCloseUp = RenderUtils.CreateClientEventHandler(
						string.Format("ASPx.HEToolbarDropDownItemCloseUp('{0}', e.toolbar, e.item)", HtmlEditor.ClientID));
				}
				BarDockControl.ClientSideEvents.CustomComboBoxInit = RenderUtils.CreateClientEventHandler(
					string.Format("ASPx.HEToolbarCustomComboBoxInit('{0}', e.toolbar, e.item)", HtmlEditor.ClientID));
			}
			BarDockControl.BarDockStyle.Assign(HtmlEditor.StylesToolbars.BarDockControl);
			BarDockControl.ToolbarStyle.Assign(HtmlEditor.StylesToolbars.Toolbar);
			BarDockControl.ToolbarItemStyle.Assign(HtmlEditor.StylesToolbars.ToolbarItem);
			BarDockControl.StylesRibbon.Assign(HtmlEditor.StylesToolbars.Ribbon);
			BarDockControl.StylesRibbon.PopupMenu.Assign(HtmlEditor.StylesToolbars.Ribbon.PopupMenu);
			BarDockControl.StylesRibbon.TabControl.Assign(HtmlEditor.StylesToolbars.Ribbon.TabControl);
		}
		private void PrepareAreaControls() {
			if(!HtmlEditor.IsAdvancedHtmlEditingMode())
				PrepareHtmlViewMemoControl();
		}
		private void PrepareStatusBar() {
			if(StatusBarCell == null)
				return;
			if(Browser.IsIE)
				StatusBarCell.VerticalAlign = VerticalAlign.Bottom;
			if(!HtmlEditor.ShowViewSwitcher) {
				HtmlEditor.GetStatusBarStyle().AssignToControl(StatusBarCell);
				if(HtmlEditor.SettingsResize.AllowResize) {
					AppearanceStyle style = HtmlEditor.StylesStatusBar.GetLightStatusBarStyle();
					style.AssignToControl(StatusBarCell);
					RenderUtils.SetPaddings(StatusBarCell, style.Paddings);
				}
			}
		}
		private void PrepareHtmlViewMemoControl() {
			if(HtmlViewMemoControl == null)
				return;
			AppearanceStyleBase viewAreaStyle = HtmlEditor.GetHtmlViewAreaStyle();
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.AssignWithoutBorders(viewAreaStyle);
			style.AssignToControl(HtmlViewMemoControl);
			AssignHtmlViewAreaBorders(viewAreaStyle);   
			HtmlViewMemoControl.ID = RenderHelper.HtmlViewEditID;
			HtmlViewMemoControl.Width = Unit.Percentage(100);
			HtmlViewMemoControl.Height = Unit.Percentage(100);
			HtmlViewMemoControl.ClientVisible = HtmlEditor.ActiveView == HtmlEditorView.Html;
		}
		void AssignHtmlViewAreaBorders(AppearanceStyleBase style) {
			Border border = new Border();
			border.Assign(style.Border);
			border.AssignToControl(HtmlViewMemoControl);
			if(style.BorderBottom != null && !style.BorderBottom.IsEmpty)
				style.BorderBottom.AssignToControl(HtmlViewMemoControl);
		}
	}
	[ToolboxItem(false)]
	public class HtmlEditorSpellChecker : ASPxSpellChecker.ASPxSpellChecker {
		protected internal const string ScriptName = ASPxHtmlEditor.HtmlEditorScriptsResourcePath + "SpellChecker.js";
		private ASPxHtmlEditor htmlEditor;
		public HtmlEditorSpellChecker(ASPxHtmlEditor htmlEditor)
			: base(htmlEditor) {
			this.htmlEditor = htmlEditor;
			ParentSkinOwner = htmlEditor;
			ParentStyles = htmlEditor.StylesSpellChecker;
		}
		public ASPxHtmlEditor HtmlEditor {
			get { return htmlEditor; }
		}
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		internal IDictionary Options {
			get { return GetSettingsDictionary(); }
		}
		public void CreateCallbackReader(string arguments, string textToCheck) {
			CallBackReader = new HtmlEditorSpellCheckerCallbackReader(arguments, textToCheck); 
		}
		public object GetSpellCheckResult() {
			return GetCallbackResult();
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.HtmlEditorClasses.Controls.HtmlEditorSpellChecker";
		}
		protected internal new void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(HtmlEditorSpellChecker), ScriptName);
		}
		protected override bool IsShowOneWordInTextPreview() {
			return true;
		}
		protected override void PrepareSpellCheckPopupControl(SpellCheckerPopupControl popupControl) {
			base.PrepareSpellCheckPopupControl(popupControl);
			popupControl.PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
			popupControl.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			popupControl.RenderIFrameForPopupElements = HtmlEditor.RenderIFrameForPopupElements;
			if(HtmlEditor.RenderIFrameForPopupElements == DefaultBoolean.True)
				popupControl.ShowShadow = false;
		}
		protected override EditorImages GetEditorsImages() {
			return HtmlEditor.ImagesEditors;
		}
		protected override ImageProperties GetLoadingPanelImage() {
			return HtmlEditor.Images.LoadingPanel;
		}
		protected override HeaderButtonImageProperties GetDialogFormCloseButtonImage() {
			return HtmlEditor.Images.DialogFormCloseButton;
		}
		protected override ButtonControlStyles GetButtonStyles() {
			return HtmlEditor.StylesButton;
		}
		protected override PopupControlStyles GetDialogFormStyles() {
			return HtmlEditor.StylesDialogForm;
		}
		protected override EditorStyles GetEditorsStyles() {
			return HtmlEditor.StylesEditors;
		}
	}
	public class HtmlEditorSpellCheckerCallbackReader : SpellCheckerCallbackReader {
		string textToCheck;
		public HtmlEditorSpellCheckerCallbackReader(string arguments, string textToCheck)
			: base(arguments) {
			this.textToCheck = textToCheck;
		}
		public override string TextToCheck { get { return this.textToCheck; } }
	}
}
