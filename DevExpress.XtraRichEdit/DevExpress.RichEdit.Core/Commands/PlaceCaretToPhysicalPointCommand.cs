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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using ModelUnit = System.Int32;
using LayoutUnit = System.Int32;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands.Internal {
	#region PlaceCaretToPhysicalPointCommand
	public class PlaceCaretToPhysicalPointCommand : RichEditCaretBasedCommand {
		Point physicalPoint;
		bool updateCaretX = true;
		bool suppressClearOutdatedSelectionItems;
		public PlaceCaretToPhysicalPointCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_PlaceCaretToPhysicalPoint; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PlaceCaretToPhysicalPointDescription; } }
		public Point PhysicalPoint { get { return physicalPoint; } set { physicalPoint = value; } }
		public bool UpdateCaretX { get { return updateCaretX; } set { updateCaretX = value; } }
		public bool SuppressClearOutdatedSelectionItems { get { return suppressClearOutdatedSelectionItems; } set { suppressClearOutdatedSelectionItems = value; } }
		protected internal virtual bool ExtendSelection { get { return false; } }
		protected internal virtual bool HitTestOnlyInPageClientBounds { get { return true; } }
		#endregion
		protected internal override void ExecuteCore() {
			CheckExecutedAtUIThread();
			RichEditHitTestRequest request = new RichEditHitTestRequest(DocumentModel.ActivePieceTable);
			request.PhysicalPoint = PhysicalPoint;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Character;
			request.Accuracy = HitTestAccuracy.NearestPage | HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestTableRow | HitTestAccuracy.NearestTableCell | HitTestAccuracy.NearestRow | HitTestAccuracy.NearestBox | HitTestAccuracy.NearestCharacter;
			if (HitTestOnlyInPageClientBounds)
				request.Accuracy |= ActiveView.DefaultHitTestPageAccuracy;
			RichEditHitTestResult hitTestResult = ActiveView.HitTestCore(request, HitTestOnlyInPageClientBounds);
			if (!hitTestResult.IsValid(DocumentLayoutDetailsLevel.Character))
				return;
			if (ActivePieceTable.IsComment && (hitTestResult.CommentViewInfo == null || hitTestResult.CommentViewInfo.Comment.Content.PieceTable != ActivePieceTable))
				return;
			DocumentLogPosition logPosition = hitTestResult.Character.GetFirstPosition(hitTestResult.PieceTable).LogPosition;
			Selection selection = DocumentModel.Selection;
			DocumentModel.BeginUpdate();
			try {
				DocumentModel.DeferredChanges.SuppressClearOutdatedSelectionItems = SuppressClearOutdatedSelectionItems;
				ChangeSelection(selection, logPosition, hitTestResult);
				if (UpdateCaretX) {
					ApplyLayoutPreferredPageIndex(CalculatePreferredPageIndex(hitTestResult));
				}
			}
			finally {
				DocumentModel.EndUpdate();
			}
			if (UpdateCaretX) {
				CaretPosition.Update(DocumentLayoutDetailsLevel.Character);
				Debug.Assert(CaretPosition.LayoutPosition.IsValid(DocumentLayoutDetailsLevel.Character));
				PageViewInfo pageViewInfo = CaretPosition.PageViewInfo;
				if (pageViewInfo != null) {
					CaretPosition.X = ActiveView.CreateLogicalPoint(pageViewInfo.ClientBounds, PhysicalPoint).X;
					TableCellViewInfo cell = CaretPosition.LayoutPosition.TableCell;
					CaretPosition.TableCellTopAnchorIndex = (cell != null) ? cell.TopAnchorIndex: -1;
				}
			}
		}
		int CalculatePreferredPageIndex(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.PieceTable.IsMain && hitTestResult.IsValid(DocumentLayoutDetailsLevel.Page))
				return hitTestResult.Page.PageIndex;
			RichEditHitTestRequest request = new RichEditHitTestRequest(DocumentModel.MainPieceTable);
			request.PhysicalPoint = PhysicalPoint;
			request.DetailsLevel = DocumentLayoutDetailsLevel.Page;
			request.Accuracy = HitTestAccuracy.NearestPage | HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestColumn | HitTestAccuracy.NearestTableRow | HitTestAccuracy.NearestTableCell | HitTestAccuracy.NearestRow | HitTestAccuracy.NearestBox | HitTestAccuracy.NearestCharacter;
			if (HitTestOnlyInPageClientBounds)
				request.Accuracy |= ActiveView.DefaultHitTestPageAccuracy;
			hitTestResult = ActiveView.HitTestCore(request, HitTestOnlyInPageClientBounds);
			if (!hitTestResult.IsValid(DocumentLayoutDetailsLevel.Page))
				return 0;
			else
				return hitTestResult.Page.PageIndex;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = true;
			state.Visible = true;
		}
		protected bool SelectToTheEndOfRow(Selection selection, Row row, bool extendSelection) {
			if (extendSelection) {
				DocumentLogPosition logPosition = row.GetLastPosition(ActivePieceTable).LogPosition;
				selection.End = logPosition;
				selection.End++;
				UpdateTableSelectionEnd(selection, logPosition);
				return true;
			}
			else {
				TextRunBase run = row.Boxes.Last.GetRun(ActivePieceTable);
				if (run is ParagraphRun)
					return false;
				else {
					DocumentModelPosition pos = row.GetLastPosition(ActivePieceTable);
					bool lineBreak = ActivePieceTable.TextBuffer[run.StartIndex + pos.RunOffset] == Characters.LineBreak;
					DocumentLogPosition logPosition = pos.LogPosition;
					selection.End = logPosition + (lineBreak ? 0 : 1);
					selection.UpdateTableSelectionEnd(logPosition);
					return !lineBreak;
				}
			}
		}
		protected virtual void UpdateTableSelectionEnd(Selection selection, DocumentLogPosition logPosition) {
			selection.UpdateTableSelectionEnd(logPosition);
		}
		protected internal virtual bool ChangeSelectionEnd(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
			if (selectionManager.ShouldSelectToTheEndOfRow(hitTestResult)) {
				if (SelectToTheEndOfRow(selection, hitTestResult.Row, ExtendSelection))
					return true;
			}
			selection.End = logPosition;
			if (ExtendSelection) {
				selection.UpdateTableSelectionEnd(logPosition);
			}
			return false;
		}
		protected internal virtual void ChangeSelectionStart(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult, int selectionItemCountBeforeChangeEnd) {
			if (!ExtendSelection) {
				selection.Start = selection.End;
				if(selection.Items.Count > selectionItemCountBeforeChangeEnd)
					selection.ClearMultiSelection(selectionItemCountBeforeChangeEnd - 1);
				selection.SetStartCell(logPosition);
			}
			else {
				EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(ActivePieceTable);
				selection.Start = selectionManager.ExtendSelectionStartToParagraphMark(selection, logPosition);
				selection.UpdateTableSelectionStart(logPosition);
			}
		}
		protected internal virtual void ChangeSelection(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			int selectionItemCountBeforeChangeEnd = selection.Items.Count;
			bool usePreviousBoxBounds = ChangeSelectionEnd(selection, logPosition, hitTestResult);
			ChangeSelectionStart(selection, logPosition, hitTestResult, selectionItemCountBeforeChangeEnd);
			selection.UsePreviousBoxBounds = usePreviousBoxBounds;
			if (ExtendSelection) {
				ValidateSelection();
			}
		}
		protected internal virtual void ValidateSelection() {
			FieldIsSelectValidator validator = new FieldIsSelectValidator(ActivePieceTable);
			validator.ValidateSelection(DocumentModel.Selection);
		}
	}
	#endregion
	public class PlaceCaretToPhysicalPointCommand2 : PlaceCaretToPhysicalPointCommand {
		public PlaceCaretToPhysicalPointCommand2(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool ChangeSelectionEnd(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			EnhancedSelectionManager selectionManager = new EnhancedSelectionManager(hitTestResult.PieceTable);
			if (selectionManager.ShouldSelectEntireRow(hitTestResult)) {
				if (SelectToTheEndOfRow(selection, hitTestResult.Row, true))
					return true;
			}
			return base.ChangeSelectionEnd(selection, logPosition, hitTestResult);
		}
	}
	public class PlaceCaretInCommentToPhysicalPointCommand : PlaceCaretToPhysicalPointCommand {
		public PlaceCaretInCommentToPhysicalPointCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool HitTestOnlyInPageClientBounds { get { return false; } }
	}
	#region ExtendSelectionToPhysicalPointCommand
	public class ExtendSelectionToPhysicalPointCommand : PlaceCaretToPhysicalPointCommand {
		public ExtendSelectionToPhysicalPointCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool ExtendSelection { get { return true; } }
		protected override void UpdateTableSelectionEnd(Selection selection, DocumentLogPosition logPosition) {
			selection.UpdateTableSelectionEnd(logPosition, true);
		}
	}
	#endregion
	#region ExtendSelectionByRangesCommandBase (abstract class)
	public abstract class ExtendSelectionByRangesCommandBase : PlaceCaretToPhysicalPointCommand {
		#region Fields
		Box initialBox;
		DocumentLogPosition initialLogPosition;
		#endregion
		protected ExtendSelectionByRangesCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override bool HitTestOnlyInPageClientBounds { get { return false; } }
		public Box InitialBox {
			get { return initialBox; }
			set {
				this.initialBox = value;
				this.initialLogPosition = InitialBox.GetFirstPosition(ActivePieceTable).LogPosition;
			}
		}
		public DocumentLogPosition InitialLogPosition { get { return initialLogPosition; } }
		#endregion
	}
	#endregion
	#region ExtendSelectionByCharactersCommand
	public class ExtendSelectionByCharactersCommand : ExtendSelectionByRangesCommandBase {
		public ExtendSelectionByCharactersCommand(IRichEditControl control)
			: base(control) {
		}
	}
	#endregion
	#region ExtendSelectionByRangesCommand (abstract class)
	public abstract class ExtendSelectionByRangesCommand : ExtendSelectionByRangesCommandBase {
		protected ExtendSelectionByRangesCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal virtual bool ExtendEnd1(Selection selection, RichEditHitTestResult hitTestResult) { 
			PieceTableIterator iterator = CreateIterator();
			DocumentModelPosition pos = hitTestResult.Character.GetFirstPosition(ActivePieceTable);
			selection.End = iterator.MoveForward(pos).LogPosition;
			return false;
		}
		protected internal virtual DocumentLogPosition ExtendEnd2(RichEditHitTestResult hitTestResult) { 
			PieceTableIterator iterator = CreateIterator();
			DocumentModelPosition pos = hitTestResult.Character.GetFirstPosition(ActivePieceTable);
			return iterator.MoveBack(pos).LogPosition;
		}
		protected internal virtual DocumentLogPosition ExtendStart1() { 
			PieceTableIterator iterator = CreateIterator();
			DocumentModelPosition pos = InitialBox.GetFirstPosition(ActivePieceTable);
			if (iterator.IsNewElement(pos))
				return pos.LogPosition;
			else
				return iterator.MoveBack(pos).LogPosition;
		}
		protected internal virtual DocumentLogPosition ExtendStart2() { 
			PieceTableIterator iterator = CreateIterator();
			DocumentModelPosition pos = InitialBox.GetFirstPosition(ActivePieceTable);
			return iterator.MoveForward(pos).LogPosition;
		}
		protected internal override bool ChangeSelectionEnd(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			if (hitTestResult.Character.GetFirstPosition(ActivePieceTable).LogPosition >= InitialLogPosition)
				return ExtendEnd1(selection, hitTestResult);
			else {
				selection.End = ExtendEnd2(hitTestResult);
				return false;
			}
		}
		protected internal override void ChangeSelectionStart(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult, int selectionItemCountBeforeChangeEnd) {
			if (hitTestResult.Character.GetFirstPosition(ActivePieceTable).LogPosition >= InitialLogPosition)
				selection.Start = ExtendStart1();
			else
				selection.Start = ExtendStart2();
		}
		protected internal abstract PieceTableIterator CreateIterator();
	}
	#endregion
	#region ExtendSelectionByWordsCommand
	public class ExtendSelectionByWordsCommand : ExtendSelectionByRangesCommand {
		public ExtendSelectionByWordsCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override PieceTableIterator CreateIterator() {
			return new WordsDocumentModelIterator(ActivePieceTable);
		}
	}
	#endregion
	#region ExtendSelectionByParagraphsCommand
	public class ExtendSelectionByParagraphsCommand : ExtendSelectionByRangesCommand {
		public ExtendSelectionByParagraphsCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override PieceTableIterator CreateIterator() {
			return new ParagraphsDocumentModelIterator(ActivePieceTable);
		}
	}
	#endregion
	#region ExtendSelectionByLinesCommand
	public class ExtendSelectionByLinesCommand : ExtendSelectionByRangesCommandBase {
		public ExtendSelectionByLinesCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal virtual bool ExtendEnd1(Selection selection, RichEditHitTestResult hitTestResult) {
			return SelectToTheEndOfRow(selection, hitTestResult.Row, true);
		}
		protected internal virtual DocumentLogPosition ExtendEnd2(RichEditHitTestResult hitTestResult) {
			return hitTestResult.Row.GetFirstPosition(ActivePieceTable).LogPosition;
		}
		protected internal virtual DocumentLogPosition ExtendStart1() {
			return InitialLogPosition;
		}
		protected internal virtual DocumentLogPosition ExtendStart2() {
			return InitialBox.GetLastPosition(ActivePieceTable).LogPosition + 1;
		}
		protected internal override bool ChangeSelectionEnd(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			if (hitTestResult.Character.GetFirstPosition(ActivePieceTable).LogPosition >= InitialLogPosition)
				return ExtendEnd1(selection, hitTestResult);
			else {
				selection.End = ExtendEnd2(hitTestResult);
				return false;
			}
		}
		protected internal override void ChangeSelectionStart(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult, int selectionItemCountBeforeChangeEnd) {
			if (hitTestResult.Character.GetFirstPosition(ActivePieceTable).LogPosition >= InitialLogPosition)
				selection.Start = ExtendStart1();
			else
				selection.Start = ExtendStart2();
		}
	}
	#endregion
	#region ExtendSelectionByCellsCommand
	public class ExtendSelectionByCellsCommand : ExtendSelectionByRangesCommandBase {
		#region Fields
		TableCell startCell;
		#endregion
		public ExtendSelectionByCellsCommand(IRichEditControl control, TableCell startCell)
			: base(control) {
			Guard.ArgumentNotNull(startCell, "startCell");
			this.startCell = startCell;
		}
		protected internal override void ChangeSelection(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			ParagraphIndex paragraphIndex = ActivePieceTable.FindParagraphIndex(logPosition);
			TableCell endCell = ActivePieceTable.TableCellsManager.GetCellByNestingLevel(paragraphIndex, startCell.Table.NestedLevel);
			if (endCell.Table.NestedLevel != startCell.Table.NestedLevel)
				startCell = ActivePieceTable.TableCellsManager.GetCellByNestingLevel(startCell.StartParagraphIndex, endCell.Table.NestedLevel);
			while (startCell != null && endCell != null) {
				if (startCell.Table == endCell.Table) {
					selection.ManualySetTableSelectionStructureAndChangeSelection(startCell, endCell);
					break;
				}
				startCell = startCell.Table.ParentCell;
				endCell = endCell.Table.ParentCell;
			}
			FieldIsSelectValidator validator = new FieldIsSelectValidator(ActivePieceTable);
			validator.ValidateSelection(selection);
		}
	}
	#endregion
	#region ExtendSelectionByTableRowsCommand
	public class ExtendSelectionByTableRowsCommand : ExtendSelectionByRangesCommandBase {
		#region Fields
		int nestingLevel;
		DocumentLogPosition startLogPosition;
		DocumentLogPosition endLogPosition;
		#endregion
		public ExtendSelectionByTableRowsCommand(IRichEditControl control, int nestingLevel)
			: base(control) {
			this.nestingLevel = nestingLevel;
		}
		#region Properties
		public DocumentLogPosition StartLogPosition { get { return startLogPosition; } set { startLogPosition = value; } }
		public DocumentLogPosition EndLogPosition { get { return endLogPosition; } set { endLogPosition = value; } }
		#endregion
		protected internal override bool ChangeSelectionEnd(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			ParagraphCollection paragraphs = ActivePieceTable.Paragraphs;
			TableRow actualTableRow = GetCurrentTableRow(logPosition);
			if (logPosition < StartLogPosition) {
				ParagraphIndex startParagraphIndexInRow = actualTableRow.FirstCell.StartParagraphIndex;
				selection.End = paragraphs[startParagraphIndexInRow].LogPosition;
			}
			else {
				ParagraphIndex endParagraphIndexInRow = actualTableRow.LastCell.EndParagraphIndex;
				selection.End = paragraphs[endParagraphIndexInRow].EndLogPosition + 1;
			}
			return true;
		}
		protected internal override void ChangeSelectionStart(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult, int selectionItemCountBeforeChangeEnd) {
		}
		protected internal virtual TableRow GetCurrentTableRow(DocumentLogPosition logPosition) {
			ParagraphIndex paragraphIndex = ActivePieceTable.FindParagraphIndex(logPosition);
			return ActivePieceTable.TableCellsManager.GetCellByNestingLevel(paragraphIndex, nestingLevel).Row;
		}
	}
	#endregion
	#region ExtendSelectionByStartTableRowsCommand
	public class ExtendSelectionByStartTableRowsCommand : ExtendSelectionByTableRowsCommand {
		#region Fields
		int startRowIndex = -1;
		#endregion
		public ExtendSelectionByStartTableRowsCommand(IRichEditControl control, int nestingLevel)
			: base(control, nestingLevel) {
		}
		#region Properties
		public int StartRowIndex { get { return startRowIndex; } set { startRowIndex = value; } }
		#endregion
		protected internal override void ChangeSelection(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			SelectTableRowCommand command = new SelectTableRowCommand(Control);
			command.CanCalculateExecutionParameters = false;
			TableRow currentTableRow = GetCurrentTableRow(logPosition);
			TableRowCollection rows = currentTableRow.Table.Rows;
			TableRow startRow = rows[StartRowIndex];
			if (startRow.Table != currentTableRow.Table)
				return;
			command.Rows = rows;
			command.StartRowIndex = StartRowIndex;
			command.EndRowIndex = currentTableRow.IndexInTable;
			command.Execute();
			((SelectedCellsCollection)selection.SelectedCells).SetOriginalStartLogPosition(StartLogPosition);
		}
	}
	#endregion
	#region ExtendSelectionByTableColumnsCommand
	public class ExtendSelectionByTableColumnsCommand : ExtendSelectionByRangesCommandBase {
		#region Fields
		int nestedLevel;
		int startColumnIndex;
		#endregion
		public ExtendSelectionByTableColumnsCommand(IRichEditControl control, int startColumnIndex, int nestedLevel)
			: base(control) {
			this.nestedLevel = nestedLevel;
			this.startColumnIndex = startColumnIndex;
		}
		protected internal override void ChangeSelection(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			ParagraphIndex paragraphIndex = ActivePieceTable.FindParagraphIndex(logPosition);
			TableCell currentTableCell = ActivePieceTable.TableCellsManager.GetCellByNestingLevel(paragraphIndex, nestedLevel);
			int endColumnIndex = currentTableCell.GetEndColumnIndexConsiderRowGrid();
			if (startColumnIndex > endColumnIndex) {
				int temp = startColumnIndex;
				startColumnIndex = endColumnIndex;
				endColumnIndex = temp;
			}
			SelectTableColumnsCommand command = new SelectTableColumnsCommand(Control);
			command.Rows = currentTableCell.Table.Rows;
			command.StartColumnIndex = startColumnIndex;
			command.EndColumnIndex = endColumnIndex;
			command.ChangeSelection(selection);
			command.ValidateSelection(selection, true);
		}
	}
	#endregion
	#region SelectLineCommand
	public class SelectLineCommand : PlaceCaretToPhysicalPointCommand {
		public SelectLineCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override bool HitTestOnlyInPageClientBounds { get { return false; } }
		protected internal override bool ChangeSelectionEnd(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			return SelectToTheEndOfRow(selection, hitTestResult.Row, true);
		}
		protected internal override void ChangeSelectionStart(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult, int selectionItemCountBeforeChangeEnd) {
			DocumentLogPosition pos = hitTestResult.Row.GetFirstPosition(ActivePieceTable).LogPosition;
			selection.Start = (pos > DocumentLogPosition.Zero) ? pos -1 : pos;
			selection.ClearMultiSelection();
			selection.SetStartCell(logPosition);
			selection.Start = pos;
		}
		protected internal override void ChangeSelection(Selection selection, DocumentLogPosition logPosition, RichEditHitTestResult hitTestResult) {
			DocumentModel.Selection.ClearMultiSelection();
			base.ChangeSelection(selection, logPosition, hitTestResult);
			FieldIsSelectValidator validator = new FieldIsSelectValidator(ActivePieceTable);
			validator.ValidateSelection(selection);
		}
	}
	#endregion
	public class VirtualTableColumn {		
		readonly TableElementAccessorCollection elements;
		int maxLeftBorder;
		int position;
		int maxRightBorder;		
		TableViewInfo tableViewInfo;
		public VirtualTableColumn() {			
			this.elements = new TableElementAccessorCollection();			
		}
		public TableElementAccessorCollection Elements { get { return elements; } }
		public TableViewInfo TableViewInfo { get { return tableViewInfo; } set { tableViewInfo = value; } }
		public int MaxLeftBorder { get { return maxLeftBorder; } set { maxLeftBorder = value; } }
		public int Position { get { return position; } set { position = value; } }
		public int MaxRightBorder { get { return maxRightBorder; } set { maxRightBorder = value; } }
	}
	#region EnhancedSelectionManager
	public class EnhancedSelectionManager {
		const int TextBoxHotZoneWidthInPixels = 5;
		readonly PieceTable pieceTable;
		readonly LayoutUnit minDistance;
		readonly LayoutUnit minBorderWidthToHitTest;
		public EnhancedSelectionManager(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
			minDistance = DocumentModel.LayoutUnitConverter.TwipsToLayoutUnits(14);
			minBorderWidthToHitTest = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(4);
		}
		protected internal DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		protected internal PieceTable PieceTable { get { return pieceTable; } }
		public virtual bool ShouldSelectFloatingObject(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Page)
				return false;
			FloatingObjectBox floatingObjectBox = hitTestResult.FloatingObjectBox;
#if !DXPORTABLE
			if (floatingObjectBox == null || DevExpress.XtraRichEdit.Mouse.RichEditMouseHandler.IsInlinePictureBoxHit(hitTestResult))
#else
			if (floatingObjectBox == null)
#endif
				return false;
			return ShouldSelectFloatingObject(floatingObjectBox, hitTestResult.LogicalPoint);
		}
		protected internal bool ShouldSelectFloatingObject(FloatingObjectBox floatingObjectBox, Point logicalPoint) {
			FloatingObjectAnchorRun run = floatingObjectBox.GetFloatingObjectRun();
			if (run == null || floatingObjectBox.DocumentLayout == null)
				return true;
			TextBoxFloatingObjectContent textBoxContent = run.Content as TextBoxFloatingObjectContent;
			if (textBoxContent == null)
				return true;
			Point point = floatingObjectBox.TransformPointBackward(logicalPoint);
			return !GetTextBoxContentBounds(floatingObjectBox, textBoxContent).Contains(point);
		}
		protected internal Rectangle GetTextBoxContentBounds(FloatingObjectBox floatingObjectBox, TextBoxFloatingObjectContent content) {
			DocumentModelUnitToLayoutUnitConverter unitConverter = this.pieceTable.DocumentModel.ToDocumentLayoutUnitConverter;
			Point location = floatingObjectBox.ContentBounds.Location;
			TextBoxProperties textBoxProperties = content.TextBoxProperties;
			int hotZoneWidth = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(TextBoxHotZoneWidthInPixels);
			int hotZoneHeight = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(TextBoxHotZoneWidthInPixels);
			int leftMargin = Math.Max(hotZoneWidth, unitConverter.ToLayoutUnits(textBoxProperties.LeftMargin));
			int topMargin = Math.Max(hotZoneHeight, unitConverter.ToLayoutUnits(textBoxProperties.TopMargin));
			int rightMargin = Math.Max(hotZoneWidth, unitConverter.ToLayoutUnits(textBoxProperties.RightMargin));
			int bottomMargin = Math.Max(hotZoneHeight, unitConverter.ToLayoutUnits(textBoxProperties.BottomMargin));
			location.X += leftMargin;
			location.Y += topMargin;
			Size size = floatingObjectBox.ContentBounds.Size;
			size.Width -= leftMargin + rightMargin;
			size.Height -= topMargin + bottomMargin;
			size.Width = Math.Max(size.Width, 50);
			size.Height = Math.Max(size.Height, 10);
			return new Rectangle(location, size);
		}
		public virtual bool ShouldSelectComment(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Page)
				return false;
			CommentViewInfo commentViewInfo = hitTestResult.CommentViewInfo;
			if (commentViewInfo != null)
				return true;
			return false;
		}
		public virtual bool ShouldSelectCommentMoreButton(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Page)
				return false;
			CommentLocationType commentLocation = hitTestResult.CommentLocation;
			if (commentLocation == CommentLocationType.CommentMoreButton)
				return true;
			return false;
		}
		public virtual bool ShouldSelectCommentContent(RichEditHitTestResult hitTestResult) {
			if (hitTestResult.DetailsLevel < DocumentLayoutDetailsLevel.Page)
				return false;
			CommentLocationType commentLocation = hitTestResult.CommentLocation;
			if (commentLocation == CommentLocationType.CommentContent)
				return true;
			return false;
		}
		public virtual bool ShouldSelectToTheEndOfRow(RichEditHitTestResult hitTestResult) {
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, pieceTable))
				return false;
			if ((hitTestResult.Accuracy & HitTestAccuracy.ExactCharacter) == 0)
				if (hitTestResult.Character.Bounds.Right < hitTestResult.LogicalPoint.X)
					return true;
			return false;
		}
		public virtual bool ShouldSelectEntireRow(RichEditHitTestResult hitTestResult) {
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, pieceTable))
				return false;
			if ((hitTestResult.Accuracy & HitTestAccuracy.ExactCharacter) == 0)
				if (hitTestResult.Row.Bounds.Left > hitTestResult.LogicalPoint.X)
					return true;
			return false;
		}
		protected internal virtual TableRowViewInfoBase CalculateTableRowToResize(RichEditHitTestResult hitTestResult) {
			if ((hitTestResult.Accuracy & HitTestAccuracy.ExactTableCell) == 0)
				return null;
			TableCellViewInfo cell = hitTestResult.TableCell;
			TableViewInfo table = cell.TableViewInfo;
			TableCellVerticalAnchor anchor = table.Anchors[cell.TopAnchorIndex];
			Rectangle bounds = table.GetCellBounds(cell);
			bounds.Y = anchor.VerticalPosition;
			bounds.Height = Math.Max(anchor.BottomTextIndent, minBorderWidthToHitTest);
			if (bounds.Contains(hitTestResult.LogicalPoint))
				return hitTestResult.TableRow.Previous;
			if (hitTestResult.TableRow.Row == table.Table.Rows.Last && table.NextTableViewInfo == null) {
				bounds.Y = table.Anchors.Last.VerticalPosition;
				bounds.Height = Math.Max(anchor.BottomTextIndent, minBorderWidthToHitTest);
				if (bounds.Contains(hitTestResult.LogicalPoint))
					return hitTestResult.TableRow;
			}
			return null;
		}
		protected internal virtual bool ShouldResizeTableRow(IRichEditControl control, RichEditHitTestResult hitTestResult, TableRowViewInfoBase tableRow) {
			if (tableRow == null)
				return false;
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, pieceTable))
				return false;
			ChangeTableRowHeightCommand command = DocumentModel.CommandsCreationStrategy.CreateChangeTableRowHeightCommand(control, tableRow.Row, 100);
			return command.CanExecute();
		}
		protected internal VirtualTableColumn CalculateTableCellsToResizeHorizontally(RichEditHitTestResult hitTestResult) {
			if ((hitTestResult.Accuracy & HitTestAccuracy.ExactTableCell) == 0)
				return null;
			TableCellViewInfo cell = hitTestResult.TableCell;
			TableViewInfo table = cell.TableViewInfo;
			TableCell tableCell = cell.Cell;
			TableRow tableRow = tableCell.Row;
			int topRowViewInfoIndex = Math.Max(cell.StartRowIndex - cell.TableViewInfo.TopRowIndex, 0);
			int bottomRowViewInfoIndex = topRowViewInfoIndex + cell.BottomAnchorIndex - cell.TopAnchorIndex - 1;
			TableRowViewInfoBase rowViewInfo = table.Rows[topRowViewInfoIndex];
			TableRowViewInfoBase bottomRowViewInfo = table.Rows[bottomRowViewInfoIndex];
			int startLayoutHorizontalPositionIndex = TableCellVerticalBorderCalculator.GetStartColumnIndex(tableCell, true);
			int startModelHorizontalPositionIndex = tableCell.GetStartColumnIndexConsiderRowGrid();
			int endLayoutHorizontalPositionIndex = startLayoutHorizontalPositionIndex + tableCell.LayoutProperties.ColumnSpan;
			int endModelHorizontalPositionIndex = tableCell.GetStartColumnIndexConsiderRowGrid() + tableCell.ColumnSpan;
			int borderLayoutHorizontalPositionIndex;
			int borderModelHorizontalPositionIndex;
			Rectangle bounds = rowViewInfo.GetVerticalBorderBounds(endLayoutHorizontalPositionIndex);
			bounds = GetNormalizedBorderBounds(bounds, rowViewInfo, bottomRowViewInfo);
			TableElementAccessorBase elementAccessor;
			if (!bounds.Contains(hitTestResult.LogicalPoint)) {
				bounds = rowViewInfo.GetVerticalBorderBounds(startLayoutHorizontalPositionIndex);
				bounds = GetNormalizedBorderBounds(bounds, rowViewInfo, bottomRowViewInfo); 
				if (!bounds.Contains(hitTestResult.LogicalPoint))
					return null;
				borderModelHorizontalPositionIndex = startModelHorizontalPositionIndex;
				borderLayoutHorizontalPositionIndex = startLayoutHorizontalPositionIndex;
				bool isFirstCellInRow = rowViewInfo.Cells.First == cell;
				if (isFirstCellInRow)
					elementAccessor = new TableRowBeforeAccessor(tableCell.Row);
				else {
					TableCellViewInfo prevCell = rowViewInfo.Cells[rowViewInfo.Cells.IndexOf(cell) - 1];
					elementAccessor = new TableCellAccessor(prevCell.Cell);
				}
			}
			else {
				elementAccessor = new TableCellAccessor(tableCell);
				borderLayoutHorizontalPositionIndex = endLayoutHorizontalPositionIndex;
				borderModelHorizontalPositionIndex = endModelHorizontalPositionIndex;
			}
			return CalculateTableCellsToResizeHorizontallyCore(table, tableRow, elementAccessor, borderLayoutHorizontalPositionIndex, borderModelHorizontalPositionIndex);
		}
		protected internal virtual Rectangle GetNormalizedBorderBounds(Rectangle bounds, TableRowViewInfoBase rowViewInfo, TableRowViewInfoBase bottomRowViewInfo) {
			Rectangle result = new Rectangle(bounds.Location, bounds.Size);
			if (bottomRowViewInfo != rowViewInfo)
				result.Height = bottomRowViewInfo.BottomAnchor.VerticalPosition - bounds.Top;
			if (bounds.Width < minBorderWidthToHitTest) {
				LayoutUnit delta = minBorderWidthToHitTest - bounds.Width;
				result.X -= delta;
				result.Width = minBorderWidthToHitTest;
			}
			return result;
		}
		protected internal VirtualTableColumn CalculateTableCellsToResizeHorizontallyCore(TableViewInfo table, TableRow tableRow, TableElementAccessorBase elementAccessor, int borderLayoutHorizontalPositionIndex, int borderModelHorizontalPositionIndex) {
			VirtualTableColumn result = new VirtualTableColumn();
			result.Position = table.GetAlignmentedPosition(borderLayoutHorizontalPositionIndex);
			result.MaxLeftBorder = Int32.MinValue;
			result.MaxRightBorder = Int32.MaxValue;
			ApplyMaxBorders(result, table, elementAccessor);
			result.TableViewInfo = table;
			result.Elements.Add(elementAccessor);
			bool appendSelectedCellsOnly = ShouldAppendSelectedCellsOnly(DocumentModel.Selection, elementAccessor);
			bool isFirstRow = tableRow.Table.Rows.First == tableRow;
			MoveNextRowDelegate moveTop = delegate(TableRow row) {
				return row.Previous;
			};
			MoveNextRowDelegate moveBottom = delegate(TableRow row) {
				return row.Next;
			};
			AppendCells(table, tableRow, borderModelHorizontalPositionIndex, result, appendSelectedCellsOnly, isFirstRow, moveTop);
			AppendCells(table, tableRow, borderModelHorizontalPositionIndex, result, appendSelectedCellsOnly, isFirstRow, moveBottom);
			return result;
		}
		protected virtual bool ShouldAppendSelectedCellsOnly(Selection selection, TableElementAccessorBase elementAccessor) {
			if (!selection.IsWholeSelectionInOneTable())
				return false;
			return ShouldAddElement(elementAccessor);
		}
		protected internal delegate TableRow MoveNextRowDelegate(TableRow currentRow);
		protected internal virtual void AppendCells(TableViewInfo tableViewInfo, TableRow startRow, int modelHorizontalPositionIndex, VirtualTableColumn result, bool appendSelectedCellsOnly, bool isFirstRow, MoveNextRowDelegate moveNextRow) {
			TableRowAlignment rowAlignment = startRow.TableRowAlignment;
			for (TableRow row = moveNextRow(startRow); row != null; row = moveNextRow(row)) {
				if (result.MaxLeftBorder == result.MaxRightBorder)
					break;
				TableElementAccessorBase element = FindElementByHorizontalPosition(row, modelHorizontalPositionIndex);
				if (row.TableRowAlignment != rowAlignment || element == null) {
					if (!appendSelectedCellsOnly && !isFirstRow)
						return;
					else
						continue;
				}
				if (!appendSelectedCellsOnly || ShouldAddElement(element)) {
					result.Elements.Add(element);
					ApplyMaxBorders(result, tableViewInfo, element);
				}
			}
		}
		protected internal virtual void ApplyMaxBorders(VirtualTableColumn result, TableViewInfo tableViewInfo, TableElementAccessorBase element) {
			result.MaxLeftBorder = Math.Max(result.MaxLeftBorder, element.GetMinRightBorderPosition(tableViewInfo) + minDistance);
			result.MaxRightBorder = Math.Min(result.MaxRightBorder, element.GetMaxRightBorderPosition(tableViewInfo) - minDistance);
			if (result.MaxLeftBorder > result.MaxRightBorder) {
				LayoutUnit middle = (result.MaxRightBorder + result.MaxLeftBorder) / 2;
				result.MaxLeftBorder = middle;
				result.MaxRightBorder = middle;
			}
		}
		protected bool ShouldAddElement(TableElementAccessorBase element) {
			Selection selection = DocumentModel.Selection;
			Debug.Assert(selection.IsWholeSelectionInOneTable());
			SelectedCellsCollection selectedCells = selection.SelectedCells as SelectedCellsCollection;
			if(selectedCells == null)
				return false;
			return element.IsRightBoundarySelected(selectedCells);
		}
		protected internal virtual TableElementAccessorBase FindElementByHorizontalPosition(TableRow row, int modelHorizontalPosition) {
			modelHorizontalPosition -= row.GridBefore;
			if (modelHorizontalPosition == 0)
				return new TableRowBeforeAccessor(row);
			TableCellCollection cells = row.Cells;
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				if (modelHorizontalPosition < 0)
					return null;
				TableCell cell = cells[i];
				modelHorizontalPosition -= cell.ColumnSpan;				
				if (modelHorizontalPosition == 0)
					return new TableCellAccessor(cell);
			}
			return null;
		}
		protected internal bool ShouldResizeTableCellsHorizontally(IRichEditControl control, RichEditHitTestResult hitTestResult, VirtualTableColumn virtualColumn) {
			if (virtualColumn == null)
				return false;
			if (virtualColumn.Elements.Count <= 0)
				return false;
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, pieceTable))
				return false;
			ChangeTableVirtualColumnRightCommand command = new ChangeTableVirtualColumnRightCommand(control, virtualColumn, 100);
			return command.CanExecute();
		}
		protected internal virtual bool ShouldSelectEntireTableColumn(RichEditHitTestResult hitTestResult) {
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, pieceTable))
				return false;
			if ((hitTestResult.Accuracy & HitTestAccuracy.ExactTableCell) == 0)
				return false;
			TableRowViewInfoBase rowInfo = hitTestResult.TableRow;
			if (rowInfo.Row.IndexInTable > 0)
				return false;
			int logicalY = hitTestResult.LogicalPoint.Y;
			ITableCellVerticalAnchor tableRowTopAnchor = rowInfo.TopAnchor;
			int tableRowTopPosition = tableRowTopAnchor.VerticalPosition;
			int offset = Math.Min(tableRowTopAnchor.BottomTextIndent + SystemInformation.DragSize.Height, rowInfo.BottomAnchor.VerticalPosition - tableRowTopPosition);
			return logicalY >= tableRowTopPosition && logicalY <= tableRowTopPosition + offset;
		}
		protected internal virtual bool ShouldSelectEntireTableRow(RichEditHitTestResult hitTestResult) {
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, pieceTable))
				return false;
			if ((hitTestResult.Accuracy & HitTestAccuracy.ExactTableRow) != 0)
				if (hitTestResult.TableRow.Cells.First.Left > hitTestResult.LogicalPoint.X)
					return true;
			return false;
		}
		protected internal virtual bool ShouldSelectEntireTableCell(RichEditHitTestResult hitTestResult) {
			if (!Object.ReferenceEquals(hitTestResult.PieceTable, pieceTable))
				return false;
			HitTestAccuracy accuracy = hitTestResult.Accuracy;
			if ((accuracy & HitTestAccuracy.ExactTableCell) != 0) {
				TableCellViewInfo cellViewInfo = hitTestResult.TableCell;
				int cellViewInfoLeft = cellViewInfo.Left;
				int cellMargin = GetWidthConsiderUnitType(cellViewInfo.Cell.GetActualLeftMargin());
				int logicalX = hitTestResult.LogicalPoint.X;
				if (cellViewInfoLeft <= logicalX && logicalX <= cellViewInfoLeft + cellMargin)
					return true;
			}
			return false;
		}
		protected internal virtual int GetWidthConsiderUnitType(WidthUnit widthUnit) {
			int result = widthUnit.Value;
			if (widthUnit.Type == WidthUnitType.ModelUnits)
				result = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(result);
			return result;
		}
		protected internal virtual DocumentLogPosition ExtendSelectionStartToParagraphMark(Selection selection, DocumentLogPosition logPosition) {
			ParagraphIndex selectionEndParagraph = GetSelectionEndParagraphIndex(selection);
			Paragraph paragraph = PieceTable.Paragraphs[selectionEndParagraph];
			if (logPosition <= paragraph.LogPosition && selection.Start == paragraph.LogPosition + paragraph.Length - 1) {
				if (paragraph.Length > 1)
					return selection.Start + 1;
			}
			return selection.Start;
		}
		protected internal virtual DocumentLogPosition ExtendSelectionEndToParagraphMark(Selection selection, DocumentLogPosition logPosition) {
			ParagraphIndex selectionEndParagraph = GetSelectionEndParagraphIndex(selection);
			Paragraph paragraph = PieceTable.Paragraphs[selectionEndParagraph];
			if (paragraph.LogPosition + paragraph.Length - 1 == logPosition) {
				if (selection.NormalizedStart <= paragraph.LogPosition) {
					if (paragraph.Length > 1) {
						if (selection.NormalizedEnd <= PieceTable.DocumentEndLogPosition)
							return logPosition + 1;
					}
				}
			}
			return logPosition;
		}
		ParagraphIndex GetSelectionEndParagraphIndex(Selection selection) {
			ParagraphIndex selectionEndParagraph = selection.Interval.NormalizedEnd.ParagraphIndex;
			ParagraphIndex lastParagraphIndex = new ParagraphIndex(selection.PieceTable.Paragraphs.Count - 1);
			if (selectionEndParagraph > lastParagraphIndex)
				return lastParagraphIndex;
			return selectionEndParagraph;
		}
		protected internal virtual DocumentLogPosition ExtendSelectionStartToTableRow(Selection selection, DocumentLogPosition logPosition) {
			ParagraphCollection paragraphs = PieceTable.Paragraphs;
			Paragraph paragraph = paragraphs[selection.Interval.NormalizedStart.ParagraphIndex];
			TableCell tableCell = paragraph.GetCell();
			if (tableCell == null)
				return selection.Start;
			return paragraphs[tableCell.Row.FirstCell.StartParagraphIndex].LogPosition;
		}
		protected internal virtual DocumentLogPosition ExtendSelectionEndToTableRow(Selection selection, DocumentLogPosition logPosition) {
			Paragraph paragraph = PieceTable.FindParagraph(selection.NormalizedVirtualEnd);
			TableCell tableCell = paragraph.GetCell();
			if (tableCell == null)
				return selection.End;
			return PieceTable.Paragraphs[tableCell.Row.LastCell.EndParagraphIndex].EndLogPosition + 1;
		}
	}
#endregion
#region SelectionValidator (abstract class)
	public abstract class SelectionValidator {
		readonly PieceTable pieceTable;
		protected SelectionValidator(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public virtual void ValidateSelection(Selection selection) {
			if (selection.Length == 0) {
				ValidateTableActiveSelection(selection); 
				return;
			}
			ValidateFieldSelectionCore(selection);
			ValidateTableSelectionCore(selection);
		}
		protected internal void ValidateTableActiveSelection(Selection selection) {
			if (selection.Items.Count != 1)
				return;
			SelectionItem activeSelection = selection.ActiveSelection;
			ParagraphIndex paragraphIndex = PieceTable.FindParagraphIndex(activeSelection.Start);
			int offset = CalculateOffset(paragraphIndex, true);
			activeSelection.LeftOffset = offset;
			activeSelection.RightOffset = -offset;
		}
		protected internal virtual void ValidateTableSelectionCore(Selection selection) {
			int selectionsCount = selection.Items.Count;
			for (int i = 0; i < selectionsCount; i++) {
				SelectionItem selectionItem = selection.Items[i];
				selectionItem.IsCovered = false;
				if (selectionsCount > 1 && IsSelectedItemCovered(selectionItem)) {
					selectionItem.IsCovered = true;
					continue;
				}
				ParagraphIndex paragraphIndex = PieceTable.FindParagraphIndex(selectionItem.NormalizedStart);
				int leftOffset = CalculateOffset(paragraphIndex, true);
				paragraphIndex = PieceTable.FindParagraphIndex(selectionItem.NormalizedEnd);
				int rightOffset = CalculateOffset(paragraphIndex, false);
				selectionItem.LeftOffset = leftOffset;
				selectionItem.RightOffset = rightOffset;
			}
		}
		bool IsSelectedItemCovered(SelectionItem item) {
			for (DocumentLogPosition i = item.NormalizedStart; i < item.NormalizedEnd; i++) {
				TableCell cell = PieceTable.FindParagraph(i).GetCell();
				if (cell == null || cell.VerticalMerging != MergingState.Continue)
					return false;
			}
			return true;
		}
		int CalculateOffset(ParagraphIndex paragraphIndex, bool incrementIndex) {
			int result = 0;
			for (; ; ) {
				TableCell cell = PieceTable.Paragraphs[paragraphIndex].GetCell();
				if (cell == null)
					break;
				if (cell.VerticalMerging != MergingState.Continue)
					break;
				if (incrementIndex)
					paragraphIndex++;
				else
					paragraphIndex--;
				result++;
			}
			return result;
		}
		protected internal virtual void ValidateFieldSelectionCore(Selection selection) {
			RunInfo selectedInterval = PieceTable.FindRunInfo(selection.NormalizedStart, selection.Length);
			Field lastField = PieceTable.FindFieldByRunIndex(selectedInterval.End.RunIndex);
			if (lastField != null)
				ChangeFieldSelection(selection, selectedInterval, lastField);
			Field firstField = PieceTable.FindFieldByRunIndex(selectedInterval.Start.RunIndex);
			if (firstField != lastField && firstField != null)
				ChangeFieldSelection(selection, selectedInterval, firstField);
		}
		protected void ChangeFieldSelection(Selection selection, RunInfo selectedInterval, Field field) {
			if (ShouldExtendInterval(selectedInterval, field))
				ChangeFieldSelectionCore(selection, field);
			if (field.Parent != null)
				ChangeFieldSelection(selection, selectedInterval, field.Parent);
		}
		protected internal virtual bool ShouldExtendInterval(RunInfo interval, Field field) {
			if (field.IsCodeView)
				return ShouldChangeInterval(interval, field.Code.Start, field.Code.End);
			else
				return ShouldChangeInterval(interval, field.Code.End, field.Result.End);
		}
		protected virtual bool ShouldChangeInterval(RunInfo interval, RunIndex start, RunIndex end) {
			return (interval.Start.RunIndex > start && interval.End.RunIndex >= end) ||
				(interval.Start.RunIndex <= start && interval.End.RunIndex < end);
		}
		protected internal abstract void ChangeFieldSelectionCore(Selection selection, Field field);
	}
#endregion
#region FieldIsSelectValidator
	public class FieldIsSelectValidator : SelectionValidator {
		public FieldIsSelectValidator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void ChangeFieldSelectionCore(Selection selection, Field field) {
			DocumentModelPosition startPos = DocumentModelPosition.FromRunStart(PieceTable, field.FirstRunIndex);
			DocumentModelPosition endPos = DocumentModelPosition.FromRunEnd(PieceTable, field.LastRunIndex);
			if (selection.End > selection.Start) {
				selection.Start = Algorithms.Min(selection.Start, startPos.LogPosition);
				selection.End = Algorithms.Max(selection.End, endPos.LogPosition);
			}
			else if (selection.End < selection.Start) {
				selection.End = Algorithms.Min(selection.End, startPos.LogPosition);
				selection.Start = Algorithms.Max(selection.Start, endPos.LogPosition);
			}			
		}
	}
#endregion
#region FieldIsUnselectValidator
	public class FieldIsUnselectValidator : SelectionValidator {
		public FieldIsUnselectValidator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override void ChangeFieldSelectionCore(Selection selection, Field field) {
			if (selection.End > selection.Start) {
				DocumentModelPosition startPos = DocumentModelPosition.FromRunStart(PieceTable, field.FirstRunIndex);
				selection.End = Algorithms.Min(selection.End, startPos.LogPosition);
			}
			else if (selection.End < selection.Start) {
				DocumentModelPosition endPos = DocumentModelPosition.FromRunEnd(PieceTable, field.LastRunIndex);
				selection.End = Algorithms.Max(selection.End, endPos.LogPosition);
			}
		}
	}
#endregion
#region FieldSelectionManager
	public class FieldSelectionManager {
		readonly PieceTable pieceTable;
		public FieldSelectionManager(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentLogPosition ExtendSelectionEndToFieldEnd(Selection selection, DocumentLogPosition logPosition) {
			RunInfo selectedInterval = GetSelectedInterval(selection.Start, logPosition);
			Field field = PieceTable.FindFieldByRunIndex(selectedInterval.End.RunIndex);
			if (field != null && ShouldChangeSelection(selectedInterval, field))
				return CalcExtendedSelectionEndPosition(selection, logPosition, field);
			return logPosition;
		}
		RunInfo GetSelectedInterval(DocumentLogPosition start, DocumentLogPosition end) {
			if (start < end)
				return PieceTable.FindRunInfo(start, end - start);
			else
				return PieceTable.FindRunInfo(end, start - end);
		}
		public DocumentLogPosition ExtendSelectionStartToFieldStart(Selection selection) {
			RunInfo selectedInterval = GetSelectedInterval(selection.Start, selection.End);
			Field field = PieceTable.FindFieldByRunIndex(selectedInterval.Start.RunIndex);
			if (field != null && ShouldChangeSelection(selectedInterval, field))
				return CalcSelectionStartPosition(selection, field);
			return selection.Start;
		}
		protected virtual bool ShouldChangeSelection(RunInfo interval, Field field) {
			RunIndex startSelectionIndex = interval.Start.RunIndex;
			RunIndex endSelectionIndex = interval.End.RunIndex;
			FieldRunInterval visibleInterval = field.IsCodeView ? field.Code : field.Result;
			return (startSelectionIndex > visibleInterval.Start && endSelectionIndex >= visibleInterval.End) ||
				(startSelectionIndex <= visibleInterval.Start && endSelectionIndex < visibleInterval.End);
		}
		DocumentLogPosition CalcExtendedSelectionEndPosition(Selection selection, DocumentLogPosition endLogPos, Field field) {
			if (endLogPos > selection.Start) {
				DocumentModelPosition endPos = DocumentModelPosition.FromRunEnd(PieceTable, field.LastRunIndex);
				return Algorithms.Max(endLogPos, endPos.LogPosition);
			}
			else {
				DocumentModelPosition startPos = DocumentModelPosition.FromRunStart(PieceTable, field.FirstRunIndex);
				return Algorithms.Min(endLogPos, startPos.LogPosition);
			}
		}
		DocumentLogPosition CalcSelectionStartPosition(Selection selection, Field field) {
			if (selection.End > selection.Start) {
				DocumentModelPosition startPos = DocumentModelPosition.FromRunStart(PieceTable, field.FirstRunIndex);
				return Algorithms.Min(selection.Start, startPos.LogPosition);
			}
			else {
				DocumentModelPosition endPos = DocumentModelPosition.FromRunEnd(PieceTable, field.LastRunIndex);
				return Algorithms.Max(selection.Start, endPos.LogPosition);
			}
		}
		public DocumentLogPosition ConstrictSelectionEndToFieldEnd(Selection selection, DocumentLogPosition logPosition) {
			RunInfo selectedInterval = GetSelectedInterval(selection.Start, logPosition);
			Field field = PieceTable.FindFieldByRunIndex(selectedInterval.End.RunIndex);
			if (field != null && ShouldChangeSelection(selectedInterval, field))
				return CalcConstrictedSelectionEndPosition(selection, logPosition, field);
			return logPosition;
		}
		DocumentLogPosition CalcConstrictedSelectionEndPosition(Selection selection, DocumentLogPosition logPosition, Field field) {
			if (logPosition > selection.Start) {
				DocumentModelPosition startPos = DocumentModelPosition.FromRunStart(PieceTable, field.FirstRunIndex);
				return Algorithms.Min(logPosition, startPos.LogPosition);
			}
			else {
				DocumentModelPosition endPos = DocumentModelPosition.FromRunEnd(PieceTable, field.LastRunIndex);
				return Algorithms.Max(logPosition, endPos.LogPosition);
			}
		}
	}
#endregion
}
