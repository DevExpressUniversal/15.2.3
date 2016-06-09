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
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Windows.Forms;
namespace DevExpress.XtraRichEdit.Mouse {
	#region IEndDocumentUpdateHandler
	public interface IEndDocumentUpdateHandler {
		void HandleEndDocumentUpdate(DocumentUpdateCompleteEventArgs e);
	}
	#endregion
	#region BoxAccessor
	public class BoxAccessor {
		static readonly BoxAccessor box = new BoxAccessor();
		static readonly RowBoxAccessor row = new RowBoxAccessor();
		static readonly CharacterBoxAccessor character = new CharacterBoxAccessor();
		public static BoxAccessor Box { get { return box; } }
		public static RowBoxAccessor Row { get { return row; } }
		public static CharacterBoxAccessor Character { get { return character; } }
		public virtual Box GetBox(RichEditHitTestResult hitTestResult) {
			return hitTestResult.Box;
		}
	}
	#endregion
	#region RowBoxAccessor
	public class RowBoxAccessor : BoxAccessor {
		public override Box GetBox(RichEditHitTestResult hitTestResult) {
			return hitTestResult.Row;
		}
	}
	#endregion
	#region CharacterBoxAccessor
	public class CharacterBoxAccessor : BoxAccessor {
		public override Box GetBox(RichEditHitTestResult hitTestResult) {
			return hitTestResult.Character;
		}
	}
	#endregion
	#region ContinueSelectionByRangesMouseHandlerState (abstract class)
	public abstract class ContinueSelectionByRangesMouseHandlerState : RichEditMouseHandlerState, IEndDocumentUpdateHandler {
		#region Fields
		BoxAccessor initialBoxAccessor;
		TableCell startCell;
		bool isInvalidInitialHitTestResult;
		RichEditHitTestResult initialHitTestResult;
		#endregion
		protected ContinueSelectionByRangesMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler) {
			Guard.ArgumentNotNull(initialBoxAccessor, "initialBoxAccessor");
			Guard.ArgumentNotNull(initialHitTestResult, "initialHitTestResult");
			this.initialBoxAccessor = initialBoxAccessor;
			this.initialHitTestResult = initialHitTestResult;
			this.startCell = CalculateStartCell();
		}
		#region Properties
		public override bool AutoScrollEnabled { get { return true; } }
		public override bool CanShowToolTip { get { return true; } }
		public override bool StopClickTimerOnStart { get { return false; } }
		protected internal Box InitialBox { get { return initialBoxAccessor.GetBox(InitialHitTestResult); } }
		protected internal BoxAccessor InitialBoxAccessor { get { return initialBoxAccessor; } }
		protected internal RichEditHitTestResult InitialHitTestResult {
			get {
				if (isInvalidInitialHitTestResult) {
					EnforceFormattingComplete();
					this.initialHitTestResult = GetHitTestResult(initialHitTestResult.PhysicalPoint);
					this.isInvalidInitialHitTestResult = false;
				}
				return initialHitTestResult;
			}
		}
		protected internal TableCell StartCell { get { return startCell; } set { startCell = value; } }
		#endregion
		void EnforceFormattingComplete() {
			Control.InnerControl.ActiveView.EnforceFormattingCompleteForVisibleArea();
		}
		protected internal virtual TableCell CalculateStartCell() {
			DocumentLogPosition logPosition = InitialBox.GetFirstPosition(ActivePieceTable).LogPosition;
			return ActivePieceTable.FindParagraph(logPosition).GetCell();
		}
		public override void Start() {
			base.Start();
			DocumentModel.EndDocumentUpdate += OnEndDocumentUpdate;
		}
		public override void Finish() {
			base.Finish();
			DocumentModel.EndDocumentUpdate -= OnEndDocumentUpdate;
			if (ChangeFontBackColorByMouseCommand.IsChangeByMouse)
				new ChangeFontBackColorByMouseCommand(Control).Execute();
		}
		void IEndDocumentUpdateHandler.HandleEndDocumentUpdate(DocumentUpdateCompleteEventArgs e) {
			this.OnEndDocumentUpdate(this, e);
		}
		void OnEndDocumentUpdate(object sender, DocumentUpdateCompleteEventArgs e) {
			DocumentModelChangeActions changeActions = e.DeferredChanges.ChangeActions;
			if ((changeActions & DocumentModelChangeActions.ResetPrimaryLayout) != 0 ||
				(changeActions & DocumentModelChangeActions.ResetAllPrimaryLayout) != 0) {
				this.isInvalidInitialHitTestResult = true;
			}
		}
		public override void OnMouseUp(MouseEventArgs e) {
			MouseHandler.SwitchToDefaultState();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			ContinueSelection(e);
			SetMouseCursor(CalculateCursor(e));
		}
		protected internal virtual RichEditCursor CalculateCursor(MouseEventArgs e) {
			MouseCursorCalculator calculator = Control.InnerControl.CreateMouseCursorCalculator();
			Point physicalPoint = new Point(e.X, e.Y);
			return calculator.Calculate(CalculateNearestCharacterHitTest(physicalPoint, ActivePieceTable), physicalPoint);
		}
		public override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			ContinueSelection(e);
		}
		protected internal virtual void ContinueSelection(MouseEventArgs e) {
			ExtendSelectionByRangesCommandBase command = CreateExtendSelectionCommand();
			command.InitialBox = this.InitialBox;
			command.PhysicalPoint = new Point(e.X, e.Y);
			command.Execute();
		}
		protected internal virtual RichEditHitTestResult GetHitTestResult(Point physicalPoint) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(ActivePieceTable);
			request.PhysicalPoint = physicalPoint;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Character;
			request.Accuracy = HitTestAccuracy.NearestCharacter;
			return Control.InnerControl.ActiveView.HitTestCore(request, false);
		}
		protected internal virtual bool NeedSwitchStateToTableCellsMouseHandlerState(Point physicalPoint) {
			Paragraph currentParagraph = GetParagraph(physicalPoint);
			if (StartCell == null || currentParagraph == null || currentParagraph.GetCell() == null)
				return false;
			return NeedSwitchStateToTableCellsMouseHandlerStateCore(currentParagraph);
		}
		protected internal virtual bool NeedSwitchStateToTableCellsMouseHandlerStateCore(Paragraph currentParagraph) {
			TableCellsManager cellManager = ActivePieceTable.TableCellsManager;
			TableCell actualCurrentCell = cellManager.GetCellByNestingLevel(currentParagraph.Index, StartCell.Table.NestedLevel);
			TableCell cellConsiderNestedLevel = StartCell;
			if (actualCurrentCell.Table.NestedLevel != StartCell.Table.NestedLevel)
				cellConsiderNestedLevel = cellManager.GetCellByNestingLevel(StartCell.StartParagraphIndex, actualCurrentCell.Table.NestedLevel);
			return cellConsiderNestedLevel != actualCurrentCell && cellConsiderNestedLevel.Table == actualCurrentCell.Table;
		}
		protected internal virtual TableCell GetCurrentCell(Point physicalPoint) {
			Paragraph paragraph = GetParagraph(physicalPoint);
			if (paragraph == null)
				return null;
			return paragraph.GetCell();
		}
		protected internal virtual Paragraph GetParagraph(Point physicalPoint) {
			RichEditHitTestRequest request = new RichEditHitTestRequest(ActivePieceTable);
			request.PhysicalPoint = physicalPoint;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Character;
			request.Accuracy = HitTestAccuracy.NearestCharacter;
			RichEditHitTestResult hitTestResult = Control.InnerControl.ActiveView.HitTestCore(request, false);
			if (!hitTestResult.IsValid(DocumentLayoutDetailsLevel.Character))
				return null;
			DocumentLogPosition logPosition = hitTestResult.Character.GetFirstPosition(ActivePieceTable).LogPosition;
			return ActivePieceTable.FindParagraph(logPosition);
		}
		protected internal abstract ExtendSelectionByRangesCommandBase CreateExtendSelectionCommand();
	}
	#endregion
	#region ContinueSelectionByCharactersMouseHandlerState
	public class ContinueSelectionByCharactersMouseHandlerState : ContinueSelectionByRangesMouseHandlerState {
		public ContinueSelectionByCharactersMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
		}
		protected internal override ExtendSelectionByRangesCommandBase CreateExtendSelectionCommand() {
			return new ExtendSelectionByCharactersCommand(Control);
		}
		protected internal override void ContinueSelection(MouseEventArgs e) {
			Point physicalPoint = new Point(e.X, e.Y);
			TableCell currentCell = GetCurrentCell(physicalPoint);
			if (NeedSwitchStateToTableCellsMouseHandlerState(physicalPoint)) {
				ContinueSelectionByTableCellsMouseHandlerState cellsMouseHandlerState = new ContinueSelectionByTableCellsMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult);
				cellsMouseHandlerState.StartCell = ActivePieceTable.TableCellsManager.GetCellByNestingLevel(StartCell.StartParagraphIndex, currentCell.Table.NestedLevel);
				MouseHandler.SwitchStateCore(cellsMouseHandlerState, physicalPoint);
				return;
			}
			if ((currentCell != null && StartCell == null) || StartCell != null && currentCell != null && StartCell.Table.NestedLevel < currentCell.Table.NestedLevel) {
				RichEditHitTestResult newHitTestResult = GetHitTestResult(physicalPoint);
				if ((newHitTestResult.Accuracy & HitTestAccuracy.ExactTableRow) == 0)
					return;
				int nestingLevel = (StartCell == null) ? 0 : newHitTestResult.TableRow.Row.Table.NestedLevel;
				MouseHandler.SwitchStateCore(new ContinueSelectionByTableRowsAfterCharactersMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult, nestingLevel), physicalPoint);
				return;
			}
			base.ContinueSelection(e);
		}
	}
	#endregion
	#region ContinueSelectionByWordsAndParagraphsMouseHandlerStateBase (abstract class)
	public abstract class ContinueSelectionByWordsAndParagraphsMouseHandlerStateBase : ContinueSelectionByRangesMouseHandlerState {
		protected ContinueSelectionByWordsAndParagraphsMouseHandlerStateBase(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
		}
		protected internal override void ContinueSelection(MouseEventArgs e) {
			Point physicalPoint = new Point(e.X, e.Y);
			TableCell currentCell = GetCurrentCell(physicalPoint);
			if ((currentCell != null && StartCell == null) || StartCell != null && currentCell != null && StartCell.Table.NestedLevel != currentCell.Table.NestedLevel) {
				RichEditHitTestResult newHitTestResult = GetHitTestResult(physicalPoint);
				if (newHitTestResult.TableRow != null) {
					int nestingLevel = newHitTestResult.TableRow.Row.Table.NestedLevel;
					MouseHandler.SwitchStateCore(GetNewMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult, nestingLevel), physicalPoint);
					return;
				}
			}
			if (currentCell != null && StartCell != null && currentCell != StartCell) { 
				MouseHandler.SwitchStateCore(new ContinueSelectionByTableCellsMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult), physicalPoint);
				return;
			}
			base.ContinueSelection(e);
		}
		protected internal abstract ContinueSelectionByTableRowsMouseHandlerStateBase GetNewMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult, int nestingLevel);
	}
	#endregion
	#region ContinueSelectionByWordsMouseHandlerState
	public class ContinueSelectionByWordsMouseHandlerState : ContinueSelectionByWordsAndParagraphsMouseHandlerStateBase {
		public ContinueSelectionByWordsMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
		}
		protected internal override ExtendSelectionByRangesCommandBase CreateExtendSelectionCommand() {
			return new ExtendSelectionByWordsCommand(Control);
		}
		protected internal override ContinueSelectionByTableRowsMouseHandlerStateBase GetNewMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult, int nestingLevel) {
			return new ContinueSelectionByTableRowsAfterWordsMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult, nestingLevel);
		}
	}
	#endregion
	#region ContinueSelectionByParagraphsMouseHandlerState
	public class ContinueSelectionByParagraphsMouseHandlerState : ContinueSelectionByWordsAndParagraphsMouseHandlerStateBase {
		public ContinueSelectionByParagraphsMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
		}
		protected internal override ExtendSelectionByRangesCommandBase CreateExtendSelectionCommand() {
			return new ExtendSelectionByParagraphsCommand(Control);
		}
		protected internal override ContinueSelectionByTableRowsMouseHandlerStateBase GetNewMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult, int nestingLevel) {
			return new ContinueSelectionByTableRowsAfterParagraphsMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult, nestingLevel);
		}
	}
	#endregion
	#region ContinueSelectionByLinesMouseHandlerState
	public class ContinueSelectionByLinesMouseHandlerState : ContinueSelectionByRangesMouseHandlerState {
		public ContinueSelectionByLinesMouseHandlerState(RichEditMouseHandler mouseHandler, RowBoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
		}
		protected internal override RichEditCursor CalculateCursor(MouseEventArgs e) {
			return RichEditCursors.SelectRow;
		}
		protected internal override ExtendSelectionByRangesCommandBase CreateExtendSelectionCommand() {
			return new ExtendSelectionByLinesCommand(Control);
		}
		protected internal override void ContinueSelection(MouseEventArgs e) {
			if (StartCell != null)
				base.ContinueSelection(e);
			Point physicalPoint = new Point(e.X, e.Y);
			RichEditHitTestResult newHitTestResult = GetHitTestResult(physicalPoint);
			if (newHitTestResult.TableRow != null) {
				int nestingLevel = newHitTestResult.TableRow.Row.Table.NestedLevel;
				MouseHandler.SwitchStateCore(new ContinueSelectionByTableRowsAfterRowMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult, nestingLevel), physicalPoint);
			}
			else
				base.ContinueSelection(e);
		}
	}
	#endregion
	#region ContinueSelectionByTableCellsMouseHandlerStateBase (abstract class)
	public abstract class ContinueSelectionByTableCellsMouseHandlerStateBase : ContinueSelectionByRangesMouseHandlerState {
		protected ContinueSelectionByTableCellsMouseHandlerStateBase(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
		}
		protected internal override ExtendSelectionByRangesCommandBase CreateExtendSelectionCommand() {
			return new ExtendSelectionByCellsCommand(Control, StartCell);
		}
		protected internal override void ContinueSelection(MouseEventArgs e) {
			Point mousePosition = new Point(e.X, e.Y);
			if (NeedSwitchState(mousePosition)) {
				ChangeSelection();
				ContinueSelectionByCharactersMouseHandlerState newState = new ContinueSelectionByCharactersMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult);
				MouseHandler.SwitchStateCore(newState, mousePosition);
				ValidateSelection();
				newState.ContinueSelection(e);
				return;
			}
			base.ContinueSelection(e);
		}
		protected internal virtual bool NeedSwitchState(Point physicalPoint) {
			TableCell currentCell = GetCurrentCell(physicalPoint);
			return currentCell == null || StartCell.Table.NestedLevel > currentCell.Table.NestedLevel;
		}
		protected abstract void ChangeSelection();
		void ValidateSelection() {
			ExtendSelectionByRangesCommandBase command = CreateExtendSelectionCommand();
			command.ValidateSelection();
		}
	}
	#endregion
	#region ContinueSelectionByTableCellsMouseHandlerState
	public class ContinueSelectionByTableCellsMouseHandlerState : ContinueSelectionByTableCellsMouseHandlerStateBase {
		public ContinueSelectionByTableCellsMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
		}
		protected internal override bool NeedSwitchState(Point physicalPoint) {
			return base.NeedSwitchState(physicalPoint) || StartCell == GetCurrentCell(physicalPoint);
		}
		protected override void ChangeSelection() {
			Selection selection = DocumentModel.Selection;
			selection.ClearSelectionInTable();
			DocumentLogPosition oldStartLogPosition = selection.SelectedCells.OriginalStartLogPosition;
			selection.SetStartCell(oldStartLogPosition);
			selection.Start = oldStartLogPosition;
		}
	}
	#endregion
	#region ContinueSelectionByStartTableCellsMouseHandlerState
	public class ContinueSelectionByStartTableCellsMouseHandlerState : ContinueSelectionByTableCellsMouseHandlerStateBase {
		public ContinueSelectionByStartTableCellsMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
		}
		protected internal override RichEditCursor CalculateCursor(MouseEventArgs e) {
			return RichEditCursors.SelectTableCell;
		}
		protected override void ChangeSelection() {
		}
	}
	#endregion
	#region ContinueSelectionByTableRowsMouseHandlerStateBase
	public abstract class ContinueSelectionByTableRowsMouseHandlerStateBase : ContinueSelectionByRangesMouseHandlerState {
		#region Fields
		int nestingLevel;
		#endregion
		protected ContinueSelectionByTableRowsMouseHandlerStateBase(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult, int nestingLevel)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
			this.nestingLevel = nestingLevel;
		}
		#region Properties
		public int NestingLevel { get { return nestingLevel; } }
		#endregion
		protected internal override RichEditCursor CalculateCursor(MouseEventArgs e) {
			return RichEditCursors.SelectRow;
		}
		protected internal override ExtendSelectionByRangesCommandBase CreateExtendSelectionCommand() {
			ExtendSelectionByTableRowsCommand command = new ExtendSelectionByTableRowsCommand(Control, NestingLevel);
			DocumentLogPosition initialBoxLogPosition = InitialBox.GetFirstPosition(ActivePieceTable).LogPosition;
			if (StartCell == null)
				command.StartLogPosition = initialBoxLogPosition;
			else
				command.StartLogPosition = DocumentModel.Selection.SelectedCells.OriginalStartLogPosition;
			command.EndLogPosition = DocumentModel.Selection.NormalizedEnd;
			return command;
		}
		protected internal override void ContinueSelection(MouseEventArgs e) {
			Point mousePosition = new Point(e.X, e.Y);
			if (NeedSwitchState(mousePosition)) {
				SwitchState(mousePosition);
				return;
			}
			base.ContinueSelection(e);
		}
		protected internal virtual bool NeedSwitchState(Point mousePosition) {
			TableCell currentCell = GetCurrentCell(mousePosition);
			if (currentCell == null || StartCell != null && StartCell.Table == currentCell.Table)
				return true;
			return false;
		}
		protected internal abstract void SwitchState(Point mousePosition);
	}
	#endregion
	#region ContinueSelectionByTableRowsAfterCharactersMouseHandlerState
	public class ContinueSelectionByTableRowsAfterCharactersMouseHandlerState : ContinueSelectionByTableRowsMouseHandlerStateBase {
		public ContinueSelectionByTableRowsAfterCharactersMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult, int nestingLevel)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult, nestingLevel) {
		}
		protected internal override void SwitchState(Point mousePosition) {
			ContinueSelectionByCharactersMouseHandlerState byLinesState = new ContinueSelectionByCharactersMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult);
			MouseHandler.SwitchStateCore(byLinesState, mousePosition);
		}
	}
	#endregion
	#region ContinueSelectionByTableRowsAfterRowMouseHandlerState
	public class ContinueSelectionByTableRowsAfterRowMouseHandlerState : ContinueSelectionByTableRowsMouseHandlerStateBase {
		public ContinueSelectionByTableRowsAfterRowMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult, int nestingLevel)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult, nestingLevel) {
		}
		protected internal override void SwitchState(Point mousePosition) {
			ContinueSelectionByLinesMouseHandlerState byLinesState = new ContinueSelectionByLinesMouseHandlerState(MouseHandler, (RowBoxAccessor)InitialBoxAccessor, InitialHitTestResult);
			MouseHandler.SwitchStateCore(byLinesState, mousePosition);
		}
	}
	#endregion
	#region ContinueSelectionByTableRowsAfterWordsMouseHandlerState
	public class ContinueSelectionByTableRowsAfterWordsMouseHandlerState : ContinueSelectionByTableRowsMouseHandlerStateBase {
		public ContinueSelectionByTableRowsAfterWordsMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult, int nestingLevel)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult, nestingLevel) {
		}
		protected internal override RichEditCursor CalculateCursor(MouseEventArgs e) {
			return RichEditCursors.IBeam;
		}
		protected internal override void SwitchState(Point mousePosition) {
			ContinueSelectionByWordsMouseHandlerState byWordsState = new ContinueSelectionByWordsMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult);
			MouseHandler.SwitchStateCore(byWordsState, mousePosition);
		}
	}
	#endregion
	#region ContinueSelectionByTableRowsAfterParagraphsMouseHandlerState
	public class ContinueSelectionByTableRowsAfterParagraphsMouseHandlerState : ContinueSelectionByTableRowsMouseHandlerStateBase {
		public ContinueSelectionByTableRowsAfterParagraphsMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult, int nestingLevel)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult, nestingLevel) {
		}
		protected internal override void SwitchState(Point mousePosition) {
			ContinueSelectionByParagraphsMouseHandlerState byParagraphs = new ContinueSelectionByParagraphsMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult);
			MouseHandler.SwitchStateCore(byParagraphs, mousePosition);
		}
	}
	#endregion
	#region ContinueSelectionByStartTableRowsMouseHandlerState
	public class ContinueSelectionByStartTableRowsMouseHandlerState : ContinueSelectionByTableRowsMouseHandlerStateBase {
		#region Fields
		int startRowIndex;
		#endregion
		public ContinueSelectionByStartTableRowsMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult, initialHitTestResult.TableRow.Row.Table.NestedLevel) {
		}
		#region Properties
		public int StartRowIndex { get { return startRowIndex; } set { startRowIndex = value; } }
		#endregion
		protected internal override ExtendSelectionByRangesCommandBase CreateExtendSelectionCommand() {
			ExtendSelectionByStartTableRowsCommand command = new ExtendSelectionByStartTableRowsCommand(Control, NestingLevel);
			command.StartRowIndex = StartRowIndex;
			return command;
		}
		protected internal override bool NeedSwitchState(Point mousePosition) {
			TableCell currentCell = GetCurrentCell(mousePosition);
			if (currentCell == null || (StartCell != null && currentCell.Table != StartCell.Table))
				return true;
			return false;
		}
		protected internal override void SwitchState(Point mousePosition) {
			ContinueSelectionByCharactersMouseHandlerState byCharactersState = new ContinueSelectionByCharactersMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult);
			MouseHandler.SwitchStateCore(byCharactersState, mousePosition);
		}
	}
	#endregion
	#region ContinueSelectionByTableColumnsMouseHandlerState
	public class ContinueSelectionByTableColumnsMouseHandlerState : ContinueSelectionByRangesMouseHandlerState {
		#region Fields
		int startColumnIndex;
		int nestedLevel;
		#endregion
		public ContinueSelectionByTableColumnsMouseHandlerState(RichEditMouseHandler mouseHandler, BoxAccessor initialBoxAccessor, RichEditHitTestResult initialHitTestResult)
			: base(mouseHandler, initialBoxAccessor, initialHitTestResult) {
		}
		#region Properties
		public int StartColumnIndex { get { return startColumnIndex; } set { startColumnIndex = value; } }
		public int NestedLevel { get { return nestedLevel; } set { nestedLevel = value; } }
		#endregion
		protected internal override RichEditCursor CalculateCursor(MouseEventArgs e) {
			return RichEditCursors.SelectTableColumn;
		}
		protected internal override ExtendSelectionByRangesCommandBase CreateExtendSelectionCommand() {
			return new ExtendSelectionByTableColumnsCommand(Control, StartColumnIndex, NestedLevel);
		}
		protected internal override void ContinueSelection(MouseEventArgs e) {
			Point mousePosition = new Point(e.X, e.Y);
			TableCell currentCell = GetCurrentCell(mousePosition);
			if (currentCell == null) {
				ContinueSelectionByCharactersMouseHandlerState byCharacterState = new ContinueSelectionByCharactersMouseHandlerState(MouseHandler, InitialBoxAccessor, InitialHitTestResult);
				MouseHandler.SwitchStateCore(byCharacterState, mousePosition);
				return;
			}
			base.ContinueSelection(e);
		}
	}
	#endregion
}
