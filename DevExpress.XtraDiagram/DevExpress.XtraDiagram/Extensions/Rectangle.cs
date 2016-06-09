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
using PlatformRect = System.Windows.Rect;
namespace DevExpress.XtraDiagram.Extensions {
	public static class RectangleExtensions {
		public static Rectangle Inflated(this Rectangle rect, int size) {
			return Rectangle.Inflate(rect, size, size);
		}
		public static Rectangle Deflated(this Rectangle rect, int size) {
			return Rectangle.Inflate(rect, -size, -size);
		}
		public static PlatformRect ToPlatformRect(this Rectangle rect) {
			return new PlatformRect(rect.X, rect.Y, rect.Width, rect.Height);
		}
		public static Point GetTopLeftPt(this Rectangle rect) {
			return new Point(rect.X, rect.Y);
		}
		public static Point GetTopPt(this Rectangle rect) {
			return new Point(rect.X + rect.Width / 2, rect.Y);
		}
		public static Point GetTopRightPt(this Rectangle rect) {
			return new Point(rect.Right, rect.Y);
		}
		public static Point GetLeftPt(this Rectangle rect) {
			return new Point(rect.X, rect.Y + rect.Height / 2);
		}
		public static Point GetRightPt(this Rectangle rect) {
			return new Point(rect.Right, rect.Y + rect.Height / 2);
		}
		public static Point GetBottomLeftPt(this Rectangle rect) {
			return new Point(rect.X, rect.Bottom);
		}
		public static Point GetBottomPt(this Rectangle rect) {
			return new Point(rect.X + rect.Width / 2, rect.Bottom);
		}
		public static Point GetBottomRightPt(this Rectangle rect) {
			return new Point(rect.Right, rect.Bottom);
		}
		public static Point[] GetPoints(this Rectangle rect) {
			return new Point[] { rect.Location, new Point(rect.Right, rect.Bottom) };
		}
		public static Point GetCenterPoint(this Rectangle rect) {
			return new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
		}
	}
	public static class RectangleFloatExtensions {
		public static Rectangle ToRectangle(this RectangleF rect) {
			return new Rectangle((int)(rect.X - 0.5), (int)(rect.Y - 0.5), (int)(rect.Width + 0.5), (int)(rect.Height + 0.5));
		}
	}
	public static class PlatformRectangleExtensions {
		public static Rectangle ToWinRect(this PlatformRect rect) {
			return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}
		public static bool IsZeroRect(this PlatformRect rect) {
			return rect == ZeroRect;
		}
		public static bool IsNonZeroRect(this PlatformRect rect) {
			return rect != ZeroRect;
		}
		static readonly PlatformRect ZeroRect = new PlatformRect();
	}
}
