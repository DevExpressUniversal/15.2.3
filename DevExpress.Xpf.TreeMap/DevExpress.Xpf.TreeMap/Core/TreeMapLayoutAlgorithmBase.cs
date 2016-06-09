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

using DevExpress.Xpf.TreeMap;
using DevExpress.Xpf.TreeMap.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace DevExpress.TreeMap.Core {
	public enum LayoutAlgorithmDirection { 
		TopLeftToBottomRight,
		BottomLeftToTopRight,
		TopRightToBottomLeft,
		BottomRightToTopLeft 
	};
	public enum SliceAndDiceAlgorithmLayoutMode {
		Auto,
		Vertical,
		Horizontal,
	};
	abstract public class LayoutAlgorithmBase {		
		public abstract void Calculate(List<ITreeMapLayoutItem> sortedData, Size size);
	}
	public abstract class SquarifiedAlgorithmBase : LayoutAlgorithmBase {
		#region inner classes
		protected class RectCalculator {
			SquarifiedAlgorithmBase algorithm;
			bool isLeftDirection { get { return algorithm.Direction == LayoutAlgorithmDirection.TopLeftToBottomRight || algorithm.Direction == LayoutAlgorithmDirection.BottomLeftToTopRight; } }
			bool isTopDirection { get { return algorithm.Direction == LayoutAlgorithmDirection.TopLeftToBottomRight || algorithm.Direction == LayoutAlgorithmDirection.TopRightToBottomLeft; } }
			public RectCalculator(SquarifiedAlgorithmBase algorithm) {
				this.algorithm = algorithm;
			}
			public TreeMapRect CalculateFirstRect(int i, TreeMapRect rect, List<ITreeMapLayoutItem> data, bool isVertical) {
				TreeMapRect treeMapRect = new TreeMapRect();
				double sum = (new List<ITreeMapLayoutItem>(data.GetRange(i, data.Count - i))).Sum(d => (d.Weight > 0) ? d.Weight : 0);		
				treeMapRect.Width = (isVertical) ? data[i].Weight / sum * rect.Width : rect.Width;
				treeMapRect.Height = (isVertical) ? rect.Height : data[i].Weight / sum * rect.Height;
				treeMapRect.X = isLeftDirection ? rect.X : rect.Width - treeMapRect.Width;
				treeMapRect.Y = isTopDirection ? rect.Y : treeMapRect.Y = rect.Height - treeMapRect.Height;
				return treeMapRect;
			}
			public TreeMapRect CalculateRect(double side, double anchorSide, double ratio, TreeMapRect rect, double offset, bool isVertical, Size availableSize) {
				TreeMapRect treeMapRect = new TreeMapRect();
				if (isVertical) {
					treeMapRect.Height = Math.Min(side, availableSize.Height);
					treeMapRect.Width = Math.Min(anchorSide, availableSize.Width);
					treeMapRect.X = isLeftDirection ? rect.X : rect.Width - treeMapRect.Width;
					treeMapRect.Y = isTopDirection ? rect.Y + offset : rect.Height - (treeMapRect.Height + offset);
				}
				else {
					treeMapRect.Width = Math.Min(side, availableSize.Width);
					treeMapRect.Height = Math.Min(anchorSide, availableSize.Height);
					treeMapRect.X = isLeftDirection ? rect.X + offset : rect.Width - (treeMapRect.Width + offset);
					treeMapRect.Y = isTopDirection ? rect.Y : rect.Height - treeMapRect.Height;
				}
				return treeMapRect;
			}
			public TreeMapRect CalculateNewInitialRect(TreeMapRect initialRect, TreeMapRect rect, bool isVertical) {
				if (isVertical) {
					initialRect.X = initialRect.X + rect.Width;
					initialRect.Width = initialRect.Width - rect.Width;
					return new TreeMapRect(initialRect.X, initialRect.Y, initialRect.Width, initialRect.Height);
				}
				else {
					initialRect.Y = initialRect.Y + rect.Height;
					initialRect.Height = initialRect.Height - rect.Height;
					return new TreeMapRect(initialRect.X, initialRect.Y, initialRect.Width, initialRect.Height);
				}
			}
		}
		#endregion
		RectCalculator rectCalculator;
		LayoutAlgorithmDirection direction;
		protected RectCalculator RectCalculatorDirection { get { return rectCalculator; } }
		public LayoutAlgorithmDirection Direction { get { return direction; } }
		public SquarifiedAlgorithmBase(LayoutAlgorithmDirection direction) {
			this.direction = direction;
			rectCalculator = new RectCalculator(this);
		}
		void FillItemsLayout(List<ITreeMapLayoutItem> sortedData, List<TreeMapRect> temp, int index) {
			foreach (TreeMapRect rect in temp) {
				sortedData[index++].Layout = RectConverterHelper.ConvertToRect(rect);
			}
		}
		bool IsCurrentCoefficientImproveList(double coef, List<double> listCoef) {
			return listCoef.Any() ? coef >= listCoef.Max() : true;
		}
		List<double> GetSides(double anchor, List<ITreeMapLayoutItem> data) {
			double sum = data.Sum(d => (d.Weight > 0) ? d.Weight : 0);
			List<double> res = new List<double>();
			for (int i = 0; i < data.Count(); i++) {
				res.Add(data[i].Weight / sum * anchor);
			}
			return res;
		}
		protected abstract bool NeedContinueRow(Size size, TreeMapRect initialRect, List<TreeMapRect> temp);
		protected abstract bool GetOrientationInitialRectangle(TreeMapRect initialRect, bool isVertical);
		protected void CalculateSquarifiedAndStriped(List<ITreeMapLayoutItem> sortedData, Size size) {
			TreeMapRect firstRect = new TreeMapRect();
			TreeMapRect initialRect = new TreeMapRect(0, 0, size.Width, size.Height);
			int newDirectionIndex = 0;
			bool isVertical = initialRect.Width > initialRect.Height;
			double ratio = initialRect.Height * initialRect.Width / sortedData.Sum(d => (d.Weight > 0) ? d.Weight : 0);
			List<TreeMapRect> tempRect = new List<TreeMapRect>();
			double coef = 0;
			for (int i = 0; i < sortedData.Count; i++) {
				if (sortedData[i].Weight <= 0)
					tempRect.Add(new TreeMapRect(0, 0, 0, 0));
				else {
					int rectCount = tempRect.Count;
					if (rectCount == 0) {
						firstRect = RectCalculatorDirection.CalculateFirstRect(i, initialRect, sortedData, isVertical);
						coef = Math.Max(firstRect.Height / firstRect.Width, firstRect.Width / firstRect.Height);
						tempRect.Add(firstRect);
					}
					else {
						double offset = 0;
						List<ITreeMapLayoutItem> ranges = sortedData.GetRange(newDirectionIndex, i - newDirectionIndex + 1);
						List<double> sides = GetSides(isVertical ? initialRect.Height : initialRect.Width, ranges);
						double anchorSide = sortedData[newDirectionIndex].Weight / sides[0] * ratio;
						List<TreeMapRect> temp = new List<TreeMapRect>();
						temp.AddRange(tempRect);
						List<double> listCoef = new List<double>();
						tempRect.Clear();
						for (int j = 0; j <= rectCount; j++) {
							TreeMapRect treeMapRect = RectCalculatorDirection.CalculateRect(sides[j], anchorSide, ratio, initialRect, offset, isVertical, size);
							offset += (isVertical) ? treeMapRect.Height : treeMapRect.Width;
							double coefCurrent = Math.Max(treeMapRect.Width / treeMapRect.Height, treeMapRect.Height / treeMapRect.Width);
							if (IsCurrentCoefficientImproveList(coef, listCoef)) {
								listCoef.Add(coefCurrent);
								tempRect.Add(treeMapRect);
							}
							else if (NeedContinueRow(size, initialRect, temp))
								tempRect.Add(new TreeMapRect(treeMapRect.X, treeMapRect.Y, treeMapRect.Width, treeMapRect.Height));
							else {
								FillItemsLayout(sortedData, temp, newDirectionIndex);
								tempRect.Clear();
								initialRect = RectCalculatorDirection.CalculateNewInitialRect(initialRect, temp[0], isVertical);
								newDirectionIndex = i;
								isVertical = GetOrientationInitialRectangle(initialRect, isVertical);
								firstRect = RectCalculatorDirection.CalculateFirstRect(i, initialRect, sortedData, isVertical);
								listCoef.Clear();
								coef = Math.Max(firstRect.Height / firstRect.Width, firstRect.Width / firstRect.Height);
								tempRect.Add(new TreeMapRect(firstRect.X, firstRect.Y, firstRect.Width, firstRect.Height));
								break;
							}
						}
						coef = listCoef.Count > 0 ? listCoef.Max() : coef;
					}
				}
			}
			FillItemsLayout(sortedData, tempRect, newDirectionIndex);
		}
	}
	public sealed class SquarifiedAlgorithm : SquarifiedAlgorithmBase {
		public SquarifiedAlgorithm() : this(LayoutAlgorithmDirection.TopLeftToBottomRight) { }
		public SquarifiedAlgorithm(LayoutAlgorithmDirection direction) : base(direction) { }
		protected override bool NeedContinueRow(Size size, TreeMapRect initialRect, List<TreeMapRect> temp) {
			return false;
		}
		protected override bool GetOrientationInitialRectangle(TreeMapRect initialRect, bool isVertical) {
			return initialRect.Width > initialRect.Height ? true : false;
		}
		public override void Calculate(List<ITreeMapLayoutItem> sortedData, Size size) {
			CalculateSquarifiedAndStriped(sortedData, size);
		}
	}
	public sealed class StripedAlgorithm : SquarifiedAlgorithmBase {
		const double defaultLastStripeMinThickness = 0.025;
		double LastStripeMinThickness;
		public StripedAlgorithm() : this(LayoutAlgorithmDirection.TopLeftToBottomRight, defaultLastStripeMinThickness) { }
		public StripedAlgorithm(LayoutAlgorithmDirection direction, double LastStripeMinThickness)
			: base(direction) {
				this.LastStripeMinThickness = LastStripeMinThickness;
		}
		protected override bool NeedContinueRow(Size size, TreeMapRect initialRect, List<TreeMapRect> temp) {
			return (((initialRect.Width - temp[0].Width) / size.Width < LastStripeMinThickness) ||
				((initialRect.Height - temp[0].Height) / size.Height < LastStripeMinThickness));
		}
		protected override bool GetOrientationInitialRectangle(TreeMapRect initialRect, bool isVertical) {
			return isVertical;
		}
		public override void Calculate(List<ITreeMapLayoutItem> sortedData, Size size) {
			CalculateSquarifiedAndStriped(sortedData, size);
		}
	}
	public sealed class SliceAndDiceAlgorithm : LayoutAlgorithmBase {
		LayoutAlgorithmDirection direction;
		SliceAndDiceAlgorithmLayoutMode mode;
		public SliceAndDiceAlgorithm() : this(LayoutAlgorithmDirection.TopLeftToBottomRight, SliceAndDiceAlgorithmLayoutMode.Auto) { }
		public SliceAndDiceAlgorithm(LayoutAlgorithmDirection direction, SliceAndDiceAlgorithmLayoutMode mode) {
			this.direction = direction;
			this.mode = mode;
		}
		bool IsVerticalDirection(Size size) {
			switch (mode) {
				case SliceAndDiceAlgorithmLayoutMode.Vertical:
					return false;
				case SliceAndDiceAlgorithmLayoutMode.Horizontal:
					return true;
				case SliceAndDiceAlgorithmLayoutMode.Auto:
				default:
					return size.Width < size.Height;
			}
		}
		bool IsFromLeft() {
			return direction == LayoutAlgorithmDirection.TopLeftToBottomRight || direction == LayoutAlgorithmDirection.BottomLeftToTopRight;
		}
		bool IsFromTop() {
			return direction == LayoutAlgorithmDirection.TopLeftToBottomRight || direction == LayoutAlgorithmDirection.TopRightToBottomLeft;
		}
		void GetKoefficients(Size size, out double begin, out int sign, out int koef) {
			if (IsVerticalDirection(size))
				if (IsFromTop()) {
					begin = 0;
					sign = 1;
				}
				else {
					begin = size.Height;
					sign = -1;
				}
			else
				if (IsFromLeft()) {
					begin = 0;
					sign = 1;
				}
				else {
					begin = size.Width;
					sign = -1;
				}
			koef = sign == -1 ? -1 : 0;
		}
		public override void Calculate(List<ITreeMapLayoutItem> sortedData, Size size) {
			double offset = 0;
			double begin;
			int sign;
			int koef;
			double sumWeight = sortedData.Sum(d => (d.Weight > 0) ? d.Weight : 0);
			bool isVertical = (size.Height > size.Width);
			GetKoefficients(size, out begin, out sign, out koef);
			for (int i = 0; i < sortedData.Count; i++) {
				if (sortedData[i].Weight <= 0) {
					sortedData[i].Layout = new Rect(0, 0, 0, 0);
				}
				else {
					TreeMapRect treeMapRect = new TreeMapRect();
					if (IsVerticalDirection(size)) {
						treeMapRect.Width = size.Width;
						treeMapRect.Height = sortedData[i].Weight / sumWeight * size.Height;
						treeMapRect.X = 0;
						treeMapRect.Y = begin + offset * sign + treeMapRect.Height * koef;
						offset += treeMapRect.Height;
					}
					else {
						treeMapRect.Width = sortedData[i].Weight / sumWeight * size.Width;
						treeMapRect.Height = size.Height;
						treeMapRect.X = begin + offset * sign + treeMapRect.Width * koef;
						treeMapRect.Y = 0;
						offset += treeMapRect.Width;
					}
					sortedData[i].Layout = RectConverterHelper.ConvertToRect(treeMapRect);
				}
			}
		}
	}
}
