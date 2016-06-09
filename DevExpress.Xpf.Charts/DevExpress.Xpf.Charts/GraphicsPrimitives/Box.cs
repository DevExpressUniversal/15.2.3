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
using System.Windows.Media.Media3D;
namespace DevExpress.Xpf.Charts.Native {
	public class Box {
		Point3D location;
		double width, height, depth;
		XPlaneRectangle left, right;
		YPlaneRectangle bottom, top;
		ZPlaneRectangle back, fore;
		public double Width { get { return width; } }
		public double Height { get { return height; } }
		public double Depth { get { return depth; } }
		public Rect3D Bounds { get { return new Rect3D(location, new Size3D(width, height, depth)); } }
		public XPlaneRectangle Left { get { return left; } }
		public XPlaneRectangle Right { get { return right; } }
		public YPlaneRectangle Bottom { get { return bottom; } }
		public YPlaneRectangle Top { get { return top; } }
		public ZPlaneRectangle Back { get { return back; } }
		public ZPlaneRectangle Fore { get { return fore; } }
		public Box(Point3D point, double width, double height, double depth) {
			location.X = width < 0 ? point.X + width : point.X;
			location.Y = height < 0 ? point.Y + height : point.Y;
			location.Z = depth < 0 ? point.Z + depth : point.Z;
			this.width = Math.Abs(width);
			this.height = Math.Abs(height);
			this.depth = Math.Abs(depth);
			InitPlanes();
		}
		void InitPlanes() {
			left = new XPlaneRectangle(location, depth, height);
			right = (XPlaneRectangle)PlanePolygon.Offset(left, width, 0, 0);
			left.InvertVerticesDirection();
			bottom = new YPlaneRectangle(location, width, depth);
			top = (YPlaneRectangle)PlanePolygon.Offset(bottom, 0, height, 0);
			bottom.InvertVerticesDirection();
			back = new ZPlaneRectangle(location, width, height);
			fore = (ZPlaneRectangle)PlanePolygon.Offset(back, 0, 0, depth);
			back.InvertVerticesDirection();
		}
		public MeshGeometry3D GetGeometry() {
			MeshGeometry3D geometry = new MeshGeometry3D();
			Graphics3DUtils.AddPolygon(geometry, left.Vertices);
			Graphics3DUtils.AddPolygon(geometry, right.Vertices);
			Graphics3DUtils.AddPolygon(geometry, bottom.Vertices);
			Graphics3DUtils.AddPolygon(geometry, top.Vertices);
			Graphics3DUtils.AddPolygon(geometry, back.Vertices);
			Graphics3DUtils.AddPolygon(geometry, fore.Vertices);
			return geometry;
		}
		public Model3D GetModel() {
			Model3DGroup modelGroup = new Model3DGroup();
			modelGroup.Children.Add(back.GetModel());
			modelGroup.Children.Add(fore.GetModel());
			modelGroup.Children.Add(left.GetModel());
			modelGroup.Children.Add(right.GetModel());
			modelGroup.Children.Add(bottom.GetModel());
			modelGroup.Children.Add(top.GetModel());
			return modelGroup;
		}
		public void SetMaterial(DiffuseMaterial material) {
			left.Material = material;
			right.Material = material;
			bottom.Material = material;
			top.Material = material;
			back.Material = material;
			fore.Material = material;
		}
	}
	[Flags()]
	public enum BoxPlane {
		Back	= 0x1,
		Fore	= 0x2,
		Bottom  = 0x4,
		Top	 = 0x8,
		Left	= 0x10,
		Right   = 0x20
	}
}
