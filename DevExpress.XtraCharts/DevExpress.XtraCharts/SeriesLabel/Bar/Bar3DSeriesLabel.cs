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
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(Bar3DSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class Bar3DSeriesLabel : BarSeriesLabel {
		protected internal override bool DefaultVisible { get { return false; } }
		protected internal override bool ShadowSupported { get { return false; } }
		protected internal override bool ConnectorSupported { get { return true; } }
		protected internal override bool ConnectorEnabled { get { return true; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("Bar3DSeriesLabelShadow"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		NonTestableProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Shadow Shadow { get { return base.Shadow; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new BarSeriesLabelPosition Position {
			get { return BarSeriesLabelPosition.Top; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int Indent {
			get { return 0; }
		}
		public Bar3DSeriesLabel() : base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new Bar3DSeriesLabel();
		}
		protected internal override void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
			XYDiagram3DSeriesLabelLayoutList xyLabelLayoutList = labelLayoutList as XYDiagram3DSeriesLabelLayoutList;
			if(xyLabelLayoutList == null) {
				ChartDebug.Fail("XYDiagram3DSeriesLabelsLayout expected.");
				return;
			}
			Bar3DSeriesView view = Series.View as Bar3DSeriesView;
			if(view == null) {
				ChartDebug.Fail("Bar3DSeriesView expected.");
				return;
			}
			Bar3DData barData = view.CreateBarData(pointData.RefinedPoint);
			if (barData.ActualValue == barData.ZeroValue && !ShowForZeroValues)
				return;
			XYDiagram3DCoordsCalculator coordsCalculator = xyLabelLayoutList.CoordsCalculator;
			DiagramPoint startPoint = coordsCalculator.GetDiagramPoint(Series, barData.DisplayArgument, Math.Max(barData.ZeroValue, barData.ActualValue), false);
			startPoint.X += barData.FixedOffset;
			DiagramPoint finishPoint = CalculateFinishPoint(-Math.PI / 2.0, startPoint);
			DiagramPoint anchorPoint = coordsCalculator.Project(finishPoint);
			double angle = coordsCalculator.CalcConnectorAngle(new DiagramVector(0, 1, 0));
			SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
			TextPainter textPainter = labelViewData.CreateTextPainterForFlankDrawing(this, textMeasurer, anchorPoint, angle);
			ConnectorPainterBase connectorPainter;
			if (ActualLineVisible) {
				bool resolveOverlapping = ResolveOverlappingMode != ResolveOverlappingMode.None;
				if (resolveOverlapping) {
					startPoint = coordsCalculator.Project(startPoint);
					finishPoint = anchorPoint;
				}
				connectorPainter = new LineConnectorPainter(startPoint, finishPoint, angle, (ZPlaneRectangle)textPainter.BoundsWithBorder, resolveOverlapping);
			}
			else
				connectorPainter = null;
			XYDiagramSeriesLabelLayout layout = XYDiagramSeriesLabelLayout.Create(pointData,
				pointData.DrawOptions.Color, textPainter, connectorPainter, ResolveOverlappingMode, startPoint);
			xyLabelLayoutList.AddLabelLayout(layout);
		}
	}
}
