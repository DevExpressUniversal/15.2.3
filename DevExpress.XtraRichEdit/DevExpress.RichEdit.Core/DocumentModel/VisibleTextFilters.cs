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
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Model {
	public interface IVisibleTextFilter {
		IVisibleTextFilter Clone(PieceTable pieceTable);
		bool IsRunVisible(RunIndex runIndex);
		RunVisibility GetRunVisibilityInField(RunIndex runIndex);
		RunIndex GetPrevVisibleRunIndex(RunIndex runIndex);
		RunIndex GetNextVisibleRunIndex(RunIndex runIndex);
		RunIndex FindVisibleRunForward(RunIndex runIndex);
		RunIndex FindVisibleParagraphRunForward(RunIndex runIndex);
		DocumentLogPosition GetNextVisibleLogPosition(DocumentModelPosition position, bool skipFieldBoundaryRanges);
		DocumentLogPosition GetNextVisibleLogPosition(DocumentModelPosition position, bool skipFieldBoundaryRanges, bool insureNextLogPositionVisible);
		DocumentLogPosition GetPrevVisibleLogPosition(DocumentModelPosition position, bool skipFieldBoundaryRanges);
		DocumentLogPosition GetVisibleLogPosition(DocumentModelPosition position, bool skipFieldBoundaryRanges);
		DocumentLogPosition GetNextVisibleLogPosition(DocumentLogPosition position, bool skipFieldResultEnd);
		DocumentLogPosition GetNextVisibleLogPosition(DocumentLogPosition position, bool skipFieldResultEnd, bool insureNextLogPositionVisible);
		DocumentLogPosition GetPrevVisibleLogPosition(DocumentLogPosition position, bool skipFieldBoundaryRanges);
		DocumentLogPosition GetVisibleLogPosition(DocumentModelPosition position);
	}
	public enum RunVisibility {
		Hidden,
		Visible,
		ForceVisible
	}
	public abstract class VisibleTextFilterBase : IVisibleTextFilter {
		readonly PieceTable pieceTable;
		protected VisibleTextFilterBase(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		protected internal PieceTable PieceTable { get { return pieceTable; } }
		public virtual bool IsRunVisible(RunIndex runIndex) {
			return IsRunVisibleCore(runIndex) != RunVisibility.Hidden;
		}
		public RunVisibility GetRunVisibilityInField(RunIndex runIndex) {
			Field field = PieceTable.FindFieldByRunIndex(runIndex);
			if (field != null)
				return IsFieldRunVisible(runIndex, field);
			return RunVisibility.Visible;
		}
		protected virtual RunVisibility IsRunVisibleCore(RunIndex runIndex) {
			TextRunBase run = PieceTable.Runs[runIndex];
			if (run is FieldResultEndRun)
				return RunVisibility.Hidden;
			if (run is SeparatorTextRun && run.Hidden)
				return RunVisibility.Hidden;
			RunVisibility fieldRunVisibility = GetRunVisibilityInField(runIndex);
			if (fieldRunVisibility != RunVisibility.Visible)
				return fieldRunVisibility;
			ParagraphRun paragraphRun = run as ParagraphRun;
			if (paragraphRun != null) {
				SectionRun sectionRun = paragraphRun as SectionRun;
				if (sectionRun != null)
					return RunVisibility.ForceVisible;
				TableCell cell = paragraphRun.Paragraph.GetCell();
				if (cell != null) {
					if (cell.VerticalMerging == MergingState.Continue)
						return RunVisibility.Hidden;
					if (cell.EndParagraphIndex == paragraphRun.Paragraph.Index)
						return RunVisibility.ForceVisible;
				}
				Paragraph paragraph = paragraphRun.Paragraph;
				ParagraphIndex nextParagraphIndex = paragraph.Index + 1;
				if (nextParagraphIndex < new ParagraphIndex(pieceTable.Paragraphs.Count)) {
					if (pieceTable.Paragraphs[nextParagraphIndex].GetCell() != cell) {
						if (HasVisibleRunInParagraph(Algorithms.Min(runIndex, paragraph.LastRunIndex)))
							return RunVisibility.ForceVisible;
						else
							return RunVisibility.Visible;
					}
				}
			}
			return RunVisibility.Visible;
		}
		bool HasVisibleRunInParagraph(RunIndex lastParagraphRun) {
			RunIndex runIndex = lastParagraphRun - 1;
			while (runIndex >= RunIndex.Zero) {
				if (IsRunVisible(runIndex)) {
					ParagraphRun paragraphRun = PieceTable.Runs[runIndex] as ParagraphRun;
					return paragraphRun == null;
				}
				runIndex--;
			}
			return false;
		}
		protected internal bool IsFieldRunVisibleCore(RunIndex runIndex, Field field) {
			if (field.IsCodeView && field.Code.Contains(runIndex))
				return true;
			if (!field.IsCodeView && field.Result.Contains(runIndex))
				return true;
			return false;
		}
		protected internal virtual RunVisibility IsFieldRunVisible(RunIndex runIndex, Field field) {
			if (IsFieldRunVisibleCore(runIndex, field)) {
				if (field.Parent != null)
					return IsFieldRunVisible(runIndex, field.Parent);
				else
					return RunVisibility.Visible;
			}
			else
				return RunVisibility.Hidden;
		}
		public virtual RunIndex FindVisibleParagraphRunForward(RunIndex runIndex) {
			TextRunCollection runs = PieceTable.Runs;
			RunIndex maxRunIndex = new RunIndex(runs.Count - 1);
			while (runIndex < maxRunIndex && !(IsRunVisible(runIndex) && runs[runIndex] is ParagraphRun))
				runIndex++;
			return runIndex;
		}
		public virtual RunIndex GetNextVisibleRunIndex(RunIndex runIndex) {
			RunIndex maxIndex = new RunIndex(PieceTable.Runs.Count - 1);
			RunIndex result = runIndex;
			while (result < maxIndex) {
				result++;
				if (IsRunVisible(result))
					break;
			}
			return result;
		}
		public virtual RunIndex GetPrevVisibleRunIndex(RunIndex runIndex) {
			RunIndex maxIndex = new RunIndex(PieceTable.Runs.Count - 1);
			RunIndex result = runIndex;
			while (result > RunIndex.Zero) {
				result--;
				if (IsRunVisible(result))
					break;
			}
			return result;
		}
		public virtual RunIndex FindVisibleRunForward(RunIndex runIndex) {
			TextRunCollection runs = PieceTable.Runs;
			RunIndex maxRunIndex = new RunIndex(runs.Count - 1);
			while (runIndex < maxRunIndex && !IsRunVisible(runIndex))
				runIndex++;
			return runIndex;
		}
		public virtual DocumentLogPosition GetNextVisibleLogPosition(DocumentModelPosition position, bool skipFieldBoundaryRanges) {
			return GetNextVisibleLogPosition(position, skipFieldBoundaryRanges, false);
		}
		public virtual DocumentLogPosition GetNextVisibleLogPosition(DocumentModelPosition position, bool skipFieldBoundaryRanges, bool insureNextLogPositionVisible) {
			DocumentModelPosition result = position;
			bool charSkipped = false;
			do {
				bool visibleCharSkipped;
				result = GetNextVisibleLogPositionCore(result, skipFieldBoundaryRanges, out visibleCharSkipped);
				charSkipped |= visibleCharSkipped;
			} while((skipFieldBoundaryRanges && (IsPositionBeforeFieldResultEndRun(result) || !charSkipped || IsPositionAfterFieldCodeEndRun(result))) || IsPositionOnInvisibleCellParagraph(result));
			if(insureNextLogPositionVisible)
				return GetVisibleLogPosition(position);
			return result.LogPosition;
		}
		private bool IsPositionOnInvisibleCellParagraph(DocumentModelPosition position) {
			int runOffset = position.RunOffset;
			int runLength = position.RunEndLogPosition - position.RunStartLogPosition + 1;
			if (runOffset != 0 && runOffset != runLength)
				return false;
			RunIndex runIndex = position.RunIndex;
			if (runOffset == runLength)
				runIndex++;			
			TextRunCollection runs = PieceTable.Runs;
			if (runIndex >= new RunIndex(runs.Count))
				return false;
			ParagraphRun run = runs[runIndex] as ParagraphRun;
			if (run != null) {
				TableCell cell = run.Paragraph.GetCell();
				if (cell != null && cell.VerticalMerging == MergingState.Continue)
					return true;
			}
			return false;
		}
		protected virtual bool IsPositionAfterFieldCodeEndRun(DocumentModelPosition position) {
			RunIndex runIndex = position.RunIndex;
			TextRunCollection runs = PieceTable.Runs;
			if (runs[runIndex] is FieldCodeEndRun && position.RunOffset == runs[runIndex].Length)
				return true;
			if (runIndex > RunIndex.Zero && position.RunStartLogPosition == position.LogPosition && runs[runIndex - 1] is FieldCodeEndRun)
				return true;
			return false;
		}
		protected virtual bool IsPositionBeforeInvisibleFieldCodeEndRun(DocumentModelPosition position) {
			TextRunCollection runs = PieceTable.Runs;
			if (position.LogPosition == position.RunStartLogPosition && runs[position.RunIndex] is FieldCodeStartRun) {
				return !IsPositionVisible(position.RunIndex);
			}
			if (position.RunIndex + 1 < new RunIndex(runs.Count) &&
				position.LogPosition == position.RunEndLogPosition + 1 &&
				runs[position.RunIndex + 1] is FieldCodeStartRun)
				return !IsPositionVisible(position.RunIndex + 1);
			return false;
		}
		protected virtual bool IsPositionVisible(RunIndex fieldCodeStartRunIndex) {
			Field field = PieceTable.FindFieldByRunIndex(fieldCodeStartRunIndex);
			Field parent = field.Parent;
			if (parent == null)
				return true;
			if (parent.Code.Contains(field))
				return parent.IsCodeView;
			else
				return !parent.IsCodeView;
		}
		protected virtual bool IsPositionBeforeInvisibleFieldCodeStartRun(DocumentModelPosition position) {
			TextRunCollection runs = PieceTable.Runs;
			if (position.LogPosition == position.RunStartLogPosition && runs[position.RunIndex] is FieldCodeStartRun)
				return !IsPositionVisible(position.RunIndex);
			if (position.RunIndex + 1 < new RunIndex(runs.Count) &&
				position.LogPosition == position.RunEndLogPosition + 1 &&
				runs[position.RunIndex + 1] is FieldCodeStartRun)
				return !IsPositionVisible(position.RunIndex + 1);
			return false;
		}
		protected virtual bool IsPositionBeforeFieldResultEndRun(DocumentModelPosition position) {
			TextRunCollection runs = PieceTable.Runs;
			if (position.LogPosition == position.RunStartLogPosition && runs[position.RunIndex] is FieldResultEndRun)
				return true;
			if (position.RunIndex + 1 < new RunIndex(runs.Count) &&
				position.LogPosition == position.RunEndLogPosition + 1 &&
				runs[position.RunIndex + 1] is FieldResultEndRun)
				return true;
			return false;
		}
		DocumentModelPosition GetNextVisibleLogPositionCore(DocumentModelPosition position, bool breakAfterFieldResultEndRun, out bool visibleCharSkipped) {
			TextRunCollection runs = PieceTable.Runs;
			RunIndex runIndex = position.RunIndex;
			DocumentLogPosition result = position.LogPosition;
			RunIndex maxIndex = new RunIndex(PieceTable.Runs.Count - 1);
			visibleCharSkipped = false;
			if (position.RunOffset < runs[runIndex].Length) {
				if(IsRunVisible(runIndex)) {
					position.LogPosition = result + 1;
					if(position.LogPosition > position.RunEndLogPosition && new RunIndex(runs.Count - 1) != runIndex) {
						position.RunIndex++;
						position.RunStartLogPosition = position.LogPosition;
					}
					visibleCharSkipped = true;
					return position;
				}
				else
					result += runs[runIndex].Length - position.RunOffset;
			}
			DocumentLogPosition runStartPosition = result;
			while (runIndex < maxIndex) {
				runStartPosition = result;
				runIndex++;
				if (IsRunVisible(runIndex))
					break;
				result += runs[runIndex].Length;
				if (breakAfterFieldResultEndRun && runs[runIndex] is FieldResultEndRun)
					break;
			}
			position.LogPosition = result;
			position.RunIndex = runIndex;
			position.RunStartLogPosition = runStartPosition;
			return position;
		}
		public virtual DocumentLogPosition GetPrevVisibleLogPosition(DocumentModelPosition position, bool skipFieldBoundaryRanges) {
			DocumentModelPosition result = position;
			bool charSkipped = false;
			do {
				bool visibleCharSkipped;
				result = GetPrevVisiblePositionCore(result, skipFieldBoundaryRanges, out visibleCharSkipped);
				charSkipped |= visibleCharSkipped;
				if (result.LogPosition == DocumentLogPosition.Zero)
					break;
			} while ((skipFieldBoundaryRanges && (IsPositionBeforeFieldResultEndRun(result) || !charSkipped || IsPositionAfterFieldCodeEndRun(result) || IsPositionBeforeInvisibleFieldCodeEndRun(result))) || IsPositionOnInvisibleCellParagraph(result));
			return result.LogPosition;
		}
		public virtual DocumentLogPosition GetVisibleLogPosition(DocumentModelPosition position, bool skipFieldBoundaryRanges) {
			DocumentModelPosition result = position;
			while ((skipFieldBoundaryRanges && (IsPositionBeforeFieldResultEndRun(result) || IsPositionAfterFieldCodeEndRun(result) || IsPositionBeforeInvisibleFieldCodeStartRun(result))) || IsPositionOnInvisibleCellParagraph(result)) {
				bool visibleCharSkipped;
				result = GetPrevVisiblePositionCore(result, skipFieldBoundaryRanges, out visibleCharSkipped);
				if (result.LogPosition == DocumentLogPosition.Zero)
					break;
			}
			return result.LogPosition;
		}
		DocumentModelPosition GetPrevVisiblePositionCore(DocumentModelPosition position, bool breakOnFieldCodeStart, out bool visibleCharSkipped) {
			DocumentLogPosition result = position.LogPosition;
			TextRunCollection runs = PieceTable.Runs;
			RunIndex runIndex = position.RunIndex;
			visibleCharSkipped = false;
			if (position.RunOffset > 0) {
				if (IsRunVisible(runIndex)) {
					position.LogPosition = result - 1;
					visibleCharSkipped = true;
					return position;
				}
				else
					result = position.RunStartLogPosition;
			}
			DocumentLogPosition runStartLogPosition = result;
			while (runIndex > RunIndex.Zero) {
				runIndex--;
				runStartLogPosition = result - runs[runIndex].Length;
				if (IsRunVisible(runIndex))
					break;
				result -= runs[runIndex].Length;
				if (breakOnFieldCodeStart && runs[runIndex] is FieldCodeStartRun) {
					break;
				}
			}
			position.LogPosition = result;
			position.RunIndex = runIndex;
			position.RunStartLogPosition = runStartLogPosition;
			return position;
		}
		public virtual DocumentLogPosition GetVisibleLogPosition(DocumentModelPosition position) {
			TextRunCollection runs = PieceTable.Runs;
			RunIndex runIndex = position.RunIndex;
			DocumentLogPosition result = position.LogPosition;
			if (IsRunVisible(runIndex))
				return result;
			else
				result += runs[runIndex].Length - position.RunOffset;
			RunIndex maxIndex = new RunIndex(PieceTable.Runs.Count - 1);
			while (runIndex < maxIndex) {
				runIndex++;
				if (IsRunVisible(runIndex))
					break;
				result += runs[runIndex].Length;
			}
			return result;
		}
		public virtual DocumentLogPosition GetNextVisibleLogPosition(DocumentLogPosition logPosition, bool skipFieldResultEnd) {
			return GetNextVisibleLogPosition(logPosition, skipFieldResultEnd, false);
		}
		public virtual DocumentLogPosition GetNextVisibleLogPosition(DocumentLogPosition logPosition, bool skipFieldResultEnd, bool insureNextLogPositionVisible) {
			DocumentModelPosition modelPosition = GetModelPosition(logPosition);
			return GetNextVisibleLogPosition(modelPosition, skipFieldResultEnd, insureNextLogPositionVisible);
		}
		public virtual DocumentLogPosition GetPrevVisibleLogPosition(DocumentLogPosition logPosition, bool skipFieldBoundaryRanges) {
			DocumentModelPosition modelPosition = GetModelPosition(logPosition);
			return GetPrevVisibleLogPosition(modelPosition, skipFieldBoundaryRanges);
		}
		protected internal virtual DocumentModelPosition GetModelPosition(DocumentLogPosition logPosition) {
			DocumentModelPosition result = new DocumentModelPosition(PieceTable);
			result.ParagraphIndex = PieceTable.FindParagraphIndex(logPosition);
			RunIndex runIndex;
			result.RunStartLogPosition = PieceTable.FindRunStartLogPosition(PieceTable.Paragraphs[result.ParagraphIndex], logPosition, out runIndex);
			result.RunIndex = runIndex;
			result.LogPosition = logPosition;
			return result;
		}
		public abstract IVisibleTextFilter Clone(PieceTable pieceTable);
	}
	public class EmptyTextFilter : VisibleTextFilterBase {
		public EmptyTextFilter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override IVisibleTextFilter Clone(PieceTable pieceTable) {
			return new EmptyTextFilter(pieceTable);
		}
	}
	public class EmptyTextFilterSkipFloatingObjects : EmptyTextFilter {
		public EmptyTextFilterSkipFloatingObjects(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override bool IsRunVisible(RunIndex runIndex) {
			return base.IsRunVisible(runIndex) && !(PieceTable.Runs[runIndex] is FloatingObjectAnchorRun);
		}
	}
	public class VisibleTextFilter : VisibleTextFilterBase {
		public VisibleTextFilter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override IVisibleTextFilter Clone(PieceTable pieceTable) {
			return new VisibleTextFilter(pieceTable);
		}
		public override bool IsRunVisible(RunIndex runIndex) {
			return IsRunVisibleCore(runIndex) != RunVisibility.Hidden || runIndex == new RunIndex(PieceTable.Runs.Count - 1);
		}
		protected override RunVisibility IsRunVisibleCore(RunIndex runIndex) {
			RunVisibility visibility = base.IsRunVisibleCore(runIndex);
			if (visibility != RunVisibility.Visible)
				return visibility;
			bool shouldDisplayHiddenText = PieceTable.DocumentModel.FormattingMarkVisibilityOptions.HiddenText == RichEditFormattingMarkVisibility.Visible;
			return (!PieceTable.Runs[runIndex].Hidden || shouldDisplayHiddenText) ? RunVisibility.Visible : RunVisibility.Hidden;
		}
	}
	public class VisibleTextFilterSkipFloatingObjects : VisibleTextFilter {
		public VisibleTextFilterSkipFloatingObjects(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override bool IsRunVisible(RunIndex runIndex) {
			return base.IsRunVisible(runIndex) && !(PieceTable.Runs[runIndex] is FloatingObjectAnchorRun);
		}
	}
	public class VisibleOnlyTextFilter : VisibleTextFilterBase {
		public VisibleOnlyTextFilter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override IVisibleTextFilter Clone(PieceTable pieceTable) {
			return new VisibleOnlyTextFilter(pieceTable);
		}
		public override bool IsRunVisible(RunIndex runIndex) {
			return IsRunVisibleCore(runIndex) != RunVisibility.Hidden || runIndex == new RunIndex(PieceTable.Runs.Count - 1);
		}
		protected override RunVisibility IsRunVisibleCore(RunIndex runIndex) {
			RunVisibility visibility = base.IsRunVisibleCore(runIndex);
			if (visibility != RunVisibility.Visible)
				return visibility;
			return (!PieceTable.Runs[runIndex].Hidden) ? RunVisibility.Visible : RunVisibility.Hidden;
		}
	}
}
