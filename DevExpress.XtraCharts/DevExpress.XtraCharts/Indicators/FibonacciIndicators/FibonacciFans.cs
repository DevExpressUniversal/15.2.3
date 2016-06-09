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
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class FibonacciFansBehavior : FibonacciLinesBehavior {
		public override bool DefaultShowLevel0 { get { return false; } }
		public override bool DefaultShowLevel100 { get { return false; } }
		public override bool DefaultShowLevel23_6 { get { return false; } }
		public override bool DefaultShowLevel76_4 { get { return false; } }
		public override bool DefaultShowAdditionalLevels { get { return false; } }
		public override bool ShowLevel0PropertyEnabled { get { return true; } }
		public override bool ShowLevel100PropertyEnabled { get { return false; } }
		public override bool ShowAdditionalLevelsPropertyEnabled { get { return false; } }
		protected override bool Antialiasing { get { return true; } }
		public FibonacciFansBehavior(FibonacciIndicator fibonacciIndicator) 
			: base(fibonacciIndicator) { }
		protected override IList<FibonacciLine> CalculateLines(IList<double> levels) {
			GRealPoint2D point1 = (GRealPoint2D)MinPoint;
			GRealPoint2D point2 = (GRealPoint2D)MaxPoint;
			double maxAxisXCoord = FibonacciIndicator.View.ActualAxisX.VisualRangeData.Max;
			FibonacciFansCalculator calculator = new FibonacciFansCalculator();
			return calculator.CalculateLines(point1, point2, levels);
		}
		protected override RotatedTextPainterNearCircleTangent CreateLabelPainter(XYDiagramMappingBase diagramMapping, FibonacciLine line, string text, SizeF size, int thickness) {
			DiagramPoint start = line.Start.ToDiagramPoint();
			DiagramPoint end = line.End.ToDiagramPoint();
			double endX = end.X;
			double endY = end.Y;
			double angle = Math.Atan2(endY - start.Y, endX - start.X) / Math.PI * 180;
			double offset = thickness / 2.0;
			return new RotatedTextPainterNearCircleTangent((float)angle, 
				new Point(MathUtils.StrongRound(endX + offset * Math.Sign(angle)), MathUtils.StrongRound(endY - offset)), 
				text, size, FibonacciIndicator.Label, true);
		}
	}
}
