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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Office;
using DevExpress.XtraRichEdit.LayoutEngine;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region SimpleViewDocumentFormattingController
	public class SimpleViewDocumentFormattingController : DocumentFormattingController {
		readonly SimpleView view;
		public SimpleViewDocumentFormattingController(SimpleView view, DocumentLayout documentLayout, PieceTable pieceTable)
			: base(documentLayout, pieceTable, null, null) {
				this.view = view;
		}
		public SimpleView View { get { return view; } }
		protected internal override PageController CreatePageController() {
			return new SimpleViewPageController(DocumentLayout);
		}
		protected internal override ColumnController CreateColumnController() {
			return new SimpleViewColumnController(PageAreaController);
		}
		protected internal override RowsController CreateRowController() {
			return new SimpleViewRowsController(this, PieceTable, ColumnController, DocumentModel.LayoutOptions.SimpleView.MatchHorizontalTableIndentsToTextEdge);
		}
	}
	#endregion
	#region DraftViewRowsController
	public abstract class SimpleViewRowsControllerBase : RowsController {
		protected SimpleViewRowsControllerBase(PieceTable pieceTable, ColumnController columnController, bool matchHorizontalTableIndentsToTextEdge)
			: base(pieceTable, columnController, matchHorizontalTableIndentsToTextEdge) {
		}
		protected internal override TablesController CreateTablesController() {
			return new SimpleViewTablesController(ColumnController.PageAreaController.PageController, this);
		}
		public override TableGridCalculator CreateTableGridCalculator(DocumentModel documentModel, TableWidthsCalculator widthsCalculator, int maxTableWidth) {
			return new TableGridCalculator(documentModel, widthsCalculator, maxTableWidth, DocumentModel.LayoutOptions.SimpleView.AllowTablesToExtendIntoMargins, true);
		}
		protected LineNumberingRestart GetEffectiveLineNumberingRestartTypeBase(Section section) {
			return base.GetEffectiveLineNumberingRestartType(section);
		}
		protected internal override LineNumberingRestart GetEffectiveLineNumberingRestartType(Section section) {
			LineNumberingRestart result = base.GetEffectiveLineNumberingRestartType(section);
			if (result == LineNumberingRestart.NewPage)
				return LineNumberingRestart.NewSection;
			return result;
		}
		protected override FloatingObjectSizeAndPositionController CreateFloatingObjectSizeAndPositionController() {
			return new SimpleViewFloatingObjectSizeAndPositionController(this);
		}
	}
	#endregion
	#region CommentRowsController
	public class CommentRowsController : RowsController {
		public CommentRowsController(PieceTable pieceTable, ColumnController columnController, bool matchHorizontalTableIndentsToTextEdge)
			: base(pieceTable, columnController, matchHorizontalTableIndentsToTextEdge) {
		}
		public int FirstParagraphIndent { get; set; }
		protected internal override void ObtainRowIndents() {
			if (Paragraph.Index != ParagraphIndex.Zero) {
				CurrentRowIndent = 0;
				RegularRowIndent = 0;
			}
			else {
				CurrentRowIndent = FirstParagraphIndent;
				RegularRowIndent = 0;
			}
		}
		protected internal override int CalculateSpacingBefore() {
			return 0;
		}
		protected internal override LineSpacingCalculatorBase CreateLineSpacingCalculator() {
			return new SingleSpacingCalculator();
		}
		protected internal override int GetContextualSpacingAfter(Paragraph paragraph) {
			return 0;
		}
	}
	#endregion
	#region SimpleViewRowsController
	public class SimpleViewRowsController : SimpleViewRowsControllerBase {
		readonly SimpleViewDocumentFormattingController controller;
		public SimpleViewRowsController(SimpleViewDocumentFormattingController controller, PieceTable pieceTable, ColumnController columnController, bool matchHorizontalTableIndentsToTextEdge)
			: base(pieceTable, columnController, matchHorizontalTableIndentsToTextEdge) {
				Guard.ArgumentNotNull(controller, "controller");
				this.controller = controller;
		}
		public SimpleViewDocumentFormattingController Controller { get { return controller; } }
		protected override CurrentHorizontalPositionController CreateCurrentHorizontalPosition() {
			return new SimpleViewCurrentHorizontalPositionController(this);
		}
		protected override CurrentHorizontalPositionController CreateCurrentHorizontalPosition(int position) {
			return new SimpleViewCurrentHorizontalPositionController(this, position);
		}
	}
	#endregion
	#region SimpleViewCurrentHorizontalPositionController
	public class SimpleViewCurrentHorizontalPositionController : CurrentHorizontalPositionController {
		public SimpleViewCurrentHorizontalPositionController(SimpleViewRowsController rowsController)
			: base(rowsController) {
		}
		 public SimpleViewCurrentHorizontalPositionController(RowsController rowsController, int position) : base(rowsController, position){
		}
		 protected internal override bool CanFitBoxToCurrentRow(Size boxSize) {
			 if (((SimpleViewRowsController)RowsController).Controller.View.InternalWordWrap)
				 return base.CanFitBoxToCurrentRow(boxSize);
			 else 
				 return true;		 
		 }
		 protected internal override int GetMaxBoxWidth() {
			 if (((SimpleViewRowsController)RowsController).Controller.View.InternalWordWrap)
				 return base.GetMaxBoxWidth();
			 else
				 return Int32.MaxValue / 2;		 
		 }
	}
	#endregion
	#region SimpleViewFloatingObjectSizeAndPositionController
	public class SimpleViewFloatingObjectSizeAndPositionController : FloatingObjectSizeAndPositionController {
		public SimpleViewFloatingObjectSizeAndPositionController(RowsController rowsController)
			: base(rowsController) {
		}
		protected override Rectangle ValidateRotatedShapeHorizontalPosition(Rectangle shapeBounds, FloatingObjectProperties properties) {
			shapeBounds.X = Math.Max(CalculatePlacementInfo(properties).OriginX, shapeBounds.X);
			return shapeBounds;
		}
		protected override Rectangle ValidateRotatedShapeVerticalPosition(Rectangle shapeBounds, FloatingObjectProperties properties) {
			shapeBounds.Y = Math.Max(CalculatePlacementInfo(properties).OriginY, shapeBounds.Y);
			return shapeBounds;
		}
	}
	#endregion
	#region SimpleViewTablesController
	public class SimpleViewTablesController : TablesController {
		public SimpleViewTablesController(PageController pageController, RowsController rowsController)
			: base(pageController, rowsController) {
		}
		protected internal override TableViewInfoManager CreateTableViewInfoManager(TableViewInfoManager parentTableViewInfoManager, PageController pageController, RowsController rowsController) {
			return new SimpleViewTableViewInfoManager(parentTableViewInfoManager, pageController, rowsController); ;
		}
		public override CanFitCurrentRowToColumnResult CanFitRowToColumn(int lastTextRowBottom, Column column) {
			if (RowsController.TablesController.IsInsideTable)
				return CanFitCurrentRowToColumnResult.RowFitted;
			Rectangle rowBounds = RowsController.CurrentRow.Bounds;
			rowBounds.Height = int.MaxValue - rowBounds.Y;
			IList<FloatingObjectBox> floatingObjects = RowsController.FloatingObjectsLayout.GetAllObjectsInRectangle(rowBounds);
			if (floatingObjects.Count > 0)
				return CanFitCurrentRowToColumnResult.RowFitted;
			return base.CanFitRowToColumn(lastTextRowBottom, column);
		}
	}
	#endregion
	#region SimpleViewTableViewInfoManager
	public class SimpleViewTableViewInfoManager : TableViewInfoManager {
		public SimpleViewTableViewInfoManager(TableViewInfoManager parentTableViewInfoManager, PageController pageController, RowsController rowsController)
			: base(parentTableViewInfoManager, pageController, rowsController) {
		}
		protected internal override List<HorizontalCellBordersInfo> GetSplitAnchorHorizontalCellBorders(TableCellVerticalAnchor splitAnchor) {
			return null;
		}
		public override void FixColumnOverflow() {
		}
	}
	#endregion
	#region SimpleViewPageController
	public class SimpleViewPageController : NonPrintViewPageControllerBase {
		#region Fields
		Size pageSize;
		int minPageWidth;
		#endregion
		public SimpleViewPageController(DocumentLayout documentLayout)
			: base(documentLayout) {
			ResetPageSize();
		}
		#region Properties
		protected internal Size PageSize { get { return pageSize; } }
		public int VirtualPageWidth {
			get { return pageSize.Width; }
			set {
				pageSize.Width = value;
			}
		}
		public int VirtualPageHeight {
			get { return pageSize.Height; }
			set {
				pageSize.Height = value;
			}
		}
		public int MinPageWidth { get { return minPageWidth; }
			set {
				minPageWidth = value;
			}
		}
		#endregion
		protected internal override PageBoundsCalculator CreatePageBoundsCalculator() {
			return new SimplePageBoundsCalculator(this);
		}
		protected internal override void FinalizePagePrimaryFormatting(Page page, bool documentEnded) {
			PageArea lastPageArea = page.Areas.Last;
			Column column = lastPageArea.Columns.First;
			Rectangle pageBounds = page.Bounds;
			Rectangle pageAreaBounds = lastPageArea.Bounds;
			Rectangle columnBounds = column.Bounds;
			int height = GetColumnBottom(column);
			pageBounds.Height = height;
			pageAreaBounds.Height = Math.Max(pageAreaBounds.Height, height);
			columnBounds.Height = Math.Max(columnBounds.Height, height);
			page.Bounds = pageBounds;
			page.ClientBounds = new Rectangle(Point.Empty, pageBounds.Size);
			lastPageArea.Bounds = pageAreaBounds;
			column.Bounds = columnBounds;
			base.FinalizePagePrimaryFormatting(page, documentEnded);
		}
		protected internal override BoxHitTestCalculator CreateHitTestCalculator(RichEditHitTestRequest request, RichEditHitTestResult result) {
			return new SimpleViewBoxHitTestCalculator(request, result);
		}
		internal void ResetPageSize() {
			this.VirtualPageHeight = DocumentLayout.UnitConverter.DocumentsToLayoutUnits(4800); 
			this.VirtualPageWidth = DocumentLayout.UnitConverter.DocumentsToLayoutUnits(900); 
			this.minPageWidth = VirtualPageWidth;
		}
	}
	#endregion
	#region MaxWidthCalculator
	public class MaxWidthCalculator {
		public int GetMaxWidth (Page page){
			int maxWidth = GetMaxWidth(page.Areas);
			if (page.FloatingObjects!= null)
				for (int i = 0; i < page.FloatingObjects.Count; i++) {
					maxWidth = Math.Max(page.FloatingObjects[i].Bounds.Right, maxWidth);
				}
			return maxWidth;
		}
		public int GetMaxWidth(PageAreaCollection areas) {
			int maxWidth = Int32.MinValue;
			int count = areas.Count;
			for (int i = 0; i < count; i++) {
				RowCollection rows = areas[i].Columns.First.Rows;
				maxWidth = Math.Max(maxWidth, GetMaxWidth(rows));
			}
			return maxWidth;
		}
		public int GetMaxWidth(RowCollection rows) {
			int maxWidth = 0;
			int count = rows.Count;
			for (int i = 0; i < count; i++) {				
				Box lastBox = GetLastNonLineBreakBox(rows[i]);
				if (lastBox != null)
					maxWidth = Math.Max(maxWidth, lastBox.Bounds.Right);
			}
			return maxWidth;
		}
		public int GetActualMaxWidth(RowCollection rows) {
			int maxWidth = 0;
			int count = rows.Count;
			for (int i = 0; i < count; i++) {
				Box lastBox = GetLastNonLineBreakOrSpaceBox(rows[i]);
				if (lastBox != null)
					maxWidth = Math.Max(maxWidth, lastBox.Bounds.Right - rows[i].Bounds.X);
			}
			return maxWidth;
		}
		Box GetLastNonLineBreakBox(Row row) {
			for (int i = row.Boxes.Count - 1; i >= 0; i--) {
				if (!row.Boxes[i].IsLineBreak)
					return row.Boxes[i];
			}
			return null;
		}
		Box GetLastNonLineBreakOrSpaceBox(Row row) {
			for (int i = row.Boxes.Count - 1; i >= 0; i--) {
				if (!row.Boxes[i].IsLineBreak && row.Boxes[i].IsNotWhiteSpaceBox)
					return row.Boxes[i];
			}
			return null;
		}
	}
	#endregion
	#region SimpleViewColumnController
	public class SimpleViewColumnController : ColumnController {
		public SimpleViewColumnController(PageAreaController pageAreaController)
			: base(pageAreaController) {
		}
		public override bool ShouldZeroSpacingBeforeWhenMoveRowToNextColumn { get { return false; } }
		protected internal override Rectangle CalculateColumnBoundsCore(int columnIndex) {
			return ColumnsBounds[0];
		}
		protected internal override ColumnsBoundsCalculator CreateColumnBoundsCalculator() {
			return new SimpleViewColumnsBoundsCalculator(PageAreaController.PageController.DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter);
		}
	}
	#endregion
	#region SimpleViewColumnsBoundsCalculator
	public class SimpleViewColumnsBoundsCalculator : ColumnsBoundsCalculator {
		public SimpleViewColumnsBoundsCalculator(DocumentModelUnitToLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected internal override void PopulateColumnsBounds(List<Rectangle> result, Rectangle bounds, ColumnInfoCollection columnInfoCollection) {
			result.Add(bounds);
		}
		protected internal override void PopulateEqualWidthColumnsBounds(List<Rectangle> result, Rectangle bounds, int columnCount, int spaceBetweenColumns) {
			result.Add(bounds);
		}
	}
	#endregion
	#region SimplePageBoundsCalculator
	public class SimplePageBoundsCalculator : PageBoundsCalculator {
		readonly SimpleViewPageController controller;
		public SimplePageBoundsCalculator(SimpleViewPageController controller)
			: base(controller.DocumentLayout.DocumentModel.ToDocumentLayoutUnitConverter) {
			Guard.ArgumentNotNull(controller, "controller");
			this.controller = controller;
		}
		protected internal override Rectangle CalculatePageBounds(Section section) {
			return CalculatePageClientBounds(section);
		}
		protected internal override Rectangle CalculatePageClientBoundsCore(int pageWidth, int pageHeight, int marginLeft, int marginTop, int marginRight, int marginBottom) {
			return new Rectangle(Point.Empty, controller.PageSize);
		}
	}
	#endregion
}
