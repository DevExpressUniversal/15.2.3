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
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class XYDiagramPaneLayout {
		public readonly XYDiagramPaneBase Pane;
		public readonly Rectangle MaxBounds;
		public readonly Rectangle InitialMappingBounds;
		public XYDiagramPaneLayout(XYDiagramPaneBase pane, Rectangle maxBounds, Rectangle initialMappingBounds) {
			this.Pane = pane;
			this.MaxBounds = maxBounds;
			this.InitialMappingBounds = initialMappingBounds;
		}
	}
	public class XYDiagramPaneLayoutCalculator {
		readonly IList<IPane> panes;
		readonly int paneDistance;
		XYDiagramPaneLayoutHelper layoutHelper;
		public XYDiagramPaneLayoutCalculator(IList<IPane> panes, int paneDistance, PaneLayoutDirection paneLayoutDirection, Rectangle originalMaxBounds, Rectangle maxBounds) {
			ChartDebug.Assert(panes.Count > 0);
			ChartDebug.Assert(paneDistance >= 0);
			this.panes = panes;
			this.paneDistance = Math.Max(paneDistance, 0);
			this.layoutHelper = XYDiagramPaneLayoutHelper.CreateInstance(paneLayoutDirection, originalMaxBounds, maxBounds);
		}
		List<Rectangle> CalculateMaxBoundsList() {
			int startValue;
			int endValue;
			int length;
			layoutHelper.GetMaxBoundsParams(out startValue, out endValue, out length);
			int actualLength = length - paneDistance * (panes.Count - 1);
			if(actualLength <= 0)
				return null;
			XYDiagramPaneLengthCalculator lengthCalculator = new XYDiagramPaneLengthCalculator(panes, actualLength);
			int[] paneLengthArray = lengthCalculator.Calculate();
			ChartDebug.Assert(paneLengthArray.Length == panes.Count);
			List<Rectangle> maxBoundsList = new List<Rectangle>(panes.Count);
			int paneStartValue = startValue;
			for(int i = 0; i < paneLengthArray.Length - 1; i++) {
				int paneEndValue = paneStartValue + paneLengthArray[i];
				maxBoundsList.Add(layoutHelper.CreatePaneMaxBounds(paneStartValue, paneEndValue));
				paneStartValue = paneEndValue + paneDistance;
			}
			ChartDebug.Assert(paneStartValue <= endValue);
			maxBoundsList.Add(layoutHelper.CreatePaneMaxBounds(paneStartValue, endValue));
			return maxBoundsList;
		}
		public List<XYDiagramPaneLayout> Calculate() {
			List<Rectangle> maxBoundsList = CalculateMaxBoundsList();
			if(maxBoundsList == null)
				return null;			
			for (int i = 0; i < panes.Count; i++) {
				XYDiagramPaneBase pane = (XYDiagramPaneBase)panes[i];
				if (!pane.Fixed)
					pane.SetSizeInPixels(layoutHelper.GetLength(maxBoundsList[i]));
			}
			List<Rectangle> initialMappingBoundsList = new List<Rectangle>(maxBoundsList);
			layoutHelper.WidenMaxBoundsList(maxBoundsList);
			List<XYDiagramPaneLayout> paneLayoutList = new List<XYDiagramPaneLayout>();
			for (int i = 0; i < panes.Count; i++)
				paneLayoutList.Add(new XYDiagramPaneLayout((XYDiagramPaneBase)panes[i], maxBoundsList[i], initialMappingBoundsList[i]));
			return paneLayoutList;
		}
	}
	public class XYDiagramPaneLengthCalculator {
		#region inner classes
		class PaneLengthInfo {
			public readonly int Length;
			public readonly int Index;
			public PaneLengthInfo(int length, int index) {
				Length = length;
				Index = index;
			}
		}
		class PaneWeightInfo {
			public readonly double Weight;
			public readonly int Index;
			public PaneWeightInfo(double weight, int index) {
				Weight = weight;
				Index = index;
			}
		}
		#endregion
		readonly IList<IPane> panes;
		readonly int length;
		public XYDiagramPaneLengthCalculator(IList<IPane> panes, int length) {
			ChartDebug.Assert(panes.Count > 0);
			ChartDebug.Assert(length >= 0);
			this.panes = panes;
			this.length = Math.Max(length, 0);
		}
		int CalculateFixedLength(out bool allPanesFixed) {
			allPanesFixed = true;
			int fixedLength = 0;
			foreach(XYDiagramPaneBase pane in panes) {
				if(pane.Fixed)
					fixedLength += pane.SizeInPixels;
				else
					allPanesFixed = false;
			}
			return fixedLength;
		}
		int CalculateInitialPaneInfoListsWhenNormalCase(List<PaneLengthInfo> fixedLengthList, List<PaneWeightInfo> resizableWeightList) {
			int fixedLength = 0;
			for(int i = 0; i < panes.Count; i++) {
				XYDiagramPaneBase pane = (XYDiagramPaneBase)panes[i];
				if (pane.Fixed) {
					fixedLength += pane.SizeInPixels;
					fixedLengthList.Add(new PaneLengthInfo(pane.SizeInPixels, i));
				}
				else
					resizableWeightList.Add(new PaneWeightInfo(pane.Weight, i));
			}
			return fixedLength;
		}
		int GetSizeInPixelsOfFirstFixedPane(out int index) {
			for (index = 0; index < panes.Count; index++) {
				XYDiagramPaneBase pane = (XYDiagramPaneBase)panes[index];
				if (pane.Fixed)
					return pane.SizeInPixels;
			}
			return 0;
		}
		void CalculateInitialPaneInfoListsWhenOverflowCase(List<PaneWeightInfo> resizableWeightList) {
			int index;
			int sizeInPixels = GetSizeInPixelsOfFirstFixedPane(out index);
			resizableWeightList.Add(new PaneWeightInfo(1.0, index));
			if(sizeInPixels == 0)
				return;
			for (int i = index + 1; i < panes.Count; i++) {
				XYDiagramPaneBase pane = (XYDiagramPaneBase)panes[i];
				if (pane.Fixed) {
					double weight = (double)pane.SizeInPixels / sizeInPixels;
					resizableWeightList.Add(new PaneWeightInfo(weight, i));
				}
			}
		}
		int CalculateInitialPaneInfoLists(List<PaneLengthInfo> fixedLengthList, List<PaneWeightInfo> resizableWeightList) {
			bool allPanesFixed;
			int fixedLength = CalculateFixedLength(out allPanesFixed);
			int resultFixedLength = 0;
			if(fixedLength < length) {
				if(allPanesFixed)
					CalculateInitialPaneInfoListsWhenOverflowCase(resizableWeightList);
				else
					resultFixedLength = CalculateInitialPaneInfoListsWhenNormalCase(fixedLengthList, resizableWeightList);
			}
			else
				CalculateInitialPaneInfoListsWhenOverflowCase(resizableWeightList);
		   return length - resultFixedLength;
		}
		double CalculateWeightSum(List<PaneWeightInfo> resizableWeightList) {
			double weightSum = 0.0;
			foreach(PaneWeightInfo info in resizableWeightList)
				weightSum += info.Weight;
			return weightSum;
		}
		List<PaneLengthInfo> CalculateResizableLengthList(List<PaneWeightInfo> resizableWeightList, int resizableLength) {
			ChartDebug.Assert(resizableWeightList.Count > 0);
			List<PaneLengthInfo> resizableLengthList = new List<PaneLengthInfo>(resizableWeightList.Count);
			double weightSum = CalculateWeightSum(resizableWeightList);
			double startValue = 0.0;
			double endValue = 0.0;
			for(int i = 0; i < resizableWeightList.Count - 1; i++) {
				endValue += (double)resizableLength * resizableWeightList[i].Weight / weightSum;
				int startValueRounded = MathUtils.StrongRound(startValue);
				int endValueRounded = MathUtils.StrongRound(endValue);
				int length = endValueRounded - startValueRounded;
				resizableLengthList.Add(new PaneLengthInfo(length, resizableWeightList[i].Index));
				startValue = endValue;
			}
			int lastLength = resizableLength - MathUtils.StrongRound(startValue);
			PaneWeightInfo lastWeightInfo = resizableWeightList[resizableWeightList.Count - 1];
			resizableLengthList.Add(new PaneLengthInfo(lastLength, lastWeightInfo.Index));
			return resizableLengthList;
		}
		int[] CalculateTotalLengthArray(List<PaneLengthInfo> fixedLengthList, List<PaneLengthInfo> resizableLengthList) {
			int[] totalLengthArray = new int[panes.Count];
			for(int i = 0; i < totalLengthArray.Length; i++)
				totalLengthArray[i] = 0;
			foreach(PaneLengthInfo info in fixedLengthList)
				totalLengthArray[info.Index] = info.Length;
			foreach(PaneLengthInfo info in resizableLengthList)
				totalLengthArray[info.Index] = info.Length;
			return totalLengthArray;
		}
		public int[] Calculate() {
			if(panes.Count == 0)
				return new int[0];
			if(length == 0)
				return CalculateTotalLengthArray(new List<PaneLengthInfo>(), new List<PaneLengthInfo>());
			List<PaneLengthInfo> fixedLengthList = new List<PaneLengthInfo>();
			List<PaneWeightInfo> resizableWeightList = new List<PaneWeightInfo>();
			int resizableLength = CalculateInitialPaneInfoLists(fixedLengthList, resizableWeightList);
			ChartDebug.Assert(resizableLength <= length);			
			List<PaneLengthInfo> resizableLengthList = CalculateResizableLengthList(resizableWeightList, resizableLength);
			ChartDebug.Assert(resizableLengthList.Count == resizableWeightList.Count);
			return CalculateTotalLengthArray(fixedLengthList, resizableLengthList);
		}
	}
	public abstract class XYDiagramPaneLayoutHelper {
		public static XYDiagramPaneLayoutHelper CreateInstance(PaneLayoutDirection paneLayoutDirection, Rectangle originalMaxBounds, Rectangle maxBounds) {
			switch(paneLayoutDirection) {
				case PaneLayoutDirection.Vertical:
					return new XYDiagramPaneVerticalLayoutHelper(originalMaxBounds, maxBounds);
				case PaneLayoutDirection.Horizontal:
					return new XYDiagramPaneHorizontalLayoutHelper(originalMaxBounds, maxBounds);
				default:
					throw new DefaultSwitchException();
			}
		}
		readonly Rectangle originalMaxBounds;
		readonly Rectangle maxBounds;
		protected Rectangle OriginalMaxBounds { get { return originalMaxBounds; } }
		protected Rectangle MaxBounds { get { return maxBounds; } }
		public XYDiagramPaneLayoutHelper(Rectangle originalMaxBounds, Rectangle maxBounds) {
			this.originalMaxBounds = originalMaxBounds;
			this.maxBounds = maxBounds;
		}
		protected abstract int GetStartValue(Rectangle bounds);
		protected abstract int GetEndValue(Rectangle bounds);
		protected abstract Rectangle WidenFirstMaxBounds(Rectangle maxBounds);
		protected abstract Rectangle WidenMaxBounds(Rectangle maxBounds);
		protected abstract Rectangle WidenLastMaxBounds(Rectangle maxBounds);
		public abstract int GetLength(Rectangle bounds);
		public abstract Rectangle CreatePaneMaxBounds(int paneStartValue, int paneEndValue);
		public void GetMaxBoundsParams(out int startValue, out int endValue, out int length) {
			startValue = GetStartValue(maxBounds);
			endValue = GetEndValue(maxBounds);
			length = GetLength(maxBounds);
		}
		public void WidenMaxBoundsList(List<Rectangle> maxBoundsList) {
			if(maxBoundsList.Count == 0)
				return;
			foreach(Rectangle maxBounds in maxBoundsList) {
				ChartDebug.Assert(maxBounds.Left >= originalMaxBounds.Left);
				ChartDebug.Assert(maxBounds.Top >= originalMaxBounds.Top);
				ChartDebug.Assert(maxBounds.Right <= originalMaxBounds.Right);
				ChartDebug.Assert(maxBounds.Bottom <= originalMaxBounds.Bottom);
			}
			maxBoundsList[0] = WidenFirstMaxBounds(maxBoundsList[0]);
			for(int i = 1; i < maxBoundsList.Count - 1; i++)
				maxBoundsList[i] = WidenMaxBounds(maxBoundsList[i]);
			maxBoundsList[maxBoundsList.Count - 1] = WidenLastMaxBounds(maxBoundsList[maxBoundsList.Count - 1]);
		}
	}
	public class XYDiagramPaneVerticalLayoutHelper : XYDiagramPaneLayoutHelper {
		public XYDiagramPaneVerticalLayoutHelper(Rectangle originalMaxBounds, Rectangle maxBounds) : base(originalMaxBounds, maxBounds) {
		}
		protected override int GetStartValue(Rectangle bounds) {
			return bounds.Top;
		}
		protected override int GetEndValue(Rectangle bounds) {
			return bounds.Bottom;
		}
		protected override Rectangle WidenFirstMaxBounds(Rectangle maxBounds) {
			return new Rectangle(OriginalMaxBounds.X, OriginalMaxBounds.Y, OriginalMaxBounds.Width, maxBounds.Height + maxBounds.Top - OriginalMaxBounds.Top);
		}
		protected override Rectangle WidenMaxBounds(Rectangle maxBounds) {
			return new Rectangle(OriginalMaxBounds.X, maxBounds.Y, OriginalMaxBounds.Width, maxBounds.Height);
		}
		protected override Rectangle WidenLastMaxBounds(Rectangle maxBounds) {
			return new Rectangle(OriginalMaxBounds.X, maxBounds.Y, OriginalMaxBounds.Width, maxBounds.Height + OriginalMaxBounds.Bottom - maxBounds.Bottom);
		}
		public override int GetLength(Rectangle bounds) {
			return bounds.Height;
		}
		public override Rectangle CreatePaneMaxBounds(int paneStartValue, int paneEndValue) {
			return new Rectangle(MaxBounds.Left, paneStartValue, MaxBounds.Width, paneEndValue - paneStartValue);
		}
	}
	public class XYDiagramPaneHorizontalLayoutHelper : XYDiagramPaneLayoutHelper {
		public XYDiagramPaneHorizontalLayoutHelper(Rectangle originalMaxBounds, Rectangle maxBounds) : base(originalMaxBounds, maxBounds) {
		}
		protected override int GetStartValue(Rectangle bounds) {
			return bounds.Left;
		}
		protected override int GetEndValue(Rectangle bounds) {
			return bounds.Right;
		}
		protected override Rectangle WidenFirstMaxBounds(Rectangle maxBounds) {
			return new Rectangle(OriginalMaxBounds.X, OriginalMaxBounds.Y, maxBounds.Width + maxBounds.Left - OriginalMaxBounds.Left, OriginalMaxBounds.Height);
		}
		protected override Rectangle WidenMaxBounds(Rectangle maxBounds) {
			return new Rectangle(maxBounds.X, OriginalMaxBounds.Y, maxBounds.Width, OriginalMaxBounds.Height);
		}
		protected override Rectangle WidenLastMaxBounds(Rectangle maxBounds) {
			return new Rectangle(maxBounds.X, OriginalMaxBounds.Y, maxBounds.Width + OriginalMaxBounds.Right - maxBounds.Right, OriginalMaxBounds.Height);
		}
		public override int GetLength(Rectangle bounds) {
			return bounds.Width;
		}
		public override Rectangle CreatePaneMaxBounds(int paneStartValue, int paneEndValue) {
			return new Rectangle(paneStartValue, MaxBounds.Top, paneEndValue - paneStartValue, MaxBounds.Height);
		}
	}	  
}
