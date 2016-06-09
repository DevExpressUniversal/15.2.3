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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.XtraDiagram.Base;
using PlatformPoint = System.Windows.Point;
namespace DevExpress.XtraDiagram.Extensions {
	public static class PointExtensions {
		public static PlatformPoint ToPlatformPoint(this Point point) {
			return new PlatformPoint(point.X, point.Y);
		}
		public static PointFloat ToPointFloat(this Point point) {
			return new PointFloat(point.X, point.Y);
		}
		public static PointPair CreateHLine(this Point point, int lineLength) {
			return new PointPair(new Point(point.X - lineLength / 2, point.Y), new Point(point.X + lineLength / 2, point.Y));
		}
		public static PointPair CreateVLine(this Point point, int lineLength) {
			return new PointPair(new Point(point.X, point.Y - lineLength / 2), new Point(point.X, point.Y + lineLength / 2));
		}
		public static Point Rotate(this Point point, Point origin, double angle) {
			Point newPoint = Point.Empty;
			using(Matrix matrix = new Matrix()) {
				matrix.RotateAt((float)angle, origin);
				Point[] points = new Point[] { point };
				matrix.TransformPoints(points);
				newPoint = points.Single();
			}
			return newPoint;
		}
		public static Point Invert(this Point point) {
			return new Point(-point.X, -point.Y);
		}
	}
	public static class PointFloatExtensions {
		public static PlatformPoint ToPlatformPoint(this PointFloat point) {
			return new PlatformPoint(point.X, point.Y);
		}
		public static Point ToPoint(this PointFloat point) {
			return new Point((int)point.X, (int)point.Y);
		}
	}
	public static class PointFExtensions {
		public static PointFloat ToPointFloat(this PointF point) {
			return new PointFloat(point.X, point.Y);
		}
	}
	public static class PlatformPointExtensions {
		public static Point ToWinPoint(this PlatformPoint point) {
			return new Point((int)point.X, (int)point.Y);
		}
		public static PointF ToWinPointF(this PlatformPoint point) {
			return new PointF((float)point.X, (float)point.Y);
		}
		public static PointFloat ToPointFloat(this PlatformPoint point) {
			return new PointFloat((float)point.X, (float)point.Y);
		}
	}
}
