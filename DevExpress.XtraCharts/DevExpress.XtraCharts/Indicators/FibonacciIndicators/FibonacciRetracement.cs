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
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native{
	public class FibonacciRetracementBehavior : FibonacciLinesBehavior {
		protected override bool Antialiasing { get { return false; } }
		public override bool DefaultShowLevel0 { get { return true; } }
		public override bool DefaultShowLevel100 { get { return true; } }
		public override bool DefaultShowLevel23_6 { get { return true; } }
		public override bool DefaultShowLevel76_4 { get { return false; } }
		public override bool DefaultShowAdditionalLevels { get { return false; } }
		public override bool ShowLevel0PropertyEnabled { get { return false; } }
		public override bool ShowLevel100PropertyEnabled { get { return false; } }
		public override bool ShowAdditionalLevelsPropertyEnabled { get { return true; } }
		public FibonacciRetracementBehavior(FibonacciIndicator fibonacciIndicator) 
			: base(fibonacciIndicator) { }
		protected override IList<FibonacciLine> CalculateLines(IList<double> levels) {
			GRealPoint2D point1 = (GRealPoint2D)MinPoint;
			GRealPoint2D point2 = (GRealPoint2D)MaxPoint;
			point2.X = Math.Max(point1.X, FibonacciIndicator.View.ActualAxisX.VisualRangeData.Max);
			FibonacciRetracementCalculator calculator = new FibonacciRetracementCalculator();
			return calculator.CalculateLines(point1, point2, levels);
		}
		protected override RotatedTextPainterNearCircleTangent CreateLabelPainter(XYDiagramMappingBase diagramMapping, FibonacciLine line, string text, SizeF size, int thickness) {
			int halfWidth = MathUtils.Ceiling(size.Width / 2.0);
			int halfThickness = MathUtils.Ceiling(thickness / 2.0);
			int x = MathUtils.StrongRound(line.End.X);
			int y = MathUtils.StrongRound(line.End.Y);
			int angle;
			if (diagramMapping.Rotated) {
				angle = 90;
				int roundedStart = MathUtils.StrongRound(line.Start.Y);
				x += halfThickness;
				if (diagramMapping.XReverse) {
					y -= halfWidth + 2;
					int limit = roundedStart + halfWidth;
					if (y < limit)
						y = limit;
				}
				else {
					y += halfWidth + 1;
					int limit = roundedStart - halfWidth;
					if (y > limit)
						y = limit;
				}
			}
			else {
				angle = 0;
				int roundedStart = MathUtils.StrongRound(line.Start.X);
				y -= halfThickness;
				if (diagramMapping.XReverse) {
					x += halfWidth + 1;
					int limit = roundedStart - halfWidth;
					if (x > limit)
						x = limit;
				}
				else {
					x -= halfWidth + 2;
					int limit = roundedStart + halfWidth;
					if (x < limit)
						x = limit;
				}
			}
			return new RotatedTextPainterNearCircleTangent(angle, new Point(x, y), text, size, FibonacciIndicator.Label, true);
		}
	}
}
