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
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.Layout.Native {
	public static class BoundsConverter {
		public static double ToDouble(float size, double fromDpi) {
			return size * XtraPrinting.GraphicsDpi.Pixel / fromDpi;
		}
		public static float ToFloat(double size, double toDpi) {
			return (float)(size * toDpi/ XtraPrinting.GraphicsDpi.Pixel);
		}
		public static Rect ToRect(System.Drawing.RectangleF rectangleF, double fromDpi) {
			return new Rect(ToPoint(rectangleF.Location, fromDpi), ToSize(rectangleF.Size, fromDpi));
		}
		public static System.Drawing.RectangleF ToRectangleF(this Rect rect, double toDpi) {
			return new System.Drawing.RectangleF(ToPointF(rect.Location, toDpi), ToSizeF(rect.Size, toDpi));
		}
		public static Point ToPoint(System.Drawing.PointF pointF, double fromDpi) {
			return new Point(ToDouble(pointF.X, fromDpi), ToDouble(pointF.Y, fromDpi));
		}
		public static System.Drawing.PointF ToPointF(Point point, double toDpi) {
			return new System.Drawing.PointF(ToFloat(point.X, toDpi), ToFloat(point.Y, toDpi));
		}
		public static Size ToSize(System.Drawing.SizeF sizeF, double fromDpi) {
			return new Size(ToDouble(sizeF.Width, fromDpi), ToDouble(sizeF.Height, fromDpi));
		}
		public static System.Drawing.SizeF ToSizeF(Size size, double toDpi) {
			return new System.Drawing.SizeF(ToFloat(size.Width, toDpi), ToFloat(size.Height, toDpi));
		}
	}
}
