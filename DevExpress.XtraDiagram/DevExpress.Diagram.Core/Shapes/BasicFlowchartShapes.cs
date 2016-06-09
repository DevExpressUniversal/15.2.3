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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Diagram.Core {
	public partial class BasicFlowchartShapes {
		const double DatabaseOffset = 10;
		const double DataOffset = 20;
		const double DocumentOffset = 20;
		const double CustomOffset = 25;
		static DiagramItemStyleId GetStyleId(string id) {
			if(id == "StartEnd")
				return DefaultDiagramStyleId.Variant3;
			if(id == "Data" || id == "Database" || id == "ExternalData")
				return DefaultDiagramStyleId.Variant4;
			return DefaultDiagramStyleId.Variant1;
		}
		static ShapeGeometry GetStartEndPoints(double width, double height) {
			double offset = Math.Min(width, height) / 2;
			Size size = new Size(offset, offset);
			return new ShapeGeometry(
				StartSegment.Create(offset, height),
				ArcSegment.Create(offset, 0, size, SweepDirection.Clockwise),
				LineSegment.Create(width - offset, 0),
				ArcSegment.Create(width - offset, height, size, SweepDirection.Clockwise),
				LineSegment.Create(width - offset, height)
			);
		}
		internal static IEnumerable<Point> GetStartEndConnectionPoints(double width, double height) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		private static ShapeGeometry GetDataPoints(double width, double height) {
			return BasicShapes.GetParallelogramPoints(width, height, new double[] { DataOffset / width });
		}
		internal static IEnumerable<Point> GetDataConnectionPoints(double width, double height) {
			return new List<Point> { new Point(width / 2, 0), new Point(width - DataOffset / 2, height / 2), new Point(width / 2, height), new Point(DataOffset / 2, height / 2) };
		}
		private static ShapeGeometry GetDatabasePoints(double width, double height) {
			double offset = DatabaseOffset;
			Size size = new Size(offset, height / 2);
			return new ShapeGeometry(
				StartSegment.Create(offset, height),
				ArcSegment.Create(offset, 0, size, SweepDirection.Clockwise),
				LineSegment.Create(width - offset, 0),
				ArcSegment.Create(width - offset, height, size, SweepDirection.Clockwise),
				LineSegment.Create(width - offset, height),
				StartSegment.Create(width - offset, 0, GeometryKind.None),
				ArcSegment.Create(width - offset, height, size, SweepDirection.Counterclockwise)
			);
		}
		internal static IEnumerable<Point> GetDatabaseConnectionPoints(double width, double height) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		private static ShapeGeometry GetExternalDataPoints(double width, double height) {
			Size size = new Size(DatabaseOffset, height / 2);
			return new ShapeGeometry(
				StartSegment.Create(DatabaseOffset, height, isSmoothJoin: true),
				ArcSegment.Create(DatabaseOffset, 0, size, SweepDirection.Clockwise),
				LineSegment.Create(width , 0),
				ArcSegment.Create(width, height, size, SweepDirection.Counterclockwise)
			);
		}
		internal static IEnumerable<Point> GetExternalDataConnectionPoints(double width, double height) {
			yield return new Point(width / 2, 0);
			yield return new Point(width - DatabaseOffset, height / 2);
			yield return new Point(width / 2, height);
			yield return new Point(0, height / 2);
		}
		static double GetCustomOffset(double length) {
			if(length > 2 * CustomOffset)
				return CustomOffset;
			return length / 2;
		}
		private static ShapeGeometry GetCustom1Points(double width, double height) {
			return new ShapeGeometry(
				StartSegment.Create(0, GetCustomOffset(height)),
				LineSegment.Create(width, 0),
				LineSegment.Create(width, height),
				LineSegment.Create(0, height)
			);
		}
		internal static IEnumerable<Point> GetCustom1ConnectionPoints(double width, double height) {
			yield return new Point(width / 2, GetCustomOffset(height) / 2);
			yield return new Point(width, height / 2);
			yield return new Point(width / 2, height);
			yield return new Point(0, height / 2);
		}
		private static ShapeGeometry GetCustom2Points(double width, double height) {
			double offset = GetCustomOffset(width / 2);
			return new ShapeGeometry(
				StartSegment.Create(0, 0),
				LineSegment.Create(width, 0),
				LineSegment.Create(width - offset, height),
				LineSegment.Create(offset, height)
			);
		}
		internal static IEnumerable<Point> GetCustom2ConnectionPoints(double width, double height) {
			double offset = GetCustomOffset(width / 2);
			yield return new Point(width / 2, 0);
			yield return new Point(width - offset / 2, height / 2);
			yield return new Point(width / 2, height);
			yield return new Point(offset / 2, height / 2);
		}
		private static ShapeGeometry GetCustom3Points(double width, double height) {
			return new ShapeGeometry(
				StartSegment.Create(0, GetCustomOffset(height)),
				LineSegment.Create(GetCustomOffset(width), 0),
				LineSegment.Create(width, 0),
				LineSegment.Create(width, height),
				LineSegment.Create(0, height)
			);
		}
		internal static IEnumerable<Point> GetCustom3ConnectionPoints(double width, double height) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
		private static ShapeGeometry GetCustom4Points(double width, double height) {
			double offset = GetCustomOffset(width / 2);
			return new ShapeGeometry(
				StartSegment.Create(0, height / 2),
				LineSegment.Create(offset, 0),
				LineSegment.Create(width - offset, 0),
				LineSegment.Create(width, height / 2),
				LineSegment.Create(width - offset, height),
				LineSegment.Create(offset, height)
			);
		}
		internal static IEnumerable<Point> GetCustom4ConnectionPoints(double width, double height) {
			return BasicShapes.GetRectangleConnectionPoints(width, height);
		}
	}
}
