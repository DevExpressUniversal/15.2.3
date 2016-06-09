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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
using System.Windows.Controls;
using DevExpress.Xpf.Utils.Native;
using System.Collections;
namespace DevExpress.Xpf.Core.Native {
	public class RowInfoCollection {
		static double GetValidSize(double normalSize, double fixedSize) {
			return double.IsNaN(fixedSize) ? normalSize : fixedSize;
		}
		static Size GetValidSize(Size availableSize, SizeHelperBase sizeHelper, double fixedSize) {
			return sizeHelper.CreateSize(GetValidSize(sizeHelper.GetDefineSize(availableSize), fixedSize), sizeHelper.GetSecondarySize(availableSize));
		}
		static double GetSecondarySizeWithSeparators(Size size, CardsPanelInfo info) {
			return info.SizeHelper.GetSecondarySize(size) + info.SeparatorThickness;
		}
		static List<RowInfo> CalculateRowInfo(Size availableSize, CardsPanelInfo panelInfo, IList<UIElement> sortedChildren) {
			if(panelInfo.MaxCardCountInRow <= 0)
				throw new ArgumentOutOfRangeException("maxCardCountInRow");
			List<RowInfo> rowInfo = new List<RowInfo>();
			double currentHeight = 0;
			double currentWidth = 0;
			int currentElementCount = 0;
			SizeHelperBase sizeHelper = panelInfo.SizeHelper;
			for(int i = 0; i < sortedChildren.Count; i++) {
				UIElement element = sortedChildren[i];
				Size elementDesiredSize = GetElementDesiredSize(
					GetValidSize(element.DesiredSize, panelInfo.SizeHelper, panelInfo.FixedSize),
					panelInfo.CardMargin);
				if(IsEnoughItems(availableSize, panelInfo, currentWidth, currentElementCount, elementDesiredSize)) {
					rowInfo.Add(new RowInfo() {
						Size = sizeHelper.CreateSize(currentHeight, currentWidth),
						ElementCount = currentElementCount
					});
					currentHeight = sizeHelper.GetDefineSize(elementDesiredSize);
					currentWidth = sizeHelper.GetSecondarySize(elementDesiredSize);
					currentElementCount = 1;
					continue;
				}
				currentElementCount++;
				currentWidth += sizeHelper.GetSecondarySize(elementDesiredSize);
				currentHeight = Math.Max(currentHeight, sizeHelper.GetDefineSize(elementDesiredSize));
			}
			rowInfo.Add(new RowInfo() { Size = sizeHelper.CreateSize(currentHeight, currentWidth), ElementCount = currentElementCount });
			return rowInfo;
		}
		static bool IsEnoughItems(Size availableSize, CardsPanelInfo panelInfo, double currentWidth, int currentElementCount, Size elementDesiredSize) {
			return currentWidth + GetSecondarySizeWithSeparators(elementDesiredSize, panelInfo) >= panelInfo.SizeHelper.GetSecondarySize(availableSize) && currentElementCount > 0 || currentElementCount >= panelInfo.MaxCardCountInRow;
		}
		static Size GetElementDesiredSize(Size size, Thickness margin) {
			size.Height += margin.Bottom + margin.Top;
			size.Width += margin.Left + margin.Right;
			return size;
		}
		CardsPanelInfo panelInfo;
		List<LineInfo> rowSeparators = new List<LineInfo>();
		List<RowInfo> info = new List<RowInfo>();
		public int Count { get { return info.Count; } }
		public List<LineInfo> RowSeparators { get { return rowSeparators; } }
		public RowInfo this[int index] { get { return info[index]; } }
		public RowInfoCollection(CardsPanelInfo panelInfo) {
			this.panelInfo = panelInfo;
		}
		Size GetUnboundInSecondaryDirectionSize(Size size) {
			double defineSize = panelInfo.SizeHelper.GetDefineSize(size);
			return panelInfo.SizeHelper.CreateSize(defineSize, double.PositiveInfinity);
		}
		public Size Measure(Size availableSize, IList<UIElement> sortedChildren) {
			foreach(UIElement element in sortedChildren)
				element.Measure(GetValidSize(GetUnboundInSecondaryDirectionSize(availableSize), panelInfo.SizeHelper, panelInfo.FixedSize));
			info.Clear();
			info.AddRange(CalculateRowInfo(availableSize, panelInfo, sortedChildren));
			double total = 0;
			SizeHelperBase sizeHelper = panelInfo.SizeHelper;
			foreach(RowInfo item in info)
				total += sizeHelper.GetDefineSize(item.Size);
			total += (info.Count - 1) * panelInfo.SeparatorThickness;
			if(double.IsPositiveInfinity(sizeHelper.GetSecondarySize(availableSize)))
				availableSize = sizeHelper.CreateSize(sizeHelper.GetDefineSize(availableSize), sizeHelper.GetSecondarySize(info[0].Size));
			return sizeHelper.CreateSize(total, sizeHelper.GetSecondarySize(availableSize));
		}
		public Rect[] Arrange(Size finalSize, IList<UIElement> sortedChildren) {
			int childIndex = 0;
			Rect[] rects = new Rect[sortedChildren.Count];
			double currentHeight = 0;
			SizeHelperBase sizeHelper = panelInfo.SizeHelper;
			RowSeparators.Clear();
			foreach(RowInfo item in info) {
				double cardSpace = CalcCardSpace(item, sizeHelper.GetSecondarySize(finalSize), panelInfo.Alignment, sizeHelper);
				double currentPosition = CalcNearOffset(item, sizeHelper.GetSecondarySize(finalSize), panelInfo.Alignment, panelInfo.SizeHelper);
				for(int i = childIndex; i < childIndex + item.ElementCount; i++) {
					UIElement element = sortedChildren[i];
					rects[i] = CreateCardRect(cardSpace, currentHeight, currentPosition, element);
					currentPosition += sizeHelper.GetSecondarySize(rects[i].Size);
				}
				childIndex += item.ElementCount;
				currentHeight += sizeHelper.GetDefineSize(item.Size) + panelInfo.SeparatorThickness;
				if(RowSeparators.Count < info.Count - 1)
					RowSeparators.Add(new LineInfo() { Location = sizeHelper.CreatePoint(currentHeight - panelInfo.SeparatorThickness, 0), Length = sizeHelper.GetSecondarySize(finalSize) });
			}
			return rects;
		}
		Rect CreateCardRect(double cardSpace, double currentHeight, double currentPosition, UIElement element) {
			SizeHelperBase sizeHelper = panelInfo.SizeHelper;
			Point location = sizeHelper.CreatePoint(currentHeight, currentPosition + cardSpace);
			Size rectSize = GetValidSize(element.DesiredSize, sizeHelper, panelInfo.FixedSize);
			return new Rect(location, GetElementDesiredSize(rectSize, panelInfo.CardMargin));
		}
		public double CalcCardSpace(RowInfo item, double finalSize, Alignment alignment, SizeHelperBase sizeHelper) {
			return alignment == Alignment.Center ? (finalSize - sizeHelper.GetSecondarySize(item.Size)) / 2 : 0;
		}
		public double CalcNearOffset(RowInfo item, double finalSize, Alignment alignment, SizeHelperBase sizeHelper) {
			return alignment == Alignment.Far ? finalSize - sizeHelper.GetSecondarySize(item.Size) : 0;
		}
	}
}
