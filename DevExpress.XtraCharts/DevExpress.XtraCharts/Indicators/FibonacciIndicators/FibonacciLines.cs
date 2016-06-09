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
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public abstract class FibonacciLinesBehavior : FibonacciIndicatorBehavior {
		protected abstract bool Antialiasing { get; }
		public FibonacciLinesBehavior(FibonacciIndicator fibonacciIndicator) : base(fibonacciIndicator) { 
		}
		IList<FibonacciLine> CalculateScreenLines(XYDiagramMappingBase diagramMapping, IList<FibonacciLine> lines) {
			List<FibonacciLine> screenLines = new List<FibonacciLine>(lines.Count);
			foreach (FibonacciLine line in lines) {
				DiagramPoint screenStartPoint = diagramMapping.GetScreenPointNoRound(line.Start.X, line.Start.Y);
				DiagramPoint screenEndPoint = diagramMapping.GetScreenPointNoRound(line.End.X, line.End.Y);
				screenLines.Add(new FibonacciLine(line.Level, (GRealPoint2D)screenStartPoint, (GRealPoint2D)screenEndPoint));
			}
			return screenLines;
		}
		IEnumerable<FibonacciLine> TruncateScreenLines(IList<FibonacciLine> lines, Rectangle bounds) {
			List<FibonacciLine> truncatedLines = new List<FibonacciLine>(lines.Count);
			foreach (FibonacciLine line in lines) {
				GRealLine2D? truncation = TruncatedLineCalculator.Truncate(line.Start, line.End, bounds, LineKind.Ray);
				if (truncation.HasValue) {
					GRealLine2D truncated = truncation.Value;
					truncatedLines.Add(new FibonacciLine(line.Level, truncated.Start, truncated.End));
				}
			}
			return truncatedLines;
		}
		IEnumerable<RotatedTextPainterNearCircleTangent> CalculateLabels(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer, IList<FibonacciLine> lines, int thickness, Color color) {
			Font font = FibonacciIndicator.Label.Font;
			List<RotatedTextPainterNearCircleTangent> labels = new List<RotatedTextPainterNearCircleTangent>(lines.Count);
			foreach (FibonacciLine line in lines) {
				string text = ConstructLevelText(line.Level);
				labels.Add(CreateLabelPainter(diagramMapping, line, text, textMeasurer.MeasureString(text, font), thickness));
			}
			return labels;
		}
		protected abstract IList<FibonacciLine> CalculateLines(IList<double> levels);
		protected abstract RotatedTextPainterNearCircleTangent CreateLabelPainter(XYDiagramMappingBase diagramMapping, FibonacciLine line, string text, SizeF size, int thickness);
		public override IndicatorLayout CreateLayout(XYDiagramMappingBase diagramMapping, TextMeasurer textMeasurer) {
			if (!Visible)
				return null;
			FibonacciIndicator fibonacciIndicator = FibonacciIndicator;
			IList<FibonacciLine> lines = CalculateScreenLines(diagramMapping, CalculateLines(GetLevels()));
			IList<FibonacciLine> baseLevelLines = CalculateScreenLines(diagramMapping, CalculateLines(GetBaseLevels()));
			Rectangle bounds = diagramMapping.Bounds;
			IEnumerable<FibonacciLine> truncatedLines = TruncateScreenLines(lines, bounds);
			IEnumerable<FibonacciLine> truncatedBaseLevelLines = TruncateScreenLines(baseLevelLines, bounds);
			FibonacciIndicatorLabelsLayout labelsLayout;
			FibonacciIndicatorLabel label = fibonacciIndicator.Label;
			if (label.ActualVisibility) {
				IEnumerable<RotatedTextPainterNearCircleTangent> labels = CalculateLabels(diagramMapping, 
					textMeasurer, lines, fibonacciIndicator.LineStyle.Thickness, label.ActualTextColor);
				IEnumerable<RotatedTextPainterNearCircleTangent> baseLevelsLabels = CalculateLabels(diagramMapping, 
					textMeasurer, baseLevelLines, fibonacciIndicator.BaseLevelLineStyle.Thickness, label.ActualBaseLevelTextColor);
				labelsLayout = new FibonacciIndicatorLabelsLayout(label, labels, baseLevelsLabels);
			}
			else
				labelsLayout = null;
			return new FibonacciLinesLayout(fibonacciIndicator, labelsLayout, truncatedLines, truncatedBaseLevelLines, Antialiasing);
		}
	}
	public class FibonacciLinesLayout : FibonacciIndicatorLayout {
		static GraphicsPath CreateGraphicsPathBase(IEnumerable<FibonacciLine> lines, int thickness, LineStyle lineStyle) {
			GraphicsPath path = new GraphicsPath();
			foreach (FibonacciLine line in lines) {
				Point start = line.Start.ToPoint();
				Point end = line.End.ToPoint();
				if (end != start) {
					path.AddLine(start, end);
					path.CloseFigure();
				}
			}
			RectangleF bounds = path.GetBounds();
			if (bounds.Width < 0.5 && bounds.Height < 0.5) {
				path.Dispose();
				return null;
			}
			using (Pen pen = new Pen(Color.Empty, thickness)) {
				if (lineStyle != null)
					pen.LineJoin = lineStyle.LineJoin;
				path.Widen(pen);
			}
			return path;
		}
		readonly IEnumerable<FibonacciLine> lines;
		readonly IEnumerable<FibonacciLine> baseLevelsLines;
		readonly bool antialiasing;
		protected override void RenderBase(IRenderer renderer) {
			FibonacciIndicator fibonacciIndicator = FibonacciIndicator;
			LineStyle lineStyle = fibonacciIndicator.LineStyle;
			Color color = Color;
			int thickness = Thickness;
			renderer.EnableAntialiasing(antialiasing);
			foreach (FibonacciLine line in lines)
				renderer.DrawLine(line.Start.ToPoint(), line.End.ToPoint(), color, thickness, lineStyle, LineCap.Flat);
			renderer.RestoreAntialiasing();
			lineStyle = fibonacciIndicator.BaseLevelLineStyle;
			color = BaseLevelColor;
			thickness = BaseLevelThickness;
			renderer.EnableAntialiasing(antialiasing);
			foreach (FibonacciLine baseLevelLine in baseLevelsLines)
				renderer.DrawLine(baseLevelLine.Start.ToPoint(), baseLevelLine.End.ToPoint(), color, thickness, lineStyle, LineCap.Flat);
			renderer.RestoreAntialiasing();
		}
		public FibonacciLinesLayout(FibonacciIndicator fibonacciIndicator, FibonacciIndicatorLabelsLayout labelsLayout, IEnumerable<FibonacciLine> lines, IEnumerable<FibonacciLine> baseLevelsLines, bool antialiasing) : base(fibonacciIndicator, labelsLayout) {
			this.lines = lines;
			this.baseLevelsLines = baseLevelsLines;
			this.antialiasing = antialiasing;
		}
		public override GraphicsPath CalculateHitTestGraphicsPath() {
			GraphicsPath path = CreateGraphicsPathBase(lines, Thickness, Indicator.LineStyle);
			GraphicsPath baseLevelsPath = CreateGraphicsPathBase(baseLevelsLines, BaseLevelThickness, Indicator.LineStyle);
			if (path == null)
				return baseLevelsPath;
			if (baseLevelsPath != null)
				using (baseLevelsPath)
					path.AddPath(baseLevelsPath, false);
			return path;
		}
	}
}
