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
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class ChartHitInfo {
		readonly List<IHitTestableElement> hitTestableElements = new List<IHitTestableElement>();
		readonly ChartControl chart;
		readonly Point point;
		readonly RefinedPoint refinedPoint;
		readonly Series series3D;
		Diagram diagram = null;
		AxisBase axis = null;
		Series Series3D { get { return series3D != null ? series3D : refinedPoint != null ? SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint).Series : null; } }
		internal RefinedPoint RefinedPoint {
			get {
				return refinedPoint != null ?
					   refinedPoint : GetAdditionalElementByType(typeof(RefinedPoint)) != null ?
					   GetAdditionalElementByType(typeof(RefinedPoint)) as RefinedPoint :
					   GetElementByType(typeof(RefinedPoint)) as RefinedPoint;
			}
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInPane")]
#endif
		public bool InPane { get { return Pane != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInDiagram")]
#endif
		public bool InDiagram { get { return Diagram != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInAxis")]
#endif
		public bool InAxis { get { return Axis != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInAxisLabel")]
#endif
		public bool InAxisLabel { get { return AxisLabel != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInAxisTitle")]
#endif
		public bool InAxisTitle { get { return AxisTitle != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInConstantLine")]
#endif
		public bool InConstantLine { get { return ConstantLine != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInStrip")]
#endif
		public bool InStrip { get { return Strip != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInSeries")]
#endif
		public bool InSeries { get { return Series != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInSeriesLabel")]
#endif
		public bool InSeriesLabel { get { return SeriesLabel != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInTitle")]
#endif
		public bool InTitle { get { return Title != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInLegend")]
#endif
		public bool InLegend { get { return Legend != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoInIndicator")]
#endif
		public bool InIndicator { get { return Indicator != null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoPane")]
#endif
		public Pane Pane { get { return GetAdditionalElementByType(typeof(Pane)) as Pane; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoDiagram")]
#endif
		public Diagram Diagram { get { return diagram == null ? GetElementByType(typeof(Diagram)) as Diagram : diagram; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoAxis")]
#endif
		public AxisBase Axis { get { return axis == null ? GetElementByType(typeof(AxisBase)) as AxisBase : axis; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoAxisLabel")]
#endif
		public AxisLabel AxisLabel { get { return GetElementByType(typeof(AxisLabel)) as AxisLabel; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoAxisTitle")]
#endif
		public AxisTitle AxisTitle { get { return GetElementByType(typeof(AxisTitle)) as AxisTitle; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoConstantLine")]
#endif
		public ConstantLine ConstantLine { get { return GetElementByType(typeof(ConstantLine)) as ConstantLine; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoStrip")]
#endif
		public Strip Strip { get { return GetElementByType(typeof(Strip)) as Strip; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoSeriesLabel")]
#endif
		public SeriesLabel SeriesLabel { get { return GetElementByType(typeof(SeriesLabel)) as SeriesLabel; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoTitle")]
#endif
		public Title Title { get { return GetElementByType(typeof(Title)) as Title; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoLegend")]
#endif
		public Legend Legend { get { return GetElementByType(typeof(Legend)) as Legend; } }
		public Indicator Indicator { get { return GetElementByType(typeof(Indicator)) as Indicator; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoSeries")]
#endif
		public Series Series { get { return Series3D != null ? Series3D : GetElementByType(typeof(Series)) as Series; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoSeriesPoint")]
#endif
		public SeriesPoint SeriesPoint { get { return RefinedPoint != null ? SeriesPoint.GetSeriesPoint(RefinedPoint.SeriesPoint) : null; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("ChartHitInfoCustomAxisLabel")]
#endif
		public CustomAxisLabel CustomAxisLabel { 
			get { 
				AxisLabelItem axisLabelItem = GetAdditionalElementByType(typeof(AxisLabelItem)) as AxisLabelItem;
				return axisLabelItem == null ? null : axisLabelItem.CustomAxisLabel;
			} 
		}
		internal ChartHitInfo(ChartControl chart, Point point, double radius = 0) {
			this.chart = chart;
			this.point = point;
			if (chart.Visibility == Visibility.Visible)
				PrepareHitTestableElements(radius);
		}
		internal ChartHitInfo(RefinedPoint refinedPoint, Series series3D) {
			this.refinedPoint = refinedPoint;
			this.series3D = series3D;
		}
		internal ChartHitInfo(Diagram3D diagram) {
			this.diagram = diagram;
		}
		internal ChartHitInfo(Axis3D axis) {
			this.axis = axis;
		}
		void AddUniqueHitTestableElement(IHitTestableElement hitTestableElement) {
			if (hitTestableElement != null && !hitTestableElements.Contains(hitTestableElement))
				hitTestableElements.Add(hitTestableElement);
		}
		void PrepareHitTestableElements(double radius) {
			if (radius <= 0)
				VisualTreeHelper.HitTest(chart, null, OnHitTestResult, new PointHitTestParameters(point));
			else {
				double side = 2 * radius;
				Geometry square = new RectangleGeometry(new Rect(point.X - radius, point.Y - radius, side, side));
				VisualTreeHelper.HitTest(chart, null, OnHitTestResult, new GeometryHitTestParameters(square));
			}
		}
		object GetElementByType(Type elementType, bool isAdditionalElement) {
			foreach (IHitTestableElement hitTestableElement in hitTestableElements) {
				object element = isAdditionalElement ? hitTestableElement.AdditionalElement : hitTestableElement.Element;
				if (element != null && elementType.IsAssignableFrom(element.GetType()))
					return element;
			}
			return null;
		}
		object GetElementByType(Type elementType) {
			return GetElementByType(elementType, false);
		}
		object GetAdditionalElementByType(Type elementType) {
			return GetElementByType(elementType, true);
		}
		HitTestResultBehavior OnHitTestResult(HitTestResult result) {
			AddUniqueHitTestableElement(HitTestingHelper.GetParentHitTestableElement(result.VisualHit));
			return HitTestResultBehavior.Continue;
		}
	}
}
