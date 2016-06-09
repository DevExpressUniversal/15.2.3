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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region RowSpacingParametersCalculatorBase (abstract class)
	public abstract class RowSpacingParametersCalculatorBase {
		#region Fields
		ParagraphFinalFormatter formatter;
		int[] parameterValues;
		int maxParameterValue;
		int maxSpaceParameterValue;
		int maxMarkParameterValue;
		#endregion
		protected RowSpacingParametersCalculatorBase(ParagraphFinalFormatter formatter, int parameterCount) {
			this.formatter = formatter;
			this.parameterValues = new int[parameterCount];
		}
		#region Properties
		public int[] ParameterValues { get { return parameterValues; } }
		public int MaxParameterValue { get { return maxParameterValue; } set { maxParameterValue = value; } }
		public int MaxSpaceParameterValue { get { return maxSpaceParameterValue; } set { maxSpaceParameterValue = value; } }
		public int MaxMarkParameterValue { get { return maxMarkParameterValue; } set { maxMarkParameterValue = value; } }
		public ParagraphFinalFormatter Formatter { get { return formatter; } }
		#endregion
		protected bool IsSpaceBox(Box box) {
			return !box.IsNotWhiteSpaceBox;
		}
		protected bool IsMarkBox(Box box) {
			return box.IsLineBreak;
		}
		protected internal virtual void ProcessParameterValue(Box box, int value) {
			if (IsMarkBox(box))
				MaxMarkParameterValue = Math.Max(MaxMarkParameterValue, value);
			else if (IsSpaceBox(box))
				MaxSpaceParameterValue = Math.Max(MaxSpaceParameterValue, value);
			else
				MaxParameterValue = Math.Max(MaxParameterValue, value);
		}
		protected internal virtual int CalculateActualMaxParameterValue() {
			if (MaxParameterValue > 0)
				return MaxParameterValue;
			if (MaxMarkParameterValue > 0)
				return MaxMarkParameterValue;
			return MaxSpaceParameterValue;
		}
		public abstract void ProcessBox(Box box, int boxIndex);
		public abstract int CalculateRowBaseLineOffset(Row row);
	}
	#endregion
	#region RowSpacingParametersCalculator
	public class RowSpacingParametersCalculator : RowSpacingParametersCalculatorBase {
		public RowSpacingParametersCalculator(ParagraphFinalFormatter formatter, int parameterCount)
			: base(formatter, parameterCount) {
		}
		public override void ProcessBox(Box box, int boxIndex) {
			int ascent = Formatter.CalcBaseBoxAscentAndFree(box);
			ProcessParameterValue(box, ascent);
			ParameterValues[boxIndex] = Formatter.CalcBoxAscentAndFree(box);
		}
		public override int CalculateRowBaseLineOffset(Row row) {
			return CalculateActualMaxParameterValue();
		}
	}
	#endregion
	#region ExactlyRowSpacingParametersCalculator
	public class ExactlyRowSpacingParametersCalculator : RowSpacingParametersCalculatorBase {
		int baseLineOffset;
		public ExactlyRowSpacingParametersCalculator(ParagraphFinalFormatter formatter, int parameterCount, int baseLineOffset)
			: base(formatter, parameterCount) {
			this.baseLineOffset = baseLineOffset;
		}
		public override void ProcessBox(Box box, int boxIndex) {
			ParameterValues[boxIndex] = Formatter.CalcBoxAscentAndFree(box);
		}
		public override int CalculateRowBaseLineOffset(Row row) {
			return baseLineOffset;
		}
	}
	#endregion
	#region AtLeastRowSpacingParametersCalculator
	public class AtLeastRowSpacingParametersCalculator : RowSpacingParametersCalculatorBase {
		int maxPictureHeight;
		public AtLeastRowSpacingParametersCalculator(ParagraphFinalFormatter formatter, int parameterCount)
			: base(formatter, parameterCount) {
		}
		public override void ProcessBox(Box box, int boxIndex) {
			if (box.IsInlinePicture)
				this.maxPictureHeight = Math.Max(this.maxPictureHeight, box.Bounds.Height);
			int descent = Formatter.CalcBaseBoxDescent(box);
			ProcessParameterValue(box, descent);
			ParameterValues[boxIndex] = Formatter.CalcBoxAscentAndFree(box);
		}
		public override int CalculateRowBaseLineOffset(Row row) {
			return Math.Max(this.maxPictureHeight, CalculateRowBaseLineOffsetCore(row));
		}
		int CalculateRowBaseLineOffsetCore(Row row) {
			if (row.LastParagraphRow)
				return row.LastParagraphRowOriginalHeight - CalculateActualMaxParameterValue() - row.SpacingBefore;
			else
				return row.Height - CalculateActualMaxParameterValue() - row.SpacingBefore;
		}
	}
	#endregion
	#region LineSpacingCalculatorBase (abstract class)
	public abstract class LineSpacingCalculatorBase {
		public virtual int CalculateSpacing(int rowHeight, int maxAscentAndFree, int maxDescent, int maxPictureHeight) {
			if (maxAscentAndFree == 0 && maxDescent == 0)
				return rowHeight;
			int rowTextHeight = maxAscentAndFree + maxDescent;
			int rowTextSpacing = CalculateSpacingCore(rowTextHeight);
			if (maxAscentAndFree >= maxPictureHeight)
				return rowTextSpacing;
			else {
				if (maxDescent == 0) 
					return rowHeight;
				else
					return CalculateSpacingInlineObjectCase(rowHeight, maxDescent, rowTextHeight, rowTextSpacing, maxPictureHeight);
			}
		}
		protected internal abstract int CalculateSpacingInlineObjectCase(int rowHeight, int maxDescent, int rowTextHeight, int rowTextSpacing, int maxPictureHeight);
		public abstract int CalculateSpacingCore(int rowHeight);
		public virtual int DefaultRowHeight { get { return 0; } }
		public virtual int CalcRowHeight(int oldRowHeight, int newBoxHeight) {
			return Math.Max(oldRowHeight, newBoxHeight);
		}
		public virtual int CalcRowHeightFromInlineObject(int oldRowHeight, int maxPictureHeight, int maxDescent) {
			return Math.Max(oldRowHeight, maxPictureHeight + maxDescent);
		}
		public static LineSpacingCalculatorBase Create(Paragraph paragraph) {
			switch (paragraph.LineSpacingType) {
				default:
				case ParagraphLineSpacing.Single:
					return new SingleSpacingCalculator();
				case ParagraphLineSpacing.Sesquialteral:
					return new SesquialteralSpacingCalculator();
				case ParagraphLineSpacing.Double:
					return new DoubleSpacingCalculator();
				case ParagraphLineSpacing.Multiple:
					float multiplier = paragraph.LineSpacing;
					return new MultipleSpacingCalculator(multiplier != 0f ? multiplier : 1.0f);
				case ParagraphLineSpacing.AtLeast:
					return new AtLeastSpacingCalculator(Math.Max(1, paragraph.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits((int)paragraph.LineSpacing)));
				case ParagraphLineSpacing.Exactly:
					return new ExactlySpacingCalculator(Math.Max(1, paragraph.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits((int)paragraph.LineSpacing)));
			}
		}
		protected internal virtual RowSpacingParametersCalculatorBase CreateParametersCalculator(ParagraphFinalFormatter formatter, int parameterCount) {
			return new RowSpacingParametersCalculator(formatter, parameterCount);
		}
		public virtual void FormatRow(ParagraphFinalFormatter formatter, Row row) {
			BoxCollection boxes = GetRowContentBoxes(row);
			int count = boxes.Count;
			if (count <= 0)
				return;
			RowSpacingParametersCalculatorBase calculator = CreateParametersCalculator(formatter, count);
			for (int i = 0; i < count; i++)
				calculator.ProcessBox(boxes[i], i);
			row.BaseLineOffset = calculator.CalculateRowBaseLineOffset(row) + row.SpacingBefore;
			AlignRowBoxesToBaseLine(formatter, row, count, calculator.ParameterValues); 
		}
		protected virtual BoxCollection GetRowContentBoxes(Row row) {
			if (row.NumberingListBox == null)
				return row.Boxes;
			BoxCollection result = new BoxCollection();
			result.Add(row.NumberingListBox);
			result.AddRange(row.Boxes);
			return result;
		}
		protected internal virtual void AlignRowBoxesToBaseLine(ParagraphFinalFormatter formatter, Row row, int count, int[] ascents) {
			int baseLine = row.BaseLineOffset;
			BoxCollection boxes = GetRowContentBoxes(row);
			int rowTop = row.Bounds.Top;
			for (int i = 0; i < count; i++) {
				Box box = boxes[i];
				Rectangle r = box.Bounds;
				r.Y = rowTop;
				r.Y += baseLine - ascents[i];
				r.Y += formatter.CalculateSubscriptOrSuperScriptOffset(box);
				box.Bounds = r;
			}
		}
	}
	#endregion
	#region MultipleSpacingCalculator
	public class MultipleSpacingCalculator : LineSpacingCalculatorBase {
		float multiplier;
		public MultipleSpacingCalculator(float multiplier) {
			if (multiplier <= 0)
				Exceptions.ThrowArgumentException("multiplier", multiplier);
			this.multiplier = multiplier;
		}
		public override int CalculateSpacingCore(int rowHeight) {
			return (int)(rowHeight * multiplier);
		}
		protected internal override int CalculateSpacingInlineObjectCase(int rowHeight, int maxDescent, int rowTextHeight, int rowTextSpacing, int maxPictureHeight) {
				return rowHeight + (rowTextSpacing - rowTextHeight);
		}
	}
	#endregion
	#region SingleSpacingCalculator
	public class SingleSpacingCalculator : LineSpacingCalculatorBase {
		public override int CalculateSpacingCore(int rowHeight) {
			return rowHeight;
		}
		protected internal override int CalculateSpacingInlineObjectCase(int rowHeight, int maxDescent, int rowTextHeight, int rowTextSpacing, int maxPictureHeight) {
			return maxPictureHeight + maxDescent;
		}
	}
	#endregion
	#region DoubleSpacingCalculator
	public class DoubleSpacingCalculator : LineSpacingCalculatorBase {
		public override int CalculateSpacingCore(int rowHeight) {
			return 2 * rowHeight;
		}
		protected internal override int CalculateSpacingInlineObjectCase(int rowHeight, int maxDescent, int rowTextHeight, int rowTextSpacing, int maxPictureHeight) {
			return rowHeight + (rowTextSpacing - rowTextHeight);
		}
	}
	#endregion
	#region SesquialteralSpacingCalculator
	public class SesquialteralSpacingCalculator : LineSpacingCalculatorBase {
		public override int CalculateSpacingCore(int rowHeight) {
			return 3 * rowHeight / 2;
		}
		protected internal override int CalculateSpacingInlineObjectCase(int rowHeight, int maxDescent, int rowTextHeight, int rowTextSpacing, int maxPictureHeight) {
			return rowHeight + (rowTextSpacing - rowTextHeight);
		}
	}
	#endregion
	#region ExactlySpacingCalculator
	public class ExactlySpacingCalculator : LineSpacingCalculatorBase {
		int spacing;
		int baseLineOffset;
		public ExactlySpacingCalculator(int spacing) {
			if (spacing <= 0)
				Exceptions.ThrowArgumentException("spacing", spacing);
			this.spacing = spacing;
			this.baseLineOffset = CalcBaseLineOffset();
		}
		public override int DefaultRowHeight { get { return spacing; } }
		int CalcBaseLineOffset() {
			return 1825 * spacing / (1825 + 443);
		}
		public override int CalculateSpacing(int rowHeight, int maxAscent, int maxDescent, int maxPictureHeight) {
			return spacing;
		}
		public override int CalculateSpacingCore(int rowHeight) {
			return spacing;
		}
		public override int CalcRowHeight(int oldRowHeight, int newBoxHeight) {
			return spacing;
		}
		public override int CalcRowHeightFromInlineObject(int oldRowHeight, int maxPictureHeight, int maxDescent) {
			return spacing;
		}
		protected internal override int CalculateSpacingInlineObjectCase(int rowHeight, int maxDescent, int rowTextHeight, int rowTextSpacing, int maxPictureHeight) {
			Exceptions.ThrowInternalException();
			return 0;
		}
		protected internal override RowSpacingParametersCalculatorBase CreateParametersCalculator(ParagraphFinalFormatter formatter, int parameterCount) {
			return new ExactlyRowSpacingParametersCalculator(formatter, parameterCount, baseLineOffset);
		}
	}
	#endregion
	#region AtLeastSpacingCalculator
	public class AtLeastSpacingCalculator : LineSpacingCalculatorBase {
		int spacing;
		public AtLeastSpacingCalculator(int spacing) {
			if (spacing <= 0)
				Exceptions.ThrowArgumentException("spacing", spacing);
			this.spacing = spacing;
		}
		public override int DefaultRowHeight { get { return spacing; } }
		public override int CalculateSpacing(int rowHeight, int maxAscentAndFree, int maxDescent, int maxPictureHeight) {
			return Math.Max(spacing, rowHeight);
		}
		public override int CalculateSpacingCore(int rowHeight) {
			return Math.Max(spacing, rowHeight);
		}
		protected internal override int CalculateSpacingInlineObjectCase(int rowHeight, int maxDescent, int rowTextHeight, int rowTextSpacing, int maxPictureHeight) {
			Exceptions.ThrowInternalException();
			return 0;
		}
		protected internal override RowSpacingParametersCalculatorBase CreateParametersCalculator(ParagraphFinalFormatter formatter, int parameterCount) {
			return new AtLeastRowSpacingParametersCalculator(formatter, parameterCount);
		}
	}
	#endregion
}
