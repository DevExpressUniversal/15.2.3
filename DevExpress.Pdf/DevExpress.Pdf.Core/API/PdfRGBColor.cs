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
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public struct PdfRGBColor {
		static readonly PdfRGBColor blackColor = Create(0, 0, 0);
		internal static byte[] FromCMYKBytes(byte cyan, byte magenta, byte yellow, byte black) {
			if (black == 255)
				return new byte[3];
			double cyanComplement = 255 - cyan;
			double magentaComplement = 255 - magenta;
			double yellowComplement = 255 - yellow;
			double blackComplement = 255 - black;
			double blackDiv = black / blackComplement;
			double addition = cyanComplement * magentaComplement * yellowComplement * blackComplement;
			double red = addition;
			double green = addition;
			double blue = addition;
			addition *= blackDiv;
			red += 0.1373 * addition;
			green += 0.1216 * addition;
			blue += 0.1255 * addition;
			addition = cyanComplement * magentaComplement * yellow * blackComplement;
			red += addition;
			green += 0.9490 * addition;
			addition *= blackDiv;
			red += 0.1098 * addition;
			green += 0.1020 * addition;
			addition = cyanComplement * magenta * yellowComplement * blackComplement;
			red += 0.9255 * addition;
			blue += 0.5490 * addition;
			red += 0.1412 * (addition * blackDiv);
			addition = cyanComplement * magenta * yellow * blackComplement;
			red += 0.9294 * addition;
			green += 0.1098 * addition;
			blue += 0.1412 * addition;
			red += 0.1333 * (addition * blackDiv);
			addition = cyan * magentaComplement * yellowComplement * blackComplement;
			green += 0.6784 * addition;
			blue += 0.9373 * addition;
			addition *= blackDiv;
			green += 0.0588 * addition;
			blue += 0.1412 * addition;
			addition = cyan * magentaComplement * yellow * blackComplement;
			green += 0.6510 * addition;
			blue += 0.3137 * addition;
			green += 0.0745 * (cyan * magentaComplement * yellow * black);
			addition = cyan * magenta * yellowComplement * blackComplement;
			red += 0.1804 * addition;
			green += 0.1922 * addition;
			blue += 0.5725 * addition;
			blue += 0.0078 * (addition * blackDiv);
			addition = cyan * magenta * yellow * blackComplement;
			int d = 16581375;
			return new byte[] { (byte)((red + 0.2118 * addition) / d), (byte)((green + 0.2119 * addition) / d), (byte)((blue + 0.2235 * addition)/d) };
		}
		internal static PdfRGBColor FromCMYK(double cyan, double magenta, double yellow, double black) {
			if (black >= 1)
				return blackColor;
			double cyanComplement = 1 - cyan;
			double magentaComplement = 1 - magenta;
			double yellowComplement = 1 - yellow;
			double blackComplement = 1 - black;
			double blackDiv = black / blackComplement;
			double addition = cyanComplement * magentaComplement * yellowComplement * blackComplement;
			double red = addition;
			double green = addition;
			double blue = addition;
			addition *= blackDiv;
			red += 0.1373 * addition;
			green += 0.1216 * addition;
			blue += 0.1255 * addition;
			addition = cyanComplement * magentaComplement * yellow * blackComplement;
			red += addition;
			green += 0.9490 * addition;
			addition *= blackDiv;
			red += 0.1098 * addition;
			green += 0.1020 * addition;
			addition = cyanComplement * magenta * yellowComplement * blackComplement;
			red += 0.9255 * addition;
			blue += 0.5490 * addition;
			red += 0.1412 * (addition * blackDiv);
			addition = cyanComplement * magenta * yellow * blackComplement;
			red += 0.9294 * addition;
			green += 0.1098 * addition;
			blue += 0.1412 * addition;
			red += 0.1333 * (addition * blackDiv);
			addition = cyan * magentaComplement * yellowComplement * blackComplement;
			green += 0.6784 * addition;
			blue += 0.9373 * addition;
			addition *= blackDiv;
			green += 0.0588 * addition;
			blue += 0.1412 * addition;
			addition = cyan * magentaComplement * yellow * blackComplement;
			green += 0.6510 * addition;
			blue += 0.3137 * addition;
			green += 0.0745 * (cyan * magentaComplement * yellow * black);
			addition = cyan * magenta * yellowComplement * blackComplement;
			red += 0.1804 * addition;
			green += 0.1922 * addition;
			blue += 0.5725 * addition;
			blue += 0.0078 * (addition * blackDiv);
			addition = cyan * magenta * yellow * blackComplement;
			return Create(PdfColor.ClipColorComponent(red + 0.2118 * addition), PdfColor.ClipColorComponent(green + 0.2119 * addition), PdfColor.ClipColorComponent(blue + 0.2235 * addition));
		}
		internal static PdfRGBColor FromColor(PdfColor color) {
			double[] components = color.Components;
			switch (components.Length) {
				case 1:
					double component = components[0];
					return Create(component, component, component);
				case 3:
					return Create(components[0], components[1], components[2]);
				case 4:
					return FromCMYK(components[0], components[1], components[2], components[3]);
				default:
					return blackColor;
			}
		}
		internal static PdfRGBColor Create(double red, double green, double blue) {
			return new PdfRGBColor() { red = red, green = green, blue = blue };
		}
		double red;
		double green;
		double blue;
		public double R {
			get { return red; }
			set {
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("R", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectColorComponentValue));
				red = value;
			}
		}
		public double G {
			get { return green; }
			set {
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("G", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectColorComponentValue));
				green = value;
			}
		}
		public double B {
			get { return blue; }
			set {
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("B", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgIncorrectColorComponentValue));
				blue = value;
			}
		}
	}
}
