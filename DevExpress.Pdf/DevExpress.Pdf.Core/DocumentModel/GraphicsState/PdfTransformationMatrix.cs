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
namespace DevExpress.Pdf {
	public class PdfTransformationMatrix {
		public static PdfTransformationMatrix Translate(PdfTransformationMatrix matrix, double translateX, double translateY) {
			return new PdfTransformationMatrix(matrix.a, matrix.b, matrix.c, matrix.d, matrix.e + translateX, matrix.f + translateY);
		}
		public static PdfTransformationMatrix Scale(PdfTransformationMatrix matrix, double scaleX, double scaleY) {
			return new PdfTransformationMatrix(matrix.a * scaleX, matrix.b * scaleY, matrix.c * scaleX, matrix.d * scaleY, matrix.e * scaleX, matrix.f * scaleY);
		}
		public static PdfTransformationMatrix Multiply(PdfTransformationMatrix matrix1, PdfTransformationMatrix matrix2) {
			double matrix1A = matrix1.a;
			double matrix1B = matrix1.b;
			double matrix1C = matrix1.c;
			double matrix1D = matrix1.d;
			double matrix1E = matrix1.e;
			double matrix1F = matrix1.f;
			double matrix2A = matrix2.a;
			double matrix2B = matrix2.b;
			double matrix2C = matrix2.c;
			double matrix2D = matrix2.d;
			return new PdfTransformationMatrix(matrix1A * matrix2A + matrix1B * matrix2C, matrix1A * matrix2B + matrix1B * matrix2D, matrix1C * matrix2A + matrix1D * matrix2C,
				matrix1C * matrix2B + matrix1D * matrix2D, matrix1E * matrix2A + matrix1F * matrix2C + matrix2.e, matrix1E * matrix2B + matrix1F * matrix2D + matrix2.f);
		}
		internal static PdfTransformationMatrix Rotate(PdfTransformationMatrix matrix, double degree) {
			double radians = degree / (180 / Math.PI);
			double sin = Math.Sin(radians);
			double cos = Math.Cos(radians);
			return Multiply(matrix, new PdfTransformationMatrix(cos, sin, -sin, cos, 0, 0));
		}
		internal static PdfTransformationMatrix Invert(PdfTransformationMatrix matrix) {
			double determinant = matrix.a * matrix.d - matrix.b * matrix.c;
			return new PdfTransformationMatrix(matrix.d / determinant, -matrix.b / determinant, -matrix.c / determinant, matrix.a / determinant, 
				(matrix.c * matrix.f - matrix.e * matrix.d) / determinant, (matrix.b * matrix.e - matrix.f * matrix.a) / determinant);
		}
		readonly double a;
		readonly double b;
		readonly double c;
		readonly double d;
		readonly double e;
		readonly double f;
		public double A { get { return a; } }
		public double B { get { return b; } }
		public double C { get { return c; } }
		public double D { get { return d; } }
		public double E { get { return e; } }
		public double F { get { return f; } }
		internal bool IsDefault { get { return a == 1 && b == 0 && c == 0 && d == 1 && e == 0 && f == 0; } }
		internal IList<object> Data { get { return new List<object> { a, b, c, d, e, f }; } }
		internal PdfTransformationMatrix(IList<object> array) {
			if (array == null) {
				a = 1;
				b = 0;
				c = 0;
				d = 1;
				e = 0;
				f = 0;
			}
			else {
				if (array.Count != 6)
					PdfDocumentReader.ThrowIncorrectDataException();
				a = PdfDocumentReader.ConvertToDouble(array[0]);
				b = PdfDocumentReader.ConvertToDouble(array[1]);
				c = PdfDocumentReader.ConvertToDouble(array[2]);
				d = PdfDocumentReader.ConvertToDouble(array[3]);
				e = PdfDocumentReader.ConvertToDouble(array[4]);
				f = PdfDocumentReader.ConvertToDouble(array[5]);
			}
		}
		internal PdfTransformationMatrix(PdfOperands operands) {
			a = operands.PopDouble();
			b = operands.PopDouble();
			c = operands.PopDouble();
			d = operands.PopDouble();
			e = operands.PopDouble();
			f = operands.PopDouble();
		}
		public PdfTransformationMatrix(double a, double b, double c, double d, double e, double f) {
			this.a = a;
			this.b = b;
			this.c = c;
			this.d = d;
			this.e = e;
			this.f = f;
		}
		public PdfTransformationMatrix()
			: this(1, 0, 0, 1, 0, 0) {
		}
		public PdfPoint Transform(PdfPoint point) {
			double x = point.X;
			double y = point.Y;
			return new PdfPoint(x * a + y * c + e, x * b + y * d + f);
		}
		public PdfTransformationMatrix Clone() {
			return new PdfTransformationMatrix(a, b, c, d, e, f);
		}
		internal bool Equals(PdfTransformationMatrix matrix) {
			return a == matrix.a && b == matrix.b && c == matrix.c && d == matrix.d && e == matrix.e && f == matrix.f;
		}
		internal void TransformPoints(PdfPoint[] points) {
			for (int i =0 ; i < points.Length; i++)
				points[i] = Transform(points[i]);
		}
		internal void Write(PdfDocumentWritableStream writer) {
			writer.WriteSpace();
			writer.WriteDouble(a);
			writer.WriteSpace();
			writer.WriteDouble(b);
			writer.WriteSpace();
			writer.WriteDouble(c);
			writer.WriteSpace();
			writer.WriteDouble(d);
			writer.WriteSpace();
			writer.WriteDouble(e);
			writer.WriteSpace();
			writer.WriteDouble(f);
		}
	}
}
