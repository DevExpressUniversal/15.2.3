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
namespace DevExpress.Web.Internal {
	public class SMCControlBase : ASPxInternalWebControl {
		private ASPxSiteMapControlBase fSiteMapControlBase = null;
		public ASPxSiteMapControlBase SiteMapControlBase {
			get { return fSiteMapControlBase; }
		}
		public SMCControlBase(ASPxSiteMapControlBase siteMapControl)
			: base() {
			fSiteMapControlBase = siteMapControl;
		}
		public bool IsRightToLeft {
			get { return (SiteMapControlBase as ISkinOwner).IsRightToLeft(); }
		}
	}
	public abstract class SMCMainControlBase : SMCControlBase {
		private Table fMainTable = null;
		protected TableCell fMainCell = null;
		public SMCMainControlBase(ASPxSiteMapControlBase siteMapControlBase)
			: base(siteMapControlBase) {
		}
		protected override void ClearControlFields() {
			fMainTable = null;
			fMainCell = null;
		}
		protected override void CreateControlHierarchy() {
			fMainTable = RenderUtils.CreateTable();
			Controls.Add(fMainTable);
			CreateContent(fMainTable);
		}
		protected override void PrepareControlHierarchy() {
			AppearanceStyleBase style = SiteMapControlBase.GetControlStyle();
			RenderUtils.AssignAttributes(SiteMapControlBase, fMainTable);
			style.AssignToControl(fMainTable, AttributesRange.Common | AttributesRange.Font);
			style.AssignToControl(fMainCell, AttributesRange.Cell);
			fMainCell.VerticalAlign = VerticalAlign.Top;
			RenderUtils.SetPaddings(fMainCell, SiteMapControlBase.GetPaddings());
		}
		protected virtual void AddControlsToMainCell(TableCell cell) {
			cell.Controls.Add(new SMCTreeViewControl(SiteMapControlBase, SiteMapControlBase.RootNodes));
		}
		protected virtual void CreateContent(Table mainTable) {
			TableRow row = RenderUtils.CreateTableRow();
			mainTable.Rows.Add(row);
			fMainCell = RenderUtils.CreateTableCell();
			row.Cells.Add(fMainCell);
			AddControlsToMainCell(fMainCell);
		}
	}
	public class SMCMainControl : SMCMainControlBase {
		public ASPxSiteMapControl SiteMapControl {
			get { return SiteMapControlBase as ASPxSiteMapControl; }
		}
		public SMCMainControl(ASPxSiteMapControl siteMapControl)
			: base(siteMapControl) {
		}
		protected override void AddControlsToMainCell(TableCell cell) {
			if (((SiteMapControl.Categorized) || (SiteMapControl.RepeatDirection == RepeatDirection.Horizontal)) &&
				!SiteMapControlBase.IsFlowLayoutLevel(0))
				cell.Controls.Add(new SMCMultiRowTreeViewControl(SiteMapControlBase, SiteMapControlBase.RootNodes));
			else
				base.AddControlsToMainCell(cell);
		}
	}
	public class SiteMapControlColumnInfo {
		public TableCell ColumnCell = null;
		public TableCell LeftPaddingCell = null;
		public TableCell RightPaddingCell = null;
		public TableCell SeparatorCell = null;
		public Table ContentColumnTable = null;
	}
	public class SMCTreeViewControl : SMCControlBase {
		private int[] fPartsInColumn = null;
		private List<SiteMapControlColumnInfo> fColumnsContentInfo = null;
		private Dictionary<SiteMapNode, int> fNodeColumnIndexDictonary = null;
		private SiteMapNodeCollection fRootNodes = null;
		protected Table fTable = null;
		protected SiteMapNodeCollection RootNodes {
			get { return fRootNodes; }
		}
		public SMCTreeViewControl(ASPxSiteMapControlBase siteMapControl, SiteMapNodeCollection rootNodes)
			: base(siteMapControl) {
			fRootNodes = rootNodes;
		}
		protected virtual SMTreeViewNodeControl CreateNodeControl(SiteMapNode node) {
			return new SMTreeViewNodeControl(this, node);
		}
		protected virtual SMTreeViewNodeControl CreateWrapNodeControl(SiteMapNode node) {
			return new SMTreeViewWrapNodeControl(this, node);
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
			fNodeColumnIndexDictonary = new Dictionary<SiteMapNode, int>();
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
		protected virtual SiteMapNode GetCategoryNode(SiteMapNode node) {
			SiteMapNode categoryNode = node;
			while (!SiteMapControlBase.IsRootNode(categoryNode) && categoryNode != null)
				categoryNode = SiteMapControlBase.GetParentNode(categoryNode);
			return categoryNode;
		}
		protected virtual int GetColumnCount(int categoryNodeCount) {
			return SiteMapControlBase.ColumnCountActual;
		}
		protected internal virtual int[] GetPartNumbers() {
			int[] ret = new int[SiteMapControlBase.GetColumns().Count];
			for (int i = 0; i < SiteMapControlBase.GetColumns().Count; i++)
				ret[i] = SiteMapControlBase.GetColumns()[i].StartingNodeIndex;
			return ret;
		}
		protected virtual bool IsFirstInColumn(SiteMapNode node) {
			SiteMapNode curRootColumnNode = node;
			if (SiteMapControlBase.IsRootNode(curRootColumnNode)) {
				SiteMapNode previousSibling = SiteMapControlBase.GetPreviousSibling(node);
				return (previousSibling == null) ||
					(GetColumnIndex(node) != GetColumnIndex(previousSibling));
			}
			else
				return false;
		}
		protected internal virtual bool IsLastNodeInColumn(SiteMapNode node) {
			SiteMapNode nextNode = SiteMapControlBase.GetNextNode(node);
			if (nextNode == null)
				return true;
			else {
				SiteMapNode curCategoryNode = GetCategoryNode(node);
				SiteMapNode nextCategoryNode = GetCategoryNode(nextNode);
				return GetColumnIndex(curCategoryNode) != GetColumnIndex(nextCategoryNode);
			}
		}
		protected internal bool IsLastCategoryNodeInColumn(SiteMapNode node) {
			return SiteMapControlBase.GetNextSibling(node) == null ||
				GetColumnIndex(node) != GetColumnIndex(SiteMapControlBase.GetNextSibling(node));
		}
		protected void AddColumnSeparator(SiteMapControlColumnInfo columnInfo, TableRow row, int index) {
			columnInfo.LeftPaddingCell = RenderUtils.CreateIndentCell();
			row.Cells.Add(columnInfo.LeftPaddingCell);
			ITemplate template = SiteMapControlBase.ColumnSeparatorTemplate;
			if (template != null) {
				SiteMapControlColumnSeparatorTemplateContainer container = new SiteMapControlColumnSeparatorTemplateContainer(index, RootNodes[0]); 
				columnInfo.SeparatorCell = RenderUtils.CreateTableCell();
				template.InstantiateIn(container);
				row.Cells.Add(columnInfo.SeparatorCell);
				container.AddToHierarchy(columnInfo.SeparatorCell, null);
			}
			else {
				columnInfo.SeparatorCell = RenderUtils.CreateIndentCell();
				row.Cells.Add(columnInfo.SeparatorCell);
			}
			columnInfo.RightPaddingCell = RenderUtils.CreateIndentCell();
			row.Cells.Add(columnInfo.RightPaddingCell);
		}
		protected List<SiteMapControlColumnInfo> CreateColumns(TableRow parentRow, SiteMapNodeCollection categoryNodes) {
			List<SiteMapControlColumnInfo> columnsContentInfo = new List<SiteMapControlColumnInfo>();
			List<List<SiteMapNode>> columnsNodeList = new List<List<SiteMapNode>>();
			int columnCount = GetColumnCount(categoryNodes.Count);
			for (int i = 0; i < columnCount; i++)
				columnsNodeList.Add(new List<SiteMapNode>());
			if (columnCount == 1) {
				foreach (SiteMapNode node in categoryNodes)
					columnsNodeList[0].Add(node);
			}
			else {
				int[] partNumbers = GetPartNumbers();
				if (IsValidNodeIndexesInColumns(partNumbers, categoryNodes.Count))
					fPartsInColumn = CommonUtils.ArrangePartsByStartingNumber(partNumbers, categoryNodes.Count, columnCount);
				else {
					int[] partHeights = GetPartHeightsByNode(categoryNodes);
					fPartsInColumn = CommonUtils.ArrangeParts(partHeights, columnCount);
				}
				int nodeIndex = 0;
				for (int i = 0; i < columnCount; i++) {
					for (int j = 0; j < fPartsInColumn[i]; j++) {
						columnsNodeList[i].Add(categoryNodes[nodeIndex]);
						nodeIndex++;
					}
				}
			}
			for (int i = 0; i < columnsNodeList.Count; i++) {
				foreach (SiteMapNode node in columnsNodeList[i])
					fNodeColumnIndexDictonary.Add(node, i);
			}
			for (int i = 0; i < columnCount; i++) {
				SiteMapControlColumnInfo columnInfo = new SiteMapControlColumnInfo();
				columnsContentInfo.Add(columnInfo);
				columnInfo.ColumnCell = RenderUtils.CreateTableCell();
				parentRow.Cells.Add(columnInfo.ColumnCell);
				columnInfo.ContentColumnTable = RenderUtils.CreateTable(true);
				columnInfo.ColumnCell.Controls.Add(columnInfo.ContentColumnTable);
				TableRow row = RenderUtils.CreateTableRow();
				columnInfo.ContentColumnTable.Rows.Add(row);
				TableCell cell = RenderUtils.CreateTableCell();
				row.Cells.Add(cell);
				if (!SiteMapControlBase.IsFlowLayoutLevel(0))
					foreach (SiteMapNode node in columnsNodeList[i])
						cell.Controls.Add(CreateNode(node));
				else
					cell.Controls.Add(CreateFlowLayoutNode(null)); 
				if (i != columnCount - 1)
					AddColumnSeparator(columnInfo, parentRow, columnsContentInfo.Count);
			}
			return columnsContentInfo;
		}
		protected void PrepareColumns(List<SiteMapControlColumnInfo> columnsContentInfo, int rowIndex) {
			Unit columnSeparatorWidth = SiteMapControlBase.GetColumnSeparatorWidth();
			AppearanceStyleBase columnSeparatorStyle = SiteMapControlBase.GetColumnSeparatorStyle();
			Paddings columnSeparatorPaddings = SiteMapControlBase.GetColumnSeparatorPaddings();
			foreach(SiteMapControlColumnInfo columnInfo in columnsContentInfo) {
				int columnIndex = columnsContentInfo.IndexOf(columnInfo);
				ColumnStyle columnStyle = SiteMapControlBase.GetColumnStyle(columnIndex);
				Unit columnWidth = SiteMapControlBase.GetColumnWidth(columnIndex);
				columnInfo.ColumnCell.Width = columnWidth;
				if ((columnWidth.Type == UnitType.Percentage) || (columnWidth.IsEmpty))
					columnInfo.ContentColumnTable.Width = new Unit(100, UnitType.Percentage);
				else
					columnInfo.ContentColumnTable.Width = columnWidth;
				columnStyle.AssignToControl(columnInfo.ColumnCell, AttributesRange.Common);
				RenderUtils.SetPaddings(columnInfo.ColumnCell, SiteMapControlBase.GetColumnPaddings(columnIndex));
				if(!SiteMapControlBase.Categorized && 
					SiteMapControlBase.RepeatDirection != RepeatDirection.Horizontal)
					columnInfo.ColumnCell.ID = SiteMapControlBase.GetColumnID(columnIndex, rowIndex);
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
			SiteMapControlBase.GetControlStyle().AssignToControl(fTable, AttributesRange.Font);
			if ((GetColumnCount(RootNodes.Count) <= 1) ||
				(SiteMapControlBase.GetControlStyle().Width.Type == UnitType.Percentage))
				fTable.Width = Unit.Percentage(100);
			if (SiteMapControlBase.GetControlStyle().Width.IsEmpty) {
				if(Browser.IsOpera && Browser.MajorVersion == 8)
					fTable.Width = 0;
				else if(Browser.IsOpera || Browser.Family.IsWebKit)
						fTable.Width = Unit.Percentage(100);
			}
		}
		protected internal Paddings GetNodeMargins(SiteMapNode node, NodeControlBase nodeControl) {
			if(nodeControl is SMTreeViewNodeControl)
				return SiteMapControlBase.GetNodeMarginsInternal(node,
					IsFirstInColumn(node), IsLastNodeInColumn(node));
			else
				if (SiteMapControlBase.Categorized) {
					return SiteMapControlBase.GetNodeMarginsInternal(node,
						IsFirstInColumn(node), IsLastNodeInColumn(node));
				}
				else {
					SiteMapNodeCollection allNodes = node.GetAllNodes();
					return SiteMapControlBase.GetNodeMarginsInternal(node.ChildNodes[0], false, IsLastNodeInColumn(allNodes[allNodes.Count - 1]));
				}
		}
		protected bool IsValidNodeIndexesInColumns(int[] partNumbers, int partCount) {
			bool ret = true;
			for (int i = 0; i < partNumbers.Length - 1; i++)
				if ((partNumbers[i] >= partNumbers[i + 1]) || (partNumbers[i + 1] == -1) || (partNumbers[i + 1] >= partCount)) {
					ret = false;
					break;
				}
			return ret;
		}
		protected internal int[] GetPartHeightsByNode(SiteMapNodeCollection rootNodes) {
			int[] ret = new int[rootNodes.Count];
			int nodeIndex = 0;
			foreach (SiteMapNode node in rootNodes) {
				ret[nodeIndex] = GetNodeCount(node) + 1;
				nodeIndex++;
			}
			return ret;
		}
		protected int GetColumnIndex(SiteMapNode node) {
			return node != null ? fNodeColumnIndexDictonary[node] : -1;
		}
		protected int GetNodeCount(SiteMapNode node) {
			int ret = 0;
			if (SiteMapControlBase.HasChildNodes(node)) {
				ret = node.ChildNodes.Count;
				foreach (SiteMapNode childNode in node.ChildNodes)
					ret = ret + GetNodeCount(childNode);
			}
			else
				ret = 0;
			return ret;
		}
		protected internal InLineNodeContentControl CreateFlowLayoutItemNode(SiteMapNode node) {
			return new InLineNodeContentControl(this, node);
		}
		protected internal FlowLayoutNodeControl CreateFlowLayoutNode(SiteMapNode node) {
			return new FlowLayoutNodeControl(this, node);
		}
		protected internal NodeControlBase CreateNode(SiteMapNode node) {
			if (SiteMapControlBase.NeedTableNodeRender(node))
				return CreateWrapNodeControl(node);
			else
				return CreateNodeControl(node);
		}
	}
	public class SiteMapControlCategoryInfo {
		public TableCell SpacingCell = null;
		public NodeContentControl HeaderContentControl = null;
		public TableCell HeaderCell = null;
		public List<SiteMapControlRowInfo> Rows = null;
		public TableCell ChildNodePaddingTopCell = null;
		public TableCell ChildNodePaddingBottomCell = null;
	}
	public class SiteMapControlRowInfo {
		public List<SiteMapControlColumnInfo> ColumnsContentInfo = null;
		public TableCell SpacingCell = null;
	}
	public class NodeControlBase : ASPxInternalWebControl {
		private SiteMapNode fNode = null;
		private ASPxSiteMapControlBase fSiteMapControlBase = null;
		private SMCTreeViewControl fTreeViewControl = null;
		public SiteMapNode Node {
			get { return fNode; }
		}
		public SMCTreeViewControl TreeViewControl {
			get { return fTreeViewControl; }
		}
		protected ASPxSiteMapControlBase SiteMapControlBase {
			get { return fSiteMapControlBase; }
		}
		protected bool IsRightToLeft {
			get { return TreeViewControl.IsRightToLeft; }
		}
		public NodeControlBase(SMCTreeViewControl treeViewControl, SiteMapNode node) {
			fTreeViewControl = treeViewControl;
			fSiteMapControlBase = treeViewControl.SiteMapControlBase;
			fNode = node;
		}
		protected HyperLink CreateNodeTextControl() {
			return RenderUtils.CreateHyperLink();
		}
		protected void CreateNodeTextTemplate(WebControl parent, ITemplate template, SiteMapNode node) {
			NodeTemplateContainer container = new NodeTemplateContainer(Node);
			template.InstantiateIn(container);
			container.AddToHierarchy(parent, SiteMapControlBase.GetNodeTextTemplateContainerID(Node));
		}
		protected void PrepareNodeTextControl(HyperLink control) {
			RenderUtils.PrepareHyperLink(control, SiteMapControlBase.HtmlEncode(Node.Title),
				SiteMapControlBase.GetNodeUrl(Node), SiteMapControlBase.GetNodeTarget(Node),
				Node.Description, SiteMapControlBase.IsEnabled());
			SiteMapControlBase.GetNodeLinkStyle(Node).AssignToHyperLink(control);
		}
	}
	public class NodeContentControl : NodeControlBase {
		private HyperLink fNodeTextControl = null;
		public NodeContentControl(SMCTreeViewControl treeViewControl, SiteMapNode node)
			: base(treeViewControl, node) {
		}
		protected override void ClearControlFields() {
			fNodeTextControl = null;
		}
		protected override void CreateControlHierarchy() {
			ITemplate nodeTextTemplate = SiteMapControlBase.GetNodeTextTemplate(Node);
			if (nodeTextTemplate != null)
				CreateNodeTextTemplate(this, nodeTextTemplate, Node);
			else {
				fNodeTextControl = CreateNodeTextControl();
				Controls.Add(fNodeTextControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			if (fNodeTextControl != null)
				PrepareNodeTextControl(fNodeTextControl);
		}
	}
	public class SMCMultiRowTreeViewControl : SMCTreeViewControl {
		private List<SiteMapControlCategoryInfo> fCategoriesInfo = null;	   
		public SMCMultiRowTreeViewControl(ASPxSiteMapControlBase siteMapControl, SiteMapNodeCollection rootNodes)
			: base(siteMapControl, rootNodes) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fCategoriesInfo = null;
		}
		protected override void CreateRows(Table table) {
			int[][] nodeCountInRows = CommonUtils.ArrangePartInRow(GetPartsCount(), 
				SiteMapControlBase.ColumnCountActual,
				SiteMapControlBase.Categorized, SiteMapControlBase.RepeatDirection);
			Dictionary<int, List<SiteMapNodeCollection>> rowsInCategoryNode = new Dictionary<int, List<SiteMapNodeCollection>>();
			for (int i = 0; i < nodeCountInRows.Length; i++) { 
				List<SiteMapNodeCollection> rowsNodes = new List<SiteMapNodeCollection>();
				int nodeIndex = 0;
				for (int j = 0; j < nodeCountInRows[i].Length; j++) { 
					SiteMapNodeCollection nodes = new SiteMapNodeCollection();
					for (int n = 0; n < nodeCountInRows[i][j]; n++) {
						if (SiteMapControlBase.Categorized)
							nodes.Add(RootNodes[i].ChildNodes[nodeIndex]);
						else
							nodes.Add(RootNodes[nodeIndex]);
						nodeIndex++;
					}
					rowsNodes.Add(nodes);
				}
				rowsInCategoryNode.Add(i, rowsNodes);
			}
			fCategoriesInfo = new List<SiteMapControlCategoryInfo>();
			foreach (int categoryNodeIndex in rowsInCategoryNode.Keys)
				fCategoriesInfo.Add(CreateCategory(table, categoryNodeIndex, rowsInCategoryNode[categoryNodeIndex],
					categoryNodeIndex == rowsInCategoryNode.Keys.Count - 1));
		}
		protected override SiteMapNode GetCategoryNode(SiteMapNode node) {
			SiteMapNode categoryNode = node;
			if (!SiteMapControlBase.IsRootNode(categoryNode)) {
				if (SiteMapControlBase.Categorized) {
					while (!SiteMapControlBase.IsRootNode(SiteMapControlBase.GetParentNode(categoryNode)))
						categoryNode = SiteMapControlBase.GetParentNode(categoryNode);
				}
				else
					categoryNode = base.GetCategoryNode(node);
			}
			return categoryNode;
		}
		protected override int GetColumnCount(int categoryNodeCount) {
			int count = SiteMapControlBase.ColumnCountActual;
			if (count >= categoryNodeCount)
				count = categoryNodeCount;
			if (count < 1)
				count = 1;
			return count;
		}
		protected internal override int[] GetPartNumbers() {
			int[] ret = null;
			if ((SiteMapControlBase.Categorized) &&
				(SiteMapControlBase.ColumnCount) == 0) {
				int colCount = SiteMapControlBase.GetMaximumColumnCount();
				ret = new int[colCount];
				for (int i = 0; i < colCount; i++)
					ret[i] = -1;
			}
			else
				ret = base.GetPartNumbers();
			return ret;
		}
		protected override bool IsFirstInColumn(SiteMapNode node) {
			if (SiteMapControlBase.Categorized) {
				SiteMapNode parentNode = SiteMapControlBase.GetParentNode(node);
				if ((parentNode != null) && (SiteMapControlBase.IsRootNode(parentNode))) {
					SiteMapNode previousSibling = SiteMapControlBase.GetPreviousSibling(node);
					return (previousSibling == null) ||
						(GetColumnIndex(node) != GetColumnIndex(previousSibling));
				}
			}
			else {
				if ((SiteMapControlBase.RepeatDirection == RepeatDirection.Horizontal) &&
					(SiteMapControlBase.IsRootNode(node)))
					return true;
			}
			return base.IsFirstInColumn(node);
		}
		protected internal override bool IsLastNodeInColumn(SiteMapNode node) {
			SiteMapNode nextNode = SiteMapControlBase.GetNextNode(node);
			if (SiteMapControlBase.RepeatDirection == RepeatDirection.Horizontal) {
				if (nextNode == null)
					return true;
				else {
					SiteMapNode curCategoryNode = GetCategoryNode(node);
					SiteMapNode nextCategoryNode = GetCategoryNode(nextNode);
					return nextCategoryNode != curCategoryNode;
				}
			}
			else { 
				if (nextNode == null)
					return true;
				else {
					if (SiteMapControlBase.IsRootNode(nextNode))
						return true;
					else {
						SiteMapNode curCategoryNode = GetCategoryNode(node);
						SiteMapNode nextCategoryNode = GetCategoryNode(nextNode);
						return GetColumnIndex(curCategoryNode) != GetColumnIndex(nextCategoryNode);
					}
				}
			}
		}
		protected override void PrepareControlHierarchy() {
			PrepareMainTable();
			PrepareCategoryRows();
		}
		protected SiteMapControlCategoryInfo CreateCategory(Table table, int categoryNodeIndex,
			List<SiteMapNodeCollection> nodesInRows, bool isLastCategory) {
			bool categoryHasRows = nodesInRows.Count > 0 && nodesInRows[0].Count > 0;
			SiteMapControlCategoryInfo categoryInfo = new SiteMapControlCategoryInfo();
			categoryInfo.Rows = new List<SiteMapControlRowInfo>();
			if (SiteMapControlBase.Categorized) {
				TableRow headerRow = RenderUtils.CreateTableRow();
				table.Rows.Add(headerRow);
				ITemplate nodeTemplate = SiteMapControlBase.GetNodeTemplate(RootNodes[categoryNodeIndex]);
				categoryInfo.HeaderCell = RenderUtils.CreateTableCell();
				headerRow.Cells.Add(categoryInfo.HeaderCell);
				if (nodeTemplate != null) {
					NodeTemplateContainer container = new NodeTemplateContainer(RootNodes[categoryNodeIndex]);
					nodeTemplate.InstantiateIn(container);
					container.AddToHierarchy(categoryInfo.HeaderCell, SiteMapControlBase.GetNodeTemplateContainerID(RootNodes[categoryNodeIndex]));
				}
				else {
					categoryInfo.HeaderContentControl = CreateNodeContentControl(RootNodes[categoryNodeIndex]);
					categoryInfo.HeaderCell.Controls.Add(categoryInfo.HeaderContentControl);
				}
				bool needCreateHeaderSeparator = false;
				if (categoryHasRows) {
					if (SiteMapControlBase.GetChildNodesPaddings(0).GetPaddingTop() != 0)
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
			if ((SiteMapControlBase.Categorized) &&
					(SiteMapControlBase.GetNodeSpacing(0) != 0) && (!isLastCategory)) {
				TableRow spacingRow = RenderUtils.CreateTableRow();
				table.Rows.Add(spacingRow);
				categoryInfo.SpacingCell = RenderUtils.CreateIndentCell();
				spacingRow.Cells.Add(categoryInfo.SpacingCell);
			}
			if ((SiteMapControlBase.Categorized) && (categoryHasRows) &&
					(SiteMapControlBase.GetChildNodesPaddings(0).GetPaddingBottom() != 0)) {
				TableRow childNodePaddingBottomRow = RenderUtils.CreateTableRow();
				table.Rows.Add(childNodePaddingBottomRow);
				categoryInfo.ChildNodePaddingBottomCell = RenderUtils.CreateIndentCell();
				childNodePaddingBottomRow.Cells.Add(categoryInfo.ChildNodePaddingBottomCell);
			}
			return categoryInfo;
		}
		protected NodeContentControl CreateNodeContentControl(SiteMapNode categoryNode) {
			return new NodeContentControl(this, categoryNode);
		}
		protected SiteMapControlRowInfo CreateRow(Table table, SiteMapNodeCollection nodes, SiteMapNode rootRowNode, bool isLastRow) {
			SiteMapControlRowInfo rowInfo = new SiteMapControlRowInfo();
			TableRow row = RenderUtils.CreateTableRow();
			table.Rows.Add(row);
			rowInfo.ColumnsContentInfo = CreateColumns(row, nodes);
			if (!isLastRow && (GetRowSpacing(SiteMapControlBase.Categorized) != 0)) {
				TableRow spacingRow = RenderUtils.CreateTableRow();
				table.Rows.Add(spacingRow);
				rowInfo.SpacingCell = RenderUtils.CreateIndentCell();
				spacingRow.Cells.Add(rowInfo.SpacingCell);
			}
			return rowInfo;
		}
		protected void PrepareCategoryRows() {
			foreach(SiteMapControlCategoryInfo info in fCategoriesInfo) {
				if (info.HeaderCell != null) {
					RenderUtils.SetPaddings(info.HeaderCell,
						SiteMapControlBase.GetNodePaddings(RootNodes[fCategoriesInfo.IndexOf(info)]));
					SiteMapControlBase.GetNodeStyle(RootNodes[fCategoriesInfo.IndexOf(info)]).AssignToControl(info.HeaderCell, AttributesRange.All);
					info.HeaderCell.ColumnSpan = GetMaximumTDCountInTable();
				}
				if (info.ChildNodePaddingTopCell != null)
					RenderUtils.PrepareIndentCell(info.ChildNodePaddingTopCell, 0, SiteMapControlBase.GetChildNodesPaddings(0).GetPaddingTop());
				if (info.ChildNodePaddingBottomCell != null)
					RenderUtils.PrepareIndentCell(info.ChildNodePaddingBottomCell, 0, SiteMapControlBase.GetChildNodesPaddings(0).GetPaddingBottom());
				if (info.SpacingCell != null)
					RenderUtils.PrepareIndentCell(info.SpacingCell, 0, SiteMapControlBase.GetNodeSpacing(0));
				foreach(SiteMapControlRowInfo rowInfo in info.Rows) {
					if (rowInfo.ColumnsContentInfo != null)
						PrepareColumns(rowInfo.ColumnsContentInfo, info.Rows.IndexOf(rowInfo));
					if (rowInfo.SpacingCell != null)
						RenderUtils.PrepareIndentCell(rowInfo.SpacingCell, 0, GetRowSpacing(SiteMapControlBase.Categorized));
				}
			}
		}
		private int GetMaximumTDCountInTable() {
			int max = 1;
			SiteMapControlRowInfo rowWithMaxCells = null;
			foreach(SiteMapControlCategoryInfo info in fCategoriesInfo) {
				List<SiteMapControlRowInfo> rowsInfo = info.Rows;
				foreach(SiteMapControlRowInfo rowContentInfo in rowsInfo) {
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
			if (SiteMapControlBase.Categorized) {
				for (int i = 0; i < RootNodes.Count; i++)
					if (SiteMapControlBase.HasChildNodes(RootNodes[i]))
						ret.Add(RootNodes[i].ChildNodes.Count);
					else
						ret.Add(0);
			}
			else
				ret.Add(RootNodes.Count);
			return ret.ToArray();
		}
		private Unit GetRowSpacing(bool isCategorize) {
			return isCategorize ? SiteMapControlBase.GetNodeSpacing(1) :
				SiteMapControlBase.GetNodeSpacing(0);
		}
	}
	public class FlowLayoutItemInfo {
		public LiteralControl SeparatorTextControl = null;
	}
	public class InLineNodeContentControl : NodeControlBase {
		private WebControl fSpanContentControl = null;
		private HyperLink fNodeTextControl = null;
		public InLineNodeContentControl(SMCTreeViewControl treeViewControl, SiteMapNode node)
			: base(treeViewControl, node) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fNodeTextControl = null;
			fSpanContentControl = null;
		}
		protected override void CreateControlHierarchy() {
			fSpanContentControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
			Controls.Add(fSpanContentControl);
			ITemplate nodeTextTemplate = SiteMapControlBase.GetNodeTextTemplate(Node);
			if (nodeTextTemplate != null)
				CreateNodeTextTemplate(fSpanContentControl, nodeTextTemplate, Node);
			else {
				fNodeTextControl = CreateNodeTextControl();
				fSpanContentControl.Controls.Add(fNodeTextControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			SiteMapControlBase.GetNodeStyle(Node).AssignToControl(fSpanContentControl, AttributesRange.Font);
			if (fNodeTextControl != null)
				PrepareNodeTextControl(fNodeTextControl);
			if(IsRightToLeft && fSpanContentControl != null)
				fSpanContentControl.Attributes["dir"] = "rtl";
		}
	}
	public class FlowLayoutNodeControl : NodeControlBase {
		private List<FlowLayoutItemInfo> fFlowLayoutItemsInfo = null;
		private SiteMapNodeCollection fNodes = null;
		protected WebControl fDiv = null;
		private LiteralControl fEndDirItemLiteralControl = null;
		protected ASPxSiteMapControl SiteMapControl {
			get { return SiteMapControlBase as ASPxSiteMapControl; }
		}
		public FlowLayoutNodeControl(SMCTreeViewControl treeViewControl, SiteMapNode parentNode)
			: base(treeViewControl, parentNode) {
			if (parentNode == null) {
				fNodes = new SiteMapNodeCollection();
				fNodes.AddRange(SiteMapControlBase.RootNodes);
			}
			else
				fNodes = parentNode.ChildNodes;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fDiv = null;
			fEndDirItemLiteralControl = null;
			fFlowLayoutItemsInfo = null;
		}
		protected override void CreateControlHierarchy() {
			fDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(fDiv);
			CreateChildNodeControls(fDiv);
		}
		protected virtual void CreateChildNodeControls(WebControl parentControl) {
			fFlowLayoutItemsInfo = new List<FlowLayoutItemInfo>();
			int maxDisplayItems = SiteMapControl.GetFlowLayoutMaximumDisplayItems();
			int nodeCount = maxDisplayItems > fNodes.Count ? fNodes.Count : maxDisplayItems;
			for (int i = 0; i < nodeCount; i++) {
				parentControl.Controls.Add(TreeViewControl.CreateFlowLayoutItemNode(fNodes[i]));
				if ((i != nodeCount - 1) || (fNodes.Count > nodeCount)) {
					FlowLayoutItemInfo flowLayoutItemInfo = new FlowLayoutItemInfo();
					flowLayoutItemInfo.SeparatorTextControl = RenderUtils.CreateLiteralControl();
					parentControl.Controls.Add(flowLayoutItemInfo.SeparatorTextControl);
					fFlowLayoutItemsInfo.Add(flowLayoutItemInfo);
				}
			}
			if (fNodes.Count > nodeCount) {
				fEndDirItemLiteralControl = RenderUtils.CreateLiteralControl();
				parentControl.Controls.Add(fEndDirItemLiteralControl);
			}
		}
		protected override void PrepareControlHierarchy() {
			if (Node != null) 
				RenderUtils.SetPaddings(fDiv, TreeViewControl.GetNodeMargins(Node, this)); 
			RenderUtils.SetLineHeight(fDiv, SiteMapControl.GetFlowLayoutTextLineHeight());
			AppearanceStyleBase flowLayoutItemNodeStyle = Node != null ? 
				SiteMapControlBase.GetNodeStyle(SiteMapControlBase.GetNodeLevel(Node.ChildNodes[0])) : 
				SiteMapControlBase.GetNodeStyle(0); 
			flowLayoutItemNodeStyle.AssignToControl(fDiv, AttributesRange.All, true);
			for (int i = 0; i < fFlowLayoutItemsInfo.Count; i++)
				fFlowLayoutItemsInfo[i].SeparatorTextControl.Text = 
					(SiteMapControlBase as ASPxSiteMapControl).GetFlowLayoutItemSeparatorText();
			if (fEndDirItemLiteralControl != null)
				fEndDirItemLiteralControl.Text = SiteMapControl.GetFlowLayoutLastItemText();
		}
	}
	public class SMTreeViewNodeControl : FlowLayoutNodeControl {
		private HyperLink fNodeTextControl = null;
		private WebControl fTemplateDiv = null;
		private WebControl fUlControl = null;
		protected Image fImage = null;
		protected WebControl fNodeDiv = null;
		public SMTreeViewNodeControl(SMCTreeViewControl treeViewControl, SiteMapNode node)
			: base(treeViewControl, node) {
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			fImage = null;
			fNodeDiv = null;
			fUlControl = null;
			fNodeTextControl = null;
			fTemplateDiv = null;
		}
		protected override void CreateControlHierarchy() {
			fDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(fDiv);
			AddControlsToMainDiv(fDiv);
		}
		protected override void PrepareControlHierarchy() {
			RenderUtils.SetPaddings(fDiv, TreeViewControl.GetNodeMargins(Node, this)); 
			if (fTemplateDiv != null)
				PrepareTemplateDiv();
			else
				PrepareNodeDiv();
		}
		protected override void CreateChildNodeControls(WebControl parentControl) {
			if (SiteMapControlBase.HasChildNodes(Node)) {
				if (!SiteMapControlBase.IsShowChildNodeAsFlowLayoutItem(Node)) {
					foreach (SiteMapNode childNode in Node.ChildNodes)
						parentControl.Controls.Add(TreeViewControl.CreateNode(childNode));
				}
				else
					parentControl.Controls.Add(TreeViewControl.CreateFlowLayoutNode(Node));
			}
		}
		protected virtual void AddControlsToMainDiv(WebControl mainDiv) {
			ITemplate template = SiteMapControlBase.GetNodeTemplate(Node);
			if (template != null) {
				NodeTemplateContainer container = new NodeTemplateContainer(Node);
				template.InstantiateIn(container);
				fTemplateDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				mainDiv.Controls.Add(fTemplateDiv);
				container.AddToHierarchy(fTemplateDiv, SiteMapControlBase.GetNodeTemplateContainerID(Node));
			}
			else
				CreateNodeDiv(mainDiv);
			CreateChildNodeControls(mainDiv);
		}
		protected virtual void CreateNodeDiv(WebControl parent) {
			fNodeDiv = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			parent.Controls.Add(fNodeDiv);
			if (SiteMapControlBase.IsBulletMode(Node))
				CreateUL(fNodeDiv);
			else {
				if (!SiteMapControlBase.GetNodeImage(Node).IsEmpty) {
					fImage = RenderUtils.CreateImage();
					fNodeDiv.Controls.Add(fImage);
				}
				ITemplate nodeTextTemplate = SiteMapControlBase.GetNodeTextTemplate(Node);
				if (nodeTextTemplate != null)
					CreateNodeTextTemplate(fNodeDiv, nodeTextTemplate, Node);				
				else {
					fNodeTextControl = CreateNodeTextControl();
					fNodeDiv.Controls.Add(fNodeTextControl);
				}
			}
		}
		protected virtual void PrepareNodeDiv() {
			SiteMapControlBase.GetNodeStyle(Node).AssignToControl(fNodeDiv, AttributesRange.All);
			RenderUtils.SetPaddings(fNodeDiv, SiteMapControlBase.GetNodePaddings(Node)); 
			if (fUlControl != null)
				PrepareUlControl();
			else {
				if (fImage != null) {
					SiteMapControlBase.GetNodeImage(Node).AssignToControl(fImage, DesignMode);
					if (SiteMapControlBase.GetNodeStyle(Node).VerticalAlign != VerticalAlign.NotSet)
						RenderUtils.SetVerticalAlign(fImage, SiteMapControlBase.GetNodeStyle(Node).VerticalAlign);
					else
						fImage.ImageAlign = ImageAlign.Middle;
					if(IsRightToLeft) {
						RenderUtils.SetHorizontalMargins(fImage, SiteMapControlBase.GetImageSpacing(Node), 0);
					} else {
						RenderUtils.SetHorizontalMargins(fImage, 0, SiteMapControlBase.GetImageSpacing(Node));
					}
				}
			}
			if (fNodeTextControl != null)
				PrepareNodeTextControl(fNodeTextControl);
		}
		protected void CreateUL(WebControl parent) {
			fUlControl = RenderUtils.CreateList(ListType.Bulleted);
			parent.Controls.Add(fUlControl);
			WebControl liControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Li);
			fUlControl.Controls.Add(liControl);
			ITemplate nodeTextTemplate = SiteMapControlBase.GetNodeTextTemplate(Node);
			if (nodeTextTemplate != null)
				CreateNodeTextTemplate(liControl, nodeTextTemplate, Node);
			else {
				fNodeTextControl = CreateNodeTextControl();
				liControl.Controls.Add(fNodeTextControl);
			}
		}
		protected void PrepareTemplateDiv() {
			SiteMapControlBase.GetNodeStyle(Node).AssignToControl(fTemplateDiv, AttributesRange.All);
		}
		protected void PrepareUlControl() {
			RenderUtils.SetStyleStringAttribute(fUlControl, "list-style-type", SiteMapControlBase.GetNodeBulletStyle(Node).ToString().ToLower());
			Unit bulletMargin = SiteMapControlBase.GetBulletIndent(Node);
			RenderUtils.SetMargins(fUlControl, IsRightToLeft ? 0 : bulletMargin, 0, IsRightToLeft ? bulletMargin : 0, 0);
		}
	}
	public class SMTreeViewWrapNodeControl : SMTreeViewNodeControl {
		private Table fImageTextTable = null;
		private TableCell fImageCell = null;
		private TableCell fTextCell = null;
		private HyperLink fNodeTextControl = null;
		public SMTreeViewWrapNodeControl(SMCTreeViewControl treeViewControl, SiteMapNode node)
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
			fImageCell = RenderUtils.CreateTableCell();
			row.Cells.Add(fImageCell);
			fImage = RenderUtils.CreateImage();
			fImageCell.Controls.Add(fImage);
			fTextCell = RenderUtils.CreateTableCell();
			row.Cells.Add(fTextCell);
			ITemplate nodeTextTemplate = SiteMapControl.GetNodeTextTemplate(Node);
			if (nodeTextTemplate != null)
				CreateNodeTextTemplate(fTextCell, nodeTextTemplate, Node);			
			else {
				fNodeTextControl = CreateNodeTextControl();
				fTextCell.Controls.Add(fNodeTextControl);
			}
		}
		protected override void PrepareNodeDiv() {
			SiteMapControlBase.GetNodeStyle(Node).AssignToControl(fNodeDiv, AttributesRange.Common);
			RenderUtils.SetPaddings(fNodeDiv, SiteMapControlBase.GetNodePaddings(Node)); 
			SiteMapControlBase.GetNodeStyle(Node).AssignToControl(fImageTextTable, AttributesRange.Font);
			SiteMapControlBase.GetNodeStyle(Node).AssignToControl(fImageCell, AttributesRange.Cell);
			SiteMapControlBase.GetNodeImage(Node).AssignToControl(fImage, DesignMode);
			fImage.ImageAlign = ImageAlign.Middle;
			if(IsRightToLeft) {
				RenderUtils.SetHorizontalMargins(fImage, SiteMapControlBase.GetImageSpacing(Node), Unit.Empty);
			} else {
				RenderUtils.SetHorizontalMargins(fImage, Unit.Empty, SiteMapControlBase.GetImageSpacing(Node));
			}
			SiteMapControlBase.GetNodeStyle(Node).AssignToControl(fTextCell, AttributesRange.Cell | AttributesRange.Font);
			if (fNodeTextControl != null)
				PrepareNodeTextControl(fNodeTextControl);
		}
	}
}
