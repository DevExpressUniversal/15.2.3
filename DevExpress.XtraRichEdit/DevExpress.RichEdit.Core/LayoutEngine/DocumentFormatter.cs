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
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region DocumentFormatter
	public class DocumentFormatter : IDisposable {
		#region Fields
		ParagraphIndex paragraphIndex = new ParagraphIndex(-1);
		readonly DocumentFormattingController controller;
		ParagraphBoxFormatter paragraphFormatter;
		DocumentFormatterState state;
		#endregion
		public DocumentFormatter(DocumentFormattingController controller) {
			Guard.ArgumentNotNull(controller, "controller");
			this.controller = controller;
			Init();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return Controller.DocumentModel; } }
		public PieceTable PieceTable { get { return Controller.PieceTable; } }
		public ParagraphBoxFormatter ParagraphFormatter { get { return paragraphFormatter; } }
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } set { paragraphIndex = value; } }
		protected internal DocumentFormattingController Controller { get { return controller; } }
		protected internal BoxMeasurer Measurer { get { return Controller.DocumentLayout.Measurer; } }
		#endregion
		internal void Init() {
			this.paragraphIndex = new ParagraphIndex(-1);
			this.paragraphFormatter = CreateParagraphBoxFormatter(controller);
			ChangeState(DocumentFormatterStateType.BeginParagraphFormatting);
		}
		protected internal virtual ParagraphBoxFormatter CreateParagraphBoxFormatter(DocumentFormattingController controller) {
			return new ParagraphBoxFormatter(PieceTable, Measurer, controller.RowsController);
		}
		protected internal virtual void OnNewMeasurementAndDrawingStrategyChanged() {
			paragraphFormatter.OnNewMeasurementAndDrawingStrategyChanged(Measurer);
		}
		public FormattingProcessResult FormatNextRow() {
			for (; ; ) {				
				if (state.FormatNextRow() == FormattingProcess.Finish)
					break;
			}
			if (state.Type == DocumentFormatterStateType.Final) {
				controller.NotifyDocumentFormattingComplete();
				return new FormattingProcessResult(FormattingProcess.Finish);
			}
			else if (state.Type == DocumentFormatterStateType.ContinueFromParagraph)
				return new FormattingProcessResult(((ContinueFromParagraph)state).NextParagraphIndex);
			else
				return new FormattingProcessResult(FormattingProcess.Continue);
		}
		public void ChangeStateContinueFromParagraph(ParagraphIndex paragraphIndex) {			
			this.state = new ContinueFromParagraph(this, paragraphIndex);			
		}
		protected internal virtual void ChangeState(DocumentFormatterStateType stateType) {
			switch (stateType) {
				case DocumentFormatterStateType.BeginParagraphFormatting:
					this.state = new BeginParagraphFormatting(this);
					this.paragraphIndex++;
					break;
				case DocumentFormatterStateType.BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow:
					this.state = new BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow(this);
					this.paragraphIndex++;
					break;
				case DocumentFormatterStateType.ContinueParagraphFormatting:
					this.state = new ContinueParagraphFormatting(this);
					break;
				case DocumentFormatterStateType.EndParagraphFormatting:
					this.state = new EndParagraphFormatting(this);
					break;
				case DocumentFormatterStateType.EndHiddenParagraphFormatting:
					this.state = new EndHiddenParagraphFormatting(this);
					break;
				case DocumentFormatterStateType.Final:
					this.state = new DocumentFormattingFinished(this);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		protected internal virtual void OnParagraphFormattingComplete() {
			Controller.OnParagraphFormattingComplete(ParagraphIndex);
		}
		internal void Restart(DocumentModelPosition from) {
			this.paragraphIndex = from.ParagraphIndex - 1; 
		}
		internal void ResetFromTheStartOfRowAtCurrentPage(DocumentModelPosition from, bool keepFloatingObjects, bool forceRestart) {
			Controller.ResetFromTheStartOfRowAtCurrentPage(from, keepFloatingObjects, forceRestart);
				Restart(from);
				ChangeState(DocumentFormatterStateType.BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow);
				BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow newState = (BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow)this.state;
				newState.BeginFromParagraphStart = (from.LogPosition == PieceTable.Paragraphs[from.ParagraphIndex].LogPosition);
				newState.StartModelPosition = from;
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (paragraphFormatter != null) {
					paragraphFormatter.Dispose();
					paragraphFormatter = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
	}
	#endregion
	#region DocumentFormatterStateType
	public enum DocumentFormatterStateType {
		BeginParagraphFormatting,
		BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow,
		ContinueParagraphFormatting,
		EndParagraphFormatting,
		EndHiddenParagraphFormatting,
		ContinueFromParagraph,
		Final
	}
	#endregion
	#region DocumentFormatterState (abstract class)
	public abstract class DocumentFormatterState {
		readonly DocumentFormatter formatter;
		protected DocumentFormatterState(DocumentFormatter formatter) {
			Guard.ArgumentNotNull(formatter, "formatter");
			this.formatter = formatter;
		}
		protected DocumentFormatter Formatter { get { return formatter; } }
		public abstract DocumentFormatterStateType Type { get; }
		public abstract FormattingProcess FormatNextRow();
	}
	#endregion
	#region ContinueFromParagraph
	public class ContinueFromParagraph : DocumentFormatterState {
		ParagraphIndex nextParagraphIndex;
		public ContinueFromParagraph(DocumentFormatter formatter, ParagraphIndex nextParagraphIndex)
			: base(formatter) {
			this.nextParagraphIndex = nextParagraphIndex;			
		}
		public override DocumentFormatterStateType Type { get { return DocumentFormatterStateType.ContinueFromParagraph; } }
		public ParagraphIndex NextParagraphIndex { get { return nextParagraphIndex; } }
		public override FormattingProcess FormatNextRow() {
			Formatter.ParagraphIndex = NextParagraphIndex - 1;
			Formatter.ChangeState(DocumentFormatterStateType.BeginParagraphFormatting);
			return FormattingProcess.Continue;
		}
	}
	#endregion
	#region BeginParagraphFormatting
	public class BeginParagraphFormatting : DocumentFormatterState {
		bool isHeaderFooter;
		public BeginParagraphFormatting(DocumentFormatter formatter)
			: base(formatter) {
			this.isHeaderFooter = formatter.PieceTable != formatter.DocumentModel.MainPieceTable;
		}
		public override DocumentFormatterStateType Type { get { return DocumentFormatterStateType.BeginParagraphFormatting; } }
		public virtual bool BeginFromParagraphStart { get { return true; } set { } }
		public override FormattingProcess FormatNextRow() {
			bool hasVisibleBoxes = EnsureParagraphBoxes();
			RowsController rowsController = this.Formatter.Controller.RowsController;
			Paragraph currentParagraph = Formatter.PieceTable.Paragraphs[Formatter.ParagraphIndex];
			if(!hasVisibleBoxes) {
				if(Formatter.ParagraphFormatter.Iterator == null)
					Formatter.ParagraphFormatter.Iterator = CreateIterator();
				Formatter.ChangeState(DocumentFormatterStateType.EndHiddenParagraphFormatting);
				return FormattingProcess.Continue;
			}
			if (Formatter.ParagraphFormatter.GetActualParagraphFrameProperties(currentParagraph) != null && !rowsController.ParagraphFramesLayout.ContainsParagraph(currentParagraph) && rowsController.FrameParagraphIndex == new ParagraphIndex(-1)) {
				Formatter.ParagraphFormatter.BeginParagraphFormatting(CreateIterator(), BeginFromParagraphStart);
				StateParagraphFrame state = new StateParagraphFrame(Formatter.ParagraphFormatter, Formatter.ParagraphFormatter.State);
				state.ContinueFormat();
				if (rowsController.FrameParagraphIndex != new ParagraphIndex(-1)) {
					Formatter.ParagraphIndex = rowsController.FrameParagraphIndex;
					rowsController.FrameParagraphIndex = new ParagraphIndex(-1);
				}
				if (Formatter.ParagraphIndex + 1 <= Formatter.PieceTable.Paragraphs.Last.Index)
					Formatter.ParagraphIndex++;
				return FormattingProcess.Finish;
			}
			ParagraphBoxIterator iterator = CreateIterator();
			Formatter.ParagraphFormatter.BeginParagraphFormatting(iterator, BeginFromParagraphStart);
			Formatter.ChangeState(DocumentFormatterStateType.ContinueParagraphFormatting);
			return FormattingProcess.Continue;
		}
		protected internal virtual ParagraphBoxIterator CreateIterator() {
			PieceTable pieceTable = Formatter.PieceTable;
			IVisibleTextFilter visibleTextFilter = pieceTable.VisibleTextFilter;
			return new ParagraphBoxIterator(pieceTable.Paragraphs[Formatter.ParagraphIndex], pieceTable, visibleTextFilter);
		}
		protected virtual bool EnsureParagraphBoxes() {
			PieceTable pieceTable = Formatter.PieceTable;
			Paragraph paragraph = pieceTable.Paragraphs[Formatter.ParagraphIndex];
			bool hasVisibleBox;
			if (!paragraph.BoxCollection.IsValid || isHeaderFooter) {
				IVisibleTextFilter visibleTextFilter = pieceTable.VisibleTextFilter;
				ParagraphCharacterIterator characterIterator = new ParagraphCharacterIterator(paragraph, pieceTable, visibleTextFilter);
				if (characterIterator.RunIndex <= paragraph.LastRunIndex) {
					ParagraphCharacterFormatter preFormatter = new ParagraphCharacterFormatter(pieceTable, Formatter.Measurer);
					preFormatter.Format(characterIterator);
					hasVisibleBox = true;
				}
				else {
					hasVisibleBox = false;
					paragraph.BoxCollection.Clear();
				}
			}
			else {
				hasVisibleBox = paragraph.BoxCollection.Count > 0;
			}
			paragraph.BoxCollection.ParagraphStartRunIndex = paragraph.FirstRunIndex;
			return hasVisibleBox;
		}
	}
	#endregion
	#region BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow
	public class BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow : BeginParagraphFormatting {
		DocumentModelPosition startModelPosition;
		bool beginFromParagraphStart;
		public BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow(DocumentFormatter formatter)
			: base(formatter) {
		}
		public override DocumentFormatterStateType Type { get { return DocumentFormatterStateType.BeginParagraphFormattingFromTheMiddleOfParagraphAndStartOfRow; } }
		public DocumentModelPosition StartModelPosition { get { return startModelPosition; } set { startModelPosition = value; } }
		public override bool BeginFromParagraphStart { get { return beginFromParagraphStart; } set { beginFromParagraphStart = value; } }
		protected override bool EnsureParagraphBoxes() {
#if DEBUGTEST
			PieceTable pieceTable = Formatter.PieceTable;
			Paragraph paragraph = pieceTable.Paragraphs[Formatter.ParagraphIndex];
			Debug.Assert(paragraph.BoxCollection.IsValid);
#endif
			return true;
		}
		protected internal override ParagraphBoxIterator CreateIterator() {
			ParagraphBoxIterator iterator = base.CreateIterator();
			FormatterPosition formatterPosition = new FormatterPosition();
			if (!iterator.VisibleTextFilter.IsRunVisible(StartModelPosition.RunIndex)) {
				RunIndex runIndex = iterator.VisibleTextFilter.GetNextVisibleRunIndex(StartModelPosition.RunIndex);
				formatterPosition.SetRunIndex(runIndex);
				formatterPosition.SetOffset(0);
			}
			else {
				formatterPosition.SetRunIndex(StartModelPosition.RunIndex);
				formatterPosition.SetOffset(StartModelPosition.RunOffset);
			}
			PieceTable pieceTable = Formatter.PieceTable;
			Paragraph paragraph = pieceTable.Paragraphs[Formatter.ParagraphIndex];
			int boxIndex = Algorithms.BinarySearch(paragraph.BoxCollection.InnerCollection, new BoxAndFormatterPositionComparable<Box>(formatterPosition));
			Debug.Assert(boxIndex >= 0);
			formatterPosition.SetBoxIndex(boxIndex);
			iterator.SetPosition(formatterPosition);
			return iterator;
		}
	}
	#endregion
	#region ContinueParagraphFormatting
	public class ContinueParagraphFormatting : DocumentFormatterState {
		public ContinueParagraphFormatting(DocumentFormatter formatter)
			: base(formatter) {
		}
		public override DocumentFormatterStateType Type { get { return DocumentFormatterStateType.ContinueParagraphFormatting; } }
		public override FormattingProcess FormatNextRow() {
			FormattingProcessResult formattingProcessResult = Formatter.ParagraphFormatter.FormatNextRow();
			if (formattingProcessResult.FormattingProcess == FormattingProcess.Finish) {
				Formatter.ChangeState(DocumentFormatterStateType.EndParagraphFormatting);
				return FormattingProcess.Continue;
			}
			else if (formattingProcessResult.FormattingProcess == FormattingProcess.ContinueFromParagraph) {
				Formatter.ChangeStateContinueFromParagraph(formattingProcessResult.ParagraphIndex);
				return FormattingProcess.Finish;
			}
			else if (formattingProcessResult.FormattingProcess == FormattingProcess.RestartFromTheStartOfRow) {
				Formatter.ResetFromTheStartOfRowAtCurrentPage(formattingProcessResult.RestartPosition, true, formattingProcessResult.ForceRestart);
				return FormattingProcess.Continue;
			}
			else
				return FormattingProcess.Finish;
		}
	}
	#endregion   
	#region EndHiddenParagraphFormatting
	public class EndHiddenParagraphFormatting : DocumentFormatterState {
		public EndHiddenParagraphFormatting(DocumentFormatter formatter)
			: base(formatter) {
		}
		public override DocumentFormatterStateType Type { get { return DocumentFormatterStateType.EndHiddenParagraphFormatting; } }
		public override FormattingProcess FormatNextRow() {			
			ParagraphCollection paragraphs = Formatter.PieceTable.Paragraphs;
			ParagraphIndex count = new ParagraphIndex(paragraphs.Count);
			if (Formatter.ParagraphFormatter.Iterator != null) {
				FormatterPosition lastPosition = Formatter.ParagraphFormatter.Iterator.CreatePosition();
				lastPosition.SetRunIndex(paragraphs[Formatter.ParagraphIndex].LastRunIndex + 1);
				Formatter.ParagraphFormatter.Iterator.SetPositionCore(lastPosition);
			}
			if (Formatter.ParagraphIndex + 1 >= count)
				Formatter.ChangeState(DocumentFormatterStateType.Final);
			else
				Formatter.ChangeState(DocumentFormatterStateType.BeginParagraphFormatting);
			return FormattingProcess.Finish;
		}
	}
	#endregion
	#region EndParagraphFormatting
	public class EndParagraphFormatting : DocumentFormatterState {
		public EndParagraphFormatting(DocumentFormatter formatter)
			: base(formatter) {
		}
		public override DocumentFormatterStateType Type { get { return DocumentFormatterStateType.EndParagraphFormatting; } }
		public override FormattingProcess FormatNextRow() {
			Formatter.ParagraphFormatter.EndParagraphFormatting();
			Formatter.OnParagraphFormattingComplete();
			ParagraphCollection paragraphs = Formatter.PieceTable.Paragraphs;
			ParagraphIndex count = new ParagraphIndex(paragraphs.Count);
			ParagraphIndex paragraphIndex = Formatter.ParagraphIndex;
			IVisibleTextFilter visibleTextFilter = Formatter.PieceTable.VisibleTextFilter;
			while (paragraphIndex < count && !visibleTextFilter.IsRunVisible(paragraphs[paragraphIndex].LastRunIndex)) {
				paragraphIndex++;
			}
			while (paragraphIndex + 1 < count && IsParagraphInInvisibleCell(paragraphs[paragraphIndex + 1])) {
				paragraphIndex++;
			}
			Formatter.ParagraphIndex = paragraphIndex;
			MergedFrameProperties currentParagraphFrameProperties = Formatter.ParagraphFormatter.GetActualParagraphFrameProperties(Formatter.PieceTable.Paragraphs[Formatter.ParagraphIndex]);
			if (Formatter.ParagraphIndex + 1 < count) {
				MergedFrameProperties nextParagraphFrameProperties = Formatter.PieceTable.Paragraphs[Formatter.ParagraphIndex + 1].GetMergedFrameProperties();
				if (currentParagraphFrameProperties != null && nextParagraphFrameProperties != null) {
					if (currentParagraphFrameProperties.CanMerge(nextParagraphFrameProperties)) {
						Formatter.ChangeState(DocumentFormatterStateType.BeginParagraphFormatting);
						return FormattingProcess.Finish;
					}
				}
			}
			if (Formatter.ParagraphIndex + 1 >= count || currentParagraphFrameProperties != null)
				Formatter.ChangeState(DocumentFormatterStateType.Final);
			else
				Formatter.ChangeState(DocumentFormatterStateType.BeginParagraphFormatting);
			return FormattingProcess.Finish;
		}
		bool IsParagraphInInvisibleCell(Paragraph paragraph) {
			TableCell cell = paragraph.GetCell();
			return cell != null && cell.VerticalMerging == MergingState.Continue;
		}
	}
	#endregion
	#region DocumentFormattingFinished
	public class DocumentFormattingFinished : DocumentFormatterState {
		public DocumentFormattingFinished(DocumentFormatter formatter)
			: base(formatter) {
		}
		public override DocumentFormatterStateType Type { get { return DocumentFormatterStateType.Final; } }
		public override FormattingProcess FormatNextRow() {
			return FormattingProcess.Finish;
		}
	}
	#endregion
}
