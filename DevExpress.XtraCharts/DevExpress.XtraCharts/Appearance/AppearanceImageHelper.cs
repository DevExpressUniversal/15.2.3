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
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public static class AppearanceImageHelper {
		struct Line {
			readonly Point p1;
			readonly Point p2;
			public Point P1 { get { return p1; } }
			public Point P2 { get { return p2; } }
			public Line(int x1, int y1, int x2, int y2) {
				p1 = new Point(x1, y1);
				p2 = new Point(x2, y2);
			}
		}
		const int line3Ddelta1 = 5;
		const int line3Ddelta2 = 2;
		const int area3DDeltaAxis = 6;
		static readonly Rectangle chartRect = new Rectangle(4, 4, 72, 42);
		static readonly RectangleF diagramRect = new RectangleF(4, 4, 72, 42);
		static readonly RectangleF diagram3DRect = new RectangleF(7, 7, 66, 36);
		static readonly PointF radarDiagramCenter = new PointF(40, 25);
		static readonly RectangleF[] sideBySideRects = new RectangleF[] {
			new RectangleF(12, 7, 8, 36), 
			new RectangleF(20, 25, 8, 18),
			new RectangleF(28, 30, 8, 13),
			new RectangleF(44, 25, 8, 18),
			new RectangleF(52, 7, 8, 36),
			new RectangleF(60, 30, 8, 13)
		};
		static readonly RectangleF[] sideBySideStackedRects = new RectangleF[] {
			new RectangleF(12, 28, 12, 15),
			new RectangleF(12, 14, 12, 14),
			new RectangleF(24, 22, 12, 21),
			new RectangleF(24, 8, 12, 14),
			new RectangleF(44, 12, 8, 31),
			new RectangleF(44, 7, 8, 5),
			new RectangleF(56, 36, 12, 7),
			new RectangleF(56, 26, 12, 10)
		};
		static readonly RectangleF[] sideBySideFullStackedRects = new RectangleF[] {
			new RectangleF(12, 28, 12, 15),
			new RectangleF(12, 7, 12, 21),
			new RectangleF(24, 22, 12, 21),
			new Rectangle(24, 7, 12, 15),
			new RectangleF(44, 12, 12, 31),
			new RectangleF(44, 7, 12, 5),
			new RectangleF(56, 32, 12, 11),
			new RectangleF(56, 7, 12, 25)
		};
		static readonly RectangleF[] stackedRects = new RectangleF[] {
			new RectangleF(17, 26, 14, 17),
			new RectangleF(17, 13, 14, 13),
			new RectangleF(17, 9, 14, 4),
			new RectangleF(49, 31, 14, 12),
			new RectangleF(49, 14, 14, 17),
			new RectangleF(49, 7, 14, 7)
		};
		static readonly RectangleF[] fullStackedRects = new RectangleF[] {
			new RectangleF(17, 27, 14, 16),
			new RectangleF(17, 15, 14, 12),
			new RectangleF(17, 7, 14, 8),
			new RectangleF(49, 31, 14, 12),
			new RectangleF(49, 14, 14, 17),
			new RectangleF(49, 7, 14, 7)
		};
		static readonly RectangleF[] sideBySideRangeRects = new RectangleF[] {
			new RectangleF(12, 18, 8, 25),
			new RectangleF(20, 7, 8, 28),
			new RectangleF(28, 13, 8, 26),
			new RectangleF(44, 7, 8, 29),
			new RectangleF(52, 15, 8, 25),
			new RectangleF(60, 17, 8, 26)
		};
		static readonly RectangleF[] stackedRangeRects = new RectangleF[] {
			new RectangleF(17, 28, 14, 15),
			new RectangleF(17, 17, 14, 11),
			new RectangleF(17, 10, 14, 7),
			new RectangleF(49, 30, 14, 10),
			new RectangleF(49, 12, 14, 18),
			new RectangleF(49, 7, 14, 5)
		};
		static readonly RectangleF[] sideBySideGanttRects = new RectangleF[] {
			new RectangleF(7, 39, 46, 4),
			new RectangleF(10, 33, 46, 4),
			new RectangleF(14, 27, 44, 4),
			new RectangleF(26, 19, 43, 4),
			new RectangleF(22, 13, 48, 4),
			new RectangleF(25, 7, 40, 4)
		};
		static readonly RectangleF[] ganttRects = new RectangleF[] {
			new RectangleF(22, 29, 10, 12),
			new RectangleF(32, 29, 32, 12),
			new RectangleF(64, 29, 6, 12),
			new RectangleF(7, 10, 20, 12),
			new RectangleF(27, 10, 17, 12),
			new RectangleF(44, 10, 11, 12)
		};
		static readonly Point[] points = new Point[] { 
			new Point(16, 25), new Point(32, 17), new Point(60, 14), new Point(18, 39), new Point(41, 34), new Point(57, 37)
		};
		static readonly Point[] stackedPoints = new Point[] { 
			new Point(16, 25), new Point(32, 17), new Point(60, 14), new Point(16, 39), new Point(32, 34), new Point(60, 37)
		};
		static readonly Point[] fullStackedPoints = new Point[] { 
			new Point(16, 14), new Point(32, 14), new Point(60, 14), new Point(16, 39), new Point(32, 34), new Point(60, 37)
		};
		static readonly Size[] bubbleSizes = new Size[] { 
			new Size(5, 5), new Size(6, 6), new Size(3, 3), new Size(4, 4), new Size(3, 3), new Size(5, 5)
		};
		static readonly Point[] swiftPlotPoints1 = new Point[] { new Point(16, 25), new Point(32, 17), new Point(60, 14) };
		static readonly Point[] swiftPlotPoints2 = new Point[] { new Point(17, 32), new Point(35, 25), new Point(59, 27) };
		static readonly Point[] swiftPlotPoints3 = new Point[] { new Point(18, 39), new Point(41, 34), new Point(57, 37) };
		static readonly Point[] radarPoints = new Point[] { 
			new Point(40, 20), new Point(23, 25), new Point(50, 25), new Point(40, 30), new Point(50, 35), new Point(30, 35)
		};
		static readonly Point[] radarLinePoints = new Point[] { new Point(40, 15), new Point(35, 30), new Point(55, 30), new Point(40, 15) };
		static readonly GRealPoint2D[] areaPolygon1 = new GRealPoint2D[] { 
			new GRealPoint2D(16, 43),  new GRealPoint2D(16, 25), new GRealPoint2D(32, 34), new GRealPoint2D(60, 14), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] areaPolygon2 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 39), new GRealPoint2D(32, 17), new GRealPoint2D(60, 37), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] stackedAreaPolygon1 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 25), new GRealPoint2D(32, 17), new GRealPoint2D(60, 14), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] stackedAreaPolygon2 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 39), new GRealPoint2D(32, 34), new GRealPoint2D(60, 37), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] fullStackedAreaPolygon1 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 7), new GRealPoint2D(32, 7), new GRealPoint2D(60, 7), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] fullStackedAreaPolygon2 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 25), new GRealPoint2D(32, 34), new GRealPoint2D(60, 14), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] stepAreaPolygon1 = new GRealPoint2D[] { 
			new GRealPoint2D(16, 43),  new GRealPoint2D(16, 25), new GRealPoint2D(27, 25), new GRealPoint2D(27, 17), new GRealPoint2D(60, 17), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] stepAreaPolygon2 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43),  new GRealPoint2D(16, 35), new GRealPoint2D(35, 35), new GRealPoint2D(35, 30), new GRealPoint2D(60, 30), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] rangeAreaPolygon1 = new GRealPoint2D[] { 
			new GRealPoint2D(16, 29),  new GRealPoint2D(16, 20), new GRealPoint2D(32, 8), new GRealPoint2D(60, 22), new GRealPoint2D(60, 43), new GRealPoint2D(32, 33)
		};
		static readonly GRealPoint2D[] rangeAreaPolygon2 = new GRealPoint2D[] {
			new GRealPoint2D(16, 39), new GRealPoint2D(16, 35), new GRealPoint2D(32, 23), new GRealPoint2D(60, 28), new GRealPoint2D(60, 37), new GRealPoint2D(32, 33)
		};
		static readonly GRealPoint2D[] radarAreaPolygon = new GRealPoint2D[] { 
			new GRealPoint2D(40, 15), new GRealPoint2D(35, 30), new GRealPoint2D(55, 30), new GRealPoint2D(40, 15)
		};
		static readonly GRealPoint2D[] splinePoints1 = new GRealPoint2D[] { 
			new GRealPoint2D(16, 25), new GRealPoint2D(32, 34), new GRealPoint2D(60, 14)
		};
		static readonly GRealPoint2D[] splinePoints2 = new GRealPoint2D[] {
			new GRealPoint2D(16, 39), new GRealPoint2D(32, 17), new GRealPoint2D(60, 37) 
		};
		static readonly GRealPoint2D[] splineZeroPoints1 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(32, 43), new GRealPoint2D(60, 43) 
		};
		static readonly GRealPoint2D[] splineZeroPoints2 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(32, 43), new GRealPoint2D(60, 43)  
		};
		static readonly GRealPoint2D[] splineStackedAreaPoints1 = new GRealPoint2D[] {
			new GRealPoint2D(16, 25), new GRealPoint2D(32, 17), new GRealPoint2D(60, 14)
		};
		static readonly GRealPoint2D[] splineStackedAreaPoints2 = new GRealPoint2D[] {
			new GRealPoint2D(16, 39), new GRealPoint2D(32, 34), new GRealPoint2D(60, 37)
		};
		static readonly GRealPoint2D[] splineStackedAreaZeroPoints1 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(32, 43), new GRealPoint2D(60, 43),
		};
		static readonly GRealPoint2D[] splineStackedAreaZeroPoints2 = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(32, 43), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] splineFullStackedAreaPoints1 = new GRealPoint2D[] { 
			new GRealPoint2D(16, 7), new GRealPoint2D(32, 7), new GRealPoint2D(60, 7)
		};
		static readonly GRealPoint2D[] splineFullStackedAreaPoints2 = new GRealPoint2D[] { 
			new GRealPoint2D(16, 25), new GRealPoint2D(32, 34), new GRealPoint2D(60, 14)
		};
		static readonly GRealPoint2D[] splineFullStackedAreaZeroPoints1 = new GRealPoint2D[] { 
			new GRealPoint2D(16, 43), new GRealPoint2D(32, 43), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] splineFullStackedAreaZeroPoints2 = new GRealPoint2D[] { 
			new GRealPoint2D(16, 43), new GRealPoint2D(32, 43), new GRealPoint2D(60, 43)
		};
		static readonly Line[] stockRedLines = { 
			new Line(11, 8, 11, 26), new Line(12, 8, 12, 26), new Line(9, 9, 12, 9), new Line(9, 10, 12, 10), 
			new Line(11, 20, 14, 20), new Line(11, 21, 14, 21), new Line(34, 15, 34, 26), new Line(35, 15, 35, 26), 
			new Line(32, 15, 35, 15), new Line(32, 16, 35, 16), new Line(34, 23, 37, 23), new Line(34, 24, 37, 24), 
			new Line(45, 20, 45, 40), new Line(46, 20, 46, 40), new Line(43, 21, 46, 21), new Line(43, 22, 46, 22), 
			new Line(45, 31, 48, 31), new Line(45, 32, 48, 32)
		};
		static readonly Line[] stockBlackLines = { 
			new Line(22, 10, 22, 26), new Line(23, 10, 23, 26), new Line(20, 19, 23, 19), new Line(20, 20, 23, 20), 
			new Line(22, 17, 25, 17), new Line(22, 18, 25, 18), new Line(56, 22, 56, 36), new Line(57, 22, 57, 36), 
			new Line(54, 27, 57, 27), new Line(54, 28, 57, 28), new Line(56, 25, 59, 25), new Line(56, 26, 59, 26), 
			new Line(67, 12, 67, 24), new Line(68, 12, 68, 24), new Line(65, 19, 68, 19), new Line(65, 20, 68, 20), 
			new Line(67, 15, 70, 15), new Line(67, 16, 70, 16)
		};
		static readonly Line[] candleStickRedLines = { 
			new Line(9, 11, 9, 23), new Line(10, 11, 10, 23), new Line(13, 11, 13, 23), new Line(14, 11, 14, 23), 
			new Line(11, 9, 11, 12), new Line(12, 9, 12, 12), new Line(11, 22, 11, 27), new Line(12, 22, 12, 27), 
			new Line(32, 17, 32, 25), new Line(33, 17, 33, 25), new Line(34, 17, 34, 27), new Line(35, 17, 35, 27), 
			new Line(36, 17, 36, 25), new Line(37, 17, 37, 25), new Line(43, 23, 43, 33), new Line(44, 23, 44, 33), 
			new Line(47, 23, 47, 33), new Line(48, 23, 48, 33), new Line(45, 21, 45, 24), new Line(46, 21, 46, 24), 
			new Line(45, 32, 45, 41), new Line(46, 32, 46, 41)
		};
		static readonly Line[] candleStickBlackLines = { 
			new Line(20, 15, 20, 21), new Line(21, 15, 21, 21), new Line(22, 11, 22, 27), new Line(23, 11, 23, 27), 
			new Line(24, 15, 24, 21), new Line(25, 15, 25, 21), new Line(54, 27, 54, 29), new Line(55, 27, 55, 29), 
			new Line(56, 23, 56, 37), new Line(57, 23, 57, 37), new Line(58, 27, 58, 29), new Line(59, 27, 59, 29), 
			new Line(65, 17, 65, 22), new Line(66, 17, 66, 22), new Line(69, 17, 69, 22), new Line(70, 17, 70, 22), 
			new Line(67, 13, 67, 18), new Line(68, 13, 68, 18), new Line(67, 21, 67, 25), new Line(68, 21, 68, 25) 
		};
		static readonly RectangleF[] sideBySideBar3DRects = new RectangleF[] {
			new RectangleF(17, 30, 12, 13),
			new RectangleF(30, 12, 12, 31),
			new RectangleF(43, 25, 12, 18)
		};
		static readonly RectangleF[] stackedBar3DRects = new RectangleF[] {
			new RectangleF(17, 26, 14, 17),
			new RectangleF(17, 21, 14, 5),
			new RectangleF(17, 12, 14, 9),
			new RectangleF(46, 33, 14, 10),
			new RectangleF(46, 21, 14, 12),
			new RectangleF(46, 16, 14, 5)
		};
		static readonly RectangleF[] fullStackedBar3DRects = new RectangleF[] {
			new RectangleF(17, 26, 14, 17),
			new RectangleF(17, 21, 14, 5),
			new RectangleF(17, 12, 14, 9),
			new RectangleF(46, 32, 14, 11),
			new RectangleF(46, 24, 14, 8),
			new RectangleF(46, 12, 14, 12)
		};
		static readonly RectangleF[] sideBySideStackedBar3DRects = new RectangleF[] {
			new RectangleF(12, 34, 10, 9),
			new RectangleF(12, 20, 10, 14),
			new RectangleF(23, 30, 10, 13),
			new RectangleF(23, 16, 10, 14),
			new RectangleF(42, 28, 10, 15),
			new RectangleF(42, 14, 10, 14),
			new RectangleF(53, 34, 10, 9),
			new RectangleF(53, 24, 10, 10)
		};
		static readonly RectangleF[] sideBySideFullStackedBar3DRects = new RectangleF[] {
			new RectangleF(12, 30, 10, 13),
			new RectangleF(12, 12, 10, 18),
			new RectangleF(23, 22, 10, 21),
			new RectangleF(23, 12, 10, 10),
			new RectangleF(42, 20, 10, 23),
			new RectangleF(42, 12, 10, 8),
			new RectangleF(53, 32, 10, 11),
			new RectangleF(53, 12, 10, 20)
		};
		static readonly RectangleF[] manhattanBarRects = new RectangleF[] {
			new RectangleF(12, 30, 12, 13),
			new RectangleF(30, 12, 12, 31),
			new RectangleF(48, 25, 12, 18)
		};
		static readonly GRealPoint2D[] firstLine3D = new GRealPoint2D[] { 
			new GRealPoint2D(20, 15), new GRealPoint2D(36, 24), new GRealPoint2D(64, 21) 
		};
		static readonly GRealPoint2D[] secondLine3D = new GRealPoint2D[] { 
			new GRealPoint2D(10, 35), new GRealPoint2D(26, 33), new GRealPoint2D(54, 37) 
		};
		static readonly GRealPoint2D[] firstStackedLine3D = new GRealPoint2D[] { 
			new GRealPoint2D(15, 15), new GRealPoint2D(31, 24), new GRealPoint2D(59, 21) 
		};
		static readonly GRealPoint2D[] secondStackedLine3D = new GRealPoint2D[] { 
			new GRealPoint2D(15, 35), new GRealPoint2D(31, 33), new GRealPoint2D(59, 37) 
		};
		static readonly GRealPoint2D[] firstFullStackedLine3D = new GRealPoint2D[] { 
			new GRealPoint2D(15, 15), new GRealPoint2D(31, 15), new GRealPoint2D(59, 15) 
		};
		static readonly GRealPoint2D[] secondFullStackedLine3D = new GRealPoint2D[] { 
			new GRealPoint2D(15, 35), new GRealPoint2D(31, 24), new GRealPoint2D(59, 37) 
		};
		static readonly GRealPoint2D[] firstStepLine3D = new GRealPoint2D[] {
			new GRealPoint2D(20, 15), new GRealPoint2D(36, 15), new GRealPoint2D(36, 24), new GRealPoint2D(64, 24), new GRealPoint2D(64, 21)
		};
		static readonly GRealPoint2D[] secondStepLine3D = new GRealPoint2D[] {
			new GRealPoint2D(10, 35), new GRealPoint2D(26, 35), new GRealPoint2D(26, 33), new GRealPoint2D(54, 33), new GRealPoint2D(54, 37)
		};
		static readonly GRealPoint2D[] firstArea3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 39), new GRealPoint2D(40, 17), new GRealPoint2D(60, 37), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] secondArea3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 25), new GRealPoint2D(40, 34), new GRealPoint2D(60, 14), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] firstStackedArea3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 25), new GRealPoint2D(40, 34), new GRealPoint2D(60, 30), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] secondStackedArea3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 25), new GRealPoint2D(16, 15), new GRealPoint2D(40, 29), 
			new GRealPoint2D(60, 10), new GRealPoint2D(60, 30), new GRealPoint2D(40, 34)
		};
		static readonly GRealPoint2D[] firstFullStackedArea3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 25), new GRealPoint2D(40, 34), new GRealPoint2D(60, 14), new GRealPoint2D(60, 43)
		};
		static readonly GRealPoint2D[] secondFullStackedArea3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 25), new GRealPoint2D(16, 12), new GRealPoint2D(60, 12), new GRealPoint2D(60, 14), new GRealPoint2D(40, 34)
		};
		static readonly GRealPoint2D[] firstAreaSpline3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 39), new GRealPoint2D(40, 17), 
			new GRealPoint2D(60, 37), new GRealPoint2D(60, 43), new GRealPoint2D(40, 43)
		};
		static readonly GRealPoint2D[] secondAreaSpline3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 25), new GRealPoint2D(40, 34), 
			new GRealPoint2D(60, 14), new GRealPoint2D(60, 43), new GRealPoint2D(40, 43)
		};
		static readonly GRealPoint2D[] firstStackedSplineArea3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 25), new GRealPoint2D(40, 34), 
			new GRealPoint2D(60, 30), new GRealPoint2D(60, 43), new GRealPoint2D(40, 43)
		};
		static readonly GRealPoint2D[] firstSplineFullStackedArea3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 43), new GRealPoint2D(16, 25), new GRealPoint2D(40, 34), 
			new GRealPoint2D(60, 14), new GRealPoint2D(60, 43), new GRealPoint2D(40, 43)
		};
		static readonly GRealPoint2D[] secondSplineFullStackedArea3DPolygon = new GRealPoint2D[] {
			new GRealPoint2D(16, 25), new GRealPoint2D(16, 12), new GRealPoint2D(40, 12), 
			new GRealPoint2D(60, 12), new GRealPoint2D(60, 14), new GRealPoint2D(40, 34)
		};
		static readonly float[] pieAngles = new float[] { 216.0f, 72.0f, 36.0f, 36.0f };
		static readonly float[] outerDoughnutAngles = new float[] { 120.0f, 150.0f, 90.0f };
		static readonly float[] innerDoughnutAngles = new float[] { 90.0f, 150.0f, 120.0f };
		static readonly Rectangle smallFunnel3DEllipseRect = new Rectangle(30, 36, 21, 8);
		static readonly Rectangle smallFunnel3DRect = new Rectangle(30, 30, 21, 10);
		static readonly Rectangle middleFunnel3DEllipseRect = new Rectangle(30, 26, 21, 8);
		static readonly Rectangle bigFunnel3DEllipseRect = new Rectangle(20, 15, 40, 10);
		static readonly Rectangle topFunnel3DEllipseRect = new Rectangle(10, 5, 60, 10);
		static readonly GRealPoint2D[] middleFunnel3DPolygonPoints = new GRealPoint2D[] { new GRealPoint2D(20, 20), new GRealPoint2D(60, 20), new GRealPoint2D(51, 30), new GRealPoint2D(30, 30) };
		static readonly GRealPoint2D[] bigFunnel3DPolygonPoints = new GRealPoint2D[] { new GRealPoint2D(10, 10), new GRealPoint2D(70, 10), new GRealPoint2D(60, 20), new GRealPoint2D(20, 20) };
		static readonly Rectangle[] sideBySideCylinderEllipseRects = new Rectangle[] { new Rectangle(17, 37, 14, 6), new Rectangle(32, 37, 14, 6),
			new Rectangle(47, 37, 14, 6) };
		static readonly Rectangle[] sideBySideCylinderRects = new Rectangle[] { new Rectangle(17, 28, 14, 12), new Rectangle(32, 10, 14, 30),
			new Rectangle(47, 23, 14, 17) };
		static readonly Rectangle[] sideBySideCylinderTopEllipseRects = new Rectangle[] { new Rectangle(17, 25, 14, 6), new Rectangle(32, 7, 14, 6),
			new Rectangle(47, 20, 14, 6) };
		static readonly Rectangle[] stackedCylinderEllipseRects = new Rectangle[] { new Rectangle(17, 37, 14, 6), new Rectangle(17, 23, 14, 6),  
			new Rectangle(17, 16, 14, 6), new Rectangle(46, 37, 14, 6), new Rectangle(46, 28, 14, 6),  new Rectangle(46, 17, 14, 6) };
		static readonly Rectangle[] stackedCylinderRects = new Rectangle[] { new Rectangle(17, 26, 14, 14), new Rectangle(17, 19, 14, 7), 
			new Rectangle(17, 10, 14, 9), new Rectangle(46, 31, 14, 9), new Rectangle(46, 20, 14, 11), new Rectangle(46, 14, 14, 6) };
		static readonly Rectangle[] stackedCylinderTopEllipseRects = new Rectangle[] { new Rectangle(17, 7, 14, 6), new Rectangle(46, 11, 14, 6) };
		static readonly Rectangle[] fullStackedCylinderEllipseRects = new Rectangle[] { new Rectangle(17, 37, 14, 6), new Rectangle(17, 23, 14, 6),  
			new Rectangle(17, 16, 14, 6), new Rectangle(46, 37, 14, 6), new Rectangle(46, 27, 14, 6),  new Rectangle(46, 18, 14, 6) };
		static readonly Rectangle[] fullStackedCylinderRects = new Rectangle[] { new Rectangle(17, 26, 14, 14), new Rectangle(17, 19, 14, 7), 
			new Rectangle(17, 10, 14, 9), new Rectangle(46, 30, 14, 10), new Rectangle(46, 21, 14, 9), new Rectangle(46, 10, 14, 11) };
		static readonly Rectangle[] fullStackedCylinderTopEllipseRects = new Rectangle[] { new Rectangle(17, 7, 14, 6), new Rectangle(46, 7, 14, 6) };
		static readonly Rectangle[] sideBySideStackedCylinderEllipseRects = new Rectangle[] { new Rectangle(12, 37, 10, 6), new Rectangle(12, 28, 10, 6),
			new Rectangle(23, 37, 10, 6), new Rectangle(23, 24, 10, 6), new Rectangle(42, 37, 10, 6), new Rectangle(42, 23, 10, 6),
			new Rectangle(53, 37, 10, 6), new Rectangle(53, 29, 10, 6) };
		static readonly Rectangle[] sideBySideStackedCylinderRects = new Rectangle[] { new Rectangle(12, 31, 10, 9), new Rectangle(12, 18, 10, 13),
			new Rectangle(23, 27, 10, 13), new Rectangle(23, 14, 10, 13), new Rectangle(42, 26, 10, 14), new Rectangle(42, 12, 10, 14),
			new Rectangle(53, 32, 10, 8), new Rectangle(53, 22, 10, 10) };
		static readonly Rectangle[] sideBySideStackedCylinderTopEllipseRects = new Rectangle[] { new Rectangle(12, 15, 10, 6), new Rectangle(23, 11, 10, 6),
			new Rectangle(42, 9, 10, 6), new Rectangle(53, 19, 10, 6) };
		static readonly Rectangle[] sideBySideFullStackedCylinderEllipseRects = new Rectangle[] { new Rectangle(12, 37, 10, 6), new Rectangle(12, 24, 10, 6),
			new Rectangle(23, 37, 10, 6), new Rectangle(23, 17, 10, 6), new Rectangle(42, 37, 10, 6), new Rectangle(42, 15, 10, 6),
			new Rectangle(53, 37, 10, 6), new Rectangle(53, 26, 10, 6) };
		static readonly Rectangle[] sideBySideFullStackedCylinderRects = new Rectangle[] { new Rectangle(12, 27, 10, 13), new Rectangle(12, 10, 10, 17),
			new Rectangle(23, 20, 10, 20), new Rectangle(23, 10, 10, 10), new Rectangle(42, 18, 10, 22), new Rectangle(42, 10, 10, 8),
			new Rectangle(53, 29, 10, 11), new Rectangle(53, 10, 10, 19) };
		static readonly Rectangle[] sideBySideFullStackedCylinderTopEllipseRects = new Rectangle[] { new Rectangle(12, 7, 10, 6), new Rectangle(23, 7, 10, 6),
			new Rectangle(42, 7, 10, 6), new Rectangle(53, 7, 10, 6) };
		static readonly Rectangle[] manhattanCylinderEllipseRects = new Rectangle[] { new Rectangle(12, 37, 14, 6), new Rectangle(30, 37, 14, 6),
			new Rectangle(48, 37, 14, 6) };
		static readonly Rectangle[] manhattanCylinderRects = new Rectangle[] { new Rectangle(12, 28, 14, 12), new Rectangle(30, 10, 14, 30),
			new Rectangle(48, 23, 14, 17) };
		static readonly Rectangle[] manhattanCylinderTopEllipseRects = new Rectangle[] { new Rectangle(12, 25, 14, 6), new Rectangle(30, 7, 14, 6),
			new Rectangle(48, 20, 14, 6) };
		static readonly Rectangle[] sideBySideConeEllipseRects = new Rectangle[] { new Rectangle(17, 37, 14, 6), new Rectangle(33, 37, 14, 6),
			new Rectangle(49, 37, 14, 6) };
		static readonly Rectangle[] sideBySideConeRects = new Rectangle[] { new Rectangle(16, 25, 15, 15), new Rectangle(32, 7, 15, 33), new Rectangle(48, 20, 15, 20) };
		static readonly Rectangle[] stackedConeEllipseRects = new Rectangle[] { new Rectangle(17, 37, 14, 6), new Rectangle(20, 24, 8, 4),
			new Rectangle(21, 17, 6, 4), new Rectangle(47, 37, 14, 6), new Rectangle(49, 28, 10, 4),  new Rectangle(52, 19, 4, 2) };
		static readonly Rectangle[] stackedConeRects = new Rectangle[] { new Rectangle(16, 7, 15, 33), new Rectangle(19, 7, 9, 19), 
			new Rectangle(20, 7, 7, 12), new Rectangle(46, 11, 15, 29), new Rectangle(48, 11, 11, 19), new Rectangle(51, 11, 5, 9) };
		static readonly Rectangle[] fullStackedConeEllipseRects = new Rectangle[] { new Rectangle(17, 37, 14, 6), new Rectangle(20, 24, 8, 4),  
			new Rectangle(21, 17, 6, 4), new Rectangle(46, 37, 14, 6), new Rectangle(48, 28, 10, 4),  new Rectangle(49, 22, 8, 4) };
		static readonly Rectangle[] fullStackedConeRects = new Rectangle[] { new Rectangle(16, 7, 15, 33), new Rectangle(19, 7, 9, 19), 
			new Rectangle(20, 7, 7, 12), new Rectangle(45, 7, 15, 33), new Rectangle(47, 7, 11, 23), new Rectangle(48, 7, 9, 17) };
		static readonly Rectangle[] sideBySideStackedConeEllipseRects = new Rectangle[] { new Rectangle(12, 37, 10, 6), new Rectangle(14, 27, 6, 4),
			new Rectangle(24, 37, 10, 6), new Rectangle(26, 24, 6, 4), new Rectangle(44, 37, 10, 6), new Rectangle(46, 23, 5, 4),
			new Rectangle(56, 37, 10, 6), new Rectangle(58, 29, 6, 4) };
		static readonly Rectangle[] sideBySideStackedConeRects = new Rectangle[] { new Rectangle(11, 15, 11, 25), new Rectangle(13, 15, 7, 14),
			new Rectangle(23, 11, 11, 29), new Rectangle(25, 11, 7, 15), new Rectangle(43, 9, 11, 31), new Rectangle(45, 9, 6, 16),
			new Rectangle(55, 19, 11, 21), new Rectangle(57, 19, 7, 12) };
		static readonly Rectangle[] sideBySideFullStackedConeEllipseRects = new Rectangle[] { new Rectangle(12, 37, 10, 6), new Rectangle(14, 24, 6, 4),
			new Rectangle(24, 37, 10, 6), new Rectangle(27, 20, 4, 2), new Rectangle(44, 37, 10, 6), new Rectangle(45, 31, 8, 4),
			new Rectangle(56, 37, 10, 6), new Rectangle(58, 26, 6, 4) };
		static readonly Rectangle[] sideBySideFullStackedConeRects = new Rectangle[] { new Rectangle(11, 7, 11, 33), new Rectangle(13, 7, 7, 19),
			new Rectangle(23, 7, 11, 33), new Rectangle(26, 7, 5, 14), new Rectangle(43, 7, 11, 33), new Rectangle(44, 7, 9, 26),
			new Rectangle(55, 7, 11, 33), new Rectangle(57, 7, 7, 21) };
		static readonly Rectangle[] manhattanConeEllipseRects = new Rectangle[] { new Rectangle(12, 37, 14, 6), new Rectangle(31, 37, 14, 6),
			new Rectangle(50, 37, 14, 6) };
		static readonly Rectangle[] manhattanConeRects = new Rectangle[] { new Rectangle(11, 25, 15, 15), new Rectangle(30, 7, 15, 33), new Rectangle(49, 20, 15, 20) };
		static readonly Rectangle[] sideBySidePyramidRects = new Rectangle[] { new Rectangle(17, 25, 14, 18), new Rectangle(32, 7, 14, 36), new Rectangle(47, 20, 14, 23) };
		static readonly PlaneTriangle[] sideBySidePyramidRightTriangles = new PlaneTriangle[] { new PlaneTriangle(new DiagramPoint(24, 25), new DiagramPoint(31, 43), new DiagramPoint(34, 40)),
			new PlaneTriangle(new DiagramPoint(39, 7), new DiagramPoint(46, 43), new DiagramPoint(49, 40)), new PlaneTriangle(new DiagramPoint(54, 20), new DiagramPoint(61, 43), new DiagramPoint(64, 40))};
		static readonly Rectangle[] stackedPyramidRects = new Rectangle[] { new Rectangle(17, 7, 14, 36), new Rectangle(21, 7, 6, 18),
			new Rectangle(22, 7, 4, 10), new Rectangle(46, 11, 14, 32), new Rectangle(49, 11, 8, 21), new Rectangle(51, 11, 4, 9) };
		static readonly PlaneTriangle[] stackedPyramidRightTriangles = new PlaneTriangle[] { new PlaneTriangle(new DiagramPoint(24, 7), new DiagramPoint(31, 43), new DiagramPoint(34, 40)),
			new PlaneTriangle(new DiagramPoint(24, 7), new DiagramPoint(27, 25), new DiagramPoint(29, 23)), new PlaneTriangle(new DiagramPoint(24, 7), new DiagramPoint(26, 17), new DiagramPoint(27, 15)),
			new PlaneTriangle(new DiagramPoint(53, 11), new DiagramPoint(60, 43), new DiagramPoint(63, 40)), new PlaneTriangle(new DiagramPoint(53, 11), new DiagramPoint(57, 32), new DiagramPoint(60, 30)),
			new PlaneTriangle(new DiagramPoint(53, 11), new DiagramPoint(55, 20), new DiagramPoint(56, 19))};
		static readonly Rectangle[] fullStackedPyramidRects = new Rectangle[] { new Rectangle(17, 7, 14, 36), new Rectangle(21, 7, 6, 18),
			new Rectangle(22, 7, 4, 10), new Rectangle(46, 7, 14, 36), new Rectangle(48, 7, 10, 27), new Rectangle(50, 7, 6, 14) };
		static readonly PlaneTriangle[] fullStackedPyramidRightTriangles = new PlaneTriangle[] { new PlaneTriangle(new DiagramPoint(24, 7), new DiagramPoint(31, 43), new DiagramPoint(34, 40)),
			new PlaneTriangle(new DiagramPoint(24, 7), new DiagramPoint(27, 25), new DiagramPoint(29, 23)), new PlaneTriangle(new DiagramPoint(24, 7), new DiagramPoint(26, 17), new DiagramPoint(27, 15)),
			new PlaneTriangle(new DiagramPoint(53, 7), new DiagramPoint(60, 43), new DiagramPoint(63, 40)), new PlaneTriangle(new DiagramPoint(53, 7), new DiagramPoint(58, 34), new DiagramPoint(61, 32)),
			new PlaneTriangle(new DiagramPoint(53, 7), new DiagramPoint(56, 21), new DiagramPoint(57, 20))};
		static readonly Rectangle[] sideBySideStackedPyramidRects = new Rectangle[] { new Rectangle(12, 15, 10, 28), new Rectangle(14, 15, 6, 16),
			new Rectangle(23, 11, 10, 32), new Rectangle(25, 11, 6, 16), new Rectangle(42, 9, 10, 34), new Rectangle(44, 9, 6, 20),
			new Rectangle(53, 19, 10, 24), new Rectangle(55, 19, 6, 17) };
		static readonly PlaneTriangle[] sideBySideStackedPyramidRightTriangles = new PlaneTriangle[] { new PlaneTriangle(new DiagramPoint(17, 15), new DiagramPoint(22, 43), new DiagramPoint(25, 40)),
			new PlaneTriangle(new DiagramPoint(17, 15), new DiagramPoint(20, 31), new DiagramPoint(22, 29)), new PlaneTriangle(new DiagramPoint(28, 11), new DiagramPoint(33, 43), new DiagramPoint(36, 40)),
			new PlaneTriangle(new DiagramPoint(28, 11), new DiagramPoint(31, 27), new DiagramPoint(32, 26)), new PlaneTriangle(new DiagramPoint(47, 9), new DiagramPoint(52, 43), new DiagramPoint(55, 40)),
			new PlaneTriangle(new DiagramPoint(47, 9), new DiagramPoint(50, 29), new DiagramPoint(52, 27)), new PlaneTriangle(new DiagramPoint(58, 19), new DiagramPoint(63, 43), new DiagramPoint(66, 40)),
			new PlaneTriangle(new DiagramPoint(58, 19), new DiagramPoint(61, 36), new DiagramPoint(63, 34))};
		static readonly Rectangle[] sideBySideFullStackedPyramidRects = new Rectangle[] { new Rectangle(12, 7, 10, 36), new Rectangle(14, 7, 6, 20),
			new Rectangle(23, 7, 10, 36), new Rectangle(26, 7, 4, 13), new Rectangle(42, 7, 10, 36), new Rectangle(45, 7, 4, 15),
			new Rectangle(53, 7, 10, 36), new Rectangle(55, 7, 6, 24) };
		static readonly PlaneTriangle[] sideBySideFullStackedPyramidRightTriangles = new PlaneTriangle[] { new PlaneTriangle(new DiagramPoint(17, 7), new DiagramPoint(22, 43), new DiagramPoint(25, 40)),
			new PlaneTriangle(new DiagramPoint(17, 7), new DiagramPoint(20, 27), new DiagramPoint(22, 26)), new PlaneTriangle(new DiagramPoint(28, 7), new DiagramPoint(33, 43), new DiagramPoint(36, 40)),
			new PlaneTriangle(new DiagramPoint(28, 7), new DiagramPoint(30, 20), new DiagramPoint(31, 19)), new PlaneTriangle(new DiagramPoint(47, 7), new DiagramPoint(52, 43), new DiagramPoint(55, 40)),
			new PlaneTriangle(new DiagramPoint(47, 7), new DiagramPoint(49, 22), new DiagramPoint(50, 21)), new PlaneTriangle(new DiagramPoint(58, 7), new DiagramPoint(63, 43), new DiagramPoint(66, 40)),
			new PlaneTriangle(new DiagramPoint(58, 7), new DiagramPoint(61, 31), new DiagramPoint(63, 29))};
		static readonly Rectangle[] manhattanPyramidRects = new Rectangle[] { new Rectangle(12, 25, 14, 18), new Rectangle(30, 7, 14, 36), new Rectangle(48, 20, 14, 23) };
		static readonly PlaneTriangle[] manhattanPyramidRightTriangles = new PlaneTriangle[] { new PlaneTriangle(new DiagramPoint(19, 25), new DiagramPoint(26, 43), new DiagramPoint(29, 40)),
			new PlaneTriangle(new DiagramPoint(37, 7), new DiagramPoint(44, 43), new DiagramPoint(47, 40)), new PlaneTriangle(new DiagramPoint(55, 20), new DiagramPoint(62, 43), new DiagramPoint(65, 40))};
		static void RenderChartAppearance(GdiPlusRenderer renderer, WholeChartAppearance chartAppearance) {
			renderer.FillRectangle(chartRect, chartAppearance.BackColor);
		}
		static void RenderBorder(GdiPlusRenderer renderer, WholeChartAppearance chartAppearance) {
			renderer.DrawRectangle(chartRect, chartAppearance.BorderColor, 1);
		}
		static void RenderXYDiagramAppearance(GdiPlusRenderer renderer, XYDiagramAppearance diagramAppearance) {
			RectangleFillStyle fillStyle = (RectangleFillStyle)diagramAppearance.FillStyle;
			fillStyle.Render(renderer, diagramRect, diagramAppearance.BackColor, Color.Empty);
		}
		static void RenderRadarDiagramAppearance(GdiPlusRenderer renderer, RadarDiagramAppearance appearance) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			float radius = (float)(radarDiagramCenter.Y - 7);
			renderer.FillCircle(radarDiagramCenter, radius, appearance.BackColor);
			Color axisColor = appearance.AxisColor;
			float radiusFraction = 0.7f * radius;
			float left = radarDiagramCenter.X - radiusFraction;
			float right = radarDiagramCenter.X + radiusFraction;
			float top = radarDiagramCenter.Y - radiusFraction;
			float bottom = radarDiagramCenter.Y + radiusFraction;
			PointF topPoint = new PointF(radarDiagramCenter.X, radarDiagramCenter.Y - radius);
			PointF bottomPoint = new PointF(radarDiagramCenter.X, radarDiagramCenter.Y + radius);
			PointF leftPoint = new PointF(radarDiagramCenter.X - radius, radarDiagramCenter.Y);
			PointF rightPoint = new PointF(radarDiagramCenter.X + radius, radarDiagramCenter.Y);
			renderer.DrawLine(new PointF((float)left, (float)top), new PointF((float)right, (float)bottom), axisColor, 1);
			renderer.DrawLine(new PointF(right, top), new PointF(left, bottom), axisColor, 1);
			renderer.DrawLine(topPoint, bottomPoint, axisColor, 1);
			renderer.DrawLine(leftPoint, rightPoint, axisColor, 1);
			renderer.DrawCircle(radarDiagramCenter, radius, axisColor, 1);
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderXYDiagram3DAppearance(GdiPlusRenderer renderer, XYDiagram3DAppearance diagramAppearance, StripAppearance stripAppearance) {
			RectangleF rectangle1 = new RectangleF(7, 7, 66, 9);
			RectangleF rectangle2 = new RectangleF(7, 25, 66, 9);
			Color stripColor = diagramAppearance.InterlacedColor;
			RectangleFillStyle stripFillStyle = stripAppearance.FillStyle;
			diagramAppearance.FillStyle.Options.RenderRectangle(renderer, diagram3DRect, diagram3DRect, diagramAppearance.BackColor, Color.Empty);
			renderer.FillRectangle(rectangle1, stripColor, stripFillStyle);
			renderer.FillRectangle(rectangle2, stripColor, stripFillStyle); 
		}
		static void RenderBarAppearance(GdiPlusRenderer renderer, BarSeriesViewAppearance barAppearance, ViewType viewType, PaletteEntry[] entries) {
			RectangleF[] rects;
			switch (viewType) {
				case ViewType.StackedBar:
					rects = stackedRects;
					break;
				case ViewType.FullStackedBar:
					rects = fullStackedRects;
					break;
				case ViewType.SideBySideStackedBar:
					rects = sideBySideStackedRects;
					break;
				case ViewType.SideBySideFullStackedBar:
					rects = sideBySideFullStackedRects;
					break;
				case ViewType.SideBySideRangeBar:
					rects = sideBySideRangeRects;
					break;
				case ViewType.RangeBar:
					rects = stackedRangeRects;
					break;
				case ViewType.SideBySideGantt:
					rects = sideBySideGanttRects;
					break;
				case ViewType.Gantt:
					rects = ganttRects;
					break;
				default:
					rects = sideBySideRects;
					break;
			}
			int seriesCount = rects.Length / 2;
			int entryIndex = 0;
			foreach (RectangleF rect in rects) {
				PaletteEntry entry = entries[entryIndex];
				barAppearance.FillStyle.Render(renderer, rect, entry.Color, entry.Color2);
				entryIndex++;
				if (entryIndex >= seriesCount || entryIndex >= entries.Length)
					entryIndex = 0;
			}
			Color borderColor = barAppearance.BorderColor;
			if (!borderColor.IsEmpty)
				foreach (RectangleF rect in rects)
					renderer.DrawRectangle(rect, borderColor, 1);
		}
		static void RenderPointAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries, Point[] points, Size[] sizes, MarkerAppearance appearance) {
			int entryIndex = 0;
			for (int i = 0; i < points.Length; i++) {
				Rectangle rect = new Rectangle(points[i], Size.Empty);
				rect.Inflate(sizes == null || sizes.Length < i + 1 ? new Size(5, 5) : sizes[i]);
				PaletteEntry entry = entries[entryIndex];
				SimpleMarker.Render(renderer, MarkerKind.Circle, 0, rect,
					appearance.FillStyle, entry.Color, entry.Color2, appearance.BorderColor, 1);
				if (++entryIndex >= 3)
					entryIndex = 0;
			}
		}
		static void RenderPointAppearance(GdiPlusRenderer renderer, IChartAppearance appearance, PaletteEntry[] entries, Point[] points, Size[] sizes) {
			RenderPointAppearance(renderer, entries, points, sizes, appearance.MarkerAppearance);
		}
		static void RenderLineAppearance(GdiPlusRenderer renderer, IChartAppearance appearance, Point[] points, bool isStepLine, PaletteEntry[] entries) {
			PointF[] firstSeries, secondSeries;
			if (isStepLine) {
				firstSeries = new PointF[] { points[0], new Point(points[1].X, points[0].Y), points[1], new Point(points[2].X, points[1].Y), points[2] };
				secondSeries = new PointF[] { points[3], new Point(points[4].X, points[3].Y), points[4], new Point(points[5].X, points[4].Y), points[5] };
			}
			else {
				firstSeries = new PointF[] { points[0], points[1], points[2] };
				secondSeries = new PointF[] { points[3], points[4], points[5] };
			}
			for (int i = 0; i < firstSeries.Length; i++) {
				firstSeries[i].X -= 0.5f;
				firstSeries[i].Y -= 0.5f;
			}
			for (int i = 0; i < secondSeries.Length; i++) {
				secondSeries[i].X -= 0.5f;
				secondSeries[i].Y -= 0.5f;
			}
			try {
				renderer.EnableAntialiasing(true);
				renderer.DrawLines(firstSeries, entries[0].Color, 2, null);
				renderer.DrawLines(secondSeries, entries[1].Color, 2, null);
			}
			finally {
				renderer.RestoreAntialiasing();
			}
			RenderPointAppearance(renderer, appearance, entries, points, new Size[0]);
		}
		static void RenderRadarLineAppearance(GdiPlusRenderer renderer, IChartAppearance appearance, PaletteEntry[] entries) {
			PointF[] series = new PointF[radarLinePoints.Length];
			for (int i = 0; i < radarLinePoints.Length; i++) {
				Point p = radarLinePoints[i];
				series[i] = new PointF(p.X - 0.5f, p.Y - 0.5f);
			}			
			try {
				renderer.EnableAntialiasing(true);
				using (Pen pen = new Pen(entries[0].Color, 2))
					renderer.DrawLines(series, entries[0].Color, 2, null);
			}
			finally {
				renderer.RestoreAntialiasing();
			}
			RenderPointAppearance(renderer, appearance, entries, radarLinePoints, new Size[0]);
		}
		static void RenderScatterLineAppearance(GdiPlusRenderer renderer, IChartAppearance appearance, PaletteEntry[] entries) {
			Point[] linePoints = new Point[] { points[3], points[2], points[0], points[5] };
			try {
				renderer.EnableAntialiasing(true);
				renderer.DrawLines(linePoints, entries[0].Color, 2, null);
			}
			finally {
				renderer.RestoreAntialiasing();
			}
			RenderPointAppearance(renderer, appearance, entries, linePoints, new Size[0]);
		}
		static void RenderSwiftPlotAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.DrawLines(swiftPlotPoints1, entries[0].Color, 1, null);
			renderer.DrawLines(swiftPlotPoints2, entries[1].Color, 1, null);
			renderer.DrawLines(swiftPlotPoints3, entries[2].Color, 1, null);
		}
		static void RenderSplineAppearance(GdiPlusRenderer renderer, IChartAppearance appearance, PaletteEntry[] entries) {
			BezierStrip firstSeries = new BezierStrip();
			firstSeries.Add(new GRealPoint2D(points[0].X, points[0].Y));
			firstSeries.Add(new GRealPoint2D(points[4].X, points[4].Y));
			firstSeries.Add(new GRealPoint2D(points[2].X, points[2].Y));
			BezierStrip secondSeries = new BezierStrip();
			secondSeries.Add(new GRealPoint2D(points[3].X, points[3].Y));
			secondSeries.Add(new GRealPoint2D(points[1].X, points[1].Y));
			secondSeries.Add(new GRealPoint2D(points[5].X, points[5].Y));
			try {
				renderer.EnableAntialiasing(true);
				renderer.DrawBezier(firstSeries, entries[0].Color, 2);
				renderer.DrawBezier(secondSeries, entries[1].Color, 2);
			}
			finally {
				renderer.RestoreAntialiasing();
			}
			RenderPointAppearance(renderer, appearance, entries, points, new Size[0]);
		}
		static RectangleF GetBounds(GRealPoint2D[] polygon) {
			using (GraphicsPath path = new GraphicsPath()) {
				path.AddPolygon(StripsUtils.Convert(polygon));
				RectangleF rect = path.GetBounds();
				return rect;
			}
		}
		static void RenderAreaAppearance(GdiPlusRenderer renderer, AreaSeriesViewAppearance areaAppearance, ViewType viewType, PaletteEntry[] entries) {
			GRealPoint2D[] polygon1, polygon2;
			switch (viewType) {
				case ViewType.StackedArea:
					polygon1 = stackedAreaPolygon1;
					polygon2 = stackedAreaPolygon2;
					break;
				case ViewType.FullStackedArea:
					polygon1 = fullStackedAreaPolygon1;
					polygon2 = fullStackedAreaPolygon2;
					break;
				case ViewType.StepArea:
					polygon1 = stepAreaPolygon1;
					polygon2 = stepAreaPolygon2;
					break;
				case ViewType.RangeArea:
					polygon1 = rangeAreaPolygon1;
					polygon2 = rangeAreaPolygon2;
					break;
				default:
					polygon1 = areaPolygon1;
					polygon2 = areaPolygon2;
					break;
			}
			LineStrip border1 = new LineStrip(polygon1.Length - 2);
			for (int i = 1; i < polygon1.Length - 1; i++)
				border1.Add(polygon1[i]);
			LineStrip border2 = new LineStrip(polygon2.Length - 2);
			for (int i = 1; i < polygon2.Length - 1; i++)
				border2.Add(polygon2[i]);
			FillOptionsBase fillOptions = areaAppearance.FillStyle.Options;
			Color borderColor = areaAppearance.BorderColor;
			fillOptions.Render(renderer, new LineStrip(polygon1), GetBounds(polygon1), entries[0].Color, entries[0].Color2);
			if (viewType != ViewType.FullStackedArea) 
				renderer.DrawLines(border1, borderColor, 1, null, LineCap.Flat);
			fillOptions.Render(renderer, new LineStrip(polygon2), GetBounds(polygon2), entries[1].Color, entries[1].Color2);
			if (viewType != ViewType.FullStackedArea)
				renderer.DrawLines(border2, borderColor, 1, null, LineCap.Flat);
		}
		static void RenderRadarAreaAppearance(GdiPlusRenderer renderer, IChartAppearance appearance, PaletteEntry[] entries) {
			PolygonFillStyle fillStyle = appearance.AreaSeriesViewAppearance.FillStyle;
			Color borderColor = appearance.AreaSeriesViewAppearance.BorderColor;
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			LineStrip polygon = new LineStrip(radarAreaPolygon);
			fillStyle.Options.Render(renderer, polygon, GetBounds(radarAreaPolygon),entries[0].Color, entries[0].Color2);
			renderer.DrawLines(polygon, borderColor, 1, null, LineCap.Flat);
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();			
			Point[] points = new Point[radarAreaPolygon.Length];
			for (int i = 0; i < points.Length; i++)
				points[i] = new Point((int)Math.Round(radarAreaPolygon[i].X), (int)Math.Round(radarAreaPolygon[i].Y));
			RenderPointAppearance(renderer, entries, points, new Size[0], appearance.MarkerAppearance);
		}
		static void RenderSplineAreaAppearance(GdiPlusRenderer renderer, AreaSeriesViewAppearance areaAppearance, ViewType viewType, PaletteEntry[] entries) {
			BezierRangeStrip strip1 = new BezierRangeStrip(0.8);
			BezierRangeStrip strip2 = new BezierRangeStrip(0.8);
			switch (viewType) {
				case ViewType.StackedSplineArea:
					strip1.TopStrip.AddRange(splineStackedAreaPoints1);
					strip1.BottomStrip.AddRange(splineStackedAreaZeroPoints1);
					strip2.TopStrip.AddRange(splineStackedAreaPoints2);
					strip2.BottomStrip.AddRange(splineStackedAreaZeroPoints2);
					break;
				case ViewType.FullStackedSplineArea:
					strip1.TopStrip.AddRange(splineFullStackedAreaPoints1);
					strip1.BottomStrip.AddRange(splineFullStackedAreaZeroPoints1);
					strip2.TopStrip.AddRange(splineFullStackedAreaPoints2);
					strip2.BottomStrip.AddRange(splineFullStackedAreaZeroPoints2);
					break;
				default:
					strip1.TopStrip.AddRange(splinePoints1);
					strip1.BottomStrip.AddRange(splineZeroPoints1);
					strip2.TopStrip.AddRange(splinePoints2);
					strip2.BottomStrip.AddRange(splineZeroPoints2);
					break;
			}
			FillOptionsBase fillOptions = areaAppearance.FillStyle.Options;
			renderer.EnablePolygonAntialiasing(true);
			fillOptions.Render(renderer, strip1, entries[0].Color, entries[0].Color2);
			fillOptions.Render(renderer, strip2, entries[1].Color, entries[1].Color2);
			renderer.RestorePolygonAntialiasing();
		}
		static void RenderFinancialAppearance(GdiPlusRenderer renderer, ViewType viewType) {
			Line[] redLines, blackLines;
			if (viewType == ViewType.Stock) {
				redLines = stockRedLines;
				blackLines = stockBlackLines;
			}
			else {
				redLines = candleStickRedLines;
				blackLines = candleStickBlackLines;
			}
			foreach (Line line in redLines)
				renderer.DrawLine(line.P1, line.P2, Color.Red, 1);
			foreach (Line line in blackLines)
				renderer.DrawLine(line.P1, line.P2, Color.Black, 1);
		}
		static Color GetHighlightColor(Color color, float luminance) {
			ColorHSL highlight = (ColorHSL)color;
			highlight.Luminance = Math.Min(highlight.Luminance * luminance, 1);
			return (Color)highlight;
		}
		static Brush GetGradientBrush(Rectangle imageRect, Color color, float luminance) {
			Rectangle gradientRect = new Rectangle(imageRect.X - 1, imageRect.Y, imageRect.Width + 2, imageRect.Height);
			LinearGradientBrush brush = new LinearGradientBrush(gradientRect, Color.White, Color.White, LinearGradientMode.Horizontal);
			ColorBlend blend = new ColorBlend();
			blend.Colors = new Color[] { GetHighlightColor(color, luminance), color, GetHighlightColor(color, luminance) };
			blend.Positions = new float[] { 0f, 0.5f, 1.0f };
			brush.InterpolationColors = blend;
			return brush;
		}
		static Brush GetConeGradientBrush(Rectangle imageRect, Color color) {
			return GetGradientBrush(imageRect, color, 1.3f);
		}
		static Brush GetCylinderGradientBrush(Rectangle imageRect, Color color) {
			return GetGradientBrush(imageRect, color, 1.2f);
		}
		static void RenderBar3DAppearance(GdiPlusRenderer renderer, Bar3DSeriesViewAppearance barAppearance, ViewType viewType, PaletteEntry[] entries) {
			RectangleF[] rects;
			int seriesCount = 3;
			switch (viewType) {
				case ViewType.Bar3D:
					rects = sideBySideBar3DRects;
					break;
				case ViewType.StackedBar3D:
					rects = stackedBar3DRects;
					break;
				case ViewType.FullStackedBar3D:
					rects = fullStackedBar3DRects;
					break;
				case ViewType.SideBySideStackedBar3D:
					rects = sideBySideStackedBar3DRects;
					seriesCount = 4;
					break;
				case ViewType.SideBySideFullStackedBar3D:
					rects = sideBySideFullStackedBar3DRects;
					seriesCount = 4;
					break;
				default:
					rects = manhattanBarRects;
					break;
			}
			for (int i = 0; i < rects.Length; i++) {
				PaletteEntry entry = entries[i % Math.Min(seriesCount, entries.Length)];
				RectangleF rect = rects[i];
				barAppearance.FillStyle.Options.RenderRectangle(renderer, rect, rect, entry.Color, entry.Color2);
				LineStrip top = new LineStrip(4);
				top.Add(new GRealPoint2D(rect.Location.X, rect.Location.Y));
				top.Add(new GRealPoint2D(rect.Location.X + rect.Width, rect.Location.Y));
				top.Add(new GRealPoint2D(rect.Location.X + rect.Width + 3, rect.Location.Y - 3));
				top.Add(new GRealPoint2D(rect.Location.X + 3, rect.Location.Y - 3));
				renderer.FillPolygon(top, GetHighlightColor(entry.Color, 1.2f));
				LineStrip right = new LineStrip(4);
				right.Add(new GRealPoint2D(rect.Location.X + rect.Width, rect.Location.Y + rect.Height));
				right.Add(new GRealPoint2D(rect.Location.X + rect.Width, rect.Location.Y));
				right.Add(new GRealPoint2D(rect.Location.X + rect.Width + 3, rect.Location.Y - 3));
				right.Add(new GRealPoint2D(rect.Location.X + rect.Width + 3, rect.Location.Y + rect.Height - 3));
				renderer.FillPolygon(right, GetHighlightColor(entry.Color, 0.7f));
			}
		}
		static void RenderBar3DCylinderAppearance(GdiPlusRenderer renderer, Bar3DSeriesViewAppearance barAppearance, ViewType viewType, PaletteEntry[] entries) {
			Rectangle[] rects;
			Rectangle[] topRects;
			Rectangle[] ellipseRects;
			int seriesCount = 3;
			int stackedSectionsPerBar = 3;
			bool stacked = false;
			switch (viewType) {
				case ViewType.Bar3D:
					rects = sideBySideCylinderRects;
					ellipseRects = sideBySideCylinderEllipseRects;
					topRects = sideBySideCylinderTopEllipseRects;
					break;
				case ViewType.StackedBar3D:
					rects = stackedCylinderRects;
					ellipseRects = stackedCylinderEllipseRects;
					topRects = stackedCylinderTopEllipseRects;
					stacked = true;
					break;
				case ViewType.FullStackedBar3D:
					rects = fullStackedCylinderRects;
					ellipseRects = fullStackedCylinderEllipseRects;
					topRects = fullStackedCylinderTopEllipseRects;
					stacked = true;
					break;
				case ViewType.SideBySideStackedBar3D:
					rects = sideBySideStackedCylinderRects;
					ellipseRects = sideBySideStackedCylinderEllipseRects;
					topRects = sideBySideStackedCylinderTopEllipseRects;
					seriesCount = 4;
					stackedSectionsPerBar = 2;
					stacked = true;
					break;
				case ViewType.SideBySideFullStackedBar3D:
					rects = sideBySideFullStackedCylinderRects;
					ellipseRects = sideBySideFullStackedCylinderEllipseRects;
					topRects = sideBySideFullStackedCylinderTopEllipseRects;
					seriesCount = 4;
					stackedSectionsPerBar = 2;
					stacked = true;
					break;
				default:
					rects = manhattanCylinderRects;
					ellipseRects = manhattanCylinderEllipseRects;
					topRects = manhattanCylinderTopEllipseRects;
					break;
			}
			renderer.SetPixelOffsetMode(PixelOffsetMode.HighQuality);
			renderer.EnableAntialiasing(true);
			try {
				for (int i = 0; i < ellipseRects.Length; i++) {
					PaletteEntry entry = entries[i % Math.Min(seriesCount, entries.Length)];
					using (Brush brush = GetCylinderGradientBrush(rects[i], entry.Color)) {
						renderer.FillEllipse(ellipseRects[i], brush);
						renderer.FillRectangle(rects[i], brush);
					}
				}
				for (int i = 0; i < topRects.Length; i++) {
					int entryIndex = stacked ? (i + 1) * stackedSectionsPerBar - 1 : i;
					entryIndex = entryIndex % Math.Min(seriesCount, entries.Length);
					PaletteEntry entry = entries[entryIndex];
					using (Brush brush = new SolidBrush(GetHighlightColor(entry.Color, 1.2f)))
						renderer.FillEllipse(topRects[i], brush);					
				}
			}
			finally {
				renderer.RestoreAntialiasing();
				renderer.RestorePixelOffsetMode();
			}
		}
		static void RenderBar3DPyramidAppearance(GdiPlusRenderer renderer, Bar3DSeriesViewAppearance barAppearance, ViewType viewType, PaletteEntry[] entries) {
			Rectangle[] rects;
			PlaneTriangle[] rightTrinagles;
			int seriesCount = 3;
			switch (viewType) {
				case ViewType.Bar3D:
					rects = sideBySidePyramidRects;
					rightTrinagles = sideBySidePyramidRightTriangles;
					break;
				case ViewType.StackedBar3D:
					rects = stackedPyramidRects;
					rightTrinagles = stackedPyramidRightTriangles;
					break;
				case ViewType.FullStackedBar3D:
					rects = fullStackedPyramidRects;
					rightTrinagles = fullStackedPyramidRightTriangles;
					break;
				case ViewType.SideBySideStackedBar3D:
					rects = sideBySideStackedPyramidRects;
					rightTrinagles = sideBySideStackedPyramidRightTriangles;
					seriesCount = 4;
					break;
				case ViewType.SideBySideFullStackedBar3D:
					rects = sideBySideFullStackedPyramidRects;
					rightTrinagles = sideBySideFullStackedPyramidRightTriangles;
					seriesCount = 4;
					break;
				default:
					rects = manhattanPyramidRects;
					rightTrinagles = manhattanPyramidRightTriangles;
					break;
			}
			renderer.EnableAntialiasing(true);
			try {
				for (int i = 0; i < rects.Length; i++) {
					PaletteEntry entry = entries[i % Math.Min(seriesCount, entries.Length)];
					using (Brush brush = new SolidBrush(entry.Color)) {
						GRealPoint2D leftBottom = new GRealPoint2D(rects[i].Left, rects[i].Top + rects[i].Height);
						GRealPoint2D rightBottom = new GRealPoint2D(rects[i].Left + rects[i].Width, rects[i].Top + rects[i].Height);
						GRealPoint2D peak = new GRealPoint2D(rects[i].Left + rects[i].Width / 2, rects[i].Top);
						LineStrip strip = new LineStrip(new GRealPoint2D[] { leftBottom, rightBottom, peak });
						renderer.FillPolygon(strip, brush);
					}
					using (Brush brush = new SolidBrush(GetHighlightColor(entry.Color, 0.7f))) {
						GRealPoint2D point0 = new GRealPoint2D((int)rightTrinagles[i].Vertices[0].X, (int)rightTrinagles[i].Vertices[0].Y);
						GRealPoint2D point1 = new GRealPoint2D((int)rightTrinagles[i].Vertices[1].X, (int)rightTrinagles[i].Vertices[1].Y);
						GRealPoint2D point2 = new GRealPoint2D((int)rightTrinagles[i].Vertices[2].X, (int)rightTrinagles[i].Vertices[2].Y);
						LineStrip strip = new LineStrip(new GRealPoint2D[] { point0, point1, point2 });
						renderer.FillPolygon(strip, brush);
					}
				}
			}
			finally {
				renderer.RestoreAntialiasing();
			}
		}
		static void RenderBar3DConeAppearance(GdiPlusRenderer renderer, Bar3DSeriesViewAppearance barAppearance, ViewType viewType, PaletteEntry[] entries) {
			Rectangle[] rects;
			Rectangle[] ellipseRects;
			int seriesCount = 3;
			switch (viewType) {
				case ViewType.Bar3D:
					rects = sideBySideConeRects;
					ellipseRects = sideBySideConeEllipseRects;
					break;
				case ViewType.StackedBar3D:
					rects = stackedConeRects;
					ellipseRects = stackedConeEllipseRects;
					break;
				case ViewType.FullStackedBar3D:
					rects = fullStackedConeRects;
					ellipseRects = fullStackedConeEllipseRects;
					break;
				case ViewType.SideBySideStackedBar3D:
					rects = sideBySideStackedConeRects;
					ellipseRects = sideBySideStackedConeEllipseRects;
					seriesCount = 4;
					break;
				case ViewType.SideBySideFullStackedBar3D:
					rects = sideBySideFullStackedConeRects;
					ellipseRects = sideBySideFullStackedConeEllipseRects;
					seriesCount = 4;
					break;
				default:
					rects = manhattanConeRects;
					ellipseRects = manhattanConeEllipseRects;
					break;
			}			
			renderer.EnableAntialiasing(true);
			try {
				for (int i = 0; i < ellipseRects.Length; i++) {
					PaletteEntry entry = entries[i % Math.Min(seriesCount, entries.Length)];
					using (Brush brush = GetConeGradientBrush(rects[i], entry.Color)) {
						renderer.SetPixelOffsetMode(PixelOffsetMode.HighQuality);
						renderer.FillPie(ellipseRects[i], 0, 180, brush);
						GRealPoint2D leftBottom = new GRealPoint2D(rects[i].Left, rects[i].Top + rects[i].Height);
						GRealPoint2D rightBottom = new GRealPoint2D(rects[i].Left + rects[i].Width, rects[i].Top + rects[i].Height);
						GRealPoint2D peak = new GRealPoint2D(rects[i].Left + rects[i].Width / 2, rects[i].Top);
						renderer.RestorePixelOffsetMode();
						LineStrip strip = new LineStrip(new GRealPoint2D[] { leftBottom, rightBottom, peak });
						renderer.FillPolygon(strip, brush);
					}
				}
			}
			finally {
				renderer.RestoreAntialiasing();
			}
		}
		static void RenderLine3DRightPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color) {
			for (int i = 0; i < vertices.Length - 1; i++) {
				LineStrip rect = new LineStrip(4);
				rect.Add(vertices[i + 1]);
				rect.Add(new GRealPoint2D(vertices[i + 1].X + line3Ddelta2, vertices[i + 1].Y + line3Ddelta2));
				rect.Add(new GRealPoint2D(vertices[i + 1].X + line3Ddelta1 + line3Ddelta2, vertices[i + 1].Y + line3Ddelta2 - line3Ddelta1));
				rect.Add(new GRealPoint2D(vertices[i + 1].X + line3Ddelta1, vertices[i + 1].Y - line3Ddelta1));
				renderer.FillPolygon(rect, color);
			}
		}
		static void RenderLine3DBottomPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color) {
			for (int i = 0; i < vertices.Length - 1; i++) {
				LineStrip rect = new LineStrip(4);
				rect.Add(vertices[i]);
				rect.Add(vertices[i + 1]);
				rect.Add(new GRealPoint2D(vertices[i + 1].X + line3Ddelta2, vertices[i + 1].Y + line3Ddelta2));
				rect.Add(new GRealPoint2D(vertices[i].X + line3Ddelta2, vertices[i].Y + line3Ddelta2));
				renderer.FillPolygon(rect, color);
			}
		}
		static void RenderLine3DTopPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color) {
			Color highlightColor = GetHighlightColor(color, 0.8f);
			for (int i = 0; i < vertices.Length - 1; i++) {
				LineStrip rect = new LineStrip(4);
				rect.Add(vertices[i]);
				rect.Add(vertices[i + 1]);
				rect.Add(new GRealPoint2D(vertices[i + 1].X + line3Ddelta1, vertices[i + 1].Y - line3Ddelta1));
				rect.Add(new GRealPoint2D(vertices[i].X + line3Ddelta1, vertices[i].Y - line3Ddelta1));
				renderer.FillPolygon(rect, vertices[i].Y > vertices[i + 1].Y ? highlightColor : color);
			}
		}
		static void RenderLine3D(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color) {
			RenderLine3DRightPolygon(renderer, vertices, color);
			RenderLine3DBottomPolygon(renderer, vertices, color);
			RenderLine3DTopPolygon(renderer, vertices, color);
		}
		static void RenderLine3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries, GRealPoint2D[] firstLine3D, GRealPoint2D[] secondLine3D) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			RenderLine3D(renderer, firstLine3D, entries[0].Color);
			RenderLine3D(renderer, secondLine3D, entries[1].Color);
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderSpline3DPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color, int deltaX, int deltaY) {
			BezierRangeStrip strip = new BezierRangeStrip(0.8);
			foreach (GRealPoint2D vertice in vertices)
				strip.Add(new StripRange(vertice, new GRealPoint2D(vertice.X + deltaX, vertice.Y + deltaY)));
			renderer.FillBezier(strip, color);
		}
		static void RenderSpline3DTopPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color) {
			RenderSpline3DPolygon(renderer, vertices, color, line3Ddelta1, -line3Ddelta1);
		}
		static void RenderSpline3DBottomPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color) {
			RenderSpline3DPolygon(renderer, vertices, color, line3Ddelta2, line3Ddelta2);
		}
		static void RenderSpline3D(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color) {
			RenderLine3DRightPolygon(renderer, vertices, color);
			RenderSpline3DBottomPolygon(renderer, vertices, color);
			RenderSpline3DTopPolygon(renderer, vertices, color);			
		}
		static void RenderSpline3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {					  
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);			
			RenderSpline3D(renderer, firstLine3D, entries[0].Color);
			RenderSpline3D(renderer, secondLine3D, entries[1].Color);		   
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();	
		}
		static void RenderStepLine3D(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color) {
			Color highlightColor = GetHighlightColor(color, 0.7f);
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			int count = vertices.Length - 1;
			for (int i = 0; i < count; i++) {
				LineStrip polygon = new LineStrip(4);
				polygon.Add(vertices[i]);
				polygon.Add(vertices[i + 1]);
				polygon.Add(new GRealPoint2D(vertices[i + 1].X + 5, vertices[i + 1].Y - 5));
				polygon.Add(new GRealPoint2D(vertices[i].X + 5, vertices[i].Y - 5));
				renderer.FillPolygon(polygon, polygon[0].X == polygon[1].X ? highlightColor : color);
			}
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderStepLine3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			RenderStepLine3D(renderer, firstStepLine3D, entries[0].Color);
			RenderStepLine3D(renderer, secondStepLine3D, entries[1].Color);
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static GRealPoint2D Sum(GRealPoint2D p1, GRealPoint2D p2) {
			return new GRealPoint2D(p1.X + p2.X, p1.Y + p2.Y);
		}
		static void RenderArea3DTopPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color, GRealPoint2D startPoint) {
			Color color1 = GetHighlightColor(color, 1.1f) ;
			Color color2 = GetHighlightColor(color, 0.9f);
			for (int i = 0; i < vertices.Length - 1; i++) {
				LineStrip rect = new LineStrip(4);
				rect.Add(Sum(vertices[i], startPoint));
				rect.Add(Sum(vertices[i + 1], startPoint));
				rect.Add(new GRealPoint2D(rect[1].X + line3Ddelta1, rect[1].Y - line3Ddelta1));
				rect.Add(new GRealPoint2D(rect[0].X + line3Ddelta1, rect[0].Y - line3Ddelta1));
				renderer.FillPolygon(rect, vertices[i].Y > vertices[i + 1].Y ? color1 : color2);
			}
		}
		static void RenderArea3DBottomPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color, GRealPoint2D startPoint) {
			LineStrip rect = new LineStrip(vertices.Length);
			foreach (GRealPoint2D point in vertices)
				rect.Add(Sum(point, startPoint));
			renderer.FillPolygon(rect, color);
		}
		static void RenderArea3D(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color, GRealPoint2D startPoint) {
			RenderArea3D(renderer, vertices, color, startPoint, 3);
		}
		static void RenderArea3D(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color, GRealPoint2D startPoint, int topAreaVertexCount) {
			GRealPoint2D[] topArea = new GRealPoint2D[topAreaVertexCount];
			for (int i = 0, j = 1; i < topAreaVertexCount; i++, j++)
				topArea[i] = vertices[j];
			RenderArea3DTopPolygon(renderer, topArea, color, startPoint);
			GRealPoint2D[] rightArea = new GRealPoint2D[2];
			for (int i = 0, j = topAreaVertexCount; i < 2; i++, j++)
				rightArea[i] = vertices[j];
			RenderArea3DTopPolygon(renderer, rightArea, color, startPoint);
			RenderArea3DBottomPolygon(renderer, vertices, GetHighlightColor(color, 1.1f), startPoint);
		}
		static void RenderArea3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			GRealPoint2D startPoint = new GRealPoint2D(area3DDeltaAxis - 5, -area3DDeltaAxis);
			RenderArea3D(renderer, firstArea3DPolygon, entries[0].Color, startPoint);
			RenderArea3D(renderer, secondArea3DPolygon, entries[1].Color, new GRealPoint2D(-5, 0));
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderStackedArea3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			GRealPoint2D startPoint = new GRealPoint2D(area3DDeltaAxis - 5, -area3DDeltaAxis);
			RenderArea3D(renderer, firstStackedArea3DPolygon, entries[0].Color, new GRealPoint2D(0, 0));
			RenderArea3D(renderer, secondStackedArea3DPolygon, entries[1].Color, new GRealPoint2D(0, 0));
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderFullStackedArea3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			GRealPoint2D startPoint = new GRealPoint2D(area3DDeltaAxis - 5, -area3DDeltaAxis);
			RenderArea3D(renderer, firstFullStackedArea3DPolygon, entries[0].Color, new GRealPoint2D(0, 0));
			RenderArea3D(renderer, secondFullStackedArea3DPolygon, entries[1].Color, new GRealPoint2D(0, 0));
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderStepArea3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			RenderArea3D(renderer, stepAreaPolygon1, entries[0].Color, new GRealPoint2D(0, 0), 4);
			RenderArea3D(renderer, stepAreaPolygon2, entries[1].Color, new GRealPoint2D(0, 0), 4);
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderRangeArea3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			RenderArea3D(renderer, rangeAreaPolygon1, entries[0].Color, new GRealPoint2D(0, 0));
			RenderArea3D(renderer, rangeAreaPolygon2, entries[1].Color, new GRealPoint2D(0, 0));
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderSplineArea3DTopPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color, GRealPoint2D startPoint) {
			BezierRangeStrip strip = new BezierRangeStrip(0.8);
			foreach (GRealPoint2D vertice in vertices) {
				GRealPoint2D point = Sum(vertice, startPoint);
				strip.Add(new StripRange(point, new GRealPoint2D(point.X + line3Ddelta1, point.Y - line3Ddelta1)));
			}
			renderer.FillBezier(strip, color);
		}
		static void RenderSplineArea3DBottomPolygon(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color, GRealPoint2D startPoint) {
			BezierRangeStrip strip = new BezierRangeStrip(0.8);
			strip.TopStrip.AddRange(new GRealPoint2D[] { Sum(vertices[1], startPoint), Sum(vertices[2], startPoint), Sum(vertices[3], startPoint) });
			strip.BottomStrip.AddRange(new GRealPoint2D[] { Sum(vertices[0], startPoint), Sum(vertices[5], startPoint), Sum(vertices[4], startPoint) });
			renderer.FillBezier(strip, color);
		}
		static void RenderSplineArea3D(GdiPlusRenderer renderer, GRealPoint2D[] vertices, Color color, GRealPoint2D startPoint) {
			Color topColor = GetHighlightColor(color, 0.9f);
			GRealPoint2D[] topArea = new GRealPoint2D[3];
			for (int i = 0, j = 1; i < 3; i++, j++)
				topArea[i] = vertices[j];
			RenderSplineArea3DTopPolygon(renderer, topArea, topColor, startPoint);
			GRealPoint2D[] rightArea = new GRealPoint2D[2];
			for (int i = 0, j = 3; i < 2; i++, j++)
				rightArea[i] = vertices[j];
			RenderSplineArea3DTopPolygon(renderer, rightArea, topColor, startPoint);
			RenderSplineArea3DBottomPolygon(renderer, vertices, GetHighlightColor(color, 1.1f), startPoint);
		}
		static void RenderSplineArea3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			GRealPoint2D startPoint = new GRealPoint2D(area3DDeltaAxis - 5, -area3DDeltaAxis);
			RenderSplineArea3D(renderer, firstAreaSpline3DPolygon, entries[0].Color, startPoint);
			RenderSplineArea3D(renderer, secondAreaSpline3DPolygon, entries[1].Color, new GRealPoint2D(-5, 0));
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderStackedSplineArea3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			GRealPoint2D startPoint = new GRealPoint2D(area3DDeltaAxis - 5, -area3DDeltaAxis);
			RenderSplineArea3D(renderer, firstStackedSplineArea3DPolygon, entries[0].Color, new GRealPoint2D(0, 0));
			RenderSplineArea3D(renderer, secondStackedArea3DPolygon, entries[1].Color, new GRealPoint2D(0, 0));
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderFullStackedSplineArea3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			GRealPoint2D startPoint = new GRealPoint2D(area3DDeltaAxis - 5, -area3DDeltaAxis);
			RenderSplineArea3D(renderer, firstSplineFullStackedArea3DPolygon, entries[0].Color, new GRealPoint2D(0, 0));
			RenderSplineArea3D(renderer, secondSplineFullStackedArea3DPolygon, entries[1].Color, new GRealPoint2D(0, 0));
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();			
		}
		static void RenderPieAppearance(GdiPlusRenderer renderer, PieSeriesViewAppearance pieAppearance, Palette palette, int colorIndex) {
			RenderPieAppearance(renderer, pieAppearance, palette, colorIndex, 19.0f, 0.0f, pieAngles);
		}
		static void RenderDoughnutAppearance(GdiPlusRenderer renderer, PieSeriesViewAppearance pieAppearance, Palette palette, int colorIndex) {
			RenderPieAppearance(renderer, pieAppearance, palette, colorIndex, 19.0f, 0.6f, pieAngles);
		}
		static void RenderNestedDoughnutAppearance(GdiPlusRenderer renderer, PieSeriesViewAppearance pieAppearance, Palette palette, int colorIndex) {
			RenderPieAppearance(renderer, pieAppearance, palette, colorIndex, 19.0f, 0.74f, outerDoughnutAngles);
			RenderPieAppearance(renderer, pieAppearance, palette, colorIndex, 12.0f, 0.55f, innerDoughnutAngles);
		}
		static void RenderPieAppearance(GdiPlusRenderer renderer, PieSeriesViewAppearance pieAppearance, Palette palette, int colorIndex, float radius, float holePercent, float[] pieAngles) {
			PaletteEntry[] entries = new PaletteEntry[pieAngles.Length];
			for (int i = 0; i < pieAngles.Length; i++)
				entries[i] = palette.GetEntry(i, pieAngles.Length, colorIndex);
			PointF center = new PointF(40.0f, 25.0f);
			RectangleF bounds = new RectangleF(new PointF(center.X, center.Y), SizeF.Empty);
			bounds.Inflate(radius, radius);
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			float angle = -90.0f;
			FillOptionsBase fillOptions = pieAppearance.FillStyle.Options;
			for (int i = 0; i < pieAngles.Length; i++) {
				PaletteEntry entry = entries[i];
				float sweepAngle = pieAngles[i];
				fillOptions.RenderPie(renderer, center,
					radius, radius, angle, sweepAngle, 0.0f, holePercent, bounds, entry.Color, entry.Color2);
				angle += sweepAngle;
			}
			float innerRadius = radius * holePercent;
			RenderPieInnerBorders(renderer, pieAppearance.BorderColor, center, radius, innerRadius, pieAngles);
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderPieInnerBorders(GdiPlusRenderer renderer, Color borderColor, PointF center, float outerRadius, float innerRadius, float[] pieAngles) {
			float angleRad = (float)(-Math.PI / 2.0);
			foreach (float sweepAngle in pieAngles) {
				float cos = (float)Math.Cos(angleRad);
				float sin = (float)Math.Sin(angleRad);
				PointF p1 = new PointF(center.X + innerRadius * cos, center.Y + innerRadius * sin);
				PointF p2 = new PointF(center.X + outerRadius * cos, center.Y + outerRadius * sin);
				renderer.DrawLine(p1, p2, borderColor, 1);
				angleRad += (float)(sweepAngle * Math.PI / 180.0);
			}
			renderer.DrawCircle(center, outerRadius, borderColor, 1);
			if (innerRadius != 0.0f)
				renderer.DrawCircle(center, innerRadius, borderColor, 1);
		}
		static void RenderPie3DAppearance(GdiPlusRenderer renderer, Pie3DSeriesViewAppearance pieAppearance, ViewType viewType, Palette palette, int colorIndex) {
			const int majorSemiAxis = 19, minorSemiAxis = 10;
			PaletteEntry[] entries = new PaletteEntry[pieAngles.Length];
			for (int i = 0; i < pieAngles.Length; i++)
				entries[i] = palette.GetEntry(i, pieAngles.Length, colorIndex);
			Point center = new Point(40, 20);
			Rectangle bounds = new Rectangle(center, Size.Empty);
			bounds.Inflate(new Size(majorSemiAxis, minorSemiAxis));
			float centerY = bounds.Top + bounds.Height / 2;
			try {
				renderer.EnableAntialiasing(true);
				using (GraphicsPath path = new GraphicsPath()) {
					const int height = 7;
					path.AddPie(bounds.Left, bounds.Top + height, bounds.Width, bounds.Height, 0.0f, pieAngles[0] - 90.0f);
					RectangleF pathBounds = path.GetBounds();
					LineStrip polygon = new LineStrip(4);
					polygon.Add(new GRealPoint2D(pathBounds.Location.X, pathBounds.Location.Y));
					polygon.Add(new GRealPoint2D(pathBounds.Left, pathBounds.Bottom));
					using (Brush brush = new SolidBrush(GetHighlightColor(entries[1].Color, 0.7f))) {
						renderer.FillPie(new Rectangle(bounds.Left, bounds.Top + height, bounds.Width, bounds.Height), pieAngles[0] - 90.0f, 270.0f - pieAngles[0], brush);
						polygon.Add(new GRealPoint2D(bounds.Left, centerY + height));
						polygon.Add(new GRealPoint2D(bounds.Left, centerY));
						renderer.FillPolygon(polygon, brush);
					}
					polygon = new LineStrip(5);
					polygon.Add(new GRealPoint2D(pathBounds.Location.X, pathBounds.Location.Y));
					polygon.Add(new GRealPoint2D(pathBounds.Left, pathBounds.Bottom));
					using (Brush brush = new SolidBrush(GetHighlightColor(entries[0].Color, 0.7f))) {
						renderer.FillPath(path, brush);
						polygon.Add(new GRealPoint2D(bounds.Right, centerY + height));
						polygon.Add(new GRealPoint2D(bounds.Right, centerY));
						polygon.Add(new GRealPoint2D((pathBounds.Left + bounds.Right) / 2.0f, pathBounds.Location.Y));
						renderer.FillPolygon(polygon, brush);
					}
					if (viewType == ViewType.Doughnut3D) {
						PointF location = new PointF(bounds.Location.X + bounds.Width * 0.2f, bounds.Location.Y + bounds.Height * 0.2f);
						SizeF size = new SizeF(bounds.Width * 0.6f, bounds.Height * 0.6f);
						RectangleF upBounds = new RectangleF(location, size);
						RectangleF downBounds = new RectangleF(new PointF(location.X, location.Y + height), size);
						float startAngle = pieAngles[0] + pieAngles[1] - 90.0f;
						for (int i = 2; i <= 3; i++)
							using (GraphicsPath innerPath = new GraphicsPath()) {
								float sweepAngle = pieAngles[i];
								innerPath.AddArc(bounds, startAngle, sweepAngle);
								innerPath.AddArc(downBounds, startAngle + sweepAngle, -sweepAngle);
								using (Brush brush = new SolidBrush(GetHighlightColor(entries[i].Color, 0.7f)))
									renderer.FillPath(innerPath, brush);
								startAngle += sweepAngle;
							}
						using (GraphicsPath innerPath = new GraphicsPath()) {
							float sweepAngle = 90.0f;
							innerPath.AddArc(bounds, startAngle, sweepAngle);
							innerPath.AddArc(downBounds, startAngle + sweepAngle, -sweepAngle);
							using (Brush brush = new SolidBrush(GetHighlightColor(entries[0].Color, 0.5f)))
								renderer.FillPath(innerPath, brush);
						}
					}
				}
			}
			finally {
				renderer.RestoreAntialiasing();
			}
			renderer.EnableAntialiasing(true);
			renderer.SetPixelOffsetMode(PixelOffsetMode.Default);
			float angle = -90.0f;
			float holePercent = viewType == ViewType.Doughnut3D ? 0.6f : 0.0f;
			for (int i = 0; i < pieAngles.Length; i++) {
				PaletteEntry entry = entries[i];
				float sweepAngle = pieAngles[i];
				pieAppearance.FillStyle.Options.RenderPie(renderer, new PointF(center.X, center.Y),
					majorSemiAxis, minorSemiAxis, angle, sweepAngle, 0.0f, holePercent, bounds, entry.Color, entry.Color2);				
				angle += sweepAngle;
			}
			renderer.RestorePixelOffsetMode();
			renderer.RestoreAntialiasing();
		}
		static void RenderFunnelAppearance(GdiPlusRenderer renderer, FunnelSeriesViewAppearance funnelAppearance, PaletteEntry[] entries) {
			LineStrip list1 = new LineStrip();
			list1.Add(new GRealPoint2D(15, 10));
			list1.Add(new GRealPoint2D(65, 10));
			list1.Add(new GRealPoint2D(55, 20));
			list1.Add(new GRealPoint2D(25, 20));
			LineStrip list2 = new LineStrip();
			list2.Add(new GRealPoint2D(25, 20));
			list2.Add(new GRealPoint2D(55, 20));
			list2.Add(new GRealPoint2D(45, 30));
			list2.Add(new GRealPoint2D(35, 30));
			LineStrip list3 = new LineStrip();
			list3.Add(new GRealPoint2D(35, 30));
			list3.Add(new GRealPoint2D(45, 30));
			list3.Add(new GRealPoint2D(45, 40));
			list3.Add(new GRealPoint2D(35, 40));
			VariousPolygon[] funnelPolygons =  new VariousPolygon[] {
				new VariousPolygon(list1, new RectangleF(0, 10, 80, 10)),
				new VariousPolygon(list2, new RectangleF(0, 20, 80, 10)),
				new VariousPolygon(list3, new RectangleF(0, 30, 80, 10))
			};
			using (GraphicsCommand command = new ContainerGraphicsCommand()) {
				FillOptionsBase fillOptions = funnelAppearance.FillStyle.Options;
				int entryIndex = 0;
				foreach (VariousPolygon polygon in funnelPolygons) {
					PaletteEntry entry = entries[entryIndex];
					fillOptions.Render(renderer, polygon.Vertices, polygon.Rect, entry.Color, entry.Color2);
					if (++entryIndex >= entries.Length)
						entryIndex = 0;
				}
				Color borderColor = funnelAppearance.BorderColor;
				if (!borderColor.IsEmpty)
					foreach (VariousPolygon polygon in funnelPolygons)
						renderer.DrawPolygon(polygon.Vertices, borderColor, 1);
			}
		}
		static void RenderFunnel3DAppearance(GdiPlusRenderer renderer, PaletteEntry[] entries) {
			renderer.EnableAntialiasing(true);
			try {
				using (Brush brush = new SolidBrush((Color)entries[2].Color)) {
					renderer.FillEllipse(smallFunnel3DEllipseRect, brush);
					renderer.FillRectangle(smallFunnel3DRect, brush);				
				}
				using (Brush brush = new SolidBrush((Color)entries[1].Color)) {
					renderer.FillEllipse(middleFunnel3DEllipseRect, brush);
					renderer.FillPolygon(new LineStrip(middleFunnel3DPolygonPoints), brush);
				}
				using (Brush brush = new SolidBrush((Color)entries[0].Color)) {
					renderer.FillEllipse(bigFunnel3DEllipseRect, brush);
					renderer.FillPolygon(new LineStrip(bigFunnel3DPolygonPoints), brush);
				}
				using (Brush brush = new SolidBrush(GetHighlightColor(entries[0].Color, 1.3f)))
					renderer.FillEllipse(topFunnel3DEllipseRect, brush);
			}
			finally {
				renderer.RestoreAntialiasing();
			}
		}
		public static Image CreateImage(ViewType viewType, ChartAppearance appearance, Palette palette, int colorIndex) {
			int entryCount = 4;
			PaletteEntry[] entries = new PaletteEntry[entryCount];
			for (int i = 0; i < entryCount; i++)
				entries[i] = palette.GetEntry(i, entryCount, colorIndex);
			Bitmap image = new Bitmap(80, 50);
			try {
				using (Graphics gr = Graphics.FromImage(image)) {
					GdiPlusRenderer renderer = new GdiPlusRenderer();
					renderer.Reset(gr, new Rectangle(new Point(0, 0), image.Size));
					IChartAppearance actualAppearance = appearance;
					RenderChartAppearance(renderer, actualAppearance.WholeChartAppearance);
					switch (viewType) {
						case ViewType.Bar:
						case ViewType.StackedBar:
						case ViewType.FullStackedBar:
						case ViewType.SideBySideStackedBar:
						case ViewType.SideBySideFullStackedBar:
						case ViewType.SideBySideRangeBar:
						case ViewType.RangeBar:
						case ViewType.SideBySideGantt:
						case ViewType.Gantt:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderBarAppearance(renderer, actualAppearance.BarSeriesViewAppearance, viewType, entries);
							break;
						case ViewType.Point:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderPointAppearance(renderer, actualAppearance, entries, points, new Size[0]);
							break;
						case ViewType.Bubble:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderPointAppearance(renderer, actualAppearance, entries, points, bubbleSizes);
							break;
						case ViewType.Line:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderLineAppearance(renderer, actualAppearance, points, false, entries);
							break;
						case ViewType.StackedLine:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderLineAppearance(renderer, actualAppearance, stackedPoints, false, entries);
							break;
						case ViewType.FullStackedLine:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderLineAppearance(renderer, actualAppearance, fullStackedPoints, false, entries);
							break;
						case ViewType.StepLine:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderLineAppearance(renderer, actualAppearance, points, true, entries);
							break;
						case ViewType.SwiftPlot:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderSwiftPlotAppearance(renderer, entries);
							break;
						case ViewType.ScatterLine:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderScatterLineAppearance(renderer, actualAppearance, entries);
							break;
						case ViewType.Spline:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderSplineAppearance(renderer, actualAppearance, entries);
							break;
						case ViewType.Area:
						case ViewType.StackedArea:
						case ViewType.FullStackedArea:
						case ViewType.StepArea:
						case ViewType.RangeArea:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderAreaAppearance(renderer, actualAppearance.AreaSeriesViewAppearance, viewType, entries);
							break;
						case ViewType.SplineArea:
						case ViewType.StackedSplineArea:
						case ViewType.FullStackedSplineArea:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderSplineAreaAppearance(renderer, actualAppearance.AreaSeriesViewAppearance, viewType, entries);
							break;
						case ViewType.Stock:
						case ViewType.CandleStick:
							RenderXYDiagramAppearance(renderer, actualAppearance.XYDiagramAppearance);
							RenderFinancialAppearance(renderer, viewType);
							break;
						case ViewType.PolarPoint:
						case ViewType.RadarPoint:
							RenderRadarDiagramAppearance(renderer, actualAppearance.RadarDiagramAppearance);
							RenderPointAppearance(renderer, actualAppearance, entries, radarPoints, new Size[0]);
							break;
						case ViewType.PolarLine:
						case ViewType.RadarLine:
							RenderRadarDiagramAppearance(renderer, actualAppearance.RadarDiagramAppearance);
							RenderRadarLineAppearance(renderer, actualAppearance, entries);
							break;
						case ViewType.PolarArea:
						case ViewType.RadarArea:
							RenderRadarDiagramAppearance(renderer, actualAppearance.RadarDiagramAppearance);
							RenderRadarAreaAppearance(renderer, actualAppearance, entries);
							break;
						case ViewType.Pie:
							RenderPieAppearance(renderer, actualAppearance.PieSeriesViewAppearance, palette, colorIndex);
							break;
						case ViewType.Doughnut:
							RenderDoughnutAppearance(renderer, actualAppearance.PieSeriesViewAppearance, palette, colorIndex);
							break;
						case ViewType.NestedDoughnut:
							RenderNestedDoughnutAppearance(renderer, actualAppearance.PieSeriesViewAppearance, palette, colorIndex);
							break;
						case ViewType.Funnel:
							RenderFunnelAppearance(renderer, actualAppearance.FunnelSeriesViewAppearance, entries);
							break;
						case ViewType.Bar3D:
						case ViewType.StackedBar3D:
						case ViewType.FullStackedBar3D:
						case ViewType.SideBySideStackedBar3D:
						case ViewType.SideBySideFullStackedBar3D:
						case ViewType.ManhattanBar:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderBar3DAppearance(renderer, actualAppearance.Bar3DSeriesViewAppearance, viewType, entries);
							break;
						case ViewType.Line3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderLine3DAppearance(renderer, entries, firstLine3D, secondLine3D);
							break;
						case ViewType.StackedLine3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderLine3DAppearance(renderer, entries, firstStackedLine3D, secondStackedLine3D);
							break;
						case ViewType.FullStackedLine3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderLine3DAppearance(renderer, entries, firstFullStackedLine3D, secondFullStackedLine3D);
							break;
						case ViewType.StepLine3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderStepLine3DAppearance(renderer, entries);
							break;
						case ViewType.Spline3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderSpline3DAppearance(renderer, entries);
							break;
						case ViewType.Area3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderArea3DAppearance(renderer, entries);
							break;
						case ViewType.StackedArea3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderStackedArea3DAppearance(renderer, entries);
							break;
						case ViewType.FullStackedArea3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderFullStackedArea3DAppearance(renderer, entries);
							break;
						case ViewType.StepArea3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderStepArea3DAppearance(renderer, entries);
							break;
						case ViewType.SplineArea3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderSplineArea3DAppearance(renderer, entries);
							break;
						case ViewType.StackedSplineArea3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderStackedSplineArea3DAppearance(renderer, entries);
							break;
						case ViewType.FullStackedSplineArea3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderFullStackedSplineArea3DAppearance(renderer, entries);
							break;
						case ViewType.RangeArea3D:
							RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
							RenderRangeArea3DAppearance(renderer, entries);
							break;
						case ViewType.Pie3D:
						case ViewType.Doughnut3D:
							RenderPie3DAppearance(renderer, actualAppearance.Pie3DSeriesViewAppearance, viewType, palette, colorIndex);
							break;
						case ViewType.Funnel3D:
							RenderFunnel3DAppearance(renderer, entries);
							break;
					}
					RenderBorder(renderer, actualAppearance.WholeChartAppearance);
					renderer.Present();
				}
			}
			catch {
				image.Dispose();
				throw;
			}
			return image;
		}
		public static Image CreateImage(ViewType viewType, ChartAppearance appearance, Palette palette, int colorIndex, Bar3DModel model) {
			if (viewType != ViewType.Bar3D && viewType != ViewType.StackedBar3D && viewType != ViewType.FullStackedBar3D &&
				viewType != ViewType.SideBySideStackedBar3D && viewType != ViewType.SideBySideFullStackedBar3D && viewType != ViewType.ManhattanBar)
				return CreateImage(viewType, appearance, palette, colorIndex);
			int entryCount = 4;
			PaletteEntry[] entries = new PaletteEntry[entryCount];
			for (int i = 0; i < entryCount; i++)
				entries[i] = palette.GetEntry(i, entryCount, colorIndex);
			Bitmap image = new Bitmap(80, 50);
			try {
				using (Graphics gr = Graphics.FromImage(image)) {
					GdiPlusRenderer renderer = new GdiPlusRenderer();
					renderer.Reset(gr, new Rectangle(new Point(0, 0), image.Size));
					IChartAppearance actualAppearance = appearance;
					RenderChartAppearance(renderer, actualAppearance.WholeChartAppearance);
					RenderXYDiagram3DAppearance(renderer, actualAppearance.XYDiagram3DAppearance, actualAppearance.StripAppearance);
					switch (model) {
						case Bar3DModel.Cylinder:
							RenderBar3DCylinderAppearance(renderer, actualAppearance.Bar3DSeriesViewAppearance, viewType, entries);
							break;
						case Bar3DModel.Pyramid:
							RenderBar3DPyramidAppearance(renderer, actualAppearance.Bar3DSeriesViewAppearance, viewType, entries);
							break;
						case Bar3DModel.Cone:
							RenderBar3DConeAppearance(renderer, actualAppearance.Bar3DSeriesViewAppearance, viewType, entries);
							break;
						case Bar3DModel.Box:
							RenderBar3DAppearance(renderer, actualAppearance.Bar3DSeriesViewAppearance, viewType, entries);
							break;
					}
					RenderBorder(renderer, actualAppearance.WholeChartAppearance);
					renderer.Present();
				}
			}
			catch {
				image.Dispose();
				throw;
			}
			return image;
		}
	}
}
