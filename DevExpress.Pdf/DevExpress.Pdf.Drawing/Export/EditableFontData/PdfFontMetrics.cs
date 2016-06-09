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
	public class PdfFontMetrics {
		readonly double ascent;
		readonly double descent;
		readonly double lineSpacing;
		readonly PdfRectangle emBBox;
		public double EmAscent { get { return ascent * 1000; } }
		public double EmDescent { get { return descent * 1000; } }
		public PdfRectangle EmBBox { get { return emBBox; } }
		public PdfFontMetrics(double ascent, double descent, double lineSpacing, double unitsPerEm) {
			this.ascent = ascent / unitsPerEm;
			this.descent = descent / unitsPerEm;
			this.lineSpacing = lineSpacing / unitsPerEm;
			emBBox = new PdfRectangle(0, 0, 0, 0);
		}
		public PdfFontMetrics(PdfFontFile fontFile) {
			if (fontFile == null)
				throw new ArgumentNullException("fontFile");
			PdfFontOS2TableDirectoryEntry os2 = fontFile.OS2;
			PdfFontHeadTableDirectoryEntry head = fontFile.Head;
			PdfFontHheaTableDirectoryEntry hhea = fontFile.Hhea;
			double emSize = head == null ? 2048 : head.UnitsPerEm;
			if (os2 != null)
				lineSpacing = 1 + os2.TypoLineGap / emSize;
			if (hhea != null) {
				ascent = hhea.Ascender / emSize;
				descent = -hhea.Descender / emSize;
			}
			double factor = 1000 / emSize;
			if (head != null)
				emBBox = new PdfRectangle(Math.Round(head.XMin * factor), Math.Round(head.YMin * factor), Math.Round(head.XMax * factor), Math.Round(head.YMax * factor));
			else
				emBBox = new PdfRectangle(0, 0, 0, 0);
		}
		public double GetAscent(double fontSize) {
			return ascent * fontSize;
		}
		public double GetDescent(double fontSize) {
			return descent * fontSize;
		}
		public double GetLineSpacing(double fontSize) {
			return lineSpacing * fontSize;
		}
	}
	public class PdfGraphicsFontMetrics : PdfFontMetrics {
		public PdfGraphicsFontMetrics(FontFamily family, FontStyle style)
			: base(family.GetCellAscent(style), family.GetCellDescent(style), family.GetLineSpacing(style), family.GetEmHeight(style)) {
		}
	}
}
