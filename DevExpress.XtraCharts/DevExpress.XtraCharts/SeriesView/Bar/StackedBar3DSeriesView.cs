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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(StackedBar3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StackedBar3DSeriesView : Bar3DSeriesView, IStackedView {
		protected override bool NeedSeriesInteraction { get { return true; } }
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnStackedBar3D); } }
		protected internal override bool DateTimeValuesSupported { get { return false; } }
		protected override Type PointInterfaceType { get { return typeof(IStackedPoint); } }
		protected override double GetRefinedPointMax(RefinedPoint point) {
			IXYPoint xyPoint = (IXYPoint)point;
			return xyPoint.Value;
		}
		protected override double GetRefinedPointMin(RefinedPoint point) {
			IXYPoint xyPoint = (IXYPoint)point;
			return xyPoint.Value;
		}
		protected override double GetRefinedPointAbsMin(RefinedPoint point) {
			return Math.Abs(((IXYPoint)point).Value);
		}
		protected override DiagramPoint? CalculateAnnotationAnchorPoint(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData, IAxisRangeData axisRangeY) {
			Bar3DData barData = CreateBarData(pointData.RefinedPoint);
			double barWidth, barDepth;
			CalcWidthAndDepth(coordsCalculator, barData.Width, out barWidth, out barDepth);
			double centerValue = MinMaxValues.Intersection(axisRangeY,new MinMaxValues(barData.ZeroValue, barData.ActualValue)).CalculateCenter();
			if (Double.IsNaN(centerValue))
				return null;
			DiagramPoint bottom = coordsCalculator.GetDiagramPoint(Series, barData.DisplayArgument, 0, false);
			DiagramPoint point = coordsCalculator.GetDiagramPoint(Series, barData.DisplayArgument, centerValue, false);
			DiagramPoint top = coordsCalculator.GetDiagramPoint(Series, barData.DisplayArgument, barData.MaxValue, false);
			double newBarDepth = !IsFlatTopModel ? barDepth * (top.Y - point.Y) / (top.Y - bottom.Y) : barDepth;
			point.Z += ((coordsCalculator.CalcVisiblePlanes() & BoxPlane.Back) > 0 ? 1 : -1) * newBarDepth / 2.0;
			point.X += barData.FixedOffset;
			return coordsCalculator.Project(point);
		}
		protected override ChartElement CreateObjectForClone() {
			return new StackedBar3DSeriesView();
		}
		protected override SeriesPointLayout CalculateSeriesPointLayoutInternal(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData, Bar3DData barData) {
			return barData.ActualValue == barData.ZeroValue ? null : base.CalculateSeriesPointLayoutInternal(coordsCalculator, pointData, barData);
		}
		protected override SeriesContainer CreateContainer() {
			return new StackedInteractionContainer(this);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new StackedBar3DSeriesLabel();
		}
		protected override double GetMinValue(IXYPoint refinedPoint) {
			return ((IStackedPoint)refinedPoint).MinValue;
		}
		protected override double GetMaxValue(IXYPoint refinedPoint) {
			return ((IStackedPoint)refinedPoint).MaxValue;
		}
		protected override double GetTotalMaxValue(IXYPoint refinedPoint) {
			return ((IStackedPoint)refinedPoint).TotalValue;
		}
	}
}
