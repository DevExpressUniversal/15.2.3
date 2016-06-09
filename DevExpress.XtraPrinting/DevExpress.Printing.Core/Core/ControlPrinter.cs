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
using System.Drawing;
using DevExpress.XtraPrinting;
#if SL
using DevExpress.Xpf.Drawing;
using DevExpress.Xpf.Drawing.Printing;
#else
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Windows.Forms;
#endif
namespace DevExpress.XtraReports.UI {
	[
	Flags,
	Serializable,
	]
	public enum XRBorderSide_Utils { None = 0, Top = 1, Bottom = 2, Left = 4, Right = 8 }
	public enum ReportUnit_Utils { HundredthsOfAnInch, TenthsOfAMillimeter, Pixels }
}
namespace DevExpress.XtraReports.UI {
	public class XRConvert : GraphicsUnitConverter {
		#region static
		static object[] psBorderSides = { BorderSide.None, BorderSide.Left, BorderSide.Top, BorderSide.Right, BorderSide.Bottom };
		static object[] xrBorderSides = { XRBorderSide_Utils.None, XRBorderSide_Utils.Left, XRBorderSide_Utils.Top, XRBorderSide_Utils.Right, XRBorderSide_Utils.Bottom };
		public static ContentAlignment ToContentAlignment(TextAlignment align) {
			if(align >= TextAlignment.TopJustify) {
				if(align == TextAlignment.TopJustify)
					return ContentAlignment.TopLeft;
				else if(align == TextAlignment.MiddleJustify)
					return ContentAlignment.MiddleLeft;
				else
					return ContentAlignment.BottomLeft;
			} else
				return (ContentAlignment)align;
		}
		public static TextAlignment ToTextAlignment(ContentAlignment align) {
			return (TextAlignment)align;
		}
		public static BorderSide ToBorderSide(XRBorderSide_Utils val) {
			BorderSide side = BorderSide.None;
			for(int i = 0; i < xrBorderSides.Length; i++)
				if((val & (XRBorderSide_Utils)xrBorderSides[i]) != 0)
					side |= (BorderSide)psBorderSides[i];
			return side;
		}
		public static XRBorderSide_Utils ToXRBorderSide(BorderSide val) {
			XRBorderSide_Utils side = XRBorderSide_Utils.None;
			for(int i = 0; i < psBorderSides.Length; i++)
				if((val & (BorderSide)psBorderSides[i]) != 0)
					side |= (XRBorderSide_Utils)xrBorderSides[i];
			return side;
		}
		public static Margins ConvertMargins(Margins val, float fromDpi, float toDpi) {
			Rectangle rect = Convert(Rectangle.FromLTRB(val.Left, val.Top, val.Right, val.Bottom), fromDpi, toDpi);
			return new Margins(rect.Left, rect.Right, rect.Top, rect.Bottom);
		}
		public static float UnitToDpi(ReportUnit_Utils reportUnit) {
			switch(reportUnit) {
				case ReportUnit_Utils.HundredthsOfAnInch: return GraphicsDpi.HundredthsOfAnInch;
				case ReportUnit_Utils.TenthsOfAMillimeter: return GraphicsDpi.TenthsOfAMillimeter;
				case ReportUnit_Utils.Pixels: return GraphicsDpi.DeviceIndependentPixel;
			}
			throw new NotSupportedException();
		}
		public static string StringArrayToString(string[] array) {
			if(array == null || array.Length <= 0)
				return String.Empty;
			System.Text.StringBuilder sb = new System.Text.StringBuilder(array[0]);
			int count = array.Length;
			for(int i = 1; i < count; i++) {
				sb.Append("\r\n");
				sb.Append(array[i]);
			}
			return sb.ToString();
		}
		public static string[] StringToStringArray(string str) {
			return (str != null) ? System.Text.RegularExpressions.Regex.Split(str, "\r\n") : new string[] { };
		}
		#endregion
		private float dpi = GraphicsDpi.HundredthsOfAnInch;
		public virtual float Dpi {
			get { return dpi; }
		}
		public XRConvert(ReportUnit_Utils unit) {
			this.dpi = UnitToDpi(unit);
		}
		public XRConvert(float dpi) {
			this.dpi = dpi;
		}
		public int ConvertFrom(int val, float dpi) {
			return System.Convert.ToInt32(Convert(val, dpi, Dpi));
		}
		public Point ConvertFrom(Point val, float dpi) {
			return Point.Round(Convert(val, dpi, Dpi));
		}
		public Size ConvertFrom(Size val, float dpi) {
			return Size.Round(Convert(val, dpi, Dpi));
		}
		public Rectangle ConvertFrom(Rectangle val, float dpi) {
			return Convert(val, dpi, Dpi);
		}
		public int ConvertTo(int val, float dpi) {
			return Convert(val, Dpi, dpi);
		}
		public Point ConvertTo(Point val, float dpi) {
			return Convert(val, Dpi, dpi);
		}
		public Size ConvertTo(Size val, float dpi) {
			return Convert(val, Dpi, dpi);
		}
		public Rectangle ConvertTo(Rectangle val, float dpi) {
			return Convert(val, Dpi, dpi);
		}
	}
}
