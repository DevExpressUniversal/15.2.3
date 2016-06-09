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
using System.Linq;
using System.Text;
using DevExpress.XtraDiagram.Extensions;
namespace DevExpress.XtraDiagram.Utils {
	public class RectangleUtils {
		public static Rectangle FromPoints(Point[] points) {
			if(points.Length != 2) throw new ArgumentException("points");
			Point loc = points[0];
			int width = Math.Max(0, points[1].X - points[0].X);
			int height = Math.Max(0, points[1].Y - points[0].Y);
			return new Rectangle(loc.X, loc.Y, width, height);
		}
		public static Rectangle ApplyOffset(Rectangle rect, Point pt) {
			return ApplyOffset(rect, pt.X, pt.Y);
		}
		public static Rectangle ApplyOffset(Rectangle rect, int x, int y) {
			Rectangle newRect = rect;
			rect.Offset(x, y);
			return rect;
		}
		public static Rectangle FitRect(Rectangle ownerRect, double width, double height) {
			return FitRect(ownerRect, (int)width, (int)height);
		}
		public static Rectangle FitRect(Rectangle ownerRect, Size size) {
			return FitRect(ownerRect, size.Width, size.Height);
		}
		public static Rectangle FitRect(Rectangle ownerRect, int width, int height) {
			Point point = ownerRect.GetCenterPoint();
			return new Size(width, height).CreateRect(point);
		}
		public static Rectangle EnsureContains(Rectangle r, Point point) {
			Rectangle rect = r;
			if(point.X < rect.Left) {
				int dx = rect.Left - point.X;
				rect.X -= dx;
				rect.Width += dx;
			}
			if(point.X > rect.Right) {
				rect.Width += point.X - rect.Right;
			}
			if(point.Y < rect.Top) {
				int dy = rect.Top - point.Y;
				rect.Y -= dy;
				rect.Height += dy;
			}
			if(point.Y > rect.Bottom) {
				rect.Height += point.Y - rect.Bottom;
			}
			return rect;
		}
		public static Rectangle CalcClipRect(Rectangle rect, Rectangle boundingRect) {
			Rectangle clipRect = new Rectangle(Point.Empty, rect.Size);
			if(rect.X < boundingRect.X) {
				clipRect.X = boundingRect.X - rect.X;
			}
			if(rect.Y < boundingRect.Y) {
				clipRect.Y = boundingRect.Y - rect.Y;
			}
			if(rect.Right > boundingRect.Right) {
				clipRect.Width -= (rect.Right - boundingRect.Right);
			}
			if(rect.Bottom > boundingRect.Bottom) {
				clipRect.Height -= (rect.Bottom - boundingRect.Bottom);
			}
			return clipRect;
		}
		public static Rectangle FromLTRB(float left, float top, float right, float bottom) {
			return Rectangle.FromLTRB(ToInt(left), ToInt(top), ToInt(right), ToInt(bottom));
		}
		static int ToInt(double val) { return (int)val; }
	}
}
