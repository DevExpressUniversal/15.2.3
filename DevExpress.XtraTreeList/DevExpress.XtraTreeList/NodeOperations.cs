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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraTreeList.Printing;
using DevExpress.XtraPrinting;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Paint;
using DevExpress.XtraTreeList.Data;
using DevExpress.XtraTreeList.Internal;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace DevExpress.XtraTreeList.Nodes.Operations {
	public delegate void TreeListOperationDelegate(TreeListNode node);
	public class TreeListNodesIterator {
		protected TreeList fTreeList;
		internal bool allowHasChildren = true;
		internal TreeListNodesIterator(TreeList treeList, bool inner) : this(treeList) { }
		protected TreeListNodesIterator(TreeList treeList) {
			if(treeList == null) throw new NullReferenceException("Can't create TreeListNodesIterator instance.");
			this.fTreeList = treeList;
		}
		public virtual void DoOperation(TreeListOperationDelegate operation) {
			DoOperation(new InternalTreeListOperation(operation));
		}
		public virtual void DoLocalOperation(TreeListOperationDelegate operation, TreeListNodes nodes) {
			DoLocalOperation(new InternalTreeListOperation(operation), nodes);
		}
		public virtual void DoOperation(TreeListOperation operation) {
			DoLocalOperation(operation, fTreeList.Nodes);
		}
		public virtual void DoLocalOperation(TreeListOperation operation, TreeListNodes nodes) {
			if(nodes == null) return;
			if(operation == null) return;
			if(nodes.TreeList != fTreeList) return;
			try {
				if(operation.NeedsFullIteration)
					VisitAllNodes(nodes, operation);
				else
					VisitParentNodes(nodes, operation);
			}
			finally {
				operation.FinalizeOperation();
			}
		}
		bool HasChildren(TreeListNode node) {
			if(allowHasChildren) return node.HasChildren;
			return node.Nodes.Count > 0;
		}
		protected virtual void VisitAllNodes(TreeListNodes nodes, TreeListOperation operation) {
			int nodesCount = nodes.Count;
			for(int index = 0; index < nodesCount; index++) {
				TreeListNode node = nodes[index];
				if(!operation.CanContinueIteration(node)) return;
				operation.Execute(node);
				if(HasChildren(node))
					if(operation.NeedsVisitChildren(node))
						VisitAllNodes(node.Nodes, operation);
			}
		}
		protected virtual void VisitParentNodes(TreeListNodes nodes, TreeListOperation operation) {
			int nodesCount = nodes.Count;
			for(int index = 0; index < nodesCount; index++) {
				TreeListNode node = nodes[index];
				if(!operation.CanContinueIteration(node)) return;
				if(HasChildren(node)) { 
					operation.Execute(node);
					if(operation.NeedsVisitChildren(node)) {
						VisitParentNodes(node.Nodes, operation);
					}
				}
			}
		}
		public virtual void DoVisibleNodesOperation(TreeListVisibleNodeOperation operation, TreeListNode visibleNodeFrom, TreeListNode visibleNodeTo) {
			if(operation == null) return;
			if(visibleNodeFrom == null || visibleNodeFrom.TreeList != fTreeList) return;
			if(visibleNodeTo == null || visibleNodeTo.TreeList != fTreeList) return;
			if(!TreeList.IsNodeVisible(visibleNodeFrom)) return;
			if(!TreeList.IsNodeVisible(visibleNodeTo)) return;
			if(visibleNodeFrom == visibleNodeTo) {
				try { operation.Execute(visibleNodeFrom); }
				finally { operation.FinalizeOperation(); }
				return;
			}
			bool forward = false;
			if(visibleNodeFrom.RootNode != visibleNodeTo.RootNode) {
				if(fTreeList.Nodes.IndexOf(visibleNodeFrom.RootNode) < fTreeList.Nodes.IndexOf(visibleNodeTo.RootNode))
					forward = true;
			}
			else {
				if(visibleNodeFrom.RootNode == visibleNodeFrom) forward = true;
				else if(visibleNodeTo.RootNode == visibleNodeTo) forward = false;
				else {
					int indexFrom = fTreeList.GetVisibleIndexByNode(visibleNodeFrom);
					int indexTo = fTreeList.GetVisibleIndexByNode(visibleNodeTo);
					forward = (indexFrom < indexTo);
				}
			}
			try {
				if(forward) VisitForwardVisibleNodes(operation, visibleNodeFrom, visibleNodeTo);
				else VisitBackwardVisibleNodes(operation, visibleNodeTo, visibleNodeFrom);
			}
			finally {
				operation.FinalizeOperation();
			}
		}
		protected virtual void VisitForwardVisibleNodes(TreeListVisibleNodeOperation operation, TreeListNode upNode, TreeListNode downNode) {
			operation.Execute(upNode);
			while(true) {
				upNode = GetNextVisible(upNode);
				if(upNode == null) break;
				if(!operation.CanContinueIteration(upNode)) return;
				operation.Execute(upNode);
				if(upNode == downNode) break;
			}
		}
		protected virtual void VisitBackwardVisibleNodes(TreeListVisibleNodeOperation operation, TreeListNode upNode, TreeListNode downNode) {
			operation.Execute(downNode);
			while(true) {
				downNode = GetPrevVisible(downNode);
				if(downNode == null) break;
				if(!operation.CanContinueIteration(downNode)) return;
				operation.Execute(downNode);
				if(upNode == downNode) break;
			}
		}
		static TreeListNode GetNearestVisibleNode(TreeListNode node) {
			if(TreeList.IsNodeVisible(node)) return node;
			TreeListNode result = node;
			TreeListNode parent = result.ParentNode;
			while(parent != null) {
				if(!parent.Expanded) {
					result = parent;
				}
				parent = parent.ParentNode;
			}
			return result;
		}
		protected static TreeListNode GetNextVisibleCore(TreeListNode node) {
			if(node == null) return null;
			node = GetNearestVisibleNode(node);
			if(node.Expanded && node.Nodes.Count > 0) return node.Nodes[0];
			TreeList treeList = node.TreeList;
			if(node.ParentNode != null) {
				TreeListNodes owner = node.owner;
				while(node == owner.LastNode) {
					if(owner == treeList.Nodes) return null;
					if(node.ParentNode == null) return null;
					node = node.ParentNode;
					owner = node.owner;
				}
				int index = owner.IndexOf(node);
				return owner[index + 1];
			}
			else {
				if(treeList.Nodes.LastNode == node) return null;
				else {
					int index = treeList.Nodes.IndexOf(node);
					return treeList.Nodes[index + 1];
				}
			}
		}
		public static TreeListNode GetNextVisible(TreeListNode node) {
			TreeListNode nextNode = GetNextVisibleCore(node);
			while(nextNode != null && !TreeListFilterHelper.IsNodeVisible(nextNode)) {
				nextNode = GetNextVisibleCore(nextNode);
			}
			return nextNode;
		}
		public static TreeListNode GetPrevVisible(TreeListNode node) {
			if(TreeListAutoFilterNode.IsAutoFilterNode(node)) return null;
			TreeListNode prevNode = GetPrevVisibleCore(node);
			while(prevNode != null && !TreeListFilterHelper.IsNodeVisible(prevNode)) {
				prevNode = GetPrevVisibleCore(prevNode);
			}
			return prevNode;
		}
		protected static TreeListNode GetPrevVisibleCore(TreeListNode node) {
			if(node == null) return null;
			node = GetNearestVisibleNode(node);
			if(node == node.TreeList.Nodes.FirstNode) return null;
			TreeListNodes owner = node.owner;
			if(node != owner.FirstNode) {
				int index = owner.IndexOf(node);
				node = owner[index - 1];
				while(node != null && node.Expanded && node.HasChildren) {
					node = node.Nodes.LastNode;
				}
				return node;
			}
			return node.ParentNode;
		}
		internal static TreeListNode GetNextVisibleParent(TreeListNode node) {
			TreeListNode newNode = TreeListNodesIterator.GetNextVisible(node);
			while(newNode != null) {
				if(!newNode.HasAsParent(node))
					return newNode;
				newNode = TreeListNodesIterator.GetNextVisible(newNode);
			}
			return null;
		}
		#region Internal
		internal class InternalTreeListOperation : TreeListOperation {
			TreeListOperationDelegate operation;
			public InternalTreeListOperation(TreeListOperationDelegate operation)
				: base() {
				this.operation = operation;
			}
			public override void Execute(TreeListNode node) {
				operation(node);
			}
		}
		#endregion
	}
	public abstract class TreeListOperation {
		public abstract void Execute(TreeListNode node);
		public virtual bool NeedsVisitChildren(TreeListNode node) { return true; }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListOperationNeedsFullIteration")]
#endif
		public virtual bool NeedsFullIteration { get { return true; } }
		public virtual bool CanContinueIteration(TreeListNode node) { return true; }
		public virtual void FinalizeOperation() { }
	}
	public abstract class TreeListVisibleNodeOperation : TreeListOperation {
		sealed public override bool NeedsVisitChildren(TreeListNode node) { return true; }
#if !SL
	[DevExpressXtraTreeListLocalizedDescription("TreeListVisibleNodeOperationNeedsFullIteration")]
#endif
		sealed public override bool NeedsFullIteration { get { return true; } }
	}
	class TreeListOperationCountVisible : TreeListVisibleNodeOperation {
		int fCount = 0;
		public override void Execute(TreeListNode node) {
			fCount++;
		}
		public int Count { get { return fCount; } }
	}
	public class TreeListOperationCount : TreeListOperation {
		protected int fCount = 0;
		public TreeListOperationCount(TreeListNodes rootNodes) {
			fCount = TreeListFilterHelper.GetVisibleNodeCount(rootNodes);
		}
		public override void Execute(TreeListNode node) {
			if(!node.Expanded) return;
			fCount += TreeListFilterHelper.GetVisibleNodeCount(node.Nodes);
		}
		public override bool NeedsVisitChildren(TreeListNode node) { return node.Expanded; }
		public override bool NeedsFullIteration { get { return false; } }
		public int Count { get { return fCount; } }
	}
	public class TreeListOperationVisibleFullCount : TreeListOperationCount {
		public TreeListOperationVisibleFullCount(TreeListNodes rootNodes) : base(rootNodes) { }
		public override void Execute(TreeListNode node) {
			fCount += TreeListFilterHelper.GetVisibleNodeCount(node.Nodes);
		}
		public override bool NeedsVisitChildren(TreeListNode node) { return true; }
		public override bool NeedsFullIteration { get { return true; } }
	}
	public class TreeListOperationFullCount : TreeListOperationCount {
		public TreeListOperationFullCount(TreeListNodes rootNodes)
			: base(rootNodes) {
			fCount = rootNodes.Count;
		}
		public override void Execute(TreeListNode node) { fCount += node.Nodes.Count; }
		public override bool NeedsVisitChildren(TreeListNode node) { return true; }
	}
	public class TreeListOperationCountWithParent : TreeListOperationFullCount {
		public TreeListOperationCountWithParent(TreeListNodes children) : base(children) { this.fCount++; }
	}
	public class TreeListOperationGroupNodesCount : TreeListOperation {
		public int Count;
		public TreeListOperationGroupNodesCount() { Count = 0; }
		public override void Execute(TreeListNode node) { Count++; }
		public override bool NeedsVisitChildren(TreeListNode node) { return node.Expanded; }
		public override bool NeedsFullIteration { get { return false; } }
	}
	public class TreeListOperationCollapseExpand : TreeListOperation {
		bool col_exp;
		public TreeListOperationCollapseExpand(bool expand) {
			this.col_exp = expand;
		}
		public override void Execute(TreeListNode node) { node.Expanded = col_exp; }
		public override bool NeedsFullIteration { get { return false; } }
	}
	public class TreeListOperationExpandAll : TreeListOperationCollapseExpand {
		int maxLevel;
		public TreeListOperationExpandAll(int maxLevel)
			: base(true) {
			this.maxLevel = maxLevel;
		}
		public override bool CanContinueIteration(TreeListNode node) {
			if(maxLevel > -1)
				return node.Level <= maxLevel;
			return base.CanContinueIteration(node);
		}
	}
	public class TreeListOperationPrintEachNode : TreeListOperation {
		TreeList treeList;
		TreeListViewInfo viewInfo;
		TreeListPrinter printer;
		Brush evenTreeLineBrush, oddTreeLineBrush;
		bool printTree;
		bool printImages;
		bool even;
		int topNodeSide;
		AppearanceObject previewStyle;
		Stack lastChildren;
		ArrayList CellWidthes;
		GraphicsCache cache;
		XPaint oldPaint;
		int rowIndex;
		int rowCount;
		public TreeListOperationPrintEachNode(TreeList treeList, TreeListPrinter printer,
			TreeListViewInfo viewInfo, bool printTree, bool printImages) {
			this.treeList = treeList;
			this.printer = printer;
			this.viewInfo = viewInfo;
			this.printTree = printTree;
			this.printImages = printImages;
			this.even = true;
			this.previewStyle = UsePrintStyles ? PrintInfo.PrintAppearance.Preview : viewInfo.PaintAppearance.Preview;
			this.lastChildren = new Stack();
			this.topNodeSide = 0;
			this.rowIndex = 0;
			this.rowCount = treeList.OptionsPrint.PrintAllNodes ? treeList.AllNodesCount : treeList.RowCount;
			this.cache = new GraphicsCache(new DXPaintEventArgs());
			this.printer.IsPrinting = true;
			CreateTreeLineBrushes();
			CreateCellWidthes();
			CreatePrintPaint();
		}
		#region initializtion
		void CreateCellWidthes() {
			CellWidthes = new ArrayList();
			foreach(PrintColumnInfo wi in printer.ColumnPrintInfo) {
				ColumnWidthInfo cellWidth = new ColumnWidthInfo();
				cellWidth.Column = wi.Column;
				cellWidth.Width = wi.Bounds.Width - 2 * (CellInfo.CellTextIndent + 1);
				CellWidthes.Add(cellWidth);
			}
		}
		void CreateTreeLineBrushes() {
			AppearanceObject lineStyle = (UsePrintStyles ? PrintInfo.PrintAppearance.Lines : viewInfo.PaintAppearance.TreeLine);
			Color backColor = ((BrickGraphics)printer.graph).PageBackColor;
			evenTreeLineBrush = viewInfo.RC.GetTreeLineBrush(lineStyle, treeList.TreeLineStyle);
			oddTreeLineBrush = viewInfo.RC.GetTreeLineBrush(lineStyle, treeList.TreeLineStyle);
		}
		void CreatePrintPaint() {
			this.oldPaint = viewInfo.GInfo.Cache.Paint;
			viewInfo.GInfo.Cache.Paint = new XPrintPaint();
		}
		#endregion
		public override bool CanContinueIteration(TreeListNode node) {
			return base.CanContinueIteration(node) && !printer.CancelPending;
		}
		public override void Execute(TreeListNode node) {
			if(!node.Visible) return;
			int leftSide = 0, nodeLevel = node.Level, firstColumnIndent = nodeLevel * viewInfo.RC.LevelWidth, nodeHeight = PrintInfo.RowHeight;
			ArrayList viewInfoList = new ArrayList();
			if(treeList.OptionsPrint.AutoRowHeight && CellWidthes.Count > 0) {
				ColumnWidthInfo cw = (ColumnWidthInfo)CellWidthes[0];
				cw.Width = printer.ColumnPrintInfo[0].Bounds.Width - firstColumnIndent - (printImages ? GetImagesTotalWidth() : 0);
				nodeHeight = treeList.InternalCalcNodeHeight(new RowInfo(viewInfo, node), nodeHeight, CellWidthes, false, out viewInfoList, even, false);
			}
			int totalHeight = nodeHeight * treeList.ViewInfo.RowLineCount;
			AppearanceObject style = GetCellAppearance(FirstColumnCellApperance, null);
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(CreateTransparentAppearance(), BorderSide.None, Color.Transparent, 0));
			int previewHeight = 0;
			if(treeList.OptionsPrint.PrintPreview)
				DrawPreviewBrick(node, leftSide, firstColumnIndent, nodeHeight, ref totalHeight, ref previewHeight);
			ArrayList indents;
			IBrick indentBrick = CreateIndentBrick(node, leftSide, nodeLevel, totalHeight, style, out indents, ref firstColumnIndent);
			printer.graph.DrawBrick(indentBrick, new Rectangle(leftSide, topNodeSide, firstColumnIndent, totalHeight + previewHeight));
			RowInfo ri = PrepareRowInfo(node, firstColumnIndent, nodeHeight, viewInfoList, rowIndex);
			foreach(CellInfo cell in ri.Cells) {
				IBrick cellBrick = cell.Column == null ? CreateEmptyCellBrick(cell, node) : CreateCellBrick(cell, node);
				printer.graph.DrawBrick(cellBrick, cell.Bounds);
			}
			even = !even;
			leftSide = 0;
			topNodeSide += totalHeight;
			if(treeList.OptionsPrint.PrintRowFooterSummary) {
				if(CanPushNode(node))
					lastChildren.Push(node.Nodes.LastNode);
				else if(lastChildren.Count > 0 && node == lastChildren.Peek())
					DrawRowFooterBricks(indents);
			}
			printer.OnPrintingProgress(Math.Min(100, (int)(((rowIndex + 1) / (float)rowCount) * 100f)));
			rowIndex++;
		}
		protected virtual void DrawPreviewBrick(TreeListNode node, int leftSide, int firstColumnIndent, int nodeHeight, ref int totalHeight, ref int previewHeight) {
			string previewText = treeList.InternalGetPrintPreviewText(node);
			if(previewText != string.Empty) {
				int imagesWidth = GetImagesTotalWidth();
				int previewWidth = printer.ColumnTotalWidth - firstColumnIndent - imagesWidth;
				previewHeight = viewInfo.GetPreviewRowHeight(previewText, previewWidth, previewStyle, node);
				Rectangle previewRect = new Rectangle(leftSide + firstColumnIndent + imagesWidth,
					topNodeSide + totalHeight, previewWidth, previewHeight);
				printer.graph.DrawBrick(CreatePreviewBrick(previewRect, previewText), previewRect);
				totalHeight += previewHeight;
			}
		}
		protected IBrick CreateIndentBrick(TreeListNode node, int leftSide, int nodeLevel, int totalHeight, AppearanceObject style, out ArrayList indents, ref int firstColumnIndent) {
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(CreateTransparentAppearance(), BorderSide.None, Color.Transparent, 0));
			indents = new ArrayList();
			Rectangle buttonRect = Rectangle.Empty;
			IPanelBrick indentBrick = CreatePanelBrick();
			indentBrick.BorderWidth = 0;
			IList indentBricks = indentBrick.Bricks;
			if(treeList.OptionsPrint.PrintFilledTreeIndent)
				indentBrick.BackColor = style.BackColor;
			if(printTree && nodeLevel > 0) {
				Rectangle indentRect = new Rectangle(leftSide, topNodeSide, firstColumnIndent, totalHeight);
				TreeListViewInfo.CalcNodeIndents(node, treeList.Nodes, indents, nodeLevel, 1);
				if(treeList.OptionsPrint.PrintTreeButtons && node.HasChildren) {
					Size buttonSize = GetExpandButtonSize();
					Rectangle rb = new Rectangle(leftSide + (nodeLevel - 1) * viewInfo.RC.LevelWidth +
						(viewInfo.RC.LevelWidth - buttonSize.Width) / 2 + 1,
						topNodeSide + (totalHeight - buttonSize.Height) / 2 + 1,
						(buttonSize.Width % 2 == 0) ? buttonSize.Width : buttonSize.Width - 1,
						(buttonSize.Height % 2 == 0) ? buttonSize.Height : buttonSize.Height - 1);
					buttonRect = rb;
				}
				CreateIndentButtonBrick(indentBricks, node, nodeLevel, totalHeight, indents, indentRect, buttonRect, style.BackColor);
			}
			if(printImages) {
				int selectIndex = -1, stateIndex = -1;
				if(printImages) {
					selectIndex = treeList.InternalGetSelectImage(node, false);
					stateIndex = treeList.InternalGetStateImage(node);
				}
				int imagesTotalWidth;
				CreateImagesBricks(indentBricks, selectIndex, stateIndex, style.BackColor, new Point(leftSide + firstColumnIndent, topNodeSide), totalHeight, out imagesTotalWidth);
				firstColumnIndent += imagesTotalWidth;
			}
			return indentBrick;
		}
		protected virtual Size GetExpandButtonSize() {
			return new Size(11, 11);
		}
		protected virtual IBrick CreatePreviewBrick(Rectangle rect, string text) {
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(previewStyle, BorderSide.All, BorderColor, 1));
			ITextBrick brick = (ITextBrick)printer.PS.CreateTextBrick();
			StringFormat format = previewStyle.GetStringFormat(TreeListViewInfo.GetPreviewTextOptions());
			brick.StringFormat = new BrickStringFormat(format);
			brick.Rect = rect;
			brick.Text = text;
			return brick;
		}
		protected IPanelBrick CreatePanelBrick() {
			return (IPanelBrick)printer.PS.CreatePanelBrick();
		}
		RowInfo PrepareRowInfo(TreeListNode node, int firstColumnIndent, int nodeHeight, ArrayList viewInfoList, int rowIndex) {
			RowInfo ri = CalcRowInfo(node);
			int width;
			int i = 0;
			foreach(PrintColumnInfo wi in printer.ColumnPrintInfo) {
				TreeListColumn col = wi.Column;
				ColumnInfo ci = viewInfo.CreateColumnInfo(col);
				width = wi.Bounds.Width;
				bool isFirst = printer.ColumnPrintInfo.IndexOf(wi) == 0 || wi.Bounds.Left == 0;
				if(isFirst) width -= firstColumnIndent;
				if(col == null) {
					EmptyCellInfo info = new EmptyCellInfo(ci, ri);
					Rectangle r = new Rectangle(wi.Bounds.Left + (isFirst ? firstColumnIndent : 0), topNodeSide + wi.RowIndex * nodeHeight, width, nodeHeight * wi.RowCount);
					info.SetBounds(Rectangle.Inflate(r, -CellInfo.CellTextIndent, -CellInfo.CellTextIndent));
					ri.Cells.Add(info);
				}
				else {
					CellInfo cell = null;
					if(viewInfoList.Count > 0) {
						CellInfo cacheCell = ((CellInfo)viewInfoList[i]);
						cell = new CellInfo(ci, ri, cacheCell.EditorViewInfo);
					}
					else
						cell = viewInfo.CreateCellInfo(ci, ri);
					Rectangle r = new Rectangle(wi.Bounds.Left + (isFirst ? firstColumnIndent : 0), topNodeSide + wi.RowIndex * nodeHeight, width, nodeHeight * wi.RowCount);
					cell.EditorViewInfo.DetailLevel = XtraEditors.Controls.DetailLevel.Minimum;
					cell.EditorViewInfo.Bounds = Rectangle.Inflate(r, -CellInfo.CellTextIndent, -CellInfo.CellTextIndent);
					viewInfo.UpdateCell(cell, col, node);
					cell.EditorViewInfo.IsPrinting = true;
					cell.EditorViewInfo.FillBackground = true;
					cell.CheckHAlignment();
					ri.Cells.Add(cell);
					i++;
				}
			}
			ri.VisibleIndex = rowIndex;
			viewInfo.UpdateRowCondition(ri);
			return ri;
		}
		AppearanceObject CreateTransparentAppearance() {
			AppearanceObject app = new AppearanceObject();
			app.BackColor = Color.Transparent;
			return app;
		}
		protected virtual void CreateIndentButtonBrick(IList bricks, TreeListNode node, int ind, int totalHeight, ArrayList indents, Rectangle indentRect, Rectangle buttonRect, Color indentBackColor) {
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(CreateTransparentAppearance(), BorderSide.None, Color.Transparent, 0));
			IVisualBrick brick;
			bool hasVisibleChildren = TreeListFilterHelper.HasVisibleChildren(node);
			if((!buttonRect.IsEmpty && hasVisibleChildren) || (TreeLineBrush != null && indents.Count > 0)) {
				IImageBrick imageBrick = printer.PS.CreateImageBrick();
				brick = imageBrick;
				Image image = new Bitmap(ind * viewInfo.RC.LevelWidth, totalHeight);
				Graphics graphics = Graphics.FromImage(image);
				if(!indentRect.IsEmpty) {
					RectangleF ef1 = new RectangleF(0, 0, (float)ind * viewInfo.RC.LevelWidth, (float)totalHeight);
					ef1.Width = ef1.Width / ((float)indents.Count);
					printer.treePainter.DrawIndents(graphics, indents, ef1, TreeLineBrush);
				}
				if(!buttonRect.IsEmpty && hasVisibleChildren) {
					GraphicsCache cache = new GraphicsCache(graphics);
					bool expanded = node.Expanded;
					if(treeList.OptionsPrint.PrintAllNodes && !expanded) expanded = true;
					printer.treePainter.DrawButton(cache, expanded, new RectangleF((float)(buttonRect.X - indentRect.X), (float)(buttonRect.Y - indentRect.Y), (float)buttonRect.Width, (float)buttonRect.Height));
					RectangleF rect = GraphicsUnitConverter.Convert(new Rectangle(buttonRect.X - indentRect.X, buttonRect.Y - indentRect.Y, buttonRect.Width, buttonRect.Height), GraphicsUnit.Pixel, GraphicsUnit.Document);
					brick.Value = new IndentButtonInfo(rect, node);
					cache.Dispose();
				}
				imageBrick.Image = image;
				graphics.Dispose();
			}
			else {
				brick = printer.PS.CreateTextBrick();
			}
			if(treeList.OptionsPrint.PrintFilledTreeIndent)
				brick.BackColor = indentBackColor;
			brick.BorderWidth = 0;
			brick.Rect = new RectangleF(indentRect.X, 0, indentRect.Width, indentRect.Height);
			bricks.Add(brick);
		}
		RowInfo CalcRowInfo(TreeListNode node) {
			RowInfo result = new RowInfo(viewInfo, node);
			result.RowState = (even ? TreeNodeCellState.Even : TreeNodeCellState.Odd);
			return result;
		}
		public override void FinalizeOperation() {
			CreateFooterBricks();
			this.printer.IsPrinting = false;
			this.cache.Dispose();
			XPaint paint = viewInfo.GInfo.Cache.Paint;
			viewInfo.GInfo.Cache.Paint = oldPaint;
			paint.Dispose();
		}
		void CreateFooterBricks() {
			if(!treeList.OptionsPrint.PrintReportFooter) return;
			foreach(PrintColumnInfo wi in printer.ColumnPrintInfo) {
				TreeListColumn col = wi.Column;
				Rectangle rect = new Rectangle(wi.Bounds.Left, topNodeSide + wi.RowIndex * PrintInfo.FooterHeight, wi.Bounds.Width, PrintInfo.FooterHeight * wi.RowCount);
				ITextBrick brick = col == null ? CreateEmptyFooterBrick(rect) : CreateFooterBrick(col, rect);
				printer.graph.DrawBrick(brick, rect);
			}
		}
		protected virtual ITextBrick CreateEmptyFooterBrick(Rectangle rect) {
			AppearanceObject style = UsePrintStyles ? PrintInfo.PrintAppearance.FooterPanel : viewInfo.PaintAppearance.FooterPanel;
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(style, DevExpress.XtraPrinting.BorderSide.All, FooterBorderColor, 1));
			ITextBrick brick = printer.PS.CreateTextBrick();
			brick.Style.SetAlignment(style.TextOptions.HAlignment, style.TextOptions.VAlignment);
			return brick;
		}
		protected virtual ITextBrick CreateFooterBrick(TreeListColumn col, Rectangle rect) {
			AppearanceObject style = UsePrintStyles ? PrintInfo.PrintAppearance.FooterPanel : viewInfo.PaintAppearance.FooterPanel;
			string s = string.Empty;
			if(col.SummaryFooter != SummaryItemType.None) {
				object val = treeList.GetPrintSummaryValue(treeList.Nodes, col, col.SummaryFooter, col.AllNodesSummary, true);
				s = TreeListViewInfo.FormatValue(val, col, col.SummaryFooterStrFormat);
			}
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(style, DevExpress.XtraPrinting.BorderSide.All, FooterBorderColor, 1));
			ITextBrick brick = printer.PS.CreateTextBrick();
			brick.Text = s;
			brick.Style.SetAlignment(style.TextOptions.HAlignment, style.TextOptions.VAlignment);
			return brick;
		}
		protected virtual IBrick CreateEmptyCellBrick(CellInfo cell, TreeListNode node) {
			AppearanceObject appearance = GetCellAppearance(null, null);
			AppearanceHelper.Combine(cell.PaintAppearance, new AppearanceObject[] { appearance, viewInfo.ClonePaintAppearance(cell.PaintAppearance) });
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(appearance, DevExpress.XtraPrinting.BorderSide.All, BorderColor, 1));
			ITextBrick brick = printer.PS.CreateTextBrick();
			brick.Rect = new Rectangle(cell.Bounds.X, 0, cell.Bounds.Width, cell.Bounds.Height);
			return brick;
		}
		protected virtual IBrick CreateCellBrick(CellInfo cell, TreeListNode node) {
			AppearanceObject appearance = GetCellAppearance(cell.Column.AppearanceCell, cell.RowInfo.GetCellConditionAppearance(cell.Column));
			if(!UsePrintStyles) appearance = treeList.InternalGetCustomNodeCellStyle(cell.Column, node, appearance);
			AppearanceHelper.Combine(cell.PaintAppearance, new AppearanceObject[] { appearance, viewInfo.ClonePaintAppearance(cell.PaintAppearance) });
			printer.SetDefaultBrickStyle(BrickStyle.CreateDefault());
			DevExpress.XtraEditors.PrintCellHelperInfo info = new DevExpress.XtraEditors.PrintCellHelperInfo(
				BorderColor,
				printer.PS,
				cell.EditorViewInfo.EditValue,
				cell.PaintAppearance,
				cell.EditorViewInfo.DisplayText,
				cell.Bounds,
				printer.graph,
				cell.EditorViewInfo.EditValue == null ? DevExpress.Utils.HorzAlignment.Default : GetCellHorizontalAlignment(cell),
				treeList.OptionsPrint.PrintHorzLines,
				treeList.OptionsPrint.PrintVertLines,
				cell.Column.Format.FormatString
				);
			IVisualBrick brick = cell.EditorViewInfo.Item.GetBrick(info);
			brick.TextValueFormatString = cell.ColumnInfo.Column.Format.FormatString;
			brick.Style.SetAlignment(cell.PaintAppearance.TextOptions.HAlignment, cell.PaintAppearance.TextOptions.VAlignment);
			BorderSide border = BorderSide.None;
			if(treeList.OptionsPrint.PrintHorzLines) border |= BorderSide.Top | BorderSide.Bottom;
			if(treeList.OptionsPrint.PrintVertLines) border |= BorderSide.Left | BorderSide.Right;
			brick.Sides = border;
			brick.Rect = new Rectangle(cell.Bounds.X, 0, cell.Bounds.Width, cell.Bounds.Height);
			return brick;
		}
		DevExpress.Utils.HorzAlignment GetCellHorizontalAlignment(CellInfo cell) {
			if(cell.PaintAppearance.TextOptions.HAlignment != HorzAlignment.Default) return cell.PaintAppearance.TextOptions.HAlignment;
			return treeList.ContainerHelper.GetDefaultValueAlignment(cell.EditorViewInfo.Item, cell.EditorViewInfo.EditValue.GetType());
		}
		IImageBrick CreateImageBrick(Image image, Rectangle rect) {
			IImageBrick brick = printer.PS.CreateImageBrick();
			brick.Image = image;
			brick.BorderWidth = 0;
			brick.Rect = rect;
			return brick;
		}
		protected virtual void CreateImagesBricks(IList bricks, int selectIndex, int stateIndex, Color indentColor, Point location, int height, out int imagesTotalWidth) {
			Image select = null, state = null;
			select = ImageCollection.GetImageListImage(treeList.SelectImageList, selectIndex);
			state = ImageCollection.GetImageListImage(treeList.StateImageList, stateIndex);
			int imagesWidth = GetImagesTotalWidth();
			imagesTotalWidth = 0;
			if(imagesWidth == 0) return;
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(CreateTransparentAppearance(), BorderSide.None, Color.Transparent, 0));
			int left = location.X;
			int indent = 0;
			if(select != null) {
				indent = SelectImageWidth;
				int y = (height - viewInfo.RC.SelectImageSize.Height) / 2;
				Rectangle selectRect = new Rectangle(left, 0, SelectImageWidth, height);
				IImageBrick selectImageBrick = CreateImageBrick(select, selectRect);
				if(treeList.OptionsPrint.PrintFilledTreeIndent) selectImageBrick.BackColor = indentColor;
				selectImageBrick.Padding = new PaddingInfo(1, 1, y, y);
				selectImageBrick.SizeMode = ImageSizeMode.CenterImage;
				bricks.Add(selectImageBrick);
			}
			if(state != null) {
				int y = (height - viewInfo.RC.StateImageSize.Height) / 2;
				Rectangle stateRect = new Rectangle(left + indent, 0, StateImageWidth, height);
				IImageBrick stateImageBrick = CreateImageBrick(state, stateRect);
				if(treeList.OptionsPrint.PrintFilledTreeIndent) stateImageBrick.BackColor = indentColor;
				stateImageBrick.Padding = new PaddingInfo(1, 1, y, y);
				stateImageBrick.SizeMode = ImageSizeMode.CenterImage;
				bricks.Add(stateImageBrick);
			}
			imagesTotalWidth = imagesWidth;
		}
		int GetImagesTotalWidth() {
			int imagesWidth = 0;
			if(treeList.SelectImageList != null)
				imagesWidth += SelectImageWidth;
			if(treeList.StateImageList != null)
				imagesWidth += StateImageWidth;
			return imagesWidth;
		}
		void DrawRowFooterBricks(ArrayList indents) {
			TreeListNode parent = null;
			int groupFooterHeight = PrintInfo.RowFooterHeight * treeList.ViewInfo.ColumnPanelRowCount;
			do {
				TreeListNode lastChildNode = (TreeListNode)lastChildren.Pop();
				parent = lastChildNode.ParentNode;
				int indent = lastChildNode.Level * viewInfo.RC.LevelWidth +
					+viewInfo.RC.SelectImageSize.Width + viewInfo.RC.StateImageSize.Width;
				if(indents != null && indents.Count != 0)
					CreateFooterIndentBrick(indents, new Rectangle(0, topNodeSide, indent,
						groupFooterHeight), lastChildNode.Level - 1);
				else if(treeList.OptionsPrint.PrintFilledTreeIndent)
					CreateFillBrick(new Rectangle(0, topNodeSide, indent,
						groupFooterHeight), 0, GetCellAppearance(FirstColumnCellApperance, null).BackColor);
				foreach(PrintColumnInfo wi in printer.ColumnPrintInfo) {
					Rectangle rect = new Rectangle(wi.Bounds.Left, topNodeSide + wi.RowIndex * PrintInfo.RowFooterHeight, wi.Bounds.Width, PrintInfo.RowFooterHeight * wi.RowCount);
					TreeListColumn column = wi.Column;
					if(printer.ColumnPrintInfo.IndexOf(wi) == 0 || wi.Bounds.Left == 0) {
						rect.X += indent;
						rect.Width -= indent;
					}
					ITextBrick brick = column == null ? CreateEmptyRowFooterBrick(rect) : CreateRowFooterBrick(column, parent, rect);
					printer.graph.DrawBrick(brick, rect);
				}
				even = !even;
				topNodeSide += groupFooterHeight;
			} while(lastChildren.Count > 0 && parent == lastChildren.Peek());
		}
		protected virtual ITextBrick CreateEmptyRowFooterBrick(Rectangle rect) {
			AppearanceObject style = UsePrintStyles ? PrintInfo.PrintAppearance.GroupFooter :
			   viewInfo.PaintAppearance.GroupFooter;
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(style, BorderSide.All, GroupFooterBorderColor, 1));
			ITextBrick brick = printer.PS.CreateTextBrick();
			brick.Rect = rect;
			return brick;
		}
		protected virtual ITextBrick CreateRowFooterBrick(TreeListColumn column, TreeListNode node, Rectangle rect) {
			string s = string.Empty;
			if(column != null && column.RowFooterSummary != SummaryItemType.None) {
				object val = treeList.GetPrintSummaryValue(node.Nodes, column, column.RowFooterSummary, false, false);
				s = TreeListViewInfo.FormatValue(val, column, column.RowFooterSummaryStrFormat);
			}
			AppearanceObject style = UsePrintStyles ? PrintInfo.PrintAppearance.GroupFooter :
				viewInfo.PaintAppearance.GroupFooter;
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(style, BorderSide.All, GroupFooterBorderColor, 1));
			ITextBrick brick = printer.PS.CreateTextBrick();
			brick.Text = s;
			brick.Rect = rect;
			return brick;
		}
		void CreateFooterIndentBrick(ArrayList indents, Rectangle r, int level) {
			if(indents == null || indents.Count == 0) return;
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(CreateTransparentAppearance(), BorderSide.None, Color.Transparent, 0));
			ArrayList inds = new ArrayList();
			for(int i = 0; i < level; i++)
				inds.Add(indents[i]);
			if(treeList.OptionsPrint.PrintFilledTreeIndent && viewInfo.RC.LevelWidth * level < r.Width) {
				IBrick filledBrick = CreateFillBrick(r, level, GetCellAppearance(FirstColumnCellApperance, null).BackColor);
				printer.graph.DrawBrick(filledBrick, r);
			}
			if(level == 0) return;
			r.Width = level * viewInfo.RC.LevelWidth;
			IImageBrick brick = (IImageBrick)printer.PS.CreateImageBrick();
			RectangleF ef1 = new RectangleF(0, 0, (float)level * viewInfo.RC.LevelWidth, (float)r.Height);
			ef1.Width = ef1.Width / ((float)inds.Count);
			Image image = new Bitmap(r.Width, r.Height);
			Graphics graphics = Graphics.FromImage(image);
			printer.treePainter.DrawIndents(graphics, inds, ef1, TreeLineBrush);
			brick.Image = image;
			if(treeList.OptionsPrint.PrintFilledTreeIndent) brick.BackColor = GetCellAppearance(FirstColumnCellApperance, null).BackColor;
			brick.BorderWidth = 0;
			printer.graph.DrawBrick(brick, r);
			graphics.Dispose();
		}
		IBrick CreateFillBrick(Rectangle r, int level, Color backColor) {
			printer.SetDefaultBrickStyle(AppearanceHelper.CreateBrick(CreateTransparentAppearance(), BorderSide.None, Color.Transparent, 0));
			int cx = viewInfo.RC.LevelWidth * level;
			r.X += cx;
			r.Width -= cx;
			IVisualBrick brick = (IVisualBrick)printer.PS.CreateBrick("VisualBrick");
			brick.BackColor = backColor;
			brick.BorderColor = backColor;
			brick.Rect = r;
			return brick;
		}
		protected virtual bool CanPushNode(TreeListNode node) {
			return (node.HasChildren && node.Nodes.Count > 0);
		}
		protected AppearanceObject GetCellAppearance(AppearanceObjectEx columnAppearanceCell, AppearanceObjectEx conditionAppearance) {
			AppearanceObject result = new AppearanceObject();
			if(UsePrintStyles)
				AppearanceHelper.Combine(result, new AppearanceObject[] { conditionAppearance, even ? PrintInfo.PrintAppearance.EvenRow : PrintInfo.PrintAppearance.OddRow });
			else
				AppearanceHelper.Combine(result, viewInfo.GetRowMixAppearances(even ? TreeNodeCellState.Even : TreeNodeCellState.Odd, columnAppearanceCell, null, conditionAppearance));
			return result;
		}
		AppearanceObjectEx FirstColumnCellApperance {
			get {
				if(printer.ColumnPrintInfo.Count == 0) return null;
				PrintColumnInfo cw = printer.ColumnPrintInfo[0];
				return (cw.Column == null ? null : cw.Column.AppearanceCell);
			}
		}
		int StateImageWidth { get { return Math.Max(viewInfo.RC.StateImageSize.Width, viewInfo.RC.LevelWidth); } }
		int SelectImageWidth { get { return Math.Max(viewInfo.RC.SelectImageSize.Width, viewInfo.RC.LevelWidth); } }
		protected Brush TreeLineBrush { get { return (even ? evenTreeLineBrush : oddTreeLineBrush); } }
		protected bool UsePrintStyles { get { return treeList.OptionsPrint.UsePrintStyles; } }
		protected Brush BorderBrush { get { return (UsePrintStyles ? PrintInfo.PrintAppearance.Lines.GetBackBrush(cache) : viewInfo.PaintAppearance.HorzLine.GetBackBrush(cache)); } }
		protected Color BorderColor { get { return (UsePrintStyles ? PrintInfo.PrintAppearance.Lines.BackColor : viewInfo.PaintAppearance.HorzLine.BackColor); } }
		protected Color FooterBorderColor { get { return (UsePrintStyles ? PrintInfo.PrintAppearance.FooterPanel.BorderColor : viewInfo.PaintAppearance.HorzLine.BackColor); } }
		protected Color GroupFooterBorderColor { get { return (UsePrintStyles ? PrintInfo.PrintAppearance.GroupFooter.BorderColor : viewInfo.PaintAppearance.HorzLine.BackColor); } }
		protected TreeListPrintInfo PrintInfo { get { return printer.PrintInfo; } }
	}
	public class TreeListOperationPrintVisibleNode : TreeListOperationPrintEachNode {
		internal TreeListOperationPrintVisibleNode(TreeList treeList, TreeListPrinter printer,
			TreeListViewInfo viewInfo, bool printTree, bool printImages)
			:
			base(treeList, printer, viewInfo, printTree, printImages) {
		}
		public override bool NeedsVisitChildren(TreeListNode node) { return node.Expanded; }
		protected override bool CanPushNode(TreeListNode node) {
			return (base.CanPushNode(node) && node.Expanded);
		}
	}
	public class TreeListOperationCalcTopAndBottomValues : TreeListOperation {
		List<object> values = new List<object>();
		protected bool recursively, getBottom, isPercentArgument;
		protected int argument;
		public TreeListOperationCalcTopAndBottomValues(TreeListColumn column, bool getBottom, bool isPercentArgument, int argument, bool recursively) {
			Column = column;
			this.getBottom = getBottom;
			this.isPercentArgument = isPercentArgument;
			this.argument = argument;
			this.recursively = recursively;
		}
		public override void Execute(TreeListNode node) {
			object value = node.GetValue(Column);
			if(object.ReferenceEquals(value, DBNull.Value)) value = null;
			values.Add(value);
		}
		public override void FinalizeOperation() {
			List<object> orderedList = getBottom ? values.OrderBy(v => v).ToList() : values.OrderByDescending(v => v).ToList();
			List<object> resultList = new List<object>();
			if(isPercentArgument)
				argument = (int)(((decimal)orderedList.Count / 100) * argument);
			for(int n = 0; n < Math.Min(argument, orderedList.Count); n++)
				resultList.Add(orderedList[n]);
			Result = resultList;
		}
		public TreeListColumn Column { get; private set; }
		public virtual List<object> Result { get; private set; }
		public override bool NeedsVisitChildren(TreeListNode node) { return recursively; }
	}
	public class TreeListOperationCalcUniqueAndDuplicateValues : TreeListOperation {
		protected bool duplicates, recursively;
		List<object> values = new List<object>();
		public TreeListOperationCalcUniqueAndDuplicateValues(TreeListColumn column, bool duplicates, bool recursively) {
			Column = column;
			this.duplicates = duplicates;
			this.recursively = recursively;
		}
		public override void Execute(TreeListNode node) {
			object value = node.GetValue(Column);
			if(object.ReferenceEquals(value, DBNull.Value)) value = null;
			values.Add(value);
		}
		public override void FinalizeOperation() {
			if(duplicates)
				Result = values.GroupBy(v => v).Where(v => v.Count() > 1).Select(v => v.Key).ToList();
			else
				Result = values.GroupBy(v => v).Where(v => v.Count() == 1).Select(v => v.Key).ToList();
		}
		public TreeListColumn Column { get; private set; }
		public virtual List<object> Result { get; private set; }
		public override bool NeedsVisitChildren(TreeListNode node) { return recursively; }
	}
	public abstract class TreeListOperationCalcSummary : TreeListOperation {
		protected TreeListColumn fColumn;
		protected SummaryItemType fItem;
		private bool recursively;
		public TreeListOperationCalcSummary(TreeListColumn column, SummaryItemType item, bool recursively) {
			this.fColumn = column;
			this.fItem = item;
			this.recursively = recursively;
		}
		public override bool NeedsVisitChildren(TreeListNode node) { return recursively; }
		public abstract object Result { get; }
	}
	public class TreeListOperationCalcSummarySum : TreeListOperationCalcSummary {
		protected decimal fSum;
		public TreeListOperationCalcSummarySum(TreeListColumn column, SummaryItemType item, bool recursively)
			:
			base(column, item, recursively) {
			fSum = 0.0M;
		}
		public override object Result { get { return fSum; } }
		public override void Execute(TreeListNode node) {
			if(!TreeListFilterHelper.IsNodeVisible(node)) return;
			object objVal = node[fColumn];
			decimal val = 0;
			if(objVal != null && objVal != DBNull.Value) {
				try { val = Convert.ToDecimal(objVal); }
				catch { val = 0; }
			}
			fSum += val;
		}
	}
	public class TreeListOperationCalcSummaryAverage : TreeListOperationCalcSummarySum {
		private int count;
		public TreeListOperationCalcSummaryAverage(TreeListColumn column, SummaryItemType item, bool recursively)
			:
			base(column, item, recursively) {
			count = 0;
		}
		public override object Result {
			get {
				if(count == 0) return 0M;
				return (fSum / count);
			}
		}
		public override void Execute(TreeListNode node) {
			base.Execute(node);
			if(!TreeListFilterHelper.IsNodeVisible(node)) return;
			count++;
		}
	}
	public class TreeListOperationCalcSummaryMinMax : TreeListOperationCalcSummary {
		private bool calcMin;
		private object min_max;
		public TreeListOperationCalcSummaryMinMax(TreeListColumn column, SummaryItemType item, bool recursively)
			:
			base(column, item, recursively) {
			calcMin = (item == SummaryItemType.Min);
			min_max = null;
		}
		public override object Result { get { return min_max; } }
		public override void Execute(TreeListNode node) {
			if(!TreeListFilterHelper.IsNodeVisible(node)) return;
			object val = node[fColumn];
			if(val == null) return;
			if(min_max == null) {
				min_max = val;
				return;
			}
			if(NodesComparer.InternalCompare(min_max, val) > 0) {
				if(calcMin) min_max = val;
			}
			else if(!calcMin) min_max = val;
		}
	}
	public class TreeListOperationColumnBestWidth : TreeListOperation {
		private TreeList treeList;
		private TreeListColumn column;
		private AppearanceObject widestAppearance;
		private int bestWidth;
		public TreeListOperationColumnBestWidth(TreeList treeList, TreeListColumn column) {
			this.treeList = treeList;
			this.column = column;
			this.treeList.ViewInfo.CreateResources();
			this.widestAppearance = treeList.ViewInfo.GetWidestRowAppearance();
			this.bestWidth = 0;
		}
		public override bool NeedsVisitChildren(TreeListNode node) {
			return (treeList.BestFitVisibleOnly ? node.Expanded : true);
		}
		public override void FinalizeOperation() {
			CalcFootersWidth();
			CalcCaptionWidth();
		}
		public override void Execute(TreeListNode node) {
			if(treeList.BestFitVisibleOnly && !node.Visible) return;
			DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo tempEditorViewInfo = CellInfo.GetEditorViewInfo(column, node);
			AppearanceHelper.Combine(tempEditorViewInfo.PaintAppearance, new AppearanceObject[] { column.AppearanceCell, this.widestAppearance });
			tempEditorViewInfo.PaintAppearance = treeList.InternalGetCustomNodeCellStyle(column, node, tempEditorViewInfo.PaintAppearance);
			if(treeList.GetColumnShowButtonMode(column) == ShowButtonModeEnum.ShowAlways)
				tempEditorViewInfo.DetailLevel = DevExpress.XtraEditors.Controls.DetailLevel.Full;
			else
				tempEditorViewInfo.DetailLevel = DevExpress.XtraEditors.Controls.DetailLevel.Minimum;
			int w = tempEditorViewInfo.CalcBestFit(graphics).Width + 4 + treeList.GetColumnIndent(column, node);
			if(w > bestWidth) bestWidth = w;
			if(treeList.IncludeRowFooterSummaryWidthInColumnBestFit) {
				if(node == node.owner.LastNodeEx && !node.Expanded) {
					object val = treeList.GetSummaryValueCore(node.owner, column, column.RowFooterSummary, false, false);
					string text = TreeListViewInfo.FormatValue(val, column, column.RowFooterSummaryStrFormat);
					bestWidth = Math.Max(bestWidth, GetFooterBoundWidth(treeList.ViewInfo.PaintAppearance.GroupFooter, text) + treeList.GetColumnIndent(column, node));
				}
			}
		}
		private void CalcCaptionWidth() {
			if(!treeList.OptionsView.ShowColumns) return;
			ColumnInfo ci = treeList.ViewInfo.CreateColumnInfo(column);
			ci.SetAppearance(treeList.ViewInfo.PaintAppearance.HeaderPanel);
			treeList.ViewInfo.UpdateGlyphInfo(ci);
			bestWidth = Math.Max(bestWidth, ObjectPainter.CalcObjectMinBounds(graphics, treeList.ElementsLookAndFeel.Painter.Header, ci).Width + 2);
		}
		private void CalcFootersWidth() {
			if(treeList.OptionsView.ShowSummaryFooter) {
				FooterItem fi = treeList.ViewInfo.SummaryFooterInfo[column];
				AppearanceObject vs = treeList.ViewInfo.PaintAppearance.FooterPanel;
				if(fi != null && fi.ItemType != SummaryItemType.None)
					bestWidth = Math.Max(bestWidth, GetFooterBoundWidth(vs, fi.ItemText));
				else
					bestWidth = Math.Max(bestWidth, GetFooterWidth(column.SummaryFooter, column.SummaryFooterStrFormat, vs));
			}
		}
		private int GetFooterWidth(SummaryItemType it, string footerFmt, AppearanceObject appearance) {
			if(it == SummaryItemType.None) return 0;
			int fullWidth = GetFooterBoundWidth(appearance, footerFmt);
			int startIndex = footerFmt.IndexOf('{', 0);
			if(startIndex == -1) return fullWidth;
			int endIndex = footerFmt.IndexOf('}', startIndex);
			if(endIndex == -1) return fullWidth;
			footerFmt = footerFmt.Remove(startIndex, endIndex - startIndex + 1);
			return GetFooterBoundWidth(appearance, footerFmt);
		}
		private int GetFooterBoundWidth(AppearanceObject appearance, string text) {
			Rectangle r = new Rectangle(Point.Empty, appearance.CalcTextSize(graphics, text, 0).ToSize());
			r.Inflate(2, 2);
			FooterCellInfoArgs args = new FooterCellInfoArgs(null);
			args.Bounds = r;
			r = treeList.ElementsLookAndFeel.Painter.FooterCell.CalcBoundsByClientRectangle(args);
			return Rectangle.Inflate(r, 2, 0).Width;
		}
		Graphics graphics { get { return treeList.ViewInfo.GInfo.Graphics; } }
		public int BestWidth { get { return bestWidth; } }
	}
	public class TreeListOperationFindNode : TreeListOperation {
		Predicate<TreeListNode> match;
		TreeListNode foundNode;
		public TreeListOperationFindNode(Predicate<TreeListNode> match) {
			this.match = match;
		}
		public override void Execute(TreeListNode node) {
			if(match(node))
				foundNode = node;
		}
		public override bool CanContinueIteration(TreeListNode node) {
			return Node == null;
		}
		public TreeListNode Node { get { return foundNode; } }
	}
	public class TreeListOperationFindNodeByID : TreeListOperationFindNode {
		public TreeListOperationFindNodeByID(int nodeId)
			: base((node) => { return node.Id == nodeId; }) {
		}
	}
	public class TreeListOperationFindNodeByFieldValue : TreeListOperationFindNode {
		public TreeListOperationFindNodeByFieldValue(object val, string fieldName)
			: base((node) =>
			{
				object value = node[fieldName];
				if(TreeListData.IsNull(val)) return (val == value);
				return val.Equals(value);
			}) {
		}
	}
	public abstract class TreeListOperationSynchronizeID : TreeListOperation {
		public abstract int GetNewId(int value);
		public override void Execute(TreeListNode node) { node.SetId(GetNewId(node.Id)); }
	}
	internal class TreeListOperationSourceSynchronizeNodesID : TreeListOperationSynchronizeID {
		NodesIdInfoManager manager;
		public TreeListOperationSourceSynchronizeNodesID(NodesIdInfoManager manager) {
			this.manager = manager;
		}
		public override int GetNewId(int value) { return (value - Manager.GetSumLength(value)); }
		NodesIdInfoManager Manager { get { return manager; } }
	}
	public class TreeListOperationSynchronizeNodesID : TreeListOperationSynchronizeID {
		protected int fIndexFrom;
		protected int fStep;
		public TreeListOperationSynchronizeNodesID(int indexFrom, int step) {
			this.fIndexFrom = indexFrom;
			this.fStep = step;
		}
		public override int GetNewId(int value) {
			if(value > fIndexFrom)
				return (value + fStep);
			return value;
		}
	}
	public class TreeListOperationSaveNodesData : TreeListOperation {
		private Hashtable nodesData;
		public TreeListOperationSaveNodesData() {
			this.nodesData = new Hashtable();
		}
		public override void Execute(TreeListNode node) {
			NodeData nData = new NodeData(node.Expanded,  node.Tag, (!node.Visible && !node.IsVisible ? false : true), node.CheckState);
			nodesData[node.Id] = nData;
		}
		public Hashtable NodesData { get { return nodesData; } }
	}
	public class TreeListOperationRestoreNodesData : TreeListOperation {
		private Hashtable nodesData;
		public TreeListOperationRestoreNodesData(Hashtable nodesData) {
			this.nodesData = nodesData;
		}
		public override void Execute(TreeListNode node) {
			NodeData nData = (NodeData)nodesData[node.Id];
			if(nData != null) {
				node.Expanded = nData.expanded;
				node.Tag = nData.data;
				node.CheckState = nData.checkState;
				if(!node.TreeList.OptionsBehavior.EnableFiltering) node.Visible = nData.visible;
			}
		}
	}
	public class TreeListOperationCloneChildren : TreeListOperation {
		private TreeListNode lastClone;
		private Hashtable pairs;
		public TreeListOperationCloneChildren(TreeListNodes ownerClone, TreeListNodes children) {
			this.pairs = new Hashtable();
			this.pairs[children] = ownerClone;
			this.lastClone = null;
		}
		public override void Execute(TreeListNode node) {
			TreeListNode clonedChild = (TreeListNode)node.Clone();
			clonedChild.owner = (TreeListNodes)pairs[node.owner];
			clonedChild.owner._add(clonedChild);
			lastClone = clonedChild;
		}
		public override bool NeedsVisitChildren(TreeListNode node) {
			pairs[node.Nodes] = lastClone.Nodes;
			return true;
		}
	}
	public class TreeListOperationAccumulateNodes : TreeListVisibleNodeOperation {
		ArrayList al;
		bool excludeFilteredNodes;
		public TreeListOperationAccumulateNodes()
			: this(false) {
		}
		public TreeListOperationAccumulateNodes(bool excludeFilteredNodes) {
			this.al = new ArrayList();
			this.excludeFilteredNodes = excludeFilteredNodes;
		}
		public override void Execute(TreeListNode node) {
			if(excludeFilteredNodes && !node.Visible) return;
			al.Add(node);
		}
		public virtual ArrayList Nodes { get { return al; } }
	}
	internal class TreeListOperationAddNodeById : TreeListOperationAccumulateNodes {
		int id;
		internal TreeListOperationAddNodeById(int id) { this.id = id; }
		public override void Execute(TreeListNode node) {
			if(node.Id == id)
				base.Execute(node);
		}
	}
	internal class TreeListOperationDeleteNodeFromSelection : TreeListOperation {
		private TreeListMultiSelection selection;
		internal TreeListOperationDeleteNodeFromSelection(TreeListMultiSelection selection) {
			this.selection = selection;
		}
		public override void Execute(TreeListNode node) {
			selection.Remove(node);
		}
	}
	public class TreeListOperationDeleteNodes : TreeListOperation {
		TreeList treeList;
		NodesIdInfoManager manager;
		public TreeListOperationDeleteNodes(TreeList treeList) {
			this.treeList = treeList;
		}
		public override void Execute(TreeListNode node) {
			TreeList.Selection.InternalRemove(node);
			if(!node.HasClones)
				Manager.Add(node.Id);
			TreeList.RemoveNodeHeight(node);
		}
		public override void FinalizeOperation() {
			if(treeList.IsGroupDeleteOperation) return;
			treeList.SynchronizeDataListOnRemove(Manager);
		}
		NodesIdInfoManager Manager {
			get {
				if(treeList.IsGroupDeleteOperation)
					return treeList.NodesIdManager;
				if(manager == null)
					manager = new NodesIdInfoManager();
				return manager;
			}
		}
		protected TreeList TreeList { get { return treeList; } }
	}
	public class TreeListOperationMoveChildNodesToRoot : TreeListOperation {
		TreeListNode parentNode;
		public TreeListOperationMoveChildNodesToRoot(TreeListNode parentNode) {
			this.parentNode = parentNode;
			TreeList.LockReloadNodes();
		}
		public override void Execute(TreeListNode node) {
			node[TreeList.ParentFieldName] = TreeList.RootValue;
		}
		public override void FinalizeOperation() { TreeList.UnlockReloadNodes(); }
		public override bool NeedsVisitChildren(TreeListNode node) { return false; }
		protected TreeList TreeList { get { return parentNode.TreeList; } }
	}
	internal class TreeListClearFilterNodesOperation : TreeListOperation {
		public override void Execute(TreeListNode node) {
			node.Visible = node._visible = true;
		}
	}
	internal class TreeListGetFilterPopupValuesOperation : TreeListOperation {
		object[] values = new object[] { };
		ArrayList internalValues = new ArrayList();
		TreeList treeList;
		TreeListColumn column;
		bool includeFilteredOut, roundDateTime, displayText;
		bool isDateTimeColumn;
		bool implyNullLikeEmptyStringWhenFiltering;
		public TreeListGetFilterPopupValuesOperation(TreeList treeList, TreeListColumn column, bool includeFilteredOut, bool roundDateTime, bool displayText, bool implyNullLikeEmptyStringWhenFiltering) {
			this.treeList = treeList;
			this.column = column;
			this.includeFilteredOut = includeFilteredOut;
			this.roundDateTime = roundDateTime;
			this.displayText = displayText;
			this.isDateTimeColumn = !displayText && (column.ColumnType.Equals(typeof(DateTime)) || column.ColumnType.Equals(typeof(DateTime?)));
			this.implyNullLikeEmptyStringWhenFiltering = implyNullLikeEmptyStringWhenFiltering;
		}
		public object[] Values { get { return values; } }
		public override void Execute(TreeListNode node) {
			if(includeFilteredOut || node.Visible) {
				object value = null;
				if(displayText)
					value = treeList.Data.DataHelper.GetDisplayText(column, node);
				else
					value = treeList.GetNodeValue(node, column);
				if(implyNullLikeEmptyStringWhenFiltering) {
					if(value != null && !(value is DBNull) && !(value is System.IComparable)) return;
				}
				else {
					if(value == null || value is DBNull || !(value is System.IComparable)) return;
				}
				if(isDateTimeColumn & roundDateTime && (value is DateTime)) {
					DateTime org = (DateTime)value;
					value = new DateTime(org.Year, org.Month, org.Day);
				}
				internalValues.Add(value);
			}
		}
		public override bool NeedsVisitChildren(TreeListNode node) {
			if(treeList.OptionsBehavior.ExpandNodesOnFiltering && treeList.GetActualTreeListFilterMode() != FilterMode.Standard) return true;
			return node.Expanded;
		}
		public override void FinalizeOperation() {
			base.FinalizeOperation();
			CollectUniqueValues();
		}
		void CollectUniqueValues() {
			if(internalValues.Count == 0) return;
			values = internalValues.ToArray();
			DevExpress.Data.ValueComparer comparer = new DevExpress.Data.ValueComparer();
			Array.Sort(values, comparer);
			int uniqCount = 0, count = values.Length;
			object[] uniqValues = new object[count];
			object lastValue = null, curValue;
			for(int n = 0; n < count; n++) {
				curValue = values[n];
				if(n == 0 || comparer.Compare(curValue, lastValue) != 0) {
					uniqValues[uniqCount++] = curValue;
				}
				lastValue = curValue;
			}
			values = new object[uniqCount];
			Array.Copy(uniqValues, 0, values, 0, uniqCount);
		}
	}
}
