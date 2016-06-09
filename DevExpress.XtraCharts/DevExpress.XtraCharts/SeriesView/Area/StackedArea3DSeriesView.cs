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
using System.ComponentModel.Design.Serialization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(StackedArea3DSeriesViewTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StackedArea3DSeriesView : Area3DSeriesView, IStackedView {
		protected internal override string StringId { get { return ChartLocalizer.GetString(ChartStringId.SvnStackedArea3D); } }
		protected override bool NeedSeriesInteraction { get { return true; } }
		protected internal override bool DateTimeValuesSupported { get { return false; } }
		protected virtual double GetAnnotationValue(MinMaxValues values, IAxisRangeData axisRangeY) {
			return values.Max;
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new StackedAreaGeometryStripCreator();
		}
		protected override DiagramPoint? CalculateAnnotationAnchorPoint(XYDiagram3DCoordsCalculator coordsCalculator, RefinedPointData pointData, IAxisRangeData axisRangeY) {
			double value = GetAnnotationValue(GetSeriesPointValues(pointData.RefinedPoint), axisRangeY);
			if (Double.IsNaN(value))
				return null;
			DiagramPoint point = coordsCalculator.GetDiagramPoint(Series, ((IXYPoint)pointData.RefinedPoint).Argument, value, false);
			point.Z += ((coordsCalculator.CalcVisiblePlanes() & BoxPlane.Back) > 0 ? 1 : -1) * coordsCalculator.CalcSeriesWidth(this) / 2.0;
			return coordsCalculator.Project(point);
		}
		protected override ChartElement CreateObjectForClone() {
			return new StackedArea3DSeriesView();
		}
		protected override SeriesContainer CreateContainer() {
			return new StackedInteractionContainer(this, true);
		}
		protected internal override MinMaxValues GetSeriesPointValues(RefinedPoint refinedPoint) {
			return new MinMaxValues(((IStackedPoint)refinedPoint).MinValue, ((IStackedPoint)refinedPoint).MaxValue);
		}
		protected internal override SeriesLabelBase CreateSeriesLabel() {
			return new StackedArea3DSeriesLabel();
		}
	}
}
