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
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Utils;
namespace DevExpress.Web.Internal {
	public class TitleIndexColumnInfo {
		public TableCell ColumnCell = null;
		public TableCell LeftPaddingCell = null;
		public TableCell RightPaddingCell = null;
		public TableCell SeparatorCell = null;
		public Table ContentColumnTable = null;
	}
	public class TitleIndexCategoryInfo {
		public TableCell SpacingCell = null;
		public TICNodeContentControl HeaderControl = null;
		public TableCell HeaderCell = null;
		public List<TitleIndexRowInfo> Rows = null;
		public TableCell ChildNodePaddingTopCell = null;
		public TableCell ChildNodePaddingBottomCell = null;
		public TableCell BackToTopCell = null;
		public BackToTopControl BackToTopControl = null;
	}
	public class TitleIndexRowInfo {
		public List<TitleIndexColumnInfo> ColumnsContentInfo = null;
		public TableCell SpacingCell = null;
	}
	public class TICControlBase : ASPxInternalWebControl {
		private ASPxTitleIndex fTitleIndexControl = null;
		public ASPxTitleIndex TitleIndexControl {
			get { return fTitleIndexControl; }
		}
		public TICControlBase(ASPxTitleIndex titleIndexControl)
			: base() {
			fTitleIndexControl = titleIndexControl;
		}
		protected bool IsRightToLeft {
			get { return (TitleIndexControl as ISkinOwner).IsRightToLeft(); }
		}
	}
	public class TICFootControlBase : TICControlBase {
		private TableCell fContentCell = null;
		private TableCell fIndexPanelSpacicngCell = null;
		private TableCell fIndexPanelSeparatorCell = null;
		private Table fTable = null;
		public TICFootControlBase(ASPxTitleIndex titleIndexControl)
			: base(titleIndexControl) {
		}
		protected override void ClearControlFields() {
			fContentCell = null;
			fIndexPanelSeparatorCell = null;
			fIndexPanelSpacicngCell = null;
			fTable = null;
		}
		protected override void CreateControlHierarchy() {
			fTable = RenderUtils.CreateTable(true);
			Controls.Add(fTable);
			AddRowsToMainTable(fTable);
		}
		protected override void PrepareControlHierarchy() {
			fTable.Width = Unit.Percentage(100);
			if (fContentCell != null) {
				IndexPanelStyle indexPanelStyle = TitleIndexControl.GetIndexPanelStyle();
				indexPanelStyle.AssignToControl(fContentCell, AttributesRange.Common | AttributesRange.Font);
				TitleIndexControl.GetIndexPanelPadding().AssignToControl(fContentCell);
				RenderUtils.SetHorizontalAlignClass(fContentCell, indexPanelStyle.HorizontalAlign);
				RenderUtils.SetLineHeight(fContentCell, TitleIndexControl.GetIndexPanelLineSpacing());
				RenderUtils.AppendDefaultDXClassName(fContentCell, TitleIndexStyles.IndexPanelSystemStyleName);
			}
			PrepareIndexPanelSpacing();
			PrepareIndexPanelSeparator();
		}
		protected virtual void AddRowsToMainTable(Table table) {
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			fContentCell = RenderUtils.CreateTableCell();
			row.Cells.Add(fContentCell);
			List<string[]> charSetInLines = TitleIndexControl.GetIndexPanelCharSetInLines();
			if (charSetInLines.Count != 0)
				fContentCell.Controls.Add(new AlphabetPager(TitleIndexControl, charSetInLines));
		}
		protected void AddIndexPanelSpacing(Table table) {
			if (!TitleIndexControl.GetIndexPanelSpacing().IsEmpty && (TitleIndexControl.GetIndexPanelSpacing().Value != 0)) {
				TableRow row = RenderUtils.CreateTableRow();
				table.Rows.Add(row);
				fIndexPanelSpacicngCell = RenderUtils.CreateIndentCell();
				row.Cells.Add(fIndexPanelSpacicngCell);
			}
		}
		protected void AddIndexPanelSeparator(Table table) {
			if (!TitleIndexControl.GetIndexPanelSeparatorHeight().IsEmpty &&
				(TitleIndexControl.GetIndexPanelSeparatorHeight().Value != 0)) {
				TableRow row = RenderUtils.CreateTableRow();
				table.Rows.Add(row);
				fIndexPanelSeparatorCell = RenderUtils.CreateIndentCell();
				row.Cells.Add(fIndexPanelSeparatorCell);
			}
		}
		private void PrepareIndexPanelSeparator() {
			if (fIndexPanelSeparatorCell != null) {
				RenderUtils.PrepareIndentCell(fIndexPanelSeparatorCell, Unit.Pixel(1),
					TitleIndexControl.GetIndexPanelSeparatorHeight());
				if (TitleIndexControl.GetControlStyle().Width.IsEmpty)
					fIndexPanelSeparatorCell.Width = Unit.Percentage(100); 
				TitleIndexControl.GetIndexPanelSeparatorStyle().AssignToControl(fIndexPanelSeparatorCell, AttributesRange.Cell | AttributesRange.Common);
			}
		}
		private void PrepareIndexPanelSpacing() {
			if (fIndexPanelSpacicngCell != null) {
				RenderUtils.PrepareIndentCell(fIndexPanelSpacicngCell, Unit.Pixel(1),
					TitleIndexControl.GetIndexPanelSpacing());
				if (TitleIndexControl.GetControlStyle().Width.IsEmpty)
					fIndexPanelSpacicngCell.Width = Unit.Percentage(100); 
			}
		}
	}
	public class TICFootControl : TICFootControlBase {
		public TICFootControl(ASPxTitleIndex titleIndexControl)
			: base(titleIndexControl) {
		}
		protected override void AddRowsToMainTable(Table table) {
			AddIndexPanelSpacing(table);
			AddIndexPanelSeparator(table);
			base.AddRowsToMainTable(table);
		}
	}
	public class TICHeadControl : TICFootControlBase {
		private TableCell fFilterBoxCell = null;
		private TableCell fFilterBoxContentCell = null;
		private Label fFilterBoxCaptionControl = null;
		private TableCell fFilterBoxCaptionCell = null;
		private TableCell fFilterBoxEditorCell = null;
		private WebControl fFilterEditorControl = null;
		private TableCell fFilterBoxSpacingCell = null;
		private TableCell fInfoTextCell = null;
		private LiteralControl fInfoTextControl = null;
		public TICHeadControl(ASPxTitleIndex titleIndexControl)
			: base(titleIndexControl) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fFilterBoxCell = null;
			fFilterBoxContentCell = null;
			fFilterEditorControl = null;
			fFilterBoxCaptionControl = null;
			fFilterBoxCaptionCell = null;
			fFilterBoxEditorCell = null;
			fFilterEditorControl = null;
			fFilterBoxSpacingCell = null;
			fInfoTextCell = null;
			fInfoTextControl = null;
		}
		protected override void AddRowsToMainTable(Table table) {
			if ((TitleIndexControl.IsClientFiltering()) &&
				(TitleIndexControl.FilterBox.VerticalPosition ==
				FilterBoxVerticalPosition.AboveIndexPanel)) {
				AddFilterBox(table);
				AddFilterBoxSpacing(table);
			}
			if (TitleIndexControl.IsShowHeadIndexPanel()) {
				base.AddRowsToMainTable(table);
				AddIndexPanelSeparator(table);
				AddIndexPanelSpacing(table);
			}
			if ((TitleIndexControl.IsClientFiltering()) &&
				(TitleIndexControl.FilterBox.VerticalPosition ==
				FilterBoxVerticalPosition.BelowIndexPanel)) {
				AddFilterBox(table);
				AddFilterBoxSpacing(table);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareFilterBox();
		}
		private void AddFilterBox(Table table) {
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			fFilterBoxCell = RenderUtils.CreateTableCell();
			row.Cells.Add(fFilterBoxCell);
			Table filterBoxTable = RenderUtils.CreateTable();
			fFilterBoxCell.Controls.Add(filterBoxTable);
			TableRow paddingRow = RenderUtils.CreateTableRow();
			filterBoxTable.Rows.Add(paddingRow);
			fFilterBoxContentCell = RenderUtils.CreateTableCell();
			paddingRow.Cells.Add(fFilterBoxContentCell);
			Table filterTable = RenderUtils.CreateTable();
			fFilterBoxContentCell.Controls.Add(filterTable);
			TableRow filterRow = RenderUtils.CreateTableRow();
			filterTable.Rows.Add(filterRow);
			if (TitleIndexControl.FilterBox.Caption != "") {
				fFilterBoxCaptionCell = RenderUtils.CreateTableCell();
				filterRow.Cells.Add(fFilterBoxCaptionCell);
				fFilterBoxCaptionControl = RenderUtils.CreateLabel();
				fFilterBoxCaptionCell.Controls.Add(fFilterBoxCaptionControl);
			}
			fFilterBoxEditorCell = RenderUtils.CreateTableCell();
			filterRow.Cells.Add(fFilterBoxEditorCell);
			fFilterEditorControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			fFilterBoxEditorCell.Controls.Add(fFilterEditorControl);
			if (TitleIndexControl.IsShowInfoTextInFilterBox()) {
				TableRow infoTextRow = RenderUtils.CreateTableRow();
				filterTable.Rows.Add(infoTextRow);
				infoTextRow.Cells.Add(RenderUtils.CreateTableCell());
				fInfoTextCell = RenderUtils.CreateTableCell();
				infoTextRow.Cells.Add(fInfoTextCell);
				fInfoTextControl = RenderUtils.CreateLiteralControl();
				fInfoTextCell.Controls.Add(fInfoTextControl);
			}
		}
		private void AddFilterBoxSpacing(Table table) {
			if (!TitleIndexControl.GetFilterBoxSpacing().IsEmpty && (TitleIndexControl.GetFilterBoxSpacing().Value != 0)) {
				TableRow row = RenderUtils.CreateTableRow();
				table.Rows.Add(row);
				fFilterBoxSpacingCell = RenderUtils.CreateIndentCell();
				row.Cells.Add(fFilterBoxSpacingCell);
			}
		}
		private void PrepareFilterBox() {
			if (fFilterBoxCell != null) {
				AppearanceStyle filterBoxStyle = TitleIndexControl.GetFilterBoxStyle();
				filterBoxStyle.AssignToControl(fFilterBoxCell, AttributesRange.All);
				filterBoxStyle.AssignToControl(fFilterBoxContentCell, AttributesRange.Font);
				TitleIndexControl.GetFilterBoxPaddings().AssignToControl(fFilterBoxCell);
				TitleIndexControl.GetFilterBoxEditStyle().AssignToControl(fFilterEditorControl, AttributesRange.All);
				TitleIndexControl.GetFilterBoxEditPaddings().AssignToControl(fFilterEditorControl);
				fFilterEditorControl.Height = TitleIndexControl.GetFilterBoxEditorHeight();
				fFilterEditorControl.Width = TitleIndexControl.GetFilterBoxEditorWidth();
				fFilterBoxEditorCell.VerticalAlign = filterBoxStyle.VerticalAlign;
				fFilterBoxEditorCell.HorizontalAlign = IsRightToLeft ? HorizontalAlign.Right : HorizontalAlign.Left;
				if (fFilterBoxCaptionCell != null) {
					fFilterBoxCaptionCell.VerticalAlign = filterBoxStyle.VerticalAlign;
					filterBoxStyle.AssignToControl(fFilterBoxCaptionCell, AttributesRange.Font);
					fFilterBoxCaptionControl.Text = TitleIndexControl.FilterBox.Caption;
					if(TitleIndexControl.IsEnabled())
						fFilterBoxCaptionControl.AssociatedControlID = TitleIndexControl.GetFilterInputID();
				}
				if (TitleIndexControl.FilterBox.HorizontalPosition == FilterBoxPosition.Center)
					fFilterBoxCell.HorizontalAlign = HorizontalAlign.Center;
				if (TitleIndexControl.FilterBox.HorizontalPosition == FilterBoxPosition.Left)
					fFilterBoxCell.HorizontalAlign = HorizontalAlign.Left;
				if (TitleIndexControl.FilterBox.HorizontalPosition == FilterBoxPosition.Right)
					fFilterBoxCell.HorizontalAlign = HorizontalAlign.Right;
				fFilterEditorControl.Enabled = TitleIndexControl.IsEnabled();
				if(!DesignMode && TitleIndexControl.IsEnabled()) {
					fFilterEditorControl.ID = TitleIndexControl.GetFilterInputID();					
					RenderUtils.SetStringAttribute(fFilterEditorControl, "onblur", TitleIndexControl.GetFilterInputOnBlur());
					RenderUtils.SetStringAttribute(fFilterEditorControl, "onchange", TitleIndexControl.GetFilterInputOnChange());
					RenderUtils.SetStringAttribute(fFilterEditorControl, "onfocus", TitleIndexControl.GetFilterInputOnFocus());
					RenderUtils.SetStringAttribute(fFilterEditorControl, "onkeypress", TitleIndexControl.GetFilterInputOnKeyPress());
					RenderUtils.SetStringAttribute(fFilterEditorControl, "onkeyup", TitleIndexControl.GetFilterInputOnKeyUp());
					RenderUtils.AppendDefaultDXClassName(fFilterEditorControl, TitleIndexStyles.FilterBoxEditSystemStyleName);
				}
				if (fInfoTextControl != null) {
					fInfoTextControl.Text = HtmlConvertor.ToMultilineHtml(TitleIndexControl.FilterBox.InfoText);
					TitleIndexControl.GetFilterBoxInfoTextStyle().AssignToControl(fInfoTextCell, AttributesRange.Font | AttributesRange.Common);
					TitleIndexControl.GetFilterBoxHintPaddings().AssignToControl(fInfoTextCell);
					fInfoTextCell.HorizontalAlign = IsRightToLeft ? HorizontalAlign.Right : HorizontalAlign.Left;
				}
				if (fFilterBoxSpacingCell != null) {
					RenderUtils.PrepareIndentCell(fFilterBoxSpacingCell, Unit.Pixel(1), TitleIndexControl.GetFilterBoxSpacing());
					if (TitleIndexControl.GetControlStyle().Width.IsEmpty)
						fFilterBoxSpacingCell.Width = Unit.Percentage(100); 
				}
			}
		}
	}
	public class TICMainControl : TICControlBase {
		private TableCell fContentCell = null; 
		private Table fContentTable = null;
		private LiteralControl fEmptyFilteringResultTextControl = null;
		private TableCell fFooterCell = null;
		private TableCell fHeaderCell = null;
		private TICControlBase fTICFooterControl = null;
		private TICControlBase fTICHeaderControl = null;
		private WebControl fEmptyFilteringResultDiv = null;
		public TICMainControl(ASPxTitleIndex titleIndexControl)
			: base(titleIndexControl) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fContentCell = null;
			fContentTable = null;
			fEmptyFilteringResultTextControl = null;
			fEmptyFilteringResultDiv = null;
			fHeaderCell = null;
			fFooterCell = null;
			fTICHeaderControl = null;
			fTICFooterControl = null;
		}
		protected override void CreateControlHierarchy() {
			fContentTable = RenderUtils.CreateTable();
			Controls.Add(fContentTable);
			fHeaderCell = CreateHeaderCell(fContentTable);
			fContentCell = CreateContentCell(fContentTable);
			fFooterCell = CreateFooterCell(fContentTable);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if (fHeaderCell != null) {
				fHeaderCell.ID = TitleIndexControl.GetHeadIndexPanelCellID();
				fHeaderCell.VerticalAlign = VerticalAlign.Top;
				fHeaderCell.Width = Unit.Percentage(100);
			}
			if (fFooterCell != null) {
				fFooterCell.ID = TitleIndexControl.GetFootIndexPanelCellID();
				fFooterCell.VerticalAlign = VerticalAlign.Top;
				fFooterCell.Width = Unit.Percentage(100);
			}
			if (fContentTable != null)
				fContentTable.Width = Unit.Percentage(100);
			if (fContentCell != null) {
				fContentCell.ID = TitleIndexControl.GetTreeViewCellID();
				fContentCell.VerticalAlign = VerticalAlign.Top;
			}
			PrepareEmptyFilteringResultCaptionControl();
		}
		private TableCell CreateContentCell(Table mainTable) {
			TableRow row = RenderUtils.CreateTableRow();
			mainTable.Rows.Add(row);
			TableCell cell = RenderUtils.CreateTableCell();
			row.Cells.Add(cell);
			if (TitleIndexControl.Categorized)
				cell.Controls.Add(new TICMultiRowTreeViewControl(TitleIndexControl, TitleIndexControl.SortedNodes));
			else
				cell.Controls.Add(new TICTreeViewControl(TitleIndexControl, TitleIndexControl.SortedNodes));
			if (!DesignMode)
				CreateEmptyFilteringResultCaptionControl(cell);
			return cell;
		}
		private void CreateEmptyFilteringResultCaptionControl(TableCell cell) {
			fEmptyFilteringResultDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			cell.Controls.Add(fEmptyFilteringResultDiv);
			fEmptyFilteringResultDiv.Width = Unit.Percentage(100);
			fEmptyFilteringResultDiv.ID = TitleIndexControl.GetEmptyFilterResultDivID();
			RenderUtils.SetStyleStringAttribute(fEmptyFilteringResultDiv, "text-align",
				HorizontalAlign.Center.ToString().ToLower());
			RenderUtils.SetWrap(fEmptyFilteringResultDiv, DefaultBoolean.False);
			fEmptyFilteringResultTextControl = RenderUtils.CreateLiteralControl();
			fEmptyFilteringResultDiv.Controls.Add(fEmptyFilteringResultTextControl);
			TitleIndexControl.GetEmptyFilteringResultCaptionPadding().AssignToControl(fEmptyFilteringResultDiv);
			RenderUtils.SetVisibility(fEmptyFilteringResultDiv, false, !DesignMode);
		}
		private TableCell CreateHeaderCell(Table mainTable) {
			TableCell cell = null;
			if (TitleIndexControl.IsShowHeadIndexPanel() ||
				TitleIndexControl.IsClientFiltering()) {
				TableRow row = RenderUtils.CreateTableRow();
				mainTable.Rows.Add(row);
				cell = RenderUtils.CreateTableCell();
				row.Cells.Add(cell);
				fTICHeaderControl = new TICHeadControl(TitleIndexControl);
				cell.Controls.Add(fTICHeaderControl);
			}
			return cell;
		}
		private TableCell CreateFooterCell(Table mainTable) {
			TableCell cell = null;
			if (TitleIndexControl.IsShowFootIndexPanel()) {
				TableRow row = RenderUtils.CreateTableRow();
				mainTable.Rows.Add(row);
				cell = RenderUtils.CreateTableCell();
				row.Cells.Add(cell);
				fTICFooterControl = new TICFootControl(TitleIndexControl);
				cell.Controls.Add(fTICFooterControl);
			}
			return cell;
		}
		private void PrepareEmptyFilteringResultCaptionControl() {
			if (fEmptyFilteringResultTextControl != null) {
				fEmptyFilteringResultTextControl.Text =
					TitleIndexControl.NoDataText;
				TitleIndexControl.GetControlStyle().AssignToControl(fEmptyFilteringResultDiv, AttributesRange.Font);
			}
		}
	}
	public  class MainControl : TICControlBase {
		private Table fMainTable = null;
		private TableCell fMainCell = null;
		public MainControl(ASPxTitleIndex titleIndexControlBase)
			: base(titleIndexControlBase) {
		}
		protected override void ClearControlFields() {
			fMainTable = null;
			fMainCell = null;
		}
		protected override void CreateControlHierarchy() {
			if (TitleIndexControl.IsShowBackToTop())
				Controls.Add(RenderUtils.CreateAnchor(TitleIndexControl.GetIndexPanelName()));
			fMainTable = RenderUtils.CreateTable();
			Controls.Add(fMainTable);
			CreateContent(fMainTable);
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyleBase style = TitleIndexControl.GetControlStyle();
			RenderUtils.AssignAttributes(TitleIndexControl, fMainTable);
			RenderUtils.SetVisibility(fMainTable, TitleIndexControl.IsClientVisible(), true);
			style.AssignToControl(fMainTable, AttributesRange.Common | AttributesRange.Font);
			RenderUtils.AppendDefaultDXClassName(fMainTable, TitleIndexStyles.ControlSystemStyleName);
			fMainCell.VerticalAlign = VerticalAlign.Top;
			fMainCell.ID = TitleIndexControl.GetContentCellID();
			style.AssignToControl(fMainCell, AttributesRange.Cell);
			RenderUtils.SetPaddings(fMainCell, TitleIndexControl.GetPaddings());
			if(TitleIndexControl.HasItemOnClick())
				RenderUtils.SetStringAttribute(fMainCell, "onclick", TitleIndexControl.GetControlOnClick());
		}
		private void CreateContent(Table mainTable) {
			TableRow row = RenderUtils.CreateTableRow();
			mainTable.Rows.Add(row);
			fMainCell = RenderUtils.CreateTableCell();
			row.Cells.Add(fMainCell);
			TICMainControl contentControl = new TICMainControl(TitleIndexControl);
			contentControl.ID = TitleIndexControl.GetContentControlID();
			fMainCell.Controls.Add(contentControl);
		}
	}
	public class TICTreeViewControl : TICControlBase {
		private int[] fPartsInColumn = null;
		private List<TitleIndexColumnInfo> fColumnsContentInfo = null;
		private Dictionary<TitleIndexNode, int> fNodeColumnIndexDictonary = null;
		private TitleIndexNodeCollection fRootNodes = null;
		protected Table fTable = null;
		protected TitleIndexNodeCollection RootNodes {
			get { return fRootNodes; }
		}
		public TICTreeViewControl(ASPxTitleIndex titleIndexControl, TitleIndexNodeCollection rootNodes)
			: base(titleIndexControl) {
			fRootNodes = rootNodes;
		}
		protected TICTreeViewNodeControlBase CreateNodeControl(TitleIndexNode node) {
			if (TitleIndexControl.GetNodeLevel(node) == 0)
				return new TICTreeViewNodeControl(this, node);
			else
				return new TICTreeViewNodeControlBase(this, node);
		}
		protected TICTreeViewNodeControlBase CreateWrapNodeControl(TitleIndexNode node) {
			if (TitleIndexControl.GetNodeLevel(node) == 0)
				return new TICTreeViewWrapNodeControl(this, node);
			else
				return new TICTreeViewWrapNodeControlBase(this, node);
		}
		protected override void ClearControlFields() {
			fPartsInColumn = null;
			fColumnsContentInfo = null;
			fNodeColumnIndexDictonary = null;
			fTable = null;
		}
		protected override void CreateControlHierarchy() {
			fTable = RenderUtils.CreateTable(true);
			Controls.Add(fTable);
			fNodeColumnIndexDictonary = new Dictionary<TitleIndexNode, int>();
			CreateRows(fTable);
		}
		protected override void PrepareControlHierarchy() {
			PrepareMainTable();
			PrepareColumns(fColumnsContentInfo, 1);
		}
		protected virtual void CreateRows(Table table) {
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			fColumnsContentInfo = CreateColumns(row, RootNodes);
		}
		protected virtual TitleIndexNode GetCategoryNode(TitleIndexNode node) {
			TitleIndexNode categoryNode = node;
			while (!TitleIndexControl.IsRootNode(categoryNode))
				categoryNode = TitleIndexControl.GetParentNode(categoryNode);
			return categoryNode;
		}
		protected virtual int GetColumnCount(int categoryNodeCount) {
			return TitleIndexControl.ColumnCountActual;
		}
		protected internal virtual bool IsFirstInColumn(TitleIndexNode node) {
			TitleIndexNode curRootColumnNode = node;
			if (TitleIndexControl.IsRootNode(curRootColumnNode)) {
				TitleIndexNode previousSibling = TitleIndexControl.GetPreviousSibling(node);
				return (previousSibling == null) ||
					(GetColumnIndex(node) != GetColumnIndex(previousSibling));
			}
			else
				return false;
		}
		protected void AddColumnSeparator(TitleIndexColumnInfo columnInfo, TableRow row, int index) {
			columnInfo.LeftPaddingCell = RenderUtils.CreateIndentCell();
			row.Cells.Add(columnInfo.LeftPaddingCell);
			ITemplate template = TitleIndexControl.ColumnSeparatorTemplate;
			if (template != null) {
				TitleIndexColumnSeparatorTemplateContainer container = new TitleIndexColumnSeparatorTemplateContainer(index, TitleIndexControl.Columns[index]);
				columnInfo.SeparatorCell = RenderUtils.CreateTableCell();
				template.InstantiateIn(container);
				container.AddToHierarchy(columnInfo.SeparatorCell, null);
				row.Cells.Add(columnInfo.SeparatorCell);
			}
			else {
				columnInfo.SeparatorCell = RenderUtils.CreateIndentCell();
				row.Cells.Add(columnInfo.SeparatorCell);
			}
			columnInfo.RightPaddingCell = RenderUtils.CreateIndentCell();
			row.Cells.Add(columnInfo.RightPaddingCell);
		}
		protected List<TitleIndexColumnInfo> CreateColumns(TableRow parentRow, TitleIndexNodeCollection categoryNodes) {
			List<TitleIndexColumnInfo> columnsContentInfo = new List<TitleIndexColumnInfo>();
			List<List<TitleIndexNode>> columnsNodeList = new List<List<TitleIndexNode>>();
			int columnCount = GetColumnCount(categoryNodes.Count);
			for (int i = 0; i < columnCount; i++)
				columnsNodeList.Add(new List<TitleIndexNode>());
			if (columnCount == 1) {
				foreach (TitleIndexNode node in categoryNodes)
					columnsNodeList[0].Add(node);
			}
			else {
				int[] partHeights = GetPartHeightsByNode(categoryNodes);
				fPartsInColumn = CommonUtils.ArrangeParts(partHeights, columnCount);
				int nodeIndex = 0;
				for (int i = 0; i < columnCount; i++) {
					for (int j = 0; j < fPartsInColumn[i]; j++) {
						columnsNodeList[i].Add(categoryNodes[nodeIndex]);
						nodeIndex++;
					}
				}
			}
			for (int i = 0; i < columnsNodeList.Count; i++) {
				foreach (TitleIndexNode node in columnsNodeList[i])
					fNodeColumnIndexDictonary.Add(node, i);
			}
			for (int i = 0; i < columnCount; i++) {
				TitleIndexColumnInfo columnInfo = new TitleIndexColumnInfo();
				columnsContentInfo.Add(columnInfo);
				columnInfo.ColumnCell = RenderUtils.CreateTableCell();
				parentRow.Cells.Add(columnInfo.ColumnCell);
				columnInfo.ContentColumnTable = RenderUtils.CreateTable(true);
				columnInfo.ColumnCell.Controls.Add(columnInfo.ContentColumnTable);
				TableRow row = RenderUtils.CreateTableRow();
				columnInfo.ContentColumnTable.Rows.Add(row);
				TableCell cell = RenderUtils.CreateTableCell();
				row.Cells.Add(cell);
				foreach (TitleIndexNode node in columnsNodeList[i])
					cell.Controls.Add(CreateNode(node));
				if (i != columnCount - 1)
					AddColumnSeparator(columnInfo, parentRow, columnsContentInfo.Count);
			}
			return columnsContentInfo;
		}
		protected void PrepareColumns(List<TitleIndexColumnInfo> columnsContentInfo, int rowIndex) {
			Unit columnSeparatorWidth = TitleIndexControl.GetColumnSeparatorWidth();
			AppearanceStyleBase columnSeparatorStyle = TitleIndexControl.GetColumnSeparatorStyle();
			Paddings columnSeparatorPaddings = TitleIndexControl.GetColumnSeparatorPaddings();
			foreach(TitleIndexColumnInfo columnInfo in columnsContentInfo) {
				int columnIndex = columnsContentInfo.IndexOf(columnInfo);
				ColumnStyle columnStyle = TitleIndexControl.GetColumnStyle(columnIndex);
				Unit columnWidth = TitleIndexControl.GetColumnWidth(columnIndex);
				columnInfo.ColumnCell.Width = columnWidth;
				if ((columnWidth.Type == UnitType.Percentage) || (columnWidth.IsEmpty))
					columnInfo.ContentColumnTable.Width = new Unit(100, UnitType.Percentage);
				else
					columnInfo.ContentColumnTable.Width = columnWidth;
				columnStyle.AssignToControl(columnInfo.ColumnCell, AttributesRange.Common);
				RenderUtils.SetPaddings(columnInfo.ColumnCell, TitleIndexControl.GetColumnPaddings(columnIndex));
				columnInfo.ColumnCell.ID = TitleIndexControl.GetColumnID(columnIndex, rowIndex);
				columnInfo.ColumnCell.VerticalAlign = VerticalAlign.Top;
				if (columnInfo.SeparatorCell != null) {
					if (columnSeparatorWidth != 0) {
						columnSeparatorStyle.AssignToControl(columnInfo.SeparatorCell, AttributesRange.All);
						RenderUtils.PrepareIndentCell(columnInfo.SeparatorCell, columnSeparatorWidth, Unit.Empty);
						Unit paddingLeft = columnSeparatorPaddings.Padding;
						if (!columnSeparatorPaddings.PaddingLeft.IsEmpty)
							paddingLeft = columnSeparatorPaddings.PaddingLeft;
						if (!paddingLeft.IsEmpty)
							RenderUtils.PrepareIndentCell(columnInfo.LeftPaddingCell, paddingLeft, Unit.Empty);
						else
							columnInfo.LeftPaddingCell.Visible = false;
						Unit paddingRight = columnSeparatorPaddings.Padding;
						if (!columnSeparatorPaddings.PaddingRight.IsEmpty)
							paddingRight = columnSeparatorPaddings.PaddingRight;
						if (!paddingRight.IsEmpty)
							RenderUtils.PrepareIndentCell(columnInfo.RightPaddingCell, paddingRight, Unit.Empty);
						else
							columnInfo.RightPaddingCell.Visible = false;
					}
					else {
						columnInfo.SeparatorCell.Visible = false;
						columnInfo.LeftPaddingCell.Visible = false;
						columnInfo.RightPaddingCell.Visible = false;
					}
				}
			}
		}
		protected void PrepareMainTable() {
			TitleIndexControl.GetControlStyle().AssignToControl(fTable, AttributesRange.Font);
			fTable.ID = TitleIndexControl.GetContentTableID();
			if ((GetColumnCount(RootNodes.Count) <= 1) ||
				(TitleIndexControl.GetControlStyle().Width.Type == UnitType.Percentage))
				fTable.Width = Unit.Percentage(100);
			if (TitleIndexControl.GetControlStyle().Width.IsEmpty) {
				if(Browser.IsOpera && Browser.MajorVersion == 8)
					fTable.Width = 0;
				else if(Browser.IsOpera || Browser.Family.IsWebKit)
					fTable.Width = Unit.Percentage(100);
			}
		}
		protected internal int[] GetPartHeightsByNode(TitleIndexNodeCollection rootNodes) {
			int[] ret = new int[rootNodes.Count];
			int nodeIndex = 0;
			foreach (TitleIndexNode node in rootNodes) {
				ret[nodeIndex] = GetNodeCount(node) + 1;
				nodeIndex++;
			}
			return ret;
		}
		protected int GetColumnIndex(TitleIndexNode node) {
			return fNodeColumnIndexDictonary[node];
		}
		protected int GetNodeCount(TitleIndexNode node) {
			int ret = 0;
			if (TitleIndexControl.HasChildNodes(node)) {
				ret = node.ChildNodes.Count;
				foreach (TitleIndexNode childNode in node.ChildNodes)
					ret = ret + GetNodeCount(childNode);
			}
			else
				ret = 0;
			return ret;
		}
		protected internal TICTreeViewNodeControlBase CreateNode(TitleIndexNode node) {
			if (TitleIndexControl.NeedTableNodeRender(node))
				return CreateWrapNodeControl(node);
			else
				return CreateNodeControl(node);
		}
	}
	public class TICMultiRowTreeViewControl : TICTreeViewControl {
		private List<TitleIndexCategoryInfo> fCategoriesInfo = null;	   
		public TICMultiRowTreeViewControl(ASPxTitleIndex titleIndexControl, TitleIndexNodeCollection rootNodes)
			: base(titleIndexControl, rootNodes) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fCategoriesInfo = null;
		}
		protected override void CreateRows(Table table) {
			int[][] nodeCountInRows = CommonUtils.ArrangePartInRow(GetPartsCount(),
				TitleIndexControl.ColumnCountActual,
				TitleIndexControl.Categorized, RepeatDirection.Vertical);
			Dictionary<int, List<TitleIndexNodeCollection>> rowsInCategoryNode = new Dictionary<int, List<TitleIndexNodeCollection>>();
			for (int i = 0; i < nodeCountInRows.Length; i++) { 
				List<TitleIndexNodeCollection> rowsNodes = new List<TitleIndexNodeCollection>();
				int nodeIndex = 0;
				for (int j = 0; j < nodeCountInRows[i].Length; j++) { 
					TitleIndexNodeCollection nodes = new TitleIndexNodeCollection(RootNodes[0]); 
					for (int n = 0; n < nodeCountInRows[i][j]; n++) {
						nodes.Add(RootNodes[i].ChildNodes[nodeIndex]);
						nodeIndex++;
					}
					rowsNodes.Add(nodes);
				}
				rowsInCategoryNode.Add(i, rowsNodes);
			}
			fCategoriesInfo = new List<TitleIndexCategoryInfo>();
			foreach (int categoryNodeIndex in rowsInCategoryNode.Keys)
				fCategoriesInfo.Add(CreateCategory(table, categoryNodeIndex, rowsInCategoryNode[categoryNodeIndex],
					categoryNodeIndex == rowsInCategoryNode.Keys.Count - 1));
		}
		protected override TitleIndexNode GetCategoryNode(TitleIndexNode node) {
			TitleIndexNode categoryNode = node;
			if (!TitleIndexControl.IsRootNode(categoryNode)) {
				if (TitleIndexControl.Categorized) {
					while (!TitleIndexControl.IsRootNode(TitleIndexControl.GetParentNode(categoryNode)))
						categoryNode = TitleIndexControl.GetParentNode(categoryNode);
				}
				else
					categoryNode = base.GetCategoryNode(node);
			}
			return categoryNode;
		}
		protected override int GetColumnCount(int categoryNodeCount) {
			int count = TitleIndexControl.ColumnCountActual;
			if (count >= categoryNodeCount)
				count = categoryNodeCount;
			if (count < 1)
				count = 1;
			return count;
		}
		protected internal override bool IsFirstInColumn(TitleIndexNode node) {
			if (TitleIndexControl.Categorized) {
				TitleIndexNode parentNode = TitleIndexControl.GetParentNode(node);
				if ((parentNode != null) && (TitleIndexControl.IsRootNode(parentNode))) {
					TitleIndexNode previousSibling = TitleIndexControl.GetPreviousSibling(node);
					return (previousSibling == null) ||
						(GetColumnIndex(node) != GetColumnIndex(previousSibling));
				}
			}
			return base.IsFirstInColumn(node);
		}
		protected override void PrepareControlHierarchy() {
			PrepareMainTable();
			PrepareCategoryRows();
		}
		protected void AddBackToTop(WebControl parentControl, TitleIndexCategoryInfo info) {
			info.BackToTopControl = new BackToTopControl(TitleIndexControl.GetIndexPanelBookmark(), IsRightToLeft);
			info.BackToTopControl.Text = TitleIndexControl.BackToTopText;
			info.BackToTopControl.BackToTopImage = TitleIndexControl.GetBackToTopImage();
			parentControl.Controls.Add(info.BackToTopControl);
		}
		protected TICNodeContentControl CreateNodeContentControl(TitleIndexNode categoryNode) {
			return new TICNodeContentControl(this, categoryNode);
		}
		protected TitleIndexCategoryInfo CreateCategory(Table table, int categoryNodeIndex,
			List<TitleIndexNodeCollection> nodesInRows, bool isLastCategory) {
			bool categoryHasRows = nodesInRows.Count > 0 && nodesInRows[0].Count > 0;
			TitleIndexCategoryInfo categoryInfo = new TitleIndexCategoryInfo();
			categoryInfo.Rows = new List<TitleIndexRowInfo>();
			if (TitleIndexControl.Categorized) {
				TableRow headerRow = RenderUtils.CreateTableRow();
				table.Rows.Add(headerRow);
				categoryInfo.HeaderCell = RenderUtils.CreateTableCell();
				headerRow.Cells.Add(categoryInfo.HeaderCell);
				categoryInfo.HeaderControl = CreateNodeContentControl(RootNodes[categoryNodeIndex]);
				categoryInfo.HeaderCell.Controls.Add(categoryInfo.HeaderControl);
				bool needCreateHeaderSeparator = false;
				if (categoryHasRows) {
					if (TitleIndexControl.GetCustomGroupContentPadding(0).GetPaddingTop() != 0)
						needCreateHeaderSeparator = true;
				}
				if (needCreateHeaderSeparator) {
					TableRow headerSeparatorRow = RenderUtils.CreateTableRow();
					table.Rows.Add(headerSeparatorRow);
					categoryInfo.ChildNodePaddingTopCell = RenderUtils.CreateIndentCell();
					headerSeparatorRow.Cells.Add(categoryInfo.ChildNodePaddingTopCell);
				}
			}
			for (int i = 0; i < nodesInRows.Count; i++)
				if (nodesInRows[i].Count != 0)
					categoryInfo.Rows.Add(CreateRow(table, nodesInRows[i], null,
						i == nodesInRows.Count - 1));
			if ((TitleIndexControl.Categorized) && (categoryHasRows) &&
					(TitleIndexControl.GetCustomGroupContentPadding(0).GetPaddingBottom() != 0)) {
				TableRow childNodePaddingBottomRow = RenderUtils.CreateTableRow();
				table.Rows.Add(childNodePaddingBottomRow);
				categoryInfo.ChildNodePaddingBottomCell = RenderUtils.CreateIndentCell();
				childNodePaddingBottomRow.Cells.Add(categoryInfo.ChildNodePaddingBottomCell);
			}
			if (TitleIndexControl.IsShowBackToTop()) {
				TableRow backToTopRow = RenderUtils.CreateTableRow();
				table.Rows.Add(backToTopRow);
				categoryInfo.BackToTopCell = RenderUtils.CreateTableCell();
				backToTopRow.Cells.Add(categoryInfo.BackToTopCell);
				AddBackToTop(categoryInfo.BackToTopCell, categoryInfo);
			}
			if ((TitleIndexControl.Categorized) &&
					(TitleIndexControl.GetItemSpacing(0) != 0) && (!isLastCategory)) {
				TableRow spacingRow = RenderUtils.CreateTableRow();
				table.Rows.Add(spacingRow);
				categoryInfo.SpacingCell = RenderUtils.CreateIndentCell();
				spacingRow.Cells.Add(categoryInfo.SpacingCell);
			}
			return categoryInfo;
		}
		protected TitleIndexRowInfo CreateRow(Table table, TitleIndexNodeCollection nodes, TitleIndexNode rootRowNode, bool isLastRow) {
			TitleIndexRowInfo rowInfo = new TitleIndexRowInfo();
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			rowInfo.ColumnsContentInfo = CreateColumns(row, nodes);
			if (!isLastRow && (GetRowSpacing(TitleIndexControl.Categorized) != 0)) {
				TableRow spacingRow = RenderUtils.CreateTableRow();
				table.Rows.Add(spacingRow);
				rowInfo.SpacingCell = RenderUtils.CreateIndentCell();
				spacingRow.Cells.Add(rowInfo.SpacingCell);
			}
			return rowInfo;
		}
		protected void PrepareBackToTop(TitleIndexCategoryInfo info) {
			if (info.BackToTopControl != null) {
				info.BackToTopCell.ColumnSpan = GetMaximumTDCountInTable();
				TitleIndexControl.GetBackToTopStyle().AssignToControl(info.BackToTopCell, AttributesRange.Cell);
				info.BackToTopControl.BackToTopStyle = TitleIndexControl.GetBackToTopStyle();
				info.BackToTopControl.BackToTopImageSpacing = TitleIndexControl.GetBackToTopStyle().ImageSpacing;
				info.BackToTopControl.BackToTopPaddings = TitleIndexControl.GetBackToTopPadding();
				info.BackToTopControl.Enabled = TitleIndexControl.IsEnabled();
				info.BackToTopControl.BackToTopSpacing = TitleIndexControl.GetBackToTopSpacing();
			}
		}
		protected void PrepareCategoryRows() {
			foreach(TitleIndexCategoryInfo info in fCategoriesInfo) {
				if (info.HeaderCell != null) {
					RenderUtils.SetPaddings(info.HeaderCell,
						TitleIndexControl.GetItemPaddings(RootNodes[fCategoriesInfo.IndexOf(info)]));
					info.HeaderCell.ID = TitleIndexControl.GetCategoryHeaderCellID(fCategoriesInfo.IndexOf(info));
					TitleIndexControl.GetItemStyle(RootNodes[fCategoriesInfo.IndexOf(info)]).AssignToControl(info.HeaderCell, AttributesRange.All);
					info.HeaderCell.ColumnSpan = GetMaximumTDCountInTable();
				}
				if (info.ChildNodePaddingTopCell != null)
					RenderUtils.PrepareIndentCell(info.ChildNodePaddingTopCell, 0, TitleIndexControl.GetCustomGroupContentPadding(0).GetPaddingTop());
				if (info.ChildNodePaddingBottomCell != null)
					RenderUtils.PrepareIndentCell(info.ChildNodePaddingBottomCell, 0, TitleIndexControl.GetCustomGroupContentPadding(0).GetPaddingBottom());
				if (info.SpacingCell != null)
					RenderUtils.PrepareIndentCell(info.SpacingCell, 0, TitleIndexControl.GetItemSpacing(0));
				foreach(TitleIndexRowInfo rowInfo in info.Rows) {
					if (rowInfo.ColumnsContentInfo != null)
						PrepareColumns(rowInfo.ColumnsContentInfo, fCategoriesInfo.IndexOf(info));
					if (rowInfo.SpacingCell != null)
						RenderUtils.PrepareIndentCell(rowInfo.SpacingCell, 0, GetRowSpacing(TitleIndexControl.Categorized));
				}
				PrepareBackToTop(info);
			}
		}
		private int GetMaximumTDCountInTable() {
			int max = 1;
			TitleIndexRowInfo rowWithMaxCells = null;
			foreach(TitleIndexCategoryInfo info in fCategoriesInfo) {
				List<TitleIndexRowInfo> rowsInfo = info.Rows;
				foreach(TitleIndexRowInfo rowContentInfo in rowsInfo) {
					if ((rowContentInfo.ColumnsContentInfo != null) &&
						(max < rowContentInfo.ColumnsContentInfo.Count)) {
						max = rowContentInfo.ColumnsContentInfo.Count;
						rowWithMaxCells = rowContentInfo;
					}
				}
			}
			if (rowWithMaxCells != null) {
				if (rowWithMaxCells.ColumnsContentInfo[0].LeftPaddingCell != null)
					max += rowWithMaxCells.ColumnsContentInfo.Count - 1;
				if (rowWithMaxCells.ColumnsContentInfo[0].RightPaddingCell != null)
					max += rowWithMaxCells.ColumnsContentInfo.Count - 1;
				if (rowWithMaxCells.ColumnsContentInfo[0].SeparatorCell != null)
					max += rowWithMaxCells.ColumnsContentInfo.Count - 1;
			}
			return max;
		}
		private int[] GetPartsCount() {
			List<int> ret = new List<int>();
			if (TitleIndexControl.Categorized) {
				for (int i = 0; i < RootNodes.Count; i++)
					if (TitleIndexControl.HasChildNodes(RootNodes[i]))
						ret.Add(RootNodes[i].ChildNodes.Count);
					else
						ret.Add(0);
			}
			else
				ret.Add(RootNodes.Count);
			return ret.ToArray();
		}
		private Unit GetRowSpacing(bool isCategorize) {
			return isCategorize ? TitleIndexControl.GetItemSpacing(1) :
				TitleIndexControl.GetItemSpacing(0);
		}							 
	}
	public class TICNodeControlBase : ASPxInternalWebControl {
		private TitleIndexNode fNode = null;
		private ASPxTitleIndex fTitleIndexControl = null;
		private TICTreeViewControl fTreeViewControl = null;
		private WebControl fSpanTextControl = null;
		public TitleIndexNode Node {
			get { return fNode; }
		}
		public TICTreeViewControl TreeViewControl {
			get { return fTreeViewControl; }
		}
		protected ASPxTitleIndex TitleIndexControl {
			get { return fTitleIndexControl; }
		}
		protected bool IsClientFiltering {
			get { return TitleIndexControl.IsClientFiltering(); }
		}
		protected bool IsRightToLeft {
			get { return (TitleIndexControl as ISkinOwner).IsRightToLeft(); }
		}
		public TICNodeControlBase(TICTreeViewControl treeViewControl, TitleIndexNode node) {
			fTreeViewControl = treeViewControl;
			fTitleIndexControl = treeViewControl.TitleIndexControl;
			fNode = node;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fSpanTextControl = null;
		}
		protected HyperLink CreateNodeTextControl(WebControl parentControl) {
			HyperLink link = RenderUtils.CreateHyperLink();
			int level = TitleIndexControl.GetNodeLevel(fNode);
			if(IsClientFiltering && (TitleIndexControl.GetNodeNavigateUrl(Node) == "")) {
				fSpanTextControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				parentControl.Controls.Add(fSpanTextControl);
				fSpanTextControl.Controls.Add(link);
				if(level == 1 && IsEnabled())
					fSpanTextControl.CssClass = TitleIndexStyles.LinkSystemStyleName;
			} else {
				parentControl.Controls.Add(link);
				if(level == 1 && IsEnabled())
					link.CssClass = TitleIndexStyles.LinkSystemStyleName;
			}
			return link;
		}
		protected void PrepareNodeTextControl(HyperLink control) {
			int level = TitleIndexControl.GetNodeLevel(fNode);
			string navigateUrl = TitleIndexControl.GetNodeNavigateUrl(Node);
			string textControlID = TitleIndexControl.GetItemElementID(Node.Item);
			if (fSpanTextControl != null) {
				if (level == 1)
					fSpanTextControl.ID = textControlID;
			} else
				if (level == 1) control.ID = textControlID;
			RenderUtils.PrepareHyperLink(control, TitleIndexControl.HtmlEncode(TitleIndexControl.GetNodeText(Node)),
				navigateUrl, TitleIndexControl.GetNodeTarget(Node), Node.Description,
				TitleIndexControl.IsEnabled());
				TitleIndexControl.GetItemLinkStyle(Node).AssignToHyperLink(control);
		}
	}
	public class TICNodeContentControl : TICNodeControlBase {
		private HyperLink fNodeTextControl = null;
		private TableCell fTextCell = null;
		private Table fTextTable = null;
		public TICNodeContentControl(TICTreeViewControl treeViewControl, TitleIndexNode node)
			: base(treeViewControl, node) {
		}
		protected override void ClearControlFields() {
			fNodeTextControl = null;
			fTextCell = null;
			fTextTable = null;
		}
		protected override void CreateControlHierarchy() {
			if (TitleIndexControl.NeedTableNodeRender(Node)) {
				fTextTable = RenderUtils.CreateTable();
				Controls.Add(RenderUtils.CreateAnchor(TitleIndexControl.GetBookmarkUniqueID(Node.Text)));
				Controls.Add(fTextTable);
				TableRow row = RenderUtils.CreateTableRow();
				fTextTable.Rows.Add(row);
				fTextCell = RenderUtils.CreateTableCell();
				row.Cells.Add(fTextCell);
				fNodeTextControl = CreateNodeTextControl(fTextCell);
			}
			else {
				Controls.Add(RenderUtils.CreateAnchor(TitleIndexControl.GetBookmarkUniqueID(Node.Text)));
				fNodeTextControl = CreateNodeTextControl(this);
			}
		}
		protected override void PrepareControlHierarchy() {
			PrepareNodeTextControl(fNodeTextControl);
			if (fTextTable != null) {
				TitleIndexControl.GetItemStyle(Node).AssignToControl(fTextTable, AttributesRange.Font);
				TitleIndexControl.GetGroupHeaderTextPaddings().AssignToControl(fTextCell);
				TitleIndexControl.GetGroupHeaderTextStyle().AssignToControl(fTextCell);
			}
		}
	}
	public class TICTreeViewNodeControlBase : TICNodeControlBase {
		private HyperLink fNodeTextControl = null;
		private WebControl fChildItemsContentDiv = null;
		protected WebControl fDiv = null;
		protected Image fImage = null;
		protected WebControl fNodeDiv = null;
		protected WebControl fTemplateDiv = null;
		protected WebControl fUlControl = null;
		public TICTreeViewNodeControlBase(TICTreeViewControl treeViewControl, TitleIndexNode node)
			: base(treeViewControl, node) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fChildItemsContentDiv = null;
			fDiv = null;
			fImage = null;
			fNodeDiv = null;
			fNodeTextControl = null;
			fTemplateDiv = null;
			fUlControl = null;
		}
		protected override void CreateControlHierarchy() {
			fDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(fDiv);
			AddControlsToMainDiv(fDiv);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.SetPaddings(fDiv, TitleIndexControl.GetItemMargins(Node, 
				TreeViewControl.IsFirstInColumn(Node))); 
			if (fChildItemsContentDiv != null) {
				fDiv.CssClass = TitleIndexStyles.GroupSystemStyleName; 
				TitleIndexControl.GetGroupContentStyle().AssignToControl(fChildItemsContentDiv);
				TitleIndexControl.GetGroupContentPadding(Node).AssignToControl(fChildItemsContentDiv);
			}
			else
				fDiv.CssClass = TitleIndexStyles.ItemSystemStyleName; 
			if (fTemplateDiv == null)
				PrepareNodeDiv();
		}
		protected virtual void AddControlsToMainDiv(WebControl mainDiv) {
			int level = TitleIndexControl.GetNodeLevel(Node);
			ITemplate template = TitleIndexControl.GetItemTemplate(Node);
			if (template != null) {
				fTemplateDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				TemplateContainerBase container = null;
				string containerID;
				if (level == 0) {
					object groupValue = TitleIndexControl.GetGroupValue(Node);
					container = new GroupHeaderTemplateContainer(Node.Index, groupValue, TitleIndexControl.GetGroupItemCount(groupValue));
					containerID = TitleIndexControl.GetGroupHeaderTemplateContainerID(Node.Item);
				}
				else {
					container = new TitleIndexItemTemplateContainer(Node.Item);
					containerID = TitleIndexControl.GetItemTemplateContainerID(Node.Item);
				}
				template.InstantiateIn(container);
				container.AddToHierarchy(fTemplateDiv, containerID);
				mainDiv.Controls.Add(fTemplateDiv);
			}
			else
				CreateNodeDiv(mainDiv);
			CreateChildNodeControls(mainDiv);
		}	   
		protected virtual void CreateNodeDiv(WebControl parent) {
			fNodeDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(fNodeDiv);
			if (TitleIndexControl.IsBulletMode(Node))
				CreateUL(fNodeDiv);
			else {
				if (!TitleIndexControl.GetItemImage(Node).IsEmpty) {
					fImage = RenderUtils.CreateImage();
					fNodeDiv.Controls.Add(fImage);
				}
				fNodeTextControl = CreateNodeTextControl(fNodeDiv);
			}
		}
		protected virtual void CreateChildNodeControls(WebControl parentControl) {
			if (TitleIndexControl.HasChildNodes(Node)) {
				fChildItemsContentDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				parentControl.Controls.Add(fChildItemsContentDiv);
				foreach (TitleIndexNode childNode in Node.ChildNodes)
					fChildItemsContentDiv.Controls.Add(TreeViewControl.CreateNode(childNode));
			}
		}
		protected void CreateUL(WebControl parent) {
			fUlControl = RenderUtils.CreateList(ListType.Bulleted);
			parent.Controls.Add(fUlControl);
			WebControl liControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Li);
			fUlControl.Controls.Add(liControl);
			fNodeTextControl = CreateNodeTextControl(liControl);
		}
		protected virtual void PrepareNodeDiv() {
			TitleIndexControl.GetItemStyle(Node).AssignToControl(fNodeDiv, AttributesRange.All);
			RenderUtils.SetPaddings(fNodeDiv, TitleIndexControl.GetItemPaddings(Node)); 
			if (fUlControl != null)
				PrepareUlControl();
			else {
				if (fImage != null) {
					TitleIndexControl.GetItemImage(Node).AssignToControl(fImage, DesignMode);
					if (TitleIndexControl.GetItemStyle(Node).VerticalAlign != VerticalAlign.NotSet)
						RenderUtils.SetVerticalAlign(fImage, TitleIndexControl.GetItemStyle(Node).VerticalAlign);
					else
						fImage.ImageAlign = ImageAlign.AbsMiddle; 
					if(IsRightToLeft) {
						RenderUtils.SetHorizontalMargins(fImage, TitleIndexControl.GetImageSpacing(Node), 0);
					} else {
						RenderUtils.SetHorizontalMargins(fImage, 0, TitleIndexControl.GetImageSpacing(Node));
					}
				}
			}
			if (fNodeTextControl != null)
				PrepareNodeTextControl(fNodeTextControl);
		}
		protected void PrepareUlControl() {
			RenderUtils.SetStyleStringAttribute(fUlControl, "list-style-type", TitleIndexControl.GetItemBulletStyle(Node).ToString().ToLower());
			Unit bulletMargin = TitleIndexControl.GetBulletIndent(Node);
			RenderUtils.SetMargins(fUlControl, IsRightToLeft ? 0 : bulletMargin, 0, IsRightToLeft ? bulletMargin : 0, 0);
		}
	}
	public class TICTreeViewNodeControl : TICTreeViewNodeControlBase {
		private BackToTopControl fBackToTopControl = null;
		public TICTreeViewNodeControl(TICTreeViewControl treeViewControl, TitleIndexNode node)
			: base(treeViewControl, node) {
		}
		protected override void AddControlsToMainDiv(WebControl mainDiv) {
			mainDiv.Controls.Add(RenderUtils.CreateAnchor(TitleIndexControl.GetBookmarkUniqueID(Node.Text)));
			base.AddControlsToMainDiv(mainDiv);
		}
		protected override void CreateChildNodeControls(WebControl parentControl) {
			base.CreateChildNodeControls(parentControl);
			if (TitleIndexControl.IsShowBackToTop())
				AddBackToTop(parentControl);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fBackToTopControl = null;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareBackToTop();
		}
		protected void AddBackToTop(WebControl parentControl) {
			fBackToTopControl = new BackToTopControl(TitleIndexControl.GetIndexPanelBookmark(), IsRightToLeft);
			fBackToTopControl.Text = TitleIndexControl.BackToTopText;
			fBackToTopControl.BackToTopImage = TitleIndexControl.GetBackToTopImage();
			parentControl.Controls.Add(fBackToTopControl);
		}
		protected void PrepareBackToTop() {
			if (fBackToTopControl != null) {
				fBackToTopControl.BackToTopStyle = TitleIndexControl.GetBackToTopStyle();
				fBackToTopControl.BackToTopImageSpacing = TitleIndexControl.GetBackToTopImageSpacing();
				fBackToTopControl.BackToTopPaddings = TitleIndexControl.GetBackToTopPadding();
				fBackToTopControl.Enabled = TitleIndexControl.IsEnabled();
				fBackToTopControl.BackToTopSpacing = TitleIndexControl.GetBackToTopSpacing();
			}
		}
	}
	public class TICTreeViewWrapNodeControlBase : TICTreeViewNodeControlBase {
		private Table fImageTextTable = null;
		private TableCell fImageCell = null;
		private TableCell fTextCell = null;
		private HyperLink fNodeTextControl = null;
		public TICTreeViewWrapNodeControlBase(TICTreeViewControl treeViewControl, TitleIndexNode node)
			: base(treeViewControl, node) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fImageTextTable = null;
			fImageCell = null;
			fTextCell = null;
			fNodeTextControl = null;
		}
		protected override void CreateNodeDiv(WebControl parent) {
			fNodeDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(fNodeDiv);
			fImageTextTable = RenderUtils.CreateTable();
			fNodeDiv.Controls.Add(fImageTextTable);
			TableRow row = RenderUtils.CreateTableRow();
			fImageTextTable.Rows.Add(row);
			if (TitleIndexControl.GetItemImage(Node).Url != "") {
				fImageCell = RenderUtils.CreateTableCell();
				row.Cells.Add(fImageCell);
				fImage = RenderUtils.CreateImage();
				fImageCell.Controls.Add(fImage);
			}
			fTextCell = RenderUtils.CreateTableCell();
			row.Cells.Add(fTextCell);
			fNodeTextControl = CreateNodeTextControl(fTextCell);
		}
		protected override void PrepareNodeDiv() {
			AppearanceStyleBase itemContentStyle = TitleIndexControl.GetItemStyle(Node);
			itemContentStyle.AssignToControl(fNodeDiv, AttributesRange.Common);
			if (itemContentStyle.HorizontalAlign != 
				HorizontalAlign.NotSet) {
				RenderUtils.SetAttribute(fNodeDiv, "align", itemContentStyle.HorizontalAlign.ToString(), "");
			}
			RenderUtils.SetPaddings(fNodeDiv, TitleIndexControl.GetItemPaddings(Node)); 
			TitleIndexControl.GetItemStyle(Node).AssignToControl(fImageTextTable, AttributesRange.Font);
			if (fImageCell != null) {
				TitleIndexControl.GetItemStyle(Node).AssignToControl(fImageCell, AttributesRange.Cell);
				TitleIndexControl.GetItemImage(Node).AssignToControl(fImage, DesignMode);
				fImage.ImageAlign = ImageAlign.Middle;
				if(IsRightToLeft) {
					RenderUtils.SetHorizontalMargins(fImage, TitleIndexControl.GetImageSpacing(Node), Unit.Empty);
				} else {
					RenderUtils.SetHorizontalMargins(fImage, Unit.Empty, TitleIndexControl.GetImageSpacing(Node));
				}
			}
			PrepareTextCell(fTextCell);
			PrepareNodeTextControl(fNodeTextControl);
		}
		protected virtual void PrepareTextCell(TableCell cell) {
			TitleIndexControl.GetItemStyle(Node).AssignToControl(cell, AttributesRange.Cell | AttributesRange.Font);
		}
	}
	public class TICTreeViewWrapNodeControl : TICTreeViewWrapNodeControlBase {
		private BackToTopControl fBackToTopControl = null;
		public TICTreeViewWrapNodeControl(TICTreeViewControl treeViewControl, TitleIndexNode node)
			: base(treeViewControl, node) {
		}
		protected override void AddControlsToMainDiv(WebControl mainDiv) {
			mainDiv.Controls.Add(RenderUtils.CreateAnchor(TitleIndexControl.GetBookmarkUniqueID(Node.Text)));
			base.AddControlsToMainDiv(mainDiv);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fBackToTopControl = null;
		}
		protected override void CreateChildNodeControls(WebControl parentControl) {
			base.CreateChildNodeControls(parentControl);
			if (TitleIndexControl.IsShowBackToTop())
				AddBackToTop(parentControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareBackToTop();
		}
		protected void AddBackToTop(WebControl parentControl) {
			fBackToTopControl = new BackToTopControl(TitleIndexControl.GetIndexPanelBookmark(), IsRightToLeft);
			fBackToTopControl.Text = TitleIndexControl.BackToTopText;
			fBackToTopControl.BackToTopImage = TitleIndexControl.GetBackToTopImage();
			parentControl.Controls.Add(fBackToTopControl);
		}
		protected void PrepareBackToTop() {
			if (fBackToTopControl != null) {
				fBackToTopControl.BackToTopStyle = TitleIndexControl.GetBackToTopStyle();
				fBackToTopControl.BackToTopImageSpacing = TitleIndexControl.GetBackToTopStyle().ImageSpacing;
				fBackToTopControl.BackToTopPaddings = TitleIndexControl.GetBackToTopPadding();
				fBackToTopControl.Enabled = TitleIndexControl.IsEnabled();
				fBackToTopControl.BackToTopSpacing = TitleIndexControl.GetBackToTopSpacing();
			}
		}
		protected override void PrepareTextCell(TableCell cell) {
			if (TitleIndexControl.GetGroupHeaderTextStyle().IsEmpty)
				base.PrepareTextCell(cell);
			else {
				TitleIndexControl.GetGroupHeaderTextPaddings().AssignToControl(cell);
				TitleIndexControl.GetGroupHeaderTextStyle().AssignToControl(cell);
			}
		}
	}
	public class AlphabetPagerItemInfo {
		public WebControl SpanControl = null;
		public LiteralControl SeparatorTextControl = null;
		public HyperLink TextHyperLink = null;
		public LiteralControl TextControl = null;
	}
	public class AlphabetPager : ASPxInternalWebControl {
		List<string[]> fStringSetInLines = null;
		List<AlphabetPagerLine> fLines = null;
		ASPxTitleIndex fTitleIndexControl = null;
		private Table fTable = null;
		private TableCell fContentCell = null;
		public ASPxTitleIndex TitleIndexControl {
			get { return fTitleIndexControl; }
		}
		public AlphabetPager(ASPxTitleIndex titleIndexControl, List<string[]> charSetInLines) {
			fStringSetInLines = charSetInLines;
			fTitleIndexControl = titleIndexControl;
		}
		protected override void ClearControlFields() {
			fLines = null;
			fTable = null;
			fContentCell = null;
		}
		protected override void CreateControlHierarchy() {
			fLines = new List<AlphabetPagerLine>();
			fTable = RenderUtils.CreateTable();
			Controls.Add(fTable);
			TableRow row = RenderUtils.CreateTableRow();
			fTable.Rows.Add(row);
			fContentCell = RenderUtils.CreateTableCell();
			row.Cells.Add(fContentCell);
			for (int i = 0; i < fStringSetInLines.Count; i++) {
				AlphabetPagerLine line = new AlphabetPagerLine(this, fStringSetInLines[i]);
				fContentCell.Controls.Add(line);
				if (fStringSetInLines.Count > 1)
					fContentCell.Controls.Add(RenderUtils.CreateBr());
				fLines.Add(line);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			IndexPanelStyle indexPanelStyle = TitleIndexControl.GetIndexPanelStyle();
			RenderUtils.SetHorizontalAlignClass(fContentCell, indexPanelStyle.HorizontalAlign);
		}
		protected internal bool IsFirstLineInPager(AlphabetPagerLine line) {
			return fLines.IndexOf(line) == 0;
		}
	}
	public class AlphabetPagerLine : ASPxInternalWebControl {
		private List<AlphabetPagerItemInfo> fAlphabetPagerItemsInfo = null;
		private AlphabetPager fPager = null;
		private string[] fSymbols = null;
		public ASPxTitleIndex TitleIndexControl {
			get { return fPager.TitleIndexControl; }
		}
		protected bool IsRightToLeft {
			get { return (TitleIndexControl as ISkinOwner).IsRightToLeft(); }
		}
		public AlphabetPagerLine(AlphabetPager pager, string[] symbols) {
			fPager = pager;
			fSymbols = symbols;
		}
		protected override void ClearControlFields() {
			fAlphabetPagerItemsInfo = null;
		}
		protected override void CreateControlHierarchy() {
			fAlphabetPagerItemsInfo = new List<AlphabetPagerItemInfo>();
			List<string> existingCharSet = new List<string>(TitleIndexControl.GetExistingCharSet());
			for (int i = 0; i < fSymbols.Length; i++) {
				AlphabetPagerItemInfo info = new AlphabetPagerItemInfo();
				ITemplate indexPanelItemTemplate = TitleIndexControl.GetIndexPanelItemTemplate();
				if (indexPanelItemTemplate != null) {
					object groupValue = TitleIndexControl.GetGroupValue(i);
					string bookMarkUrl = TitleIndexControl.GetBookmarkLinkBySymbol(fSymbols[i]);
					IndexPanelItemTemplateContainer container = new IndexPanelItemTemplateContainer(i, groupValue,
						TitleIndexControl.GetGroupItemCount(groupValue), bookMarkUrl);
					info.SpanControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
					indexPanelItemTemplate.InstantiateIn(container);
					container.AddToHierarchy(info.SpanControl, null);
					Controls.Add(info.SpanControl);
				}
				else {
					info.SpanControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
					Controls.Add(info.SpanControl);
					if (existingCharSet.Contains(fSymbols[i]) &&
						!TitleIndexControl.IsCurrentIndexPanelIndex(fSymbols[i]) && !TitleIndexControl.IsPaging()) {
						info.TextHyperLink = RenderUtils.CreateHyperLink(false);
						info.SpanControl.Controls.Add(info.TextHyperLink);
					}
					else {
						info.TextControl = RenderUtils.CreateLiteralControl();
						info.SpanControl.Controls.Add(info.TextControl);
					}
				}
				if (i != fSymbols.Length - 1) {
					info.SeparatorTextControl = RenderUtils.CreateLiteralControl();
					Controls.Add(info.SeparatorTextControl);
				}
				fAlphabetPagerItemsInfo.Add(info);
			}
		}
		protected override void PrepareControlHierarchy() {
			foreach (AlphabetPagerItemInfo info in fAlphabetPagerItemsInfo) {
				int i = fAlphabetPagerItemsInfo.IndexOf(info);
				TitleIndexControl.GetIndexPanelItemStyle(fSymbols[i]).AssignToControl(info.SpanControl);
				if (TitleIndexControl.HasAlphabetItemCellOnClick(fSymbols[i]))
					RenderUtils.SetStringAttribute(info.SpanControl, "onclick", TitleIndexControl.GetIndexPanelItemCellOnClick(fSymbols[i]));
				Paddings itemPaddings = TitleIndexControl.GetIndexPanelItemPadding();
				itemPaddings.AssignToControl(info.SpanControl);
				string itemText = TitleIndexControl.GetIndexPanelItemFormattedText(fSymbols[i]);
				if (info.TextHyperLink != null) {
					RenderUtils.PrepareHyperLink(info.TextHyperLink, itemText, 
						TitleIndexControl.GetBookmarkLinkBySymbol(fSymbols[i]), "", "", TitleIndexControl.IsEnabled());
					TitleIndexControl.GetIndexPanelItemLinkStyle().AssignToHyperLink(info.TextHyperLink);
					if (Browser.Family.IsWebKit) {
						Paddings correctingPaddings = new Paddings(Unit.Empty, Unit.Empty, Unit.Empty, itemPaddings.GetPaddingBottom());
						correctingPaddings.AssignToControl(info.TextHyperLink);
					}
				}
				else
					if (info.TextControl != null) {
						TitleIndexControl.GetIndexPanelItemStyle(fSymbols[i]).AssignToControl(info.SpanControl,
							AttributesRange.All);
						info.TextControl.Text = HttpUtility.HtmlEncode(itemText);
					}
				if (info.SeparatorTextControl != null)
					info.SeparatorTextControl.Text = TitleIndexControl.GetIndexPanelItemSeparator();
				if(IsRightToLeft)
					info.SpanControl.Attributes["dir"] = "rtl";
			}
		}
	}
}
