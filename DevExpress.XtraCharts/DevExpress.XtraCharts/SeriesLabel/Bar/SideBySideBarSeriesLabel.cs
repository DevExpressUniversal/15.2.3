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
using System.Drawing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(SideBySideBarSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class SideBySideBarSeriesLabel : BarSeriesLabel {
		protected internal override bool ShadowSupported { get { return true; } }
		protected internal override bool ConnectorSupported { get { return true; } }
		protected internal override bool ConnectorEnabled { get { return Position == BarSeriesLabelPosition.Top || Position == BarSeriesLabelPosition.Auto; } }
		public SideBySideBarSeriesLabel() : base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new SideBySideBarSeriesLabel();
		}
		XYDiagramSeriesLabelLayout CalculateLayoutForTopPosition(XYDiagramSeriesLabelLayoutList labelLayoutList, BarData barData, RefinedPointData pointData, SeriesLabelViewData labelViewData, Color color, TextMeasurer textMeasurer) {
			SideBySideBarSeriesView view = Series.View as SideBySideBarSeriesView;
			if (view == null) {
				ChartDebug.Fail("SideBySideBarSeriesView expected.");
				return null;
			}
			XYDiagramMappingBase diagramMapping = view.IsScrollingEnabled ?
				labelLayoutList.MappingContainer.MappingForScrolling : barData.GetMappingForTopLabelPosition(labelLayoutList.MappingContainer);
			if(diagramMapping == null)
				return null;
			double angle = XYDiagramMappingHelper.CorrectAngle(diagramMapping, barData.ActualValue < 0 ? -Math.PI / 2 : Math.PI / 2);
			DiagramPoint startPoint = barData.GetScreenPoint(barData.Argument, barData.ActualValue, diagramMapping);
			DiagramPoint finishPoint = CalculateFinishPoint(angle, startPoint);
			TextPainter painter = labelViewData.CreateTextPainterForFlankDrawing(this, textMeasurer, finishPoint, angle);
			return XYDiagramSeriesLabelLayout.Create(pointData, color, painter,
				ActualLineVisible ? new LineConnectorPainter(startPoint, finishPoint, angle, (ZPlaneRectangle)painter.BoundsWithBorder, true) : null, 
				ResolveOverlappingMode, startPoint);
		}
		protected internal override void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
			XYDiagramSeriesLabelLayoutList xyLabelLayoutList = labelLayoutList as XYDiagramSeriesLabelLayoutList;
			if(xyLabelLayoutList == null) {
				ChartDebug.Fail("XYDiagramSeriesLabelsLayout expected.");
				return;
			}
			SideBySideBarSeriesView view = Series.View as SideBySideBarSeriesView;
			if(view == null) {
				ChartDebug.Fail("SideBySideBarSeriesView expected.");
				return;
			}
			BarData barData = pointData.GetBarData(view);
			if (barData.ActualValue == 0.0 && !ShowForZeroValues)
				return;
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			BarSeriesLabelPosition actualPosition = Position;
			if (actualPosition == BarSeriesLabelPosition.Auto) {
				RectangleF bounds = barData.GetTotalRect(xyLabelLayoutList.MappingContainer);
				Size labelSize = labelViewData.TextSize;
				if (Border.ActualVisibility) {
					int doubleBorderThickness = 2 * Border.ActualThickness;
					labelSize = new Size(labelSize.Width + doubleBorderThickness, labelSize.Height + doubleBorderThickness);
				}
				actualPosition = bounds.Height > labelSize.Height ? BarSeriesLabelPosition.Center : BarSeriesLabelPosition.Top;
			}
			Color color = pointData.DrawOptions.Color;
			XYDiagramSeriesLabelLayout layout =
				actualPosition != BarSeriesLabelPosition.Top ?
				SeriesLabelHelper.CalculateLayoutForInsideBarPosition(xyLabelLayoutList, textMeasurer, barData, pointData, labelViewData, actualPosition, Indent, color) :
				CalculateLayoutForTopPosition(xyLabelLayoutList, barData, pointData, labelViewData, color, textMeasurer);
			if(layout != null)
				xyLabelLayoutList.Add(layout);
		}
		protected internal override void AssignBarLabelPosition(BarSeriesLabel label) {
			SideBySideBarSeriesLabel sideBySideLabel = label as SideBySideBarSeriesLabel;
			if (sideBySideLabel == null)
				return;
			Position = sideBySideLabel.Position;
		}
	}	 
}
