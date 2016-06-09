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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.ComponentModel;
namespace DevExpress.Web.ASPxTreeList.Internal {
	public abstract class TreeListInternalWebControl : ASPxInternalWebControl {
		TreeListRenderHelper renderHelper;
		public TreeListInternalWebControl(TreeListRenderHelper helper)
			: base() {
			this.renderHelper = helper;
		}
		protected TreeListRenderHelper RenderHelper { get { return renderHelper; } }
	}
	public class TreeListDataTable : TreeListTableBase, ITreeListBuilder {
		const string DataTableClassName = TreeListStyles.Prefix + "DataTable";
		TreeListDataTableRenderPart renderPart;
		public TreeListDataTable(TreeListRenderHelper helper)
			: this(helper, TreeListDataTableRenderPart.All) {
		}
		public TreeListDataTable(TreeListRenderHelper helper, TreeListDataTableRenderPart renderPart)
			: base(helper) {
			this.renderPart = renderPart;
			ID = RenderHelper.GetDataTableID(RenderPart);
			CssClass = DataTableClassName;
		}
		protected TreeListDataTableRenderPart RenderPart { get { return renderPart; } }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.RequireFixedTableLayout)
				Rows.Add(new TreeListArmRow(RenderHelper, RenderPart == TreeListDataTableRenderPart.Header || RenderPart == TreeListDataTableRenderPart.All));
			TreeListBuilderHelper.Build(this, RenderPart);
			CreateHiddenRow();
			if(RenderPart == TreeListDataTableRenderPart.Content && RenderHelper.Rows.Count == 0)
				CreateEmptyRow();
		}
		protected virtual void CreateHiddenRow() {
			foreach(TableRow r in Rows) {
				if(r.TableSection == TableRowSection.TableBody)
					return;
			}
			var row = CreateEmptyRow();
			row.ID = TreeListRenderHelper.HiddenEmptyRowID;
			row.Style["display"] = "none";
		}
		protected virtual TableRow CreateEmptyRow() {
			var row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			var cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			cell.Text = "&nbsp;";
			cell.ColumnSpan = RenderHelper.GetTotalCellCount();
			return row;
		}
		protected override void PrepareControlHierarchy() {
			Width = Unit.Percentage(100);
			CellPadding = 0;
			TreeListRenderHelper.SetZeroCellSpacing(this);
			if(RenderHelper.IsNodeDragDropEnabled)
				RenderUtils.SetStringAttribute(this, TouchUtils.TouchMouseDownEventName, RenderHelper.GetDataTableOnMouseDown());
			else
				RenderUtils.SetStringAttribute(this, "onclick", RenderHelper.GetDataTableOnClick());
			if(RenderHelper.IsDataDataTableDblClickRequired)
				RenderUtils.SetStringAttribute(this, "ondblclick", RenderHelper.GetDataTableOnDblClick());
			if(RenderHelper.RequireFixedTableLayout)
				Style["table-layout"] = "fixed";
			if(RenderHelper.IsRightToLeft)
				Attributes["dir"] = "rtl";
			if(!RenderUtils.IsHtml5Mode(this))
				RenderUtils.SetStringAttribute(this, "summary", RenderHelper.TreeList.SummaryText);
			else if(RenderHelper.TreeList.IsAccessibilityCompliantRender()) {
					RenderUtils.SetStringAttribute(this, "aria-label", GetAccessibilityDescriptionText());
					RenderUtils.SetStringAttribute(this, "role", "application"); 
			}   
		}
		string GetAccessibilityDescriptionText() {
			int rowCount = RenderHelper.TreeList.GetDataTable().Rows.Count;
			if(RenderHelper.TreeList.Settings.ShowColumnHeaders)
				rowCount--;
			string description = string.Format("This is TreeList with {0} rows and {1} columns.", rowCount, RenderHelper.TreeList.VisibleColumns.Count);
			if(!Page.IsCallback) {
				description += "Use Tab to navigate. Use Enter key to Expand/Collapse nodes.";
				if(RenderHelper.TreeList.SettingsSelection.Enabled)
					description += "Use Checkboxes to select node.";
			}
			return description;
		}
		#region ITreeListBuilder Members
		TreeListRenderHelper ITreeListBuilder.RenderHelper { get { return RenderHelper; } }
		void ITreeListBuilder.CreateHeader() {
			Rows.Add(new TreeListHeaderRow(RenderHelper));
		}
		void ITreeListBuilder.CreateDataRow(int rowIndex) {
			Rows.Add(new TreeListDataRow(RenderHelper, rowIndex));
		}
		void ITreeListBuilder.CreatePreview(int rowIndex) {
			Rows.Add(new TreeListPreviewRow(RenderHelper, rowIndex));
		}
		void ITreeListBuilder.CreateGroupFooter(int rowIndex) {
			Rows.Add(new TreeListFooterRow(RenderHelper, rowIndex, false));
		}
		void ITreeListBuilder.CreateFooter(int rowIndex) {
			Rows.Add(new TreeListFooterRow(RenderHelper, rowIndex, true));
		}
		void ITreeListBuilder.CreateInlineEditRow(int rowIndex) {
			Rows.Add(new TreeListInlineEditRow(RenderHelper, rowIndex));
		}
		void ITreeListBuilder.CreateEditFormRow(int rowIndex, bool isAuxRow) {
			Rows.Add(new TreeListEditFormRow(RenderHelper, rowIndex, isAuxRow));
		}
		void ITreeListBuilder.CreateErrorRow(int rowIndex) {
			Rows.Add(new TreeListErrorRow(RenderHelper, rowIndex));
		}
		#endregion
	}
	public abstract class TreeListStyleTableBase : TreeListTableBase {
		TableRow mainRow;
		TableCell focusedCell, selectedCell;
		public TreeListStyleTableBase(TreeListRenderHelper helper)
			: base(helper) {
			ID = TreeListRenderHelper.StyleTableID;
		}
		protected TableRow MainRow { get { return mainRow; } }
		protected TableCell FocusedCell { get { return focusedCell; } }
		protected TableCell SelectedCell { get { return selectedCell; } }
		protected override void CreateControlHierarchy() {
			this.mainRow = RenderUtils.CreateTableRow();
			this.focusedCell = RenderUtils.CreateTableCell();
			this.selectedCell = RenderUtils.CreateTableCell();
			MainRow.Cells.Add(FocusedCell);
			MainRow.Cells.Add(SelectedCell);
			CreateStyledCells();
			Rows.Add(MainRow);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.SetVisibility(this, false, true);
			PrepareStyledCell(FocusedCell, RenderHelper.GetFocusedNodeStyle());
			PrepareStyledCell(SelectedCell, RenderHelper.GetSelectedNodeStyle());
			PrepareStyledCells();
		}
		protected void PrepareStyledCell(TableCell cell, Style style) {
			cell.CssClass = style.CssClass;
			cell.Text = style.GetStyleAttributes(Page).Value;
		}
		protected abstract void CreateStyledCells();
		protected abstract void PrepareStyledCells();
	}
	public class TreeListSimpleStyleTable : TreeListStyleTableBase {
		TableCell nodeCell;
		public TreeListSimpleStyleTable(TreeListRenderHelper helper)
			: base(helper) {
		}
		protected TableCell NodeCell { get { return nodeCell; } }
		protected override void CreateStyledCells() {
			this.nodeCell = RenderUtils.CreateTableCell();
			MainRow.Cells.Add(NodeCell);
		}
		protected override void PrepareStyledCells() {
			PrepareStyledCell(NodeCell, RenderHelper.GetNodeStyle());
		}
	}
	public class TreeListExactStyleTable : TreeListStyleTableBase {
		Table prototypeTable;
		public TreeListExactStyleTable(TreeListRenderHelper helper, Table prototypeTable)
			: base(helper) {
			this.prototypeTable = prototypeTable;
		}
		protected Table PrototypeTable { get { return prototypeTable; } }
		protected override void CreateStyledCells() {
			foreach(TableRow row in PrototypeTable.Rows) {
				TableCell cell = RenderUtils.CreateTableCell();
				MainRow.Cells.Add(cell);
			}
		}
		protected override void PrepareStyledCells() {
			int index = 2;
			foreach(TableRow row in PrototypeTable.Rows) {
				TreeListDataRow dataRow = row as TreeListDataRow;
				if(dataRow != null) {
					TableCell cell = MainRow.Cells[index];
					PrepareStyledCell(cell, dataRow.SavedStyle);
				}
				index++;
			}
		}
	}
	[ViewStateModeById]
	public class TreeListUpdatableContainer : TreeListInternalWebControl {
		TreeListDataTable dataTable;
		public TreeListUpdatableContainer(TreeListRenderHelper helper)
			: base(helper) {
		}
		protected internal TreeListDataTable DataTable { get { return dataTable; } }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.IsTopPagerVisible)
				Controls.Add(new TreeListPagerPanel(RenderHelper, PagerPosition.Top));
			CreateDataTable();
			if(RenderHelper.HasScrolling)
				Controls.Add(new TreeListScrollableControl(RenderHelper, DataTable));
			else
				Controls.Add(DataTable);
			if(RenderHelper.IsBottomPagerVisible)
				Controls.Add(new TreeListPagerPanel(RenderHelper, PagerPosition.Bottom));
			if(RenderHelper.NeedRenderStyleTable) {
				TreeListStyleTableBase styleTable = RenderHelper.NeedExactStyleTable
					? new TreeListExactStyleTable(RenderHelper, DataTable) as TreeListStyleTableBase
					: new TreeListSimpleStyleTable(RenderHelper) as TreeListStyleTableBase;
				Controls.Add(styleTable);
			}
			if(RenderHelper.NeedRenderCustomizationWindow)
				Controls.Add(new TreeListCustomizationWindow(RenderHelper));
			if(RenderHelper.RenderPopupEditForm)
				Controls.Add(new TreeListPopupEditForm(RenderHelper));
		}
		protected override void PrepareControlHierarchy() {
		}
		void CreateDataTable() {
			TreeListDataTableRenderPart rPart = TreeListDataTableRenderPart.All;
			if(RenderHelper.HasVerticalScrollBar)
				rPart = TreeListDataTableRenderPart.Content;
			this.dataTable = new TreeListDataTable(RenderHelper, rPart);
		}
	}
	public class TreeListServiceControl : TreeListInternalWebControl {
		Image dragAndDropArrowDown, dragAndDropArrowUp, dragAndDropHide, dragAndDropNode;
		Dictionary<HiddenField, string> inputValues;
		LiteralControl inputValuesScript;
		bool inputValuesScriptPrepared = false;
		public TreeListServiceControl(TreeListRenderHelper helper)
			: base(helper) {
			this.inputValues = new Dictionary<HiddenField, string>();
		}
		protected ASPxTreeList TreeList { get { return RenderHelper.TreeList; } }
		protected TreeListDataHelper TreeDataHelper { get { return RenderHelper.TreeDataHelper; } }
		protected bool UseFixBehavior { get { return Browser.Family.IsNetscape && Page != null && !Page.IsPostBack; } }
		protected Image DragAndDropArrowDown { get { return dragAndDropArrowDown; } }
		protected Image DragAndDropArrowUp { get { return dragAndDropArrowUp; } }
		protected Image DragAndDropHide { get { return dragAndDropHide; } }
		protected Image DragAndDropNode { get { return dragAndDropNode; } }
		protected override void CreateControlHierarchy() {
			if(RenderHelper.IsColumnDragDropEnabled) {
				this.dragAndDropArrowDown = RenderUtils.CreateImage();
				this.dragAndDropArrowUp = RenderUtils.CreateImage();
				this.dragAndDropHide = RenderUtils.CreateImage();
				Controls.Add(DragAndDropArrowDown);
				Controls.Add(DragAndDropArrowUp);
				Controls.Add(DragAndDropHide);
			}
			if(RenderHelper.IsNodeDragDropEnabled) {
				this.dragAndDropNode = RenderUtils.CreateImage();
				Controls.Add(DragAndDropNode);
			}
			this.inputValuesScript = RenderUtils.CreateLiteralControl();
			Controls.Add(this.inputValuesScript);
		}
		protected override void PrepareControlHierarchy() {
			PrepareInputValuesScript();
			if(DragAndDropArrowDown != null)
				PrepareDragAndDropImage(DragAndDropArrowDown, TreeListRenderHelper.DragAndDropArrowDownID, TreeListImages.DragAndDropArrowDownName);
			if(DragAndDropArrowUp != null)
				PrepareDragAndDropImage(DragAndDropArrowUp, TreeListRenderHelper.DragAndDropArrowUpID, TreeListImages.DragAndDropArrowUpName);
			if(DragAndDropHide != null)
				PrepareDragAndDropImage(DragAndDropHide, TreeListRenderHelper.DragAndDropHideID, TreeListImages.DragAndDropHideName);
			if(DragAndDropNode != null)
				PrepareDragAndDropImage(DragAndDropNode, TreeListRenderHelper.DragAndDropNodeID, TreeListImages.DragAndDropNodeName);
		}
		protected void PrepareDragAndDropImage(Image image, string id, string name) {
			image.ID = id;
			RenderHelper.GetImage(name).AssignToControl(image, DesignMode);
			RenderUtils.SetStyleStringAttribute(image, "visibility", "hidden");
			RenderUtils.SetStyleStringAttribute(image, "position", "absolute");
			RenderUtils.SetStyleStringAttribute(image, "z-index", RenderUtils.LoadingPanelZIndex.ToString());
			RenderUtils.SetStyleStringAttribute(image, "top", "-100px"); 
		}
		void SetHiddenInputValue(HiddenField input, string value) {
			if(this.inputValuesScriptPrepared)
				throw new InvalidOperationException();
			if(UseFixBehavior || String.IsNullOrEmpty(value)) {
				input.Value = "!";
				input.Value = String.Empty;
				this.inputValues.Add(input, value);
			} else {
				input.Value = value;
			}
		}
		void PrepareInputValuesScript() {
			if(!UseFixBehavior) return;
			if(this.inputValuesScript == null) return;
			StringBuilder builder = new StringBuilder();
			foreach(HiddenField control in this.inputValues.Keys) {
				builder.AppendFormat("document.getElementById({0}).value={1};\n",
					HtmlConvertor.ToScript(control.ClientID),
					HtmlConvertor.ToScript(this.inputValues[control]));
			}
			this.inputValuesScript.Text = RenderUtils.GetScriptHtml(builder.ToString());
			this.inputValuesScriptPrepared = true;
		}
	}
	public class TreeListMainTable : TreeListTableBase {
		TableCell updatableCell;
		TreeListUpdatableContainer containerControl;
		public TreeListMainTable(TreeListRenderHelper helper)
			: base(helper) {
		}
		protected TableCell UpdatableCell { get { return updatableCell; } }
		protected internal TreeListUpdatableContainer ContainerControl { get { return containerControl; } }
		protected override void CreateControlHierarchy() {
			TableRow row = RenderUtils.CreateTableRow();
			Rows.Add(row);
			this.updatableCell = RenderUtils.CreateTableCell();
			UpdatableCell.ID = TreeListRenderHelper.UpdatableCellID;
			row.Cells.Add(UpdatableCell);
			this.containerControl = new TreeListUpdatableContainer(RenderHelper);
			UpdatableCell.Controls.Add(ContainerControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AssignAttributes(RenderHelper.TreeList, this);
			if(RenderHelper.TreeList.KeyboardSupport)
				AccessKey = String.Empty;
			RenderUtils.SetVisibility(this, RenderHelper.TreeList.IsClientVisible(), true);
			CellPadding = 0;
			TreeListRenderHelper.SetZeroCellSpacing(this);
			AppearanceStyle rootStyle = RenderHelper.GetRootControlStyle();
			rootStyle.AssignToControl(this);
			if(RenderUtils.Browser.IsIE && rootStyle.Paddings.IsEmpty) 
				rootStyle.Paddings.MergeWith(new Paddings(0));
			rootStyle.Paddings.AssignToControl(UpdatableCell);
			UpdatableCell.VerticalAlign = VerticalAlign.Top;
			Caption = RenderHelper.TreeList.Caption;
			if(RenderUtils.Browser.IsIE && Width.IsEmpty && string.IsNullOrEmpty(Attributes["width"]))
				Attributes["width"] = "1";
			if(RenderUtils.IsHtml5Mode(this) && RenderHelper.TreeList.IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(this, "role", "presentation");
		}
	}
	public class TreeListPagerPanel : TreeListInternalWebControl {
		WebControl panel;
		PagerPosition position;
		TreeListPager pagerControl;
		public TreeListPagerPanel(TreeListRenderHelper helper, PagerPosition position)
			: base(helper) {
			if(position != PagerPosition.Top && position != PagerPosition.Bottom)
				throw new ArgumentException("position");
			this.position = position;
			ID = TreeListRenderHelper.PagerPanelID + position.ToString();
		}
		protected WebControl Panel { get { return panel; } }
		protected PagerPosition Position { get { return position; } }
		protected TreeListPager PagerControl { get { return pagerControl; } }
		protected override void CreateControlHierarchy() {
			this.panel = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			this.pagerControl = new TreeListPager(RenderHelper.TreeList);
			Panel.Controls.Add(PagerControl);
			Controls.Add(Panel);
		}
		protected override void PrepareControlHierarchy() {
			TreeListPagerPanelStyle style = Position == PagerPosition.Top
				? RenderHelper.GetPagerTopPanelStyle()
				: RenderHelper.GetPagerBottomPanelStyle();
			style.AssignToControl(Panel, true);
			Unit spacing = style.Spacing;
			if(!spacing.IsEmpty) {
				RenderUtils.SetVerticalMargins(Panel,
					Position == PagerPosition.Bottom ? spacing : Unit.Empty,
					Position == PagerPosition.Top ? spacing : Unit.Empty);
			}
			PagerControl.ForeColor = style.ForeColor;
			PagerControl.Font.CopyFrom(style.Font);
		}
	}
	[ToolboxItem(false), ViewStateModeById]
	public class TreeListPopupEditForm : ASPxPopupControl {
		TreeListRenderHelper helper;
		InternalWebControl contentContainer;
		InternalTable editFormTable;
		TableCell errorCell;
		public TreeListPopupEditForm(TreeListRenderHelper helper)
			: base(helper.TreeList) {
			this.helper = helper;
			ParentSkinOwner = TreeList;
			EnableViewState = false;
			PopupAnimationType = AnimationType.Fade;
			AllowDragging = true;
			CloseAction = CloseAction.CloseButton;
			ID = TreeListRenderHelper.PopupEditFormID;
		}
		protected ASPxTreeList TreeList { get { return RenderHelper.TreeList; } }
		protected TreeListSettingsPopupEditForm Settings { get { return TreeList.SettingsPopupEditForm; } }
		protected TreeListRenderHelper RenderHelper { get { return helper; } }
		protected TreeListStyles TreeListStyles { get { return TreeList.Styles; } }
		protected InternalWebControl ContentContainer { get { return contentContainer; } }
		protected InternalTable EditFormTable { get { return editFormTable; } }
		protected TableCell ErrorCell { get { return errorCell; } }
		protected internal override Paddings GetContentPaddings(PopupWindow window) {
			if(TreeListStyles.PopupEditFormWindowContent.Paddings.IsEmpty)
				return new Paddings(0);
			return TreeListStyles.PopupEditFormWindowContent.Paddings;
		}
		protected override bool BindContainersOnCreate() {
			return false;
		}
		protected override bool NeedCreateHierarchyOnInit() { return false; }
		protected override void EnsureChildControls() {
			base.EnsureChildControls();
			if(EditFormTable != null)
				((IASPxWebControl)EditFormTable).EnsureChildControls();
		}
		protected override void CreateControlHierarchy() {
			ShowHeader = Settings.ShowHeader;
			Modal = Settings.Modal;
			AllowResize = Settings.AllowResize;
			MinWidth = Settings.MinWidth;
			MinHeight = Settings.MinHeight;
			base.CreateControlHierarchy();
			this.contentContainer = new InternalWebControl(HtmlTextWriterTag.Div);
			Controls.Add(ContentContainer);
			if(!AddTemplateControl()) {
				this.editFormTable = new TreeListEditFormTable(RenderHelper, TreeListRenderHelper.PopupEditFormRowIndex, true);
				ContentContainer.Controls.Add(EditFormTable);
			}
			ContentContainer.Controls.Add(CreateErrorTable());
			EnsureChildControlsRecursive(this, false);
		}
		bool AddTemplateControl() {
			Control templateContainer = RenderHelper.CreateEditFormTemplateContainer(ContentContainer, TreeListRenderHelper.PopupEditFormRowIndex);			
			return templateContainer != null;
		}
		Table CreateErrorTable() {
			Table table = RenderUtils.CreateTable();
			table.Width = Unit.Percentage(100);
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			this.errorCell = RenderUtils.CreateTableCell();
			ErrorCell.ID = TreeListRenderHelper.ErrorCellSuffix;
			row.Controls.Add(ErrorCell);
			return table;
		}
		protected override void PrepareControlHierarchy() {
			ShowOnPageLoad = false;
			PopupElementID = GetPopupElementID();
			HeaderText = Settings.Caption;
			PopupHorizontalAlign = Settings.HorizontalAlign;
			PopupVerticalAlign = Settings.VerticalAlign;
			PopupHorizontalOffset = Settings.HorizontalOffset;
			PopupVerticalOffset = Settings.VerticalOffset;
			if(!Settings.Width.IsEmpty)
				Width = Settings.Width;
			Font.CopyFrom(TreeList.Font);
			ControlStyle.CopyFrom(TreeListStyles.PopupEditFormWindow);
			HeaderStyle.CopyFrom(TreeListStyles.PopupEditFormWindowHeader);
			ContentStyle.CopyFrom(TreeListStyles.PopupEditFormWindowContent);
			CloseButtonStyle.CopyFrom(TreeListStyles.PopupEditFormWindowCloseButton);
			CloseButtonImage.CopyFrom(TreeList.Images.PopupEditFormWindowClose);
			base.PrepareControlHierarchy();
			ClientSideEvents.Init = "function(s, e){ s.Show(); }";
			ClientSideEvents.CloseButtonClick = string.Format("function(){{ {0} }}", 
				RenderHelper.GetCommandButtonOnClick(TreeListCommandColumnButtonType.Cancel, string.Empty));
			RenderHelper.GetPopupEditFormStyle().AssignToControl(ContentContainer, true);
			if(!Settings.Height.IsEmpty) {
				ContentContainer.Height = Settings.Height;
				ContentContainer.Style[HtmlTextWriterStyle.OverflowY] = "auto";
			}
			PrepareErrorCell();
		}
		void PrepareErrorCell() {
			ErrorCell.Text = RenderHelper.TreeDataHelper.EditingNodeError;
			RenderHelper.GetErrorStyle().AssignToControl(ErrorCell, true);
			RenderUtils.SetVisibility((TableRow)ErrorCell.Parent, !String.IsNullOrEmpty(RenderHelper.TreeDataHelper.EditingNodeError), true);
		}
		string GetPopupElementID() {
			if(!string.IsNullOrEmpty(Settings.PopupElementID))
				return Settings.PopupElementID;
			if(RenderHelper.Rows.Count == 0)
				return TreeList.UniqueID;
			string nodeKey = RenderHelper.IsNewNodeEditing ? RenderHelper.NewNodeParentKey : RenderHelper.EditingNodeKey;
			int rowIndex = GetRowIndex(TreeList.FindNodeByKeyValue(nodeKey));
			return RenderHelper.GetDataRowId(rowIndex);
		}
		int GetRowIndex(TreeListNode node) {
			while(node != null && node.Key != TreeListRenderHelper.RootNodeKey) {
				if(TreeListRows.ContainsKey(node.Key))
					return TreeListRows[node.Key];
				node = node.ParentNode;
			}
			return 0;
		}
		Dictionary<string, int> treeListRows;
		Dictionary<string, int> TreeListRows {
			get {
				if(treeListRows == null)
					treeListRows = CreateTreeListRows();
				return treeListRows;
			}
		}
		Dictionary<string, int> CreateTreeListRows() {
			Dictionary<string, int> rows = new Dictionary<string, int>();
			for(int i = 0; i < RenderHelper.Rows.Count; i++)
				rows[RenderHelper.Rows[i].NodeKey] = i;
			return rows;
		}
	}
	public class TreeListScrollableControl : TreeListInternalWebControl {
		const string 
			HeaderScrollDivContainerClassName = TreeListStyles.Prefix + "HSDC",
			ContentScrollDivClassName = TreeListStyles.Prefix + "CSD",
			FooterScrollDivContainerClassName = TreeListStyles.Prefix + "FSDC";
		TreeListDataTable dataTable;
		WebControl headerScrollDiv, contentScrollDiv, footerScrollDiv, headerDivContainer, footerDivContainer;
		public TreeListScrollableControl(TreeListRenderHelper helper, TreeListDataTable dataTable)
			: base(helper) {
			this.dataTable = dataTable;
			ID = TreeListRenderHelper.ScrollableControlID;
		}
		protected TreeListDataTable DataTable { get { return dataTable; } }
		protected WebControl HeaderScrollDiv { get { return headerScrollDiv; } }
		protected WebControl ContentScrollDiv { get { return contentScrollDiv; } }
		protected WebControl FooterScrollDiv { get { return footerScrollDiv; } }
		protected WebControl HeaderDivContainer { get { return headerDivContainer; } }
		protected WebControl FooterDivContainer { get { return footerDivContainer; } }
		protected override void CreateControlHierarchy() {
			CreateHeader();
			CreateContent();
			CreateFooter();
		}
		protected virtual void CreateHeader() {
			if(!RenderHelper.HasVerticalScrollBar || !RenderHelper.IsHeaderRowVisible) 
				return;
			this.headerDivContainer = RenderUtils.CreateDiv();
			Controls.Add(HeaderDivContainer);
			this.headerScrollDiv = RenderUtils.CreateDiv();
			HeaderDivContainer.Controls.Add(HeaderScrollDiv);
			TreeListDataTable table = new TreeListDataTable(RenderHelper, TreeListDataTableRenderPart.Header);
			HeaderScrollDiv.Controls.Add(table);
		}
		protected virtual void CreateContent() {
			this.contentScrollDiv = RenderUtils.CreateDiv();
			Controls.Add(ContentScrollDiv);
			ContentScrollDiv.Controls.Add(DataTable);
		}
		protected virtual void CreateFooter() {
			if(!RenderHelper.HasVerticalScrollBar || !RenderHelper.IsTotalFooterVisible)
				return;
			this.footerDivContainer = RenderUtils.CreateDiv();
			Controls.Add(FooterDivContainer);
			this.footerScrollDiv = RenderUtils.CreateDiv();
			FooterDivContainer.Controls.Add(FooterScrollDiv);
			TreeListDataTable table = new TreeListDataTable(RenderHelper, TreeListDataTableRenderPart.Footer);
			FooterScrollDiv.Controls.Add(table);
		}
		protected override void PrepareControlHierarchy() {
			PrepareHeader();
			PrepareContent();
			PrepareFooter();
		}
		protected virtual void PrepareHeader() {
			if(HeaderScrollDiv == null)
				return;
			PrepareScrollDiv(HeaderScrollDiv);
			HeaderScrollDiv.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			HeaderDivContainer.CssClass = HeaderScrollDivContainerClassName;
		}
		protected virtual void PrepareContent() {
			PrepareScrollDiv(ContentScrollDiv);
			RenderUtils.SetScrollBars(ContentScrollDiv, RenderHelper.HorizontalScrollBarMode, RenderHelper.VerticalScrollBarMode, DesignMode);
			if(RenderHelper.HasVerticalScrollBar)
				ContentScrollDiv.Height = RenderHelper.TreeList.Settings.ScrollableHeight;
			if(DesignMode && RenderHelper.HorizontalScrollBarMode != ScrollBarMode.Hidden)
				RenderUtils.SetStyleStringAttribute(ContentScrollDiv, "float", "left");
		}
		protected virtual void PrepareFooter() {
			if(FooterScrollDiv == null)
				return;
			PrepareScrollDiv(FooterScrollDiv);
			FooterScrollDiv.Style[HtmlTextWriterStyle.Overflow] = "hidden";
			FooterDivContainer.CssClass = FooterScrollDivContainerClassName;
		}
		protected void PrepareScrollDiv(WebControl div) {
			if(RenderHelper.IsRightToLeft)
				div.Attributes["dir"] = "ltr";
			Unit controlWidth = RenderHelper.TreeList.Width;
			if(RenderHelper.HasHorizontalScrollBar)
				div.Width = DesignMode ? controlWidth : 1;
			bool pagerShowed = RenderHelper.TreeList.SettingsPager.Visible && RenderHelper.TreeList.SettingsPager.Mode == TreeListPagerMode.ShowPager;
			if(DesignMode && !RenderHelper.HasHorizontalScrollBar && !pagerShowed)
				div.Width = !controlWidth.IsEmpty ? controlWidth : 200;
			div.CssClass = ContentScrollDivClassName;
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class TreeListEditorRegistrator : Control, INamingContainer {
		protected override void Render(HtmlTextWriter writer) {
		}
	}
}
