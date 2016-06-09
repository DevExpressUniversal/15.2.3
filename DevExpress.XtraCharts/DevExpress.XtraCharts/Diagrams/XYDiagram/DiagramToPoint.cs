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
using System.Drawing;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public static class DiagramToPointUtils {
		static object CheckValue(object value, ActualScaleType scaleType, ActualScaleType incorrectScaleType) {
			if (scaleType == incorrectScaleType)
				throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgDiagramToPointIncorrectValue), value.GetType().ToString(), scaleType.ToString()));
			if (scaleType == ActualScaleType.Qualitative)
				return value.ToString();
			return value;
		}
		public static object CheckValue(string value, ActualScaleType scaleType) {
			object convertedArgument = value;
			try {
				if (scaleType == ActualScaleType.DateTime)
					convertedArgument = Convert.ToDateTime(value);
				else if (scaleType == ActualScaleType.Numerical)
					convertedArgument = Convert.ToDouble(value);
			}
			catch (Exception e) {
				throw new ArgumentException(String.Format(ChartLocalizer.GetString(ChartStringId.MsgDiagramToPointIncorrectValue), value.GetType().ToString(), scaleType.ToString()), e);
			}
			return convertedArgument;
		}
		public static object CheckValue(double value, ActualScaleType scaleType) {
			return CheckValue(value, scaleType, ActualScaleType.DateTime);
		}
		public static object CheckValue(DateTime value, ActualScaleType scaleType) {
			return CheckValue(value, scaleType, ActualScaleType.Numerical);
		}
		public static object CheckValue(object value, ActualScaleType scaleType) {
			if (value is string)
				return CheckValue((string)value, scaleType);
			else if (value is DateTime)
				return CheckValue((DateTime)value, scaleType);
			return CheckValue((double)value, scaleType);
		}
	}
	public class DiagramToPointCalculator {
		static bool ValueInScaleBreak(List<AxisIntervalLayout> intervalLayouts) {
			return intervalLayouts.Count == 2;
		}
		static bool CheckLayouts(List<AxisIntervalLayout> intervalLayouts) {
			return intervalLayouts.Count == 1 || intervalLayouts.Count == 2;
		}
		static Point GetCoordInScaleBreak(double value, List<AxisIntervalLayout> layout, Axis2D axis, XYDiagramMappingContainer mappingContainer) {
			AxisMapping axisMapping = new AxisMapping(mappingContainer.IntervalsLayoutRepository, axis);
			AxisIntervalLayout minInterval, maxInterval;
			if (layout[0].Range.Max > layout[1].Range.Max) {
				minInterval = layout[1];
				maxInterval = layout[0];
			}
			else {
				minInterval = layout[0];
				maxInterval = layout[1];
			}
			double min = minInterval.Range.Max;
			double max = maxInterval.Range.Min;
			double factor = max - min != 0 ? (value - min) / (max - min) : 0;
			double coord = minInterval.End + (maxInterval.Start - minInterval.End) * factor;
			return (Point)axisMapping.InterimPointToScreenPoint(new DiagramPoint(coord, 0));
		}
		static List<AxisIntervalLayout> CalcIntervals(double value, IList<AxisIntervalLayout> intervalsLayout, out bool inRange) {
			inRange = false;
			List<AxisIntervalLayout> resultIntervals = new List<AxisIntervalLayout>();
			AxisIntervalLayout aboveInterval = null;
			AxisIntervalLayout belowInterval = null;
			foreach (AxisIntervalLayout layout in intervalsLayout) {
				if (layout.ValueWithinRange(value)) {
					resultIntervals.Add(layout);
					inRange = true;
					return resultIntervals;
				}
				if (value > layout.Range.Max) {
					if (belowInterval == null)
						belowInterval = layout;
					else if (belowInterval.Range.Max < layout.Range.Max)
						belowInterval = layout;
				}
				if (value < layout.Range.Min)
					if (aboveInterval == null)
						aboveInterval = layout;
					else if (aboveInterval.Range.Min > layout.Range.Min)
						aboveInterval = layout;
			}
			if (belowInterval != null)
				resultIntervals.Add(belowInterval);
			if (aboveInterval != null)
				resultIntervals.Add(aboveInterval);
			return resultIntervals;
		}
		public static ControlCoordinates CalculateCoords(XYDiagramPaneBase pane, XYDiagramMappingContainer mappingContainer, double argument, double value) {
			bool valueXinRange = false;
			bool valueYinRange = false;
			List<AxisIntervalLayout> layoutsX = CalcIntervals(argument, mappingContainer.IntervalsLayoutX, out valueXinRange);
			List<AxisIntervalLayout> layoutsY = CalcIntervals(value, mappingContainer.IntervalsLayoutY, out valueYinRange);
			if (!CheckLayouts(layoutsX) || !CheckLayouts(layoutsY))
				return new ControlCoordinates(pane);
			ControlCoordinatesVisibility visibility = (valueXinRange && valueYinRange) ? ControlCoordinatesVisibility.Visible : ControlCoordinatesVisibility.Hidden;
			XYDiagramMappingBase mapping = mappingContainer.CreateDiagramMapping(layoutsX[0], layoutsY[0]);
			Point point = (Point)mapping.GetScreenPoint(argument, value);
			if (!ValueInScaleBreak(layoutsX) && !ValueInScaleBreak(layoutsY))
				return new ControlCoordinates(pane, visibility, point);
			int x, y;
			Axis2D axisX = mappingContainer.AxisX;
			Axis2D axisY = mappingContainer.AxisY;
			if (!mappingContainer.Rotated) {
				x = ValueInScaleBreak(layoutsX) ? GetCoordInScaleBreak(argument, layoutsX, axisX, mappingContainer).X : point.X;
				y = ValueInScaleBreak(layoutsY) ? GetCoordInScaleBreak(value, layoutsY, axisY, mappingContainer).Y : point.Y;
			}
			else {
				x = ValueInScaleBreak(layoutsY) ? GetCoordInScaleBreak(value, layoutsY, axisY, mappingContainer).X : point.X;
				y = ValueInScaleBreak(layoutsX) ? GetCoordInScaleBreak(argument, layoutsX, axisX, mappingContainer).Y : point.Y;
			}
			return new ControlCoordinates(pane, visibility, new Point(x, y));
		}
	}
}
