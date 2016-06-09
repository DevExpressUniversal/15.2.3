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
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
using System.Linq;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Editors.Internal {
	public class TokenEditorLineInfo {
		public TokenEditorLineInfo(List<TokenInfo> tokens, int index) {
			Tokens = tokens;
			Height = CalcHeight();
			Index = index;
		}
		public TokenEditorLineInfo(List<TokenInfo> tokens) {
			Tokens = tokens;
			Height = CalcHeight();
		}
		public double Height { get; private set; }
		public List<TokenInfo> Tokens { get; private set; }
		public int Index { get; set; }
		private double CalcHeight() {
			double maxHeight = 0;
			Tokens.ForEach(x => maxHeight = Math.Max(maxHeight, x.Size.Height));
			return maxHeight;
		}
	}
	public class TokenInfo {
		public TokenInfo(int index, Size size) {
			VisibleIndex = index;
			Size = size;
		}
		public int VisibleIndex { get; private set; }
		public Size Size { get; private set; }
	}
	public abstract class TokenEditorMeasureStrategyBase {
		const double TokenMagicHeight = 20;
		public TokenEditorMeasureStrategyBase(TokenEditorPanel panel) {
			OwnerPanel = panel;
			Lines = new List<TokenEditorLineInfo>();
		}
		public abstract Orientation Orientation { get; }
		public bool ShouldMeasureFromLastToken { get; protected set; }
		public int MaxVisibleIndex { get { return TokensCount; } }
		public int MinVisibleIndex { get { return 0; } }
		public int LinesCount { get { return Lines.Count; } }
		public int OffsetTokenIndex { get; protected set; }
		protected TokenEditorPanel OwnerPanel { get; private set; }
		protected bool ShowDefaultToken { get { return OwnerPanel.CanShowDefaultToken; } }
		protected int TokensCount { get { return OwnerPanel.Items.Count; } }
		protected TokenEditorPresenter DefaultToken { get { return OwnerPanel.DefaultTokenPresenter; } }
		protected abstract int DefaultTokenVisibleIndex { get; }
		protected List<TokenEditorLineInfo> Lines { get; set; }
		protected bool IsLinesValid { get; set; }
		protected bool ShouldDestroyLines { get; set; }
		protected int OffsetLineIndex { get; set; }
		protected bool CanArrangeDefaultToken { get { return ShowDefaultToken || DefaultToken.HasNullText; } }
		double TokenMaxWidth { get { return OwnerPanel.GetTokenMaxWidth(); } }
		public abstract Size Measure(Size constraint);
		public abstract List<UIElement> Arrange(Size finalSize);
		public abstract int ConvertToEditableIndex(int visualIndex);
		public abstract int ConvertToVisibleIndex(int editableIndex);
		protected abstract bool CanArrange(int index, double x);
		public virtual int MeasureFromEnd(Size constraint, out double offset) {
			offset = 0;
			return 0;
		}
		public void RenumerateLines() {
			int offsetIndex = OwnerPanel.OffsetToIndex(OwnerPanel.VerticalOffset);
			var line = GetLineByAbsolutIndex(OffsetLineIndex);
			if (line != null) {
				NumerateLinesCore(Lines.IndexOf(line), offsetIndex);
				OffsetLineIndex = offsetIndex;
			}
		}
		public TokenEditorLineInfo GetLine(int lineIndex) {
			if (Lines.Count > lineIndex)
				return Lines[lineIndex];
			return null;
		}
		public bool IsMaxVisibleIndex(int index) {
			return MaxVisibleIndex == index;
		}
		public bool IsMinVisibleIndex(int index) {
			return index == MinVisibleIndex;
		}
		public double GetBottomTokenIndex() {
			return GetFirstTokenIndexInLine(Lines.Count);
		}
		public double GetTopTokenIndex() {
			return GetFirstTokenIndexInLine(0);
		}
		public double CalcMaxLineHeight() {
			double height = 0;
			Lines.ForEach(x => height = Math.Max(height, x.Height));
			return Lines.Count > 0 ? height : TokenMagicHeight;
		}
		public bool IsDefaultToken(int index) {
			return index == DefaultTokenVisibleIndex;
		}
		public double CalcLinesHeight() {
			double height = 0;
			Lines.ForEach(x => height += x.Height);
			return height;
		}
		internal double CalcLinesHeightFromOffset(double offset) {
			double height = 0;
			var line = GetLineByAbsolutIndex(OwnerPanel.OffsetToIndex(offset));
			if (line != null) {
				int index = Lines.IndexOf(line);
				for (int i = index; i < LinesCount; i++)
					height += Lines[i].Height;
			}
			return height;
		}
		public TokenEditorLineInfo GetLineByAbsolutIndex(int lineIndex) {
			return Lines.Where(x => x.Index == lineIndex).FirstOrDefault();
		}
		public TokenEditorLineInfo GetContainedLine(int tokenIndex) {
			foreach (var line in Lines) {
				foreach (var token in line.Tokens)
					if (token.VisibleIndex == tokenIndex) return line;
			}
			return null;
		}
		public void InvalidateLines() {
			IsLinesValid = false;
		}
		public void DestroyLines() {
			ShouldDestroyLines = true;
		}
		public int GetIndexOfLine(TokenEditorLineInfo line) {
			return Lines.IndexOf(line);
		}
		protected double CalcResultSize(double maxSize, double calcSize) {
			return Math.Max(0, Math.Min(maxSize, calcSize));
		}
		protected double CalcTopPoint(double whole, double height) {
			var delta = whole - height;
			return delta > 0 ? delta / 2 : 0;
		}
		protected bool ShouldMeasureDefaultToken(int index) {
			return IsDefaultToken(index);
		}
		protected void NumerateLines(TokenEditorLineInfo offsetLine) {
			int offsetIndex = Lines.IndexOf(offsetLine);
			NumerateLinesCore(offsetIndex, OffsetLineIndex);
		}
		protected double GetTokenMaxWidth(double width) {
			return TokenMaxWidth > 0 && width > TokenMaxWidth ? TokenMaxWidth : width;
		}
		protected void NumerateLinesCore(int realtiveLineIndex, int offsetLineIndex) {
			for (int i = 0; i < LinesCount; i++) {
				var delta = i - realtiveLineIndex;
				Lines[i].Index = offsetLineIndex + delta;
			}
		}
		private int GetFirstTokenIndexInLine(int index) {
			if (Lines.Count > 0) {
				var line = Lines[index].Return(x => x.Tokens.Count > 0 ? x.Tokens[0] : null, () => null);
				return line != null ? line.VisibleIndex : 0;
			}
			return 0;
		}
	}
	public class TokenEditorLineMeasureStrategy : TokenEditorMeasureStrategyBase {
		public TokenEditorLineMeasureStrategy(TokenEditorPanel panel) : base(panel) { }
		protected override int DefaultTokenVisibleIndex { get { return 0; } }
		public override int MeasureFromEnd(Size constraint, out double offset) {
			int index = MaxVisibleIndex;
			double wholeWidth = 0;
			double wholeHeight = 0;
			while (index > -1 && wholeWidth < constraint.Width) {
				if (ShouldMeasureDefaultToken(index))
					MeasureDefaultToken(constraint, ref wholeWidth, ref wholeHeight);
				else
					MeasureToken(constraint, ref wholeWidth, ref wholeHeight, index);
				index--;
			}
			offset = Math.Abs(wholeWidth - constraint.Width);
			return (index + 1);
		}
		public override List<UIElement> Arrange(Size finalSize) {
			List<UIElement> arranged = new List<UIElement>();
			int index = 0;
			double x = 0;
			OwnerPanel.CalcRelativeOffsetAndIndex(ref x, ref index);
			while (CanArrange(index, x)) {
				UIElement container = null;
				double arrangeWidth = 0;
				container = OwnerPanel.GetContainer(ConvertToEditableIndex(index));
				if (container == null) break;
				if (IsDefaultToken(index)) {
					if (!CanArrangeDefaultToken)
						arrangeWidth = 0;
					else {
						arrangeWidth = TokensCount > 0 ? container.DesiredSize.Width : finalSize.Width;
					}
				}
				else
					arrangeWidth = GetTokenMaxWidth(container.DesiredSize.Width);
				var bounds = new Rect(x, CalcTopPoint(finalSize.Height, container.DesiredSize.Height), arrangeWidth, container.DesiredSize.Height);
				container.Arrange(bounds);
				arranged.Add(container);
				x += arrangeWidth;
				index++;
			}
			return arranged;
		}
		public override Size Measure(Size constraint) {
			double wholeWidth = 0;
			double wholeHeight = 0;
			int additionalTokens = 1;
			int index = Math.Max(0, OwnerPanel.StartMeasureIndex - additionalTokens);
			double startPosition = 0;
			while (index <= TokensCount && additionalTokens > 0) {
				Size size = new Size();
				if (ShouldMeasureDefaultToken(index)) {
					MeasureDefaultToken(constraint, ref wholeWidth, ref wholeHeight);
					size = DefaultToken.DesiredSize;
				}
				else
					size = MeasureToken(constraint, ref wholeWidth, ref wholeHeight, index);
				if (index == OwnerPanel.StartMeasureIndex)
					startPosition -= size.Width;
				if (index > OwnerPanel.StartMeasureIndex)
					startPosition += size.Width;
				if (startPosition > constraint.Width)
					additionalTokens--;
				index++;
			}
			return new Size(CalcResultSize(constraint.Width, wholeWidth), CalcResultSize(constraint.Height, wholeHeight)); ;
		}
		public override int ConvertToEditableIndex(int visualIndex) {
			return IsDefaultToken(visualIndex) ? TokensCount : visualIndex - 1;
		}
		public override int ConvertToVisibleIndex(int editableIndex) {
			return editableIndex == TokensCount ? DefaultTokenVisibleIndex : editableIndex + 1;
		}
		public override Orientation Orientation { get { return System.Windows.Controls.Orientation.Horizontal; } }
		protected override bool CanArrange(int index, double x) {
			return index <= MaxVisibleIndex;
		}
		private bool IsHorizontalOffsetMaximum() {
			return OwnerPanel.ExtentWidth > OwnerPanel.ViewportWidth && OwnerPanel.HorizontalOffset + OwnerPanel.ViewportWidth == OwnerPanel.ExtentWidth;
		}
		private void MeasureDefaultToken(Size constraint, ref double wholeWidth, ref double wholeHeight) {
			DefaultToken.Measure(constraint);
			OwnerPanel.AddDefaultTokenContainerToMeasured();
			if (ShowDefaultToken) {
				wholeWidth += DefaultToken.DesiredSize.Width;
			}
			wholeHeight = Math.Max(wholeHeight, DefaultToken.DesiredSize.Height);
		}
		private Size MeasureToken(Size constraint, ref double wholeWidth, ref double wholeHeight, int index) {
			var container = OwnerPanel.PrepareContainer(ConvertToEditableIndex(index));
			if (container == null) return new Size();
			constraint = new Size(GetTokenMaxWidth(constraint.Width), constraint.Height);
			container.Measure(constraint);
			wholeHeight = Math.Max(wholeHeight, container.DesiredSize.Height);
			var size = new Size(GetTokenMaxWidth(container.DesiredSize.Width), container.DesiredSize.Height);
			wholeWidth += container.DesiredSize.Width;
			return container.DesiredSize;
		}
	}
	public class TokenEditorLineFromEndMeasureStrategy : TokenEditorLineMeasureStrategy {
		public TokenEditorLineFromEndMeasureStrategy(TokenEditorPanel panel) : base(panel) { }
		protected override int DefaultTokenVisibleIndex { get { return TokensCount; } }
		public override int ConvertToEditableIndex(int visualIndex) {
			return IsDefaultToken(visualIndex) ? TokensCount : visualIndex;
		}
		public override int ConvertToVisibleIndex(int editableIndex) {
			return editableIndex == TokensCount ? DefaultTokenVisibleIndex : editableIndex;
		}
	}
	public class TokenEditorWrapLineMeasureStrategy : TokenEditorMeasureStrategyBase {
		const int AdditionalLines = 2;
		public TokenEditorWrapLineMeasureStrategy(TokenEditorPanel panel) : base(panel) { }
		public override Orientation Orientation { get { return System.Windows.Controls.Orientation.Vertical; } }
		protected override int DefaultTokenVisibleIndex { get { return 0; } }
		public override int MeasureFromEnd(Size constraint, out double offset) {
			offset = 0;
			Size size = new Size();
			int index = MaxVisibleIndex;
			List<TokenInfo> tokens = new List<TokenInfo>();
			double width = 0;
			double height = 0;
			List<TokenEditorLineInfo> newLines = new List<TokenEditorLineInfo>();
			while (index > -1 && height < constraint.Height) {
				double lineHeight = 0;
				size = MeasureToken(constraint, index, ref lineHeight);
				width += size.Width;
				if (width > constraint.Width) {
					width = size.Width;
					height += lineHeight;
					newLines.Insert(0, new TokenEditorLineInfo(tokens));
					tokens = new List<TokenInfo>();
				}
				tokens.Insert(0, new TokenInfo(index, size));
				index--;
			}
			if (newLines.Count > 0) {
				var line = GetContainedLine(newLines[0].Tokens[0].VisibleIndex);
				if (line != null) {
					Lines = newLines;
					NumerateLinesCore(0, line.Index);
				}
				else {
					Lines = newLines;
					NumerateLinesCore(0, newLines[0].Tokens[0].VisibleIndex / 3);
				}
				OffsetLineIndex = Lines[0].Index;
			}
			offset = Math.Abs(height - constraint.Height);
			var verticalOffset = OwnerPanel.OffsetToIndex(OwnerPanel.VerticalOffset);
			offset = Math.Abs(OwnerPanel.VerticalOffset - verticalOffset);
			return verticalOffset;
		}
		Size prevConstraint = new Size();
		public override Size Measure(Size constraint) {
			if (!IsSizeEmpty(prevConstraint) && prevConstraint != constraint)
				ShouldDestroyLines = true;
			prevConstraint = constraint;
			OffsetLineIndex = OwnerPanel.OffsetToIndex(OwnerPanel.VerticalOffset);
			double wholeWidth = 0;
			double wholeHeight = 0;
			var newOffsetLine = GetLineByAbsolutIndex(OffsetLineIndex);
			bool isLinesMeasured = false;
			if (!ShouldDestroyLines)
				isLinesMeasured = MeasureOldLines(constraint, OffsetLineIndex, newOffsetLine);
			else {
				ShouldDestroyLines = false;
				if (newOffsetLine == null)
					OffsetTokenIndex = CalcFirstVisibleTokenIndexByMagic();
				else
					OffsetTokenIndex = CalcFirstVisibleTokenIndex();
				Lines = new List<TokenEditorLineInfo>();
			}
			if (Lines.Count == 0)
				CreateLines(constraint, newOffsetLine);
			else if (!isLinesMeasured) {
				for (int i = 0; i < LinesCount; i++) {
					MeasureLine(constraint, Lines[i]);
					wholeHeight += Lines[i].Height;
				}
				NumerateLines(newOffsetLine);
			}
			wholeHeight = CalcLinesHeight();
			wholeWidth = CalcMaxLineWidth();
			return new Size(CalcResultSize(wholeWidth, constraint.Width), CalcResultSize(wholeHeight, constraint.Height));
		}
		private bool IsSizeEmpty(Size size) {
			return size.Height == 0 && size.Width == 0;
		}
		private void CreateLines(Size constraint, TokenEditorLineInfo newOffsetLine) {
			int index = OffsetTokenIndex - 1;
			List<TokenInfo> tokens = new List<TokenInfo>();
			int additionalLines = AdditionalLines;
			double lineHeight = 0;
			Size size = new Size();
			double wholeHeight = 0;
			double width = 0;
			if (index > -1 && index <= TokensCount) {
				while (index > -1) {
					size = MeasureToken(constraint, index, ref lineHeight);
					width += size.Width;
					if (width > constraint.Width) {
						width = size.Width;
						if (tokens.Count == 0) {
							tokens.Add(new TokenInfo(index, size));
							Lines.Insert(0, new TokenEditorLineInfo(tokens));
							tokens = new List<TokenInfo>();
							width = 0;
						}
						else {
							Lines.Insert(0, new TokenEditorLineInfo(tokens));
							tokens = new List<TokenInfo>() { new TokenInfo(index, size) };
						}
						additionalLines--;
						if (additionalLines == 0) break;
					}
					else
						tokens.Insert(0, new TokenInfo(index, size));
					index--;
				}
				if (index == -1 && additionalLines != 0)
					Lines.Insert(0, new TokenEditorLineInfo(tokens));
			}
			double startPosition = 0;
			additionalLines = AdditionalLines;
			tokens = new List<TokenInfo>();
			index = OffsetTokenIndex;
			while (index <= TokensCount) {
				size = MeasureToken(constraint, index, ref lineHeight);
				startPosition += size.Width;
				if (startPosition > constraint.Width) {
					startPosition = size.Width;
					wholeHeight += lineHeight;
					if (tokens.Count == 0) {
						tokens.Add(new TokenInfo(index, size));
						Lines.Add(new TokenEditorLineInfo(tokens));
						tokens = new List<TokenInfo>();
						width = 0;
					}
					else {
						Lines.Add(new TokenEditorLineInfo(tokens));
						tokens = new List<TokenInfo>() { new TokenInfo(index, size) };
					}
					if (wholeHeight > constraint.Height) {
						additionalLines--;
					}
					if (additionalLines == 0) break;
				}
				else
					tokens.Add(new TokenInfo(index, size));
				index++;
			}
			if (additionalLines != 0) {
				Lines.Add(new TokenEditorLineInfo(tokens));
			}
			newOffsetLine = GetContainedLine(OffsetTokenIndex);
			NumerateLines(newOffsetLine);
		}
		int prevMagic = 0;
		int CalcFirstVisibleTokenIndexByMagic() {
			int previousLineIndex = Math.Max(0, OwnerPanel.OffsetToIndex(OwnerPanel.PreviousVerticalOffset));
			int tokenIndex = GetLineByAbsolutIndex(previousLineIndex).Return(x => x.Tokens[0].VisibleIndex, () => -1);
			if (tokenIndex == -1) return 0;
			if (OwnerPanel.PreviousVerticalOffset <= OwnerPanel.VerticalOffset) {
				double averageTokenOnPixel = (TokensCount - tokenIndex) / Math.Abs(OwnerPanel.PreviousVerticalOffset - OwnerPanel.ExtentHeight);
				int startIndex = prevMagic == 0 ? tokenIndex : prevMagic;
				var newIndex = (int)(startIndex + averageTokenOnPixel * Math.Abs(OwnerPanel.PreviousVerticalOffset - OwnerPanel.VerticalOffset));
				prevMagic = newIndex < TokensCount ? newIndex : tokenIndex;
				return prevMagic;
			}
			else {
				double averageTokenOnPixel = tokenIndex / OwnerPanel.PreviousVerticalOffset;
				int startIndex = prevMagic == 0 ? tokenIndex : prevMagic;
				var newIndex = (int)(startIndex - averageTokenOnPixel * Math.Abs(OwnerPanel.PreviousVerticalOffset - OwnerPanel.VerticalOffset));
				prevMagic = Math.Max(0, newIndex);
				return prevMagic;
			}
		}
		int CalcFirstVisibleTokenIndex() {
			int lineIndex = Math.Max(0, OwnerPanel.OffsetToIndex(OwnerPanel.VerticalOffset));
			if (lineIndex == 0) return MinVisibleIndex;
			var tokenInfo = GetLineByAbsolutIndex(lineIndex).Return(x => x.Tokens.Count > 0 ? x.Tokens[0] : null, () => null);
			return tokenInfo != null ? tokenInfo.VisibleIndex : 0;
		}
		private double CalcMaxLineWidth() {
			double width = 0;
			foreach (var line in Lines) {
				double lineWidth = 0;
				line.Tokens.ForEach(x => lineWidth += x.Size.Width);
				width = Math.Max(width, lineWidth);
			}
			return width;
		}
		double CalcWidht(List<TokenInfo> list) {
			double width = 0;
			list.ForEach(x => width += x.Size.Width);
			return width;
		}
		private bool MeasureOldLines(Size constraint, int newOffsetLineIndex, TokenEditorLineInfo newOffsetLine) {
			if (IsLinesValid)
				return false;
			IsLinesValid = true;
			bool isLinesMeasured = false;
			int prevOffsetLineIndex = OwnerPanel.OffsetToIndex(OwnerPanel.PreviousVerticalOffset);
			if (newOffsetLine == null) {
				if (LinesCount != 0)
					OffsetTokenIndex = CalcFirstVisibleTokenIndexByMagic();
				Lines = new List<TokenEditorLineInfo>();
			}
			else if (Math.Abs(OwnerPanel.PreviousVerticalOffset - OwnerPanel.VerticalOffset) > 0) {
				isLinesMeasured = true;
				prevMagic = 0;
				var newLines = new List<TokenEditorLineInfo>();
				if (OwnerPanel.VerticalOffset >= OwnerPanel.PreviousVerticalOffset)
					newLines = MeasureLinesOnScrollDown(constraint, newOffsetLine);
				else
					newLines = MeasureLinesOnScrollUp(constraint, newOffsetLine);
				Lines = newLines;
				NumerateLines(newOffsetLine);
			}
			if (newOffsetLineIndex == 0) OffsetTokenIndex = MinVisibleIndex;
			return isLinesMeasured;
		}
		private List<TokenEditorLineInfo> MeasureLinesOnScrollUp(Size constraint, TokenEditorLineInfo newOffsetLine) {
			var newLines = new List<TokenEditorLineInfo>();
			int prevOffsetLineIndex = OwnerPanel.OffsetToIndex(OwnerPanel.PreviousVerticalOffset);
			var prevOffsetLine = GetLineByAbsolutIndex(prevOffsetLineIndex);
			OffsetTokenIndex = CalcFirstVisibleTokenIndex();
			int shiftCount = Math.Abs(Lines.IndexOf(newOffsetLine) - Lines.IndexOf(prevOffsetLine));
			double wholeHeight = CalcMaxLineHeight() * Lines.Count;
			double maxSize = AdditionalLines * CalcMaxLineHeight() + constraint.Height;
			if (GetContainedLine(MinVisibleIndex) == null && wholeHeight >= maxSize)
				newLines = ShiftLines(-shiftCount);
			else
				newLines = Lines;
			for (int i = 0; i < newLines.Count; i++) {
				MeasureLine(constraint, newLines[i]);
			}
			int additionalLines = newLines.IndexOf(newOffsetLine) > 0 ? shiftCount : AdditionalLines;
			int index = Math.Max(MinVisibleIndex, Lines[0].Tokens[0].VisibleIndex - 1);
			if (GetContainedLine(index) == null && additionalLines > 0) {
				Size size = new Size();
				List<TokenInfo> tokens = new List<TokenInfo>();
				double width = 0;
				while (index > -1) {
					double lineHeight = 0;
					size = MeasureToken(constraint, index, ref lineHeight);
					width += size.Width;
					if (width > constraint.Width) {
						width = size.Width;
						if (tokens.Count == 0) {
							tokens.Add(new TokenInfo(index, size));
							newLines.Insert(0, new TokenEditorLineInfo(tokens));
							tokens = new List<TokenInfo>();
							width = 0;
						}
						else {
							newLines.Insert(0, new TokenEditorLineInfo(tokens));
							tokens = new List<TokenInfo>() { new TokenInfo(index, size) };
						}
						additionalLines--;
						if (additionalLines <= 0) break;
					}
					else
						tokens.Insert(0, new TokenInfo(index, size));
					index--;
				}
				if (index == -1 && additionalLines != 0)
					newLines.Insert(0, new TokenEditorLineInfo(tokens));
			}
			return newLines;
		}
		private List<TokenEditorLineInfo> MeasureLinesOnScrollDown(Size constraint, TokenEditorLineInfo newOffsetLine) {
			List<TokenEditorLineInfo> newLines = new List<TokenEditorLineInfo>();
			int prevOffsetLineIndex = OwnerPanel.OffsetToIndex(OwnerPanel.PreviousVerticalOffset);
			var prevOffsetLine = GetLineByAbsolutIndex(prevOffsetLineIndex);
			double startPosition = 0;
			OffsetTokenIndex = CalcFirstVisibleTokenIndex();
			int shiftCount = (Lines.IndexOf(newOffsetLine) + 1) - AdditionalLines;
			int additionalLines = shiftCount;
			double lineHeight = CalcMaxLineHeight();
			double wholeHeight = CalcMaxLineHeight() * Lines.Count;
			double maxSize = AdditionalLines * CalcMaxLineHeight() + constraint.Height;
			if (wholeHeight >= maxSize)
				newLines = ShiftLines(shiftCount);
			else {
				newLines = Lines;
				additionalLines = AdditionalLines;
			}
			for (int i = 0; i < newLines.Count; i++) {
				MeasureLine(constraint, newLines[i]);
			}
			wholeHeight = lineHeight * Math.Abs(newLines.IndexOf(newOffsetLine) - newLines.Count);
			int index = newLines[newLines.Count - 1].Tokens.Last<TokenInfo>().VisibleIndex + 1;
			if (index != -1 && additionalLines > 0) {
				if (index <= MaxVisibleIndex) {
					Size size = new Size();
					List<TokenInfo> tokens = new List<TokenInfo>();
					while (index <= MaxVisibleIndex) {
						size = MeasureToken(constraint, index, ref lineHeight);
						startPosition += size.Width;
						if (startPosition > constraint.Width) {
							startPosition = size.Width;
							if (tokens.Count == 0) {
								tokens.Add(new TokenInfo(index, size));
								newLines.Add(new TokenEditorLineInfo(tokens));
								tokens = new List<TokenInfo>();
							}
							else {
								newLines.Add(new TokenEditorLineInfo(tokens));
								tokens = new List<TokenInfo>() { new TokenInfo(index, size) };
							}
							additionalLines--;
							if (additionalLines <= 0) break;
						}
						else
							tokens.Add(new TokenInfo(index, size));
						index++;
					}
					if (additionalLines != 0 && index <= MaxVisibleIndex) {
						wholeHeight += size.Height;
						newLines.Add(new TokenEditorLineInfo(tokens));
					}
				}
			}
			return newLines;
		}
		private TokenEditorLineInfo GetLastVisibleLine() {
			double lines = OwnerPanel.ViewportHeight / CalcMaxLineHeight();
			return GetLineByAbsolutIndex((int)Math.Ceiling(OwnerPanel.OffsetToIndex(OwnerPanel.VerticalOffset) + lines));
		}
		private void MeasureLine(Size constraint, TokenEditorLineInfo line) {
			foreach (var token in line.Tokens) {
				double lineHeight = 0;
				MeasureToken(constraint, token.VisibleIndex, ref lineHeight);
			}
		}
		private Size MeasureToken(Size constraint, int index, ref double lineHeight) {
			if (ShouldMeasureDefaultToken(index)) {
				MeasureDefaultToken(constraint, ref lineHeight);
				return new Size(GetTokenMaxWidth(DefaultToken.DesiredSize.Width), DefaultToken.DesiredSize.Height);
			}
			else {
				constraint = new Size(GetTokenMaxWidth(constraint.Width), constraint.Height);
				return MeasureTokenCore(constraint, ref lineHeight, index);
			}
		}
		private List<TokenEditorLineInfo> ShiftLines(int shiftCount) {
			var result = new List<TokenEditorLineInfo>();
			for (int i = 0; i < Lines.Count; i++) {
				int index = i - shiftCount;
				if (index > -1 && index < LinesCount)
					result.Add(Lines[i]);
			}
			return result;
		}
		public override List<UIElement> Arrange(Size finalSize) {
			var startLineIndex = Math.Max(0, OffsetLineIndex);
			double relativeOffset = OwnerPanel.IndexToOffset(startLineIndex) - OwnerPanel.VerticalOffset;
			List<UIElement> arranged = new List<UIElement>();
			double y = relativeOffset;
			double lineHeght = 0;
			var startLine = Lines.Where(x => x.Index == startLineIndex).FirstOrDefault();
			if (startLine == null) return arranged;
			int relativeLineIndex = Lines.IndexOf(startLine);
			for (int i = relativeLineIndex; i < Lines.Count; i++) {
				var line = Lines[i];
				double x = 0;
				foreach (var tokenInfo in line.Tokens) {
					int index = tokenInfo.VisibleIndex;
					UIElement container = null;
					double arrangeWidth = 0;
					container = OwnerPanel.GetContainer(ConvertToEditableIndex(index));
					if (container == null)
						break;
					if (IsDefaultToken(index)) {
						if (ShowDefaultToken) {
							arrangeWidth = TokensCount > 0 ? container.DesiredSize.Width : finalSize.Width;
							if (OwnerPanel.ShowNewTokenFromEnd)
								arrangeWidth = Math.Max(arrangeWidth, finalSize.Width - x);
						}
					}
					else
						arrangeWidth = GetTokenMaxWidth(container.DesiredSize.Width);
					lineHeght = Math.Max(lineHeght, container.DesiredSize.Height);
					var bounds = new Rect(x, y, arrangeWidth, lineHeght);
					container.Arrange(bounds);
					arranged.Add(container);
					x += bounds.Width;
				}
				y += lineHeght;
			}
			return arranged;
		}
		public override int ConvertToEditableIndex(int visualIndex) {
			return IsDefaultToken(visualIndex) ? TokensCount : visualIndex - 1;
		}
		public override int ConvertToVisibleIndex(int editableIndex) {
			return editableIndex == TokensCount ? DefaultTokenVisibleIndex : editableIndex + 1;
		}
		protected override bool CanArrange(int index, double x) {
			return index <= MaxVisibleIndex;
		}
		private Size MeasureDefaultToken(Size constraint, ref double wholeHeight) {
			DefaultToken.Measure(constraint);
			OwnerPanel.AddDefaultTokenContainerToMeasured();
			wholeHeight = Math.Max(wholeHeight, DefaultToken.DesiredSize.Height);
			return DefaultToken.DesiredSize;
		}
		private Size MeasureTokenCore(Size constraint, ref double wholeHeight, int index) {
			var container = OwnerPanel.PrepareContainer(ConvertToEditableIndex(index));
			if (container == null) return new Size();
			container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			wholeHeight = Math.Max(wholeHeight, container.DesiredSize.Height);
			return new Size(GetTokenMaxWidth(container.DesiredSize.Width), container.DesiredSize.Height);
		}
	}
	public class TokenEditorWrapLineFromEndMeasureStrategy : TokenEditorWrapLineMeasureStrategy {
		public TokenEditorWrapLineFromEndMeasureStrategy(TokenEditorPanel panel) : base(panel) { }
		protected override int DefaultTokenVisibleIndex { get { return TokensCount; } }
		public override int ConvertToEditableIndex(int visualIndex) {
			return IsDefaultToken(visualIndex) ? TokensCount : visualIndex;
		}
		public override int ConvertToVisibleIndex(int editableIndex) {
			return editableIndex == TokensCount ? DefaultTokenVisibleIndex : editableIndex;
		}
	}
}
