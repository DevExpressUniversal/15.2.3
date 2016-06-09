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
using System.Windows;
using System.Windows.Media.Media3D;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class PlaneRectangle : PlanePolygon {
		protected PlaneRectangle()
			: base() {
			TextureCoordsEnabled = true;
		}
		public PlaneRectangle(Point3D point, double width, double height)
			: this() {
			Init(point, width, height);
		}
		protected abstract void Init(Point3D point, double width, double height);
	}
	public class XPlaneRectangle : PlaneRectangle {
		protected XPlaneRectangle()
			: base() {
		}
		public XPlaneRectangle(Point3D point, double width, double height)
			: base(point, width, height) {
		}
		protected override void Init(Point3D point, double width, double height) {
			Point3D location = new Point3D();
			location.X = point.X;
			location.Y = height < 0 ? point.Y + height : point.Y;
			location.Z = width < 0 ? point.Z + width : point.Z;
			width = Math.Abs(width);
			height = Math.Abs(height);
			List<Vertex> vertices = new List<Vertex>();
			vertices.Add(new Vertex(location, new Point(1, 1)));
			vertices.Add(new Vertex(MathUtils.Offset(location, 0, height, 0), new Point(1, 0)));
			vertices.Add(new Vertex(MathUtils.Offset(location, 0, height, width), new Point(0, 0)));
			vertices.Add(new Vertex(MathUtils.Offset(location, 0, 0, width), new Point(0, 1)));
			SetVertices(vertices);
		}
		protected override PlanePolygon CreateInstance() {
			return new XPlaneRectangle();
		}
	}
	public class YPlaneRectangle : PlaneRectangle {
		protected YPlaneRectangle()
			: base() {
		}
		public YPlaneRectangle(Point3D point, double width, double height)
			: base(point, width, height) {
		}
		protected override void Init(Point3D point, double width, double height) {
			Point3D location = new Point3D();
			location.X = width < 0 ? point.X + width : point.X;
			location.Y = point.Y;
			location.Z = height < 0 ? point.Z + height : point.Z;
			width = Math.Abs(width);
			height = Math.Abs(height);
			List<Vertex> vertices = new List<Vertex>();
			vertices.Add(new Vertex(location, new Point(0, 0)));
			vertices.Add(new Vertex(MathUtils.Offset(location, 0, 0, height), new Point(0, 1)));
			vertices.Add(new Vertex(MathUtils.Offset(location, width, 0, height), new Point(1, 1)));
			vertices.Add(new Vertex(MathUtils.Offset(location, width, 0, 0), new Point(1, 0)));
			SetVertices(vertices);
		}
		protected override PlanePolygon CreateInstance() {
			return new YPlaneRectangle();
		}
	}
	public class ZPlaneRectangle : PlaneRectangle {
		protected ZPlaneRectangle()
			: base() {
		}
		public ZPlaneRectangle(Point3D point, double width, double height)
			: base(point, width, height) {
		}
		protected override void Init(Point3D point, double width, double height) {
			Point3D location = new Point3D();
			location.X = width < 0 ? point.X + width : point.X;
			location.Y = height < 0 ? point.Y + height : point.Y;
			location.Z = point.Z;
			width = Math.Abs(width);
			height = Math.Abs(height);
			List<Vertex> vertices = new List<Vertex>();
			vertices.Add(new Vertex(location, new Point(0, 1)));
			vertices.Add(new Vertex(MathUtils.Offset(location, width, 0, 0), new Point(1, 1)));
			vertices.Add(new Vertex(MathUtils.Offset(location, width, height, 0), new Point(1, 0)));
			vertices.Add(new Vertex(MathUtils.Offset(location, 0, height, 0), new Point(0, 0)));
			SetVertices(vertices);
		}
		protected override PlanePolygon CreateInstance() {
			return new ZPlaneRectangle();
		}
	}
}
