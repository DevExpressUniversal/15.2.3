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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class AxisGridAndTextData : Dictionary<AxisInterval, GridAndTextDataEx> {
		readonly Axis2D axis;
		readonly Rectangle maxBounds;
		public Axis2D Axis { get { return axis; } }
		public Rectangle MaxBounds { get { return maxBounds; } }
		public AxisGridAndTextData(Axis2D axis, Rectangle maxBounds) {
			this.axis = axis;
			this.maxBounds = maxBounds;
			int axisLength = axis.IsVertical ? maxBounds.Height : maxBounds.Width;
			foreach (AxisIntervalLayout layout in AxisIntervalLayout.CreateIntervalsLayout(axis, axis, axisLength)) {
				AxisInterval interval = layout.Interval;
				Add(interval, new GridAndTextDataEx(axis.GetSeries(), axis, axis.XYDiagram2D.IsScrollingEnabled, interval.Range, interval.WholeRange, layout.Bounds.Length, false));
			}
		}
	}
	public class AxisViewDataCalculator {
		public static List<AxisViewData> Calculate(TextMeasurer textMeasurer, XYDiagramPaneBase pane, AxisIntervalsLayoutRepository intervalsLayoutRepository,
			PaneAxesContainer paneAxesData, ScrollBarPositions scrollBarPositions, List<AxisGridAndTextData> axisGridAndTextDataList, bool resolveLabelsOverlapping, bool autoLayout) {
				AxisViewDataCalculator axisViewDataCalculator = new AxisViewDataCalculator(textMeasurer, pane, intervalsLayoutRepository, scrollBarPositions, paneAxesData, resolveLabelsOverlapping, autoLayout);
			return axisViewDataCalculator.Calculate(axisGridAndTextDataList);
		}
		const int secondaryAxisIndent = 8;
		const int scrollBarIndent = 3;
		readonly TextMeasurer textMeasurer;
		readonly XYDiagramPaneBase pane;
		readonly AxisIntervalsLayoutRepository intervalsLayoutRepository;
		readonly ScrollBarPositions scrollBarPositions;
		readonly IAxisData primaryAxisX;
		readonly IAxisData primaryAxisY;
		readonly AxisMapping primaryAxisXMapping;
		readonly AxisMapping primaryAxisYMapping;
		readonly int scrollBarThickness;
		readonly bool resolveLabelsOverlapping;
		readonly bool autoLayout;
		AxisViewDataCalculator(TextMeasurer textMeasurer, XYDiagramPaneBase pane, AxisIntervalsLayoutRepository intervalsLayoutRepository, ScrollBarPositions scrollBarPositions, PaneAxesContainer paneAxesData, bool resolveLabelsOverlapping, bool autoLayout) {
			this.textMeasurer = textMeasurer;
			this.pane = pane;
			this.intervalsLayoutRepository = intervalsLayoutRepository;
			this.scrollBarPositions = scrollBarPositions;
			this.resolveLabelsOverlapping = resolveLabelsOverlapping;
			this.autoLayout = autoLayout;
			primaryAxisX = paneAxesData.PrimaryAxisX;
			primaryAxisY = paneAxesData.PrimaryAxisY;
			primaryAxisXMapping = new AxisMapping(intervalsLayoutRepository, (Axis2D)primaryAxisX);
			primaryAxisYMapping = new AxisMapping(intervalsLayoutRepository, (Axis2D)primaryAxisY);
			AxisMapping.InterimZeroCalculator calculator;
			calculator = new AxisMapping.InterimZeroCalculator(primaryAxisY, intervalsLayoutRepository.MappingBounds, intervalsLayoutRepository.GetIntervalsLayout(primaryAxisY), ((Axis2D)primaryAxisY).Alignment);
			primaryAxisXMapping.InitializeInterimZero(calculator.CalculateInterimZero());
			calculator = new AxisMapping.InterimZeroCalculator(primaryAxisX, intervalsLayoutRepository.MappingBounds, intervalsLayoutRepository.GetIntervalsLayout(primaryAxisX), ((Axis2D)primaryAxisY).Alignment);
			primaryAxisYMapping.InitializeInterimZero(calculator.CalculateInterimZero());
			scrollBarThickness = pane.ScrollBarOptions.BarThickness;
		}
		bool IsAxisVisible(Axis2D axis) {
			return axis.ActualVisibility && axis.VisibleInPane(pane);
		}
		AxisMapping GetAxisMapping(Axis2D axis) {
			if (axis == primaryAxisX)
				return primaryAxisXMapping;
			if (axis == primaryAxisY)
				return primaryAxisYMapping;
			return new AxisMapping(intervalsLayoutRepository, axis);
		}
		List<AxisIntervalMapping> GetAxisIntervalMapping(Axis2D axis, AxisMapping axisMapping) {
			List<AxisIntervalMapping> intervalMappings = new List<AxisIntervalMapping>();
			List<AxisIntervalLayout> intervalsLayout = this.intervalsLayoutRepository.GetIntervalsLayout(axis);
			foreach (AxisIntervalLayout intervalLayout in intervalsLayout) {
				AxisIntervalMapping axisIntervalMapping = new AxisIntervalMapping(axis, intervalLayout, axisMapping.MappingBounds, axisMapping.Alignment);
				axisIntervalMapping.InitializeInterimZero(axisMapping.InterimZero);
				intervalMappings.Add(axisIntervalMapping);
			}
			return intervalMappings;
		}
		AxisViewData CalculateViewData(AxisGridAndTextData gridAndTextData, bool scrollBarVisible, int axisOffset) {
			Axis2D axis = gridAndTextData.Axis;
			int elementOffset = 0;
			bool axisVisible = IsAxisVisible(axis);
			if (axisVisible) {
				if (axisOffset > 0) {
					axisOffset += secondaryAxisIndent;
					if (axis.Tickmarks.CrossAxis)
						axisOffset += axis.Tickmarks.MaxLength;
				}
				else if (scrollBarVisible)
					elementOffset = scrollBarThickness + scrollBarIndent;
			}
			AxisMapping axisMapping = GetAxisMapping(axis);
			return new AxisViewData(axis, textMeasurer, gridAndTextData, axisMapping, GetAxisIntervalMapping(axis, axisMapping), axisVisible, axisOffset, elementOffset, resolveLabelsOverlapping, autoLayout);
		}
		AxisViewData CalculateZeroViewData(AxisGridAndTextData gridAndTextData) {
			Axis2D axis = gridAndTextData.Axis;
			AxisMapping axisMapping = GetAxisMapping(axis);
			return new AxisViewData(axis, textMeasurer, gridAndTextData, axisMapping, GetAxisIntervalMapping(axis, axisMapping), IsAxisVisible(axis), 0, 0, resolveLabelsOverlapping, autoLayout);
		}
		List<AxisViewData> Calculate(List<AxisGridAndTextData> axisGridAndTextDataList) {
			int leftAxisOffset = 0;
			int topAxisOffset = 0;
			int rightAxisOffset = 0;
			int bottomAxisOffset = 0;
			List<AxisViewData> viewData = new List<AxisViewData>();
			foreach (AxisGridAndTextData axisGridAndTextData in axisGridAndTextDataList) {
				Axis2D axis = axisGridAndTextData.Axis;
				AxisViewData axisViewData = null;
				if (axis.IsVertical) {
					switch (axis.Alignment) {
						case AxisAlignment.Far: {
								bool hasRightScrollbar = (scrollBarPositions & ScrollBarPositions.Right) != 0;
								axisViewData = CalculateViewData(axisGridAndTextData, hasRightScrollbar, rightAxisOffset);
								rightAxisOffset = axisViewData.NextAxisOffset;
								break;
							}
						case AxisAlignment.Zero:
							axisViewData = CalculateZeroViewData(axisGridAndTextData);
							break;
						default: {
								bool hasLeftScrollbar = (scrollBarPositions & ScrollBarPositions.Left) != 0;
								axisViewData = CalculateViewData(axisGridAndTextData, hasLeftScrollbar, leftAxisOffset);
								leftAxisOffset = axisViewData.NextAxisOffset;
								break;
							}
					}
				}
				else {
					switch (axis.Alignment) {
						case AxisAlignment.Far: {
								bool hasTopScrollbar = (scrollBarPositions & ScrollBarPositions.Top) != 0;
								axisViewData = CalculateViewData(axisGridAndTextData, hasTopScrollbar, topAxisOffset);
								topAxisOffset = axisViewData.NextAxisOffset;
								break;
							}
						case AxisAlignment.Zero:
							axisViewData = CalculateZeroViewData(axisGridAndTextData);
							break;
						default: {
								bool hasBottomScrollBar = (scrollBarPositions & ScrollBarPositions.Bottom) != 0;
								axisViewData = CalculateViewData(axisGridAndTextData, hasBottomScrollBar, bottomAxisOffset);
								bottomAxisOffset = axisViewData.NextAxisOffset;
								break;
							}
					}
				}
				viewData.Add(axisViewData);
			}
			return viewData;
		}
	}
}
