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
using System.Drawing;
using DevExpress.Services;
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Commands.Internal;
using LayoutUnit = System.Int32;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Mouse {
	#region ResizeTableRowMouseHandlerState
	public class ResizeTableRowMouseHandlerState : CancellableDragMouseHandlerStateBase {
		#region Fields
		readonly RichEditHitTestResult initialHitTestResult;
		readonly TableRowViewInfoBase tableRow;
		readonly PageViewInfo pageViewInfo;
		readonly int referenceTop; 
		readonly ResizeTableRowMouseHandlerStateStrategy platformStrategy;
		int initialTableRowBottom; 
		int tableRowBottom; 
		int mouseCursorOffset;
		#endregion
		public ResizeTableRowMouseHandlerState(RichEditMouseHandler mouseHandler, RichEditHitTestResult initialHitTestResult, TableRowViewInfoBase tableRow)
			: base(mouseHandler) {
			Guard.ArgumentNotNull(initialHitTestResult, "initialHitTestResult");
			Guard.ArgumentNotNull(tableRow, "tableRow");
			this.platformStrategy = CreatePlatformStrategy();
			this.initialHitTestResult = initialHitTestResult;
			this.tableRow = tableRow;
			this.mouseCursorOffset = initialHitTestResult.LogicalPoint.Y - tableRow.BottomAnchor.VerticalPosition;
			this.referenceTop = tableRow.TopAnchor.VerticalPosition;
			this.tableRowBottom = tableRow.BottomAnchor.VerticalPosition;
			this.initialTableRowBottom = tableRowBottom;
			Page topLevelPage = GetTopLevelPage(initialHitTestResult);
			this.pageViewInfo = Control.InnerControl.ActiveView.LookupPageViewInfoByPage(topLevelPage);
		}
		protected virtual ResizeTableRowMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateResizeTableRowMouseHandlerStateStrategy(this);
		}
		#region Properties
		protected internal override DocumentLayoutDetailsLevel HitTestDetailsLevel { get { return DocumentLayoutDetailsLevel.Page; } }
		public override bool AutoScrollEnabled { get { return false; } }
		public PageViewInfo PageViewInfo { get { return pageViewInfo; } }
		#endregion
		protected internal override RichEditCursor CalculateMouseCursor() {
			return RichEditCursors.ResizeTableRow;
		}
		protected internal override DataObject CreateDataObject() {
			return null;
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			SetMouseCursor(RichEditCursors.ResizeTableRow);
			RichEditHitTestResult hitTestResult = CalculateHitTest(point);
			if (hitTestResult == null)
				return allowedEffects;
			if (!Object.ReferenceEquals(GetTopLevelPage(initialHitTestResult), GetTopLevelPage(hitTestResult)))
				return allowedEffects;
			HideVisualFeedback();
			int bottom = SnapVertically(hitTestResult.LogicalPoint.Y - mouseCursorOffset);
			this.tableRowBottom = Math.Max(referenceTop, bottom);
			ShowVisualFeedback();
			return allowedEffects;
		}
		protected internal override bool CommitDrag(Point point, IDataObject dataObject) {
			if (tableRowBottom != initialTableRowBottom) {
				int bottom = SnapVertically(tableRowBottom);
				int height = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(bottom - referenceTop);
				ChangeTableRowHeightCommand command = DocumentModel.CommandsCreationStrategy.CreateChangeTableRowHeightCommand(Control, tableRow.Row, height);
				command.Execute();
			}
			return true;
		}
		protected internal virtual int SnapVertically(int value) {
			if (KeyboardHandler.IsAltPressed)
				return value;
			TableCellVerticalAnchorCollection anchors = tableRow.TableViewInfo.Anchors;
			List<TableCellVerticalAnchor> items = anchors.Items;
			int index = Algorithms.BinarySearch(items, new TableCellVerticalAnchorYComparable(value));
			if (index >= 0)
				return value;
			index = ~index;
			if (index >= items.Count) {
				if (ShouldSnapVertically(value, anchors.Last))
					return anchors.Last.VerticalPosition;
				return value;
			}
			if (ShouldSnapVertically(value, anchors[index]))
				return anchors[index].VerticalPosition;
			if (index > 0) {
				if (ShouldSnapVertically(value, anchors[index - 1]))
					return anchors[index - 1].VerticalPosition;
			}
			return value;
		}
		protected internal virtual bool ShouldSnapVertically(int value, ITableCellVerticalAnchor anchor) {
			int snapValue = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(2);
			int y = anchor.VerticalPosition;
			if (value < y)
				return y - value <= snapValue;
			y = anchor.VerticalPosition + anchor.BottomTextIndent;
			if (value > y)
				return value - y <= snapValue;
			return true;
		}
		protected internal override void BeginVisualFeedback() {
			platformStrategy.BeginVisualFeedback();
		}
		protected internal override void ShowVisualFeedback() {
			platformStrategy.ShowVisualFeedback();
		}
		protected internal override void HideVisualFeedback() {
			platformStrategy.HideVisualFeedback();
		}
		protected internal override void EndVisualFeedback() {
			platformStrategy.EndVisualFeedback();
		}
		protected internal void DrawReversibleLineCore() {
			DrawReversibleLineCore(tableRowBottom);
		}
		protected internal virtual void DrawReversibleLineCore(int y) {
			platformStrategy.DrawReversibleLineCore(y);
		}
	}
	#endregion
	#region ResizeTableVirtualColumnMouseHandlerState
	public class ResizeTableVirtualColumnMouseHandlerState : CancellableDragMouseHandlerStateBase {
		#region Fields
		readonly RichEditHitTestResult initialHitTestResult;
		readonly VirtualTableColumn column;
		readonly PageViewInfo pageViewInfo;
		readonly LayoutUnit referenceLeft; 
		readonly LayoutUnit referenceRight;
		readonly ResizeTableVirtualColumnMouseHandlerStateStrategy platformStrategy;
		int initialTableColumnRight; 
		int tableColumnRight; 
		int mouseCursorOffset;
		#endregion
		public ResizeTableVirtualColumnMouseHandlerState(RichEditMouseHandler mouseHandler, RichEditHitTestResult initialHitTestResult, VirtualTableColumn column)
			: base(mouseHandler) {
			Guard.ArgumentNotNull(initialHitTestResult, "initialHitTestResult");
			Guard.ArgumentNotNull(column, "column");
			this.platformStrategy = CreatePlatformStrategy();
			this.initialHitTestResult = initialHitTestResult;
			this.column = column;
			this.mouseCursorOffset = 0;
			this.referenceLeft = column.MaxLeftBorder;
			this.referenceRight = column.MaxRightBorder;
			this.tableColumnRight = column.Position;
			this.initialTableColumnRight = tableColumnRight;
			Page topLevelPage = GetTopLevelPage(initialHitTestResult);
			this.pageViewInfo = Control.InnerControl.ActiveView.LookupPageViewInfoByPage(topLevelPage);
		}
		#region Properties
		protected internal override DocumentLayoutDetailsLevel HitTestDetailsLevel { get { return DocumentLayoutDetailsLevel.Page; } }
		public override bool AutoScrollEnabled { get { return false; } }
		public PageViewInfo PageViewInfo { get { return pageViewInfo; } }
		#endregion
		protected virtual ResizeTableVirtualColumnMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateResizeTableVirtualColumnMouseHandlerStateStrategy(this);
		}
		protected internal override RichEditCursor CalculateMouseCursor() {
			return RichEditCursors.ResizeTableColumn;
		}
		protected internal override DataObject CreateDataObject() {
			return null;
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			SetMouseCursor(RichEditCursors.ResizeTableColumn);
			RichEditHitTestResult hitTestResult = CalculateHitTest(point);
			if (hitTestResult == null)
				return allowedEffects;
			if (!Object.ReferenceEquals(GetTopLevelPage(initialHitTestResult), GetTopLevelPage(hitTestResult)))
				return allowedEffects;
			HideVisualFeedback();
			int right = SnapHorizontally(hitTestResult.LogicalPoint.X - mouseCursorOffset);
			this.tableColumnRight = Math.Min(Math.Max(referenceLeft, right), referenceRight);			
			ShowVisualFeedback();
			return allowedEffects;
		}
		protected internal override bool CommitDrag(Point point, IDataObject dataObject) {
			if (tableColumnRight != initialTableColumnRight) {
				int right = SnapHorizontally(tableColumnRight);
				this.tableColumnRight = Math.Min(Math.Max(referenceLeft, right), referenceRight);
				ChangeTableVirtualColumnRightCommand command = new ChangeTableVirtualColumnRightCommand(Control, column, tableColumnRight);
				command.Execute();
			}
			return true;
		}
		protected internal virtual int SnapHorizontally(int value) {
			if (KeyboardHandler.IsAltPressed)
				return value;
			TableViewInfo tableViewInfo = column.TableViewInfo;
			int horizontalOffset = tableViewInfo.Column.Bounds.Left;
			SortedList<LayoutUnit> positions = tableViewInfo.VerticalBorderPositions.AlignmentedPosition;
			int index = positions.BinarySearch(value - horizontalOffset);
			if (index >= 0)
				return value;
			index = ~index;
			if (index >= positions.Count) {
				if (ShouldSnapHorizontally(value, positions.Count - 1, tableViewInfo))
					return positions.Last + horizontalOffset;
				else
					return value;
			}
			if (ShouldSnapHorizontally(value, index, tableViewInfo))
				return positions[index] + horizontalOffset;
			if (index > 0) {
				if (ShouldSnapHorizontally(value, index - 1, tableViewInfo))
					return positions[index - 1] + horizontalOffset;
			}
			return value;
		}
		protected internal virtual bool ShouldSnapHorizontally(int value, int snapPositionIndex, TableViewInfo tableViewInfo) {
			int snapValue = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(2);
			int position = tableViewInfo.GetAlignmentedPosition(snapPositionIndex);
			return Math.Abs(position - value) <= snapValue;
		}
		protected internal override void BeginVisualFeedback() {
			platformStrategy.BeginVisualFeedback();
		}
		protected internal override void ShowVisualFeedback() {
			platformStrategy.ShowVisualFeedback();
		}
		protected internal override void HideVisualFeedback() {
			platformStrategy.HideVisualFeedback();
		}
		protected internal override void EndVisualFeedback() {
			platformStrategy.EndVisualFeedback();
		}
		protected internal void DrawReversibleLineCore() {
			DrawReversibleLineCore(tableColumnRight);
		}
		protected internal virtual void DrawReversibleLineCore(int x) {
			platformStrategy.DrawReversibleLineCore(x);
		}
	}
	#endregion
}
