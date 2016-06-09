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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class CrosshairDrawInfo {
		#region Nested structs: CrosshairPointDrawInfo, CrosshairAxisLableDrawInfo, CrosshairLineDrawInfo
		internal struct CrosshairPointDrawInfo {
			CrosshairSeriesPointEx crosshairSeriesPoint;
			CrosshairElement crosshairElement;
			public CrosshairSeriesPointEx CrosshairSeriesPoint { get { return crosshairSeriesPoint; } }
			public CrosshairElement CrosshairElement { get { return crosshairElement; } }
			public CrosshairPointDrawInfo(CrosshairSeriesPointEx crosshairSeriesPoint, CrosshairElement crosshairElement) {
				this.crosshairSeriesPoint = crosshairSeriesPoint;
				this.crosshairElement = crosshairElement;
			}
		}
		internal struct CrosshairAxisLableDrawInfo {
			CrosshairAxisInfo crosshairAxisInfo;
			CrosshairAxisLabelElement crosshairAxisLabelElement;
			public CrosshairAxisInfo CrosshairAxisInfo { get { return crosshairAxisInfo; } }
			public CrosshairAxisLabelElement CrosshairAxisLabelElement { get { return crosshairAxisLabelElement; } }
			public CrosshairAxisLableDrawInfo(CrosshairAxisInfo crosshairAxisInfo, CrosshairAxisLabelElement crosshairAxisLabelElement) {
				this.crosshairAxisInfo = crosshairAxisInfo;
				this.crosshairAxisLabelElement = crosshairAxisLabelElement;
			}
		}
		internal struct CrosshairLineDrawInfo {
			CrosshairLine crosshairLine;
			CrosshairLineElement crosshairLineElement;
			public CrosshairLine CrosshairLine { get { return crosshairLine; } }
			public CrosshairLineElement CrosshairLineElement {
				get { return crosshairLineElement; }
				set { crosshairLineElement = value; }
			}
			public CrosshairLineDrawInfo(CrosshairLine crosshairLine, CrosshairLineElement crosshairLineElement) {
				this.crosshairLine = crosshairLine;
				this.crosshairLineElement = crosshairLineElement;
			}
		}
		#endregion
		readonly Size defaultMarkerSize = new Size(15, 12);
		List<CrosshairPointDrawInfo> pointDrawInfos;
		CrosshairLineDrawInfo cursorLineDrawInfo;
		List<CrosshairAxisLableDrawInfo> cursorAxisLableDrawInfos;
		List<CrosshairElementGroup> elementGroups;
		internal List<CrosshairPointDrawInfo> PointDrawInfos { get { return pointDrawInfos; } }
		internal List<CrosshairAxisLableDrawInfo> CursorAxisLableDrawInfos { get { return cursorAxisLableDrawInfos; } }
		internal CrosshairLineDrawInfo CursorLineDrawInfo { get { return cursorLineDrawInfo; } }
		internal CrosshairLineElement CrosshairLineElement {
			get { return cursorLineDrawInfo.CrosshairLineElement; }
			set { cursorLineDrawInfo.CrosshairLineElement = value; }
		}
		public List<CrosshairElementGroup> CrosshairElementGroups { get { return elementGroups; } }
		public CrosshairDrawInfo(CrosshairPaneInfoEx crosshairInfo, CrosshairOptions crosshairOptions, TextAnnotationAppearance appearence) {
			pointDrawInfos = new List<CrosshairPointDrawInfo>();
			elementGroups = new List<CrosshairElementGroup>();
			cursorAxisLableDrawInfos = new List<CrosshairAxisLableDrawInfo>();
			FillPointDrawInfos(crosshairInfo, crosshairOptions, appearence);
			if (crosshairInfo.CursorCrossLine != null) {
				CrosshairLineElement cursorCrosshairLineElement = CreateCrosshairLineElement(crosshairOptions, crosshairInfo.CursorCrossLine);
				cursorLineDrawInfo = new CrosshairLineDrawInfo(crosshairInfo.CursorCrossLine, cursorCrosshairLineElement);
			}
			FillCursorAxisLableDrawInfos(crosshairInfo);
		}
		void FillPointDrawInfos(CrosshairPaneInfoEx crosshairInfo, CrosshairOptions crosshairOptions, TextAnnotationAppearance appearence) {
			foreach (CrosshairLabelInfoEx labelInfo in crosshairInfo.LabelsInfo) {
				foreach (CrosshairPointsGroup pointsGroup in labelInfo.PointGroups) {
					List<CrosshairElement> groupElements = new List<CrosshairElement>();
					foreach (CrosshairSeriesPointEx crosshairPoint in pointsGroup.SeriesPoints) {
						CrosshairLineElement crosshairLineElement = CreateCrosshairLineElement(crosshairOptions, crosshairPoint.CrosshairLine);
						CrosshairAxisLabelElement crosshairAxisLabelElement = CreateCrosshairAxisLabelElement(crosshairPoint.CrosshairAxisInfo);
						CrosshairLabelElement crosshairLabelElement = CreateCrosshairLabelElement(crosshairPoint.CrosshairSeriesText, appearence, crosshairOptions, crosshairPoint);
						CrosshairElement crosshairElement = new CrosshairElement(crosshairPoint.RefinedPoint, crosshairPoint.RefinedSeries, crosshairLineElement, crosshairAxisLabelElement, crosshairLabelElement);
						pointDrawInfos.Add(new CrosshairPointDrawInfo(crosshairPoint, crosshairElement));
						groupElements.Add(crosshairElement);
					}
					CrosshairElementGroup elementGroup = new CrosshairElementGroup(CreateCrosshairGroupHeaderElement(pointsGroup, crosshairOptions, appearence), groupElements, labelInfo);
					elementGroups.Add(elementGroup);
				}
			}
		}
		void FillCursorAxisLableDrawInfos(CrosshairPaneInfoEx crosshairInfo) {
			foreach (CrosshairAxisInfo crosshairAxisInfo in crosshairInfo.CursorAxesInfo) {
				CrosshairAxisLabelElement axisLabelElement = CreateCrosshairAxisLabelElement(crosshairAxisInfo);
				cursorAxisLableDrawInfos.Add(new CrosshairAxisLableDrawInfo(crosshairAxisInfo, axisLabelElement));
			}
		}
		CrosshairAxisLabelElement CreateCrosshairAxisLabelElement(CrosshairAxisInfo crosshairAxisInfo) {
			CrosshairAxisLabelOptions crosshairAxisLabelOptions = ((Axis2D)crosshairAxisInfo.Axis).CrosshairAxisLabelOptions;
			return new CrosshairAxisLabelElement(crosshairAxisInfo.Value, crosshairAxisInfo.Text,
				crosshairAxisLabelOptions.ActualBackColor, crosshairAxisLabelOptions.ActualTextColor, crosshairAxisLabelOptions.ActualFont, crosshairAxisLabelOptions.ActualVisibility);
		}
		CrosshairLabelElement CreateCrosshairLabelElement(CrosshairSeriesTextEx crosshairText, TextAnnotationAppearance appearence, CrosshairOptions crosshairOptions, CrosshairSeriesPointEx crosshairPoint) {
			Color textColor = appearence.TextColor;
			SeriesViewBase view = crosshairPoint.View as SeriesViewBase;
			IRefinedSeries refinedSeries = crosshairPoint.RefinedSeries;
			RefinedPoint point = crosshairPoint.RefinedPoint;
			DrawOptions pointDrawOptions = view.CreatePointDrawOptions(refinedSeries, point);
			CrosshairLabelElement crosshairLabelElement = new CrosshairLabelElement(crosshairText.Text, textColor,
				DefaultFonts.Tahoma8, null, ChartImageSizeMode.AutoSize, defaultMarkerSize, pointDrawOptions.Color, true, true, view.Series.ActualCrosshairLabelVisibility);
			return crosshairLabelElement;
		}
		CrosshairGroupHeaderElement CreateCrosshairGroupHeaderElement(CrosshairPointsGroup pointsGroup, CrosshairOptions options, TextAnnotationAppearance appearence) {
			Color textColor = appearence.TextColor;
			return new CrosshairGroupHeaderElement(CollectPoints(pointsGroup), pointsGroup.HeaderText, textColor, DefaultFonts.Tahoma8);
		}
		CrosshairLineElement CreateCrosshairLineElement(CrosshairOptions crosshairOptions, CrosshairLine crosshairLine) {
			CrosshairLineElement crosshairLineElement = new CrosshairLineElement(GetLineColor(crosshairLine, crosshairOptions), GetLineStyle(crosshairLine, crosshairOptions), crosshairLine.IsValueLine ? crosshairOptions.ShowValueLine : crosshairOptions.ShowArgumentLine);
			return crosshairLineElement;
		}
		Color GetLineColor(CrosshairLine crosshairLine, CrosshairOptions crosshairOptions) {
			return crosshairLine.IsValueLine ? crosshairOptions.ValueLineColor : crosshairOptions.ArgumentLineColor;
		}
		LineStyle GetLineStyle(CrosshairLine crosshairLine, CrosshairOptions crosshairOptions) {
			return crosshairLine.IsValueLine ? crosshairOptions.ValueLineStyle : crosshairOptions.ArgumentLineStyle;
		}
		List<SeriesPoint> CollectPoints(CrosshairPointsGroup group) {
			List<SeriesPoint> points = new List<SeriesPoint>();
			foreach (CrosshairSeriesTextEx text in group.SeriesTexts) {
				points.Add(SeriesPoint.GetSeriesPoint(text.RefinedPoint.SeriesPoint));
			}
			return points;
		}
	}
}
