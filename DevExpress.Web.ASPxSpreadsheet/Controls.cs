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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxSpreadsheet.Internal.Commands;
using DevExpress.Web.ASPxSpreadsheet.Internal.TabControl;
using DevExpress.Web.ASPxSpreadsheet.Localization;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Services;
using ChartsModel = DevExpress.Charts.Model;
using DevExpress.XtraSpreadsheet.Layout.Engine;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public enum SpreadsheetHeaderType { Column, Row }
	public class SpreadsheetControlBase : ASPxInternalWebControl {
		public SpreadsheetControlBase(ASPxSpreadsheet spreadsheet) {
			Spreadsheet = spreadsheet;
		}
		public ASPxSpreadsheet Spreadsheet { get; private set; }
	}
	[ToolboxItem(false)]
	public class SpreadsheetDialogControl : ASPxPopupControl {
		ASPxSpreadsheet spreadsheet;
		public SpreadsheetDialogControl(ASPxSpreadsheet spreadsheet)
			: base(null) {
			this.spreadsheet = spreadsheet;
			AllowDragging = true;
			EnableClientSideAPI = true;
			PopupAnimationType = AnimationType.Fade;
			Modal = true;
			CloseAction = CloseAction.CloseButton;
			CloseOnEscape = true;
			ParentSkinOwner = spreadsheet;
			PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
			PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
			ID = ASPxSpreadsheet.SpreadsheetPopupDialogContainerID;
		}
		public ASPxSpreadsheet Spreadsheet {
			get { return spreadsheet; }
			set { spreadsheet = value; }
		}
		protected override bool HideBodyScrollWhenModal() {
			return !Browser.Family.IsNetscape;
		}
		protected override bool LoadWindowsState(string state) {
			return false;
		}
		protected override StylesBase CreateStyles() {
			return new SpreadsheetDialogFormStylesLite(this);
		}
	}
	[ToolboxItem(false)]
	public abstract class SpreadsheetPopupMenuControlBase : SpreadsheetControlBase {
		ASPxPopupMenuExt popupMenu = new ASPxPopupMenuExt();
		public SpreadsheetPopupMenuControlBase(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		public ASPxPopupMenuExt PopupMenu { get { return popupMenu; } }
		protected abstract string GetPopupMenuID();
		protected abstract PopupAction GetPopupMenuAction();
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.popupMenu = null;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PopupMenu.ParentStyles = Spreadsheet.StylesPopupMenu;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.popupMenu = new ASPxPopupMenuExt();
			PopupMenu.ID = GetPopupMenuID();
			Controls.Add(PopupMenu);
			SetupMenuControl(PopupMenu);
		}
		protected virtual void SetupMenuControl(ASPxPopupMenuExt popupMenu) {
			popupMenu.PopupAction = PopupAction.RightMouseClick;
			popupMenu.CloseAction = PopupMenuCloseAction.OuterMouseClick;
			popupMenu.EncodeHtml = Spreadsheet.EncodeHtml;
			popupMenu.EnableViewState = false;
			popupMenu.ClientSideEvents.ItemClick = Spreadsheet.GetPopupMenuControlOnItemClickScript();
			popupMenu.ClientSideEvents.CloseUp = Spreadsheet.GetPopupMenuControlOnCloseUpScript();
		}
	}
	public class SpreadsheetPopupMenuControl : SpreadsheetPopupMenuControlBase {
		public SpreadsheetPopupMenuControl(ASPxSpreadsheet spreadsheet) 
			: base(spreadsheet) {
		}
		protected override string GetPopupMenuID() {
 			return ASPxSpreadsheet.SpreadsheetPopupMenuContainerID;
		}
		protected override PopupAction GetPopupMenuAction() {
 			return PopupAction.RightMouseClick;
		}
	}
	public class SpreadsheetAutoFilterPopupMenuControl : SpreadsheetPopupMenuControlBase {
		public SpreadsheetAutoFilterPopupMenuControl(ASPxSpreadsheet spreadsheet) 
			: base(spreadsheet) {
		}
		protected override string GetPopupMenuID() {
 			return ASPxSpreadsheet.SpreadsheetAutoFilterPopupMenuContainerID;
		}
		protected override PopupAction GetPopupMenuAction() {
 			return PopupAction.LeftMouseClick;
		}
		protected override void SetupMenuControl(ASPxPopupMenuExt popupMenu) {
			base.SetupMenuControl(popupMenu);
			popupMenu.PopupVerticalAlign = PopupVerticalAlign.Below;
		}
	}
	[ToolboxItem(false)]
	public class SpreadsheetTabsControl : SpreadsheetControlBase {
		TabControlWithSample tabControl;
		public SpreadsheetTabsControl(ASPxSpreadsheet spreadsheet)
			:base (spreadsheet) {}
		public TabControlWithSample TabControl { get { return tabControl; } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.tabControl = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.tabControl = new TabControlWithSample(Spreadsheet);
			TabControl.ID = ASPxSpreadsheet.SpreadsheetTabContainerID;
			TabControl.ViewStateMode = ViewStateMode.Disabled;
			Controls.Add(TabControl);
			SetupTabControl(TabControl);
			FillTabs(TabControl);
		}
		protected void SetupTabControl(TabControlWithSample TabControl) {
			TabControl.TabPosition = TabPosition.Bottom;
			TabControl.TabAlign = TabAlign.Left;
			TabControl.Width = Unit.Percentage(100);
			TabControl.EncodeHtml = Spreadsheet.EncodeHtml;
			TabControl.EnableTabScrolling = true;
			TabControl.ParentSkinOwner = Spreadsheet;
		}
		protected void FillTabs(TabControlWithSample tabControl) {
			SpreadsheetWorkSession workSession = Spreadsheet.GetCurrentWorkSessions();
			if(workSession != null) {
				DocumentModel documentModel = workSession.DocumentModel;
				List<Worksheet> sheets = documentModel.GetVisibleSheets();
				foreach(var sheet in sheets) {
					Tab sheetTab = new Tab(sheet.Name);
					sheetTab.ToolTip = sheet.Name;
					tabControl.Tabs.Add(sheetTab);
					if(sheet.SheetId == documentModel.ActiveSheet.SheetId)
						tabControl.ActiveTab = sheetTab;
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			TabControl.Styles.Assign(Spreadsheet.StylesTabControl);
		}
	}
	public class SpreadsheetControl : SpreadsheetControlBase {
		public SpreadsheetControl(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		protected internal WorkBookControl WorkBookControl { get; private set; }
		protected internal ASPxRibbon RibbonControl { get; set; }
		protected internal SpreadsheetDialogControl Dialog { get; private set; }
		protected internal WebControl WorkBookContainer { get; set; }
		protected internal SpreadsheetTabsControl TabControl { get; set; }
		protected internal SpreadsheetFormulaBarControl FormulaBar { get; set; }
		protected internal SpreadsheetFunctionsListBox FunctionsListBox { get; set; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.Dialog = null;
			this.RibbonControl = null;
			this.WorkBookContainer = null;
			this.TabControl = null;
			this.WorkBookControl = null;
			this.FormulaBar = null;
			this.FunctionsListBox = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateRibbon();
			if(Spreadsheet.ShowFormulaBar)
				CreateFormulaBar();
			CreateWorkBookContainer();
			CreateWorkBookControl();
			CreateTabControl();
			CreatePopupMenuControl();
			CreateAutoFilterPopupMenuControl();
			CreateDialogControl();
			CreateFunctionsListBox();
		}
		protected virtual void CreateFormulaBar() {
			this.FormulaBar = new SpreadsheetFormulaBarControl(Spreadsheet);
			Controls.Add(this.FormulaBar);
		}
		protected virtual void CreateRibbon() {
			if(Spreadsheet.RibbonMode == SpreadsheetRibbonMode.ExternalRibbon) {
				RibbonControl = RibbonHelper.LookupRibbonControl(this, Spreadsheet.AssociatedRibbonID);
				if(RibbonControl != null) {
					RenderUtils.EnsureChildControlsRecursive(RibbonControl, false);
					SpreadsheetRibbonHelper.UpdateRibbonTabCollection(RibbonControl.Tabs);
				}
			} else if(Spreadsheet.RibbonMode != SpreadsheetRibbonMode.None) {
				RibbonControl = CreateRibbonCore();
				RibbonControl.OneLineMode = IsOneLineRibbonMode();
				Controls.Add(RibbonControl);
			}
		}
		bool IsOneLineRibbonMode() {
			bool isOneLineMode = Spreadsheet.RibbonMode == SpreadsheetRibbonMode.OneLineRibbon;
			bool isAutoMode = Spreadsheet.RibbonMode == SpreadsheetRibbonMode.Auto;
			bool isMobilePlatform = Browser.Platform.IsMobileUI;
			return isOneLineMode || (isAutoMode && isMobilePlatform);
		}
		protected ASPxRibbon CreateRibbonCore() {
			ASPxRibbon ribbon = new ASPxRibbon();
			ribbon.ID = ASPxSpreadsheet.SpreadsheetRibbonContainerID;
			ribbon.ShowFileTab = false;
			ribbon.Width = Unit.Percentage(100);
			ribbon.EncodeHtml = Spreadsheet.EncodeHtml;
			ribbon.ParentSkinOwner = Spreadsheet;
			ribbon.ViewStateMode = ViewStateMode.Disabled;
			ribbon.SettingsPopupMenu.EnableScrolling = true;
			ribbon.Images.IconSet = Spreadsheet.Images.MenuIconSet;
			if(ribbon.Tabs.IsEmpty) {
				if(Spreadsheet.RibbonTabs.IsEmpty) {
					ribbon.Tabs.AddRange(new SpreadsheetDefaultRibbon(Spreadsheet).DefaultRibbonTabs);
				} else {
					ribbon.Tabs.Assign(Spreadsheet.RibbonTabs);
				}
				SpreadsheetRibbonHelper.UpdateRibbonTabCollection(ribbon.Tabs);
			}
			if(ribbon.ContextTabCategories.IsEmpty) {
				if(Spreadsheet.RibbonContextTabCategories.IsEmpty)
					ribbon.ContextTabCategories.AddRange(new SpreadsheetDefaultRibbon(Spreadsheet).DefaultRibbonContextTabCategories);
				else
					ribbon.ContextTabCategories.Assign(Spreadsheet.RibbonContextTabCategories);
				SpreadsheetRibbonHelper.UpdateRibbonContextTabCategories(ribbon.ContextTabCategories);
			}
			ribbon.ActiveTabIndex = Spreadsheet.ActiveTabIndex;
			return ribbon;
		}
		protected virtual void CreateWorkBookContainer() {
			WorkBookContainer = RenderUtils.CreateDiv();
			WorkBookContainer.ID = ASPxSpreadsheet.WorkBookContainerID;
			Controls.Add(WorkBookContainer);
		}
		protected virtual void CreateWorkBookControl() {
			WorkBookControl = new WorkBookControl(Spreadsheet);
			WorkBookContainer.Controls.Add(WorkBookControl);
		}
		protected virtual void CreateTabControl() {
			TabControl = new SpreadsheetTabsControl(Spreadsheet);
			Controls.Add(TabControl);
		}
		protected virtual void CreatePopupMenuControl() {
			SpreadsheetPopupMenuControl popupMenu = new SpreadsheetPopupMenuControl(Spreadsheet);
			Controls.Add(popupMenu);
		}
		protected virtual void CreateAutoFilterPopupMenuControl() {
			SpreadsheetAutoFilterPopupMenuControl popupMenu = new SpreadsheetAutoFilterPopupMenuControl(Spreadsheet);
			Controls.Add(popupMenu);
		}
		protected virtual void CreateDialogControl() {
			this.Dialog = new SpreadsheetDialogControl(Spreadsheet);
			Controls.Add(Dialog);
		}
		protected virtual void CreateFunctionsListBox() {
			this.FunctionsListBox = new SpreadsheetFunctionsListBox(Spreadsheet);
			this.FunctionsListBox.ID = ASPxSpreadsheet.SpreadsheetFunctionsListBoxID;
			Controls.Add(this.FunctionsListBox);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			this.Width = Spreadsheet.Width;
			this.Height = Spreadsheet.Height;
			RenderUtils.AppendDefaultDXClassName(this, SpreadsheetStyles.MainElementCssClass);
			Spreadsheet.GetControlStyle().AssignToControl(this, true);
			RenderUtils.AssignAttributes(Spreadsheet, this);
			PrepareWorkBookContainer();
			if(RibbonControl != null)
				SpreadsheetRibbonHelper.PrepareDocumentUnitDependencyItems(RibbonControl.Tabs, Spreadsheet);
			PrepareFunctionsListBox();
		}
		protected void PrepareWorkBookContainer() {
			WorkBookContainer.Attributes.Add("class", "dxss-md");
			WorkBookContainer.Width = Unit.Percentage(100);
			WorkBookContainer.Height = Unit.Percentage(100);
		}
		protected virtual void PrepareFunctionsListBox() {
			Spreadsheet.StylesFormulaAutoCompete.GetListBoxStyle().AssignToControl(this.FunctionsListBox, true);
			this.FunctionsListBox.ItemStyle.Assign(Spreadsheet.StylesFormulaAutoCompete.GetListBoxItemStyle());
		}
	}
	public class SpreadsheetControlDesignTime : SpreadsheetControl {
		public SpreadsheetControlDesignTime(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Table; } }
		protected override void CreateRibbon() {
			if(Spreadsheet.RibbonMode == SpreadsheetRibbonMode.ExternalRibbon) {
				RibbonControl = RibbonHelper.LookupRibbonControl(this, Spreadsheet.AssociatedRibbonID);
				if(RibbonControl != null)
					RenderUtils.EnsureChildControlsRecursive(RibbonControl, false);
			} else if(Spreadsheet.RibbonMode != SpreadsheetRibbonMode.None) {
				RibbonControl = CreateRibbonCore();
				if(!Spreadsheet.Width.IsEmpty)
					RibbonControl.Width = Spreadsheet.Width;
				else
					RibbonControl.Width = Unit.Pixel(1000);
				RibbonControl.OneLineMode = Spreadsheet.RibbonMode == SpreadsheetRibbonMode.OneLineRibbon;
				var tcRow = RenderUtils.CreateTableRow();
				tcRow.Height = Unit.Pixel(1);
				var tcCell = RenderUtils.CreateTableCell();
				tcCell.Controls.Add(RibbonControl);
				tcRow.Controls.Add(tcCell);
				Controls.Add(tcRow);
			}
		}
		protected override void CreateTabControl() {
			TabControl = new SpreadsheetTabsControl(Spreadsheet);
			var tcRow = RenderUtils.CreateTableRow();
			tcRow.Height = Unit.Pixel(1);
			var tcCell = RenderUtils.CreateTableCell();
			tcCell.Controls.Add(TabControl);
			tcRow.Controls.Add(tcCell);
			Controls.Add(tcRow);
		}
		protected override void CreateWorkBookContainer() {
			WorkBookContainer = RenderUtils.CreateDiv();
			FillWorkbookContainer();
			var tcRow = RenderUtils.CreateTableRow();
			tcRow.Height = Unit.Percentage(100);
			var tcCell = RenderUtils.CreateTableCell();
			tcCell.Controls.Add(WorkBookContainer);
			tcRow.Controls.Add(tcCell);
			Controls.Add(tcRow);
		}
		protected void FillWorkbookContainer() {
			WorkBookContainer.ID = ASPxSpreadsheet.WorkBookContainerID;
			WorkBookContainer.Style.Add("overflow", "hidden");
			double controlMaxWidth = 1920,
				controlMaxHeight = 1080;
			int controlHeight = 0,
				controlWidth = 0,
				rowCount = 0,
				columnCount = 0;
			while(controlHeight <= controlMaxHeight) {
				controlWidth = 0;
				columnCount = 0;
				while(controlWidth <= controlMaxWidth) {
					var div = RenderUtils.CreateDiv("dxss-dtgc");
					div.Style.Add("left", (columnCount * 64).ToString() + "px");
					div.Style.Add("top", (rowCount * 20).ToString() + "px");
					WorkBookContainer.Controls.Add(div);
					controlWidth += 64;
					columnCount++;
				}
				controlHeight += 20;
				rowCount++;
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			Attributes["cellpadding"] = "0";
			Attributes["cellspacing"] = "0";
		}
		protected override void CreateDialogControl() { }
		protected override void CreatePopupMenuControl() { }
		protected override void CreateWorkBookControl() { }
		protected override void CreateFormulaBar() {
			FormulaBar = new SpreadsheetFormulaBarControlDesignTime(Spreadsheet);
			var tcRow = RenderUtils.CreateTableRow();
			tcRow.Height = Unit.Pixel(1);
			var tcCell = RenderUtils.CreateTableCell();
			tcCell.Controls.Add(FormulaBar);
			tcRow.Controls.Add(tcCell);
			Controls.Add(tcRow);
		}
		protected override void CreateAutoFilterPopupMenuControl() { }
		protected override void CreateFunctionsListBox() { }
		protected override void PrepareFunctionsListBox() { }
	}
	public class WorkBookControl : SpreadsheetControlBase {
		public WorkBookControl(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			var columnHeader = new HeaderControl(Spreadsheet, SpreadsheetHeaderType.Column);
			columnHeader.ID = "ColumnHeader";
			Controls.Add(columnHeader);
			var rowHeader = new HeaderControl(Spreadsheet, SpreadsheetHeaderType.Row);
			rowHeader.ID = "RowHeader";
			Controls.Add(rowHeader);
			var grid = new GridControl(Spreadsheet);
			grid.ID = "Grid";
			Controls.Add(grid);
			var scrollableControl = new ScrollableControl(Spreadsheet);
			scrollableControl.ID = "ScrollContainer";
			Controls.Add(scrollableControl);
		}
	}
	public abstract class TilesContainerBase : SpreadsheetControlBase {
		public TilesContainerBase(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		protected WebControl TilesContainer { get; private set; }
		protected WebControl FloatContainer { get; private set; }
		protected WebControl TopSpacer { get; private set; }
		protected WebControl BottomSpacer { get; private set; }
		protected WebControl LeftSpacer { get; private set; }
		protected WebControl RightSpacer { get; private set; }
		protected abstract bool HasTopSpacer { get; }
		protected abstract bool HasBottomSpacer { get; }
		protected abstract bool HasLeftSpacer { get; }
		protected abstract bool HasRightSpacer { get; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void CreateControlHierarchy() {
			CreateTopSpacer();
			CreateLeftSpacer();
			CreateTilesContainer();
			CreateRightSpacer();
			CreateBottomSpacer();
		}
		protected override void PrepareControlHierarchy() {
			TilesContainer.CssClass = "dxss-tc"; 
			if(FloatContainer != null)
				FloatContainer.CssClass = "dxss-fc";
			if(TopSpacer != null)
				TopSpacer.CssClass = "dxss-ts";
			if(BottomSpacer != null)
				BottomSpacer.CssClass = "dxss-bs";
			if(LeftSpacer != null)
				LeftSpacer.CssClass = "dxss-ls";
			if(RightSpacer != null)
				RightSpacer.CssClass = "dxss-rs";
		}
		void CreateTopSpacer() {
			if(!HasTopSpacer)
				return;
			TopSpacer = CreateDiv(this);
		}
		void CreateBottomSpacer() {
			if(!HasBottomSpacer)
				return;
			BottomSpacer = CreateDiv(this);
		}
		void CreateLeftSpacer() {
			if(!HasLeftSpacer)
				return;
			EnsureFloatContainer();
			LeftSpacer = CreateDiv(FloatContainer);
		}
		void CreateRightSpacer() {
			if(!HasRightSpacer)
				return;
			EnsureFloatContainer();
			RightSpacer = CreateDiv(FloatContainer);
			FloatContainer.Controls.Add(RenderUtils.CreateClearElement());
		}
		void CreateTilesContainer() {
			TilesContainer = CreateDiv(FloatContainer ?? this);
		}
		void EnsureFloatContainer() {
			if(FloatContainer != null || !HasLeftSpacer || !HasRightSpacer)
				return;
			FloatContainer = CreateDiv(this);
		}
		protected WebControl CreateDiv(WebControl container, string id) {
			var div = CreateDiv(container);
			div.ID = id;
			return div;
		}
		protected WebControl CreateDiv(WebControl container) {
			var div = RenderUtils.CreateDiv();
			container.Controls.Add(div);
			return div;
		}
	}
	public class HeaderControl : TilesContainerBase {
		public HeaderControl(ASPxSpreadsheet spreadsheet, SpreadsheetHeaderType headerType)
			: base(spreadsheet) {
			HeaderType = headerType;
		}
		protected SpreadsheetHeaderType HeaderType { get; private set; }
		protected override bool HasTopSpacer { get { return HeaderType == SpreadsheetHeaderType.Row; } }
		protected override bool HasBottomSpacer { get { return HeaderType == SpreadsheetHeaderType.Row; } }
		protected override bool HasLeftSpacer { get { return HeaderType == SpreadsheetHeaderType.Column; } }
		protected override bool HasRightSpacer { get { return HeaderType == SpreadsheetHeaderType.Column; } }
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CssClass = HeaderType == SpreadsheetHeaderType.Column ? "dxss-colHeader" : "dxss-rowHeader";
		}
	}
	public class GridControl : TilesContainerBase {
		public GridControl(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		protected override bool HasTopSpacer { get { return true; } }
		protected override bool HasBottomSpacer { get { return true; } }
		protected override bool HasLeftSpacer { get { return true; } }
		protected override bool HasRightSpacer { get { return true; } }
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			CssClass = "dxss-grid"; 
		}
	}
	public class ScrollableControl : SpreadsheetControlBase {
		public ScrollableControl(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		protected override void CreateControlHierarchy() {
			var scrollContent = RenderUtils.CreateDiv();
			scrollContent.ID = "ScrollContent";
			Controls.Add(scrollContent);
			scrollContent.Width = 10000;
			scrollContent.Height = 10000;
		}
		protected override void PrepareControlHierarchy() {
			CssClass = "dxss-sc";
		}
	}
	public class SpreadsheetTileControlBase : ASPxInternalWebControl {
		public SpreadsheetTileControlBase(SpreadsheetRenderHelper renderHelper) {
			RenderHelper = renderHelper;
		}
		public SpreadsheetRenderHelper RenderHelper { get; private set; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
	}
	public class GridTileControl : SpreadsheetTileControlBase {
		const string BorderHorizontalStyleClassName = "dxss-h";
		const string BorderVerticalStyleClassName = "dxss-v";
		public GridTileControl(SpreadsheetRenderHelper renderHelper, DevExpress.XtraSpreadsheet.Layout.Page page, int colIndex, int rowIndex, PanesType paneType)
			: base(renderHelper) {
			LayoutPage = page;
			ColIndex = colIndex;
			RowIndex = rowIndex;
			PaneType = paneType;
		}
		public DevExpress.XtraSpreadsheet.Layout.Page LayoutPage { get; private set; }
		public int ColIndex { get; private set; }
		public int RowIndex { get; private set; }
		public PanesType PaneType { get; private set; }
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateTextBoxes(LayoutPage.ComplexBoxes, "dxss-cctb", true);
			CreateTextBoxes(LayoutPage.Boxes, "dxss-sctb", false);
			CreateDrawingBoxes(LayoutPage.DrawingBoxes, "dxss-db");
			CreateVerticalBorders();
			CreateHorizontalBorders();
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			ID = string.Format("r{0}c{1}", RowIndex, ColIndex);
			Width = LayoutPage.Bounds.Width;
			Height = LayoutPage.Bounds.Height;
			CssClass = "dxss-gt"; 
		}
		protected void CreateTextBoxes(IEnumerable<CellTextBoxBase> textBoxes, string className, bool isComplexBox) {
			foreach(var textBox in textBoxes) {
				var cell = textBox.GetCell(LayoutPage.GridColumns, LayoutPage.GridRows, LayoutPage.Sheet);
				bool backgroundWrapperBox = string.IsNullOrEmpty(cell.DisplayText);
				string backgroundWrapperBoxClassName = "dxss-tbg";
				CreateBoxCore(backgroundWrapperBox ? backgroundWrapperBoxClassName : className, isComplexBox, textBox, cell);
			}
		}
		private void CreateBoxCore(string className, bool isComplexBox, CellTextBoxBase textBox, ICell cell) {
			WebControl parent = CreateTextBox_CellElement(textBox, cell, className, isComplexBox);
			CreateTextBox_TextContainer(textBox, cell, parent, isComplexBox);
		}
		protected string GetDrawingBoxUrl(DrawingBox drawingBox) {
			PictureBox pictureBox = drawingBox as PictureBox;
			if(pictureBox != null)
				return GetPictureBoxCommandUrl(pictureBox);
			ChartBox chartBox = drawingBox as ChartBox;
			if(chartBox != null)
				return GetChartBoxUrl(chartBox);
			return null;
		}
		protected string GetPictureBoxCommandUrl(PictureBox pictureBox) {
			return string.Format("{0}DXS.axd?s={1}&commandID=w{2}&simg={3}", HttpUtils.GetApplicationUrl(this), 
				RenderHelper.WorkSession.ID, (int)WebSpreadsheetCommandID.GetPictureContent, pictureBox.Image.ImageCacheKey);
		}
		protected string GetChartBoxUrl(ChartBox chartBox) {
			Chart chart = chartBox.Chart;
			IChartControllerFactoryService service = chart.DocumentModel.GetService<IChartControllerFactoryService>();
			if(service != null) {
				ChartsModel.ModelRect rect = new ChartsModel.ModelRect(0, 0, chartBox.Bounds.Width, chartBox.Bounds.Height);
				using(Bitmap image = new Bitmap(chartBox.Bounds.Width, chartBox.Bounds.Height)) {
					using(Graphics graphics = Graphics.FromImage(image)) {
						chart.Controller.RenderChart(service.Factory.CreateRenderContext(rect, graphics));
						if(chart.Is3DChart)
							image.RotateFlip(RotateFlipType.RotateNoneFlipY);
						using(MemoryStream mstream = new MemoryStream()) {
							image.Save(mstream, System.Drawing.Imaging.ImageFormat.Png);
							return GetImageBytesURL(mstream.ToArray());
						}
					}
				}
			}
			return null;
		}
		protected string GetImageBytesURL(byte[] imageBytes) {
			return BinaryStorage.GetImageUrl(this, imageBytes, BinaryStorageMode.Cache); 
		}
		protected void CreateDrawingBoxes(List<DrawingBox> drawingBoxs, string className) {
			foreach(var drawingBox in drawingBoxs) {
				string url = GetDrawingBoxUrl(drawingBox);
				if(!string.IsNullOrEmpty(url))
					CreateImageBox(drawingBox, url, className);
			}
		}
		protected void CreateImageBox(DrawingBox drawingBox, string url, string className) {
			var rect = drawingBox.Bounds;
			var image = new WebControl(HtmlTextWriterTag.Img);
			if(drawingBox.DrawingType == DrawingObjectType.Chart) {
				className += " dxss-chart";
			}
			RenderProvider.SetControlRect(image, rect, className);
			image.Attributes.Add("src", url);
			image.Attributes.Add("data-dbi", drawingBox.DrawingIndex.ToString());
			Controls.Add(image);
		}
		protected void CreateHorizontalBorders() {
			CreateBorders(LayoutPage.HorizontalBorders, BorderHorizontalStyleClassName);
		}
		protected void CreateVerticalBorders() {
			CreateBorders(LayoutPage.VerticalBorders, BorderVerticalStyleClassName);
		}
		protected Rectangle GetWebClipBounds(CellTextBoxBase cellTextBoxBase, DevExpress.XtraSpreadsheet.Layout.Page page) {
			Rectangle clipBounds = cellTextBoxBase.GetClipBounds(page);
			clipBounds.Intersect(page.Bounds);
			Point offset = Point.Empty;
			offset.X = -page.GridColumns.ActualFirst.Near;
			offset.Y = -page.GridRows.ActualFirst.Near;
			clipBounds.Offset(offset);
			return clipBounds;
		}
		protected Rectangle GetWebTextBounds(CellTextBoxBase cellTextBoxBase, DevExpress.XtraSpreadsheet.Layout.Page page) {
			Rectangle clipBounds = cellTextBoxBase.GetTextBounds(page, page.DocumentLayout);
			Point offset = Point.Empty;
			offset.X = -page.GridColumns.ActualFirst.Near;
			offset.Y = -page.GridRows.ActualFirst.Near;
			clipBounds.Offset(offset);
			return clipBounds;
		}
		protected WebControl CreateTextBox_CellElement(CellTextBoxBase cellTextBoxBase, ICell cell, string className, bool isComplexBox) {
			Rectangle rect;
			if(isComplexBox)
				rect = GetWebClipBounds(cellTextBoxBase, LayoutPage);
			else
				rect = cellTextBoxBase.GetWebBounds(LayoutPage);
			var div = RenderProvider.CreateDiv(rect, className);
			Controls.Add(div);
			StyleHelper.AssignFillStyleToElement(RenderHelper.Model, div, cell);
			ExtractDataAttributesFromCellPosition(div, cell.Position, "ctb");
			return div;
		}
		protected void CreateTextBox_TextContainer(CellTextBoxBase cellTextBoxBase, ICell cell, WebControl parent, bool isComplexBox) {
			Rectangle parentRect;
			Rectangle rect;
			if(isComplexBox) {
				parentRect = GetWebClipBounds(cellTextBoxBase, LayoutPage);
				rect = GetWebTextBounds(cellTextBoxBase, LayoutPage);
				rect.X -= parentRect.X;
				rect.Y -= parentRect.Y;
			} else {
				parentRect = cellTextBoxBase.GetWebBounds(LayoutPage);
				rect = GetWebTextBounds(cellTextBoxBase, LayoutPage);
			}
			int gridLineSizeCorrection = 0;
			WebControl divText;
			WebControl divTextWrapper;
			if(isComplexBox) {
				divTextWrapper = RenderProvider.CreateDiv(rect, "dxss-tw");
				divText = RenderProvider.CreateDivWithSizesOnly(rect, "dxss-tb", gridLineSizeCorrection);
				parent.Controls.Add(divTextWrapper);
				divTextWrapper.Controls.Add(divText);
			} else {
				divText = RenderProvider.CreateDivWithSizesOnly(rect, "dxss-tb", gridLineSizeCorrection);
				parent.Controls.Add(divText);
			}
			WebControl parentElement = divText;
			int hyperLinkIndex = LayoutPage.Sheet.Hyperlinks.GetHyperlink(cell);
			if(hyperLinkIndex > -1) {
				var modelHyperLink = LayoutPage.Sheet.Hyperlinks[hyperLinkIndex];
				WebControl linkElement = new WebControl(HtmlTextWriterTag.A);
				linkElement.Attributes["href"] = "#";
				linkElement.Attributes.Add("_loc", modelHyperLink.TargetUri);
				linkElement.Attributes.Add("onClick", "javascript:return ASPx.SSOnHyperLinkClicked(event, this);");
				linkElement.Attributes["title"] = modelHyperLink.TooltipText;
				parentElement.Controls.Add(linkElement);
				parentElement = linkElement;
			}
			parentElement.Controls.Add(new LiteralControl(HttpUtility.HtmlEncode(cell.DisplayText)));
			StyleHelper.AssignFontToElement(RenderHelper.Model, divText, cell);
			StyleHelper.AssignAlignToElement(divText, cell.ActualAlignment, cell.ActualHorizontalAlignment);
			StyleHelper.AssignWrapTextToElement(divText, cell.ActualAlignment);
			if(cell.ActualAlignment.WrapText)
				parent.Style.Add(HtmlTextWriterStyle.Overflow, "hidden");
			if(!isComplexBox)
				StyleHelper.AssignRectDifferenceAsPaddings(divText, rect, parentRect, gridLineSizeCorrection);
			ExtractDataAttributesFromCellPosition(divText, cell.Position, "tb");
		}
		protected void CreateBorders(List<PageBorderCollection> borders, string className) {
			foreach(var border in borders)
				foreach(var box in border.Boxes)
					CreateBorder(border, box, className);
		}
		protected void CreateBorder(PageBorderCollection pageBorderCollection, BorderLineBox borderLineBox, string className) {
			if(borderLineBox.LineStyle == XlBorderLineStyle.None)
				return;
			var rect = GetCorrectedWebBounds(borderLineBox, pageBorderCollection);
			var color = RenderHelper.Model.Cache.ColorModelInfoCache[borderLineBox.ColorIndex].ToRgb(RenderHelper.Model.StyleSheet.Palette, RenderHelper.Model.OfficeTheme.Colors);
			string borderTypeStyleClassCame = StyleHelper.GetBorderTypeClassName(borderLineBox.LineStyle);
			var div = RenderProvider.CreateDiv(rect, className + " " + borderTypeStyleClassCame, color);
			Controls.Add(div);
		}
		protected Rectangle GetCorrectedWebBounds(BorderLineBox borderLineBox, PageBorderCollection pageBorderCollection) {
			Rectangle borderRect = borderLineBox.GetWebBounds(LayoutPage, pageBorderCollection);
			int lineThickness = 0;
			if(borderLineBox.LineStyle == XlBorderLineStyle.Double)
				lineThickness = lineThickness / 2;
			else 
				lineThickness = LayoutPage.DocumentLayout.LineThicknessTable[borderLineBox.LineStyle];
			borderRect.Width += lineThickness;
			return borderRect;
		}
		void ExtractDataAttributesFromCellPosition(WebControl div, CellPosition cellPosition, string type) { 
			string cellPostion = string.Format("{0}.{1}", cellPosition.Column.ToString(), cellPosition.Row.ToString());
			div.ID = string.Format("{0}.{1}.{2}", type, cellPosition.Column.ToString(), cellPosition.Row.ToString());
		}
	}
	public class SpreadsheetFormulaBarControl : SpreadsheetControlBase {
		protected internal WebControl MainElement { get; set; }
		protected internal ASPxTextBox TextBox { get; set; }
		protected internal SpreadsheetFormulaBarMenu Menu { get; set; }
		public SpreadsheetFormulaBarControl(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		protected SpreadsheetFormulaBarStyles StylesFormulaBar {
			get { return Spreadsheet.StylesFormulaBar; }
		}
		protected override void ClearControlFields() {
			this.MainElement = null;
			this.TextBox = null;
			this.Menu = null;
		}
		protected SpreadsheetFormulaBarMenu CreateMenu() {
			SpreadsheetFormulaBarMenu menu = new SpreadsheetFormulaBarMenu();
			menu.ID = ASPxSpreadsheet.SpreadsheetFormulaBarMenuID;
			menu.EnableViewState = false; 
			menu.ParentSkinOwner = Spreadsheet;
			menu.Items.Add("", ASPxSpreadsheet.SpreadsheetFormulaBarMenuCancelItemName);
			menu.Items.Add("", ASPxSpreadsheet.SpreadsheetFormulaBarMenuEnterItemName);
			menu.Items[0].ToolTip = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FormulaBar_CancelButtonTooltip);
			menu.Items[1].ToolTip = ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.FormulaBar_EnterButtonTooltip);
			menu.Items[0].Image.CopyFrom(Spreadsheet.GetFormulaBarMenuCancelItemImageProperties());
			menu.Items[1].Image.CopyFrom(Spreadsheet.GetFormulaBarMenuEnterItemImageProperties());
			menu.Items[0].ClientEnabled = false;
			menu.Items[1].ClientEnabled = false;
			menu.ClientSideEvents.ItemClick = Spreadsheet.GetFormulaBarMenuOnItemClickScript();
			return menu;
		}
		protected override void CreateControlHierarchy() {
			this.MainElement = RenderUtils.CreateDiv();
			this.MainElement.ID = ASPxSpreadsheet.SpreadsheetFormulaBarID;
			this.Menu = CreateMenu();
			this.MainElement.Controls.Add(Menu);
			this.TextBox = new ASPxTextBox();
			this.TextBox.ID = ASPxSpreadsheet.SpreadsheetFormulaBarTextBoxID;
			this.MainElement.Controls.Add(TextBox);
			Controls.Add(this.MainElement);
		}
		protected override void PrepareControlHierarchy() {
			StylesFormulaBar.GetMainElementStyle().AssignToControl(this.MainElement, true);
			StylesFormulaBar.GetTextBoxStyle().AssignToControl(this.TextBox, true);
			PrepareMenu();
		}
		protected void PrepareMenu() {
			SpreadsheetFormulaBarButtonSectionStyles buttonSectionStyles = StylesFormulaBar.GetButtonSectionStyles();
			this.Menu.Styles.Item.Assign(buttonSectionStyles.Button);
			this.Menu.Styles.Style.Assign(buttonSectionStyles.MainElement);
			MenuItem cancelItem = this.Menu.Items.FindByName(ASPxSpreadsheet.SpreadsheetFormulaBarMenuCancelItemName);
			MenuItem enterItem = this.Menu.Items.FindByName(ASPxSpreadsheet.SpreadsheetFormulaBarMenuEnterItemName);
			cancelItem.ItemStyle.Assign(StylesFormulaBar.GetCancelButtonStyle());
			enterItem.ItemStyle.Assign(StylesFormulaBar.GetEnterButtonStyle());
		}
	}
	[ToolboxItem(false)]
	public class SpreadsheetFormulaBarMenu : ASPxMenu {
		protected internal new MenuStyles Styles {
			get { return base.Styles; }
		}
	}
	[ToolboxItem(false)]
	public class SpreadsheetFunctionsListBox : ASPxListBox {
		const string ControlSystemClassName = SpreadsheetStyles.CssClassPrefix + "-funcLB";
		ASPxSpreadsheet Spreadsheet { get; set; }
		public SpreadsheetFunctionsListBox(ASPxSpreadsheet spreadsheet) {
			this.EnableClientSideAPI = true;
			this.EnableViewState = false;
			this.ParentSkinOwner = spreadsheet;
			this.Spreadsheet = spreadsheet;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, ControlSystemClassName);
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(SpreadsheetFunctionsListBox), ASPxSpreadsheet.SpreadsheetFunctionsListBoxScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPx.SpreadsheetFunctionsListBox";
		}
		protected override void GetCreateClientObjectScript(System.Text.StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			AppearanceStyle functionDescriptionHintStyle = Spreadsheet.StylesFormulaAutoCompete.GetFunctionInfoStyle();
			stb.AppendFormat("{0}.functionDescriptionHintStyle=[{1}, {2}];\n", localVarName,
				HtmlConvertor.ToScript(functionDescriptionHintStyle.CssClass),
				HtmlConvertor.ToScript(functionDescriptionHintStyle.GetStyleAttributes(Page).Value));
		}
	}
	public class SpreadsheetFormulaBarControlDesignTime : SpreadsheetFormulaBarControl {
		public SpreadsheetFormulaBarControlDesignTime(ASPxSpreadsheet spreadsheet)
			: base(spreadsheet) {
		}
		protected override void CreateControlHierarchy() {
			this.MainElement = RenderUtils.CreateTable();
			this.MainElement.ID = ASPxSpreadsheet.SpreadsheetFormulaBarID;
			TableRow row = RenderUtils.CreateTableRow();
			TableCell tc1 = RenderUtils.CreateTableCell();
			TableCell tc2 = RenderUtils.CreateTableCell();
			tc1.Width = Unit.Pixel(0);
			tc2.Width = Unit.Percentage(100);
			((System.Web.UI.WebControls.Table)this.MainElement).Rows.Add(row);
			row.Cells.Add(tc1);
			row.Cells.Add(tc2);
			this.Menu = CreateMenu();
			tc1.Controls.Add(Menu);
			this.TextBox = new ASPxTextBox();
			this.TextBox.ID = ASPxSpreadsheet.SpreadsheetFormulaBarTextBoxID;
			this.TextBox.Width = Unit.Percentage(100);
			tc2.Controls.Add(TextBox);
			Controls.Add(this.MainElement);
		}
	}
	public static class StyleHelper {
		static Dictionary<XlBorderLineStyle, string> borderTypes;
		const double defaultFontSize = 0;
		const string wrappedTextDivClassName = " " + "dxss-wrap";
		static readonly object locker = new object();
		static Dictionary<XlBorderLineStyle, string> BorderTypes {
			get {
				if(borderTypes == null) {
					lock(locker) {
						if(borderTypes == null) {
							borderTypes = new Dictionary<XlBorderLineStyle, string>() {
								{ XlBorderLineStyle.Thin, "dxss-bt-t" },
								{ XlBorderLineStyle.Hair, "dxss-bt-h" },
								{ XlBorderLineStyle.Dotted, "dxss-bt-d" },
								{ XlBorderLineStyle.Dashed, "dxss-bt-da" },
								{ XlBorderLineStyle.DashDot, "dxss-bt-ddd" },
								{ XlBorderLineStyle.DashDotDot, "dxss-bt-ddd" },
								{ XlBorderLineStyle.Double, "dxss-bt-dbl" },
								{ XlBorderLineStyle.Medium, "dxss-bt-m" },
								{ XlBorderLineStyle.MediumDashed, "dxss-bt-md" },
								{ XlBorderLineStyle.MediumDashDot, "dxss-bt-mdd" },
								{ XlBorderLineStyle.MediumDashDotDot, "dxss-bt-mddd" },
								{ XlBorderLineStyle.SlantDashDot, "dxss-bt-sdd" },
								{ XlBorderLineStyle.Thick, "dxss-bt-tk" },
							};
						}
					}
				}
				return borderTypes;
			}
		}
		public static void AssignFontToElement(DocumentModel model, WebControl element, ICell cell) {
			IActualRunFontInfo actualRunFontInfo = cell.ActualFont;
			if(actualRunFontInfo.Bold)
				element.Style.Add("font-weight", "bold");
			Color foreColor = GetForeColor(model, cell);
			if(!foreColor.IsEmpty)
				element.ForeColor = Color.FromArgb(foreColor.ToArgb());
			if(actualRunFontInfo.Italic)
				element.Style.Add(HtmlTextWriterStyle.FontStyle, "italic");
			if(!string.IsNullOrEmpty(actualRunFontInfo.Name))
				element.Style.Add(HtmlTextWriterStyle.FontFamily, actualRunFontInfo.Name);
			if(actualRunFontInfo.Size != defaultFontSize) {
				double fontSize = actualRunFontInfo.Size;
				element.Style.Add(HtmlTextWriterStyle.FontSize, string.Format("{0}pt", fontSize));
			}
			string textDecoration = string.Empty;
			if(actualRunFontInfo.Underline != XlUnderlineType.None)
				textDecoration = "underline";
			if(actualRunFontInfo.StrikeThrough)
				textDecoration = textDecoration + " line-through";
			if(!string.IsNullOrEmpty(textDecoration))
				element.Style.Add(HtmlTextWriterStyle.TextDecoration, textDecoration);
		}
		private static Color GetForeColor(DocumentModel model, ICell cell) {
			NumberFormatParameters parameters = new NumberFormatParameters();
			NumberFormatResult formatResult = cell.GetFormatResult(parameters);
			IActualRunFontInfo actualRunFontInfo = cell.ActualFont;
			Color foreColor = Cell.GetTextColor(model, actualRunFontInfo, formatResult, false);
			return foreColor;
		}
		public static string GetBorderTypeClassName(XlBorderLineStyle XlBorderLineStyle) {
			return BorderTypes[XlBorderLineStyle];
		}
		public static void AssignFillStyleToElement(DocumentModel model, WebControl element, ICell cell) {
			Color backColor = GetBackColor(model, cell);
			if(!DXColor.IsTransparentOrEmpty(backColor))
				element.BackColor = backColor;
		}
		private static Color GetBackColor(DocumentModel model, ICell cell) {
			CellBackgroundDisplayFormat result = new CellBackgroundDisplayFormat();
			result.Cell = cell;
			result.PatternType = cell.ActualPatternType;
			result.BackColor = cell.ActualBackgroundColor;
			if(result.ShouldUseForeColor)
				result.ForeColor = CellTextBoxBase.GetFillForeColor(cell.ActualForegroundColor, model);
			if(DXColor.IsTransparentOrEmpty(result.BackColor)) {
				IActualFillInfo actualFill = cell.ActualFill;
				if(actualFill.FillType == ModelFillType.Gradient)
					result.GradientFill = actualFill.GradientFill;
			}
			return result.BackColor;
		}
		public static void AssignAlignToElement(WebControl element, IActualCellAlignmentInfo actualCellAlignmentInfo, XlHorizontalAlignment actualHorizontalAlignment) {
			if(actualHorizontalAlignment != XlHorizontalAlignment.General)
				element.Style.Add(HtmlTextWriterStyle.TextAlign, actualHorizontalAlignment.ToString().ToLower());
			if(actualCellAlignmentInfo.Vertical != XlVerticalAlignment.Distributed) { 
				element.Style.Add(HtmlTextWriterStyle.Display, "table-cell");
				string verticalAlign = actualCellAlignmentInfo.Vertical.ToString().ToLower();
				if(verticalAlign == "center")
					verticalAlign = "middle";
				element.Style.Add(HtmlTextWriterStyle.VerticalAlign, verticalAlign);
			}
		}
		public static void AssignWrapTextToElement(WebControl element, IActualCellAlignmentInfo actualCellAlignmentInfo) {
			if(actualCellAlignmentInfo.WrapText)
				element.CssClass += wrappedTextDivClassName;
		}
		public static void AssignRectDifferenceAsPaddings(WebControl div, Rectangle rect, Rectangle parentRect, int gridLineSizeCorrection) {
			int topPadding = parentRect.Y - rect.Y + gridLineSizeCorrection;
			int leftPadding = rect.X - parentRect.X;
			string paddings = string.Format("{0}px 0 0 {1}px", topPadding, leftPadding);
			div.Style.Add(HtmlTextWriterStyle.Padding, paddings);
		}
	}
	public static class WebSpreadsheetControlUtils {
		public static InnerSpreadsheetControl GetInnerControl(ASPxSpreadsheet aspxSpreadsheet) {
			var currentWorkSession = aspxSpreadsheet.GetCurrentWorkSessions();
			if(currentWorkSession != null)
				return currentWorkSession.WebSpreadsheetControl.InnerControl;
			return CreateTemporalInnerControl();
		}
		static InnerSpreadsheetControl CreateTemporalInnerControl() {
			var webSpreadsheetControl = new WebSpreadsheetControl();
			return webSpreadsheetControl.InnerControl;
		}
	}
}
