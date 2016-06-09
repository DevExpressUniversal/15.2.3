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
using DevExpress.Pdf.Native;
using DevExpress.Pdf.Localization;
namespace DevExpress.Pdf {
	public class PdfColor {
		internal static double ClipColorComponent(double component) {
			return PdfMathUtils.Min(1, PdfMathUtils.Max(0, component));
		}
		internal static PdfColor FromXYZ(double x, double y, double z, double whitePointZ) {
			double red;
			double green;
			double blue;
			if (whitePointZ < 1) {
				red = x * 3.1339 + y * -1.6170 + z * -0.4906;
				green = x * -0.9785 + y * 1.9160 + z * 0.0333;
				blue = x * 0.0720 + y * -0.2290 + z * 1.4057;
			}
			else {
				red = x * 3.2406 + y * -1.5372 + z * -0.4986;
				green = x * -0.9689 + y * 1.8758 + z * 0.0415;
				blue = x * 0.0557 + y * -0.2040 + z * 1.0570;
			}
			return new PdfColor(ColorComponentTransferFunction(red), ColorComponentTransferFunction(green), ColorComponentTransferFunction(blue));
		}
		static double ColorComponentTransferFunction(double component) {
			component = ClipColorComponent(component);
			return ClipColorComponent(component > 0.0031308 ? (Math.Pow(component, 1 / 2.4) * 1.055 - 0.055) : (component * 12.92));
		}
		readonly PdfPattern pattern;
		readonly double[] components;
		public PdfPattern Pattern { get { return pattern; } }
		public double[] Components { get { return components; } }
		public PdfColor(PdfPattern pattern, params double[] components) {
			if (pattern == null && components.Length == 0)
				throw new ArgumentOutOfRangeException("components", PdfCoreLocalizer.GetString(PdfCoreStringId.MsgZeroColorComponentsCount));
			this.pattern = pattern;
			this.components = components;
		}
		public PdfColor(params double[] components) : this(null, components) {
		}
		internal object ToWritableObject() {
			return new PdfWritableDoubleArray(components);
		}
	}
}
