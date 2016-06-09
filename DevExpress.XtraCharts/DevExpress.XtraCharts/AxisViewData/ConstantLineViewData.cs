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
using System.Drawing.Drawing2D;
namespace DevExpress.XtraCharts.Native {
	public class ConstantLineViewData {
		readonly ConstantLine constantLine;
		readonly DiagramPoint point1;
		readonly DiagramPoint point2;
		readonly RotatedTextPainterNearLine titlePainter;
		HitTestController HitTestController { get { return constantLine.Axis.Diagram.Chart.HitTestController; } } 
		public ConstantLine ConstantLine { get { return constantLine; } }
		public DiagramPoint Point1 { get { return point1; } }
		public DiagramPoint Point2 { get { return point2; } }
		public RotatedTextPainterNearLine TitlePainter { get { return titlePainter; } }
		public ConstantLineViewData(TextMeasurer textMeasurer, ConstantLine constantLine, AxisIntervalMapping intervalMapping, double value) {
			this.constantLine = constantLine;
			point1 = intervalMapping.GetNearScreenPoint(value, 0, 0);
			point2 = intervalMapping.GetFarScreenPoint(value, 0, 0);
			ConstantLineTitle title = constantLine.Title;
			string titleText = title.ActualText;
			if (title.ActualVisibility && !String.IsNullOrEmpty(titleText)) {
				Axis2D axis = constantLine.Axis;
				SizeF titleTextSize = textMeasurer.MeasureString(titleText, title.Font);
				int selfOffset = MathUtils.Ceiling((double)constantLine.LineStyle.Thickness / 2);
				if (title.ShowBelowLine ^ (axis.ActualReverse && ((XYDiagram2D)axis.Diagram).ActualRotated))
					selfOffset = -selfOffset;
				int offset = MathUtils.Ceiling(titleTextSize.Width / 2.0) + 1;
				Point location = title.Alignment == ConstantLineTitleAlignment.Far ?
					(Point)intervalMapping.GetFarScreenPoint(value, selfOffset, -offset) :
					(Point)intervalMapping.GetNearScreenPoint(value, selfOffset, offset);
				titlePainter = new RotatedTextPainterNearLine(location, titleText, titleTextSize, title, 
					axis.GetConstantLineTitleNearTextPosition(title.ShowBelowLine), axis.GetConstantLineTitleActualAngle(), true);
			}
		}
		IHitRegion CalcHitRegion(HitRegionContainer paneHitContainer) {
			using (Pen pen = new Pen(Color.Empty, constantLine.LineStyle.Thickness + 2)) {
				GraphicsPath path = new GraphicsPath();
				path.AddLine((Point)point1, (Point)point2);
				if (path.PointCount != 0 && point1 != point2)
					path.Widen(pen);
				HitRegionContainer hitContainer = new HitRegionContainer(new HitRegion(path));
				hitContainer.Intersect(paneHitContainer);
				return hitContainer.Underlying;
			}
		}
		public void Render(IRenderer renderer, HitRegionContainer paneHitContainer) {
			HitTestState hitTestState = ((IHitTest)constantLine).State;
			LineStyle lineStyle = constantLine.LineStyle;
			int thickness = hitTestState.Normal ? lineStyle.Thickness : GraphicUtils.CorrectThicknessByHitTestState(lineStyle.Thickness, hitTestState, 1);
			renderer.EnableAntialiasing(lineStyle.AntiAlias);
			renderer.DrawLine((Point)point1, (Point)point2, GraphicUtils.CorrectColorByHitTestState(constantLine.ActualColor, hitTestState), thickness, lineStyle, LineCap.Flat);
			renderer.RestoreAntialiasing();
			renderer.ProcessHitTestRegion(HitTestController, constantLine, null, CalcHitRegion(paneHitContainer));
		}
		public void RenderTitle(IRenderer renderer) {
			if (titlePainter != null)
				titlePainter.Render(renderer, HitTestController);
		}
	}
}
