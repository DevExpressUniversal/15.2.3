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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public struct PdfEditorBorder {
		readonly PdfEditorBorderStyle borderStyle;
		readonly Color color;
		readonly double borderWidth;
		readonly double horizontalRadius;
		readonly double verticalRadius;
		readonly double[] dashPattern;
		readonly double dashPhase;
		public PdfEditorBorderStyle BorderStyle { get { return borderStyle; } }
		public Color Color { get { return color; } }
		public double BorderWidth { get { return borderWidth; } }
		public double HorizontalRadius { get { return horizontalRadius; } }
		public double VerticalRadius { get { return verticalRadius; } }
		public double[] DashPattern { get { return dashPattern; } }
		public double DashPhase { get { return dashPhase; } }
		public PdfEditorBorder(PdfWidgetAnnotation widget) {
			borderStyle = PdfEditorBorderStyle.Solid;
			borderWidth = 1;
			horizontalRadius = 0;
			verticalRadius = 0;
			dashPattern = null;
			dashPhase = 0;
			PdfWidgetAppearanceCharacteristics characteristics = widget.AppearanceCharacteristics;
			PdfColor pdfBorderColor = characteristics == null ? null : characteristics.BorderColor;
			if (pdfBorderColor == null)
				color = Color.Empty;
			else {
				PdfRGBColor rgbColor = PdfRGBColor.FromColor(pdfBorderColor);
				color = Color.FromArgb(255, Convert.ToByte(rgbColor.R * 255), Convert.ToByte(rgbColor.G * 255), Convert.ToByte(rgbColor.B * 255));
			}
			if (widget.BorderStyle != null) {
				PdfAnnotationBorderStyle annotationBorderStyle = widget.BorderStyle;
				switch (annotationBorderStyle.StyleName) {
					case "D":
						borderStyle = PdfEditorBorderStyle.Dashed;
						PdfLineStyle lineStyle = annotationBorderStyle.LineStyle;
						if (lineStyle != null) {
							dashPattern = lineStyle.DashPattern;
							dashPhase = lineStyle.DashPhase;
						}
						break;
					case "B":
						borderStyle = PdfEditorBorderStyle.Beveled;
						break;
					case "I":
						borderStyle = PdfEditorBorderStyle.Inset;
						break;
					case "U":
						borderStyle = PdfEditorBorderStyle.Underline;
						break;
					default:
						borderStyle = PdfEditorBorderStyle.Solid;
						break;
				}
				borderWidth = color.IsEmpty ? 0 : annotationBorderStyle.Width;
			}
			else if (widget.Border != null) {
				PdfAnnotationBorder border = widget.Border;
				horizontalRadius = border.HorizontalCornerRadius;
				verticalRadius = border.VerticalCornerRadius;
				borderWidth = color.IsEmpty ? 0 : border.LineWidth;
				PdfLineStyle lineStyle = border.LineStyle;
				if (lineStyle != null && lineStyle.IsDashed) {
					borderStyle = PdfEditorBorderStyle.Dashed;
					dashPattern = lineStyle.DashPattern;
				}
			}
		}
	}
}
