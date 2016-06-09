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
	public enum PdfPredefinedSpotFunctionKind { SimpleDot, InvertedSimpleDot, DoubleDot, InvertedDoubleDot, CosineDot, Double, InvertedDouble, Line, LineX, LineY, 
												Round, Ellipse, EllipseA, InvertedEllipseA, EllipseB, EllipseC, InvertedEllipseC, Square, Cross, Rhomboid, Diamond }
	public class PdfPredefinedSpotFunction : PdfFunction {
		const double doublePi = Math.PI * 2;
		readonly PdfPredefinedSpotFunctionKind kind;
		public PdfPredefinedSpotFunctionKind Kind { get { return kind; } }
		public PdfPredefinedSpotFunction(PdfPredefinedSpotFunctionKind kind) {
			this.kind = kind;
		}
		internal PdfPredefinedSpotFunction(string name) {
			kind = PdfEnumToStringConverter.Parse<PdfPredefinedSpotFunctionKind>(name, false);
		}
		protected internal override double[] Transform(double[] arguments) {
			double result = 0.0;
			if (arguments.Length >= 2) {
				double x = arguments[0];
				double y = arguments[1];
				switch (kind) {
					case PdfPredefinedSpotFunctionKind.SimpleDot:
						result = 1 - (x * x + y * y);
						break;
					case PdfPredefinedSpotFunctionKind.InvertedSimpleDot:
						result = x * x + y * y - 1;
						break;
					case PdfPredefinedSpotFunctionKind.DoubleDot:
						result = (Math.Sin(doublePi * x) + Math.Sin(doublePi * y)) / 2;
						break;
					case PdfPredefinedSpotFunctionKind.InvertedDoubleDot:
						result = -(Math.Sin(doublePi * x) + Math.Sin(doublePi * y)) / 2;
						break;
					case PdfPredefinedSpotFunctionKind.CosineDot:
						result = (Math.Cos(Math.PI * x) + Math.Cos(Math.PI * y)) / 2;
						break;
					case PdfPredefinedSpotFunctionKind.Double:
						result = (Math.Sin(Math.PI * x) + Math.Sin(doublePi * y)) / 2;
						break;
					case PdfPredefinedSpotFunctionKind.InvertedDouble:
						result = -(Math.Sin(Math.PI * x) + Math.Sin(doublePi * y)) / 2;
						break;
					case PdfPredefinedSpotFunctionKind.Line:
						result = -Math.Abs(y);
						break;
					case PdfPredefinedSpotFunctionKind.LineX:
						result = x;
						break;
					case PdfPredefinedSpotFunctionKind.LineY:
						result = y;
						break;
					case PdfPredefinedSpotFunctionKind.Round:
						x = Math.Abs(x);
						y = Math.Abs(y);
						if (x + y > 1) {
							x -= 1;
							y -= 1;
							result = x * x + y * y - 1;
						}
						else
							result = 1 - (x * x + y * y);
						break;
					case PdfPredefinedSpotFunctionKind.Ellipse:
						x = Math.Abs(x);
						y = Math.Abs(y);
						result = 3 * x + 4 * y - 3;
						if (result < 0) {
							y /= 0.75;
							result = 1 - (x * x + y * y) / 4;
						}
						else if (result > 1)
							result = ((1 - x * x) + (1 - y * y) / 0.5625) / 4 - 1;
						else
							result = 0.5 - result;
						break;
					case PdfPredefinedSpotFunctionKind.EllipseA:
						result = 1 - (x * x + 0.9 * y * y);
						break;
					case PdfPredefinedSpotFunctionKind.InvertedEllipseA:
						result = x * x + 0.9 * y * y - 1;
						break;
					case PdfPredefinedSpotFunctionKind.EllipseB:
						result = 1 - Math.Sqrt(x * x + 0.625 * y * y);
						break;
					case PdfPredefinedSpotFunctionKind.EllipseC:
						result = 1 - (0.9 * x * x + y * y);
						break;
					case PdfPredefinedSpotFunctionKind.InvertedEllipseC:
						result = 0.9 * x * x + y * y - 1;
						break;
					case PdfPredefinedSpotFunctionKind.Square:
						result = -Math.Max(Math.Abs(x), Math.Abs(y));
						break;
					case PdfPredefinedSpotFunctionKind.Cross:
						result = -Math.Min(Math.Abs(x), Math.Abs(y));
						break;
					case PdfPredefinedSpotFunctionKind.Rhomboid:
						result = (0.9 * Math.Abs(x) + Math.Abs(y)) / 2;
						break;
					case PdfPredefinedSpotFunctionKind.Diamond:
						x = Math.Abs(x);
						y = Math.Abs(y);
						result = x + y;
						if (result <= 0.75) 
							result = 1 - (x * x + y * y);
						else if (result <= 1.23)
							result = 1 - (0.85 * x + y);
						else {
							x -= 1;
							y -= 1;
							result = x * x + y * y - 1;
						}
						break;
				}
			}
			return new double[] { result };
		}
		protected internal override bool IsSame(PdfFunction function) {
			PdfPredefinedSpotFunction spotFunction = function as PdfPredefinedSpotFunction;
			return spotFunction != null && kind == spotFunction.kind;
		}
		protected internal override object Write(PdfObjectCollection objects) {
			return new PdfName(PdfEnumToStringConverter.Convert(kind));
		}
		protected internal override object ToWritableObject(PdfObjectCollection objects) {
			return Write(objects);
		}
	}
}
