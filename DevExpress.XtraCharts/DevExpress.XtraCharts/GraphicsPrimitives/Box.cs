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
namespace DevExpress.XtraCharts.Native {
	public class Box {
		public static bool Contains(Box box, PlaneRectangle rect) {
			return rect != null && box != null && box.Contains(rect);
		}
		ZPlaneRectangle back, fore;
		XPlaneRectangle left, right;
		YPlaneRectangle bottom, top;
		public ZPlaneRectangle Back { get { return this.back; } }
		public ZPlaneRectangle Fore { get { return this.fore; } }
		public XPlaneRectangle Left { get { return this.left; } }
		public XPlaneRectangle Right { get { return this.right; } }
		public YPlaneRectangle Bottom { get { return this.bottom; } }
		public YPlaneRectangle Top { get { return this.top; } }
		public DiagramPoint Location { get { return back.Location; } }
		public double Width { get { return top.Height; } }
		public double Height { 
			get { return fore.Height; } 
			set {
				top.Offset(0.0, value - Height, 0.0);
				back.Height = value;
				fore.Height = value;
				left.Width = value;
				right.Width = value;
			}
		}
		public double Depth { get { return top.Width; } }
		public Box(DiagramPoint point, double width, double height) : this(point, width, height, width) {
		}
		public Box(DiagramPoint point, double width, double height, double depth) {
			if (width < 0) {
				point.X += width;
				width = Math.Abs(width);
			}
			if (height < 0) {
				point.Y += height;
				height = Math.Abs(height);
			}
			if (depth < 0) {
				point.Z += depth;
				depth = Math.Abs(depth);
			}
			bottom = new YPlaneRectangle(point, depth, width, this);
			top = new YPlaneRectangle(DiagramPoint.Offset(point, 0, height, 0), depth, width, this);
			left = new XPlaneRectangle(point, height, depth, this);
			right = new XPlaneRectangle(DiagramPoint.Offset(point, width, 0, 0), height, depth, this);
			back = new ZPlaneRectangle(point, width, height, this);
			fore = new ZPlaneRectangle(DiagramPoint.Offset(point, 0, 0, depth), width, height, this);
		}
		public Box(XPlaneRectangle rect, double depth) : this(rect.Location, depth, rect.Width, rect.Height) {
		}
		public Box(YPlaneRectangle rect, double height) : this(rect.Location, rect.Height, height, rect.Width) {
		}
		public Box(ZPlaneRectangle rect, double depth) : this(rect.Location, rect.Width, rect.Height, depth) {
		}
		public PlaneRectangle[] GetPlanes() {
			return new PlaneRectangle[] { bottom, top, left, right, back, fore };
		}
		public PlanePolygon[] GetLaterals() { 
			return new PlanePolygon[] { 
				left, right, back.CalcLeftHalf(), back.CalcRightHalf(), fore.CalcLeftHalf(), fore.CalcRightHalf() }; 
		}
		public BoxPlane GetBoxPlane(PlaneRectangle rect) {
			if (rect == null)
				return 0;
			if (rect == back)
				return BoxPlane.Back;
			if (rect == fore)
				return BoxPlane.Fore;
			if (rect == left)
				return BoxPlane.Left;
			if (rect == right)
				return BoxPlane.Right;
			if (rect == bottom)
				return BoxPlane.Bottom;
			if (rect == top)
				return BoxPlane.Top;
			return 0;
		}
		public bool Contains(PlaneRectangle rect) {
			if (rect == null)
				return false;
			int rectHash = rect.GetHashCode();
			return rectHash == bottom.GetHashCode() || rectHash == top.GetHashCode() ||
				rectHash == back.GetHashCode() || rectHash == fore.GetHashCode() ||
				rectHash == left.GetHashCode() || rectHash == right.GetHashCode();
		}
		public void Offset(double dx, double dy, double dz) {
			this.back.Offset(dx, dy, dz);
			this.fore.Offset(dx, dy, dz);
			this.left.Offset(dx, dy, dz);
			this.right.Offset(dx, dy, dz);
			this.bottom.Offset(dx, dy, dz);
			this.top.Offset(dx, dy, dz);
		}
		public List<PlaneQuadrangle> FilterPlanes(Box box) {
			List<PlaneQuadrangle> planes = new List<PlaneQuadrangle>();
			IList<PlaneRectangle> selfPlanes = GetPlanes();
			foreach (PlaneRectangle plane in box.GetPlanes())
				foreach (PlaneRectangle selfPlane in selfPlanes)
					if (MathUtils.IsOnePlanePrimitives(plane, selfPlane)) {
						planes.AddRange(plane.SeparateByCenter());
						break;
					}
			return planes;
		}
		public PlanePolygon[] GetRepresentation() {
			List<PlanePolygon> representation = new List<PlanePolygon>();
			representation.Add(top.CalcTopHalf());
			representation.Add(top.CalcBottomHalf());
			representation.Add(bottom.CalcTopHalf());
			representation.Add(bottom.CalcBottomHalf());
			representation.AddRange(fore.SeparateByCenter());
			representation.AddRange(back.SeparateByCenter());
			representation.Add(left.CalcLeftHalf());
			representation.Add(left.CalcRightHalf());
			representation.Add(right.CalcLeftHalf());
			representation.Add(right.CalcRightHalf());
			return representation.ToArray();
		}
	}
	public enum BoxPlane {
		Back		= 0x1,
		Fore		= 0x2,	
		Bottom		= 0x4,
		Top			= 0x8,
		Left		= 0x10,
		Right		= 0x20
	}
}
