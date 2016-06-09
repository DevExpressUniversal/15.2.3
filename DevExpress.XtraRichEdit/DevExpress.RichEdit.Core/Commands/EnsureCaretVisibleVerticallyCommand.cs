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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	#region EnsureCaretVisibleVerticallyCommand
	public class EnsureCaretVisibleVerticallyCommand : RichEditCaretBasedCommand {
		protected internal struct CurrentRowInfo {
			readonly Row row;
			readonly Rectangle boundsRelativeToPage;
			public CurrentRowInfo(Row row, Rectangle boundsRelativeToPage) {
				this.row = row;
				this.boundsRelativeToPage = boundsRelativeToPage;
			}
			public Row Row { get { return row; } }
			public Rectangle BoundsRelativeToPage { get { return boundsRelativeToPage; } }
		}
		#region Fields
		CurrentRowInfo currentRow;
		Page currentPage;
		PageViewInfo currentPageViewInfo;
		float relativeCaretPosition = -1;
		#endregion
		public EnsureCaretVisibleVerticallyCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_EnsureCaretVisibleVertically; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_EnsureCaretVisibleVerticallyDescription; } }
		protected internal virtual CurrentRowInfo CurrentRow { get { return currentRow; } }
		protected internal virtual PageViewInfo CurrentPageViewInfo { get { return currentPageViewInfo; } }
		protected internal virtual Page CurrentPage { get { return currentPage; } }
		public float RelativeCaretPosition {
			get { return relativeCaretPosition; }
			set {
				if (relativeCaretPosition == value)
					return;
				if (value >= 0)
					value = Math.Min(1.0f, value);
				relativeCaretPosition = value;
			}
		}
		#endregion
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			Control.UpdateControlAutoSize();
			UpdateCaretPosition(DocumentLayoutDetailsLevel.Row);
			if (CaretPosition.LayoutPosition.DetailsLevel >= DocumentLayoutDetailsLevel.Row) {
				bool redraw = false;
				InnerControl.BeginDocumentRendering();
				try {
					if (CaretPosition.PageViewInfo == null)
						UpdateCaretPosition(DocumentLayoutDetailsLevel.Row);
					if (!IsCaretVisible()) {
						ScrollToMakeCaretVisible();
						redraw = false;
					}
					else {
						if (RelativeCaretPosition >= 0) {
							ScrollToSetRelativeCaretPosition();
							redraw = true;
						}
					}
				}
				finally {
					InnerControl.EndDocumentRendering();
				}
				UpdateCaretPosition(DocumentLayoutDetailsLevel.Row);
				Debug.Assert(CaretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.Row));
				Debug.Assert(CaretPosition.PageViewInfo != null);
				if (redraw)
					InnerControl.Owner.Redraw();
			}
			Debug.Assert(CaretPosition.PageViewInfo != null);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = true;
			state.Visible = true;
			state.Checked = false;
		}
		protected internal virtual bool ShouldAdjustCaretPosition() {
			if (DocumentModel.Selection.End <= DocumentModel.Selection.Start)
				return false;
			return CaretPosition.LogPosition == CaretPosition.LayoutPosition.Row.GetFirstPosition(ActivePieceTable).LogPosition;
		}
		protected internal virtual void AdjustAndSaveCaretPosition() {
			if (ShouldAdjustCaretPosition()) {
				CaretPosition pos = CreateAdjustedCaretPosition();
				SaveCaretPosition(pos);
			}
			else
				SaveCaretPosition(CaretPosition);
		}
		protected internal virtual CaretPosition CreateAdjustedCaretPosition() {
			DocumentLogPosition logPosition = ActivePieceTable.NavigationVisibleTextFilter.GetPrevVisibleLogPosition(CaretPosition.LogPosition, false);
			CaretPosition pos = CaretPosition.CreateExplicitCaretPosition(logPosition);
			pos.Update(DocumentLayoutDetailsLevel.Row);
			return pos;
		}
		protected internal virtual void SaveCaretPosition(CaretPosition pos) {
			this.currentPage = pos.LayoutPosition.Page;
			this.currentRow = new CurrentRowInfo(pos.LayoutPosition.Row, pos.GetRowBoundsRelativeToPage());
			this.currentPageViewInfo = pos.PageViewInfo;
		}
		public enum CaretScrollDirection {
			None,
			Unknown,
			Up,
			Down
		}
		protected internal virtual CaretScrollDirection CalculateScrollDirectionToCaret(CaretPosition pos) {
			if (pos.PageViewInfo == null)
				return CaretScrollDirection.Unknown;
			Rectangle physicalCaretRowBounds = ActiveView.CreatePhysicalRectangle(pos.PageViewInfo,  pos.GetRowBoundsRelativeToPage());
			Rectangle viewBounds = ActiveView.Bounds;
			if (physicalCaretRowBounds.Y < viewBounds.Y) { 
				return CaretScrollDirection.Up;
			}
			if (physicalCaretRowBounds.Bottom > viewBounds.Bottom)
				return CaretScrollDirection.Down;
			return CaretScrollDirection.None;
		}
		protected internal bool IsCaretVisible() {
			CaretScrollDirection caretScrollDirection = CalculateScrollDirectionToCaret(CaretPosition);
			if (caretScrollDirection == CaretScrollDirection.Down || caretScrollDirection == CaretScrollDirection.Unknown) {
				SaveCaretPosition(CaretPosition);
				return false;
			}
			if (caretScrollDirection == CaretScrollDirection.Up) {
				AdjustAndSaveCaretPosition();
				return false;
			}
			Debug.Assert(caretScrollDirection == CaretScrollDirection.None);
			Debug.Assert(CaretPosition.PageViewInfo != null);
			if (!ShouldAdjustCaretPosition()) {
				SaveCaretPosition(CaretPosition);
				return true;
			}
			CaretPosition pos = CreateAdjustedCaretPosition();
			caretScrollDirection = CalculateScrollDirectionToCaret(pos);
			if (caretScrollDirection == CaretScrollDirection.None) {
				SaveCaretPosition(CaretPosition);
				return true;
			}
			else {
				if (caretScrollDirection == CaretScrollDirection.Up || caretScrollDirection == CaretScrollDirection.Down) {
					if (BothPositionsWillValid(CaretPosition, pos))
						SaveCaretPosition(pos);
					else
						SaveCaretPosition(CaretPosition);
				}
				else
					SaveCaretPosition(CaretPosition);
				return false;
			}
		}
		protected internal virtual bool BothPositionsWillValid(CaretPosition currentCaretPosition, CaretPosition adjustedCaretPosition) {
			if (adjustedCaretPosition.PageViewInfo == null)
				return false;
			Rectangle physicalCaretRowBounds = ActiveView.CreatePhysicalRectangle(currentCaretPosition.PageViewInfo, currentCaretPosition.GetRowBoundsRelativeToPage());
			Rectangle physicalAdjustedCaretRowBounds = ActiveView.CreatePhysicalRectangle(adjustedCaretPosition.PageViewInfo, adjustedCaretPosition.GetRowBoundsRelativeToPage());
			Rectangle viewBounds = ActiveView.Bounds;
			Debug.Assert(physicalAdjustedCaretRowBounds.Top < viewBounds.Top);
			return physicalCaretRowBounds.Top + viewBounds.Top - physicalAdjustedCaretRowBounds.Top <= viewBounds.Bottom;
		}
		protected internal virtual void ScrollToMakeCaretVisible() {			
			ActiveView.VerticalScrollController.UpdateScrollBar();
			PageViewInfoGenerator pageViewInfoGenerator = ActiveView.PageViewInfoGenerator;
			if (pageViewInfoGenerator.TopInvisibleHeight >= pageViewInfoGenerator.TotalHeight)
				ScrollToAlreadyFormattedContentCore(CurrentRow.BoundsRelativeToPage.Top);
			else {
				int offset = CalculatePhysicalOffsetToCaret();
				if (offset < 0) 
					offset += CalculateOffsetToVisibleArea(0);
				else 
					offset += CalculateOffsetToVisibleArea(ActiveView.Bounds.Height - ActiveView.CreatePhysicalRectangle(new Rectangle(0, 0, 100, 100), CurrentRow.BoundsRelativeToPage).Height);
				ScrollVerticallyByPhysicalOffset(offset);
			}
		}
		protected internal virtual void ScrollToAlreadyFormattedContentCore(int verticalOffset) {
			ActiveView.ResetPages(PageGenerationStrategyType.FirstPageOffset);
			if (CurrentPageViewInfo != null) {
				Rectangle physicalCaretRowBounds = ActiveView.CreatePhysicalRectangle(CurrentPageViewInfo, new Rectangle(0, verticalOffset, 100, 100));
				int pageIndex = ActiveView.FormattingController.PageController.Pages.IndexOf(CurrentPage);
				Debug.Assert(pageIndex >= 0);
				PageViewInfoGenerator pageViewInfoGenerator = ActiveView.PageViewInfoGenerator;
				pageViewInfoGenerator.FirstPageOffsetAnchor.PageIndex = pageIndex;
				int offset = CalculateOffsetToVisibleArea(physicalCaretRowBounds.Y);
				pageViewInfoGenerator.FirstPageOffsetAnchor.VerticalOffset = Math.Max(0, pageViewInfoGenerator.FirstPageOffsetAnchor.VerticalOffset + offset);
			}
			ActiveView.GeneratePages();
		}
		protected internal virtual void ScrollToSetRelativeCaretPosition() {
			if (CurrentPageViewInfo == null)
				return;
			Rectangle physicalCaretRowBounds = ActiveView.CreatePhysicalRectangle(CurrentPageViewInfo, currentRow.BoundsRelativeToPage);
			int offset = CalculateOffsetToVisibleArea(physicalCaretRowBounds.Y);
			if (offset == 0)
				return;
			ActiveView.VerticalScrollController.UpdateScrollBar();
			ScrollVerticallyByPhysicalOffset(offset);
		}
		protected internal virtual int CalculateOffsetToVisibleArea(int initialCaretPhysicalTop) {
			if (RelativeCaretPosition < 0)
				return 0;
			Rectangle viewBounds = ActiveView.Bounds;
			Rectangle physicalCaretRowBounds = ActiveView.CreatePhysicalRectangle(new Rectangle(0, 0, 100, 100), currentRow.BoundsRelativeToPage);
			viewBounds.Height -= physicalCaretRowBounds.Height;
			if (viewBounds.Height <= 0)
				return 0;
			int targetY = (int)Math.Round(viewBounds.Height * RelativeCaretPosition);
			return initialCaretPhysicalTop - targetY;
		}
		protected internal virtual int CalculatePhysicalOffsetToCaret() {
			if (CurrentPageViewInfo != null)
				return CalculatePhysicalOffsetToCaretAtVisiblePage();
			else
				return CalculatePhysicalOffsetToCaretAtInvisiblePage();
		}
		protected internal virtual SelectionMovementVerticalDirection CalculateDirectionToInvisibleCaret() {
			PageCollection pages = ActiveView.FormattingController.PageController.Pages;
			int caretPageIndex = pages.IndexOf(CurrentPage);
			NextCaretPositionLineUpCalculator calculator = new NextCaretPositionLineUpCalculator(Control);
			calculator.FixedDirection = false;
			int firstVisiblePageIndex = calculator.CalculateFirstInvisiblePageIndex() + 1;
			Debug.Assert(caretPageIndex >= 0);
			Debug.Assert(firstVisiblePageIndex >= 0);
			Debug.Assert(caretPageIndex != firstVisiblePageIndex);
			if (caretPageIndex < firstVisiblePageIndex)
				return SelectionMovementVerticalDirection.Up;
			else
				return SelectionMovementVerticalDirection.Down;
		}
		protected internal virtual int CalculatePhysicalOffsetToCaretAtVisiblePage() {
			return CalculatePhysicalOffsetForCaretPositionRowVisibilityCore(CurrentPageViewInfo, CurrentRow);
		}
		protected internal virtual int CalculatePhysicalOffsetToCaretAtInvisiblePage() {
			SelectionMovementVerticalDirection direction = CalculateDirectionToInvisibleCaret();
			PageViewInfo pageViewInfo = LookupInvisiblePageWithCaret(direction);
			pageViewInfo.Bounds = CalculateActualInvisiblePageBounds(pageViewInfo, direction);
			ActiveView.PageViewInfoGenerator.UpdatePageClientBounds(pageViewInfo);
			return CalculatePhysicalOffsetForCaretPositionRowVisibilityCore(pageViewInfo, CurrentRow);
		}
		protected internal virtual PageViewInfo LookupInvisiblePageWithCaret(SelectionMovementVerticalDirection direction) {
			NextCaretPositionVerticalDirectionCalculator calculator;
			if (direction == SelectionMovementVerticalDirection.Up)
				calculator = new NextCaretPositionLineUpCalculator(Control);
			else
				calculator = new NextCaretPositionLineDownCalculator(Control);
			calculator.FixedDirection = false;
			InvisiblePageRowsGenerator generator = calculator.CreateInvisiblePageRowsGenerator();
			for (; ; ) {
				PageViewInfoRow row = generator.GenerateNextRow();
				if (row == null)
					Exceptions.ThrowInternalException();
				int count = row.Count;
				for (int i = 0; i < count; i++) {
					PageViewInfo pageViewInfo = row[i];
					if (pageViewInfo.Page == CurrentPage)
						return pageViewInfo;
				}
			}
		}
		protected internal virtual Rectangle CalculateActualInvisiblePageBounds(PageViewInfo pageViewInfo, SelectionMovementVerticalDirection direction) {
			int pageOffset;
			if (direction == SelectionMovementVerticalDirection.Down)
				pageOffset = ActiveView.PageViewInfoGenerator.ActiveGenerator.PageRows.Last.Bounds.Bottom;
			else
				pageOffset = ActiveView.PageViewInfoGenerator.ActiveGenerator.PageRows.First.Bounds.Top - 2 * pageViewInfo.Bounds.Bottom + pageViewInfo.Bounds.Height;
			return ActiveView.PageViewInfoGenerator.OffsetRectangle(pageViewInfo.Bounds, 0, pageOffset);
		}
		protected internal virtual int CalculatePhysicalOffsetForCaretPositionRowVisibilityCore(PageViewInfo pageViewInfo, CurrentRowInfo row) {
			Rectangle logicalRowBounds = CalculateTargetRowLogicalBounds(pageViewInfo, row);
			Rectangle physicalRowBounds = ActiveView.CreatePhysicalRectangle(pageViewInfo, logicalRowBounds);
			return CalculatePhysicalOffsetForRowBoundsVisibility(ActiveView.Bounds, physicalRowBounds);
		}
		protected internal virtual int CalculatePhysicalOffsetForRowBoundsVisibility(Rectangle viewBounds, Rectangle physicalRowBounds) {
			if (viewBounds.Y <= physicalRowBounds.Y && physicalRowBounds.Bottom <= viewBounds.Bottom)
				return 0;
			if (physicalRowBounds.Y < viewBounds.Y)
				return physicalRowBounds.Y - viewBounds.Y;
			else
				return physicalRowBounds.Bottom - viewBounds.Bottom;
		}
		protected internal virtual void ScrollVerticallyByPhysicalOffset(int offset) {
			if (offset != 0) {
				ScrollVerticallyByPhysicalOffsetEnsurePageGenerationCommand command = new ScrollVerticallyByPhysicalOffsetEnsurePageGenerationCommand(Control);
				command.PhysicalOffset = offset;
				command.Execute();
			}
		}
		protected internal virtual Rectangle CalculateTargetRowLogicalBounds(PageViewInfo pageViewInfo, CurrentRowInfo row) {
			return ValidateRowBounds(pageViewInfo, row.BoundsRelativeToPage);
		}
		protected internal virtual Rectangle ValidateRowBounds(PageViewInfo pageViewInfo, Rectangle rowBounds) {
			int pageBottom = pageViewInfo.Page.Bounds.Bottom;
			if (rowBounds.Bottom > pageBottom)
				rowBounds.Height = pageBottom - rowBounds.Top;
			return rowBounds;
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands {
	#region SelectionMovementVerticalDirection
	public enum SelectionMovementVerticalDirection {
		Up,
		Down
	}
	#endregion
}
