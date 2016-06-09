#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfRectangle {
		static bool CheckNumbers(double a, double b, double eps) {
			return Math.Abs(a - b) <= eps;
		}
		internal static bool AreEqual(PdfRectangle r1, PdfRectangle r2, double eps) {
			if (Object.ReferenceEquals(r1, null))
				return Object.ReferenceEquals(r2, null);
			if (Object.ReferenceEquals(r2, null))
				return false;
			return CheckNumbers(r1.left, r2.left, eps) && CheckNumbers(r1.bottom, r2.bottom, eps) && CheckNumbers(r1.right, r2.right, eps) && CheckNumbers(r1.top, r2.top, eps);
		}
		internal static PdfRectangle Intersect(PdfRectangle r1, PdfRectangle r2) {
			if (r1.Intersects(r2))
				return new PdfRectangle(Math.Max(r1.Left, r2.Left), Math.Max(r1.Bottom, r2.Bottom), Math.Min(r1.Right, r2.Right), Math.Min(r1.Top, r2.Top));
			return null;
		}
		internal static PdfRectangle InflateToNonEmpty(PdfRectangle r) {
			if (r.Width > 0 && r.Height > 0)
				return r;
			double left = r.Left;
			double bottom = r.Bottom;
			return new PdfRectangle(left, bottom, left + 1, bottom + 1);
		}
		readonly double left;
		readonly double bottom;
		readonly double right;
		readonly double top;
		public double Left { get { return left; } }
		public double Bottom { get { return bottom; } }
		public double Right { get { return right; } }
		public double Top { get { return top; } }
		public double Width { get { return right - left; } }
		public double Height { get { return top - bottom; } }
		public PdfPoint BottomLeft { get { return new PdfPoint(left, bottom); } }
		public PdfPoint TopLeft { get { return new PdfPoint(left, top); } }
		public PdfPoint BottomRight { get { return new PdfPoint(right, bottom); } }
		public PdfPoint TopRight { get { return new PdfPoint(right, top); } }
		public PdfRectangle(double left, double bottom, double right, double top) {
			if (left > right)
				throw new ArgumentOutOfRangeException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectRectangleWidth));
			if (bottom > top)
				throw new ArgumentOutOfRangeException(PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectRectangleHeight));
			this.left = left;
			this.bottom = bottom;
			this.right = right;
			this.top = top;
		}
		internal PdfRectangle(PdfPoint point1, PdfPoint point2) {
			double x1 = point1.X;
			double x2 = point2.X;
			if (x1 < x2) {
				left = x1;
				right = x2;
			}
			else {
				left = x2;
				right = x1;
			}
			double y1 = point1.Y;
			double y2 = point2.Y;
			if (y1 < y2) {
				bottom = y1;
				top = y2;
			}
			else {
				bottom = y2;
				top = y1;
			}
		}
		internal PdfRectangle(IList<object> array) {
			if (array.Count != 4)
				PdfDocumentReader.ThrowIncorrectDataException();
			left = PdfDocumentReader.ConvertToDouble(array[0]);
			bottom = PdfDocumentReader.ConvertToDouble(array[1]);
			right = PdfDocumentReader.ConvertToDouble(array[2]);
			top = PdfDocumentReader.ConvertToDouble(array[3]);
			if (right < left) {
				double temp = right;
				right = left;
				left = temp;
			}
			if (top < bottom) {
				double temp = bottom;
				bottom = top;
				top = temp;
			}
		}
		internal bool Contains(PdfRectangle rectangle) {
			return left <= rectangle.left && right >= rectangle.right && bottom <= rectangle.bottom && top >= rectangle.top;
		}
		internal bool Contains(PdfPoint point) {
			return left <= point.X && right >= point.X && top >= point.Y && bottom <= point.Y;
		}
		internal bool Intersects(PdfRectangle rectangle) {
			return left <= rectangle.Right && right >= rectangle.Left && top >= rectangle.Bottom && bottom <= rectangle.Top;
		}
		internal PdfRectangle Trim(PdfRectangle rectangle) {
			return new PdfRectangle(Math.Max(left, rectangle.left), Math.Max(bottom, rectangle.bottom), Math.Min(right, rectangle.right), Math.Min(top, rectangle.top));
		}
		public override bool Equals(object obj) {
			PdfRectangle rectangle = obj as PdfRectangle;
			return rectangle != null && left == rectangle.left && right == rectangle.right && top == rectangle.top && bottom == rectangle.bottom;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		protected internal PdfWritableDoubleArray ToWritableObject() {
			return new PdfWritableDoubleArray(left, bottom, right, top);
		}
	}
}
