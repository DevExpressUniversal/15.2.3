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
using System.Windows;
using DevExpress.Xpf.WindowsUI.Base;
namespace DevExpress.Xpf.WindowsUI.Internal {
	public interface ISplitStrategy {
		Rect[] Split(Rect content, ILayoutCell[] layoutCells, double spacing, bool horizontal);
		Rect[] Split(Rect content, ILayoutCell[] layoutCells, double spacing, bool horizontal, bool allowClipping);
	}
	class ProportionalSplitStrategy : ISplitStrategy {
		public Rect[] Split(Rect content, ILayoutCell[] layoutCells, double spacing, bool horizontal) {
			return Split(content, layoutCells, spacing, horizontal, false);
		}
		public Rect[] Split(Rect content, ILayoutCell[] layoutCells, double spacing, bool horizontal, bool allowClipping) {
			return ProportionalLayoutCalculator.Calc(content, layoutCells, spacing, horizontal);
		}
	}
	static class ProportionalLayoutCalculator {
		public static Rect[] Calc(Rect bounds, ILayoutCell[] layoutCells, bool horizontal) {
			return Calc(bounds, layoutCells, 0.0, horizontal);
		}
		public static Rect[] Calc(Rect bounds, ILayoutCell[] layoutCells, double spacing, bool horizontal) {
			double[] spacings = new double[Math.Max(0, layoutCells.Length - 1)];
			for(int i = 0; i < spacings.Length; i++)
				spacings[i] = spacing;
			return Calc(bounds, layoutCells, spacings, horizontal);
		}
		public static Rect[] Calc(Rect bounds, ILayoutCell[] layoutCells, double[] spacings, bool horizontal) {
			Rect[] result = new Rect[layoutCells.Length];
			if(layoutCells.Length == 0)
				return result;
			int proportionalLengthsCount = 0;
			int desiredLengthsCount = 0;
			double totalDesiredLength = 0;
			double maxDesiredLength = 0;
			double totalProportionalLength = 0;
			for(int i = 0; i < layoutCells.Length; i++) {
				if(!layoutCells[i].IsAutoSize) {
					proportionalLengthsCount++;
					double cellLength = MathHelper.GetDimension(layoutCells[i].StretchRatio, 1.0);
					totalProportionalLength += cellLength;
				}
				else {
					desiredLengthsCount++;
					double cellLength = layoutCells[i].GetLength(horizontal);
					totalDesiredLength += cellLength;
					maxDesiredLength = Math.Max(maxDesiredLength, cellLength);
				}
			}
			double totalSpacing = 0;
			for(int i = 0; i < spacings.Length; i++)
				totalSpacing += spacings[i];
			double availableLength = (horizontal ? bounds.Width : bounds.Height);
			double e = 0;
			bool isInfiniteMeasure = double.IsInfinity(availableLength) || double.IsNaN(availableLength);
			if(!isInfiniteMeasure) {
				if(!MathHelper.AreClose(availableLength, totalDesiredLength))
					availableLength = Math.Max(0, availableLength - totalSpacing);
				e = (availableLength - totalDesiredLength) / totalProportionalLength;
			}
			else e = 0;
			double[] lengths = new double[layoutCells.Length];
			CorrectionEntry[] entries = new CorrectionEntry[layoutCells.Length];
			for(int i = 0; i < lengths.Length; i++) {
				double desiredLength = layoutCells[i].GetLength(horizontal);
				if(isInfiniteMeasure || layoutCells[i].IsAutoSize)
					lengths[i] = desiredLength;
				else {
					double ratio = MathHelper.GetDimension(layoutCells[i].StretchRatio, 1.0);
					lengths[i] = e * ratio;
					if(lengths[i] < 0) {
						lengths[i] = 0;
						if(i > 0)
							spacings[i - 1] = 0;
					}
					if(!isInfiniteMeasure)
						entries[i] = new CorrectionEntry(i, ratio, desiredLength - lengths[i]);
				}
			}
			if(!isInfiniteMeasure) {
				Array.Sort(entries, CorrectionEntry.Compare);
				for(int i = 0; i < entries.Length; i++) {
					if(entries[i] == null) continue;
					if(entries[i].Value > 0)
						CorrectionEntry.Correct(entries, i + 1, entries[i].Value);
					else break;
				}
				for(int i = 0; i < entries.Length; i++) {
					if(entries[i] == null) continue;
					lengths[entries[i].Index] += entries[i].Correction;
				}
			}
			double offset = 0; int length = 0;
			double round = 0;
			for(int i = 0; i < result.Length; i++) {
				double d = (double)lengths[i] + round;
				length = (int)(d + 0.5d);
				round = d - (double)length;
				if(horizontal)
					result[i] = new Rect(bounds.Left + offset, bounds.Top, length, bounds.Height);
				else
					result[i] = new Rect(bounds.Left, bounds.Top + offset, bounds.Width, length);
				if(i + 1 < result.Length)
					offset += (length + spacings[i]);
			}
			if(!isInfiniteMeasure)
				LayoutAlignmentHelper.CorrectCellRectsByAlignment(result, layoutCells, bounds, spacings, horizontal);
			return result;
		}
		class CorrectionEntry {
			public CorrectionEntry(int index, double ratio, double value) {
				this.Index = index;
				this.Ratio = ratio;
				this.valueCore = value;
			}
			public readonly int Index;
			public readonly double Ratio;
			double valueCore;
			public double Value {
				get { return valueCore; }
			}
			double correctionCore;
			public double Correction {
				get { return correctionCore; }
			}
			public static int Compare(CorrectionEntry e1, CorrectionEntry e2) {
				if(e1 == e2) return 0;
				if(e1 == null) return -1;
				if(e2 == null) return 1;
				return e2.Value.CompareTo(e1.Value);
			}
			public static void Correct(CorrectionEntry[] entries, int startPos, double value) {
				double totalRatio = 0;
				double totalValue = 0;
				for(int i = startPos; i < entries.Length; i++) {
					if(entries[i] == null) continue;
					totalRatio += entries[i].Ratio;
					totalValue += entries[i].Value;
				}
				if(!MathHelper.IsZero(value + totalValue) && (value + totalValue) > 0)
					return;
				entries[startPos - 1].correctionCore = value;
				double e = value / totalRatio;
				for(int i = startPos; i < entries.Length; i++) {
					if(entries[i] == null) continue;
					var entry = entries[i];
					double c = entry.Ratio * e;
					var correctionValue = (c + entry.Value);
					if(MathHelper.IsZero(correctionValue) || correctionValue < 0) {
						entry.correctionCore -= c;
						value -= c;
						entry.valueCore += c;
					}
					else {
						entry.correctionCore += entry.Value;
						value += entry.Value;
						entry.valueCore = 0.0;
						if(MathHelper.AreClose(totalRatio, entry.Ratio))
							break;
						totalRatio -= entry.Ratio;
						e = value / totalRatio;
					}
				}
			}
			public override string ToString() {
				return string.Format("{1},[{0}],{2}*,({3})", Index, Value, Ratio, Correction);
			}
		}
	}
	class StackedSplitStrategy : ISplitStrategy {
		public Rect[] Split(Rect content, ILayoutCell[] layoutCells, double spacing, bool horizontal) {
			return Split(content, layoutCells, spacing, horizontal, false);
		}
		public Rect[] Split(Rect content, ILayoutCell[] layoutCells, double spacing, bool horizontal, bool allowClipping) {
			double[] splitLengths = new double[layoutCells.Length];
			for(int i = 0; i < layoutCells.Length; i++) {
				splitLengths[i] = horizontal ? layoutCells[i].DesiredSize.Width : layoutCells[i].DesiredSize.Height;
			}
			Rect[] rects = StackedLayoutCalculator.Calc(content, splitLengths, spacing, horizontal);
			double availableLength = (horizontal ? content.Width : content.Height);
			if(!double.IsInfinity(availableLength))
				CorrectCellsByAlignment(content, layoutCells, spacing, horizontal, rects);
			return rects;
		}
		protected virtual void CorrectCellsByAlignment(Rect content, ILayoutCell[] layoutCells, double spacing, bool horizontal, Rect[] rects) {
			LayoutAlignmentHelper.CorrectCellRectsByAlignment(rects, layoutCells, content, spacing, horizontal);
		}
	}
	static class StackedLayoutCalculator {
		public static Rect[] Calc(Rect bounds, double[] splitLengths, double spacing, bool horizontal, bool allowClipping = false) {
			Rect[] result = new Rect[splitLengths.Length];
			if(splitLengths.Length == 0)
				return result;
			double offset = 0;
			double maxOffset = horizontal ? bounds.Width : bounds.Height;
			double length = 0;
			for(int i = 0; i < result.Length; i++) {
				length = splitLengths[i];
				if(!allowClipping && offset + length - maxOffset > 0.1)
					length = 0;
				if(horizontal)
					result[i] = new Rect(bounds.Left + offset, bounds.Top, length, bounds.Height);
				else
					result[i] = new Rect(bounds.Left, bounds.Top + offset, bounds.Width, length);
				if(i + 1 < result.Length && length > 0)
					offset += (length + spacing);
			}
			return result;
		}
	}
}
