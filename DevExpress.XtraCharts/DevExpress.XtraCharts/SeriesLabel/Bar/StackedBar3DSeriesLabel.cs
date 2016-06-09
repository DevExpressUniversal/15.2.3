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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(StackedBar3DSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class StackedBar3DSeriesLabel : BarSeriesLabel {	  
		protected internal override bool DefaultVisible { get { return false; } }
		protected internal override bool ShadowSupported { get { return false; } }
		protected internal override bool ConnectorSupported { get { return false; } }
		protected internal override bool ConnectorEnabled { get { return false; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new int LineLength { get { return 0; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new bool LineVisible { get { return false; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new Color LineColor { get { return Color.Empty; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden)
		]
		public new LineStyle LineStyle { get { return null; } }
		[
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
			get { return BarSeriesLabelPosition.Center; }
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
		public StackedBar3DSeriesLabel() : base() {
		}
		protected override ChartElement CreateObjectForClone() {
			return new StackedBar3DSeriesLabel();
		}
		protected internal override void CalculateLayout(SeriesLabelLayoutList labelLayoutList, RefinedPointData pointData, TextMeasurer textMeasurer) {
			XYDiagram3DSeriesLabelLayoutList xyLabelLayoutList = labelLayoutList as XYDiagram3DSeriesLabelLayoutList;
			if (xyLabelLayoutList == null) {
				ChartDebug.Fail("XYDiagram3DSeriesLabelsLayout expected.");
				return;
			}
			StackedBar3DSeriesView view = Series.View as StackedBar3DSeriesView;
			if (view == null) {
				ChartDebug.Fail("StackedBar3DSeriesView expected.");
				return;
			}
			Bar3DData barData = view.CreateBarData(pointData.RefinedPoint);
			if (barData.ActualValue == barData.ZeroValue && !ShowForZeroValues)
				return;
			XYDiagram3DCoordsCalculator coordsCalculator = xyLabelLayoutList.CoordsCalculator;
			double barWidth, barDepth;
			view.CalcWidthAndDepth(coordsCalculator, barData.Width, out barWidth, out barDepth);
			MinMaxValues minMaxValues = new MinMaxValues(Math.Min(barData.ZeroValue, barData.ActualValue), Math.Max(barData.ZeroValue, barData.ActualValue));
			minMaxValues.Intersection(xyLabelLayoutList.AxisRangeY);
			double centerValue = minMaxValues.CalculateCenter();
			if (!Double.IsNaN(centerValue)) {
				DiagramPoint bottom = coordsCalculator.GetDiagramPoint(Series, barData.DisplayArgument, 0, false);
				DiagramPoint point = coordsCalculator.GetDiagramPoint(Series, barData.DisplayArgument, centerValue, false);
				DiagramPoint top = coordsCalculator.GetDiagramPoint(Series, barData.DisplayArgument, barData.MaxValue, false);
				double newBarDepth = !view.IsFlatTopModel ? barDepth * (top.Y - point.Y) / (top.Y - bottom.Y) : barDepth;
				point.Z += ((coordsCalculator.CalcVisiblePlanes() & BoxPlane.Back) > 0 ? 1 : -1) * newBarDepth / 2.0;
				point.X += barData.FixedOffset;
				SeriesLabelViewData labelViewData = pointData.LabelViewData[0];
				TextPainter textPainter = labelViewData.CreateTextPainterForCenterDrawing(this, textMeasurer, coordsCalculator.Project(point));
				XYDiagramSeriesLabelLayout layout = XYDiagramSeriesLabelLayout.Create(pointData, pointData.DrawOptions.Color,
					textPainter, null, ResolveOverlappingMode, point);
				xyLabelLayoutList.AddLabelLayout(layout);
			}
		}
	}
	[
	TypeConverter(typeof(FullStackedBar3DSeriesLabelTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class FullStackedBar3DSeriesLabel : StackedBar3DSeriesLabel {
		public FullStackedBar3DSeriesLabel() : base() {
		}
	}
}
