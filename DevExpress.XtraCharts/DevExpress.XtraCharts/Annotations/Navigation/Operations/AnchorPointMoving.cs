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
namespace DevExpress.XtraCharts.Native {
	public class ChartAnchorPointMoving : AnnotationOperation {
		ChartAnchorPoint AnchorPoint { get { return Annotation.AnchorPoint as ChartAnchorPoint; } }
		public ChartAnchorPointMoving(Annotation annotation)
			: base(annotation) {
		}
		public override void Run(int x, int y, int dx, int dy) {
			if (Annotation == null || x < 0 || y < 0)
				return;
			AnchorPoint.SetPosition(AnchorPoint.X + dx, AnchorPoint.Y + dy);
		}
	}
	public class PaneAnchorPointMoving : AnnotationOperation {
		class AnchorPointParams {
			XYDiagramPaneBase pane = null;
			Axis2D axisX = null;
			object axisXValue = null;
			Axis2D axisY = null;
			object axisYValue = null;
			public bool IsEmpty { get { return Pane == null || AxisX == null || AxisY == null || AxisXValue == null || AxisYValue == null; } }
			public XYDiagramPaneBase Pane { get { return pane; } set { pane = value; } }
			public Axis2D AxisX { get { return axisX; } set { axisX = value; } }
			public object AxisXValue { get { return axisXValue; } set { axisXValue = value; } }
			public Axis2D AxisY { get { return axisY; } set { axisY = value; } }
			public object AxisYValue { get { return axisYValue; } set { axisYValue = value; } }
		}
		PaneAnchorPoint AnchorPoint { get { return Annotation.AnchorPoint as PaneAnchorPoint; } }
		public PaneAnchorPointMoving(Annotation annotation)
			: base(annotation) {
		}
		AnchorPointParams GetAnchorPointParamsByCoordinates(DiagramCoordinates coordinates) {
			AnchorPointParams result = new AnchorPointParams();
			if (!coordinates.IsEmpty) {
				AxisValue axisXValue, axisYValue;
				if (AnchorPoint.Pane == coordinates.Pane) {
					result.Pane = AnchorPoint.Pane;
					axisXValue = coordinates.GetAxisValue(AnchorPoint.AxisXCoordinate.Axis);
					if (axisXValue == null) {
						result.AxisX = coordinates.AxisX as Axis2D;
						axisXValue = coordinates.GetAxisValue(result.AxisX);
					}
					else
						result.AxisX = AnchorPoint.AxisXCoordinate.Axis;
					axisYValue = coordinates.GetAxisValue(AnchorPoint.AxisYCoordinate.Axis);
					if (axisYValue == null) {
						result.AxisY = coordinates.AxisY as Axis2D;
						axisYValue = coordinates.GetAxisValue(result.AxisY);
					}
					else
						result.AxisY = AnchorPoint.AxisYCoordinate.Axis;
				}
				else {
					result.Pane = coordinates.Pane;
					result.AxisX = coordinates.AxisX as Axis2D;
					result.AxisY = coordinates.AxisY as Axis2D;
					axisXValue = coordinates.GetAxisValue(result.AxisX);
					axisYValue = coordinates.GetAxisValue(result.AxisY);
				}				 
				if (axisXValue != null)
					switch (axisXValue.ScaleType) {
						case ScaleType.Numerical:
						result.AxisXValue = axisXValue.NumericalValue;
						break;
						case ScaleType.Qualitative:
						result.AxisXValue = axisXValue.QualitativeValue;
						break;
						case ScaleType.DateTime:
						result.AxisXValue = axisXValue.DateTimeValue;
						break;
					}
				if(axisYValue != null)
					switch (axisYValue.ScaleType) {
						case ScaleType.Numerical:
						result.AxisYValue = axisYValue.NumericalValue;
						break;
						case ScaleType.Qualitative:
						result.AxisYValue = axisYValue.QualitativeValue;
						break;
						case ScaleType.DateTime:
						result.AxisYValue = axisYValue.DateTimeValue;
						break;
					}
			}
			return result;
		}
		public override void Run(int x, int y, int dx, int dy) {
			if (Annotation == null || x < 0 || y < 0)
				return;
			XYDiagram2D diagram = Annotation.ChartContainer.Chart == null ? null :
				Annotation.ChartContainer.Chart.Diagram as XYDiagram2D;
			if (diagram == null)
				return;
			AnchorPointParams pointParams = GetAnchorPointParamsByCoordinates(diagram.PointToDiagram(new Point(x + dx, y + dy)));
			if (!pointParams.IsEmpty)
				AnchorPoint.SetPosition(pointParams.Pane, pointParams.AxisX, pointParams.AxisY, pointParams.AxisXValue, pointParams.AxisYValue);
		}
	}
}
