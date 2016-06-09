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
using DevExpress.Utils;
namespace DevExpress.Charts.Native {
	public enum ScrollingOrientation {
		None,
		AxisXScroll,
		AxisYScroll,
		BothAxesScroll
	}
	public enum ZoomingKind {
		None,
		MouseWheel,
		Keyboard,
		ZoomIn,
		ZoomOut,
		SetRange,
		ZoomPosistion,
		Gesture
	}
	public enum NavigationType {
		LargeDecrement,
		LargeIncrement,
		SmallDecrement,
		SmallIncrement,
		ThumbPosition,
		LeftButtonMouseDrag,
		MiddleButtonMouseDrag,
		ArrowKeys,
		ZoomIn,
		ZoomOut,
		SetRange,
		ZoomUndo,
		Command,
		Gesture,
		ZoomThumbRelease,
		ZoomReset
	}
	public interface IZoomablePane : IPane {
		bool Rotated { get; }
		bool ScrollingByXEnabled { get; }
		bool ScrollingByYEnabled { get; }
		bool ZoomingByXEnabled { get; }
		bool ZoomingByYEnabled { get; }
		double AxisXMaxZoomPercent { get; }
		double AxisYMaxZoomPercent { get; }
		ZoomCacheEx ZoomCacheEx { get; }
		GRect2D Bounds { get; }
		void RangeLimitsUpdated();
		void BeginZooming();
		void EndZooming(NavigationType navigationType, RangesSnapshot oldRange, RangesSnapshot newRange);
		void EndScrolling(ScrollingOrientation orientation, NavigationType navigationType, RangesSnapshot oldRange, RangesSnapshot newRange);
	}
	public class ZoomCacheItemEx {
		readonly PaneAxesContainer paneAxesData;
		readonly AxisRangeInfo axisXRange;
		readonly AxisRangeInfo axisYRange;
		public PaneAxesContainer PaneAxesData { get { return paneAxesData; } }
		public AxisRangeInfo AxisXRange { get { return axisXRange; } }
		public AxisRangeInfo AxisYRange { get { return axisYRange; } }
		public ZoomCacheItemEx(PaneAxesContainer paneAxesData) {
			this.paneAxesData = paneAxesData;
			axisXRange = new AxisRangeInfo(paneAxesData.PrimaryAxisX.VisualRange);
			axisYRange = new AxisRangeInfo(paneAxesData.PrimaryAxisY.VisualRange);
		}
	}
	public class ZoomCacheEx {
		readonly Stack<ZoomCacheItemEx> stack = new Stack<ZoomCacheItemEx>();
		ZoomingKind lastZoomingKind = ZoomingKind.None;
		public bool IsEmpty { get { return stack.Count == 0; } }
		public void Push(ZoomCacheItemEx item, ZoomingKind zoomingKind) {
			if (zoomingKind != lastZoomingKind || zoomingKind == ZoomingKind.ZoomIn || zoomingKind == ZoomingKind.ZoomOut) {
				lastZoomingKind = zoomingKind;
				stack.Push(item);
			}
		}
		public void Pop() {
			if (stack.Count > 0) {
				ZoomCacheItemEx item = stack.Pop();
				item.PaneAxesData.UndoZoom(item);
				lastZoomingKind = ZoomingKind.None;
			}
		}
		public void Clear() {
			stack.Clear();
		}
	}
	public class PaneAxesContainer : IPaneAxesContainer {
		public const double DefaultMaximumZoomPercent = 100.0;
		class AxesList : SortedList<int, IAxisData> {
			public void Register(int index, IAxisData axis) {
				if (!ContainsKey(index))
					Add(index, axis);
			}
		}
		static double ClampPosition(double value) {
			if (value < 0)
				return 0;
			if (value > 1)
				return 1;
			return value;
		}
		static double CalcAxisStep(IAxisData axis, int step, int size, bool useReverse) {
			if (!CanScrollAxis(axis))
				return 0.0;
			IVisualAxisRangeData visualRange = axis.VisualRange;
			IWholeAxisRangeData wholeRange = axis.WholeRange;
			ITransformation transformation = axis.AxisScaleTypeMap.Transformation;
			double delta = transformation.TransformForward(visualRange.Max) - transformation.TransformForward(visualRange.Min);
			double axisStep = delta / size * step;
			if (axisStep == 0.0)
				return axisStep;
			if (useReverse && axis.Reverse)
				axisStep = -axisStep;
			if (axisStep > 0.0) {
				double newMax = transformation.TransformForward(visualRange.Max) + axisStep;
				double diff = newMax - transformation.TransformForward(wholeRange.Max);
				if (diff > 0.0)
					axisStep -= diff;
			}
			else {
				double newMin = transformation.TransformForward(visualRange.Min) + axisStep;
				double diff = newMin - transformation.TransformForward(wholeRange.Min);
				if (diff < 0.0)
					axisStep -= diff;
			}
			return axisStep;
		}
		static bool Scroll(IAxisData axis, int step, int size, bool useReverse) {
			double axisStep = CalcAxisStep(axis, step, size, useReverse);
			if (axisStep == 0.0)
				return false;
			IVisualAxisRangeData visualRange = axis.VisualRange;
			ITransformation transformation = axis.AxisScaleTypeMap.Transformation;
			double min = transformation.TransformBackward(transformation.TransformForward(visualRange.Min) + axisStep);
			double max = transformation.TransformBackward(transformation.TransformForward(visualRange.Max) + axisStep);
			SetMinMax(axis, min, max);
			return true;
		}
		static bool ScrollTo(IAxisData axis, double position) {
			position = ClampPosition(position);
			ITransformation transformation = axis.AxisScaleTypeMap.Transformation;
			IMinMaxValues visualRange = Transform(transformation, axis.VisualRange);
			IMinMaxValues wholeRange = Transform(transformation, axis.WholeRange);
			double delta = visualRange.Delta;
			double scrollingDelta = wholeRange.Delta;
			if (axis.Reverse)
				position = 1.0 - position;
			double min, max;
			if (delta >= scrollingDelta) {
				min = wholeRange.Min;
				max = wholeRange.Max;
			}
			else {
				min = wholeRange.Min + (scrollingDelta - delta) * position;
				max = min + delta;
			}
			if (min == visualRange.Min && max == visualRange.Max)
				return false;
			double minValue = transformation.TransformBackward(min);
			double maxValue = transformation.TransformBackward(max);
			SetMinMax(axis, minValue, maxValue);
			return true;
		}
		static object InternalToNative(AxisScaleTypeMap scaleMap, double internalValue) {
			return scaleMap.RefinedToNative(scaleMap.InternalToRefinedExact(internalValue));
		}
		public static bool CanScrollAxis(IAxisData axis) {
			return CanZoomOutAxis(axis);
		}
		public static bool CanZoomOutAxis(IAxisData axis) {
			return axis.WholeRange.Min != axis.VisualRange.Min || axis.WholeRange.Max != axis.VisualRange.Max;
		}
		public static void SetMinMax(IAxisData axis, double min, double max) {
			IVisualAxisRangeData visualRange = axis.VisualRange;
			ILogarithmic logarithmic = axis as ILogarithmic;
			object minValue = null;
			object maxValue = null;
			AxisScaleTypeMap scaleMap = axis.AxisScaleTypeMap;
			if (scaleMap.ScaleType == ActualScaleType.Numerical) {
				minValue = InternalToNative(scaleMap, min);
				maxValue = InternalToNative(scaleMap, max);
				visualRange.ScrollOrZoomRange(minValue, maxValue, min, max);
			}
			else if (scaleMap.ScaleType == ActualScaleType.DateTime) {
				if (!Double.IsNaN(min))
					minValue = InternalToNative(scaleMap, min);
				if (!Double.IsNaN(max))
					maxValue = InternalToNative(scaleMap, max);
				visualRange.ScrollOrZoomRange(minValue, maxValue, min, max);
			}
			else if (scaleMap.ScaleType == ActualScaleType.Qualitative) {
				double roundedMin = Math.Min(Math.Ceiling(min), Math.Floor(max));
				double roundedMax = Math.Max(Math.Ceiling(min), Math.Floor(max));
				minValue = scaleMap.InternalToNative(roundedMin);
				maxValue = scaleMap.InternalToNative(roundedMax);
				visualRange.ScrollOrZoomRange(minValue, maxValue, min, max);
			}
		}	   
		static IMinMaxValues Transform(ITransformation transformation, IMinMaxValues minMaxValue) {
			double min = transformation.TransformForward(minMaxValue.Min);
			double max = transformation.TransformForward(minMaxValue.Max);
			return new MinMaxValues(min, max);
		}
		public static double GetScrollBarPosition(IAxisData axis) {
			if (!CanZoomOutAxis(axis))
				return 0.5;
			IMinMaxValues visualRange = Transform(axis.AxisScaleTypeMap.Transformation, axis.VisualRange);
			double center = (visualRange.Min + visualRange.Max) / 2.0;
			double delta = visualRange.Delta;
			double halfDelta = delta / 2.0;
			IMinMaxValues wholeRange = Transform(axis.AxisScaleTypeMap.Transformation, axis.WholeRange);
			double centerMin = wholeRange.Min + halfDelta;
			double offset = center - centerMin;
			double range = wholeRange.Delta - delta;
			return (offset + range == offset) ? 0.5 : (offset / range);
		}
		public static double GetScrollBarRelativeSize(IAxisData axis) {
			IMinMaxValues visualRange = Transform(axis.AxisScaleTypeMap.Transformation, axis.VisualRange);
			IMinMaxValues wholeRange = Transform(axis.AxisScaleTypeMap.Transformation, axis.WholeRange);
			return visualRange.Delta / wholeRange.Delta;
		}
		readonly IPane pane;
		readonly AxesList axesX = new AxesList();
		readonly AxesList axesY = new AxesList();
		readonly List<IAxisData> secondaryAxesX = new List<IAxisData>();
		readonly List<IAxisData> secondaryAxesY = new List<IAxisData>();
		readonly List<IAxisData> totalSecondaryAxesX = new List<IAxisData>();
		readonly List<IAxisData> totalSecondaryAxesY = new List<IAxisData>();
		IAxisData primaryAxisX;
		IAxisData primaryAxisY;
		public IPane Pane { get { return pane; } }
		public IList<IAxisData> AxesX { get { return axesX.Values; } }
		public IList<IAxisData> AxesY { get { return axesY.Values; } }
		public IList<IAxisData> SecondaryAxesX { get { return secondaryAxesX; } }
		public IList<IAxisData> SecondaryAxesY { get { return secondaryAxesY; } }
		public List<IAxisData> TotalSecondaryAxesX { get { return totalSecondaryAxesX; } }
		public List<IAxisData> TotalSecondaryAxesY { get { return totalSecondaryAxesY; } }
		public IAxisData PrimaryAxisX { get { return primaryAxisX; } }
		public IAxisData PrimaryAxisY { get { return primaryAxisY; } }
		public IEnumerable<IAxisData> Axes {
			get {
				foreach (IAxisData axis in AxesX)
					yield return axis;
				foreach (IAxisData axis in AxesY)
					yield return axis;
			}
		}
		public bool ScrollingEnabled {
			get {
				IZoomablePane zoomablePane = pane as IZoomablePane;
				return zoomablePane != null &&
					   ((zoomablePane.ScrollingByXEnabled && CanScrollAxis(primaryAxisX)) ||
						(zoomablePane.ScrollingByYEnabled && CanScrollAxis(primaryAxisY)));
			}
		}
		public bool CanZoomOut {
			get {
				IZoomablePane zoomablePane = pane as IZoomablePane;
				return zoomablePane != null &&
					   ((zoomablePane.ZoomingByXEnabled && CanZoomOutAxis(primaryAxisX)) ||
						(zoomablePane.ZoomingByYEnabled && CanZoomOutAxis(primaryAxisY)));
			}
		}
		public PaneAxesContainer(IPane pane) {
			this.pane = pane;
		}
		public bool CanZoomIn() {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			return zoomablePane != null &&
				   ((zoomablePane.ZoomingByXEnabled && CanZoomInAxis(primaryAxisX)) ||
					(zoomablePane.ZoomingByYEnabled && CanZoomInAxis(primaryAxisY)));
		}
		public void RegisterAxisX(int index, IAxisData axis) {
			axesX.Register(index, axis);
		}
		public void RegisterAxisY(int index, IAxisData axis) {
			axesY.Register(index, axis);
		}
		public void InitializePrimaryAndSecondaryAxes() {
			IList<IAxisData> axesX = AxesX;
			if (axesX.Count > 0) {
				primaryAxisX = axesX[0];
				for (int i = 1; i < axesX.Count; i++)
					secondaryAxesX.Add(axesX[i]);
			}
			IList<IAxisData> axesY = AxesY;
			if (axesY.Count > 0) {
				primaryAxisY = axesY[0];
				for (int i = 1; i < axesY.Count; i++)
					secondaryAxesY.Add(axesY[i]);
			}
		}
		public void InitializeTotalSecondaryAxes(PaneAxesContainerRepository repository) {
			foreach (PaneAxesContainer controller in repository.Values) {
				if (Object.ReferenceEquals(controller.primaryAxisX, primaryAxisX))
					foreach (IAxisData axis in controller.secondaryAxesX)
						if (!totalSecondaryAxesX.Contains(axis))
							totalSecondaryAxesX.Add(axis);
				if (Object.ReferenceEquals(controller.primaryAxisY, primaryAxisY))
					foreach (IAxisData axis in controller.secondaryAxesY)
						if (!totalSecondaryAxesY.Contains(axis))
							totalSecondaryAxesY.Add(axis);
			}
		}
		public void RangeLimitsUpdated() {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null)
				zoomablePane.RangeLimitsUpdated();
		}
		public bool CanScroll(int dx, int dy, bool useReverse) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane == null)
				return false;
			GRect2D bounds = zoomablePane.Bounds;
			if (bounds.IsEmpty)
				return false;
			if (zoomablePane.Rotated) {
				if ((zoomablePane.ScrollingByYEnabled && CalcAxisStep(primaryAxisY, -dx, bounds.Width, useReverse) != 0.0) ||
					(zoomablePane.ScrollingByXEnabled && CalcAxisStep(primaryAxisX, dy, bounds.Height, useReverse) != 0.0))
					return true;
			}
			else if ((zoomablePane.ScrollingByXEnabled && CalcAxisStep(primaryAxisX, -dx, bounds.Width, useReverse) != 0.0) ||
					 (zoomablePane.ScrollingByYEnabled && CalcAxisStep(primaryAxisY, dy, bounds.Height, useReverse) != 0.0))
				return true;
			return false;
		}
		public bool Scroll(int dx, int dy, bool useReverse, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane == null || !ScrollingEnabled)
				return false;
			GRect2D bounds = zoomablePane.Bounds;
			if (bounds.IsEmpty)
				return false;
			RangesSnapshot rangesDiapason = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
			ScrollingOrientation scrollingOrientation = ScrollingOrientation.None;
			if (zoomablePane.Rotated) {
				if (zoomablePane.ScrollingByYEnabled && Scroll(primaryAxisY, -dx, bounds.Width, useReverse))
					scrollingOrientation = ScrollingOrientation.AxisYScroll;
				if (zoomablePane.ScrollingByXEnabled && Scroll(primaryAxisX, dy, bounds.Height, useReverse))
					scrollingOrientation = scrollingOrientation == ScrollingOrientation.None ?
						ScrollingOrientation.AxisXScroll : ScrollingOrientation.BothAxesScroll;
			}
			else {
				if (zoomablePane.ScrollingByXEnabled && Scroll(primaryAxisX, -dx, bounds.Width, useReverse))
					scrollingOrientation = ScrollingOrientation.AxisXScroll;
				if (zoomablePane.ScrollingByYEnabled && Scroll(primaryAxisY, dy, bounds.Height, useReverse))
					scrollingOrientation = scrollingOrientation == ScrollingOrientation.None ?
						ScrollingOrientation.AxisYScroll : ScrollingOrientation.BothAxesScroll;
			}
			if (scrollingOrientation == ScrollingOrientation.None)
				return false;
			FinishScrolling(scrollingOrientation, rangesDiapason, navigationType);
			return true;
		}
		public bool CanScrollTo(double position, ScrollingOrientation orientation) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane == null)
				return false;
			switch (orientation) {
				case ScrollingOrientation.AxisXScroll:
					return zoomablePane.ScrollingByXEnabled;
				case ScrollingOrientation.AxisYScroll:
					return zoomablePane.ScrollingByYEnabled;
				default:
					return false;
			}
		}
		public bool ScrollTo(double position, ScrollingOrientation orientation, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane == null)
				return false;
			RangesSnapshot rangesSnapshot = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
			switch (orientation) {
				case ScrollingOrientation.AxisXScroll:
					if (!zoomablePane.ScrollingByXEnabled || !ScrollTo(primaryAxisX, position))
						return false;
					break;
				case ScrollingOrientation.AxisYScroll:
					if (!zoomablePane.ScrollingByYEnabled || !ScrollTo(primaryAxisY, position))
						return false;
					break;
				default:
					return false;
			}
			FinishScrolling(orientation, rangesSnapshot, navigationType);
			return true;
		}
		public bool CanZoomInAxis(IAxisData axis) {
			double epsilon = GetMaximumZoomPercent(axis) * 1e-5;
			double ratio = axis.WholeRange.Delta / axis.VisualRange.Delta;
			return ComparingUtils.CompareDoubles(ratio, GetMaximumZoomPercent(axis), epsilon) < 0;
		}
		public void SetAxisXRange(double position1, double position2, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null && zoomablePane.ZoomingByXEnabled) {
				if (navigationType == NavigationType.SetRange)
					BeginZooming(ZoomingKind.SetRange);
				RangesSnapshot rangesDiapason = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
				SetRange(primaryAxisX, position1, position2);
				if (navigationType == NavigationType.SetRange || navigationType == NavigationType.ZoomThumbRelease)
					FinishZooming(rangesDiapason, navigationType);
				else
					FinishScrolling(ScrollingOrientation.AxisXScroll, rangesDiapason, navigationType);
			}
		}
		public void SetAxisYRange(double position1, double position2, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null && zoomablePane.ZoomingByYEnabled) {
				MinMaxValues percents = new MinMaxValues(position1, position2);
				if (navigationType == NavigationType.SetRange)
					BeginZooming(ZoomingKind.SetRange);
				RangesSnapshot rangesDiapason = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
				SetRange(primaryAxisY, position1, position2);
				if (navigationType == NavigationType.SetRange || navigationType == NavigationType.ZoomThumbRelease)
					FinishZooming(rangesDiapason, navigationType);
				else
					FinishScrolling(ScrollingOrientation.AxisYScroll, rangesDiapason, navigationType);
			}
		}
		public void SetAxisXZoom(double factor, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null && zoomablePane.ZoomingByXEnabled)
				SetAxisZoom(primaryAxisX, factor, navigationType);
		}
		public void SetAxisYZoom(double factor, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null && zoomablePane.ZoomingByYEnabled)
				SetAxisZoom(primaryAxisY, factor, navigationType);
		}
		public void Zoom(GPoint2D center, double zoomPercent, ZoomingKind zoomingKind, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null && (zoomablePane.ZoomingByXEnabled || zoomablePane.ZoomingByYEnabled)) {
				GRect2D bounds = zoomablePane.Bounds;
				if (!bounds.IsEmpty) {
					BeginZooming(zoomingKind);
					RangesSnapshot rangesDiapason = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
					PerformZoom(zoomablePane, bounds, center, zoomPercent, zoomPercent);
					FinishZooming(rangesDiapason, navigationType);
				}
			}
		}
		public void Zoom(GPoint2D center, double axisXMin, double axisXMax, double axisYMin, double axisYMax, double xZoomPercent, double yZoomPercent, ZoomingKind zoomingKind, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null && (zoomablePane.ZoomingByXEnabled || zoomablePane.ZoomingByYEnabled)) {
				GRect2D bounds = zoomablePane.Bounds;
				if (!bounds.IsEmpty) {
					BeginZooming(zoomingKind);
					RangesSnapshot rangesDiapason = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
					SetMinMax(primaryAxisX, axisXMin, axisXMax);
					SetMinMax(primaryAxisY, axisYMin, axisYMax);
					PerformZoom(zoomablePane, bounds, center, xZoomPercent, yZoomPercent);
					FinishZooming(rangesDiapason, navigationType);
				}
			}
		}
		public void Zoom(int delta, ZoomingKind zoomingKind) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null && (zoomablePane.ZoomingByXEnabled || zoomablePane.ZoomingByYEnabled)) {
				BeginZooming(zoomingKind);
				var rangesSnapshot = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
				double zoomPercent = -0.1 * delta;
				if (zoomablePane.ZoomingByXEnabled)
					Zoom(primaryAxisX, zoomPercent);
				if (zoomablePane.ZoomingByYEnabled)
					Zoom(primaryAxisY, zoomPercent);
				FinishZooming(rangesSnapshot, delta > 0 ? NavigationType.ZoomIn : NavigationType.ZoomOut);
			}
		}
		public void ZoomIn(GRect2D rect) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null && (zoomablePane.ZoomingByXEnabled || zoomablePane.ZoomingByYEnabled)) {
				GRect2D bounds = zoomablePane.Bounds;
				if (!bounds.IsEmpty) {
					BeginZooming(ZoomingKind.ZoomIn);
					RangesSnapshot rangesDiapason = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
					rect.Offset(-bounds.Left, 0);
					double xMinPercent, xMaxPercent, yMinPercent, yMaxPercent;
					if (zoomablePane.Rotated) {
						yMinPercent = (double)rect.Left / bounds.Width;
						yMaxPercent = (double)rect.Right / bounds.Width;
						xMaxPercent = (double)(bounds.Bottom - rect.Top) / bounds.Height;
						xMinPercent = (double)(bounds.Bottom - rect.Bottom) / bounds.Height;
					}
					else {
						xMinPercent = (double)rect.Left / bounds.Width;
						xMaxPercent = (double)rect.Right / bounds.Width;
						yMaxPercent = (double)(bounds.Bottom - rect.Top) / bounds.Height;
						yMinPercent = (double)(bounds.Bottom - rect.Bottom) / bounds.Height;
					}
					if (zoomablePane.ZoomingByXEnabled)
						ZoomIn(primaryAxisX, xMinPercent, xMaxPercent);
					if (zoomablePane.ZoomingByYEnabled)
						ZoomIn(primaryAxisY, yMinPercent, yMaxPercent);
					FinishZooming(rangesDiapason, NavigationType.ZoomIn);
				}
			}
		}
		public void UndoZoom(ZoomCacheItemEx zoomCacheItem) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null) {
				zoomablePane.BeginZooming();
				RangesSnapshot rangesDiapason = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
				zoomCacheItem.AxisXRange.UpdateRange(primaryAxisX.VisualRange);
				zoomCacheItem.AxisYRange.UpdateRange(primaryAxisY.VisualRange);
				FinishZooming(rangesDiapason, NavigationType.ZoomUndo);
			}
		}
		public void ResetZoom() {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null && (zoomablePane.ZoomingByXEnabled || zoomablePane.ZoomingByYEnabled)) {
				zoomablePane.BeginZooming();
				RangesSnapshot rangesDiapason = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
				if (zoomablePane.ZoomingByXEnabled)
					ResetZoom(primaryAxisX);
				if (zoomablePane.ZoomingByYEnabled)
					ResetZoom(primaryAxisY);
				FinishZooming(rangesDiapason, NavigationType.ZoomReset);
			}
		}
		public GPoint2D GetZoomRegionPosition(GPoint2D p) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane == null)
				return p;
			GRect2D bounds = zoomablePane.Bounds;
			if (bounds.IsEmpty)
				return p;
			if (p.X < bounds.Left)
				p.X = bounds.Left;
			else if (p.X > bounds.Right)
				p.X = bounds.Right;
			if (p.Y < bounds.Top)
				p.Y = bounds.Top;
			else if (p.Y > bounds.Bottom)
				p.Y = bounds.Bottom;
			return p;
		}
		double GetMaximumZoomPercent(IAxisData axis) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (pane != null) {
				double actualZoomPercent = axis.WholeRange.Delta / axis.VisualRange.Delta;
				if (axis == primaryAxisX)
					return Math.Max(actualZoomPercent, zoomablePane.AxisXMaxZoomPercent / 100.0);
				if (axis == primaryAxisY)
					return Math.Max(actualZoomPercent, zoomablePane.AxisYMaxZoomPercent / 100.0);
			}
			return DefaultMaximumZoomPercent;
		}
		void PerformZoom(IAxisData axis, double newMin, double newMax) {
			IWholeAxisRangeData wholeRange = axis.WholeRange;
			double delta = newMax - newMin;
			if (delta < 0.0) {
				newMax = newMin;
				delta = 0.0;
			}
			double maxDelta = wholeRange.Delta / GetMaximumZoomPercent(axis);
			double factor = maxDelta / delta;
			if (Double.IsInfinity(factor) || factor > 1.0) {
				if (!CanZoomInAxis(axis) && delta < axis.VisualRange.Delta)
					return;
				double center = (newMin + newMax) / 2.0;
				maxDelta /= 2.0;
				newMin = center - maxDelta;
				newMax = center + maxDelta;
			}
			double minDiff = wholeRange.Min - newMin;
			if (minDiff > 0) {
				newMin += minDiff;
				newMax += minDiff;
			}
			else {
				double maxDiff = newMax - wholeRange.Max;
				if (maxDiff > 0) {
					newMin -= maxDiff;
					newMax -= maxDiff;
				}
			}
			SetMinMax(axis, newMin, newMax);
		}
		void PerformZoom(IZoomablePane zoomablePane, GRect2D bounds, GPoint2D center, double xZoomPercent, double yZoomPercent) {
			double xRatio = (double)(center.X - bounds.Left) / bounds.Width;
			double yRatio = 1.0 - (double)(center.Y - bounds.Top) / bounds.Height;
			if (zoomablePane.Rotated) {
				if (zoomablePane.ZoomingByYEnabled)
					Zoom(primaryAxisY, xRatio, xZoomPercent);
				if (zoomablePane.ZoomingByXEnabled)
					Zoom(primaryAxisX, yRatio, yZoomPercent);
			}
			else {
				if (zoomablePane.ZoomingByXEnabled)
					Zoom(primaryAxisX, xRatio, xZoomPercent);
				if (zoomablePane.ZoomingByYEnabled)
					Zoom(primaryAxisY, yRatio, yZoomPercent);
			}
		}
		void ResetZoom(IAxisData axis) {
			IWholeAxisRangeData wholeRange = axis.WholeRange;
			SetMinMax(axis, wholeRange.Min, wholeRange.Max);
		}
		void Zoom(IAxisData axis, double zoomPercent) {
			if (zoomPercent < 1.0 || CanZoomOutAxis(axis)) {
				IVisualAxisRangeData visualRange = axis.VisualRange;
				IWholeAxisRangeData wholeRange = axis.WholeRange;
				ITransformation transformation = axis.AxisScaleTypeMap.Transformation;
				double min = transformation.TransformForward(visualRange.Min);
				double max = transformation.TransformForward(visualRange.Max);
				double delta = max - min;
				double zoomSize = delta * zoomPercent;
				double newMin = transformation.TransformBackward(min - zoomSize);
				double newMax = transformation.TransformBackward(max + zoomSize);
				if (newMin < wholeRange.Min)
					newMin = wholeRange.Min;
				if (newMax > wholeRange.Max)
					newMax = wholeRange.Max;
				PerformZoom(axis, newMin, newMax);
			}
		}
		void Zoom(IAxisData axis, double position, double zoomPercent) {
			if (zoomPercent < 1.0 || CanZoomOutAxis(axis)) {
				IVisualAxisRangeData visualRange = axis.VisualRange;
				IWholeAxisRangeData wholeRange = axis.WholeRange;
				ITransformation transformation = axis.AxisScaleTypeMap.Transformation;
				double min = transformation.TransformForward(visualRange.Min);
				double max = transformation.TransformForward(visualRange.Max);
				double delta = max - min;
				double center = axis.Reverse ? (max - delta * position) : (min + delta * position);
				double halfDelta = delta * zoomPercent / 2;
				double newMin = transformation.TransformBackward(center - halfDelta);
				double newMax = transformation.TransformBackward(center + halfDelta);
				if (newMin < wholeRange.Min) {
					newMax += wholeRange.Min - newMin;
					newMin = wholeRange.Min;
					if (newMax > wholeRange.Max)
						newMax = wholeRange.Max;
				}
				else if (newMax > wholeRange.Max) {
					newMin -= newMax - wholeRange.Max;
					newMax = wholeRange.Max;
					if (newMin < wholeRange.Min)
						newMin = wholeRange.Min;
				}
				PerformZoom(axis, newMin, newMax);
			}
		}
		void ZoomIn(IAxisData axis, double minPercent, double maxPercent) {
			IVisualAxisRangeData visualRange = axis.VisualRange;
			ITransformation transformation = axis.AxisScaleTypeMap.Transformation;
			double min = transformation.TransformForward(visualRange.Min);
			double max = transformation.TransformForward(visualRange.Max);
			double delta = max - min;
			double newMin, newMax;
			if (axis.Reverse) {
				newMin = transformation.TransformBackward(max - delta * maxPercent);
				newMax = transformation.TransformBackward(max - delta * minPercent);
			}
			else {
				newMin = transformation.TransformBackward(min + delta * minPercent);
				newMax = transformation.TransformBackward(min + delta * maxPercent);
			}
			PerformZoom(axis, newMin, newMax);
		}
		void SetRange(IAxisData axis, double position1, double position2) {
			position1 = ClampPosition(position1);
			position2 = ClampPosition(position2);
			if (position2 < position1) {
				double temp = position1;
				position1 = position2;
				position2 = temp;
			}
			IWholeAxisRangeData wholeRange = axis.WholeRange;
			double delta = wholeRange.Delta;
			double newMin, newMax;
			if (axis.Reverse) {
				newMin = wholeRange.Max - delta * position2;
				newMax = wholeRange.Max - delta * position1;
			}
			else {
				newMin = wholeRange.Min + delta * position1;
				newMax = wholeRange.Min + delta * position2;
			}
			PerformZoom(axis, newMin, newMax);
		}
		void SetAxisZoom(IAxisData axis, double factor, NavigationType navigationType) {
			BeginZooming(ZoomingKind.ZoomPosistion);
			RangesSnapshot rangesDiapason = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
			ITransformation transformation = axis.AxisScaleTypeMap.Transformation;
			MinMaxValues visualRange = Transformation.TransformForward(axis.VisualRange, transformation);
			MinMaxValues wholeRange = Transformation.TransformForward(axis.WholeRange, transformation);
			double center = (visualRange.Min + visualRange.Max) / 2.0;
			double wholeRangeDelta = wholeRange.Max - wholeRange.Min;
			double minimumDelta = wholeRangeDelta / GetMaximumZoomPercent(axis);
			factor = factor > 1 ? 1 : factor;
			factor = factor < 0 ? 0 : factor;
			double visualRangeDelta = wholeRangeDelta - (wholeRangeDelta - minimumDelta) * factor;
			double newMin = transformation.TransformBackward(center - visualRangeDelta / 2.0);
			double newMax = transformation.TransformBackward(center + visualRangeDelta / 2.0);
			PerformZoom(axis, newMin, newMax);
			FinishZooming(rangesDiapason, navigationType);
		}
		void FinishScrolling(ScrollingOrientation scrollingOrientation, RangesSnapshot rangesDiapason, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null) {
				zoomablePane.RangeLimitsUpdated();
				rangesDiapason.CorrectSecondaryAxesPositions(totalSecondaryAxesX, zoomablePane.ScrollingByXEnabled, totalSecondaryAxesY, zoomablePane.ScrollingByYEnabled);
				zoomablePane.EndScrolling(scrollingOrientation, navigationType, rangesDiapason, new RangesSnapshot(pane, primaryAxisX, primaryAxisY));
			}
		}
		void BeginZooming(ZoomingKind zoomingKind) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null) {
				ZoomCacheEx cache = zoomablePane.ZoomCacheEx;
				if (cache != null)
					cache.Push(new ZoomCacheItemEx(this), zoomingKind);
				zoomablePane.BeginZooming();
			}
		}
		void FinishZooming(RangesSnapshot rangesSnapshot, NavigationType navigationType) {
			IZoomablePane zoomablePane = pane as IZoomablePane;
			if (zoomablePane != null) {
				zoomablePane.RangeLimitsUpdated();
				rangesSnapshot.CorrectSecondaryAxesPositions(totalSecondaryAxesX, zoomablePane.ScrollingByXEnabled || zoomablePane.ZoomingByXEnabled, totalSecondaryAxesY, zoomablePane.ScrollingByYEnabled || zoomablePane.ZoomingByYEnabled);
				var newRangesSnapshot = new RangesSnapshot(pane, primaryAxisX, primaryAxisY);
				zoomablePane.EndZooming(navigationType, rangesSnapshot, newRangesSnapshot);
			}
		}
	}
}
