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
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region CharacterLineCalculator<T> (abstract class)
	public abstract class CharacterLineCalculator<T> where T : struct {
		#region Fields
		protected const CharacterFormattingScript UnknownScript = (CharacterFormattingScript)(-1);
		BoxCollection boxes;
		Row row;
		PieceTable pieceTable;
		T currentRunCharacterLineType;		
		int boxCount;		
		CharacterFormattingScript currentScript;
		bool useForWordsOnly;
		int numberingListBoxesCount;
		NumberingListBox numberingListBox;
		#endregion
		protected CharacterLineCalculator(PieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		#region Properties
		protected internal BoxCollection Boxes { get { return boxes; } }
		protected internal Row Row { get { return row; } }
		protected internal DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		protected internal PieceTable PieceTable {
			get { return pieceTable; }
			set {
				Guard.ArgumentNotNull(value, "pieceTable");
				pieceTable = value;
			}
		}
		protected internal CharacterFormattingScript CurrentScript { get { return currentScript; } set { currentScript = value; } }
		protected internal int BoxCount { get { return boxCount; } }
		protected internal abstract T CharacterLineTypeNone { get; }
		internal bool UseForWordsOnly { get { return useForWordsOnly; } set { useForWordsOnly = value; } }
		internal T CurrentRunCharacterLineType { get { return currentRunCharacterLineType; } set { currentRunCharacterLineType = value; } }
		#endregion
		protected void SetRow(Row row) {
			this.row = row;
		}
		protected internal abstract void StartCharacterLine(int startAnchorIndex);
		protected internal abstract void EndCharacterLine(int endAnchorIndex, FontInfo currentFontInfo);
		protected internal abstract bool IsCharacterLineBreak(bool characterLineTypeChanged, bool runChanged, CharacterFormattingScript boxScript);
		public virtual void Calculate(Row row) {
			Guard.ArgumentNotNull(row, "row");
			BeforeCalculate(row);
			if (boxCount == 0)
				return;
			int lastNonSpaceBoxIndex = FindLastNonSpaceBoxIndex();
			if (lastNonSpaceBoxIndex < 0)
				return;
			CalculateCharacterLineBoxesByType(lastNonSpaceBoxIndex);
			AfterCalculate();
		}
		public int FindLastNonSpaceBoxIndex() {
			return FindLastNonSpaceBoxIndex(0);
		}
		public int FindLastNonSpaceBoxIndex(int startIndex) {
			return Row.FindLastNonSpaceBoxIndex(Boxes, startIndex);
		}
		protected internal virtual void AfterCalculate() {
		}
		protected internal virtual void BeforeCalculate(Row row) {
			Initialize(row);
		}
		protected internal virtual void Initialize(Row row) {
			if (row.NumberingListBox != null) {
				numberingListBox = row.NumberingListBox;				
				this.boxCount = row.Boxes.Count + 1;
				this.boxes = new BoxCollection(this.boxCount);
				this.boxes.Add(numberingListBox);
				this.numberingListBoxesCount = 1;
				this.boxes.AddRange(row.Boxes);
			}
			else {
				this.boxes = row.Boxes;
				this.boxCount = boxes.Count;
				this.numberingListBoxesCount = 0;
			}
			this.row = row;
		}
		protected internal virtual void AddBoxToCharacterLine(Box box) {
		}
		protected internal virtual void CalculateCharacterLineBoxesByType(int lastNonSpaceBoxIndex) {
#if DEBUG || DEBUGTEST
			Debug.Assert(lastNonSpaceBoxIndex >= 0);
#endif
			FontInfo currentFontInfo = null;
			RunIndex prevBoxIndex = new RunIndex(-1);
			currentRunCharacterLineType = CharacterLineTypeNone;
			currentScript = UnknownScript;
			T currentCharacterLineType = CharacterLineTypeNone;
			CharacterFormattingScript boxScript = CharacterFormattingScript.Normal;
			for (int i = 0; i <= lastNonSpaceBoxIndex; i++) {				
				Box box = boxes[i];
				bool runChanged;
				if (i >= numberingListBoxesCount) {
					runChanged = SetCurrentRunCharacterLineTypeFromBox(box, prevBoxIndex);
					prevBoxIndex = box.StartPos.RunIndex;
				}
				else {
					runChanged = SetCurrentRunCharacterLineTypeFromNumberingListBox();
				}
				T boxCharacterLineType = CalculateActualCharacterLineType(box);
				bool characterLineTypeChanged = !currentCharacterLineType.Equals(boxCharacterLineType);
				if (runChanged)
					boxScript = GetBoxScript(i);
				CharacterFormattingScript actualScript = CalculateActualScript(boxScript);				
				if (IsCharacterLineBreak(characterLineTypeChanged, runChanged, boxScript)) {
					if (!currentCharacterLineType.Equals(CharacterLineTypeNone))
						EndCharacterLine(i, currentFontInfo);
					currentScript = actualScript;
					currentCharacterLineType = boxCharacterLineType;
					if (!currentCharacterLineType.Equals(CharacterLineTypeNone))
						StartCharacterLine(i);
					else
						currentScript = UnknownScript;
				}
				if (!currentCharacterLineType.Equals(CharacterLineTypeNone)) {
					if (runChanged) {
						if (i >= numberingListBoxesCount) {
							TextRun currentTextRun = (TextRun)pieceTable.Runs[prevBoxIndex];
							currentFontInfo = DocumentModel.FontCache[currentTextRun.FontCacheIndex];
						}
						else
							currentFontInfo = numberingListBox.GetFontInfo(PieceTable);
					}
					currentScript = actualScript;
					AddBoxToCharacterLine(box);
				}
			}
			if (!currentCharacterLineType.Equals(CharacterLineTypeNone))
				EndCharacterLine(lastNonSpaceBoxIndex + 1, currentFontInfo);
		}
		protected internal virtual bool SetCurrentRunCharacterLineTypeFromNumberingListBox() {			
			currentRunCharacterLineType = GetNumberingListBoxCharacterLineType(numberingListBox);
			useForWordsOnly = GetNumberingListBoxCharacterLineUseForWordsOnly(numberingListBox);
			return true;
		}
		protected internal virtual bool SetCurrentRunCharacterLineTypeFromBox(Box box, RunIndex prevBoxIndex) {
			if (box.StartPos.RunIndex == prevBoxIndex)
				return false;
			RunIndex currentRunIndex = box.StartPos.RunIndex;
			TextRun currentTextRun = pieceTable.Runs[currentRunIndex] as TextRun;
			if (currentTextRun != null) {
				currentRunCharacterLineType = GetRunCharacterLineType(currentTextRun, currentRunIndex);
				useForWordsOnly = GetRunCharacterLineUseForWordsOnly(currentTextRun, currentRunIndex);
			}
			else {
				currentRunCharacterLineType = CharacterLineTypeNone;
				useForWordsOnly = false;
			}
			return true;
		}
		protected internal abstract T GetRunCharacterLineType(TextRun run, RunIndex runIndex);
		protected internal abstract bool GetRunCharacterLineUseForWordsOnly(TextRun run, RunIndex runIndex);
		protected abstract T GetNumberingListBoxCharacterLineType(NumberingListBox numberingListBox);
		protected abstract bool GetNumberingListBoxCharacterLineUseForWordsOnly(NumberingListBox numberingListBox);
		protected internal virtual CharacterFormattingScript GetBoxScript(int boxIndex) {
			if (boxIndex < numberingListBoxesCount) {
				NumberingListBox numberingListBox = Boxes[0] as NumberingListBox;
				return numberingListBox.GetNumerationCharacterProperties(PieceTable).Info.Script;
			}
			else
				return Boxes[boxIndex].GetRun(PieceTable).Script;
		}
		protected internal virtual FontInfo GetBoxFontInfo(int boxIndex) {
			if (boxIndex < numberingListBoxesCount) {
				NumberingListBox numberingListBox = Boxes[0] as NumberingListBox;
				return numberingListBox.GetFontInfo(PieceTable);
			}
			else {
				int index = Boxes[boxIndex].GetRun(PieceTable).FontCacheIndex;
				return DocumentModel.FontCache[index];
			}
		}
		protected internal virtual T CalculateActualCharacterLineType(Box box) {
			if (useForWordsOnly)
				return box is ISpaceBox ? CharacterLineTypeNone : currentRunCharacterLineType;
			else
				return currentRunCharacterLineType;
		}
		protected internal virtual CharacterFormattingScript CalculateActualScript(CharacterFormattingScript script) {
			return script;
		}
		protected internal int CalculateScriptOffset(FontInfo fontInfo, CharacterFormattingScript script) {
			switch (script) {
				case CharacterFormattingScript.Subscript:
					return fontInfo.SubscriptOffset.Y;
				case CharacterFormattingScript.Superscript:
					return fontInfo.SuperscriptOffset.Y;
				case CharacterFormattingScript.Normal:
				default:
					return 0;
			}
		}
	}
	#endregion
	#region UnderlineInfo
	public class UnderlineInfo {
		#region Fields
		int totalUnderlineWidth;
		int weightedUnderlinePositionSum;
		int weightedUnderlineThicknessSum;
		int topOffset;
		#endregion
		#region Properties
		public int TotalUnderlineWidth { get { return totalUnderlineWidth; } set { totalUnderlineWidth = value; } }
		public int WeightedUnderlinePositionSum { get { return weightedUnderlinePositionSum; } set { weightedUnderlinePositionSum = value; } }
		public int WeightedUnderlineThicknessSum { get { return weightedUnderlineThicknessSum; } set { weightedUnderlineThicknessSum = value; } }
		public int TopOffset { get { return topOffset; } set { topOffset = value; } }
		#endregion
	}
	#endregion
	#region UnderlineCalculator
	public class UnderlineCalculator : CharacterLineCalculator<UnderlineType> {
		#region Fields
		int startAnchorIndex;
		UnderlineBoxCollection underlineBoxesByType;
		CharacterFormattingScript actualScript;
		#endregion
		public UnderlineCalculator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		#region Properties
		protected internal override UnderlineType CharacterLineTypeNone { get { return UnderlineType.None; } }
		protected internal int StartAnchorIndex { get { return startAnchorIndex; } set { startAnchorIndex = value; } }
		protected internal UnderlineBoxCollection UnderlineBoxesByType { get { return underlineBoxesByType; } set { underlineBoxesByType = value; } }
		protected internal CharacterFormattingScript ActualScript { get { return actualScript; } }
		#endregion
		protected internal override void BeforeCalculate(Row row) {
			base.BeforeCalculate(row);
			row.ClearUnderlines();
			underlineBoxesByType = new UnderlineBoxCollection();
		}
		protected internal override void AfterCalculate() {
			base.AfterCalculate();
			SplitUnderlinesByColor(underlineBoxesByType);
			SetUnderlinesBounds();
		}
		protected internal override CharacterFormattingScript CalculateActualScript(CharacterFormattingScript script) {
			if (script == CharacterFormattingScript.Subscript)
				return CharacterFormattingScript.Subscript;
			if (script == CharacterFormattingScript.Normal)
				return CharacterFormattingScript.Normal;
			if (CurrentScript == CharacterFormattingScript.Superscript || CurrentScript == UnknownScript)
				return CharacterFormattingScript.Superscript;
			return CharacterFormattingScript.Normal;
		}
		protected internal bool ShouldBreakCharacterLineByScript(CharacterFormattingScript script) {
			if (CurrentScript == UnknownScript)
				return true;
			if (CurrentScript == CharacterFormattingScript.Subscript && script == CharacterFormattingScript.Subscript)
				return false;
			return CurrentScript == CharacterFormattingScript.Subscript || script == CharacterFormattingScript.Subscript;
		}
		protected internal override bool IsCharacterLineBreak(bool characterLineTypeChanged, bool runChanged, CharacterFormattingScript boxScript) {
			bool shouldBreakCharacterLineByScript = ShouldBreakCharacterLineByScript(boxScript);
			return characterLineTypeChanged || shouldBreakCharacterLineByScript || (CurrentScript == CharacterFormattingScript.Subscript && runChanged);
		}
		protected internal override void StartCharacterLine(int startAnchorIndex) {			
			actualScript = CurrentScript;
			this.startAnchorIndex = startAnchorIndex;
		}
		protected internal override void EndCharacterLine(int endAnchorIndex, FontInfo currentFontInfo) {
			UnderlineInfo info = CalculateUnderlineInfo(endAnchorIndex);
			UnderlineBox underlineBox = new UnderlineBox(startAnchorIndex, endAnchorIndex - startAnchorIndex);
			underlineBox.UnderlinePosition = CalculateUnderlinePosition(info.WeightedUnderlinePositionSum, info.TotalUnderlineWidth);
			underlineBox.UnderlineThickness = CalculateUnderlineThickness(info.WeightedUnderlineThicknessSum, info.TotalUnderlineWidth);
			underlineBox.UnderlineBounds = new Rectangle(0, info.TopOffset, 0, 0);
			underlineBoxesByType.Add(underlineBox);
		}
		public UnderlineBox CreateTabLeaderUnderlineBox(TabSpaceBox box, Rectangle bounds, Row row) {
			SetRow(row);
			UnderlineInfo info = CalculateTabLeaderUnderlineInfo(box, bounds);
			UnderlineBox underlineBox = new UnderlineBox();
			underlineBox.UnderlinePosition = CalculateUnderlinePosition(info.WeightedUnderlinePositionSum, info.TotalUnderlineWidth);
			underlineBox.UnderlineThickness = CalculateUnderlineThickness(info.WeightedUnderlineThicknessSum, info.TotalUnderlineWidth);
			underlineBox.UnderlineBounds = new Rectangle(0, info.TopOffset, 0, 0);
			int baseLinePosition = Row.Bounds.Top + Row.BaseLineOffset + underlineBox.UnderlineBounds.Y;
			int underlineTop = baseLinePosition - underlineBox.UnderlineThickness;
			int underlineBottom = baseLinePosition + underlineBox.UnderlineThickness / 2 ;
			underlineBox.UnderlineBounds = Rectangle.FromLTRB(Row.Bounds.Left, underlineTop, Row.Bounds.Right, underlineBottom);
			underlineBox.ClipBounds = Rectangle.FromLTRB(bounds.Left, underlineTop, bounds.Right, underlineBottom);
			underlineBox.UnderlinePosition = 0;
			return underlineBox;
		}
		public void CenterTabLeaderUnderlineBoxVertically(UnderlineBox underlineBox, Rectangle bounds) {
			int offset = (underlineBox.UnderlineBounds.Top - bounds.Top) / 2 - underlineBox.UnderlineThickness;
			Rectangle r = underlineBox.UnderlineBounds;
			r.Y -= offset;
			underlineBox.UnderlineBounds = r;
			r = underlineBox.ClipBounds;
			r.Y -= offset;
			underlineBox.ClipBounds = r;
		}
		protected internal override void AddBoxToCharacterLine(Box box) {
			actualScript = CurrentScript;
		}
		protected internal virtual UnderlineInfo CalculateUnderlineInfo(int endAnchorIndex) {
			UnderlineInfo result = new UnderlineInfo();
			int maxTopOffset = int.MinValue;
			int minTopOffset = int.MaxValue;
			int underlinePositionForMaxTopOffset = 0;
			for (int i = startAnchorIndex; i < endAnchorIndex; i++) {
				if (actualScript == CharacterFormattingScript.Normal && GetBoxScript(i) == CharacterFormattingScript.Superscript)
					continue;
				FontInfo fontInfo = GetBoxFontInfo(i);
				int topOffset = CalculateScriptOffset(fontInfo, actualScript);
				minTopOffset = Math.Min(maxTopOffset, topOffset);
				if (topOffset > maxTopOffset) {
					maxTopOffset = topOffset;
					underlinePositionForMaxTopOffset = fontInfo.UnderlinePosition;
				}
				int boxWidth = Boxes[i].Bounds.Width;
				result.TotalUnderlineWidth += boxWidth;
				result.WeightedUnderlinePositionSum += fontInfo.UnderlinePosition * boxWidth;
				result.WeightedUnderlineThicknessSum += fontInfo.UnderlineThickness * boxWidth;
			}
			if (result.TopOffset == int.MinValue)
				result.TopOffset = 0;
			else {
				result.TopOffset = maxTopOffset;
				if (minTopOffset != maxTopOffset)
					result.WeightedUnderlinePositionSum = underlinePositionForMaxTopOffset * result.TotalUnderlineWidth;
			}
			return result;
		}
		protected internal virtual UnderlineInfo CalculateTabLeaderUnderlineInfo(TabSpaceBox box, Rectangle bounds) {
			UnderlineInfo result = new UnderlineInfo();
			FontInfo fontInfo = box.GetFontInfo(PieceTable);
			int boxWidth = bounds.Width;
			result.TotalUnderlineWidth += boxWidth;
			result.WeightedUnderlinePositionSum += fontInfo.UnderlinePosition * boxWidth;
			result.WeightedUnderlineThicknessSum += fontInfo.UnderlineThickness * boxWidth;
			return result;
		}
		protected internal override UnderlineType GetRunCharacterLineType(TextRun run, RunIndex runIndex) {
			return run.FontUnderlineType;
		}
		protected internal override bool GetRunCharacterLineUseForWordsOnly(TextRun run, RunIndex runIndex) {
			return run.UnderlineWordsOnly;
		}
		protected override UnderlineType GetNumberingListBoxCharacterLineType(NumberingListBox numberingListBox) {
			return numberingListBox.GetFontUnderlineType(PieceTable);
		}
		protected override bool GetNumberingListBoxCharacterLineUseForWordsOnly(NumberingListBox numberingListBox) {
			return numberingListBox.GetNumerationCharacterProperties(PieceTable).Info.UnderlineWordsOnly;
		}
		static internal int CalculateUnderlineThickness(int weightedUnderlineThicknessSum, int totalUnderlineWidth) {
			if (totalUnderlineWidth != 0)
				return Math.Max(weightedUnderlineThicknessSum / totalUnderlineWidth, 1);
			return 1;
		}
		static internal int CalculateUnderlinePosition(int weightedUnderlinePositionSum, int totalUnderlineWidth) {
			return totalUnderlineWidth != 0 ? weightedUnderlinePositionSum / totalUnderlineWidth : 0;
		}
		protected internal virtual void SplitUnderlinesByColor(UnderlineBoxCollection underlines) {
			underlines.ForEach(SplitUnderlineBoxByColor);
		}
		protected internal virtual void SplitUnderlineBoxByColor(UnderlineBox underlineBox) {
			int startAnchorIndex = underlineBox.StartAnchorIndex;
			int endAnchorIndex = underlineBox.EndAnchorIndex;
			int startIndex = startAnchorIndex;
			Box box = Boxes[startIndex];
			RunIndex currentRunIndex = box.StartPos.RunIndex;
			Color currentUnderlineColor = box.GetActualUnderlineColor(PieceTable, TextColors.Defaults, DXColor.Empty);
			for (int i = startAnchorIndex + 1; i < endAnchorIndex; i++) {
				box = Boxes[i];
				RunIndex boxRunIndex = box.StartPos.RunIndex;
				if (boxRunIndex == currentRunIndex)
					continue;
				currentRunIndex = boxRunIndex;
				Color boxUnderlineColor = box.GetActualUnderlineColor(PieceTable, TextColors.Defaults, DXColor.Empty);
				if (boxUnderlineColor != currentUnderlineColor) {
					AddNewUnderlineBoxToRow(underlineBox, startIndex, i);
					startIndex = i;
					currentUnderlineColor = boxUnderlineColor;
				}
			}
			AddNewUnderlineBoxToRow(underlineBox, startIndex, endAnchorIndex);
		}
		protected internal virtual void AddNewUnderlineBoxToRow(UnderlineBox sourceUnderlineBox, int startIndex, int endIndex) {
			UnderlineBox newUnderlineBox = new UnderlineBox(startIndex, endIndex - startIndex);
			newUnderlineBox.UnderlinePosition = sourceUnderlineBox.UnderlinePosition;
			newUnderlineBox.UnderlineThickness = sourceUnderlineBox.UnderlineThickness;
			newUnderlineBox.UnderlineBounds = sourceUnderlineBox.UnderlineBounds;
			Row.Underlines.Add(newUnderlineBox);
		}
		protected internal virtual void SetUnderlinesBounds() {
			UnderlineBoxCollection underlines = Row.InnerUnderlines;
			if (underlines != null)
				underlines.ForEach(SetUnderlineBoxBounds);
		}
		protected internal virtual void SetUnderlineBoxBounds(UnderlineBox underlineBox) {
			int underlineLeft = GetUnderlineLeft(underlineBox);
			int underlineRight = GetUnderlineRight(underlineBox);
			int baseLinePosition = Row.Bounds.Top + Row.BaseLineOffset + underlineBox.UnderlineBounds.Y;
			int underlineTop = baseLinePosition + underlineBox.UnderlinePosition / 2;
			int underlineBottom = baseLinePosition + underlineBox.UnderlinePosition + underlineBox.UnderlineThickness;
			int bottomDistance = Row.Bounds.Bottom - underlineBottom;
			if (bottomDistance < 0) {
				underlineTop += bottomDistance;
				underlineBottom += bottomDistance;
			}
			underlineBox.UnderlineBounds = Rectangle.FromLTRB(Row.Bounds.Left, underlineTop, Row.Bounds.Right, underlineBottom);
			underlineBox.ClipBounds = Rectangle.FromLTRB(underlineLeft, underlineTop, underlineRight, underlineBottom);
			underlineBox.UnderlinePosition -= underlineBox.UnderlinePosition / 2;
		}
		protected internal virtual int GetUnderlineLeft(UnderlineBox underlineBox) {
			return Boxes[underlineBox.StartAnchorIndex].Bounds.Left;
		}
		protected internal virtual int GetUnderlineRight(UnderlineBox underlineBox) {
			int result;
			int endAnchorIndex = underlineBox.EndAnchorIndex;
			int boxesCount = Boxes.Count;
			if (endAnchorIndex < boxesCount)
				result = Boxes[endAnchorIndex].Bounds.Left;
			else
				result = Boxes[boxesCount - 1].Bounds.Right;
			return result;
		}
	}
	#endregion
	#region StrikeoutCalculator
	public class StrikeoutCalculator : CharacterLineCalculator<StrikeoutType> {
		int startAnchorIndex;
		public StrikeoutCalculator(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected internal override StrikeoutType CharacterLineTypeNone { get { return StrikeoutType.None; } }
		protected internal override void BeforeCalculate(Row row) {
			base.BeforeCalculate(row);
			row.ClearStrikeouts();
		}		
		protected internal override bool IsCharacterLineBreak(bool characterLineTypeChanged, bool runChanged, CharacterFormattingScript boxScript) {
			return characterLineTypeChanged || runChanged;
		}
		protected internal override void StartCharacterLine(int startAnchorIndex) {
			this.startAnchorIndex = startAnchorIndex;
		}
		protected internal virtual int CalculateStrikeoutBoxTop(FontInfo fontInfo) {
			int top = Row.Bounds.Top + Row.BaseLineOffset - fontInfo.StrikeoutPosition;
			top += CalculateScriptOffset(fontInfo, GetBoxScript(startAnchorIndex));
			return top;
		}
		protected internal override void EndCharacterLine(int endAnchorIndex, FontInfo fontInfo) {
			UnderlineBox strikeoutBox = new UnderlineBox(startAnchorIndex, endAnchorIndex - startAnchorIndex);
			Row.Strikeouts.Add(strikeoutBox);
			int top = CalculateStrikeoutBoxTop(fontInfo);
			Rectangle strikeoutBounds = Row.Bounds;
			strikeoutBounds.Y = top;
			strikeoutBounds.Height = 0;
			int right = endAnchorIndex < BoxCount ? Boxes[endAnchorIndex].Bounds.Left : Boxes[BoxCount - 1].Bounds.Right;
			Rectangle clipBounds = Rectangle.FromLTRB(Boxes[startAnchorIndex].Bounds.Left, top, right, top);
			strikeoutBox.UnderlineBounds = strikeoutBounds;
			strikeoutBox.ClipBounds = clipBounds;
			strikeoutBox.UnderlinePosition = fontInfo.StrikeoutPosition;
			strikeoutBox.UnderlineThickness = fontInfo.StrikeoutThickness;
		}
		protected internal override StrikeoutType GetRunCharacterLineType(TextRun run, RunIndex runIndex) {
			return run.FontStrikeoutType;
		}
		protected internal override bool GetRunCharacterLineUseForWordsOnly(TextRun run, RunIndex runIndex) {
			return run.StrikeoutWordsOnly;
		}
		protected override StrikeoutType GetNumberingListBoxCharacterLineType(NumberingListBox numberingListBox) {
			return numberingListBox.GetNumerationCharacterProperties(PieceTable).Info.FontStrikeoutType;
		}
		protected override bool GetNumberingListBoxCharacterLineUseForWordsOnly(NumberingListBox numberingListBox) {
			return numberingListBox.GetNumerationCharacterProperties(PieceTable).Info.StrikeoutWordsOnly;
		}
	}
	#endregion
}
