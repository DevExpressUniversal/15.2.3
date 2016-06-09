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
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.Charts.Native {
	public struct CrosshairValueItem {
		double value;
		int pointIndex;
		public double Value { get { return value; } }
		public int PointIndex { get { return pointIndex; } }
		public CrosshairValueItem(double value, int index) {
			this.value = value;
			this.pointIndex = index;
		}
	}
	public class CrosshairSortedPointsInfoComparer : IComparer<CrosshairValueItem> {
		public int Compare(CrosshairValueItem item1, CrosshairValueItem item2) {
			return SortingUtils.CompareDoubles(item1.Value, item2.Value);
		}
	}
	public class CrosshairManager {
		public static MinMaxValues CalculateMinMaxMarkerSeriesRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair) {
			MinMaxValues minMaxRange = new MinMaxValues();
			if (isHorizontalCrosshair) {
				minMaxRange.Min = point.AnchorPoint.X - range;
				minMaxRange.Max = point.AnchorPoint.X + range;
			}
			else {
				minMaxRange.Min = point.AnchorPoint.Y - range;
				minMaxRange.Max = point.AnchorPoint.Y + range;
			}
			return minMaxRange;
		}
		public static MinMaxValues CalculateMinMaxContinuousSeriesRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, CrosshairPaneInfoEx crosshairPaneInfo, CrosshairSnapModeCore snapMode) {
			MinMaxValues minMaxRange = new MinMaxValues(-1, -1);
			int sortedPointsCount = point.RefinedSeries.Points.Count;
			if (sortedPointsCount > 0) {
				GRealPoint2D pointCoords = point.AnchorPoint;
				double minBoundsValue = isHorizontalCrosshair ? crosshairPaneInfo.MappingBounds.Left : crosshairPaneInfo.MappingBounds.Top;
				double maxBoundsValue = isHorizontalCrosshair ? crosshairPaneInfo.MappingBounds.Right : crosshairPaneInfo.MappingBounds.Bottom;
				IAxisData primaryAxis = snapMode == CrosshairSnapModeCore.NearestArgument ? point.AxisXProjection.Axis : point.AxisYProjection.Axis;
				if (sortedPointsCount > 1) {
					switch (point.Position) {
						case PointPositionInSeries.Left:
							if (isHorizontalCrosshair) {
								minMaxRange.Min = !primaryAxis.Reverse ? pointCoords.X - range : minBoundsValue;
								minMaxRange.Max = !primaryAxis.Reverse ? maxBoundsValue : pointCoords.X + range;
							}
							else {
								minMaxRange.Min = !primaryAxis.Reverse ? minBoundsValue : pointCoords.Y - range;
								minMaxRange.Max = !primaryAxis.Reverse ? pointCoords.Y + range : maxBoundsValue;
							}
							break;
						case PointPositionInSeries.Center:
							minMaxRange.Min = minBoundsValue;
							minMaxRange.Max = maxBoundsValue;
							break;
						case PointPositionInSeries.Right:
							if (isHorizontalCrosshair) {
								minMaxRange.Min = !primaryAxis.Reverse ? minBoundsValue : pointCoords.X - range;
								minMaxRange.Max = !primaryAxis.Reverse ? pointCoords.X + range : maxBoundsValue;
							}
							else {
								minMaxRange.Min = !primaryAxis.Reverse ? pointCoords.Y - range : minBoundsValue;
								minMaxRange.Max = !primaryAxis.Reverse ? maxBoundsValue : pointCoords.Y + range;
							}
							break;
					}
				}
				else {
					if (isHorizontalCrosshair) {
						minMaxRange.Min = pointCoords.X - range;
						minMaxRange.Max = pointCoords.X + range;
					}
					else {
						minMaxRange.Min = pointCoords.Y - range;
						minMaxRange.Max = pointCoords.Y + range;
					}
				}
			}
			return minMaxRange;
		}
		public static MinMaxValues CalculateMinMaxBarRangeValues(CrosshairSeriesPointEx point, double range, bool isHorizontalCrosshair, IXYDiagram diagram, IPane pane, CrosshairSnapModeCore snapMode) {
			GRealPoint2D pointCoords = point.AnchorPoint;
			MinMaxValues minMaxValues = new MinMaxValues();
			if (snapMode == CrosshairSnapModeCore.NearestArgument) {
				double pointInternalArgument = point.RefinedPoint.Argument;
				IAxisData axisX = point.AxisXProjection.Axis;
				IAxisData axisY = point.AxisYProjection.Axis;
				ISideBySidePoint refinedPoint = (RefinedPoint)point.RefinedPoint;
				double barWidth = refinedPoint.BarWidth != 0 && !Double.IsNaN(refinedPoint.BarWidth) ? refinedPoint.BarWidth : ((IBarSeriesView)point.View).BarWidth;
				double pointOffset = !Double.IsNaN(refinedPoint.Offset) ? refinedPoint.Offset : 0.0;
				double pointValue = pointInternalArgument + pointOffset;
				GRealPoint2D minBarPoint = diagram.MapInternalToPoint(pane, axisX, axisY, pointValue - barWidth * 0.5, 0.0);
				GRealPoint2D maxBarPoint = diagram.MapInternalToPoint(pane, axisX, axisY, pointValue + barWidth * 0.5, 0.0);
				double offset = axisX.Reverse ^ !diagram.Rotated ? refinedPoint.FixedOffset : -refinedPoint.FixedOffset;
				if (isHorizontalCrosshair) {
					double minBarX = minBarPoint.X < maxBarPoint.X ? minBarPoint.X : maxBarPoint.X;
					double maxBarX = minBarPoint.X < maxBarPoint.X ? maxBarPoint.X : minBarPoint.X;
					minMaxValues.Min = Math.Min(minBarX + offset, pointCoords.X - range);
					minMaxValues.Max = Math.Max(maxBarX + offset, pointCoords.X + range);
				}
				else {
					double minBarY = minBarPoint.Y < maxBarPoint.Y ? minBarPoint.Y : maxBarPoint.Y;
					double maxBarY = minBarPoint.Y < maxBarPoint.Y ? maxBarPoint.Y : minBarPoint.Y;
					minMaxValues.Min = Math.Min(minBarY + offset, pointCoords.Y - range);
					minMaxValues.Max = Math.Max(maxBarY + offset, pointCoords.Y + range);
				}
			}
			else
				if (isHorizontalCrosshair) {
					minMaxValues.Min = pointCoords.X - range;
					minMaxValues.Max = pointCoords.X + range;
				}
				else {
					minMaxValues.Min = pointCoords.Y - range;
					minMaxValues.Max = pointCoords.Y + range;
				}
			return minMaxValues;
		}
		const double crosshairRange = 0.16;
		const int crosshairPixelsRange = 20;
		readonly IXYDiagram xyDiagram;
		IList<PaneAxesContainer> paneAxisRepositories;
		List<CrosshairSeriesData> crosshairDataList = new List<CrosshairSeriesData>();
		bool IsHorizontalCrosshair {
			get {
				return xyDiagram.Rotated ^ xyDiagram.CrosshairOptions.SnapMode != CrosshairSnapModeCore.NearestValue;
			}
		}
		ICrosshairFreePosition LabelPosition {
			get {
				return xyDiagram.CrosshairOptions.LabelPosition;
			}
		}
		bool SnapToArgument { get { return CrosshairOptions.SnapMode == CrosshairSnapModeCore.NearestArgument; } }
		ICrosshairOptions CrosshairOptions { get { return xyDiagram.CrosshairOptions; } }
		public CrosshairManager(IXYDiagram diagram) {
			xyDiagram = diagram;
		}
		CrosshairSeriesData GetCrosshairData(IRefinedSeries refinedSeries) {
			foreach (var crosshairData in crosshairDataList) {
				if (crosshairData.Series == refinedSeries)
					return crosshairData;
			}
			return null;
		}
		List<CrosshairSeriesPointEx> CalcualteCrosshairPoints(CrosshairPaneInfoEx crosshairPaneInfo, IList<IRefinedSeries> refinedSeries) {
			List<CrosshairSeriesPointEx> crosshairSeriesPoints = new List<CrosshairSeriesPointEx>();
			List<IAxisData> qualitativeAxes = new List<IAxisData>();
			foreach (var series in refinedSeries) {
				bool hasCrosshairPointForSeries = false;
				foreach (var point in crosshairSeriesPoints) {
					if (point.RefinedSeries == series){
						hasCrosshairPointForSeries = true;
						break;
					}
				}
				if (hasCrosshairPointForSeries)
					continue;
				CrosshairSeriesData crosshairSeriesData = GetCrosshairData(series);
				if (crosshairSeriesData != null) {
					List<CrosshairSeriesPointEx> points = GetCrosshairPoints(series, crosshairSeriesData, crosshairPaneInfo);
					crosshairSeriesPoints.AddRange(points);
					foreach (CrosshairSeriesPointEx point in points)
						if (point.AxisXProjection.Axis.AxisScaleTypeMap is AxisQualitativeMap && !qualitativeAxes.Contains(point.AxisXProjection.Axis))
							qualitativeAxes.Add(point.AxisXProjection.Axis);
				}
			}
			if (CrosshairOptions.LabelMode == CrosshairLabelModeCore.ShowForNearestSeries) {
				GRealPoint2D crosshairLocation = crosshairPaneInfo.CursorLocation;
				crosshairSeriesPoints = CalculateNearestPoint(crosshairSeriesPoints, crosshairLocation);
			}
			if (CrosshairOptions.LabelMode == CrosshairLabelModeCore.ShowCommonForAllSeries && qualitativeAxes.Count > 0) {
				List<CrosshairSeriesPointEx> removedPoints = FindDifferentPoints(qualitativeAxes, crosshairPaneInfo, crosshairSeriesPoints);
				foreach (CrosshairSeriesPointEx pointToRemove in removedPoints)
					crosshairSeriesPoints.Remove(pointToRemove);
			}
			return crosshairSeriesPoints;
		}
		List<CrosshairSeriesPointEx> GetCrosshairPoints(IRefinedSeries refinedSeries, CrosshairSeriesData crosshairSeriesData, CrosshairPaneInfoEx crosshairPaneInfo) {
			List<CrosshairSeriesPointEx> crosshairPoints = new List<CrosshairSeriesPointEx>();
			CrosshairSeriesPointEx crosshairSeriesPoint = FindCrosshairPoint(refinedSeries, crosshairSeriesData, crosshairPaneInfo);
			if (crosshairSeriesPoint != null && IsPointInCrosshairRange(crosshairSeriesPoint, crosshairPaneInfo, refinedSeries.SeriesView))
				crosshairPoints = CalculatePointGroups(crosshairSeriesPoint, refinedSeries, crosshairPaneInfo);
			return crosshairPoints;
		}
		List<CrosshairSeriesPointEx> CalculatePointGroups(CrosshairSeriesPointEx crosshairPoint, IRefinedSeries refinedSeries, CrosshairPaneInfoEx crosshairPaneInfo) {
			List<CrosshairSeriesPointEx> pointGroup = new List<CrosshairSeriesPointEx>();
			pointGroup.Add(crosshairPoint);
			if ((refinedSeries.SeriesView is IBarSeriesView) && (CrosshairOptions.LabelMode != CrosshairLabelModeCore.ShowForNearestSeries)) {
				List<CrosshairSeriesPointEx> connectedPoints = GetConnectedPoints(crosshairPoint, crosshairPaneInfo);
				pointGroup.AddRange(connectedPoints);
				CrosshairPointComparer comparer = new CrosshairPointComparer(IsHorizontalCrosshair);
				pointGroup.Sort(comparer);
			}
			return pointGroup;
		}
		List<CrosshairSeriesPointEx> GetConnectedPoints(CrosshairSeriesPointEx point, CrosshairPaneInfoEx crosshairPaneInfo) {
			List<CrosshairSeriesPointEx> connectedPoints = new List<CrosshairSeriesPointEx>();
			if (!SnapToArgument)
				return connectedPoints;
			List<RefinedSeries> sideBySideSeries = point.RefinedPoint.GetSideBySideSeries();
			if (sideBySideSeries != null) {
				foreach (var refinedSeries in sideBySideSeries) {
					IXYSeriesView xyView = refinedSeries.SeriesView as IXYSeriesView;
					if (xyView == null || !xyView.CrosshairEnabled || point.RefinedSeries == refinedSeries)
						continue;
					int index;
					RefinedPoint findedPoint = refinedSeries.GetFinalPointByArgument(point.RefinedPoint.Argument, out index);
					if (findedPoint != null) {
						double pointValue = findedPoint.Value1;
						foreach (double value in xyView.GetCrosshairValues(findedPoint)) {
							pointValue = value;
							break;
						}
						connectedPoints.Add(CreateCrosshairSeriesPoint(xyView, findedPoint.Argument, pointValue, crosshairPaneInfo, refinedSeries, findedPoint, PointPositionInSeries.Center, index));
					}
				}
			}
			return connectedPoints;
		}
		List<CrosshairSeriesPointEx> FindDifferentPoints(List<IAxisData> qualitativeAxes, CrosshairPaneInfoEx crosshairPaneInfo, List<CrosshairSeriesPointEx> crosshairSeriesPoints) {
			List<CrosshairSeriesPointEx> differentPoints = new List<CrosshairSeriesPointEx>();
			foreach (IAxisData axis in qualitativeAxes) {
				object crosshairAxisValue = null;
				foreach (PointProjectionOnAxis projection in crosshairPaneInfo.CursorCoords.AxesXValues)
					if (projection.Axis == axis)
						crosshairAxisValue = projection.NativeValue;
				if (crosshairAxisValue != null)
					foreach (CrosshairSeriesPointEx point in crosshairSeriesPoints)
						if (point.AxisXProjection.Axis == axis && point.AxisXProjection.NativeValue != crosshairAxisValue)
							differentPoints.Add(point);
			}
			return differentPoints;
		}
		CrosshairSeriesPointEx FindCrosshairPoint(IRefinedSeries refinedSeries, CrosshairSeriesData crosshairSeriesData, CrosshairPaneInfoEx crosshairPaneInfo) {
			PointProjectionsForPane crosshairCoords = crosshairPaneInfo.CursorCoords;
			IXYSeriesView view = refinedSeries.SeriesView as IXYSeriesView;
			if (view == null || !view.CrosshairEnabled || !Object.ReferenceEquals(view.Pane, crosshairPaneInfo.Pane))
				return null;
			double crosshairArgument = crosshairCoords.GetArgumentByAxisX(view.AxisXData);
			double crosshairValue = crosshairCoords.GetValueByAxisY(view.AxisYData);
			if (double.IsNaN(crosshairArgument) || double.IsNaN(crosshairValue))
				return null;
			double pointArgument, pointValue;
			PointPositionInSeries pointPosition;
			int pointIndex;
			RefinedPoint nearestPointInfo = crosshairSeriesData.GetCrosshairPoint(crosshairArgument, crosshairValue, out pointArgument, out pointValue, out pointPosition, out pointIndex);
			if (nearestPointInfo != null && !nearestPointInfo.IsEmpty)
				return CreateCrosshairSeriesPoint(view, pointArgument, pointValue, crosshairPaneInfo, refinedSeries, nearestPointInfo, pointPosition, pointIndex);
			return null;
		}
		CrosshairSeriesPointEx CreateCrosshairSeriesPoint(IXYSeriesView seriesView, double pointArgument, double pointValue,
			CrosshairPaneInfoEx crosshairPaneInfo, IRefinedSeries refinedSeries, ISideBySidePoint nearestPointInfo, PointPositionInSeries pointPosition, int pointIndex) {
			PointProjectionOnAxis pointArgumentPair = new PointProjectionOnAxis(seriesView.AxisXData, pointArgument);
			PointProjectionOnAxis pointValuePair = new PointProjectionOnAxis(seriesView.AxisYData, pointValue);
			double offset = double.IsNaN(nearestPointInfo.Offset) ? 0 : nearestPointInfo.Offset;
			double fixedOffset = double.IsNaN(nearestPointInfo.FixedOffset) ? 0 : nearestPointInfo.FixedOffset;
			GRealPoint2D coords = xyDiagram.MapInternalToPoint(crosshairPaneInfo.Pane, pointArgumentPair.Axis, pointValuePair.Axis, pointArgumentPair.Value + offset, pointValuePair.Value);
			GRealPoint2D anchorPoint = xyDiagram.Rotated ? new GRealPoint2D(coords.X, coords.Y + fixedOffset) : new GRealPoint2D(coords.X + fixedOffset, coords.Y);
			return new CrosshairSeriesPointEx(pointArgumentPair, pointValuePair, anchorPoint, (RefinedPoint)nearestPointInfo, refinedSeries, pointPosition, pointIndex);
		}
		bool IsPointInCrosshairRange(CrosshairSeriesPointEx point, CrosshairPaneInfoEx crosshairPaneInfo, ISeriesView seriesView) {
			GRealRect2D mappingBounds = crosshairPaneInfo.MappingBounds;
			GRealPoint2D cursorLocation = crosshairPaneInfo.CursorLocation;
			double range = crosshairRange * (IsHorizontalCrosshair ? mappingBounds.Width : mappingBounds.Height);
			range = Math.Min(range, crosshairPixelsRange);
			if (point != null) {
				if (!mappingBounds.Contains(point.AnchorPoint))
					return false;
				MinMaxValues pointRange = seriesView.CalculateMinMaxPointRangeValues(point, range, IsHorizontalCrosshair, xyDiagram, crosshairPaneInfo, CrosshairOptions.SnapMode);
				if (IsHorizontalCrosshair) {
					if (cursorLocation.X >= pointRange.Min && cursorLocation.X <= pointRange.Max)
						return true;
				}
				else
					if (cursorLocation.Y >= pointRange.Min && cursorLocation.Y <= pointRange.Max)
						return true;
			}
			return false;
		}
		List<CrosshairSeriesPointEx> CalculateNearestPoint(List<CrosshairSeriesPointEx> crosshairSeriesPoints, GRealPoint2D crosshairLocation) {
			List<CrosshairSeriesPointEx> nearestPointList = crosshairSeriesPoints;
			CrosshairSeriesPointEx nearestPoint = null;
			foreach (CrosshairSeriesPointEx point in crosshairSeriesPoints)
				if (nearestPoint == null || GeometricUtils.CalcDistance(crosshairLocation, point.AnchorPoint) < GeometricUtils.CalcDistance(crosshairLocation, nearestPoint.AnchorPoint))
					nearestPoint = point;
			if (nearestPoint != null)
				nearestPointList = new List<CrosshairSeriesPointEx>() { nearestPoint };
			return nearestPointList;
		}
		CrosshairInfoEx CreateCrosshairPaneInfos(GRealPoint2D crosshairLocation, IPane focusedPane, IXYDiagram diagram) {
			GRealRect2D focusedMappingBounds = focusedPane.MappingBounds.Value;
			CrosshairInfoEx crosshairInfo = new CrosshairInfoEx();
			List<IPane> crosshairPanes = new List<IPane>();
			if (diagram.CrosshairOptions.ShowOnlyInFocusedPane)
				crosshairPanes.Add(focusedPane);
			else
				crosshairPanes.AddRange(diagram.GetCrosshairSyncPanes(focusedPane, IsHorizontalCrosshair));
			for (int i = 0; i < crosshairPanes.Count; i++) {
				IPane crosshairPane = crosshairPanes[i];
				if (!crosshairPane.MappingBounds.HasValue)
					continue;
				GRealPoint2D cursorLocation = crosshairLocation;
				GRealRect2D mappingBounds = crosshairPane.MappingBounds.Value;
				if (!Object.ReferenceEquals(focusedPane, crosshairPane)) {
					if (IsHorizontalCrosshair) {
						double y = (mappingBounds.Top + mappingBounds.Bottom) / 2.0;
						cursorLocation = new GRealPoint2D(crosshairLocation.X, y);
					}
					else {
						double x = (mappingBounds.Left + mappingBounds.Right) / 2.0;
						cursorLocation = new GRealPoint2D(x, crosshairLocation.Y);
					}
				}
				PointProjectionsForPane cursorCoords = new PointProjectionsForPane(crosshairPane, diagram.MapPointToInternal(crosshairPane, cursorLocation));
				CrosshairPaneInfoEx crosshairPaneInfo = new CrosshairPaneInfoEx(crosshairPane, mappingBounds, cursorLocation, cursorCoords);
				if (crosshairPaneInfo != null)
					crosshairInfo.Add(crosshairPaneInfo);
			}
			return crosshairInfo;
		}
		void CalculateAxisValues(CrosshairPaneInfoEx crosshairPaneInfo, List<CrosshairSeriesPointEx> crosshairPoints, ICrosshairOptions options) {
			AddAxisValuesForCursorSnapAxes(crosshairPaneInfo, options);
			AddAxisValuesByPoints(crosshairPaneInfo, crosshairPoints, options);
		}
		void AddAxisValuesForCursorSnapAxes(CrosshairPaneInfoEx crosshairPaneInfo, ICrosshairOptions options) {
			bool snapToArguments = options.SnapMode == CrosshairSnapModeCore.NearestArgument;
			List<PointProjectionOnAxis> projectionCollection = GetAxisValueForCursorSnapAxes(crosshairPaneInfo.CursorCoords, crosshairPaneInfo.Pane, snapToArguments);
			foreach (PointProjectionOnAxis pointProjection in projectionCollection)
				crosshairPaneInfo.AddValue(pointProjection);
		}
		PaneAxesContainer GetPaneAxesContainer(IPane pane) {
			foreach (var repository in paneAxisRepositories) {
				if (repository.Pane == pane)
					return repository;
			}
			return null;
		}
		List<PointProjectionOnAxis> GetAxisValueForCursorSnapAxes(PointProjectionsForPane crosshairCoords, IPane pane, bool snapToArguments) {
			List<PointProjectionOnAxis> axisValuePairs = new List<PointProjectionOnAxis>();
			if (snapToArguments) {
				PaneAxesContainer container = GetPaneAxesContainer(pane);
				if (container != null)
					foreach (IAxisData axisX in container.AxesX)
						if (crosshairCoords.AxesXValues.Count > 0)
							axisValuePairs.Add(new PointProjectionOnAxis(axisX, crosshairCoords.GetArgumentByAxisX(axisX)));
			}
			else {
				PaneAxesContainer container = GetPaneAxesContainer(pane);
				if (container != null)
					foreach (IAxisData axisY in container.AxesY)
						axisValuePairs.Add(new PointProjectionOnAxis(axisY, crosshairCoords.GetValueByAxisY(axisY)));
			}
			return axisValuePairs;
		}
		void AddAxisValuesByPoints(CrosshairPaneInfoEx crosshairPaneInfo, List<CrosshairSeriesPointEx> crosshairPoints, ICrosshairOptions options) {
			bool snapToArguments = options.SnapMode == CrosshairSnapModeCore.NearestArgument;
			foreach (CrosshairSeriesPointEx point in crosshairPoints) {
				crosshairPaneInfo.AddSeriesPoint(point);
				if (snapToArguments)
					crosshairPaneInfo.AddValue(point.AxisYProjection);
				else
					crosshairPaneInfo.AddValue(point.AxisXProjection);
			}
		}
		Dictionary<PointProjectionOnAxis, GRealPoint2D> BindAxisValueAndScreenCoords(ICrosshairOptions options, CrosshairPaneInfoEx crosshairPaneInfo) {
			IPane pane = crosshairPaneInfo.Pane;
			IXYDiagram diagram = xyDiagram;
			PointProjectionsForPane cursorCoords = crosshairPaneInfo.CursorCoords;
			GRealPoint2D crosshairLocation = crosshairPaneInfo.CursorLocation;
			Dictionary<PointProjectionOnAxis, GRealPoint2D> screenCoords = new Dictionary<PointProjectionOnAxis, GRealPoint2D>();
			if (options.SnapMode == CrosshairSnapModeCore.NearestValue) {
				PaneAxesContainer container = GetPaneAxesContainer(pane);
				if (container != null) {
					IAxisData axisY = container.PrimaryAxisY;
					double axisYValue = cursorCoords.GetValueByAxisY(axisY);
					foreach (PointProjectionOnAxis axisValue in crosshairPaneInfo.AllAxesValues) {
						GRealPoint2D crosshairCoords = crosshairLocation;
						if (!axisValue.Axis.IsValueAxis)
							crosshairCoords = diagram.MapInternalToPoint(pane, axisValue.Axis, axisY, axisValue.Value, axisYValue);
						if (!screenCoords.ContainsKey(axisValue))
							screenCoords.Add(axisValue, crosshairCoords);
					}
				}
			}
			else {
				PaneAxesContainer container = GetPaneAxesContainer(pane);
				if (container != null) {
					IAxisData axisX = container.PrimaryAxisX;
					double axisXArgument = cursorCoords.GetArgumentByAxisX(axisX);
					foreach (PointProjectionOnAxis axisValue in crosshairPaneInfo.AllAxesValues) {
						GRealPoint2D crosshairCoords = crosshairLocation;
						if (axisValue.Axis.IsValueAxis)
							crosshairCoords = diagram.MapInternalToPoint(pane, axisX, axisValue.Axis, axisXArgument, axisValue.Value);
						if (!screenCoords.ContainsKey(axisValue))
							screenCoords.Add(axisValue, crosshairCoords);
					}
				}
			}
			return screenCoords;
		}
		void CalculateCrossLines(CrosshairPaneInfoEx crosshairPaneInfo, List<CrosshairSeriesPointEx> crosshairPoints, Dictionary<PointProjectionOnAxis, GRealPoint2D> screenCoords) {
			CrosshairLine crosshairLine;
			bool snapToArguments = CrosshairOptions.SnapMode == CrosshairSnapModeCore.NearestArgument;
			bool showArgumentLine = CrosshairOptions.ShowArgumentLine;
			bool showValueLine = CrosshairOptions.ShowValueLine;
			foreach (CrosshairSeriesPointEx point in crosshairPoints) {
				PointProjectionOnAxis axisValuePair = GetAxisValuePair(CrosshairOptions, point);
				crosshairLine = CreateCrossLine(screenCoords, crosshairPaneInfo.MappingBounds, axisValuePair);
				if (snapToArguments ? showValueLine : showArgumentLine)
					crosshairPaneInfo.AddCrossLine(crosshairLine);
				if (point != null && crosshairLine != null)
					point.CrosshairLine = crosshairLine;
			}
			List<PointProjectionOnAxis> CursorValues = GetAxisValueForCursorSnapAxes(crosshairPaneInfo.CursorCoords, crosshairPaneInfo.Pane, snapToArguments);
			foreach (PointProjectionOnAxis axisValuePair in CursorValues) {
				crosshairLine = CreateCrossLine(screenCoords, crosshairPaneInfo.MappingBounds, axisValuePair);
				if (snapToArguments ? showArgumentLine : showValueLine)
					crosshairPaneInfo.AddCrossLine(crosshairLine);
				if (crosshairPaneInfo.CursorCrossLine == null)
					crosshairPaneInfo.CursorCrossLine = crosshairLine;
			}
		}
		void CalculateAxisLabels(CrosshairPaneInfoEx crosshairPaneInfo, List<CrosshairSeriesPointEx> crosshairPoints,  Dictionary<PointProjectionOnAxis, GRealPoint2D> screenCoords) {
			foreach (CrosshairSeriesPointEx point in crosshairPoints) {
				PointProjectionOnAxis axisValuePair = GetAxisValuePair(CrosshairOptions, point);
				CrosshairAxisInfo crosshairAxisInfo = AddCrosshairAxisInfo(crosshairPaneInfo, screenCoords, axisValuePair);
				if (crosshairAxisInfo != null && point != null)
					point.CrosshairAxisInfo = crosshairAxisInfo;
				if (crosshairAxisInfo != null && IsAxisLableVisible(axisValuePair))
					crosshairPaneInfo.AddAxisLabel(crosshairAxisInfo);
			}
			List<PointProjectionOnAxis> CursorValues = GetAxisValueForCursorSnapAxes(crosshairPaneInfo.CursorCoords, crosshairPaneInfo.Pane, CrosshairOptions.SnapMode == CrosshairSnapModeCore.NearestArgument);
			foreach (PointProjectionOnAxis axisValuePair in CursorValues) {
				CrosshairAxisInfo crosshairAxisInfo = AddCrosshairAxisInfo(crosshairPaneInfo, screenCoords, axisValuePair);
				if (crosshairAxisInfo != null && IsAxisLableVisible(axisValuePair)) {
					crosshairPaneInfo.CursorAxesInfo.Add(crosshairAxisInfo);
					crosshairPaneInfo.AddAxisLabel(crosshairAxisInfo);
				}
			}
		}
		void CreateCrosshairLabels(GRealRect2D labelsConstraint, IPane focusedPane, ICrosshairOptions options, CrosshairInfoEx crosshairInfo) {
			if (options.LabelMode == CrosshairLabelModeCore.ShowCommonForAllSeries) {
				List<CrosshairSeriesPointEx> commonSeriesPoints = new List<CrosshairSeriesPointEx>();
				foreach (CrosshairPaneInfoEx crosshairPaneInfo in crosshairInfo)
					commonSeriesPoints.AddRange(crosshairPaneInfo.SeriesPoints);
				CrosshairPaneInfoEx focusedPaneInfo = crosshairInfo.GetByPane(focusedPane);
				if (!LabelPosition.IsMousePosition)
					CompleteCrosshairFreeLabelsInfo(focusedPaneInfo, commonSeriesPoints, LabelPosition, options);
				else {
					focusedPaneInfo.LabelsConstraint = labelsConstraint;
					focusedPaneInfo.AddCrosshairLabel(commonSeriesPoints, focusedPaneInfo.CursorLocation, AnnotationLocation.TopRight, false, LabelPosition.Offset, options);
				}
			}
			else {
				foreach (CrosshairPaneInfoEx crosshairPaneInfo in crosshairInfo) {
					crosshairPaneInfo.LabelsConstraint = labelsConstraint;
					foreach (CrosshairSeriesPointEx point in crosshairPaneInfo.SeriesPoints)
						crosshairPaneInfo.AddCrosshairLabel(point, options);
				}
			}
		}
		PointProjectionOnAxis GetAxisValuePair(ICrosshairOptions options, CrosshairSeriesPointEx point) {
			bool snapToArguments = options.SnapMode == CrosshairSnapModeCore.NearestArgument;
			bool showArgumentLine = options.ShowArgumentLine;
			bool showValueLine = options.ShowValueLine;
			return snapToArguments ? point.AxisYProjection : point.AxisXProjection;
		}
		CrosshairAxisInfo AddCrosshairAxisInfo(CrosshairPaneInfoEx crosshairPaneInfo, Dictionary<PointProjectionOnAxis, GRealPoint2D> screenCoords, PointProjectionOnAxis axisValue) {
			GRealRect2D labelBounds = axisValue.Axis.GetLabelBounds(crosshairPaneInfo.Pane);
			GRealPoint2D anchorPoint = axisValue.Axis.IsVertical ? new GRealPoint2D((labelBounds.Left + labelBounds.Right) / 2, screenCoords[axisValue].Y) :
				new GRealPoint2D(screenCoords[axisValue].X, (labelBounds.Top + labelBounds.Bottom) / 2);
			CrosshairAxisInfo crosshairAxisInfo = new CrosshairAxisInfo(CrosshairPaneInfoEx.ConstructAxisText(axisValue), anchorPoint, new GRealSize2D(labelBounds.Width, labelBounds.Height), axisValue.Axis, axisValue.NativeValue);
			return crosshairAxisInfo;
		}
		bool IsAxisLableVisible(PointProjectionOnAxis axisValuePair) {
			return ((ICrosshairAxis)axisValuePair.Axis).LabelVisible;
		}
		CrosshairLine CreateCrossLine(Dictionary<PointProjectionOnAxis, GRealPoint2D> screenCoords, GRealRect2D mappingBounds, PointProjectionOnAxis axisValue) {
			CrosshairLine crosshairLine;
			if (axisValue.Axis.IsVertical) {
				double y = screenCoords[axisValue].Y;
				crosshairLine = CreateCrossLine(mappingBounds.Left, y, mappingBounds.Right, y, axisValue.Axis.IsValueAxis);
			}
			else {
				double x = screenCoords[axisValue].X;
				crosshairLine = CreateCrossLine(x, mappingBounds.Top, x, mappingBounds.Bottom, axisValue.Axis.IsValueAxis);
			}
			return crosshairLine;
		}
		CrosshairLine CreateCrossLine(double startX, double startY, double endX, double endY, bool isValueLine) {
			return new CrosshairLine(new GRealLine2D(new GRealPoint2D(startX, startY), new GRealPoint2D(endX, endY)), isValueLine);
		}
		void CompleteCrosshairFreeLabelsInfo(CrosshairPaneInfoEx crosshairPaneInfo, List<CrosshairSeriesPointEx> seriesPoints, ICrosshairFreePosition labelPosition, ICrosshairOptions crosshairOptions) {
			GRealRect2D bounds = labelPosition.DockBounds;
			if (bounds.IsEmpty)
				bounds = xyDiagram.ChartBounds;
			GRealPoint2D anchorPoint = new GRealPoint2D();
			switch (labelPosition.DockCorner) {
				case DockCornerCore.TopLeft:
					anchorPoint = new GRealPoint2D(bounds.Left, bounds.Top);
					break;
				case DockCornerCore.TopRight:
					anchorPoint = new GRealPoint2D(bounds.Right, bounds.Top);
					break;
				case DockCornerCore.BottomLeft:
					anchorPoint = new GRealPoint2D(bounds.Left, bounds.Bottom);
					break;
				case DockCornerCore.BottomRight:
					anchorPoint = new GRealPoint2D(bounds.Right, bounds.Bottom);
					break;
				default:
					ChartDebug.Fail("Unexpected dock position");
					break;
			}
			AnnotationLocation annotationLocation = AnnotationLocation.BottomRight;
			switch (labelPosition.DockCorner) {
				case DockCornerCore.TopLeft:
					annotationLocation = AnnotationLocation.BottomRight;
					break;
				case DockCornerCore.TopRight:
					annotationLocation = AnnotationLocation.BottomLeft;
					break;
				case DockCornerCore.BottomLeft:
					annotationLocation = AnnotationLocation.TopRight;
					break;
				case DockCornerCore.BottomRight:
					annotationLocation = AnnotationLocation.TopLeft;
					break;
				default:
					ChartDebug.Fail("Unexpected dock position");
					break;
			}
			crosshairPaneInfo.AddCrosshairLabel(seriesPoints, anchorPoint, annotationLocation, false, labelPosition.Offset, crosshairOptions);
		}
		public CrosshairInfoEx CalculateCrosshairInfo(GRealPoint2D crosshairLocation, GRealRect2D labelsConstraint, IList<PaneAxesContainer> paneAxisRepositories, IPane focusedPane, IList<IRefinedSeries> refinedSeries) {
			this.paneAxisRepositories = paneAxisRepositories;
			if (CrosshairOptions == null || focusedPane == null)
				return null;
			if (!focusedPane.MappingBounds.HasValue)
				return null;
			CrosshairInfoEx crosshairInfo = CreateCrosshairPaneInfos(crosshairLocation, focusedPane, xyDiagram);
			Dictionary<PointProjectionOnAxis, GRealPoint2D> screenCoords;
			foreach (CrosshairPaneInfoEx crosshairPaneInfo in crosshairInfo) {
				List<CrosshairSeriesPointEx> crosshairPoints = CalcualteCrosshairPoints(crosshairPaneInfo, refinedSeries);
				CalculateAxisValues(crosshairPaneInfo, crosshairPoints, CrosshairOptions);
				screenCoords = BindAxisValueAndScreenCoords(CrosshairOptions, crosshairPaneInfo);
				CalculateCrossLines(crosshairPaneInfo, crosshairPoints, screenCoords);
				CalculateAxisLabels(crosshairPaneInfo, crosshairPoints, screenCoords);
			}
			CreateCrosshairLabels(labelsConstraint, focusedPane, CrosshairOptions, crosshairInfo);
			return crosshairInfo;
		}
		public void UpdateCrosshairData(IList<RefinedSeries> seriesCollection) {
			crosshairDataList.Clear();
			foreach (var refinedSeries in seriesCollection)
				if (refinedSeries.Series.ShouldBeDrawnOnDiagram)
					crosshairDataList.Add(new CrosshairSeriesData(xyDiagram, refinedSeries, ((IRefinedSeries)refinedSeries).Points));
		}
		public List<CrosshairValueItem> GetCrosshairSortedData(IRefinedSeries refinedSeries) {
			foreach (var seriesInfo in crosshairDataList) {
				if (seriesInfo.Series == refinedSeries)
					return seriesInfo.CrosshairSortedData;
			}
			return null;
		}
	}
	public class CrosshairSeriesData {
		static readonly IComparer<RefinedPoint> comparer = new RefinedPointArgumentComparer();
		static readonly CrosshairSortedPointsInfoComparer crosshairComparer = new CrosshairSortedPointsInfoComparer();
		readonly IXYDiagram diagram;
		readonly IXYSeriesView seriesView;
		readonly IRefinedSeries series;
		readonly List<CrosshairValueItem> crosshairSortedData;
		IList<RefinedPoint> pointsInfo;
		bool ShowCrosshair {
			get {
				return seriesView != null && seriesView.CrosshairEnabled && diagram != null && diagram.CrosshairOptions != null;
			}
		}
		bool SnapToArgument { get { return diagram.CrosshairOptions.SnapMode == CrosshairSnapModeCore.NearestArgument; } }
		public IRefinedSeries Series { get { return series; } }
		public IList<RefinedPoint> PointsInfo { get { return pointsInfo; } }
		public List<CrosshairValueItem> CrosshairSortedData { get { return crosshairSortedData; } }
		public CrosshairSeriesData(IXYDiagram diagram, IRefinedSeries series, IList<RefinedPoint> pointsInfo) {
			this.diagram = diagram;
			this.series = series;
			this.pointsInfo = pointsInfo;
			this.seriesView = series.SeriesView as IXYSeriesView;
			if (ShowCrosshair) {
				crosshairSortedData = new List<CrosshairValueItem>();
				FillCrosshairSortedData();
			}
		}
		bool IsValueInScaleBreak(IEnumerable<IScaleBreak> scaleBreaks, double value) {
			if (scaleBreaks is IList && ((IList)scaleBreaks).Count == 0)
				return false;
			IXYDiagram xYDiagram = diagram as IXYDiagram;
			bool scrollingEnabled = xYDiagram != null ? xYDiagram.ScrollingEnabled : false;
			if (scaleBreaks != null && !scrollingEnabled) {
				foreach (IScaleBreak scaleBreak in scaleBreaks) {
					if (scaleBreak.Visible && ((scaleBreak.Edge1.GetValue() < value && value < scaleBreak.Edge2.GetValue()) ||
						(scaleBreak.Edge2.GetValue() < value && value < scaleBreak.Edge1.GetValue())))
						return true;
				}
			}
			return false;
		}
		void FillCrosshairSortedData() {
			if (seriesView == null) {
				ChartDebug.Fail("The IXYSeriesView interface is not implemented.");
				return;
			}
			if (seriesView.AxisXData != null && seriesView.AxisYData != null) {
				IEnumerable<IScaleBreak> argumentScaleBreaks = seriesView.AxisXData.ScaleBreaks;
				IEnumerable<IScaleBreak> valueScaleBreaks = seriesView.AxisYData.ScaleBreaks;
				bool snapToArgument = SnapToArgument;
				for (int i = 0; i < pointsInfo.Count; i++) {
					RefinedPoint pointInfo = pointsInfo[i];
					if (IsValueInScaleBreak(argumentScaleBreaks, pointInfo.Argument))
						continue;
					IEnumerable<double> crosshairValues = seriesView.GetCrosshairValues(pointInfo);
					if (snapToArgument) {
						bool hasVisibleValue = false;
						if (crosshairValues != null)
							foreach (double value in crosshairValues) {
								hasVisibleValue = !IsValueInScaleBreak(valueScaleBreaks, value);
								if (hasVisibleValue)
									break;
							}
						if (hasVisibleValue)
							crosshairSortedData.Add(new CrosshairValueItem(pointInfo.Argument, i));
					}
					else {
						if (crosshairValues != null)
							foreach (double value in crosshairValues) {
								if (!IsValueInScaleBreak(valueScaleBreaks, value))
									crosshairSortedData.Add(new CrosshairValueItem(value, i));
							}
					}
				}
				crosshairSortedData.Sort(crosshairComparer);
			}
		}
		int CheckPointsWithSameValue(int crosshairValueItemIndex, double cursorPoint, out double nearestValue) {
			int neighbourValueItemIndex = crosshairValueItemIndex;
			double distance = GetMinDistance(cursorPoint, crosshairValueItemIndex, out nearestValue);
			double currentValue = crosshairSortedData[crosshairValueItemIndex].Value;
			int nextIndex = crosshairValueItemIndex + 1;
			while (nextIndex < crosshairSortedData.Count) {
				if (crosshairSortedData[nextIndex].Value != currentValue)
					break;
				double value;
				double nextDistance = GetMinDistance(cursorPoint, nextIndex, out value);
				if (nextDistance < distance) {
					neighbourValueItemIndex = nextIndex;
					nearestValue = value;
					distance = nextDistance;
				}
				nextIndex++;
			}
			int previosIndex = crosshairValueItemIndex - 1;
			while (previosIndex >= 0) {
				if (crosshairSortedData[previosIndex].Value != currentValue)
					break;
				double value;
				double previousDistance = GetMinDistance(cursorPoint, previosIndex, out value);
				if (previousDistance < distance) {
					neighbourValueItemIndex = previosIndex;
					nearestValue = value;
					distance = previousDistance;
				}
				previosIndex--;
			}
			return neighbourValueItemIndex;
		}
		double GetMinDistance(double cursorPoint, int crosshairValueItemIndex, out double value) {
			if (SnapToArgument)
				return GetMinValueDistance(cursorPoint, crosshairValueItemIndex, out value);
			return GetMinArgumentDistance(cursorPoint, crosshairValueItemIndex, out value);
		}
		double GetMinValueDistance(double value, int crosshairValueItemIndex, out double pointValue) {
			pointValue = double.NaN;
			int pointInfoIndex = crosshairSortedData[crosshairValueItemIndex].PointIndex;
			double distance = double.NaN;
			if (seriesView == null)
				return double.NaN;
			IEnumerable<IScaleBreak> valueScaleBreaks = seriesView.AxisYData.ScaleBreaks;
			foreach (double valueLevel in seriesView.GetCrosshairValues(pointsInfo[pointInfoIndex])) {
				if (IsValueInScaleBreak(valueScaleBreaks, valueLevel))
					continue;
				if (double.IsNaN(distance)) {
					pointValue = valueLevel;
					distance = Math.Abs(pointValue - value);
				}
				else {
					double distanceForOtherValue = Math.Abs(valueLevel - value);
					if (distanceForOtherValue < distance) {
						distance = distanceForOtherValue;
						pointValue = valueLevel;
					}
				}
			}
			return distance;
		}
		double GetMinArgumentDistance(double argument, int crosshairValueItemIndex, out double pointArgument) {
			int pointInfoIndex = crosshairSortedData[crosshairValueItemIndex].PointIndex;
			pointArgument = pointsInfo[pointInfoIndex].Argument;
			return Math.Abs(pointArgument - argument);
		}
		int CorrectCrossahirPointIndex(int pointIndex, double delta1, double delta2) {
			IStepSeriesView stepView = seriesView as IStepSeriesView;
			if (stepView == null) {
				if (delta2 < delta1)
					return pointIndex - 1;
			}
			else
				if (!stepView.InvertedStep && pointIndex > 0)
					return pointIndex - 1;
			return pointIndex;
		} 
		PointPositionInSeries CalculatePointPositionInSeries(int pointIndex) {
			if (pointIndex == 0 ||
				pointIndex - 1 >= 0 && pointsInfo[crosshairSortedData[pointIndex - 1].PointIndex].IsEmpty)
				return PointPositionInSeries.Left;
			if (pointIndex == crosshairSortedData.Count - 1 ||
				pointIndex + 1 < crosshairSortedData.Count && pointsInfo[crosshairSortedData[pointIndex + 1].PointIndex].IsEmpty)
				return PointPositionInSeries.Right;
			return PointPositionInSeries.Center;
		}
		public RefinedPoint GetCrosshairPoint(double argument, double value, out double pointArgument, out double pointValue, out PointPositionInSeries position, out int pointIndex) {
			pointArgument = double.NaN;
			pointValue = double.NaN;
			position = PointPositionInSeries.Center;
			pointIndex = -1;
			if (crosshairSortedData.Count == 0)
				return null;
			double cursorValue = SnapToArgument ? argument : value;
			CrosshairValueItem cursorPoint = new CrosshairValueItem(cursorValue, 0);
			int valueItemIndex = crosshairSortedData.BinarySearch(cursorPoint, crosshairComparer);
			if (valueItemIndex < 0) {
				valueItemIndex = ~valueItemIndex;
				if (valueItemIndex != 0) {
					if (valueItemIndex == crosshairSortedData.Count)
						valueItemIndex = crosshairSortedData.Count - 1;
					else {
						double delta1 = crosshairSortedData[valueItemIndex].Value - cursorValue;
						double delta2 = cursorValue - crosshairSortedData[valueItemIndex - 1].Value;
						valueItemIndex = CorrectCrossahirPointIndex(valueItemIndex, delta1, delta2);
					}
				}
			}
			position = CalculatePointPositionInSeries(valueItemIndex);
			if (SnapToArgument) {
				valueItemIndex = CheckPointsWithSameValue(valueItemIndex, value, out pointValue);
				pointArgument = crosshairSortedData[valueItemIndex].Value;
			}
			else {
				valueItemIndex = CheckPointsWithSameValue(valueItemIndex, argument, out pointArgument);
				pointValue = crosshairSortedData[valueItemIndex].Value;
			}
			IAxisRangeData xRange = seriesView.AxisXData.VisualRange;
			pointIndex = crosshairSortedData[valueItemIndex].PointIndex;
			ISideBySidePoint refinedPoint = pointsInfo[pointIndex];
			double offset = double.IsNaN(refinedPoint.Offset) ? 0 : refinedPoint.Offset;
			double halfBarWidth = double.IsNaN(refinedPoint.BarWidth) || refinedPoint.BarWidth == 0 ? 0 : refinedPoint.BarWidth / 2;
			double minPointValue = pointArgument + offset - halfBarWidth;
			double maxPointValue = pointArgument + offset + halfBarWidth;
			if (maxPointValue < xRange.Min || minPointValue > xRange.Max)
				return null;
			IAxisRangeData yRange = seriesView.AxisYData.VisualRange;
			if (pointValue < yRange.Min || pointValue > yRange.Max)
				return null;
			return (RefinedPoint)refinedPoint;
		}
		public RefinedPoint GetPointWithSameCrosshairValue(double argument, double value, out double pointArgument, out double pointValue) {
			double cursorValue = SnapToArgument ? argument : value;
			CrosshairValueItem cursorPoint = new CrosshairValueItem(cursorValue, 0);
			int pointIndex = crosshairSortedData.BinarySearch(cursorPoint, crosshairComparer);
			if (pointIndex >= 0) {
				int pointInfoIndex = crosshairSortedData[pointIndex].PointIndex;
				if (SnapToArgument) {
					pointIndex = CheckPointsWithSameValue(pointIndex, value, out pointValue);
					pointArgument = crosshairSortedData[pointIndex].Value;
				}
				else {
					pointIndex = CheckPointsWithSameValue(pointIndex, argument, out pointArgument);
					pointValue = crosshairSortedData[pointIndex].Value;
				}
				return pointsInfo[pointInfoIndex];
			}
			else {
				pointArgument = 0.0;
				pointValue = 0.0;
				return null;
			}
		}
	}
	public class CrosshairSeriesPointEx {
		readonly PointProjectionOnAxis argumentProjection;
		readonly PointProjectionOnAxis valueProjection;
		readonly GRealPoint2D anchorPoint;
		readonly IRefinedSeries refinedSeries;
		readonly RefinedPoint refinedPoint;
		readonly PointPositionInSeries position;
		readonly int pointIndex;
		CrosshairLine crosshairLine;
		CrosshairAxisInfo crosshairAxisInfo;
		CrosshairSeriesTextEx crosshairSeriesText;
		public CrosshairLine CrosshairLine {
			get { return crosshairLine; }
			internal set { crosshairLine = value; }
		}
		public CrosshairAxisInfo CrosshairAxisInfo {
			get { return crosshairAxisInfo; }
			internal set { crosshairAxisInfo = value; }
		}
		public CrosshairSeriesTextEx CrosshairSeriesText {
			get { return crosshairSeriesText; }
			internal set { crosshairSeriesText = value; }
		}
		public PointProjectionOnAxis AxisXProjection { get { return argumentProjection; } }
		public PointProjectionOnAxis AxisYProjection { get { return valueProjection; } }
		public GRealPoint2D AnchorPoint { get { return anchorPoint; } }
		public IRefinedSeries RefinedSeries { get { return refinedSeries; } }
		public RefinedPoint RefinedPoint { get { return refinedPoint; } }
		public IXYSeriesView View { get { return refinedSeries.SeriesView as IXYSeriesView; } }
		public PointPositionInSeries Position { get { return position; } }
		public int PointIndex { get { return pointIndex; } }
		public CrosshairSeriesPointEx(PointProjectionOnAxis argument, PointProjectionOnAxis value, GRealPoint2D anchorPoint, RefinedPoint refinedPoint, IRefinedSeries refinedSeries) {
			this.argumentProjection = argument;
			this.valueProjection = value;
			this.anchorPoint = anchorPoint;
			this.refinedPoint = refinedPoint;
			this.refinedSeries = refinedSeries;
		}
		public CrosshairSeriesPointEx(PointProjectionOnAxis argument, PointProjectionOnAxis value, GRealPoint2D anchorPoint, RefinedPoint refinedPoint, IRefinedSeries refinedSeries, PointPositionInSeries position, int pointIndex)
			: this(argument, value, anchorPoint, refinedPoint, refinedSeries) {
			this.position = position;
			this.pointIndex = pointIndex;
		}
	}
	public class CrosshairPaneInfoEx {
		public static string ConstructAxisText(PointProjectionOnAxis axisValue) {
			IAxisData axis = axisValue.Axis;
			if (!(axis is ICrosshairAxis))
				return string.Empty;
			string pattern = ((ICrosshairAxis)axis).LabelPattern;
			if (string.IsNullOrEmpty(pattern)) 
				pattern = axis.Label.TextPattern;
			PatternParser patternParser = new PatternParser(pattern, (IPatternHolder)axis);
			patternParser.SetContext(axisValue.NativeValue);
			return patternParser.GetText(); 
		}
		static string ConstructSeriesText(RefinedPoint refinedPoint, IRefinedSeries refinedSeries, IXYSeriesView view, ICrosshairOptions options, bool isInGroup) {
			string pattern = GetActualPatternForGroupedElements(view, options, isInGroup);
			PatternParser patternParser = new PatternParser(pattern,(IPatternHolder)refinedSeries.SeriesView);
			patternParser.SetContext(refinedPoint, refinedSeries.Series);
			return  patternParser.GetText(); 
		}
		static string ConstructGroupHeaderText(RefinedPoint refinedPoint, IRefinedSeries refinedSeries, ICrosshairOptions options) {
			bool snapsToArgument = options.SnapMode == CrosshairSnapModeCore.NearestArgument;
			CrosshairGroupHeaderValueToStringConverter converter = new CrosshairGroupHeaderValueToStringConverter(refinedSeries.Series, snapsToArgument, !snapsToArgument);
			string pattern = GetActualGroupHeaderPattern(options, converter);
			PatternParser patternParser = new PatternParser(pattern, (IPatternHolder)refinedSeries.SeriesView);
			patternParser.SetContext(refinedPoint);
			return patternParser.GetText(); 
		}
		static string GetActualAxisPattern(IAxisData axis, string format) {
			return axis.IsValueAxis ? "{V:" + format + "}" : "{A:" + format + "}";
		}
		static string GetActualPatternForGroupedElements(IXYSeriesView view, ICrosshairOptions options, bool isInGroup) {
			bool useGroupElementsPattern = options.ShowGroupHeaders && isInGroup;
			bool snapsToArgument = options.SnapMode == CrosshairSnapModeCore.NearestArgument;
			if (useGroupElementsPattern && String.IsNullOrEmpty(view.CrosshairLabelPattern)) {
				return view.CrosshairConverter.GetGroupedPointPattern(!snapsToArgument, snapsToArgument);
			}
			return GetActualPattern(view);
		}
		static string GetActualPattern(string crosshairLabelPattern, ToolTipPointDataToStringConverter converter) {
			return String.IsNullOrEmpty(crosshairLabelPattern) ? converter.DefaulPointPattern : crosshairLabelPattern;
		}
		public static string GetActualPattern(IXYSeriesView view) {
			return GetActualPattern(view.CrosshairLabelPattern, view.CrosshairConverter);
		}
		public static string GetActualGroupHeaderPattern(ICrosshairOptions options, ToolTipPointDataToStringConverter converter) {
			return String.IsNullOrEmpty(options.GroupHeaderPattern) ? converter.DefaulPointPattern : options.GroupHeaderPattern;
		}
		public static string GetActualAxisPattern(IAxisData axis, INumericOptions numericOptions, IDateTimeOptions dateTimeOptions) {
			string pattern = ((ICrosshairAxis)axis).LabelPattern;
			if (string.IsNullOrEmpty(pattern)) {
				switch (axis.AxisScaleTypeMap.ScaleType) {
					case ActualScaleType.DateTime:
						return GetActualAxisPattern(axis, DateTimeOptionsHelper.GetFormatString(dateTimeOptions));
					case ActualScaleType.Numerical:
						return GetActualAxisPattern(axis, NumericOptionsHelper.GetFormatString(numericOptions));
					default:
						return string.Empty;
				}
			}
			return pattern;
		}
		readonly IPane pane;
		readonly GRealRect2D mappingBounds;
		readonly GRealPoint2D cursorLocation;
		readonly PointProjectionsForPane cursorCoords;
		GRealRect2D labelsConstraint;
		readonly List<PointProjectionOnAxis> allPointProjections = new List<PointProjectionOnAxis>();
		readonly List<CrosshairSeriesPointEx> seriesPoints = new List<CrosshairSeriesPointEx>();
		readonly List<CrosshairLine> crossLines = new List<CrosshairLine>();
		readonly List<CrosshairLabelInfoEx> labelsInfo = new List<CrosshairLabelInfoEx>();
		readonly List<CrosshairAxisInfo> axesInfo = new List<CrosshairAxisInfo>();
		CrosshairLine cursorCrossLine;
		List<CrosshairAxisInfo> cursorAxesInfo = new List<CrosshairAxisInfo>();
		public IPane Pane { get { return pane; } }
		public GRealRect2D MappingBounds { get { return mappingBounds; } }
		public GRealPoint2D CursorLocation { get { return cursorLocation; } }
		public PointProjectionsForPane CursorCoords { get { return cursorCoords; } }
		public List<PointProjectionOnAxis> AllAxesValues { get { return allPointProjections; } }
		public List<CrosshairSeriesPointEx> SeriesPoints { get { return seriesPoints; } }
		public List<CrosshairLine> CrossLines { get { return crossLines; } }
		public List<CrosshairAxisInfo> AxesInfo { get { return axesInfo; } }
		public List<CrosshairLabelInfoEx> LabelsInfo { get { return labelsInfo; } }
		public GRealRect2D LabelsConstraint {
			get { return labelsConstraint; }
			set { labelsConstraint = value; }
		}
		public CrosshairLine CursorCrossLine {
			get { return cursorCrossLine; }
			internal set { cursorCrossLine = value; }
		}
		public List<CrosshairAxisInfo> CursorAxesInfo {
			get { return cursorAxesInfo; }
			internal set { cursorAxesInfo = value; }
		}
		public CrosshairPaneInfoEx(IPane pane, GRealRect2D mappingBounds, GRealPoint2D cursorLocation, PointProjectionsForPane cursorCoords) {
			this.pane = pane;
			this.mappingBounds = mappingBounds;
			this.cursorLocation = cursorLocation;
			this.cursorCoords = cursorCoords;
		}
		bool PointBelongsToGroup(RefinedPoint refinedPoint, CrosshairPointsGroup group, bool snapsToArgument) {
			if (snapsToArgument)
				return group.AnchorValue == refinedPoint.Argument;
			else
				return group.AnchorValue == refinedPoint.Value1 || group.AnchorValue == refinedPoint.Value2 ||
					group.AnchorValue == refinedPoint.Value3 || group.AnchorValue == refinedPoint.Value4;
		}
		void AddCrosshairSeriesText(CrosshairSeriesPointEx targetPoint, CrosshairPointsGroup targetGroup, ICrosshairOptions options, bool isInGroup) {
			string text = ConstructSeriesText(targetPoint.RefinedPoint, targetPoint.RefinedSeries, targetPoint.View, options, isInGroup);
			targetPoint.CrosshairSeriesText = new CrosshairSeriesTextEx(text, targetPoint.RefinedPoint, targetPoint.RefinedSeries);
			targetGroup.SeriesTexts.Add(targetPoint.CrosshairSeriesText);
		}
		public void AddValue(PointProjectionOnAxis axisValue) {
			if (!allPointProjections.Contains(axisValue))
				allPointProjections.Add(axisValue);
		}
		public void AddSeriesPoint(CrosshairSeriesPointEx seriesPoint) {
			seriesPoints.Add(seriesPoint);
		}
		public void AddCrossLine(CrosshairLine line) {
			crossLines.Add(line);
		}
		public void AddCrosshairLabel(List<CrosshairSeriesPointEx> seriesPoints,
			GRealPoint2D anchorPoint, AnnotationLocation defaultLocation, bool showTail, GRealPoint2D initOffset, ICrosshairOptions crosshairOptions) {
			if (seriesPoints.Count != 0) {
				List<CrosshairPointsGroup> groups = new List<CrosshairPointsGroup>();
				bool snapsToArgument = crosshairOptions.SnapMode == CrosshairSnapModeCore.NearestArgument;
				foreach (CrosshairSeriesPointEx point in seriesPoints) {
					bool isInGroup = false;
					if (crosshairOptions.ShowGroupHeaders) {
						foreach (CrosshairPointsGroup group in groups)
							if (PointBelongsToGroup(point.RefinedPoint, group, snapsToArgument)) {
								isInGroup = true;
								group.SeriesPoints.Add(point);
								break;
							}
					}
					if (!isInGroup) {
						CrosshairPointsGroup group = new CrosshairPointsGroup(point, snapsToArgument);
						group.SeriesPoints.Add(point);
						groups.Add(group);
					}
				}
				bool hasHeader = groups.Count != seriesPoints.Count && crosshairOptions.ShowGroupHeaders;
				foreach (CrosshairPointsGroup group in groups) {
					CrosshairSeriesPointEx basePoint = group.SeriesPoints[0];
					group.HeaderText = hasHeader ? ConstructGroupHeaderText(basePoint.RefinedPoint, basePoint.RefinedSeries, crosshairOptions) : "";
					foreach (CrosshairSeriesPointEx point in group.SeriesPoints) {
						AddCrosshairSeriesText(point, group, crosshairOptions, hasHeader);
					}
				}
				labelsInfo.Add(new CrosshairLabelInfoEx(groups, anchorPoint, defaultLocation, showTail, initOffset));
			}
		}
		public void AddCrosshairLabel(CrosshairSeriesPointEx seriesPoint, ICrosshairOptions crosshairOptions) {
			AddCrosshairLabel(new List<CrosshairSeriesPointEx>() { seriesPoint },
				 seriesPoint.AnchorPoint, AnnotationLocation.TopRight, true, new GRealPoint2D(0, 0), crosshairOptions);
		}
		public CrosshairAxisInfo AddAxisLabel(PointProjectionOnAxis axisValue, GRealPoint2D anchorPoint, GRealSize2D maxSize) {
			string text = ConstructAxisText(axisValue);
			if (!string.IsNullOrEmpty(text)) {
				CrosshairAxisInfo crosshairAxisInfo = new CrosshairAxisInfo(text, anchorPoint, maxSize, axisValue.Axis, axisValue.NativeValue);
				axesInfo.Add(crosshairAxisInfo);
				return crosshairAxisInfo;
			}
			return null;
		}
		public void AddAxisLabel(CrosshairAxisInfo crosshairAxisInfo) {
			axesInfo.Add(crosshairAxisInfo);
		}
		public void CompleteLabelsLayout() {
			List<IAnnotationLayout> labelLayouts = new List<IAnnotationLayout>();
			foreach (CrosshairLabelInfoEx seriesLabelInfo in labelsInfo)
				labelLayouts.Add(seriesLabelInfo);
			if (labelsConstraint.IsEmpty)
				AnnotationLayoutCalculator.CalculateAutoLayout(labelLayouts);
			else
				AnnotationLayoutCalculator.CalculateAutoLayout(labelLayouts, labelsConstraint);
		}
	}
	public class CrosshairSeriesTextEx {
		readonly RefinedPoint refinedPoint;
		readonly IRefinedSeries refinedSeries;
		readonly string text;
		public IRefinedSeries RefinedSeries { get { return refinedSeries; } }
		public RefinedPoint RefinedPoint { get { return refinedPoint; } }
		public string Text { get { return text; } }
		public CrosshairSeriesTextEx(string text, RefinedPoint refinedPoint, IRefinedSeries refinedSeries) {
			this.text = text;
			this.refinedPoint = refinedPoint;
			this.refinedSeries = refinedSeries;
		}
	}
	public class CrosshairPointsGroup {
		string headerText = "";
		List<CrosshairSeriesTextEx> seriesTexts;
		List<CrosshairSeriesPointEx> seriesPoints;
		readonly double anchorValue;
		internal double AnchorValue { get { return anchorValue; } }
		public List<CrosshairSeriesPointEx> SeriesPoints { get { return seriesPoints; } }
		public List<CrosshairSeriesTextEx> SeriesTexts { get { return seriesTexts; } }
		public string HeaderText {
			get { return headerText; }
			internal set { headerText = value; }
		}
		internal CrosshairPointsGroup(CrosshairSeriesPointEx anchorPoint, bool snapsToArgument) {
			this.seriesTexts = new List<CrosshairSeriesTextEx>();
			this.seriesPoints = new List<CrosshairSeriesPointEx>();
			anchorValue = snapsToArgument ? anchorPoint.RefinedPoint.Argument : anchorPoint.RefinedPoint.Value1;
		}
	}
	public class CrosshairInfoEx : IEnumerable<CrosshairPaneInfoEx> {
		readonly List<CrosshairPaneInfoEx> crosshairPaneInfos = new List<CrosshairPaneInfoEx>();
		#region IEnumerable
		IEnumerator<CrosshairPaneInfoEx> IEnumerable<CrosshairPaneInfoEx>.GetEnumerator() {
			return crosshairPaneInfos.GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return crosshairPaneInfos.GetEnumerator();
		}
		#endregion
		public void Add(CrosshairPaneInfoEx paneInfo) {
			crosshairPaneInfos.Add(paneInfo);
		}
		public CrosshairPaneInfoEx GetByPane(IPane pane) {
			foreach (CrosshairPaneInfoEx paneInfo in crosshairPaneInfos) {
				if (Object.ReferenceEquals(paneInfo.Pane, pane))
					return paneInfo;
			}
			return null;
		}
		public void CompleteLabelsLayout() {
			foreach (CrosshairPaneInfoEx crosshairPaneInfo in crosshairPaneInfos)
				crosshairPaneInfo.CompleteLabelsLayout();
		}
	}
	public class CrosshairLabelInfoEx : IAnnotationLayout {
		readonly ICollection<CrosshairPointsGroup> pointGroups;
		readonly GRealPoint2D anchorPoint;
		GRealSize2D size;
		GRealRect2D bounds;
		bool showTail;
		AnnotationLocation location;
		GRealPoint2D offset;
		GRealPoint2D initOffset;
		public ICollection<CrosshairPointsGroup> PointGroups { get { return pointGroups; } }
		public GRealPoint2D AnchorPoint { get { return anchorPoint; } }
		public GRealSize2D Size { get { return size; } set { size = value; } }
		public GRealRect2D Bounds { get { return bounds; } set { bounds = value; } }
		public bool ShowTail { get { return showTail; } set { showTail = value; } }
		public double CursorOffset { get { return 0; } }
		public AnnotationLocation Location { get { return location; } set { location = value; } }
		public GRealPoint2D Offset {
			get { return offset; }
			set { offset = value; }
		}
		public GRealPoint2D InitOffset { get { return initOffset; } }
		public CrosshairLabelInfoEx(IList<CrosshairPointsGroup> pointGroups, GRealPoint2D anchorPoint, AnnotationLocation defaultLocation, bool showTail, GRealPoint2D initOffset) {
			this.pointGroups = pointGroups;
			this.anchorPoint = anchorPoint;
			this.location = defaultLocation;
			this.showTail = showTail;
			this.initOffset = initOffset;
		}
	}
	class CrosshairPointComparer : Comparer<CrosshairSeriesPointEx> {
		bool isHorisontalCrosshair;
		public CrosshairPointComparer(bool isHorisontalCrosshair) {
			this.isHorisontalCrosshair = isHorisontalCrosshair;
		}
		public override int Compare(CrosshairSeriesPointEx point1, CrosshairSeriesPointEx point2) {
			return ((RefinedSeries)point1.RefinedSeries).ActiveIndex.CompareTo(((RefinedSeries)point2.RefinedSeries).ActiveIndex);
		}
	}
}
