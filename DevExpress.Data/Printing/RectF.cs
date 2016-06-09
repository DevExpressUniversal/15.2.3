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
using System.Text;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraPrinting.Native {
	public class RectFBase {
		static public RectangleF FromPoints(PointF pt1, PointF pt2) {
			float left = Math.Min(pt1.X, pt2.X);
			float right = Math.Max(pt1.X, pt2.X);
			float top = Math.Min(pt1.Y, pt2.Y);
			float bottom = Math.Max(pt1.Y, pt2.Y);
			return RectangleF.FromLTRB(left, top, right, bottom);
		}
		static public RectangleF Offset(RectangleF val, float dx, float dy) {
			val.Offset(dx, dy);
			return val;
		}
		static public RectangleF Center(RectangleF rect, RectangleF baseRect) {
			rect.Offset(baseRect.Left - rect.Left, baseRect.Top - rect.Top);
			float dx = (baseRect.Width - rect.Width) / 2;
			float dy = (baseRect.Height - rect.Height) / 2;
			rect.Offset(dx, dy);
			return rect;
		}
		static public Rectangle Round(RectangleF value) {
			return Rectangle.FromLTRB((int)Math.Round(value.Left), (int)Math.Round(value.Top), (int)Math.Round(value.Right), (int)Math.Round(value.Bottom));
		}
		static public bool IntersectByX(RectangleF rect1, RectangleF rect2) {
			return rect1.X < rect2.Right && rect2.X < rect1.Right;
		}
		static public bool IntersectByY(RectangleF rect1, RectangleF rect2) {
			return (float)rect1.Y < (float)rect2.Bottom && (float)rect2.Y < (float)rect1.Bottom;
		}
		static public bool ContainsByX(RectangleF rect1, RectangleF rect2) {
			return rect1.X <= rect2.X && rect2.Right <= rect1.Right;
		}
		static public bool ContainsByY(RectangleF rect1, RectangleF rect2) {
			return rect1.Y <= rect2.Y && rect2.Bottom <= rect1.Bottom;
		}
		static public bool IntersectAbove(RectangleF baseRect, RectangleF rect) {
			return baseRect.Top > rect.Top && baseRect.Top < rect.Bottom;
		}
		static public bool IntersectBelow(RectangleF baseRect, RectangleF rect) {
			return baseRect.Bottom > rect.Top && baseRect.Bottom < rect.Bottom;
		}
	}
	public class RectF : RectFBase {
		public static bool Contains(RectangleF baseRect, RectangleF rect) {
			if ((baseRect.X <= rect.X) && ((float)(rect.X + rect.Width) <= (float)(baseRect.X + baseRect.Width)) && (baseRect.Y <= rect.Y)) {
				return (float)(rect.Y + rect.Height) <= (float)(baseRect.Y + baseRect.Height);
			}
			return false;
		}
		public static RectangleF DeflateRect(RectangleF rect, MarginsF margins) {
			return RectangleF.FromLTRB(rect.Left + margins.Left, rect.Top + margins.Top, rect.Right - margins.Right, rect.Bottom - margins.Bottom);
		}
		static public RectangleF Align(RectangleF rect, RectangleF baseRect, BrickAlignment alignment, BrickAlignment lineAlignment) {
			RectangleF r = AlignHorz(rect, baseRect, alignment);
			return AlignVert(r, baseRect, lineAlignment);
		}
		static public RectangleF AlignHorz(RectangleF rect, RectangleF baseRect, BrickAlignment alignment) {
			switch (alignment) {
				case BrickAlignment.Near:
					rect.X = baseRect.X;
					break;
				case BrickAlignment.Center: {
						RectangleF r = rect;
						rect = Center(rect, baseRect);
						rect.Y = r.Y;
						break;
					}
				case BrickAlignment.Far:
					rect.X = baseRect.Right - rect.Width;
					break;
			}
			return rect;
		}
		static public RectangleF AlignVert(RectangleF rect, RectangleF baseRect, BrickAlignment alignment) {
			switch (alignment) {
				case BrickAlignment.Near:
					rect.Y = baseRect.Y;
					break;
				case BrickAlignment.Center: {
						RectangleF r = rect;
						rect = Center(rect, baseRect);
						rect.X = r.X;
						break;
					}
				case BrickAlignment.Far:
					rect.Y = baseRect.Bottom - rect.Height;
					break;
			}
			return rect;
		}
	}
	public struct PointD {
		public double X;
		public double Y;
		public PointD(double x, double y) {
			this.X = x;
			this.Y = y;
		}
		public PointF ToPointF() {
			return new PointF((float)X, (float)Y);
		}
		public override string  ToString() {
 			return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{X={0}, Y={1}}}", new object[] { this.X, this.Y });
		}
	}
}
