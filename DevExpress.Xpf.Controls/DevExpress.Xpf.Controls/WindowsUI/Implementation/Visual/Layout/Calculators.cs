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
using System.Windows.Controls;
namespace DevExpress.Xpf.WindowsUI.Base {
	static class LayoutCalculatorFactory{
		public static ILayoutCalculator Resolve(bool isHorz, PageHeadersLayoutType layoutType) {
			switch(layoutType){
				case DevExpress.Xpf.WindowsUI.PageHeadersLayoutType.Scroll:
					break;
				default:
					return new ClipLayoutCalculator() { IsHorizontal = isHorz };
			}
			throw new NotSupportedException("layoutType");
		}
	}
	public class ClipLayoutCalculator : ILayoutCalculator {
		public ClipLayoutCalculator() {
			IsHorizontal = true;
		}
		public bool IsHorizontal { get; set; }
		public ILayoutCalculatorResult Measure(ILayoutCalculatorOptions options) {
			Rect[] rects = null;
			Size totalSize = new Size();
			if(options != null) {
				rects = GetRects(options);
				totalSize = GetTotalSize(options, rects);
				for(int i = 0; i < options.Headers.Length; i++) {
					if(IsHorizontal) {
						if(rects[i].Width != 0) {
							rects[i].Height = totalSize.Height;
						}
					} else {
						if(rects[i].Height != 0) {
							rects[i].Width = totalSize.Width;
						}
					}
				}
			}
			return new BaseMeasureResult(rects, totalSize);
		}
		Size GetTotalSize(ILayoutCalculatorOptions options, Rect[] rects) {
			double usedWidth = 0, usedHeight = 0;
			int availableItemsCount = 0;
			Size totalSize = new Size();
			for(int i = 0; i < rects.Length; i++) {
				if(IsHorizontal) {
					if(0 != rects[i].Width) {
						usedWidth += rects[i].Width;
						availableItemsCount++;
					}
				} else {
					if(0 != rects[i].Height) {
						usedHeight += rects[i].Height;
						availableItemsCount++;
					}
				}
			}
			if(availableItemsCount > 0) {
				if(IsHorizontal) {
					double maxHeight = options.AvailableSize.Height;
					if(double.IsInfinity(maxHeight)) {
						maxHeight = 0;
					}
					for(int i = 0; i < options.Headers.Length; i++) {
						if(rects[i].Width != 0) {
							maxHeight = Math.Max(maxHeight, options.Headers[i].Header.Height);
						}
					}
					totalSize.Height = maxHeight;
					totalSize.Width = usedWidth + (availableItemsCount - 1) * options.Spacing;
				} else {
					double maxWidth = options.AvailableSize.Width;
					if(double.IsInfinity(maxWidth)) {
						maxWidth = 0;
					}
					for(int i = 0; i < options.Headers.Length; i++) {
						if(rects[i].Height != 0) {
							maxWidth = Math.Max(maxWidth, options.Headers[i].Header.Width);
						}
					}
					totalSize.Height = usedHeight + (availableItemsCount - 1) * options.Spacing;
					totalSize.Width = maxWidth;
				}
			}
			return totalSize;
		}
		double GetLength(Size s) {
			return IsHorizontal ? s.Width : s.Height;
		}
		Rect[] GetRects(ILayoutCalculatorOptions options) {
			bool isAnyItemSelected = false;
			int selectedItemIndex = -1;
			int currentItemIndex = 0;
			double offsetToSelectedItem = 0;
			bool isSelectedItemVisible = false;
			Rect[] rects = null;
			bool selectedItemIsToBeArranged = true;
			double availableLength = GetLength(options.AvailableSize);
			foreach(IItemHeaderInfo item in options.Headers) {
				if(item.IsSelected) {
					isAnyItemSelected = true;
					selectedItemIndex = currentItemIndex;
					double header = GetLength(item.Header);
					selectedItemIsToBeArranged = header <= availableLength;
					break;
				}
				if(!isAnyItemSelected) {
					offsetToSelectedItem += (GetLength(item.Header) + options.Spacing);
				}
				currentItemIndex++;
			}
			if(isAnyItemSelected) {
				if(selectedItemIsToBeArranged) {
					isSelectedItemVisible = GetLength(options.Headers[selectedItemIndex].Header) + offsetToSelectedItem <= availableLength;
				}
				else {
					return new Rect[options.Headers.Length];
				}
			}
			int ignorableItemsCount = 0;
			double ignorableItemsLength = 0;
			if(!isSelectedItemVisible && isAnyItemSelected) {
				foreach(IItemHeaderInfo item in options.Headers) {
					if(ignorableItemsCount == selectedItemIndex)
						break;
					double itemLength = GetLength(item.Header);
					ignorableItemsLength += itemLength + options.Spacing;
					ignorableItemsCount++;
					if(GetLength(options.Headers[selectedItemIndex].Header) + offsetToSelectedItem - ignorableItemsLength <= availableLength) {
						isSelectedItemVisible = true;
						break;
					}
				}
			}
			if(isSelectedItemVisible || !isAnyItemSelected) {
				rects = new Rect[options.Headers.Length];
				double X = 0, Y = 0;
				for(int i = ignorableItemsCount; i < options.Headers.Length; i++) {
					double currentHeaderLength = GetLength(options.Headers[i].Header);
					if(IsHorizontal) {
						if(X + currentHeaderLength <= availableLength) {
							rects[i].X = X;
							X += (currentHeaderLength + options.Spacing);
							rects[i].Width = currentHeaderLength;
						}
					}
					else {
						if(Y + currentHeaderLength <= availableLength) {
							rects[i].Y = Y;
							Y += (currentHeaderLength + options.Spacing);
							rects[i].Height = currentHeaderLength;
						}
					}
				}
			}
			return rects;
		}
	}
	public class BaseMeasureOptions : ILayoutCalculatorOptions {
		public BaseMeasureOptions(Size availableSize, IItemHeaderInfo[] headers, double spacing) {
			availableSizeCore = availableSize;
			headersCore = headers;
			spacingCore = spacing;
		}
		private Size availableSizeCore;
		private IItemHeaderInfo[] headersCore;
		private double spacingCore;
		#region ILayoutCalculatorOptions Members
		public Size AvailableSize {
			get { return availableSizeCore; }
		}
		public IItemHeaderInfo[] Headers {
			get { return headersCore; }
		}
		public double Spacing {
			get { return spacingCore; }
		}
		#endregion
	}
	public class BaseMeasureResult : ILayoutCalculatorResult {
		public BaseMeasureResult(Rect[] rects, Size totalSize) {
			itemRectsCore = rects;
			totalSizeCore = totalSize;
		}
		private Size totalSizeCore;
		Rect[] itemRectsCore;
		#region ILayoutCalculatorResult Members
		public Size TotalSize {
			get { return totalSizeCore; }
		}
		public Rect[] ItemRects {
			get { return itemRectsCore; }
		}
		#endregion
	}
	public class BaseItemOptions : IItemHeaderInfo {
		public BaseItemOptions(Size header, bool isSelected) {
			headerCore = header;
			isSelectedCore = isSelected;
		}
		Size headerCore;
		bool isSelectedCore;
		public Size Header {
			get { return headerCore; }
		}
		public bool IsSelected {
			get { return isSelectedCore; }
		}
	}
}
